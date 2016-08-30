using System;
using AssetsLibrary;
using CoreGraphics;
using CoreImage;
using Foundation;
using Photos;
using UIKit;

namespace TestWork
{
	static public class PhotoLibraryController
	{
		public static void SaveVideo(NSUrl url)
		{
			ALAssetsLibrary library = new ALAssetsLibrary();
			library.WriteVideoToSavedPhotosAlbum(url,null);
		}

		public static void SavePhoto(CGImage image)
		{
			ALAssetsLibrary library = new ALAssetsLibrary();
			library.WriteImageToSavedPhotosAlbum(image, new NSDictionary(), null);
		}

		public static UIImage GetThumbnailLastPhoto()
		{
			var imgMngr = new PHImageManager();
			PHFetchResult fetchResults = PHAsset.FetchAssets(PHAssetMediaType.Image, null);
			var index = fetchResults.Count - 1;
			var image = fetchResults[index] as PHAsset;

			UIImage thumbnail = new UIImage();
			imgMngr.RequestImageForAsset(image, new CoreGraphics.CGSize(image.PixelWidth, image.PixelHeight),
					PHImageContentMode.AspectFill, new PHImageRequestOptions(), (resultImage, info) =>
					{
						thumbnail = resultImage;

					});
			return thumbnail;
		}
	}

}

