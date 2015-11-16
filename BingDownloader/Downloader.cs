using CommandLineParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BingDownloader
{
	class Downloader
	{
		const string baseBING = "http://www.bing.com";

		WebClient client;

		string destinationFolder = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);

		string FileName = string.Empty;

		string OriginalFileName = string.Empty;

		string ImagePath = string.Empty;

		List<string> Locales;

		const string imageUrlPattern = "(g_img={url\\:'(?<url>.*)',id)";

		Options options = new Options ();

		static void Main (string[] args)
		{
			try {
				var downloader = new Downloader (args);
				downloader.ClientCountries ();
				downloader.BatchDownload ();
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}

		public Downloader (string[] args)
		{
			client = new WebClient ();
			Locales = new List<string> ();
			options.Destination = destinationFolder;
			Parser.ParseArgumentsWithUsage (args, options);
		}

		public void BatchDownload ()
		{
			var uriBuilder = new UriBuilder (baseBING);
			foreach (string current in Locales) {
				try {
					Console.WriteLine ("Downloading {0}", current);
					uriBuilder.Query = string.Format ("scope=web&setmkt={0}", current);
					var streamReader = new StreamReader (client.OpenRead (uriBuilder.Uri));
					string input = streamReader.ReadToEnd ();
					ParseData (input);
					streamReader.Close ();
					Trace.WriteLine (string.Format ("loc {0} image {1} file {2}", current, ImagePath, FileName));
					if (!string.IsNullOrEmpty (FileName) && !File.Exists (FileName)) {
						string uriString = string.Format ("{0}/{1}", baseBING, ImagePath);
						Image image = DownloadImage (new Uri (uriString));
						SaveImage (image);
					}
				} catch (Exception ex) {
					Console.WriteLine ("Ignoring this error...");
					Console.Error.WriteLine (ex);
				}
			}
		}

		public void ClientCountries ()
		{
			var streamReader = new StreamReader (client.OpenRead ("http://www.bing.com/account/general?FORM=O2HV46"));
			string input = streamReader.ReadToEnd ();
			ListCountries (input);
			streamReader.Close ();
		}

		public Image DownloadImage (Uri uri)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create (uri);
			httpWebRequest.Method = "GET";
			WebResponse response = httpWebRequest.GetResponse ();
			Stream responseStream = response.GetResponseStream ();
			int.Parse (response.Headers ["Content-Length"]);
			return Image.FromStream (responseStream);
		}

		void ListCountries (string input)
		{
			foreach (Match match in Regex.Matches(input, "(mkt=([a-zA-Z\\-]*))")) {
				Trace.WriteLine (match.Value);
				string value = match.Groups [match.Groups.Count - 1].Value;
				if (!Locales.Contains (value)) {
					Locales.Add (value);
				}
			}
		}

		void ParseData (string input)
		{
			Match match = Regex.Match (input, imageUrlPattern);
			Console.WriteLine ("Match: " + match.Value);
			string value = match.Groups ["url"].Value;
			ImagePath = Uri.UnescapeDataString (Regex.Unescape (value));
			string[] array = ImagePath.Split (new char[] {
				'/'
			});
			OriginalFileName = array [array.Length - 1];
			string arg = "none";
			Trace.WriteLine (string.Format ("Original filename: {0}", OriginalFileName));
			match = Regex.Match (OriginalFileName, "(?<name>[a-zA-Z0-9]+)_(?<locale>[a-zA-Z\\-]{5})*(?<suffix>.*)\\.(?<ext>.*)");
			if (match.Groups.Count > 1) {
				OriginalFileName = string.Format ("{0}.{1}", match.Groups ["name"], match.Groups ["ext"]);
				arg = match.Groups ["locale"].Value;
			}
			FileName = !string.IsNullOrEmpty (OriginalFileName) ? Path.Combine (options.Destination, OriginalFileName) : string.Empty;
			Trace.WriteLine (string.Format ("New file {0} (locale={1})", FileName, arg));
		}

		void SaveImage (Image image)
		{
			image.Save (FileName);
		}

		void SaveImage (Stream stream)
		{
			if (!string.IsNullOrEmpty (FileName)) {
				var binaryReader = new BinaryReader (stream);
				var buffer = new byte[2048];
				var binaryWriter = new BinaryWriter (File.Open (FileName, FileMode.Create));
				int num;
				do {
					num = binaryReader.Read (buffer, 0, 2048);
					if (num > 0) {
						binaryWriter.Write (buffer, 0, num);
					}
				} while (num > 0);
				binaryWriter.Close ();
			}
		}
	}
}
