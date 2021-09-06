using Anna;
using Anna.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnnaTest
{
    class Program
    {
        private static IDictionary<string, string> headers = new Dictionary<string, string>() { { "Content-Type", "application/octet-stream" },
                                                                                                { "Content-Disposition","form-data; name=file; filename=screenshot.png" } };

        static void Main(string[] args)
        {
            using (var server = new HttpServer("http://localhost:1234/"))
            {
                // simple basic usage, all subscriptions will run in a single event-loop

                server.GET("/ws")
                     .Subscribe(async ctx =>
                     {
                         if( ctx.Request.IsWebSocketRequest )
                         {
                             var websocketContext = await ctx.AcceptWebSocketAsync(null);
                             Console.WriteLine("new socket");
                             ProcessClient(websocketContext.WebSocket);
                         }
                     });

                server.GET("/")
                     .Subscribe(ctx =>
                     {
                         //ctx.Respond("Hello, Index!");
                         ctx.Redrict("http://www.baidu.com");
                     });


               server.GET("/hello/{Name}")
                      .Subscribe(ctx =>
                      {
                          ctx.Respond("Hello, " + ctx.Request.UriArguments.Name + "!");
                      }
                        
                      );

                server.GET("/download")
                      .Subscribe(ctx =>
                      {
                          Console.WriteLine("get file----");
                          ctx.StaticFileResponse("111.png",1024, headers).Send();
                          
                      }

                      );

                // use Rx LINQ operators
                server.POST("/hi/{Name}")
                      .Where(ctx => ctx.Request.UriArguments.Name == "George")
                      .Subscribe(ctx => ctx.Respond("Hi, George!"));

                server.POST("/hi/{Name}")
                      .Where(ctx => ctx.Request.UriArguments.Name == "Pete")
                      .Subscribe(ctx => ctx.Respond("Hi, Pete!"));
                    
                Console.WriteLine("Listen on  port 1234"); 
                Console.ReadLine();


                //System.Threading.Thread.Sleep(5000);
            }
        }
        async static void ProcessClient(WebSocket websocket)
        {
            var data = new byte[1500];
            var buffer = new ArraySegment<byte>(data);

            while (true)
            {
                var result = await websocket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.CloseStatus != null)
                {
                    Console.WriteLine("socket closed");
                    websocket.Abort();
                    return;
                }

                var message = Encoding.UTF8.GetString(data, 0, result.Count);
                var reply = new ArraySegment<byte>(Encoding.UTF8.GetBytes(string.Format("Server Time:{0},Message:{1}", DateTime.Now, message)));

                Console.WriteLine(">>> " + message);
                await websocket.SendAsync(reply, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
