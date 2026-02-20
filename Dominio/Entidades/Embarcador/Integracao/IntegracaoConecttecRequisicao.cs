using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_INTEGRACAO_CONECTTEC_REQUISICAO", EntityName = "IntegracaoConecttecRequisicao", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoConecttecRequisicao", NameType = typeof(IntegracaoConecttecRequisicao))]
    public class IntegracaoConecttecRequisicao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ICR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRequisicao", Column = "ICR_DATA_REQUISICAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReserveID", Column = "ICR_RESERVE_ID", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ReserveID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JSON", Column = "ICR_JSON", Type = "StringClob", NotNull = false)]
        public virtual string JSON { get; set; }

    }
}
