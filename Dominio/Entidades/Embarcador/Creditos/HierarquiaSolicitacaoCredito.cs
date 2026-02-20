using System;

namespace Dominio.Entidades.Embarcador.Creditos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CREDITO_HIERARQUIA_SOLICITACAO", EntityName = "HierarquiaSolicitacaoCredito", Name = "Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito", NameType = typeof(HierarquiaSolicitacaoCredito))]
    public class HierarquiaSolicitacaoCredito : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HSC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_SOLICITANTE", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Solicitante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CREDITOR", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Creditor { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Solicitante?.Nome ?? string.Empty;
            }
        }

        public virtual bool Equals(HierarquiaSolicitacaoCredito other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
