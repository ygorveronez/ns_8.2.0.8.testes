using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota.Programacao
{
    public class ProgramacaoMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista>
    {

        public ProgramacaoMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista> _Consultar(int codigoEmpresa, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoMotorista> Consultar(int codigoEmpresa, string descricao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, descricao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao)
        {
            var result = _Consultar(codigoEmpresa, descricao);

            return result.Count();
        }
    }
}
