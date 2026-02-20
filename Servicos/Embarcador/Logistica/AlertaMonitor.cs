using System;

namespace Servicos.Embarcador.Logistica
{
    public static class AlertaMonitor
    {
        public static void InserirAlertaEntrega(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, decimal? latitude, decimal? longitude, DateTime dataAlerta, string observacao, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);

            var novoAlerta = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitor
            {
                DataCadastro = DateTime.Now,
                MonitoramentoEvento = monitoramentoEvento,
                Data = dataAlerta,
                TipoAlerta = tipoAlerta,
                Veiculo = cargaEntrega.Carga.Veiculo,
                Observacao = observacao,
                Latitude = latitude,
                Longitude = longitude,
                Carga = cargaEntrega.Carga,
                CargaEntrega = cargaEntrega
            };

            repAlertaMonitor.Inserir(novoAlerta);

        }

        public static void EfetuarTratativaAlertaMonitor(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alertaMonitor, string ObservacaoFinalizacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);

            alertaMonitor.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado;
            alertaMonitor.Observacao = ObservacaoFinalizacao;

            repAlertaMonitor.Atualizar(alertaMonitor);
        }


    }
}
