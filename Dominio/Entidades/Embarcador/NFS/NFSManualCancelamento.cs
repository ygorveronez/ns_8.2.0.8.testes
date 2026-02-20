using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFS_MANUAL_CANCELAMENTO", EntityName = "NFSManualCancelamento", Name = "Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento", NameType = typeof(NFSManualCancelamento))]
    public class NFSManualCancelamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LancamentoNFSManual", Column = "LNM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual LancamentoNFSManual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamento", Column = "CAC_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoCancelamento", Column = "NMC_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NMC_SITUACAO_CANCELAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento SituacaoNFSManualCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicaoCancelamento", Column = "NMC_MOTIVO_REJEICAO_CANCELAMENTO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string MotivoRejeicaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NMC_GERANDO_INTEGRACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerandoIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NMC_CANCELOU_DOCUMENTOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CancelouDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NFS_MANUAL_CANCELAMENTO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NMC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NFSManualCancelamentoIntegracao", Column = "ILN_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracao> Integracoes { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.LancamentoNFSManual?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (SituacaoNFSManualCancelamento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.EmCancelamento:
                        return "Em Cancelamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.Cancelada:
                        return "Cancelada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.CancelamentoRejeitado:
                        return "Rejeição no Cancelamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.AgIntegracao:
                        return "Ag. Integrações";
                    default:
                        return string.Empty;
                }
            }
        }

    }
}
