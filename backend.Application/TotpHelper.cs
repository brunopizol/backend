using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Application
{
    public static class TotpHelper
    {
        public static bool Verify(string base32Secret, string code)
        {
            var secret = Base32Encoding.ToBytes(base32Secret);
            var totp = new Totp(secret, step: 30, mode: OtpHashMode.Sha1);
            return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
        }
    }
}
