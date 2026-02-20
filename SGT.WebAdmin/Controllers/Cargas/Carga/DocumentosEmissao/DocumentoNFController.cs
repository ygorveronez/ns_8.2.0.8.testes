using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    [CustomAuthorize(new string[] { "DownloadDANFE", "DownloadXml" }, "Cargas/Carga", "Logistica/JanelaCarregamento", "Logistica/AgendamentoColeta", "DocumentoNF/Pesquisa", "Documentos/DocumentoDestinadoEmpresa")]
    public class DocumentoNFController : BaseController
    {
        #region Construtores

        public DocumentoNFController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repoDocDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                //a carga mercosul na etapa da nfe tera duas consultas de nota, uma factura e outra nota fiscal, essa flag diferencia qual delas esta
                bool etapaNfMecosul = Request.GetBoolParam("EtapaNfMercosul");
                bool? cargaMercosul = (!etapaNfMecosul && (carga.TipoOperacao?.TipoOperacaoMercosul ?? false));
                bool sistemaMultimodal = ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal;
                bool retornarSituacoesNaoConformidadeNotasFiscais = (carga.TipoOperacao?.ConfiguracaoCarga?.AtivoModuloNaoConformidades ?? false);

                if (carga?.TipoOperacao?.ConfiguracaoCarga?.TipoOperacaoInternacional ?? false)
                    cargaMercosul = null;

                Servicos.Embarcador.Carga.OcultarInformacoesCarga.OcultarInformacoesCarga serOcultarInformacoesCarga = new Servicos.Embarcador.Carga.OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);

                bool possuiOcultarInformacoesCarga = serOcultarInformacoesCarga.PossuiOcultarInformacoesCarga(this.Usuario.Codigo);
                Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga = null;
                if (possuiOcultarInformacoesCarga)
                    ocultarInformacoesCarga = serOcultarInformacoesCarga.ObterOcultarInformacoesCarga(this.Usuario.Codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CTesEmitidos", false);
                grid.AdicionarCabecalho("PossuiCartaCorrecao", false);
                grid.AdicionarCabecalho("Número", "Numero", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Número Pedido NF-e", "NumeroPedidoEmbarcador", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Modelo", "Modelo", 4, Models.Grid.Align.center, true);

                if (!cargaMercosul.HasValue || !cargaMercosul.Value)
                {
                    if (sistemaMultimodal)
                        grid.AdicionarCabecalho("Chave / Descrição", "Chave", 15, Models.Grid.Align.left, false, true, true, false, true);
                    else
                        grid.AdicionarCabecalho("Chave", "Chave", 15, Models.Grid.Align.left, false, true, true, false, true);
                }

                if (retornarSituacoesNaoConformidadeNotasFiscais)
                    grid.AdicionarCabecalho("Não Conformidade", "SituacaoNaoConformidade", 14, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Data Emissao", "DataEmissao", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emitente", "Emitente", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modalidade Pgto.", "DescricaoModalidadeFrete", 5, Models.Grid.Align.center, true);

                if (ConfiguracaoEmbarcador.ExibirClassificacaoNFe)
                    grid.AdicionarCabecalho("Classificação NFe", "ClassificacaoNFe", 5, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Peso", "Peso", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Peso Cubado", "PesoCubado", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Peso Liquido", "PesoLiquido", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 6, Models.Grid.Align.left, true);

                if (carga.TabelaFrete != null && carga.TabelaFrete.TipoCalculo == TipoCalculoTabelaFrete.PorDocumentoEmitido && carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe)
                {
                    grid.AdicionarCabecalho("Frete Embarcador", "ValorFreteEmbarcador", 6, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Frete Tabela", "ValorFreteTabelaFrete", 6, Models.Grid.Align.left, true);
                }
                else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador)
                    grid.AdicionarCabecalho("Frete", "ValorFrete", 6, Models.Grid.Align.left, true);

                if (sistemaMultimodal)
                {
                    grid.AdicionarCabecalho("NCM", "NCM", 3, Models.Grid.Align.left, false, true, true, false, true);
                    grid.AdicionarCabecalho("CFOP", "CFOP", 3, Models.Grid.Align.left, false, true, true, false, true);
                    grid.AdicionarCabecalho("PIN SUF.", "PINSUFRAMA", 4, Models.Grid.Align.left, false, true, true, false, true);
                    grid.AdicionarCabecalho("Forma Associação NF", "FormaIntegracao", 5, Models.Grid.Align.left, false, true, true, false, true);
                }
                else
                {
                    grid.AdicionarCabecalho("NCM", false);
                    grid.AdicionarCabecalho("CFOP", false);
                    grid.AdicionarCabecalho("PINSUFRAMA", false);
                    grid.AdicionarCabecalho("FormaIntegracao", false);
                }

                grid.AdicionarCabecalho("NumeroOutroDocumento", false);

                bool nfAtivas = (carga.SituacaoCarga != SituacaoCarga.Cancelada);
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar != "ValorFreteTabelaFrete")
                {
                    if (propOrdenar == "Emitente" || propOrdenar == "Destinatario")
                        propOrdenar += ".Nome";
                    else if (propOrdenar == "Destino")
                        propOrdenar = "Destinatario.Localidade.Descricao";
                    else if (propOrdenar == "DescricaoModalidadeFrete")
                        propOrdenar = "ModalidadeFrete";

                    propOrdenar = "XMLNotaFiscal." + propOrdenar;
                }

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                int totalRegistros = repositorioPedidoXMLNotaFiscal.ContarPorCargaPedido(codigoCargaPedido, nfAtivas, cargaMercosul);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotasFiscais;
                List<(int CodigoXmlNotaFiscal, SituacaoNaoConformidade Situacao)> situacoesNaoConformidadeNotasFiscal;
                List<dynamic> notasFiscaisRetornar = new List<dynamic>();

                if (totalRegistros > 0)
                {
                    pedidosXMLNotasFiscais = repositorioPedidoXMLNotaFiscal.ConsultarPorCargaPedido(codigoCargaPedido, nfAtivas, cargaMercosul, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                    if (retornarSituacoesNaoConformidadeNotasFiscais)
                        situacoesNaoConformidadeNotasFiscal = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork).BuscarSituacoesNotasFiscaisPorCarga(carga.Codigo);
                    else
                        situacoesNaoConformidadeNotasFiscal = new List<(int CodigoXmlNotaFiscal, SituacaoNaoConformidade Situacao)>();
                }
                else
                {
                    pedidosXMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                    situacoesNaoConformidadeNotasFiscal = new List<(int CodigoXmlNotaFiscal, SituacaoNaoConformidade Situacao)>();
                }

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidosXMLNotasFiscais)
                {
                    string situacaoNaoConformidade = string.Empty;

                    if (retornarSituacoesNaoConformidadeNotasFiscais)
                    {
                        if (situacoesNaoConformidadeNotasFiscal.Any(o => o.CodigoXmlNotaFiscal == pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo && o.Situacao == SituacaoNaoConformidade.AguardandoTratativa))
                            situacaoNaoConformidade = SituacaoNaoConformidade.AguardandoTratativa.ObterDescricao();
                        else if (situacoesNaoConformidadeNotasFiscal.Any(o => o.CodigoXmlNotaFiscal == pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo && o.Situacao == SituacaoNaoConformidade.SemRegraAprovacao))
                            situacaoNaoConformidade = SituacaoNaoConformidade.SemRegraAprovacao.ObterDescricao();
                        else if (situacoesNaoConformidadeNotasFiscal.Any(o => o.CodigoXmlNotaFiscal == pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo && o.Situacao == SituacaoNaoConformidade.Reprovada))
                            situacaoNaoConformidade = SituacaoNaoConformidade.Reprovada.ObterDescricao();
                        else if (situacoesNaoConformidadeNotasFiscal.Count > 0)
                            situacaoNaoConformidade = "Liberada";
                    }

                    bool notaPossuiCartaCorrecao = pedidoXMLNotaFiscal.XMLNotaFiscal.Chave == "" ? false : repoDocDestinadoEmpresa.VerificarNotaCartaCorrecaoEmitente(pedidoXMLNotaFiscal.XMLNotaFiscal.Chave);

                    notasFiscaisRetornar.Add(new
                    {
                        pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo,
                        pedidoXMLNotaFiscal.CargaPedido.CTesEmitidos,
                        pedidoXMLNotaFiscal.XMLNotaFiscal.DataEmissao,
                        pedidoXMLNotaFiscal.XMLNotaFiscal.Numero,
                        pedidoXMLNotaFiscal.XMLNotaFiscal.NumeroPedidoEmbarcador,
                        pedidoXMLNotaFiscal.XMLNotaFiscal.Chave,
                        Emitente = pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente != null ? pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.Descricao : "",
                        Origem = pedidoXMLNotaFiscal.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ? pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente != null ? pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.Localidade.DescricaoCidadeEstado : "" : pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario != null ? pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.Localidade.DescricaoCidadeEstado : "",
                        Destinatario = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario != null ? pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.Descricao : "",
                        Destino = pedidoXMLNotaFiscal.XMLNotaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ? pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario != null ? pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.Localidade.DescricaoCidadeEstado : "" : pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente != null ? pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.Localidade.DescricaoCidadeEstado : "",
                        Valor = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, pedidoXMLNotaFiscal.XMLNotaFiscal.Valor) : pedidoXMLNotaFiscal.XMLNotaFiscal.Valor,
                        Peso = pedidoXMLNotaFiscal.XMLNotaFiscal.Peso.ToString("n3"),
                        PesoCubado = pedidoXMLNotaFiscal.XMLNotaFiscal.PesoCubado.ToString("n3"),
                        pedidoXMLNotaFiscal.XMLNotaFiscal.ValorFrete,
                        pedidoXMLNotaFiscal.XMLNotaFiscal.ValorFreteEmbarcador,
                        pedidoXMLNotaFiscal.ValorFreteTabelaFrete,
                        pedidoXMLNotaFiscal.XMLNotaFiscal.DescricaoModalidadeFrete,
                        pedidoXMLNotaFiscal.XMLNotaFiscal.NCM,
                        pedidoXMLNotaFiscal.XMLNotaFiscal.CFOP,
                        pedidoXMLNotaFiscal.XMLNotaFiscal.PINSUFRAMA,
                        pedidoXMLNotaFiscal.XMLNotaFiscal.NumeroOutroDocumento,
                        PesoLiquido = pedidoXMLNotaFiscal.XMLNotaFiscal.PesoLiquido,
                        ClassificacaoNFe = pedidoXMLNotaFiscal.XMLNotaFiscal.ClassificacaoNFe?.ObterDescricao() ?? string.Empty,
                        SituacaoNaoConformidade = situacaoNaoConformidade,
                        DT_RowColor = (
                            pedidoXMLNotaFiscal.XMLNotaFiscal.CanceladaPeloEmitente ? "rgba(193, 101, 101, 1)" :
                            sistemaMultimodal && (string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente?.CodigoIntegracao) || string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario?.CodigoIntegracao)) ? "rgba(193, 101, 101, 1)" :
                            pedidoXMLNotaFiscal.XMLNotaFiscal.TipoEmissao == TipoEmissaoNotaFiscal.ContingenciaFSDA || pedidoXMLNotaFiscal.XMLNotaFiscal.TipoEmissao == TipoEmissaoNotaFiscal.ContingenciaFSIA || notaPossuiCartaCorrecao ? CorGrid.Yellow : pedidoXMLNotaFiscal.NotaFiscalEmOutraCarga ? "#ffd6b8" : ""
                        ),
                        DT_FontColor = (
                            pedidoXMLNotaFiscal.XMLNotaFiscal.CanceladaPeloEmitente ? "#FFFFFF" :
                            sistemaMultimodal && (string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente?.CodigoIntegracao) || string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario?.CodigoIntegracao)) ? "#FFFFFF" : ""
                        ),
                        PossuiCartaCorrecao = notaPossuiCartaCorrecao,
                        FormaIntegracao = pedidoXMLNotaFiscal.XMLNotaFiscal.FormaIntegracao.HasValue ? pedidoXMLNotaFiscal.XMLNotaFiscal.FormaIntegracao.Value.ObterDescricao() : "Manual",
                        pedidoXMLNotaFiscal.XMLNotaFiscal.Modelo
                    });
                }

                grid.AdicionaRows(notasFiscaisRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CTesVinculadosOSMae()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoAdicional repPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);
                Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repPedidoAdicional.BuscarPorPedido(cargaPedido?.Pedido?.Codigo ?? 0);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Chave", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Nº NF", "NotasFiscais", 8, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", 20, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 10, Models.Grid.Align.right, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "Origem")
                    propOrdenacao = "LocalidadeInicioPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "DescricaoTipoServico")
                    propOrdenacao = "TipoServico";

                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.ConsultarPorNumeroOS(codigoCargaPedido, pedidoAdicional?.NumeroOSMae ?? "", propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCTe.ContarPorNumeroOS(codigoCargaPedido, pedidoAdicional?.NumeroOSMae ?? ""));

                var lista = (from obj in ctes
                             select new
                             {
                                 obj.Codigo,
                                 obj.Chave,
                                 obj.DescricaoTipoServico,
                                 NumeroModeloDocumentoFiscal = obj.ModeloDocumentoFiscal.Numero,
                                 AbreviacaoModeloDocumentoFiscal = obj.ModeloDocumentoFiscal.Abreviacao,
                                 CodigoEmpresa = obj.Empresa.Codigo,
                                 obj.Numero,
                                 SituacaoCTe = obj.Status,
                                 Serie = obj.Serie.Numero,
                                 obj.DescricaoTipoPagamento,
                                 //Motorista = BuscarMotoristas(obj.Motoristas.ToList()),
                                 //Veiculo = BuscarPlacas(obj.Veiculos.ToList()),
                                 GrupoPessoas = obj.TomadorPagador?.GrupoPessoas?.Descricao ?? string.Empty,
                                 Remetente = obj.Remetente != null ? obj.Remetente.Nome + "(" + obj.Remetente.CPF_CNPJ_Formatado + ")" : "",
                                 Destinatario = obj.Destinatario != null ? obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")" : "",
                                 Origem = obj.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                 Destino = obj.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                 ValorFrete = obj.ValorAReceber.ToString("n2"),
                                 Aliquota = obj.AliquotaICMS.ToString("n2"),
                                 Observacao = obj.ObservacoesGerais,
                                 NotasFiscais = obj.NumeroNotas,
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaEspelhoIntercement()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");

                Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement repPedidoEspelhoIntercement = new Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("FKNum", "FKNUM", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("REbel", "REBEL", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("SIGni", "SIGNI", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("VBeln", "VBELN", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tarifa", "TARIFA", 10, Models.Grid.Align.right, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement> espelhoIntercements = repPedidoEspelhoIntercement.ConsultarPorCargaPedido(codigoCargaPedido, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPedidoEspelhoIntercement.ContarPorCargaPedido(codigoCargaPedido));

                var dynespelhoIntercements = (from obj in espelhoIntercements
                                              select new
                                              {
                                                  obj.Codigo,
                                                  obj.FKNUM,
                                                  obj.REBEL,
                                                  obj.SIGNI,
                                                  obj.VBELN,
                                                  TARIFA = obj.TARIFA.ToString("n2")
                                              }).ToList();

                grid.AdicionaRows(dynespelhoIntercements);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNotasCompativeis(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");
                int codCarga = Request.GetIntParam("Carga");

                DateTime dataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial");
                DateTime dataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal");
                int numeroNotaInicial = Request.GetIntParam("NumeroNotaInicial");
                int numeroNotaFinal = Request.GetIntParam("NumeroNotaFinal");
                bool possuiIntegracaoMichelin = Request.GetBoolParam("PossuiIntegracaoMichelin");
                string numeroCarregamento = Request.GetStringParam("NumeroCarregamento");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork, cancellationToken);


                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codCarga);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repTipoIntegracao.BuscarPorTipoAsync(TipoIntegracao.Minerva);

                if (tipoIntegracao != null && carga != null && (carga.TipoOperacao?.PermitirSelecionarNotasCompativeis ?? false) && (carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNota = await repositorioCargaPedido.BuscarPorCodigoAsync(codigoCargaPedido);

                    Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva servicoMinerva = new Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva(unitOfWork, cancellationToken);
                    await servicoMinerva.BuscarNotasFiscaisPorCNPJAsync(cargaPedidoNota.Pedido.Remetente.CPF_CNPJ_SemFormato, carga.Filial?.CodigoFilialEmbarcador, carga.TipoOperacao?.CodigoIntegracao, carga.DataCriacaoCarga.Date.AddDays(-7), DateTime.Today, ConfiguracaoEmbarcador);
                }


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarPorCodigoAsync(codigoCargaPedido);

                if (possuiIntegracaoMichelin)
                    grid.AdicionarCabecalho("Trip", "NumeroCarregamento", 8, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("NumeroCarregamento", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Chave", "Chave", 15, Models.Grid.Align.left, false, true, true, false, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 17, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 13, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Peso", "Peso", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 6, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                if (propOrdenar != "ValorFreteTabelaFrete")
                {
                    if (propOrdenar == "Emitente" || propOrdenar == "Destinatario")
                        propOrdenar += ".Nome";
                    else if (propOrdenar == "Destino")
                        propOrdenar = "Destinatario.Localidade.Descricao";
                    else if (propOrdenar == "DescricaoModalidadeFrete")
                        propOrdenar = "ModalidadeFrete";
                }

                string numeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador;

                if (!string.IsNullOrWhiteSpace(numeroPedido) && numeroPedido.Contains("_"))
                    numeroPedido = numeroPedido.Split('_')[0];

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador &&
                    (cargaPedido.Carga.TipoOperacao?.PermitirSelecionarNotasCompativeis ?? false))
                    numeroPedido = string.Empty;

                if (((carga.TipoOperacao?.PermitirSelecionarNotasCompativeis ?? false) && (carga.TipoOperacao?.PermiteImportarDocumentosManualmente ?? false)) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && ConfiguracaoGeralCarga.PadraoVisualizacaoOperadorLogistico))
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotaFiscal = cargaPedido.Pedido.Remetente != null ? repPedidoXMLNotaFiscal.ConsultarPorEmitenteEDestinoCargaPedido(numeroCarregamento, ConfiguracaoEmbarcador?.FiltrarNotasCompativeisPeloDestinatario ?? false, numeroPedido, numeroNotaInicial, numeroNotaFinal, cargaPedido.Pedido.Remetente.CPF_CNPJ, cargaPedido.Pedido.Destinatario?.CPF_CNPJ ?? 0, dataEmissaoInicial, dataEmissaoFinal, TipoServicoMultisoftware, ConfiguracaoGeralCarga.PadraoVisualizacaoOperadorLogistico, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                    grid.setarQuantidadeTotal(cargaPedido.Pedido.Remetente != null ? repPedidoXMLNotaFiscal.ContarPorEmitenteEDestinatarioCargaPedido(numeroCarregamento, ConfiguracaoEmbarcador?.FiltrarNotasCompativeisPeloDestinatario ?? false, numeroPedido, numeroNotaInicial, numeroNotaFinal, cargaPedido.Pedido.Remetente.CPF_CNPJ, cargaPedido.Pedido.Destinatario?.CPF_CNPJ ?? 0, dataEmissaoInicial, dataEmissaoFinal, TipoServicoMultisoftware, ConfiguracaoGeralCarga.PadraoVisualizacaoOperadorLogistico) : 0);

                    var dynXmlNotaFiscal = (from obj in xmlNotaFiscal
                                            select new
                                            {
                                                obj.Codigo,
                                                obj.NumeroCarregamento,
                                                obj.Numero,
                                                obj.Chave,
                                                Emitente = obj.Emitente != null ? obj.Emitente.Descricao : "",
                                                Destinatario = obj.Destinatario != null ? obj.Destinatario.Descricao : "",
                                                Destino = obj.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ? obj.Destinatario != null ? obj.Destinatario.Localidade.DescricaoCidadeEstado : "" : obj.Emitente != null ? obj.Emitente.Localidade.DescricaoCidadeEstado : "",
                                                obj.Valor,
                                                Peso = obj.Peso.ToString("n3"),
                                                obj.ValorFrete,
                                                obj.DescricaoModalidadeFrete,
                                                DT_RowColor = obj.CanceladaPeloEmitente ? "rgba(193, 101, 101, 1)" : "",
                                                DT_FontColor = obj.CanceladaPeloEmitente ? "#FFFFFF" : ""
                                            }).ToList();

                    grid.AdicionaRows(dynXmlNotaFiscal);
                }
                else
                {
                    grid.AdicionaRows(new List<dynamic>());
                    grid.setarQuantidadeTotal(0);
                }

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNotasCompativeisApenasPorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                int numeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal");
                string chaveNotaFiscal = Request.GetStringParam("ChaveNotaFiscal");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Chave", "Chave", 17, Models.Grid.Align.left, false, true, true, false, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 17, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Peso", "Peso", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 6, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados cargaDadosSumarizados = repCargaDadosSumarizados.BuscarPorCargaComFetch(codigoCarga);
                List<int> codigosOrigens = cargaDadosSumarizados.ClientesRemetentes.Select(o => o.Localidade.Codigo).ToList();

                if (propOrdenar == "Destinatario")
                    propOrdenar += ".Nome";

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotaFiscal = repPedidoXMLNotaFiscal.ConsultarPorOrigemDaCarga(codigosOrigens, numeroNotaFiscal, chaveNotaFiscal, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPedidoXMLNotaFiscal.ContarPorOrigemDaCarga(codigosOrigens, numeroNotaFiscal, chaveNotaFiscal));

                var dynXmlNotaFiscal = (from obj in xmlNotaFiscal
                                        select new
                                        {
                                            obj.Codigo,
                                            obj.Numero,
                                            obj.Chave,
                                            Destinatario = obj.Destinatario?.Descricao ?? "",
                                            Destino = obj.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ? obj.Destinatario != null ? obj.Destinatario.Localidade.DescricaoCidadeEstado : "" : obj.Emitente != null ? obj.Emitente.Localidade.DescricaoCidadeEstado : "",
                                            obj.Valor,
                                            Peso = obj.Peso.ToString("n3"),
                                            DT_RowColor = obj.CanceladaPeloEmitente ? "rgba(193, 101, 101, 1)" : "",
                                            DT_FontColor = obj.CanceladaPeloEmitente ? "#FFFFFF" : ""
                                        }).ToList();

                grid.AdicionaRows(dynXmlNotaFiscal);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXMLNotaFiscal.BuscarPorCodigo(codigo);

                if (xmlNotaFiscal == null)
                    return new JsonpResult(false, true, "Nota fiscal não localizada ou inativa no sistema.");

                var retorno = new
                {
                    xmlNotaFiscal.Codigo,
                    Peso = xmlNotaFiscal.Peso.ToString("n3"),
                    PesoLiquido = xmlNotaFiscal.PesoLiquido.ToString("n3"),
                    PesoCubado = xmlNotaFiscal.PesoCubado.ToString("n2"),
                    PesoPaletizado = xmlNotaFiscal.PesoPaletizado.ToString("n2"),
                    MetrosCubicos = xmlNotaFiscal.MetrosCubicos.ToString("n6"),
                    Pallets = xmlNotaFiscal.QuantidadePallets.ToString("n3"),
                    Volumes = xmlNotaFiscal.Volumes.ToString(),
                    ValorFrete = xmlNotaFiscal.ValorFrete.ToString("n2"),
                    xmlNotaFiscal.NCM,
                    xmlNotaFiscal.CFOP,
                    xmlNotaFiscal.TipoFatura,
                    xmlNotaFiscal.NumeroControleCliente,
                    xmlNotaFiscal.NumeroReferenciaEDI,
                    xmlNotaFiscal.Embarque,
                    xmlNotaFiscal.MasterBL,
                    xmlNotaFiscal.NumeroDI,
                    xmlNotaFiscal.NumeroCanhoto,
                    xmlNotaFiscal.PINSUFRAMA,
                    xmlNotaFiscal.NumeroOutroDocumento,
                    Dimensoes = (from obj in xmlNotaFiscal.Dimensoes
                                 select new
                                 {
                                     Altura = new { val = obj.Altura, tipo = "decimal", configDecimal = new { precision = 3 } },
                                     obj.Codigo,
                                     Comprimento = new { val = obj.Comprimento, tipo = "decimal", configDecimal = new { precision = 3 } },
                                     Largura = new { val = obj.Largura, tipo = "decimal", configDecimal = new { precision = 3 } },
                                     MetrosCubicos = new { val = obj.MetrosCubicos, tipo = "decimal", configDecimal = new { precision = 6 } },
                                     Volumes = obj.Volumes.ToString()
                                 }).ToList(),
                    Expedidor = new
                    {
                        Codigo = xmlNotaFiscal.Expedidor?.CPF_CNPJ ?? 0,
                        Descricao = xmlNotaFiscal.Expedidor?.Descricao ?? string.Empty
                    },
                    Recebedor = new
                    {
                        Codigo = xmlNotaFiscal.Recebedor?.CPF_CNPJ ?? 0,
                        Descricao = xmlNotaFiscal.Recebedor?.Descricao ?? string.Empty
                    },
                    Destinatario = new
                    {
                        Codigo = xmlNotaFiscal.Destinatario?.CPF_CNPJ ?? 0,
                        Descricao = xmlNotaFiscal.Destinatario?.Descricao ?? string.Empty
                    },
                    Remetente = new
                    {
                        Codigo = xmlNotaFiscal.Emitente?.CPF_CNPJ ?? 0,
                        Descricao = xmlNotaFiscal.Emitente?.Descricao ?? string.Empty
                    },
                    xmlNotaFiscal.ClassificacaoNFe,
                    xmlNotaFiscal.SegundoCodigoBarras,
                    xmlNotaFiscal.ChaveVenda,
                    xmlNotaFiscal.TipoEmissao,
                    xmlNotaFiscal.PalletsControle,
                    ValorNotaFiscal = xmlNotaFiscal.Valor.ToString("n2"),
                    ValorTotalProdutos = xmlNotaFiscal.ValorTotalProdutos.ToString("n2"),
                    xmlNotaFiscal.TipoDocumento,
                    FacturaFake = xmlNotaFiscal.FaturaFake
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes da nota fiscal.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VincularNotas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoCargaPedido = Request.GetIntParam("CargaPedido");
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                bool transportadorPassa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && (cargaPedido?.Carga?.TipoOperacao?.PermitirTransportadorInformeNotasCompativeis ?? false);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais) && !this.Usuario.UsuarioAdministrador && !transportadorPassa)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfigGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfigGeralCarga.BuscarPrimeiroRegistro();

                if (cargaPedido.Carga.CargaTransbordo)
                    return new JsonpResult(false, true, "Não é possível adicionar notas fiscais em cargas de transbordo.");

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                ClassificacaoNFe? classificacaoNFe = Request.GetNullableEnumParam<ClassificacaoNFe>("ClassificacaoNFe");

                #region Filtros de notas compatíveis

                DateTime dataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial");
                DateTime dataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal");
                List<int> codigosDocumentos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Documentos"));
                string numeroCarregamento = Request.GetStringParam("NumeroCarregamento");
                int numeroNotaInicial = Request.GetIntParam("NumeroNotaInicial");
                int numeroNotaFinal = Request.GetIntParam("NumeroNotaFinal");

                bool.TryParse(Request.Params("SelecionarTodos"), out bool selecionarTodos);

                string numeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador;

                if (!string.IsNullOrWhiteSpace(numeroPedido) && numeroPedido.Contains("_"))
                    numeroPedido = numeroPedido.Split('_')[0];

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador &&
                    (cargaPedido.Carga.TipoOperacao?.PermitirSelecionarNotasCompativeis ?? false))
                    numeroPedido = string.Empty;

                #endregion

                List<int> listaCodigosDocumentos = cargaPedido.Pedido.Remetente != null ? repPedidoXMLNotaFiscal.ObterCodigosDocumentosPorEmitenteEDestinoCargaPedido(numeroCarregamento, selecionarTodos, codigosDocumentos, configuracao?.FiltrarNotasCompativeisPeloDestinatario ?? false, numeroPedido, numeroNotaInicial, numeroNotaFinal, cargaPedido.Pedido.Remetente.CPF_CNPJ, cargaPedido.Pedido.Destinatario?.CPF_CNPJ ?? 0, dataEmissaoInicial, dataEmissaoFinal, TipoServicoMultisoftware, configuracaoGeralCarga.PadraoVisualizacaoOperadorLogistico) : new List<int>();

                foreach (int codigo in listaCodigosDocumentos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repXmlNotaFiscal.BuscarPorCodigo(codigo);
                    bool notaFiscalEmOutraCarga = false;

                    if (xmlNotaFiscal != null && configuracao.NotaUnicaEmCargas)
                    {
                        List<string> chaves = new List<string>();
                        chaves.Add(xmlNotaFiscal.Chave);
                        List<int> numerosExistentes = repXmlNotaFiscal.BuscarNotasAtivasPorChave(chaves, ignorarReentrega: true);

                        if (numerosExistentes.Count > 0)
                        {
                            notaFiscalEmOutraCarga = true;
                            if (!configuracao.PossuiValidacaoParaLiberacaoCargaComNotaJaUtilizada)
                            {
                                List<string> numerosCargas = repXmlNotaFiscal.BuscarCargasAtivasPorChave(chaves, ignorarReentrega: true);
                                Servicos.Log.TratarErro($"A nota fiscal ({string.Join(", ", numerosExistentes.ToList())}) já esta vinculada a outra carga ({string.Join(", ", numerosCargas.ToList())})");
                                return new JsonpResult(false, true, $"A nota fiscal ({string.Join(", ", numerosExistentes.ToList())}) já esta vinculada a outra carga ({string.Join(", ", numerosCargas.ToList())})");
                            }
                        }
                    }

                    serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, TipoServicoMultisoftware, TipoNotaFiscal.Venda, ConfiguracaoEmbarcador, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, null, "Adicionado via notas compatíveis", unitOfWork);

                    xmlNotaFiscal.SemCarga = false;
                    xmlNotaFiscal.ClassificacaoNFe = classificacaoNFe;
                    repXmlNotaFiscal.Atualizar(xmlNotaFiscal);
                }

                unitOfWork.CommitChanges();
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Adicionou os Documento NF-e.", unitOfWork);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codCargaPedido = Request.GetIntParam("CargaPedido");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCargaPedido(codCargaPedido);
                if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                {
                    return new JsonpResult(false, true, "A carga está bloqueada e não pode ser editada");
                }

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais) && this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    if (this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || (!(carga?.TipoOperacao?.PermitirTransportadorEnviarNotasFiscais ?? false)))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                }

                await unitOfWork.StartAsync();

                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repCargaPedido.BuscarPorCodigoAsync(codCargaPedido);

                TipoDocumento tipoDocumento = Request.GetEnumParam<TipoDocumento>("TipoDocumento");

                if ((carga?.TipoOperacao?.TipoOperacaoMercosul ?? false) || ((carga?.TipoOperacao?.ConfiguracaoCarga?.TipoOperacaoInternacional ?? false) && tipoDocumento == TipoDocumento.Outros))
                {
                    if (cargaPedido == null)
                        throw new ControllerException("Pedido não encontrado");

                    //Factura
                    TipoContratacaoCarga tipoContratacao = cargaPedido.TipoContratacaoCarga;

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;
                    string retorno = AdicionarFactura(cargaPedido, carga, out xmlNotaFiscal, unitOfWork);

                    if (!string.IsNullOrEmpty(retorno))
                        throw new ControllerException(retorno);

                    SalvarProdutos(xmlNotaFiscal, unitOfWork);

                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = await serCargaNotaFiscal.InserirNotaCargaPedidoAsync(xmlNotaFiscal, cargaPedido, TipoServicoMultisoftware, TipoNotaFiscal.Venda, ConfiguracaoEmbarcador, false, false, Auditado);
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, xmlNotaFiscal, null, "Adicionado manualmente na carga", unitOfWork);

                    if (!ConfiguracaoEmbarcador.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga)
                    {
                        if (pedidoXMLNotaFiscal != null && pedidoXMLNotaFiscal.CargaPedido.Codigo != cargaPedido.Codigo)
                            retorno = "A NF-e " + pedidoXMLNotaFiscal.XMLNotaFiscal.Numero + " já foi adicionada ao pedido " + pedidoXMLNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador;
                    }

                    if (string.IsNullOrEmpty(retorno))
                    {
                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaPedido.Carga, null, "Adicionou um Documento Factura.", unitOfWork);
                        await unitOfWork.CommitChangesAsync();
                        if (tipoContratacao != cargaPedido.TipoContratacaoCarga || (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes))
                            return new JsonpResult(await serCarga.ObterDetalhesDaCargaAsync(cargaPedido.Carga, TipoServicoMultisoftware, unitOfWork));
                        else
                            return new JsonpResult(true);
                    }
                    else
                        throw new ControllerException(retorno);

                }

                if (tipoDocumento == TipoDocumento.Outros && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteAdicionarOutrosDocumentos) && (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || !(carga?.TipoOperacao?.PermitirTransportadorEnviarNotasFiscais ?? false)))
                    throw new ControllerException("Você não possui permissões para executar esta ação.");

                string chave = Request.Params("Chave").Replace(" ", "");

                if (chave.Length == 44 || tipoDocumento == TipoDocumento.Outros)
                {
                    if (tipoDocumento == TipoDocumento.Outros || serDocumento.ValidarChave(chave))
                    {
                        Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                        Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                        Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                        Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = await repIntegracaoIntercab.BuscarIntegracaoAsync();

                        if (cargaPedido != null)
                        {
                            if (cargaPedido.Carga.CargaTransbordo)
                                throw new ControllerException("Não é possível adicionar notas fiscais em cargas de transbordo.");

                            if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                                throw new ControllerException("A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                            TipoContratacaoCarga tipoContratacao = cargaPedido.TipoContratacaoCarga;

                            if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                                if (!string.IsNullOrWhiteSpace(chave))
                                    xmlNotaFiscal = await repXmlNotaFiscal.BuscarPorChaveAsync(chave, cancellationToken);

                                if (xmlNotaFiscal == null)
                                    xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
                                else
                                    xmlNotaFiscal.Initialize();

                                xmlNotaFiscal.TipoDocumento = tipoDocumento;

                                if (!string.IsNullOrWhiteSpace(Request.Params("BaseCalculoICMS")))
                                    xmlNotaFiscal.BaseCalculoICMS = decimal.Parse(Request.Params("BaseCalculoICMS"));

                                if (!string.IsNullOrWhiteSpace(Request.Params("BaseCalculoICMS")))
                                    xmlNotaFiscal.BaseCalculoICMS = decimal.Parse(Request.Params("BaseCalculoICMS"));

                                xmlNotaFiscal.Chave = chave;
                                xmlNotaFiscal.TipoEmissao = Utilidades.Chave.ObterTipoEmissao(xmlNotaFiscal.Chave).ToString().ToEnum<TipoEmissaoNotaFiscal>();
                                xmlNotaFiscal.CNPJTranposrtador = cargaPedido.Carga?.Empresa?.CNPJ_SemFormato ?? "";
                                xmlNotaFiscal.Empresa = cargaPedido.Carga?.Empresa;

                                if (!string.IsNullOrWhiteSpace(Request.Params("DataEmissao")))
                                {
                                    DateTime dataEmissao;
                                    DateTime.TryParseExact(Request.Params("DataEmissao"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                                    xmlNotaFiscal.DataEmissao = dataEmissao;
                                }
                                else
                                    xmlNotaFiscal.DataEmissao = DateTime.Now;

                                double destinatario = double.Parse(Request.Params("Destinatario"));
                                if (destinatario > 0)
                                {
                                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                                    xmlNotaFiscal.Destinatario = await repCliente.BuscarPorCPFCNPJAsync(destinatario);

                                    if (cargaPedido.Pedido.Destinatario == null && cargaPedido.Pedido.Recebedor == null)
                                    {
                                        cargaPedido.Pedido.Destinatario = xmlNotaFiscal.Destinatario;
                                        cargaPedido.Destino = xmlNotaFiscal.Destinatario.Localidade;
                                        await repPedido.AtualizarAsync(cargaPedido.Pedido);
                                    }
                                }
                                else
                                    xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Destinatario;

                                if (xmlNotaFiscal.Destinatario == null)
                                    throw new ControllerException("O destinatário não foi informado no pedido, sendo necessário informar na nota.");

                                xmlNotaFiscal.NCM = Request.Params("NCM");
                                xmlNotaFiscal.CFOP = Request.Params("CFOP");
                                xmlNotaFiscal.NumeroControleCliente = Request.Params("NumeroControleCliente");
                                xmlNotaFiscal.NumeroReferenciaEDI = Request.Params("NumeroReferenciaEDI");
                                xmlNotaFiscal.Embarque = Request.GetStringParam("Embarque");
                                xmlNotaFiscal.MasterBL = Request.GetStringParam("MasterBL");
                                xmlNotaFiscal.NumeroDI = Request.GetStringParam("NumeroDI");
                                xmlNotaFiscal.NumeroCanhoto = Request.Params("NumeroCanhoto");
                                xmlNotaFiscal.PINSUFRAMA = Request.Params("PINSUFRAMA");
                                xmlNotaFiscal.NumeroOutroDocumento = Request.GetStringParam("NumeroOutroDocumento");
                                xmlNotaFiscal.Filial = cargaPedido.Pedido.Filial;
                                xmlNotaFiscal.Modelo = tipoDocumento == TipoDocumento.DCe ? "99" : Request.Params("Modelo");
                                xmlNotaFiscal.NaturezaOP = "";
                                xmlNotaFiscal.nfAtiva = true;
                                xmlNotaFiscal.Numero = Request.GetIntParam("Numero");
                                xmlNotaFiscal.Peso = Request.GetDecimalParam("Peso");
                                xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;

                                if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                                {
                                    xmlNotaFiscal.Moeda = Request.GetEnumParam<MoedaCotacaoBancoCentral>("Moeda");
                                    xmlNotaFiscal.ValorTotalMoeda = Request.GetDecimalParam("ValorTotalMoeda");
                                    xmlNotaFiscal.ValorCotacaoMoeda = Request.GetDecimalParam("ValorCotacaoMoeda");
                                }

                                if (!string.IsNullOrWhiteSpace(Request.Params("PesoLiquido")))
                                    xmlNotaFiscal.PesoLiquido = Request.GetDecimalParam("PesoLiquido");
                                if (cargaPedido.Carga.Veiculo != null)
                                    xmlNotaFiscal.PlacaVeiculoNotaFiscal = cargaPedido.Carga.Veiculo.Placa;
                                else
                                    xmlNotaFiscal.PlacaVeiculoNotaFiscal = "";

                                double remetente = double.Parse(Request.Params("Remetente"));
                                if ((configuracaoGeralCarga?.PermiteInformarRemetenteLancamentoNotaManualCarga ?? false) && remetente > 0)
                                {
                                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                                    xmlNotaFiscal.Emitente = await repCliente.BuscarPorCPFCNPJAsync(remetente);

                                }
                                else if (cargaPedido.Pedido.Remetente != null)
                                    xmlNotaFiscal.Emitente = cargaPedido.Pedido.Remetente;
                                else
                                    throw new ControllerException("Para informar uma nota manualmente é necessário informar o rementete do pedido");

                                xmlNotaFiscal.Serie = Request.Params("Serie");

                                if (tipoDocumento == TipoDocumento.Outros)
                                {
                                    if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                                        xmlNotaFiscal.ModalidadeFrete = ModalidadePagamentoFrete.A_Pagar;
                                    else
                                        xmlNotaFiscal.ModalidadeFrete = ModalidadePagamentoFrete.Pago;
                                }

                                xmlNotaFiscal.TipoOperacaoNotaFiscal = Request.GetEnumParam<TipoOperacaoNotaFiscal>("Tipo");

                                xmlNotaFiscal.Descricao = Request.Params("Descricao");

                                xmlNotaFiscal.Valor = Request.GetDecimalParam("Valor");

                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorCOFINS")))
                                    xmlNotaFiscal.ValorCOFINS = Request.GetDecimalParam("ValorCOFINS");

                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorDesconto")))
                                    xmlNotaFiscal.ValorDesconto = Request.GetDecimalParam("ValorDesconto");

                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorICMS")))
                                    xmlNotaFiscal.ValorICMS = Request.GetDecimalParam("ValorICMS");

                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorIPI")))
                                    xmlNotaFiscal.ValorIPI = Request.GetDecimalParam("ValorIPI");

                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorPIS")))
                                    xmlNotaFiscal.ValorPIS = Request.GetDecimalParam("ValorPIS");

                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorST")))
                                    xmlNotaFiscal.ValorST = Request.GetDecimalParam("ValorST");

                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorTotalProdutos")))
                                    xmlNotaFiscal.ValorTotalProdutos = Request.GetDecimalParam("ValorTotalProdutos");

                                if (!string.IsNullOrWhiteSpace(Request.Params("Volumes")))
                                    xmlNotaFiscal.Volumes = Request.GetIntParam("Volumes");

                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorFrete")))
                                    xmlNotaFiscal.ValorFrete = Request.GetDecimalParam("ValorFrete");

                                if (!string.IsNullOrWhiteSpace(Request.Params("NumeroPedidoNFe")))
                                    xmlNotaFiscal.NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoNFe");

                                if (!string.IsNullOrWhiteSpace(Request.Params("AliquotaImpostoSuspenso")))
                                    xmlNotaFiscal.AliquotaImpostoSuspenso = Request.GetDecimalParam("AliquotaImpostoSuspenso");

                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorImpostoSuspenso")))
                                    xmlNotaFiscal.ValorImpostoSuspenso = Request.GetDecimalParam("ValorImpostoSuspenso");

                                if (!string.IsNullOrWhiteSpace(Request.Params("ValorCommodities")))
                                    xmlNotaFiscal.ValorCommodities = Request.GetDecimalParam("ValorCommodities");

                                xmlNotaFiscal.XML = "";

                                string retorno = string.Empty;
                                bool notaFiscalEmOutraCarga = false;

                                if (tipoDocumento != TipoDocumento.NFe)
                                {
                                    if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroControle) && repPedidoXMLNotaFiscal.ContemDocumentoLancadoComOutroTipo(cargaPedido.Codigo, tipoDocumento))
                                        throw new ControllerException("Não é possível enviar notas de outro tipo de documento já existente nesta carga.");

                                    xmlNotaFiscal.DataRecebimento = DateTime.Now;
                                    await repXmlNotaFiscal.InserirAsync(xmlNotaFiscal);

                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(cargaPedido.Carga.Codigo);

                                    int numeroCTe = repPedidoCTeParaSubContratacao.ContarPorCargaPedido(cargaPedido.Codigo);
                                    if (numeroCTe > 0)
                                    {
                                        throw new ControllerException("Não é possível enviar notas para o pedido pois o mesmo possui CT-es anteriores vinculados e ele, remova os CT-es anteriores para poder enviar as notas.");
                                    }
                                    else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SubContratada)
                                    {
                                        cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Normal;
                                        cargaPedido.Pedido.PedidoSubContratado = false;
                                        cargaPedido.Pedido.SubContratante = null;
                                        cargaPedido.TipoTomador = cargaPedido.Pedido.TipoTomador;

                                        if (cargaPedido.Pedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && cargaPedido.Pedido.Tomador != null)
                                            cargaPedido.Tomador = cargaPedido.Pedido.Tomador;
                                        else
                                            cargaPedido.Tomador = null;

                                        cargaPedido.ModeloDocumentoFiscal = null;
                                    }

                                    if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                                    {
                                        cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Normal;
                                        cargaPedido.Carga.TipoServicoCarga = TipoServicoCarga.Normal;
                                    }

                                    await repCargaPedido.AtualizarAsync(cargaPedido);

                                    await repCarga.AtualizarAsync(cargaPedido.Carga);
                                    await serCarga.SetarTipoContratacaoCargaAsync(cargaPedido.Carga, unitOfWork);
                                }
                                else
                                {
                                    bool msgAlertaObservacao = false;
                                    retorno = serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, TipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);
                                    if (msgAlertaObservacao && !string.IsNullOrWhiteSpace(retorno))
                                        retorno = "";

                                    if (xmlNotaFiscal.Codigo == 0)
                                    {
                                        xmlNotaFiscal.DataRecebimento = DateTime.Now;
                                        await repXmlNotaFiscal.InserirAsync(xmlNotaFiscal);
                                    }
                                    else
                                        await repXmlNotaFiscal.AtualizarAsync(xmlNotaFiscal, Auditado);
                                }

                                if (string.IsNullOrEmpty(retorno))
                                {
                                    SalvarProdutos(xmlNotaFiscal, unitOfWork);

                                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = await serCargaNotaFiscal.InserirNotaCargaPedidoAsync(xmlNotaFiscal, cargaPedido, TipoServicoMultisoftware, TipoNotaFiscal.Venda, ConfiguracaoEmbarcador, notaFiscalEmOutraCarga, false, Auditado);
                                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, xmlNotaFiscal, null, "Adicionado manualmente na carga", unitOfWork);

                                    if (!ConfiguracaoEmbarcador.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga)
                                    {
                                        if (pedidoXMLNotaFiscal != null && pedidoXMLNotaFiscal.CargaPedido.Codigo != cargaPedido.Codigo)
                                            retorno = "A NF-e " + pedidoXMLNotaFiscal.XMLNotaFiscal.Numero + " já foi adicionada ao pedido " + pedidoXMLNotaFiscal.CargaPedido.Pedido.NumeroPedidoEmbarcador;
                                    }

                                    if (string.IsNullOrEmpty(retorno))
                                    {
                                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaPedido.Carga, null, "Adicionou um Documento NF-e.", unitOfWork);
                                        await unitOfWork.CommitChangesAsync();
                                        if (tipoContratacao != cargaPedido.TipoContratacaoCarga || (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes))
                                            return new JsonpResult(await serCarga.ObterDetalhesDaCargaAsync(cargaPedido.Carga, TipoServicoMultisoftware, unitOfWork));
                                        else
                                            return new JsonpResult(true);
                                    }
                                    else
                                        throw new ControllerException(retorno);
                                }
                                else
                                    throw new ControllerException(retorno);
                            }
                            else
                                throw new ControllerException("A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite o envio de notas fiscais");
                        }
                        else
                            throw new ControllerException("Pedido não encontrado");
                    }
                    else
                        throw new ControllerException("A chave informada é inválida, por favor, verifique e tente novamente.");
                }
                else
                    throw new ControllerException("A chave informada não contem 44 caracteres");
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais) && this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);
                int.TryParse(Request.Params("Volumes"), out int volumes);

                decimal.TryParse(Request.Params("Peso"), out decimal peso);
                decimal.TryParse(Request.Params("PesoLiquido"), out decimal pesoLiquido);
                decimal.TryParse(Request.Params("Altura"), out decimal altura);
                decimal.TryParse(Request.Params("Largura"), out decimal largura);
                decimal.TryParse(Request.Params("Comprimento"), out decimal comprimento);
                decimal.TryParse(Request.Params("MetrosCubicos"), out decimal metrosCubicos);
                decimal.TryParse(Request.Params("Pallets"), out decimal pallets);

                double.TryParse(Request.Params("Expedidor"), out double expedidor);
                double.TryParse(Request.Params("Recebedor"), out double recebedor);
                double cnpjCpfDestinatario = Request.GetDoubleParam("Destinatario");

                string segundoCodigoBarras = Utilidades.String.OnlyNumbers(Request.GetStringParam("SegundoCodigoBarras"));
                string chaveVenda = Utilidades.String.OnlyNumbers(Request.GetStringParam("ChaveVenda"));

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

                Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repCargaPedido.BuscarPorCodigoAsync(codigoCargaPedido);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                if (carga.CargaTransbordo)
                    return new JsonpResult(false, true, "Não é possível adicionar notas fiscais em cargas de transbordo.");

                if (carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                if (carga.SituacaoCarga != SituacaoCarga.AgNFe && carga.SituacaoCarga != SituacaoCarga.Nova)
                    return new JsonpResult(false, true, "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ") não permite a alteração de notas fiscais");

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = await repXMLNotaFiscal.BuscarPorCodigoAsync(codigo, true);

                if (xmlNotaFiscal == null)
                    return new JsonpResult(false, true, "Nota fiscal não encontrada.");

                await unidadeTrabalho.StartAsync();

                if ((carga?.TipoOperacao?.TipoOperacaoMercosul ?? false) || ((carga?.TipoOperacao?.ConfiguracaoCarga.TipoOperacaoInternacional ?? false) && xmlNotaFiscal.TipoDocumento == TipoDocumento.Outros))
                {
                    //Factura
                    AtualizarFactura(xmlNotaFiscal, carga, unidadeTrabalho);

                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, xmlNotaFiscal, null, "Atualizado manualmente na carga", unidadeTrabalho);

                    await unidadeTrabalho.CommitChangesAsync();

                    return new JsonpResult(true);
                }

                if (!string.IsNullOrWhiteSpace(segundoCodigoBarras))
                {
                    if (segundoCodigoBarras.Length != 36)
                        return new JsonpResult(false, true, "Código de barras para NF-e de contingência precisa ter 36 caracteres.");
                    if (xmlNotaFiscal.TipoEmissao != TipoEmissaoNotaFiscal.ContingenciaFSDA && xmlNotaFiscal.TipoEmissao != TipoEmissaoNotaFiscal.ContingenciaFSIA)
                        return new JsonpResult(false, true, "Informado código de barras para NF-e que não é de contingência.");
                }

                if (!string.IsNullOrWhiteSpace(chaveVenda) && chaveVenda.Length != 44)
                    return new JsonpResult(false, true, "A chave de venda informada não contem 44 caracteres");

                string numeroReferenciaEDI = Request.Params("NumeroReferenciaEDI");
                bool replicarReferenciaEDI = false;
                if (string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroReferenciaEDI) && !string.IsNullOrWhiteSpace(numeroReferenciaEDI))
                {
                    if (tomador != null && tomador.ReplicarNumeroReferenciaTodasNotasCarga)
                        replicarReferenciaEDI = true;
                    else if (tomador != null && tomador.GrupoPessoas != null && tomador.GrupoPessoas.ReplicarNumeroReferenciaTodasNotasCarga)
                        replicarReferenciaEDI = true;
                }

                string numeroControleCliente = Request.Params("NumeroControleCliente");
                bool replicarNumeroControleCliente = false;
                if (string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroControleCliente) && !string.IsNullOrWhiteSpace(numeroControleCliente))
                {
                    if (tomador != null && tomador.ReplicarNumeroControleCliente)
                        replicarNumeroControleCliente = true;
                    else if (tomador != null && tomador.GrupoPessoas != null && tomador.GrupoPessoas.ReplicarNumeroControleCliente)
                        replicarNumeroControleCliente = true;
                }

                int volumesAnterior = xmlNotaFiscal.Volumes;

                xmlNotaFiscal.Peso = peso;
                xmlNotaFiscal.PesoLiquido = pesoLiquido;
                xmlNotaFiscal.QuantidadePallets = pallets;
                xmlNotaFiscal.MetrosCubicos = metrosCubicos;
                xmlNotaFiscal.Volumes = volumes;
                xmlNotaFiscal.NCM = Request.Params("NCM");
                xmlNotaFiscal.CFOP = Request.Params("CFOP");
                xmlNotaFiscal.NumeroControleCliente = numeroControleCliente;
                xmlNotaFiscal.Expedidor = expedidor > 0 ? repCliente.BuscarPorCPFCNPJ(expedidor) : null;
                xmlNotaFiscal.Recebedor = recebedor > 0 ? repCliente.BuscarPorCPFCNPJ(recebedor) : null;
                xmlNotaFiscal.NumeroReferenciaEDI = numeroReferenciaEDI;
                xmlNotaFiscal.NumeroCanhoto = Request.Params("NumeroCanhoto");
                xmlNotaFiscal.PINSUFRAMA = Request.Params("PINSUFRAMA");
                xmlNotaFiscal.Embarque = Request.GetStringParam("Embarque");
                xmlNotaFiscal.MasterBL = Request.GetStringParam("MasterBL");
                xmlNotaFiscal.NumeroDI = Request.GetStringParam("NumeroDI");
                xmlNotaFiscal.ValorFrete = Request.GetDecimalParam("ValorFrete");
                xmlNotaFiscal.NumeroOutroDocumento = Request.GetStringParam("NumeroOutroDocumento");
                xmlNotaFiscal.ClassificacaoNFe = Request.GetNullableEnumParam<ClassificacaoNFe>("ClassificacaoNFe");
                xmlNotaFiscal.SegundoCodigoBarras = segundoCodigoBarras;
                xmlNotaFiscal.ChaveVenda = chaveVenda;
                xmlNotaFiscal.PalletsControle = Request.GetIntParam("PalletsControle");
                xmlNotaFiscal.Valor = Request.GetDecimalParam("ValorNotaFiscal");
                xmlNotaFiscal.ValorTotalProdutos = Request.GetDecimalParam("ValorTotalProdutos");
                if (xmlNotaFiscal.TipoDocumento != TipoDocumento.NFe)
                    xmlNotaFiscal.Destinatario = cnpjCpfDestinatario > 0 ? await repCliente.BuscarPorCPFCNPJAsync(cnpjCpfDestinatario) : null;

                if (carga.TipoOperacao != null)
                {
                    if (xmlNotaFiscal.MetrosCubicos > 0m && carga.TipoOperacao.UtilizarFatorCubagem && carga.TipoOperacao.FatorCubagem.HasValue)
                    {
                        xmlNotaFiscal.PesoCubado = xmlNotaFiscal.MetrosCubicos * carga.TipoOperacao.FatorCubagem.Value;
                        xmlNotaFiscal.FatorCubagem = carga.TipoOperacao.FatorCubagem.Value;

                        if (xmlNotaFiscal.PesoCubado > xmlNotaFiscal.Peso)
                            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.PesoCubado;
                        else
                            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
                    }
                    else
                        xmlNotaFiscal.PesoCubado = 0m;

                    if (xmlNotaFiscal.QuantidadePallets > 0m && carga.TipoOperacao.UtilizarPaletizacao && carga.TipoOperacao.PesoPorPallet.HasValue)
                    {
                        xmlNotaFiscal.PesoPaletizado = xmlNotaFiscal.QuantidadePallets * carga.TipoOperacao.PesoPorPallet.Value;
                        xmlNotaFiscal.PesoPorPallet = carga.TipoOperacao.PesoPorPallet.Value;

                        if (xmlNotaFiscal.PesoPaletizado > xmlNotaFiscal.Peso)
                            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.PesoPaletizado;
                        else
                            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
                    }
                    else
                        xmlNotaFiscal.PesoPaletizado = 0m;
                }
                else
                {
                    xmlNotaFiscal.PesoCubado = 0m;
                    xmlNotaFiscal.PesoPaletizado = 0m;
                }

                SalvarDimensoes(xmlNotaFiscal, unidadeTrabalho);

                await repXMLNotaFiscal.AtualizarAsync(xmlNotaFiscal, Auditado);

                if (replicarReferenciaEDI)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasDaCarga = repXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
                    if (notasDaCarga != null && notasDaCarga.Count > 0)
                    {
                        foreach (var nota in notasDaCarga)
                        {
                            nota.Initialize();
                            nota.NumeroReferenciaEDI = numeroReferenciaEDI;
                            await repXMLNotaFiscal.AtualizarAsync(nota, Auditado);
                        }
                    }
                }

                if (replicarNumeroControleCliente)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasDaCarga = repXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
                    if (notasDaCarga != null && notasDaCarga.Count > 0)
                    {
                        foreach (var nota in notasDaCarga)
                        {
                            nota.Initialize();
                            nota.NumeroControleCliente = numeroControleCliente;
                            await repXMLNotaFiscal.AtualizarAsync(nota, Auditado);
                        }
                    }
                }

                servicoCargaPedido.AlterarDadosSumarizadosCargaPedido(cargaPedido, volumesAnterior, volumes);
                await repCargaPedido.AtualizarAsync(cargaPedido);

                await unidadeTrabalho.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unidadeTrabalho.RollbackAsync();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a nota fiscal.");
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }

        public async Task<IActionResult> Excluir(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
                int codXMLNotaFiscal = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = await repIntegracaoIntercab.BuscarIntegracaoAsync();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCargaPedido(codCargaPedido);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais) && this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    if (this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || (!(carga?.TipoOperacao?.PermitirTransportadorEnviarNotasFiscais ?? false)))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                }

                await unitOfWork.StartAsync();

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repCargaPedido.BuscarPorCodigoAsync(codCargaPedido);
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = await repXmlNotaFiscal.BuscarPorCodigoAsync(codXMLNotaFiscal, false);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, "Pedido não encontrado");

                if (xmlNotaFiscal == null)
                    return new JsonpResult(false, true, "Nota fiscal não encontrada");

                string retorno = Servicos.WebService.NFe.NotaFiscal.ExcluirNotaFiscal(xmlNotaFiscal, cargaPedido, Auditado, TipoServicoMultisoftware, unitOfWork);
                if (string.IsNullOrWhiteSpace(retorno))
                {
                    new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado).CancelarMovimentacaoPallet(xmlNotaFiscal, cargaPedido);

                    if (!repPedidoXMLNotaFiscal.ContemNotaNaCarga(carga.Codigo))
                    {
                        if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                            cargaPedido.Carga.TipoServicoCarga = TipoServicoCarga.NaoInformado;
                        cargaPedido.Carga.DataRecebimentoUltimaNFe = null;
                        await repositorioCarga.AtualizarAsync(cargaPedido.Carga);
                    }

                    await unitOfWork.CommitChangesAsync();
                    return new JsonpResult(cargaPedido.TipoContratacaoCarga);
                }
                else
                {
                    await unitOfWork.RollbackAsync();
                    return new JsonpResult(false, true, retorno);
                }
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirNotasFiscais(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaPedido = Request.GetIntParam("CargaPedido");
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarPorCodigoAsync(codigoCargaPedido);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais) && this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    if (this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || (!(carga?.TipoOperacao?.PermitirTransportadorEnviarNotasFiscais ?? false)))
                        return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                }

                if (carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                await unitOfWork.StartAsync();

                Servicos.Embarcador.Carga.DocumentoEmissao servicoDocumentoEmissao = new Servicos.Embarcador.Carga.DocumentoEmissao(unitOfWork);

                servicoDocumentoEmissao.DeletarTodos(cargaPedido, Auditado);

                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = await repositorioIntegracaoIntercab.BuscarIntegracaoAsync();

                if ((cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.NormalESubContratada) || (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false))
                {
                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                    int numeroNotas = repositorioPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo);

                    if (numeroNotas == 0)
                    {
                        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                        Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repositorioPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repositorioPedidoCTeParaSubContratacao.BuscarPorCargaPedido(cargaPedido.Codigo);

                        if (pedidosCTeParaSubContratacao.Count > 0)
                        {
                            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();

                            cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.SubContratada;
                            cargaPedido.Pedido.PedidoSubContratado = true;
                            cargaPedido.Tomador = pedidosCTeParaSubContratacao.First().CTeTerceiro.TransportadorTerceiro;
                            cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                            cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;

                            await repositorioPedido.AtualizarAsync(cargaPedido.Pedido);
                            await repositorioCargaPedido.AtualizarAsync(cargaPedido);
                            servicoHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, TipoAcaoCarga.Alterada, _conexao.StringConexao);
                        }
                        else
                        {
                            cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Normal;

                            await repositorioCargaPedido.AtualizarAsync(cargaPedido);
                        }

                        cargaPedido.Carga.DataRecebimentoUltimaNFe = null;

                        if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                            cargaPedido.Carga.TipoServicoCarga = TipoServicoCarga.NaoInformado;

                        await repositorioCarga.AtualizarAsync(cargaPedido.Carga);
                    }
                }

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaPedido.Carga, null, "Excluiu todas as notas fiscais.", unitOfWork);
                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir as notas fiscais.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> VicularNotasDaCargaParaTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais) && this.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);

                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
                int codCargaTransbordo = int.Parse(Request.Params("CargaTransbordo"));

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTransbordos = repCargaPedido.BuscarPorCarga(codCargaTransbordo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codCargaPedido);

                if (cargaPedido.Carga.TipoOperacao == null || !cargaPedido.Carga.TipoOperacao.PermitirTransbordarNotasDeOutrasCargas)
                    return new JsonpResult(false, true, "Não é possível vincular notas de transbordo a essa carga, pois a mesma não está configurada para isso.");

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                unitOfWork.Start();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTransbordar in cargaPedidosTransbordos)
                {
                    for (int i = 0; i < cargaPedidoTransbordar.NotasFiscais.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = cargaPedidoTransbordar.NotasFiscais[i].XMLNotaFiscal;
                        serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, TipoServicoMultisoftware, TipoNotaFiscal.Venda, ConfiguracaoEmbarcador, false, out bool alteradoTipoDeCarga, Auditado);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, xmlNotaFiscal, null, "Adicionado por vínculos de transbordo", unitOfWork);
                    }
                }

                string codigoEmbarcadoCargaTransbordo = cargaPedidosTransbordos.Count > 0 ? " - " + cargaPedidosTransbordos.FirstOrDefault().Carga.Descricao : "";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Vinculou Notas Da Carga Para Transbordo" + codigoEmbarcadoCargaTransbordo + ".", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao vincular nas notas da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadDANFECCe(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
                int codXMLNotaFiscal = int.Parse(Request.Params("Codigo"));


                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorNotaFiscalCargaPedido(codXMLNotaFiscal, codCargaPedido);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva);

                Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva servicoMinerva = new Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva(unitOfWork, cancellationToken);

                if (pedidoXMLNotaFiscal == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                if (tipoIntegracao != null)
                {
                    byte[] arquivoPDF = null;

                    if (pedidoXMLNotaFiscal.CargaPedido.Carga.TipoOperacao?.TipoImpressaoDiarioBordo == TipoImpressaoDiarioBordo.MinutaFreteBovino)
                    {
                        if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML))
                            arquivoPDF = servicoMinerva.BuscarPDFNFePorXML(pedidoXMLNotaFiscal.XMLNotaFiscal.XML);
                        else
                            return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");
                    }
                    else if (Utilidades.Validate.ValidarChave(pedidoXMLNotaFiscal.XMLNotaFiscal.Chave))
                        arquivoPDF = servicoMinerva.BuscarPDFNFe(pedidoXMLNotaFiscal.XMLNotaFiscal.Chave);
                    else
                        return new JsonpResult(false, "Chave da Nota Fiscal ( " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + " ) não é valida.");
                    if (arquivoPDF == null)
                        return new JsonpResult(false, "PDF não disponibilizado pela integração da Minerva.");
                    else
                        return Arquivo(arquivoPDF, "application/x-pkcs12", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                }
                else
                {
                    string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "DANFE Documentos Emitidos", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");

                    if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML) && pedidoXMLNotaFiscal.XMLNotaFiscal.XML.Contains("</nfeProc>"))
                    {
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, pedidoXMLNotaFiscal.XMLNotaFiscal.XML, caminhoDANFE, false, false))
                            return new JsonpResult(false, true, erro);
                    }
                    else if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML) && pedidoXMLNotaFiscal.XMLNotaFiscal.XML.Contains("</NFe>"))
                    {
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, pedidoXMLNotaFiscal.XMLNotaFiscal.XML, caminhoDANFE, false, true))
                            return new JsonpResult(false, true, erro);
                    }
                    else if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML) && pedidoXMLNotaFiscal.XMLNotaFiscal.XML.Contains("nfeProc"))
                    {
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, pedidoXMLNotaFiscal.XMLNotaFiscal.XML, caminhoDANFE, true, false))
                            return new JsonpResult(false, true, erro);
                    }
                    else
                    {
                        string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                        caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                            return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                        else
                        {
                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".xml");
                            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");

                            var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                            var retorno = z.GerarDANFENFeDocumentoDestinados(caminho, caminhoDANFE, unitOfWork);

                            if (retorno == "")
                                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                            else
                                return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");
                        }
                    }

                    return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o DANFE.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadDANFE(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
                int codXMLNotaFiscal = int.Parse(Request.Params("Codigo"));

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorNotaFiscalCargaPedido(codXMLNotaFiscal, codCargaPedido);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva);

                Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva servicoMinerva = new Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva(unitOfWork, cancellationToken);

                if (pedidoXMLNotaFiscal == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                if (tipoIntegracao != null)
                {
                    byte[] arquivoPDF = null;

                    if (pedidoXMLNotaFiscal.CargaPedido.Carga.TipoOperacao?.TipoImpressaoDiarioBordo == TipoImpressaoDiarioBordo.MinutaFreteBovino)
                    {
                        if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML))
                            arquivoPDF = servicoMinerva.BuscarPDFNFePorXML(pedidoXMLNotaFiscal.XMLNotaFiscal.XML);
                        else
                            return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");
                    }
                    else if (Utilidades.Validate.ValidarChave(pedidoXMLNotaFiscal.XMLNotaFiscal.Chave))
                        arquivoPDF = servicoMinerva.BuscarPDFNFe(pedidoXMLNotaFiscal.XMLNotaFiscal.Chave);
                    else
                        return new JsonpResult(false, "Chave da Nota Fiscal ( " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + " ) não é valida.");
                    if (arquivoPDF == null)
                        return new JsonpResult(false, "PDF não disponibilizado pela integração da Minerva.");
                    else
                        return Arquivo(arquivoPDF, "application/x-pkcs12", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                }
                else
                {
                    string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "DANFE Documentos Emitidos", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");

                    if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML) && pedidoXMLNotaFiscal.XMLNotaFiscal.XML.Contains("</nfeProc>"))
                    {
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, pedidoXMLNotaFiscal.XMLNotaFiscal.XML, caminhoDANFE, false, false))
                            return new JsonpResult(false, true, erro);
                    }
                    else if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML) && pedidoXMLNotaFiscal.XMLNotaFiscal.XML.Contains("</NFe>"))
                    {
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, pedidoXMLNotaFiscal.XMLNotaFiscal.XML, caminhoDANFE, false, true))
                            return new JsonpResult(false, true, erro);
                    }
                    else if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML) && pedidoXMLNotaFiscal.XMLNotaFiscal.XML.Contains("nfeProc"))
                    {
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, pedidoXMLNotaFiscal.XMLNotaFiscal.XML, caminhoDANFE, true, false))
                            return new JsonpResult(false, true, erro);
                    }
                    else
                    {
                        string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                        caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                            return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                        else
                        {
                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".xml");
                            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");

                            var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                            var retorno = z.GerarDANFENFeDocumentoDestinados(caminho, caminhoDANFE, unitOfWork);

                            if (retorno == "")
                                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                            else
                                return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");
                        }
                    }

                    return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o DANFE.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> RenderizarDANFE(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codCargaPedido = int.Parse(Request.Params("CargaPedido"));
                int codXMLNotaFiscal = int.Parse(Request.Params("Codigo"));


                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorNotaFiscalCargaPedido(codXMLNotaFiscal, codCargaPedido);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Minerva);

                Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva servicoMinerva = new Servicos.Embarcador.Integracao.Minerva.IntegracaoMinerva(unitOfWork, cancellationToken);


                if (pedidoXMLNotaFiscal == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                if (tipoIntegracao != null)
                {
                    byte[] arquivoPDF = null;

                    if (pedidoXMLNotaFiscal.CargaPedido.Carga.TipoOperacao?.TipoImpressaoDiarioBordo == TipoImpressaoDiarioBordo.MinutaFreteBovino)
                    {
                        if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML))
                            arquivoPDF = servicoMinerva.BuscarPDFNFePorXML(pedidoXMLNotaFiscal.XMLNotaFiscal.XML);
                        else
                            return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");
                    }
                    else if (Utilidades.Validate.ValidarChave(pedidoXMLNotaFiscal.XMLNotaFiscal.Chave))
                        arquivoPDF = servicoMinerva.BuscarPDFNFe(pedidoXMLNotaFiscal.XMLNotaFiscal.Chave);
                    else
                        return new JsonpResult(false, "Chave da Nota Fiscal ( " + pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + " ) não é valida.");
                    if (arquivoPDF == null)
                        return new JsonpResult(false, "PDF não disponibilizado pela integração da Minerva.");
                    else
                    {
                        MemoryStream streamDANFE = new MemoryStream();
                        await streamDANFE.WriteAsync(arquivoPDF, 0, arquivoPDF.Length);
                        streamDANFE.Position = 0;
                        return File(streamDANFE, "application/pdf", $"DANFE - {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero}");
                    }
                }
                else
                {
                    string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "DANFE Documentos Emitidos", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");

                    if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML) && pedidoXMLNotaFiscal.XMLNotaFiscal.XML.Contains("</nfeProc>"))
                    {
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, pedidoXMLNotaFiscal.XMLNotaFiscal.XML, caminhoDANFE, false, false))
                            return new JsonpResult(false, true, erro);
                    }
                    else if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML) && pedidoXMLNotaFiscal.XMLNotaFiscal.XML.Contains("</NFe>"))
                    {
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, pedidoXMLNotaFiscal.XMLNotaFiscal.XML, caminhoDANFE, false, true))
                            return new JsonpResult(false, true, erro);
                    }
                    else if (!string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal.XML) && pedidoXMLNotaFiscal.XMLNotaFiscal.XML.Contains("nfeProc"))
                    {
                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE) && !Zeus.Embarcador.ZeusNFe.Zeus.GerarDANFE(out string erro, pedidoXMLNotaFiscal.XMLNotaFiscal.XML, caminhoDANFE, true, false))
                            return new JsonpResult(false, true, erro);
                    }
                    else
                    {
                        string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                        caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".pdf");
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                        {

                            byte[] arquivoPDF = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE);

                            MemoryStream streamDANFE = new MemoryStream();
                            await streamDANFE.WriteAsync(arquivoPDF, 0, arquivoPDF.Length);
                            streamDANFE.Position = 0;

                            return File(streamDANFE, "application/pdf", $"DANFE - {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero}");
                        }
                        else
                        {
                            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", pedidoXMLNotaFiscal.XMLNotaFiscal.Chave + ".xml");
                            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");

                            var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                            var retorno = z.GerarDANFENFeDocumentoDestinados(caminho, caminhoDANFE, unitOfWork);

                            if (retorno == "")
                            {

                                byte[] arquivoPDF = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE);

                                MemoryStream streamDANFE = new MemoryStream();
                                await streamDANFE.WriteAsync(arquivoPDF, 0, arquivoPDF.Length);
                                streamDANFE.Position = 0;

                                return File(streamDANFE, "application/pdf", $"DANFE - {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero}");
                            }
                            else
                                return new JsonpResult(false, "O documento não possui arquivo XML para a geração da DANFE.");
                        }
                    }

                    byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE);

                    MemoryStream stream = new MemoryStream();
                    await stream.WriteAsync(pdf, 0, pdf.Length);
                    stream.Position = 0;

                    return File(stream, "application/pdf", $"DANFE - {pedidoXMLNotaFiscal.XMLNotaFiscal.Numero}");
                }
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar o termo de quitação!");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadXmlCCe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codDocumento = Request.GetIntParam("Codigo");
                string caminhoDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;


                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codDocumento);

                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documento.Chave + "-" + codDocumento + "-cce" + ".xml");

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho), "text/xml", System.IO.Path.GetFileName(caminho));
                else
                    return new JsonpResult(false, true, "Documento não encontrado.");

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadDACCe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codDocumento = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codDocumento);

                if (documento == null)
                    throw new ControllerException("Documento não encontrado");
                if (documento.TipoDocumento != TipoDocumentoDestinadoEmpresa.CCe)
                    throw new ControllerException("O Documento não é uma Carte de Correção");

                return Arquivo(new Servicos.Embarcador.NFe.NFe().ObterDACCe(documento, unitOfWork), "application/pdf", $"DACCe - {documento.Chave}.pdf");
            }
            catch (ServicoException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o DACCe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadXml()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codCargaPedido = Request.GetIntParam("CargaPedido");
                int codXMLNotaFiscal = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorNotaFiscalCargaPedido(codXMLNotaFiscal, codCargaPedido);

                if ((pedidoXMLNotaFiscal == null) || string.IsNullOrWhiteSpace(pedidoXMLNotaFiscal.XMLNotaFiscal?.XML))
                    return new JsonpResult(false, true, "Documento não encontrado.");

                byte[] xmlBinario = System.Text.Encoding.UTF8.GetBytes(pedidoXMLNotaFiscal.XMLNotaFiscal.XML);

                return Arquivo(xmlBinario, "text/xml", $"{pedidoXMLNotaFiscal.XMLNotaFiscal.Chave}.xml");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadTodosXmlPorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            TipoArquivo tipoArquivo = TipoArquivo.Zip;

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                int totalXMLCarga = repositorioPedidoXMLNotaFiscal.ContarComXMLPorCarga(codigoCarga);

                if (totalXMLCarga == 0)
                    return new JsonpResult(false, true, "Nenhum XML de nota encontrado para a carga.");

                Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo = servicoArquivo.Adicionar("XML das Notas da Carga", Usuario, tipoArquivo);
                string stringConexao = _conexao.StringConexao;

                Task.Factory.StartNew(() => BaixarTodosXmlPorCarga(stringConexao, codigoCarga, controleGeracaoArquivo));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, $"Ocorreu uma falha ao baixar o arquivo {tipoArquivo.ObterDescricao()} dos XML das notas da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarNOTFISMichelin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin integracaoMichelin = new Servicos.Embarcador.Integracao.Michelin.IntegracaoMichelin(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);
                if (cargaPedido == null || cargaPedido.Pedido == null)
                    return new JsonpResult(false, "Pedido não encontrado");

                if (string.IsNullOrEmpty(cargaPedido.Pedido.MessageIdentifierCodeMichelin))
                    return new JsonpResult(false, "Este pedido não possui MessageTdentifierCode informado. Este processo de consulta do NOTFIS está disponível apenas para os pedidos importados do WS.");

                unitOfWork.Start();

                integracaoMichelin.BuscarNOTFISAsync(cargaPedido, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, _conexao.AdminStringConexao, cargaPedido.Carga.Empresa, Auditado, out string msgRetorno, out bool alteradoTipoDeCarga);

                unitOfWork.CommitChanges();

                if (!string.IsNullOrWhiteSpace(msgRetorno))
                    return new JsonpResult(false, msgRetorno);
                else
                {
                    if (alteradoTipoDeCarga)
                    {
                        Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                        serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);

                        var dadosRetorno = new
                        {
                            DetalhesCarga = serCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork),
                            Mensagem = "NOTFIS consultado com sucesso. Message Identifier Code: " + cargaPedido.Pedido.MessageIdentifierCodeMichelin
                        };
                        return new JsonpResult(dadosRetorno);
                        //carregarGridDocumentosParaEmissao();
                    }
                    else
                        return new JsonpResult(true, true, "NOTFIS consultado com sucesso. Message Identifier Code: " + cargaPedido.Pedido.MessageIdentifierCodeMichelin);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema.");
                }
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirTodosEspelhos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");

                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement repPedidoEspelhoIntercement = new Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement(unitOfWork);
                Repositorio.Embarcador.Pedidos.EspelhoIntercement repEspelhoIntercement = new Repositorio.Embarcador.Pedidos.EspelhoIntercement(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement> pedidoEspelhoIntercements = repPedidoEspelhoIntercement.BuscarPorCargaPedido(codigoCargaPedido);
                List<Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement> espelhoIntercements = repPedidoEspelhoIntercement.ConsultarPorCargaPedido(codigoCargaPedido);
                if (espelhoIntercements.Count == 0 || pedidoEspelhoIntercements.Count == 0)
                    return new JsonpResult(false, "Intercement não encontrado");

                if (pedidoEspelhoIntercements.Count > 0)
                {
                    foreach (var item in pedidoEspelhoIntercements)
                        repPedidoEspelhoIntercement.Deletar(item, Auditado);
                }

                if (espelhoIntercements.Count > 0)
                {
                    foreach (var item in espelhoIntercements)
                        repEspelhoIntercement.Deletar(item, Auditado);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema.");
                }
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VincularCTeOSMae()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");

                unitOfWork.Start();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
                Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);
                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                bool enviarCTeApenasParaTomador = (configuracaoGeral?.EnviarCTeApenasParaTomador ?? false);

                if (cte == null || cargaPedido == null)
                    return new JsonpResult(false, "CT-e não localizado");

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteObjeto = serCte.ConverterEntidadeCTeParaObjeto(cte, enviarCTeApenasParaTomador, unitOfWork);

                serCargaCteParaSubContratacao.VincularCTeTerceiroACargaPedido(cteObjeto, cargaPedido, unitOfWork, TipoServicoMultisoftware);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, $"Inseriu CT-e anterior {cte.Chave} pela consulta de CT-es vinculados a O.S. mãe.", unitOfWork);

                serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                unitOfWork.CommitChanges();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);

                var dadosRetorno = new
                {
                    DetalhesCarga = serCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork)
                };
                return new JsonpResult(dadosRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirEspelho()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoEspelho = Request.GetIntParam("Codigo");
                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");

                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement repPedidoEspelhoIntercement = new Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement(unitOfWork);
                Repositorio.Embarcador.Pedidos.EspelhoIntercement repEspelhoIntercement = new Repositorio.Embarcador.Pedidos.EspelhoIntercement(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement> pedidosEspelhoIntercements = repPedidoEspelhoIntercement.BuscarPorEspelho(codigoEspelho);
                Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement espelhoIntercements = repEspelhoIntercement.BuscarPorCodigo(codigoEspelho);

                if (espelhoIntercements == null || pedidosEspelhoIntercements.Count == 0)
                    return new JsonpResult(false, "Intercement não encontrado");

                if (pedidosEspelhoIntercements.Count > 0)
                {
                    foreach (var item in pedidosEspelhoIntercements)
                        repPedidoEspelhoIntercement.Deletar(item, Auditado);
                }

                repEspelhoIntercement.Deletar(espelhoIntercements, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema.");
                }
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RetornarCargasPedidoPorNumeroNF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int numeroNF = Request.GetIntParam("NumeroNF");

                List<int> cargasPedidos = repPedidoXMLNotaFiscal.BuscarCargasPedidoPorCargaNumeroNF(codigoCarga, numeroNF);

                var dynCargaPedido = new
                {
                    CargaPedido = (from obj in cargasPedidos
                                   select new
                                   {
                                       CodigoCargaPedido = obj,
                                       CodigoCarga = codigoCarga,
                                       NumeroNF = numeroNF
                                   }).ToList()
                };

                return new JsonpResult(dynCargaPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao retornar os pedidos por número da nota.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void BaixarTodosXmlPorCarga(string stringConexao, int codigoCarga, Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);

            try
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarComXMLPorCarga(codigoCarga);

                if (listaPedidoXMLNotaFiscal.Count > 0)
                {
                    Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in listaPedidoXMLNotaFiscal)
                    {
                        string nomeArquivo = $"{pedidoXMLNotaFiscal.XMLNotaFiscal.Chave}.xml";

                        if (!conteudoCompactar.ContainsKey(nomeArquivo))
                            conteudoCompactar.Add(nomeArquivo, System.Text.Encoding.UTF8.GetBytes(pedidoXMLNotaFiscal.XMLNotaFiscal.XML));
                    }

                    System.IO.MemoryStream arquivoTodosXML = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
                    byte[] arquivoBinarioTodosXML = arquivoTodosXML.ToArray();

                    arquivoTodosXML.Dispose();

                    servicoArquivo.SalvarArquivo(controleGeracaoArquivo, arquivoTodosXML);
                    servicoArquivo.Finalizar(controleGeracaoArquivo, nota: $"Geração do arquivo {controleGeracaoArquivo.TipoArquivo.ObterDescricao()} dos XML das notas da carga concluído", urlPagina: "Cargas/Carga");
                }
                else
                    servicoArquivo.Remover(controleGeracaoArquivo);
            }
            catch (Exception excecao)
            {
                servicoArquivo.FinalizarComFalha(controleGeracaoArquivo, nota: $"Ocorreu uma falha ao gerar o arquivo {controleGeracaoArquivo.TipoArquivo.ObterDescricao()} das notas da carga", urlPagina: "Cargas/Carga", excecao: excecao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void SalvarDimensoes(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalDimensao repDimensao = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalDimensao(unitOfWork);

            dynamic dimensoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Dimensoes"));

            if (xmlNotaFiscal.Dimensoes != null && xmlNotaFiscal.Dimensoes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dimensao in dimensoes)
                    if (dimensao.Codigo != null)
                        codigos.Add((int)dimensao.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalDimensao> dimensoesDeletar = (from obj in xmlNotaFiscal.Dimensoes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < dimensoesDeletar.Count; i++)
                    repDimensao.Deletar(dimensoesDeletar[i]);
            }

            foreach (dynamic dimensao in dimensoes)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalDimensao xmlNotaFiscalDimensao = null;

                int codigo = 0;

                if (dimensao.Codigo != null && int.TryParse((string)dimensao.Codigo, out codigo))
                    xmlNotaFiscalDimensao = repDimensao.BuscarPorCodigo(codigo, false);

                if (xmlNotaFiscalDimensao == null)
                    xmlNotaFiscalDimensao = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalDimensao();

                xmlNotaFiscalDimensao.XMLNotaFiscal = xmlNotaFiscal;
                xmlNotaFiscalDimensao.Altura = (decimal)dimensao.Altura;
                xmlNotaFiscalDimensao.Comprimento = (decimal)dimensao.Comprimento;
                xmlNotaFiscalDimensao.Largura = (decimal)dimensao.Largura;
                xmlNotaFiscalDimensao.MetrosCubicos = (decimal)dimensao.MetrosCubicos;
                xmlNotaFiscalDimensao.Volumes = (int)dimensao.Volumes;

                if (xmlNotaFiscalDimensao.Codigo > 0)
                    repDimensao.Atualizar(xmlNotaFiscalDimensao);
                else
                    repDimensao.Inserir(xmlNotaFiscalDimensao);
            }
        }

        private void SalvarProdutos(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            dynamic dynProdutos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));
            if (dynProdutos == null || dynProdutos.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtos = repXMLNotaFiscalProduto.BuscarPorNotaFiscal(xmlNotaFiscal.Codigo);

            if (produtos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic produto in dynProdutos)
                    if (produto.Codigo != null)
                        codigos.Add((int)produto.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtosDeletar = (from obj in produtos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < produtosDeletar.Count; i++)
                    repXMLNotaFiscalProduto.Deletar(produtosDeletar[i]);
            }

            foreach (dynamic produto in dynProdutos)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto xmlNotaFiscalProduto = produto.Codigo != null ? repXMLNotaFiscalProduto.BuscarPorCodigo((int)produto.Codigo, false) : null;
                if (xmlNotaFiscalProduto == null)
                {
                    xmlNotaFiscalProduto = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto();
                    xmlNotaFiscalProduto.XMLNotaFiscal = xmlNotaFiscal;
                }

                xmlNotaFiscalProduto.Quantidade = (decimal)produto.Quantidade;
                xmlNotaFiscalProduto.ValorProduto = (decimal)produto.ValorUnitario;
                xmlNotaFiscalProduto.UnidadeMedida = (string)produto.UnidadeMedida;

                int codigoProduto = ((string)produto.Produto.Codigo).ToInt();
                xmlNotaFiscalProduto.Produto = repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);

                if (xmlNotaFiscalProduto.Codigo > 0)
                    repXMLNotaFiscalProduto.Atualizar(xmlNotaFiscalProduto);
                else
                    repXMLNotaFiscalProduto.Inserir(xmlNotaFiscalProduto);
            }
        }

        private string AdicionarFactura(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            string retorno = string.Empty;
            xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

            if (cargaPedido != null)
            {
                if (cargaPedido.Carga.CargaTransbordo)
                    return "Não é possível adicionar notas fiscais em cargas de transbordo.";

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.";

                TipoContratacaoCarga tipoContratacao = cargaPedido.TipoContratacaoCarga;

                if (cargaPedido.Carga.SituacaoCarga == SituacaoCarga.AgNFe)
                {
                    xmlNotaFiscal.Initialize();
                    xmlNotaFiscal.TipoDocumento = TipoDocumento.Outros;
                    xmlNotaFiscal.TipoFatura = true;
                    xmlNotaFiscal.BaseCalculoICMS = 0;
                    xmlNotaFiscal.BaseCalculoICMS = 0;
                    xmlNotaFiscal.TipoEmissao = TipoEmissaoNotaFiscal.NaoEletronica;
                    xmlNotaFiscal.CNPJTranposrtador = cargaPedido.Carga.Empresa.CNPJ_SemFormato;
                    xmlNotaFiscal.Empresa = cargaPedido.Carga.Empresa;

                    if (!string.IsNullOrWhiteSpace(Request.Params("DataEmissao")))
                    {
                        DateTime dataEmissao;
                        DateTime.TryParseExact(Request.Params("DataEmissao"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                        xmlNotaFiscal.DataEmissao = dataEmissao;
                    }
                    else
                        xmlNotaFiscal.DataEmissao = DateTime.Now;

                    double destinatario = double.Parse(Request.Params("Destinatario"));
                    if (destinatario > 0)
                    {
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                        xmlNotaFiscal.Destinatario = repCliente.BuscarPorCPFCNPJ(destinatario);
                        if (cargaPedido.Pedido.Destinatario == null)
                        {
                            cargaPedido.Pedido.Destinatario = xmlNotaFiscal.Destinatario;
                            cargaPedido.Destino = xmlNotaFiscal.Destinatario.Localidade;
                            repPedido.Atualizar(cargaPedido.Pedido);
                        }
                    }
                    else
                        xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Destinatario;

                    if (xmlNotaFiscal.Destinatario == null)
                        throw new ControllerException("O destinatário não foi informado no pedido, sendo necessário informar na nota.");

                    xmlNotaFiscal.NCM = Request.Params("NCM");
                    xmlNotaFiscal.CFOP = Request.Params("CFOP");
                    xmlNotaFiscal.NumeroControleCliente = Request.Params("NumeroControleCliente");
                    xmlNotaFiscal.NumeroReferenciaEDI = Request.Params("NumeroReferenciaEDI");
                    xmlNotaFiscal.Embarque = Request.GetStringParam("Embarque");
                    xmlNotaFiscal.MasterBL = Request.GetStringParam("MasterBL");
                    xmlNotaFiscal.NumeroDI = Request.GetStringParam("NumeroDI");
                    xmlNotaFiscal.NumeroCanhoto = Request.Params("NumeroCanhoto");
                    xmlNotaFiscal.FaturaFake = Request.GetBoolParam("FacturaFake");
                    xmlNotaFiscal.PINSUFRAMA = Request.Params("PINSUFRAMA");
                    xmlNotaFiscal.NumeroOutroDocumento = Request.GetStringParam("NumeroOutroDocumento");

                    xmlNotaFiscal.Filial = cargaPedido.Pedido.Filial;
                    xmlNotaFiscal.Modelo = Request.Params("Modelo");
                    xmlNotaFiscal.NaturezaOP = "";
                    xmlNotaFiscal.nfAtiva = true;
                    xmlNotaFiscal.Numero = Request.GetIntParam("Numero");
                    xmlNotaFiscal.Peso = Request.GetDecimalParam("Peso");

                    xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;

                    if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                    {
                        xmlNotaFiscal.Moeda = Request.GetEnumParam<MoedaCotacaoBancoCentral>("Moeda");
                        xmlNotaFiscal.ValorTotalMoeda = Request.GetDecimalParam("ValorTotalMoeda");
                        xmlNotaFiscal.ValorCotacaoMoeda = Request.GetDecimalParam("ValorCotacaoMoeda");
                    }

                    if (!string.IsNullOrWhiteSpace(Request.Params("PesoLiquido")))
                        xmlNotaFiscal.PesoLiquido = Request.GetDecimalParam("PesoLiquido");
                    if (cargaPedido.Carga.Veiculo != null)
                        xmlNotaFiscal.PlacaVeiculoNotaFiscal = cargaPedido.Carga.Veiculo.Placa;
                    else
                        xmlNotaFiscal.PlacaVeiculoNotaFiscal = "";

                    double remetente = double.Parse(Request.Params("Remetente"));
                    if ((configuracaoGeralCarga?.PermiteInformarRemetenteLancamentoNotaManualCarga ?? false) && remetente > 0)
                    {
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                        xmlNotaFiscal.Emitente = repCliente.BuscarPorCPFCNPJ(remetente);

                    }
                    else if (cargaPedido.Pedido.Remetente != null)
                        xmlNotaFiscal.Emitente = cargaPedido.Pedido.Remetente;
                    else
                        throw new ControllerException("Para informar uma nota manualmente é necessário informar o rementete do pedido");

                    xmlNotaFiscal.Serie = Request.Params("Serie");

                    if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                        xmlNotaFiscal.ModalidadeFrete = ModalidadePagamentoFrete.A_Pagar;
                    else
                        xmlNotaFiscal.ModalidadeFrete = ModalidadePagamentoFrete.Pago;

                    xmlNotaFiscal.TipoOperacaoNotaFiscal = Request.GetEnumParam<TipoOperacaoNotaFiscal>("Tipo");

                    xmlNotaFiscal.Descricao = Request.Params("Descricao");

                    xmlNotaFiscal.Valor = Request.GetDecimalParam("Valor");

                    if (!string.IsNullOrWhiteSpace(Request.Params("ValorCOFINS")))
                        xmlNotaFiscal.ValorCOFINS = Request.GetDecimalParam("ValorCOFINS");

                    if (!string.IsNullOrWhiteSpace(Request.Params("ValorDesconto")))
                        xmlNotaFiscal.ValorDesconto = Request.GetDecimalParam("ValorDesconto");

                    if (!string.IsNullOrWhiteSpace(Request.Params("ValorICMS")))
                        xmlNotaFiscal.ValorICMS = Request.GetDecimalParam("ValorICMS");

                    if (!string.IsNullOrWhiteSpace(Request.Params("ValorIPI")))
                        xmlNotaFiscal.ValorIPI = Request.GetDecimalParam("ValorIPI");

                    if (!string.IsNullOrWhiteSpace(Request.Params("ValorPIS")))
                        xmlNotaFiscal.ValorPIS = Request.GetDecimalParam("ValorPIS");

                    if (!string.IsNullOrWhiteSpace(Request.Params("ValorST")))
                        xmlNotaFiscal.ValorST = Request.GetDecimalParam("ValorST");

                    if (!string.IsNullOrWhiteSpace(Request.Params("ValorTotalProdutos")))
                        xmlNotaFiscal.ValorTotalProdutos = Request.GetDecimalParam("ValorTotalProdutos");

                    if (!string.IsNullOrWhiteSpace(Request.Params("Volumes")))
                        xmlNotaFiscal.Volumes = Request.GetIntParam("Volumes");

                    if (!string.IsNullOrWhiteSpace(Request.Params("ValorFrete")))
                        xmlNotaFiscal.ValorFrete = Request.GetDecimalParam("ValorFrete");

                    if (!string.IsNullOrWhiteSpace(Request.Params("AliquotaImpostoSuspenso")))
                        xmlNotaFiscal.AliquotaImpostoSuspenso = Request.GetDecimalParam("AliquotaImpostoSuspenso");

                    if (!string.IsNullOrWhiteSpace(Request.Params("ValorImpostoSuspenso")))
                        xmlNotaFiscal.ValorImpostoSuspenso = Request.GetDecimalParam("ValorImpostoSuspenso");

                    if (!string.IsNullOrWhiteSpace(Request.Params("ValorCommodities")))
                        xmlNotaFiscal.ValorCommodities = Request.GetDecimalParam("ValorCommodities");

                    xmlNotaFiscal.XML = "";

                    bool msgAlertaObservacao = false;
                    bool notaFiscalEmOutraCarga = false;

                    retorno = serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, TipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);
                    if (msgAlertaObservacao && !string.IsNullOrWhiteSpace(retorno))
                        retorno = "";

                    if (xmlNotaFiscal.FaturaFake == true)
                        cargaPedido.Carga.PossuiFacturaFake = true;

                    if (xmlNotaFiscal.Codigo == 0)
                    {
                        xmlNotaFiscal.DataRecebimento = DateTime.Now;
                        repXmlNotaFiscal.Inserir(xmlNotaFiscal);
                    }
                    else
                        repXmlNotaFiscal.Atualizar(xmlNotaFiscal, Auditado);

                    return retorno;
                }
                else
                    return "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite adicionar facturas";
            }
            else
                return "Pedido não encontrado";

        }

        private void AtualizarFactura(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            xmlNotaFiscal.TipoDocumento = TipoDocumento.Outros;
            xmlNotaFiscal.TipoFatura = true;
            xmlNotaFiscal.BaseCalculoICMS = 0;
            xmlNotaFiscal.BaseCalculoICMS = 0;
            xmlNotaFiscal.TipoEmissao = TipoEmissaoNotaFiscal.NaoEletronica;

            if (!string.IsNullOrWhiteSpace(Request.Params("DataEmissao")))
            {
                DateTime dataEmissao;
                DateTime.TryParseExact(Request.Params("DataEmissao"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                xmlNotaFiscal.DataEmissao = dataEmissao;
            }
            else
                xmlNotaFiscal.DataEmissao = DateTime.Now;

            double destinatario = double.Parse(Request.Params("Destinatario"));
            if (destinatario > 0)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                xmlNotaFiscal.Destinatario = repCliente.BuscarPorCPFCNPJ(destinatario);
            }

            xmlNotaFiscal.NCM = Request.Params("NCM");
            xmlNotaFiscal.CFOP = Request.Params("CFOP");
            xmlNotaFiscal.NumeroControleCliente = Request.Params("NumeroControleCliente");
            xmlNotaFiscal.NumeroReferenciaEDI = Request.Params("NumeroReferenciaEDI");
            xmlNotaFiscal.Embarque = Request.GetStringParam("Embarque");
            xmlNotaFiscal.MasterBL = Request.GetStringParam("MasterBL");
            xmlNotaFiscal.NumeroDI = Request.GetStringParam("NumeroDI");
            xmlNotaFiscal.NumeroCanhoto = Request.Params("NumeroCanhoto");
            xmlNotaFiscal.FaturaFake = Request.GetBoolParam("FacturaFake");
            xmlNotaFiscal.PINSUFRAMA = Request.Params("PINSUFRAMA");
            xmlNotaFiscal.NumeroOutroDocumento = Request.GetStringParam("NumeroOutroDocumento");

            xmlNotaFiscal.Modelo = Request.Params("Modelo");
            xmlNotaFiscal.NaturezaOP = "";
            xmlNotaFiscal.nfAtiva = true;
            xmlNotaFiscal.Numero = Request.GetIntParam("Numero");
            xmlNotaFiscal.Peso = Request.GetDecimalParam("Peso");
            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;

            if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
            {
                xmlNotaFiscal.Moeda = Request.GetEnumParam<MoedaCotacaoBancoCentral>("Moeda");
                xmlNotaFiscal.ValorTotalMoeda = Request.GetDecimalParam("ValorTotalMoeda");
                xmlNotaFiscal.ValorCotacaoMoeda = Request.GetDecimalParam("ValorCotacaoMoeda");
            }

            if (!string.IsNullOrWhiteSpace(Request.Params("PesoLiquido")))
                xmlNotaFiscal.PesoLiquido = Request.GetDecimalParam("PesoLiquido");

            xmlNotaFiscal.Serie = Request.Params("Serie");

            xmlNotaFiscal.TipoOperacaoNotaFiscal = Request.GetEnumParam<TipoOperacaoNotaFiscal>("Tipo");

            xmlNotaFiscal.Descricao = Request.Params("Descricao");

            xmlNotaFiscal.Valor = Request.GetDecimalParam("Valor");

            if (!string.IsNullOrWhiteSpace(Request.Params("ValorCOFINS")))
                xmlNotaFiscal.ValorCOFINS = Request.GetDecimalParam("ValorCOFINS");

            if (!string.IsNullOrWhiteSpace(Request.Params("ValorDesconto")))
                xmlNotaFiscal.ValorDesconto = Request.GetDecimalParam("ValorDesconto");

            if (!string.IsNullOrWhiteSpace(Request.Params("ValorICMS")))
                xmlNotaFiscal.ValorICMS = Request.GetDecimalParam("ValorICMS");

            if (!string.IsNullOrWhiteSpace(Request.Params("ValorIPI")))
                xmlNotaFiscal.ValorIPI = Request.GetDecimalParam("ValorIPI");

            if (!string.IsNullOrWhiteSpace(Request.Params("ValorPIS")))
                xmlNotaFiscal.ValorPIS = Request.GetDecimalParam("ValorPIS");

            if (!string.IsNullOrWhiteSpace(Request.Params("ValorST")))
                xmlNotaFiscal.ValorST = Request.GetDecimalParam("ValorST");

            if (!string.IsNullOrWhiteSpace(Request.Params("ValorTotalProdutos")))
                xmlNotaFiscal.ValorTotalProdutos = Request.GetDecimalParam("ValorTotalProdutos");

            if (!string.IsNullOrWhiteSpace(Request.Params("Volumes")))
                xmlNotaFiscal.Volumes = Request.GetIntParam("Volumes");

            if (!string.IsNullOrWhiteSpace(Request.Params("ValorFrete")))
                xmlNotaFiscal.ValorFrete = Request.GetDecimalParam("ValorFrete");

            xmlNotaFiscal.XML = "";

            if (xmlNotaFiscal.FaturaFake == true)
                carga.PossuiFacturaFake = true;
            else
            {
                //validar se existe fatura fake na carga..
                if (serCargaNotaFiscal.ValidarExisteFaturaFakeNaCarga(carga))
                    carga.PossuiFacturaFake = true;
                else
                    carga.PossuiFacturaFake = false;
            }

            repCarga.Atualizar(carga);
            repXmlNotaFiscal.Atualizar(xmlNotaFiscal, Auditado);

        }

        #endregion
    }
}
