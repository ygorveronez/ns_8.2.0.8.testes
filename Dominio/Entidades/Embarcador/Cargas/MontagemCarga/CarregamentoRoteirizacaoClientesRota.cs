using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO_ROTEIRIZACAO_CLIENTES_ROTA", EntityName = "CarregamentoRoteirizacaoClientesRota", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota", NameType = typeof(CarregamentoRoteirizacaoClientesRota))]
    public class CarregamentoRoteirizacaoClientesRota : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CarregamentoRoteirizacao", Column = "CRT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao CarregamentoRoteirizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "CTC_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Coleta", Column = "CTC_COLETA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Coleta { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "PrimeiraEntrega", Column = "CTC_PRIMEIRA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool PrimeiraEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteOutroEndereco", Column = "COE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Embarcador.Pessoas.ClienteOutroEndereco OutroEndereco { get; set; }

        public virtual bool Equals(CarregamentoRoteirizacaoClientesRota other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
