using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_MIGRATE", EntityName = "IntegracaoMigrate", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMigrate", NameType = typeof(IntegracaoMigrate))]
    public class IntegracaoMigrate : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIM_URL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

       
    }
}
