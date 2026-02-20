using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Tipo_Anexo
{
    public class TipoAnexoController : BaseController
    {
		#region Construtores

		public TipoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.TipoAnexo repTipoAnexo = new Repositorio.Embarcador.Cargas.TipoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoAnexo tipoAnexo = new Dominio.Entidades.Embarcador.Cargas.TipoAnexo();

                PreencherTipoAnexo(tipoAnexo);

                repTipoAnexo.Inserir(tipoAnexo);
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
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");
                
                Repositorio.Embarcador.Cargas.TipoAnexo repTipoAnexo = new Repositorio.Embarcador.Cargas.TipoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoAnexo tipoAnexo = repTipoAnexo.BuscarPorCodigo(codigo,false);

                if (tipoAnexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherTipoAnexo(tipoAnexo);
                repTipoAnexo.Atualizar(tipoAnexo);

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
                Repositorio.Embarcador.Cargas.TipoAnexo repTipoAnexo = new Repositorio.Embarcador.Cargas.TipoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoAnexo tipoAnexo = repTipoAnexo.BuscarPorCodigo(codigo, false);

                if (tipoAnexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tipoAnexo.Codigo,
                    tipoAnexo.Descricao,
                    tipoAnexo.Status
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
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");
              
                Repositorio.Embarcador.Cargas.TipoAnexo repTipoAnexo = new Repositorio.Embarcador.Cargas.TipoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoAnexo tipoAnexo = repTipoAnexo.BuscarPorCodigo(codigo, false);

                if (tipoAnexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repTipoAnexo.Deletar(tipoAnexo);

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

        #region Metodos Privados
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.TipoAnexo repTipoAnexo = new Repositorio.Embarcador.Cargas.TipoAnexo(unitOfWork);

                string descricao = Request.GetStringParam("Descricao");
                bool status = Request.GetBoolParam("Status");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);
                
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta =  grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.Cargas.TipoAnexo> listTipoAnexos = repTipoAnexo.Consultar(descricao, status, parametroConsulta);
                int totalRegistros = repTipoAnexo.ContarConsulta(descricao, status);

                var listaMotivoPunicaoRetornar = (
                    from tipoAnexo in listTipoAnexos
                    select new
                    {
                        tipoAnexo.Codigo,
                        tipoAnexo.Descricao,
                        tipoAnexo.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoPunicaoRetornar);
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

        private void PreencherTipoAnexo(Dominio.Entidades.Embarcador.Cargas.TipoAnexo tipoAnexo)
        {
            tipoAnexo.Descricao = Request.GetStringParam("Descricao");
            tipoAnexo.Status = Request.GetBoolParam("Status");
        }
        #endregion

    }
}
