using UnityEngine;
using System;

public static class ReportDataManager
{
    private const float IDEAL_REACTION_TIME = 1.2f;

    private static string TodayKey()
    {
        return DateTime.Now.ToString("yyyyMMdd");
    }

    private static string WeekKey()
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        int week = culture.Calendar.GetWeekOfYear(
            DateTime.Now,
            System.Globalization.CalendarWeekRule.FirstDay,
            DayOfWeek.Monday
        );

        return DateTime.Now.Year + "_W" + week;
    }

    private static string DailyPrefix(string gameName)
    {
        return "Daily_" + TodayKey() + "_" + gameName + "_";
    }

    private static string WeeklyPrefix(string gameName)
    {
        return "Weekly_" + WeekKey() + "_" + gameName + "_";
    }

    public static void SaveGameReport(
        string gameName,
        float gameDuration,
        float focusedTime,
        float unfocusedTime,
        int focusedCorrectActions,
        int focusedWrongActions,
        int unfocusedCorrectActions,
        int unfocusedWrongActions,
        int correctActions,
        int wrongActions,
        int missedActions,
        float totalReactionTime,
        int reactionCount
    )
    {
        SaveToPrefix(DailyPrefix(gameName), gameDuration, focusedTime, unfocusedTime,
            focusedCorrectActions, focusedWrongActions, unfocusedCorrectActions, unfocusedWrongActions,
            correctActions, wrongActions, missedActions, totalReactionTime, reactionCount);

        SaveToPrefix(WeeklyPrefix(gameName), gameDuration, focusedTime, unfocusedTime,
            focusedCorrectActions, focusedWrongActions, unfocusedCorrectActions, unfocusedWrongActions,
            correctActions, wrongActions, missedActions, totalReactionTime, reactionCount);

        PlayerPrefs.Save();
    }

    private static void SaveToPrefix(
        string prefix,
        float gameDuration,
        float focusedTime,
        float unfocusedTime,
        int focusedCorrectActions,
        int focusedWrongActions,
        int unfocusedCorrectActions,
        int unfocusedWrongActions,
        int correctActions,
        int wrongActions,
        int missedActions,
        float totalReactionTime,
        int reactionCount
    )
    {
        PlayerPrefs.SetFloat(prefix + "GameDuration", PlayerPrefs.GetFloat(prefix + "GameDuration", 0) + gameDuration);
        PlayerPrefs.SetFloat(prefix + "FocusedTime", PlayerPrefs.GetFloat(prefix + "FocusedTime", 0) + focusedTime);
        PlayerPrefs.SetFloat(prefix + "UnfocusedTime", PlayerPrefs.GetFloat(prefix + "UnfocusedTime", 0) + unfocusedTime);

        PlayerPrefs.SetInt(prefix + "FocusedCorrectActions", PlayerPrefs.GetInt(prefix + "FocusedCorrectActions", 0) + focusedCorrectActions);
        PlayerPrefs.SetInt(prefix + "FocusedWrongActions", PlayerPrefs.GetInt(prefix + "FocusedWrongActions", 0) + focusedWrongActions);
        PlayerPrefs.SetInt(prefix + "UnfocusedCorrectActions", PlayerPrefs.GetInt(prefix + "UnfocusedCorrectActions", 0) + unfocusedCorrectActions);
        PlayerPrefs.SetInt(prefix + "UnfocusedWrongActions", PlayerPrefs.GetInt(prefix + "UnfocusedWrongActions", 0) + unfocusedWrongActions);

        PlayerPrefs.SetInt(prefix + "CorrectActions", PlayerPrefs.GetInt(prefix + "CorrectActions", 0) + correctActions);
        PlayerPrefs.SetInt(prefix + "WrongActions", PlayerPrefs.GetInt(prefix + "WrongActions", 0) + wrongActions);
        PlayerPrefs.SetInt(prefix + "MissedActions", PlayerPrefs.GetInt(prefix + "MissedActions", 0) + missedActions);

        PlayerPrefs.SetFloat(prefix + "TotalReactionTime", PlayerPrefs.GetFloat(prefix + "TotalReactionTime", 0) + totalReactionTime);
        PlayerPrefs.SetInt(prefix + "ReactionCount", PlayerPrefs.GetInt(prefix + "ReactionCount", 0) + reactionCount);
    }

    public static string GetReportText(string gameName, bool weekly)
    {
        string prefix = weekly ? WeeklyPrefix(gameName) : DailyPrefix(gameName);

        float duration = PlayerPrefs.GetFloat(prefix + "GameDuration", 0);
        float focused = PlayerPrefs.GetFloat(prefix + "FocusedTime", 0);
        float unfocused = PlayerPrefs.GetFloat(prefix + "UnfocusedTime", 0);

        int focusedCorrectActions = PlayerPrefs.GetInt(prefix + "FocusedCorrectActions", 0);
        int focusedWrongActions = PlayerPrefs.GetInt(prefix + "FocusedWrongActions", 0);
        int unfocusedCorrectActions = PlayerPrefs.GetInt(prefix + "UnfocusedCorrectActions", 0);
        int unfocusedWrongActions = PlayerPrefs.GetInt(prefix + "UnfocusedWrongActions", 0);

        int missedActions = PlayerPrefs.GetInt(prefix + "MissedActions", 0);

        float totalReactionTime = PlayerPrefs.GetFloat(prefix + "TotalReactionTime", 0);
        int reactionCount = PlayerPrefs.GetInt(prefix + "ReactionCount", 0);

        float totalAttentionTime = focused + unfocused;
        float attentionRatio = totalAttentionTime > 0 ? (focused / totalAttentionTime) * 100f : 0f;
        float averageReaction = reactionCount > 0 ? totalReactionTime / reactionCount : 0f;

        float advancedAttentionScore = CalculateAdvancedAttentionScore(
            gameName,
            focused,
            unfocused,
            focusedCorrectActions,
            focusedWrongActions,
            unfocusedCorrectActions,
            unfocusedWrongActions,
            missedActions,
            totalReactionTime,
            reactionCount
        );

        string report =
            gameName + "\n" +
            "Played Time: " + FormatTime(duration) + "\n" +
            "Focused Time: " + FormatTime(focused) + "\n" +
            "Unfocused Time: " + FormatTime(unfocused) + "\n" +
            "Attention Ratio: " + attentionRatio.ToString("F1") + "%\n" +
            "Advanced Attention Score: " + advancedAttentionScore.ToString("F1") + "% " + GetScoreScaleInfo(gameName) + "\n" +
            "Attention Quality: " + GetAdvancedAttentionComment(advancedAttentionScore, duration) + "\n" +
            "Focused Correct Attempts: " + focusedCorrectActions + "\n" +
            "Focused Wrong Attempts: " + focusedWrongActions + "\n" +
            "Unfocused Correct Attempts: " + unfocusedCorrectActions + "\n" +
            "Unfocused Wrong Attempts: " + unfocusedWrongActions;

        if (gameName == "Game3")
            report += "\nMissed Actions: " + missedActions;

        if (reactionCount > 0)
            report += "\nAverage Reaction: " + averageReaction.ToString("F2") + " s";

        return report;
    }

    public static string GetGeneralReport(bool weekly)
    {
        string[] games = { "Game1", "Game2", "Game3", "Game4" };

        float totalDuration = 0;
        float totalFocused = 0;
        float totalUnfocused = 0;
        float totalReactionTime = 0;

        int totalFocusedCorrect = 0;
        int totalFocusedWrong = 0;
        int totalUnfocusedCorrect = 0;
        int totalUnfocusedWrong = 0;
        int totalMissed = 0;
        int totalReactionCount = 0;

        foreach (string game in games)
        {
            string prefix = weekly ? WeeklyPrefix(game) : DailyPrefix(game);

            totalDuration += PlayerPrefs.GetFloat(prefix + "GameDuration", 0);
            totalFocused += PlayerPrefs.GetFloat(prefix + "FocusedTime", 0);
            totalUnfocused += PlayerPrefs.GetFloat(prefix + "UnfocusedTime", 0);

            totalFocusedCorrect += PlayerPrefs.GetInt(prefix + "FocusedCorrectActions", 0);
            totalFocusedWrong += PlayerPrefs.GetInt(prefix + "FocusedWrongActions", 0);
            totalUnfocusedCorrect += PlayerPrefs.GetInt(prefix + "UnfocusedCorrectActions", 0);
            totalUnfocusedWrong += PlayerPrefs.GetInt(prefix + "UnfocusedWrongActions", 0);

            totalReactionTime += PlayerPrefs.GetFloat(prefix + "TotalReactionTime", 0);
            totalReactionCount += PlayerPrefs.GetInt(prefix + "ReactionCount", 0);

            if (game == "Game3")
                totalMissed += PlayerPrefs.GetInt(prefix + "MissedActions", 0);
        }

        float totalAttentionTime = totalFocused + totalUnfocused;
        float ratio = totalAttentionTime > 0 ? (totalFocused / totalAttentionTime) * 100f : 0f;

        float advancedAttentionScore = CalculateAdvancedAttentionScore(
            "General",
            totalFocused,
            totalUnfocused,
            totalFocusedCorrect,
            totalFocusedWrong,
            totalUnfocusedCorrect,
            totalUnfocusedWrong,
            totalMissed,
            totalReactionTime,
            totalReactionCount
        );

        return
            "GENERAL REPORT\n" +
            "Total Played Time: " + FormatTime(totalDuration) + "\n" +
            "Total Focused Time: " + FormatTime(totalFocused) + "\n" +
            "Total Unfocused Time: " + FormatTime(totalUnfocused) + "\n" +
            "General Attention Ratio: " + ratio.ToString("F1") + "%\n" +
            "General Advanced Attention Score: " + advancedAttentionScore.ToString("F1") + "% (Combined score from all game-specific attention metrics)\n" +
            "General Attention Quality: " + GetAdvancedAttentionComment(advancedAttentionScore, totalDuration) + "\n" +
            "Total Focused Correct Attempts: " + totalFocusedCorrect + "\n" +
            "Total Focused Wrong Attempts: " + totalFocusedWrong + "\n" +
            "Total Unfocused Correct Attempts: " + totalUnfocusedCorrect + "\n" +
            "Total Unfocused Wrong Attempts: " + totalUnfocusedWrong + "\n" +
            "Total Missed Actions: " + totalMissed + "\n" +
            GetAttentionComment(ratio, totalDuration);
    }

    private static float CalculateAdvancedAttentionScore(
        string gameName,
        float focusedTime,
        float unfocusedTime,
        int focusedCorrectActions,
        int focusedWrongActions,
        int unfocusedCorrectActions,
        int unfocusedWrongActions,
        int missedActions,
        float totalReactionTime,
        int reactionCount
    )
    {
        float totalTime = focusedTime + unfocusedTime;

        int totalCorrectActions = focusedCorrectActions + unfocusedCorrectActions;
        int totalWrongActions = focusedWrongActions + unfocusedWrongActions;

        int totalAttempts =
            focusedCorrectActions +
            focusedWrongActions +
            unfocusedCorrectActions +
            unfocusedWrongActions;

        int totalOpportunities = totalAttempts + missedActions;

        if (totalTime <= 0f || totalAttempts <= 0)
            return 0f;

        float focusRatio = focusedTime / totalTime;

        float accuracyQuality = totalOpportunities > 0
            ? (float)totalCorrectActions / totalOpportunities
            : 0f;

        float inhibitionControl = totalAttempts > 0
            ? 1f - ((float)totalWrongActions / totalAttempts)
            : 0f;

        float unfocusedControl = totalAttempts > 0
            ? 1f - ((float)(unfocusedCorrectActions + unfocusedWrongActions) / totalAttempts)
            : 0f;

        float missControl = totalOpportunities > 0
            ? 1f - ((float)missedActions / totalOpportunities)
            : 0f;

        float reactionQuality = 0f;

        if (reactionCount > 0)
        {
            float averageReaction = totalReactionTime / reactionCount;

            reactionQuality = averageReaction > 0
                ? IDEAL_REACTION_TIME / averageReaction
                : 0f;
        }

        focusRatio = Mathf.Clamp01(focusRatio);
        accuracyQuality = Mathf.Clamp01(accuracyQuality);
        inhibitionControl = Mathf.Clamp01(inhibitionControl);
        unfocusedControl = Mathf.Clamp01(unfocusedControl);
        missControl = Mathf.Clamp01(missControl);
        reactionQuality = Mathf.Clamp01(reactionQuality);

        float score;

        switch (gameName)
        {
            case "Game1":
                score =
                    (focusRatio * 0.35f) +
                    (accuracyQuality * 0.35f) +
                    (inhibitionControl * 0.20f) +
                    (unfocusedControl * 0.10f);
                break;

            case "Game2":
                score =
                    (focusRatio * 0.30f) +
                    (accuracyQuality * 0.40f) +
                    (inhibitionControl * 0.20f) +
                    (unfocusedControl * 0.10f);
                break;

            case "Game3":
                score =
                    (focusRatio * 0.25f) +
                    (accuracyQuality * 0.20f) +
                    (inhibitionControl * 0.15f) +
                    (reactionQuality * 0.25f) +
                    (missControl * 0.15f);
                break;

            case "Game4":
                score =
                    (focusRatio * 0.25f) +
                    (accuracyQuality * 0.35f) +
                    (inhibitionControl * 0.25f) +
                    (unfocusedControl * 0.15f);
                break;

            default:
                score =
                    (focusRatio * 0.30f) +
                    (accuracyQuality * 0.30f) +
                    (inhibitionControl * 0.20f) +
                    (reactionQuality * 0.10f) +
                    (missControl * 0.10f);
                break;
        }

        return Mathf.Clamp01(score) * 100f;
    }

    private static string GetScoreScaleInfo(string gameName)
    {
        switch (gameName)
        {
            case "Game1":
                return "(Focus, Accuracy, Inhibition, Unfocused Control)";

            case "Game2":
                return "(Focus, Accuracy, Decision Accuracy, Inhibition)";

            case "Game3":
                return "(Focus, Accuracy, Reaction Speed, Miss Control)";

            case "Game4":
                return "(Focus, Accuracy, Memory Control, Inhibition)";

            default:
                return "(General Attention Performance)";
        }
    }

    private static string GetAttentionComment(float ratio, float duration)
    {
        if (duration < 60f)
            return "Comment: Not enough play time for a reliable analysis.";

        if (ratio >= 80f)
            return "Comment: Attention level is strong.";

        if (ratio >= 60f)
            return "Comment: Attention level is moderate.";

        return "Comment: Attention level needs improvement.";
    }

    private static string GetAdvancedAttentionComment(float score, float duration)
    {
        if (duration < 60f)
            return "Not enough play time";

        if (score >= 80f)
            return "Strong";

        if (score >= 60f)
            return "Moderate";

        return "Needs improvement";
    }

    private static string FormatTime(float seconds)
    {
        if (seconds < 60f)
            return seconds.ToString("F1") + " sec";

        int minutes = Mathf.FloorToInt(seconds / 60f);
        float secs = seconds % 60f;

        return minutes + " min " + secs.ToString("F1") + " sec";
    }
}