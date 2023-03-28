using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullCoreProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FullCoreProject
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContextPool<AppDbContext>(options=>options.UseSqlServer(_config.GetConnectionString("EmployeeDBConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options=> {
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>();
          
           //services.AddMvcCore();
             services.AddMvc(options=> {
                 var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                 options.Filters.Add(new AuthorizeFilter(policy));
             });        //Apply Authorize Attribute globally
            //services.AddMvc().AddXmlSerializerFormatters(); api xml format
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Roles"));
                options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Roles","true"));
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
            });

            services.ConfigureApplicationCookie(option => 
            {
                option.AccessDeniedPath = new PathString("/Adminstration/AccessDenied");
            });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("DeleteandCreateRolePolicy", policy => policy.RequireClaim("Delete Roles").RequireClaim("Create Roles"));
            //});
            //services.AddSingleton<IEmployeeRepository, MockEmployeeRepository>();
            // services.AddSingleton<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            //services.AddTransient<IEmployeeRepository, MockEmployeeRepository>();


        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }

            else
            {
                app.UseExceptionHandler("/Error");
                //app.UseStatusCodePages();
                //app.UseStatusCodePagesWithRedirects("/Error/{0}");
                 app.UseStatusCodePagesWithReExecute("/Error/{0}");

            }

            app.UseStaticFiles();
            app.UseAuthentication();
            //app.UseMvcWithDefaultRoute();
            app.UseMvc(routes =>
            {
                routes.MapRoute("Default", "{controller=Home}/{action=index}/{id?}");
            });

            app.UseMvc();




            //app.Run(async (context) =>
            //{

            //    await context.Response.WriteAsync("Hosting Environment:  " + env.EnvironmentName);

            //});


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        DeveloperExceptionPageOptions developerExceptionPageOptions = new DeveloperExceptionPageOptions()
        //        {
        //            SourceCodeLineCount = 10
        //        };
        //        app.UseDeveloperExceptionPage(developerExceptionPageOptions);//detects if any middleware comes after it throwing an exception; 
        //    }


        //    //app.UseDefaultFiles();// (change request path only)  search for default.html ,default.htm,index.html,index.htm
        //    //app.UseStaticFiles();//serve the static files 

        //    //to change default files to be served

        //    //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
        //    //defaultFilesOptions.DefaultFileNames.Clear();
        //    //defaultFilesOptions.DefaultFileNames.Add("Test.html");
        //    //app.UseDefaultFiles(defaultFilesOptions);
        //    //app.UseStaticFiles();

        //    //  app.UseFileServer()   combines app.UseDefaultFiles()+app.UseStaticFiles()


        //    //another way to serve static files and change defaults

        //    //FileServerOptions fileServerOptions = new FileServerOptions();
        //    //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
        //    //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("Test.html");
        //    //app.UseFileServer(fileServerOptions);


        //    app.Run(async (context) =>
        //    {
        //        throw new Exception("Some error here");
        //        await context.Response.WriteAsync("Hello World!");

        //    });


        //}

        //// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILogger<Startup>logger)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }

        //    app.Use(async (context,next) =>
        //    {

        //        logger.LogInformation("MW1:Incoming Request");
        //        await next();
        //        logger.LogInformation("MW1:Outgoing Response");
        //    });

        //    app.Use(async (context, next) =>
        //    {

        //        logger.LogInformation("MW2:Incoming Request");
        //        await next();
        //        logger.LogInformation("MW2:Outgoing Response");
        //    });
        //    app.Run(async (context) =>
        //    {
        //        //await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);//get hosting process
        //        //await context.Response.WriteAsync(_config["mykey"]);//to access configurations
        //        await context.Response.WriteAsync("MW3:request and response handled");
        //        logger.LogInformation("MW3:request and response handled");
        //        //await context.Response.WriteAsync("Hello World!");
        //    });


        //    //This will not be executed because the above middleware is a terminal middleware
        //    //app.Run(async (context) =>
        //    //{ 
        //    //    //await context.Response.WriteAsync("Hello World!");
        //    //});
        //}
    }
}
