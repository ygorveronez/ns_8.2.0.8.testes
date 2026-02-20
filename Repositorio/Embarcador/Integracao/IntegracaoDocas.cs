using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Integracao
{

    public class IntegracaoDocas : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas>
    {
        public IntegracaoDocas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas BuscarPorCodigo(int codigo)
        {
            var integracaoDocas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();
            return integracaoDocas;
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var integracaoDocas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao)
                .FirstOrDefault();
            return integracaoDocas;
        }


        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas> ConsultarTodos()
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas>().ToList();
        }

        public bool PossuiIntegracaoDocas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();
            var result = from obj in query where obj.Ativo && obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.VTEX select obj;
            return result.Any();
        }

        

    }

}
