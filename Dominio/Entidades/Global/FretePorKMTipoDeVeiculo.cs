namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FRETE_KM_TIPO_VEICULO", EntityName = "FretePorKMTipoDeVeiculo", Name = "Dominio.Entidades.FretePorKMTipoDeVeiculo", NameType = typeof(FretePorKMTipoDeVeiculo))]
    public class FretePorKMTipoDeVeiculo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FKT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoVeiculo", Column = "VTI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoVeiculo TipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FKT_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FKT_KM_FRANQUIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal KMFranquia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FKT_EXCEDENTE_POR_KM", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ExcedentePorKM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FKT_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FKT_TIPO_CALCULO", TypeType = typeof(Dominio.Enumeradores.TipoCalculoFreteKMTipoVeiculo), Length = 1, NotNull = true)]
        public virtual Dominio.Enumeradores.TipoCalculoFreteKMTipoVeiculo TipoCalculo { get; set; }

        public virtual string DescricaoTipoCalculo
        {
            get
            {
                switch (this.TipoCalculo)
                {
                    case Enumeradores.TipoCalculoFreteKMTipoVeiculo.Acerto:
                        return "Acerto";
                    case Enumeradores.TipoCalculoFreteKMTipoVeiculo.CTe:
                        return "CT-e";
                    default:
                        return "";
                }
            }
        }


        public virtual string DescricaoStatus
        {
            get
            {
                if (this.Status == "A")
                    return "Ativo";
                else if (this.Status == "I")
                    return "Inativo";
                else
                    return "";
            }
        }

    }
}
