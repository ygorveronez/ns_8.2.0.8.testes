using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Global
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_INFORMACOES_BANCARIAS", EntityName = "CargaInformacoesBancarias", Name = "Dominio.Entidades.CargaInformacoesBancarias", NameType = typeof(CargaInformacoesBancarias))]
    public class CargaInformacoesBancarias : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public virtual string Descricao { get { return $"{Carga.CodigoCargaEmbarcador} - {TipoInformacaoBancaria?.ObterDescricao() ?? ""}"; } }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChavePIX", Column = "CIB_CHAVE_PIX", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ChavePIX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoChavePIX", Column = "CIB_TIPO_CHAVE_PIX_CIOT", TypeType = typeof(Dominio.ObjetosDeValor.Enumerador.TipoChavePix), Length = 200, NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Enumerador.TipoChavePix? TipoChavePIX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Conta", Column = "CIB_CONTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Conta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "CIB_AGENCIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ipef", Column = "CIB_IPEF", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Ipef { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoInformacaoBancaria", Column = "CIB_TIPO_INFORMACAO_BANCARIA", TypeType = typeof(TipoPagamentoMDFe), Length = 100, NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe? TipoInformacaoBancaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "CIB_TIPO_PAGAMENTO", TypeType = typeof(FormasPagamento), Length = 100, NotNull = false)]
        public virtual FormasPagamento? TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "CIB_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamento", Column = "CIB_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegistradoPeloEmbarcador", Column = "CIB_REGISTRADO_PELO_EMBARCADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? RegistradoPeloEmbarcador { get; set; }
    }
}

