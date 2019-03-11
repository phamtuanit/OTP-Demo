using OtpSharp;
using System;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace OTPServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [PropertyChanged.ImplementPropertyChanged]
    public partial class MainWindow : Window
    {
        private Totp totp;

        private DispatcherTimer timer;

        public OTPRequest OTPRequest { get; set; } = new OTPRequest();

        public OTP OTP { get; set; } = new OTP();

        public string UserOtp { get; set; }

        public Image QRCode { get; set; }

        public bool IsValid { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Input, this.TickCallback, this.Dispatcher);
        }

        private void GenerateOTP()
        {
            // Reseting data
            this.timer.Stop();
            this.OTP.Otp = "";
            this.OTP.RemainingSeconds = 0;
            this.IsValid = false;

            // Validating input
            if (string.IsNullOrWhiteSpace(this.OTPRequest.SecretKey))
            {
                this.totp = null;
                return;
            }

            if (this.OTPRequest.Step <= 0)
            {
                this.OTPRequest.Size = 30;
            }

            if (this.OTPRequest.Step <= 4)
            {
                this.OTPRequest.Size = 6;
            }

            byte[] rfcKey = UTF8Encoding.ASCII.GetBytes(this.OTPRequest.SecretKey);

            // Generating TOTP
            this.totp = new Totp(rfcKey, this.OTPRequest.Step,
                                    OtpHashMode.Sha1, this.OTPRequest.Size);

            // Generate shared key (QR)
            string url = KeyUrl.GetTotpUrl(rfcKey, this.OTPRequest.SiteId, this.OTPRequest.Step, OtpHashMode.Sha1, this.OTPRequest.Size);
            this.LoadQr(url);

            // Updating TOTP data
            this.UpdateOTPInfo();
            this.timer.Start();
        }

        private void LoadQr(string url)
        {
            // downloading image
            string qrUrl = string.Format("http://qrcode.kaywa.com/img.php?s=4&d={0}", HttpUtility.UrlEncode(url));
            Uri urlUri = new Uri(qrUrl);
            var request = WebRequest.CreateDefault(urlUri);

            // Rendering image
            Image image = new System.Windows.Controls.Image();
            image.BeginInit();
            image.Source = BitmapFrame.Create(request.GetResponse().GetResponseStream(), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            image.EndInit();

            this.QRCode = image;
        }

        private void UpdateOTPInfo()
        {
            this.OTP.Otp = this.totp.ComputeTotp();
            this.OTP.RemainingSeconds = this.totp.RemainingSeconds();
        }

        private void Verify(string otp)
        {
            if (string.IsNullOrWhiteSpace(otp))
            {
                return;
            }

            long windowUsed;
            this.IsValid = totp.VerifyTotp(otp, out windowUsed, new VerificationWindow(0, 0));
        }

        private void TickCallback(object sender, EventArgs e)
        {
            if (this.totp == null)
            {
                return;
            }

            this.UpdateOTPInfo();
        }

        private void GenerateOtp_Click(object sender, RoutedEventArgs e)
        {
            this.GenerateOTP();
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            this.Verify(this.UserOtp);
        }
    }
}
