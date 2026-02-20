namespace Dominio.ObjetosDeValor.CTe
{
    public class Seguro
    {
        public virtual Dominio.Enumeradores.TipoSeguro Tipo { get; set; }

        public virtual string NomeSeguradora { get; set; }

        public virtual string NumeroApolice { get; set; }

        public virtual string NumeroAverbacao { get; set; }

        public virtual decimal Valor { get; set; }

        public virtual string CNPJSeguradora { get; set; }
    }
}
