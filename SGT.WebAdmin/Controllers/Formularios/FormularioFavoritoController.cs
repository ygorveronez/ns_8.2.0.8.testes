using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Formularios
{
    public class FormularioFavoritoController : BaseController
    {
		#region Construtores

		public FormularioFavoritoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Salvar(string path)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = null;

            try
            {
                Modulos modulos = new Modulos(_conexao);

                List<CacheFormulario> formulariosCache = modulos.RetornarFormulariosEmCache();

                CacheFormulario formularioCache = formulariosCache.Where(o => o.CaminhoFormulario == path).FirstOrDefault();

                bool favorito = false;

                if (formularioCache == null)
                    return new JsonpResult(false, true, "A página não foi encontrada para adicionar aos favoritos.");

                unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao); //está depois do módulo pois o módulo finaliza a sessão do admin internamente

                Repositorio.Embarcador.Usuarios.UsuarioFormularioFavorito repUsuarioFormularioFavorito = new Repositorio.Embarcador.Usuarios.UsuarioFormularioFavorito(unitOfWork);
                AdminMultisoftware.Repositorio.Modulos.Formulario repFormulario = new AdminMultisoftware.Repositorio.Modulos.Formulario(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario = repFormulario.BuscarPorCodigoFormulario(formularioCache.CodigoFormulario);
                Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito usuarioFormularioFavorito = repUsuarioFormularioFavorito.BuscarPorUsuarioECodigoFormulario(Usuario.Codigo, formularioCache.CodigoFormulario);

                if (usuarioFormularioFavorito != null)
                    repUsuarioFormularioFavorito.Deletar(usuarioFormularioFavorito);
                else
                {
                    favorito = true;

                    usuarioFormularioFavorito = new Dominio.Entidades.Embarcador.Usuarios.UsuarioFormularioFavorito()
                    {
                        CodigoFormulario = formularioCache.CodigoFormulario,
                        Usuario = Usuario
                    };

                    repUsuarioFormularioFavorito.Inserir(usuarioFormularioFavorito);
                }

                return new JsonpResult(new { Favorito = favorito, Formulario = new Dominio.ObjetosDeValor.Embarcador.Formulario.Formulario(formulario) });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o favorito.");
            }
            finally
            {
                unitOfWork.Dispose();
                unitOfWorkAdmin?.Dispose();
            }
        }
    }
}
