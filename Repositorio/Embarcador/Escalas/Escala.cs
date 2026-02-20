using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escalas
{
    public class Escala : RepositorioBase<Dominio.Entidades.Embarcador.Escalas.Escala>
    {

        public Escala(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Escalas.Escala BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.Escala>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escalas.Escala> BuscarPorCentroCarregamento(int centroCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.Escala>();
            var result = from obj in query where obj.CentroCarregamento.Codigo == centroCarregamento select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Escalas.Escala BuscarPorCentroCarregamentoEDescarregamento(int centroCarregamento, int centroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.Escala>();
            var result = from obj in query where obj.CentroCarregamento.Codigo == centroCarregamento && obj.CentroDescarregamento.Codigo == centroDescarregamento && obj.Status select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Escalas.Escala> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala? classificacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.Escala>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (classificacao.HasValue)
                result = result.Where(o => o.Classificacao == classificacao.Value);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status == true);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => o.Status == false);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Escalas.Escala> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala? classificacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, classificacao, status);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala? classificacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, classificacao, status);

            return result.Count();
        }
    }
}
