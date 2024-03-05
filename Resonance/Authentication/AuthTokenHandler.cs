using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using ResoClassAPI.Models.Domain;

namespace ResoClassAPI.Authentication
{
    public class AuthTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly string AuthorizationText = "Authorization";
        private IConfiguration _config;
        private readonly ResoClassContext dbContext;

        public AuthTokenHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration, ResoClassContext _dbContext) : 
            base(options, logger, encoder, clock)
        {
            this._config = configuration;
            dbContext = _dbContext;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey(AuthorizationText))
            {
                try
                {
                    var authHeader = AuthenticationHeaderValue.Parse(Request.Headers[AuthorizationText]);
                    var parts = authHeader.ToString().Split(' ');

                    return await ValidateToken(authHeader.ToString().Split(' ')[1]);
                }
                catch
                {
                    return AuthenticateResult.Fail("Invalid Access Token");
                }
            }
            else
                return AuthenticateResult.Fail("Access Token is missing");
        }

        private async Task<AuthenticateResult> ValidateToken(string accessToken)
        {
            AuthenticateResult result = AuthenticateResult.Fail("UnAuthorized User");


            if (!string.IsNullOrEmpty(accessToken))
            {
                var validationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:Key"]))
                };

                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(accessToken, validationParameters, out var validToken);
                JwtSecurityToken? token = validToken as JwtSecurityToken;

                if (token == null)
                {
                    result = AuthenticateResult.Fail("Invalid Token");
                }
                else
                {
                    var deviceId = token.Claims.Where(t => t.Type == "DeviceId").FirstOrDefault();

                    if (deviceId == null || string.IsNullOrEmpty(deviceId.Value))
                    {
                        //Web User - No need to check DeviceId
                        result = AuthenticateResult.Success(GetAuthenticationTicket(token.Claims.ToList()));
                    }
                    else
                    {
                        //Mobile User - need to check the deviceId with the student login
                        Student student = null;
                        var studentId = token.Claims.Where(t => t.Type == ClaimTypes.Sid).FirstOrDefault();
                        if (studentId != null && !string.IsNullOrEmpty(studentId.Value))
                            student = dbContext.Students.FirstOrDefault(x => x.Id == Convert.ToInt64(studentId.Value) && x.DeviceId == deviceId.Value.ToString());
                        
                        if (student != null)
                        {
                            result = AuthenticateResult.Success(GetAuthenticationTicket(token.Claims.ToList()));
                        }
                        else
                        {
                            result = AuthenticateResult.Fail("Device is not registered with the User");
                        }
                    }
                }

            }
            return await Task.FromResult(result);
        }

        private AuthenticationTicket GetAuthenticationTicket(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principle = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principle, Scheme.Name);

            return ticket;
        }
    }
}
