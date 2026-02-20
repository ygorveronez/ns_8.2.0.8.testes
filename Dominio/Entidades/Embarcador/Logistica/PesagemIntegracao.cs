using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESAGEM_INTEGRACAO", EntityName = "PesagemIntegracao", Name = "Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao", NameType = typeof(PesagemIntegracao))]
    public class PesagemIntegracao : Integracao.Integracao, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pesagem", Column = "PSG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pesagem Pesagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoBalanca", Column = "INT_TIPO_INTEGRACAO_BALANCA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoBalanca), NotNull = true)]
        public virtual TipoIntegracaoBalanca TipoIntegracaoBalanca { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FilialBalanca", Column = "FBA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.FilialBalanca Balanca { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESAGEM_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get { return TipoIntegracao.Tipo.ObterDescricao() + " - " + TipoIntegracaoBalanca.ObterDescricao(); }
        }
    }
}
