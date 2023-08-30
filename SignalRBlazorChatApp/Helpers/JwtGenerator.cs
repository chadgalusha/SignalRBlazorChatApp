using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SignalRBlazorChatApp.Helpers
{
	public class JwtGenerator : IJwtGenerator
	{
		private readonly IConfiguration _configuration;

		public JwtGenerator(IConfiguration configuration)
		{
			_configuration = configuration ?? throw new Exception(nameof(configuration));
		}

		public string GetJwtToken(AuthenticationState authState)
		{
			(string, string) userIdAndRole = GetUserIdAndRole(authState);
			return CreateJWT(userIdAndRole.Item1, userIdAndRole.Item2);
		}

		#region PRIVATE METHODS

		private class Authentication
		{
			public string SecretForKey { get; set; } = string.Empty;
			public string Issuer { get; set; } = string.Empty;
			public string Audience { get; set; } = string.Empty;
		}

		private Authentication GetAuthentication()
		{
			return new()
			{
				SecretForKey = _configuration["Authentication:SecretForKey"]!,
				Issuer = _configuration["Authentication:Issuer"]!,
				Audience = _configuration["Authentication:Audience"]!
			};
		}

		private static SymmetricSecurityKey GetSecurityKey(string key)
		{
			return new(Encoding.ASCII.GetBytes(key));
		}

		public (string, string) GetUserIdAndRole(AuthenticationState authState)
		{
            var user = authState.User;
            var claims = user.Claims;

            string userId = claims.First(c => c.Type == ClaimTypes.NameIdentifier)
				.ToString();

            string userRole = claims.Where(c => c.Type == ClaimTypes.Role)
                .Select(_ => _.Value).First();

            if (userId.IsNullOrEmpty())
            {
                userId = "";
            }
            if (userRole.IsNullOrEmpty())
            {
                userRole = "";
            }

            return (userId, userRole)!;
        }

		private string CreateJWT(string userId, string userRole)
		{
            Authentication authentication = GetAuthentication();

            SymmetricSecurityKey securityKey = GetSecurityKey(authentication.SecretForKey);

            SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claimsForToken = new()
            {
                new("userId", userId),
                new("userRole", userRole)
            };

            JwtSecurityToken jwtSecurityToken = new(
                authentication.Issuer,
                authentication.Audience,
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddSeconds(20),
                signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

		#endregion
	}
}
