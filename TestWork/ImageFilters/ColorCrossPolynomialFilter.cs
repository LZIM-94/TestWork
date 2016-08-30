using System;
using CoreGraphics;
using CoreImage;
using UIKit;

namespace TestWork
{
	public class ColorCrossPolynomialFilter : Filter
	{
		public ColorCrossPolynomialFilter() : base("Acid")
		{
		}

		public override CIImage ApplyFilter(CIImage image)
		{
			using (var contr = new CIColorControls()
			{
				Image = image,
				Contrast = 1.3f

			})
			{
				using (var falseColor = new CIFalseColor()
				{
					Image = contr.OutputImage,
					Color0 = new CIColor(new CGColor(51F, 0F, 255F)),
					Color1 = new CIColor(new CGColor(255F, 251F, 0F))
				})
				{
					return falseColor.OutputImage;
				}
			}
		}
	}
}


