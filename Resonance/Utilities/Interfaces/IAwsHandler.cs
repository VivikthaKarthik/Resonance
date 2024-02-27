namespace ResoClassAPI.Utilities.Interfaces
{
    public interface IAwsHandler
    {
        Task<string> UploadImage(byte[] imageData, string filename, string bucketName, string folderPath);
    }
}
