using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.OcultarInformacoesCarga
{
    [CustomAuthorize("Cargas/OcultarInformacoesCarga")]
    public class OcultarInformacoesCargaController : BaseController
    {
		#region Construtores

		public OcultarInformacoesCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga repositorio = new Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.FiltroPesquisaOcultarInformacoesCarga filtrosPesquisa = ObterFiltrosPesquisa();

                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga> listaRegistros = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga>();

                grid.AdicionaRows((
                     from o in listaRegistros
                     select new
                     {
                         o.Codigo,
                         o.Descricao,
                     }).ToList()
                 );

                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga repositorio = new Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga = new Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga();

                PreencherEntidade(ocultarInformacoesCarga, unitOfWork);
                SalvarUsuarios(ocultarInformacoesCarga, unitOfWork);
                SalvarPerfisAcesso(ocultarInformacoesCarga, unitOfWork);

                repositorio.Inserir(ocultarInformacoesCarga, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga repositorio = new Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga = repositorio.BuscarPorCodigo(codigo, true);

                unitOfWork.Start();

                PreencherEntidade(ocultarInformacoesCarga, unitOfWork);
                SalvarUsuarios(ocultarInformacoesCarga, unitOfWork);
                SalvarPerfisAcesso(ocultarInformacoesCarga, unitOfWork);

                repositorio.Atualizar(ocultarInformacoesCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga repositorio = new Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);

                int codigoOcultar = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga = repositorio.BuscarPorCodigo(codigoOcultar, true);

                if (ocultarInformacoesCarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                ocultarInformacoesCarga.Usuarios.Clear();
                ocultarInformacoesCarga.PerfisAcesso.Clear();
                repositorio.Deletar(ocultarInformacoesCarga, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga repositorio = new Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga = repositorio.BuscarPorCodigo(codigo, true);

                if (ocultarInformacoesCarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                dynamic dynOcultar = new
                {
                    Descricao = ocultarInformacoesCarga.Descricao,
                    ValorFrete = ocultarInformacoesCarga.ValorFrete,
                    Rota = ocultarInformacoesCarga.Rota,
                    ValorNotaFiscal = ocultarInformacoesCarga.ValorNotaFiscal,
                    ValorProduto = ocultarInformacoesCarga.ValorProduto,
                    VisualizarRota = ocultarInformacoesCarga.VisualizarRota,
                    Usuarios = (
                        from obj in ocultarInformacoesCarga.Usuarios
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao
                        }
                    ).ToList(),
                    PerfisAcesso = (
                        from obj in ocultarInformacoesCarga.PerfisAcesso
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao
                        }
                    ).ToList(),
                };

                return new JsonpResult(dynOcultar);
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

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga, Repositorio.UnitOfWork unitOfWork)
        {
            ocultarInformacoesCarga.Descricao = Request.GetStringParam("Descricao");
            ocultarInformacoesCarga.ValorFrete = Request.GetBoolParam("ValorFrete");
            ocultarInformacoesCarga.Rota = Request.GetBoolParam("Rota");
            ocultarInformacoesCarga.ValorNotaFiscal = Request.GetBoolParam("ValorNotaFiscal");
            ocultarInformacoesCarga.ValorProduto = Request.GetBoolParam("ValorProduto");
            ocultarInformacoesCarga.VisualizarRota = Request.GetBoolParam("VisualizarRota");
        }

        private void SalvarUsuarios(Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            dynamic dynUsuarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Usuarios"));

            List<int> codigosUsuarios = new List<int>();
            foreach (dynamic usuario in dynUsuarios)
            {
                int codigo = ((string)usuario.Codigo).ToInt();
                if (codigo > 0)
                    codigosUsuarios.Add(codigo);
            }

            if (ocultarInformacoesCarga.Usuarios == null)
                ocultarInformacoesCarga.Usuarios = new List<Dominio.Entidades.Usuario>();
            else
                ocultarInformacoesCarga.Usuarios.Clear();

            foreach (int codigoUsuario in codigosUsuarios)
                ocultarInformacoesCarga.Usuarios.Add(repUsuario.BuscarPorCodigo(codigoUsuario));
        }

        private void SalvarPerfisAcesso(Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(unitOfWork);

            dynamic dynPerfisAcesso = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PerfisAcesso"));

            List<int> codigosPerfisAcesso = new List<int>();
            foreach (dynamic perfilAcesso in dynPerfisAcesso)
            {
                int codigo = ((string)perfilAcesso.Codigo).ToInt();
                if (codigo > 0)
                    codigosPerfisAcesso.Add(codigo);
            }

            if (ocultarInformacoesCarga.PerfisAcesso == null)
                ocultarInformacoesCarga.PerfisAcesso = new List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso>();
            else
                ocultarInformacoesCarga.PerfisAcesso.Clear();

            foreach (int codigoPerfilAcesso in codigosPerfisAcesso)
                ocultarInformacoesCarga.PerfisAcesso.Add(repPerfilAcesso.BuscarPorCodigo(codigoPerfilAcesso));
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.FiltroPesquisaOcultarInformacoesCarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.FiltroPesquisaOcultarInformacoesCarga()
            {
                Descricao = Request.GetStringParam("Descricao"),
            };
        }

        #endregion
    }
}
