using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DADOS_TRANSPORTE_FLUXO_PATIO", EntityName = "DadosTransporteFluxoPatio", Name = "Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio", NameType = typeof(DadosTransporteFluxoPatio))]
    public class DadosTransporteFluxoPatio : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoVeiculo", Column = "SOV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SolicitacaoVeiculo SolicitacaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FotoMotorista", Column = "DFP_FOTO_MOTORISTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string FotoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Ajudantes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DADOS_TRANSPORTE_FLUXO_PATIO_MOTORISTA_AJUDANTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DFP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Usuario> Ajudantes { get; set; }

        public virtual string Descricao
        {
            get { return SolicitacaoVeiculo.Carga != null ? $"Dados de Transporte da carga {SolicitacaoVeiculo.Carga.CodigoCargaEmbarcador}" : $"Dados de Transporte da pré carga {SolicitacaoVeiculo.PreCarga.NumeroPreCarga}"; }
        }
    }
}
