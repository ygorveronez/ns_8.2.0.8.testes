namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DESTINO_PRIORITARIO_CALCULO_FRETE_LOCALIDADE", EntityName = "DestinoPrioritarioCalculoFreteLocalidade", Name = "Dominio.Entidades.Embarcador.Ocorrencias.DestinoPrioritarioCalculoFreteLocalidade", NameType = typeof(DestinoPrioritarioCalculoFreteLocalidade))]
    public class DestinoPrioritarioCalculoFreteLocalidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DPL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DestinoPrioritarioCalculoFrete", Column = "DPC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DestinoPrioritarioCalculoFrete DestinoPrioritarioCalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Localidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPL_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPL_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Ordem.ToString() + " - " + this.Localidade.Descricao;
            }
        }
    }

}
