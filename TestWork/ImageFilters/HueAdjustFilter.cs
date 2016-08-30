using System;
using CoreImage;
using UIKit;

namespace TestWork
{
	public class HueAdjustFilter : Filter
	{
		public HueAdjustFilter() : base("Drugs")
		{
			
		}

		public override CIImage ApplyFilter (CIImage image)
		{


			using (var hueAdjust = new CIHueAdjust()
			{
				Image = image,
				Angle = 90F // Default is 0
			})
			{

				using (var contr = new CIColorControls()
				{
					Image = hueAdjust.OutputImage,
					Contrast = 1.5f

				})
				{

					return contr.OutputImage;
				}
			}
		}
	}
}

