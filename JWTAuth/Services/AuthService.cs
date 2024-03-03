using JWTAuth.Models;

namespace JWTAuth.Services;

public interface IAuthService
{
    public Task<UserLoginResponse> LoginUserAsync(UserLoginRequest request);
}

public class AuthService(ITokenService tokenService) : IAuthService
{
    private List<User> _usersTable =
    [
        new User()
        {
            Id = 999,
            Username = "admin@store.com",
            Password = "p", // Hashed Password :p
            Email = "admin@store.com",
            Role = "Admin",
            FirstName = "FirstName",
            LastName = "LastName",
        }
    ];

    public async Task<UserLoginResponse> LoginUserAsync(UserLoginRequest req)
    {
        if (string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Password))
        {
            throw new ArgumentNullException(nameof(req));
        }

        // Hash the password
        // req.Password = req.Password.EncryptRijndael();
        
        var dbUser = _usersTable.Find(x => x.Username.Equals(req.Username) && x.Password.Equals(req.Password));

        if (dbUser != null)
        {
            var tokenResponse = await tokenService.GenerateToken(new TokenRequest()
                { Username = req.Username, ExpireIn = 120 });

            var response = new UserLoginResponse()
            {
                User = dbUser,
                Authenticate = true,
                Token = tokenResponse.Token,
                TokenExpireIn = tokenResponse.TokenExpireIn,
                TokenExpireDate = tokenResponse.TokenExpireUtc,
            };
            response.User.Password = string.Empty;
            return await Task.FromResult(response);
        }

        throw new BadHttpRequestException(nameof(req));
    }
}