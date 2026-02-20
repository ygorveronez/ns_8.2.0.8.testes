using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class PendenciaContratoFreteFuturo : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo>
    {
        public PendenciaContratoFreteFuturo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo> BuscarPorTerceiroEVeiculo(double cnpjCpfTerceiro, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.PendenciaContratoFreteFuturo>();

            var result = from obj in query where obj.TransportadorTerceiro.CPF_CNPJ == cnpjCpfTerceiro && obj.Veiculo.Codigo == codigoVeiculo && obj.Ativo select obj;

            return result.ToList();
        }
    }
}
