using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaNFe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaNFe>
    {
        public CargaNFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> ConsultarPorCarga(int codigoCarga, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>();

            var result = from obj in query select obj;
            if (codigoCarga > 0)
                result = result.Where(obj => obj.Carga.Codigo == codigoCarga);

            return result.Select(o => o.NotaFiscal).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public List<int> BuscarCodigosNFesAutorizadosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Autorizado);

            query = query.OrderBy("NotaFiscal.Numero");
            return query.Select(o => o.NotaFiscal.Codigo).ToList();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaNFe> BuscarCargasEmEmissao(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>();

            query = query.Where(obj => obj.Carga.EmitindoNFeRemessa && !obj.Carga.problemaEmissaoNFeRemessa);

            return query.Fetch(obj => obj.NotaFiscal).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaNFe> BuscarPorCargas(List<int> codigosCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>()
                .Where(obj => codigosCargas.Contains(obj.CargaOrigem.Codigo));
            
            
            return query
                .Fetch(obj => obj.NotaFiscal)
                .ToList();
        }

        public int ContarConsultaPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>();

            var result = from obj in query select obj;
            if (codigoCarga > 0)
                result = result.Where(obj => obj.Carga.Codigo == codigoCarga);

            return result.Count();
        }

        public int ContarPorCargaEStatus(int codigoCarga, Dominio.Enumeradores.StatusNFe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.NotaFiscal.Status == status select obj;

            return result.Count();
        }

        public bool NotaJaGeradaNaCarga(int codigoCarga, string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.NotaFiscal.ReferenciaNFe.Any(n => n.Chave == chave) select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaNFe BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>()
                .Where(obj => obj.CargaOrigem.Codigo == codigoCarga);


            return query
                .Fetch(obj => obj.NotaFiscal)
                .FirstOrDefault();
        }
    }
}
