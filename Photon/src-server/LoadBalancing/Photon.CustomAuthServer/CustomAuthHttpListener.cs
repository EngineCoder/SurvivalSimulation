using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using ExitGames.Logging;

namespace Photon.CustomAuthServer
{
    public class CustomAuthHttpListener : IDisposable
    {
        private readonly HttpListener listener;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public bool IsRunning { get; private set; }

        public bool IsDisposed { get; private set; }

        public Uri Url { get; }

        public CustomAuthHttpListener(bool start = true)
        {
            var uri = $"http://localhost:{55533}";
            this.Url = new Uri(uri);

            this.listener = new HttpListener();
            this.listener.Prefixes.Add(this.Url.ToString());

            if (start)
            {
                this.Start();
            }
        }

        public CustomAuthHttpListener(Uri uriPrefix)
        {
            this.Url = uriPrefix;
            this.listener = new HttpListener();
            listener.Prefixes.Add(uriPrefix.ToString());
        }

        public void Start()
        {
            lock (this.listener)
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(CustomAuthHttpListener));
                }

                if (log.IsDebugEnabled)
                {
                    log.Debug("Starting Http Listener ....");
                }

                if (this.IsRunning)
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("Http Listener is running. return");
                    }

                    return;
                }

                this.listener.Start();
                this.IsRunning = true;
                this.listener.GetContextAsync().ContinueWith(this.HandleRequest);

                if (log.IsDebugEnabled)
                {
                    log.Debug("Http Listener started");
                }

            }
        }

        public void Stop()
        {
            lock (this.listener)
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(CustomAuthHttpListener));
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

        private async void HandleRequest(Task<HttpListenerContext> contextTask)
        {
            if (contextTask.IsCanceled)
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Task canceled");
                }
                return;
            }

            if (contextTask.IsFaulted && contextTask.Exception?.InnerException is ObjectDisposedException)
            {
                log.Debug("Task faulted with ObjectDisposedException");
                return;
            }

            try
            {
                _ = this.listener.GetContextAsync().ContinueWith(this.HandleRequest, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            catch (ObjectDisposedException) { }

            if (!contextTask.IsFaulted)
            {
                await HandleRequest(contextTask.Result);
            }
        }

        private static async Task HandleRequest(HttpListenerContext context)
        {
            try
            {
                if (log.IsDebugEnabled)
                {
                    log.Debug("Handling of the request");
                }

                var request = context.Request;
                var userNameParam = request.QueryString["username"];
                byte[] data;

                if (string.IsNullOrEmpty(userNameParam))
                {
                    if (log.IsDebugEnabled)
                    {
                        log.Debug("user name is not set. Reject");
                    }

                    data = Encoding.UTF8.GetBytes("{ \"ResultCode\": 3, \"Message\": \"UserId is not set.\" }");
                }
                else
                {
                    if (userNameParam == "testUserName")
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug("Custom Auth passed");
                        }
                        // success
                        data = Encoding.UTF8.GetBytes($"{{ \"ResultCode\": 1, \"UserId\": \"{userNameParam}\" }}");
                    }
                    else
                    {
                        if (log.IsDebugEnabled)
                        {
                            log.Debug($"Custom Auth wrong user name {userNameParam}. Reject");
                        }
                        data = Encoding.UTF8.GetBytes("{ \"ResultCode\": 2, \"Message\": \"Authentication failed. Wrong credentials.\" }");
                    }
                }

                await context.Response.OutputStream.WriteAsync(data, 0, data.Length);
                await context.Response.OutputStream.FlushAsync();
                context.Response.Close();
            }
            catch (Exception e)
            {
                log.Error(e);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusDescription = e.Message;
                context.Response.Close();
            }
        }
    }
}
