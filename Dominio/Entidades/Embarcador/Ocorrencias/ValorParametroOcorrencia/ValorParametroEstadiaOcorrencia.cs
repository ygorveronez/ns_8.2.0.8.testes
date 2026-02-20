using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALOR_PARAMETRO_ESTADIA_OCORRENCIA", EntityName = "ValorParametroEstadiaOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia", NameType = typeof(ValorParametroEstadiaOcorrencia))]
    public class ValorParametroEstadiaOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ValorParametroOcorrencia", Column = "VPO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroOcorrencia ValorParametroOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_VEICULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO_AJUDANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFreteAjudante { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Veiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VALOR_PARAMETRO_ESTADIA_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ValorParametroEstadiaVeiculo", Column = "PSV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaVeiculo> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Ajudantes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VALOR_PARAMETRO_ESTADIA_AJUDANTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VPS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ValorParametroEstadiaAjudante", Column = "PSA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaAjudante> Ajudantes { get; set; }


        public virtual string Descricao
        {
            get
            {
                return "Estadia - " + (this.ValorParametroOcorrencia?.Descricao ?? string.Empty);
            }
        }
    }
}
