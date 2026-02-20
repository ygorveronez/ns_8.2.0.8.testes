using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Frota;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frota
{
    public sealed class Infracao : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Infracao>
    {
        #region Construtores

        public Infracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Infracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.Infracao> Consultar(FiltroPesquisaInfracao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Infracao>();

            if (filtrosPesquisa.CodigoCidade > 0)
                query = query.Where(o => o.Cidade.Codigo == filtrosPesquisa.CodigoCidade);

            if (filtrosPesquisa.CodigoTipoInfracao > 0)
                query = query.Where(o => o.TipoInfracao.Codigo == filtrosPesquisa.CodigoTipoInfracao);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo);

            if (filtrosPesquisa.DataInicio.HasValue)
                query = query.Where(o => o.Data >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                query = query.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Numero > 0)
                query = query.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroAtuacao))
                query = query.Where(o => o.NumeroAtuacao == filtrosPesquisa.NumeroAtuacao);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                query = query.Where(o => o.Veiculo.Placa == filtrosPesquisa.Placa);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.Motorista > 0)
                query = query.Where(o => o.Motorista.Codigo == filtrosPesquisa.Motorista);

            if (filtrosPesquisa.Funcionario > 0)
                query = query.Where(o => o.Funcionario.Codigo == filtrosPesquisa.Funcionario);

            if (filtrosPesquisa.OrgaoEmissor > 0D)
                query = query.Where(o => o.OrgaoEmissor.CPF_CNPJ == filtrosPesquisa.OrgaoEmissor);

            if (filtrosPesquisa.TipoOcorrenciaInfracao.HasValue)
                query = query.Where(o => o.TipoOcorrenciaInfracao == filtrosPesquisa.TipoOcorrenciaInfracao);

            if (filtrosPesquisa.InfracoesPendentes)
            {
                var queryInfracaoAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao>();
                query = query.Where(o => !queryInfracaoAcerto.Any(p => p.Infracao == o));
            }

            if (filtrosPesquisa.TipoInfracao.HasValue)
                query = query.Where(o => o.TipoInfracao.Tipo == filtrosPesquisa.TipoInfracao.Value);

            if (filtrosPesquisa.TipoHistorico.HasValue)
                query = query.Where(o => o.Historicos.Any(x => x.Tipo == filtrosPesquisa.TipoHistorico));

            return query;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.Infracao BuscarPorCodigo(int codigo)
        {
            var infracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Infracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return infracao;
        }

        public List<Dominio.Entidades.Embarcador.Frota.Infracao> BuscarInfracoesMotorista(int codigoMotorista, DateTime? dataInicial, DateTime? dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Infracao>();

            var result = from obj in query where obj.Situacao == SituacaoInfracao.Finalizada && obj.Motorista.Codigo == codigoMotorista select obj;

            if (dataInicial.HasValue)
                result = result.Where(o => o.Data.Date >= dataInicial.Value.Date);

            if (dataFinal.HasValue)
                result = result.Where(o => o.Data.Date <= dataFinal.Value.Date);

            var queryAcertoViagemInfracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao>();
            var resultAcertoViagemInfracao = from obj in queryAcertoViagemInfracao where obj.AcertoViagem.Situacao != SituacaoAcertoViagem.Cancelado select obj;
            result = result.Where(o => !resultAcertoViagemInfracao.Select(p => p.Infracao).Contains(o));

            return result.ToList();
        }

        public int BuscarProximoNumero()
        {
            var consultaInfracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Infracao>();
            int? ultimoNumero = consultaInfracao.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Frota.Infracao> Consultar(FiltroPesquisaInfracao filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaInfracao = Consultar(filtrosPesquisa);

            return ObterLista(consultaInfracao, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(FiltroPesquisaInfracao filtrosPesquisa)
        {
            var consultaInfracao = Consultar(filtrosPesquisa);

            return consultaInfracao.Count();
        }

        public bool ContemSinistroAdvertenciaMotorista(int motorista, DateTime dataInicial, DateTime dataFinal, TipoInfracaoTransito tipoInfracao)
        {
            var infracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Infracao>()
                .Where(o => o.Situacao != SituacaoInfracao.Cancelada && o.Motorista.Codigo == motorista && o.TipoInfracao != null && o.TipoInfracao.Tipo == tipoInfracao && o.Data <= dataFinal.AddDays(1) && o.Data >= dataInicial);

            return infracao.Any();
        }

        public bool ContemInfracaoMesmoNumeroAtuacao(string numeroAtuacao, int codigo)
        {
            var infracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Infracao>()
                .Where(o => o.Codigo != codigo && o.NumeroAtuacao == numeroAtuacao && o.Situacao != SituacaoInfracao.Cancelada);

            return infracao.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Frota.Infracao> BuscarInfracoesMotorista(bool somenteParaAcertoDeViagem, DateTime? dataInicial, DateTime? dataFinal, int codigoFuncionario, int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Infracao>();
            var result = from obj in query select obj;

            if (dataInicial.HasValue && dataInicial.Value != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicial.Value.Date);

            if (dataFinal.HasValue && dataFinal.Value != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFinal.Value.Date);

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoFuncionario);

            result = result.Where(obj => obj.Situacao == SituacaoInfracao.Finalizada);

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao>();
            var resultAcerto = from obj in queryAcerto where obj.Infracao != null && (obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento || obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado) select obj;

            //result = result.Where(obj => !(from p in resultAcerto select p.Infracao).Contains(obj));
            result = result.Where(obj => !resultAcerto.Any(c => c.Infracao == obj));

            return result.ToList();
        }

        public bool TituloContemInfracao(int codigoTitulo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frota.Infracao> infracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Infracao>();

            infracao = infracao.Where(o => o.TituloEmpresa.Codigo == codigoTitulo || o.Titulo.Codigo == codigoTitulo || o.TitulosEmpresa.Any(t => t.Titulo.Codigo == codigoTitulo) || o.Parcelas.Any(p => p.Titulo.Codigo == codigoTitulo));

            return infracao.Any();
        }

        public void RemoverVinculosPorCodigo(int codigoInfracao)
        {
            UnitOfWork.Sessao.CreateSQLQuery(@"DELETE A FROM T_ACERTO_VEICULO_TABELA_INFRACAO A JOIN T_ACERTO_DE_VIAGEM AA ON AA.ACV_CODIGO = A.ACV_CODIGO WHERE AA.ACV_SITUACAO = 3 AND A.INF_CODIGO = :codigoInfracao").SetParameter("codigoInfracao", codigoInfracao).ExecuteUpdate();
        }

        #endregion

        #region Relatório de Infrações

        public int ContarConsultaRelatorioInfracao(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMulta filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioInfracao(true, agrupamentos, filtrosPesquisa, "", "", "", "", 0, 0));

            return query.SetTimeout(300).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Infracao> ConsultarRelatorioInfracao(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMulta filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioInfracao(false, agrupamentos, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Infracao)));

            return query.SetTimeout(300).List<Dominio.Relatorios.Embarcador.DataSource.Frota.Infracao>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Infracao>> ConsultarRelatorioInfracaoAsync(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMulta filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioInfracao(false, agrupamentos, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Infracao)));
            return await query.SetTimeout(300).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.Infracao>();
        }

        private string ObterSelectConsultaRelatorioInfracao(bool count, List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMulta filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                  groupBy = string.Empty,
                  joins = string.Empty,
                  where = string.Empty,
                  orderBy = string.Empty;

            for (var i = agrupamentos.Count - 1; i >= 0; i--)
                SetarSelectConsultaRelatorioInfracao(agrupamentos[i].Propriedade, agrupamentos[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereConsultaRelatorioInfracao(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectConsultaRelatorioInfracao(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            return (count ? "select distinct(count(0) over ())" : "select " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_INFRACAO Infracao " + joins +
                   " where 1=1" + where +
                   (groupBy.Length > 0 ? " group by " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");
        }

        private void SetarWhereConsultaRelatorioInfracao(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMulta filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where += " and Infracao.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoTipoInfracao))
            {
                where += $" and TipoInfracao.TIN_TIPO = {filtrosPesquisa.TipoTipoInfracao} ";
                if (!joins.Contains(" TipoInfracao "))
                    joins += " left join T_TIPO_INFRACAO TipoInfracao on TipoInfracao.TIN_CODIGO = Infracao.TIN_CODIGO ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NivelInfracao))
            {
                where += $" and TipoInfracao.TIN_NIVEL = {filtrosPesquisa.NivelInfracao} ";
                if (!joins.Contains(" TipoInfracao "))
                    joins += " left join T_TIPO_INFRACAO TipoInfracao on TipoInfracao.TIN_CODIGO = Infracao.TIN_CODIGO ";
            }

            if (filtrosPesquisa.NumeroMulta > 0)
                where += " and Infracao.INF_NUMERO = " + filtrosPesquisa.NumeroMulta.ToString();

            if (filtrosPesquisa.CodigoCidade > 0)
                where += " and Infracao.LOC_CODIGO = " + filtrosPesquisa.CodigoCidade.ToString();

            if (filtrosPesquisa.CodigoTipoInfracao > 0)
                where += " and Infracao.TIN_CODIGO = " + filtrosPesquisa.CodigoTipoInfracao.ToString();

            if (filtrosPesquisa.CodigosTipoInfracoes.Count > 0)
                where += $" and Infracao.TIN_CODIGO in({ string.Join(", ", filtrosPesquisa.CodigosTipoInfracoes)}) ";

            if (filtrosPesquisa.CodigoMotorista > 0)
                where += " and Infracao.FUN_CODIGO_MOTORISTA = " + filtrosPesquisa.CodigoMotorista.ToString();

            if (filtrosPesquisa.CodigoTitulo > 0)
                where += " and Infracao.TIT_CODIGO = " + filtrosPesquisa.CodigoTitulo.ToString();

            if (filtrosPesquisa.CnpjPessoa > 0)
                where += " and Infracao.CLI_CGCCPF = " + filtrosPesquisa.CnpjPessoa.ToString();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroAtuacao))
                where += " and Infracao.INF_NUMERO_ATUACAO = '" + filtrosPesquisa.NumeroAtuacao + "'";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where += " and CAST(Infracao.INF_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where += " and CAST(Infracao.INF_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.DataLancamentoInicial != DateTime.MinValue)
                where += " and CAST(Infracao.INF_DATA_LANCAMENTO AS DATE) >= '" + filtrosPesquisa.DataLancamentoInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataLancamentoFinal != DateTime.MinValue)
                where += " and CAST(Infracao.INF_DATA_LANCAMENTO AS DATE) <= '" + filtrosPesquisa.DataLancamentoFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.DataVencimentoInicial != DateTime.MinValue)
            {
                where += " and CAST(InfracaoTitulo.IFT_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataVencimentoInicial.ToString(pattern) + "'";
                if (!joins.Contains(" InfracaoTitulo "))
                    joins += " left join T_INFRACAO_TITULO InfracaoTitulo on InfracaoTitulo.IFT_CODIGO = Infracao.IFT_CODIGO ";
            }

            if (filtrosPesquisa.DataVencimentoFinal != DateTime.MinValue)
            {
                where += " and CAST(InfracaoTitulo.IFT_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataVencimentoFinal.ToString(pattern) + "'";
                if (!joins.Contains(" InfracaoTitulo "))
                    joins += " left join T_INFRACAO_TITULO InfracaoTitulo on InfracaoTitulo.IFT_CODIGO = Infracao.IFT_CODIGO ";
            }

            if (filtrosPesquisa.DataVencimentoInicialPagar != DateTime.MinValue)
            {
                where += " and CAST(InfracaoPagar.ITE_DATA_VENCIMENTO AS DATE) >= '" + filtrosPesquisa.DataVencimentoInicialPagar.ToString(pattern) + "'";
                if (!joins.Contains(" InfracaoPagar "))
                    joins += " left join T_INFRACAO_TITULO_EMPRESA InfracaoPagar on InfracaoPagar.INF_CODIGO = Infracao.INF_CODIGO ";
            }

            if (filtrosPesquisa.DataVencimentoFinalPagar != DateTime.MinValue)
            {
                where += " and CAST(InfracaoPagar.ITE_DATA_VENCIMENTO AS DATE) <= '" + filtrosPesquisa.DataVencimentoFinalPagar.ToString(pattern) + "'";
                if (!joins.Contains(" InfracaoPagar "))
                    joins += " left join T_INFRACAO_TITULO_EMPRESA InfracaoPagar on InfracaoPagar.INF_CODIGO = Infracao.INF_CODIGO ";
            }

            if (filtrosPesquisa.CnpjFornecedorPagar > 0)
            {
                where += " and InfracaoPagar.CLI_CGCCPF = " + filtrosPesquisa.CnpjFornecedorPagar.ToString();
                if (!joins.Contains(" InfracaoPagar "))
                    joins += " left join T_INFRACAO_TITULO_EMPRESA InfracaoPagar on InfracaoPagar.INF_CODIGO = Infracao.INF_CODIGO ";
            }

            if ((int)filtrosPesquisa.PagoPor > 0)
                where += " and Infracao.INF_RESPONSAVEL_PAGAMENTO_INFRACAO = " + ((int)filtrosPesquisa.PagoPor).ToString();

            if ((int)filtrosPesquisa.StatusMulta > 0)
                where += " and Infracao.INF_SITUACAO = " + ((int)filtrosPesquisa.StatusMulta).ToString();

            if (filtrosPesquisa.DataLimiteInicial != DateTime.MinValue)
                where += " and CAST(Infracao.INF_DATA_LIMITE_INDICACAO_CONDUTOR AS DATE) >= '" + filtrosPesquisa.DataLimiteInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataLimiteFinal != DateTime.MinValue)
                where += " and CAST(Infracao.INF_DATA_LIMITE_INDICACAO_CONDUTOR AS DATE) <= '" + filtrosPesquisa.DataLimiteFinal.ToString(pattern) + "'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoOcorrenciaInfracao))
                where += $" and Infracao.INF_TIPO_OCORRENCIA_INFRACAO = {filtrosPesquisa.TipoOcorrenciaInfracao} ";

            if (filtrosPesquisa.TipoMotorista != TipoMotorista.Todos)
            {
                where += $" and Motorista.FUN_TIPO_MOTORISTA = " + ((int)filtrosPesquisa.TipoMotorista).ToString();
                if (!joins.Contains(" Motorista "))
                    joins += " left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Infracao.FUN_CODIGO_MOTORISTA ";
            }

            if (filtrosPesquisa.DataInicialEmissaoInfracao != DateTime.MinValue)
                where += " and CAST(Infracao.INF_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicialEmissaoInfracao.ToString(pattern) + "'";

            if (filtrosPesquisa.DataFinalEmissaoInfracao != DateTime.MinValue)
                where += " and CAST(Infracao.INF_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinalEmissaoInfracao.ToString(pattern) + "'";

        }

        private void SetarSelectConsultaRelatorioInfracao(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {

                case "AcertoViagem":
                    if (!select.Contains(" AcertoViagem,"))
                    {
                        select += @"(SELECT TOP(1) CAST(A.ACV_NUMERO AS VARCHAR(20)) FROM T_ACERTO_VEICULO_TABELA_INFRACAO AI
                                    JOIN T_INFRACAO I ON I.INF_CODIGO = AI.INF_CODIGO
                                    JOIN T_ACERTO_DE_VIAGEM A ON A.ACV_CODIGO = AI.ACV_CODIGO
                                    WHERE AI.INF_CODIGO = Infracao.INF_CODIGO) AcertoViagem, ";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo,"))
                    {
                        select += "Veiculo.VEI_PLACA Veiculo, ";

                        if (!joins.Contains(" Veiculo "))
                            joins += " left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Infracao.VEI_CODIGO ";
                    }
                    break;
                case "TipoTipoInfracaoDescricao":
                case "TipoTipoInfracao":
                    if (!select.Contains(" TipoTipoInfracao,"))
                    {
                        select += "TipoInfracao.TIN_TIPO TipoTipoInfracao, ";

                        if (!joins.Contains(" TipoInfracao "))
                            joins += " left join T_TIPO_INFRACAO TipoInfracao on TipoInfracao.TIN_CODIGO = Infracao.TIN_CODIGO ";
                    }
                    break;
                case "NumeroFrota":
                    if (!select.Contains(" NumeroFrota,"))
                    {
                        select += "Veiculo.VEI_NUMERO_FROTA NumeroFrota, ";

                        if (!joins.Contains(" Veiculo "))
                            joins += " left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Infracao.VEI_CODIGO ";
                    }
                    break;
                case "NumeroAtuacao":
                    if (!select.Contains(" NumeroAtuacao,"))
                    {
                        select += "Infracao.INF_NUMERO_ATUACAO NumeroAtuacao, ";
                    }
                    break;
                case "NumeroMulta":
                    if (!select.Contains(" NumeroMulta,"))
                    {
                        select += "Infracao.INF_NUMERO NumeroMulta, ";
                    }
                    break;
                case "DescricaoDataMulta":
                    if (!select.Contains(" DataMulta,"))
                    {
                        select += "Infracao.INF_DATA DataMulta, ";
                    }
                    break;
                case "DescricaoPagoPor":
                    if (!select.Contains(" PagoPor,"))
                    {
                        select += "Infracao.INF_RESPONSAVEL_PAGAMENTO_INFRACAO PagoPor, ";
                    }
                    break;
                case "LocalInfracao":
                    if (!select.Contains(" LocalInfracao,"))
                    {
                        select += "Infracao.INF_LOCAL LocalInfracao, ";
                    }
                    break;
                case "Cidade":
                    if (!select.Contains(" Cidade,"))
                    {
                        select += "Cidade.LOC_DESCRICAO Cidade, ";

                        if (!joins.Contains(" Cidade "))
                            joins += " left join T_LOCALIDADES Cidade on Cidade.LOC_CODIGO = Infracao.LOC_CODIGO ";
                    }
                    break;
                case "TipoInfracao":
                    if (!select.Contains(" TipoInfracao,"))
                    {
                        select += "TipoInfracao.TIN_DESCRICAO TipoInfracao, ";

                        if (!joins.Contains(" TipoInfracao "))
                            joins += " left join T_TIPO_INFRACAO TipoInfracao on TipoInfracao.TIN_CODIGO = Infracao.TIN_CODIGO ";
                    }
                    break;
                case "DescricaoNivel":
                    if (!select.Contains(" Nivel,"))
                    {
                        select += "TipoInfracao.TIN_NIVEL Nivel, ";

                        if (!joins.Contains(" TipoInfracao "))
                            joins += " left join T_TIPO_INFRACAO TipoInfracao on TipoInfracao.TIN_CODIGO = Infracao.TIN_CODIGO ";
                    }
                    break;
                case "ValorTipoInfracao":
                    if (!select.Contains(" ValorTipoInfracao,"))
                    {
                        select += "TipoInfracao.TIN_VALOR ValorTipoInfracao, ";

                        if (!joins.Contains(" TipoInfracao "))
                            joins += " left join T_TIPO_INFRACAO TipoInfracao on TipoInfracao.TIN_CODIGO = Infracao.TIN_CODIGO ";
                    }
                    break;
                case "PontosTipoInfracao":
                    if (!select.Contains(" PontosTipoInfracao,"))
                    {
                        select += "TipoInfracao.TIN_PONTOS PontosTipoInfracao, ";

                        if (!joins.Contains(" TipoInfracao "))
                            joins += " left join T_TIPO_INFRACAO TipoInfracao on TipoInfracao.TIN_CODIGO = Infracao.TIN_CODIGO ";
                    }
                    break;
                case "ReducaoComissao":
                    if (!select.Contains(" ReducaoComissao,"))
                    {
                        select += "TipoInfracao.TIN_PERCENTUAL_REDUCAO_COMISSAO_MOTORISTA ReducaoComissao, ";

                        if (!joins.Contains(" TipoInfracao "))
                            joins += " left join T_TIPO_INFRACAO TipoInfracao on TipoInfracao.TIN_CODIGO = Infracao.TIN_CODIGO ";
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista,"))
                    {
                        select += "Motorista.FUN_NOME Motorista, ";

                        if (!joins.Contains(" Motorista "))
                            joins += " left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Infracao.FUN_CODIGO_MOTORISTA ";
                    }
                    break;
                case "Pessoa":
                    if (!select.Contains(" Pessoa,"))
                    {
                        select += "Pessoa.CLI_NOME Pessoa, ";

                        if (!joins.Contains(" Pessoa "))
                            joins += " left join T_CLIENTE Pessoa on Pessoa.CLI_CGCCPF = Infracao.CLI_CGCCPF ";
                    }
                    break;
                case "DescricaoVencimento":
                    if (!select.Contains(" Vencimento,"))
                    {
                        select += "InfracaoTitulo.IFT_DATA_VENCIMENTO Vencimento, ";

                        if (!joins.Contains(" InfracaoTitulo "))
                            joins += " left join T_INFRACAO_TITULO InfracaoTitulo on InfracaoTitulo.IFT_CODIGO = Infracao.IFT_CODIGO ";
                    }
                    break;
                case "DescricaoVencimentoPagar":
                    if (!select.Contains(" VencimentoPagar,"))
                    {
                        select += "InfracaoPagar.ITE_DATA_VENCIMENTO VencimentoPagar, ";

                        if (!joins.Contains(" InfracaoPagar "))
                            joins += " left join T_INFRACAO_TITULO_EMPRESA InfracaoPagar on InfracaoPagar.INF_CODIGO = Infracao.INF_CODIGO ";
                    }
                    break;
                case "FornecedorPagar":
                    if (!select.Contains(" FornecedorPagar,"))
                    {
                        select += "ClienteFornecedorPagar.CLI_NOME FornecedorPagar, ";

                        if (!joins.Contains(" InfracaoPagar "))
                            joins += " left join T_INFRACAO_TITULO_EMPRESA InfracaoPagar on InfracaoPagar.INF_CODIGO = Infracao.INF_CODIGO ";
                        if (!joins.Contains(" ClienteFornecedorPagar "))
                            joins += " left join T_CLIENTE ClienteFornecedorPagar on InfracaoPagar.CLI_CGCCPF = ClienteFornecedorPagar.CLI_CGCCPF ";
                    }
                    break;
                case "DescricaoCompensacao":
                    if (!select.Contains(" Compensacao,"))
                    {
                        select += "InfracaoTitulo.IFT_DATA_COMPENSACAO Compensacao, ";

                        if (!joins.Contains(" InfracaoTitulo "))
                            joins += " left join T_INFRACAO_TITULO InfracaoTitulo on InfracaoTitulo.IFT_CODIGO = Infracao.IFT_CODIGO ";
                    }
                    break;
                case "ValorAteVencimento":
                    if (!select.Contains(" ValorAteVencimento,"))
                    {
                        select += "InfracaoTitulo.IFT_VALOR ValorAteVencimento, ";

                        if (!joins.Contains(" InfracaoTitulo "))
                            joins += " left join T_INFRACAO_TITULO InfracaoTitulo on InfracaoTitulo.IFT_CODIGO = Infracao.IFT_CODIGO ";
                    }
                    break;
                case "ValorAposVencimento":
                    if (!select.Contains(" ValorAposVencimento,"))
                    {
                        select += "InfracaoTitulo.IFT_VALOR_APOS_VENCIMENTO ValorAposVencimento, ";

                        if (!joins.Contains(" InfracaoTitulo "))
                            joins += " left join T_INFRACAO_TITULO InfracaoTitulo on InfracaoTitulo.IFT_CODIGO = Infracao.IFT_CODIGO ";
                    }
                    break;
                case "CodigoTitulo":
                    if (!select.Contains(" CodigoTitulo,"))
                    {
                        select += "Titulo.TIT_CODIGO CodigoTitulo, ";

                        if (!joins.Contains(" Titulo "))
                            joins += " left join T_TITULO Titulo on Titulo.TIT_CODIGO = Infracao.TIT_CODIGO ";
                    }
                    break;
                case "DescricaoSituacaoTitulo":
                    if (!select.Contains(" SituacaoTitulo,"))
                    {
                        select += "Titulo.TIT_STATUS SituacaoTitulo, ";

                        if (!joins.Contains(" Titulo "))
                            joins += " left join T_TITULO Titulo on Titulo.TIT_CODIGO = Infracao.TIT_CODIGO ";
                    }
                    break;
                case "SaldoTitulo":
                    if (!select.Contains(" SaldoTitulo,"))
                    {
                        select += "Titulo.TIT_VALOR_PENDENTE SaldoTitulo, ";

                        if (!joins.Contains(" Titulo "))
                            joins += " left join T_TITULO Titulo on Titulo.TIT_CODIGO = Infracao.TIT_CODIGO ";
                    }
                    break;
                case "DescricaoStatusMulta":
                    if (!select.Contains(" StatusMulta,"))
                    {
                        select += "Infracao.INF_SITUACAO StatusMulta, ";
                    }
                    break;
                case "NumeroMatriculaMotorista":
                    if (!select.Contains(" NumeroMatriculaMotorista,"))
                    {
                        select += "Motorista.FUN_NUMERO_MATRICULA NumeroMatriculaMotorista, ";

                        if (!joins.Contains(" Motorista "))
                            joins += " left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Infracao.FUN_CODIGO_MOTORISTA ";
                    }
                    break;
                case "NumerosParcelasMulta":
                    if (!select.Contains(" NumerosParcelasMulta,"))
                    {
                        select += @"SUBSTRING((SELECT ', ' + CAST(InfracaoParcela.IFP_PARCELA AS NVARCHAR(10))
		                                        FROM T_INFRACAO_PARCELA InfracaoParcela
		                                        WHERE InfracaoParcela.INF_CODIGO = Infracao.INF_CODIGO FOR XML PATH('')), 3, 1000) AS NumerosParcelasMulta, ";
                    }
                    break;
                case "TitulosParcelasMulta":
                    if (!select.Contains(" TitulosParcelasMulta,"))
                    {
                        select += @"SUBSTRING((SELECT ', ' + CAST(InfracaoParcela.TIT_CODIGO AS NVARCHAR(10))
		                                        FROM T_INFRACAO_PARCELA InfracaoParcela
                                                INNER JOIN T_TITULO Titulo on Titulo.TIT_CODIGO = InfracaoParcela.TIT_CODIGO
		                                        WHERE InfracaoParcela.INF_CODIGO = Infracao.INF_CODIGO FOR XML PATH('')), 3, 1000) AS TitulosParcelasMulta, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao,"))
                    {
                        select += @"Infracao.INF_OBSERVACAO Observacao, ";
                    }
                    break;
                case "UltimoHistoricoDescricao":
                case "UltimoHistorico":
                    if (!select.Contains(" UltimoHistoricoDescricao,"))
                    {
                        select += @"(SELECT TOP 1 _historico.IFH_TIPO FROM T_INFRACAO_HISTORICO _historico WHERE _historico.INF_CODIGO = Infracao.INF_CODIGO order by _historico.IFH_DATA desc) UltimoHistorico, ";
                    }
                    break;
                case "CPFMotoristaFormatada":
                    if (!select.Contains(" CPFMotorista,"))
                    {
                        select += "Motorista.FUN_CPF CPFMotorista, ";

                        if (!joins.Contains(" Motorista "))
                            joins += " left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Infracao.FUN_CODIGO_MOTORISTA ";
                    }
                    break;
                case "RGMotorista":
                    if (!select.Contains(" RGMotorista,"))
                    {
                        select += "Motorista.FUN_RG RGMotorista, ";

                        if (!joins.Contains(" Motorista "))
                            joins += " left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Infracao.FUN_CODIGO_MOTORISTA ";
                    }
                    break;
                case "CodigoIntegracaoMotorista":
                    if (!select.Contains(" CodigoIntegracaoMotorista,"))
                    {
                        select += "Motorista.FUN_CODIGO_INTEGRACAO CodigoIntegracaoMotorista, ";

                        if (!joins.Contains(" Motorista "))
                            joins += " left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Infracao.FUN_CODIGO_MOTORISTA ";
                    }
                    break;
                case "DataInfracaoFormatada":
                    if (!select.Contains(" DataInfracao,"))
                    {
                        select += "Infracao.INF_DATA_LIMITE_INDICACAO_CONDUTOR DataInfracao, ";
                    }
                    break;
                case "ValorEtapa4":
                    if (!select.Contains(" ValorEtapa4,"))
                    {
                        select += "(SELECT SUM(TituloEmpresa.ITE_VALOR) " +
                            "FROM T_INFRACAO_TITULO_EMPRESA TituloEmpresa " +
                            "LEFT JOIN T_INFRACAO _infracao ON _infracao.INF_CODIGO = TituloEmpresa.INF_CODIGO " +
                            "WHERE _infracao.INF_CODIGO = Infracao.INF_CODIGO) ValorEtapa4, ";
                    }
                    break;
                case "ValorNotaFiscal":
                    if (!select.Contains(" ValorNotaFiscal,"))
                    {
                        select += "Infracao.INF_VALOR_ESTIMADO_PREJUIZO ValorNotaFiscal, ";
                    }
                    break;
                case "ValorEstimadoPrejuizo":
                    if (!select.Contains(" ValorEstimadoPrejuizo,"))
                    {
                        select += "Infracao.INF_VALOR_NOTA_FISCAL ValorEstimadoPrejuizo, ";
                    }
                    break;
                case "DataLancamentoFormatada":
                    if (!select.Contains(" DataLancamento,"))
                    {
                        select += "Infracao.INF_DATA_LANCAMENTO DataLancamento, ";
                    }
                    break;
                case "OrgaoEmissor":
                    if (!select.Contains(" OrgaoEmissor,"))
                    {
                        select += "OrgaoEmissor.CLI_NOME OrgaoEmissor, ";

                        if (!joins.Contains(" OrgaoEmissor "))
                            joins += " left join T_CLIENTE OrgaoEmissor on OrgaoEmissor.CLI_CGCCPF = Infracao.CLI_CGCCPF_ORGAO_EMISSOR ";
                    }
                    break;
                case "DescricaoTipoMotorista":
                    if (!select.Contains(" TipoMotorista,"))
                    {
                        select += "Motorista.FUN_TIPO_MOTORISTA TipoMotorista, ";

                        if (!joins.Contains(" Motorista "))
                            joins += " left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Infracao.FUN_CODIGO_MOTORISTA ";
                    }
                    break;
                case "UF":
                    if (!select.Contains(" UF, "))
                    {
                        select += "Cidade.UF_SIGLA UF, ";

                        if (!joins.Contains(" Cidade "))
                            joins += " left join T_LOCALIDADES Cidade on Cidade.LOC_CODIGO = Infracao.LOC_CODIGO ";
                    }
                    break;

                case "DataEmissaoInfracaoFormatada":
                    if (!select.Contains(" DataEmissaoInfracao,"))
                    {
                        select += "Infracao.INF_DATA_EMISSAO DataEmissaoInfracao, ";
                    }
                    break;
            }
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.MultaParcela> ConsultarRelatorioMultaParcela(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaMultaParcela().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.MultaParcela)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.MultaParcela>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.MultaParcela>> ConsultarRelatorioMultaParcelaAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaMultaParcela().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.MultaParcela)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.MultaParcela>();
        }

        public int ContarConsultaRelatorioMultaParcela(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioMultaParcela filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaMultaParcela().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
