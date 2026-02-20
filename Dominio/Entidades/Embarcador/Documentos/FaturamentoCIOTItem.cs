using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_FATURAMENTO_ITEM", EntityName = "FaturamentoCIOTItem", Name = "Dominio.Entidades.Embarcador.Documentos.FaturamentoCIOTItem", NameType = typeof(FaturamentoCIOTItem))]
    public class FaturamentoCIOTItem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturamentoCIOT", Column = "CFA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FaturamentoCIOT FaturamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_DATA_DECLARACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataDeclaracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CIOT", Column = "CIO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CIOT CIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_ID_OPERACAO_CLIENTE", TypeType = typeof(string), NotNull = false)]
        public virtual string IdOperacaoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Adiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_LIVRE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Livre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_ESTADIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Estadia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_QUITACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Quitacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_FROTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Frota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_SERVICO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Servico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_ID", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Id { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_RECIBO", TypeType = typeof(int), NotNull = false)]
        public virtual int Recibo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_DOCUMENTO_VIAGEM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DocumentoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_QUANTIDADE_DA_MERCADORIA_NO_DESEMBARQUE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? QuantidadeDaMercadoriaNoDesembarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_VALOR_DIFERENCA_DE_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorDiferencaDeFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_VALOR_QUEBRA_DE_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorQuebraDeFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFI_TIPO", TypeType = typeof(FaturamentoEFreteItemTipo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteItemTipo Tipo { get; set; }
    }
}
