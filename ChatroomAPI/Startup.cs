using ChatroomAPI.Database;
using ChatroomAPI.Middleware;
using ChatroomAPI.Middleware.Interface;
using ChatroomAPI.Model.Hubs;
using ChatroomAPI.Repositories;
using ChatroomAPI.Repositories.Interface;
using ChatroomAPI.Services;
using ChatroomAPI.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI
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
            //services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            //{
            //    builder.AllowAnyOrigin()
            //           .AllowAnyMethod()
            //           .AllowAnyHeader()
            //           .DisallowCredentials();
            //}));
            //    services.AddDbContext<ChatContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContextPool<ChatContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlConnectionString"));
            });

            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IChatServices, ChatServices>();
            services.AddScoped<FileServices>();
            //services.AddSingleton<IChatMiddleware, ChatMiddleware>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddHttpContextAccessor();
            services.AddSignalR();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChatroomAPI", Version = "v1" });
            });


            //services.AddMvc(option => { option.EnableEndpointRouting = false; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatroomAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseCors("MyPolicy");

            app.UseCors(builder => builder
                         .AllowAnyMethod()
                         .AllowAnyHeader()
                         .SetIsOriginAllowed(origin => true)
                         .AllowCredentials());

            app.UseSession();

            //app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                var hubContext = context.User;

                //if(hubContext != null)
                //    await hubContext.Clients.All.SendAsync("ReceiveMessage", "122", "444");

                if (next != null)
                {
                    await next.Invoke();
                }
            });

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");

                endpoints.MapHub<ChatHub>("/ChatHub");
            });

            //app.UseMvc();
        }
    }
}
