using Dominio.Entidades.Embarcador.BI;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.VLI
{
    public class IntegracaoVLI
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _cliente;

        private List<string> _listaChavesNotaCarregamento;
        private List<string> _listacodigosTerminais;
        #endregion

        #region Construtores

        public IntegracaoVLI(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _cliente = cliente;
        }

        #endregion

        #region Métodos Privados Para Autenticacao

        private void Autenticar()
        {
            if (TokenAcesso.TokenValido())
                return;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracao = ObterConfiguracao();

            List<(string Chave, string Valor)> headersAutenticacao = ObterHeadersAutenticacao(configuracao);


            RestRequest requisicao = CriaRequisicao(configuracao.UrlAutenticacao, "POST", "", headersAutenticacao, null);

            var client = new RestClient();
            IRestResponse responsenew = client.Execute(requisicao);

            Log.TratarErro($"URL: {configuracao.UrlAutenticacao}\nHeaders:{JsonConvert.SerializeObject(headersAutenticacao)}\n\nResponse:\n{responsenew.Content}", "INTEGRACAO_VLI");

            if (!IsRetornoIsRetornoSucessoSucesso(responsenew))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.ErrorResponse error = null;
                try
                {
                    error = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.ErrorResponse>(responsenew.Content);
                }
                catch (Exception ex) 
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de erro da API VLI: {ex.ToString()}", "CatchNoAction");
                }

                if (error == null)
                    throw new ServicoException("Ocorreu uma falha ao autenticar com a API da VLI.");
                else
                {
                    string erroTratado = error.result;
                    if (error.errors != null && error.errors.Count > 0)
                        erroTratado = string.Join(";", (from obj in error.errors select obj.message).ToList());
                    throw new ServicoException(error.status + " - " + erroTratado);
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RespostaAutenticacao respostaAutenticacao = ObterBodyConvertidoRespostaAutenticacao(responsenew.Content);

            TokenAcesso.SetCacheToken(respostaAutenticacao.TokenAcesso, respostaAutenticacao.TempoExpiracao);
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI ObterConfiguracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoVLI repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoVLI(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI integracao = repositorioIntegracao.Buscar();

            if (integracao == null || string.IsNullOrEmpty(integracao?.UrlIntegracaoRastreamento))
                throw new ServicoException("Não foram configurados os dados de integração com a VLI");

            return integracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RespostaAutenticacao ObterBodyConvertidoRespostaAutenticacao(string body)
        {
            return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RespostaAutenticacao>(body);
        }

        private List<(string Chave, string Valor)> ObterHeadersAutenticacao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracao)
        {

            string headerKey = "Authorization";
            string headerValue = $"Basic {Utilidades.String.Base64Encode($"{configuracao.IDAutenticacao}:{configuracao.SenhaAutenticacao}")}";

            var headers = new List<(string Chave, string Valor)>() {
                ValueTuple.Create(headerKey, headerValue)
            };

            return headers;
        }

        private List<(string Chave, string Valor)> ObterHeadersRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracao, bool carregamento = false, bool descarregamento = false, string codigoTerminal = "")
        {
            string dateformat = "dd/MM/yyyy";

            string headerKeyToken = "access_token";
            string headerValueToken = $"{TokenAcesso.ObterToken()}";

            string headerKeyClient = "client_id";
            string headerValueClient = $"{configuracao.IDAutenticacao}";

            string headerKeyEdiToken = "edi-token";
            string headerValueEdiToken = $"{configuracao.EdiToken}";

            var headers = new List<(string Chave, string Valor)>() {
                ValueTuple.Create(headerKeyToken, headerValueToken),
                ValueTuple.Create(headerKeyClient, headerValueClient),
                ValueTuple.Create(headerKeyEdiToken, headerValueEdiToken),
            };

            if (descarregamento && codigoTerminal != "")
            {
                string headerKeyDataToken = "dataDescarga";
                string headerValueDataToken = $"{DateTime.Now.ToString(dateformat)}";

                headers.Add(ValueTuple.Create(headerKeyDataToken, headerValueDataToken));

                string headerKeyTerminalToken = "terminal";
                string headerValueTerminalToken = $"{codigoTerminal}";

                headers.Add(ValueTuple.Create(headerKeyTerminalToken, headerValueTerminalToken));
            }

            return headers;
        }

        #endregion

        #region Métodos Privados Request

        private RestRequest CriaRequisicao(string url, string metodo, string body, List<(string Chave, string Valor)> headers = null, string contentType = "application/json")
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var requisicao = new RestRequest(url, metodo == "POST" ? Method.POST : Method.GET);
            requisicao.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            foreach (var (Chave, Valor) in (headers ?? new List<(string Chave, string Valor)>()))
                requisicao.AddHeader(Chave, Valor);

            requisicao.AddParameter("grant_type", "client_credentials");

            //WebRequest requisicao = WebRequest.Create(url);

            ////byte[] byteArrayDadosRequisicao = Encoding.ASCII.GetBytes(body);
            //byte[] byteArrayDadosRequisicao = Encoding.UTF8.GetBytes(body);

            //requisicao.Method = metodo;
            //requisicao.ContentType = contentType;

            //foreach (var (Chave, Valor) in (headers ?? new List<(string Chave, string Valor)>()))
            //    requisicao.Headers[Chave] = Valor;

            //if (!String.IsNullOrEmpty(body))
            //{
            //    requisicao.ContentLength = byteArrayDadosRequisicao.Length;

            //    System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream();
            //    streamDadosRequisicao.Write(byteArrayDadosRequisicao, 0, byteArrayDadosRequisicao.Length);
            //    streamDadosRequisicao.Close();
            //}

            return requisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoRastreio ObterRespostaEndPointRastreio()
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracao = ObterConfiguracao();

            List<(string Chave, string Valor)> headersIntegracao = ObterHeadersRequisicao(configuracao);

            RestRequest requisicaoRastreio = CriaRequisicao(configuracao.UrlIntegracaoRastreamento, "GET", "", headersIntegracao);

            RestClient client = new RestClient();

            RestResponse response = (RestResponse)client.Execute(requisicaoRastreio);

            if (!IsRetornoIsRetornoSucessoSucesso(response))
                throw new ServicoException($"Ocorreu uma falha ao integrar com a API de Rastreio. {response.Content.ToString()}");

            Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoRastreio respostaIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoRastreio>(response.Content);

            //Log.TratarErro($"REQUEST URL: {configuracao.UrlIntegracaoRastreamento }\nHeaders:{JsonConvert.SerializeObject(headersIntegracao)}\n", "INTEGRACAO_VLI_RASTREIO");

            return respostaIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoDescarregamentoFerroviario ObterRespostaEndPointDescarregamentoFerroviario(string codigoTerminal)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracao = ObterConfiguracao();

            List<(string Chave, string Valor)> headersIntegracao = ObterHeadersRequisicao(configuracao, false, true, codigoTerminal);

            RestRequest requisicaoDescarregamento = CriaRequisicao(configuracao.UrlIntegracaoDescarregamento, "GET", "", headersIntegracao);

            RestClient client = new RestClient();

            RestResponse response = (RestResponse)client.Execute(requisicaoDescarregamento);

            Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoDescarregamentoFerroviario respostaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoDescarregamentoFerroviario();

            if (!IsRetornoIsRetornoSucessoSucesso(response))
            {
                Log.TratarErro($"Ocorreu uma falha ao integrar com a API de Descarregamento Ferroviario {response.Content.ToString()}.");
                return respostaIntegracao;
            }

            respostaIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoDescarregamentoFerroviario>(response.Content);

            return respostaIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoDescarregamentoPortosVale ObterRespostaEndPointDescarregamentoPortosVale()
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracao = ObterConfiguracao();

            List<(string Chave, string Valor)> headersIntegracao = ObterHeadersRequisicao(configuracao);

            RestRequest requisicaoDescarregamento = CriaRequisicao(configuracao.UrlIntegracaoDescarregamentoPortosValeVLI, "GET", "", headersIntegracao);
            RestClient client = new RestClient();

            RestResponse response = (RestResponse)client.Execute(requisicaoDescarregamento);

            Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoDescarregamentoPortosVale respostaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoDescarregamentoPortosVale();
            if (!IsRetornoIsRetornoSucessoSucesso(response))
            {
                Log.TratarErro($"Ocorreu uma falha ao integrar com a API de Descarregamento Portos Vale {response.Content.ToString()}.");
                return respostaIntegracao;
            }

            respostaIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoDescarregamentoPortosVale>(response.Content);

            return respostaIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoCarregamento ObterRespostaEndPointCarregamento(string codigoTerminal)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracao = ObterConfiguracao();

            string dateformat = "yyyy-MM-dd";

            List<(string Chave, string Valor)> headersIntegracao = ObterHeadersRequisicao(configuracao, true, false, codigoTerminal);

            RestRequest requisicaoCarregamento = CriaRequisicao(configuracao.UrlIntegracaoCarregamento + $"?data={DateTime.Now.ToString(dateformat)}&terminal={codigoTerminal}", "GET", "", headersIntegracao);
            RestClient client = new RestClient();

            RestResponse response = (RestResponse)client.Execute(requisicaoCarregamento);

            if (!IsRetornoIsRetornoSucessoSucesso(response))
                throw new ServicoException("Ocorreu uma falha ao integrar com a API de Carregamento.");

            Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoCarregamento respostaIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoCarregamento>(response.Content);

            //Log.TratarErro($"REQUEST URL: {configuracao.UrlIntegracaoCarregamento }\nHeaders:{JsonConvert.SerializeObject(headersIntegracao)}\n", "INTEGRACAO_VLI_CARREGAMENTO");

            return respostaIntegracao;
        }

        private bool IsRetornoIsRetornoSucessoSucesso(IRestResponse retornoRequisicao)
        {
            if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                return true;

            if (retornoRequisicao.StatusCode == HttpStatusCode.Created)
                return true;

            return false;
        }

        private void ObterInformacoesCarregamentos()
        {
            preencherListaTerminais(true);
            _listaChavesNotaCarregamento = new List<string>();

            foreach (string codigoTerminal in _listacodigosTerminais)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoCarregamento respostaIntegracaoCarregamento = ObterRespostaEndPointCarregamento(codigoTerminal);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.ListaCarregamentoFerroviario carregamentoFerroviario in respostaIntegracaoCarregamento.Data.ListaCarregamentoFerroviario)
                {
                    Log.TratarErro($"Nº Notas Carregamento: {carregamentoFerroviario.ListaNFe.Count} Terminal: {codigoTerminal}\n", "INTEGRACAO_VLI_CARREGAMENTO");

                    if (carregamentoFerroviario.ListaNFe != null && carregamentoFerroviario.ListaNFe.Count > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.Nfe nota in carregamentoFerroviario.ListaNFe)
                        {
                            Log.TratarErro($"Chave Carregamento: {nota.ChaveNfe}\n", "INTEGRACAO_VLI_CARREGAMENTO");
                            _listaChavesNotaCarregamento.Add(nota.ChaveNfe);
                        }
                    }
                }
            }
        }

        private void ObterInformacoesRastreamento()
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNota = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI configuracao = ObterConfiguracao();

            //agora vamos buscar no rastreio para encontrar a nota e se estiver na lista carga entrega, iniciar viagem com a DataHoraPartidaTrem

            Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoRastreio respostaRastreio = ObterRespostaEndPointRastreio();

            if (respostaRastreio != null && respostaRastreio?.Data != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.ListaRastreio rastreio in respostaRastreio.Data.ListaRastreio)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listCargaEntregasRastreio = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
                    Log.TratarErro($"Nº Carregamentos: {rastreio.Carregamentos.Count}\n", "INTEGRACAO_VLI_RASTREIO");

                    if (rastreio.Carregamentos != null && rastreio.Carregamentos.Count > 0)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.Carregamento carregamento in rastreio.Carregamentos)
                        {
                            //vamos encontrar as entregas pelas notas fiscais;
                            foreach (var nota in carregamento.NotasFiscais)
                            {
                                if (_listaChavesNotaCarregamento.Contains(nota.ChaveNotaFiscal))
                                {
                                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFiscal.BuscarPorChave(nota.ChaveNotaFiscal);

                                    Log.TratarErro($"CHAVE NOTA FISCAL: {nota.ChaveNotaFiscal}\n", "INTEGRACAO_VLI_RASTREIO");

                                    if (xmlNotaFiscalExiste != null)
                                    {
                                        Log.TratarErro($"Encontrou xml: {nota.ChaveNotaFiscal}\n", "INTEGRACAO_VLI_RASTREIO");

                                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXml = repPedidoXmlNota.BuscarPorXMLNotaFiscal(xmlNotaFiscalExiste.Codigo);

                                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXml in pedidosXml)
                                        {
                                            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = repCargaEntregaNota.BuscarPorPedidoXMLNotaFiscal(pedidoXml.Codigo);

                                            if (cargaEntregaNotaFiscal != null)
                                                listCargaEntregasRastreio.Add(cargaEntregaNotaFiscal.CargaEntrega);
                                        }
                                    }
                                }
                                else
                                    Log.TratarErro($"Chave nota não esta em carregamento: {nota.ChaveNotaFiscal}\n", "INTEGRACAO_VLI_RASTREIO");
                            }
                        }

                        Log.TratarErro($"Entregas Encontradas: {listCargaEntregasRastreio.Count}\n", "INTEGRACAO_VLI_RASTREIO");

                        if (listCargaEntregasRastreio != null && listCargaEntregasRastreio.Count > 0)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.Carga> listCargas = listCargaEntregasRastreio.Select(x => x.Carga).Distinct().ToList();

                            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in listCargas)
                            {
                                if (!carga.DataInicioViagem.HasValue && rastreio.DataHoraPartidaTrem.HasValue && rastreio.DataHoraPartidaTrem.Value != DateTime.MinValue)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                                    auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                                    auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.IntegracaoCargaMultiEmbarcador;

                                    DateTime dataInicioViagem = rastreio.DataHoraPartidaTrem.Value;

                                    if (dataInicioViagem < carga.DataInicioViagemPrevista)
                                        dataInicioViagem = DateTime.Now;

                                    if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(carga.Codigo, dataInicioViagem, OrigemSituacaoEntrega.WebService, null, configuracaoTMS, _tipoServicoMultisoftware, _cliente, auditado, _unitOfWork))
                                        Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, "Informou o início da viagem em integracao ferroviaria VLI", _unitOfWork);
                                }
                            }
                            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

                            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entrega in listCargaEntregasRastreio)
                            {
                                if (rastreio.DataHoraPrevisaoChegadaTrem.HasValue)
                                    entrega.DataPrevista = rastreio.DataHoraPrevisaoChegadaTrem.Value;

                                if (!entrega.DataEntradaRaio.HasValue && rastreio.DataHoraChegadaTrem.HasValue && rastreio.DataHoraChegadaTrem.Value != DateTime.MinValue)
                                    entrega.DataEntradaRaio = rastreio.DataHoraChegadaTrem;

                                if (entrega.DataConfirmacao.HasValue)
                                    entrega.DataSaidaRaio = entrega.DataConfirmacao;

                                if (!entrega.LatitudeFinalizada.HasValue && !entrega.LongitudeFinalizada.HasValue)
                                {
                                    entrega.LatitudeFinalizada = rastreio.Latitude.ToDecimal();
                                    entrega.LongitudeFinalizada = rastreio.Longitude.ToDecimal();

                                    repCargaEntrega.Atualizar(entrega);
                                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(entrega, repCargaEntrega, _unitOfWork, configControleEntrega);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Log.TratarErro($"Resposta Rastreio: {(respostaRastreio != null && respostaRastreio?.Errors?.Count() > 0 ? respostaRastreio.Errors[0].message : "")}\n", "INTEGRACAO_VLI_RASTREIO");
            }
        }

        private void ObterInformacoesDescarregamentoFerroviario()
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNota = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = null;

            preencherListaTerminais(false);

            foreach (string codigoTerminal in _listacodigosTerminais)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoDescarregamentoFerroviario respostaDescarregamentoFerroviario = ObterRespostaEndPointDescarregamentoFerroviario(codigoTerminal);
                Log.TratarErro($"Resposta Descarregamento Terminal: {codigoTerminal} Total: {respostaDescarregamentoFerroviario?.Data?.Count()} \n", "INTEGRACAO_VLI_DESCARREGAMENTO_FERROVIARIO");

                if (respostaDescarregamentoFerroviario != null && respostaDescarregamentoFerroviario?.Data?.Count() > 0)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.Datum data in respostaDescarregamentoFerroviario.Data)
                    {
                        Log.TratarErro($"Nº Notas : {data.listaNfes.Count} Terminal: {codigoTerminal}\n", "INTEGRACAO_VLI_DESCARREGAMENTO_FERROVIARIO");
                        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listCargaEntregasDescarregamento = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

                        foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.NFeDescarregamentoFerroviario notaDescarregamento in data.listaNfes)
                        {
                            Log.TratarErro($"CHAVE NOTA: {notaDescarregamento.chave}\n", "INTEGRACAO_VLI_DESCARREGAMENTO_FERROVIARIO");

                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFiscal.BuscarPorChave(notaDescarregamento.chave);

                            if (xmlNotaFiscalExiste != null)
                            {
                                Log.TratarErro($"ENCONTROU CHAVE NOTA: {notaDescarregamento.chave}\n", "INTEGRACAO_VLI_DESCARREGAMENTO_FERROVIARIO");
                                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXml = repPedidoXmlNota.BuscarPorXMLNotaFiscal(xmlNotaFiscalExiste.Codigo);

                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXml in pedidosXml)
                                {
                                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = repCargaEntregaNota.BuscarPorPedidoXMLNotaFiscal(pedidoXml.Codigo);

                                    if (cargaEntregaNotaFiscal != null)
                                        listCargaEntregasDescarregamento.Add(cargaEntregaNotaFiscal.CargaEntrega);
                                }
                            }
                        }

                        if (listCargaEntregasDescarregamento.Count > 0)
                        {
                            foreach (var cargaEntregaDescarregamento in listCargaEntregasDescarregamento)
                            {
                                if (SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(cargaEntregaDescarregamento.Situacao))
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                                    auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                                    auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.IntegracaoCargaMultiEmbarcador;

                                    if (respostaDescarregamentoFerroviario.Data[0].dataDescarga.HasValue)
                                    {
                                        //if (rastreio.DataHoraInicioCarregamento.HasValue && rastreio.DataHoraFimCarregamento.Value != DateTime.MinValue)
                                        //    entrega.DataInicio = rastreio.DataHoraFimCarregamento;

                                        cargaEntregaDescarregamento.DataConfirmacao = data.dataDescarga;
                                        cargaEntregaDescarregamento.DataInicio = data.dataDescarga;

                                        if (!cargaEntregaDescarregamento.DataSaidaRaio.HasValue)
                                            cargaEntregaDescarregamento.DataSaidaRaio = data.dataDescarga;

                                        tipoOperacaoParametro ??= new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork).BuscarPorCodigoFetch(cargaEntregaDescarregamento.Carga.TipoOperacao?.Codigo ?? 0);
                                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(cargaEntregaDescarregamento, cargaEntregaDescarregamento.DataConfirmacao.Value, null, null, 0, "", configuracaoTMS, _tipoServicoMultisoftware, auditado, OrigemSituacaoEntrega.WebService, _cliente, _unitOfWork, false, configuracaoControleEntrega, tipoOperacaoParametro);
                                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaDescarregamento, null, "Finalizou entrega por integracao ferroviaria VLI Descarregamento Ferroviario", _unitOfWork);
                                    }
                                }
                            }
                        }
                    }

                }
                else if (respostaDescarregamentoFerroviario != null && respostaDescarregamentoFerroviario?.Errors?.Count() > 0)
                    Log.TratarErro($"Resposta Descarregamento Ferroviario: {respostaDescarregamentoFerroviario.Errors[0].message}\n", "INTEGRACAO_VLI_DESCARREGAMENTO_FERROVIARIO");
            }
        }

        private void ObterInformacoesDescarregamentoPortosVale()
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNota = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNota = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = null;

            Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.RetornoIntegracaoDescarregamentoPortosVale respostaDescarregamentoPortosVale = ObterRespostaEndPointDescarregamentoPortosVale();

            if (respostaDescarregamentoPortosVale != null && respostaDescarregamentoPortosVale?.Data.Count() > 0)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.DataDescarregamentoPortosVale data in respostaDescarregamentoPortosVale?.Data)
                {
                    Log.TratarErro($"Nº Notas Descarregamento portos vale : {data.ListaNfes.Count}\n", "INTEGRACAO_VLI_DESCARREGAMENTO_PORTOS_VALE");
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listCargaEntregasDescarregamentoPortosVale = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

                    foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VLI.NfeDescarregamentoPortosVale notaDescarregamento in data.ListaNfes)
                    {
                        Log.TratarErro($"CHAVE NOTA: {notaDescarregamento.ChaveNfe}\n", "INTEGRACAO_VLI_DESCARREGAMENTO_PORTOS_VALE");

                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscalExiste = repXMLNotaFiscal.BuscarPorChave(notaDescarregamento.ChaveNfe);

                        if (xmlNotaFiscalExiste != null)
                        {
                            Log.TratarErro($"ENCONTROU CHAVE NOTA: {notaDescarregamento.ChaveNfe}\n", "INTEGRACAO_VLI_DESCARREGAMENTO_PORTOS_VALE");

                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXml = repPedidoXmlNota.BuscarPorXMLNotaFiscal(xmlNotaFiscalExiste.Codigo);

                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXml in pedidosXml)
                            {
                                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = repCargaEntregaNota.BuscarPorPedidoXMLNotaFiscal(pedidoXml.Codigo);

                                if (cargaEntregaNotaFiscal != null)
                                    listCargaEntregasDescarregamentoPortosVale.Add(cargaEntregaNotaFiscal.CargaEntrega);
                            }
                        }
                    }

                    if (listCargaEntregasDescarregamentoPortosVale.Count > 0)
                    {
                        foreach (var cargaEntregaDescarregamento in listCargaEntregasDescarregamentoPortosVale)
                        {
                            if (SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(cargaEntregaDescarregamento.Situacao))
                            {
                                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.IntegracaoCargaMultiEmbarcador;

                                if (data.DescargaTermino.HasValue)
                                {
                                    //if (rastreio.DataHoraInicioCarregamento.HasValue && rastreio.DataHoraFimCarregamento.Value != DateTime.MinValue)
                                    //    entrega.DataInicio = rastreio.DataHoraFimCarregamento;

                                    cargaEntregaDescarregamento.DataConfirmacao = data.DescargaTermino;
                                    cargaEntregaDescarregamento.DataInicio = data.DescargaTermino;

                                    tipoOperacaoParametro ??= new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork).BuscarPorCodigoFetch(cargaEntregaDescarregamento.Carga.TipoOperacao?.Codigo ?? 0);
                                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(cargaEntregaDescarregamento, cargaEntregaDescarregamento.DataConfirmacao.Value, null, null, 0, "", configuracaoTMS, _tipoServicoMultisoftware, auditado, OrigemSituacaoEntrega.WebService, _cliente, _unitOfWork, false, configuracaoControleEntrega, tipoOperacaoParametro);
                                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaDescarregamento, null, "Finalizou entrega por integracao ferroviaria VLI Descarregamento Portos vale", _unitOfWork);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Log.TratarErro($"Resposta Descarregamento portos vale: {(respostaDescarregamentoPortosVale != null && respostaDescarregamentoPortosVale?.Errors?.Count() > 0 ? respostaDescarregamentoPortosVale.Errors[0].message : "Data: " + respostaDescarregamentoPortosVale.Data.Count().ToString())}\n", "INTEGRACAO_VLI_DESCARREGAMENTO_PORTOS_VALE");
            }
        }


        private void preencherListaTerminais(bool carregamento)
        {
            _listacodigosTerminais = new List<string>();

            if (!carregamento)
            {
                //Descarga Ferroviária(Terminal de Descarga)

                //EBM - BARRA MANSA(BARRA MANSA)
                //ECB - CAMPO BELO(CAMPO BELO)
                //ECE - CAPITÃO EDUARDO FCA(SANTA LUZIA)
                //EEL - ELDORADO(CONTAGEM)
                //EGS - GARCAS DE MINAS(ARCOS)
                //EKK - CALCARIO(VOLTA REDONDA)
                //ENG - NOVA GRANJA(SAO JOSE DA LAPA)
                //EPI - PARQUE INDUSTRIAL(CONTAGEM)
                //EPM - PRUDENTE DE MORAIS(PRUDENTE DE MORAIS)

                _listacodigosTerminais.Add("EBM");
                _listacodigosTerminais.Add("ECB");
                _listacodigosTerminais.Add("ECE");
                _listacodigosTerminais.Add("EEL");
                _listacodigosTerminais.Add("EGS");
                _listacodigosTerminais.Add("EKK");
                _listacodigosTerminais.Add("ENG");
                _listacodigosTerminais.Add("EPI");
                _listacodigosTerminais.Add("EPM");
                _listacodigosTerminais.Add("VGV");
            }
            else
            {
                //Carregamento(Terminal de Transbordo - Origem)

                //VJM - JOAO MONLEVADE
                //ENG - NOVA GRANJA
                //EPI - PARQUE INDUSTRIAL
                //VAZ - ARACRUZ
                //VTU - TUBARAO
                //EEL - ELDORADO
                //EBI - BATISTA ALMEIDA
                //ECE - CAPITÃO EDUARDO FCA
                //VPN - VITORIA PEDRO NOLASCO
                //EMS - MATOSINHOS
                _listacodigosTerminais.Add("VJM");
                _listacodigosTerminais.Add("ENG");
                _listacodigosTerminais.Add("EPI");
                _listacodigosTerminais.Add("VAZ");
                _listacodigosTerminais.Add("VTU");
                _listacodigosTerminais.Add("EEL");
                _listacodigosTerminais.Add("EBI");
                _listacodigosTerminais.Add("ECE");
                _listacodigosTerminais.Add("VPN");
                _listacodigosTerminais.Add("EMS");
                _listacodigosTerminais.Add("VGV");
            }

        }

        #endregion Métodos Privados Request


        #region Metotodos Publicos

        public void IntegrarEventosRastreamentoVLI()
        {


            try
            {
                Autenticar();

                ObterInformacoesRastreamento();

            }
            catch (ServicoException ex)
            {
                Log.TratarErro(ex);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
            }
        }


        public void IntegrarEventosDescarregamentoVLI()
        {

            try
            {
                Autenticar();

                ObterInformacoesDescarregamentoFerroviario();
                ObterInformacoesDescarregamentoPortosVale();

            }
            catch (ServicoException ex)
            {
                Log.TratarErro(ex);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
            }
        }


        #endregion

        #region Carregamentos

        public void IntegrarEventosCarregamentoVLI()
        {

            try
            {
                Autenticar();

                ObterInformacoesCarregamentos();

            }
            catch (ServicoException ex)
            {
                Log.TratarErro(ex);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
            }
        }

        #endregion
    }
}

