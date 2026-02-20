using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class PendenciaMotoristaAnexo : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo>
    {
        public PendenciaMotoristaAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo> BuscarPorPendenciaMotorista(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo>();
            var result = from obj in query where obj.PendenciaMotorista.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo> Consultar(int codigoChamado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo>();
            var result = from obj in query where obj.PendenciaMotorista.Codigo == codigoChamado select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo>();
            var result = from obj in query where obj.PendenciaMotorista.Codigo == codigo select obj;

            return result.Count();
        }

        public bool PossuiAnexo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PendenciaMotoristaAnexo>();
            var result = query.Where(obj => obj.PendenciaMotorista.Codigo == codigo);
            return result.Any();
        }

        #endregion
    }
}
