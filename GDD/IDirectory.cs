using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace GDD
{
    public enum FileAccess
    {
        Read, Write
    }

    public class File
    {
        virtual public string Id { get; set; }
        virtual public string Title { get; set; }
        virtual public bool IsDirectory { get; set; }
        virtual public long Length { get; set; }
    }

    public interface Drive
    {
        string Name { get; }
    }

    public interface IDrive
    {
        /* Memebers for UI Binding */
        string Name { get; }
        bool IsInteractive { get; }

        ObservableCollection<File> FileCollection { get; set; }

        ObservableCollection<Drive> Drives { get; } //only if is interactive
        Drive CurrentDrive { get; set; } //only if is interactive

        /* File management API */
        bool ChangeDirectory(object newDir);
        string GetCurrentDir();
        Collection<File> GetListing(object dir);
        Collection<File> GetListing();
        File GetNewFile(string title, string target);
        Task<bool> CopyTo(object dst, Stream src);
    }

    //public interface IDrive
    //{
    //    string Name { get; }
    //    bool IsInteractive { get; }

    //    ObservableCollection<Drive> Drives { get; }
    //    Drive CurrentDrive { get; set; }

    //    bool ChangeDirectory(object newDir);
    //    string GetCurrentDir();
    //    Collection<File> GetListing(object dir);
    //    Collection<File> GetListing();
    //    File GetNewFile(string title, string target);
    //    Task<bool> CopyTo(object dst, Stream src);
    //}
}