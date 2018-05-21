using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace aspNetCore.Infra
{
    public class AuthConfiguration
    {
		public SecurityKey Key { get; }
		public SigningCredentials SigningCredentials { get; }

		public AuthConfiguration()
		{
			using (var provider = new RSACryptoServiceProvider(2048))
			{
				Key = new RsaSecurityKey(provider.ExportParameters(true));
			}

			SigningCredentials = new SigningCredentials(
				Key, SecurityAlgorithms.RsaSha256Signature);
		}
	}
}
