using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 600000)]
    public class AlertaEventosCarga : LongRunningProcessBase<AlertaEventosCarga>
    {
        #region Metodos protegidos
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ProcessarAlertasCargasAtivos(unitOfWork, _tipoServicoMultisoftware);
        }

        #endregion

        #region Metodos Privados

        private void ProcessarAlertasCargasAtivos(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Servicos.Embarcador.Carga.AlertaCarga.AbstractAlertaCarga> eventosAtivos = CarregarEventosAtivos(unitOfWork, _stringConexao);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertasPendentes = BuscarAlertasPendentes(unitOfWork, eventosAtivos);

            if (eventosAtivos.Count > 0)
            {
                ProcessarEventosAtivos(eventosAtivos, alertasPendentes);
            }
        }

        private void ProcessarEventosAtivos(List<Servicos.Embarcador.Carga.AlertaCarga.AbstractAlertaCarga> eventos, IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertasPendentes)
        {
            int total = eventos.Count;
            if (total > 0)
            {
                // Processamento de cada um dos eventos
                for (int i = 0; i < total; i++)
                {
                    Type thisType = eventos[i].GetType();
                    MethodInfo theMethod = thisType.GetMethod("Processar");
                    theMethod.Invoke(eventos[i], new object[] { alertasPendentes });
                }
            }
        }

        private List<Servicos.Embarcador.Carga.AlertaCarga.AbstractAlertaCarga> CarregarEventosAtivos(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            DateTime inicio = DateTime.UtcNow;
            List<Servicos.Embarcador.Carga.AlertaCarga.AbstractAlertaCarga> eventos = new List<Servicos.Embarcador.Carga.AlertaCarga.AbstractAlertaCarga>();


            Servicos.Embarcador.Carga.AlertaCarga.AntecendenciaAGradeCarregamento antecendenciaAGradeCarregamento = new Servicos.Embarcador.Carga.AlertaCarga.AntecendenciaAGradeCarregamento(unitOfWork, stringConexao);
            if (antecendenciaAGradeCarregamento.EstaAtivo())
            {
                eventos.Add(antecendenciaAGradeCarregamento);
            }

            Servicos.Embarcador.Carga.AlertaCarga.CargaSemTransportador cargaSemTransportador = new Servicos.Embarcador.Carga.AlertaCarga.CargaSemTransportador(unitOfWork, stringConexao);
            if (cargaSemTransportador.EstaAtivo())
            {
                eventos.Add(cargaSemTransportador);
            }

            Servicos.Embarcador.Carga.AlertaCarga.CargaSemVeiculo cargaSemVeiculo = new Servicos.Embarcador.Carga.AlertaCarga.CargaSemVeiculo(unitOfWork, stringConexao);
            if (cargaSemVeiculo.EstaAtivo())
            {
                eventos.Add(cargaSemVeiculo);
            }

            Servicos.Embarcador.Carga.AlertaCarga.CargaVeiculoInsumo cargaVeiculoInsumo = new Servicos.Embarcador.Carga.AlertaCarga.CargaVeiculoInsumo(unitOfWork, stringConexao);
            if (cargaVeiculoInsumo.EstaAtivo())
            {
                eventos.Add(cargaVeiculoInsumo);
            }

            Servicos.Embarcador.Carga.AlertaCarga.CargaVeiculoNaoMonitorado cargaVeiculoNaoMonitorado = new Servicos.Embarcador.Carga.AlertaCarga.CargaVeiculoNaoMonitorado(unitOfWork, stringConexao);
            if (cargaVeiculoNaoMonitorado.EstaAtivo())
            {
                eventos.Add(cargaVeiculoNaoMonitorado);
            }

            Servicos.Embarcador.Carga.AlertaCarga.NaoAtendimentoAgenda naoAtendimentoAgenda = new Servicos.Embarcador.Carga.AlertaCarga.NaoAtendimentoAgenda(unitOfWork, stringConexao);
            if (naoAtendimentoAgenda.EstaAtivo())
            {
                eventos.Add(naoAtendimentoAgenda);
            }

            Servicos.Embarcador.Carga.AlertaCarga.InicioViagem incioViagem = new Servicos.Embarcador.Carga.AlertaCarga.InicioViagem(unitOfWork, stringConexao);
            if (incioViagem.EstaAtivo())
            {
                eventos.Add(incioViagem);
            }

            Servicos.Embarcador.Carga.AlertaCarga.FimViagem fimViagem = new Servicos.Embarcador.Carga.AlertaCarga.FimViagem(unitOfWork, stringConexao);
            if (fimViagem.EstaAtivo())
            {
                eventos.Add(fimViagem);
            }

            Servicos.Embarcador.Carga.AlertaCarga.ConfirmacaoColetaEntrega confirmacaoColetaEntrega = new Servicos.Embarcador.Carga.AlertaCarga.ConfirmacaoColetaEntrega(unitOfWork, stringConexao);
            if (confirmacaoColetaEntrega.EstaAtivo())
            {
                eventos.Add(confirmacaoColetaEntrega);
            }

            Servicos.Embarcador.Carga.AlertaCarga.CargaAlertaTendenciaEntregaColeta alertaTendenciaEntregaColeta = new Servicos.Embarcador.Carga.AlertaCarga.CargaAlertaTendenciaEntregaColeta(unitOfWork, stringConexao);
            if (alertaTendenciaEntregaColeta.EstaAtivo())
            {
                eventos.Add(alertaTendenciaEntregaColeta);
            }

            return eventos;
        }

        private IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> BuscarAlertasPendentes(Repositorio.UnitOfWork unitOfWork, List<Servicos.Embarcador.Carga.AlertaCarga.AbstractAlertaCarga> eventos)
        {
            DateTime inicio = DateTime.UtcNow;
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga>();

            int total = eventos.Count;
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga>();
            for (int i = 0; i < total; i++)
            {
                tiposAlerta.Add(eventos[i].GetTipoAlerta());
            }

            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
            alertas = repCargaEvento.BuscarUltimoAlertaPendenteObjetoDeValorPorTiposDeAlertaeCarga(tiposAlerta);

            return alertas;
        }


        #endregion
    }
}