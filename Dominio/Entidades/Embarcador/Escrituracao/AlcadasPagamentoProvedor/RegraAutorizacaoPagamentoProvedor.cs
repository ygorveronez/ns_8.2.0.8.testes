using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_PAGAMENTO_PROVEDOR", EntityName = "RegraPagamentoProvedor", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamentoProvedor.RegraPagamentoProvedor", NameType = typeof(RegraPagamentoProvedor))]
    public class RegraPagamentoProvedor : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDiferencaValor", Column = "RAT_DIFERENCA_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorDiferencaValor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasDiferencaValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PAGAMENTO_PROVEDOR_DIFERENCA_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasPagamentoProvedor.AlcadasDiferencaValor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadasDiferencaValor> AlcadasDiferencaValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDiferencaValorMenor", Column = "RAT_DIFERENCA_VALOR_MENOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorDiferencaValorMenor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasDiferencaValorMenor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PAGAMENTO_PROVEDOR_DIFERENCA_VALOR_MENOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasPagamentoProvedor.AlcadasDiferencaValorMenor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadasDiferencaValorMenor> AlcadasDiferencaValorMenor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDiferencaValorMaior", Column = "RAT_DIFERENCA_VALOR_MAIOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorDiferencaValorMaior { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasDiferencaValorMaior", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PAGAMENTO_PROVEDOR_DIFERENCA_VALOR_MAIOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasPagamentoProvedor.AlcadasDiferencaValorMaior", Column = "ALC_CODIGO")]
        public virtual IList<AlcadasDiferencaValorMaior> AlcadasDiferencaValorMaior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidarTodosCamposAuditoriaDocumentoProvedor", Column = "RAT_VALIDAR_TODOS_CAMPOS_AUDITORIA_DOCUMENTO_PROVEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidarTodosCamposAuditoriaDocumentoProvedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearPagamentoMultiplosCTe", Column = "RAT_BLOQUEAR_PAGAMENTO_MULTIPLOS_CTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearPagamentoMultiplosCTe { get; set; }
        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PAGAMENTO_PROVEDOR_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (
                RegraPorDiferencaValor || RegraPorDiferencaValorMenor || RegraPorDiferencaValorMaior
            );
        }

        public override void LimparAlcadas()
        {
            AlcadasDiferencaValor?.Clear();
            AlcadasDiferencaValorMenor?.Clear();
            AlcadasDiferencaValorMaior?.Clear();
        }

        #endregion
    }
}
