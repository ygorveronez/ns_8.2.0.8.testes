namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CUSTO_VIAGEM", EntityName = "CentroCustoViagem", Name = "Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem", NameType = typeof(CentroCustoViagem))]
    public class CentroCustoViagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCV_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCV_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCV_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCV_CODIGO_TRANSPORTADOR_OPENTECH", TypeType = typeof(int), NotNull = false)]
        public virtual int? CodigoTransportadorOpenTech { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCV_CODIGO_FILIAL_REPOM", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string CodigoFilialRepom { get; set; }

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
