using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System.Collections.Generic;

namespace Servicos.Embarcador.Pessoa
{
    public class PessoaIntegracao
    {
        #region Atributos
        private readonly Repositorio.UnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public PessoaIntegracao(Repositorio.UnitOfWork unitOfWork) : base()
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Metodos Publicos

        public void ProcessarIntegracoesPendentesPessoa()
        {
            Repositorio.Embarcador.Pessoas.PessoaIntegracao repIntegracao = new Repositorio.Embarcador.Pessoas.PessoaIntegracao(this._unitOfWork);
            List<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> integracoesPendentes = repIntegracao.BuscarPendentesIntegracao(20, 5, 2);
            Repositorio.Embarcador.Configuracoes.IntegracaoSIC repositorioIntegracaoSIC = new Repositorio.Embarcador.Configuracoes.IntegracaoSIC(this._unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC = repositorioIntegracaoSIC.Buscar();
            
            foreach (var integracaoPendente in integracoesPendentes)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.SIC:
                        if (configuracaoIntegracaoSIC?.PossuiIntegracaoSIC ?? false)
                        {
                            if (string.IsNullOrEmpty(integracaoPendente.Protocolo))
                                new Servicos.Embarcador.Integracao.SIC.IntegracaoSIC(_unitOfWork).SalvarListaClienteTerceiro(integracaoPendente, configuracaoIntegracaoSIC);
                            else
                                new Servicos.Embarcador.Integracao.SIC.IntegracaoSIC(_unitOfWork).ConsultaInformacaoProtocoloInclusaoCadastro(integracaoPendente, configuracaoIntegracaoSIC);
                        }
                        break;

                    case TipoIntegracao.KMM:
                        var kmmService = new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(_unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
                        kmmService.IntegrarPessoaMotorista(integracaoPendente);
                        break;
                    case TipoIntegracao.Globus:
                        var globusService = new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(_unitOfWork);
                        globusService.IntegrarPessoa(integracaoPendente);
                        break;
                }
            }

        }

        public static void ReIntegrar(UnitOfWork unitOfWork, int idIntegracao)
        {
            Repositorio.Embarcador.Pessoas.PessoaIntegracao repIntegracao = new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao domIntegracao = repIntegracao.BuscarPorId(idIntegracao);
            if (!(domIntegracao == null))
            {// não existe integração 
                domIntegracao.ProblemaIntegracao = "";
                domIntegracao.NumeroTentativas = 0;
                domIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                repIntegracao.Atualizar(domIntegracao);
            }

        }
        
        #endregion

    }
}
