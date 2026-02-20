using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    sealed class ConsultaCheque : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio>
    {
        #region Construtores

        public ConsultaCheque() : base(tabela: "T_CHEQUE as Cheque") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsBanco(StringBuilder joins)
        {
            if (!joins.Contains(" Banco "))
                joins.Append("join T_BANCO Banco on Banco.BCO_CODIGO = Cheque.BCO_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append("join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Cheque.EMP_CODIGO ");
        }

        private void SetarJoinsPessoa(StringBuilder joins)
        {
            if (!joins.Contains(" Pessoa "))
                joins.Append("join T_CLIENTE Pessoa on Pessoa.CLI_CGCCPF = Cheque.CLI_CGCCPF ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio filtroPesquisa)
        {
            switch (propriedade)
            {
                case "CnpjEmpresa":
                    if (!select.Contains(" CnpjEmpresa"))
                    {
                        select.Append("Empresa.EMP_CNPJ as CnpjEmpresa, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CpfCnpjPessoaFormatado":
                    if (!select.Contains(" CpfCnpjPessoa"))
                    {
                        select.Append("Pessoa.CLI_CGCCPF as CpfCnpjPessoa, Pessoa.CLI_FISJUR as TipoPessoa, ");
                        groupBy.Append("Pessoa.CLI_CGCCPF, Pessoa.CLI_FISJUR, ");

                        SetarJoinsPessoa(joins);
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("Cheque.CHQ_CODIGO as Codigo, ");
                        groupBy.Append("Cheque.CHQ_CODIGO, ");
                    }
                    break;

                case "DataCadastro":
                case "DataCadastroFormatada":
                    if (!select.Contains(" DataCadastro"))
                    {
                        select.Append("Cheque.CHQ_DATA_CADASTRO as DataCadastro, ");
                        groupBy.Append("Cheque.CHQ_DATA_CADASTRO, ");
                    }
                    break;

                case "DataCompensacao":
                case "DataCompensacaoFormatada":
                    if (!select.Contains(" DataCompensacao"))
                    {
                        select.Append("Cheque.CHQ_DATA_COMPENSACAO as DataCompensacao, ");
                        groupBy.Append("Cheque.CHQ_DATA_COMPENSACAO, ");
                    }
                    break;

                case "DataTransacao":
                case "DataTransacaoFormatada":
                    if (!select.Contains(" DataTransacao"))
                    {
                        select.Append("Cheque.CHQ_DATA_TRANSACAO as DataTransacao, ");
                        groupBy.Append("Cheque.CHQ_DATA_TRANSACAO, ");
                    }
                    break;

                case "DataVencimento":
                case "DataVencimentoFormatada":
                    if (!select.Contains(" DataVencimento"))
                    {
                        select.Append("Cheque.CHQ_DATA_VENCIMENTO as DataVencimento, ");
                        groupBy.Append("Cheque.CHQ_DATA_VENCIMENTO, ");
                    }
                    break;

                case "DescricaoBanco":
                    if (!select.Contains(" DescricaoBanco"))
                    {
                        select.Append("Banco.BCO_DESCRICAO as DescricaoBanco, ");
                        groupBy.Append("Banco.BCO_DESCRICAO, ");

                        SetarJoinsBanco(joins);
                    }
                    break;

                case "DigitoAgencia":
                    if (!select.Contains(" DigitoAgencia"))
                    {
                        select.Append("Cheque.CHQ_DIGITO_AGENCIA as DigitoAgencia, ");
                        groupBy.Append("Cheque.CHQ_DIGITO_AGENCIA, ");
                    }
                    break;

                case "NomePessoa":
                    if (!select.Contains(" NomePessoa"))
                    {
                        select.Append("Pessoa.CLI_NOME as NomePessoa, ");
                        groupBy.Append("Pessoa.CLI_NOME, ");

                        SetarJoinsPessoa(joins);
                    }
                    break;

                case "NumeroAgencia":
                    if (!select.Contains(" NumeroAgencia"))
                    {
                        select.Append("Cheque.CHQ_NUMERO_AGENCIA as NumeroAgencia, ");
                        groupBy.Append("Cheque.CHQ_NUMERO_AGENCIA, ");
                    }
                    break;

                case "NumeroCheque":
                    if (!select.Contains(" NumeroCheque"))
                    {
                        select.Append("Cheque.CHQ_NUMERO_CHEQUE as NumeroCheque, ");
                        groupBy.Append("Cheque.CHQ_NUMERO_CHEQUE, ");
                    }
                    break;

                case "NumeroConta":
                    if (!select.Contains(" NumeroConta"))
                    {
                        select.Append("Cheque.CHQ_NUMERO_CONTA as NumeroConta, ");
                        groupBy.Append("Cheque.CHQ_NUMERO_CONTA, ");
                    }
                    break;

                case "NumeroTitulo":
                    if (!select.Contains(" NumeroTitulo"))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    select ', ' + cast(Titulo.TIT_CODIGO as nvarchar(20)) ");
                        select.Append("      from T_TITULO_BAIXA_CHEQUE TituloBaixaCheque ");
                        select.Append("      join T_TITULO_BAIXA TituloBaixa on TituloBaixa.TIB_CODIGO = TituloBaixaCheque.TIB_CODIGO ");
                        select.Append("      join T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado on TituloBaixaAgrupado.TIB_CODIGO = TituloBaixa.TIB_CODIGO ");
                        select.Append("      join T_TITULO Titulo on Titulo.TIT_CODIGO = TituloBaixaAgrupado.TIT_CODIGO ");
                        select.Append("     where TituloBaixaCheque.CHQ_CODIGO = Cheque.CHQ_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as NumeroTitulo, ");

                        groupBy.Append("Cheque.CHQ_CODIGO, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao"))
                    {
                        select.Append("Cheque.CHQ_OBSERVACAO as Observacao, ");
                        groupBy.Append("Cheque.CHQ_OBSERVACAO, ");
                    }
                    break;

                case "RazaoSocialEmpresa":
                    if (!select.Contains(" RazaoSocialEmpresa"))
                    {
                        select.Append("Empresa.EMP_RAZAO as RazaoSocialEmpresa, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Status":
                case "StatusDescricao":
                    if (!select.Contains(" Status"))
                    {
                        select.Append("Cheque.CHQ_STATUS as Status, ");
                        groupBy.Append("Cheque.CHQ_STATUS, ");
                    }
                    break;

                case "Tipo":
                case "TipoDescricao":
                    if (!select.Contains(" Tipo"))
                    {
                        select.Append("Cheque.CHQ_TIPO as Tipo, ");
                        groupBy.Append("Cheque.CHQ_TIPO, ");
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor"))
                    {
                        select.Append("Cheque.CHQ_VALOR as Valor, ");
                        groupBy.Append("Cheque.CHQ_VALOR, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.CpfCnpjPessoa > 0d)
                where.Append($" and Cheque.CLI_CGCCPF = {filtrosPesquisa.CpfCnpjPessoa}");

            if (filtrosPesquisa.Status.HasValue)
                where.Append($" and Cheque.CHQ_STATUS = {(int)filtrosPesquisa.Status.Value}");

            if (filtrosPesquisa.Tipo.HasValue)
                where.Append($" and Cheque.CHQ_TIPO = {(int)filtrosPesquisa.Tipo.Value}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCheque))
                where.Append($" and Cheque.CHQ_NUMERO_CHEQUE like '%{filtrosPesquisa.NumeroCheque}%'");

            if (filtrosPesquisa.DataCompensacaoInicio.HasValue)
                where.Append($" and Cheque.CHQ_DATA_COMPENSACAO >= '{filtrosPesquisa.DataCompensacaoInicio.Value.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataCompensacaoLimite.HasValue)
                where.Append($" and Cheque.CHQ_DATA_COMPENSACAO <= '{filtrosPesquisa.DataCompensacaoLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataTransacaoInicio.HasValue)
                where.Append($" and Cheque.CHQ_DATA_TRANSACAO >= '{filtrosPesquisa.DataTransacaoInicio.Value.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataTransacaoLimite.HasValue)
                where.Append($" and Cheque.CHQ_DATA_TRANSACAO <= '{filtrosPesquisa.DataTransacaoLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataVencimentoInicio.HasValue)
                where.Append($" and Cheque.CHQ_DATA_VENCIMENTO >= '{filtrosPesquisa.DataVencimentoInicio.Value.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataVencimentoLimite.HasValue)
                where.Append($" and Cheque.CHQ_DATA_VENCIMENTO <= '{filtrosPesquisa.DataVencimentoLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.ValorInicio > 0m)
                where.Append($" and Cheque.CHQ_VALOR >= {filtrosPesquisa.ValorInicio.ToString("n2").Replace(".", "").Replace(",", ".")}");

            if (filtrosPesquisa.ValorLimite > 0m)
                where.Append($" and Cheque.CHQ_VALOR <= {filtrosPesquisa.ValorLimite.ToString("n2").Replace(".", "").Replace(",", ".")}");

            if (filtrosPesquisa.CodigoTitulo > 0)
            {
                where.Append(" and exists( ");
                where.Append("     select 1 ");
                where.Append("       from T_TITULO_BAIXA_CHEQUE TituloBaixaCheque ");
                where.Append("       join T_TITULO_BAIXA TituloBaixa on TituloBaixa.TIB_CODIGO = TituloBaixaCheque.TIB_CODIGO ");
                where.Append("       join T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado on TituloBaixaAgrupado.TIB_CODIGO = TituloBaixa.TIB_CODIGO ");
                where.Append("      where TituloBaixaCheque.CHQ_CODIGO = Cheque.CHQ_CODIGO ");
                where.Append($"       and TituloBaixaAgrupado.TIT_CODIGO = {filtrosPesquisa.CodigoTitulo} ");
                where.Append(" ) ");
            }

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" and Cheque.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

            if (filtrosPesquisa.Banco > 0)
                where.Append($" and Cheque.BCO_CODIGO = {filtrosPesquisa.Banco}");
        }

        #endregion
    }
}
