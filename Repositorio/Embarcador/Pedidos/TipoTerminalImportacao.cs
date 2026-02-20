using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoTerminalImportacao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>
    {
        public TipoTerminalImportacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> ConsultarPentendeIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarPentendeIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao BuscarTodosPorCodigoDocumento(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            var result = from obj in query where obj.CodigoDocumento == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao BuscarTodosPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            var result = from obj in query where obj.CodigoIntegracao == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao BuscarPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            var result = from obj in query where obj.CodigoIntegracao == codigo && obj.Ativo == true select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao BuscarPorCodigoTerminal(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            var result = from obj in query
                         where obj.CodigoTerminal == codigo && obj.Ativo == true
                         select obj;
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            var result = from obj in query where obj.Descricao.ToLower() == descricao.ToLower() && obj.Ativo == true select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> BuscarTodosCadastros(int codigoTerminal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();

            if (codigoTerminal > 0)
                query = query.Where(obj => obj.Codigo == codigoTerminal);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao BuscarPorPorto(int codigoPorto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            var result = from obj in query where obj.Porto.Codigo == codigoPorto select obj;
            return result.FirstOrDefault();
        }

      

        public Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao BuscarPorCodigoDocumentacao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();
            var result = from obj in query where obj.CodigoDocumento == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> Consultar(string porto, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(porto))
                result = result.Where(obj => obj.Porto.Descricao.Contains(porto));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(string porto, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(porto))
                result = result.Where(obj => obj.Porto.Descricao.Contains(porto));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> BuscarPorCodigos(List<int> codigosTerminal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>();

            query = query.Where(obj => codigosTerminal.Contains(obj.Codigo));

            return query.ToList();
        }
    }
}
