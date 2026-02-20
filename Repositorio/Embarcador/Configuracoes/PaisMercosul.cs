using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class PaisMercosul : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.PaisMercosul>
    {
        public PaisMercosul(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Configuracoes.PaisMercosul> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.PaisMercosul>();

            var result = from obj in query select obj;

            return result.ToList();
        }

        public void InserirLista(List<Dominio.Entidades.Embarcador.Configuracoes.PaisMercosul> listaPaises)
        {
            if (listaPaises != null && listaPaises.Any())
            {
                LimparLista();
                foreach (var pais in listaPaises)
                {
                    this.SessionNHiBernate.Save(pais);
                }
            }
        }
        public void LimparLista()
        {
            var hql = "DELETE FROM Dominio.Entidades.Embarcador.Configuracoes.PaisMercosul";
            this.SessionNHiBernate.CreateQuery(hql).ExecuteUpdate();
        }

    }
}
