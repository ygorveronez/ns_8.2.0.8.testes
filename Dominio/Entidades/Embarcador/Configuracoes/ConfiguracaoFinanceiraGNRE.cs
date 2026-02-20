using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_FINANCEIRA_GNRE", EntityName = "ConfiguracaoFinanceiraGNRE", Name = "Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE", NameType = typeof(ConfiguracaoFinanceiraGNRE))]
    public class ConfiguracaoFinanceiraGNRE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFF_GERAR_GNRE_PARA_CTES_EMITIDOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarGNREParaCTesEmitidos { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CFF_ALERTAR_DISPONIBILIDADE_GNRE_PARA_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AlertarDisponibilidadeGNREParaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CFF_GERAR_GNRE_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarGNREAutomaticamente { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConfiguracoesRegistros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_FINANCEIRA_GNRE_REGISTRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CFG_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoFinanceiraGNRERegistro", Column = "CGR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro> ConfiguracoesRegistros { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
