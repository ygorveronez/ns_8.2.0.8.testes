using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoSemParar")]
    public class ConfiguracaoSemPararController : BaseController
    {
		#region Construtores

		public ConfiguracaoSemPararController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = repIntegracaoSemParar.Buscar();

                if (integracaoSemParar == null)
                    integracaoSemParar = repIntegracaoSemParar.BuscarPrimeira();

                var retorno = new {
                    TipoRotaSemParar = integracaoSemParar?.TipoRota
                }; 
                

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

    }
}
