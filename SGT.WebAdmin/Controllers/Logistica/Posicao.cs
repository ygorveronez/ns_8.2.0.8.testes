using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/Posicao")]
    public class PosicaoController : BaseController
    {
		#region Construtores

		public PosicaoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais


        private Dominio.Entidades.Embarcador.Logistica.PosicaoAtual InserirNovaPosicaoAtual(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual)
        {
            var novaPosicao = new Dominio.Entidades.Embarcador.Logistica.PosicaoAtual
            {
                Data = posicao.Data,
                DataVeiculo = posicao.Data,
                DataCadastro = DateTime.Now,
                IDEquipamento = posicao.IDEquipamento,
                Velocidade = posicao.Velocidade,
                Temperatura = posicao.Temperatura,
                SensorTemperatura = posicao.SensorTemperatura,
                Descricao = posicao.Descricao,
                Latitude = posicao.Latitude,
                Longitude = posicao.Longitude,
                Ignicao = posicao.Ignicao,
                Veiculo = posicao.Veiculo,
                Posicao = posicao
            };

            repPosicaoAtual.Inserir(novaPosicao);

            return novaPosicao;
        }

        private void AtualizarDadosPosicaoAtual(Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posAtual, Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao, Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual)
        {
            posAtual.Data = novaPosicao.Data;
            posAtual.DataVeiculo = novaPosicao.DataVeiculo;
            posAtual.DataCadastro = DateTime.Now;
            posAtual.Descricao = novaPosicao.Descricao;
            posAtual.IDEquipamento = novaPosicao.IDEquipamento;
            posAtual.Latitude = novaPosicao.Latitude;
            posAtual.Longitude = novaPosicao.Longitude;
            posAtual.Velocidade = novaPosicao.Velocidade;
            posAtual.Ignicao = novaPosicao.Ignicao;
            posAtual.Veiculo = novaPosicao.Veiculo;
            posAtual.Posicao = novaPosicao;
        }


        private void PreencherDados(Dominio.Entidades.Embarcador.Logistica.Posicao reg, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            DateTime data = Request.GetDateTimeParam("Data");
            string Local = Request.GetStringParam("Local");
            string idEquipamento = Request.GetStringParam("IDEquipamento");
            double Latitude = Request.GetDoubleParam("Latitude");
            double Longitude = Request.GetDoubleParam("Longitude");
            int Velocidade = Request.GetIntParam("Velocidade");
            int Temperatura = Request.GetIntParam("Temperatura");
            int Ignicao = Request.GetIntParam("Ignicao");
            bool SensorTemperatura = Request.GetBoolParam("SensorTemperatura");

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarVeiculosPorRastreador(idEquipamento);


            reg.Data = data;
            reg.DataVeiculo = data;
            reg.DataCadastro = DateTime.Now;
            reg.IDEquipamento = idEquipamento;
            reg.Veiculo = veiculo;
            reg.Velocidade = Velocidade;
            reg.Temperatura = Temperatura;
            reg.SensorTemperatura = SensorTemperatura;
            reg.Descricao = Local;
            reg.Latitude = Latitude;
            reg.Longitude = Longitude;
            reg.Ignicao = Ignicao;
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.Posicao posicao = new Dominio.Entidades.Embarcador.Logistica.Posicao();

                PreencherDados(posicao, unitOfWork);

                try
                {
                    PreencherDados(posicao, unitOfWork);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repPosicao.Inserir(posicao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion

    }
}
