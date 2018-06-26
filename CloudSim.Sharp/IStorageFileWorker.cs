using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public interface IStorageFileWorker
    {
        double AddReservedFile(File file);

        File GetFile(String fileName);

        List<String> FileNameList { get; }

        double AddFile(File file);

        double AddFile(List<File> list);

        File DeleteFile(String fileName);

        double DeleteFile(String fileName, File file);

        double DeleteFile(File file);

        bool Contains(String fileName);

        bool Contains(File file);

        bool RenameFile(File file, String newName);
    }
}
