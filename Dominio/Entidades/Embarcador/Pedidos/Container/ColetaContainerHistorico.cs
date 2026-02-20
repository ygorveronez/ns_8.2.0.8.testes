using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_CONTAINER_HISTORICO", EntityName = "ColetaContainerHistorico", Name = "Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico", NameType = typeof(ColetaContainerHistorico))]

    public class ColetaContainerHistorico : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColetaContainer", Column = "CCR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer ColetaContainer { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_DATA_HISTORICO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataHistorico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_DATA_FIM_HISTORICO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimHistorico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_LOCAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Local { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CCH_STATUS", TypeType = typeof(StatusColetaContainer), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemMovimentacao", Column = "CCH_ORIGEM", TypeType = typeof(OrigemMovimentacaoContainer), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemMovimentacaoContainer OrigemMovimentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformacaoOrigemMovimentacao", Column = "CCH_INFORMACAO", TypeType = typeof(OrigemMovimentacaoContainer), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.InformacaoOrigemMovimentacaoContainer InformacaoOrigemMovimentacao { get; set; }

        /// <summary>
        /// Campo criado para gravar o codigo da carga no momento do historico (pode ser uma pré carga)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCH_CODIGO_CARGA_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCargaEmbarcador { get; set; }

        public virtual string ObterCodigoCargaEmbarcador()
        {
            return string.IsNullOrWhiteSpace(CodigoCargaEmbarcador) ? Carga?.CodigoCargaEmbarcador ?? "" : CodigoCargaEmbarcador;
        }
    }
}
