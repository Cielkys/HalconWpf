using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HalconDotNet;
using HalconWpf.Halcon.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalconWpf.ViewModels
{
    public partial class CameraViewModel : ObservableObject
    {
        private ICameraService _cameraService;

        /// <summary>相机连接/采集状态文本。</summary>
        [ObservableProperty]
        private string _cameraStatus = "未连接";

        /// <summary>连接/断开进行中，防止重复点击。</summary>
        [ObservableProperty]
        private bool _isBusy;

        public CameraViewModel(ICameraService cameraService)
        {
            _cameraService = cameraService;

        }


        [RelayCommand]
        public void OpenCamera()
        {
            if (IsBusy) return;
            IsBusy = true;
            CameraStatus = "连接中…";
            // OpenFramegrabber 是驱动级阻塞调用，放 UI 线程会冻住整个界面
            Task.Run(() =>
            {
                try
                {
                    _cameraService.ConnectCamera();
                    CameraStatus = "已连接";
                }
                catch (Exception ex)
                {
                    CameraStatus = "连接失败：" + ex.Message;
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        [RelayCommand]
        public void DisconnectCamera()
        {
            if (IsBusy) return;
            IsBusy = true;
            CameraStatus = "断开中…";
            Task.Run(() =>
            {
                try
                {
                    _cameraService.DisconnectCamera();
                    CameraStatus = "未连接";
                }
                catch (Exception ex)
                {
                    CameraStatus = "断开失败：" + ex.Message;
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        [RelayCommand]
        public void StartCollect()
        {
            if (IsBusy) return;
            CameraStatus = "采集中…";
            // CollectImage 是阻塞循环，必须离开 UI 线程，否则界面卡死
            Task.Run(() =>
            {
                try
                {
                    _cameraService.CollectImage();
                    CameraStatus = "已连接";
                }
                catch (Exception ex)
                {
                    CameraStatus = "采集失败：" + ex.Message;
                }
            });
        }

        [RelayCommand]
        public void StopCollect()
        {
            _cameraService.DisCollectImage();
        }
    }
}
