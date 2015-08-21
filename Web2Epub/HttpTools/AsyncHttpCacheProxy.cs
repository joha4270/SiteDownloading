using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace web2Ebook.HttpTools
{
	public class AsyncHttpCacheProxy
	{
		private LinkedList<Tuple<Uri,Task<HttpResponseMessage>>> _cacheList = new LinkedList<Tuple<Uri, Task<HttpResponseMessage>>>();
		private Dictionary<Uri, LinkedListNode<Tuple<Uri, Task<HttpResponseMessage>>>> _cacheDictionary = new Dictionary<Uri, LinkedListNode<Tuple<Uri, Task<HttpResponseMessage>>>>();
		private int _count = 0;
		private int _maxSize = 100;
		private HttpRatelimitedClient _client = new HttpRatelimitedClient();
		public int MaxSize
		{
			get { return _maxSize; }
			set { _maxSize = value; }
		}

		public async Task<HttpResponseMessage> Get(Uri url)
		{
			if (_cacheDictionary.ContainsKey(url))
			{
				LinkedListNode<Tuple<Uri, Task<HttpResponseMessage>>> cachenode = _cacheDictionary[url];
				_cacheList.Remove(cachenode);
				_cacheList.AddFirst(cachenode);

				return await cachenode.Value.Item2;
			}
			else
			{
				LinkedListNode<Tuple<Uri, Task<HttpResponseMessage>>> nnode = _cacheList.AddFirst(FetchHttp(url));
				_cacheDictionary.Add(url, nnode);
				_count++;

				while (_count > _maxSize)
				{
					LinkedListNode<Tuple<Uri, Task<HttpResponseMessage>>> last = _cacheList.Last;
					_cacheList.Remove(last);
					_cacheDictionary.Remove(last.Value.Item1);
					_count--;


				}

				return await nnode.Value.Item2;
			}
		}

		private Tuple<Uri, Task<HttpResponseMessage>> FetchHttp(Uri url)
		{
			return Tuple.Create(url, _client.GetAsync(url));
		}
	}
}
