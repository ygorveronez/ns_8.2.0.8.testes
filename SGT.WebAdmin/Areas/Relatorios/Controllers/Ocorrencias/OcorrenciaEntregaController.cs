using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Ocorrencias
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Ocorrencias/OcorrenciaEntrega")]
    public class OcorrenciaEntregaController : BaseController
    {
        #region Construtores

        public OcorrenciaEntregaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R352_OcorrenciasEntrega;
        private int NumeroMaximoParametrosDinamicos = 6;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorioEntrega(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Ocorrências de Entrega", "Ocorrencias", "OcorrenciaEntrega.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, false, 7);
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

        public async Task<IActionResult> PesquisaEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Ocorrencias.OcorrenciaEntrega servicoOcorrencia = new Servicos.Embarcador.Relatorios.Ocorrencias.OcorrenciaEntrega(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoOcorrencia.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaEntrega> lista, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

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

        public async Task<IActionResult> GerarRelatorioEntrega(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
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

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega()
            {
                CodigosFilial = Request.GetListParam<int>("Filial").Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : Request.GetListParam<int>("Filial"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigosOcorrencia = Request.GetListParam<int>("Ocorrencia"),
                CodigoRecebedor = Request.GetIntParam("Operador"),
                CodigoSolicitante = Request.GetListParam<int>("Solicitante"),
                CodigoTransportadorChamado = Request.GetIntParam("TransportadorChamado"),
                CodigosTransportadorCarga = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? new List<int>() { Usuario?.Empresa?.Codigo ?? 0 } : Request.GetListParam<int>("TransportadorCarga"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CpfCnpjPessoa = Request.GetDoubleParam("Pessoa"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                NumeroOcorrenciaFinal = Request.GetIntParam("NumeroOcorrenciaFinal"),
                NumeroOcorrenciaInicial = Request.GetIntParam("NumeroOcorrenciaInicial"),
                SituacoesCancelamento = Request.GetListEnumParam<SituacaoOcorrencia>("SituacaoCancelamento"),
                SituacoesOcorrencia = Request.GetListEnumParam<SituacaoOcorrencia>("SituacaoOcorrencia"),
                TiposOperacaoCarga = Request.GetListParam<int>("TipoOperacaoCarga"),
                CargaAgrupada = Request.GetStringParam("CargaAgrupada"),
                DataCriacaoFinal = Request.GetNullableDateTimeParam("DataSolicitacaoFim"),
                DataCriacaoInicial = Request.GetNullableDateTimeParam("DataSolicitacaoInicial"),
                CodigosGrupoOcorrencia = Request.GetListParam<int>("GrupoOcorrencia")
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
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.ObterConfiguracaoPadrao();
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            // daqui
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DescricaoOcorrencia, "DescricaoOcorrenciaFormatada", 35, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataOcorrencia, "DataOcorrenciaFormatada", 10, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Latitude, "Latitude", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Longitude, "Longitude", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataPosicao, "DataPosicaoFormatada", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DataPrevisaoReprogramadaDaEntrega, "DataPrevisaoReprogramadaDaEntregaFormatada", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.TempoPercurso, "TempoPercurso", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.DistanciaAteDestino, "DistanciaCalculada", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Pacote, "Pacote", 5, Models.Grid.Align.left, true, false, false, true, configuracaoControleEntrega.ExibirPacotesOcorrenciaControleEntrega);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Volumes, "Volumes", 5, Models.Grid.Align.left, true, false, false, true, configuracaoControleEntrega.ExibirPacotesOcorrenciaControleEntrega);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Origem, "OrigemFormatada", 10, Models.Grid.Align.left, true, false, false, true, true);
            // até aqui

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroOcorrencia, "NumeroOcorrencia", 6, Models.Grid.Align.right, true, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Carga, "CodigoCargaEmbarcador", 6, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.GrupoPessoas, "GrupoPessoas", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NotasFicais, "NotasFiscais", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.SerieNotasFiscais, "SerieNotasFiscais", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.GrupoOcorrencias, "GrupoOcorrencia", 10, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Chamado, "Chamado", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CNPJTransportadora, "CNPJTransportadora", 8, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Transportadora, "Transportadora", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CodigoDestinatarios, "CodigoIntegracaoDestinatarios", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Destinatarios, "Destinatarios", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CNPJDestinatarios, "CNPJDestinatariosFormatado", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Cliente, "Cliente", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Expedidor, "Expedidor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Recebedor, "Recebedor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataCarga, "DataCargaFormatada", 5, Models.Grid.Align.center, true, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Observacao, "Observacao", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Motorista, "Motorista", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Veiculo, "Placa", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoVeiculo, "TipoVeiculo", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CodigoFilial, "CodigoIntegracaoFilial", 6, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CNPJFilial, "CNPJFilialFormatado", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Filial, "Filial", 10, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.TipoOperacaoCarga, "TipoOperacaoCarga", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CargasAgrupadas, "CargaAgrupada", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NomeFantasiaDestinatarios, "NomeFantasiaDestinatarios", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.NumeroPedido, "PedidosFormatado", 6, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.Destinos, "Destinos", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataCarregamento, "DataCarregamentoFormatada", 5, Models.Grid.Align.center, true, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CNPJCliente, "CPFCNPJClienteDescricao", 7, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.CPFMotorista, "CPFMotoristaFormatado", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Ocorrencias.Ocorrencia.DataPrevisaoEntregaAjustada, "DataPrevisaoEntregaAjustadaFormatada", 5, Models.Grid.Align.center, true, false, false, true, false);

            return grid;
        }

        #endregion
    }
}
