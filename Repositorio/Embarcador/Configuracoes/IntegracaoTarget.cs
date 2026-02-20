using System;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoTarget : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget>
    {
        public IntegracaoTarget(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget Buscar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget>();

            var result = from obj in query select obj;

            //Filtrar apenas as configurações sem filial
            var querySemFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            result = result.Where(o => !(from obj in querySemFilial where obj.IntegracaoTarget != null select obj.IntegracaoTarget.Codigo).Contains(o.Codigo));

            return result.FirstOrDefault();
        }
    }
}
