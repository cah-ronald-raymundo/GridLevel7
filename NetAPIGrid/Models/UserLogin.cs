namespace NetAPIGrid.Models
{
    public class UserRequest
    {
        public string? Username { get; set; }
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
