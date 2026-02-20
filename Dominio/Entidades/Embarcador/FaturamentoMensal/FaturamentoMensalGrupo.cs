using System;

namespace Dominio.Entidades.Embarcador.FaturamentoMensal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURAMENTO_MENSAL_GRUPO", EntityName = "FaturamentoMensalGrupo", Name = "Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo", NameType = typeof(FaturamentoMensalGrupo))]
    public class FaturamentoMensalGrupo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FMG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FMG_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FMG_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoObservacaoFaturamentoMensal", Column = "FMG_TIPO_OBSERVACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal TipoObservacaoFaturamentoMensal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "FMG_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturamentoAutomatico", Column = "FMG_FATURAMENTO_AUTOMATICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaFatura", Column = "FMG_DIA_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaFatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Servico", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.Servico Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO_DENTRO_ESTADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaDaOperacao NaturezaDaOperacaoDentroEstado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO_FORA_ESTADO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaDaOperacao NaturezaDaOperacaoForaEstado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.BoletoConfiguracao BoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoAdesao", Column = "FMG_OBSERVACAO_ADESAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoAdesao { get; set; }

        public virtual string DescricaoTipoObservacaoFaturamentoMensal
        {
            get
            {
                switch (this.TipoObservacaoFaturamentoMensal)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto:
                        return "Usar em BOLETO";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum:
                        return "Nenhum";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscal:
                        return "Usar em NF";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto:
                        return "Usar em NF e BOLETO";
                    default:
                        return "";
                }
            }
        }
        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(FaturamentoMensalGrupo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
