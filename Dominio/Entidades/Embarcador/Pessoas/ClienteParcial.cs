using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_PARCIAL", EntityName = "ClienteParcial", Name = "Dominio.Entidades.Embarcador.Pessoas.ClienteParcial", NameType = typeof(ClienteParcial))]
    public class ClienteParcial : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.ClienteParcial>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJ", Column = "CLP_CNPJ", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CLP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoMunicipal", Column = "CLP_INSCRICAO_MUNICIPAL", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string InscricaoMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InscricaoEstadual", Column = "CLP_INSCRICAO_ESTADUAL", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string InscricaoEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "CLP_TELEFONE", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "CLP_EMAIL", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeFantasia", Column = "CLP_NOME_FANTASIA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeFantasia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "CLP_SITUACAO_INTEGRACAO", TypeType = typeof(SistemaEmissor), NotNull = true)]
        public virtual SituacaoIntegracao SituacaoIntegracao { get; set; }

        public virtual bool Equals(ClienteParcial other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
