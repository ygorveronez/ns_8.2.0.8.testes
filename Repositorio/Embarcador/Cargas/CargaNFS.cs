using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaNFS : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaNFS>
    {
        public CargaNFS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaNFS BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFS>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaNFS> BuscarParaConversao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFS>();
            var result = from obj in query where obj.CargaCTe == null && obj.NotaFiscalServico != null select obj;
            return result.ToList();
        }


        public Dominio.Entidades.Embarcador.Cargas.CargaNFS BuscarPorCodigoNFS(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFS>();
            var result = from obj in query where obj.NotaFiscalServico.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaNFS BuscarPorCodigoNFSe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFS>();
            var result = from obj in query where obj.NotaFiscalServico.NFSe.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaNFS> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFS>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }


        public int ContarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFS>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaNFS> BuscarPendentesPorEmpresa(int codEmpresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFS>();
            var result = from obj in query
                         where obj.NotaFiscalServico == null &&
                            obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
                         select obj;

            if (codEmpresa > 0)
                result = result.Where(obj => obj.Carga.Empresa.Codigo == codEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarPendentesPorEmpresa(int codEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFS>();
            var result = from obj in query
                         where obj.NotaFiscalServico == null &&
                             obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada
                         select obj;

            if (codEmpresa > 0)
                result = result.Where(obj => obj.Carga.Empresa.Codigo == codEmpresa);

            return result.Count();
        }



        public List<Dominio.Entidades.Embarcador.Cargas.CargaNFS> ConsultarNFS(int carga, string StatusNFSe, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFS>();

            var result = from obj in query where obj.Carga.Codigo == carga && obj.NotaFiscalServico != null select obj;

            return result.Fetch(o => o.NotaFiscalServico).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.NotaFiscalServico).ThenFetch(nfs => nfs.NFSe).ThenFetch(nfse => nfse.Tomador)
                .Fetch(obj => obj.NotaFiscalServico).ThenFetch(nfs => nfs.NFSe).ThenFetch(nfse => nfse.LocalidadePrestacaoServico).ThenFetch(loc => loc.Estado)
                .ToList();
        }

        public int ContarConsultaNFS(int carga, string StatusNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFS>();

            var result = from obj in query where obj.Carga.Codigo == carga && obj.NotaFiscalServico != null select obj;

            //if (!string.IsNullOrWhiteSpace(StatusCTe))
            //{
            //    result = result.Where(obj => obj.CTe.Status == StatusCTe);
            //}

            //if (apenasCTesNormais)
            //    result = result.Where(obj => obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

            return result.Count();
        }

    }
}
