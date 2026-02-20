using Dominio.Interfaces.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.AbastecimentoInterno
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LIBERACAO_ABASTECIMENTO_AUTOMATIZADO_INTEGRACAO", EntityName = "LiberacaoAbastecimentoAutomatizadoIntegracao", Name = "Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao", NameType = typeof(LiberacaoAbastecimentoAutomatizadoIntegracao))]
    public class LiberacaoAbastecimentoAutomatizadoIntegracao : Integracao.Integracao, IIntegracaoComArquivo<Cargas.CargaCTeIntegracaoArquivo>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReserveID", Column = "AAI_RESERVE_ID", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ReserveID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAutorizacao", Column = "AAI_DATA_AUTORIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LiberacaoAbastecimentoAutomatizado", Column = "LAA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LiberacaoAbastecimentoAutomatizado LiberacaoAbastecimentoAutomatizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AuthID", Column = "AAI_AUTH_ID", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string AuthID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeAbastecida", Column = "AAI_QUANTIDADE_ABASTECIDA", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeAbastecida { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_LIBERACAO_ABASTECIMENTO_AUTOMATIZADO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AAI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

    }
}

