using System;

namespace Dominio.Entidades.Embarcador.Simulacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_BONIFICACAO_VIGENCIA", EntityName = "GrupoBonificacaoVigencia", Name = "Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia", NameType = typeof(GrupoBonificacaoVigencia))]
    public class GrupoBonificacaoVigencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GBV__CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoBonificacao", Column = "GRB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Embarcador.Simulacoes.GrupoBonificacao GrupoBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "GBV_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "GBV_DATA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "GBV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca Situacao { get; set; }

        public virtual string Descricao
        {
            get { return $"De {DataInicial.ToString("dd/MM/yyyy")} até {DataFinal.ToString("dd/MM/yyyy")}"; }
        }
    }
}
