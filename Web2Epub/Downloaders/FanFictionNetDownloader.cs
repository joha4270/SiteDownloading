using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace web2Ebook.Downloaders
{
	internal class FanFictionNetDownloader : IDownloader
	{

		public bool CanHandleUri(Uri uri)
		{
			int notused;
			bool result = uri.Host == "www.fanfiction.net" &&
				uri.Segments[0] == "/" &&
				uri.Segments[1] == "s/" &&
				int.TryParse(uri.Segments[2].Substring(0, uri.Segments[2].Length - 1), out notused);

			return result;

		}

		public async Task<Book> Download(Uri uri)
		{
			if (!CanHandleUri(uri))
			{
				throw new NotSupportedException();
			}

			string id = ExtractIdFromUri(uri);

			string firstPage = $"http://{uri.Host}/s/{id}/1";

			HttpResponseMessage firstPageHttp = await BookFactory.DownloaderProxy.Get(new Uri(firstPage));
			if (!firstPageHttp.IsSuccessStatusCode)
			{
				return null;
			}
			
			Stream documentStream = await firstPageHttp.Content.ReadAsStreamAsync();


			HtmlDocument firstDocument = new HtmlDocument();
			firstDocument.Load(documentStream, Encoding.GetEncoding(firstPageHttp.Content.Headers.ContentType.CharSet));

			HtmlNode Document = firstDocument.DocumentNode;

			HtmlNode chapters = Document.SelectSingleNode("//select[@id = 'chap_select']");

			HtmlNode infoNode = Document.SelectSingleNode("//div[@id = 'profile_top']");

			Book retBook = new Book(infoNode.ChildNodes[2].InnerText, null, infoNode.ChildNodes[12].InnerText);

			//this happens if the entire story is single page
			if (chapters == null)
			{ 
				retBook.Chapters.Add(new Chapter()
				{
					Name = retBook.Name,
					Content = Document.SelectSingleNode("//div[@id = 'storytext']").InnerHtml
				});
			}
			else
			{

				Chapter first = await MkChapter(firstPageHttp);

				retBook.Chapters.Add(first);

				int chaptersCount = chapters.ChildNodes.Count / 2;

				Task<HttpResponseMessage>[] resp = new Task<HttpResponseMessage>[chaptersCount - 1];

				for (int i = 2; i < chaptersCount + 1; i++)
				{
					resp[i-2] =	BookFactory.DownloaderProxy.Get(new Uri($"http://{uri.Host}/s/{id}/{i}"));
				}

				for (int i = 0; i < resp.Length; i++)
				{
					HttpResponseMessage httpstuff = await resp[i];

					Chapter chap = await MkChapter(httpstuff);

					retBook.Chapters.Add(chap);

					
				}

			}

            return retBook;
			
		}

		Regex name = new Regex("[0-9]*\\. ");
		private async Task<Chapter> MkChapter(HttpResponseMessage httpstuff)
		{
			try
			{
				Stream stream = await httpstuff.Content.ReadAsStreamAsync();
				stream.Seek(0, SeekOrigin.Begin);
				HtmlDocument doc = new HtmlDocument();
				doc.Load(stream, Encoding.GetEncoding(httpstuff.Content.Headers.ContentType.CharSet));

				HtmlNode n = doc.DocumentNode.SelectSingleNode("//select[@id = 'chap_select']/*[@selected]");


				Match m = name.Match(n.NextSibling.InnerText);

				return new Chapter()
				{
					Name = n.NextSibling.InnerText.Substring(m.Value.Length),
					Content = doc.DocumentNode.SelectSingleNode("//div[@id = 'storytext']").InnerHtml
				};
			}
			catch (Exception ex)
			{
				;

				throw ex;
			}
			
		}

		private string ExtractIdFromUri(Uri uri)
		{
			return uri.Segments[2].Substring(0, uri.Segments[2].Length - 1);
		}
	}
}