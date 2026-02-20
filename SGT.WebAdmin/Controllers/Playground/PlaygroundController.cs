using SGTAdmin.Controllers;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AdminMultisoftware.Dominio.Enumeradores;

/*
 * Controller para ser usado como teste para rotas. Não commite nada importante aqui.
 */
namespace SGT.WebAdmin.Controllers.Playground
{
    [CustomAuthorize("Playground/Playground")]
    
    public class PlaygroundController : BaseController
    {
		#region Construtores

		public PlaygroundController(Conexao conexao) : base(conexao) { }

		#endregion


        public async Task<IActionResult> Teste(int codigoCarga)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            //var serIntegracaoEmillenium = new Servicos.Embarcador.Integracao.Emillenium.IntegracaoEmilleniumBuscaPedidos(unitOfWork, TipoServicoMultisoftware);
            //serIntegracaoEmillenium.SalvarListaPedidos();
            //serIntegracaoEmillenium.IntegrarPedidos();

            var carga = repCarga.BuscarPorCodigo(codigoCarga);
            return new JsonpResult("Hello world");
        }

        /// <summary>
        /// Rota para teste de notificações de paradas
        /// </summary>
        public async Task<IActionResult> TesteMtrackNotificationCargaEntrega(int codigoCargaEntrega, MobileHubs metodo, int codigoClienteMultisoftware = -1)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            var cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
            var ser = new Servicos.Embarcador.Notificacao.NotificacaoMTrack(unitOfWork);

            ser.NotificarMudancaCargaEntrega(cargaEntrega, cargaEntrega.Carga.Motoristas.ToList(), metodo, true, codigoClienteMultisoftware != -1 ? codigoClienteMultisoftware : Empresa.Codigo);
            return new JsonpResult("Hello world");
        }

        /// <summary>
        /// Rota para teste de notificações de cargas
        /// </summary>
        public async Task<IActionResult> TesteMtrackNotificationCarga(int codigoCarga, MobileHubs metodo, int codigoClienteMultisoftware = -1)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            var carga = repCarga.BuscarPorCodigo(codigoCarga);
            var ser = new Servicos.Embarcador.Notificacao.NotificacaoMTrack(unitOfWork);

            ser.NotificarMudancaCarga(carga, carga.Motoristas.ToList(), metodo, true, codigoClienteMultisoftware != -1 ? codigoClienteMultisoftware : Empresa.Codigo);
            return new JsonpResult("Hello world");
        }

    }

}
