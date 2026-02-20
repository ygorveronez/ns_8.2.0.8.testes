using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_ALTERACAO_REGRA_ICMS", EntityName = "RegraAutorizacaoAlteracaoRegraICMS", Name = "Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS", NameType = typeof(RegraAutorizacaoAlteracaoRegraICMS))]
    public class RegraAutorizacaoAlteracaoRegraICMS : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ALTERACAO_REGRA_ICMS_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return true;
        }

        public override void LimparAlcadas()
        {
        }

        #endregion
    }
}
