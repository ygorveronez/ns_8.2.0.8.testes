using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
namespace Repositorio.Embarcador.RH
{
    public class TabelaProdutividadeValores : RepositorioBase<Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores>
    {
        public TabelaProdutividadeValores(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores BuscarPorValor(decimal valor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores>();
            var result = from obj in query where obj.ValorInicial <= valor && obj.ValorFinal >= valor && obj.TabelaProdutividade.Ativo == true select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores> BuscarPorTabela(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores>();
            var result = from obj in query where obj.TabelaProdutividade.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<int> BuscarItensNaoPesentesNaLista(int parametro, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores>();
            var result = from obj in query
                         where
                            obj.TabelaProdutividade.Codigo == parametro
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores BuscarPorParametroHoraExtraECodigo(int parametro, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores>();
            var result = from obj in query
                         where
                            obj.TabelaProdutividade.Codigo == parametro
                            && obj.Codigo == codigo
                         select obj;

            return result.FirstOrDefault();
        }
    }
}
