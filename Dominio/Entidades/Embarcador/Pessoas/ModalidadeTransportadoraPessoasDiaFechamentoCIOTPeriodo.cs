using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_MODALIDADE_TRANSPORTADORAS_DIA_FECHAMENTO_CIOT", EntityName = "ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo", Name = "Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo", NameType = typeof(ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo))]
    public class ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo>
    {
        public ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MTD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MTD_DIA_FECHAMENTO_CIOT", TypeType = typeof(int), NotNull = true)]
        public virtual int DiaFechamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalidadeTransportadoraPessoas", Column = "MOT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas ModalidadeTransportadoraPessoas { get; set; }

        public virtual bool Equals(ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
