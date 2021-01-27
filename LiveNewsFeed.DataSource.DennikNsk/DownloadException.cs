using System;

namespace LiveNewsFeed.DataSource.DennikNsk
{
    public class DownloadException : Exception
    {
        public DownloadException()
        {
        }

        public DownloadException(string message) : base(message)
        {
        }

        public DownloadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}