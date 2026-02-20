using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VENCIMENTO_CERTIFICADO", EntityName = "VencimentoCertificado", Name = "Dominio.Entidades.VencimentoCertificado", NameType = typeof(VencimentoCertificado))]
    public class VencimentoCertificado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJ", Column = "VC_CNPJ", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "VC_NOME", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "VC_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "VC_EMAIL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "VC_TELEFONE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ambiente", Column = "VC_AMBIENTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Ambiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Homologacao", Column = "VC_HOMOLOGACAO", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Homologacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "VC_STATUS", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VC_STATUS_VENDA", TypeType = typeof(Enumeradores.StatusVendaCertificado), NotNull = false)]
        public virtual Enumeradores.StatusVendaCertificado? StatusVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelSatisfacao", Column = "VC_SATISFACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? NivelSatisfacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Log", Column = "VC_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case "P":
                        return "Pendente";
                    case "A":
                        return "Atualizado (Aguardando confirmação)";
                    case "C":
                        return "Atualização Confirmada";
                    case "I":
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }


    }
}
