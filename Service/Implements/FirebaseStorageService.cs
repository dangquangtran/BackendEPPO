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
            _firebaseStorage = new FirebaseStorage(
                _configuration["Firebase:Bucket"],
                new FirebaseStorageOptions
                {
                    ThrowOnCancel = true
                });
        }

        private async Task<string> UploadImageAsync(Stream imageStream, string fileName, string folderName)
        {
            var uploadTask = await _firebaseStorage
                .Child(folderName)
                .Child(DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmssfff") + "_" + fileName)
                .PutAsync(imageStream);

            return uploadTask;
        }

        // Hàm upload ảnh cho Plant
        public async Task<string> UploadPlantImageAsync(Stream imageStream, string fileName)
        {
            return await UploadImageAsync(imageStream, fileName, "imagesPlant");
        }

        // Hàm upload ảnh cho User
        public async Task<string> UploadUserImageAsync(Stream imageStream, string fileName)
        {
            return await UploadImageAsync(imageStream, fileName, "imagesUser");
        }

        // Hàm upload ảnh cho Feedback
        public async Task<string> UploadFeedbackImageAsync(Stream imageStream, string fileName)
        {
            return await UploadImageAsync(imageStream, fileName, "imagesFeedback");
        }
        private async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName)
        {
            var uploadTask = await _firebaseStorage
                .Child(folderName)
                .Child(fileName)
                .PutAsync(fileStream);

            return uploadTask;
        }

        // Hàm upload PDF hợp đồng
        public async Task<string> UploadContractPdfAsync(Stream pdfStream, string fileName)
        {
            return await UploadFileAsync(pdfStream, fileName, "contracts");
        }
        public async Task<Stream> DownloadFileAsync(string fileName, string folderName)
        {
            try
            {
                // Lấy URL tải về từ Firebase Storage
                var downloadUrl = await _firebaseStorage
                    .Child(folderName)
                    .Child(fileName)
                    .GetDownloadUrlAsync();

                // Tạo một HttpClient để tải file từ URL
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetStreamAsync(downloadUrl);
                    return response; // Trả về stream của file
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (nếu có)
                throw new Exception($"Error downloading file: {ex.Message}");
            }
        }
        //public async Task DeletePlantImageAsync(string imageUrl)
        //{
        //    await DeleteImageAsync(imageUrl, "imagesPlant");
        //}

        //// Hàm xóa ảnh cho User
        //public async Task DeleteUserImageAsync(string imageUrl)
        //{
        //    await DeleteImageAsync(imageUrl, "imagesUser");
        //}

        //// Hàm xóa ảnh cho Feedback
        //public async Task DeleteFeedbackImageAsync(string imageUrl)
        //{
        //    await DeleteImageAsync(imageUrl, "imagesFeedback");
        //}

        //// Hàm xóa ảnh chung
        //private async Task DeleteImageAsync(string imageUrl, string folderName)
        //{
        //    var fileName = imageUrl.Split(new string[] { $"{folderName}%2F" }, StringSplitOptions.None)[1].Split('?')[0];

        //    await _firebaseStorage
        //        .Child(folderName)
        //        .Child(fileName)
        //        .DeleteAsync();
        //}
    }
}
