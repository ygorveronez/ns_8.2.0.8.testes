using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Usuarios.Colaborador
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLABORADOR_LANCAMENTO", EntityName = "ColaboradorLancamento", Name = "Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento", NameType = typeof(ColaboradorLancamento))]
    public class ColaboradorLancamento : EntidadeBase, IEquatable<ColaboradorLancamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLS_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLS_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CLS_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "CLS_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "CLS_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoLancamento", Column = "CLS_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador SituacaoLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CLS_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_OPERADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Operador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColaboradorSituacao", Column = "CSI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ColaboradorSituacao ColaboradorSituacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Colaborador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COLABORADOR_LANCAMENTO_ANEXO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CLS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ColaboradorLancamentoAnexo", Column = "CLA_CODIGO")]
        public virtual IList<ColaboradorLancamentoAnexo> Anexos { get; set; }

        public virtual bool Equals(ColaboradorLancamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
