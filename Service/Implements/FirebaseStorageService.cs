using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Service.Implements
{
    public class FirebaseStorageService
    {
        private readonly FirebaseStorage _firebaseStorage;
        private readonly IConfiguration _configuration;

        public FirebaseStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            // Khởi tạo FirebaseStorage với bucket của bạn
            _firebaseStorage = new FirebaseStorage(
                _configuration["Firebase:Bucket"],
                new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                });
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            var uploadTask = await _firebaseStorage
            .Child("imagesPlant")
                .Child(DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + fileName)
                .PutAsync(imageStream);

            return uploadTask;
        }
    }
}
