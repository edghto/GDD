using System;
using System.IO;
using System.Threading.Tasks;

namespace GDD
{
    public delegate void ChunkCopiedEventHandler(int size);

    public delegate void ErrorEventHandler(string message);

    public delegate void FinishedEventHandler();

    public class Copier
    {
        public event ChunkCopiedEventHandler ChunkCopied;

        public event ErrorEventHandler Error;

        public event FinishedEventHandler Finished;
        
        public async System.Threading.Tasks.Task<bool> CopyAsync(Stream dst, Stream src)
        {
            try
            {
                while (true)
                {
                    int size = 4096;
                    byte[] b = new byte[size];
                    size = await src.ReadAsync(b, 0, size);
                    if (size <= 0)
                        break;
                    await dst.WriteAsync(b, 0, size);

                    if(ChunkCopied != null)
                        ChunkCopied(size);
                }
                await dst.FlushAsync(); // it is necessary hack to copy content to gdrive - check iplementation of GDriveUploadStream
            }
            catch(IOException e)
            {
                if (Error != null)
                    Error(e.Message);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("An error occurred: " + e.Message);
#endif
                return false;
            }

            if (Finished != null)
                Finished();

            return true;
        }
    }

    public class CopierManager : IDisposable
    {
        private File srcFile;
        private File dstFile;
        private IDrive copiableTo;
        
        public CopierManager(IDrive copiableTo, File srcFile, File dstFile)
        {
            this.copiableTo = copiableTo;
            this.srcFile = srcFile;
            this.dstFile = dstFile;
        }

        public async Task CopyAsync()
        {
            System.IO.Stream srcStream = StreamFactory.GetStream(srcFile, FileAccess.Read);
            await System.Threading.Tasks.Task.Run(async () => { await copiableTo.CopyTo(dstFile, srcStream); });
        }

        public void Dispose()
        {
        }
    }
}