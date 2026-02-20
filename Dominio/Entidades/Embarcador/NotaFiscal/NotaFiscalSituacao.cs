using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_SITUACAO", EntityName = "NotaFiscalSituacao", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao", NameType = typeof(NotaFiscalSituacao))]
    public class NotaFiscalSituacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFS_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFS_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFS_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFS_BLOQUEAR_VISUALIZACAO_AGENDAMENTO_ENTREGA_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearVisualizacaoAgendamentoEntregaPedido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "NFS_FINALIZAR_AGENDAMENTO_ENTREGA_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FinalizarAgendamentoEntregaPedido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "NFS_GATILHO", TypeType = typeof(NotaFiscalSituacaoGatilho), NotNull = false)]
        public virtual NotaFiscalSituacaoGatilho NotaFiscalSituacaoGatilho { get; set; }
        
        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }
    }
}
