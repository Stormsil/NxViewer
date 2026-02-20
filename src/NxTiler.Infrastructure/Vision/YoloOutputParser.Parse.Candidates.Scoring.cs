namespace NxTiler.Infrastructure.Vision;

internal sealed partial class YoloOutputParser
{
    private bool TryResolveCandidateScoreAndClass(
        Func<int, float> getValue,
        int features,
        float minConfidence,
        out int classId,
        out float confidence)
    {
        var noObjectness = SelectBestClass(getValue, start: 4, end: features);
        var withObjectness = SelectBestClass(getValue, start: 5, end: features);
        var objectness = Clamp01(getValue(4));
        var withObjectnessScore = withObjectness.Score * objectness;

        var useObjectness = withObjectness.ClassId >= 0 && withObjectnessScore > noObjectness.Score;
        confidence = useObjectness ? withObjectnessScore : noObjectness.Score;
        if (confidence < minConfidence)
        {
            classId = -1;
            return false;
        }

        classId = useObjectness
            ? withObjectness.ClassId
            : noObjectness.ClassId;
        return classId >= 0;
    }
}
