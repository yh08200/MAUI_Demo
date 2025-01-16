using MauiAppDemo.Common;
using MauiAppDemo.VM;
using System.ComponentModel;

namespace MauiAppDemo.Page;

public partial class Mine : ContentPage
{
    private readonly UdpViewModel _viewModel;
    public Mine()
    {
        InitializeComponent();
        // 手动实例化并设置为 BindingContext
        _viewModel = new UdpViewModel();
        BindingContext = _viewModel;

        UDPHelper.Instance.Initialize(8899); // 本地端口初始化
        _viewModel.StartReceiving();         // 开始接收消息

        // 监听 ReceivedMessage 属性变化
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UdpViewModel.ReceivedMessage))
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // 自动滚动到最后一条消息
                await MessageScrollView.ScrollToAsync(ReceivedMessageLabel, ScrollToPosition.End, true);
            });
        }
    }

    private async void backBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // 返回到上一页面
    }

    private void connectBtn_Clicked(object sender, EventArgs e)
    {

    }

    private void OnStartSendingClicked(object sender, EventArgs e)
    {
        string message = MessageEntry.Text;
        if (!string.IsNullOrWhiteSpace(message))
        {
            _viewModel.StartSending(message + "\r\n", 1000); // 每秒发送一次
        }
    }

    private void OnStopSendingClicked(object sender, EventArgs e)
    {
        _viewModel.StopSending();
    }

    private async void StartReceivingMessages()
    {
        while (true)
        {
            string receivedMessage = await UDPHelper.Instance.ReceiveMessageAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                _viewModel.ReceivedMessage = receivedMessage;
            });
        }
    }
}