using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/ProvisaoManual")]
    public class ProvisaoManualController : BaseController
    {
		#region Construtores

		public ProvisaoManualController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.ProvisaoManual repProvisaoManual = new Repositorio.Embarcador.Escrituracao.ProvisaoManual(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual provisaoManual = new Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual();

                PreencherEntidade(provisaoManual, unitOfWork);

                unitOfWork.Start();

                repProvisaoManual.Inserir(provisaoManual, Auditado);

                unitOfWork.CommitChanges();


                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Escrituracao.ProvisaoManual repProvisaoManual = new Repositorio.Embarcador.Escrituracao.ProvisaoManual(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual provisaoManual = repProvisaoManual.BuscarPorCodigo(codigo, true);

                if (provisaoManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(provisaoManual, unitOfWork);

                unitOfWork.Start();

                repProvisaoManual.Atualizar(provisaoManual, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.Embarcador.Escrituracao.ProvisaoManual repProvisaoManual = new Repositorio.Embarcador.Escrituracao.ProvisaoManual(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual provisaoManual = repProvisaoManual.BuscarPorCodigo(codigo, false);

                if (provisaoManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    provisaoManual.Codigo,
                    CentroResultado = new { provisaoManual.CentroResultado.Codigo, provisaoManual.CentroResultado.Descricao },
                    Filial = new { provisaoManual.Filial.Codigo, provisaoManual.Filial.Descricao },
                    DataInicio = provisaoManual.DataInicio.ToString("dd/MM/yyyy"),
                    DataFim = provisaoManual.DataFim.ToString("dd/MM/yyyy"),
                    ValorProvisionado = provisaoManual.ValorProvisionado.ToString("n2"),
                    provisaoManual.Observacao
                });
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Escrituracao.ProvisaoManual repProvisaoManual = new Repositorio.Embarcador.Escrituracao.ProvisaoManual(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual provisaoManual = repProvisaoManual.BuscarPorCodigo(codigo, true);

                if (provisaoManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();


                repProvisaoManual.Deletar(provisaoManual, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

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
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual provisaoManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            provisaoManual.CentroResultado = repCentroResultado.BuscarPorCodigo(Request.GetIntParam("CentroResultado"));
            provisaoManual.Filial = repFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));
            provisaoManual.DataInicio = Request.GetDateTimeParam("DataInicio");
            provisaoManual.DataFim = Request.GetDateTimeParam("DataFim");
            provisaoManual.Observacao = Request.GetStringParam("Observacao");
            provisaoManual.ValorProvisionado = Request.GetDecimalParam("ValorProvisionado");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int centroResultado = Request.GetIntParam("CentroResultado");
                int filial = Request.GetIntParam("Filial");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Centro Resultado", "CentroResultado", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Filial", "Filial", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Inicio", "DataInicio", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Fim", "DataFim", 20, Models.Grid.Align.center, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Escrituracao.ProvisaoManual repProvisaoManual = new Repositorio.Embarcador.Escrituracao.ProvisaoManual(unitOfWork);

                List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual> listaProvisaoManual = repProvisaoManual.Consultar(centroResultado, filial, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repProvisaoManual.ContarConsulta(centroResultado, filial);

                var retorno = listaProvisaoManual.Select(obj => new
                {
                    obj.Codigo,
                    CentroResultado = obj.CentroResultado.Descricao,
                    Filial = obj.Filial.Descricao,
                    DataInicio = obj.DataInicio.ToString("dd/MM/yyyy"),
                    DataFim = obj.DataFim.ToString("dd/MM/yyyy")
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
