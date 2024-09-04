using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Photon.Hive.Plugin;
using Photon.SocketServer.NUnit.Utils.Http;
using Photon.Hive.Plugin.WebHooks;

namespace TestPlugins
{
    public class HttpPluginsTestListener : IDisposable
    {
        private readonly HttpListener listener;

        private readonly IPluginLogger log;

        private static readonly byte[] defaultResponseData = Encoding.UTF8.GetBytes("Hello");

        private readonly Dictionary<string, SerializableGameState> games =
            new Dictionary<string, SerializableGameState>();

        private readonly Dictionary<string, Dictionary<string, object>> authCookie = new Dictionary<string, Dictionary<string, object>>();

        public bool IsRunning { get; private set; }

        public bool IsDisposed { get; private set; }

        public Uri Url { get; }

        public HttpPluginsTestListener(IPluginLogger logger, bool start = true)
        {
            this.log = logger;

            int port = 55557;
            if (!PortManager.IsPortFree(port))
            {
                port = PortManager.TakePort();
            }

            var uri = $"http://localhost:{port}";
            this.Url = new Uri(uri);

            this.listener = new HttpListener();
            this.listener.Prefixes.Add(this.Url.ToString());

            if (start)
            {
                this.Start();
            }
        }

        public void Start()
        {
            lock (this.listener)
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(HttpPluginsTestListener));
                }

                if (this.IsRunning)
                {
                    return;
                }

