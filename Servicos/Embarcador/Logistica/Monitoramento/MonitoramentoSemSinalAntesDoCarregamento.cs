using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Excecoes.Embarcador;

namespace Servicos.Embarcador.Logistica.Monitoramento
{
    public class SemSinalAntesDoCarregamento
    {
        private Repositorio.UnitOfWork _unitOfWork;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento;
        private Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual;
        private Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor;
        private Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao repMonitoramentoVeiculoPosicao;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSemSinalAntesDoCarregamento> monitoramentosAtivos;
        private List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> eventosAtivos;
        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> posicoesAtuais;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfigTMS;

        public SemSinalAntesDoCarregamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            _unitOfWork = unitOfWork;
            ConfigTMS = configuracaoTMS;
        }

        public void Iniciar()
        {
            Processar();
        }

        private void Inicializar()
        {
            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(_unitOfWork);
            eventosAtivos = repMonitoramentoEvento.BuscarAtivosSemSinalTempoReferenteaDataCarregamentoCarga();

            if (eventosAtivos.Count <= 0) return;

            int maiorTempoEvento = eventosAtivos.Max(evento => evento.Gatilho.TempoEvento);
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork);
            monitoramentosAtivos = repMonitoramento.BuscarMonitoramentoIniciadoParaCargasComDataCarregamentoCargaDefinido(maiorTempoEvento);

            repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(_unitOfWork);
            repMonitoramentoVeiculoPosicao = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao(_unitOfWork);
        }

        private void Processar()
        {
            Inicializar();

            try
            {
                foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento in eventosAtivos)
                {
                    foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSemSinalAntesDoCarregamento monitoramentoAtivo in monitoramentosAtivos)
                    {
                        int minutosAteOCarregamento = monitoramentoAtivo.DataCarregamentoCarga.HasValue ? Convert.ToInt32((monitoramentoAtivo.DataCarregamentoCarga - DateTime.Now).Value.TotalMinutes) : 0;
                        if (minutosAteOCarregamento < 0) continue;

                        //Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = posicoesAtuais.Find(pos => pos.Veiculo?.Codigo == monitoramentoAtivo.CodigoVeiculo);
                        bool possuiPosicoes = repMonitoramentoVeiculoPosicao.ExistePosicoesPorMonitoramento(monitoramentoAtivo.CodigoMonitoramento, monitoramentoAtivo.CodigoVeiculo);

                        if (minutosAteOCarregamento <= evento.Gatilho.TempoEvento && !possuiPosicoes)
                        {
                            _unitOfWork.Start();
                            string descricaoAlerta = $"{minutosAteOCarregamento} minutos até o carregamento";
                            GerarEvento(evento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemSinal, descricaoAlerta, monitoramentoAtivo.CodigoCarga, monitoramentoAtivo.CodigoVeiculo, DateTime.Now, null);
                            _unitOfWork.CommitChanges();
                        }
                    }
                }
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                _unitOfWork.Rollback();
            }
        }

        private void GerarEvento(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, string descricaoAlerta, int codigoCarga, int codigoVeiculo, DateTime dataAlerta, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>();
            tiposAlerta.Add(tipoAlerta);
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas = repAlertaMonitor.BuscarUltimoAlertaCargaETipoDeAlerta(codigoCarga, tiposAlerta);

            if (alertas == null ||
                alertas.Count == 0 ||
                (alertas[0].Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado &&
                 alertas[0].Data.AddMinutes(evento.Gatilho.Tempo) < dataAlerta))
            {
                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitor()
                {
                    TipoAlerta = tipoAlerta,
                    MonitoramentoEvento = evento,
                    Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto,
                    DataCadastro = DateTime.Now,
                    Data = dataAlerta,
                    Veiculo = new Dominio.Entidades.Veiculo { Codigo = codigoVeiculo },
                    Carga = new Dominio.Entidades.Embarcador.Cargas.Carga { Codigo = codigoCarga },
                    AlertaDescricao = descricaoAlerta.Length > 50 ? descricaoAlerta.Substring(0, 50) : descricaoAlerta,
                    Latitude = Convert.ToDecimal(posicao?.Latitude ?? 0),
                    Longitude = Convert.ToDecimal(posicao?.Longitude ?? 0)
                };
                repAlertaMonitor.Inserir(alerta);
            }
        }
    }
}