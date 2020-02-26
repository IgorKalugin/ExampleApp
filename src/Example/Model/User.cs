namespace Example.Model
{
    public class User : IWithId
    {
        public int Id { get; set; }
        
        public string Email { get; set; }
        
        public bool IsLoggedIn { get; set; }
    }
}