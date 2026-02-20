using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico>
    {
        #region Construtores

        public FilaCarregamentoVeiculoHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico> BuscarPorFilaCarregamentoVeiculo(int codigo)
        {
            var consultaHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico>()
                .Where(o => o.FilaCarregamentoVeiculo.Codigo == codigo);

            return consultaHistorico.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico BuscarUltimoPorFilaCarregamentoVeiculoETipo(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilaCarregamentoVeiculoHistorico? tipo)
        {
            var consultaHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico>()
                .Where(o => o.FilaCarregamentoVeiculo.Codigo == codigo);

            if (tipo.HasValue)
                consultaHistorico = consultaHistorico.Where(o => o.Tipo == tipo.Value);

            return consultaHistorico.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.FilaCarregamentoVeiculoHistorico> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFilaCarregamentoHistorico = new Repositorio.Embarcador.Logistica.Consulta.ConsultaFilaCarregamentoVeiculoHistorico().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaFilaCarregamentoHistorico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.FilaCarregamentoVeiculoHistorico)));

            return consultaFilaCarregamentoHistorico.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.FilaCarregamentoVeiculoHistorico>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaFilaCarregamentoHistorico = new Repositorio.Embarcador.Logistica.Consulta.ConsultaFilaCarregamentoVeiculoHistorico().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaFilaCarregamentoHistorico.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
