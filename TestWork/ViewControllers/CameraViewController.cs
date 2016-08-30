using Foundation;
using System;
using UIKit;
using AVFoundation;
using System.Threading.Tasks;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using System.IO;
using Photos;
using AudioToolbox;
using CoreFoundation;
using CoreImage;

namespace TestWork
{
    public partial class CameraViewController : UIViewController
	{
		bool flashOn = false;

		UIVisualEffectView blurEffectView;
		UIButton switchCameraButton, flashButton, photoLibraryButton;
		RoundPhotoButton takePhotoButton;
		UISwitch switchMode;

		RoundPhotoButton.PhotoButtonType currentState;
		SystemSound bleepIn, bleepOut;

		CameraController camController;
		UIImagePickerController imagePicker = new UIImagePickerController();


		public RoundPhotoButton.PhotoButtonType CurrentState
		{
			get
			{
				return currentState;
			}

			set
			{
				currentState = value;
				takePhotoButton.SwitchButton(currentState);

			}
		}

		private bool isRecording
		{
			get { return camController.isRecording; }
		}

		public CameraViewController(IntPtr handle) : base (handle)
		{
		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.BackgroundColor = UIColor.Black;

			var urlIn = NSUrl.FromFilename("BleepIn.aiff");
			var urlOut = NSUrl.FromFilename("BleepOut.aiff");
			bleepIn = new SystemSound(urlIn);
			bleepOut= new SystemSound(urlOut);

			NavigationController.NavigationBarHidden = false;
			var NavigationBarHeight = NavigationController.NavigationBar.Frame.Bottom;
			Title = "";
			var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
			var blurNavigationBar = new UIVisualEffectView(blur)
			{
				Frame = new CGRect(0,0,View.Frame.Width,NavigationBarHeight)
			};

			liveCameraStream.SizeThatFits(View.Bounds.Size);
			camController = new CameraController(liveCameraStream);
			await camController.AuthorizeCameraUse();
			camController.FinishRecording += FinishedRecording;

			var blurEffectView = new UIVisualEffectView(blur)
			{
				Frame = new CGRect(0, View.Bounds.Height - 90, View.Frame.Width, 90)
			};

			imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
			imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
			imagePicker.Canceled += Handle_Canceled;

			takePhotoButton = new RoundPhotoButton(RoundPhotoButton.PhotoButtonType.PhotoButton,new CGRect(View.Bounds.Width / 2 - 35, View.Bounds.Bottom - 80, 70, 70));
			takePhotoButton.button.SetTitle("", UIControlState.Normal);
			takePhotoButton.button.TouchDown += TakePhotoButton_TouchDown;
		
			switchCameraButton = new UIButton(UIButtonType.Custom);
			switchCameraButton.SetImage(UIImage.FromFile("SwapCamera.png"), UIControlState.Normal);
			switchCameraButton.SetTitle("Switch", UIControlState.Normal);
			switchCameraButton.TouchDown += SwitchCameraButton_TouchDown;
			switchCameraButton.Frame = new CGRect(View.Bounds.Width - 50,NavigationBarHeight+10, 40, 40);

			photoLibraryButton = new UIButton(UIButtonType.Custom);
			photoLibraryButton.TouchDown += (sender, e) => { GetMediaFromLibrary();};
			photoLibraryButton.SetBackgroundImage(PhotoLibraryController.GetThumbnailLastPhoto(), UIControlState.Normal);
			photoLibraryButton.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			photoLibraryButton.Frame = new CGRect(View.Bounds.Width - 70, View.Bounds.Bottom - 70, 50, 50);
			photoLibraryButton.Layer.CornerRadius = photoLibraryButton.Bounds.Height / 4;
			photoLibraryButton.ClipsToBounds = true;
			photoLibraryButton.Layer.BorderWidth = 1;
			photoLibraryButton.Layer.BorderColor = UIColor.White.CGColor;

			flashButton = new UIButton(UIButtonType.Custom);
			flashButton.SetImage(UIImage.FromFile("FlashOFF.png"), UIControlState.Normal);
			flashButton.SetImage(UIImage.FromFile("FlashON.png"), UIControlState.Selected);
			flashButton.TouchDown += FlashButton_TouchDown;
			flashButton.Frame = new CGRect(10, NavigationBarHeight + 10, 40, 40);

			switchMode = new UISwitch();
			switchMode.Frame = new CGRect(10, View.Bounds.Bottom - 45, 35, 35);
			switchMode.On = false;
			switchMode.OnTintColor = UIColor.LightGray;
			switchMode.ValueChanged+= SwitchMode_ValueChanged;
			var cameraIcon = new UIImageView(UIImage.FromFile("CameraIcon.png"));
			cameraIcon.Frame = new CGRect(20, View.Bounds.Bottom - 80, 35, 35);

			currentState = RoundPhotoButton.PhotoButtonType.PhotoButton;

			View.AddSubview(blurNavigationBar);
			View.AddSubview(blurEffectView);
			View.AddSubview(takePhotoButton);
			View.AddSubview(switchCameraButton);
			View.AddSubview(photoLibraryButton);
			View.AddSubview(flashButton);
			View.AddSubview(switchMode);
			View.AddSubview(cameraIcon);
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}

