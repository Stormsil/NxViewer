namespace NxTiler.Infrastructure.Vision;

internal sealed partial class YoloOutputParser
{
    private static (int ClassId, float Score) SelectBestClass(Func<int, float> getValue, int start, int end)
    {
        if (start >= end)
        {
            return (-1, 0f);
        }

        var bestIndex = -1;
        var bestScore = 0f;
        for (var i = start; i < end; i++)
        {
            var score = Clamp01(getValue(i));
            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i - start;
            }
        }

        return (bestIndex, bestScore);
    }

    private static float Clamp(float value, float min, float max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }

    private static float Clamp01(float value) => Clamp(value, 0f, 1f);
}
