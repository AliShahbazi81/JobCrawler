using System.Globalization;
using JobCrawler.Services.Crawler.DTO;

namespace JobCrawler.Services.TelegramAPI.Templates
{
    public static class JobBoardingTemplate
    {
        private static readonly string[] BackendKeywords =
        [
            "Dotnet", "Java", "Csharp", "Python", "Django", "Flask", "FastAPI", "C++", "Laravel", "PHP", "Ruby", "Ruby on Rails", "Swift", "C", "Perl", "GoLang", "NodeJs", "ExpressJs", "ASP.NET"
        ];

        private static readonly string[] FrontendKeywords =
        [
            "HTML", "CSS", "JavaScript", "React", "NextJs", "Angular", "VueJs", "TypeScript"
        ];

        private static readonly string[] DatabaseKeywords =
        [
            "SQL", "NoSQL", "MongoDB", "PostgreSQL", "MySQL", "SQLite", "Redis", "RabbitMQ", "Kafka", "Elasticsearch"
        ];

        private static readonly string[] ToolsKeywords =
        [
            "AWS", "Azure", "Docker", "Kubernetes", "Jenkins", "Git", "GitHub", "GitLab", "Bitbucket", "Jira", "Confluence", "Slack", "Trello", "Azure DevOps", "AWS CodePipeline", "AWS CodeBuild", "AWS CodeDeploy", "AWS CodeCommit", "AWS CodeStar", "AWS CodeArtifact", "AWS CodeGuru", "Ubuntu", "Debian", "CentOS", "RedHat", "Fedora", "Windows", "MacOS", "Linux", "Unix", "Shell Scripting", "PowerShell", "Bash", "Zsh", "Terraform", "Ansible", "Chef", "Puppet", "SaltStack", "Nginx", "Apache", "IIS", "Logstash", "Kibana", "Prometheus", "Grafana", "Splunk", "Datadog", "New Relic", "Sentry", "AppDynamics", "Dynatrace", "Postman", "Swagger", "OpenAPI", "REST", "GraphQL", "gRPC", "SOAP", "WebSockets", "WebRTC", "OAuth", "JWT", "SAML", "OpenID", "LDAP", "Active Directory"
        ];

        public static string CreateJobMessage(JobDto job)
        {
            var categorizedSkills = CategorizeSkills(job.JobDescription);

            return $"🧾 <b>Title: {job.Title}</b>\n\n" +
                   $"💻 <b>{job.LocationType}</b> \n" +
                   $"🏢 <b>Company:</b> {job.Company}\n" +
                   $"📍 <b>Location:</b> {job.Location}\n\n" +
                   $"⏰ <b>Posted </b> {job.PostedDate}\n" +
                   $"🙋 <b>Applicants:</b> {job.NumberOfEmployees}\n\n" +
                   $"⭐️ <b>Requirements:</b>\n{categorizedSkills}\n";
        }

        private static string CategorizeSkills(string? jobDescription)
        {
            if (string.IsNullOrWhiteSpace(jobDescription) || jobDescription == "N/A")
            {
                return "N/A";
            }

            var skills = jobDescription.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(skill => skill.Trim())
                .Select(CapitalizeFirstLetter)
                .ToList();

            var backendSkills = skills.Where(skill => BackendKeywords.Contains(skill, StringComparer.OrdinalIgnoreCase)).ToList();
            var frontendSkills = skills.Where(skill => FrontendKeywords.Contains(skill, StringComparer.OrdinalIgnoreCase)).ToList();
            var databaseSkills = skills.Where(skill => DatabaseKeywords.Contains(skill, StringComparer.OrdinalIgnoreCase)).ToList();
            var toolsSkills = skills.Where(skill => ToolsKeywords.Contains(skill, StringComparer.OrdinalIgnoreCase)).ToList();
            var otherSkills = skills.Except(backendSkills).Except(frontendSkills).Except(databaseSkills).Except(toolsSkills).ToList();

            return FormatCategorizedSkills("Backend", backendSkills) +
                   FormatCategorizedSkills("Frontend", frontendSkills) +
                   FormatCategorizedSkills("Database", databaseSkills) +
                   FormatCategorizedSkills("Tools and Technologies", toolsSkills) +
                   FormatCategorizedSkills("Others", otherSkills);
        }

        private static string FormatCategorizedSkills(string category, List<string> skills)
        {
            if (skills.Count == 0) return string.Empty;
            var formattedSkills = skills.Select(skill => $"• #{skill}");
            return $"\n<b>{category}:</b>\n{string.Join("\n", formattedSkills)}\n";
        }

        private static string CapitalizeFirstLetter(string input)
        {
            return string.IsNullOrWhiteSpace(input) 
                ? input 
                : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }
    }
}
