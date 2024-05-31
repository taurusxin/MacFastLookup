using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class FileDownloader
{
    private readonly string url;
    private readonly string destinationFilePath;
    private readonly Action<double> progressCallback;
    private readonly Action<string> completionCallback;
    private CancellationTokenSource cancellationTokenSource;

    public FileDownloader(string url, string destinationFilePath, Action<double> progressCallback, Action<string> completionCallback)
    {
        this.url = url;
        this.destinationFilePath = destinationFilePath;
        this.progressCallback = progressCallback;
        this.completionCallback = completionCallback;
        this.cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task StartDownloadAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token))
                {
                    response.EnsureSuccessStatusCode();

                    long? contentLength = response.Content.Headers.ContentLength;

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                  fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationTokenSource.Token);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                                TriggerProgressChanged(totalRead, contentLength);
                                continue;
                            }

                            await fileStream.WriteAsync(buffer, 0, read, cancellationTokenSource.Token);
                            totalRead += read;
                            TriggerProgressChanged(totalRead, contentLength);
                        }
                        while (isMoreToRead);
                    }
                }

                Console.WriteLine("下载完成！");
                completionCallback?.Invoke(destinationFilePath);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("下载已取消。");
                // 删除未完成的文件
                if (File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }
                completionCallback?.Invoke(null); // 表示下载取消
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下载过程中出现错误: {ex.Message}");
                // 删除未完成的文件
                if (File.Exists(destinationFilePath))
                {
                    File.Delete(destinationFilePath);
                }
                completionCallback?.Invoke(null); // 表示下载失败
            }
        }
    }

    public void CancelDownload()
    {
        cancellationTokenSource.Cancel();
    }

    private void TriggerProgressChanged(long totalRead, long? contentLength)
    {
        if (contentLength.HasValue)
        {
            double progress = (double)totalRead / contentLength.Value * 100;
            progressCallback?.Invoke(progress);
        }
        else
        {
            progressCallback?.Invoke(-1); // 表示未知的进度
        }
    }
}
