using System.Collections.Generic;

namespace Servicos.Embarcador.ICMS
{
    public sealed class RegrasCalculoImpostos
    {
        #region Atributos

        private static RegrasCalculoImpostos _instancia;
        private List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> _regrasICMS;

        #endregion

        #region Construtores

        private RegrasCalculoImpostos() { }

        public static RegrasCalculoImpostos GetInstance(Repositorio.UnitOfWork unitOfWork)
        {
            if (_instancia == null)
            {
                _instancia = new RegrasCalculoImpostos();
                _instancia.CarregarTodasRegras(unitOfWork);
            }

            return _instancia;
        }

        #endregion

        #region Métodos Privados

        private void CarregarTodasRegras(Repositorio.UnitOfWork unitOfWork)
        {
            _regrasICMS = new Repositorio.Embarcador.ICMS.RegraICMS(unitOfWork).BuscarTodosAtivas();

            foreach (Dominio.Entidades.Embarcador.ICMS.RegraICMS regra in _regrasICMS)
            {
                regra.TiposOperacao = regra.TiposOperacao;
                regra.ProdutosEmbarcador = regra.ProdutosEmbarcador;
            }
        }

        #endregion

        #region Métodos Públicos

        public void AtualizarRegrasICMS(Repositorio.UnitOfWork unitOfWork)
        {
            CarregarTodasRegras(unitOfWork);
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> ObterRegrasICMS()
        {
            return _regrasICMS ?? new List<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();
        }

        #endregion
    }
}
