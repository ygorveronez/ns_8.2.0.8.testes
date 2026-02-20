namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SIGA_FACIL_CIOT_CTE", EntityName = "CTeCIOTSigaFacil", Name = "Dominio.Entidades.CTeCIOTSigaFacil", NameType = typeof(CTeCIOTSigaFacil))]
    public class CTeCIOTSigaFacil : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SFT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CIOTSigaFacil", Column = "SFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CIOTSigaFacil CIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNotaFiscal", Column = "SFT_NUMERO_NOTA_FISCAL", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NSU", Column = "SFT_NSU", TypeType = typeof(int), NotNull = true)]
        public virtual int NSU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeMercadoria", Column = "SFT_QUANTIDADE_MERCADORIA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EspecieMercadoria", Column = "SFT_ESPECIE_MERCADORIA", TypeType = typeof(string), Length = 2, NotNull = true)]
        public virtual string EspecieMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPeso", Column = "SFT_TIPO_PESO", TypeType = typeof(Enumeradores.TipoPeso), NotNull = true)]
        public virtual Enumeradores.TipoPeso TipoPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBruto", Column = "SFT_PESO_BRUTO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal PesoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLotacao", Column = "SFT_PESO_LOTACAO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal PesoLotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMercadoriaKG", Column = "SFT_VALOR_MERCADORIA_KG", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal ValorMercadoriaKG { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoria", Column = "SFT_VALOR_TOTAL_MERCADORIA", TypeType = typeof(decimal), Scale = 2, Precision = 10, NotNull = false)]
        public virtual decimal ValorTotalMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTarifaFrete", Column = "SFT_VALOR_TARIFA_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal ValorTarifaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "SFT_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RecalculoFrete", Column = "SFT_RECALCULO_FRETE", TypeType = typeof(Enumeradores.RecalculoFrete), NotNull = true)]
        public virtual Enumeradores.RecalculoFrete RecalculoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigePesoChegada", Column = "SFT_EXIGE_PESO_CHEGADA", TypeType = typeof(Enumeradores.ExigePesoChegada), NotNull = true)]
        public virtual Enumeradores.ExigePesoChegada ExigePesoChegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoQuebra", Column = "SFT_TIPO_QUEBRA", TypeType = typeof(Enumeradores.TipoQuebra), NotNull = true)]
        public virtual Enumeradores.TipoQuebra TipoQuebra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTolerancia", Column = "SFT_TIPO_TOLERANCIA", TypeType = typeof(Enumeradores.TipoTolerancia), NotNull = true)]
        public virtual Enumeradores.TipoTolerancia TipoTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualTolerancia", Column = "SFT_PERCENTUAL_TOLERANCIA", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal PercentualTolerancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaSuperior", Column = "SFT_PERCENTUAL_TOLERANCIA_SUPERIOR", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal PercentualToleranciaSuperior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "SFT_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSeguro", Column = "SFT_VALOR_SEGURO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTarifaEmissaoCartao", Column = "SFT_VALOR_TARIFA_EMISSAO_CARTAO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorTarifaEmissaoCartao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "SFT_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCartaoPedagio", Column = "SFT_VALOR_CARTAO_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorCartaoPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIRRF", Column = "SFT_VALOR_IRRF", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorIRRF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorINSS", Column = "SFT_VALOR_INSS", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSENAT", Column = "SFT_VALOR_SENAT", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorSEST", Column = "SFT_VALOR_SEST", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutrosDescontos", Column = "SFT_VALOR_OUTROS_DESCONTOS", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = true)]
        public virtual decimal ValorOutrosDescontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetorno", Column = "SFT_CODIGO_RETORNO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string CodigoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NSUFastCred", Column = "SFT_NSU_FAST_CRED", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string NSUFastCred { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContrato", Column = "SFT_NUMERO_CONTRATO", TypeType = typeof(string), Length = 12, NotNull = false)]
        public virtual string NumeroContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAbastecimento", Column = "SFT_VALOR_ABASTECIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 8, NotNull = false)]
        public virtual decimal ValorAbastecimento { get; set; }
    }
}
