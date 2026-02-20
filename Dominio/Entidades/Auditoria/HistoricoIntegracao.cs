using NHibernate.Mapping.Attributes;
using System;

namespace Dominio.Entidades.Auditoria
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_INTEGRACAO", EntityName = "HistoricoIntegracao", Name = "Dominio.Entidades.Auditoria.HistoricoIntegracao", NameType = typeof(HistoricoIntegracao))]
    public class HistoricoIntegracao : EntidadeBase, IEquatable<HistoricoIntegracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "HIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "HIN_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "HIN_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeMetodo", Column = "HIN_NOME_METODO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string NomeMetodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Retorno", Column = "HIN_RETORNO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Retorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusRetorno", Column = "HIN_STATUS_RETORNO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao StatusRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Origem", Column = "HIN_ORIGEM", TypeType = typeof(ObjetosDeValor.Enumerador.TipoAuditado), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.OrigemAuditado Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", NotNull = false, Lazy = Laziness.Proxy)]
        public virtual WebService.Integradora Integradora { get; set; }

        public virtual bool Equals(HistoricoIntegracao other)
        {
            return (this.Codigo == other.Codigo);
        }

    }
}


