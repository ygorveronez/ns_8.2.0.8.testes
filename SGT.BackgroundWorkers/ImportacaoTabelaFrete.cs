using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class ImportacaoTabelaFrete : LongRunningProcessBase<ImportacaoTabelaFrete>
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private List<int> _importacoesEmAndamento;
        private List<int> _importacoesEmAndamentoNoBanco;

        #endregion Atributos

        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            LocalizarProcessarImportacaoTabelaFretePendente(unitOfWork);
        }

        #endregion Métodos Protegidos

        #region Métodos Privados

        private void LocalizarProcessarImportacaoTabelaFretePendente(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repImportacaoTabelaFrete = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ConfigTMS = ObterConfiguracaoEmbarcador(unitOfWork);

            // Busca a próxima planilha pendente
            List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete> ImportacaoTabelaFretePendente = repImportacaoTabelaFrete.BuscarPlanilhasImportacaoPendente();
            List<Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete> planilhasProcessando = repImportacaoTabelaFrete.BuscarPlanilhasProcessando();

            if (_importacoesEmAndamento == null) _importacoesEmAndamento = new List<int>();

            if (_importacoesEmAndamentoNoBanco == null) _importacoesEmAndamentoNoBanco = new List<int>();

            foreach (Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete processando in planilhasProcessando)
            {
                if (_importacoesEmAndamentoNoBanco.Contains(processando.Codigo)) return;


                _importacoesEmAndamentoNoBanco.Add(processando.Codigo);
            }

            if (ImportacaoTabelaFretePendente.Count > 0 || _importacoesEmAndamentoNoBanco.Count > 0)
            {
                foreach (int codigoPlanilhaImportacaoNoBanco in _importacoesEmAndamentoNoBanco)
                {
                    if (_importacoesEmAndamento.Contains(codigoPlanilhaImportacaoNoBanco)) return;

                    if (_importacoesEmAndamento.Count == (ConfigTMS.MaximoThreadsImportacaoTabelaFrete <= 0 ? 1 : ConfigTMS.MaximoThreadsImportacaoTabelaFrete)) return;

                    _importacoesEmAndamento.Add(codigoPlanilhaImportacaoNoBanco);

                    System.Threading.Thread thread = new System.Threading.Thread(() => ProcessarImportacaoTabelaFrete(codigoPlanilhaImportacaoNoBanco));
                    thread.Start();

                    System.Threading.Tasks.Task.Delay(3000).Wait();
                }

                foreach (Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete planilhaPendente in ImportacaoTabelaFretePendente)
                {
                    if (_importacoesEmAndamento.Count == (ConfigTMS.MaximoThreadsImportacaoTabelaFrete <= 0 ? 1 : ConfigTMS.MaximoThreadsImportacaoTabelaFrete)) return;

                    if (_importacoesEmAndamento.Contains(planilhaPendente.Codigo)) return;

                    _importacoesEmAndamento.Add(planilhaPendente.Codigo);

                    IndicarInicioDoProcessamento(repImportacaoTabelaFrete, planilhaPendente);

                    System.Threading.Thread thread = new System.Threading.Thread(() => ProcessarImportacaoTabelaFrete(planilhaPendente.Codigo));
                    thread.Start();

                    System.Threading.Tasks.Task.Delay(3000).Wait();
                }
            }
        }

        private void ProcessarImportacaoTabelaFrete(int codigoImportacaoTabelaFretePendente)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {

                Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repImportacaoTabela = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete ImportacaoTabelaFretePendente = repImportacaoTabela.BuscarPorCodigo(codigoImportacaoTabelaFretePendente, false);

                try
                {
                    // Importa a planilha
                    Servicos.Embarcador.Importacao.ImportacaoTabelaFrete servicoImportacaoTabelaFrete = new Servicos.Embarcador.Importacao.ImportacaoTabelaFrete(unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = servicoImportacaoTabelaFrete.Importar(ImportacaoTabelaFretePendente, ImportacaoTabelaFretePendente.Usuario, _tipoServicoMultisoftware);

                    string mensagem;
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete situacao;
                    if (string.IsNullOrEmpty(retorno.MensagemAviso))
                    {
                        mensagem = $"{retorno.Importados} registros importados com sucesso!";
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Sucesso;
                    }
                    else if (retorno.MensagemAviso == "Importação cancelada pelo Usuário")
                    {
                        mensagem = retorno.MensagemAviso;
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Cancelado;
                    }
                    else
                    {
                        mensagem = retorno.MensagemAviso;
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Erro;
                    }

                    // Registra o fim do processamento da planilha com erro ou sucesso
                    IndicarFimDoProcessamento(unitOfWork, ImportacaoTabelaFretePendente, situacao, mensagem);

                    _importacoesEmAndamento.Remove(ImportacaoTabelaFretePendente.Codigo);
                    _importacoesEmAndamentoNoBanco.Remove(ImportacaoTabelaFretePendente.Codigo);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Erro processar tabela de frete " + ImportacaoTabelaFretePendente.Codigo + ":" + ex.Message);

                    IndicarFimDoProcessamento(unitOfWork, ImportacaoTabelaFretePendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Erro, ex.Message);

                    _importacoesEmAndamento.Remove(ImportacaoTabelaFretePendente.Codigo);
                    _importacoesEmAndamentoNoBanco.Remove(ImportacaoTabelaFretePendente.Codigo);

                    throw;
                }
            }
        }

        private void IndicarInicioDoProcessamento(Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repImportacaoTabelaFrete, Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete ImportacaoTabelaFretePendente)
        {
            // Registra o início do processamento da planilha, de "Pendente" para "Processando"
            if (!ImportacaoTabelaFretePendente.DataInicioProcessamento.HasValue || ImportacaoTabelaFretePendente.DataInicioProcessamento == ImportacaoTabelaFretePendente.DataImportacao)
                ImportacaoTabelaFretePendente.DataInicioProcessamento = DateTime.Now;

            ImportacaoTabelaFretePendente.DataFimProcessamento = null;
            ImportacaoTabelaFretePendente.Mensagem = null;
            ImportacaoTabelaFretePendente.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete.Processando;
            repImportacaoTabelaFrete.Atualizar(ImportacaoTabelaFretePendente);
        }

        private void IndicarFimDoProcessamento(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete ImportacaoTabelaFretePendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoImportacaoTabelaFrete situacao, string mensagem)
        {
            Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete repositorioImportacaoTabelaFrete = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha repositorioImportacaoTabelaFreteLinha = new Repositorio.Embarcador.Frete.ImportacaoTabelaFrete.ImportacaoTabelaFreteLinha(unitOfWork);

            unitOfWork.Start();

            // Registra o fim do processamento da planilha, de "Processando" para "Sucesso" ou "Erro"
            if (!ImportacaoTabelaFretePendente.DataInicioProcessamento.HasValue)
                ImportacaoTabelaFretePendente.DataInicioProcessamento = DateTime.Now;

            ImportacaoTabelaFretePendente.DataFimProcessamento = DateTime.Now;
            ImportacaoTabelaFretePendente.Situacao = situacao;
            ImportacaoTabelaFretePendente.Mensagem = mensagem;

            repositorioImportacaoTabelaFrete.Atualizar(ImportacaoTabelaFretePendente);

            List<int> codigosTabelasFreteClientePermitemSolicitarAprovacao = repositorioImportacaoTabelaFreteLinha.BuscarCodigosTabelasFreteClientePermitemSolicitarAprovacao(ImportacaoTabelaFretePendente.Codigo);

            if (codigosTabelasFreteClientePermitemSolicitarAprovacao.Count > 0)
            {
                Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada servicoAprovacaoTabelaFrete = new Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada(unitOfWork, ObterConfiguracaoEmbarcador(unitOfWork), notificarUsuario: false);

                servicoAprovacaoTabelaFrete.AtualizarAprovacao(ImportacaoTabelaFretePendente.TabelaFrete, codigosTabelasFreteClientePermitemSolicitarAprovacao, _tipoServicoMultisoftware);
            }

            unitOfWork.CommitChanges();
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        #endregion Métodos Privados
    }
}