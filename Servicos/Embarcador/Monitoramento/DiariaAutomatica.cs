using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Frete;

namespace Servicos.Embarcador.Monitoramento
{
    public class DiariaAutomatica
    {
        private Repositorio.UnitOfWork unitOfWork;

        public DiariaAutomatica(Repositorio.UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region Métodos públicos

        public void CriarDiariasAutomaticasSeNecessario(TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            var listaCargasParaCriarDiariaAutomatica = repCarga.ObterCargasParaDiariaAutomatica();

            foreach(var carga in listaCargasParaCriarDiariaAutomatica)
            {
                CriarDiariaAutomaticaSeNecessarioPorCarga(carga, tipoServicoMultisoftware);
            }
        }

        /// <summary>
        /// Busca todas as Diárias Automáticas atuais do sistemas e atualiza seu valor e status. É chamado por uma Thread a cada X minutos.
        /// </summary>
        public void AtualizarEstadoDiariasAutomaticas(TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.DiariaAutomatica repDiariaAutomatica = new Repositorio.Embarcador.Logistica.DiariaAutomatica(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica repConfiguracaoDiariaAutomatica = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica(unitOfWork);

            var configuracaoDiariaAutomatica = repConfiguracaoDiariaAutomatica.BuscarConfiguracaoPadrao();

            if(!configuracaoDiariaAutomatica.HabilitarDiariaAutomatica)
            {
                return;
            }

            // Busca diárias automáticas que estão há X minutos sem atualização
            List<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica> listaDiariasAutomaticas = repDiariaAutomatica.BuscarPorMinutosSemAtualizacao(configuracaoDiariaAutomatica.FrequenciaAtualizacao);
            
            foreach(var diariaAutomatica in listaDiariasAutomaticas)
            {
                List<LocalFreeTime> tiposLocaisEspecificos = new List<LocalFreeTime> {
                    LocalFreeTime.Coleta,
                    LocalFreeTime.Entrega,
                    LocalFreeTime.Fronteira,
                };

                bool ehDeTipoEspecifico = tiposLocaisEspecificos.Contains(diariaAutomatica.LocalFreeTime);
                var cargaEntregas = ObterCargasEntregaPorLocalFreeTime(diariaAutomatica.Carga, diariaAutomatica.LocalFreeTime);
                bool naoTemEntregasPendentes = !TemCargaPendenteDoTipoLocal(cargaEntregas, diariaAutomatica.LocalFreeTime);
                
                // Se a diária tem um tipo de local específico (coleta, entrega, fronteira) e não há mais nenhuma CargaEntrega desse tipo a ser atendida
                if (ehDeTipoEspecifico && naoTemEntregasPendentes)
                {
                    FinalizarDiariaAutomatica(diariaAutomatica, StatusDiariaAutomatica.Finalizada, tipoServicoMultisoftware);
                    continue;
                }

                diariaAutomatica.Status = ObterStatusDiariaAutomatica(cargaEntregas);
                CalcularValorDiariaAutomatica(diariaAutomatica, tipoServicoMultisoftware);
            }
        }

        public void FinalizarDiariaAutomatica(Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica diariaAutomatica, StatusDiariaAutomatica status, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.DiariaAutomatica repDiariaAutomatica = new Repositorio.Embarcador.Logistica.DiariaAutomatica(unitOfWork);
            CalcularValorDiariaAutomatica(diariaAutomatica, tipoServicoMultisoftware);
            diariaAutomatica.Status = status;
            repDiariaAutomatica.Atualizar(diariaAutomatica);
        }

        /// <summary>
        /// Calcula e adiciona o valor da diária automática na entidade
        /// </summary>
        /// <param name="diariaAutomatica"></param>
        /// <param name="unitOfWork"></param>
        public void CalcularValorDiariaAutomatica(Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica diariaAutomatica, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Logistica.DiariaAutomatica repDiariaAutomatica = new Repositorio.Embarcador.Logistica.DiariaAutomatica(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            var tabelaFrete = ObterTabelaFreteDaCarga(diariaAutomatica.Carga, tipoServicoMultisoftware);

            double tempoTotalParado = ObterTempoParadoFreeTime(diariaAutomatica.Carga, tabelaFrete.LocalFreeTime);
            int tempoFreeTime = ObterTempoMaximoFreeTime(diariaAutomatica.Carga, tabelaFrete.LocalFreeTime);

            diariaAutomatica.TempoTotal = (int)(tempoTotalParado - tempoFreeTime);

            var servicoCalculoDiaria = new Servicos.Embarcador.Logistica.DiariaAutomaticaCalculoFrete(unitOfWork);
            var valorFinalDiaria = servicoCalculoDiaria.CalcularValorDiariaAutomatica(diariaAutomatica, configuracaoEmbarcador, tipoServicoMultisoftware);

            diariaAutomatica.ValorDiaria = valorFinalDiaria.ValorDiariaAutomatica;
            diariaAutomatica.TempoFreeTime = tempoFreeTime;
            diariaAutomatica.DataUltimaAtualizacao = DateTime.Now;

            repDiariaAutomatica.Atualizar(diariaAutomatica);

            SalvarComposicaoFrete(diariaAutomatica, valorFinalDiaria.ListaComposicaoFrete);
        }

        /// <summary>
        /// Obtém apenas as CargasEntregas que fazem parte do LocalFreeTime da tabela de frete da carga
        /// </summary>
        /// <param name="carga"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ObterCargasEntregaPorLocalFreeTime(Dominio.Entidades.Embarcador.Cargas.Carga carga, LocalFreeTime localFreeTime)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            var cargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntregaFiltradas = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            switch (localFreeTime)
            {
                case LocalFreeTime.Todos:
                    // Soma o tempo de todas as coletas/entregas/fronteiras
                    foreach (var cargaEntrega in cargaEntregas)
                    {
                        cargasEntregaFiltradas.Add(cargaEntrega);
                    }
                    break;

                case LocalFreeTime.Entrega:
                    // Soma o tempo de todas as entregas
                    foreach (var cargaEntrega in cargaEntregas)
                    {
                        if (!cargaEntrega.Coleta && cargaEntrega.Cliente != null && !cargaEntrega.Cliente.FronteiraAlfandega)
                        {
                            cargasEntregaFiltradas.Add(cargaEntrega);
                        }
                    }
                    break;

                case LocalFreeTime.Coleta:
                    // Soma o tempo de todas as coletas
                    foreach (var cargaEntrega in cargaEntregas)
                    {
                        if (cargaEntrega.Coleta && cargaEntrega.Cliente != null && !cargaEntrega.Cliente.FronteiraAlfandega)
                        {
                            cargasEntregaFiltradas.Add(cargaEntrega);
                        }
                    }
                    break;

                case LocalFreeTime.Fronteira:
                    // Soma o tempo de todas as fronteiras
                    foreach (var cargaEntrega in cargaEntregas)
                    {
                        if (cargaEntrega.Cliente != null && cargaEntrega.Cliente.FronteiraAlfandega)
                        {
                            cargasEntregaFiltradas.Add(cargaEntrega);
                        }
                    }
                    break;
            }

            return cargasEntregaFiltradas;
        }
        #endregion

        #region Métodos privados

        /// <summary>
        /// Checa se é necessário criar uma Diária Automática para uma carga específica e, se precisar, cria
        /// </summary>
        /// <param name="monitoramento"></param>
        private void CriarDiariaAutomaticaSeNecessarioPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.DiariaAutomatica repDiariaAutomatica = new Repositorio.Embarcador.Logistica.DiariaAutomatica(unitOfWork);
            var diariaAutomatica = repDiariaAutomatica.BuscarPorCarga(carga.Codigo);

            if (diariaAutomatica != null)
            {
                return;
            }

            var tabelaFrete = ObterTabelaFreteDaCarga(carga, tipoServicoMultisoftware);

            if (tabelaFrete == null)
            {
                return;
            }

            DateTime? dataQuandoExcedeuFreeTime = null;
            if (ValidarCargaExcedeuFreeTime(carga, tabelaFrete.LocalFreeTime, ref dataQuandoExcedeuFreeTime))
            {
                if (dataQuandoExcedeuFreeTime.HasValue)
                {
                    CriarDiariaAutomatica(carga, dataQuandoExcedeuFreeTime.Value, tabelaFrete.LocalFreeTime);
                }
                else
                {
                    Log.TratarErro("Erro ao criar a DiariaAutomatica. Tentou criar, mas não existe a dataQuandoExcedeu");
                }
            }

        }

        private Dominio.Entidades.Embarcador.Frete.TabelaFrete ObterTabelaFreteDaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if(carga.TabelaFrete != null)
            {
                return carga.TabelaFrete;
            }
            
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            System.Text.StringBuilder mensagemRetorno = new System.Text.StringBuilder();

            Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, tipoServicoMultisoftware);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = serFrete.ObterTabelasFrete(carga, unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador, ref mensagemRetorno, false, null, false, false);

