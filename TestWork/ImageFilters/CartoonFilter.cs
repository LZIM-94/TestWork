using System;
using CoreImage;

namespace TestWork
{
	public class CartoonFilter:Filter
	{
		public CartoonFilter() : base("Cartoon")
		{

		}

		public override CIImage ApplyFilter(CIImage image)
		{


			using (var gloom = new CIComicEffect()
			{
				Image = image,

			})
			{


				return gloom.OutputImage;
			}
		}
	}
}

