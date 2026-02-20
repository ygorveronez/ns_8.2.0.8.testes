using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize("Frotas/ControleKmReboque")]
    public class ControleKmReboqueController : BaseController
    {
		#region Construtores

		public ControleKmReboqueController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaHistoricoVinculoKmReboque filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "PlacaVeiculo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Reboque", "PlacaReboque", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Criação", "DataCriacao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Alteração", "DataAlteracao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("KM", "KM", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 12, Models.Grid.Align.right, false);
                
                //var propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                //if (propOrdenar == "PlacaReboque")
                //    propOrdenar = "Veiculo.Placa";
                

                var repHistoricoVinculoKmReboque = new Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque(unitOfWork);
                var listaHistorico = repHistoricoVinculoKmReboque.Consulta(filtrosPesquisa, null, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repHistoricoVinculoKmReboque.ContaConsulta(filtrosPesquisa));

                var lista = (from p in listaHistorico
                             select new
                             {
                                 p.Codigo,
                                 PlacaVeiculo = p.Veiculo != null ? p.Veiculo.Placa : string.Empty,
                                 PlacaReboque = p.Reboque != null ? p.Reboque.Placa : string.Empty,
                                 DataCriacao = p.DataCriacao != null ? p.DataCriacao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 DataAlteracao = p.DataAlteracao != null ? p.DataAlteracao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 KM = p.KMAtual.ToString("n0"),
                                 Tipo = p.TipoMovimento.ObterDescricao()

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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaHistoricoVinculoKmReboque filtrosPesquisa = ObterFiltrosPesquisa(unidadeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Reboque", "Reboque", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Criação", "DataCriacao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Alteração", "DataAlteracao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("KM", "KM", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 12, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                //if (propOrdenar == "Placa")
                //    propOrdenar = "Veiculo.Placa";

                Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque repHistoricoVinculoKmReboque = new Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque(unidadeTrabalho);
                int countHistoricoVinculoKmReboque = repHistoricoVinculoKmReboque.ContaConsulta(filtrosPesquisa);
                if (countHistoricoVinculoKmReboque > 5000)
                    return new JsonpResult(false, true, "A quantidade de registros para exportação não pode ser superior a 5000.");

                List<Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque> listaAbastecimento = repHistoricoVinculoKmReboque.Consulta(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(countHistoricoVinculoKmReboque);

                var lista = (from p in listaAbastecimento
                             select new
                             {
                                 p.Codigo,
                                 Veiculo = p.Veiculo != null ? p.Veiculo.Placa : string.Empty,
                                 Reboque = p.Reboque != null ? p.Reboque.Placa : string.Empty,
                                 DataCriacao = p.DataCriacao != null ? p.DataCriacao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 DataAlteracao = p.DataAlteracao != null ? p.DataAlteracao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 KM = p.KMAtual.ToString("n0"),
                                 Tipo = p.TipoMovimento.ObterDescricao()
                             }).ToList();

                grid.AdicionaRows(lista);
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
                unidadeTrabalho.Dispose();
            }
        }

        
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque repHistoricoVinculoKmReboque = new Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque historico = repHistoricoVinculoKmReboque.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));

                string msgErro = "";
                int.TryParse(Request.Params("KM"), out int kmAtual);
                int.TryParse(Request.Params("Tipo"), out int tipo);
                int.TryParse(Request.Params("Veiculo"), out int vei);
                int.TryParse(Request.Params("Reboque"), out int reb);



                Servicos.Embarcador.Veiculo.CalculoKmReboque calculoKmReboque = new Servicos.Embarcador.Veiculo.CalculoKmReboque(unitOfWork);                
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(vei);
                Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorCodigo(reb);

                calculoKmReboque.CalcularKmReboque(veiculo, reboque, (TipoMovimentoKmReboque)tipo, kmAtual, true, historico, out msgErro, unitOfWork, Auditado);
                if (msgErro == "")
                {
                    historico.KMAtual = kmAtual;
                    historico.DataAlteracao = DateTime.Now;

                    repHistoricoVinculoKmReboque.Atualizar(historico);
                    unitOfWork.CommitChanges();
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, msgErro);                    
                }
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque repHistoricoVinculoKmReboque = new Repositorio.Embarcador.Veiculos.HistoricoVinculoKmReboque(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque historico = repHistoricoVinculoKmReboque.BuscarPorCodigo(codigo);                

                if (historico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
              
                var dynHistorico = new
                {
                    historico.Codigo,
                    Veiculo = historico.Veiculo != null ? new { Codigo = historico.Veiculo.Codigo, Descricao = historico.Veiculo.Placa } : null,
                    Reboque = historico.Reboque != null ? new { Codigo = historico.Reboque.Codigo, Descricao = historico.Reboque.Placa } : null,
                    Tipo = historico.TipoMovimento,
                    DataCriacao = historico.DataCriacao.HasValue ? historico.DataCriacao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataAltercao = historico.DataAlteracao.HasValue ? historico.DataAlteracao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,                    
                    KM = historico.KMAtual,                    
                };

                return new JsonpResult(dynHistorico);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaHistoricoVinculoKmReboque ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaHistoricoVinculoKmReboque filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaHistoricoVinculoKmReboque()
            {
                DataCriacaoInicial = Request.GetNullableDateTimeParam("DataCriacaoInicial"),
                DataCriacaoFinal = Request.GetNullableDateTimeParam("DataCriacaoFinal"),
                DataAlteracaoInicial = Request.GetNullableDateTimeParam("DataAlteracaoInicial"),
                DataAlteracaoFinal = Request.GetNullableDateTimeParam("DataAlteracaoFinal"),
                Veiculo = Request.GetIntParam("Veiculo"),
                Reboque = Request.GetIntParam("Reboque"),
                TipoMovimentoKmReboque = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoKmReboque>("TipoMovimento"),                
            };

            return filtrosPesquisa;
        }

        #endregion Métodos Privados
    }
}
