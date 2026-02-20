using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_AVERBACOES", EntityName = "FechamentoAverbacao", Name = "Dominio.Entidades.Embarcador.Seguros.FechamentoAverbacao", NameType = typeof(FechamentoAverbacao))]
    public class FechamentoAverbacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAV_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAV_DATA_FECHAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataFechamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAV_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAV_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAV_ADICIONAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Adicional { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FAV_IOF", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal IOF { get; set; }
        
        [NHibernate.Mapping.Attributes.Bag(0, Name = "Averbacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AVERBACOES_FECHAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FAV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AverbacoesFechamento", Column = "AFE_CODIGO")]
        public virtual IList<AverbacoesFechamento> Averbacoes { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch(Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes.Aberta:
                        return "Aberta";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes.Cancelada:
                        return "Cancelada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoAverbacoes.Fechada:
                        return "Fechada";
                    default:
                        return "";
                }
            }
        }
    }
}
