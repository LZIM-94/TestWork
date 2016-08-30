using System;
using CoreGraphics;
using CoreImage;
using Foundation;
using UIKit;

namespace TestWork
{
	public static class ImageTransformation
	{
		private static UIImageOrientation lastOrientation = UIImageOrientation.Right;

		public static UIImage Rotate(UIImage imageToRotate, bool isCounterClockWise)
		{
			switch (imageToRotate.Orientation)
			{
				case UIImageOrientation.Up:
					lastOrientation = isCounterClockWise ? UIImageOrientation.Right : UIImageOrientation.Left;
					break;
				case UIImageOrientation.Down:
					lastOrientation = isCounterClockWise ? UIImageOrientation.RightMirrored : UIImageOrientation.LeftMirrored;
					break;
				case UIImageOrientation.Left:
					lastOrientation = isCounterClockWise ? UIImageOrientation.Up : UIImageOrientation.Down;
					break;
				case UIImageOrientation.Right:
					lastOrientation = isCounterClockWise ? UIImageOrientation.Down : UIImageOrientation.Up;
					break;
				case UIImageOrientation.UpMirrored:
					lastOrientation = isCounterClockWise ? UIImageOrientation.RightMirrored : UIImageOrientation.LeftMirrored;
					break;
				case UIImageOrientation.DownMirrored:
					lastOrientation = isCounterClockWise ? UIImageOrientation.LeftMirrored : UIImageOrientation.RightMirrored;
					break;
				case UIImageOrientation.LeftMirrored:
					lastOrientation = isCounterClockWise ? UIImageOrientation.UpMirrored : UIImageOrientation.DownMirrored;
					break;
				case UIImageOrientation.RightMirrored:
					lastOrientation = isCounterClockWise ? UIImageOrientation.DownMirrored : UIImageOrientation.UpMirrored;
					break;
			}

			imageToRotate = UIImage.FromImage(imageToRotate.CGImage, imageToRotate.CurrentScale, lastOrientation);


			return imageToRotate;
		}

		public static UIImage MaxResizeImage(this UIImage sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1) return sourceImage;
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext(new CGSize(width, height));
			sourceImage.Draw(new CGRect(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}

		public static UIImage CropImage(UIImage sourceImage, CGRect rect)
		{
			if (sourceImage.Orientation == (UIImageOrientation.Right | UIImageOrientation.Left))
				rect = new CGRect(rect.Y, rect.X, rect.Height, rect.Width);
			using (var inputImage = CIImage.FromCGImage(sourceImage.CGImage))
			{
				using (CIImage imageWithOrientation = inputImage.ImageByCroppingToRect(rect))
				{
					using (CIImage outputImage = imageWithOrientation.CreateWithOrientation(Convert(sourceImage.Orientation)))
					{

						using (CIContext context = CIContext.FromOptions(null))
						{
							CGImage cgImage = context.CreateCGImage(outputImage, outputImage.Extent);
							return UIImage.FromImage(cgImage);
						}
					}
				}
			}
			
		}


		public static CIImageOrientation Convert(UIImageOrientation imageOrientation)
		{
			switch (imageOrientation)
			{
				case UIImageOrientation.Up:
					return CIImageOrientation.TopLeft;
				case UIImageOrientation.Down:
					return CIImageOrientation.BottomRight;
				case UIImageOrientation.Left:
					return CIImageOrientation.LeftBottom;
				case UIImageOrientation.Right:
					return CIImageOrientation.RightTop;
				case UIImageOrientation.UpMirrored:
					return CIImageOrientation.TopRight;
				case UIImageOrientation.DownMirrored:
					return CIImageOrientation.BottomLeft;
				case UIImageOrientation.LeftMirrored:
					return CIImageOrientation.LeftTop;
				case UIImageOrientation.RightMirrored:
					return CIImageOrientation.RightBottom;
				default:
					throw new NotImplementedException();
			}
		}

	}
}


