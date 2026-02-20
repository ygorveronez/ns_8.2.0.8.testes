using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_HISTORICO_VEICULO", EntityName = "HistoricoVeiculo", Name = "Dominio.Entidades.HistoricoVeiculo", NameType = typeof(HistoricoVeiculo))]
    public class HistoricoVeiculo : EntidadeBase, IEquatable<Dominio.Entidades.HistoricoVeiculo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "HIS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ServicoVeiculo", Column = "SER_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ServicoVeiculo Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "HIS_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGarantia", Column = "HIS_DATA_GARANTIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataGarantia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMVeiculo", Column = "HIS_KM_VEICULO", TypeType = typeof(int), NotNull = false)]
        public virtual int KMVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "HIS_QTD", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "HIS_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "HIS_OBS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFornecedor", Column = "HIS_FORNECEDOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "HIS_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
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

        public virtual bool Equals(HistoricoVeiculo other)
        {
            if (this.Codigo == other.Codigo)
                return true;
            else
                return false;
        }
    }
}
