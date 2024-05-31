namespace JobCrawler.Domain.Variables;

public static class SharedVariables
{
    public const int TimeIntervalSeconds = 1800;
    public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36";
    
    /* Pages nodes and classes */
    public const string JobCards = "//div[contains(@class, 'base-card') and contains(@class, 'job-search-card')]";
    
    public const string JobTitleNode = ".//h3[contains(@class, 'base-search-card__title')]";
    public const string CompanyNode = ".//h4[contains(@class, 'base-search-card__subtitle')]";
    public const string LocationNode = ".//span[contains(@class, 'job-search-card__location')]";
    public const string JobUrlNode = ".//a[contains(@class, 'base-card__full-link')]";
    public const string PostedDateNode = ".//time[contains(@class, 'job-search-card__listdate')]";
    
    public const string EmploymentTypeNode = "//span[contains(@class, 'description__job-criteria-text') and (text()='Full-time' or text()='Part-time')]";
    public const string LocationTypeNode = "//span[contains(@class, 'job-criteria__text') and (text()='Remote' or text()='On-site' or text()='Hybrid')]";
    public const string NumberOfEmployeesNode = "//span[contains(@class, 'num-applicants__caption')]";
    
    public const string JobDescriptionNode = ".description__text--rich";
    

}
