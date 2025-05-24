# BoletoNetCore: Informações do Projeto

## 1. Introdução ao BoletoNetCore

O BoletoNetCore é uma biblioteca .NET de código aberto destinada a facilitar a integração de sistemas brasileiros com a rede bancária para a emissão, processamento e manipulação de boletos de cobrança. Ela oferece suporte à geração de boletos, criação de arquivos de remessa nos padrões CNAB (240, 400, 150) e ao processamento de arquivos de retorno bancário. O projeto é uma evolução do Boleto2Net, modernizado para .NET Standard e .NET Core, permitindo uso multiplataforma.

**Principais Funcionalidades:**
*   Geração de Boletos Bancários.
*   Criação de Arquivos de Remessa (CNAB240, CNAB400, CNAB150).
*   Leitura e Processamento de Arquivos de Retorno (CNAB240, CNAB400).
*   Impressão de Boletos (HTML e PDF através de projeto complementar).
*   Suporte a diversos bancos brasileiros.

## 2. Pontos de Extensão

A arquitetura do BoletoNetCore foi projetada para ser extensível, permitindo a adição de novas funcionalidades, principalmente o suporte a novos bancos ou novas carteiras de cobrança para bancos já existentes.

**Como Adicionar um Novo Banco ou Carteira:**
A principal forma de estender o BoletoNetCore é implementando as interfaces bancárias. O processo envolve:
1.  Criar uma nova classe para o banco (ex: `BancoNovoSA.cs`).
2.  Fazer com que esta classe implemente a interface `IBanco` e as interfaces CNAB relevantes (ex: `IBancoCNAB400`, `IBancoCNAB240`).
3.  Implementar todos os métodos definidos pelas interfaces, que incluem:
    *   Formatação de Nosso Número.
    *   Formatação do Código de Barras e Linha Digitável.
    *   Validação específica do boleto para o banco.
    *   Geração dos registros de Header, Detalhe e Trailer para arquivos de remessa.
    *   Leitura dos registros de Header, Detalhe e Trailer de arquivos de retorno.
4.  Adicionar a lógica de instanciação do novo banco na classe `Bancos` (método `Instancia` ou `Fabrica`).
5.  Criar testes unitários para validar a implementação, incluindo geração de boleto, arquivo remessa e leitura de arquivo retorno.

O arquivo `Como Implementar um novo Banco ou Carteira.txt` (localizado na raiz do projeto `BoletoNetCore`) contém instruções mais detalhadas sobre este processo.

## 3. Principais Interfaces

As interfaces são a espinha dorsal da extensibilidade e flexibilidade do BoletoNetCore.

*   **`IBanco`**:
    *   **Definição:** `BoletoNetCore/Banco/IBanco.cs`
    *   **Descrição:** É a interface fundamental que todas as classes de banco devem implementar. Define o contrato básico para um banco, incluindo propriedades como `Codigo`, `Nome`, `Beneficiario` e métodos para formatação (`FormataNossoNumero`, `FormataCodigoBarraCampoLivre`), validação (`ValidaBoleto`) e geração de partes genéricas de arquivos de remessa.

*   **`IBancoCNAB240`**:
    *   **Definição:** `BoletoNetCore/Banco/IBanco.cs`
    *   **Descrição:** Estende `IBanco`. Adiciona métodos específicos para a geração e leitura de arquivos no formato CNAB240.
        *   `GerarHeaderRemessaCNAB240`, `GerarHeaderLoteRemessaCNAB240`, `GerarDetalheRemessaCNAB240`, `GerarTrailerLoteRemessaCNAB240`, `GerarTrailerRemessaCNAB240`.
        *   `LerHeaderRetornoCNAB240`, `LerDetalheRetornoCNAB240SegmentoT`, `LerDetalheRetornoCNAB240SegmentoU`, `LerDetalheRetornoCNAB240SegmentoA`.

