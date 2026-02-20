using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Configuracoes
{
    public sealed class ConfiguracaoLegenda
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ConfiguracaoLegenda(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private dynamic Obter(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda configuracaoLegenda)
        {
            return new
            {
                NomePropriedade = configuracaoLegenda.CodigoControle.ObterNomePropriedade(),
                configuracaoLegenda.Descricao,
                configuracaoLegenda.Exibir
            };
        }

        #endregion

        #region Métodos Públicos

        public dynamic ObterPorCodigoControle(CodigoControleLegenda codigoControle)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoLegenda repositorioConfiguracaoLegenda = new Repositorio.Embarcador.Configuracoes.ConfiguracaoLegenda(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda configuracaoLegenda = repositorioConfiguracaoLegenda.BuscarPorCodigoControle(codigoControle);

            if (configuracaoLegenda == null)
                return null;

            return Obter(configuracaoLegenda);
        }

        public dynamic ObterPorCodigosControle(List<CodigoControleLegenda> codigosControle)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoLegenda repositorioConfiguracaoLegenda = new Repositorio.Embarcador.Configuracoes.ConfiguracaoLegenda(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda> configuracoesLegenda = repositorioConfiguracaoLegenda.BuscarPorCodigosControle(codigosControle);

            return (
                from o in configuracoesLegenda
                select Obter(o)
            ).ToList();
        }

        public dynamic ObterPorGrupoCodigoControle(GrupoCodigoControleLegenda grupoCodigoControle)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoLegenda repositorioConfiguracaoLegenda = new Repositorio.Embarcador.Configuracoes.ConfiguracaoLegenda(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLegenda> configuracoesLegenda = repositorioConfiguracaoLegenda.BuscarPorGrupoCodigoControle(grupoCodigoControle);

            return (
                from o in configuracoesLegenda
                select Obter(o)
            ).ToList();
        }

        #endregion
    }
}
