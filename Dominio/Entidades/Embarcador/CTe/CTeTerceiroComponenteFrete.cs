namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_TERCEIRO_COMPONENTE_FRETE", EntityName = "CTeTerceiroComponenteFrete", Name = "Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete", NameType = typeof(CTeTerceiroComponenteFrete))]
    public class CTeTerceiroComponenteFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CTeTerceiro", Column = "CPS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CTeTerceiro CTeTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CSC_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }

        public virtual bool Equals(CTeTerceiroComponenteFrete other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual CTeTerceiroComponenteFrete Clonar()
        {
            return (CTeTerceiroComponenteFrete)this.MemberwiseClone();
        }
    }
}
