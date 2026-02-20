using System;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSemParar : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar>
    {
        public IntegracaoSemParar(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar Buscar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar>();

            var result = from obj in query select obj;
            
            //Filtrar apenas as configurações sem filial
            var querySemFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            result = result.Where(o => !(from obj in querySemFilial where obj.IntegracaoSemParar != null  select obj.IntegracaoSemParar.Codigo).Contains(o.Codigo));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar BuscarPrimeira()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }
    }
}
