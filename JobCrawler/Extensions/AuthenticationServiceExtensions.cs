namespace JobScrawler.Extensions;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddAuthenticationService(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
            // For development: allow CORS from localhost ports typically used by frontend frameworks
            services.AddCors(options =>
            {
                options.AddPolicy("cors", builder =>
                    builder.WithOrigins("http://localhost:3000") // Adjust this to match your frontend's URL in development
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
        else // Production settings
            // For production: specify the actual domain name of your frontend
            services.AddCors(options =>
            {
                options.AddPolicy("cors", builder =>
                    builder.WithOrigins("https://yourproductiondomain.com") // Replace with your production frontend domain
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

        // Configure other authentication services here if necessary

        return services;
    }
}