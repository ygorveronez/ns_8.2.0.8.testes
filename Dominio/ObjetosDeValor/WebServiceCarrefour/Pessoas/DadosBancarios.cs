namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Pessoas
{
    public sealed class DadosBancarios
    {
        public Banco Banco { get; set; }

        public string Agencia { get; set; }

        public string DigitoAgencia { get; set; }

        public string NumeroConta { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco TipoContaBanco { get; set; }
    }
}
