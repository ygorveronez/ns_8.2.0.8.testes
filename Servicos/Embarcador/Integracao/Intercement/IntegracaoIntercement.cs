using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Servicos.Embarcador.Integracao.Intercement
{
    public class IntegracaoIntercement
    {

        #region Métodos Públicos

        public static dynamic Ler(System.IO.Stream stream)
        {
            stream.Position = 0;

            XDocument doc = null;

            System.IO.Stream stream2 = new MemoryStream();

            // Definio inicio do XML, que pode ser 0, ou o tamanho do so cabecalho
            int inicioXML = 0;
            try
            {
                StreamReader oReader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1"));
                doc = XDocument.Load(oReader);
                doc.Save(stream2);
            }
            catch (System.Exception)
            {

            }

            if (doc == null)
            {
                try
                {
                    // Remove Cabecalho
                    inicioXML = ("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>".Length * 2) - 2; // Multiplicado por 2 pois a string do strim possui "\0" para cada caracter
                    stream.Position = inicioXML;

                    StreamReader oReader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1"));
                    doc = XDocument.Load(oReader);
                    doc.Save(stream2);
                }
                catch (System.Exception)
                {
                }
            }

            if (doc == null)
            {
                stream.Position = 0;
                doc = XDocument.Load(stream);
                doc.Save(stream2);
            }

            stream2.Position = 0;

            if (doc != null && (doc.Root.Descendants("TAB").Count() > 0 || doc.Descendants("item").Count() > 0))
            {
                string jsonText = JsonConvert.SerializeXNode(doc.Descendants("item").FirstOrDefault());
                return JsonConvert.DeserializeObject<dynamic>(jsonText);
            }

            return null;
        }

        public static string ProcessarXMLIntercement(dynamic objIntercement, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string retorno = "";

            Repositorio.Embarcador.Pedidos.EspelhoIntercement repEspelhoIntercement = new Repositorio.Embarcador.Pedidos.EspelhoIntercement(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement repPedidoEspelhoIntercement = new Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorPedido(cargaPedido.Pedido?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(cargaPedido.Pedido?.Codigo ?? 0, true);

            if (carga == null)
                return "Carga não localizada.";

            if (objIntercement != null && objIntercement.item != null)
            {
                string vblen = (string)objIntercement.item.VBELN;
                string placaVeiculo = (string)objIntercement.item.SIGNI;

                if (!string.IsNullOrWhiteSpace(vblen) && !string.IsNullOrWhiteSpace(placaVeiculo))
                {
                    Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement espelhoIntercement = repEspelhoIntercement.BuscarPorVBELN(vblen);
                    if (espelhoIntercement == null)
                    {
                        DateTime.TryParseExact((string)objIntercement.item.ERDAT, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime dataERDAT);
                        DateTime.TryParseExact((string)objIntercement.item.ERZET, "HH:mm:ss.fff", null, System.Globalization.DateTimeStyles.None, out DateTime horaERZET);

                        espelhoIntercement = new Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement()
                        {
                            CHAPA = ((string)objIntercement.item.CHAPA).Replace(".", ",").ToDecimal(),
                            DIFPESO = ((string)objIntercement.item.DIFPESO).Replace(".", ",").ToDecimal(),
                            ERDAT = dataERDAT,
                            ERZET = horaERZET.TimeOfDay, //((string)objIntercement.item.ERZET).ToTime(),
                            FKNUM = (string)objIntercement.item.FKNUM,
                            OUTROS = ((string)objIntercement.item.OUTROS).Replace(".", ",").ToDecimal(),
                            REBEL = (string)objIntercement.item.REBEL,
                            SIGNI = (string)objIntercement.item.SIGNI,
                            TARIFA = ((string)objIntercement.item.TARIFA).Replace(".", ",").ToDecimal(),
                            TOTAL = ((string)objIntercement.item.TOTAL).Replace(".", ",").ToDecimal(),
                            VBELN = (string)objIntercement.item.VBELN,
                            VIAGEM = (string)objIntercement.item.VIAGEM
                        };

                        if (objIntercement.item.PISCOFINS != null && !string.IsNullOrWhiteSpace((string)objIntercement.item.PISCOFINS))
                            espelhoIntercement.PISCOFINS = ((string)objIntercement.item.PISCOFINS).Replace(".", ",").ToDecimal();
                    }

                    if (carga.Veiculo?.Placa != placaVeiculo && !carga.VeiculosVinculados.Any(c => c.Placa == placaVeiculo))
                        return "O veículo (" + placaVeiculo + ") informado no Espelho não se encontra na carga selecionada.";

                    unitOfWork.Start();

                    if (espelhoIntercement.Codigo == 0)
                        repEspelhoIntercement.Inserir(espelhoIntercement);

                    if (!repPedidoEspelhoIntercement.ContemPorEspelhoCargaPedido(espelhoIntercement.Codigo, cargaPedido.Codigo))
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement pedidoEspelhoIntercement = new Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement()
                        {
                            CargaPedido = cargaPedido,
                            EspelhoIntercement = espelhoIntercement
                        };

                        repPedidoEspelhoIntercement.Inserir(pedidoEspelhoIntercement);

                        if (espelhoIntercement.ERDAT.HasValue && espelhoIntercement.ERZET.HasValue)
                        {
                            pedido.DataPrevisaoSaida = espelhoIntercement.ERDAT.Value.Add(espelhoIntercement.ERZET.Value);
                            pedido.PrevisaoEntrega = espelhoIntercement.ERDAT.Value.AddDays(1).Add(espelhoIntercement.ERZET.Value);
                            repPedido.Atualizar(pedido, auditado);
                        }

                        BuscarNotasFiscaisPendentesVinculo(espelhoIntercement.VBELN, cargaPedido, unitOfWork, auditado, configuracaoTMS, stringConexao, tipoServicoMultisoftware);

                        unitOfWork.CommitChanges();
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return "Espelho já inserido nesta carga.";
                    }
                }
                else
                    return "Não foi encontrado nenhuma descrição de VBELN espelho da Intercement.";
            }
            else
                return "Não foi encontrado nenhum espelho da Intercement.";

            return retorno;
        }

        public static void BuscarNotasFiscaisPendentesVinculo(string VBELN, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (string.IsNullOrWhiteSpace(VBELN))
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);

            Servicos.WebService.Carga.Pedido serWSPedido = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasSemCarga = repXMLNotaFiscal.BuscarPorNumeroVBELNIntercementSemCarga(VBELN);
            if (notasSemCarga != null && notasSemCarga.Count > 0)
            {
                foreach (var xmlNotaFiscal in notasSemCarga)
                {
                    if (cargaPedido != null && cargaPedido.Pedido != null)
                    {
                        new Servicos.Embarcador.Pessoa.GrupoPessoasObservacaoNfe().AdicionarDadosNfePorGrupoPessoasEmitente(cargaPedido, xmlNotaFiscal.Emitente, xmlNotaFiscal.Observacao, tipoServicoMultisoftware, configuracaoTMS, unitOfWork, xmlNotaFiscal, auditado);
                        repPedido.Atualizar(cargaPedido.Pedido);
                    }
                    if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarDadosPedidoParaNotasExterior && xmlNotaFiscal.Destinatario != null && xmlNotaFiscal.Destinatario.Tipo == "E")
                    {
                        switch (cargaPedido.Pedido.TipoPagamento)
                        {
                            case Dominio.Enumeradores.TipoPagamento.Pago:
                                xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                                break;
                            case Dominio.Enumeradores.TipoPagamento.A_Pagar:
                                xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                                break;
                            case Dominio.Enumeradores.TipoPagamento.Outros:
                                xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
                                break;
                            default:
                                break;
                        }

                        if (xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada)
                        {
                            xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Remetente;
                            if (configuracaoTMS.UtilizaEmissaoMultimodal)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                                serWSPedido.PreecherEnderecoPedidoPorCliente(enderecoOrigem, cargaPedido.Pedido.Remetente);
                                repPedidoEndereco.Inserir(enderecoOrigem);
                                cargaPedido.Pedido.EnderecoOrigem = enderecoOrigem;
                            }
                        }
                        else
                            xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Destinatario;
                    }

                    xmlNotaFiscal.Filial = cargaPedido.Pedido.Filial;

                    bool msgAlertaObservacao = false;
                    bool notaFiscalEmOutraCarga = false;
                    string retorno = serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);
                    if (msgAlertaObservacao && !string.IsNullOrWhiteSpace(retorno))
                        retorno = "";

                    if (string.IsNullOrEmpty(retorno))
                    {
                        if (!serCargaNotaFiscal.InformarComponentesOperacaoIntercement(out string msgErro, cargaPedido, xmlNotaFiscal))
                            return;
                        else
                            repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                        serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, configuracaoTMS, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, null, "Adicionado pela integração Intercement", unitOfWork);
                    }
                }
            }
        }

        #endregion
    }
}
