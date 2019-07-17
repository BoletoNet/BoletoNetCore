Roteiro para migração do Boleto.Net para Boleto2.Net:

1 - Adicione o projeto Boleto2Net à Solution, remova a versão antiga e dê um "clean" na solution. Isso irá mostrar os erros de referência, logo é só substituir pelo Boleto2Net.

2 - Agora baixe os 2 arquivos que deixei em anexo e verifique as diferenças entre eles. Sei que vão estar bem diferentes um do outro, porém a maioria das alterações que fiz foram referente a reestruturação do meu código, e não ao BoletoNet.

Note que as maiores mudanças foram:

Na classe "ArquivoRemessa" - Tanto em sua instanciação quanto no método "GerarArquivoRemessa", que agora recebe apenas 2 parâmetros, pois o restante das informações ele pega do objeto global "_banco", passado ao instanciar "ArquivoRemessa".
Na função "new BoletoBancario().MontaBytesListaBoletosPDF()" - Esta não está mais presente no Boleto2Net, portanto adicionei um substituto para ele, "FuncoesBoleto.GeraPDFBoletos()", que retorna um byte[] com os arquivos PDFs.
OBS.: Você terá que adicionar o pacote nuget "NReco.PdfGenerator" ao projeto para usar essa função.

O restante continua da mesma maneira, só que mais intuitivo.

Veja abaixo a parte referente ao ArquivoRemessa:

## BoletoNet - Versão antiga
```csharp
var rem = new ArquivoRemessa(TipoArquivo.CNAB240);
MemoryStream ms = new MemoryStream();
rem.GerarArquivoRemessa(numeroConvenio, new Banco(codBanco), cedente, _boletos, ms, numeroArqRemessa);
retBoletos.ArquivoRemessa = Encoding.Default.GetString(ms.ToArray());
```

## Boleto2Net - Nova Versão
```csharp
var rem = new ArquivoRemessa(_banco, TipoArquivo.CNAB240, 1); //<== IMPLEMENTAR AQUI NUMERO SEQUENCIAL DO ARQ REMESSA!!!
MemoryStream ms = new MemoryStream();
rem.GerarArquivoRemessa(boletos, ms);
retBoletos.ArquivoRemessa = Encoding.Default.GetString(ms.ToArray()); //<
`````


