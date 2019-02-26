using System.IO;

namespace ImageProcessor
{
    class RenameAndReplaceImage
    {
        FileInfo[] _imageInfo;
        public RenameAndReplaceImage(FileInfo[] imageinfo)
        {
            _imageInfo = imageinfo;
        }

        public void ChangeNameByDate()
        {
            var parentDirectoreName = _imageInfo[0].Directory.FullName;

            Directory.CreateDirectory(parentDirectoreName + "ChangeNameByDate");

            for (int i = 0; i < _imageInfo.Length; i++)
            {
                string newImageName = ImageInfo.MetaInfoDate(_imageInfo[i]).ConvertDateTimeToString();
                _imageInfo[i].CopyWithoutRewrite(parentDirectoreName + "ChangeNameByDate\\ " + newImageName + _imageInfo[i].Extension);
            }
        }
    }
}
