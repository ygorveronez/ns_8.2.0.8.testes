using System;
using System.Collections.Generic;


namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_MDFE_MANUAL_CANCELAMENTO", DynamicUpdate = true, EntityName = "CargaMDFeManualCancelamento", Name = "Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento", NameType = typeof(CargaMDFeManualCancelamento))]
    public class CargaMDFeManualCancelamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaMDFeManual", Column = "CMM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual CargaMDFeManual { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCancelamento", Column = "CAC_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoCancelamento", Column = "CMC_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_SITUACAO_CANCELAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento SituacaoMDFeManualCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoRejeicaoCancelamento", Column = "CMC_MOTIVO_REJEICAO_CANCELAMENTO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string MotivoRejeicaoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RecebidoPorIntegracao", Column = "CMC_RECEBIDO_POR_INTEGRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RecebidoPorIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMC_GERANDO_INTEGRACOES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerandoIntegracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Integracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_MDFE_MANUAL_CANCELAMENTO_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaMDFeManualCancelamentoIntegracao", Column = "CMC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao> Integracoes { get; set; }
        public virtual string DescricaoSituacao
        {
            get
            {
                switch (SituacaoMDFeManualCancelamento)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.EmCancelamento:
                        return "Em Cancelamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.Cancelada:
                        return "Cancelada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.CancelamentoRejeitado:
                        return "Rejeição no Cancelamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.AgIntegracao:
                        return "Aguardando integrações";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.FalhaIntegracao:
                        return "Problema de integração";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.CargaMDFeManual.Descricao;
            }
        }
    }
}
