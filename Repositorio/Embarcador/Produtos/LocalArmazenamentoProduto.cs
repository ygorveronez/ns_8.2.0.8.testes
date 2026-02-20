using Dominio.Relatorios.Embarcador.DataSource.Frota;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class LocalArmazenamentoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto>
    {
        public LocalArmazenamentoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto BuscarPrimeiroPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto>()
                .Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto> Consultar(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaLocalArmazenamentoProduto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaLocalArmazenamentoProduto filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoTanques> ConsultaMovimentacaoTanques(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMovimentacaoTanques filtrosPesquisa)
        {
            DateTime? dataInicial = filtrosPesquisa.DataInicial?.Date != DateTime.MinValue ? filtrosPesquisa.DataInicial?.Date : null;
            DateTime? dataFinal = filtrosPesquisa.DataFinal?.Date != DateTime.MinValue ? filtrosPesquisa.DataFinal?.Date.AddDays(1).AddTicks(-1) : null;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto>()
                .Where(local => local.TipoOleo != null)
                .Select(local => new MovimentacaoTanques
                {
                    LocalArmazenamento = local.Descricao,
                    CodigoLocalArmazenamento = local.Codigo,
                    SaldoInicialTanque = ObterSaldoInicialTanque(local.Codigo, dataInicial, dataFinal),
                    ValorEntradaMovimentacao = ObterValorEntradaMovimentacao(local.Codigo, dataInicial, dataFinal),
                    ValorSaidaMovimentacao = ObterValorSaidaMovimentacao(local.Codigo, dataInicial, dataFinal),
                    SaldoAtualTanque = local.SaldoDoTanque
                });

            var result = query.ToList();

            // Aplicar filtro sobre o tanque escolhido
            if (filtrosPesquisa.LocalArmazenamento.HasValue && filtrosPesquisa.LocalArmazenamento.Value > 0)
            {
                result = result.Where(x => x.CodigoLocalArmazenamento == filtrosPesquisa.LocalArmazenamento).ToList();
            }

            return result;
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoTanques> ConsultaMovimentacaoTanquesDetalhes(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMovimentacaoTanques filtrosPesquisa)
        {
            DateTime? dataInicial = filtrosPesquisa.DataInicial?.Date != DateTime.MinValue ? filtrosPesquisa.DataInicial?.Date : null;
            DateTime? dataFinal = filtrosPesquisa.DataFinal?.Date != DateTime.MinValue ? filtrosPesquisa.DataFinal?.Date.AddDays(1).AddTicks(-1) : null;

            var local = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto>()
                .Where(local => local.Codigo == filtrosPesquisa.LocalArmazenamento)
                .Select(local => new
                {
                    local.Codigo,
                    local.SaldoDoTanque
                }).FirstOrDefault();

            if (local == null)
            {
                return new List<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoTanques>();
            }

            var entradas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque>()
                .Where(entrada => entrada.LocalArmazenamentoProduto.Codigo == local.Codigo
                                  && (dataInicial == null || entrada.DataHoraEntrada >= dataInicial)
                                  && (dataFinal == null || entrada.DataHoraEntrada <= dataFinal))
                .Select(entrada => new Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoTanques
                {
                    DataExibirDetalhes = entrada.DataHoraEntrada,
                    SaldoInicialTanque = entrada.SaldoInicialAntesMovimentacaoEntrada ?? 0,
                    ValorEntradaMovimentacaoDetalhes = entrada.QuantidadeLitros,
                    ValorSaidaMovimentacaoDetalhes = 0M,
                    SaldoAtualTanque = local.SaldoDoTanque ?? null
                }).ToList();

            var saidas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida>()
                .Where(saida => saida.LocalArmazenamentoProduto.Codigo == local.Codigo
                                && (dataInicial == null || saida.Data >= dataInicial)
                                && (dataFinal == null || saida.Data <= dataFinal))
                .Select(saida => new Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoTanques
                {
                    DataExibirDetalhes = saida.Data,
                    SaldoInicialTanque = saida.SaldoInicialAntesMovimentacaoSaida ?? 0,
                    ValorEntradaMovimentacaoDetalhes = 0M,
                    ValorSaidaMovimentacaoDetalhes = saida.QuantidadeLitros,
                    SaldoAtualTanque = local.SaldoDoTanque ?? null
                }).ToList();

            var movimentacoes = entradas
                .Concat(saidas)
                .OrderByDescending(movimentacao => movimentacao.DataExibirDetalhes)
                .ToList();

            return movimentacoes;
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto> Consultar(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaLocalArmazenamentoProduto filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.TipoOleo > 0)
                result = result.Where(o => o.TipoOleo.Codigo == filtrosPesquisa.TipoOleo);

            return result;
        }

        private decimal ObterSaldoInicialTanque(int codigoLocal, DateTime? dataInicial, DateTime? dataFinal)
        {
            var saldoInicialEntrada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque>()
                .Where(entrada => entrada.LocalArmazenamentoProduto.Codigo == codigoLocal
                                  && (dataInicial == null || entrada.DataHoraEntrada >= dataInicial)
                                  && (dataFinal == null || entrada.DataHoraEntrada <= dataFinal))
                .OrderBy(entrada => entrada.DataHoraEntrada)
                .ThenBy(entrada => entrada.SaldoInicialAntesMovimentacaoEntrada)
                .Select(entrada => new { entrada.SaldoInicialAntesMovimentacaoEntrada, entrada.DataHoraEntrada })
                .FirstOrDefault();

            var saldoInicialSaida = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida>()
                .Where(saida => saida.LocalArmazenamentoProduto.Codigo == codigoLocal
                                && (dataInicial == null || saida.Data >= dataInicial)
                                && (dataFinal == null || saida.Data <= dataFinal))
                .OrderBy(saida => saida.Data)
                .ThenBy(saida => saida.SaldoInicialAntesMovimentacaoSaida)
                .Select(saida => new { saida.SaldoInicialAntesMovimentacaoSaida, saida.Data })
            .FirstOrDefault();

            if (saldoInicialEntrada != null && (saldoInicialSaida == null || saldoInicialEntrada.DataHoraEntrada <= saldoInicialSaida.Data))
            {
                return saldoInicialEntrada.SaldoInicialAntesMovimentacaoEntrada ?? 0;
            }
            else if (saldoInicialSaida != null)
            {
                return saldoInicialSaida.SaldoInicialAntesMovimentacaoSaida ?? 0;
            }

            return 0;
        }

        private decimal ObterValorEntradaMovimentacao(int codigoLocal, DateTime? dataInicial, DateTime? dataFinal)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentoEntradaTanque>()
                .Where(entrada => entrada.LocalArmazenamentoProduto.Codigo == codigoLocal
                                  && (dataInicial == null || entrada.DataHoraEntrada >= dataInicial)
                                  && (dataFinal == null || entrada.DataHoraEntrada <= dataFinal))
                .Sum(entrada => (decimal?)entrada.QuantidadeLitros) ?? 0;
        }

        private decimal ObterValorSaidaMovimentacao(int codigoLocal, DateTime? dataInicial, DateTime? dataFinal)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.AbastecimentoInterno.MovimentacaoAbastecimentoSaida>()
                .Where(saida => saida.LocalArmazenamentoProduto.Codigo == codigoLocal
                                && (dataInicial == null || saida.Data >= dataInicial)
                                && (dataFinal == null || saida.Data <= dataFinal))
                .Sum(saida => (decimal?)saida.QuantidadeLitros) ?? 0;
        }

        #endregion
    }
}
