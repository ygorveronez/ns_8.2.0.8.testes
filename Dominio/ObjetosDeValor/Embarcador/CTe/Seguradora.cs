namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class Seguradora
    {
        public virtual Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ClienteSeguradora { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Observacao { get; set; }
    }
}
