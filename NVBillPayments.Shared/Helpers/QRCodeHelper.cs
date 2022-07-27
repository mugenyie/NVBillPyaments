using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace NVBillPayments.Shared.Helpers
{
    public static class QRCodeHelper
    {
        public static string Generate(string inputData)
        {
            StringBuilder qrCodeString = new StringBuilder();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
                QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(inputData, QRCodeGenerator.ECCLevel.Q);
                QRCode qRCode = new QRCode(qRCodeData);
                using (Bitmap bitmap = qRCode.GetGraphic(20))
                {
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    qrCodeString.Append(Convert.ToBase64String(memoryStream.ToArray())); //"data:image/png;base64," + 
                }
            }
            return qrCodeString.ToString();
        }
    }
}
