using System;
using CoreImage;
using UIKit;

namespace TestWork
{
	public class TemperatureFilter : Filter
	{
		public TemperatureFilter (): base("Hot")
		{
			
		}

		public override CIImage ApplyFilter (CIImage image)
		{
			using (var temperature = new CITemperatureAndTint()
			{
				Image = image,
				Neutral = new CIVector(6500, 0), // Default [6500, 0]
				TargetNeutral = new CIVector(2000, 0), // Default [6500, 0]
			})
			{

				using (var filter = new CIPhotoEffectChrome()
				{
					Image = temperature.OutputImage
				})
				{
					return filter.OutputImage;
				}
			}
		}
	}

}

