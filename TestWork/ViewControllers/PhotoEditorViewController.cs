using Foundation;
using System;
using UIKit;
using AVKit;
using System.Collections.Generic;
using CoreImage;
using CoreGraphics;

namespace TestWork
{

	public partial class PhotoEditorViewController : UIViewController
	{
		private UIImageView ImageView;
		private UIImage image;
		private Filter currentFilter;
		private FilterCollectionView filterCollectionView;
		private UIToolbar toolBar;
		private UIBarButtonItem[] editorButtons, cropButtons;
		private List<Filter> filters;
		private bool isCropMode = false;

		static readonly NSString cellToken = new NSString("FilterCell");

		private UIImage FilteredImage
		{
			get
			{
				if (CurrentFilter != null)
				{
					var procImg = CurrentFilter.ProcessingImage(Image);
					return procImg;
				}
				return Image;
			}
		}

		public UIImage Image
		{
			get
			{
				return image;
			}

			set
			{
				image = value;
				if (ImageView != null)
				{
					ImageView.Image = FilteredImage;
					ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				}

			}
		}

		public Filter CurrentFilter
		{
			get
			{
				return currentFilter;
			}

			set
			{
				currentFilter = value;
				if (ImageView != null)
				{
					ImageView.Image = FilteredImage;
					ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				}
			}
		}


		public PhotoEditorViewController(IntPtr handle) : base(handle)
		{
		}

		public void InitializeToolBars()
		{
			var fontAttribute = new UITextAttributes();
			fontAttribute.Font = UIFont.SystemFontOfSize(26, UIFontWeight.Heavy);

			//Создание кнопок редактирования фотографии для панели инструментов

			var RotateLeft = new UIBarButtonItem("↰", UIBarButtonItemStyle.Done, (sender, e) => { Image = ImageTransformation.Rotate(Image, false); });
			RotateLeft.SetTitleTextAttributes(fontAttribute, UIControlState.Normal);
			var RotateRight = new UIBarButtonItem("↱", UIBarButtonItemStyle.Done, (sender, e) => { Image = ImageTransformation.Rotate(Image, true); });
			RotateRight.SetTitleTextAttributes(fontAttribute, UIControlState.Normal);
			var CropImage = new UIBarButtonItem("Обрезать", UIBarButtonItemStyle.Done, (sender, e) => { StartCropImage();});

			editorButtons = new UIBarButtonItem[] {
	new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				RotateLeft,
	new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				RotateRight,
	new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				CropImage,
	new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
			};

			//Создание кнопок обрезки фотографии для панели инструментов

			var Cancel = new UIBarButtonItem("Отменить", UIBarButtonItemStyle.Done, (sender, e) => { CompleteCropImage(true); });
			var Done = new UIBarButtonItem("ОК", UIBarButtonItemStyle.Done, (sender, e) => { CompleteCropImage(false);  });

			cropButtons = new UIBarButtonItem[] {
	new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				Cancel,
	new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
				Done,
	new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
			};

			var frame = new CGRect(0, filterCollectionView.Frame.Top - 40, View.Bounds.Width, 40);
			toolBar = new UIToolbar(frame);
			toolBar.SetBackgroundImage(new UIImage(), UIToolbarPosition.Any, UIBarMetrics.Default);
			toolBar.Translucent = true;
			toolBar.TintColor = UIColor.White;
			toolBar.SetItems(editorButtons, false);
		}

		void SetEditorToolBar()
		{
			toolBar.SetItems(editorButtons, true);
		}

		void SetCropToolBar()
		{
			toolBar.SetItems(cropButtons, true);
		}

		public override void ViewDidLoad()
		{
			//Установка изображения на задний фон
			UIImageView background = new UIImageView(UIImage.FromFile("Background.jpg")) { Frame = View.Bounds };
			background.ContentMode = UIViewContentMode.ScaleAspectFill;

			//Настройка NavigationBar и создание кнопки сохранения
			NavigationController.NavigationBarHidden = false;
			var NavigationBarHeight = NavigationController.NavigationBar.Frame.Bottom;
			this.NavigationItem.SetRightBarButtonItem(
				new UIBarButtonItem(UIBarButtonSystemItem.Action, (sender, args) =>
			{
				UIAlertController alertControll = UIAlertController.Create("Сохранение", "Сохранить фото в библиотеку?", UIAlertControllerStyle.Alert);

				alertControll.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, alert =>
					{
						PhotoLibraryController.SavePhoto(CurrentFilter.ProcessingImageToCGImage(Image));
					}));
				alertControll.AddAction(UIAlertAction.Create("Отменить", UIAlertActionStyle.Cancel, alert => { }));

				PresentViewController(alertControll, true, null);
			
			})
				, true);

