using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
namespace TestWork
{
	public class RoundPhotoButton : UIView
	{
		public UIButton button;
		CircleLayer circleLayer, innerCircleLayer;
		PhotoButtonType type;

		public PhotoButtonType ButtonType
		{
			get
			{
				return type;
			}

			set
			{
				type = value;
			}
		}



		public RoundPhotoButton(PhotoButtonType type, CGRect frame)
		{
			Frame = frame;
			ButtonType = type;
			circleLayer = new CircleLayer();
			innerCircleLayer = new CircleLayer();
			this.innerCircleLayer.CornerRadius = this.Bounds.Height / 2;

			switch (ButtonType)
			{
				case PhotoButtonType.PhotoButton: CreatePhotoButton(); break;
				case PhotoButtonType.VideoButton: CreateVideoButton(); break;
			}

			Layer.AddSublayer(circleLayer);
			Layer.AddSublayer(innerCircleLayer);
			innerCircleLayer.SetNeedsDisplay();
			circleLayer.SetNeedsDisplay();

			button = new UIButton();
			button.Frame = Bounds;
			button.TouchDown += ButtonPressed;
			AddSubview(button);
		}

		public void SetRecordingMode(bool isRecording)
		{
			if (isRecording)
			{
				this.innerCircleLayer.CornerRadius = this.Bounds.Height / 8;
				this.innerCircleLayer.Frame = new CGRect(Bounds.Width / 4, Bounds.Height / 4, Bounds.Width / 2, Bounds.Height / 2);
			}
			else {
				this.innerCircleLayer.CornerRadius = this.Bounds.Height/2;
				this.innerCircleLayer.Frame = Bounds;
			}
		}


		public void ButtonPressed(object sender, EventArgs e)
		{
			if (type == PhotoButtonType.PhotoButton)
			{
				var colorAnimation = CABasicAnimation.FromKeyPath("circleColor");
				colorAnimation.Duration = 0.2;
				colorAnimation.AutoReverses = true;
				colorAnimation.RemovedOnCompletion = true;
				var color = UIColor.White.ColorWithAlpha(0.5f);
				colorAnimation.To = new NSObject(color.CGColor.Handle);
				innerCircleLayer.AddAnimation(colorAnimation, "colorAnimation");
			}
		}


		public void SwitchButton(PhotoButtonType type)
		{
			var colorAnimation = CABasicAnimation.FromKeyPath("circleColor");
			colorAnimation.Duration = 0.3;
			var radiusAnimation = CABasicAnimation.FromKeyPath("radius");
			radiusAnimation.Duration = 0.3;
			var thicknessAnimation = CABasicAnimation.FromKeyPath("thickness");
			thicknessAnimation.Duration = 0.3;

			var startRadiusOuter = circleLayer.Radius;
			var startThicknesOuter = circleLayer.Thickness;
			var startColorOuter = circleLayer.Color;
			var startRadiusInner = innerCircleLayer.Radius;
			var startThicknesInner = innerCircleLayer.Thickness;
			var startColorInner = innerCircleLayer.Color;


			switch (type)
			{
				case PhotoButtonType.PhotoButton:
					{
						CATransaction.DisableActions = true;

						CreatePhotoButton();

						radiusAnimation.From = NSNumber.FromDouble(startRadiusOuter);
						thicknessAnimation.From = NSNumber.FromDouble(startThicknesOuter);
						colorAnimation.From = new NSObject(startColorOuter.Handle);

						radiusAnimation.To = NSNumber.FromDouble(35);
						thicknessAnimation.To = NSNumber.FromDouble(5);
						colorAnimation.To = new NSObject(UIColor.White.CGColor.Handle);

						circleLayer.AddAnimation(radiusAnimation, "radiusAnimation");
						circleLayer.AddAnimation(thicknessAnimation, "thicknessAnimation");
						circleLayer.AddAnimation(colorAnimation, "colorAnimation");

						radiusAnimation.From = NSNumber.FromDouble(startRadiusInner);
						thicknessAnimation.From = NSNumber.FromDouble(startThicknesInner);
						colorAnimation.From = new NSObject(startColorInner.Handle);

						radiusAnimation.To = NSNumber.FromDouble(27);
						thicknessAnimation.To = NSNumber.FromDouble(27);
						colorAnimation.To = new NSObject(UIColor.White.CGColor.Handle);

						innerCircleLayer.AddAnimation(radiusAnimation, "radiusAnimation");
						innerCircleLayer.AddAnimation(thicknessAnimation, "thicknessAnimation");
						innerCircleLayer.AddAnimation(colorAnimation, "colorAnimation");

						break;

					}
				case PhotoButtonType.VideoButton:
					{
						CATransaction.DisableActions = true;
						CreateVideoButton();

						radiusAnimation.From = NSNumber.FromDouble(startRadiusOuter);
						thicknessAnimation.From = NSNumber.FromDouble(startThicknesOuter);
						colorAnimation.From = new NSObject(startColorOuter.Handle);

						radiusAnimation.To = NSNumber.FromDouble(35);
						thicknessAnimation.To = NSNumber.FromDouble(3);
						colorAnimation.To = new NSObject(UIColor.White.CGColor.Handle);

						circleLayer.AddAnimation(radiusAnimation, "radiusAnimation");
						circleLayer.AddAnimation(thicknessAnimation, "thicknessAnimation");
						circleLayer.AddAnimation(colorAnimation, "colorAnimation");

						radiusAnimation.From = NSNumber.FromDouble(startRadiusInner);
						thicknessAnimation.From = NSNumber.FromDouble(startThicknesInner);
						colorAnimation.From = new NSObject(startColorInner.Handle);

						radiusAnimation.To = NSNumber.FromDouble(29);
						thicknessAnimation.To = NSNumber.FromDouble(29);
						colorAnimation.To = new NSObject(UIColor.Red.CGColor.Handle);

						innerCircleLayer.AddAnimation(radiusAnimation, "radiusAnimation");
						innerCircleLayer.AddAnimation(thicknessAnimation, "thicknessAnimation");
						innerCircleLayer.AddAnimation(colorAnimation, "colorAnimation");

						break;
					}
			}
		}

