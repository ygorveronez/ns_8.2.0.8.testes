using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONTROLE_THREAD", EntityName = "ControleThread", Name = "Dominio.Entidades.ControleThread", NameType = typeof(ControleThread))]
    public class ControleThread : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Thread", Column = "CTH_THREAD", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Thread { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "CTH_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "CTH_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFim", Column = "CTH_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tempo", Column = "CTH_TEMPO", TypeType = typeof(int), NotNull = true)]
        public virtual int Tempo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CTH_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao { get { return Thread; } }

        public virtual string DescricaoAtivo
        {
            get { return Ativo ? "Ativo" : "Inativo"; }
        }

    }
}
