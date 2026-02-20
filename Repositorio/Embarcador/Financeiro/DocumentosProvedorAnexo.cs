using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentosProvedorAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentosProvedorAnexo>
    {
        public DocumentosProvedorAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentosProvedorAnexo> BuscarPorPagamnetoProvedor(int codigoPagamentoProvedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentosProvedorAnexo>();

            var result = from obj in query where obj.EntidadeAnexo.Codigo == codigoPagamentoProvedor select obj;

            return result.ToList();
        }

        #endregion Métodos Públicos

    }
}