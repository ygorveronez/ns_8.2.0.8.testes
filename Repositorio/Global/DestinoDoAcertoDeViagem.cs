using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class DestinoDoAcertoDeViagem : RepositorioBase<Dominio.Entidades.DestinoDoAcertoDeViagem>, Dominio.Interfaces.Repositorios.DestinoDoAcertoDeViagem
    {
        public DestinoDoAcertoDeViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.DestinoDoAcertoDeViagem BuscarPorCodigoEAcertoDeViagem(int codigo, int codigoAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DestinoDoAcertoDeViagem>();
            var result = from obj in query where obj.Codigo == codigo && obj.AcertoDeViagem.Codigo == codigoAcertoViagem select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.DestinoDoAcertoDeViagem BuscarPorCTe(int empresa, int cte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DestinoDoAcertoDeViagem>();
            var result = from obj in query where obj.CTe.Codigo == cte && obj.AcertoDeViagem.Empresa.Codigo == empresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DestinoDoAcertoDeViagem> BuscarPorAcertoDeViagem(int codigoAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DestinoDoAcertoDeViagem>();
            var result = from obj in query where obj.AcertoDeViagem.Codigo == codigoAcertoViagem select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.DestinoDoAcertoDeViagem> BuscarPorAcertos(List<int> codigosAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DestinoDoAcertoDeViagem>();
            var result = from obj in query where codigosAcertoViagem.Contains(obj.AcertoDeViagem.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.DestinoDoAcertoDeViagem> BuscarPorListaDeAcertosDeViagens(List<int> listaCodigosAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DestinoDoAcertoDeViagem>();
            var result = from obj in query where listaCodigosAcertoViagem.Contains(obj.AcertoDeViagem.Codigo) select obj;
            return result.ToList();
        }
    }
}
