using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.OpenApi;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Simaira.Function.API.Swagger.Middlewares;
using Microsoft.Extensions.Configuration;

//namespace Simaira.Function.API.Swagger
//{
//    internal class Start
//    {
//    }
//}


//[assembly: FunctionsStartup(typeof(Simaira.Function.API.Swagger.Startup))]

namespace Simaira.Function.API.Swagger
{
    // Note: When Function is In Process Model
    //class Startup : FunctionsStartup
    //{
    //    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    //    {

    //    }

    //    public override void Configure(IFunctionsHostBuilder builder)
    //    {
    //    }
    //}


    // Note: When Function is In Isolate Model
    public static class Program
    {
        public async static Task  Main()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    
                })                 
                .ConfigureFunctionsWebApplication( app =>
                {
                    // app.UseFunctionExecutionMiddleware();
                    app.UseMiddleware<JwtBearerMiddleware>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();
                    services.AddSwagger();
                    services.Configure<JwtBearerOptions>(options =>
                    {
                        options.Audience = "abhishek-sahil";
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = OnAuthenticationFailedContext()
                        };

                    });

                    services.Configure<List<string>>(options =>
                    {
                        options.Add("SwaggerUi");
                        options.Add("SwaggerYaml");
                        options.Add("SwaggerJson");
                        options.Add("SwaggerOAuth2Redirect");

                    });

                    services.AddAuthentication();
                    services.AddAuthorization();
                })
                .Build();

            await host.RunAsync();           

        }

        public static void AddSwagger(this IServiceCollection services)
        {
            //Register the extension
            // https://github.com/vitalybibikov/AzureExtensions.Swashbuckle
            services.AddSwashBuckle(opts =>
            {
                // If you want to add Newtonsoft support insert next line
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                opts.AddNewtonsoftSupport = true;
                opts.RoutePrefix = "api";
                opts.SpecVersion = OpenApiSpecVersion.OpenApi3_0;
                opts.AddCodeParameter = true;
                opts.PrependOperationWithRoutePrefix = true;
                opts.XmlPath = xmlPath;
                opts.Documents = new[]
                {
                new SwaggerDocument
                {
                    Name = "v1",
                    Title = "Simaira Function API Swagger",
                    Description = "Simaira Function API Swagger",
                    Version = "v2"
                },
                new SwaggerDocument
                {
                    Name = "v2",
                    Title = "Simaira Function API Swagger",
                    Description = "Simaira Function API Swagger",
                    Version = "v2"
                }
            };
                opts.Title = "Simaira Function API Swagger";

                //opts.OverridenPathToSwaggerJson = new Uri("http://localhost:7071/api/Swagger/json");

                // TO DO: https://medium.com/@codewithankitsahu/authentication-and-authorization-in-net-8-web-api-94dda49516ee
                // Outh 2
                //opts.AddOauth2();

                // JWT Bearer Token
                opts.AddJWTToken();


            });

        }

        public static void AddOauth2(this SwaggerDocOptions swaggerOptions)
        {
            swaggerOptions.ConfigureSwaggerGen = x =>
            {
                //custom operation example
                x.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
                    ? methodInfo.Name
                    : new Guid().ToString());

                //custom filter example
                //x.DocumentFilter<RemoveSchemasFilter>();

                //oauth2
                x.AddSecurityDefinition("oauth2",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri("https://your.idserver.net/connect/authorize"),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "api.read", "Access read operations" },
                                    { "api.write", "Access write operations" }
                                }
                            }
                        }
                    });
            };

            // set up your client ID if your API is protected
            swaggerOptions.ClientId = "your.client.id";
            swaggerOptions.OAuth2RedirectPath = "http://localhost:7071/api/swagger/oauth2-redirect";

        }

        public static void AddJWTToken(this SwaggerDocOptions swaggerOptions)
        {
            swaggerOptions.ConfigureSwaggerGen = x =>
            {
                //custom operation example
                x.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
                    ? methodInfo.Name
                    : new Guid().ToString());

                //custom filter example
                //x.DocumentFilter<RemoveSchemasFilter>();

                //oauth2
                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            };

            // set up your client ID if your API is protected
            swaggerOptions.ClientId = "your.client.id";
            swaggerOptions.OAuth2RedirectPath = "http://localhost:7071/api/swagger/oauth2-redirect";

        }


        private static Func<AuthenticationFailedContext, Task> OnAuthenticationFailedContext()
        {
            return ctx =>
            {
                if (ctx.HttpContext.Request != null)
                {
                    var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILogger>();
                    logger.LogError(0, ctx.Exception, "Token validation failed");
                }

                return Task.CompletedTask;
            };
        }
    }
}
