using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_ARQUIVOS", EntityName = "NotaFiscalArquivos", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos", NameType = typeof(NotaFiscalArquivos))]
    public class NotaFiscalArquivos : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLDistribuicao", Column = "NFA_XML_DISTRIBUICAO", Type = "StringClob", NotNull = false)]
        public virtual string XMLDistribuicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLCancelamento", Column = "NFA_XML_CANCELAMENTO", Type = "StringClob", NotNull = false)]
        public virtual string XMLCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLCartaCorrecao", Column = "NFA_XML_CARTA_CORRECAO", Type = "StringClob", NotNull = false)]
        public virtual string XMLCartaCorrecao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLInutilizacao", Column = "NFA_XML_INUTILIZACAO", Type = "StringClob", NotNull = false)]
        public virtual string XMLInutilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLNaoAssinado", Column = "NFA_XML_NAO_ASSINADO", Type = "StringClob", NotNull = false)]
        public virtual string XMLNaoAssinado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLCancelamentoNaoAssinado", Column = "NFA_XML_CANCELAMENTO_NAO_ASSINADO", Type = "StringClob", NotNull = false)]
        public virtual string XMLCancelamentoNaoAssinado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLInutilizacaoNaoAssinado", Column = "NFA_XML_INUTILIZACAO_NAO_ASSINADO", Type = "StringClob", NotNull = false)]
        public virtual string XMLInutilizacaoNaoAssinado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XMLCartaCorrecaoNaoAssinado", Column = "NFA_XML_CARTA_CORRECAO_NAO_ASSINADO", Type = "StringClob", NotNull = false)]
        public virtual string XMLCartaCorrecaoNaoAssinado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }

        public virtual bool Equals(NotaFiscalArquivos other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