			//Эффект размытия для некоторых элементов
			var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
			var blurNavigationBar = new UIVisualEffectView(blur)
			{
				Frame = new CGRect(0, 0, View.Frame.Width, NavigationBarHeight)
			};

			filters = Filters.GetAllFilters();

			//Создание колекции фильтров на экране
			filterCollectionView = new FilterCollectionView(new CGRect(0, View.Bounds.Height - 100, View.Bounds.Width, 100), filters, Image);
			filterCollectionView.RegisterClassForCell(typeof(FilterCell), cellToken);
			filterCollectionView.onSelectFilter += (filter) => { CurrentFilter = filter; };

			//Инициализация панелей инструментов
			InitializeToolBars();

			var blurBottomBar = new UIVisualEffectView(blur)
			{
				Frame = new CGRect(0, toolBar.Frame.Top, View.Frame.Width, View.Bounds.Height - toolBar.Frame.Top)
			};

			//Ставим по-умолчанию первый фильтр(NoneFilter)
			CurrentFilter = filters[0];

			//Элемент отображения обработанной фотографии
			ImageView = new UIImageView();
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			ImageView.Frame = View.Bounds;
			ImageView.UserInteractionEnabled = true;
			UIPanGestureRecognizer gesture = new UIPanGestureRecognizer();
			gesture.AddTarget(() => HandleDrag(gesture));
			ImageView.AddGestureRecognizer(gesture);
			if (Image != null) ImageView.Image = FilteredImage;

			View.AddSubview(background);
			View.AddSubview(ImageView);
			View.AddSubview(blurBottomBar);
			View.AddSubview(filterCollectionView);
			View.AddSubview(toolBar);
			View.AddSubview(blurNavigationBar);

			selectionRectangle = new SelectionRectangle(new CGPoint(0, 0), ImageView.Frame.Size);
			selectionRectangle.Hidden = true;
			ImageView.Layer.AddSublayer(selectionRectangle);
		}

		//Переход в режим обрезки фото
		void StartCropImage()
		{
			isCropMode = true;
			selectionRectangle.Hidden = false;
			SetCropToolBar();
		}

		//Завершение обрезки фото
		void CompleteCropImage(bool canceled)
		{
			if(!canceled)
				Image = ImageTransformation.CropImage(Image,ImageRectFromScreenRect(selectionRectangle.Rectangle));
			SetEditorToolBar();
			isCropMode = false;
			selectionRectangle.Hidden = true;
		}

		//Обработка ручной обрезки фото

		private CGPoint pointStart, pointEnd;
		private SelectionRectangle selectionRectangle;

		//Получения рамки на фото из рамки на экране
		private CGRect ImageRectFromScreenRect(CGRect rect)
		{
			nfloat widthScale = ImageView.Bounds.Size.Width / ImageView.Image.Size.Width;
			nfloat heightScale = ImageView.Bounds.Size.Height / ImageView.Image.Size.Height;

			rect.X = rect.X * (1 / widthScale);
			rect.Y = rect.Y * (1 / heightScale);
			rect.Width = rect.Width * (1 / widthScale);
			rect.Height = rect.Height * (1 / heightScale);

			return rect;
		}

		//Рисование рамки выделения
		public void HandleDrag(UIPanGestureRecognizer recognizer)
		{
			if (isCropMode)
			{
				if (recognizer.State == UIGestureRecognizerState.Began)
				{
					pointStart = recognizer.LocationInView(ImageView);
				}

				if (recognizer.State != (UIGestureRecognizerState.Cancelled | UIGestureRecognizerState.Failed | UIGestureRecognizerState.Possible))
				{
					pointEnd = recognizer.LocationInView(ImageView);
					selectionRectangle.Draw(pointStart, pointEnd);
				}
			}
		}

	

		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.Default;
		}

	}
}
