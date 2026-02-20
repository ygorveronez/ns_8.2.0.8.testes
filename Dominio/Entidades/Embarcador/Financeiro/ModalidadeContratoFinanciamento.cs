using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODALIDADE_CONTRATO_FINANCIAMENTO", EntityName = "ModalidadeContratoFinanciamento", Name = "Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento", NameType = typeof(ModalidadeContratoFinanciamento))]
    public class ModalidadeContratoFinanciamento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.ModalidadeContratoFinanciamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MCF_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }
        
        public virtual string DescricaoAtivo
        {
            get
            {
                return Ativo ? "Ativo" : "Inativo";
            }
        }

        public virtual bool Equals(ModalidadeContratoFinanciamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }



    }
}
