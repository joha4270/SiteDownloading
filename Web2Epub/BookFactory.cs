using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using web2Ebook.Downloaders;
using web2Ebook.HttpTools;

namespace web2Ebook
{
	class BookFactory
	{
		public static AsyncHttpCacheProxy DownloaderProxy { get; } = new AsyncHttpCacheProxy();
		readonly List<IDownloader> _downloaders = new List<IDownloader>();

		internal BookFactory()
		{
			_downloaders.Add(new FanFictionNetDownloader());
		}

		public async Task<Book> GetBook(Uri uri)
		{
			foreach (IDownloader downloader in _downloaders)
			{
				if (downloader.CanHandleUri(uri))
				{
					return await downloader.Download(uri);
				}
			}

			return null;
		}
	}
}
