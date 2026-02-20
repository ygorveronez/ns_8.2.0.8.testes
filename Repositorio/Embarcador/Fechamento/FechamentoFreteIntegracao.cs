using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fechamento
{
    public class FechamentoFreteIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteIntegracao>
    {
        #region Construtores

        public FechamentoFreteIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteIntegracao> BuscarPorFechamento(int codigoFechamento)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteIntegracao>()
                .Where(integracao => integracao.FechamentoFrete.Codigo == codigoFechamento);

            return consultaIntegracao.ToList();
        }

        public bool ExistePorFechamentoETIpo(int codigoFechamento, int codigoTipoIntegracao)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteIntegracao>()
                .Where(integracao => integracao.FechamentoFrete.Codigo == codigoFechamento && integracao.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return consultaIntegracao.Count() > 0;
        }

        #endregion Métodos Públicos
    }
}
