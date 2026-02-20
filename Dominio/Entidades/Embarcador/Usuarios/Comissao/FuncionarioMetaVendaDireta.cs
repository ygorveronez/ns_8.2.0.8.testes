using System;

namespace Dominio.Entidades.Embarcador.Usuarios.Comissao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_META_VENDA_DIRETA", EntityName = "FuncionarioMetaVendaDireta", Name = "Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaVendaDireta", NameType = typeof(FuncionarioMetaVendaDireta))]

    public class FuncionarioMetaVendaDireta : EntidadeBase, IEquatable<FuncionarioMetaVendaDireta>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FMV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "FMV_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "FMV_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FMV_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FMV_TIPO_META_VENDA_DIRETA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMetaVendaDireta), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMetaVendaDireta TipoMetaVendaDireta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataGeracao", Column = "FMV_DATA_GERACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataGeracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualMeta", Column = "FMV_PERCENTUAL_META", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = false)]
        public virtual decimal PercentualMeta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(FuncionarioMetaVendaDireta other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
