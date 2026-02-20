using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoNotaFiscalEletronica
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        public IntegracaoNotaFiscalEletronica(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Métodos Públicos

        public void IniciarIntegracoesNotaFiscalEletronica(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscalEletronica)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repositorioNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao notaFiscalEletronicaIntegracao = repositorioNotaFiscalEletronicaIntegracao.BuscarIntegracaoPorCodigoNotaFiscal(notaFiscalEletronica.Codigo);

            if (notaFiscalEletronicaIntegracao?.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado &&
                notaFiscalEletronica.Status == Dominio.Enumeradores.StatusNFe.Cancelado)
            {
                notaFiscalEletronicaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                repositorioNotaFiscalEletronicaIntegracao.Atualizar(notaFiscalEletronicaIntegracao);

                return;
            }
            else if (notaFiscalEletronicaIntegracao?.SituacaoIntegracao != null)
            {
                return;
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        if (notaFiscalEletronica.Status == Dominio.Enumeradores.StatusNFe.Autorizado ||
                            notaFiscalEletronica.Status == Dominio.Enumeradores.StatusNFe.Cancelado)
                            AdicionarNotaFiscalEletronicaIntegracao(notaFiscalEletronica, tipoIntegracao, _unitOfWork);
                        break;
                    default:
                        break;
                }
            }
        }

        public void VerificarIntegracoesPendentesNotaFiscalEletronica()
        {
            int numeroTentativas = 5;
            int minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao> notasFiscaisEletronicasIntegracao = repNotaFiscalEletronicaIntegracao.BuscarPendentesIntegracao(numeroRegistrosPorVez, numeroTentativas, minutosACadaTentativa);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> notaFiscalEletronica = (from obj in notasFiscaisEletronicasIntegracao select obj.NotaFiscal).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao notaFiscalIntegracao in notasFiscaisEletronicasIntegracao)
            {
                switch (notaFiscalIntegracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        if (notaFiscalIntegracao.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Autorizado)
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(_unitOfWork).IntegrarNotaFiscalEletronica(notaFiscalIntegracao);
                        else if (notaFiscalIntegracao.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Cancelado)
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(_unitOfWork).IntegrarCancelamentoNotaFiscalEletronica(notaFiscalIntegracao);
                        break;
                    default:
                        break;
                }

                repNotaFiscalEletronicaIntegracao.Atualizar(notaFiscalIntegracao);
            }

        }
        public void AdicionarNotaFiscalEletronicaIntegracao(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscalEletronica, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao repositorioNotaFiscalEletronicaIntegracao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoNotaFiscalEletronica = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao notaFiscalIntegracao = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao()
            {
                NotaFiscal = notaFiscalEletronica,
                DataIntegracao = DateTime.Now,
                TipoIntegracao = tipoIntegracaoNotaFiscalEletronica,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
            };

            repositorioNotaFiscalEletronicaIntegracao.Inserir(notaFiscalIntegracao);
        }
        #endregion

    }
}
