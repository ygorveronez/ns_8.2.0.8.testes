using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Terceiros
{
    public class ContratoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>
    {
        public ContratoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ContratoFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> BuscarContratoPendenteEncerramentoCIOT()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();
            var resut = from obj in query where obj.EmEncerramentoCIOT && obj.SituacaoContratoFrete != SituacaoContratoFrete.Finalizada && obj.SituacaoContratoFrete != SituacaoContratoFrete.Cancelado select obj;
            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFrete BuscarPorPagamentoAgregado(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();
            var resut = from obj in query where obj.PagamentoAgregado.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFrete BuscarPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            query = query.Where(o => o.Carga.Codigo == carga && o.Transbordo == null);

            return query.Fetch(o => o.ConfiguracaoCIOT).FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> BuscarPorCargaAsync(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            query = query.Where(o => o.Carga.Codigo == carga && o.Transbordo == null);

            return await query.Fetch(o => o.ConfiguracaoCIOT).FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFrete BuscarPorTransbordo(int transbordo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();
            var resut = from obj in query where obj.Transbordo.Codigo == transbordo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> BuscarTodosPendentes()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            query = query.Where(o => o.CIOT.Situacao != SituacaoCIOT.Cancelado);

            return query.Select(o => o.ContratoFrete).Distinct().ToList();
        }

        public List<int> CONTRATOSPARAREPROCESSAR()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();
            var resut = from obj in query
                        where
                        obj.SituacaoContratoFrete == SituacaoContratoFrete.AgAprovacao
                        &&
                        (
                            obj.Carga.SituacaoCarga == SituacaoCarga.Nova &&
                            obj.Carga.SituacaoCarga == SituacaoCarga.AgNFe &&
                            obj.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete
                        )
                        select obj.Codigo;
            return resut.ToList();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            int? retorno = query.Max(o => (int?)o.NumeroContrato);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        public int ContarPorNumeroContrato(int numeroContrato)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            query = query.Where(o => o.NumeroContrato == numeroContrato);

            return query.Select(o => o.Codigo).Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFrete filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> result = Consultar(filtrosPesquisa);

            result = result.Fetch(o => o.TransportadorTerceiro);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFrete filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<int> ConsultarSeExisteContratoPendente(int codigoEmpresa, DateTime dataFechamento, SituacaoContratoFrete[] situacaoContratoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            query = query.Where(o => o.DataEmissaoContrato < dataFechamento.AddDays(1).Date && situacaoContratoFrete.Contains(o.SituacaoContratoFrete));

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Carga.Empresa.Codigo == codigoEmpresa);

            return query.Select(o => o.NumeroContrato).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> BuscarContratosESocial(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var resut = from obj in query
                        where
                              obj.StatusTitulo == StatusTitulo.Quitada &&
                              obj.DataLiquidacao.Value.Date >= dataInicial &&
                              dataFinal >= obj.DataLiquidacao.Value.Date &&
                              obj.ContratoFrete != null &&
                              obj.ContratoFrete.TransportadorTerceiro.Tipo == "F" &&
                              obj.ContratoFrete.TransportadorTerceiro.Modalidades.Where(o => o.TipoModalidade == TipoModalidade.TransportadorTerceiro).Any(p => p.ModalidadesTransportadora.Any(t => t.TipoTransportador == TipoProprietarioVeiculo.TACIndependente || t.TipoTransportador == TipoProprietarioVeiculo.TACAgregado))
                        select obj;

            if (codigoEmpresa > 0)
                resut = resut.Where(obj => obj.ContratoFrete.Carga.Empresa.Codigo == codigoEmpresa);

            return resut.Select(c => c.ContratoFrete)?.OrderBy("TransportadorTerceiro.CPF_CNPJ, NumeroContrato")?.ToList() ?? null;
        }

        public int ContarAprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var resut = from obj in query
                        where
                            obj.ContratoFrete.Codigo == codigo
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada
                        select obj;

            return resut.Count();
        }

        public int ContarReprovacoes(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var resut = from obj in query
                        where
                            obj.ContratoFrete.Codigo == codigo
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesNecessarias(int codigo)
        {
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AprovacaoAlcadaContratoFrete>();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro>();

            var resutAutorizacao = from aut in queryAutorizacao where aut.ContratoFrete.Codigo == codigo select aut.RegraContratoFreteTerceiro;
            var resut = from obj in query where resutAutorizacao.Contains(obj) select obj;

            return resut.Sum(o => (int?)o.NumeroAprovadores) ?? 0;
        }

        public List<int> BuscarNumeroContratoInvalidoParaCancelamento(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     o.Transbordo == null &&
                                     (o.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Finalizada ||
                                      o.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado));

            return query.Select(o => o.NumeroContrato).ToList();
        }

        public decimal BuscarValorPorRaizCNPJPorPeriodo(string empresaCNPJ, double transportadorTerceiro, DateTime dataInicial, DateTime dataFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            query = query.Where(o => o.SituacaoContratoFrete != SituacaoContratoFrete.Cancelado && o.Carga.Empresa.CNPJ.Substring(0, 8) == empresaCNPJ.Substring(0, 8) && o.TransportadorTerceiro.CPF_CNPJ == transportadorTerceiro && o.DataEmissaoContrato >= dataInicial && o.DataEmissaoContrato < dataFinal);

            return query.Sum(o => (decimal?)(o.ValorFreteSubcontratacao - o.TarifaSaque - o.TarifaTransferencia)) ?? 0m;
        }

        public decimal BuscarValorPorTerceiroPorPeriodo(double transportadorTerceiro, DateTime dataInicial, DateTime dataFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            query = query.Where(o => o.SituacaoContratoFrete != SituacaoContratoFrete.Cancelado && o.TransportadorTerceiro.CPF_CNPJ == transportadorTerceiro && o.DataEmissaoContrato >= dataInicial && o.DataEmissaoContrato < dataFinal);

            return query.Sum(o => (decimal?)(o.ValorFreteSubcontratacao - o.TarifaSaque - o.TarifaTransferencia)) ?? 0m;
        }
        public decimal BuscarIRRFPorRaizCNPJPorPeriodo(string empresaCNPJ, double transportadorTerceiro, DateTime dataInicial, DateTime dataFinal, int codigoContratoFrete)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            query = query.Where(o => o.SituacaoContratoFrete != SituacaoContratoFrete.Cancelado && o.Carga.Empresa.CNPJ.Substring(0, 8) == empresaCNPJ.Substring(0, 8) && o.TransportadorTerceiro.CPF_CNPJ == transportadorTerceiro && o.DataEmissaoContrato >= dataInicial && o.DataEmissaoContrato < dataFinal && o.Codigo != codigoContratoFrete);

            return query.Sum(o => (decimal?)o.ValorIRRF) ?? 0m;
        }

        public decimal BuscarIRRFPorTerceiroPorPeriodo(double transportadorTerceiro, DateTime dataInicial, DateTime dataFinal, int codigoContratoFrete)
        {
            IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            query = query.Where(o => o.SituacaoContratoFrete != SituacaoContratoFrete.Cancelado && o.TransportadorTerceiro.CPF_CNPJ == transportadorTerceiro && o.DataEmissaoContrato >= dataInicial && o.DataEmissaoContrato < dataFinal && o.Codigo != codigoContratoFrete);

            return query.Sum(o => (decimal?)o.ValorIRRF) ?? 0m;
        }

        public int ContarContratosFretePendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();
            query = query.Where(c => ((bool?)c.Integrado ?? false) == false);
            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> BuscarContratoFreteNaoIntegrados(string dataInicial, string dataFinal, int inicio, int quantidadeRegistros)
        {
            DateTime? _dataInicial = dataInicial.ToNullableDateTime();
            DateTime? _dataFinal = dataFinal.ToNullableDateTime();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            if (_dataInicial.HasValue)
                query = query.Where(c => c.DataEmissaoContrato.Date >= _dataInicial.Value.Date);

            if (_dataFinal.HasValue)
                query = query.Where(c => c.DataEmissaoContrato.Date <= _dataFinal.Value.Date);

            query = query.Where(c => ((bool?)c.Integrado ?? false) == false);

            return query.OrderBy(c => c.Codigo).Skip(inicio).Take(quantidadeRegistros).ToList();
        }

        public bool BuscarCiotPorContrato(int contratoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();
            var result = from obj in query where obj.ContratoFrete.Codigo == contratoFrete select obj;
            return result.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFrete filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.NumeroContrato > 0)
                result = result.Where(obj => obj.NumeroContrato == filtrosPesquisa.NumeroContrato);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
                result = result.Where(obj => obj.Carga.CodigoCargaEmbarcador.Equals(filtrosPesquisa.Carga));

            if (filtrosPesquisa.TransportadorTerceiro > 0)
                result = result.Where(obj => obj.TransportadorTerceiro.CPF_CNPJ == filtrosPesquisa.TransportadorTerceiro);

            if (filtrosPesquisa.SituacaoContrato?.Count > 0 && !filtrosPesquisa.SituacaoContrato.Contains(SituacaoContratoFrete.todos))
                result = result.Where(obj => filtrosPesquisa.SituacaoContrato.Contains(obj.SituacaoContratoFrete));

            if (filtrosPesquisa.Bloqueado.HasValue)
                result = result.Where(o => o.Bloqueado == filtrosPesquisa.Bloqueado.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCIOT))
                result = result.Where(obj => obj.Carga.CargaCIOTs.Any(x => x.CIOT.Numero.Contains(filtrosPesquisa.NumeroCIOT)));

            if (filtrosPesquisa.DataInicialContratoFrete != DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissaoContrato >= filtrosPesquisa.DataInicialContratoFrete);

            if (filtrosPesquisa.DataFinalContratoFrete != DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissaoContrato <= filtrosPesquisa.DataFinalContratoFrete);

            return result;
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizado> ConsultarRelatorioFreteTerceirizado(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizado filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioFreteTerceirizado(false, propriedades, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizado)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizado>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizado>> ConsultarRelatorioFreteTerceirizadoAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizado filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioFreteTerceirizado(false, propriedades, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizado)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizado>();
        }

        public async Task<int> ContarConsultaRelatorioFreteTerceirizadoAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizado filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioFreteTerceirizado(true, propriedades, filtrosPesquisa, "", "", "", "", 0, 0);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return await query.SetTimeout(600).UniqueResultAsync<int>();
        }

        public int ContarConsultaRelatorioFreteTerceirizado(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizado filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioFreteTerceirizado(true, propriedades, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private SQLDinamico ObterSelectConsultaRelatorioFreteTerceirizado(bool count, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizado filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            var parametros = new List<ParametroSQL>();

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioFreteTerceirizado(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count, filtrosPesquisa);

            SetarWhereRelatorioFreteTerceirizado(ref where, ref groupBy, ref joins, ref parametros, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioFreteTerceirizado(propAgrupa, 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            var sql = (count ? "select distinct(count(0) over ())" : "select " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_CONTRATO_FRETE_TERCEIRO Contrato " + joins + ", T_CONFIGURACAO_EMBARCADOR ConfiguracaoEmbarcador" +
                   " where 1=1" + where +
                   (groupBy.Length > 0 ? " group by " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");


            return new SQLDinamico(sql, parametros);
        }

        private void SetarSelectRelatorioFreteTerceirizado(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizado filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "TipoFornecedor":
                    if (!select.Contains(" TipoFornecedor, "))
                    {
                        select += "Terceiro.CLI_TIPO_FORNECEDOR TipoFornecedor, ";
                        groupBy += "Terceiro.CLI_TIPO_FORNECEDOR, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "MesCompetencia":
                    if (!select.Contains("MesCompetencia,"))
                    {
                        select += " MONTH(Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO) MesCompetencia, ";
                        groupBy += "Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO, ";
                    }
                    break;
                case "AnoCompetencia":
                    if (!select.Contains("AnoCompetencia,"))
                    {
                        select += " YEAR(Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO) MesCompetencia, ";
                        groupBy += "Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO, ";
                    }
                    break;
                case "CentroResultadoEmpresa":
                    if (!select.Contains(" CentroResultadoEmpresa, "))
                    {
                        select += "Empresa.EMP_CODIGO_CENTRO_CUSTO CentroResultadoEmpresa, ";
                        groupBy += "Empresa.EMP_CODIGO_CENTRO_CUSTO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";
                    }
                    break;
                case "CodigoEstabelecimento":
                    if (!select.Contains(" CodigoEstabelecimento, "))
                    {
                        select += "Empresa.EMP_CODIGO_ESTABELECIMENTO CodigoEstabelecimento, ";
                        groupBy += "Empresa.EMP_CODIGO_ESTABELECIMENTO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";
                    }
                    break;
                case "CodigoEmpresa":
                    if (!select.Contains(" CodigoEmpresa, "))
                    {
                        select += "Empresa.EMP_CODIGO_EMPRESA CodigoEmpresa, ";
                        groupBy += "Empresa.EMP_CODIGO_EMPRESA, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";
                    }
                    break;
                case "MunicipioLancamento":
                    if (!select.Contains(" MunicipioLancamento, "))
                    {
                        select += "LocalidadeEmpresa.LOC_IBGE MunicipioLancamento, ";
                        groupBy += "LocalidadeEmpresa.LOC_IBGE, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";

                        if (!joins.Contains(" LocalidadeEmpresa "))
                            joins += "join T_LOCALIDADES LocalidadeEmpresa on Empresa.LOC_CODIGO = LocalidadeEmpresa.LOC_CODIGO ";
                    }
                    break;
                case "ContratoFrete":
                    if (!select.Contains(" ContratoFrete, "))
                    {
                        select += "Contrato.CFT_NUMERO_CONTRATO ContratoFrete, ";
                        groupBy += "Contrato.CFT_NUMERO_CONTRATO, ";
                    }
                    break;
                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga,"))
                    {
                        select += "Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ";
                        groupBy += "Carga.CAR_CODIGO_CARGA_EMBARCADOR, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "NumeroCTes":
                    if (!select.Contains("NumeroCTes,"))
                    {
                        select += "SUBSTRING((SELECT DISTINCT ', ' + CAST(cte.CON_NUM AS NVARCHAR(20)) FROM T_CARGA_CTE cargaCTe inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO WHERE cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({ string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D"))) }) " : string.Empty;
                        select += " FOR XML PATH('')), 3, 1000) NumeroCTes, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "SerieCTes":
                    if (!select.Contains("SerieCTes,"))
                    {
                        select += "SUBSTRING((SELECT DISTINCT ', ' + CAST(Serie.ESE_NUMERO AS NVARCHAR(20)) FROM T_CARGA_CTE cargaCTe inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO JOIN T_EMPRESA_SERIE Serie on Serie.ESE_CODIGO = cte.CON_SERIE WHERE cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({ string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D"))) }) " : string.Empty;
                        select += " FOR XML PATH('')), 3, 1000) SerieCTes, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "TipoDocumento":
                    if (!select.Contains("TipoDocumento,"))
                    {
                        select += "SUBSTRING((SELECT DISTINCT ', ' + modeloDocumento.MOD_ABREVIACAO FROM T_CARGA_CTE cargaCTe inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO inner join T_MODDOCFISCAL modeloDocumento on modeloDocumento.MOD_CODIGO = cte.CON_MODELODOC WHERE cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL  FOR XML PATH('')), 3, 1000) TipoDocumento,  ";
                        groupBy += "Carga.CAR_CODIGO, ";
                    }
                    break;
                case "StatusCTes":
                    if (!select.Contains("StatusCTes,"))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + CASE WHEN cte.CON_STATUS = 'P' THEN 'Pendente'
	                                WHEN cte.CON_STATUS = 'E' THEN 'Enviado'
	                                WHEN cte.CON_STATUS = 'S' THEN 'Salvo'
	                                WHEN cte.CON_STATUS = 'A' THEN 'Autorizado'
	                                WHEN cte.CON_STATUS = 'C' THEN 'Cancelado'
	                                WHEN cte.CON_STATUS = 'Z' THEN 'Anulado'
	                                WHEN cte.CON_STATUS = 'R' THEN 'Rejeitado'
	                                WHEN cte.CON_STATUS = 'I' THEN 'Inutilizado'
	                                WHEN cte.CON_STATUS = 'S' THEN 'Em Digitação'
	                                WHEN cte.CON_STATUS = 'D' THEN 'Denegado'
	                                ELSE cte.CON_STATUS
                                END FROM T_CARGA_CTE cargaCTe inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO WHERE cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({ string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D"))) }) " : string.Empty;
                        select += " FOR XML PATH('')), 3, 1000) StatusCTes, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "RG":
                    if (!select.Contains("RG,"))
                    {
                        select += "SUBSTRING(ISNULL((SELECT DISTINCT ', ' + mot.FUN_RG FROM T_CARGA_MOTORISTA cargaMotorista inner join T_FUNCIONARIO mot ON mot.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) RG, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "Motorista":
                    if (!select.Contains("Motorista,"))
                    {
                        select += "SUBSTRING(ISNULL((SELECT DISTINCT ', ' + mot.FUN_NOME FROM T_CARGA_MOTORISTA cargaMotorista inner join T_FUNCIONARIO mot ON mot.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) Motorista, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "CPFMotorista":
                    if (!select.Contains("CPFMotorista,"))
                    {
                        select += "SUBSTRING(ISNULL((SELECT DISTINCT ', ' + mot.FUN_CPF FROM T_CARGA_MOTORISTA cargaMotorista inner join T_FUNCIONARIO mot ON mot.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) CPFMotorista, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "NumeroMDFes":
                    if (!select.Contains("NumeroMDFes,"))
                    {
                        select += "SUBSTRING((SELECT DISTINCT ', ' + CAST(mdfe.MDF_NUMERO AS NVARCHAR(20)) FROM T_CARGA_MDFE cargaMDFe inner join T_MDFE mdfe ON mdfe.MDF_CODIGO = cargaMDFe.MDF_CODIGO WHERE cargaMDFe.CAR_CODIGO = Carga.CAR_CODIGO and cargaMDFe.MDF_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000) NumeroMDFes, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "NumeroCIOT":
                    if (!select.Contains("NumeroCIOT,"))
                    {
                        select += "CIOT.CIO_NUMERO NumeroCIOT, ";
                        groupBy += "CIOT.CIO_NUMERO, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;
                case "DataSaqueAdiantamento":
                    if (!select.Contains("DataSaqueAdiantamento,"))
                    {
                        select += "convert(nvarchar(10), CIOT.CIO_DATA_SAQUE_ADIANTAMENTO, 3) + ' ' + convert(nvarchar(10), CIOT.CIO_DATA_SAQUE_ADIANTAMENTO, 108) DataSaqueAdiantamento, ";
                        groupBy += "CIOT.CIO_DATA_SAQUE_ADIANTAMENTO, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;
                case "ValorSaqueAdiantamento":
                    if (!select.Contains("ValorSaqueAdiantamento,"))
                    {
                        select += "CIOT.CIO_VALOR_SAQUE_ADIANTAMENTO ValorSaqueAdiantamento, ";
                        groupBy += "CIOT.CIO_VALOR_SAQUE_ADIANTAMENTO, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo,"))
                    {
                        select += "((select vei.VEI_PLACA from T_VEICULO vei where vei.VEI_CODIGO = Carga.CAR_VEICULO) + ISNULL((SELECT ', ' + veiculo1.VEI_PLACA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')) Veiculo, ";
                        groupBy += "Carga.CAR_CODIGO, Carga.CAR_VEICULO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador,"))
                    {
                        select += "SUBSTRING((SELECT DISTINCT ', ' + pedido.PED_NUMERO_PEDIDO_EMBARCADOR FROM T_PEDIDO pedido inner join T_CARGA_PEDIDO cargaPedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO WHERE cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) NumeroPedidoEmbarcador, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular,"))
                    {
                        select += "reverse(stuff(reverse(ISNULL((select mod.MVC_DESCRICAO + ', ' from T_VEICULO vei inner join T_MODELO_VEICULAR_CARGA mod on mod.MVC_CODIGO = vei.MVC_CODIGO where vei.VEI_CODIGO = Carga.CAR_VEICULO), '') + ISNULL((SELECT mod.MVC_DESCRICAO + ', ' FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO inner join T_MODELO_VEICULAR_CARGA mod on mod.MVC_CODIGO = veiculo1.MVC_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')), 1, 2, '')) ModeloVeicular, ";
                        groupBy += "Carga.CAR_CODIGO, Carga.CAR_VEICULO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "SegmentoVeiculo":
                    if (!select.Contains(" SegmentoVeiculo,"))
                    {
                        select += "reverse(stuff(reverse(ISNULL((select seg.VSE_DESCRICAO + ', ' from T_VEICULO vei inner join T_VEICULO_SEGMENTO seg on seg.VSE_CODIGO = vei.VSE_CODIGO where vei.VEI_CODIGO = Carga.CAR_VEICULO), '') + ISNULL((SELECT seg.VSE_DESCRICAO + ', ' FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO inner join T_VEICULO_SEGMENTO seg on seg.VSE_CODIGO = veiculo1.VSE_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')), 1, 2, '')) SegmentoVeiculo, ";
                        groupBy += "Carga.CAR_CODIGO, Carga.CAR_VEICULO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "Terceiro":
                    if (!select.Contains(" Terceiro,"))
                    {
                        select += "Terceiro.CLI_NOME Terceiro, ";
                        groupBy += "Terceiro.CLI_NOME, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "CPFCNPJTerceiroFormatado":
                    if (!select.Contains("CPFCNPJTerceiro,"))
                    {
                        select += "Terceiro.CLI_CGCCPF CPFCNPJTerceiro, Terceiro.CLI_FISJUR TipoTerceiro, ";
                        groupBy += "Terceiro.CLI_CGCCPF, Terceiro.CLI_FISJUR, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "PISPASEPTerceiro":
                    if (!select.Contains("PISPASEPTerceiro,"))
                    {
                        select += "Terceiro.CLI_PIS_PASEP PISPASEPTerceiro, ";
                        groupBy += "Terceiro.CLI_PIS_PASEP, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "DataNascimentoTerceiro":
                    if (!select.Contains("DataNascimentoTerceiro,"))
                    {
                        select += "CONVERT(NVARCHAR(10), Terceiro.CLI_DATA_NASCIMENTO, 103) DataNascimentoTerceiro, ";
                        groupBy += "Terceiro.CLI_DATA_NASCIMENTO, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "RegimeTributarioTerceiroFormatado":
                    if (!select.Contains(" RegimeTributarioTerceiro, "))
                    {
                        select += "Terceiro.CLI_REGIME_TRIBUTARIO RegimeTributarioTerceiro, ";
                        groupBy += "Terceiro.CLI_REGIME_TRIBUTARIO, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "TipoPessoaTerceiroFormatado":
                    if (!select.Contains(" TipoPessoaTerceiro, "))
                    {
                        select += "Terceiro.CLI_FISJUR TipoPessoaTerceiro, ";
                        groupBy += "Terceiro.CLI_FISJUR, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select += "CargaDadosSumarizados.CDS_REMETENTES Remetente, ";
                        groupBy += "CargaDadosSumarizados.CDS_REMETENTES, ";

                        if (!joins.Contains(" Carga "))
                            joins += " left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "LocalidadeRemetente":
                    if (!select.Contains("LocalidadeRemetente,"))
                    {
                        select += "CargaDadosSumarizados.CDS_ORIGENS LocalidadeRemetente, ";
                        groupBy += "CargaDadosSumarizados.CDS_ORIGENS, ";

                        if (!joins.Contains(" Carga "))
                            joins += " left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;

                case "CPFCNPJRemetente":
                    if (!select.Contains("CPFCNPJRemetente,"))
                    {
                        select += @"SUBSTRING(ISNULL((SELECT DISTINCT ', ' + case when Cliente.CLI_FISJUR = 'J' then FORMAT(Cliente.CLI_CGCCPF, '00\.000\.000\/0000-00')
                                    when Cliente.CLI_FISJUR = 'F' then FORMAT(Cliente.CLI_CGCCPF, '000\.000\.000-00') 
                                    else '' end
                                    FROM T_CARGA_PEDIDO CargaPedido                                    
                                    JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                                    JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
                                    WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) CPFCNPJRemetente, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;

                case "CPFCNPJDestinatario":
                    if (!select.Contains("CPFCNPJDestinatario,"))
                    {
                        select += @"SUBSTRING(ISNULL((SELECT DISTINCT ', ' + case when Cliente.CLI_FISJUR = 'J' then FORMAT(Cliente.CLI_CGCCPF, '00\.000\.000\/0000-00')
                                    when Cliente.CLI_FISJUR = 'F' then FORMAT(Cliente.CLI_CGCCPF, '000\.000\.000-00') 
                                    else '' end
                                    FROM T_CARGA_PEDIDO CargaPedido                                    
                                    JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                                    JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = Pedido.CLI_CODIGO
                                    WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) CPFCNPJDestinatario, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor,"))
                    {
                        select += "CargaDadosSumarizados.CDS_EXPEDIDORES Expedidor, ";
                        groupBy += "CargaDadosSumarizados.CDS_EXPEDIDORES, ";

                        if (!joins.Contains(" Carga "))
                            joins += " left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "LocalidadeExpedidor":
                    if (!select.Contains("LocalidadeExpedidor,"))
                    {
                        select += @"ISNULL((SELECT TOP(1) Localidade.LOC_DESCRICAO LocalidadeExpedidor FROM T_LOCALIDADES Localidade
                                    JOIN T_CLIENTE Cliente on Cliente.LOC_CODIGO = Localidade.LOC_CODIGO
                                    JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CLI_CODIGO_EXPEDIDOR = Cliente.CLI_CGCCPF 
                                    WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO), '') LocalidadeExpedidor, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "UFExpedidor":
                    if (!select.Contains("UFExpedidor,"))
                    {
                        select += @"ISNULL((SELECT TOP(1) Localidade.UF_SIGLA UFExpedidor FROM T_LOCALIDADES Localidade
                                    JOIN T_CLIENTE Cliente on Cliente.LOC_CODIGO = Localidade.LOC_CODIGO
                                    JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CLI_CODIGO_EXPEDIDOR = Cliente.CLI_CGCCPF 
                                    WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO), '') UFExpedidor, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains(" Destinatario,"))
                    {
                        select += "CargaDadosSumarizados.CDS_DESTINATARIOS Destinatario, ";
                        groupBy += "CargaDadosSumarizados.CDS_DESTINATARIOS, ";

                        if (!joins.Contains(" Carga "))
                            joins += " left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "LocalidadeDestinatario":
                    if (!select.Contains("LocalidadeDestinatario,"))
                    {
                        select += "CargaDadosSumarizados.CDS_DESTINOS LocalidadeDestinatario, ";
                        groupBy += "CargaDadosSumarizados.CDS_DESTINOS, ";

                        if (!joins.Contains(" Carga "))
                            joins += " left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "ValorICMS":
                    if (!select.Contains("ValorICMS,"))
                    {
                        select += "Carga.CAR_VALOR_ICMS ValorICMS, ";
                        groupBy += "Carga.CAR_VALOR_ICMS, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";
                    }
                    break;
                case "ValorFrete":
                    if (!select.Contains("ValorFrete,"))
                    {
                        select += "Carga.CAR_VALOR_FRETE_LIQUIDO ValorFrete, ";
                        groupBy += "Carga.CAR_VALOR_FRETE_LIQUIDO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";
                    }
                    break;
                case "ValorReceber":
                    if (!select.Contains("ValorReceber,"))
                    {
                        select += "Carga.CAR_VALOR_FRETE_PAGAR ValorReceber, ";
                        groupBy += "Carga.CAR_VALOR_FRETE_PAGAR, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";
                    }
                    break;
                case "PercentualAdiantamento":
                    if (!select.Contains("PercentualAdiantamento,"))
                    {
                        select += "Contrato.CFT_PERCENTUAL_ADIANTAMENTO PercentualAdiantamento, ";
                        groupBy += "Contrato.CFT_PERCENTUAL_ADIANTAMENTO, ";
                    }
                    break;
                case "ValorAdiantamento":
                    if (!select.Contains("ValorAdiantamento,"))
                    {
                        select += "Contrato.CFT_ADIANTAMENTO ValorAdiantamento, ";

                        if (!groupBy.Contains("Contrato.CFT_ADIANTAMENTO"))
                            groupBy += "Contrato.CFT_ADIANTAMENTO, ";
                    }
                    break;
                case "PercentualAbastecimento":
                    if (!select.Contains("PercentualAbastecimento,"))
                    {
                        select += "Contrato.CFT_PERCENTUAL_ABASTECIMENTO PercentualAbastecimento, ";

                        if (!groupBy.Contains("Contrato.CFT_PERCENTUAL_ABASTECIMENTO"))
                            groupBy += "Contrato.CFT_PERCENTUAL_ABASTECIMENTO, ";
                    }
                    break;
                case "ValorAbastecimento":
                    if (!select.Contains("ValorAbastecimento,"))
                    {
                        select += "Contrato.CFT_ABASTECIMENTO ValorAbastecimento, ";

                        if (!groupBy.Contains("Contrato.CFT_ABASTECIMENTO"))
                            groupBy += "Contrato.CFT_ABASTECIMENTO, ";
                    }
                    break;
                case "ValorFreteMenosAbastecimento":
                    if (!select.Contains("ValorFreteMenosAbastecimento,"))
                    {
                        select += "(Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO - ISNULL(Contrato.CFT_ABASTECIMENTO, 0)) ValorFreteMenosAbastecimento, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";

                        if (!groupBy.Contains("Contrato.CFT_ABASTECIMENTO"))
                            groupBy += "Contrato.CFT_ABASTECIMENTO, ";
                    }
                    break;
                case "ValorPago":
                    if (!select.Contains("ValorPago,"))
                    {
                        select += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO ValorPago, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";
                    }
                    break;
                case "ValorBruto":
                    if (!select.Contains("ValorBruto,"))
                    {
                        select += @"(Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO
                                    - ISNULL((SELECT SUM(CFV_VALOR) FROM T_CONTRATO_FRETE_TERCEIRO_VALOR WHERE CFT_CODIGO = Contrato.CFT_CODIGO AND CFV_TIPO_JUSTIFICATIVA = 2), 0)
                                    + ISNULL((SELECT SUM(CFV_VALOR) FROM T_CONTRATO_FRETE_TERCEIRO_VALOR WHERE CFT_CODIGO = Contrato.CFT_CODIGO AND CFV_TIPO_JUSTIFICATIVA = 1), 0)) ValorBruto, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "ValorSaldo":
                    if (!select.Contains("ValorSaldo,"))
                    {
                        select += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO - (CASE WHEN Contrato.CFT_RETER_IMPOSTOS_CONTRATO_FRETE = 1 THEN (CASE WHEN Contrato.CFT_VALOR_IRRF_PERIODO >= 10 THEN Contrato.CFT_VALOR_IRRF WHEN Contrato.CFT_VALOR_IRRF_PERIODO <= 10 AND (Contrato.CFT_VALOR_IRRF_PERIODO + Contrato.CFT_VALOR_IRRF) >= 10 THEN Contrato.CFT_VALOR_IRRF_PERIODO + Contrato.CFT_VALOR_IRRF ELSE 0 END) + Contrato.CFT_VALOR_INSS + Contrato.CFT_VALOR_SENAT + Contrato.CFT_VALOR_SEST ELSE 0 END) - Contrato.CFT_ADIANTAMENTO - (CASE TipoIntegracaoValePedagio.TPI_NAO_SUBTRAIR_VALE_PEDAGIO_DO_CONTRATO WHEN 0 THEN Contrato.CFT_VALOR_PEDAGIO ELSE 0 END) + Contrato.CFT_VALOR_TOTAL_ACRESCIMO_SALDO - Contrato.CFT_VALOR_TOTAL_DESCONTO_SALDO + Contrato.CFT_VALOR_TOTAL_ACRESCIMO_ADIANTAMENTO - Contrato.CFT_VALOR_TOTAL_DESCONTO_ADIANTAMENTO ValorSaldo, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy += "Contrato.CFT_VALOR_IRRF, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF_PERIODO"))
                            groupBy += "Contrato.CFT_VALOR_IRRF_PERIODO, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_TOTAL_DESCONTO_SALDO"))
                            groupBy += "Contrato.CFT_VALOR_TOTAL_DESCONTO_SALDO, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_TOTAL_ACRESCIMO_SALDO"))
                            groupBy += "Contrato.CFT_VALOR_TOTAL_ACRESCIMO_SALDO, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_PEDAGIO"))
                            groupBy += "Contrato.CFT_VALOR_PEDAGIO, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";

                        if (!groupBy.Contains("Contrato.CFT_ADIANTAMENTO"))
                            groupBy += "Contrato.CFT_ADIANTAMENTO, ";

                        if (!groupBy.Contains("Contrato.CFT_RETER_IMPOSTOS_CONTRATO_FRETE"))
                            groupBy += "Contrato.CFT_RETER_IMPOSTOS_CONTRATO_FRETE, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_INSS"))
                            groupBy += "Contrato.CFT_VALOR_INSS, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_SEST"))
                            groupBy += "Contrato.CFT_VALOR_SEST, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_SENAT"))
                            groupBy += "Contrato.CFT_VALOR_SENAT, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy += "Contrato.CFT_VALOR_IRRF, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy += "Contrato.CFT_VALOR_IRRF, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_TOTAL_ACRESCIMO_ADIANTAMENTO"))
                            groupBy += "Contrato.CFT_VALOR_TOTAL_ACRESCIMO_ADIANTAMENTO, ";

                        if (!groupBy.Contains("TipoIntegracaoValePedagio.TPI_NAO_SUBTRAIR_VALE_PEDAGIO_DO_CONTRATO"))
                            groupBy += "TipoIntegracaoValePedagio.TPI_NAO_SUBTRAIR_VALE_PEDAGIO_DO_CONTRATO, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_TOTAL_DESCONTO_ADIANTAMENTO"))
                            groupBy += "Contrato.CFT_VALOR_TOTAL_DESCONTO_ADIANTAMENTO, ";

                        if (!joins.Contains(" TipoIntegracaoValePedagio "))
                            joins += "left outer join T_TIPO_INTEGRACAO TipoIntegracaoValePedagio on Contrato.TPI_CODIGO_VALE_PEDAGIO = TipoIntegracaoValePedagio.TPI_CODIGO ";
                    }
                    break;
                case "ValorINSS":
                    if (!select.Contains("ValorINSS,"))
                    {
                        select += "Contrato.CFT_VALOR_INSS ValorINSS, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_INSS"))
                            groupBy += "Contrato.CFT_VALOR_INSS, ";
                    }
                    break;
                case "ValorIRRF":
                    if (!select.Contains("ValorIRRF,"))
                    {
                        select += "Contrato.CFT_VALOR_IRRF ValorIRRF, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy += "Contrato.CFT_VALOR_IRRF, ";
                    }
                    break;
                case "ValorSEST":
                    if (!select.Contains("ValorSEST,"))
                    {
                        select += "Contrato.CFT_VALOR_SEST ValorSEST, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_SEST"))
                            groupBy += "Contrato.CFT_VALOR_SEST, ";
                    }
                    break;
                case "ValorSENAT":
                    if (!select.Contains("ValorSENAT,"))
                    {
                        select += "Contrato.CFT_VALOR_SENAT ValorSENAT, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_SENAT"))
                            groupBy += "Contrato.CFT_VALOR_SENAT, ";
                    }
                    break;
                case "BCICMS":
                    if (!select.Contains("BCICMS,"))
                    {
                        select += @"ISNULL((SELECT SUM(CTe.CON_BC_ICMS) FROM T_CARGA_CTE CargaCTe 
                            JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO
                            WHERE CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({ string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D"))) }) " : string.Empty;
                        select += " ), 0) BCICMS, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "AliquotaICMS":
                    if (!select.Contains("AliquotaICMS,"))
                    {
                        select += @"ISNULL((SELECT MAX(CTe.CON_ALIQ_ICMS) FROM T_CARGA_CTE CargaCTe 
                            JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO
                            WHERE CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({ string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D"))) }) " : string.Empty;
                        select += " ), 0) AliquotaICMS, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "PesoKg":
                    if (!select.Contains("PesoKg,"))
                    {
                        select += @"ISNULL((SELECT SUM(CTe.CON_PESO) FROM T_CARGA_CTE CargaCTe 
                            JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO
                            WHERE CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({ string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D"))) }) " : string.Empty;
                        select += " ), 0) PesoKg, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "ValorFreteNegociado":
                    if (!select.Contains("ValorFreteNegociado,"))
                    {
                        select += @"ISNULL((SELECT MAX(Pedido.PED_VALOR_FRETE_TONELADA_NEGOCIADO) FROM T_CARGA_PEDIDO CargaPedido 
                            JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO), 0 ) ValorFreteNegociado, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "ValorFreteTerceiro":
                    if (!select.Contains("ValorFreteTerceiro,"))
                    {
                        select += @"ISNULL((SELECT MAX(Pedido.PED_VALOR_FRETE_TONELADA_TERCEIRO) FROM T_CARGA_PEDIDO CargaPedido 
                            JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO), 0) ValorFreteTerceiro, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "ProdutoPredominante":
                    if (!select.Contains("ProdutoPredominante,"))
                    {
                        select += @"ISNULL((SELECT MAX(CTe.CON_PRODUTO_PRED) FROM T_CARGA_CTE CargaCTe 
                            JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO
                            WHERE CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({ string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D"))) }) " : string.Empty;
                        select += "), '') ProdutoPredominante, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "NumeroValePedagio":
                    if (!select.Contains("NumeroValePedagio,"))
                    {
                        select += @"(ISNULL((  
                                            SELECT MAX(ValePedagio.CVP_NUMERO_VALE_PEDAGIO)
                                            FROM T_CARGA_INTEGRACAO_VALE_PEDAGIO ValePedagio
                                            WHERE ValePedagio.CAR_CODIGO = Contrato.CAR_CODIGO), 
   
                                            (SELECT MAX(ValePedagio.CVP_NUMERO_COMPROVANTE)
                                            FROM T_CARGA_VALE_PEDAGIO ValePedagio
                                            WHERE ValePedagio.CAR_CODIGO = Contrato.CAR_CODIGO)   
                                           )) NumeroValePedagio, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "ValorPedagioManual":
                    if (!select.Contains(" ValorPedagioManual, "))
                    {
                        select += @"(SELECT SUM(ValePedagio.CVP_VALOR) FROM T_CARGA_VALE_PEDAGIO ValePedagio WHERE ValePedagio.CAR_CODIGO = Contrato.CAR_CODIGO) ValorPedagioManual, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "ValorAcrescimos":
                    if (!select.Contains("ValorAcrescimos,"))
                    {
                        select += "(SELECT SUM(CFV_VALOR) FROM T_CONTRATO_FRETE_TERCEIRO_VALOR WHERE CFT_CODIGO = Contrato.CFT_CODIGO AND CFV_TIPO_JUSTIFICATIVA = 2) ValorAcrescimos, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "TotalDescontos":
                    if (!select.Contains("TotalDescontos,"))
                    {
                        select += "ISNULL(Contrato.CFT_TARIFA_SAQUE, 0) + ISNULL(Contrato.CFT_TARIFA_TRANSFERENCIA, 0) + ISNULL(Contrato.CFT_DESCONTO, 0) TotalDescontos, ";

                        if (!groupBy.Contains("Contrato.CFT_TARIFA_SAQUE"))
                            groupBy += "Contrato.CFT_TARIFA_SAQUE, ";
                        if (!groupBy.Contains("Contrato.CFT_TARIFA_TRANSFERENCIA"))
                            groupBy += "Contrato.CFT_TARIFA_TRANSFERENCIA, ";
                        if (!groupBy.Contains("Contrato.CFT_DESCONTO"))
                            groupBy += "Contrato.CFT_DESCONTO, ";
                    }
                    break;
                case "SaldoFrete":
                    if (!select.Contains("Impostos,"))
                    {
                        select += "ISNULL(Contrato.CFT_VALOR_INSS, 0) + ISNULL(Contrato.CFT_VALOR_IRRF, 0) + ISNULL(Contrato.CFT_VALOR_SEST, 0) + ISNULL(Contrato.CFT_VALOR_SENAT, 0) Impostos, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_INSS"))
                            groupBy += "Contrato.CFT_VALOR_INSS, ";
                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy += "Contrato.CFT_VALOR_IRRF, ";
                        if (!groupBy.Contains("Contrato.CFT_VALOR_SEST"))
                            groupBy += "Contrato.CFT_VALOR_SEST, ";
                        if (!groupBy.Contains("Contrato.CFT_VALOR_SENAT"))
                            groupBy += "Contrato.CFT_VALOR_SENAT, ";
                    }
                    if (!select.Contains("ValorPago,"))
                    {
                        select += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO ValorPago, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";
                    }
                    if (!select.Contains("TotalDescontos,"))
                    {
                        select += "ISNULL(Contrato.CFT_TARIFA_SAQUE, 0) + ISNULL(Contrato.CFT_TARIFA_TRANSFERENCIA, 0) + ISNULL(Contrato.CFT_DESCONTO, 0) TotalDescontos, ";

                        if (!groupBy.Contains("Contrato.CFT_TARIFA_SAQUE"))
                            groupBy += "Contrato.CFT_TARIFA_SAQUE, ";
                        if (!groupBy.Contains("Contrato.CFT_TARIFA_TRANSFERENCIA"))
                            groupBy += "Contrato.CFT_TARIFA_TRANSFERENCIA, ";
                        if (!groupBy.Contains("Contrato.CFT_DESCONTO"))
                            groupBy += "Contrato.CFT_DESCONTO, ";
                    }
                    if (!select.Contains("ValorAdiantamento,"))
                    {
                        select += "Contrato.CFT_ADIANTAMENTO ValorAdiantamento, ";

                        if (!groupBy.Contains("Contrato.CFT_ADIANTAMENTO"))
                            groupBy += "Contrato.CFT_ADIANTAMENTO, ";
                    }
                    break;
                case "ValorDescontos":
                    if (!select.Contains("ValorDescontos,"))
                    {
                        select += "(SELECT SUM(CFV_VALOR) FROM T_CONTRATO_FRETE_TERCEIRO_VALOR WHERE CFT_CODIGO = Contrato.CFT_CODIGO AND CFV_TIPO_JUSTIFICATIVA = 1) ValorDescontos, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataEmissao":
                    if (!select.Contains("DataEmissao,"))
                    {
                        select += "Contrato.CFT_DATA_EMISSAO_CONTRATO DataEmissao, ";
                        groupBy += "Contrato.CFT_DATA_EMISSAO_CONTRATO, ";
                    }
                    break;
                case "DataEncerramento":
                    if (!select.Contains("DataEncerramento,"))
                    {
                        select += "CONVERT(NVARCHAR(10), Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO, 3) DataEncerramento, ";
                        groupBy += "Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO, ";
                    }
                    break;
                case "DataAutorizacaoPagamento":
                    if (!select.Contains("DataAutorizacaoPagamento,"))
                    {
                        select += "(select TOP 1 CONVERT(NVARCHAR(10), CIOT.CIO_DATA_AUTORIZACAO_PAGAMENTO, 3) from T_CARGA_CIOT CargaCIOT inner join T_CIOT CIOT on CargaCIOT.CIO_CODIGO = CIOT.CIO_CODIGO WHERE CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO) DataAutorizacaoPagamento, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataVencimentoAdiantamento":
                    if (!select.Contains("DataVencimentoAdiantamento,"))
                    {
                        select += "(select top(1) CASE TIT_DATA_VENCIMENTO WHEN NULL THEN '' ELSE convert(nvarchar(10), TIT_DATA_VENCIMENTO, 3) END from T_TITULO where CFT_CODIGO = Contrato.CFT_CODIGO and TIT_SEQUENCIA = 1 AND TIT_STATUS = 3) DataVencimentoAdiantamento, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataPagamentoAdiantamento":
                    if (!select.Contains("DataPagamentoAdiantamento,"))
                    {
                        select += "(select top(1) CASE TIT_DATA_LIQUIDACAO WHEN NULL THEN '' ELSE convert(nvarchar(10), TIT_DATA_LIQUIDACAO, 3) + ' ' + convert(nvarchar(10), TIT_DATA_LIQUIDACAO, 108) END from T_TITULO where CFT_CODIGO = Contrato.CFT_CODIGO and TIT_SEQUENCIA = 1 AND TIT_STATUS = 3) DataPagamentoAdiantamento, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataVencimentoValor":
                    if (!select.Contains("DataVencimentoValor,"))
                    {
                        select += "(select top(1) CASE TIT_DATA_VENCIMENTO WHEN NULL THEN '' ELSE convert(nvarchar(10), TIT_DATA_VENCIMENTO, 3) END from T_TITULO where CFT_CODIGO = Contrato.CFT_CODIGO and TIT_SEQUENCIA = 2 AND TIT_STATUS = 3) DataVencimentoValor, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataPagamentoValor":
                    if (!select.Contains("DataPagamentoValor,"))
                    {
                        select += "(select top(1) CASE TIT_DATA_LIQUIDACAO WHEN NULL THEN '' ELSE convert(nvarchar(10), TIT_DATA_LIQUIDACAO, 3) + ' ' + convert(nvarchar(10), TIT_DATA_LIQUIDACAO, 108) END from T_TITULO where CFT_CODIGO = Contrato.CFT_CODIGO and TIT_SEQUENCIA = 2 AND TIT_STATUS = 3) DataPagamentoValor, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "Titulos":
                    if (!select.Contains("Titulos,"))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + 
                                                               CONVERT(NVARCHAR(50), titulo.TIT_CODIGO) + 
                                                               ' (' + 
                                                               (CASE titulo.TIT_STATUS WHEN 1 THEN 'Aberto' WHEN 2 THEN 'Atrasado' WHEN 3 THEN 'Quitado' WHEN 4 THEN 'Cancelado' WHEN 5 THEN 'Em Negociação' WHEN 6 THEN 'Bloqueado' ELSE '' END) + 
                                                               ')'
                                               FROM T_TITULO titulo 
                                               WHERE titulo.CFT_CODIGO = Contrato.CFT_CODIGO FOR XML PATH('')), 3, 1000) Titulos, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "CNPJEmpresa":
                    if (!select.Contains(" CNPJEmpresa, "))
                    {
                        select += "Empresa.EMP_CNPJ CNPJEmpresa, ";
                        groupBy += "Empresa.EMP_CNPJ, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";
                    }
                    break;
                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        select += "Empresa.EMP_RAZAO Empresa, ";
                        groupBy += "Empresa.EMP_RAZAO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";
                    }
                    break;
                case "CPFCNPJFilialEmissoraFormatado":
                    if (!select.Contains(" CPFCNPJFilialEmissora, "))
                    {
                        select += "EmpresaFilialEmissora.EMP_CNPJ CPFCNPJFilialEmissora, ";
                        groupBy += "EmpresaFilialEmissora.EMP_CNPJ, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" EmpresaFilialEmissora "))
                            joins += "left outer join T_EMPRESA EmpresaFilialEmissora on EmpresaFilialEmissora.EMP_CODIGO = Carga.EMP_CODIGO_FILIAL_EMISSORA ";
                    }
                    break;
                case "FilialEmissora":
                    if (!select.Contains(" FilialEmissora, "))
                    {
                        select += "EmpresaFilialEmissora.EMP_RAZAO FilialEmissora, ";
                        groupBy += "EmpresaFilialEmissora.EMP_RAZAO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" EmpresaFilialEmissora "))
                            joins += "left outer join T_EMPRESA EmpresaFilialEmissora on EmpresaFilialEmissora.EMP_CODIGO = Carga.EMP_CODIGO_FILIAL_EMISSORA ";
                    }
                    break;
                case "UFFilialEmissora":
                    if (!select.Contains(" UFFilialEmissora, "))
                    {
                        select += "LocalidadeFilialEmissora.UF_SIGLA UFFilialEmissora, ";
                        groupBy += "LocalidadeFilialEmissora.UF_SIGLA, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" EmpresaFilialEmissora "))
                            joins += "left outer join T_EMPRESA EmpresaFilialEmissora on EmpresaFilialEmissora.EMP_CODIGO = Carga.EMP_CODIGO_FILIAL_EMISSORA ";

                        if (!joins.Contains(" LocalidadeFilialEmissora "))
                            joins += "left outer join T_LOCALIDADES LocalidadeFilialEmissora on LocalidadeFilialEmissora.LOC_CODIGO = EmpresaFilialEmissora.LOC_CODIGO ";
                    }
                    break;
                case "DescricaoSituacaoContratoFrete":
                    if (!select.Contains(" SituacaoContratoFrete, "))
                    {
                        select += "Contrato.CFT_CONTRATO_FRETE SituacaoContratoFrete, ";
                        groupBy += "Contrato.CFT_CONTRATO_FRETE, ";
                    }
                    break;
                case "DataAprovacaoFormatada":
                    if (!select.Contains(" DataAprovacao, "))
                    {
                        select += "(SELECT top 1 AAC_DATA FROM T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO WHERE CFT_CODIGO = Contrato.CFT_CODIGO AND AAC_SITUACAO = 1 ORDER BY AAC_CODIGO DESC) DataAprovacao, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select += "TipoOperacao.TOP_DESCRICAO TipoOperacao, ";
                        groupBy += "TipoOperacao.TOP_DESCRICAO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" TipoOperacao "))
                            joins += "left outer join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ";
                    }
                    break;
                case "DataAberturaCIOT":
                    if (!select.Contains(" DataAberturaCIOT, "))
                    {
                        select += "(select TOP 1 CONVERT(NVARCHAR(10), CIOT.CIO_DATA_ABERTURA, 3) from T_CARGA_CIOT CargaCIOT inner join T_CIOT CIOT on CargaCIOT.CIO_CODIGO = CIOT.CIO_CODIGO WHERE CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO) DataAberturaCIOT, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataEncerramentoCIOT":
                    if (!select.Contains(" DataEncerramentoCIOT, "))
                    {
                        select += "(select TOP 1 CONVERT(NVARCHAR(10), CIOT.CIO_DATA_ENCERRAMENTO, 3) from T_CARGA_CIOT CargaCIOT inner join T_CIOT CIOT on CargaCIOT.CIO_CODIGO = CIOT.CIO_CODIGO WHERE CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO) DataEncerramentoCIOT, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "PercentualVariacao":
                    if (!select.Contains(" PercentualVariacao, "))
                    {
                        select += @"CASE WHEN (Carga.CAR_VALOR_FRETE > 0 
                                            AND (ConfiguracaoEmbarcador.CEM_EXIBIR_VARIACAO_NEGATIVA_CONTRATO_FRETE_TERCEIRO = 1 OR ((Carga.CAR_VALOR_FRETE - Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO) > 0)))
                                        THEN CAST(round((((Carga.CAR_VALOR_FRETE - Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO) * 100) / Carga.CAR_VALOR_FRETE), 0) AS INT)
                                    ELSE 0 END PercentualVariacao, ";

                        groupBy += "ConfiguracaoEmbarcador.CEM_EXIBIR_VARIACAO_NEGATIVA_CONTRATO_FRETE_TERCEIRO, ";
                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";

                        if (!groupBy.Contains("Carga.CAR_VALOR_FRETE"))
                            groupBy += "Carga.CAR_VALOR_FRETE, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";
                    }
                    break;
                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + centroResultado.CRE_DESCRICAO 
                                                FROM T_CENTRO_RESULTADO centroResultado
                                                inner join T_PEDIDO pedido on pedido.CRE_CODIGO = centroResultado.CRE_CODIGO
                                                inner join T_CARGA_PEDIDO cargaPedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                                                WHERE cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) CentroResultado, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "ValorPedagio":
                    if (!select.Contains(" ValorPedagio, "))
                    {
                        select += "Contrato.CFT_VALOR_PEDAGIO ValorPedagio, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_PEDAGIO"))
                            groupBy += "Contrato.CFT_VALOR_PEDAGIO, ";
                    }
                    break;

                case "ProtocoloCIOT":
                    if (!select.Contains(" ProtocoloCIOT, "))
                    {
                        select += "CIOT.CIO_PROTOCOLO_AUTORIZACAO ProtocoloCIOT, ";
                        groupBy += "CIOT.CIO_PROTOCOLO_AUTORIZACAO, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;

                case "CodigoVerificadorCIOT":
                    if (!select.Contains(" CodigoVerificadorCIOT, "))
                    {
                        select += "CIOT.CIO_CODIGO_VERIFICADOR CodigoVerificadorCIOT, ";
                        groupBy += "CIOT.CIO_CODIGO_VERIFICADOR, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador,"))
                    {
                        select += "substring(( ";
                        select += "    select distinct ',' + ";
                        select += "           case CargaPedido.PED_TIPO_TOMADOR ";
                        select += "               when 0 then Remetente.CLI_NOME ";
                        select += "               when 1 then Expedidor.CLI_NOME ";
                        select += "               when 2 then Recebedor.CLI_NOME ";
                        select += "               when 3 then Destinatario.CLI_NOME ";
                        select += "               when 4 then Tomador.CLI_NOME ";
                        select += "               else '' ";
                        select += "           end ";
                        select += "     from T_CARGA_PEDIDO CargaPedido ";
                        select += "         join T_PEDIDO Pedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ";
                        select += "         left join T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR ";
                        select += "         left join T_CLIENTE Recebedor on Recebedor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR ";
                        select += "         left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = CargaPedido.CLI_CODIGO_TOMADOR ";
                        select += "         left join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ";
                        select += "         left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ";
                        select += "     where CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += "     for xml path('') ";
                        select += "), 2, 500) Tomador, ";

                        if (!groupBy.Contains("Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;

                case "CPFCNPJTomadorFormatado":
                    if (!select.Contains("CPFCNPJTomador,"))
                    {
                        select += "substring(( ";
                        select += "    select distinct ',' + ";
                        select += "            case CargaPedido.PED_TIPO_TOMADOR ";
                        select += "                when 0 then CONVERT(VARCHAR,CAST(Remetente.CLI_CGCCPF AS DECIMAL))";
                        select += "                when 1 then CONVERT(VARCHAR,CAST(Expedidor.CLI_CGCCPF AS DECIMAL)) ";
                        select += "                when 2 then CONVERT(VARCHAR,CAST(Recebedor.CLI_CGCCPF AS DECIMAL)) ";
                        select += "                when 3 then CONVERT(VARCHAR,CAST(Destinatario.CLI_CGCCPF AS DECIMAL)) ";
                        select += "                when 4 then CONVERT(VARCHAR,CAST(Tomador.CLI_CGCCPF AS DECIMAL)) ";
                        select += "                else '' ";
                        select += "            end ";
                        select += "    from T_CARGA_PEDIDO CargaPedido ";
                        select += "        join T_PEDIDO Pedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ";
                        select += "        left join T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR ";
                        select += "        left join T_CLIENTE Recebedor on Recebedor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR ";
                        select += "        left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = CargaPedido.CLI_CODIGO_TOMADOR ";
                        select += "        left join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ";
                        select += "        left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ";
                        select += "    where CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += "    for xml path('') ";
                        select += "), 2, 500) CPFCNPJTomador, ";

                        if (!groupBy.Contains("Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;

                case "ValorLiquidoSemAdiantamento":
                    SetarSelectRelatorioFreteTerceirizado("ValorPago", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizado("TotalDescontos", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizado("ValorIRRF", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizado("ValorINSS", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizado("ValorSEST", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizado("ValorSENAT", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    break;

                case "ValorSubcontratacao":
                    SetarSelectRelatorioFreteTerceirizado("ValorBruto", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizado("ValorPedagio", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    break;

                case "SituacaoCargaDescricao":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select += "Carga.CAR_SITUACAO SituacaoCarga, ";

                        if (!groupBy.Contains("Carga.CAR_SITUACAO"))
                            groupBy += "Carga.CAR_SITUACAO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "LocalidadeEmpresaFilial":
                    if (!select.Contains(" LocalidadeEmpresaFilial, "))
                    {
                        select += "(LocalidadeEmpresa.LOC_DESCRICAO + ' - ' + LocalidadeEmpresa.UF_SIGLA) LocalidadeEmpresaFilial, ";

                        if (!groupBy.Contains("LocalidadeEmpresa.LOC_DESCRICAO"))
                            groupBy += "LocalidadeEmpresa.LOC_DESCRICAO, ";

                        if (!groupBy.Contains("LocalidadeEmpresa.UF_SIGLA"))
                            groupBy += "LocalidadeEmpresa.UF_SIGLA, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";

                        if (!joins.Contains(" LocalidadeEmpresa "))
                            joins += "left outer join T_LOCALIDADES LocalidadeEmpresa on LocalidadeEmpresa.LOC_CODIGO = Empresa.LOC_CODIGO ";
                    }
                    break;

                case "ValorReceberCTes":
                    if (!select.Contains(" ValorReceberCTes, "))
                    {
                        select += "(SELECT SUM(CON_VALOR_RECEBER) FROM T_CARGA_CTE CargaCTe INNER JOIN T_CTE CTe ON CTe.CON_CODIGO = CargaCTe.CON_CODIGO WHERE CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO AND CargaCTe.CON_CODIGO IS NOT NULL ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({ string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D"))) }) " : string.Empty;
                        select += " ) ValorReceberCTes, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "NumeroDocAnterior":
                    if (!select.Contains(" NumeroDocAnterior, "))
                        select += @"SUBSTRING(
                               (SELECT DISTINCT ', ' + CAST(cteT.CPS_NUMERO AS NVARCHAR(20))
                                FROM T_CARGA_PEDIDO cargaPedido
                                INNER JOIN T_PEDIDO_CTE_PARA_SUB_CONTRATACAO cte ON cte.CPE_CODIGO = cargaPedido.CPE_CODIGO
                                INNER JOIN T_CTE_TERCEIRO cteT on cteT.CPS_CODIGO = cte.CPS_CODIGO
                                WHERE cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO                      
                                  FOR XML PATH('')), 3, 1000) NumeroDocAnterior, ";
                    if (!joins.Contains(" Carga "))
                        joins += "LEFT OUTER JOIN T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";

                    if (!groupBy.Contains("Carga.CAR_CODIGO"))
                        groupBy += "Carga.CAR_CODIGO, ";

                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                        select += @" TipoCarga.TCG_DESCRICAO TipoCarga, ";

                    if (!joins.Contains(" Carga "))
                        joins += "LEFT OUTER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";

                    if (!joins.Contains(" TipoCarga "))
                        joins += "LEFT OUTER JOIN T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ";

                    if (!groupBy.Contains("TipoCarga.TCG_DESCRICAO"))
                        groupBy += "TipoCarga.TCG_DESCRICAO, ";
                    break;

                case "DataCargaFormatada":
                    if (!select.Contains(" DataCarga, "))
                        select += @"Carga.CAR_DATA_CRIACAO DataCarga, ";
                    if (!joins.Contains(" Carga "))
                        joins += "LEFT OUTER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    if (!groupBy.Contains("Carga.CAR_DATA_CRIACAO"))
                        groupBy += "Carga.CAR_DATA_CRIACAO, ";
                    break;

                case "ValorTotalProdutosNotaFiscal":
                    if (!select.Contains("ValorTotalProdutosNotaFiscal, "))
                    {
                        select += @"(SELECT SUM(NF_VALOR) FROM T_XML_NOTA_FISCAL XMLNotaFiscal 
                                    INNER JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal ON PedidoXMLNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO
                                    INNER JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                                    ) ValorTotalProdutosNotaFiscal, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "Banco":
                    if (!select.Contains(" Banco,"))
                    {
                        select += "Banco.BCO_DESCRICAO Banco, ";
                        groupBy += "Banco.BCO_DESCRICAO , ";


                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";

                        if (!joins.Contains(" Banco "))
                        {
                            joins += "left outer join T_BANCO Banco on Banco.BCO_CODIGO = Terceiro.BCO_CODIGO ";
                        }
                    }
                    break;
                case "Agencia":
                    if (!select.Contains(" Agencia,"))
                    {
                        select += "Terceiro.CLI_BANCO_AGENCIA + '-' + Terceiro.CLI_BANCO_DIGITO_AGENCIA Agencia, ";
                        groupBy += "Terceiro.CLI_BANCO_AGENCIA , Terceiro.CLI_BANCO_DIGITO_AGENCIA ,  ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO  ";
                    }
                    break;
                case "Conta":
                    if (!select.Contains(" Conta,"))
                    {
                        select += "Terceiro.CLI_BANCO_NUMERO_CONTA Conta, ";
                        groupBy += "Terceiro.CLI_BANCO_NUMERO_CONTA , ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO  ";
                    }
                    break;
                case "TipoDaConta":
                    if (!select.Contains(" TipoDaConta,"))
                    {
                        select += "Terceiro.CLI_BANCO_TIPO_CONTA TipoDaConta, ";
                        groupBy += "Terceiro.CLI_BANCO_TIPO_CONTA , ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO  ";
                    }
                    break;
                case "Titular":
                    if (!select.Contains(" Titular,"))
                    {
                        select += "Terceiro.CLI_NOME Titular, ";
                        groupBy += "Terceiro.CLI_NOME , ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO  ";
                    }
                    break;
                case "CodigoRetornoST":
                    if (!select.Contains(" CodigoRetornoST,"))
                    {
                        select += "(SELECT CCA_PROTOCOLO AS CodigoRetornoST  \r\nFROM T_CARGA_CARGA_INTEGRACAO TCCI \r\nWHERE TCCI.CAR_CODIGO = Contrato.CAR_CODIGO \r\nAND TPI_CODIGO = 16) CodigoRetornoST, ";

                        if (!groupBy.Contains("Contrato.CAR_CODIGO"))
                        {
                            groupBy += "Contrato.CAR_CODIGO, ";
                        }
                    }
                    break;
                case "CodigoRetornoSUFormatado":
                    if (!select.Contains(" CodigoRetornoSU,"))
                    {
                        select += "(SELECT TOP 1 INT_PROBLEMA_INTEGRACAO  AS CodigoRetornoSU\r\nFROM T_CARGA_REGISTRO_ENCERRAMENTO_CARGA_INTEGRACAO  TCRECI \r\nINNER JOIN T_CARGA_REGISTRO_ENCERRAMENTO TCRE \r\nON TCRE.CRE_CODIGO = TCRECI.CRE_CODIGO \r\nWHERE TCRE.CAR_CODIGO = Contrato.CAR_CODIGO ) CodigoRetornoSU, ";

                        if (!groupBy.Contains("Contrato.CAR_CODIGO")) 
                        {
                            groupBy += "Contrato.CAR_CODIGO, ";
                        }
                            
                    }
                    break;
                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao,"))
                    {
                        select += "Terceiro.CLI_CODIGO_INTEGRACAO  CodigoIntegracao, ";
                        groupBy += "Terceiro.CLI_CODIGO_INTEGRACAO , ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO  ";
                    }
                    break;
                case "OperadoraDescricao":
                    if (!select.Contains(" Operadora, "))
                    {
                        select += "CIOT.CIO_OPERADORA Operadora, ";
                        groupBy += "CIOT.CIO_OPERADORA, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;

                default:
                    if (!count && propriedade.Contains("ValorComponente"))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";

                        select += "SUM(" + propriedade + ".CCF_VALOR_COMPONENTE) " + propriedade + ", ";

                        joins += "left outer join T_CARGA_COMPONENTES_FRETE " + propriedade + " on Carga.CAR_CODIGO = " + propriedade + ".CAR_CODIGO and " + propriedade + ".CFR_CODIGO = " + codigoDinamico + " ";
                    }
                    break;
                case "CentroCustoViagem":
                    if (!select.Contains(" CentroCustoViagem, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + centroCustoViagem.CCV_DESCRICAO 
                                                FROM T_CENTRO_CUSTO_VIAGEM centroCustoViagem
                                                inner join T_PEDIDO pedido on pedido.CCV_CODIGO = centroCustoViagem.CCV_CODIGO
                                                inner join T_CARGA_PEDIDO cargaPedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                                                WHERE cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) centroCustoViagem, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioFreteTerceirizado(ref string where, ref string groupBy, ref string joins, ref List<ParametroSQL> parametros, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizado filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CpfCnpjTerceiros.Count > 0)
                where += $" and Contrato.CLI_CGCCPF_TERCEIRO in({string.Join(",", filtrosPesquisa.CpfCnpjTerceiros)})";

            if (filtrosPesquisa.DataEmissaoContratoInicial != DateTime.MinValue)
                where += " and CAST(Contrato.CFT_DATA_EMISSAO_CONTRATO AS DATE) >= '" + filtrosPesquisa.DataEmissaoContratoInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataEmissaoContratoFinal != DateTime.MinValue)
                where += " and CAST(Contrato.CFT_DATA_EMISSAO_CONTRATO AS DATE) <= '" + filtrosPesquisa.DataEmissaoContratoFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.NumeroContrato > 0)
                where += " and Contrato.CFT_NUMERO_CONTRATO = " + filtrosPesquisa.NumeroContrato.ToString();

            if (filtrosPesquisa.TiposCargaTerceiros == Dominio.Enumeradores.TiposCargaTerceiros.Terceiros)
                where += "and Carga.CAR_FRETE_TERCEIRO = 1";

            if (filtrosPesquisa.TiposCargaTerceiros == Dominio.Enumeradores.TiposCargaTerceiros.Proprias)
                where += "and (Carga.CAR_FRETE_TERCEIRO = 0 OR Carga.CAR_FRETE_TERCEIRO is null)";

            if (filtrosPesquisa.Empresa > 0)
            {
                where += $" and Carga.EMP_CODIGO = {filtrosPesquisa.Empresa} ";

                if (!joins.Contains(" Carga "))
                    joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where += " and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '" + filtrosPesquisa.NumeroCarga + "'";

                if (!joins.Contains(" Carga "))
                    joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
            }

            if (filtrosPesquisa.NumeroCTe > 0 || filtrosPesquisa.StatusCTe != string.Empty || filtrosPesquisa.TipoCTe.Count > 0) 
            {
                where += " AND EXISTS(select CTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO where CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";
                where += filtrosPesquisa.NumeroCTe > 0 ? " AND CON_NUM = " + filtrosPesquisa.NumeroCTe.ToString() : string.Empty;
                where += filtrosPesquisa.StatusCTe != string.Empty ? $" AND CON_STATUS = :CON_STATUS " : string.Empty;
                where += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CON_TIPO_CTE IN ({ string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D"))) }) " : string.Empty;
                where += " ) ";

                parametros.Add(new ParametroSQL("CON_STATUS", filtrosPesquisa.StatusCTe));
            }

            if (filtrosPesquisa.Veiculo > 0)
                where += $" and (Carga.CAR_VEICULO = {filtrosPesquisa.Veiculo} or exists (select VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS where CAR_CODIGO = Carga.CAR_CODIGO and VEI_CODIGO = {filtrosPesquisa.Veiculo}))";

            if (filtrosPesquisa.ModeloVeicular > 0)
            {
                where += $" and (ModeloVeicular.MVC_CODIGO = {filtrosPesquisa.ModeloVeicular} or exists (select VeiculoVinculado.VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS VeiculoVinculado INNER JOIN T_VEICULO Veiculo ON VeiculoVinculado.VEI_CODIGO = Veiculo.VEI_CODIGO INNER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON Veiculo.MVC_CODIGO = ModeloVeicular.MVC_CODIGO where VeiculoVinculado.CAR_CODIGO = Carga.CAR_CODIGO and ModeloVeicular.MVC_CODIGO = {filtrosPesquisa.ModeloVeicular}))"; // SQL-INJECTION-SAFE

                if (!joins.Contains(" Carga "))
                    joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";

                if (!joins.Contains(" Veiculo "))
                    joins += "left outer join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ";

                if (!joins.Contains(" ModeloVeicular "))
                    joins += "left outer join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO ";
            }

            if (filtrosPesquisa.Situacao?.Count > 0)
                where += $" and Contrato.CFT_CONTRATO_FRETE in ({string.Join(",", filtrosPesquisa.Situacao.Select(o => o.ToString("D")))})";

            if (filtrosPesquisa.DataAprovacaoInicial != DateTime.MinValue)
                where += " and Contrato.CFT_CODIGO in (SELECT CFT_CODIGO FROM T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO WHERE CAST(AAC_DATA AS DATE) >= '" + filtrosPesquisa.DataAprovacaoInicial.ToString(pattern) + "')"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.DataAprovacaoFinal != DateTime.MinValue)
                where += " and Contrato.CFT_CODIGO in (SELECT CFT_CODIGO FROM T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO WHERE CAST(AAC_DATA AS DATE) <= '" + filtrosPesquisa.DataAprovacaoFinal.ToString(pattern) + "')"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.DataEncerramentoInicial != DateTime.MinValue)
                where += " and CAST(Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO AS DATE) >= '" + filtrosPesquisa.DataEncerramentoInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataEncerramentoFinal != DateTime.MinValue)
                where += " and CAST(Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO AS DATE) <= '" + filtrosPesquisa.DataEncerramentoFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.TipoOperacao > 0)
            {
                where += " and Carga.TOP_CODIGO = " + filtrosPesquisa.TipoOperacao.ToString();

                if (!joins.Contains(" Carga "))
                    joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";
            }

            if (filtrosPesquisa.DataAberturaCIOTInicial != DateTime.MinValue || filtrosPesquisa.DataAberturaCIOTFinal != DateTime.MinValue || filtrosPesquisa.DataEncerramentoCIOTInicial != DateTime.MinValue || filtrosPesquisa.DataEncerramentoCIOTFinal != DateTime.MinValue)
                where += $" AND EXISTS (SELECT CFT_CODIGO FROM T_CARGA_CIOT cargaCIOT inner join T_CIOT ciot ON cargaCIOT.CIO_CODIGO = ciot.CIO_CODIGO where cargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO{(filtrosPesquisa.DataAberturaCIOTInicial != DateTime.MinValue ? $" AND ciot.CIO_DATA_ABERTURA >= '{filtrosPesquisa.DataAberturaCIOTInicial.ToString("yyyy-MM-dd")}'" : string.Empty)}{(filtrosPesquisa.DataAberturaCIOTFinal != DateTime.MinValue ? $" AND ciot.CIO_DATA_ABERTURA < '{filtrosPesquisa.DataAberturaCIOTFinal.AddDays(1).ToString("yyyy-MM-dd")}'" : string.Empty)}{(filtrosPesquisa.DataEncerramentoCIOTInicial != DateTime.MinValue ? $" AND ciot.CIO_DATA_ENCERRAMENTO >= '{filtrosPesquisa.DataEncerramentoCIOTInicial.ToString("yyyy-MM-dd")}'" : string.Empty)}{(filtrosPesquisa.DataEncerramentoCIOTFinal != DateTime.MinValue ? $" AND ciot.CIO_DATA_ENCERRAMENTO < '{filtrosPesquisa.DataEncerramentoCIOTFinal.AddDays(1).ToString("yyyy-MM-dd")}'" : string.Empty)}) "; // SQL-INJECTION-SAFE

            //if (filtrosPesquisa.StatusCTe != string.Empty)
            //{
            //    where += $@" AND '{filtrosPesquisa.StatusCTe}' IN(
            //       SELECT 
            //        CTe.CON_STATUS
            //       FROM T_CARGA_CTE CargaCTe 
            //        INNER JOIN T_CTE CTe ON CTe.CON_CODIGO = CargaCTe.CON_CODIGO 
            //       WHERE CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO AND CargaCTe.CON_CODIGO IS NOT NULL) ";

            //    if (!joins.Contains(" Carga "))
            //        joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
            //}

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCIOT))
                where += $" AND EXISTS (SELECT CFT_CODIGO FROM T_CARGA_CIOT CargaCIOT INNER JOIN T_CIOT CIOT ON CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO WHERE CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO AND CIOT.CIO_NUMERO = '{filtrosPesquisa.NumeroCIOT}') ";

            if (filtrosPesquisa.TipoProprietario != null)
            {
                if (!joins.Contains(" Terceiro "))
                    joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";

                if (!joins.Contains(" Modalidade "))
                    joins += "LEFT OUTER JOIN T_CLIENTE_MODALIDADE Modalidade on Modalidade.CPF_CNPJ = Terceiro.CLI_CGCCPF ";

                if (!joins.Contains(" Modalidadetransportador "))
                    joins += "LEFT OUTER JOIN T_CLIENTE_MODALIDADE_TRANSPORTADORAS Modalidadetransportador on Modalidadetransportador.MOD_CODIGO = Modalidade.MOD_CODIGO ";

                where += "and Modalidadetransportador.MOT_TIPO_TRANSPORTADOR = " + (int)filtrosPesquisa.TipoProprietario;
            }
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoPorCTe> ConsultarRelatorioFreteTerceirizadoPorCTe(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioFreteTerceirizadoPorCTe(false, propriedades, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoPorCTe)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoPorCTe>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoPorCTe>> ConsultarRelatorioFreteTerceirizadoPorCTeAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioFreteTerceirizadoPorCTe(false, propriedades, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoPorCTe)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoPorCTe>();
        }

        public int ContarConsultaRelatorioFreteTerceirizadoPorCTe(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioFreteTerceirizadoPorCTe(true, propriedades, filtrosPesquisa, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioFreteTerceirizadoPorCTe(bool count, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioFreteTerceirizadoPorCTe(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count, filtrosPesquisa);

            SetarWhereRelatorioFreteTerceirizadoPorCTe(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioFreteTerceirizadoPorCTe(propAgrupa, 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);

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
                   " from T_CONTRATO_FRETE_TERCEIRO Contrato " + joins + ", T_CONFIGURACAO_EMBARCADOR ConfiguracaoEmbarcador" +
                   " where 1=1" + where +
                   (groupBy.Length > 0 ? " group by " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");
        }

        private void SetarWhereRelatorioFreteTerceirizadoPorCTe(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CpfCnpjTerceiros.Count > 0)
                where += $" and Contrato.CLI_CGCCPF_TERCEIRO in({string.Join(",", filtrosPesquisa.CpfCnpjTerceiros)})";

            if (filtrosPesquisa.DataEmissaoContratoInicial != DateTime.MinValue)
                where += " and CAST(Contrato.CFT_DATA_EMISSAO_CONTRATO AS DATE) >= '" + filtrosPesquisa.DataEmissaoContratoInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataEmissaoContratoFinal != DateTime.MinValue)
                where += " and CAST(Contrato.CFT_DATA_EMISSAO_CONTRATO AS DATE) <= '" + filtrosPesquisa.DataEmissaoContratoFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.NumeroContrato > 0)
                where += " and Contrato.CFT_NUMERO_CONTRATO = " + filtrosPesquisa.NumeroContrato.ToString();

            if (filtrosPesquisa.TiposCargaTerceiros == Dominio.Enumeradores.TiposCargaTerceiros.Terceiros)
                where += "and Carga.CAR_FRETE_TERCEIRO = 1";

            if (filtrosPesquisa.TiposCargaTerceiros == Dominio.Enumeradores.TiposCargaTerceiros.Proprias)
                where += "and (Carga.CAR_FRETE_TERCEIRO = 0 OR Carga.CAR_FRETE_TERCEIRO is null)";

            if (filtrosPesquisa.Empresa > 0)
            {
                where += $" and Carga.EMP_CODIGO = {filtrosPesquisa.Empresa} ";

                if (!joins.Contains(" Carga "))
                    joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where += " and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '" + filtrosPesquisa.NumeroCarga + "'";

                if (!joins.Contains(" Carga "))
                    joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
            }

            if (filtrosPesquisa.NumeroCTe > 0 || filtrosPesquisa.StatusCTe != string.Empty || filtrosPesquisa.TipoCTe.Count > 0)
            {
                where += " AND EXISTS(select CTe.CON_CODIGO from T_CARGA_CTE CargaCTe inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO where CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";
                where += filtrosPesquisa.NumeroCTe > 0 ? " AND CON_NUM = " + filtrosPesquisa.NumeroCTe.ToString() : string.Empty;
                where += filtrosPesquisa.StatusCTe != string.Empty ? $" AND CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                where += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CON_TIPO_CTE IN ({ string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D"))) }) " : string.Empty;
                where += " ) ";
            }

            if (filtrosPesquisa.Veiculo > 0)
                where += $" and (Carga.CAR_VEICULO = {filtrosPesquisa.Veiculo} or exists (select VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS where CAR_CODIGO = Carga.CAR_CODIGO and VEI_CODIGO = {filtrosPesquisa.Veiculo}))";

            if (filtrosPesquisa.ModeloVeicular > 0)
            {
                where += $" and (ModeloVeicular.MVC_CODIGO = {filtrosPesquisa.ModeloVeicular} or exists (select VeiculoVinculado.VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS VeiculoVinculado INNER JOIN T_VEICULO Veiculo ON VeiculoVinculado.VEI_CODIGO = Veiculo.VEI_CODIGO INNER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular ON Veiculo.MVC_CODIGO = ModeloVeicular.MVC_CODIGO where VeiculoVinculado.CAR_CODIGO = Carga.CAR_CODIGO and ModeloVeicular.MVC_CODIGO = {filtrosPesquisa.ModeloVeicular}))";

                if (!joins.Contains(" Carga "))
                    joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";

                if (!joins.Contains(" Veiculo "))
                    joins += "left outer join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ";

                if (!joins.Contains(" ModeloVeicular "))
                    joins += "left outer join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO ";
            }

            if (filtrosPesquisa.Situacao?.Count > 0)
                where += $" and Contrato.CFT_CONTRATO_FRETE in ({string.Join(",", filtrosPesquisa.Situacao.Select(o => o.ToString("D")))})";

            if (filtrosPesquisa.DataAprovacaoInicial != DateTime.MinValue)
                where += " and Contrato.CFT_CODIGO in (SELECT CFT_CODIGO FROM T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO WHERE CAST(AAC_DATA AS DATE) >= '" + filtrosPesquisa.DataAprovacaoInicial.ToString(pattern) + "')"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.DataAprovacaoFinal != DateTime.MinValue)
                where += " and Contrato.CFT_CODIGO in (SELECT CFT_CODIGO FROM T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO WHERE CAST(AAC_DATA AS DATE) <= '" + filtrosPesquisa.DataAprovacaoFinal.ToString(pattern) + "')"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.DataEncerramentoInicial != DateTime.MinValue)
                where += " and CAST(Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO AS DATE) >= '" + filtrosPesquisa.DataEncerramentoInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataEncerramentoFinal != DateTime.MinValue)
                where += " and CAST(Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO AS DATE) <= '" + filtrosPesquisa.DataEncerramentoFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.TipoOperacao > 0)
            {
                where += " and Carga.TOP_CODIGO = " + filtrosPesquisa.TipoOperacao.ToString();

                if (!joins.Contains(" Carga "))
                    joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";
            }

            if (filtrosPesquisa.DataAberturaCIOTInicial != DateTime.MinValue || filtrosPesquisa.DataAberturaCIOTFinal != DateTime.MinValue || filtrosPesquisa.DataEncerramentoCIOTInicial != DateTime.MinValue || filtrosPesquisa.DataEncerramentoCIOTFinal != DateTime.MinValue)
                where += $" AND EXISTS (SELECT CFT_CODIGO FROM T_CARGA_CIOT cargaCIOT inner join T_CIOT ciot ON cargaCIOT.CIO_CODIGO = ciot.CIO_CODIGO where cargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO{(filtrosPesquisa.DataAberturaCIOTInicial != DateTime.MinValue ? $" AND ciot.CIO_DATA_ABERTURA >= '{filtrosPesquisa.DataAberturaCIOTInicial.ToString("yyyy-MM-dd")}'" : string.Empty)}{(filtrosPesquisa.DataAberturaCIOTFinal != DateTime.MinValue ? $" AND ciot.CIO_DATA_ABERTURA < '{filtrosPesquisa.DataAberturaCIOTFinal.AddDays(1).ToString("yyyy-MM-dd")}'" : string.Empty)}{(filtrosPesquisa.DataEncerramentoCIOTInicial != DateTime.MinValue ? $" AND ciot.CIO_DATA_ENCERRAMENTO >= '{filtrosPesquisa.DataEncerramentoCIOTInicial.ToString("yyyy-MM-dd")}'" : string.Empty)}{(filtrosPesquisa.DataEncerramentoCIOTFinal != DateTime.MinValue ? $" AND ciot.CIO_DATA_ENCERRAMENTO < '{filtrosPesquisa.DataEncerramentoCIOTFinal.AddDays(1).ToString("yyyy-MM-dd")}'" : string.Empty)}) "; // SQL-INJECTION-SAFE

            //if (filtrosPesquisa.StatusCTe != string.Empty)
            //{
            //    where += $@" AND '{filtrosPesquisa.StatusCTe}' IN(
            //       SELECT 
            //        CTe.CON_STATUS
            //       FROM T_CARGA_CTE CargaCTe 
            //        INNER JOIN T_CTE CTe ON CTe.CON_CODIGO = CargaCTe.CON_CODIGO 
            //       WHERE CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO AND CargaCTe.CON_CODIGO IS NOT NULL) ";

            //    if (!joins.Contains(" Carga "))
            //        joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
            //}

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCIOT))
                where += $" AND EXISTS (SELECT CFT_CODIGO FROM T_CARGA_CIOT CargaCIOT INNER JOIN T_CIOT CIOT ON CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO WHERE CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO AND CIOT.CIO_NUMERO = '{filtrosPesquisa.NumeroCIOT}') ";

            if (filtrosPesquisa.TipoProprietario != null)
            {
                if (!joins.Contains(" Terceiro "))
                    joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";

                if (!joins.Contains(" Modalidade "))
                    joins += "LEFT OUTER JOIN T_CLIENTE_MODALIDADE Modalidade on Modalidade.CPF_CNPJ = Terceiro.CLI_CGCCPF ";

                if (!joins.Contains(" Modalidadetransportador "))
                    joins += "LEFT OUTER JOIN T_CLIENTE_MODALIDADE_TRANSPORTADORAS Modalidadetransportador on Modalidadetransportador.MOD_CODIGO = Modalidade.MOD_CODIGO ";

                where += "and Modalidadetransportador.MOT_TIPO_TRANSPORTADOR = " + (int)filtrosPesquisa.TipoProprietario;
            }
        }

        private void SetarSelectRelatorioFreteTerceirizadoPorCTe(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoPorCTe filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "TipoFornecedor":
                    if (!select.Contains(" TipoFornecedor, "))
                    {
                        select += "Terceiro.CLI_TIPO_FORNECEDOR TipoFornecedor, ";
                        groupBy += "Terceiro.CLI_TIPO_FORNECEDOR, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "MesCompetencia":
                    if (!select.Contains("MesCompetencia,"))
                    {
                        select += " MONTH(Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO) MesCompetencia, ";
                        groupBy += "Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO, ";
                    }
                    break;
                case "AnoCompetencia":
                    if (!select.Contains("AnoCompetencia,"))
                    {
                        select += " YEAR(Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO) MesCompetencia, ";
                        groupBy += "Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO, ";
                    }
                    break;
                case "CentroResultadoEmpresa":
                    if (!select.Contains(" CentroResultadoEmpresa, "))
                    {
                        select += "Empresa.EMP_CODIGO_CENTRO_CUSTO CentroResultadoEmpresa, ";
                        groupBy += "Empresa.EMP_CODIGO_CENTRO_CUSTO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";
                    }
                    break;
                case "CodigoEstabelecimento":
                    if (!select.Contains(" CodigoEstabelecimento, "))
                    {
                        select += "Empresa.EMP_CODIGO_ESTABELECIMENTO CodigoEstabelecimento, ";
                        groupBy += "Empresa.EMP_CODIGO_ESTABELECIMENTO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";
                    }
                    break;
                case "CodigoEmpresa":
                    if (!select.Contains(" CodigoEmpresa, "))
                    {
                        select += "Empresa.EMP_CODIGO_EMPRESA CodigoEmpresa, ";
                        groupBy += "Empresa.EMP_CODIGO_EMPRESA, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";
                    }
                    break;
                case "MunicipioLancamento":
                    if (!select.Contains(" MunicipioLancamento, "))
                    {
                        select += "LocalidadeEmpresa.LOC_IBGE MunicipioLancamento, ";
                        groupBy += "LocalidadeEmpresa.LOC_IBGE, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";

                        if (!joins.Contains(" LocalidadeEmpresa "))
                            joins += "join T_LOCALIDADES LocalidadeEmpresa on Empresa.LOC_CODIGO = LocalidadeEmpresa.LOC_CODIGO ";
                    }
                    break;
                case "ContratoFrete":
                    if (!select.Contains(" ContratoFrete, "))
                    {
                        select += "Contrato.CFT_NUMERO_CONTRATO ContratoFrete, ";
                        groupBy += "Contrato.CFT_NUMERO_CONTRATO, ";
                    }
                    break;
                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga,"))
                    {
                        select += "Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ";
                        groupBy += "Carga.CAR_CODIGO_CARGA_EMBARCADOR, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "NumeroCTes":
                    if (!select.Contains(" NumeroCTes, "))
                    {
                        select += "CTe.CON_NUM NumeroCTes, ";
                        groupBy += "CTe.CON_NUM, ";

                        if (!joins.Contains(" CargaCTe "))
                            joins += "LEFT JOIN T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";

                        if (!joins.Contains(" CTe "))
                            joins += "LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO ";
                    }
                    break;


                case "QuantiaCTes":
                    if (!select.Contains(" QuantiaCTes, "))
                    {
                        select += "(Select count(CargaCTe.CAR_CODIGO) FROM T_CARGA_CTE CargaCTe WHERE CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO) QuantiaCTes, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains("Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;

                case "SerieCTes":
                    if (!select.Contains(" SerieCTes, "))
                    {
                        select += "CTe.CON_SERIE NumeroCTes, ";
                        groupBy += "CTe.CON_SERIE, ";

                        if (!joins.Contains(" CargaCTe "))
                            joins += "LEFT JOIN T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";

                        if (!joins.Contains(" CTe "))
                            joins += "LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO ";
                    }
                    break;

                case "StatusCTes":
                    if (!select.Contains(" StatusCTes, "))
                    {
                        select += "CTe.CON_STATUS NumeroCTes, ";
                        groupBy += "CTe.CON_STATUS, ";

                        if (!joins.Contains(" CargaCTe "))
                            joins += "LEFT JOIN T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";

                        if (!joins.Contains(" CTe "))
                            joins += "LEFT JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO ";
                    }
                    break;

                case "TipoDocumento":
                    if (!select.Contains("TipoDocumento,"))
                    {
                        select += "SUBSTRING((SELECT DISTINCT ', ' + modeloDocumento.MOD_ABREVIACAO FROM T_CARGA_CTE cargaCTe inner join T_CTE cte ON cte.CON_CODIGO = cargaCTe.CON_CODIGO inner join T_MODDOCFISCAL modeloDocumento on modeloDocumento.MOD_CODIGO = cte.CON_MODELODOC WHERE cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and cargaCTe.CON_CODIGO IS NOT NULL  FOR XML PATH('')), 3, 1000) TipoDocumento,  ";
                        groupBy += "Carga.CAR_CODIGO, ";
                    }
                    break;
                case "RG":
                    if (!select.Contains("RG,"))
                    {
                        select += "SUBSTRING(ISNULL((SELECT DISTINCT ', ' + mot.FUN_RG FROM T_CARGA_MOTORISTA cargaMotorista inner join T_FUNCIONARIO mot ON mot.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) RG, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "Motorista":
                    if (!select.Contains("Motorista,"))
                    {
                        select += "SUBSTRING(ISNULL((SELECT DISTINCT ', ' + mot.FUN_NOME FROM T_CARGA_MOTORISTA cargaMotorista inner join T_FUNCIONARIO mot ON mot.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) Motorista, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "CPFMotorista":
                    if (!select.Contains("CPFMotorista,"))
                    {
                        select += "SUBSTRING(ISNULL((SELECT DISTINCT ', ' + mot.FUN_CPF FROM T_CARGA_MOTORISTA cargaMotorista inner join T_FUNCIONARIO mot ON mot.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) CPFMotorista, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "NumeroMDFes":
                    if (!select.Contains("NumeroMDFes,"))
                    {
                        select += "SUBSTRING((SELECT DISTINCT ', ' + CAST(mdfe.MDF_NUMERO AS NVARCHAR(20)) FROM T_CARGA_MDFE cargaMDFe inner join T_MDFE mdfe ON mdfe.MDF_CODIGO = cargaMDFe.MDF_CODIGO WHERE cargaMDFe.CAR_CODIGO = Carga.CAR_CODIGO and cargaMDFe.MDF_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000) NumeroMDFes, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "NumeroCIOT":
                    if (!select.Contains("NumeroCIOT,"))
                    {
                        select += "CIOT.CIO_NUMERO NumeroCIOT, ";
                        groupBy += "CIOT.CIO_NUMERO, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;
                case "DataSaqueAdiantamento":
                    if (!select.Contains("DataSaqueAdiantamento,"))
                    {
                        select += "convert(nvarchar(10), CIOT.CIO_DATA_SAQUE_ADIANTAMENTO, 3) + ' ' + convert(nvarchar(10), CIOT.CIO_DATA_SAQUE_ADIANTAMENTO, 108) DataSaqueAdiantamento, ";
                        groupBy += "CIOT.CIO_DATA_SAQUE_ADIANTAMENTO, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;
                case "ValorSaqueAdiantamento":
                    if (!select.Contains("ValorSaqueAdiantamento,"))
                    {
                        select += "CIOT.CIO_VALOR_SAQUE_ADIANTAMENTO ValorSaqueAdiantamento, ";
                        groupBy += "CIOT.CIO_VALOR_SAQUE_ADIANTAMENTO, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo,"))
                    {
                        select += "((select vei.VEI_PLACA from T_VEICULO vei where vei.VEI_CODIGO = Carga.CAR_VEICULO) + ISNULL((SELECT ', ' + veiculo1.VEI_PLACA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')) Veiculo, ";
                        groupBy += "Carga.CAR_CODIGO, Carga.CAR_VEICULO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador,"))
                    {
                        select += "SUBSTRING((SELECT DISTINCT ', ' + pedido.PED_NUMERO_PEDIDO_EMBARCADOR FROM T_PEDIDO pedido inner join T_CARGA_PEDIDO cargaPedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO WHERE cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) NumeroPedidoEmbarcador, ";
                        groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular,"))
                    {
                        select += "reverse(stuff(reverse(ISNULL((select mod.MVC_DESCRICAO + ', ' from T_VEICULO vei inner join T_MODELO_VEICULAR_CARGA mod on mod.MVC_CODIGO = vei.MVC_CODIGO where vei.VEI_CODIGO = Carga.CAR_VEICULO), '') + ISNULL((SELECT mod.MVC_DESCRICAO + ', ' FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO inner join T_MODELO_VEICULAR_CARGA mod on mod.MVC_CODIGO = veiculo1.MVC_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')), 1, 2, '')) ModeloVeicular, ";
                        groupBy += "Carga.CAR_CODIGO, Carga.CAR_VEICULO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;
                case "SegmentoVeiculo":
                    if (!select.Contains(" SegmentoVeiculo,"))
                    {
                        select += "reverse(stuff(reverse(ISNULL((select seg.VSE_DESCRICAO + ', ' from T_VEICULO vei inner join T_VEICULO_SEGMENTO seg on seg.VSE_CODIGO = vei.VSE_CODIGO where vei.VEI_CODIGO = Carga.CAR_VEICULO), '') + ISNULL((SELECT seg.VSE_DESCRICAO + ', ' FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO inner join T_VEICULO_SEGMENTO seg on seg.VSE_CODIGO = veiculo1.VSE_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')), 1, 2, '')) SegmentoVeiculo, ";
                        groupBy += "Carga.CAR_CODIGO, Carga.CAR_VEICULO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "Terceiro":
                    if (!select.Contains(" Terceiro,"))
                    {
                        select += "Terceiro.CLI_NOME Terceiro, ";
                        groupBy += "Terceiro.CLI_NOME, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "CPFCNPJTerceiroFormatado":
                    if (!select.Contains("CPFCNPJTerceiro,"))
                    {
                        select += "Terceiro.CLI_CGCCPF CPFCNPJTerceiro, Terceiro.CLI_FISJUR TipoTerceiro, ";
                        groupBy += "Terceiro.CLI_CGCCPF, Terceiro.CLI_FISJUR, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "PISPASEPTerceiro":
                    if (!select.Contains("PISPASEPTerceiro,"))
                    {
                        select += "Terceiro.CLI_PIS_PASEP PISPASEPTerceiro, ";
                        groupBy += "Terceiro.CLI_PIS_PASEP, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "DataNascimentoTerceiro":
                    if (!select.Contains("DataNascimentoTerceiro,"))
                    {
                        select += "CONVERT(NVARCHAR(10), Terceiro.CLI_DATA_NASCIMENTO, 103) DataNascimentoTerceiro, ";
                        groupBy += "Terceiro.CLI_DATA_NASCIMENTO, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "RegimeTributarioTerceiroFormatado":
                    if (!select.Contains(" RegimeTributarioTerceiro, "))
                    {
                        select += "Terceiro.CLI_REGIME_TRIBUTARIO RegimeTributarioTerceiro, ";
                        groupBy += "Terceiro.CLI_REGIME_TRIBUTARIO, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;
                case "TipoPessoaTerceiroFormatado":
                    if (!select.Contains(" TipoPessoaTerceiro, "))
                    {
                        select += "Terceiro.CLI_FISJUR TipoPessoaTerceiro, ";
                        groupBy += "Terceiro.CLI_FISJUR, ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select += "CargaDadosSumarizados.CDS_REMETENTES Remetente, ";
                        groupBy += "CargaDadosSumarizados.CDS_REMETENTES, ";

                        if (!joins.Contains(" Carga "))
                            joins += " left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "LocalidadeRemetente":
                    if (!select.Contains("LocalidadeRemetente,"))
                    {
                        select += "CargaDadosSumarizados.CDS_ORIGENS LocalidadeRemetente, ";
                        groupBy += "CargaDadosSumarizados.CDS_ORIGENS, ";

                        if (!joins.Contains(" Carga "))
                            joins += " left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;

                case "CPFCNPJRemetente":
                    if (!select.Contains("CPFCNPJRemetente,"))
                    {
                        select += @"SUBSTRING(ISNULL((SELECT DISTINCT ', ' + case when Cliente.CLI_FISJUR = 'J' then FORMAT(Cliente.CLI_CGCCPF, '00\.000\.000\/0000-00')
                                    when Cliente.CLI_FISJUR = 'F' then FORMAT(Cliente.CLI_CGCCPF, '000\.000\.000-00') 
                                    else '' end
                                    FROM T_CARGA_PEDIDO CargaPedido                                    
                                    JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                                    JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE
                                    WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) CPFCNPJRemetente, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;

                case "CPFCNPJDestinatario":
                    if (!select.Contains("CPFCNPJDestinatario,"))
                    {
                        select += @"SUBSTRING(ISNULL((SELECT DISTINCT ', ' + case when Cliente.CLI_FISJUR = 'J' then FORMAT(Cliente.CLI_CGCCPF, '00\.000\.000\/0000-00')
                                    when Cliente.CLI_FISJUR = 'F' then FORMAT(Cliente.CLI_CGCCPF, '000\.000\.000-00') 
                                    else '' end
                                    FROM T_CARGA_PEDIDO CargaPedido                                    
                                    JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO 
                                    JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = Pedido.CLI_CODIGO
                                    WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) CPFCNPJDestinatario, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor,"))
                    {
                        select += "CargaDadosSumarizados.CDS_EXPEDIDORES Expedidor, ";
                        groupBy += "CargaDadosSumarizados.CDS_EXPEDIDORES, ";

                        if (!joins.Contains(" Carga "))
                            joins += " left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "LocalidadeExpedidor":
                    if (!select.Contains("LocalidadeExpedidor,"))
                    {
                        select += @"ISNULL((SELECT TOP(1) Localidade.LOC_DESCRICAO LocalidadeExpedidor FROM T_LOCALIDADES Localidade
                                    JOIN T_CLIENTE Cliente on Cliente.LOC_CODIGO = Localidade.LOC_CODIGO
                                    JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CLI_CODIGO_EXPEDIDOR = Cliente.CLI_CGCCPF 
                                    WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO), '') LocalidadeExpedidor, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "UFExpedidor":
                    if (!select.Contains("UFExpedidor,"))
                    {
                        select += @"ISNULL((SELECT TOP(1) Localidade.UF_SIGLA UFExpedidor FROM T_LOCALIDADES Localidade
                                    JOIN T_CLIENTE Cliente on Cliente.LOC_CODIGO = Localidade.LOC_CODIGO
                                    JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CLI_CODIGO_EXPEDIDOR = Cliente.CLI_CGCCPF 
                                    WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO), '') UFExpedidor, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains(" Destinatario,"))
                    {
                        select += "CargaDadosSumarizados.CDS_DESTINATARIOS Destinatario, ";
                        groupBy += "CargaDadosSumarizados.CDS_DESTINATARIOS, ";

                        if (!joins.Contains(" Carga "))
                            joins += " left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "LocalidadeDestinatario":
                    if (!select.Contains("LocalidadeDestinatario,"))
                    {
                        select += "CargaDadosSumarizados.CDS_DESTINOS LocalidadeDestinatario, ";
                        groupBy += "CargaDadosSumarizados.CDS_DESTINOS, ";

                        if (!joins.Contains(" Carga "))
                            joins += " left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" CargaDadosSumarizados "))
                            joins += " left outer join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ";
                    }
                    break;
                case "ValorICMS":
                    if (!select.Contains("ValorICMS,"))
                    {
                        select += "Carga.CAR_VALOR_ICMS ValorICMS, ";
                        groupBy += "Carga.CAR_VALOR_ICMS, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";
                    }
                    break;
                case "ValorFrete":
                    if (!select.Contains("ValorFrete,"))
                    {
                        select += "Carga.CAR_VALOR_FRETE_LIQUIDO ValorFrete, ";
                        groupBy += "Carga.CAR_VALOR_FRETE_LIQUIDO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";
                    }
                    break;
                case "ValorReceber":
                    if (!select.Contains("ValorReceber,"))
                    {
                        select += "Carga.CAR_VALOR_FRETE_PAGAR ValorReceber, ";
                        groupBy += "Carga.CAR_VALOR_FRETE_PAGAR, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";
                    }
                    break;
                case "PercentualAdiantamento":
                    if (!select.Contains("PercentualAdiantamento,"))
                    {
                        select += "Contrato.CFT_PERCENTUAL_ADIANTAMENTO PercentualAdiantamento, ";
                        groupBy += "Contrato.CFT_PERCENTUAL_ADIANTAMENTO, ";
                    }
                    break;
                case "ValorAdiantamento":
                    if (!select.Contains("ValorAdiantamento,"))
                    {
                        select += "Contrato.CFT_ADIANTAMENTO ValorAdiantamento, ";

                        if (!groupBy.Contains("Contrato.CFT_ADIANTAMENTO"))
                            groupBy += "Contrato.CFT_ADIANTAMENTO, ";
                    }
                    break;
                case "PercentualAbastecimento":
                    if (!select.Contains("PercentualAbastecimento,"))
                    {
                        select += "Contrato.CFT_PERCENTUAL_ABASTECIMENTO PercentualAbastecimento, ";

                        if (!groupBy.Contains("Contrato.CFT_PERCENTUAL_ABASTECIMENTO"))
                            groupBy += "Contrato.CFT_PERCENTUAL_ABASTECIMENTO, ";
                    }
                    break;
                case "ValorAbastecimento":
                    if (!select.Contains("ValorAbastecimento,"))
                    {
                        select += "Contrato.CFT_ABASTECIMENTO ValorAbastecimento, ";

                        if (!groupBy.Contains("Contrato.CFT_ABASTECIMENTO"))
                            groupBy += "Contrato.CFT_ABASTECIMENTO, ";
                    }
                    break;
                case "ValorFreteMenosAbastecimento":
                    if (!select.Contains("ValorFreteMenosAbastecimento,"))
                    {
                        select += "(Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO - ISNULL(Contrato.CFT_ABASTECIMENTO, 0)) ValorFreteMenosAbastecimento, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";

                        if (!groupBy.Contains("Contrato.CFT_ABASTECIMENTO"))
                            groupBy += "Contrato.CFT_ABASTECIMENTO, ";
                    }
                    break;
                case "ValorPago":
                    if (!select.Contains("ValorPago,"))
                    {
                        select += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO ValorPago, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";
                    }
                    break;
                case "ValorBruto":
                    if (!select.Contains("ValorBruto,"))
                    {
                        select += @"(Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO
                                    - ISNULL((SELECT SUM(CFV_VALOR) FROM T_CONTRATO_FRETE_TERCEIRO_VALOR WHERE CFT_CODIGO = Contrato.CFT_CODIGO AND CFV_TIPO_JUSTIFICATIVA = 2), 0)
                                    + ISNULL((SELECT SUM(CFV_VALOR) FROM T_CONTRATO_FRETE_TERCEIRO_VALOR WHERE CFT_CODIGO = Contrato.CFT_CODIGO AND CFV_TIPO_JUSTIFICATIVA = 1), 0)) ValorBruto, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "ValorSaldo":
                    if (!select.Contains("ValorSaldo,"))
                    {
                        select += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO - (CASE WHEN Contrato.CFT_RETER_IMPOSTOS_CONTRATO_FRETE = 1 THEN (CASE WHEN Contrato.CFT_VALOR_IRRF_PERIODO >= 10 THEN Contrato.CFT_VALOR_IRRF WHEN Contrato.CFT_VALOR_IRRF_PERIODO <= 10 AND (Contrato.CFT_VALOR_IRRF_PERIODO + Contrato.CFT_VALOR_IRRF) >= 10 THEN Contrato.CFT_VALOR_IRRF_PERIODO + Contrato.CFT_VALOR_IRRF ELSE 0 END) + Contrato.CFT_VALOR_INSS + Contrato.CFT_VALOR_SENAT + Contrato.CFT_VALOR_SEST ELSE 0 END) - Contrato.CFT_ADIANTAMENTO - (CASE TipoIntegracaoValePedagio.TPI_NAO_SUBTRAIR_VALE_PEDAGIO_DO_CONTRATO WHEN 0 THEN Contrato.CFT_VALOR_PEDAGIO ELSE 0 END) + Contrato.CFT_VALOR_TOTAL_ACRESCIMO_SALDO - Contrato.CFT_VALOR_TOTAL_DESCONTO_SALDO + Contrato.CFT_VALOR_TOTAL_ACRESCIMO_ADIANTAMENTO - Contrato.CFT_VALOR_TOTAL_DESCONTO_ADIANTAMENTO ValorSaldo, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy += "Contrato.CFT_VALOR_IRRF, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF_PERIODO"))
                            groupBy += "Contrato.CFT_VALOR_IRRF_PERIODO, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_TOTAL_DESCONTO_SALDO"))
                            groupBy += "Contrato.CFT_VALOR_TOTAL_DESCONTO_SALDO, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_TOTAL_ACRESCIMO_SALDO"))
                            groupBy += "Contrato.CFT_VALOR_TOTAL_ACRESCIMO_SALDO, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_PEDAGIO"))
                            groupBy += "Contrato.CFT_VALOR_PEDAGIO, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";

                        if (!groupBy.Contains("Contrato.CFT_ADIANTAMENTO"))
                            groupBy += "Contrato.CFT_ADIANTAMENTO, ";

                        if (!groupBy.Contains("Contrato.CFT_RETER_IMPOSTOS_CONTRATO_FRETE"))
                            groupBy += "Contrato.CFT_RETER_IMPOSTOS_CONTRATO_FRETE, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_INSS"))
                            groupBy += "Contrato.CFT_VALOR_INSS, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_SEST"))
                            groupBy += "Contrato.CFT_VALOR_SEST, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_SENAT"))
                            groupBy += "Contrato.CFT_VALOR_SENAT, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy += "Contrato.CFT_VALOR_IRRF, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy += "Contrato.CFT_VALOR_IRRF, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_TOTAL_ACRESCIMO_ADIANTAMENTO"))
                            groupBy += "Contrato.CFT_VALOR_TOTAL_ACRESCIMO_ADIANTAMENTO, ";

                        if (!groupBy.Contains("TipoIntegracaoValePedagio.TPI_NAO_SUBTRAIR_VALE_PEDAGIO_DO_CONTRATO"))
                            groupBy += "TipoIntegracaoValePedagio.TPI_NAO_SUBTRAIR_VALE_PEDAGIO_DO_CONTRATO, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_TOTAL_DESCONTO_ADIANTAMENTO"))
                            groupBy += "Contrato.CFT_VALOR_TOTAL_DESCONTO_ADIANTAMENTO, ";

                        if (!joins.Contains(" TipoIntegracaoValePedagio "))
                            joins += "left outer join T_TIPO_INTEGRACAO TipoIntegracaoValePedagio on Contrato.TPI_CODIGO_VALE_PEDAGIO = TipoIntegracaoValePedagio.TPI_CODIGO ";
                    }
                    break;
                case "ValorINSS":
                    if (!select.Contains("ValorINSS,"))
                    {
                        select += "Contrato.CFT_VALOR_INSS ValorINSS, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_INSS"))
                            groupBy += "Contrato.CFT_VALOR_INSS, ";
                    }
                    break;
                case "ValorIRRF":
                    if (!select.Contains("ValorIRRF,"))
                    {
                        select += "Contrato.CFT_VALOR_IRRF ValorIRRF, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy += "Contrato.CFT_VALOR_IRRF, ";
                    }
                    break;
                case "ValorSEST":
                    if (!select.Contains("ValorSEST,"))
                    {
                        select += "Contrato.CFT_VALOR_SEST ValorSEST, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_SEST"))
                            groupBy += "Contrato.CFT_VALOR_SEST, ";
                    }
                    break;
                case "ValorSENAT":
                    if (!select.Contains("ValorSENAT,"))
                    {
                        select += "Contrato.CFT_VALOR_SENAT ValorSENAT, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_SENAT"))
                            groupBy += "Contrato.CFT_VALOR_SENAT, ";
                    }
                    break;
                case "BCICMS":
                    if (!select.Contains("BCICMS,"))
                    {
                        select += @"ISNULL((SELECT SUM(CTe.CON_BC_ICMS) FROM T_CARGA_CTE CargaCTe 
                            JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO
                            WHERE CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D")))}) " : string.Empty;
                        select += " ), 0) BCICMS, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "AliquotaICMS":
                    if (!select.Contains("AliquotaICMS,"))
                    {
                        select += @"ISNULL((SELECT MAX(CTe.CON_ALIQ_ICMS) FROM T_CARGA_CTE CargaCTe 
                            JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO
                            WHERE CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D")))}) " : string.Empty;
                        select += " ), 0) AliquotaICMS, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "PesoKg":
                    if (!select.Contains("PesoKg,"))
                    {
                        select += @"ISNULL((SELECT SUM(CTe.CON_PESO) FROM T_CARGA_CTE CargaCTe 
                            JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO
                            WHERE CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D")))}) " : string.Empty;
                        select += " ), 0) PesoKg, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "ValorFreteNegociado":
                    if (!select.Contains("ValorFreteNegociado,"))
                    {
                        select += @"ISNULL((SELECT MAX(Pedido.PED_VALOR_FRETE_TONELADA_NEGOCIADO) FROM T_CARGA_PEDIDO CargaPedido 
                            JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO), 0 ) ValorFreteNegociado, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "ValorFreteTerceiro":
                    if (!select.Contains("ValorFreteTerceiro,"))
                    {
                        select += @"ISNULL((SELECT MAX(Pedido.PED_VALOR_FRETE_TONELADA_TERCEIRO) FROM T_CARGA_PEDIDO CargaPedido 
                            JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                            WHERE CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO), 0) ValorFreteTerceiro, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "ProdutoPredominante":
                    if (!select.Contains("ProdutoPredominante,"))
                    {
                        select += @"ISNULL((SELECT MAX(CTe.CON_PRODUTO_PRED) FROM T_CARGA_CTE CargaCTe 
                            JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO
                            WHERE CargaCTe.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D")))}) " : string.Empty;
                        select += "), '') ProdutoPredominante, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "NumeroValePedagio":
                    if (!select.Contains("NumeroValePedagio,"))
                    {
                        select += @"(ISNULL((  
                                            SELECT MAX(ValePedagio.CVP_NUMERO_VALE_PEDAGIO)
                                            FROM T_CARGA_INTEGRACAO_VALE_PEDAGIO ValePedagio
                                            WHERE ValePedagio.CAR_CODIGO = Contrato.CAR_CODIGO), 
   
                                            (SELECT MAX(ValePedagio.CVP_NUMERO_COMPROVANTE)
                                            FROM T_CARGA_VALE_PEDAGIO ValePedagio
                                            WHERE ValePedagio.CAR_CODIGO = Contrato.CAR_CODIGO)   
                                           )) NumeroValePedagio, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "ValorPedagioManual":
                    if (!select.Contains(" ValorPedagioManual, "))
                    {
                        select += @"(SELECT SUM(ValePedagio.CVP_VALOR) FROM T_CARGA_VALE_PEDAGIO ValePedagio WHERE ValePedagio.CAR_CODIGO = Contrato.CAR_CODIGO) ValorPedagioManual, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";

                        if (!groupBy.Contains(" Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;
                case "ValorAcrescimos":
                    if (!select.Contains("ValorAcrescimos,"))
                    {
                        select += "(SELECT SUM(CFV_VALOR) FROM T_CONTRATO_FRETE_TERCEIRO_VALOR WHERE CFT_CODIGO = Contrato.CFT_CODIGO AND CFV_TIPO_JUSTIFICATIVA = 2) ValorAcrescimos, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "TotalDescontos":
                    if (!select.Contains("TotalDescontos,"))
                    {
                        select += "ISNULL(Contrato.CFT_TARIFA_SAQUE, 0) + ISNULL(Contrato.CFT_TARIFA_TRANSFERENCIA, 0) + ISNULL(Contrato.CFT_DESCONTO, 0) TotalDescontos, ";

                        if (!groupBy.Contains("Contrato.CFT_TARIFA_SAQUE"))
                            groupBy += "Contrato.CFT_TARIFA_SAQUE, ";
                        if (!groupBy.Contains("Contrato.CFT_TARIFA_TRANSFERENCIA"))
                            groupBy += "Contrato.CFT_TARIFA_TRANSFERENCIA, ";
                        if (!groupBy.Contains("Contrato.CFT_DESCONTO"))
                            groupBy += "Contrato.CFT_DESCONTO, ";
                    }
                    break;
                case "SaldoFrete":
                    if (!select.Contains("Impostos,"))
                    {
                        select += "ISNULL(Contrato.CFT_VALOR_INSS, 0) + ISNULL(Contrato.CFT_VALOR_IRRF, 0) + ISNULL(Contrato.CFT_VALOR_SEST, 0) + ISNULL(Contrato.CFT_VALOR_SENAT, 0) Impostos, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_INSS"))
                            groupBy += "Contrato.CFT_VALOR_INSS, ";
                        if (!groupBy.Contains("Contrato.CFT_VALOR_IRRF"))
                            groupBy += "Contrato.CFT_VALOR_IRRF, ";
                        if (!groupBy.Contains("Contrato.CFT_VALOR_SEST"))
                            groupBy += "Contrato.CFT_VALOR_SEST, ";
                        if (!groupBy.Contains("Contrato.CFT_VALOR_SENAT"))
                            groupBy += "Contrato.CFT_VALOR_SENAT, ";
                    }
                    if (!select.Contains("ValorPago,"))
                    {
                        select += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO ValorPago, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";
                    }
                    if (!select.Contains("TotalDescontos,"))
                    {
                        select += "ISNULL(Contrato.CFT_TARIFA_SAQUE, 0) + ISNULL(Contrato.CFT_TARIFA_TRANSFERENCIA, 0) + ISNULL(Contrato.CFT_DESCONTO, 0) TotalDescontos, ";

                        if (!groupBy.Contains("Contrato.CFT_TARIFA_SAQUE"))
                            groupBy += "Contrato.CFT_TARIFA_SAQUE, ";
                        if (!groupBy.Contains("Contrato.CFT_TARIFA_TRANSFERENCIA"))
                            groupBy += "Contrato.CFT_TARIFA_TRANSFERENCIA, ";
                        if (!groupBy.Contains("Contrato.CFT_DESCONTO"))
                            groupBy += "Contrato.CFT_DESCONTO, ";
                    }
                    if (!select.Contains("ValorAdiantamento,"))
                    {
                        select += "Contrato.CFT_ADIANTAMENTO ValorAdiantamento, ";

                        if (!groupBy.Contains("Contrato.CFT_ADIANTAMENTO"))
                            groupBy += "Contrato.CFT_ADIANTAMENTO, ";
                    }
                    break;
                case "ValorDescontos":
                    if (!select.Contains("ValorDescontos,"))
                    {
                        select += "(SELECT SUM(CFV_VALOR) FROM T_CONTRATO_FRETE_TERCEIRO_VALOR WHERE CFT_CODIGO = Contrato.CFT_CODIGO AND CFV_TIPO_JUSTIFICATIVA = 1) ValorDescontos, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataEmissao":
                    if (!select.Contains("DataEmissao,"))
                    {
                        select += "Contrato.CFT_DATA_EMISSAO_CONTRATO DataEmissao, ";
                        groupBy += "Contrato.CFT_DATA_EMISSAO_CONTRATO, ";
                    }
                    break;
                case "DataEncerramento":
                    if (!select.Contains("DataEncerramento,"))
                    {
                        select += "CONVERT(NVARCHAR(10), Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO, 3) DataEncerramento, ";
                        groupBy += "Contrato.CFT_DATA_ENCERRAMENTO_CONTRATO, ";
                    }
                    break;
                case "DataAutorizacaoPagamento":
                    if (!select.Contains("DataAutorizacaoPagamento,"))
                    {
                        select += "(select TOP 1 CONVERT(NVARCHAR(10), CIOT.CIO_DATA_AUTORIZACAO_PAGAMENTO, 3) from T_CARGA_CIOT CargaCIOT inner join T_CIOT CIOT on CargaCIOT.CIO_CODIGO = CIOT.CIO_CODIGO WHERE CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO) DataAutorizacaoPagamento, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataVencimentoAdiantamento":
                    if (!select.Contains("DataVencimentoAdiantamento,"))
                    {
                        select += "(select top(1) CASE TIT_DATA_VENCIMENTO WHEN NULL THEN '' ELSE convert(nvarchar(10), TIT_DATA_VENCIMENTO, 3) END from T_TITULO where CFT_CODIGO = Contrato.CFT_CODIGO and TIT_SEQUENCIA = 1 AND TIT_STATUS = 3) DataVencimentoAdiantamento, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataPagamentoAdiantamento":
                    if (!select.Contains("DataPagamentoAdiantamento,"))
                    {
                        select += "(select top(1) CASE TIT_DATA_LIQUIDACAO WHEN NULL THEN '' ELSE convert(nvarchar(10), TIT_DATA_LIQUIDACAO, 3) + ' ' + convert(nvarchar(10), TIT_DATA_LIQUIDACAO, 108) END from T_TITULO where CFT_CODIGO = Contrato.CFT_CODIGO and TIT_SEQUENCIA = 1 AND TIT_STATUS = 3) DataPagamentoAdiantamento, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataVencimentoValor":
                    if (!select.Contains("DataVencimentoValor,"))
                    {
                        select += "(select top(1) CASE TIT_DATA_VENCIMENTO WHEN NULL THEN '' ELSE convert(nvarchar(10), TIT_DATA_VENCIMENTO, 3) END from T_TITULO where CFT_CODIGO = Contrato.CFT_CODIGO and TIT_SEQUENCIA = 2 AND TIT_STATUS = 3) DataVencimentoValor, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataPagamentoValor":
                    if (!select.Contains("DataPagamentoValor,"))
                    {
                        select += "(select top(1) CASE TIT_DATA_LIQUIDACAO WHEN NULL THEN '' ELSE convert(nvarchar(10), TIT_DATA_LIQUIDACAO, 3) + ' ' + convert(nvarchar(10), TIT_DATA_LIQUIDACAO, 108) END from T_TITULO where CFT_CODIGO = Contrato.CFT_CODIGO and TIT_SEQUENCIA = 2 AND TIT_STATUS = 3) DataPagamentoValor, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "Titulos":
                    if (!select.Contains("Titulos,"))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + 
                                                               CONVERT(NVARCHAR(50), titulo.TIT_CODIGO) + 
                                                               ' (' + 
                                                               (CASE titulo.TIT_STATUS WHEN 1 THEN 'Aberto' WHEN 2 THEN 'Atrasado' WHEN 3 THEN 'Quitado' WHEN 4 THEN 'Cancelado' WHEN 5 THEN 'Em Negociação' WHEN 6 THEN 'Bloqueado' ELSE '' END) + 
                                                               ')'
                                               FROM T_TITULO titulo 
                                               WHERE titulo.CFT_CODIGO = Contrato.CFT_CODIGO FOR XML PATH('')), 3, 1000) Titulos, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "CNPJEmpresa":
                    if (!select.Contains(" CNPJEmpresa, "))
                    {
                        select += "Empresa.EMP_CNPJ CNPJEmpresa, ";
                        groupBy += "Empresa.EMP_CNPJ, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";
                    }
                    break;
                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        select += "Empresa.EMP_RAZAO Empresa, ";
                        groupBy += "Empresa.EMP_RAZAO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";
                    }
                    break;
                case "CPFCNPJFilialEmissoraFormatado":
                    if (!select.Contains(" CPFCNPJFilialEmissora, "))
                    {
                        select += "EmpresaFilialEmissora.EMP_CNPJ CPFCNPJFilialEmissora, ";
                        groupBy += "EmpresaFilialEmissora.EMP_CNPJ, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" EmpresaFilialEmissora "))
                            joins += "left outer join T_EMPRESA EmpresaFilialEmissora on EmpresaFilialEmissora.EMP_CODIGO = Carga.EMP_CODIGO_FILIAL_EMISSORA ";
                    }
                    break;
                case "FilialEmissora":
                    if (!select.Contains(" FilialEmissora, "))
                    {
                        select += "EmpresaFilialEmissora.EMP_RAZAO FilialEmissora, ";
                        groupBy += "EmpresaFilialEmissora.EMP_RAZAO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" EmpresaFilialEmissora "))
                            joins += "left outer join T_EMPRESA EmpresaFilialEmissora on EmpresaFilialEmissora.EMP_CODIGO = Carga.EMP_CODIGO_FILIAL_EMISSORA ";
                    }
                    break;
                case "UFFilialEmissora":
                    if (!select.Contains(" UFFilialEmissora, "))
                    {
                        select += "LocalidadeFilialEmissora.UF_SIGLA UFFilialEmissora, ";
                        groupBy += "LocalidadeFilialEmissora.UF_SIGLA, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" EmpresaFilialEmissora "))
                            joins += "left outer join T_EMPRESA EmpresaFilialEmissora on EmpresaFilialEmissora.EMP_CODIGO = Carga.EMP_CODIGO_FILIAL_EMISSORA ";

                        if (!joins.Contains(" LocalidadeFilialEmissora "))
                            joins += "left outer join T_LOCALIDADES LocalidadeFilialEmissora on LocalidadeFilialEmissora.LOC_CODIGO = EmpresaFilialEmissora.LOC_CODIGO ";
                    }
                    break;
                case "DescricaoSituacaoContratoFrete":
                    if (!select.Contains(" SituacaoContratoFrete, "))
                    {
                        select += "Contrato.CFT_CONTRATO_FRETE SituacaoContratoFrete, ";
                        groupBy += "Contrato.CFT_CONTRATO_FRETE, ";
                    }
                    break;
                case "DataAprovacaoFormatada":
                    if (!select.Contains(" DataAprovacao, "))
                    {
                        select += "(SELECT top 1 AAC_DATA FROM T_AUTORIZACAO_ALCADA_CONTRATO_FRETE_TERCEIRO WHERE CFT_CODIGO = Contrato.CFT_CODIGO AND AAC_SITUACAO = 1 ORDER BY AAC_CODIGO DESC) DataAprovacao, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select += "TipoOperacao.TOP_DESCRICAO TipoOperacao, ";
                        groupBy += "TipoOperacao.TOP_DESCRICAO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";

                        if (!joins.Contains(" TipoOperacao "))
                            joins += "left outer join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ";
                    }
                    break;
                case "DataAberturaCIOT":
                    if (!select.Contains(" DataAberturaCIOT, "))
                    {
                        select += "(select TOP 1 CONVERT(NVARCHAR(10), CIOT.CIO_DATA_ABERTURA, 3) from T_CARGA_CIOT CargaCIOT inner join T_CIOT CIOT on CargaCIOT.CIO_CODIGO = CIOT.CIO_CODIGO WHERE CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO) DataAberturaCIOT, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "DataEncerramentoCIOT":
                    if (!select.Contains(" DataEncerramentoCIOT, "))
                    {
                        select += "(select TOP 1 CONVERT(NVARCHAR(10), CIOT.CIO_DATA_ENCERRAMENTO, 3) from T_CARGA_CIOT CargaCIOT inner join T_CIOT CIOT on CargaCIOT.CIO_CODIGO = CIOT.CIO_CODIGO WHERE CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO) DataEncerramentoCIOT, ";

                        if (!groupBy.Contains("Contrato.CFT_CODIGO"))
                            groupBy += "Contrato.CFT_CODIGO, ";
                    }
                    break;
                case "PercentualVariacao":
                    if (!select.Contains(" PercentualVariacao, "))
                    {
                        select += @"CASE WHEN (Carga.CAR_VALOR_FRETE > 0 
                                            AND (ConfiguracaoEmbarcador.CEM_EXIBIR_VARIACAO_NEGATIVA_CONTRATO_FRETE_TERCEIRO = 1 OR ((Carga.CAR_VALOR_FRETE - Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO) > 0)))
                                        THEN CAST(round((((Carga.CAR_VALOR_FRETE - Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO) * 100) / Carga.CAR_VALOR_FRETE), 0) AS INT)
                                    ELSE 0 END PercentualVariacao, ";

                        groupBy += "ConfiguracaoEmbarcador.CEM_EXIBIR_VARIACAO_NEGATIVA_CONTRATO_FRETE_TERCEIRO, ";
                        if (!groupBy.Contains("Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO"))
                            groupBy += "Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO, ";

                        if (!groupBy.Contains("Carga.CAR_VALOR_FRETE"))
                            groupBy += "Carga.CAR_VALOR_FRETE, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Contrato.CAR_CODIGO = Carga.CAR_CODIGO ";
                    }
                    break;
                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select += @"SUBSTRING((SELECT DISTINCT ', ' + centroResultado.CRE_DESCRICAO 
                                                FROM T_CENTRO_RESULTADO centroResultado
                                                inner join T_PEDIDO pedido on pedido.CRE_CODIGO = centroResultado.CRE_CODIGO
                                                inner join T_CARGA_PEDIDO cargaPedido ON cargaPedido.PED_CODIGO = pedido.PED_CODIGO 
                                                WHERE cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) CentroResultado, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "ValorPedagio":
                    if (!select.Contains(" ValorPedagio, "))
                    {
                        select += "Contrato.CFT_VALOR_PEDAGIO ValorPedagio, ";

                        if (!groupBy.Contains("Contrato.CFT_VALOR_PEDAGIO"))
                            groupBy += "Contrato.CFT_VALOR_PEDAGIO, ";
                    }
                    break;

                case "ProtocoloCIOT":
                    if (!select.Contains(" ProtocoloCIOT, "))
                    {
                        select += "CIOT.CIO_PROTOCOLO_AUTORIZACAO ProtocoloCIOT, ";
                        groupBy += "CIOT.CIO_PROTOCOLO_AUTORIZACAO, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;

                case "CodigoVerificadorCIOT":
                    if (!select.Contains(" CodigoVerificadorCIOT, "))
                    {
                        select += "CIOT.CIO_CODIGO_VERIFICADOR CodigoVerificadorCIOT, ";
                        groupBy += "CIOT.CIO_CODIGO_VERIFICADOR, ";

                        if (!joins.Contains(" CargaCIOT "))
                            joins += "left outer join T_CARGA_CIOT CargaCIOT on CargaCIOT.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" CIOT "))
                            joins += "left outer join T_CIOT CIOT on CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ";
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" Tomador,"))
                    {
                        select += "substring(( ";
                        select += "    select distinct ',' + ";
                        select += "           case CargaPedido.PED_TIPO_TOMADOR ";
                        select += "               when 0 then Remetente.CLI_NOME ";
                        select += "               when 1 then Expedidor.CLI_NOME ";
                        select += "               when 2 then Recebedor.CLI_NOME ";
                        select += "               when 3 then Destinatario.CLI_NOME ";
                        select += "               when 4 then Tomador.CLI_NOME ";
                        select += "               else '' ";
                        select += "           end ";
                        select += "     from T_CARGA_PEDIDO CargaPedido ";
                        select += "         join T_PEDIDO Pedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ";
                        select += "         left join T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR ";
                        select += "         left join T_CLIENTE Recebedor on Recebedor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR ";
                        select += "         left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = CargaPedido.CLI_CODIGO_TOMADOR ";
                        select += "         left join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ";
                        select += "         left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ";
                        select += "     where CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += "     for xml path('') ";
                        select += "), 2, 500) Tomador, ";

                        if (!groupBy.Contains("Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;

                case "CPFCNPJTomadorFormatado":
                    if (!select.Contains("CPFCNPJTomador,"))
                    {
                        select += "substring(( ";
                        select += "    select distinct ',' + ";
                        select += "            case CargaPedido.PED_TIPO_TOMADOR ";
                        select += "                when 0 then CONVERT(VARCHAR,CAST(Remetente.CLI_CGCCPF AS DECIMAL))";
                        select += "                when 1 then CONVERT(VARCHAR,CAST(Expedidor.CLI_CGCCPF AS DECIMAL)) ";
                        select += "                when 2 then CONVERT(VARCHAR,CAST(Recebedor.CLI_CGCCPF AS DECIMAL)) ";
                        select += "                when 3 then CONVERT(VARCHAR,CAST(Destinatario.CLI_CGCCPF AS DECIMAL)) ";
                        select += "                when 4 then CONVERT(VARCHAR,CAST(Tomador.CLI_CGCCPF AS DECIMAL)) ";
                        select += "                else '' ";
                        select += "            end ";
                        select += "    from T_CARGA_PEDIDO CargaPedido ";
                        select += "        join T_PEDIDO Pedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ";
                        select += "        left join T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR ";
                        select += "        left join T_CLIENTE Recebedor on Recebedor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR ";
                        select += "        left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = CargaPedido.CLI_CODIGO_TOMADOR ";
                        select += "        left join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ";
                        select += "        left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ";
                        select += "    where CargaPedido.CAR_CODIGO = Contrato.CAR_CODIGO ";
                        select += "    for xml path('') ";
                        select += "), 2, 500) CPFCNPJTomador, ";

                        if (!groupBy.Contains("Contrato.CAR_CODIGO"))
                            groupBy += "Contrato.CAR_CODIGO, ";
                    }
                    break;

                case "ValorLiquidoSemAdiantamento":
                    SetarSelectRelatorioFreteTerceirizadoPorCTe("ValorPago", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizadoPorCTe("TotalDescontos", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizadoPorCTe("ValorIRRF", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizadoPorCTe("ValorINSS", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizadoPorCTe("ValorSEST", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizadoPorCTe("ValorSENAT", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    break;

                case "ValorSubcontratacao":
                    SetarSelectRelatorioFreteTerceirizadoPorCTe("ValorBruto", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizadoPorCTe("ValorPedagio", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    break;

                case "ValorPagoPorCTeFormatado":
                    SetarSelectRelatorioFreteTerceirizadoPorCTe("ValorPago", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    SetarSelectRelatorioFreteTerceirizadoPorCTe("QuantiaCTes", 0, ref select, ref groupBy, ref joins, count, filtrosPesquisa);
                    break;

                case "SituacaoCargaDescricao":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select += "Carga.CAR_SITUACAO SituacaoCarga, ";

                        if (!groupBy.Contains("Carga.CAR_SITUACAO"))
                            groupBy += "Carga.CAR_SITUACAO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "LocalidadeEmpresaFilial":
                    if (!select.Contains(" LocalidadeEmpresaFilial, "))
                    {
                        select += "(LocalidadeEmpresa.LOC_DESCRICAO + ' - ' + LocalidadeEmpresa.UF_SIGLA) LocalidadeEmpresaFilial, ";

                        if (!groupBy.Contains("LocalidadeEmpresa.LOC_DESCRICAO"))
                            groupBy += "LocalidadeEmpresa.LOC_DESCRICAO, ";

                        if (!groupBy.Contains("LocalidadeEmpresa.UF_SIGLA"))
                            groupBy += "LocalidadeEmpresa.UF_SIGLA, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";

                        if (!joins.Contains(" Empresa "))
                            joins += "left outer join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ";

                        if (!joins.Contains(" LocalidadeEmpresa "))
                            joins += "left outer join T_LOCALIDADES LocalidadeEmpresa on LocalidadeEmpresa.LOC_CODIGO = Empresa.LOC_CODIGO ";
                    }
                    break;

                case "ValorReceberCTes":
                    if (!select.Contains(" ValorReceberCTes, "))
                    {
                        select += "(SELECT SUM(CON_VALOR_RECEBER) FROM T_CARGA_CTE CargaCTe INNER JOIN T_CTE CTe ON CTe.CON_CODIGO = CargaCTe.CON_CODIGO WHERE CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO AND CargaCTe.CON_CODIGO IS NOT NULL ";
                        select += filtrosPesquisa.NumeroCTe > 0 ? $" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} " : string.Empty;
                        select += !string.IsNullOrWhiteSpace(filtrosPesquisa.StatusCTe) ? $" and CTe.CON_STATUS = '{filtrosPesquisa.StatusCTe}' " : string.Empty;
                        select += filtrosPesquisa.TipoCTe.Count > 0 ? $" AND CTe.CON_TIPO_CTE IN ({string.Join(", ", filtrosPesquisa.TipoCTe.Select(o => o.ToString("D")))}) " : string.Empty;
                        select += " ) ValorReceberCTes, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "NumeroDocAnterior":
                    if (!select.Contains(" NumeroDocAnterior, "))
                        select += @"SUBSTRING(
                               (SELECT DISTINCT ', ' + CAST(cteT.CPS_NUMERO AS NVARCHAR(20))
                                FROM T_CARGA_PEDIDO cargaPedido
                                INNER JOIN T_PEDIDO_CTE_PARA_SUB_CONTRATACAO cte ON cte.CPE_CODIGO = cargaPedido.CPE_CODIGO
                                INNER JOIN T_CTE_TERCEIRO cteT on cteT.CPS_CODIGO = cte.CPS_CODIGO
                                WHERE cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO                      
                                  FOR XML PATH('')), 3, 1000) NumeroDocAnterior, ";
                    if (!joins.Contains(" Carga "))
                        joins += "LEFT OUTER JOIN T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";

                    if (!groupBy.Contains("Carga.CAR_CODIGO"))
                        groupBy += "Carga.CAR_CODIGO, ";

                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                        select += @" TipoCarga.TCG_DESCRICAO TipoCarga, ";

                    if (!joins.Contains(" Carga "))
                        joins += "LEFT OUTER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";

                    if (!joins.Contains(" TipoCarga "))
                        joins += "LEFT OUTER JOIN T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ";

                    if (!groupBy.Contains("TipoCarga.TCG_DESCRICAO"))
                        groupBy += "TipoCarga.TCG_DESCRICAO, ";
                    break;

                case "DataCargaFormatada":
                    if (!select.Contains(" DataCarga, "))
                        select += @"Carga.CAR_DATA_CRIACAO DataCarga, ";
                    if (!joins.Contains(" Carga "))
                        joins += "LEFT OUTER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    if (!groupBy.Contains("Carga.CAR_DATA_CRIACAO"))
                        groupBy += "Carga.CAR_DATA_CRIACAO, ";
                    break;

                case "ValorTotalProdutosNotaFiscal":
                    if (!select.Contains("ValorTotalProdutosNotaFiscal, "))
                    {
                        select += @"(SELECT SUM(NF_VALOR) FROM T_XML_NOTA_FISCAL XMLNotaFiscal 
                                    INNER JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal ON PedidoXMLNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO
                                    INNER JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoXMLNotaFiscal.CPE_CODIGO
                                    WHERE CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO 
                                    ) ValorTotalProdutosNotaFiscal, ";

                        if (!groupBy.Contains("Carga.CAR_CODIGO"))
                            groupBy += "Carga.CAR_CODIGO, ";

                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";
                    }
                    break;

                case "Banco":
                    if (!select.Contains(" Banco,"))
                    {
                        select += "Banco.BCO_DESCRICAO Banco, ";
                        groupBy += "Banco.BCO_DESCRICAO , ";


                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO ";

                        if (!joins.Contains(" Banco "))
                        {
                            joins += "left outer join T_BANCO Banco on Banco.BCO_CODIGO = Terceiro.BCO_CODIGO ";
                        }
                    }
                    break;
                case "Agencia":
                    if (!select.Contains(" Agencia,"))
                    {
                        select += "Terceiro.CLI_BANCO_AGENCIA + '-' + Terceiro.CLI_BANCO_DIGITO_AGENCIA Agencia, ";
                        groupBy += "Terceiro.CLI_BANCO_AGENCIA , Terceiro.CLI_BANCO_DIGITO_AGENCIA ,  ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO  ";
                    }
                    break;
                case "Conta":
                    if (!select.Contains(" Conta,"))
                    {
                        select += "Terceiro.CLI_BANCO_NUMERO_CONTA Conta, ";
                        groupBy += "Terceiro.CLI_BANCO_NUMERO_CONTA , ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO  ";
                    }
                    break;
                case "TipoDaConta":
                    if (!select.Contains(" TipoDaConta,"))
                    {
                        select += "Terceiro.CLI_BANCO_TIPO_CONTA TipoDaConta, ";
                        groupBy += "Terceiro.CLI_BANCO_TIPO_CONTA , ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO  ";
                    }
                    break;
                case "Titular":
                    if (!select.Contains(" Titular,"))
                    {
                        select += "Terceiro.CLI_NOME Titular, ";
                        groupBy += "Terceiro.CLI_NOME , ";

                        if (!joins.Contains(" Terceiro "))
                            joins += "left outer join T_CLIENTE Terceiro on Terceiro.CLI_CGCCPF = Contrato.CLI_CGCCPF_TERCEIRO  ";
                    }
                    break;

                default:
                    if (!count && propriedade.Contains("ValorComponente"))
                    {
                        if (!joins.Contains(" Carga "))
                            joins += "left outer join T_CARGA Carga on Carga.CAR_CODIGO = Contrato.CAR_CODIGO ";

                        select += "SUM(" + propriedade + ".CCF_VALOR_COMPONENTE) " + propriedade + ", ";

                        joins += "left outer join T_CARGA_COMPONENTES_FRETE " + propriedade + " on Carga.CAR_CODIGO = " + propriedade + ".CAR_CODIGO and " + propriedade + ".CFR_CODIGO = " + codigoDinamico + " ";
                    }
                    break;
            }
        }
        
        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoAcrescimoDesconto> ConsultarRelatorioFreteTerceirizadoAcrescimoDesconto(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new Frete.ConsultaFreteTerceirizadoAcrescimoDesconto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoAcrescimoDesconto)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoAcrescimoDesconto>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoAcrescimoDesconto>> ConsultarRelatorioFreteTerceirizadoAcrescimoDescontoAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new Frete.ConsultaFreteTerceirizadoAcrescimoDesconto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoAcrescimoDesconto)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoAcrescimoDesconto>();
        }

        public int ContarConsultaRelatorioFreteTerceirizadoAcrescimoDesconto(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new Frete.ConsultaFreteTerceirizadoAcrescimoDesconto().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoValePedagio> ConsultarRelatorioFreteTerceirizadoValePedagio(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new Frete.ConsultaFreteTerceirizadoValePedagio().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoValePedagio)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoValePedagio>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoValePedagio>> ConsultarRelatorioFreteTerceirizadoValePedagioAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new Frete.ConsultaFreteTerceirizadoValePedagio().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoValePedagio)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoValePedagio>();
        }

        public int ContarConsultaRelatorioFreteTerceirizadoValePedagio(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoValePedagio filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new Frete.ConsultaFreteTerceirizadoValePedagio().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ExportacaoContratoFrete> ConsultarRelatorioExportacaoContratoFrete(DateTime dataEmissao, DateTime dataAberturaCIOT)
        {
            string query = $@"
            SELECT 
                Ciot.CIO_DATA_ABERTURA DataAberturaCIOT, 
	            SUBSTRING(
		            ISNULL((
			            SELECT DISTINCT ', ' + 
				            CAST(CTe.CON_NUM AS VARCHAR(20)) 
			            FROM T_CTE CTe 
				            JOIN T_CARGA_CTE CargaCTE 
					            on CargaCTE.CAR_CODIGO = CargaCiot.CAR_CODIGO 
			            where CTe.CON_CODIGO = CargaCTE.CON_CODIGO 
			            FOR XML PATH('')
		            ), '' ), 3, 1000
	            ) AdicionalCTe,
	            Ciot.CIO_PROTOCOLO_AUTORIZACAO NumeroFormulario,
	            1 SerieFormulario,
	            (
		            select top(1) 
			            CTe.CON_DATAHORAEMISSAO 
		            from t_cte cte 
			            JOIN T_CARGA_CTE CargaCTE 
				            on CargaCTE.CAR_CODIGO = CargaCiot.CAR_CODIGO 
		            where CTe.CON_CODIGO = CargaCTE.CON_CODIGO
	            ) DataEmissao,
	            SUBSTRING(
		            ISNULL((
			            SELECT DISTINCT ', ' + 
				            Remetente.PCT_CPF_CNPJ
			            FROM T_CTE CTe 
				            JOIN T_CTE_PARTICIPANTE Remetente on Remetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE
				            JOIN T_CARGA_CTE CargaCTE on CargaCTE.CAR_CODIGO = CargaCiot.CAR_CODIGO 
			            where CTe.CON_CODIGO = CargaCTE.CON_CODIGO 
			            FOR XML PATH('')
		            ), ''), 3, 1000
	            ) CNPJCPFRemetente,	            
	            SUBSTRING(
		            ISNULL((
			            SELECT DISTINCT ', ' + 
				            Destinatario.PCT_CPF_CNPJ
			            FROM T_CTE CTe 
				            JOIN T_CTE_PARTICIPANTE Destinatario on Destinatario.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE
				            JOIN T_CARGA_CTE CargaCTE on CargaCTE.CAR_CODIGO = CargaCiot.CAR_CODIGO 
			            where CTe.CON_CODIGO = CargaCTE.CON_CODIGO 
			            FOR XML PATH('')
		            ), ''), 3, 1000
	            ) CNPJCPFDestinatario,	
	            (
		            select 
			            sum(CTe.CON_PESO) 
		            from t_cte cte 
			            JOIN T_CARGA_CTE CargaCTE on CargaCTE.CAR_CODIGO = CargaCiot.CAR_CODIGO 
		            where CTe.CON_CODIGO = CargaCTE.CON_CODIGO
	            ) Peso,
	            '' Volume,
	            '' MetrosCubicos,
	            Contrato.CFT_VALOR_FRETE_SUB_CONTRATACAO ValorUnitario,
	            '' ValorFreteBruto,
	            '' ValorFreteLiquido,
	            SUBSTRING((
		            SELECT DISTINCT ', ' + 
			            Motorista.FUN_CPF 
		            FROM T_CARGA_MOTORISTA CargaMotorista
			            INNER JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = CargaMotorista.CAR_MOTORISTA
		            WHERE CargaMotorista.CAR_CODIGO = CargaCiot.CAR_CODIGO 
		            FOR XML PATH('')), 
	            3, 1000) CPFMotorista, 
		        (
			        SELECT VEI.VEI_PLACA 
			        FROM T_VEICULO VEI 
			        WHERE VEI.VEI_CODIGO = Carga.CAR_VEICULO
		        )  PlacaControle,
	            '' PlacaReferencia,
	            CASE 
		            WHEN Contrato.CFT_ADIANTAMENTO > 0 
			            THEN Contrato.CFT_ADIANTAMENTO - Contrato.CFT_VALOR_PEDAGIO
		            ELSE 0.0
	            END ValorItemAdiantamento,
                Contrato.CFT_VALOR_PEDAGIO ValorPedagio,
	            '' NumeroCRTSistemaExterno,
	            '' CentroCustoGerencial,
	            '' DataCancelamento,
	            '' Observacao,
	            '' UnidadeNegocio,
	            0.0 TotalAcrescimos,
	            0.0 TotalDescontos,
	            '' NumeroDocumento,
	            SUBSTRING(
		            ISNULL((
			            SELECT DISTINCT ', ' + 
				            Tomador.PCT_NOME
			            FROM T_CTE CTe 
				            JOIN T_CTE_PARTICIPANTE Tomador on Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE
				            JOIN T_CARGA_CTE CargaCTE on CargaCTE.CAR_CODIGO = CargaCiot.CAR_CODIGO 
			            where CTe.CON_CODIGO = CargaCTE.CON_CODIGO 
			            FOR XML PATH('')
		            ), ''), 3, 1000
	            ) NomeCliente, 
	            SUBSTRING(
		            ISNULL((
			            SELECT DISTINCT ', ' + 
				            Tomador.PCT_CPF_CNPJ
			            FROM T_CTE CTe 				
				            JOIN T_CTE_PARTICIPANTE Tomador on Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE
				            JOIN T_CARGA_CTE CargaCTE on CargaCTE.CAR_CODIGO = CargaCiot.CAR_CODIGO 
			            where CTe.CON_CODIGO = CargaCTE.CON_CODIGO 
			            FOR XML PATH('')
		            ), ''), 3, 1000
	            ) CNPJCPFCliente,
	            '' CentroCustoOriginal
            FROM T_CIOT Ciot
	            JOIN T_CARGA_CIOT CargaCiot on CargaCiot.CIO_CODIGO = Ciot.CIO_CODIGO
	            JOIN T_CARGA Carga on Carga.CAR_CODIGO = CargaCiot.CAR_CODIGO
	            LEFT OUTER JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO
	            JOIN T_CONTRATO_FRETE_TERCEIRO Contrato on Contrato.CAR_CODIGO = CargaCiot.CAR_CODIGO 
            WHERE Ciot.CIO_SITUACAO <> 2 ";

            query += dataEmissao != DateTime.MinValue ? $" AND EXISTS(SELECT CTe.CON_CODIGO FROM T_CARGA_CTE CargaCTe JOIN T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO WHERE CargaCTE.CAR_CODIGO = CargaCiot.CAR_CODIGO AND CTe.CON_DATAHORAEMISSAO >= '{dataEmissao.ToString("yyyy-MM-dd")}' AND CTe.CON_DATAHORAEMISSAO < '{dataEmissao.AddDays(1).ToString("yyyy-MM-dd")}') " : string.Empty;
            query += dataAberturaCIOT != DateTime.MinValue ? $" AND CIOT.CIO_DATA_ABERTURA >= '{dataAberturaCIOT.ToString("yyyy-MM-dd")}' AND CIOT.CIO_DATA_ABERTURA < '{dataAberturaCIOT.AddDays(1).ToString("yyyy-MM-dd")}' " : string.Empty;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);
            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Terceiros.ExportacaoContratoFrete)));
            return nhQuery.SetTimeout(900).List<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ExportacaoContratoFrete>();
        }

        #endregion
    }
}
