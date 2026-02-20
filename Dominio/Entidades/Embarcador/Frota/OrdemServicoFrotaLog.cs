using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_ORDEM_SERVICO_LOG", EntityName = "OrdemServicoFrotaLog", Name = "Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaLog", NameType = typeof(OrdemServicoFrotaLog))]
    public class OrdemServicoFrotaLog: EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OSL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrota OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "OSL_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "OSL_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }
    }
}
