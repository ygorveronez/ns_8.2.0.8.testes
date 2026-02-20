using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/SetorFuncionario")]
    public class SetorFuncionarioController : BaseController
    {
		#region Construtores

		public SetorFuncionarioController(Conexao conexao) : base(conexao) { }

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
                PropOrdena(ref propOrdenar);

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Setor setor = repSetor.BuscarPorCodigo(codigo);

                if (setor == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.SetorFuncionario.NaoFoiPossivelEncontrar);

                var retorno = new
                {
                    setor.Codigo,
                    setor.Descricao,
                    Status = setor.Status == "A",
                    TipoCargoFuncionario = setor.TipoCargoFuncionario != null ? setor.TipoCargoFuncionario.Select(o => o).ToList() : null,
                    setor.PermitirAssumirChamadosDoMesmoSetor,
                    setor.PermitirCancelarAtendimento,
                    setor.TipoSetorFuncionario,
                    TipoGrot = setor.Checklist == null ? null : new { setor.Checklist.Codigo, setor.Checklist.Descricao },
                    setor.NotificarCenarioPosEntregaImprocedenteGestaoDevolucao
            };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.SetorFuncionario.OcorreuFalhaConsultar);
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

                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

                Dominio.Entidades.Setor setor = new Dominio.Entidades.Setor();

                PreencheEntidade(setor, unitOfWork);

                if (!ValidaEntidade(setor, out string erro))
                    return new JsonpResult(false, true, erro);

                repSetor.Inserir(setor, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.SetorFuncionario.OcorreuFalhaAdicionar);
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

                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Setor setor = repSetor.BuscarPorCodigo(codigo, true);

                if (setor == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.SetorFuncionario.NaoFoiPossivelEncontrar);

                PreencheEntidade(setor, unitOfWork);

                if (!ValidaEntidade(setor, out string erro))
                    return new JsonpResult(false, true, erro);

                repSetor.Atualizar(setor, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Pessoas.SetorFuncionario.OcorreuFalhaAdicionar);
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
                unitOfWork.Start();

                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Setor setor = repSetor.BuscarPorCodigo(codigo, true);

                if (setor == null)
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.SetorFuncionario.NaoFoiPossivelEncontrar);

                repSetor.Deletar(setor, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Pessoas.SetorFuncionario.NaoFoiPossivelExcluirJaPossuiVinculo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Pessoas.SetorFuncionario.OcorreuFalhaExcluir);
                }
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
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Descricao").Nome(Localization.Resources.Gerais.Geral.Descricao).Tamanho(55).Align(Models.Grid.Align.left);
            if (Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Status") == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                grid.Prop("Status").Nome(Localization.Resources.Gerais.Geral.Situacao).Tamanho(25).Align(Models.Grid.Align.left);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Setor repositorio = new Repositorio.Setor(unitOfWork);

            string descricao = Request.Params("Descricao");
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoIrregularidade = Request.GetIntParam("CodigoIrregularidade");
            List<int> setores = BuscarSetoresIrregularidade(codigoIrregularidade, unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Status");
            List<Dominio.Entidades.Setor> listaSetores = repositorio.Consultar(descricao, status, codigoFilial, setores, propOrdenar, dirOrdena, inicio, limite);

            totalRegistros = repositorio.ContarConsulta(descricao, status, codigoFilial, setores);

            return (
                from setor in listaSetores
                select new
                {
                    setor.Codigo,
                    setor.Descricao,
                    Status = setor.DescricaoStatus
                }
            ).ToList();
        }

        private void PreencheEntidade(Dominio.Entidades.Setor setor, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Checklist.Checklist repChecklistGrot = new Repositorio.Embarcador.Checklist.Checklist(unitOfWork);

            string descricao = Request.GetStringParam("Descricao");
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargoFuncionario> tipoCargoFuncionario = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargoFuncionario>("TipoCargoFuncionario");
            bool ativo = Request.GetBoolParam("Status");

            int tipoGrot = Request.GetIntParam("TipoGrot");
            if (tipoGrot > 0)
                setor.Checklist = repChecklistGrot.BuscarPorCodigo(tipoGrot, false);

            setor.Descricao = descricao;
            setor.Status = ativo ? "A" : "I";

            if (setor.TipoCargoFuncionario == null)
                setor.TipoCargoFuncionario = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargoFuncionario>();
            else
                setor.TipoCargoFuncionario.Clear();

            setor.PermitirAssumirChamadosDoMesmoSetor = Request.GetBoolParam("PermitirAssumirChamadosDoMesmoSetor");
            setor.PermitirCancelarAtendimento = Request.GetBoolParam("PermitirCancelarAtendimento");
            setor.TipoSetorFuncionario = Request.GetEnumParam<TipoSetorFuncionario>("TipoSetorFuncionario");
            setor.NotificarCenarioPosEntregaImprocedenteGestaoDevolucao = Request.GetBoolParam("NotificarCenarioPosEntregaImprocedenteGestaoDevolucao");

            if (tipoCargoFuncionario != null)
            {
                foreach (var tipo in tipoCargoFuncionario)
                {
                    setor.TipoCargoFuncionario.Add(tipo);
                }
            }
        }

        private bool ValidaEntidade(Dominio.Entidades.Setor setor, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(setor.Descricao))
            {
                msgErro = Localization.Resources.Pessoas.SetorFuncionario.DescricaoObrigatoria;
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
        }

        private List<int> BuscarSetoresIrregularidade(int codigoIrregularidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade repTratativa = new Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade(unitOfWork);

            return repTratativa.BuscarCodigosSetoresPorIrregularidade(codigoIrregularidade);
        }

        #endregion
    }
}
