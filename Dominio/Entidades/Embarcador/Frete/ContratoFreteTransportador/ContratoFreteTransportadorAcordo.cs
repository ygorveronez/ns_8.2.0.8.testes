namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_TRANSPORTADOR_ACORDO", EntityName = "ContratoFreteTransportadorAcordo", Name = "Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo", NameType = typeof(ContratoFreteTransportadorAcordo))]
    public class ContratoFreteTransportadorAcordo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteTransportador", Column = "CFT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFreteTransportador ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_PERIODO", TypeType = typeof(int), NotNull = false)]
        public virtual int Periodo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_ROTULO", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal Rotulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_VALOR_ACORDADO", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal ValorAcordado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_QUANTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_FRANQUIA_KM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool FranquiaPorKm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_TOTAL", TypeType = typeof(decimal), Scale = 8, Precision = 18, NotNull = false)]
        public virtual decimal Total { get; set; }

        public virtual string DescricaoFranquiaPorKm
        {
            get
            {
                if (this.FranquiaPorKm)
                    return "Sim";
                else
                    return "Não";
            }
        }

        public virtual string Descricao
        {
            get
            {
                return ModeloVeicular.Descricao;
            }
        }

        public virtual string DescricaoModeloValor
        {
            get => $"{ModeloVeicular.Descricao} - R$ {ValorAcordado.ToString("n2")}";
        }
    }
}
