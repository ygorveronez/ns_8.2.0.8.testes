using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Roteirizador.MapRequest
{
    [CustomAuthorize(new string[] { "BuscarDadosRoteirizacao" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class MapRequestController : BaseController
    {
		#region Construtores

		public MapRequestController(Conexao conexao) : base(conexao) { }

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

                Servicos.Embarcador.Carga.RoteirizadorMapRequest serCargaRoteirizadorMapRequest = new Servicos.Embarcador.Carga.RoteirizadorMapRequest(unidadeTrabalho);


                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotaInformacao = serCargaRoteirizadorMapRequest.CriarRotaViaMapRequest(carga, cargaPedidos, tipoUltimoPontoRoteirizacao, tipoRota, unidadeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Criou Rotas Via MapRequest.", unidadeTrabalho);

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

        //public async Task<IActionResult> BuscarDadosRoteirizacao()
        //{
        //    Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {
        //        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
        //        Repositorio.Embarcador.Cargas.CargaRoteirizacao repCargaRoteirizacao = new Repositorio.Embarcador.Cargas.CargaRoteirizacao(unidadeTrabalho);
        //        Repositorio.Embarcador.Cargas.CargaRoteirizacaoClientesRota repCargaRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.CargaRoteirizacaoClientesRota(unidadeTrabalho);

        //        Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(_conexao.StringConexao);

        //        int codigoCarga;
        //        int.TryParse(Request.Params("Carga"), out codigoCarga);

        //        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

        //        Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao cargaRoteirizacao = repCargaRoteirizacao.BuscarPorCarga(carga.Codigo);

        //        if (cargaRoteirizacao != null)
        //        {
        //            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotasInformacaoPessoa = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa>();

        //            List<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota> cargaRoteirizacaoClientesRota = repCargaRoteirizacaoClientesRota.BuscarPorCargaRoteirizacao(cargaRoteirizacao.Codigo);
        //            foreach (Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota clientesRota in cargaRoteirizacaoClientesRota)
        //            {
        //                Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa rotaInformacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa();
        //                rotaInformacao.coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas();

        //                rotaInformacao.coordenadas.tipoLocalizacao = clientesRota.Cliente.TipoLocalizacao;
        //                rotaInformacao.pessoa = serPessoa.ConverterObjetoPessoa(clientesRota.Cliente);
        //                rotaInformacao.coordenadas.latitude = clientesRota.Cliente.Latitude;
        //                rotaInformacao.coordenadas.longitude = clientesRota.Cliente.Longitude;
        //                rotasInformacaoPessoa.Add(rotaInformacao);
        //            }

        //            var retorno = new
        //            {
        //                roteirizado = true,
        //                rotasInformacaoPessoa = rotasInformacaoPessoa,
        //                cargaRoteirizacao.DistanciaKM,
        //                cargaRoteirizacao.TipoRota,
        //                cargaRoteirizacao.TipoUltimoPontoRoteirizacao
        //            };

        //            return new JsonpResult(retorno);
        //        }
        //        else
        //        {
        //            var retorno = new
        //            {
        //                roteirizado = false
        //            };
        //            return new JsonpResult(retorno);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);

        //        return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados da Roteirização.");
        //    }
        //    finally
        //    {
        //        unidadeTrabalho.Dispose();
        //    }
        //}



    }
}
