using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOCAL_ARMAZENAMENTO_PRODUTO", EntityName = "LocalArmazenamentoProduto", Name = "Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto", NameType = typeof(LocalArmazenamentoProduto))]
    public class LocalArmazenamentoProduto : EntidadeBase, IEquatable<LocalArmazenamentoProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "LAP_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "LAP_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "LAP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOleo", Column = "TOL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frotas.TipoOleo TipoOleo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CapacidadeTotalLitros", Column = "LAP_CAPACIDADE_TOTAL_LITROS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal? CapacidadeTotalLitros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeSinalizacaoLitros", Column = "LAP_QUANTIDADE_SINALIZACAO_LITROS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal? QuantidadeSinalizacaoLitros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Regua", Column = "LAP_REGUA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? Regua { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Densidade", Column = "LAP_DENSIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? Densidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControleAbastecimentoDisponivel", Column = "LAP_CONTROLE_ABASTECIMENTO_DISPONIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? ControleAbastecimentoDisponivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoDoTanque", Column = "LAP_SALDO_TANQUE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal? SaldoDoTanque { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Posto { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Transferencias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LOCAL_ARMAZENAMENTO_PRODUTO_TRANSFERENCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "LAP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "LocalArmazenamentoProdutoTransferencia", Column = "LPT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia> Transferencias { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case true: return "Ativo";
                    case false: return "Inativo";
                    default: return "";
                }
            }
        }

        public virtual bool Equals(LocalArmazenamentoProduto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
