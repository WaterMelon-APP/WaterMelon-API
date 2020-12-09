using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WaterMelon_API.Models;
using Microsoft.Extensions.Options;
using WaterMelon_API.Services;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using System;

namespace WaterMelon_API
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
            IdentityModelEventSource.ShowPII = true; // show error details
            var facebookAuthSettings = new FacebookAuthSettings();

            services.Configure<FormOptions>(options =>
            {
                options.MemoryBufferThreshold = Int32.MaxValue;
            });

            // requires using Microsoft.Extensions.Options
            services.Configure<UserDatabaseSettings>(Configuration.GetSection(nameof(UserDatabaseSettings)));
            services.AddSingleton<IUserDatabaseSettings>(sp => sp.GetRequiredService<IOptions<UserDatabaseSettings>>().Value);
            services.AddSingleton<UserService>();

            services.Configure<EventDatabaseSettings>(Configuration.GetSection(nameof(EventDatabaseSettings)));
            services.AddSingleton<IEventDatabaseSettings>(sp => sp.GetRequiredService<IOptions<EventDatabaseSettings>>().Value);
            services.AddSingleton<EventService>();

            services.Configure<ItemDatabaseSettings>(Configuration.GetSection(nameof(ItemDatabaseSettings)));
            services.AddSingleton<IItemDatabaseSettings>(sp => sp.GetRequiredService<IOptions<ItemDatabaseSettings>>().Value);
            services.AddSingleton<ItemService>();

            services.Configure<NotificationDatabaseSettings>(Configuration.GetSection(nameof(NotificationDatabaseSettings)));
            services.AddSingleton<INotificationDatabaseSettings>(sp => sp.GetRequiredService<IOptions<NotificationDatabaseSettings>>().Value);
            services.AddSingleton<NotificationService>();

            services.Configure<InvitationDatabaseSettings>(Configuration.GetSection(nameof(InvitationDatabaseSettings)));
            services.AddSingleton<IInvitationDatabaseSettings>(sp => sp.GetRequiredService<IOptions<InvitationDatabaseSettings>>().Value);
            services.AddSingleton<InvitationService>();

            services.Configure<ProfilePictureDatabaseSettings>(Configuration.GetSection(nameof(ProfilePictureDatabaseSettings)));
            services.AddSingleton<IProfilePictureDatabaseSettings>(sp => sp.GetRequiredService<IOptions<ProfilePictureDatabaseSettings>>().Value);
            services.AddSingleton<ProfilePictureService>();

            Configuration.Bind(nameof(FacebookAuthSettings), facebookAuthSettings);
            services.AddSingleton(facebookAuthSettings);

            services.AddHttpClient();
            services.AddSingleton<FacebookAuthService>();
            services.AddSingleton<GoogleAuthService>();
            services.AddSingleton<EmailService>();

            services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["jwt:issuer"],
                    ValidAudience = Configuration["jwt:issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["jwt:key"]))
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
