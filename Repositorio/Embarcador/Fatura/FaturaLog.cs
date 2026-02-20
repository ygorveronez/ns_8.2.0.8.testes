using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaLog : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaLog>
    {
        public FaturaLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fatura.FaturaLog BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaLog>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaLog> BuscarPorCodigoFaturaIntegracaoTipo(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura tipoLogFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaIntegracao>();
            query = query.Where(obj => obj.Codigo == codigo);

            var queryLog = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaLog>();
            queryLog = queryLog.Where(l => l.TipoLogFatura == tipoLogFatura);
            queryLog = queryLog.Where(l => query.Any(i => i.Fatura == l.Fatura));
            return queryLog.ToList();
        }

    }
}
