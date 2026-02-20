using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class LeilaoTipoOperacaoConfiguracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao>
    {
        public LeilaoTipoOperacaoConfiguracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao BuscarPorTipoOperacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao tipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao>();
            var resut = from obj in query where obj.TipoOperacaoEmissao == tipoOperacao select obj;
            return resut.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao tipoOperacaoEmissao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao>();
            var result = from obj in query select obj;

            if (tipoOperacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.todos)
                result = result.Where(obj => obj.TipoOperacaoEmissao == tipoOperacaoEmissao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao tipoOperacaoEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LeilaoTipoOperacaoConfiguracao>();
            var result = from obj in query select obj;
            
            if (tipoOperacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.todos)
                result = result.Where(obj => obj.TipoOperacaoEmissao == tipoOperacaoEmissao);

            return result.Count();
        }

    }
}
