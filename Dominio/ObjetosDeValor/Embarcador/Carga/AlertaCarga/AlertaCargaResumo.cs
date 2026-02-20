namespace Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga
{
    public class AlertaCargaResumo
    {

        public ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga TipoAlerta { get; set; }

        public string Descricao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCargaHelper.ObterDescricao(TipoAlerta); }
        }

        public int Total { get; set; }
        public int Cargas { get; set; }
        public int Pendentes { get; set; }
    }
}
