namespace LoginSignupFormWithPostgreSQL.Models
{
    public class User
    {
        public int Id { get; set; }
        public string EMail { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
    }
}
