using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaFoto : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>
    {
        public CargaEntregaFoto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto> BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto BuscarPrimeiraPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto BuscarUltimaPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega).OrderByDescending(obj => obj.Codigo);
            return result.FirstOrDefault();
        }

        public bool PossuiImagem(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);
            return result.Any();
        }

        public bool PossuiImagemEnviadaAposData(int codigoCargaEntrega, DateTime dataEnvio)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.DataEnvioImagem >= dataEnvio);
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto> ConsultarImagensEntregaPendentesIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();

            query = query.Where(obj => ((bool?)obj.Integrado ?? false) == false);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(o => o.CargaEntrega)
                .ToList();
        }

        public int ContarConsultaImagensEntregaPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();

            query = query.Where(obj => ((bool?)obj.Integrado ?? false) == false);

            return query.Count();
        }

        #endregion
    }
}
