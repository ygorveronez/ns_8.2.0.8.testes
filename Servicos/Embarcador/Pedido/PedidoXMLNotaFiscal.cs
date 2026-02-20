using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Pedido
{
    public class PedidoXMLNotaFiscal
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga _configuracaoGeralCarga;

        #endregion Atributos

        #region Construtores

        public PedidoXMLNotaFiscal(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, configuracaoGeralCarga: null) { }

        public PedidoXMLNotaFiscal(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, configuracaoGeralCarga: null) { }

        public PedidoXMLNotaFiscal(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _configuracaoGeralCarga = configuracaoGeralCarga;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void ArmazenarProdutosXML(string xml, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProdutoLote repXMLNotaFiscalProdutoLote = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProdutoLote(_unitOfWork);

            Servicos.NFe svcNFe = new Servicos.NFe(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe nfXml = null;

            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(xml);
                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(byteArray);
                nfXml = svcNFe.ObterDocumentoPorXML(memoryStream, _unitOfWork);
            }
            catch (Exception)
            {

            }

            if (nfXml != null)
            {
                repXMLNotaFiscalProdutoLote.ExcluirTodosPorXMLNotaFiscal(xMLNotaFiscal.Codigo);
                repXMLNotaFiscalProduto.ExcluirTodosPorXMLNotaFiscal(xMLNotaFiscal.Codigo);

                AtualizarProdutosCargaPedidoPorNotaFiscal(nfXml.Produtos, xMLNotaFiscal, Auditado, tipoServicoMultisoftware, pedido);
            }
        }

        public async Task ArmazenarProdutosXMLAsync(string xml, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProdutoLote repXMLNotaFiscalProdutoLote = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProdutoLote(_unitOfWork);

            Servicos.NFe svcNFe = new Servicos.NFe(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe nfXml = null;

            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(xml);
                System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(byteArray);
                nfXml = svcNFe.ObterDocumentoPorXML(memoryStream, _unitOfWork);
            }
            catch (Exception)
            {

            }

            if (nfXml != null)
            {
                await repXMLNotaFiscalProdutoLote.ExcluirTodosPorXMLNotaFiscalAsync(xMLNotaFiscal.Codigo);
                await repXMLNotaFiscalProduto.ExcluirTodosPorXMLNotaFiscalAsync(xMLNotaFiscal.Codigo);

                AtualizarProdutosCargaPedidoPorNotaFiscal(nfXml.Produtos, xMLNotaFiscal, Auditado, tipoServicoMultisoftware, pedido);
            }
        }

        public void ArmazenarProdutosNotaFiscalPorListaDeProduto(List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtos, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (produtos != null && produtos.Count > 0)
                PreencherNotaProduto(produtos, xMLNotaFiscal, pedido, Auditado, tipoServicoMultisoftware);
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PreencherDadosNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNota = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal()
            {
                BaseCalculoICMS = 0m,
                BaseCalculoICMSFilialEmissora = 0m,
                BaseCalculoISS = 0m,
                CargaPedido = cargaPedido,
                CentroResultado = null,
                CentroResultadoDestinatario = null,
                CentroResultadoEscrituracao = null,
                CentroResultadoICMS = null,
                CentroResultadoPIS = null,
                CentroResultadoCOFINS = null,
                CFOP = cargaPedido.CFOP,
                CFOPFilialEmissora = cargaPedido.CFOPFilialEmissora,
                CST = cargaPedido.CST,
                CSTFilialEmissora = cargaPedido.CSTFilialEmissora,
                DescontarICMSDoValorAReceber = cargaPedido.DescontarICMSDoValorAReceber,
                ICMSPagoPorST = cargaPedido.ICMSPagoPorST,
                ICMSPagoPorSTFilialEmissora = cargaPedido.ICMSPagoPorSTFilialEmissora,
                IncluirICMSBaseCalculo = cargaPedido.IncluirICMSBaseCalculo,
                IncluirICMSBaseCalculoFilialEmissora = cargaPedido.IncluirICMSBaseCalculoFilialEmissora,
                IncluirISSBaseCalculo = cargaPedido.IncluirISSBaseCalculo,
                ItemServico = cargaPedido.ItemServico,
                ModeloDocumentoFiscal = cargaPedido.ModeloDocumentoFiscal,
                Moeda = cargaPedido.Moeda,
                NaoImprimirImpostosDACTE = cargaPedido.NaoImprimirImpostosDACTE,
                NaoEnviarImpostoICMSNaEmissaoCte = cargaPedido.NaoEnviarImpostoICMSNaEmissaoCte,
                NaoReduzirRetencaoICMSDoValorDaPrestacao = cargaPedido.NaoReduzirRetencaoICMSDoValorDaPrestacao,
                NotaFiscalEmOutraCarga = false,
                ObservacaoNotaFiscal = "",
                ObservacaoRegraICMSCTe = cargaPedido.ObservacaoRegraICMSCTe,
                ObservacaoRegraICMSCTeFilialEmissora = cargaPedido.ObservacaoRegraICMSCTeFilialEmissora,
                OrdemColeta = 0,
                OrdemEntrega = 0,
                PercentualAliquota = cargaPedido.PercentualAliquota,
                PercentualAliquotaFilialEmissora = cargaPedido.PercentualAliquotaFilialEmissora,
                PercentualAliquotaFilialEmissoraInternaDifal = cargaPedido.PercentualAliquotaFilialEmissoraInternaDifal,
                PercentualAliquotaInternaDifal = cargaPedido.PercentualAliquotaInternaDifal,
                PercentualCreditoPresumido = cargaPedido.PercentualCreditoPresumido,
                PercentualAliquotaISS = cargaPedido.PercentualAliquotaISS,
                PercentualCreditoPresumidoFilialEmissora = cargaPedido.PercentualCreditoPresumidoFilialEmissora,
                PercentualIncluirBaseCalculo = cargaPedido.PercentualIncluirBaseCalculo,
                PercentualIncluirBaseCalculoFilialEmissora = cargaPedido.PercentualIncluirBaseCalculoFilialEmissora,
                PercentualPagamentoAgregado = cargaPedido.PercentualPagamentoAgregado,
                PercentualReducaoBC = cargaPedido.PercentualReducaoBC,
                PercentualReducaoBCFilialEmissora = cargaPedido.PercentualReducaoBCFilialEmissora,
                PercentualRetencaoISS = cargaPedido.PercentualRetencaoISS,
                Peso = notaFiscal.Peso,
                PossuiCTe = true,
                PossuiNFS = false,
                PossuiNFSManual = false,
                SituacaoNotaFiscal = SituacaoNotaFiscal.AgEntrega,
                TipoNotaFiscal = TipoNotaFiscal.Todas,
                ValorCotacaoMoeda = cargaPedido.ValorCotacaoMoeda,
                ValorCreditoPresumido = 0m,
                ValorFrete = 0m,
                ValorFreteFilialEmissora = 0m,
                ValorFreteTabelaFrete = 0m,
                ValorCreditoPresumidoFilialEmissora = 0m,
                ValorFreteTabelaFreteFilialEmissora = 0m,
                ValorICMS = 0m,
                ValorICMSFilialEmissora = 0m,
                ValorICMSIncluso = 0m,
                ValorISS = 0m,
                ValorMaximoCentroContabilizacao = 0m,
                ValorRetencaoISS = 0m,
                ValorTotalComponentes = 0m,
                ValorTotalMoeda = 0m,
                ValorTotalMoedaComponentes = 0m,
                XMLNotaFiscal = notaFiscal
            };

            return pedidoXMLNota;
        }


        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterRetornoImpostoIBSCBS(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool filialEmissora = false)
        {
            if (filialEmissora)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS()
                {
                    CST = pedidoXMLNotaFiscal.CSTIBSCBSFilialEmissora,
                    ClassificacaoTributaria = pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBSFilialEmissora,

                    CodigoIndicadorOperacao = pedidoXMLNotaFiscal.CodigoIndicadorOperacao,
                    NBS = pedidoXMLNotaFiscal.NBS,

                    AliquotaIBSEstadual = pedidoXMLNotaFiscal.AliquotaIBSEstadualFilialEmissora,
                    PercentualReducaoIBSEstadual = pedidoXMLNotaFiscal.PercentualReducaoIBSEstadualFilialEmissora,

                    AliquotaIBSMunicipal = pedidoXMLNotaFiscal.AliquotaIBSMunicipalFilialEmissora,
                    PercentualReducaoIBSMunicipal = pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipalFilialEmissora,

                    AliquotaCBS = pedidoXMLNotaFiscal.AliquotaCBSFilialEmissora,
                    PercentualReducaoCBS = pedidoXMLNotaFiscal.PercentualReducaoCBSFilialEmissora,
                };
            }

            return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS()
            {
                CodigoOutraAliquota = pedidoXMLNotaFiscal.OutrasAliquotas?.Codigo ?? 0,
                CST = pedidoXMLNotaFiscal.CSTIBSCBS,
                ClassificacaoTributaria = pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS,

                CodigoIndicadorOperacao = pedidoXMLNotaFiscal.CodigoIndicadorOperacao,
                NBS = pedidoXMLNotaFiscal.NBS,

                AliquotaIBSEstadual = pedidoXMLNotaFiscal.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual,

                AliquotaIBSMunicipal = pedidoXMLNotaFiscal.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal,

                AliquotaCBS = pedidoXMLNotaFiscal.AliquotaCBS,
                PercentualReducaoCBS = pedidoXMLNotaFiscal.PercentualReducaoCBS,
            };
        }

        public void PreencherValoresRetornoIBSCBS(Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS)
        {
            impostoIBSCBS.BaseCalculo += baseCalculo;
            impostoIBSCBS.ValorIBSEstadual += valorIBSEstadual;
            impostoIBSCBS.ValorIBSMunicipal += valorIBSMunicipal;
            impostoIBSCBS.ValorCBS += valorCBS;
        }

        public void PreencherCamposImpostoIBSCBS(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS)
        {
            pedidoXMLNotaFiscal.SetarRegraOutraAliquota(impostoIBSCBS.CodigoOutraAliquota);
            pedidoXMLNotaFiscal.CSTIBSCBS = impostoIBSCBS.CST;
            pedidoXMLNotaFiscal.CodigoIndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;
            pedidoXMLNotaFiscal.NBS = impostoIBSCBS.NBS;
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributaria;
            pedidoXMLNotaFiscal.BaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;

            pedidoXMLNotaFiscal.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
            pedidoXMLNotaFiscal.ValorIBSEstadual = Math.Round(impostoIBSCBS.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);

            pedidoXMLNotaFiscal.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
            pedidoXMLNotaFiscal.ValorIBSMunicipal = Math.Round(impostoIBSCBS.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);

            pedidoXMLNotaFiscal.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
            pedidoXMLNotaFiscal.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
            pedidoXMLNotaFiscal.ValorCBS = Math.Round(impostoIBSCBS.ValorCBS, 3, MidpointRounding.AwayFromZero);
        }

        public void PreencherCamposImpostoIBSCBSFilialEmissora(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS)
        {
            pedidoXMLNotaFiscal.CSTIBSCBSFilialEmissora = impostoIBSCBS.CST;
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBSFilialEmissora = impostoIBSCBS.ClassificacaoTributaria;
            pedidoXMLNotaFiscal.BaseCalculoIBSCBSFilialEmissora = impostoIBSCBS.BaseCalculo;

            pedidoXMLNotaFiscal.AliquotaIBSEstadualFilialEmissora = impostoIBSCBS.AliquotaIBSEstadual;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadualFilialEmissora = impostoIBSCBS.PercentualReducaoIBSEstadual;
            pedidoXMLNotaFiscal.ValorIBSEstadualFilialEmissora = Math.Round(impostoIBSCBS.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);

            pedidoXMLNotaFiscal.AliquotaIBSMunicipalFilialEmissora = impostoIBSCBS.AliquotaIBSMunicipal;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipalFilialEmissora = impostoIBSCBS.PercentualReducaoIBSMunicipal;
            pedidoXMLNotaFiscal.ValorIBSMunicipalFilialEmissora = Math.Round(impostoIBSCBS.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);

            pedidoXMLNotaFiscal.AliquotaCBSFilialEmissora = impostoIBSCBS.AliquotaCBS;
            pedidoXMLNotaFiscal.PercentualReducaoCBSFilialEmissora = impostoIBSCBS.PercentualReducaoCBS;
            pedidoXMLNotaFiscal.ValorCBSFilialEmissora = Math.Round(impostoIBSCBS.ValorCBS, 3, MidpointRounding.AwayFromZero);
        }

        public void PreencherCamposImpostoIBSCBSComTributacaoDefinida(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            pedidoXMLNotaFiscal.CSTIBSCBS = cargaPedido.CSTIBSCBS;
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS = cargaPedido.ClassificacaoTributariaIBSCBS;

            pedidoXMLNotaFiscal.NBS = cargaPedido.NBS;
            pedidoXMLNotaFiscal.CodigoIndicadorOperacao = cargaPedido.CodigoIndicadorOperacao;

            pedidoXMLNotaFiscal.AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadual;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadual;

            pedidoXMLNotaFiscal.AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipal;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipal;

            pedidoXMLNotaFiscal.AliquotaCBS = cargaPedido.AliquotaCBS;
            pedidoXMLNotaFiscal.PercentualReducaoCBS = cargaPedido.PercentualReducaoCBS;
        }

        public void PreencherCamposImpostoIBSCBSComTributacaoDefinida(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao)
        {
            pedidoXMLNotaFiscal.CSTIBSCBS = pedidoCTeParaSubContratacao.CSTIBSCBS;
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS = pedidoCTeParaSubContratacao.ClassificacaoTributariaIBSCBS;

            pedidoXMLNotaFiscal.AliquotaIBSEstadual = pedidoCTeParaSubContratacao.AliquotaIBSEstadual;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual = pedidoCTeParaSubContratacao.PercentualReducaoIBSEstadual;

            pedidoXMLNotaFiscal.AliquotaIBSMunicipal = pedidoCTeParaSubContratacao.AliquotaIBSMunicipal;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal = pedidoCTeParaSubContratacao.PercentualReducaoIBSMunicipal;

            pedidoXMLNotaFiscal.AliquotaCBS = pedidoCTeParaSubContratacao.AliquotaCBS;
            pedidoXMLNotaFiscal.PercentualReducaoCBS = pedidoCTeParaSubContratacao.PercentualReducaoCBS;
        }

        public void PreencherCamposImpostoIBSCBSComTributacaoDefinida(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS)
        {
            pedidoXMLNotaFiscal.CSTIBSCBS = cargaPedido.CSTIBSCBS;
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS = cargaPedido.ClassificacaoTributariaIBSCBS;
            pedidoXMLNotaFiscal.BaseCalculoIBSCBS = baseCalculo;

            pedidoXMLNotaFiscal.AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadual;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadual;
            pedidoXMLNotaFiscal.ValorIBSEstadual = valorIBSEstadual;

            pedidoXMLNotaFiscal.AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipal;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipal;
            pedidoXMLNotaFiscal.ValorIBSMunicipal = valorIBSMunicipal;

            pedidoXMLNotaFiscal.AliquotaCBS = cargaPedido.AliquotaCBS;
            pedidoXMLNotaFiscal.PercentualReducaoCBS = cargaPedido.PercentualReducaoCBS;
            pedidoXMLNotaFiscal.ValorCBS = valorCBS;
        }

        public void PreencherCamposImpostoIBSCBSComTributacaoDefinidaFilialEmissora(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            pedidoXMLNotaFiscal.CSTIBSCBSFilialEmissora = cargaPedido.CSTIBSCBSFilialEmissora;
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBSFilialEmissora = cargaPedido.ClassificacaoTributariaIBSCBSFilialEmissora;

            pedidoXMLNotaFiscal.AliquotaIBSEstadualFilialEmissora = cargaPedido.AliquotaIBSEstadualFilialEmissora;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadualFilialEmissora = cargaPedido.PercentualReducaoIBSEstadualFilialEmissora;

            pedidoXMLNotaFiscal.AliquotaIBSMunicipalFilialEmissora = cargaPedido.AliquotaIBSMunicipalFilialEmissora;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipalFilialEmissora = cargaPedido.PercentualReducaoIBSMunicipalFilialEmissora;

            pedidoXMLNotaFiscal.AliquotaCBSFilialEmissora = cargaPedido.AliquotaCBSFilialEmissora;
            pedidoXMLNotaFiscal.PercentualReducaoCBSFilialEmissora = cargaPedido.PercentualReducaoCBSFilialEmissora;
        }

        public void PreencherCamposImpostoIBSCBSComTributacaoDefinidaFilialEmissora(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS)
        {
            pedidoXMLNotaFiscal.CSTIBSCBSFilialEmissora = cargaPedido.CSTIBSCBSFilialEmissora;
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBSFilialEmissora = cargaPedido.ClassificacaoTributariaIBSCBSFilialEmissora;
            pedidoXMLNotaFiscal.BaseCalculoIBSCBSFilialEmissora = baseCalculo;

            pedidoXMLNotaFiscal.AliquotaIBSEstadualFilialEmissora = cargaPedido.AliquotaIBSEstadualFilialEmissora;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadualFilialEmissora = cargaPedido.PercentualReducaoIBSEstadualFilialEmissora;
            pedidoXMLNotaFiscal.ValorIBSEstadualFilialEmissora = valorIBSEstadual;

            pedidoXMLNotaFiscal.AliquotaIBSMunicipalFilialEmissora = cargaPedido.AliquotaIBSMunicipalFilialEmissora;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipalFilialEmissora = cargaPedido.PercentualReducaoIBSMunicipalFilialEmissora;
            pedidoXMLNotaFiscal.ValorIBSMunicipalFilialEmissora = valorIBSMunicipal;

            pedidoXMLNotaFiscal.AliquotaCBSFilialEmissora = cargaPedido.AliquotaCBSFilialEmissora;
            pedidoXMLNotaFiscal.PercentualReducaoCBSFilialEmissora = cargaPedido.PercentualReducaoCBSFilialEmissora;
            pedidoXMLNotaFiscal.ValorCBSFilialEmissora = valorCBS;
        }

        public void PreencherValoresImpostoIBSCBSRateado(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS)
        {
            pedidoXMLNotaFiscal.BaseCalculoIBSCBS = baseCalculo;
            pedidoXMLNotaFiscal.ValorIBSEstadual = valorIBSEstadual;
            pedidoXMLNotaFiscal.ValorIBSMunicipal = valorIBSMunicipal;
            pedidoXMLNotaFiscal.ValorCBS = valorCBS;
        }

        public void PreencherValoresImpostoIBSCBSFilialEmissoraRateado(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS)
        {
            pedidoXMLNotaFiscal.BaseCalculoIBSCBSFilialEmissora = baseCalculo;
            pedidoXMLNotaFiscal.ValorIBSEstadualFilialEmissora = valorIBSEstadual;
            pedidoXMLNotaFiscal.ValorIBSMunicipalFilialEmissora = valorIBSMunicipal;
            pedidoXMLNotaFiscal.ValorCBSFilialEmissora = valorCBS;
        }

        public void AcrescentarValoresImpostoIBSCBS(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS)
        {
            pedidoXMLNotaFiscal.BaseCalculoIBSCBS += baseCalculo;
            pedidoXMLNotaFiscal.ValorIBSEstadual += valorIBSEstadual;
            pedidoXMLNotaFiscal.ValorIBSMunicipal += valorIBSMunicipal;
            pedidoXMLNotaFiscal.ValorCBS += valorCBS;
        }

        public void AcrescentarValoresImpostoIBSCBSFilialEmissora(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS)
        {
            pedidoXMLNotaFiscal.BaseCalculoIBSCBSFilialEmissora += baseCalculo;
            pedidoXMLNotaFiscal.ValorIBSEstadualFilialEmissora += valorIBSEstadual;
            pedidoXMLNotaFiscal.ValorIBSMunicipalFilialEmissora += valorIBSMunicipal;
            pedidoXMLNotaFiscal.ValorCBSFilialEmissora += valorCBS;
        }

        public void ZerarCamposImpostoIBSCBS(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool apenasValores = false)
        {
            if (apenasValores)
            {
                pedidoXMLNotaFiscal.BaseCalculoIBSCBS = 0m;
                pedidoXMLNotaFiscal.ValorIBSEstadual = 0m;
                pedidoXMLNotaFiscal.ValorIBSMunicipal = 0m;
                pedidoXMLNotaFiscal.ValorCBS = 0m;

                return;
            }

            pedidoXMLNotaFiscal.SetarRegraOutraAliquota(0);
            pedidoXMLNotaFiscal.CST = string.Empty;
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBS = string.Empty;
            pedidoXMLNotaFiscal.BaseCalculoIBSCBS = 0m;

            pedidoXMLNotaFiscal.AliquotaIBSEstadual = 0m;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadual = 0m;
            pedidoXMLNotaFiscal.ValorIBSEstadual = 0m;

            pedidoXMLNotaFiscal.AliquotaIBSMunicipal = 0m;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipal = 0m;
            pedidoXMLNotaFiscal.ValorIBSMunicipal = 0m;

            pedidoXMLNotaFiscal.AliquotaCBS = 0m;
            pedidoXMLNotaFiscal.PercentualReducaoCBS = 0m;
            pedidoXMLNotaFiscal.ValorCBS = 0m;
        }

        public void ZerarCamposImpostoIBSCBSFilialEmissora(Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, bool apenasValores = false)
        {
            if (apenasValores)
            {
                pedidoXMLNotaFiscal.BaseCalculoIBSCBS = 0m;
                pedidoXMLNotaFiscal.ValorIBSEstadual = 0m;
                pedidoXMLNotaFiscal.ValorIBSMunicipal = 0m;
                pedidoXMLNotaFiscal.ValorCBS = 0m;

                return;
            }

            pedidoXMLNotaFiscal.CSTFilialEmissora = string.Empty;
            pedidoXMLNotaFiscal.ClassificacaoTributariaIBSCBSFilialEmissora = string.Empty;
            pedidoXMLNotaFiscal.BaseCalculoIBSCBSFilialEmissora = 0m;

            pedidoXMLNotaFiscal.AliquotaIBSEstadualFilialEmissora = 0m;
            pedidoXMLNotaFiscal.PercentualReducaoIBSEstadualFilialEmissora = 0m;
            pedidoXMLNotaFiscal.ValorIBSEstadualFilialEmissora = 0m;

            pedidoXMLNotaFiscal.AliquotaIBSMunicipalFilialEmissora = 0m;
            pedidoXMLNotaFiscal.PercentualReducaoIBSMunicipalFilialEmissora = 0m;
            pedidoXMLNotaFiscal.ValorIBSMunicipalFilialEmissora = 0m;

            pedidoXMLNotaFiscal.AliquotaCBSFilialEmissora = 0m;
            pedidoXMLNotaFiscal.PercentualReducaoCBSFilialEmissora = 0m;
            pedidoXMLNotaFiscal.ValorCBSFilialEmissora = 0m;
        }

        #endregion

        #region Métodos Privados

        private void AtualizarProdutosCargaPedidoPorNotaFiscal(List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> produtos, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xMLNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosEmbarcador = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();

            foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produtoNFe in produtos)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();

                produtoEmbarcador.CodigoProduto = produtoNFe.Codigo;
                produtoEmbarcador.DescricaoProduto = produtoNFe.Descricao;
                produtoEmbarcador.CodigocEAN = produtoNFe.CodigoCEAN;
                produtoEmbarcador.UnidadeMedida = produtoNFe.UnidadeComercial;
                produtoEmbarcador.Quantidade = produtoNFe.QuantidadeComercial;
                produtoEmbarcador.NumeroPedidoCompra = produtoNFe.NumeroPedidoCompra;
                produtoEmbarcador.ValorUnitario = produtoNFe.ValorUnitarioComercial;
                produtoEmbarcador.ProdutoLotes = produtoNFe.Lotes;
                produtoEmbarcador.CodigoNCM = produtoNFe.NCM;
                produtoEmbarcador.CSTICMS = produtoNFe.CSTICMS;
                produtoEmbarcador.OrigemMercadoria = produtoNFe.OrigemMercadoria;
                produtoEmbarcador.CodigoNFCI = produtoNFe.CodigoNFCI;
                produtoEmbarcador.CodigoEAN = produtoNFe.CodigoCEAN;

                if (produtoNFe.QuantidadeTributaria > 0)
                {
                    produtoEmbarcador.QuantidadePorCaixa = produtoNFe.QuantidadeComercial > 0 ? (int)(produtoNFe.QuantidadeTributaria / produtoNFe.QuantidadeComercial) : (int)(produtoNFe.QuantidadeTributaria);
                    produtoEmbarcador.Atualizar = true;
                }

                produtosEmbarcador.Add(produtoEmbarcador);
            }

            PreencherNotaProduto(produtosEmbarcador, xMLNotaFiscal, pedido, Auditado, tipoServicoMultisoftware);
        }

        private void PreencherNotaProduto(List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosEmbarcador, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (xmlNotaFiscal == null)
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(_unitOfWork);

            if (repositorioXMLNotaFiscalProduto.VerificarExistePorPedido(pedido?.Codigo ?? 0, xmlNotaFiscal.Codigo))
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProdutoLote repositorioXMLNotaFiscalProdutoLote = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProdutoLote(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProdutoLote repositorioPedidoProdutoLote = new Repositorio.Embarcador.Pedidos.PedidoProdutoLote(_unitOfWork);
            Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor repositorioUnidadeMedidaFornecedor = new Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor(_unitOfWork);
            Repositorio.Produto repositorioProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.ProdutoFornecedor repositorioProdutoFornecedor = new Repositorio.ProdutoFornecedor(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();
            bool utilizarPesoProdutoParaCalcularPesoCarga = configuracaoGeralCarga?.UtilizarPesoProdutoParaCalcularPesoCarga ?? false;
            int grupoPessoa = xmlNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ? (xmlNotaFiscal.Emitente.GrupoPessoas?.Codigo ?? 0) : (xmlNotaFiscal.Destinatario.GrupoPessoas?.Codigo ?? 0);
            decimal pesoProdutos = 0m;
            decimal pesoLiquidoProdutos = 0m;

            Servicos.Embarcador.NotaFiscal.NotaFiscalProduto servicoNotaFiscalProduto = new Embarcador.NotaFiscal.NotaFiscalProduto(_unitOfWork);
            Servicos.ProdutoEmbarcador servicoProdutoEmbarcador = new Servicos.ProdutoEmbarcador(_unitOfWork, configuracaoGeralCarga);

            foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtocargaIntegracao in produtosEmbarcador)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto xmlNotaFiscalProduto = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto();

                if (configuracao.FatorMetroCubicoProdutoEmbarcadorIntegracao != 1)
                {
                    produtocargaIntegracao.QuantidadePorCaixa = 0;
                    produtocargaIntegracao.Atualizar = false;
                }

                xmlNotaFiscalProduto.XMLNotaFiscal = xmlNotaFiscal;
                xmlNotaFiscalProduto.Produto = servicoProdutoEmbarcador.IntegrarProduto(produtocargaIntegracao.CodigoProduto, produtocargaIntegracao.CodigocEAN, grupoPessoa, produtocargaIntegracao.DescricaoProduto, produtocargaIntegracao.PesoUnitario, null, produtocargaIntegracao.MetroCubito, Auditado, produtocargaIntegracao.CodigoDocumentacao, produtocargaIntegracao.InativarCadastro, produtocargaIntegracao.Atualizar, produtocargaIntegracao.CodigoNCM, tipoServicoMultisoftware, produtocargaIntegracao.QuantidadePorCaixa, produtocargaIntegracao.SiglaUnidade, produtocargaIntegracao.TemperaturaTransporte, produtocargaIntegracao.PesoLiquidoUnitario, produtocargaIntegracao.QtdPalet, produtocargaIntegracao.AlturaCM, produtocargaIntegracao.LarguraCM, produtocargaIntegracao.ComprimentoCM, produtocargaIntegracao.Observacao, produtocargaIntegracao.QuantidadeCaixaPorPallet);
                xmlNotaFiscalProduto.UnidadeMedida = produtocargaIntegracao.UnidadeMedida;
                xmlNotaFiscalProduto.ValorProduto = produtocargaIntegracao.ValorUnitario;
                xmlNotaFiscalProduto.Quantidade = produtocargaIntegracao.Quantidade;
                xmlNotaFiscalProduto.CST = produtocargaIntegracao.CSTICMS;
                xmlNotaFiscalProduto.Origem = produtocargaIntegracao.OrigemMercadoria;
                xmlNotaFiscalProduto.CodigoNFCI = produtocargaIntegracao.CodigoNFCI;
                xmlNotaFiscalProduto.CodigoEAN = produtocargaIntegracao.CodigoEAN;
                xmlNotaFiscalProduto.NumeroPedidoCompra = produtocargaIntegracao.NumeroPedidoCompra;
                xmlNotaFiscalProduto.Pedido = pedido;
                xmlNotaFiscalProduto.NCM = produtocargaIntegracao.CodigoNCM;
                xmlNotaFiscalProduto.cProd = produtocargaIntegracao.CodigoProduto;

                Dominio.Entidades.ProdutoFornecedor produtoFornecedor = repositorioProdutoFornecedor.BuscarPorProdutoEFornecedor(produtocargaIntegracao.CodigoProduto, xmlNotaFiscal.Emitente.CPF_CNPJ);

                if (produtoFornecedor == null || produtoFornecedor.Produto == null)
                {
                    Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor unidadeMedida = repositorioUnidadeMedidaFornecedor.BuscarPorDescricao(produtocargaIntegracao.UnidadeMedida, 0);
                    UnidadeDeMedida unidadeFornecedor = unidadeMedida?.UnidadeDeMedida ?? UnidadeDeMedida.Unidade;
                    int codigoProduto = servicoNotaFiscalProduto.CadastrarProduto(produtocargaIntegracao.DescricaoProduto, produtocargaIntegracao.CodigoProduto, produtocargaIntegracao.CodigoNCM, unidadeFornecedor, 0, _unitOfWork, tipoServicoMultisoftware, configuracao);

                    xmlNotaFiscalProduto.ProdutoInterno = repositorioProduto.BuscarPorCodigo(codigoProduto);

                    produtoFornecedor = new Dominio.Entidades.ProdutoFornecedor()
                    {
                        CodigoProduto = produtocargaIntegracao.CodigoProduto,
                        Fornecedor = xmlNotaFiscal.Emitente,
                        Produto = xmlNotaFiscalProduto.ProdutoInterno,
                        FatorConversao = 0
                    };

                    repositorioProdutoFornecedor.Inserir(produtoFornecedor);
                }
                else
                    xmlNotaFiscalProduto.ProdutoInterno = produtoFornecedor.Produto;

                repositorioXMLNotaFiscalProduto.Inserir(xmlNotaFiscalProduto);

                if (produtocargaIntegracao.ProdutoLotes != null)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Lote produtoLote in produtocargaIntegracao.ProdutoLotes)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote xmlNotaFiscalProdutoLote = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProdutoLote();

                        xmlNotaFiscalProdutoLote.XMLNotaFiscalProduto = xmlNotaFiscalProduto;
                        xmlNotaFiscalProdutoLote.NumeroLote = produtoLote.NumeroLote;
                        xmlNotaFiscalProdutoLote.DataFabricacao = produtoLote.DataFabricacao;
                        xmlNotaFiscalProdutoLote.DataValidade = produtoLote.DataValidade;
                        xmlNotaFiscalProdutoLote.Quantidade = produtoLote.QuantidadeLote;

                        repositorioXMLNotaFiscalProdutoLote.Inserir(xmlNotaFiscalProdutoLote);
                    }
                }

                pesoProdutos += xmlNotaFiscalProduto.PesoProduto;
                pesoLiquidoProdutos += xmlNotaFiscalProduto.PesoLiquidoProduto;
            }

            if (utilizarPesoProdutoParaCalcularPesoCarga)
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

                xmlNotaFiscal.Peso = pesoProdutos;
                xmlNotaFiscal.PesoLiquido = pesoLiquidoProdutos;
                xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;

                repositorioXMLNotaFiscal.Atualizar(xmlNotaFiscal);
            }
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga ObterConfiguracaoGeralCarga()
        {
            if (_configuracaoGeralCarga == null)
                _configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoGeralCarga;
        }

        #endregion
    }
}
