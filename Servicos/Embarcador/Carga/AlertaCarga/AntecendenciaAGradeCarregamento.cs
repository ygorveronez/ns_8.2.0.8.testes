using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.AlertaCarga
{
    public class AntecendenciaAGradeCarregamento : AbstractAlertaCarga
    {
        #region Métodos públicos

        public AntecendenciaAGradeCarregamento(Repositorio.UnitOfWork unitOfWork, string stringConexao) : base(unitOfWork, stringConexao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.AntecedenciaGrade) { }

        public override void ProcessarEvento(List<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCarga> alertas)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> ListaCargasAlertar = BuscarCargasAntecedenciaGrade();

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento cargaAlertar in ListaCargasAlertar)
            {
                if (!base.ExisteAlertaCarga(cargaAlertar.CodigoCarga, alertas))
                {
                    string texto = "A data criação da carga " + cargaAlertar.CargaEmbarcador + " está em desacordo com data de carregamento da grade do centro de carregamento " + cargaAlertar.DataLimiteCarregamento.Value.ToString("dd/MM/yyyy hh:mm:ss");
                    base.CriarAlerta(alertas, cargaAlertar.CodigoCarga, 0, 0, texto);
                }
            }
        }

        public new bool EstaAtivo()
        {
            return base.EstaAtivo();
        }

        #endregion

        #region Metodos Privados

        private IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> BuscarCargasAntecedenciaGrade()
        {

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(base.unitOfWork);

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.CargaProcessarEvento> ListaCargasAlertar = repCarga.BuscarCargasParaAlertaEventoCarga(base.GetTempoEvento(), base.GetTipoAlerta());
            return ListaCargasAlertar;
        }

        #endregion
    }
}
