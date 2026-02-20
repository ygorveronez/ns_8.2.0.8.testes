using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public class RestricaoRodagem : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem>
    {
        #region Construtores

        public RestricaoRodagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem> ConsultarRestricaoRodagem(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRestricaoRodagem filtrosPesquisa)
        {
            var consultaRestricaoRodagem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaRestricaoRodagem = consultaRestricaoRodagem.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.DiaSemana.HasValue)
                consultaRestricaoRodagem = consultaRestricaoRodagem.Where(o => o.DiaSemana == filtrosPesquisa.DiaSemana.Value);

            if (filtrosPesquisa.FinalPlaca.HasValue)
                consultaRestricaoRodagem = consultaRestricaoRodagem.Where(o => o.FinalPlaca == filtrosPesquisa.FinalPlaca.Value);

            return consultaRestricaoRodagem;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem BuscarPorCodigo(int codigo)
        {
            var restricaoRodagem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return restricaoRodagem;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRestricaoRodagem filtrosPesquisa)
        {
            return Consultar(filtrosPesquisa, parametrosConsulta: null);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRestricaoRodagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRestricaoRodagem = ConsultarRestricaoRodagem(filtrosPesquisa);

            return ObterLista(consultaRestricaoRodagem, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRestricaoRodagem filtrosPesquisa)
        {
            var consultaRestricaoRodagem = ConsultarRestricaoRodagem(filtrosPesquisa);

            return consultaRestricaoRodagem.Count();
        }

        #endregion
    }
}
