using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento
{
    public class AlertaMonitor
    {

        #region Métodos públicos
        public void FinalizarAlertas(List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas,
                    DateTime? dataFim,
                    string observacaoFinalizacao,
                    Repositorio.UnitOfWork unitOfWork,
                    Dominio.Entidades.Usuario responsavel,
                    bool utilizaTratativa,
                    Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa eventoCausa,
                    Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao tratativaAcao,
                    bool tratativaAutomatica = false)
        {
            try
            {
                foreach (Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta in alertas)
                {
                    FinalizarAlerta(alerta,
                                    dataFim ?? DateTime.Now,
                                    observacaoFinalizacao,
                                    unitOfWork,
                                    responsavel,
                                    false,
                                    0,
                                    utilizaTratativa,
                                    eventoCausa,
                                    tratativaAcao,
                                    true,
                                    true,
                                    tratativaAutomatica);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "FinalizarAlertas");
            }
        }

        public void FinalizarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta,
            DateTime? dataFim,
            string observacaoFinalizacao,
            Repositorio.UnitOfWork unitOfWork,
            Dominio.Entidades.Usuario responsavel,
            bool reprogramarAlerta,
            int tempoReprogramacaoAlerta,
            bool utilizaTratativa,
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa eventoCausa,
            Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao tratativaAcao,
            bool finalizar = true,
            bool atualizarAcompanhamentoCarga = false,
            bool tratativaAutomatica = false)
        {
            if (alerta != null)
            {
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativa alertaTratativa;

                //Gerar ou Atualizar a tratativa do alerta.
                alertaTratativa = GerarOuAtualizarTratativa(unitOfWork, alerta, eventoCausa, tratativaAcao, responsavel, observacaoFinalizacao, dataFim ?? DateTime.Now);

                //Salvar observações e tratativas sem finalização.
                if (finalizar)
                {
                    alerta.DataFim = dataFim.HasValue ? dataFim : DateTime.Now;
                    alerta.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado;
                    alerta.Observacao = observacaoFinalizacao;
                    alerta.Responsavel = responsavel ?? alerta.Responsavel ?? null;
                    alerta.AlertaTrativaAutomatica = tratativaAutomatica;
                }

                if (reprogramarAlerta)
                {
                    alerta.AlertaReprogramado = reprogramarAlerta;
                    alerta.TempoReprogramado = tempoReprogramacaoAlerta;
                }

                repAlertaMonitor.Atualizar(alerta);

                if (atualizarAcompanhamentoCarga)
                    servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(alerta, null);
            }
        }

        public static string FormatarTextoParada(DateTime dataInicial, DateTime dataFinal)
        {
            return FormatarTextoParada(dataInicial, dataFinal, dataFinal - dataInicial);
        }

        public static string FormatarTextoParada(DateTime dataInicial, DateTime dataFinal, TimeSpan tempo)
        {
            return "De " + dataInicial.ToString("dd/MM/yyyy HH:mm") + " até " + dataFinal.ToString("dd/MM/yyyy HH:mm") + ": " + FormatarTempo(tempo);
        }

        public static string FormatarTempo(TimeSpan tempo)
        {
            string formato = String.Empty;
            if (tempo.Days > 0)
            {
                formato = $"{tempo.Days}d";
            }
            else if (tempo.Days < 0)
            {
                formato = $"{tempo.Days * -1}d";
            }
            formato += tempo.ToString(@"hh\:mm");
            return formato;
        }

        public static void AlterarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaAntiga != null && cargaNova != null)
            {
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = repAlertaMonitor.BuscarPorCarga(cargaAntiga.Codigo);
                int total = alertas.Count;
                for (int i = 0; i < total; i++)
                {
                    alertas[i].Carga = cargaNova;
                    repAlertaMonitor.Atualizar(alertas[i]);
                }
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Logistica.AlertaTratativa GerarOuAtualizarTratativa(
            Repositorio.UnitOfWork unitOfWork,
            Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta,
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa eventoCausa,
            Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao tratativaAcao,
            Dominio.Entidades.Usuario usuario,
            string observacao,
            DateTime data)
        {
            Repositorio.Embarcador.Logistica.AlertaTratativa repTratativa = new Repositorio.Embarcador.Logistica.AlertaTratativa(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AlertaTratativa alertaTratativa = repTratativa.BuscarPorAlerta(alerta.Codigo);

            bool inserirTratativa = alertaTratativa == null;
            if (inserirTratativa)
                alertaTratativa = new Dominio.Entidades.Embarcador.Logistica.AlertaTratativa();

            alertaTratativa.AlertaMonitor = alerta;
            alertaTratativa.Observacao = observacao;
            alertaTratativa.Data = data;
            alertaTratativa.Usuario = usuario;
            alertaTratativa.CausaAlertaTratativa = eventoCausa;
            alertaTratativa.AlertaTratativaAcao = tratativaAcao;

            if (inserirTratativa)
                repTratativa.Inserir(alertaTratativa);
            else
                repTratativa.Atualizar(alertaTratativa);

            return alertaTratativa;
        }
        #endregion
    }
}
