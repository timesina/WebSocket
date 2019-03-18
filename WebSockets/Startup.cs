using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebSockets
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSignalR(options =>
            {
                // Faster pings for testing
                options.KeepAliveInterval = TimeSpan.FromSeconds(5);//心跳包间隔时间，单位 秒，可以稍微调大一点儿
            }).AddJsonProtocol(options =>
            {
                //options.PayloadSerializerSettings.Converters.Add(JsonConver);
                //the next settings are important in order to serialize and deserialize date times as is and not convert time zones
                options.PayloadSerializerSettings.Converters.Add(new IsoDateTimeConverter());
                options.PayloadSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                options.PayloadSerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseWebSockets();
         
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSignalR(routes => {
                routes.MapHub<ChatHub>("/ChatHub", options => options.WebSockets.SubProtocolSelector = requestedProtocols =>
                {
                    return requestedProtocols.Count > 0 ? requestedProtocols[0] : null;
                });
            });

            app.UseMvc();
            //app.Use(async (context, next) => {
            //    if (context.Request.Path == "/ws")
            //    {
            //        if (context.WebSockets.IsWebSocketRequest)
            //        {
            //            var webSockets = await context.WebSockets.AcceptWebSocketAsync();
            //            await Echo(context, webSockets);
            //        }
            //    }
            //    else
            //    {
            //        context.Response.StatusCode = 400;
            //    }
            //});
        }

        //private async Task Echo(HttpContext context,WebSocket webSocket)
        //{
        //    var buff = new byte[1024 * 4];
        //    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buff), CancellationToken.None);
        //    while (!result.CloseStatus.HasValue)
        //    {
        //        await webSocket.SendAsync(new ArraySegment<byte>(buff,0,result.Count),result.MessageType,result.EndOfMessage,CancellationToken.None);
        //        var outbuff = new ArraySegment<byte>(buff, 0, result.Count);
        //        string str = System.Text.Encoding.Default.GetString(outbuff);
        //        result = await webSocket.ReceiveAsync(outbuff, CancellationToken.None);

        //    }
        //    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription,CancellationToken.None);

        //}
    }
}
