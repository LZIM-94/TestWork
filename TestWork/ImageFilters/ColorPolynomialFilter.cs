using System;
using CoreImage;

namespace TestWork
{
	public class ColorPolynomialFilter : Filter
	{
		public ColorPolynomialFilter (): base("Contrast")
		{
		}

		public override CIImage ApplyFilter (CIImage image)
		{
			using (var color_polynomial = new CIColorPolynomial()
			{
				Image = image,
				RedCoefficients = new CIVector(0, 0, 0, .4f),
				GreenCoefficients = new CIVector(0, 0, .5f, .8f),
				BlueCoefficients = new CIVector(0, 0, .5f, 1),
				AlphaCoefficients = new CIVector(0, 1, 1, 1),
			})
			{
				return color_polynomial.OutputImage;
			}
		}
	}
}

