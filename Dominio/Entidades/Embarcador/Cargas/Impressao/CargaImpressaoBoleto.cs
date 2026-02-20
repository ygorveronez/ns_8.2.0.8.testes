namespace Dominio.Entidades.Embarcador.Cargas.Impressao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_IMPRESSAO_BOLETO", EntityName = "CargaImpressaoBoleto", Name = "Dominio.Entidades.Embarcador.Cargas.CargaImpressaoBoleto", NameType = typeof(CargaImpressaoBoleto))]
    public class CargaImpressaoBoleto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoImpressao", Column = "CIB_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao SituacaoImpressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBanco", Column = "CIB_CODIGO_BANCO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeBanco", Column = "CIB_NOME_BANCO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "CIB_AGENCIA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NossoNumero", Column = "CIB_NOSSO_NUMERO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NossoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "CIB_DATA_VENCIMENTO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LinhaDigitavel", Column = "CIB_LINHA_DIGITAVEL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string LinhaDigitavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarras", Column = "CIB_CODIGO_BARRAS", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string CodigoBarras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "CIB_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDocumento", Column = "CIB_DATA_DOCUMENTO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string DataDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LocalPagamento", Column = "CIB_LOCAL_PAGAMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string LocalPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EspecieDocumento", Column = "CIB_ESPECIE_DOCUMENTO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string EspecieDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aceite", Column = "CIB_ACEITE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Aceite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProcessamento", Column = "CIB_DATA_PROCESSAMENTO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string DataProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsoBanco", Column = "CIB_USO_BANCO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UsoBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Carteira", Column = "CIB_CARTEIRA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Carteira { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EspecieMoeda", Column = "CIB_ESPECIE_MOEDA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string EspecieMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "CIB_QUANTIDADE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Instrucoes", Column = "CIB_INSTRUCOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Instrucoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InstrucoesAdicional", Column = "CIB_INSTRUCOES_ADICIONAL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string InstrucoesAdicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SacadoCNPJ", Column = "CIB_SACADO_CNPJ", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SacadoCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SacadoIE", Column = "CIB_SACADO_IE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SacadoIE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SacadoNome", Column = "CIB_SACADO_NOME", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SacadoNome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SacadoRua", Column = "CIB_SACADO_RUA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SacadoRua { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SacadoNumero", Column = "CIB_SACADO_NUMERO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string SacadoNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SacadoComplemento", Column = "CIB_SACADO_COMPLEMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SacadoComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SacadoCEP", Column = "CIB_SACADO_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string SacadoCEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SacadoBairro", Column = "CIB_SACADO_BAIRRO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SacadoBairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SacadoCidade", Column = "CIB_SACADO_CIDADE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SacadoCidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SacadoEstado", Column = "CIB_SACADO_ESTADO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string SacadoEstado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteCodigo", Column = "CIB_CEDENTE_CEDENTE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CedenteCodigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteCNPJ", Column = "CIB_CEDENTE_CNPJ", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CedenteCNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteIE", Column = "CIB_CEDENTE_IE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CedenteIE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteNome", Column = "CIB_CEDENTE_NOME", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CedenteNome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteRua", Column = "CIB_CEDENTE_RUA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CedenteRua { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteNumero", Column = "CIB_CEDENTE_NUMERO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CedenteNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteComplemento", Column = "CIB_CEDENTE_COMPLEMENTO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CedenteComplemento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteCEP", Column = "CIB_CEDENTE_CEP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CedenteCEP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteBairro", Column = "CIB_CEDENTE_BAIRRO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CedenteBairro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteCidade", Column = "CIB_CEDENTE_CIDADE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CedenteCidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CedenteEstado", Column = "CIB_CEDENTE_ESTADO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CedenteEstado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDocumento", Column = "CIB_VALOR_DOCUMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoAcrescimo", Column = "CIB_VALOR_DESCONTO_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorDescontoAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCobrado", Column = "CIB_VALOR_COBRADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCobrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoBanco", Column = "CIB_DIGITO_BANCO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string DigitoBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CIP", Column = "CIB_CIP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CIP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoPDF", Column = "CIB_CAMINHO_PDF", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CaminhoPDF { get; set; }
    }
}
