using System;
using CoreImage;
using UIKit;

namespace TestWork
{
	public class ToneFilter: Filter
	{
		public ToneFilter() : base("Creep")
		{
		}

		public override CIImage ApplyFilter (CIImage image)
		{
			using (var toneCurve = new CIToneCurve()
			{
				Image = image,
				Point0 = new CIVector(0, 0),
				Point1 = new CIVector(.1F, .5F),
				Point2 = new CIVector(.3F, .15F),
				Point3 = new CIVector(.6F, .6F),
				Point4 = new CIVector(1.1F, 1F),
			})
			{
				return toneCurve.OutputImage;
			}
		}
	}
}

