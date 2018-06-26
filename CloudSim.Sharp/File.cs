using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{

    public class File
    {
        public static readonly int NOT_REGISTERED = -1;
        public static readonly int TYPE_UNKOWN = 0;
        public static readonly int TYPE_RAW_DATA = 1;
        public static readonly int TYPE_RECONSTRUCTED_DATA = 2;
        public static readonly int TYPE_TAG_DATA = 3;

        private double _transactionTime;

        public FileAttribute Attribute { get; }

        public String Name
        {
            get { return Attribute.Name; }
            set { Attribute.Name = value; }
        }

        public int RegistrationId
        {
            get { return Attribute.RegistrationId; }
            set { Attribute.RegistrationId = value; }
        }

        public String OwnerName
        {
            get { return Attribute.OwnerName; }
            set { Attribute.OwnerName = value; }
        }

        public int Size
        {
            get { return Attribute.FileSize; }
            set { Attribute.FileSize = value; }
        }

        public int SizeInBytes
        {
            get { return Attribute.FileSizeInBytes; }
        }

        public bool MasterCopy
        {
            get { return Attribute.MasterCopy; }
            set { Attribute.MasterCopy = value; }
        }

        public double LastUpdateTime
        {
            get { return Attribute.LastUpdateTime; }
            set { Attribute.LastUpdateTime = value; }
        }

        public int Type
        {
            get { return Attribute.Type; }
            set { Attribute.Type = value; }
        }

        public int Checksum
        {
            get { return Attribute.Checksum; }
            set { Attribute.Checksum = value; }
        }

        public double Cost
        {
            get { return Attribute.Cost; }
            set { Attribute.Cost = value; }
        }

        public bool ReadOnly
        {
            get { return Attribute.ReadOnly; }
            set { Attribute.ReadOnly = value; }
        }

        public long CreationTime { get { return Attribute.CreationTime; } }

        public int AttributeSize { get { return Attribute.AttributeSize; } }

        public int ResourceId { get { return Attribute.ResourceId; } }

        public double TransactionTime
        {
            get { return _transactionTime; }
            set
            {
                if (value < 0)
                    throw new ArgumentException
                        ("TransactionTime can not be less than 0.");
                _transactionTime = value;
            }
        }


        public File(String fileName, int fileSize)
        {
            if (fileName == null || fileName.Length == 0)
            {
                throw new ArgumentException("Invalid file name.");
            }

            if (fileSize <= 0)
            {
                throw new ArgumentException("File size less than 0.");
            }

            Name = fileName;
            Attribute = new FileAttribute(fileName, fileSize);
            TransactionTime = 0;
        }

        public File(File file)
        {
            if (file == null)
            {
                throw new ArgumentException("File is null.");
            }

            FileAttribute fileAttr = file.Attribute;
            Attribute.CopyValue(fileAttr);
            MasterCopy = false;
        }

        public File MakeReplica()
        {
            return MakeCopy();
        }

        public File MakeMasterCopy()
        {
            File file = MakeCopy();
            if (file != null)
            {
                file.MasterCopy = true;
            }

            return file;
        }

        private File MakeCopy()
        {
            File file = null;

            try
            {
                file = new File(Name, Attribute.FileSize);
                FileAttribute fileAttr = file.Attribute;
                Attribute.CopyValue(fileAttr);
                fileAttr.MasterCopy = false;
            }
            catch (Exception)
            {
                file = null;
            }

            return file;
        }

        public bool IsRegistered
        {
            get { return Attribute.IsRegistered; }
        }

    }
}
