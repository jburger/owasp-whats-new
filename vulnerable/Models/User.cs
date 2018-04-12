namespace vulnerable.Models {
    public class User {
        public string Username { get; set; } 
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
        public UserType Type { get; set; }
    }

    public enum UserType {
        Admin,
        User
    }
}