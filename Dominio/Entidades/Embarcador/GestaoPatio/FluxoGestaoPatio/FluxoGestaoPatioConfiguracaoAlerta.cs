using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_GESTAO_PATIO_CONFIGURACAO_ALERTA", EntityName = "FluxoGestaoPatioConfiguracaoAlerta", Name = "Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta", NameType = typeof(FluxoGestaoPatioConfiguracaoAlerta))]
    public class FluxoGestaoPatioConfiguracaoAlerta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "FCA_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Etapas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FLUXO_GESTAO_PATIO_CONFIGURACAO_ALERTA_ETAPA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FCA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FluxoGestaoPatioConfiguracaoAlertaEtapa", Column = "FCE_CODIGO")]
        public virtual IList<FluxoGestaoPatioConfiguracaoAlertaEtapa> Etapas { get; set; }

        public virtual string Descricao
        {
            get { return $"{Filial?.Descricao} - {Usuario?.Nome}"; }
        }
    }
}
