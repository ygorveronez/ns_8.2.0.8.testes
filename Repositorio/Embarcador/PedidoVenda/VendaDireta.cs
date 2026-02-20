using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.PedidoVenda
{
    public class VendaDireta : RepositorioBase<Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta>
    {
        public VendaDireta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ProximoNumeroVendaDireta(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (result.Count() > 0)
                return result.Max(obj => obj.Numero) + 1;
            else
                return 1;
        }

        public List<Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta> Consultar(int funcionarioValidador, ProdutoServico produtoServico, int codigoEmpresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, int funcionario, List<double> cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta statusVendaDireta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDireta statusPedidoVendaDireta, int agendador, DateTime vencimentoInicial, DateTime vencimentoFinal, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta>();

            var result = from obj in query select obj;

            if (produtoServico != ProdutoServico.Nenhum)
                result = result.Where(obj => obj.ProdutoServico == produtoServico);

            if (numeroInicial > 0)
                result = result.Where(obj => obj.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(obj => obj.Numero <= numeroFinal);

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicial.Date);

            if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFinal.Date);

            if (vencimentoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimentoCertificado.Value.Date >= vencimentoInicial.Date);

            if (vencimentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimentoCertificado.Value.Date <= vencimentoFinal.Date);

            if ((int)statusVendaDireta > 0)
                result = result.Where(obj => obj.Status == statusVendaDireta);

            if ((int)statusPedidoVendaDireta > 0)
                result = result.Where(obj => obj.StatusPedido == statusPedidoVendaDireta);

            if (funcionarioValidador > 0)
                result = result.Where(obj => obj.FuncionarioValidador.Codigo == funcionarioValidador);

            if (funcionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == funcionario);

            if (agendador > 0)
                result = result.Where(obj => obj.Agendador.Codigo == agendador);

            if (cliente != null && cliente.Count > 0)
                result = result.Where(obj => cliente.Contains(obj.Cliente.CPF_CNPJ));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int funcionarioValidador, ProdutoServico produtoServico, int codigoEmpresa, int numeroInicial, int numeroFinal, DateTime dataInicial, DateTime dataFinal, int funcionario, List<double> cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta statusVendaDireta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDireta statusPedidoVendaDireta, int agendador, DateTime vencimentoInicial, DateTime vencimentoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta>();

            var result = from obj in query select obj;

            if (produtoServico != ProdutoServico.Nenhum)
                result = result.Where(obj => obj.ProdutoServico == produtoServico);

            if (numeroInicial > 0)
                result = result.Where(obj => obj.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(obj => obj.Numero <= numeroFinal);

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicial.Date);

            if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFinal.Date);

            if ((int)statusVendaDireta > 0)
                result = result.Where(obj => obj.Status == statusVendaDireta);

            if ((int)statusPedidoVendaDireta > 0)
                result = result.Where(obj => obj.StatusPedido == statusPedidoVendaDireta);

            if (funcionarioValidador > 0)
                result = result.Where(obj => obj.FuncionarioValidador.Codigo == funcionarioValidador);

            if (funcionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == funcionario);

            if (agendador > 0)
                result = result.Where(obj => obj.Agendador.Codigo == agendador);

            if (vencimentoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimentoCertificado.Value.Date >= vencimentoInicial.Date);

            if (vencimentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataVencimentoCertificado.Value.Date <= vencimentoFinal.Date);

            if (cliente != null && cliente.Count > 0)
                result = result.Where(obj => cliente.Contains(obj.Cliente.CPF_CNPJ));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }

        #region Relatório de Vendas Diretas

        public IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioVendaDireta> ConsultarRelatorioVendaDireta(int codigoValidador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProdutoServico produtoServico, int codigoGrupoPessoas, DateTime dataVencimentoCertificadoInicial, DateTime dataVencimentoCertificadoFinal, DateTime dataVencimentoCobrancaInicial, DateTime dataVencimentoCobrancaFinal, int codigoEmpresa, DateTime dataVendaInicial, DateTime dataVendaFinal, DateTime dataFinalizacaoInicial, DateTime dataFinalizacaoFinal, DateTime dataAgendamentoInicial, DateTime dataAgendamentoFinal, string numeroPedido, string numeroBoleto, int codigoFuncionario, int codigoAgendador, int codigoProduto, int codigoServico, int codigoTitulo, double pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta statusVenda, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDireta statusPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaVendaDireta tipoCobrancaVendaDireta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioVendaDireta(codigoValidador, produtoServico, codigoGrupoPessoas, dataVencimentoCertificadoInicial, dataVencimentoCertificadoFinal, dataVencimentoCobrancaInicial, dataVencimentoCobrancaFinal, codigoEmpresa, dataVendaInicial, dataVendaFinal, dataFinalizacaoInicial, dataFinalizacaoFinal, dataAgendamentoInicial, dataAgendamentoFinal, numeroPedido, numeroBoleto, codigoFuncionario, codigoAgendador, codigoProduto, codigoServico, codigoTitulo, pessoa, statusVenda, statusPedido, tipoCobrancaVendaDireta, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioVendaDireta)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioVendaDireta>();
        }

        public int ContarConsultaRelatorioVendaDireta(int codigoValidador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProdutoServico produtoServico, int codigoGrupoPessoas, DateTime dataVencimentoCertificadoInicial, DateTime dataVencimentoCertificadoFinal, DateTime dataVencimentoCobrancaInicial, DateTime dataVencimentoCobrancaFinal, int codigoEmpresa, DateTime dataVendaInicial, DateTime dataVendaFinal, DateTime dataFinalizacaoInicial, DateTime dataFinalizacaoFinal, DateTime dataAgendamentoInicial, DateTime dataAgendamentoFinal, string numeroPedido, string numeroBoleto, int codigoFuncionario, int codigoAgendador, int codigoProduto, int codigoServico, int codigoTitulo, double pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta statusVenda, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDireta statusPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaVendaDireta tipoCobrancaVendaDireta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioVendaDireta(codigoValidador, produtoServico, codigoGrupoPessoas, dataVencimentoCertificadoInicial, dataVencimentoCertificadoFinal, dataVencimentoCobrancaInicial, dataVencimentoCobrancaFinal, codigoEmpresa, dataVendaInicial, dataVendaFinal, dataFinalizacaoInicial, dataFinalizacaoFinal, dataAgendamentoInicial, dataAgendamentoFinal, numeroPedido, numeroBoleto, codigoFuncionario, codigoAgendador, codigoProduto, codigoServico, codigoTitulo, pessoa, statusVenda, statusPedido, tipoCobrancaVendaDireta, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioVendaDireta(int codigoValidador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProdutoServico produtoServico, int codigoGrupoPessoas, DateTime dataVencimentoCertificadoInicial, DateTime dataVencimentoCertificadoFinal, DateTime dataVencimentoCobrancaInicial, DateTime dataVencimentoCobrancaFinal, int codigoEmpresa, DateTime dataVendaInicial, DateTime dataVendaFinal, DateTime dataFinalizacaoInicial, DateTime dataFinalizacaoFinal, DateTime dataAgendamentoInicial, DateTime dataAgendamentoFinal, string numeroPedido, string numeroBoleto, int codigoFuncionario, int codigoAgendador, int codigoProduto, int codigoServico, int codigoTitulo, double pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta statusVenda, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDireta statusPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaVendaDireta tipoCobrancaVendaDireta, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioVendaDireta(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioVendaDireta(ref where, ref groupBy, ref joins, codigoValidador, produtoServico, codigoGrupoPessoas, dataVencimentoCertificadoInicial, dataVencimentoCertificadoFinal, dataVencimentoCobrancaInicial, dataVencimentoCobrancaFinal, codigoEmpresa, dataVendaInicial, dataVendaFinal, dataFinalizacaoInicial, dataFinalizacaoFinal, dataAgendamentoInicial, dataAgendamentoFinal, numeroPedido, numeroBoleto, codigoFuncionario, codigoAgendador, codigoProduto, codigoServico, codigoTitulo, pessoa, statusVenda, statusPedido, tipoCobrancaVendaDireta);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioVendaDireta(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_VENDA_DIRETA VendaDireta ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioVendaDireta(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            if (!joins.Contains(" Pessoa "))
                joins += " JOIN T_CLIENTE Pessoa ON Pessoa.CLI_CGCCPF = VendaDireta.CLI_CGCCPF";

            if (!joins.Contains(" GrupoPessoa "))
                joins += " LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoPessoa on GrupoPessoa.GRP_CODIGO = Pessoa.GRP_CODIGO";

            if (!joins.Contains(" FuncionarioTreinamento "))
                joins += " left outer join T_FUNCIONARIO FuncionarioTreinamento on FuncionarioTreinamento.FUN_CODIGO = VendaDireta.FUN_CODIGO_TREINAMENTO";

            if (!joins.Contains(" Funcionario "))
                joins += " left outer join T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = VendaDireta.FUN_CODIGO";

            if (!joins.Contains(" FuncionarioValidador "))
                joins += " left outer join T_FUNCIONARIO FuncionarioValidador on FuncionarioValidador.FUN_CODIGO = VendaDireta.FUN_CODIGO_VALIDADOR";

            if (!joins.Contains(" FuncionarioAgendador "))
                joins += " left outer join T_FUNCIONARIO FuncionarioAgendador on FuncionarioAgendador.FUN_CODIGO = VendaDireta.FUN_CODIGO_AGENDADOR";

            if (!joins.Contains(" FuncionarioContestacao "))
                joins += " left outer join T_FUNCIONARIO FuncionarioContestacao on FuncionarioContestacao.FUN_CODIGO = VendaDireta.FUN_CODIGO_CONTESTACAO";

            if (!joins.Contains(" PessoaEmpresa "))
                joins += " LEFT OUTER JOIN T_CLIENTE PessoaEmpresa ON PessoaEmpresa.CLI_CGCCPF = Pessoa.CLI_CLIENTE_PAI";

            switch (propriedade)
            {
                case "ObservacaoContestacao":
                    if (!select.Contains(" ObservacaoContestacao, "))
                    {
                        select += "VendaDireta.VED_OBSERVACAO_CONTESTACAO ObservacaoContestacao, ";
                        groupBy += "VendaDireta.VED_OBSERVACAO_CONTESTACAO, ";
                    }
                    break;
                case "Funcionario":
                    if (!select.Contains(" Funcionario, "))
                    {
                        select += "Funcionario.FUN_NOME Funcionario, ";
                        groupBy += "Funcionario.FUN_NOME, ";
                    }
                    break;
                case "FuncionarioValidador":
                    if (!select.Contains(" FuncionarioValidador, "))
                    {
                        select += "FuncionarioValidador.FUN_NOME FuncionarioValidador, ";
                        groupBy += "FuncionarioValidador.FUN_NOME, ";
                    }
                    break;
                case "FuncionarioAgendador":
                    if (!select.Contains(" FuncionarioAgendador, "))
                    {
                        select += "FuncionarioAgendador.FUN_NOME FuncionarioAgendador, ";
                        groupBy += "FuncionarioAgendador.FUN_NOME, ";
                    }
                    break;
                case "FuncionarioContestacao":
                    if (!select.Contains(" FuncionarioContestacao, "))
                    {
                        select += "FuncionarioContestacao.FUN_NOME FuncionarioContestacao, ";
                        groupBy += "FuncionarioContestacao.FUN_NOME, ";
                    }
                    break;
                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        select += "ISNULL((SELECT SUM(Itens.VDI_VALOR_TOTAL) FROM T_VENDA_DIRETA_ITEM Itens WHERE Itens.VED_CODIGO = VendaDireta.VED_CODIGO), 0) ValorTotal, ";
                        groupBy += "VendaDireta.VED_CODIGO, ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "VendaDireta.VED_NUMERO Numero, ";
                        groupBy += "VendaDireta.VED_NUMERO, ";
                    }
                    break;
                case "DataAgendadoForaFormatado":
                    if (!select.Contains(" DataAgendadoFora, "))
                    {
                        select += "VendaDireta.VED_DATA_AGENDADO_FORA DataAgendadoFora, ";
                        groupBy += "VendaDireta.VED_DATA_AGENDADO_FORA, ";
                    }
                    break;
                case "DataAprovadoFormatado":
                    if (!select.Contains(" DataAprovado, "))
                    {
                        select += "VendaDireta.VED_DATA_APROVADO DataAprovado, ";
                        groupBy += "VendaDireta.VED_DATA_APROVADO, ";
                    }
                    break;
                case "DataBaixadoFormatado":
                    if (!select.Contains(" DataBaixado, "))
                    {
                        select += "VendaDireta.VED_DATA_BAIXADO DataBaixado, ";
                        groupBy += "VendaDireta.VED_DATA_BAIXADO, ";
                    }
                    break;
                case "DataFaltaAgendarFormatado":
                    if (!select.Contains(" DataFaltaAgendar, "))
                    {
                        select += "VendaDireta.VED_DATA_FALTA_AGENDAR DataFaltaAgendar, ";
                        groupBy += "VendaDireta.VED_DATA_FALTA_AGENDAR, ";
                    }
                    break;
                case "DataAgendadoFormatado":
                    if (!select.Contains(" DataAgendado, "))
                    {
                        select += "VendaDireta.VED_DATA_AGENDADO DataAgendado, ";
                        groupBy += "VendaDireta.VED_DATA_AGENDADO, ";
                    }
                    break;
                case "DataContato1Formatado":
                    if (!select.Contains(" DataContato1, "))
                    {
                        select += "VendaDireta.VED_DATA_CONTATO_1 DataContato1, ";
                        groupBy += "VendaDireta.VED_DATA_CONTATO_1, ";
                    }
                    break;
                case "DataContato2Formatado":
                    if (!select.Contains(" DataContato2, "))
                    {
                        select += "VendaDireta.VED_DATA_CONTATO_2 DataContato2, ";
                        groupBy += "VendaDireta.VED_DATA_CONTATO_2, ";
                    }
                    break;
                case "DataContato3Formatado":
                    if (!select.Contains(" DataContato3, "))
                    {
                        select += "VendaDireta.VED_DATA_CONTATO_3 DataContato3, ";
                        groupBy += "VendaDireta.VED_DATA_CONTATO_3, ";
                    }
                    break;
                case "DataProblemaFormatado":
                    if (!select.Contains(" DataProblema, "))
                    {
                        select += "VendaDireta.VED_DATA_PROBLEMA DataProblema, ";
                        groupBy += "VendaDireta.VED_DATA_PROBLEMA, ";
                    }
                    break;
                case "DataReagendarFormatado":
                    if (!select.Contains(" DataReagendar, "))
                    {
                        select += "VendaDireta.VED_DATA_REAGENDAR DataReagendar, ";
                        groupBy += "VendaDireta.VED_DATA_REAGENDAR, ";
                    }
                    break;
                case "DataClienteBaixaFormatado":
                    if (!select.Contains(" DataClienteBaixa, "))
                    {
                        select += "VendaDireta.VED_DATA_CLIENTE_BAIXA DataClienteBaixa, ";
                        groupBy += "VendaDireta.VED_DATA_CLIENTE_BAIXA, ";
                    }
                    break;
                case "DataAguardandoVerificacaoFormatado":
                    if (!select.Contains(" DataAguardandoVerificacao, "))
                    {
                        select += "VendaDireta.VED_DATA_AGUARDANDO_VERIFICACAO DataAguardandoVerificacao, ";
                        groupBy += "VendaDireta.VED_DATA_AGUARDANDO_VERIFICACAO, ";
                    }
                    break;
                case "DataFormatado":
                    if (!select.Contains(" Data, "))
                    {
                        select += "VendaDireta.VED_DATA Data, ";
                        groupBy += "VendaDireta.VED_DATA, ";
                    }
                    break;
                case "DataFinalizacaoFormatado":
                    if (!select.Contains(" DataFinalizacao, "))
                    {
                        select += "VendaDireta.VED_DATA_FINALIZACAO DataFinalizacao, ";
                        groupBy += "VendaDireta.VED_DATA_FINALIZACAO, ";
                    }
                    break;
                case "DataCancelamentoFormatado":
                    if (!select.Contains(" DataCancelamento, "))
                    {
                        select += "VendaDireta.VED_DATA_CANCELAMENTO DataCancelamento, ";
                        groupBy += "VendaDireta.VED_DATA_CANCELAMENTO, ";
                    }
                    break;
                case "DataValidacaoFormatado":
                    if (!select.Contains(" DataValidacao, "))
                    {
                        select += "VendaDireta.VED_DATA_VALIDACAO DataValidacao, ";
                        groupBy += "VendaDireta.VED_DATA_VALIDACAO, ";
                    }
                    break;
                case "DataContestacaoFormatado":
                    if (!select.Contains(" DataContestacao, "))
                    {
                        select += "VendaDireta.VED_DATA_CONTESTACAO DataContestacao, ";
                        groupBy += "VendaDireta.VED_DATA_CONTESTACAO, ";
                    }
                    break;
                case "DataAgendamentoFormatado":
                    if (!select.Contains(" DataAgendamento, "))
                    {
                        select += "VendaDireta.VED_DATA_AGENDAMENTO DataAgendamento, ";
                        groupBy += "VendaDireta.VED_DATA_AGENDAMENTO, ";
                    }
                    break;
                case "ProdutoServicoFormatado":
                    if (!select.Contains(" ProdutoServico, "))
                    {
                        select += "VendaDireta.VED_PRODUTO_SERVICO ProdutoServico, ";
                        groupBy += "VendaDireta.VED_PRODUTO_SERVICO, ";
                    }
                    break;
                case "DataTreinamentoFormatado":
                    if (!select.Contains(" DataTreinamento, "))
                    {
                        select += "VendaDireta.VED_DATA_TREINAMENTO DataTreinamento, ";
                        groupBy += "VendaDireta.VED_DATA_TREINAMENTO, ";
                    }
                    break;
                case "StatusCadastroFormatado":
                    if (!select.Contains(" StatusCadastro, "))
                    {
                        select += "VendaDireta.VED_STATUS_CADASTRO StatusCadastro, ";
                        groupBy += "VendaDireta.VED_STATUS_CADASTRO, ";
                    }
                    break;
                case "TipoClienteVendaDiretaFormatado":
                    if (!select.Contains(" TipoClienteVendaDireta, "))
                    {
                        select += "VendaDireta.VED_TIPO_CLIENTE_VENDA_DIRETA TipoClienteVendaDireta, ";
                        groupBy += "VendaDireta.VED_TIPO_CLIENTE_VENDA_DIRETA, ";
                    }
                    break;
                case "EmitidoDocumentosFormatado":
                    if (!select.Contains(" EmitidoDocumentos, "))
                    {
                        select += "VendaDireta.VED_EMITIDO_DOCUMENTOS EmitidoDocumentos, ";
                        groupBy += "VendaDireta.VED_EMITIDO_DOCUMENTOS, ";
                    }
                    break;
                case "PendenciaFormatado":
                    if (!select.Contains(" Pendencia, "))
                    {
                        select += "VendaDireta.VED_PENDENCIA Pendencia, ";
                        groupBy += "VendaDireta.VED_PENDENCIA, ";
                    }
                    break;
                case "CertificadoFormatado":
                    if (!select.Contains(" Certificado, "))
                    {
                        select += "VendaDireta.VED_CERTIFICADO Certificado, ";
                        groupBy += "VendaDireta.VED_CERTIFICADO, ";
                    }
                    break;
                case "FuncionarioTreinamento":
                    if (!select.Contains(" FuncionarioTreinamento, "))
                    {
                        select += "FuncionarioTreinamento.FUN_NOME FuncionarioTreinamento, ";
                        groupBy += "FuncionarioTreinamento.FUN_NOME, ";
                    }
                    break;
                case "DataVecimentoCertificadoFormatado":
                    if (!select.Contains(" DataVecimentoCertificado, "))
                    {
                        select += "VendaDireta.VED_DATA_VENCIMENTO_CERTIFICADO DataVecimentoCertificado, ";
                        groupBy += "VendaDireta.VED_DATA_VENCIMENTO_CERTIFICADO, ";
                    }
                    break;
                case "DataVecimentoCobrancaFormatado":
                    if (!select.Contains(" DataVecimentoCobranca, "))
                    {
                        select += "VendaDireta.VED_DATA_VENCIMENTO_COBRANCA DataVecimentoCobranca, ";
                        groupBy += "VendaDireta.VED_DATA_VENCIMENTO_COBRANCA, ";
                    }
                    break;
                case "NecessarioGerarNFFormatado":
                    if (!select.Contains(" NecessarioGerarNF, "))
                    {
                        select += "VendaDireta.VED_GERAR_NF NecessarioGerarNF, ";
                        groupBy += "VendaDireta.VED_GERAR_NF, ";
                    }
                    break;
                case "StatusVendaFormatado":
                    if (!select.Contains(" StatusVenda, "))
                    {
                        select += "VendaDireta.VED_STATUS StatusVenda, ";
                        groupBy += "VendaDireta.VED_STATUS, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "VendaDireta.VED_OBSERVACAO Observacao, ";
                        groupBy += "VendaDireta.VED_OBSERVACAO, ";
                    }
                    break;
                case "TipoAssinaturaFormatado":
                    if (!select.Contains(" TipoAssinatura, "))
                    {
                        select += "VendaDireta.VED_TIPO_ASSINATURA TipoAssinatura, ";
                        groupBy += "VendaDireta.VED_TIPO_ASSINATURA, ";
                    }
                    break;
                case "CodigoEmissao2":
                    if (!select.Contains(" CodigoEmissao2, "))
                    {
                        select += "VendaDireta.VED_CODIGO_EMISSAO_2 CodigoEmissao2, ";
                        groupBy += "VendaDireta.VED_CODIGO_EMISSAO_2, ";
                    }
                    break;
                case "CodigoEmissao1":
                    if (!select.Contains(" CodigoEmissao1, "))
                    {
                        select += "VendaDireta.VED_CODIGO_EMISSAO_1 CodigoEmissao1, ";
                        groupBy += "VendaDireta.VED_CODIGO_EMISSAO_1, ";
                    }
                    break;
                case "StatusPedidoFormatado":
                    if (!select.Contains(" StatusPedido, "))
                    {
                        select += "VendaDireta.VED_STATUS_PEDIDO StatusPedido, ";
                        groupBy += "VendaDireta.VED_STATUS_PEDIDO, ";
                    }
                    break;
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "VendaDireta.VED_CODIGO Codigo, ";
                        groupBy += "VendaDireta.VED_CODIGO, ";
                    }
                    break;

                case "NumeroBoleto":
                    if (!select.Contains(" NumeroBoleto, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + Titulo.TIT_NOSSO_NUMERO
                                    FROM T_VENDA_DIRETA_PARCELA Parcelas
                                    inner join T_TITULO Titulo ON Titulo.TIT_CODIGO = Parcelas.TIT_CODIGO
                                    WHERE Parcelas.VED_CODIGO = VendaDireta.VED_CODIGO FOR XML PATH('')), 3, 1000) NumeroBoleto, ";
                        groupBy += "VendaDireta.VED_CODIGO, ";
                    }
                    break;

                case "StatusTitulo":
                    if (!select.Contains(" StatusTitulo, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + CASE WHEN Titulo.TIT_STATUS = 1 THEN 'Em Aberto' 
                                        WHEN Titulo.TIT_STATUS = 2 THEN 'Atrazado'
                                        WHEN Titulo.TIT_STATUS = 3 THEN 'Quitado'
                                        WHEN Titulo.TIT_STATUS = 4 THEN 'Cancelado'
                                        WHEN Titulo.TIT_STATUS = 5 THEN 'Em Negociação'
                                        WHEN Titulo.TIT_STATUS = 6 THEN 'Bloqueado'
                                        WHEN Titulo.TIT_STATUS = 7 THEN 'Antecipado'
                                        END
                                        FROM T_VENDA_DIRETA_PARCELA Parcelas
                                        inner join T_TITULO Titulo ON Titulo.TIT_CODIGO = Parcelas.TIT_CODIGO
                                        WHERE Parcelas.VED_CODIGO = VendaDireta.VED_CODIGO FOR XML PATH('')), 3, 1000) StatusTitulo, ";
                        groupBy += "VendaDireta.VED_CODIGO, ";
                    }
                    break;

                case "Produtos":
                    if (!select.Contains(" Produtos, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + Produto.PRO_DESCRICAO
                                    FROM T_VENDA_DIRETA_ITEM Itens
                                    inner join T_PRODUTO Produto ON Itens.PRO_CODIGO = Produto.PRO_CODIGO
                                    WHERE Itens.VED_CODIGO = VendaDireta.VED_CODIGO FOR XML PATH('')), 3, 1000) Produtos, ";
                        groupBy += "VendaDireta.VED_CODIGO, ";
                    }
                    break;
                case "Servicos":
                    if (!select.Contains(" Servicos, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + Servico.SER_DESCRICAO
                                    FROM T_VENDA_DIRETA_ITEM Itens
                                    inner join T_SERVICO Servico ON Itens.SER_CODIGO = Servico.SER_CODIGO
                                    WHERE Itens.VED_CODIGO = VendaDireta.VED_CODIGO FOR XML PATH('')), 3, 1000) Servicos, ";
                        groupBy += "VendaDireta.VED_CODIGO, ";
                    }
                    break;
                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido, "))
                    {
                        select += "VendaDireta.VED_NUMERO_PEDIDO NumeroPedido, ";
                        groupBy += "VendaDireta.VED_NUMERO_PEDIDO, ";
                    }
                    break;
                case "CPFCNPJPessoaFormatado":
                    if (!select.Contains(" CNPJCPFPessoa, "))
                    {
                        select += "VendaDireta.CLI_CGCCPF CNPJCPFPessoa, Pessoa.CLI_FISJUR TipoPessoa, ";
                        groupBy += "VendaDireta.CLI_CGCCPF, Pessoa.CLI_FISJUR, ";
                    }
                    break;
                case "GrupoPessoa":
                    if (!select.Contains(" GrupoPessoa, "))
                    {
                        select += "GrupoPessoa.GRP_DESCRICAO RazaoPessoa, ";
                        groupBy += "GrupoPessoa.GRP_DESCRICAO, ";
                    }
                    break;
                case "RazaoPessoa":
                    if (!select.Contains(" RazaoPessoa, "))
                    {
                        select += "Pessoa.CLI_NOME RazaoPessoa, ";
                        groupBy += "Pessoa.CLI_NOME, ";
                    }
                    break;
                case "DataNascimentoFormatada":
                    if (!select.Contains(" DataNascimento, "))
                    {
                        select += "Pessoa.CLI_DATA_NASCIMENTO DataNascimento, ";
                        groupBy += "Pessoa.CLI_DATA_NASCIMENTO, ";
                    }
                    break;
                case "Email":
                    if (!select.Contains(" Email, "))
                    {
                        select += "Pessoa.CLI_EMAIL Email, ";
                        groupBy += "Pessoa.CLI_EMAIL, ";
                    }
                    break;
                case "RG":
                    if (!select.Contains(" RG, "))
                    {
                        select += "Pessoa.CLI_RG RG, ";
                        groupBy += "Pessoa.CLI_RG, ";
                    }
                    break;
                case "OrgaoEmissorFormatado":
                    if (!select.Contains(" OrgaoEmissor, "))
                    {
                        select += "CAST(Pessoa.CLI_ORGAO_EMISSOR_RG AS VARCHAR(2)) OrgaoEmissor, ";
                        groupBy += "Pessoa.CLI_ORGAO_EMISSOR_RG, ";
                    }
                    break;
                case "UF":
                    if (!select.Contains(" UF, "))
                    {
                        if (!joins.Contains(" Localidade "))
                            joins += " LEFT JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = Pessoa.LOC_CODIGO";

                        select += "Localidade.UF_SIGLA UF, ";
                        groupBy += "Localidade.UF_SIGLA, ";
                    }
                    break;
                case "Telefone":
                    if (!select.Contains(" Telefone, "))
                    {
                        select += "Pessoa.CLI_FONE Telefone, ";
                        groupBy += "Pessoa.CLI_FONE, ";
                    }
                    break;
                case "Profissao":
                    if (!select.Contains(" Profissao, "))
                    {
                        select += "Pessoa.CLI_PROFISSAO Profissao, ";
                        groupBy += "Pessoa.CLI_PROFISSAO, ";
                    }
                    break;
                case "TituloEleitoral":
                    if (!select.Contains(" TituloEleitoral, "))
                    {
                        select += "Pessoa.CLI_TITULO_ELEITORAL TituloEleitoral, ";
                        groupBy += "Pessoa.CLI_TITULO_ELEITORAL, ";
                    }
                    break;
                case "ZonaEleitoral":
                    if (!select.Contains(" ZonaEleitoral, "))
                    {
                        select += "Pessoa.CLI_ZONA_ELEITORAL ZonaEleitoral, ";
                        groupBy += "Pessoa.CLI_ZONA_ELEITORAL, ";
                    }
                    break;
                case "SecaoEleitoral":
                    if (!select.Contains(" SecaoEleitoral, "))
                    {
                        select += "Pessoa.CLI_SECAO_ELEITORAL SecaoEleitoral, ";
                        groupBy += "Pessoa.CLI_SECAO_ELEITORAL, ";
                    }
                    break;
                case "Cidade":
                    if (!select.Contains(" Cidade, "))
                    {
                        if (!joins.Contains(" Localidade "))
                            joins += " LEFT JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = Pessoa.LOC_CODIGO";

                        select += "Localidade.LOC_DESCRICAO Cidade, ";
                        groupBy += "Localidade.LOC_DESCRICAO, ";
                    }
                    break;
                case "Estado":
                    if (!select.Contains(" Estado, "))
                    {
                        if (!joins.Contains(" Localidade "))
                            joins += " LEFT JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = Pessoa.LOC_CODIGO";

                        if (!joins.Contains(" Estado "))
                            joins += " LEFT JOIN T_UF Estado ON Estado.UF_SIGLA = Localidade.UF_SIGLA";

                        select += "Estado.UF_NOME Estado, ";
                        groupBy += "Estado.UF_NOME, ";
                    }
                    break;
                case "PisPasep":
                    if (!select.Contains(" PisPasep, "))
                    {
                        select += "Pessoa.CLI_PIS_PASEP PisPasep, ";
                        groupBy += "Pessoa.CLI_PIS_PASEP, ";
                    }
                    break;
                case "NumeroCEI":
                    if (!select.Contains(" NumeroCEI, "))
                    {
                        select += "Pessoa.CLI_NUMERO_CEI NumeroCEI, ";
                        groupBy += "Pessoa.CLI_NUMERO_CEI, ";
                    }
                    break;
                case "CPFCNPJEmpresaFormatado":
                    if (!select.Contains(" CNPJCPFEmpresa, "))
                    {
                        select += "PessoaEmpresa.CLI_CGCCPF CNPJCPFEmpresa, PessoaEmpresa.CLI_FISJUR TipoEmpresa, ";
                        groupBy += "PessoaEmpresa.CLI_CGCCPF, PessoaEmpresa.CLI_FISJUR, ";
                    }
                    break;
                case "RazaoEmpresa":
                    if (!select.Contains(" RazaoEmpresa, "))
                    {
                        select += "PessoaEmpresa.CLI_NOME RazaoEmpresa, ";
                        groupBy += "PessoaEmpresa.CLI_NOME, ";
                    }
                    break;
                case "NumeroCEIEmpresa":
                    if (!select.Contains(" NumeroCEIEmpresa, "))
                    {
                        select += "PessoaEmpresa.CLI_NUMERO_CEI NumeroCEIEmpresa, ";
                        groupBy += "PessoaEmpresa.CLI_NUMERO_CEI, ";
                    }
                    break;
                case "TelefoneEmpresa":
                    if (!select.Contains(" TelefoneEmpresa, "))
                    {
                        select += "PessoaEmpresa.CLI_FONE TelefoneEmpresa, ";
                        groupBy += "PessoaEmpresa.CLI_FONE, ";
                    }
                    break;
                case "CidadeEmpresa":
                    if (!select.Contains(" CidadeEmpresa, "))
                    {
                        if (!joins.Contains(" LocalidadeEmpresa "))
                            joins += " LEFT JOIN T_LOCALIDADES LocalidadeEmpresa ON LocalidadeEmpresa.LOC_CODIGO = PessoaEmpresa.LOC_CODIGO";

                        select += "LocalidadeEmpresa.LOC_DESCRICAO CidadeEmpresa, ";
                        groupBy += "LocalidadeEmpresa.LOC_DESCRICAO, ";
                    }
                    break;
                case "UFEmpresa":
                    if (!select.Contains(" UFEmpresa, "))
                    {
                        if (!joins.Contains(" LocalidadeEmpresa "))
                            joins += " LEFT JOIN T_LOCALIDADES LocalidadeEmpresa ON LocalidadeEmpresa.LOC_CODIGO = PessoaEmpresa.LOC_CODIGO";

                        select += "LocalidadeEmpresa.UF_SIGLA UFEmpresa, ";
                        groupBy += "LocalidadeEmpresa.UF_SIGLA, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioVendaDireta(ref string where, ref string groupBy, ref string joins, int codigoValidador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProdutoServico produtoServico, int codigoGrupoPessoas, DateTime dataVencimentoCertificadoInicial, DateTime dataVencimentoCertificadoFinal, DateTime dataVencimentoCobrancaInicial, DateTime dataVencimentoCobrancaFinal, int codigoEmpresa, DateTime dataVendaInicial, DateTime dataVendaFinal, DateTime dataFinalizacaoInicial, DateTime dataFinalizacaoFinal, DateTime dataAgendamentoInicial, DateTime dataAgendamentoFinal, string numeroPedido, string numeroBoleto, int codigoFuncionario, int codigoAgendador, int codigoProduto, int codigoServico, int codigoTitulo, double pessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta statusVenda, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDireta statusPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaVendaDireta tipoCobrancaVendaDireta)
        {
            string pattern = "yyyy-MM-dd";

            if (!joins.Contains(" Pessoa "))
                joins += " JOIN T_CLIENTE Pessoa ON Pessoa.CLI_CGCCPF = VendaDireta.CLI_CGCCPF";

            if (produtoServico == ProdutoServico.Produto)
                where += " AND VendaDireta.VED_PRODUTO_SERVICO = 1 ";
            else if (produtoServico == ProdutoServico.Servico)
                where += " AND VendaDireta.VED_PRODUTO_SERVICO = 2 ";

            if (codigoEmpresa > 0)
                where += " AND VendaDireta.EMP_CODIGO = '" + codigoEmpresa.ToString() + "' ";

            if (dataVendaInicial != DateTime.MinValue)
                where += " AND VendaDireta.VED_DATA >= '" + dataVendaInicial.ToString(pattern) + "' ";

            if (dataVendaFinal != DateTime.MinValue)
                where += " AND VendaDireta.VED_DATA <= '" + dataVendaFinal.AddDays(1).ToString(pattern) + "'";

            if (dataFinalizacaoInicial != DateTime.MinValue)
                where += " AND VendaDireta.VED_DATA_FINALIZACAO >= '" + dataFinalizacaoInicial.ToString(pattern) + "' ";

            if (dataFinalizacaoFinal != DateTime.MinValue)
                where += " AND VendaDireta.VED_DATA_FINALIZACAO <= '" + dataFinalizacaoFinal.AddDays(1).ToString(pattern) + "'";

            if (dataAgendamentoInicial != DateTime.MinValue)
                where += " AND VendaDireta.VED_DATA_AGENDAMENTO >= '" + dataAgendamentoInicial.ToString(pattern) + "' ";

            if (dataAgendamentoFinal != DateTime.MinValue)
                where += " AND VendaDireta.VED_DATA_AGENDAMENTO <= '" + dataAgendamentoFinal.AddDays(1).ToString(pattern) + "'";

            if (!string.IsNullOrWhiteSpace(numeroPedido))
                where += " AND VendaDireta.VED_NUMERO_PEDIDO LIKE '%" + numeroPedido + "%'";

            if (!string.IsNullOrWhiteSpace(numeroBoleto))
                where += @" AND VendaDireta.VED_CODIGO IN ( SELECT DISTINCT VED_CODIGO FROM T_VENDA_DIRETA_PARCELA VDP
                                                            JOIN T_TITULO T ON T.TIT_CODIGO = VDP.TIT_CODIGO
                                                            WHERE TIT_NOSSO_NUMERO LIKE '%" + numeroBoleto + "%')";

            if (codigoFuncionario > 0)
                where += " AND VendaDireta.FUN_CODIGO = " + codigoFuncionario;

            if (codigoAgendador > 0)
                where += " AND VendaDireta.FUN_CODIGO_AGENDADOR = " + codigoAgendador;

            if (codigoValidador > 0)
                where += " AND VendaDireta.FUN_CODIGO_VALIDADOR = " + codigoValidador;

            if (pessoa > 0)
                where += " AND VendaDireta.CLI_CGCCPF = " + pessoa;

            if (codigoGrupoPessoas > 0)
                where += " AND Pessoa.GRP_CODIGO = " + pessoa;

            if (codigoProduto > 0)
                where += " AND VendaDireta.VED_CODIGO IN ( SELECT DISTINCT VED_CODIGO FROM T_VENDA_DIRETA_ITEM WHERE PRO_CODIGO = " + codigoProduto + ")"; // SQL-INJECTION-SAFE

            if (codigoServico > 0)
                where += " AND VendaDireta.VED_CODIGO IN ( SELECT DISTINCT VED_CODIGO FROM T_VENDA_DIRETA_ITEM WHERE SER_CODIGO = " + codigoServico + ")"; // SQL-INJECTION-SAFE

            if (codigoTitulo > 0)
                where += " AND VendaDireta.VED_CODIGO IN ( SELECT DISTINCT VED_CODIGO FROM T_VENDA_DIRETA_PARCELA WHERE TIT_CODIGO = " + codigoTitulo + ")"; // SQL-INJECTION-SAFE

            if ((int)statusVenda > 0)
                where += " AND VendaDireta.VED_STATUS = " + statusVenda.ToString("D");

            if ((int)statusPedido > 0)
                where += " AND VendaDireta.VED_STATUS_PEDIDO = " + statusPedido.ToString("D");

            if ((int)tipoCobrancaVendaDireta > 0)
                where += " AND VendaDireta.VED_TIPO_COBRANCA_VENDA_DIRETA = " + tipoCobrancaVendaDireta.ToString("D");

            if (dataVencimentoCertificadoInicial != DateTime.MinValue)
                where += " AND VendaDireta.VED_DATA_VENCIMENTO_CERTIFICADO >= '" + dataVencimentoCertificadoInicial.ToString(pattern) + "' ";

            if (dataVencimentoCertificadoFinal != DateTime.MinValue)
                where += " AND VendaDireta.VED_DATA_VENCIMENTO_CERTIFICADO <= '" + dataVencimentoCertificadoFinal.AddDays(1).ToString(pattern) + "'";

            if (dataVencimentoCobrancaInicial != DateTime.MinValue)
                where += " AND VendaDireta.VED_DATA_VENCIMENTO_COBRANCA >= '" + dataVencimentoCobrancaInicial.ToString(pattern) + "' ";

            if (dataVencimentoCobrancaFinal != DateTime.MinValue)
                where += " AND VendaDireta.VED_DATA_VENCIMENTO_COBRANCA <= '" + dataVencimentoCobrancaFinal.AddDays(1).ToString(pattern) + "'";

        }

        #endregion

    }
}
