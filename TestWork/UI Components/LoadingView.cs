using System;
using UIKit;
namespace TestWork
{
	public class LoadingView:UIView
	{
		public LoadingView()
		{
			Frame = UIScreen.MainScreen.Bounds;

			var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
			var BlurView = new UIVisualEffectView(blur)
			{
				Frame = Frame
			};
			var label = new UILabel(new CoreGraphics.CGRect(BlurView.Frame.Width/2-100,BlurView.Frame.Height/2-100,200,200));
			label.Text = "Сохранение...";
			label.TextAlignment = UITextAlignment.Center;
			label.ContentMode = UIViewContentMode.ScaleToFill;
			label.MinimumFontSize = 30;
			label.TextColor = UIColor.White;
			BlurView.AddSubview(label);
			AddSubview(BlurView);
		}

	}
}

