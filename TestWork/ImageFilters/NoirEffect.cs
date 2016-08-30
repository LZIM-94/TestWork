using System;
using CoreImage;

namespace TestWork
{
	public class NoirFilter:Filter
	{
		public NoirFilter(): base("Noir")
		{

		}

		public override CIImage ApplyFilter(CIImage image)
		{
			using (var noir = new CIPhotoEffectNoir()
			{
				Image = image

			})
			{

				using (var contr = new CIColorControls()
				{
					Image = noir.OutputImage,
					Contrast = 1.2f

				})
				{

					return contr.OutputImage;
				}
			}
		}
	}
}

