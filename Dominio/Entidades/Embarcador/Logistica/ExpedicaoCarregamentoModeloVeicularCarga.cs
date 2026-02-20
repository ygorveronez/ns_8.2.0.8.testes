namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_EXPEDICAO_MODELO_VEICULAR_CARGA", EntityName = "ExpedicaoCarregamentoModeloVeicularCarga", Name = "Dominio.Entidades.Embarcador.Logistica.ExpedicaoCarregamentoModeloVeicularCarga", NameType = typeof(ExpedicaoCarregamentoModeloVeicularCarga))]
    public class ExpedicaoCarregamentoModeloVeicularCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ExpedicaoCarregamento", Column = "EXC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ExpedicaoCarregamento ExpedicaoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        public virtual string Descricao
        {
            get { return $"{this.ExpedicaoCarregamento.Descricao} | [Modelo Veicular] {this.ModeloVeicularCarga.Descricao}"; }
        }
    }
}
