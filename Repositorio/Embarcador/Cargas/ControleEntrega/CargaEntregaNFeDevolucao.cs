using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaNFeDevolucao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao>
    {
        public CargaEntregaNFeDevolucao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao> BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);
            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao BuscarControleNotaDevolucao(int codigoCargaEntrega)
            => (from controleNota in SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao>()
                where controleNota.CargaEntregaNFeDevolucao.Codigo == codigoCargaEntrega
                select controleNota).FirstOrDefault();


        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao> BuscarPorChamado(int codigoChamado)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao>();
            var result = query.Where(obj => obj.Chamado.Codigo == codigoChamado);
            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        #endregion
    }
}