*   **`IBancoCNAB400`**:
    *   **Definição:** `BoletoNetCore/Banco/IBanco.cs`
    *   **Descrição:** Estende `IBanco`. Adiciona métodos específicos para a geração e leitura de arquivos no formato CNAB400.
        *   `GerarHeaderRemessaCNAB400`, `GerarDetalheRemessaCNAB400`, `GerarTrailerRemessaCNAB400`.
        *   `LerHeaderRetornoCNAB400`, `LerDetalheRetornoCNAB400Segmento1`, `LerDetalheRetornoCNAB400Segmento7`.

*   **`IBancoCNAB150`**:
    *   **Definição:** `BoletoNetCore/Banco/IBanco.cs`
    *   **Descrição:** Estende `IBanco`. Adiciona métodos específicos para a geração e leitura de arquivos no formato CNAB150 (menos comum, mas suportado por alguns bancos para finalidades específicas).

*   **`ICarteira`**:
    *   **Definição:** `BoletoNetCore/Banco/ICarteira.cs`
    *   **Descrição:** Embora a listagem inicial não tenha focado neste arquivo, carteiras de cobrança são um conceito fundamental. Esta interface (ou classes abstratas relacionadas) define como diferentes tipos de carteiras (Simples, Registrada, Descontada, etc.) devem se comportar em relação à formatação de dados específicos do boleto para aquela carteira. As implementações de `IBanco` geralmente trabalham em conjunto com implementações de `ICarteira`.

## 4. Instruções Resumidas para Implementar Novos Bancos

Conforme mencionado no item "Pontos de Extensão", a adição de um novo banco requer a criação de uma classe que implemente `IBanco` e as interfaces CNAB relevantes.

**Passos Chave:**
1.  **Criação da Classe do Banco:**
    *   Herde de `BancoFebraban` (se aplicável, para reutilizar lógicas comuns) ou implemente `IBanco` diretamente.
    *   Implemente `IBancoCNAB240` e/ou `IBancoCNAB400` conforme os formatos suportados pelo banco.
2.  **Implementação dos Métodos da Interface:**
    *   **Validações:** `ValidaBoleto(Boleto boleto)` - Verifique regras específicas do banco.
    *   **Formatação:** `FormataNossoNumero(Boleto boleto)`, `FormataCodigoBarraCampoLivre(Boleto boleto)`.
    *   **Remessa (CNAB específico):** Implemente todos os métodos `GerarHeader...`, `GerarDetalhe...`, `GerarTrailer...`. Preste atenção meticulosa aos layouts posicionais definidos pelo manual do banco.
    *   **Retorno (CNAB específico):** Implemente todos os métodos `LerHeader...`, `LerDetalhe...`. Interprete cada campo posicional conforme o manual.
3.  **Registro do Banco:**
    *   Adicione uma entrada para seu novo banco na classe `Bancos` (geralmente no método estático `Instancia` ou `Fabrica`) para que ele possa ser instanciado pela biblioteca.
4.  **Testes:**
    *   Crie testes unitários abrangentes em `BoletoNetCore.Testes`. Teste a geração de boletos (linha digitável, código de barras), a criação de arquivos de remessa completos e a leitura de arquivos de retorno de exemplo.

**Recurso Principal:** Consulte o arquivo `BoletoNetCore/Como Implementar um novo Banco ou Carteira.txt` para um guia mais aprofundado.

## 5. Exemplos de Uso da Biblioteca

