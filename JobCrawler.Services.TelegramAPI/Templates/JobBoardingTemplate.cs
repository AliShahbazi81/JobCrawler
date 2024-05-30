using System.Globalization;
using JobCrawler.Services.Crawler.DTO;

namespace JobCrawler.Services.TelegramAPI.Templates;

public static class JobBoardingTemplate
{
    public static string CreateJobMessage(JobDto job)
    {
        var formattedSkills = FormatSkills(job.JobDescription);
        
        return $"<b>Title: {job.Title}</b>\n" +
               $"<i>Company: {job.Company}</i>\n" +
               $"<i>Location: {job.Location}</i>\n\n" +
               $"<b>Posted Date:</b> {job.PostedDate}\n" +
               $"<b>Number of Applicants:</b> {job.NumberOfEmployees}\n\n" +
               $"<b>Skills:</b>\n{formattedSkills}\n";
    }
    
    private static string FormatSkills(string? jobDescription)
    {
        if (string.IsNullOrWhiteSpace(jobDescription) || jobDescription == "N/A")
        {
            return "N/A";
        }

        var skills = jobDescription.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(skill => skill.Trim())
            .Select(skill => $"• #{CapitalizeFirstLetter(skill)}");

        return string.Join("\n", skills);
    }

    private static string CapitalizeFirstLetter(string input)
    {
        return string.IsNullOrWhiteSpace(input) 
            ? input 
            : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }
}