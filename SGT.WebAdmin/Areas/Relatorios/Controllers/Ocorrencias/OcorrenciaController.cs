using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Ocorrencias
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Ocorrencias/Ocorrencia")]
    public class OcorrenciaController : BaseController
    {
        #region Construtores

        public OcorrenciaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R037_Ocorrencias;
        private int NumeroMaximoParametrosDinamicos = 6;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Ocorrências", "Ocorrencias", "Ocorrencia.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, false, 7);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoBuscarDadosRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Ocorrencias.Ocorrencia servicoOcorrencia = new Servicos.Embarcador.Relatorios.Ocorrencias.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoOcorrencia.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.Ocorrencia.Ocorrencia> lista, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, propriedades, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.OcorreuUmaFalhaAoGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrencia ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrencia filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrencia()
            {
                CodigosFilial = Request.GetListParam<int>("Filial").Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : Request.GetListParam<int>("Filial"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigosOcorrencia = Request.GetListParam<int>("Ocorrencia"),
                Recebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                CodigoRecebedor = Request.GetIntParam("Operador"),
                CodigoResponsavelChamado = Request.GetIntParam("ResponsavelChamado"),
                CodigoSolicitante = Request.GetListParam<int>("Solicitante"),
                CodigoTransportadorChamado = Request.GetIntParam("TransportadorChamado"),
                CodigosTransportadorCarga = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? new List<int>() { Usuario?.Empresa?.Codigo ?? 0 } : Request.GetListParam<int>("TransportadorCarga"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CpfCnpjPessoa = Request.GetDoubleParam("Pessoa"),
                DataCancelamentoFinal = Request.GetNullableDateTimeParam("DataCancelamentoFinal"),
                DataCancelamentoInicial = Request.GetNullableDateTimeParam("DataCancelamentoInicial"),
                DataOcorrenciaFinal = Request.GetNullableDateTimeParam("DataOcorrenciaFinal"),
                DataOcorrenciaInicial = Request.GetNullableDateTimeParam("DataOcorrenciaInicial"),
                DataSolicitacaoFinal = Request.GetNullableDateTimeParam("DataSolicitacaoFim"),
                DataSolicitacaoInicial = Request.GetNullableDateTimeParam("DataSolicitacaoInicial"),
                NumeroCTeGerado = Request.GetIntParam("NumeroCTeGerado"),
                NumeroCTeOriginal = Request.GetIntParam("NumeroCTeOriginal"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                NumeroOcorrenciaCliente = Request.GetStringParam("NumeroOcorrenciaCliente"),
                NumeroOcorrenciaFinal = Request.GetIntParam("NumeroOcorrenciaFinal"),
                NumeroOcorrenciaInicial = Request.GetIntParam("NumeroOcorrenciaInicial"),
                SituacoesCancelamento = Request.GetListEnumParam<SituacaoOcorrencia>("SituacaoCancelamento"),
                SituacoesOcorrencia = Request.GetListEnumParam<SituacaoOcorrencia>("SituacaoOcorrencia"),
                ValorFinal = Request.GetDecimalParam("ValorFinal"),
                ValorInicial = Request.GetDecimalParam("ValorInicial"),
                TiposOperacaoCarga = Request.GetListParam<int>("TipoOperacaoCarga"),
                TipoDocumentoCreditoDebito = Request.GetEnumParam("TipoDocumentoCreditoDebito", TipoDocumentoCreditoDebito.Todos),
                TipoDocumentoEmissao = Request.GetEnumParam<Dominio.Enumeradores.TipoDocumento>("ModeloDocumentoFiscal"),
                CargaAgrupada = Request.GetStringParam("CargaAgrupada"),
                OcorrenciaEstadia = Request.GetNullableBoolParam("OcorrenciaEstadia"),
                EtapaEstadia = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaEntrega>("EtapaEstadia"),
                CodigoGrupoOcorrencia = Request.GetListParam<int>("GrupoOcorrencia")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                filtrosPesquisa.CodigoSolicitante.Clear();
                filtrosPesquisa.CodigoSolicitante.Add(this.Usuario.Codigo);
            }

            int codigoCarga = Request.GetIntParam("Carga");

            if (codigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                filtrosPesquisa.CodigoCargaEmbarcador = repositorioCarga.BuscarPorCodigo(codigoCarga)?.CodigoCargaEmbarcador ?? "";
            }

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repositorioConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repositorioConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia> parametrosOcorrencia = repParametroOcorrencia.BuscarTodosAtivos();

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroOcorrencia, "NumeroOcorrencia", 6, Models.Grid.Align.right, true, false, false, false, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroOcorrenciaCliente, "NumeroOcorrenciaCliente", 6, Models.Grid.Align.right, true, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Carga, "Carga", 6, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CodigoTomador, "CodigoIntegracaoTomador", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Tomador, "Tomador", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.GrupoPessoas, "GrupoPessoas", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.GrupoOcorrencias, "GrupoOcorrencia", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CTesOriginal, "NumerosCTeOriginal", 6, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumerosCTeOcorrencia, "NumerosCTeOcorrencia", 6, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CTesComp, "NumerosCTes", 6, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NotasFicais, "NotasFiscais", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.SerieNotasFiscais, "SerieNotasFiscais", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ObsCTeComp, "ObservacaoCTeComp", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Resposavel, "Responsavel", 10, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataOcorrencia, "DataSolicitacao", 5, Models.Grid.Align.center, true, false, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataGeracao, "DataAlteracao", 5, Models.Grid.Align.center, true, false, false, false, true);
            }
            else
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataSolicitacao, "DataSolicitacao", 5, Models.Grid.Align.center, true, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Solicitante, "Solicitante", 13, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DescricaoOcorrencia, "DescricaoOcorrencia", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Valor, "Valor", 8, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Operador, "Operador", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Situacao, "DescricaoSituacao", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.SituacaoCancelamento, "DescricaoSituacaoCancelamento", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Chamado, "Chamado", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataAprovacaoRejeicao, "DataAprovacaoFormatada", 5, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.JustificativaRejeicao, "JustificativaRejeicao", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.MotivoRejeicao, "MotivoRejeicaoFormatado", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.MotivoAprovacao, "MotivoAprovacao", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ObservacaoAprovacao, "ObservacaoAprovacao", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CNPJTransportadora, "CNPJTransportadora", 8, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Transportadora, "Transportadora", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CodigoRemetentes, "CodigoIntegracaoRemetentes", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Remetentes, "Remetentes", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CodigoDestinatarios, "CodigoIntegracaoDestinatarios", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Destinatarios, "Destinatarios", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CNPJDestinatarios, "CNPJDestinatariosFormatado", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Cliente, "Cliente", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Expedidor, "Expedidor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Recebedor, "Recebedor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataCarga, "DataCargaFormatada", 5, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ObsCTeOrig, "ObservacaoCTeOriginal", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Motivo, "MotivoCancelamento", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Carencia, "Carencia", 4, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataHoraSaida, "DataSaidaFormatada", 7, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataHoraChegada, "DataChegadaFormatada", 7, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataHoraEmbarque, "DataEmbarqueFormatada", 7, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.QuantidadeHoras, "QuantidadeDeHoras", 5, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataChegadaReentrega, "DataChegadaReentregaFormatada", 7, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataRetornoReentrega, "DataRetornoReentregaFormatada", 7, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroGLog, "NumeroGlog", 7, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.JanelaDescarga, "JanelaDescarga", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Observacao, "Observacao", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Motorista, "Motorista", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Veiculo, "Placa", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoVeiculo, "TipoVeiculo", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CodigoFilial, "CodigoIntegracaoFilial", 6, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CNPJFilial, "CNPJFilialFormatado", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Filial, "Filial", 10, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ChaveCTeComp, "ChaveCTeComp", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataEmissaoCTeComp, "DataEmissaoCTeComp", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ValorReceberCTeComp, "ValorReceberCTeComp", 6, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ValorICMSCTeComp, "ValorIcmsCTeComp", 6, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CSTIBSCBSCTeComp, "CSTIBSCBSCTeComp", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ClassTribIBSCBSCTeComp, "ClassTribIBSCBSCTeComp", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ValorCBSCTeComp, "ValorCBSCTeComp", 6, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ValorIBSMunicipalCTeComp, "ValorIBSMunicipalCTeComp", 6, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ValorIBSUFCTeComp, "ValorIBSUFCTeComp", 6, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.StatusCTeComp, "StatusCTeComp", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.RetornoSefazCTeComp, "RetornoSefazCTeComp", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CSTICMSCTeComp, "CSTICMSCTeComp", 6, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoOperacaoCarga, "TipoOperacaoCarga", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoCreditoDebito, "TipoCreditoDebito", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CargaPerido, "CargaPeriodo", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.MesPeriodo, "MesPeriodo", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.AnoPeriodo, "AnoPeriodo", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CentroResultado, "CentroResultado", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoDocumento, "TipoDocumentoFormatado", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Categoria, "Categoria", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.SetorResponsavel, "Setor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CargasAgrupadas, "CargaAgrupada", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroAcerto, "NumeroAcerto", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NomeFantasiaDestinatarios, "NomeFantasiaDestinatarios", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroPedido, "PedidosFormatado", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Destinos, "Destinos", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataCarregamento, "DataCarregamentoFormatada", 7, Models.Grid.Align.center, false, false, false, false, false);

            //Estadia
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.InicioEstadia, "DataInicioEstadia", 7, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.FimEstadia, "DataFimEstadia", 7, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.HorasEstadia, "HorasTotaisEstadia", 7, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.HorasExcedentesEstadia, "HorasExcedentesEstadia", 7, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.HorasFreeTime, "HorasFreetime", 7, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.EtapaEstadia, "EtapaEstadia", 7, Models.Grid.Align.center, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ValorOriginal, "ValorOriginal", 8, Models.Grid.Align.right, false, false, false, true, false);

            if (configuracaoOcorrencia.ExibirCampoInformativoPagadorAutorizacaoOcorrencia)
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Pagamento, "PagamentoFormatado", 7, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CNPJCliente, "CPFCNPJClienteDescricao", 7, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ChaveCTesOrig, "ChavesCTeOriginal", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.JustificativaOcorrencia, "JustificativaOcorrencia", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CPFMotorista, "CPFMotoristaFormatado", 10, Models.Grid.Align.left, false, false, false, false, false);

            if (ConfiguracaoEmbarcador.Pais == TipoPais.Exterior)
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CodigoAprovacao, "CodigoAprovacao", 10, Models.Grid.Align.left, true, false, false, false, false);

            //Colunas montadas dinamicamente
            for (int i = 0; i < parametrosOcorrencia.Count; i++)
            {
                if (i < NumeroMaximoParametrosDinamicos)
                    grid.AdicionarCabecalho(parametrosOcorrencia[i].Descricao, "ParametroOcorrencia" + (i + 1).ToString(), 8, Models.Grid.Align.left, false, false, false, false, TipoSumarizacao.nenhum, parametrosOcorrencia[i].Codigo);
            }

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Origem, "Origem", 10, Models.Grid.Align.center, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.ProtocoloOcorrencia, "ProtocoloOcorrencia", 6, Models.Grid.Align.center, false, false, false, true, false);

            return grid;
        }

        #endregion
    }
}
