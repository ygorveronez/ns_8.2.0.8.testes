using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class CentroDescarregamentoLimiteDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento>
    {
        #region Construtores

        public CentroDescarregamentoLimiteDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion
        public Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento BuscarPorCodigo(int codigo)
        {
            var consultaLimiteDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaLimiteDescarregamento.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento> BuscarPorDiaMes(int codigoCentro, int dia, int mes)
        {
            var consultaLimiteDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamentoLimiteDescarregamento>()
                .Where(o => o.CentroDescarregamento.Codigo == codigoCentro && o.DiaDoMes == dia && o.Mes == mes);

            return consultaLimiteDescarregamento.ToList();
        }
    }
}
