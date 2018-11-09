using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using WebApi.Api.Filters;
using WebApi.Api.Middleware;
using WebApi.Core.Contracts.Gateways;
using WebApi.Infrastructure.Data.Repositories;

namespace WebApi.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient<ILaunchPadRespository, SpaceXLaunchPadApiRespository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "LaunchPad API",
                    Description = "An Api Excercise for SmileDirectClub",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Robert McCardell",
                        Email = "mccardellr@gmail.com",
                        Url = "https://github.com/rmccardell"
                    }
                });
            });

            services.AddScoped<ApiResultsFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //install serilog middleware
            app.UseMiddleware<SerilogRequestMiddleware>();

            //setup swagger endpoint
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LaunchPad API V1");
            });

            app.UseMvc();


        }
    }
}
