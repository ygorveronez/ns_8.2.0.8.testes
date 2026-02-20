namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DEPOSITO_RUA", EntityName = "DepositoRua", Name = "Dominio.Entidades.Embarcador.WMS.DepositoRua", NameType = typeof(DepositoRua))]
    public class DepositoRua : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DER_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DER_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Deposito", Column = "DEP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.WMS.Deposito Deposito { get; set; }

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
