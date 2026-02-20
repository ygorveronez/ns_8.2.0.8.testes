using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALOR_PARAMETRO_PERNOITE_OCORRENCIA", EntityName = "ValorParametroPernoiteOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteOcorrencia", NameType = typeof(ValorParametroPernoiteOcorrencia))]
    public class ValorParametroPernoiteOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValorParametroOcorrencia", Column = "VPO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia ValorParametroOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Veiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VALOR_PARAMETRO_PERNOITE_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VPP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ValorParametroPernoiteVeiculo", Column = "PPV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroPernoiteVeiculo> Veiculos { get; set; }


        public virtual string Descricao
        {
            get
            {
                return "Pernoite - " + (this.ValorParametroOcorrencia?.Descricao ?? string.Empty);
            }
        }
    }
}
