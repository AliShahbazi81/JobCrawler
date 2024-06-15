using JobCrawler.Data.Crawler.Context;
using JobCrawler.Data.Crawler.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobCrawler.Data.Crawler.Initializer;

public static class DbInitializer
{
    public static async Task Initialize(IDbContextFactory<ApplicationDbContext> context)
    {
        await SeedFields(context);
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

        var softwareDevelopment = await dbContext.Fields
            .FirstAsync(f => f.Name == "Software Development");

        dbContext.Keywords.AddRange(
        [
            new Keyword { Name = ".net", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "java", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "c#", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "python", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "django", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "flask", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "fastapi", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "c++", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "laravel", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "php", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "ruby", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "ruby on rails", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "swift", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "perl", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "golang", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "nodejs", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "expressjs", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "asp.net", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "html", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "css", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "javascript", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "react", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "nextjs", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "angular", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "vuejs", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "typescript", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "sql", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "nosql", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "mongodb", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "postgresql", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "mysql", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "sqlite", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "redis", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "rabbitmq", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "kafka", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "elasticsearch", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "aws", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "azure", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "docker", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "kubernetes", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "jenkins", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "git", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "github", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "gitlab", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "bitbucket", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "jira", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "confluence", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "slack", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "trello", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "azure devops", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "aws codepipeline", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "aws codebuild", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "aws codedeploy", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "aws codecommit", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "aws codestar", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "aws codeartifact", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "aws codeguru", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "ubuntu", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "debian", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "centos", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "redhat", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "fedora", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "windows", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "macos", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "linux", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "unix", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "shell scripting", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "powershell", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "bash", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "zsh", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "terraform", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "ansible", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "chef", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "puppet", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "saltstack", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "nginx", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "apache", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "iis", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "logstash", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "kibana", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "prometheus", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "grafana", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "splunk", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "datadog", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "new relic", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "sentry", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "appdynamics", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "dynatrace", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "postman", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "swagger", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "openapi", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "rest", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "graphql", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "grpc", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "soap", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "websockets", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "webrtc", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "oauth", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "jwt", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "saml", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "openid", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "ldap", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "active directory", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "oauth2", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "oidc", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "saml2", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "openid2", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "ldap2", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "active directory2", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "oauth3", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "oidc3", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "saml3", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "openid3", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "ldap3", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "active directory3", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "oauth4", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "oidc4", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "saml4", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "openid4", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "ldap4", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "active directory4", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "oauth5", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "oidc5", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "saml5", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "openid5", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "ldap5", FieldId = softwareDevelopment.Id },
            new Keyword { Name = "active directory5", FieldId = softwareDevelopment.Id },
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

    private static async Task SeedFields(IDbContextFactory<ApplicationDbContext> dbContext)
    {
        await using var context = await dbContext.CreateDbContextAsync();
        if (context.Fields.Any())
        {
            return;
        }

        context.Fields.AddRange(
            new Field { Name = "Software Development" });
        await context.SaveChangesAsync();
    }
}