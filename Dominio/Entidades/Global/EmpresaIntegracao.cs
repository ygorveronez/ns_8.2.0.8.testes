using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Entidades.Embarcador.Integracao;
using Dominio.Interfaces.Embarcador.Integracao;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_INTEGRACAO", EntityName = "EmpresaIntegracao", Name = "Dominio.Entidades.EmpresaIntegracao", NameType = typeof(EmpresaIntegracao))]
    public class EmpresaIntegracao : Integracao, IIntegracaoComArquivo<CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SimplesNacional", Column = "INT_SIMPLES_NACIONAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SimplesNacional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtualizouCadastro", Column = "INT_ATUALIZOU_CADASTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtualizouCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EMPRESA_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get { return Empresa.NomeCNPJ; }
        }
    }
}
