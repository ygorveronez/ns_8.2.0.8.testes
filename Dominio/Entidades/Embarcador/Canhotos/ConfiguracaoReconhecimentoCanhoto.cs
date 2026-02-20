using Dominio.Interfaces.Embarcador.Entidade;

namespace Dominio.Entidades.Embarcador.Canhotos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_RECONHECIMENTO_CANHOTO", EntityName = "ConfiguracaoReconhecimentoCanhoto", Name = "Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto", NameType = typeof(ConfiguracaoReconhecimentoCanhoto))]
    public class ConfiguracaoReconhecimentoCanhoto : EntidadeBase, IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_PALAVRAS_CHAVES", Type = "StringClob", NotNull = true)]
        public virtual string PalavrasChaves { get; set; }

        public virtual bool Equals(ConfiguracaoReconhecimentoCanhoto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

    }
}