using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_MODALIDADE_TRANSPORTADORAS_TIPO_PAGAMENTO_CIOT", EntityName = "ModalidadeTransportadoraPessoasTipoPagamentoCIOT", Name = "Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT", NameType = typeof(ModalidadeTransportadoraPessoasTipoPagamentoCIOT))]
    public class ModalidadeTransportadoraPessoasTipoPagamentoCIOT : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT>
    {
        public ModalidadeTransportadoraPessoasTipoPagamentoCIOT() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MPT_PAGAMENTO_CIOT", TypeType = typeof(TipoPagamentoCIOT), NotNull = false)]
        public virtual TipoPagamentoCIOT TipoPagamentoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Operadora", Column = "MPT_OPERADORA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT Operadora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalidadeTransportadoraPessoas", Column = "MOT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas ModalidadeTransportadoraPessoas { get; set; }

        public virtual bool Equals(ModalidadeTransportadoraPessoasTipoPagamentoCIOT other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
