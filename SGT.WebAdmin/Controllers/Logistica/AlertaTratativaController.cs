using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AlertaTratativa", "Logistica/Monitoramento", "Cargas/ControleEntrega", "TorreControle/AcompanhamentoCarga", "Cargas/AlertasTransportador", "Logistica/MonitoramentoNovo")]
    public class AlertaTratativaController : BaseController
    {
		#region Construtores

		public AlertaTratativaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool utilizaTratativa = Request.GetBoolParam("UtilizaTratativa");
                int codigoAlerta = Request.GetIntParam("CodigoAlerta");
                string observacao = Request.GetStringParam("Observacao");
                int tratativaAcao = Request.GetIntParam("Tratativa");
                int causa = Request.GetIntParam("Causa");
                bool reprogramarAlerta = Request.GetBoolParam("ReprogramarAlerta");
                bool tratarTodosDoMesmoTipo = Request.GetBoolParam("TratarTodosDoMesmoTipo");
                TimeSpan tempoReprogramacaoAlerta = Request.GetTimeParam("TempoReprogramacaoAlerta");
                bool salvarTratativa = Request.GetBoolParam("SalvarTratativa");

                if (utilizaTratativa && !salvarTratativa)
                {
                    if (observacao.Length > 500)
                        return new JsonpResult(false, true, "Descrição não pode passar de 500 caracteres.");

                    if (tratativaAcao == 0)
                        return new JsonpResult(false, true, "Uma tratativa deve ser informada.");
                }

                Repositorio.Embarcador.Logistica.AlertaTratativa repTratativa = new Repositorio.Embarcador.Logistica.AlertaTratativa(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaTratativaAcao repAlertaTratativaAcao = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                Repositorio.Embarcador.Logistica.MonitoramentoEventoCausa repositorioMonitoramentoEventoCausa = new Repositorio.Embarcador.Logistica.MonitoramentoEventoCausa(unitOfWork);

                Servicos.Embarcador.Monitoramento.AlertaMonitor serAlertaMonitor = new Servicos.Embarcador.Monitoramento.AlertaMonitor();

                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alertaSelecionado = repAlertaMonitor.BuscarPorCodigo(codigoAlerta);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa eventoCausa = repositorioMonitoramentoEventoCausa.BuscarPorCodigo(causa, false);
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao alertaTratativaAcao = repAlertaTratativaAcao.BuscarPorCodigo(tratativaAcao);
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativa alertaTratativa;

                if (alertaSelecionado == null) return new JsonpResult(false, "Alerta não encontrado.");
                if (alertaSelecionado.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado) return new JsonpResult(false, "Alerta já finalizado.");

                List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = null;
                if (tratarTodosDoMesmoTipo && !salvarTratativa)
                    alertas = repAlertaMonitor.BuscarEmAbertoPorCargaeTipoAlertaEResponsavel(alertaSelecionado.Carga.Codigo, alertaSelecionado.TipoAlerta, this.Usuario.Codigo);

                try
                {
                    unitOfWork.Start();

                    DateTime data = DateTime.Now;

                    serAlertaMonitor.FinalizarAlerta(
                        alertaSelecionado,
                        data,
                        observacao,
                        unitOfWork,
                        this.Usuario,
                        reprogramarAlerta,
                        Convert.ToInt32(tempoReprogramacaoAlerta.TotalMinutes),
                        utilizaTratativa,
                        eventoCausa,
                        alertaTratativaAcao,
                        !salvarTratativa,
                        true);

                    unitOfWork.CommitChanges();
                    alertaTratativa = repTratativa.BuscarPorAlerta(codigoAlerta);

                    if (alertas?.Count > 0)
                    {
                        if (alertas.Contains(alertaSelecionado))
                            alertas.Remove(alertaSelecionado);
                        serAlertaMonitor.FinalizarAlertas(alertas, data, observacao + $" (Finalizado em massa via alerta {alertaSelecionado.Codigo})", unitOfWork, this.Usuario, true, eventoCausa, alertaTratativaAcao);
                    }
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }

                var retorno = new
                {
                    CodigoAlerta = alertaSelecionado.Codigo,
                    alertaSelecionado.Responsavel.Codigo,
                    DescricaoStatus = alertaSelecionado.Status.ObterDescricao() ?? "-",
                    EnumStatus = alertaSelecionado.Status,
                    ImagemStatus = alertaSelecionado.Status.ObterImagemStatus(),
                    TipoAlerta = alertaSelecionado.TipoAlerta.ObterDescricao() ?? "-",
                    Tipo = alertaSelecionado.TipoAlerta,
                    DataInicio = alertaSelecionado.Data.ToString("dd/MM/yyyy HH:mm:ss") ?? "-",
                    DataFim = alertaSelecionado.DataFim?.ToString("dd/MM/yyyy HH:mm:ss") ?? "-",
                    Responsavel = alertaSelecionado.Responsavel?.Nome ?? "-",
                    Latitude = alertaSelecionado.Latitude?.ToString() ?? string.Empty,
                    Longitude = alertaSelecionado.Longitude?.ToString() ?? string.Empty,
                    Tratativa = alertaTratativa?.AlertaTratativaAcao?.Descricao ?? string.Empty,
                    CodigoTratativa = alertaTratativa?.AlertaTratativaAcao?.Codigo ?? 0,
                    Causa = alertaTratativa?.CausaAlertaTratativa?.Descricao ?? string.Empty,
                    CodigoCausa = alertaTratativa?.CausaAlertaTratativa?.Codigo ?? 0,
                    Observacao = alertaTratativa?.Observacao ?? string.Empty,
                    ResolvidoPor = alertaTratativa?.Usuario?.Nome ?? string.Empty,
                    ReprogramarAlerta = alertaSelecionado?.AlertaReprogramado ?? false,
                    TempoReprogramacaoAlerta = alertaSelecionado?.TempoReprogramado,
                };

                return new JsonpResult(retorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AlertaTratativa repositorio = new Repositorio.Embarcador.Logistica.AlertaTratativa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativa reg = repositorio.BuscarPorCodigo(codigo);

                if (reg == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    reg.Codigo,
                    reg.Data,
                    reg.Observacao

                });
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


        #endregion

        #region Métodos 
        #endregion
    }
}
