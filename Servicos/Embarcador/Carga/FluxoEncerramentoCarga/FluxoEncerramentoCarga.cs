namespace Servicos.Embarcador.Carga.FluxoEncerramentoCarga
{
    public class FluxoEncerramentoCarga
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public FluxoEncerramentoCarga(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion






    }
}
