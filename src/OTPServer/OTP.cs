using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTPServer
{
    [PropertyChanged.ImplementPropertyChanged]
    public class OTP
    {
        public string Otp { get; set; }

        public int RemainingSeconds { get; set; } = 0; // Seconds
    }
}
