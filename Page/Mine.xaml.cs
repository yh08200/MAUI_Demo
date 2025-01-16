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
        // �ֶ�ʵ����������Ϊ BindingContext
        _viewModel = new UdpViewModel();
        BindingContext = _viewModel;

        UDPHelper.Instance.Initialize(8899); // ���ض˿ڳ�ʼ��
        _viewModel.StartReceiving();         // ��ʼ������Ϣ

        // ���� ReceivedMessage ���Ա仯
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UdpViewModel.ReceivedMessage))
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // �Զ����������һ����Ϣ
                await MessageScrollView.ScrollToAsync(ReceivedMessageLabel, ScrollToPosition.End, true);
            });
        }
    }

    private async void backBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(); // ���ص���һҳ��
    }

    private void connectBtn_Clicked(object sender, EventArgs e)
    {

    }

    private void OnStartSendingClicked(object sender, EventArgs e)
    {
        string message = MessageEntry.Text;
        if (!string.IsNullOrWhiteSpace(message))
        {
            _viewModel.StartSending(message + "\r\n", 1000); // ÿ�뷢��һ��
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