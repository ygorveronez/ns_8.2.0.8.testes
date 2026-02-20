using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BLOCO_CARREGAMENTO", EntityName = "BlocoCarregamento", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento", NameType = typeof(BlocoCarregamento))]
    public class BlocoCarregamento : EntidadeBase, IEquatable<BlocoCarregamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BLC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carregamento Carregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BlocoCarregamento", Column = "BLC_CODIGO_SEGUNDO_TRECHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual BlocoCarregamento BlocoCarregamentoSegundoTrecho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bloco", Column = "BLC_BLOCO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Bloco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemCarregamento", Column = "BLC_ORDEM_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrdemEntrega", Column = "BLC_ORDEM_ENTREGA", TypeType = typeof(int), NotNull = false)]
        public virtual int OrdemEntrega { get; set; }

        public virtual string Descricao {
            get { return $"{this.Carregamento.Descricao} - {this.Bloco}"; }
        }

        public virtual bool Equals(BlocoCarregamento other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
