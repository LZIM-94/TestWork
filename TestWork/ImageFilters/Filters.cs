using System;
using System.Collections.Generic;

namespace TestWork
{
	public static class Filters
	{
		public static List<Filter> GetAllFilters()
		{
			var filters = new List<Filter>();
			filters = new List<Filter>();
			filters.Add(new NoneFilter());
			filters.Add(new ColorCrossPolynomialFilter());
			filters.Add(new ColorMatrixFilter());
			filters.Add(new NoirFilter());
			filters.Add(new CartoonFilter());
			filters.Add(new ColorPolynomialFilter());
			filters.Add(new ExposureAdjustFilter());
			filters.Add(new HueAdjustFilter());
			filters.Add(new TemperatureFilter());
			filters.Add(new ToneFilter());
			return filters;
		}
	}
}

