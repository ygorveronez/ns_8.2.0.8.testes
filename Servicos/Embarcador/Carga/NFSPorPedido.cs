//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Servicos.Embarcador.Carga
//{
//    public class NFSPorPedido : ServicoBase
//    {
//        public NFSPorPedido(string stringConexao) : base(stringConexao) { }
//        public void GerarNFSPorPedidoAgrupado(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente expedidor, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Localidade localidadePrestacao, Dominio.Enumeradores.TipoPagamento tipoPagamento, ref List<Dominio.Entidades.NFSe> NFSesParaEmissao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, int totalDocumentos, ref int totalDocumentosGerados)
//        {
//            bool emite = true;
            
//            Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete repCargaNFSComponentesFrete = new Repositorio.Embarcador.Cargas.CargaNFSComponentesFrete(unitOfWork);

//            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS repCargaPedidoXMLNotaFiscalNFS = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS(unitOfWork);
//            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
//            Servicos.Embarcador.NFSe.NFSe serNFSe = new NFSe.NFSe(StringConexao);
//            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);

//            decimal valorFrete = 0;
//            decimal peso = 0;
//            Dominio.Entidades.Cliente tomadorNFe = null;

//            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = serNFSe.BuscarConfiguracaoEmissaoNFSe(carga.Empresa.Codigo, localidadePrestacao.Codigo, unitOfWork);
//            Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS regraISS = new Dominio.ObjetosDeValor.Embarcador.ISS.RegraISS();

//            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = serCargaPedido.BuscarCargaPedidoComponentesFrete(cargaPedidos, unitOfWork);

//            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
//            {
//                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

//                if (pedidoXMLNotaFiscais.Any(o => o.NFSs.Count > 0))
//                {
//                    emite = false;
//                    break;
//                }

//                peso += (from obj in pedidoXMLNotaFiscais select obj.XMLNotaFiscal.Peso).Sum();
//                valorFrete += cargaPedido.ValorFrete;

//                if (tipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
//                    tomadorNFe = remetente;
//                else if (tipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
//                    tomadorNFe = destinatario;
//                else
//                    tomadorNFe = tomador;

//                regraISS.AliquotaISS = cargaPedido.PercentualAliquotaISS;
//                regraISS.PercentualRetencaoISS = cargaPedido.PercentualRetencaoISS;
//                regraISS.IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo;
//                regraISS.ValorBaseCalculoISS += cargaPedido.BaseCalculoISS;
//                regraISS.ValorISS += cargaPedido.ValorISS;
//                regraISS.ValorRetencaoISS += cargaPedido.ValorRetencaoISS;
//            }

//            if (emite && valorFrete > 0)
//            {
//                peso = Math.Round(peso, 6, MidpointRounding.AwayFromZero);

//                Dominio.Entidades.Embarcador.Cargas.CargaNFS cargaNFS = serNFSe.GerarNFSe(carga, peso, valorFrete, tomadorNFe, localidadePrestacao, regraISS, transportadorConfiguracaoNFSe.ServicoNFSe, transportadorConfiguracaoNFSe.NaturezaNFSe, tipoServicoMultisoftware, cargaPedidoComponentesFretesCliente, unitOfWork);

//                NFSesParaEmissao.Add(cargaNFS.NotaFiscalServico.NFSe);
                
//                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
//                {
//                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
//                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscais)
//                    {
//                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS cargaPedidoXMLNotaFiscalNFS = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalNFS();
//                        cargaPedidoXMLNotaFiscalNFS.CargaNFS = cargaNFS;
//                        cargaPedidoXMLNotaFiscalNFS.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
//                        repCargaPedidoXMLNotaFiscalNFS.Inserir(cargaPedidoXMLNotaFiscalNFS);
//                    }

//                }
//            }
//        }
//    }
//}
