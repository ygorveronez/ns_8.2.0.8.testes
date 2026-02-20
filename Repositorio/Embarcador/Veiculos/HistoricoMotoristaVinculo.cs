using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Repositorio.Embarcador.Veiculos.Consulta;

namespace Repositorio.Embarcador.Veiculos
{
    public class HistoricoMotoristaVinculo : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculo>
    {
        public HistoricoMotoristaVinculo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public HistoricoMotoristaVinculo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculo BuscarPorMotorista(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculo>();
            var result = from obj in query where obj.Motorista.Codigo == codigo select obj;
            return result.OrderBy("Codigo desc").FirstOrDefault();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoMotoristaCentro>> ConsultarRelatorioHistoricoMotoristaCentro(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotorista = new ConsultaHistoricoMotoristaVinculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMotorista.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoMotoristaCentro)));

            return await consultaMotorista.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoMotoristaCentro>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoMotoristaCentro>> ConsultarRelatorioHistoricoMotoristaCentroAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotorista = new ConsultaHistoricoMotoristaVinculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaMotorista.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoMotoristaCentro)));

            return await consultaMotorista.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoMotoristaCentro>();
        }

        public async Task<int> ContarConsultaRelatorioHistoricoMotoristaCentro(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaMotorista = new ConsultaHistoricoMotoristaVinculo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return await consultaMotorista.SetTimeout(600).UniqueResultAsync<int>();
        }
    }
}
