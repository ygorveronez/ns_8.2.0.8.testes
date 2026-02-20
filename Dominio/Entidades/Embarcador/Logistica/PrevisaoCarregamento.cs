using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_CARREGAMENTO_PREVISAO_CARREGAMENTO", EntityName = "PrevisaoCarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento", NameType = typeof(PrevisaoCarregamento))]
    public class PrevisaoCarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PRC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroCarregamento", Column = "CEC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ExcecaoCapacidadeCarregamento", Column = "CEX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento ExcecaoCapacidadeCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete Rota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dia", Column = "PRC_DIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana Dia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCargas", Column = "PRC_QUANTIDADE_CARGAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCargasExcedentes", Column = "PRC_QUANTIDADE_CARGAS_EXCEDENTES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCargasExcedentes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_CARREGAMENTO_PREVISAO_CARREGAMENTO_MODELO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> ModelosVeiculos { get; set; }
    }
}
