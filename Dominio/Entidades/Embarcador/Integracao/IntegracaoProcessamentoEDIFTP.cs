using Dominio.Entidades.Embarcador.Pessoas;
using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_PROCESSAMENTO_EDI_FTP", EntityName = "IntegracaoProcessamentoEDIFTP", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP", NameType = typeof(IntegracaoProcessamentoEDIFTP))]
    public class IntegracaoProcessamentoEDIFTP : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "IPE_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "IPE_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "IPE_MENSAGEM_RETORNO", Type = "StringClob", NotNull = true)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "IPE_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "IPE_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracaoEDIFTP", Column = "IPE_SITUACAO_INTEGRACAO_EDI_FTP", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP SituacaoIntegracaoEDIFTP { get; set; }

    }
}
