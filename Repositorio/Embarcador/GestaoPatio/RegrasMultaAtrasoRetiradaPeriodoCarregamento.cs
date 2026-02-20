using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class RegrasMultaAtrasoRetiradaPeriodoCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento>
    {
        #region Construtores

        public RegrasMultaAtrasoRetiradaPeriodoCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento> BuscarPorRegrasMultaAtrasoRetirada(int codigoRegrasMultaAtrasoRetirada)
        {
            var consultarRegrasMultaAtrasoRetiradaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento>()
                .Where(o => o.RegrasMultaAtrasoRetirada.Codigo == codigoRegrasMultaAtrasoRetirada);

            return consultarRegrasMultaAtrasoRetiradaPeriodoCarregamento.OrderBy(o => o.HoraInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento> BuscarPorRegrasMultaAtrasoRetiradaEDia(int codigoRegrasMultaAtrasoRetirada, DiaSemana dia)
        {
            var consultarRegrasMultaAtrasoRetiradaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento>()
                .Where(o =>
                    o.RegrasMultaAtrasoRetirada.Codigo == codigoRegrasMultaAtrasoRetirada &&
                    o.Dia == dia
                );

            return consultarRegrasMultaAtrasoRetiradaPeriodoCarregamento.OrderBy(o => o.HoraInicio).ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento BuscarPorCodigo(int codigo)
        {
            var consultarRegrasMultaAtrasoRetiradaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetiradaPeriodoCarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultarRegrasMultaAtrasoRetiradaPeriodoCarregamento.FirstOrDefault();
        }

        #endregion
    }
}
