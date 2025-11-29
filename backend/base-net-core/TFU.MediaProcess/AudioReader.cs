using ATL;
using System;

namespace TFU.MediaProcess
{
	public class AudioReader
	{
		public static AudioModel Read(string fileName)
		{
			try
			{
				double duration = 0;
				string ext = System.IO.Path.GetExtension(fileName);
				switch (ext)
				{
					case ".mp3":
						Track theTrack = new Track(fileName);
						duration = theTrack.DurationMs;
						break;
					default:
						theTrack = new Track(fileName);
						duration = theTrack.DurationMs;
						break;
				}
				return new AudioModel
				{
					Duration = duration,
					FileName = fileName
				};
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}
