using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ExternalServices.UploadServices
{
    public interface IUploadService
    {
        Task<string> UploadAsync(Stream stream, string fileName) ;
        Task<string> UploadSpecificTypeAsync(Stream stream, string fileName, string contentType);
    }
}
