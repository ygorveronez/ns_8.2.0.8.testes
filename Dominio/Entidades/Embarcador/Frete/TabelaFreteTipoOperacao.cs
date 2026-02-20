using System;


namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_TIPO_OPERACAO", EntityName = "TabelaFreteTipoOperacao", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao", NameType = typeof(TabelaFreteTipoOperacao))]
    [Obsolete("Remover essa classe")]
    public class TabelaFreteTipoOperacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOperacaoEmissao", Column = "FTO_TIPO_OPERACAO_EMISSAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao TipoOperacaoEmissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO_REDESPACHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFrete TabelaFreteRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagarPorToneladaPercentualExcedente", Column = "FTO_PAGAR_TONELADA_PERCENTUAL_EXCEDENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PagarPorToneladaPercentualExcedente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PagarPorToneladaPercentualExcedenteRedespacho", Column = "FTO_PAGAR_TONELADA_PERCENTUAL_EXCEDENTE_REDESPACHO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PagarPorToneladaPercentualExcedenteRedespacho { get; set; }

    }
}
