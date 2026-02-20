using Dominio.ObjetosDeValor.Embarcador.Canhoto;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Canhotos
{
    sealed class ConsultaHistoricoMovimentacaoCanhoto : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaHistoricoMovimentacaoCanhoto>
    {
        #region Construtores

        public ConsultaHistoricoMovimentacaoCanhoto() : base(tabela: "T_CANHOTO_NOTA_FISCAL_HISTORICO CanhotoHistorico ") { }

        #endregion

        #region Métodos Privados
        private void SetarJoinsCanhoto(StringBuilder joins)
        {
            if (!joins.Contains(" Canhoto "))
                joins.Append(" LEFT JOIN T_CANHOTO_NOTA_FISCAL Canhoto ON Canhoto.CNF_CODIGO = CanhotoHistorico.CNF_CODIGO ");
        }
        private void SetarJoinFuncionario(StringBuilder joins)
        {

            if (!joins.Contains(" Funcionario "))
                joins.Append(" LEFT JOIN T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = CanhotoHistorico.FUN_CODIGO ");

        }
        private void SetarJoinMotivoRejeicao(StringBuilder joins)
        {
            SetarJoinsCanhoto(joins);

            if (!joins.Contains(" MotivoRejeicaoAuditoria "))
                joins.Append(" LEFT JOIN T_MOTIVO_REJEICAO_AUDITORIA MotivoRejeicaoAuditoria on MotivoRejeicaoAuditoria.MRA_CODIGO = Canhoto.MRA_CODIGO ");

        }
        private void SetarJoinEmitente(StringBuilder joins)
        {
            SetarJoinsCanhoto(joins);

            if (!joins.Contains(" Emitente "))
                joins.Append(" LEFT JOIN T_CLIENTE Emitente on Emitente.CLI_CGCCPF = Canhoto.CLI_CODIGO_EMITENTE ");
        }
        private void SetarJoinInconsistenciaDigitacaoCanhoto(StringBuilder joins)
        {
            SetarJoinsCanhoto(joins);

            if (!joins.Contains(" InconsistenciaDigitacaoCanhoto "))
                joins.Append(" LEFT JOIN T_INCONSISTENCIA_DIGITACAO_CANHOTO InconsistenciaDigitacaoCanhoto on InconsistenciaDigitacaoCanhoto.CNF_CODIGO = Canhoto.CNF_CODIGO ");
        }
        private void SetarJoinMotivoInconsistencia(StringBuilder joins)
        {
            SetarJoinInconsistenciaDigitacaoCanhoto(joins);

            if (!joins.Contains(" MotivoInconsistenciaCanhoto "))
                joins.Append(" LEFT JOIN T_MOTIVO_INCONSISTENCIA_DIGITACAO_CANHOTO MotivoInconsistenciaCanhoto on MotivoInconsistenciaCanhoto.CMI_CODIGO = InconsistenciaDigitacaoCanhoto.CMI_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaHistoricoMovimentacaoCanhoto filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "DataRecebimentoFisico":
                case "DataRecebimentoFisicoFormatada":
                    if (!select.Contains(" DataRecebimentoFisico, "))
                    {
                        select.Append(@" CASE WHEN Canhoto.CNF_SITUACAO_CANHOTO=3  THEN Canhoto.CNF_DATA_ENVIO_CANHOTO
                                                ELSE null
                                                END DataRecebimentoFisico, ");
                        groupBy.Append("Canhoto.CNF_DATA_ENVIO_CANHOTO, ");
                        groupBy.Append("Canhoto.CNF_SITUACAO_CANHOTO, ");

                        SetarJoinsCanhoto(joins);

                    }
                    break;

                case "DataUpload":
                case "DataUploadFormatada":
                    if (!select.Contains(" DataUpload, "))
                    {
                        select.Append("Canhoto.CNF_DATA_DIGITALIZACAO DataUpload, ");
                        groupBy.Append("Canhoto.CNF_DATA_DIGITALIZACAO, ");

                        SetarJoinsCanhoto(joins);

                    }
                    break;

                case "DataAprovacao":
                case "DataAprovacaoFormatada":
                    if (!select.Contains(" DataAprovacao, "))
                    {

                        select.Append("Canhoto.CNF_DATA_APROVACAO_DIGITALIZACAO DataAprovacao, ");
                        groupBy.Append("Canhoto.CNF_DATA_APROVACAO_DIGITALIZACAO, ");

                    }
                    break;

                case "DataRejeicao":
                case "DataRejeicaoFormatada":
                    if (!select.Contains(" DataRejeicao, "))
                    {
                        select.Append("InconsistenciaDigitacaoCanhoto.CID_DATA DataRejeicao, ");
                        groupBy.Append("InconsistenciaDigitacaoCanhoto.CID_DATA, ");

                        SetarJoinInconsistenciaDigitacaoCanhoto(joins);

                    }
                    break;

                case "DataConfirmacaoEntrega":
                case "DataConfirmacaoEntregaFormatada":

                    if (!select.Contains(" DataConfirmacaoEntrega, "))
                    {
                        select.Append("Canhoto.CNF_DATA_ENTREGA_NOTA_CLIENTE DataConfirmacaoEntrega, ");
                        groupBy.Append("Canhoto.CNF_DATA_ENTREGA_NOTA_CLIENTE, ");

                        SetarJoinsCanhoto(joins);

                    }
                    break;

                case "Usuario":
                    if (!select.Contains(" Usuario, "))
                    {

                        select.Append("Funcionario.FUN_NOME Usuario, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");

                        SetarJoinFuncionario(joins);
                    }
                    break;

                case "MotivoRejeicao":
                    if (!select.Contains(" MotivoRejeicao, "))
                    {

                        select.Append("MotivoInconsistenciaCanhoto.CMI_DESCRICAO MotivoRejeicao, ");
                        groupBy.Append("MotivoInconsistenciaCanhoto.CMI_DESCRICAO, ");

                        SetarJoinMotivoInconsistencia(joins);

                    }
                    break;

                case "DataReversao":
                case "DataReversaoFormatada":
                    if (!select.Contains(" DataReversao, "))
                    {
                        select.Append("Canhoto.CNF_DATA_REVERSAO DataReversao, ");
                        groupBy.Append("Canhoto.CNF_DATA_REVERSAO, ");

                        SetarJoinsCanhoto(joins);

                    }
                    break;

                case "NumeroCanhoto":
                    if (!select.Contains(" NumeroCanhoto, "))
                    {
                        select.Append("Canhoto.CNF_NUMERO NumeroCanhoto, ");
                        groupBy.Append("Canhoto.CNF_NUMERO, ");

                        SetarJoinsCanhoto(joins);
                    }
                    break;

                case "SerieCanhoto":
                    if (!select.Contains(" SerieCanhoto, "))
                    {
                        select.Append("Canhoto.CNF_SERIE SerieCanhoto, ");
                        groupBy.Append("Canhoto.CNF_SERIE, ");

                        SetarJoinsCanhoto(joins);
                    }
                    break;

                case "NomeEmitente":
                    if (!select.Contains(" NomeEmitente, "))
                    {
                        select.Append("Emitente.CLI_NOME NomeEmitente, ");
                        groupBy.Append("Emitente.CLI_NOME, ");

                        SetarJoinEmitente(joins);
                    }
                    break;

                case "CNPJEmitente":
                case "CNPJEmitenteFormatado":
                    if (!select.Contains(" CNPJEmitente, "))
                    {
                        select.Append("Emitente.CLI_CGCCPF CNPJEmitente, ");
                        groupBy.Append("Emitente.CLI_CGCCPF, ");

                        SetarJoinEmitente(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaHistoricoMovimentacaoCanhoto filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.DataUpload != DateTime.MinValue)
                where.Append($" AND canhoto.CNF_DATA_DIGITALIZACAO >= '{filtrosPesquisa.DataUpload.ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.DataAprovacao != DateTime.MinValue)
                where.Append($" AND canhoto.CNF_DATA_APROVACAO_DIGITALIZACAO >= '{filtrosPesquisa.DataAprovacao.ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.DataRejeicao != DateTime.MinValue)
                where.Append($" AND canhoto.CNF_DATA_REJECIAO_AUDITORIA >= '{filtrosPesquisa.DataRejeicao.ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.DataRecebimentoFisico != DateTime.MinValue)
            {
                where.Append($" AND canhoto.CNF_DATA_ENVIO_CANHOTO >= '{filtrosPesquisa.DataRecebimentoFisico.ToString("yyyy-MM-dd")}'  ");
                where.Append($" AND Canhoto.CNF_SITUACAO_CANHOTO = 3 ");

            }


            if (filtrosPesquisa.DataConfirmacaoEntrega != DateTime.MinValue)
                where.Append($" AND canhoto.CNF_DATA_ENTREGA_NOTA_CLIENTE >= '{filtrosPesquisa.DataConfirmacaoEntrega.ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.Usuario > 0)
                where.Append($" AND CanhotoHistorico.FUN_CODIGO = {filtrosPesquisa.Usuario} ");

            if (filtrosPesquisa.MotivoRejeicao > 0)
                where.Append($" AND canhoto.MRA_CODIGO = {filtrosPesquisa.MotivoRejeicao} ");

            if (filtrosPesquisa.DataReversao != DateTime.MinValue)
                where.Append($" AND canhoto.CNF_DATA_REVERSAO >= '{filtrosPesquisa.DataReversao.ToString("yyyy-MM-dd")}' ");

            if (filtrosPesquisa.NumeroCanhoto > 0)
                where.Append($" AND canhoto.CNF_NUMERO = {filtrosPesquisa.NumeroCanhoto} ");

            if (filtrosPesquisa.CodigoEmitente.ToLong() > 0)
            {
                SetarJoinEmitente(joins);
                where.Append($" AND Emitente.CLI_CGCCPF = {filtrosPesquisa.CodigoEmitente} ");
            }


        }
        #endregion
    }
}
