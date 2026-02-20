using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RECEBIMENTO", EntityName = "Recebimento", Name = "Dominio.Entidades.Embarcador.WMS.Recebimento", NameType = typeof(Recebimento))]
    public class Recebimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "RME_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "RME_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoRecebimento", Column = "RME_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento SituacaoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRecebimentoMercadoria", Column = "RME_TIPO_RECEBIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria TipoRecebimentoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Mercadorias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_RECEBIMENTO_MERCADORIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RME_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RecebimentoMercadoria", Column = "REM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> Mercadorias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AutorizadoProdutosFaltantes", Column = "RME_AUTORIZADO_PRODUTOS_FALTANTES", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AutorizadoProdutosFaltantes { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }

        public virtual string DescricaoSituacaoRecebimento
        {
            get
            {
                if (this.SituacaoRecebimento == ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Cancelada)
                    return "Cancelado";
                if (this.SituacaoRecebimento == ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Finalizada)
                    return "Finalizado";
                if (this.SituacaoRecebimento == ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Iniciada)
                    return "Iniciado";
                else
                    return "";
            }
        }

        public virtual string DescricaoTipoRecebimentoMercadoria
        {
            get
            {
                if (this.TipoRecebimentoMercadoria == ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria)
                    return "Mercadoria";
                if (this.TipoRecebimentoMercadoria == ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume)
                    return "Volume";                
                else
                    return "Mercadoria";
            }
        }

    }
}
