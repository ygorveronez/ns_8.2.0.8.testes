using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_MARFRIG_CTE_TITULOS_RECEBER", EntityName = "IntegracaoMarfigCteTituloReceber", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber", NameType = typeof(IntegracaoMarfigCteTituloReceber))]
    public class IntegracaoMarfigCteTituloReceber : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "IMT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConsulta", Column = "IMT_DATA_CONSULTA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "IMT_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataQuitacaoCadastro", Column = "IMT_DATA_QUITACAO_CADASTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataQuitacaoCadastro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Retorno", Column = "IMT_RETORNO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Retorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "IMT_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "IMT_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRACAO_MARFRIG_CTE_TITULO_RECEBER_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IMT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "IntegracaoMarfrigCteTituloReceberArquivo", Column = "IMA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

    }
}

