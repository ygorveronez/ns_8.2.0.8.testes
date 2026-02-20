using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_PROVEDOR", EntityName = "PagamentoProvedor", Name = "Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedor", NameType = typeof(PagamentoProvedor))]
    public class PagamentoProvedor : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_PROVEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Provedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoProvedor", Column = "PRO_TIPO_DOCUMENTO_PROVEDOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoProvedor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoProvedor TipoDocumentoProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoLiberacaoPagamentoProvedor", Column = "PRO_SITUACAO_PROVEDOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLiberacaoPagamentoProvedor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLiberacaoPagamentoProvedor SituacaoLiberacaoPagamentoProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaLiberacaoPagamentoProvedor", Column = "PRO_ETAPA_LIBERACAO_PAGAMENTO_PROVEDOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaLiberacaoPagamentoProvedor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaLiberacaoPagamentoProvedor EtapaLiberacaoPagamentoProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAlteracaoRegraPagamentoProvedor", Column = "PRO_SITUACAO_ALTERACAO_PAGAMENTO_PROVEDOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoRegraPagamentoProvedor), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoRegraPagamentoProvedor SituacaoAlteracaoRegraPagamentoProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_MOTIVO_APROVACAO_REGRA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string MotivoAprovacaoRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PRO_MOTIVO_REJEICAO_REGRA", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string MotivoRejeicaoRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProvedor", Column = "PRO_VALOR_PROVEDOR", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorProvedor { get; set; }

        [Obsolete ("Campo criado incorretamente, migrado para LocalidadesPrestacoes")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade LocalidadePrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoNFSe", Column = "PRO_DATA_EMISSAO_NFSE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNFSe", Column = "PRO_NUMERO_NFSE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorNFSe", Column = "PRO_VALOR_NFSE", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorNFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "PRO_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRejeicao", Column = "PRO_DATA_REJEICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinalizacao", Column = "PRO_DATA_FINALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MultiplosCTe", Column = "PRO_MULTIPLOS_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MultiplosCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCTes", Column = "PRO_NUMERO_CTES", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string NumeroCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCTes", Column = "PRO_VALOR_CTES", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCTeProvedor", Column = "PRO_ALIQUOTA_CTE_PROVEDOR", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal AliquotaCTeProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSProvedor", Column = "PRO_VALOR_ICMS_PROVEDOR", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorICMSProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoCTes", Column = "PRO_DATA_EMISSAO_CTES", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoCTes { get; set; }

        #region CT-e Recebido

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAReceberCTe", Column = "PRO_VALOR_A_RECEBER_CTE", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorAReceberCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCTe", Column = "PRO_ALIQUOTA_CTE", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal AliquotaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSCTe", Column = "PRO_ICMS", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorICMSCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EMISSOR_CTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente EmissorCTe { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.Empresa Emissor { get; set; }

        #endregion

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "PRO_ARQUIVO_XML_CTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoXMLCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "PRO_ARQUIVO_XML_NFSE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoXMLNFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "PRO_ARQUIVO_PDF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoPDF { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PagamentoProvedorCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PAGAMENTO_PROVEDOR_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "PagamentoProvedorCarga")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorCarga> PagamentoProvedorCarga { get; set; }        
        
        [NHibernate.Mapping.Attributes.Set(0, Name = "PagamentoProvedorLocalidadePrestacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PAGAMENTO_PROVEDOR_LOCALIDADE_PRESTACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "PagamentoProvedorLocalidadePrestacao")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Financeiro.PagamentoProvedorLocalidadePrestacao> PagamentoProvedorLocalidadePrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NovoFluxoIniciado", Column = "PRO_NOVO_FLUXO_INICIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NovoFluxoIniciado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraICMSCTe", Column = "PRO_REGRA_ICMS_CTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string RegraICMSCTe { get; set; }

        public virtual bool Equals(PagamentoProvedor other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao { get { return this.Codigo.ToString(); } }
    }
}
