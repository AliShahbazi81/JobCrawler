<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
</head>
<body>
    <h1>JobCrawler</h1>
    <p>JobCrawler is a robust job searching bot designed to find and aggregate job postings from popular job board platforms such as LinkedIn, Glassdoor, and Indeed. It filters jobs based on their post time and sends them to a configured Telegram channel.</p>
    <h2>Features</h2>
    <ul>
        <li><strong>Job Scraping:</strong> Retrieves job listings from LinkedIn, Glassdoor, and Indeed.</li>
        <li><strong>Telegram Integration:</strong> Sends job details to a specified Telegram channel.</li>
        <li><strong>Filtering:</strong> Filters jobs based on posting time to ensure the latest opportunities.</li>
        <li><strong>Configurable:</strong> Easy configuration through application settings.</li>
    </ul>
    <h2>Technologies Used</h2>
    <ul>
        <li><strong>.NET Core 8 WebAPI:</strong> The backend framework for building the API.</li>
        <li><strong>Services and Interfaces:</strong> Modular design with services handling core functionalities.</li>
        <li><strong>Telegram Bot API:</strong> Integration for sending messages to Telegram channels.</li>
        <li><strong>Dependency Injection:</strong> Ensures scalable and maintainable code.</li>
    </ul>
    <h2>Installation</h2>
    <ol>
        <li>Clone the repository:
            <pre><code>git clone https://github.com/AliShahbazi81/JobCrawler.git
cd JobCrawler</code></pre>
        </li>
        <li>Update <code>appsettings.json</code> with your specific configurations, especially the Telegram API settings.</li>
        <li>Restore the necessary packages:
            <pre><code>dotnet restore</code></pre>
        </li>
        <li>Build the project:
            <pre><code>dotnet build</code></pre>
        </li>
    </ol>
    <h2>Usage</h2>
    <ol>
        <li>Run the application:
            <pre><code>dotnet run</code></pre>
        </li>
        <li>Monitor the console output for logs indicating successful job scraping and Telegram message dispatches.</li>
    </ol>
    <h2>Configuration</h2>
    <p>The application settings are located in <code>appsettings.json</code>. Configure your Telegram bot settings under the <code>TelegramConfigs</code> section:</p>
    <pre><code>{
  "TelegramConfigs": {
    "BotToken": "YOUR_TELEGRAM_BOT_TOKEN",
    "ChannelId": "YOUR_TELEGRAM_CHANNEL_ID"
  }
}</code></pre>
    <h2>Project Structure</h2>
    <ul>
        <li><strong>JobCrawler.Domain:</strong> Contains the domain entities and business logic.</li>
        <li><strong>JobCrawler.Infrastructure.Crawler:</strong> Handles the job scraping functionalities.</li>
        <li><strong>JobCrawler.Services.Crawler:</strong> Manages the interaction with job board platforms.</li>
        <li><strong>JobCrawler.Services.TelegramAPI:</strong> Manages the Telegram Bot API integration.</li>
    </ul>
    <h2>Contributing</h2>
    <ol>
        <li>Fork the repository.</li>
        <li>Create a new feature branch (<code>git checkout -b feature/AmazingFeature</code>).</li>
        <li>Commit your changes (<code>git commit -m 'Add some AmazingFeature'</code>).</li>
        <li>Push to the branch (<code>git push origin feature/AmazingFeature</code>).</li>
        <li>Open a pull request.</li>
    </ol>
    <h2>License</h2>
    <p>This project is licensed under the MIT License. See the <code>LICENSE</code> file for details.</p>
    <h2>Contact</h2>
    <p>For any questions or feedback, please contact Ali Shahbazi at <strong>Ali@shahbazi.me</strong> or open an issue on this repository.</p>
</body>
</html>
