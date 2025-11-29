using ImageMagick;
using ImageMagick.Formats;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TFU.MediaProcess
{
	public class ImageProcessing
	{
		/// <summary>
		/// Compress the image with the quality value 50.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static Image CompressImage(Image source)
		{
			Stream stream = new MemoryStream();
			MemoryStream memoryStream = new MemoryStream();
			source.Save(stream, ImageFormat.Png);
			stream.Position = 0;
			using (var image = new MagickImage(stream))
			{
				image.Write(memoryStream);
			}
			stream.Dispose();
			return Image.FromStream(memoryStream);
		}

		public static Image CompressImage50Path(string pathName, int standardPixel = 1000)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (var image = new MagickImage(pathName))
			{
				image.AutoOrient();
				var sourceWidth = 0M;
				var sourceHeight = 0M;
				if (image.Height < image.Width)
				{
					sourceHeight = standardPixel;
					sourceWidth = sourceHeight * ((decimal)image.Width / image.Height);
				}
				else
				{
					sourceWidth = standardPixel;
					sourceHeight = sourceWidth * ((decimal)image.Height / image.Width);
				}

				image.Strip();
				image.Density = new Density(72, 72, DensityUnit.PixelsPerInch);
				image.Resize((int)(sourceWidth), (int)(sourceHeight));
				image.Write(memoryStream);
			}
			return Image.FromStream(memoryStream);
		}

		public static Image CompressImageWebp(string pathName, int standardPixel)
		{
			//ghi file
			MemoryStream memoryStream = new MemoryStream();
			using (var image = new MagickImage(pathName))
			{
				image.AutoOrient();
				var sourceWidth = 0M;
				var sourceHeight = 0M;
				if (image.Height < image.Width)
				{
					sourceHeight = standardPixel;
					sourceWidth = sourceHeight * ((decimal)image.Width / image.Height);
				}
				else
				{
					sourceWidth = standardPixel;
					sourceHeight = sourceWidth * ((decimal)image.Height / image.Width);
				}

				image.Strip();
				image.Density = new Density(72, 72, DensityUnit.PixelsPerInch);
				image.Resize((int)(sourceWidth), (int)(sourceHeight));
				image.Settings.SetDefine(MagickFormat.WebP, "lossless", true);
				image.Write(memoryStream);
			}
			return Image.FromStream(memoryStream);
		}

		public static void CompressImageWebp(string pathName, string output, int minSide)
		{
			using (var image = new MagickImage(pathName))
			{
				image.AutoOrient();
				var sourceWidth = image.Width;
				var sourceHeight = image.Height;
				if (sourceWidth < sourceHeight)
				{
					double ratio = minSide / sourceWidth * 1.0;
					sourceWidth = minSide;
					sourceHeight = (int)(sourceHeight * ratio);
				}
				else
				{
					double ratio = minSide / sourceHeight * 1.0;
					sourceHeight = minSide;
					sourceWidth = (int)(sourceWidth * ratio);
				}
				image.Resize((int)(sourceWidth), (int)(sourceHeight));
				var defines = new WebPWriteDefines
				{
					Lossless = true,
					Method = 6
				};
				image.Write(output, defines);
			}
		}

		public static Image CompressImageWebpSpecificWidth(string pathName, int widthPixel)
		{
			//ghi file
			MemoryStream memoryStream = new MemoryStream();
			using (var image = new MagickImage(pathName))
			{
				var sourceWidth = widthPixel;
				var sourceHeight = sourceWidth * ((decimal)image.Height / image.Width);
				image.AutoOrient();
				image.Strip();
				image.Quality = 50;
				image.Density = new Density(72, 72, DensityUnit.PixelsPerInch);
				image.Resize((int)(sourceWidth), (int)(sourceHeight));
				image.Settings.SetDefine(MagickFormat.WebP, "lossless", true);
				image.Write(memoryStream);
			}
			return Image.FromStream(memoryStream);
		}
	}
}
