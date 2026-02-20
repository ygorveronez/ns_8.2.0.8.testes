using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class PendenciaMotorista : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista>
    {
        public PendenciaMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista> Consultar(int codigoMotorista, decimal valorInicial, decimal valorFinal, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPendenciaMotorista situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista>();

            var result = from obj in query select obj;

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            if (valorInicial > 0)
                result = result.Where(obj => obj.Valor >= valorInicial);

            if (valorFinal > 0)
                result = result.Where(obj => obj.Valor <= valorFinal);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPendenciaMotorista.Todos)
                result = result.Where(o => o.Situacao == situacao);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFim);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.Motorista)
                .ToList();

        }

        public int ContarConsultar(int codigoMotorista, decimal valorInicial, decimal valorFinal, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPendenciaMotorista situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotorista>();

            var result = from obj in query select obj;

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            if (valorInicial > 0)
                result = result.Where(obj => obj.Valor >= valorInicial);

            if (valorFinal > 0)
                result = result.Where(obj => obj.Valor <= valorFinal);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPendenciaMotorista.Todos)
                result = result.Where(o => o.Situacao == situacao);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Date <= dataFim);

            return result.Count();

        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPendenciaMotorista = new ConsultaPendenciaMotorista().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPendenciaMotorista.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PendenciaMotorista> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPendenciaMotorista = new ConsultaPendenciaMotorista().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPendenciaMotorista.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PendenciaMotorista)));

            return consultaPendenciaMotorista.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PendenciaMotorista>();
        }
    }
}
