using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using Repositorio.Embarcador.WMS;

namespace Repositorio.Embarcador.CRM
{
    public class Prospeccao : RepositorioBase<Dominio.Entidades.Embarcador.CRM.Prospeccao>
    {

        public Prospeccao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Prospeccao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.CRM.Prospeccao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.Prospeccao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CRM.Prospeccao BuscarPorNome(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.Prospeccao>();
            var result = from obj in query where obj.Nome.Equals(nome) select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.CRM.Prospeccao> _Consultar(int usuario, string nomeCliente, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao? situacao, DateTime dataLancamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.Prospeccao>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            if (!string.IsNullOrWhiteSpace(nomeCliente))
                result = result.Where(o => o.Nome.Contains(nomeCliente));

            if (dataLancamento.Date > DateTime.MinValue)
                result = result.Where(o => o.DataLancamento >= dataLancamento && o.DataLancamento <= dataLancamento.AddDays(1));

            if (situacao != null)
                result = result.Where(o => o.Situacao == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.CRM.Prospeccao> Consultar(int usuario, string nomeCliente, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao? situacao, DateTime dataLancamento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(usuario, nomeCliente, codigoEmpresa, situacao, dataLancamento);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int usuario, string nomeCliente, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProspeccao? situacao, DateTime dataLancamento)
        {
            var result = _Consultar(usuario, nomeCliente, codigoEmpresa, situacao, dataLancamento);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.CRM.Prospeccao> ConsultarHistorico(int cliente, int codigo, int codigoEmpresa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.Prospeccao>();

            var result = from obj in query where obj.Cliente.Codigo == cliente && obj.Codigo != codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaHistorico(int cliente, int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.Prospeccao>();

            var result = from obj in query where obj.Cliente.Codigo == cliente && obj.Codigo != codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.CRM.Prospeccao> ConsultarRelatorioProspeccao(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioProspeccao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = new ConsultaProspeccao().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            result.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CRM.Prospeccao)));

            return result.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CRM.Prospeccao>();
        }

        public int ContarConsultaRelatorioProspeccao(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioProspeccao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var result = new ConsultaProspeccao().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return result.SetTimeout(600).UniqueResult<int>();
        }



    }
}
