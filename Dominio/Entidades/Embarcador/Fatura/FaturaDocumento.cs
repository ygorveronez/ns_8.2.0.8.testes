namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_DOCUMENTO", EntityName = "FaturaDocumento", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaDocumento", NameType = typeof(FaturaDocumento))]
    public class FaturaDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FDO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoFaturamento", Column = "DFA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento Documento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDO_VALOR_COBRAR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorACobrar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDO_VALOR_DESCONTO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDO_VALOR_ACRESCIMO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDO_VALOR_TOTAL_COBRAR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorTotalACobrar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDO_TITULO_GERADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool TituloGerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDO_CANCELADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Cancelado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDO_VALOR_DESCONTO_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorDescontoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDO_VALOR_ACRESCIMO_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAcrescimoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FDO_ACRESCENTAR_DESCONTAR_VALOR_DESSE_DOCUMENTO_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcrescentarOuDescontarValorDesseDocumentoFatura { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Documento.Descricao;
            }
        }
    }
}
