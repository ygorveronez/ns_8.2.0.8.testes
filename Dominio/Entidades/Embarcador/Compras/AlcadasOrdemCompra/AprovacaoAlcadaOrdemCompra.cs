using System;

namespace Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_ORDEM_COMPRA", EntityName = "AprovacaoAlcadaOrdemCompra", Name = "Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaOrdemCompra", NameType = typeof(AprovacaoAlcadaOrdemCompra))]
    public class AprovacaoAlcadaOrdemCompra : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemCompra", Column = "ORC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.OrdemCompra OrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasOrdemCompra", Column = "RRC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra RegraOrdemCompra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_DELEGADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Delegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.OrdemCompra?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada:
                        return "Aprovada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada:
                        return "Rejeitada";
                    default:
                        return "";
                }
            }
        }
    }

}