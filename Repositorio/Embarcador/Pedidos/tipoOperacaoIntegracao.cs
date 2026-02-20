using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracao>
    {
        #region Construtores

        public TipoOperacaoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<int> BuscarCodigosTiposOperacaoPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var consultaTipoOperacaoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracao>()
                .Where(integracao => integracao.Tipo == tipoIntegracao);

            return consultaTipoOperacaoIntegracao
                .Select(Integracao => Integracao.TipoOperacao.Codigo)
                .Distinct()
                .ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposIntegracaoPorFechamento(int codigoFechamento)
        {
            var consultaCargaFechamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga>()
                .Where(cargaFechamento => cargaFechamento.Fechamento.Codigo == codigoFechamento);

            var consultaTipoOperacaoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracao>()
                .Where(integracao => consultaCargaFechamento.Any(cargaFechamento => cargaFechamento.Carga.TipoOperacao.Codigo == integracao.TipoOperacao.Codigo));

            return consultaTipoOperacaoIntegracao
                .Select(Integracao => Integracao.Tipo)
                .Distinct()
                .ToList();
        }

        public bool ExisteTipoOperacaoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, int codigoTipoOperacao)
        {
            var consultaTipoOperacaoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoIntegracao>()
                .Where(integracao => integracao.Tipo == tipoIntegracao && integracao.TipoOperacao.Codigo == codigoTipoOperacao);

            return consultaTipoOperacaoIntegracao.Any();
        }

        #endregion Métodos Públicos
    }
}
