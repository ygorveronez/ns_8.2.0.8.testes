namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_DOCUMENTO", EntityName = "TituloDocumento", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloDocumento", NameType = typeof(TituloDocumento))]
    public class TituloDocumento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturaDocumento", Column = "FDO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.FaturaDocumento FaturaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_TIPO_DOCUMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo TipoDocumento { get; set; }

        /// <summary>
        /// Valor referente ao documento a ser cobrado/pago no título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Valor { get; set; }

        /// <summary>
        /// Valor do desconto concedido na geração do título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_DESCONTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorDesconto { get; set; }

        /// <summary>
        /// Valor do acréscimo concedido na geração do título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_ACRESCIMO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAcrescimo { get; set; }

        /// <summary>
        /// Valor + ValorAcrescimo - ValorDesconto
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_TOTAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotal { get; set; }
                
        /// <summary>
        /// Valor pendente para cobrança/pagamento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_PENDENTE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorPendente { get; set; }
        
        /// <summary>
        /// Valor do desconto concedido na baixa do título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_DESCONTO_BAIXA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorDescontoBaixa { get; set; }

        /// <summary>
        /// Valor do acréscimo concedido na baixa do título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_ACRESCIMO_BAIXA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAcrescimoBaixa { get; set; }

        /// <summary>
        /// Valor total cobrado/pago na baixa do título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_PAGO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorPago { get; set; }

        /// <summary>
        /// Desmembramento do valor pago, referente ao valor do documento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_PAGO_DOCUMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorPagoDocumento { get; set; }

        /// <summary>
        /// Desmembramento do valor pago, referente ao valor do acréscimo
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_PAGO_ACRESCIMO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorPagoAcrescimo { get; set; }

        /// <summary>
        /// Valor em moeda estrangeira referente ao documento a ser cobrado/pago no título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorMoeda { get; set; }

        /// <summary>
        /// ValorMoeda + ValorAcrescimoMoeda - ValorDescontoMoeda
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotalMoeda { get; set; }

        /// <summary>
        /// Valor em moeda estrangeira pendente para cobrança/pagamento
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_PENDENTE_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorPendenteMoeda { get; set; }

        /// <summary>
        /// Valor da cotação originária do documento no título
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "TDO_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorCotacaoMoeda { get; set; }

        public virtual string NumeroDocumento
        {
            get
            {
                if (TipoDocumento == ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoTitulo.CTe)
                    return "CT-e " + CTe.Numero.ToString() + "-" + CTe.Serie.Numero.ToString();
                else
                    return "Carga " + Carga.CodigoCargaEmbarcador;
            }
        }
        public virtual string Descricao
        {
            get
            {
                return this.NumeroDocumento;
            }
        }
    }
}
