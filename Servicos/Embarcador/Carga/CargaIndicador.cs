namespace Servicos.Embarcador.Carga
{
    public sealed class CargaIndicador
    {
        #region Variáveis

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CargaIndicador(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public void DefinirIndicadorTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaIndicadorTransportador indicador)
        {
            if (carga == null)
                return;
            
            Repositorio.Embarcador.Cargas.CargaIndicador repositorioCargaIndicador = new Repositorio.Embarcador.Cargas.CargaIndicador(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaIndicador cargaIndicador = repositorioCargaIndicador.BuscarPorCarga(carga.Codigo) ?? new Dominio.Entidades.Embarcador.Cargas.CargaIndicador();

            cargaIndicador.Carga = carga;
            cargaIndicador.CargaIndicadorTransportador = indicador;

            if (cargaIndicador.Codigo > 0)
                repositorioCargaIndicador.Atualizar(cargaIndicador);
            else
                repositorioCargaIndicador.Inserir(cargaIndicador);
        }
        
        public void DefinirIndicadorDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaIndicadorVeiculoMotorista indicador)
        {
            if (carga == null)
                return;
            
            Repositorio.Embarcador.Cargas.CargaIndicador repositorioCargaIndicador = new Repositorio.Embarcador.Cargas.CargaIndicador(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaIndicador cargaIndicador = repositorioCargaIndicador.BuscarPorCarga(carga.Codigo) ?? new Dominio.Entidades.Embarcador.Cargas.CargaIndicador();

            cargaIndicador.Carga = carga;
            cargaIndicador.CargaIndicadorVeiculoMotorista = indicador;

            if (cargaIndicador.Codigo > 0)
                repositorioCargaIndicador.Atualizar(cargaIndicador);
            else
                repositorioCargaIndicador.Inserir(cargaIndicador);
        }
    }
}
