using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/ConfiguracaoProvisao")]
    public class ConfiguracaoProvisaoController : BaseController
    {
		#region Construtores

		public ConfiguracaoProvisaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.ConfiguracaoProvisao repositorioConfiguracao = new Repositorio.Embarcador.Escrituracao.ConfiguracaoProvisao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao configuracao = repositorioConfiguracao.BuscarConfiguracao();

                if (configuracao == null)
                    configuracao = new Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao();
                else
                    configuracao.Initialize();

                PreencherConfiguracaoProvisao(configuracao);

                unitOfWork.Start();

                if (configuracao.IsInitialized())
                    repositorioConfiguracao.Atualizar(configuracao, Auditado);
                else
                    repositorioConfiguracao.Inserir(configuracao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(true, false, "Ocorreu uma falha ao salvar a configuração de provisão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.ConfiguracaoProvisao repositorioConfiguracao = new Repositorio.Embarcador.Escrituracao.ConfiguracaoProvisao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao configuracao = repositorioConfiguracao.BuscarConfiguracao();

                if (configuracao == null)
                    return new JsonpResult(true);

                return new JsonpResult(new
                {
                    configuracao.DiasForaMes
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os as configurações de provisão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherConfiguracaoProvisao(Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao configuracao)
        {
            configuracao.DiasForaMes = Request.GetIntParam("DiasForaMes");
        }

        #endregion
    }
}
