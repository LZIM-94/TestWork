using System;
using System.IO;
using System.Threading.Tasks;
using AVFoundation;
using CoreMedia;
using Foundation;
using UIKit;

namespace TestWork
{
	public class CameraController:UIViewController,IAVCaptureFileOutputRecordingDelegate
	{
		AVCaptureSession captureSession;
		AVCaptureDeviceInput captureDeviceInput;
		AVCaptureStillImageOutput stillImageOutput;
		AVCaptureMovieFileOutput movieFileOutput;
		AVCaptureVideoPreviewLayer videoPreviewLayer;
		CMSampleBuffer mediaBuffer;
		UIView cameraView;

		public AVCaptureDevice Device
		{
			get { return captureDeviceInput.Device;}
		}

		public bool isRecording
		{
			get { return movieFileOutput.Recording; }
		}


		public delegate void VideoDataDelegate(NSUrl videUrl);
		public event VideoDataDelegate FinishRecording;

		public CameraController(UIView cameraView)
		{
			this.cameraView = cameraView;
			SetupLiveCameraStream();
		}

		async public Task<UIImage> TakePhoto()
		{
			
			var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
			if (videoConnection.SupportsVideoOrientation)
			{
				videoConnection.VideoOrientation = (AVCaptureVideoOrientation)UIDevice.CurrentDevice.Orientation;
			}
			mediaBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);

			var jpg = AVCaptureStillImageOutput.JpegStillToNSData(mediaBuffer);

			return UIImage.LoadFromData(jpg);
			}

		public void StartRecordVideo()
		{
			var connection = movieFileOutput.Connections[0];
			if (connection.SupportsVideoOrientation)
			{
				connection.VideoOrientation = (AVCaptureVideoOrientation)UIDevice.CurrentDevice.Orientation;
			}
			movieFileOutput.SetRecordsVideoOrientationAndMirroringChanges(true, connection);
			movieFileOutput.StartRecordingToOutputFile(new NSUrl(GetTmpFilePath("mov"), false), this);

		}

		public void StopRecordVideo()
		{
			movieFileOutput.StopRecording();


		}

		public void SwitchCamera()
		{
			var devicePosition = captureDeviceInput.Device.Position;
			if (devicePosition == AVCaptureDevicePosition.Front)
			{
				devicePosition = AVCaptureDevicePosition.Back;
			}
			else {
				devicePosition = AVCaptureDevicePosition.Front;
			}

			var device = GetCameraForOrientation(devicePosition);
			ConfigureCameraForDevice(device);

			captureSession.BeginConfiguration();
			captureSession.RemoveInput(captureDeviceInput);
			captureDeviceInput = AVCaptureDeviceInput.FromDevice(device);
			captureSession.AddInput(captureDeviceInput);
			captureSession.CommitConfiguration();
		}

		public AVCaptureDevice GetCameraForOrientation(AVCaptureDevicePosition orientation)
		{
			var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);

			foreach (var device in devices)
			{
				if (device.Position == orientation)
				{
					return device;
				}
			}

			return null;
		}

		public async Task AuthorizeCameraUse()
		{
			var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

			if (authorizationStatus != AVAuthorizationStatus.Authorized)
			{
				await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
			}
		}

		public void SetupLiveCameraStream()
		{
			captureSession = new AVCaptureSession();

			var viewLayer = cameraView.Layer;
			videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
			{
				Frame = UIScreen.MainScreen.Bounds
			};
			cameraView.Layer.AddSublayer(videoPreviewLayer);

			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
			ConfigureCameraForDevice(captureDevice);
			captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);
			captureSession.AddInput(captureDeviceInput);

			var dictionary = new NSMutableDictionary();
			dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
			stillImageOutput = new AVCaptureStillImageOutput()
			{
				OutputSettings = new NSDictionary()
			};
			captureSession.AddOutput(stillImageOutput);

			movieFileOutput = new AVCaptureMovieFileOutput();
			captureSession.AddOutput(movieFileOutput);

			captureSession.StartRunning();
		}

		void ConfigureCameraForDevice(AVCaptureDevice device)
		{
			var error = new NSError();
			if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
			{
				device.LockForConfiguration(out error);
				device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
				device.UnlockForConfiguration();
			}
			else if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
			{
				device.LockForConfiguration(out error);
				device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
				device.UnlockForConfiguration();
			}
			else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
			{
				device.LockForConfiguration(out error);
				device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
				device.UnlockForConfiguration();
			}
		}

		public void FinishedRecording(AVCaptureFileOutput captureOutput, NSUrl outputFileUrl, NSObject[] connections, NSError error)
		{
			Action cleanup = () =>
			{
				NSError err;
				NSFileManager.DefaultManager.Remove(outputFileUrl, out err);
			};

			bool success = true;
			if (error != null)
			{
				Console.WriteLine("Movie file finishing error: {0}", error);
				success = ((NSNumber)error.UserInfo[AVErrorKeys.RecordingSuccessfullyFinished]).BoolValue;
			}

			if (!success)
			{
				cleanup();
				return;
			}

			if (FinishRecording != null) FinishRecording(outputFileUrl);
		}

		static string GetTmpFilePath(string extension)
		{
			string outputFileName = "tempVideo";
			string tmpDir = Path.GetTempPath();
			string outputFilePath = Path.Combine(tmpDir, outputFileName);
			return Path.ChangeExtension(outputFilePath, extension);
		}

	}
}

