using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroCarregamentoDoca : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca>
    {
        public CentroCarregamentoDoca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca BuscarPorCodigoIntegracaoDuplicado(int codigo, string códigoIntegracao)
        {
            var centroCarregamentoDoca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca>()
                .Where(o => (o.Codigo != codigo) && (o.CodigoIntegracao == códigoIntegracao))
                .FirstOrDefault();

            return centroCarregamentoDoca;
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca BuscarPorFilialNumero(int codigoFilial, int numero)
        {
            var centroCarregamentoDoca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca>()
                .Where(o => o.CentroCarregamento.Filial.Codigo == codigoFilial && o.Numero == numero)
                .FirstOrDefault();

            return centroCarregamentoDoca;
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca BuscarPorFilialDescricao(int codigoFilial, string descricao)
        {
            var centroCarregamentoDoca = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca>()
                .Where(o => o.CentroCarregamento.Filial.Codigo == codigoFilial && o.Descricao.Equals(descricao))
                .FirstOrDefault();

            return centroCarregamentoDoca;
        }
    }
}
