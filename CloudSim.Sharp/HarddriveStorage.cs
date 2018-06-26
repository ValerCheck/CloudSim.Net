using CloudSim.Sharp.Distributions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class HarddriveStorage : IStorage
    {

        private List<File> _fileList;
        private IContinuosDistribution _gen;
        private double _currentSize;
        private readonly double _capacity;
        private double _latency;
        private double _avgSeekTime;

        public double Latency
        {
            get { return _latency; }
            set
            {
                if (value >= 0)
                    _latency = value;
                else
                    throw new ArgumentException();
            }
        }

        public double AvgSeekTime
        {
            get { return _avgSeekTime; }
            set { SetAvgSeekTime(value, null); }
        }

        public string Name { get; }
        public double Capacity { get; }
        public double CurrentSize { get; }
        public virtual double MaxTransferRate { get; private set; }

        public double AvailableSpace
        {
            get { return _currentSize - _capacity; }
        }

        public int NumStoredFile { get; set; }
        public List<String> FileNameList { get; private set; }

        public HarddriveStorage(String name, double capacity)
        {
            if (name == null || name.Length == 0)
            {
                throw new ArgumentException
                        ("HarddriveStorage(): Error - invalid storage name.");
            }

            if (capacity <= 0)
            {
                throw new ArgumentException
                    ("HarddriveStorage(): Error - capacity <= 0.");
            }

            Name = name;

            _capacity = capacity;

            Init();

        }

        public HarddriveStorage(double capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException
                            ("HarddriveStorage(): Error - capacity <= 0.");
            }
            Name = "HarddriveStorage";

            _capacity = capacity;

            Init();
        }

        private void Init()
        {
            _fileList = new List<File>();
            FileNameList = new List<String>();
            _gen = null;
            _currentSize = 0;

            _latency = 0.00417;     
            _avgSeekTime = 0.009;   
            MaxTransferRate = 133; 
        }

        public bool IsFull()
        {
            if (Math.Abs(AvailableSpace) < .0000001)
            {
                return true;
            }
            return false;
        }

        public int GetNumStoredFile()
        {
            return _fileList.Count;
        }

        public bool ReserveSpace(int fileSize)
        {
            if (fileSize <= 0)
            {
                return false;
            }

            if (_currentSize + fileSize >= _capacity)
            {
                return false;
            }

            _currentSize += fileSize;
            return true;
        }

        public virtual double AddReservedFile(File file)
        {
            if (file == null)
            {
                return 0;
            }

            _currentSize -= file.Size;
            double result = AddFile(file);

            if (result == 0.0)
            {
                _currentSize += file.Size;
            }

            return result;
        }

        public bool HasPotentialAvailableSpace(int fileSize)
        {
            if (fileSize <= 0)
            {
                return false;
            }

            if (AvailableSpace > fileSize)
            {
                return true;
            }

            int deletedFileSize = 0;

            bool result = false;
            foreach (var file in _fileList)
            {
                if (!file.ReadOnly)
                {
                    deletedFileSize += (int)file.Size;
                }

                if (deletedFileSize > fileSize)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public bool SetAvgSeekTime
            (double seekTime, IContinuosDistribution gen)
        {
            if (seekTime <= 0.0)
            {
                return false;
            }

            _avgSeekTime = seekTime;
            _gen = gen;
            return true;
        }

        public File GetFile(String fileName)
        {
            File obj = null;

            if (String.IsNullOrEmpty(fileName))
            {
                Log.WriteConcatLine(fileName,
                    ".getFile(): Warning - invalid " + "file name.");
                return obj;
            }

            int size = 0;
            int index = 0;
            bool found = false;
            File tempFile = null;

            foreach (var file in _fileList)
            {
                size += file.Size;

                if (file.Name.Equals(fileName))
                {
                    found = true;
                    obj = tempFile;
                    break;
                }

                index++;
            }

            if (found)
            {
                obj = _fileList[index];
                double seekTime = GetSeekTime(size);
                double transferTime = GetTransferTime(obj.Size);

                obj.TransactionTime = seekTime + transferTime;
            }

            return obj;
        }

        private double GetSeekTime(int fileSize)
        {
            double result = 0;

            if (_gen != null)
            {
                result += _gen.Sample();
            }

            if (fileSize > 0 && Capacity != 0)
            {
                result += (fileSize / Capacity);
            }

            return result;
        }

        private double GetTransferTime(int fileSize)
        {
            double result = 0;
            if (fileSize > 0 && Capacity != 0)
            {
                result = (fileSize * MaxTransferRate) / Capacity;
            }

            return result;
        }

        private bool IsFileValid(File file, String methodName)
        {

            if (file == null)
            {
                Log.WriteConcatLine
                    (Name, ".", methodName, 
                    ": Warning - the given file is null.");

                return false;
            }

            String fileName = file.Name;
            if (String.IsNullOrEmpty(fileName))
            {
                Log.WriteConcatLine
                    (Name, "." + methodName,
                    ": Warning - invalid file name.");

                return false;
            }

            return true;
        }

        public virtual double AddFile(File file)
        {
            double result = 0.0;
           
            if (!IsFileValid(file, "addFile()"))
            {
                return result;
            }

            if (file.Size + CurrentSize > Capacity)
            {
                Log.WriteConcatLine
                    (Name, ".addFile(): " +
                    "Warning - not enough space to store ",
                    file.Name);

                return result;
            }
            
            if (!Contains(file.Name))
            {
                double seekTime = GetSeekTime(file.Size);
                double transferTime = GetTransferTime(file.Size);

                _fileList.Add(file);              
                FileNameList.Add(file.Name);     
                _currentSize += file.Size;    
                result = seekTime + transferTime;  
            }
            file.TransactionTime = result;
            return result;
        }

        public virtual double AddFile(List<File> list)
        {
            double result = 0.0;
            if (list == null || list.Count == 0)
            {
                Log.WriteConcatLine
                    (Name, ".addFile(): Warning - list is empty.");

                return result;
            }

            list.ForEach(x => result += AddFile(x));
           
            return result;
        }

        public virtual File DeleteFile(String fileName)
        {
            File resultFile = null;

            if (String.IsNullOrEmpty(fileName))
            {
                return resultFile;
            }
            List<File> files = _fileList;

            foreach (var file in files)
            {
                String name = file.Name;
                
                if (fileName.Equals(name))
                {
                    resultFile = file;
                    double result = DeleteFile(file);
                    file.TransactionTime = result;
                    break;
                }
                
            }
            return resultFile;
        }

        public virtual double DeleteFile(String fileName, File file) 
            => DeleteFile(file);

        public virtual double DeleteFile(File file)
        {
            double result = 0.0;
            
            if (!IsFileValid(file, "deleteFile()"))
            {
                return result;
            }
            double seekTime = GetSeekTime(file.Size);
            double transferTime = GetTransferTime(file.Size);
            
            if (Contains(file))
            {
                _fileList.Remove(file);       
                FileNameList.Remove(file.Name); 
                _currentSize -= file.Size;   
                result = seekTime + transferTime;  
                file.TransactionTime = result;
            }
            return result;
        }

        public bool Contains(String fileName)
        {
            bool result = false;
            if (String.IsNullOrEmpty(fileName))
            {
                Log.WriteConcatLine
                    (Name, ".contains(): Warning - invalid file name");

                return result;
            }

            result = FileNameList.Contains(fileName);

            return result;
        }

        public bool Contains(File file)
        {
            bool result = false;
            if (!IsFileValid(file, "contains()"))
            {
                return result;
            }

            result = Contains(file.Name);

            return result;
        }

        public bool RenameFile(File file, String newName)
        {
            bool result = false;

            if (Contains(newName))
            {
                return result;
            }

            File obj = GetFile(file.Name);

            if (obj == null)
            {
                return result;
            }
            else
            {
                obj.Name = newName;
            }

            List<String> names = FileNameList;

            foreach (var name in names)
            {

                if (name.Equals(file.Name))
                {
                    file.TransactionTime = 0;
                    FileNameList.Remove(name);
                    FileNameList.Add(newName);
                    result = true;
                    break;
                }
            }

            return result;
        }
        
    }
}

