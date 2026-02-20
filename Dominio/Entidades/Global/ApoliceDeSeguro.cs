using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APOLICE_SEGURO", EntityName = "ApoliceDeSeguro", Name = "Dominio.Entidades.ApoliceDeSeguro", NameType = typeof(ApoliceDeSeguro))]
    public class ApoliceDeSeguro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeSeguradora", Column = "APS_SEGURADORA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NomeSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroApolice", Column = "APS_APOLICE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ramo", Column = "APS_RAMO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Ramo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioVigencia", Column = "APS_DATAINI", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimVigencia", Column = "APS_DATAFIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "APS_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJSeguradora", Column = "APS_CNPJSEGURADORA", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJSeguradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Responsavel", Column = "APS_RESPONSAVEL", TypeType = typeof(int), Length = 1, NotNull = false)]
        public virtual int Responsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJResposavelNaObservacaoContribuinte", Column = "APS_CNPJ_RESPONSAVEL_OBSERVACAO_CONTRIBUINTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CNPJResposavelNaObservacaoContribuinte { get; set; }

        public virtual string Descricao
        {
            get
            {
                return NumeroApolice;
            }
        }

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
