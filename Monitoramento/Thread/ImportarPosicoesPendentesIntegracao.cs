using Servicos.Embarcador.Monitoramento;
using System.Threading;

namespace Monitoramento.Thread
{

    public class VeiculoPosicaoPendente
    {
        public int codigoVeiculo;
        public List<long> posicoesIDs;
    }

    public class ImportarPosicoesPendentesIntegracao : AbstractThreadProcessamento
    {

        #region Atributos privados

        private static ImportarPosicoesPendentesIntegracao Instante;
        private static System.Threading.Thread PosicoesPendentesThread;

        private int tempoSleep = 5;
        private bool enable = true;
        private int limiteRegistros = 100;
        private string arquivoNivelLog;
        private List<VeiculoPosicaoPendente> posicoesCache;
        private int minutosDiferencaMinimaEntrePosicoes = 0;

        #endregion

        #region Constantes

        private const int MAX_POSICOES_CACHE = 50000;
        protected List<Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao> UltimasAtualizacoesPosicoesVeiculo;

        #endregion

        #region Métodos públicos

        // Singleton
        public static ImportarPosicoesPendentesIntegracao GetInstance(string stringConexao)
        {
            if (Instante == null) Instante = new ImportarPosicoesPendentesIntegracao(stringConexao);
            return Instante;
        }

        public System.Threading.Thread Iniciar(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancellationToken)
        {
            if (enable)
                PosicoesPendentesThread = base.IniciarThread(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware, arquivoNivelLog, tempoSleep, cancellationToken);

            return PosicoesPendentesThread;
        }

        public void Finalizar()
        {
            if (enable)
                Parar();
        }

        #endregion

        #region Implementação dos métodos abstratos

        override protected void Executar(Repositorio.UnitOfWork unitOfWork, string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            BuscarImportarPosicoesPendentes(unitOfWork);
        }

        override protected void Parar()
        {
            if (PosicoesPendentesThread != null)
            {
                PosicoesPendentesThread.Interrupt();
                PosicoesPendentesThread = null;
            }
        }

        #endregion

        #region Construtor privado

