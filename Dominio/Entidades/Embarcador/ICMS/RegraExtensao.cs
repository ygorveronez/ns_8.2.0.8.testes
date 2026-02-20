namespace Dominio.Entidades.Embarcador.ICMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_EXTENSAO", EntityName = "RegraExtensao", Name = "Dominio.Entidades.Embarcador.ICMS.RegraExtensao", NameType = typeof(RegraExtensao))]
    public class RegraExtensao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "REX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "REX_EXTENSAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Extensao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPropriedade", Column = "REX_TIPO_PROPRIEDADE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo TipoPropriedade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }


        public virtual string Descricao
        {
            get { return this.Extensao; }
        }

    }
}
