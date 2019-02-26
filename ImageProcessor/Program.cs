using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            ControlImageProcessor();
        }

        public static void ControlImageProcessor()
        {
            var imageInfo1 = new ImageInfo();
            while (true)
            {
                Console.Clear();
                //status bar
                Console.WriteLine("Рабочая папка: " + imageInfo1.GetWorkDirectory);
                Console.WriteLine("выбор папки -  S, выход - E ");
                if (imageInfo1.pathExist)
                {
                    Console.WriteLine(" переименовать по дате съемки - 1\n добавить пометку с датой на фото - 2\n " +
                       "сортировка по году съемки  - 3\n");
                }

                switch (Console.ReadKey().KeyChar)
                {
                    case 's':
                    case 'S':
                        imageInfo1.GetImageInfo();
                        break;

                    case 'e':
                    case 'E':
                        return;

                    case '1' when imageInfo1.pathExist:
                        var renameImage1 = new RenameAndReplaceImage(imageInfo1.ImageFilesInfo);
                        renameImage1.ChangeNameByDate();
                        break;

                    case '2' when imageInfo1.pathExist:
                        var addDateToImage = new ImageWithData(imageInfo1.ImageFilesInfo);
                        addDateToImage.AddDateToImage();
                        break;

                    case '3' when imageInfo1.pathExist:
                        var sortbyyear = new SortByYear(imageInfo1.ImageFilesInfo);
                        sortbyyear.SortImageByYear();
                        break;

                    case '4' when imageInfo1.pathExist:   //
                        for (int i = 0; i < imageInfo1.ImageFilesInfo.Length; i++)
                        {
                            double[] arr = imageInfo1?.GPSExtractor(imageInfo1.ImageFilesInfo[i]);
                        }
                        Console.ReadKey();
                        break;

                    default:
                        Console.WriteLine("Не то");
                        System.Threading.Thread.Sleep(500);
                        break;
                }
            }
        }
    }
}
