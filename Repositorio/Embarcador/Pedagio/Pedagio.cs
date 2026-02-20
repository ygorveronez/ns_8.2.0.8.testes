using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedagio
{
    public class Pedagio : RepositorioBase<Dominio.Entidades.Embarcador.Pedagio.Pedagio>
    {
        public Pedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Pedagio(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedagio.Pedagio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> BuscarPorPlaca(string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
            var result = from obj in query where obj.Veiculo.Placa.Equals(placa) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedagio.Pedagio BuscarPorDados(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, string placa, DateTime data, string rodovia, string praca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
            var result = from obj in query
                         where obj.Veiculo.Placa.Equals(placa) && obj.Rodovia.Equals(rodovia) && obj.Praca.Equals(praca) && obj.TipoPedagio == tipoPedagio
                         select obj;

            result = result.Where(o => o.Data == data);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, DateTime dataImportacao, int codigoEmpresa, string placa, string rodovia, string praca, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(obj => obj.Veiculo.Placa.Contains(placa));

            if (!string.IsNullOrWhiteSpace(rodovia))
                result = result.Where(obj => obj.Rodovia.Contains(rodovia));

            if (!string.IsNullOrWhiteSpace(praca))
                result = result.Where(obj => obj.Praca.Contains(praca));

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFim);

            if (dataImportacao != DateTime.MinValue)
                result = result.Where(obj => obj.DataImportacao == dataImportacao);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Todos)
                result = result.Where(o => o.SituacaoPedagio == situacao);

            if (tipoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Todos)
                result = result.Where(o => o.TipoPedagio == tipoPedagio);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();

        }

        public List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, int codigoEmpresa, int veiculo, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();

            var result = from obj in query select obj;

            if (veiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == veiculo);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFim);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Todos)
                result = result.Where(o => o.SituacaoPedagio == situacao);

            if (tipoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Todos)
                result = result.Where(o => o.TipoPedagio == tipoPedagio);

            return result.ToList();

        }

        public int ContarConsultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, DateTime dataImportacao, int codigoEmpresa, string placa, string rodovia, string praca, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(placa))
                result = result.Where(obj => obj.Veiculo.Placa.Contains(placa));

            if (!string.IsNullOrWhiteSpace(rodovia))
                result = result.Where(obj => obj.Rodovia.Contains(rodovia));

            if (!string.IsNullOrWhiteSpace(praca))
                result = result.Where(obj => obj.Praca.Contains(praca));

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFim);

            if (dataImportacao != DateTime.MinValue)
                result = result.Where(obj => obj.DataImportacao == dataImportacao);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Todos)
                result = result.Where(o => o.SituacaoPedagio == situacao);

            if (tipoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Todos)
                result = result.Where(o => o.TipoPedagio == tipoPedagio);

            return result.Count();

        }

        public List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> BuscarPedagioPorVeiculoDataSemAcerto(List<int> codigosVeiculos, DateTime dataInicial, DateTime dataFinal)
        {
            var queryPedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
            var resultPedagio = from obj in queryPedagio select obj;

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio>();
            var resultAcerto = from obj in queryAcerto where obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento || obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado select obj;

            if (codigosVeiculos != null && codigosVeiculos.Count > 0)
            {
                resultPedagio = resultPedagio.Where(obj => codigosVeiculos.Contains(obj.Veiculo.Codigo));// (from p in acertoViagem.Veiculos select p.Veiculo).Contains(obj.Veiculo));                                
            }

            resultPedagio = resultPedagio.Where(obj => !(from p in resultAcerto select p.Pedagio).Contains(obj));
            if (dataFinal > DateTime.MinValue)
                resultPedagio = resultPedagio.Where(obj => obj.Data <= dataFinal);

            return resultPedagio.Timeout(5000).ToList();

        }

        public List<int> ConsultarSeExistePedagioPendente(int codigoEmpresa, DateTime dataFechamento, SituacaoPedagio[] situacaoPedagio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();

            query = query.Where(o => o.Data < dataFechamento.AddDays(1).Date && situacaoPedagio.Contains(o.SituacaoPedagio));

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Empresa.Codigo == codigoEmpresa);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> BuscarPedagioPorVeiculoDataSemAcerto(int codigoVeiculo, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();

            query = query.Where(o => o.Data <= dataFinal && o.Veiculo.Codigo == codigoVeiculo && !o.AcertosViagem.Any(a => a.AcertoViagem.Situacao != SituacaoAcertoViagem.Cancelado));

            return query.Timeout(5000).ToList();

            //var queryPedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
            //var resultPedagio = from obj in queryPedagio select obj;

            //var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio>();
            //var resultAcerto = from obj in queryAcerto where obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento || obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado select obj;

            //resultPedagio = resultPedagio.Where(obj => obj.Data <= dataFinal);
            //resultPedagio = resultPedagio.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            //resultPedagio = resultPedagio.Where(obj => !(from p in resultAcerto select p.Pedagio).Contains(obj));
            //resultPedagio = resultPedagio.Where(obj => obj.Data <= dataFinal);

            //return resultPedagio.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> ConsultarPedagiosDoAcertoViagem(DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, int codigoEmpresa, string praca, DateTime data, int codigoAcertoViagem, int codigoVeiculo, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var queryPedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
            var resultPedagio = from obj in queryPedagio select obj;

            if (!string.IsNullOrWhiteSpace(praca))
                resultPedagio = resultPedagio.Where(obj => obj.Praca.Contains(praca));

            if (data > DateTime.MinValue && dataFinal == DateTime.MinValue)
                resultPedagio = resultPedagio.Where(obj => obj.Data.Date >= data.Date);
            else if (data == DateTime.MinValue && dataFinal > DateTime.MinValue)
                resultPedagio = resultPedagio.Where(obj => obj.Data.Date <= dataFinal.Date);
            else if (data > DateTime.MinValue && dataFinal > DateTime.MinValue)
                resultPedagio = resultPedagio.Where(obj => obj.Data.Date >= data.Date && obj.Data.Date <= dataFinal.Date);

            if (codigoVeiculo > 0)
                resultPedagio = resultPedagio.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (codigoEmpresa > 0)
                resultPedagio = resultPedagio.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (tipoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Todos)
                resultPedagio = resultPedagio.Where(o => o.TipoPedagio == tipoPedagio);

            if (acertoViagem != null && acertoViagem.Veiculos != null)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();
                var resultVeiculo = from obj in queryVeiculos where obj.AcertoViagem.Codigo == codigoAcertoViagem select obj;
                resultPedagio = resultPedagio.Where(obj => resultVeiculo.Any(c => c.Veiculo == obj.Veiculo));//resultVeiculo.Select(a => a.Veiculo).Contains(obj.Veiculo));

                if (acertoViagem.DataFinal.HasValue)
                    resultPedagio = resultPedagio.Where(obj => obj.Data >= acertoViagem.DataInicial && obj.Data <= acertoViagem.DataFinal.Value.AddDays(1));
                else
                    resultPedagio = resultPedagio.Where(obj => obj.Data >= acertoViagem.DataInicial);
            }


            return resultPedagio.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public List<int> BuscarCodigosPorDataAlteracao(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedagio.Pedagio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();

            query = query.Where(o => o.DataAlteracao > dataUltimoProcessamento && o.DataAlteracao <= dataProcessamentoAtual);

            return query.Select(o => o.Codigo).ToList();
        }

        public int ContarConsultarPedagiosDoAcertoViagem(DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, int codigoEmpresa, string praca, DateTime data, int codigoAcertoViagem, int codigoVeiculo, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem)
        {
            var queryPedagio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();
            var resultPedagio = from obj in queryPedagio select obj;

            if (!string.IsNullOrWhiteSpace(praca))
                resultPedagio = resultPedagio.Where(obj => obj.Praca.Contains(praca));

            if (data > DateTime.MinValue && dataFinal == DateTime.MinValue)
                resultPedagio = resultPedagio.Where(obj => obj.Data.Date >= data.Date);
            else if (data == DateTime.MinValue && dataFinal > DateTime.MinValue)
                resultPedagio = resultPedagio.Where(obj => obj.Data.Date <= dataFinal.Date);
            else if (data > DateTime.MinValue && dataFinal > DateTime.MinValue)
                resultPedagio = resultPedagio.Where(obj => obj.Data.Date >= data.Date && obj.Data.Date <= dataFinal.Date);

            if (codigoVeiculo > 0)
                resultPedagio = resultPedagio.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (codigoEmpresa > 0)
                resultPedagio = resultPedagio.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (tipoPedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Todos)
                resultPedagio = resultPedagio.Where(o => o.TipoPedagio == tipoPedagio);

            if (acertoViagem != null && acertoViagem.Veiculos != null)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo>();
                var resultVeiculo = from obj in queryVeiculos where obj.AcertoViagem.Codigo == codigoAcertoViagem select obj;
                resultPedagio = resultPedagio.Where(obj => resultVeiculo.Any(c => c.Veiculo == obj.Veiculo));//resultVeiculo.Select(a => a.Veiculo).Contains(obj.Veiculo));

                if (acertoViagem.DataFinal.HasValue)
                    resultPedagio = resultPedagio.Where(obj => obj.Data >= acertoViagem.DataInicial && obj.Data <= acertoViagem.DataFinal.Value.AddDays(1));
                else
                    resultPedagio = resultPedagio.Where(obj => obj.Data >= acertoViagem.DataInicial);
            }

            return resultPedagio.Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> ConsultarParaFechamento(int codigoVeiculo, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedagio.Pedagio> result = ConsultarParaFechamento(codigoVeiculo, dataInicio, dataFim, situacao);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarParaFechamento(int codigoVeiculo, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedagio.Pedagio> result = ConsultarParaFechamento(codigoVeiculo, dataInicio, dataFim, situacao);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedagio.Pedagio> ConsultarParaFechamento(int codigoVeiculo, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedagio.Pedagio>();

            var result = from obj in query where obj.FechamentoPedagio == null && obj.TipoPedagio == TipoPedagio.Debito select obj;

            if (codigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFim);

            if (situacao != null)
                result = result.Where(obj => obj.SituacaoPedagio == situacao || obj.SituacaoPedagio == SituacaoPedagio.Todos);

            return result;
        }

        #endregion

        #region Relatório de Pedágios

        public int ContarConsultaRelatorioPedagio(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, List<PropriedadeAgrupamento> agrupamentos, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, decimal valorInicial, decimal valorFinal, List<SituacaoPedagio> situacoes)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioPedagio(tipoPedagio, true, agrupamentos, codigoVeiculo, dataInicial, dataFinal, valorInicial, valorFinal, situacoes, "", "", "", "", 0, 0));

            return query.SetTimeout(300).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Pedagio> ConsultarRelatorioPedagio(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, List<PropriedadeAgrupamento> agrupamentos, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, decimal valorInicial, decimal valorFinal, List<SituacaoPedagio> situacoes, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioPedagio(tipoPedagio, false, agrupamentos, codigoVeiculo, dataInicial, dataFinal, valorInicial, valorFinal, situacoes, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Pedagio)));

            return query.SetTimeout(300).List<Dominio.Relatorios.Embarcador.DataSource.Frota.Pedagio>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Pedagio>> ConsultarRelatorioPedagioAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, List<PropriedadeAgrupamento> agrupamentos, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, decimal valorInicial, decimal valorFinal, List<SituacaoPedagio> situacoes, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioPedagio(tipoPedagio, false, agrupamentos, codigoVeiculo, dataInicial, dataFinal, valorInicial, valorFinal, situacoes, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Pedagio)));

            return await query.SetTimeout(300).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.Pedagio>();
        }

        private string ObterSelectConsultaRelatorioPedagio(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio, bool count, List<PropriedadeAgrupamento> agrupamentos, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, decimal valorInicial, decimal valorFinal, List<SituacaoPedagio> situacoes, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                  groupBy = string.Empty,
                  joins = string.Empty,
                  where = string.Empty,
                  orderBy = string.Empty;

            for (var i = agrupamentos.Count - 1; i >= 0; i--)
                SetarSelectConsultaRelatorioPedagio(agrupamentos[i].Propriedade, agrupamentos[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereConsultaRelatorioPedagio(ref where, ref groupBy, ref joins, codigoVeiculo, dataInicial, dataFinal, valorInicial, valorFinal, situacoes, tipoPedagio);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectConsultaRelatorioPedagio(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
                   " from T_PEDAGIO Pedagio " + joins +
                   " where 1=1" + where +
                   (groupBy.Length > 0 ? " group by " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");
        }

        private void SetarWhereConsultaRelatorioPedagio(ref string where, ref string groupBy, ref string joins, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, decimal valorInicial, decimal valorFinal, List<SituacaoPedagio> situacoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio)
        {
            if (tipoPedagio == TipoPedagio.Debito)
                where += " and Pedagio.PED_TIPO = 1";
            else if (tipoPedagio == TipoPedagio.Credito)
                where += " and Pedagio.PED_TIPO = 2";

            if (codigoVeiculo > 0)
                where += " and Pedagio.VEI_CODIGO = " + codigoVeiculo.ToString();

            if (dataInicial != DateTime.MinValue)
                where += " and Pedagio.PED_DATA >= '" + dataInicial.ToString("yyyy-MM-dd") + "'";

            if (dataFinal != DateTime.MinValue)
                where += " and Pedagio.PED_DATA < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (valorInicial > 0m)
                where += " and Pedagio.PED_VALOR >= " + valorInicial.ToString("F2").Replace(",", ".");

            if (valorFinal > 0m)
                where += " and Pedagio.PED_VALOR < " + valorFinal.ToString("F2").Replace(",", ".");

            if (situacoes != null && situacoes.Count > 0)
                where += " and Pedagio.PED_SITUACAO in (" + string.Join(",", situacoes.Select(o => o.ToString("D"))) + ")";
        }

        private void SetarSelectConsultaRelatorioPedagio(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select += "Pedagio.PED_CODIGO Codigo, ";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo,"))
                    {
                        select += "Veiculo.VEI_PLACA Veiculo, ";

                        if (!joins.Contains(" Veiculo "))
                            joins += " left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Pedagio.VEI_CODIGO ";
                    }
                    break;
                case "DataPassagem":
                    if (!select.Contains(" DataPassagem,"))
                    {
                        select += "Pedagio.PED_DATA DataPassagem, ";
                    }
                    break;
                case "Praca":
                    if (!select.Contains(" Praca,"))
                    {
                        select += "Pedagio.PED_PRACA Praca, ";
                    }
                    break;
                case "Rodovia":
                    if (!select.Contains(" Rodovia,"))
                    {
                        select += "Pedagio.PED_RODOVIA Rodovia, ";
                    }
                    break;
                case "Situacao":
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao,"))
                    {
                        select += "Pedagio.PED_SITUACAO Situacao, ";
                    }
                    break;
                case "TipoPedagio":
                case "DescricaoTipoPedagio":
                    if (!select.Contains(" TipoPedagio,"))
                    {
                        select += "Pedagio.PED_TIPO TipoPedagio, ";
                    }
                    break;
                case "Valor":
                    if (!select.Contains(" Valor,"))
                    {
                        select += "Pedagio.PED_VALOR Valor, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "Pedagio.PED_OBSERVACAO Observacao, ";
                    }
                    break;
            }
        }

        #endregion
    }
}
