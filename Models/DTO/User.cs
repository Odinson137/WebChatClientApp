namespace WebChatClientApp.Models.DTO
{
    public class User
    {
        public string UserName{ get; set; }
        public string Password { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string Token { get; set; }
    }
}
