using System;

namespace Dominio.Entidades.Embarcador.Atendimento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ATENDIMENTO_TAREFA_LOG", EntityName = "AtendimentoTarefaLog", Name = "Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaLog", NameType = typeof(AtendimentoTarefaLog))]
    public class AtendimentoTarefaLog : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaLog>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "ATG_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ATG_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ATG_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AtendimentoTarefa", Column = "ATC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa AtendimentoTarefa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_FILHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaFilho { get; set; }

        public virtual bool Equals(AtendimentoTarefaLog other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
