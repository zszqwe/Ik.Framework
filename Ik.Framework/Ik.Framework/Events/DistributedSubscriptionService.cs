using Ik.Framework.Configuration;
using Ik.Framework.Logging;
using IkZooKeeperNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;
using System.Threading;
using Ik.Framework.ZooKeeperNet;

namespace Ik.Framework.Events
{
    public class DistributedSubscriptionService : BackgroundServiceBase, IEventTransport
    {
        private static ILog _logger = LogManager.GetLogger(SubscriptionManager.LogModelName_SubscriptionService);
        private IkZooKeeperClient _client = null;
		public const string EVENT_TOPIC_ROOT = "event_topics";
        private ConcurrentDictionary<string, DistributedEventPublisherStatus> _publishers = new ConcurrentDictionary<string, DistributedEventPublisherStatus>();
        private BlockingCollection<ClearRegisteredConsumer> sourceArrays = new BlockingCollection<ClearRegisteredConsumer>(int.MaxValue);
        protected readonly IList<TimerWorkerThread> workerThreads = new List<TimerWorkerThread>();
        private int _port = 9191;
        private ServiceHost host = null;
        private bool _islistener = false;
        private int _heartbeatCheckMilliseconds = 10000;
        private string _eventServerAddress = "";

        protected override void StartService()
        {
            int port = ConfigManager.Instance.Get<int>("event_listener_port");
            if (port > 0)
            {
                this._port = port;
            }

            int heartbeatCheckMilliseconds = ConfigManager.Instance.Get<int>("event_heartbeat_check");
            if (heartbeatCheckMilliseconds > 0)
            {
                this._heartbeatCheckMilliseconds = heartbeatCheckMilliseconds;
            }
			string eventServerAddress = ConfigManager.Instance.Get<string>("event_server_address");
            if (string.IsNullOrEmpty(eventServerAddress))
            {
                throw new EventException("事件服务地址不能为空");
            }
            this._eventServerAddress = eventServerAddress;
            this._client = new IkZooKeeperClient(eventServerAddress, TimeSpan.FromSeconds(15), null);
            _logger.Info(string.Format("开始分布式事件订阅服务，监听端口：{0}，心跳检查时间：{1}，事件服务地址：{2}", this._port, this._heartbeatCheckMilliseconds, eventServerAddress));
            Task.Run(() =>
            {
                foreach (var item in sourceArrays.GetConsumingEnumerable())
                {
                    if (item.ClearType == ClearType.Address)
                    {
                        var publisher = GetEventPublisher(item.Topic, item.Address);
                        bool flag = false;
                        try
                        {
                            publisher.EventPublisher.Heartbeat();
                            flag = true;
                            break;
                        }
                        catch { }
                        if (!flag)
                        {
                            var path = BuildZooKeeperPath(item.Topic, item.Address);
                            try
                            {
                                if (_client.EnsureExists(path, false))
                                {
                                    _client.EnsureDelete(path);
                                }
                                if(!RemoveEventPublisher(item.Topic, item.Address))
                                {
                                    _logger.Error(string.Format("清理通信通道错误，主题：{0}，地址：{1}", item.Topic, item.Address));
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(string.Format("清理消费错误，主题：{0}，地址：{1}", item.Topic, item.Address), ex);
                            }
                        }
                    }
                    else
                    {
                        var path = BuildZooKeeperPath(item.Topic);
                        try
                        {
                            if (_client.EnsureExists(path, false))
                            {
                                _client.EnsureDelete(path);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(string.Format("清理消费错误，主题：{0}", item.Topic), ex);
                        }
                    }
                }
            });
            CheckTopicServerHeartbeat(); 
            EnsureSubscribe();
            var worker = AddWorkerThread();
            worker.Start();
        }

        private void EnsureSubscribe()
        {
            IList<string> topics = new List<string>();
            string address = "";
            try
            {
                topics = LocalSubscribeContext.Current.GetDistributedSubscriptionTopics();

                if (!_islistener && topics.Count > 0)
                {
                    host = new ServiceHost(typeof(DistributedEventPublisher));
                    NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
                    host.AddServiceEndpoint(typeof(IDistributedEventPublisher), binding, string.Format("net.tcp://localhost:{0}/", this._port));
                    host.Open();
                    _islistener = true;
                    _logger.Info(string.Format("分布式事件订阅服务通信服务已打开，监听端口：{0}，心跳检查时间：{1}，事件服务地址：{2}", this._port, this._heartbeatCheckMilliseconds, this._eventServerAddress));
                }


                address = string.Format("{0}:{1}", GetIntranetIp(), this._port);
                foreach (var topic in topics)
                {
                    EnsureZooKeeperPath(topic, address);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("注册订阅主题错误,主题：{0}，地址：{1}", string.Join(",", topics.ToArray()), address), ex);
            }
        }

        protected override void ShutDownService()
        {
            _logger.Info("分布式订阅服务开始关闭");
            sourceArrays.CompleteAdding();

            var list = workerThreads.ToList();
            foreach (var item in list)
            {
                item.Stop();
            }
            while (workerThreads.Count > 0)
            {
                Thread.Sleep(200);
            }
            _logger.Info("心跳检查及通信清理服务关闭完成");
            var topics = LocalSubscribeContext.Current.GetDistributedSubscriptionTopics();

            var address = string.Format("{0}:{1}", GetIntranetIp(), this._port);
            foreach (var topic in topics)
            {
                var path = BuildZooKeeperPath(topic, address);
                try
                {
                    if (_client.EnsureExists(path, false))
                    {
                        _client.EnsureDelete(path);
                    }
                    _logger.Info(string.Format("取消订阅完成，主题：{0}，地址：{1}", topic, address));
                    RemoveEventPublisher(topic, address);
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("清理消费错误，主题：{0}，地址：{1}", topic, address), ex);
                }
            }
            _logger.Info(string.Format("已取消订阅{0}个", topics.Count));
            this._client.Dispose();
            if (this._islistener)
            {
                host.Close();
            }
            _logger.Info("分布式订阅服务关闭完成");
        }

        private TimerWorkerThread AddWorkerThread()
        {
            lock (workerThreads)
            {
                var result = new TimerWorkerThread(() =>
                {
                    EnsureSubscribe();
                    CheckTopicServerHeartbeat();
                }, this._heartbeatCheckMilliseconds);

                workerThreads.Add(result);

                result.Stopped += delegate(object sender, EventArgs e)
                {
                    var wt = sender as TimerWorkerThread;
                    lock (workerThreads)
                    {
                        workerThreads.Remove(wt);
                    }
                };

                return result;
            }
        }

        private void CheckTopicServerHeartbeat()
        {
            try
            {
                var topics = GetConsumerTopics();
                foreach (var topic in topics)
                {
                    var addresses = GetConsumerAddress(topic);
                    if (addresses.Count() == 0)
                    {
                        if (!sourceArrays.IsAddingCompleted)
                        {
                            sourceArrays.Add(new ClearRegisteredConsumer { Topic = topic, ClearType = ClearType.Topic });
                        }
                    }
                    foreach (var item in addresses)
                    {
                        var address = string.Format("{0}:{1}", GetIntranetIp(), this._port);
                        if (item == address)
                        {
                            continue;
                        }
                        bool isLive = false;
                        int index = 0;
                        while (index <= 2)
                        {
                            var publisher = GetEventPublisher(topic, item);
                            try
                            {
                                publisher.EventPublisher.Heartbeat();
                                isLive = true;
                                break;
                            }
                            catch
                            {
                                index++;
                            }
                        }
                        if (isLive)
                        {
                            continue;
                        }
                        if (!sourceArrays.IsAddingCompleted)
                        {
                            sourceArrays.Add(new ClearRegisteredConsumer { Topic = topic, Address = item, ClearType = ClearType.Address });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("心跳检查错误", ex);
            }
        }

        private string GetIntranetIp()
        {
            string localIP = "?";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        private string BuildZooKeeperPath()
        {
            return "/" + EVENT_TOPIC_ROOT;
        }

        private string BuildZooKeeperPath(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentNullException("主题不能为空");
            }
            return "/" + EVENT_TOPIC_ROOT + "/" + topic ;
        }

        private string BuildZooKeeperPath(string topic, string address)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentNullException("主题不能为空");
            }

            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("监听地址不能为空");
            }
            return "/" + EVENT_TOPIC_ROOT + "/" + topic + "/" + address;
        }

        private void EnsureZooKeeperPath(string topic, string address)
        {
            if (!_client.EnsureExists("/", false))
            {
                _client.EnsureCreate("/");
            }
            var root = BuildZooKeeperPath();
            if (!_client.EnsureExists(root, false))
            {
                _client.EnsureCreate(root);
            }
            var levelTopic = BuildZooKeeperPath(topic);
            if (!_client.EnsureExists(levelTopic, false))
            {
                _client.EnsureCreate(levelTopic);
            }

            var levelAddress = BuildZooKeeperPath(topic, address);
            if (!_client.EnsureExists(levelAddress, false))
            {
                _client.EnsureCreate(levelAddress);
            }
            
        }

        public bool SendEventMessage(DistributedEvent dEvent, DistributedConsumerType consumerType)
        {
            try
            {
                bool hasError = false;
                var list = GetConsumerAddress(dEvent.Topic);
                if (list.Count() == 0)
                {
                    EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，没有检测到任何分布式订阅对象，通知已退出，对象名：{0}", dEvent.ObjectName), dEvent.Topic, dEvent.Lable, dEvent.GetValue());
                    return true;
                }
                foreach (var item in list)
                {
                    bool flag = false;
                    var address = string.Format("{0}:{1}", GetIntranetIp(), this._port);
                    if (item == address)
                    {
                        continue;
                    }
                    EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，请求远程通信，地址：{0}，对象名：{1}", item, dEvent.ObjectName), dEvent.Topic, dEvent.Lable, dEvent.GetValue());
                    flag = SendEventMessage(dEvent, item);
                    if (flag)
                    {
                        EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，请求远程通信完成，地址：{0}，对象名：{1}", address, dEvent.ObjectName), dEvent.Topic, dEvent.Lable, dEvent.GetValue());
                        if (consumerType == DistributedConsumerType.OnlyOne)
                        {
                            break;
                        }
                    }
                    else
                    {
                        hasError = true;
                    }
                }
                EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，请求远程通信完成，对象名：{0}", dEvent.ObjectName), dEvent.Topic, dEvent.Lable, dEvent.GetValue());
                return hasError == true ? false : true;
            }
            catch(Exception ex)
            {
                EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，请求远程通信错误，对象名：{0}", dEvent.ObjectName), dEvent.Topic, dEvent.Lable, dEvent.GetValue(), ex);
            }
            return false;
        }

        private bool SendEventMessage(DistributedEvent dEvent, string address)
        {
            int index = 0;
            while (index <= 2)
            {
                var publisherStatus = GetEventPublisher(dEvent.Topic, address);
                if (publisherStatus.ErrorCount > 2)
                {
                    if (RemoveEventPublisher(dEvent.Topic, address))
                    {
                        publisherStatus = GetEventPublisher(dEvent.Topic, address);
                    }
                }
                try
                {
                    publisherStatus.EventPublisher.Process(dEvent);
                    return true;
                }
                catch (EndpointNotFoundException)
                {
                    if (!sourceArrays.IsAddingCompleted)
                    {
                        sourceArrays.Add(new ClearRegisteredConsumer { Topic = dEvent.Topic, Address = address });
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    publisherStatus.Error();
                    EventPublisher.WriteTrackLog(string.Format("发布分布式事件通知，请求远程通信错误，地址：{0}，对象名：{1},重试第{2}次", address, dEvent.ObjectName, index + 1), dEvent.Topic, dEvent.Lable, dEvent.GetValue(), ex);
                }
                index++;
            }
            
            return false;
        }

        private DistributedEventPublisherStatus GetEventPublisher(string topic, string address)
        {
            string key = string.Format("{0}.{1}", topic, address);
            return _publishers.GetOrAdd(key, k => CreateDistributedEventPublisher(address));
        }

        private bool RemoveEventPublisher(string topic, string address)
        {
            string key = string.Format("{0}.{1}", topic, address);
            DistributedEventPublisherStatus publisher;
            if (_publishers.TryRemove(key, out publisher))
            {
                return true;
            }
            return false;
        }

        private IEnumerable<string> GetConsumerAddress(string topic)
        {
            var path = BuildZooKeeperPath(topic);
            if (_client.EnsureExists(path, false))
            {
                return _client.EnsureGetChildren(path, false);
            }
            return new List<string>();
        }

        private IEnumerable<string> GetConsumerTopics()
        {
            var path = BuildZooKeeperPath();
            if (_client.EnsureExists(path, false))
            {
                return _client.EnsureGetChildren(path, false);
            }
            return new List<string>();
        }

        private DistributedEventPublisherStatus CreateDistributedEventPublisher(string address)
        {
            var localIp = GetIntranetIp();
            if (address.IndexOf(localIp) > -1)
            {
                address = address.Replace(localIp,"localhost");
            }
            EndpointAddress endPoint = new EndpointAddress(string.Format("net.tcp://{0}/", address));
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            ChannelFactory<IDistributedEventPublisher> factory = new ChannelFactory<IDistributedEventPublisher>(binding);
            return new DistributedEventPublisherStatus(factory.CreateChannel(endPoint));
        }




        public string GetEventServerAddress()
        {
            return string.Format("{0}:{1}", GetIntranetIp(), this._port);
        }
    }

    public class DistributedEventPublisherStatus
    {
        public DistributedEventPublisherStatus(IDistributedEventPublisher eventPublisher)
        {
            this.EventPublisher = eventPublisher;
        }

        private int errorCount = 0;
        public IDistributedEventPublisher EventPublisher { get; private set; }

        public int ErrorCount 
        {
            get
            {
                return errorCount;
            }
        }

        public void Error()
        {
            Interlocked.Increment(ref errorCount);
        }
    }

    public class ClearRegisteredConsumer
    {

        public string Topic { get; set; }

        public string Address { get; set; }

        public ClearType ClearType { get; set; }
    }

    public enum ClearType
    {
        Topic, Address
    }
}
