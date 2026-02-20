using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_PARCELA", EntityName = "FaturaParcela", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaParcela", NameType = typeof(FaturaParcela))]
    public class FaturaParcela : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fatura.FaturaParcela>
    {
        public FaturaParcela() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "FAP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMoeda", Column = "FAP_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "FAP_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Acrescimo", Column = "FAP_ACRESCIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Acrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "FAP_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "FAP_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Sequencia", Column = "FAP_SEQUENCIA", TypeType = typeof(int), NotNull = false)]
        public virtual int Sequencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoFaturaParcela", Column = "FAP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela SituacaoFaturaParcela { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAP_TITULO_GERADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TituloGerado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaTitulo", Column = "FAP_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo? FormaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTitulo", Formula = @"CASE WHEN FAP_CODIGO_TITULO_INTEGRACAO IS NULL OR FAP_CODIGO_TITULO_INTEGRACAO = 0 THEN ISNULL((SELECT DISTINCT TOP 1 ISNULL(T.TIT_CODIGO, 0)
                                                                                        FROM T_TITULO T 
                                                                                        WHERE T.FAP_CODIGO = FAP_CODIGO), 0) ELSE FAP_CODIGO_TITULO_INTEGRACAO END", TypeType = typeof(int), Lazy = true)]
        public virtual int CodigoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NossoNumeroTitulo", Formula = @"ISNULL((SELECT DISTINCT TOP 1 ISNULL(T.TIT_NOSSO_NUMERO, '')
                                                                                        FROM T_TITULO T 
                                                                                        WHERE T.FAP_CODIGO = FAP_CODIGO), 0)", TypeType = typeof(string), Lazy = true)]
        public virtual string NossoNumeroTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoTitulo", Formula = @"ISNULL((SELECT DISTINCT TOP 1 T.TIT_DATA_VENCIMENTO
                                                                                        FROM T_TITULO T 
                                                                                        WHERE T.FAP_CODIGO = FAP_CODIGO), 0)", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime DataVencimentoTitulo { get; set; }

        public virtual DateTime VencimentoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTituloIntegracao", Column = "FAP_CODIGO_TITULO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? CodigoTituloIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAP_NOSSO_NUMERO_INTEGRADO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NossoNumeroIntegrado { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Titulos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TITULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Titulo", Column = "TIT_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Financeiro.Titulo> Titulos { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.SituacaoFaturaParcela)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.Atrazada:
                        return "Atrazada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto:
                        return "Em aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.Quitada:
                        return "Quitada";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.DataVencimento.ToString("dd/MM/yyyy");
            }
        }

        public virtual bool Equals(FaturaParcela other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
