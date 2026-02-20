using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ConfiguracaoIntercab")]
    public class ConfiguracaoIntercabController : BaseController
    {
		#region Construtores

		public ConfiguracaoIntercabController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarIntegracaoCTeManual()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var retorno = false;

                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.SAP);

                if (integracaoIntercab == null)
                    return new JsonpResult(retorno);

                if (integracaoIntercab.AtivarIntegracaoCteManual)
                    retorno = true;
                else if (tipoIntegracao != null)
                    retorno = true;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarIntegracaoMDFeAquaviarioManual()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            
            {
                var retorno = false;

                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                if (integracaoIntercab == null)
                    return new JsonpResult(retorno);

                if (integracaoIntercab.AtivarIntegracaoMDFeAquaviario)
                    retorno = true;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
