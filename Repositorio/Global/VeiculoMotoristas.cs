using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class VeiculoMotoristas : RepositorioBase<Dominio.Entidades.VeiculoMotoristas>, Dominio.Interfaces.Repositorios.VeiculoMotoristas
    {
        public VeiculoMotoristas(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.VeiculoMotoristas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMotoristas>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.VeiculoMotoristas> BuscarPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoMotoristas>();

            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo select obj;

            return result.ToList();
        }
        
    }
}
