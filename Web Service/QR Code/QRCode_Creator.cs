using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;
namespace Web_Service
{
    public class QRCode_Creator
    {
        
        public static byte[] Create(string data)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                return qrCode.GetGraphic(20);
            }
        }
    }
}
