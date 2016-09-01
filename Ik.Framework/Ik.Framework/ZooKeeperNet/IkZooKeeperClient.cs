using IkZooKeeperNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.ZooKeeperNet
{
    public class IkZooKeeperClient:IDisposable
    {
        private IZooKeeper _client = null;
        private string _address = "";
        private TimeSpan _sessionTimeOut;
        private IWatcher _watcher;
        public IkZooKeeperClient(string address, TimeSpan sessionTimeOut, IWatcher watcher)
        {
            this._client = new ZooKeeper(address, sessionTimeOut, watcher);
            this._address = address;
            this._sessionTimeOut = sessionTimeOut;
            this._watcher = watcher;
        }

        private IZooKeeper EnsureAvailableZooKeeperClient()
        {
            if (!this._client.State.IsAlive())
            {
                this._client = new ZooKeeper(_address, _sessionTimeOut, _watcher, this._client.SessionId, this._client.SesionPassword);
            }
            return this._client;
        }

        public bool EnsureExists(string path, bool isWatch, int retry = 2)
        {
            int index = 0;
            Exception lastEx = null;
            while (index <= retry)
            {
                try
                {
                    var client = EnsureAvailableZooKeeperClient();
                    var state = client.Exists(path, isWatch);
                    return state == null ? false : true;
                }
                catch (IkZooKeeperNet.KeeperException.ConnectionLossException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionExpiredException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionMovedException ex)
                {
                    lastEx = ex;
                }
                index++;
            }
            if (index > retry)
            {
                throw new ZookeepExtException(string.Format("检测节点状态失败：{0}", path), lastEx);
            }
            return false;
        }

        public  bool EnsureSetData(string path, byte[] data, int retry = 2)
        {
            int index = 0;
            Exception lastEx = null;
            while (index <= retry)
            {
                try
                {
                    var client = EnsureAvailableZooKeeperClient();
                    var state = client.SetData(path, data, -1);
                    return state == null ? false : true;
                }
                catch (IkZooKeeperNet.KeeperException.ConnectionLossException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionExpiredException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionMovedException ex)
                {
                    lastEx = ex;
                }
                index++;
            }
            if (index > 2)
            {
                throw new ZookeepExtException(string.Format("设置节点数据失败：{0}", path));
            }
            return false;
        }

        public byte[] EnsureGetData( string path, int retry = 2)
        {
            int index = 0;
            Exception lastEx = null;
            while (index <= retry)
            {
                try
                {
                    var client = EnsureAvailableZooKeeperClient();
                    return client.GetData(path, null, null);
                }
                catch (IkZooKeeperNet.KeeperException.ConnectionLossException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionExpiredException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionMovedException ex)
                {
                    lastEx = ex;
                }
                index++;
            }
            if (index > retry)
            {
                throw new ZookeepExtException(string.Format("获取节点数据失败：{0}", path));
            }
            return null;
        }

        public void EnsureCreate(string path, int retry = 2)
        {
            int index = 0;
            Exception lastEx = null;
            while (index <= retry)
            {
                try
                {
                    var client = EnsureAvailableZooKeeperClient();
                    client.Create(path, null, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                    return;
                }
                catch (IkZooKeeperNet.KeeperException.ConnectionLossException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionExpiredException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionMovedException ex)
                {
                    lastEx = ex;
                }
                index++;
            }
            if (index > retry)
            {
                throw new ZookeepExtException(string.Format("节点创建失败：{0}", path));
            }
        }

        public bool EnsureDelete( string path, int retry = 2)
        {
            int index = 0;
            Exception lastEx = null;
            while (index <= retry)
            {
                try
                {
                    var client = EnsureAvailableZooKeeperClient();
                    client.Delete(path, -1);
                    return true;
                }
                catch (IkZooKeeperNet.KeeperException.ConnectionLossException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionExpiredException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionMovedException ex)
                {
                    lastEx = ex;
                }
                index++;
            }
            if (index > retry)
            {
                throw new ZookeepExtException(string.Format("节点删除失败：{0}", path));
            }
            return false;
        }

        public IEnumerable<string> EnsureGetChildren(string path, bool isWatch, int retry = 2)
        {
            int index = 0;
            Exception lastEx = null;
            while (index < 2)
            {
                try
                {
                    var client = EnsureAvailableZooKeeperClient();
                    return client.GetChildren(path, false);
                }
                catch (IkZooKeeperNet.KeeperException.ConnectionLossException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionExpiredException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.SessionMovedException ex)
                {
                    lastEx = ex;
                }
                catch (IkZooKeeperNet.KeeperException.NoNodeException ex)
                {
                    lastEx = ex;
                }
                index++;
            }
            throw new ZookeepExtException(string.Format("检测节点状态失败：{0}", path));
        }

        public void Dispose()
        {
            this._client.Dispose();
        }
    }

    public class ZookeepExtException : InkeyException
    {
        public ZookeepExtException(string desc)
            : base(InkeyErrorCodes.CommonFailure, desc)
        {

        }

        public ZookeepExtException(string desc, Exception ex)
            : base(InkeyErrorCodes.CommonFailure, desc, ex)
        {

        }
    }
}
