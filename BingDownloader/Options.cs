using CommandLineParser;
using System;

namespace BingDownloader
{
	internal class Options
	{
		[Argument(ArgumentTypes.AtMostOnce, HelpText = "Folder to save the downloaded files, will use the binary folder by default", ShortName = "d")]
		public string Destination;
	}
}
