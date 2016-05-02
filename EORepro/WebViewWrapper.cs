using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using EO.WebBrowser;

namespace EORepro
{
	public class WebViewWrapper : MarshalByRefObject
	{
		private readonly WebView _webView;
		private readonly InterceptingResourceHandler _handler;

		public WebViewWrapper()
		{
			_webView = new WebView();

			_webView.Create(IntPtr.Zero);

			_handler = new InterceptingResourceHandler();

			_webView.RegisterResourceHandler(_handler);
		}

		public void Shutdown()
		{
			EO.Base.Runtime.Shutdown();
		}

		public void Destroy()
		{
			_webView.Destroy();
		}

		public void LoadUrlAndWait(string url)
		{
			var request = new Request(url);

			request.Headers["Upgrade-Insecure-Requests"] = "2";

			_webView.LoadRequest(request).WaitOne(4000);
		}
	}

	public class InterceptingResourceHandler : ResourceHandler
	{
		public static IDictionary<string, string> ToDictionary(NameValueCollection collection)
		{
			if (collection == null)
			{
				return null;
			}

			var dictionary = new Dictionary<string, string>(collection.Count);

			foreach (string name in collection)
			{
				dictionary[name] = collection[name];
			}

			return dictionary;
		}

		public override bool Match(Request request)
		{
			return true;
		}

		public override void ProcessRequest(Request request, Response response)
		{
			// Actual: "2\r,1"
			// Expected: "2"
			var header = request.Headers["Upgrade-Insecure-Requests"];

			Debugger.Break();
		}
	}
}