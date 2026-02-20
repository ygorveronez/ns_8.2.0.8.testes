using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROCESSAMENTO_FINALIZACAO_COLETA_ENTREGA_EM_LOTE", EntityName = "ProcessamentoFinalizacaoColetaEntregaEmLote", Name = "Dominio.Entidades.Embarcador.Frete.ProcessamentoFinalizacaoColetaEntregaEmLote", NameType = typeof(ProcessamentoFinalizacaoColetaEntregaEmLote))]
    public class ProcessamentoFinalizacaoColetaEntregaEmLote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCarga", Column = "CAR_CODIGO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProcessamento", Column = "PFC_DATA_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "PFC_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoFinalizacaoColetaEntregaEmLote), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoFinalizacaoColetaEntregaEmLote Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tentativas", Column = "PFC_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int Tentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PFC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }
    }
}
