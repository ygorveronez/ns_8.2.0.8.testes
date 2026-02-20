using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACOES_PAGINACAO_INTERFACES", EntityName = "ConfiguracaoPaginacaoInterfaces", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaginacaoInterfaces", NameType = typeof(ConfiguracaoPaginacaoInterfaces))]
    public class ConfiguracaoPaginacaoInterfaces : EntidadeBase, IEquatable<ConfiguracaoPaginacaoInterfaces>
    {
        public ConfiguracaoPaginacaoInterfaces() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Interface", Column = "TCI_INTERFACES", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ConfiguracaoPaginacaoInterfaces), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ConfiguracaoPaginacaoInterfaces Interface { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dias", Column = "TCI_DIAS", TypeType = typeof(OperadoraCIOT), NotNull = false)]
        public virtual int Dias { get; set; }

        public virtual bool Equals(ConfiguracaoPaginacaoInterfaces other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
