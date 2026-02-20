using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class MinutaDevolucaoContainer : RepositorioBase<Dominio.Entidades.MinutaDevolucaoContainer>, Dominio.Interfaces.Repositorios.MinutaDevolucaoContainer
    {
        public MinutaDevolucaoContainer(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.MinutaDevolucaoContainer BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MinutaDevolucaoContainer>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MinutaDevolucaoContainer BuscarPorNumero(int codigoEmpresa, int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MinutaDevolucaoContainer>();

            var result = from obj in query where obj.Numero == numero select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public int ObterUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MinutaDevolucaoContainer>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            int? retorno = result.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value : 0;
        }

        public List<Dominio.Entidades.MinutaDevolucaoContainer> Consultar(int codigoEmpresa, int numero, string container, string importador, string nomeTerminal, string armador, string navio, string nomeMotorista, string placaTracao, string placaReboque, int codigoCTe, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MinutaDevolucaoContainer>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(container))
                result = result.Where(o => o.Container == container);

            if (!string.IsNullOrWhiteSpace(importador))
                result = result.Where(o => o.Importador == importador);

            if (!string.IsNullOrWhiteSpace(armador))
                result = result.Where(o => o.Armador == armador);

            if (!string.IsNullOrWhiteSpace(navio))
                result = result.Where(o => o.Navio == navio);

            if (!string.IsNullOrWhiteSpace(nomeTerminal))
                result = result.Where(o => o.Terminal.Nome.Contains(nomeTerminal));

            if (!string.IsNullOrWhiteSpace(nomeMotorista))
                result = result.Where(o => o.Motorista.Nome.Contains(nomeMotorista));

            if (!string.IsNullOrWhiteSpace(placaTracao))
                result = result.Where(o => o.Veiculo.Placa.Equals(placaTracao));

            if (!string.IsNullOrWhiteSpace(placaReboque))
                result = result.Where(o => o.Reboque.Placa.Equals(placaReboque));

            if (codigoCTe > 0)
                result = result.Where(o => o.CTE.Codigo == codigoCTe);

            return result.OrderByDescending(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int numero, string container, string importador, string nomeTerminal, string armador, string navio, string nomeMotorista, string placaTracao, string placaReboque, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MinutaDevolucaoContainer>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (!string.IsNullOrWhiteSpace(container))
                result = result.Where(o => o.Container == container);

            if (!string.IsNullOrWhiteSpace(importador))
                result = result.Where(o => o.Importador == importador);

            if (!string.IsNullOrWhiteSpace(armador))
                result = result.Where(o => o.Armador == armador);

            if (!string.IsNullOrWhiteSpace(navio))
                result = result.Where(o => o.Navio == navio);

            if (!string.IsNullOrWhiteSpace(nomeTerminal))
                result = result.Where(o => o.Terminal.Nome.Contains(nomeTerminal));

            if (!string.IsNullOrWhiteSpace(nomeMotorista))
                result = result.Where(o => o.Motorista.Nome.Contains(nomeMotorista));

            if (!string.IsNullOrWhiteSpace(placaTracao))
                result = result.Where(o => o.Veiculo.Placa.Equals(placaTracao));

            if (!string.IsNullOrWhiteSpace(placaReboque))
                result = result.Where(o => o.Reboque.Placa.Equals(placaReboque));

            if (codigoCTe > 0)
                result = result.Where(o => o.CTE.Codigo == codigoCTe);

            return result.Count();
        }


        public List<Dominio.ObjetosDeValor.Relatorios.MinutaDevolucaoContainer> BuscarEspelho(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MinutaDevolucaoContainer>();

            var result = from minutaDevolucaoContainer in query
                         where minutaDevolucaoContainer.Codigo == codigo 
                         select new Dominio.ObjetosDeValor.Relatorios.MinutaDevolucaoContainer()
                         {
                             Numero = minutaDevolucaoContainer.Numero,
                             Container = minutaDevolucaoContainer.Container,
                             Importador = minutaDevolucaoContainer.Importador,
                             NomeTerminal = string.Empty,
                             CNPJTerminal = string.Empty,
                             EnderecoTerminal = string.Empty,
                             ContatoTransportador = minutaDevolucaoContainer.Empresa.Contato + " " + minutaDevolucaoContainer.Empresa.TelefoneContato,
                             Armador = minutaDevolucaoContainer.Armador,
                             TipoEquipamento = minutaDevolucaoContainer.TipoEquipamento,
                             Quantidade = minutaDevolucaoContainer.Quantidade,
                             Peso = minutaDevolucaoContainer.Peso,
                             Navio = minutaDevolucaoContainer.Navio,
                             CPFMotorista =  string.Empty,
                             NomeMotorista =  string.Empty,
                             CNHMotorista = string.Empty,
                             PlacaTracao = string.Empty,
                             PlacaReboque = string.Empty,
                             CTe = string.Empty,
                             Observacap = minutaDevolucaoContainer.Observacao
                         };

            return result.ToList();
        }

    }
}
