using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_OBSERVACAO_NFE_FORMULA", EntityName = "GrupoPessoasObservacaoNfeFormula", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasObservacaoNfeFormula", NameType = typeof(GrupoPessoasObservacaoNfeFormula))]
    public class GrupoPessoasObservacaoNfeFormula : EntidadeBase, IEquatable<GrupoPessoasObservacaoNfeFormula>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GOF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GOF_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificadorInicio", Column = "GOF_IDENTIFICADOR_INICIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IdentificadorInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificadorFim", Column = "GOF_IDENTIFICADOR_FIM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IdentificadorFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tag", Column = "GOF_TAG", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Tag { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOF_NUMERO_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOF_NUMERO_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NumeroContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOF_TARA_CONTAINER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TaraContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOF_LACRE_CONTAINER_UM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LacreContainerUm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOF_LACRE_CONTAINER_DOIS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LacreContainerDois { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOF_LACRE_CONTAINER_TRES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LacreContainerTres { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOF_NUMERO_CONTROLE_CLIENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NumeroControleCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOF_NUMERO_CONTROLE_PEDIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NumeroControlePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GOF_NUMERO_REFERENCIA_EDI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NumeroReferenciaEDI { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdMinimoDigitos", Column = "GOF_QTD_MINIMO_DIGITOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? QtdMinimoDigitos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdMaximoDigitos", Column = "GOF_QTD_MAXIMO_DIGITOS", TypeType = typeof(int), NotNull = false)]
        public virtual int? QtdMaximoDigitos { get; set; }

        public virtual bool Equals(GrupoPessoasObservacaoNfeFormula other)
        {
            return other.Codigo == this.Codigo;
        }
    }
}
