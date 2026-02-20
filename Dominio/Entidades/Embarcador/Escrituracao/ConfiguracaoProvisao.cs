using System;

namespace Dominio.Entidades.Embarcador.Escrituracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROVISAO_CONFIGURACAO", EntityName = "ConfiguracaoProvisao", Name = "Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao", NameType = typeof(ConfiguracaoProvisao))]
    public class ConfiguracaoProvisao : EntidadeBase, IEquatable<ConfiguracaoProvisao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasForaMes", Column = "PCN_DIAS_FORA_MES", TypeType = typeof(int), NotNull = true)]
        public virtual int DiasForaMes { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração de Provisão"; }
        }

        public virtual bool Equals(ConfiguracaoProvisao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
