namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DEPOSITO", EntityName = "Deposito", Name = "Dominio.Entidades.Embarcador.WMS.Deposito", NameType = typeof(Deposito))]
    public class Deposito : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DEP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "DEP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DEP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroUnidadeImpressao", Column = "DEP_NUMERO_UNIDADE_IMPRESSAO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroUnidadeImpressao { get; set; }

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
