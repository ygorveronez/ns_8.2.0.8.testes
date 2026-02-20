using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize(new string[] { "Buscar" }, "Configuracoes/ConfiguracaoPreCarga")]
    public class ConfiguracaoPreCargaController : BaseController
    {
		#region Construtores

		public ConfiguracaoPreCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga repositorioConfiguracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = repositorioConfiguracaoPreCarga.BuscarPrimeiroRegistro();

                configuracaoPreCarga.Initialize();
                configuracaoPreCarga.AceitarVinculoFilaCarregamentoVeiculoAutomaticamente = Request.GetBoolParam("AceitarVinculoFilaCarregamentoVeiculoAutomaticamente");
                configuracaoPreCarga.VincularFilaCarregamentoVeiculoAutomaticamente = Request.GetBoolParam("VincularFilaCarregamentoVeiculoAutomaticamente");
                configuracaoPreCarga.VincularPrePlanoSemValidarModeloVeicularCarga = Request.GetBoolParam("VincularPrePlanoSemValidarModeloVeicularCarga");
                configuracaoPreCarga.DiasParaTransportadorAdicionarFilaCarregamentoVeiculo = Request.GetIntParam("DiasParaTransportadorAdicionarFilaCarregamentoVeiculo");
                configuracaoPreCarga.DiasTransicaoAutomaticaFilaCarregamentoVeiculo = Request.GetIntParam("DiasTransicaoAutomaticaFilaCarregamentoVeiculo");
                configuracaoPreCarga.TempoAguardarConfirmacaoTransportador = Request.GetIntParam("TempoAguardarConfirmacaoTransportador");

                repositorioConfiguracaoPreCarga.Atualizar(configuracaoPreCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a configuração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Buscar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga repositorioConfiguracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = repositorioConfiguracaoPreCarga.BuscarPrimeiroRegistro();

                return new JsonpResult(new
                {
                    DiasParaTransportadorAdicionarFilaCarregamentoVeiculo = (configuracaoPreCarga.DiasParaTransportadorAdicionarFilaCarregamentoVeiculo > 0) ? configuracaoPreCarga.DiasParaTransportadorAdicionarFilaCarregamentoVeiculo.ToString() : "",
                    DiasTransicaoAutomaticaFilaCarregamentoVeiculo = (configuracaoPreCarga.DiasTransicaoAutomaticaFilaCarregamentoVeiculo > 0) ? configuracaoPreCarga.DiasTransicaoAutomaticaFilaCarregamentoVeiculo.ToString() : "",
                    TempoAguardarConfirmacaoTransportador = (configuracaoPreCarga.TempoAguardarConfirmacaoTransportador > 0) ? configuracaoPreCarga.TempoAguardarConfirmacaoTransportador.ToString("n0") : "",
                    configuracaoPreCarga.AceitarVinculoFilaCarregamentoVeiculoAutomaticamente,
                    configuracaoPreCarga.VincularFilaCarregamentoVeiculoAutomaticamente,
                    configuracaoPreCarga.VincularPrePlanoSemValidarModeloVeicularCarga,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter a configuração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais
    }
}

