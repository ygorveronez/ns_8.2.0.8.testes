using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_TRAFEGUS", EntityName = "IntegracaoTrafegus", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrafegus", NameType = typeof(IntegracaoTrafegus))]
    public class IntegracaoTrafegus : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "CIT_USUARIO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "CIT_SENHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Url", Column = "CIT_URL", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Url { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "CIT_POSICAO", TypeType = typeof(Int64))]
        public virtual Int64 Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PGR", Column = "CIT_PGR", TypeType = typeof(int))]
        public virtual int PGR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJEmbarcador", Column = "CIT_CNPJ_EMBARCADOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CNPJEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiIntegracao", Column = "CIT_POSSUI_INTEGRACAO_TRAFEGUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiIntegracao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return "Configuração Integração Trafegus";
            }
        }

    }
}
