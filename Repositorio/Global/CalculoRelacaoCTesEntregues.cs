using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class CalculoRelacaoCTesEntregues : RepositorioBase<Dominio.Entidades.CalculoRelacaoCTesEntregues>
    {

        public CalculoRelacaoCTesEntregues(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CalculoRelacaoCTesEntregues BuscarPorCodigo(int empresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CalculoRelacaoCTesEntregues>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == empresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.CalculoRelacaoCTesEntregues BuscarPorEmpresaECliente(int empresa, double cliente, double emissor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CalculoRelacaoCTesEntregues>();
            var result = from obj in query where obj.Cliente.CPF_CNPJ == cliente && obj.Empresa.Codigo == empresa select obj;

            if (emissor > 0)
                result = result.Where(o => o.Emissor.CPF_CNPJ == emissor);

            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.CalculoRelacaoCTesEntregues> _Consultar(int empresa, double cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CalculoRelacaoCTesEntregues>();

            var result = from obj in query select obj;

            // Filtros
            if (empresa > 0)
                result = result.Where(o => o.Empresa.Codigo == empresa);

            if (cliente > 0)
                result = result.Where(o => o.Cliente.CPF_CNPJ == cliente);

            return result;
        }

        public List<Dominio.Entidades.CalculoRelacaoCTesEntregues> Consultar(int empresa, double cliente, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(empresa, cliente);
            
            result = result.OrderBy("Cliente.Nome ascending");

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int empresa, double cliente)
        {
            var result = _Consultar(empresa, cliente);

            return result.Count();
        }
    }
}
