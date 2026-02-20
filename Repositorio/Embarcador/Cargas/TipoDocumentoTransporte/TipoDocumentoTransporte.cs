using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.TipoDocumentoTransporte
{
    public class TipoDocumentoTransporte : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte>
    {
        public TipoDocumentoTransporte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.TipoDocumentoTransporte.FiltroTipoDocumentoTransporte filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.TipoDocumentoTransporte.FiltroTipoDocumentoTransporte filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }
        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.TipoDocumentoTransporte.FiltroTipoDocumentoTransporte filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDocumentoTransporte>();
            query = from obj in query select obj;

            query = query.Where(obj => obj.Status == filtrosPesquisa.Status);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));


            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIntegracao))
                query = query.Where(obj => obj.CodigoIntegracao == filtrosPesquisa.CodigoIntegracao);

            return query;
        }
        #endregion
    }
}
