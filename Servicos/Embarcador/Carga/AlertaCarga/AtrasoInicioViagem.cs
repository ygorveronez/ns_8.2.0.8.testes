using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    class AtrasoInicioViagem : AbstractAlertaCarga
    {
        #region Métodos públicos

        public AtrasoInicioViagem(Repositorio.UnitOfWork unitOfWork, string stringConexao) : base(unitOfWork, stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.AtrasoInicioViagem) { }

        public override void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> ListaCargasAlertar = BuscarCargasComAtrasoInicioViagem();
            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento cargaAlertar in ListaCargasAlertar)
            {
                if (!base.ExisteAlertaAbertoOuFechadoHaPouco(cargaAlertar.CodigoCarga, alertas))
                {
                    string texto = $"A carga " + cargaAlertar.CargaEmbarcador + " não iniciou viagem, início viagem prevista em " + cargaAlertar.DataInicioViagemPrevista.Value.ToString("dd/MM/yyyy HH:mm:ss") + " ultrapassando o tempo limite de " + base.GetTempoEvento() + " Minutos";
                    base.CriarAlerta(alertas, cargaAlertar.CodigoCarga, 0,0, "");
                }
            }
        }

        public new bool EstaAtivo()
        {
            return base.EstaAtivo() && this.ConfiguracaoAlertaCarga.Tempo > 0 && this.ConfiguracaoAlertaCarga.TempoEvento > 0;
        }

        #endregion


        #region Metodos Privados

        private IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> BuscarCargasComAtrasoInicioViagem()
        {
            int tempo = base.GetTempoEvento();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(base.unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> ListaCargasAlertar = repCarga.BuscarCargasParaAlertaEventoCarga(tempo, base.GetTipoAlerta());
            return ListaCargasAlertar;
        }

        #endregion
    }
}
