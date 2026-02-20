using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_FINALIZACAO_ASSINCRONA", EntityName = "CargaEntregaFinalizacaoAssincrona", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona", NameType = typeof(CargaEntregaFinalizacaoAssincrona))]
    public class CargaEntregaFinalizacaoAssincrona : EntidadeBase
    {
        #region Propriedades
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "EFA_PARAMETROS_FINALIZACAO", Type = "StringClob", NotNull = true)]
        public virtual string ParametrosFinalizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInclusao", Column = "EFA_DATA_INCLUSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInclusao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProcessamento", Column = "EFA_DATA_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoProcessamento", Column = "EFA_SITUACAO_PROCESSAMENTO", TypeType = typeof(SituacaoProcessamentoIntegracao), NotNull = false)]
        public virtual SituacaoProcessamentoIntegracao SituacaoProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DetalhesProcessamento", Column = "EFA_DETALHES_PROCESSAMENTO", TypeType = typeof(string), NotNull = false)]
        public virtual string DetalhesProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "EFA_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativas { get; set; }
        #endregion Propriedades

        #region Propriedades com Regras
        public virtual string Descricao
        {
            get
            {
                return string.Empty;
            }
        }
        #endregion Métodos
    }
}