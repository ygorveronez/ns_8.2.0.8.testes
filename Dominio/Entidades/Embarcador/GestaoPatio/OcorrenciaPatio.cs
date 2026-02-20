using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_PATIO", EntityName = "OcorrenciaPatio", Name = "Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio", NameType = typeof(OcorrenciaPatio))]
    public class OcorrenciaPatio : EntidadeBase, IEntidade, IEquatable<OcorrenciaPatio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ORP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "ORP_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ORP_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ORP_SITUACAO", TypeType = typeof(SituacaoOcorrenciaPatio), NotNull = true)]
        public virtual SituacaoOcorrenciaPatio Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoLancamento", Column = "ORP_TIPO_LANCAMENTO", TypeType = typeof(TipoLancamento), NotNull = true)]
        public virtual TipoLancamento TipoLancamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaPatioTipo", Column = "OPT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OcorrenciaPatioTipo OcorrenciaPatioTipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO_TRACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Tracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Reboques", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_PATIO_REBOQUE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ORP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> Reboques { get; set; }

        public virtual bool Equals(OcorrenciaPatio other)
        {
            return (this.Codigo == other.Codigo);
        }
    }
}
