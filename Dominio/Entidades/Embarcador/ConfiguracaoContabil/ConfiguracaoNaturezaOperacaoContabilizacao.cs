using System;

namespace Dominio.Entidades.Embarcador.ConfiguracaoContabil
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_NATUREZA_OPERACAO_CONTABILIZACAO", EntityName = "ConfiguracaoNaturezaOperacaoContabilizacao", Name = "Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao", NameType = typeof(ConfiguracaoNaturezaOperacaoContabilizacao))]
    public class ConfiguracaoNaturezaOperacaoContabilizacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacaoContabilizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoNaturezaOperacao", Column = "CNP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoNaturezaOperacao ConfiguracaoNaturezaOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.NaturezaDaOperacao NaturezaDaOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaContabil", Column = "CCE_TIPO_CONTA_CONTABIL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.ConfiguracaoNaturezaOperacao.Descricao + " (" + NaturezaDaOperacao.Descricao + ")";
            }
        }

        public virtual bool Equals(ConfiguracaoNaturezaOperacaoContabilizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
