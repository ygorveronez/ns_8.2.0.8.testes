using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_RPS", EntityName = "RPSNFSe", Name = "Dominio.Entidades.RPSNFSe", NameType = typeof(RPSNFSe))]
    public class RPSNFSe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "RPS_SERIE", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "RPS_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "RPS_PROTOCOLO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "RPS_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Hora", Column = "RPS_HORA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Hora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetorno", Column = "RPS_RETORNO_CODIGO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "RPS_RETORNO_MENSAGEM", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "RPS_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloCancelamento", Column = "RPS_PROTOCOLO_CANCELAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ProtocoloCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProtocolo", Column = "RPS_DATA_RETORNO_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "RPS_STATUS", TypeType = typeof(string), Length = 1, NotNull = true)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTentativaConsulta", Column = "RPS_QUANTIDADE_TENTATIVA_CONSULTA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeTentativaConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaConsulta", Column = "RPS_DATA_ULTIMA_CONSULTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaConsulta { get; set; }
    }
}
