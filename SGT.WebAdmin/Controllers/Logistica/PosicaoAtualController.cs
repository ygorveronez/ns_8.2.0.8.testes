using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/PosicaoAtual")]
    public class PosicaoAtualController : BaseController
    {
		#region Construtores

		public PosicaoAtualController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Não foi possível encontrar a carga.");

                return new JsonpResult(ObterDadosPosicaoAtual(carga.Veiculo, unitOfWork));
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);

                return new JsonpResult(ObterDadosPosicaoAtual(veiculo, unitOfWork));
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.Posicao posicao = new Dominio.Entidades.Embarcador.Logistica.Posicao();

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

                AtualizarDadosPosicaoAtual(posicao, unitOfWork);

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

        #endregion Métodos Globais

        #region Métodos Privados

        private dynamic ObterDadosPosicaoAtual(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            if (veiculo == null)
                throw new ControllerException("Não foi possível encontrar o veículo.");

            Repositorio.Embarcador.Logistica.PosicaoAtual repositorioPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioPosicaoAtual.BuscarPorVeiculo(veiculo.Codigo);

            if ((posicaoAtual == null) || (posicaoAtual.Latitude == 0) || (posicaoAtual.Longitude == 0))
                throw new ControllerException("Não foi possível encontrar posição do veículo.");

            return new
            {
                posicaoAtual.Latitude,
                posicaoAtual.Longitude,
                UltimaAtualizacao = posicaoAtual.DataVeiculo.ToDateTimeString(showSeconds: true)
            };
        }

        private void AtualizarDadosPosicaoAtual(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posAtual = repPosicaoAtual.BuscarPorIDEquipamento(posicao.IDEquipamento);

            bool inserir = false;
            if (posAtual == null)
            {
                posAtual = new Dominio.Entidades.Embarcador.Logistica.PosicaoAtual();
                inserir = true;
            }

            posAtual.Data = posicao.Data;
            posAtual.DataVeiculo = posicao.DataVeiculo;
            posAtual.DataCadastro = DateTime.Now;
            posAtual.Descricao = posicao.Descricao;
            posAtual.IDEquipamento = posicao.IDEquipamento;
            posAtual.Latitude = posicao.Latitude;
            posAtual.Longitude = posicao.Longitude;
            posAtual.Velocidade = posicao.Velocidade;
            posAtual.Ignicao = posicao.Ignicao;
            posAtual.Veiculo = posicao.Veiculo;
            posAtual.Temperatura = posicao.Temperatura;
            posAtual.NivelBateria = posicao.NivelBateria;
            posAtual.NivelSinalGPS = posicao.NivelSinalGPS;
            //posAtual.Posicao = posicao;

            if (inserir == true)
                repPosicaoAtual.Inserir(posAtual);
            else
                repPosicaoAtual.Atualizar(posAtual);

        }

        private void PreencherDados(Dominio.Entidades.Embarcador.Logistica.Posicao reg, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            DateTime data = Request.GetDateTimeParam("Data");
            string local = Request.GetStringParam("Local");
            string idEquipamento = Request.GetStringParam("IDEquipamento");
            double latitude = Request.GetDoubleParam("Latitude");
            double longitude = Request.GetDoubleParam("Longitude");
            int velocidade = Request.GetIntParam("Velocidade");
            int temperatura = Request.GetIntParam("Temperatura");
            int ignicao = Request.GetIntParam("Ignicao");
            bool sensorTemperatura = Request.GetBoolParam("SensorTemperatura");

            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarVeiculosPorRastreador(idEquipamento);


            reg.Data = data;
            reg.DataVeiculo = data;
            reg.DataCadastro = DateTime.Now;
            reg.IDEquipamento = idEquipamento;
            reg.Veiculo = veiculo;
            reg.Velocidade = velocidade;
            reg.Temperatura = temperatura;
            reg.SensorTemperatura = sensorTemperatura;
            reg.Descricao = local;
            reg.Latitude = latitude;
            reg.Longitude = longitude;
            reg.Ignicao = ignicao;
        }

        #endregion Métodos Privados
    }
}
