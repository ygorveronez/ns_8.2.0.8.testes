using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/PosicaoDaFrotaMapa")]
    public class PosicaoDaFrotaMapaController : BaseController
    {
		#region Construtores

		public PosicaoDaFrotaMapaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            return null;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosMapa()
        {

            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            string PlacaVeiculo = Request.GetStringParam("PlacaVeiculo");

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoDaFrotaMapa> posicoesFrotaMapa;
                Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
                if (String.IsNullOrWhiteSpace(PlacaVeiculo))
                    posicoesFrotaMapa = repPosicaoAtual.BuscarTodosPosicaoDaFrota();
                else
                    posicoesFrotaMapa = repPosicaoAtual.BuscarPosicaoDaFrotaporPlaca(PlacaVeiculo);


                var retorno = (
                       from posicaoAtual in posicoesFrotaMapa
                       select
                    new
                    {
                        PlacaVeiculo = posicaoAtual?.Placa ?? "ID="+ posicaoAtual?.IDEquipamento.ToString(),
                        Latitude = posicaoAtual.Latitude,
                        Longitude = posicaoAtual.Longitude,
                        Descricao = posicaoAtual.Descricao,
                        DataPosicao = posicaoAtual.DataPosicao.ToString("dd/MM/yyyy hh:mm"),
                        Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicaoHelper.ObterDescricao(posicaoAtual.Status),
                        StatusCor = ObterStatusCor(posicaoAtual.Status)
                    }
                );


                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao obter dados do mapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        private string ObterStatusCor(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao status)
        {

            switch (status)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.SemViagem: return "#3498db";//Azul
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.EmViagem: return "#1abc9c";//Verde
                default: return "#95A5A6";
            }

        }

    }
}







