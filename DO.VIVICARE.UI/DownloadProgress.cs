using System;

namespace DO.VIVICARE.UI
{
    /// <summary>
    /// Progress report per il download di un plugin
    /// </summary>
    public class DownloadProgress
    {
        public long BytesDownloaded { get; set; }
        public long TotalBytes { get; set; }
        public DateTime StartTime { get; set; }

        public int PercentComplete =>
            TotalBytes > 0 ? (int)((BytesDownloaded * 100) / TotalBytes) : 0;

        public TimeSpan ElapsedTime =>
            DateTime.UtcNow - StartTime;

        public TimeSpan EstimatedTimeRemaining
        {
            get
            {
                if (BytesDownloaded == 0 || ElapsedTime.TotalSeconds == 0)
                    return TimeSpan.Zero;

                double bytesPerSecond = BytesDownloaded / ElapsedTime.TotalSeconds;
                long remainingBytes = TotalBytes - BytesDownloaded;
                double secondsRemaining = remainingBytes / bytesPerSecond;

                return TimeSpan.FromSeconds(secondsRemaining);
            }
        }
    }
}
