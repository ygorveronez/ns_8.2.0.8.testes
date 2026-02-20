namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CONTRATO_FRETE_TERCEIRO_ACRESCIMO", EntityName = "AlcadaContratoFreteTerceiroValorAcrescimo", Name = "Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo", NameType = typeof(AlcadaContratoFreteTerceiroValorAcrescimo))]
    public class AlcadaContratoFreteTerceiroValorAcrescimo: Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraContratoFreteTerceiro", Column = "RCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraContratoFreteTerceiro RegraContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ARA_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Valor.ToString("n2");
            }
        }

        public virtual decimal PropriedadeAlcada
        {
            get
            {
                return this.Valor;
            }
            set
            {
                this.Valor = value;
            }
        }
    }
}
