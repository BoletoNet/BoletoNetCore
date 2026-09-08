# CpfCnpjComBr.BoletoNetCore

Autopreenchimento de `Pagador`, `Beneficiario` e `Endereco` do
[BoletoNetCore](https://github.com/BoletoNet/BoletoNetCore) a partir de um CPF ou
CNPJ, usando a API [cpfcnpj.com.br](https://www.cpfcnpj.com.br/dev/).

Pacote **aditivo**: nao altera o core do BoletoNetCore, apenas o referencia. Alvo
`netstandard2.0`, compativel com .NET Framework, .NET Core e .NET 5 ou superior.

## Instalacao

```
dotnet add package BoletoNetCore
dotnet add package CpfCnpjComBr.BoletoNetCore
```

## Uso

```csharp
using BoletoNetCore;
using CpfCnpjComBr.BoletoNetCore;

// Reaproveite o HttpClient (idealmente via IHttpClientFactory).
var lookup = new CpfCnpjComBrLookup(httpClient, new CpfCnpjComBrOptions
{
    Token = "SEU_TOKEN"
});

var resolver = new PagadorResolver(lookup);

// Devolve um Pagador nativo ja preenchido:
Pagador pagador = await resolver.ResolverPagadorAsync("27.272.134/0001-18");

// Ou preencha um Pagador existente sem sobrescrever o que ja foi informado:
var outro = new Pagador { Nome = "Nome ja definido" };
await outro.PreencherAsync(lookup, "390.533.447-05", sobrescrever: false);
```

## Mapeamento

| Campo da API            | Pacote | Destino nativo                  |
|-------------------------|--------|---------------------------------|
| nome / razao            | 3 / 5  | `Pagador.Nome`                  |
| CPF / CNPJ              | 3 / 5  | `Pagador.CPFCNPJ`               |
| endereco / logradouro   | 3 / 5  | `Endereco.LogradouroEndereco`   |
| numero                  | 3 / 5  | `Endereco.LogradouroNumero`     |
| complemento             | 3 / 5  | `Endereco.LogradouroComplemento`|
| bairro                  | 3 / 5  | `Endereco.Bairro`               |
| cidade                  | 3 / 5  | `Endereco.Cidade`               |
| uf                      | 3 / 5  | `Endereco.UF`                   |
| cep                     | 3 / 5  | `Endereco.CEP`                  |

Para CNPJ, o tipo de logradouro (por exemplo `R` ou `Avenida`) e concatenado ao
logradouro respeitando abreviacoes, sem duplicar o prefixo.

## Validacao

Antes de atribuir ao setter de `CPFCNPJ` (que exige 11 ou 14 posicoes), o documento
e normalizado e validado pela nossa regra, incluindo **CNPJ alfanumerico** (12
posicoes base alfanumericas mais 2 digitos verificadores numericos). Documentos
invalidos resultam em `ArgumentException` antes de qualquer chamada de rede.

## Licenca

MIT, acompanhando o BoletoNetCore.
