using System.Collections.Generic;

namespace Servicos.Embarcador.ISS
{
    public sealed class RegrasCalculoISS
    {
        private static RegrasCalculoISS _instancia;
        private List<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> _transportadorConfiguracaoNFSe;

        #region Construtores

        private RegrasCalculoISS() { }

        public static RegrasCalculoISS GetInstance(Repositorio.UnitOfWork unitOfWork)
        {
            if (_instancia == null)
            {
                _instancia = new RegrasCalculoISS();
                _instancia.CarregarTodasRegras(unitOfWork);
            }

            return _instancia;
        }

        private void CarregarTodasRegras(Repositorio.UnitOfWork unitOfWork)
        {
            _transportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork).BuscarTodosAtivas();

        }

        #endregion


        #region Métodos Públicos

        public void AtualizarRegrasISS(Repositorio.UnitOfWork unitOfWork)
        {
            CarregarTodasRegras(unitOfWork);
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> ObterRegrasISS()
        {
            return _transportadorConfiguracaoNFSe ?? new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe>();
        }

        #endregion
    }
}
