using System;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOVIMENTO_CONTRATO_FRETE_CLIENTE", EntityName = "MovimentoContratoFreteCliente", Name = "Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente", NameType = typeof(MovimentoContratoFreteCliente))]
    public class MovimentoContratoFreteCliente : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFreteCliente", Column = "CFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Escrituracao.ContratoFreteCliente ContratoFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MFC_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MFC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [Obsolete("Não será necessário")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "MFC_SALDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Saldo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimentoContrato", Column = "MFC_TIPO", TypeType = typeof(ObjetosDeValor.Enumerador.TipoMovimentoContrato), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Enumerador.TipoMovimentoContrato TipoMovimentoContrato { get; set; }
    }
}
