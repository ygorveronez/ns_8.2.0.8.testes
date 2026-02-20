namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_MODELO_VEICULAR", EntityName = "ContratoFreteTransportadorModeloVeicular", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorClienteModeloVeicular", NameType = typeof(ContratoFreteTransportadorModeloVeicular))]
    public class ContratoFreteTransportadorModeloVeicular : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFM_VALOR_DIARIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDiaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFM_VALOR_QUINZENA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorQuinzena { get; set; }


        public virtual string Descricao
        {
            get
            {
                return ContratoFrete.Descricao + " - " + ModeloVeicular.Descricao;
            }
        }
    }
}
