using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MauiAppDemo.Common
{
    /// <summary>
    /// udp帮助类
    /// </summary>
    public class UDPHelper
    {
        private static readonly Lazy<UDPHelper> _instance = new(() => new UDPHelper());
        private static readonly object _lock = new(); // 用于线程安全的锁
        private UdpClient _udpClient;

        private CancellationTokenSource _cancellationTokenSource;

        private int _port;

        // 单例模式，构造函数设为私有
        private UDPHelper()
        {
            // 初始化 UdpClient，不绑定端口以支持发送
            _udpClient = new UdpClient();
        }

        // 获取单例实例
        public static UDPHelper Instance => _instance.Value;

        // 设置监听端口
        public void Initialize(int port)
        {
            lock (_lock)
            {
                _port = port;
                // 关闭之前的客户端（如果存在），并重新初始化
                _udpClient?.Close();
                _udpClient = new UdpClient(_port);
            }
        }

        // 发送消息（线程安全）
        public async Task SendMessageAsync(string message, string ipAddress, int port)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await _udpClient.SendAsync(data, data.Length, ipAddress, port);
        }

        // 接收消息（线程安全）
        public async Task<string> ReceiveMessageAsync()
        {
            try
            {
                var result = await _udpClient.ReceiveAsync();
                return Encoding.UTF8.GetString(result.Buffer);
            }
            catch (Exception ex)
            {
                // 打印错误日志以检查问题
                Console.WriteLine($"接收消息时出错: {ex.Message}");
                return string.Empty;
            }
        }

        public void StopReceiving()
        {
            _cancellationTokenSource?.Cancel();
            _udpClient?.Close();
            _udpClient = null;
        }
    }
}
