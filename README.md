[![Build status](https://ci.appveyor.com/api/projects/status/fv9cin5fmpaqri7o?svg=true)](https://ci.appveyor.com/project/carloscds/boletonetcore)
[![Nuget count](http://img.shields.io/nuget/v/BoletoNetCore.svg)](https://www.nuget.org/packages/BoletoNetCore/)
[![Nuget count](http://img.shields.io/nuget/v/BoletoNetCore.PDF.svg)](https://www.nuget.org/packages/BoletoNetCore.PDF/)
[![Issues open](https://img.shields.io/github/issues/BoletoNet/boletonetCore.svg)](https://huboard.com/BoletoNet/boletonetcore/)
[![Coverage Status](https://coveralls.io/repos/github/BoletoNet/boletonetcore/badge.svg?branch=master)](https://coveralls.io/github/BoletoNet/boletonetcore?branch=master)
[![MyGet Ultimo PR](https://img.shields.io/myget/boletonetcorebuild/v/boletonetcore.svg)](https://www.myget.org/gallery/boletonetcorebuild)

# Status
![Alt](https://repobeats.axiom.co/api/embed/624e926e9d7b272a2137660ee27c9575d5aec3ac.svg "Repobeats analytics image")

# Projeto boleto .NET Core
Esta é uma versão do Boleto2Net, adaptada para funcionar com .NET Core. Para evitar a quebra de compatibilidade com aplicações que utilizam versões do .NET inferiores à 4.6.1, optamos por criar um novo projeto.

**O que é o projeto boleto .net core?**
R: [Live sobre o entendimento do projeto](https://www.youtube.com/watch?v=ci8u2c3awI8&t=2728s)

#### Carteiras Homologadas

| Banco | Código do banco | Carteira |
| :-------- | :------- | :-----------|
| Banrisul | 041 | 1  |
| Bradesco | 237 | 09 |
| Bradesco | 237 | 09 |
| Brasil   | 001 | 17 ` (Variações 019 027 035)`|
| Caixa Econômica Federal | 104 |SIG14|
| Cecred/Ailos|085|1|
|Itau|341|109, 112|
|Inter|077|110|
|Safra|422|1|
|Santander|033|101|
|Sicoob|756|1-01|
|Sicredi|748|1-A|
|Uniprime Norte PR|084|09|

#### Carteiras Implementadas (Não foi homologada. Falta teste unitário)

| Banco | Código do banco | Carteira |
| :-------- | :------- | :-----------|
|Banco do Brasil| 001 |11 `(Variação 019)`|
|Banco CrediSIS|097|18|
|Nordeste|004|1|
> Atenção: Para manter a ordem do projeto, qualquer solicitação de Pull Request de um novo banco ou carteira implementada, deverá seguir o formato dos bancos/carteiras já implementados e vir acompanhado de teste unitário da geração do boleto (PDF), arquivo remessa e geração de 9 boletos, com dígitos da linha digitável variando de 1 a 9, checando além do próprio dígito verificador, o cálculo do nosso número, linha digitável e código de barras.

### Pre requisitos
* Visual Studio 2017 ou superior
* .NET Framework 4.6.1 ou superior

## Como Contribuir

[Leia o arquivo contributing.md](contributing.md)

Este projeto está dividido em 3 partes: 

### BoletoNetCore (Projeto Principal)
Responsável por guardar toda a lógica de leitura de remessa e retorno de arquivos e regras e impressão do boleto em hipertexto. Por ser um projeto **multitarget**, todo o código será avaliado se puder rodar corretamente tanto em netstandard2 quanto em net40. 

### BoletoNetCore.Pdf
Responsável pelos serviços de  impessão em PDF. 

Obs.: Para geração de PDF em linux (ambientes baseados em Debian), é necessário a instalação das seguintes dependências:

```bash
apt-get install -y libfontconfig1 libxrender1 libxext6
```

Também é necessário garantir a permissão de execução para o binário responsável por gerar o PDF:

```bash
chmod +x "<Caminho do projeto>/BoletoNetCore.Testes/bin/Debug/net7.0/Rotativa/Linux/wkhtmltopdf"
```

### BoletoNetCore.Testes
Validação e testes de toda a lógica dos boletos.

- Em linhas gerais, novas carteiras deverão passar por validações e apresentar comprovação de passe nos testes propostos, contendo validações conforme proposto acima.
- Procure comentar todo o código para facilitar o entendimento e motivação para outros colegas. Embora o código original não seja muito comentado, não é motivo para que se crie o hábito. 
- Se houver a necessidade de incluir novas imagens ou recursos para impressão, abra uma issue primeiro, ou apenas use as pastas convencionadas no projeto para receber esses tipos de arquivo. /Images e /BoletoBancario
- Nomenclaturas e termos devem estar alinhados aos padrões definidos no CNAB: <https://cmsportal.febraban.org.br/Arquivos/documentos/PDF/Layout%20padrao%20CNAB240%20%20V%2010%2005%20-%2005_11_18.pdf>
- A Estrutura das classes dos Banco estão distribuídas em arquivos que representam partial classes
- Cada uma das partial classes implementa uma interface diferente que representa um formato, existem 3 formatos implementados:
CNAB400, CNAB240 e OnlineRest (**Procuramos Implementadores**)

## Migrando do Boleto2Net
Este projeto possui algumas diferenças relevantes em relação ao Boleto2Net que podem quebrar o seu código:
- Retorno de Arquivos CNAB geram **CodMovimentoRetorno** no Lugar de **CodOcorrencia**.
- Se você quer usar a impressão em PDF, use o **BoletoNetCorePdfProxy** e não **BoletoNetCoreProxy**.
- Para a impressão em PDF, também é necessário a instalação do pacote [BoletoNetCore.Pdf](https://www.nuget.org/packages/BoletoNetCore.PDF/).
- Este projeto não usa **System.Web** então, não existem componentes manipuláveis para WebForms para o Editor do VS. 
- Cedente e Sacado foram substituidos em todo o projeto pelos termos atuais **Beneficiario** e **Pagador**
