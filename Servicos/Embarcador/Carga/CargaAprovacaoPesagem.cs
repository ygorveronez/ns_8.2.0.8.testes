using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public sealed class CargaAprovacaoPesagem : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita,
        Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem,
        Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio _configuracaoGestaoPatio;

        #endregion Atributos Privados

        #region Construtores

        public CargaAprovacaoPesagem(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoGestaoPatio: null) { }

        public CargaAprovacaoPesagem(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio configuracaoGestaoPatio) : base(unitOfWork)
        {
            _configuracaoGestaoPatio = configuracaoGestaoPatio;
        }

        #endregion Construtores

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita, List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.GestaoPatio.AprovacaoAlcadaToleranciaPesagem repositorio = new Repositorio.Embarcador.GestaoPatio.AprovacaoAlcadaToleranciaPesagem(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem aprovacao = new Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem()
                        {
                            OrigemAprovacao = guarita,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = guarita.Carga.DataCriacaoCarga,
                            NumeroAprovadores = regra.NumeroAprovadores,
                            GuidCarga = Guid.NewGuid().ToString()
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(guarita, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    var aprovacao = new Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem()
                    {
                        OrigemAprovacao = guarita,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = guarita.Carga.DataCriacaoCarga,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            if (existeRegraSemAprovacao)
                guarita.SituacaoPesagemCarga = SituacaoPesagemCarga.AguardandoAprovacao;
            else
                guarita.SituacaoPesagemCarga = SituacaoPesagemCarga.Aprovada;
        }

        private bool IsCriarRegrasAprovacao()
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio configuracaoGestaoPatio = ObterConfiguracaoFluxoPatio();

            if (!(configuracaoGestaoPatio.ValidarPesoCargaComPesagemVeiculo ?? false))
                return false;

            return true;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio ObterConfiguracaoFluxoPatio()
        {
            if (_configuracaoGestaoPatio == null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio repositorioConfiguracaoFluxoPatio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio(_unitOfWork);
                _configuracaoGestaoPatio = repositorioConfiguracaoFluxoPatio.BuscarConfiguracaoPadrao();
            }

            return _configuracaoGestaoPatio;
        }

        private List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem>(_unitOfWork);

            List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem>();

            foreach (Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem regra in listaRegras)
            {
                int codigoFilial = carga.Filial?.Codigo ?? 0;
                if (carga.FilialOrigem != null)
                    codigoFilial = carga.FilialOrigem.Codigo;

                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, codigoFilial))
                    continue;

                if (regra.RegraPorModeloVeicularCarga && !ValidarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(regra.AlcadasModeloVeicularCarga, carga.ModeloVeicularCarga?.Codigo))
                    continue;

                if (regra.RegraPorTipoCarga && !ValidarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(regra.AlcadasTipoCarga, carga.TipoDeCarga?.Codigo))
                    continue;

                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, carga.TipoOperacao?.Codigo))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita, Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: guarita.Carga.Codigo,
                URLPagina: "GestaoPatio/AutorizacaoPesagem",
                titulo: Localization.Resources.Frotas.OrdemServico.TituloOrdemServico,
                nota: $"Criada Solicitação Aprovação Pesagem da carga: {guarita.Carga.CodigoCargaEmbarcador}",
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio servicoMensagemAlerta = new Servicos.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio(_unitOfWork);

            servicoMensagemAlerta.Confirmar(guarita.FluxoGestaoPatio, TipoMensagemAlerta.CargaSemRegraAutorizacaoTolerenciaPesagem);
            servicoMensagemAlerta.Confirmar(guarita.FluxoGestaoPatio, TipoMensagemAlerta.CargaAguardandoAprovacaoPesagem);

            RemoverAprovacao(guarita);

            if (!IsCriarRegrasAprovacao())
                return;

            List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem> regras = ObterRegrasAutorizacao(guarita.Carga);

            if (regras.Count > 0)
            {
                CriarRegrasAprovacao(guarita, regras, tipoServicoMultisoftware);
                servicoMensagemAlerta.Adicionar(guarita.FluxoGestaoPatio, TipoMensagemAlerta.CargaAguardandoAprovacaoPesagem, "Aguardando aprovação da pesagem");

                return;
            }

            servicoMensagemAlerta.Adicionar(guarita.FluxoGestaoPatio, TipoMensagemAlerta.CargaSemRegraAutorizacaoTolerenciaPesagem, "Não foram encontradas regras de aprovação compativel para Tolerancia de pesagem");
            throw new ServicoException("Não foram encontradas regras de aprovação compativel para Tolerancia de pesagem");
        }

        public override void RemoverAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita)
        {
            base.RemoverAprovacao(guarita);

            guarita.SituacaoPesagemCarga = SituacaoPesagemCarga.NaoInformada;
        }

        #endregion Métodos Públicos
    }
}
