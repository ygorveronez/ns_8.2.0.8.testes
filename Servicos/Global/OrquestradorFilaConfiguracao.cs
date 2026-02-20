using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Global
{
    public sealed class OrquestradorFilaConfiguracao
    {
        #region Atributos

        private static readonly Lazy<OrquestradorFilaConfiguracao> _orquestradorFilaConfiguracao = new Lazy<OrquestradorFilaConfiguracao>(() => new OrquestradorFilaConfiguracao());
        private List<Dominio.ObjetosDeValor.Embarcador.OrquestradorFila.ConfiguracaoOrquestradorFila> _configuracaoOrquestradorFila;

        #endregion Atributos

        #region Construtores

        private OrquestradorFilaConfiguracao() { }

        public static OrquestradorFilaConfiguracao GetInstance()
        {
            return _orquestradorFilaConfiguracao.Value;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void RecarrregarConfiguracoes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConfiguracaoOrquestradorFila repositorioConfiguracaoOrquestradorFila = new Repositorio.ConfiguracaoOrquestradorFila(unitOfWork);

            _configuracaoOrquestradorFila = repositorioConfiguracaoOrquestradorFila.Buscar();
        }

        public Dominio.ObjetosDeValor.Embarcador.OrquestradorFila.ConfiguracaoOrquestradorFila ObterConfiguracao(Repositorio.UnitOfWork unitOfWork, IdentificadorControlePosicaoThread identificador)
        {
            if (_configuracaoOrquestradorFila == null)
                RecarrregarConfiguracoes(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.OrquestradorFila.ConfiguracaoOrquestradorFila configuracaoOrquestradorFila = _configuracaoOrquestradorFila.FirstOrDefault(o => o.Identificador == identificador);

            if (configuracaoOrquestradorFila == null)
                configuracaoOrquestradorFila = new Dominio.ObjetosDeValor.Embarcador.OrquestradorFila.ConfiguracaoOrquestradorFila()
                {
                    TratarRegistrosComFalha = false,
                    Identificador = identificador,
                    QuantidadeRegistrosConsulta = identificador.ObterQuantidadeRegistrosRetornoPadrao(),
                    QuantidadeRegistrosRetorno = identificador.ObterQuantidadeRegistrosRetornoPadrao(),
                    LimiteTentativas = 0
                };

            return configuracaoOrquestradorFila;
        }

        #endregion Métodos Públicos
    }
}
