using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using web2Ebook;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			Program p = new Program();

			if (Console.ReadKey(true).KeyChar == 't')
			{
				p.tester(new Uri(Console.ReadLine()));
			}
			else
			{
				Book b = Book.GetBook(new Uri(Console.ReadLine()));
			}

			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.Read();

		}
		
		void tester(Uri uri)
		{
			HttpClient client = new HttpClient();

			HttpResponseMessage resp = client.GetAsync(new Uri("https://www.fanfiction.net/s/7262793/101/Ashes-of-the-Past")).Result;

			HtmlDocument firstDocument = new HtmlDocument();
			firstDocument.Load(resp.Content.ReadAsStreamAsync().Result,
				Encoding.GetEncoding(resp.Content.Headers.ContentType.CharSet));

			while (true)
			{
				try
				{
					Console.Write("Xpath:");
					String XPath = Console.ReadLine();


					Console.WriteLine("Testing XPath");

					HtmlNodeCollection col = firstDocument.DocumentNode.SelectNodes(XPath);
					if (col == null)
					{
						Console.WriteLine("Xpath returned nothing");
						continue;
					}

					foreach (HtmlNode selectNode in col)
					{
						Console.WriteLine(selectNode.Name);
						Console.Write(selectNode.InnerHtml.Substring(0, Math.Min(236, selectNode.InnerHtml.Length)));
						Console.WriteLine("...");

					}
				}
				catch(Exception ex)
				{
					Console.WriteLine("EXCEPTION: {0}", ex.GetType());
					continue;
					
				}
			}
		}
	}
}
