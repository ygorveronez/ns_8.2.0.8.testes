using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Frimesa
{
    public sealed class IntegracaoFrimesa
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoFrimesa(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarFrete(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa repIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa configuracaoIntegracao = repIntegracao.Buscar();

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento corpoRequisicao = PreencherCorpoRequisicaoFrete(integracaoPendente);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLContabilizacao, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created)
                {
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoPendente.ProblemaIntegracao = (string)retornoIntegracao.msgRetorno;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace((string)retornoIntegracao.msgRetorno))
                    throw new ServicoException((string)retornoIntegracao.msgRetorno);
                else if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado.");
                else
                    throw new ServicoException("Retorno de status não tratado, verificar a comunicação com a Frimesa!");
            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoFrimesa");

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar com Frimesa.";
            }

            servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaCargaIntegracao.Atualizar(integracaoPendente);
        }

        public void IntegrarValePedagio(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa repIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa configuracaoIntegracao = repIntegracao.Buscar();

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento corpoRequisicao = PreencherCorpoRequisicaoValePedagio(integracaoPendente);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLContabilizacao, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created)
                {
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoPendente.ProblemaIntegracao = (string)retornoIntegracao.msgRetorno;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace((string)retornoIntegracao.msgRetorno))
                    throw new ServicoException((string)retornoIntegracao.msgRetorno);
                else if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado.");
                else
                    throw new ServicoException("Retorno de status não tratado, verificar a comunicação com a Frimesa!");
            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoFrimesa");

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar com Frimesa.";
            }

            servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaCargaIntegracao.Atualizar(integracaoPendente);
        }

        public void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracaoPendente)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);

            ocorrenciaCTeIntegracaoPendente.NumeroTentativas++;
            ocorrenciaCTeIntegracaoPendente.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa repIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa configuracaoIntegracao = repIntegracao.Buscar();

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento corpoRequisicao = PreencherCorpoRequisicaoOcorrencia(ocorrenciaCTeIntegracaoPendente);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLContabilizacao, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created)
                {
                    ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = (string)retornoIntegracao.msgRetorno;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace((string)retornoIntegracao.msgRetorno))
                    throw new ServicoException((string)retornoIntegracao.msgRetorno);
                else if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado.");
                else
                    throw new ServicoException("Retorno de status não tratado, verificar a comunicação com a Frimesa!");
            }
            catch (ServicoException excecao)
            {
                ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoFrimesa");

                ocorrenciaCTeIntegracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar com Frimesa.";
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracaoPendente, jsonRequisicao, jsonRetorno, "json");

            repositorioOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracaoPendente);
        }

        public bool PermiteFinalizarIntegracaoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente)
        {
            //Quando e se migrar a integração para a IntegracaoEnvioProgramado, tirar esse médodo daqui e colocar no serviço de integração
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTes = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            DateTime dataUltimaAutorizacaoCTe = repositorioCargaCTes.BuscarUltimaDataAutorizacaoPorCarga(integracaoPendente.Carga.Codigo, "A");

            if (dataUltimaAutorizacaoCTe == DateTime.MinValue)
            {
                //Se não tem CTE autorizada, não tem como finalizar a integração, vamos tirar da fila pra não travar

                integracaoPendente.NumeroTentativas = 2;
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Não foi possível integrar o frete, pois não foi encontrada nenhum CTe autorizado na Carga.";

                repositorioCargaCargaIntegracao.Atualizar(integracaoPendente);

                return false;
            }

            if (dataUltimaAutorizacaoCTe.AddHours(24) >= DateTime.Now)
            {
                if (dataUltimaAutorizacaoCTe != DateTime.MinValue)
                {
                    integracaoPendente.DataIntegracao = dataUltimaAutorizacaoCTe;
                    repositorioCargaCargaIntegracao.Atualizar(integracaoPendente);
                }
                return false;
            }

            return true;
        }

        public bool PermiteFinalizarIntegracaoValePedagio(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente)
        {
            //Quando e se migrar a integração para o EnvioProgramado, tirar esse médodo daqui e colocar no serviço de integração
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> cargaIntegracaoValesPedagio = repositorioCargaIntegracaoValePedagio.BuscarPorCarga(integracaoPendente.Carga.Codigo);

            if (cargaIntegracaoValesPedagio == null || cargaIntegracaoValesPedagio.Count <= 0)
            {
                //Se não tem Vale Pedágio, não tem como finalizar a integração, vamos tirar da fila pra não travar
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

                integracaoPendente.NumeroTentativas = 2;
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Não foi possível integrar o Vale Pedágio, pois não foi encontrada nenhum Vale Pedágio na Carga.";

                repositorioCargaCargaIntegracao.Atualizar(integracaoPendente);

                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = cargaIntegracaoValesPedagio.FirstOrDefault();

            if (cargaIntegracaoValePedagio.DataIntegracao == DateTime.MinValue || cargaIntegracaoValePedagio.DataIntegracao.AddHours(2) >= DateTime.Now)
            {
                return false;
            }

            return true;
        }

        public void IntegrarNFSManual(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao integracao)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repositorioNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo>(_unitOfWork);

            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa repIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoFrimesa(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa configuracaoIntegracao = repIntegracao.Buscar();

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento corpoRequisicao = PreencherCorpoRequisicaoNFManual(integracao);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLContabilizacao, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created)
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracao.ProblemaIntegracao = (string)retornoIntegracao.msgRetorno;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace((string)retornoIntegracao.msgRetorno))
                    throw new ServicoException((string)retornoIntegracao.msgRetorno);
                else if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado.");
                else
                    throw new ServicoException("Retorno de status não tratado, verificar a comunicação com a Frimesa!");
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoFrimesa");

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Problema ao tentar integrar com Frimesa.";
            }

            servicoArquivoTransacao.Adicionar(integracao, jsonRequisicao, jsonRetorno, "json");

            repositorioNFSManualCTeIntegracao.Atualizar(integracao);

        }
        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoFrimesa));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.URLContabilizacao);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (configuracaoIntegracao.TipoIntegracaoOAuth == TipoIntegracaoOAuth.OAuth2_0 && configuracaoIntegracao.Situacao)
            {
                string token = ObterToken(configuracaoIntegracao);
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
                requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

            return requisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento PreencherCorpoRequisicaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrencia)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe recargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento corpoRequisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento
            {
                MSPagamentoID = ocorrencia.CargaOcorrencia?.NumeroOcorrencia ?? 0,
                NumeroCarga = ocorrencia.CargaOcorrencia?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                OrganizationCode = ocorrencia.CargaOcorrencia?.Carga.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                CNPJTransportador = ocorrencia.CargaCTe.Carga?.Empresa?.CNPJ_SemFormato ?? string.Empty,
                PlacaVeiculo = ocorrencia.CargaOcorrencia?.Carga?.Veiculo?.Placa ?? string.Empty,
                PlacaReboque = ocorrencia.CargaOcorrencia?.Carga?.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty,
                DataServico = ocorrencia.CargaOcorrencia.DataOcorrencia.Date.ToString("yyyy-MM-dd"),
                CNPJOrigem = ocorrencia.CargaCTe.CTe?.Expedidor?.Cliente?.CPF_CNPJ_SemFormato ?? ocorrencia.CargaCTe.CTe?.TomadorPagador?.Cliente?.CPF_CNPJ_SemFormato ?? string.Empty,
                TipoOperacao = "FRETE",
                TipoCusto = "ACESSORIO",
                CustoAcessorio = ocorrencia.CargaOcorrencia?.TipoOcorrencia?.Descricao ?? string.Empty,
                Moeda = "BRL",
                TipoTaxa = "",
                DataTaxa = "",
                Taxa = 0
            };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfos = repCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrencia.CargaOcorrencia.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotaFiscalCTEOcorrencias = recargaPedidoXMLNotaFiscalCTe.BuscarPorOcorrencia(ocorrencia.CargaOcorrencia.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas> pagamentoLinhas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargaCTeComplementoInfos)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTeComplementoInfo.CTe;
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotasFiscaisCTes = (from obj in cargaPedidoXMLNotaFiscalCTEOcorrencias where obj.CargaCTe.CargaCTeComplementoInfo.Codigo == cargaCTeComplementoInfo.Codigo select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTe in cargaPedidoXMLNotasFiscaisCTes)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal;
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal;

                    Dominio.Entidades.Cliente recebedor = xmlNotaFiscal.Recebedor ?? pedidoXMLNotaFiscal.CargaPedido.Recebedor;

                    pagamentoLinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas()
                    {
                        NumeroPedido = pedidoXMLNotaFiscal.CargaPedido.Pedido.NumeroOrdem.ToInt(),
                        NumeroNF = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero,
                        CNPJDestino = recebedor?.CPF_CNPJ_SemFormato ?? pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                        TipoDocumento = cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? "CTE" : "NOTA",
                        NumeroDocumento = cte.Numero,
                        ChaveDocumento = cte.ChaveAcesso,
                        ProtocoloDocumento = cte.Codigo.ToString(),
                        ValorFrete = cargaPedidoXMLNotaFiscalCTe.ValorComplemento
                    });
                }
            }

            corpoRequisicao.PagamentoLinhas = pagamentoLinhas.ToArray();

            return corpoRequisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento PreencherCorpoRequisicaoValePedagio(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repValePegadio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio valePedagio = repValePegadio.BuscarPorUnicaCarga(cargaIntegracao.Carga.Codigo);
            Dominio.Entidades.Cliente expedidor = cargaIntegracao.Carga.Pedidos?.Select(obj => obj.Expedidor).FirstOrDefault(obj => obj != null);
            List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> cargaValePedagio = repCargaValePedagio.BuscarPorCarga(valePedagio.Carga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento corpoRequisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento()
            {
                MSPagamentoID = valePedagio.NumeroValePedagio.ToInt(),
                NumeroCarga = cargaIntegracao.Carga.CodigoCargaEmbarcador ?? string.Empty,
                OrganizationCode = cargaIntegracao.Carga.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                CNPJTransportador = string.Join(",", cargaValePedagio?.Select(o => o.Fornecedor?.CPF_CNPJ_SemFormato)) ?? string.Empty,
                PlacaVeiculo = cargaIntegracao.Carga.Veiculo?.Placa ?? string.Empty,
                PlacaReboque = cargaIntegracao.Carga.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty,
                DataServico = cargaIntegracao.DataIntegracao.Date.ToString("yyyy-MM-dd"),
                CNPJOrigem = expedidor?.CPF_CNPJ_SemFormato ?? cargaIntegracao.Carga.Filial?.CNPJ_SemFormato ?? string.Empty,
                TipoOperacao = "PEDAGIO",
                TipoCusto = "BASE",
                CustoAcessorio = string.Empty,
                Moeda = "BRL",
                TipoTaxa = string.Empty,
                DataTaxa = string.Empty,
                Taxa = 0
            };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(cargaIntegracao.Carga.Codigo, true);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notas = repNota.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas> pagamentoLinhas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas>();

            decimal valorFreteTotal = (from obj in notas select obj.ValorTotalAReceberComICMSeISS).Sum();
            decimal valorTotalRateado = 0;
            Dominio.Entidades.Embarcador.Cargas.CargaCTe utitmoCargaCTe = cargaCTes.LastOrDefault();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargacte in cargaCTes)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal ultimoXMLNotaFiscal = (from obj in cargacte.CTe.XMLNotaFiscais select obj).LastOrDefault();
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in cargacte.CTe.XMLNotaFiscais.ToList())
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal notaPedidoNotaFiscal = (from obj in notas where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo select obj).FirstOrDefault();
                    if (notaPedidoNotaFiscal == null)
                        continue;

                    decimal valorPedagio = Math.Round((valePedagio.ValorValePedagio / valorFreteTotal) * notaPedidoNotaFiscal.ValorTotalAReceberComICMSeISS, 2);
                    valorTotalRateado += valorPedagio;

                    if (ultimoXMLNotaFiscal == xmlNotaFiscal && utitmoCargaCTe == cargacte)
                        valorPedagio += valePedagio.ValorValePedagio - valorTotalRateado;

                    Dominio.Entidades.Cliente recebedor = xmlNotaFiscal.Recebedor ?? notaPedidoNotaFiscal.CargaPedido.Recebedor;

                    pagamentoLinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas()
                    {
                        NumeroPedido = notaPedidoNotaFiscal.CargaPedido.Pedido.NumeroOrdem.ToInt(),
                        NumeroNF = xmlNotaFiscal.Numero,
                        CNPJDestino = recebedor?.CPF_CNPJ_SemFormato ?? xmlNotaFiscal.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                        TipoDocumento = (cargacte.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe) ? "CTE" : "NOTA",
                        NumeroDocumento = cargacte.CTe.Numero,
                        ChaveDocumento = cargacte.CTe.ChaveAcesso,
                        ProtocoloDocumento = cargacte.CTe.Codigo.ToString() ?? string.Empty,
                        ValorFrete = valorPedagio
                    });
                }
            }

            corpoRequisicao.PagamentoLinhas = pagamentoLinhas.ToArray();

            return corpoRequisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento PreencherCorpoRequisicaoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Dominio.Entidades.Cliente expedidor = cargaIntegracao.Carga.Pedidos?.Select(obj => obj.Expedidor).FirstOrDefault(obj => obj != null);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento corpoRequisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento()
            {
                MSPagamentoID = cargaIntegracao.Carga.Codigo,
                NumeroCarga = cargaIntegracao.Carga.CodigoCargaEmbarcador,
                OrganizationCode = cargaIntegracao.Carga.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                CNPJTransportador = cargaIntegracao.Carga.Empresa?.CNPJ_SemFormato ?? string.Empty,
                PlacaVeiculo = cargaIntegracao.Carga.Veiculo?.Placa ?? string.Empty,
                PlacaReboque = cargaIntegracao.Carga.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty,
                DataServico = cargaIntegracao.DataIntegracao.Date.ToString("yyyy-MM-dd"),
                CNPJOrigem = expedidor?.CPF_CNPJ_SemFormato ?? cargaIntegracao.Carga.Filial?.CNPJ_SemFormato ?? string.Empty,
                TipoOperacao = cargaIntegracao.Carga.TipoOperacao?.RetornoVazio == true ? "RETORNO" : "FRETE",
                TipoCusto = "BASE",
                CustoAcessorio = string.Empty,
                Moeda = "BRL",
                TipoTaxa = string.Empty,
                DataTaxa = string.Empty,
                Taxa = 0
            };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(cargaIntegracao.Carga.Codigo, true);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notas = repNota.BuscarPorCarga(cargaIntegracao.Carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas> pagamentoLinhas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargacte in cargaCTes)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in cargacte.CTe.XMLNotaFiscais.ToList())
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal notaPedidoNotaFiscal = (from obj in notas where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo select obj).FirstOrDefault();
                    if (notaPedidoNotaFiscal == null)
                        continue;

                    Dominio.Entidades.Cliente recebedor = xmlNotaFiscal.Recebedor ?? notaPedidoNotaFiscal.CargaPedido.Recebedor;

                    pagamentoLinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas()
                    {
                        NumeroPedido = notaPedidoNotaFiscal.CargaPedido.Pedido.NumeroOrdem.ToInt(),
                        NumeroNF = xmlNotaFiscal.Numero,
                        CNPJDestino = recebedor?.CPF_CNPJ_SemFormato ?? xmlNotaFiscal.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                        TipoDocumento = (cargacte.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe) ? "CTE" : "NOTA",
                        NumeroDocumento = cargacte.CTe.Numero,
                        ChaveDocumento = cargacte.CTe.ChaveAcesso,
                        ProtocoloDocumento = cargacte.CTe.Codigo.ToString() ?? string.Empty,
                        ValorFrete = notaPedidoNotaFiscal.ValorTotalAReceberComICMSeISS
                    });
                }
            }

            corpoRequisicao.PagamentoLinhas = pagamentoLinhas.ToArray();
            return corpoRequisicao;
        }

        private string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient(configuracaoIntegracao.AccessToken);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", configuracaoIntegracao.ClientID);
            request.AddParameter("client_secret", configuracaoIntegracao.ClientSecret);
            request.AddParameter("scope", configuracaoIntegracao.Scope);

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);

            return retorno.access_token;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento PreencherCorpoRequisicaoNFManual(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao integracao)
        {
            Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = integracao.LancamentoNFSManual;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCodigoCTe(lancamentoNFSManual.CTe?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCTes.FirstOrDefault()?.Carga;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento corpoRequisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.RequisicaoEnviaPagamento
            {
                MSPagamentoID = lancamentoNFSManual.Codigo,
                NumeroCarga = carga?.CodigoCargaEmbarcador ?? string.Empty,
                OrganizationCode = carga?.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                CNPJTransportador = carga?.Empresa?.CNPJ_SemFormato ?? string.Empty,
                PlacaVeiculo = carga?.Veiculo?.Placa ?? string.Empty,
                PlacaReboque = carga?.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty,
                DataServico = lancamentoNFSManual.DataCriacao.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd"),
                CNPJOrigem = cargaCTes?.FirstOrDefault()?.CTe?.Expedidor?.Cliente?.CPF_CNPJ_SemFormato ?? cargaCTes?.FirstOrDefault()?.CTe?.TomadorPagador?.Cliente?.CPF_CNPJ_SemFormato ?? string.Empty,
                TipoOperacao = "FRETE",
                TipoCusto = "ACESSORIO",
                CustoAcessorio = lancamentoNFSManual.Descricao ?? string.Empty,
                Moeda = "BRL",
                TipoTaxa = "",
                DataTaxa = "",
                Taxa = 0
            };

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas> pagamentoLinhas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas>();

            foreach (var cargaDocumentoParaEmissaoNFSManual in lancamentoNFSManual.Documentos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = cargaDocumentoParaEmissaoNFSManual.PedidoXMLNotaFiscal;

                if (pedidoXMLNotaFiscal == null)
                    throw new ServicoException("Há documentos originários da NFS que não são notas fiscais.");

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal;
                Dominio.Entidades.Cliente recebedor = xmlNotaFiscal?.Recebedor ?? pedidoXMLNotaFiscal.CargaPedido.Recebedor;

                pagamentoLinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Frimesa.PagamentoLinhas()
                {
                    NumeroPedido = pedidoXMLNotaFiscal.CargaPedido?.Pedido?.NumeroOrdem?.ToInt() ?? 0,
                    NumeroNF = xmlNotaFiscal?.Numero ?? 0,
                    CNPJDestino = recebedor?.CPF_CNPJ_SemFormato ?? xmlNotaFiscal?.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                    TipoDocumento = "NOTA",
                    NumeroDocumento = xmlNotaFiscal?.Numero ?? 0,
                    ChaveDocumento = xmlNotaFiscal?.Chave ?? string.Empty,
                    ProtocoloDocumento = cargaDocumentoParaEmissaoNFSManual.Codigo.ToString(),
                    ValorFrete = cargaDocumentoParaEmissaoNFSManual.ValorFrete
                });
            }

            corpoRequisicao.PagamentoLinhas = pagamentoLinhas.ToArray();

            return corpoRequisicao;
        }

        #endregion Métodos Privados
    }
}