		async void TakePhotoButton_TouchDown(object sender, EventArgs e)
		{
			if (currentState == RoundPhotoButton.PhotoButtonType.PhotoButton)
			{
				var image = await camController.TakePhoto();
				StartEditPhoto(image);
			}
			else
			{
				takePhotoButton.SetRecordingMode(!isRecording);
				if (isRecording)
				{
					bleepOut.PlaySystemSound();
					switchCameraButton.Enabled = true;			
					switchMode.Enabled = true;
					camController.StopRecordVideo();
				}
				else {
					bleepIn.PlaySystemSound();
					switchCameraButton.Enabled = false;
					switchMode.Enabled = false;
					camController.StartRecordVideo();
				}
			}
		}

	 	void SwitchCameraButton_TouchDown(object sender, EventArgs e)
		{
			camController.SwitchCamera();
		}

		void FlashButton_TouchDown(object sender, EventArgs e)
		{
			var device = camController.Device;

			var error = new NSError();
			if (device.HasFlash)
			{
				if (device.FlashMode == AVCaptureFlashMode.On)
				{
					device.LockForConfiguration(out error);
					device.FlashMode = AVCaptureFlashMode.Off;
					device.UnlockForConfiguration();
					flashButton.Selected = false;

				}
				else {
					device.LockForConfiguration(out error);
					device.FlashMode = AVCaptureFlashMode.On;
					device.UnlockForConfiguration();
					flashButton.Selected = true;
				}
			}

			flashOn = !flashOn;
		}

		void SwitchMode_ValueChanged(object sender, EventArgs e)
		{
			if ((sender as UISwitch).On)
			{
				CurrentState = RoundPhotoButton.PhotoButtonType.VideoButton;
				flashButton.Hidden = true;
			}
			else
			{
				CurrentState = RoundPhotoButton.PhotoButtonType.PhotoButton;
				flashButton.Hidden = false;
			}
		}

		public void FinishedRecording(NSUrl outputFileUrl)
		{
			StartEditVideo(outputFileUrl);
		}

		void StartEditPhoto(UIImage image)
		{
			PhotoEditorViewController controller = Storyboard.InstantiateViewController("PhotoEditorViewController") as PhotoEditorViewController;
			controller.Title = "Обработка фото";
			controller.Image = image;
			NavigationController.PushViewController(controller, true);
		}

		void StartEditVideo(NSUrl url)
		{
			
			VideoEditorViewController controller = Storyboard.InstantiateViewController("VideoEditorViewController") as VideoEditorViewController;
			controller.Title = "Обработка видео";
			controller.VideoPath = url;
			NavigationController.PushViewController(controller, true);
		}

		public void GetMediaFromLibrary()
		{
			NavigationController.PresentModalViewController(imagePicker, true);
		}

		private void Handle_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			bool isImage = false;

			switch (e.Info[UIImagePickerController.MediaType].ToString())
			{
				case "public.image":
					isImage = true;
					break;
				case "public.video":
					break;
			}

			NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;


			if (isImage)
			{
				UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
				if (originalImage != null)
				{
					StartEditPhoto(originalImage);
				}
			}
			else
			{
				NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
				if (mediaURL != null)
				{
					StartEditVideo(mediaURL);
				}
			}

			imagePicker.DismissModalViewController(true);
		}

		void Handle_Canceled(object sender, EventArgs e)
		{
			imagePicker.DismissModalViewController(true);
		}

		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.LightContent;
		}
	}
}