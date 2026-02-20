using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_SASCAR", EntityName = "IntegracaoSascar", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSascar", NameType = typeof(IntegracaoSascar))]
    public class IntegracaoSascar : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIS_USUARIO", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIS_SENHA", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string Senha { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Pacote", Column = "CIS_PACOTE", TypeType = typeof(Int64), NotNull = true)]
        public virtual Int64 Pacote { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração Sascar";
            }
        }

    }
}
