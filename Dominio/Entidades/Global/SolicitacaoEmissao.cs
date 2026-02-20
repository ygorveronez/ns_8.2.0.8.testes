using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_EMISSAO", EntityName = "SolicitacaoEmissao", Name = "Dominio.Entidades.SolicitacaoEmissao", NameType = typeof(SolicitacaoEmissao))]
    public class SolicitacaoEmissao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SOE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Assunto", Column = "SOE_ASSUNTO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Assunto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Texto", Column = "SOE_TEXTO", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string Texto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "SOE_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetorno", Column = "SOE_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "SOE_USUARIO_ENVIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioEnvio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "SOE_USUARIO_RETORNO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "SOE_STATUS", TypeType = typeof(Enumeradores.TipoDuplicata), NotNull = true)]
        public virtual Enumeradores.StatusSolicitacaoEmissao Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

    }
}