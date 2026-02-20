using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Repositorio.Embarcador.Veiculos.Consulta;

namespace Repositorio.Embarcador.Veiculos
{
    public class HistoricoVeiculoVinculo : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo>
    {
        public HistoricoVeiculoVinculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo BuscarPorVeiculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo>();
            var result = from obj in query where obj.Veiculo.Codigo == codigo select obj;
            return result.OrderBy("Codigo desc").FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculo> ConsultarRelatorioHistoricoVeiculoVinculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCheque = new ConsultaHistoricoVeiculoVinculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCheque.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculo)));

            return consultaCheque.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculo>();
        }

        public int ContarConsultaRelatorioHistoricoVeiculoVinculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCheque = new ConsultaHistoricoVeiculoVinculo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCheque.SetTimeout(600).UniqueResult<int>();
        }

    }
}
