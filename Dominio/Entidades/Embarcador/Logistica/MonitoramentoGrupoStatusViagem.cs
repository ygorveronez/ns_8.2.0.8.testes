using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_MONITORAMENTO_GRUPO_STATUS_VIAGEM", EntityName = "MonitoramentoGrupoStatusViagem", Name = "Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem", NameType = typeof(MonitoramentoGrupoStatusViagem))]
    public class MonitoramentoGrupoStatusViagem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MGV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MGV_DESCRICAO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cor", Column = "MGV_COR", TypeType = typeof(string), NotNull = true)]
        public virtual string Cor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "MGV_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "MGV_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "MGV_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        public virtual string DescricaoAtivo { get { return (this.Ativo) ? "Ativo" : "Inativo"; } }

    }
}
