using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_MODALIDADE_FORNECEDORES_TABELA_VALORES", EntityName = "PostoCombustivelTabelaValores", Name = "Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores", NameType = typeof(PostoCombustivelTabelaValores))]
    public class PostoCombustivelTabelaValores : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>
    {
        public PostoCombustivelTabelaValores() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MOT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModalidadeFornecedorPessoas", Column = "MOF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas ModalidadeFornecedorPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeDeMedida", Column = "MOT_UNIDADE_MEDIDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida? UnidadeDeMedida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFixo", Column = "MOT_VALOR_FIXO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal ValorFixo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAte", Column = "MOT_VALOR_LIMITE_MAXIMO", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal? ValorAte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDesconto", Column = "MOT_PERCENTUAL_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "MOT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "MOT_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Integrado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "MOT_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "MOT_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "MOT_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaCotacao", Column = "MOT_VALOR_MOEDA_COTACAO", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorMoedaCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOriginalMoedaEstrangeira", Column = "MOT_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorOriginalMoedaEstrangeira { get; set; }

        public virtual string DescricaoUnidadeDeMedida
        {
            get
            {
                switch (this.UnidadeDeMedida)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Litros:
                        return "Litros";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MetroCubico:
                        return "Metro Cúbico";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MMBTU:
                        return "MMBTU";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Quilograma:
                        return "Quilograma";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tonelada:
                        return "Tonelada";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade:
                        return "Unidade";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.ModalidadeFornecedorPessoas?.ModalidadePessoas?.DescricaoTipoModalidade ?? string.Empty;
            }
        }

        public virtual bool Equals(PostoCombustivelTabelaValores other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}