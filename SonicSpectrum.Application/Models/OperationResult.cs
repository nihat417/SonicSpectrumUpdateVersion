namespace SonicSpectrum.Application.Models
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? Message {  get; set; }
        public string? ErrorMessage { get; set; }
    }
}
