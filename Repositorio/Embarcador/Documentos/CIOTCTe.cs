using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class CIOTCTe : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.CIOTCTe>
    {
        public CIOTCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Documentos.CIOTCTe> Consultar(int codigoCIOT, string propOrdenacao, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOTCTe>();

            query = query.Where(o => o.CIOT.Codigo == codigoCIOT && o.CargaCTe.CTe.ModeloDocumentoFiscal.Numero == "57");

            return query.OrderBy(propOrdenacao + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOTCTe>();

            query = query.Where(o => o.CIOT.Codigo == codigoCIOT && o.CargaCTe.CTe.ModeloDocumentoFiscal.Numero == "57");

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOTCTe>();
            query = query.Where(c => c.CargaCTe.CTe.Codigo == codigoCTe);
            return query.Select(c => c.CIOT)?.FirstOrDefault() ?? null;

        }
    }
}
