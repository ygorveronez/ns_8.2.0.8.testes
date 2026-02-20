using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class DespesaDoAcertoDeViagem : RepositorioBase<Dominio.Entidades.DespesaDoAcertoDeViagem>, Dominio.Interfaces.Repositorios.DespesaDoAcertoDeViagem
    {
        public DespesaDoAcertoDeViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.DespesaDoAcertoDeViagem BuscarPorCodigoEAcertoDeViagem(int codigo, int codigoAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DespesaDoAcertoDeViagem>();
            var result = from obj in query where obj.Codigo == codigo && obj.AcertoDeViagem.Codigo == codigoAcertoViagem select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DespesaDoAcertoDeViagem> BuscarPorAcertoDeViagem(int codigoAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DespesaDoAcertoDeViagem>();
            var result = from obj in query where obj.AcertoDeViagem.Codigo == codigoAcertoViagem select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.DespesaDoAcertoDeViagem> BuscarPorAcertos(List<int> codigosAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DespesaDoAcertoDeViagem>();
            var result = from obj in query where codigosAcertoViagem.Contains(obj.AcertoDeViagem.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.DespesaDoAcertoDeViagem> BuscarPorListaDeAcertosDeViagens(List<int> listaCodigosAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DespesaDoAcertoDeViagem>();
            var result = from obj in query where listaCodigosAcertoViagem.Contains(obj.AcertoDeViagem.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.DespesaDoAcertoDeViagem BuscarPorCodigo(int codigoDespesa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DespesaDoAcertoDeViagem>();

            var result = from obj in query where obj.Codigo == codigoDespesa select obj;

            return result.FirstOrDefault();
        }
    }
}
