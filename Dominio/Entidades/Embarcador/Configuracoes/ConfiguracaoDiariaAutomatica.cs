namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_DIARIA_AUTOMATICA", EntityName = "ConfiguracaoDiariaAutomatica", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica", NameType = typeof(ConfiguracaoDiariaAutomatica))]
    public class ConfiguracaoDiariaAutomatica : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDA_HABILITAR_DIARIA_AUTOMATICA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool HabilitarDiariaAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CDA_FREQUENCIA_ATUALIZACAO", TypeType = typeof(int), NotNull = true)]
        public virtual int FrequenciaAtualizacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }

    }
}
