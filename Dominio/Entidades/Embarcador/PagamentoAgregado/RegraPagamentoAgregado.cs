using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.PagamentoAgregado
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PAGAMENTO_AGREGADO", EntityName = "RegraPagamentoAgregado", Name = "Dominio.Entidades.Embarcador.PagamentoAgregado.RegraPagamentoAgregado", NameType = typeof(RegraPagamentoAgregado))]
    public class RegraPagamentoAgregado : Alcada.RegraAprovacao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPA_CLIENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPA_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PAGAMENTO_AGREGADO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPessoa", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PAGAMENTO_AGREGADO_PESSOA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaPessoaPagamentoAgregado", Column = "RPP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaPessoaPagamentoAgregado> AlcadasPessoa { get; set; }
        // --------------------------------------

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PAGAMENTO_AGREGADO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaValorPagamentoAgregado", Column = "RPV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.PagamentoAgregado.AlcadaValorPagamentoAgregado> AlcadasValor { get; set; }
        // --------------------------------------

    }
}
