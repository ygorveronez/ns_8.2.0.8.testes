using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoDocumentoEntrada
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        public IntegracaoDocumentoEntrada(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Métodos Públicos

        public void IniciarIntegracoesDeDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada)
        {

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        if (documentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado ||
                            documentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Cancelado)
                            AdcionarDocumentoEntradaIntegracao(documentoEntrada, tipoIntegracao, _unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }

        public void VerificarIntegracoesPendentesDocumentoEntrada()
        {
            int numeroTentativas = 5;
            int minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao> documentosEntradaIntegracao = repDocumentoEntradaIntegracao.BuscarPendentesIntegracao(numeroRegistrosPorVez, numeroTentativas, minutosACadaTentativa);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> documentoEntrada = (from obj in documentosEntradaIntegracao select obj.DocumentoEntrada).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao documentoIntegracao in documentosEntradaIntegracao)
            {
                switch (documentoIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        if (documentoIntegracao.DocumentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(_unitOfWork).IntegrarDocumentoEntrada(documentoIntegracao);
                        else if (documentoIntegracao.DocumentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Cancelado)
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(_unitOfWork).IntegrarCancelamentoDocumentoEntrada(documentoIntegracao);
                        break;
                    default:
                        break;
                }

                repDocumentoEntradaIntegracao.Atualizar(documentoIntegracao);
            }

        }
        public void AdcionarDocumentoEntradaIntegracao(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repositorioDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao documentoEntradaIntegracao = repositorioDocumentoEntradaIntegracao.BuscarIntegracaoPorCodigoDocumentoEntrada(documentoEntrada.Codigo);

            if (documentoEntradaIntegracao?.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
            {
                documentoEntradaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                repositorioDocumentoEntradaIntegracao.Atualizar(documentoEntradaIntegracao);

                return;
            }
            else if (documentoEntradaIntegracao?.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
            {
                return;
            }

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoDocumentoEntrada = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao documentoIntegracao = new Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao()
            {
                DocumentoEntrada = documentoEntrada,
                DataIntegracao = DateTime.Now,
                TipoIntegracao = tipoIntegracaoDocumentoEntrada,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
            };

            repositorioDocumentoEntradaIntegracao.Inserir(documentoIntegracao);
        }
        #endregion

    }
}
