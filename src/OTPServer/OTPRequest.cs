using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTPServer
{
    [PropertyChanged.ImplementPropertyChanged]
    public class OTPRequest
    {
        public string SecretKey { get; set; } = "MyKey@12345";

        public string SiteId { get; set; } = "otp@phamtuantech.com";

        public int Step { get; set; } = 30; // Seconds

        public int Size { get; set; } = 6;
    }
}
