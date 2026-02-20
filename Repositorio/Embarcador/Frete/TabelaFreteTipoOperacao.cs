using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteTipoOperacao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao>
    {
         public TabelaFreteTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }


         public Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

         public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao> Consultar( Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao tipoOperacaoEmissao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao>();

            var result = from obj in query select obj;

            if (tipoOperacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.todos)
                result = result.Where(obj => obj.TipoOperacaoEmissao == tipoOperacaoEmissao);
        
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

         public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao tipoOperacaoEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao>();

            var result = from obj in query select obj;


            if (tipoOperacaoEmissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.todos)
                result = result.Where(obj => obj.TipoOperacaoEmissao == tipoOperacaoEmissao);
        
            return result.Count();
        }

         public Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao BuscarPorTipoOperacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao tipoOperacao)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteTipoOperacao>();
             var resut = from obj in query where obj.TipoOperacaoEmissao == tipoOperacao select obj;
             return resut.FirstOrDefault();
         }
    }
}
