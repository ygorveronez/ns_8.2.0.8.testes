using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DE_CARGA_MODELO_VEICULAR_AUTO_CONFIG", EntityName = "TipoCargaModeloVeicularAutoConfig", Name = "Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig", NameType = typeof(TipoCargaModeloVeicularAutoConfig))]
    public class TipoCargaModeloVeicularAutoConfig : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [Obsolete("Utilizar o campo TipoAutomatizacaoTipoCarga. Este campo será deletado.")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "AutoTipoCargaHabilitado", Column = "TMC_AUTO_TIPO_CARGA_HABILITADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AutoTipoCargaHabilitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AutoModeloVeicularHabilitado", Column = "TMC_AUTO_MODELO_VEICULAR_HABILITADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool AutoModeloVeicularHabilitado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlarModeloPorNumeroPaletes", Column = "TMC_CONTROLAR_MVC_POR_PALETES", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ControlarModeloPorNumeroPaletes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlarModeloPorPeso", Column = "TMC_CONTROLAR_MVC_POR_PESO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ControlarModeloPorPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ConsiderarToleranciaMenorPesoModelo", Column = "TMC_CONSIDERAR_TOLERANCIA_MENOR_PESO_MODELO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ConsiderarToleranciaMenorPesoModelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAutomatizacaoTipoCarga", Column = "TMC_TIPO_AUTOMATIZACAO_TIPO_CARGA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutomatizacaoTipoCarga), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutomatizacaoTipoCarga TipoAutomatizacaoTipoCarga { get; set; }

        public virtual bool Equals(TipoCargaModeloVeicularAutoConfig other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Tipo de Carga Modelo Veicular";
            }
        }
    }
}
