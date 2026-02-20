using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.Acerto
{
    sealed class ConsultaBonificacaoAcertoViagem : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioBonificacaoAcertoViagem>
    {
        #region Construtores

        public ConsultaBonificacaoAcertoViagem() : base(tabela: "T_ACERTO_BONIFICACAO as AcertoBonificacao") { }

        #endregion

        #region Métodos Privados
        private void SetarJoinsAcertoViagem(StringBuilder joins)
        {
            if (!joins.Contains(" AcertoViagem "))
                joins.Append(" INNER JOIN T_ACERTO_DE_VIAGEM AcertoViagem ON AcertoViagem.ACV_CODIGO = AcertoBonificacao.ACV_CODIGO ");
        }

        private void SetarJoinsMotorista(StringBuilder joins)
        {
            SetarJoinsAcertoViagem(joins);

            if (!joins.Contains(" Motorista "))
                joins.Append(" INNER JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = AcertoViagem.FUN_CODIGO_MOTORISTA ");
        }
        private void SetarJoinsJustificativa(StringBuilder joins)
        {
            if (!joins.Contains(" Justificativa "))
                joins.Append(" INNER JOIN T_JUSTIFICATIVA Justificativa ON Justificativa.JUS_CODIGO = AcertoBonificacao.JUS_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioBonificacaoAcertoViagem filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("Motorista.FUN_NOME as Motorista, ");
                        groupBy.Append("Motorista.FUN_NOME, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "NumeroAcerto":
                    if (!select.Contains(" NumeroAcerto, "))
                    {
                        select.Append("AcertoViagem.ACV_NUMERO as NumeroAcerto, ");
                        groupBy.Append("AcertoViagem.ACV_NUMERO, ");

                        SetarJoinsAcertoViagem(joins);
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor, "))
                    {
                        select.Append("AcertoBonificacao.ABO_VALOR_BONIFICACAO as Valor, ");
                        groupBy.Append("AcertoBonificacao.ABO_VALOR_BONIFICACAO, ");
                    }
                    break;

                case "Bonificacao":
                    if (!select.Contains(" Bonificacao, "))
                    {
                        select.Append("AcertoBonificacao.ABO_MOTIVO as Bonificacao, ");
                        groupBy.Append("AcertoBonificacao.ABO_MOTIVO, ");
                    }
                    break;

                case "PeriodoAcertoFormatada":
                    if (!select.Contains(" DataInicialAcerto, DataFinalAcerto "))
                    {
                        select.Append("AcertoViagem.ACV_DATA_INICIAL as DataInicialAcerto, AcertoViagem.ACV_DATA_FINAL as DataFinalAcerto, ");
                        groupBy.Append("AcertoViagem.ACV_DATA_INICIAL, AcertoViagem.ACV_DATA_FINAL, ");

                        SetarJoinsAcertoViagem(joins);
                    }
                    break;

                case "Justificativa":
                    if (!select.Contains(" Justificativa "))
                    {
                        select.Append("Justificativa.JUS_DESCRICAO as Justificativa, ");
                        groupBy.Append("Justificativa.JUS_DESCRICAO, ");

                        SetarJoinsJustificativa(joins);
                    }
                    break;

                case "DataFormatada":
                    if (!select.Contains(" Data "))
                    {
                        select.Append("AcertoBonificacao.ABO_DATA as Data, ");
                        groupBy.Append("AcertoBonificacao.ABO_DATA, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioBonificacaoAcertoViagem filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.NumeroAcerto > 0)
            {
                where.Append($" AND AcertoViagem.ACV_NUMERO = {filtrosPesquisa.NumeroAcerto}");
                SetarJoinsAcertoViagem(joins);
            }

            if (filtrosPesquisa.TipoBonificacao > 0)
            {
                where.Append($" AND Justificativa.JUS_CODIGO = {filtrosPesquisa.TipoBonificacao}");
                SetarJoinsJustificativa(joins);
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                where.Append($" AND AcertoViagem.FUN_CODIGO_MOTORISTA = {filtrosPesquisa.CodigoMotorista}");
                SetarJoinsAcertoViagem(joins);
            }

            if (filtrosPesquisa.DataInicialAcerto != DateTime.MinValue)
            {
                where.Append($" AND CAST(AcertoViagem.ACV_DATA_INICIAL AS DATE) >= '{filtrosPesquisa.DataInicialAcerto.ToString(pattern)}'");
                SetarJoinsAcertoViagem(joins);
            }

            if (filtrosPesquisa.DataFinalAcerto != DateTime.MinValue)
            {
                where.Append($" AND CAST(AcertoViagem.ACV_DATA_FINAL AS DATE) <= '{filtrosPesquisa.DataFinalAcerto.ToString(pattern)}'");
                SetarJoinsAcertoViagem(joins);
            }

            if (filtrosPesquisa.DataInicialBonificacao != DateTime.MinValue)
                where.Append($" AND CAST(AcertoBonificacao.ABO_DATA AS DATE) >= '{filtrosPesquisa.DataInicialBonificacao.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinalBonificacao != DateTime.MinValue)
                where.Append($" AND CAST(AcertoBonificacao.ABO_DATA AS DATE) <= '{filtrosPesquisa.DataFinalBonificacao.ToString(pattern)}'");

        }

        #endregion
    }
}
