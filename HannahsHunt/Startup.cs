using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HannahsHunt.Data;
using HannahsHunt.Models;
using HannahsHunt.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using System.Security.Claims;
//using static HannahsHunt.Extensions.HannahsClaimsPrincipalFactory;

namespace HannahsHunt
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Administrator", "Basic" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            //Create a super user who will maintain the web app
            var poweruser = new ApplicationUser
            {
                UserName = Configuration["Authentication:HannahsHunt:AdminUserName"],
                Email = Configuration["Authentication:HannahsHunt:AdminUserEmail"],
                FirstName = Configuration["Authentication:HannahsHunt:AdminUserFirstName"],
                LastName = Configuration["Authentication:HannahsHunt:AdminUserLastName"]
            };
            //Ensure you have these values in your appsettings.json file
            string userPWD = Configuration["Authentication:HannahsHunt:AdminUserPassword"];
            var _user = await UserManager.FindByEmailAsync(Configuration["Authentication:HannahsHunt:AdminUserEmail"]);

            if (_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {

                    await UserManager.AddClaimAsync(poweruser, new Claim("FirstName", poweruser.FirstName));
                    await UserManager.AddClaimAsync(poweruser, new Claim("LastName", poweruser.LastName));
                    await UserManager.AddClaimAsync(poweruser, new Claim("FullName", poweruser.FullName));

                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, "Administrator");

                }
            }
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, Extensions.HannahsClaimsPrincipalFactory>();

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });
            services.AddAuthentication().AddTwitter(twitterOptions =>
                        {
                            twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
                            twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                        });


            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            var builder = new ConfigurationBuilder();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                builder.AddUserSecrets<Startup>();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            CreateRoles(serviceProvider).Wait();
            var options = new RewriteOptions().AddRedirectToHttps();
            app.UseRewriter(options);
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
        }
    }
}