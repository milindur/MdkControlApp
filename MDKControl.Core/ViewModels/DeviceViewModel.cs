using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MDKControl.Core.Services;
using Robotics.Mobile.Core.Bluetooth.LE;
using Xamarin.Forms;

namespace MDKControl.Core.ViewModels
{
    public class DeviceViewModel : ViewModelBase
    {
        private readonly IMoCoBusCommService _commService;
        private readonly MoCoBusProtocolService _protocolService;

        public DeviceViewModel(IMoCoBusCommService commService)
        {
            _commService = commService;
            _commService.ConnectionChanged += CommServiceOnConnectionChanged;
            _protocolService = new MoCoBusProtocolService(_commService, 3);
        }

        private void CommServiceOnConnectionChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(() => IsConnected);
        }

        public bool IsConnected
        {
            get
            {
                return _commService.ConnectionState == ConnectionState.Connecting
                       || _commService.ConnectionState == ConnectionState.Connected;
            }
            set
            {
                if (_commService.ConnectionState == ConnectionState.Disconnected && value)
                {
                    _commService.Connect();
                }
                else if (_commService.ConnectionState != ConnectionState.Disconnected && !value)
                {
                    _commService.Disconnect();
                }
                OnPropertyChanged(() => IsConnected);
            }
        }

        public int FirmwareVersion { get; private set; }

        private ICommand _testCommand;
        public ICommand TestCommand { get { return _testCommand ?? (_testCommand = new Command(async () => await Test())); } }

        private async Task Test()
        {
            FirmwareVersion = await _protocolService.GetFirmwareVersion();
            OnPropertyChanged(() => FirmwareVersion);
        }
    }
}
