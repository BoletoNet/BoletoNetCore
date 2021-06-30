using System;
using System.Net.Http;
using System.Threading;

namespace BoletoNetCore
{
    internal class BancoOnline<T> where T : IBancoOnlineRest
    {
        private HttpClient httpClient;
        private T bancoOnline;

        public BancoOnline(T bancoOnline)
        {
            this.bancoOnline = bancoOnline;
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(this.bancoOnline.UrlApi);
        }
    }
}