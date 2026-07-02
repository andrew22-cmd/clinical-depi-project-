namespace clinicsystem.ViewModels
{
    public class ConfirmEmailViewModel
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;

        // Shown only when confirmation failed (invalid/expired token) so the
        // user can request a fresh link without re-typing their email from scratch.
        public string? Email { get; set; }
    }
}
