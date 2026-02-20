using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_WEB_SERVICE_INTEGRACAO", EntityName = "ConfiguracaoWebServiceIntegracao", Name = "Dominio.Entidades.Embarcador.Integracao.ConfiguracaoWebServiceIntegracao", NameType = typeof(ConfiguracaoWebServiceIntegracao))]
    public class ConfiguracaoWebServiceIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CWS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        //ATM, Avon, Oracle CT-e, Oracle MDF-e... 
        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao), NotNull = true, Unique = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_ENDPOINT", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string Endpoint { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_TIPO_BINDING", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracaoBinding), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracaoBinding? TipoBinding { get; set; }

        //JSON com as configuracoes do binding
        [NHibernate.Mapping.Attributes.Property(0, Column = "CWS_CONFIGURACAO_BINDING", Type = "StringClob", NotNull = false)]
        public virtual string ConfiguracaoBinding { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Tipo.ObterDescricao();
            }
        }
    }
}
