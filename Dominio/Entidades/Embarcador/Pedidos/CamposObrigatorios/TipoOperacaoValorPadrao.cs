using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_VALOR_PADRAO", EntityName = "TipoOperacaoValorPadrao", Name = "Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoValorPadrao", NameType = typeof(TipoOperacaoValorPadrao))]
    public class TipoOperacaoValorPadrao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacaoCampo", Column = "TOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.CamposObrigatorios.TipoOperacaoCampo Campo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Habilitar", Column = "TOC_HABILITAR", TypeType = typeof(bool))]
        public virtual bool Habilitar { get; set; }
       public virtual string Descricao 
        {  get 
            { 
                return this.Codigo.ToString(); 
            } 
        }
    }
}
