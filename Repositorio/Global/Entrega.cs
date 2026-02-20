using System;
using System.Linq;

namespace Repositorio
{
    public class Entrega : RepositorioBase<Dominio.Entidades.Entrega>, Dominio.Interfaces.Repositorios.Entrega
    {
        public Entrega(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Entrega BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Entrega>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
        
        public int ObterUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Entrega>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            int? retorno = result.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value : 0;
        }

        public dynamic Consultar(int codigoEmpresa, DateTime dataEntrega, string placaVeiculo, string nomeMotorista, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Entrega>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataEntrega != DateTime.MinValue)
                result = result.Where(o => o.Data == dataEntrega.Date);

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Veiculo.Placa.Contains(placaVeiculo));

            if (!string.IsNullOrWhiteSpace(nomeMotorista))
                result = result.Where(o => o.Motorista.Nome.Contains(nomeMotorista));

            return result.OrderByDescending(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).Select(o => new
                   {
                       o.Codigo,
                       o.Numero,
                       Data = o.Data.ToString("dd/MM/yyyy"),
                       Veiculo = o.Veiculo.Placa,
                       Motorista = o.Motorista.Nome
                   }).ToList();
        }

        public int ContarConsuta(int codigoEmpresa, DateTime dataEntrega, string placaVeiculo, string nomeMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Entrega>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (dataEntrega != DateTime.MinValue)
                result = result.Where(o => o.Data == dataEntrega.Date);

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Veiculo.Placa.Contains(placaVeiculo));

            if (!string.IsNullOrWhiteSpace(nomeMotorista))
                result = result.Where(o => o.Motorista.Nome.Contains(nomeMotorista));

            return result.Count();
        }
    }
}
