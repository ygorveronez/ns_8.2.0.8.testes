using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.MontagemCarga
{
    public sealed class CarregamentoAprovacao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao,
        Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento,
        Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion

        #region Construtores

        public CarregamentoAprovacao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public CarregamentoAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao, List<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento repositorio = new Repositorio.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento aprovacao = new Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento()
                        {
                            OrigemAprovacao = carregamentoSolicitacao,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = DateTime.Now,
                            NumeroAprovadores = regra.NumeroAprovadores,
                            GuidCarregamento = Guid.NewGuid().ToString()
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(carregamentoSolicitacao, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento aprovacao = new Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento()
                    {
                        OrigemAprovacao = carregamentoSolicitacao,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = DateTime.Now
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            carregamentoSolicitacao.Situacao = existeRegraSemAprovacao ? SituacaoCarregamentoSolicitacao.AguardandoAprovacao : SituacaoCarregamentoSolicitacao.Aprovada;
        }

        private bool IsUtilizarAlcadaAprovacaoCarregamento()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            return configuracaoEmbarcador?.UtilizarAlcadaAprovacaoCarregamento ?? false;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento>(_unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento>();

            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguro = repositorioCarregamentoApolice.BuscarApolicesPorCarregamento(carregamentoSolicitacao.Carregamento.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Carga.CarregamentoDadosPesagem dadosPesagem = new MontagemCarga(_unitOfWork).ObterDadosPesagem(carregamentoSolicitacao.Carregamento);
            List<int> listaCodigoFilial = new List<int>();
            decimal diferencaValorApoliceTransportador = 0m;

            if (listaRegras.Exists(o => o.RegraPorFilial))
            {
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = carregamentoSolicitacao.Carregamento.Pedidos.Select(o => o.Pedido.Filial).Distinct().ToList();

                listaCodigoFilial.AddRange((from o in filiais select o.Codigo).ToList());
            }

            if (listaRegras.Exists(o => o.RegraPorDiferencaValorApoliceTransportador))
            {
                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguroTransportador = apolicesSeguro.Where(o => o.Responsavel == ResponsavelSeguro.Transportador && o.ValorLimiteApolice > 0m).ToList();

                if (apolicesSeguroTransportador.Count > 0)
                {
                    decimal valorMercadorias = carregamentoSolicitacao.Carregamento.Pedidos.Select(o => o.Pedido.ValorTotalNotasFiscais).Sum();

                    diferencaValorApoliceTransportador = (valorMercadorias - apolicesSeguroTransportador.Sum(o => o.ValorLimiteApolice));
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento regra in listaRegras)
            {
                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, listaCodigoFilial))
                    continue;

                if (regra.RegraPorModeloVeicularCarga && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(regra.AlcadasModeloVeicularCarga, carregamentoSolicitacao.Carregamento.ModeloVeicularCarga?.Codigo))
                    continue;

                if (regra.RegraPorTipoCarga && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(regra.AlcadasTipoCarga, carregamentoSolicitacao.Carregamento.TipoDeCarga?.Codigo))
                    continue;

                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, carregamentoSolicitacao.Carregamento.TipoOperacao?.Codigo))
                    continue;

                if (regra.RegraPorPercentualOcupacaoCubagem && (dadosPesagem.PercentualOcupacaoCubagem > 0m) && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoCubagem, decimal>(regra.AlcadasPercentualOcupacaoCubagem, dadosPesagem.PercentualOcupacaoCubagem))
                    continue;

                if (regra.RegraPorPercentualOcupacaoPallet && (dadosPesagem.PercentualOcupacaoPallet > 0m) && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoPallet, decimal>(regra.AlcadasPercentualOcupacaoPallet, dadosPesagem.PercentualOcupacaoPallet))
                    continue;

                if (regra.RegraPorPercentualOcupacaoPeso && (dadosPesagem.PercentualOcupacaoPeso > 0m) && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaPercentualOcupacaoPeso, decimal>(regra.AlcadasPercentualOcupacaoPeso, dadosPesagem.PercentualOcupacaoPeso))
                    continue;

                if (regra.RegraPorDiferencaValorApoliceTransportador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AlcadaDiferencaValorApoliceTransportador, decimal>(regra.AlcadasDiferencaValorApoliceTransportador, diferencaValorApoliceTransportador))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao, Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            System.Text.StringBuilder st = new System.Text.StringBuilder();
            st.Append(string.Format(Localization.Resources.Cargas.CarregamentoAprovacao.CarregamentoAguardandoAprovacaoGerarCarga, carregamentoSolicitacao.Carregamento.NumeroCarregamento));

            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (aprovacao.RegraAutorizacao.EnviarLinkParaAprovacaoPorEmail)
                st.AppendLine(string.Format(Localization.Resources.Cargas.CarregamentoAprovacao.LinkAcessoVerificarAutorizacaoOcorrencia, $"https://{_configuracaoEmbarcador.LinkUrlAcessoCliente}/aprovacao-carregamento/{aprovacao.GuidCarregamento}"));

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: carregamentoSolicitacao.Codigo,
                URLPagina: "Cargas/MontagemCarga",
                titulo: Localization.Resources.Cargas.CarregamentoAprovacao.Carregamento,
                nota: st.ToString(),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao = repositorioCarregamentoSolicitacao.BuscarPendentePorCarregamento(carregamento.Codigo);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (carregamentoSolicitacao != null)
                throw new ServicoException($"Já existe uma solicitação de {(configuracaoEmbarcador.OcultaGerarCarregamentosMontagemCarga ? "agendamento" : "geração de carga")} {carregamentoSolicitacao.Situacao.ObterDescricao().ToLower()}");

            carregamentoSolicitacao = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao()
            {
                Carregamento = carregamento,
                Numero = repositorioCarregamentoSolicitacao.BuscarProximoNumeroPorCarregamento(carregamento.Codigo),
                Situacao = SituacaoCarregamentoSolicitacao.AguardandoAprovacao
            };

            repositorioCarregamentoSolicitacao.Inserir(carregamentoSolicitacao);

            CriarAprovacao(carregamentoSolicitacao, tipoServicoMultisoftware);
        }

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento> regras = ObterRegrasAutorizacao(carregamentoSolicitacao);

            if (regras.Count > 0)
                CriarRegrasAprovacao(carregamentoSolicitacao, regras, tipoServicoMultisoftware);
            else
                carregamentoSolicitacao.Situacao = SituacaoCarregamentoSolicitacao.SemRegraAprovacao;

            repositorioCarregamentoSolicitacao.Atualizar(carregamentoSolicitacao);

            if (carregamentoSolicitacao.Situacao == SituacaoCarregamentoSolicitacao.Aprovada)
                carregamentoSolicitacao.Carregamento.SituacaoCarregamento = SituacaoCarregamento.EmMontagem;
            else
                carregamentoSolicitacao.Carregamento.SituacaoCarregamento = SituacaoCarregamento.AguardandoAprovacaoSolicitacao;
        }

        public bool IsCriarAprovacaoCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento)
        {
            if (!IsUtilizarAlcadaAprovacaoCarregamento())
                return false;

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao repositorioCarregamentoSolicitacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao carregamentoSolicitacao = repositorioCarregamentoSolicitacao.BuscarUltimaPorCarregamento(carregamento.Codigo);

            return (carregamentoSolicitacao == null) || (carregamentoSolicitacao.Situacao == SituacaoCarregamentoSolicitacao.Reprovada);
        }

        #endregion
    }
}
