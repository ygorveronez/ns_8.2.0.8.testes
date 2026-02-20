using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoGrupoTomadorBloqueado : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado>
    {
        public TipoOperacaoGrupoTomadorBloqueado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado> BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado>();

            var result = from obj in query where obj.TipoOperacao.Codigo == codigoTipoOperacao select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado BuscarPorGrupoETipoOperacao(int codigoTipoOperacao, int codigoGrupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado>();

            var result = from obj in query where obj.TipoOperacao.Codigo == codigoTipoOperacao && obj.GrupoPessoas.Codigo == codigoGrupoPessoas select obj;

            return result.FirstOrDefault();
        }
    }
}
