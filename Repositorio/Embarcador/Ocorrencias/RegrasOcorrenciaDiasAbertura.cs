using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class RegrasOcorrenciaDiasAbertura : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura>
    {
        #region Construtores

        public RegrasOcorrenciaDiasAbertura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura> BuscarPorRegras(int codigo)
        {
            var consultaRegrasDiasAbertura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura>()
                .Where(o => o.RegrasAutorizacaoOcorrencia.Codigo == codigo);

            return consultaRegrasDiasAbertura.OrderBy("Ordem ascending").ToList();
        }

        #endregion
    }
}
