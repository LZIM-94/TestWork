using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace TestWork
{
	public class SelectionRectangle:CALayer
	{
		public CGRect Rectangle;
		private CAShapeLayer selectionRect; 

		public SelectionRectangle(CGPoint beginPoint, CGSize beginSize)
		{
			CGRect frameRect = new CGRect(beginPoint.X, beginPoint.Y, beginSize.Width, beginSize.Height);
			Rectangle = new CGRect(beginPoint, beginSize);

			this.Frame = frameRect;

			var path = CGPath.FromRect(Rectangle);
			selectionRect = new CAShapeLayer();
			selectionRect.Path = path;
			selectionRect.FillColor = null;
			selectionRect.LineWidth = 4;
			selectionRect.StrokeColor = UIColor.White.CGColor;
			selectionRect.LineDashPattern = new NSNumber[] { 2, 4 };

			this.AddSublayer(selectionRect);
		}

		private CGRect GetRectFromPoints(CGPoint point1, CGPoint point2)
		{
			var minX = point1.X;
			var maxX = point2.X;
			if (point1.X > point2.X) { minX = point2.X; maxX = point1.X;}
			var minY = point1.Y;
			var maxY = point2.Y;
			if (point1.Y > point2.X) { minY = point2.Y; maxY = point1.Y; }
			var width = maxX - minX;
			var height = maxY - minY;
			return new CGRect(minX, minY, width, height);
		}

		public void Draw(CGPoint point1,CGPoint point2)
		{
			Rectangle = GetRectFromPoints(point1, point2);
			var path = CGPath.FromRect(Rectangle);
			selectionRect.Path = path;
		}
	}
}