        private ImportarPosicoesPendentesIntegracao(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = string.IsNullOrWhiteSpace(stringConexao) ? null : new Repositorio.UnitOfWork(stringConexao);
            this.posicoesCache = new List<VeiculoPosicaoPendente>();
            try
            {
                tempoSleep = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao().TempoSleepThread;
                enable = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao().Ativo;
                limiteRegistros = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao().LimiteRegistros;
                minutosDiferencaMinimaEntrePosicoes = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao().MinutosDiferencaMinimaEntrePosicoes;
                arquivoNivelLog = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao().ArquivoNivelLog;
            }
            catch (Exception e)
            {
                Log(e.Message);
                throw;
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }

        #endregion

        #region Métodos privados

        private void BuscarImportarPosicoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {

            // Consulta as posições pendentes de integração
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> posicoesPendentes = BuscarPosicoesPendentes(unitOfWork);
            if (posicoesPendentes.Count > 0)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                unitOfWork.Start();
                try
                {
                    // Limpa as posições mantendo apenas as posições únicas e ainda não registradas
                    List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> posicoesValidas = ValidarPosicoesPendentes(posicoesPendentes);
                    if (posicoesValidas.Count > 0)
                    {

                        // Insere as posições válidas na tabela oficial (T_POSICAO)
                        InserirPosicoesPendentes(unitOfWork, posicoesValidas, configuracaoTMS);

                        // Adiciona as posições recebidas no cache para evitar duplicidade
                        RegistrarPosicoesPendentesNoCache(posicoesValidas);

                        // Limpa posições mais antigas do cache
                        SanitizarVeiculoPosicaoPendenteCache();

                    }

                    // Remove as posições pendentes da tabela temporária
                    DeletarPosicoesPendentes(unitOfWork, posicoesPendentes);

                    unitOfWork.CommitChanges();

                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Log(ex.ToString());
                }
            }

        }

        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> BuscarPosicoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            DateTime inicio = DateTime.UtcNow;
            Log("Consultando posicoes pendentes");
            Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao repPosicaoPendenteIntegracao = new Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> posicoesPendentes = repPosicaoPendenteIntegracao.BuscarComLimite(this.limiteRegistros);
            Log(posicoesPendentes.Count + " posicoes pendentes encontradas", inicio);
            return posicoesPendentes;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> ValidarPosicoesPendentes(List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> posicoesPendentes)
        {
            VeiculoPosicaoPendente veiculoPosicaoPendente;
            DateTime inicio = DateTime.UtcNow;
            bool encontrada;
            int total = posicoesPendentes.Count, totalPosicoesValidas;
            Log($"Validando {total} posicoes pendentes", 1);

            // Remove as posiçoes repetidas
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> posicoesValidas = new List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao>();
            for (int i = 0; i < total; i++)
            {
                totalPosicoesValidas = posicoesValidas.Count;
                encontrada = false;
                for (int j = 0; j < totalPosicoesValidas; j++)
                {
                    if (posicoesPendentes[i].ID == posicoesValidas[j].ID)
                    {
                        encontrada = true;
                        break;
                    }
                }

                // Não está repetida
                if (!encontrada)
                {
                    // Confirma que a posição distinta não existe no cach
                    veiculoPosicaoPendente = ObterVeiculoPosicaoPendenteCache(posicoesPendentes[i].Veiculo.Codigo);
                    if (!ExistePosicaoNoCacheVeiculo(posicoesPendentes[i].ID, veiculoPosicaoPendente))
                    {
                        Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao ultima = Servicos.Embarcador.Monitoramento.UltimaAtualizacao.ObterUltimaAtualizacao(posicoesPendentes[i].Veiculo.Codigo, ref this.UltimasAtualizacoesPosicoesVeiculo);
                        if (Servicos.Embarcador.Monitoramento.UltimaAtualizacao.VerificaSeJaExpirouEAtualiza(ref ultima, posicoesPendentes[i].DataVeiculo, this.minutosDiferencaMinimaEntrePosicoes))
                        {
                            posicoesValidas.Add(posicoesPendentes[i]);
                        }
                    }
                }
            }

            Log(posicoesValidas.Count + " posicoes válidas encontradas", inicio, 1);
            return posicoesValidas;
        }

        private void InserirPosicoesPendentes(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> posicoesPendentes, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            int total = posicoesPendentes.Count;
            if (total > 0)
            {
                DateTime inicio = DateTime.UtcNow;
                Log("Inserindo " + total + " posicoes pendentes");
                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao = new Dominio.Entidades.Embarcador.Logistica.Posicao
                    {
                        Veiculo = new Dominio.Entidades.Veiculo { Codigo = posicoesPendentes[i].Veiculo.Codigo, Placa = posicoesPendentes[i].Veiculo.Placa },
                        IDEquipamento = posicoesPendentes[i].IDEquipamento,
                        Data = posicoesPendentes[i].Data,
                        DataVeiculo = posicoesPendentes[i].DataVeiculo,
                        DataCadastro = posicoesPendentes[i].DataCadastro,
                        Descricao = posicoesPendentes[i].Descricao,
                        Latitude = posicoesPendentes[i].Latitude,
                        Longitude = posicoesPendentes[i].Longitude,
                        Velocidade = (posicoesPendentes[i].Velocidade >= configuracaoTMS.VelocidadeMinimaAceitaDasTecnologias && posicoesPendentes[i].Velocidade < configuracaoTMS.VelocidadeMaximaAceitaDasTecnologias) ? posicoesPendentes[i].Velocidade : 0,
                        Temperatura = (posicoesPendentes[i].Temperatura != null && posicoesPendentes[i].Temperatura >= configuracaoTMS.TemperaturaMinimaAceitaDasTecnologias && posicoesPendentes[i].Temperatura < configuracaoTMS.TemperaturaMaximaAceitaDasTecnologias) ? posicoesPendentes[i].Temperatura : null,
                        Ignicao = posicoesPendentes[i].Ignicao,
                        NivelBateria = posicoesPendentes[i].NivelBateria,
                        NivelSinalGPS = posicoesPendentes[i].NivelSinalGPS,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Mobile
                    };
                    repPosicao.Inserir(novaPosicao);
                }
                Log("Inseridas " + total + " posicoes pendentes com sucesso", inicio);
            }
        }

