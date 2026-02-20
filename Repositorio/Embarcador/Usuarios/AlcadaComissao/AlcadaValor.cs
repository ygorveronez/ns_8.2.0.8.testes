using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios.AlcadaComissao
{
    public class AlcadaValor : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor>
    {
        public AlcadaValor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AlcadaValor>();
            var result = from obj in query where obj.RegraFuncionarioComissao.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
