namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_ENTREGA_COMP_PREST", EntityName = "EntregaCTeComponentePrestacao", Name = "Dominio.Entidades.EntregaCTeComponentePrestacao", NameType = typeof(EntregaCTeComponentePrestacao))]
    public class EntregaCTeComponentePrestacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ETP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EntregaCTe", Column = "ETC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EntregaCTe EntregaCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeCTe", Column = "ETP_NOME", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string NomeCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "ETP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiNaBaseDeCalculoDoICMS", Column = "ETP_INCLUI_BC_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluiNaBaseDeCalculoDoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiNoTotalAReceber", Column = "ETP_INCLUI_TOTAL_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluiNoTotalAReceber { get; set; }

        public virtual string Nome
        {
            get
            {
                return NomeCTe != null ? NomeCTe.ToUpper() : string.Empty;
            }
            set
            {
                NomeCTe = value != null ? value.ToUpper() : value;
            }
        }

        public virtual EntregaCTeComponentePrestacao Clonar()
        {
            return (EntregaCTeComponentePrestacao)this.MemberwiseClone();
        }
    }
}
