using System;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRODUTO_EMBARCADOR_LOTE", EntityName = "ProdutoEmbarcadorLote", Name = "Dominio.Entidades.Embarcador.Embarcador.ProdutoEmbarcadorLote", NameType = typeof(ProdutoEmbarcadorLote))]
    public class ProdutoEmbarcadorLote : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PEL_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "PEL_NCM", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PEL_NUMERO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarras", Column = "PEL_CODIGO_BARRAS", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoBarras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "PEL_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeLote", Column = "PEL_QUANTIDADE_LOTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePalet", Column = "PEL_QUANTIDADE_PALET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePalet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubico", Column = "PEL_METRO_CUBICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetroCubico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "PEL_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeAtual", Column = "PEL_QUANTIDADE_ATUAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeAtual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DepositoPosicao", Column = "DPO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual WMS.DepositoPosicao DepositoPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRecebimentoMercadoria", Column = "PEL_TIPO_RECEBIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria? TipoRecebimentoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CPF_CNPJ_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CPF_CNPJ_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }        

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_DISTRIBUIDORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaDistribuidora { get; set; }

        public virtual decimal PesoAtual
        {
            get
            {
                return this.Peso * this.QuantidadeAtual;
            }
        }

        public virtual decimal MetroCubicoAtual
        {
            get
            {
                return this.MetroCubico * this.QuantidadeAtual;
            }
        }

        public virtual decimal QuantidadePaletAtual
        {
            get
            {
                return this.QuantidadePalet * this.QuantidadeAtual;
            }
        }

        public virtual bool Equals(ProdutoEmbarcadorLote other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
