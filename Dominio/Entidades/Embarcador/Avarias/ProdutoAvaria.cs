using System;

namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_AVARIA", EntityName = "ProdutoAvaria", Name = "Dominio.Entidades.Embarcador.Avarias.ProdutoAvaria", NameType = typeof(ProdutoAvaria))]
    public class ProdutoAvaria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixa", Column = "PAV_QUANTIDADE_CAIXA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaixasPallet", Column = "PAV_CAIXAS_PALLET", TypeType = typeof(int), NotNull = false)]
        public virtual int CaixasPallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoUnitario", Column = "PAV_PESO_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoCaixa", Column = "PAV_PESO_CAIXA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoCaixa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorEstorno", Column = "PAV_VALOR_ESTORNO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorEstorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProducao", Column = "PAV_VALOR_PRODUCAO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorProducao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrecoTransferencia", Column = "PAV_PRECO_TRANSFERENCIA", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PrecoTransferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoPrimario", Column = "PAV_CUSTO_PRIMARIO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal CustoPrimario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoSecundario", Column = "PAV_CUSTO_SECUNDARIO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal CustoSecundario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PAV_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "PAV_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCadastro { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.ProdutoEmbarcador?.Descricao ?? "";
            }
        }


        public virtual bool Equals(ProdutoAvaria other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
