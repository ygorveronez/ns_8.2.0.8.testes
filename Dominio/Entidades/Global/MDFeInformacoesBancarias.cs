using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_INFORMACOES_BANCARIAS", EntityName = "MDFeInformacoesBancarias", Name = "Dominio.Entidades.MDFeInformacoesBancarias", NameType = typeof(MDFeInformacoesBancarias))]
    public class MDFeInformacoesBancarias : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MIB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        public virtual string Descricao { get { return $"{MDFe.Numero} - {TipoInformacaoBancaria?.ObterDescricao() ?? ""}"; } }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChavePIX", Column = "MIB_CHAVE_PIX", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChavePIX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Conta", Column = "MIB_CONTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Conta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "MIB_AGENCIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ipef", Column = "MIB_IPEF", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Ipef { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoInformacaoBancaria", Column = "MIB_TIPO_INFORMACAO_BANCARIA", TypeType = typeof(TipoPagamentoMDFe), Length = 100, NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe? TipoInformacaoBancaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "MIB_TIPO_PAGAMENTO", TypeType = typeof(FormasPagamento), Length = 100, NotNull = false)]
        public virtual FormasPagamento? TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "MIB_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorAltoDesempenho", Column = "MIB_INDICADOR_ALTO_DESEMPENHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? IndicadorAltoDesempenho { get; set; }
    }
}
