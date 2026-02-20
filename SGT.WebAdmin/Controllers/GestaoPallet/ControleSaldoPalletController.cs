using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet.DetalhesGestaoPallet;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.GestaoPallet
{
    [CustomAuthorize("GestaoPallet/ControleSaldoPallet")]
    public class ControleSaldoPalletController : BaseController
    {
        #region Construtores

        public ControleSaldoPalletController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa(unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar controle de saldos pallet.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Grid grid = ObterGridPesquisa(unidadeTrabalho);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);

                return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unidadeTrabalho, cancellationToken);

                DetalhesGestaoPallet dados = await repositorioMovimentacaoPallet.BuscarPorMovimentacaoPalletAsync(codigo);

                return new JsonpResult(dados);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter controle de saldo.");
            }
            finally
            {
                await unidadeTrabalho.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unidadeTrabalho).BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet filtrosPesquisa = ObterFiltrosPesquisa(configuracaoPaletes);
                Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.TotalizadoresControleSaldoPallet dados = repositorioMovimentacaoPallet.ConsultarTotalizadoresControleSaldoPallet(filtrosPesquisa);

                return new JsonpResult(dados);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais do controle de saldos.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Grid ObterGridPesquisa(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unidadeTrabalho).BuscarConfiguracaoPadrao();

            Grid grid = ObterCabecalhosGridPesquisa();
            Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet filtrosPesquisa = ObterFiltrosPesquisa(configuracaoPaletes);
            int totalLinhas = repositorioMovimentacaoPallet.ContarConsultaControleSaldoPallet(filtrosPesquisa);

            GridPreferencias gridPreferencias = new GridPreferencias(unidadeTrabalho, "ControleSaldoPallet/Pesquisa", "grid-gestao-pallet-controle-saldo");
            Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciasGrid = gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo);
            grid.AplicarPreferenciasGrid(preferenciasGrid);

            if (totalLinhas == 0)
            {
                grid.AdicionaRows(new List<dynamic>() { });
                return grid;
            }

            string ordenacao = ObterPropriedadeOrdenarOuAgrupar(grid.header[grid.indiceColunaOrdena].data);
            IList<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.DadosControleSaldoPallet> dados = repositorioMovimentacaoPallet.ConsultaControleSaldoPallet(filtrosPesquisa, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);

            grid.AdicionaRows(dados);
            grid.setarQuantidadeTotal(totalLinhas);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet ObterFiltrosPesquisa(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes)
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControleSaldoPallet.FiltroPesquisaControleSaldoPallet()
            {
                CodigosFilial = Request.GetListParam<int>("Filial"),
                NumeroCarga = Request.GetStringParam("Carga"),
                NumeroNotaFiscal = Request.GetStringParam("NotaFiscal"),
                DataInicialCriacaoCarga = Request.GetDateTimeParam("DataInicialCriacaoCarga"),
                DataFinalCriacaoCarga = Request.GetDateTimeParam("DataFinalCriacaoCarga"),
                SituacaoPallet = Request.GetNullableEnumParam<SituacaoGestaoPallet>("SituacaoPallet"),
                SituacaoPalletGestaoDevolucao = Request.GetNullableEnumParam<SituacaoPalletGestaoDevolucao>("SituacaoPalletGestaoDevolucao"),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                filtrosPesquisa.ResponsavelPallet = ResponsavelPallet.Transportador;
                filtrosPesquisa.CodigoTransportador = Usuario.Empresa.Codigo;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                filtrosPesquisa.ResponsavelPallet = ResponsavelPallet.Cliente;
                filtrosPesquisa.CodigoCliente = Usuario.Cliente.Codigo;
            }

            if (configuracaoPaletes != null)
            {
                filtrosPesquisa.DiasLimiteParaDevolucao = configuracaoPaletes.LimiteDiasParaDevolucaoDePallet;
                filtrosPesquisa.DataLimiteGeracaoDevolucao = DateTime.Now.AddDays(-configuracaoPaletes.LimiteDiasParaDevolucaoDePallet);
            }

            return filtrosPesquisa;
        }

        private Grid ObterCabecalhosGridPesquisa()
        {
            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            grid.AdicionarCabecalho("", "Codigo", 7, Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Número da Nota Fiscal", "NumeroNota", 10, Align.center, true);
            grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 10, Align.center, true);
            grid.AdicionarCabecalho("Quantidade de Pallet", "QuantidadePallets", 10, Align.center, false);
            grid.AdicionarCabecalho("Data", "DataRecebimentoFormatada", 10, Align.center, false);
            grid.AdicionarCabecalho("Tipo de Regra", "RegraPalletFormatada", 10, Align.left, false);
            grid.AdicionarCabecalho("Situação", "SituacaoPalletFormatada", 10, Align.center, false);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Align.left, false);
            grid.AdicionarCabecalho("Destino", "Destino", 10, Align.left, false);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Align.left, false);
            grid.AdicionarCabecalho("Tipo Devolução", "DescricaoTipoGestaoDevolucao", 10, Align.left, false);
            grid.AdicionarCabecalho("LeadTime da Devolução", "LeadTimeDevolucaoFormatado", 10, Align.left, false);
            grid.AdicionarCabecalho("Situação da Devolução", "SituacaoDevolucao", 10, Align.center, false, false);
            grid.AdicionarCabecalho("Responsável", "ResponsavelFormatada", 10, Align.left, false, false);
            grid.AdicionarCabecalho("Quebra de Regra", "QuebraRegraFormatada", 10, Align.center, false, false);
            grid.AdicionarCabecalho("Tipo Lançamento", "DescricaoTipoLancamento", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo Movimentação", "DescricaoTipoMovimentacao", 5, Models.Grid.Align.left, false, false);

            return grid;
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion Métodos Privados
    }
}