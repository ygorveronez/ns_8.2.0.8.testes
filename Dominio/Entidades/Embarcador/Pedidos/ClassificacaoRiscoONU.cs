using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLASSIFICACAI_RISCO_ONU", EntityName = "ClassificacaoRiscoONU", Name = "Dominio.Entidades.Embarcador.Pedidos.ClassificacaoRiscoONU", NameType = typeof(ClassificacaoRiscoONU))]
    public class ClassificacaoRiscoONU : EntidadeBase, IComparable<ClassificacaoRiscoONU>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroONU", Column = "CRO_NUMERO_ONU", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroONU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CRO_DESCRICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClasseRisco", Column = "CRO_CLASSE_RISCO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ClasseRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RiscoSubsidiario", Column = "CRO_RISCO_SUBSIDIARIO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string RiscoSubsidiario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRisco", Column = "CRO_NUMERO_RISCO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NumeroRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GrupoEmbarcado", Column = "CRO_GRUPO_EMBARCADO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string GrupoEmbarcado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProvisoesEspeciais", Column = "CRO_PROVISOES_ESPECIAIS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ProvisoesEspeciais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteKGVeiculo", Column = "CRO_LIMITE_KG_VEICULO", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal LimiteKGVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteLitroEmbalagemInterna", Column = "CRO_LIMITE_LITRO_EMBALAGEM_INTERNA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal LimiteLitroEmbalagemInterna { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmbalagemInstrucao", Column = "CRO_EMBALAGEM_INSTRUCAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EmbalagemInstrucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmbalagemProvisoesEspeciais", Column = "CRO_EMBALAGEM_PROVISOES_ESPECIAIS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string EmbalagemProvisoesEspeciais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TanqueInstrucao", Column = "CRO_TANQUE_INSTRUCAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TanqueInstrucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TanqueProvisoesEspeciais", Column = "CRO_TANQUE_PROVISOES_ESPECIAIS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TanqueProvisoesEspeciais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "CRO_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual int CompareTo(ClassificacaoRiscoONU other)
        {
            if (other == null)
                return -1;

            return other.Codigo.CompareTo(Codigo);
        }
    }
}
