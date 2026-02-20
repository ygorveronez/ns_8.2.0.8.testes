using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public sealed class CargaCancelamentoAprovacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao,
        Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento,
        Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion

        #region Construtores

        public CargaCancelamentoAprovacao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public CargaCancelamentoAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao cargaCancelamentoSolicitacao, List<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento repositorio = new Repositorio.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;
            List<Dominio.Entidades.Usuario> aprovadoresPorTransportador;

            if (regras.Any(regra => regra.TipoAprovadorRegra == TipoAprovadorRegra.Transportador))
                aprovadoresPorTransportador = new Repositorio.Usuario(_unitOfWork).BuscarUsuariosAcessoTransportador(cargaCancelamentoSolicitacao.CargaCancelamento.Carga?.Empresa?.Codigo ?? 0);
            else
                aprovadoresPorTransportador = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;
                    List<Dominio.Entidades.Usuario> aprovadores;

                    if (regra.TipoAprovadorRegra == TipoAprovadorRegra.Usuario)
                        aprovadores = regra.Aprovadores.ToList();
                    else
                        aprovadores = aprovadoresPorTransportador;

                    foreach (Dominio.Entidades.Usuario aprovador in aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento aprovacao = new Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento()
                        {
                            OrigemAprovacao = cargaCancelamentoSolicitacao,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = cargaCancelamentoSolicitacao.CargaCancelamento.DataEnvioCancelamento ?? DateTime.Now,
                            NumeroAprovadores = regra.NumeroAprovadores,
                            TipoAprovadorRegra = regra.TipoAprovadorRegra
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(cargaCancelamentoSolicitacao, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento aprovacao = new Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento()
                    {
                        OrigemAprovacao = cargaCancelamentoSolicitacao,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = cargaCancelamentoSolicitacao.CargaCancelamento.DataEnvioCancelamento ?? DateTime.Now,
                        TipoAprovadorRegra = regra.TipoAprovadorRegra
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            if (existeRegraSemAprovacao)
            {
                cargaCancelamentoSolicitacao.Situacao = SituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao;

                NotificarSituacaoAprovacaoAoOperadorCarga(cargaCancelamentoSolicitacao, tipoServicoMultisoftware);
            }
            else
                cargaCancelamentoSolicitacao.Situacao = SituacaoCargaCancelamentoSolicitacao.Aprovada;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao cargaCancelamentoSolicitacao)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCancelamentoSolicitacao.CargaCancelamento.Carga;
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento>(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            List<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento>();
            bool freteCalculadoPorFilialEmissora = (carga.EmpresaFilialEmissora != null && (configuracaoEmbarcador.CalcularFreteFilialEmissoraPorTabelaDeFrete || (carga.TipoOperacao?.CalculaFretePorTabelaFreteFilialEmissora ?? false)));
            decimal valorFrete = freteCalculadoPorFilialEmissora ? carga.ValorFreteAPagarFilialEmissora : carga.ValorFreteAPagar;

            foreach (var regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, carga.Filial?.Codigo))
                    continue;

                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, carga.TipoOperacao?.Codigo))
                    continue;

                if (regra.RegraPorValorFrete && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AlcadaValorFrete, decimal>(regra.AlcadasValorFrete, valorFrete))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private void ValidarAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao cargaCancelamentoSolicitacao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repositorioProvisao.BuscarPorCarga(cargaCancelamentoSolicitacao?.CargaCancelamento?.Carga?.Codigo ?? 0);

            if (cargaCancelamentoSolicitacao.CargaCancelamento.CTe != null)
            {
                if (provisao == null || provisao.DocumentosProvisao.Any(d => d.Stage.Cancelado || d.Stage.Inconsistente))
                {
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repositorioMDFe.BuscarPorCodigoCTe(cargaCancelamentoSolicitacao.CargaCancelamento.CTe.Codigo);

                    if (mdfe == null)
                        return;

                    cargaCancelamentoSolicitacao.CargaCancelamento.AguardandoXmlDesacordo = true;
                }
                else if (provisao != null && !provisao.DocumentosProvisao.Any(d => d.Stage.Cancelado || d.Stage.Inconsistente))
                    cargaCancelamentoSolicitacao.CargaCancelamento.AguardandoXmlDesacordo = true;

                if (cargaCancelamentoSolicitacao?.CargaCancelamento?.Carga?.Empresa?.EmissaoDocumentosForaDoSistema ?? false)
                    cargaCancelamentoSolicitacao.CargaCancelamento.CTe = null;

                repositorioCargaCancelamento.Atualizar(cargaCancelamentoSolicitacao.CargaCancelamento);
                return;
            }

            if (provisao == null || provisao.DocumentosProvisao.Any(d => d.Stage.Cancelado || d.Stage.Inconsistente))
            {
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repositorioCargaMDFe.BuscarMDFePorCarga(cargaCancelamentoSolicitacao.CargaCancelamento.Carga.Codigo);

                if (mdfe == null)
                    return;

                //cargaCancelamentoSolicitacao.CargaCancelamento.AguardandoConfirmacaoCancelamento = true;
            }
            else if (provisao != null && !provisao.DocumentosProvisao.Any(d => d.Stage.Cancelado || d.Stage.Inconsistente))
                throw new ControllerException("Não é possivel gerar cancelamentos por que possui FRS que não estão canceladas ou inconsistentes");

            repositorioCargaCancelamento.Atualizar(cargaCancelamentoSolicitacao.CargaCancelamento);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao cargaCancelamentoSolicitacao, Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: cargaCancelamentoSolicitacao.Codigo,
                URLPagina: "Cargas/CancelamentoCarga",
                titulo: Localization.Resources.Cargas.CargaCancelamentoAprovacao.CancelamentoCarga,
                nota: string.Format(Localization.Resources.Cargas.CargaCancelamentoAprovacao.CriadaSolicitacaoCancelamentoCarga, cargaCancelamentoSolicitacao.CargaCancelamento.Carga.CodigoCargaEmbarcador),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            if (!configuracaoEmbarcador.NaoPermitirCancelarCargaComInicioViagem ||
                (cargaCancelamento.Carga.DataInicioViagem == null) ||
                (cargaCancelamento.Carga.DataInicioViagem == DateTime.MinValue) ||
                (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
            )
                if (!configuracaoGeralCarga.UtilizaRegrasDeAprovacaoParaCancelamentoDaCarga)
                    return;

            if (configuracaoGeralCarga.UtilizaRegrasDeAprovacaoParaCancelamentoDaCarga)
            {
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes = repositorioCargaCTe.BuscarCTesPorCarga(cargaCancelamento.Carga.Codigo);

                int documentosAutorizados = 0;

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe in CTes)
                    if (CTe.Status == "A")
                        documentosAutorizados++;

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> MDFes = repositorioCargaMDFe.BuscarMDFesPorCarga(cargaCancelamento.Carga.Codigo);

                foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe in MDFes)
                    if (MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                        documentosAutorizados++;

                if (documentosAutorizados == 0)
                    return;
            }

            Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao repositorioCargaCancelamentoSolicitacao = new Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao cargaCancelamentoSolicitacao = repositorioCargaCancelamentoSolicitacao.BuscarPendentePorCargaCancelamento(cargaCancelamento.Codigo);

            if (cargaCancelamentoSolicitacao != null)
                throw new ServicoException($"Já existe uma solicitação de cancelamento {cargaCancelamentoSolicitacao.Situacao.ObterDescricao().ToLower()}");

            cargaCancelamentoSolicitacao = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao()
            {
                CargaCancelamento = cargaCancelamento,
                Numero = repositorioCargaCancelamentoSolicitacao.BuscarProximoNumeroPorCargaCancelamento(cargaCancelamento.Codigo),
                Situacao = SituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao
            };

            repositorioCargaCancelamentoSolicitacao.Inserir(cargaCancelamentoSolicitacao);

            CriarAprovacao(cargaCancelamentoSolicitacao, tipoServicoMultisoftware);
        }

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao cargaCancelamentoSolicitacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao repositorioCargaCancelamentoSolicitacao = new Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento> regras = ObterRegrasAutorizacao(cargaCancelamentoSolicitacao);

            if (regras.Count > 0)
                CriarRegrasAprovacao(cargaCancelamentoSolicitacao, regras, tipoServicoMultisoftware);
            else
                cargaCancelamentoSolicitacao.Situacao = SituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao;

            repositorioCargaCancelamentoSolicitacao.Atualizar(cargaCancelamentoSolicitacao);

            if (cargaCancelamentoSolicitacao.Situacao != SituacaoCargaCancelamentoSolicitacao.Aprovada)
            {
                cargaCancelamentoSolicitacao.CargaCancelamento.Situacao = SituacaoCancelamentoCarga.AgAprovacaoSolicitacao;
                return;
            }

            // cargaCancelamentoSolicitacao.CargaCancelamento.AguardandoConfirmacaoCancelamento = false; 
            cargaCancelamentoSolicitacao.CargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
            ValidarAprovacao(cargaCancelamentoSolicitacao);
        }

        public void NotificarSituacaoAprovacaoAoOperadorCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao cargaCancelamentoSolicitacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                return;

            string acao = string.Empty;

            if (cargaCancelamentoSolicitacao.Situacao == SituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao)
                acao = Localization.Resources.Gerais.Geral.Criada;
            else if (cargaCancelamentoSolicitacao.Situacao == SituacaoCargaCancelamentoSolicitacao.Aprovada)
                acao = Localization.Resources.Gerais.Geral.Aprovada;
            else
                acao = Localization.Resources.Gerais.Geral.Rejeitada;

            new Carga(_unitOfWork).NotificarAlteracaoAoOperador(cargaCancelamentoSolicitacao.CargaCancelamento.Carga, string.Format(Localization.Resources.Cargas.Carga.AcaoSolicitacaoCancelamentoCarga, acao, cargaCancelamentoSolicitacao.CargaCancelamento.Carga.CodigoCargaEmbarcador), _unitOfWork, tipoServicoMultisoftware);
        }

        #endregion
    }
}
