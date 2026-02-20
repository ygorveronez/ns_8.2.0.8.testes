using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_TRANSPORTADOR", EntityName = "TipoOperacaoTransportador", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador", NameType = typeof(TipoOperacaoTransportador))]
    public class TipoOperacaoTransportador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoUltimoPontoRoteirizacao", Column = "TOT_TIPO_ULTIMO_PONTO", TypeType = typeof(TipoUltimoPontoRoteirizacao), NotNull = false)]
        public virtual TipoUltimoPontoRoteirizacao TipoUltimoPontoRoteirizacao { get; set; }

        public virtual string Descricao
        {
            get { return $"Configuração do Transportador {Transportador.Descricao}";  }
        }
    }
}
