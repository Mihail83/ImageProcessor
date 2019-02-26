using System.IO;

namespace ImageProcessor
{
    class SortByYear
    {
        FileInfo[] _fileinfo;
        public SortByYear(FileInfo[] fileinfo)
        {
            _fileinfo = fileinfo;
        }

        public void SortImageByYear()
        {
            Directory.CreateDirectory(_fileinfo[0].DirectoryName + "SortImageByYear");
            Directory.SetCurrentDirectory(_fileinfo[0].DirectoryName + "SortImageByYear");

            for (int i = 0; i < _fileinfo.Length; i++)
            {
                var year = ImageInfo.MetaInfoDate(_fileinfo[i]).Year.ToString();
                Directory.CreateDirectory(year);
                _fileinfo[i].CopyTo(Path.Combine(year, _fileinfo[i].Name), true);
            }
        }
    }
}