        private void DeletarPosicoesPendentes(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> posicoesPendentes)
        {
            int total = posicoesPendentes.Count;
            if (total > 0)
            {
                DateTime inicio = DateTime.UtcNow;
                Log("Excluindo " + total + " posicoes pendentes", 1);
                Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao repPosicaoPendenteIntegracao = new Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    repPosicaoPendenteIntegracao.Deletar(posicoesPendentes[i]);
                }
                Log("Excluidas " + total + " posicoes pendentes com sucesso", inicio, 1);
            }
        }

        private void RegistrarPosicoesPendentesNoCache(List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> posicoesPendentes)
        {
            DateTime inicio = DateTime.UtcNow;
            Log("Registrando posicoes no cache", 1);
            int total = posicoesPendentes.Count;
            for (int i = 0; i < total; i++)
            {
                VeiculoPosicaoPendente veiculoPosicaoPendente = ObterVeiculoPosicaoPendenteCache(posicoesPendentes[i].Veiculo.Codigo);
                veiculoPosicaoPendente.posicoesIDs.Add(posicoesPendentes[i].ID);
            }
            Log("Cache atualizado", inicio, 1);
        }

        private bool ExistePosicaoNoCache(int codigoVeiculo, long idPosicao)
        {
            VeiculoPosicaoPendente veiculoPosicaoPendenteCache = ObterVeiculoPosicaoPendenteCache(codigoVeiculo);
            if (veiculoPosicaoPendenteCache != null)
            {
                return ExistePosicaoNoCacheVeiculo(idPosicao, veiculoPosicaoPendenteCache);
            }
            else
            {
                AdicionaVeiculoPosicaoPendenteCache(codigoVeiculo);
            }
            return false;
        }

        private bool ExistePosicaoNoCacheVeiculo(long idPosicao, VeiculoPosicaoPendente veiculoPosicaoPendenteCache)
        {
            if (veiculoPosicaoPendenteCache != null)
            {
                int total = veiculoPosicaoPendenteCache.posicoesIDs.Count;
                for (int i = 0; i < total; i++)
                {
                    if (veiculoPosicaoPendenteCache.posicoesIDs[i] == idPosicao)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private VeiculoPosicaoPendente ObterVeiculoPosicaoPendenteCache(int codigoVeiculo)
        {
            int total = this.posicoesCache.Count;
            for (int i = 0; i < total; i++)
            {
                if (this.posicoesCache[i].codigoVeiculo == codigoVeiculo)
                {
                    return this.posicoesCache[i];
                }
            }
            return AdicionaVeiculoPosicaoPendenteCache(codigoVeiculo);
        }

        private VeiculoPosicaoPendente AdicionaVeiculoPosicaoPendenteCache(int codigoVeiculo)
        {
            VeiculoPosicaoPendente veiculoPosicaoPendenteCache = new VeiculoPosicaoPendente();
            veiculoPosicaoPendenteCache.codigoVeiculo = codigoVeiculo;
            veiculoPosicaoPendenteCache.posicoesIDs = new List<long>();
            this.posicoesCache.Add(veiculoPosicaoPendenteCache);
            return veiculoPosicaoPendenteCache;
        }

        private void SanitizarVeiculoPosicaoPendenteCache()
        {
            int total = this.posicoesCache.Count, totalPosicoes;
            for (int i = 0; i < total; i++)
            {
                totalPosicoes = this.posicoesCache[i].posicoesIDs.Count;
                if (totalPosicoes > MAX_POSICOES_CACHE)
                {
                    this.posicoesCache[i].posicoesIDs = this.posicoesCache[i].posicoesIDs.Skip(totalPosicoes - MAX_POSICOES_CACHE).ToList();
                }
            }
        }

        #endregion

    }
}
