using System;
using System.IO;

namespace ImageProcessor
{
    static class Exstention
    {
        public static void CopyWithoutRewrite(this FileInfo fileInfo, string destFileName)
        {
            string fileIndex = "";
            int i = 1;
            var parentDirectoryName = Path.GetDirectoryName(destFileName);
            var destFileNameWithoutExstention = Path.GetFileNameWithoutExtension(destFileName);
            var ExstentionOfdestFileName = Path.GetExtension(destFileName);


            while (true)
            {
                var checkFile = new FileInfo(Path.Combine(parentDirectoryName, destFileNameWithoutExstention + fileIndex + ExstentionOfdestFileName));
                if (checkFile.Exists)
                {
                    fileIndex = $"({i++})";
                }
                else
                {
                    fileInfo.CopyTo(checkFile.FullName);
                    return;
                }
            }
        }

        public static string ConvertDateTimeToString(this DateTime date)
        {
            return (date.ToShortDateString() + " " + date.ToLongTimeString()).Replace('.', '-').Replace(':', '-');
        }
    }
}
