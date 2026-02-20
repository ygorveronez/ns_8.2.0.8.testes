namespace Dominio.Entidades.Embarcador.Contabeis
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DIREITO_FISCAL", EntityName = "DireitoFiscal", Name = "Dominio.Entidades.Embarcador.Contabeis.DireitoFiscal", NameType = typeof(DireitoFiscal))]
    public class DireitoFiscal : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DIF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DIF_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ImpostoValorAgregado", Column = "IVA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ImpostoValorAgregado ImpostoValorAgregado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DIF_ICMS_ISS", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ICMS_ISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DIF_IPI", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DIF_PIS", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string PIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DIF_COFINS", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string COFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DIF_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }
    }
}
