namespace NHIABackendService.Core.Utility
{
    public static class CommonHelper
    {
        public static string GenerateTimeStampedFileName(string fileName)
        {
            var ext = fileName.Split(".")[fileName.Split(".").Length - 1];
            var path = $"{fileName.Substring(0, fileName.Length - ext.Length)}_{DateTime.Now.Ticks}.{ext}";
            path = path.Replace(" ", "");
            path = GetSafeFileName(path);
            return path;
        }

        private static string GetSafeFileName(string name, char replace = '_')
        {
            var invalids = Path.GetInvalidFileNameChars();
            return new string(name.Select(c => invalids.Contains(c) ? replace : c).ToArray());
        }
    }
}
