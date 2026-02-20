using System;

namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CHAMADO_TMS_ADIANTAMENTO_MOTORISTA", EntityName = "ChamadoTMSAdiantamentoMotorista", Name = "Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAdiantamentoMotorista", NameType = typeof(ChamadoTMSAdiantamentoMotorista))]
    public class ChamadoTMSAdiantamentoMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CHM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChamadoTMS", Column = "CHT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ChamadoTMS Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTipo", Column = "PMT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotorista.PagamentoMotoristaTipo PagamentoMotoristaTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CHM_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CHM_VALOR", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 15)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CHM_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.PagamentoMotoristaTipo?.Descricao ?? string.Empty) + " - " + (this.Chamado?.Descricao ?? string.Empty);
            }
        }
    }
}
