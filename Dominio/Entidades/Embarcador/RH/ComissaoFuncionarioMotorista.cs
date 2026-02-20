using System;

namespace Dominio.Entidades.Embarcador.RH
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COMISSAO_FUNCIONARIO_MOTORISTA", EntityName = "ComissaoFuncionarioMotorista", Name = "Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista", NameType = typeof(ComissaoFuncionarioMotorista))]
    public class ComissaoFuncionarioMotorista : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CFM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComissaoFuncionario", Column = "CMF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.RH.ComissaoFuncionario ComissaoFuncionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDiasEmViagem", Column = "CFM_NUMERO_DIAS_EM_VIAGEM", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroDiasEmViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalFrete", Column = "CFM_VALOR_TOTAL_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissao", Column = "CFM_PERCENTUAL_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorNormativo", Column = "CFM_VALOR_NORMATIVO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorNormativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "CFM_VALOR_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPedagio", Column = "CFM_VALOR_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrosValores", Column = "CFM_OUTROS_VALORES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal OutrosValores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValoFreteLiquido", Column = "CFM_VALOR_FRETE_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValoFreteLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValoBaseCalculo", Column = "CFM_VALOR_BASE_CALCULO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValoBaseCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComissao", Column = "CFM_VALOR_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarComissao", Column = "CFM_GERAR_COMISSAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool GerarComissao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaProdutividadeValores", Column = "TPV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaProdutividadeValores TabelaProdutividadeValores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AtingiuMedia", Column = "CFM_ATINGIU_MEDIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AtingiuMedia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoHouveSinitro", Column = "CFM_NAO_HOUVE_SINISTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoHouveSinitro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoHouveAdvertencia", Column = "CFM_NAO_HOUVE_ADVERTENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoHouveAdvertencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ContemDeslocamentoVazio", Column = "CFM_CONTEM_DESLOCAMENTO_VAZIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ContemDeslocamentoVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturamentoMinimo", Column = "CFM_FATURAMENTO_MINIMO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal FaturamentoMinimo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cargo", Column = "CRG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.Cargo CargoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBonificacao", Column = "CFM_VALOR_BONIFICACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorBaseComissao", Column = "CFM_VALOR_BASE_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorBaseComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualMedia", Column = "CFM_PERCENTUAL_MEDIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualMedia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualSinistro", Column = "CFM_PERCENTUAL_SINISTRO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualSinistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAdvertencia", Column = "CFM_PERCENTUAL_ADVERTENCIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualAdvertencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMFinal", Column = "CFM_KM_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal KMFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMInicial", Column = "CFM_KM_INICIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal KMInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMTotal", Column = "CFM_KM_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal KMTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LitrosTotalAbastecimento", Column = "CFM_LITROS_TOTAL_ABASTECIMENTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal LitrosTotalAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaFinal", Column = "CFM_MEDIA_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MediaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaIdeal", Column = "CFM_MEDIA_IDEAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MediaIdeal { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaMediaModeloPeso", Column = "TMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso TabelaMediaModeloPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiDuasFrotas", Column = "CFM_POSSUI_DUAS_FROTAS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiDuasFrotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrimeiraFrota", Column = "CFM_PRIMEIRA_FROTA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string PrimeiraFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MediaPeso", Column = "CFM_MEDIA_PESO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal MediaPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualAtingirMedia", Column = "CFM_PERCENTUAL_ATINGIR_MEDIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? PercentualAtingirMedia { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.Motorista?.Descricao ?? string.Empty;
            }
        }

        public virtual string ValorProdutividade
        {
            get
            {
                return this.TabelaProdutividadeValores?.Valor.ToString("n2") ?? "0,00";
            }
        }        

        public virtual bool Equals(ComissaoFuncionarioMotorista other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
