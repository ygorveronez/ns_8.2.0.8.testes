using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_TOKEN", EntityName = "RegraAutorizacaoToken", Name = "Dominio.Entidades.Embarcador.Transportadores.RegraAutorizacaoToken", NameType = typeof(RegraAutorizacaoToken))]
    public class RegraAutorizacaoToken : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_ETAPA_AUTORIZACAO_TOKEN", TypeType = typeof(EtapaAutorizacaoToken), NotNull = false)]
        public virtual EtapaAutorizacaoToken EtapaAutorizacaoToken { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroReprovadores", Column = "RAT_NUMERO_REPROVADORES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroReprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrazoAprovacaoAutomatica", Column = "RAT_PRAZO_APROVACAO_AUTOMATICA", TypeType = typeof(int), NotNull = false)]
        public virtual int PrazoAprovacaoAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDiasAprovacao", Column = "RAT_TIPO_DIAS_APROVACAO", TypeType = typeof(TipoDiasAprovacao), NotNull = false)]
        public virtual TipoDiasAprovacao TipoDiasAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarLinkParaAprovacaoPorEmail", Column = "RAT_ENVIAR_LINK_PARA_APROVACAO_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarLinkParaAprovacaoPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_DIAS_PRAZO_APROVACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? PrazoAprovacao { get; set; }
        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_AUTORIZACAO_TOKEN_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        public override bool IsAlcadaAtiva()
        {
            return true;
        }

        public override void LimparAlcadas()
        {
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        #endregion
    }
}
