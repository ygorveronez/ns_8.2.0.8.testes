using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escrituracao
{
    public class EstornoProvisaoSolicitacaoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo>
    {
        public EstornoProvisaoSolicitacaoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo> BuscarPorSolicitacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo>();
            var result = from obj in query where obj.EntidadeAnexo.Codigo == codigo select obj;
            return result.ToList();
        }

        public bool PossuiAnexo(int codigoChamado)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacaoAnexo>();
            var result = query.Where(obj => obj.EntidadeAnexo.Codigo == codigoChamado);
            return result.Any();
        }

        #endregion
    }
}