            if (tabelasFrete.Count > 0)
            {
                return tabelasFrete[0];

            }

            return null;
        }

        private bool ValidarCargaExcedeuFreeTime(Dominio.Entidades.Embarcador.Cargas.Carga carga, LocalFreeTime localFreeTime, ref DateTime? dataExcedeuFreeTime)
        {
            bool temFreeTime = localFreeTime != LocalFreeTime.Nenhum;

            if (!temFreeTime)
            {
                return false;
            }

            double tempoMaximoFreeTime = ObterTempoMaximoFreeTime(carga, localFreeTime);
            double tempoGasto = ObterTempoParadoFreeTime(carga, localFreeTime);

            bool excedeu = tempoGasto > tempoMaximoFreeTime;

            if (excedeu)
            {
                dataExcedeuFreeTime = ObterDataQuandoExcedeuFreeTime(carga, localFreeTime);
            }

            return excedeu;
        }

        private Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica CriarDiariaAutomatica(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataQuandoExcedeuFreeTime, LocalFreeTime localFreeTime)
        {
            Repositorio.Embarcador.Logistica.DiariaAutomatica repDiariaAutomatica = new Repositorio.Embarcador.Logistica.DiariaAutomatica(unitOfWork);

            var diariaAutomatica = new Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica
            {
                Carga = carga,
                Status = StatusDiariaAutomatica.EsperandoNoLocal,
                LocalFreeTime = localFreeTime,
                DataInicioCobranca = dataQuandoExcedeuFreeTime,
            };

            repDiariaAutomatica.Inserir(diariaAutomatica);

            return diariaAutomatica;
        }

