using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
#if !UniRxLibrary
using ObservableUnity = UniRx.Observable;

#endif

#if UNITY_2018_3_OR_NEWER
#pragma warning disable CS0618
#endif

namespace UniRx
{
#if !(UNITY_METRO || UNITY_WP8) && (UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0 || UNITY_2_6_1 || UNITY_2_6)
    // Fallback for Unity versions below 4.5
    using Hash = System.Collections.Hashtable;
    using HashEntry = System.Collections.DictionaryEntry;
#else
	// Unity 4.5 release notes: 
	// WWW: deprecated 'WWW(string url, byte[] postData, Hashtable headers)', 
	// use 'public WWW(string url, byte[] postData, Dictionary<string, string> headers)' instead.
	using Hash = Dictionary<string, string>;
	using HashEntry = KeyValuePair<string, string>;

#endif

#if UNITY_2018_3_OR_NEWER
	[Obsolete("Use UnityWebRequest, a fully featured replacement which is more efficient and has additional features")]
#endif
	public static class ObservableWWW
	{
		public static IObservable<string> Get(string url, Hash headers = null, IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<string>((observer, cancellation) =>
				FetchText(new WWW(url, null, headers ?? new Hash()), observer, progress, cancellation));
		}

		public static IObservable<byte[]> GetAndGetBytes(string url, Hash headers = null,
			IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) =>
				FetchBytes(new WWW(url, null, headers ?? new Hash()), observer, progress, cancellation));
		}

		public static IObservable<WWW> GetWWW(string url, Hash headers = null, IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) =>
				Fetch(new WWW(url, null, headers ?? new Hash()), observer, progress, cancellation));
		}

		public static IObservable<string> Post(string url, byte[] postData, IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<string>((observer, cancellation) =>
				FetchText(new WWW(url, postData), observer, progress, cancellation));
		}

		public static IObservable<string> Post(string url, byte[] postData, Hash headers,
			IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<string>((observer, cancellation) =>
				FetchText(new WWW(url, postData, headers), observer, progress, cancellation));
		}

		public static IObservable<string> Post(string url, WWWForm content, IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<string>((observer, cancellation) =>
				FetchText(new WWW(url, content), observer, progress, cancellation));
		}

		public static IObservable<string> Post(string url, WWWForm content, Hash headers,
			IProgress<float> progress = null)
		{
			Hash contentHeaders = content.headers;
			return ObservableUnity.FromCoroutine<string>((observer, cancellation) =>
				FetchText(new WWW(url, content.data, MergeHash(contentHeaders, headers)), observer, progress,
					cancellation));
		}

		public static IObservable<byte[]> PostAndGetBytes(string url, byte[] postData, IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) =>
				FetchBytes(new WWW(url, postData), observer, progress, cancellation));
		}

		public static IObservable<byte[]> PostAndGetBytes(string url, byte[] postData, Hash headers,
			IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) =>
				FetchBytes(new WWW(url, postData, headers), observer, progress, cancellation));
		}

		public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) =>
				FetchBytes(new WWW(url, content), observer, progress, cancellation));
		}

		public static IObservable<byte[]> PostAndGetBytes(string url, WWWForm content, Hash headers,
			IProgress<float> progress = null)
		{
			Hash contentHeaders = content.headers;
			return ObservableUnity.FromCoroutine<byte[]>((observer, cancellation) =>
				FetchBytes(new WWW(url, content.data, MergeHash(contentHeaders, headers)), observer, progress,
					cancellation));
		}

		public static IObservable<WWW> PostWWW(string url, byte[] postData, IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) =>
				Fetch(new WWW(url, postData), observer, progress, cancellation));
		}

		public static IObservable<WWW> PostWWW(string url, byte[] postData, Hash headers,
			IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) =>
				Fetch(new WWW(url, postData, headers), observer, progress, cancellation));
		}

		public static IObservable<WWW> PostWWW(string url, WWWForm content, IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) =>
				Fetch(new WWW(url, content), observer, progress, cancellation));
		}

		public static IObservable<WWW> PostWWW(string url, WWWForm content, Hash headers,
			IProgress<float> progress = null)
		{
			Hash contentHeaders = content.headers;
			return ObservableUnity.FromCoroutine<WWW>((observer, cancellation) =>
				Fetch(new WWW(url, content.data, MergeHash(contentHeaders, headers)), observer, progress,
					cancellation));
		}

		public static IObservable<AssetBundle> LoadFromCacheOrDownload(string url, int version,
			IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<AssetBundle>((observer, cancellation) =>
				FetchAssetBundle(WWW.LoadFromCacheOrDownload(url, version), observer, progress, cancellation));
		}

		public static IObservable<AssetBundle> LoadFromCacheOrDownload(string url, int version, uint crc,
			IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<AssetBundle>((observer, cancellation) =>
				FetchAssetBundle(WWW.LoadFromCacheOrDownload(url, version, crc), observer, progress, cancellation));
		}

		// over Unity5 supports Hash128
