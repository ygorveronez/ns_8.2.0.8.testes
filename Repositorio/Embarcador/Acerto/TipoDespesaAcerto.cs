using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class TipoDespesaAcerto: RepositorioBase<Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto>
    {
        public TipoDespesaAcerto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa tipoDespesa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto>();

            var result = from obj in query select obj;

            if (tipoDespesa != 0)
            {
                result = result.Where(obj => obj.TipoDeDespesa.Value == tipoDespesa);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));          

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }
        public List<Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto> ConsultarTodosTiposDespesas(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto>();

            var result = from obj in query select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }
        public int ContarTodosTiposDespesas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto>();

            var result = from obj in query select obj;
            return result.Count();
        }
        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa tipoDespesa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto>();

            var result = from obj in query select obj;
            if (tipoDespesa != 0)
            {
                result = result.Where(obj => obj.TipoDeDespesa.Value == tipoDespesa);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));           

            return result.Count();
        }
    }
}
