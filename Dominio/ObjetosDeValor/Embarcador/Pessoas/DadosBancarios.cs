namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class DadosBancarios
    {
        public virtual Banco Banco { get; set; }
        public string Agencia { get; set; }
        public string DigitoAgencia { get; set; }
        public string NumeroConta { get; set; }
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco TipoContaBanco { get; set; }
        public virtual Pessoa PortadorConta { get; set; }
    }
}
