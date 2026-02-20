using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALE_AVULSO", EntityName = "ValeAvulso", Name = "Dominio.Entidades.Embarcador.Financeiro.ValeAvulso", NameType = typeof(ValeAvulso))]
    public class ValeAvulso : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VLA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "VLA_NUMERO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "VLA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "VLA_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Correspondente", Column = "VLA_CORRESPONDENTE", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Correspondente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "VLA_SITUACAO", NotNull = true, TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoValeAvulso))]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoValeAvulso Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroVale", Column = "VLA_NUMERO_VALE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroVale { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "VLA_TIPO_DOCUMENTO", NotNull = false, TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoValeAvulso))]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoValeAvulso TipoDocumento { get; set; }

        public virtual string Descricao
        {
            get { return Numero; }
        }

        public virtual bool Equals(ValeAvulso other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
