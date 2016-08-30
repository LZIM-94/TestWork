using System;
using CoreImage;
using UIKit;

namespace TestWork
{
	public class ExposureAdjustFilter : Filter
	{
		public ExposureAdjustFilter (): base("Exposure")
		{
		}

		public override CIImage ApplyFilter (CIImage image)
		{
			using (var exposureAdjust = new CIExposureAdjust()
			{
				Image = image,
				EV = 2F // Default value: 0.50 Minimum: 0.00 Maximum: 0.00 Slider minimum: -10.00 Slider maximum: 10.00 Identity: 0.00
			})
			{
				return exposureAdjust.OutputImage;
			}
		}
	}
}

