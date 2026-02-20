using System.Collections.Generic;

namespace Servicos.Embarcador.ConfiguracaoContabil
{
    public sealed class RegraConfiguracaoCentroResultado
    {
        private static RegraConfiguracaoCentroResultado _Instancia;

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado> ConfiguracaoCentroResultado { get; private set; }

        private RegraConfiguracaoCentroResultado()
        {

        }


        public static RegraConfiguracaoCentroResultado GetInstance(Repositorio.UnitOfWork unitOfWork)
        {
            if (_Instancia == null)
            {
                _Instancia = new RegraConfiguracaoCentroResultado();
                _Instancia.CarregarTodasConfiguracoes(unitOfWork);
            }
            return _Instancia;
        }

        public void AtualizarConfiguracaoCentroResultado(Repositorio.UnitOfWork unitOfWork)
        {
            GetInstance(unitOfWork).CarregarTodasConfiguracoes(unitOfWork);
        }

        private void CarregarTodasConfiguracoes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado repConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork);
      
            ConfiguracaoCentroResultado = repConfiguracaoCentroResultado.BuscarTodosAtivas();
        }
    }
}
