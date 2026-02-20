using System;

namespace Dominio.Entidades.Embarcador.Agenda
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AGENDA_TAREFA", EntityName = "AgendaTarefa", Name = "Dominio.Entidades.Embarcador.Agenda.AgendaTarefa", NameType = typeof(AgendaTarefa))]
    public class AgendaTarefa : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Agenda.AgendaTarefa>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "ATA_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "ATA_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "ATA_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ATA_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusAgendaTarefa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusAgendaTarefa Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ATA_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Prioridade", Column = "ATA_PRIORIDADE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento Prioridade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAssinatura", Column = "ATA_TIPO_ASSINATURA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAssinaturaVendaDireta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAssinaturaVendaDireta TipoAssinatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Servico", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.Servico Servico { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Observacao;
            }
        }

        public virtual bool Equals(AgendaTarefa other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
