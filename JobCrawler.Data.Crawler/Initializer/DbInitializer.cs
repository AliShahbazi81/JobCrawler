using JobCrawler.Data.Crawler.Context;
using JobCrawler.Data.Crawler.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobCrawler.Data.Crawler.Initializer;

public static class DbInitializer
{
    public static async Task Initialize(IDbContextFactory<ApplicationDbContext> context)
    {
        await SeedCountries(context);
        await SeedKeywords(context);
    }

    private static async Task SeedKeywords(IDbContextFactory<ApplicationDbContext> context)
    {
        // Seed keywords for Software Development in Fields
        await using var dbContext = await context.CreateDbContextAsync();
        if (dbContext.Keywords.Any())
        {
            return;
        }

        /*
         *".NET", "Java", "C#", "Python", "Django", "Flask", "FastAPI", "C++", "Laravel", "PHP", "Ruby", "Ruby on Rails", "Swift", "Perl", "GoLang", "NodeJs", "ExpressJs", "ASP.NET",

           // Frontend Development
           "HTML", "CSS", "JavaScript", "React", "NextJs", "Angular", "VueJs", "TypeScript",

           // Database
           "SQL", "NoSQL", "MongoDB", "PostgreSQL", "MySQL", "SQLite", "Redis", "RabbitMQ", "Kafka", "Elasticsearch",

           // Tools and Technologies
           "AWS", "Azure", "Docker", "Kubernetes", "Jenkins", "Git", "GitHub", "GitLab", "Bitbucket", "Jira", "Confluence", "Slack", "Trello", "Azure DevOps", "AWS CodePipeline", "AWS CodeBuild", "AWS CodeDeploy", "AWS CodeCommit", "AWS CodeStar", "AWS CodeArtifact", "AWS CodeGuru", "Ubuntu", "Debian", "CentOS", "RedHat", "Fedora", "Windows", "MacOS", "Linux", "Unix", "Shell Scripting", "PowerShell", "Bash", "Zsh", "Terraform", "Ansible", "Chef", "Puppet", "SaltStack", "Nginx", "Apache", "IIS", "Logstash", "Kibana", "Prometheus", "Grafana", "Splunk", "Datadog", "New Relic", "Sentry", "AppDynamics", "Dynatrace", "Postman", "Swagger", "OpenAPI", "REST", "GraphQL", "gRPC", "SOAP", "WebSockets", "WebRTC", "OAuth", "JWT", "SAML", "OpenID", "LDAP", "Active Directory",

           // Others
           "OAuth2", "OIDC", "SAML2", "OpenID2", "LDAP2", "Active Directory2", "OAuth3", "OIDC3", "SAML3", "OpenID3", "LDAP3", "Active Directory3", "OAuth4", "OIDC4", "SAML4", "OpenID4", "LDAP4", "Active Directory4", "OAuth5", "OIDC5", "SAML5", "OpenID5", "LDAP5", "Active Directory5", "OAuth6", "OIDC6", "SAML6", "OpenID6", "LDAP6", "Active Directory6", "OAuth7", "OIDC7", "SAML7", "OpenID7", "LDAP7", "Active Directory7", "OAuth8", "OIDC8", "SAML8", "OpenID8", "LDAP8", "Active Directory8", "OAuth9", "OIDC9", "SAML9", "OpenID9", "LDAP9", "Active Directory9", "OAuth10", "OIDC10", "SAML10", "OpenID10", "LDAP10", "Active Directory10", "OAuth11", "OIDC11", "SAML11", "OpenID11", "LDAP11", "Active Directory11", "OAuth12", "OIDC12", "SAML12", "OpenID12", "LDAP12", "Active Directory"

         *
         */

        dbContext.Keywords.AddRange(
        [
            new Keyword { Name = ".net" },
            new Keyword { Name = "java" },
            new Keyword { Name = "c#" },
            new Keyword { Name = "python" },
            new Keyword { Name = "django" },
            new Keyword { Name = "flask" },
            new Keyword { Name = "fastapi" },
            new Keyword { Name = "c++" },
            new Keyword { Name = "laravel" },
            new Keyword { Name = "php", },
            new Keyword { Name = "ruby" },
            new Keyword { Name = "ruby on rails" },
            new Keyword { Name = "swift" },
            new Keyword { Name = "perl" },
            new Keyword { Name = "golang" },
            new Keyword { Name = "nodejs" },
            new Keyword { Name = "expressjs" },
            new Keyword { Name = "asp.net" },
            new Keyword { Name = "html" },
            new Keyword { Name = "css" },
            new Keyword { Name = "javascript" },
            new Keyword { Name = "react" },
            new Keyword { Name = "nextjs" },
            new Keyword { Name = "angular" },
            new Keyword { Name = "vuejs" },
            new Keyword { Name = "typescript" },
            new Keyword { Name = "sql" },
            new Keyword { Name = "nosql" },
            new Keyword { Name = "mongodb" },
            new Keyword { Name = "postgresql" },
            new Keyword { Name = "mysql" },
            new Keyword { Name = "sqlite" },
            new Keyword { Name = "redis" },
            new Keyword { Name = "rabbitmq" },
            new Keyword { Name = "kafka" },
            new Keyword { Name = "elasticsearch" },
            new Keyword { Name = "aws" },
            new Keyword { Name = "azure" },
            new Keyword { Name = "docker" },
            new Keyword { Name = "kubernetes" },
            new Keyword { Name = "jenkins" },
            new Keyword { Name = "git" },
            new Keyword { Name = "github" },
            new Keyword { Name = "gitlab" },
            new Keyword { Name = "bitbucket" },
            new Keyword { Name = "jira" },
            new Keyword { Name = "confluence" },
            new Keyword { Name = "slack" },
            new Keyword { Name = "trello" },
            new Keyword { Name = "azure devops" },
            new Keyword { Name = "aws codepipeline" },
            new Keyword { Name = "aws codebuild" },
            new Keyword { Name = "aws codedeploy" },
            new Keyword { Name = "aws codecommit" },
            new Keyword { Name = "aws codestar" },
            new Keyword { Name = "aws codeartifact" },
            new Keyword { Name = "aws codeguru" },
            new Keyword { Name = "ubuntu" },
            new Keyword { Name = "debian" },
            new Keyword { Name = "centos" },
            new Keyword { Name = "redhat" },
            new Keyword { Name = "fedora" },
            new Keyword { Name = "windows" },
            new Keyword { Name = "macos" },
            new Keyword { Name = "linux" },
            new Keyword { Name = "unix" },
            new Keyword { Name = "shell scripting" },
            new Keyword { Name = "powershell" },
            new Keyword { Name = "bash" },
            new Keyword { Name = "zsh" },
            new Keyword { Name = "terraform" },
            new Keyword { Name = "ansible" },
            new Keyword { Name = "chef" },
            new Keyword { Name = "puppet" },
            new Keyword { Name = "saltstack" },
            new Keyword { Name = "nginx" },
            new Keyword { Name = "apache" },
            new Keyword { Name = "iis" },
            new Keyword { Name = "logstash" },
            new Keyword { Name = "kibana" },
            new Keyword { Name = "prometheus" },
            new Keyword { Name = "grafana" },
            new Keyword { Name = "splunk" },
            new Keyword { Name = "datadog" },
            new Keyword { Name = "new relic" },
            new Keyword { Name = "sentry" },
            new Keyword { Name = "appdynamics" },
            new Keyword { Name = "dynatrace" },
            new Keyword { Name = "postman" },
            new Keyword { Name = "swagger" },
            new Keyword { Name = "openapi" },
            new Keyword { Name = "rest" },
            new Keyword { Name = "graphql" },
            new Keyword { Name = "grpc" },
            new Keyword { Name = "soap" },
            new Keyword { Name = "websockets" },
            new Keyword { Name = "webrtc" },
            new Keyword { Name = "oauth" },
            new Keyword { Name = "jwt" },
            new Keyword { Name = "saml" },
            new Keyword { Name = "openid" },
            new Keyword { Name = "ldap" },
            new Keyword { Name = "active directory" },
            new Keyword { Name = "oauth2" },
            new Keyword { Name = "oidc" },
            new Keyword { Name = "saml2" },
            new Keyword { Name = "openid2" },
            new Keyword { Name = "ldap2" },
            new Keyword { Name = "active directory2" },
            new Keyword { Name = "oauth3" },
            new Keyword { Name = "oidc3" },
            new Keyword { Name = "saml3" },
            new Keyword { Name = "openid3" },
            new Keyword { Name = "ldap3" },
            new Keyword { Name = "active directory3" },
            new Keyword { Name = "oauth4" },
            new Keyword { Name = "oidc4" },
            new Keyword { Name = "saml4" },
            new Keyword { Name = "openid4" },
            new Keyword { Name = "ldap4" },
            new Keyword { Name = "active directory4" },
            new Keyword { Name = "oauth5" },
            new Keyword { Name = "oidc5" },
            new Keyword { Name = "saml5" },
            new Keyword { Name = "openid5" },
            new Keyword { Name = "ldap5" },
            new Keyword { Name = "active directory5" },
        ]);

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedCountries(IDbContextFactory<ApplicationDbContext> context)
    {
        await using var dbContext = await context.CreateDbContextAsync();
        if (dbContext.Countries.Any())
        {
            return;
        }

        dbContext.Countries.AddRange(
            new Country { Name = "Canada" },
            new Country { Name = "Australia" },
            new Country { Name = "Netherlands" },
            new Country { Name = "Germany" });

        await dbContext.SaveChangesAsync();
    }
}