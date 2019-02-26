using System.IO;
using System.Drawing;

namespace ImageProcessor
{
    class ImageWithData
    {
        FileInfo[] _imageInfo = null;
        public ImageWithData(FileInfo[] imageinfo)
        {
            _imageInfo = imageinfo;
        }

        Brush brush = new SolidBrush(Color.DeepPink);
        Font font = new Font("arial", 12, GraphicsUnit.Pixel);

        public void AddDateToImage()
        {
            string newFolder = (Directory.CreateDirectory(_imageInfo[0].DirectoryName + "AddDateToImage")).FullName;
            for (int i = 0; i < _imageInfo.Length; i++)
            {
                using (Bitmap bitmapToAddDate = new Bitmap(_imageInfo[i].FullName))
                {
                    Graphics graphicsToAddDate = Graphics.FromImage(bitmapToAddDate);
                    var dateToDraw = ImageInfo.MetaInfoDate(_imageInfo[i]).ConvertDateTimeToString();

                    var sizeOfDate = graphicsToAddDate.MeasureString(dateToDraw, font);
                    graphicsToAddDate.DrawString(dateToDraw, font, brush, graphicsToAddDate.VisibleClipBounds.Width - sizeOfDate.Width, 0);
                    bitmapToAddDate.Save(Path.Combine(newFolder, _imageInfo[i].Name));
                }
            }
        }
    }
}
