using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ROTEIRIZACAO_CLIENTES_ROTA", EntityName = "CargaRoteirizacaoClientesRota", Name = "Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota", NameType = typeof(CargaRoteirizacaoClientesRota))]
    public class CargaRoteirizacaoClientesRota : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaRoteirizacao", Column = "CRT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacao CargaRoteirizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "CTC_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        public virtual bool Equals(CargaRoteirizacaoClientesRota other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
