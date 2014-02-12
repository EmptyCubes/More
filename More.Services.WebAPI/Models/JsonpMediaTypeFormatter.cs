using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace GlobalCORS_RC.Handlers
{
    public class CorsHandler : DelegatingHandler
    {
        const string Origin = "Origin";
        const string AccessControlRequestMethod = "Access-Control-Request-Method";
        const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool isCorsRequest = request.Headers.Contains(Origin);
            bool isPreflightRequest = request.Method == HttpMethod.Options;
            if (isCorsRequest)
            {
                if (isPreflightRequest)
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());

                    string accessControlRequestMethod = request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
                    if (accessControlRequestMethod != null)
                    {
                        response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
                    }

                    string requestedHeaders = string.Join(", ", request.Headers.GetValues(AccessControlRequestHeaders));
                    if (!string.IsNullOrEmpty(requestedHeaders))
                    {
                        response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
                    }

                    TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
                    tcs.SetResult(response);
                    return tcs.Task;
                }
                else
                {
                    return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>(t =>
                    {
                        HttpResponseMessage resp = t.Result;
                        resp.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
                        return resp;
                    });
                }
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
{
    private string callbackQueryParameter;

    public JsonpMediaTypeFormatter()
    {
        SupportedMediaTypes.Add(DefaultMediaType);
        SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

        MediaTypeMappings.Add(new UriPathExtensionMapping("jsonp", DefaultMediaType));
    }

    public string CallbackQueryParameter
    {
        get { return callbackQueryParameter ?? "callback"; }
        set { callbackQueryParameter = value; }
    }

    public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
    {
        string callback;

        if (IsJsonpRequest(out callback))
        {
            return Task.Factory.StartNew(() =>
            {
                var writer = new StreamWriter(stream);
                writer.Write(callback + "(");
                writer.Flush();

                base.WriteToStreamAsync(type, value, stream, contentHeaders, transportContext).Wait();

                writer.Write(")");
                writer.Flush();
            });
        }
        else
        {
            return base.WriteToStreamAsync(type, value, stream, contentHeaders, transportContext);
        }
    }


    private bool IsJsonpRequest(out string callback)
    {
        callback = null;

        if (HttpContext.Current.Request.HttpMethod != "GET")
            return false;

        callback = HttpContext.Current.Request.QueryString[CallbackQueryParameter];

        return !string.IsNullOrEmpty(callback);
    }
}