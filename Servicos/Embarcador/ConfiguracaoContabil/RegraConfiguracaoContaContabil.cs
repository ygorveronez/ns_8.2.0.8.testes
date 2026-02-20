using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.ConfiguracaoContabil
{
    public sealed class RegraConfiguracaoContaContabil
    {
        private static RegraConfiguracaoContaContabil _Instancia;
        public IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> ConfiguracoesContaContabil { get; private set; }

        private RegraConfiguracaoContaContabil()
        {

        }

        public static RegraConfiguracaoContaContabil GetInstance(Repositorio.UnitOfWork unitOfWork)
        {
            if (_Instancia == null)
            {
                _Instancia = new RegraConfiguracaoContaContabil();
                _Instancia.CarregarTodasConfiguracoes(unitOfWork);
            }
            return _Instancia;
        }

        public void AtualizarConfiguracaoContaContabil(Repositorio.UnitOfWork unitOfWork)
        {
            GetInstance(unitOfWork).CarregarTodasConfiguracoes(unitOfWork);
        }

        private void CarregarTodasConfiguracoes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil repConfiguracaoContaContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao repConfiguracaoContaContabilContabilizacao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao repConfiguracaoContaContabilEscrituracao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao repConfiguracaoContaContabilProvisao = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao(unitOfWork);

            ConfiguracoesContaContabil = repConfiguracaoContaContabil.ConsultarConfiguracaoContaContabilAtiva();

            IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao> configuracaoContaContabilContabilizacao = repConfiguracaoContaContabilContabilizacao.ConsultarConfiguracaoContaContabilContabilizacao();
            IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao> configuracaoContaContabilEscrituracao = repConfiguracaoContaContabilEscrituracao.ConsultarConfiguracaoContaContabilEscrituracao();
            IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao> configuracaoContaContabilProvisao = repConfiguracaoContaContabilProvisao.ConsultarConfiguracaoContaContabilProvisao();

            foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil in ConfiguracoesContaContabil)
            {
                int codigoContaContabil = configuracaoContaContabil.Codigo;

                configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes = configuracaoContaContabilContabilizacao
                    .Where(obj => obj.CodigoConfiguracaoContaContabil == codigoContaContabil)
                    .ToList();

                configuracaoContaContabil.ConfiguracaoContaContabilEscrituracoes = configuracaoContaContabilEscrituracao
                    .Where(obj => obj.CodigoConfiguracaoContaContabil == codigoContaContabil)
                    .ToList();

                configuracaoContaContabil.ConfiguracaoContaContabilProvisoes = configuracaoContaContabilProvisao
                    .Where(obj => obj.CodigoConfiguracaoContaContabil == codigoContaContabil)
                    .ToList();

                foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao contabilizacao in configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes)
                {
                    contabilizacao.ConfiguracaoContaContabilDescricao = configuracaoContaContabil.Descricao;
                }

                foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao escrituracao in configuracaoContaContabil.ConfiguracaoContaContabilEscrituracoes)
                {
                    escrituracao.ConfiguracaoContaContabilDescricao = configuracaoContaContabil.Descricao;
                }

                foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao provisao in configuracaoContaContabil.ConfiguracaoContaContabilProvisoes)
                {
                    provisao.ConfiguracaoContaContabilDescricao = configuracaoContaContabil.Descricao;
                }
            }
        }
    }
}