```csharp
using BoletoNetCore;
using System;
using System.IO;
using System.Linq;

public class ExemploBoletoNetCore
{
    private IBanco _banco;
    private Beneficiario _beneficiario;
    private Pagador _pagador;

    public ExemploBoletoNetCore()
    {
        // Configuração inicial (exemplo com Banco do Brasil)
        _banco = Banco.Instancia(Bancos.BancoDoBrasil);
        _beneficiario = new Beneficiario
        {
            CPFCNPJ = "123.456.789/0001-00",
            Nome = "Empresa Beneficiária LTDA",
            ContaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "5",
                Conta = "12345",
                DigitoConta = "6",
                CarteiraPadrao = "17", // Carteira de exemplo
                VariacaoCarteiraPadrao = "019",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa,
                Operacao = "123" // Código do convênio/contrato
            },
            Endereco = new Endereco
            {
                LogradouroEndereco = "Rua Exemplo",
                NumeroEndereco = "123",
                BairroEndereco = "Centro",
                CidadeEndereco = "Cidade Exemplo",
                UFEndereco = "EX",
                CEPEndereco = "12345-678"
            }
        };
        _banco.Beneficiario = _beneficiario;

        _pagador = new Pagador
        {
            CPFCNPJ = "987.654.321-00",
            Nome = "Cliente Pagador",
            Endereco = new Endereco
            {
                LogradouroEndereco = "Av. Principal",
                NumeroEndereco = "456",
                BairroEndereco = "Bairro Modelo",
                CidadeEndereco = "Cidade Pagadora",
                UFEndereco = "PX",
                CEPEndereco = "98765-432"
            }
        };
    }

    public Boleto GerarBoletoSimples()
    {
        var boleto = new Boleto(_banco)
        {
            Pagador = _pagador,
            DataEmissao = DateTime.Now,
            DataProcessamento = DateTime.Now,
            DataVencimento = DateTime.Now.AddDays(15),
            ValorTitulo = 150.75m,
            NumeroDocumento = "DOC001", // Número interno do seu sistema
            EspecieDocumento = TipoEspecieDocumento.DM, // Duplicata Mercantil
            Aceite = "N",
            CodigoInstrucao1 = "00", // Outras Instruções
            ComplementoInstrucao1 = "Não receber após 30 dias do vencimento.",
            NossoNumero = "1234567890" // Nosso número (pode ser gerado pelo banco ou informado)
        };

        boleto.ValidarDados(); // Valida e formata campos como Nosso Número, Linha Digitável, Código de Barras
        
        Console.WriteLine($"Nosso Número: {boleto.NossoNumeroFormatado}");
        Console.WriteLine($"Linha Digitável: {boleto.CodigoBarra.LinhaDigitavel}");
        Console.WriteLine($"Código de Barras: {boleto.CodigoBarra.CodigoDeBarras}");

        return boleto;
    }

    public void GerarArquivoRemessa(Boletos boletos, int numeroArquivoRemessa)
    {
        var arquivoRemessa = new ArquivoRemessa(_banco, TipoArquivo.CNAB240, numeroArquivoRemessa);
        using (var stream = new MemoryStream())
        {
            arquivoRemessa.GerarArquivoRemessa(boletos, stream);
            // Salvar o stream em um arquivo:
            // File.WriteAllBytes($"C:\temp\REMESSA_{numeroArquivoRemessa}.REM", stream.ToArray());
            Console.WriteLine($"Arquivo de remessa gerado (em memória). Tamanho: {stream.Length} bytes.");
        }
    }

    public void LerArquivoRetorno(string caminhoArquivoRetorno)
    {
        if (!File.Exists(caminhoArquivoRetorno))
        {
            Console.WriteLine($"Arquivo de retorno não encontrado: {caminhoArquivoRetorno}");
            return;
        }

        using (var stream = File.OpenRead(caminhoArquivoRetorno))
        {
            var arquivoRetorno = new ArquivoRetorno(_banco, TipoArquivo.CNAB240); // Ou TipoArquivo.CNAB400
            // Para detecção automática do banco e tipo de arquivo (se não souber de antemão):
            // var arquivoRetorno = new ArquivoRetorno(stream); 
            
            arquivoRetorno.LerArquivoRetorno(stream);

            foreach (var boletoRetornado in arquivoRetorno.Boletos)
            {
                Console.WriteLine($"------------------------------------");
                Console.WriteLine($"Nosso Número: {boletoRetornado.NossoNumero}");
                Console.WriteLine($"Número Documento: {boletoRetornado.NumeroDocumento}");
                Console.WriteLine($"Valor Título: {boletoRetornado.ValorTitulo}");
                Console.WriteLine($"Valor Pago: {boletoRetornado.ValorPago}");
                Console.WriteLine($"Data Pagamento: {boletoRetornado.DataPagamento}");
                Console.WriteLine($"Data Crédito: {boletoRetornado.DataCredito}");
                Console.WriteLine($"Código Movimento: {boletoRetornado.CodigoMovimentoRetorno}");
                Console.WriteLine($"Descrição Movimento: {boletoRetornado.DescricaoMovimentoRetorno}");
                if (!string.IsNullOrEmpty(boletoRetornado.CodigoMotivoOcorrencia))
                {
                    Console.WriteLine($"Motivos Ocorrência: {boletoRetornado.DescricaoMotivoOcorrencia}");
                }
            }
        }
    }

    public static void Main(string[] args)
    {
        var exemplo = new ExemploBoletoNetCore();
        
        // 1. Gerar um boleto
        var boleto1 = exemplo.GerarBoletoSimples();
        // (Opcional) Gerar PDF/HTML do boleto1 aqui usando BoletoNetCore.Pdf ou BoletoBancario()

        // 2. Gerar arquivo de remessa
        var listaBoletos = new Boletos { boleto1 };
        // Adicionar mais boletos à listaBoletos se necessário
        exemplo.GerarArquivoRemessa(listaBoletos, 1); // Número sequencial do arquivo

        // 3. Ler arquivo de retorno (exemplo)
        // Suponha que você tenha um arquivo RETORNO.RET em C:	emp        // exemplo.LerArquivoRetorno("C:\temp\RETORNO.RET");
    }
}
```

