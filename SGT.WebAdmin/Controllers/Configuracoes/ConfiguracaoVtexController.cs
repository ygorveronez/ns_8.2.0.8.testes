using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize("Configuracoes/ConfiguracaoVtex")]
    public class ConfiguracaoVtexController : BaseController
    {
		#region Construtores

		public ConfiguracaoVtexController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> VerificarExisteIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoVtex filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoVtex();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);
            int totalRegistros = repConfiguracaoVtex.ContarConsulta(filtrosPesquisa);

            return new JsonpResult(new
            {
                temIntegracao = totalRegistros > 0
            });
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex ConfiguracaoVtex = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex();

                PreencherEntidade(ConfiguracaoVtex, unitOfWork);
                ValidarEntidade(ConfiguracaoVtex, unitOfWork);

                repConfiguracaoVtex.Inserir(ConfiguracaoVtex, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
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

                Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex ConfiguracaoVtex = repConfiguracaoVtex.BuscarPorCodigo(codigo, true);

                if (ConfiguracaoVtex == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                PreencherEntidade(ConfiguracaoVtex, unitOfWork);
                ValidarEntidade(ConfiguracaoVtex, unitOfWork);

                repConfiguracaoVtex.Atualizar(ConfiguracaoVtex, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar os dados.");
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

                Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex ConfiguracaoVtex = repConfiguracaoVtex.BuscarPorCodigo(codigo, true);

                if (ConfiguracaoVtex == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                repConfiguracaoVtex.Deletar(ConfiguracaoVtex, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
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
                Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex ConfiguracaoVtex = repConfiguracaoVtex.BuscarPorCodigo(codigo, false);

                if (ConfiguracaoVtex == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                return new JsonpResult(new
                {
                    ConfiguracaoVtex.Codigo,
                    Filial = ConfiguracaoVtex.Filial == null ? null : new
                    {
                        ConfiguracaoVtex.Filial.Codigo,
                        ConfiguracaoVtex.Filial.Descricao,
                    },
                    ConfiguracaoVtex.Situacao,
                    ConfiguracaoVtex.AccountName,
                    ConfiguracaoVtex.Environment,
                    ConfiguracaoVtex.XVtexApiAppToken,
                    ConfiguracaoVtex.XVtexApiAppKey,
                    ConfiguracaoVtex.QuantidadeNotificacao,
                    ConfiguracaoVtex.EmailsNotificacao,

                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar registro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Filial", "Filial", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 25, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoVtex filtrosPesquisa = ObterFiltrosPesquisa();

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex> listaConfiguracores = repConfiguracaoVtex.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repConfiguracaoVtex.ContarConsulta(filtrosPesquisa);

                var retorno = (from configuracao in listaConfiguracores
                               select new
                               {
                                   configuracao.Codigo,
                                   Filial = configuracao.Filial?.Descricao ?? string.Empty,
                                   Situacao = configuracao.DescricaoSituacao
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoVtex ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoVtex()
            {
                Filial = Request.GetIntParam("Filial"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex ConfiguracaoVtex, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            ConfiguracaoVtex.Filial = repFilial.BuscarPorCodigo(Request.GetIntParam("Filial"));
            ConfiguracaoVtex.Situacao = Request.GetBoolParam("Situacao");
            ConfiguracaoVtex.AccountName = Request.GetStringParam("AccountName");
            ConfiguracaoVtex.Environment = Request.GetStringParam("Environment");
            ConfiguracaoVtex.XVtexApiAppToken = Request.GetStringParam("XVtexApiAppToken");
            ConfiguracaoVtex.XVtexApiAppKey = Request.GetStringParam("XVtexApiAppKey");
            ConfiguracaoVtex.QuantidadeNotificacao = Request.GetIntParam("QuantidadeNotificacao");
            ConfiguracaoVtex.EmailsNotificacao = Request.GetStringParam("EmailsNotificacao");
        }

        private void ValidarEntidade(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVtex ConfiguracaoVtex, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex repConfiguracaoVtex = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVtex(unitOfWork);

            var configDuplicada = repConfiguracaoVtex.BuscarConfiguracaoDuplicada(ConfiguracaoVtex);
            if (ConfiguracaoVtex.Situacao && configDuplicada != null)
                throw new ControllerException("Já existe uma configuração VTEX com os dados informados.");
        }

        #endregion
    }
}
