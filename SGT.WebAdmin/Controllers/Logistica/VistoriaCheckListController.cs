using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/VistoriaCheckList")]
    public class VistoriaCheckListController : BaseController
    {
		#region Construtores

		public VistoriaCheckListController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaVistoriaCheckList filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CheckList", "Checklist", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Modelo Veícular", "ModelosVeiculares", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Periodicidade", "PeriocidadeVencimento", 10, Models.Grid.Align.center, true);
                

                if (filtrosPesquisa.Status == true)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Logistica.VistoriaCheckList repVistoriaCheckList = new Repositorio.Embarcador.Logistica.VistoriaCheckList(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList> VistoriaCheckLists = repVistoriaCheckList.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repVistoriaCheckList.ContarConsulta(filtrosPesquisa));

                var lista = (from p in VistoriaCheckLists
                             select new
                             {
                                 p.Codigo,
                                 Checklist = p.Checklist?.Descricao ?? string.Empty ,
                                 ModelosVeiculares = string.Join(",", (from obj in p.ModelosVeiculares.ToList() select obj.Descricao).ToList()),
                                 p.PeriocidadeVencimento,
                                 p.DescricaoStatus
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.VistoriaCheckList repVistoriaCheckList = new Repositorio.Embarcador.Logistica.VistoriaCheckList(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList VistoriaCheckList = new Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList();

                PreencherVistoriaCheckList(VistoriaCheckList, unitOfWork);

                repVistoriaCheckList.Inserir(VistoriaCheckList);

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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.VistoriaCheckList repVistoriaCheckList = new Repositorio.Embarcador.Logistica.VistoriaCheckList(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList VistoriaCheckList = repVistoriaCheckList.BuscarPorCodigo(codigo, true);

                if (VistoriaCheckList == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherVistoriaCheckList(VistoriaCheckList, unitOfWork);

                repVistoriaCheckList.Atualizar(VistoriaCheckList);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.VistoriaCheckList repVistoriaCheckList = new Repositorio.Embarcador.Logistica.VistoriaCheckList(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList VistoriaCheckList = repVistoriaCheckList.BuscarPorCodigo(codigo, false);

                if (VistoriaCheckList == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynVistoriaCheckList = new
                {
                    VistoriaCheckList.Codigo,
                    Checklist = VistoriaCheckList.Checklist !=  null? new { VistoriaCheckList.Checklist.Descricao , VistoriaCheckList.Checklist?.Codigo  }: null,
                    VistoriaCheckList.PeriocidadeVencimento,
                    ModeloVeicular = (from obj in VistoriaCheckList.ModelosVeiculares select new { Codigo = obj.Codigo, Descricao = obj.Descricao}).ToList(),
                    VistoriaCheckList.Status,
                };

                return new JsonpResult(dynVistoriaCheckList);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.VistoriaCheckList repVistoriaCheckList = new Repositorio.Embarcador.Logistica.VistoriaCheckList(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList VistoriaCheckList = repVistoriaCheckList.BuscarPorCodigo(codigo, true);

                if (VistoriaCheckList == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repVistoriaCheckList.Deletar(VistoriaCheckList);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vinculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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

        private void PreencherVistoriaCheckList(Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList VistoriaCheckList, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkListTipo = repCheckListTipo.BuscarPorCodigo(Request.GetIntParam("Checklist"));
            
            if (checkListTipo == null)
                throw new Exception("A CheckList não pode ser nula");

            VistoriaCheckList.Checklist = checkListTipo;
            VistoriaCheckList.Status = Request.GetBoolParam("Status");
            VistoriaCheckList.PeriocidadeVencimento = Request.GetIntParam("PeriocidadeVencimento");
            PreencherModelosVeiculares(VistoriaCheckList, unitOfWork);
        }

        private void PreencherModelosVeiculares(Dominio.Entidades.Embarcador.Logistica.VistoriaCheckList vistoriaCheckList, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            

            dynamic codigosmodelos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModeloVeicular"));

            
            if (vistoriaCheckList.ModelosVeiculares != null && vistoriaCheckList.ModelosVeiculares.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic codigo in codigosmodelos)
                    if (codigo != null)
                        codigos.Add((int)codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> listamodeloVeicular = vistoriaCheckList.ModelosVeiculares.Where(o => !codigos.Contains(o.Codigo)).ToList();

                for (var i = 0; i < listamodeloVeicular.Count; i++)
                    vistoriaCheckList.ModelosVeiculares.Remove(listamodeloVeicular[i]);
            }

            if (vistoriaCheckList.ModelosVeiculares == null || vistoriaCheckList.ModelosVeiculares.Count == 0)
                vistoriaCheckList.ModelosVeiculares = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

            foreach (var codigoModeloVeicular in codigosmodelos)
            {
                if (vistoriaCheckList.ModelosVeiculares.Any(o => o.Codigo == (int)codigoModeloVeicular))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular= repModeloVeicularCarga.BuscarPorCodigo((int)codigoModeloVeicular, false);

                if (modeloVeicular != null)
                    vistoriaCheckList.ModelosVeiculares.Add(modeloVeicular);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaVistoriaCheckList ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaVistoriaCheckList()
            {
                CheckList = Request.GetIntParam("Checklist"),
                Status = Request.GetBoolParam("Status"),
                ModeloVeicular = Request.GetIntParam("ModeloVeicular"),
            };
        }

        #endregion
    }
}   
