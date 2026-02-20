using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Repositorio.Embarcador.RH
{
    public class FolhaLancamento : RepositorioBase<Dominio.Entidades.Embarcador.RH.FolhaLancamento>
    {
        #region Métodos Públicos
        public FolhaLancamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> BuscarFolhaLancamentoMotorista(int codigoMotorista, DateTime dataInicial, DateTime? dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.FolhaLancamento>();

            var result = from obj in query where obj.Funcionario.Codigo == codigoMotorista && obj.DataInicial.Date >= dataInicial.Date select obj;

            if (dataFinal.HasValue)
                result = result.Where(o => o.DataInicial.Date <= dataFinal.Value.Date);

            var queryAcertoFolhaLancamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento>();
            var resultAcertoFolhaLancamento = from obj in queryAcertoFolhaLancamento where obj.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado select obj;
            result = result.Where(o => !resultAcertoFolhaLancamento.Select(p => p.FolhaLancamento).Contains(o));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> Consultar(string descricao, int numeroEvento, int numeroContrato, DateTime dataInicial, DateTime dataFinal, int funcionario, int folhaInformacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.FolhaLancamento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (numeroEvento > 0)
                result = result.Where(obj => obj.NumeroEvento == numeroEvento);

            if (numeroContrato > 0)
                result = result.Where(obj => obj.NumeroContrato == numeroContrato);

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial >= dataInicial);

            if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal <= dataFinal);

            if (funcionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == funcionario);

            if (folhaInformacao > 0)
                result = result.Where(obj => obj.FolhaInformacao.Codigo == folhaInformacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, int numeroEvento, int numeroContrato, DateTime dataInicial, DateTime dataFinal, int funcionario, int folhaInformacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.FolhaLancamento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (numeroEvento > 0)
                result = result.Where(obj => obj.NumeroEvento == numeroEvento);

            if (numeroContrato > 0)
                result = result.Where(obj => obj.NumeroContrato == numeroContrato);

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial >= dataInicial);

            if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal <= dataFinal);

            if (funcionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == funcionario);

            if (folhaInformacao > 0)
                result = result.Where(obj => obj.FolhaInformacao.Codigo == folhaInformacao);

            return result.Count();
        }

        public bool ContemFolha(int codigoFuncionario, int codigoInformacao, DateTime dataInicial, DateTime dataFinal, decimal valor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.FolhaLancamento>();
            var result = from obj in query
                         where
                         obj.Funcionario.Codigo == codigoFuncionario &&
                         obj.FolhaInformacao.Codigo == codigoInformacao &&
                         obj.DataInicial >= dataInicial && obj.DataFinal <= dataFinal &&
                         obj.Valor == valor
                         select obj;

            return result.Count() > 0;
        }
        #endregion

        #region Relatório de Folha de Lançamento

        public IList<Dominio.Relatorios.Embarcador.DataSource.RH.FolhaLancamento> ConsultarRelatorioFolhaLancamento(Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaRelatorioFolhaLancamento filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new Consulta.ConsultaFolhaLancamento().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.RH.FolhaLancamento)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.RH.FolhaLancamento>();
        }

        public int ContarConsultaRelatorioFolhaLancamento(Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaRelatorioFolhaLancamento filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new Consulta.ConsultaFolhaLancamento().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
