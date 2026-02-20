using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PEDIDO_TABELA_FRETE_CLIENTE", EntityName = "CargaPedidoTabelaFreteCliente", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente", NameType = typeof(CargaPedidoTabelaFreteCliente))]
    public class CargaPedidoTabelaFreteCliente : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CPC_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFixo", Column = "CPC_VALOR_FIXO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualSobreNF", Column = "CPC_PERC_NOTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualSobreNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CPC_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPC_OBSERVACAO_TERCEIRO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ObservacaoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TabelaFreteFilialEmissora", Column = "CPC_TABELA_FRETE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TabelaFreteFilialEmissora { get; set; }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente)this.MemberwiseClone();
        }
        
        public virtual bool Equals(CargaPedidoTabelaFreteCliente other)
        {
            if (other.Codigo == this.Codigo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
