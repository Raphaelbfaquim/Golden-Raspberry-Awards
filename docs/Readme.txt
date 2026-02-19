Golden Raspberry Awards - API
=============================

O que o projeto faz
--------------------

Esta API trabalha em cima dos dados do Golden Raspberry Awards (o "Oscar do pior filme").
Ela lê um CSV com os vencedores de cada ano, guarda no banco e expõe um endpoint que responde:
quais produtores tiveram o MENOR intervalo entre duas vitórias seguidas e quais tiveram o MAIOR.

Ou seja: a API descobre quem ganhou duas vezes mais "rápido" e quem demorou mais entre um prêmio e outro.
Tudo isso em cima do arquivo Movielist.csv que já vem no projeto.


Estrutura do projeto
--------------------

Na raiz você encontra:

  - src/GoldenRaspberryAwards.Api    -> a API em si (ASP.NET Core)
  - tests/                           -> testes de integração
  - scripts/                         -> scripts úteis (ex.: dotnet no PATH)
  - docs/                            -> documentação e collection do Postman

Dentro da API (src/GoldenRaspberryAwards.Api):

  - Controllers    -> AuthController (login) e ProducersController (intervalos)
  - Models         -> DTOs e modelos de domínio (ProducerWin, ProducerIntervalResult, LoginRequest, etc.)
  - Services       -> regra de negócio: carregar CSV, calcular intervalos (interfaces + implementações)
  - Data           -> contexto do Entity Framework e banco SQLite
  - Middleware     -> tratamento de exceção e headers de segurança
  - Filters        -> filtro que devolve 400 padronizado quando a validação falha

Ou seja: controllers finos, lógica nos services, persistência no Data, e segurança/erro no pipeline (middleware e filtros).


Endpoints
---------

  1) POST /api/auth/login
     - Envia userName e password no body (JSON).
     - Se estiver certo (configurado no appsettings), devolve um token JWT.
     - Esse token é usado no header "Authorization: Bearer <token>" nos outros endpoints.

  2) GET /api/producers/intervals
     - Precisa estar autenticado (Bearer token).
     - Resposta: quem tem menor e maior intervalo entre duas vitórias (min e max), em JSON.


Segurança
---------

  - Autenticação JWT: só quem faz login e recebe o token acessa o endpoint de produtores.
  - Headers de segurança: em toda resposta a API coloca Strict-Transport-Security, X-Content-Type-Options (nosniff) e X-Frame-Options (DENY) para reduzir riscos no browser.
  - Rate limiting: limite de 100 requisições por minuto por cliente (por IP); quem passar recebe 429 e uma mensagem em JSON.
  - Validação de entrada: o login usa DataAnnotations (Required, StringLength etc.) e um filtro global devolve 400 com lista de erros quando o body está inválido.
  - Tratamento de exceção: um middleware no início do pipeline captura qualquer erro não tratado, devolve 500 com uma mensagem genérica e um traceId (sem mostrar stack na resposta) e registra a exceção completa no log do servidor.

Resumindo: login com JWT, headers de proteção, limite de requisição e validação/erro tratados de forma centralizada.


Métodos e padrões
-----------------

  - Injeção de dependência: os services (ICsvLoaderService, IProducerIntervalService) são registrados como Scoped e injetados nos controllers. O DbContext também.
  - Interfaces para os services: a lógica de carregar CSV e calcular intervalos está atrás de interfaces, o que facilita testes e troca de implementação.
  - Middleware: ExceptionHandlingMiddleware e SecurityHeadersMiddleware rodam no pipeline na ordem certa (exceção primeiro, depois headers, depois auth, Swagger, controllers).
  - Filtro de validação: ValidationResponseFilter roda em toda action e, se o ModelState estiver inválido, retorna 400 com o formato { message, errors } em vez de deixar o comportamento padrão do [ApiController].
  - Configuração por appsettings: connection string, JWT (secret, issuer, audience, expiração) e usuário/senha do login vêm do appsettings.json (em produção use variáveis de ambiente ou User Secrets).


Banco e dados
--------------

  - SQLite, arquivo razzies.db (ou o que estiver em ConnectionStrings:DefaultConnection).
  - Na subida da aplicação, o banco é criado (EnsureCreated) e o CSV é carregado só se a tabela de vitórias estiver vazia.
  - O CSV (Movielist.csv) tem colunas como year, title, studios, producers, winner; a API considera só linhas com winner "yes" e extrai os produtores para montar as vitórias por ano.


Como rodar e testar
-------------------

  - Rodar a API: na pasta do projeto, "dotnet run" no projeto da API (ou pela IDE). Por padrão fica em http://localhost:5000.
  - Swagger: abra http://localhost:5000/swagger para ver os endpoints e testar; use "Authorize" com o token obtido no login.
  - Testes: "dotnet test" na raiz da solução. Os testes de integração sobem a API em memória, fazem login com usuário/senha de teste (configurados no WebAppFactory) e chamam o endpoint de intervalos.
  - Postman: na pasta docs tem uma collection (Golden-Raspberry-Awards-API.postman_collection.json). Importe no Postman; a variável baseUrl é http://localhost:5000. Rode primeiro o request "Login"; o token é salvo automaticamente e o "Get Intervals" já usa esse token.

Credenciais padrão de login (appsettings): userName "admin", password "admin". Em produção troque por algo seguro e não deixe senha no appsettings.


Resumo
------

É uma API pequena e direta: lê o CSV dos piores filmes, guarda no SQLite e responde quem teve menor e maior intervalo entre duas vitórias. Tudo isso com autenticação JWT, headers de segurança, rate limit, validação e tratamento de erro centralizados, seguindo boas práticas de ASP.NET Core e uma estrutura fácil de entender e manter.
