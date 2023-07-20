using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTGenerator
{
    public static class JwtProcessor
    {
        // randomly generated securityKey that is also found in other projects appsettings.json file.
        private static readonly SymmetricSecurityKey securityKey = new(Encoding.ASCII.GetBytes("Iyg17SD2f7tUhV1cRxEox3WqqUwJH2I0PRLefDOYp3SsIQOg2+E4SPUGiJnF2bK0"));
        private static readonly string issuer = "https://localhost:7117";
        private static readonly string audience = "SignalRBlazorChatApp";

        public static string GetJwtToken()
        {
            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claimsForToken = new()
            {
                new("userId", "e08b0077-3c15-477e-84bb-bf9d41196455")
            };

            JwtSecurityToken jwtSecurityToken = new(
                issuer,
                audience,
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
