using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace web2Ebook
{
	public class Book
	{
		private static readonly BookFactory _Factory = new BookFactory();
		public static Book GetBook(Uri uri)
		{
			Task<Book> asyncBook = GetBookAsync(uri);

			return asyncBook.Result;
		}

		public static async Task<Book> GetBookAsync(Uri uri)
		{
			return await _Factory.GetBook(uri);
		}

		public List<Chapter> Chapters { get; private set; }

		public String Name { get; set; }

		public String Description { get; set; }
		public Book(string name = "New Book", IEnumerable<Chapter> chapters = null, string description = "")
		{
			Name = name;
			if (chapters == null)
			{
				Chapters = new List<Chapter>();
			}
			else
			{
				Chapters = new List<Chapter>(chapters);
			}

			Description = description;
		}
	}
}