        /// <summary>
        /// Obtém o tempo parado em cada carga, mas apenas o que é relativo ao local de FreeTime cadastrado.
        /// </summary>
        /// <returns></returns>
        private double ObterTempoParadoFreeTime(Dominio.Entidades.Embarcador.Cargas.Carga carga, LocalFreeTime localFreeTime)
        {
            if (localFreeTime == Dominio.ObjetosDeValor.Embarcador.Enumeradores.LocalFreeTime.Nenhum)
            {
                return 0;
            }

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            double tempoTotal = 0;

            var cargaEntregas = ObterCargasEntregaPorLocalFreeTime(carga, localFreeTime);

            // Soma o tempo de todas as paradas que sejam do tipo do FreeTime
            foreach (var cargaEntrega in cargaEntregas)
            {
                tempoTotal += ObterTempoParadoCargaEntrega(cargaEntrega);
            }

            return tempoTotal;
        }

        private int ObterTempoMaximoFreeTime(Dominio.Entidades.Embarcador.Cargas.Carga carga, LocalFreeTime localFreeTime)
        {
            switch (localFreeTime)
            {
                case LocalFreeTime.Todos:
                    return carga.TipoOperacao?.ConfiguracaoFreeTime?.TempoTotalViagem ?? 0;
                case LocalFreeTime.Coleta:
                    return carga.TipoOperacao?.ConfiguracaoFreeTime?.TempoColetas ?? 0;
                case LocalFreeTime.Entrega:
                    return carga.TipoOperacao?.ConfiguracaoFreeTime?.TempoEntregas ?? 0;
                case LocalFreeTime.Fronteira:
                    return carga.TipoOperacao?.ConfiguracaoFreeTime?.TempoFronteiras ?? 0;
            }

            return 0;
        }

        private double ObterTempoParadoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            /*
             * Observação: estamos usando DataEntradaRaio e DataSaidaRaio por ora, mas dependendo do futuro isso pode ser
             * mudado para DataInicioEntrega e DataEntrega.
             * */
            bool temDataEntrada = cargaEntrega.DataEntradaRaio.HasValue && cargaEntrega.DataEntradaRaio != DateTime.MinValue;
            bool temDataSaida = cargaEntrega.DataSaidaRaio.HasValue && cargaEntrega.DataSaidaRaio != DateTime.MinValue;

            // Se já entrou e saiu, calcula a diferença entre a entrada e saída
            if (temDataEntrada && temDataSaida)
            {
                return (cargaEntrega.DataSaidaRaio.Value - cargaEntrega.DataEntradaRaio.Value).TotalMinutes;
            }

            // Se só entrou e ainda está lá, calcula a diferença entre e entrada e agora
            if(temDataEntrada)
            {
                return (DateTime.Now - cargaEntrega.DataEntradaRaio.Value).TotalMinutes;
            }

