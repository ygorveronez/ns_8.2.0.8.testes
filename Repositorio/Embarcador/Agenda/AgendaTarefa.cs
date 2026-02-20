using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Agenda
{
    public class AgendaTarefa : RepositorioBase<Dominio.Entidades.Embarcador.Agenda.AgendaTarefa>
    {
        public AgendaTarefa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Agenda.AgendaTarefa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Agenda.AgendaTarefa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Agenda.AgendaTarefa> BuscarTarefasPendentes(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Agenda.AgendaTarefa>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && (obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAgendaTarefa.Aberto || obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAgendaTarefa.EmAndamento) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Agenda.AgendaTarefa> Consultar(string observacao, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAgendaTarefa status, int codigoFuncionario, double cliente, int codigoEmpresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Agenda.AgendaTarefa>();

            var result = from obj in query select obj;

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial.Date >= dataInicial.Date && obj.DataFinal.Date <= dataFinal.Date);
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial.Date >= dataInicial.Date);
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal.Date <= dataFinal.Date);

            if ((int)status > 0)
                result = result.Where(obj => obj.Status == status);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(obj => obj.Observacao.Contains(observacao));

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == codigoFuncionario);

            if (cliente > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
            else
                return result.Distinct().ToList();
        }

        public int ContarConsulta(string observacao, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAgendaTarefa status, int codigoFuncionario, double cliente, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Agenda.AgendaTarefa>();

            var result = from obj in query select obj;

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial.Date >= dataInicial.Date && obj.DataFinal.Date <= dataFinal.Date);
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataInicial.Date >= dataInicial.Date);
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataFinal.Date <= dataFinal.Date);

            if ((int)status > 0)
                result = result.Where(obj => obj.Status == status);

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(obj => obj.Observacao.Contains(observacao));

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == codigoFuncionario);

            if (cliente > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == cliente);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Distinct().Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CRM.AgendaTarefas> ConsultarRelatorioAgendaTarefas(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioAgendaTarefas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAgendaTarefas = new Repositorio.Embarcador.CRM.ConsultaAgendaTarefas().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaAgendaTarefas.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CRM.AgendaTarefas)));

            return consultaAgendaTarefas.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.CRM.AgendaTarefas>();
        }

        public int ContarConsultaRelatorioAgendaTarefas(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioAgendaTarefas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaAgendaTarefas = new Repositorio.Embarcador.CRM.ConsultaAgendaTarefas().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaAgendaTarefas.SetTimeout(1200).UniqueResult<int>();
        }
    }
}
