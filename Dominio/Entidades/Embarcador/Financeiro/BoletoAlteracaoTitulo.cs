using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BOLETO_ALTERACAO_TITULO", EntityName = "BoletoAlteracaoTitulo", Name = "Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo", NameType = typeof(BoletoAlteracaoTitulo))]
    public class BoletoAlteracaoTitulo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.BoletoAlteracaoTitulo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoOriginal", Column = "BAT_VENCIMENTO_ORIGINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataVencimentoOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoAlterado", Column = "BAT_VENCIMENTO_ALTERADO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataVencimentoAlterado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoOriginal", Column = "BAT_OBSERVACAO_ORIGINAL", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ObservacaoOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAlterada", Column = "BAT_OBSERVACAO_ALTERADAL", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ObservacaoAlterada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BoletoStatusTitulo", Column = "BAT_STATUS_BOLETO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo BoletoStatusTitulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoRemessa", Column = "BRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BoletoRemessa BoletoRemessa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoAlteracao", Column = "BAL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BoletoAlteracao BoletoAlteracao { get; set; }

        public virtual bool Equals(BoletoAlteracaoTitulo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
