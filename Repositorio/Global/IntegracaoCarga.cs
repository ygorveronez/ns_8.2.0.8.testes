using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class IntegracaoCarga : RepositorioBase<Dominio.Entidades.IntegracaoCarga>, Dominio.Interfaces.Repositorios.IntegracaoCarga
    {
        public IntegracaoCarga(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.IntegracaoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.IntegracaoCarga BuscarPorCargaUniadade(string numeroCarga, string numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCarga>();
            var result = from obj in query where obj.NumeroDaCarga == numeroCarga && obj.NumeroDaUnidade == numeroUnidade select obj;
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.IntegracaoCarga BuscarPorCargaUniadadeStatus(string numeroCarga, string numeroUnidade, Dominio.Enumeradores.StatusIntegracaoCarga status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCarga>();
            var result = from obj in query where obj.NumeroDaCarga == numeroCarga && obj.NumeroDaUnidade == numeroUnidade && obj.Status == status select obj;
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.IntegracaoCarga BuscarPorCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCarga>();
            var result = from obj in query where obj.CodigoCarga == codigoCarga select obj;
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.IntegracaoCarga> BuscarPorStatus(Dominio.Enumeradores.StatusIntegracaoCarga status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCarga>();
            var result = from obj in query where obj.Status == status select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.IntegracaoCarga> BuscarPendentesIntegracao(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCarga>();
            var result = from obj in query where obj.Status == Dominio.Enumeradores.StatusIntegracaoCarga.Pendente select obj;
            if (maximoRegistros > 0)
                return result.OrderBy(o => o.Codigo).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(o => o.Codigo).ToList();
        }

        public int ContarPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCarga>();
            var result = from obj in query where obj.Status == Dominio.Enumeradores.StatusIntegracaoCarga.Pendente select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.IntegracaoCarga> BuscarPendentesCancelamento(int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCarga>();
            var result = from obj in query where obj.Status == Dominio.Enumeradores.StatusIntegracaoCarga.PendenteCancelamento select obj;
            if (maximoRegistros > 0)
                return result.OrderBy(o => o.Codigo).Take(maximoRegistros).ToList();
            else
                return result.OrderBy(o => o.Codigo).ToList();
        }

        public int ContarPendentesCancelamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCarga>();
            var result = from obj in query where obj.Status == Dominio.Enumeradores.StatusIntegracaoCarga.PendenteCancelamento select obj;
            return result.Count();
        }
    }
}
