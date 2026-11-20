using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CpfCnpjComBr.BoletoNetCore.Testes
{
    /// <summary>
    /// Handler HTTP de teste: devolve um corpo fixo e captura a ultima URL chamada,
    /// permitindo validar o cliente sem tocar a rede.
    /// </summary>
    internal sealed class FakeHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _corpo;

        public Uri UltimaUrl { get; private set; }

        public FakeHandler(string corpo, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _corpo = corpo;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            UltimaUrl = request.RequestUri;
            var resposta = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_corpo),
            };
            return Task.FromResult(resposta);
        }
    }
}
