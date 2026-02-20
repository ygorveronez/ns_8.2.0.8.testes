using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_VISITA_PESSOA", EntityName = "ControleVisitaPessoa", Name = "Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoa", NameType = typeof(ControleVisitaPessoa))]
    public class ControleVisitaPessoa : EntidadeBase
    {
        public ControleVisitaPessoa() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "CVP_CPF", TypeType = typeof(string), NotNull = false, Length = 20)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "CVP_NOME", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CVP_DATA_NASCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataNascimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identidade", Column = "CVP_IDENTIDADE", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string Identidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrgaoEmissor", Column = "CVP_ORGAO_EMISSOR", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string OrgaoEmissor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Empresa", Column = "CVP_EMPRESA", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PlacaVeiculo", Column = "CVP_PLACA_VEICULO", TypeType = typeof(string), NotNull = false, Length = 20)]
        public virtual string PlacaVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloVeiculo", Column = "CVP_MODELO_VEICULO", TypeType = typeof(string), NotNull = false, Length = 2000)]
        public virtual string ModeloVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTROLE_VISITA_PESSOA_FOTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CVP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ControleVisitaPessoaFoto", Column = "CPF_CODIGO")]
        public virtual IList<ControleVisitaPessoaFoto> Foto { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Nome;
            }
        }
    }
}