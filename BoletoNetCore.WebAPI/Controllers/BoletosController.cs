using BoletoNetCore.WebAPI.Extensions;
using BoletoNetCore.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoletoNetCore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoletosController : ControllerBase
    {

        /// <summary>
        /// Endpoint para retornar o HTML do boleto do banco ITAU. **negrito**
        /// </summary>
        /// <remarks>
        /// 
        /// ```csharp
        /// 
        ///     var teste = string.Empty;
        /// 
        /// ```
        /// 
        /// </remarks>
        /// <returns></returns>
        [HttpPost("BoletoItau")]
        public async Task<IActionResult> PostBoletoItau(DadosBoleto dadosBoleto)
        {
            try
            {
                GerarBoletoBancos gerarBoletoBancos = new GerarBoletoBancos(Banco.Instancia(Bancos.Itau));
                var htmlBoleto = gerarBoletoBancos.RetornarHtmlBoleto(dadosBoleto);

                return Content(htmlBoleto, "text/html");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
