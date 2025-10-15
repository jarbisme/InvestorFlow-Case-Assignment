namespace MinimalAPI.Models.DTOs
{
    public class CreateContactRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}