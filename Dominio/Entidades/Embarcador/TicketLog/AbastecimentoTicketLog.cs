using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ABASTECIMENTO_TICKETLOG", EntityName = "AbastecimentoTicketLog", Name = "Dominio.Entidades.AbastecimentoTicketLog", NameType = typeof(Dominio.Entidades.AbastecimentoTicketLog))]
    public class AbastecimentoTicketLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ABA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTransacao", Column = "ABA_CODIGO_TRANSACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTransacao", Column = "ABA_VALOR_TRANSACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ValorTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTransacaoDecimal", Column = "ABA_VALOR_TRANSACAO_DECIMAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTransacaoDecimal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataTransacao", Column = "ABA_DATA_TRANSACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CnpjEstabelecimento", Column = "ABA_CNPJ_ESTABELECIMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CnpjEstabelecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Litros", Column = "ABA_LITROS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Litros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LitrosDecimal", Column = "ABA_LITROS_DECIMAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal LitrosDecimal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCombustivel", Column = "ABA_TIPO_COMBUSTIVEL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TipoCombustivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "ABA_PLACA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLitro", Column = "ABA_VALOR_LITRO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ValorLitro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLitroDecimal", Column = "ABA_VALOR_LITRO_DECIMAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorLitroDecimal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quilometragem", Column = "ABA_QUILOMETRAGEM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Quilometragem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuilometragemInt", Column = "ABA_QUILOMETRAGEM_INT", TypeType = typeof(int), NotNull = false)]
        public virtual int QuilometragemInt { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "ABA_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Placa + " - " + this.Litros;
            }
        }

    }
}
