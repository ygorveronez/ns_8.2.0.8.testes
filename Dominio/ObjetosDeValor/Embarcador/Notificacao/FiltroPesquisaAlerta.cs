namespace Dominio.ObjetosDeValor.Embarcador.Notificacao
{
    public sealed class FiltroPesquisaAlerta
    {
        public bool AlertarAposVencimento { get; set; }

        public int DiasAlertarAntesVencimento { get; set; }

        public int DiasRepetirAlerta { get; set; }
    }
}
