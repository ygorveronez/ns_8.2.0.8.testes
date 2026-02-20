using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete.Consulta
{
    public class ExtracaoMassivaTabelaFrete
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frete.TabelaFreteCliente _repositorioTabelaFreteCliente;

        private readonly Repositorio.Embarcador.Cargas.TipoDeCarga _repositorioTipoCarga;
        private readonly Repositorio.Embarcador.Cargas.ModeloVeicularCarga _repositorioModeloVeicularCarga;
        private readonly Repositorio.Embarcador.Frete.ComponenteFrete _repositorioComponenteFrete;
        private readonly Repositorio.Embarcador.Frete.DistanciaTabelaFrete _repositorioDistancia;
        private readonly Repositorio.Embarcador.Frete.NumeroEntregaTabelaFrete _repositorioNumeroEntrega;
        private readonly Repositorio.Embarcador.Frete.PalletTabelaFrete _repositorioPallet;
        private readonly Repositorio.Embarcador.Frete.PesoTabelaFrete _repositorioPeso;
        private readonly Repositorio.Embarcador.Frete.TabelaFreteTempo _repositorioTempo;
        private readonly Repositorio.Embarcador.Frete.TabelaFreteAjudante _repositorioAjudante;
        private readonly Repositorio.Embarcador.Frete.TabelaFreteHora _repositorioHora;
        private readonly Repositorio.Embarcador.Frete.PacoteTabelaFrete _repositorioPacote;
        private readonly Repositorio.Embarcador.Produtos.TipoEmbalagem _repositorioTipoEmbalagem;
        private readonly Repositorio.RotaFrete _repositorioRota;
        private readonly Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia _repositorioParametroOcorrencia;

        private const string _prefixoPropriedadeComparacao = "Descricao";
        private const string _sufixoPropriedadeComparacaoAntes = "Antes";
        private const string _sufixoPropriedadeComparacaoDepois = "Depois";
        private IEnumerable<System.Reflection.PropertyInfo> _propriedadesComparacaoAntes;
        private IEnumerable<System.Reflection.PropertyInfo> _propriedadesComparacaoDepois;

        #endregion Atributos

        #region Contrutores

        public ExtracaoMassivaTabelaFrete(Repositorio.UnitOfWork unitOfWork)
        {
            _repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

            _repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            _repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            _repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            _repositorioDistancia = new Repositorio.Embarcador.Frete.DistanciaTabelaFrete(unitOfWork);
            _repositorioNumeroEntrega = new Repositorio.Embarcador.Frete.NumeroEntregaTabelaFrete(unitOfWork);
            _repositorioPallet = new Repositorio.Embarcador.Frete.PalletTabelaFrete(unitOfWork);
            _repositorioPeso = new Repositorio.Embarcador.Frete.PesoTabelaFrete(unitOfWork);
            _repositorioTempo = new Repositorio.Embarcador.Frete.TabelaFreteTempo(unitOfWork);
            _repositorioAjudante = new Repositorio.Embarcador.Frete.TabelaFreteAjudante(unitOfWork);
            _repositorioHora = new Repositorio.Embarcador.Frete.TabelaFreteHora(unitOfWork);
            _repositorioPacote = new Repositorio.Embarcador.Frete.PacoteTabelaFrete(unitOfWork);
            _repositorioTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unitOfWork);
            _repositorioRota = new Repositorio.RotaFrete(unitOfWork);
            _repositorioParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
        }

        #endregion Construtores

        #region Métodos Públicos

        public void CarregarDadosAnteriores(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> registrosRelatorio)
        {
            if (!propriedades.Exists(prop => Repositorio.Embarcador.Frete.ConsultaExtracaoMassivaTabelaFreteCliente.ObterPropriedadesParametros().Contains(prop.Propriedade)))
                return;

            var registrosAgrupadosPorTabelaFrete = registrosRelatorio.GroupBy(registro => registro.CodigoTabelaFrete);

            foreach (var registrosTabelaFrete in registrosAgrupadosPorTabelaFrete)
            {
                int codigoTabelaFrete = registrosTabelaFrete.Key;

                Dominio.ObjetosDeValor.Embarcador.Frete.TabelaFreteClienteHistorico historico = filtrosPesquisa.TabelasFreteClienteHistorico
                    .FirstOrDefault(tabela => tabela.CodigoTabelaFrete == codigoTabelaFrete);

                var registrosAgrupadosPorTabelaFreteClienteOriginal = registrosTabelaFrete
                    .GroupBy(registros => registros.CodigoTabelaFreteClienteOriginal == 0 ? registros.CodigoTabelaFreteCliente : registros.CodigoTabelaFreteClienteOriginal);

                foreach (var tabelaFreteCliente in registrosAgrupadosPorTabelaFreteClienteOriginal)
                {
                    var registrosAgrupadosPorTabelaFreteCliente = tabelaFreteCliente.GroupBy(registros => registros.CodigoTabelaFreteCliente);

                    List<int> codigosTabelaFreteClienteHistorico = (historico?.CodigosTabelasFreteClienteHistorico.ContainsKey(tabelaFreteCliente.Key) ?? false) ? historico.CodigosTabelasFreteClienteHistorico[tabelaFreteCliente.Key] : new List<int>();

                    if (!codigosTabelaFreteClienteHistorico.Contains(tabelaFreteCliente.Key))
                        codigosTabelaFreteClienteHistorico.Add(tabelaFreteCliente.Key);

                    foreach (var historicoAlteracoesTabelaFreteCliente in registrosAgrupadosPorTabelaFreteCliente)
                    {
                        int codigoTabelaFreteCliente = historicoAlteracoesTabelaFreteCliente.Key;

                        List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> alteracoes = historicoAlteracoesTabelaFreteCliente.ToList();
                        int codigoTabelaFreteClienteAnterior = ObterCodigoTabelaFreteAnterior(codigosTabelaFreteClienteHistorico, alteracoes);

                        if (codigoTabelaFreteClienteAnterior == codigoTabelaFreteCliente)
                            codigoTabelaFreteClienteAnterior = 0;

                        CarregarParametroBasePorTabelaFreteCliente(codigoTabelaFreteCliente, alteracoes);
                        CarregarTabelaFreteCliente(codigoTabelaFreteClienteAnterior, alteracoes);
                        CarregarTipoAcao(codigoTabelaFreteClienteAnterior, alteracoes);
                    }
                }
            }
        }

        private void CarregarTipoAcao(int codigoTabelaFreteClienteAnterior, List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> alteracoes)
        {
            alteracoes.ForEach(alteracao =>
            {
                if (codigoTabelaFreteClienteAnterior == 0)
                    if (AuditoriaEhReferenteImportacaoPlanilha(alteracao.DescricaoAcaoAuditoria))
                        alteracao.TipoAcao = ObterDescricaoTipoAcao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete.ValorImportadorPorPlanilha);
                    else
                        alteracao.TipoAcao = ObterDescricaoTipoAcao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete.ValorAdicionado);

                alteracao.CodigoAlteracaoOrigem = codigoTabelaFreteClienteAnterior;
                alteracao.CodigoAlteracaoAtual = alteracao.CodigoTabelaFreteCliente;
            });

            if (codigoTabelaFreteClienteAnterior == 0)
                return;

            CarregarPropriedadesComparacao();

            foreach (Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete alteracao in alteracoes)
            {
                TratarPropriedadesParaComparacao(alteracao);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete> acoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete>();

                foreach (System.Reflection.PropertyInfo propriedadeDepois in _propriedadesComparacaoDepois)
                {
                    System.Reflection.PropertyInfo propriedadeAntes = ObterPropriedadeAntes(propriedadeDepois);

                    if (propriedadeAntes == null)
                        continue;

                    string antes = propriedadeAntes.GetValue(alteracao)?.ToString() ?? "";
                    string depois = propriedadeDepois.GetValue(alteracao)?.ToString() ?? "";

                    if (alteracao.Exclusao) //Registro originário de exclusão
                        AdicionarAcaoSeNaoExistir(acoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete.Removido);
                    else if (string.IsNullOrEmpty(antes) && !string.IsNullOrEmpty(depois)) //Não tinha nada antes e coloquei alguma coisa
                    {
                        if (AuditoriaEhReferenteImportacaoPlanilha(alteracao.DescricaoAcaoAuditoria))
                            AdicionarAcaoSeNaoExistir(acoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete.ValorImportadorPorPlanilha);
                        else
                            AdicionarAcaoSeNaoExistir(acoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete.ValorAdicionado);
                    }
                    else if (depois.Equals(antes)) //Valores estão iguais
                        AdicionarAcaoSeNaoExistir(acoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete.SemRegistroDeAcao);
                    else //Valores mudaram
                        AdicionarAcaoSeNaoExistir(acoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete.ValorAtualizado);
                }

                if (acoes.Count > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete acaoExibir = acoes.OrderByDescending(prioridade => (int)prioridade).FirstOrDefault();
                    alteracao.TipoAcao = ObterDescricaoTipoAcao(acaoExibir);
                }
                else
                    alteracao.TipoAcao = ObterDescricaoTipoAcao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete.RegistroDeAcao);
            }
        }

        private void CarregarTabelaFreteCliente(int codigoTabelaFreteClienteAnterior, List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> historicoAlteracoes)
        {
            Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete primeiraAlteracao = historicoAlteracoes[0];
            int codigoTabelaFreteClienteAtual = primeiraAlteracao.CodigoTabelaFreteCliente;

            TipoParametroBaseTabelaFrete parametroBaseTabelaAtual = _parametroBasePorTabelaFreteCliente[codigoTabelaFreteClienteAtual];

            foreach (var alteracao in historicoAlteracoes)
            {
                alteracao.DescricaoObjetoParametroBaseCalculo = ObterParametroGenerico(parametroBaseTabelaAtual, alteracao.CodigoObjetoParametroBaseCalculo);
                alteracao.DescricaoObjetoItem = ObterParametroGenerico(alteracao.TipoParametroObjetoItem, alteracao.CodigoObjetoItem);
            }

            if (codigoTabelaFreteClienteAnterior > 0)
            {
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> historicoAlteracoesTabelaFreteClienteAnterior =
                    ObterHistoricoAlteracoesTabelaFreteClienteAnterior(codigoTabelaFreteClienteAnterior, primeiraAlteracao);

                if (!_parametroBasePorTabelaFreteCliente.ContainsKey(codigoTabelaFreteClienteAnterior))
                    CarregarParametroBasePorTabelaFreteCliente(codigoTabelaFreteClienteAnterior, historicoAlteracoesTabelaFreteClienteAnterior);

                TipoParametroBaseTabelaFrete parametroBaseTabelaAnterior = _parametroBasePorTabelaFreteCliente.ContainsKey(codigoTabelaFreteClienteAnterior)
                    ? _parametroBasePorTabelaFreteCliente[codigoTabelaFreteClienteAnterior]
                    : parametroBaseTabelaAtual;

                foreach (var alteracao in historicoAlteracoes)
                {
                    if (parametroBaseTabelaAnterior == parametroBaseTabelaAtual)
                    {

                        Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete alteracaoAnterior = historicoAlteracoesTabelaFreteClienteAnterior
                            .FirstOrDefault(anterior => anterior.CodigoObjetoParametroBaseCalculo == alteracao.CodigoObjetoParametroBaseCalculo
                                && anterior.CodigoObjetoItem == alteracao.CodigoObjetoItem
                                && anterior.TipoValorItem == alteracao.TipoValorItem);

                        if (alteracaoAnterior == null)
                        {
                            List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> alteracosComMesmoParametroEObjeto = historicoAlteracoesTabelaFreteClienteAnterior
                                .Where(anterior => anterior.CodigoObjetoParametroBaseCalculo == alteracao.CodigoObjetoParametroBaseCalculo
                                    && anterior.CodigoObjetoItem == alteracao.CodigoObjetoItem)
                                .ToList();

                            if (alteracosComMesmoParametroEObjeto.Count == 1)
                                alteracaoAnterior = alteracosComMesmoParametroEObjeto.First();
                        }

                        if (alteracaoAnterior == null)
                            alteracaoAnterior = historicoAlteracoesTabelaFreteClienteAnterior
                                .FirstOrDefault(anterior => anterior.CodigoObjetoParametroBaseCalculo == alteracao.CodigoObjetoParametroBaseCalculo
                                    && anterior.CodigoObjetoItem == 0);

                        CarregarAlteracoesAnteriores(alteracao, alteracaoAnterior);
                    }
                    else if (alteracao.Exclusao)
                    {
                        alteracao.TipoParametroBaseTabelaFrete = parametroBaseTabelaAnterior;
                        alteracao.DescricaoObjetoParametroBaseCalculo = ObterParametroGenerico(parametroBaseTabelaAnterior, alteracao.CodigoObjetoParametroBaseCalculo);

                        CarregarAlteracoesAnteriores(alteracao, null);
                    }
                }
            }
        }

        private static bool AuditoriaEhReferenteImportacaoPlanilha(string descricaoAuditoria)
        {
            string[] palavrasChave = { "Importou", "Importação" };
            return palavrasChave.Any(palavra => descricaoAuditoria.IndexOf(palavra, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static void CarregarAlteracoesAnteriores(Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete alteracao, Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete alteracaoAnterior)
        {
            if (alteracao.Exclusao && alteracaoAnterior == null)
                alteracaoAnterior = alteracao;

            alteracao.DataInicialVigenciaAntes = alteracaoAnterior?.DataInicialVigenciaDepois ?? "";
            alteracao.DataFinalVigenciaAntes = alteracaoAnterior?.DataFinalVigenciaDepois ?? "";
            alteracao.DescricaoOrigemAntes = alteracaoAnterior?.DescricaoOrigemDepois ?? "";
            alteracao.DescricaoDestinoAntes = alteracaoAnterior?.DescricaoDestinoDepois ?? "";
            alteracao.DescricaoValorMinimoGarantidoParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoValorMinimoGarantidoParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoValorEntregaExcedenteParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoValorEntregaExcedenteParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoValorPalletExcedenteParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoValorPalletExcedenteParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoValorQuilometragemExcedenteParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoValorQuilometragemExcedenteParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoValorPesoExcedenteParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoValorPesoExcedenteParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoValorAjudanteExcedenteParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoValorAjudanteExcedenteParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoValorMaximoParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoValorMaximoParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoPercentualPagamentoAgregadoParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoPercentualPagamentoAgregadoParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoValorBaseParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoValorBaseParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoValorHoraExcedenteParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoValorHoraExcedenteParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoValorPacoteExcedenteParametroBaseCalculoAntes = alteracaoAnterior?.DescricaoValorPacoteExcedenteParametroBaseCalculoDepois ?? "";
            alteracao.DescricaoValorItemAntes = alteracaoAnterior?.DescricaoValorItemDepois ?? "";

            if (alteracao.Exclusao)
            {
                alteracao.ValorMinimoGarantidoParametroBaseCalculoDepois = 0;
                alteracao.ValorEntregaExcedenteParametroBaseCalculoDepois = 0;
                alteracao.ValorPalletExcedenteParametroBaseCalculoDepois = 0;
                alteracao.ValorQuilometragemParametroBaseCalculoDepois = 0;
                alteracao.ValorPesoExcedenteParametroBaseCalculoDepois = 0;
                alteracao.ValorAjudanteExcedenteParametroBaseCalculoDepois = 0;
                alteracao.ValorMaximoParametroBaseCalculoDepois = 0;
                alteracao.PercentualPagamentoAgregadoParametroBaseCalculoDepois = 0;
                alteracao.ValorBaseParametroBaseCalculoDepois = 0;
                alteracao.ValorHoraExcedenteParametroBaseCalculoDepois = 0;
                alteracao.ValorPacoteExcedenteParametroBaseCalculoDepois = 0;
                alteracao.ValorItemDepois = 0;
            }
        }

        #endregion

        #region Métodos Privados

        private static void TratarPropriedadesParaComparacao(Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete alteracao)
        {
            alteracao.DescricaoOrigemAntes = OrdenarEnderecos(alteracao.DescricaoOrigemAntes);
            alteracao.DescricaoOrigemDepois = OrdenarEnderecos(alteracao.DescricaoOrigemDepois);
            alteracao.DescricaoDestinoAntes = OrdenarEnderecos(alteracao.DescricaoDestinoAntes);
            alteracao.DescricaoDestinoDepois = OrdenarEnderecos(alteracao.DescricaoDestinoDepois);
        }

        private static string OrdenarEnderecos(string enderecos, char separador = '/')
        {
            if (string.IsNullOrEmpty(enderecos) || !enderecos.Contains(separador))
                return enderecos;

            string[] enderecosOrdenados = enderecos
                .Split(new char[] { separador }, StringSplitOptions.RemoveEmptyEntries)
                .Select(endereco => endereco.Trim())
                .OrderBy(endereco => endereco)
                .ToArray();

            return string.Join(separador.ToString() + " ", enderecosOrdenados);
        }

        private void CarregarParametroBasePorTabelaFreteCliente(int codigoTabelaFreteCliente, IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> historicoAlteracoes)
        {
            if (historicoAlteracoes == null || historicoAlteracoes.Count == 0)
                return;

            Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete primeiraAlteracao = historicoAlteracoes[0];

            List<Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.HistoricoParametroBaseTabelaFrete> historicoParametrosBase = ObterHistoricoParametrosBaseTabelaFrete(primeiraAlteracao.CodigoTabelaFrete);
            if (historicoParametrosBase == null || historicoParametrosBase.Count == 0)
            {
                AdicionarParametroBaseTabelaFreteCliente(codigoTabelaFreteCliente, primeiraAlteracao.TipoParametroBaseTabelaFrete);
                return;
            }

            Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.HistoricoParametroBaseTabelaFrete primeiroHistoricoParametroBase = historicoParametrosBase
                .OrderBy(historico => historico.DataHistorico)
                .ToList()[0];

            string descricaoNovoParametroBase = "";
            if (primeiraAlteracao.DataAlteracao < primeiroHistoricoParametroBase.DataHistorico)
                descricaoNovoParametroBase = primeiroHistoricoParametroBase.DescricaoParametroBaseDe;
            else if (historicoParametrosBase.Exists(historico => historico.DataHistorico < primeiraAlteracao.DataAlteracao))
                descricaoNovoParametroBase = historicoParametrosBase.Last(historico => historico.DataHistorico < primeiraAlteracao.DataAlteracao).DescricaoParametroBasePara;
            else
                descricaoNovoParametroBase = historicoParametrosBase.Last().DescricaoParametroBasePara;

            TipoParametroBaseTabelaFrete parametroBase = primeiraAlteracao.TipoParametroBaseTabelaFrete;

            if (string.IsNullOrEmpty(descricaoNovoParametroBase))
            {
                AdicionarParametroBaseTabelaFreteCliente(codigoTabelaFreteCliente, parametroBase);
                return;
            }

            bool alterouParametroBase = false;
            if (int.TryParse(descricaoNovoParametroBase.Split(' ').FirstOrDefault() ?? "", out int codigoParametroBase)
                && Enum.IsDefined(typeof(TipoParametroBaseTabelaFrete), codigoParametroBase)
                && codigoParametroBase != (int)parametroBase)
            {
                alterouParametroBase = true;
                parametroBase = (TipoParametroBaseTabelaFrete)codigoParametroBase;
            }

            AdicionarParametroBaseTabelaFreteCliente(codigoTabelaFreteCliente, parametroBase);

            if (!alterouParametroBase)
                return;

            foreach (var alteracao in historicoAlteracoes)
            {
                alteracao.TipoParametroBaseTabelaFrete = parametroBase;
            }
        }


        #endregion

        #region Métodos Comuns

        public bool ValorZerado(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return true;

            texto = texto.Trim().Replace(",", "").Replace(".", "");

            if (decimal.TryParse(texto, out decimal result))
            {
                return result == 0;
            }

            return false;
        }

        private static string ObterDescricaoTipoAcao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete tipoAcao)
        {
            return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFreteHelper.ObterDescricao(tipoAcao);
        }

        private static void AdicionarAcaoSeNaoExistir(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete> acoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoExtracaoMassivaTabelaFrete tipoAcao)
        {
            if (acoes.Contains(tipoAcao))
                return;

            acoes.Add(tipoAcao);
        }

        private void CarregarPropriedadesComparacao()
        {
            if (_propriedadesComparacaoAntes != null && _propriedadesComparacaoDepois != null)
                return;

            _propriedadesComparacaoAntes = typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete)
                .GetProperties()
                .Where(propAntes => propAntes.Name.StartsWith(_prefixoPropriedadeComparacao)
                    && propAntes.Name.EndsWith(_sufixoPropriedadeComparacaoAntes)
                    && propAntes.PropertyType == typeof(string));

            _propriedadesComparacaoDepois = typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete)
                .GetProperties()
                .Where(propDepois => propDepois.Name.StartsWith(_prefixoPropriedadeComparacao)
                    && propDepois.Name.EndsWith(_sufixoPropriedadeComparacaoDepois)
                    && propDepois.PropertyType == typeof(string)
                    && ObterPropriedadeAntes(propDepois) != null);
        }

        private System.Reflection.PropertyInfo ObterPropriedadeAntes(System.Reflection.PropertyInfo propriedadeDepois)
        {
            return _propriedadesComparacaoAntes.FirstOrDefault(propAntes => propAntes.Name.StartsWith(propriedadeDepois.Name.Replace(_sufixoPropriedadeComparacaoDepois, _sufixoPropriedadeComparacaoAntes)));
        }

        private int ObterCodigoTabelaFreteAnterior(List<int> codigosTabelasFreteCliente, List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> historicoAlteracoes)
        {
            if (codigosTabelasFreteCliente == null || codigosTabelasFreteCliente.Count <= 1)
                return 0;

            codigosTabelasFreteCliente = codigosTabelasFreteCliente
                .OrderBy(codigo => codigo)
                .ToList();

            int codigoVigente = codigosTabelasFreteCliente.First();
            int codigoPrimeiraAlteracao = codigosTabelasFreteCliente.First(codigo => codigo > codigoVigente);

            int codigoAtual = historicoAlteracoes[0].CodigoTabelaFreteCliente;

            if (codigoAtual == codigoVigente)
                return codigosTabelasFreteCliente.Last();
            else if (codigoAtual == codigoPrimeiraAlteracao)
                return 0;
            else
                return codigosTabelasFreteCliente.Last(codigo => codigo < codigoAtual);
        }

        private IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> ObterHistoricoAlteracoesTabelaFreteClienteAnterior(int codigoTabelaFreteClienteAnterior, Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete primeiraAlteracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaExtracaoMassivaTabelaFreteCliente()
            {
                CodigosTabelasFrete = new List<int>()
                    {
                            primeiraAlteracao.CodigoTabelaFrete
                    },
                CodigoTabelaFreteClientePesquisaParametrosAnteriores = codigoTabelaFreteClienteAnterior
            };

            IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ExtracaoMassivaTabelaFrete> historicoAlteracoesTabelaFreteClienteAnterior = _repositorioTabelaFreteCliente
                .ConsultaExtracaoMassiva(filtroPesquisa, null, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta());
            return historicoAlteracoesTabelaFreteClienteAnterior;
        }

        #endregion

        #region Métodos para obter valores repetidos

        private readonly Dictionary<TipoParametroBaseTabelaFrete, List<Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico>> _parametrosGenericosPorTipoParametroBase = new();
        private string ObterParametroGenerico(TipoParametroBaseTabelaFrete tipoParametroBase, int codigoObjeto)
        {
            if (!_parametrosGenericosPorTipoParametroBase.ContainsKey(tipoParametroBase))
                _parametrosGenericosPorTipoParametroBase.Add(tipoParametroBase, new List<Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico>());

            if (_parametrosGenericosPorTipoParametroBase[tipoParametroBase].Exists(item => item.Codigo == codigoObjeto))
                return _parametrosGenericosPorTipoParametroBase[tipoParametroBase].First(item => item.Codigo == codigoObjeto).Descricao;

            Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico parametroGenerico = null;

            switch (tipoParametroBase)
            {
                case TipoParametroBaseTabelaFrete.TipoCarga:
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = _repositorioTipoCarga.BuscarPorCodigo(codigoObjeto);

                    if (tipoCarga == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = tipoCarga.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.ModeloReboque:
                case TipoParametroBaseTabelaFrete.ModeloTracao:
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modelo = _repositorioModeloVeicularCarga.BuscarPorCodigo(codigoObjeto);

                    if (modelo == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = modelo.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.ComponenteFrete:
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = _repositorioComponenteFrete.BuscarPorCodigo(codigoObjeto);

                    if (componenteFrete == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = componenteFrete.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.NumeroEntrega:
                    Dominio.Entidades.Embarcador.Frete.NumeroEntregaTabelaFrete numeroEntrega = _repositorioNumeroEntrega.BuscarPorCodigo(codigoObjeto);

                    if (numeroEntrega == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = numeroEntrega.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.Peso:
                    Dominio.Entidades.Embarcador.Frete.PesoTabelaFrete peso = _repositorioPeso.BuscarPorCodigo(codigoObjeto);

                    if (peso == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = peso.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.Distancia:
                    Dominio.Entidades.Embarcador.Frete.DistanciaTabelaFrete distancia = _repositorioDistancia.BuscarPorCodigo(codigoObjeto);

                    if (distancia == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = distancia.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.Rota:
                    Dominio.Entidades.RotaFrete rota = _repositorioRota.BuscarPorCodigo(codigoObjeto);

                    if (rota == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = rota.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.ParametrosOcorrencia:
                    Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroOcorrencia = _repositorioParametroOcorrencia.BuscarPorCodigo(codigoObjeto);

                    if (parametroOcorrencia == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = parametroOcorrencia.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.Pallets:
                    Dominio.Entidades.Embarcador.Frete.PalletTabelaFrete pallet = _repositorioPallet.BuscarPorCodigo(codigoObjeto);

                    if (pallet == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = pallet.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.Tempo:
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteTempo tempo = _repositorioTempo.BuscarPorCodigo(codigoObjeto, false);

                    if (tempo == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = tempo.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.Ajudante:
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteAjudante ajudante = _repositorioAjudante.BuscarPorCodigo(codigoObjeto, false);

                    if (ajudante == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = ajudante.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.Hora:
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteHora hora = _repositorioHora.BuscarPorCodigo(codigoObjeto, false);

                    if (hora == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = hora.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.TipoEmbalagem:
                    Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = _repositorioTipoEmbalagem.BuscarPorCodigo(codigoObjeto, false);

                    if (tipoEmbalagem == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = tipoEmbalagem.Descricao ?? ""
                    };
                    break;
                case TipoParametroBaseTabelaFrete.Pacote:
                    Dominio.Entidades.Embarcador.Frete.PacoteTabelaFrete pacote = _repositorioPacote.BuscarPorCodigo(codigoObjeto, false);

                    if (pacote == null)
                        break;

                    parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                    {
                        Codigo = codigoObjeto,
                        Descricao = pacote.Descricao ?? ""
                    };
                    break;
                default:
                    break;
            }

            if (parametroGenerico == null)
                parametroGenerico = new Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.ParametroGenerico()
                {
                    Codigo = codigoObjeto,
                    Descricao = ""
                };

            _parametrosGenericosPorTipoParametroBase[tipoParametroBase].Add(parametroGenerico);
            return parametroGenerico.Descricao;
        }

        private readonly Dictionary<int, TipoParametroBaseTabelaFrete> _parametroBasePorTabelaFreteCliente = new();
        private void AdicionarParametroBaseTabelaFreteCliente(int codigoTabelaFreteCliente, TipoParametroBaseTabelaFrete parametroBase)
        {
            if (!_parametroBasePorTabelaFreteCliente.ContainsKey(codigoTabelaFreteCliente))
                _parametroBasePorTabelaFreteCliente.Add(codigoTabelaFreteCliente, parametroBase);
        }

        private readonly Dictionary<int, List<Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.HistoricoParametroBaseTabelaFrete>> _historicoParametrosBasePorTabelaFrete = new();
        private List<Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.HistoricoParametroBaseTabelaFrete> ObterHistoricoParametrosBaseTabelaFrete(int codigoTabelaFrete)
        {
            if (!_historicoParametrosBasePorTabelaFrete.ContainsKey(codigoTabelaFrete))
            {
                // TODO: ToList cast
                List<Dominio.ObjetosDeValor.Embarcador.Frete.Consulta.HistoricoParametroBaseTabelaFrete> historicoParametrosBase = _repositorioTabelaFreteCliente
                    .ConsultaHistoricoParametrosBaseTabelaFrete(codigoTabelaFrete).ToList();
                _historicoParametrosBasePorTabelaFrete.Add(codigoTabelaFrete, historicoParametrosBase);
            }

            return _historicoParametrosBasePorTabelaFrete[codigoTabelaFrete];
        }

        #endregion
    }
}
