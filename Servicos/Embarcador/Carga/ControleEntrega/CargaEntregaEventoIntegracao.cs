using Servicos.Embarcador.Integracao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class CargaEntregaEventoIntegracao
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CargaEntregaEventoIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void GerarIntegracoes(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao cargaEntregaEventoIntegracao = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao()
                {
                    CargaEntregaEvento = cargaEntregaEvento,
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                    DataIntegracao = DateTime.Now,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = "",
                    TipoIntegracao = tipoIntegracao
                };

                repCargaEntregaEventoIntegracao.Inserir(cargaEntregaEventoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
                };
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaEventoIntegracao, "Registro de integração.", _unitOfWork);
            }
        }

        public async Task ProcessarIntegracoesPendentesAsync()
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(_unitOfWork);
            int intervaloTempoRejeitadas = 5;
            List<int> integracoesPendentes = repCargaEntregaEventoIntegracao.BuscarIntegracoesPorSituacao(intervaloTempoRejeitadas, 100);
            int total = integracoesPendentes.Count();

            for (int i = 0; i < total; i++)
            {
                await ProcessarIntegracaoPendenteAsync(integracoesPendentes[i]);

                _unitOfWork.FlushAndClear();
            }
        }

        public async Task ProcessarIntegracaoPendenteAsync(int codigoCargaEntregaEventoIntegracao)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao cargaEntregaEventoIntegracao = repCargaEntregaEventoIntegracao.BuscarPorCodigo(codigoCargaEntregaEventoIntegracao);

            switch (cargaEntregaEventoIntegracao?.TipoIntegracao.Tipo ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal:
                    ProcessarIntegracao_ArcelorMittal(cargaEntregaEventoIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LoggiEventosEntrega:
                    ProcessarIntegracao_LoggiEventosEntrega(cargaEntregaEventoIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YPEEventosEntrega:
                    ProcessarIntegracao_YPEEventosEntrega(cargaEntregaEventoIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buntech:
                    await ProcessarIntegracao_BuntechEventosEntregaAsync(cargaEntregaEventoIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CassolEventosEntrega:
                    ProcessarIntegracao_CassolEventosEntrega(cargaEntregaEventoIntegracao);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AcocearenseEventosEntrega:
                    ProcessarIntegracao_AcocearenseEventoEntrega(cargaEntregaEventoIntegracao);
                    break;
            }
        }

        #endregion

        #region Métodos Privados

        private void ProcessarIntegracao_ArcelorMittal(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao integracao)
        {
            try
            {
                Integracao.ArcelorMittal.IntegracaoArcelorMittal servicoIntegracaoArcelorMittal = new Integracao.ArcelorMittal.IntegracaoArcelorMittal(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = servicoIntegracaoArcelorMittal.EnviarOcorrencia(integracao.CargaEntregaEvento);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao.Codigo, e.Message);
            }
        }

        private void ProcessarIntegracao_LoggiEventosEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao integracao)
        {
            try
            {
                Integracao.Loggi.IntegracaoLoggiEventosEntrega svcInteracaoLoggiEventosEntrega = new Integracao.Loggi.IntegracaoLoggiEventosEntrega(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = svcInteracaoLoggiEventosEntrega.EnviarRequisicao(integracao.CargaEntregaEvento);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao.Codigo, e.Message);
            }
        }

        private void ProcessarIntegracao_YPEEventosEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao integracao)
        {
            try
            {
                Integracao.YPE.IntegracaoYPEEventosEntrega svcIntegracaoYPEEventosEntrega = new Integracao.YPE.IntegracaoYPEEventosEntrega(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = svcIntegracaoYPEEventosEntrega.IntegrarEventoEntrega(integracao.CargaEntregaEvento);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao.Codigo, e.Message);
            }
        }

        private async Task ProcessarIntegracao_BuntechEventosEntregaAsync(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao integracao)
        {
            try
            {
                Integracao.Buntech.IntegracaoBuntech svcIntegracaoBuntechEventosEntrega = new Integracao.Buntech.IntegracaoBuntech(_unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = await svcIntegracaoBuntechEventosEntrega.IntegrarEventoEntregaAsync(integracao.CargaEntregaEvento);

                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao.Codigo, e.Message);
            }
        }

        private void ProcessarIntegracao_CassolEventosEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao integracao)
        {
            try
            {
                Integracao.Cassol.IntegracaoCassolEventosEntrega svcIntegracaoCassolEventosEntrega = new(_unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = svcIntegracaoCassolEventosEntrega.IntegrarEventoEntrega(integracao);
                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao.Codigo, e.Message);
            }
        }

        private void ProcessarIntegracao_AcocearenseEventoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao integracao)
        {
            try
            {
                Integracao.YMS.IntegracaoYMS svcIntegracaoAcocearenseEventoEntrega = new(_unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = svcIntegracaoAcocearenseEventoEntrega.IntegrarEventoEntrega(integracao);
                SalvarResultadoIntegracao(integracao, httpRequisicaoResposta);
            }
            catch (Exception e)
            {
                SalvarResultadoIntegracaoFalha(integracao.Codigo, e.Message);
            }
        }

        #endregion

        #region Métodos Privados - Salvar Resultado

        private void SalvarResultadoIntegracao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao integracao, Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta)
        {
            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;
            integracao.ProblemaIntegracao = httpRequisicaoResposta.mensagem;
            integracao.SituacaoIntegracao = httpRequisicaoResposta.sucesso ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            servicoArquivoTransacao.Adicionar(integracao, httpRequisicaoResposta.conteudoRequisicao, httpRequisicaoResposta.conteudoResposta, httpRequisicaoResposta.extensaoResposta);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(_unitOfWork);
            repCargaEntregaEventoIntegracao.Atualizar(integracao);
        }

        private void SalvarResultadoIntegracaoFalha(int idIntegracao, string mensagem)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao repCargaEntregaEventoIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao(_unitOfWork);

            if (idIntegracao > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEventoIntegracao integracao = repCargaEntregaEventoIntegracao.BuscarPorCodigo(idIntegracao);

                integracao.NumeroTentativas++;
                integracao.DataIntegracao = DateTime.Now;
                integracao.ProblemaIntegracao = mensagem;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                repCargaEntregaEventoIntegracao.Atualizar(integracao);
            }
        }

        #endregion
    }
}
