using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    /// <summary>
    /// ARMAZENA O HISTÓRICO DOS COMPONENTES OBTIDOS PELA TABELA DE FRETE DA CARGA
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_TABELA_FRETE_COMPONENTES_FRETE", EntityName = "CargaTabelaFreteComponenteFrete", Name = "Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteComponenteFrete", NameType = typeof(CargaTabelaFreteComponenteFrete))]
    public class CargaTabelaFreteComponenteFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponente", Column = "CTC_VALOR_COMPONENTE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal ValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualSobreNF", Column = "CTC_PERCENTUAL_NOTA_FISCAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualSobreNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComponenteFrete", Column = "CTC_TIPO_COMPONENTE_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteComponenteFrete Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteComponenteFrete)this.MemberwiseClone();
        }
        public virtual string DescricaoComponente
        {
            get
            {
                return TipoComponenteFrete.ObterDescricao();
            }
        }
    }
}
