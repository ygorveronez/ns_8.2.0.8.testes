using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TACOGRAFO", EntityName = "LeituraDeTacografo", Name = "Dominio.Entidades.LeituraDeTacografo", NameType = typeof(LeituraDeTacografo))]
    public class LeituraDeTacografo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDeCadastro", Column = "TAC_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataDeCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "TAC_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "TAC_DATA_FIM", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TAC_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Excesso", Column = "TAC_EXCESSO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Excesso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "TAC_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "A":
                        return "Ativo";
                    case "I":
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }
    }
}
