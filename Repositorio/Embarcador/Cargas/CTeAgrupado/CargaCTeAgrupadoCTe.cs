using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas.CTeAgrupado
{
    public class CargaCTeAgrupadoCTe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>
    {
        public CargaCTeAgrupadoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe BuscarPorCodigoComFetch(int codigoCargaCTeAgrupadoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>();

            query = query.Where(o => o.Codigo == codigoCargaCTeAgrupadoCTe);

            return query.Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal).Fetch(o => o.CTe).ThenFetch(o => o.Empresa).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> BuscarPorCargaCTeAgrupado(int codigoCargaCTeAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado);

            return query.ToList();
        }

        public bool ExisteCTeDiferenteDeAutorizado(int codigoCargaCTeAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado && o.CTe.Status != "A");

            return query.Select(o => o.Codigo).Any();
        }

        public List<int> BuscarCodigosPorCargaCTeAgrupado(int codigoCargaCTeAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado);

            return query.Select(o => o.Codigo).ToList();
        }

        public bool ExistePorCargaCTeAgrupado(int codigoCargaCTeAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExistePorCargaCTeAgrupadoESituacao(int codigoCargaCTeAgrupado, List<string> situacoesCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado && situacoesCTe.Contains(o.CTe.Status));

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExistePorCargaCTeAgrupadoESituacaoDiff(int codigoCargaCTeAgrupado, string situacaoCTeDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado && o.CTe.Status != situacaoCTeDiff);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExistePorCargaCTeAgrupadoESituacaoDiff(int codigoCargaCTeAgrupado, List<string> situacaoCTeDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado && !situacaoCTeDiff.Contains(o.CTe.Status));

            return query.Select(o => o.Codigo).Any();
        }

        public List<int> BuscarNumeroCTesPorCTeAgrupado(int codigoCargaCTeAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado);

            return query.Select(o => o.CTe.Numero).ToList();
        }

        #region Consulta Por CT-e Agrupado

        public int ContarConsultaPorCargaCTeAgrupado(int codigoCargaCTeAgrupado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = ObterQueryConsultaPorCargaCTeAgrupado(codigoCargaCTeAgrupado);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> ConsultarPorCargaCTeAgrupado(int codigoCargaCTeAgrupado, string propOrdenacao, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = ObterQueryConsultaPorCargaCTeAgrupado(codigoCargaCTeAgrupado);

            return query.Fetch(o => o.CTe).ThenFetch(o => o.Remetente)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                        .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Empresa)
                        .ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> ObterQueryConsultaPorCargaCTeAgrupado(int codigoCargaCTeAgrupado, string propOrdenacao = "", string dirOrdena = "", int inicio = 0, int limite = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe>();

            query = query.Where(o => o.CargaCTeAgrupado.Codigo == codigoCargaCTeAgrupado);

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propOrdenacao + " " + dirOrdena);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query;
        }

        #endregion
    }
}
