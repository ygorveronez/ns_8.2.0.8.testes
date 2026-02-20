using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.OrdemServico
{
    [CustomAuthorize("Frota/TipoLocalManutencao")]
    public class TipoLocalManutencaoController : BaseController
    {
		#region Construtores

		public TipoLocalManutencaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao repTipoLocalManutencao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao tipoLocalManutencao = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao();

                PreencherEntidade(tipoLocalManutencao, unitOfWork);

                unitOfWork.Start();

                repTipoLocalManutencao.Inserir(tipoLocalManutencao, Auditado);

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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao repTipoLocalManutencao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao tipoLocalManutencao = repTipoLocalManutencao.BuscarPorCodigo(codigo, true);

                if (tipoLocalManutencao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(tipoLocalManutencao, unitOfWork);

                unitOfWork.Start();

                repTipoLocalManutencao.Atualizar(tipoLocalManutencao, Auditado);

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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao repTipoLocalManutencao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao tipoLocalManutencao = repTipoLocalManutencao.BuscarPorCodigo(codigo, false);

                if (tipoLocalManutencao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tipoLocalManutencao.Codigo,
                    tipoLocalManutencao.Descricao,
                    Situacao = tipoLocalManutencao.Ativo,
                    tipoLocalManutencao.Observacao,
                    tipoLocalManutencao.CodigoIntegracao,
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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao repTipoLocalManutencao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao tipoLocalManutencao = repTipoLocalManutencao.BuscarPorCodigo(codigo, true);

                if (tipoLocalManutencao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repTipoLocalManutencao.Deletar(tipoLocalManutencao, Auditado);

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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao tipoLocalManutencao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            bool ativo = Request.GetBoolParam("Situacao");
            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            tipoLocalManutencao.Ativo = ativo;
            tipoLocalManutencao.Descricao = descricao;
            tipoLocalManutencao.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tipoLocalManutencao.Observacao = observacao;
            tipoLocalManutencao.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao reptipoLocalManutencao = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipoLocalManutencao> listaGrupoDespesa = reptipoLocalManutencao.Consultar(descricao, status, codigoEmpresa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = reptipoLocalManutencao.ContarConsulta(descricao, status, codigoEmpresa);

                var retorno = (from motivo in listaGrupoDespesa
                               select new
                               {
                                   motivo.Codigo,
                                   motivo.Descricao,
                                   motivo.DescricaoAtivo
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
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
