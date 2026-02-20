using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Configuracoes
{
    public class ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void SalvarConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro, List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> configuracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracaos)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> configuracoesExistentes = repositorio.BuscarPorCondiguracaoFinanceira(configuracaoFinanceiro.Codigo);

            List<int> codigosNovos = configuracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracaos.Select(x => x.Codigo).ToList();

            if (configuracoesExistentes.Any())
            {
                RemoverConfiguracoesObsoletas(repositorio, configuracoesExistentes, codigosNovos);
                AtualizarOuInserirConfiguracoes(repositorio, configuracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracaos, configuracoesExistentes, configuracaoFinanceiro);
            }
            else
            {
                InserirNovasConfiguracoes(repositorio, configuracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracaos, configuracaoFinanceiro);
            }
        }

        public int ObterIntervalo()
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> configuracoesExistentes = repositorio.BuscarPorCondiguracaoFinanceira(configuracaoFinanceiro.Codigo);

            if (!configuracaoFinanceiro.HabilitaIntervaloTempoLiberaDocumentoEmitidoEscrituracao ?? false)
                return 0;

            try
            {
                if (configuracoesExistentes.Any())
                {
                    DateTime dataAtual = DateTime.Today;
                    int mesAtual = dataAtual.Month;
                    int anoAtual = dataAtual.Year;

                    DateTime dataInicial;
                    DateTime dataFinal;
                    int ultimoDiaDoMes = DateTime.DaysInMonth(anoAtual, mesAtual);

                    foreach (var item in configuracoesExistentes)
                    {
                        dataInicial = new DateTime(anoAtual, mesAtual, Math.Min(item.DiaInicial, ultimoDiaDoMes));
                        dataFinal = new DateTime(anoAtual, mesAtual, Math.Min(item.DiaFinal, ultimoDiaDoMes));

                        if (dataInicial > dataFinal)
                        {
                            if (dataInicial < dataAtual && dataFinal.AddMonths(1) >= dataAtual)
                            {
                                return item.IntervaloHora;
                            }
                            else if (dataInicial.AddMonths(-1) <= dataAtual && dataFinal > dataAtual)
                            {
                                return item.IntervaloHora;
                            }
                        }
                        else
                        {
                            if (dataInicial <= dataAtual && dataFinal >= dataAtual)
                            {
                                return item.IntervaloHora;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.TratarErro(e);
            }

            return 0;
        }
        #endregion

        #region Métodos Privados

        private void RemoverConfiguracoesObsoletas(Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao repositorio, List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> configuracoesExistentes, List<int> codigosNovos)
        {
            var configuracoesParaRemover = configuracoesExistentes.Where(c => !codigosNovos.Contains(c.Codigo)).ToList();
            foreach (var configuracao in configuracoesParaRemover)
            {
                repositorio.Deletar(configuracao);
            }
        }

        private void AtualizarOuInserirConfiguracoes(Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao repositorio, List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> configuracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracaos, List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> configuracoesExistentes, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro)
        {
            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> intervalosParaInserir = new List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao>();

            foreach (var item in configuracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracaos)
            {
                if (configuracoesExistentes.Exists(c => c.Codigo == item.Codigo))
                {
                    var configuracaoExistente = repositorio.BuscarPorCodigo(item.Codigo, false) ?? throw new ControllerException("Não Foi Possivel Atualizar os Registros!");
                    configuracaoExistente.DiaInicial = item.DiaInicial;
                    configuracaoExistente.DiaFinal = item.DiaFinal;
                    configuracaoExistente.IntervaloHora = item.IntervaloHora;
                    configuracaoExistente.ConfiguracaoFinanceiro = configuracaoFinanceiro;
                    repositorio.Atualizar(configuracaoExistente);
                }
                else
                {
                    intervalosParaInserir.Add(item);
                }
            }

            InserirNovasConfiguracoes(repositorio, intervalosParaInserir, configuracaoFinanceiro);
        }

        private void InserirNovasConfiguracoes(Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao repositorio, List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracao> configuracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracaos, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro)
        {
            foreach (var item in configuracaoFinanceiroIntervaloTempoDocumentoEmitidoEscrituracaos)
            {
                item.ConfiguracaoFinanceiro = configuracaoFinanceiro;
                repositorio.Inserir(item);
            }
        }

        #endregion
    }
}
