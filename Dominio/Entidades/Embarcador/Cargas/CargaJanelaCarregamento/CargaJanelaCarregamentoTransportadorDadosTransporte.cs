using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_DADOS_TRANSPORTE", EntityName = "CargaJanelaCarregamentoTransportadorDadosTransporte", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte", NameType = typeof(CargaJanelaCarregamentoTransportadorDadosTransporte))]
    public class CargaJanelaCarregamentoTransportadorDadosTransporte : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JTD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoTransportador", Column = "JCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamentoTransportador CargaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_VEICULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosVinculados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_DADOS_TRANSPORTE_VEICULOS_VINCULADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "JTD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> VeiculosVinculados { get; set; }

        public virtual Veiculos.ModeloCarroceria ModeloCarroceria
        {
            get { return (VeiculosVinculados.Count > 0) ? VeiculosVinculados.FirstOrDefault().ModeloCarroceria : Veiculo.ModeloCarroceria; }
        }
    }
}
