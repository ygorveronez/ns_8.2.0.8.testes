using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOG_ALTERACAO_TIPO_CARGA_MODELO_VEICULAR", EntityName = "LogAlteracaoTipoCargaModeloVeicular", Name = "Dominio.Entidades.Embarcador.Cargas.LogAlteracaoTipoCargaModeloVeicular", NameType = typeof(LogAlteracaoTipoCargaModeloVeicular))]
    public class LogAlteracaoTipoCargaModeloVeicular : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LTM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRegistroLog", Column = "LTM_DATA_REGISTRO_LOG", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataRegistroLog { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO_ATUAL", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCargaAtual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO_ATUAL", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCargaAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SistemaNaoGerouTipoCargaEModelo", Column = "LTM_SISTEMA_NAO_GEROU_CARGA_MODELO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SistemaNaoGerouTipoCargaEModelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Justificativa", Column = "LTM_JUSTIFICATIVA", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Justificativa { get; set; }
    }
}
