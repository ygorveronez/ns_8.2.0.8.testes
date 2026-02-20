namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_VALORES_OUTROS_RECURSOS", EntityName = "ContratoFreteTransportadorValoresOutrosRecursos", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos", NameType = typeof(ContratoFreteTransportadorValoresOutrosRecursos))]
    public class ContratoFreteTransportadorValoresOutrosRecursos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFR_TIPO_MAO_DE_OBRA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string TipoMaoDeObra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFR_PRECO_ATUAL", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal PrecoAtual { get; set; }

    }
}
