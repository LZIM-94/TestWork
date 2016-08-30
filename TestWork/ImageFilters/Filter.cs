using System;
using CoreGraphics;
using CoreImage;
using UIKit;

namespace TestWork
{
	public abstract class Filter
	{
		public Filter(string name)
		{
			Name = name;
		}

	
		public string Name { get; set; }

		public UIImage ProcessingImage(UIImage image)
		{
			return UIImage.FromImage(ProcessingImageToCGImage(image));
		}

		public CGImage ProcessingImageToCGImage(UIImage image)
		{
			using (CIImage inputImage = CIImage.FromCGImage(image.CGImage))
			{
				using (CIImage imageWithOrientation = inputImage.CreateWithOrientation(ImageTransformation.Convert(image.Orientation)))
				{
					using (CIImage outputImage = ApplyFilter(imageWithOrientation))
					{
						using (CIContext context = CIContext.FromOptions(null))
						{
							CGImage cgImage = context.CreateCGImage(outputImage, outputImage.Extent);
							return cgImage;
						}
					}
				}
			}
		}


		public abstract CIImage ApplyFilter(CIImage ciImage);

	}

	public class NoneFilter : Filter
	{
		public NoneFilter() : base("None")
		{

		}

		public override CIImage ApplyFilter(CIImage image)
		{
			return image;
		}
	}
}







