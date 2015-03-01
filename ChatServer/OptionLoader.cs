using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Text;


namespace ChatServer
{
    public class OptionLoader
    {
        private NameValueCollection list;

        public OptionLoader()
        {
            list = ConfigurationManager.AppSettings;

            // DONE 自環境情報の取得と設定
            IPAddress[] myIpAddress = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in myIpAddress) 
            {
                if (ip.IsIPv6LinkLocal)
                    continue;

                this.list["localip"] = ip.ToString();
                break;
            }

            // TODO ポート番号を定数から 
            this.list["localport"] = "5555";
        }

        public string this[string key] 
        {
            get 
            {
                if (list.Count == 0)
                    return "";

                return this.list[key];
            }
            set
            {
                this.list[key] = value;   
            }
        }
    }
}
