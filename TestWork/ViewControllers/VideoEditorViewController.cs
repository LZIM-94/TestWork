using Foundation;
using System;
using UIKit;
using AVKit;
using AVFoundation;
using System.Collections.Generic;
using CoreGraphics;
using System.IO;
using CoreVideo;
using CoreImage;
using System.Threading.Tasks;
using CoreMedia;
using System.Diagnostics;
using RangeSlider;

namespace TestWork
{
	public partial class VideoEditorViewController : UIViewController
	{
		public VideoEditorViewController(IntPtr handle) : base(handle)
		{
		}

		private NSUrl videoPath;
		private AVPlayerViewController playerController;
		private AVPlayer player;
		private Filter currentFilter;
		private FilterCollectionView filterCollectionView;
		private List<Filter> filters;
		private AVUrlAsset videoAsset;
		private AVVideoComposition composition;
		private LoadingView loadingView;
		private RangeSliderView rangeSlider;

		static readonly NSString cellToken = new NSString("FilterCell");

		public Filter CurrentFilter
		{
			get
			{
				return currentFilter;
			}

			set
			{
				currentFilter = value;
				if (VideoAsset != null)
				{
					InitializeVideoPlayer(videoAsset);

				}
			}
		}



		public NSUrl VideoPath
		{
			get
			{
				return videoPath;
			}

			set
			{
				videoPath = value;
				VideoAsset = new AVUrlAsset(videoPath,new NSDictionary());

			}
		}

		public AVUrlAsset VideoAsset
		{
			get
			{
				return videoAsset;
			}

			set
			{
				videoAsset = value;
				InitializeVideoPlayer(VideoAsset);
			}
		}


		public void InitializeVideoPlayer(AVUrlAsset asset)
		{
			if (playerController != null)
			{
				if(composition!=null)
					composition.Dispose();
				composition = AVVideoComposition.CreateVideoComposition(asset, (AVAsynchronousCIImageFilteringRequest request) =>
				{
					using (var source = request.SourceImage)
					{
						using (var output = CurrentFilter.ApplyFilter(source))
						{
							request.Finish(output, null);
							request.Dispose();
						}
					}
				});
				AVPlayerItem item = new AVPlayerItem(asset);
				item.SeekingWaitsForVideoCompositionRendering = true;
				item.VideoComposition = composition;
				player.ReplaceCurrentItemWithPlayerItem(item);
			}
		}

		public override void ViewDidLoad()
		{
			UIImageView background = new UIImageView(UIImage.FromFile("Background.jpg")) { Frame = View.Bounds }; ;
			background.ContentMode = UIViewContentMode.ScaleAspectFill;


			NavigationController.NavigationBarHidden = false;
			var NavigationBarHeight = NavigationController.NavigationBar.Frame.Bottom;
			this.NavigationItem.SetRightBarButtonItem(
				new UIBarButtonItem(UIBarButtonSystemItem.Action, (sender, args) =>
			{
				UIAlertController alertControll = UIAlertController.Create("Сохранение", "Сохранить видео в библиотеку?", UIAlertControllerStyle.Alert);

				alertControll.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, alert =>
			{
				RenderToFile(videoAsset);
			}));
				alertControll.AddAction(UIAlertAction.Create("Отменить", UIAlertActionStyle.Cancel, alert => { }));

				PresentViewController(alertControll, true, null);

			})
				, true);



			var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
			var blurNavigationBar = new UIVisualEffectView(blur)
			{
				Frame = new CGRect(0, 0, View.Frame.Width, NavigationBarHeight)
			};

			filters = Filters.GetAllFilters();

			filterCollectionView = new FilterCollectionView(new CGRect(0, View.Bounds.Height - 100, View.Bounds.Width, 100), filters, null);
			filterCollectionView.RegisterClassForCell(typeof(FilterCell), cellToken);
			filterCollectionView.onSelectFilter += (filter) => { CurrentFilter = filter; };
			currentFilter = filters[0];

			var blurBottomBar = new UIVisualEffectView(blur)
			{
				Frame = filterCollectionView.Frame
			};

			playerController = new AVPlayerViewController();
			playerController.View.Frame = new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height - blurBottomBar.Bounds.Height);
			player = new AVPlayer();
			playerController.Player = player;

			View.AddSubview(background);
			AddChildViewController(playerController);
			View.AddSubview(playerController.View);
			if (VideoAsset != null) InitializeVideoPlayer(VideoAsset);

			View.AddSubview(blurNavigationBar);
			View.AddSubview(blurBottomBar);
			View.AddSubview(filterCollectionView); 

			loadingView = new LoadingView();
			loadingView.Hidden = true;
			View.AddSubview(loadingView);

		}

		void RenderToFile(AVUrlAsset video)
		{
			loadingView.Hidden = false;

			string downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string downloadFilePath = Path.Combine(downloadPath, "filteredImage.mp4");
			if (File.Exists(downloadFilePath))
			{
				File.Delete(downloadFilePath);
			}

			var asset = AVAsset.FromUrl(VideoPath);


			AVAssetExportSession export = new AVAssetExportSession(asset, AVAssetExportSession.PresetMediumQuality);

			export.OutputUrl = NSUrl.FromFilename(downloadFilePath);
			export.OutputFileType = AVFileType.Mpeg4;
			export.ShouldOptimizeForNetworkUse = true;
			export.VideoComposition = composition;


			Action handler = new Action(delegate ()
			{
				
				AVAssetExportSessionStatus status = export.Status;
				Console.WriteLine("Export status: " + status.ToString());
				InvokeOnMainThread(() =>
				{
					loadingView.Hidden = true;
				});

				if (File.Exists(downloadFilePath))
				{
					Console.WriteLine("Created");
					FinishProcessing(NSUrl.FromFilename(downloadFilePath));

				}
				else
					Console.WriteLine("Failed");
				//AVAssetExportSessionStatus
			});

			export.ExportAsynchronously(handler);

		}

		string GetTmpFilePath(string extension)
		{
			string outputFileName = "tempFilterVideo";
			string tmpDir = Path.GetTempPath();
			string outputFilePath = Path.Combine(tmpDir, outputFileName);
			return Path.ChangeExtension(outputFilePath, extension);
		}

		void FinishProcessing(NSUrl url)
		{
			PhotoLibraryController.SaveVideo(url);
			InvokeOnMainThread(() =>
			{
				var alert = new UIAlertView("Сохранение", "Видео успешно сохранено", new UIAlertViewDelegate(), "ОК", null);
				alert.Show();
			});
		}


		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
		}

		public override void ViewDidDisappear(bool animated)
		{
			if (File.Exists(videoPath.AbsoluteString)) File.Delete(videoPath.AbsoluteString);
		}


		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.Default;
		}
	}
}