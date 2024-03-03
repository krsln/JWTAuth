using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWTAuth.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuth.Services;

public interface ITokenService
{
    public Task<TokenResponse> GenerateToken(TokenRequest request);
}

public class TokenService(IOptions<AppSettingsOptions> appSettings) : ITokenService
{
    private readonly AppSettingsOptions _appSettings = appSettings.Value;

    public Task<TokenResponse> GenerateToken(TokenRequest request)
    {
        // var secretKey = Configuration.GetSection("AppSettings:Secret").Value;
        // authentication successful so generate jwt token
        // user.Token = JwtUtils.GenerateToken(_appSettings.Secret, user, 40);

        if (_appSettings.Jwt != null)
        {
            // https://medium.com/@vndpal/how-to-implement-jwt-token-authentication-in-net-core-6-ab7f48470f5c
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(_appSettings.Jwt.Issuer, _appSettings.Jwt.Audience,
                claims: new List<Claim>
                {
                    new("userName", request.Username)
                },
                expires: DateTime.Now.AddMinutes(request.ExpireIn),
                signingCredentials: signingCredentials);

            var dateTimeNow = DateTime.UtcNow;
            return Task.FromResult(new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                TokenExpireIn = request.ExpireIn,
                TokenExpireUtc = dateTimeNow.Add(TimeSpan.FromMinutes(request.ExpireIn))
            });
        }

        throw new Exception("AppSettings Jwt Values Required");
    }
}

public class TokenResponse
{
    public string Token { get; set; }

    /// <summary>
    /// In minutes. 
    /// </summary>
    public int TokenExpireIn { get; set; }

    public DateTime TokenExpireUtc { get; set; }
}

public class TokenRequest
{
    public string Username { get; set; }

    /// <summary>
    /// In minutes. Default is `120`
    /// </summary>
    public int ExpireIn { get; set; } = 120;
}