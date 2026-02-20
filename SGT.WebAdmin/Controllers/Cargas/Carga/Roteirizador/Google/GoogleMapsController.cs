using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas.Carga.Roteirizador.Google
{
    [CustomAuthorize(new string[] { "BuscarDadosRoteirizacao" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class GoogleMapsController : BaseController
    {
		#region Construtores

		public GoogleMapsController(Conexao conexao) : base(conexao) { }

		#endregion


        public async Task<IActionResult> CriarRotaCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao)int.Parse(Request.Params("TipoUltimoPontoRoteirizacao"));
                string tipoRota = Request.Params("TipoRota");

                Servicos.Embarcador.Maps.Google.RoteirizarCarga serGoogleMaps = new Servicos.Embarcador.Maps.Google.RoteirizarCarga(unidadeTrabalho);


                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotaInformacao = serGoogleMaps.CriarRotaViaGoogleMaps(carga, cargaPedidos, tipoUltimoPontoRoteirizacao, tipoRota, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Criou Rotas Via GoogleMaps.", unidadeTrabalho);

                return new JsonpResult(rotaInformacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter a melhor rota.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
