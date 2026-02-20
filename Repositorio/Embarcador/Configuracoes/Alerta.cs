using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class Alerta : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.Alerta>
    {
        public Alerta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.Alerta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Alerta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.Alerta> BuscarAlertasLicencas(int codigoEmpresa, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.Alerta>();
            var result = from obj in query where !obj.Ocultar && obj.Funcionario.Codigo == codigoUsuario select obj;
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);
            return result.ToList();
        }
    }
}
