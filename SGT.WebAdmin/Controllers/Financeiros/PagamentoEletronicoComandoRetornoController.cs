using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/PagamentoEletronicoComandoRetorno")]
    public class PagamentoEletronicoComandoRetornoController : BaseController
    {
		#region Construtores

		public PagamentoEletronicoComandoRetornoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Código", "Codigo", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Comando", "Comando", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 7, Models.Grid.Align.center, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno repPagamentoEletronicoComandoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno(unitOfWork);

            string descricao = Request.Params("Descricao");
            string comando = Request.Params("Comando");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;
            int codigoBoletoConiguracao = 0;
            int.TryParse(Request.Params("BoletoConfiguracao"), out codigoBoletoConiguracao);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;
            Enum.TryParse(Request.Params("Ativo"), out ativo);

            List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno> listaPagamentoEletronicoComandoRetorno = repPagamentoEletronicoComandoRetorno.Consultar(codigoEmpresa, codigoBoletoConiguracao, comando, descricao, ativo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repPagamentoEletronicoComandoRetorno.ContarConsulta(codigoEmpresa, codigoBoletoConiguracao, comando, descricao, ativo);
            var lista = (from p in listaPagamentoEletronicoComandoRetorno
                         select new
                         {
                             p.Codigo,
                             p.Comando,
                             p.Descricao,
                             p.DescricaoAtivo
                         }).ToList();

            return lista.ToList();
        }

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

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                // Retorna Dados
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno repPagamentoEletronicoComandoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int.TryParse(Request.Params("BoletoConfiguracao"), out int codigoBoletoConfiguracao);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno pagamentoEletronicoComandoRetorno = new Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno();

                pagamentoEletronicoComandoRetorno.Ativo = bool.Parse(Request.Params("Ativo"));
                pagamentoEletronicoComandoRetorno.Descricao = Request.Params("Descricao");
                pagamentoEletronicoComandoRetorno.Comando = Request.Params("Comando");
                pagamentoEletronicoComandoRetorno.ComandoDeEstorno = bool.Parse(Request.Params("ComandoDeEstorno"));
                pagamentoEletronicoComandoRetorno.ComandoDeLiquidacao = bool.Parse(Request.Params("ComandoDeLiquidacao"));
                pagamentoEletronicoComandoRetorno.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    pagamentoEletronicoComandoRetorno.Empresa = Empresa;

                if (pagamentoEletronicoComandoRetorno.ComandoDeEstorno && pagamentoEletronicoComandoRetorno.ComandoDeLiquidacao)
                    return new JsonpResult(false, "Não é possível cadastrar um comando que Liquide e Estorne ao mesmo tempo.");

                repPagamentoEletronicoComandoRetorno.Inserir(pagamentoEletronicoComandoRetorno, Auditado);

                unitOfWork.CommitChanges();
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno repPagamentoEletronicoComandoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int.TryParse(Request.Params("BoletoConfiguracao"), out int codigoBoletoConfiguracao);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno pagamentoEletronicoComandoRetorno = repPagamentoEletronicoComandoRetorno.BuscarPorCodigo(codigo, true);
                pagamentoEletronicoComandoRetorno.Ativo = bool.Parse(Request.Params("Ativo"));
                pagamentoEletronicoComandoRetorno.Descricao = Request.Params("Descricao");
                pagamentoEletronicoComandoRetorno.Comando = Request.Params("Comando");
                pagamentoEletronicoComandoRetorno.ComandoDeEstorno = bool.Parse(Request.Params("ComandoDeEstorno"));
                pagamentoEletronicoComandoRetorno.ComandoDeLiquidacao = bool.Parse(Request.Params("ComandoDeLiquidacao"));
                pagamentoEletronicoComandoRetorno.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);

                if (pagamentoEletronicoComandoRetorno.ComandoDeEstorno && pagamentoEletronicoComandoRetorno.ComandoDeLiquidacao)
                    return new JsonpResult(false, "Não é possível cadastrar um comando que Liquide e Estorne ao mesmo tempo.");

                repPagamentoEletronicoComandoRetorno.Atualizar(pagamentoEletronicoComandoRetorno, Auditado);

                unitOfWork.CommitChanges();
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno repPagamentoEletronicoComandoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno pagamentoEletronicoComandoRetorno = repPagamentoEletronicoComandoRetorno.BuscarPorCodigo(codigo);

                var dynPagamentoEletronicoComandoRetorno = new
                {
                    pagamentoEletronicoComandoRetorno.Codigo,
                    pagamentoEletronicoComandoRetorno.Descricao,
                    BoletoConfiguracao = new { Codigo = pagamentoEletronicoComandoRetorno.BoletoConfiguracao != null ? pagamentoEletronicoComandoRetorno.BoletoConfiguracao.Codigo : 0, Descricao = pagamentoEletronicoComandoRetorno.BoletoConfiguracao != null ? pagamentoEletronicoComandoRetorno.BoletoConfiguracao.DescricaoBanco : "" },
                    pagamentoEletronicoComandoRetorno.Comando,
                    pagamentoEletronicoComandoRetorno.Ativo,
                    pagamentoEletronicoComandoRetorno.ComandoDeEstorno,
                    pagamentoEletronicoComandoRetorno.ComandoDeLiquidacao
                };
                return new JsonpResult(dynPagamentoEletronicoComandoRetorno);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno repPagamentoEletronicoComandoRetorno = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoComandoRetorno pagamentoEletronicoComandoRetorno = repPagamentoEletronicoComandoRetorno.BuscarPorCodigo(codigo);
                repPagamentoEletronicoComandoRetorno.Deletar(pagamentoEletronicoComandoRetorno, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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

        #endregion
    }
}
