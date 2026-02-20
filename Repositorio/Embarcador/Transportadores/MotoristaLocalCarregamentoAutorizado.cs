using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Transportadores
{
    public class MotoristaLocalCarregamentoAutorizado : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado>
    {
        public MotoristaLocalCarregamentoAutorizado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado> BuscarPorMotorista(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado>();

            var result = from obj in query where obj.Motorista.Codigo == codigoMotorista select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado BuscarPorMotoristaECliente(int codigoMotorista, double codigoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLocalCarregamentoAutorizado>();

            var result = from obj in query where obj.Motorista.Codigo == codigoMotorista && obj.Cliente.CPF_CNPJ == codigoCliente select obj;

            return result.FirstOrDefault();
        }
    }
}
