using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TinyPng;

namespace TinyPNG_AWS
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("start processing");
            await ProcessImageatAWS();
        }

        static async Task<string> ProcessImageatAWS()
        {
            var png = new TinyPngClient("yourSecretApiKey");

            var imageBytes = await png.CompressFromUrl("https://singleton-dev-media.s3-us-west-2.amazonaws.com/04380167-70ca-4d78-99dc-993128f07005.png")
                     .Resize(1366, 768, TinyPng.ResizeOperations.ResizeType.Fit).GetImageByteData();  // Options are Fit, Scale and Cover 
            return await UploadImageToS3FromBytes(imageBytes);
        }

         static async Task<string> UploadImageToS3FromBytes(byte[] imageBytes)
        {
            var stream = new MemoryStream(imageBytes, 0, imageBytes.Length);
            using var client = new AmazonS3Client("AccesskeyId","AccessKeySecret");
            var request = new PutObjectRequest
            {
                InputStream = stream,
                BucketName = "nameofthebucket",
                Key = $"{Guid.NewGuid()}.jpeg",
                ContentType = "image/jpeg",
                CannedACL = S3CannedACL.Private,
                TagSet = new List<Tag>()
            };

            request.Headers.CacheControl = "";
            var response = await client.PutObjectAsync(request);
            return response.HttpStatusCode.ToString() == "OK" ? request.Key : string.Empty;
        }
    }
}
