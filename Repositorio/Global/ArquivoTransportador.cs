using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class ArquivoTransportador : RepositorioBase<Dominio.Entidades.ArquivoTransportador>
    {
        public ArquivoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ArquivoTransportador BuscarPorCodigo(int empresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ArquivoTransportador>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == empresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ArquivoTransportador> BuscarPorTransportador(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ArquivoTransportador>();
            var result = from obj in query where obj.Empresa.Codigo == empresa select obj;
            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.ArquivoTransportador> _Consultar(int empresa, DateTime dataInicial, DateTime dataFinal, string descricao, bool? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ArquivoTransportador>();

            var result = from obj in query where obj.Empresa.Codigo == empresa select obj;

            // Filtros
            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.Data.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.Data.Date <= dataFinal);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (status.HasValue)
                result = result.Where(o => o.Ativo == status);

            return result;
        }

        public List<Dominio.Entidades.ArquivoTransportador> Consultar(int empresa, DateTime dataInicial, DateTime dataFinal, string descricao, bool? status, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(empresa, dataInicial, dataFinal, descricao, status);

            result = result.OrderBy("Data descending");

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int empresa, DateTime dataInicial, DateTime dataFinal, string descricao, bool? status)
        {
            var result = _Consultar(empresa, dataInicial, dataFinal, descricao, status);

            return result.Count();
        }
    }
}
