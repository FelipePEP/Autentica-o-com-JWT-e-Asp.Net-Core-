using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using aspNetCore.Repository;
using aspNetCore.Infra;

namespace aspNetCore
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient<UserRepository>();
			var authConfig = new AuthConfiguration();
			services.AddSingleton(authConfig);
			var tokenConfig = new TokenConfigurations();
			new ConfigureFromConfigurationOptions<TokenConfigurations>(
				Configuration.GetSection("TokenConfigurations"))
				.Configure(tokenConfig);
			services.AddSingleton(tokenConfig);
			services.AddAuthentication(authOptions =>
			{
				authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(bearerOptions =>
			{
				var paramsValidation = bearerOptions.TokenValidationParameters;
				paramsValidation.IssuerSigningKey = authConfig.Key;
				paramsValidation.ValidAudience = tokenConfig.Audience;
				paramsValidation.ValidIssuer = tokenConfig.Issuer;
				// Valida a assinatura de um token recebido
				paramsValidation.ValidateIssuerSigningKey = true;
				// Verifica se um token recebido ainda é válido
				paramsValidation.ValidateLifetime = true;
				// Expiração limite Para o caso de problemas de sincronismo entre PCs diferentes na comunicação)
				paramsValidation.ClockSkew = TimeSpan.Zero;
			});
			// Ativa o uso do token como forma de autorização
			services.AddAuthorization(auth =>
			{
				auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
					.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser().Build());
			});

			services.AddMvc();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseMvc();
		}
	}
}