            return 0;
        }

        /// <summary>
        /// Verifica se há alguma CargaEntrega ainda pendente que seja do tipo LocalFreeTime
        /// </summary>
        private bool TemCargaPendenteDoTipoLocal(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, LocalFreeTime localFreeTime)
        {
            foreach(var cargaEntrega in cargaEntregas)
            {
                var cargaEntregaEstaPendente = !cargaEntrega.DataConfirmacao.HasValue || cargaEntrega.DataConfirmacao == DateTime.MinValue;

                if(!cargaEntregaEstaPendente)
                {
                    continue;
                }

                switch(localFreeTime)
                {
                    case LocalFreeTime.Todos:
                        return true;
                    case LocalFreeTime.Entrega:
                        if(!cargaEntrega.Coleta && cargaEntrega.Cliente != null && !cargaEntrega.Cliente.FronteiraAlfandega) {
                            return true;
                        }
                        continue;
                    case LocalFreeTime.Coleta:
                        if (cargaEntrega.Coleta && cargaEntrega.Cliente != null && !cargaEntrega.Cliente.FronteiraAlfandega)
                        {
                            return true;
                        }
                        continue;
                    case LocalFreeTime.Fronteira:
                        if (cargaEntrega.Cliente != null && cargaEntrega.Cliente.FronteiraAlfandega)
                        {
                            return true;
                        }
                        continue;
                }
            }

            return false;
        }

        private DateTime? ObterDataQuandoExcedeuFreeTime(Dominio.Entidades.Embarcador.Cargas.Carga carga, LocalFreeTime localFreeTime)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            var cargaEntregas = ObterCargasEntregaPorLocalFreeTime(carga, localFreeTime);
            cargaEntregas.Sort(new Comparison<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>((a, b) => a.OrdemRealizada > b.OrdemRealizada ? 1 : -1));

            double tempoTotal = 0;
            int tempoMaximoFreeTime = ObterTempoMaximoFreeTime(carga, localFreeTime);

            DateTime? dataExcedeuFreeTime = null;

            foreach (var cargaEntrega in cargaEntregas)
            {
                var tempoCargaEntrega = ObterTempoParadoCargaEntrega(cargaEntrega);

                if (tempoTotal + tempoCargaEntrega > tempoMaximoFreeTime) // Excedeu nessa entrega
                {
                    double minutosDepoisInicioEntrega = tempoTotal + tempoCargaEntrega - tempoMaximoFreeTime;
                    dataExcedeuFreeTime = cargaEntrega.DataEntradaRaio.Value.AddMinutes(minutosDepoisInicioEntrega);
                }

                tempoTotal += tempoCargaEntrega;
            }


            return dataExcedeuFreeTime;
        }

        private void SalvarComposicaoFrete(Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica diariaAutomatica, List<ComposicaoFrete> listaComposicaoFrete)
        {
            var repDiariaAutomaticaComposicaoFrete = new Repositorio.Embarcador.Logistica.DiariaAutomaticaComposicaoFrete(unitOfWork);

            // Deleta os antigos

            var listaComposicoesParaDeletar = repDiariaAutomaticaComposicaoFrete.BuscarPorDiariaAutomatica(diariaAutomatica.Codigo);

            foreach(var composicaoFreteParaDeletar in listaComposicoesParaDeletar)
            {
                repDiariaAutomaticaComposicaoFrete.Deletar(composicaoFreteParaDeletar);
            }

            foreach (var composicaoFrete in listaComposicaoFrete)
            {
                Dominio.Entidades.Embarcador.Logistica.DiariaAutomaticaComposicaoFrete diariaAutomaticaComposicaoFrete = new Dominio.Entidades.Embarcador.Logistica.DiariaAutomaticaComposicaoFrete
                {
                    DiariaAutomatica = diariaAutomatica,
                    TipoParametro = composicaoFrete.TipoParametro,
                    TipoValor = composicaoFrete.TipoValor,
                    Valor = composicaoFrete.Valor,
                    Formula = composicaoFrete.Formula,
                    DescricaoComponente = composicaoFrete.DescricaoComponente,
                    CodigoComponente = composicaoFrete.CodigoComponente,
                    ValoresFormula = composicaoFrete.ValoresFormula,
                    ValorCalculado = composicaoFrete.ValorCalculado,
                };

                repDiariaAutomaticaComposicaoFrete.Inserir(diariaAutomaticaComposicaoFrete);
            }
        }

        /// <summary>
        /// Infere se o motorista está no local de algum cliente ou em deslocamento
        /// </summary>
        /// <param name="cargaEntregas"></param>
        /// <returns></returns>
        private StatusDiariaAutomatica ObterStatusDiariaAutomatica(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas)
        {
            bool estaEmAlgumRaio = cargaEntregas.Find(o => o.DataEntradaRaio.HasValue && !o.DataSaidaRaio.HasValue) != null;
            return estaEmAlgumRaio ? StatusDiariaAutomatica.EsperandoNoLocal : StatusDiariaAutomatica.EmDeslocamento;
        }

        #endregion
    }
}
