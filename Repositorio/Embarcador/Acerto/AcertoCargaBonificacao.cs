using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoCargaBonificacao : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao>
    {
        public AcertoCargaBonificacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao> BuscarPorAcertoCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao>();
            var result = from obj in query where obj.AcertoCarga.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao> BuscarPorAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao>();
            var result = from obj in query where obj.AcertoCarga.AcertoViagem.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao> ConsultarAcertoCargaBonificacao(int codigoCarga, int codigoAcerto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao>();
            var resultCarga = from obj in queryCarga where obj.AcertoCarga.Carga.Codigo == codigoCarga && obj.AcertoCarga.AcertoViagem.Codigo == codigoAcerto select obj;

            return resultCarga.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarAcertoCargaBonificacao(int codigoCarga, int codigoAcerto)
        {
            var queryCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao>();
            var resultCarga = from obj in queryCarga where obj.AcertoCarga.Carga.Codigo == codigoCarga && obj.AcertoCarga.AcertoViagem.Codigo == codigoAcerto select obj;
            return resultCarga.Count();
        }

    }
}
