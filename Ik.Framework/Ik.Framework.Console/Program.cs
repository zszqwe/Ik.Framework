using Ik.Framework.Logging;
//using Inkey.Framework.BufferService;
//using Inkey.Framework.Configuration;
//using Inkey.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ik.Framework.Common.Extension;
using System.Configuration;
using Ik.Framework.Caching;
using Ik.Framework.Common.Serialization;
using Ik.Framework.Events;
using Ik.Service.Auth;
using Ik.Framework.Configuration;
using Ik.Framework.Caching.CacheKeyManager;
using Ik.Framework.DataAccess.DataMapping;
using Ik.Framework.SerialNumber;
using StackExchange.Redis;
using Ik.Framework.Redis;
using Ik.Framework.DataAccess;

namespace Ik.Framework.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigManager.RemoteConfigService.ServiceException += ServiceException;
            ConfigManager.RemoteConfigService.Start();
            var rdString = ConfigManager.Instance.Get<string>("redis_connection");
            var size = ConfigManager.Instance.Get<int>("default_page_size");
            var ct = ConfigManager.Instance.Get<SerialNumberCreateType>("default_sn_cr_type");
            var ds = ConfigManager.Instance.Get<DataSourcesConfig>("dataSourcesDefines");
            var dt = ConfigManager.Instance.Get<DateTime>("default_datetime");
            ConfigManager.RemoteConfigService.Stop();
            //13fe0162a4754954:Zp2015Zp2015@13fe0162a4754954.m.cnhza.kvstore.aliyuncs.com:6379
            //string connstring = "172.16.0.151:6379,password=inkey.123";
            //string connstring = "13fe0162a4754954.m.cnhza.kvstore.aliyuncs.com:6379,password=13fe0162a4754954:Zp2015Zp2015";
            //System.Console.WriteLine(connstring);
            //RedisManager.DefaultConnectionString = connstring;
            //RedisManager cache = new RedisManager();
            //string key1 = "api_keyTest";
            //cache.Remove(key1);
            //cache.RawAdd(key1, new MyUserInfo { UserId = 10, UserName = "name1" });

            //cache.Add(key1, "test");
            //string vstring = cache.Get<string>(key1);
            //System.Console.WriteLine(vstring);
            //cache.Remove(key1);
            //cache.Set(key1, new MyUserInfo { UserId = 1, UserName = "test1" });
            //MyUserInfo vmy = cache.Get<MyUserInfo>(key1);
            //cache.Remove(key1);
            //cache.Set(key1, new List<MyUserInfo> { new MyUserInfo { UserId = 1, UserName = "test1" } });
            //List<MyUserInfo> vmylist = cache.Get<List<MyUserInfo>>(key1);

            //cache.Remove(key1);
            //string vstring = cache.Get<string>(key1, TimeSpan.FromSeconds(5), () => {
            //    return "test";
            //});

            //vstring = cache.Get<string>(key1);
            //cache.Remove(key1);
            //cache.SetCreateAdd<string>(key1, new List<string> { "test1", "test2" });
            //cache.SetAdd<string>(key1, new List<string> { "test2", "test3" });

            //cache.Remove(key1);
            //string v;
            //bool flag1 = cache.IsAdd<string>(key1,out v);
            //cache.Add(key1, "test1");
            //bool flag2 = cache.IsAdd<string>(key1, out v);
           // System.Console.WriteLine("--------------");
           // cache.Remove(key1);
           // cache.ListCreateAdd<string>(key1, new List<string> { "test1", "test2" });
           // cache.ListPushAdd<string>(key1, new List<string> { "test1", "test2" });
           // var list = cache.ListGet<string>(key1);
           // foreach (var item in list)
           // {
           //     System.Console.WriteLine(item);
           // }
           // System.Console.WriteLine("--------------");
           // cache.Remove(key1);
           // cache.HashCreateAdd<string, string>(key1, new List<HashValue<string, string>> { new HashValue<string, string>("key1", "value1"), new HashValue<string, string>("key2", "value2") });
           // cache.HashAdd<string, string>(key1, new HashValue<string, string>("key3", "value3"));

           //var hashlist =  cache.HashAllGet(key1);
           //foreach (var item in hashlist)
           //{
           //    System.Console.WriteLine(string.Format("key:{0},value:{1}", item.Key.GetKey<string>(),item.Value.GetValue<string>()));
           //}
            //var flag = db.StringSet("key1", "test");
            //if (flag)
            //{
            //    var v = (string)db.StringGet("key1");
            //}
           //db.ListLeftPush("key_list1", "value1");
           //db.ListLeftPush("key_list1", "value2");
           //var listv = (string)db.ListGetByIndex("key_list1", 0);
           //var listv = (string)  db.ListLeftPop("key_list1");
            //db.KeyDelete("key_list1");
            //var d = new string[] { "value1", "value2" }.Select(v => (RedisValue)v).ToArray();
            //db.ListRightPush("key_list1", d);
           System.Console.ReadLine();
            //var factory = new DataAccessFactory("OperationSupport");
            //var ser = new SerialNumberService("yihuo_Information_code", SerialNumberCreateType.DataBase, factory);
            //var number = ser.GetSerialNumber(30);

            //var a = ConfigManager.Instance.Get<int?>("aa");

            //var k = DefaultKeyFormat.Instance;

            //var t = k.GetKeyDefineType("key1");



            //LogManager.Initialize();

            //var logger = LogManager.GetLogger();

            //logger.Debug("Debug");
            //logger.Error("Error");
            //logger.Info("Info");
            //logger.Warn("Warn");
            
            //SubscriptionManager.DistributedEventService.ServiceException += DistributedEventService_ServiceException;
            //SubscriptionManager.DistributedEventService.Start();

            //SubscriptionManager.SubscriberContextInitialize();

            //IkAuthorizationContext.Initialize();
            //IkAuthorizationContext.Current.UpdateNotificationAll();

            //MemcachedConfig c = new MemcachedConfig();
            //c.MaxPoolSize = 200;
            //c.MinPoolSize = 2;
            //c.Servers = new List<MemcachedServer> { new MemcachedServer { Address = "127.0.0.1", Port = 1211 }, new MemcachedServer { Address = "128.0.0.1", Port = 1211 } };
            //c.AuthParameters = new List<AuthenticationParameter> { new AuthenticationParameter { Key = "user", Value = "sss" }, new AuthenticationParameter { Key = "user1", Value = "sss1" } };

            //var v = XmlSerializerManager.XmlSerializer(c);

            //MemcachedManager m = new MemcachedManager();

           // var f1 = m.RawSet("api_keyTest", new List<MyUserInfo> { new MyUserInfo { UserId = 1, UserName = "Test1" }, new MyUserInfo { UserId = 2, UserName = "Test2" } });
            //List<MyUserInfo> v;
            //var f = m.RawIsSet<List<MyUserInfo>>("api_keyTest", out v);
            //var c = m.Get<string>("api_keyTest");
            //if (string.IsNullOrEmpty(c))
            //{
            //    m.Set("api_keyTest", "test");
            //}
            //c = m.Get<string>("api_keyTest");
            //System.Console.WriteLine(c);
            
            //LogConfig config = new LogConfig();
            //config.DefaultLevel = LogLevel.ERROR | LogLevel.INFO | LogLevel.WARN;
            //config.DefaultWorkType = LogWorkType.Local;
            //config.Rules = new List<LogDispatchRule>();
            //config.Rules.Add(new LogDispatchRule { AppName = "配置中心项目", WorkType = LogWorkType.Buffer, Level = LogLevel.DEBUG| LogLevel.ERROR| LogLevel.INFO| LogLevel.WARN });

            //string s = config.ToJsonString();

            //var data = ConfigurationManager.GetSection("dataSources");

            //string s2 = data.ToJsonString();

            //BufferServiceManager.AddBufferProcessorService(new List<BufferProcessorService> { new LogToDataBaseProcessorService() });
            //BufferServiceManager.DispatcherService.ServiceException += ServiceException;
            //BufferServiceManager.DispatcherService.Start();
            //ConfigManager.RemoteConfigService.ServiceException += ServiceException;
            //ConfigManager.RemoteConfigService.Start();
            //LogManager.Init();
            //var logger = LogManager.GetLogger("测试模块");
            //for (int i = 0; i < 10; i++)
            //{
            //    logger.Debug("日志测试标题" + i, 100, "日志测试内容");
            //}
            //System.Console.ReadLine();
            //BufferServiceManager.DispatcherService.Stop();
            //ConfigManager.RemoteConfigService.Stop();
        }

        private static void ServiceException(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        static void DistributedEventService_ServiceException(object sender, ErrorEventArgs e)
        {
            System.Console.WriteLine(e.Exception.ToString());
        }

    }

    [Serializable]
    public class MyUserInfo
    {
        public string UserName { get; set; }

        public int UserId { get; set; }
    }
}

#region copyright
/*
*.NET基础开发框架
*Copyright (C) 。。。
*地址：git@github.com:gangzaicd/Ik.Framework.git
*作者：到大叔碗里来（大叔）
*QQ：397754531
*eMail：gangzaicd@163.com
*/
#endregion copyright