		private void CreateVideoButton()
		{
			circleLayer.Color = UIColor.White.CGColor;
			circleLayer.Thickness = 3;
			circleLayer.Radius = 35;
			circleLayer.Frame = Bounds;

			innerCircleLayer.Color = UIColor.Red.CGColor;
			innerCircleLayer.Thickness = 29;
			innerCircleLayer.Radius = 29;
			innerCircleLayer.Frame = Bounds;

		}

		private void CreatePhotoButton()
		{
			circleLayer.Color = UIColor.White.CGColor;
			circleLayer.Thickness = 5f;
			circleLayer.Radius = 35;
			circleLayer.Frame = Bounds;

			innerCircleLayer.Color = UIColor.White.CGColor;
			innerCircleLayer.Thickness = 27;
			innerCircleLayer.Radius = 27;
			innerCircleLayer.Frame = Bounds;

		}

		public enum PhotoButtonType
		{
			PhotoButton,
			VideoButton
		}
	}



	public class CircleLayer : CALayer
	{
		public CircleLayer()
		{
			ContentsScale = UIScreen.MainScreen.Scale;
		}

		[Export("initWithLayer:")]
		public CircleLayer(CALayer other)
			: base(other)
		{
		}

		public override void Clone(CALayer other)
		{
			CircleLayer o = (CircleLayer)other;
			Radius = o.Radius;
			Color = o.Color;
			Thickness = o.Thickness;
			base.Clone(other);
		}

		[Export("radius")]
		public double Radius { get; set; }

		[Export("thickness")]
		public double Thickness { get; set; }

		[Export("circleColor")]
		public CGColor Color { get; set; }

		[Export("needsDisplayForKey:")]
		static bool NeedsDisplayForKey(NSString key)
		{
			switch (key.ToString())
			{
				case "radius":
				case "thickness":
				case "circleColor":
					return true;
				default:
					return CALayer.NeedsDisplayForKey(key);
			}
		}


		public override void DrawInContext(CGContext context)
		{
			base.DrawInContext(context);

			CGPoint centerPoint = new CGPoint(this.Bounds.Width / 2, this.Bounds.Height / 2);
			CGColor glowColor = new UIColor(Color).ColorWithAlpha(0.85f).CGColor;
			double innerRadius = (Radius - Thickness) > 0 ? Radius - Thickness : 0;
			context.AddEllipseInRect(new CGRect(centerPoint.X - (float)Radius,
													centerPoint.Y - (float)Radius,
													(float)Radius * 2,
													(float)Radius * 2));
			context.AddEllipseInRect(new CGRect(centerPoint.X - (float)innerRadius,
													centerPoint.Y - (float)innerRadius,
													(float)innerRadius * 2,
													(float)innerRadius * 2));

			context.SetFillColor(Color);
			//context.SetShadow(CGSize.Empty, 10.0f, glowColor);
			context.EOFillPath();
		}
	}
}

