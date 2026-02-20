using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_QUALIDADE", EntityName = "CargaEntregaQualidade", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaQualidade", NameType = typeof(CargaEntregaQualidade))]
    public class CargaEntregaQualidade : EntidadeBase
    {
        public CargaEntregaQualidade() { }

        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEQ_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraDataConfirmacaoIntervaloRaio", Column = "CEQ_REGRA_DATA_CONFIRMACAO_INTERVALO_RAIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraDataConfirmacaoIntervaloRaio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHoraRegraDataConfirmacaoIntervaloRaio", Column = "CEQ_DATA_HORA_REGRA_DATA_CONFIRMACAO_INTERVALO_RAIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHoraRegraDataConfirmacaoIntervaloRaio { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras
        public virtual string Descricao
        {
            get
            {
                return "Qualidade da Coleta/Entrega";
            }
        }
        #endregion Propriedades com Regras
    }
}