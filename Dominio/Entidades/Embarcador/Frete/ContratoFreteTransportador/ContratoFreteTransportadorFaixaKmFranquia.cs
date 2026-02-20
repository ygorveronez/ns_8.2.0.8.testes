namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_FAIXA_KM_FRANQUIA", EntityName = "ContratoFreteTransportadorFaixaKmFranquia", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia", NameType = typeof(ContratoFreteTransportadorFaixaKmFranquia))]
    public class ContratoFreteTransportadorFaixaKmFranquia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFK_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuilometragemInicial", Column = "CFK_QUILOMETRAGEM_INICIAL", TypeType = typeof(int), NotNull = true)]
        public virtual int QuilometragemInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuilometragemFinal", Column = "CFK_QUILOMETRAGEM_FINAL", TypeType = typeof(int), NotNull = true)]
        public virtual int QuilometragemFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFK_VALOR_POR_QUILOMETRO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorPorQuilometro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFrete { get; set; }

        public virtual string Descricao
        {
            get { return $"De {this.QuilometragemInicial.ToString("n0")} a {this.QuilometragemFinal.ToString("n0")} quilômetros"; }
        }
    }
}
