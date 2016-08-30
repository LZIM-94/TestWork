using System;
using CoreImage;
using UIKit;

namespace TestWork
{
	public class ColorMatrixFilter : Filter
	{
		public ColorMatrixFilter (): base("Cold")
		{
		}

		public override CIImage ApplyFilter (CoreImage.CIImage image)
		{

			using (var colorMatrix = new CIPhotoEffectProcess()
			{
				Image = image,
			})
			{

				return colorMatrix.OutputImage;
			}
		}
	}
}

