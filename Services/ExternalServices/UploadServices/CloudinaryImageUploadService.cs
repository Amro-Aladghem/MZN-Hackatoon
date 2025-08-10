using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Services.ExternalServices.UploadServices
{
    public class CloudinaryImageUploadService : IUploadService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageUploadService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName)
        {
            const long ImageMaxSize = 3 * 1024 * 1024;
            var allowedextention = new[] { ".jpg", ".jpeg", ".png" };

            string ex = Path.GetExtension(fileName).ToLower();

            if (fileStream.Length > ImageMaxSize)
            {
                throw new Exception("The Picture you has been sent is over 3mb sizing");
            }

            if (!allowedextention.Contains(ex))
            {
                throw new Exception("The file is not supported , only .png , .jpg");
            }

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream)
            };


            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }

            throw new Exception("Error uploading image to Server.");
        }

        public async Task<string> UploadSpecificTypeAsync(Stream fileStream, string fileName,string contentType)
        {
            throw new Exception("Error uploading image to Server.");
        }
    }
}
