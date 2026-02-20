using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Atendimento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ATENDIMENTO_TAREFA", EntityName = "AtendimentoTarefa", Name = "Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa", NameType = typeof(AtendimentoTarefa))]
    public class AtendimentoTarefa : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "ATC_PRIORIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento Prioridade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ATC_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoProblema", Column = "ATC_MOTIVO_PROBLEMA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string MotivoProblema { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaObservacao", Column = "ATC_JUSTIFICATIVA_SOLUCAO_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string JustificativaObservacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ATC_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraInicial", Column = "ATC_HORA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? HoraInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraFinal", Column = "ATC_HORA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? HoraFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Duracao", Column = "ATC_DURACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Duracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Atendimento", Column = "ATE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Atendimento.Atendimento Atendimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AtendimentoTela", Column = "ATL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Atendimento.AtendimentoTela AtendimentoTela { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AtendimentoTipo", Column = "ATT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo AtendimentoTipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AtendimentoModulo", Column = "ATM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Atendimento.AtendimentoModulo AtendimentoModulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AtendimentoSistema", Column = "ATS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Atendimento.AtendimentoSistema AtendimentoSistema { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ATENDIMENTO_TAREFA_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ATC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AtendimentoTarefaAnexo", Column = "ATN_CODIGO")]
        public virtual IList<Embarcador.Atendimento.AtendimentoTarefaAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulo", Column = "ATC_TITULO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_SOLICITANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Solicitante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroChamadoRedMine", Column = "ATC_NUMERO_CHAMADO_REDMINE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroChamadoRedMine { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Aberto:
                        return "Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Parcial:
                        return "Parcial";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.PendenteRetornoCliente:
                        return "Pendente Retorno Cliente";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.PendenteRetornoSuporte:
                        return "Pendente Retorno Suporte";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.EmTreinamento:
                        return "Em Treinamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.EmAtendimento:
                        return "Em Atendimento";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(AtendimentoTarefa other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get { return Titulo; }
        }
    }
}
