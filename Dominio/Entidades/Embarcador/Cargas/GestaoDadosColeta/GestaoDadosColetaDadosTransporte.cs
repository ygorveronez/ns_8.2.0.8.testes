using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_GESTAO_DADOS_COLETA_DADOS_TRANSPORTE", EntityName = "GestaoDadosColetaDadosTransporte", Name = "Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte", NameType = typeof(GestaoDadosColetaDadosTransporte))]
    public class GestaoDadosColetaDadosTransporte : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GDT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDadosColeta", Column = "GDC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GestaoDadosColeta GestaoDadosColeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosVinculados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GESTAO_DADOS_COLETA_DADOS_TRANSPORTE_VEICULOS_VINCULADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GDT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> VeiculosVinculados { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Motoristas", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_GESTAO_DADOS_COLETA_DADOS_TRANSPORTE_MOTORISTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "GDT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Motoristas { get; set; }

        #endregion Propriedades
    }
}
