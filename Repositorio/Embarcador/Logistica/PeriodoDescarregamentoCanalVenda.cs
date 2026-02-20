using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class PeriodoDescarregamentoCanalVenda : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda>
    {
        public PeriodoDescarregamentoCanalVenda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> BuscarPorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            var consultaPeriodoDescarregamentoCanalVenda = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda>()
                .Where(o => o.PeriodoDescarregamento.CentroDescarregamento.Codigo == codigoCentroDescarregamento);

            return consultaPeriodoDescarregamentoCanalVenda.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> BuscarPorExcecao(int codigoExcecaoCapacidadeDescarregamento)
        {
            var consultaPeriodoDescarregamentoCanalVenda = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda>()
                .Where(o => o.PeriodoDescarregamento.ExcecaoCapacidadeDescarregamento.Codigo == codigoExcecaoCapacidadeDescarregamento);

            return consultaPeriodoDescarregamentoCanalVenda.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> BuscarPorPeriodoDescarregamento(int codigoPeriodoDescarregamento)
        {
            var consultaPeriodoDescarregamentoCanalVenda = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda>()
                .Where(o => o.PeriodoDescarregamento.Codigo == codigoPeriodoDescarregamento);

            return consultaPeriodoDescarregamentoCanalVenda.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> BuscarPorPeriodosDescarregamento(List<int> codigosPeriodosDescarregamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosPeriodosDescarregamento.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda> periodoDescarregamentoCanalVendaRetornar = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                periodoDescarregamentoCanalVendaRetornar.AddRange(query.Where(o => codigosPeriodosDescarregamento.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.PeriodoDescarregamento.Codigo)).ToList());

            return periodoDescarregamentoCanalVendaRetornar.ToList();
        }

        public bool ExistePorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            var consultaPeriodoDescarregamentoCanalVenda = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda>()
                .Where(o => o.PeriodoDescarregamento.CentroDescarregamento.Codigo == codigoCentroDescarregamento);

            return consultaPeriodoDescarregamentoCanalVenda.Count() > 0;
        }

        public bool ExisteRegistroCadastrado()
        {
            var consultaPeriodoDescarregamentoCanalVenda = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda>();

            return consultaPeriodoDescarregamentoCanalVenda.Count() > 0;
        }
        
        public Task<bool> ExisteRegistroCadastradoAsync()
        {
            var consultaPeriodoDescarregamentoCanalVenda = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda>();

            return consultaPeriodoDescarregamentoCanalVenda.AnyAsync();
        }
    }
}
