using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao.OFX;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Financeiro
{
    public class ConciliacaoBancaria
    {
        #region Métodos Públicos

        public static object ObterDetalhesConciliacaoBancaria(int codigo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
            Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiro = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo);

            IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria> listaMovimentosConcolidados = repMovimentoFinanceiro.ConsultarMovimentoConcolidados(codigo);
            IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria> listaExtratosConcolidados = repExtratoBancario.ConsultarExtratoConcolidados(codigo);

            return new
            {
                Codigo = conciliacaoBancaria.Codigo,
                DataFinal = conciliacaoBancaria.DataFinal.HasValue ? conciliacaoBancaria.DataFinal.Value.ToString("dd/MM/yyyy") : "",
                DataInicial = conciliacaoBancaria.DataInicial.HasValue ? conciliacaoBancaria.DataInicial.Value.ToString("dd/MM/yyyy") : "",
                PlanoConta = new { Codigo = conciliacaoBancaria.PlanoConta != null ? conciliacaoBancaria.PlanoConta.Codigo : 0, Descricao = conciliacaoBancaria.PlanoConta != null ? conciliacaoBancaria.PlanoConta.Descricao : "" },
                PlanoContaSintetico = new { Codigo = conciliacaoBancaria.PlanoContaSintetico != null ? conciliacaoBancaria.PlanoContaSintetico.Codigo : 0, Descricao = conciliacaoBancaria.PlanoContaSintetico != null ? conciliacaoBancaria.PlanoContaSintetico.Descricao : "" },
                conciliacaoBancaria.AnaliticoSintetico,
                conciliacaoBancaria.RealizarConciliacaoAutomatica,
                conciliacaoBancaria.SituacaoConciliacaoBancaria,
                ValorTotalCreditoExtrato = conciliacaoBancaria.ValorTotalCreditoExtrato.ToString("n2"),
                ValorTotalCreditoMovimento = conciliacaoBancaria.ValorTotalCreditoMovimento.ToString("n2"),
                ValorTotalDebitoExtrato = conciliacaoBancaria.ValorTotalDebitoExtrato.ToString("n2"),
                ValorTotalDebitoMovimento = conciliacaoBancaria.ValorTotalDebitoMovimento.ToString("n2"),
                ValorTotalExtrato = conciliacaoBancaria.ValorTotalExtrato.ToString("n2"),
                ValorTotalMovimento = conciliacaoBancaria.ValorTotalMovimento.ToString("n2"),
                ValorTotalGeralDebitoMovimento = conciliacaoBancaria.ValorTotalGeralDebitoMovimento.ToString("n2"),
                ValorTotalGeralCreditoMovimento = conciliacaoBancaria.ValorTotalGeralCreditoMovimento.ToString("n2"),
                ValorTotalGeralMovimento = conciliacaoBancaria.ValorTotalGeralMovimento.ToString("n2"),
                ValorTotalGeralDebitoExtrato = conciliacaoBancaria.ValorTotalGeralDebitoExtrato.ToString("n2"),
                ValorTotalGeralCreditoExtrato = conciliacaoBancaria.ValorTotalGeralCreditoExtrato.ToString("n2"),
                ValorTotalGeralExtrato = conciliacaoBancaria.ValorTotalGeralExtrato.ToString("n2"),
                ExtratosConcolidados = listaExtratosConcolidados != null ? (
                        from p in listaExtratosConcolidados
                        select new
                        {
                            DT_RowId = p.Codigo.ToString(),
                            p.Codigo,
                            p.CodigoPlano,
                            p.ExtratoConcolidado,
                            p.DescricaoDataMovimento,
                            p.Documento,
                            p.DescricaoTipoDocumentoMovimento,
                            p.Observacao,
                            p.CodigoLancamento,
                            p.ValorDebito,
                            p.ValorCredito,
                            p.Saldo
                        }
                    ).ToList() : null,
                MovimentosConcolidados = listaMovimentosConcolidados != null ? (
                        from p in listaMovimentosConcolidados
                        select new
                        {
                            DT_RowId = p.Codigo.ToString(),
                            p.Codigo,
                            p.CodigoPlano,
                            p.MovimentoConcolidado,
                            p.DescricaoDataMovimento,
                            p.Documento,
                            p.DescricaoTipoDocumentoMovimento,
                            p.Observacao,
                            p.ValorDebito,
                            p.ValorCredito,
                            p.Pessoa
                        }
                    ).ToList() : null
            };
        }

        public static bool ProcessarConciliacaoBancaria(int codigo, string planoContaSintetico, int codigoPlanoConta, DateTime? dataInicial, DateTime? dataFinal, int codigoEmpresa, bool concolidarAutomaticamente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimentoFinanceiroDebitoCredito = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
            Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria repConciliacaoBancaria = new Repositorio.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito> movimentos = repMovimentoFinanceiroDebitoCredito.BuscarMovimentoParaConciliacaoBancaria(planoContaSintetico, codigoPlanoConta, dataInicial, dataFinal, codigoEmpresa);
            List<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario> extratos = repExtratoBancario.BuscarMovimentoParaConciliacaoBancaria(planoContaSintetico, codigoPlanoConta, dataInicial, dataFinal, codigoEmpresa);
            Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria conciliacaoBancaria = repConciliacaoBancaria.BuscarPorCodigo(codigo);

            decimal valorTotalCreditoExtrato = 0, valorTotalCreditoMovimento = 0, valorTotalDebitoExtrato = 0, valorTotalDebitoMovimento = 0;

            decimal valorTotalGeralDebitoMovimento = 0, valorTotalGeralCreditoMovimento = 0, valorTotalGeralMovimento = 0, valorTotalGeralDebitoExtrato = 0, valorTotalGeralCreditoExtrato = 0, valorTotalGeralExtrato = 0;

            bool contemExtratos = extratos?.Count > 0;
            bool contemMovimentos = movimentos?.Count > 0;

            if (conciliacaoBancaria.Movimentos == null)
                conciliacaoBancaria.Movimentos = new List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();
            if (conciliacaoBancaria.Extratos == null)
                conciliacaoBancaria.Extratos = new List<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario>();

            if (contemMovimentos)
            {
                valorTotalGeralDebitoMovimento = movimentos.Where(c => c.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)?.Sum(c => (decimal?)c.Valor) ?? 0m;
                valorTotalGeralCreditoMovimento = movimentos.Where(c => c.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)?.Sum(c => (decimal?)c.Valor) ?? 0m;
                valorTotalGeralMovimento = valorTotalGeralCreditoMovimento - valorTotalGeralDebitoMovimento;
                foreach (var movimento in movimentos)
                {
                    if (!conciliacaoBancaria.Movimentos.Contains(movimento))
                        conciliacaoBancaria.Movimentos.Add(movimento);

                    //if (repMovimentoFinanceiroDebitoCredito.ConteMovimentacaoEmOutraConciliacao(movimento.Codigo, codigo))
                    //    continue;

                    if (concolidarAutomaticamente && contemExtratos)
                    {
                        if (extratos.Any(o => o.Valor == movimento.Valor && o.Documento == movimento.MovimentoFinanceiro.Documento && o.DebitoCredito != movimento.DebitoCredito))
                        {
                            movimento.MovimentoConcolidado = true;
                            repMovimentoFinanceiroDebitoCredito.Atualizar(movimento);
                            if (movimento.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)
                                valorTotalCreditoMovimento += movimento.Valor;
                            else
                                valorTotalDebitoMovimento += movimento.Valor;
                        }
                    }
                }
            }

            if (contemExtratos)
            {
                valorTotalGeralDebitoExtrato = extratos.Where(c => c.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)?.Sum(c => (decimal?)c.Valor) ?? 0m;
                valorTotalGeralCreditoExtrato = extratos.Where(c => c.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)?.Sum(c => (decimal?)c.Valor) ?? 0m;
                valorTotalGeralExtrato = valorTotalGeralCreditoExtrato - valorTotalGeralDebitoExtrato;
                foreach (var extrato in extratos)
                {
                    if (!conciliacaoBancaria.Extratos.Contains(extrato))
                        conciliacaoBancaria.Extratos.Add(extrato);

                    //if (repExtratoBancario.ContemExtratoEmOutraConciliacao(extrato.Codigo, codigo))
                    //    continue;

                    if (concolidarAutomaticamente && contemMovimentos)
                    {
                        if (movimentos.Any(o => o.Valor == extrato.Valor && o.MovimentoFinanceiro.Documento == extrato.Documento && o.DebitoCredito != extrato.DebitoCredito))
                        {
                            extrato.ExtratoConcolidado = true;
                            repExtratoBancario.Atualizar(extrato);
                            if (extrato.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)
                                valorTotalCreditoExtrato += extrato.Valor;
                            else
                                valorTotalDebitoExtrato += extrato.Valor;
                        }
                    }
                }
            }

            conciliacaoBancaria.ValorTotalCreditoExtrato += valorTotalCreditoExtrato;
            conciliacaoBancaria.ValorTotalDebitoExtrato += valorTotalDebitoExtrato;
            conciliacaoBancaria.ValorTotalExtrato += (valorTotalCreditoExtrato - valorTotalDebitoExtrato);

            conciliacaoBancaria.ValorTotalCreditoMovimento += valorTotalCreditoMovimento;
            conciliacaoBancaria.ValorTotalDebitoMovimento += valorTotalDebitoMovimento;
            conciliacaoBancaria.ValorTotalMovimento += (valorTotalCreditoMovimento - valorTotalDebitoMovimento);

            conciliacaoBancaria.ValorTotalGeralDebitoMovimento = valorTotalGeralDebitoMovimento;
            conciliacaoBancaria.ValorTotalGeralCreditoMovimento = valorTotalGeralCreditoMovimento;
            conciliacaoBancaria.ValorTotalGeralMovimento = valorTotalGeralMovimento;
            conciliacaoBancaria.ValorTotalGeralDebitoExtrato = valorTotalGeralDebitoExtrato;
            conciliacaoBancaria.ValorTotalGeralCreditoExtrato = valorTotalGeralCreditoExtrato;
            conciliacaoBancaria.ValorTotalGeralExtrato = valorTotalGeralExtrato;

            repConciliacaoBancaria.Atualizar(conciliacaoBancaria);

            return true;
        }

        public static List<dynamic> ImportarExtratoBancario(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Empresa empresa, int codigoPlanoConta, StreamReader streamReader, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool apenasLeitura, out decimal saldoInicial, out decimal saldoFinal)
        {
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta = repPlanoConta.BuscarPorCodigo(codigoPlanoConta, true);

            bool layout200 = false, layout240 = false;
            int linha = 0;
            var cellValue = "";
            string numeroBanco = "";
            saldoInicial = 0m;
            saldoFinal = 0m;

            List<dynamic> dadosRetorno = new List<dynamic>();

            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            streamReader.DiscardBufferedData();

            while ((cellValue = streamReader.ReadLine()) != null)
            {
                if (linha == 0)
                {
                    if (cellValue.Length == 200)
                    {
                        layout200 = true;
                        numeroBanco = cellValue.Substring(76, 3).Trim();//Numero do Banco
                        if (numeroBanco != "237" && numeroBanco != "001")
                            throw new ServicoException("O Banco do arquivo importado não se encontra homologado, por favor verifique!");
                    }
                    else if (cellValue.Length == 240)
                    {
                        layout240 = true;
                        numeroBanco = cellValue.Substring(0, 3).Trim();//Numero do Banco
                        if (numeroBanco != "237" && numeroBanco != "104" && numeroBanco != "756" && numeroBanco != "085" && numeroBanco != "033" && numeroBanco != "422" && numeroBanco != "341" && numeroBanco != "136")
                            throw new ServicoException("O Banco do arquivo importado não se encontra homologado, por favor verifique!");
                    }
                    else
                        throw new ServicoException("O layout do arquivo não se encontra homologado!");
                }
                if (layout200)
                {
                    string posicaoLinha = cellValue.Substring(0, 1).Trim();
                    if (linha == 1)
                    {
                        string vValorSaldoInicial = cellValue.Substring(86, 18).Trim();
                        decimal valorSaldoConvertido = 0;
                        if (!string.IsNullOrWhiteSpace(vValorSaldoInicial))
                        {
                            vValorSaldoInicial = vValorSaldoInicial.TrimStart('0');
                            decimal.TryParse(vValorSaldoInicial, out valorSaldoConvertido);
                            if (valorSaldoConvertido > 0)
                                valorSaldoConvertido = valorSaldoConvertido / 100;
                        }
                        saldoInicial = valorSaldoConvertido;
                        if (planoConta.SaldoFinalConciliacaoBancaria > 0 && valorSaldoConvertido > 0 && planoConta.SaldoFinalConciliacaoBancaria != valorSaldoConvertido)
                            throw new ServicoException("O valor do saldo inicial deste arquivo não bate com o saldo final do último arquivo importado! Saldo do arquivo R$" + valorSaldoConvertido.ToString("n2") + " Saldo do plano de contas: " + planoConta.SaldoFinalConciliacaoBancaria.ToString("n2"));
                        else
                            planoConta.SaldoInicialConciliacaoBancaria = valorSaldoConvertido;
                    }
                    else if (posicaoLinha == "1" && linha > 1)
                    {
                        posicaoLinha = cellValue.Substring(104, 2).Trim();
                        bool posicaoSaldoFinal = cellValue.Substring(76, 4) == "    ";
                        if (!posicaoSaldoFinal)
                        {
                            if (numeroBanco == "237")
                                dadosRetorno.Add(ProcessarLayouBradesco(cellValue, usuario, empresa, planoConta, unitOfWork, apenasLeitura));
                            else if (numeroBanco == "001")
                                dadosRetorno.Add(ProcessarLayouBancoDoBrasil(cellValue, usuario, empresa, planoConta, unitOfWork, apenasLeitura));
                        }
                        else if (posicaoSaldoFinal && (posicaoLinha == "CP" || posicaoLinha == "CF" || posicaoLinha == "DF"))
                        {
                            string vValorSaldoFinal = "";
                            if (numeroBanco == "237")
                                vValorSaldoFinal = cellValue.Substring(106, 18).Trim();
                            else
                                vValorSaldoFinal = cellValue.Substring(86, 18).Trim();
                            decimal valorSaldoConvertido = 0;
                            if (!string.IsNullOrWhiteSpace(vValorSaldoFinal))
                            {
                                vValorSaldoFinal = vValorSaldoFinal.TrimStart('0');
                                decimal.TryParse(vValorSaldoFinal, out valorSaldoConvertido);
                                if (valorSaldoConvertido > 0)
                                    valorSaldoConvertido = valorSaldoConvertido / 100;
                            }
                            saldoFinal = valorSaldoConvertido;
                            planoConta.SaldoFinalConciliacaoBancaria = valorSaldoConvertido;
                        }
                    }
                }
                else if (layout240)
                {
                    string posicaoLinha = cellValue.Substring(13, 1).Trim();
                    if (linha == 1)
                    {
                        string vValorSaldoInicial = cellValue.Substring(150, 18).Trim();
                        decimal valorSaldoConvertido = 0;
                        if (!string.IsNullOrWhiteSpace(vValorSaldoInicial))
                        {
                            vValorSaldoInicial = vValorSaldoInicial.TrimStart('0');
                            decimal.TryParse(vValorSaldoInicial, out valorSaldoConvertido);
                            if (valorSaldoConvertido > 0)
                                valorSaldoConvertido = valorSaldoConvertido / 100;
                        }
                        saldoInicial = valorSaldoConvertido;
                        if (planoConta.SaldoFinalConciliacaoBancaria > 0 && valorSaldoConvertido > 0 && planoConta.SaldoFinalConciliacaoBancaria != valorSaldoConvertido)
                            throw new ServicoException("O valor do saldo inicial deste arquivo não bate com o saldo final do último arquivo importado! Saldo do arquivo R$" + valorSaldoConvertido.ToString("n2") + " Saldo do plano de contas: " + planoConta.SaldoFinalConciliacaoBancaria.ToString("n2"));
                        else
                            planoConta.SaldoInicialConciliacaoBancaria = valorSaldoConvertido;
                    }
                    else if (posicaoLinha == "E" && linha > 1)
                    {
                        if (numeroBanco == "136")
                            dadosRetorno.Add(ProcessarLayouUnicred(cellValue, usuario, empresa, planoConta, unitOfWork, apenasLeitura));
                        else 
                            dadosRetorno.Add(ProcessarLayou240(cellValue, usuario, empresa, planoConta, unitOfWork, apenasLeitura));
                    }
                    else if (linha > 1)
                    {                    
                        posicaoLinha = cellValue.Substring(168, 2).Trim();
                        if (posicaoLinha == "CP" || posicaoLinha == "CF" || posicaoLinha == "DF")
                        {
                            string vValorSaldoFinal = cellValue.Substring(150, 18).Trim();
                            decimal valorSaldoConvertido = 0;
                            if (!string.IsNullOrWhiteSpace(vValorSaldoFinal))
                            {
                                vValorSaldoFinal = vValorSaldoFinal.TrimStart('0');
                                decimal.TryParse(vValorSaldoFinal, out valorSaldoConvertido);
                                if (valorSaldoConvertido > 0)
                                    valorSaldoConvertido = valorSaldoConvertido / 100;
                            }
                            saldoFinal = valorSaldoConvertido;
                            planoConta.SaldoFinalConciliacaoBancaria = valorSaldoConvertido;
                        }
                    }
                }

                linha += 1;
            }

            if (apenasLeitura)
                return dadosRetorno;

            repPlanoConta.Atualizar(planoConta, auditado);
            return null;
        }

        public static List<dynamic> ImportarExtratoBancarioOFX(Dominio.Entidades.Usuario usuario, Dominio.Entidades.Empresa empresa, int codigoPlanoConta, StreamReader streamReader, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool apenasLeitura, out decimal saldoInicial, out decimal saldoFinal)
        {            
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta = repPlanoConta.BuscarPorCodigo(codigoPlanoConta, true);
         
            string numeroBanco = "";
            saldoInicial = 0m;
            saldoFinal = 0m;

            List<dynamic> dadosRetorno = new List<dynamic>();

            XmlDocument xml = AjustarArquivoOFX(streamReader.ReadToEnd());

            OFX ofx;
            using (StringReader reader = new StringReader(xml.OuterXml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OFX));
                ofx = (OFX)serializer.Deserialize(reader);
            }

            if (ofx != null)
            {
                numeroBanco = ofx.BankMessages.StatementTransactionResponse.StatementTransactions.BankAccountFrom.BankID;

                foreach (var transacao in ofx.BankMessages.StatementTransactionResponse.StatementTransactions.BankTransactionList.Transactions)
                {
                    Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);

                    DateTime date = DateTime.ParseExact(transacao.PostedDate.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture);

                    Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario extratoBancario = new Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario()
                    {
                        Colaborador = usuario,
                        DataGeracaoMovimento = DateTime.Now,
                        DataMovimento = date != null ? date : DateTime.Now.Date,
                        DebitoCredito = transacao.TransactionType == "CREDIT" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito : Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito,
                        Documento = transacao.FitID,
                        Empresa = empresa,
                        Observacao = transacao.Memo,
                        ExtratoConcolidado = false,
                        PlanoConta = planoConta,
                        TipoDocumentoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                        TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Automatica,
                        Valor = transacao.TransactionAmount,
                        CodigoLancamento = transacao.TransactionType == "CREDIT" ? "0007" : "0069",
                    };

                    if (apenasLeitura)
                        dadosRetorno.Add(extratoBancario);

                    if (!repExtratoBancario.ContemExtratoDuplicado(extratoBancario.Observacao, extratoBancario.DataMovimento, extratoBancario.Valor, extratoBancario.Documento, extratoBancario.PlanoConta.Codigo))
                        repExtratoBancario.Inserir(extratoBancario);

                }

                decimal valorSaldoConvertidoFinal = ofx.BankMessages.StatementTransactionResponse.StatementTransactions.LedgerBalance.BalanceAmount;
                saldoInicial = planoConta.SaldoFinalConciliacaoBancaria;                
                planoConta.SaldoInicialConciliacaoBancaria = planoConta.SaldoFinalConciliacaoBancaria;
                saldoFinal = valorSaldoConvertidoFinal;
                planoConta.SaldoFinalConciliacaoBancaria = valorSaldoConvertidoFinal;                
            }                       

            if (apenasLeitura)
                return dadosRetorno;

            repPlanoConta.Atualizar(planoConta, auditado);
            return null;
        }

        #endregion

        #region Métodos Privados

        private static dynamic ProcessarLayou240(string cellValue, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta, Repositorio.UnitOfWork unitOfWork, bool apenasLeitura)
        {
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);
            DateTime dataConvertida = DateTime.Now.Date;
            DateTime.TryParseExact(cellValue.Substring(142, 8).Trim(), "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);
            string vValorRetorno = cellValue.Substring(150, 18).Trim();
            decimal valorConvertido = 0;

            if (!string.IsNullOrWhiteSpace(vValorRetorno))
            {
                vValorRetorno = vValorRetorno.TrimStart('0');
                decimal.TryParse(vValorRetorno, out valorConvertido);
                if (valorConvertido > 0)
                    valorConvertido = valorConvertido / 100;
            }

            Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario extratoBancario = new Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario()
            {
                Colaborador = usuario,
                DataGeracaoMovimento = DateTime.Now,
                DataMovimento = dataConvertida,
                DebitoCredito = cellValue.Substring(168, 1).Trim() == "D" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito : Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito,
                Documento = cellValue.Substring(201, 7).Trim(),
                Empresa = empresa,
                Observacao = cellValue.Substring(176, 25).Trim(),
                ExtratoConcolidado = false,
                PlanoConta = planoConta,
                TipoDocumentoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Automatica,
                Valor = valorConvertido,
                CodigoLancamento = cellValue.Substring(172, 4).Trim(),
                ExtratoBancarioTipoLancamento = repExtratoBancarioTipoLancamento.BuscarPorCodigoIntegracao(cellValue.Substring(172, 4).Trim())
            };

            if (apenasLeitura)
                return extratoBancario;

            if (extratoBancario.ExtratoBancarioTipoLancamento == null || !extratoBancario.ExtratoBancarioTipoLancamento.NaoImportarRegistroAoEstrato)
                repExtratoBancario.Inserir(extratoBancario);

            return null;
        }

        private static dynamic ProcessarLayouBradesco(string cellValue, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta, Repositorio.UnitOfWork unitOfWork, bool apenasLeitura)
        {
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);
            DateTime dataConvertida = DateTime.Now.Date;
            DateTime.TryParseExact(cellValue.Substring(80, 6).Trim(), "ddMMyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);
            string vValorRetorno = cellValue.Substring(86, 18).Trim();
            decimal valorConvertido = 0;
            if (!string.IsNullOrWhiteSpace(vValorRetorno))
            {
                vValorRetorno = vValorRetorno.TrimStart('0');
                decimal.TryParse(vValorRetorno, out valorConvertido);
                if (valorConvertido > 0)
                    valorConvertido = valorConvertido / 100;
            }

            Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario extratoBancario = new Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario()
            {
                Colaborador = usuario,
                DataGeracaoMovimento = DateTime.Now,
                DataMovimento = dataConvertida,
                DebitoCredito = cellValue.Substring(104, 1).Trim() == "D" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito : Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito,
                Documento = cellValue.Substring(74, 6).Trim(),
                Empresa = empresa,
                Observacao = cellValue.Substring(49, 25).Trim() + " " + cellValue.Substring(105, 32).Trim(),
                ExtratoConcolidado = false,
                PlanoConta = planoConta,
                TipoDocumentoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Automatica,
                Valor = valorConvertido,
                CodigoLancamento = cellValue.Substring(45, 4).Trim(),
                ExtratoBancarioTipoLancamento = repExtratoBancarioTipoLancamento.BuscarPorCodigoIntegracao(cellValue.Substring(45, 4).Trim())
            };

            if (apenasLeitura)
                return extratoBancario;

            if (extratoBancario.ExtratoBancarioTipoLancamento == null || !extratoBancario.ExtratoBancarioTipoLancamento.NaoImportarRegistroAoEstrato)
                repExtratoBancario.Inserir(extratoBancario);

            return null;
        }

        private static dynamic ProcessarLayouBancoDoBrasil(string cellValue, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta, Repositorio.UnitOfWork unitOfWork, bool apenasLeitura)
        {
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);
            DateTime dataConvertida = DateTime.Now.Date;
            DateTime.TryParseExact(cellValue.Substring(80, 6).Trim(), "ddMMyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);
            string vValorRetorno = cellValue.Substring(86, 18).Trim();
            decimal valorConvertido = 0;
            if (!string.IsNullOrWhiteSpace(vValorRetorno))
            {
                vValorRetorno = vValorRetorno.TrimStart('0');
                decimal.TryParse(vValorRetorno, out valorConvertido);
                if (valorConvertido > 0)
                    valorConvertido = valorConvertido / 100;
            }

            Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario extratoBancario = new Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario()
            {
                Colaborador = usuario,
                DataGeracaoMovimento = DateTime.Now,
                DataMovimento = dataConvertida,
                DebitoCredito = RetornarDebitoCreditoBancoBrasil(cellValue.Substring(42, 3).Trim()),
                Documento = cellValue.Substring(74, 6).Trim(),
                Empresa = empresa,
                Observacao = cellValue.Substring(49, 25).Trim(),
                ExtratoConcolidado = false,
                PlanoConta = planoConta,
                TipoDocumentoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Automatica,
                Valor = valorConvertido,
                CodigoLancamento = cellValue.Substring(45, 4).Trim(),
                ExtratoBancarioTipoLancamento = repExtratoBancarioTipoLancamento.BuscarPorCodigoIntegracao(cellValue.Substring(45, 4).Trim())
            };

            if (apenasLeitura)
                return extratoBancario;

            if (extratoBancario.ExtratoBancarioTipoLancamento == null || !extratoBancario.ExtratoBancarioTipoLancamento.NaoImportarRegistroAoEstrato)
                repExtratoBancario.Inserir(extratoBancario);

            return null;
        }

        private static dynamic ProcessarLayouUnicred(string cellValue, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoConta, Repositorio.UnitOfWork unitOfWork, bool apenasLeitura)
        {
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario repExtratoBancario = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancario(unitOfWork);
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);
            DateTime dataConvertida = DateTime.Now.Date;
            DateTime.TryParseExact(cellValue.Substring(142, 8).Trim(), "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out dataConvertida);
            string vValorRetorno = cellValue.Substring(150, 18).Trim();
            decimal valorConvertido = 0;

            if (!string.IsNullOrWhiteSpace(vValorRetorno))
            {
                vValorRetorno = vValorRetorno.TrimStart('0');
                decimal.TryParse(vValorRetorno, out valorConvertido);
                if (valorConvertido > 0)
                    valorConvertido = valorConvertido / 100;
            }

            Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario extratoBancario = new Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario()
            {
                Colaborador = usuario,
                DataGeracaoMovimento = DateTime.Now,
                DataMovimento = dataConvertida,
                DebitoCredito = cellValue.Substring(168, 1).Trim() == "D" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito : Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito,
                Documento = cellValue.Substring(201, 39).Trim(),
                Empresa = empresa,
                Observacao = cellValue.Substring(176, 25).Trim(),
                ExtratoConcolidado = false,
                PlanoConta = planoConta,
                TipoDocumentoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                TipoGeracaoMovimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento.Automatica,
                Valor = valorConvertido,
                CodigoLancamento = cellValue.Substring(172, 4).Trim(),
                ExtratoBancarioTipoLancamento = repExtratoBancarioTipoLancamento.BuscarPorCodigoIntegracao(cellValue.Substring(172, 4).Trim())
            };

            if (apenasLeitura)
                return extratoBancario;

            if (extratoBancario.ExtratoBancarioTipoLancamento == null || !extratoBancario.ExtratoBancarioTipoLancamento.NaoImportarRegistroAoEstrato)
                repExtratoBancario.Inserir(extratoBancario);

            return null;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito RetornarDebitoCreditoBancoBrasil(string codigoLancamento)
        {
            int.TryParse(codigoLancamento, out int codLancamento);
            if (codLancamento > 200)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito;
            else
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito;
        }

        private static XmlDocument AjustarArquivoOFX(string arquivoofx)
        {
            var xmlPattern = new Regex(@"<OFX[\s\S]*?</OFX>", RegexOptions.Compiled);
            string arquivo = xmlPattern.Match(arquivoofx).ToString();

            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.LoadXml(arquivo.ToString());
            }
            catch (XmlException ex)
            {
                string[] linhas = arquivo.Split(new[] { '\n' }, StringSplitOptions.None);

                string xml = "";

                foreach (var l in linhas)
                {
                    string linha = l.TrimStart();

                    if (linha.Substring(0, 2) != @"</")
                    {
                        string tagName = linha.Substring(linha.ToString().IndexOf('<') + 1, linha.ToString().IndexOf('>') - 1);

                        if (!arquivo.Contains($"</{tagName}>"))
                        {
                            xml += linha.TrimEnd() + $"</{tagName}>";
                        }
                        else
                        {
                            xml += linha;
                        }
                    }
                    else
                    {
                        xml += linha;
                    }
                }

                xmlDoc.LoadXml(xml);
            }

            return xmlDoc;
        }

        #endregion
    }
}
