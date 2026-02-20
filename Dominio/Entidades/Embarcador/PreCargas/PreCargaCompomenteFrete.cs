using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.PreCargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CARGA_COMPONENTES_FRETE", EntityName = "PreCargaCompomenteFrete", Name = "Dominio.Entidades.Embarcador.PreCargas.PreCargaCompomenteFrete", NameType = typeof(PreCargaCompomenteFrete))]
    public class PreCargaCompomenteFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponente", Column = "PCF_VALOR_COMPONENTE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal ValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "PCF_PERCENTUAL_NOTA_FISCAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "PCF_TIPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarValorTotalAReceber", Column = "PCF_DESCONTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcrescentaValorTotalAReceber", Column = "PCF_ACRESCENTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcrescentaValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalAReceber", Column = "PCF_NAO_SOMAR_VALOR_TOTA_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCF_NAO_SOMAR_VALOR_TOTAL_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SomarComponenteFreteLiquido", Column = "PCF_SOMAR_COMPONENTE_FRETE_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SomarComponenteFreteLiquido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarComponenteFreteLiquido", Column = "PCF_DESCONTAR_COMPONENTE_FRETE_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarComponenteFreteLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComponenteFrete", Column = "PCF_TIPO_COMPONENTE_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutraDescricaoCTe", Column = "PCF_OUTRA_DESCRICAO_CTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string OutraDescricaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "PCF_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirBaseCalculoICMS", Column = "PCF_INCLUIR_BC_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SempreExtornar", Column = "PCF_SEMPRE_EXTORNAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreExtornar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComponenteFilialEmissora", Column = "PCF_COMPONENTE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComponenteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioInformou { get; set; }
        
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete)this.MemberwiseClone();
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
