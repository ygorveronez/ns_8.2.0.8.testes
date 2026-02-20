namespace Dominio.ObjetosDeValor
{
    public class Pagina
    {
        public virtual string Formulario { get; set; }

        public virtual string Descricao { get; set; }

        public virtual string Menu { get; set; }

        public virtual MenuApp MenuApp { get; set; }

        public virtual string Status { get; set; }

        public virtual string Icone { get; set; }

        public virtual bool MostraNoMenu { get; set; }
    }
}
