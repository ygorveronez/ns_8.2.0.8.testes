using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class ExcecaoCapacidadeDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento>
    {
        #region Construtores

        public ExcecaoCapacidadeDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeDescarregamento filtrosPesquisa)
        {
            var consultaExcecaoCapacidadeCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento>();

            if (filtrosPesquisa.CodigoCentroDescarregamento > 0)
                consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.CentroDescarregamento.Codigo == filtrosPesquisa.CodigoCentroDescarregamento);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.DataInicial.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaExcecaoCapacidadeCarregamento = consultaExcecaoCapacidadeCarregamento.Where(o => o.DataInicial.Date <= filtrosPesquisa.DataLimite.Value.Date);

            return consultaExcecaoCapacidadeCarregamento;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento BuscarPorCodigo(int codigo)
        {
            var consultaExcecaoCapacidadeDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaExcecaoCapacidadeDescarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento BuscarPorCentroDescarregamento(int codigoCentroDescarregamento, DateTime data, DateTime dataFinal)
        {
            var consultaExcecaoCapacidadeDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento>()
                .Where(o => o.CentroDescarregamento.Codigo == codigoCentroDescarregamento && o.DataInicial <= data.Date && o.DataFinal >= dataFinal.Date);

            return consultaExcecaoCapacidadeDescarregamento.FirstOrDefault();
        }

        public DateTime? BuscarProximaDataPorCentroDescarregamento(int codigoCentroDescarregamento, DateTime data)
        {
            var consultaExcecaoCapacidadeDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento>()
                .Where(o => o.CentroDescarregamento.Codigo == codigoCentroDescarregamento && o.DataInicial > data.Date);

            return consultaExcecaoCapacidadeDescarregamento
                .OrderBy(o => o.DataInicial)
                .Select(o => (DateTime?)o.DataInicial)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeDescarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaExcecaoCapacidadeCarregamento = Consultar(filtrosPesquisa);

            return ObterLista(consultaExcecaoCapacidadeCarregamento, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeDescarregamento filtrosPesquisa)
        {
            var consultaExcecaoCapacidadeCarregamento = Consultar(filtrosPesquisa);

            return consultaExcecaoCapacidadeCarregamento.Count();
        }

        #endregion
    }
}