## 6. Sugestões de Melhoria para o Projeto BoletoNetCore

1.  **Documentação Oficial Mais Abrangente:**
    *   Embora existam arquivos como `README.md` e `Como Implementar...txt`, uma documentação online mais robusta (como um site gerado com DocFX ou similar) com tutoriais detalhados, referências de API completas e mais exemplos de código facilitaria a adoção e contribuição.
    *   Detalhar o processo de homologação de novas carteiras.

2.  **Suporte a PIX no Boleto:**
    *   Com a popularização do PIX, integrar a geração de QR Codes PIX (estático ou dinâmico referenciando o boleto) diretamente no corpo do boleto seria uma grande adição, seguindo as normativas do Banco Central.

3.  **Modernização da Geração de PDF:**
    *   Avaliar alternativas mais modernas ou flexíveis ao Rotativa/wkhtmltopdf (se ainda for o principal) para a geração de PDF, buscando melhor performance, menor dependência de binários externos ou mais opções de customização. Bibliotecas como QuestPDF (já presente em `BoletoNetCore.QuestPdf`) são um bom caminho. Unificar e padronizar essa abordagem.

4.  **APIs Assíncronas (`async`/`await`):**
    *   Para operações que podem envolver I/O (como leitura/escrita de arquivos, ou futuras integrações com APIs bancárias online), prover uma API totalmente assíncrona pode melhorar a escalabilidade de aplicações que utilizam a biblioteca.

5.  **Validações Mais Ricas e Detalhadas:**
    *   Expandir as validações para cobrir mais cenários específicos de cada banco e retornar mensagens de erro mais descritivas, talvez com códigos de erro padronizados.

6.  **Interface Fluent para Construção de Boletos:**
    *   Considerar a adição de uma API Fluent para a criação de boletos, o que pode tornar o código do usuário mais legível e expressivo. Ex: `Boleto.Novo(_banco).ComPagador(...).ComVencimento(...).ComValor(...);`

7.  **Testes de Integração Contínua Mais Abrangentes:**
    *   Expandir a suíte de testes para cobrir mais bancos e carteiras, e garantir que os arquivos de remessa/retorno gerados/lidos sejam compatíveis com softwares validadores dos bancos (se disponíveis).

8.  **Logging:**
    *   Integrar com alguma abstração de logging popular (ex: `Microsoft.Extensions.Logging`) para permitir que os usuários da biblioteca capturem logs detalhados sobre o processamento interno, útil para diagnóstico.

9.  **Facilitar a Gestão de "Nosso Número":**
    *   Prover mais utilitários ou estratégias para o gerenciamento do "Nosso Número", especialmente para bancos que exigem sequenciais ou cálculos específicos que podem ser complexos para o usuário final gerenciar.

10. **Suporte a Outros Tipos de Pagamento/Arrecadação:**
    *   Considerar a longo prazo a extensão para outros tipos de documentos de arrecadação ou pagamentos que utilizam formatos CNAB, se houver demanda (ex: Débito Direto Autorizado - DDA).
