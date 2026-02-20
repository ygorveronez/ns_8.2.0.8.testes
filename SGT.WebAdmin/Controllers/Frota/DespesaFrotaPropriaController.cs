using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/DespesaFrotaPropria")]
    public class DespesaFrotaPropriaController : BaseController
    {
		#region Construtores

		public DespesaFrotaPropriaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork);
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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork, true);
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

                Repositorio.Embarcador.Frota.DespesaFrotaPropria repDespesaFrotaPropria = new Repositorio.Embarcador.Frota.DespesaFrotaPropria(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria despesaFrotaPropria = new Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria();

                PreencherDespesaFrotaPropria(despesaFrotaPropria, unitOfWork);

                repDespesaFrotaPropria.Inserir(despesaFrotaPropria, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.DespesaFrotaPropria repDespesaFrotaPropria = new Repositorio.Embarcador.Frota.DespesaFrotaPropria(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria despesaFrotaPropria = repDespesaFrotaPropria.BuscarPorCodigo(codigo, true);

                PreencherDespesaFrotaPropria(despesaFrotaPropria, unitOfWork);

                repDespesaFrotaPropria.Atualizar(despesaFrotaPropria, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.DespesaFrotaPropria repDespesaFrotaPropria = new Repositorio.Embarcador.Frota.DespesaFrotaPropria(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria despesaFrotaPropria = repDespesaFrotaPropria.BuscarPorCodigo(codigo, false);

                var dynDespesaFrotaPropria = new
                {
                    Codigo = despesaFrotaPropria.Codigo,
                    Data = despesaFrotaPropria.Data.ToString("dd/MM/yyyy"),
                    Filial = despesaFrotaPropria.Filial != null ? new { despesaFrotaPropria.Filial.Codigo, Descricao = despesaFrotaPropria.Filial.Descricao } : null,
                    Valor = despesaFrotaPropria.Valor,
                    Observacao = despesaFrotaPropria.Observacao
                };

                return new JsonpResult(dynDespesaFrotaPropria);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.DespesaFrotaPropria repDespesaFrotaPropria = new Repositorio.Embarcador.Frota.DespesaFrotaPropria(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria despesaFrotaPropria = repDespesaFrotaPropria.BuscarPorCodigo(codigo, true);

                if (despesaFrotaPropria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repDespesaFrotaPropria.Deletar(despesaFrotaPropria, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

        #region Métodos Privados

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaDespesaFrotaPropria filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaDespesaFrotaPropria()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
            };

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 10, Models.Grid.Align.left, true);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

            Repositorio.Embarcador.Frota.DespesaFrotaPropria repDespesaFrotaPropria = new Repositorio.Embarcador.Frota.DespesaFrotaPropria(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria> despesaFrotaPropria = repDespesaFrotaPropria.Consultar(filtrosPesquisa, parametrosConsulta);
            grid.setarQuantidadeTotal(repDespesaFrotaPropria.ContarConsulta(filtrosPesquisa));

            var lista = (from despesa in despesaFrotaPropria
                         select new
                         {
                             Codigo = despesa.Codigo,
                             Filial = despesa.Filial?.Descricao ?? "",
                             Data = despesa.Data.ToString("dd/MM/yyyy") ?? DateTime.MinValue.ToString(),
                             Valor = despesa.Valor,
                             Observacao = despesa.Observacao,
                         }).ToList();

            grid.AdicionaRows(lista);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
                return new JsonpResult(grid);
        }

        private void PreencherDespesaFrotaPropria(Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria despesaFrotaPropria, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));

            despesaFrotaPropria.Filial = filial;
            despesaFrotaPropria.Data = Request.GetDateTimeParam("Data");
            despesaFrotaPropria.Valor = Utilidades.Decimal.Converter(Request.GetStringParam("Valor"));
            despesaFrotaPropria.Observacao = Request.GetStringParam("Observacao");

            if (despesaFrotaPropria.Filial == null)
                throw new ControllerException("Filial é obrigatória.");

            if (despesaFrotaPropria.Data == DateTime.MinValue)
                throw new ControllerException("Data é obrigatória");

            if (despesaFrotaPropria.Valor == 0)
                throw new ControllerException("Valor é obrigatório");

        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }
        #endregion
    }
}
