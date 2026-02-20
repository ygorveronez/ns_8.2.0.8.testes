using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using FluentFTP;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Carga
{
    public class CargaEncerramento
    {
        #region Atributos

        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion

        #region Construtores

        public CargaEncerramento() { }

        public CargaEncerramento(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _auditado = auditado;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public void VerificarEncerramentoCIOT(Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            string mensagem = "";

            try
            {
                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCarga(cargaRegistroEncerramento.Carga.Codigo);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                if (ciot != null && (!configuracaoGeralCarga?.NaoPermitirEncerrarCIOTEncerrarCarga ?? false))
                {
                    ObterSituacaoEncerramentoCIOT(ciot, tipoServicoMultisoftware, unitOfWork, out mensagem);
                }

                AlterarSituacaoEncerramento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoCarga.AgEncerramentoMDFe, mensagem, cargaRegistroEncerramento, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
            }
        }

        public void VerificarEncerramentoMDFe(Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string webServiceConsultaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaRegistroEncerramento.Carga.Codigo);

            string mensagemErro = "";
            bool existeMFFeNaCarga = false;

            try
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFEs = repCargaMDFe.BuscarPorCarga(cargaRegistroEncerramento.Carga.Codigo);

                List<string> falhouAoEncerrar = new List<string>();
                if (!cargaRegistroEncerramento.Carga.TipoOperacao?.EncerrarMDFeManualmente ?? false)
                {
                    existeMFFeNaCarga = cargaMDFEs.Count > 0;

                    if (existeMFFeNaCarga)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in cargaMDFEs)
                        {
                            EncerrarMDFe(ref mensagemErro, cargaMDFe, carga, repCargaMDFe, serCargaMDFe, configuracaoTMS, auditado, tipoServicoMultisoftware, webServiceConsultaCTe, unitOfWork);
                            if (!string.IsNullOrWhiteSpace(mensagemErro))
                                falhouAoEncerrar.Add(mensagemErro);
                        }
                    }

                    svcCarga.ValidarCargasFinalizadas(ref carga, tipoServicoMultisoftware, null, unitOfWork);
                }
                if (falhouAoEncerrar.Count > 0)
                    mensagemErro = falhouAoEncerrar.Join(" | ");

                AlterarSituacaoEncerramento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoCarga.AgIntegracao, mensagemErro, cargaRegistroEncerramento, unitOfWork, existeMDFeNaCarga: existeMFFeNaCarga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
            }
        }

        public void VerificarIntegracoesCargaEncerramento(Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarTipos();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_SU:
                        if (cargaRegistroEncerramento.Carga.FreteDeTerceiro)
                            AdicionarIntegracaoCargaRegistroEncerramento(cargaRegistroEncerramento, tipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        AdicionarIntegracaoCargaRegistroEncerramento(cargaRegistroEncerramento, tipoIntegracao, unitOfWork);
                        break;
                }
            }

            if (!cargaRegistroEncerramento.PossuiIntegracao)
            {
                cargaRegistroEncerramento.EncerrarSemIntegracao = true;
                repCargaRegistroEncerramento.Atualizar(cargaRegistroEncerramento);
            }
        }

        public void IniciarIntegracoesCargaEncerramentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao repositorioCargaRegistroIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao> integracoes = repositorioCargaRegistroIntegracao.BuscarPorCargaEncerramento(cargaRegistroEncerramento.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao integracao in integracoes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_SU:
                        AdicionarCargaRegistroEncerramentoCargaIntegracao(cargaRegistroEncerramento, integracao.TipoIntegracao, unitOfWork);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        AdicionarCargaRegistroEncerramentoCargaIntegracao(cargaRegistroEncerramento, integracao.TipoIntegracao, unitOfWork);
                        break;
                }
            }
        }

        public void VerificarIntegracoesCargaPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao> integracoesPendentes = repCargaCargaIntegracao.BuscarIntegracoesPendentes();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao integracaoPendente in integracoesPendentes)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_SU:
                        new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unitOfWork).IntegrarEncerramentoCarga(integracaoPendente);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).IntegrarCargaEncerramento(integracaoPendente);
                        break;
                }
            }
        }

        public void FinalizarCargaEncerramento(int codigoCargaEncerramento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = repCargaRegistroEncerramento.BuscarPorCodigo(codigoCargaEncerramento);
            try
            {
                unitOfWork.Start();
                EncerrarCarga(cargaRegistroEncerramento.Carga, unitOfWork);
                AlterarSituacaoEncerramento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoCarga.Encerrada, "", cargaRegistroEncerramento, unitOfWork, false, true);
                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();
            }
            catch (Exception ex)
            {

                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
            }
        }

        #endregion

        #region Métodos Privados

        private void ObterSituacaoEncerramentoCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string mensagem)
        {
            Servicos.Embarcador.CIOT.CIOT serCIOT = new Servicos.Embarcador.CIOT.CIOT();
            mensagem = "";

            if (ciot.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)
                return;
            else if (ciot.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
            {
                Servicos.Log.TratarErro("Thread EncerramentoCarga codigoCiot" + ciot.Codigo.ToString(), "QuitacaoCIOTCarga");
                bool sucesso = serCIOT.EncerrarCIOT(ciot, unitOfWork, tipoServicoMultisoftware, out mensagem);
                if (sucesso)
                    mensagem = "";
            }
            else
                mensagem = $"A situação do CIOT ({ciot.Situacao.ObterDescricao()}) não permite que ele seja encerrado.";
        }

        private void EncerrarMDFe(ref string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe, Servicos.Embarcador.Carga.MDFe serCargaMDFe, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string webServiceConsultaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaMDFe.MDFe != null && cargaMDFe.MDFe.Importado != true && (configuracaoTMS.PermiteEncerrarMDFeEmitidoNoEmbarcador || cargaMDFe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe))
            {
                if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe dadosEncerramento = serCargaMDFe.ObterDadosEncerramento(cargaMDFe.Codigo, unitOfWork);

                    mensagemErro = serCargaMDFe.EncerrarMDFe(dadosEncerramento.Codigo, carga.Codigo, dadosEncerramento.Localidades[0].Codigo, dadosEncerramento.DataEncerramento, webServiceConsultaCTe, auditado.Usuario, tipoServicoMultisoftware, unitOfWork, auditado);

                    if (string.IsNullOrWhiteSpace(mensagemErro))
                    {
                        if (auditado != null)
                        {
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaMDFe.MDFe, "Solicitou o encerramento do MDF-e.", unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaMDFe, "Solicitou o encerramento do MDF-e.", unitOfWork);
                        }

                        cargaMDFe.EmEncerramento = true;

                        repCargaMDFe.Atualizar(cargaMDFe);
                    }
                }
                else
                {
                    if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                    {
                        cargaMDFe.EmEncerramento = true;
                        repCargaMDFe.Atualizar(cargaMDFe);
                    }
                    else if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Enviado)
                    {
                        mensagemErro = "MDF-e enviado para encerramento";
                    }
                    else if (cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)
                    {
                        mensagemErro = "MDF-e Rejeitado: " + cargaMDFe.MDFe.MensagemRetornoSefaz;
                    }
                }

                if (auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, auditado.Texto, unitOfWork);
            }
        }

        private void AdicionarIntegracaoCargaRegistroEncerramento(Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoDaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao repCargaRegistroEncerramentoIntegracao = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(tipoDaIntegracao);

            if (!repCargaRegistroEncerramentoIntegracao.ExisteIntegracao(tipoIntegracao.Codigo, cargaRegistroEncerramento.Codigo))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao integracao = new Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoIntegracao();
                integracao.CargaRegistroEncerramento = cargaRegistroEncerramento;
                integracao.TipoIntegracao = tipoIntegracao;
                cargaRegistroEncerramento.PossuiIntegracao = true;
                repCargaRegistroEncerramento.Atualizar(cargaRegistroEncerramento);
                repCargaRegistroEncerramentoIntegracao.Inserir(integracao);
            }
        }

        private void AdicionarCargaRegistroEncerramentoCargaIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao(unitOfWork);

            if (repCargaRegistroEncerramento.PossuiIntegracoesPendentes(cargaRegistroEncerramento.Codigo))
            {
                return;
            }

            if (repCargaRegistroEncerramento.PossuiIntegracoesIntegrada(cargaRegistroEncerramento.Codigo))
            {
                return;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao cargaEncerramentoCargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramentoCargaIntegracao();

            cargaEncerramentoCargaIntegracao.CargaRegistroEncerramento = cargaRegistroEncerramento;
            cargaEncerramentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaEncerramentoCargaIntegracao.NumeroTentativas = 0;
            cargaEncerramentoCargaIntegracao.ProblemaIntegracao = "";
            cargaEncerramentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            cargaEncerramentoCargaIntegracao.TipoIntegracao = tipoIntegracao;

            repCargaRegistroEncerramento.Inserir(cargaEncerramentoCargaIntegracao);
        }

        private void EncerrarCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Carga.Carga servicoCarga = new Carga(unitOfWork);

            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada;
            carga = servicoCarga.AtualizarStatusCustoExtra(carga, servicoHubCarga, repCarga);
            carga.DataEncerramentoCarga = DateTime.Now;
            Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada.ObterDescricao()}", unitOfWork);

            repCarga.Atualizar(carga);


            if (_auditado != null)
                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, null, "Carga encerrada através do fluxo de encerramento de carga", unitOfWork);
        }

        private void AlterarSituacaoEncerramento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoCarga situacao, string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento, Repositorio.UnitOfWork unitOfWork, bool existeMDFeNaCarga = false, bool iniciouTransacao = false)
        {

            var repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unitOfWork);
            var svcHubCarga = new Hubs.Carga();

            if (!iniciouTransacao)
                unitOfWork.Start();

            if (RejeicaoDetectada(mensagem))
                AtualizarComoRejeitada(ref cargaRegistroEncerramento, mensagem);
            else
                AtualizarSituacaoNormal(ref cargaRegistroEncerramento, situacao, existeMDFeNaCarga, unitOfWork);

            repCargaRegistroEncerramento.Atualizar(cargaRegistroEncerramento);
            if (!iniciouTransacao)
                unitOfWork.CommitChanges();

            svcHubCarga.InformarEncerramentoAtualizado(cargaRegistroEncerramento.Codigo);
            if (!iniciouTransacao)
                unitOfWork.FlushAndClear();

        }

        private bool RejeicaoDetectada(string mensagem)
        {
            return !string.IsNullOrWhiteSpace(mensagem) && mensagem.Contains("Rejeitado");
        }

        private void AtualizarComoRejeitada(ref Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento, string mensagem)
        {
            cargaRegistroEncerramento.Situacao = SituacaoEncerramentoCarga.RejeicaoEncerramento;
            cargaRegistroEncerramento.MotivoRejeicao = mensagem;
        }

        private void AtualizarSituacaoNormal(ref Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEncerramentoCarga novaSituacao, bool existeMDFeNaCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaRegistroEncerramento.Situacao == SituacaoEncerramentoCarga.AgEncerramentoMDFe)
                cargaRegistroEncerramento.EncerrouMDFes = true;

            if (novaSituacao == SituacaoEncerramentoCarga.Encerrada)
            {
                cargaRegistroEncerramento.DataEncerramento = DateTime.Now;
            }
            else if (novaSituacao == SituacaoEncerramentoCarga.AgIntegracao)
            {
                if (existeMDFeNaCarga)
                {
                    var repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                    bool existeMDFeNaoEncerrado = repCargaMDFe.ExisteMDFeNaoEncerradoPorCarga(cargaRegistroEncerramento.Carga.Codigo);

                    if (existeMDFeNaoEncerrado)
                    {
                        cargaRegistroEncerramento.Situacao = SituacaoEncerramentoCarga.RejeicaoEncerramento;
                        cargaRegistroEncerramento.MotivoRejeicao = "Existe pelo menos um MDF-e com Status diferente de Encerrado na Carga";
                        cargaRegistroEncerramento.EncerrouMDFes = false;
                        return;
                    }
                }
            }

            cargaRegistroEncerramento.Situacao = novaSituacao;
        }

        #endregion
    }
}