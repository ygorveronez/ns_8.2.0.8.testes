using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.Ortec
{
    public sealed class IntegracaoOrtec
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoOrtec(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        public class DynamicXml : DynamicObject
        {
            XElement _root;
            private DynamicXml(XElement root)
            {
                _root = root;
            }

            public static DynamicXml Parse(string xmlString)
            {
                return new DynamicXml(RemoveNamespaces(XDocument.Parse(xmlString).Root));
            }

            public static DynamicXml Load(string filename)
            {
                return new DynamicXml(RemoveNamespaces(XDocument.Load(filename).Root));
            }

            private static XElement RemoveNamespaces(XElement xElem)
            {
                var attrs = xElem.Attributes()
                            .Where(a => !a.IsNamespaceDeclaration)
                            .Select(a => new XAttribute(a.Name.LocalName, a.Value))
                            .ToList();

                if (!xElem.HasElements)
                {
                    XElement xElement = new XElement(xElem.Name.LocalName, attrs);
                    xElement.Value = xElem.Value;
                    return xElement;
                }

                var newXElem = new XElement(xElem.Name.LocalName, xElem.Elements().Select(e => RemoveNamespaces(e)));
                newXElem.Add(attrs);
                return newXElem;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;

                var att = _root.Attribute(binder.Name);
                if (att != null)
                {
                    result = att.Value;
                    return true;
                }

                var nodes = _root.Elements(binder.Name);
                if (nodes.Count() > 1)
                {
                    result = nodes.Select(n => n.HasElements ? (object)new DynamicXml(n) : n.Value).ToList();
                    return true;
                }

                var node = _root.Element(binder.Name);
                if (node != null)
                {
                    result = node.HasElements || node.HasAttributes ? (object)new DynamicXml(node) : node.Value;
                    return true;
                }

                return true;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.order ObterObterOrder(Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao integracaoSeparacao)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProdutoLote repXMLNotaFiscalProdutoLote = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProdutoLote(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.order order = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.order();
            order.id = integracaoSeparacao.Codigo;

            Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido separacaoPedidoPedido = integracaoSeparacao.SeparacaoPedidoPedido;

            order.transport_details = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.transport_details();
            order.SKUs = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.SKU>();
            order.reentrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.reentrega();
            if (separacaoPedidoPedido == null)
            {
                separacaoPedidoPedido = integracaoSeparacao.SeparacaoPedido.Pedidos.FirstOrDefault();
                order.weight = integracaoSeparacao.SeparacaoPedido.Pedidos.Sum(obj => obj.Pedido.PesoTotal);
                order.reentrega.status = separacaoPedidoPedido.Pedido.ReentregaSolicitada;
                order.transport_details.type = "AGRP";
                order.order_number = separacaoPedidoPedido.Pedido.NumeroPedidoEmbarcador;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.SKU sKU = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.SKU();
                sKU.code = "AGRP";
                sKU.lot = "";
                sKU.amount = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.amount();
                sKU.amount.unit = "UN";
                sKU.amount.value = 1;
                order.SKUs.Add(sKU);
            }
            else
            {
                order.weight = integracaoSeparacao.XMLNotaFiscal.Peso;
                order.invoice_number = integracaoSeparacao.XMLNotaFiscal.Numero.ToString();
                order.invoice_serie = integracaoSeparacao.XMLNotaFiscal.Serie.ToString();
                order.invoice_date = integracaoSeparacao.XMLNotaFiscal.DataEmissao.ToString("yyyy-MM-dd");
                order.invoice_seller = integracaoSeparacao.XMLNotaFiscal.Emitente.CPF_CNPJ_SemFormato;
                order.invoice_customer = integracaoSeparacao.XMLNotaFiscal.Destinatario.CPF_CNPJ_SemFormato;

                if (integracaoSeparacao.XMLNotaFiscal.Emitente?.GrupoPessoas?.ProvisionarDocumentos == true)
                    order.order_number = integracaoSeparacao.XMLNotaFiscal.NumeroPedidoEmbarcador;
                else
                {
                    order.colog_CNPJ = separacaoPedidoPedido.Pedido.Remetente.CPF_CNPJ_SemFormato;
                    order.order_number = integracaoSeparacao.XMLNotaFiscal.Numero.ToString();
                }

                order.transport_details.type = "NRML";
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote> xMLNotaFiscalProdutoLotes = repXMLNotaFiscalProdutoLote.BuscarPorNotaFiscal(integracaoSeparacao.XMLNotaFiscal.Codigo);
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote XMLNotaFiscalProdutoLote in xMLNotaFiscalProdutoLotes)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.SKU sKU = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.SKU();
                    sKU.code = XMLNotaFiscalProdutoLote.XMLNotaFiscalProduto.Produto.CodigoProdutoEmbarcador;
                    sKU.lot = XMLNotaFiscalProdutoLote.NumeroLote;
                    sKU.amount = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.amount();
                    sKU.amount.unit = "UN";
                    sKU.amount.value = XMLNotaFiscalProdutoLote.XMLNotaFiscalProduto.Quantidade;
                    order.SKUs.Add(sKU);
                }
            }

            order.reentrega.status = separacaoPedidoPedido.Pedido.ReentregaSolicitada;

            if (separacaoPedidoPedido.SeparacaoPedido.DataExpedicao.HasValue)
                order.reentrega.pickupDate = separacaoPedidoPedido.SeparacaoPedido.DataExpedicao.Value.ToString("yyyy-MM-dd");
            else
                order.reentrega.pickupDate = separacaoPedidoPedido.Pedido.DataInicialColeta?.ToString("yyyy-MM-dd") ?? integracaoSeparacao.SeparacaoPedido.Data.ToString("yyyy-MM-dd");

            order.transport_details.pickup_code = separacaoPedidoPedido.Pedido.Remetente.CPF_CNPJ_SemFormato;
            order.transport_details.delivery_code = integracaoSeparacao.SeparacaoPedido.LocalEntrega?.CPF_CNPJ_SemFormato ?? separacaoPedidoPedido.Pedido.Remetente.CPF_CNPJ_SemFormato;
            order.transport_details.leadtime = 1;//fixo 1, (é o tempo para entrega em dias), verificar e implementar.


            return order;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shift ObterObjetoIntegracaoEntregas(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao integracaoEntrega)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shift xmlintegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shift();

            xmlintegracao.id_shift = (uint)integracaoEntrega.CodigoAgrupador;
            xmlintegracao.totalDistance = integracaoEntrega.Carga.Distancia > 0 ? (int)integracaoEntrega.Carga.Distancia : 0;
            xmlintegracao.startInstant = integracaoEntrega.Carga.DataInicioViagem != null && integracaoEntrega.Carga.DataInicioViagem != DateTime.MinValue ? (DateTime)integracaoEntrega.Carga.DataInicioViagem : DateTime.Now;
            xmlintegracao.finishInstant = integracaoEntrega.Carga.DataFimViagem != null && integracaoEntrega.Carga.DataFimViagem != DateTime.MinValue ? (DateTime)integracaoEntrega.Carga.DataFimViagem : DateTime.Now;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shiftResources dadosCarga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shiftResources();
            dadosCarga.driver = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shiftResourcesDriver();
            dadosCarga.truck = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shiftResourcesTruck();

            dadosCarga.driver.code = integracaoEntrega.Carga.Motoristas.Count > 0 ? integracaoEntrega.Carga.Motoristas[0].CPF : "";
            dadosCarga.driver.name = integracaoEntrega.Carga.Motoristas.Count > 0 ? integracaoEntrega.Carga.Motoristas[0].Nome : "";
            dadosCarga.truck.code = integracaoEntrega.Carga.Veiculo != null ? integracaoEntrega.Carga.Veiculo.Placa : "";
            dadosCarga.truck.name = integracaoEntrega.Carga.Veiculo != null ? integracaoEntrega.Carga.Veiculo.Descricao : "";

            xmlintegracao.resources = dadosCarga;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> ListaCargaEntregaNotaFiscal = repCargaEntregaNotaFiscal.BuscarPorCarga(integracaoEntrega.Carga.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.ShiftOrders shiftOrder = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.ShiftOrders();

            int cont = 1;
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in ListaCargaEntregaNotaFiscal)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shifOrderOrder order = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shifOrderOrder();
                order.order_reference = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.NumeroPedidoEmbarcador; //.ToLong(); //cargaEntregaNotaFiscal.CargaEntrega.Ordem;
                order.invoice_number = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero; //nota fiscal
                order.longitude = cargaEntregaNotaFiscal.CargaEntrega.LongitudeFinalizada ?? 0;
                order.latitude = cargaEntregaNotaFiscal.CargaEntrega.LatitudeFinalizada ?? 0;
                order.sequence = cargaEntregaNotaFiscal.CargaEntrega.OrdemRealizada;
                if (cargaEntregaNotaFiscal.CargaEntrega.DataEntradaRaio.HasValue)
                    order.startInstant = cargaEntregaNotaFiscal.CargaEntrega.DataEntradaRaio.Value;
                if (cargaEntregaNotaFiscal.CargaEntrega.DataSaidaRaio.HasValue)
                    order.finishInstant = cargaEntregaNotaFiscal.CargaEntrega.DataSaidaRaio.Value;

                order.orderStatus = cargaEntregaNotaFiscal.CargaEntrega.Situacao == SituacaoEntrega.Entregue ? "Entregue" : "Nao Entregue"; // "TBD"; //ver o q colocar aqui;

                order.amounts = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shiftOrdersOrderAmount[1];

                Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shiftOrdersOrderAmount amountPeso = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shiftOrdersOrderAmount();
                amountPeso.unit_code = "kg";
                amountPeso.value = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso; //peso notas

                order.amounts[0] = amountPeso;
                if (shiftOrder.shiftOrders == null)
                    shiftOrder.shiftOrders = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shifOrderOrder>();

                shiftOrder.shiftOrders.Add(order);
                cont++;
            }

            xmlintegracao.orders = shiftOrder;
            return xmlintegracao;
        }

        public void EnviarPedidoSeparacaoOrtec(Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao integracaoSeparacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao repSeparacaoPedidoIntegracao = new Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            if (string.IsNullOrWhiteSpace(integracao.URLOrtec))
            {
                integracaoSeparacao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoSeparacao.ProblemaIntegracao = "Não foi configurada a integração com a Ortec, por favor verifique.";
                return;
            }

            AppOrtec.ApplicationIntegrationServiceClient applicationIntegrationServiceClient = ObterClient(integracao);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.order order = ObterObterOrder(integracaoSeparacao);

            XmlSerializerNamespaces emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            System.Xml.Serialization.XmlSerializer xmlSerializerSend = new System.Xml.Serialization.XmlSerializer(order.GetType());

            string xmlOrder = "";
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                xmlSerializerSend.Serialize(writer, order, emptyNamespaces);
                xmlOrder = stream.ToString();
            }
            //XmlDocument xDoc = new XmlDocument();
            //xmlOrder = "<![CDATA[" + xmlOrder + "]]>"; //xDoc.CreateCDataSection(xmlOrder.Replace(" <![CDATA[", "").Replace("]]>", ""));

            integracaoSeparacao.DataIntegracao = DateTime.Now;
            integracaoSeparacao.NumeroTentativas += 1;

            InspectorBehavior inspector = new InspectorBehavior();

            applicationIntegrationServiceClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem = null;
            bool sucesso = false;

            try
            {
                string retorno = applicationIntegrationServiceClient.SendMessage(xmlOrder, "MS_importOrderColog");
                Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.comtec comtec = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.comtec();
                System.Xml.Serialization.XmlSerializer xmlSerializerReturn = new System.Xml.Serialization.XmlSerializer(comtec.GetType());
                using (TextReader reader = new StringReader(retorno))
                {
                    comtec = (dynamic)xmlSerializerReturn.Deserialize(reader);
                }

                if (comtec.transport_order.success.ToLower() == "true")
                {
                    sucesso = true;
                    mensagem = "Integração realizada com sucesso.";
                }
                else
                {
                    sucesso = false;
                    mensagem = comtec.transport_order.answer;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falha ao realizar a integração.";
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
            arquivoIntegracao.Data = integracaoSeparacao.DataIntegracao;
            arquivoIntegracao.Mensagem = mensagem;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            integracaoSeparacao.ArquivosTransacao.Add(arquivoIntegracao);
            integracaoSeparacao.ProblemaIntegracao = mensagem;

            if (!sucesso)
                integracaoSeparacao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            else
                integracaoSeparacao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

            repSeparacaoPedidoIntegracao.Atualizar(integracaoSeparacao);
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repPreAgrupamentoCargaAgrupador = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga = repPreAgrupamentoCarga.BuscarPorCarga(cargaAtual.Codigo);

            if (preAgrupamentoCarga != null)
            {
                preAgrupamentoCarga.Carga = cargaNova;
                repPreAgrupamentoCarga.Atualizar(preAgrupamentoCarga);
                Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador agrupador = repPreAgrupamentoCargaAgrupador.BuscarPorCodigo(preAgrupamentoCarga.Agrupador.Codigo);
                if (!repPreAgrupamentoCarga.VerificarSeExisteCargaDePreCargaPorAgrupamento(agrupador.Codigo) && agrupador.IsTodosAgrupamentosPossuemCarga(false))
                {
                    bool ajustarSequencia = false;
                    if (agrupador.Carga?.Codigo == cargaAtual.Codigo)
                    {
                        agrupador.Carga = cargaNova;
                        ajustarSequencia = true;
                    }

                    agrupador.PossuiPreCargas = false;
                    repPreAgrupamentoCargaAgrupador.Atualizar(agrupador);

                    if (ajustarSequencia)
                        AjustarSequenciaEntrega(agrupador);
                }
            }
        }

        private void AtualizarSituacaoAgrupadores(HashSet<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> listaPreAgrupamentoCargaAgrupador)
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repositorioPreAgrupamentoCargaAgrupador = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador preAgrupamentoCargaAgrupador in listaPreAgrupamentoCargaAgrupador)
            {
                if (preAgrupamentoCargaAgrupador.PossuiPreCargas || preAgrupamentoCargaAgrupador.Situacao == SituacaoPreAgrupamentoCarga.AguardandoCargasPararEncaixe)
                {
                    if (!repPreAgrupamentoCarga.VerificarSeExisteCargaDePreCargaPorAgrupamento(preAgrupamentoCargaAgrupador.Codigo) && preAgrupamentoCargaAgrupador.IsTodosAgrupamentosPossuemCarga(false))
                    {
                        preAgrupamentoCargaAgrupador.PossuiPreCargas = false;
                        repositorioPreAgrupamentoCargaAgrupador.Atualizar(preAgrupamentoCargaAgrupador);
                        continue;
                    }
                }


                if (preAgrupamentoCargaAgrupador.IsTodosAgrupamentosPossuemCarga(preAgrupamentoCargaAgrupador.PossuiPreCargas))
                {
                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.AguardandoCarregamento;

                    repositorioPreAgrupamentoCargaAgrupador.Atualizar(preAgrupamentoCargaAgrupador);
                }
                else if (preAgrupamentoCargaAgrupador.IsTodosAgrupamentosPossuemCargaOuCargaRedespacho(preAgrupamentoCargaAgrupador.PossuiPreCargas))
                {
                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.AguardandoRedespacho;

                    repositorioPreAgrupamentoCargaAgrupador.Atualizar(preAgrupamentoCargaAgrupador);
                }
            }
        }

        private List<Dominio.Entidades.Embarcador.Cargas.Carga> AplicarRecebedorCargaPedido(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool proximoTrechoComplemento, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAjustar = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoFor in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidoFor;
                if (cargaPedido.Recebedor == null)
                {
                    cargaPedido.Recebedor = recebedor;
                    cargaPedido.Destino = recebedor.Localidade;
                    if (cargaPedido.Expedidor != null)
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
                    else
                        cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;

                    cargaPedido.ProximoTrechoComplementaFilialEmissora = (proximoTrechoComplemento && recebedor.Localidade.Estado.Sigla != cargaPedido.Origem.Estado.Sigla && !cargaPedido.EmitirComplementarFilialEmissora && cargaPedido.CargaPedidoFilialEmissora);
                    if (cargaPedido.ProximoTrechoComplementaFilialEmissora)
                        serCargaPedido.VerificarFilialEmissaoCargaPedido(cargaPedido, configuracaoGeralCarga);

                    repCargaPedido.Atualizar(cargaPedido);

                    if (!cargasAjustar.Contains(cargaPedido.Carga))
                        cargasAjustar.Add(cargaPedido.Carga);
                }
            }
            return cargasAjustar;
        }

        private void AgruparCargas(Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador agrupamento, List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentoCargas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal repPreAgrupamentoNotaFiscal = new Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.Rota serRota = new Servicos.Embarcador.Carga.Rota(_unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(_unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            // manda primeiro a que tem empresa para nos redespachos se necessário puxar automaticamente os dados da carga base.
            cargas.AddRange((from obj in preAgrupamentoCargas where obj.Carga != null && obj.Carga.Empresa != null select obj.Carga).Distinct().ToList());
            cargas.AddRange((from obj in preAgrupamentoCargas where obj.Carga != null && obj.Carga.Empresa == null select obj.Carga).Distinct().ToList());

            List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> preAgrupamentoNotas = repPreAgrupamentoNotaFiscal.BuscarPorCargaAgrupadores((from obj in preAgrupamentoCargas select obj.Codigo).ToList());

            if (cargas.Count < 2)
            {
                List<string> recebedores = (from obj in preAgrupamentoCargas where !string.IsNullOrWhiteSpace(obj.CnpjRecebedor) select obj.CnpjRecebedor).Distinct().ToList();
                if (recebedores.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAjustar = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                    bool proximoTrechoComplemento = false;

                    if (recebedores.Count > 1)
                    {
                        foreach (string cnpjrecebedor in recebedores)
                        {
                            double codigoRecebedor = 0;
                            double.TryParse(cnpjrecebedor, out codigoRecebedor);
                            Dominio.Entidades.Cliente recebedor = repCliente.BuscarPorCPFCNPJ(codigoRecebedor);

                            if (recebedor != null)
                            {

                                if (configuracaoTMS.EmitirComplementarRedespachoFilialEmissoraDiferenteUFOrigem)
                                {
                                    if (repEmpresa.BuscarEmpresaFilialEmissoraPadraoPorEstadoOrigemRedespacho(recebedor.Localidade.Estado) == null)
                                        proximoTrechoComplemento = true;
                                }

                                List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentoCargasRecebedor = (from obj in preAgrupamentoCargas where obj.CnpjRecebedor == cnpjrecebedor select obj).ToList();
                                foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga PreAgrupamentoCargaRecebedor in preAgrupamentoCargasRecebedor)
                                {
                                    List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> preAgrupamentoNotasRecebedor = (from obj in preAgrupamentoNotas where obj.PreAgrupamentoCarga.Codigo == PreAgrupamentoCargaRecebedor.Codigo select obj).ToList();
                                    List<string> strNotas = (from obj in preAgrupamentoNotasRecebedor select obj.NumeroNota).ToList();
                                    List<int> notas = new List<int>();
                                    foreach (string strnota in strNotas)
                                    {
                                        int.TryParse(strnota, out int nota);
                                        if (nota > 0)
                                            notas.Add(nota);
                                    }
                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repPedidoXMLNotaFiscal.BuscarPorCargaENumeroNota(notas, PreAgrupamentoCargaRecebedor.Carga.Codigo);
                                    cargasAjustar = AplicarRecebedorCargaPedido(cargaPedidos, proximoTrechoComplemento, recebedor, configuracaoGeralCarga);
                                }
                            }
                        }
                    }
                    else
                    {
                        string cnpjrecebedor = recebedores.FirstOrDefault();
                        double.TryParse(cnpjrecebedor, out double codigoRecebedor);
                        Dominio.Entidades.Cliente recebedor = repCliente.BuscarPorCPFCNPJ(codigoRecebedor);
                        if (recebedor != null)
                        {
                            if (configuracaoTMS.EmitirComplementarRedespachoFilialEmissoraDiferenteUFOrigem)
                            {
                                if (repEmpresa.BuscarEmpresaFilialEmissoraPadraoPorEstadoOrigemRedespacho(recebedor.Localidade.Estado) == null)
                                    proximoTrechoComplemento = true;
                            }
                            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasPreAgrupamento = (from obj in preAgrupamentoCargas where obj.Carga != null select obj.Carga).Distinct().ToList();
                            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupamento in cargasPreAgrupamento)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaAgrupamento.Codigo);
                                cargasAjustar = AplicarRecebedorCargaPedido(cargaPedidos, proximoTrechoComplemento, recebedor, configuracaoGeralCarga);
                            }
                        }
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargasAjustar)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCarga = repCargaPedido.BuscarPorCarga(carga.Codigo);

                        if (carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                        {
                            Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, cargaPedidosCarga, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware);
                            carga.CalculandoFrete = true;
                            carga.DataInicioCalculoFrete = DateTime.Now;
                        }

                        serRota.DeletarPercursoDestinosCarga(carga, _unitOfWork);
                        serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargaPedidosCarga, _unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
                        Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidosCarga, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware);
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaref = carga;
                        serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaref, cargaPedidosCarga, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware);
                        repCarga.Atualizar(carga);
                    }
                }

                if (cargas.Count == 1)
                {
                    AjustarDataPrevisaoEntrega(preAgrupamentoNotas, cargas.FirstOrDefault());
                    agrupamento.Carga = cargas.FirstOrDefault();
                }

            }
            else
            {
                Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = new Carga.CargaAgrupada(_unitOfWork).AgruparCargas(cargas, preAgrupamentoCargas, agrupamento.CodigoAgrupamento.ToString(), null, null, tipoServicoMultisoftware, null);
                if (cargaAgrupada != null)
                {
                    AjustarDataPrevisaoEntrega(preAgrupamentoNotas, cargaAgrupada);
                    agrupamento.Carga = cargaAgrupada;
                }
            }
        }

        private void AjustarDataPrevisaoEntrega(List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> preAgrupamentoNotas, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (preAgrupamentoNotas.Any(obj => obj.DataPrevisaoEntrega.HasValue))
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXmlNotaFiscal.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoAjustarData = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscaltem in pedidoXMLNotaFiscals)
                {
                    Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal preAgrupamentoNotaFiscal = (from obj in preAgrupamentoNotas where obj.NumeroNota == pedidoXMLNotaFiscaltem.XMLNotaFiscal.Numero.ToString() select obj).FirstOrDefault();
                    if (preAgrupamentoNotaFiscal != null && preAgrupamentoNotaFiscal.DataPrevisaoEntrega.HasValue)
                    {
                        if (!cargaPedidoAjustarData.Contains(pedidoXMLNotaFiscaltem.CargaPedido))
                        {
                            pedidoXMLNotaFiscaltem.CargaPedido.PrevisaoEntrega = preAgrupamentoNotaFiscal.DataPrevisaoEntrega;
                            if (string.IsNullOrWhiteSpace(pedidoXMLNotaFiscaltem.XMLNotaFiscal.NumeroPedidoEmbarcador) && !string.IsNullOrWhiteSpace(preAgrupamentoNotaFiscal.NumeroPedido))
                            {
                                pedidoXMLNotaFiscaltem.XMLNotaFiscal.NumeroPedidoEmbarcador = preAgrupamentoNotaFiscal.NumeroPedido;
                                repXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscaltem.XMLNotaFiscal);
                            }

                            cargaPedidoAjustarData.Add(pedidoXMLNotaFiscaltem.CargaPedido);
                        }
                    }
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoAjustarData)
                    repCargaPedido.Atualizar(cargaPedido);
            }
        }

        private void CriarCargaDedicada(Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador agrupamento, List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentosEncaixe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware)
        {
            if (preAgrupamentosEncaixe.Count == 0)
                return;

            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarTipoOperacaoPadraoQuandoTotalmenteSubContradada();

            string cnpjExpedidor = (from obj in preAgrupamentosEncaixe select obj.CnpjExpedidor).FirstOrDefault();
            Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
            if (!string.IsNullOrWhiteSpace(cnpjExpedidor))
                filial = repFilial.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjExpedidor));

            if (filial == null)
                filial = preAgrupamentosEncaixe.FirstOrDefault().CargaPedidoEncaixe.Carga.Filial;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaDedicada = new Dominio.Entidades.Embarcador.Cargas.Carga()
            {
                CodigoCargaEmbarcador = agrupamento.CodigoAgrupamento.ToString(),
                DataEnvioUltimaNFe = DateTime.Now,
                DataInicioEmissaoDocumentos = DateTime.Now,
                Filial = filial,
                FilialOrigem = filial,
                TipoDeCarga = filial?.TipoDeCarga,
                ExigeNotaFiscalParaCalcularFrete = true,
                SituacaoCarga = SituacaoCarga.Nova,
                TipoOperacao = tipoOperacao,
                Empresa = agrupamento.Empresa
            };
            Dominio.Entidades.Veiculo veiculo = agrupamento.Veiculo;
            if (veiculo != null)
            {
                if (veiculo.TipoVeiculo == "1")
                {
                    if (agrupamento.Veiculo.VeiculosTracao != null && agrupamento.Veiculo.VeiculosTracao.Count > 0)
                        veiculo = agrupamento.Veiculo.VeiculosTracao.FirstOrDefault();
                }

                cargaDedicada.Veiculo = veiculo;
                if (veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                    cargaDedicada.VeiculosVinculados = veiculo.VeiculosVinculados.ToList();
            }

            repositorioCarga.Inserir(cargaDedicada);
            agrupamento.Carga = cargaDedicada;

            Dominio.Entidades.Usuario veiculoMotorista = veiculo != null ? repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo) : null;
            servicoCargaMotorista.AdicionarMotorista(cargaDedicada, veiculoMotorista);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDedicados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            List<int> codigos = (from obj in preAgrupamentosEncaixe select obj.Codigo).ToList();
            foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoEncaixe in preAgrupamentosEncaixe)
            {
                if (!cargaPedidosDedicados.Contains(preAgrupamentoEncaixe.CargaPedidoEncaixe))
                {
                    Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCargaExiste = repPreAgrupamentoCarga.VerificarSeExistePorCargaPedido(preAgrupamentoEncaixe.CargaPedidoEncaixe.Codigo, codigos, preAgrupamentoEncaixe.CnpjExpedidor);
                    if (preAgrupamentoCargaExiste == null || preAgrupamentoCargaExiste.CargaPedidoEncaixe.Pedido.ReentregaSolicitada)
                    {
                        CriarCargaDedicadaPedido(preAgrupamentoEncaixe, cargaDedicada, configuracaoTMS, tipoServicoMultisoftware);
                        cargaPedidosDedicados.Add(preAgrupamentoEncaixe.CargaPedidoEncaixe);
                    }
                    else
                    {
                        throw new ServicoException("Essa carga dedicada já foi gerada anteriormente no agrupamento " + preAgrupamentoCargaExiste.Agrupador.CodigoAgrupamento);
                    }

                }
            }
            Servicos.Embarcador.Carga.Carga serCarga = new Carga.Carga(_unitOfWork);
            serCarga.FecharCarga(cargaDedicada, _unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware);
            cargaDedicada.ExigeNotaFiscalParaCalcularFrete = true;
            cargaDedicada.CargaFechada = true;
            cargaDedicada.Protocolo = cargaDedicada.Codigo;
            repositorioCarga.Atualizar(cargaDedicada);
        }

        private void CriarCargaDedicadaPedido(Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoEncaixe, Dominio.Entidades.Embarcador.Cargas.Carga cargaDedicada, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = preAgrupamentoEncaixe.CargaPedidoEncaixe;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDuplicado = cargaPedido.Clonar();
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            Utilidades.Object.DefinirListasGenericasComoNulas(cargaPedidoDuplicado);

            cargaPedidoDuplicado.Carga = cargaDedicada;
            cargaPedidoDuplicado.CargaOrigem = cargaDedicada;
            cargaPedidoDuplicado.Pedido = cargaPedido.Pedido;
            cargaPedidoDuplicado.Recebedor = null;
            cargaPedidoDuplicado.Expedidor = ObterCliente(preAgrupamentoEncaixe.CnpjExpedidor.ToDouble());
            cargaPedidoDuplicado.Recebedor = ObterCliente(preAgrupamentoEncaixe.CnpjRecebedor.ToDouble());
            cargaPedidoDuplicado.CargaPedidoFilialEmissora = false;
            cargaPedidoDuplicado.Destino = cargaPedido.Pedido.Destino;
            cargaPedidoDuplicado.Origem = cargaPedido.Pedido.Origem;
            cargaPedidoDuplicado.TipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.Normal;
            cargaPedidoDuplicado.ValorFrete = 0;
            cargaPedidoDuplicado.PendenteGerarCargaDistribuidor = false;
            cargaPedidoDuplicado.ValorFreteAPagar = 0;
            cargaPedidoDuplicado.CargaRedespacho = null;
            cargaPedidoDuplicado.ValorFreteAPagarFilialEmissora = 0;
            cargaPedidoDuplicado.RegraTomador = null;
            cargaPedidoDuplicado.ValorFreteTabelaFrete = 0;
            cargaPedidoDuplicado.ValorFreteTabelaFreteFilialEmissora = 0;
            cargaPedidoDuplicado.ImpostoInformadoPeloEmbarcador = false;
            cargaPedidoDuplicado.ValorFreteFilialEmissora = 0;
            cargaPedidoDuplicado.BaseCalculoICMS = 0;
            cargaPedidoDuplicado.CargaPedidoProximoTrecho = null;
            cargaPedidoDuplicado.CargaPedidoTrechoAnterior = null;
            cargaPedidoDuplicado.ValorICMS = 0;
            cargaPedidoDuplicado.ValorAdValorem = 0;
            cargaPedidoDuplicado.ValorDescarga = 0;
            cargaPedidoDuplicado.PedidoEncaixado = false;

            if (cargaPedido.Pedido.ReentregaSolicitada)
            {
                cargaPedidoDuplicado.ReentregaSolicitada = true;
                cargaPedido.Pedido.ReentregaSolicitada = false;
                repPedido.Atualizar(cargaPedido.Pedido);
            }

            if (cargaPedidoDuplicado.ReentregaSolicitada && configuracaoTMS.NaoEmitirDocumentosEmCargasDeReentrega)
            {
                cargaPedidoDuplicado.PedidoSemNFe = true;
                //cargaPedidoDuplicado.CTeEmitidoNoEmbarcador = true;
                cargaPedidoDuplicado.CTesEmitidos = true;
            }

            if (cargaPedidoDuplicado.Expedidor != null && cargaPedidoDuplicado.Recebedor != null)
            {
                cargaPedidoDuplicado.Origem = cargaPedidoDuplicado.Expedidor.Localidade;
                cargaPedidoDuplicado.Expedidor = cargaPedidoDuplicado.Expedidor;
                cargaPedidoDuplicado.Destino = cargaPedidoDuplicado.Recebedor.Localidade;
                cargaPedidoDuplicado.Recebedor = cargaPedidoDuplicado.Recebedor;
                cargaPedidoDuplicado.TipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;
            }
            else if (cargaPedidoDuplicado.Expedidor != null)
            {
                cargaPedidoDuplicado.Origem = cargaPedidoDuplicado.Expedidor.Localidade;
                cargaPedidoDuplicado.Expedidor = cargaPedidoDuplicado.Expedidor;
                cargaPedidoDuplicado.TipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.ComExpedidor;
            }
            else if (cargaPedidoDuplicado.Recebedor != null)
            {
                cargaPedidoDuplicado.Origem = cargaPedidoDuplicado.Recebedor.Localidade;
                cargaPedidoDuplicado.Expedidor = cargaPedidoDuplicado.Recebedor;
                cargaPedidoDuplicado.TipoEmissaoCTeParticipantes = TipoEmissaoCTeParticipantes.ComRecebedor;
            }

            bool possuiCTe = false;
            bool possuiNFS = false;
            bool possuiNFSManual = false;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoIntramunicipal = null;
            Carga.CargaPedido servicoCargaPedido = new Carga.CargaPedido(_unitOfWork);

            servicoCargaPedido.VerificarQuaisDocumentosDeveEmitir(cargaDedicada, cargaPedidoDuplicado, cargaPedidoDuplicado.Origem, cargaPedidoDuplicado.Destino, tipoServicoMultisoftware, _unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoIntramunicipal, configuracaoTMS, out bool sempreDisponibilizarDocumentoNFSManual);

            cargaPedidoDuplicado.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;
            cargaPedidoDuplicado.PossuiCTe = possuiCTe;
            cargaPedidoDuplicado.PossuiNFS = possuiNFS;
            cargaPedidoDuplicado.PossuiNFSManual = possuiNFSManual;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            repositorioCargaPedido.Inserir(cargaPedidoDuplicado);

            Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repositorioCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProdutoTransbordo = cargaPedidoProduto.Clonar();

                cargaPedidoProdutoTransbordo.CargaPedido = cargaPedidoDuplicado;

                repositorioCargaPedidoProduto.Inserir(cargaPedidoProdutoTransbordo);
            }

            Carga.FilialEmissora servicoFilialEmissora = new Carga.FilialEmissora();
            servicoFilialEmissora.GerarCTesAnteriores(cargaPedido, cargaPedidoDuplicado, tipoServicoMultisoftware, _unitOfWork, configuracaoTMS);

            cargaPedidoDuplicado.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
            double.TryParse(cargaPedido.CargaOrigem.Empresa?.CNPJ_SemFormato ?? "", out double cnpjEmissor);
            cargaPedidoDuplicado.Tomador = repCliente.BuscarPorCPFCNPJ(cnpjEmissor);
            if (cargaPedidoDuplicado.Tomador == null)
            {
                Servicos.Log.TratarErro("Não foi localizado o tomador para a carga pedido" + cargaPedido.Codigo.ToString());
                throw new ServicoException("O emitente do CT-e anterior não está cadastrado como cliente na base Multisoftware (" + (cargaPedido.CargaOrigem.Empresa?.CNPJ_SemFormato ?? "Não informado") + ")");
            }

            if ((cargaPedidoDuplicado.TipoContratacaoCarga != TipoContratacaoCarga.Redespacho) && (cargaPedidoDuplicado.TipoContratacaoCarga != TipoContratacaoCarga.RedespachoIntermediario))
            {
                if ((cargaPedidoDuplicado.Expedidor != null) && (cargaPedidoDuplicado.Recebedor != null))
                {
                    if ((cargaPedidoDuplicado.Carga.GrupoPessoaPrincipal != null) && (cargaPedidoDuplicado.Carga.GrupoPessoaPrincipal.EmitirSempreComoRedespacho))
                        cargaPedidoDuplicado.TipoContratacaoCarga = TipoContratacaoCarga.Redespacho;
                    else
                        cargaPedidoDuplicado.TipoContratacaoCarga = TipoContratacaoCarga.RedespachoIntermediario;
                }
                else if ((cargaPedidoDuplicado.Expedidor != null) || (cargaPedidoDuplicado.Recebedor != null))
                    cargaPedidoDuplicado.TipoContratacaoCarga = TipoContratacaoCarga.Redespacho;
                else
                    cargaPedidoDuplicado.TipoContratacaoCarga = TipoContratacaoCarga.SubContratada;

                cargaPedidoDuplicado.CargaPedidoFilialEmissora = false;
            }



            repositorioCargaPedido.Atualizar(cargaPedidoDuplicado);
        }

        private void ValidarEncaixarPedidos(List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentosEncaixe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga in preAgrupamentosEncaixe)
                Carga.PedidoVinculado.ValidarPermitePedidoDeEncaixe(new List<int>() { preAgrupamentoCarga.CargaPedidoEncaixe.Codigo }, preAgrupamentoCarga.CnpjExpedidor.ToDouble(), _unitOfWork, tipoServicoMultisoftware, configuracaoTMS);
        }

        private void EncaixarPedidos(Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador agrupamento, List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentosEncaixe, List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentoCargas, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            if (preAgrupamentoCargas.Count > 0)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigoFetch(preAgrupamentoCargas.FirstOrDefault().Carga.Codigo);

                if (carga.CargaAgrupamento != null)
                    carga = carga.CargaAgrupamento;

                Carga.PedidoVinculado.CriarPedidoDeEncaixe(carga, preAgrupamentosEncaixe, _unitOfWork, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga);
            }
            else
                CriarCargaDedicada(agrupamento, preAgrupamentosEncaixe, configuracaoTMS, tipoServicoMultisoftware, null);
        }

        private void AjustarSequenciaEntrega(Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador agrupamento)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao repPreAgrupamentoCargaRoteirizacao = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao> preAgrupamentoCargaRoteirizacoes = repPreAgrupamentoCargaRoteirizacao.BuscaPorCodigoAgrupador(agrupamento.Codigo);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = agrupamento.Carga;
            if (preAgrupamentoCargaRoteirizacoes.Count > 0 && carga != null)
            {
                bool afetou = false;
                foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao preAgrupamentoCargaRoteirizacao in preAgrupamentoCargaRoteirizacoes)
                {
                    double.TryParse(preAgrupamentoCargaRoteirizacao.CNPJDestinatario, out double cnpj);
                    if (repCargaPedido.SetarOrdemEntrega(agrupamento.Carga?.Codigo ?? 0, preAgrupamentoCargaRoteirizacao.Sequencia, cnpj, preAgrupamentoCargaRoteirizacao.DataPrevisaoChegada, preAgrupamentoCargaRoteirizacao.InicioJanelaDescarga, preAgrupamentoCargaRoteirizacao.FimJanelaDescarga) > 0)
                        afetou = true;
                }
                if (afetou)
                {

                    if (carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Concluido)
                        carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;

                    carga.OrdemRoteirizacaoDefinida = true;
                    repCarga.Atualizar(carga);
                }
            }

        }

        private void GerarEncaixesPendentes(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorioPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repositorioPreAgrupamentoCargaAgrupador = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> agrupamentos = repositorioPreAgrupamentoCargaAgrupador.BucarPorSituacaoEncaixePendente(5);

            foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador agrupamento in agrupamentos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentoCargas = repositorioPreAgrupamentoCarga.BuscarCargasPorCodigoAgrupador(agrupamento.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentosEncaixe = repositorioPreAgrupamentoCarga.BuscarCargasPedidoEncaixePorCodigoAgrupador(agrupamento.Codigo);

                try
                {
                    _unitOfWork.Start();

                    if (preAgrupamentosEncaixe.Any(obj => obj.CargaPedidoEncaixe == null))
                    {
                        agrupamento.PossuiPreCargas = true;
                        agrupamento.Situacao = SituacaoPreAgrupamentoCarga.ProblemaCarregamento;
                        agrupamento.Pendencia = "não existem pedidos para encaixe compativeis com as informações que vieram na integração.";
                    }
                    else
                    {
                        EncaixarPedidos(agrupamento, preAgrupamentosEncaixe, preAgrupamentoCargas, configuracaoTMS, tipoServicoMultisoftware, configuracaoGeralCarga);

                        AjustarSequenciaEntrega(agrupamento);
                        agrupamento.Situacao = SituacaoPreAgrupamentoCarga.Carregado;

                        repositorioPreAgrupamentoCargaAgrupador.Atualizar(agrupamento);
                    }

                    _unitOfWork.CommitChanges();
                }
                catch (ServicoException excecao)
                {
                    _unitOfWork.Rollback();

                    agrupamento.PossuiPreCargas = true;
                    agrupamento.Situacao = SituacaoPreAgrupamentoCarga.ProblemaCarregamento;
                    agrupamento.Pendencia = Utilidades.String.Left(excecao.Message, 500);

                    repositorioPreAgrupamentoCargaAgrupador.Atualizar(agrupamento);
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    agrupamento.PossuiPreCargas = true;
                    agrupamento.Situacao = SituacaoPreAgrupamentoCarga.ProblemaCarregamento;
                    agrupamento.Pendencia = "Ocorreu uma falha ao gerar o carregamento";

                    repositorioPreAgrupamentoCargaAgrupador.Atualizar(agrupamento);
                    Servicos.Log.TratarErro(ex);
                    throw;
                }
            }
        }

        private void GerarCarregamentoPorAgrupamento(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorioPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repositorioPreAgrupamentoCargaAgrupador = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> agrupamentos = repositorioPreAgrupamentoCargaAgrupador.BucarPorSituacaoAguardandoCarregamento(5);

            foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador agrupamento in agrupamentos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentoCargas = repositorioPreAgrupamentoCarga.BuscarCargasPorCodigoAgrupador(agrupamento.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentosEncaixe = repositorioPreAgrupamentoCarga.BuscarCargasPedidoEncaixePorCodigoAgrupador(agrupamento.Codigo);

                try
                {
                    _unitOfWork.Start();

                    if (!agrupamento.PossuiPreCargas)
                        EncaixarPedidos(agrupamento, preAgrupamentosEncaixe, preAgrupamentoCargas, configuracaoTMS, tipoServicoMultisoftware, configuracaoGeralCarga);

                    AgruparCargas(agrupamento, preAgrupamentoCargas, tipoServicoMultisoftware, configuracaoTMS, configuracaoGeralCarga, configuracaoPedido);

                    if (!agrupamento.PossuiPreCargas || preAgrupamentosEncaixe.Count == 0)
                    {
                        AjustarSequenciaEntrega(agrupamento);
                        agrupamento.Situacao = SituacaoPreAgrupamentoCarga.Carregado;
                    }
                    else
                        agrupamento.Situacao = SituacaoPreAgrupamentoCarga.AguardandoCargasPararEncaixe;

                    repositorioPreAgrupamentoCargaAgrupador.Atualizar(agrupamento);

                    _unitOfWork.CommitChanges();
                }
                catch (ServicoException excecao)
                {
                    _unitOfWork.Rollback();
                    Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador preAgrupamentoCargaAgrupador = repositorioPreAgrupamentoCargaAgrupador.BuscarPorCodigo(agrupamento.Codigo);

                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.ProblemaCarregamento;
                    preAgrupamentoCargaAgrupador.Pendencia = Utilidades.String.Left(excecao.Message, 500);

                    repositorioPreAgrupamentoCargaAgrupador.Atualizar(preAgrupamentoCargaAgrupador);
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador preAgrupamentoCargaAgrupador = repositorioPreAgrupamentoCargaAgrupador.BuscarPorCodigo(agrupamento.Codigo);

                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.ProblemaCarregamento;
                    preAgrupamentoCargaAgrupador.Pendencia = "Ocorreu uma falha ao gerar o carregamento";

                    repositorioPreAgrupamentoCargaAgrupador.Atualizar(preAgrupamentoCargaAgrupador);

                    throw;
                }
            }
        }

        public static void RemoverVinculosCargaCancelada(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec servicoIntegracaoOrtec = new Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec(unitOfWork);

            if (servicoIntegracaoOrtec.IsPossuiIntegracaoOrtec())
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repPreAgrupamentoCargaAgrupador = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> preAgrupamentosAgrupador = new List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>();
                if (carga.CargaAgrupada)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = repCarga.BuscarCargasOriginais(carga.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaorigem in cargasOrigem)
                        LiberarAgrupamentoCarga(cargaorigem, preAgrupamentosAgrupador, unitOfWork);
                }
                else
                    LiberarAgrupamentoCarga(carga, preAgrupamentosAgrupador, unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador preAgrupamentoCargaAgrupador in preAgrupamentosAgrupador)
                {
                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.SemCarga;
                    preAgrupamentoCargaAgrupador.Pendencia = "";
                    repPreAgrupamentoCargaAgrupador.Atualizar(preAgrupamentoCargaAgrupador);
                }
            }
        }

        public static void LiberarAgrupamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> preAgrupamentosAgrupador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentosCarga = repPreAgrupamentoCarga.BuscarListaPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga in preAgrupamentosCarga)
            {
                preAgrupamentoCarga.Carga = null;
                preAgrupamentoCarga.CargaPedidoEncaixe = null;
                if ((preAgrupamentoCarga.CargaRedespacho?.Codigo ?? 0) == carga.Codigo)
                    preAgrupamentoCarga.CargaRedespacho = null;

                repPreAgrupamentoCarga.Atualizar(preAgrupamentoCarga);

                if (!preAgrupamentosAgrupador.Contains(preAgrupamentoCarga.Agrupador))
                    preAgrupamentosAgrupador.Add(preAgrupamentoCarga.Agrupador);
            }
        }

        private void GerarRedespachoPorAgrupamento(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Redespacho repositorioRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repositorioPreAgrupamentoCargaAgrupador = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal repositorioPreAgrupamentoNotaFiscal = new Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> agrupamentos = repositorioPreAgrupamentoCargaAgrupador.BucarPorSituacaoAguardandoRedespacho(5);

            foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador agrupamento in agrupamentos)
            {
                try
                {
                    List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentosEncaixe = repPreAgrupamentoCarga.BuscarCargasPedidoEncaixePorCodigoAgrupador(agrupamento.Codigo);

                    if (preAgrupamentosEncaixe.Count > 0)
                        ValidarEncaixarPedidos(preAgrupamentosEncaixe, configuracaoTMS, tipoServicoMultisoftware);

                    _unitOfWork.Start();

                    List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> listaNotaFiscalAgrupador = repositorioPreAgrupamentoNotaFiscal.BuscarPorAgrupador(agrupamento.Codigo);
                    List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> preAgrupamentosCarga = repPreAgrupamentoCarga.BuscaPorCodigoAgrupador(agrupamento.Codigo);

                    List<Dominio.Entidades.Empresa> transportadores = (from obj in preAgrupamentosCarga where obj.Carga != null && !obj.PedidoEncaixe && obj.Carga.Empresa != null select obj.Carga.Empresa).Distinct().ToList();

                    foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga in preAgrupamentosCarga)
                    {
                        if ((!string.IsNullOrWhiteSpace(preAgrupamentoCarga.CnpjExpedidor) || preAgrupamentoCarga.PedidoReentrega) && preAgrupamentoCarga.Carga == null)
                        {
                            List<int> listaNumeroNotaFiscalPreAgrupamentoCarga = (
                                from notaFiscalAgrupador in listaNotaFiscalAgrupador
                                where notaFiscalAgrupador.PreAgrupamentoCarga.Codigo == preAgrupamentoCarga.Codigo
                                select notaFiscalAgrupador.NumeroNota.ToInt()
                            ).ToList();

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioPedidoXMLNotaFiscal.BuscarPorCargaENumerosNota(preAgrupamentoCarga.CargaRedespacho.Codigo, listaNumeroNotaFiscalPreAgrupamentoCarga);

                            if (cargaPedidos.Count == 0)
                                throw new ServicoException("Não foi possível encontrar os pedidos da carga para gerar redespacho.");

                            double cnpjExpedidor = 0;//preAgrupamentoCarga.CnpjExpedidor.ToDouble();

                            double.TryParse(preAgrupamentoCarga.CnpjExpedidor, out cnpjExpedidor);

                            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = preAgrupamentoCarga.CargaRedespacho.TipoOperacao;
                            if (preAgrupamentoCarga.CargaRedespacho?.Filial?.TipoOperacaoRedespacho != null)
                                tipoOperacao = preAgrupamentoCarga.CargaRedespacho?.Filial?.TipoOperacaoRedespacho;

                            Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho = new Dominio.Entidades.Embarcador.Cargas.Redespacho()
                            {
                                DataRedespacho = DateTime.Now,
                                Carga = preAgrupamentoCarga.CargaRedespacho,
                                NumeroRedespacho = repositorioRedespacho.BuscarProximoCodigo(),
                                TipoOperacao = tipoOperacao,
                                Expedidor = ObterCliente(cnpjExpedidor)
                            };

                            if (redespacho.Expedidor == null && !preAgrupamentoCarga.PedidoReentrega)
                                throw new ServicoException($"Não é possível gerar o redespacho pois não existe um cliente cadastrado com o cnpj do expedidor informado ({cnpjExpedidor}).");

                            repositorioRedespacho.Inserir(redespacho);

                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                            {
                                if (
                                    cargaPedido.CargaRedespacho != null &&
                                    (cargaPedido.CargaRedespacho.Expedidor != null || preAgrupamentoCarga.PedidoReentrega) &&
                                    cargaPedido.CargaRedespacho.CargaGerada.SituacaoCarga != SituacaoCarga.Cancelada &&
                                    cargaPedido.CargaRedespacho.CargaGerada.SituacaoCarga != SituacaoCarga.Anulada &&
                                    (preAgrupamentoCarga.PedidoReentrega || cargaPedido.CargaRedespacho.Expedidor?.CPF_CNPJ == redespacho.Expedidor?.CPF_CNPJ) &&
                                    !cargaPedido.Pedido.ReentregaSolicitada &&
                                    !(redespacho.Carga.TipoOperacao?.PermitirGerarRecorrenciaRedespacho ?? false)
                                )
                                {
                                    string strRedespacho = "o redespacho";
                                    string motivo = $" pois ele já foi redespachado pelo expedidor {cargaPedido.CargaRedespacho?.Expedidor?.Descricao ?? ""} no redespacho de número {cargaPedido.CargaRedespacho?.NumeroRedespacho ?? 0}. ";
                                    if (preAgrupamentoCarga.PedidoReentrega)
                                    {
                                        strRedespacho = "a reentrega";
                                        if (!cargaPedido.Pedido.ReentregaSolicitada)
                                            motivo = " pois não foi solicitada a reentrega do pedido " + cargaPedido.Pedido.NumeroPedidoEmbarcador + " no MultiEmbarcador (A operação da Danone deve fazer isso corretamente no MultiEmbarcador para poder solicitar a reentrega via Ortec depois). ";
                                        else if (!(redespacho.Carga.TipoOperacao?.PermitirGerarRecorrenciaRedespacho ?? false))
                                            motivo = " pois o tipo de operação " + (redespacho.Carga?.TipoOperacao?.Descricao ?? "") + " não permite gerar redespacho";
                                    }
                                    string mensagem = "Não é possível gerar " + strRedespacho + " do pedido " + motivo;
                                    throw new ServicoException(mensagem);
                                }

                                if (cargaPedido.CargaPedidoProximoTrecho != null)
                                {
                                    if (cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga != SituacaoCarga.Cancelada && cargaPedido.CargaPedidoProximoTrecho.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                                        throw new ServicoException($"Não é possível gerar o redespacho do pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} pois ele já foi possui um segundo trecho gerado para o expedidor {(cargaPedido.CargaPedidoProximoTrecho.Expedidor?.Descricao ?? "")}.");

                                    if (cargaPedido.PendenteGerarCargaDistribuidor)
                                        throw new ServicoException($"Não é possível gerar o redespacho do pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} pois no momento já está sendo gerada uma carga de segundo trecho para ele. ");
                                }

                                cargaPedido.CargaRedespacho = redespacho;

                                repositorioCargaPedido.Atualizar(cargaPedido);
                            }
                            redespacho.CargaGerada = Carga.CargaDistribuidor.GerarCargaProximoTrecho(redespacho.Carga, redespacho.TipoOperacao, 0m, true, redespacho.Expedidor, cargaPedidos, agrupamento.Empresa, configuracaoTMS, false, null, agrupamento.Veiculo, tipoServicoMultisoftware, _unitOfWork);

                            if (transportadores.Count > 0 && redespacho.CargaGerada.Empresa != null)
                            {
                                string raiz = transportadores.FirstOrDefault().CNPJ_SemFormato.Remove(8, 6);
                                if (!redespacho.CargaGerada.Empresa.CNPJ_SemFormato.Contains(raiz))
                                {
                                    throw new ServicoException($"Não é possível agrupar cargas de diferentes transportadores.");
                                }
                            }

                            preAgrupamentoCarga.Carga = redespacho.CargaGerada;
                            if (preAgrupamentoCarga.DataPrevisaoInicioViagem.HasValue)
                            {
                                redespacho.CargaGerada.DataInicioViagemPrevista = preAgrupamentoCarga.DataPrevisaoInicioViagem;
                                repCarga.Atualizar(redespacho.CargaGerada);
                            }

                            repositorioRedespacho.Atualizar(redespacho);
                        }
                    }

                    agrupamento.Situacao = SituacaoPreAgrupamentoCarga.AguardandoCarregamento;

                    repositorioPreAgrupamentoCargaAgrupador.Atualizar(agrupamento);

                    _unitOfWork.CommitChanges();
                }
                catch (ServicoException excecao)
                {
                    _unitOfWork.Rollback();

                    agrupamento.Situacao = SituacaoPreAgrupamentoCarga.ProblemaCarregamento;
                    agrupamento.Pendencia = Utilidades.String.Left(excecao.Message, 500);

                    repositorioPreAgrupamentoCargaAgrupador.Atualizar(agrupamento);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    _unitOfWork.Rollback();

                    agrupamento.Situacao = SituacaoPreAgrupamentoCarga.ProblemaCarregamento;
                    agrupamento.Pendencia = "Ocorreu uma falha ao gerar o redespacho";

                    repositorioPreAgrupamentoCargaAgrupador.Atualizar(agrupamento);
                }
            }
        }

        private void SalvarArquivoIntegracao(Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.Agrupador agrupador)
        {
            try
            {
                string caminhoArquivoIntegracao = ObterCaminhoArquivoIntegracao();
                string nomeArquivoIntegracao = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivoIntegracao, $"agrupador_{agrupador.CodigoAgrupador}.xml");
                JsonSerializerSettings configuracoesDeserializacao = new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore };
                string agrupadorJson = JsonConvert.SerializeObject(agrupador, configuracoesDeserializacao);
                System.Xml.XmlDocument agrupadorXml = JsonConvert.DeserializeXmlNode(agrupadorJson, "Agrupador");

                Utilidades.IO.FileStorageService.Storage.WriteAllText(nomeArquivoIntegracao, agrupadorXml.OuterXml);
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Erro ao salvar o arquivo de integração da Ortec (agrupador {agrupador.CodigoAgrupador}):" + excecao.Message);
            }
        }

        private void VincularCargaAoAgrupamento(Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga, Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador preAgrupamentoCargaAgrupador)
        {
            if (!string.IsNullOrWhiteSpace(preAgrupamentoCarga.CodigoViagem))
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigoEmbarcador(preAgrupamentoCarga.CodigoViagem, false);

                if (carga != null)
                {
                    Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorioPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);

                    if (string.IsNullOrWhiteSpace(preAgrupamentoCarga.CnpjExpedidor) && !preAgrupamentoCarga.PedidoReentrega)
                    {
                        preAgrupamentoCarga.Carga = carga;
                        preAgrupamentoCargaAgrupador.PossuiPreCargas = carga.CargaDePreCarga;

                        if (repositorioPreAgrupamentoCarga.VerificarSeExisteCargaDePreCargaPorAgrupamento(preAgrupamentoCargaAgrupador.Codigo))
                            preAgrupamentoCargaAgrupador.PossuiPreCargas = true;
                        else
                            preAgrupamentoCargaAgrupador.PossuiPreCargas = carga.CargaDePreCarga;

                        repositorioPreAgrupamentoCarga.Atualizar(preAgrupamentoCarga);
                    }
                    else if (carga.TipoOperacao?.PermitirGerarRedespacho ?? true)
                    {
                        if (!carga.CargaDePreCarga)
                        {
                            preAgrupamentoCarga.CargaRedespacho = carga;
                            repositorioPreAgrupamentoCarga.Atualizar(preAgrupamentoCarga);
                        }
                    }

                    if (preAgrupamentoCarga.DataPrevisaoInicioViagem.HasValue)
                    {
                        carga.DataInicioViagemPrevista = preAgrupamentoCarga.DataPrevisaoInicioViagem;
                        repositorioCarga.Atualizar(carga);
                    }
                }
            }
            else
            {
                preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.ProblemaCarregamento;
                preAgrupamentoCargaAgrupador.Pendencia = "Para pedidos que não são de encaixe é obrigatório informar o número da carga.";
            }
        }

        private bool informarCargaPedidoAgrupamentoCarga(ref Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if ((cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgIntegracao ||
                 cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos ||
                 cargaPedido.Carga.SituacaoCarga == SituacaoCarga.EmTransporte ||
                 cargaPedido.Carga.SituacaoCarga == SituacaoCarga.Encerrada)
                 && (cargaPedido.Carga.Empresa?.PermiteEmitirSubcontratacao ?? false))
            {
                preAgrupamentoCarga.Carga = cargaPedido.Carga;
                preAgrupamentoCarga.CargaPedidoEncaixe = cargaPedido;

                if (preAgrupamentoCarga.DataCarregamento.HasValue || preAgrupamentoCarga.DataPrevisaoEntrega.HasValue)
                {
                    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                    preAgrupamentoCarga.CargaPedidoEncaixe.Pedido.DataCarregamentoPedido = preAgrupamentoCarga.DataCarregamento;
                    preAgrupamentoCarga.CargaPedidoEncaixe.PrevisaoEntrega = preAgrupamentoCarga.DataPrevisaoEntrega;
                    repPedido.Atualizar(preAgrupamentoCarga.CargaPedidoEncaixe.Pedido);
                }

                if (preAgrupamentoCarga.DataPrevisaoInicioViagem.HasValue)
                {
                    cargaPedido.Carga.DataInicioViagemPrevista = preAgrupamentoCarga.DataPrevisaoInicioViagem;
                    repositorioCarga.Atualizar(cargaPedido.Carga);
                }

                return true;
            }
            return false;
        }

        private void VincularCargaAoAgrupamentoPorPedidoEncaixe(Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga, List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> preAgrupamentoNotaFiscaisAgrupador)
        {
            List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> preAgrupamentoNotaFiscais = (from o in preAgrupamentoNotaFiscaisAgrupador where o.PreAgrupamentoCarga.Codigo == preAgrupamentoCarga.Codigo select o).ToList();
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorioPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            if (preAgrupamentoCarga.CodigoIntegracao > 0)
            {
                Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao repSeparacaoPedidoIntegracao = new Repositorio.Embarcador.WMS.SeparacaoPedidoIntegracao(_unitOfWork);
                Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoIntegracao separacaoPedidoIntegracao = repSeparacaoPedidoIntegracao.BuscarPorCodigo(preAgrupamentoCarga.CodigoIntegracao);
                if (separacaoPedidoIntegracao != null)
                {
                    if (separacaoPedidoIntegracao.SeparacaoPedidoPedido != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarNaoEncaixadaPorPedido(separacaoPedidoIntegracao.SeparacaoPedidoPedido.Pedido.Codigo);
                        bool encontrou = informarCargaPedidoAgrupamentoCarga(ref preAgrupamentoCarga, cargaPedido);
                        if (encontrou)
                            repositorioPreAgrupamentoCarga.Atualizar(preAgrupamentoCarga);
                    }
                    else
                    {
                        Repositorio.Embarcador.WMS.SeparacaoPedidoPedido repSeparacaoPedidoPedido = new Repositorio.Embarcador.WMS.SeparacaoPedidoPedido(_unitOfWork);
                        List<Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido> separacaoPedidoPedidos = repSeparacaoPedidoPedido.BuscarPorSeparacaoPedido(separacaoPedidoIntegracao.SeparacaoPedido.Codigo);
                        foreach (Dominio.Entidades.Embarcador.WMS.SeparacaoPedidoPedido separacaoPedidoPedido in separacaoPedidoPedidos)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarNaoEncaixadaPorPedido(separacaoPedidoPedido.Pedido.Codigo);
                            if (preAgrupamentoCarga.CargaRedespacho == null)
                            {
                                informarCargaPedidoAgrupamentoCarga(ref preAgrupamentoCarga, cargaPedido);
                                repositorioPreAgrupamentoCarga.Atualizar(preAgrupamentoCarga);
                            }
                            else
                            {
                                Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCargaClone = preAgrupamentoCarga.Clonar();
                                preAgrupamentoCargaClone.Carga = null;
                                preAgrupamentoCargaClone.CargaPedidoEncaixe = null;
                                preAgrupamentoCargaClone.CodigoIntegracao = 0;
                                informarCargaPedidoAgrupamentoCarga(ref preAgrupamentoCargaClone, cargaPedido);
                                repositorioPreAgrupamentoCarga.Inserir(preAgrupamentoCargaClone);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal preAgrupamentoNota in preAgrupamentoNotaFiscais)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorNumeroEEmitente(preAgrupamentoNota.NumeroNota.ToInt(), preAgrupamentoNota.CnpjEmitente.ToDouble());

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in listaPedidoXMLNotaFiscal)
                    {
                        if (pedidoXMLNotaFiscal != null)
                        {
                            bool encontrou = informarCargaPedidoAgrupamentoCarga(ref preAgrupamentoCarga, pedidoXMLNotaFiscal.CargaPedido);
                            if (encontrou)
                            {
                                if (string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.NumeroPedidoEmbarcador) && !string.IsNullOrWhiteSpace(preAgrupamentoNota.NumeroPedido))
                                {
                                    pedidoXMLNotaFiscal.XMLNotaFiscal.NumeroPedidoEmbarcador = preAgrupamentoNota.NumeroPedido;
                                    repXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal.XMLNotaFiscal);
                                }

                                repositorioPreAgrupamentoCarga.Atualizar(preAgrupamentoCarga);
                                break;
                            }
                        }
                    }
                }
            }


        }

        public void EnviarIntegracaoEntregaCarga(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao integracaoEntrega, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao repCargaEntregaIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            if (string.IsNullOrWhiteSpace(integracao.URLOrtec) || !integracao.IntegrarEntregaOrtec)
            {
                integracaoEntrega.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoEntrega.ProblemaIntegracao = "Não foi configurada a integração com a Ortec para entregas, por favor verifique.";
                return;
            }

            AppOrtec.ApplicationIntegrationServiceClient applicationIntegrationServiceClient = ObterClient(integracao);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Ortec.shift shift = ObterObjetoIntegracaoEntregas(integracaoEntrega);

            XmlSerializerNamespaces emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            System.Xml.Serialization.XmlSerializer xmlSerializerSend = new System.Xml.Serialization.XmlSerializer(shift.GetType());

            string xmlEnvio = "";
            var settings = new XmlWriterSettings();

            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                xmlSerializerSend.Serialize(writer, shift, emptyNamespaces);
                xmlEnvio = stream.ToString();
            }
            //xmlEnvio = "<![CDATA[" + xmlEnvio + "]]>";

            integracaoEntrega.DataIntegracao = DateTime.Now;
            integracaoEntrega.NumeroTentativas += 1;

            InspectorBehavior inspector = new InspectorBehavior();

            applicationIntegrationServiceClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem = null;
            bool sucesso = false;

            try
            {
                string retorno = applicationIntegrationServiceClient.SendMessage(xmlEnvio, "MS_ReceiveActuals");

                dynamic ret = DynamicXml.Parse(retorno);
                if (ret.success.ToLower() == "true")
                {
                    sucesso = true;
                    mensagem = "Integração realizada com sucesso.";
                }
                else
                {
                    sucesso = false;
                    mensagem = ret.answer;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falha ao realizar a integração.";
            }

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracaoArquivo();

            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
            arquivoIntegracao.Data = integracaoEntrega.DataIntegracao;
            arquivoIntegracao.Mensagem = mensagem;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            integracaoEntrega.ArquivosTransacao.Add(arquivoIntegracao);
            integracaoEntrega.ProblemaIntegracao = mensagem;

            if (!sucesso)
                integracaoEntrega.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            else
                integracaoEntrega.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

            repCargaEntregaIntegracao.Atualizar(integracaoEntrega);
        }

        private AppOrtec.ApplicationIntegrationServiceClient ObterClient(Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(integracao.URLOrtec);
            System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding();
            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (integracao.URLOrtec.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.SecurityMode.Transport;

            AppOrtec.ApplicationIntegrationServiceClient client = new AppOrtec.ApplicationIntegrationServiceClient(binding, endpointAddress);

            client.ClientCredentials.UserName.Password = integracao.SenhaOrtec;
            client.ClientCredentials.UserName.UserName = integracao.UsuarioOrtec;

            return client;
        }

        #endregion

        #region Métodos Privados de Consulta

        private Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ObterArquivoIntegracao(int codigoAgrupador)
        {
            string nomeArquivo = $"agrupador_{codigoAgrupador}.xml";
            Repositorio.Embarcador.Integracao.ArquivoIntegracao repositorioArquivoIntegracao = new Repositorio.Embarcador.Integracao.ArquivoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao = repositorioArquivoIntegracao.BuscarPorNomeArquivo(nomeArquivo);

            if (arquivoIntegracao == null)
                arquivoIntegracao = ArquivoIntegracao.SalvarArquivoIntegracao(nomeArquivo, _unitOfWork);

            return arquivoIntegracao;
        }

        private string ObterCaminhoArquivoIntegracao()
        {
            string caminhoArquivoIntegracao = Utilidades.IO.FileStorageService.Storage.Combine(ArquivoIntegracao.ObterCaminhoArquivoIntegracao(), "Ortec");

            return caminhoArquivoIntegracao;
        }

        private Dominio.Entidades.Cliente ObterCliente(double cpfCnjp)
        {
            if (cpfCnjp == 0d)
                return null;

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

            return repositorioCliente.BuscarPorCPFCNPJ(cpfCnjp);
        }

        private List<Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.PedidoAgrupamento> ObterListaPedidoAgrupamentoSumarizado(Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.Agrupador agrupador)
        {
            List<Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.PedidoAgrupamento> listaPedidoAgrupamentoSumarizado = new List<Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.PedidoAgrupamento>();

            foreach (Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.PedidoAgrupamento pedidoAgrupamento in agrupador.Pedidos.ToList())
            {
                if (pedidoAgrupamento.NotasFiscal == null)
                    pedidoAgrupamento.NotasFiscal = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>(); //throw new ServicoException("É obrigatório informar as notas fiscais.");

                Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.PedidoAgrupamento pedidoAgrupamentoSumarizado = (
                    from o in listaPedidoAgrupamentoSumarizado
                    where (
                        o.NumeroCarga.Trim() == pedidoAgrupamento.NumeroCarga.Trim() &&
                        o.Expedidor?.CPFCNPJ.Trim() == pedidoAgrupamento.Expedidor?.CPFCNPJ.Trim() &&
                        o.Recebedor?.CPFCNPJ.Trim() == pedidoAgrupamento.Recebedor?.CPFCNPJ.Trim() &&
                        !o.PedidoDeEncaixe &&
                        !pedidoAgrupamento.PedidoDeEncaixe
                    )
                    select o
                ).FirstOrDefault();

                foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nfe in pedidoAgrupamento.NotasFiscal)
                {
                    nfe.DataPrevisao = pedidoAgrupamento.DataPrevisaoEntrega;
                    nfe.NumeroPedido = pedidoAgrupamento.NumeroPedido;
                }

                if (pedidoAgrupamentoSumarizado == null)
                    listaPedidoAgrupamentoSumarizado.Add(pedidoAgrupamento);
                else
                    pedidoAgrupamentoSumarizado.NotasFiscal.AddRange(pedidoAgrupamento.NotasFiscal);
            }

            return listaPedidoAgrupamentoSumarizado;
        }

        private Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga ObterPreAgrupamentoCarga(int codigoPreAgrupamentoCarga)
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorioPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga = repositorioPreAgrupamentoCarga.BuscarPorCodigo(codigoPreAgrupamentoCarga, auditavel: false);

            if (preAgrupamentoCarga == null)
                throw new ServicoException("Pré agrupamento de carga não encontrado.");

            return preAgrupamentoCarga;
        }

        #endregion

        #region Métodos Públicos

        public int AdicionarPreAgrupamentoCarga(Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.Agrupador agrupador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (agrupador == null)
                throw new ServicoException("Favor não enviar um Agrupador nulo");

            int codigoAgrupamento = agrupador.CodigoAgrupador.ToInt();

            if (codigoAgrupamento <= 0)
                throw new ServicoException($"O código do agrupamento ({agrupador.CodigoAgrupador}) informado é inválido");

            if ((agrupador.Pedidos == null) || (agrupador.Pedidos.Count == 0))
                throw new ServicoException($"É obrigatório informar ao menos um pedido no agrupamento ({codigoAgrupamento})");

            try
            {
                _unitOfWork.Start();

                Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repositorioAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador preAgrupamentoCargaAgrupador = repositorioAgrupamentoCarga.BuscarPorCodigoAgrupamento(codigoAgrupamento);
                Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorioPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);
                Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal repositorioPreAgrupamentoNotaFiscal = new Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal(_unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

                if (preAgrupamentoCargaAgrupador == null)
                    preAgrupamentoCargaAgrupador = new Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador()
                    {
                        CodigoAgrupamento = codigoAgrupamento
                    };
                else
                {
                    if (!preAgrupamentoCargaAgrupador.Situacao.IsPermitirAlterar())
                        throw new ServicoException($"Não é possível alterar o agrupamento ({codigoAgrupamento}) pois as viagens do mesmo já estão agrupadas e fechadas, se necessário cancele a carga no portal da multisoftware e refaça todo o processo.");

                    repositorioPreAgrupamentoNotaFiscal.DeletarPorAgrupamento(preAgrupamentoCargaAgrupador.Codigo);
                    repositorioPreAgrupamentoCarga.DeletarPorAgrupamento(preAgrupamentoCargaAgrupador.Codigo);
                }

                preAgrupamentoCargaAgrupador.ArquivoIntegracao = ObterArquivoIntegracao(codigoAgrupamento);
                preAgrupamentoCargaAgrupador.DataCriacao = DateTime.Now;
                preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.AguardandoProcessamento;
                preAgrupamentoCargaAgrupador.Pendencia = "";
                if (agrupador.Transportadora != null && !string.IsNullOrEmpty(agrupador.Transportadora.CNPJ))
                {
                    preAgrupamentoCargaAgrupador.Empresa = repEmpresa.BuscarPorCNPJ(agrupador.Transportadora.CNPJ);
                    if (preAgrupamentoCargaAgrupador.Empresa == null)
                        throw new ServicoException($"Não foi localizado uma transportadora cadastrada na base multisoftware para o CNPJ {agrupador.Transportadora.CNPJ}.");

                    if (agrupador.Veiculo != null && !string.IsNullOrEmpty(agrupador.Veiculo.Placa))
                        preAgrupamentoCargaAgrupador.Veiculo = repVeiculo.BuscarPorPlacaVarrendoFiliais(preAgrupamentoCargaAgrupador.Empresa.Codigo, agrupador.Veiculo.Placa);

                }


                if (preAgrupamentoCargaAgrupador.Codigo == 0)
                {
                    repositorioAgrupamentoCarga.Inserir(preAgrupamentoCargaAgrupador);
                    Auditoria.Auditoria.Auditar(auditado, preAgrupamentoCargaAgrupador, "Criou o Agrupamento.", _unitOfWork);
                }
                else
                {
                    Auditoria.Auditoria.Auditar(auditado, preAgrupamentoCargaAgrupador, "Atualizou o Agrupamento.", _unitOfWork);
                    repositorioAgrupamentoCarga.Atualizar(preAgrupamentoCargaAgrupador);
                }

                List<Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.PedidoAgrupamento> pedidoAgrupamentoSumarizados = ObterListaPedidoAgrupamentoSumarizado(agrupador);

                if (agrupador.Roteirizacao != null)
                {
                    Repositorio.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao repPreAgrupamentoCargaRoteirizacao = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao(_unitOfWork);
                    foreach (Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.Roteirizacao roteirizacao in agrupador.Roteirizacao)
                    {
                        Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao preAgrupamentoCargaRoteirizacao = new Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao();
                        preAgrupamentoCargaRoteirizacao.Agrupador = preAgrupamentoCargaAgrupador;

                        if (string.IsNullOrWhiteSpace(roteirizacao.Destinatario?.CPFCNPJ))
                            throw new ServicoException($"É obrigatório informar o destinatário da roteirização");

                        preAgrupamentoCargaRoteirizacao.CNPJDestinatario = roteirizacao.Destinatario.CPFCNPJ.Trim();
                        if (!string.IsNullOrWhiteSpace(roteirizacao.DataInicioDescarregamento))
                        {
                            if (!DateTime.TryParseExact(roteirizacao.DataInicioDescarregamento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataDescarregamento))
                                throw new ServicoException($"A data de descarga não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");

                            preAgrupamentoCargaRoteirizacao.DataInicioDescarregamento = dataDescarregamento;
                        }

                        if (!string.IsNullOrWhiteSpace(roteirizacao.DataInicioDescarregamento))
                        {
                            if (!DateTime.TryParseExact(roteirizacao.DataPrevisaoChegada, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisaoChegada))
                                throw new ServicoException($"A data de previsão de chegada não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");

                            preAgrupamentoCargaRoteirizacao.DataPrevisaoChegada = dataPrevisaoChegada;
                        }

                        preAgrupamentoCargaRoteirizacao.IdEntrega = roteirizacao.IdEntrega;
                        preAgrupamentoCargaRoteirizacao.Sequencia = roteirizacao.Sequencia;
                        preAgrupamentoCargaRoteirizacao.TempoDescarregamentoMinutos = roteirizacao.TempoDescarregamentoMinutos;

                        if (!string.IsNullOrWhiteSpace(roteirizacao.JanelaDeRecebimento) && roteirizacao.JanelaDeRecebimento.Contains("-") && preAgrupamentoCargaRoteirizacao.DataPrevisaoChegada.HasValue)
                        {
                            //converter objeto para datas inicio e fim.
                            TimeSpan timeInicio, timeFim;
                            string[] horarios = roteirizacao.JanelaDeRecebimento.Split('-');

                            if (!TimeSpan.TryParseExact(horarios[0], "hh\\:mm", CultureInfo.InvariantCulture, out timeInicio) || !TimeSpan.TryParseExact(horarios[1], "hh\\:mm", CultureInfo.InvariantCulture, out timeFim))
                                throw new ServicoException($"Horarios de Janela de Recebimento não esta em um formato correto (HH:mm)");

                            preAgrupamentoCargaRoteirizacao.InicioJanelaDescarga = preAgrupamentoCargaRoteirizacao.DataPrevisaoChegada.Value + timeInicio;
                            preAgrupamentoCargaRoteirizacao.FimJanelaDescarga = preAgrupamentoCargaRoteirizacao.DataPrevisaoChegada.Value + timeFim;
                        }

                        repPreAgrupamentoCargaRoteirizacao.Inserir(preAgrupamentoCargaRoteirizacao);
                    }
                }

                foreach (Dominio.ObjetosDeValor.WebService.Carga.Agrupamento.PedidoAgrupamento pedidoAgrupamento in pedidoAgrupamentoSumarizados)
                {
                    Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga = new Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga()
                    {
                        CodigoIntegracao = pedidoAgrupamento.IDIntegracao,
                        Agrupador = preAgrupamentoCargaAgrupador,
                        CnpjEmitente = pedidoAgrupamento.NotasFiscal.FirstOrDefault()?.Emitente?.CPFCNPJ.Trim() ?? "",
                        CodigoViagem = pedidoAgrupamento.NumeroCarga.Trim(),
                        NumeroNota = Utilidades.String.Left(string.Join(", ", (from obj in pedidoAgrupamento.NotasFiscal select obj.Numero).ToList()), 1000),
                        TipoViagem = pedidoAgrupamento.PedidoDeEncaixe ? "E" : "D",
                        CnpjExpedidor = pedidoAgrupamento.Expedidor?.CPFCNPJ.Trim() ?? "",
                        CnpjRecebedor = pedidoAgrupamento.Recebedor?.CPFCNPJ.Trim() ?? "",
                        PedidoEncaixe = pedidoAgrupamento.PedidoDeEncaixe,
                        PedidoReentrega = pedidoAgrupamento.Reentrega
                    };

                    if (!string.IsNullOrWhiteSpace(pedidoAgrupamento.DataCarregamento))
                    {
                        if (!DateTime.TryParseExact(pedidoAgrupamento.DataCarregamento, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataCarregamento))
                            throw new ServicoException($"A data de carregamento não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");

                        preAgrupamentoCarga.DataCarregamento = dataCarregamento;
                    }

                    if (!string.IsNullOrWhiteSpace(pedidoAgrupamento.DataPrevisaoEntrega))
                    {
                        if (!DateTime.TryParseExact(pedidoAgrupamento.DataPrevisaoEntrega, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisaoEntrega))
                            throw new ServicoException($"A data de previsão de entrega não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");

                        preAgrupamentoCarga.DataPrevisaoEntrega = dataPrevisaoEntrega;
                    }

                    if (!string.IsNullOrWhiteSpace(agrupador.DataPrevisaoInicioViagem))
                    {
                        if (!DateTime.TryParseExact(agrupador.DataPrevisaoInicioViagem, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevisaoInicioViagem))
                            throw new ServicoException($"A data de previsão inicio viagem não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");

                        preAgrupamentoCarga.DataPrevisaoInicioViagem = dataPrevisaoInicioViagem;
                    }

                    if (preAgrupamentoCarga.CnpjEmitente == preAgrupamentoCarga.CnpjExpedidor)
                        preAgrupamentoCarga.CnpjExpedidor = "";

                    if ((!string.IsNullOrWhiteSpace(preAgrupamentoCarga.CnpjExpedidor) || pedidoAgrupamento.PedidoDeEncaixe) && pedidoAgrupamento.NotasFiscal.Count == 0)
                        throw new ServicoException($"É obrigatório informar as notas fiscais.");

                    repositorioPreAgrupamentoCarga.Inserir(preAgrupamentoCarga);

                    foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal in pedidoAgrupamento.NotasFiscal)
                    {
                        DateTime dataPrevisaoEntrega = DateTime.MinValue;
                        if (!string.IsNullOrWhiteSpace(notaFiscal.DataPrevisao))
                        {
                            if (!DateTime.TryParseExact(notaFiscal.DataPrevisao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataPrevisaoEntrega))
                                throw new ServicoException($"A data de previsão de entrega não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");
                        }

                        Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal preAgrupamentoNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal()
                        {
                            CnpjEmitente = notaFiscal.Emitente?.CPFCNPJ.Trim() ?? "",
                            CnpjDestinatario = notaFiscal.Destinatario?.CPFCNPJ.Trim() ?? "",
                            NumeroNota = notaFiscal.Numero.ToString(),
                            SerieNota = notaFiscal.Serie.Trim(),
                            PreAgrupamentoCarga = preAgrupamentoCarga,
                            NumeroPedido = Utilidades.String.Left(notaFiscal.NumeroPedido, 50)
                        };

                        if (dataPrevisaoEntrega != DateTime.MinValue)
                            preAgrupamentoNotaFiscal.DataPrevisaoEntrega = dataPrevisaoEntrega;

                        repositorioPreAgrupamentoNotaFiscal.Inserir(preAgrupamentoNotaFiscal);
                    }
                }

                _unitOfWork.CommitChanges();

                SalvarArquivoIntegracao(agrupador);

                return preAgrupamentoCargaAgrupador.Codigo;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void ExcluirPorPreAgrupamentoCarga(int codigoPreAgrupamentoCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorioPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao repPreAgrupamentoCargaRoteirizacao = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaRoteirizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga = repositorioPreAgrupamentoCarga.BuscarPorCodigo(codigoPreAgrupamentoCarga, auditavel: false);

            if (preAgrupamentoCarga == null)
                throw new ServicoException("Pré agrupamento de carga não encontrado.");

            Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador agrupador = preAgrupamentoCarga.Agrupador;

            if (!agrupador.Situacao.IsPermitirExcluir())
                throw new ServicoException("A atual situação do agrupamento não permite essa operação.");

            if (agrupador.Agrupamentos.Any(obj => !obj.PedidoEncaixe && obj.Carga != null && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && (obj.Carga.CargaFechada || obj.Carga.CargaAgrupamento != null)))
                throw new ServicoException("Não é possível excluir o agrupamento pois o mesmo está vinculado a cargas ativas. Favor cancelar as cargas para exluir o agrupamento.");

            Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repositorioPreAgrupamentoCargaAgrupador = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal repositorioPreAgrupamentoNotaFiscal = new Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal(_unitOfWork);

            try
            {
                _unitOfWork.Start();

                repPreAgrupamentoCargaRoteirizacao.DeletarPorAgrupamento(agrupador.Codigo);
                repositorioPreAgrupamentoNotaFiscal.DeletarPorAgrupamento(agrupador.Codigo);
                repositorioPreAgrupamentoCarga.DeletarPorAgrupamento(agrupador.Codigo);
                repositorioPreAgrupamentoCargaAgrupador.Deletar(agrupador, auditado);

                _unitOfWork.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void GerarCarregamentos(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork).BuscarConfiguracaoPadrao();

            GerarRedespachoPorAgrupamento(configuracaoTMS, tipoServicoMultisoftware);
            GerarCarregamentoPorAgrupamento(configuracaoTMS, tipoServicoMultisoftware, configuracaoGeralCarga, configuracaoPedido);
            GerarEncaixesPendentes(configuracaoTMS, tipoServicoMultisoftware, configuracaoGeralCarga);
        }

        public void VincularCargaAosAgrupamentosPorPedidoEncaixe(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal repositorioPreAgrupamentoNotaFiscal = new Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorioPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscaisCarga = repositorioPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            HashSet<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> listaPreAgrupamentoCargaAgrupador = new HashSet<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidosXMLNotaFiscaisCarga)
            {
                List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> listaPreAgrupamentoNotaFiscal = repositorioPreAgrupamentoNotaFiscal.BuscarPorNumeroNotaEmitente(pedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString(), pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ_SemFormato);

                foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal preAgrupamentoNotaFiscal in listaPreAgrupamentoNotaFiscal)
                {
                    preAgrupamentoNotaFiscal.PreAgrupamentoCarga.CargaPedidoEncaixe = pedidoXMLNotaFiscal.CargaPedido;
                    preAgrupamentoNotaFiscal.PreAgrupamentoCarga.Carga = pedidoXMLNotaFiscal.CargaPedido.Carga;

                    repositorioPreAgrupamentoCarga.Atualizar(preAgrupamentoNotaFiscal.PreAgrupamentoCarga);
                    listaPreAgrupamentoCargaAgrupador.Add(preAgrupamentoNotaFiscal.PreAgrupamentoCarga.Agrupador);

                    if (string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.NumeroPedidoEmbarcador) && !string.IsNullOrWhiteSpace(preAgrupamentoNotaFiscal.NumeroPedido))
                    {
                        pedidoXMLNotaFiscal.XMLNotaFiscal.NumeroPedidoEmbarcador = preAgrupamentoNotaFiscal.NumeroPedido;
                        repXMLNotaFiscal.Atualizar(pedidoXMLNotaFiscal.XMLNotaFiscal);
                    }

                }
            }

            AtualizarSituacaoAgrupadores(listaPreAgrupamentoCargaAgrupador);
        }

        public void VincularCargaAosAgrupamentosSemCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorioPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> agrupamentos = repositorioPreAgrupamentoCarga.BuscarSemCargaPorNumeroCarga(carga.CodigoCargaEmbarcador);

            if (agrupamentos.Count > 0)
            {
                HashSet<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> agrupadores = new HashSet<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>();

                foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga agrupamento in agrupamentos)
                {
                    if (string.IsNullOrWhiteSpace(agrupamento.CnpjExpedidor))
                    {
                        agrupamento.Carga = carga;

                        if (repositorioPreAgrupamentoCarga.VerificarSeExisteCargaDePreCargaPorAgrupamento(agrupamento.Agrupador.Codigo))
                            agrupamento.Agrupador.PossuiPreCargas = true;
                        else
                            agrupamento.Agrupador.PossuiPreCargas = carga.CargaDePreCarga;

                        repositorioPreAgrupamentoCarga.Atualizar(agrupamento);
                        agrupadores.Add(agrupamento.Agrupador);
                    }
                    else if (carga.TipoOperacao?.PermitirGerarRedespacho ?? true)
                    {
                        if (!carga.CargaDePreCarga)
                        {
                            agrupamento.CargaRedespacho = carga;
                            repositorioPreAgrupamentoCarga.Atualizar(agrupamento);
                            agrupadores.Add(agrupamento.Agrupador);
                        }
                    }

                    if (agrupamento.DataPrevisaoInicioViagem.HasValue)
                    {
                        carga.DataInicioViagemPrevista = agrupamento.DataPrevisaoInicioViagem;
                        repositorioCarga.Atualizar(carga);
                    }
                }

                AtualizarSituacaoAgrupadores(agrupadores);
            }
        }

        public void VincularCargasAosAgrupamentosAguardandoProcessamento()
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repositorioPreAgrupamentoCargaAgrupador = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> preAgrupamentoCargaAgrupadores = repositorioPreAgrupamentoCargaAgrupador.BucarPorSituacaoAguardandoProcessamento(10);

            foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador preAgrupamentoCargaAgrupador in preAgrupamentoCargaAgrupadores)
            {
                Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal repositorioPreAgrupamentoNotaFiscal = new Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> preAgrupamentoNotaFiscaisAgrupador = repositorioPreAgrupamentoNotaFiscal.BuscarPorAgrupador(preAgrupamentoCargaAgrupador.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga in preAgrupamentoCargaAgrupador.Agrupamentos)
                {
                    if (preAgrupamentoCarga.PedidoEncaixe)
                        VincularCargaAoAgrupamentoPorPedidoEncaixe(preAgrupamentoCarga, preAgrupamentoNotaFiscaisAgrupador);
                    else
                        VincularCargaAoAgrupamento(preAgrupamentoCarga, preAgrupamentoCargaAgrupador);
                }

                if (preAgrupamentoCargaAgrupador.IsTodosAgrupamentosPossuemCarga(preAgrupamentoCargaAgrupador.PossuiPreCargas))
                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.AguardandoCarregamento;
                else if (preAgrupamentoCargaAgrupador.IsTodosAgrupamentosPossuemCargaOuCargaRedespacho(preAgrupamentoCargaAgrupador.PossuiPreCargas))
                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.AguardandoRedespacho;
                else
                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.SemCarga;

                repositorioPreAgrupamentoCargaAgrupador.Atualizar(preAgrupamentoCargaAgrupador);
            }
        }

        #endregion

        #region Métodos Públicos de Consulta

        public bool IsCargaPossuiAgrupamentoVinculado(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorioPreAgrupamentoCarga = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(_unitOfWork);

            return repositorioPreAgrupamentoCarga.BuscarPorCarga(codigoCarga) != null;
        }

        public bool IsPossuiIntegracaoOrtec()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            return repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Ortec);
        }

        public string ObterNomeArquivoIntegracaoPorPreAgrupamentoCarga(int codigoPreAgrupamentoCarga)
        {
            Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga = ObterPreAgrupamentoCarga(codigoPreAgrupamentoCarga);

            if (preAgrupamentoCarga.Agrupador.ArquivoIntegracao == null)
                throw new ServicoException("Não foi possível encontrar o arquivo de integração");

            string caminhoArquivoIntegracao = ObterCaminhoArquivoIntegracao();
            string nomeArquivoIntegracao = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivoIntegracao, preAgrupamentoCarga.Agrupador.ArquivoIntegracao.NomeArquivo);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivoIntegracao))
                throw new ServicoException("O arquivo de integração não existe");

            return nomeArquivoIntegracao;
        }

        #endregion
    }
}
