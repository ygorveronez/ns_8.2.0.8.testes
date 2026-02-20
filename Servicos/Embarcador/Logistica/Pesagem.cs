using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public class Pesagem
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Pesagem(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void VerificarPesagemIntegracoesPendentes(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Logistica.PesagemIntegracao repositorioPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);

            Servicos.Embarcador.Integracao.Toledo.IntegracaoToledo servicoToledo = new Integracao.Toledo.IntegracaoToledo(_unitOfWork);
            Servicos.Embarcador.Integracao.Qbit.IntegracaoQbit servicoQbit = new Integracao.Qbit.IntegracaoQbit(_unitOfWork);
            Servicos.Embarcador.Integracao.Deca.IntegracaoDeca servicoDeca = new Integracao.Deca.IntegracaoDeca(_unitOfWork);
            Servicos.Embarcador.Integracao.BalancaKIKI.IntegracaoBalancaKIKI servicoIntegracaoBalancaKIKI = new Integracao.BalancaKIKI.IntegracaoBalancaKIKI(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao> pesagemIntegracoes = repositorioPesagemIntegracao.BuscarIntegracaoPendente(5, 5, "Codigo", "asc", 20, TipoEnvioIntegracao.Individual);
            List<Dominio.Entidades.Embarcador.Logistica.Pesagem> pesagens = new List<Dominio.Entidades.Embarcador.Logistica.Pesagem>();
            for (int i = 0; i < pesagemIntegracoes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem = pesagemIntegracoes[i];
                if (integracaoPesagem.TipoIntegracao.Tipo == TipoIntegracao.Toledo)
                {
                    if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.CadastroVeiculo)
                        servicoToledo.EnviarCriacaoTicketToledo(integracaoPesagem, tipoServicoMultisoftware);
                    else if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.Bloqueado)
                        servicoToledo.AplicarManutencaoTicket(1, integracaoPesagem.Pesagem, null, integracaoPesagem);
                    else if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.AguardandoLiberacao)
                        servicoToledo.AplicarManutencaoTicket(2, integracaoPesagem.Pesagem, null, integracaoPesagem);
                    else if (integracaoPesagem.TipoIntegracaoBalanca == TipoIntegracaoBalanca.RefazerPesagem)
                        servicoToledo.RefazUltimaOperacaoAtiva(integracaoPesagem.Pesagem, integracaoPesagem);
                }
                else if (integracaoPesagem.TipoIntegracao.Tipo == TipoIntegracao.Qbit)
                    servicoQbit.ConsultarPesagensPlaca(integracaoPesagem);
                else if (integracaoPesagem.TipoIntegracao.Tipo == TipoIntegracao.Deca)
                    servicoDeca.ConsultarPesagensBalanca(integracaoPesagem);
                else if (integracaoPesagem.TipoIntegracao.Tipo == TipoIntegracao.BalancaKIKI)
                    servicoIntegracaoBalancaKIKI.ConsultarPesagensPlaca(integracaoPesagem);
                else
                {
                    integracaoPesagem.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracaoPesagem.ProblemaIntegracao = "Tipo de integração não implementada";
                    integracaoPesagem.NumeroTentativas++;
                }

                repositorioPesagemIntegracao.Atualizar(integracaoPesagem);

                if (!pesagens.Contains(integracaoPesagem.Pesagem))
                    pesagens.Add(integracaoPesagem.Pesagem);
            }

            foreach (Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem in pesagens)
            {
                List<Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao> pesagemIntegracaos = repositorioPesagemIntegracao.BuscarPorPesagem(pesagem.Codigo);

                if (pesagemIntegracaos.Any(obj => obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao))
                    pesagem.StatusBalanca = StatusBalanca.FalhaIntegracao;
                else if (pesagemIntegracaos.Any(obj => obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao))
                    pesagem.StatusBalanca = StatusBalanca.AgIntegracao;

                repositorioPesagem.Atualizar(pesagem);
            }

            servicoToledo.ConsultarConfirmarTicketsCadastrados();
        }

        public Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao GerarIntegracoes(Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem, TipoIntegracaoBalanca tipoIntegracaoBalanca, SituacaoIntegracao situacaoIntegracao)
        {
            return GerarPesagemIntegracao(pesagem, TipoIntegracao.Toledo, tipoIntegracaoBalanca, situacaoIntegracao);
        }

        public Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao GerarAtualizarIntegracoes(Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem, TipoIntegracaoBalanca tipoIntegracaoBalanca, SituacaoIntegracao situacaoIntegracao, int codigoBalanca, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);
            Repositorio.Embarcador.Filiais.FilialBalanca repFilialBalanca = new Repositorio.Embarcador.Filiais.FilialBalanca(_unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.FilialBalanca filialBalanca = repFilialBalanca.BuscarPorCodigo(codigoBalanca, false);

            Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao pesagemIntegracao = repPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, tipoIntegracaoBalanca);
            if (pesagemIntegracao != null)
            {
                pesagemIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                pesagemIntegracao.Balanca = filialBalanca;
                repPesagemIntegracao.Atualizar(pesagemIntegracao, auditado, null, "Atualizou a integração");
                return pesagemIntegracao;
            }

            return GerarPesagemIntegracao(pesagem, TipoIntegracao.Deca, tipoIntegracaoBalanca, situacaoIntegracao, null, filialBalanca);
        }

        public void GerarPesagemInicialPorFluxoGestaoPatio(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio)
        {
            if (!sequenciaGestaoPatio.GuaritaEntradaTipoIntegracaoBalanca.HasValue)
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);

            List<TipoIntegracao> tiposIntegracaoPesagem = new List<TipoIntegracao> { TipoIntegracao.Qbit, TipoIntegracao.BalancaKIKI };//Integrações de geração automática

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarPorTipos(tiposIntegracaoPesagem);
            if (tiposIntegracao.Count == 0)
                return;

            Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = GerarPesagemPorFluxoGestaoPatio(fluxoGestaoPatio);
            if (pesagem == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoGerar = tiposIntegracao.Where(o => o.Tipo == sequenciaGestaoPatio.GuaritaEntradaTipoIntegracaoBalanca).FirstOrDefault();//Só poderá uma Pesagem por fluxo de pátio
            if (tipoIntegracaoGerar == null)
                return;

            Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem = GerarPesagemIntegracao(pesagem, tipoIntegracaoGerar.Tipo, TipoIntegracaoBalanca.PesagemInicial, SituacaoIntegracao.AgIntegracao, tipoIntegracaoGerar, sequenciaGestaoPatio.BalancaGuaritaEntrada);

            if (integracaoPesagem != null)
                pesagem.StatusBalanca = StatusBalanca.AgIntegracao;

            repositorioPesagem.Atualizar(pesagem);
        }

        public void GerarPesagemFinalPorFluxoGestaoPatio(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio)
        {
            if (!sequenciaGestaoPatio.GuaritaSaidaTipoIntegracaoBalanca.HasValue)
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.Pesagem repositorioPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);
            Repositorio.Embarcador.Logistica.PesagemIntegracao repositorioPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);

            List<TipoIntegracao> tiposIntegracaoPesagem = new List<TipoIntegracao> { TipoIntegracao.Qbit, TipoIntegracao.BalancaKIKI };//Integrações de geração automática

            Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = repositorioPesagem.BuscarPorCarga(fluxoGestaoPatio.Carga.Codigo);
            if (pesagem == null)
                return;

            Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagem = repositorioPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.PesagemFinal);
            if (integracaoPesagem != null)//Validar volta de etapa, para não deixar gerar novamente
                return;

            Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao integracaoPesagemInicial = repositorioPesagemIntegracao.BuscarPorPesagemETipoIntegracao(pesagem.Codigo, TipoIntegracaoBalanca.PesagemInicial);
            if (integracaoPesagemInicial == null || !tiposIntegracaoPesagem.Contains(integracaoPesagemInicial.TipoIntegracao.Tipo))
                return;

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarPorTipos(tiposIntegracaoPesagem);
            if (tiposIntegracao.Count == 0)
                return;

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoGerar = tiposIntegracao.Where(o => o.Tipo == sequenciaGestaoPatio.GuaritaSaidaTipoIntegracaoBalanca).FirstOrDefault();
            if (tipoIntegracaoGerar == null)
                return;

            integracaoPesagem = GerarPesagemIntegracao(pesagem, tipoIntegracaoGerar.Tipo, TipoIntegracaoBalanca.PesagemFinal, SituacaoIntegracao.AgIntegracao, tipoIntegracaoGerar, sequenciaGestaoPatio.BalancaGuaritaSaida);

            if (integracaoPesagem != null)
                pesagem.StatusBalanca = StatusBalanca.AgIntegracao;

            repositorioPesagem.Atualizar(pesagem);
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Logistica.Pesagem GerarPesagemPorFluxoGestaoPatio(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Logistica.Pesagem repPesagem = new Repositorio.Embarcador.Logistica.Pesagem(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioCargaGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioCargaGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);
            if (guarita == null)
                return null;

            Dominio.Entidades.Embarcador.Logistica.Pesagem pesagemExite = repPesagem.BuscarPorGuarita(guarita.Codigo);
            if (pesagemExite != null)
                return null;

            Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem = new Dominio.Entidades.Embarcador.Logistica.Pesagem()
            {
                DataPesagem = DateTime.Now,
                StatusBalanca = StatusBalanca.TicketCriado,
                Guarita = guarita
            };

            repPesagem.Inserir(pesagem);

            return pesagem;
        }

        private Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao GerarPesagemIntegracao(Dominio.Entidades.Embarcador.Logistica.Pesagem pesagem, TipoIntegracao tipoIntegracao, TipoIntegracaoBalanca tipoIntegracaoBalanca, SituacaoIntegracao situacaoIntegracao = SituacaoIntegracao.AgIntegracao, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo = null, Dominio.Entidades.Embarcador.Filiais.FilialBalanca filialBalanca = null)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Logistica.PesagemIntegracao repPesagemIntegracao = new Repositorio.Embarcador.Logistica.PesagemIntegracao(_unitOfWork);

            if (tipo == null)
                tipo = repTipoIntegracao.BuscarPorTipo(tipoIntegracao);

            if (tipo == null)
            {
                pesagem.StatusBalanca = StatusBalanca.FalhaIntegracao;
                return null;
            }

            Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao pesagemIntegracao = new Dominio.Entidades.Embarcador.Logistica.PesagemIntegracao()
            {
                TipoIntegracao = tipo,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                Pesagem = pesagem,
                SituacaoIntegracao = situacaoIntegracao,
                TipoIntegracaoBalanca = tipoIntegracaoBalanca,
                Balanca = filialBalanca
            };

            repPesagemIntegracao.Inserir(pesagemIntegracao);

            return pesagemIntegracao;
        }

        #endregion
    }
}
