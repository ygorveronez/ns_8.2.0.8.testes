using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public sealed class CargaAprovacaoFrete : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Cargas.Carga,
        Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga,
        Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga
    >
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion

        #region Construtores

        public CargaAprovacaoFrete(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public CargaAprovacaoFrete(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : base(unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga repositorio = new Repositorio.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga(_unitOfWork);
            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            foreach (Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga aprovacao = new Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga()
                        {
                            OrigemAprovacao = carga,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = carga.DataCriacaoCarga,
                            NumeroAprovadores = regra.NumeroAprovadores,
                            GuidCarga = Guid.NewGuid().ToString()
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(carga, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga aprovacao = new Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga()
                    {
                        OrigemAprovacao = carga,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = carga.DataCriacaoCarga,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            if (existeRegraSemAprovacao)
            {
                carga.SituacaoAlteracaoFreteCarga = SituacaoAlteracaoFreteCarga.AguardandoAprovacao;

                NotificarSituacaoAprovacaoAoOperadorCarga(carga, tipoServicoMultisoftware);
            }
            else
                carga.SituacaoAlteracaoFreteCarga = SituacaoAlteracaoFreteCarga.Aprovada;
        }

        private bool IsCriarRegrasAprovacao(TipoRegraAutorizacaoCarga tipoRegra, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (tipoRegra == TipoRegraAutorizacaoCarga.InformadoManualmente && !configuracaoEmbarcador.UtilizarAlcadaAprovacaoAlteracaoValorFrete)// João, não pode alterar o que está funcionando, se tem algo deve ser criado uma nova regra && !(carga.TipoOperacao?.ConfiguracaoCalculoFrete?.UtilizarContratoFreteCliente ?? false))
                return false;

            if (tipoRegra == TipoRegraAutorizacaoCarga.TabelaFrete && !configuracaoEmbarcador.UtilizarAlcadaAprovacaoValorTabelaFreteCarga)
                return false;

            if (tipoRegra == TipoRegraAutorizacaoCarga.Outros && !configuracaoEmbarcador.UtilizaEmissaoMultimodal && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !configuracaoEmbarcador.UtilizarAlcadaAprovacaoAlteracaoValorFrete)
                return false;

            if (!configuracaoEmbarcador.UtilizarAlcadaAprovacaoAlteracaoValorFrete && !configuracaoEmbarcador.UtilizarAlcadaAprovacaoValorTabelaFreteCarga)
                return false;

            return true;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                _configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            }

            return _configuracaoEmbarcador;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoRegraAutorizacaoCarga tipoRegra, decimal novoValorFrete)
        {
            Repositorio.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga repositorioRegraAutorizacaoCarga = new Repositorio.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repositorioContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Servicos.Embarcador.Carga.Frete servicoFrete = new Servicos.Embarcador.Carga.Frete(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repositorioContratoFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga> listaRegras = repositorioRegraAutorizacaoCarga.BuscarAtivas(tipoRegra);
            List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga>();
            bool freteCalculadoPorFilialEmissora = (carga.EmpresaFilialEmissora != null && (configuracaoEmbarcador.CalcularFreteFilialEmissoraPorTabelaDeFrete || (carga.TipoOperacao?.CalculaFretePorTabelaFreteFilialEmissora ?? false)));
            decimal diferencaPesoMaximoContainer = 0;
            decimal valorFrete = freteCalculadoPorFilialEmissora ? carga.ValorFreteAPagarFilialEmissora : carga.ValorFreteAPagar;
            decimal valorTabelaFrete = freteCalculadoPorFilialEmissora ? carga.ValorFreteTabelaFreteFilialEmissora : carga.ValorFreteTabelaFrete;
            decimal percentualDiferencaValorTabelaFrete = 0m;
            decimal diferencaValorFrete = 0m;

            if ((valorTabelaFrete > 0m) && (valorFrete > 0m))
                percentualDiferencaValorTabelaFrete = ((valorFrete - valorTabelaFrete) * 100) / valorTabelaFrete;

            if (novoValorFrete > 0)
                diferencaValorFrete = novoValorFrete - carga.ValorFreteNegociado;
            //diferencaValorFrete = (carga.ValorFreteNegociado - novoValorFrete) > 0 ? carga.ValorFreteNegociado - novoValorFrete : novoValorFrete - carga.ValorFreteNegociado;

            if (carga != null && carga.Pedidos != null)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in carga.Pedidos)
                {
                    if (diferencaPesoMaximoContainer >= 0 && pedido.Pedido != null && pedido.Pedido.Container != null && pedido.Pedido.Container.ContainerTipo != null && pedido.Pedido.Container.ContainerTipo.PesoMaximo > 0 && pedido.NotasFiscais != null && pedido.NotasFiscais.Count > 0)
                    {
                        decimal pesoInformadoDasNotas = pedido.NotasFiscais.Sum(n => n.XMLNotaFiscal.Peso);
                        if (pesoInformadoDasNotas > 0)
                        {
                            diferencaPesoMaximoContainer = pesoInformadoDasNotas - pedido.Pedido.Container.ContainerTipo.PesoMaximo;
                        }
                    }
                }
            }
            if (diferencaPesoMaximoContainer < 0)
                diferencaPesoMaximoContainer = 0;

            decimal valorFretePercentualSobreNotaPedido = servicoFrete.CalcularPercentualFreteSobreNotaPedido(carga);

            foreach (Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga regra in listaRegras)
            {
                if (regra.RegraPorComponenteFrete)
                {
                    List<int> listaCodigoComponente = (from componente in carga.Componentes where componente.ComponenteFrete != null select componente.ComponenteFrete.Codigo).ToList();

                    if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaComponenteFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete>(regra.AlcadasComponenteFrete, listaCodigoComponente))
                        continue;
                }

                if (regra.RegraPorAutorizacaoTipoTerceiro
                    && carga.FreteDeTerceiro && contratoFrete != null
                    && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaAutorizacaoTipoTerceiro, Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>(regra.AlcadasAutorizacaoTipoTerceiro, contratoFrete.TipoTerceiro?.Codigo))
                    continue;

                int codigoFilial = carga.Filial?.Codigo ?? 0;
                if (carga.FilialOrigem != null)
                    codigoFilial = carga.FilialOrigem.Codigo;

                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, codigoFilial))
                    continue;

                if (regra.RegraPorModeloVeicularCarga && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaModeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>(regra.AlcadasModeloVeicularCarga, carga.ModeloVeicularCarga?.Codigo))
                    continue;

                if (regra.RegraPorTipoCarga && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTipoCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(regra.AlcadasTipoCarga, carga.TipoDeCarga?.Codigo))
                    continue;

                if (regra.RegraPorTomador && carga.Pedidos != null && carga.Pedidos.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in carga.Pedidos)
                    {
                        if (regra.RegraPorTomador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTomador, Dominio.Entidades.Cliente>(regra.AlcadasTomador, pedido?.ObterTomador()?.CPF_CNPJ))
                            continue;
                    }
                }

                if (regra.RegraPorPortoDestino && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPortoDestino, Dominio.Entidades.Embarcador.Pedidos.Porto>(regra.AlcadasPortoDestino, carga.PortoDestino?.Codigo))
                    continue;

                if (regra.RegraPorPortoOrigem && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPortoOrigem, Dominio.Entidades.Embarcador.Pedidos.Porto>(regra.AlcadasPortoOrigem, carga.PortoOrigem?.Codigo))
                    continue;

                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, carga.TipoOperacao?.Codigo))
                    continue;

                if (regra.RegraPorValorFrete && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaValorFrete, decimal>(regra.AlcadasValorFrete, valorFrete))
                    continue;

                if (regra.RegraPorPesoContainer && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPesoContainer, decimal>(regra.AlcadasPesoContainer, diferencaPesoMaximoContainer))
                    continue;

                if (regra.RegraPorMotivoSolicitacaoFrete && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaMotivoSolicitacaoFrete, Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete>(regra.AlcadasMotivoSolicitacaoFrete, carga.MotivoSolicitacaoFrete?.Codigo))
                    continue;

                if (regra.RegraPorPercentualAcrescimoValorTabelaFrete && regra.RegraPorPercentualDescontoValorTabelaFrete)
                {
                    if (percentualDiferencaValorTabelaFrete == 0m)
                        continue;

                    if ((percentualDiferencaValorTabelaFrete > 0m) && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualAcrescimoValorTabelaFrete, decimal>(regra.AlcadasPercentualAcrescimoValorTabelaFrete, percentualDiferencaValorTabelaFrete))
                        continue;

                    if ((percentualDiferencaValorTabelaFrete < 0m) && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDescontoValorTabelaFrete, decimal>(regra.AlcadasPercentualDescontoValorTabelaFrete, (percentualDiferencaValorTabelaFrete * -1)))
                        continue;
                }
                else if (regra.RegraPorPercentualAcrescimoValorTabelaFrete)
                {
                    if ((percentualDiferencaValorTabelaFrete <= 0m) || !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualAcrescimoValorTabelaFrete, decimal>(regra.AlcadasPercentualAcrescimoValorTabelaFrete, percentualDiferencaValorTabelaFrete))
                        continue;
                }
                else if (regra.RegraPorPercentualDescontoValorTabelaFrete)
                {
                    if ((percentualDiferencaValorTabelaFrete >= 0m) || !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDescontoValorTabelaFrete, decimal>(regra.AlcadasPercentualDescontoValorTabelaFrete, (percentualDiferencaValorTabelaFrete * -1)))
                        continue;
                }

                if (regra.RegraPorPercentualDiferencaFreteLiquidoFreteTerceiro)
                    if (!carga.FreteDeTerceiro
                        || (contratoFrete != null && regra.RegraPorPercentualDiferencaFreteLiquidoFreteTerceiro && contratoFrete.ValorFreteSubcontratacao > 0
                            && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoFreteTerceiro, decimal>(regra.AlcadasPercentualDiferencaFreteLiquidoFreteTerceiro, ((valorFrete - contratoFrete.ValorFreteSubcontratacao) * 100) / contratoFrete.ValorFreteSubcontratacao)))
                        continue;

                if (regra.RegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro)
                    if (!carga.FreteDeTerceiro
                        || (contratoFrete != null && regra.RegraPorPercentualDiferencaFreteLiquidoTotalFreteTerceiro && contratoFrete.ValorFreteSubcontratacao > 0
                            && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualDiferencaFreteLiquidoTotalFreteTerceiro, decimal>(regra.AlcadasPercentualDiferencaFreteLiquidoTotalFreteTerceiro, (((valorFrete - carga?.ValorICMS ?? 0) - contratoFrete.ValorFreteSubcontratacao) * 100) / contratoFrete.ValorFreteSubcontratacao)))
                        continue;

                if (regra.RegraPorComponenteFrete && regra.ValidarAcrescimoValorTabelaFretePorComponenteFrete && regra.RegraPorValorAcrescimoValorTabelaFrete)
                {
                    if (!RegraPorValorAcrescimoValorTabelaFrete(regra, carga.Componentes))
                        continue;
                }
                else if (regra.RegraPorValorAcrescimoValorTabelaFrete && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaValorAcrescimoValorTabelaFrete, decimal>(regra.AlcadasValorAcrescimoValorTabelaFrete, (valorFrete - carga?.ValorICMS ?? 0) - valorTabelaFrete))
                    continue;

                if (regra.RegraPorPercentualFreteSobreNota && !ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaPercentualFreteSobreNota, decimal>(regra.AlcadasPercentualFreteSobreNota, (valorFretePercentualSobreNotaPedido)))
                    continue;

                if (regra.RegraPorDiferencaValorFrete && diferencaValorFrete != 0)
                {
                    if (diferencaValorFrete < 0)
                    {
                        decimal valorCalculado = diferencaValorFrete * -1;
                        if (!regra.AlcadasDiferencaValorFrete.Any(c => c.Condicao == CondicaoAutorizao.MenorIgualQue || c.Condicao == CondicaoAutorizao.MenorQue))
                            continue;
                        else if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaDiferencaValorFrete, decimal>(regra.AlcadasDiferencaValorFrete, valorCalculado))
                                continue;
                    }
                    else
                    {
                        if (!regra.AlcadasDiferencaValorFrete.Any(c => c.Condicao == CondicaoAutorizao.MaiorQue || c.Condicao == CondicaoAutorizao.MaiorIgualQue))
                            continue;
                        else if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaDiferencaValorFrete, decimal>(regra.AlcadasDiferencaValorFrete, diferencaValorFrete))
                                continue;
                    }
                }

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private bool RegraPorValorAcrescimoValorTabelaFrete(Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga regra, IList<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentes)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesValidacaoAcrescimo = cargaComponentes
                        .Where(cargaComponente => cargaComponente.CargaComplementoFrete != null && cargaComponente.CargaComplementoFrete.SituacaoComplementoFrete == SituacaoComplementoFrete.Utilizada)
                        .ToList();

            List<int> codigosComponentes = cargaComponentesValidacaoAcrescimo
                .Select(cargaComponente => cargaComponente.ComponenteFrete.Codigo)
                .ToList();

            if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaComponenteFrete, Dominio.Entidades.Embarcador.Frete.ComponenteFrete>(regra.AlcadasComponenteFrete, codigosComponentes))
                return false;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componenteValidacaoAcrescimo in cargaComponentesValidacaoAcrescimo)
            {
                if (!ValidarAlcadas<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaValorAcrescimoValorTabelaFrete, decimal>(regra.AlcadasValorAcrescimoValorTabelaFrete, componenteValidacaoAcrescimo.ValorComponente))
                    continue;

                return true;
            }

            return false;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            System.Text.StringBuilder st = new System.Text.StringBuilder();
            st.Append(string.Format(Localization.Resources.Cargas.CargaAprovacaoFrete.CriadaSolicitacaoAlteracaoFreteCarga, carga.CodigoCargaEmbarcador)).AppendLine("");

            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (aprovacao.RegraAutorizacao.EnviarLinkParaAprovacaoPorEmail)
                st.AppendLine(string.Format(Localization.Resources.Cargas.CargaAprovacaoFrete.LinkVerificarAutorizacaoOcorrencia, $"https://{_configuracaoEmbarcador.LinkUrlAcessoCliente}/aprovacao-carga/{aprovacao.GuidCarga}"));

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: carga.Codigo,
                URLPagina: "Cargas/AutorizacaoCarga",
                titulo: Localization.Resources.Cargas.CargaAprovacaoFrete.FreteCarga,
                nota: st.ToString(),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoRegraAutorizacaoCarga tipoRegra, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, decimal novoValorFrete = 0m)
        {
            RemoverAprovacao(carga);

            if (!IsCriarRegrasAprovacao(tipoRegra, tipoServicoMultisoftware, carga))
                return;

            List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.RegraAutorizacaoCarga> regras = ObterRegrasAutorizacao(carga, tipoRegra, novoValorFrete);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (regras.Count > 0)
                CriarRegrasAprovacao(carga, regras, tipoServicoMultisoftware);
            else if (!configuracaoEmbarcador.UtilizaEmissaoMultimodal)
                carga.SituacaoAlteracaoFreteCarga = SituacaoAlteracaoFreteCarga.SemRegraAprovacao;

            if (carga.SituacaoAlteracaoFreteCarga != SituacaoAlteracaoFreteCarga.Aprovada)
                carga.SituacaoCarga = SituacaoCarga.CalculoFrete;
        }

        public bool IsUtilizarAlcadaAprovacaoAlteracaoValorFrete()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            return configuracaoEmbarcador?.UtilizarAlcadaAprovacaoAlteracaoValorFrete ?? false;
        }

        public void NotificarSituacaoAprovacaoAoOperadorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.NotificarAlteracaoCargaAoOperador)
                return;

            string acao = string.Empty;

            if (carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.AguardandoAprovacao)
                acao = Localization.Resources.Gerais.Geral.Criada;
            else if (carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.Aprovada)
                acao = Localization.Resources.Gerais.Geral.Aprovada;
            else
                acao = Localization.Resources.Gerais.Geral.Rejeitada;

            new Carga(_unitOfWork).NotificarAlteracaoAoOperador(carga, string.Format(Localization.Resources.Cargas.Carga.AcaoSolicitacaoAlteracaoFreteCarga, acao, carga.CodigoCargaEmbarcador), _unitOfWork, tipoServicoMultisoftware);
        }

        public override void RemoverAprovacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            base.RemoverAprovacao(carga);

            carga.SituacaoAlteracaoFreteCarga = SituacaoAlteracaoFreteCarga.NaoInformada;
        }

        #endregion
    }
}
