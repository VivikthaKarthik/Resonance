﻿namespace ResoClassAPI.Utilities.Interfaces
{
    public interface IAwsHandler
    {
        Task<string> UploadImage(byte[] imageData, string folderName, string filename);
    }
}
