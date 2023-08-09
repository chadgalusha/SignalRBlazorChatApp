﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SignalRBlazorChatApp.Helpers
{
	public class JwtGenerator
	{
		private readonly IConfiguration _configuration;

		public JwtGenerator(IConfiguration configuration)
		{
			_configuration = configuration ?? throw new Exception(nameof(configuration));
		}

		public string GetJwtToken()
		{
			Authentication authentication = GetAuthentication();

			SymmetricSecurityKey securityKey = GetSecurityKey(authentication.SecretForKey);

			SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

			List<Claim> claimsForToken = new()
			{
				new("userId", ""),
				new("userRole", "")
			};

			JwtSecurityToken jwtSecurityToken = new(
				authentication.Issuer,
				authentication.Audience,
				claimsForToken,
				DateTime.UtcNow,
				DateTime.UtcNow.AddMinutes(1),
				signingCredentials
				);

			return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
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
				Issuer		 = _configuration["Authentication:Issuer"]!,
				Audience	 = _configuration["Authentication:Audience"]!
			};
		}

		private static SymmetricSecurityKey GetSecurityKey(string key)
		{
			return new(Encoding.ASCII.GetBytes(key));
		}

		#endregion
	}
}
