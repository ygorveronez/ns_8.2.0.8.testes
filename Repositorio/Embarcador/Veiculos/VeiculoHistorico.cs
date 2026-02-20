using Repositorio.Embarcador.Veiculos.Consulta;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoHistorico>
    {
        public VeiculoHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public VeiculoHistorico(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoHistorico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoHistorico>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.VeiculoHistorico BuscarPorVeiculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoHistorico>();
            var result = from obj in query where obj.Veiculo.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculo> ConsultarRelatorioHistoricoVeiculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCheque = new ConsultaHistoricoVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCheque.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculo)));

            return consultaCheque.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculo>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculo>> ConsultarRelatorioHistoricoVeiculoAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCheque = new ConsultaHistoricoVeiculo().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCheque.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculo)));

            return await consultaCheque.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculo>();
        }

        public int ContarConsultaRelatorioHistoricoVeiculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCheque = new ConsultaHistoricoVeiculo().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCheque.SetTimeout(600).UniqueResult<int>();
        }
    }
}
