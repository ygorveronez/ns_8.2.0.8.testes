namespace Dominio.Entidades.Embarcador.Importacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_IMPORTACAO", EntityName = "ConfiguracaoImportacao", Name = "Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao", NameType = typeof(ConfiguracaoImportacao))]
    public class ConfiguracaoImportacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_CODIGO_CONTROLE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoControleImportacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoControleImportacao CodigoControle { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CIM_ORDEM", TypeType = typeof(string), Length = 400, NotNull = true)]
        public virtual string Ordem { get; set; }

        public virtual bool Equals(ConfiguracaoImportacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
