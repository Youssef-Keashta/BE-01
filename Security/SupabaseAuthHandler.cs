using System.Security.Claims;
using System.Text.Encodings.Web;
using BE_01.Data;
using BE_01.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace BE_01.Security
{
    public class SupabaseAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly SupabaseAuthService _authService;

        public SupabaseAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            SupabaseAuthService authService)
            : base(options, logger, encoder)
        {
            _authService = authService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return AuthenticateResult.Fail("Access token required");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("Access token required");
            }

            var (valid, body) = await _authService.GetUser(token);

            if (!valid)
            {
                return AuthenticateResult.Fail("Invalid or expired token");
            }

            var claims = new[] { new Claim("access_token", token), new Claim("supabase_user", body) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = 401;
            Response.ContentType = "application/json";
            var message = properties?.Items.TryGetValue("error", out var err) == true ? err : "Access token required";
            await Response.WriteAsync($"{{\"error\":\"{message}\"}}");
        }
    }
}