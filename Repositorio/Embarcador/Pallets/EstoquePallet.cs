using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pallets
{
    public class EstoquePallet : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>
    {
        #region Construtores

        public EstoquePallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> ConsultarCliente(double cpfCnpjCliente, DateTime? dataInicial, DateTime? dataFinal, int codigoGrupoPessoas)
        {
            var movimentacoesEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>()
                .Where(o => o.NaturezaMovimentacao == NaturezaMovimentacaoEstoquePallet.Cliente);

            if (cpfCnpjCliente > 0d)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Cliente.CPF_CNPJ == cpfCnpjCliente);

            if (dataInicial.HasValue)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Data >= dataInicial.Value.Date);

            if (dataFinal.HasValue)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Data < dataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (codigoGrupoPessoas > 0)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoas);

            return movimentacoesEstoque;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> ConsultarFilial(List<int> codigosFilial, DateTime? dataInicial, DateTime? dataFinal)
        {
            var movimentacoesEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>()
                .Where(o => o.NaturezaMovimentacao == NaturezaMovimentacaoEstoquePallet.Filial);

            if (codigosFilial?.Count() > 0)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => codigosFilial.Contains(o.Filial.Codigo));

            if (dataInicial.HasValue)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Data >= dataInicial.Value.Date);

            if (dataFinal.HasValue)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Data < dataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return movimentacoesEstoque;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> ConsultarTransportador(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo filtrosPesquisa)
        {
            var movimentacoesEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>()
                .Where(o => o.NaturezaMovimentacao == NaturezaMovimentacaoEstoquePallet.Transportador);

            if (filtrosPesquisa.CodigosTransportador?.Count() > 0)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => filtrosPesquisa.CodigosTransportador.Contains(o.Transportador.Codigo));

            if (filtrosPesquisa.NumeroDocumento > 0)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Devolucao.NumeroDevolucao == filtrosPesquisa.NumeroDocumento);

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoCargaEmbarcador))
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Devolucao.CargaPedido.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador);

            if (filtrosPesquisa.CodigoFilial > 0)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.DataMovimentoInicial.HasValue)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Data >= filtrosPesquisa.DataMovimentoInicial.Value.Date);

            if (filtrosPesquisa.DataMovimentoFinal.HasValue)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Data < filtrosPesquisa.DataMovimentoFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.TipoMovimento == TipoMovimentacaoEstoquePallet.Entrada)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.TipoMovimentacao == filtrosPesquisa.TipoMovimento);
            else if (filtrosPesquisa.TipoMovimento == TipoMovimentacaoEstoquePallet.Saida)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.TipoMovimentacao == filtrosPesquisa.TipoMovimento);

            return movimentacoesEstoque;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> ConsultarRelatorioControleEntradaSaida(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleEntradaSaidaPallet filtrosPesquisa)
        {
            var movimentacoesEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>();

            if (filtrosPesquisa.ListaCodigoFilial?.Count > 0)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => filtrosPesquisa.ListaCodigoFilial.Contains(o.Filial.Codigo));

            if (filtrosPesquisa.ListaCodigoTransportador?.Count > 0)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => filtrosPesquisa.ListaCodigoTransportador.Contains(o.Transportador.Codigo));

            if (filtrosPesquisa.ListaCpfCnpjCliente?.Count > 0)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => filtrosPesquisa.ListaCpfCnpjCliente.Contains(o.Cliente.CPF_CNPJ));

            if (filtrosPesquisa.NaturezaMovimentacao.HasValue)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.NaturezaMovimentacao == filtrosPesquisa.NaturezaMovimentacao);

            if (filtrosPesquisa.DataInicio.HasValue)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Data >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                movimentacoesEstoque = movimentacoesEstoque.Where(o => o.Data < filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return movimentacoesEstoque;
        }

        private string ObterSelectConsultaRelatorioFechamento(List<int> codigosTransportador, DateTime dataInicial, DateTime dataFinal)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();

            sql.Append("select Empresa.EMP_CNPJ as CNPJTransportador, Empresa.EMP_RAZAO as Transportador, ");
            sql.Append("       Empresa.EMP_CODIGO_INTEGRACAO as TransportadorCodigoIntegracao, Empresa.EMP_DIAS_ROTATIVIDADE_PALLETS as DiasDeRotatividade, ");
            sql.Append("       ( ");
            sql.Append("           select sum(EstoqueEntrada.PES_QUANTIDADE) ");
            sql.Append("             from T_PALLET_ESTOQUE as EstoqueEntrada ");
            sql.Append("            where EstoqueEntrada.EMP_CODIGO = EstoquePallet.EMP_CODIGO ");
            sql.Append($"             and EstoqueEntrada.PES_TIPO_MOVIMENTACAO = {(int)TipoMovimentacaoEstoquePallet.Entrada} ");
            sql.Append($"             and EstoqueEntrada.PES_NATUREZA_MOVIMENTACAO = {(int)NaturezaMovimentacaoEstoquePallet.Transportador} ");

            if (dataInicial != DateTime.MinValue)
                sql.Append($"         and EstoqueEntrada.pes_data >= '{dataInicial.ToString("yyyy-MM-dd")}' ");

            if (dataFinal != DateTime.MinValue)
                sql.Append($"         and EstoqueEntrada.pes_data < '{dataFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");

            sql.Append("       ) as TotalEntradas, ");
            sql.Append("       ( ");
            sql.Append("           select sum(EstoqueSaida.PES_QUANTIDADE) ");
            sql.Append("             from T_PALLET_ESTOQUE as EstoqueSaida ");
            sql.Append("            where EstoqueSaida.EMP_CODIGO = EstoquePallet.EMP_CODIGO ");
            sql.Append($"             and EstoqueSaida.PES_TIPO_MOVIMENTACAO = {(int)TipoMovimentacaoEstoquePallet.Saida} ");
            sql.Append($"             and EstoqueSaida.PES_NATUREZA_MOVIMENTACAO = {(int)NaturezaMovimentacaoEstoquePallet.Transportador} ");

            if (dataInicial != DateTime.MinValue)
                sql.Append($"         and EstoqueSaida.pes_data >= '{dataInicial.ToString("yyyy-MM-dd")}' ");

            if (dataFinal != DateTime.MinValue)
                sql.Append($"         and EstoqueSaida.pes_data < '{dataFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");

            sql.Append("       ) as TotalSaidas, ");
            sql.Append("       ( ");
            sql.Append("           select top 1 (EstoqueSaldo.pes_saldo_total) ");
            sql.Append("             from T_PALLET_ESTOQUE as EstoqueSaldo ");
            sql.Append("            where EstoqueSaldo.EMP_CODIGO = EstoquePallet.EMP_CODIGO ");
            sql.Append($"             and EstoqueSaldo.PES_NATUREZA_MOVIMENTACAO = {(int)NaturezaMovimentacaoEstoquePallet.Transportador} ");

            if (dataInicial != DateTime.MinValue)
                sql.Append($"         and EstoqueSaldo.pes_data >= '{dataInicial.ToString("yyyy-MM-dd")}' ");

            if (dataFinal != DateTime.MinValue)
                sql.Append($"         and EstoqueSaldo.pes_data < '{dataFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");

            sql.Append("            order by EstoqueSaldo.pes_data desc ");
            sql.Append("       ) as SaldoTotal, ");
            sql.Append("       ( ");
            sql.Append("           select sum(EstoqueSaida.PES_QUANTIDADE) ");
            sql.Append("             from T_PALLET_ESTOQUE as EstoqueSaida ");
            sql.Append("             join T_PALLET_DEVOLUCAO as DevolucaoPallet on DevolucaoPallet.PDE_CODIGO = EstoqueSaida.PDE_CODIGO ");
            sql.Append("            where EstoqueSaida.EMP_CODIGO = EstoquePallet.EMP_CODIGO ");
            sql.Append($"             and EstoqueSaida.PES_TIPO_MOVIMENTACAO = {(int)TipoMovimentacaoEstoquePallet.Entrada} ");
            sql.Append($"             and EstoqueSaida.PES_NATUREZA_MOVIMENTACAO = {(int)NaturezaMovimentacaoEstoquePallet.Transportador} ");
            sql.Append("              and EstoqueSaida.pes_data >= dateadd(day, - Empresa.EMP_DIAS_ROTATIVIDADE_PALLETS, getdate()) ");
            sql.Append($"             and DevolucaoPallet.PDE_SITUACAO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega} ");

            if (dataFinal != DateTime.MinValue)
                sql.Append($"         and EstoqueSaida.pes_data < '{dataFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");

            sql.Append("       ) as TotalEmRotatividade ");
            sql.Append("  from T_PALLET_ESTOQUE EstoquePallet ");
            sql.Append("  join T_EMPRESA as Empresa on Empresa.EMP_CODIGO = EstoquePallet.EMP_CODIGO ");
            sql.Append(" where 1 = 1 ");

            if (dataInicial != DateTime.MinValue)
                sql.Append($"and EstoquePallet.pes_data >= '{dataInicial.ToString("yyyy-MM-dd")}' ");

            if (dataFinal != DateTime.MinValue)
                sql.Append($"and EstoquePallet.pes_data < '{dataFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");

            if (codigosTransportador?.Count > 0)
                sql.Append($" and Empresa.EMP_CODIGO in ({string.Join(", ", codigosTransportador)}) ");
            else
                sql.Append("  and Empresa.EMP_CODIGO is not null ");

            sql.Append(" group by Empresa.EMP_CNPJ, Empresa.EMP_RAZAO, Empresa.EMP_CODIGO_INTEGRACAO, EstoquePallet.EMP_CODIGO, Empresa.EMP_DIAS_ROTATIVIDADE_PALLETS ");

            return sql.ToString();
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pallets.EstoquePallet BuscarPorCodigo(int codigo)
        {
            var estoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return estoque;
        }

        public List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> BuscarPorData(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>();

            var result = from obj in query
                         where
                             obj.Fechamento == null
                             && obj.Data.Date >= dataInicial
                             && obj.Data.Date <= dataFinal
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> BuscarPorFechamento(int fechamento, bool? adicionado = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>();

            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (adicionado.HasValue)
                result = result.Where(o => o.AdicionarAoFechamento == adicionado.Value);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> ConsultaPorFechamento(int fechamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorFechamento(int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> ConsultarCliente(double cpfCnpjCliente, DateTime? dataInicial, DateTime? dataFinal, int codigoGrupoPessoas, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var movimentacoesEstoque = ConsultarCliente(cpfCnpjCliente, dataInicial, dataFinal, codigoGrupoPessoas);

            return ObterLista(movimentacoesEstoque, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> ConsultarFilial(List<int> codigosFilial, DateTime? dataInicial, DateTime? dataFinal, string propriedadeAgrupar, string propriedadeOrdenar, string direcaoOrdenacao, int registroInicial, int maximoRegistros)
        {
            var movimentacoesEstoque = ConsultarFilial(codigosFilial, dataInicial, dataFinal);

            if (!string.IsNullOrWhiteSpace(propriedadeAgrupar))
                movimentacoesEstoque = movimentacoesEstoque.OrderBy(propriedadeAgrupar + (direcaoOrdenacao == "asc" ? " ascending" : " descending"));

            if (propriedadeAgrupar != propriedadeOrdenar)
                movimentacoesEstoque = movimentacoesEstoque.OrderBy(propriedadeOrdenar + (direcaoOrdenacao == "asc" ? " ascending" : " descending"));

            if (registroInicial > 0)
                movimentacoesEstoque = movimentacoesEstoque.Skip(registroInicial);

            if (maximoRegistros > 0)
                movimentacoesEstoque = movimentacoesEstoque.Take(maximoRegistros);

            return movimentacoesEstoque.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pallets.FechamentoTransportador> ConsultarRelatario(List<int> codigosTransportador, DateTime dataInicial, DateTime dataFinal, string propOrdenar, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioFechamento(codigosTransportador, dataInicial, dataFinal);

            sql += " order by " + propOrdenar + " " + dirOrdena;

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pallets.FechamentoTransportador)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Pallets.FechamentoTransportador>();

        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pallets.FechamentoTransportador> ConsultarRelatario(List<int> codigosTransportador, DateTime dataInicial, DateTime dataFinal, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioFechamento(codigosTransportador, dataInicial, dataFinal);

            sql += " order by " + propOrdenar + " " + dirOrdena;
            sql += " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pallets.FechamentoTransportador)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Pallets.FechamentoTransportador>();

        }

        public List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> ConsultarRelatorioControleEntradaSaida(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleEntradaSaidaPallet filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int registroInicial, int maximoRegistros)
        {
            var movimentacoesEstoque = ConsultarRelatorioControleEntradaSaida(filtrosPesquisa);

            return ObterLista(movimentacoesEstoque, propriedadeOrdenar, direcaoOrdenacao, registroInicial, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> ConsultarTransportador(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int registroInicial, int maximoRegistros)
        {
            var movimentacoesEstoque = ConsultarTransportador(filtrosPesquisa);

            return ObterLista(movimentacoesEstoque, propriedadeOrdenar, direcaoOrdenacao, registroInicial, maximoRegistros);
        }

        public int ContarConsultaCliente(double cpfCnpjCliente, DateTime? dataInicial, DateTime? dataFinal, int codigoGrupoPessoas)
        {
            var movimentacoesEstoque = ConsultarCliente(cpfCnpjCliente, dataInicial, dataFinal, codigoGrupoPessoas);

            return movimentacoesEstoque.Count();
        }

        public int ContarConsultaFilial(List<int> codigosFilial, DateTime? dataInicial, DateTime? dataFinal)
        {
            var movimentacoesEstoque = ConsultarFilial(codigosFilial, dataInicial, dataFinal);

            return movimentacoesEstoque.Count();
        }

        public int ContarConsultaRelatorio(List<int> codigosTransportador, DateTime dataInicial, DateTime dataFinal)
        {
            string select = "select distinct(count(0) over ()) from T_PALLET_ESTOQUE EstoquePallet ";
            select += " inner join T_EMPRESA as Empresa on Empresa.EMP_CODIGO = EstoquePallet.EMP_CODIGO ";
            select += " where 1=1 ";

            if (dataInicial != DateTime.MinValue)
                select += " and EstoquePallet.pes_data >= '" + dataInicial.ToString("yyyy-MM-dd") + "' ";
            if (dataFinal != DateTime.MinValue)
                select += " and EstoquePallet.pes_data < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (codigosTransportador?.Count > 0)
                select += $" and Empresa.EMP_CODIGO in ({string.Join(", ", codigosTransportador)}) ";
            else
                select += " and Empresa.EMP_CODIGO is not null ";

            select += " group by Empresa.EMP_CNPJ, Empresa.EMP_RAZAO, EstoquePallet.EMP_CODIGO, Empresa.EMP_DIAS_ROTATIVIDADE_PALLETS ";

            var query = this.SessionNHiBernate.CreateSQLQuery(select);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public int ContarConsultaRelatorioControleEntradaSaida(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleEntradaSaidaPallet filtrosPesquisa)
        {
            var movimentacoesEstoque = ConsultarRelatorioControleEntradaSaida(filtrosPesquisa);

            return movimentacoesEstoque.Count();
        }

        public int ContarConsultaTransportador(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo filtrosPesquisa)
        {
            var movimentacoesEstoque = ConsultarTransportador(filtrosPesquisa);

            return movimentacoesEstoque.Count();
        }

        public int ObterSaldoAvariaPorFilial(int codigoSituacaoDevolucaoPallet, int codigoFilial)
        {
            var movimentacoesEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>()
                .Where(o => (
                    (o.NaturezaMovimentacao == NaturezaMovimentacaoEstoquePallet.Avaria) &&
                    (o.SituacaoDevolucaoPallet.Codigo == codigoSituacaoDevolucaoPallet) &&
                    (o.Filial.Codigo == codigoFilial)
                ));

            var saldoTotalEntrada = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada select (int?)movimentacao.Quantidade).Sum() ?? 0;
            var saldoTotalSaida = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Saida select (int?)movimentacao.Quantidade).Sum() ?? 0;

            return (saldoTotalEntrada - saldoTotalSaida);
        }

        public int ObterSaldoCliente(double cpfCnpjCliente)
        {
            var movimentacoesEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>().Where(o => (o.NaturezaMovimentacao == NaturezaMovimentacaoEstoquePallet.Cliente) && (o.Cliente.CPF_CNPJ == cpfCnpjCliente));
            var saldoTotalEntrada = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada select (int?)movimentacao.Quantidade).Sum() ?? 0;
            var saldoTotalSaida = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Saida select (int?)movimentacao.Quantidade).Sum() ?? 0;

            return (saldoTotalEntrada - saldoTotalSaida);
        }

        public int ObterSaldoFilial(int codigoFilial)
        {
            var movimentacoesEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>().Where(o => (o.NaturezaMovimentacao == NaturezaMovimentacaoEstoquePallet.Filial) && (o.Filial.Codigo == codigoFilial));
            var saldoTotalEntrada = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada select (int?)movimentacao.Quantidade).Sum() ?? 0;
            var saldoTotalSaida = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Saida select (int?)movimentacao.Quantidade).Sum() ?? 0;

            return (saldoTotalEntrada - saldoTotalSaida);
        }

        public int ObterSaldoReformaPorFilial(int codigoSituacaoDevolucaoPallet, int codigoFilial)
        {
            var movimentacoesEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>()
                .Where(o => (
                   (o.NaturezaMovimentacao == NaturezaMovimentacaoEstoquePallet.Reforma) &&
                   (o.SituacaoDevolucaoPallet.Codigo == codigoSituacaoDevolucaoPallet) &&
                   (o.Filial.Codigo == codigoFilial)
                ));

            var saldoTotalEntrada = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada select (int?)movimentacao.Quantidade).Sum() ?? 0;
            var saldoTotalSaida = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Saida select (int?)movimentacao.Quantidade).Sum() ?? 0;

            return (saldoTotalEntrada - saldoTotalSaida);
        }

        public int ObterSaldoTransportador(int codigoTransportador)
        {
            var movimentacoesEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>().Where(o => (o.NaturezaMovimentacao == NaturezaMovimentacaoEstoquePallet.Transportador) && (o.Transportador.Codigo == codigoTransportador));
            var saldoTotalEntrada = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada select (int?)movimentacao.Quantidade).Sum() ?? 0;
            var saldoTotalSaida = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Saida select (int?)movimentacao.Quantidade).Sum() ?? 0;

            return (saldoTotalEntrada - saldoTotalSaida);
        }

        //public int ObterSaldoTransportadorPorRotatividade(int codigoTransportador, DateTime dataPosicao)
        //{
        //    dataPosicao.Date.Add(DateTime.MaxValue.TimeOfDay);

        //    var movimentacoesEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>().Where(o => (o.NaturezaMovimentacao == NaturezaMovimentacaoEstoquePallet.Transportador) && (o.Transportador.Codigo == codigoTransportador));
        //    var saldoTotalEntrada = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada select (int?)movimentacao.Quantidade).Sum() ?? 0;
        //    var saldoTotalSaida = (from movimentacao in movimentacoesEstoque where movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Saida && (movimentacao.Data <= dataPosicao) select (int?)movimentacao.Quantidade).Sum() ?? 0;

        //    return (saldoTotalEntrada - saldoTotalSaida);
        //}

        public int ObterSaldoTransportadorPorRotatividade(int codigoTransportador, int diasRotatividade)
        {
            System.Text.StringBuilder sql = new System.Text.StringBuilder();

            sql.Append("select isnull(sum(EstoqueSaida.PES_QUANTIDADE), 0) ");
            sql.Append("  from T_PALLET_ESTOQUE as EstoqueSaida ");
            sql.Append("  join T_PALLET_DEVOLUCAO as DevolucaoPallet on DevolucaoPallet.PDE_CODIGO = EstoqueSaida.PDE_CODIGO ");
            sql.Append($"where EstoqueSaida.EMP_CODIGO = {codigoTransportador} ");
            sql.Append($"  and EstoqueSaida.PES_TIPO_MOVIMENTACAO = {(int)TipoMovimentacaoEstoquePallet.Entrada} ");
            sql.Append($"  and EstoqueSaida.PES_NATUREZA_MOVIMENTACAO = {(int)NaturezaMovimentacaoEstoquePallet.Transportador} ");
            sql.Append($"  and EstoqueSaida.pes_data >= dateadd(day, -{diasRotatividade}, getdate()) ");
            sql.Append($"  and DevolucaoPallet.PDE_SITUACAO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega} ");

            var query = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            return query.UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> BuscarPorDevolucao(int codigoDevolucao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pallets.EstoquePallet> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.EstoquePallet>();

            query = query.Where(o => o.Devolucao.Codigo == codigoDevolucao);

            return query.ToList();
        }

        #endregion
    }
}
