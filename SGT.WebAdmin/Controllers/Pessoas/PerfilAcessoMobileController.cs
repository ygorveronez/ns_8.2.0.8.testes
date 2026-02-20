using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/PerfilAcessoMobile")]
    public class PerfilAcessoMobileController : BaseController
    {
		#region Construtores

		public PerfilAcessoMobileController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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

        /// <summary>
        /// Usado na tela de usuário para buscar o perfil de permissões
        /// </summary>
        /// <returns></returns>
        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorPerfilMobile()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Usuarios.PerfilAcessoMobile repPerfilAcessoMobile = new Repositorio.Embarcador.Usuarios.PerfilAcessoMobile(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile perfilAcessoMobile = repPerfilAcessoMobile.BuscarPorCodigo(codigo, false);

                if (perfilAcessoMobile == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o perfil de acesso mobile.");

                var retorno = new
                {
                    UsuarioAdministradorMobile = perfilAcessoMobile.PerfilAdministrador,
                    ModulosUsuarioMobile = (from codigoModulo in perfilAcessoMobile.ModulosLiberados
                                            select new
                                            {
                                                CodigoModulo = codigoModulo
                                            }).ToList()
                };

                return new JsonpResult(retorno);
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

                Repositorio.Embarcador.Usuarios.PerfilAcessoMobile repPerfilAcessoMobile = new Repositorio.Embarcador.Usuarios.PerfilAcessoMobile(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile perfilAcessoMobile = new Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile();

                PreencheEntidade(ref perfilAcessoMobile, unitOfWork);

                string erro;
                if (!ValidaEntidade(perfilAcessoMobile, out erro))
                    return new JsonpResult(false, true, erro);

                repPerfilAcessoMobile.Inserir(perfilAcessoMobile, Auditado);

                PreencheFormularios(perfilAcessoMobile, unitOfWork);

                unitOfWork.CommitChanges();

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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Usuarios.PerfilAcessoMobile repPerfilAcessoMobile = new Repositorio.Embarcador.Usuarios.PerfilAcessoMobile(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile perfilAcessoMobile = repPerfilAcessoMobile.BuscarPorCodigo(codigo, true);

                if (perfilAcessoMobile == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencheEntidade(ref perfilAcessoMobile, unitOfWork);

                string erro;
                if (!ValidaEntidade(perfilAcessoMobile, out erro))
                    return new JsonpResult(false, true, erro);

                repPerfilAcessoMobile.Atualizar(perfilAcessoMobile, Auditado);

                PreencheFormularios(perfilAcessoMobile, unitOfWork);

                unitOfWork.CommitChanges();

                unitOfWork.FlushAndClear();

                unitOfWork.Start();

                // Atualiza todos os usuários com esse perfil
                List<Dominio.Entidades.Usuario> usuarios = repUsuario.BuscarPorPerfilEmbarcadorMobile(perfilAcessoMobile.Codigo);
                foreach (Dominio.Entidades.Usuario usuario in usuarios)
                {
                    usuario.UsuarioAdministradorMobile = perfilAcessoMobile.PerfilAdministrador;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, usuario, null, "Atualizou o perfil de acesso mobile " + perfilAcessoMobile.Descricao + " .", unitOfWork);
                    // Atualiza modulos
                    usuario.ModulosLiberadosMobile.Clear();
                    foreach (int codigoModulo in perfilAcessoMobile.ModulosLiberados)
                        usuario.ModulosLiberadosMobile.Add(codigoModulo);

                    repUsuario.Atualizar(usuario);
                }

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Usuarios.PerfilAcessoMobile repPerfilAcessoMobile = new Repositorio.Embarcador.Usuarios.PerfilAcessoMobile(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile perfilAcessoMobile = repPerfilAcessoMobile.BuscarPorCodigo(codigo, false);

                if (perfilAcessoMobile == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    perfilAcessoMobile.Codigo,
                    perfilAcessoMobile.Descricao,
                    perfilAcessoMobile.PerfilAdministrador,
                    Status = perfilAcessoMobile.Ativo,
                    ModulosPerfilMobile = (from codigoModulo in perfilAcessoMobile.ModulosLiberados
                                           select new
                                           {
                                               CodigoModulo = codigoModulo
                                           }).ToList()
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Usuarios.PerfilAcessoMobile repPerfilAcessoMobile = new Repositorio.Embarcador.Usuarios.PerfilAcessoMobile(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile perfilAcessoMobile = repPerfilAcessoMobile.BuscarPorCodigo(codigo, true);

                if (perfilAcessoMobile == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repPerfilAcessoMobile.Deletar(perfilAcessoMobile, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile perfilAcessoMobile, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            bool ativo = false;
            bool.TryParse(Request.Params("Status"), out ativo);

            bool perfilAdministrador = false;
            bool.TryParse(Request.Params("PerfilAdministrador"), out perfilAdministrador);

            perfilAcessoMobile.Descricao = descricao;
            perfilAcessoMobile.Ativo = ativo;
            perfilAcessoMobile.PerfilAdministrador = perfilAdministrador;
        }

        private void PreencheFormularios(Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile perfilAcessoMobile, Repositorio.UnitOfWork unitOfWork)
        {
            // Vincula Módulos
            perfilAcessoMobile.ModulosLiberados = new List<int>();
            var jModulosPerfil = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModulosPerfilMobile"));
            foreach (var jModulo in jModulosPerfil)
            {
                perfilAcessoMobile.ModulosLiberados.Add((int)jModulo.CodigoModulo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, perfilAcessoMobile, null, "Módulo " + (string)jModulo.CodigoModulo + " adicionado ao perfil mobile.", unitOfWork);
            }
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile perfilAcessoMobile, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(perfilAcessoMobile.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.PerfilAcessoMobile repPerfilAcessoMobile = new Repositorio.Embarcador.Usuarios.PerfilAcessoMobile(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
            if (!string.IsNullOrWhiteSpace(Request.Params("Ativo")))
                Enum.TryParse(Request.Params("Ativo"), out status);
            else
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            string descricao = Request.Params("Descricao");

            List<Dominio.Entidades.Embarcador.Usuarios.PerfilAcessoMobile> listaGrid = repPerfilAcessoMobile.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repPerfilAcessoMobile.ContarConsulta(descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Ativo, "DescricaoAtivo", 20, Models.Grid.Align.left, true);

            return grid;
        }

        #endregion
    }
}
