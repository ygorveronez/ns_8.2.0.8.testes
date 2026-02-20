using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.Retornos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RETORNO_CARGA", EntityName = "RetornoCarga", Name = "Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga", NameType = typeof(RetornoCarga))]
    public class RetornoCarga : EntidadeBase, IEquatable<RetornoCarga>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_RETORNO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga CargaRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga CargaColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_COLETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_ORIGEM_RETORNO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente OrigemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoRetornoCarga", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoRetornoCarga TipoRetornoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoRetornoCarga", Column = "CAR_SITUACAO_RETORNO_CARGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoCarga), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoCarga SituacaoRetornoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Reboques", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RETORNO_CARGA_REBOQUES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Reboques { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornarSomenteComTracao", Column = "CAR_RETORNAR_SOMENTE_COM_TRACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RetornarSomenteComTracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoEntrega", Column = "RCA_DATA_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntrega { get; set; }

        public virtual string Descricao
        {
            get { return Carga.CodigoCargaEmbarcador; }
        }

        public virtual bool Equals(RetornoCarga other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
