using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class SMViagemMDFe : RepositorioBase<Dominio.Entidades.SMViagemMDFe>, Dominio.Interfaces.Repositorios.SMViagemMDFe
    {
        public SMViagemMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.SMViagemMDFe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SMViagemMDFe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.SMViagemMDFe> BuscarPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SMViagemMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.ToList();
        }

        public Dominio.Entidades.SMViagemMDFe BuscarPrimeiroPorMDFe(int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SMViagemMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.SMViagemMDFe> BuscarListaPorMDFeTipoStatus(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoSM tipo, Dominio.Enumeradores.StatusIntegracaoSM status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SMViagemMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipo && obj.Status == status select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.SMViagemMDFe> BuscarPorMDFeTipoStatus(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoSM tipo, Dominio.Enumeradores.StatusIntegracaoSM status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SMViagemMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipo && obj.Status == status select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.SMViagemMDFe> BuscarPorMDFeTipo(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoSM tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SMViagemMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.SMViagemMDFe> BuscarPorStatusETipo(Dominio.Enumeradores.StatusIntegracaoSM status, Dominio.Enumeradores.TipoIntegracaoSM tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SMViagemMDFe>();
            var result = from obj in query where obj.Status == status && obj.Tipo == tipo select obj;
            return result.ToList();
        }
    }
}
