using ChatroomAPI.Model.Frontend;
using ChatroomAPI.Repositories.Interface;
using ChatroomAPI.Services.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Services
{
    public class FileServices : IFileServices
    {
        private IWebHostEnvironment _hostEnvironment;
        private IChatRepository _chatRepository { get; set; }
        private string contentRootPath { get; set; }

        public FileServices(IWebHostEnvironment environment, IChatRepository chatRepository)
        {
            _hostEnvironment = environment;
            _chatRepository = chatRepository;
            contentRootPath = _hostEnvironment.ContentRootPath;
        }

        public async Task SaveFile(IFormFile file, Message message)
        {
            var rootPath = Path.GetFullPath(Path.Combine(contentRootPath, @"..\..\")) + @"FileStorage\Upload";
            var target = "";
            var filePath = "";

            if (message.RoomName == null)
            {
                target = rootPath + @"\Private\" + message.SenderUID + ";" + message.ReceiverUID + @"\";
                filePath = Path.Combine(target, message.UID);
            }
            else
            {
                target = rootPath + @"\Room\" + _chatRepository.GetRoomId(message.RoomName) + @"\";
                filePath = Path.Combine(target, message.UID);
            }

            Directory.CreateDirectory(target);

            if (file.Length <= 0) return;
            //var filePath = Path.Combine(target, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }

        public async Task<(MemoryStream ms, string fileName)> DownloadFile(string attachment_id)
        {
            var rootPath = Path.GetFullPath(Path.Combine(contentRootPath, @"..\..\")) + @"FileStorage\Upload";
            var message = await _chatRepository.GetMessage(attachment_id);

            string filePath = "";
            string senderUID = message.SenderUID;
            string receiverUID = message.ReceiverUID;
            string fileName = message.MessageBody;

            if (message.RoomId.HasValue == false)
            {
                filePath = rootPath +  @"\Private\" + $@"{senderUID};{receiverUID}\" + attachment_id;
            }
            else
            {
                filePath = rootPath + @"\Room\" + $@"{message.RoomId.Value}\" + attachment_id;
            }

            if (!File.Exists(filePath))
                throw new Exception("File not exist.");

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return (memory, fileName);
        }


        public static string SizeConverter(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            switch (fileSize)
            {
                case var _ when fileSize < kilobyte:
                    return $"Less then 1KB";
                case var _ when fileSize < megabyte:
                    return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                case var _ when fileSize < gigabyte:
                    return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                case var _ when fileSize >= gigabyte:
                    return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                default:
                    return "n/a";
            }
        }

        //public (string fileType, byte[] archiveData, string archiveName) FetechFiles(string subDirectory)
        //{
        //    var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

        //    var files = Directory.GetFiles(Path.Combine(@"C:\Users\ACER\Desktop\Jaeden\Personal\Tools\ChatApplication\FileStorage\TestUpload", subDirectory)).ToList();

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //        {
        //            files.ForEach(file =>
        //            {
        //                var theFile = archive.CreateEntry(file);
        //                using (var streamWriter = new StreamWriter(theFile.Open()))
        //                {
        //                    streamWriter.Write(File.ReadAllText(file));
        //                }

        //            });
        //        }

        //        return ("application/zip", memoryStream.ToArray(), zipName);
        //    }

        //}
    }
}
