using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class NFe : ServicoBase
    {        
        public NFe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public string EmitirNFeRemessa(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, string webServiceConsultaCTe, int tipoEnvio)
        {
            string mensagem = "";
            Hubs.Carga svcHubCarga = new Hubs.Carga();
            Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaNFe repCargaNFe = new Repositorio.Embarcador.Cargas.CargaNFe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
            Repositorio.Embarcador.WMS.Recebimento repRecebimento = new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
            Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria = new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe>();
            permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoNFe.total);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            if (pedidoXMLNotaFiscals != null && pedidoXMLNotaFiscals.Count > 0)
            {
                int totalDocumentos = pedidoXMLNotaFiscals.Count(), totalDocumentosGerados = 0;
                foreach (var pedidoNota in pedidoXMLNotaFiscals)
                {
                    if (pedidoNota.XMLNotaFiscal != null && !string.IsNullOrWhiteSpace(pedidoNota.XMLNotaFiscal.Chave) && pedidoNota.XMLNotaFiscal.Modelo == "55")
                    {
                        if (!repCargaNFe.NotaJaGeradaNaCarga(carga.Codigo, pedidoNota.XMLNotaFiscal.Chave))
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> mercadorias = repXMLNotaFiscalProduto.BuscarPorNotaFiscal(pedidoNota.XMLNotaFiscal.Codigo, true);
                            if (mercadorias != null && mercadorias.Count > 0)
                            {
                                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfeIntegracao = serNotaFiscalEletronica.ConverterProdutosXMLParaNFe(carga, mercadorias, unitOfWork, carga.Empresa);
                                if (nfeIntegracao != null)
                                {
                                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal();
                                    unitOfWork.Start();
                                    serNotaFiscalEletronica.SalvarDadosNFe(ref nfe, nfeIntegracao, permissoes, carga.Operador, unitOfWork, carga.Empresa, tipoServicoMultisoftware, configuracao);

                                    Dominio.Entidades.Embarcador.Cargas.CargaNFe cargaNFe = new Dominio.Entidades.Embarcador.Cargas.CargaNFe()
                                    {
                                        Carga = carga,
                                        CargaOrigem = carga,
                                        NotaFiscal = nfe
                                    };
                                    repCargaNFe.Inserir(cargaNFe);

                                    unitOfWork.CommitChanges();
                                }
                            }
                        }
                    }
                    totalDocumentosGerados++;
                    svcHubCarga.InformarQuantidadeDocumentosEmitidos(carga.Codigo, totalDocumentos, totalDocumentosGerados);
                }
            }
            return mensagem;
        }

        public string EmitirNFe(int codigoNFe, Repositorio.UnitOfWork unitOfWork, string relatorio, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario)
        {
            string mensagem = "";
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            if (nfe.Empresa.DataFinalCertificado != null && nfe.Empresa.DataFinalCertificado > DateTime.MinValue)
            {
                if (nfe.Empresa.DataFinalCertificado < DateTime.Now)
                {
                    return "O certificado digital cadastrado na empresa se encontra vencido.";
                }
            }

            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> itens = repNotaFiscalProdutos.BuscarPorNota(codigoNFe);

            if (nfe == null)
            {
                mensagem = "O NF-e informado não foi localizado";
            }
            else if (nfe != null && (itens == null || itens.Count == 0))
            {
                mensagem = "A NF-e não possui nenhum item lançado.";
            }
            else
            {
                //var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                mensagem = "";//z.CriarEnviarNFe(codigoNFe, unitOfWork, relatorio, caminhoRelatoriosEmbarcador, usuario, "55", 1, false, false);
            }

            return mensagem;
        }

        #endregion
    }
}
