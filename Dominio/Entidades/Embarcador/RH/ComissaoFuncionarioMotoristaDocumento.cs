using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COMISSAO_FUNCIONARIO_MOTORISTA_DOCUMENTO", EntityName = "ComissaoFuncionarioMotoristaDocumento", Name = "Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento", NameType = typeof(ComissaoFuncionarioMotoristaDocumento))]
    public class ComissaoFuncionarioMotoristaDocumento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CMD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComissaoFuncionarioMotorista", Column = "CFM_CODIGO", NotNull = true   , Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista ComissaoFuncionarioMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CFM_NUMERO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "CAR_VEICULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosVinculados", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COMISSAO_FUNCIONARIO_MOTORISTA_DOCUMENTO_VEICULOS_VINCULADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CMD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> VeiculosVinculados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "CFM_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoComissao", Column = "CFM_TIPO_DOCUMENTO_COMISSAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoComissao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoComissao TipoDocumentoComissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaDadosSumarizados", Column = "CDS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados CargaDadosSumarizados { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalFrete", Column = "CMD_VALOR_TOTAL_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "CMD_VALOR_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "CMD_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrosValores", Column = "CMD_OUTROS_VALORES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal OutrosValores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValoFreteLiquido", Column = "CMD_VALOR_FRETE_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValoFreteLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValoBaseCalculo", Column = "CMD_VALOR_BASE_CALCULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValoBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComissao", Column = "CMD_VALOR_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMD_PERCENTUAL_COMISSAO_PARCIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualComissaoParcial { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalFreteOriginal", Column = "CMD_VALOR_TOTAL_FRETE_ORIGINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalFreteOriginal { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValoFreteLiquidoOriginal", Column = "CMD_VALOR_FRETE_LIQUIDO_ORIGINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValoFreteLiquidoOriginal { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValoBaseCalculoOriginal", Column = "CMD_VALOR_BASE_CALCULO_ORIGINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValoBaseCalculoOriginal { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComissaoOriginal", Column = "CMD_VALOR_COMISSAO_ORIGINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorComissaoOriginal { get; set; }
        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualExecucao", Column = "CMD_PERCENTUAL_EXECUCAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualExecucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMD_PESO_CARGA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PesoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CMD_MEDIA_IDEAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MediaIdeal { get; set; }

        public virtual string Descricao
        {
            get
            {
                if(TipoDocumentoComissao == ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoComissao.carga)
                    return this.Carga?.Descricao ?? string.Empty;
                else
                    return this.CargaOcorrencia?.Descricao ?? string.Empty;
            }
        }

        public virtual decimal PercentualUtilizado
        {
            get { return 100 - PercentualExecucao; }
            set { PercentualUtilizado = value; }
        }

        public virtual bool Equals(ComissaoFuncionarioMotoristaDocumento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }


        public virtual Dominio.Entidades.Embarcador.Cargas.Carga CurrentCarga
        {
            get
            {
                return Carga ?? CargaOcorrencia.Carga;
            }
        }
    }
}
