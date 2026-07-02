namespace clinicsystem.Services
{
    /// <summary>
    /// Bound from the "EmailSettings" section of appsettings.json.
    /// No credentials are ever hardcoded — everything comes from configuration,
    /// so switching between Gmail, Outlook, or any other SMTP provider is just
    /// a config change.
    /// </summary>
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }
}
