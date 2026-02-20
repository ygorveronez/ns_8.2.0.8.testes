using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACORDO_FATURAMENTO_CLIENTE", EntityName = "AcordoFaturamentoCliente", Name = "Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente", NameType = typeof(AcordoFaturamentoCliente))]
    public class AcordoFaturamentoCliente : EntidadeBase, IEquatable<AcordoFaturamentoCliente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPessoa", Column = "AFC_TIPO_PESSOA", TypeType = typeof(TipoPessoa), NotNull = true)]
        public virtual TipoPessoa TipoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "AFC_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_FATURAMENTO_PERMISSAO_EXCLUSIVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoPermissaoExclusiva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_NAO_ENVIAR_EMAIL_FATURA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoEnviarEmailFaturaAutomaticamente { get; set; }

        #region Frete Cabotagem
        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_FATURAMENTO_PERMISSAO_EXCLUSIVA_CABOTAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoPermissaoExclusivaCabotagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_CABOTAGEM_GERAR_FATURAMENTO_A_VISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CabotagemGerarFaturamentoAVista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CabotagemDiasDePrazoFatura", Column = "AFC_CABOTAGEM_DIA_DE_PRAZO_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int CabotagemDiasDePrazoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CabotagemTipoPrazoFaturamento", Column = "AFC_CABOTAGEM_TIPO_PRAZO_FATURAMENTO", TypeType = typeof(TipoPrazoFaturamento), NotNull = false)]
        public virtual TipoPrazoFaturamento CabotagemTipoPrazoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CabotagemEmail", Column = "AFC_CABOTAGEM_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CabotagemEmail { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CabotagemDiasSemanaFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACORDO_FATURAMENTO_CLIENTE_CABOTAGEM_DIA_SEMANA_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "AFC_CABOTAGEM_DIA_SEMANA_FATURA", TypeType = typeof(DiaSemana), NotNull = true)]
        public virtual ICollection<DiaSemana> CabotagemDiasSemanaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CabotagemDiasMesFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACORDO_FATURAMENTO_CLIENTE_CABOTAGEM_DIA_MES_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "AFC_CABOTAGEM_DIA_MES_FATURA", TypeType = typeof(int), NotNull = true)]
        public virtual ICollection<int> CabotagemDiasMesFatura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "EmailsCabotagem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACORDO_FATURAMENTO_CLIENTE_EMAIL_CABOTAGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcordoFaturamentoEmailCabotagem", Column = "AEC_CODIGO")]
        public virtual IList<AcordoFaturamentoEmailCabotagem> EmailsCabotagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_CABOTAGEM_NAO_ENVIAR_EMAIL_FATURA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CabotagemNaoEnviarEmailFaturaAutomaticamente { get; set; }

        #endregion

        #region Frete Longo Curso
        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_FATURAMENTO_PERMISSAO_EXCLUSIVA_LONGO_CURSO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoPermissaoExclusivaLongoCurso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_LONGO_CURSO_GERAR_FATURAMENTO_A_VISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LongoCursoGerarFaturamentoAVista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongoCursoDiasDePrazoFatura", Column = "AFC_LONGO_CURSO_DIA_DE_PRAZO_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int LongoCursoDiasDePrazoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongoCursoTipoPrazoFaturamento", Column = "AFC_LONGO_CURSO_TIPO_PRAZO_FATURAMENTO", TypeType = typeof(TipoPrazoFaturamento), NotNull = false)]
        public virtual TipoPrazoFaturamento LongoCursoTipoPrazoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LongoCursoEmail", Column = "AFC_LONGO_CURSO_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string LongoCursoEmail { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LongoCursoDiasSemanaFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACORDO_FATURAMENTO_CLIENTE_LONGO_CURSO_DIA_SEMANA_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "AFC_LONGO_CURSO_DIA_SEMANA_FATURA", TypeType = typeof(DiaSemana), NotNull = true)]
        public virtual ICollection<DiaSemana> LongoCursoDiasSemanaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "LongoCursoDiasMesFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACORDO_FATURAMENTO_CLIENTE_LONGO_CURSO_DIA_MES_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "AFC_LONGO_CURSO_DIA_MES_FATURA", TypeType = typeof(int), NotNull = true)]
        public virtual ICollection<int> LongoCursoDiasMesFatura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "EmailsLongoCurso", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACORDO_FATURAMENTO_CLIENTE_EMAIL_LONGO_CURSO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcordoFaturamentoEmailLongoCurso", Column = "ALC_CODIGO")]
        public virtual IList<AcordoFaturamentoEmailLongoCurso> EmailsLongoCurso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_LONGO_CURSO_NAO_ENVIAR_EMAIL_FATURA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LongoCursoNaoEnviarEmailFaturaAutomaticamente { get; set; }

        #endregion

        #region Custo Extra
        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_FATURAMENTO_PERMISSAO_EXCLUSIVA_CUSTO_EXTRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool FaturamentoPermissaoExclusivaCustoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_CUSTO_EXTRA_GERAR_FATURAMENTO_A_VISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CustoExtraGerarFaturamentoAVista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoExtraDiasDePrazoFatura", Column = "AFC_CUSTO_EXTRA_DIA_DE_PRAZO_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int CustoExtraDiasDePrazoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoExtraTipoPrazoFaturamento", Column = "AFC_CUSTO_EXTRA_TIPO_PRAZO_FATURAMENTO", TypeType = typeof(TipoPrazoFaturamento), NotNull = false)]
        public virtual TipoPrazoFaturamento CustoExtraTipoPrazoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CustoExtraEmail", Column = "AFC_CUSTO_EXTRA_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CustoExtraEmail { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CustoExtraDiasSemanaFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACORDO_FATURAMENTO_CLIENTE_CUSTO_EXTRA_DIA_SEMANA_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "AFC_CUSTO_EXTRA_DIA_SEMANA_FATURA", TypeType = typeof(DiaSemana), NotNull = true)]
        public virtual ICollection<DiaSemana> CustoExtraDiasSemanaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CustoExtraDiasMesFatura", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACORDO_FATURAMENTO_CLIENTE_CUSTO_EXTRA_DIA_MES_FATURA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "AFC_CUSTO_EXTRA_DIA_MES_FATURA", TypeType = typeof(int), NotNull = true)]
        public virtual ICollection<int> CustoExtraDiasMesFatura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "EmailsCustoExtra", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ACORDO_FATURAMENTO_CLIENTE_EMAIL_CUSTO_EXTRA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AFC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AcordoFaturamentoEmailCustoExtra", Column = "ALE_CODIGO")]
        public virtual IList<AcordoFaturamentoEmailCustoExtra> EmailsCustoExtra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AFC_CUSTO_EXTRA_NAO_ENVIAR_EMAIL_FATURA_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CustoExtraNaoEnviarEmailFaturaAutomaticamente { get; set; }

        #endregion

        #region Take or Pay

        [NHibernate.Mapping.Attributes.Property(0, Column = "ACF_CONSIDERAR_PARAMETROS_FRETE_CABOTAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsiderarParametrosDeFreteCabotagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TakeOrPayDiasDePrazoFatura", Column = "AFC_TAKE_OR_PAY_DIA_DE_PRAZO_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int TakeOrPayDiasDePrazoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasPrazoFaturamentoDnD", Column = "AFC_DIAS_PRAZO_FATURAMENTO_DND", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasPrazoFaturamentoDnD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasPrazoVencimentoNotaDebito", Column = "AFC_DIAS_PRAZO_VENCIMENTO_NOTA_DEBITO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasPrazoVencimentoNotaDebito { get; set; }

        #endregion

        public virtual string Descricao
        {
            get
            {
                return $"Tipo: {TipoPessoa.ObterDescricao()} - {Pessoa?.Nome ?? GrupoPessoas?.Descricao}";
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
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

        public virtual bool Equals(AcordoFaturamentoCliente other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
