using Dominio.ObjetosDeValor.WebService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SGT.WebService.REST.Middleware
{
    public class ProcessarRequisicao
    {
        #region Propiedades Privadas
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<HttpLoggingOptions> _options;
        protected readonly IConfiguration _configuration;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        private static bool _isFirstRequestExecuted = false;
        private static readonly object _lock = new object();
        private static int _maxPoolSize = 600;
        #endregion

        #region Metodos Publicos

        public ProcessarRequisicao(RequestDelegate next, IOptionsMonitor<HttpLoggingOptions> options, ILogger<ProcessarRequisicao> logger, IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
            _options = options;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _maxPoolSize = _configuration.GetValue<int>("MaxPoolSize");
        }

        public Task Invoke(HttpContext context)
        {
            ExecuteOnFirstRequestOnly(context);

            if (!_logger.IsEnabled(LogLevel.Information))
            {
                return _next(context);
            }
            return InvokeInternal(context);
        }

        #endregion

        #region Metodo Privados

        private void ExecuteOnFirstRequestOnly(HttpContext context)
        {
            if (!_isFirstRequestExecuted)
            {
                lock (_lock)
                {
                    if (!_isFirstRequestExecuted)
                    {
                        Conexao.ConfigureFileStorage(context.Request, _memoryCache, _configuration, _webHostEnvironment);

                        _isFirstRequestExecuted = true;
                    }
                }
            }
        }

        private async Task InvokeInternal(HttpContext context)
        {
            using Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(context.Request, _memoryCache, _configuration, _webHostEnvironment, _maxPoolSize), Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);

            dynamic options = _options.CurrentValue;

            HttpRequest request = context.Request;
            string metodoUtilizado = request.Path.Value.Split("/")[2];
            string webService = request.Path.Value.Split("/")[1];

            string caminho = CaminhoArquivo(unitOfWork);
            string guidResponse = ObterGuid();
            string guidRequest = ObterGuid();

            string caminhoArquivoResponse = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidResponse);
            string caminhoArquivoRequest = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidRequest);

            Stream originalBody = context.Response.Body;
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            if (!VerificarSeMetodoExisteEGeraLog(unitOfWork, metodoUtilizado))
            {
                await responseBody.CopyToAsync(originalBody);
                context.Response.Body = originalBody;
                await _next(context);
                return;
            }

            Repositorio.Embarcador.Integracao.ControleDasIntegracoes repositorioControleDasIntegracoes = new Repositorio.Embarcador.Integracao.ControleDasIntegracoes(unitOfWork);
            Repositorio.WebService.Integradora repIntegradora = new Repositorio.WebService.Integradora(unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes controleDasIntegracoes = new Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes()
            {
                DataRequisicao = DateTime.Now,
                NomeMetodo = metodoUtilizado,
                Origem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas,
                Situacao = false,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao
            };
            await repositorioControleDasIntegracoes.InserirAsync(controleDasIntegracoes);
            await unitOfWork.CommitChangesAsync();

            try
            {
                context.Request.EnableBuffering();
                string token = request.Headers["token"];

                Dominio.Entidades.WebService.Integradora integradora = await repIntegradora.BuscarPorTokenAsync(token);
                context.Items["body"] = await GerarArquivoLogDoPayload(request, caminhoArquivoRequest, unitOfWork, controleDasIntegracoes, guidRequest, metodoUtilizado);

                if (integradora == null)
                    throw new Exception("Problema de autenticação. Token invalido ou integradora não cadastrada.");



                controleDasIntegracoes.Integradora = integradora;
                await _next(context);

                await GerarArquivoLogDoResponse(context.Response, caminhoArquivoResponse, unitOfWork, controleDasIntegracoes, guidResponse);

                await responseBody.CopyToAsync(originalBody);
                context.Response.Body = originalBody;

                if (context.Response.StatusCode != 200)
                    throw new Exception("Ocorreu uma falha ao processar a requisição.");

                controleDasIntegracoes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                controleDasIntegracoes.MensagemRetorno = "Integração feita com sucesso";
                controleDasIntegracoes.Situacao = true;
                await repositorioControleDasIntegracoes.AtualizarAsync(controleDasIntegracoes);
            }
            catch (Exception ex)
            {
                await responseBody.CopyToAsync(originalBody);
                context.Response.Body = originalBody;
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;
                string response = CriarRetorno(ex.Message);
                await context.Response.WriteAsync(response);

                controleDasIntegracoes.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                controleDasIntegracoes.MensagemRetorno = ex.Message;
                await repositorioControleDasIntegracoes.AtualizarAsync(controleDasIntegracoes);
                await GerarArquivoLogDoResponse(context.Response, caminhoArquivoResponse, unitOfWork, controleDasIntegracoes, guidResponse, response);
            }
        }

        private bool VerificarSeMetodoExisteEGeraLog(Repositorio.UnitOfWork unitOfWork, string NomeMetodo)
        {
            Repositorio.WebService.MetodosRest repositorioMetodosRequest = new Repositorio.WebService.MetodosRest(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            bool gerarLogMetodos = repositorioConfiguracaoWebService.BuscarGerarLogMetodosREST();

            List<dynamic> listaMetodos = null;
            _memoryCache.TryGetValue("MetodosRest", out listaMetodos);

            if (listaMetodos == null)
            {
                List<Dominio.Entidades.WebService.MetodosRest> listaMetodosExistentes = repositorioMetodosRequest.BuscarTodos();
                listaMetodos = new List<dynamic>();

                foreach (var registro in listaMetodosExistentes)
                    listaMetodos.Add(new { registro.NomeMetodo, registro.GeraLog });

                _memoryCache.Set("MetodosRest", listaMetodos);

                Dominio.Entidades.WebService.MetodosRest existeMetodoCadastrado = listaMetodosExistentes.Where(m => m.NomeMetodo.ToLower() == NomeMetodo.ToLower()).FirstOrDefault();

                if (existeMetodoCadastrado != null)
                {
                    existeMetodoCadastrado.GeraLog = gerarLogMetodos;

                    return existeMetodoCadastrado.GeraLog;
                }

                SalvarNovoMetodo(NomeMetodo, gerarLogMetodos, unitOfWork);
                return false;
            }

            dynamic metodo = listaMetodos.Where(m => m.NomeMetodo == NomeMetodo).FirstOrDefault();

            if (listaMetodos != null && listaMetodos.Count > 0 && metodo == null)
            {
                listaMetodos.Add(new { NomeMetodo, GeraLog = gerarLogMetodos });
                _memoryCache.Set("MetodosRest", listaMetodos);

                SalvarNovoMetodo(NomeMetodo, gerarLogMetodos, unitOfWork);
                return false;
            }

            return metodo.GeraLog;
        }

        private async Task<string> GerarArquivoLogDoPayload(HttpRequest request, string nomeArquvio, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes controleDasIntegracoes, string guidRequest, string nomeMetodo)
        {
            Repositorio.Embarcador.Integracao.ControleDasIntegracoesAnexo repositorioControleAnexos = new Repositorio.Embarcador.Integracao.ControleDasIntegracoesAnexo(unitOfWork);

            using var streamReader = new System.IO.StreamReader(
                          request.Body, System.Text.Encoding.UTF8, leaveOpen: true);

            string body = await streamReader.ReadToEndAsync();

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquvio))
            {
                await Utilidades.IO.FileStorageService.Storage.WriteLineAsync(nomeArquvio, ["Payload:", body]);
            }
            request.Body.Position = 0;

            Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoesAnexo registroRequest = new Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoesAnexo()
            {
                Descricao = "",
                EntidadeAnexo = controleDasIntegracoes,
                GuidArquivo = guidRequest,
                NomeArquivo = guidRequest
            };

            await repositorioControleAnexos.InserirAsync(registroRequest);

            SalvarArquivoExclusivo(body, nomeMetodo, unitOfWork);

            return body;
        }

        private void SalvarArquivoExclusivo(string body, string nomeMetodo, Repositorio.UnitOfWork unitOfWork)
        {
            if (nomeMetodo.ToLower() == "totemfilah" || nomeMetodo.ToLower() == "ghostvalidacaofiscal")
            {
                Servicos.Embarcador.Integracao.Ghost.IntegracaoGhost svcGhost = new Servicos.Embarcador.Integracao.Ghost.IntegracaoGhost(unitOfWork);
                svcGhost.GerarIntegracaoGhost(body, nomeMetodo.ToLower() == "totemfilah" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDestinoGhost.MuleSoft : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDestinoGhost.FilaH);
            }
        }

        private async Task GerarArquivoLogDoResponse(HttpResponse response, string nomeArquivo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes controleDasIntegracoes, string guid, string reponseString = "")
        {
            Repositorio.Embarcador.Integracao.ControleDasIntegracoesAnexo repositorioControleAnexos = new Repositorio.Embarcador.Integracao.ControleDasIntegracoesAnexo(unitOfWork);

            string text = string.Empty;
            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
            {
                if (string.IsNullOrEmpty(reponseString))
                {
                    Stream bodyRespose = response.Body;

                    bodyRespose.Seek(0, SeekOrigin.Begin);
                    text = await new StreamReader(bodyRespose).ReadToEndAsync();
                    bodyRespose.Seek(0, SeekOrigin.Begin);
                }
                else
                    text = reponseString;

                await Utilidades.IO.FileStorageService.Storage.WriteLineAsync(nomeArquivo, ["Resposta: ", text]);
            }

            Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoesAnexo registroResponse = new Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoesAnexo()
            {
                Descricao = "",
                EntidadeAnexo = controleDasIntegracoes,
                GuidArquivo = guid,
                NomeArquivo = guid
            };

            await repositorioControleAnexos.InserirAsync(registroResponse);
        }

        private void SalvarNovoMetodo(string nomeMetodo, bool gerarLogMetodos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.WebService.MetodosRest repositorioMetodosRequest = new Repositorio.WebService.MetodosRest(unitOfWork);

            bool exiteMetodoBanco = repositorioMetodosRequest.ExiteMetodoSalvoNoBanco(nomeMetodo);

            if (exiteMetodoBanco)
                return;

            Dominio.Entidades.WebService.MetodosRest novoMetodoRest = new Dominio.Entidades.WebService.MetodosRest()
            {
                GeraLog = gerarLogMetodos,
                NomeMetodo = nomeMetodo
            };
            repositorioMetodosRequest.Inserir(novoMetodoRest);
        }

        private string CaminhoArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "ControleDasIntegracoes");
#if DEBUG
            caminho = "C:\\Arquivos\\Integracao\\ControleDasIntegracoes";
#endif

            return caminho;
        }

        private string CriarRetorno(string message)
        {
            Retorno<bool> retorno = new Retorno<bool>()
            {
                CodigoMensagem = 400,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy"),
                Mensagem = message,
                Objeto = false,
                Status = false
            };
            return JsonConvert.SerializeObject(retorno);
        }

        private string ObterGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "") + ".txt";
        }
        #endregion
    }
}
