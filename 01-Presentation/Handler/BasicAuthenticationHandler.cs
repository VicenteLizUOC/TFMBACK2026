using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using TFGBack._03_Infrastructure.Data;

namespace TFGBack._01_Presentation.Handler
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly DataContext _dataContext;

        public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        DataContext dataContext
        ) : base(options, logger, encoder, clock)
        {
            _dataContext = dataContext;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization headers");
            }

            try
            {
                var uthenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(uthenticationHeaderValue.Parameter);
                var credentials = Encoding.UTF8.GetString(bytes).Split(":");
                string email = credentials[0];
                var password = credentials[1];


                var user = _dataContext.Users.Where(user => user.Email == email).FirstOrDefault(); //BUSCAR EN EL DATACONETXT DE Users Y COMPARAR AMBOS
                if (user == null)
                {
                    return AuthenticateResult.Fail("Invalid email");
                }
                else
                {

                    bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);

                    if (isPasswordCorrect)
                    {
                        var claims = new[] { new Claim(ClaimTypes.Name, user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email.ToString()) };
                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return AuthenticateResult.Success(ticket);

                    }
                    else
                    {
                        return AuthenticateResult.Fail("Invalid password");
                    }
                }
            }
            catch (Exception)
            {
                return AuthenticateResult.Fail("Fail");
            }
        }
    }
}
