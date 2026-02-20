using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System.Data;
using System.Globalization;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosCarga
{
    [CustomAuthorize(new string[] { "PesquisaCargas", "PesquisaCargaPermiteCTeComplementar", "PesquisaCargasNaGrid", "BuscarDetalhesDaCarga", "BuscarDadosCargaPedido", "ObterRetiradaContainer" }, "Cargas/Carga", "Logistica/JanelaCarregamento", "GestaoPatio/FluxoPatio", "Logistica/Monitoramento", "Acertos/AcertoViagem", "Pedidos/Container", "CTe/ConsultaCTe")]
    public class CargaController : BaseController
    {
        #region Construtores

        public CargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = await repConfiguracaoGeral.BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();


            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaPesquisa filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaPesquisa()
                {
                    CodigosFilial = Request.GetListParam<int>("Filial"),
                    CodigosFilialVenda = Request.GetListParam<int>("FilialVenda"),
                    NumeroNF = Request.GetIntParam("NumeroNF"),
                    NumeroCTe = Request.GetIntParam("NumeroCTe"),
                    NumeroMDFe = Request.GetIntParam("NumeroMDFe"),
                    CodigoVeiculo = Request.GetIntParam("Veiculo"),
                    CodigoMotorista = Request.GetIntParam("Motorista"),
                    NumeroFrota = Request.GetStringParam("NumeroFrota"),
                    CodigosEmpresa = Request.GetListParam<int>("Empresa"),
                    CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                    CodigoTipoOcorrencia = Request.GetIntParam("TipoOcorrencia"),
                    CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                    CodigoOrigem = Request.GetIntParam("Origem"),
                    CodigoDestino = Request.GetIntParam("Destino"),
                    NumeroCTeSubcontratacao = Request.GetIntParam("NumeroCTeSubcontratacao"),

                    FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                    CargasNaoFechadas = Request.GetBoolParam("CargaNaoFechadas"),
                    CargasNaoAgrupadas = Request.GetBoolParam("CargasNaoAgrupadas"),
                    CargasDisponiveisParaJanela = Request.GetBoolParam("CargasDisponiveisParaJanela"),
                    PossuiDTNatura = Request.GetNullableBoolParam("PossuiDTNatura"),
                    CargaTransbordo = Request.GetNullableBoolParam("CargaTransbordo"),

                    CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                    CpfCnpjExpedidor = Request.GetDoubleParam("Expedidor"),
                    CpfCnpjRecebedor = Request.GetDoubleParam("Recebedor"),
                    CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                    CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),

                    CodigoCargaEmbarcador = Request.GetStringParam("NumeroCarga"),
                    NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                    NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),

                    Situacao = Request.GetListParam<SituacaoCarga>("Situacao"),
                    SituacaoDiferente = Request.GetListParam<SituacaoCarga>("StatusDiff"),
                    TipoIntegracao = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TipoIntegracao>>(Request.Params("TipoIntegracao")),
                    TiposPropostasMultimodal = Usuario?.PerfilAcesso?.TiposPropostasMultimodal.ToList(),

                    DataInicialEmissao = Request.GetNullableDateTimeParam("DataInicialEmissao"),
                    DataFinalEmissao = Request.GetNullableDateTimeParam("DataFinalEmissao"),
                    DataInicialCarga = Request.GetNullableDateTimeParam("DataInicialCarga"),
                    DataFinalCarga = Request.GetNullableDateTimeParam("DataFinalCarga"),
                    CodigosEmpresas = (this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null ? this.Usuario.Empresas.Select(c => c.Codigo).ToList() : null,
                    NumeroContainer = Request.GetStringParam("NumeroContainer"),
                    SomenteNaoFechadas = Request.GetBoolParam("SomenteNaoFechadas"),
                };

                if (configuracaoGeralCarga.SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho)
                    filtroPesquisa.SomenteCargasDeRedespacho = configuracaoGeralCarga.SelecionarSomenteOperacoesDeRedespachoNaTelaDeRedespacho;
                else
                    filtroPesquisa.SomenteCargasDeRedespacho = null;

                bool telaRedespacho = Request.GetBoolParam("TelaRedespacho");
                bool telaPagametoProvedor = Request.GetBoolParam("TelaPagametoProvedor");
                bool telaCancelamento = Request.GetBoolParam("TelaCancelamento");
                bool telaPagamento = Request.GetBoolParam("TelaPagamento");

                filtroPesquisa.NaoExibirCargasCanceladas = telaPagamento;
                filtroPesquisa.NaoRetornarSubCarga = telaCancelamento;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && telaRedespacho)
                    if (this.Usuario.Empresa != null)
                    {
                        filtroPesquisa.CpfCnpjRecebedor = this.Usuario.Empresa.CNPJ.ToDouble();
                        filtroPesquisa.TelaRedespacho = telaRedespacho;
                    }

                int codigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento");
                if (codigoCentroDescarregamento > 0 && filtroPesquisa.CpfCnpjDestinatario <= 0)
                {
                    Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork, cancellationToken);
                    filtroPesquisa.CpfCnpjDestinatario = repositorioCentroDescarregamento.BuscarDestinatarioPorCentroDeDescarregamento(codigoCentroDescarregamento)?.CPF_CNPJ ?? 0;
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    List<int> filiais = Empresa.Filiais.Where(fil => fil.Matriz.Any(ma => ma.Codigo == Empresa.Codigo)).Select(fil => fil.Codigo).ToList();
                    filtroPesquisa.CodigosEmpresa.Add(Empresa.Codigo);
                    filtroPesquisa.CodigosEmpresa.AddRange(filiais);
                }
                else if (TipoServicoMultisoftware == TipoServicoMultisoftware.TransportadorTerceiro)
                    filtroPesquisa.CodigoClienteTerceiro = this.Usuario.ClienteTerceiro.Codigo;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pessoas.ClienteDescarga repositorioClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork, cancellationToken);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("CodigoVeiculo", false);
                grid.AdicionarCabecalho("CodigoMotorista", false);
                grid.AdicionarCabecalho("NaoPermitirGerarAtendimento", false);
                grid.AdicionarCabecalho("ObservacaoRelatorioDeEmbarque", false);
                grid.AdicionarCabecalho("NumeroReboques", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.TipoDeCarga, "TipoCarga", 15, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.ModeloVeicular, "ModeloVeicular", 15, Models.Grid.Align.left, false, true, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.Origem, "Origens", 15, Models.Grid.Align.left, false, true, true);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.Destino, "Destinos", 15, Models.Grid.Align.left, false, true, true);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.Cte, "NumeroCTes", 10, Models.Grid.Align.left, false, true, true);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.EmpresaFilial, "Transportador", 15, Models.Grid.Align.left, false, true, true);
                }
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.Filial, "Filial", 10, Models.Grid.Align.left, false, true, true);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.FilialVenda, "FilialVenda", 10, Models.Grid.Align.left, false, true, true);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.Transportador, "Transportador", 15, Models.Grid.Align.left, false, true, true);
                }

                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.TipoDeOperacao, "TipoOperacao", 10, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.Motorista, "Motorista", 15, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.DataCarregamento, "DataCarregamentoDescricao", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoDescricao", 10, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.DataEmissaoDocumentos, "DataEmissaoDocumentosDescricao", 10, Models.Grid.Align.center, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.DataCriacaoCarga, "DataCriacaoCargaDescricao", 10, Models.Grid.Align.center, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.ValorFrete, "ValorFrete", 10, Models.Grid.Align.right, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.QuantidadeDocumentosFrete, "QuantidadeDocumentosFrete", 10, Models.Grid.Align.right, false, true, true);

                if (filtroPesquisa.CargasNaoFechadas)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Carga.Fechada, "CargaFechadaDescricao", 10, Models.Grid.Align.center, false, true, true);

                bool parametrosInformados = true;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                {
                    double.TryParse(Usuario.ClienteTerceiro?.CPF_CNPJ_SemFormato, out double proprietarioVeiculo);
                    filtroPesquisa.ProprietarioVeiculo = proprietarioVeiculo;
                    parametrosInformados = filtroPesquisa.IsParametrosInformados();
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FiltrarCargasPorProprietario.Value)
                    {
                        filtroPesquisa.CodigosEmpresa = new List<int>();
                        double.TryParse(Usuario.Empresa?.CNPJ_SemFormato, out double proprietarioVeiculo);
                        filtroPesquisa.ProprietarioVeiculo = proprietarioVeiculo;
                        parametrosInformados = filtroPesquisa.IsParametrosInformados();
                    }
                }

                if (TipoServicoMultisoftware == TipoServicoMultisoftware.Fornecedor && telaPagametoProvedor)
                    filtroPesquisa.CpfCnpjFornecedor = Usuario.ClienteFornecedor?.CPF_CNPJ ?? 0;

                bool? permiteCTeComplementar = Request.GetNullableBoolParam("PermiteCTeComplementar");
                if (permiteCTeComplementar ?? false)
                {
                    if (filtroPesquisa.Situacao == null || filtroPesquisa.Situacao.Count() <= 0)
                    {
                        filtroPesquisa.Situacao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
                        {
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                        };

                        Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork, cancellationToken);
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = await repConfiguracaoOcorrencia.BuscarConfiguracaoPadraoAsync();

                        if (configuracaoOcorrencia?.PermiteGerarOcorrenciaCargaAnulada ?? false)
                            filtroPesquisa.Situacao.Add(SituacaoCarga.Anulada);
                    }

                    if (filtroPesquisa.CodigoTipoOcorrencia > 0)
                    {
                        Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork, cancellationToken);

                        Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = await repTipoOcorrencia.BuscarPorCodigoAsync(filtroPesquisa.CodigoTipoOcorrencia);

                        if (tipoOcorrencia != null)
                        {
                            if (tipoOcorrencia.OcorrenciaComplementoValorFreteCarga)
                            {
                                filtroPesquisa.Situacao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
                                {
                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova,
                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe,
                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete
                                };
                            }

                            if (tipoOcorrencia.SomenteCargasFinalizadas)
                            {
                                filtroPesquisa.Situacao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
                                {
                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                                };
                            }

                            if (!tipoOcorrencia.PermiteInformarValor && !tipoOcorrencia.CalculaValorPorTabelaFrete)
                                filtroPesquisa.CargasAguardandoImportacaoPreCte = true;
                        }
                    }
                }

                if (parametrosInformados)
                {
                    double cpfCNPJ = this.Usuario?.Cliente?.CPF_CNPJ ?? 0;
                    int codigoFilialRedespacho = cpfCNPJ > 0d ? await repositorioClienteDescarga.BuscarFilialRedespachoPorClienteAsync(cpfCNPJ) : 0;

                    if (filtroPesquisa.CodigosFilial == null || filtroPesquisa.CodigosFilial.Count == 0)
                        filtroPesquisa.CodigosFilial = await ObterListaCodigoFilialPermitidasOperadorLogisticaAsync(unitOfWork, cancellationToken);

                    if (filtroPesquisa.CodigosFilialVenda == null || filtroPesquisa.CodigosFilialVenda.Count == 0)
                        filtroPesquisa.CodigosFilialVenda = await ObterListaCodigoFilialVendaPermitidasOperadorLogisticaAsync(unitOfWork, cancellationToken);

                    filtroPesquisa.CodigosTipoCarga = await ObterListaCodigoTipoCargaPermitidosOperadorLogisticaAsync(unitOfWork, cancellationToken);

                    if (filtroPesquisa.CodigosTipoOperacao == null || filtroPesquisa.CodigosTipoOperacao.Count == 0)
                        filtroPesquisa.CodigosTipoOperacao = await ObterListaCodigoTipoOperacaoPermitidosOperadorLogisticaAsync(unitOfWork, cancellationToken);

                    if (codigoFilialRedespacho > 0)
                        filtroPesquisa.CodigosFilial.Add(codigoFilialRedespacho);

                    int quantidade = await repCarga.ContarConsultaCargaAsync(filtroPesquisa);
                    IList<Dominio.ObjetosDeValor.Embarcador.Carga.ConsultaCarga> listaCarga = quantidade > 0 ?
                        await repCarga.ConsultarAsync(filtroPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.ObjetosDeValor.Embarcador.Carga.ConsultaCarga>();

                    grid.setarQuantidadeTotal(quantidade);
                    grid.AdicionaRows(listaCarga);
                }
                else
                {
                    grid.setarQuantidadeTotal(0);
                    grid.AdicionaRows(new List<dynamic>());
                }

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargaPermiteCTeComplementarAsync(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                string codigoCargaEmbarcador = Request.Params("Descricao");

                int.TryParse(Request.Params("NumeroCTe"), out int numeroCTe);
                int.TryParse(Request.Params("TipoOcorrencia"), out int codigoTipoOcorrencia);
                int.TryParse(Request.Params("NumeroMDFe"), out int numeroMDFe);

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true, true, false, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 30, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Filial, "Filial", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 20, Models.Grid.Align.left, false);
                }
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Ctes, "NumeroCTes", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 40, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.EmpresaFilial, "Transportador", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Situacao, "DescricaoSituacao", 8, Models.Grid.Align.left, false);
                }

                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DataCarregamento, "DataCarregamento", 10, Models.Grid.Align.left, false, false);

                bool parametrosInformados = true;
                string proprietarioVeiculo = string.Empty;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    proprietarioVeiculo = Usuario.ClienteTerceiro != null ? Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : null;
                    if (string.IsNullOrWhiteSpace(codigoCargaEmbarcador) && numeroCTe == 0 && codigoTipoOcorrencia == 0 && numeroMDFe == 0)
                        parametrosInformados = false;
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                {
                    proprietarioVeiculo = Usuario.ClienteTerceiro != null ? Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : null;
                    if (string.IsNullOrWhiteSpace(codigoCargaEmbarcador) && numeroCTe == 0 && codigoTipoOcorrencia == 0 && numeroMDFe == 0)
                        parametrosInformados = false;
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    proprietarioVeiculo = Usuario.Empresa != null ? Usuario.Empresa.CNPJ_SemFormato : null;
                    if (string.IsNullOrWhiteSpace(codigoCargaEmbarcador) && numeroCTe == 0 && codigoTipoOcorrencia == 0 && numeroMDFe == 0)
                        parametrosInformados = false;
                }

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                };

                if (codigoTipoOcorrencia > 0)
                {
                    Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork, cancellationToken);

                    Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = await repTipoOcorrencia.BuscarPorCodigoAsync(codigoTipoOcorrencia);

                    if (tipoOcorrencia.OcorrenciaComplementoValorFreteCarga)
                    {
                        situacoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>()
                        {
                             Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova,
                             Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe,
                             Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete
                        };
                    }
                }

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (parametrosInformados)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = await repCarga.ConsultarPermiteCTeComplementarAsync(numeroMDFe, codigoCargaEmbarcador, proprietarioVeiculo, numeroCTe, situacoes, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                    int quantidade = await repCarga.ContarConsultaPermiteCTeComplementarAsync(numeroMDFe, codigoCargaEmbarcador, proprietarioVeiculo, numeroCTe, situacoes);

                    grid.setarQuantidadeTotal(quantidade);

                    var dynListaCarga = (from obj in listaCarga
                                         select new
                                         {
                                             Codigo = obj.Codigo,
                                             Descricao = obj.CodigoCargaEmbarcador,
                                             CodigoCargaEmbarcador = obj.CodigoCargaEmbarcador,
                                             Filial = obj.Filial != null ? obj.Filial.Descricao : "",
                                             OrigemDestino = serCargaDadosSumarizados.ObterOrigemDestinos(obj, false, TipoServicoMultisoftware),
                                             Transportador = obj.Empresa != null ? obj.Empresa.RazaoSocial + " (" + obj.Empresa.Localidade.DescricaoCidadeEstado + " )" : string.Empty,
                                             Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : "",
                                             DataCarregamento = obj.DataCarregamentoCarga.HasValue ? obj.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy") : "",
                                             NumeroCTes = obj.NumerosCTes,
                                             DescricaoSituacao = obj.DescricaoSituacaoCarga
                                         }).ToList();
                    grid.AdicionaRows(dynListaCarga);
                }
                else
                {
                    grid.setarQuantidadeTotal(0);
                }
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasLiberadaRetiradaContainerAsync(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                string codigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador");
                int tipoContainer = Request.GetIntParam("ContainerTipo");

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true, true, false, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 30, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Filial, "Filial", 20, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 20, Models.Grid.Align.left, false);
                }
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Ctes, "NumeroCTes", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 40, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.EmpresaFilial, "Transportador", 15, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Situacao, "DescricaoSituacao", 8, Models.Grid.Align.left, false);
                }

                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DataCarregamento, "DataCarregamento", 10, Models.Grid.Align.left, false, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                int quantidade = await repCarga.ContarCargasexpLiberadasSemRetiradaContainerAsync(tipoContainer, codigoCargaEmbarcador);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = (quantidade > 0) ?
                    await repCarga.BuscarCargasexpLiberadasSemRetiradaContainerAsync(tipoContainer, codigoCargaEmbarcador, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();


                grid.setarQuantidadeTotal(quantidade);

                var dynListaCarga = (from obj in listaCarga
                                     select new
                                     {
                                         Codigo = obj.Codigo,
                                         CodigoCargaEmbarcador = obj.CodigoCargaEmbarcador,
                                         Filial = obj.Filial != null ? obj.Filial.Descricao : "",
                                         OrigemDestino = serCargaDadosSumarizados.ObterOrigemDestinos(obj, false, TipoServicoMultisoftware),
                                         Transportador = obj.Empresa != null ? obj.Empresa.RazaoSocial + " (" + obj.Empresa.Localidade.DescricaoCidadeEstado + " )" : string.Empty,
                                         Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : "",
                                         DataCarregamento = obj.DataCarregamentoCarga.HasValue ? obj.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy") : "",
                                         NumeroCTes = obj.NumerosCTes,
                                         DescricaoSituacao = obj.DescricaoSituacaoCarga
                                     }).ToList();
                grid.AdicionaRows(dynListaCarga);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterIntegracoesCargaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork, cancellationToken);

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracao> integracoes = await repCargaIntegracao.BuscarPorCargaAsync(codigoCarga);

                var retorno = (from obj in integracoes select obj.TipoIntegracao.Tipo).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterAsIntegracoesDisponiveisParaCarga);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CancelarOrdemEmbarqueAsync(CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                int codigoOrdemEmbarque = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork, cancellation);
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(unitOfWork, cancellation);
                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque ordemEmbarque = await repositorioOrdemEmbarque.BuscarPorCodigoAsync(codigoOrdemEmbarque, false);
                string retorno = servicoIntegracaoOrdemEmbarqueMarfrig.IntegrarCancelamentoOrdemEmbarque(ordemEmbarque, Localization.Resources.Cargas.Carga.ApenasOrdemDeEmbarqueFoiCanceladaAtravesDaTelaDeCarga, Usuario);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ordemEmbarque.Carga, string.Format(Localization.Resources.Cargas.Carga.OrdemDeEmbarqueCancelada, ordemEmbarque.Numero), unitOfWork);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true, true, retorno);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoCancelarOrdemDeEmbarque);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterOrdemEmbarqueDetalhesAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque repositorioOrdemEmbarque = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico repositorioOrdemEmbarqueSituacaoHistorico = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao repositorioOrdemEmbarqueSituacao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao repositorioOrdemEmbarqueHistoricoIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarque> ordensEmbarque = await repositorioOrdemEmbarque.BuscarPorCargaAsync(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacaoHistorico> situacoesHistorico = await repositorioOrdemEmbarqueSituacaoHistorico.BuscarPorCargaAsync(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao> historicosIntegracao = await repositorioOrdemEmbarqueHistoricoIntegracao.BuscarPorCargaAsync(codigoCarga);

                var ordensEmbarqueRetornar = (
                    from ordemEmbarque in ordensEmbarque
                    select new
                    {
                        Dados = new
                        {
                            ordemEmbarque.Codigo,
                            ordemEmbarque.Descricao,
                            Situacao = ordemEmbarque.Situacao?.Descricao,
                            PermiteCancelar = !repositorioOrdemEmbarqueSituacao.SituacaoEhCancelada(ordemEmbarque.Situacao) && !repositorioOrdemEmbarqueSituacao.SituacaoEhEmCancelamento(ordemEmbarque.Situacao)
                        },
                        SituacoesHistorico = (
                            from o in situacoesHistorico
                            where o.OrdemEmbarque.Codigo == ordemEmbarque.Codigo
                            select new
                            {
                                o.Codigo,
                                o.Situacao.CodigoIntegracao,
                                DataAtualizacao = o.DataAtualizacao.ToString("dd/MM/yyyy HH:mm"),
                                DataCriacao = o.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                                Situacao = o.Situacao.Descricao
                            }
                        ).ToList(),
                        HistoricosIntegracao = (
                            from historicoIntegracao in historicosIntegracao
                            where historicoIntegracao.OrdemEmbarque.Codigo == ordemEmbarque.Codigo
                            select new
                            {
                                historicoIntegracao.Codigo,
                                historicoIntegracao.ProblemaIntegracao,
                                SituacaoIntegracao = historicoIntegracao.SituacaoIntegracao.ObterDescricao(),
                                Tipo = historicoIntegracao.Tipo.ObterDescricao(),
                                DataIntegracao = historicoIntegracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm")
                            }
                        ).ToList()
                    }
                ).ToList();

                return new JsonpResult(ordensEmbarqueRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterOsDetalhesDasOrdensDeEmbarque);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaOrdemEmbarqueHistoricoIntegracaoAsync(CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Data, "Data", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Tipo, "DescricaoTipo", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Retorno, "Mensagem", 40, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao(unitOfWork, cancellation);
                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao integracao = await repositorioIntegracao.BuscarPorCodigoAsync(codigo, auditavel: false);

                var arquivosTransacaoRetornar = (
                    from arquivoTransacao in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                    select new
                    {
                        arquivoTransacao.Codigo,
                        Data = arquivoTransacao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                        arquivoTransacao.DescricaoTipo,
                        arquivoTransacao.Mensagem
                    }
                ).ToList();

                grid.AdicionaRows(arquivosTransacaoRetornar);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosOrdemEmbarqueHistoricoIntegracaoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque
                    .OrdemEmbarqueHistoricoIntegracao(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao integracao = await repositorioIntegracao.BuscarPorCodigoArquivoAsync(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.Carga.HistoricoNaoEncontrado);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if ((arquivoIntegracao == null) || ((arquivoIntegracao.ArquivoRequisicao == null) && (arquivoIntegracao.ArquivoResposta == null)))
                    return new JsonpResult(true, false, Localization.Resources.Cargas.Carga.NaoHaRegistrosDeArquivosSalvosParaEsteHistoricoDeConsulta);

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos do {integracao.Descricao}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoRealizarDownloadDosArquivosDeIntegracao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDatasCarregamentoAsync(CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellation);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork, cancellation);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = await repositorioCarga.BuscarCargasOriginaisAsync(codigoCarga);
                List<int> codigosCargas = (from o in cargasOriginais select o.Codigo).ToList();

                codigosCargas.Add(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCargas(codigosCargas);

                var cargasJanelaCarregamentoRetornar = (
                    from o in cargasJanelaCarregamento
                    select new
                    {
                        Carga = o.Carga.Codigo,
                        CentroCarregamento = o.CentroCarregamento?.Descricao ?? "",
                        InicioCarregamento = o.InicioCarregamento.ToString("dd/MM/yyyy HH:mm"),
                        TerminoCarregamento = o.Carga.DataPrevisaoTerminoCarga?.ToString("")
                    }
                ).ToList();

                return new JsonpResult(cargasJanelaCarregamentoRetornar);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterAsDatasDeCarregamentoDaCarga);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCargaPorCodigoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Carga"));
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigo);

                return new JsonpResult(serCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork, usuario: this.Usuario));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreUmaFalhaAoBuscarPorCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfirmarMensagemAlertaAsync()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork, Auditado);

                await servicoMensagemAlerta.ConfirmarPorCodigoAsync(codigo);

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
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a confirmação.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> PesquisaCargasAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                bool permitirLiberarComProblemaIntegracaoGrMotoristaVeiculo = Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ConfirmarIntegracao);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = await repConfiguracaoGeral.BuscarConfiguracaoPadraoAsync();
                bool existePacote = await repPacote.ExistePacoteAsync();

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await repOperadorLogistica.BuscarPorUsuarioAsync(this.Usuario.Codigo);
                if (operadorLogistica != null || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = null;
                    int quantidade = 0;
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        DirecaoOrdenar = Request.GetBoolParam("OrdenacaoAsc") ? "asc" : "desc",
                        InicioRegistros = Request.GetIntParam("inicio"),
                        LimiteRegistros = Request.GetIntParam("limite"),
                        PropriedadeOrdenar = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) ? "CodigoCargaEmbarcador" : "Codigo"
                    };

                    ExecutarBusca(ref listaCarga, ref quantidade, parametroConsulta, configuracaoGeral);

                    Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig repTipoCargaModeloVeicularAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig(unitOfWork, cancellationToken);
                    Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig tipoCargaModeloVeicularAutoConfig = repTipoCargaModeloVeicularAutoConfig.Buscar();

                    List<dynamic> lista = await serCarga.ObterDetalhesDaCargasAsync(cancellationToken, listaCarga, ConfiguracaoEmbarcador, TipoServicoMultisoftware, existePacote, unitOfWork,
                        permitirLiberarComProblemaIntegracaoGrMotoristaVeiculo, tipoCargaModeloVeicularAutoConfig, null, this.Usuario);

                    return new JsonpResult(lista, quantidade);
                }
                else
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.UsuarioNaoOperadorDaLogistica);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> PesquisaCargasNaGridAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = await repConfiguracaoGeral.BuscarConfiguracaoPadraoAsync();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "Carga/PesquisaCargasNaGrid", "grid-carga-agrupada");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));


                grid.header = new List<Models.Grid.Head>();
                //grid.AdicionarCabecalho("Nº da Carga", "Codigo", 8, Models.Grid.Align.center, true);

                bool somentePermiteAgrupamento = false;
                bool.TryParse(Request.Params("SomentePermiteAgrupamento"), out somentePermiteAgrupamento);

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("EmpresaCodigo", false);
                grid.AdicionarCabecalho("RaizCNPJEmpresa", false);
                grid.AdicionarCabecalho("PlacaDeAgrupamento", false);
                grid.AdicionarCabecalho("CargaDePreCarga", false);
                grid.AdicionarCabecalho("ExigeConfirmacaoTracao", false);
                grid.AdicionarCabecalho("NumeroReboques", false);
                grid.AdicionarCabecalho("ModeloVeicularOrigem", false);
                grid.AdicionarCabecalho("ZonaTransporte", false);
                grid.AdicionarCabecalho("FilialCodigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", somentePermiteAgrupamento ? 15 : 10, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 30, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Filial, "Filial", 20, Models.Grid.Align.left, false, !somentePermiteAgrupamento);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 20, Models.Grid.Align.left, false);
                }
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 50, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.EmpresaFilial, "Transportador", 20, Models.Grid.Align.left, false);
                }

                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DataCarregamento, "DataCarregamento", 10, Models.Grid.Align.left, false, !somentePermiteAgrupamento);

                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Volumes, "VolumesTotal", 8, Models.Grid.Align.right, false, somentePermiteAgrupamento);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Peso, "PesoTotal", 8, Models.Grid.Align.right, false, somentePermiteAgrupamento);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ZonaDeTransporte, "ZonaTransporteDadosSumarizados", 8, Models.Grid.Align.right, false, somentePermiteAgrupamento);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Cubagem, "Cubagem", 8, Models.Grid.Align.right, false, somentePermiteAgrupamento);

                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = null;
                int quantidade = 0;
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                ExecutarBusca(ref listaCarga, ref quantidade, parametrosConsulta, configuracaoGeral);
                grid.setarQuantidadeTotal(quantidade);

                var dynListaCarga = (from obj in listaCarga
                                     select new
                                     {
                                         obj.Codigo,
                                         ExigeConfirmacaoTracao = obj.TipoOperacao?.ExigePlacaTracao ?? false,
                                         NumeroReboques = obj.ModeloVeicularCarga?.NumeroReboques ?? 0,
                                         DT_RowColor = obj.CargaDePreCarga ? "#D3D3D3" : "",
                                         obj.CargaDePreCarga,
                                         obj.CodigoCargaEmbarcador,
                                         Filial = obj.DadosSumarizados?.Filiais ?? "",
                                         obj.PlacaDeAgrupamento,
                                         ModeloVeicularOrigem = obj.ModeloVeicularOrigem?.Descricao ?? obj.ModeloVeicularCarga?.Descricao ?? "",
                                         EmpresaCodigo = obj.Empresa?.Codigo ?? 0,
                                         RaizCNPJEmpresa = !string.IsNullOrWhiteSpace(obj.Empresa?.CNPJ_SemFormato) ? obj.Empresa.CNPJ_SemFormato.Remove(8, 6) : "",
                                         OrigemDestino = serCargaDadosSumarizados.ObterOrigemDestinos(obj, false, TipoServicoMultisoftware),
                                         Transportador = obj.Empresa != null ? obj.Empresa.RazaoSocial + " (" + obj.Empresa.Localidade.DescricaoCidadeEstado + " )" : string.Empty,
                                         Veiculo = !string.IsNullOrWhiteSpace(obj.PlacaDeAgrupamento) ? obj.PlacaDeAgrupamento : (obj.Veiculo != null ? obj.Veiculo.Placa : ""),
                                         DataCarregamento = obj.DataCarregamentoCarga.HasValue ? obj.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy") : "",
                                         VolumesTotal = obj.DadosSumarizados?.VolumesTotal ?? 0,
                                         PesoTotal = obj.DadosSumarizados?.PesoTotal ?? 0,
                                         ZonaTransporte = ObterZonaTransporteCarga(obj, unitOfWork),
                                         Cubagem = obj.DadosSumarizados?.CubagemTotal ?? 0,
                                         ZonaTransporteDadosSumarizados = obj.DadosSumarizados?.ZonasTransporte ?? string.Empty,
                                         FilialCodigo = obj.Filial?.Codigo ?? 0,
                                     }).ToList();

                grid.AdicionaRows(dynListaCarga);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDetalhesDaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("codigo");
                var culture = CultureInfo.CreateSpecificCulture("pt-BR");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = BuscarListaCargaPedidoOrdenada(carga, unitOfWork);

                List<dynamic> listaPedidoRetornar = new List<dynamic>();

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork);
                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                Servicos.Embarcador.Carga.OcultarInformacoesCarga.OcultarInformacoesCarga serOcultarInformacoesCarga = new Servicos.Embarcador.Carga.OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao repositorioOrdemEmbarqueHistoricoIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento repositorioPedidoDadosTransporteMaritimoRoteamento = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoTroca repositorioPedidoTroca = new Repositorio.Embarcador.Pedidos.PedidoTroca(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProdutoLote repositorioPedidoProdutoLote = new Repositorio.Embarcador.Pedidos.PedidoProdutoLote(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                bool exibirHoraECodigosIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork).BuscarConfiguracaoPadrao().ExibirHoraCarregamentoEmPedidosDeColetaECodigosIntegracao;

                List<int> codigosPedido = (from o in cargaPedidos select o.Pedido.Codigo).ToList();
                List<int> codigosDosPedidosDestinadosAFiliais = repositorioPedido.BuscarCodigosDosPedidosDestinadosAFiliais(codigosPedido);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> pedidosDadosTransporteMaritimo = codigosPedido.Count > 0 ? repositorioPedidoDadosTransporteMaritimo.BuscarPorPedidos(codigosPedido) : new List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoRoteamento> pedidoDadosTransporteMaritimoRoteamentos = repositorioPedidoDadosTransporteMaritimoRoteamento.BuscarPorPedidos(codigosPedido);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca> pedidosTroca = codigosPedido.Count > 0 ? repositorioPedidoTroca.BuscarPorPedidosDefinitivos(codigosPedido) : new List<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> janelasDescarregamento = repositorioCargaJanelaDescarregamento.BuscarTodasPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> xmlNotasFiscais = cargaPedidos.Count > 0 ? repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedidos((from obj in cargaPedidos select obj.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> pedidosAdicional = repositorioPedidoAdicional.BuscarPorPedidos(codigosPedido);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                bool usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1 = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm);
                bool exigirDefinicaoReboquePedido = (carga.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false) && (carga.ModeloVeicularCarga?.NumeroReboques > 1);
                bool possuiOrdemEmbarque = servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarque(carga);
                bool possuiOrdemEmbarqueAguardandoRetornoIntegracao = servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarqueAguardandoRetornoIntegracao(carga);
                bool cargaBloqueada = Servicos.Embarcador.Carga.Carga.IsCargaBloqueada(carga, unitOfWork);
                bool cargaPermiteAdicionarOuRemoverPedido = false;

                if (carga.ExigeNotaFiscalParaCalcularFrete)
                    cargaPermiteAdicionarOuRemoverPedido = (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.AgNFe || carga.SituacaoCarga == SituacaoCarga.CalculoFrete);
                else
                    cargaPermiteAdicionarOuRemoverPedido = (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.CalculoFrete || carga.SituacaoCarga == SituacaoCarga.AgTransportador || carga.SituacaoCarga == SituacaoCarga.AgNFe);

                if (carga.CargaDePreCargaEmFechamento || possuiOrdemEmbarqueAguardandoRetornoIntegracao || cargaBloqueada)
                    cargaPermiteAdicionarOuRemoverPedido = false;

                bool permitirAdicionarOuRemoverPedido = (carga.TipoOperacao?.PermitirAdicionarRemoverPedidosEtapa1 ?? false) && cargaPermiteAdicionarOuRemoverPedido && usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1;
                bool permitirAdicionarPedidoOutraFilial = ConfiguracaoEmbarcador.PermitirAdicionarPedidoOutraFilialCarga && cargaPermiteAdicionarOuRemoverPedido && usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1;
                bool permitirAdicionarPedidoTroca = false;
                bool permitirRemoverPedidoCargaComPendenciaDocumentos = ConfiguracaoEmbarcador.PermitirRemoverPedidoCargaComPendenciaDocumentos && (cargaPermiteAdicionarOuRemoverPedido || (carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && carga.AgImportacaoCTe)) && usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1;
                bool permitirTrocarPedidoPedido = ConfiguracaoEmbarcador.PermitirTrocarPedidoCarga && cargaPermiteAdicionarOuRemoverPedido && usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1;

                if (servicoCarga.RecebeuNumeroCargaEmbarcador(carga, unitOfWork))
                    permitirAdicionarOuRemoverPedido = false;

                bool permitirAdicionarPedido = !permitirAdicionarPedidoOutraFilial && !permitirTrocarPedidoPedido && permitirAdicionarOuRemoverPedido;
                bool permitirAdicionarPedidoMesmaFilial = (permitirAdicionarPedidoOutraFilial || permitirTrocarPedidoPedido) && permitirAdicionarOuRemoverPedido;
                bool permitirAdicionarNovosPedidosPorNotasAvulsas = (carga.TipoOperacao?.ConfiguracaoCarga?.PermitirAdicionarNovosPedidosPorNotasAvulsas ?? false) && cargaPermiteAdicionarOuRemoverPedido && usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1;

                bool possuiOcultarInformacoesCarga = serOcultarInformacoesCarga.PossuiOcultarInformacoesCarga(this.Usuario.Codigo);
                Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga = null;
                if (possuiOcultarInformacoesCarga)
                    ocultarInformacoesCarga = serOcultarInformacoesCarga.ObterOcultarInformacoesCarga(this.Usuario.Codigo);


                string numeroProtocoloIntegracaoCarga = cargaPedidos.Where(obj => obj.CargaOrigem.SituacaoCarga != SituacaoCarga.Cancelada && obj.CargaOrigem.SituacaoCarga != SituacaoCarga.Anulada && obj.CargaOrigem.Protocolo > 0).Select(obj => obj.CargaOrigem.Protocolo).FirstOrDefault().ToString();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimo = pedidosDadosTransporteMaritimo.Where(o => o.Pedido.Codigo == cargaPedido.Pedido.Codigo).FirstOrDefault();
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProduto> produtosCargaPedido = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProduto>();
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote> pedidoProdutoLotes = cargaPedidos.Count < 100 ? repositorioPedidoProdutoLote.BuscarPedidoProdutosLotesPorPedidoProduto((from p in cargaPedido.Pedido.Produtos select p.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProdutoLote>();
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosProvisorio = (from o in pedidosTroca where o.PedidoDefinitivo.Codigo == cargaPedido.Pedido.Codigo select o.PedidoProvisorio).ToList();
                    Dominio.Entidades.Cliente destinatario = (cargaPedido.Recebedor != null) ? cargaPedido.Recebedor : cargaPedido.Pedido.Destinatario;
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento janelaDescarregamento = (from o in janelasDescarregamento where o.CentroDescarregamento != null && o.CentroDescarregamento.Destinatario.CPF_CNPJ == (destinatario?.CPF_CNPJ ?? 0d) select o).FirstOrDefault();
                    bool pedidoProvisorio = IsPedidoProvisorio(cargaPedido.Pedido, ConfiguracaoEmbarcador);
                    bool permitirDesfazerTrocaPedido = !carga.CargaAgrupada && permitirTrocarPedidoPedido && (pedidosProvisorio.Count > 0);
                    bool permitirTrocarPedido = (!carga.CargaAgrupada && permitirTrocarPedidoPedido && !permitirDesfazerTrocaPedido);
                    bool permitirRemoverPedido = !permitirDesfazerTrocaPedido && (permitirAdicionarOuRemoverPedido || permitirRemoverPedidoCargaComPendenciaDocumentos);
                    bool pedidoDestinadoAFilial = codigosDosPedidosDestinadosAFiliais.Contains(cargaPedido.Pedido.Codigo);

                    if (permitirTrocarPedido)
                    {
                        permitirAdicionarPedidoTroca = true;

                        if (pedidoProvisorio)
                            permitirAdicionarPedidoOutraFilial = false;
                    }

                    if (possuiOrdemEmbarque && (permitirDesfazerTrocaPedido || permitirRemoverPedido))
                    {
                        Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao ultimoHistoricoIntegracaoTrocaPedido = repositorioOrdemEmbarqueHistoricoIntegracao.BuscarUltimoPorTrocaPedido(carga.Codigo, cargaPedido.Pedido.Codigo, cargaPedido.NumeroReboque);

                        if (ultimoHistoricoIntegracaoTrocaPedido?.SituacaoIntegracao.IntegracaoPendente() ?? false)
                        {
                            permitirDesfazerTrocaPedido = false;
                            permitirRemoverPedido = false;
                        }
                    }

                    if (ConfiguracaoEmbarcador.AtualizarProdutosPedidoPorIntegracao)
                    {
                        if (cargaPedidos.Count < 100)
                        {
                            produtosCargaPedido = (
                                from p in cargaPedido.Pedido.Produtos
                                select new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProduto
                                {
                                    Produto = p.Produto.CodigoProdutoEmbarcador + " - " + p.Produto.Descricao,
                                    Quantidade = p.Quantidade,
                                    QuantidadePlanejada = p.QuantidadePlanejada,
                                    PesoUnitario = p.PesoUnitario,
                                    LinhaSeparacao = p.LinhaSeparacao?.Descricao,
                                    Valor = p.ValorProduto * p.Quantidade,
                                    ValorUnitario = p.ValorProduto,
                                    PesoTotalEmbalagem = p.PesoTotalEmbalagem,
                                    NumeroLotePedidoProdutoLote = pedidoProdutoLotes != null ? string.Join(", ", (from o in pedidoProdutoLotes where o.PedidoProduto.Codigo == p.Codigo select o.NumeroLote).ToList()) : string.Empty,
                                    ValorTotal = p.ValorTotal,
                                }
                            ).ToList();
                        }
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> produtos = repositorioCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

                        produtosCargaPedido = (
                            from p in produtos
                            select new Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoProduto
                            {
                                Produto = p.Produto.CodigoProdutoEmbarcador + " - " + p.Produto.Descricao,
                                Quantidade = p.Quantidade,
                                QuantidadePlanejada = p.QuantidadePlanejada,
                                PesoUnitario = p.PesoUnitario,
                                Valor = p.ValorUnitarioProduto * p.Quantidade,
                                ValorUnitario = p.ValorUnitarioProduto,
                                PesoTotalEmbalagem = p.PesoTotalEmbalagem,
                                NumeroLotePedidoProdutoLote = pedidoProdutoLotes != null ? string.Join(", ", (from o in pedidoProdutoLotes where o.PedidoProduto.Codigo == p.Codigo select o.NumeroLote).ToList()) : string.Empty,
                                ValorTotal = p.ValorTotal,
                            }
                        ).ToList();
                    }

                    string endereco = cargaPedido.Pedido.EnderecoDestino?.Endereco + ", " + (cargaPedido.Pedido.EnderecoDestino?.Numero.ToString() ?? "") + " " + cargaPedido.Pedido.EnderecoDestino?.Complemento + ", " + cargaPedido.Pedido.EnderecoDestino?.Bairro + ", " + Localization.Resources.Cargas.Carga.CEP + ": " + cargaPedido.Pedido.EnderecoDestino?.CEP + " - " + cargaPedido.Pedido.EnderecoDestino?.Localidade?.DescricaoCidadeEstado ?? "";
                    string enderecoRemetente = cargaPedido.Pedido.EnderecoOrigem?.Endereco + ", " + (cargaPedido.Pedido.EnderecoOrigem?.Numero.ToString() ?? "") + cargaPedido.Pedido.EnderecoOrigem?.Complemento + ", " + cargaPedido.Pedido.EnderecoOrigem?.Bairro + ", " + Localization.Resources.Cargas.Carga.CEP + ": " + cargaPedido.Pedido.EnderecoOrigem?.CEP + " - " + cargaPedido.Pedido.EnderecoOrigem?.Localidade?.DescricaoCidadeEstado;

                    if (cargaPedido.Pedido.UsarOutroEnderecoDestino)
                        endereco = CriarObjetoDetalheOutroEndereco(cargaPedido.Pedido.EnderecoDestino.ClienteOutroEndereco);

                    if (cargaPedido.Pedido.UsarOutroEnderecoOrigem)
                        enderecoRemetente = CriarObjetoDetalheOutroEndereco(cargaPedido.Pedido.EnderecoOrigem.ClienteOutroEndereco);

                    Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = cargaPedido.Pedido.Destinatario?.ClienteDescargas.FirstOrDefault() ?? new Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga();

                    string codigoRecebedor = "", codigoExpedidor = "", codigoRemetente = "", codigoDestinatario = "";

                    if (exibirHoraECodigosIntegracao)
                    {
                        codigoRecebedor = cargaPedido.Recebedor != null ? cargaPedido.Recebedor.CodigoIntegracao + " - " : "";
                        codigoExpedidor = cargaPedido.Expedidor != null ? cargaPedido.Expedidor.CodigoIntegracao + " - " : "";
                        codigoRemetente = cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.CodigoIntegracao + " - " : "";
                        codigoDestinatario = cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.CodigoIntegracao + " - " : "";
                    }

                    dynamic pedidoRetornar = new
                    {
                        DetalhesPedido = new
                        {
                            DataPrevisaoTerminoCarregamento = cargaPedido.Pedido?.DataPrevisaoTerminoCarregamento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                            Origem = new { cargaPedido.Origem?.Codigo, Descricao = cargaPedido.Origem?.DescricaoCidadeEstado },
                            Destino = new { Codigo = cargaPedido.Destino != null ? cargaPedido.Pedido.Codigo : 0, Descricao = cargaPedido.Destino != null ? cargaPedido.Destino.DescricaoCidadeEstado : "" },
                            Filial = new { Codigo = cargaPedido.Pedido.Filial != null ? cargaPedido.Pedido.Filial.Codigo : 0, Descricao = cargaPedido.Pedido.Filial != null ? cargaPedido.Pedido.Filial.Descricao : "" },
                            FilialVenda = new { Codigo = cargaPedido.Pedido.FilialVenda != null ? cargaPedido.Pedido.FilialVenda.Codigo : 0, Descricao = cargaPedido.Pedido.FilialVenda != null ? cargaPedido.Pedido.FilialVenda.Descricao : "" },
                            GrupoRemetente = new { Codigo = cargaPedido.Pedido.GrupoPessoas != null ? cargaPedido.Pedido.GrupoPessoas.Codigo : 0, Descricao = cargaPedido.Pedido.GrupoPessoas != null ? cargaPedido.Pedido.GrupoPessoas.Descricao : "" },
                            Remetente = new { Codigo = cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.CPF_CNPJ : 0, Descricao = cargaPedido.Pedido.Remetente != null ? codigoRemetente + cargaPedido.Pedido.Remetente.NomeCNPJ : "", CPF_CNPJ = cargaPedido.Pedido.Remetente != null ? cargaPedido.Pedido.Remetente.CPF_CNPJ_Formatado : "" },
                            Destinatario = new { Codigo = cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.CPF_CNPJ : 0, Descricao = cargaPedido.Pedido.Destinatario != null ? codigoDestinatario + cargaPedido.Pedido.Destinatario.NomeCNPJ : "", CPF_CNPJ = cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido.Destinatario.CPF_CNPJ_Formatado : "" },
                            DestinatarioCidade = cargaPedido.Pedido.Destinatario != null ? cargaPedido.Pedido?.Destinatario?.Localidade?.DescricaoCidadeEstado.ToString() : " ",
                            cargaPedido.Pedido.Codigo,
                            cargaPedido.Pedido.PesoTotalPaletes,
                            RecebedorEndereco = cargaPedido.Pedido?.Recebedor?.EnderecoCompletoCidadeeEstado.ToString() ?? "",
                            RemetenteEndereco = enderecoRemetente,
                            ValorFrete = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, Math.Round(cargaPedido.ValorFreteAPagar, 2, MidpointRounding.AwayFromZero)) : Math.Round(cargaPedido.ValorFreteAPagar, 2, MidpointRounding.AwayFromZero),
                            NumeroPaletes = (cargaPedido.Pedido.NumeroPaletes + cargaPedido.Pedido.NumeroPaletesFracionado).ToString("n2"),
                            PrevisaoEntrega = cargaPedido.Pedido.PrevisaoEntrega != null ? cargaPedido.Pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm") : "",
                            DataAgendamentoEntregaPedido = cargaPedido.Pedido.DataAgendamento.HasValue ? cargaPedido.Pedido.DataAgendamento.Value.ToString("dd/MM/yyyy HH:mm") : "",
                            Cubagem = cargaPedido.Pedido.CubagemTotal.ToString("n2"),
                            NotasFiscais = string.Join(", ", (from o in xmlNotasFiscais where o.CargaPedido.Codigo == cargaPedido.Codigo select o.XMLNotaFiscal.Numero).ToList()),
                            PrevisaoSaida = cargaPedido.Pedido.DataPrevisaoSaida != null ? cargaPedido.Pedido.DataPrevisaoSaida.Value.ToString("dd/MM/yyyy HH:mm") : "",
                            cargaPedido.Pedido.SituacaoPedido,
                            NumeroPedido = cargaPedido.Pedido.Numero.ToString("D"),
                            RecebeuDadosPreCalculoFrete = cargaPedido.RecebeuDadosPreCalculoFrete ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                            cargaPedido.Pedido.NumeroPedidoEmbarcador,
                            PesoTotal = cargaPedido.Peso.ToString("N", culture),
                            PesoLiquido = cargaPedido.PesoLiquido.ToString("N", culture),
                            cargaPedido.Pedido.ProdutoPredominante,
                            ValorPedido = (from p in produtosCargaPedido select p.Valor).Sum().ToString("N", culture),
                            Temperatura = cargaPedido.Pedido.Temperatura ?? "",
                            Vendedor = cargaPedido.Pedido.Vendedor ?? "",
                            Ordem = cargaPedido.Pedido.Ordem ?? "",
                            Ajudante = cargaPedido.Pedido.QtdAjudantes.ToString(),
                            ExigeAjudantes = cargaPedido.Pedido.Ajudante ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                            TipoTomador = cargaPedido.DescricaoTipoPagamentoCIFFOB,
                            PortoSaida = cargaPedido.Pedido.PortoSaida ?? "",
                            PortoChegada = cargaPedido.Pedido.PortoChegada ?? "",
                            Companhia = cargaPedido.Pedido.Companhia ?? "",
                            NumeroNavio = cargaPedido.Pedido.NumeroNavio ?? "",
                            Reserva = cargaPedido.Pedido.Reserva ?? "",
                            Resumo = cargaPedido.Pedido.Resumo ?? "",
                            TipoEmbarque = cargaPedido.Pedido.TipoEmbarque ?? "",
                            DeliveryTerm = cargaPedido.Pedido.DeliveryTerm ?? "",
                            IdAutorizacao = cargaPedido.Pedido.IdAutorizacao ?? "",
                            DataETA = cargaPedido.Pedido.DataETA.HasValue ? cargaPedido.Pedido.DataETA.Value.ToString("dd/MM/yyyy HH:mm") : "",
                            DataInclusaoBooking = cargaPedido.Pedido.DataInclusaoBooking.HasValue ? cargaPedido.Pedido.DataInclusaoBooking.Value.ToString("dd/MM/yyyy HH:mm") : "",
                            DataInclusaoPCP = cargaPedido.Pedido.DataInclusaoPCP.HasValue ? cargaPedido.Pedido.DataInclusaoPCP.Value.ToString("dd/MM/yyyy HH:mm") : "",
                            NumeroContainer = cargaPedido.Pedido.Container != null ? (cargaPedido.Pedido.Container.Numero + " (" + cargaPedido.Pedido.LacreContainerUm + " " + cargaPedido.Pedido.LacreContainerDois + " " + cargaPedido.Pedido.LacreContainerTres + ")") : "",
                            PedidoViagemNavio = cargaPedido.Pedido.PedidoViagemNavio?.Descricao ?? "",
                            cargaPedido.Pedido.NumeroBooking,
                            PortoOrigem = cargaPedido.Pedido.Porto?.Descricao ?? "",
                            cargaPedido.NumeroReboque,
                            NumeroReboqueDescricao = cargaPedido.NumeroReboque.ObterDescricao(),
                            cargaPedido.TipoCarregamentoPedido,
                            TipoCarregamentoPedidoDescricao = cargaPedido.TipoCarregamentoPedido.ObterDescricao(),
                            PedidoDestinadoAFilial = pedidoDestinadoAFilial,
                            PortoDestino = cargaPedido.Pedido.PortoDestino?.Descricao ?? "",
                            EmpresaResponsavel = cargaPedido.Pedido.PedidoEmpresaResponsavel?.Descricao ?? "",
                            CentroCusto = cargaPedido.Pedido.PedidoCentroCusto?.Descricao ?? "",
                            PedidoTrocaNota = cargaPedido.Pedido.PedidoTrocaNota ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                            cargaPedido.Pedido.NumeroPedidoTrocaNota,
                            PedidoOriginal = string.Join(", ", (from o in pedidosProvisorio where !IsPedidoProvisorio(o, ConfiguracaoEmbarcador) select o.NumeroPedidoEmbarcador).ToList()) ?? "",
                            PedidoProvisorio = string.Join(", ", (from o in pedidosProvisorio where IsPedidoProvisorio(o, ConfiguracaoEmbarcador) select o.NumeroPedidoEmbarcador).ToList()) ?? "",
                            FuncionarioVendedor = cargaPedido.Pedido.FuncionarioVendedor?.DescricaoTelefoneEmail ?? "",
                            DataDescarregamento = (janelaDescarregamento != null && !janelaDescarregamento.Excedente) ? janelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm") : "",
                            QuantidadeVolumes = cargaPedido.Pedido.QtVolumes.ToString(),
                            ZonaTransporte = (from o in pedidosAdicional where o.Pedido.Codigo == cargaPedido.Pedido.Codigo && o.ZonaTransporte != null select o.ZonaTransporte.Descricao)?.FirstOrDefault() ?? "",
                            QuantidadeVolumesNF = ConfiguracaoEmbarcador.ExibirQuantidadeVolumesNF ? (from o in cargaPedido.NotasFiscais where o.XMLNotaFiscal.nfAtiva select o.XMLNotaFiscal.Volumes).Sum() : 0,
                            VolumesDaNF = cargaPedido.QuantidadeVolumesNF,
                            ExpedidorEndereco = cargaPedido?.Expedidor?.EnderecoCompletoCidadeeEstado ?? string.Empty,
                            RestricoesCliente = cargaPedido?.Pedido?.Destinatario != null ? string.Join(", ", cargaPedido.Pedido.Destinatario?.ClienteDescargas.SelectMany(x => x.RestricoesDescarga.Select(y => y.Descricao))) : string.Empty,
                            Agendado = clienteDescarga?.ExigeAgendamento ?? false ? "Sim" : "Não",
                            AgendadoComNF = clienteDescarga?.AgendamentoExigeNotaFiscal ?? false ? "Sim" : "Não",
                            DataAgendamento = cargaPedido.Pedido.DataAgendamento != null ? cargaPedido.Pedido.DataAgendamento.Value.ToString("dd/MM/yyyy HH:mm") : "",
                            Produtos = (
                                from p in produtosCargaPedido
                                select new
                                {
                                    p.Produto,
                                    Quantidade = p.Quantidade.ToString("N", culture),
                                    PesoUnitario = p.PesoUnitario.ToString("N3", culture),
                                    PesoTotalEmbalagem = p.PesoTotalEmbalagem.ToString("N3", culture),
                                    PesoTotal = p.PesoTotal.ToString("N", culture),
                                    p.LinhaSeparacao,
                                    Valor = p.Valor.ToString("N", culture),
                                    ValorUnitario = p.ValorUnitario.ToString("N", culture),
                                    p.NumeroLotePedidoProdutoLote,
                                    ValorTotal = p.ValorTotal.ToString("N", culture)
                                }
                            ).ToList(),
                            QuantidadeItensProdutos = (!ConfiguracaoEmbarcador?.NaoConsiderarProdutosSemPesoParaSumarizarVolumes ?? false) ? produtosCargaPedido.Sum(x => x.Quantidade).ToString("n2") : produtosCargaPedido.Where(o => o.PesoUnitario > 0).Sum(x => x.Quantidade).ToString("n2"),
                            PermitirDesfazerTrocaPedido = permitirDesfazerTrocaPedido,
                            PermitirTrocarPedidoPedido = permitirTrocarPedido,
                            PermitirRemoverPedido = permitirRemoverPedido,
                            Observacao = cargaPedido.Pedido.Observacao ?? "",
                            Endereco = endereco,
                            PLPCorreios = cargaPedido.Pedido.PLPCorreios ?? "",
                            NumeroEtiquetaCorreios = cargaPedido.Pedido.NumeroEtiquetaCorreios ?? "",
                            Protocolo = cargaPedido.Pedido.Protocolo > 0 ? cargaPedido.Pedido.Protocolo.ToString() : string.Empty,
                            Recebedor = codigoRecebedor + cargaPedido?.Recebedor?.NomeCNPJ ?? string.Empty,
                            Expedidor = codigoExpedidor + cargaPedido?.Expedidor?.NomeCNPJ ?? string.Empty,
                            cargaPedido.Pedido.PossuiStage,
                            SenhaAgendamentoCliente = cargaPedido.Pedido?.SenhaAgendamentoCliente ?? string.Empty,
                            LeadTime = cargaPedido.Pedido?.LeadTime ?? 0,
                            LeadTimeTransportador = cargaPedido.Pedido?.DiasUteisPrazoTransportador ?? 0,
                            LeadTimeFilialEmissora = cargaPedido.Pedido?.LeadTimeFilialEmissora ?? 0,
                            PrevisaoEntregaFilialEmissora = cargaPedido.Pedido.PrevisaoEntregaFilialEmissora != null ? cargaPedido.Pedido.PrevisaoEntregaFilialEmissora.Value.ToString("dd/MM/yyyy HH:mm") : "",
                            TipoCargaTrecho = cargaPedido.Carga.DadosSumarizados?.CargaTrecho ?? CargaTrechoSumarizada.Agrupadora,
                            TipoDeCarga = cargaPedido.Pedido.TipoDeCarga?.Descricao ?? string.Empty,
                            GrupoPessoasDestinatario = cargaPedido.Pedido.Destinatario?.GrupoPessoas?.Descricao ?? string.Empty,
                            RestricaoEntrega = (from o in pedidosAdicional where o.Pedido.Codigo == cargaPedido.Pedido.Codigo select o.RestricaoEntrega)?.FirstOrDefault() ?? "",
                        },
                        DetalhesPedidoExportacao = new
                        {
                            AcondicionamentoCarga = cargaPedido.Pedido.AcondicionamentoCarga?.ObterDescricao() ?? "",
                            CargaPaletizada = cargaPedido.Pedido.CargaPaletizada ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                            ClienteAdicional = cargaPedido.Pedido.ClienteAdicional?.Descricao ?? "",
                            ClienteDonoContainer = cargaPedido.Pedido.ClienteDonoContainer?.Descricao ?? "",
                            DataEstufagem = cargaPedido.Pedido.DataEstufagem?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                            DataDeadLCargaNavioViagem = cargaPedido.Pedido.DataDeadLCargaNavioViagem?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                            DataDeadLineNavioViagem = cargaPedido.Pedido.DataDeadLineNavioViagem?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                            cargaPedido.Pedido.Despachante,
                            ETA = cargaPedido.Pedido.DataETA?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                            ETS = cargaPedido.Pedido.DataETS?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                            PossuiGenset = cargaPedido.Pedido.PossuiGenset ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                            cargaPedido.Pedido.InLand,
                            cargaPedido.Pedido.NavioViagem,
                            NumeroBooking = cargaPedido.Pedido.NumeroBooking ?? "",
                            NumeroEXP = cargaPedido.Pedido.NumeroEXP ?? "",
                            NumeroPedidoProvisorio = cargaPedido.Pedido.NumeroPedidoProvisorio ?? "",
                            NumeroReserva = cargaPedido.Pedido.Reserva ?? "",
                            PortoViagemDestino = cargaPedido.Pedido.PortoViagemDestino ?? "",
                            PortoViagemOrigem = cargaPedido.Pedido.PortoViagemOrigem ?? "",
                            RefEXPTransferencia = cargaPedido.Pedido.RefEXPTransferencia ?? "",
                            TipoContainer = cargaPedido.Pedido.ModeloVeicularCarga?.Descricao ?? "",
                            TipoProbe = cargaPedido.Pedido.TipoProbe?.ObterDescricao() ?? "",
                            ViaTransporte = cargaPedido.Pedido.ViaTransporte?.Descricao ?? ""
                        },
                        DetalhesPedidoTransporteMaritimo = servicoPedido.ObterDetalhesPedidoTransporteMaritimo(pedidoDadosTransporteMaritimo, pedidoDadosTransporteMaritimoRoteamentos)
                    };

                    listaPedidoRetornar.Add(pedidoRetornar);
                }

                bool possuiNumeroCarregamento = false;

                if (permitirAdicionarPedido)
                    possuiNumeroCarregamento = repositorioPedido.ExistePedidoComNumeroCarregamento();

                bool cargaColeta = repositorioCargaEntrega.BuscarPorCarga(carga.Codigo).Exists(cen => cen.Coleta == true);

                string dataCarregamentoCargaFormato = "dd/MM/yyyy";
                if (exibirHoraECodigosIntegracao && cargaColeta)
                    dataCarregamentoCargaFormato = "dd/MM/yyyy HH:mm";

                return new JsonpResult(new
                {
                    Carga = new
                    {
                        CodigoCarga = carga.Codigo,
                        DataCarregamentoCarga = carga.DataCarregamentoCarga != null ? carga.DataCarregamentoCarga.Value.ToString(dataCarregamentoCargaFormato) : "",
                        ExigirDefinicaoReboquePedido = exigirDefinicaoReboquePedido,
                        PermitirTrocarMultiplosPedidos = !possuiOrdemEmbarque,
                        Filial = new { Codigo = carga.Filial?.Codigo ?? 0, Descricao = carga.Filial?.Descricao ?? "" },
                        TipoCarga = new { Codigo = carga.TipoDeCarga?.Codigo ?? 0, Descricao = carga.TipoDeCarga?.Descricao ?? "" },
                        PermitirAdicionarPedido = permitirAdicionarPedido,
                        PermitirAdicionarPedidoMesmaFilial = permitirAdicionarPedidoMesmaFilial,
                        PermitirAdicionarPedidoOutraFilial = !carga.CargaAgrupada && permitirAdicionarPedidoOutraFilial,
                        PermitirAdicionarPedidoTroca = permitirAdicionarPedidoTroca,
                        NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega = carga.TipoOperacao?.NaoPermiteAvancarCargaSemDataPrevisaoDeEntrega ?? false,
                        PermitirAlterarVolumesNaCarga = carga.TipoOperacao?.PermitirAlterarVolumesNaCarga ?? false,
                        PermitirAdicionarPedidosNaEtapaUm = (carga.TipoOperacao?.PermitirAdicionarRemoverPedidosEtapa1 ?? false) && usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1,
                        PermitirAdicionarNovosPedidosPorNotasAvulsas = permitirAdicionarNovosPedidosPorNotasAvulsas,
                        PossuiNumeroCarregamento = possuiNumeroCarregamento,
                        NumeroProtocoloIntegracaoCarga = numeroProtocoloIntegracaoCarga
                    },
                    Pedidos = listaPedidoRetornar.ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga, true) ?? throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Pedido.OcorrenciaPedido servicoOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);
                carga.Initialize();

                if (servicoCarga.RecebeuNumeroCargaEmbarcador(carga, unitOfWork))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaJaRecebeuNumeroDecargaDoEmbarcadorNaoPermiteEssaAlteracao);

                DateTime? dataCarregamento = Request.GetNullableDateTimeParam("DataCarregamento");
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);
                bool encaixarHorario = (operadorLogistica?.PermiteSelecionarHorarioEncaixe ?? false) && Request.GetBoolParam("EncaixarHorario");

                if (!dataCarregamento.HasValue)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.DataDeCarregamentoDeveSerInformada);

                if ((dataCarregamento.Value < DateTime.Now.AddDays(-1)) && !encaixarHorario && !ConfiguracaoEmbarcador.PermitirAlterarCargaHorarioCarregamentoInferiorAtual)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelInformarUmaDataMenorQueDiaDeHoje);

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCarga(carga.Codigo);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();

                bool dataAlteradaPelaPrimeiraVez = carga.DataCarregamentoCarga == null;
                carga.DataCarregamentoCarga = dataCarregamento.Value;

                if (configuracaoDadosTransporte?.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte ?? false)
                    carga.CarregamentoIntegradoERP = false;

                repositorioCarga.Atualizar(carga, Auditado);

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    pedido.DataCarregamentoPedido = carga.DataCarregamentoCarga;
                    repositorioPedido.Atualizar(pedido);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

                if ((cargaJanelaCarregamento != null) && (cargaJanelaCarregamento.CentroCarregamento != null))
                {
                    carga.CargaGeradaPeloMetodoGerarCarregamento = true;

                    repositorioCarga.Atualizar(carga, Auditado);

                    cargaJanelaCarregamento.NaoComparecido = Request.GetBoolParam("NaoComparecimento") ? TipoNaoComparecimento.NaoCompareceu : TipoNaoComparecimento.Compareceu;
                    cargaJanelaCarregamento.HorarioEncaixado = encaixarHorario;
                    cargaJanelaCarregamento.TipoOperacaoEncaixe = encaixarHorario ? repositorioTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacaoEncaixe")) : null;

                    if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                        cargaJanelaCarregamento.QuantidadeAlteracoesManuaisHorarioCarregamento += 1;

                    cargaJanelaCarregamento.Initialize();

                    Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento(cargaJanelaCarregamento)
                    {
                        PermitirHorarioCarregamentoComLimiteAtingido = true
                    };
                    Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, configuracaoEmbarcador, Auditado, configuracaoDisponibilidadeCarregamento);
                    Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain servicoIntegracaoSaintGobain = new Servicos.Embarcador.Integracao.SaintGobain.IntegracaoSaintGobain(unitOfWork);

                    servicoDisponibilidadeCarregamento.ValidarPermissaoAlterarHorarioCarregamento(cargaJanelaCarregamento);
                    servicoDisponibilidadeCarregamento.AlterarHorarioCarregamento(cargaJanelaCarregamento, dataCarregamento.Value, TipoServicoMultisoftware);
                    servicoIntegracaoSaintGobain.ReenviarIntegrarCarregamento(cargaJanelaCarregamento);
                    Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, cargaJanelaCarregamento, cargaJanelaCarregamento.GetChanges(), Localization.Resources.Cargas.Carga.DataDeCarregamentoAlterada + (cargaJanelaCarregamento.Excedente ? "" : Localization.Resources.Cargas.Carga.EspacoParaEspaco + cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy")), unitOfWork);
                }

                AtualizarHorarioOcorrenciasProvisionadas(carga, unitOfWork);
                servicoCarga.AtualizarDataEstufagemDadosTransporteMaritimo(carga, unitOfWork, false);

                if (carga != null && configuracaoEmbarcador.DataBaseCalculoPrevisaoControleEntrega == DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga)
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(carga, configuracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), Localization.Resources.Cargas.Carga.AlterouDataDeCarregamentoPara + dataCarregamento.Value.ToDateTimeString(), unitOfWork);

                if (dataAlteradaPelaPrimeiraVez)
                    servicoOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoEmSeparacao, pedidos, ConfiguracaoEmbarcador, null);

                unitOfWork.CommitChanges();

                if (cargaJanelaCarregamento != null)
                    new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarDataDeCarregamentoDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarTipoPagamentoValePedagio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                Dominio.Enumeradores.TipoPagamentoValePedagio tipoPagamentoValePedagio = Request.GetEnumParam<Dominio.Enumeradores.TipoPagamentoValePedagio>("TipoPagamentoValePedagio");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga, true);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

                bool valePedagioComprado = repositorioCargaIntegracaoValePedagio.VerificarSeExisteValePedagioPorStatus(codigoCarga, SituacaoValePedagio.Comprada);

                if (valePedagioComprado)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.JaExisteValePedagioCompradoParaCarga);

                carga.TipoPagamentoValePedagio = tipoPagamentoValePedagio;

                repositorioCarga.Atualizar(carga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarTipoPagamentoValePedagioDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDatasCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga, true) ?? throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                carga.Initialize();

                if (servicoCarga.RecebeuNumeroCargaEmbarcador(carga, unitOfWork))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaJaRecebeuNumeroDecargaDoEmbarcadorNaoPermiteEssaAlteracao);

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo) ?? throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarJanelaDeCarregamento);

                DateTime inicioCarregamento = Request.GetNullableDateTimeParam("InicioCarregamento") ?? throw new ControllerException(Localization.Resources.Cargas.Carga.DataDoInicioDoCarregamentoDeveSerInformada);
                DateTime terminoCarregamento = Request.GetNullableDateTimeParam("TerminoCarregamento") ?? throw new ControllerException(Localization.Resources.Cargas.Carga.DataDoTerminoDoCarregamentoDeveSerInformada);

                if (inicioCarregamento < DateTime.Now.AddDays(-1) && !ConfiguracaoEmbarcador.PermitirAlterarCargaHorarioCarregamentoInferiorAtual)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelInformarUmaDataMenorQueDiaDeHoje);

                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeCarregamento()
                {
                    PermitirHorarioCarregamentoComLimiteAtingido = true
                };
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade servicoDisponibilidadeCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoDisponibilidade(unitOfWork, configuracaoEmbarcador, Auditado, configuracaoDisponibilidadeCarregamento);

                carga.DataCarregamentoCarga = inicioCarregamento;

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    carga.DataPrevisaoTerminoCarga = terminoCarregamento;

                repositorioCarga.Atualizar(carga);

                cargaJanelaCarregamento.Initialize();
                cargaJanelaCarregamento.HorarioEncaixado = false;

                servicoDisponibilidadeCarregamento.ValidarPermissaoAlterarHorarioCarregamento(cargaJanelaCarregamento);
                servicoDisponibilidadeCarregamento.AlterarHorarioCarregamento(cargaJanelaCarregamento, inicioCarregamento, TipoServicoMultisoftware);
                Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, cargaJanelaCarregamento, cargaJanelaCarregamento.GetChanges(), Localization.Resources.Cargas.Carga.DataDeCarregamentoAlterada + (cargaJanelaCarregamento.Excedente ? "" : Localization.Resources.Cargas.Carga.EspacoParaEspaco + cargaJanelaCarregamento.InicioCarregamento.ToString("dd/MM/yyyy")), unitOfWork);

                if (configuracaoEmbarcador.DataBaseCalculoPrevisaoControleEntrega == DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga)
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(carga, configuracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), Localization.Resources.Cargas.Carga.AlterouDataDeCarregamentoPara + inicioCarregamento.ToString("dd/MM/yyyy"), unitOfWork);

                servicoCarga.AtualizarDataEstufagemDadosTransporteMaritimo(carga, unitOfWork, false);

                unitOfWork.CommitChanges();

                new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarDataDeCarregamentoDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataRetornoCD()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga, true);

                if (carga == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (!(carga.TipoOperacao?.ConfiguracaoCarga?.PermitirAlterarDataRetornoCDCarga ?? false))
                    throw new ControllerException(Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                if (carga.SituacaoCarga.IsSituacaoCargaFaturada())
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelAlterarNaAtualSituacao);

                DateTime dataRetornoCD = Request.GetDateTimeParam("DataRetornoCD");
                DateTime? dataUltimaEntrega = repositorioCargaEntrega.BuscarPrevisaoUltimaEntregaPorCarga(codigoCarga);

                if (dataUltimaEntrega.HasValue && dataRetornoCD < dataUltimaEntrega.Value)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.DataNaoPodeSerMenorUltimaDataEntrega);

                carga.DataRetornoCD = dataRetornoCD;

                repositorioCarga.Atualizar(carga);

                Servicos.Embarcador.Integracao.Diaria.DiariaMotorista servicoDiariaMotorista = new Servicos.Embarcador.Integracao.Diaria.DiariaMotorista(unitOfWork);
                servicoDiariaMotorista.RecalcularPagamentoMotoristaEmbarcador(dataRetornoCD, carga, carga.Pedidos.ToList(), Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), Localization.Resources.Cargas.Carga.AlterouDataRetornoCD, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarDataRetornoCD);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarPercentualExecucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AlterarPercentualExecucao) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
            try
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                int codCarga = int.Parse(Request.Params("Carga"));
                List<dynamic> motoristas = Request.GetListParam<dynamic>("Motoristas");
                if (motoristas != null && motoristas.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> MotoristasCraga = repCargaMotorista.BuscarPorCarga(carga.Codigo);

                    foreach (var motorista in motoristas)
                    {
                        int Codigo;
                        int.TryParse((string)motorista.Codigo, out Codigo);
                        decimal PercentualExecucao;
                        decimal.TryParse((string)motorista.PercentualExecucao, out PercentualExecucao);

                        Dominio.Entidades.Usuario motoristaDB = repUsuario.BuscarPorCodigo(Codigo);

                        Dominio.Entidades.Embarcador.Cargas.CargaMotorista MotoristaCraga = MotoristasCraga.Where(q => q.Motorista.Codigo == motoristaDB.Codigo).FirstOrDefault();

                        if (MotoristaCraga != null)
                        {
                            MotoristaCraga.PercentualExecucao = PercentualExecucao;
                            repCargaMotorista.Atualizar(MotoristaCraga);
                        }
                        else
                        {
                            throw new CustomException("O motorista " + motoristaDB.Nome + " não está presente na carga, por favor salve os dados da carga antes de realizar essa operação");
                        }
                    }
                }
                else
                {
                    throw new CustomException("Nenhum motorista informado.");
                }

                return new JsonpResult(true);
            }
            catch (CustomException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o % de execução.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarFaixaTemperatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codCarga = int.Parse(Request.Params("Carga"));
                int codFaixaTemperatura = int.Parse(Request.Params("FaixaTemperatura"));

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = repFaixaTemperatura.BuscarPorCodigo(codFaixaTemperatura);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.CargaNaoEncontrada);

                if (faixaTemperatura == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.FaixaDeTemperaturaNaoEncontrada);

                unitOfWork.Start();

                carga.FaixaTemperatura = faixaTemperatura;
                repCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.AlterourFaixaDaTemperaturaPara, faixaTemperatura.Descricao), unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarFaixaDeTemperatura);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarRaioXCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(carga.Codigo);

                if (carga == null)
                    throw new Exception(Localization.Resources.Cargas.Carga.CargaNaoEncontrada);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                var listaCargaEntrega = repCargaEntrega.BuscarPorCarga(carga.Codigo);
                int numeroColetas = (from o in listaCargaEntrega where o.Coleta select o).ToList().Count;
                int numeroEntregas = (from o in listaCargaEntrega where !o.Coleta select o).ToList().Count;
                string paradas = $"({numeroColetas} {((numeroColetas == 1) ? Localization.Resources.Cargas.Carga.Coleta : Localization.Resources.Cargas.Carga.Coletas)}, {numeroEntregas} {(numeroEntregas == 1 ? Localization.Resources.Cargas.Carga.EntregaSingular : Localization.Resources.Cargas.Carga.Entregas)})";

                var listaParadasPendentes = (from o in listaCargaEntrega where o.EstaPendente select o).ToList();
                int numeroParadasPendentes = listaParadasPendentes.Count;
                decimal pesoTotalCarga = (from o in listaCargaEntrega select (from a in o.Pedidos select a.CargaPedido.Peso).Sum()).Sum();
                decimal pesoPendenteCarga = (from o in listaParadasPendentes select (from a in o.Pedidos select a.CargaPedido.Peso).Sum()).Sum();

                string reboques = string.Join(", ", carga.VeiculosVinculados.Select(o => o.Placa));
                var dynCarga = new
                {
                    Motoristas = (
                        from cargaMotorista in cargaMotoristas
                        select new
                        {
                            Nome = cargaMotorista == null ? "" : cargaMotorista?.Motorista?.Nome,
                            CPF_Formatado = cargaMotorista == null ? "" : cargaMotorista?.Motorista?.CPF_Formatado,
                            Telefone = string.IsNullOrWhiteSpace(cargaMotorista?.Motorista?.Telefone) ? null : cargaMotorista.Motorista.Telefone
                        }
                    ).ToList(),
                    Cavalo = carga.Veiculo == null ? "" : carga.Veiculo?.Placa,
                    ModeloVeiculo = carga.ModeloVeicularCarga?.Descricao ?? Localization.Resources.Cargas.Carga.SemModelo,
                    Reboque = reboques,
                    CargaInicioViagem = carga.DataInicioViagem == null ? "" : carga.DataInicioViagem?.ToString("HH:mm dd/MM/yyyy"),
                    CargaFimViagem = carga.DataFimViagem == null ? "" : carga.DataFimViagem?.ToString("HH:mm dd/MM/yyyy"),
                    CargaParadas = paradas,
                    CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador,
                    NomeTransportador = carga.Empresa?.NomeFantasia,
                    PesoTotal = pesoTotalCarga,
                    PesoPendente = pesoPendenteCarga,
                    NumeroParadasPendentes = numeroParadasPendentes,
                };

                return new JsonpResult(dynCarga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);

                decimal peso = 0;
                if (carga.CargaAgrupamento == null)
                    peso = repCargaPedido.BuscarPesoTotalPorCarga(carga.Codigo);
                else
                    peso = repCargaPedido.BuscarPesoTotalPorCargaOrigem(carga.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(carga.Codigo);

                Dominio.Entidades.Veiculo veiculoAgrupamento = null;

                if (!string.IsNullOrWhiteSpace(carga.PlacaDeAgrupamento))
                    veiculoAgrupamento = repVeiculo.BuscarPorPlaca(carga.Empresa.Codigo, carga.PlacaDeAgrupamento);

                var dynCarga = new
                {
                    Descricao = serCargaDadosSumarizados.ObterOrigemDestinos(carga, true, TipoServicoMultisoftware),
                    carga.Codigo,
                    carga.CodigoCargaEmbarcador,
                    Empresa = carga.Empresa != null ? new { Codigo = carga.Empresa.Codigo, Descricao = carga.Empresa.RazaoSocial, RaizCNPJEmpresa = carga.Empresa.CNPJ_SemFormato.Remove(8, 6) } : new { Codigo = 0, Descricao = "", RaizCNPJEmpresa = "" },
                    Filial = carga.Filial != null ? new { Codigo = carga.Filial.Codigo, Descricao = carga.Filial.Descricao } : new { Codigo = 0, Descricao = "" },
                    ValorFrete = carga.ValorFrete.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                    Motorista = cargaMotoristas.Count > 0 ? new { Codigo = cargaMotoristas.FirstOrDefault().Motorista.Codigo, Descricao = cargaMotoristas.FirstOrDefault().Motorista.Nome } : new { Codigo = 0, Descricao = "" },
                    Veiculo = carga.Veiculo != null ? new { Codigo = carga.Veiculo.Codigo, Descricao = carga.Veiculo.Placa } : new { Codigo = 0, Descricao = "" },
                    Placa = carga.RetornarPlacas,
                    TipoCarga = carga.TipoDeCarga != null ? new { Codigo = carga.TipoDeCarga.Codigo, Descricao = carga.TipoDeCarga.Descricao } : new { Codigo = 0, Descricao = "" },
                    ModeloVeicularCarga = carga.ModeloVeicularCarga != null ? new { Codigo = carga.ModeloVeicularCarga.Codigo, Descricao = carga.ModeloVeicularCarga.Descricao } : new { Codigo = 0, Descricao = "" },
                    Peso = peso.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                    SituacaoCarga = carga.SituacaoCarga,
                    DescricaoSituacaoCarga = carga.SituacaoCarga.ObterDescricao(),
                    carga.SituacaoAlteracaoFreteCarga,
                    carga.PlacaDeAgrupamento,
                    carga.TipoCarregamento,
                    TipoOperacao = carga.TipoOperacao != null ? carga.TipoOperacao.Descricao : "",
                    DataCarregamento = carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm"),
                    Operador = carga.Operador?.Descricao ?? "",
                    CentroResultado = carga.TipoOperacao != null ? carga.TipoOperacao?.ConfiguracaoPagamentos?.CentroResultado?.Descricao : ""
                };

                return new JsonpResult(dynCarga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasLiberadasGuarita()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                string codigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador");

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TipoDeCarga, "TipoDeCarga", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloVeicular, "ModeloVeicular", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 19, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 5, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int totalRegistros = repositorioCarga.ContarConsultaCargasLiberadasGuarita(codigoCargaEmbarcador);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = (totalRegistros > 0) ? repositorioCarga.ConsultarCargasLiberadasGuarita(codigoCargaEmbarcador, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var listaCargaRetornar = (
                    from carga in listaCarga
                    select new
                    {
                        carga.Codigo,
                        carga.CodigoCargaEmbarcador,
                        Filial = carga.Filial != null ? carga.Filial.Descricao : "",
                        OrigemDestino = servicoCargaDadosSumarizados.ObterOrigemDestinos(carga, true, TipoServicoMultisoftware),
                        TipoDeCarga = carga.TipoDeCarga != null ? carga.TipoDeCarga.Descricao : string.Empty,
                        ModeloVeicular = carga.ModeloVeicularCarga != null ? carga.ModeloVeicularCarga.Descricao : string.Empty,
                        Transportador = carga.Empresa != null ? carga.Empresa.RazaoSocial : string.Empty,
                        Veiculo = carga.Veiculo != null ? carga.Veiculo.Placa : ""
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasPorSituacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TipoDeCarga, "TipoDeCarga", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloVeicular, "ModeloVeicular", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 19, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 5, Models.Grid.Align.left, false);

                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = ObterFiltrosPesquisaCargaPorSituacao(unitOfWork, configuracaoGeral);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                int totalRegistros = repositorioCarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = (totalRegistros > 0) ? repositorioCarga.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var listaCargaRetornar = (
                    from carga in listaCarga
                    select new
                    {
                        carga.Codigo,
                        carga.CodigoCargaEmbarcador,
                        Filial = carga.Filial != null ? carga.Filial.Descricao : "",
                        OrigemDestino = servicoCargaDadosSumarizados.ObterOrigemDestinos(carga, true, TipoServicoMultisoftware),
                        TipoDeCarga = carga.TipoDeCarga != null ? carga.TipoDeCarga.Descricao : string.Empty,
                        ModeloVeicular = carga.ModeloVeicularCarga != null ? carga.ModeloVeicularCarga.Descricao : string.Empty,
                        Transportador = carga.Empresa != null ? carga.Empresa.RazaoSocial : string.Empty,
                        Veiculo = carga.Veiculo != null ? carga.Veiculo.Placa : ""
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasPedidoParaEncaixeDeSubcontratacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                int empresa;
                int.TryParse(Request.Params("Empresa"), out empresa);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                string CodigoCargaEmbarcador = Request.Params("CodigoCargaEmbarcador");
                string CodigoPedidoEmbarcador = Request.Params("CodigoPedidoEmbarcador");
                int operador;
                int.TryParse(Request.Params("Operador"), out operador);
                int veiculo;
                int.TryParse(Request.Params("Veiculo"), out veiculo);

                int numeroNF;
                int.TryParse(Request.Params("NumeroNF"), out numeroNF);

                int numeroCTe;
                int.TryParse(Request.Params("NumeroCTe"), out numeroCTe);

                int modeloVeicularCarga;
                int.TryParse(Request.Params("ModeloVeicularCarga"), out modeloVeicularCarga);
                int tipoCarga;
                int.TryParse(Request.Params("TipoCarga"), out tipoCarga);
                double destinatario;
                double.TryParse(Request.Params("Destinatario"), out destinatario);
                double remetente;
                double.TryParse(Request.Params("Remetente"), out remetente);
                int filial, origem, destino;
                int.TryParse(Request.Params("Filial"), out filial);
                int.TryParse(Request.Params("Origem"), out origem);
                int.TryParse(Request.Params("Destino"), out destino);

                string estadoDestino = Request.Params("EstadoDestino");

                int tipoOperacao = 0;
                int.TryParse(Request.Params("TipoOperacao"), out tipoOperacao);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDoPedido, "CodigoPedidoEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Destino, "Destino", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Destinatario, "Destinatario", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NotasFiscais, "NotasFiscais", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Reentrega, "Reentrega", 10, Models.Grid.Align.center, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                List<SituacaoCarga> situacoesLiberadas = new List<SituacaoCarga>() { SituacaoCarga.EmTransporte, SituacaoCarga.Encerrada, SituacaoCarga.AgImpressaoDocumentos };
                List<int> codigosFiliais = new List<int>(); //ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork);


                int totalRegistros = repositorioCargaPedido.ContarConsulta(situacoesLiberadas, 0, false, numeroNF, numeroCTe, CodigoPedidoEmbarcador, CodigoCargaEmbarcador, origem, destino, filial, remetente, destinatario, 0, 0, true, false, estadoDestino, false, codigosFiliais);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = (totalRegistros > 0) ? repositorioCargaPedido.Consultar(situacoesLiberadas, 0, false, numeroNF, numeroCTe, CodigoPedidoEmbarcador, CodigoCargaEmbarcador, origem, destino, filial, remetente, destinatario, 0, 0, true, false, estadoDestino, false, codigosFiliais, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> xmlNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedidos((from obj in listaCargaPedido select obj.Codigo).ToList());

                var listaCargaPedidoRetornar = (
                    from cargaPedido in listaCargaPedido
                    select ObterCargaPedidoParaEncaixeDeSubcontratacao(cargaPedido, xmlNotasFiscais)
                ).ToList();

                grid.AdicionaRows(listaCargaPedidoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasParaEncaixeDeSubcontratacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TipoDeCarga, "TipoDeCarga", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloVeicular, "ModeloVeicular", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 19, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 5, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = ObterFiltrosPesquisaCargaParaEncaixeDeSubcontratacao(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                int totalRegistros = repositorioCarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = (totalRegistros > 0) ? repositorioCarga.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var listaCargaRetornar = (
                    from obj in listaCarga
                    select new
                    {
                        obj.Codigo,
                        obj.CodigoCargaEmbarcador,
                        Filial = obj.Filial != null ? obj.Filial.Descricao : "",
                        OrigemDestino = servicoCargaDadosSumarizados.ObterOrigemDestinos(obj, true, TipoServicoMultisoftware),
                        TipoDeCarga = obj.TipoDeCarga != null ? obj.TipoDeCarga.Descricao : string.Empty,
                        ModeloVeicular = obj.ModeloVeicularCarga != null ? obj.ModeloVeicularCarga.Descricao : string.Empty,
                        Transportador = obj.Empresa != null ? obj.Empresa.RazaoSocial : string.Empty,
                        Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : ""
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargaFinalizadasParaAcertoDeViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unitOfWork);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configuracaoAcertoViagem = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarPrimeiroRegistro();
                bool contemConfiguracao = configuracaoAcertoViagem != null && configuracaoAcertoViagem.GerarMovimentoAutomaticoNaGeracaoAcertoViagem ? true : false;
                bool visualizarPalletsCanhotosNasCargas = configuraoAcertoViagem?.VisualizarPalletsCanhotosNasCargas ?? false;

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                string codigoCargaEmbarcador = Request.Params("CodigoCargaEmbarcador");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga;
                Enum.TryParse(Request.Params("Situacao"), out situacaoCarga);
                int codigoAcertoViagem, codigoVeiculo;
                int.TryParse(Request.Params("AcertoViagem"), out codigoAcertoViagem);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                DateTime dataCarga;
                DateTime.TryParse(Request.Params("DataCarga"), out dataCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoAcertoCarga", false);
                grid.AdicionarCabecalho("LancadoManualmente", false);
                grid.AdicionarCabecalho("DataCriacaoCarga", false);
                grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DescricaoCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Numero", false);
                grid.AdicionarCabecalho("Placa", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Emitente, "Emitente", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", false);
                grid.AdicionarCabecalho("ValorBrutoCarga", false);
                grid.AdicionarCabecalho("ValorICMSCarga", false);
                grid.AdicionarCabecalho("CargaFracionada", false);
                grid.AdicionarCabecalho("ValorBonificacaoCliente", false);
                grid.AdicionarCabecalho("PedagioAcertoCredito", false);
                grid.AdicionarCabecalho("PedagioAcerto", false);
                grid.AdicionarCabecalho("ValorFrete", false);
                grid.AdicionarCabecalho("PercentualAcerto", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Motoristas, "Motoristas", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("ContemMaisDeUmMotorista", false);
                grid.AdicionarCabecalho("ContemMDFeEncerrado", false);
                grid.AdicionarCabecalho("CargaLancadaEmOutroAcerto", false);
                grid.AdicionarCabecalho("DT_RowColor", false);
                grid.AdicionarCabecalho("SituacaoCanhotos", false);
                grid.AdicionarCabecalho("SituacaoPallets", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Data")
                    propOrdenacao = "DataCriacaoCarga";

                var dynListaCarga = repCarga.ConsultarParaAcertoViagem(codigoCargaEmbarcador, situacaoCarga, codigoAcertoViagem, true, dataCarga, codigoVeiculo, contemConfiguracao, visualizarPalletsCanhotosNasCargas, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repCarga.ContarConsultaParaAcertoViagem(codigoCargaEmbarcador, situacaoCarga, codigoAcertoViagem, true, dataCarga, codigoVeiculo));

                grid.AdicionaRows(dynListaCarga);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargaComissaoFuncionario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                string codigoCargaEmbarcador = Request.Params("CodigoCargaEmbarcador");
                int codigoComissaoFuncionario, codigoVeiculo;
                int.TryParse(Request.Params("ComissaoFuncionario"), out codigoComissaoFuncionario);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
                DateTime dataCarga;
                DateTime.TryParse(Request.Params("DataCarga"), out dataCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TipoDeCarga, "TipoDeCarga", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloVeicular, "ModeloVeicular", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 19, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 5, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Data")
                    propOrdenacao = "DataCriacaoCarga";

                var listaCarga = repCarga.ConsultarSemComissaoMotorista(codigoCargaEmbarcador, codigoMotorista, codigoComissaoFuncionario, dataCarga, codigoVeiculo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                var listaCargaRetornar = (
                    from carga in listaCarga
                    select new
                    {
                        carga.Codigo,
                        carga.CodigoCargaEmbarcador,
                        Filial = carga.Filial != null ? carga.Filial.Descricao : "",
                        OrigemDestino = servicoCargaDadosSumarizados.ObterOrigemDestinos(carga, true, TipoServicoMultisoftware),
                        TipoDeCarga = carga.TipoDeCarga != null ? carga.TipoDeCarga.Descricao : string.Empty,
                        ModeloVeicular = carga.ModeloVeicularCarga != null ? carga.ModeloVeicularCarga.Descricao : string.Empty,
                        Transportador = carga.Empresa != null ? carga.Empresa.RazaoSocial : string.Empty,
                        Veiculo = carga.Veiculo != null ? carga.Veiculo.Placa : ""
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaRetornar);
                grid.setarQuantidadeTotal(repCarga.ContarConsultaSemComissaoMotorista(codigoCargaEmbarcador, codigoMotorista, codigoComissaoFuncionario, dataCarga, codigoVeiculo));

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargaFinalizadasSemAcertoDeViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unitOfWork);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configuracaoAcertoViagem = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarPrimeiroRegistro();
                bool contemConfiguracao = configuracaoAcertoViagem != null && configuracaoAcertoViagem.GerarMovimentoAutomaticoNaGeracaoAcertoViagem ? true : false;
                bool visualizarPalletsCanhotosNasCargas = configuraoAcertoViagem?.VisualizarPalletsCanhotosNasCargas ?? false;

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                string codigoCargaEmbarcador = Request.Params("CodigoCargaEmbarcador");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga;
                Enum.TryParse(Request.Params("Situacao"), out situacaoCarga);
                int codigoAcertoViagem, codigoVeiculo;
                int.TryParse(Request.Params("AcertoViagem"), out codigoAcertoViagem);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                DateTime dataCarga;
                DateTime.TryParse(Request.Params("DataCarga"), out dataCarga);

                string situacaoesCarga = ConfiguracaoEmbarcador?.SituacaoCargaAcertoViagem ?? "";
                List<int> listaSituacaoCarga = new List<int>();
                if (!string.IsNullOrEmpty(situacaoesCarga))
                {
                    var lista = situacaoesCarga.Split(';');
                    foreach (var situacao in lista)
                        listaSituacaoCarga.Add(int.Parse(Utilidades.String.OnlyNumbers(situacao)));
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoAcertoCarga", false);
                grid.AdicionarCabecalho("LancadoManualmente", false);
                grid.AdicionarCabecalho("DataCriacaoCarga", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Data, "Data", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DescricaoCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Numero", false);
                grid.AdicionarCabecalho("Placa", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Emitente, "Emitente", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", false);
                grid.AdicionarCabecalho("ValorBrutoCarga", false);
                grid.AdicionarCabecalho("ValorICMSCarga", false);
                grid.AdicionarCabecalho("CargaFracionada", false);
                grid.AdicionarCabecalho("ValorBonificacaoCliente", false);
                grid.AdicionarCabecalho("PedagioAcertoCredito", false);
                grid.AdicionarCabecalho("PedagioAcerto", false);
                grid.AdicionarCabecalho("ValorFrete", false);
                grid.AdicionarCabecalho("PercentualAcerto", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Motoristas, "Motoristas", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("ContemMaisDeUmMotorista", false);
                grid.AdicionarCabecalho("ContemMDFeEncerrado", false);
                grid.AdicionarCabecalho("CargaLancadaEmOutroAcerto", false);
                grid.AdicionarCabecalho("DT_RowColor", false);
                grid.AdicionarCabecalho("SituacaoCanhotos", false);
                grid.AdicionarCabecalho("SituacaoPallets", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Data")
                    propOrdenacao = "DataCriacaoCarga";

                var dynListaCarga = repCarga.ConsultarSemAcertoViagem(codigoCargaEmbarcador, situacaoCarga, codigoAcertoViagem, true, dataCarga, codigoVeiculo, contemConfiguracao, visualizarPalletsCanhotosNasCargas, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite, listaSituacaoCarga);

                grid.setarQuantidadeTotal(repCarga.ContarConsultaSemAcertoViagem(codigoCargaEmbarcador, situacaoCarga, codigoAcertoViagem, true, dataCarga, codigoVeiculo, listaSituacaoCarga));

                grid.AdicionaRows(dynListaCarga);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasParaTrocaNota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TipoDeCarga, "TipoDeCarga", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloVeicular, "ModeloVeicular", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 19, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 5, Models.Grid.Align.left, false);

                string codigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                int quantidade = repositorioCarga.ContarConsultaOperacaoTrocaNota(codigoCargaEmbarcador);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = quantidade > 0 ? repositorioCarga.ConsultarOperacaoTrocaNota(codigoCargaEmbarcador, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var listaCargaRetornar = (
                    from carga in listaCarga
                    select new
                    {
                        carga.Codigo,
                        carga.CodigoCargaEmbarcador,
                        OrigemDestino = servicoCargaDadosSumarizados.ObterOrigemDestinos(carga, true, TipoServicoMultisoftware),
                        TipoDeCarga = carga.TipoDeCarga != null ? carga.TipoDeCarga.Descricao : string.Empty,
                        ModeloVeicular = carga.ModeloVeicularCarga != null ? carga.ModeloVeicularCarga.Descricao : string.Empty,
                        Transportador = carga.Empresa != null ? carga.Empresa.RazaoSocial : string.Empty,
                        Veiculo = carga.Veiculo != null ? carga.Veiculo.Placa : ""
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaRetornar);
                grid.setarQuantidadeTotal(quantidade);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasParaFilaCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TipoDeCarga, "TipoDeCarga", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloVeicular, "ModeloVeicular", 14, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 19, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 5, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFilaCarregamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaFilaCarregamento()
                {
                    CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                    DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                    DataLimite = Request.GetNullableDateTimeParam("DataFinal"),
                    TipoServicoMultisoftware = TipoServicoMultisoftware
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                int totalRegistros = repositorioCarga.ContarConsultaParaFilaCarregamento(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = totalRegistros > 0 ? repositorioCarga.ConsultarParaFilaCarregamento(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var listaCargaRetornar = (
                    from carga in listaCarga
                    select new
                    {
                        carga.Codigo,
                        carga.CodigoCargaEmbarcador,
                        OrigemDestino = servicoCargaDadosSumarizados.ObterOrigemDestinos(carga, true, TipoServicoMultisoftware),
                        TipoDeCarga = carga.TipoDeCarga?.Descricao ?? "",
                        ModeloVeicular = carga.ModeloVeicularCarga?.Descricao ?? "",
                        Transportador = carga.Empresa?.RazaoSocial ?? "",
                        Veiculo = carga.Veiculo?.Placa ?? ""
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDetalhesDaCargaAcertoViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                int codigoAcerto = 0;
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Acerto.AcertoCarga acertoCarga = repAcertoCarga.BuscarPorCodigoAcertoCodigoCarga(codigoAcerto, codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                List<dynamic> lista = new List<dynamic>();
                var dynPedido = new
                {
                    Codigo = carga.Codigo,
                    Carga = carga.CodigoCargaEmbarcador,
                    TipoVeiculo = carga.Veiculo != null ? carga.Veiculo.TipoVeiculo != null ? carga.Veiculo.DescricaoTipoVeiculo : string.Empty : string.Empty,
                    TipoCarga = carga.TipoDeCarga != null ? carga.TipoDeCarga.Descricao : string.Empty,
                    NumeroEntrega = cargaPedidos.Count().ToString() + " " + Localization.Resources.Cargas.Carga.Entrega,
                    Motorista = string.Join(",", (from obj in repCargaMotorista.BuscarPorCarga(carga.Codigo) select obj.Motorista.Nome)),
                    Placa = carga.RetornarPlacas,
                    ValorFrete = carga.ValorFrete.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                    //ICMS = (from p in repCargaCTe.BuscarPorCarga(carga.Codigo) select p.CTe.ValorICMS).Sum().ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                    CargaFracionada = acertoCarga != null ? acertoCarga.CargaFracionada : false,
                    ValorBrutoCarga = acertoCarga != null ? acertoCarga.ValorBrutoCarga.ToString("n2") : repCarga.BuscarValorFreteAReceberConhecimentos(carga.Codigo).ToString("n2"),
                    ValorICMSCarga = acertoCarga != null ? acertoCarga.ValorICMSCarga.ToString("n2") : repCarga.BuscarValorICMSConhecimentos(carga.Codigo).ToString("n2"),
                    ValorBonificacaoCliente = acertoCarga != null ? acertoCarga.ValorBonificacaoCliente.ToString("n2") : 0.ToString("n2"),
                    ComponentesFrete = (from componente in repCargaComponentesFrete.BuscarPorCargaComImpostosSemComponenteFreteLiquido(carga.Codigo, false)
                                        select new
                                        {
                                            Codigo = componente?.Codigo ?? 0,
                                            ValorComponente = componente?.ValorComponente ?? 0m,
                                            DescricaoComponente = componente?.ComponenteFrete?.DescricaoComponente ?? "",
                                            TipoComponenteFrete = componente?.ComponenteFrete?.TipoComponenteFrete ?? TipoComponenteFrete.OUTROS
                                        }).ToList(),
                    Total = (from p in repCargaCTe.BuscarPorCarga(carga.Codigo) where p.CargaCTeComplementoInfo == null && p.CTe.Status == "A" select p.CTe.ValorAReceber).Sum().ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                    Pedido = (from cargaPedido in cargaPedidos
                              select new
                              {
                                  Codigo = cargaPedido.Pedido.Codigo,
                                  NumeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                                  Remetente = cargaPedido.Pedido.Remetente.Nome + "(" + cargaPedido.Pedido.Remetente.Localidade.DescricaoCidadeEstado + ")",
                                  Destinatario = cargaPedido.Pedido.Destinatario.Nome + "(" + cargaPedido.Pedido.Destinatario.Localidade.DescricaoCidadeEstado + ")",
                                  ProdutoPredominante = cargaPedido.Pedido.ProdutoPredominante != null ? cargaPedido.Pedido.ProdutoPredominante : string.Empty,
                                  Peso = cargaPedido.Pedido.PesoTotal.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                                  Emissora = cargaPedido.Carga.Empresa != null ? cargaPedido.Carga.Empresa.RazaoSocial + "(" + cargaPedido.Carga.Empresa.Localidade.DescricaoCidadeEstado + ")" : string.Empty,
                                  Conhecimento = ""
                              }).ToList()
                };
                lista.Add(dynPedido);
                return new JsonpResult(lista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasFinalizadasAmbiente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                // Grid View
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho("DataEnvioUltimaNFe", false); // Criterio de ordenação, não mudar de posição
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 19, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Transportador")
                    propOrdenacao = "Empresa.RazaoSocial";
                else if (propOrdenacao == "Filial")
                    propOrdenacao = "Filial.Descricao";
                else if (propOrdenacao == "Placa")
                    propOrdenacao = "Veiculo.Placa";

                // Converte parametros
                string numeroCarga = Request.Params("Codigo");

                // Serviço MultiCTe lista apenas avarias do Usuário
                int.TryParse(Request.Params("Transportador"), out int transportador);
                //if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                //    transportador = this.Empresa.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacao = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                };

                long? codigoTransportadorTerceiro = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    transportador = this.Empresa.Codigo;
                else if (TipoServicoMultisoftware == TipoServicoMultisoftware.TransportadorTerceiro)
                    codigoTransportadorTerceiro = this.Usuario.ClienteTerceiro.Codigo;

                // Busca
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = repCarga.ConsultarCargasFinalizadasAmbiente(false, transportador, numeroCarga, situacao, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int quantidade = repCarga.ContarConsultaCargasFinalizadasAmbiente(false, transportador, codigoTransportadorTerceiro, numeroCarga, situacao);

                // Formata retorno
                var dynListaCarga = (from obj in listaCarga
                                     select new
                                     {
                                         obj.Codigo,
                                         obj.DataEnvioUltimaNFe,
                                         obj.CodigoCargaEmbarcador,
                                         Filial = obj.Filial != null ? obj.Filial.Descricao : "",
                                         OrigemDestino = serCargaDadosSumarizados.ObterOrigemDestinos(obj, true, TipoServicoMultisoftware),
                                         Transportador = obj.Empresa != null ? obj.Empresa.RazaoSocial : string.Empty,
                                         Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : ""
                                     }).ToList();
                // Retorna dados
                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(dynListaCarga);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarCargasParaChamados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                // Grid View
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho("DataEnvioUltimaNFe", false); // Criterio de ordenação, não mudar de posição
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.OrigemDestino, "OrigemDestino", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 19, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Transportador")
                    propOrdenacao = "Empresa.RazaoSocial";
                else if (propOrdenacao == "Filial")
                    propOrdenacao = "Filial.Descricao";
                else if (propOrdenacao == "Placa")
                    propOrdenacao = "Veiculo.Placa";

                // Converte parametros
                string numeroCarga = Request.Params("Codigo");

                // Serviço MultiCTe lista apenas avarias do Usuário
                int.TryParse(Request.Params("Transportador"), out int transportador);
                //if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                //    transportador = this.Empresa.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacao = null;
                if (configuracao != null && configuracao.FiltrarCargasSemDocumentosParaChamados)
                {
                    situacao = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                    };
                }
                else
                {
                    situacao = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                    };
                }

                long? codigoTransportadorTerceiro = null;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    transportador = this.Empresa.Codigo;
                else if (TipoServicoMultisoftware == TipoServicoMultisoftware.TransportadorTerceiro)
                    codigoTransportadorTerceiro = this.Usuario.ClienteTerceiro.Codigo;

                bool cargasNaoAgrupadas = !ConfiguracaoEmbarcador.GerarOcorrenciaParaCargaAgrupada;

                // Busca
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = repCarga.ConsultarCargasFinalizadasAmbiente(cargasNaoAgrupadas, transportador, numeroCarga, situacao, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int quantidade = repCarga.ContarConsultaCargasFinalizadasAmbiente(cargasNaoAgrupadas, transportador, codigoTransportadorTerceiro, numeroCarga, situacao);

                // Formata retorno
                var dynListaCarga = (from obj in listaCarga
                                     select new
                                     {
                                         obj.Codigo,
                                         obj.DataEnvioUltimaNFe,
                                         obj.CodigoCargaEmbarcador,
                                         Filial = obj.Filial != null ? obj.Filial.Descricao : "",
                                         OrigemDestino = serCargaDadosSumarizados.ObterOrigemDestinos(obj, true, TipoServicoMultisoftware),
                                         Transportador = obj.Empresa != null ? obj.Empresa.RazaoSocial : string.Empty,
                                         Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : ""
                                     }).ToList();
                // Retorna dados
                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(dynListaCarga);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasAvarias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                var repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                var repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                var repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                // Grid View
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Motorista", false);
                grid.AdicionarCabecalho("RGMotorista", false);
                grid.AdicionarCabecalho("CPFMotorista", false);
                grid.AdicionarCabecalho("CodigoTipoOperacao", false);
                grid.AdicionarCabecalho("DescricaoTipoOperacao", false);
                grid.AdicionarCabecalho("Empresa", false);
                grid.AdicionarCabecalho("NomeEmpresa", false);
                grid.AdicionarCabecalho("CentroResultadoCodigo", false);
                grid.AdicionarCabecalho("CentroResultadoDescricao", false);

                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Filial, "Filial", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Placa", 15, Models.Grid.Align.left, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Transportador")
                    propOrdenacao = "Empresa.RazaoSocial";
                else if (propOrdenacao == "Filial")
                    propOrdenacao = "Filial.Descricao";
                else if (propOrdenacao == "Placa")
                    propOrdenacao = "Veiculo.Placa";

                // Converte parametros
                string numeroCarga = Request.Params("CodigoCargaEmbarcador");

                // Serviço MultiCTe lista apenas avarias do Usuário
                double recebedor = 0;
                double destinatario = 0;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    double.TryParse(this.Empresa.CNPJ, out recebedor);


                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacao = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                };

                // Busca
                var listaCarga = repCarga.ConsultarAvarias(recebedor, destinatario, numeroCarga, situacao, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                var quantidade = repCarga.ContarConsultaAvarias(recebedor, destinatario, numeroCarga, situacao);

                // Caso a carga esteja agrupada é utilizado o código do agrupamento
                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotorista = repCargaMotorista.BuscarPorCargas(listaCarga.Select(obj => obj.CargaAgrupamento != null ? obj.CargaAgrupamento.Codigo : obj.Codigo).ToList());

                // Formata retorno
                var dynListaCarga = (from obj in listaCarga
                                     let codigoCarga = obj.CargaAgrupamento != null ? obj.CargaAgrupamento.Codigo : obj.Codigo
                                     let mot = (from mot in cargaMotorista where mot.Carga.Codigo == codigoCarga select mot.Motorista).FirstOrDefault()
                                     let centroResultados = repPedidos.BuscarPorCarga(obj.Codigo).FirstOrDefault()
                                     select new
                                     {
                                         obj.Codigo,
                                         Motorista = mot?.Nome ?? "",
                                         RGMotorista = mot?.RG ?? "",
                                         CPFMotorista = mot?.CPF_Formatado ?? "",
                                         CodigoTipoOperacao = obj.TipoOperacao?.Codigo ?? 0,
                                         DescricaoTipoOperacao = obj.TipoOperacao?.Descricao ?? "",
                                         obj.CodigoCargaEmbarcador,
                                         Transportador = obj.Empresa?.RazaoSocial ?? "",
                                         Filial = obj.Filial?.Descricao ?? "",
                                         Placa = obj.DadosSumarizados.Veiculos,
                                         Empresa = obj.Empresa?.Codigo ?? 0,
                                         NomeEmpresa = obj.Empresa?.RazaoSocial ?? "",
                                         CentroResultadoCodigo = centroResultados?.CentroResultado?.Codigo ?? 0,
                                         CentroResultadoDescricao = centroResultados?.CentroResultado?.Descricao ?? ""
                                     }).ToList();

                // Retorna dados
                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(dynListaCarga);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaConhecimentoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int carga;
                int.TryParse(Request.Params("Codigo"), out carga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Numero, "Numero", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Serie, "Serie", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DTEmissao, "DataEmissao", 9, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Remetente, "Remetente", 13, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Destinatario, "Destinatario", 13, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Frete, "ValorFrete", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ICMS, "ICMS", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Receber, "ValorAReceber", 7, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repCargaCTe.BuscarConhecimentoPorCarga(carga, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaCTe.ContarConhecimentoPorCarga(carga));

                var dynXmlNotaFiscal = (from obj in listaCargaCTe
                                        select new
                                        {
                                            Codigo = obj.CTe != null ? obj.CTe.Codigo : 0,
                                            Numero = obj.CTe != null ? obj.CTe.Numero : 0,
                                            Serie = obj.CTe != null && obj.CTe.Serie != null ? obj.CTe.Serie.Numero : 0,
                                            DataEmissao = obj.CTe != null && obj.CTe.DataEmissao.HasValue ? obj.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                            Remetente = obj.CTe != null && obj.CTe.Remetente != null ? obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.Localidade.DescricaoCidadeEstado + ")" : string.Empty,
                                            Destinatario = obj.CTe != null && obj.CTe.Destinatario != null ? obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.Localidade.DescricaoCidadeEstado + ")" : string.Empty,
                                            ValorFrete = obj.CTe != null ? obj.CTe.ValorFrete.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")) : 0.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                                            ICMS = obj.CTe != null ? obj.CTe.ValorICMS.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")) : 0.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                                            ValorAReceber = obj.CTe != null ? obj.CTe.ValorAReceber.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")) : 0.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR")),
                                        }).ToList();

                grid.AdicionaRows(dynXmlNotaFiscal);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosCargaPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                int codCargaPedido;
                int.TryParse(Request.Params("CodigoCargaPedido"), out codCargaPedido);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigoDetalhes(codCargaPedido);
                if (cargaPedido != null)
                {
                    return new JsonpResult(serCarga.ObterDadosCargaPedido(cargaPedido, TipoServicoMultisoftware, unitOfWork));
                }
                else
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.NaoExistePedidoInformadoNaConsulta);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarOsDetalhesDoPedido);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaReboquesCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codCarga;
                int.TryParse(Request.Params("Codigo"), out codCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Placa, "Placa", 50, Models.Grid.Align.center, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);

                //List<Dominio.Entidades.Veiculo> listaVeiculo = repCarga.BuscarVeiculosVinculados(codCarga, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(carga.VeiculosVinculados.Count());

                var dynRetorno = (from obj in carga.VeiculosVinculados.ToList()
                                  select new
                                  {
                                      Codigo = obj.Codigo,
                                      Placa = obj.Placa
                                  }).ToList();

                grid.AdicionaRows(dynRetorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreUmaFalhaAoConsultarOsVeiculosVinculadoCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasFinalizadasPelaOSMaeOuOS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridCargasFinalizadasPelaOSMaeOuOS();

                int codigoCargaAtual = Request.GetIntParam("CargaAtual");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCargaAtual);

                if (carga == null)
                {
                    var listaResultadoVazia = new List<object>();
                    grid.AdicionaRows(listaResultadoVazia);
                    grid.setarQuantidadeTotal(0);
                    return new JsonpResult(grid);
                }

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
                {
                    ListaNumeroOSMae = repositorioPedidoAdicional.BuscarNumerosOSMae(carga.Pedidos.Select(os => os.Pedido.Codigo).ToList()),
                    ListaNumeroOS = carga.Pedidos.Select(os => os.Pedido.NumeroOS).ToList(),
                    Situacoes = new List<SituacaoCarga> { SituacaoCarga.EmTransporte, SituacaoCarga.Encerrada }
                };

                if (!filtrosPesquisa.ListaNumeroOSMae.Any(item => !string.IsNullOrEmpty(item)) && !filtrosPesquisa.ListaNumeroOS.Any(item => !string.IsNullOrEmpty(item)))
                {
                    var listaResultadoVazia = new List<object>();
                    grid.AdicionaRows(listaResultadoVazia);
                    grid.setarQuantidadeTotal(0);
                    return new JsonpResult(grid);
                }

                filtrosPesquisa.CodigoCargaAtual = codigoCargaAtual;
                filtrosPesquisa.CodigoCargaEmbarcador = Request.GetStringParam("NumeroCarga");
                filtrosPesquisa.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada;

                int totalRegistros = repositorioCarga.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = totalRegistros > 0 ? repositorioCarga.Consultar(filtrosPesquisa, grid.ObterParametrosConsulta()) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var listaResultado = cargas.Select(o => new
                {
                    o.Codigo,
                    Descricao = o.CodigoCargaEmbarcador,
                    Carga = o.CodigoCargaEmbarcador,
                    Veiculo = o.Veiculo?.Placa ?? "",
                    Empresa = o.Empresa?.Descricao ?? ""
                }).ToList();

                grid.AdicionaRows(listaResultado);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar cargas agrupadas");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverReboqueCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codCarga, codigoReboque, codigoAcerto;
                int.TryParse(Request.Params("CodigoCarga"), out codCarga);
                int.TryParse(Request.Params("CodigoVeiculo"), out codigoReboque);
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoReboque);
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                if (carga != null && veiculo != null && acertoViagem != null)
                {
                    carga.VeiculosVinculados.Remove(veiculo);
                    repCarga.Atualizar(carga);

                    Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo acertoVeiculo = repAcertoVeiculo.BuscarPorAcertoEVeiculo(codigoAcerto, codigoReboque);
                    if (acertoVeiculo != null)
                    {
                        List<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento> listasAcertoResumoAbastecimento = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoVeiculo(codigoAcerto, codigoReboque);
                        foreach (var resumo in listasAcertoResumoAbastecimento)
                        {
                            repAcertoResumoAbastecimento.Deletar(resumo);
                        }
                        List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> abastecimenos = repAcertoAbastecimento.BuscarPorVeiculoCodigoAcerto(codigoAcerto, codigoReboque);
                        for (int i = 0; i < abastecimenos.Count; i++)
                        {
                            repAcertoAbastecimento.Deletar(abastecimenos[i]);
                        }
                        List<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio> pedagios = repAcertoPedagio.BuscarPorCodigoAcertoVeiculo(codigoAcerto, codigoReboque);
                        for (int i = 0; i < pedagios.Count; i++)
                        {
                            repAcertoPedagio.Deletar(pedagios[i]);
                        }
                        List<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> despesas = repAcertoOutraDespesa.BuscarPorCodigoAcertoVeiculo(codigoAcerto, codigoReboque);
                        for (int i = 0; i < despesas.Count; i++)
                        {
                            repAcertoOutraDespesa.Deletar(despesas[i]);
                        }
                        repAcertoVeiculo.Deletar(acertoVeiculo);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.RemoveuReboque, veiculo.Placa), unitOfWork);
                    unitOfWork.CommitChanges();
                    unitOfWork.Start(IsolationLevel.ReadUncommitted);

                    servAcertoViagem.AtualizarCargasAcerto(acertoViagem, unitOfWork, Request.Params("ListaCargas"), Auditado);
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas = servAcertoViagem.InserirCargaAcerto(acertoViagem, unitOfWork, ConfiguracaoEmbarcador.SituacaoCargaAcertoViagem);
                    List<Dominio.Entidades.Veiculo> listaVeiculo = new List<Dominio.Entidades.Veiculo>();
                    listaVeiculo = servAcertoViagem.InserirVeiculoAcerto(acertoViagem, unitOfWork);
                    servAcertoViagem.InserirPegadioAcerto(acertoViagem, unitOfWork, listaVeiculo, listaCargas);
                    servAcertoViagem.InserirAbastecimentoAcerto(acertoViagem, unitOfWork, listaVeiculo, listaCargas, Auditado);
                    serCarga.AlterarSegmentoCarga(ref carga);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        serCarga.AtualizarVeiculoEMotoristasPedidos(carga, Auditado, unitOfWork);

                    unitOfWork.CommitChanges();

                    var dynRetorno = new { Codigo = acertoViagem.Codigo }; //servAcertoViagem.RetornaObjetoCompletoAcertoViagem(acertoViagem.Codigo, unitOfWork);

                    return new JsonpResult(dynRetorno, true, Localization.Resources.Gerais.Geral.Sucesso);
                }
                else
                {
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.CargaVeiculoNaoLocalizada);
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoRemoverVinculoDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarReboqueCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codCarga, codigoReboque, codigoAcerto;
                int.TryParse(Request.Params("CodigoCarga"), out codCarga);
                int.TryParse(Request.Params("CodigoVeiculo"), out codigoReboque);
                int.TryParse(Request.Params("CodigoAcerto"), out codigoAcerto);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoReboque);
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                if (carga != null && veiculo != null && acertoViagem != null)
                {
                    if (carga.VeiculosVinculados.Contains(veiculo))
                        return new JsonpResult(false, Localization.Resources.Cargas.Carga.EsteVeiculoJaEstaVinculadoNaCargaSelecionado);

                    carga.VeiculosVinculados.Add(veiculo);
                    repCarga.Atualizar(carga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.AdicionouReboque, veiculo.Placa), unitOfWork);

                    servAcertoViagem.AtualizarCargasAcerto(acertoViagem, unitOfWork, Request.Params("ListaCargas"), Auditado);
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCargas = servAcertoViagem.InserirCargaAcerto(acertoViagem, unitOfWork, ConfiguracaoEmbarcador.SituacaoCargaAcertoViagem);
                    List<Dominio.Entidades.Veiculo> listaVeiculo = new List<Dominio.Entidades.Veiculo>();
                    listaVeiculo = servAcertoViagem.InserirVeiculoAcerto(acertoViagem, unitOfWork);
                    for (int i = 0; i < listaVeiculo.Count; i++)
                    {
                        if (listaVeiculo[i].SegmentoVeiculo == null && listaVeiculo[i].TipoVeiculo == "1")
                            return new JsonpResult(false, string.Format(Localization.Resources.Cargas.Carga.CarretaNaoPossuiSegmentoEmSeuCadastro, listaVeiculo[i].Placa));
                    }
                    servAcertoViagem.InserirPegadioAcerto(acertoViagem, unitOfWork, listaVeiculo, listaCargas);
                    servAcertoViagem.InserirAbastecimentoAcerto(acertoViagem, unitOfWork, listaVeiculo, listaCargas, Auditado);
                    serCarga.AlterarSegmentoCarga(ref carga);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        serCarga.AtualizarVeiculoEMotoristasPedidos(carga, Auditado, unitOfWork);

                    unitOfWork.CommitChanges();

                    var dynRetorno = new { Codigo = acertoViagem.Codigo }; //servAcertoViagem.RetornaObjetoCompletoAcertoViagem(acertoViagem.Codigo, unitOfWork);

                    return new JsonpResult(dynRetorno, true, Localization.Resources.Gerais.Geral.Sucesso);
                }
                else
                {
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.CargaVeiculoNaoLocalizada);
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAdicionarVinculoDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverPedidoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                bool usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1 = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm);

                if (!usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PermissaoInvalidaParaAdicionarPedido);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoPedido = Request.GetIntParam("CodigoPedido");
                string justificativaRemocaoPedidos = Request.GetStringParam("Justificativa");

                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.TelhaNorte);

                unitOfWork.Start();

                if (tipoIntegracao != null)
                {
                    Servicos.Embarcador.Integracao.TelhaNorte.IntegracaoTelhaNorte servicoIntegracaoTelhaNorte = new Servicos.Embarcador.Integracao.TelhaNorte.IntegracaoTelhaNorte(unitOfWork);

                    string mensagemRetorno = servicoIntegracaoTelhaNorte.SimularIntegracaoCarregamento(carga, codigoPedido, null);
                    if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                        throw new ControllerException(mensagemRetorno);
                }

                if (carga == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaNaoLocalizada);

                if (!(carga.TipoOperacao?.PermitirAdicionarRemoverPedidosEtapa1 ?? false) && !configuracaoEmbarcador.PermitirRemoverPedidoCargaComPendenciaDocumentos)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.TipoDeOperacaoNaoPermiteRemoverPedidoDaCarga);

                if (carga.CarregamentoIntegradoERP)
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoSG = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.SaintGobain);
                    if (tipoIntegracaoSG != null)
                        throw new ControllerException("Carregamento ja foi confirmado não permite remover pedido.");
                }

                Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork);

                if (servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarqueAguardandoRetornoIntegracao(carga))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.IntegracaoDaCargaParaGerarOrdemDeEmbarqueEstaAguardandoRetorno);

                bool permitirSeparacaoMercadoriaInformada = Request.GetBoolParam("PermitirSeparacaoMercadoriaInformada");

                ValidarSeparacaoMercadoria(carga, permitirSeparacaoMercadoriaInformada, unitOfWork);

                string mensagemErro = "";
                bool permitirRemoverTodos = Request.GetBoolParam("PermitirRemoverTodos");
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCargaEPedido(carga.Codigo, codigoPedido) ?? throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarPedidoInformado);

                ValidarAlteracoesPedidos(carga, pedidosAdicionados: null, cargaPedido.Pedido, unitOfWork);

                var reentregaPedido = cargaPedido?.ReentregaSolicitada ?? null;
                Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(carga, cargaPedido, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, reentregaPedido, permitirRemoverTodos);

                string auditoriaPedido = string.Empty;

                if (!string.IsNullOrEmpty(justificativaRemocaoPedidos))
                    auditoriaPedido = $" - Justificativa da Remoção: {justificativaRemocaoPedidos}";

                if (permitirSeparacaoMercadoriaInformada)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.RemoveuPedidoComSeparacaoMercadoriaInformada + auditoriaPedido, cargaPedido.Pedido.NumeroPedidoEmbarcador), unitOfWork);
                else
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.RemoveuPedido + auditoriaPedido, cargaPedido.Pedido.NumeroPedidoEmbarcador), unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Pedido, null, string.Format(Localization.Resources.Cargas.Carga.RemoveuPedidoCarga, carga.CodigoCargaEmbarcador), unitOfWork);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);

                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);
                servicoCarga.AtualizarCargaJanelaCarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaDescarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork, cargaPedidosAdicionados: null, cargaPedidosRemovidos: new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido });
                servicoIntegracaoOrdemEmbarqueMarfrig.IntegrarPedidoRemovido(carga, cargaPedido.Pedido, cargaPedido.NumeroReboque, Usuario);

                if (permitirRemoverTodos && carga.Pedidos.Count == 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                    {
                        Carga = carga,
                        MotivoCancelamento = Localization.Resources.Cargas.Carga.CancelamentoPorRemocaoDoUltimoPedido,
                        TipoServicoMultisoftware = TipoServicoMultisoftware,
                        Usuario = this.Usuario
                    };

                    Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, ConfiguracaoEmbarcador, unitOfWork);
                    Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware);

                    if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                        throw new ControllerException(cargaCancelamento.MensagemRejeicaoCancelamento);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento, null, Localization.Resources.Cargas.Carga.AdicionouCancelamentoDaCargaAoRemoverSeuUltimoPedido, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, Localization.Resources.Cargas.Carga.AdicionouCancelamentoDaCargaAoRemoverSeuUltimoPedido, unitOfWork);
                }
                ReenviaIntegracaoDadosTransporte(carga.Codigo, unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(servicoCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork));
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.PedidoVinculadoCarga)
                {
                    if (configuracaoGeralCarga?.NaoPermitirRemoverUltimoPedidoCarga ?? false)
                        return new JsonpResult(new
                        {
                            NaoPermitirRemoverUltimoPedidoCarga = true
                        });

                    return new JsonpResult(new
                    {
                        ConfirmarRemocaoPedidoViculadoCarga = true
                    });
                }

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.SeparacaoMercadoriaInformada)
                    return new JsonpResult(new
                    {
                        ConfirmarSeparacaoMercadoriaInformada = true,
                        MensagemErro = excecao.Message
                    });

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.AlteracoesPedidosNaoConfirmada)
                    return new JsonpResult(new
                    {
                        ConfirmarAlteracoesPedidos = true,
                        MensagemErro = excecao.Message
                    });

                return new JsonpResult(null, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoRemoverPedidoDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarPedidoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                bool usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1 = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm);

                if (!usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PermissaoInvalidaParaAdicionarPedido);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                string numeroCarregamentoPedido = Request.GetStringParam("NumeroCarregamentoPedido");
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);


                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.TelhaNorte);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                List<int> codigosPedidos = Request.GetListParam<int>("CodigosPedidos");

                if (!string.IsNullOrEmpty(numeroCarregamentoPedido))
                    pedidos = repositorioPedido.BuscarPorNumeroCarregamentoPedido(numeroCarregamentoPedido);
                else
                    pedidos = repositorioPedido.BuscarPorCodigos(codigosPedidos);

                if (pedidos.Count != codigosPedidos.Count && string.IsNullOrEmpty(numeroCarregamentoPedido))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoForamEncontradosTodosOsPedidosSelecionados);

                unitOfWork.Start();

                if (tipoIntegracao != null)
                {
                    Servicos.Embarcador.Integracao.TelhaNorte.IntegracaoTelhaNorte servicoIntegracaoTelhaNorte = new Servicos.Embarcador.Integracao.TelhaNorte.IntegracaoTelhaNorte(unitOfWork);

                    string mensagemRetorno = servicoIntegracaoTelhaNorte.SimularIntegracaoCarregamento(carga, 0, pedidos);
                    if (!string.IsNullOrWhiteSpace(mensagemRetorno))
                        throw new ControllerException(mensagemRetorno);
                }

                NumeroReboque numeroReboque = Request.GetEnumParam<NumeroReboque>("NumeroReboque");
                TipoCarregamentoPedido tipoCarregamentoPedido = Request.GetEnumParam("TipoCarregamentoPedido", TipoCarregamentoPedido.Normal);
                bool permitirSomentePedidoMesmaFilial = Request.GetBoolParam("PermitirSomentePedidoMesmaFilial");
                bool permitirSeparacaoMercadoriaInformada = Request.GetBoolParam("PermitirSeparacaoMercadoriaInformada");

                if (carga == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaNaoLocalizada);

                if (carga.CarregamentoIntegradoERP)
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoSG = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.SaintGobain);
                    if (tipoIntegracaoSG != null)
                        throw new ControllerException("Não permite adicionar pedido. Carregamento ja confirmado.");
                }

                if (pedidos.Count == 0)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarOsPedidos);

                if (pedidos.Count > 100 && string.IsNullOrEmpty(numeroCarregamentoPedido))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoPossivelAdicionarMaisDeVintePedidosPorVez);

                string mensagem = "";
                if (!Servicos.Embarcador.Carga.Cancelamento.VerificarSeJaIntegradoComERP(carga, out mensagem, "Adicionada", ConfiguracaoEmbarcador, unitOfWork))
                    throw new ControllerException(mensagem);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                if (!(carga.TipoOperacao?.PermitirAdicionarRemoverPedidosEtapa1 ?? false))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.TipoDeOperacaoNaoPermiteAdicionarPedidoDaCarga);

                Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork);

                if (servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarqueAguardandoRetornoIntegracao(carga))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.IntegracaoDaCargaParaGerarOrdemDeEmbarqueEstaAguardandoRetorno);

                ValidarSeparacaoMercadoria(carga, permitirSeparacaoMercadoriaInformada, unitOfWork);
                ValidarAlteracoesPedidos(carga, pedidos, pedidoRemovido: null, unitOfWork);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosAdicionados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                bool existeIntegracaoTelhaNorte = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.TelhaNorte);

                if (existeIntegracaoTelhaNorte && (carga.TipoOperacao?.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores ?? false))
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasTelhaNorte = repositorioCarga.BuscarCargasAgrupadasTelhaNorte(carga.CodigoCargaEmbarcador);

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaTelha in cargasTelhaNorte)
                        cargaPedidosAdicionados.Add(Servicos.Embarcador.Carga.CargaPedido.AdicionarPedidoCarga(cargaTelha, pedidos.FirstOrDefault(), numeroReboque, tipoCarregamentoPedido, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, permitirSomentePedidoMesmaFilial, true, false, false));
                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = Servicos.Embarcador.Carga.CargaPedido.AdicionarPedidoCarga(carga, pedido, numeroReboque, tipoCarregamentoPedido, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, permitirSomentePedidoMesmaFilial, true);
                        cargaPedidosAdicionados.Add(cargaPedido);
                    }
                }
                ReenviaIntegracaoDadosTransporte(carga.Codigo, unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, listaCargaPedidos, configuracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);
                servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, listaCargaPedidos, unitOfWork, TipoServicoMultisoftware, configuracaoPedido);
                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, listaCargaPedidos, configuracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                if (permitirSeparacaoMercadoriaInformada)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.AdicionouOsPedidosComSeparacaoDeMercadoriasInformada, string.Join(",", pedidos.Select(obj => obj.NumeroPedidoEmbarcador).ToList())), unitOfWork);
                else
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.AdicionouOsPedidos, string.Join(",", pedidos.Select(obj => obj.NumeroPedidoEmbarcador).ToList())), unitOfWork);

                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);
                servicoCarga.ValidarCapacidadeModeloVeicularCarga(carga, configuracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaCarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaDescarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork, cargaPedidosAdicionados, cargaPedidosRemovidos: null);

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    servicoIntegracaoOrdemEmbarqueMarfrig.IntegrarPedidoAdicionado(carga, pedido, numeroReboque, Usuario);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, string.Format(Localization.Resources.Cargas.Carga.AdicionouPedidoCarga, carga.CodigoCargaEmbarcador), unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(servicoCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork));
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.SeparacaoMercadoriaInformada)
                    return new JsonpResult(new
                    {
                        ConfirmarSeparacaoMercadoriaInformada = true,
                        MensagemErro = excecao.Message
                    });

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.AlteracoesPedidosNaoConfirmada)
                    return new JsonpResult(new
                    {
                        ConfirmarAlteracoesPedidos = true,
                        MensagemErro = excecao.Message
                    });

                return new JsonpResult(null, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAdicionarPedidoNaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarPedidoOutraFilialCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                bool usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1 = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm);

                if (!usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PermissaoInvalidaParaAdicionarPedido);

                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoPedido = Request.GetIntParam("CodigoPedido");
                NumeroReboque numeroReboque = Request.GetEnumParam<NumeroReboque>("NumeroReboque");
                TipoCarregamentoPedido tipoCarregamentoPedido = Request.GetEnumParam("TipoCarregamentoPedido", TipoCarregamentoPedido.Normal);
                DateTime? inicioCarregamento = Request.GetNullableDateTimeParam("InicioCarregamento");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                if (carga == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaNaoLocalizada);

                if (!inicioCarregamento.HasValue)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PeriodoDeCarregamentoDeveSerInformado);

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigoPedido);
                if (pedido == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarPedido);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                if (!configuracaoEmbarcador.PermitirAdicionarPedidoOutraFilialCarga)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.ConfiguracaoAtualNaoPermiteAdicionarUmPedidoDeOutraFilialNaCarga);

                Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork);

                if (servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarqueAguardandoRetornoIntegracao(carga))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.IntegracaoDaCargaParaGerarOrdemDeEmbarqueEstaAguardandoRetorno);

                Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = Servicos.Embarcador.Carga.CargaPedido.AdicionarPedidoOutraFilialCarga(carga, pedido, inicioCarregamento.Value, numeroReboque, tipoCarregamentoPedido, configuracaoEmbarcador, TipoServicoMultisoftware, Cliente, unitOfWork, Auditado);

                servicoIntegracaoOrdemEmbarqueMarfrig.IntegrarPedidoAdicionado(carga, pedido, numeroReboque, Usuario);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                return new JsonpResult(servicoCarga.ObterDetalhesDaCarga(cargaAgrupada, TipoServicoMultisoftware, unitOfWork));
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(null, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAdicionarPedidoNaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarPedidoTrocaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                bool permiteTrocarPedidoDefinitivosUFsDestinatariosDiferentes = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteTrocarPedidosDefinitivosUFsDestinatariosDiferentes);
                bool usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1 = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm);

                if (!usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PermissaoInvalidaParaAdicionarPedido);

                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoPedido = Request.GetIntParam("CodigoPedido");
                NumeroReboque numeroReboque = Request.GetEnumParam<NumeroReboque>("NumeroReboque");
                TipoCarregamentoPedido tipoCarregamentoPedido = Request.GetEnumParam("TipoCarregamentoPedido", TipoCarregamentoPedido.Normal);
                List<int> listaCodigoPedidoRemover = Request.GetListParam<int>("PedidosTroca");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                if (carga == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaNaoLocalizada);

                if (listaCodigoPedidoRemover.Count == 0)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NenhumPedidoParaTrocaFoiInformado);

                Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork);
                bool possuiOrdemEmbarque = servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarque(carga);
                bool integrarTrocaPedidoOrdemEmbarque = possuiOrdemEmbarque && carga.CargaDePreCarga;

                if (possuiOrdemEmbarque && (listaCodigoPedidoRemover.Count() > 1))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaPossuiOrdemDeEmbarquePermiteSubstituirApenasUmPedidoPorOutro);

                if (servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarqueAguardandoRetornoIntegracao(carga))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.IntegracaoDaCargaParaGerarOrdemDeEmbarqueEstaAguardandoRetorno);

                Repositorio.Embarcador.Pedidos.PedidoTroca repositorioPedidoTroca = new Repositorio.Embarcador.Pedidos.PedidoTroca(unitOfWork);

                if (repositorioPedidoTroca.VerificarExistePorPedidoDefinitivo(codigoPedido))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PedidoInformadoJaFoiTrocado);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                if (!configuracaoEmbarcador.PermitirTrocarPedidoCarga)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.ConfiguracaoAtualNaoPermiteTrocarPedidoDaCarga);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosRemover = repositorioCargaPedido.BuscarPorCargaEPedidos(carga.Codigo, listaCodigoPedidoRemover);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAdicionar = repositorioPedido.BuscarPorCodigo(codigoPedido);
                bool possuiPedidosProvisoriosParaRemover = cargaPedidosRemover.Any(o => IsPedidoProvisorio(o.Pedido, configuracaoEmbarcador));
                bool possuiPedidosDefinitivosParaRemover = cargaPedidosRemover.Any(o => !IsPedidoProvisorio(o.Pedido, configuracaoEmbarcador));

                if (possuiOrdemEmbarque && (pedidoAdicionar.Filial?.Codigo != cargaPedidosRemover.FirstOrDefault().Pedido.Filial.Codigo))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaPossuiOrdemDeEmbarquePermiteSubstituirApenasPedidosDaMesmaFilial);

                if (possuiPedidosProvisoriosParaRemover && possuiPedidosDefinitivosParaRemover)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.TrocaDePedidosProvisoriosDefinitivosDeveSerRealizadaSeparadamente);

                if (!permiteTrocarPedidoDefinitivosUFsDestinatariosDiferentes && possuiPedidosDefinitivosParaRemover && cargaPedidosRemover.Any(o => o.ObterDestinatario().Localidade.Estado.Sigla != pedidoAdicionar.ObterDestinatario().Localidade.Estado.Sigla))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.UFDosdestinatariosDeveSerMesmaEmTodosPedidosDaTroca);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoAdicionado = Servicos.Embarcador.Carga.CargaPedido.AdicionarPedidoCarga(carga, pedidoAdicionar, numeroReboque, tipoCarregamentoPedido, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.AdicionouPedido, pedidoAdicionar.NumeroPedidoEmbarcador), unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRemover in cargaPedidosRemover)
                {
                    Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(carga, cargaPedidoRemover, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga);
                    bool pedidoProvisorio = IsPedidoProvisorio(cargaPedidoRemover.Pedido, configuracaoEmbarcador);

                    if (pedidoProvisorio || integrarTrocaPedidoOrdemEmbarque)
                    {
                        //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                        Servicos.Log.TratarErro($"Pedido {cargaPedidoRemover.Pedido.NumeroPedidoEmbarcador} - Totalmente carregado.: {cargaPedidoRemover.Pedido.PesoSaldoRestante}. CargaController.AdicionarPedidoTrocaCarga", "SaldoPedido");
                        cargaPedidoRemover.Pedido.PedidoTotalmenteCarregado = true;
                        repositorioPedido.Atualizar(cargaPedidoRemover.Pedido);

                        repositorioPedidoTroca.Inserir(new Dominio.Entidades.Embarcador.Pedidos.PedidoTroca()
                        {
                            PedidoDefinitivo = pedidoAdicionar,
                            PedidoProvisorio = cargaPedidoRemover.Pedido
                        });
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.RemoveuPedido, cargaPedidoRemover.Pedido.NumeroPedidoEmbarcador), unitOfWork);
                }

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);

                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);
                servicoCarga.ValidarCapacidadeModeloVeicularCarga(carga, configuracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaCarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaDescarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork, cargaPedidosAdicionados: new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedidoAdicionado }, cargaPedidosRemover);

                servicoIntegracaoOrdemEmbarqueMarfrig.IntegrarTrocaPedido(carga, cargaPedidosRemover.First().Pedido, pedidoAdicionar, cargaPedidosRemover.First().NumeroReboque, Usuario);

                unitOfWork.CommitChanges();

                return new JsonpResult(servicoCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork));
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(null, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAdicionarPedidoNaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarNovosPedidosPorNotasAvulsas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                bool usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1 = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm);

                if (!usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PermissaoInvalidaParaAdicionarPedido);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int numeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal");
                string chaveNotaFiscal = Request.GetStringParam("ChaveNotaFiscal");

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaNaoLocalizada);

                Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados cargaDadosSumarizados = repCargaDadosSumarizados.BuscarPorCargaComFetch(codigoCarga);
                List<int> codigosOrigens = cargaDadosSumarizados.ClientesRemetentes.Select(o => o.Localidade.Codigo).ToList();
                List<int> codigosDocumentos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Documentos"));
                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");
                List<int> listaCodigosDocumentos = repPedidoXMLNotaFiscal.ObterCodigosDocumentosPorPorOrigemDaCarga(codigosOrigens, numeroNotaFiscal, chaveNotaFiscal, selecionarTodos, codigosDocumentos);

                if (listaCodigosDocumentos.Count == 0)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.SelecioneAoMenosUmDocumentoParaRealizarVinculoCarga);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlsNotaFiscal = repXmlNotaFiscal.BuscarPorCodigos(listaCodigosDocumentos);

                if (xmlsNotaFiscal.Count == 0)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NenhumaNotaFoiEncontrada);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosAdicionados = serCargaNotaFiscal.GerarPedidosPorNotasFiscaisEVincularNaCarga(carga, xmlsNotaFiscal, TipoServicoMultisoftware, ConfiguracaoEmbarcador, Auditado);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);

                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware);
                servicoCarga.ValidarCapacidadeModeloVeicularCarga(carga, ConfiguracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaCarregamento(carga, cargaJanelaCarregamento, ConfiguracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaDescarregamento(carga, cargaJanelaCarregamento, ConfiguracaoEmbarcador, unitOfWork, cargaPedidosAdicionados, cargaPedidosRemovidos: null);

                unitOfWork.CommitChanges();

                return new JsonpResult(servicoCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork));
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAdicionarPedidoNaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DesfazerTrocaPedidoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                bool usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1 = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm);

                if (!usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PermissaoInvalidaParaAdicionarPedido);

                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoPedido = Request.GetIntParam("CodigoPedido");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga) ?? throw new ControllerException(Localization.Resources.Cargas.Carga.CargaNaoLocalizada);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCargaEPedido(carga.Codigo, codigoPedido) ?? throw new ControllerException(Localization.Resources.Cargas.Carga.PedidoNaoLocalizado);

                Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao repositorioOrdemEmbarqueHistoricoIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueHistoricoIntegracao ordemEmbarqueHistoricoIntegracao = repositorioOrdemEmbarqueHistoricoIntegracao.BuscarUltimoPorTrocaPedido(carga.Codigo, cargaPedido.Pedido.Codigo, cargaPedido.NumeroReboque);

                if (ordemEmbarqueHistoricoIntegracao?.SituacaoIntegracao.IntegracaoPendente() ?? false)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.TrocaDePedidoDaOrdemDeEmbarqueEstaAguardandoRetorno);

                Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork);

                if (servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarqueAguardandoRetornoIntegracao(carga))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.IntegracaoDaCargaParaGerarOrdemDeEmbarqueEstaAguardandoRetorno);

                Servicos.Embarcador.Carga.CargaPedido.DesfazerTrocaPedidoCarga(carga, cargaPedido.Pedido, TipoServicoMultisoftware, Auditado, unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                return new JsonpResult(servicoCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork));
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(null, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoDesfazerTrocaDoPedidoNaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> TrocarPedidoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                bool permiteTrocarPedidoDefinitivosUFsDestinatariosDiferentes = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteTrocarPedidosDefinitivosUFsDestinatariosDiferentes);
                bool usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1 = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirAdicionarRemoverPedidoEtapaUm);

                if (!usuarioPossuiPermissaoAdicionarOuRemoverPedidoEtapa1)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PermissaoInvalidaParaAdicionarPedido);

                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoPedido = Request.GetIntParam("CodigoPedido");
                List<dynamic> listaPedidoTroca = Request.GetListParam<dynamic>("PedidosTroca");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga) ?? throw new ControllerException(Localization.Resources.Cargas.Carga.CargaNaoLocalizada);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();


                if (listaPedidoTroca.Count == 0)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.NenhumPedidoParaTrocaFoiInformado);

                Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork);
                bool possuiOrdemEmbarque = servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarque(carga);
                bool integrarTrocaPedidoOrdemEmbarque = possuiOrdemEmbarque && carga.CargaDePreCarga;

                if (possuiOrdemEmbarque && (listaPedidoTroca.Count() > 1))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaPossuiOrdemDeEmbarquePermiteSubstituirApenasUmPedidoPorOutro);

                if (servicoIntegracaoOrdemEmbarqueMarfrig.PossuiOrdemEmbarqueAguardandoRetornoIntegracao(carga))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.IntegracaoDaCargaParaGerarOrdemDeEmbarqueEstaAguardandoRetorno);

                Repositorio.Embarcador.Pedidos.PedidoTroca repositorioPedidoTroca = new Repositorio.Embarcador.Pedidos.PedidoTroca(unitOfWork);

                if (repositorioPedidoTroca.VerificarExistePorPedidoProvisorio(codigoPedido))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PedidoInformadoJaFoiTrocado);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                if (!configuracaoEmbarcador.PermitirTrocarPedidoCarga)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.ConfiguracaoAtualNaoPermiteTrocarPedidoDaCarga);

                List<int> listaCodigoPedidoAdicionar = (from o in listaPedidoTroca select ((string)o.Pedido).ToInt()).ToList();
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRemover = repositorioCargaPedido.BuscarPorCargaEPedido(carga.Codigo, codigoPedido) ?? throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarPedidoInformado);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoRemover = cargaPedidoRemover.Pedido;
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAdicionar = repositorioPedido.BuscarPorCodigos(listaCodigoPedidoAdicionar);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosAdicionar = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                bool pedidoProvisorio = IsPedidoProvisorio(pedidoRemover, configuracaoEmbarcador);

                if (possuiOrdemEmbarque && (cargaPedidoRemover.Pedido.Filial?.Codigo != pedidosAdicionar.FirstOrDefault().Filial?.Codigo))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaPossuiOrdemDeEmbarquePermiteSubstituirApenasPedidosDaMesmaFilial);

                if (!permiteTrocarPedidoDefinitivosUFsDestinatariosDiferentes && !pedidoProvisorio && pedidosAdicionar.Any(o => o.ObterDestinatario().Localidade.Estado.Sigla != pedidoRemover.ObterDestinatario().Localidade.Estado.Sigla))
                    throw new ControllerException(Localization.Resources.Cargas.Carga.UFDosdestinatariosDeveSerMesmaEmTodosPedidosDaTroca);

                foreach (var pedidoTroca in listaPedidoTroca)
                {
                    int codigoPedidoTroca = ((string)pedidoTroca.Pedido).ToInt();
                    TipoCarregamentoPedido tipoCarregamentoPedidoTroca = ((string)pedidoTroca.TipoCarregamentoPedido).ToEnum(TipoCarregamentoPedido.Normal);
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAdicionar = (from o in pedidosAdicionar where o.Codigo == codigoPedidoTroca select o).FirstOrDefault();
                    cargaPedidosAdicionar.Add(Servicos.Embarcador.Carga.CargaPedido.AdicionarPedidoCarga(carga, pedidoAdicionar, cargaPedidoRemover.NumeroReboque, tipoCarregamentoPedidoTroca, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork));
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.AdicionouPedido, pedidoAdicionar.NumeroPedidoEmbarcador), unitOfWork);

                    if (pedidoProvisorio || integrarTrocaPedidoOrdemEmbarque)
                        repositorioPedidoTroca.Inserir(new Dominio.Entidades.Embarcador.Pedidos.PedidoTroca()
                        {
                            PedidoDefinitivo = pedidoAdicionar,
                            PedidoProvisorio = pedidoRemover
                        });
                }

                Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(carga, cargaPedidoRemover, configuracaoEmbarcador, TipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga);

                if (pedidoProvisorio || integrarTrocaPedidoOrdemEmbarque)
                {
                    //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                    Servicos.Log.TratarErro($"Pedido {pedidoRemover.NumeroPedidoEmbarcador} - Totalmente carregado.: {pedidoRemover.PesoSaldoRestante}. CargaController.TrocarPedidoCarga", "SaldoPedido");
                    pedidoRemover.PedidoTotalmenteCarregado = true;
                    repositorioPedido.Atualizar(pedidoRemover);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, string.Format(Localization.Resources.Cargas.Carga.RemoveuPedido, pedidoRemover.NumeroPedidoEmbarcador), unitOfWork);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);

                Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);
                servicoCarga.ValidarCapacidadeModeloVeicularCarga(carga, configuracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaCarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork);
                servicoCarga.AtualizarCargaJanelaDescarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork, cargaPedidosAdicionar, cargaPedidosRemovidos: new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedidoRemover });
                servicoIntegracaoOrdemEmbarqueMarfrig.IntegrarTrocaPedido(carga, pedidoRemover, pedidosAdicionar.First(), cargaPedidoRemover.NumeroReboque, Usuario);

                unitOfWork.CommitChanges();

                return new JsonpResult(servicoCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork));
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(null, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAdicionarPedidoNaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDetalhesCargaPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                List<string> filiais = repFilial.BuscarListaCNPJAtivas();

                Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                List<dynamic> pedidos = (from obj in cargaPedidos select servicoPedido.ObterDetalhesPedido(obj, filiais, carga, unitOfWork)).ToList();
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = cargaJanelaCarregamento?.CentroCarregamento;

                if (centroCarregamento == null)
                {
                    Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorTipoCargaEFilial(carga.TipoDeCarga?.Codigo ?? 0, carga.Filial?.Codigo ?? 0, ativo: true, carga);
                }

                return new JsonpResult(new
                {
                    Carga = new
                    {
                        CodigoCarga = carga.Codigo,
                        CentroCarregamento = new { Codigo = centroCarregamento?.Codigo ?? 0, Descricao = centroCarregamento?.Descricao ?? "" },
                        Filial = new { Codigo = carga.Filial?.Codigo ?? 0, Descricao = carga.Filial?.Descricao ?? "" },
                        TipoCarga = new { Codigo = carga.TipoDeCarga?.Codigo ?? 0, Descricao = carga.TipoDeCarga?.Descricao ?? "" },
                        PermitirAdicionarRemoverPedidosEtapa1 = carga.TipoOperacao?.PermitirAdicionarRemoverPedidosEtapa1 ?? false
                    },
                    Pedidos = pedidos
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarOsDetalhesDosPedidos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadLoteDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = null;
                int quantidade = 0;
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "desc",
                    InicioRegistros = 0,
                    LimiteRegistros = 0,
                    PropriedadeOrdenar = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) ? "CodigoCargaEmbarcador" : "Codigo"
                };

                ExecutarBusca(ref listaCarga, ref quantidade, parametroConsulta, configuracaoGeral, true);

                if (listaCarga != null && listaCarga.Count > 0)
                {
                    int codigoUsuario = this.Usuario.Codigo;
                    string stringConexao = _conexao.StringConexao;
                    string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos;
                    string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;
                    string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                    string caminhoArquivosAnexos = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido" });

                    List<int> codigosCargas = listaCarga.Select(o => o.Codigo).ToList();

                    if (codigosCargas.Count <= 0)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiEncontradaNenhumaCargaAutorizadaParaGerarImpressaoDosDocumentos);

                    List<int> ctes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigosCargas, false, false);

                    if (ctes.Count <= 0)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoForamEncontradasCTesautorizadosParaEstaCarga);

                    Task.Run(() => Zeus.Embarcador.ZeusNFe.Zeus.GerarPDFTodosDocumentos(0, ctes, stringConexao, codigoUsuario, caminhoRelatorios, caminhoArquivos, diretorioDocumentosFiscais, "Cargas/Carga", caminhoArquivosAnexos));

                    return new JsonpResult(true, true, Localization.Resources.Gerais.Geral.Sucesso);
                }
                else
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoForamEncontradosCTesAutorizadosComConsultaRealizada);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoSolicitarOsDocumentosDasCargasPesquisadas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarAnexosDaCargaNFe(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Repositorio.Embarcador.Cargas.CargaNFeAnexo repositorioCargaNFeAnexo = new Repositorio.Embarcador.Cargas.CargaNFeAnexo(unitOfWork, cancellationToken);

                bool isTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe;

                List<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo> listaAnexos = await repositorioCargaNFeAnexo.BuscarPorCargaAsync(codigoCarga, isTransportador);

                var listaDinamicaAnexos = (
                                            from anexo in listaAnexos
                                            select new
                                            {
                                                anexo.Codigo,
                                                anexo.Descricao,
                                                anexo.NomeArquivo,
                                                anexo.OcultarParaTransportador
                                            }
                                        ).ToList();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos
                });
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarAnexosDaCargaCTe(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Repositorio.Embarcador.Cargas.CargaCTeAnexo repositorioCargaCTeAnexo = new Repositorio.Embarcador.Cargas.CargaCTeAnexo(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeAnexo> listaAnexos = await repositorioCargaCTeAnexo.BuscarPorCargaAsync(codigoCarga);

                var listaDinamicaAnexos = (
                                            from anexo in listaAnexos
                                            select new
                                            {
                                                anexo.Codigo,
                                                anexo.Descricao,
                                                anexo.NomeArquivo,
                                            }
                                        ).ToList();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos
                });
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AlterarDataPrevisaoEntregaPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPedido = Request.GetIntParam("Pedido");
                DateTime dataPrevisaoEntrega = Request.GetDateTimeParam("DataPrevisaoEntrega");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                string observacao = Request.GetStringParam("Observacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAlteracaoDataPedido? responsavel = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAlteracaoDataPedido>("Responsavel");
                if (configuracao.NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga)
                {
                    if (string.IsNullOrWhiteSpace(observacao)) return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.DeveSerInformadaObservacao);
                    if (responsavel == null) return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.DeveSerInformadoResponsavelPelaAlteracao);
                }

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoHistoricoAlteracaoData repPedidoHistoricoAlteracaoData = new Repositorio.Embarcador.Pedidos.PedidoHistoricoAlteracaoData(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido, true);
                if (pedido == null) return new JsonpResult(false, Localization.Resources.Cargas.Carga.PedidoNaoEncontrado);
                if (pedido.PrevisaoEntrega == dataPrevisaoEntrega) return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.DataNaoFoiAlterada);

                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = pedido.CargasPedido.FirstOrDefault();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = carga.Pedidos.ToList();
                int total = pedidos.Count();
                for (int i = 0; i < total; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = pedidos[i];
                    if (cargaPedido.Pedido.Destinatario.Codigo == pedido.Destinatario.Codigo)
                    {
                        CriarPedidoHistoricoAlteracaoData(cargaPedido.Pedido, TipoDataPedido.DataPrevisaoEntrega, responsavel.Value, observacao, cargaPedido.Pedido.PrevisaoEntrega, configuracao, repPedidoHistoricoAlteracaoData);
                        cargaPedido.Pedido.PrevisaoEntrega = dataPrevisaoEntrega;
                        repPedido.Atualizar(cargaPedido.Pedido, Auditado);

                        AlterarDataEntregaControleEntregadoPedido(cargaPedido.Pedido, dataPrevisaoEntrega, cargaPedido.Carga);
                    }
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Servicos.Embarcador.Integracao.Diaria.DiariaMotorista servicoDiariaMotorista = new Servicos.Embarcador.Integracao.Diaria.DiariaMotorista(unitOfWork);
                    servicoDiariaMotorista.RecalcularPagamentoMotoristaEmbarcador(dataPrevisaoEntrega, carga, pedidos, Auditado);

                    Servicos.Embarcador.Carga.Carga.AjustarDataPrevisaoTerminoCarga(carga, 0, unitOfWork);
                }

                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, pedidos, configuracao, unitOfWork, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarDataDePrevisaoDeEntregaDoPedido);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataPrevisaoSaidaPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPedido = Request.GetIntParam("Pedido");
                DateTime dataPrevisaoSaida = Request.GetDateTimeParam("DataPrevisaoSaida");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();


                string observacao = Request.GetStringParam("Observacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAlteracaoDataPedido? responsavel = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAlteracaoDataPedido>("Responsavel");
                if (configuracao.NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga)
                {
                    if (string.IsNullOrWhiteSpace(observacao)) return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.DeveSerInformadaObservacao);
                    if (responsavel == null) return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.DeveSerInformadoResponsavelPelaAlteracao);
                }

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoHistoricoAlteracaoData repPedidoHistoricoAlteracaoData = new Repositorio.Embarcador.Pedidos.PedidoHistoricoAlteracaoData(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido, true);
                if (pedido == null) return new JsonpResult(false, Localization.Resources.Cargas.Carga.PedidoNaoEncontrado);
                if (pedido.DataPrevisaoSaida == dataPrevisaoSaida) return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.DataNaoFoiAlterada);

                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = pedido.CargasPedido.FirstOrDefault();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos = carga.Pedidos.ToList();
                int total = pedidos.Count();
                for (int i = 0; i < total; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = pedidos[i];
                    if (cargaPedido.Pedido.Remetente.Codigo == pedido.Remetente.Codigo)
                    {
                        CriarPedidoHistoricoAlteracaoData(cargaPedido.Pedido, TipoDataPedido.DataPrevisaoSaida, responsavel.Value, observacao, cargaPedido.Pedido.DataPrevisaoSaida, configuracao, repPedidoHistoricoAlteracaoData);
                        cargaPedido.Pedido.DataPrevisaoSaida = dataPrevisaoSaida;
                        repPedido.Atualizar(cargaPedido.Pedido, Auditado);
                    }
                }
                
                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, pedidos, configuracao, unitOfWork, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarDataDePrevisaoDeSaidaDoPedido);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarQuantidadeVolumesPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {
                int codigoPedido = Request.GetIntParam("Pedido");
                int quantidadeVolumes = Request.GetIntParam("QuantidadeVolumes");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido, true);
                if (pedido == null)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.PedidoNaoEncontrado);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorPedido(pedido.Codigo);

                if (pedido.QtVolumes == quantidadeVolumes)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.VolumeNaoFoiAlterado);

                pedido.QtVolumes = quantidadeVolumes;

                repPedido.Atualizar(pedido, Auditado);

                if (carga.DadosSumarizados != null)
                {
                    carga.DadosSumarizados.VolumesTotal = quantidadeVolumes;
                    repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados, Auditado);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarQuantidadeDeVolumeDoPedido);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarTipoCarregamentoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoPedido = Request.GetIntParam("Pedido");
                TipoCarregamentoPedido tipoCarregamentoPedido = Request.GetEnumParam("TipoCarregamentoPedido", TipoCarregamentoPedido.Normal);

                if (tipoCarregamentoPedido == TipoCarregamentoPedido.NaoDefinido)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.TipoDeCarregamentoNaoPodeSerDefinido);

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCargaEPedido(codigoCarga, codigoPedido);

                if (cargaPedido == null)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.PedidoNaoFoiEncontrado);

                cargaPedido.TipoCarregamentoPedido = tipoCarregamentoPedido;

                repositorioCargaPedido.Atualizar(cargaPedido);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, string.Format(Localization.Resources.Cargas.Carga.AlterouTipoDoCarregamentoDoPedidoPara, cargaPedido.Pedido.NumeroPedidoEmbarcador, tipoCarregamentoPedido.ObterDescricao()), unitOfWork);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(cargaPedido.Carga.Codigo);

                servicoCarga.AtualizarCargaJanelaDescarregamento(cargaPedido.Carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork, cargaPedidosAdicionados: new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, cargaPedidosRemovidos: null);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAlterarTipoDeCarregamentoDoPedido);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ControleVeiculosCheckList()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                byte[] pdf = ReportRequest.WithType(ReportType.DocumentoCargaRiachuelo)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoCarga", carga.Codigo)
                    .CallReport()
                    .GetContentFile();

                if (pdf == null)
                {
                    return new JsonpResult(true, false, Localization.Resources.Cargas.Carga.NaoFoiPossivelGerarDocumentoDaCarga);
                }

                return Arquivo(pdf, "application/pdf", Localization.Resources.Cargas.Carga.DocumentoCarga + " " + carga.Descricao + ".pdf");

            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoGerarDocumentoDaCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarFichaMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);
                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Dominio.Entidades.Usuario motorista = repCargaMotorista.BuscarPrimeiroMotoristaPorCarga(carga.Codigo);
                if (motorista == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarMotorista);

                Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorMotorista(motorista.Codigo, "0");
                List<Dominio.Entidades.Veiculo> veiculoReboques = repVeiculo.BuscarPorCPFMotorista(motorista.CPF);
                List<int> codigosReboques = new List<int>();
                foreach (Dominio.Entidades.Veiculo reb in veiculoReboques)
                {
                    if (reb.TipoVeiculo.Equals("1"))
                        codigosReboques.Add(reb.Codigo);
                }

                byte[] pdf = ReportRequest.WithType(ReportType.FichaMotoristaCarga)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoCarga", carga.Codigo)
                    .AddExtraData("CodigoVeiucloTracao", veiculoTracao.Codigo)
                    .AddExtraData("CodigosReboques", codigosReboques.ToJson())
                    .CallReport()
                    .GetContentFile();

                if (pdf == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelGerarDocumentoDaCarga);

                Servicos.Embarcador.Carga.FichaMotorista svcFichaMotorista = new Servicos.Embarcador.Carga.FichaMotorista(unitOfWork);

                return Arquivo(svcFichaMotorista.ObterFichaMotoristaMergePDFs(pdf, motorista.Codigo, veiculoTracao.Codigo, codigosReboques), "application/pdf", $"Ficha_Motorista_{carga.CodigoCargaEmbarcador}.pdf");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoGerarFichaDoMotorista);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarOrdemColeta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Servicos.Embarcador.Pedido.ImpressaoPedido serImpressaoPedido = new Servicos.Embarcador.Pedido.ImpressaoPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);
                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarNenhumPedido);

                if (!serImpressaoPedido.GerarRelatorioTMS(false, cargaPedido.Pedido, false, out string msg, true, carga, carga.Codigo, true, false, _conexao.StringConexao, TipoServicoMultisoftware, carga.Empresa?.NomeFantasia ?? "", carga.Operador, false, out string guidRelatorio, out string fileName))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelGerarImpressaoDacarga);

                if (string.IsNullOrWhiteSpace(guidRelatorio))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelGerarImpressaoDacarga);

                string pastaRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();
                string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, guidRelatorio);
                string caminhoArquivoFileName = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, fileName.Replace("-", ""));

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo + ".pdf"))
                {
                    byte[] pdfRelatorioCarga = null;
                    pdfRelatorioCarga = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivo + ".pdf");

                    if (pdfRelatorioCarga != null)
                        return Arquivo(pdfRelatorioCarga, "application/pdf", $"Ordem_Coleta_{carga.CodigoCargaEmbarcador}.pdf");
                    else
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelGerarImpressaoDacarga);
                }
                else if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivoFileName))
                {
                    byte[] pdfRelatorioCarga = null;
                    pdfRelatorioCarga = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivoFileName);

                    if (pdfRelatorioCarga != null)
                        return Arquivo(pdfRelatorioCarga, "application/pdf", $"Ordem_Coleta_{carga.CodigoCargaEmbarcador}.pdf");
                    else
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelGerarImpressaoDacarga);
                }
                else
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelGerarImpressaoDacarga);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoGerarImpressaoDaOrdemDeColeta);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterRetiradaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(carga.Codigo);

                if (carga.SituacaoCarga == SituacaoCarga.Cancelada && retiradaContainer != null)
                    retiradaContainer.Local = null;

                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = retiradaContainer?.ContainerTipo ?? carga.ModeloVeicularCarga?.ContainerTipo ?? carga.Carregamento?.ModeloVeicularCarga?.ContainerTipo;
                Dominio.Entidades.Embarcador.Pedidos.Container container = retiradaContainer?.ColetaContainer?.Container ?? retiradaContainer?.Container;
                string mensagem = string.Empty;

                if (carga.LiberadaSemRetiradaContainer)
                    mensagem = Localization.Resources.Cargas.Carga.CargaLiberadaSemRetiradaContainer;

                return new JsonpResult(new
                {
                    Local = new { Codigo = retiradaContainer?.Local?.Codigo ?? 0, Descricao = retiradaContainer?.Local?.Descricao ?? "" },
                    ContainerTipo = new { containerTipo.Codigo, containerTipo.Descricao },
                    Container = new { Codigo = container?.Codigo, Descricao = container?.Descricao },
                    PermitirEditar = (retiradaContainer?.ColetaContainer == null) && !carga.LiberadaSemRetiradaContainer,
                    PermiteRemoverContainer = container != null && (carga.SituacaoCarga == SituacaoCarga.AgNFe || carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.AgTransportador),
                    Mensagem = mensagem
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterRetiradaDeContainer);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterCargaAgrupadaDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = repositorioCarga.BuscarCargasOriginais(carga.Codigo);

                var cargasRetornar = (
                    from o in cargasOriginais
                    select new
                    {
                        o.Codigo,
                        Filial = new { o.Filial.Codigo, o.Filial.Descricao },
                        Empresa = new { Codigo = o.Empresa?.Codigo ?? 0, Descricao = o.Empresa?.Descricao ?? "" }
                    }
                ).ToList();

                return new JsonpResult(cargasRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterOsDadosDasCargasAgrupadas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarCargaAgrupadaDados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga) ?? throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarCarga);

                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                List<dynamic> cargasAgrupadas = Request.GetListParam<dynamic>("CargasAgrupadas");

                foreach (var cargaAgrupada in cargasAgrupadas)
                {
                    int codigoCargaAgrupada = ((string)cargaAgrupada.Codigo).ToInt();
                    int codigoEmpresa = ((string)cargaAgrupada.Empresa).ToInt();
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupadaAtualizar = repositorioCarga.BuscarPorCodigo(codigoCargaAgrupada, auditavel: true) ?? throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarCarga);

                    if (codigoEmpresa > 0)
                        cargaAgrupadaAtualizar.Empresa = repositorioEmpresa.BuscarPorCodigo(codigoEmpresa) ?? throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarTransportador);
                    else
                        cargaAgrupadaAtualizar.Empresa = null;

                    repositorioCarga.Atualizar(cargaAgrupadaAtualizar, Auditado);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, Localization.Resources.Cargas.Carga.AtualizadoDadosDasCargasAgrupadas, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAtualizarDadosDasCargasAgrupadas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarRetiradaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (carga.LiberadaSemRetiradaContainer)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.CargaLiberadaSemRetiradaContainer);

                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = carga.ModeloVeicularCarga?.ContainerTipo ?? carga.Carregamento?.ModeloVeicularCarga?.ContainerTipo;

                if (containerTipo == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarTipoDoContainer);

                double cpfCnpjLocal = Request.GetDoubleParam("Local");
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente local = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjLocal);

                if (local == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarLocalDeRetirada);

                Repositorio.Embarcador.Pedidos.Container repositorioContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(carga.Codigo);
                int codigoContainer = Request.GetIntParam("Container");

                if (retiradaContainer == null)
                    retiradaContainer = new Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer()
                    {
                        Carga = carga
                    };

                unitOfWork.Start();

                retiradaContainer.Initialize();

                if (retiradaContainer.Codigo != 0 && carga.SituacaoCarga != SituacaoCarga.Nova && (retiradaContainer.Container?.Codigo ?? 0) != 0)
                {
                    Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);

                    if (codigoContainer != 0 && retiradaContainer.Container.Codigo != codigoContainer)
                    {
                        bool status = retiradaContainer.Container?.Status ?? false;

                        retiradaContainer.Container = repositorioContainer.BuscarPorCodigo(codigoContainer);

                        Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
                        {
                            Container = retiradaContainer.Container,
                            coletaContainer = retiradaContainer.ColetaContainer,
                            DataAtualizacao = DateTime.Now
                        };

                        retiradaContainer.Container.Status = status;

                        repositorioContainer.Atualizar(retiradaContainer.Container, Auditado);

                        servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);
                    }
                }
                else
                    retiradaContainer.Container = (codigoContainer > 0) ? repositorioContainer.BuscarPorCodigo(codigoContainer) : null;

                retiradaContainer.ContainerTipo = containerTipo;
                retiradaContainer.Local = local;

                if (retiradaContainer.Codigo == 0)
                    repositorioRetiradaContainer.Inserir(retiradaContainer);
                else
                    repositorioRetiradaContainer.Atualizar(retiradaContainer);

                if (retiradaContainer.IsChanged())
                {
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                    Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);
                }

                if (carga.SituacaoCarga == SituacaoCarga.AgNFe)
                {
                    carga.ProcessandoDocumentosFiscais = true;
                    repositorioCarga.Atualizar(carga);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterRetiradaDeContainer);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarSemRetiradaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                carga.LiberadaSemRetiradaContainer = true;
                if (carga.SituacaoCarga == SituacaoCarga.AgNFe)
                    carga.ProcessandoDocumentosFiscais = true;

                repositorioCarga.Atualizar(carga);

                Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(carga.Codigo);

                if (retiradaContainer != null)
                {
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();

                    repositorioRetiradaContainer.Deletar(retiradaContainer);
                    Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoEmbarcador, TipoServicoMultisoftware);
                }

                new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork).Confirmar(carga, TipoMensagemAlerta.CargaSemInformacaoContainer);//remover mensagem

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, Localization.Resources.Cargas.Carga.CargaLiberadaSemInformarRetiradaDeContainer, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoLiberarSemInformarRetiradaContainer);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCargaVinculadaPreCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string numeroCarga = Request.GetStringParam("NumeroCarga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                bool possuiPreCargaComNumeroCarga = repositorioCarga.BuscarPorCodigoVinculado(numeroCarga) != null;

                return new JsonpResult(new
                {
                    PossuiPreCargaComNumeroCarga = possuiPreCargaComNumeroCarga
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarVinculoComPreCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConfirmarMotoristaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int CodigoCarga = Request.GetIntParam("carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(CodigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork, Auditado);

                servicoMensagemAlerta.Confirmar(carga, TipoMensagemAlerta.CargaSemConfirmacaoMotorista);

                carga.DataLimiteConfirmacaoMotorista = null;
                repositorioCarga.Atualizar(carga);

                return new JsonpResult(true, "");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoGerarImpressaoDaOrdemDeColeta);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarVeiculoMotoristas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoCarga.Importar(linhas, unitOfWork, ClienteAcesso, unitOfWorkAdmin, Auditado);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Nro Carga", Propriedade = "NumeroCarga", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Placa veiculo", Propriedade = "PlacaVeiculo", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "CPF Motorista", Propriedade = "CpfMotorista", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Nome Motorista", Propriedade = "NomeMotorista", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Data de check-out do veículo", Propriedade = "DataCheckoutVeiculo", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Tipo Checkin", Propriedade = "TipoCheckin", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Nro de Eixos no checkin", Propriedade = "NroEixosCheckin", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Placa Reboque", Propriedade = "PlacaReboque", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Numero Veiculo", Propriedade = "NumeroVeiculo", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Tipo de Carga", Propriedade = "TipoCarga", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Modelo Veicular", Propriedade = "ModeloVeicular", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "CNPJ Transportador", Propriedade = "CNPJTransportador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Tipo de Operação", Propriedade = "TipoOperacao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return new JsonpResult(configuracoes.ToList());

        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarObservacaoCarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                carga.Carregamento.Observacao = Request.GetStringParam("Observacao");

                repCarga.Atualizar(carga, Auditado);

                return new JsonpResult(true, "Observação atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar atualizar a observação do carregamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPedidos(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido filtrosPesquisa = ObterFiltroPesquisaPedidos();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.DescricaoPedido, "NumeroPedidoEmbarcador", 12, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Remetente, "Remetente", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Origem, "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destinatario, "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destino, "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.Carregamento, "DataCarregamentoPedido", 10, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = await repositorioPedido.ContarConsultaAsync(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedido = totalRegistros > 0 ? await repositorioPedido.ConsultarAsync(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                var listaPedidoRetornar = (
                   from pedido in listaPedido
                   select new
                   {
                       pedido.Codigo,
                       pedido.NumeroPedidoEmbarcador,
                       Remetente = pedido.Remetente?.Descricao ?? pedido.GrupoPessoas?.Descricao ?? string.Empty,
                       Destinatario = pedido.Destinatario?.Descricao ?? string.Empty,
                       Destino = pedido.Destino?.DescricaoCidadeEstado ?? string.Empty,
                       Origem = pedido.Origem?.DescricaoCidadeEstado ?? string.Empty,
                       DataCarregamentoPedido = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                   }
               ).ToList();

                grid.AdicionaRows(listaPedidoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDetalhesCargaRetiradaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                int codigo = Request.GetIntParam("CodigoTipoContainer");
                double local = Request.GetDoubleParam("CodigoLocal");

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número da Carga", "CargaEmbarcador", tamanho: 26m, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Número EXP", "NumeroEXP", tamanho: 18m, Models.Grid.Align.left, false, false, false, false, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Pedido.DetalheLocalRetiradaContainer> detalhesLocaisRetiradaContainer = repositorioColetaContainer.BuscarDetalhesLocaisRetiradaContainer(local, codigo);

                int totalRegistros = detalhesLocaisRetiradaContainer.Count;

                grid.AdicionaRows(detalhesLocaisRetiradaContainer);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os locais de retirada de container.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracoesIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repositorioIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repositorioIntegracaoIntercab.BuscarIntegracao();

                var dynRetorno = new
                {
                    AtivarNovosFiltrosConsultaCargaIntercab = integracaoIntercab?.AtivarNovosFiltrosConsultaCarga ?? false,
                    AjustarLayoutFiltrosTelaCargaIntercab = integracaoIntercab?.AjustarLayoutFiltrosTelaCarga ?? false,
                    AtivarPreFiltrosTelaCargaIntercab = integracaoIntercab?.AtivarPreFiltrosTelaCarga ?? false,
                    QuantidadeDiasParaDataInicialIntercab = integracaoIntercab?.QuantidadeDiasParaDataInicial ?? 0,
                    SituacoesCargaIntercab = integracaoIntercab?.SituacoesCarga.ToList() ?? null
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarConfiguracaoPadrao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarExternalID()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Carga");
                int numeroExternalID = Request.GetIntParam("NumeroID");

                string externalId = Request.GetStringParam("ExternalID");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Os dados da carga não foram localizados");

                if (numeroExternalID == 1)
                    carga.ExternalID1 = externalId;
                else
                    carga.ExternalID2 = externalId;

                repCarga.Atualizar(carga);

                unitOfWork.CommitChanges();

                var dynRetorno = new
                {
                    ExternalID = numeroExternalID == 1 ? carga.ExternalID1 : carga.ExternalID2,
                    NumeroExternalID = numeroExternalID
                };

                return new JsonpResult(dynRetorno);
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracaoTipoOperacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new Exception(Localization.Resources.Cargas.Carga.CargaNaoEncontrada);

                var dynCarga = new
                {
                    NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica = (carga.TipoOperacao?.ConfiguracaoCarga?.NaoPermitirEditarPesoBrutoDaNotaFiscalEletronica ?? false) ? false : true,
                    BuscarDocumentosEAverbacaoPelaOSMae = (carga.TipoOperacao?.ConfiguracaoCarga?.BuscarDocumentosEAverbacaoPelaOSMae ?? false)
                };

                return new JsonpResult(dynCarga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterLogTrechos()
        {
            // para chamar http://localhost:1736/Carga/ObterLogTrechos?Carga=0001761449
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string codigoCarga = Request.GetStringParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarCargaPorCodigoEmbarcadorProcessarDocumentos(codigoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                List<string> log = new List<string>();

                servicoCarga.ObterTipoTrecho(carga, cargaPedidos, unitOfWork, true, ref log);
                return new JsonpResult(log);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDetalhesAE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Os dados da carga não foram localizados");

                var dynRetorno = new
                {
                    MotivoSituacaoAE = carga.MotivoSituacaoAE ?? string.Empty,
                };

                return new JsonpResult(dynRetorno);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaCargaCarOrganizacao()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true, true, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Motorista, "Motorista", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TracaoCavalo, "DescricaoTracao", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDoReboque, "DescricaoReboque", 10, Models.Grid.Align.left, false, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = ObterFiltrosPesquisaCarga(unitOfWork, false, null);

                int quantidade = repCarga.ContarCargasOrganizacao(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = (quantidade > 0) ? repCarga.BuscarCargasOrganizacao(filtrosPesquisa, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();


                grid.setarQuantidadeTotal(quantidade);

                var dynListaCarga = (from obj in listaCarga
                                     select new
                                     {
                                         obj.Codigo,
                                         CodigoCargaEmbarcador = obj.CodigoCargaEmbarcador,
                                         Transportador = obj.Empresa?.Descricao ?? string.Empty,
                                         Motorista = obj.NomeMotoristas ?? string.Empty,
                                         DescricaoTracao = obj.Veiculo?.Placa ?? string.Empty,
                                         DescricaoReboque = obj.VeiculosVinculados?.ElementAtOrDefault(0)?.Placa ?? string.Empty,
                                     }).ToList();
                grid.AdicionaRows(dynListaCarga);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterCargaOrganizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCargaOrganizacao> cargasOrganizacao = repositorioCarga.BuscarCargaCargasOrganizacao(carga.Codigo);

                return new JsonpResult(new
                {
                    CargasOrganizacao = (
                            from obj in cargasOrganizacao
                            select new
                            {
                                obj.Codigo,
                                CodigoCargaEmbarcador = obj.CargaOrganizacao.CodigoCargaEmbarcador,
                                Transportador = obj.CargaOrganizacao.Empresa?.Descricao ?? string.Empty,
                                Motorista = obj.CargaOrganizacao.NomeMotoristas ?? string.Empty,
                                DescricaoTracao = obj.CargaOrganizacao.Veiculo?.Placa ?? string.Empty,
                                DescricaoReboque = obj.CargaOrganizacao.VeiculosVinculados?.ElementAtOrDefault(0)?.Placa ?? string.Empty,
                            }
                        ).ToList(),
                    PermiteEditar = carga.SituacaoCarga == SituacaoCarga.AgNFe,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterRetiradaDeContainer);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> LiberarSemCargaOrganizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                // Caso houver registros serão deletados.
                Servicos.Embarcador.Carga.CargaOrganizacao.CargaOrganizacao servicoCargaOrganizacao = new Servicos.Embarcador.Carga.CargaOrganizacao.CargaOrganizacao(unitOfWork);
                servicoCargaOrganizacao.RemoverCargaOrganizacao(carga, unitOfWork);

                carga.LiberadoSemCargaOrganizacao = true;

                repositorioCarga.Atualizar(carga);

                new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork).Confirmar(carga, TipoMensagemAlerta.CargaSemInformacaoContainer);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Carga liberada sem Pré Carga", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoLiberarSemInformarRetiradaContainer);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> VincularCargaComCargaOrganizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                List<int> codigosCargaOrganizacao = Request.GetListParam<int>("CodigosCargaOrganizacao");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrganizacao = repositorioCarga.BuscarPorCodigos(codigosCargaOrganizacao);

                if (carga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (cargasOrganizacao.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                Servicos.Embarcador.Carga.CargaOrganizacao.CargaOrganizacao servicoCargaOrganizacao = new Servicos.Embarcador.Carga.CargaOrganizacao.CargaOrganizacao(unitOfWork);

                servicoCargaOrganizacao.RemoverCargaOrganizacao(carga, unitOfWork);

                servicoCargaOrganizacao.CriarCargaOrganizacao(carga, cargasOrganizacao, unitOfWork);

                carga.LiberadoSemCargaOrganizacao = false;

                repositorioCarga.Atualizar(carga);

                new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork).Confirmar(carga, TipoMensagemAlerta.CargaSemInformacaoContainer);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Vinculou Pré Carga", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoLiberarSemInformarRetiradaContainer);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImprimirCheckListMinuta(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.Impressao servicoImpressao = new Servicos.Embarcador.Carga.Impressao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                if (carga == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                byte[] arquivo = servicoImpressao.ObterCheckListMinutaTransporte(carga.Codigo);

                if (arquivo == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerarRelatorio);

                return Arquivo(arquivo, "application/pdf", $"CheckList Minuta de Transporte {carga.CodigoCargaEmbarcador}.pdf");
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
        #endregion

        #region Métodos Privados

        private void ExecutarBusca(ref List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga, ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral, bool consultaParaGeracaoDocumentos = false)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = ObterFiltrosPesquisaCarga(unitOfWork, consultaParaGeracaoDocumentos, configuracaoGeral);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            totalRegistros = repositorioCarga.ContarConsulta(filtrosPesquisa);
            listaCarga = (totalRegistros > 0) ? repositorioCarga.Consultar(filtrosPesquisa, parametroConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
        }
        protected async Task ExecutarBuscaAsync(List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga, int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral, bool consultaParaGeracaoDocumentos, CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = await ObterFiltrosPesquisaCargaAsync(unitOfWork, consultaParaGeracaoDocumentos, configuracaoGeral, cancellation);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellation);

            totalRegistros = await repositorioCarga.ContarConsultaAsync(filtrosPesquisa, cancellation);
            listaCarga = (totalRegistros > 0) ? await repositorioCarga.ConsultarAsync(filtrosPesquisa, parametroConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
        }


        private bool IsPedidoProvisorio(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            return pedido.Provisorio && !configuracaoEmbarcador.TrocarPreCargaPorCarga;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga ObterFiltrosPesquisaCarga(Repositorio.UnitOfWork unitOfWork, bool consultaParaGeracaoDocumentos, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral)
        {
            Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = repositorioOperadorLogistica.BuscarPorUsuario(this.Usuario.Codigo);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
            {
                Serie = Request.GetIntParam("Serie"),
                BuscarCargasRedespacho = Request.GetBoolParam("CargasDoRedespacho"),
                Codigo = Request.GetIntParam("Codigo"),
                CodigoFuncionarioVendedor = Request.GetIntParam("FuncionarioVendedor"),
                CodigoCanalEntrega = Request.GetIntParam("CanalEntrega"),
                CodigoCanalVenda = Request.GetIntParam("CanalVenda"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoCarregamento = Request.GetIntParam("Carregamento"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigosEmpresa = Request.GetListParam<int>("Empresa"),
                CodigosRota = Request.GetListParam<int>("Rota"),
                CodigoGrupoPessoas = Request.GetListParam<int>("GrupoPessoa"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                codigoPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador"),
                CodigoPedidoViagemNavio = Request.GetIntParam("PedidoViagemNavio"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                CpfCnpjExpedidor = Request.GetDoubleParam("Expedidor"),
                CpfCnpjRecebedor = Request.GetDoubleParam("Recebedor"),
                DataFinal = Request.GetNullableDateTimeParam("DataFim"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataCarregamentoFinal = Request.GetNullableDateTimeParam("DataCarregamentoFim"),
                DataCarregamentoInicial = Request.GetNullableDateTimeParam("DataCarregamentoInicio"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                NumeroCTeSubcontratacao = Request.GetIntParam("NumeroCTeSubcontratacao"),
                NumeroMDFe = Request.GetIntParam("NumeroMDFe"),
                NumeroNF = Request.GetIntParam("NumeroNF"),
                NumeroPedidoNFe = Request.GetStringParam("NumeroPedidoNFe"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroPedido = Request.GetIntParam("NumeroPedido"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                NumeroPedidoTrocado = Request.GetStringParam("NumeroPedidoTrocado"),
                NumeroTransporte = Request.GetStringParam("NumeroTransporte"),
                OperadorLogistica = operadorLogistica,
                Ordem = Request.GetStringParam("Ordem"),
                PedidoCentroCusto = Request.GetIntParam("PedidoCentroCusto"),
                Container = Request.GetIntParam("Container") > 0 ? Request.GetIntParam("Container") : Request.GetIntParam("ContainerTMS"),
                PedidoEmpresaResponsavel = Request.GetIntParam("PedidoEmpresaResponsavel"),
                PlacaAgrupamento = Request.GetStringParam("PlacaDeAgrupamento"),
                Situacoes = Request.GetListEnumParam<SituacaoCarga>("Situacoes"),
                SituacoesCargaMercante = Request.GetListEnumParam<SituacaoCargaMercante>("SituacaoCargaMercante"),
                SomenteAgrupadas = Request.GetBoolParam("SomenteAgrupadas"),
                SomenteCargasReentrega = Request.GetBoolParam("SomenteCargasReentrega"),
                SomentePermiteAgrupamento = Request.GetBoolParam("SomentePermiteAgrupamento"),
                SomentePermiteMDFeManual = Request.GetBoolParam("SomentePermiteMDFeManual"),
                SomenteTerceiros = Request.GetBoolParam("SomenteTerceiros"),
                ExibirCargasNaoFechadas = Request.GetBoolParam("ExibirCargasNaoFechadas"),
                TipoContratacaoCarga = Request.GetNullableEnumParam<TipoContratacaoCarga>("TipoContratacaoCarga"),
                TipoOperacaoCargaCTeManual = Request.GetEnumParam<TipoOperacaoCargaCTeManual>("TipoOperacaoCargaCTeManual"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                TipoCobrancaMultimodal = Request.GetListEnumParam<TipoCobrancaMultimodal>("TipoCobrancaMultimodal"),
                DataInicialEmissao = Request.GetNullableDateTimeParam("DataInicioEmissao"),
                DataFinalEmissao = Request.GetNullableDateTimeParam("DataFimEmissao"),
                PortoOrigem = Request.GetIntParam("PortoOrigem"),
                PortoDestino = Request.GetIntParam("PortoDestino"),
                CargaPerigosa = Request.GetBoolParam("CargaPerigosa"),
                DataInicioAverbacao = Request.GetNullableDateTimeParam("DataInicioAverbacao"),
                DataFimAverbacao = Request.GetNullableDateTimeParam("DataFimAverbacao"),
                RaizCNPJ = Utilidades.String.OnlyNumbers(Request.GetStringParam("RaizCNPJ")),
                DeliveryTerm = Request.GetStringParam("DeliveryTerm"),
                IdAutorizacao = Request.GetStringParam("IdAutorizacao"),
                DataInclusaoPCPInicial = Request.GetNullableDateTimeParam("DataInclusaoPCPInicial"),
                DataInclusaoPCPLimite = Request.GetNullableDateTimeParam("DataInclusaoPCPLimite"),
                DataInclusaoBookingInicial = Request.GetNullableDateTimeParam("DataInclusaoBookingInicial"),
                DataInclusaoBookingLimite = Request.GetNullableDateTimeParam("DataInclusaoBookingLimite"),
                TiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null,
                SiglaEstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                SiglaEstadoDestino = Request.GetStringParam("EstadoDestino"),
                CargaTrocaDeNota = Request.GetBoolParam("CargaTrocaDeNota"),
                ConsultaParaGeracaoDocumentos = consultaParaGeracaoDocumentos,
                HabilitarHoraFiltroDataInicialFinalRelatorioCargas = ConfiguracaoEmbarcador.HabilitarHoraFiltroDataInicialFinalRelatorioCargas,
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                ObservacaoCTe = Request.GetStringParam("ObservacaoCTe"),
                CpfCnpjTransportadorTerceiro = Request.GetDoubleParam("TransportadorTerceiro"),
                RotaEmbarcador = Request.GetStringParam("RotaEmbarcador"),
                CodigosModeloDocumentoFiscal = Request.GetListParam<int>("ModeloDocumentoFiscal"),
                NumeroOe = Request.GetStringParam("NumeroOe"),
                CodigoPedidoCliente = Request.GetStringParam("CodigoPedidoCliente"),
                RetornarCargaDocumentoEmitido = Request.GetBoolParam("RetornarCargaDocumentoEmitido"),
                CargasAguardandoImportacaoCTe = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNao>("CargasAguardandoImportacaoCTe"),
                CodigoOperadorInsercao = Request.GetIntParam("OperadorInsercao"),
                CodigoCIOT = Request.GetIntParam("CIOT"),
                SomenteCargasComValePedagio = Request.GetBoolParam("SomenteCargasComValePedagio"),
                PossuiPendencia = Request.GetNullableBoolParam("PossuiPendencia"),
                CodigoZonaTransporte = Request.GetIntParam("ZonaDeTransporte"),
                CargaRelacionadas = Request.GetBoolParam("CargaRelacionadas"),
                SomenteCargasComFaturaFake = Request.GetBoolParam("SomenteCargasComFacturaFake"),
                SomenteCargasComDocumentoOriginarioVinculado = Request.GetBoolParam("SomenteCargasComDocumentoOriginarioVinculado"),
                CargaTrechoSumarizada = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaTrechoSumarizada>("CargaTrechoSumarizada"),
                SituacaoCarga = Request.GetEnumParam<SituacaoCarga>("Situacao"),
                CodigosEmpresas = (this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null ? this.Usuario.Empresas.Select(c => c.Codigo).ToList() : null,
                CodigoCarga = Request.GetIntParam("CodigoCarga"),
                NumeroOT = Request.GetStringParam("NumeroOT"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                FormaIntegracaoNotas = Request.GetEnumParam<FormaIntegracao>("FormaIntegracaoNotas"),
                CategoriaOS = Request.GetListEnumParam<CategoriaOS>("CategoriaOS"),
                TipoOSConvertido = Request.GetListEnumParam<TipoOSConvertido>("TipoOSConvertido"),
                TipoOS = Request.GetListEnumParam<TipoOS>("TipoOS"),
                DirecionamentoCustoExtra = Request.GetListEnumParam<TipoDirecionamentoCustoExtra>("DirecionamentoCustoExtra"),
                StatusCustoExtra = Request.GetListEnumParam<StatusCustoExtra>("StatusCustoExtra"),
                UsuarioUtilizaSegregacaoPorProvedor = Usuario.UsuarioUtilizaSegregacaoPorProvedor,
                CodigosProvedores = Usuario.UsuarioUtilizaSegregacaoPorProvedor ? Usuario.ClientesProvedores.Select(o => o.CPF_CNPJ).ToList() : new List<double>(),
                SomenteCargasNaoValidadasNaGR = Request.GetBoolParam("SomenteCargasNaoValidadasNaGR"),
                SomenteCargasCriticas = Request.GetBoolParam("SomenteCargasCriticas"),
                SomenteCargasSemCIOT = Request.GetBoolParam("SomenteCargasSemCIOT"),
                SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual = (configuracaoGeralCarga?.SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual ?? false),
                PossuiValePedagio = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNao>("PossuiValePedagio", Dominio.Enumeradores.OpcaoSimNao.Todos),
                PreCarga = Request.GetStringParam("PreCarga"),
                NumeroPreCarga = Request.GetStringParam("NumeroPreCarga"),
                PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados = configuracaoGeralCarga?.PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados ?? false,
                NumeroContainerVeiculo = Request.GetStringParam("NumeroContainerVeiculo"),
            };

            if (filtrosPesquisa.CodigoGrupoPessoas.Count() == 0)
            {
                int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoa");
                if (codigoGrupoPessoa > 0)
                    filtrosPesquisa.CodigoGrupoPessoas.Add(codigoGrupoPessoa);
            }

            if (Request.GetIntParam("Empresa") > 0)
                filtrosPesquisa.CodigosEmpresa.Add(Request.GetIntParam("Empresa"));
            if (Request.GetIntParam("Veiculo") > 0)
                filtrosPesquisa.CodigoVeiculo = Request.GetIntParam("Veiculo");
            else
                filtrosPesquisa.CodigosVeiculos = Request.GetListParam<int>("Veiculo");

            if (filtrosPesquisa.Situacoes.Count > 0)
                filtrosPesquisa.SomenteSituacao = true;
            else
            {
                filtrosPesquisa.SomenteSituacao = false;
            }

            if (this.Usuario.AssociarUsuarioCliente && this.Usuario.Cliente != null)
                filtrosPesquisa.CnpjClienteUsuario = this.Usuario.Cliente.CPF_CNPJ;

            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosFilialVenda = Request.GetListParam<int>("FilialVenda");
            List<int> codigosTiposOperacao = Request.GetListParam<int>("TipoOperacao");

            if (codigoTipoOperacao > 0 && codigosTiposOperacao.Count <= 0)
                codigosTiposOperacao.Add(codigoTipoOperacao);

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosFilialVenda = codigosFilialVenda.Count == 0 ? ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork) : codigosFilialVenda;
            filtrosPesquisa.CodigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga };
            filtrosPesquisa.CodigosTipoOperacao = codigosTiposOperacao.Count() <= 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : codigosTiposOperacao;
            filtrosPesquisa.CpfCnpjRemetentesOuDestinatarios = ObterListaCnpjCpfClientePermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CpfCnpjRecebedoresOuSemRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);

            filtrosPesquisa.CpfCnpjEmpresasConsulta = new List<double>();
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                filtrosPesquisa.CodigoEmpresa = this.Empresa.Codigo;

                //devemos buscar os cnpjs das empresas onde essa empresa é o armazem responsavel.
                Dominio.Entidades.Empresa empresaCTe = repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa);
                filtrosPesquisa.CpfCnpjEmpresasConsulta.Add(double.Parse(empresaCTe.CNPJ));

                if (empresaCTe.Filiais != null)
                    filtrosPesquisa.CpfCnpjEmpresasConsulta.AddRange(empresaCTe.Filiais.Select(x => double.Parse(x.CNPJ)).ToList());

                filtrosPesquisa.CpfCnpjEmpresasConsulta.AddRange(repCliente.BuscarCpfCnpjClienteArmazemResponsavel(double.Parse(empresaCTe.CNPJ)));
            }
            else if (TipoServicoMultisoftware == TipoServicoMultisoftware.TransportadorTerceiro)
            {
                filtrosPesquisa.CpfCnpjTransportadorTerceiro = this.Usuario?.ClienteTerceiro?.CPF_CNPJ ?? 99999;
            }

            if (TipoServicoMultisoftware != TipoServicoMultisoftware.MultiCTe && TipoServicoMultisoftware != TipoServicoMultisoftware.TransportadorTerceiro)
            {
                if ((filtrosPesquisa.CodigoOperador == 0) && !operadorLogistica.SupervisorLogistica)
                    filtrosPesquisa.CodigoOperador = this.Usuario.Codigo;
            }

            filtrosPesquisa.Regiao = Request.GetListParam<int>("Regiao");
            filtrosPesquisa.Mesorregiao = Request.GetListParam<int>("Mesorregiao");

            return filtrosPesquisa;
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga> ObterFiltrosPesquisaCargaAsync(Repositorio.UnitOfWork unitOfWork, bool consultaParaGeracaoDocumentos,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral, CancellationToken cancellation)
        {
            Repositorio.Embarcador.Operacional.OperadorLogistica repositorioOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork, cancellation);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork, cancellation).BuscarPrimeiroRegistroAsync();
            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = await repositorioOperadorLogistica.BuscarPorUsuarioAsync(this.Usuario.Codigo);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellation);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellation);

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
            {
                Serie = Request.GetIntParam("Serie"),
                BuscarCargasRedespacho = Request.GetBoolParam("CargasDoRedespacho"),
                Codigo = Request.GetIntParam("Codigo"),
                CodigoFuncionarioVendedor = Request.GetIntParam("FuncionarioVendedor"),
                CodigoCanalEntrega = Request.GetIntParam("CanalEntrega"),
                CodigoCanalVenda = Request.GetIntParam("CanalVenda"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoCarregamento = Request.GetIntParam("Carregamento"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigosEmpresa = Request.GetListParam<int>("Empresa"),
                CodigosRota = Request.GetListParam<int>("Rota"),
                CodigoGrupoPessoas = Request.GetListParam<int>("GrupoPessoa"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                codigoPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador"),
                CodigoPedidoViagemNavio = Request.GetIntParam("PedidoViagemNavio"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                CpfCnpjExpedidor = Request.GetDoubleParam("Expedidor"),
                CpfCnpjRecebedor = Request.GetDoubleParam("Recebedor"),
                DataFinal = Request.GetNullableDateTimeParam("DataFim"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataCarregamentoFinal = Request.GetNullableDateTimeParam("DataCarregamentoFim"),
                DataCarregamentoInicial = Request.GetNullableDateTimeParam("DataCarregamentoInicio"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                NumeroCTeSubcontratacao = Request.GetIntParam("NumeroCTeSubcontratacao"),
                NumeroMDFe = Request.GetIntParam("NumeroMDFe"),
                NumeroNF = Request.GetIntParam("NumeroNF"),
                NumeroPedidoNFe = Request.GetStringParam("NumeroPedidoNFe"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroPedido = Request.GetIntParam("NumeroPedido"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                NumeroPedidoTrocado = Request.GetStringParam("NumeroPedidoTrocado"),
                NumeroTransporte = Request.GetStringParam("NumeroTransporte"),
                OperadorLogistica = operadorLogistica,
                Ordem = Request.GetStringParam("Ordem"),
                PedidoCentroCusto = Request.GetIntParam("PedidoCentroCusto"),
                Container = Request.GetIntParam("Container") > 0 ? Request.GetIntParam("Container") : Request.GetIntParam("ContainerTMS"),
                PedidoEmpresaResponsavel = Request.GetIntParam("PedidoEmpresaResponsavel"),
                PlacaAgrupamento = Request.GetStringParam("PlacaDeAgrupamento"),
                Situacoes = Request.GetListEnumParam<SituacaoCarga>("Situacoes"),
                SituacoesCargaMercante = Request.GetListEnumParam<SituacaoCargaMercante>("SituacaoCargaMercante"),
                SomenteAgrupadas = Request.GetBoolParam("SomenteAgrupadas"),
                SomenteCargasReentrega = Request.GetBoolParam("SomenteCargasReentrega"),
                SomentePermiteAgrupamento = Request.GetBoolParam("SomentePermiteAgrupamento"),
                SomentePermiteMDFeManual = Request.GetBoolParam("SomentePermiteMDFeManual"),
                SomenteTerceiros = Request.GetBoolParam("SomenteTerceiros"),
                ExibirCargasNaoFechadas = Request.GetBoolParam("ExibirCargasNaoFechadas"),
                TipoContratacaoCarga = Request.GetNullableEnumParam<TipoContratacaoCarga>("TipoContratacaoCarga"),
                TipoOperacaoCargaCTeManual = Request.GetEnumParam<TipoOperacaoCargaCTeManual>("TipoOperacaoCargaCTeManual"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                TipoCobrancaMultimodal = Request.GetListEnumParam<TipoCobrancaMultimodal>("TipoCobrancaMultimodal"),
                DataInicialEmissao = Request.GetNullableDateTimeParam("DataInicioEmissao"),
                DataFinalEmissao = Request.GetNullableDateTimeParam("DataFimEmissao"),
                PortoOrigem = Request.GetIntParam("PortoOrigem"),
                PortoDestino = Request.GetIntParam("PortoDestino"),
                CargaPerigosa = Request.GetBoolParam("CargaPerigosa"),
                DataInicioAverbacao = Request.GetNullableDateTimeParam("DataInicioAverbacao"),
                DataFimAverbacao = Request.GetNullableDateTimeParam("DataFimAverbacao"),
                RaizCNPJ = Utilidades.String.OnlyNumbers(Request.GetStringParam("RaizCNPJ")),
                DeliveryTerm = Request.GetStringParam("DeliveryTerm"),
                IdAutorizacao = Request.GetStringParam("IdAutorizacao"),
                DataInclusaoPCPInicial = Request.GetNullableDateTimeParam("DataInclusaoPCPInicial"),
                DataInclusaoPCPLimite = Request.GetNullableDateTimeParam("DataInclusaoPCPLimite"),
                DataInclusaoBookingInicial = Request.GetNullableDateTimeParam("DataInclusaoBookingInicial"),
                DataInclusaoBookingLimite = Request.GetNullableDateTimeParam("DataInclusaoBookingLimite"),
                TiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null,
                SiglaEstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                SiglaEstadoDestino = Request.GetStringParam("EstadoDestino"),
                CargaTrocaDeNota = Request.GetBoolParam("CargaTrocaDeNota"),
                ConsultaParaGeracaoDocumentos = consultaParaGeracaoDocumentos,
                HabilitarHoraFiltroDataInicialFinalRelatorioCargas = ConfiguracaoEmbarcador.HabilitarHoraFiltroDataInicialFinalRelatorioCargas,
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                ObservacaoCTe = Request.GetStringParam("ObservacaoCTe"),
                CpfCnpjTransportadorTerceiro = Request.GetDoubleParam("TransportadorTerceiro"),
                RotaEmbarcador = Request.GetStringParam("RotaEmbarcador"),
                CodigosModeloDocumentoFiscal = Request.GetListParam<int>("ModeloDocumentoFiscal"),
                NumeroOe = Request.GetStringParam("NumeroOe"),
                CodigoPedidoCliente = Request.GetStringParam("CodigoPedidoCliente"),
                RetornarCargaDocumentoEmitido = Request.GetBoolParam("RetornarCargaDocumentoEmitido"),
                CargasAguardandoImportacaoCTe = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNao>("CargasAguardandoImportacaoCTe"),
                CodigoOperadorInsercao = Request.GetIntParam("OperadorInsercao"),
                CodigoCIOT = Request.GetIntParam("CIOT"),
                SomenteCargasComValePedagio = Request.GetBoolParam("SomenteCargasComValePedagio"),
                PossuiPendencia = Request.GetNullableBoolParam("PossuiPendencia"),
                CodigoZonaTransporte = Request.GetIntParam("ZonaDeTransporte"),
                CargaRelacionadas = Request.GetBoolParam("CargaRelacionadas"),
                SomenteCargasComFaturaFake = Request.GetBoolParam("SomenteCargasComFacturaFake"),
                SomenteCargasComDocumentoOriginarioVinculado = Request.GetBoolParam("SomenteCargasComDocumentoOriginarioVinculado"),
                CargaTrechoSumarizada = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaTrechoSumarizada>("CargaTrechoSumarizada"),
                SituacaoCarga = Request.GetEnumParam<SituacaoCarga>("Situacao"),
                CodigosEmpresas = (this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null ? this.Usuario.Empresas.Select(c => c.Codigo).ToList() : null,
                CodigoCarga = Request.GetIntParam("CodigoCarga"),
                NumeroOT = Request.GetStringParam("NumeroOT"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                FormaIntegracaoNotas = Request.GetEnumParam<FormaIntegracao>("FormaIntegracaoNotas"),
                CategoriaOS = Request.GetListEnumParam<CategoriaOS>("CategoriaOS"),
                TipoOSConvertido = Request.GetListEnumParam<TipoOSConvertido>("TipoOSConvertido"),
                TipoOS = Request.GetListEnumParam<TipoOS>("TipoOS"),
                DirecionamentoCustoExtra = Request.GetListEnumParam<TipoDirecionamentoCustoExtra>("DirecionamentoCustoExtra"),
                StatusCustoExtra = Request.GetListEnumParam<StatusCustoExtra>("StatusCustoExtra"),
                UsuarioUtilizaSegregacaoPorProvedor = Usuario.UsuarioUtilizaSegregacaoPorProvedor,
                CodigosProvedores = Usuario.UsuarioUtilizaSegregacaoPorProvedor ? Usuario.ClientesProvedores.Select(o => o.CPF_CNPJ).ToList() : new List<double>(),
                SomenteCargasNaoValidadasNaGR = Request.GetBoolParam("SomenteCargasNaoValidadasNaGR"),
                SomenteCargasCriticas = Request.GetBoolParam("SomenteCargasCriticas"),
                SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual = (configuracaoGeralCarga?.SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual ?? false),
                PossuiValePedagio = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNao>("PossuiValePedagio", Dominio.Enumeradores.OpcaoSimNao.Todos),
                PreCarga = Request.GetStringParam("PreCarga"),
                NumeroPreCarga = Request.GetStringParam("NumeroPreCarga"),
                PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados = configuracaoGeralCarga?.PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados ?? false,
            };

            if (filtrosPesquisa.CodigoGrupoPessoas.Count() == 0)
            {
                int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoa");
                if (codigoGrupoPessoa > 0)
                    filtrosPesquisa.CodigoGrupoPessoas.Add(codigoGrupoPessoa);
            }

            if (Request.GetIntParam("Empresa") > 0)
                filtrosPesquisa.CodigosEmpresa.Add(Request.GetIntParam("Empresa"));
            if (Request.GetIntParam("Veiculo") > 0)
                filtrosPesquisa.CodigoVeiculo = Request.GetIntParam("Veiculo");
            else
                filtrosPesquisa.CodigosVeiculos = Request.GetListParam<int>("Veiculo");

            if (filtrosPesquisa.Situacoes.Count > 0)
                filtrosPesquisa.SomenteSituacao = true;
            else
            {
                filtrosPesquisa.SomenteSituacao = false;
            }

            if (this.Usuario.AssociarUsuarioCliente && this.Usuario.Cliente != null)
                filtrosPesquisa.CnpjClienteUsuario = this.Usuario.Cliente.CPF_CNPJ;

            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            List<int> codigosFilialVenda = Request.GetListParam<int>("FilialVenda");
            List<int> codigosTiposOperacao = Request.GetListParam<int>("TipoOperacao");

            if (codigoTipoOperacao > 0 && codigosTiposOperacao.Count <= 0)
                codigosTiposOperacao.Add(codigoTipoOperacao);

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? await ObterListaCodigoFilialPermitidasOperadorLogisticaAsync(unitOfWork, cancellation) : codigosFilial;
            filtrosPesquisa.CodigosFilialVenda = codigosFilialVenda.Count == 0 ? await ObterListaCodigoFilialVendaPermitidasOperadorLogisticaAsync(unitOfWork, cancellation) : codigosFilialVenda;
            filtrosPesquisa.CodigosTipoCarga = codigoTipoCarga == 0 ? await ObterListaCodigoTipoCargaPermitidosOperadorLogisticaAsync(unitOfWork, cancellation) : new List<int>() { codigoTipoCarga };
            filtrosPesquisa.CodigosTipoOperacao = codigosTiposOperacao.Count() <= 0 ? await ObterListaCodigoTipoOperacaoPermitidosOperadorLogisticaAsync(unitOfWork, cancellation) : codigosTiposOperacao;
            filtrosPesquisa.CpfCnpjRemetentesOuDestinatarios = await ObterListaCnpjCpfClientePermitidosOperadorLogisticaAsync(unitOfWork, cancellation);
            filtrosPesquisa.CpfCnpjRecebedoresOuSemRecebedores = await ObterListaCnpjCpfRecebedorPermitidosOperadorLogisticaAsync(unitOfWork, cancellation);

            filtrosPesquisa.CpfCnpjEmpresasConsulta = new List<double>();
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                filtrosPesquisa.CodigoEmpresa = this.Empresa.Codigo;

                //devemos buscar os cnpjs das empresas onde essa empresa é o armazem responsavel.
                Dominio.Entidades.Empresa empresaCTe = await repEmpresa.BuscarPorCodigoAsync(filtrosPesquisa.CodigoEmpresa);
                filtrosPesquisa.CpfCnpjEmpresasConsulta.Add(double.Parse(empresaCTe.CNPJ));

                if (empresaCTe.Filiais != null)
                    filtrosPesquisa.CpfCnpjEmpresasConsulta.AddRange(empresaCTe.Filiais.Select(x => double.Parse(x.CNPJ)).ToList());

                filtrosPesquisa.CpfCnpjEmpresasConsulta.AddRange(await repCliente.BuscarCpfCnpjClienteArmazemResponsavelAsync(double.Parse(empresaCTe.CNPJ)));
            }
            else if (TipoServicoMultisoftware == TipoServicoMultisoftware.TransportadorTerceiro)
            {
                filtrosPesquisa.CpfCnpjTransportadorTerceiro = this.Usuario?.ClienteTerceiro?.CPF_CNPJ ?? 99999;
            }

            if (TipoServicoMultisoftware != TipoServicoMultisoftware.MultiCTe && TipoServicoMultisoftware != TipoServicoMultisoftware.TransportadorTerceiro)
            {
                if ((filtrosPesquisa.CodigoOperador == 0) && !operadorLogistica.SupervisorLogistica)
                    filtrosPesquisa.CodigoOperador = this.Usuario.Codigo;
            }

            filtrosPesquisa.Regiao = Request.GetListParam<int>("Regiao");
            filtrosPesquisa.Mesorregiao = Request.GetListParam<int>("Mesorregiao");

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga ObterFiltrosPesquisaCargaPorSituacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral)
        {
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoFilialVenda = Request.GetIntParam("FilialVenda");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumento");

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
            {
                Codigo = Request.GetIntParam("Codigo"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigosEmpresa = Request.GetListParam<int>("Empresa"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                codigoPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador"),
                CodigoPedidoViagemNavio = Request.GetIntParam("PedidoViagemNavio"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                NumeroMDFe = Request.GetIntParam("NumeroMDFe"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                PedidoCentroCusto = Request.GetIntParam("PedidoCentroCusto"),
                Container = Request.GetIntParam("Container"),
                PedidoEmpresaResponsavel = Request.GetIntParam("PedidoEmpresaResponsavel"),
                Situacoes = Request.GetListEnumParam<SituacaoCarga>("Situacao"),
                NumeroNF = Request.GetIntParam("NumeroNF"),
                ApenasMDFeEncerrados = Request.GetBoolParam("ApenasMDFeEncerrados"),
                CargasParaEncerramento = Request.GetBoolParam("CargasParaEncerramento"),
                SomenteSituacao = true,
                TipoOperacaoCargaCTeManual = TipoOperacaoCargaCTeManual.NovaCarga,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                TiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null,
                CodigosEmpresas = (this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null ? this.Usuario.Empresas.Select(c => c.Codigo).ToList() : null
            };

            if (codigoModeloDocumentoFiscal > 0)
                filtrosPesquisa.CodigosModeloDocumentoFiscal = new List<int>() { codigoModeloDocumentoFiscal };

            filtrosPesquisa.CodigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial };
            filtrosPesquisa.CodigosFilialVenda = codigoFilialVenda == 0 ? ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilialVenda };
            filtrosPesquisa.CodigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga };
            filtrosPesquisa.CodigosTipoOperacao = codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };
            filtrosPesquisa.CpfCnpjRemetentesOuDestinatarios = ObterListaCnpjCpfClientePermitidosOperadorLogistica(unitOfWork);

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga ObterFiltrosPesquisaCargaParaEncaixeDeSubcontratacao(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
            {
                ApenasEmpresaPermiteEncaixe = true,
                Codigo = Request.GetIntParam("Codigo"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                codigoPedidoEmbarcador = Request.GetStringParam("CodigoPedidoEmbarcador"),
                CodigoPedidoViagemNavio = Request.GetIntParam("PedidoViagemNavio"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                PedidoCentroCusto = Request.GetIntParam("PedidoCentroCusto"),
                Container = Request.GetIntParam("Container"),
                PedidoEmpresaResponsavel = Request.GetIntParam("PedidoEmpresaResponsavel"),
                SituacaoCarga = SituacaoCarga.Todas,
                Situacoes = new List<SituacaoCarga>() { SituacaoCarga.EmTransporte, SituacaoCarga.Encerrada, SituacaoCarga.AgImpressaoDocumentos },
                SomenteSituacao = true,
                TipoOperacaoCargaCTeManual = TipoOperacaoCargaCTeManual.NovaCarga,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                TiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null
            };

            filtrosPesquisa.CodigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial };
            filtrosPesquisa.CodigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga };
            filtrosPesquisa.CodigosTipoOperacao = codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga ObterFiltrosPesquisaCargaOrganizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarga()
            {
                CodigosOrigem = (from cargaPedido in carga.Pedidos select cargaPedido.Origem.Codigo).ToList(),
                CodigosDestino = (from cargaPedido in carga.Pedidos select cargaPedido.Destino.Codigo).ToList(),
                CpfCnpjRemetentes = (from cargaPedido in carga.Pedidos select cargaPedido.Pedido.Remetente.CPF_CNPJ).ToList(),
                CpfCnpjDestinatarios = (from cargaPedido in carga.Pedidos select cargaPedido.Pedido.Destinatario.CPF_CNPJ).ToList()
            };

            return filtrosPesquisa;
        }

        private dynamic ObterCargaPedidoParaEncaixeDeSubcontratacao(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> xmlNotasFiscais)
        {
            string descricaoDestinatario = cargaPedido.Pedido.Destinatario?.Descricao ?? "";
            string numeroNotasFiscais = string.Join(",", (from o in xmlNotasFiscais where o.CargaPedido.Codigo == cargaPedido.Codigo select o.XMLNotaFiscal.Numero));

            return new
            {
                cargaPedido.Codigo,
                cargaPedido.Carga.CodigoCargaEmbarcador,
                CodigoPedidoEmbarcador = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                Destino = cargaPedido.Destino?.DescricaoCidadeEstado ?? "",
                Descricao = $"{cargaPedido.Pedido.NumeroPedidoEmbarcador}{(string.IsNullOrWhiteSpace(descricaoDestinatario) ? "" : $" - {descricaoDestinatario}")}{(string.IsNullOrWhiteSpace(numeroNotasFiscais) ? "" : $" (Notas: {numeroNotasFiscais})")}",
                Destinatario = cargaPedido.Pedido.Destinatario?.Descricao ?? "",
                NotasFiscais = numeroNotasFiscais,
                Reentrega = (cargaPedido.Pedido.ReentregaSolicitada ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao)
            };
        }

        private dynamic CriarObjetoDetalheOutroEndereco(Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco outroEndereco)
        {
            string endereco = "";
            if (outroEndereco != null)
            {
                endereco = outroEndereco.Endereco ?? "";

                if (!string.IsNullOrWhiteSpace(outroEndereco.Numero))
                    endereco += ", " + outroEndereco.Numero;

                if (!string.IsNullOrWhiteSpace(outroEndereco.Complemento))
                    endereco += " (" + outroEndereco.Complemento + ")";

                if (!string.IsNullOrWhiteSpace(outroEndereco.Bairro))
                    endereco += " - " + outroEndereco.Bairro;

                if (outroEndereco.CEP != "")
                    endereco += ", CEP: " + outroEndereco.CEP;

                if (outroEndereco.Localidade != null)
                    endereco += " - " + outroEndereco.Localidade.DescricaoCidadeEstado;

                if (!string.IsNullOrWhiteSpace(outroEndereco.CodigoEmbarcador))
                    endereco += ". (Código Endereço: " + outroEndereco.CodigoEmbarcador + ")";
            }

            return endereco;
        }

        private void CriarPedidoHistoricoAlteracaoData(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, TipoDataPedido tipoDataPedido, ResponsavelAlteracaoDataPedido responsavel, string observacao, DateTime? dataAnteriorAlterada, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.Embarcador.Pedidos.PedidoHistoricoAlteracaoData repPedidoHistoricoAlteracaoData)
        {
            if (configuracao.NecessarioInformarJustificativaAoAlterarDataSaidaOuPrevisaoEntregaPedidoNaCarga && dataAnteriorAlterada != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData pedidoHistoricoAlteracaoData = new Dominio.Entidades.Embarcador.Pedidos.PedidoHistoricoAlteracaoData
                {
                    DataAlteracao = DateTime.Now,
                    Usuario = Usuario,
                    Pedido = pedido,
                    TipoData = tipoDataPedido,
                    DataAnterior = dataAnteriorAlterada.Value,
                    Responsavel = responsavel,
                    Observacao = observacao
                };
                repPedidoHistoricoAlteracaoData.Inserir(pedidoHistoricoAlteracaoData);
            }
        }

        private void AlterarDataEntregaControleEntregadoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, DateTime? dataAlterada, Dominio.Entidades.Embarcador.Cargas.Carga Carga)
        {
            if (dataAlterada != null && pedido != null & Carga != null)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(Carga.Codigo, pedido.Codigo);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = repositorioCargaEntregaPedido.BuscarPorCargaPedido(cargaPedido.Codigo);
                if (cargaEntregaPedido != null)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    cargaEntregaPedido.CargaEntrega.DataPrevista = dataAlterada;
                    repEntrega.Atualizar(cargaEntregaPedido.CargaEntrega, Auditado);
                }
            }
        }

        private void AtualizarHorarioOcorrenciasProvisionadas(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            var ocorrencias = repCargaOcorrencia.BuscarProvisionadasPorCarga(carga.Codigo);

            foreach (var ocorrencia in ocorrencias)
            {
                if (carga.DataCarregamentoCarga.HasValue)
                {
                    ocorrencia.ParametroDataInicial = carga.DataCarregamentoCarga.Value;
                    repCargaOcorrencia.Atualizar(ocorrencia);
                }
            }
        }

        private void ValidarAlteracoesPedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosAdicionados, Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoRemovido, Repositorio.UnitOfWork unitOfWork)
        {
            if (!(carga.TipoOperacao?.ConfiguracaoCarga?.AlertarAlteracoesPedidoNoFluxoPatio ?? false))
                return;

            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);

            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.EtapaAtual == 0) || fluxoGestaoPatio.DataFinalizacaoFluxo.HasValue)
                return;

            if (!Request.GetBoolParam("PermitirAlteracoesPedidos"))
            {
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                string descricaoEtapa = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(fluxoGestaoPatio).Descricao;

                throw new ControllerException($"O fluxo de pátio já está na etapa {descricaoEtapa}.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.AlteracoesPedidosNaoConfirmada);
            }

            Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio servicoMensagemAlerta = new Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio(unitOfWork);

            if (pedidosAdicionados?.Count > 0)
            {
                if (pedidosAdicionados.Count == 1)
                    servicoMensagemAlerta.Adicionar(fluxoGestaoPatio, TipoMensagemAlerta.AlteracaoPedidos, $"O pedido {pedidosAdicionados.FirstOrDefault().NumeroPedidoEmbarcador} foi adicionado na carga");
                else
                    servicoMensagemAlerta.Adicionar(fluxoGestaoPatio, TipoMensagemAlerta.AlteracaoPedidos, $"Os pedidos {string.Join(", ", pedidosAdicionados)} foram adicionados na carga");
            }
            else
                servicoMensagemAlerta.Adicionar(fluxoGestaoPatio, TipoMensagemAlerta.AlteracaoPedidos, $"O pedido {pedidoRemovido.NumeroPedidoEmbarcador} foi removido da carga");
        }

        private void ValidarSeparacaoMercadoria(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool permitirSeparacaoMercadoriaInformada, Repositorio.UnitOfWork unitOfWork)
        {
            if (permitirSeparacaoMercadoriaInformada)
                return;

            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);

            if ((fluxoGestaoPatio == null) || fluxoGestaoPatio.DataFinalizacaoFluxo.HasValue)
                return;

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas repositorioFluxoGestaoPatioEtapas = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas fluxoGestaoPatioEtapa = repositorioFluxoGestaoPatioEtapas.BuscarPorGestaoEEtapa(fluxoGestaoPatio.Codigo, EtapaFluxoGestaoPatio.SeparacaoMercadoria);

            if (fluxoGestaoPatioEtapa == null)
                return;

            if (!fluxoGestaoPatioEtapa.EtapaLiberada)
                return;

            if (fluxoGestaoPatioEtapa.EtapaFluxoGestaoPatio != fluxoGestaoPatioEtapa.FluxoGestaoPatio.EtapaFluxoGestaoPatioAtual)
                throw new ControllerException(Localization.Resources.Cargas.Carga.SeparacaoMercadoriaInformadaFluxoPtio, Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.SeparacaoMercadoriaInformada);

            fluxoGestaoPatioEtapa.ExibirAlerta = true;
            repositorioFluxoGestaoPatioEtapas.Atualizar(fluxoGestaoPatioEtapa);
        }

        private string ObterZonaTransporteCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaZonaTransporte repcargaZonaTransporte = new Repositorio.Embarcador.Cargas.CargaZonaTransporte(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte> cargaZonaTransporte = repcargaZonaTransporte.Consultar(carga.Codigo, null);

            if (cargaZonaTransporte != null && cargaZonaTransporte.Count > 0)
                return cargaZonaTransporte.FirstOrDefault().ZonaTransporte.Descricao;
            else
                return "";
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido ObterFiltroPesquisaPedidos()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedido
            {
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedido"),
            };
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarListaCargaPedidoOrdenada(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            bool ordernarPorStage = false;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                if (cargaPedido.Pedido?.PossuiStage ?? false)
                    ordernarPorStage = true;

            if (ordernarPorStage)
                cargaPedidos = cargaPedidos.OrderBy(o => o.Pedido?.StagesPedido?.Count() > 0 ? o.Pedido?.StagesPedido?.FirstOrDefault().Stage?.NumeroStage ?? string.Empty : string.Empty).ToList();

            return cargaPedidos;
        }

        private string PedidosCodigosRemetentesDestinatarios(List<Dominio.Entidades.Cliente> clientes)
        {
            if (clientes == null)
                return "[]";

            List<double> codigos = (from p in clientes select p.CPF_CNPJ).Distinct().ToList();

            return "[" + String.Join(", ", codigos) + "]";
        }

        private Models.Grid.Grid ObterGridCargasFinalizadasPelaOSMaeOuOS()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descricao", false);
            grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Empresa", 15, Models.Grid.Align.left, true);

            return grid;
        }

        private void ReenviaIntegracaoDadosTransporte(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> alteracoesEtapaUm = repositorioCargaDadosTransporteIntegracao.BuscarPorCarga(codigoCarga);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao in alteracoesEtapaUm)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> lstTipos = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>
                        {
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Obramax
                        };

                if (repositorioTipoIntegracao.ExistePorTipo(lstTipos))
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
                }
            }
        }
        #endregion
    }
}
