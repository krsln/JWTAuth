namespace JWTAuth.Models;

public class UserLoginResponse
{
    public User User { get; set; }
    
    public bool Authenticate { get; set; }
    public string Token { get; set; }
    
    public DateTime TokenExpireDate { get; set; }
    public int TokenExpireIn { get; set; }
}