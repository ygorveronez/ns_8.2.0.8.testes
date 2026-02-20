using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Data;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/Mensalidade")]
    public class MensalidadeController : BaseController
    {
        #region Construtores

        public MensalidadeController(Conexao conexao) : base(conexao) { }

        #endregion
        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                Servicos.Log.TratarErro(ex);
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
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", "Codigo", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Vencimento", "DataVencimento", 12, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 12, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor", "Valor", 12, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 12, Models.Grid.Align.center, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            double cnpjCliente = this.Empresa?.CNPJ.ToDouble() ?? 0d;
            int codigoEmpresaPai = this.Empresa?.EmpresaPai?.Codigo ?? 0;

            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repTitulo.ConsultarTitulosPorPessoa(cnpjCliente, codigoEmpresaPai, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repTitulo.ContarConsultarTitulosPorPessoa(cnpjCliente, codigoEmpresaPai);
            var lista = (from p in listaTitulo
                         select new
                         {
                             Codigo = p.Codigo,
                             DataVencimento = p.DataVencimento.Value.ToString("dd/MM/yyyy"),
                             DataEmissao = p.DataEmissao.Value.ToString("dd/MM/yyyy"),
                             Valor = p.ValorOriginal.ToString("n2"),
                             p.DescricaoSituacao,
                             DT_RowColor = p.StatusTitulo == StatusTitulo.Quitada ? CorGrid.Verde : CorGrid.Branco
                         }).ToList();

            return lista.ToList();
        }

        #endregion
    }
}