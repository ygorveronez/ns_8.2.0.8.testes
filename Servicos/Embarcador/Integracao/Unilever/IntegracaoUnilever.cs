using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Unilever
{
    public class IntegracaoUnilever
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoUnilever(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null ||
                string.IsNullOrWhiteSpace(configuracaoIntegracao.URLHomologacaoUnileverFourKites) ||
                string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoUnileverFourKites) ||
                string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioUnileverFourKites) ||
                string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaUnileverFourKites))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Unilever Four Kites.";

                repCargaCargaIntegracao.Atualizar(cargaIntegracao);

                return;
            }

            string endpoint = configuracaoIntegracao.URLHomologacaoUnileverFourKites;

            if (cargaIntegracao.Carga?.Empresa?.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                endpoint = configuracaoIntegracao.URLProducaoUnileverFourKites;

            string request = string.Empty;
            string response = string.Empty;
            string mensagemErro = string.Empty;

            bool sucesso = false;

            try
            {
                request = ObterRequestTruckAssignment(cargaIntegracao);

                using (var client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(configuracaoIntegracao.UsuarioUnileverFourKites + ":" + configuracaoIntegracao.SenhaUnileverFourKites)));

                    System.Net.Http.StringContent content = new System.Net.Http.StringContent(request, Encoding.UTF8, "application/json");
                    System.Net.Http.HttpResponseMessage result = client.PostAsync(endpoint, content).Result;

                    response = result.Content.ReadAsStringAsync().Result;

                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(response);

                    string statusCode = response.Contains("statusCode") ? (string)retorno.statusCode : string.Empty;

                    if (result.IsSuccessStatusCode)
                    {
                        if (statusCode == "200")
                        {
                            mensagemErro = statusCode + " - " + (string)retorno.message;
                            sucesso = true;
                        }
                        else
                        {
                            if (response.Contains("errors"))
                                mensagemErro = statusCode + " - " + string.Join(" / ", retorno.errors);
                            else if (response.Contains("error"))
                                mensagemErro = statusCode + " - " + retorno.error;
                        }
                    }
                    else
                    {
                        if (response.Contains("errors"))
                            mensagemErro = statusCode + " - " + string.Join(" / ", retorno.errors);
                        else if (response.Contains("error"))
                            mensagemErro = statusCode + " - " + (string)retorno.error;
                        else
                            mensagemErro = "Problemas ao comunicar com o serviço da Unilever Four Kites.";
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Ocorreu uma falha ao realizar a integração.";
            }

            cargaIntegracao.ProblemaIntegracao = mensagemErro;

            if (sucesso)
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            else
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                Data = cargaIntegracao.DataIntegracao,
                Mensagem = cargaIntegracao.ProblemaIntegracao,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", _unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", _unitOfWork)
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public void IntegrarRetornoCargaPreCalculoFrete(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaPreCalculo repositorioCargaPreCalculo = new Repositorio.Embarcador.Cargas.CargaPreCalculo(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            try
            {
                Servicos.Embarcador.Carga.Frete servicoFrete = new Carga.Frete(_unitOfWork, TipoServicoMultisoftware.MultiEmbarcador);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaDadosTransporteIntegracao.Carga;
                Dominio.Entidades.Embarcador.Cargas.CargaPreCalculo cargaPreCalculo = repositorioCargaPreCalculo.BuscarPorCarga(carga.Codigo);

                cargaDadosTransporteIntegracao.NumeroTentativas++;
                cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

                if (carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Erro && carga.NumeroTentativasRoteirizacaoCarga > 3)
                    throw new ServicoException("Carga aguardando roteirização");

                if (carga.SituacaoCarga == SituacaoCarga.Cancelada)
                    throw new ServicoException("Carga cancelada");

                if (carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Concluido)
                {

                    cargaDadosTransporteIntegracao.ProblemaIntegracao = string.Empty;
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                    if (repositorioAgrupamento.EstaCargaFoiGeradoPorUmAgrupamento(carga.Codigo))
                    {
                        repositorioCargaDadosIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                        return;
                    }

                    bool sucessoExecucao = servicoFrete.ExecutarPreCalculo(ref cargaPreCalculo, _unitOfWork, carga);

                    if (!sucessoExecucao)
                    {
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = cargaPreCalculo?.Observacao ?? string.Empty;
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }

                    repositorioCargaDadosIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                    EnviarRetornoPreCalculo(cargaDadosTransporteIntegracao, carga, sucessoExecucao, cargaPreCalculo.Observacao);
                }
            }
            catch (BaseException ex)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                repositorioCargaDadosIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Problema ao tentar executar precalculo";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                repositorioCargaDadosIntegracao.Atualizar(cargaDadosTransporteIntegracao);
            }
        }

        public void IntegrarDadosValePedagio(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            if (!(cargaDadosTransporteIntegracao?.Carga?.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? false) && cargaDadosTransporteIntegracao.Carga.ValorFrete <= 0m && cargaDadosTransporteIntegracao.Carga?.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
            {
                cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now.AddMinutes(15);
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Carga sem valor de frete";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                repositorioCargaDadosTransporte.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }

            Repositorio.Embarcador.Pedidos.StageAgrupamento respositoriStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;


            if (!respositoriStageAgrupamento.ExisteAgrupamentoDaCargaComVeiculo(cargaDadosTransporteIntegracao.Carga.Codigo) && cargaDadosTransporteIntegracao.Carga.Veiculo == null)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Aguardando dados do veículo.";
                repositorioCargaDadosTransporte.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }


            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (!configuracaoIntegracaoUnilever.IntegrarDadosValePedagio)
                    throw new ServicoException("Integração de dados vale pedagio inativa");

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever))
                    throw new ServicoException("Não existe URL de integração de travamento configurada para a Unilever");


                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(ObterObjetoRequisicaoValePedagio(cargaDadosTransporteIntegracao.Carga), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (resposta?.UpdateFreightResponse?.errorMessage == "Sucesso")
                    {
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração feita com sucesso";
                    }
                    else
                    {
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = resposta.UpdateFreightResponse.errorMessage;
                    }
                }
                else
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração não realizar com sucesso.";
                }

                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");

            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCargaDadosTransporte.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            if (!(cargaDadosTransporteIntegracao?.Carga?.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? false) && cargaDadosTransporteIntegracao.Carga.ValorFrete <= 0m && cargaDadosTransporteIntegracao.Carga?.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
            {
                cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now.AddMinutes(15);
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Carga sem valor de frete";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                repositorioCargaDadosTransporte.Atualizar(cargaDadosTransporteIntegracao);
                return;
            }

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever))
                    throw new ServicoException("Não existe URL de integração de travamento configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(ObterObjetoRequisicao(cargaDadosTransporteIntegracao.Carga.CodigoCargaEmbarcador, cargaDadosTransporteIntegracao.Carga.Veiculo != null), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (resposta?.UpdateFreightResponse?.errorMessage == "Sucesso")
                    {
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração feita com sucesso";
                    }
                    else
                    {
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = resposta.UpdateFreightResponse.errorMessage;
                    }
                }
                else
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Error ao tentar integrar placas.";
                }

                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");
            }
            repositorioCargaDadosTransporte.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarRetornoCargaPreCalculoStages(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool sucesso, string retorno, bool tornarAssincrono = false)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Unilever);

            if (tipoIntegracao == null)
                return;

            if (!(carga.TipoOperacao?.ConfiguracaoCalculoFrete?.ExecutarPreCalculoFrete ?? false))
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, TipoIntegracao.Unilever);

            if (cargaDadosTransporteIntegracao == null)
                cargaDadosTransporteIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao();

            cargaDadosTransporteIntegracao.Carga = carga;

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
            cargaDadosTransporteIntegracao.ProblemaIntegracao = string.Empty;

            if (tornarAssincrono)
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            else
            {
                cargaDadosTransporteIntegracao.NumeroTentativas++;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }

            cargaDadosTransporteIntegracao.TipoIntegracao = tipoIntegracao;

            if (!sucesso)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = retorno;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }

            if (!tornarAssincrono)
                EnviarRetornoPreCalculo(cargaDadosTransporteIntegracao, carga, sucesso, retorno);
        }

        public void IntegrarRertornoCarga(Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Unilever);
            if (tipoIntegracao == null)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = repositorioConfiguracaoIntegracaoUnilever.Buscar();
            if (configuracaoIntegracaoUnilever == null || !configuracaoIntegracaoUnilever.PossuiIntegracaoUnilever || string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoRetornoUnilever))
                return;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;
            string mensagemException = string.Empty;
            string mensagem = string.Empty;

            try
            {
                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoRetornoUnilever;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.RetornoCarga retornoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.RetornoCarga()
                {
                    Protocol = integracao.Carga?.Protocolo ?? 0,
                    shipment = integracao.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    Status = integracao.Sucesso,
                    dataRetorno = integracao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                    Mensagem = integracao.Sucesso ? string.Empty : integracao.Mensagem,
                    SAPRequest = integracao.Carga?.ControleIntegracaoEmbarcador ?? string.Empty,
                    codigoMensagem = integracao.Situacao == SituacaoIntegracao.Integrado ? 200 : integracao.Situacao == SituacaoIntegracao.ProblemaIntegracao ? 300 : 400
                };

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                ValueTuple.Create("client_id", clientID),
                ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(retornoCarga, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);

                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

                jsonResponse = ObterResposta(retornoRequisicao);

                if (!IsRetornoSucesso(retornoRequisicao))
                {
                    mensagem = "Integração de retorno não retornou sucesso.";
                    integracao.SituacaoRetorno = SituacaoIntegracao.ProblemaIntegracao;
                    integracao.MensagemRetorno = mensagem;
                }
                else
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (retorno.StatusCode == 200)
                    {
                        mensagem = retorno.StatusMessage;
                        integracao.SituacaoRetorno = SituacaoIntegracao.Integrado;
                        integracao.MensagemRetorno = mensagem;
                    }
                    else
                    {
                        mensagem = (string)retorno.StatusMessage ?? "Ocorreu uma falha ao obter o retorno da Unilever";
                        integracao.SituacaoRetorno = SituacaoIntegracao.ProblemaIntegracao;
                        integracao.MensagemRetorno = mensagem;
                    }
                }

                repIntegradoraIntegracaoRetorno.Atualizar(integracao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemException = ex.Message;
                integracao.SituacaoRetorno = SituacaoIntegracao.ProblemaIntegracao;
                integracao.MensagemRetorno = mensagemException.Length > 250 ? mensagemException.Substring(0, 250) : mensagemException;
                repIntegradoraIntegracaoRetorno.Atualizar(integracao);
            }

            Dominio.Entidades.WebService.IntegradoraIntegracaoRetornoArquivo arquivoIntegracao = new Dominio.Entidades.WebService.IntegradoraIntegracaoRetornoArquivo();
            arquivoIntegracao.Data = DateTime.Now;
            arquivoIntegracao.Mensagem = !string.IsNullOrWhiteSpace(mensagemException) ? mensagemException.Length > 400 ? mensagemException.Substring(0, 400) : mensagemException : mensagem.Length > 400 ? mensagem.Substring(0, 400) : mensagem;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", _unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", _unitOfWork);

            Repositorio.WebService.IntegradoraIntegracaoRetornoArquivo repIntegradoraIntegracaoRetornoArquivo = new Repositorio.WebService.IntegradoraIntegracaoRetornoArquivo(_unitOfWork);
            repIntegradoraIntegracaoRetornoArquivo.Inserir(arquivoIntegracao);

            integracao.ArquivosIntegracaoRetorno.Add(arquivoIntegracao);
            repIntegradoraIntegracaoRetorno.Atualizar(integracao);
        }

        public void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracaoPendente)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever integracaoUnilever = repositorioUnilever.BuscarPrimeiroRegistro();

            if (integracaoUnilever == null || !integracaoUnilever.PossuiIntegracaoUnilever || string.IsNullOrEmpty(integracaoUnilever.URLIntegracaoAvancoParaEmissao))
                return;

            ocorrenciaCTeIntegracaoPendente.NumeroTentativas++;
            ocorrenciaCTeIntegracaoPendente.DataIntegracao = DateTime.Now;

            string jsonRequest = "";
            string jsonResponse = "";

            var pedidos = ocorrenciaCTeIntegracaoPendente?.CargaCTe?.Carga?.Pedidos?.FirstOrDefault();
            string stageEnvia = pedidos?.Pedido?.StagesPedido?.FirstOrDefault()?.Stage.NumeroStage ?? string.Empty;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = ocorrenciaCTeIntegracaoPendente?.CargaCTe?.Carga;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.DOCUMENT documentoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.DOCUMENT()
            {
                SAPDOCTYPE = (carga?.CargaGeradaViaDocumentoTransporte ?? false) || carga?.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga ? "S" : "O",
                BASE64FILE = "",
                DOCISSUANCE = "Y",
                STATUS = "100",
                NumeroCarga = ocorrenciaCTeIntegracaoPendente?.CargaOcorrencia?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                STAGE = stageEnvia,
                FISDOCKEY = ocorrenciaCTeIntegracaoPendente?.CargaCTe?.CTe?.Chave ?? string.Empty,
                FISDOCTYPE = ObterFISDOCTYPE(ocorrenciaCTeIntegracaoPendente?.CargaCTe?.CTe ?? null),
                REFDOCKEY = ocorrenciaCTeIntegracaoPendente?.CargaCTe?.CargaCTeComplementoInfo?.CTeComplementado?.Chave ?? string.Empty,
                UNIDOCS = ""
            };
            var headersIntegracao = new List<(string Chave, string Valor)>() {
                ValueTuple.Create("client_id", integracaoUnilever.ClientIDIntegracaoUnilever),
                ValueTuple.Create("client_secret", integracaoUnilever.ClientSecretIntegracaoUnilever)
                };

            dynamic DOCUMENT = new
            {
                DOCUMENT = documentoRequest
            };

            jsonRequest = JsonConvert.SerializeObject(DOCUMENT, Formatting.Indented);

            WebRequest requisicao = CriaRequisicao(integracaoUnilever.URLIntegracaoAvancoParaEmissao, "POST", jsonRequest, headersIntegracao);
            HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
            jsonResponse = ObterResposta(retornoRequisicao);
            dynamic response = JsonConvert.DeserializeObject(jsonResponse);

            if (!IsRetornoSucesso(jsonResponse))
            {
                ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = response["return"].errorMessage;
            }
            else
            {
                ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = "";
                ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracaoPendente, jsonRequest, jsonResponse, "json");
        }

        public void IntegrarCancelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao cancelamentoOcorrencia)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repOcorrenciaCTeCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever integracaoUnilever = repositorioUnilever.BuscarPrimeiroRegistro();

            if (integracaoUnilever == null || !integracaoUnilever.PossuiIntegracaoUnilever || string.IsNullOrEmpty(integracaoUnilever.URLIntegracaoAvancoParaEmissao))
                return;

            cancelamentoOcorrencia.NumeroTentativas += 1;
            cancelamentoOcorrencia.DataIntegracao = DateTime.Now;

            string jsonRequest = "";
            string jsonResponse = "";

            try
            {
                var pedidos = cancelamentoOcorrencia?.OcorrenciaCTeIntegracao?.CargaCTe?.Carga?.Pedidos?.FirstOrDefault();
                string stageEnvia = pedidos?.Pedido?.StagesPedido?.FirstOrDefault()?.Stage.NumeroStage ?? string.Empty;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cancelamentoOcorrencia?.OcorrenciaCTeIntegracao?.CargaCTe?.Carga;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.DOCUMENT documentoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.DOCUMENT()
                {
                    SAPDOCTYPE = (carga?.CargaGeradaViaDocumentoTransporte ?? false) || carga?.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga ? "S" : "O",
                    BASE64FILE = "",
                    DOCISSUANCE = "Y",
                    STATUS = "100",
                    STAGE = stageEnvia,
                    NumeroCarga = cancelamentoOcorrencia?.OcorrenciaCTeIntegracao?.CargaCTe?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    FISDOCKEY = cancelamentoOcorrencia?.OcorrenciaCTeIntegracao?.CargaCTe?.CTe?.Chave ?? string.Empty,
                    REFDOCKEY = cancelamentoOcorrencia?.OcorrenciaCTeIntegracao?.CargaCTe?.CargaCTeComplementoInfo?.CTeComplementado?.Chave ?? string.Empty,
                    FISDOCTYPE = ObterFISDOCTYPE(cancelamentoOcorrencia?.OcorrenciaCTeIntegracao?.CargaCTe?.CTe ?? null),
                    UNIDOCS = ""
                };
                var headersIntegracao = new List<(string Chave, string Valor)>() {
                ValueTuple.Create("client_id", integracaoUnilever.ClientIDIntegracaoUnilever),
                ValueTuple.Create("client_secret", integracaoUnilever.ClientSecretIntegracaoUnilever)
                };

                dynamic DOCUMENT = new
                {
                    DOCUMENT = documentoRequest
                };

                jsonRequest = JsonConvert.SerializeObject(DOCUMENT, Formatting.Indented);

                WebRequest requisicao = CriaRequisicao(integracaoUnilever.URLIntegracaoAvancoParaEmissao, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);
                dynamic response = JsonConvert.DeserializeObject(jsonResponse);

                if (!IsRetornoSucesso(jsonResponse))
                {
                    cancelamentoOcorrencia.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cancelamentoOcorrencia.ProblemaIntegracao = response["return"].errorMessage;
                }
                else
                {
                    cancelamentoOcorrencia.ProblemaIntegracao = "";
                    cancelamentoOcorrencia.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                cancelamentoOcorrencia.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoOcorrencia.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Unilever";
            }

            repOcorrenciaCTeCancelamentoIntegracao.Atualizar(cancelamentoOcorrencia);
            servicoArquivoTransacao.Adicionar(cancelamentoOcorrencia, jsonRequest, jsonResponse, "json");
        }

        public void IntegrarCargaFrete(Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe)
        {
            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(_unitOfWork);

            if (cargaFreteIntegracao.Carga.ValorFrete <= 0m)
            {
                cargaFreteIntegracao.DataIntegracao = DateTime.Now.AddMinutes(15);
                cargaFreteIntegracao.ProblemaIntegracao = "Carga sem valor de frete";
                cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                repositorioCargaFreteIntegracao.Atualizar(cargaFreteIntegracao);
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTms = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequest = "";
            string jsonResponse = "";
            cargaFreteIntegracao.DataIntegracao = DateTime.Now;
            cargaFreteIntegracao.NumeroTentativas++;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoAvancoParaEmissao))
                    throw new ServicoException("Url para integração não configurada");

                var listaCargaPedido = repositorioCargaPedido.BuscarPorCarga(cargaFreteIntegracao.Carga.Codigo);

                Dominio.Entidades.Embarcador.Pedidos.Stage exiteStage = repositorioStage.BuscarPrimeiraPorCarga(cargaFreteIntegracao.Carga.Codigo);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.DOCUMENT documentoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.DOCUMENT()
                {
                    SAPDOCTYPE = (cargaFreteIntegracao.Carga?.CargaGeradaViaDocumentoTransporte ?? false) || cargaFreteIntegracao.Carga?.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga ? "S" : "O",
                    BASE64FILE = "",
                    DOCISSUANCE = "Y",
                    STATUS = "",
                    NumeroCarga = cargaFreteIntegracao.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    STAGE = !string.IsNullOrEmpty(cargaFreteIntegracao?.Stage?.NumeroStage) ? cargaFreteIntegracao?.Stage?.NumeroStage : exiteStage != null ? exiteStage.NumeroStage : string.Empty,
                    FISDOCKEY = "",
                    FISDOCTYPE = "",
                    REFDOCKEY = "",
                    UNIDOCS = ""
                };

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                ValueTuple.Create("client_id", configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever),
                ValueTuple.Create("client_secret", configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever)
                };

                dynamic DOCUMENT = new
                {
                    DOCUMENT = documentoRequest
                };

                jsonRequest = JsonConvert.SerializeObject(DOCUMENT, Formatting.Indented);
                WebRequest requisicao = CriaRequisicao(configuracaoIntegracaoUnilever.URLIntegracaoAvancoParaEmissao, "POST", jsonRequest, headersIntegracao);

                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

                jsonResponse = ObterResposta(retornoRequisicao);

                dynamic response = JsonConvert.DeserializeObject(jsonResponse);

                if (!IsRetornoSucesso(jsonResponse))
                {
                    Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);
                    servicoMensagemAlerta.Adicionar(cargaFreteIntegracao.Carga, TipoMensagemAlerta.ProblemaComTipoOperacao, $"Por que não foi encontrada uma regra valida para um tipo operação");
                    cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = true;
                    cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
                    cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaFreteIntegracao.ProblemaIntegracao = response["return"].errorMessage;
                }
                else
                {
                    cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = false;
                    cargaFreteIntegracao.Carga.LiberadaComPendenciaIntegracaoFrete = false;
                    cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
                    cargaFreteIntegracao.ProblemaIntegracao = "Integração Feita com sucesso";
                    cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }
                servicoArquivoTransacao.Adicionar(cargaFreteIntegracao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException exception)
            {
                cargaFreteIntegracao.ProblemaIntegracao = exception.Message;
                cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception exception)
            {
                Log.TratarErro(exception);
                cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = true;
                cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
                cargaFreteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                servicoArquivoTransacao.Adicionar(cargaFreteIntegracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCarga.Atualizar(cargaFreteIntegracao.Carga);
            repositorioCargaFreteIntegracao.Atualizar(cargaFreteIntegracao);

            if (repositorioCargaFreteIntegracao.ExisteIntegracoesCargaFreteParaEstaCarga(cargaFreteIntegracao.Carga.Codigo).Count > 0)
            {
                repositorioCargaFreteIntegracao.Atualizar(cargaFreteIntegracao);
            }

            if (cargaFreteIntegracao.Carga.SituacaoCarga == SituacaoCarga.AgNFe && cargaFreteIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                Servicos.Embarcador.Carga.Carga.AvancarEtapaDocumentosEmissaoCarga(out string erro, cargaFreteIntegracao.Carga.Codigo, false, repositorioConfiguracaoTms.BuscarConfiguracaoPadrao(), tipoServicoMultisoftware, unitOfWork, null, null, auditado, true);
            //else if (cargaFreteIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)//nao entendi pq disso...
            //    cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = true;

            new Servicos.Embarcador.Carga.Carga(unitOfWork).LiberarEtapaEmisao(cargaFreteIntegracao.Carga, unitOfWork, auditado, tipoServicoMultisoftware, webServiceConsultaCTe, out string messagem);

        }

        public void IntegrarCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            cargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeIntegracao.NumeroTentativas++;

            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositorioCargCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = repositorioConfiguracaoIntegracaoUnilever.Buscar();
            if (configuracaoIntegracaoUnilever == null || !configuracaoIntegracaoUnilever.PossuiIntegracaoUnilever || string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoAvancoParaEmissao))
                return;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoAvancoParaEmissao;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.IntegracaoCTe dadosRequisicao = ObterDadosIntegracaoCTeEmissaoInicial(cargaCTeIntegracao.CargaCTe);

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);

                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

                jsonResponse = ObterResposta(retornoRequisicao);

                if (!IsRetornoSucesso(retornoRequisicao))
                {
                    cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaCTeIntegracao.ProblemaIntegracao = "Integração de retorno não retornou sucesso.";
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.RetornoAvancoEmissao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.RetornoAvancoEmissao>(jsonResponse);

                    if (retorno.Return.ErrorCode == 105)
                    {
                        cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaCTeIntegracao.ProblemaIntegracao = retorno.Return.ErrorMessage ?? "Status atualizado.";
                    }
                    else
                    {
                        cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaCTeIntegracao.ProblemaIntegracao = retorno.Return.ErrorMessage ?? "Ocorreu uma falha ao obter o retorno da Unilever";
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = ex.Message;
            }

            servicoArquivoTransacao.Adicionar(cargaCTeIntegracao, jsonRequest, jsonResponse, "json");
            repositorioCargCTeIntegracao.Atualizar(cargaCTeIntegracao);
        }

        public void IntegrarCancelamentoCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cancelamentoCargaCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repositorioCargCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(_unitOfWork);
            cancelamentoCargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cancelamentoCargaCTeIntegracao.NumeroTentativas++;

            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = repositorioConfiguracaoIntegracaoUnilever.Buscar();

            if (configuracaoIntegracaoUnilever == null || !configuracaoIntegracaoUnilever.PossuiIntegracaoUnilever || string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever))
                return;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.IntegracaoCTe dadosRequisicao = null;

                if (((cancelamentoCargaCTeIntegracao.CargaCTe.Carga.TipoOperacao?.ConfiguracaoCarga?.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Documentos) && cancelamentoCargaCTeIntegracao.CargaCancelamento.CTe != null)
                    dadosRequisicao = ObterDadosIntegracaoCTeEmissaoCancelamento(cancelamentoCargaCTeIntegracao.CargaCTe.Carga.Codigo, cancelamentoCargaCTeIntegracao.CargaCTe.Carga.CodigoCargaEmbarcador, cancelamentoCargaCTeIntegracao.CargaCTe.CTe?.Chave ?? "", cancelamentoCargaCTeIntegracao.CargaCTe.CTe?.Codigo ?? 0, "");
                else
                    dadosRequisicao = ObterDadosIntegracaoCTeEmissaoInicial(cancelamentoCargaCTeIntegracao.CargaCTe);

                List<(string Chave, string Valor)> headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);

                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

                jsonResponse = ObterResposta(retornoRequisicao);

                if (!IsRetornoSucesso(retornoRequisicao))
                {
                    cancelamentoCargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cancelamentoCargaCTeIntegracao.ProblemaIntegracao = "Integração de retorno não retornou sucesso.";
                }
                else
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (retorno.StatusCode == 200)
                    {
                        cancelamentoCargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cancelamentoCargaCTeIntegracao.ProblemaIntegracao = retorno.StatusMessage;
                    }
                    else
                    {
                        cancelamentoCargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cancelamentoCargaCTeIntegracao.ProblemaIntegracao = (string)retorno.StatusMessage ?? "Ocorreu uma falha ao obter o retorno da Unilever";
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cancelamentoCargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoCargaCTeIntegracao.ProblemaIntegracao = ex.Message;
            }

            servicoArquivoTransacao.Adicionar(cancelamentoCargaCTeIntegracao, jsonRequest, jsonResponse, "json");
            repositorioCargCTeIntegracao.Atualizar(cancelamentoCargaCTeIntegracao);
        }

        public void IntegrarCancelamentoCargaCte(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaIntegracao.NumeroTentativas++;

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoCte = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);


            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = repositorioConfiguracaoIntegracaoUnilever.Buscar();

            if (configuracaoIntegracaoUnilever == null || !configuracaoIntegracaoUnilever.PossuiIntegracaoUnilever || string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever))
                return;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoAvancoParaEmissao;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.IntegracaoCTe dadosRequisicao = null;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga;

                if (cargaCancelamentoCargaIntegracao.CargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos)
                {
                    if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
                        carga = repositorioCargaCte.BuscarCargaFilhaRetornarPorCte(cargaCancelamentoCargaIntegracao.CargaCancelamento.CTe.Codigo, carga.Codigo);

                    dadosRequisicao = ObterDadosIntegracaoCTeEmissaoCancelamento(carga?.Codigo ?? 0, carga?.CodigoCargaEmbarcador ?? "", cargaCancelamentoCargaIntegracao.CargaCancelamento.CTe.Chave, cargaCancelamentoCargaIntegracao.CargaCancelamento.CTe.Codigo, cargaCancelamentoCargaIntegracao.Stage?.NumeroStage ?? string.Empty);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCtePrincipal = repositorioCargaPedidoCte.BuscarCargaCtePorStage(cargaCancelamentoCargaIntegracao.Stage.Codigo);
                    dadosRequisicao = ObterDadosIntegracaoCTeEmissaoCancelamento(carga?.Codigo ?? 0, carga?.CodigoCargaEmbarcador ?? "", cargaCtePrincipal?.CTe?.Chave ?? string.Empty, cargaCtePrincipal?.CTe?.Codigo ?? 0, cargaCancelamentoCargaIntegracao?.Stage?.NumeroStage ?? string.Empty);
                }

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);

                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

                jsonResponse = ObterResposta(retornoRequisicao);

                if (!IsRetornoSucesso(retornoRequisicao))
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    string mesagem = string.Empty;

                    if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                        mesagem = "Error ao tentar comunicar com o servidor";
                    else if (retorno["error"] != null)
                        mesagem = retorno["error"];
                    else
                        mesagem = retorno["return"]["errorMessage"];

                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mesagem ?? "Integração de retorno não retornou sucesso.";
                }
                else
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    string mesagem = string.Empty;

                    if (retorno["error"] != null)
                        mesagem = retorno["error"];
                    else
                        mesagem = retorno["return"]["errorMessage"];

                    if (retorno.StatusCode == 200 || retorno["return"]["errorCode"] == 105)
                    {
                        cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mesagem ?? "Ocorreu uma falha ao obter o retorno da Unilever";
                    }
                    else
                    {
                        cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mesagem ?? "Ocorreu uma falha ao obter o retorno da Unilever";
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = ex.Message;
            }

            servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, jsonRequest, jsonResponse, "json");
            repositorioCargCTeIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        public void IntegrarProvisao(Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao provisaoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            provisaoIntegracao.NumeroTentativas += 1;
            provisaoIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoProvisaoUnilever))
                    throw new ServicoException("Não existe URL de integração de provisão configurada para a Unilever");

                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repositorioDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(_unitOfWork);
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentosContabeis = repositorioDocumentoContabil.BuscarPorProvisao(provisaoIntegracao.Provisao.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.ProvisaoStage> stagesEnviar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.ProvisaoStage>();

                IEnumerable<IGrouping<(int, string), Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>> documentosContabeisAgrupadosPorStage = documentosContabeis
                    .GroupBy(documento => ValueTuple.Create(documento.Stage?.Codigo ?? 0, documento.Stage?.NumeroStage ?? string.Empty));

                foreach (IGrouping<(int CodigoStage, string NumeroStage), Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> agrupamentoDocumentosContabeisPorStage in documentosContabeisAgrupadosPorStage)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.ProvisaoDocumentoContabil> documentoContabeisEnviar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.ProvisaoDocumentoContabil>();

                    IEnumerable<IGrouping<(string, TipoContaContabil), Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>> documentosContabeisAgrupadosPorStageETipo = agrupamentoDocumentosContabeisPorStage
                        .GroupBy(documento => ValueTuple.Create(documento.PlanoConta?.PlanoContabilidade ?? string.Empty, documento.TipoContaContabil));

                    foreach (IGrouping<(string PlanoContabilidade, TipoContaContabil TipoContaContabil), Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> agrupamentoDocumentosContabeisPorStageETipo in documentosContabeisAgrupadosPorStageETipo)
                    {
                        decimal valorContabilizacao = agrupamentoDocumentosContabeisPorStageETipo.Sum(o => o.ValorContabilizacao);

                        if (valorContabilizacao <= 0m)
                            continue;

                        documentoContabeisEnviar.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.ProvisaoDocumentoContabil()
                        {
                            Item = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.ProvisaoDocumentoContabilItem()
                            {
                                Tipo = string.IsNullOrWhiteSpace(agrupamentoDocumentosContabeisPorStageETipo.Key.PlanoContabilidade) ? agrupamentoDocumentosContabeisPorStageETipo.Key.TipoContaContabil.ObterDescricao() : agrupamentoDocumentosContabeisPorStageETipo.Key.PlanoContabilidade,
                                Valor = string.Format(CultureInfo.InvariantCulture, "{0:0.00}", valorContabilizacao)
                            }
                        });
                    }

                    Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil primeiroDocumentoContabil = agrupamentoDocumentosContabeisPorStage.OrderBy(documento => documento.ImpostoValorAgregado != null).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.ProvisaoStage stageEnviar = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.ProvisaoStage()
                    {
                        Numero = agrupamentoDocumentosContabeisPorStage.Key.NumeroStage,
                        ImpostoValorAgregado = primeiroDocumentoContabil.ImpostoValorAgregado?.CodigoIVA ?? "",
                        KOMOK = "",
                        DocumentosContabeis = documentoContabeisEnviar.ToArray()
                    };

                    stagesEnviar.Add(stageEnviar);
                }

                Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.Provisao provisaoEnviar = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.Provisao()
                {
                    NumeroCarga = provisaoIntegracao.Provisao.Carga.CodigoCargaEmbarcador,
                    Stages = stagesEnviar.ToArray()
                };

                jsonRequisicao = JsonConvert.SerializeObject(provisaoEnviar, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                HttpClient cliente = ObterCliente(configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever, configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever, "json");
                HttpResponseMessage retornoRequisicao = cliente.PostAsync(configuracaoIntegracaoUnilever.URLIntegracaoProvisaoUnilever, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if ((retornoRequisicao.StatusCode == HttpStatusCode.OK) || (retornoRequisicao.StatusCode == HttpStatusCode.Created))
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                    provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                    provisaoIntegracao.ProblemaIntegracao = retorno.StatusMessage;
                }
                else
                {
                    provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    provisaoIntegracao.ProblemaIntegracao = $"Ocorreu uma falha ao realizar a integração com a Unilever.";
                }

                servicoArquivoTransacao.Adicionar(provisaoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                provisaoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                provisaoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Unilever.";

                servicoArquivoTransacao.Adicionar(provisaoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioProvisaoIntegracao.Atualizar(provisaoIntegracao);
        }

        public void IntegrarPagamento(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            pagamentoIntegracao.NumeroTentativas += 1;
            pagamentoIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoLotePagamento))
                    throw new ServicoException("Não existe URL de integração de pagamento configurada para a Unilever");

                if (pagamentoIntegracao.DocumentoFaturamento == null || pagamentoIntegracao.DocumentoFaturamento.CTe == null)
                    throw new ServicoException("Não existe documento para a integração");

                dynamic objEnvio = new { RequestInvoiceProcessing = ObterRequestPagamento(pagamentoIntegracao.DocumentoFaturamento, false) };

                jsonRequisicao = JsonConvert.SerializeObject(objEnvio, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                HttpClient cliente = ObterCliente(configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever, configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever, "json");
                HttpResponseMessage retornoRequisicao = cliente.PostAsync(configuracaoIntegracaoUnilever.URLIntegracaoLotePagamento, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.IsSuccessStatusCode)
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                    pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                    pagamentoIntegracao.ProblemaIntegracao = $"Aguardo Retorno da Unilever";
                }
                else
                {
                    pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    pagamentoIntegracao.ProblemaIntegracao = $"Ocorreu uma falha ao realizar a integração com a Unilever.";
                }

                servicoArquivoTransacao.Adicionar(pagamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (ServicoException excecao)
            {
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Unilever.";

                servicoArquivoTransacao.Adicionar(pagamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repPagamentoIntegracao.Atualizar(pagamentoIntegracao);
        }

        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaCancelamentoIntegracao.NumeroTentativas++;
            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoRetornoUnilever))
                    throw new ServicoException("Não existe configuração de integração disponível para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoCancelamento;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.RetornoCarga retornoCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.RetornoCarga()
                {
                    Protocol = cargaCancelamentoIntegracao.CargaCancelamento?.Carga?.Protocolo ?? 0,
                    shipment = cargaCancelamentoIntegracao.CargaCancelamento?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    Status = true,
                    dataRetorno = cargaCancelamentoIntegracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                    Mensagem = "",
                    SAPRequest = cargaCancelamentoIntegracao.CargaCancelamento?.ControleIntegracaoEmbarcador ?? string.Empty,
                    codigoMensagem = 200
                };

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(retornoCarga, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);

                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);

                jsonResponse = ObterResposta(retornoRequisicao);

                if (!IsRetornoSucesso(retornoRequisicao))
                {
                    cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Integração de retorno não retornou sucesso.";
                }
                else
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (retorno.StatusCode == 200)
                    {
                        cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaCancelamentoIntegracao.ProblemaIntegracao = retorno.StatusMessage;
                    }
                    else
                    {
                        cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaCancelamentoIntegracao.ProblemaIntegracao = retorno.StatusMessage;
                    }
                }
                servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException exception)
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = exception.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = ex.Message.Length > 250 ? ex.Message.Substring(0, 250) : ex.Message;
                servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, jsonRequest, jsonResponse, "json");
            }
            repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
        }

        public string ConfirmarPlacasPreChekin(Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao cargaPreChekinIntegracao, bool permitirIntegrarComFalha, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento respositorioAgrupamentoStage = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTms = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaDt = repositorioCarga.BuscarPorCodigoFetch(cargaPreChekinIntegracao.Carga.Codigo);

            string msgRetorno = "Placas Confirmadas com sucesso!";

            if (!permitirIntegrarComFalha)
            {
                IntegrarPreChekin(cargaPreChekinIntegracao, true);

                if (cargaPreChekinIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                    return msgRetorno;
            }

            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stageAgrupamentos = respositorioAgrupamentoStage.BuscarPorCargaDt(cargaPreChekinIntegracao.Carga.Codigo);

            bool enviarIntegracaoTrasnferenciaEntrega = true;
            foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento in stageAgrupamentos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = repStage.BuscarporAgrupamento(agrupamento.Codigo);
                List<Vazio> tipoPercusoStages = stages.Select(s => s.TipoPercurso).ToList();

                if (tipoPercusoStages.Any(s => s == Vazio.PercursoPrincipal))
                {
                    //TRANSFERENCIA
                    if (agrupamento.CargaGerada != null)
                    {
                        if (repCargaCTe.BuscarPrimeirPorCarga(agrupamento.CargaGerada.Codigo) != null)
                        {
                            //ja emitiu cte
                            if (repCargaCTe.CargaPossuiCtesInviaveisParaIntegracao(cargaPreChekinIntegracao.Carga.Codigo))
                            {
                                enviarIntegracaoTrasnferenciaEntrega = false;
                                msgRetorno = "Placas para cargas de coletas podem ser confirmadas, mas não é possível confirmar as placas para cargas de Transferencia e Entrega enquanto os CTes não forem confirmados ou recusados";
                                continue;
                            }
                        }
                    }
                }

                if (tipoPercusoStages.Any(s => s == Vazio.PercursoSubSeQuente))
                {
                    //ENTREGA
                    if (agrupamento.CargaGerada != null)
                    {
                        if (repCargaCTe.BuscarPrimeirPorCarga(agrupamento.CargaGerada.Codigo) != null)
                        {
                            //ja emitiu cte
                            if (repCargaCTe.CargaPossuiCtesInviaveisParaIntegracao(cargaPreChekinIntegracao.Carga.Codigo))
                            {
                                enviarIntegracaoTrasnferenciaEntrega = false;
                                msgRetorno = "Placas para cargas de coletas podem ser confirmadas, mas não é possível confirmar as placas para cargas de Transferencia e Entrega enquanto os CTes não forem confirmados ou recusados";
                                continue;
                            }

                        }
                    }
                }

                if (agrupamento.Veiculo != null)
                {
                    agrupamento.PlacasConfirmadas = true;
                    respositorioAgrupamentoStage.Atualizar(agrupamento);
                }

                if (agrupamento.CargaGerada == null)
                    continue;

                if (!serCarga.VerificarSeCargaEstaNaLogistica(agrupamento.CargaGerada, tipoServicoMultisoftware))
                {
                    if (agrupamento.CargaGerada.Veiculo != null && agrupamento.CargaGerada.Motoristas?.Count >= 0)
                    {
                        agrupamento.MensagemRetornoDadosFrete = $"A Atual situação da carga ({agrupamento.CargaGerada.DescricaoSituacaoCarga}) não permite essa alteração de placas.";
                        continue;
                    }
                    else
                        agrupamento.MensagemRetornoDadosFrete = "";
                }

                bool existeMotoristaNaCarga = agrupamento.CargaGerada.Motoristas.Any(m => m.Codigo == agrupamento?.Motorista?.Codigo);

                if (!existeMotoristaNaCarga && agrupamento?.Motorista != null)
                    agrupamento.CargaGerada.Motoristas.Add(agrupamento.Motorista);

                bool existeVeiculo = agrupamento.Veiculo != null && agrupamento.CargaGerada.Veiculo == agrupamento.Veiculo ? true : false;

                if (!existeVeiculo && agrupamento.Veiculo != null)
                    agrupamento.CargaGerada.Veiculo = agrupamento.Veiculo;

                if (agrupamento.CargaGerada.VeiculosVinculados == null)
                    agrupamento.CargaGerada.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                agrupamento.CargaGerada.VeiculosVinculados.Clear();

                if (agrupamento.Reboque != null)
                {
                    if (!agrupamento.CargaGerada.VeiculosVinculados.Any(v => v.Codigo == agrupamento.Reboque.Codigo))
                        agrupamento.CargaGerada.VeiculosVinculados.Add(agrupamento.Reboque);
                }

                if (agrupamento.SegundoReboque != null)
                {
                    if (!agrupamento.CargaGerada.VeiculosVinculados.Any(v => v.Codigo == agrupamento.SegundoReboque.Codigo))
                        agrupamento.CargaGerada.VeiculosVinculados.Add(agrupamento.SegundoReboque);
                }

                respositorioAgrupamentoStage.Atualizar(agrupamento);
                repositorioCarga.Atualizar(agrupamento.CargaGerada);

                InformarDadosTransporteCargaGerada(agrupamento.CargaGerada, agrupamento, serCarga, auditado, _unitOfWork);

                //if (!string.IsNullOrEmpty(msgErro))
                //    throw new ServicoException(msgErro);
            }

            //List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(cargaDt.Codigo);
            //Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarIntegracoesCargaDadosTransporte(cargaDt, cargaMotoristas, repositorioConfiguracaoTms.BuscarConfiguracaoPadrao(), tipoServicoMultisoftware, _unitOfWork);

            if (!repCargaCTe.CargaPossuiCtesInviaveisParaIntegracao(cargaDt.Codigo) && cargaDt.CargaEmitidaParcialmente)
                cargaDt.CargaEmitidaParcialmente = false;
            else
                cargaDt.CargaEmitidaParcialmente = true;

            repositorioCarga.Atualizar(cargaDt);

            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPreChekinIntegracao.Carga, null, "Operador confirmou Placas de pre-chekin.", _unitOfWork);

            GerarIntegracoes(cargaPreChekinIntegracao.Carga, stageAgrupamentos, false, enviarIntegracaoTrasnferenciaEntrega);

            return msgRetorno;
        }

        public void RemoverPlacasPreChekin(Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao cargaPreChekinIntegracao, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTms = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            if (!PermiteGerarIntegracaoPreChekin(cargaPreChekinIntegracao.Carga.SituacaoCarga))
                throw new ServicoException("A situação da carga não permite remover placas");

            IntegrarPreChekin(cargaPreChekinIntegracao, false);

            if (cargaPreChekinIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                return;

            Repositorio.Embarcador.Pedidos.StageAgrupamento respositorioAgrupamentoStage = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stageAgrupamentos = respositorioAgrupamentoStage.BuscarPorCargaDt(cargaPreChekinIntegracao.Carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento in stageAgrupamentos)
            {
                agrupamento.Veiculo = null;
                agrupamento.Motorista = null;
                agrupamento.Reboque = null;
                agrupamento.SegundoReboque = null;
                respositorioAgrupamentoStage.Atualizar(agrupamento);
            }

            GerarIntegracoes(cargaPreChekinIntegracao.Carga, null, true);

            //List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(cargaPreChekinIntegracao.Carga.Codigo);
            //Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarIntegracoesCargaDadosTransporte(cargaPreChekinIntegracao.Carga, cargaMotoristas, repositorioConfiguracaoTms.BuscarConfiguracaoPadrao(), tipoServicoMultisoftware, _unitOfWork);

        }

        public void GerarTransportesCargasPreChekin(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento respositorioAgrupamentoStage = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stageAgrupamentos = respositorioAgrupamentoStage.BuscarPorCargaDt(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento in stageAgrupamentos)
            {
                if (agrupamento.CargaGerada == null)
                    continue;

                bool existeMotoristaNaCarga = agrupamento.CargaGerada.Motoristas.Any(m => m.Codigo == agrupamento?.Motorista?.Codigo);

                if (!existeMotoristaNaCarga && agrupamento?.Motorista != null)
                    agrupamento.CargaGerada.Motoristas.Add(agrupamento.Motorista);

                bool existeVeiculo = agrupamento.Veiculo != null && agrupamento.CargaGerada.Veiculo == agrupamento.Veiculo ? true : false;

                if (!existeVeiculo && agrupamento.Veiculo != null)
                    agrupamento.CargaGerada.Veiculo = agrupamento.Veiculo;

                respositorioAgrupamentoStage.Atualizar(agrupamento);
                repositorioCarga.Atualizar(agrupamento.CargaGerada);
            }

            //aqui caso a carga é consolidacao == preCheckin devemos gerar as cargas dos agrupamentos (processo do inbound)
            if (carga.TipoOperacao != null && carga.TipoOperacao.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                serCarga.AutorizarGeracaoCargaStageAgrupamentoPreChekin(carga, _unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, "Operador confirmou a geração dos transportes.", _unitOfWork);
            }

        }

        public void IntegrarLinkNotas(Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe)
        {
            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(_unitOfWork);

            if (cargaFreteIntegracao.Carga.ValorFrete <= 0m)
            {
                cargaFreteIntegracao.DataIntegracao = DateTime.Now;
                repositorioCargaFreteIntegracao.Atualizar(cargaFreteIntegracao);
            }

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaFreteIntegracao.NumeroTentativas++;
            cargaFreteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever))
                    throw new ServicoException("Não existe URL de integração de travamento configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(ObterObjetoRequisicaoConfirmacaoPlacaComLinkNotasERelevanciaCusto(cargaFreteIntegracao.Carga, cargaFreteIntegracao.Stage), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (resposta?.UpdateFreightResponse?.errorMessage == "Sucesso")
                    {
                        cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaFreteIntegracao.ProblemaIntegracao = "Integração feita com sucesso";
                        cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = false;
                        cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
                        cargaFreteIntegracao.Carga.LiberadaComPendenciaIntegracaoFrete = false;
                    }
                    else
                    {
                        cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaFreteIntegracao.ProblemaIntegracao = resposta.UpdateFreightResponse.errorMessage;
                        cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = true;
                        cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
                    }
                }
                else
                {
                    cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaFreteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar notas";
                    cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = true;
                    cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
                }

                servicoArquivoTransacao.Adicionar(cargaFreteIntegracao, jsonRequest, jsonResponse, "json");

                if (cargaFreteIntegracao.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                    new Servicos.Embarcador.Carga.Carga(unitOfWork).LiberarEtapaEmisao(cargaFreteIntegracao.Carga, _unitOfWork, auditado, tipoServicoMultisoftware, webServiceConsultaCTe, out string messagem);

            }
            catch (ServicoException excecao)
            {
                cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaFreteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaFreteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";

                servicoArquivoTransacao.Adicionar(cargaFreteIntegracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCargaFreteIntegracao.Atualizar(cargaFreteIntegracao);
        }

        public void IntegrarRelevanciCustos(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoCarga.NumeroTentativas++;
            integracaoCarga.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever))
                    throw new ServicoException("Não existe URL de integração de travamento configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(ObterObjetoRequisicaoConfirmacaoPlacaComLinkNotasERelevanciaCusto(integracaoCarga.Carga, integracaoCarga.Stage), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (resposta?.UpdateFreightResponse?.errorMessage == "Sucesso")
                    {
                        integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        integracaoCarga.ProblemaIntegracao = "Integração feita com sucesso";
                    }
                    else
                    {
                        integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracaoCarga.ProblemaIntegracao = resposta.UpdateFreightResponse.errorMessage;
                    }
                }
                else
                {
                    integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoCarga.ProblemaIntegracao = "Problema ao tentar integrar notas";
                }

                servicoArquivoTransacao.Adicionar(integracaoCarga, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException excecao)
            {
                integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoCarga.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoCarga.ProblemaIntegracao = "Problema ao tentar integrar.";

                servicoArquivoTransacao.Adicionar(integracaoCarga, jsonRequest, jsonResponse, "json");
            }

            repositorioCargaCargaIntegracao.Atualizar(integracaoCarga);
        }

        public void IntegrarPrechekinTransferencia(Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaFreteIntegracao repositorioCargaFreteIntegracao = new Repositorio.Embarcador.Cargas.CargaFreteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaFreteIntegracao.NumeroTentativas++;
            cargaFreteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoAvancoParaEmissao))
                    throw new ServicoException("Não existe URL de integração de avanço emissão configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoAvancoParaEmissao;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                Dominio.Entidades.Embarcador.Pedidos.Stage existeStage = null;

                if (cargaFreteIntegracao.Stage != null)
                    existeStage = cargaFreteIntegracao.Stage;

                if (existeStage == null)
                    existeStage = repositorioStage.BuscarPrimeiraPorCarga(cargaFreteIntegracao.Carga.Codigo);

                if (existeStage == null)
                    throw new ServicoException($"Não foi possivel encontrar stage realizar a integração");

                jsonRequest = JsonConvert.SerializeObject(ObterObjetoPreChekinTranferencia(cargaFreteIntegracao.Carga, existeStage.NumeroStage), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    bool sucesso = resposta["return"]?.errorCode == 105 ?? false;

                    if (sucesso)
                    {
                        cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaFreteIntegracao.ProblemaIntegracao = "Integração feita com sucesso";
                        cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = false;
                        cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
                        cargaFreteIntegracao.Carga.LiberadaComPendenciaIntegracaoFrete = false;

                    }
                    else
                    {
                        cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaFreteIntegracao.ProblemaIntegracao = resposta.UpdateFreightResponse.errorMessage;
                        cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = true;
                        cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
                    }
                }
                else
                {
                    cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaFreteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar notas";
                    cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = true;
                    cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
                }

                servicoArquivoTransacao.Adicionar(cargaFreteIntegracao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaFreteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaFreteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(cargaFreteIntegracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCargaFreteIntegracao.Atualizar(cargaFreteIntegracao);
        }

        public void IntegrarLeilaoManual(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (!configuracaoIntegracaoUnilever.IntegrarLeilaoManual)
                    throw new ServicoException("Integração de leilão manual inativa");

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoLeilaoManual))
                    throw new ServicoException("Não existe URL de integração de leilão manual configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoLeilaoManual;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaDadosTransporteIntegracao.Carga;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                dynamic objetoRequisicao = new
                {
                    UpdateFreightData = new
                    {
                        shipment = new
                        {
                            edoc = "S",
                            shipmentID = carga.CodigoCargaEmbarcador,
                            vpDetails = new List<dynamic>() { },
                            ServAgent = carga.Empresa?.CodigoIntegracao?.Replace("T", "")
                        }
                    }
                };

                jsonRequest = JsonConvert.SerializeObject(objetoRequisicao, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (resposta?.UpdateFreightResponse?.errorMessage == "Sucesso")
                    {
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração feita com sucesso";
                    }
                    else
                    {
                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = resposta.UpdateFreightResponse.errorMessage;
                    }
                }
                else
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração não realizar com sucesso.";
                }

                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");

            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCargaDadosTransporte.Atualizar(cargaDadosTransporteIntegracao);
        }

        public void IntegrarStatusDocumentoDestinado(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repositorioDocumentoDestinadoIntegracao = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repositorioDocumentoDestinado = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;
            integracaoPendente.ExisteXmlCompletoDocumentoDestinado = true;
            integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            bool eventoDeCancelamento = integracaoPendente.DocumentoDestinadoEmpresa?.TipoDocumento == TipoDocumentoDestinadoEmpresa.CancelamentoCTe || integracaoPendente.DocumentoDestinadoEmpresa?.TipoDocumento == TipoDocumentoDestinadoEmpresa.CancelamentoNFe;

            if (!eventoDeCancelamento)
            {
                integracaoPendente.ProblemaIntegracao = "Documento não é evento de cancelamento";
                repositorioDocumentoDestinadoIntegracao.Atualizar(integracaoPendente);

                return;
            }

            var codigoDocumentoPrincipal = repositorioDocumentoDestinado.BuscarNFePorChave(integracaoPendente.DocumentoDestinadoEmpresa.Chave)?.Codigo ?? 0;

            if (repositorioDocumentoDestinadoIntegracao.BuscarPorCodigoDocumentoDestinado((int)codigoDocumentoPrincipal)?.SituacaoIntegracao != SituacaoIntegracao.Integrado)
            {
                integracaoPendente.ProblemaIntegracao = "Documento principal ainda não foi integrado";
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ExisteXmlCompletoDocumentoDestinado = false;
                repositorioDocumentoDestinadoIntegracao.Atualizar(integracaoPendente);
                return;
            }


            bool sucesso = true;
            string mensagem = "";

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP configuracaoIntegracao = ObterConfiguracaoIntegracaoDestinados();

                if (string.IsNullOrEmpty(configuracaoIntegracao.URLIntegracaoStatus))
                    throw new ServicoException("Url de integração do status não configurada");

                jsonRequest = JsonConvert.SerializeObject(ObterObjetoRequisicaoStatusDocumento(integracaoPendente?.DocumentoDestinadoEmpresa));

                HttpClient cliente = ObterCliente(configuracaoIntegracao.ClientIDIntegracao, configuracaoIntegracao.ClientSecretIntegracao, tipoArquivo: "json");
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                HttpResponseMessage response = cliente.PostAsync(configuracaoIntegracao.URLIntegracaoStatus, content).Result;
                jsonResponse = response.Content.ReadAsStringAsync().Result;

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (retorno?.ZP10BR_CHECKCTESTATUSResponse?.OUT_TAB_REULTS?.cte != null)
                {
                    mensagem = retorno.ZP10BR_CHECKCTESTATUSResponse.OUT_TAB_REULTS.cte.item.validationMessage;
                    sucesso = retorno.ZP10BR_CHECKCTESTATUSResponse.OUT_TAB_REULTS.cte.item.validationStatusResult == "True" ? true : false;
                }
                else if (retorno?.ZP10BR_CHECKNFESTATUSResponse?.OUT_TAB_REULTS?.nfe != null)
                {
                    mensagem = retorno.ZP10BR_CHECKNFESTATUSResponse.OUT_TAB_REULTS.nfe.item.validationMessage;
                    sucesso = retorno.ZP10BR_CHECKNFESTATUSResponse.OUT_TAB_REULTS.nfe.item.validationStatusResult == "True" ? true : false;
                }

                if (string.IsNullOrEmpty(mensagem) && !string.IsNullOrWhiteSpace(retorno["statusMessage"]))
                {
                    mensagem = retorno["statusMessage"];
                    sucesso = false;
                }
                servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequest, jsonResponse, "json");

            }
            catch (ServicoException ex)
            {
                sucesso = false;
                mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                sucesso = false;
                mensagem = ex.Message;
                servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequest, jsonResponse, "json");
            }
            integracaoPendente.ProblemaIntegracao = mensagem;
            integracaoPendente.SituacaoIntegracao = sucesso ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;

            repositorioDocumentoDestinadoIntegracao.Atualizar(integracaoPendente);
        }

        public void IntegrarXmlDocumentoDestinado(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repositorioDocumentoDestinadoIntegracao = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP configuracaoIntegracao = ObterConfiguracaoIntegracaoDestinados();

                if (string.IsNullOrEmpty(configuracaoIntegracao.URLIntegracaoXML))
                    throw new ServicoException("Url de integração do xml não configurada");

                byte[] xml = ObterXmlRequisicaoPorTipo(integracaoPendente.DocumentoDestinadoEmpresa, _unitOfWork);

                xmlRequest = Encoding.UTF8.GetString(xml);

                HttpClient cliente = ObterCliente(configuracaoIntegracao.ClientIDIntegracao, configuracaoIntegracao.ClientSecretIntegracao, tipoArquivo: "xml");
                StringContent content = new StringContent(xmlRequest, Encoding.UTF8, "application/xml");

                HttpResponseMessage response = cliente.PostAsync(configuracaoIntegracao.URLIntegracaoXML, content).Result;
                xmlResponse = response.Content.ReadAsStringAsync().Result;

                integracaoPendente.ProblemaIntegracao = "Integração do xml feita com sucesso";
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracaoPendente.ExisteXmlCompletoDocumentoDestinado = true;

                if (!response.IsSuccessStatusCode)
                {
                    integracaoPendente.ProblemaIntegracao = "Error ao tentar integrar";
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }

                servicoArquivoTransacao.Adicionar(integracaoPendente, xmlRequest, xmlResponse, "xml");
            }
            catch (ServicoException ex)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = ex.Message;
                integracaoPendente.ExisteXmlCompletoDocumentoDestinado = false;
                //servicoArquivoTransacao.Adicionar(integracaoPendente, xmlRequest, ex.Message, "xml");
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(integracaoPendente, xmlRequest, xmlResponse, "xml");
            }

            repositorioDocumentoDestinadoIntegracao.Atualizar(integracaoPendente);
        }

        public void ReenviarIntegracoesPreCalculo(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao exiteIntegracaoPrecalculo = repositorioCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(codigoCarga, TipoIntegracao.Unilever);

            if (exiteIntegracaoPrecalculo == null)
                return;

            exiteIntegracaoPrecalculo.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            exiteIntegracaoPrecalculo.NumeroTentativas++;
            exiteIntegracaoPrecalculo.ProblemaIntegracao = "";
            repositorioCargaDadosTransporteIntegracao.Atualizar(exiteIntegracaoPrecalculo);

        }

        public void IntegrarLoteEscrituracaoRetorno(Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituacaoMiroIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoMiroIntegracao repositorioDocumentoDestinadoIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoMiroIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracao = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrEmpty(configuracaoIntegracao.URLIntegracaoEscrituracaoRetorno))
                    throw new ServicoException("Url de integração do xml não configurada");


                jsonRequest = JsonConvert.SerializeObject(new
                {
                    MIRONUMBER = integracaoPendente.LoteEscrituracaoMiroDocumento.NumeroFolha,
                    COMPCODE = int.Parse(integracaoPendente.LoteEscrituracaoMiroDocumento?.ControleDocumento?.CTe?.Remetente?.GrupoPessoas?.Descricao?.ObterSomenteNumeros() ?? "0"),
                    YEAR = integracaoPendente.LoteEscrituracaoMiroDocumento.DataMiro.Value.Year
                });

                HttpClient cliente = ObterCliente(configuracaoIntegracao.ClientIDIntegracaoUnilever, configuracaoIntegracao.ClientSecretIntegracaoUnilever, tipoArquivo: "json");
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                HttpResponseMessage response = cliente.PostAsync(configuracaoIntegracao.URLIntegracaoEscrituracaoRetorno, content).Result;
                jsonResponse = response.Content.ReadAsStringAsync().Result;
                dynamic reponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (!response.IsSuccessStatusCode)
                    throw new ServicoException("Problema ao tentar Integrar");

                if (reponse["StatusCode"] != 200)
                    throw new ServicoException(reponse["StatusMessage"]);

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                integracaoPendente.ProblemaIntegracao = reponse["StatusMessage"];

                servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = ex.Message;
                servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequest, jsonResponse, "json");
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequest, jsonResponse, "json");
            }

            repositorioDocumentoDestinadoIntegracao.Atualizar(integracaoPendente);
        }

        public void IntegrarCancelamentoDocumentoProvisao(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoCancelamentoProvisao))
                    throw new ServicoException("Não existe URL de integração de travamento configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoCancelamentoProvisao;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                List<dynamic> envioNumero = new List<dynamic>();
                envioNumero.Add(new { numeroFolha = integracaoPendente.DocumentoProvisao.Stage.NumeroFolha });

                jsonRequest = JsonConvert.SerializeObject(envioNumero, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                bool sucesso = IsRetornoSucesso(retornoRequisicao);
                integracaoPendente.SituacaoIntegracao = sucesso ? SituacaoIntegracao.AgRetorno : SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = sucesso ? "Integração feita com sucesso (Aguardando Retorno)" : "Problema ao tentar receber a resposta";

                servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequest, jsonResponse, "json");
            }

            repositorioIntegracao.Atualizar(integracaoPendente);
        }

        public void ProcessamentoDadosTransporteCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso = null, string adminStringConexao = "")
        {
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repConfigMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracoaTms = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repConfigMotorista.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracoaTms.BuscarConfiguracaoPadrao();


            if (documentoTransporte.Motoristas != null && documentoTransporte.Motoristas.Count > 0)
            {
                List<int> listaCodigosMotoristasAntigos = new List<int>();

                if (carga.Motoristas?.Count > 0)
                    listaCodigosMotoristasAntigos = carga.Motoristas.Select(m => m.Codigo).ToList();

                bool incluiuNovoMotorista = false;

                carga.Motoristas = new List<Dominio.Entidades.Usuario>();
                Servicos.Log.TratarErro($"Inicio Cadastro Motoristas {string.Join(",", documentoTransporte.Motoristas.Select(c => c.CPF).ToList())}");
                foreach (var motorista in documentoTransporte.Motoristas)
                {
                    var existeMotorista = repMotorista.BuscarPorCPF(motorista.CPF.ObterSomenteNumeros());
                    Servicos.Log.TratarErro($"Inicio Cadastro Motoristas Codigo Motorista Existente: {existeMotorista?.Codigo ?? 0} CPF: {motorista.CPF.ObterSomenteNumeros()} ");
                    if (existeMotorista == null)
                    {
                        string message = string.Empty;
                        existeMotorista = new Servicos.WebService.Empresa.Motorista(_unitOfWork).SalvarMotorista(new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista
                        {
                            CPF = motorista.CPF.ObterSomenteNumeros(),
                            Nome = motorista.Nome,
                            Ativo = true
                        }, carga.Empresa, ref message, _unitOfWork, tipoServicoMultisoftware, auditado, clienteAcesso, adminStringConexao);

                        if (!string.IsNullOrWhiteSpace(message))
                            throw new ServicoException(message);
                    }
                    else
                    {
                        if (configuracaoTMS.CadastrarMotoristaMobileAutomaticamente)
                            new Servicos.WebService.Empresa.Motorista(_unitOfWork).ConfigurarUsuarioMobile(ref existeMotorista, clienteAcesso, adminStringConexao);
                    }

                    bool adicionar = true;
                    if (configuracaoMotorista.MotoristasIgnorados != null && configuracaoMotorista.MotoristasIgnorados.Count > 0)
                    {
                        if (configuracaoMotorista.MotoristasIgnorados.Any(obj => obj.ToLower() == motorista.Nome.ToLower()))
                            adicionar = false;
                    }

                    if (adicionar && existeMotorista != null && !carga.Motoristas.Any(x => x.Codigo == existeMotorista.Codigo || x.CPF == existeMotorista.CPF))
                    {
                        carga.Motoristas.Add(existeMotorista);
                        if (!listaCodigosMotoristasAntigos.Contains(existeMotorista.Codigo))
                            incluiuNovoMotorista = true;
                    }
                }

                if ((configuracaoTMS?.UtilizaAppTrizy ?? false) &&
                    (carga.Filial?.HabilitarPreViagemTrizy ?? false) && (carga.Filial?.TipoOperacoesTrizy?.Count > 0 && carga.Filial.TipoOperacoesTrizy.Contains(carga.TipoOperacao)) &&
                    (listaCodigosMotoristasAntigos.Count != carga.Motoristas.Count || incluiuNovoMotorista))
                    Servicos.Embarcador.Integracao.IntegracaoCarga.ReenviarIntegracaoDadosTransporte(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Trizy, _unitOfWork, auditado);

                if (carga.DataLimiteConfirmacaoMotorista.HasValue && servicoMensagemAlerta.IsMensagemSemConfirmacao(carga, TipoMensagemAlerta.CargaSemConfirmacaoMotorista))
                {
                    TimeSpan tempoLimite = carga.Carregamento?.TempoLimiteConfirmacaoMotorista.TotalSeconds > 0 ? carga.Carregamento.TempoLimiteConfirmacaoMotorista : carga.TipoOperacao?.ConfiguracaoMobile?.TempoLimiteConfirmacaoMotorista ?? new TimeSpan();

                    if (tempoLimite.TotalSeconds > 0)
                        carga.DataLimiteConfirmacaoMotorista = DateTime.Now.Add(tempoLimite);
                }
            }

            if (carga.Empresa == null)
                return;

            if (!string.IsNullOrWhiteSpace(documentoTransporte?.Veiculo?.Placa))
                carga.Veiculo = ObterVeiculo(documentoTransporte?.Veiculo, carga.Empresa);

            if (carga.ModeloVeicularCarga != null && carga.ModeloVeicularCarga.Tipo == TipoModeloVeicularCarga.Geral && string.IsNullOrWhiteSpace(documentoTransporte?.Veiculo?.Placa) && documentoTransporte?.Veiculo?.Reboques?.FirstOrDefault() != null)
            {
                carga.Veiculo = ObterVeiculo(documentoTransporte?.Veiculo?.Reboques?.FirstOrDefault(), carga.Empresa);
                carga.VeiculosVinculados.Clear();
                Servicos.Log.TratarErro($"Removendo Reboques carga " + $"({carga.CodigoCargaEmbarcador})" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), "RemocaoPlaca");
            }
            else if (documentoTransporte?.Veiculo?.Reboques?.Count > 0)
            {
                List<Dominio.Entidades.Veiculo> listaVeiculos = new List<Dominio.Entidades.Veiculo>();

                foreach (var veiculo in documentoTransporte?.Veiculo?.Reboques)
                {
                    Dominio.Entidades.Veiculo veiculoexite = ObterVeiculo(veiculo, carga.Empresa);
                    if (veiculoexite == null)
                        continue;

                    listaVeiculos.Add(veiculoexite);
                }
                carga.VeiculosVinculados = listaVeiculos;

                Servicos.Log.TratarErro($"Removendo Reboques carga " + $"({carga.CodigoCargaEmbarcador})" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), "RemocaoPlaca");
            }
        }

        public void EnviarCteCanhoto(Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao integracao)
        {
            Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repCTeCanhotoIntegracao = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoCanhoto))
                    throw new ServicoException("Não existe URL de integração de canhoto configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoCanhoto;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(ObterObjetoRequisicaoCteCanhoto(integracao.CTe, integracao.TipoRegistro), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                else
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                integracao.ProblemaIntegracao = (string)resposta?.StatusMessage ?? "";

                servicoArquivoTransacao.Adicionar(integracao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(integracao, jsonRequest, jsonResponse, "json");
            }

            repCTeCanhotoIntegracao.Atualizar(integracao);
        }

        public void IntegrarCancelamentoPagamento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao integracao)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao repCancelamentoPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();
            if (!configuracaoIntegracaoUnilever.IntegrarCancelamentoPagamento)
                return;

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoCancelamentoPagamento))
                    throw new ServicoException("Não existe URL de integração de cancelamento de pagamento configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoCancelamentoPagamento;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(ObterObjetoRequisicaoCancelamentoPagamento(integracao.DocumentoFaturamento), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    if ((string)resposta?.RequestInvoiceProcessingResponse?.errorMessage == null)
                        integracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                    else
                        integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                integracao.ProblemaIntegracao = (string)resposta?.RequestInvoiceProcessingResponse?.errorMessage ?? "";

                servicoArquivoTransacao.Adicionar(integracao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(integracao, jsonRequest, jsonResponse, "json");
            }

            repCancelamentoPagamentoIntegracao.Atualizar(integracao);
        }

        public void IntegrarUnileverRecusa(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever))
                    throw new ServicoException("Não existe URL de integração de ocorrência configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(ObterDadosRequisicaoUnileverBase(integracao.Carga, integracaoInfrutifera: false), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (resposta?.UpdateFreightResponse?.errorMessage == "Sucesso")
                    {
                        integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        integracao.ProblemaIntegracao = "Integração feita com sucesso";
                    }
                    else
                    {
                        integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracao.ProblemaIntegracao = resposta.UpdateFreightResponse.errorMessage;
                    }
                }
                else
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracao.ProblemaIntegracao = "Integração não realizar com sucesso.";
                }

                servicoArquivoTransacao.Adicionar(integracao, jsonRequest, jsonResponse, "json");

            }
            catch (ServicoException ex)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(integracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCargaDadosTransporte.Atualizar(integracao);
        }

        public void IntegrarUnileverInfrutifera(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao)
        {
            ///Precisamos validar como vai se pego a vinculação desta carga com a carga originario do pedido.
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever))
                    throw new ServicoException("Não existe URL de integração de ocorrência configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(ObterDadosRequisicaoUnileverBase(integracao.Carga, integracaoInfrutifera: true), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (resposta?.UpdateFreightResponse?.errorMessage == "Sucesso")
                    {
                        integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        integracao.ProblemaIntegracao = "Integração feita com sucesso";
                    }
                    else
                    {
                        integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracao.ProblemaIntegracao = resposta.UpdateFreightResponse.errorMessage;
                    }
                }
                else
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracao.ProblemaIntegracao = "Integração não realizar com sucesso.";
                }

                servicoArquivoTransacao.Adicionar(integracao, jsonRequest, jsonResponse, "json");

            }
            catch (ServicoException ex)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(integracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCargaDadosTransporte.Atualizar(integracao);
        }

        #endregion Métodos Públicos

        #region Integrações Privadas

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.IntegracaoCTe ObterDadosIntegracaoCTeEmissaoInicial(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            return ObterDadosIntegracaoCTe(cargaCTe?.Carga?.Codigo ?? 0, "Y", "100", cargaCTe?.Carga?.CodigoCargaEmbarcador ?? string.Empty, cargaCTe?.CTe?.Chave ?? "", cargaCTe?.CTe?.Codigo ?? 0, numeroStage: "");
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.IntegracaoCTe ObterDadosIntegracaoCTeEmissaoCancelamento(int codigoCarga, string codigoEmbarcardor, string chaveCte, int codigoCTe, string numeroStage)
        {
            return ObterDadosIntegracaoCTe(codigoCarga, "N", "101", codigoEmbarcardor, chaveCte, codigoCTe, numeroStage);
        }

        private void IntegrarPreChekin(Dominio.Entidades.Embarcador.Cargas.CargaPreChekinIntegracao cargaPreChekinIntegracao, bool confirmacao)
        {
            Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao repositorioCargaPreChekin = new Repositorio.Embarcador.Cargas.CargaPreChekinIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaPreChekinIntegracao.NumeroTentativas++;
            cargaPreChekinIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever))
                    throw new ServicoException("Não existe URL de integração de travamento configurada para a Unilever");

                string endPoint = configuracaoIntegracaoUnilever.URLIntegracaoTravamentoDTUnilever;
                string clientID = configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever;
                string clientSecret = configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever;

                var headersIntegracao = new List<(string Chave, string Valor)>() {
                    ValueTuple.Create("client_id", clientID),
                    ValueTuple.Create("client_secret", clientSecret)
                };

                jsonRequest = JsonConvert.SerializeObject(ObterObjetoRequisicao(cargaPreChekinIntegracao.Carga.CodigoCargaEmbarcador, confirmacao), Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                WebRequest requisicao = CriaRequisicao(endPoint, "POST", jsonRequest, headersIntegracao);
                HttpWebResponse retornoRequisicao = ExecutarRequisicao(requisicao);
                jsonResponse = ObterResposta(retornoRequisicao);

                if (IsRetornoSucesso(retornoRequisicao))
                {
                    dynamic resposta = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    if (resposta?.UpdateFreightResponse?.errorMessage == "Sucesso")
                    {
                        cargaPreChekinIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaPreChekinIntegracao.ProblemaIntegracao = "Integração feita com sucesso";
                    }
                    else
                    {
                        cargaPreChekinIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        cargaPreChekinIntegracao.ProblemaIntegracao = resposta.UpdateFreightResponse.errorMessage;
                    }
                }
                else
                {
                    cargaPreChekinIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaPreChekinIntegracao.ProblemaIntegracao = confirmacao ? "Erro de integração ao tentar confirmar placas." : "Erro ao tentar remover placas.";
                }

                servicoArquivoTransacao.Adicionar(cargaPreChekinIntegracao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                cargaPreChekinIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaPreChekinIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaPreChekinIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaPreChekinIntegracao.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(cargaPreChekinIntegracao, jsonRequest, jsonResponse, "json");
            }

            repositorioCargaPreChekin.Atualizar(cargaPreChekinIntegracao);
        }

        private string InformarDadosTransporteCargaGerada(Dominio.Entidades.Embarcador.Cargas.Carga cargaGerada, Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento agrupamento, Servicos.Embarcador.Carga.Carga svcCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagemErro = string.Empty;

            bool dadosTransporteInformados = (
                                (cargaGerada.TipoDeCarga != null) &&
                                (cargaGerada.ModeloVeicularCarga != null) &&
                                (cargaGerada.Veiculo != null) &&
                                (!(cargaGerada.TipoOperacao?.ExigePlacaTracao ?? false) || ((cargaGerada.VeiculosVinculados?.Count ?? 0) == cargaGerada.ModeloVeicularCarga.NumeroReboques))
                            );
            if (dadosTransporteInformados)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte dadosTransporte = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaDadosTransporte()
                {
                    Carga = cargaGerada,
                    CodigoEmpresa = cargaGerada.Empresa?.Codigo ?? 0,
                    CodigoModeloVeicular = cargaGerada.ModeloVeicularCarga?.Codigo ?? 0,
                    CodigoReboque = (cargaGerada.VeiculosVinculados?.Count > 0) ? cargaGerada.VeiculosVinculados.ElementAt(0).Codigo : 0,
                    CodigoSegundoReboque = (cargaGerada.VeiculosVinculados?.Count > 1) ? cargaGerada.VeiculosVinculados.ElementAt(1).Codigo : 0,
                    CodigoTipoCarga = cargaGerada.TipoDeCarga?.Codigo ?? 0,
                    CodigoTipoOperacao = cargaGerada.TipoOperacao?.Codigo ?? 0,
                    CodigoTracao = cargaGerada.Veiculo?.Codigo ?? 0,
                    ObservacaoTransportador = cargaGerada.ObservacaoTransportador
                };

                if (agrupamento.Motorista != null)
                    dadosTransporte.ListaCodigoMotorista.Add(agrupamento.Motorista.Codigo);

                svcCarga.SalvarDadosTransporteCarga(dadosTransporte, out mensagemErro, usuario: null, liberarComProblemaIntegracaoGrMotoristaVeiculo: false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, webServiceConsultaCTe: string.Empty, cliente: null, auditado, unitOfWork, true);
            }

            return mensagemErro;
        }

        #endregion Integrações Privadas

        #region Métodos Privados

        private HttpClient ObterCliente(string clientId, string clientSecret, string tipoArquivo)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient cliente = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoUnilever));

            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue($"application/{tipoArquivo}"));
            cliente.DefaultRequestHeaders.Add("Client_id", clientId);
            cliente.DefaultRequestHeaders.Add("Client_secret", clientSecret);

            return cliente;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever ObterConfiguracaoIntegracaoUnilever()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoUnilever repositorioConfiguracaoIntegracaoUnilever = new Repositorio.Embarcador.Configuracoes.IntegracaoUnilever(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = repositorioConfiguracaoIntegracaoUnilever.Buscar();

            if ((configuracaoIntegracaoUnilever == null) || !configuracaoIntegracaoUnilever.PossuiIntegracaoUnilever)
                throw new ServicoException("Não existe configuração de integração disponível para a Unilever");

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever) || string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever))
                throw new ServicoException("O Client ID e o Client Secret devem estar preenchidos na configuração de integração da Unilever");

            return configuracaoIntegracaoUnilever;
        }

        private void EnviarRetornoPreCalculo(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool sucesso, string retorno)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
            Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever configuracaoIntegracaoUnilever = ObterConfiguracaoIntegracaoUnilever();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoUnilever.URLIntegracaoValorPreCalculoUnilever))
                    throw new ServicoException("Não existe URL de integração de valor de pré cálculo configurada para a Unilever");

                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stagesAgrupadas = repositorioAgrupamento.BuscarPorCargaDt(carga.Codigo);

                dynamic requestIntegracaoRetorno = new
                {
                    UpdateFreightData = new
                    {
                        shipment = new
                        {
                            edoc = "S",
                            shipmentID = carga?.CodigoCargaEmbarcador ?? string.Empty,
                            IM_MS_FREIGHT = new List<dynamic>()
                        }
                    }
                };

                if (stagesAgrupadas.Count > 0)
                {

                    foreach (var stageAgrupada in stagesAgrupadas)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = repositorioStage.BuscarporAgrupamento(stageAgrupada.Codigo);
                        foreach (var stage in stages)
                            requestIntegracaoRetorno.UpdateFreightData.shipment.IM_MS_FREIGHT.Add(new
                            {
                                STAGE = new
                                {
                                    STAGE_ID = stage.NumeroStage,
                                    FSTATUS = stageAgrupada.ValorFreteTotal > 0 ? "S" : "F",
                                    FREASON = stageAgrupada.ValorFreteTotal > 0 ? "Frete calculado com sucesso" : Utilidades.String.Left(stageAgrupada.MensagemRetornoDadosFrete, 150) ?? string.Empty,
                                    LCHANGED = DateTime.Now.ToString("yyyy-MM-dd"),
                                    LCHANGET = DateTime.Now.ToString("hh:mm:ss")
                                }
                            });
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Pedidos.Stage existeStages = repositorioStage.BuscarPrimeiraPorCarga(carga.Codigo);
                    requestIntegracaoRetorno.UpdateFreightData.shipment.IM_MS_FREIGHT.Add(new
                    {
                        STAGE = new
                        {
                            STAGE_ID = existeStages.NumeroStage,
                            FSTATUS = sucesso ? "S" : "F",
                            FREASON = sucesso ? "Frete calculado com sucesso" : retorno ?? string.Empty,
                            LCHANGED = DateTime.Now.ToString("yyyy-MM-dd"),
                            LCHANGET = DateTime.Now.ToString("hh:mm:ss")
                        }
                    });
                }

                jsonRequest = JsonConvert.SerializeObject(requestIntegracaoRetorno);

                HttpClient cliente = ObterCliente(configuracaoIntegracaoUnilever.ClientIDIntegracaoUnilever, configuracaoIntegracaoUnilever.ClientSecretIntegracaoUnilever, "json");
                StringContent dadosEnviar = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = cliente.PostAsync(configuracaoIntegracaoUnilever.URLIntegracaoValorPreCalculoUnilever, dadosEnviar).Result;

                jsonResponse = retornoRequisicao.Content.ReadAsStringAsync().Result;
                dynamic response = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Erro ao tentar comunicar com o servidor");

                if (response.UpdateFreightResponse.errorMessage == "DT não encontrado")
                    throw new ServicoException("DT não encontrado");

                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Pre calculo integrado";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                if (cargaDadosTransporteIntegracao.Codigo > 0)
                    repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                else
                    repositorioCargaDadosTransporteIntegracao.Inserir(cargaDadosTransporteIntegracao);
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequest, jsonResponse, "json");
        }

        private string ObterRequestTruckAssignment(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaIntegracao.Carga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.RequestTruckAssignment requestTruckAssignment = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.RequestTruckAssignment()
            {
                shipper = cargaPedido.Pedido.ObterTomador().CPF_CNPJ_SemFormato,
                billOfLading = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                tractorNumber = cargaIntegracao.Carga.Veiculo?.Placa
            };

            return JsonConvert.SerializeObject(requestTruckAssignment, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
        }

        private WebRequest CriaRequisicao(string url, string metodo, string body, List<(string Chave, string Valor)> headers = null, string contentType = "application/json")
        {
            WebRequest requisicao = WebRequest.Create(url);

            byte[] byteArrayDadosRequisicao = Encoding.UTF8.GetBytes(body);

            requisicao.Method = metodo;
            requisicao.ContentType = contentType;

            foreach (var (Chave, Valor) in (headers ?? new List<(string Chave, string Valor)>()))
                requisicao.Headers[Chave] = Valor;

            requisicao.ContentLength = byteArrayDadosRequisicao.Length;

            System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream();
            streamDadosRequisicao.Write(byteArrayDadosRequisicao, 0, byteArrayDadosRequisicao.Length);
            streamDadosRequisicao.Close();

            return requisicao;
        }

        private HttpWebResponse ExecutarRequisicao(WebRequest request)
        {
            try
            {
                WebResponse retornoRequisicao = request.GetResponse();
                return (HttpWebResponse)retornoRequisicao;
            }
            catch (WebException webException)
            {
                if (webException.Response == null)
                    throw new ServicoException("Falha ao processar o retorno da API");

                return (HttpWebResponse)webException.Response;
            }
        }

        private string ObterResposta(HttpWebResponse response)
        {
            string jsonDadosRetornoRequisicao;
            using (System.IO.Stream streamDadosRetornoRequisicao = response.GetResponseStream())
            {
                System.IO.StreamReader leitorDadosRetornoRequisicao = new System.IO.StreamReader(streamDadosRetornoRequisicao);
                jsonDadosRetornoRequisicao = leitorDadosRetornoRequisicao.ReadToEnd();
                leitorDadosRetornoRequisicao.Close();
            }

            response.Close();

            return jsonDadosRetornoRequisicao;
        }

        private bool IsRetornoSucesso(HttpWebResponse retornoRequisicao)
        {
            if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                return true;

            if (retornoRequisicao.StatusCode == HttpStatusCode.Created)
                return true;

            return false;
        }

        private bool IsRetornoSucesso(string jsonDadosRetornoRequisicao)
        {
            string codigoNaoExistente = "110";
            string codigoTabelaOcupada = "108";

            if (jsonDadosRetornoRequisicao.Contains(codigoNaoExistente) || jsonDadosRetornoRequisicao.Contains(codigoTabelaOcupada))
                return false;

            return true;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.IntegracaoCTe ObterDadosIntegracaoCTe(int codigoCarga, string statusSap, string status, string codigoCargaIntegracao, string chaveCte, int codigoCTe, string numeroStage)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedido = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedido.BuscarPedidoXMLNotaFiscalPorCodigoCTe(codigoCTe, codigoCarga);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.IntegracaoCTe
            {
                Documento = new IntegracaoCTeDocumento
                {
                    TipoDocumentoSap = "S",
                    NumeroDocumento = codigoCargaIntegracao,
                    Stage = ObterNumeroStage(pedidoXMLNotaFiscal?.CargaPedido ?? null, numeroStage),
                    TipoDocumentoFis = "A",
                    ChaveDocumentoFis = chaveCte,
                    ChaveReferenciaDocumento = "",
                    UniDocs = "",
                    Status = status,
                    EmissaoDocumento = statusSap,
                    ArquivoBase64 = ""
                }
            };
        }

        private string ObterNumeroStage(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, string numeroStage)
        {
            if (!string.IsNullOrEmpty(numeroStage))
                return numeroStage;

            if (!string.IsNullOrEmpty(cargaPedido?.StageRelevanteCusto?.NumeroStage ?? string.Empty))
                return cargaPedido.StageRelevanteCusto.NumeroStage;

            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Stage stage = repPedidoStage.BuscarStagePorCargaPedido(cargaPedido?.Codigo ?? 0, considerarEtapaRetorno: false);

            return stage?.NumeroStage ?? string.Empty;
        }

        private Dominio.Entidades.Veiculo ObterVeiculo(Dominio.ObjetosDeValor.WebService.Rest.Veiculo veiculo, Dominio.Entidades.Empresa empresa)
        {
            if (veiculo == null)
                return null;

            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);

            string placa = !string.IsNullOrEmpty(veiculo?.Placa ?? "") ? veiculo.Placa.Replace("-", "") : "";
            Dominio.Entidades.Veiculo existeVeiculo = repositorioVeiculo.BuscarPorPlaca(empresa.Codigo, placa);

            if (existeVeiculo == null)
                existeVeiculo = repositorioVeiculo.BuscarPorPlacaMaisRecente(placa);

            if (existeVeiculo == null)
            {
                existeVeiculo = new Dominio.Entidades.Veiculo();
                existeVeiculo.Placa = placa;
                existeVeiculo.Ativo = true;
                existeVeiculo.Renavam = veiculo?.Renavam ?? string.Empty;
                existeVeiculo.Empresa = empresa;
                existeVeiculo.TipoVeiculo = veiculo?.TipoVeiculo == TipoVeiculo.Tracao ? "0" : "1";
                existeVeiculo.ModeloVeicularCarga = repositorioModeloVeicularCarga.buscarPorCodigoIntegracao(veiculo?.ModeloVeicular?.CodigoIntegracao);
                existeVeiculo.Estado = repositorioEstado.BuscarPorSigla(veiculo?.UF);

                if (existeVeiculo.Estado == null && existeVeiculo.Empresa != null && existeVeiculo.Empresa != null)
                    existeVeiculo.Estado = existeVeiculo.Empresa.Localidade.Estado;

                repositorioVeiculo.Inserir(existeVeiculo);
            }
            else
            {
                bool situacaoAnterior = existeVeiculo.Ativo;
                existeVeiculo.Ativo = true;
                existeVeiculo.Empresa = empresa;
                if (Enum.IsDefined(typeof(TipoVeiculo), veiculo?.TipoVeiculo ?? null))
                    existeVeiculo.TipoVeiculo = veiculo?.TipoVeiculo == TipoVeiculo.Tracao ? "0" : "1";

                if (!string.IsNullOrEmpty(veiculo.Renavam ?? string.Empty))
                    existeVeiculo.Renavam = veiculo.Renavam;

                if (!string.IsNullOrEmpty(veiculo?.ModeloVeicular?.CodigoIntegracao))
                    existeVeiculo.ModeloVeicularCarga = repositorioModeloVeicularCarga.buscarPorCodigoIntegracao(veiculo?.ModeloVeicular?.CodigoIntegracao);

                Dominio.Entidades.Estado uf = repositorioEstado.BuscarPorSigla(veiculo?.UF);
                if (uf != null)
                    existeVeiculo.Estado = uf;

                if (existeVeiculo.Estado == null && existeVeiculo.Empresa != null && existeVeiculo.Empresa != null)
                    existeVeiculo.Estado = existeVeiculo.Empresa.Localidade.Estado;

                Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(existeVeiculo, situacaoAnterior, MetodosAlteracaoVeiculo.ObterVeiculo_IntegracaoUnilever, null, _unitOfWork);

                repositorioVeiculo.Atualizar(existeVeiculo);
            }

            return existeVeiculo;
        }

        private bool PermiteGerarIntegracaoPreChekin(SituacaoCarga situacaoCarga)
        {
            List<SituacaoCarga> situacaoesPermitidas = new List<SituacaoCarga>() { SituacaoCarga.Nova, SituacaoCarga.AgNFe, SituacaoCarga.CalculoFrete, SituacaoCarga.AgTransportador };
            return situacaoesPermitidas.Contains(situacaoCarga);
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP ObterConfiguracaoIntegracaoDestinados()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDestinadosSAP repositorioIntegracaoDestinados = new Repositorio.Embarcador.Configuracoes.IntegracaoDestinadosSAP(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP existeIntegracaoDestinados = repositorioIntegracaoDestinados.BuscarPrimeiroRegistro();

            if (existeIntegracaoDestinados == null || !existeIntegracaoDestinados.PossuiIntegracao)
                throw new ServicoException("Não Existe Configuração de Integração Destinados");

            if (string.IsNullOrWhiteSpace(existeIntegracaoDestinados.ClientIDIntegracao) || string.IsNullOrWhiteSpace(existeIntegracaoDestinados.ClientSecretIntegracao))
                throw new ServicoException("Dados de autenticação não configurados corretamente.");

            return existeIntegracaoDestinados;
        }

        #endregion Métodos Privados

        #region Metodos Construtores dos Objetos das Integrações

        private dynamic ObterObjetoRequisicao(string codigoCargaEmbarcador, bool confirmacao)
        {
            return new
            {
                UpdateFreightData = new
                {
                    shipment = new
                    {
                        edoc = "S",
                        shipmentID = codigoCargaEmbarcador,
                        IM_MS_Precheckin = new
                        {
                            PCSTATUS = confirmacao ? "Y" : "C",
                            PCDETAILS = confirmacao ? "Checkin realizado" : "Checkin cancelado",
                            PCLCHANGED = DateTime.Now.ToString("yyyy-MM-dd"),
                            PCLCHANGET = DateTime.Now.ToString("HH:mm:ss")
                        }
                    }
                }
            };
        }

        private dynamic ObterObjetoRequisicaoCteCanhoto(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, TipoRegistroIntegracaoCTeCanhoto tipoRegistro)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCte.BuscarPorCTe(cte.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.Stage stageCTe = Servicos.Embarcador.Pedido.Stage.BuscarStagePorCargaCte(cargaCte.Codigo, _unitOfWork);

            return new
            {
                ShipmentNumber = cargaCte.Carga.CodigoCargaEmbarcador,
                OccurrenceNumber = string.Empty,
                Stages = from obj in cte.XMLNotaFiscais
                         select new
                         {
                             StageId = stageCTe.NumeroStage,
                             AcessKey = obj.Chave,
                             PodTempDate = tipoRegistro == TipoRegistroIntegracaoCTeCanhoto.Imagem ? obj.Canhoto?.DataEntregaNotaCliente?.ToString("yyyy-MM-dd") ?? string.Empty : string.Empty,
                             PodTempTime = tipoRegistro == TipoRegistroIntegracaoCTeCanhoto.Imagem ? obj.Canhoto?.DataEntregaNotaCliente?.ToString("HH:mm:ss") ?? string.Empty : string.Empty,
                             PodDefDate = tipoRegistro == TipoRegistroIntegracaoCTeCanhoto.Confirmacao ? obj.Canhoto?.DataEntregaNotaCliente?.ToString("yyyy-MM-dd") ?? string.Empty : string.Empty,
                             PodDefTime = tipoRegistro == TipoRegistroIntegracaoCTeCanhoto.Confirmacao ? obj.Canhoto?.DataEntregaNotaCliente?.ToString("HH:mm:ss") ?? string.Empty : string.Empty,
                             PodSinister = string.Empty,
                             PodMapaoDate = tipoRegistro == TipoRegistroIntegracaoCTeCanhoto.Confirmacao ? obj.Canhoto?.DataAprovacaoDigitalizacao?.ToString("yyyy-MM-dd") ?? string.Empty : string.Empty,
                             PodMapaoTime = tipoRegistro == TipoRegistroIntegracaoCTeCanhoto.Confirmacao ? obj.Canhoto?.DataAprovacaoDigitalizacao?.ToString("HH:mm:ss") ?? string.Empty : string.Empty
                         }
            };
        }

        private dynamic ObterObjetoRequisicaoCancelamentoPagamento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento)
        {
            return new
            {
                RequestInvoiceProcessing = ObterRequestPagamento(documentoFaturamento, true)
            };
        }

        private dynamic ObterObjetoRequisicaoConfirmacaoPlacaComLinkNotasERelevanciaCusto(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Stage stage)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioNotaProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            dynamic objetoRequisicao = new
            {
                UpdateFreightData = new
                {
                    shipment = new
                    {
                        edoc = "S",
                        shipmentID = carga?.CodigoCargaEmbarcador ?? string.Empty,
                        vpDetails = new List<dynamic>() { },
                        ServAgent = ""
                    }
                }
            };

            if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                List<int> codigosXmlNotaFiscalComRecusa = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCodigosXmlNotaFiscalComCTeRecusaPorCarga(carga.Codigo);
                List<int> codigosPedidosCarga = repCargaPedido.BuscarCodigosPedidoPorCarga(carga.Codigo);
                List<int> codigosCargaCTeComCheckinRecusado = repCargaCte.BuscarCodigosComCheckinRecusadoPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesCarga = repPedidoStage.BuscarPorListaPedidosECargaDt(codigosPedidosCarga, carga.Codigo);
                List<int> codigosStagesRecusadas = new List<int>();

                foreach (int codigoCargaCTe in codigosCargaCTeComCheckinRecusado)
                {
                    int codigoStage = repositorioCargaCTe.BuscarCodigoStageRelevantePorCargaCTe(codigoCargaCTe);

                    if (codigoStage > 0)
                        codigosStagesRecusadas.Add(codigoStage);
                }

                foreach (Dominio.Entidades.Embarcador.Pedidos.Stage stageCarga in stagesCarga)
                {
                    bool enviarConfirmacaoPlaca = stageCarga.RelevanteVP && (stageCarga.StageAgrupamento?.PlacasConfirmadas ?? false);
                    bool enviarLinkNotas = !codigosStagesRecusadas.Contains(stageCarga.Codigo);

                    if (!enviarConfirmacaoPlaca && !enviarLinkNotas)
                        continue;

                    dynamic details = new ExpandoObject();

                    details.stageID = stageCarga.NumeroStage ?? string.Empty;
                    details.axlesTRCK = "";
                    details.axlesTRLR = "";
                    details.axlesBTRA = "";

                    if (enviarConfirmacaoPlaca)
                    {
                        details.plateTRCK = stageCarga.StageAgrupamento.Veiculo?.Placa ?? string.Empty;
                        details.axlesTRCK = stageCarga.StageAgrupamento.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.plateBTRA = stageCarga.StageAgrupamento.SegundoReboque?.Placa ?? string.Empty;
                        details.ufTRLRID = stageCarga.StageAgrupamento.Veiculo?.Estado?.Sigla ?? string.Empty;
                        details.plateTRLR = stageCarga.StageAgrupamento.Reboque?.Placa ?? string.Empty;
                        details.axlesTRLR = stageCarga.StageAgrupamento.Reboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.ufTRLRBLDID = stageCarga.StageAgrupamento.Reboque?.Estado?.Sigla ?? string.Empty;
                        details.axlesBTRA = stageCarga.StageAgrupamento.SegundoReboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.ufZBTRAIN = stageCarga.StageAgrupamento.SegundoReboque?.Estado?.Sigla ?? string.Empty;
                    }

                    if (enviarLinkNotas)
                    {
                        details.nfe = new List<dynamic>() { };

                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasStage = repositorioNotaFiscal.BuscarNotasPorStageECarga(stageCarga.Codigo, carga.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasStage)
                        {
                            if (codigosXmlNotaFiscalComRecusa.Contains(notaFiscal.Codigo))
                                continue;

                            details.nfe.Add(new
                            {
                                item = new
                                {
                                    fisDocType = notaFiscal.TipoDocumento == TipoDocumento.NFe ? "S" : "O",
                                    fisDocKey = notaFiscal?.Chave ?? string.Empty,
                                    docdat = notaFiscal?.DataEmissao.ToString("yyyy-MM-dd") ?? string.Empty,
                                    status = "100",
                                    totalWeight = (carga.TipoOperacao?.EnviarPesoLiquidoLinkNotas ?? false) ? notaFiscal.PesoLiquido : notaFiscal.Peso,
                                    value = notaFiscal.Valor,
                                    //nFunt = repositorioNotaProduto.ObterSiglaUnidadeMediaPorNota(notaFiscal.Codigo) ?? string.Empty,
                                    nFunt = "KG",
                                    costRelevant = notaFiscal.IrrelevanteParaFrete ? "" : "x"
                                }
                            });
                        }
                    }

                    objetoRequisicao.UpdateFreightData.shipment.vpDetails.Add(new
                    {
                        detail = details
                    });
                }

                return objetoRequisicao;
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesCarga = new List<Dominio.Entidades.Embarcador.Pedidos.Stage>();

                if (stage != null)
                    stagesCarga.Add(stage);
                else
                {
                    Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
                    Repositorio.Embarcador.Pedidos.StageAgrupamento repositorioStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(_unitOfWork);
                    List<int> codigosAgrupamentos = repositorioStageAgrupamento.BuscarPorCargaDtCodigos(carga.Codigo);

                    if (codigosAgrupamentos.Count > 0)
                        stagesCarga.AddRange(repositorioStage.BuscarporCodigosAgrupamentos(codigosAgrupamentos));
                    else
                    {
                        stagesCarga.AddRange(repositorioStage.BuscarStagesPorCarga(carga.Codigo));
                    }
                }

                Dominio.Entidades.Veiculo reboque = (carga.VeiculosVinculados?.Count > 0) ? carga.VeiculosVinculados.ElementAt(0) : null;
                Dominio.Entidades.Veiculo segundoReboque = (carga.VeiculosVinculados?.Count > 1) ? carga.VeiculosVinculados.ElementAt(1) : null;

                foreach (Dominio.Entidades.Embarcador.Pedidos.Stage stageCarga in stagesCarga)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDaCarga;
                    dynamic details = new ExpandoObject();

                    details.stageID = stageCarga.NumeroStage ?? string.Empty;
                    details.plateTRCK = carga.Veiculo?.Placa ?? string.Empty;
                    details.axlesTRCK = carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                    details.plateBTRA = segundoReboque?.Placa ?? string.Empty;
                    details.ufTRLRID = carga.Veiculo?.Estado?.Sigla ?? string.Empty;
                    details.plateTRLR = reboque?.Placa ?? string.Empty;
                    details.axlesTRLR = reboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                    details.ufTRLRBLDID = reboque?.Estado?.Sigla ?? string.Empty;
                    details.axlesBTRA = segundoReboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                    details.ufZBTRAIN = segundoReboque?.Estado?.Sigla ?? string.Empty;

                    if (stage != null)
                        notasFiscaisDaCarga = repositorioNotaFiscal.BuscarNotasPorStageECarga(stage.Codigo, carga.Codigo);
                    else
                        notasFiscaisDaCarga = repositorioNotaFiscal.BuscarNotasPorStageECarga(stageCarga.Codigo, carga.Codigo);

                    if (notasFiscaisDaCarga.Count > 0)
                    {
                        details.nfe = new List<dynamic>() { };

                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscaisDaCarga)
                        {
                            details.nfe.Add(new
                            {
                                item = new
                                {
                                    fisDocType = notaFiscal.TipoDocumento == TipoDocumento.NFe ? "S" : "O",
                                    fisDocKey = notaFiscal?.Chave ?? string.Empty,
                                    docdat = notaFiscal?.DataEmissao.ToString("yyyy-MM-dd") ?? string.Empty,
                                    status = "100",
                                    totalWeight = notaFiscal.Peso,
                                    value = notaFiscal.Valor,
                                    //nFunt = repositorioNotaProduto.ObterSiglaUnidadeMediaPorNota(notaFiscal.Codigo) ?? string.Empty,
                                    nFunt = "KG",
                                    costRelevant = notaFiscal.IrrelevanteParaFrete ? "" : "x"
                                }
                            });
                        }
                    }

                    objetoRequisicao.UpdateFreightData.shipment.vpDetails.Add(new
                    {
                        detail = details
                    });
                }

                return objetoRequisicao;
            }
        }

        private dynamic ObterObjetoRequisicaoValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioNotaProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            dynamic objetoRequisicao = new
            {
                UpdateFreightData = new
                {
                    shipment = new
                    {
                        edoc = "S",
                        shipmentID = carga?.CodigoCargaEmbarcador ?? string.Empty,
                        vpDetails = new List<dynamic>() { },
                        ServAgent = ""
                    }
                }
            };

            if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
                List<int> codigosXmlNotaFiscalComRecusa = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCodigosXmlNotaFiscalComCTeRecusaPorCarga(carga.Codigo);
                List<int> codigosPedidosCarga = repCargaPedido.BuscarCodigosPedidoPorCarga(carga.Codigo);
                List<int> codigosCargaCTeComCheckinRecusado = repCargaCte.BuscarCodigosComCheckinRecusadoPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesCarga = repPedidoStage.BuscarPorListaPedidos(codigosPedidosCarga);
                List<int> codigosStagesRecusadas = new List<int>();

                foreach (int codigoCargaCTe in codigosCargaCTeComCheckinRecusado)
                {
                    int codigoStage = repositorioCargaCTe.BuscarCodigoStageRelevantePorCargaCTe(codigoCargaCTe);

                    if (codigoStage > 0)
                        codigosStagesRecusadas.Add(codigoStage);
                }

                foreach (Dominio.Entidades.Embarcador.Pedidos.Stage stageCarga in stagesCarga)
                {
                    bool enviarConfirmacaoPlaca = stageCarga.RelevanteVP && (stageCarga.StageAgrupamento?.PlacasConfirmadas ?? false);
                    bool enviarLinkNotas = !codigosStagesRecusadas.Contains(stageCarga.Codigo);

                    if (!enviarConfirmacaoPlaca && !enviarLinkNotas)
                        continue;

                    dynamic details = new ExpandoObject();

                    details.stageID = stageCarga.NumeroStage ?? string.Empty;
                    details.axlesTRCK = "";
                    details.axlesTRLR = "";
                    details.axlesBTRA = "";

                    if (enviarConfirmacaoPlaca)
                    {
                        details.plateTRCK = stageCarga.StageAgrupamento.Veiculo?.Placa ?? string.Empty;
                        details.axlesTRCK = stageCarga.StageAgrupamento.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.plateBTRA = stageCarga.StageAgrupamento.SegundoReboque?.Placa ?? string.Empty;
                        details.ufTRLRID = stageCarga.StageAgrupamento.Veiculo?.Estado?.Sigla ?? string.Empty;
                        details.plateTRLR = stageCarga.StageAgrupamento.Reboque?.Placa ?? string.Empty;
                        details.axlesTRLR = stageCarga.StageAgrupamento.Reboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.ufTRLRBLDID = stageCarga.StageAgrupamento.Reboque?.Estado?.Sigla ?? string.Empty;
                        details.axlesBTRA = stageCarga.StageAgrupamento.SegundoReboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.ufZBTRAIN = stageCarga.StageAgrupamento.SegundoReboque?.Estado?.Sigla ?? string.Empty;
                    }

                    if (enviarLinkNotas)
                    {
                        details.nfe = new List<dynamic>() { };

                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasStage = repositorioNotaFiscal.BuscarNotasPorStageECarga(stageCarga.Codigo, carga.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasStage)
                        {
                            if (codigosXmlNotaFiscalComRecusa.Contains(notaFiscal.Codigo))
                                continue;

                            details.nfe.Add(new
                            {
                                item = new
                                {
                                    fisDocType = notaFiscal.TipoDocumento == TipoDocumento.NFe ? "S" : "O",
                                    fisDocKey = notaFiscal?.Chave ?? string.Empty,
                                    docdat = notaFiscal?.DataEmissao.ToString("yyyy-MM-dd") ?? string.Empty,
                                    status = "100",
                                    totalWeight = (carga.TipoOperacao?.EnviarPesoLiquidoLinkNotas ?? false) ? notaFiscal.PesoLiquido : notaFiscal.Peso,
                                    value = notaFiscal.Valor,
                                    //nFunt = repositorioNotaProduto.ObterSiglaUnidadeMediaPorNota(notaFiscal.Codigo) ?? string.Empty,
                                    nFunt = "KG",
                                    costRelevant = notaFiscal.IrrelevanteParaFrete ? "" : "x"
                                }
                            });
                        }
                    }

                    objetoRequisicao.UpdateFreightData.shipment.vpDetails.Add(new
                    {
                        detail = details
                    });
                }

                return objetoRequisicao;
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesCarga = new List<Dominio.Entidades.Embarcador.Pedidos.Stage>();
                Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);

                stagesCarga.AddRange(repositorioStage.BuscarStagesPorCarga(carga.Codigo));

                Dominio.Entidades.Veiculo reboque = (carga.VeiculosVinculados?.Count > 0) ? carga.VeiculosVinculados.ElementAt(0) : null;
                Dominio.Entidades.Veiculo segundoReboque = (carga.VeiculosVinculados?.Count > 1) ? carga.VeiculosVinculados.ElementAt(1) : null;

                foreach (Dominio.Entidades.Embarcador.Pedidos.Stage stageCarga in stagesCarga)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDaCarga;
                    dynamic details = new ExpandoObject();

                    //details.plateTRCK = carga.Veiculo?.Placa ?? string.Empty;
                    //details.axlesTRCK = carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                    //details.plateBTRA = segundoReboque?.Placa ?? string.Empty;
                    //details.ufTRLRID = carga.Veiculo?.Estado?.Sigla ?? string.Empty;
                    //details.plateTRLR = reboque?.Placa ?? string.Empty;
                    //details.axlesTRLR = reboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                    //details.ufTRLRBLDID = reboque?.Estado?.Sigla ?? string.Empty;
                    //details.axlesBTRA = segundoReboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                    //details.ufZBTRAIN = segundoReboque?.Estado?.Sigla ?? string.Empty;

                    details.stageID = stageCarga.NumeroStage ?? string.Empty;


                    if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                    {
                        details.plateTRCK = stageCarga.StageAgrupamento.Veiculo?.Placa ?? string.Empty;
                        details.axlesTRCK = stageCarga.StageAgrupamento.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.plateBTRA = stageCarga.StageAgrupamento.SegundoReboque?.Placa ?? string.Empty;
                        details.ufTRLRID = stageCarga.StageAgrupamento.Veiculo?.Estado?.Sigla ?? string.Empty;
                        details.plateTRLR = stageCarga.StageAgrupamento.Reboque?.Placa ?? string.Empty;
                        details.axlesTRLR = stageCarga.StageAgrupamento.Reboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.ufTRLRBLDID = stageCarga.StageAgrupamento.Reboque?.Estado?.Sigla ?? string.Empty;
                        details.axlesBTRA = stageCarga.StageAgrupamento.SegundoReboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.ufZBTRAIN = stageCarga.StageAgrupamento.SegundoReboque?.Estado?.Sigla ?? string.Empty;
                    }
                    else
                    {
                        details.plateTRCK = stageCarga.StageAgrupamento?.Veiculo?.Placa ?? carga.Veiculo?.Placa ?? string.Empty;
                        details.axlesTRCK = stageCarga.StageAgrupamento?.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.plateBTRA = stageCarga.StageAgrupamento?.SegundoReboque?.Placa ?? segundoReboque?.Placa ?? string.Empty;
                        details.ufTRLRID = stageCarga.StageAgrupamento?.Veiculo?.Estado?.Sigla ?? carga.Veiculo?.Estado?.Sigla ?? string.Empty;
                        details.plateTRLR = stageCarga.StageAgrupamento?.Reboque?.Placa ?? reboque?.Placa ?? string.Empty;
                        details.axlesTRLR = stageCarga.StageAgrupamento?.Reboque?.ModeloVeicularCarga?.NumeroEixos ?? reboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.ufTRLRBLDID = stageCarga.StageAgrupamento?.Reboque?.Estado?.Sigla ?? reboque?.Estado?.Sigla ?? string.Empty;
                        details.axlesBTRA = stageCarga.StageAgrupamento?.SegundoReboque?.ModeloVeicularCarga?.NumeroEixos ?? segundoReboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                        details.ufZBTRAIN = stageCarga.StageAgrupamento?.SegundoReboque?.Estado?.Sigla ?? segundoReboque?.Estado?.Sigla ?? string.Empty;

                    }

                    notasFiscaisDaCarga = repositorioNotaFiscal.BuscarNotasPorStageECarga(stageCarga.Codigo, carga.Codigo);

                    if (notasFiscaisDaCarga.Count > 0)
                    {
                        details.nfe = new List<dynamic>() { };

                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscaisDaCarga)
                        {
                            details.nfe.Add(new
                            {
                                item = new
                                {
                                    fisDocType = notaFiscal.TipoDocumento == TipoDocumento.NFe ? "S" : "O",
                                    fisDocKey = notaFiscal?.Chave ?? string.Empty,
                                    docdat = notaFiscal?.DataEmissao.ToString("yyyy-MM-dd") ?? string.Empty,
                                    status = "100",
                                    totalWeight = notaFiscal.Peso,
                                    value = notaFiscal.Valor,
                                    //nFunt = repositorioNotaProduto.ObterSiglaUnidadeMediaPorNota(notaFiscal.Codigo) ?? string.Empty,
                                    nFunt = "KG",
                                    costRelevant = notaFiscal.IrrelevanteParaFrete ? "" : "x"
                                }
                            });
                        }
                    }

                    objetoRequisicao.UpdateFreightData.shipment.vpDetails.Add(new
                    {
                        detail = details
                    });
                }

                return objetoRequisicao;
            }
        }

        private dynamic ObterObjetoPreChekinTranferencia(Dominio.Entidades.Embarcador.Cargas.Carga carga, string numeroStage)
        {
            return new
            {
                DOCUMENT = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.DOCUMENT()
                {
                    SAPDOCTYPE = (carga?.CargaGeradaViaDocumentoTransporte ?? false) || carga?.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga ? "S" : "O",
                    BASE64FILE = "",
                    DOCISSUANCE = "N",
                    STATUS = "",
                    NumeroCarga = carga?.CodigoCargaEmbarcador ?? string.Empty,
                    STAGE = numeroStage,
                    FISDOCKEY = "",
                    FISDOCTYPE = "",
                    REFDOCKEY = "",
                    UNIDOCS = ""
                }
            };
        }

        private byte[] ObterXmlRequisicaoPorTipo(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

            if (documentoDestinadoEmpresa.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFeDestinada)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinadoEmpresa.Chave + ".xml");
            else if (documentoDestinadoEmpresa.TipoDocumento == TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", documentoDestinadoEmpresa.Empresa.CNPJ, documentoDestinadoEmpresa.Chave + ".xml");

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                throw new ServicoException("Xml do documento destinado não encontrado ou ainda não recebido da Sefaz.");

            return Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
        }

        private dynamic ObterObjetoRequisicaoStatusDocumento(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa)
        {
            dynamic objeto = null;
            string dataRecibidoDocumento = documentoDestinadoEmpresa?.DataIntegracao?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? string.Empty;
            string chave = documentoDestinadoEmpresa?.Chave;


            List<TipoDocumentoDestinadoEmpresa> tipoNotaAceitas = new List<TipoDocumentoDestinadoEmpresa>() {
                TipoDocumentoDestinadoEmpresa.CancelamentoNFe
            };

            if (tipoNotaAceitas.Contains(documentoDestinadoEmpresa.TipoDocumento))
            {
                objeto = new
                {
                    validateFiscalDocument = new
                    {
                        nfeAccessKeyInfo = new dynamic[1]
                    }
                };

                objeto.validateFiscalDocument.nfeAccessKeyInfo[0] = new
                {
                    accessKey = chave,
                    zStatusCheck = "101",
                    zValDatetime = dataRecibidoDocumento
                };

                return objeto;
            }

            List<TipoDocumentoDestinadoEmpresa> tipoCteAceitos = new List<TipoDocumentoDestinadoEmpresa>() {
                TipoDocumentoDestinadoEmpresa.CancelamentoCTe,
            };

            if (!tipoCteAceitos.Contains(documentoDestinadoEmpresa.TipoDocumento))
                throw new ServicoException("Tipo de documento não valido para integração");

            objeto = new
            {
                validateFiscalDocument = new
                {
                    requestType = "A",
                    accessKeyInfo = new dynamic[1]
                }
            };

            objeto.validateFiscalDocument.accessKeyInfo[0] = new
            {
                accessKey = documentoDestinadoEmpresa?.Chave,
                zStatusCheck = "101",
                zValDatetime = dataRecibidoDocumento
            };

            return objeto;
        }

        public void GerarIntegracoes(Dominio.Entidades.Embarcador.Cargas.Carga cargaDt, List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stageAgrupamentos = null, bool removendoPlacas = false, bool enviarIntegracaoTransfEntrega = true)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoGerarRegistros = new List<TipoIntegracao>()
            {
                TipoIntegracao.UnileverDadosValePedagio,
                TipoIntegracao.DigitalCom
            };

            foreach (var tipoIntegracao in tiposIntegracaoGerarRegistros)
            {
                if (tipoIntegracao != TipoIntegracao.DigitalCom || removendoPlacas)
                {
                    Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaDadosTransporteIntegracao(cargaDt, repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao), _unitOfWork, true, false, true);
                    continue;
                }

                if (stageAgrupamentos == null || stageAgrupamentos.Count == 0)
                    continue;

                foreach (var agrupamento in stageAgrupamentos)
                {
                    string placa = agrupamento?.Veiculo?.Placa ?? "";

                    if (!enviarIntegracaoTransfEntrega)
                        continue;

                    if (!agrupamento.PlacasConfirmadas || string.IsNullOrEmpty(placa))
                        continue;

                    if (repositorioIntegracao.ExistePorProtocoloECarga(placa, cargaDt.Codigo))
                        continue;

                    Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaDadosTransporteIntegracao(cargaDt, repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao), _unitOfWork, true, false, true, agrupamento?.Veiculo?.Placa ?? "");
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.RequestInvoiceProcessing ObterRequestPagamento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento, bool cancelamento)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repEntregaNF = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Contabeis.DireitoFiscal repDireitoFiscal = new Repositorio.Embarcador.Contabeis.DireitoFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(_unitOfWork);

            Servicos.Embarcador.Pedido.Stage svcStage = new Servicos.Embarcador.Pedido.Stage();

            if (documento == null || documento.CTe == null)
                throw new ServicoException("Não existe documento para a integração");

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(documento.CTe.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repXMLNotaFiscal.BuscarPorCTe(documento.CTe);

            if (cargaCTe?.Carga == null || cargaCTe.CTe == null)
                throw new ServicoException("Não existe documento para a integração");

            List<Dominio.Entidades.Embarcador.Pedidos.Stage> stages = repStage.BuscarPorCargaCTe(cargaCTe.Carga.Codigo, cargaCTe.CTe.Codigo);

            if (stages == null || stages.Count == 0)
                throw new ServicoException("Não existe documento para a integração");

            Dominio.Enumeradores.TipoDocumento tipoDocumento = documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.RequestInvoiceProcessing obj = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.RequestInvoiceProcessing() { };

            obj.accessKey = ObterAcessKey(documento);
            obj.objectName = documento.CTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? "CTE" : "NFE";
            obj.companyCode = ObterCompanyCode(documento.CTe.Tomador?.CPF_CNPJ ?? string.Empty);
            obj.vendorID = !string.IsNullOrWhiteSpace(documento.Empresa?.CodigoIntegracao) ? documento.Empresa.CodigoIntegracao[0] == 'T' ? documento.Empresa.CodigoIntegracao.Substring(1) : documento.Empresa.CodigoIntegracao : string.Empty;
            obj.nfType = ObterNFType(documento);
            obj.invoiceNumber = $"{documento.CTe.Numero}-{documento.CTe.Serie?.Numero ?? 0}";
            obj.invoiceDate = documento.DataEmissao.ToString("yyyy-MM-dd");
            obj.referenceDocumentCategory = "4";
            obj.cancellationFlag = cancelamento ? "true" : string.Empty;
            obj.annulationFlag = string.Empty;
            obj.annulationReason = string.Empty;
            obj.inOutState = documento.Origem?.Estado?.Sigla == documento.Destino?.Estado?.Sigla ? "INSTATE" : "OUTSTATE";
            obj.transactionCode = ObterTransactionCode(documento.CTe);
            obj.headerText = "Post Multi";
            obj.headerTextSummary = $"{DateTime.Now.ToString("yyyyMMdd")}_Multi_{documento.Pagamento?.Carga?.CodigoCargaEmbarcador ?? "0"}";
            obj.totalAmount = Math.Round(documento.ValorAFaturar, 2);
            obj.valueNet = Math.Round(documento.ValorLiquido, 2);
            obj.protocolNumber = documento.CTe.Protocolo ?? string.Empty;
            obj.checkDigit = documento.CTe.Chave?.Right(1);
            obj.nfeIssType = documento.CTe.Chave?.Substring(34, 1) ?? string.Empty;
            obj.randomNumber = documento.CTe.Chave?.Substring(35, 8) ?? string.Empty;
            obj.baselineDate = documento.DataEmissao.ToString("yyyy-MM-dd");
            obj.complementSESFlag = documento.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento ? true : false;
            obj.freeFreightFlag = false;
            obj.complementarOccurrenceGrossValue = Math.Round(documento.Pagamento?.CargaOcorrencia?.ValorOcorrencia ?? 0, 2);
            obj.complementarOccurrenceIcmsValue = Math.Round(documento.Pagamento?.CargaOcorrencia?.ValorICMS ?? 0, 2);
            obj.complementarOccurrenceConfinsValue = Math.Round(0m, 2);
            obj.complementarOccurrencePisValue = Math.Round(0m, 2);
            obj.cnpjDest = documento.CTe.Destinatario?.CPF_CNPJ ?? string.Empty;
            obj.cnpjUnl = documento.CTe.Tomador?.CPF_CNPJ ?? string.Empty;
            obj.items = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.Item>();


            foreach (var stage in stages)
            {

                if (stage == null)
                    throw new ServicoException("Não foi possível encontrar o documento da provisão");

                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentosContabeis = repDocumentoContabil.BuscarPorStage(stage.Codigo);

                List<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado> impostosValorAgregado = documentosContabeis.Select(docCont => docCont.ImpostoValorAgregado).Distinct().ToList();
                Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado = (impostosValorAgregado.Count == 1) ? impostosValorAgregado.FirstOrDefault() : null;

                var dados = documentosContabeis
                        .GroupBy(documentoCont => ValueTuple.Create(documentoCont.Stage?.Codigo ?? 0, documentoCont.Stage?.NumeroStage ?? string.Empty))
                        .Select(documentoAgrupado => Servicos.Embarcador.Pedido.Stage.ObterDocumentoContabelPorStage(documentoAgrupado, _unitOfWork))
                        .ToList();

                string cfop = ObterCFOP(documento);

                dynamic dadosDocumento = dados.Where(x => (int)x.CodigoStage == stage.Codigo).FirstOrDefault();

                Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal direitoFiscal = repDireitoFiscal.BuscarPorIVA(impostoValorAgregado?.Codigo ?? 0);
                if (direitoFiscal == null)
                    throw new ServicoException("Falta de configuração de Direito Fiscal");

                decimal.TryParse((string)dadosDocumento.FreteLiquido, out decimal frete);
                decimal.TryParse((string)dadosDocumento.AliquotaIcms, out decimal aliquotaIcms);
                decimal.TryParse((string)dadosDocumento.FreteTotal, out decimal freteTotal);
                decimal.TryParse((string)dadosDocumento.Icms, out decimal valorIcms);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.Item item = new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.Item()
                {
                    sesNumber = stage.NumeroFolha?.ToString() ?? string.Empty,
                    sesNetValue = Math.Round(frete, 2),
                    taxCode = (string)dadosDocumento.IVA ?? string.Empty,
                    valorBaseIcms = Math.Round(freteTotal, 2),
                    aliquotaIcms = aliquotaIcms,
                    valorIcmsSt = Math.Round(valorIcms, 2),
                    cfopInboundItem = cfop,
                };
                item.taxes = ObterTaxes((string)dadosDocumento.IVA, cfop, dadosDocumento, direitoFiscal, tipoDocumento, documento, freteTotal, documentosContabeis);

                obj.items.Add(item);
            }

            obj.withholdItems = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.WithholdItem>();
            obj.withholdItems.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.WithholdItem()
            {
                withholdingTaxType = (documento.CTe?.ISSRetido ?? false) ? "IS" : string.Empty,
                withholdingTaxCode = (documento.CTe?.ISSRetido ?? false) ? ObterISSRetido(documento.CTe) : string.Empty,
                withholdingTaxBaseAmount = (documento.CTe?.ISSRetido ?? false) ? documento.CTe.ValorAReceber.ToString() : string.Empty,
                withholdingTaxApplyFlag = documento.CTe?.ISSRetido ?? false
            });

            return obj;
        }

        private string ObterCFOP(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento)
        {

            string cfop = string.Empty;
            Servicos.Embarcador.NFSe.NFSe serNFSe = new Servicos.Embarcador.NFSe.NFSe(_unitOfWork);
            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = serNFSe.BuscarConfiguracaoEmissaoNFSe(documento.Empresa.Codigo, documento.Tomador?.Localidade?.Codigo ?? 0, documento.Tomador?.Localidade?.Estado?.Sigla ?? "", documento.Tomador?.GrupoPessoas?.Codigo ?? 0, documento.Tomador?.Localidade?.Codigo ?? 0, documento.Carga?.TipoOperacao?.Codigo ?? 0, documento.Tomador?.CPF_CNPJ ?? 0, 0, _unitOfWork);

            string numeroCFOP = documento.CTe?.CFOP?.CodigoCFOP.ToString() ?? string.Empty;
            if (documento.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                bool mesmoUF = CompararMesmoUFClienteEmpresa(documento.Empresa, documento.Tomador);
                if ((numeroCFOP == "5932" || numeroCFOP == "6932") && mesmoUF)
                    return "1932";

                if ((numeroCFOP == "5932" || numeroCFOP == "6932") && !mesmoUF)
                    return "2932";

                if (!(numeroCFOP == "5932" || numeroCFOP == "6932") && mesmoUF && (documento.Tomador?.Atividade?.Codigo ?? 0) == 2)
                    return "1352";

                if (!(numeroCFOP == "5932" || numeroCFOP == "6932") && !mesmoUF && (documento.Tomador?.Atividade?.Codigo ?? 0) == 2)
                    return "2352";

                if (!(numeroCFOP == "5932" || numeroCFOP == "6932") && mesmoUF && !((documento.Tomador?.Atividade?.Codigo ?? 0) == 2))
                    return "1353";

                if (!(numeroCFOP == "5932" || numeroCFOP == "6932") && !mesmoUF && !((documento.Tomador?.Atividade?.Codigo ?? 0) == 2))
                    return "2353";
            }
            else if ((documento.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS) || (documento.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito && documento.CargaOcorrenciaPagamento != null))
            {
                if ((documento.Tomador?.Localidade?.Estado?.Sigla ?? string.Empty) == (transportadorConfiguracaoNFSe.LocalidadePrestacao?.Estado?.Sigla ?? string.Empty))
                    return "1999";

                if (!((documento.Tomador?.Localidade?.Estado?.Sigla ?? string.Empty) == (transportadorConfiguracaoNFSe.LocalidadePrestacao?.Estado?.Sigla ?? string.Empty)))
                    return "2999";
            }

            return cfop;
        }

        private string ObterNFType(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento)
        {
            if (documento.CTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                return "CE";
            if (documento.CTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                return "Z5";
            if (documento.CTe.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Credito)
                return "EM";
            if (documento.CTe.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito)
                return "SI";
            return string.Empty;
        }

        private string ObterAcessKey(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento)
        {
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = documento.CTe;
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            if (cte.ModeloDocumentoFiscal == null)
                return cte.ChaveAcesso;
            if (new List<Dominio.Enumeradores.TipoDocumento>() { Dominio.Enumeradores.TipoDocumento.NFS, Dominio.Enumeradores.TipoDocumento.NFSe }.Contains(cte.ModeloDocumentoFiscal.TipoDocumentoEmissao))
            {
                string accessKey = string.Empty;
                accessKey += !string.IsNullOrWhiteSpace(cte.Empresa?.Localidade?.Estado?.CodigoIBGE.ToString()) ? cte.Empresa.Localidade.Estado.CodigoIBGE.ToString("D2") : "0";
                accessKey += cte.DataEmissao.HasValue ? cte.DataEmissao.Value.Year.ToString().Substring(2, 2) : "0";
                accessKey += cte.DataEmissao.HasValue ? cte.DataEmissao.Value.Month.ToString() : "0";
                accessKey += (cte.Empresa?.CNPJ ?? "0");
                accessKey += "99";
                accessKey += (cte.Serie?.Numero.ToString("D3") ?? "0");
                accessKey += (documento.Numero.ToInt().ToString("D9") ?? "0");
                accessKey += "14";
                accessKey += new Random().Next(1000000).ToString("D6");
                accessKey += Utilidades.Calc.Modulo11(accessKey).ToString().ToInt().ToString("D2");

                cte.Chave = accessKey;
                repCTe.Atualizar(cte);

                return accessKey;
            }
            return cte.ChaveAcesso;
        }

        private string ObterCompanyCode(string cnpj)
        {
            cnpj = Utilidades.String.OnlyNumbers(cnpj);
            if (cnpj.Contains("61068276"))
                return "2236";
            if (cnpj.Contains("11806723"))
                return "5454";
            if (cnpj.Contains("01615814"))
                return "9311";
            return string.Empty;
        }

        private string ObterTransactionCode(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                return "1";
            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                return "3";
            return string.Empty;
        }

        private bool CompararMesmoUFClienteEmpresa(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente cliente)
        {
            if (empresa == null || cliente == null)
                return false;
            return (empresa.Localidade?.Estado?.Sigla ?? string.Empty) == (cliente.Localidade?.Estado?.Sigla ?? string.Empty);
        }

        private string ObterISSRetido(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            string retorno = string.Empty;
            switch (cte.AliquotaISS)
            {
                case 2:
                    retorno = "S2";
                    break;
                case 3:
                    retorno = "S3";
                    break;
                case 4:
                    retorno = "S4";
                    break;
                case 5:
                    retorno = "S5";
                    break;
                case 2.5m:
                    retorno = "S7";
                    break;
            }
            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.Taxis> ObterTaxes(string iva, string cfop, dynamic dadosDocumento, Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal direitoFiscal, Dominio.Enumeradores.TipoDocumento tipoDocumento, Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento, decimal baseCalculoIcms, List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentosContabeis)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.Taxis> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.Taxis>();
            List<string> impostos = new List<string>();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            switch (iva)
            {
                case "XA":
                    impostos = new List<string>() { "ZIC1", "IPI0", "IPIS", "ICOF" };
                    break;
                case "FZ":
                    impostos = new List<string>() { "ICM0", "IPI0", "IPIS", "ICOF" };
                    break;
                case "FK":
                    impostos = new List<string>() { "ICM0", "IPI0", "IPIS", "ICOF" };
                    break;
                case "XG":
                    impostos = new List<string>() { "ICM0", "IPI0", "IPIS", "ZCON" };
                    break;
                case "XC":
                    impostos = new List<string>() { "ZIC2", "IPI0", "IPIS", "ICOF" };
                    break;
                case "XE":
                    impostos = new List<string>() { "ZIC2", "IPI0", "IPIS", "ZCON" };
                    break;
                case "FH":
                    impostos = new List<string>() { "IPIS", "ZCON", "ZISS" };
                    break;
                case "FG":
                    impostos = new List<string>() { "IPIS", "ICOF", "ZISS" };
                    break;
                case "FI":
                    impostos = new List<string>() { "ZPIS", "ZCOF", "ZISS" };
                    break;
                default:
                    return retorno;
            }

            List<string> impostosPisCofins = new List<string>
            {
                "IPIS",
                "ZPIS",
                "ICOF",
                "ZCON",
                "ZCOF"
            };

            decimal valorIpi = documentosContabeis.Sum(x => x.XMLNotaFiscal?.ValorIPI ?? 0);

            foreach (string imposto in impostos)
            {
                decimal baseICMS = configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins && impostosPisCofins.Contains(imposto) ? (baseCalculoIcms - documento.ValorICMS) : baseCalculoIcms;
                retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Unilever.Provisao.Taxis()
                {
                    taxType = imposto,
                    cfopInbound = cfop,
                    rate = Math.Round(ObterRateImposto(dadosDocumento, imposto, documento.CTe.CFOP?.AliquotaIPI ?? 0), 2),
                    icmsLaw = direitoFiscal.ICMS_ISS ?? string.Empty,
                    ipiLaw = direitoFiscal.IPI ?? string.Empty,
                    cofinsLaw = direitoFiscal.COFINS ?? string.Empty,
                    pisLaw = direitoFiscal.PIS ?? string.Empty,
                    issLaw = direitoFiscal.ICMS_ISS ?? string.Empty,
                    taxValue = Math.Round(ObterValorImposto(dadosDocumento, imposto, valorIpi), 2),
                    baseAmount = Math.Round((iva == "XA" && imposto != "IPI0") ? baseICMS : 0, 2),
                    otherBase = Math.Round((iva == "XA" && imposto == "IPI0") ? baseICMS : 0, 2)
                });
            }
            return retorno;
        }

        public decimal ObterRateImposto(dynamic dadosDocumento, string imposto, decimal aliquotaIpi)
        {
            decimal.TryParse((string)dadosDocumento.AliquotaIcms, out decimal aliquotaIcms);
            decimal.TryParse((string)dadosDocumento.AliquotaPis, out decimal aliquotaPis);
            decimal.TryParse((string)dadosDocumento.AliquotaCofins, out decimal aliquotaCofins);
            decimal.TryParse((string)dadosDocumento.AliquotaIss, out decimal AliquotaIss);

            switch (imposto)
            {
                case "ZIC1":
                case "ICM0":
                case "ZIC2":
                    return aliquotaIcms;
                case "IPI0":
                    return aliquotaIpi;
                case "IPIS":
                case "ZPIS":
                    return aliquotaPis;
                case "ICOF":
                case "ZCON":
                case "ZCOF":
                    return aliquotaCofins;
                case "ZISS":
                    return AliquotaIss;
                default:
                    return 0;
            }
        }

        public decimal ObterValorImposto(dynamic dadosDocumento, string imposto, decimal valorIPI)
        {
            decimal.TryParse((string)dadosDocumento.Icms, out decimal Icms);
            decimal.TryParse((string)dadosDocumento.Pis, out decimal Pis);
            decimal.TryParse((string)dadosDocumento.Cofins, out decimal Cofins);
            decimal.TryParse((string)dadosDocumento.Iss, out decimal Iss);
            switch (imposto)
            {
                case "ZIC1":
                case "ICM0":
                case "ZIC2":
                    return Icms;

                case "IPI0":
                    return valorIPI;

                case "IPIS":
                case "ZPIS":
                    return Pis;

                case "ICOF":
                case "ZCON":
                case "ZCOF":
                    return Cofins;

                case "ZISS":
                    return Iss;
                default:
                    return 0;
            }
        }

        private dynamic ObterDadosDocumento(List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentoAgrupado)
        {
            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentosContabeis = documentoAgrupado.ToList();
            List<Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado> impostosValorAgregado = documentosContabeis.Select(documento => documento.ImpostoValorAgregado).Distinct().ToList();
            Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado = (impostosValorAgregado.Count == 1) ? impostosValorAgregado.FirstOrDefault() : null;

            decimal aliquotaCofins = 0m;
            decimal aliquotaIcms = 0m;
            decimal aliquotaIss = 0m;
            decimal aliquotaPis = 0m;
            decimal valorTotalAdValorem = 0m;
            decimal valorTotalCofins = 0m;
            decimal valorTotalIcms = 0m;
            decimal valorTotalIcmsST = 0m;
            decimal valorTotalIss = 0m;
            decimal valorTotalIssRetido = 0m;
            decimal valorTotalFreteLiquido = 0m;
            decimal valorTotalGris = 0m;
            decimal valorTotalPedagio = 0m;
            decimal valorTotalPis = 0m;
            decimal valorTotalReceber = 0m;
            decimal valorTotalTaxaDescarga = 0m;
            decimal valorTotalTaxaEntrega = 0m;
            decimal valorTotalCustoFixo = 0m;
            decimal valorTotalFreteCaixa = 0m;
            decimal valorTotalFreteKM = 0m;
            decimal valorTotalFretePeso = 0m;
            decimal valorTotalFreteViagem = 0m;
            decimal valorTotalTaxa = 0m;
            decimal valorTotalPernoite = 0m;

            foreach (Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documentoContabil in documentosContabeis)
            {
                if (documentoContabil.TipoContaContabil == TipoContaContabil.AdValorem)
                    valorTotalAdValorem += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido || documentoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido2 || documentoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido9)
                    valorTotalFreteLiquido += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TotalReceber)
                    valorTotalReceber += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.Pedagio)
                    valorTotalPedagio += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TaxaDescarga)
                    valorTotalTaxaDescarga += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TaxaEntrega)
                    valorTotalTaxaEntrega += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ICMSST)
                    valorTotalIcmsST += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.GRIS)
                    valorTotalGris += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.CustoFixo)
                    valorTotalCustoFixo += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteCaixa)
                    valorTotalFreteCaixa += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteKM)
                    valorTotalFreteKM += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FretePeso)
                    valorTotalFretePeso += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.FreteViagem)
                    valorTotalFreteViagem += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.TaxaTotal)
                    valorTotalTaxa += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.Pernoite)
                    valorTotalPernoite += documentoContabil.ValorContabilizacao;
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ISS)
                {
                    aliquotaIss = documentoContabil.AliquotaIss;
                    valorTotalIss += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ISSRetido)
                {
                    aliquotaIss = documentoContabil.AliquotaIss;
                    valorTotalIssRetido += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.ICMS)
                {
                    aliquotaIcms = documentoContabil.DocumentoProvisao?.PercentualAliquota ?? 0m;
                    valorTotalIcms += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.PIS)
                {
                    aliquotaPis = documentoContabil.AliquotaPis;
                    valorTotalPis += documentoContabil.ValorContabilizacao;
                }
                else if (documentoContabil.TipoContaContabil == TipoContaContabil.COFINS)
                {
                    aliquotaCofins = documentoContabil.AliquotaCofins;
                    valorTotalCofins += documentoContabil.ValorContabilizacao;
                }
            }

            return new
            {
                IVA = impostoValorAgregado?.CodigoIVA ?? "",
                AdValorem = (valorTotalAdValorem > 0) ? valorTotalAdValorem.ToString("n2") : "",
                AliquotaCofins = (aliquotaCofins > 0) ? aliquotaCofins.ToString("n2") : "",
                AliquotaIcms = (aliquotaIcms > 0) ? aliquotaIcms.ToString("n2") : "",
                AliquotaIss = (aliquotaIss > 0) ? aliquotaIss.ToString("n2") : "",
                AliquotaPis = (aliquotaPis > 0) ? aliquotaPis.ToString("n2") : "",
                Cofins = (valorTotalCofins > 0) ? valorTotalCofins.ToString("n2") : "",
                Icms = (valorTotalIcms > 0) ? valorTotalIcms.ToString("n2") : "",
                IcmsST = (valorTotalIcmsST > 0) ? valorTotalIcmsST.ToString("n2") : "",
                Iss = (valorTotalIss > 0) ? valorTotalIss.ToString("n2") : "",
                IssRetido = (valorTotalIssRetido > 0) ? valorTotalIssRetido.ToString("n2") : "",
                CustoFixo = (valorTotalCustoFixo > 0) ? valorTotalCustoFixo.ToString("n2") : "",
                FreteCaixa = (valorTotalFreteCaixa > 0) ? valorTotalFreteCaixa.ToString("n2") : "",
                FreteKM = (valorTotalFreteKM > 0) ? valorTotalFreteKM.ToString("n2") : "",
                FretePeso = (valorTotalFretePeso > 0) ? valorTotalFretePeso.ToString("n2") : "",
                FreteViagem = (valorTotalFreteViagem > 0) ? valorTotalFreteViagem.ToString("n2") : "",
                FreteLiquido = (valorTotalFreteLiquido > 0) ? valorTotalFreteLiquido.ToString("n2") : "",
                FreteTotal = (valorTotalReceber > 0) ? valorTotalReceber.ToString("n2") : "",
                Gris = (valorTotalGris > 0) ? valorTotalGris.ToString("n2") : "",
                Pedagio = (valorTotalPedagio > 0) ? valorTotalPedagio.ToString("n2") : "",
                Pis = (valorTotalPis > 0) ? valorTotalPis.ToString("n2") : "",
                TaxaDescarga = (valorTotalTaxaDescarga > 0) ? valorTotalTaxaDescarga.ToString("n2") : "",
                TaxaEntrega = (valorTotalTaxaEntrega > 0) ? valorTotalTaxaEntrega.ToString("n2") : "",
                TaxaTotal = (valorTotalTaxa > 0) ? valorTotalTaxa.ToString("n2") : "",
                Pernoite = (valorTotalPernoite > 0) ? valorTotalPernoite.ToString("n2") : ""
            };
        }

        private string ObterFISDOCTYPE(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte == null)
                return "B";

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
            {
                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                    return "A";

                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                    return "B";

                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
                    return "C";
            }

            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                    return "D";

                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                    return "E";
            }

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && cte.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim)
                return "H";

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto && cte.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim)
                return "K";

            return "J";
        }

        private dynamic ObterDadosRequisicaoUnileverBase(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool integracaoInfrutifera)
        {
            dynamic objetoRequisicao = new
            {
                UpdateFreightData = new
                {
                    occurrence = new
                    {
                        occurrenceNumber = ""
                    },
                    vehiclePlate = string.Empty,
                    shipment = new
                    {
                        edoc = "S",
                        MultiSoftware = "S",
                        shipmentID = carga?.CodigoCargaEmbarcador ?? string.Empty,
                        vpDetails = !integracaoInfrutifera ? ObterDadosRequisicaoUnileverRecusaVpDetalhes(carga) : ObterDadosInfrutifera(carga),
                    }
                }
            };

            return objetoRequisicao;
        }
        private List<dynamic> ObterDadosInfrutifera(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repositorioChamadoOcorrencia = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
            List<int> codigosPedidosCarga = repCargaPedido.BuscarCodigosPedidoPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesCarga = repPedidoStage.BuscarPorListaPedidos(codigosPedidosCarga);
            List<dynamic> listdetails = new List<dynamic>();

            Dominio.Entidades.Embarcador.Chamados.Chamado existeChamdo = repositorioChamadoOcorrencia.BuscarPorCarga(carga.Codigo);

            if (existeChamdo == null)
                throw new ServicoException("Não existe Atendimento para esta carga, para enviar integração");

            foreach (var stage in stagesCarga)
            {
                dynamic details = new ExpandoObject();
                details.stageID = stage.NumeroStage;
                details.plateTRCK = string.Empty;
                details.axlesTRCK = string.Empty;
                details.plateBTRA = string.Empty;
                details.ufTRLRID = string.Empty;
                details.plateTRLR = string.Empty;
                details.axlesTRLR = string.Empty;
                details.ufTRLRBLDID = string.Empty;
                details.axlesBTRA = string.Empty;
                details.ufZBTRAIN = string.Empty;
                details.empty = "Y";

                listdetails.Add(new
                {
                    detail = details
                });
            }
            return listdetails;
        }

        private List<dynamic> ObterDadosRequisicaoUnileverRecusaVpDetalhes(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(_unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repositorioChamadoOcorrencia = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioNotaProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            List<int> codigosPedidosCarga = repCargaPedido.BuscarCodigosPedidoPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesCarga = repPedidoStage.BuscarPorListaPedidos(codigosPedidosCarga);
            List<dynamic> listdetails = new List<dynamic>();

            Dominio.Entidades.Veiculo reboque = (carga.VeiculosVinculados?.Count > 0) ? carga.VeiculosVinculados.ElementAt(0) : null;
            Dominio.Entidades.Veiculo segundoReboque = (carga.VeiculosVinculados?.Count > 1) ? carga.VeiculosVinculados.ElementAt(1) : null;
            Dominio.Entidades.Embarcador.Chamados.Chamado existeChamdo = repositorioChamadoOcorrencia.BuscarPorCarga(carga.Codigo);

            if (existeChamdo == null)
                throw new ServicoException("Não existe Atendimento para esta carga, para enviar integração");

            foreach (var stage in stagesCarga)
            {

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscaisDaCarga = repositorioNotaFiscal.BuscarNotasPorStageECarga(stage.Codigo, carga.Codigo);
                Dominio.Entidades.Cliente destinatario = existeChamdo.Cliente;

                if (!notasFiscaisDaCarga.Any(x => x.Destinatario.CPF_CNPJ == destinatario.CPF_CNPJ))
                    continue;

                dynamic details = new ExpandoObject();
                details.stageID = stage.NumeroStage;
                details.plateTRCK = carga.Veiculo?.Placa ?? string.Empty;
                details.axlesTRCK = carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                details.plateBTRA = segundoReboque?.Placa ?? string.Empty;
                details.ufTRLRID = carga.Veiculo?.Estado?.Sigla ?? string.Empty;
                details.plateTRLR = reboque?.Placa ?? string.Empty;
                details.axlesTRLR = reboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                details.ufTRLRBLDID = reboque?.Estado?.Sigla ?? string.Empty;
                details.axlesBTRA = segundoReboque?.ModeloVeicularCarga?.NumeroEixos ?? 0;
                details.ufZBTRAIN = segundoReboque?.Estado?.Sigla ?? string.Empty;


                if (notasFiscaisDaCarga.Count > 0)
                {
                    details.nfe = new List<dynamic>() { };

                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscaisDaCarga)
                    {
                        details.nfe.Add(new
                        {
                            item = new
                            {
                                fisDocType = notaFiscal.TipoDocumento == TipoDocumento.NFe ? "S" : "O",
                                fisDocKey = notaFiscal?.Chave ?? string.Empty,
                                docdat = notaFiscal?.DataEmissao.ToString("yyyy-MM-dd") ?? string.Empty,
                                status = "100",
                                totalWeight = notaFiscal.Peso,
                                value = notaFiscal.Valor,
                                //nFunt = repositorioNotaProduto.ObterSiglaUnidadeMediaPorNota(notaFiscal.Codigo) ?? string.Empty,
                                nFunt = "KG",
                                costRelevant = notaFiscal.IrrelevanteParaFrete ? "" : "x",
                                refusal = existeChamdo.XMLNotasFiscais.Any(x => x.Codigo == notaFiscal.Codigo) ? "Y" : "",
                                accident = ""
                            }
                        });
                    }
                }

                listdetails.Add(new
                {
                    detail = details
                });
            }

            return listdetails;
        }

        #endregion Metodos Construtores dos Objetos das Integrações
    }
}