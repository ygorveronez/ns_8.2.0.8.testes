namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PENDENCIA_CONTRATO_FRETE_FUTURO", EntityName = "PendenciaContratoFreteFuturo", Name = "Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo", NameType = typeof(PendenciaContratoFreteFuturo))]
    public class PendenciaContratoFreteFuturo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "PCF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PCF_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFrete ContratoFreteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFrete", Column = "CFT_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFrete ContratoFreteDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TERCEIRO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente TransportadorTerceiro { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

    }
}
