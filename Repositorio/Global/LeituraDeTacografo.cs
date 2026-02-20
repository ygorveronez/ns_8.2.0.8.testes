using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class LeituraDeTacografo : RepositorioBase<Dominio.Entidades.LeituraDeTacografo>, Dominio.Interfaces.Repositorios.LeituraDeTacografo
    {
        public LeituraDeTacografo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.LeituraDeTacografo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LeituraDeTacografo>();
            var result = from obj in query where obj.Codigo == codigo && obj.Veiculo.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.LeituraDeTacografo> Consultar(int codigoEmpresa, string nomeMotorista, string placaVeiculo, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LeituraDeTacografo>();
            var result = from obj in query where obj.Veiculo.Empresa.Codigo == codigoEmpresa select obj;
            if (!string.IsNullOrWhiteSpace(nomeMotorista))
                result = result.Where(o => o.Motorista.Nome.Contains(nomeMotorista));
            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Veiculo.Placa.Contains(placaVeiculo));
            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));
            return result.OrderByDescending(o => o.DataDeCadastro).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string nomeMotorista, string placaVeiculo, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.LeituraDeTacografo>();
            var result = from obj in query where obj.Veiculo.Empresa.Codigo == codigoEmpresa select obj;
            if (!string.IsNullOrWhiteSpace(nomeMotorista))
                result = result.Where(o => o.Motorista.Nome.Contains(nomeMotorista));
            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Veiculo.Placa.Contains(placaVeiculo));
            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));
            return result.Count();
        }
    }
}
