using AutoMapper;
using SolarLab.BusManager.Abstraction;
using SolarLab.BusManager.Implementation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SevSharks.Gateway.WebApi.Middleware;

namespace SevSharks.Gateway.WebApi
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// IConfiguration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        [UsedImplicitly]
        public void ConfigureServices(IServiceCollection services)
        {
            InstallWebHostCoreServices(services);
            InstallConfiguration(services);
            InstallHttpClientServices(services);
            InstallAutomapper(services);
            InstallBus(services);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="env">IHostingEnvironment</param>
        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseAuthentication();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }

        #region Private members

        private void InstallBus(IServiceCollection services)
        {
            services.AddSingleton<IBusManager, MassTransitBusManager>();
        }

        private void InstallWebHostCoreServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthorization();
            var identityServerAuthenticationAuthority = GetStringFromConfig("IdentityServerAuthentication", "Authority");
            var identityServerAuthenticationApiName = GetStringFromConfig("IdentityServerAuthentication", "ApiName");

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = identityServerAuthenticationAuthority;
                    options.ApiName = identityServerAuthenticationApiName;
                    options.RequireHttpsMetadata = false;//TODO: if we need https
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Gateway Web API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert jwt-token into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }}
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            string GetStringFromConfig(string firstName, string secondName)
            {
                var result = Configuration[$"{firstName}:{secondName}"];
                if (string.IsNullOrEmpty(result))
                {
                    result = Configuration[$"{firstName}_{secondName}"];
                }

                if (string.IsNullOrEmpty(result))
                {
                    throw new Exception($"Configuration setting does not exist. Setting name {firstName}:{secondName}");
                }

                return result;
            }
        }

        private void InstallConfiguration(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddSingleton((IConfigurationRoot)Configuration);
        }

        private void InstallHttpClientServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //set 5 min as the lifetime for each HttpMessageHandler int the pool
            services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(TimeSpan.FromMinutes(5));
        }

        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<Mappers.UserMapProfile>();
            });
            configuration.AssertConfigurationIsValid();
            return configuration;
        }

        private void InstallAutomapper(IServiceCollection services)
        {
            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));
        }

        #endregion
    }
}
