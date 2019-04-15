using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using got_winner_voting.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace got_winner_voting
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
            services.AddApplicationInsightsTelemetry();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSignalR()
                    .AddAzureSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseFileServer();
            app.UseAzureSignalR(routes =>
            {
                routes.MapHub<VoteHub>("/vote");
            });

            Globals.GlobalItems.SqlConnectionStr = Configuration["Azure:Sql:ConnectionString"];
            Globals.GlobalItems.RedisConnection = new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect(Configuration["Azure:Redis:CacheConnection"])
            );

            var cache = Globals.GlobalItems.RedisConnection.Value.GetDatabase();
            cache.HashSet("got", "DANY", 0);
            cache.HashSet("got", "JON", 0);
            cache.HashSet("got", "TYRION", 0);
            cache.HashSet("got", "CERSEI", 0);
            cache.HashSet("got", "SANSA", 0);

        }
    }
}
