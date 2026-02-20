using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public class NaoAtendimentoAgenda : AbstractAlertaCarga
    {
        #region Métodos públicos

        public NaoAtendimentoAgenda(Repositorio.UnitOfWork unitOfWork, string stringConexao) : base(unitOfWork, stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.NaoAtendimentoAgenda) { }

        public override void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> ListaCargasAlertar = BuscarCargasNaoAtendiementoAgenda();

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento cargaAlertar in ListaCargasAlertar)
            {
                if (!base.ExisteAlertaCarga(cargaAlertar.CodigoCarga, alertas))
                {
                    string texto = "A carga " + cargaAlertar.CargaEmbarcador + " possuí data de previsão de chegada " + cargaAlertar.DataPrevisaoChegadaPlanta.Value.ToString("dd/MM/yyyy hh:mm:ss") + " maior que a data da agenda " + cargaAlertar.DataInicioCarregamentoJanela.Value.ToString("dd/MM/yyyy hh:mm:ss");
                    base.CriarAlerta(alertas, cargaAlertar.CodigoCarga, 0, 0, "");
                }
            }
        }

        public new bool EstaAtivo()
        {
            return base.EstaAtivo();
        }

        #endregion

        #region Metodos Privados

        private IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> BuscarCargasNaoAtendiementoAgenda()
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(base.unitOfWork);

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> ListaCargasAlertar = repCarga.BuscarCargasParaAlertaEventoCarga(base.GetTempoEvento(), base.GetTipoAlerta());
            return ListaCargasAlertar;
        }

        #endregion

    }
}
