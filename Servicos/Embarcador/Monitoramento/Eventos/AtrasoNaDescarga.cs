using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public class AtrasoNaDescarga : AbstractEvento
    {

        #region Métodos públicos

        public AtrasoNaDescarga(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.AtrasoNaDescarga)
        {

        }

        /**
         * Confirma que o evento está ativo e com as configurações mínimas
         */
        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.Tempo != null) && this.ListaMonitoramentoEvento.Any(x => x.Gatilho?.TempoEvento != null);
        }

        /**
         * Processa a posição atual recebida para gerar um possível alerta
         */
        public override void ProcessarEvento(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, List<double> codigosClientesAlvo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelasDescarregamento)
        {

            // Há carga e iniciou a viagem
            if (monitoramento != null && monitoramento.Carga != null && monitoramento.Carga.DataInicioViagem != null)
            {
                int total = entregas?.Count ?? 0;
                for (int i = 0; i < total; i++)
                {
                    if (entregas[i].Coleta == false && entregas[i].DataInicio != null)
                    {

                        if (this.MonitoramentoEvento.Gatilho.DataBase == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.EntradaCliente) // quando data base é entrada cliente, este alerta ja é processado no entrada do raio (thread ProcessarTrocaAlvo) #34309
                            return;


                        DateTime? dataBase = BuscarDataMonitoramentoEvento(this.MonitoramentoEvento.Gatilho.DataBase, monitoramentoProcessarEvento, monitoramento, entregas[i], cargaJanelaCarregamento, cargaJanelasDescarregamento);
                        if (dataBase != null)
                        {
                            //verificar data se é menor que data Agenda
                            if (dataBase < entregas[i].DataPrevista)
                                dataBase = entregas[i].DataPrevista;
                        }

                        if (dataBase == null) dataBase = monitoramentoProcessarEvento.DataVeiculoPosicao.Value;

                        DateTime? dataReferencia = BuscarDataMonitoramentoEvento(this.MonitoramentoEvento.Gatilho.DataReferencia, monitoramentoProcessarEvento, monitoramento, entregas[i], cargaJanelaCarregamento, cargaJanelasDescarregamento);
                        if (dataReferencia == null) dataReferencia = monitoramentoProcessarEvento.DataVeiculoPosicao.Value;

                        if (dataReferencia != null)
                        {

                            // Adiciona a tolerância
                            dataReferencia = dataReferencia.Value.AddMinutes(this.MonitoramentoEvento.Gatilho.TempoEvento);

                            //caso o gatilho tem configuracao de considerar apenas data sem hora
                            if (this.MonitoramentoEvento.Gatilho.ConsiderarApenasDataNaReferencia && dataReferencia != null && dataBase != null)
                            {
                                dataReferencia = dataReferencia.Value.Date;
                                dataBase = dataBase.Value.Date;
                            }

                            // Atrasou?
                            if (dataBase > dataReferencia)
                            {
                                // Cria o alerta para a carga se não existir algum

                                TimeSpan atraso = dataBase.Value - dataReferencia.Value;
                                string texto = $"Descarga {entregas[i].Cliente?.CPF_CNPJ_Formatado ?? ""} atrasada por " + Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(atraso);
                                base.CriarAlertaSeNaoExistir(alertas, monitoramentoProcessarEvento, monitoramento.Carga, new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> { entregas[i] }, texto);
                            }
                        }
                    }
                }

            }
        }

        public override void ExecutarDepoisDeCriarAlerta(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento) { }

        #endregion

    }

}
