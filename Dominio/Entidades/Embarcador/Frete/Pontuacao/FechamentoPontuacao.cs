using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete.Pontuacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_PONTUACAO", EntityName = "FechamentoPontuacao", Name = "Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao", NameType = typeof(FechamentoPontuacao))]
    public class FechamentoPontuacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FPN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FPN_ANO", TypeType = typeof(int), NotNull = true)]
        public virtual int Ano { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "FPN_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FPN_MES", TypeType = typeof(Mes), NotNull = true)]
        public virtual Mes Mes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FPN_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FPN_SITUACAO", TypeType = typeof(SituacaoFechamentoPontuacao), NotNull = true)]
        public virtual SituacaoFechamentoPontuacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "FechamentoPontuacaoTransportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FECHAMENTO_PONTUACAO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FPN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FechamentoPontuacaoTransportador", Column = "FPT_CODIGO")]
        public virtual IList<FechamentoPontuacaoTransportador> FechamentoPontuacaoTransportadores { get; set; }

        public virtual string Descricao
        {
            get { return $"Fechamento de pontuação de {Mes.ObterDescricao().ToLower()} de {Ano}"; }
        }

        public virtual bool FechamentoMesAnterior
        {
            get
            {
                DateTime dataAtual = DateTime.Now;

                return (dataAtual.Year == Ano) && ((dataAtual.Month - 1) == (int)Mes);
            }
        }
    }
}
