using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class PermanenciaNoPontoApoio : AbstractEvento
    {

        #region Métodos públicos

        public PermanenciaNoPontoApoio(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PermanenciaNoPontoApoio)
        {

        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.PontoApoio != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Tempo != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.TempoEvento != null);
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {
            if (monitoramentoProcessarEvento != null && monitoramento != null && monitoramento.Carga != null && this.MonitoramentoEvento?.Gatilho?.PontosDeApoio != null)
            {
                List<int> codigosPermanenciaLocais = this.MonitoramentoEvento.Gatilho.PontosDeApoio.Select(p => p.Codigo).ToList();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal> permanenciaLocais = this.PermanenciaLocais?.Where(p => p.CodigoCarga == monitoramento.Carga.Codigo && codigosPermanenciaLocais.Contains(p.CodigoLocal)).ToList() ?? new();

                foreach(Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal permanenciaAbertaNoLocal in permanenciaLocais)
                {
                    if (monitoramentoProcessarEvento.DataVeiculoPosicao.HasValue && monitoramentoProcessarEvento.DataVeiculoPosicao.Value > permanenciaAbertaNoLocal.DataInicio)
                    {
                        Log.TratarErro($"1 - Processando Permanencia: {permanenciaAbertaNoLocal.CodigoPermanencia} ", "PermanenciaPontoApoio");
                        TimeSpan tempo = monitoramentoProcessarEvento.DataVeiculoPosicao.Value - permanenciaAbertaNoLocal.DataInicio;

                        if (tempo.TotalMinutes >= this.MonitoramentoEvento.Gatilho.TempoEvento)
                        {
                            string texto = (tempo.TotalMinutes < 60) ? $"Há {(int)tempo.TotalMinutes} minutos" : Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(tempo);
                            texto += ", desde " + permanenciaAbertaNoLocal.DataInicio.ToString("dd/MM/yyyy HH:mm");
                            texto += $" em {permanenciaAbertaNoLocal.Descricao}";
                            Log.TratarErro($"2 - Gerando alerta: {permanenciaAbertaNoLocal.CodigoPermanencia}", "PermanenciaPontoApoio");
                            base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento?.Carga ?? null, entregas, texto);
                            Log.TratarErro($"3 - Gerou alerta: {permanenciaAbertaNoLocal.CodigoPermanencia}", "PermanenciaPontoApoio");
                        }
                        Log.TratarErro($"4 - Permanencia Finalizada: {permanenciaAbertaNoLocal.CodigoPermanencia}", "PermanenciaPontoApoio");
                        Log.TratarErro($"_________________________________________________________________________________", "PermanenciaPontoApoio");
                    }
                }
            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }
}