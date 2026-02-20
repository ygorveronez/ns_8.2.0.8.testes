using NHibernate.Type;
using System;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_INTEGRACAO_CTASMART", EntityName = "IntegracaoCTASmart", Name = "Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart", NameType = typeof(IntegracaoCTASmart))]
    public class IntegracaoCTASmart : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "URL", Column = "CIC_URL_CTASMART", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string URL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "CIC_TOKEN_CTASMART", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "CIC_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmpresa", Column = "CIC_CODIGO_EMPRESA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoEmpresa { get; set; }
    }
}
