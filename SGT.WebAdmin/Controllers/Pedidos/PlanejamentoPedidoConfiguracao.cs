
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/PlanejamentoPedidoConfiguracao")]
    public class PlanejamentoPedidoConfiguracaoController : BaseController
    {
		#region Construtores

		public PlanejamentoPedidoConfiguracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
       
        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                string email = Request.GetStringParam("Email");

                Repositorio.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao repPlanejamentoPedidoConfiguracao = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao planejamentoPedidoConfiguracao = repPlanejamentoPedidoConfiguracao.BuscarConfiguracao();

                if (planejamentoPedidoConfiguracao == null)
                {
                    planejamentoPedidoConfiguracao = new Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao();

                    planejamentoPedidoConfiguracao.Email = email;
                    repPlanejamentoPedidoConfiguracao.Inserir(planejamentoPedidoConfiguracao);
                }
                else
                {
                    planejamentoPedidoConfiguracao.Email = email;
                    repPlanejamentoPedidoConfiguracao.Atualizar(planejamentoPedidoConfiguracao);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao repPlanejamentoPedidoConfiguracao = new Repositorio.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao planejamentoPedidoConfiguracao = repPlanejamentoPedidoConfiguracao.BuscarConfiguracao();

                var dynConfig = new
                {
                    Codigo = planejamentoPedidoConfiguracao?.Codigo ?? 0,
                    Email = planejamentoPedidoConfiguracao?.Email ?? string.Empty
                    
                };

                return new JsonpResult(dynConfig);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        #endregion

        
    }
}
