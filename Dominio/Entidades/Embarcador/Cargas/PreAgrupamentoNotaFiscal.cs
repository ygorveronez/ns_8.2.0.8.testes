using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PRE_AGRUPAMENTO_NOTA_FISCAL", EntityName = "PreAgrupamentoNotaFiscal", Name = "Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal", NameType = typeof(PreAgrupamentoNotaFiscal))]
    public class PreAgrupamentoNotaFiscal : EntidadeBase, IEquatable<PreAgrupamentoNotaFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreAgrupamentoCarga", Column = "PAC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreAgrupamentoCarga PreAgrupamentoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjEmitente", Column = "PAN_CNPJ_EMITENTE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CnpjEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjDestinatario", Column = "PAN_CNPJ_DESTINATARIO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CnpjDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNota", Column = "PAN_NUMERO_NOTA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieNota", Column = "PAN_SERIE_NOTA", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string SerieNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedido", Column = "PAN_NUMERO_PEDIDO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoEntrega", Column = "PAN_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntrega { get; set; }

        public virtual string Descricao
        {
            get { return NumeroNota.ToString(); }
        }

        public virtual bool Equals(PreAgrupamentoNotaFiscal other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
