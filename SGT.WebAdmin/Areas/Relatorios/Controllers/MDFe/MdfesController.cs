using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.MDFe
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Mdfe/Mdfes")]
    public class MdfesController : BaseController
    {
		#region Construtores

		public MdfesController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R161_Mdfe;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de MDF-e", "Mdfe", "Mdfe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
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
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.MDFes.MDFes servicoRelatorioMFDes = new Servicos.Embarcador.Relatorios.MDFes.MDFes(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioMFDes.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.MDFe.Mdfe> listaMdfe, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaMdfe);

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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMdfeRelatorio()
            {
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoSerie = Request.GetIntParam("Serie"),
                CpfMotorista = Request.GetStringParam("CpfMotorista").ObterSomenteNumeros(),
                DataAutorizacaoInicial = Request.GetNullableDateTimeParam("DataAutorizacaoInicial"),
                DataAutorizacaoLimite = Request.GetNullableDateTimeParam("DataAutorizacaoLimite"),
                DataCancelamentoInicial = Request.GetNullableDateTimeParam("DataCancelamentoInicial"),
                DataCancelamentoLimite = Request.GetNullableDateTimeParam("DataCancelamentoLimite"),
                DataEmissaoInicial = Request.GetNullableDateTimeParam("DataEmissaoInicial"),
                DataEmissaoLimite = Request.GetNullableDateTimeParam("DataEmissaoLimite"),
                DataEncerramentoInicial = Request.GetNullableDateTimeParam("DataEncerramentoInicial"),
                DataEncerramentoLimite = Request.GetNullableDateTimeParam("DataEncerramentoLimite"),
                EstadoCarregamento = Request.GetStringParam("EstadoCarregamento"),
                EstadoDescarregamento = Request.GetStringParam("EstadoDescarregamento"),
                ListaStatusMdfe = Request.GetListEnumParam<StatusMDFe>("StatusMdfe"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroLimite = Request.GetIntParam("NumeroLimite"),
                PlacaVeiculo = Request.GetStringParam("PlacaVeiculo"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                ExibirCTes = Request.GetBoolParam("ExibirCTes"),
                MDFeVinculadoACarga = Request.GetNullableBoolParam("MDFeVinculadoACarga"),
                MunicipioDescarregamento = Request.GetIntParam("MunicipioDescarregamento"),
                CodigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork)
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Série", "Serie", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Autorização", "DataAutorizacaoFormatada", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Cancelamento", "DataCancelamentoFormatada", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Encerramento", "DataEncerramentoFormatada", 10, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Protocolo Autorização", "ProtocoloAutorizacao", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Protocolo Cancelamento", "ProtocoloCancelamento", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Protocolo Encerramento", "ProtocoloEncerramento", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave Acesso", "ChaveAcesso", 15, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("CNPJ Empresa/Filial", "CnpjEmpresa", 15, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Empresa/Filial", "RazaoSocialEmpresa", 26, Models.Grid.Align.left, true, false, false, true, true);
            }
            else
            {
                grid.AdicionarCabecalho("CNPJ Transportador", "CnpjEmpresa", 15, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Transportador", "RazaoSocialEmpresa", 26, Models.Grid.Align.left, true, false, false, true, true);
            }

            grid.AdicionarCabecalho("Retorno Sefaz", "MensagemRetornoSefaz", 26, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação", "StatusMdfeDescricao", 12, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("UF Carregamento", "UfCarregamento", 15, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("UF Descarregamento", "UfDescarregamento", 15, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Veículos", "Veiculos", 26, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Motoristas", "Motoristas", 26, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Valor Total Mercadoria", "ValorTotalMercadoria", 15, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso Bruto Mercadoria", "PesoBrutoMercadoriaFormatado", 15, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Justificativa de Cancelamento", "JustificativaCancelamento", 26, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CT-es", "CTes", 12, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", 15, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor a Receber", "ValorReceber", 15, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Município Descarregamento", "MunicipioDescarregamento", 15, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CPF Motorista", "CPFMotoristaFormatado", 15, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomadorFormatado", 15, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", 26, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
