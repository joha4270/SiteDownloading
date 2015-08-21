using System;
using System.Threading.Tasks;

namespace web2Ebook
{
	public interface IDownloader
	{
				bool CanHandleUri(Uri uri);

		Task<Book> Download(Uri uri);
	}
}
