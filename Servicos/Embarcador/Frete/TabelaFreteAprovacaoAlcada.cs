using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public sealed class TabelaFreteAprovacaoAlcada : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao,
        Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete,
        Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao _configuracaoAprovacao;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly bool _notificarUsuario;
        private SituacaoItemParametroBaseCalculoTabelaFrete? _situacaoItemAprovado;

        #endregion

        #region Construtores

        public TabelaFreteAprovacaoAlcada(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, notificarUsuario: true) { }

        public TabelaFreteAprovacaoAlcada(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, notificarUsuario: true) { }

        public TabelaFreteAprovacaoAlcada(Repositorio.UnitOfWork unitOfWork, bool notificarUsuario) : this(unitOfWork, configuracaoEmbarcador: null, notificarUsuario) { }

        public TabelaFreteAprovacaoAlcada(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, bool notificarUsuario) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _notificarUsuario = notificarUsuario;
        }

        #endregion

        #region Métodos Privados

        private void AdicionarOuAtualizarTabelaFreteClienteAlteracao(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao repositorioTabelaFreteClienteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao tabelaFreteClienteAlteracao = repositorioTabelaFreteClienteAlteracao.BuscarPorAlteracao(tabelaFreteAlteracao.Codigo, tabelaFreteCliente.Codigo);

            if (tabelaFreteClienteAlteracao == null)
            {
                tabelaFreteClienteAlteracao = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaFreteAlteracao = tabelaFreteAlteracao,
                    TabelaFreteCliente = tabelaFreteCliente,
                    Usuario = usuario
                };

                repositorioTabelaFreteClienteAlteracao.Inserir(tabelaFreteClienteAlteracao);
            }
            else
            {
                tabelaFreteClienteAlteracao.DataAlteracao = DateTime.Now;
                tabelaFreteClienteAlteracao.Usuario = usuario;

                repositorioTabelaFreteClienteAlteracao.Atualizar(tabelaFreteClienteAlteracao);
            }
        }

        private Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao AdicionarTabelaFreteAlteracao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao = new Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao()
            {
                Numero = repositorioTabelaFreteAlteracao.BuscarProximoNumeroPorTabelaFrete(tabelaFrete.Codigo),
                SituacaoAlteracao = SituacaoAlteracaoTabelaFrete.AguardandoAprovacao,
                TabelaFrete = tabelaFrete
            };

            repositorioTabelaFreteAlteracao.Inserir(tabelaFreteAlteracao);

            return tabelaFreteAlteracao;
        }

        private void AtualizarSituacaoTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, SituacaoAlteracaoTabelaFrete novaSituacao)
        {
            if (ObterConfiguracaoAprovacao().UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente)
            {
                tabelaFrete.SituacaoAlteracao = SituacaoAlteracaoTabelaFrete.NaoInformada;
                return;
            }

            tabelaFrete.SituacaoAlteracao = novaSituacao;
        }

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao, List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            if (regras.Count == 0)
            {
                tabelaFreteAlteracao.SituacaoAlteracao = SituacaoAlteracaoTabelaFrete.SemRegraAprovacao;
                return;
            }

            Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete repositorio = new Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete(_unitOfWork);
            List<Dominio.Entidades.Usuario> aprovadoresPorTransportador;
            bool existeRegraSemAprovacao = false;
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            if (regras.Any(regra => regra.TipoAprovadorRegra == TipoAprovadorRegra.Transportador))
                aprovadoresPorTransportador = new Repositorio.Usuario(_unitOfWork).BuscarUsuariosAcessoTransportador(tabelaFreteCliente?.Empresa?.Codigo ?? 0);
            else
                aprovadoresPorTransportador = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete regra in regras)
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
                        Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete aprovacao = new Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete()
                        {
                            OrigemAprovacao = tabelaFreteAlteracao,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = DateTime.Now,
                            NumeroAprovadores = regra.NumeroAprovadores,
                            TipoAprovadorRegra = regra.TipoAprovadorRegra
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(tabelaFreteAlteracao, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete aprovacao = new Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete()
                    {
                        OrigemAprovacao = tabelaFreteAlteracao,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = DateTime.Now,
                        TipoAprovadorRegra = regra.TipoAprovadorRegra
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            tabelaFreteAlteracao.SituacaoAlteracao = existeRegraSemAprovacao ? SituacaoAlteracaoTabelaFrete.AguardandoAprovacao : SituacaoAlteracaoTabelaFrete.Aprovada;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao ObterConfiguracaoAprovacao()
        {
            if (_configuracaoAprovacao == null)
                _configuracaoAprovacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAprovacao(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoAprovacao;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete>(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.RegraAutorizacaoTabelaFrete repRegraAutorizacaoTabelaFrete = new Repositorio.Embarcador.Frete.RegraAutorizacaoTabelaFrete(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> listaRegras = repRegraAutorizacaoTabelaFrete.ObterAtivas(tabelaFrete.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete>();

            List<int> listaCodigoDestino = repositorioTabelaFreteCliente.BuscarDestinoTabelasPorTabelaFreteAlteracao(tabelaFrete.Codigo, tabelaFreteCliente?.Codigo ?? 0);
            List<int> listaCodigoFilial = repositorioTabelaFrete.BuscarFiliaisPorTabelaFrete(tabelaFrete.Codigo);
            List<int> listaCodigoOrigem = repositorioTabelaFreteCliente.BuscarOrigemTabelasPorTabelaFreteAlteracao(tabelaFrete.Codigo, tabelaFreteCliente?.Codigo ?? 0);
            List<int> listaCodigoTipoOperacao = repositorioTabelaFrete.BuscarTiposOperacaoPorTabelaFrete(tabelaFrete.Codigo);
            List<int> listaCodigoTransportador = repositorioTabelaFrete.BuscarTransportadoresPorTabelaFrete(tabelaFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete regra in listaRegras)
            {
                if (regra.RegraPorDestino)
                {
                    if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaDestino, Dominio.Entidades.Localidade>(regra.AlcadasDestino, listaCodigoDestino))
                        continue;
                }

                if (regra.RegraPorFilial)
                {
                    if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, listaCodigoFilial))
                        continue;
                }

                if (regra.RegraPorOrigem)
                {
                    if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaOrigem, Dominio.Entidades.Localidade>(regra.AlcadasOrigem, listaCodigoOrigem))
                        continue;
                }

                if (regra.RegraPorTipoOperacao)
                {
                    if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, listaCodigoTipoOperacao))
                        continue;
                }

                if (regra.RegraPorTransportador)
                {
                    if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, listaCodigoTransportador))
                        continue;
                }

                listaRegrasFiltradas.Add(regra);
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && listaRegrasFiltradas.Count <= 0)
            {
                //para o TMS, caso não tenha regra por algum dos parâmetros acima, utiliza apenas a tabela de frete
                List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> regrasPorTabelaFrete = repRegraAutorizacaoTabelaFrete.ObterAtivasPorTabelaFrete(tabelaFrete.Codigo);

                listaRegrasFiltradas = regrasPorTabelaFrete.Where(o => !o.IsAlcadaAtiva()).ToList();

                if (listaRegrasFiltradas.Count == 0)
                {
                    regrasPorTabelaFrete = repRegraAutorizacaoTabelaFrete.ObterAtivasPorTabelaFrete(0);

                    listaRegrasFiltradas = regrasPorTabelaFrete.Where(o => !o.IsAlcadaAtiva()).ToList();
                }
            }

            return listaRegrasFiltradas;
        }

        private SituacaoItemParametroBaseCalculoTabelaFrete ObterSituacaoItemAprovado()
        {
            if (!_situacaoItemAprovado.HasValue)
            {
                if (new TabelaFreteClienteIntegracao(_unitOfWork).PossuiIntegracaoControlaSituacaoItens())
                    _situacaoItemAprovado = SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoIntegracao;
                else
                    _situacaoItemAprovado = SituacaoItemParametroBaseCalculoTabelaFrete.Ativo;
            }

            return _situacaoItemAprovado.Value;
        }

        private Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao ObterTabelaFreteAlteracao(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = ObterConfiguracaoAprovacao();

            if (configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente)
                return repositorioTabelaFreteAlteracao.BuscarPendentePorTabelaFreteCliente(tabelaFreteCliente.Codigo);

            return repositorioTabelaFreteAlteracao.BuscarPendentePorTabelaFrete(tabelaFreteCliente.TabelaFrete.Codigo);
        }

        private void RemoverAprovacaoTabelaFreteAlteracao(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao)
        {
            if (tabelaFreteAlteracao != null)
                RemoverAprovacao(tabelaFreteAlteracao);
        }

        private void RemoverTabelaFreteAlteracao(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao)
        {
            if (tabelaFreteAlteracao == null)
                return;

            Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao repositorioTabelaFreteClienteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao(_unitOfWork);

            repositorioTabelaFreteClienteAlteracao.DeletarPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo);
            repositorioTabelaFreteAlteracao.Deletar(tabelaFreteAlteracao);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao, Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!_notificarUsuario && !(aprovacao.RegraAutorizacao?.EnviarLinkParaAprovacaoPorEmail ?? false))
                return;

            System.Text.StringBuilder st = new System.Text.StringBuilder();
            st.Append(string.Format(Localization.Resources.Fretes.TabelaFrete.AlteracaoValoresTabelaFrete, tabelaFreteAlteracao.TabelaFrete.Descricao));

            bool enviarLinkAprovacao = false;
            if (aprovacao.RegraAutorizacao.EnviarLinkParaAprovacaoPorEmail)
            {
                enviarLinkAprovacao = true;
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = ObterConfiguracaoEmbarcador();
                if (!String.IsNullOrWhiteSpace(configuracaoTMS.LinkUrlAcessoCliente))
                    st.AppendLine(string.Format($", {Localization.Resources.Fretes.TabelaFrete.LinkAcessoVerificarAlteracaoTabelaFrete}", $"https://{configuracaoTMS.LinkUrlAcessoCliente}/#Fretes/AutorizacaoTabelaFrete"));
            }

            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: tabelaFreteAlteracao.Codigo,
                URLPagina: "Fretes/TabelaFrete",
                titulo: enviarLinkAprovacao ? Localization.Resources.Fretes.TabelaFrete.TabelaFretePendenteAprovacao : Localization.Resources.Fretes.TabelaFrete.ValoresDaTabelaDeFrete,
                nota: st.ToString(),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void AtualizarAprovacao(int codigoTabelaFrete, List<int> codigosTabelaFreteCliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

            AtualizarAprovacao(tabelaFrete, codigosTabelaFreteCliente, tipoServicoMultisoftware);
        }

        public void AtualizarAprovacao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, List<int> codigosTabelaFreteCliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao repositorioTabelaFreteClienteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarPendentePorTabelaFrete(tabelaFrete.Codigo);

            RemoverAprovacaoTabelaFreteAlteracao(tabelaFreteAlteracao);

            if (IsUtilizarAlcadaTabelaFrete())
            {
                if (tabelaFreteAlteracao == null)
                    tabelaFreteAlteracao = AdicionarTabelaFreteAlteracao(tabelaFrete);

                repositorioTabelaFreteClienteAlteracao.InserirAlteracoes(tabelaFreteAlteracao.Codigo, codigosTabelaFreteCliente);

                List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> regras = ObterRegrasAutorizacao(tabelaFrete, tabelaFreteCliente: null, tipoServicoMultisoftware);

                CriarRegrasAprovacao(tabelaFreteAlteracao, regras, tipoServicoMultisoftware, tabelaFreteCliente: null);

                if (tabelaFreteAlteracao.SituacaoAlteracao == SituacaoAlteracaoTabelaFrete.Aprovada)
                    repositorioTabelaFreteCliente.LiberarAlteracaoPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo, ObterSituacaoItemAprovado());
                else
                    repositorioTabelaFreteCliente.BloquearAlteracaoPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo, tabelaFreteAlteracao.SituacaoAlteracao);

                repositorioTabelaFreteAlteracao.Atualizar(tabelaFreteAlteracao);
                AtualizarSituacaoTabelaFrete(tabelaFrete, tabelaFreteAlteracao.SituacaoAlteracao);

                if (tabelaFreteAlteracao.SituacaoAlteracao == SituacaoAlteracaoTabelaFrete.Aprovada)
                    new TabelaFreteClienteIntegracao(_unitOfWork).AdicionarIntegracoes(tabelaFreteAlteracao);
            }
            else
            {
                RemoverTabelaFreteAlteracao(tabelaFreteAlteracao);
                repositorioTabelaFreteCliente.LiberarAlteracaoPorTabelaFrete(tabelaFrete.Codigo, ObterSituacaoItemAprovado());
                AtualizarSituacaoTabelaFrete(tabelaFrete, SituacaoAlteracaoTabelaFrete.NaoInformada);
            }

            repositorioTabelaFrete.Atualizar(tabelaFrete);
        }

        public void AtualizarAprovacao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> tabelasFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarPendentesPorTabelaFrete(tabelaFrete.Codigo);
            SituacaoItemParametroBaseCalculoTabelaFrete situacaoItemAprovado = ObterSituacaoItemAprovado();

            if (IsUtilizarAlcadaTabelaFrete())
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAprovacao configuracaoAprovacao = ObterConfiguracaoAprovacao();

                if (configuracaoAprovacao.UtilizarAlcadaAprovacaoTabelaFretePorTabelaFreteCliente)
                    return;

                if (tabelasFreteAlteracao.Count == 0)
                    tabelasFreteAlteracao.Add(AdicionarTabelaFreteAlteracao(tabelaFrete));

                List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> regras = ObterRegrasAutorizacao(tabelaFrete, tabelaFreteCliente: null, tipoServicoMultisoftware);

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao in tabelasFreteAlteracao)
                {
                    RemoverAprovacaoTabelaFreteAlteracao(tabelaFreteAlteracao);
                    CriarRegrasAprovacao(tabelaFreteAlteracao, regras, tipoServicoMultisoftware, tabelaFreteCliente: null);

                    repositorioTabelaFreteAlteracao.Atualizar(tabelaFreteAlteracao);

                    if (tabelaFreteAlteracao.SituacaoAlteracao == SituacaoAlteracaoTabelaFrete.Aprovada)
                        repositorioTabelaFreteCliente.LiberarAlteracaoPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo, situacaoItemAprovado);
                    else
                        repositorioTabelaFreteCliente.BloquearAlteracaoPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo, tabelaFreteAlteracao.SituacaoAlteracao);
                }

                AtualizarSituacaoTabelaFrete(tabelaFrete, tabelasFreteAlteracao.First().SituacaoAlteracao);
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao in tabelasFreteAlteracao)
                {
                    RemoverAprovacaoTabelaFreteAlteracao(tabelaFreteAlteracao);
                    RemoverTabelaFreteAlteracao(tabelaFreteAlteracao);
                }

                repositorioTabelaFreteCliente.LiberarAlteracaoPorTabelaFrete(tabelaFrete.Codigo, situacaoItemAprovado);
                AtualizarSituacaoTabelaFrete(tabelaFrete, SituacaoAlteracaoTabelaFrete.NaoInformada);
            }

            repositorioTabelaFrete.Atualizar(tabelaFrete);
        }

        public void AtualizarAprovacao(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(tabelaFreteCliente.TabelaFrete.Codigo);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao = ObterTabelaFreteAlteracao(tabelaFreteCliente);

            SituacaoAlteracaoTabelaFrete situacaoAlteracao = SituacaoAlteracaoTabelaFrete.NaoInformada;

            RemoverAprovacaoTabelaFreteAlteracao(tabelaFreteAlteracao);

            if (IsUtilizarAlcadaTabelaFrete())
            {
                if (tabelaFreteAlteracao == null)
                    tabelaFreteAlteracao = AdicionarTabelaFreteAlteracao(tabelaFreteCliente.TabelaFrete);

                AdicionarOuAtualizarTabelaFreteClienteAlteracao(tabelaFreteAlteracao, tabelaFreteCliente, usuario);

                List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.RegraAutorizacaoTabelaFrete> regras = ObterRegrasAutorizacao(tabelaFrete, tabelaFreteCliente, tipoServicoMultisoftware);

                CriarRegrasAprovacao(tabelaFreteAlteracao, regras, tipoServicoMultisoftware, tabelaFreteCliente);

                repositorioTabelaFreteAlteracao.Atualizar(tabelaFreteAlteracao);

                situacaoAlteracao = tabelaFreteAlteracao.SituacaoAlteracao;
            }
            else
                RemoverTabelaFreteAlteracao(tabelaFreteAlteracao);

            if (situacaoAlteracao == SituacaoAlteracaoTabelaFrete.Aprovada)
            {
                tabelaFreteCliente.Tipo = TipoTabelaFreteCliente.Calculo;
                repositorioTabelaFreteCliente.LiberarAlteracaoPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo, ObterSituacaoItemAprovado());
            }
            else if (situacaoAlteracao == SituacaoAlteracaoTabelaFrete.NaoInformada)
            {
                tabelaFreteCliente.Tipo = TipoTabelaFreteCliente.Calculo;
                repositorioTabelaFreteCliente.LiberarAlteracaoPorTabelaFrete(tabelaFrete.Codigo, ObterSituacaoItemAprovado());
            }
            else
            {
                tabelaFreteCliente.Tipo = TipoTabelaFreteCliente.Alteracao;
                repositorioTabelaFreteCliente.AtualizarSituacaoAlteracaoPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo, situacaoAlteracao);
            }

            tabelaFreteCliente.SituacaoAlteracao = situacaoAlteracao;

            AtualizarSituacaoTabelaFrete(tabelaFrete, situacaoAlteracao);
            repositorioTabelaFrete.Atualizar(tabelaFrete);
            repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente);

            if (situacaoAlteracao == SituacaoAlteracaoTabelaFrete.Aprovada)
                new TabelaFreteClienteIntegracao(_unitOfWork).AdicionarIntegracoes(tabelaFreteAlteracao);
        }

        public bool IsUtilizarAlcadaTabelaFrete()
        {
            return ObterConfiguracaoEmbarcador()?.UtilizarAlcadaAprovacaoTabelaFrete ?? false;
        }

        public void LiberarAlteracaoTabelaFreteCliente(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);

            AtualizarSituacaoTabelaFrete(tabelaFreteAlteracao.TabelaFrete, SituacaoAlteracaoTabelaFrete.Aprovada);
            repositorioTabelaFrete.Atualizar(tabelaFreteAlteracao.TabelaFrete);
            repositorioTabelaFreteCliente.LiberarAlteracaoPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo, ObterSituacaoItemAprovado());
            new TabelaFreteClienteIntegracao(_unitOfWork).AdicionarIntegracoes(tabelaFreteAlteracao);
        }

        public void ReprovarAlteracaoTabelaFreteCliente(Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);

            AtualizarSituacaoTabelaFrete(tabelaFreteAlteracao.TabelaFrete, SituacaoAlteracaoTabelaFrete.Reprovada);
            repositorioTabelaFrete.Atualizar(tabelaFreteAlteracao.TabelaFrete);
            repositorioTabelaFreteCliente.AtualizarSituacaoAlteracaoPorTabelaFreteAlteracao(tabelaFreteAlteracao.Codigo, SituacaoAlteracaoTabelaFrete.Reprovada);
        }

        #endregion
    }
}
