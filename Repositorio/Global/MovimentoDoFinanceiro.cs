using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class MovimentoDoFinanceiro : RepositorioBase<Dominio.Entidades.MovimentoDoFinanceiro>, Dominio.Interfaces.Repositorios.MovimentoDoFinanceiro
    {
        public MovimentoDoFinanceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorParcelaCobrancaCTe(int codigoEmpresa, int codigoParcelaCobranca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.ParcelasCTe.Contains(new Dominio.Entidades.ParcelaCobrancaCTe() { Codigo = codigoParcelaCobranca }) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.MovimentoDoFinanceiro> BuscarPorCIOT(int codigoEmpresa, int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.CIOTs.Contains(new Dominio.Entidades.CIOTSigaFacil() { Codigo = codigoCIOT }) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.MovimentoDoFinanceiro> BuscarPorPagamentoMotoristaCTe(int codigoEmpresa, int codigoPagamentoMotoristaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.PagamentosMotoristasCTe.Contains(new Dominio.Entidades.PagamentoMotorista() { Codigo = codigoPagamentoMotoristaCTe }) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.MovimentoDoFinanceiro> BuscarPorPagamentoMotoristaMDFe(int codigoEmpresa, int codigoPagamentoMotoristaMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.PagamentosMotoristasMDFe.Contains(new Dominio.Entidades.PagamentoMotoristaMDFe() { Codigo = codigoPagamentoMotoristaMDFe }) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorAbastecimento(int codigoEmpresa, int codigoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Abastecimentos.Contains(new Dominio.Entidades.Abastecimento() { Codigo = codigoAbastecimento }) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorDuplicataDocumentoEntrada(int codigoEmpresa, int codigoDuplicataDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.ParcelaDocumentoEntrada.Contains(new Dominio.Entidades.ParcelaDocumentoEntrada() { Codigo = codigoDuplicataDocumentoEntrada }) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorDuplicata(int codigoDuplicata)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();
            var result = from obj in query where obj.Duplicata.Contains(new Dominio.Entidades.Duplicata() { Codigo = codigoDuplicata }) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorDuplicataDesconto(int codigoDuplicata)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();
            var result = from obj in query where obj.DuplicataDesconto.Contains(new Dominio.Entidades.Duplicata() { Codigo = codigoDuplicata }) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorDuplicataAcrescimo(int codigoDuplicata)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();
            var result = from obj in query where obj.DuplicataAcrescimo.Contains(new Dominio.Entidades.Duplicata() { Codigo = codigoDuplicata }) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorBaixaCTeDuplicata(int codigoDuplicata)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();
            var result = from obj in query where obj.DuplicataBaixaCte.Contains(new Dominio.Entidades.Duplicata() { Codigo = codigoDuplicata }) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorDuplicataParcela(int codigoEmpresa, int codigoParcela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.DuplicataParcelas.Contains(new Dominio.Entidades.DuplicataParcelas() { Codigo = codigoParcela }) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorDocumento(string documento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();
            var result = from obj in query where obj.Documento.Equals(documento) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorDespesa(int codigoEmpresa, int codigoDespesa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Despesas.Contains(new Dominio.Entidades.DespesaDoAcertoDeViagem() { Codigo = codigoDespesa }) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MovimentoDoFinanceiro BuscarPorHistoricoDeVeiculo(int codigoEmpresa, int codigoHistoricoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.HistoricosVeiculos.Contains(new Dominio.Entidades.HistoricoVeiculo() { Codigo = codigoHistoricoVeiculo }) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.MovimentoDoFinanceiro> Consultar(int codigoEmpresa, decimal valorInicial, decimal valorFinal, DateTime dataInicial, DateTime dataFinal, string documento, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Abastecimentos.Count() == 0 && obj.ParcelasCTe.Count() == 0 && obj.HistoricosVeiculos.Count() == 0 select obj;

            if (valorInicial > 0)
                result = result.Where(o => o.Valor >= valorInicial);

            if (valorFinal > 0)
                result = result.Where(o => o.Valor <= valorFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.Data >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.Data < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(documento))
                result = result.Where(o => o.Documento.Equals(documento));

            return result.OrderByDescending(o => o.Data).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, decimal valorInicial, decimal valorFinal, DateTime dataInicial, DateTime dataFinal, string documento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Abastecimentos.Count() == 0 && obj.ParcelasCTe.Count() == 0 && obj.HistoricosVeiculos.Count() == 0 select obj;

            if (valorInicial > 0)
                result = result.Where(o => o.Valor >= valorInicial);

            if (valorFinal > 0)
                result = result.Where(o => o.Valor <= valorFinal);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.Data >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.Data < dataFinal.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(documento))
                result = result.Where(o => o.Documento.Equals(documento));

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioMovimentoDoFinanceiroAgrupado> ObterMovimentosAgrupadosPorPeriodo(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoVeiculo, int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();

            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa && obj.Data >= dataInicial.Date && obj.Data < dataFinal.AddDays(1).Date
                         select obj;

            //Adicionado filtro para exibir apenas as contas Débitos e Créditos com movimento do tipo Entrada
            //result = result.Where(o => o.PlanoDeConta.TipoDeConta != "O" && o.Tipo == Dominio.Enumeradores.TipoMovimento.Entrada); 
            //Alterado 18/05/2018 #1427 para exibir apenas contas configuradas para exibir
            result = result.Where(o => o.PlanoDeConta.NaoExibirDRE == false);

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            var agrupamento = result.GroupBy(obj => new
            {
                obj.Data.Year,
                obj.Data.Month,
                CodigoPlano = obj.PlanoDeConta.Codigo,
                Conta = obj.PlanoDeConta.Conta,
                DescricaoPlano = obj.PlanoDeConta.Descricao,
                TipoPlano = obj.PlanoDeConta.Tipo,
                TipoConta = obj.PlanoDeConta.TipoDeConta
            }).Select(obj => new Dominio.ObjetosDeValor.Relatorios.RelatorioMovimentoDoFinanceiroAgrupado()
            {
                Mes = obj.Key.Month,
                Ano = obj.Key.Year,
                CodigoPlano = obj.Key.CodigoPlano,
                Conta = obj.Key.Conta,
                DescricaoPlano = obj.Key.DescricaoPlano,
                TipoPlano = obj.Key.TipoPlano,
                TipoConta = obj.Key.TipoConta,
                ValorTotal = obj.Sum(o => o.Valor)
            });

            return agrupamento.ToList();
        }

        public List<Dominio.Entidades.MovimentoDoFinanceiro> BuscarExtratoContas(int codigoEmpresa, int codigoPlanoConta, double cpfCnpjPessoa, DateTime dataInicialLancamento, DateTime dataFinalLancamento, DateTime dataInicialPagamento, DateTime dataFinalPagamento, DateTime dataInicialBaixa, DateTime dataFinalBaixa, string situacaoMovimentos, string tipoPlanoConta, int codigoMotorista, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MovimentoDoFinanceiro>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (codigoPlanoConta > 0)
                result = result.Where(o => o.PlanoDeConta.Codigo == codigoPlanoConta);

            if (cpfCnpjPessoa > 0f)
                result = result.Where(o => o.Pessoa.CPF_CNPJ == cpfCnpjPessoa);

            if (dataInicialLancamento != DateTime.MinValue)
                result = result.Where(o => o.Data >= dataInicialLancamento.Date);

            if (dataFinalLancamento != DateTime.MinValue)
                result = result.Where(o => o.Data < dataFinalLancamento.AddDays(1).Date);

            if (dataInicialPagamento != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento >= dataInicialPagamento.Date);

            if (dataFinalPagamento != DateTime.MinValue)
                result = result.Where(o => o.DataPagamento < dataFinalPagamento.AddDays(1).Date);

            if (dataInicialBaixa != DateTime.MinValue)
                result = result.Where(o => o.DataBaixa >= dataInicialBaixa.Date);

            if (dataFinalBaixa != DateTime.MinValue)
                result = result.Where(o => o.DataBaixa < dataFinalBaixa.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(tipoPlanoConta))
                result = result.Where(o => o.PlanoDeConta.TipoDeConta.Equals(tipoPlanoConta));

            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (!string.IsNullOrWhiteSpace(situacaoMovimentos))
            {
                if (situacaoMovimentos == "A")
                    result = result.Where(o => o.DataBaixa == null);
                else if (situacaoMovimentos == "P")
                    result = result.Where(o => o.DataPagamento != null);
                else if (situacaoMovimentos == "B")
                    result = result.Where(o => o.DataBaixa != null);
            }

            return result.Fetch(o => o.Pessoa)
                         .Fetch(o => o.PlanoDeConta)
                         .ToList();
        }
    }
}
