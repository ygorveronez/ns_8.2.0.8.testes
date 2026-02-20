namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_REAJUSTE_FRETE_FILIAL", EntityName = "AlcadaReajusteFreteFilial", Name = "Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteFilial", NameType = typeof(AlcadaReajusteFreteFilial))]
    public class AlcadaReajusteFreteFilial : Alcada.Alcada
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ARF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraControleReajusteFretePlanilha", Column = "RRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraControleReajusteFretePlanilha RegraControleReajusteFretePlanilha { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Filial?.Descricao ?? string.Empty;
            }
        }

        public virtual Filiais.Filial PropriedadeAlcada
        {
            get
            {
                return this.Filial;
            }
            set
            {
                this.Filial = value;
            }
        }
    }
}