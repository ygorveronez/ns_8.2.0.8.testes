using System;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DADOS_NFS_MANUAL", EntityName = "DadosNFSManual", Name = "Dominio.Entidades.Embarcador.NFS.DadosNFSManual", NameType = typeof(DadosNFSManual))]
    public class DadosNFSManual : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NFS.DadosNFSManual>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NSM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscalServico", Column = "NFS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual LancamentoNFSAutorizacao NotaFiscalServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NSM_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "NSM_VALOR_FRETE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontos", Column = "NSM_VALOR_DESCONTOS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorDescontos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBaseCalculo", Column = "NSM_VALOR_FRETE_BASE_CALCULO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaISS", Column = "NSM_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirISSBC", Column = "NSM_INCLUIR_ISS_BC", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirISSBC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarLocalidadeCarga", Column = "NSM_CONSIDERAR_LOCALIDADE_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarLocalidadeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "NSM_VALOR_ISS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoArredondamentoISS", Column = "NSM_TIPO_ARREDONDAMENTO_ISS", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamentoNFSManual), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamentoNFSManual TipoArredondamentoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorRetido", Column = "NSM_VALOR_RETIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualRetencao", Column = "NSM_PERCENTUAL_RETENCAO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualRetencao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "ESE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie Serie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "NSM_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "NSM_OBSERVACOES", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImagemNFS", Column = "NSM_IMAGEM_NFS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ImagemNFS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NSM_ANEXO_NFS", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string AnexoNFS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "NSM_XML", Type = "StringClob", NotNull = false)]
        public virtual string XML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NSM_VALOR_PIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NSM_VALOR_COFINS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NSM_VALOR_IR", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NSM_VALOR_CSLL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCSLL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NSM_VALOR_RECEBER", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoAutenticidade", Column = "NSM_CODIGO_AUTENTICIDADE", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CodigoAutenticidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRPS", Column = "NSM_NUMERO_RPS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroRPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NSM_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NSM_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoNFSe", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ServicoNFSe ServicoNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "NSM_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "NSM_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "NSM_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "NSM_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "NSM_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "NSM_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "NSM_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "NSM_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "NSM_BASE_CALCULO_IBS_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "NSM_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "NSM_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "NSM_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NBS", Column = "NSM_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorOperacao", Column = "NSM_VALOR_INDICADOR_OPERACAO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string IndicadorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPISIBSCBS", Column = "NSM_VALOR_PIS_IBS_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPISIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINSIBSCBS", Column = "NSM_VALOR_COFINS_IBS_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCOFINSIBSCBS { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual bool Equals(DadosNFSManual other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
