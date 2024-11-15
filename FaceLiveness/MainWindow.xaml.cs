using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Threading;
using System.Drawing;

using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;
using System.Windows.Controls;
using RestSharp;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FaceLiveness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        #region Public properties

        public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; this.OnPropertyChanged("CurrentDevice"); }
        }
        private FilterInfo _currentDevice;

        #endregion

        #region Private fields

        private IVideoSource _videoSource;
        private readonly OpenCvSharp.CascadeClassifier _cascadeClassifier;

        private bool isFirstFrame = false;
        private int ovalWidth;
        private int ovalHeight;
        private string message = "";
        private bool isFaceOkay = false;
        private bool hasImageUploaded = false;
        private int numberOfGoodFramesCaptured = 0;
        private const int numberOfGoodFramesRequired = 30;

        #endregion


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            GetVideoDevices();
            this.Closing += MainWindow_Closing;

            ovalWidth = 0;
            ovalHeight = 0;

            Uri model = new Uri("pack://application:,,,/haarcascade_frontalface_alt.xml");

            _cascadeClassifier = new OpenCvSharp.CascadeClassifier(
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_alt.xml")
                //model.AbsolutePath
                );
        }

        private void Reset()
        {
            isFirstFrame = false;
            ovalWidth = 0;
            ovalHeight = 0;
            message = "";
            isFaceOkay = false;
            hasImageUploaded = false;
            numberOfGoodFramesCaptured = 0;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopCamera();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            StopCamera();
            StartCamera();
        }


        private BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        public Mat ApplyOvalOverlay(Mat frame, int ovalWidth, int ovalHeight)
        {
            // Step 1: Create a mask with the same size as the frame and a darkened color
            Mat mask = new Mat(frame.Size(), MatType.CV_8UC3, new Scalar(0, 0, 0));

            // Step 2: Define the oval's position and size
            Point center = new Point(frame.Width / 2, frame.Height / 2);

            // Step 3: Draw a filled white oval in the mask
            Cv2.Ellipse(mask, center, new Size(ovalWidth / 2, ovalHeight / 2), 0, 0, 360, new Scalar(255, 255, 255), -1);

            // Step 4: Invert the mask and overlay it onto the original frame
            Mat inverseMask = new Mat();
            Cv2.BitwiseNot(mask, inverseMask); // Invert the mask to darken outside the oval
            Cv2.AddWeighted(frame, 1, inverseMask, 0.5, 1, frame); // Adjust the 0.5 value to control darkness
            return frame;
        }

        public Mat AddTextWithRoundedRectangle(Mat frame, string text)
        {
            HersheyFonts fontFace = HersheyFonts.HersheySimplex;
            double fontScale = 0.7;
            int thickness = 1;
            Scalar textColor = new Scalar(255, 255, 255); // White color for the text
            Scalar rectColor = new Scalar(0, 0, 0); // Black color for the rectangle
            int padding = 10; // Padding around the text within the rectangle

            // Get the text size
            Size textSize = Cv2.GetTextSize(text, fontFace, fontScale, thickness, out int baseline);
            baseline += thickness;

            // Calculate the position for the text
            Point textPosition = new Point(
                (frame.Width - textSize.Width) / 2,
                frame.Height - baseline - 4 // Adjusting to place text slightly above bottom edge
            );

            // Calculate the rectangle around the text with padding
            OpenCvSharp.Rect rect = new OpenCvSharp.Rect(
                textPosition.X - padding,
                textPosition.Y - textSize.Height - padding,
                textSize.Width + 2 * padding,
                textSize.Height + 2 * padding
            );
            Cv2.Rectangle(frame, rect, rectColor, -1); // Draw filled rectangle
            Cv2.Rectangle(frame, rect, rectColor, thickness); // Border for the rounded rectangle

            // Draw the text inside the rounded rectangle
            Cv2.PutText(frame, text, textPosition, fontFace, fontScale, textColor, thickness);

            return frame;
        }

        public class Response
        {
            public int Status { get; set; }
            public Result Result { get; set; }
        }

        public class Result
        {
            public string Status { get; set; }
            public double Confidence { get; set; }
        }

        public async Task<Response> SendImageToServerWithRestSharpAsync(Mat image, string url)
        {
            // Step 1: Convert Mat to byte array in JPEG format
            Cv2.ImEncode(".jpg", image, out byte[] imageData);

            // Step 2: Create RestSharp client and request
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);

            // Step 3: Add image as file to the request
            request.AddFile("image", imageData, "image.jpg", "image/jpeg");

            // Step 4: Execute the request
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                // Handle success
                System.Console.WriteLine("Response received: " + response.Content);
                return JsonConvert.DeserializeObject<Response>(response.Content);
            }
            else
            {
                // Handle error
                System.Console.WriteLine("Error: " + response.ErrorMessage);
                return null;
            }
        }

        public Mat CropToCenterAspectRatio(Mat image, double aspectRatioWidth = 3, double aspectRatioHeight = 4)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // Calculate the desired height and width based on the aspect ratio
            int newWidth, newHeight;

            // Check if we should limit by width or height
            if (originalWidth / (double)originalHeight > aspectRatioWidth / aspectRatioHeight)
            {
                // Width is limiting factor (crop by width)
                newHeight = originalHeight;
                newWidth = (int)(newHeight * (aspectRatioWidth / aspectRatioHeight));
            }
            else
            {
                // Height is limiting factor (crop by height)
                newWidth = originalWidth;
                newHeight = (int)(newWidth * (aspectRatioHeight / aspectRatioWidth));
            }

            // Calculate the top-left point for the crop so it's centered
            int x = (originalWidth - newWidth) / 2;
            int y = (originalHeight - newHeight) / 2;

            // Crop the image using a centered rectangle
            OpenCvSharp.Rect cropRect = new OpenCvSharp.Rect(x, y, newWidth, newHeight);
            Mat croppedImage = new Mat(image, cropRect);

            return croppedImage;
        }

        private void Video_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            if (hasImageUploaded)
            {
                return;
            }

            try
            {
                BitmapImage bi;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    // convert bitmap to OpenCV Mat
                    var mat = bitmap.ToMat();

                    Cv2.Resize(mat, mat, new Size(1280, 720));

                    var matCopy = mat.Clone();

                    if (!isFirstFrame)
                    {
                        isFirstFrame = true;
                        ovalWidth = (int)(mat.Width / 2.8);
                        ovalHeight = (int)(mat.Height / 1.2);
                    }

                    // flip frame horizontally
                    Cv2.Flip(mat, mat, FlipMode.Y);
                    Cv2.Flip(matCopy, matCopy, FlipMode.Y);

                    var grayScale = new OpenCvSharp.Mat();

                    // convert to grayscale
                    Cv2.CvtColor(mat, grayScale, OpenCvSharp.ColorConversionCodes.BGR2GRAY);

                    // detect faces
                    var faces = _cascadeClassifier.DetectMultiScale(
                        image: grayScale,
                        scaleFactor: 1.3,
                        minNeighbors: 3,
                        flags: HaarDetectionTypes.DoRoughSearch | HaarDetectionTypes.ScaleImage,
                        minSize: new OpenCvSharp.Size(30, 30)
                        );

                    if (faces.Length > 1)
                    {
                        message = "Please show only one face";
                    }
                    else if (faces.Length == 0)
                    {
                        message = "No face detected";
                    }
                    // if face frame is not in the oval
                    else if (faces[0].X < mat.Width / 2 - ovalWidth / 2 || faces[0].X + faces[0].Width > mat.Width / 2 + ovalWidth / 2 ||
                        faces[0].Y < mat.Height / 2 - ovalHeight / 2 || faces[0].Y + faces[0].Height > mat.Height / 2 + ovalHeight / 2)
                    {
                        message = "Please move your face to the oval";
                    }
                    // if face is too small
                    else if (faces[0].Width < ovalWidth / 1.2 || faces[0].Height < ovalHeight / 2)
                    {
                        message = "Please move closer";
                    }
                    // if face is too big
                    else if (faces[0].Width > ovalWidth || faces[0].Height > ovalHeight)
                    {
                        message = "Please move away";
                    }
                    else
                    {
                        isFaceOkay = true;
                        message = "Hold Still";
                    }

                    // Frames are captured only when the face is okay
                    if (isFaceOkay)
                    {
                        numberOfGoodFramesCaptured++;
                    } else
                    {
                        numberOfGoodFramesCaptured = 0;
                    }


                    if (numberOfGoodFramesCaptured == numberOfGoodFramesRequired)
                    {
                        // write a post request to the server
                        // if the response is okay, then show the success message
                        // else show the error message

                        Task.Run(async () =>
                        {
                            Mat image = CropToCenterAspectRatio(matCopy);

                            // Resize to 720x960
                            Cv2.Resize(image, image, new Size(720, 960));

                            //Cv2.ImWrite("test.jpg", image);


                            Console.WriteLine("Uploading image");
                            Response response = await SendImageToServerWithRestSharpAsync(image, "https://face-api.truid.ai/check-face-liveness");

                            Cv2.ImShow("Hello", image);
                            Cv2.WaitKey(1000);
                            Cv2.DestroyAllWindows();

                            Console.WriteLine("Response received: " + response.Status + " " + response.Result.Status);

                            //Reset();

                            StopCamera();

                            Dispatcher.Invoke(() =>
                            {
                                messageLabel.Content = "Press start to check again";
                            });

                            // show a dialog box with the response
                            Dispatcher.Invoke(() =>
                            {
                                if (response.Result.Status != "error")
                                {
                                    MessageBox.Show($"Face liveness check successful. {response.Result.Confidence}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show($"Face liveness check failed. {response.Result.Confidence}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            });

                        });
                        hasImageUploaded = true;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        messageLabel.Content = message;
                    });

                    // draw rectangles around detected faces
                    foreach (var face in faces)
                    {
                        Cv2.Rectangle(mat, face, new OpenCvSharp.Scalar(0, 255, 0), 2);
                    }

                    mat = ApplyOvalOverlay(mat, ovalWidth, ovalHeight);

                    bi = ToBitmapImage(mat.ToBitmap());
                }
                bi.Freeze(); // avoid cross thread operations and prevents leaks
                Dispatcher.BeginInvoke(new ThreadStart(delegate { videoPlayer.Source = bi; }));
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            StopCamera();
        }

        private void GetVideoDevices()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo filterInfo in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                VideoDevices.Add(filterInfo);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("No video sources found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartCamera()
        {
            if (CurrentDevice != null)
            {
                _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource.NewFrame += Video_NewFrame;
                _videoSource.Start();
            }
        }

        private void StopCamera()
        {
            Reset();

            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= new NewFrameEventHandler(Video_NewFrame);
            }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
