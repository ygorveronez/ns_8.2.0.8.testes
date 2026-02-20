using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CENTRO_DESCARREGAMENTO_PREVISAO_DESCARREGAMENTO", EntityName = "PrevisaoDescarregamento", Name = "Dominio.Entidades.Embarcador.Logistica.PrevisaoDescarregamento", NameType = typeof(PrevisaoDescarregamento))]
    public class PrevisaoDescarregamento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PRD_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroDescarregamento", Column = "CED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento CentroDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ExcecaoCapacidadeDescarregamento", Column = "CEX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento ExcecaoCapacidadeDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete Rota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dia", Column = "PRD_DIA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana Dia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaDoMes", Column = "PRD_DIA_DO_MES", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaDoMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mes", Column = "PRD_MES", TypeType = typeof(int), NotNull = false)]
        public virtual int Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCargas", Column = "PRD_QUANTIDADE_CARGAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeCargas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCargasExcedentes", Column = "PRD_QUANTIDADE_CARGAS_EXCEDENTES", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeCargasExcedentes { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeiculos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CENTRO_DESCARREGAMENTO_PREVISAO_DESCARREGAMENTO_MODELO_VEICULO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PRD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<Cargas.ModeloVeicularCarga> ModelosVeiculos { get; set; }
    }
}