#if !(UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0 || UNITY_2_6_1 || UNITY_2_6)
		public static IObservable<AssetBundle> LoadFromCacheOrDownload(string url, Hash128 hash128,
			IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<AssetBundle>((observer, cancellation) =>
				FetchAssetBundle(WWW.LoadFromCacheOrDownload(url, hash128), observer, progress, cancellation));
		}

		public static IObservable<AssetBundle> LoadFromCacheOrDownload(string url, Hash128 hash128, uint crc,
			IProgress<float> progress = null)
		{
			return ObservableUnity.FromCoroutine<AssetBundle>((observer, cancellation) =>
				FetchAssetBundle(WWW.LoadFromCacheOrDownload(url, hash128, crc), observer, progress, cancellation));
		}
#endif

		// over 4.5, Hash define is Dictionary.
		// below Unity 4.5, WWW only supports Hashtable.
		// Unity 4.5, 4.6 WWW supports Dictionary and [Obsolete]Hashtable but WWWForm.content is Hashtable.
		// Unity 5.0 WWW only supports Dictionary and WWWForm.content is also Dictionary.
#if !(UNITY_METRO || UNITY_WP8) && (UNITY_4_5 || UNITY_4_6 || UNITY_4_7)
        static Hash MergeHash(Hashtable wwwFormHeaders, Hash externalHeaders)
        {
            var newHeaders = new Hash();
            foreach (DictionaryEntry item in wwwFormHeaders)
            {
                newHeaders[item.Key.ToString()] = item.Value.ToString();
            }
            foreach (HashEntry item in externalHeaders)
            {
                newHeaders[item.Key] = item.Value;
            }
            return newHeaders;
        }
#else
		private static Hash MergeHash(Hash wwwFormHeaders, Hash externalHeaders)
		{
			foreach (HashEntry item in externalHeaders)
			{
				wwwFormHeaders[item.Key] = item.Value;
			}

			return wwwFormHeaders;
		}
