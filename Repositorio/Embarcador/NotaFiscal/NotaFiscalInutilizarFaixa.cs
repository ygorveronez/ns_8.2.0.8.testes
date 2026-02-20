using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalInutilizarFaixa : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa>
    {
        public NotaFiscalInutilizarFaixa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa> Consultar(string justificativa, int numeroInicial, int numeroFinal, int empresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(justificativa))
                result = result.Where(obj => obj.Justificativa.Contains(justificativa));

            if (numeroInicial > 0)
                result = result.Where(obj => obj.NumeroInicial.Equals(numeroInicial));

            if (numeroFinal > 0)
                result = result.Where(obj => obj.NumeroFinal.Equals(numeroFinal));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string justificativa, int numeroInicial, int numeroFinal, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizarFaixa>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(justificativa))
                result = result.Where(obj => obj.Justificativa.Contains(justificativa));

            if (numeroInicial > 0)
                result = result.Where(obj => obj.NumeroInicial.Equals(numeroInicial));

            if (numeroFinal > 0)
                result = result.Where(obj => obj.NumeroFinal.Equals(numeroFinal));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            return result.Count();
        }
    }
}
