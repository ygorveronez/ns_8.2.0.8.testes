using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public class CargaSemVeiculo : AbstractAlertaCarga
    {
        #region Métodos públicos

        public CargaSemVeiculo(Repositorio.UnitOfWork unitOfWork, string stringConexao) : base(unitOfWork, stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.CagraSemVeiculo) { }

        public override void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> ListaCargasAlertar = BuscarCargasSemVeiculoEmTempo();
            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento cargaAlertar in ListaCargasAlertar)
            {
                if (!base.ExisteAlertaAbertoOuFechadoHaPouco(cargaAlertar.CodigoCarga, alertas))
                {
                    string texto = $"A carga " + cargaAlertar.CargaEmbarcador + " está sem veículo informado ultrapassando o tempo limite de " + base.GetTempoEvento() + " Minutos";
                    base.CriarAlerta(alertas, cargaAlertar.CodigoCarga, 0,0, "");
                }
            }
        }

        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ConfiguracaoAlertaCarga.Tempo > 0;
        }


        public void TratarAlertaAposAdicionarVeiculoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(base.unitOfWork);
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarUltimoEmAbertoPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.CagraSemVeiculo);
            if (cargaEvento != null && veiculo != null)
            {
                cargaEvento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado;
                cargaEvento.Veiculo = veiculo;
                cargaEvento.Observacao = "Tratamento do alerta atomático após informar veículo na carga. Placa: " + veiculo.Placa + " Em " + DateTime.Now.ToString("dd/MM/yyyy");

                repCargaEvento.Atualizar(cargaEvento);
                servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
            }
        }

        #endregion


        #region Metodos Privados

        private IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> BuscarCargasSemVeiculoEmTempo()
        {
            int tempo = base.GetTempoEvento();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(base.unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> ListaCargasAlertar = repCarga.BuscarCargasParaAlertaEventoCarga(tempo, base.GetTipoAlerta());
            return ListaCargasAlertar;
        }

        #endregion
    }
}
