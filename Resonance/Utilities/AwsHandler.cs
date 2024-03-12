using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using DocumentFormat.OpenXml.Vml;
using Microsoft.Extensions.Configuration;
using ResoClassAPI.Controllers;
using ResoClassAPI.DTOs;
using ResoClassAPI.Middleware;
using ResoClassAPI.Services.Interfaces;
using ResoClassAPI.Utilities.Interfaces;

namespace ResoClassAPI.Utilities
{
    public class AwsHandler : IAwsHandler
    {
        private readonly ILogger<AwsHandler> logger;
        private readonly IAmazonS3 _s3Client;
        private readonly ICommonService _commonService;
        private IConfiguration _config;

        public AwsHandler(ILogger<AwsHandler> _logger, IAmazonS3 s3Client, IConfiguration configuration, ICommonService commonService)
        {
            logger = _logger;
            _s3Client = s3Client;
            _commonService = commonService;
            _config = configuration;
        }
        public async Task<string> UploadImage(byte[] imageData, string fileName)
        {
            string imageUrl = string.Empty;
            try
            {
                string bucketName = _config["S3:BucketName"];
                string folderPath = _config["S3:FolderName"];
                using (var memoryStream = new MemoryStream(imageData))
                {
                    var credentials = new Amazon.Runtime.BasicAWSCredentials("AKIAQ3EGRZ7DC24FKX3X", "2M5fIKfy7EMWLegExiQgH1EYz2gc+4L0G5kf3PkV");
                    var region = RegionEndpoint.APSoutheast2;
                    // Create S3 client with root credentials
                    var s3Client = new AmazonS3Client(credentials, region);

                    var fileTransferUtility = new TransferUtility(_s3Client);

                    var keyName = folderPath + "/" + fileName;
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = keyName,
                        InputStream = memoryStream
                    };

                    var response = await s3Client.PutObjectAsync(putRequest);
                    _commonService.LogError(typeof(GlobalExceptionHandler), bucketName, keyName, typeof(AwsHandler).Name);
                    var regionName = region.SystemName;
                    imageUrl = GenerateS3ObjectUrl(regionName, bucketName, folderPath, fileName);

                }
            }
            catch (Exception ex)
            {
                _commonService.LogError(typeof(GlobalExceptionHandler), ex.Message, ex.StackTrace, ex.GetType().Name);
                throw ex;
            }
            return imageUrl;
        }

        private string GenerateS3ObjectUrl(string regionName, string bucketName, string folderName, string key)
        {
            return $"https://{bucketName}.s3.{regionName}.amazonaws.com/{folderName}/{key}";
        }
    }
}