                this.listener.Start();
                this.IsRunning = true;
                this.listener.GetContextAsync().ContinueWith(this.HandleRequest);
            }
        }

        public void Stop()
        {
            lock (this.listener)
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(HttpPluginsTestListener));
                }

                this.listener.Stop();
                this.IsRunning = false;
            }
        }

        public void Dispose()
        {
            lock (this.listener)
            {
                if (this.IsDisposed)
                {
                    return;
                }

                this.Stop();
                this.IsDisposed = true;

                this.listener.Abort();
            }
        }

        public string GetTooBigResponse()
        {
            return this.Url + "?toobig=1000";
        }

        public string GetWebHooks12Url()
        {
            return this.Url + "realtime-webhooks-1.2";
        }

        private async void HandleRequest(Task<HttpListenerContext> contextTask)
        {
            if (contextTask.IsCanceled)
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.Debug("Task canceled");
                }

                return;
            }

            if (contextTask.IsFaulted && contextTask.Exception?.InnerException is ObjectDisposedException)
            {
                this.log.Debug("Task faulted with ObjectDisposedException");
                return;
            }

            try
            {
                _ = this.listener.GetContextAsync()
                    .ContinueWith(this.HandleRequest, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (ObjectDisposedException)
            {
            }

            if (!contextTask.IsFaulted)
            {
                await this.HandleRequest(contextTask.Result);
            }
        }

        public struct SecureCheckRequestData
        {
            public string Check { get; set; }
        }
        private async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                if (this.log.IsDebugEnabled)
                {
                    this.log.DebugFormat("Handling of the request. url:{0}", context.Request.RawUrl);
                }

                var request = context.Request;

                if (request.RawUrl.Contains("realtime-webhooks-1.2"))
                {
                    var segments = request.Url.Segments;
                    var lastSegment = segments[segments.Length - 1];

                    //here we handle all requests with path .../realtime-webhooks-1.2/...

                    switch (lastSegment)
                    {
                        case "WrongUriPath":
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            break;
                        case "GetGameList":
                        {
                            var data = Encoding.UTF8.GetBytes("{\"ResultCode\":0, \"Message\":\"\" }");
                            await context.Response.OutputStream.WriteAsync(data, 0, data.Length);
                            break;
                        }
                        case "CheckSecure":
                        {
                            // here we check that webhooks requests that were done before had AuthCookie
                            // during handling of requests we put auth cookie to dictionary
                            // during handling of this request we check that dictionary contains corresponding AuthCookie
                            await this.HandleCheckSecureRequest(context, request);
                            break;
                        }
                        default:
                            this.HandleWebhooksRequest(context);
                            break;
                    }
                }
                else
                {
                    if (((IList) request.QueryString.AllKeys).Contains("timeout"))
                    {
                        return;
                    }

                    var delayParam = request.QueryString["delay"];
                    if (delayParam != null && int.TryParse(delayParam, out int delay))
                    {
                        if (this.log.IsDebugEnabled)
                        {
                            this.log.Debug("delay before answer");
                        }

                        await Task.Delay(TimeSpan.FromMilliseconds(delay));
                    }

                    context.Response.StatusCode = (int) GetStatusCode(request);

                    var responseData = GetResponseData(request);
                    if (request.QueryString.AllKeys.Contains(@"chunksending"))
                    {
                        if (this.log.IsDebugEnabled)
                        {
                            this.log.Debug("chunk sending");
                        }

                        responseData = new byte[2048];

                        context.Response.SendChunked = true;
                        context.Response.OutputStream.Write(responseData, 0, responseData.Length / 2);
                        context.Response.OutputStream.Flush();

                        await Task.Delay(500);

                        context.Response.OutputStream.Write(responseData, responseData.Length / 2,
                            responseData.Length - responseData.Length / 2);
                        context.Response.OutputStream.Flush();
                    }
                    else if (request.QueryString.AllKeys.Contains(@"toobig"))
                    {
                        responseData = new byte[10000];
                        context.Response.ContentLength64 = responseData.Length;
                        context.Response.OutputStream.Write(responseData, 0, responseData.Length);
                        context.Response.OutputStream.Flush();
                    }
                    else
                    {
                        if (this.log.IsDebugEnabled)
                        {
                            this.log.Debug("sending...");
                        }

                        context.Response.OutputStream.Write(responseData, 0, responseData.Length);
                    }
                }

                context.Response.Close();
            }
            catch (Exception e)
            {
                this.log.Error(e);

                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.StatusDescription = e.Message;
                context.Response.Close();
            }
        }

        private async Task HandleCheckSecureRequest(HttpListenerContext context, HttpListenerRequest request)
        {
            var incomingData = new byte[1024];
            var size = await request.InputStream.ReadAsync(incomingData, 0, 1024);
            var str = Encoding.UTF8.GetString(incomingData, 0, size);

            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("Got CheckSecure request with data {0}", str);
            }

            var checkRequest = JsonConvert.DeserializeObject<SecureCheckRequestData>(str);

            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("Got CheckSecure request with requested field {0}", checkRequest.Check);
            }


            var responseDict = new Dictionary<string, object>
            {
                { "ResultCode", 0 },
                { "Debug", checkRequest.Check},
            };

            if (this.authCookie.TryGetValue(checkRequest.Check, out var dataToSend))
            {
                // take data out from store to not intersect with other tests if any
                this.authCookie.Remove(checkRequest.Check);

                responseDict.Add("Data", JsonConvert.SerializeObject(dataToSend));
            }


            var responseStr = JsonConvert.SerializeObject(responseDict);

            if (this.log.IsDebugEnabled)
            {
                this.log.DebugFormat("Answering CheckSecure request. response {0}", responseStr);
            }

            var bytesToSend = Encoding.UTF8.GetBytes(responseStr);

            await context.Response.OutputStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        private void HandleWebhooksRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var dataLen = (int)request.ContentLength64;
            var data = new byte[dataLen];
            request.InputStream.Read(data, 0, dataLen);

            var split = request.Url.Segments;
            var last = split[split.Length - 1];

            var strData = Encoding.UTF8.GetString(data);

            if (this.log.IsDebugEnabled)
            {
                this.log.Debug($"Got new webhooks request with data: {strData}");
            }

            var webHooksRequest = JsonConvert.DeserializeObject<WebhooksRequest>(strData);

            if (this.log.IsDebugEnabled)
            {
                this.log.Debug($"Got new webhooks request with type_{webHooksRequest.Type}, GameId:{webHooksRequest.GameId}, actorId:{webHooksRequest.UserId}");
            }

            switch (webHooksRequest.Type)
            {
                case "Create":
                case "Save":
                    this.HandleCreateRequest(webHooksRequest, context);
                    break;
                case "Load":
                    this.HandleLoadRequest(webHooksRequest, context);
                    break;
                case "Join":
                    this.HandleGameJoinRequest(webHooksRequest, context);
                    break;
                default:
                {
                    if (last.Contains("Rpc"))
                    {
                        var response = new WebhooksResponse()
                        {
                            ResultCode = 0
                        };

                        var d = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                        context.Response.OutputStream.Write(d, 0, d.Length);
                        context.Response.OutputStream.Flush();
                    }

                    break;
                }
            }

            // if we have something like CreateGameSecure in last segment, than we put auth cookie to storage
            if (last.Contains("Secure") && webHooksRequest.AuthCookie != null)
            {
                last = last.Replace("Secure", "AuthCookie").Replace("?", "");

                this.authCookie.Add(last, webHooksRequest.AuthCookie);
                if (this.log.IsDebugEnabled)
                {
                    this.log.DebugFormat("AuthCookie were added. key:{0}", last);
                }
            }
        }

        private void HandleGameJoinRequest(WebhooksRequest webHooksRequest, HttpListenerContext context)
        {
            var response = new WebhooksResponse()
            {
                ResultCode = 0
            };
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));

            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.OutputStream.Flush();

        }

        private void HandleLoadRequest(WebhooksRequest webHooksRequest, HttpListenerContext context)
        {
            var response = new WebhooksResponse()
            {
                ResultCode = 0
            };

            if (this.games.TryGetValue(webHooksRequest.GameId, out var gameState))
            {
                response.State = gameState;
            }
            else
            {
                response.ResultCode = 1;
                response.Message = $"Game '{webHooksRequest.GameId}' not found";
            }
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));

            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.OutputStream.Flush();
        }

        private void HandleCreateRequest(WebhooksRequest webHooksRequest, HttpListenerContext context)
        {
            this.games[webHooksRequest.GameId] = webHooksRequest.State;

            var response = new WebhooksResponse()
            {
                ResultCode = 0
            };
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));

            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.OutputStream.Flush();
        }

        private static HttpStatusCode GetStatusCode(HttpListenerRequest request)
        {
            string codeString = request.QueryString["statusCode"];
            if (string.IsNullOrEmpty(codeString))
                return HttpStatusCode.OK;

            if (int.TryParse(codeString, out var code))
                return (HttpStatusCode) code;

            if (Enum.TryParse<HttpStatusCode>(codeString, true, out var statusCode))
                return statusCode;

            return HttpStatusCode.OK;
        }

        private static byte[] GetResponseData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return defaultResponseData;
            }

            var result = new byte[request.ContentLength64];
            request.InputStream.Read(result, 0, result.Length);
            return result;
        }

    }
}