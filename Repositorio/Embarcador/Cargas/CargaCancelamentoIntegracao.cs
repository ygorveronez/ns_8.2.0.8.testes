using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCancelamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao>
    {
        public CargaCancelamentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ContarPorCancelamentoETipoIntegracao(int codigoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao>();

            var resut = from obj in query where obj.CargaCancelamento.Codigo == codigoCancelamento && obj.TipoIntegracao.Tipo == tipoIntegracao select obj.Codigo;

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao> BuscarPorCargaCancelamento(int cargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao>();
            var resut = from obj in query where obj.CargaCancelamento.Codigo == cargaCancelamento select obj;
            return resut.Fetch(o => o.TipoIntegracao).ToList();
        }   
        
        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao>();
            var resut = from obj in query where obj.CargaCancelamento.Carga.Codigo == codigoCarga select obj;
            return resut.Fetch(o => o.TipoIntegracao).ToList();
        }

    }
}
