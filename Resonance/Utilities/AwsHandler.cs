using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using DocumentFormat.OpenXml.Vml;
using ResoClassAPI.Controllers;
using ResoClassAPI.DTOs;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities.Interfaces;

namespace ResoClassAPI.Utilities
{
    public class AwsHandler : IAwsHandler
    {
        private readonly ILogger<AwsHandler> logger;
        private readonly IAmazonS3 _s3Client;

        public AwsHandler(ILogger<AwsHandler> _logger, IAmazonS3 s3Client)
        {
            logger = _logger;
            _s3Client = s3Client;
        }
        public async Task<string> UploadImage(byte[] imageData, string fileName, string bucketName, string folderPath)
        {
            string imageUrl = string.Empty;
            try
            {
                using (var memoryStream = new MemoryStream(imageData))
                {
                    var fileTransferUtility = new TransferUtility(_s3Client);

                    var keyName = folderPath + "/" + fileName;

                    await fileTransferUtility.UploadAsync(memoryStream, bucketName, keyName);

                    imageUrl = GenerateS3ObjectUrl(bucketName, folderPath, fileName);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return imageUrl;
        }

        private string GenerateS3ObjectUrl(string bucketName, string folderName, string key)
        {
            var region = RegionEndpoint.APSouth1.SystemName;
            return $"https://{bucketName}.s3.{region}.amazonaws.com/{folderName}/{key}";
        }
    }
}
