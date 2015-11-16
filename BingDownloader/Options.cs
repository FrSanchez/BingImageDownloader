using CommandLineParser;

namespace BingDownloader
{
	class Options
	{
		[Argument (ArgumentTypes.AtMostOnce, HelpText = "Folder to save the downloaded files, will use the binary folder by default", ShortName = "d")]
		public string Destination;
	}
}
