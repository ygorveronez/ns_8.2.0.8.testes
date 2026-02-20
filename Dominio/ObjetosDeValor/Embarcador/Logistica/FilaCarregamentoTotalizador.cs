namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FilaCarregamentoTotalizador
    {
        public int AguardandoAceitePreCarga { get; set; }

        public int AguardandoConfirmacao { get; set; }

        public int AguardandoConjuntos { get; set; }

        public int CargaCancelada { get; set; }

        public int EmRemocao { get; set; }

        public int AguardandoAceite { get; set; }

        public int EmChecklist { get; set; }

        public int EmViagem { get; set; }

        public int AguardandoCarga { get; set; }

        public int AguardandoDesatrelar { get; set; }

        public int EmReversa { get; set; }

        public int Vazio { get; set; }

        public int CargaRecusada { get; set; }

        public int PerdeuSenha { get; set; }
    }
}
