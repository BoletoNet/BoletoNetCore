using BoletoNetCore.WebAPI.Extensions;
using BoletoNetCore.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BoletoNetCore.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoletosController : ControllerBase
    {
        MetodosUteis metodosUteis = new MetodosUteis();

        public BoletosController()
        {
        }


        /// <summary>
        /// Endpoint para retornar o HTML do boleto do banco ITAU.
        /// </summary>
        /// <remarks>
        /// ## Carteiras:
        ///- Banrisul (041) - Carteira 1
        ///- Bradesco (237) - Carteira 09
        ///- Brasil (001) - Carteira 17 (Variações 019 027 035)
        ///- Caixa Econômica Federal (104) - Carteira SIG14
        ///- Cecred/Ailos (085) - Carteira 1
        ///- Itau (341) - Carteira 109, 112
        ///- Safra (422) - Carteira 1
        ///- Santander (033) - Carteira 101
        ///- Sicoob (756) - Carteira 1-01
        ///- Sicredi (748) - Carteira 1-A
        ///
        /// ## Tipo de banco emissor
        /// O tipo de banco deve ser informado dentro do parâmetro para que nossa API possa identificar de que banco se trata
        /// - BancoDoBrasil = 001
        /// - BancoDoNordeste = 004
        /// - Santander = 033
        /// - Banrisul = 041
        /// - UniprimeNortePR = 084
        /// - Cecred = 085
        /// - Caixa = 104
        /// - Bradesco = 237
        /// - Itau = 341
        /// - Safra = 422
        /// - Sicredi = 748
        /// - Sicoob = 756
        /// </remarks>
        /// <returns>Retornar o HTML do boleto.</returns>
        [ProducesResponseType(typeof(DadosBoleto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpPost("GerarBoletos")]
        public IActionResult PostGerarBoletos(DadosBoleto dadosBoleto, int tipoBancoEmissor)
        {

            try
            {
                if(dadosBoleto.BeneficiarioResponse.CPFCNPJ == null || (dadosBoleto.BeneficiarioResponse.CPFCNPJ.Length != 11 && dadosBoleto.BeneficiarioResponse.CPFCNPJ.Length != 14))
                {
                    var retorno = metodosUteis.RetornarErroPersonalizado((int)HttpStatusCode.BadRequest, "Requisição Inválida", "CPF/CNPJ inválido: Utilize 11 dígitos para CPF ou 14 para CNPJ.", "/api/Boletos/BoletoItau");
                    return BadRequest(retorno);
                }

                if (string.IsNullOrWhiteSpace(dadosBoleto.BeneficiarioResponse.ContaBancariaResponse.CarteiraPadrao))
                {
                    var retorno = metodosUteis.RetornarErroPersonalizado((int)HttpStatusCode.BadRequest, "Requisição Inválida", "Favor informar a carteira do banco.", "/api/Boletos/BoletoItau");
                    return BadRequest(retorno);
                }

                GerarBoletoBancos gerarBoletoBancos = new GerarBoletoBancos(Banco.Instancia(metodosUteis.RetornarBancoEmissor(tipoBancoEmissor)));
                var htmlBoleto = gerarBoletoBancos.RetornarHtmlBoleto(dadosBoleto);

                return Content(htmlBoleto, "text/html");
            }
            catch (Exception ex)
            {
                var retorno = metodosUteis.RetornarErroPersonalizado((int)HttpStatusCode.InternalServerError, "Requisição Inválida", $"Detalhe do erro: {ex.Message}", string.Empty);
                return StatusCode(StatusCodes.Status500InternalServerError, retorno);
            }
        }
    }
}
