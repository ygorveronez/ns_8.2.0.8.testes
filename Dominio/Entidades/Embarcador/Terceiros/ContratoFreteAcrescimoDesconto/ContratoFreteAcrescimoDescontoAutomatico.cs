namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_ACRESCIMO_DESCONTO_AUTOMATICO", EntityName = "ContratoFreteAcrescimoDescontoAutomatico", Name = "Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico", NameType = typeof(ContratoFreteAcrescimoDescontoAutomatico))]
    public class ContratoFreteAcrescimoDescontoAutomatico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CFA_VALOR", TypeType = typeof(decimal), NotNull = true, Precision = 18, Scale = 2)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "CFA_TIPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoValorContratoFreteADA), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoValorContratoFreteADA TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCalculo", Column = "CFA_TIPO_CALCULO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoContratoFreteADA), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoContratoFreteADA? TipoCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFA_OBSERVACOES", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacoes { get; set; }
    }
}