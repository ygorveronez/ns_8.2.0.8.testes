using System;

namespace Dominio.Entidades.Embarcador.Financeiro.Conciliacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EXTRATO_BANCARIO_TIPO_LANCAMENTO", EntityName = "ExtratoBancarioTipoLancamento", Name = "Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento", NameType = typeof(ExtratoBancarioTipoLancamento))]
    public class ExtratoBancarioTipoLancamento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ETP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ETP_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "ETP_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ETP_SITUACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoImportarRegistroAoEstrato", Column = "ETP_NAO_IMPORTAR_REGISTRO_AO_EXTRATO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoImportarRegistroAoEstrato { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }


        public virtual bool Equals(ExtratoBancarioTipoLancamento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoCompleta
        {
            get
            {
                return this.Descricao + " (" + this.CodigoIntegracao + ")";
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Situacao)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
