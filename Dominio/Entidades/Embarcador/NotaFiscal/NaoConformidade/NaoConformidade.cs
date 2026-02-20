using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NAO_CONFORMIDADE", EntityName = "NaoConformidade", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade", NameType = typeof(NaoConformidade))]
    public class NaoConformidade : EntidadeBase, IEntidade
    {
        public NaoConformidade()
        {
            DataCriacao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "NCF_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "NCF_Situacao", TypeType = typeof(SituacaoNaoConformidade), NotNull = true)]
        public virtual SituacaoNaoConformidade Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ItemNaoConformidade", Column = "INC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ItemNaoConformidade ItemNaoConformidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoParticipante", Column = "NCF_TIPO_PARTICIPANTE", TypeType = typeof(TipoParticipante), NotNull = false)]
        public virtual TipoParticipante? TipoParticipante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmailEnviado", Column = "NCF_EMAIL_ENVIADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EmailEnviado { get; set; }

        public virtual string Descricao
        {
            get
            {
                return $"Não conformidade ({ItemNaoConformidade.TipoRegra.ObterDescricao()})";
            }
        }
    }
}
