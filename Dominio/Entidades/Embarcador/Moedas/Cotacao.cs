using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Moedas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOEDA_COTACAO", EntityName = "Cotacao", Name = "Dominio.Entidades.Embarcador.Moedas.Cotacao", NameType = typeof(Cotacao))]
    public class Cotacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Moedas.Cotacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCotacaoBancoCentral", Column = "MCO_MOEDA_COTACAO_BANCO_CENTRAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = true)]
        public virtual MoedaCotacaoBancoCentral MoedaCotacaoBancoCentral { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoeda", Column = "MCO_VALOR_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = true)]
        public virtual decimal ValorMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCotacao", Column = "MCO_DATA_COTACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCotacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CotacaoAutomaticaViaWS", Column = "MCO_COTACAO_AUTOMATICA_VIA_WS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CotacaoAutomaticaViaWS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarCotacaoRetroativa", Column = "MCO_UTILIZAR_COTACAO_RETROATIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCotacaoRetroativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CotacaoAtiva", Column = "MCO_COTACAO_ATIVA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool CotacaoAtiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigenciaInicial", Column = "MCO_DATA_VIGENCIA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVigenciaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigenciaFinal", Column = "MCO_DATA_VIGENCIA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVigenciaFinal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "MCO_TIPO_PESSOA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }

        public virtual string Descricao
        {
            get { return MoedaCotacaoBancoCentral.ObterDescricao(); }
        }

        public virtual bool Equals(Cotacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
