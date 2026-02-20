using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class BoletoRetornoComando : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando>
    {
        public BoletoRetornoComando(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando> Consultar(string comando, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(comando))
                result = result.Where(obj => obj.Comando.Equals(comando));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string comando, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(comando))
                result = result.Where(obj => obj.Comando.Equals(comando));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result.Count();
        }
    }
}