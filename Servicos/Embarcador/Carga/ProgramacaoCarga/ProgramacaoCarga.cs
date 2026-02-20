using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.ProgramacaoCarga
{
    public sealed class ProgramacaoCarga
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga _configuracaoPreCarga;

        #endregion Atributos

        #region Construtores

        public ProgramacaoCarga(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null, auditado: null) { }

        public ProgramacaoCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador) : this(unitOfWork, configuracaoEmbarcador, auditado: null) { }

        public ProgramacaoCarga(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, configuracaoEmbarcador: null, auditado) { }

        public ProgramacaoCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion Contrutores

        #region Métodos Privados

        private void AdicionarSugestaoProgramacaoCargaDestinos(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga, Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCargaAdicionar)
        {
            if ((sugestaoProgramacaoCarga.CodigosDestinos == null) || (sugestaoProgramacaoCarga.CodigosDestinos.Count == 0))
                return;

            Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino repositorioDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino(_unitOfWork);

            foreach (int codigoDestino in sugestaoProgramacaoCarga.CodigosDestinos)
            {
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino destino = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino()
                {
                    Localidade = new Dominio.Entidades.Localidade() { Codigo = codigoDestino },
                    SugestaoProgramacaoCarga = sugestaoProgramacaoCargaAdicionar
                };

                repositorioDestino.Inserir(destino);
            }
        }

        private void AdicionarSugestaoProgramacaoCargaEstadosDestino(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga, Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCargaAdicionar)
        {
            if ((sugestaoProgramacaoCarga.SiglasEstadosDestino == null) || (sugestaoProgramacaoCarga.SiglasEstadosDestino.Count == 0))
                return;

            Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino repositorioEstadoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino(_unitOfWork);

            foreach (string siglaEstadoDestino in sugestaoProgramacaoCarga.SiglasEstadosDestino)
            {
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino estadoDestino = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino()
                {
                    Estado = new Dominio.Entidades.Estado() { Sigla = siglaEstadoDestino },
                    SugestaoProgramacaoCarga = sugestaoProgramacaoCargaAdicionar
                };

                repositorioEstadoDestino.Inserir(estadoDestino);
            }
        }

        private void AdicionarSugestaoProgramacaoCargaHistoricoCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga, Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCargaAdicionar)
        {
            if ((sugestaoProgramacaoCarga.CodigosCargas == null) || (sugestaoProgramacaoCarga.CodigosCargas.Count == 0))
                return;

            Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga repositorioHistoricoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga(_unitOfWork);

            foreach (int codigoCarga in sugestaoProgramacaoCarga.CodigosCargas)
            {
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga historicoCarga = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaHistoricoCarga()
                {
                    Carga = new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = codigoCarga },
                    SugestaoProgramacaoCarga = sugestaoProgramacaoCargaAdicionar
                };

                repositorioHistoricoCarga.Inserir(historicoCarga);
            }
        }

        private void AdicionarSugestaoProgramacaoCargaRegioesDestino(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga, Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCargaAdicionar)
        {
            if ((sugestaoProgramacaoCarga.CodigosRegioesDestino == null) || (sugestaoProgramacaoCarga.CodigosRegioesDestino.Count == 0))
                return;

            Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino repositorioRegiaoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino(_unitOfWork);

            foreach (int codigoRegiaoDestino in sugestaoProgramacaoCarga.CodigosRegioesDestino)
            {
                Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino regiaoDestino = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino()
                {
                    Regiao = new Dominio.Entidades.Embarcador.Localidades.Regiao() { Codigo = codigoRegiaoDestino },
                    SugestaoProgramacaoCarga = sugestaoProgramacaoCargaAdicionar
                };

                repositorioRegiaoDestino.Inserir(regiaoDestino);
            }
        }
        
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga ObterConfiguracaoPreCarga()
        {
            if (_configuracaoPreCarga == null)
                _configuracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoPreCarga;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaCarga ObterFiltrosPesquisaCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.ParametroSugestaoProgramacaoCarga parametroSugestaoProgramacaoCarga, (int Codigo, int CodigoFilial) configuracaoProgramacaoCarga)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaCarga();

            filtrosPesquisa.CodigoFilial = configuracaoProgramacaoCarga.CodigoFilial;
            filtrosPesquisa.DatasFinalizacaoEmissao = parametroSugestaoProgramacaoCarga.DataHistoricoInicial.DatesBetweenWithoutWeekend(parametroSugestaoProgramacaoCarga.DataHistoricoFinal);
            filtrosPesquisa.CodigosDestinos = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaDestino(_unitOfWork).BuscarPorConfiguracaoParaSugestao(configuracaoProgramacaoCarga.Codigo);
            filtrosPesquisa.CodigosModelosVeicularesCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaModeloVeicularCarga(_unitOfWork).BuscarPorConfiguracaoParaSugestao(configuracaoProgramacaoCarga.Codigo);
            filtrosPesquisa.CodigosTiposCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoCarga(_unitOfWork).BuscarPorConfiguracaoParaSugestao(configuracaoProgramacaoCarga.Codigo);
            filtrosPesquisa.CodigosTiposOperacao = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaTipoOperacao(_unitOfWork).BuscarPorConfiguracaoParaSugestao(configuracaoProgramacaoCarga.Codigo);
            filtrosPesquisa.CodigosRegioesDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaRegiaoDestino(_unitOfWork).BuscarPorConfiguracaoParaSugestao(configuracaoProgramacaoCarga.Codigo);
            filtrosPesquisa.SiglasEstadosDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCargaEstadoDestino(_unitOfWork).BuscarPorConfiguracaoParaSugestao(configuracaoProgramacaoCarga.Codigo);

            return filtrosPesquisa;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga> ObterSugestoesProgramacaoCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.ParametroSugestaoProgramacaoCarga parametroSugestaoProgramacaoCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga> sugestoesProgramacaoCarga = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga>();
            List<DateTime> datasSugestaoProgramacaoCarga = parametroSugestaoProgramacaoCarga.DataProgramacaoInicial.DatesBetweenWithoutWeekend(parametroSugestaoProgramacaoCarga.DataProgramacaoFinal);

            if (datasSugestaoProgramacaoCarga.Count == 0)
                return sugestoesProgramacaoCarga;

            Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga repositorioConfiguracaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga(_unitOfWork);
            (int Codigo, int CodigoFilial) configuracaoProgramacaoCarga = repositorioConfiguracaoProgramacaoCarga.BuscarPorCodigoParaSugestao(parametroSugestaoProgramacaoCarga.CodigoConfiguracaoProgramacaoCarga);

            if (configuracaoProgramacaoCarga.Equals(default))
                return sugestoesProgramacaoCarga;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.FiltroPesquisaCarga filtrosPesquisa = ObterFiltrosPesquisaCarga(parametroSugestaoProgramacaoCarga, configuracaoProgramacaoCarga);
            List<(int Codigo, DateTime DataFinalizacaoEmissao, int CodigoModeloVeicularCarga, int CodigoTipoCarga, int CodigoTipoOperacao)> cargas = repositorioCarga.BuscarCargasParaSugestaoProgramacaoCarga(filtrosPesquisa);

            List<(int CodigoModeloVeicularCarga, int CodigoTipoCarga, int CodigoTipoOperacao)> configuracoesPorCarga = cargas
                .GroupBy(carga => new { carga.CodigoModeloVeicularCarga, carga.CodigoTipoCarga, carga.CodigoTipoOperacao })
                .Select(agrupamentoCarga => ValueTuple.Create(agrupamentoCarga.Key.CodigoModeloVeicularCarga, agrupamentoCarga.Key.CodigoTipoCarga, agrupamentoCarga.Key.CodigoTipoOperacao))
                .ToList();

            foreach ((int CodigoModeloVeicularCarga, int CodigoTipoCarga, int CodigoTipoOperacao) configuracaoPorCarga in configuracoesPorCarga)
            {
                List<(int Codigo, DateTime DataFinalizacaoEmissao, int CodigoModeloVeicularCarga, int CodigoTipoCarga, int CodigoTipoOperacao)> cargasPorConfiguracao = cargas
                    .Where(carga =>
                        carga.CodigoModeloVeicularCarga == configuracaoPorCarga.CodigoModeloVeicularCarga &&
                        carga.CodigoTipoCarga == configuracaoPorCarga.CodigoTipoCarga &&
                        carga.CodigoTipoOperacao == configuracaoPorCarga.CodigoTipoOperacao
                    )
                    .ToList();

                decimal totalDatasFinalizacaoEmissao = filtrosPesquisa.DatasFinalizacaoEmissao.Count;
                decimal mediaCargasGeradas = (totalDatasFinalizacaoEmissao > 0) ? Math.Round((cargasPorConfiguracao.Count / totalDatasFinalizacaoEmissao), 2) : 0m;

                if (mediaCargasGeradas == 0m)
                    continue;

                foreach (DateTime dataProgramacao in datasSugestaoProgramacaoCarga)
                    sugestoesProgramacaoCarga.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga()
                    {
                        CodigoConfiguracaoProgramacaoCarga = configuracaoProgramacaoCarga.Codigo,
                        CodigoFilial = configuracaoProgramacaoCarga.CodigoFilial,
                        CodigoModeloVeicularCarga = configuracaoPorCarga.CodigoModeloVeicularCarga,
                        CodigoTipoCarga = configuracaoPorCarga.CodigoTipoCarga,
                        CodigoTipoOperacao = configuracaoPorCarga.CodigoTipoOperacao,
                        CodigosCargas = cargasPorConfiguracao.Select(carga => carga.Codigo).ToList(),
                        CodigosDestinos = filtrosPesquisa.CodigosDestinos.ToList(),
                        CodigosRegioesDestino = filtrosPesquisa.CodigosRegioesDestino.ToList(),
                        Data = dataProgramacao,
                        Quantidade = mediaCargasGeradas,
                        QuantidadeValidada = (int)mediaCargasGeradas,
                        SiglasEstadosDestino = filtrosPesquisa.SiglasEstadosDestino.ToList(),
                    });
            }

            return sugestoesProgramacaoCarga;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga> ObterSugestoesProgramacaoCargaNaoGeradas(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.ParametroSugestaoProgramacaoCarga parametroSugestaoProgramacaoCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga> sugestoesProgramacaoCarga = ObterSugestoesProgramacaoCarga(parametroSugestaoProgramacaoCarga);

            if (sugestoesProgramacaoCarga.Count == 0)
                throw new ServicoException("Não existe base histórica suficiente para geração da sugestão de pré planejamento.");

            Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(_unitOfWork);
            List<(int CodigoConfiguracaoProgramacaoCarga, DateTime Data)> configuracoesSemSugestaoGeradas = new List<(int CodigoConfiguracaoProgramacaoCarga, DateTime Data)>();

            List<(int CodigoConfiguracaoProgramacaoCarga, DateTime Data)> configuracoesAgrupadasPorData = sugestoesProgramacaoCarga
                .GroupBy(sugestao => new { sugestao.CodigoConfiguracaoProgramacaoCarga, sugestao.Data })
                .Select(agrupamentoSugestao => ValueTuple.Create(agrupamentoSugestao.Key.CodigoConfiguracaoProgramacaoCarga, agrupamentoSugestao.Key.Data))
                .ToList();

            foreach ((int CodigoConfiguracaoProgramacaoCarga, DateTime Data) configuracaoAgrupadaPorData in configuracoesAgrupadasPorData)
            {
                if (!repositorioSugestaoProgramacaoCarga.ExistePorDataProgramacao(configuracaoAgrupadaPorData.CodigoConfiguracaoProgramacaoCarga, configuracaoAgrupadaPorData.Data))
                    configuracoesSemSugestaoGeradas.Add(configuracaoAgrupadaPorData);
            }

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga> sugestoesProgramacaoCargaNaoGeradas = sugestoesProgramacaoCarga
                .Where(sugestao =>
                    configuracoesSemSugestaoGeradas.Any(configuracao =>
                        sugestao.CodigoConfiguracaoProgramacaoCarga == configuracao.CodigoConfiguracaoProgramacaoCarga &&
                        sugestao.Data == configuracao.Data
                    )
                )
                .ToList();

            if (sugestoesProgramacaoCargaNaoGeradas.Count == 0)
                throw new ServicoException("Sugestões de pré planejamento já cadastradas para o período informado.");

            return sugestoesProgramacaoCargaNaoGeradas;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void AdicionarSugestaoProgramacaoCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga)
        {
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCargaAdicionar = new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga()
            {
                ConfiguracaoProgramacaoCarga = (sugestaoProgramacaoCarga.CodigoConfiguracaoProgramacaoCarga > 0) ? new Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.ConfiguracaoProgramacaoCarga() { Codigo = sugestaoProgramacaoCarga.CodigoConfiguracaoProgramacaoCarga } : null,
                DataProgramacao = sugestaoProgramacaoCarga.Data,
                Filial = new Dominio.Entidades.Embarcador.Filiais.Filial() { Codigo = sugestaoProgramacaoCarga.CodigoFilial },
                ModeloVeicularCarga = (sugestaoProgramacaoCarga.CodigoModeloVeicularCarga > 0) ? new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = sugestaoProgramacaoCarga.CodigoModeloVeicularCarga } : null,
                Quantidade = sugestaoProgramacaoCarga.Quantidade,
                QuantidadeValidada = sugestaoProgramacaoCarga.QuantidadeValidada,
                Situacao = SituacaoSugestaoProgramacaoCarga.Gerada,
                TipoCarga = (sugestaoProgramacaoCarga.CodigoTipoCarga > 0) ? new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = sugestaoProgramacaoCarga.CodigoTipoCarga } : null,
                TipoOperacao = (sugestaoProgramacaoCarga.CodigoTipoOperacao > 0) ? new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao() { Codigo = sugestaoProgramacaoCarga.CodigoTipoOperacao } : null,
                Usuario = _auditado?.Usuario
            };

            repositorioSugestaoProgramacaoCarga.Inserir(sugestaoProgramacaoCargaAdicionar);
            AdicionarSugestaoProgramacaoCargaHistoricoCarga(sugestaoProgramacaoCarga, sugestaoProgramacaoCargaAdicionar);
            AdicionarSugestaoProgramacaoCargaDestinos(sugestaoProgramacaoCarga, sugestaoProgramacaoCargaAdicionar);
            AdicionarSugestaoProgramacaoCargaEstadosDestino(sugestaoProgramacaoCarga, sugestaoProgramacaoCargaAdicionar);
            AdicionarSugestaoProgramacaoCargaRegioesDestino(sugestaoProgramacaoCarga, sugestaoProgramacaoCargaAdicionar);
            Auditoria.Auditoria.Auditar(_auditado, sugestaoProgramacaoCargaAdicionar, $"Sugestão adicionada {(sugestaoProgramacaoCargaAdicionar.ConfiguracaoProgramacaoCarga == null ? "manualmente" : "automaticamente")}", _unitOfWork);
        }

        public void CancelarSugestaoProgramacaoCarga(Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga)
        {
            sugestaoProgramacaoCarga.Situacao = SituacaoSugestaoProgramacaoCarga.Cancelada;

            new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(_unitOfWork).Atualizar(sugestaoProgramacaoCarga);
            Auditoria.Auditoria.Auditar(_auditado, sugestaoProgramacaoCarga, "Sugestão cancelada", _unitOfWork);
        }

        public void GerarSugestoesProgramacaoCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.ParametroSugestaoProgramacaoCarga parametroSugestaoProgramacaoCarga)
        {
            if (parametroSugestaoProgramacaoCarga.DataProgramacaoInicial < DateTime.Now)
                throw new ServicoException("A data de pré planejamento inicial deve ser maior ou igual a data atual.");

            if (parametroSugestaoProgramacaoCarga.DataProgramacaoFinal < DateTime.Now)
                throw new ServicoException("A data de pré planejamento final deve ser maior ou igual a data atual.");

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga> sugestoesProgramacaoCarga = ObterSugestoesProgramacaoCargaNaoGeradas(parametroSugestaoProgramacaoCarga);

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga in sugestoesProgramacaoCarga)
                AdicionarSugestaoProgramacaoCarga(sugestaoProgramacaoCarga);
        }

        public void PublicarSugestaoProgramacaoCarga(Dominio.Entidades.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga sugestaoProgramacaoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga repositorioSugestaoProgramacaoCarga = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino repositorioSugestaoProgramacaoCargaDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaDestino(_unitOfWork);
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino repositorioSugestaoProgramacaoCargaEstadoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaEstadoDestino(_unitOfWork);
            Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino repositorioSugestaoProgramacaoCargaRegiaoDestino = new Repositorio.Embarcador.Cargas.ProgramacaoCarga.SugestaoProgramacaoCargaRegiaoDestino(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(_unitOfWork);
            Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Logistica.FilaCarregamentoVeiculo(_unitOfWork, _auditado?.Usuario, Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(tipoServicoMultisoftware));
            PreCarga.PreCargaOfertaTransportador servicoPreCargaOfertaTransportador = new PreCarga.PreCargaOfertaTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = ObterConfiguracaoPreCarga();
            List<Dominio.Entidades.Localidade> sugestaoProgramacaoCargaDestinos = repositorioSugestaoProgramacaoCargaDestino.BuscarDestinosPorSugestaoProgramacaoCarga(sugestaoProgramacaoCarga.Codigo);
            List<Dominio.Entidades.Estado> sugestaoProgramacaoCargaEstadosDestino = repositorioSugestaoProgramacaoCargaEstadoDestino.BuscarEstadosPorSugestaoProgramacaoCarga(sugestaoProgramacaoCarga.Codigo);
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> sugestaoProgramacaoCargaRegioesDestino = repositorioSugestaoProgramacaoCargaRegiaoDestino.BuscarRegioesPorSugestaoProgramacaoCarga(sugestaoProgramacaoCarga.Codigo);
            int proximoNumeroPreCarga = (configuracaoEmbarcador?.UtilizarNumeroPreCargaPorFilial ?? true) ? repositorioPreCarga.ObterProximoCodigo(sugestaoProgramacaoCarga.Filial?.Codigo ?? 0) : repositorioPreCarga.ObterProximoCodigo();

            for (int i = 0; i < sugestaoProgramacaoCarga.QuantidadeValidada; i++)
            {
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = new Dominio.Entidades.Embarcador.PreCargas.PreCarga();

                preCarga.AdicionadaManualmente = true;
                preCarga.ConfiguracaoProgramacaoCarga = sugestaoProgramacaoCarga.ConfiguracaoProgramacaoCarga;
                preCarga.SugestaoProgramacaoCarga = sugestaoProgramacaoCarga;
                preCarga.ProgramacaoCarga = true;
                preCarga.DataCriacaoPreCarga = DateTime.Now;
                preCarga.DataPrevisaoEntrega = sugestaoProgramacaoCarga.DataProgramacao;
                preCarga.NumeroPreCarga = proximoNumeroPreCarga.ToString();
                preCarga.SituacaoPreCarga = SituacaoPreCarga.Nova;
                preCarga.Filial = sugestaoProgramacaoCarga.Filial;
                preCarga.ModeloVeicularCarga = sugestaoProgramacaoCarga.ModeloVeicularCarga;
                preCarga.TipoDeCarga = sugestaoProgramacaoCarga.TipoCarga;
                preCarga.TipoOperacao = sugestaoProgramacaoCarga.TipoOperacao;

                repositorioPreCarga.Inserir(preCarga);

                foreach (Dominio.Entidades.Localidade destino in sugestaoProgramacaoCargaDestinos)
                {
                    Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino preCargaDestino = new Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino()
                    {
                        PreCarga = preCarga,
                        Localidade = destino
                    };

                    repositorioPreCargaDestino.Inserir(preCargaDestino);
                }

                foreach (Dominio.Entidades.Estado estadoDestino in sugestaoProgramacaoCargaEstadosDestino)
                {
                    Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino preCargaEstadoDestino = new Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino()
                    {
                        PreCarga = preCarga,
                        Estado = estadoDestino
                    };

                    repositorioPreCargaEstadoDestino.Inserir(preCargaEstadoDestino);
                }

                foreach (Dominio.Entidades.Embarcador.Localidades.Regiao regiaoDestino in sugestaoProgramacaoCargaRegioesDestino)
                {
                    Dominio.Entidades.Embarcador.PreCargas.PreCargaRegiaoDestino preCargaRegiaoDestino = new Dominio.Entidades.Embarcador.PreCargas.PreCargaRegiaoDestino()
                    {
                        PreCarga = preCarga,
                        Regiao = regiaoDestino
                    };

                    repositorioPreCargaRegiaoDestino.Inserir(preCargaRegiaoDestino);
                }

                if (configuracaoPreCarga.VincularFilaCarregamentoVeiculoAutomaticamente)
                {
                    try
                    {
                        servicoFilaCarregamentoVeiculo.AlocarParaPrimeiroDaFila(preCarga, tipoServicoMultisoftware);
                    }
                    catch
                    {
                        servicoPreCargaOfertaTransportador.DisponibilizarParaTransportadorPorRota(preCarga);
                    }
                }

                proximoNumeroPreCarga++;
            }

            sugestaoProgramacaoCarga.Situacao = SituacaoSugestaoProgramacaoCarga.Publicada;

            repositorioSugestaoProgramacaoCarga.Atualizar(sugestaoProgramacaoCarga);
            Auditoria.Auditoria.Auditar(_auditado, sugestaoProgramacaoCarga, "Sugestão publicada", _unitOfWork);
        }

        #endregion Métodos Públicos
    }
}
