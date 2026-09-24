using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HalconWpf.Halcon.Camera
{
    public class CameraService:ICameraService
    {
        private readonly IHalconService _halcon;

        public CameraService(IHalconService halcon)
        {
            _halcon = halcon;
        }

        HTuple? _acqHandle;
        HObject? _image;
        bool state;
        public void ConnectCamera()
        {
            HOperatorSet.OpenFramegrabber("MVision", 1, 1, 0, 0, 0, 0, "progressive", 8,"default", -1, "false", "manual","U3V:Vir15672957 MV-CA003-21UC", 0, -1, out _acqHandle);
            HOperatorSet.SetFramegrabberParam(_acqHandle, "PixelFormat", "Mono8");
        }

        public void DisconnectCamera()
        {
            HOperatorSet.CloseFramegrabber(_acqHandle);
        }

        public void CollectImage()
        {
            state = true;
            
            HOperatorSet.GrabImageStart(_acqHandle, -1);
            while(state)
            {
                HOperatorSet.GrabImageAsync(out _image, _acqHandle, 10000);
                _halcon.DisplayFrame(_image);
            }
        }

        public void DisCollectImage()
        {
            state = false;
        }
    }
}
