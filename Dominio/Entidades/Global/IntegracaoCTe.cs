namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_CTE", EntityName = "IntegracaoCTe", Name = "Dominio.Entidades.IntegracaoCTe", NameType = typeof(IntegracaoCTe))]
    public class IntegracaoCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "ICT_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Arquivo", Column = "ICT_ARQUIVO", TypeType = typeof(string), Length = 10000, NotNull = true)]
        public virtual string Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaCarga", Column = "ICT_NUMERO_CARGA", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaUnidade", Column = "ICT_NUMERO_UNIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroDaUnidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ICT_STATUS", TypeType = typeof(Enumeradores.StatusIntegracao), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusIntegracao Status { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoArquivo", Column = "ICT_TIPO_ARQUIVO", TypeType = typeof(Enumeradores.TipoArquivoIntegracao), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoArquivoIntegracao TipoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "ICT_TIPO", TypeType = typeof(Enumeradores.TipoIntegracao), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoIntegracao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoControleInternoCliente", Column = "ICT_CODIGO_CONTROLE_INTERNO_CLIENTE", TypeType = typeof(string), Length=200, NotNull = false)]
        public virtual string CodigoControleInternoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTipoOperacao", Column = "ICT_CODIGO_TIPO_OPERACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Romaneio", Column = "ICT_ROMANEIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Romaneio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoVeiculo", Column = "ICT_TIPO_VEICULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCalculo", Column = "ICT_TIPO_CALCULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TipoCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDespesa", Column = "ICT_VALOR_DESPESA", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal ValorDespesa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouCargaEmbarcador", Column = "ICT_GEROU_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouCargaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizarCarga", Column = "ICT_FINALIZAR_CARGA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string FinalizarCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FinalizouCarga", Column = "ICT_FINALIZOU_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizouCarga { get; set; }
            
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tentativas", Column = "ICT_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int Tentativas { get; set; }
    }
}
