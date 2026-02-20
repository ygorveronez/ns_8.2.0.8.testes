using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALOR_PARAMETRO_HORA_EXTRA_OCORRENCIA", EntityName = "ValorParametroHoraExtraOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraOcorrencia", NameType = typeof(ValorParametroHoraExtraOcorrencia))]
    public class ValorParametroHoraExtraOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPH_CODIGO")]
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

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Veiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VALOR_PARAMETRO_HORA_EXTRA_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VPH_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ValorParametroHoraExtraVeiculo", Column = "PHV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraVeiculo> Veiculos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Ajudantes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_VALOR_PARAMETRO_HORA_EXTRA_AJUDANTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "VPH_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ValorParametroHoraExtraAjudante", Column = "PHA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroHoraExtraAjudante> Ajudantes { get; set; }


        public virtual string Descricao
        {
            get
            {
                return "Hora Extra - " + (this.ValorParametroOcorrencia?.Descricao ?? string.Empty);
            }
        }
    }
}
