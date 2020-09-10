using System;
using System.Text;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AgriLogBackend.Startup))]

namespace AgriLogBackend
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,


                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("GuessThePasswordToThisSiteAndGetACookieFromMeOrIGetACookieFromYou"))
                    }
                });
        }
    }
}