#endif

		private static IEnumerator Fetch(WWW www, IObserver<WWW> observer, IProgress<float> reportProgress,
			CancellationToken cancel)
		{
			using (www)
			{
				if (reportProgress != null)
				{
					while (!www.isDone && !cancel.IsCancellationRequested)
					{
						try
						{
							reportProgress.Report(www.progress);
						}
						catch (Exception ex)
						{
							observer.OnError(ex);
							yield break;
						}

						yield return null;
					}
				}
				else
				{
					if (!www.isDone)
					{
						yield return www;
					}
				}

				if (cancel.IsCancellationRequested)
				{
					yield break;
				}

				if (reportProgress != null)
				{
					try
					{
						reportProgress.Report(www.progress);
					}
					catch (Exception ex)
					{
						observer.OnError(ex);
						yield break;
					}
				}

				if (!string.IsNullOrEmpty(www.error))
				{
					observer.OnError(new WWWErrorException(www, www.text));
				}
				else
				{
					observer.OnNext(www);
					observer.OnCompleted();
				}
			}
		}

		private static IEnumerator FetchText(WWW www, IObserver<string> observer, IProgress<float> reportProgress,
			CancellationToken cancel)
		{
			using (www)
			{
				if (reportProgress != null)
				{
					while (!www.isDone && !cancel.IsCancellationRequested)
					{
						try
						{
							reportProgress.Report(www.progress);
						}
						catch (Exception ex)
						{
							observer.OnError(ex);
							yield break;
						}

						yield return null;
					}
				}
				else
				{
					if (!www.isDone)
					{
						yield return www;
					}
				}

				if (cancel.IsCancellationRequested)
				{
					yield break;
				}

				if (reportProgress != null)
				{
					try
					{
						reportProgress.Report(www.progress);
					}
					catch (Exception ex)
					{
						observer.OnError(ex);
						yield break;
					}
				}

				if (!string.IsNullOrEmpty(www.error))
				{
					observer.OnError(new WWWErrorException(www, www.text));
				}
				else
				{
					observer.OnNext(www.text);
					observer.OnCompleted();
				}
			}
		}

		private static IEnumerator FetchBytes(WWW www, IObserver<byte[]> observer, IProgress<float> reportProgress,
			CancellationToken cancel)
		{
			using (www)
			{
				if (reportProgress != null)
				{
					while (!www.isDone && !cancel.IsCancellationRequested)
					{
						try
						{
							reportProgress.Report(www.progress);
						}
						catch (Exception ex)
						{
							observer.OnError(ex);
							yield break;
						}

						yield return null;
					}
				}
				else
				{
					if (!www.isDone)
					{
						yield return www;
					}
				}

				if (cancel.IsCancellationRequested)
				{
					yield break;
				}

				if (reportProgress != null)
				{
					try
					{
						reportProgress.Report(www.progress);
					}
					catch (Exception ex)
					{
						observer.OnError(ex);
						yield break;
					}
				}

				if (!string.IsNullOrEmpty(www.error))
				{
					observer.OnError(new WWWErrorException(www, www.text));
				}
				else
				{
					observer.OnNext(www.bytes);
					observer.OnCompleted();
				}
			}
		}

		private static IEnumerator FetchAssetBundle(WWW www, IObserver<AssetBundle> observer,
			IProgress<float> reportProgress, CancellationToken cancel)
		{
			using (www)
			{
				if (reportProgress != null)
				{
					while (!www.isDone && !cancel.IsCancellationRequested)
					{
						try
						{
							reportProgress.Report(www.progress);
						}
						catch (Exception ex)
						{
							observer.OnError(ex);
							yield break;
						}

						yield return null;
					}
				}
				else
				{
					if (!www.isDone)
					{
						yield return www;
					}
				}

				if (cancel.IsCancellationRequested)
				{
					yield break;
				}

				if (reportProgress != null)
				{
					try
					{
						reportProgress.Report(www.progress);
					}
					catch (Exception ex)
					{
						observer.OnError(ex);
						yield break;
					}
				}

				if (!string.IsNullOrEmpty(www.error))
				{
					observer.OnError(new WWWErrorException(www, ""));
				}
				else
				{
					observer.OnNext(www.assetBundle);
					observer.OnCompleted();
				}
			}
		}
	}

	public class WWWErrorException : Exception
	{
		// cache the text because if www was disposed, can't access it.
		public WWWErrorException(WWW www, string text)
		{
			WWW = www;
			RawErrorMessage = www.error;
			ResponseHeaders = www.responseHeaders;
			HasResponse = false;
			Text = text;

			string[] splitted = RawErrorMessage.Split(' ', ':');
			if (splitted.Length != 0)
			{
				int statusCode;
				if (int.TryParse(splitted[0], out statusCode))
				{
					HasResponse = true;
					StatusCode = (HttpStatusCode) statusCode;
				}
			}
		}

		public string RawErrorMessage { get; }
		public bool HasResponse { get; }
		public string Text { get; }
		public HttpStatusCode StatusCode { get; }
		public Dictionary<string, string> ResponseHeaders { get; }
		public WWW WWW { get; }

		public override string ToString()
		{
			string text = Text;
			if (string.IsNullOrEmpty(text))
			{
				return RawErrorMessage;
			}

			return RawErrorMessage + " " + text;
		}
	}
}

#if UNITY_2018_3_OR_NEWER
#pragma warning restore CS0618
#endif