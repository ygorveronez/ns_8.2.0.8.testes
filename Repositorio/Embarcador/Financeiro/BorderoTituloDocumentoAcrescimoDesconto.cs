using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class BorderoTituloDocumentoAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto>
    {
        public BorderoTituloDocumentoAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto> BuscarPorBorderoTituloDocumento(int codigoBorderoTituloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.BorderoTituloDocumento.Codigo == codigoBorderoTituloDocumento);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto BuscarPorCodigo(int codigoAcrescimoDesconto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.Codigo == codigoAcrescimoDesconto);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto> Consultar(int codigoBorderoTituloDocumento, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.BorderoTituloDocumento.Codigo == codigoBorderoTituloDocumento);

            return query.OrderBy(propOrdena + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsulta(int codigoBorderoTituloDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto>();

            query = query.Where(o => o.BorderoTituloDocumento.Codigo == codigoBorderoTituloDocumento);

            return query.Count();
        }
    }
}
