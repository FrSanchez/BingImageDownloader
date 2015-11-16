using CommandLineParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace BingDownloader
{
	internal class Downloader
	{
		private string baseBING = "http://www.bing.com";

		private WebClient client;

		private string destinationFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		private string FileName = string.Empty;

		private string OriginalFileName = string.Empty;

		private string ImagePath = string.Empty;

		private List<string> Locales;

		private string imageUrlPattern = "(g_img={url\\:'(?<url>.*)',id)";

		private Options options = new Options();

		private static void Main(string[] args)
		{
			try
			{
				Downloader downloader = new Downloader(args);
				downloader.ClientCountries();
				downloader.BatchDownload();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Thread.Sleep(10000);
			}
		}

		public Downloader(string[] args)
		{
			this.client = new WebClient();
			this.Locales = new List<string>();
			this.options.Destination = this.destinationFolder;
			Parser.ParseArgumentsWithUsage(args, this.options);
		}

		public void BatchDownload()
		{
			UriBuilder uriBuilder = new UriBuilder(this.baseBING);
			foreach (string current in this.Locales)
			{
				try
				{
					Console.WriteLine("Downloading {0}", current);
					uriBuilder.Query = string.Format("scope=web&setmkt={0}", current);
					StreamReader streamReader = new StreamReader(this.client.OpenRead(uriBuilder.Uri));
					string input = streamReader.ReadToEnd();
					this.ParseData(input);
					streamReader.Close();
					Trace.WriteLine(string.Format("loc {0} image {1} file {2}", current, this.ImagePath, this.FileName));
					if (!string.IsNullOrEmpty(this.FileName) && !File.Exists(this.FileName))
					{
						string uriString = string.Format("{0}/{1}", this.baseBING, this.ImagePath);
						Image image = this.DownloadImage(new Uri(uriString));
						this.SaveImage(image);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Ignoring this error...");
					Console.Error.WriteLine(ex.ToString());
				}
			}
		}

		public void ClientCountries()
		{
			StreamReader streamReader = new StreamReader(this.client.OpenRead("http://www.bing.com/account/general?FORM=O2HV46"));
			string input = streamReader.ReadToEnd();
			this.ListCountries(input);
			streamReader.Close();
		}

		public Image DownloadImage(Uri uri)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Method = "GET";
			WebResponse response = httpWebRequest.GetResponse();
			Stream responseStream = response.GetResponseStream();
			int.Parse(response.Headers["Content-Length"]);
			return Image.FromStream(responseStream);
		}

		private void ListCountries(string input)
		{
			foreach (Match match in Regex.Matches(input, "(mkt=([a-zA-Z\\-]*))"))
			{
				Trace.WriteLine(match.Value);
				string value = match.Groups[match.Groups.Count - 1].Value;
				if (!this.Locales.Contains(value))
				{
					this.Locales.Add(value);
				}
			}
		}

		private void ParseData(string input)
		{
			Match match = Regex.Match(input, this.imageUrlPattern);
			Console.WriteLine("Match: " + match.Value);
			string value = match.Groups["url"].Value;
			this.ImagePath = Uri.UnescapeDataString(Regex.Unescape(value));
			string[] array = this.ImagePath.Split(new char[]
				{
					'/'
				});
			this.OriginalFileName = array[array.Length - 1];
			string arg = "none";
			Trace.WriteLine(string.Format("Original filename: {0}", this.OriginalFileName));
			match = Regex.Match(this.OriginalFileName, "(?<name>[a-zA-Z0-9]+)_(?<locale>[a-zA-Z\\-]{5})*(?<suffix>.*)\\.(?<ext>.*)");
			if (match.Groups.Count > 1)
			{
				this.OriginalFileName = string.Format("{0}.{1}", match.Groups["name"], match.Groups["ext"]);
				arg = match.Groups["locale"].Value;
			}
			if (!string.IsNullOrEmpty(this.OriginalFileName))
			{
				this.FileName = Path.Combine(this.options.Destination, this.OriginalFileName);
			}
			else
			{
				this.FileName = string.Empty;
			}
			Trace.WriteLine(string.Format("New file {0} (locale={1})", this.FileName, arg));
		}

		private void SaveImage(Image image)
		{
			image.Save(this.FileName);
		}

		private void SaveImage(Stream stream)
		{
			if (!string.IsNullOrEmpty(this.FileName))
			{
				BinaryReader binaryReader = new BinaryReader(stream);
				byte[] buffer = new byte[2048];
				BinaryWriter binaryWriter = new BinaryWriter(File.Open(this.FileName, FileMode.Create));
				int num;
				do
				{
					num = binaryReader.Read(buffer, 0, 2048);
					if (num > 0)
					{
						binaryWriter.Write(buffer, 0, num);
					}
				}
				while (num > 0);
				binaryWriter.Close();
			}
		}
	}
}
