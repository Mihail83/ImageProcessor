using System;
using System.Drawing.Imaging;
using System.Drawing;
using static System.Threading.Thread;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System.Linq;
using System.Text;
using System.IO;

namespace ImageProcessor
{
    class ImageInfo
    {
        public FileInfo[] ImageFilesInfo = null;

        public bool pathExist;
        public string GetWorkDirectory
        {
            get
            {
                if (ImageFilesInfo == null)
                {
                    return "Не задана";
                }
                else
                {
                    return ImageFilesInfo[0].DirectoryName;
                }
            }
        }

        public void GetImageInfo()
        {
            FileInfo[] ImageFilesInfoTEMP = null;
            DirectoryInfo imageDirInfo;

            Console.WriteLine("ВВедите путь к обрабатываемой папке");
            var path = Console.ReadLine();
            try
            {
                imageDirInfo = new DirectoryInfo(path = "H:\\labaImage");
            }
            catch (Exception)
            {
                Console.WriteLine("неправилный путь");
                Sleep(1500);
                return;
            }

            if (imageDirInfo.Exists)
            {
                var ext = new string[] { ".jpeg", ".jpg", ".png", "tiff" };
                ImageFilesInfoTEMP = (from fi in new DirectoryInfo(path).GetFiles() where ext.Contains(fi.Extension.ToLower()) select fi).ToArray();
            }
            else
            {
                Console.WriteLine("Такой папки не существует");
                Sleep(1500);
                return;
            }

            if (ImageFilesInfoTEMP.Length == 0)
            {
                Console.WriteLine("Фотографии не найдены");
                Sleep(1500);
            }
            else
            {
                ImageFilesInfo = ImageFilesInfoTEMP;
                pathExist = true;
            }
        }

        public static DateTime MetaInfoDate(FileInfo imagefileinfo)   // out  image???
        {
            Image image = new Bitmap(imagefileinfo.FullName);
            PropertyItem imageProperty = null;
            try
            {
                imageProperty = image.GetPropertyItem(0x132);
            }
            catch
            {
            }
            if (imageProperty != null)
            {
                try
                {
                    string dateTaken = Encoding.UTF8.GetString(imageProperty.Value).Trim().Substring(0, 19);
                    var firstHalf = dateTaken.Substring(0, dateTaken.IndexOf(' ')).Replace(':', '.');
                    var secondHalf = dateTaken.Substring(dateTaken.IndexOf(' ') + 1, 8);
                    var Date = DateTime.Parse(firstHalf + " " + secondHalf);
                    return Date;
                }
                catch (Exception)
                {
                    return imagefileinfo.LastWriteTime;
                }

            }
            else
                return imagefileinfo.LastWriteTime;
        }

        public double[] GPSExtractor(FileInfo imagefileinfo)  //вариант получения метаданных с помощью стороннего пакета
        {

            var metainfo = ImageMetadataReader.ReadMetadata(imagefileinfo.FullName);            
            var gpsDirectory = metainfo.OfType<GpsDirectory>().FirstOrDefault();

            try
            {
                var gpsLocation = gpsDirectory.GetGeoLocation();
                double[] arr = { gpsLocation.Latitude, gpsLocation.Longitude };
                Console.WriteLine($"\n\n {imagefileinfo.FullName}   \n{gpsLocation.Latitude}\n {gpsLocation.Longitude}");
                return arr;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
    }
}
