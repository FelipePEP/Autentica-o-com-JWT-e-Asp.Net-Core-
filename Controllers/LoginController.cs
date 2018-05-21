using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using aspNetCore.Repository;
using aspNetCore.Infra;
using aspNetCore.Business.Models;

namespace aspNetCore.Controllers
{
	[Route("api/[controller]")]
	public class LoginController : Controller
	{
		[AllowAnonymous]
		[HttpPost]
		public object Post(
			[FromBody]User user,
			[FromServices]UserRepository userRepository,
			[FromServices]AuthConfiguration authConfiguration,
			[FromServices]TokenConfigurations tokenConfigurations)
		{
			bool validCredentials = false;
			if (user != null && !String.IsNullOrWhiteSpace(user.Login))
			{
				var userTemp = userRepository.GetUser(user.Login);
				validCredentials = (userTemp != null &&
					user.Login == userTemp.Login &&
					user.AccessKey == userTemp.AccessKey);
			}

			if (validCredentials)
			{
				ClaimsIdentity identity = new ClaimsIdentity(
					new GenericIdentity(user.Login, "Login"),
					new[] {
						new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
						new Claim(JwtRegisteredClaimNames.UniqueName, user.Login)
					}
				);

				DateTime createDate = DateTime.Now;
				DateTime expireDate = createDate +
					TimeSpan.FromSeconds(tokenConfigurations.Seconds);

				var handler = new JwtSecurityTokenHandler();
				var securityToken = handler.CreateToken(new SecurityTokenDescriptor
				{
					Issuer = tokenConfigurations.Issuer,
					Audience = tokenConfigurations.Audience,
					SigningCredentials = authConfiguration.SigningCredentials,
					Subject = identity,
					NotBefore = createDate,
					Expires = expireDate
				});
				var token = handler.WriteToken(securityToken);

				return new
				{
					authenticated = true,
					created = createDate.ToString("yyyy-MM-dd HH:mm:ss"),
					expiration = expireDate.ToString("yyyy-MM-dd HH:mm:ss"),
					accessToken = token,
					message = "OK"
				};
			}
			else
			{
				return new
				{
					authenticated = false,
					message = "Falha ao autenticar"
				};
			}
		}
	}
}