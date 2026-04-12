using QRCoder;

namespace MuseumWebApp.Services
{
    public class QRCodeService
    {
        // Возвращает PNG байты QR-кода для переданного текста
        public byte[] GenerateQRCode(string text)
        {
            using var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var code = new PngByteQRCode(data);
            return code.GetGraphic(6);
        }

        // Возвращает base64 data URI для вставки в <img src="...">
        public string GenerateQRCodeBase64(string text)
        {
            var bytes = GenerateQRCode(text);
            return "data:image/png;base64," + Convert.ToBase64String(bytes);
        }
    }
}
