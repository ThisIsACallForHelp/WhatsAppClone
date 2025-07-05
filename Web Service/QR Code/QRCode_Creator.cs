using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;
using System.Drawing.Drawing2D;
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

                return qrCode.GetGraphic(
                                         pixelsPerModule: 20,
                                         darkColor: Color.Black,
                                         lightColor: Color.White, 
                                         drawQuietZones: false          
                                        );
            }
        }

        public static Bitmap GradientQR(Bitmap qrBitmap) //Bitmap: Image, but represented as bits 
        {
            Bitmap QR = new Bitmap(qrBitmap.Width, qrBitmap.Height); //Create a new bitmap
            using (Graphics g = Graphics.FromImage(QR)) //Get the graphics form the original pic 
            {
                
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Rectangle(0, 0, qrBitmap.Width, qrBitmap.Height),
                    ColorTranslator.FromHtml("#005f99"), ColorTranslator.FromHtml("#0a2342"), 
                    LinearGradientMode.BackwardDiagonal)) //create a new Rectangle with the same width and height
                    //and a gradient color 
                {
                    g.FillRectangle(brush, 0, 0, qrBitmap.Width, qrBitmap.Height); //fill that rectangle 
                }             
            }
            Bitmap finalQR = new Bitmap(qrBitmap.Width, qrBitmap.Height); //create another bitmap that we will return

            for (int x = 0; x < qrBitmap.Width; x++) //loop for each row 
            {
                for (int y = 0; y < qrBitmap.Height; y++) //loop for each column
                {
                    var pixel = qrBitmap.GetPixel(x, y); //get the pixel's colours 

                    // If it's a black QR pixel, replace it and use gradient pixel
                    if (pixel.R < 50 && pixel.G < 50 && pixel.B < 50)
                    {
                        finalQR.SetPixel(x, y, QR.GetPixel(x, y));
                    }
                    else
                    {
                        finalQR.SetPixel(x, y, Color.White); // but dont touch it if its white
                    }
                }
            }
            return finalQR; //return the QR code 
        }

    }
}
