namespace Dominio.Entidades.Embarcador.Contatos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTATO_CLIENTE_DOCUMENTO", EntityName = "ContatoClienteDocumento", Name = "Dominio.Entidades.Embarcador.Contatos.ContatoClienteDocumento", NameType = typeof(ContatoClienteDocumento))]
    public class ContatoClienteDocumento: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCD_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContatoCliente", Column = "CCL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Contatos.ContatoCliente ContatoCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bordero", Column = "BOR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Bordero Bordero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.Titulo Titulo { get; set; }
    }
}
