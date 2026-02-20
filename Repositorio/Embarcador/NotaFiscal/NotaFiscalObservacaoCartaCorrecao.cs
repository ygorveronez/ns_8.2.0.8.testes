using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscalObservacaoCartaCorrecao : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao>
    {
        public NotaFiscalObservacaoCartaCorrecao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao> Consultar(string especificacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(especificacao))
                result = result.Where(obj => obj.Especificacao.Contains(especificacao));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string especificacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoCartaCorrecao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(especificacao))
                result = result.Where(obj => obj.Especificacao.Contains(especificacao));

            return result.Count();
        }
    }
}
