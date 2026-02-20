using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.OcultarInformacoesCarga
{
    public class OcultarInformacoesCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga>
    {
        #region Construtores

        public OcultarInformacoesCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public bool PossuiOcultarInformacoesCarga(int codigoUsuario, int codigoPerfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga>();

            var result = query.Where(o => o.Usuarios.Any(u => u.Codigo == codigoUsuario) || o.PerfisAcesso.Any(pa => pa.Codigo == codigoPerfil));

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga BuscarOcultarInformacoesPorUsuarioEPerfil(int codigoUsuario, int codigoPerfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga>();

            var result = query.Where(o => o.Usuarios.Any(u => u.Codigo == codigoUsuario) || o.PerfisAcesso.Any(pa => pa.Codigo == codigoPerfil));

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.FiltroPesquisaOcultarInformacoesCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.FiltroPesquisaOcultarInformacoesCarga filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga.FiltroPesquisaOcultarInformacoesCarga filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga>();

            query = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            return query;
        }
        #endregion
    }
}
