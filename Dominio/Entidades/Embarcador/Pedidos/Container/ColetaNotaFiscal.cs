using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_NOTA_FISCAL", EntityName = "ColetaNotaFiscal", Name = "Dominio.Entidades.Embarcador.Pedidos.ColetaNotaFiscal", NameType = typeof(ColetaNotaFiscal))]
    public class ColetaNotaFiscal : EntidadeBase, IEquatable<ColetaNotaFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CNF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOS", Column = "CNF_NUMERO_OS", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContainer", Column = "CNF_NUMERO_CONTAINER", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TaraContainer", Column = "CNF_TARA_CONTAINER", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal TaraContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ColetaProcessada", Column = "CNF_COLETA_PROCESSADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ColetaProcessada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Container Container { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "CNF_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Lacres", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COLETA_NOTA_FISCAL_LACRES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CNF_LACRE", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual ICollection<string> Lacres { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Chaves", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COLETA_NOTA_FISCAL_CHAVES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNF_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CNF_CHAVE", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual ICollection<string> Chaves { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Notas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COLETA_NOTA_XML_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CNF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "XMLNotaFiscal", Column = "NFX_CODIGO")]
        public virtual IList<Embarcador.Pedidos.XMLNotaFiscal> Notas { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NumeroOS + " " + this.NumeroContainer;
            }
        }

        public virtual bool Equals(ColetaNotaFiscal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
