using System;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LANCAMENTO_NFS_AUTORIZACAO", EntityName = "LancamentoNFSAutorizacao", Name = "Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao", NameType = typeof(LancamentoNFSAutorizacao))]
    public class LancamentoNFSAutorizacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAA_BLOQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bloqueada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LancamentoNFSManual LancamentoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoNFSManual", Column = "RAN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoNFSManual RegrasAutorizacaoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRejeicaoLancamentoNFS", Column = "MRL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoRejeicaoLancamentoNFS MotivoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAA_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LAA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Aprovada:
                        return "Aprovada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Rejeitada:
                        return "Rejeitada";
                    default:
                        return "";
                }
            }
        }
    }
}
