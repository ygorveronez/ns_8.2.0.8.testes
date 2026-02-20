using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoCargaPedagio : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio>
    {
        public AcertoCargaPedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio> BuscarPorAcertoCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio>();
            var result = from obj in query where obj.AcertoCarga.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio> BuscarPorAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio>();
            var result = from obj in query where obj.AcertoCarga.AcertoViagem.Codigo == codigo select obj;
            return result.ToList();
        }

        public decimal TotalValorPedagio(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio>();
            var result = from obj in query where obj.AcertoCarga.AcertoViagem.Codigo == codigo select obj;
            return result.Sum(o => (decimal?)o.Valor) ?? 0m;

            //if (result.Count() > 0)
            //    return result.Select(obj => obj.Valor).Sum();
            //else
            //    return 0;
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio> ConsultarAcertoCargaPedagio(int codigoCarga, int codigoAcerto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio>();
            var resultCarga = from obj in queryCarga where obj.AcertoCarga.Carga.Codigo == codigoCarga && obj.AcertoCarga.AcertoViagem.Codigo == codigoAcerto select obj;

            return resultCarga.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarAcertoCargaPedagio(int codigoCarga, int codigoAcerto)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio>();
            var resultCarga = from obj in queryCarga where obj.AcertoCarga.Carga.Codigo == codigoCarga && obj.AcertoCarga.AcertoViagem.Codigo == codigoAcerto select obj;
            return resultCarga.Count();
        }

        public static implicit operator AcertoCargaPedagio(AcertoCargaBonificacao v)
        {
            throw new NotImplementedException();
        }
    }
}
