using UnityEngine;
using TMPro;

public class ParentPanelManager : MonoBehaviour
{
    [Header("Header")]
    public TMP_Text titleText;
    [Header("Report Texts")]
    public TMP_Text generalReportText;
    public TMP_Text game1ReportText;
    public TMP_Text game2ReportText;
    public TMP_Text game3ReportText;
    public TMP_Text game4ReportText;

    void Start()
    {
        ShowDailyReport();
    }

    public void ShowDailyReport()
    {
        SetHeader("Daily Report");

        if (generalReportText != null)
            generalReportText.text = ReportDataManager.GetGeneralReport(false);

        if (game1ReportText != null)
            game1ReportText.text =
                ReportDataManager.GetReportText("Game1", false) +
                "\n\nAdvanced Attention: based on focused correct actions, focused wrong actions, unfocused correct attempts and unfocused wrong attempts.";

        if (game2ReportText != null)
            game2ReportText.text =
                ReportDataManager.GetReportText("Game2", false) +
                "\n\nAdvanced Attention: based on category accuracy, correct selections and wrong selections.";

        if (game3ReportText != null)
            game3ReportText.text =
                ReportDataManager.GetReportText("Game3", false) +
                "\n\nAdvanced Attention: based on reaction speed, focused responses, unfocused correct attempts and unfocused wrong attempts.";

        if (game4ReportText != null)
            game4ReportText.text =
                ReportDataManager.GetReportText("Game4", false) +
                "\n\nAdvanced Attention: based on sustained focus, correct actions, focused wrong actions and unfocused wrong attempts.";
    }

    public void ShowWeeklyReport()
    {
        SetHeader("Weekly Report");
        if (generalReportText != null)
            generalReportText.text = ReportDataManager.GetGeneralReport(true);

        if (game1ReportText != null)
            game1ReportText.text =
                ReportDataManager.GetReportText("Game1", true) +
                "\n\nAdvanced Attention: based on focused correct actions, focused wrong actions, unfocused correct attempts and unfocused wrong attempts.";

        if (game2ReportText != null)
            game2ReportText.text =
                ReportDataManager.GetReportText("Game2", true) +
                "\n\nAdvanced Attention: based on category accuracy, correct selections and wrong selections.";

        if (game3ReportText != null)
            game3ReportText.text =
                ReportDataManager.GetReportText("Game3", true) +
                "\n\nAdvanced Attention: based on reaction speed, focused responses, unfocused correct attempts and unfocused wrong attempts.";

        if (game4ReportText != null)
            game4ReportText.text =
                ReportDataManager.GetReportText("Game4", true) +
                "\n\nAdvanced Attention: based on sustained focus, correct actions, focused wrong actions and unfocused wrong attempts.";
    }

    private void SetHeader(string title)
    {
        if (titleText != null)
            titleText.text = title;
    }
}