using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_OBSERVACAO_CARTA_CORRECAO", EntityName = "NotaFiscalObservacaoCartaCorrecao", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao", NameType = typeof(NotaFiscalObservacaoCartaCorrecao))]
    public class NotaFiscalObservacaoCartaCorrecao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NOC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "NOC_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Especificacao", Column = "NOC_ESPECIFICACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Especificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NOC_STATUS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Status { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Especificacao;
            }
        }

        public virtual bool Equals(NotaFiscalObservacaoCartaCorrecao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoStatus
        {
            get
            {
                if (this.Status)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
