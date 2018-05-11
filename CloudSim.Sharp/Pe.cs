using CloudSim.Sharp.Provisioners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSim.Sharp
{
    public class Pe
    {
        public const int FREE = 1;
        public const int BUSY = 2;
        public const int FAILED = 3;

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public void SetStatusFree()
        {
            Status = FREE;
        }

        public void SetStatusBusy()
        {
            Status = BUSY;
        }

        public void SetStatusFailed()
        {
            Status = FAILED;
        }

        public PeProvisioner PeProvisioner
        {
            get { return _peProvisioner; }
            protected set { _peProvisioner = value; }
        }

        private int _id;
        private int _status;
        private PeProvisioner _peProvisioner;

        public Pe(int id, PeProvisioner peProvisioner)
        {
            Id = id;
            PeProvisioner = peProvisioner;
            _status = FREE;
        }

        public int Mips
        {
            get { return (int)PeProvisioner.Mips; }
            set { PeProvisioner.Mips = value; }
        }
    }
}
