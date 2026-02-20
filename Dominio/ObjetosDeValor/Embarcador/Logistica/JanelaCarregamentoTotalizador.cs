namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class JanelaCarregamentoTotalizador
    {        
        public int AguardandoAceiteTransportador { get; set; }

        public int AguardandoConfirmacaoTransportador { get; set; }

        public int AguardandoEncosta { get; set; }

        public int AguardandoLiberacaoTransportadores { get; set; }

        public int Descarregamento { get; set; }

        public int Faturada { get; set; }

        public int FOB { get; set; }

        public int ProntaCarregamento { get; set; }

        public int SemTransportador { get; set; }

        public int SemValorFrete { get; set; }
    }
}