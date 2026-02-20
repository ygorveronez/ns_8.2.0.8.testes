namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_CONFIGURACAO_NFSE", EntityName = "TransportadorConfiguracaoNFSe", Name = "Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe", NameType = typeof(TransportadorConfiguracaoNFSe))]
    public class TransportadorConfiguracaoNFSe : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ECN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoNFSe", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ServicoNFSe ServicoNFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaNFSe", Column = "NAN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.NaturezaNFSe NaturezaNFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_PRESTACAO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadePrestacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "COF_SERIE_NFSE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie SerieNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieRPS", Column = "ECN_SERIE_RPS", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string SerieRPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URLPrefeitura", Column = "ECN_URL_PREFEITURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLPrefeitura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FraseSecreta", Column = "ECN_FRASE_SECRETA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string FraseSecreta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LoginSitePrefeitura", Column = "ECN_LOGIN_PREFEITURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string LoginSitePrefeitura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NBS", Column = "ECN_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaSitePrefeitura", Column = "ECN_SENHA_PREFEITURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string SenhaSitePrefeitura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoIntegracao", Column = "ECN_OBSERVACAO_INTEGRACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiscriminacaoNFSe", Column = "ECN_DISCRIMINACAO_NFSE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string DiscriminacaoNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaISS", Column = "ECN_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 6, NotNull = false)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetencaoISS", Column = "ECN_RETENCAO_ISS", TypeType = typeof(decimal), Scale = 2, Precision = 5, NotNull = false)]
        public virtual decimal RetencaoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ExigibilidadeISS", Column = "ECN_EXIGIBILIDADE_ISS", TypeType = typeof(Dominio.Enumeradores.ExigibilidadeISS), NotNull = false)]
        public virtual Dominio.Enumeradores.ExigibilidadeISS ExigibilidadeISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirISSBaseCalculo", Column = "ECN_INCLUIR_ISS_BASE_CALCULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirISSBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RealizarArredondamentoCalculoIss", Column = "ECN_REALIZAR_ARREDONDAMENTO_ISS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarArredondamentoCalculoIss { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncidenciaISSLocalidadePrestador", Column = "ECN_INCIDENCIA_ISS_LOCALIDADE_PRESTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncidenciaISSLocalidadePrestador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECN_PERMITE_ANULAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermiteAnular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECN_PRAZO_CANCELAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoCancelamento { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado UFTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadeTomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteTomador { get; set; }

        /// <summary>
        /// TODO: Se True, não irá emitir NFS para a carga, porém irá gerar os custos de ISS para provisão de Frete (apenas embarcador)
        /// </summary>

        [NHibernate.Mapping.Attributes.Property(0, Column = "ECN_CONFIGURACAO_PARA_PROVSIAO_DE_ISS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConfiguracaoParaProvisaoDeISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReterIReDestacarNFs", Column = "ECN_RETER_DESTACAR_IR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReterIReDestacarNFs { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIR", Column = "ECN_ALIQUOTA_IR", TypeType = typeof(decimal), Scale = 4, Precision = 6, NotNull = false)]
        public virtual decimal AliquotaIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIR", Column = "ECN_BASE_CALCULO_IR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal BaseCalculoIR { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoEnviarAliquotaEValorISS", Column = "ECN_NAO_ENVIAR_ALIQUOTA_E_VALOR_ISS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarAliquotaEValorISS { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Empresa.RazaoSocial;
            }
        }

    }
}
