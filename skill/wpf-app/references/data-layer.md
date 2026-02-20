# Data Layer Reference

## Table of Contents
1. [When to Use SQLite](#when-to-use-sqlite)
2. [EF Core Setup](#ef-core-setup)
3. [DbContext Pattern](#dbcontext-pattern)
4. [Repository Pattern](#repository-pattern)
5. [Migrations](#migrations)
6. [Performance Tips](#performance-tips)

---

## When to Use SQLite

Use SQLite when the app:
- Manages structured data that persists between sessions
- Has lists, records, logs, or inventory to store
- Needs search/filter/sort over data
- Works offline (no server dependency)

Don't use SQLite when:
- Only simple key-value settings are needed (use `appsettings.json` or `IOptions<T>`)
- Data is tiny and changes rarely (use JSON file)
- App is a viewer/tool with no state

---

## EF Core Setup

### Additional NuGet packages

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.*">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

### Database Location

Store the database in `%LocalAppData%/AppName/`:

```csharp
public static string GetDatabasePath()
{
    var folder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AppName");
    Directory.CreateDirectory(folder);
    return Path.Combine(folder, "appdata.db");
}
```

Never store the database in the application directory — it may be read-only, and uninstalling
the app would delete user data.

---

## DbContext Pattern

```csharp
using Microsoft.EntityFrameworkCore;

namespace AppName.Data;

public class AppDbContext : DbContext
{
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Category> Categories => Set<Category>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entity relationships and constraints
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");

            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Items)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.Name);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });
    }
}
```

### Model Entities

```csharp
namespace AppName.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Item> Items { get; set; } = [];
}
```

### DI Registration

```csharp
// In App.xaml.cs ConfigureServices:
services.AddDbContext<AppDbContext>(options =>
{
    var dbPath = AppDbContext.GetDatabasePath(); // or use a helper
    options.UseSqlite($"Data Source={dbPath}");
#if DEBUG
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
#endif
});
```

### Auto-Migration on Startup

Apply migrations automatically when the app starts. This keeps the database schema
in sync without requiring the user to run CLI commands:

```csharp
// In App.xaml.cs OnStartup, after host starts:
using var scope = Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.MigrateAsync();
```

---

## Repository Pattern

For apps with moderate complexity, add a repository layer between services and EF Core.
This makes testing easier and prevents DbContext leaking into ViewModels.

```csharp
// Data/Repositories/IRepository.cs
namespace AppName.Data.Repositories;

public interface IRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

// Data/Repositories/Repository.cs
namespace AppName.Data.Repositories;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
{
    protected readonly AppDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.AsNoTracking().ToListAsync(ct);

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _dbSet.FindAsync([id], ct);

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _dbSet.FindAsync([id], ct);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
```

Register:
```csharp
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

---

## Migrations

### Creating Migrations

From the project directory (where .csproj is):

```bash
dotnet ef migrations add InitialCreate
dotnet ef migrations add AddCategoryTable
```

### Design-Time Factory

EF Core tools need a way to create DbContext at design time:

```csharp
// Data/DesignTimeDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AppName.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=design-time.db")
            .Options;
        return new AppDbContext(options);
    }
}
```

---

## Performance Tips

### Use AsNoTracking for Read-Only Queries

```csharp
// Good — no change tracking overhead
var items = await db.Items.AsNoTracking().ToListAsync();

// Bad for read-only — tracks entities in memory unnecessarily
var items = await db.Items.ToListAsync();
```

### Project Only What You Need

```csharp
// Good — only fetches Name and Id
var summaries = await db.Items
    .AsNoTracking()
    .Select(i => new { i.Id, i.Name })
    .ToListAsync();

// Bad — fetches entire entity with all navigation properties
var items = await db.Items.Include(i => i.Category).ToListAsync();
```

### Batch Operations

```csharp
// Good — single round trip
await db.Items.Where(i => i.IsArchived).ExecuteDeleteAsync();

// Bad — N+1 queries
var items = await db.Items.Where(i => i.IsArchived).ToListAsync();
foreach (var item in items)
    db.Items.Remove(item);
await db.SaveChangesAsync();
```

### Index Frequently Filtered Columns

```csharp
// In OnModelCreating:
entity.HasIndex(e => e.Name);
entity.HasIndex(e => e.CreatedAt);
entity.HasIndex(e => new { e.CategoryId, e.CreatedAt }); // composite
```

### Connection Pooling

SQLite with EF Core uses connection pooling by default. Don't manually open/close connections.
The scoped lifetime of DbContext handles this correctly.
