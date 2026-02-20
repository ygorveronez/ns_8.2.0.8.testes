using AdminMultisoftware.Dominio.Enumeradores;

namespace Servicos.Embarcador.Carga
{
    public sealed class CargaOperador
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CargaOperador(Repositorio.UnitOfWork unitOfWork) : this (unitOfWork, configuracaoEmbarcador: null) { }

        public CargaOperador(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Público

        public void Atualizar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario usuario, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe)
                return;

            if ((carga.OperadorContratouCarga != null) && ObterConfiguracaoEmbarcador().FixarOperadorContratouCarga)
            {
                carga.Operador = carga.OperadorContratouCarga;
                return;
            }

            carga.Operador = usuario;
        }

        #endregion
    }
}
