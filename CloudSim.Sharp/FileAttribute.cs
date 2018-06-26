using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class FileAttribute
    {
        private int _Id;
        private long _creationTime;
        private int _resourceId;
        private String _ownerName;
        private int _size;
        private double _updateTime;
        private int _type;
        private int _checksum;
        private double _cost;

        public int AttributeSize
        {
            get
            {
                int length = (int)DataCloudTags.PKT_SIZE;
                if (_ownerName != null)
                {
                    length += _ownerName.Length;
                }

                if (Name != null)
                {
                    length += Name.Length;
                }

                return length;
            }
        }

        public int RegistrationId
        {
            get { return _Id; }
            set {
                if (value < 0)
                    throw new ArgumentException
                        ("RegistrationId can not be less than 0.");
                _Id = value;
            }
        }

        public int Checksum
        {
            get { return _checksum; }
            set
            {
                if (value < 0)
                    throw new ArgumentException
                        ("Checksum can not be lass than 0.");
                _checksum = value;
            }
        }

        public bool MasterCopy
        {
            get;
            set;
        }

        public bool ReadOnly
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }

        public String OwnerName
        {
            get { return _ownerName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException
                        ("OwnerName can not be null or empty");
                _ownerName = value;
            }
        }

        public int Type
        {
            get { return _type; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Wrong Type.");

                _type = value;
            }
        }

        public int FileSize
        {
            get { return _size; }
            set
            {
                if (value < 0)
                    throw new ArgumentException
                        ("FileSize can not be less than 0.");
                _size = value;
            }
        }

        public int FileSizeInBytes { get { return _size * Consts.MILLION; } }

        public double LastUpdateTime
        {
            get { return _updateTime; }
            set
            {
                if (value < _updateTime)
                    throw new ArgumentException
                        ("LastUpdate can not be less " +
                        "than previous update time or 0.");
                _updateTime = value;
            }
        }

        public long CreationTime
        {
            get { return _creationTime; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("");
                _creationTime = value;
            }
        }

        public double Cost
        {
            get { return _cost; }
            set
            {
                if (value < 0)
                    throw new ArgumentException
                        ("Cost can not be less than 0.");
                _cost = value;
            }
        }

        public int ResourceId
        {
            get { return _resourceId; }
            set
            {
                if (value == -1)
                    throw new ArgumentException("ResourceId can not be -1");
                _resourceId = value;
            }
        }

        public FileAttribute(String fileName, int fileSize)
        {

            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("Invalid file name.");
            }

            if (fileSize <= 0)
            {
                throw new ArgumentException("FileAttribute size less than 0.");
            }

            FileSize = fileSize;
            Name = fileName;

            DateTime date = CloudSim.Sharp.Core.CloudSim.GetSimulationDateTime();

            if (date == null)
            {
                CreationTime = 0;
            }
            else
            {
                CreationTime = date.TimeOfDay.Milliseconds;
            }

            OwnerName = null;
            RegistrationId = File.NOT_REGISTERED;
            Checksum = 0;
            Type = File.TYPE_UNKOWN;
            LastUpdateTime = 0;
            Cost = 0;
            ResourceId = -1;
            MasterCopy = true;
            ReadOnly = false;
        }

        public bool CopyValue(FileAttribute attr)
        {
            if (attr == null)
            {
                return false;
            }

            attr.FileSize = FileSize;
            attr.ResourceId = ResourceId;
            attr.OwnerName = OwnerName;
            attr.LastUpdateTime = LastUpdateTime;
            attr.RegistrationId = RegistrationId;
            attr.Type = Type;
            attr.Checksum = Checksum;
            attr.Cost = Cost;
            attr.MasterCopy = MasterCopy;
            attr.ReadOnly = ReadOnly;
            attr.Name = Name;
            attr.CreationTime = CreationTime;

            return true;
        }

        public bool IsRegistered
        {
            get
            {
                bool result = true;
                if (_Id == File.NOT_REGISTERED)
                {
                    result = false;
                }

                return result;
            }
        }
        
    }
}
