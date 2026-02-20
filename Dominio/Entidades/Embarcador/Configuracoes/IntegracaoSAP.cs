namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SAP", EntityName = "IntegracaoSAP", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP", NameType = typeof(IntegracaoSAP))]
    public class IntegracaoSAP : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIS_POSSUI_INTEGRACAO_SAP", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_INTEGRACAO_SAP", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_ENVIA_VENDA_FRETE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnviaVendaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_USUARIO_INTEGRACAO_SAP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_SENHA_INTEGRACAO_SAP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RealizarIntegracaoComDadosFatura", Column = "CIS_REALIZAR_INTEGRACAO_COM_DADOS_FATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarIntegracaoComDadosFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_DESCONTO_AVARIA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLDescontoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_INTEGRACAO_FATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_INTEGRACAO_ESTORNO_FATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLIntegracaoEstornoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_CONSULTA_ESTORNO_FATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLConsultaEstornoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_CRIAR_SALDO_FRETE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLCriarSaldoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_CONSULTA_DOCUMENTOS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLConsultaDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_CONSULTA_FATURA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLConsultaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_SOLICITACAO_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLSolicitacaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_SOLICITACAO_CANCELAMENTO_CTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLSolicitacaoCancelamentoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIS_URL_ENVIA_VENDA_SERVICO_NFSE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URLEnviaVendaServicoNFSe { get; set; }
        
    }
}