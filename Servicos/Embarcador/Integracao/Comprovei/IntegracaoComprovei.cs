using Dominio.Entidades.ProcessadorTarefas;
using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Repositorios.ProcessadorTarefas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.ProcessadorTarefas;
using Infrastructure.Services.HttpClientFactory;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.Comprovei
{
    public class IntegracaoComprovei
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Repositorio.Embarcador.Cargas.TipoIntegracao _repositorioTipoIntegracao;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei _configuracaoIntegracao;
        private ITarefaIntegracao _repositorioTarefaIntegracao;
        private readonly IRequestDocumentoRepository _repositorioRequestDocumento;
        private readonly IRequestSubtarefaRepository _repositorioSubtarefa;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoComprovei(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IntegracaoComprovei(Repositorio.UnitOfWork unitOfWork, IRequestDocumentoRepository repositorioRequestDocumento, ITarefaIntegracao repositorioTarefaIntegracao)
        {
            _unitOfWork = unitOfWork;
            _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
            _repositorioRequestDocumento = repositorioRequestDocumento;
        }

        public IntegracaoComprovei(Repositorio.UnitOfWork unitOfWork, IRequestSubtarefaRepository repositorioSubtarefa, ITarefaIntegracao repositorioTarefaIntegracao)
        {
            _unitOfWork = unitOfWork;
            _repositorioTarefaIntegracao = repositorioTarefaIntegracao;
            _repositorioSubtarefa = repositorioSubtarefa;
        }

        #endregion Construtores

        #region Métodos Públicos

        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei configComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(unitOfWork).Buscar();

            if (configComprovei == null)
            {
                cargaIntegracao.ProblemaIntegracao = "Falha ao buscar configuração da integração com a Comprovei";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            else
            {
                try
                {
                    System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configComprovei.URL);
                    System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                    binding.MaxReceivedMessageSize = int.MaxValue;
                    binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

                    if (configComprovei.URL.StartsWith("https"))
                        binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                    else
                        binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

                    binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

                    ServicoComprovei.CriarRota.WebServiceComproveiPortTypeClient client = new ServicoComprovei.CriarRota.WebServiceComproveiPortTypeClient(binding, endpointAddress);
                    ServicoComprovei.CriarRota.uploadRouteUsingDocumentKeyRequest request = new ServicoComprovei.CriarRota.uploadRouteUsingDocumentKeyRequest();

                    client.ClientCredentials.UserName.UserName = configComprovei.Usuario;
                    client.ClientCredentials.UserName.Password = configComprovei.Senha;

                    request.Credenciais = new ServicoComprovei.CriarRota.Credenciais();
                    request.Credenciais.Usuario = configComprovei.Usuario;
                    request.Credenciais.Senha = configComprovei.Senha;

                    request.Rotas = new ServicoComprovei.CriarRota.RotasType1();
                    request.Rotas.Rota = new ServicoComprovei.CriarRota.RotasTypeRota2();
                    request.Rotas.Rota.numero = (cargaIntegracao.Carga?.Codigo ?? 0).ToString();
                    request.Rotas.Rota.Data = cargaIntegracao.Carga.DataCriacaoCarga.ToString("yyyyMMdd");

                    request.Rotas.Rota.Transportadora = new ServicoComprovei.CriarRota.RotasTypeRotaTransportadora1();
                    request.Rotas.Rota.Transportadora.Codigo = cargaIntegracao.Carga?.Empresa.CNPJ ?? string.Empty;
                    request.Rotas.Rota.Transportadora.Razao = cargaIntegracao.Carga?.Empresa.RazaoSocial ?? string.Empty;

                    request.Rotas.Rota.Motorista = new ServicoComprovei.CriarRota.RotasTypeRotaMotorista1();
                    request.Rotas.Rota.Motorista.Usuario = cargaIntegracao.Carga.Motoristas.FirstOrDefault()?.CPF ?? String.Empty;
                    request.Rotas.Rota.Motorista.PlacaVeiculo = cargaIntegracao.Carga?.Veiculo?.Placa ?? String.Empty;
                    request.Rotas.Rota.Motorista.Nome = cargaIntegracao.Carga?.Motoristas.FirstOrDefault()?.Nome ?? String.Empty;

                    request.Rotas.Rota.TipoRota = ServicoComprovei.CriarRota.RotasTypeRotaTipoRota1.D;

                    request.nomeArquivo = "rota_" + (cargaIntegracao.Carga?.Codigo ?? 0).ToString() + ".xml";

                    request.Rotas.Rota.Paradas = new List<ServicoComprovei.CriarRota.RotasTypeRotaParada4>().ToArray<ServicoComprovei.CriarRota.RotasTypeRotaParada4>();

                    List<ServicoComprovei.CriarRota.RotasTypeRotaParada4> paradas = new List<ServicoComprovei.CriarRota.RotasTypeRotaParada4>();

                    int sequencia = 1;
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in cargaIntegracao.Carga.Pedidos)
                    {
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal nota in pedido.NotasFiscais)
                        {
                            ServicoComprovei.CriarRota.RotasTypeRotaParada4 rota = new ServicoComprovei.CriarRota.RotasTypeRotaParada4();
                            rota.numero = (sequencia++).ToString();
                            rota.Documento = new ServicoComprovei.CriarRota.RotasTypeRotaParadaDocumento2();
                            rota.Documento.ChaveNota = nota?.XMLNotaFiscal?.Chave ?? string.Empty;
                            paradas.Add(rota);
                        }
                    }

                    request.Rotas.Rota.Paradas = paradas.ToArray<ServicoComprovei.CriarRota.RotasTypeRotaParada4>();

                    System.Threading.Tasks.Task<ServicoComprovei.CriarRota.uploadRouteUsingDocumentKeyResponse> response = client.uploadRouteUsingDocumentKeyAsync(request);

                    /*
                    string protocolo = null;

                    var response = client.uploadRouteUsingDocumentKey(request.Credenciais, request.Rotas, request.nomeArquivo, out protocolo);

                    if (!string.IsNullOrEmpty(protocolo))
                    {
                        cargaIntegracao.Protocolo = protocolo;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                        cargaIntegracao.PendenteRetorno = true;
                        arquivoIntegracao.Mensagem = string.Empty;
                    }
                    else
                    {
                        arquivoIntegracao.Mensagem = response;
                        cargaIntegracao.ProblemaIntegracao = response;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    */

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

                    if (!string.IsNullOrEmpty(response?.Result?.protocolo))
                    {
                        cargaIntegracao.Protocolo = response.Result.protocolo;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                        cargaIntegracao.PendenteRetorno = true;
                        arquivoIntegracao.Mensagem = string.Empty;
                    }
                    else
                    {
                        arquivoIntegracao.Mensagem = response.Result.status;
                        cargaIntegracao.ProblemaIntegracao = response.Result.status;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    cargaIntegracao.NumeroTentativas += 1;
                    cargaIntegracao.DataIntegracao = DateTime.Now;

                    arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(Utilidades.XML.Serializar(request), "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(Utilidades.XML.Serializar(response), "xml", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                    cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);

                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = excecao.Message;
                }
            }

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei configComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(unitOfWork).Buscar();
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unitOfWork);

            if (configComprovei == null)
            {
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Falha ao buscar configuração da integração com a Comprovei";
                ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            else if ((ocorrenciaCTeIntegracao.CargaOcorrencia?.TipoOcorrencia?.TipoIntegracaoComprovei ?? null) == null)
            {
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Tipo de ocorrencia não tem tipo de integração com a Comprovei cadastrada";
                ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            else
            {
                HttpClient requisicao = CriarRequisicao(configComprovei);

                string jsonRequisicao = gerarJsonRequisicaoOcorrencia(ocorrenciaCTeIntegracao, unitOfWork);

                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configComprovei.URLBaseRest + endpointOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia?.TipoOcorrencia?.TipoIntegracaoComprovei ?? TipoIntegracaoComprovei.AgendamentoEntrega), conteudoRequisicao).Result;

                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created || retornoRequisicao.StatusCode == HttpStatusCode.Accepted) && !string.IsNullOrEmpty(retornoIntegracao.protocol.Value))
                {
                    ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    ocorrenciaCTeIntegracao.PendenteRetorno = true;
                    ocorrenciaCTeIntegracao.Protocolo = retornoIntegracao.Protocolo;
                }
                else
                {
                    ocorrenciaCTeIntegracao.ProblemaIntegracao = (string)retornoIntegracao.message;
                    ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();

                ocorrenciaCTeIntegracao.NumeroTentativas += 1;
                ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;

                arquivoIntegracao.Data = ocorrenciaCTeIntegracao.DataIntegracao;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork);
                arquivoIntegracao.Mensagem = (string)retornoIntegracao.message;

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            }

            repOcorrenciaIntegracao.Atualizar(ocorrenciaCTeIntegracao);
        }

        public static void ConsultarProtocolos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> cargaCargaIntegracaos = repCargaIntegracao.BuscarRegistrosComRetornoPendente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao in cargaCargaIntegracaos)
            {
                ConsultarProtocoloCarga(cargaIntegracao, unitOfWork);
            }

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ocorrenciaCTeIntegracaos = repOcorrenciaIntegracao.BuscarRegistrosComRetornoPendente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Comprovei);

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ocorrenciaCTeIntegracaos)
            {
                ConsultarProtocoloOcorrencia(ocorrenciaCTeIntegracao, unitOfWork);
            }
        }

        public void IntegrarIACanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei configuracaoIntegracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(_unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(_unitOfWork).BuscarConfiguracaoPadrao();
            Servicos.Embarcador.Canhotos.CanhotoIntegracao integracaoCanhoto = new Servicos.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork);

            canhotoIntegracao.DataIntegracao = DateTime.Now;
            canhotoIntegracao.NumeroTentativas++;

            string mensagem = string.Empty;
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;
            int tentativas = 0;
            bool sucesso = false;
            bool reenviou = false;

            try
            {
                if (string.IsNullOrEmpty(configuracaoIntegracaoComprovei.URLIACanhoto))
                    throw new ServicoException("Sem URL de Configuração para Integração do Canhoto com a Comprovei.");

                string URL = configuracaoIntegracaoComprovei.URLIACanhoto; //URL Base: "https://api.comprovei.com.br/api/1.1/util/evaluateInvoiceImage";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoComprovei));
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(configuracaoIntegracaoComprovei.UsuarioIACanhoto, configuracaoIntegracaoComprovei.SenhaIACanhoto);
                client.Timeout = TimeSpan.FromMinutes(5);

                ValidarDadosCanhotoParaEnvio(canhotoIntegracao.Canhoto);
                jsonRequisicao = ObterJsonValidacaoIACanhoto(canhotoIntegracao.Canhoto, configuracaoCanhoto);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                while (tentativas < 2)
                {
                    tentativas++;

                    HttpResponseMessage resposta = client.PostAsync(URL, conteudoRequisicao).Result;
                    jsonRetorno = resposta.Content.ReadAsStringAsync().Result;
                    dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                    if (resposta.StatusCode == HttpStatusCode.OK && retornoIntegracao != null && retornoIntegracao.message == null)
                    {
                        bool number_percent = retornoIntegracao.number_percent == "1.00";
                        bool neural_evaluation = retornoIntegracao.neural_evaluation == "1.00";
                        bool date_evaluation = retornoIntegracao.date_evaluation == "1.00";
                        bool signature_evaluation = retornoIntegracao.signature_evaluation == "1.00";

                        if (configuracaoCanhoto.ValidacaoNumero != null)
                        {
                            double numberPercent = double.Parse(((string)retornoIntegracao.number_percent).Replace(".", ","));
                            number_percent = number_percent || numberPercent >= configuracaoCanhoto.ValidacaoNumero;
                        }

                        if (configuracaoCanhoto.ValidacaoCanhoto != null)
                        {
                            double neuralEvaluation = double.Parse(((string)retornoIntegracao.neural_evaluation).Replace(".", ","));
                            neural_evaluation = neural_evaluation || neuralEvaluation >= configuracaoCanhoto.ValidacaoCanhoto;
                        }

                        if (configuracaoCanhoto.ValidacaoEncontrouData != null)
                        {
                            double dateEvaluation = double.Parse(((string)retornoIntegracao.date_evaluation).Replace(".", ","));
                            date_evaluation = date_evaluation || dateEvaluation >= configuracaoCanhoto.ValidacaoEncontrouData;
                        }

                        if (configuracaoCanhoto.ValidacaoAssinatura != null)
                        {
                            double signatureEvaluation = double.Parse(((string)retornoIntegracao.signature_evaluation).Replace(".", ","));
                            signature_evaluation = signature_evaluation || signatureEvaluation >= configuracaoCanhoto.ValidacaoAssinatura;
                        }

                        //Preenche dados de validação do canhoto no canhoto.
                        canhotoIntegracao.Canhoto.PossuiIntegracaoComprovei = true;
                        canhotoIntegracao.Canhoto.ValidacaoCanhoto = neural_evaluation;
                        canhotoIntegracao.Canhoto.ValidacaoNumero = number_percent;
                        canhotoIntegracao.Canhoto.ValidacaoEncontrouData = date_evaluation;
                        canhotoIntegracao.Canhoto.ValidacaoAssinatura = signature_evaluation;

                        //Replica informação na integração, para guardar histórico por integração.
                        canhotoIntegracao.ValidacaoCanhoto = canhotoIntegracao.Canhoto.ValidacaoCanhoto;
                        canhotoIntegracao.ValidacaoNumero = canhotoIntegracao.Canhoto.ValidacaoNumero;
                        canhotoIntegracao.ValidacaoEncontrouData = canhotoIntegracao.Canhoto.ValidacaoEncontrouData;
                        canhotoIntegracao.ValidacaoAssinatura = canhotoIntegracao.Canhoto.ValidacaoAssinatura;

                        //Devops: #4769 - Aprovação automática dos canhotos.
                        if (configuracaoCanhoto.AprovarAutomaticamenteADigitalizacaoDosCanhotosCasoAValidacaoDaIAComproveiSejaCompleta)
                        {
                            if (canhotoIntegracao.Canhoto.ValidacaoCanhoto && canhotoIntegracao.Canhoto.ValidacaoNumero && canhotoIntegracao.Canhoto.ValidacaoEncontrouData && canhotoIntegracao.Canhoto.ValidacaoAssinatura)
                            {
                                canhotoIntegracao.Canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                                canhotoIntegracao.Canhoto.OrigemSituacaoDigitalizacaoCanhoto = OrigemSituacaoDigitalizacaoCanhoto.IA;
                                Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhotoIntegracao.Canhoto, _unitOfWork, canhotoIntegracao.TipoServicoMultisoftware);
                            }
                            else
                            {
                                if (configuracaoCanhoto.HabilitarFluxoAnaliseCanhotoRejeitadoIA)
                                {
                                    canhotoIntegracao.Canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.ValidacaoEmbarcador;
                                }
                                else
                                {
                                    canhotoIntegracao.Canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.DigitalizacaoRejeitada;
                                }
                            }
                        }

                        canhotoIntegracao.Canhoto.DataUltimaModificacao = DateTime.Now;
                        repCanhoto.Atualizar(canhotoIntegracao.Canhoto);

                        sucesso = true;
                        mensagem = SituacaoIntegracao.Integrado.ObterDescricao();
                    }
                    else
                    {
                        mensagem = retornoIntegracao["message"] != null ? retornoIntegracao.message : resposta.ToString();
                        canhotoIntegracao.ProblemaIntegracao = mensagem;
                    }

                    if (canhotoIntegracao.Canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado && ObterRepositorioTipoIntegracao().ExistePorTipo(TipoIntegracao.Mars))
                        integracaoCanhoto.GerarIntegracaoAceiteCanhotoAsync(canhotoIntegracao.Canhoto, canhotoIntegracao.TipoServicoMultisoftware).ConfigureAwait(false).GetAwaiter().GetResult();

                    //#16969 - Reenvio de integração caso houver falha na validação de Número ou Formato do canhoto.
                    if (!reenviou &&
                        configuracaoCanhoto.ReenviarUmaVezIntegracaoCasoRetornarFalhaNaValidacaoDoNumeroDoCanhotoEOuFormatoDoCanhoto &&
                        (!canhotoIntegracao.ValidacaoCanhoto || !canhotoIntegracao.ValidacaoNumero))
                    {
                        SalvarJsonIntegracao(canhotoIntegracao, jsonRequisicao, jsonRetorno, "Falha na Validação do Número do Canhoto ou Formato do Canhoto, encaminhado para reenvio.");
                        reenviou = true;
                    }
                    else
                        break;
                }

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                mensagem = $"Falha ao integrar canhoto: {ex.Message}";

                if (ex.InnerException != null)
                    mensagem += $" - {ex.InnerException.Message}";
            }

            SalvarJsonIntegracao(canhotoIntegracao, jsonRequisicao, jsonRetorno, mensagem);

            canhotoIntegracao.SituacaoIntegracao = sucesso ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
            canhotoIntegracao.ProblemaIntegracao = mensagem;

            repCanhotoIntegracao.Atualizar(canhotoIntegracao);
        }

        public async Task IntegrarRetornoConfirmacaoPedidosAsync(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
        {
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            tarefaIntegracao.Tentativas++;
            tarefaIntegracao.DataIntegracao = DateTime.UtcNow;

            try
            {
                await ObterConfiguracaoIntegracaoAsync();

                HttpClient requisicao = CriarRequisicao(_configuracaoIntegracao);

                List<RequestSubtarefa> subtarefas = await _repositorioSubtarefa.ObterPorTarefaIdAsync(contexto.TarefaId, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.ConfirmacaoPedido.EnviaConfirmacaoPedido corpoRequisicao = PreencherConfirmacaoPedido(contexto, subtarefas);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracao.URLIntegracaoRetornoConfirmacaoPedidos, conteudoRequisicao).Result;
                retornoRequisicao.Content.Headers.ContentType.CharSet = "ISO-8859-1";

                jsonRetorno = Newtonsoft.Json.Linq.JToken.Parse(await retornoRequisicao.Content.ReadAsStringAsync()).ToString(Formatting.Indented);
                jsonRetorno = LimparRespostaComprovei(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado Comprovei.");

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Problema ao tentar integrar com Comprovei.");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                tarefaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                tarefaIntegracao.ProblemaIntegracao = excecao.Message;
                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoComprovei");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tarefaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Comprovei.";
            }
            finally
            {
                var arquivo = new Dominio.Entidades.ProcessadorTarefas.ArquivoIntegracao
                {
                    ArquivoRequisicao = !string.IsNullOrEmpty(jsonRequisicao) ? MongoDB.Bson.BsonDocument.Parse(jsonRequisicao) : null,
                    ArquivoResposta = !string.IsNullOrEmpty(jsonRetorno) ? MongoDB.Bson.BsonDocument.Parse(jsonRetorno) : null,
                    Tipo = "json"
                };

                await _repositorioTarefaIntegracao.AdicionarArquivoAsync(tarefaIntegracao.Id, arquivo, cancellationToken);

                var update = Builders<TarefaIntegracao>.Update
                    .Set(x => x.SituacaoIntegracao, tarefaIntegracao.SituacaoIntegracao)
                    .Set(x => x.DataIntegracao, tarefaIntegracao.DataIntegracao)
                    .Set(x => x.ProblemaIntegracao, tarefaIntegracao.ProblemaIntegracao)
                    .Set(x => x.Tentativas, tarefaIntegracao.Tentativas);

                await _repositorioTarefaIntegracao.AtualizarAsync(tarefaIntegracao.Id, update, cancellationToken);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.ConfirmacaoPedido.EnviaConfirmacaoPedido PreencherConfirmacaoPedido(ContextoEtapa contexto, List<RequestSubtarefa> subtarefas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.ConfirmacaoPedido.ConfiguracaoPedido> configuracaoPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.ConfirmacaoPedido.ConfiguracaoPedido>();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ObjetoConfirmacaoPedido> objeto = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ObjetoConfirmacaoPedido>();

            if (contexto.Tarefa.Resultado != null && contexto.Tarefa.Resultado.Contains("pedidos") && contexto.Tarefa.Resultado["pedidos"].IsBsonArray)
            {
                var pedidosArray = contexto.Tarefa.Resultado["pedidos"].AsBsonArray;
                objeto = pedidosArray.Select(p => p.AsBsonDocument.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ObjetoConfirmacaoPedido>()).ToList();
            }

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            foreach (RequestSubtarefa subtarefa in subtarefas)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmacaoPedido.ObjetoConfirmacaoPedido item = objeto.Find(o => o.Codigo == subtarefa.Id);

                string numeroPedido = item?.NumeroPedido ?? string.Empty;
                int protocolo = item?.Protocolo ?? 0;

                if (string.IsNullOrWhiteSpace(numeroPedido) && protocolo > 0)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(protocolo);
                    if (pedido != null && !string.IsNullOrWhiteSpace(pedido.NumeroPedidoEmbarcador))
                    {
                        numeroPedido = pedido.NumeroPedidoEmbarcador;
                    }
                }

                configuracaoPedidos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.ConfirmacaoPedido.ConfiguracaoPedido
                {
                    NumeroPedido = numeroPedido,
                    Protocolo = protocolo,
                    Mensagem = subtarefa.Mensagem ?? string.Empty
                });
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.ConfirmacaoPedido.EnviaConfirmacaoPedido
            {
                ConfiguracaoPedido = configuracaoPedidos
            };
        }

        public async Task IntegrarRetornoGerarCarregamentoAsync(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
        {
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            tarefaIntegracao.Tentativas++;
            tarefaIntegracao.DataIntegracao = DateTime.UtcNow;

            try
            {
                await ObterConfiguracaoIntegracaoAsync();

                HttpClient requisicao = CriarRequisicao(_configuracaoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.RequisicaoRetornoGerarCarregamento corpoRequisicao = await PreencherRetornoGerarCarregamento(contexto, cancellationToken);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = await requisicao.PostAsync(_configuracaoIntegracao.URLIntegracaoRetornoGerarCarregamento, conteudoRequisicao);
                retornoRequisicao.Content.Headers.ContentType.CharSet = "ISO-8859-1";

                jsonRetorno = Newtonsoft.Json.Linq.JToken.Parse(await retornoRequisicao.Content.ReadAsStringAsync()).ToString(Formatting.Indented);
                jsonRetorno = LimparRespostaComprovei(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado Comprovei.");

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Problema ao tentar integrar com Comprovei.");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                tarefaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                tarefaIntegracao.ProblemaIntegracao = excecao.Message;
                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoComprovei");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tarefaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com Comprovei.";
            }
            finally
            {
                var arquivo = new Dominio.Entidades.ProcessadorTarefas.ArquivoIntegracao
                {
                    ArquivoRequisicao = !string.IsNullOrEmpty(jsonRequisicao) ? MongoDB.Bson.BsonDocument.Parse(jsonRequisicao) : null,
                    ArquivoResposta = !string.IsNullOrEmpty(jsonRetorno) ? MongoDB.Bson.BsonDocument.Parse(jsonRetorno) : null,
                    Tipo = "json"
                };

                await _repositorioTarefaIntegracao.AdicionarArquivoAsync(tarefaIntegracao.Id, arquivo, cancellationToken);

                var update = Builders<TarefaIntegracao>.Update
                    .Set(x => x.SituacaoIntegracao, tarefaIntegracao.SituacaoIntegracao)
                    .Set(x => x.DataIntegracao, tarefaIntegracao.DataIntegracao)
                    .Set(x => x.ProblemaIntegracao, tarefaIntegracao.ProblemaIntegracao)
                    .Set(x => x.Tentativas, tarefaIntegracao.Tentativas);

                await _repositorioTarefaIntegracao.AtualizarAsync(tarefaIntegracao.Id, update, cancellationToken);
            }
        }

        public async Task IntegrarRetornoEnviarDigitalizacaoCanhotoAsync(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
        {
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            tarefaIntegracao.Tentativas++;
            tarefaIntegracao.DataIntegracao = DateTime.UtcNow;

            try
            {
                await ObterConfiguracaoIntegracaoAsync();

                HttpClient requisicao = CriarRequisicao(_configuracaoIntegracao);

                List<RequestSubtarefa> subtarefas = await _repositorioSubtarefa.ObterPorTarefaIdAsync(contexto.TarefaId, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto.RequisicaoEnviarDigitalizacaoCanhoto corpoRequisicao = PreencherEnviarDigitalizacaoCanhoto(contexto, subtarefas);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracao.URLIntegracaoRetornoEnviarDigitalizacaoCanhoto, conteudoRequisicao).Result;
                retornoRequisicao.Content.Headers.ContentType.CharSet = "ISO-8859-1";

                jsonRetorno = Newtonsoft.Json.Linq.JToken.Parse(await retornoRequisicao.Content.ReadAsStringAsync()).ToString(Formatting.Indented);
                jsonRetorno = LimparRespostaComprovei(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado Comprovei.");

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException("Problema ao tentar integrar com Comprovei.");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                tarefaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                tarefaIntegracao.ProblemaIntegracao = excecao.Message;
                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoComprovei");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tarefaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Comprovei.";
            }
            finally
            {
                var arquivo = new Dominio.Entidades.ProcessadorTarefas.ArquivoIntegracao
                {
                    ArquivoRequisicao = !string.IsNullOrEmpty(jsonRequisicao) ? MongoDB.Bson.BsonDocument.Parse(jsonRequisicao) : null,
                    ArquivoResposta = !string.IsNullOrEmpty(jsonRetorno) ? MongoDB.Bson.BsonDocument.Parse(jsonRetorno) : null,
                    Tipo = "json"
                };

                await _repositorioTarefaIntegracao.AdicionarArquivoAsync(tarefaIntegracao.Id, arquivo, cancellationToken);

                var update = Builders<TarefaIntegracao>.Update
                    .Set(x => x.SituacaoIntegracao, tarefaIntegracao.SituacaoIntegracao)
                    .Set(x => x.DataIntegracao, tarefaIntegracao.DataIntegracao)
                    .Set(x => x.ProblemaIntegracao, tarefaIntegracao.ProblemaIntegracao)
                    .Set(x => x.Tentativas, tarefaIntegracao.Tentativas);

                await _repositorioTarefaIntegracao.AtualizarAsync(tarefaIntegracao.Id, update, cancellationToken);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto.RequisicaoEnviarDigitalizacaoCanhoto PreencherEnviarDigitalizacaoCanhoto(ContextoEtapa contexto, List<RequestSubtarefa> subtarefas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto.ObjetoEnviarDigitalizacaoCanhoto> objetos = contexto.Tarefa.Resultado?.FromBsonDocument<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto.ObjetoEnviarDigitalizacaoCanhoto>>() ?? new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto.ObjetoEnviarDigitalizacaoCanhoto>();

            if (objetos == null || objetos.Count == 0)
                Log.TratarErro($"Tarefa objeto {contexto.TarefaId} não definido no método PreencherEnviarDigitalizacaoCanhoto", "IntegracaoComprovei");

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto.ConfiguracaoCanhoto> resultados = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto.ConfiguracaoCanhoto>();

            foreach (RequestSubtarefa subtarefa in subtarefas)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto.ObjetoEnviarDigitalizacaoCanhoto item = objetos.FirstOrDefault(o => o.Codigo == subtarefa.Id);
                resultados.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto.ConfiguracaoCanhoto
                {
                    ChaveNFe = item?.ChaveNFe ?? string.Empty,
                    Sucesso = item?.StatusIntegracao ?? false,
                    Descricao = item?.Descricao ?? string.Empty,
                });
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.EnviarDigitalizacaoCanhoto.RequisicaoEnviarDigitalizacaoCanhoto
            {
                Success = resultados.All(x => x.Sucesso),
                Account = objetos.FirstOrDefault()?.Account ?? string.Empty,
                Protocol = objetos.FirstOrDefault()?.Protocolo ?? string.Empty,
                Description = resultados.Any(x => !x.Sucesso) ? "Falha no processamento" : "Processado com sucesso",
                Results = resultados
            };
        }

        public async Task IntegrarRetornoAdicionarAtendimentoAsync(ContextoEtapa contexto, TarefaIntegracao tarefaIntegracao, CancellationToken cancellationToken)
        {
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            tarefaIntegracao.Tentativas++;
            tarefaIntegracao.DataIntegracao = DateTime.UtcNow;

            try
            {
                await ObterConfiguracaoIntegracaoAsync();

                HttpClient requisicao = CriarRequisicao(_configuracaoIntegracao);

                List<RequestSubtarefa> subtarefas = await _repositorioSubtarefa.ObterPorTarefaIdAsync(contexto.TarefaId, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.RequisicaoAdicionarAtendimentoEmLote corpoRequisicao = PreencherRetornoAdicionarAtendimento(contexto, subtarefas);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracao.URLIntegracaoRetornoEnviarDigitalizacaoCanhoto, conteudoRequisicao).Result;
                retornoRequisicao.Content.Headers.ContentType.CharSet = "ISO-8859-1";

                jsonRetorno = Newtonsoft.Json.Linq.JToken.Parse(await retornoRequisicao.Content.ReadAsStringAsync()).ToString(Formatting.Indented);
                jsonRetorno = LimparRespostaComprovei(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado Comprovei.");

                if (!retornoRequisicao.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.RetornoIntegracao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.RetornoIntegracao>(jsonRetorno);
                    throw new ServicoException("Retorno integração Comprovei: " + retorno.Mensagem);
                }

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                tarefaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
            }
            catch (ServicoException excecao)
            {
                tarefaIntegracao.ProblemaIntegracao = excecao.Message;
                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoComprovei");

                tarefaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                tarefaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Comprovei.";
            }
            finally
            {
                var arquivo = new Dominio.Entidades.ProcessadorTarefas.ArquivoIntegracao
                {
                    ArquivoRequisicao = !string.IsNullOrEmpty(jsonRequisicao) ? MongoDB.Bson.BsonDocument.Parse(jsonRequisicao) : null,
                    ArquivoResposta = !string.IsNullOrEmpty(jsonRetorno) ? MongoDB.Bson.BsonDocument.Parse(jsonRetorno) : null,
                    Tipo = "json"
                };

                await _repositorioTarefaIntegracao.AdicionarArquivoAsync(tarefaIntegracao.Id, arquivo, cancellationToken);

                var update = Builders<TarefaIntegracao>.Update
                    .Set(x => x.SituacaoIntegracao, tarefaIntegracao.SituacaoIntegracao)
                    .Set(x => x.DataIntegracao, tarefaIntegracao.DataIntegracao)
                    .Set(x => x.ProblemaIntegracao, tarefaIntegracao.ProblemaIntegracao)
                    .Set(x => x.Tentativas, tarefaIntegracao.Tentativas);

                await _repositorioTarefaIntegracao.AtualizarAsync(tarefaIntegracao.Id, update, cancellationToken);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.RequisicaoAdicionarAtendimentoEmLote PreencherRetornoAdicionarAtendimento(ContextoEtapa contexto, List<RequestSubtarefa> subtarefas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.ObjetoAdicionarAtendimentoEmLote> objetos = contexto.Tarefa.Resultado?.FromBsonDocument<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.ObjetoAdicionarAtendimentoEmLote>>() ?? new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.ObjetoAdicionarAtendimentoEmLote>();

            if (objetos == null || objetos.Count == 0)
                Log.TratarErro($"Tarefa objeto {contexto.TarefaId} não definido no método PreencherRetornoAdicionarAtendimento", "IntegracaoComprovei");

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.ConfiguracaoAtendimento> results = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.ConfiguracaoAtendimento>();

            foreach (RequestSubtarefa subtarefa in subtarefas)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.ObjetoAdicionarAtendimentoEmLote item = objetos.FirstOrDefault(o => o.Codigo == subtarefa.Id);
                results.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.ConfiguracaoAtendimento
                {
                    ChaveNFe = item?.ChaveNFe ?? string.Empty,
                    Sucesso = item?.StatusIntegracao ?? false,
                    Descricao = item?.Descricao ?? string.Empty,
                });
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.AdicionarAtendimento.RequisicaoAdicionarAtendimentoEmLote
            {
                Success = results.All(x => x.Sucesso),
                Account = objetos.FirstOrDefault()?.Account ?? string.Empty,
                Protocol = objetos.FirstOrDefault()?.Protocolo ?? string.Empty,
                Description = results.Any(x => !x.Sucesso) ? "Falha no processamento" : "Processado com sucesso",
                Results = results
            };
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private static HttpClient CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei configComprovei)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoComprovei));

            requisicao.BaseAddress = new Uri(configComprovei.URLBaseRest);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(configComprovei.Usuario, configComprovei.Senha);

            return requisicao;
        }

        private static string endpointOcorrencia(TipoIntegracaoComprovei tipoIntegracao)
        {
            return tipoIntegracao == TipoIntegracaoComprovei.AgendamentoEntrega ? "v1/documents/scheduling" : "v1/events/send";
        }

        private static string gerarJsonRequisicaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            string json = string.Empty;

            if (ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.TipoIntegracaoComprovei == TipoIntegracaoComprovei.AgendamentoEntrega)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.RequisicaoAgendamentoDocumentos bodyRequisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.RequisicaoAgendamentoDocumentos()
                {
                    Key = ocorrenciaCTeIntegracao?.CargaCTe?.CTe?.Chave ?? string.Empty,
                    Date = ocorrenciaCTeIntegracao?.CargaOcorrencia?.DataOcorrencia.ToString("yyyy-MM-dd") ?? "0000-00-00",
                    Annotation = ocorrenciaCTeIntegracao?.CargaOcorrencia?.Observacao ?? string.Empty,
                    Code = ocorrenciaCTeIntegracao?.CargaOcorrencia?.TipoOcorrencia?.CodigoIntegracao ?? string.Empty
                };

                json = "[" + JsonConvert.SerializeObject(bodyRequisicao) + "]";
            }

            if (ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.TipoIntegracaoComprovei == TipoIntegracaoComprovei.BaixaDocumentos)
            {
                json = "{ ";

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNota in ocorrenciaCTeIntegracao.CargaCTe?.CTe.XMLNotaFiscais ?? new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>())
                {
                    string arquivoCanhoto = string.Empty;

                    if (XMLNota.Canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado)
                    {
                        string extensao = System.IO.Path.GetExtension(XMLNota.Canhoto.NomeArquivo).ToLower();
                        string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(XMLNota.Canhoto, unitOfWork);
                        arquivoCanhoto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, XMLNota.Canhoto.GuidNomeArquivo + extensao);
                    }

                    json += "\"" + XMLNota.Chave + "\": { " +
                                "\"code\": \"" + (ocorrenciaCTeIntegracao?.CargaOcorrencia?.TipoOcorrencia?.CodigoIntegracao ?? string.Empty) + "\", " +
                                "\"start_date\": \"" + (ocorrenciaCTeIntegracao.CargaOcorrencia.DataEvento?.ToString("yyyy'-'MM'-'dd HH':'mm':'ss") ?? string.Empty) + "\", " +
                                "\"end_date\": \"" + (ocorrenciaCTeIntegracao.CargaOcorrencia?.DataOcorrencia.ToString("yyyy'-'MM'-'dd HH':'mm':'ss") ?? string.Empty) + "\", ";

                    if (!string.IsNullOrEmpty(arquivoCanhoto))
                        json += "\"pictures\": [" +
                                    "{" +
                                        "\"link\": \"" + "" + arquivoCanhoto + "\"," +
                                        "\"subtitle\": \"" + XMLNota.Canhoto.NomeArquivo + "\" " +
                                    "}" +
                                "], ";

                    json += "\"annotation\": \"" + (ocorrenciaCTeIntegracao?.CargaOcorrencia?.Observacao ?? string.Empty) + "\"," +
                                "\"coordinates\": {" +
                                    "\"latitude\": " + (XMLNota?.Destinatario?.Latitude.ToString() ?? "0.0") + ", " +
                                    "\"longitude\": " + (XMLNota?.Destinatario?.Latitude.ToString() ?? "0.0") +
                                "}" +
                            "},";

                }
                json = json.Remove(json.Length - 1) + "}";

            }

            return json;
        }

        private static void ConsultarProtocoloCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei configComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(unitOfWork).Buscar();

            if (configComprovei != null)
            {
                System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configComprovei.URL);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

                if (configComprovei.URL.StartsWith("https"))
                    binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                else
                    binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

                ServicoComprovei.CriarRota.WebServiceComproveiPortTypeClient client = new ServicoComprovei.CriarRota.WebServiceComproveiPortTypeClient(binding, endpointAddress);
                ServicoComprovei.CriarRota.getImportProtocolStatusRequest request = new ServicoComprovei.CriarRota.getImportProtocolStatusRequest();

                client.ClientCredentials.UserName.UserName = configComprovei.Usuario;
                client.ClientCredentials.UserName.Password = configComprovei.Senha;

                request.Credenciais = new ServicoComprovei.CriarRota.Credenciais();
                request.Credenciais.Usuario = configComprovei.Usuario;
                request.Credenciais.Senha = configComprovei.Senha;

                request.protocolo = cargaIntegracao?.Protocolo ?? string.Empty;

                System.Threading.Tasks.Task<Servicos.ServicoComprovei.CriarRota.getImportProtocolStatusResponse> response = client.getImportProtocolStatusAsync(request);

                if (response.Result.status == "Protocolo encontrado." && response.Result.processado == "Sim")
                {
                    if (response.Result.resultado == "Rota importada com sucesso!")
                    {
                        cargaIntegracao.PendenteRetorno = false;
                    }
                    else
                    {
                        cargaIntegracao.ProblemaIntegracao = response.Result.resultado;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    repCargaIntegracao.Atualizar(cargaIntegracao);
                }
            }
        }

        private static void ConsultarProtocoloOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei configComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(unitOfWork).Buscar();
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);

            if (configComprovei != null)
            {
                HttpClient requisicao = CriarRequisicao(configComprovei);

                HttpResponseMessage retornoRequisicao = requisicao.GetAsync(configComprovei.URLBaseRest + "v1/protocols/" + (ocorrenciaCTeIntegracao?.CargaOcorrencia?.Protocolo.ToString() ?? string.Empty)).Result;

                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created) && (int)retornoIntegracao.processed == 1)
                {
                    if (retornoIntegracao.success != null)
                    {
                        if (!(bool)retornoIntegracao.success)
                        {
                            ocorrenciaCTeIntegracao.ProblemaIntegracao = (string)retornoIntegracao.details[(retornoIntegracao.details.Count - 1 >= 0) ? retornoIntegracao.details.Count - 1 : 0].response.message;
                            ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }

                        ocorrenciaCTeIntegracao.PendenteRetorno = false;
                        repOcorrenciaIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                    }
                }
            }
        }

        private string ObterJsonValidacaoIACanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto)
        {
            string base64String = string.Empty;
            string caminho = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto);
            string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
            if (extensao == ".pdf") throw new ServicoException("Arquivo PDF não está preparado para integrar.");
            string nomeAbsolutoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);
            if (Utilidades.IO.FileStorageService.Storage.Exists(nomeAbsolutoArquivo))
                base64String = "data:image/" + extensao.Replace(".", "") + ";base64," + Convert.ToBase64String(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeAbsolutoArquivo));

            long tamanhoBase64String = Utilidades.IO.FileStorageService.Storage.GetFileInfo(nomeAbsolutoArquivo).Size;
            long tamanhoMaximoBits = 1000000;

            if (configuracaoCanhoto.CompactarImagemCanhotoIaComproveiCasoTamanhoUltrapasseUmMB && tamanhoBase64String > tamanhoMaximoBits)
            {
                ComprimirImagemAteTamanho(nomeAbsolutoArquivo, tamanhoMaximoBits);
            }

            double flexibilidade = 0.8;
            if (configuracaoCanhoto.FlexibilidadeParaValidacaoNaIAComprovei > 0)
                flexibilidade = configuracaoCanhoto.FlexibilidadeParaValidacaoNaIAComprovei;

            dynamic objCanhoto = new
            {
                leitura = string.Empty,
                numero_nota = canhoto.XMLNotaFiscal.Numero.ToString(),
                completar_numero = false,
                mostrar_leitura = true,
                flexibilidade = flexibilidade,
                img_data = base64String
            };


            return JsonConvert.SerializeObject(objCanhoto);
        }

        private void ValidarDadosCanhotoParaEnvio(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto)
        {
            Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(_unitOfWork);

            if ((canhoto.Carga.TipoOperacao?.ConfiguracaoCanhoto?.NaoPermiteUploadDeCanhotosComCTeNaoAutorizado ?? false) && servicoCanhoto.CanhotoPossuiCTeNaoAutorizado(canhoto))
                throw new ServicoException("O CT-e do Canhoto não está Autorizado.");
        }

        private void SalvarJsonIntegracao(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao, string jsonRequisicao, string jsonRetorno, string mensagemRetorno = null)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Data = canhotoIntegracao.DataIntegracao,
                Mensagem = !string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : canhotoIntegracao.ProblemaIntegracao,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (canhotoIntegracao.ArquivosTransacao == null)
                canhotoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            canhotoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private Repositorio.Embarcador.Cargas.TipoIntegracao ObterRepositorioTipoIntegracao()
        {
            if (_repositorioTipoIntegracao == null)
                _repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            return _repositorioTipoIntegracao;
        }

        private async Task ObterConfiguracaoIntegracaoAsync()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoComprovei repositorioIntegracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(_unitOfWork);
            _configuracaoIntegracao = await repositorioIntegracaoComprovei.BuscarPrimeiroRegistroAsync();

            if ((_configuracaoIntegracao == null) || !_configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para o Comprovei");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLBaseRest))
                throw new ServicoException("O URL deve estar preenchidos na configuração de integração do Comprovei");
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.RequisicaoRetornoGerarCarregamento> PreencherRetornoGerarCarregamento(ContextoEtapa contexto, CancellationToken cancellationToken)
        {
            RequestDocumento requestDoc = contexto.RequestDoc;
            if (requestDoc == null && !string.IsNullOrEmpty(contexto.Tarefa.RequestId))
            {
                requestDoc = await _repositorioRequestDocumento.ObterPorIdAsync(contexto.Tarefa.RequestId, cancellationToken);
            }
            
            Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento carregamento = requestDoc?.Dados?.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.Carga.Carregamento>();
            
            string protocoloCargaFechada = string.Empty;
            bool status = true;
            string mensagem = string.Empty;

            if (contexto.Tarefa.Resultado != null)
            {
                if (contexto.Tarefa.Resultado.Contains("protocolo_carga_fechada"))
                {
                    protocoloCargaFechada = contexto.Tarefa.Resultado["protocolo_carga_fechada"].ToString();
                }

                if (string.IsNullOrEmpty(protocoloCargaFechada) && contexto.Tarefa.Resultado.Contains("cargas") && contexto.Tarefa.Resultado["cargas"].IsBsonArray)
                {
                    var cargasArray = contexto.Tarefa.Resultado["cargas"].AsBsonArray;
                    if (cargasArray.Count > 0)
                    {
                        var primeiraCarga = cargasArray[0].AsBsonDocument.FromBsonDocument<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoObjetoTarefa>();
                        
                        protocoloCargaFechada = primeiraCarga?.Protocolo ?? string.Empty;
                        status = primeiraCarga?.Status ?? true;
                        mensagem = primeiraCarga?.Mensagem ?? string.Empty;
                    }
                }
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Comprovei.RequisicaoRetornoGerarCarregamento()
            {
                Protocolo = protocoloCargaFechada,
                Status = status,
                Mensagem = mensagem,
                NumeroCarregamento = carregamento?.NumeroCarregamento ?? string.Empty,
                CamposPersonalizados = carregamento?.CamposPersonalizados
            };
        }
        private void ComprimirImagemAteTamanho(string caminhoArquivo, long tamanhoMaximo)
        {
            try
            {
                int qualidadeImagem = 90;
                byte[] imagemBytes = File.ReadAllBytes(caminhoArquivo);

                using (MemoryStream msInput = new MemoryStream(imagemBytes))
                using (Image imagem = new Bitmap(Image.FromStream(msInput)))
                {
                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                    while (true)
                    {
                        using (MemoryStream msOutput = new MemoryStream())
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualidadeImagem);

                            // Salva a imagem comprimida na memória
                            imagem.Save(msOutput, jpgEncoder, encoderParams);
                            long tamanhoAtual = msOutput.Length;

                            // Se a imagem já está menor que o tamanho desejado, grava no arquivo e sai do loop
                            if (tamanhoAtual <= tamanhoMaximo)
                            {
                                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoArquivo, msOutput.ToArray());
                                break;
                            }

                            qualidadeImagem -= 5;
                            if (qualidadeImagem < 10) break; // Evita que a qualidade fique muito baixa
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro($"Falha ao comprimir a imagem {caminhoArquivo}: {ex.Message}");
                //Segue o fluxo normalmente
            }
        }


        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        private string LimparRespostaComprovei(string jsonRetorno)
        {
            if (string.IsNullOrEmpty(jsonRetorno))
                return jsonRetorno;

            try
            {
                var respostaJson = Newtonsoft.Json.Linq.JToken.Parse(jsonRetorno);
                if (respostaJson["request_body"] != null)
                {
                    respostaJson["request_body"].Parent.Remove();
                    return respostaJson.ToString(Formatting.Indented);
                }
            }
            catch
            {
            }

            return jsonRetorno;
        }

        #endregion Métodos Privados
    }
}