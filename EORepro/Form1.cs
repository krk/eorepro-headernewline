using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows.Forms;
using EO.WebBrowser;

namespace EORepro
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			label1.Text = $"EO {typeof(WebView).Assembly.GetName().Version}";
		}

		ConcurrentQueue<string> _urls = new ConcurrentQueue<string>();

		private void button1_Click_1(object sender, EventArgs e)
		{
			for (var i = 0; i < 1000; i++)
			{
				var url = $"https://www.google.com.tr/search?q={i}";

				_urls.Enqueue(url);
			}

			var po = new ParallelOptions()
			{
				MaxDegreeOfParallelism = 6,
			};

			Parallel.For(0, 1, po, i =>
			{
				var wrapper = new WebViewWrapper();

				string url;

				if (!_urls.TryDequeue(out url))
				{
					return;
				}

				wrapper.LoadUrlAndWait(url);

				Task.Delay(10).Wait();

				wrapper.Destroy();
			});

			EO.Base.Runtime.Shutdown();
		}
	}
}
