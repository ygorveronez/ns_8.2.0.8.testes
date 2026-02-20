namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_ESCRITURACAO", EntityName = "RegraEscrituracao", Name = "Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao", NameType = typeof(RegraEscrituracao))]
    public class RegraEscrituracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RES_DESCRICAO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RES_ORIGEM_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool OrigemFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RES_DESTINO_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DestinoFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RES_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }
    }
}
