[![Build status](https://ci.appveyor.com/api/projects/status/r9ovipu6yu7numn6?svg=true)](https://ci.appveyor.com/project/carloscds/boleto2net)
[![Nuget count](http://img.shields.io/nuget/v/BoletoNetCore.Net.svg)](http://www.nuget.org/packages/BoletoNetCore.Net/)
[![Issues open](https://img.shields.io/github/issues/BoletoNet/boletonetCore.svg)](https://huboard.com/BoletoNet/boletonetcore/)
[![Coverage Status](https://coveralls.io/repos/github/BoletoNet/boletonetcore/badge.svg?branch=master)](https://coveralls.io/github/BoletoNet/boletonetcore?branch=master)

# BoletoNetCore
Esta é uma versão baseado no Boleto2Net, mas para funcionar com .NET Core

Foi criado um novo projeto para não quebrar a compatibilidade com aplicações que usam .NET inferior ao 4.6.1

### Carteiras Homologadas
* Banrisul (041) - Carteira 1
* Bradesco (237) - Carteira 09
* Brasil (001) - Carteira 17 (Variações 019 027)
* Caixa Econômica Federal (104) - Carteira SIG14
* Itau (341) - Carteira 109, 112
* Safra (422) - Carteira 1
* Santander (033) - Carteira 101
* Sicoob (756) - Carteira 1-01
* Sicreed (748) - Carteira 1-A

### Carteiras Implementadas (Não foi homologada. Falta teste unitário)
* Banco do Brasil (001) - Carteira 11 (Variação 019)

> Atenção: Para manter a ordem do projeto, qualquer solicitação de Pull Request de um novo banco ou carteira implementada, deverá seguir o formato dos bancos/carteiras já implementados e vir acompanhado de teste unitário da geração do boleto (PDF), arquivo remessa e geração de 9 boletos, com dígitos da linha digitável variando de 1 a 9, checando além do próprio dígito verificador, o cálculo do nosso número, linha digitável e código de barras.

### Pre requisitos
Visual Studio 2017 ou superior
.NET Framework 4.6.1 ou superior
