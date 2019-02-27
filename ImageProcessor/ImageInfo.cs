using System;
using System.Drawing.Imaging;
using System.Drawing;
using static System.Threading.Thread;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;

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

        public GeoLocation GPSExtractor(FileInfo imagefileinfo)  //вариант получения метаданных с помощью стороннего пакета
        {
            var metainfo = ImageMetadataReader.ReadMetadata(imagefileinfo.FullName);
            var gpsDirectory = metainfo.OfType<GpsDirectory>().FirstOrDefault();

            try
            {
                var gpsLocation = gpsDirectory.GetGeoLocation();
                //Console.WriteLine($"\n\n {imagefileinfo.FullName}   \n{gpsLocation.Latitude}\n {gpsLocation.Longitude}");
                return gpsLocation;
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }

        public void SortImageByLokality()
        {
            for (int i = 0; i < ImageFilesInfo.Length; i++)
            {
                var gps = this?.GPSExtractor(ImageFilesInfo[i]);

                System.IO.Directory.CreateDirectory(ImageFilesInfo[0].DirectoryName + "SortImageByLokality");
                System.IO.Directory.SetCurrentDirectory(ImageFilesInfo[0].DirectoryName + "SortImageByLokality");

                if (gps != null)
                {
                    HttpWebRequest reg = (HttpWebRequest)WebRequest
                        .Create(@"https://geocode-maps.yandex.ru/1.x/?geocode=" + gps.Longitude + "," + gps.Latitude + "&kind=locality&results=1");
                    HttpWebResponse response = (HttpWebResponse)reg.GetResponse();

                    using (Stream responsestream = response.GetResponseStream())
                    {
                        XmlDocument xmlRespond = new XmlDocument();
                        xmlRespond.Load(responsestream);

                        // XmlNamespaceManager namespaces = new XmlNamespaceManager(xmlRespond.NameTable);

                        //namespaces.AddNamespace("ns1", "urn: oasis:names: tc:ciq: xsdschema:xAL: 2.0");
                        //namespaces.AddNamespace("ns2", "http://maps.yandex.ru/geocoder/1.x");
                        //namespaces.AddNamespace("ns3", "http://www.opengis.net/gml");
                        //namespaces.AddNamespace("ns4", "http://maps.yandex.ru/ymaps/1.x");

                        //XmlNode localityName;
                        //XmlNode root = xmlRespond.DocumentElement;
                        //localityName = root.SelectSingleNode("//ns4:LocalityName", namespaces);        //никак не догоняю Xpath...

                        var nam = xmlRespond.InnerXml;
                        int first = nam.IndexOf(@"<localityname>", StringComparison.CurrentCultureIgnoreCase) + 14;
                        int last = nam.LastIndexOf(@"</localityname>", StringComparison.CurrentCultureIgnoreCase);
                        nam = nam.Substring(first, last - first);

                        System.IO.Directory.CreateDirectory(nam);
                        ImageFilesInfo[i].CopyTo(Path.Combine(nam, ImageFilesInfo[i].Name), true);
                    }
                }
                else
                    continue;
            }
        }
    }
}
