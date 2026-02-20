using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao.Unilever
{
    public class DocumentoDestinado
    {
        #region Atributos
        private readonly Repositorio.UnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public DocumentoDestinado(Repositorio.UnitOfWork unitOfWork) : base() { _unitOfWork = unitOfWork; }
        #endregion

        #region Metodos Publicos

        public void GerarIntegracaoDocumentoDestinado(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa, TipoDocumentoDestinadoEmpresa tipoDocumentoDestinadoEmpresa)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repositorioDocumentoDestinado = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP existeConfiguracao = ObterConfiguracao();
            Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever integracaoUnilever = new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(_unitOfWork);

            List<TipoDocumentoDestinadoEmpresa> tipoDocumentoDestinadoPermitidosGeracao = new List<TipoDocumentoDestinadoEmpresa>()
            {
                TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador,
                TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario,
                TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente,
                TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente,
                TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor,
                TipoDocumentoDestinadoEmpresa.CTeOSDestinadoTomador,
                TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor,
                TipoDocumentoDestinadoEmpresa.CTeDestinadoTerceiro,                
                TipoDocumentoDestinadoEmpresa.NFeDestinada,
                TipoDocumentoDestinadoEmpresa.CancelamentoCTe,
                TipoDocumentoDestinadoEmpresa.CancelamentoNFe
            };

            List<TipoIntegracao> tiposIntegracoesAGerarPorTipoDocumento = new List<TipoIntegracao>() {
                   TipoIntegracao.UnileverXml                   
            };

            if (tipoDocumentoDestinadoEmpresa == TipoDocumentoDestinadoEmpresa.CancelamentoNFe || tipoDocumentoDestinadoEmpresa == TipoDocumentoDestinadoEmpresa.CancelamentoCTe)
                tiposIntegracoesAGerarPorTipoDocumento.Add(TipoIntegracao.UnileverStatus);

            if (existeConfiguracao == null || !tipoDocumentoDestinadoPermitidosGeracao.Contains(tipoDocumentoDestinadoEmpresa))
                return;

            foreach (TipoIntegracao tipoIntegracao in tiposIntegracoesAGerarPorTipoDocumento)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao existeTipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(tipoIntegracao);
                if (existeTipoIntegracao == null)
                    continue;

                GerarIntegracaoDocumentoDestinadoPorTipo(documentoDestinadoEmpresa, existeTipoIntegracao);
            }
        }


        public void ProcessarIntegracoesPendentesDestinados()
        {
            Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repositorioDocumentoDestinadoIntegracao = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> integracoesPendentes = repositorioDocumentoDestinadoIntegracao.BuscarIntegracoesPendentes(quantideRegistro: 200, numeroTentativas: 3, minutosACadaTentativa: 3);

            foreach (Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao integracaoPendente in integracoesPendentes)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.UnileverXml:
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(_unitOfWork).IntegrarXmlDocumentoDestinado(integracaoPendente);
                        break;
                    case TipoIntegracao.UnileverStatus:                        
                        new Servicos.Embarcador.Integracao.Unilever.IntegracaoUnilever(_unitOfWork).IntegrarStatusDocumentoDestinado(integracaoPendente);
                        break;
                }
            }
        }


        public void VincularCTeComPreCte()
        {
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCte = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            Repositorio.DocumentosCTE repositorioDocumentosCte = new Repositorio.DocumentosCTE(_unitOfWork);
            Repositorio.DocumentosPreCTE repositorioDocumentosPreCte = new Repositorio.DocumentosPreCTE(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            var svcControleDoc = new Servicos.Embarcador.Documentos.ControleDocumento(_unitOfWork);

            Repositorio.PreConhecimentoDeTransporteEletronico repositorioPreCte = new Repositorio.PreConhecimentoDeTransporteEletronico(_unitOfWork);
            try
            {
                List<int> codigoCTesSemVinculoComCargaCte = repositorioCte.BuscarCTesSemVinculos();
                foreach (var codigoCte in codigoCTesSemVinculoComCargaCte)
                {
                    List<string> chavesNotasCteVincular = repositorioDocumentosCte.BuscarChavesPorCTe(codigoCte);
                    List<int> prectes = repositorioDocumentosPreCte.BuscarCodigosPreCTesPorChavesNFe(chavesNotasCteVincular);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCtes = repositorioCargaCte.BuscarSemCtePorPreCte(prectes);
                    foreach (var cargaCte in cargasCtes)
                    {
                        _unitOfWork.Start();
                        
                        cargaCte.CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico() { Codigo = codigoCte };
                        repositorioCargaCte.Atualizar(cargaCte);
                        Dominio.Entidades.Embarcador.Documentos.ControleDocumento existeControleDocumentoPorCteSemCargaCte = repositorioControleDocumento.BuscarPorCodigoCtESemCargaCte(codigoCte);
                      if(existeControleDocumentoPorCteSemCargaCte != null)
                        {
                            existeControleDocumentoPorCteSemCargaCte.CargaCTe = cargaCte;
                            existeControleDocumentoPorCteSemCargaCte.SituacaoVerificacao = SituacaoVerificacaoControleDocumento.AgVerificacao;
                            existeControleDocumentoPorCteSemCargaCte.SituacaoControleDocumento = svcControleDoc.ObterSituacaoDocumento(cargaCte.CTe);
                            repositorioControleDocumento.Atualizar(existeControleDocumentoPorCteSemCargaCte);
                        }
                        
                        _unitOfWork.CommitChanges();
                        _unitOfWork.FlushAndClear();
                    }
                }
            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
            }
        }
        #endregion

        #region Metodos Privados
        private void GerarIntegracaoDocumentoDestinadoPorTipo(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repositorioDocumentoDestinadoIntegracao = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao documentoDestinadoIntegracao = new Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao()
            {
                TipoIntegracao = tipoIntegracao,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                NumeroTentativas = 0,
                DocumentoDestinadoEmpresa = documentoDestinadoEmpresa,
            };

            repositorioDocumentoDestinadoIntegracao.Inserir(documentoDestinadoIntegracao);
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP ObterConfiguracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDestinadosSAP repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoDestinadosSAP(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP configuracaoIntegracao = repositorioIntegracao.BuscarPrimeiroRegistro();

            if (configuracaoIntegracao == null)
                return null;

            return configuracaoIntegracao;
        }

        #endregion
    }
}
