using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalconWpf.Halcon.Camera
{
    public interface ICameraService
    {
        /// <summary>
        /// 连接相机
        /// </summary>
        public void ConnectCamera();

        public void DisconnectCamera();

        public void CollectImage();

        public void DisCollectImage();
    }
}
