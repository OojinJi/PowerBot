using DotNetTools.SharpGrabber;
using DotNetTools.SharpGrabber.Converter;
using DotNetTools.SharpGrabber.Grabbed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBot.Services
{
    public class GrabberService
    {
        private static readonly HttpClient Client = new HttpClient();
        private static readonly HashSet<string> TempFiles = new HashSet<string>();

        public async Task grabber(string url)
        {
            var grabber = GrabberBuilder.New()
                .UseDefaultServices()
                .AddInstagram()
                .AddYouTube()
                .Build();
            FFmpeg.AutoGen.ffmpeg.RootPath = "@C:/Users/Oojin Ji/Downloads/ffmpeg - 2023 - 05 - 18 - git - 01d9a84ef5 - essentials_build/ffmpeg - 2023 - 05 - 18 - git - 01d9a84ef5 - essentials_build/bin/ffmpeg.exe";
            var result = await grabber.GrabAsync(new Uri(url, UriKind.Absolute));
			var videos = result.Resource<GrabbedMedia>();
        }
		private void GenerateOutputFile(string audioPath, string videoPath, GrabbedMedia videoStream)
		{
			var outputPath = "/TemoDownload";
			var merger = new MediaMerger(outputPath);
			merger.AddStreamSource(audioPath, MediaStreamType.Audio);
			merger.AddStreamSource(videoPath, MediaStreamType.Video);
			merger.OutputMimeType = videoStream.Format.Mime;
			merger.OutputShortName = videoStream.Format.Extension;
			merger.Build();
			Console.WriteLine($"Output file successfully created.");
		}

		private GrabbedMedia ChooseMonoMedia(GrabResult result, MediaChannels channel)
		{
			var resources = result.Resources<GrabbedMedia>()
				.Where(m => m.Channels == channel)
				.ToList();
			if (resources.Count == 0)
				return null;
			for (var i = 0; i < resources.Count; i++)
			{
				var resource = resources[i];
				Console.WriteLine($"{i}. {resource.Title ?? resource.FormatTitle ?? resource.Resolution}");
			}

			while (true)
			{
				Console.Write($"Choose the {channel} file: ");
				var choiceStr = Console.ReadLine();
				if (!int.TryParse(choiceStr, out var choice))
				{
					Console.WriteLine("Number expected.");
					continue;
				}
				if (choice < 0 || choice >= resources.Count)
				{
					Console.WriteLine("Invalid number.");
					continue;
				}
				return resources[choice];
			}
		}

		private async Task<string> DownloadMedia(GrabbedMedia media, IGrabResult grabResult)
		{
			Console.WriteLine("Downloading {0}...", media.Title ?? media.FormatTitle ?? media.Resolution);
			using var response = await Client.GetAsync(media.ResourceUri);
			response.EnsureSuccessStatusCode();
			using var downloadStream = await response.Content.ReadAsStreamAsync();
			using var resourceStream = await grabResult.WrapStreamAsync(downloadStream);
			var path = Path.GetTempFileName();
			using var fileStream = new FileStream(path, FileMode.Create);
			TempFiles.Add(path);
			await resourceStream.CopyToAsync(fileStream);
			return path;
		}
	}
}
