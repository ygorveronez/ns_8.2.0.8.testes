using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.Correios
{
    public class IntegracaoCorreios
    {
        #region Propriedades Privadas

        private string URL;
        private string Usuario;
        private string Senha;

        protected Servicos.Correios.AtendeClienteClient client;
        protected Servicos.Models.Integracao.InspectorBehavior inspector;

        #endregion

        #region Singleton

        private static IntegracaoCorreios instance;

        public static IntegracaoCorreios GetInstance(Repositorio.UnitOfWork unitOfWork)
        {
            if (instance == null) instance = new IntegracaoCorreios(unitOfWork);
            return instance;
        }

        private IntegracaoCorreios(Repositorio.UnitOfWork unitOfWork)
        {
            CarregarConfiguracoes(unitOfWork);
        }

        public IntegracaoCorreios() { }

        #endregion

        public static void GerarIntegracaoPendente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool transportadoraConfigurada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Correios);
            if (tipoIntegracao != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao()
                {
                    Pedido = pedido,
                    TipoIntegracao = tipoIntegracao,
                    SituacaoIntegracao = transportadoraConfigurada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado,
                    Tentativas = 0,
                    ProblemaIntegracao = transportadoraConfigurada ? string.Empty : "Integração não relizada pois transportadora não está configurada para integrar com os Correios.",
                    DataEnvio = DateTime.Now
                };

                Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(unitOfWork);
                repPedidoIntegracao.Inserir(pedidoIntegracao);
            }
        }

        public static void CarregarPrecoPostagem(ref Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            pedidoIntegracao.Tentativas += 1;
            pedidoIntegracao.DataEnvio = DateTime.Now;

            string mensagem = string.Empty;
            string mensagemException = string.Empty;
            string xmlEnvio = string.Empty;
            string xmlRetorno = string.Empty;

            long idPLPMaster = 0;
            string etiquetasCorreios = string.Empty;
            try
            {
                idPLPMaster = long.Parse(pedidoIntegracao.Pedido.PLPCorreios);
                etiquetasCorreios = pedidoIntegracao.Pedido.NumeroEtiquetaCorreios;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao parsear dados PLP/etiquetas dos Correios: {ex.ToString()}", "CatchNoAction");
            }

            if (idPLPMaster > 0)
            {
                IntegracaoCorreios serIntegracaoCorreios = IntegracaoCorreios.GetInstance(unitOfWork);
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ReturnSolicitaXmlPlp result = null;
                    bool integrouSucesso = false;
                    try
                    {
                        result = serIntegracaoCorreios.SolicitarXMLPLP(idPLPMaster, unitOfWork);
                        integrouSucesso = true;
                    }
                    catch (Exception e)
                    {
                        xmlEnvio = serIntegracaoCorreios.inspector?.LastRequestXML;
                        xmlRetorno = serIntegracaoCorreios.inspector?.LastResponseXML;

                        Servicos.Log.TratarErro(e, "IntegracaoCorreios");
                        Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoCorreios");
                        Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoCorreios");

                        if (string.IsNullOrWhiteSpace(xmlRetorno))
                        {
                            mensagem = "Ocorreu uma falha ao comunicar com o Serviço dos Correios.";
                            mensagemException = e.InnerException?.Message ?? e.Message;
                        }
                        else
                            mensagem = "Correios: " + (e.InnerException?.Message ?? e.Message);
                    }

                    xmlEnvio = serIntegracaoCorreios.inspector.LastRequestXML;
                    xmlRetorno = serIntegracaoCorreios.inspector.LastResponseXML;

                    if (!integrouSucesso)
                    {
                        pedidoIntegracao.ProblemaIntegracao = mensagem;
                        pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (result?.ObjetoPostal != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ObjetoPostal objetoPostal = (from o in result.ObjetoPostal where o.NumeroEtiqueta == etiquetasCorreios select o).FirstOrDefault();

                        if (objetoPostal != null)
                        {
                            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork, configuracao);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorPedido(pedidoIntegracao.Pedido.Codigo);
                            int total = cargaPedidos.Count;
                            if (total > 0)
                            {
                                for (int i = 0; i < total; i++)
                                {
                                    // Garantir que o fluxo ainda não passou da etapa do frete.
                                    if (cargaPedidos[i].Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica ||
                                        cargaPedidos[i].Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova ||
                                        cargaPedidos[i].Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                                    {
                                        decimal.TryParse(objetoPostal.ValorCobrado.Replace('.', ','), out decimal valorFreteTemp);
                                        if (valorFreteTemp > 0)
                                        {
                                            unitOfWork.Start();

                                            decimal valorFrete = 0;
                                            //decimal.TryParse(result.ObjetoPostal.ValorCobrado.Replace('.', ','), out decimal valorFrete);
                                            valorFrete = decimal.Parse(objetoPostal.ValorCobrado, cultura);

                                            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);
                                            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaPedidos[i].Carga.Codigo);

                                            carga.MotivoPendencia = "";
                                            carga.PossuiPendencia = false;
                                            carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;
                                            carga.ValorFrete = valorFrete;
                                            carga.DataEnvioUltimaNFe = DateTime.Now;

                                            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> componentesCarga = repCargaComponentesFrete.BuscarPorCarga(carga.Codigo);
                                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componenteCarga in componentesCarga)
                                                repCargaComponentesFrete.Deletar(componenteCarga);

                                            servicoRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, false, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                                            Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = (Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Correios", $" Valor Informado = {carga.ValorFrete.ToString("n2")}", carga.ValorFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Correios", 0, carga.ValorFrete));
                                            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicaoFrete, unitOfWork, null);

                                            carga.ValorFreteOperador = carga.ValorFrete;

                                            repCarga.Atualizar(carga);

                                            servicoCargaJanelaCarregamento.AtualizarSituacao(carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                                            servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, configuracao, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                                            unitOfWork.CommitChanges();

                                            pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                        }
                                        else
                                        {
                                            mensagem = "Valor retornado no campo valor_cobrado está inválido ou igual a zero.";
                                            pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                mensagem = "Pedido não encontrado em nenhuma carga.";
                                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            }
                        }
                        else
                        {
                            mensagem = "Retorno não contém numero etiqueta igual a " + etiquetasCorreios;
                            pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                    }
                    else
                    {
                        mensagem = "Valor de frete não encontrado na resposta do WS dos Correios.";
                        pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                }
                catch (ServicoException e)
                {
                    unitOfWork.Rollback();
                    mensagem = e.Message;
                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e, "IntegracaoCorreios");
                    Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoCorreios");
                    Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoCorreios");

                    unitOfWork.Rollback();

                    if (string.IsNullOrWhiteSpace(xmlRetorno))
                        mensagem = "Ocorreu uma falha ao comunicar com o Serviço dos Correios.";
                    else
                        mensagem = "Correios: " + (e.InnerException?.Message ?? e.Message);
                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                arquivoIntegracao.Mensagem = !string.IsNullOrWhiteSpace(mensagemException) ? mensagemException.Length > 400 ? mensagemException.Substring(0, 400) : mensagemException : mensagem.Length > 400 ? mensagem.Substring(0, 400) : mensagem;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "xml", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "xml", unitOfWork);

                Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo repPedidoIntegracaoArquivo = new Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo(unitOfWork);
                repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            }
            else
            {
                mensagem = "Plp não informada/inválida.";
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            pedidoIntegracao.ProblemaIntegracao = mensagem;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.CabecalhoOcorrencias ConsultarOcorrenciaPostagem(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoCorreios, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            var configuracaoIntegracao = repConfiguracaoIntegracao.BuscarPrimeiroRegistro();

            string urlToken = configuracaoCorreios.URLToken;
            string numeroCliente = configuracaoCorreios.CartaoPostagem;
            string user = configuracaoIntegracao.UsuarioCorreios;
            string password = configuracaoIntegracao.SenhaCorreios;
            string endPoint = configuracaoCorreios.URLEventos;

            string codigoObjetos = pedido.NumeroRastreioCorreios; // "QM562812498BR";
            string mensagemRetorno = string.Empty;
            string token = ObterToken(urlToken, numeroCliente, user, password, ref mensagemRetorno);

            if (string.IsNullOrWhiteSpace(token))
                throw new ServicoException("Correios não retornou token " + mensagemRetorno);

            endPoint = string.Concat(endPoint, "/objetos?codigosObjetos=", codigoObjetos, "&resultado=T");

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCorreios));
            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonRequest = endPoint;
            var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
            var result = client.GetAsync(endPoint).Result;
            var jsonResponse = result.Content.ReadAsStringAsync().Result;

            if (result.IsSuccessStatusCode)
            {
                var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                Servicos.Log.TratarErro("Retorno Correios " + endPoint, "ProcessarIntegracaCorreios");
                Servicos.Log.TratarErro(retorno, "IntegracaoCorreios");

                return JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.CabecalhoOcorrencias>(retorno);

            }
            else
            {
                Servicos.Log.TratarErro("Retorno Correios " + endPoint + result.StatusCode, "ProcessarIntegracaCorreios");

                return null;
            }
        }

        #region Métodos públicos

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ReturnSolicitaXmlPlp SolicitarXMLPLP(long idPLPMaster, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            PrepararClientWS(unitOfWork);
            try
            {
                Servicos.Correios.solicitaXmlPlpResponse response = this.client.solicitaXmlPlpAsync(idPLPMaster, this.Usuario, this.Senha).Result;
                XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ReturnSolicitaXmlPlp));
                TextReader reader = new StringReader(response.@return);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ReturnSolicitaXmlPlp result = (Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ReturnSolicitaXmlPlp)serializer.Deserialize(reader);
                return result;
            }
            catch (WebException e)
            {
                throw new ServicoException(e.Message);
            }
        }

        public void IntegrarPreListaPostagem(ref Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedidoIntegracao == null)
                return;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo repPedidoIntegracaoArquivo = new Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoCorreios repositorioConfiguracaoIntegracaoCorreios = new Repositorio.Embarcador.Configuracoes.IntegracaoCorreios(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoIntegracaoCorreios = repositorioConfiguracaoIntegracaoCorreios.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if (configuracaoIntegracao == null)
                return;

            pedidoIntegracao.Tentativas++;
            pedidoIntegracao.DataEnvio = DateTime.Now;

            string mensagemErro = string.Empty;
            string xmlRequisicao = string.Empty;
            string xmlResponse = string.Empty;
            try
            {
                PrepararClientWS(configuracaoIntegracaoCorreios, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ReturnSolicitaXmlPlp requestCorreios = ObterXML(pedidoIntegracao, configuracaoIntegracaoCorreios, configuracaoIntegracao, unitOfWork);
                xmlRequisicao = Utilidades.XML.Serializar(requestCorreios);

                string xmlcomEncoding = xmlRequisicao.Replace("<correioslog>", $"<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>\n<correioslog>");
                string codigoEtiqueta = pedidoIntegracao.Pedido?.NumeroRastreioCorreios ?? string.Empty;

                if (string.IsNullOrWhiteSpace(codigoEtiqueta))
                    throw new ServicoException("Número de rastreio precisa ser informado");

                List<string> etiquetas = new List<string>();
                etiquetas.Add(RemoverDigitoVerificador(codigoEtiqueta));
                string[] arrayEtiquetas = etiquetas.ToArray();

                XmlDocument xDoc = new XmlDocument();
                long response = client.fechaPlpVariosServicos(xDoc.CreateCDataSection(xmlcomEncoding), pedidoIntegracao.Pedido?.Numero ?? 0, configuracaoIntegracaoCorreios?.CartaoPostagem ?? string.Empty, arrayEtiquetas, configuracaoIntegracaoCorreios.UsuarioSIGEP, configuracaoIntegracaoCorreios.SenhaSIGEP);

                if (response > 0)
                {
                    pedidoIntegracao.Pedido.PLPCorreios = response.ToString();
                    repPedido.Atualizar(pedidoIntegracao.Pedido);
                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
            }
            catch (ServicoException e)
            {
                mensagemErro = e.Message;
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "IntegrarPreListaPostagem");
                Servicos.Log.TratarErro("Request: " + inspector.LastRequestXML, "IntegrarPreListaPostagem");
                Servicos.Log.TratarErro("Response: " + inspector.LastResponseXML, "IntegrarPreListaPostagem");

                if (string.IsNullOrWhiteSpace(inspector.LastResponseXML))
                    mensagemErro = $"Ocorreu uma falha ao comunicar com o Serviço dos Correios. Erro: {e?.Message ?? string.Empty}";
                else
                    mensagemErro = "Correios: " + (e.InnerException?.Message ?? e.Message);
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
            arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
            arquivoIntegracao.Mensagem = mensagemErro;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);

            arquivoIntegracao.ArquivoRequisicao = arquivoRequisicao;
            arquivoIntegracao.ArquivoResposta = arquivoResposta;

            repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

            pedidoIntegracao.ProblemaIntegracao = mensagemErro;
            pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        #endregion

        #region Métodos privados

        private void CarregarConfiguracoes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
            if (integracao == null)
            {
                throw new Exception("Sem integração configurada.");
            }
            else if (string.IsNullOrWhiteSpace(integracao.URLCorreios))
            {
                throw new Exception("Integração Correios sem URL.");
            }
            else if (string.IsNullOrWhiteSpace(integracao.UsuarioCorreios))
            {
                throw new Exception("Integração Correios sem usuário.");
            }
            else if (string.IsNullOrWhiteSpace(integracao.SenhaCorreios))
            {
                throw new Exception("Integração Correios sem senha.");
            }
            this.URL = integracao.URLCorreios;
            this.Usuario = integracao.UsuarioCorreios;
            this.Senha = integracao.SenhaCorreios;
        }

        private void PrepararClientWS(Repositorio.UnitOfWork unitOfWork)
        {
            this.client = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<Servicos.Correios.AtendeClienteClient, Servicos.Correios.AtendeCliente>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Correios_AtendeCliente, out inspector);

            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(this.URL);
        }

        private void PrepararClientWS(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoCorreios, Repositorio.UnitOfWork unitOfWork)
        {
            this.client = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<Servicos.Correios.AtendeClienteClient, Servicos.Correios.AtendeCliente>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Correios_AtendeCliente, out inspector);

            client.Endpoint.Address = new System.ServiceModel.EndpointAddress(configuracaoCorreios.URLPLP);
        }

        private static string ObterToken(string url, string numeroCliente, string user, string password, ref string mensagemRetorno)
        {
            string basicAuthorization = Utilidades.String.Base64Encode(user + ":" + password);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCorreios));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthorization);

            var dadosRequisicao = new
            {
                numero = numeroCliente
            };
            string jsonRequestBody = JsonConvert.SerializeObject(dadosRequisicao, Newtonsoft.Json.Formatting.Indented);

            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            HttpResponseMessage result = client.PostAsync(url, content).Result;

            if (!result.IsSuccessStatusCode)
            {
                string jsonErro = result.Content.ReadAsStringAsync().Result;
                dynamic objErro = JsonConvert.DeserializeObject<dynamic>(jsonErro);
                if (objErro != null && objErro.msgs != null && objErro.msgs.Count > 0)
                    mensagemRetorno = (string)objErro.msgs[0];
                else
                    mensagemRetorno = result.ReasonPhrase;
                return null;
            }

            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            dynamic obj = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            return (string)obj.token;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ReturnSolicitaXmlPlp ObterXML(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoIntegracaoCorreios, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ReturnSolicitaXmlPlp request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ReturnSolicitaXmlPlp();

            request.TipoArquivo = "Postagem";
            request.VersaoArquivo = "2.3";
            request.PLP = PreencherPLP(configuracaoIntegracaoCorreios);
            request.Remetente = PreencherRemetente(configuracaoIntegracaoCorreios, pedidoIntegracao);
            request.ObjetoPostal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ObjetoPostal[] { PreencherObjetoPostal(pedidoIntegracao, configuracaoIntegracaoCorreios, unitOfWork) };

            return request;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.PLP PreencherPLP(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoIntegracaoCorreios)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.PLP()
            {
                id_plp = string.Empty,
                ValorGlobal = string.Empty,
                MCUUnidadePostagem = string.Empty,
                NomeUnidadePostagem = string.Empty,
                CartaoPostagem = configuracaoIntegracaoCorreios?.CartaoPostagem ?? string.Empty,
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.Remetente PreencherRemetente(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoIntegracaoCorreios, Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.Remetente
            {
                NumeroContrato = configuracaoIntegracaoCorreios?.NumeroContrato.ToLong() ?? 0,
                NumeroDiretoria = configuracaoIntegracaoCorreios?.NumeroDiretoria.ToInt() ?? 0,
                CodigoAdministrativo = configuracaoIntegracaoCorreios?.CodigoAdministrativo.ToInt() ?? 0,
                Nome = pedidoIntegracao.Pedido?.Remetente?.Nome ?? string.Empty,
                Logradouro = pedidoIntegracao.Pedido?.Remetente?.Endereco ?? string.Empty,
                Numero = pedidoIntegracao.Pedido?.Remetente?.Numero ?? string.Empty,
                Complemento = pedidoIntegracao.Pedido?.Remetente?.Complemento ?? string.Empty,
                Bairro = pedidoIntegracao.Pedido?.Remetente?.Bairro ?? string.Empty,
                CEP = pedidoIntegracao.Pedido?.Remetente?.CEP ?? string.Empty,
                Cidade = pedidoIntegracao.Pedido?.Remetente?.Localidade?.Descricao ?? string.Empty,
                UF = pedidoIntegracao.Pedido?.Remetente?.Localidade?.Estado?.Sigla ?? string.Empty,
                Telefone = pedidoIntegracao.Pedido?.Remetente?.Telefone1 ?? string.Empty,
                Fax = string.Empty,
                Email = pedidoIntegracao.Pedido?.Remetente?.Email ?? string.Empty,
                Celular = pedidoIntegracao.Pedido?.Remetente?.Celular ?? string.Empty,
                CPF_CNPJ = pedidoIntegracao.Pedido?.Remetente?.CPF_CNPJ.ToString() ?? string.Empty,
                CienciaConteudoProibido = "S",
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ObjetoPostal PreencherObjetoPostal(Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoIntegracaoCorreios, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = repPedidoXmlNotaFiscal.BuscarPrimeiraNotaFiscalPorPedido(pedidoIntegracao.Pedido?.Codigo ?? 0);

            decimal pesoEmGramas = notaFiscal.Peso * 1000;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ObjetoPostal objetoPostal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ObjetoPostal()
            {
                NumeroEtiqueta = pedidoIntegracao.Pedido?.NumeroRastreioCorreios ?? string.Empty,
                SSCC = string.Empty,
                CodigoObjetoCliente = string.Empty,
                CodigoServicoPostagem = pedidoIntegracao.Empresa?.CodigoServicoCorreios ?? "03298",
                Cubagem = "0.00",
                Peso = pesoEmGramas.ToString("N0").Replace(".", ""),
                RT1 = string.Empty,
                RT2 = string.Empty,
                RestricaoANAC = "S",
            };

            objetoPostal.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.Destinatario()
            {
                Nome = pedidoIntegracao.Pedido?.Destinatario?.Nome ?? string.Empty,
                Telefone = pedidoIntegracao.Pedido?.Destinatario?.Telefone1 ?? string.Empty,
                Celular = pedidoIntegracao.Pedido?.Destinatario?.Celular ?? string.Empty,
                Email = pedidoIntegracao.Pedido?.Destinatario?.Email ?? string.Empty,
                Logradouro = pedidoIntegracao.Pedido?.Destinatario?.Endereco ?? string.Empty,
                Complemento = pedidoIntegracao.Pedido?.Destinatario?.Complemento ?? string.Empty,
                Numero = pedidoIntegracao.Pedido?.Destinatario?.Numero ?? string.Empty,
                CPFCNPJ = pedidoIntegracao.Pedido?.Destinatario?.CPF_CNPJ.ToString() ?? string.Empty,
            };

            objetoPostal.Nacional = new Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.Nacional()
            {
                BairroDestinatario = pedidoIntegracao.Pedido?.Destinatario?.Bairro ?? string.Empty,
                CidadeDestinatario = pedidoIntegracao.Pedido?.Destinatario?.Localidade?.Descricao ?? string.Empty,
                UFDestinatario = pedidoIntegracao.Pedido?.Destinatario?.Localidade?.Estado?.Sigla ?? string.Empty,
                CEPDestinatario = pedidoIntegracao.Pedido?.Destinatario?.CEP ?? string.Empty,
                CodigoUsuarioPostal = string.Empty,
                CentroCustoCliente = string.Empty,
                NumeroNotaFiscal = notaFiscal != null ? notaFiscal.Numero.ToString() : string.Empty,
                SerieNotaFiscal = string.Empty,
                ValorNotaFiscal = string.Empty,
                NaturezaNotaFiscal = string.Empty,
                DescricaoObjeto = string.Empty,
                ValorACobrar = "0,0",
            };

            objetoPostal.ServicoAdicional = new Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.ServicoAdicional()
            {
                CodigosServicoAdicional = configuracaoIntegracaoCorreios?.CodigoServicoAdicional ?? string.Empty,
                ValorDeclarado = string.Empty,
            };

            objetoPostal.DimensaoObjeto = new Dominio.ObjetosDeValor.Embarcador.Integracao.Correios.DimensaoObjeto()
            {
                TipoObjeto = "002",
                Altura = "1",
                Largura = "1",
                Comprimento = "1",
                Diametro = "0",
            };

            objetoPostal.DataPostagemSara = string.Empty;
            objetoPostal.StatusProcessamento = "0";
            objetoPostal.NumeroComprovantePostagem = string.Empty;
            objetoPostal.ValorCobrado = string.Empty;

            return objetoPostal;
        }

        private string RemoverDigitoVerificador(string codigoEtiqueta)
        {
            string padraoLetras = @"[A-Za-z]+";
            string somenteNumerosEtiqueta = codigoEtiqueta.ObterSomenteNumeros();
            string numerosSemCodigoVerificador = somenteNumerosEtiqueta.Length > 1 ? somenteNumerosEtiqueta.Substring(0, somenteNumerosEtiqueta.Length - 1) : somenteNumerosEtiqueta; //somenteNumerosEtiqueta.TrimEnd(somenteNumerosEtiqueta[somenteNumerosEtiqueta.Length -1]);

            MatchCollection matchesLetras = Regex.Matches(codigoEtiqueta, padraoLetras);

            string resultado = matchesLetras[0].Value;

            resultado += numerosSemCodigoVerificador;

            if (matchesLetras.Count > 1)
                resultado += matchesLetras[1].Value;

            return resultado;
        }

        #endregion

    }

}

