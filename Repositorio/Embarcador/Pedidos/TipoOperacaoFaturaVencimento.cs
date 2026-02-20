using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
    {
        public class TipoOperacaoFaturaVencimento : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento>
        {
            public TipoOperacaoFaturaVencimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

            public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento> BuscarPorTipoOperacao(int codigo)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoFaturaVencimento>();

                var result = from obj in query where obj.TipoOperacao.Codigo == codigo select obj;

                return result.ToList();
            }
        }
    }

