using System;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FRETE_CLIENTE", EntityName = "ContratoFreteCliente", Name = "Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente", NameType = typeof(ContratoFreteCliente))]
    public class ContratoFreteCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_NUMERO_CONTRATO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string NumeroContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_DESCRICAO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_VALOR_CONTRATO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_SALDO_ATUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFC_FECHADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Fechado { get; set; }
    }
}
