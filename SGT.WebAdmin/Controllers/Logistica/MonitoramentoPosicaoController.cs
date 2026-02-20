using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/MonitoramentoPosicao")]
    public class MonitoramentoPosicaoController : BaseController
    {
		#region Construtores

		public MonitoramentoPosicaoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);

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

                Servicos.Embarcador.Monitoramento.Monitoramento.AtualizarDadosPosicaoAtual(posicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.Todos, true, unitOfWork);

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


        private void PreencherDados(Dominio.Entidades.Embarcador.Logistica.Posicao reg, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            string idEquipamento = Request.GetStringParam("IDEquipamento");
            string placa = Request.GetStringParam("Placa");
            int codigoMobile = Request.GetIntParam("CodigoMobile");
            DateTime data = Request.GetDateTimeParam("Data");
            double latitude = Request.GetDoubleParam("Latitude");
            double longitude = Request.GetDoubleParam("Longitude");
            string local = Request.GetStringParam("Local");
            local = !string.IsNullOrEmpty(local) ? local : $"{latitude}, {longitude}";
            int velocidade = Request.GetIntParam("Velocidade");
            int temperatura = Request.GetIntParam("Temperatura");
            int ignicao = Request.GetIntParam("Ignicao");
            bool sensorTemperatura = Request.GetBoolParam("SensorTemperatura");

            Dominio.Entidades.Veiculo veiculo = null;

            if (!string.IsNullOrEmpty(idEquipamento))
                veiculo = repVeiculo.BuscarVeiculosPorRastreador(idEquipamento);
            else if (!string.IsNullOrEmpty(placa))
                veiculo = repVeiculo.BuscarPorPlaca(idEquipamento);
            else if (codigoMobile > 0)
                veiculo = repVeiculo.BuscarPorCodigoMobile(codigoMobile);

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


    }
}
