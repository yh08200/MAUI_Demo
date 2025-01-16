using MauiAppDemo.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppDemo.VM
{
    /// <summary>
    /// udp消息接收类
    /// </summary>
    public class UdpViewModel : INotifyPropertyChanged
    {
        private string _receivedMessage=string.Empty;

        public string ReceivedMessage
        {
            get => _receivedMessage;
            set
            {
                if (_receivedMessage != value)
                {
                    _receivedMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private Timer _timer;
        private readonly string _targetIp = "10.10.100.254";
        private readonly int _targetPort = 8899;

        public void StartSending(string message, double intervalMs)
        {
            StopSending(); // 确保只有一个定时器运行
            _timer = new Timer(async _ =>
            {
                await UDPHelper.Instance.SendMessageAsync(message, _targetIp, _targetPort);
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(intervalMs));
        }

        public void StopSending()
        {
            _timer?.Dispose();
            _timer = null;
        }

        public void StartReceiving()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    string receivedMessage = await UDPHelper.Instance.ReceiveMessageAsync();
                    if (!string.IsNullOrEmpty(receivedMessage))
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            ReceivedMessage += receivedMessage;
                        });
                    }
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
