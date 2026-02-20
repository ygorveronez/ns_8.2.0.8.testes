using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escrituracao
{
    public class RegraEscrituracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao>
    {

        public RegraEscrituracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo == true);

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => o.Ativo == false);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, status);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao> BuscarRegrasAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao>();
            var result = from obj in query
                         where
                             obj.Ativo == true
                         orderby obj.Descricao ascending
                         select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao> BuscarRegrasValidas(double rementente, double destinatario, bool origemFilial, bool destinoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao>();
            var result = from obj in query
                         where 
                             obj.Ativo == true
                             && (obj.Remetente.CPF_CNPJ == rementente || obj.Remetente == null)
                             && (obj.Destinatario.CPF_CNPJ == destinatario || obj.Destinatario == null)
                             && obj.OrigemFilial == origemFilial
                             && obj.DestinoFilial == destinoFilial

                         orderby obj.Destinatario, obj.Remetente descending 
                         select obj;
            return result.ToList();
        }
    }
}
