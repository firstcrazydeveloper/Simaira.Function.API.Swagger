using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;

namespace Simaira.Function.API.Swagger.Middlewares
{
    public class JwtBearerMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly JwtBearerOptions _options;
        private readonly JwtSecurityTokenHandler _validator = new();
        private TokenValidationParameters? _tokenValidationParameters;
        private IEnumerable<string> _swaggerEndpoints;

        public JwtBearerMiddleware(IOptions<JwtBearerOptions> options, IOptions<List<string>> test)
        {
            _options = options.Value;
            _swaggerEndpoints = test.Value;
        }

        async Task<TokenValidationParameters> GetTokenValidationParameters(CancellationToken cancellationToken)
        {
            if (_tokenValidationParameters != null)
                return _tokenValidationParameters.Clone();

            if (String.IsNullOrWhiteSpace(_options.Authority))
                return _tokenValidationParameters = _options.TokenValidationParameters.Clone();

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{_options.Authority}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever()
            );

            var validationParameters = _options.TokenValidationParameters.Clone();
            var openIdConfig = await configurationManager.GetConfigurationAsync(cancellationToken);
            validationParameters.ValidIssuer = openIdConfig.Issuer;
            validationParameters.IssuerSigningKeys = openIdConfig.SigningKeys;
            return _tokenValidationParameters = validationParameters;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var req = await context.GetHttpRequestDataAsync();

            var funcName = context.FunctionDefinition.Name;

            bool isSwaggerEndpoint = _swaggerEndpoints.Any(name => name.Equals(funcName));

            if (!isSwaggerEndpoint)
            {

                if (req is null) return;

                if (!req.Headers.TryGetValues("Authorization", out var authHeaders) || !authHeaders.Any())
                {
                    var res = req.CreateResponse(HttpStatusCode.Unauthorized);
                    await res.WriteStringAsync("Missing Bearer Authorization");
                    context.GetInvocationResult().Value = res;
                    return;
                }

                var token = authHeaders.Single();

                if (token.StartsWith("Bearer ")) token = token.Substring(7);

                if (!_validator.CanReadToken(token))
                {
                    var res = req.CreateResponse(HttpStatusCode.Unauthorized);
                    await res.WriteStringAsync("Invalid Bearer token");
                    context.GetInvocationResult().Value = res;
                    return;
                }

                var validationParameters = await GetTokenValidationParameters(context.CancellationToken);

                try
                {
                    var principal = _validator.ValidateToken(token, validationParameters, out var validatedToken);

                    if (principal is null)
                    {
                        var res = req.CreateResponse(HttpStatusCode.Unauthorized);
                        await res.WriteStringAsync("Invalid Bearer token data");
                        context.GetInvocationResult().Value = res;
                        return;
                    }

                    context.Features.Set(principal);
                }
                catch
                {
                    var res = req.CreateResponse(HttpStatusCode.Unauthorized);
                    await res.WriteStringAsync("Failed to validate Bearer token");
                    context.GetInvocationResult().Value = res;
                    return;
                }
            }

            await next(context);
        }
    }
}
