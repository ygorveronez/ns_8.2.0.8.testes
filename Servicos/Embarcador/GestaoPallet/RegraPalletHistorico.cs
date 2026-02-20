using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPallet
{
    public sealed class RegraPalletHistorico
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Repositorio.Embarcador.GestaoPallet.RegraPalletHistorico _repositorioRegraPalletHistorico;

        #endregion Atributos Privados Somente Leitura

        #region Construtores

        public RegraPalletHistorico(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repositorioRegraPalletHistorico = new Repositorio.Embarcador.GestaoPallet.RegraPalletHistorico(_unitOfWork);
        }

        #endregion Construtores

        #region Métodos Públicos

        public void SalvarNovaRegraPeriodo(Dominio.Entidades.Cliente cliente)
        {
            Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico ultimoRegraPalletHistorico = _repositorioRegraPalletHistorico.BuscarUltimaRegra(cliente.CPF_CNPJ);

            if (ultimoRegraPalletHistorico != null)
            {
                if (ultimoRegraPalletHistorico.RegraPallet == cliente.RegraPallet)
                    return;

                ultimoRegraPalletHistorico.DataFinal = DateTime.Now;
                _repositorioRegraPalletHistorico.Atualizar(ultimoRegraPalletHistorico);
            }

            if (!cliente.RegraPallet.IsRegraClienteValida())
                return;

            Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico novoRegraPalletHistorico = new Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico
            {
                Cliente = cliente,
                RegraPallet = cliente.RegraPallet,
                DataInicial = DateTime.Now
            };

            _repositorioRegraPalletHistorico.Inserir(novoRegraPalletHistorico);
        }

        public RegraPallet ObterRegraPeriodo(Dominio.Entidades.Cliente cliente, DateTime dataReferencia)
        {
            Dominio.Entidades.Embarcador.GestaoPallet.RegraPalletHistorico regraPalletHistorico = _repositorioRegraPalletHistorico.BuscarRegraNoPeriodo(cliente.CPF_CNPJ, dataReferencia);

            if (regraPalletHistorico != null)
                return regraPalletHistorico.RegraPallet;

            return cliente.RegraPallet;
        }

        #endregion Métodos Públicos
    }
}
