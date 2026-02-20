namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ERRO_SEFAZ", EntityName = "ErroSefaz", Name = "Dominio.Entidades.ErroSefaz", NameType = typeof(ErroSefaz))]
    public class ErroSefaz : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ERR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDoErro", Column = "ERR_CODIGO_ERRO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoDoErro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemDoErro", Column = "ERR_MENSAGEM_ERRO", TypeType = typeof(string), Length = 255, NotNull = true)]
        public virtual string MensagemDoErro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ERR_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "ERR_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoErroSefaz), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoErroSefaz Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PermiteLiberarSemInutilizacao", Column = "EER_PERMITE_LIBERAR_SEM_INUTILIZACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool PermiteLiberarSemInutilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoPermiteReutilizarNumeracao", Column = "EER_NAO_PERMITE_REUTILIZAR_NUMERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool NaoPermiteReutilizarNumeracao { get; set; }
    }
}
