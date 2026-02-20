using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class SefazHistorico : RepositorioBase<Dominio.Entidades.SefazHistorico>
    {
        public SefazHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.SefazHistorico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SefazHistorico>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.SefazHistorico> Consultar(int codigoUsuario, string sefaz, DateTime dataInicial, DateTime dataFinal, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoUsuario, sefaz, dataInicial, dataFinal);

            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoUsuario, string sefaz, DateTime dataInicial, DateTime dataFinal)
        {
            var result = _Consultar(codigoUsuario, sefaz, dataInicial, dataFinal);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.SefazHistorico> _Consultar(int codigoUsuario, string sefaz, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.SefazHistorico>();

            var result = from obj in query select obj;

            if (codigoUsuario > 0)
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);
            
            if (!string.IsNullOrWhiteSpace(sefaz))
                result = result.Where(o => o.SefazCTe.Descricao.Contains(sefaz));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.Data.Value.Date > dataInicial.Date);
            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.Data.Value.Date <= dataFinal.Date);

            return result;
        }
    }
}
