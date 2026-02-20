using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Entidades.Embarcador.Filiais;
using Dominio.Entidades.Embarcador.Pedidos;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_TOLERANCIA_PESAGEM", EntityName = "ConfiguracaoToleranciaPesagem", Name = "Dominio.Entidades.Embarcador.GestaoPatio.ConfiguracaoToleranciaPesagem", NameType = typeof(ConfiguracaoToleranciaPesagem))]
    public class ConfiguracaoToleranciaPesagem : EntidadeBase
    {
        public ConfiguracaoToleranciaPesagem() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CTP_DESCRICAO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_TIPO_REGRA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoToleranciaPesagem), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoToleranciaPesagem TipoRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaPesoSuperior", Column = "CTP_TOLERANCIA_PESO_SUPERIOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ToleranciaPesoSuperior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaPesoInferior", Column = "CTP_TOLERANCIA_PESO_INFERIOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ToleranciaPesoInferior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaPesoSuperior", Column = "CTP_PERCENTUAL_TOLERANCIA_PESO_SUPERIOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualToleranciaPesoSuperior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualToleranciaPesoInferior", Column = "CTP_PERCENTUAL_TOLERANCIA_PESO_INFERIOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal PercentualToleranciaPesoInferior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CTP_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidaTipoCarga", Column = "CTP_VALIDA_TIPO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidaTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidaTipoOperacao", Column = "CTP_VALIDA_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidaTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidaModeloVeicularCarga", Column = "CTP_VALIDA_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidaModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValidaFilial", Column = "CTP_VALIDA_FILIAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValidaFilial { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_TOLERANCIA_PESAGEM_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<TipoDeCarga> TiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_TOLERANCIA_PESAGEM_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<TipoOperacao> TiposOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ModelosVeicularesCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_TOLERANCIA_PESAGEM_MODELO_VEICULAR_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO")]
        public virtual ICollection<ModeloVeicularCarga> ModelosVeicularesCarga { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Filials", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_TOLERANCIA_PESAGEM_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Filial> Filials { get; set; }
    }
}