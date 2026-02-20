namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SEGURADORA", EntityName = "Seguradora", Name = "Dominio.Entidades.Embarcador.Seguros.Seguradora", NameType = typeof(Seguradora))]
    public class Seguradora : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_SEGURADORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "SEA_NOME", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "SEA_OBSERVACAO", TypeType = typeof(string), Length = 450, NotNull = false)]
        public virtual string Observacao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "SEA_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ClienteSeguradora?.Descricao;
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
