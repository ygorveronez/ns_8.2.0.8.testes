using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/CTeTituloReceber")]
    public class CTeTituloReceberController : BaseController
    {
        #region Construtores

        public CTeTituloReceberController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados Somente Leitura

        private readonly decimal _tamanhoColunasPequeno = 6m;
        private readonly decimal _tamanhoColunasMedio = 15m;
        private readonly decimal _tamanhoColunasGrande = 20m;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = ObterGridPesquisa(unitOfWork);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCTeTituloReceber filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = ObterGridPadrao(filtrosPesquisa);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "CTeTituloReceber/Pesquisa", "grid-pesquisa-cteTituloReceber");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

            int totalRegistros = repCTe.ContarTituloReceberCTe(filtrosPesquisa);
            IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.CTeTituloReceber> lista = totalRegistros > 0 ? repCTe.ConsultarTituloReceberCTe(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Financeiro.CTeTituloReceber>();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCTeTituloReceber ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCTeTituloReceber()
            {
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa.Codigo : Request.GetIntParam("Empresa"),
                StatusTitulo = Request.GetEnumParam<StatusTitulo>("StatusTitulo"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                TipoServicoMultisoftware = TipoServicoMultisoftware
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.StartsWith("Descricao"))
                return propriedadeOrdenar.Replace("Descricao", "");

            return propriedadeOrdenar;
        }

        private Models.Grid.Grid ObterGridPadrao(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCTeTituloReceber filtrosPesquisa)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Observacao", false);
            grid.AdicionarCabecalho("Número CT-e", "Numero", _tamanhoColunasPequeno, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Série CT-e", "Serie", _tamanhoColunasPequeno, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo Doc.", "TipoDocumento", _tamanhoColunasPequeno, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Origem", "Origem", _tamanhoColunasMedio, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destino", "Destino", _tamanhoColunasMedio, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data de Emissão", "DescricaoDataEmissao", _tamanhoColunasMedio, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data de Vencimento", "DescricaoDataVencimento", _tamanhoColunasMedio, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data do Pagamento", "DescricaoDataLiquidacao", _tamanhoColunasMedio, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor a Receber", "DescricaoValorAReceber", _tamanhoColunasMedio, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Número da Fatura", "NumeroFatura", _tamanhoColunasPequeno, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Número do Pagamento", "NumeroPagamento", _tamanhoColunasPequeno, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Qtde de Parcelas", "DescricaoQuantidadeParcelas", _tamanhoColunasPequeno, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Sequência Parcela", "DescricaoSequenciaParcela", _tamanhoColunasPequeno, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Valor Parcela Paga", "DescricaoValorParcelaPaga", _tamanhoColunasPequeno, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Sequência Parcela Paga", "DescricaoSequenciaParcelaPaga", _tamanhoColunasPequeno, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("CodigoTitulo", false);
            grid.AdicionarCabecalho("CodigoEmpresa", false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho("Transportador", "Transportador", _tamanhoColunasGrande, Models.Grid.Align.left, true);

            if (filtrosPesquisa.StatusTitulo == StatusTitulo.Todos)
                grid.AdicionarCabecalho("Situação Título", "DescricaoStatusTitulo", _tamanhoColunasGrande, Models.Grid.Align.center, true);

            return grid;
        }

        #endregion
    }
}
