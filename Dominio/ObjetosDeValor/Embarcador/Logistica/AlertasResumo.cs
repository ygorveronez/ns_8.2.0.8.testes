namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class AlertasResumo
    {
        public ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }

        public int MonitoramentoEvento { get; set; }

        public string Descricao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterDescricao(TipoAlerta); }
        }

        public int Total { get; set; }

    }
}
