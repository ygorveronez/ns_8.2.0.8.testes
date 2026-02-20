using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NaoConformidadeAnexo : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidadeAnexo>
    {
        #region Construtores

        public NaoConformidadeAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidadeAnexo> BuscarAnexosPorNaoConformidade(int codigoNaoConformidade)
        {
            var consultaNaoConformidadeAnexo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidadeAnexo>()
                .Where(naoConformidadeAnexo => naoConformidadeAnexo.EntidadeAnexo.Codigo == codigoNaoConformidade);

            return consultaNaoConformidadeAnexo.ToList();
        }

        #endregion Métodos Públicos
    }
}
