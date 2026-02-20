using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escalas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GERACAO_ESCALA", EntityName = "GeracaoEscala", Name = "Dominio.Entidades.Embarcador.Escalas.GeracaoEscala", NameType = typeof(GeracaoEscala))]
    public class GeracaoEscala : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Escalas.GeracaoEscala>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GES_NUMERO_ESCALA", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroEscala { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GES_DATA_ESCALA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEscala { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Criador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GES_DATA_GERADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataGerada { get; set; }

        /// <summary>
        /// Utiliza como informação para gerar a escala de todos os Centros que possuem expedição do produto.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CAR_GERAR_PARA_TODOS_CENTROS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarParaTodosOsCentros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoEscala", Column = "GES_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFilial), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscala SituacaoEscala { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoEscalaCancelamento", Column = "GES_SITUACAO_CANCELAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFilial), NotNull = false)]
        //public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscala SituacaoEscalaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GES_OBSERVACAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Observacao { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Column = "GES_JUSTIFICATIVA_CANCELAMENTO", TypeType = typeof(string), Length = 400, NotNull = false)]
        //public virtual string JustificativaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CentrosCarregamento", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GERACAO_ESCALA_CENTRO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GES_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CentroCarregamento", Column = "CEC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> CentrosCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Produtos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GERACAO_ESCALA_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GES_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> Produtos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "GeracaoEscalaRestricoesModeloVeicular", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GERACAO_ESCALA_RESTRICAO_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GES_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "GeracaoEscalaRestricaoModeloVeicular", Column = "ERM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Escalas.GeracaoEscalaRestricaoModeloVeicular> GeracaoEscalaRestricoesModeloVeicular { get; set; }

        public virtual string Descricao
        {
            get { return this.NumeroEscala.ToString(); }
        }

        public virtual bool Equals(GeracaoEscala other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
