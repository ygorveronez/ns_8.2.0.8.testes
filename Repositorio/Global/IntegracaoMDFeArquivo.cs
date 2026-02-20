using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class IntegracaoMDFeArquivo: RepositorioBase<Dominio.Entidades.IntegracaoMDFeArquivo>, Dominio.Interfaces.Repositorios.IntegracaoMDFeArquivo
    {
        public IntegracaoMDFeArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.IntegracaoMDFeArquivo> Buscar(int numeroCarga, int numeroUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFeArquivo>();

            var result = from obj in query select obj;

            if (numeroCarga > 0)
                result = result.Where(o => o.NumeroDaCarga == numeroCarga);

            if (numeroUnidade > 0)
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            return result.ToList();
        }

        public List<Dominio.Entidades.IntegracaoMDFeArquivo> Buscar(int codigoEmpresa, int numeroCarga, int numeroUnidade){
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoMDFeArquivo>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if(numeroCarga > 0)
                result = result.Where (o => o.NumeroDaCarga == numeroCarga);

            if(numeroUnidade > 0)
                result = result.Where(o => o.NumeroDaUnidade == numeroUnidade);

            return result.ToList();
        }
    }
}
