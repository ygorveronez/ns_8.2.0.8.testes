using System;

namespace Servicos.Embarcador.Cargas
{
    public class CargaSequencial
    {
        #region Atributos

        private static readonly Lazy<CargaSequencial> _cargaSequencial = new Lazy<CargaSequencial>(() => new CargaSequencial());
        private readonly object _lockSequencial = new object();

        #endregion Atributos

        #region Construtores

        private CargaSequencial() { }

        public static CargaSequencial GetInstance()
        {
            return _cargaSequencial.Value;
        }

        #endregion Construtores

        #region Métodos Públicos

        public int ObterProximoNumeroSequencial(Repositorio.UnitOfWork unitOfWork, int? codigoFilial = null, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null)
        {
            lock (_lockSequencial)
            {
                if (tipoOperacao != null && (tipoOperacao.ConfiguracaoCarga?.IncrementaCodigoPorTipoOperacao ?? false))
                    return ObterSequencialPorTipoOperacao(tipoOperacao, unitOfWork);

                int proximoNumeroSequencial = new Repositorio.Embarcador.Cargas.CargaNumeroSequencial(unitOfWork).ObterProximoCodigo(codigoFilial);

                if (proximoNumeroSequencial <= 0)
                    proximoNumeroSequencial = new Repositorio.Embarcador.Cargas.Carga(unitOfWork).ObterProximoCodigo(codigoFilial);

                AtualizarSequencial(proximoNumeroSequencial, codigoFilial, unitOfWork);

                return proximoNumeroSequencial;
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private int ObterSequencialPorTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            tipoOperacao.ConfiguracaoCarga.ProximoCodigoCarga++;
            new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCarga(unitOfWork).Atualizar(tipoOperacao.ConfiguracaoCarga);
            return tipoOperacao.ConfiguracaoCarga.ProximoCodigoCarga;
        }

        private void AtualizarSequencial(int numeroSequencial, int? codigoFilial, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaNumeroSequencial repositorioCargaNumeroSequencial = new Repositorio.Embarcador.Cargas.CargaNumeroSequencial(unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = codigoFilial.HasValue ? repositorioFilial.BuscarPorCodigo(codigoFilial.Value) : null;
            Dominio.Entidades.Embarcador.Cargas.CargaNumeroSequencial cargaNumeroSequencial = repositorioCargaNumeroSequencial.BuscarPorFilial(filial?.Codigo);

            if (cargaNumeroSequencial == null)
                cargaNumeroSequencial = new Dominio.Entidades.Embarcador.Cargas.CargaNumeroSequencial();

            cargaNumeroSequencial.NumeroSequencial = numeroSequencial;
            cargaNumeroSequencial.Filial = filial;

            if (cargaNumeroSequencial.Codigo > 0)
                repositorioCargaNumeroSequencial.Atualizar(cargaNumeroSequencial);
            else
                repositorioCargaNumeroSequencial.Inserir(cargaNumeroSequencial);

            unitOfWork.Flush();
        }

        #endregion
    }
}
