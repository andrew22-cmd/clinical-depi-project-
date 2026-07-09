namespace clinicsystem.ViewModels
{
    public class ConfirmEmailViewModel
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? Email { get; set; }
    }
}