using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Sistema
{
    [CustomAuthorize("Sistema/Preferencias")]
    public class PreferenciasController : BaseController
    {
		#region Construtores

		public PreferenciasController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarPreferenciasGrid()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string url = Request.GetStringParam("urlGrid");
                string grid = Request.GetStringParam("idGrid");
                string dados = Request.GetStringParam("dadosGrid");

                if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(grid) || string.IsNullOrWhiteSpace(dados))
                    return new JsonpResult(false, "Parâmetros obrigatórios não informados");

                // Confirma a estrutura básica das preferências da grid
                Dominio.ObjetosDeValor.Grid.PreferenciaGrid preferencia = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Grid.PreferenciaGrid>(Request.Params("dadosGrid"));

                if (preferencia == null)
                    return new JsonpResult(false, "Dados em formato desconhecido");

                unitOfWork.Start();

                Repositorio.Embarcador.Preferencias.PreferenciaGrid repOSITORIOPreferenciaGrid = new Repositorio.Embarcador.Preferencias.PreferenciaGrid(unitOfWork);
                Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciaGridPorUsuario = repOSITORIOPreferenciaGrid.BuscarPorUsuarioUrlGrid(this.Usuario.Codigo, url, grid);

                if (preferenciaGridPorUsuario != null)
                {
                    preferenciaGridPorUsuario.Dados = dados;

                    repOSITORIOPreferenciaGrid.Atualizar(preferenciaGridPorUsuario);
                }
                else
                {
                    preferenciaGridPorUsuario = new Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid();

                    preferenciaGridPorUsuario.Usuario = this.Usuario;
                    preferenciaGridPorUsuario.URL = url;
                    preferenciaGridPorUsuario.Grid = grid;
                    preferenciaGridPorUsuario.Dados = dados;

                    repOSITORIOPreferenciaGrid.Inserir(preferenciaGridPorUsuario);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);              
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar as preferências do grid.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RestaurarPadraoGrid()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string url = Request.GetStringParam("urlGrid");
                string grid = Request.GetStringParam("idGrid");
                if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(grid)) return new JsonpResult(false, "Parâmetros obrigatórios não informados");

                Repositorio.Embarcador.Preferencias.PreferenciaGrid repPreferenciaGrid = new Repositorio.Embarcador.Preferencias.PreferenciaGrid(unitOfWork);
                Repositorio.Embarcador.Preferencias.ModeloGrid repositorioModeloGrid = new Repositorio.Embarcador.Preferencias.ModeloGrid(unitOfWork);

                Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciaGridPorUsuario = repPreferenciaGrid.BuscarPorUsuarioUrlGrid(this.Usuario.Codigo, url, grid);
                Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid modeloPadrao = repPreferenciaGrid.BuscarModeloPadraoPorUrlGrid(url, grid);

                unitOfWork.Start();

                if (preferenciaGridPorUsuario != null)
                    repPreferenciaGrid.Deletar(preferenciaGridPorUsuario);

                if (modeloPadrao != null)
				{
                    Dominio.Entidades.Embarcador.Preferencias.ModeloGrid modeloGridPadrao = modeloPadrao.ModeloGrid;
                    modeloGridPadrao.ModeloPadrao = false;
                    repositorioModeloGrid.Atualizar(modeloGridPadrao);
				}

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao restaurar o padrão do grid.");
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> BuscarModelosGrid()
		{
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

			try
			{
                Repositorio.Embarcador.Preferencias.PreferenciaGrid repositorioPreferenciaGrid = new Repositorio.Embarcador.Preferencias.PreferenciaGrid(unitOfWork);

                string url = Request.GetStringParam("UrlGrid");
                string grid = Request.GetStringParam("IdGrid");

                Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciaGridPorUsuario = repositorioPreferenciaGrid.BuscarPorUsuarioUrlGrid(this.Usuario.Codigo, url, grid);
                List<Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid> modelos = repositorioPreferenciaGrid.BuscarModeloPorUrlGrid(url, grid);

                dynamic retorno = (from modelo in modelos
                                   select new
                                   {
                                       modelo.Codigo,
                                       CodigoModelo = modelo.ModeloGrid?.Codigo ?? 0,
                                       DescricaoModelo = modelo.ModeloGrid?.Descricao ?? string.Empty,
                                       ModeloPadrao = modelo.ModeloGrid?.ModeloPadrao ?? false ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                                   }).ToList();

                return new JsonpResult(new
                {
                    PossuiConfiguraoPorUsuario = preferenciaGridPorUsuario != null ? true : false,
                    Modelos = retorno
                });
			}
            catch (Exception ex)
			{
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
			}
            finally
			{
                unitOfWork.Dispose();
			}
		}

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarModeloGrid()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Preferencias.PreferenciaGrid repositorioPreferenciaGrid = new Repositorio.Embarcador.Preferencias.PreferenciaGrid(unitOfWork);
                Repositorio.Embarcador.Preferencias.ModeloGrid repositorioModeloGrid = new Repositorio.Embarcador.Preferencias.ModeloGrid(unitOfWork);

                string url = Request.GetStringParam("URLGrid");
                string grid = Request.GetStringParam("IdGrid");
                string dados = Request.GetStringParam("DadosGrid");
                string descricao = Request.GetStringParam("Descricao");
                bool modeloPadrao = Request.GetBoolParam("ModeloPadrao");

                if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(grid) || string.IsNullOrWhiteSpace(dados))
                    return new JsonpResult(false, "Parâmetros obrigatórios não informados");

                // Confirma a estrutura básica das preferências da grid
                Dominio.ObjetosDeValor.Grid.PreferenciaGrid preferencia = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Grid.PreferenciaGrid>(Request.Params("DadosGrid"));

                if (preferencia == null)
                    return new JsonpResult(false, "Dados em formato desconhecido");

                unitOfWork.Start();

                // Quando o novo modelo for padrão remove o modelo padrão de outros modelos, caso haja
                if (modeloPadrao)
				{
                    Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid modeloGridPadrao = repositorioPreferenciaGrid.BuscarModeloPadraoPorUrlGrid(url, grid);
                    if (modeloGridPadrao != null)
					{
                        Dominio.Entidades.Embarcador.Preferencias.ModeloGrid modeloGrid = modeloGridPadrao.ModeloGrid;
                        modeloGrid.ModeloPadrao = false;
                        repositorioModeloGrid.Atualizar(modeloGrid);
					}
				}

                Dominio.Entidades.Embarcador.Preferencias.ModeloGrid modeloGridNovo = new Dominio.Entidades.Embarcador.Preferencias.ModeloGrid()
                {
                    Descricao = descricao,
                    ModeloPadrao = modeloPadrao,
                    UsuarioCadastro = this.Usuario,
                    DataCadastro = DateTime.Now
                };

                repositorioModeloGrid.Inserir(modeloGridNovo);

                Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciaGridNovo = new Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid()
                {
                    URL = url,
                    Grid = grid,
                    Dados = dados,
                    ModeloGrid = modeloGridNovo,
                };

                repositorioPreferenciaGrid.Inserir(preferenciaGridNovo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverModeloGrid()
		{
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Preferencias.PreferenciaGrid repositorioPreferenciaGrid = new Repositorio.Embarcador.Preferencias.PreferenciaGrid(unitOfWork);
                Repositorio.Embarcador.Preferencias.ModeloGrid repositorioModeloGrid = new Repositorio.Embarcador.Preferencias.ModeloGrid(unitOfWork);

                int codigoModelo = Request.GetIntParam("CodigoModelo");

                Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid modelo = repositorioPreferenciaGrid.BuscarPorCodigo(codigoModelo, false);
                
                if (modelo == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                Dominio.Entidades.Embarcador.Preferencias.ModeloGrid modeloGrid = modelo.ModeloGrid;

                unitOfWork.Start();

                repositorioPreferenciaGrid.Deletar(modelo);

                repositorioModeloGrid.Deletar(modeloGrid);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPreferenciasGrid()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoModelo = Request.GetIntParam("CodigoModelo", 0);
                string gridUrl = Request.GetStringParam("urlGrid");
                string gridId = Request.GetStringParam("idGrid");

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, gridUrl, gridId);
                Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid GridPreferencias = gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, codigoModelo);
                if (GridPreferencias != null)
                {
                    dynamic retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(GridPreferencias?.Dados);
                    return new JsonpResult(retorno);
                }

                return new JsonpResult(null);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion
    }
}
