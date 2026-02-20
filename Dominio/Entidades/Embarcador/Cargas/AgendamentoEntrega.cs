using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDAMENTO_ENTREGA", EntityName = "AgendamentoEntrega", Name = "Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega", NameType = typeof(AgendamentoEntrega))]
    public class AgendamentoEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AGE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaAgendamento", Column = "AGE_SENHA_AGENDAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int SenhaAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "AGE_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Transportador", Column = "AGE_TRANSPORTADOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "AGE_PLACA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Placa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motorista", Column = "AGE_MOTORISTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "AGE_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAgendamento", Column = "AGE_DATA_AGENDAMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataAgendamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "AGE_SITUACAO", TypeType = typeof(SituacaoAgendamentoEntrega), NotNull = false)]
        public virtual SituacaoAgendamentoEntrega Situacao { get; set; }

        public virtual string Descricao
        {
            get { return SenhaAgendamento.ToString(); }
        }
    }
}
