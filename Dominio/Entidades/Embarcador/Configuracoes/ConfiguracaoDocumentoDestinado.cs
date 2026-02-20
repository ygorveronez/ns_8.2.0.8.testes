using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_DOCUMENTO_DESTINADO", EntityName = "ConfiguracaoDocumentoDestinado", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado", NameType = typeof(ConfiguracaoDocumentoDestinado))]
    public class ConfiguracaoDocumentoDestinado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDD_BLOQUEAR_LANCAMENTO_DOCUMENTOS_TIPO_ENTRADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearLancamentoDocumentosTipoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDD_SALVAR_DOCUMENTOS_RECEBIDOS_EMAIL_DESTINADOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SalvarDocumentosRecebidosEmailDestinados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDD_VINCULAR_CTE_NA_OCORRENCIA_APARTIR_DA_OBSERVACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VincularCteNaOcorrenciaApartirDaObservacao { get; set; }

        [Obsolete("Por padrão agora sempre será inutilizado gerencialmente, não existe mais inutilização na sefaz")]
        [NHibernate.Mapping.Attributes.Property(0, Column = "CDD_NAO_INUTILIZAR_CTES_FISCALMENTE_APENAS_GERENCIALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoInutilizarCTEsFiscalmenteApenasGerencialmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDD_NAO_REUTILIZAR_NUMERACAO_APOS_ANULAR_GERENCIALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoReutilizarNumeracaoAposAnularGerencialmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDD_NAO_SALVAR_XML_APENAS_NA_FALHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSalvarXmlApenasNaFalha { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para os documentos destinados"; }
        }

    }
}
