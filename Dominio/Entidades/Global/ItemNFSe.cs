namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_ITEM", EntityName = "ItemNFSe", Name = "Dominio.Entidades.ItemNFSe", NameType = typeof(ItemNFSe))]
    public class ItemNFSe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoNFSe", Column = "SER_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ServicoNFSe Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_MUNICIPIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Municipio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_MUNICIPIO_INCIDENCIA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade MunicipioIncidencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pais", Column = "PAI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pais PaisPrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ServicoPrestadoNoPais", Column = "NFI_PRESTADO_PAIS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ServicoPrestadoNoPais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorServico", Column = "NFI_VALOR_SERVICO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "NFI_QUANTIDADE", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal Quantidade { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "NFI_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoCondicionado", Column = "NFI_VALOR_DESC_CONDICIONADO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorDescontoCondicionado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoIncondicionado", Column = "NFI_VALOR_DESC_INCONDICIONADO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorDescontoIncondicionado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDeducoes", Column = "NFI_VALOR_DEDUCOES", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorDeducoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoISS", Column = "NFI_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaISS", Column = "NFI_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 6, NotNull = true)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "NFI_VALOR_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Discriminacao", Column = "NFI_DISCRIMINACAO", TypeType = typeof(string), Length = 2000, NotNull = true)]
        public virtual string Discriminacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigibilidadeISS", Column = "NFI_EXIGIBILIDADE_ISS", TypeType = typeof(Enumeradores.ExigibilidadeISS), NotNull = true)]
        public virtual Enumeradores.ExigibilidadeISS ExigibilidadeISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirISSNoFrete", Column = "NFI_INCLUIR_ISS", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao IncluirISSNoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFI_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFI_CODIGO_INDICADOR_OPERACAO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string CodigoIndicadorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "NFI_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "NFI_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "NFI_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "NFI_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "NFI_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "NFI_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "NFI_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "NFI_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "NFI_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "NFI_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "NFI_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "NFI_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTPIS", Column = "NFI_CST_PIS", Type = "AnsiString", Length = 5, NotNull = false)]
        public virtual string CSTPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTCOFINS", Column = "NFI_CST_COFINS", Type = "AnsiString", Length = 5, NotNull = false)]
        public virtual string CSTCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoPIS", Column = "NFI_BASE_CALCULO_PIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoCOFINS", Column = "NFI_BASE_CALCULO_COFINS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPIS", Column = "NFI_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINS", Column = "NFI_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPIS", Column = "NFI_VALOR_PIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINS", Column = "NFI_VALOR_COFINS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCOFINS { get; set; }
    }
}
