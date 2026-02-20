using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    sealed class FilaCarregamentoVeiculoDadosAdicionar
    {
        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamento CentroCarregamento { get; set; }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo ConjuntoVeiculo { get; set; }

        public DateTime? DataProgramada { get; set; }

        public bool EmTransicao { get; set; }

        public Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        public Dominio.Entidades.Usuario Motorista { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga TipoRetornoCarga { get; set; }

        public List<int> CodigosDestino { get; set; }

        public List<int> CodigosRegiaoDestino { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<string> SiglasEstadoDestino { get; set; }

        public Dominio.Entidades.Embarcador.Logistica.AreaVeiculo AreaVeiculo { get; set; }

        public Dominio.Entidades.Embarcador.Veiculos.Equipamento Equipamento { get; set; }

    }

    public sealed class FilaCarregamentoVeiculo : FilaCarregamentoBase
    {
        #region Atributos Privados Somente Leitura

        private readonly Dominio.Entidades.Usuario _usuario;

        #endregion

        #region Construtores

        public FilaCarregamentoVeiculo(Repositorio.UnitOfWork unitOfWork, OrigemAlteracaoFilaCarregamento origemAlteracao) : base(unitOfWork, origemAlteracao) { }

        public FilaCarregamentoVeiculo(Repositorio.UnitOfWorkContainer unitOfWorkContainer, OrigemAlteracaoFilaCarregamento origemAlteracao) : base(unitOfWorkContainer, origemAlteracao) { }

        public FilaCarregamentoVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, OrigemAlteracaoFilaCarregamento origemAlteracao) : base(unitOfWork, origemAlteracao)
        {
            _usuario = usuario;
        }

        public FilaCarregamentoVeiculo(Repositorio.UnitOfWorkContainer unitOfWorkContainer, Dominio.Entidades.Usuario usuario, OrigemAlteracaoFilaCarregamento origemAlteracao) : base(unitOfWorkContainer, origemAlteracao)
        {
            _usuario = usuario;
        }

        #endregion

        #region Métodos Privados

        private void AceitarCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, string descricao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            AceitarCarga(filaCarregamentoVeiculo, descricao, cargaJanelaCarregamento: null, tipoServicoMultisoftware);
        }

        private void AceitarCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, string descricao, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = descricao,
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaAceita
            });

            if (filaCarregamentoVeiculo.ConjuntoVeiculo.IsCompleto())
                AlterarParaEmViagem(filaCarregamentoVeiculo, cargaJanelaCarregamento, tipoServicoMultisoftware);
            else
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos;

            AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.CargaAceita);

            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            repositorioVeiculo.Atualizar(filaCarregamentoVeiculo);
        }

        private void AceitarCargaConjuntoVeiculoIncompleto(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null)
            {
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao;

                AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.CargaAlocada);
            }
            else
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos;

            repositorio.Atualizar(filaCarregamentoVeiculo);

            new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"Carga {ObterNumeroCarga(filaCarregamentoVeiculo.Carga)} aceita pelo portal",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaAceitaPortal
            });

            IntegrarCargaGPA(filaCarregamentoVeiculo);
        }

        private Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo Adicionar(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista, Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista conjuntoMotorista = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista()
            {
                Exclusivo = true,
                FilaCarregamentoMotorista = filaCarregamentoMotorista,
                Motorista = filaCarregamentoMotorista.Motorista
            };
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo = Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo.Criar(filaCarregamentoMotorista.ConjuntoVeiculo);

            ValidarConjuntoVeiculoDisponivelAdicionarFilaCarregamento(conjuntoVeiculo, filaCarregamentoMotorista.CentroCarregamento.Codigo);
            ValidarConjuntoVeiculo(conjuntoVeiculo, filaCarregamentoMotorista.CentroCarregamento.NaoValidarIntegracaoGR);
            ValidarConjuntoVeiculoExclusivoOutroCentroCarregamento(conjuntoVeiculo, filaCarregamentoMotorista.CentroCarregamento);
            ValidarConjuntoVeiculoFilaCarregamentoSimultanea(conjuntoVeiculo);

            int posicao = repositorioFilaCarregamentoVeiculo.BuscarProximaPosicao(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao()
            {
                CodigoCentroCarregamento = filaCarregamentoMotorista.CentroCarregamento.Codigo,
                CodigoModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : filaCarregamentoMotorista.ConjuntoVeiculo.ModeloVeicularCarga.Codigo
            });

            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo()
            {
                CentroCarregamento = filaCarregamentoMotorista.CentroCarregamento,
                ConjuntoMotorista = conjuntoMotorista,
                ConjuntoVeiculo = conjuntoVeiculo,
                DataEntrada = filaCarregamentoMotorista.DataEntrada,
                Posicao = posicao,
                Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel,
                Tipo = tipoRetornoCarga.ObterTipoFilaCarregamentoVeiculo(),
                TipoRetornoCarga = tipoRetornoCarga
            };

            if (IsConjuntoVeiculoEmViagem(conjuntoVeiculo))
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo;
            else if ((filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Vazio) && IsConjuntoVeiculoEmChecklist(conjuntoVeiculo))
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.EmChecklist;

            Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista repositorioFilaCarregamentoConjuntoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo repositorioFilaCarregamentoConjuntoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork);

            repositorioFilaCarregamentoConjuntoMotorista.Inserir(conjuntoMotorista);
            repositorioFilaCarregamentoConjuntoVeiculo.Inserir(conjuntoVeiculo);
            repositorioFilaCarregamentoVeiculo.Inserir(filaCarregamentoVeiculo);

            new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"Entrou na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}{(filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Reversa ? " com reversa" : "")}",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.EntradaFila
            });

            AdicionarFilaCarregamentoVeiculoReversa(filaCarregamentoVeiculo);

            return filaCarregamentoVeiculo;
        }

        private Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo Adicionar(FilaCarregamentoVeiculoDadosAdicionar dadosAdicionar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!dadosAdicionar.ConjuntoVeiculo.IsPermiteEntrarFila())
                throw new ServicoException("O conjunto de veículo não permite entrar na fila");

            if (dadosAdicionar.ConjuntoVeiculo.ModeloVeicularCarga == null)
                throw new ServicoException("O modelo veicular de carga não foi encontrado para o conjunto de veículo");

            if (dadosAdicionar.EmTransicao)
                ValidarConjuntoVeiculoDisponivelAdicionarFilaCarregamentoEmTrasicao(dadosAdicionar.ConjuntoVeiculo);
            else
                ValidarConjuntoVeiculoDisponivelAdicionarFilaCarregamento(dadosAdicionar.ConjuntoVeiculo, dadosAdicionar.CentroCarregamento?.Codigo ?? 0, dadosAdicionar.Filial?.Codigo ?? 0);

            ValidarConjuntoVeiculo(dadosAdicionar.ConjuntoVeiculo, dadosAdicionar.CentroCarregamento?.NaoValidarIntegracaoGR ?? false);
            ValidarConjuntoVeiculoExclusivoOutroCentroCarregamento(dadosAdicionar.ConjuntoVeiculo, dadosAdicionar.CentroCarregamento);
            ValidarConjuntoVeiculoFilaCarregamentoSimultanea(dadosAdicionar.ConjuntoVeiculo);
            ValidarAreaVeiculo(dadosAdicionar.AreaVeiculo);

            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista conjuntoMotorista = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista();

            if (dadosAdicionar.Motorista != null)
            {
                ValidarMotorista(dadosAdicionar.Motorista, dadosAdicionar.CentroCarregamento?.NaoValidarIntegracaoGR ?? false);

                conjuntoMotorista.Motorista = dadosAdicionar.Motorista;
            }

            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo()
            {
                CentroCarregamento = dadosAdicionar.CentroCarregamento,
                ConjuntoMotorista = conjuntoMotorista,
                ConjuntoVeiculo = dadosAdicionar.ConjuntoVeiculo,
                DataEntrada = DateTime.Now,
                DataProgramada = dadosAdicionar.DataProgramada,
                DataProgramadaInicial = dadosAdicionar.DataProgramada,
                Filial = dadosAdicionar.Filial,
                Situacao = dadosAdicionar.EmTransicao ? SituacaoFilaCarregamentoVeiculo.EmTransicao : SituacaoFilaCarregamentoVeiculo.Disponivel,
                Tipo = dadosAdicionar.TipoRetornoCarga.ObterTipoFilaCarregamentoVeiculo(),
                TipoRetornoCarga = dadosAdicionar.TipoRetornoCarga,
                AreaVeiculo = dadosAdicionar.AreaVeiculo,
                Equipamento = dadosAdicionar.Equipamento
            };

            if (!dadosAdicionar.EmTransicao)
                filaCarregamentoVeiculo.Posicao = repositorioFilaCarregamentoVeiculo.BuscarProximaPosicao(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao()
                {
                    CodigoCentroCarregamento = dadosAdicionar.CentroCarregamento?.Codigo ?? 0,
                    CodigoFilial = dadosAdicionar.Filial?.Codigo ?? 0,
                    CodigoModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : dadosAdicionar.ConjuntoVeiculo.ModeloVeicularCarga.Codigo
                });

            if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel)
            {
                if (IsConjuntoVeiculoEmViagem(dadosAdicionar.ConjuntoVeiculo))
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo;
                else if ((filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Vazio) && IsConjuntoVeiculoEmChecklist(dadosAdicionar.ConjuntoVeiculo))
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.EmChecklist;
            }

            ValidarTransportadorIgual(filaCarregamentoVeiculo);
            ValidarEquipamento(filaCarregamentoVeiculo);

            try
            {
                Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista repositorioFilaCarregamentoConjuntoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista(_unitOfWorkContainer.UnitOfWork);
                Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo repositorioFilaCarregamentoConjuntoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork);

                _unitOfWorkContainer.Start();

                if (!dadosAdicionar.EmTransicao && dadosAdicionar.DataProgramada.HasValue)
                {
                    filaCarregamentoVeiculo.ConjuntoVeiculoDedicado = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo(_unitOfWorkContainer.UnitOfWork).ExiteContratosAtivosPorVeiculos(filaCarregamentoVeiculo.ConjuntoVeiculo.ObterCodigos(), filaCarregamentoVeiculo.DataProgramada.Value);

                    int posicao = repositorioFilaCarregamentoVeiculo.BuscarProximaPosicao(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao()
                    {
                        CodigoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0,
                        CodigoFilial = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : filaCarregamentoVeiculo.Filial?.Codigo ?? 0,
                        CodigoModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo,
                        ConjuntoVeiculoDedicado = filaCarregamentoVeiculo.ConjuntoVeiculoDedicado,
                        DataEntrada = filaCarregamentoVeiculo.DataEntrada,
                        DataProgramada = filaCarregamentoVeiculo.DataProgramada
                    });

                    if (filaCarregamentoVeiculo.Posicao != posicao)
                        AlterarPosicao(filaCarregamentoVeiculo, posicao, tipoServicoMultisoftware);
                }

                repositorioFilaCarregamentoConjuntoMotorista.Inserir(conjuntoMotorista);
                repositorioFilaCarregamentoConjuntoVeiculo.Inserir(dadosAdicionar.ConjuntoVeiculo);
                repositorioFilaCarregamentoVeiculo.Inserir(filaCarregamentoVeiculo);

                if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmTransicao)
                    new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = $"Entrou na fila de carregamento em transição{(filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Reversa ? " com reversa" : "")}",
                        FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.EntradaFila
                    });
                else
                    new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = $"Entrou na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}{(filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Reversa ? " com reversa" : "")}",
                        FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.EntradaFila
                    });

                if (dadosAdicionar.CodigosDestino?.Count > 0)
                {
                    Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoDestino repositorioFilaCarregamentoVeiculoDestino = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoDestino(_unitOfWorkContainer.UnitOfWork);

                    foreach (int codigoDestino in dadosAdicionar.CodigosDestino)
                        repositorioFilaCarregamentoVeiculoDestino.Inserir(new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino()
                        {
                            FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                            Localidade = new Dominio.Entidades.Localidade() { Codigo = codigoDestino }
                        });
                }

                if (dadosAdicionar.SiglasEstadoDestino?.Count > 0)
                {
                    Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino repositorioFilaCarregamentoVeiculoEstadoDestino = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino(_unitOfWorkContainer.UnitOfWork);

                    foreach (string siglaEstadoDestino in dadosAdicionar.SiglasEstadoDestino)
                        repositorioFilaCarregamentoVeiculoEstadoDestino.Inserir(new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino()
                        {
                            FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                            Estado = new Dominio.Entidades.Estado() { Sigla = siglaEstadoDestino }
                        });
                }

                if (dadosAdicionar.CodigosRegiaoDestino?.Count > 0)
                {
                    Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino repositorioFilaCarregamentoVeiculoRegiaoDestino = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino(_unitOfWorkContainer.UnitOfWork);

                    foreach (int codigoRegiaoDestino in dadosAdicionar.CodigosRegiaoDestino)
                        repositorioFilaCarregamentoVeiculoRegiaoDestino.Inserir(new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino()
                        {
                            FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                            Regiao = new Dominio.Entidades.Embarcador.Localidades.Regiao() { Codigo = codigoRegiaoDestino }
                        });
                }

                if (dadosAdicionar.CodigosTipoCarga?.Count > 0)
                {
                    Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga repositorioFilaCarregamentoVeiculoTipoCarga = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga(_unitOfWorkContainer.UnitOfWork);

                    foreach (int codigoTipoCarga in dadosAdicionar.CodigosTipoCarga)
                        repositorioFilaCarregamentoVeiculoTipoCarga.Inserir(new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga()
                        {
                            FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                            TipoCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = codigoTipoCarga }
                        });
                }

                AdicionarFilaCarregamentoVeiculoReversa(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();

                NotificarFilaAlterada(filaCarregamentoVeiculo, filaCarregamentoVeiculo.Situacao.ObterTiposFilaCarregamentoAlteracao());

                return filaCarregamentoVeiculo;
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        private void ValidarAreaVeiculo(Dominio.Entidades.Embarcador.Logistica.AreaVeiculo areaVeiculo)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento repositorioConfiguracaoFilaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento configuracaoFilaCarregamento = repositorioConfiguracaoFilaCarregamento.BuscarPrimeiroRegistro();

            if (configuracaoFilaCarregamento.InformarAreaCDAdicionarVeiculo && areaVeiculo == null)
            {
                throw new ServicoException($"Informe a Área do CD.");
            }
        }

        private void AdicionarCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            filaCarregamentoVeiculo.Carga = carga;

            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"Carga {ObterNumeroCarga(carga)} adicionada",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaAdicionada
            });

            if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoCarga)
                AlterarParaEmViagem(filaCarregamentoVeiculo, ObterCargaJanelaCarregamento(filaCarregamentoVeiculo), tipoServicoMultisoftware);

            repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);
        }

        private void AdicionarCargaJanelaCarregamentoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento == null)
                return;

            // TODO: Transportador Terceiro
            // Se o veículo tiver terceiro (pegar pelo veículo) e a flag global estiver ativa, cria pro terceiro

            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(_unitOfWorkContainer.UnitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
            {
                Transportador = cargaJanelaCarregamento.Carga.Empresa,
                CargaJanelaCarregamento = cargaJanelaCarregamento,
                HorarioLiberacao = DateTime.Now,
                Situacao = SituacaoCargaJanelaCarregamentoTransportador.Confirmada,
                Tipo = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCarga
            };

            repositorioCargaJanelaCarregamentoTransportador.Inserir(cargaJanelaCarregamentoTransportador);
            servicoCargaJanelaCarregamentoTransportador.SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga disponibilizada para o transportador");
            servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador);
        }

        private void AdicionarFilaCarregamentoVeiculoReversa(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            FilaCarregamentoVeiculoReversa servicoFilaCarregamentoVeiculoReversa = new FilaCarregamentoVeiculoReversa(_unitOfWorkContainer, _usuario);

            servicoFilaCarregamentoVeiculoReversa.Adicionar(filaCarregamentoVeiculo);
        }

        private void AjustarFluxoGestaoPatioCargaJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento != null)
                new GestaoPatio.FluxoGestaoPatio(_unitOfWorkContainer.UnitOfWork).Adicionar(cargaJanelaCarregamento.CargaBase, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, cargaJanelaCarregamento);
        }

        private void AlterarCentroCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, int codigoCentroCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!IsSituacaoPermiteAlterarCentroCarregamento(filaCarregamentoVeiculo))
                throw new ServicoException("A situação atual não permite alterar o centro de carregamento.");

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = ObterCentroCarregamento(codigoCentroCarregamento);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            try
            {
                _unitOfWorkContainer.Start();

                servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = "Removido da fila de carregamento. Motivo: Alteração de CD",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.SaidaFila
                });

                AtualizarPosicoesFilaCarregamentoVeiculoRemovida(filaCarregamentoVeiculo, tipoServicoMultisoftware);
                ValidarConjuntoVeiculoDisponivelAdicionarFilaCarregamento(filaCarregamentoVeiculo.ConjuntoVeiculo, centroCarregamento.Codigo);
                ValidarConjuntoVeiculo(filaCarregamentoVeiculo.ConjuntoVeiculo, centroCarregamento.NaoValidarIntegracaoGR);
                AtualizarConjuntoVeiculoParaEmViagem(filaCarregamentoVeiculo.ConjuntoVeiculo);

                if (IsConjuntoVeiculoEmViagem(filaCarregamentoVeiculo.ConjuntoVeiculo))
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo;
                else
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

                filaCarregamentoVeiculo.Posicao = repositorioFilaCarregamentoVeiculo.BuscarProximaPosicao(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao()
                {
                    CodigoCentroCarregamento = centroCarregamento.Codigo,
                    CodigoFilaCarregamentoVeiculoDesconsiderar = filaCarregamentoVeiculo.Codigo,
                    CodigoModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo
                });
                filaCarregamentoVeiculo.CentroCarregamento = centroCarregamento;

                servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Adicionado na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}{(filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Reversa ? " com reversa" : "")}",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.EntradaFila
                });
                repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        private void AlterarParaDisponivel(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, int codigoCentroCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!IsSituacaoPermiteAlterarParaDisponivel(filaCarregamentoVeiculo))
                throw new ServicoException("A situação atual não permite a entrada na fila de carregamento.");

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = ObterCentroCarregamento(codigoCentroCarregamento);

            ValidarConjuntoVeiculoDisponivelAdicionarFilaCarregamento(filaCarregamentoVeiculo.ConjuntoVeiculo, centroCarregamento.Codigo);
            ValidarConjuntoVeiculo(filaCarregamentoVeiculo.ConjuntoVeiculo, centroCarregamento.NaoValidarIntegracaoGR);

            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            if (IsConjuntoVeiculoEmViagem(filaCarregamentoVeiculo.ConjuntoVeiculo))
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo;
            else if ((filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Vazio) && IsConjuntoVeiculoEmChecklist(filaCarregamentoVeiculo.ConjuntoVeiculo))
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.EmChecklist;
            else
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

            filaCarregamentoVeiculo.Posicao = repositorioFilaCarregamentoVeiculo.BuscarProximaPosicao(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao()
            {
                CodigoCentroCarregamento = centroCarregamento.Codigo,
                CodigoModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo
            });
            filaCarregamentoVeiculo.CentroCarregamento = centroCarregamento;

            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"Adicionado na fila de carregamento do CD {filaCarregamentoVeiculo.CentroCarregamento.Descricao}{(filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Reversa ? " com reversa" : "")}",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.EntradaFila
            });
            AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.Disponivel);

            repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);
        }

        private void AlterarParaEmTransicao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!IsSituacaoPermiteAlterarParaEmTransicao(filaCarregamentoVeiculo))
                throw new ServicoException("A situação atual não permite alterar a fila para transição.");

            ValidarConjuntoVeiculoDisponivelAdicionarFilaCarregamentoEmTrasicao(filaCarregamentoVeiculo.ConjuntoVeiculo);

            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = "Movido para a fila de carregamento em transição",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.SaidaFila
            });
            AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.Removido);
            AtualizarPosicoesFilaCarregamentoVeiculoRemovida(filaCarregamentoVeiculo, tipoServicoMultisoftware);
            AtualizarConjuntoVeiculoParaEmViagem(filaCarregamentoVeiculo.ConjuntoVeiculo);

            filaCarregamentoVeiculo.CentroCarregamento = null;
            filaCarregamentoVeiculo.Filial = null;
            filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.EmTransicao;
            filaCarregamentoVeiculo.Posicao = 0;

            repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);
        }

        private void AlterarParaEmViagem(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            AlterarParaEmViagem(filaCarregamentoVeiculo, cargaJanelaCarregamento: null, tipoServicoMultisoftware);
        }

        private void AlterarParaEmViagem(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento repositorioConfiguracaoFilaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento(_unitOfWorkContainer.UnitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento configuracaoFilaCarregamento = repositorioConfiguracaoFilaCarregamento.BuscarPrimeiroRegistro();

            if (configuracaoFilaCarregamento.PermiteAvancarPrimeiraEtapaCargaAoAlocarDadosTransportePelaFilaCarregamento)
            {
                AtualizarSituacaoJanelaCarregamento(cargaJanelaCarregamento, SituacaoCargaJanelaCarregamento.ProntaParaCarregamento);
                AdicionarCargaJanelaCarregamentoTransportador(cargaJanelaCarregamento);

                if (
                    ((filaCarregamentoVeiculo.Carga.SituacaoCarga == SituacaoCarga.Nova) && filaCarregamentoVeiculo.Carga.ExigeNotaFiscalParaCalcularFrete) ||
                    ((filaCarregamentoVeiculo.Carga.SituacaoCarga == SituacaoCarga.AgTransportador) && !filaCarregamentoVeiculo.Carga.ExigeNotaFiscalParaCalcularFrete)
                )
                {
                    filaCarregamentoVeiculo.Carga.AguardandoSalvarDadosTransporteCarga = true;
                    new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.Carga);
                }
            }
            else
                AtualizarSituacaoJanelaCarregamento(cargaJanelaCarregamento, SituacaoCargaJanelaCarregamento.AgEncosta);

            AjustarFluxoGestaoPatioCargaJanelaCarregamento(cargaJanelaCarregamento);
            AtualizarConjuntoVeiculoParaEmViagem(filaCarregamentoVeiculo.ConjuntoVeiculo);
            AtualizarPosicoesFilaCarregamentoVeiculoRemovida(filaCarregamentoVeiculo, tipoServicoMultisoftware);

            filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.EmViagem;
            filaCarregamentoVeiculo.Posicao = 0;

            Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(filaCarregamentoVeiculo.Carga, ObterConfiguracaoEmbarcador(), ObterAuditado(), "Iniciado ao selecionar veiculos por fila de carregamento", _unitOfWorkContainer.UnitOfWork);
        }

        private void AlterarParaRemovida(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, int codigoMotivoRetiradaFilaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string observacao)
        {
            Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento motivoRetiradaFilaCarregamento = ObterMotivoRetiradaFilaCarregamento(codigoMotivoRetiradaFilaCarregamento);

            if (!IsSituacaoPermiteAlterarParaRemovida(filaCarregamentoVeiculo))
                throw new ServicoException("A situação atual não permite remover a fila de carregamento.");

            if (filaCarregamentoVeiculo.Carga?.SituacaoCarga.IsSituacaoCargaEmitida() ?? false)
                throw new ServicoException("Situação atual da carga não permite remover a fila de carregamento");

            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"Removido da fila de carregamento. Motivo: {motivoRetiradaFilaCarregamento.Descricao}",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                MotivoRetiradaFilaCarregamento = motivoRetiradaFilaCarregamento,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.SaidaFila,
                Observacao = observacao
            });

            RemoverFilaCarregamento(filaCarregamentoVeiculo, tipoServicoMultisoftware);
        }

        private void AlterarPosicao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, int novaPosicao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            int posicaoAtual = filaCarregamentoVeiculo.Posicao;

            if (posicaoAtual != novaPosicao)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo()
                {
                    CodigoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0,
                    CodigoFilial = filaCarregamentoVeiculo.Filial?.Codigo ?? 0,
                    CodigosModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : new List<int>() { filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo },
                    Situacoes = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesNaFila()
                };

                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = repositorio.BuscarPorPosicaoAjustar(filtrosPesquisa, posicaoAtual, novaPosicao);
                int posicaoAjustar = (posicaoAtual > novaPosicao) ? 1 : -1;

                foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo fila in listaFilaCarregamentoVeiculo)
                {
                    fila.Posicao += posicaoAjustar;

                    repositorio.Atualizar(fila);
                }

                filaCarregamentoVeiculo.Posicao = novaPosicao;
            }
        }

        private void AlterarPosicaoSituacaoFila(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, int ultimaPosicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo situacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string placa)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            int posicaoAtual = filaCarregamentoVeiculo.Posicao;

            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo()
            {
                CodigoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0,
                CodigoFilial = filaCarregamentoVeiculo.Filial?.Codigo ?? 0,
                CodigosModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : new List<int>() { filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo },
                Situacoes = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesNaFila()
            };

            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = repositorio.BuscarPorPosicaoAjustar(filtrosPesquisa, 0, ultimaPosicao);

            foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo fila in listaFilaCarregamentoVeiculo)
            {
                int posicaoAjustar = (ultimaPosicao > fila.Posicao) ? -1 : 1;

                fila.Posicao += posicaoAjustar;

                if (fila.Posicao <= 0)
                    fila.Posicao = 1;

                if (fila.Codigo != filaCarregamentoVeiculo.Codigo)
                {
                    servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = $"Alterado posição na fila de carregamento. Motivo: Veiculo({placa}) realocado na fila ! ",
                        FilaCarregamentoVeiculo = fila,
                        MotivoRetiradaFilaCarregamento = null,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.PosicaoAlterada
                    });
                }
                repositorio.Atualizar(fila);
            }

            filaCarregamentoVeiculo.Posicao = ultimaPosicao;
            filaCarregamentoVeiculo.Situacao = situacao;
            filaCarregamentoVeiculo.DataEntrada = DateTime.Now;
            filaCarregamentoVeiculo.Carga = null;

            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"Veiculo({placa}) realocado na fila de carregamento ",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                MotivoRetiradaFilaCarregamento = null,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.EntradaFila
            });

            repositorio.Atualizar(filaCarregamentoVeiculo);
        }

        private void AlterarPosicaoParaPrimeira(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            AlterarPosicaoSituacaoNaFila(filaCarregamentoVeiculo, novaPosicao: 1, tipoServicoMultisoftware);
        }

        private void AlterarPosicaoParaUltima(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            int novaPosicao = repositorio.BuscarUltimaPosicao(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao()
            {
                CodigoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0,
                CodigoFilial = filaCarregamentoVeiculo.Filial?.Codigo ?? 0,
                CodigoModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo,
                ConjuntoVeiculoDedicado = filaCarregamentoVeiculo.ConjuntoVeiculoDedicado,
                DataProgramada = filaCarregamentoVeiculo.DataProgramada.HasValue ? (DateTime?)filaCarregamentoVeiculo.DataProgramada.Value.Date.Add(DateTime.MaxValue.TimeOfDay) : null,
                DataProgramadaAlteradaAutomaticamente = filaCarregamentoVeiculo.DataProgramadaAlteradaAutomaticamente
            });

            AlterarPosicaoSituacaoNaFila(filaCarregamentoVeiculo, novaPosicao, tipoServicoMultisoftware);
        }

        private void AlterarPosicaoSituacaoNaFila(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, int novaPosicao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            AlterarPosicaoSituacaoNaFila(filaCarregamentoVeiculo, novaPosicao, motivo: null, tipoServicoMultisoftware);
        }

        private void AlterarPosicaoSituacaoNaFila(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, int novaPosicao, Dominio.Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento motivo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string observacao = null)
        {
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            int posicaoAtual = filaCarregamentoVeiculo.Posicao;
            bool isAlterarPosicao = (posicaoAtual != novaPosicao);
            bool isLiberarFilaCarregamento = IsSituacaoPermiteLiberacao(filaCarregamentoVeiculo);

            try
            {
                _unitOfWorkContainer.Start();

                if (isAlterarPosicao)
                {
                    servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = $"Alterada a posição de {posicaoAtual} para {novaPosicao}{(motivo == null ? "" : $". Motivo: {motivo.Descricao}")}",
                        FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                        MotivoAlteracaoPosicaoFilaCarregamento = motivo,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.PosicaoAlterada,
                        Observacao = observacao
                    });

                    AlterarPosicao(filaCarregamentoVeiculo, novaPosicao, tipoServicoMultisoftware);

                    new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo);
                }

                if (IsSituacaoPermiteLiberarMotorista(filaCarregamentoVeiculo))
                {
                    servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = "Motorista liberado pela alteração da posição",
                        FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.MotoristaLiberado
                    });

                    AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.Disponivel);
                }

                if (IsSituacaoPermiteLiberarVeiculo(filaCarregamentoVeiculo))
                    servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = "Veículo liberado pela alteração da posição",
                        FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.VeiculoLiberado
                    });

                if (isLiberarFilaCarregamento)
                {
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

                    new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo);
                }

                _unitOfWorkContainer.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();

                throw;
            }

            if (isAlterarPosicao)
                NotificarFilaAlterada(filaCarregamentoVeiculo);
            else if (isLiberarFilaCarregamento)
                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
        }

        private void AlterarSituacao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, int codigoCentroCarregamento, SituacaoFilaCarregamentoVeiculo novaSituacao, int codigoMotivoRetiradaFilaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string observacao)
        {
            switch (novaSituacao)
            {
                case SituacaoFilaCarregamentoVeiculo.Disponivel:
                    AlterarParaDisponivel(filaCarregamentoVeiculo, codigoCentroCarregamento, tipoServicoMultisoftware);
                    break;

                case SituacaoFilaCarregamentoVeiculo.EmTransicao:
                    AlterarParaEmTransicao(filaCarregamentoVeiculo, tipoServicoMultisoftware);
                    break;

                case SituacaoFilaCarregamentoVeiculo.Removida:
                    AlterarParaRemovida(filaCarregamentoVeiculo, codigoMotivoRetiradaFilaCarregamento, tipoServicoMultisoftware, observacao);
                    break;

                default:
                    throw new ServicoException($"Não é possivel alterar a situação da fila para {novaSituacao.ObterDescricao()}");
            }
        }

        private void AlterarSituacaoFilaCarregamentoMotorista(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista novaSituacao)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoMotorista servicoFilaCarregamentoMotorista = new Servicos.Embarcador.Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer, _origemAlteracao);

            servicoFilaCarregamentoMotorista.AlterarSituacao(filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista, novaSituacao);
        }

        private void AtualizarPedido(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = filaCarregamentoVeiculo.Carga;
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWorkContainer.UnitOfWork);

            if (carga.Pedidos != null && carga.Pedidos.Count > 0)
            {
                foreach (var pedidoCarga in carga.Pedidos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidoCarga.Pedido;
                    if (pedido.Veiculos == null)
                        pedido.Veiculos = new List<Dominio.Entidades.Veiculo>();
                    else
                        pedido.Veiculos.Clear();

                    if (pedido.Motoristas == null)
                        pedido.Motoristas = new List<Dominio.Entidades.Usuario>();

                    pedido.VeiculoTracao = filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao;
                    if (filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques?.Count > 0)
                    {
                        foreach (var veiculo in filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques)
                        {
                            pedido.Veiculos.Add(veiculo);
                        }
                    }

                    if (filaCarregamentoVeiculo.ConjuntoMotorista.IsCompleto())
                    {
                        pedido.Motoristas.Add(filaCarregamentoVeiculo.ConjuntoMotorista.Motorista);
                    }

                    repPedido.Atualizar(pedido);

                    Servicos.Embarcador.Integracao.IntegracaoPedido.ReenviarIntegracaoPedidos(pedido.Codigo, _unitOfWorkContainer.UnitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(ObterAuditado(), pedido, $"Definido o veículo {filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao.Placa_Formatada} para o pedido", _unitOfWorkContainer.UnitOfWork);
                }
            }
        }

        private void AtualizarCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Veiculo.LicencaVeiculo servicoLicencaVeiculo = new Servicos.Embarcador.Veiculo.LicencaVeiculo(_unitOfWorkContainer.UnitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();

            if (filaCarregamentoVeiculo.ConjuntoMotorista.IsCompleto())
            {
                if (tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS && tipoServicoMultisoftware != TipoServicoMultisoftware.TransportadorTerceiro)
                    filaCarregamentoVeiculo.Carga.Empresa = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Empresa;

                filaCarregamentoVeiculo.Carga.Motoristas = new List<Dominio.Entidades.Usuario>() { filaCarregamentoVeiculo.ConjuntoMotorista.Motorista };
            }

            if (tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS && tipoServicoMultisoftware != TipoServicoMultisoftware.TransportadorTerceiro)
            {
                if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                    filaCarregamentoVeiculo.Carga.Empresa = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista?.Empresa ?? filaCarregamentoVeiculo.ConjuntoVeiculo.ObterEmpresa();
            }

            if (filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao != null)
            {
                Repositorio.Embarcador.GestaoPatio.FluxoPatioCancelamento repositorioFluxoPatioCancelamento = new Repositorio.Embarcador.GestaoPatio.FluxoPatioCancelamento(_unitOfWorkContainer.UnitOfWork);

                bool possuiVeiculoBloqueado = repositorioFluxoPatioCancelamento.BuscarSeVeiculoEstaBloqueadoPorCargaVeiculo(filaCarregamentoVeiculo.Carga.Codigo, filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao.Codigo);

                if (possuiVeiculoBloqueado)
                    throw new ServicoException("Esse veículo está bloqueado para está carga.");
            }

            filaCarregamentoVeiculo.Carga.DataAtualizacaoCarga = DateTime.Now;
            filaCarregamentoVeiculo.Carga.Veiculo = filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao;

            if (filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques?.Count > 0)
                filaCarregamentoVeiculo.Carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>(filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques);

            if (tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS && tipoServicoMultisoftware != TipoServicoMultisoftware.TransportadorTerceiro)
            {
                if (filaCarregamentoVeiculo.Carga.CargaAgrupada)
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = repCarga.BuscarCargasOriginais(filaCarregamentoVeiculo.Carga.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargasOrigem)
                    {
                        cargaOrigem.Empresa = filaCarregamentoVeiculo.Carga.Empresa;
                        repCarga.Atualizar(cargaOrigem);
                    }
                }
            }

            servicoLicencaVeiculo.GerarCargaLicenca(filaCarregamentoVeiculo.Carga);

            if (filaCarregamentoVeiculo.Carga.SituacaoCarga == SituacaoCarga.AgTransportador || filaCarregamentoVeiculo.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                filaCarregamentoVeiculo.Carga.CalculandoFrete = true;

            new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.Carga);

            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            {
                AtualizarPedido(filaCarregamentoVeiculo);
            }

            Servicos.Auditoria.Auditoria.Auditar(ObterAuditado(), filaCarregamentoVeiculo.Carga, "Informou dados de transporte via fila de carregamento", _unitOfWorkContainer.UnitOfWork);
        }

        private void AtualizarConjuntoVeiculoParaEmPatio(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo)
        {
            if (conjuntoVeiculo.Reboques?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in conjuntoVeiculo.Reboques)
                    AtualizarVeiculoParaEmPatio(reboque);
            }

            if (conjuntoVeiculo.Tracao != null)
                AtualizarVeiculoParaEmPatio(conjuntoVeiculo.Tracao);
        }

        private void AtualizarConjuntoVeiculoParaEmViagem(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo)
        {
            if (conjuntoVeiculo.Reboques?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in conjuntoVeiculo.Reboques)
                    AtualizarVeiculoParaEmViagem(reboque);
            }

            if (conjuntoVeiculo.Tracao != null)
                AtualizarVeiculoParaEmViagem(conjuntoVeiculo.Tracao);
        }

        private void AtualizarLocalAtual(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo, Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao local)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWorkContainer.UnitOfWork);

            if (conjuntoVeiculo.Tracao != null)
            {
                conjuntoVeiculo.Tracao.LocalAtual = local;

                repositorioVeiculo.Atualizar(conjuntoVeiculo.Tracao);
            }

            if (conjuntoVeiculo.Reboques?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in conjuntoVeiculo.Reboques)
                {
                    reboque.LocalAtual = local;

                    repositorioVeiculo.Atualizar(reboque);
                }
            }
        }

        private void AtualizarMotoristaCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.Carga != null)
            {
                filaCarregamentoVeiculo.Carga.DataAtualizacaoCarga = DateTime.Now;
                filaCarregamentoVeiculo.Carga.Empresa = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Empresa;
                filaCarregamentoVeiculo.Carga.Motoristas = new List<Dominio.Entidades.Usuario>() { filaCarregamentoVeiculo.ConjuntoMotorista.Motorista };

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.Carga);
            }
        }

        private void AtualizarMotoristaPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.PreCarga == null)
                return;

            filaCarregamentoVeiculo.PreCarga.Empresa = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Empresa;
            filaCarregamentoVeiculo.PreCarga.Motoristas = new List<Dominio.Entidades.Usuario>() { filaCarregamentoVeiculo.ConjuntoMotorista.Motorista };

            AtualizarSituacaoPreCarga(filaCarregamentoVeiculo);
        }

        private void AtualizarPosicoesFilaCarregamentoVeiculoRemovida(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo()
            {
                CodigoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0,
                CodigoFilial = filaCarregamentoVeiculo.Filial?.Codigo ?? 0,
                CodigosModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? null : new List<int>() { filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo },
                Situacoes = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesNaFila()
            };

            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculoAjustarPosicao = repositorio.BuscarPorPosicaoSuperior(filtrosPesquisa, filaCarregamentoVeiculo.Posicao);

            foreach (var fila in listaFilaCarregamentoVeiculoAjustarPosicao)
            {
                fila.Posicao -= 1;

                repositorio.Atualizar(fila);
            }
        }

        private void AtualizarPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.PreCarga == null)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();

            if (filaCarregamentoVeiculo.ConjuntoMotorista.IsCompleto())
            {
                filaCarregamentoVeiculo.PreCarga.Empresa = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Empresa;
                filaCarregamentoVeiculo.PreCarga.Motoristas = new List<Dominio.Entidades.Usuario>() { filaCarregamentoVeiculo.ConjuntoMotorista.Motorista };
            }

            if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                filaCarregamentoVeiculo.PreCarga.Empresa = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista?.Empresa ?? filaCarregamentoVeiculo.ConjuntoVeiculo.ObterEmpresa();

            filaCarregamentoVeiculo.PreCarga.Veiculo = filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao;

            if (filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques?.Count > 0)
                filaCarregamentoVeiculo.PreCarga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>(filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques);

            new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.PreCarga);
        }

        private void AtualizarSituacao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos) && (filaCarregamentoVeiculo.IsConjuntosCompletos()))
            {
                if (filaCarregamentoVeiculo.Carga == null)
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoCarga;
                else
                    AlterarParaEmViagem(filaCarregamentoVeiculo, ObterCargaJanelaCarregamento(filaCarregamentoVeiculo), tipoServicoMultisoftware);

                new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo);
            }
        }

        private void AtualizarSituacaoJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, SituacaoCargaJanelaCarregamento situacao)
        {
            if (cargaJanelaCarregamento == null)
                return;

            new CargaJanelaCarregamento(_unitOfWorkContainer.UnitOfWork).AlterarSituacao(cargaJanelaCarregamento, situacao);
        }

        private void AtualizarSituacaoPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.PreCarga == null)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();

            if (!configuracaoGeralCarga.UtilizarProgramacaoCarga)
                return;

            if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel)
                filaCarregamentoVeiculo.PreCarga.SituacaoPreCarga = SituacaoPreCarga.Nova;
            else if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga)
                filaCarregamentoVeiculo.PreCarga.SituacaoPreCarga = SituacaoPreCarga.AguardandoAceite;
            else if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Removida)
                filaCarregamentoVeiculo.PreCarga.SituacaoPreCarga = SituacaoPreCarga.Nova;
            else
            {
                if (filaCarregamentoVeiculo.PreCarga.SituacaoPreCarga == SituacaoPreCarga.AguardandoAceite)
                    filaCarregamentoVeiculo.PreCarga.SituacaoPreCarga = SituacaoPreCarga.AguardandoDadosTransporte;

                if ((filaCarregamentoVeiculo.PreCarga.SituacaoPreCarga == SituacaoPreCarga.AguardandoDadosTransporte) && filaCarregamentoVeiculo.IsConjuntosCompletos())
                    filaCarregamentoVeiculo.PreCarga.SituacaoPreCarga = SituacaoPreCarga.AguardandoGeracaoCarga;

                if ((filaCarregamentoVeiculo.PreCarga.SituacaoPreCarga == SituacaoPreCarga.AguardandoGeracaoCarga) && (filaCarregamentoVeiculo.PreCarga.Carga != null))
                    filaCarregamentoVeiculo.PreCarga.SituacaoPreCarga = SituacaoPreCarga.CargaGerada;
            }

            new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.PreCarga);
        }

        private void AtualizarTracaoCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.Carga != null)
            {
                if (filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao != null)
                {
                    Repositorio.Embarcador.GestaoPatio.FluxoPatioCancelamento repositorioFluxoPatioCancelamento = new Repositorio.Embarcador.GestaoPatio.FluxoPatioCancelamento(_unitOfWorkContainer.UnitOfWork);

                    bool possuiVeiculoBloqueado = repositorioFluxoPatioCancelamento.BuscarSeVeiculoEstaBloqueadoPorCargaVeiculo(filaCarregamentoVeiculo.Carga.Codigo, filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao.Codigo);

                    if (possuiVeiculoBloqueado)
                        throw new ServicoException("Esse veículo está bloqueado para está carga.");
                }

                filaCarregamentoVeiculo.Carga.Veiculo = filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao;

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.Carga);
            }
        }

        private void AtualizarTracaoPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.PreCarga == null)
                return;

            filaCarregamentoVeiculo.PreCarga.Veiculo = filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao;

            AtualizarSituacaoPreCarga(filaCarregamentoVeiculo);
        }

        private void AtualizarVeiculoParaEmPatio(Dominio.Entidades.Veiculo veiculo)
        {
            veiculo.EmViagem = false;

            new Repositorio.Veiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(veiculo);
        }

        private void AtualizarVeiculoParaEmViagem(Dominio.Entidades.Veiculo veiculo)
        {
            veiculo.EmViagem = true;

            new Repositorio.Veiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(veiculo);
        }

        private void CancelarAlteracaoParaEmViagem(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            {

                if (filaCarregamentoVeiculo.ConjuntoVeiculo != null && filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao != null)
                    RealocarVeiculoNaFila(filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao.Codigo, tipoServicoMultisoftware, situacao: SituacaoFilaCarregamentoVeiculo.CargaCancelada);

                if (filaCarregamentoVeiculo.ConjuntoVeiculo != null && filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques.Count() > 0)
                {
                    foreach (var veiculo in filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques)
                    {
                        RealocarVeiculoNaFila(veiculo.Codigo, tipoServicoMultisoftware, situacao: SituacaoFilaCarregamentoVeiculo.CargaCancelada);
                    }
                }

                AtualizarConjuntoVeiculoParaEmPatio(filaCarregamentoVeiculo.ConjuntoVeiculo);
            }
            else
            {

                if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem)
                {
                    AtualizarConjuntoVeiculoParaEmPatio(filaCarregamentoVeiculo.ConjuntoVeiculo);

                    filaCarregamentoVeiculo.Posicao = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).BuscarProximaPosicao(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao()
                    {
                        CodigoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0,
                        CodigoFilaCarregamentoVeiculoDesconsiderar = filaCarregamentoVeiculo.Codigo,
                        CodigoFilial = filaCarregamentoVeiculo.Filial?.Codigo ?? 0,
                        CodigoModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo
                    });

                    AlterarPosicao(filaCarregamentoVeiculo, novaPosicao: 1, tipoServicoMultisoftware);
                }
            }


        }

        private void ConfirmarChegadaConjuntoVeiculo(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            AtualizarConjuntoVeiculoParaEmPatio(filaCarregamentoVeiculo.ConjuntoVeiculo);

            if ((filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Vazio) && IsConjuntoVeiculoEmChecklist(filaCarregamentoVeiculo.ConjuntoVeiculo))
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.EmChecklist;
            else
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

            new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo);

            new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = (filaCarregamentoVeiculo.ConjuntoVeiculo.ObterTotalPlacas() > 1) ? $"Chegada de veículos confirmada. Placas: {filaCarregamentoVeiculo.ConjuntoVeiculo.ObterPlacas()}" : $"Chegada de veículo confirmada. Placa: {filaCarregamentoVeiculo.ConjuntoVeiculo.ObterPlacas()}",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.ChegadaVeiculoConfirmada
            });
        }

        private void DesalocarCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            CancelarAlteracaoParaEmViagem(filaCarregamentoVeiculo, tipoServicoMultisoftware);

            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico historico = servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"Carga {ObterNumeroCarga(filaCarregamentoVeiculo.Carga)} removida",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaRemovida
            });

            filaCarregamentoVeiculo.Carga = null;

            if ((filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem) || (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga))
                filaCarregamentoVeiculo.Situacao = (filaCarregamentoVeiculo.PreCarga != null) ? SituacaoFilaCarregamentoVeiculo.AguardandoCarga : SituacaoFilaCarregamentoVeiculo.Disponivel;

            repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);
            NotificarMobile(filaCarregamentoVeiculo, TipoNotificacaoMobile.CargaRemovida, historico.Descricao);
        }

        private void DesalocarCargaEPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico ultimoHistorico = null;

            if (filaCarregamentoVeiculo.Carga != null)
            {
                CancelarAlteracaoParaEmViagem(filaCarregamentoVeiculo, tipoServicoMultisoftware);

                ultimoHistorico = servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Carga {ObterNumeroCarga(filaCarregamentoVeiculo.Carga)} removida",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaRemovida
                });

                filaCarregamentoVeiculo.Carga = null;
            }

            filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

            if (filaCarregamentoVeiculo.PreCarga != null)
            {
                ultimoHistorico = servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Pré planejamento {filaCarregamentoVeiculo.PreCarga.NumeroPreCarga} desalocado",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.PreCargaDesalocada
                });

                AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.Disponivel);
                AtualizarSituacaoPreCarga(filaCarregamentoVeiculo);

                filaCarregamentoVeiculo.PreCarga = null;
            }

            repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);

            if (ultimoHistorico != null)
                NotificarMobile(filaCarregamentoVeiculo, ((ultimoHistorico.Tipo == TipoFilaCarregamentoVeiculoHistorico.CargaRemovida) ? TipoNotificacaoMobile.CargaRemovida : TipoNotificacaoMobile.PreCargaDesalocada), ultimoHistorico.Descricao);
        }

        private void DesatrelarVeiculo(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado filaCarregamentoVeiculoAtrelado)
        {
            if (filaCarregamentoVeiculoAtrelado == null)
                throw new ServicoException("Não foi possível encontrar o registro de veículo atrelado.");

            if (filaCarregamentoVeiculoAtrelado.FilaCarregamentoVeiculo.Situacao != SituacaoFilaCarregamentoVeiculo.ReboqueAtrelado)
                throw new ServicoException("A situação atual não permite remover o veículo atrelado.");

            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            FilaCarregamentoMotorista servicoFilaCarregamentoMotorista = new FilaCarregamentoMotorista(_unitOfWorkContainer, _origemAlteracao);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            servicoFilaCarregamentoMotorista.AlterarSituacao(filaCarregamentoVeiculoAtrelado.FilaCarregamentoMotorista, SituacaoFilaCarregamentoMotorista.Disponivel);

            if ((filaCarregamentoVeiculoAtrelado.FilaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Vazio) && IsConjuntoVeiculoEmChecklist(filaCarregamentoVeiculoAtrelado.FilaCarregamentoVeiculo.ConjuntoVeiculo))
                filaCarregamentoVeiculoAtrelado.FilaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.EmChecklist;
            else
                filaCarregamentoVeiculoAtrelado.FilaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

            repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculoAtrelado.FilaCarregamentoVeiculo);

            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"Tração {filaCarregamentoVeiculoAtrelado.FilaCarregamentoMotorista.ConjuntoVeiculo.Tracao.Placa_Formatada} desatrelada",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculoAtrelado.FilaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.VeiculoDesatrelado
            });
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga DesvincularCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, int codigoCarga = 0)
        {
            if (filaCarregamentoVeiculo.Carga == null)
                throw new ServicoException("Nenhuma carga encontrada");

            Dominio.Entidades.Embarcador.Cargas.Carga cargaDesvinculada = filaCarregamentoVeiculo.Carga;

            if (cargaDesvinculada.Codigo == codigoCarga)
            {
                filaCarregamentoVeiculo.Carga = null;
                return cargaDesvinculada;
            }

            Servicos.Log.GravarInfo($"Removido dados da carga da fila de carregamento Codigo {filaCarregamentoVeiculo.Codigo} - Carga {cargaDesvinculada.Codigo} - Veiculo {cargaDesvinculada.Veiculo?.Codigo} - Empresa {cargaDesvinculada.Empresa?.Codigo}. Ao atualizar a carga Codigo {codigoCarga}", "RemocaoFilaCarregamento");

            cargaDesvinculada.Empresa = null;
            cargaDesvinculada.Motoristas = null;
            cargaDesvinculada.Veiculo = null;
            cargaDesvinculada.VeiculosVinculados = null;

            new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork).Atualizar(cargaDesvinculada);

            AtualizarSituacaoJanelaCarregamento(ObterCargaJanelaCarregamentoPorCarga(cargaDesvinculada), SituacaoCargaJanelaCarregamento.SemTransportador);

            filaCarregamentoVeiculo.Carga = null;

            return cargaDesvinculada;
        }

        private Dominio.Entidades.Embarcador.PreCargas.PreCarga DesvincularPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.PreCarga == null)
                throw new ServicoException("Nenhum pré planejamento encontrado");

            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCargaDesvinculada = filaCarregamentoVeiculo.PreCarga;

            preCargaDesvinculada.Empresa = null;
            preCargaDesvinculada.Motoristas = null;
            preCargaDesvinculada.Veiculo = null;
            preCargaDesvinculada.VeiculosVinculados = null;

            AtualizarSituacaoPreCarga(filaCarregamentoVeiculo);

            filaCarregamentoVeiculo.PreCarga = null;

            return preCargaDesvinculada;
        }

        private void FinalizarPorVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarEmViagemPorVeiculo(veiculo.Codigo);

            foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in filasCarregamentoVeiculo)
                Finalizar(filaCarregamentoVeiculo, descricaoHistorico: "Fila de carregamento encerrada");
        }

        private void IntegrarCargaGPA(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioIntegracaoCarga = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWorkContainer.UnitOfWork);

            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaIntegracao integracao = repCargaIntegracao.BuscarPorCargaETipo(filaCarregamentoVeiculo.Carga.Codigo, TipoIntegracao.GPA);


            if (integracao != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao();

                cargaIntegracao.Carga = filaCarregamentoVeiculo.Carga;
                cargaIntegracao.DataIntegracao = DateTime.Now;
                cargaIntegracao.NumeroTentativas = 0;
                cargaIntegracao.ProblemaIntegracao = "";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                cargaIntegracao.TipoIntegracao = integracao.TipoIntegracao;

                repositorioIntegracaoCarga.Inserir(cargaIntegracao);
            }
        }

        private void LiberarMotorista(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            try
            {
                _unitOfWorkContainer.Start();

                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

                new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo);

                if (IsSituacaoPermiteLiberarMotorista(filaCarregamentoVeiculo))
                    AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.Disponivel);

                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = "Motorista liberado",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.MotoristaLiberado
                });

                _unitOfWorkContainer.CommitChanges();

                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();

                throw;
            }
        }

        private void LiberarVeiculo(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            try
            {
                _unitOfWorkContainer.Start();

                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

                repositorioVeiculo.Atualizar(filaCarregamentoVeiculo);

                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = "Veículo liberado",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.VeiculoLiberado
                });

                _unitOfWorkContainer.CommitChanges();

                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();

                throw;
            }
        }

        private void RemoverConjuntoMotorista(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (!filaCarregamentoVeiculo.ConjuntoMotorista.IsCompleto())
                throw new ServicoException("O conjunto do motorista não foi informado");

            if (filaCarregamentoVeiculo.ConjuntoMotorista.Exclusivo)
                throw new ServicoException("A fila de carregamento é exclusiva do motorista. Não é possível alterar o motorista");

            if (filaCarregamentoVeiculo.Carga != null)
                throw new ServicoException("A fila de carregamento não permite remover o conjunto do motorista com carga alocada");

            if (filaCarregamentoVeiculo.PreCarga != null)
                throw new ServicoException("A fila de carregamento não permite remover o conjunto do motorista com pré planejamento alocado");

            FilaCarregamentoVeiculoHistorico servicoFilaCarregamentoVeiculoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null)
            {
                if (
                    (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.ConjuntoVeiculo?.Tracao != null) &&
                    (filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Codigo == filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.ConjuntoVeiculo.Tracao.Codigo)
                )
                {
                    filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao = null;

                    new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.ConjuntoVeiculo);

                    servicoFilaCarregamentoVeiculoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = $"A tração {filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.ConjuntoVeiculo.Tracao.Placa_Formatada} foi removida",
                        FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado
                    });

                    RemoverTracaoCarga(filaCarregamentoVeiculo);
                    RemoverTracaoPreCarga(filaCarregamentoVeiculo);
                }

                AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.Disponivel);
            }

            servicoFilaCarregamentoVeiculoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"O motorista {filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Nome} ({filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.CPF_Formatado}) foi removido",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoMotoristaAlterado
            });

            RemoverMotoristaCarga(filaCarregamentoVeiculo);
            RemoverMotoristaPreCarga(filaCarregamentoVeiculo);

            filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista = null;
            filaCarregamentoVeiculo.ConjuntoMotorista.Motorista = null;

            new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.ConjuntoMotorista);
        }

        public void RemoverFilaCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoCarga = 0)
        {
            AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.Removido);

            if (SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesNaFila().Contains(filaCarregamentoVeiculo.Situacao))
                AtualizarPosicoesFilaCarregamentoVeiculoRemovida(filaCarregamentoVeiculo, tipoServicoMultisoftware);

            filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Removida;
            filaCarregamentoVeiculo.Posicao = 0;

            if (filaCarregamentoVeiculo.PreCarga != null)
                DesvincularPreCarga(filaCarregamentoVeiculo);

            if (filaCarregamentoVeiculo.Carga != null)
                DesvincularCarga(filaCarregamentoVeiculo, codigoCarga);

            new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo);
        }

        private void RemoverMotoristaCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.Carga != null)
            {
                filaCarregamentoVeiculo.Carga.DataAtualizacaoCarga = DateTime.Now;
                filaCarregamentoVeiculo.Carga.Empresa = null;
                filaCarregamentoVeiculo.Carga.Motoristas.Remove(filaCarregamentoVeiculo.ConjuntoMotorista.Motorista);

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.Carga);
            }
        }

        private void RemoverMotoristaPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.PreCarga != null)
            {
                filaCarregamentoVeiculo.PreCarga.Empresa = null;
                filaCarregamentoVeiculo.PreCarga.Motoristas.Remove(filaCarregamentoVeiculo.ConjuntoMotorista.Motorista);

                new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.PreCarga);
            }
        }

        private void RemoverTracaoCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.Carga != null)
            {
                filaCarregamentoVeiculo.Carga.Veiculo = null;

                new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.Carga);
            }
        }

        private void RemoverTracaoPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.PreCarga != null)
            {
                filaCarregamentoVeiculo.PreCarga.Veiculo = null;

                new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.PreCarga);
            }
        }

        private void ValidarConjuntoVeiculoDisponivelAdicionarFilaCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo, int codigoCentroCarregamento)
        {
            ValidarConjuntoVeiculoDisponivelAdicionarFilaCarregamento(conjuntoVeiculo, codigoCentroCarregamento, codigoFilial: 0);
        }

        private void ValidarConjuntoVeiculoDisponivelAdicionarFilaCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo, int codigoCentroCarregamento, int codigoFilial)
        {
            if (conjuntoVeiculo.Reboques?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in conjuntoVeiculo.Reboques)
                    ValidarVeiculoDisponivelAdicionarFilaCarregamento(reboque, codigoCentroCarregamento, codigoFilial);
            }
            else
                ValidarVeiculoDisponivelAdicionarFilaCarregamento(conjuntoVeiculo.Tracao, codigoCentroCarregamento, codigoFilial);
        }

        private void ValidarEquipamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (!(filaCarregamentoVeiculo.Filial?.InformarEquipamentoFluxoPatio ?? false))
                return;

            if (filaCarregamentoVeiculo.Equipamento == null)
                throw new ServicoException("Favor preencher o equipamento");

            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            bool equipamentoEmUso = repositorioFilaCarregamentoVeiculo.ExisteEquipamentoEmUsoFilaCarregamento(filaCarregamentoVeiculo.Equipamento.Codigo);

            if (equipamentoEmUso)
                throw new ServicoException("Equipamento já está sendo utilizado em outro veículo");
        }

        private void ValidarTransportadorIgual(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();

            if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                return;

            Dominio.Entidades.Empresa transportadorMotorista = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista?.Empresa;
            Dominio.Entidades.Empresa transportadorTracao = filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Empresa;
            Dominio.Entidades.Empresa transportadorReboques = filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques?.FirstOrDefault()?.Empresa;

            if ((transportadorMotorista != null) && (transportadorTracao != null) && (transportadorMotorista.Codigo != transportadorTracao.Codigo))
                throw new ServicoException("Transportador do motorista e da tração diferentes");

            if ((transportadorMotorista != null) && (transportadorReboques != null) && (transportadorMotorista.Codigo != transportadorReboques.Codigo))
                throw new ServicoException($"Transportador do motorista e {(filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques.Count == 1 ? "do reboque" : "dos reboques")} diferentes");

            if ((transportadorTracao != null) && (transportadorReboques != null) && (transportadorTracao.Codigo != transportadorReboques.Codigo))
                throw new ServicoException($"Transportador da tração e {(filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques.Count == 1 ? "do reboque" : "dos reboques")} diferentes");
        }

        private void ValidarConjuntoVeiculoDisponivelAdicionarFilaCarregamentoEmTrasicao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo)
        {
            if (conjuntoVeiculo.Reboques?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in conjuntoVeiculo.Reboques)
                    ValidarVeiculoDisponivelAdicionarFilaCarregamentoEmTrasicao(reboque);
            }
            else
                ValidarVeiculoDisponivelAdicionarFilaCarregamentoEmTrasicao(conjuntoVeiculo.Tracao);
        }

        private void ValidarVeiculoDisponivelAdicionarFilaCarregamento(Dominio.Entidades.Veiculo veiculo, int codigoCentroCarregamento, int codigoFilial)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarAtivaPorVeiculo(veiculo.Codigo, codigoCentroCarregamento, codigoFilial, somenteSemCargaOuPreCargaAlocada: configuracaoGeralCarga.UtilizarProgramacaoCarga);

            if (filaCarregamentoVeiculo != null)
            {
                string descricaoTipo = veiculo.IsTipoVeiculoTracao() ? "A tração" : "O reboque";

                throw new ServicoException(ObterDescricaoIndisponivelAdicionarFilaCarregamento(filaCarregamentoVeiculo, descricaoTipo));
            }
        }

        private void ValidarVeiculoDisponivelAdicionarFilaCarregamentoEmTrasicao(Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarEmTransicaoPorVeiculo(veiculo.Codigo);

            if (filaCarregamentoVeiculo != null)
                throw new ServicoException($"{(veiculo.IsTipoVeiculoTracao() ? "A tração" : "O reboque")} {veiculo.Placa_Formatada} já está na fila de carregamento em transição.");
        }
        private void ValidarConjuntoVeiculoFilaCarregamentoSimultanea(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento repConfiguracaoFilaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento configuracaoFilaCarregamento = repConfiguracaoFilaCarregamento.BuscarPrimeiroRegistro();
            if (configuracaoFilaCarregamento?.NaoPermitirAdicionarVeiculoEmMaisDeUmaFilaCarregamentoSimultaneamente ?? false)
            {
                if (conjuntoVeiculo.Reboques?.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in conjuntoVeiculo.Reboques)
                        ValidarVeiculoFilaCarregamentoSimultanea(reboque);
                }
                else
                    ValidarVeiculoFilaCarregamentoSimultanea(conjuntoVeiculo.Tracao);
            }
        }
        private void ValidarVeiculoFilaCarregamentoSimultanea(Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.ExisteFilaCarregamentoVeiculoSituacao(veiculo.Codigo, new SituacaoFilaCarregamentoVeiculo[] {SituacaoFilaCarregamentoVeiculo.Finalizada,
                                                                                                                                                                                                                SituacaoFilaCarregamentoVeiculo.CargaCancelada,
                                                                                                                                                                                                                SituacaoFilaCarregamentoVeiculo.Removida });
            if (filaCarregamentoVeiculo != null)
            {
                string descricaoTipo = veiculo.IsTipoVeiculoTracao() ? "A tração" : "O reboque";
                throw new ServicoException(ObterDescricaoIndisponivelAdicionarFilaCarregamento(filaCarregamentoVeiculo, descricaoTipo));

            }
        }
        private void VincularCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            VincularCarga(filaCarregamentoVeiculo, carga, observacaoAlocacao: null, AtualizarFilaCarregamentoVeiculo: null, tipoServicoMultisoftware);
        }

        private void VincularCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga, string observacaoAlocacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            VincularCarga(filaCarregamentoVeiculo, carga, observacaoAlocacao, AtualizarFilaCarregamentoVeiculo: null, tipoServicoMultisoftware);
        }

        private void VincularCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga, Action<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> AtualizarFilaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            VincularCarga(filaCarregamentoVeiculo, carga, observacaoAlocacao: null, AtualizarFilaCarregamentoVeiculo: AtualizarFilaCarregamentoVeiculo, tipoServicoMultisoftware);
        }

        private void VincularCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga, string observacaoAlocacao, Action<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> AtualizarFilaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            try
            {
                _unitOfWorkContainer.Start();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = ObterCargaJanelaCarregamentoPorCarga(carga);

                AtualizarFilaCarregamentoVeiculo?.Invoke(filaCarregamentoVeiculo);

                filaCarregamentoVeiculo.Carga = carga;

                if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.CargaCancelada)
                    filaCarregamentoVeiculo.PreCarga = null;

                AtualizarCarga(filaCarregamentoVeiculo, tipoServicoMultisoftware);
                AtualizarSituacaoJanelaCarregamento(cargaJanelaCarregamento, SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador);

                string numeroCarga = ObterNumeroCarga(carga);

                servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Carga {numeroCarga} alocada{(string.IsNullOrWhiteSpace(observacaoAlocacao) ? "" : $". Observação: {observacaoAlocacao}")}",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaAlocada
                });

                if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                {
                    if (filaCarregamentoVeiculo.PreCarga == null)
                        filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga;
                }
                else if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null)
                {
                    if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.CargaCancelada)
                    {
                        AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.CargaAceita);

                        servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                        {
                            Descricao = $"Carga {numeroCarga} aceita automaticamente",
                            FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                            OrigemAlteracao = _origemAlteracao,
                            Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaAceita
                        });

                        if (filaCarregamentoVeiculo.IsConjuntosCompletos())
                            AlterarParaEmViagem(filaCarregamentoVeiculo, cargaJanelaCarregamento, tipoServicoMultisoftware);
                        else
                            filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos;

                        IntegrarCargaGPA(filaCarregamentoVeiculo);
                    }
                    else
                    {
                        filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao;

                        AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.CargaAlocada);
                    }
                }
                else if (filaCarregamentoVeiculo.IsConjuntosCompletos())
                {
                    servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = $"Carga {numeroCarga} confirmada para o motorista",
                        FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaAceita
                    });
                    AlterarParaEmViagem(filaCarregamentoVeiculo, cargaJanelaCarregamento, tipoServicoMultisoftware);
                }
                else if (!filaCarregamentoVeiculo.ConjuntoVeiculo.IsCompleto())
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga;
                else
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos;

                new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();

                NotificarMotoristaCargaAlocada(filaCarregamentoVeiculo);
                NotificarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem)
                    NotificarFilaAlterada(filaCarregamentoVeiculo);
                else
                    NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        private void VincularPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!IsSituacaoPermiteAlocarCargaOuPreCarga(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite alocar o pré planejamento");

            AtualizarSituacaoJanelaCarregamento(ObterCargaJanelaCarregamentoPorPreCarga(preCarga), SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = ObterConfiguracaoPreCarga();
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            filaCarregamentoVeiculo.PreCarga = preCarga;

            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"Pré planejamento {preCarga.NumeroPreCarga} alocado",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.PreCargaAlocada
            });

            if (configuracaoGeralCarga.UtilizarProgramacaoCarga && !configuracaoPreCarga.AceitarVinculoFilaCarregamentoVeiculoAutomaticamente)
            {
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga;
                AtualizarSituacaoPreCarga(filaCarregamentoVeiculo);
            }
            else
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos;

            AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.PreCargaAlocada);
            AtualizarSituacao(filaCarregamentoVeiculo, tipoServicoMultisoftware);

            new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo);

            NotificarMobile(filaCarregamentoVeiculo, TipoNotificacaoMobile.PreCargaAlocada, $"Pré planejamento {preCarga.NumeroPreCarga} alocado");
        }

        #endregion

        #region Métodos Privados de Consulta

        private bool IsAvancarEtapaGestaoPatio(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);

            return fluxoGestaoPatio?.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.InformarDoca;
        }

        private bool IsConjuntoVeiculoEmChecklist(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo)
        {
            if (conjuntoVeiculo.Reboques?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in conjuntoVeiculo.Reboques)
                {
                    if (IsVeiculoEmChecklist(reboque))
                        return true;
                }
            }

            if (conjuntoVeiculo.Tracao != null)
                return IsVeiculoEmChecklist(conjuntoVeiculo.Tracao);

            return false;
        }

        private bool IsConjuntoVeiculoEmViagem(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo)
        {
            if (!IsUtilizarControleVeiculoEmPatio())
                return false;

            if (conjuntoVeiculo.Reboques?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in conjuntoVeiculo.Reboques)
                {
                    if (reboque.EmViagem)
                        return true;
                }
            }

            return conjuntoVeiculo.Tracao?.EmViagem ?? false;
        }

        private bool IsMotoristaPerdeuSenhaCargaAlocada(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico ultimoHistorico = servicoHistorico.ObterUltimoPorTipo(filaCarregamentoVeiculo.Codigo, TipoFilaCarregamentoVeiculoHistorico.CargaAlocada);

            return (
                (ultimoHistorico != null) &&
                (ultimoHistorico.Data < DateTime.Now.AddMinutes(-ultimoHistorico.FilaCarregamentoVeiculo.CentroCarregamento?.TempoAguardarConfirmacaoTransportador ?? 0))
            );
        }

        private bool IsPossuiRestricaoRodagem(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataCarregamento)
        {
            List<Dominio.Entidades.Cliente> clientesDestino = (from cargaPedido in carga.Pedidos select cargaPedido.Pedido.Destinatario).ToList();

            return IsPossuiRestricaoRodagem(filaCarregamentoVeiculo, dataCarregamento, clientesDestino);
        }

        private bool IsPossuiRestricaoRodagem(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, DateTime dataCarregamento)
        {
            List<Dominio.Entidades.Cliente> clientesDestino = (from pedido in preCarga.Pedidos select pedido.Destinatario).ToList();

            return IsPossuiRestricaoRodagem(filaCarregamentoVeiculo, dataCarregamento, clientesDestino);
        }

        private bool IsPossuiRestricaoRodagem(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, DateTime dataCarregamento, List<Dominio.Entidades.Cliente> clientesDestino)
        {
            if (filaCarregamentoVeiculo.CentroCarregamento == null)
                return false;

            RestricaoRodagem servicoRestricaoRodagem = new RestricaoRodagem(_unitOfWorkContainer.UnitOfWork);

            if (filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao != null)
            {
                if (servicoRestricaoRodagem.IsPossuiRestricao(filaCarregamentoVeiculo.CentroCarregamento.Codigo, dataCarregamento, filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao.Placa, clientesDestino))
                    return true;
            }

            foreach (Dominio.Entidades.Veiculo reboque in filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques)
            {
                if (servicoRestricaoRodagem.IsPossuiRestricao(filaCarregamentoVeiculo.CentroCarregamento.Codigo, dataCarregamento, reboque.Placa, clientesDestino))
                    return true;
            }

            return false;
        }

        private bool IsSituacaoPermiteAceitarOuRecusarCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao) &&
                (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista?.Situacao == SituacaoFilaCarregamentoMotorista.CargaAlocada)
            );
        }

        private bool IsSituacaoPermiteAceitarOuRecusarCargaManualmente(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga);
        }

        private bool IsSituacaoPermiteAceitarOuRecusarPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga);
        }

        private bool IsSituacaoPermiteAceitarOuRecusarSaidaFila(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmRemocao);
        }

        private bool IsSituacaoPermiteAlocarCargaOuPreCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) &&
                (
                    (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista == null) ||
                    (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel)
                )
            );
        }

        private bool IsSituacaoPermiteAlterarCentroCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo) ||
                (
                    (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) &&
                    (
                        (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista == null) ||
                        (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel)
                    )
                )
            );
        }

        private bool IsSituacaoPermiteAlterarParaDisponivel(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmTransicao);
        }

        private bool IsSituacaoPermiteAlterarParaEmTransicao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo) ||
                (
                    (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) &&
                    (
                        (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista == null) ||
                        (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel)
                    )
                )
            );
        }

        private bool IsSituacaoPermiteAlterarParaRemovida(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return ((filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmTransicao) || IsSituacaoPermiteRemoverFilaCarregamento(filaCarregamentoVeiculo));
        }

        private bool IsSituacaoPermiteAlterarPosicao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.CargaCancelada) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmChecklist)
            )
                return true;

            if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel)
            {
                if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null)
                    return (
                        (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.CargaRecusada) ||
                        (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.SenhaPerdida) ||
                        (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel)
                    );

                return true;
            }

            return false;
        }

        private bool IsSituacaoPermiteConfirmarChegadaVeiculo(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo);
        }

        private bool IsSituacaoPermiteInformarDoca(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem;
        }

        private bool IsSituacaoPermiteInformarMotorista(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoCarga) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.CargaCancelada) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmChecklist)
            );
        }

        private bool IsSituacaoPermiteInformarTracao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoCarga) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.CargaCancelada) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel)
            );
        }

        private bool IsSituacaoPermiteLiberacao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.CargaCancelada) ||
                (
                    (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) &&
                    (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null) &&
                    SituacaoFilaCarregamentoMotoristaHelper.ObterSituacoesBloqueadas().Contains(filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao)
                )
            );
        }

        private bool IsSituacaoPermiteLiberarVeiculo(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado);
        }

        private bool IsSituacaoPermiteLiberarMotorista(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if ((filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null) && (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.CargaCancelada))
                return true;

            return (
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) &&
                (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null) &&
                SituacaoFilaCarregamentoMotoristaHelper.ObterSituacoesBloqueadas().Contains(filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao)
            );
        }

        private bool IsSituacaoPermiteRemoverFilaCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoCarga) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos) ||
                (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.CargaCancelada) ||
                (
                    (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) &&
                    (
                        (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista == null) ||
                        (
                            (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel) ||
                            (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.CargaAlocada) || //Tratativa para situação onde Fila Veiculo ficou como Disponivel e Fila Motorista com Carga Alocada
                            SituacaoFilaCarregamentoMotoristaHelper.ObterSituacoesBloqueadas().Contains(filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao)
                        )
                    )
                )
            );
        }

        private bool IsSituacaoPermiteRemoverReversa(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (
                (
                    (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo) ||
                    (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel)
                ) &&
                (filaCarregamentoVeiculo.Tipo == TipoFilaCarregamentoVeiculo.Reversa)
            );
        }

        private bool IsSituacaoPermiteVisualizarDetalhesCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.CargaAlocada);
        }

        private bool IsUtilizarControleVeiculoEmPatio()
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            return configuracaoEmbarcador?.UtilizarControleVeiculoEmPatio ?? false;
        }

        private bool IsVeiculoEmChecklist(Dominio.Entidades.Veiculo veiculo)
        {
            if ((veiculo.ModeloVeicularCarga == null) || (veiculo.ModeloVeicularCarga.DiasRealizarProximoChecklist == 0))
                return false;

            return (!veiculo.DataValidadeGerenciadoraRisco.HasValue || (veiculo.DataValidadeGerenciadoraRisco.Value < DateTime.Now.Date));
        }

        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado ObterAuditado()
        {
            if (_usuario == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.Usuario = _usuario;
            auditado.Empresa = _usuario.Empresa;
            auditado.OrigemAuditado = OrigemAuditado.Sistema;
            auditado.TipoAuditado = TipoAuditado.Sistema;
            auditado.Texto = "";


            return auditado;
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento ObterCargaJanelaCarregamentoPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                return null;

            return new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWorkContainer.UnitOfWork).BuscarPorCarga(carga.Codigo);
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento ObterCargaJanelaCarregamentoPorPreCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            if (preCarga == null)
                return null;

            return new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWorkContainer.UnitOfWork).BuscarPorPreCarga(preCarga.Codigo);
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroCarregamento ObterCentroCarregamento(int codigo)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWorkContainer.UnitOfWork);

            return repositorioCentroCarregamento.BuscarPorCodigo(codigo) ?? throw new ServicoException("Centro de carregamento não encontrado.");
        }

        private string ObterDescricaoIndisponivelAdicionarFilaCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, string descricaotipo)
        {
            switch (filaCarregamentoVeiculo.Situacao)
            {
                case SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga:
                    return $"{descricaotipo} já está na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()} aguardando o aceite da carga.";

                case SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao:
                    return $"{descricaotipo} já está na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()} aguardando confirmação do motorista.";

                case SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos:
                    return $"{descricaotipo} já está na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()} aguardando conjuntos.";

                case SituacaoFilaCarregamentoVeiculo.CargaCancelada:
                    return $"{descricaotipo} já está na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()} com carga cancelada.";

                case SituacaoFilaCarregamentoVeiculo.EmChecklist:
                    return $"{descricaotipo} já está na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()} em checklist.";

                case SituacaoFilaCarregamentoVeiculo.EmRemocao:
                    return $"{descricaotipo} já está na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()} aguardando liberação para sair da fila.";

                case SituacaoFilaCarregamentoVeiculo.EmViagem:
                    return $"{descricaotipo} ainda está em viagem com a carga {ObterNumeroCarga(filaCarregamentoVeiculo.Carga)}.";

                case SituacaoFilaCarregamentoVeiculo.AguardandoCarga:
                    return $"{descricaotipo} já está na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()} com o pré planejamento {filaCarregamentoVeiculo.PreCarga.NumeroPreCarga} aguardando a carga.";

                default:
                    return $"{descricaotipo} já está na {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}.";
            }
        }

        private Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo ObterFilaCarregamentoConjuntoVeiculo(int codigoVeiculo)
        {
            Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorCodigo(codigoVeiculo);

            return Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo.Criar(veiculo);
        }

        private Dominio.Entidades.Embarcador.Filiais.Filial ObterFilial(int codigo)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWorkContainer.UnitOfWork);

            return repositorioFilial.BuscarPorCodigo(codigo) ?? throw new ServicoException("Filial não encontrada.");
        }

        private Dominio.Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento ObterMotivoAlteracaoPosicaoFilaCarregamento(int codigoMotivo)
        {
            Repositorio.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento repositorioMotivoAlteracaoPosicaoFilaCarregamento = new Repositorio.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento(_unitOfWorkContainer.UnitOfWork);

            return repositorioMotivoAlteracaoPosicaoFilaCarregamento.BuscarPorCodigo(codigoMotivo) ?? throw new ServicoException("Não foi possível encontrar o motivo de alteração de posição da fila de carregamento.");
        }

        private Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento ObterMotivoRetiradaFilaCarregamento(int codigoMotivo)
        {
            Repositorio.Embarcador.Logistica.MotivoRetiradaFilaCarregamento repositorioMotivoRetiradaFila = new Repositorio.Embarcador.Logistica.MotivoRetiradaFilaCarregamento(_unitOfWorkContainer.UnitOfWork);

            return repositorioMotivoRetiradaFila.BuscarPorCodigo(codigoMotivo) ?? throw new ServicoException("Não foi possível encontrar o motivo de retirada da fila de carregamento.");
        }

        private int ObterPrazoAlocarPreCargaAntesInicioCarregamento(int codigoCentroCarregamento)
        {
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = ObterCentroCarregamento(codigoCentroCarregamento);

            return (centroCarregamento.TempoEncostaDoca > 0) ? centroCarregamento.TempoEncostaDoca : 120;
        }

        private int ObterPrazoSituacaoCarga(SituacaoCargaJanelaCarregamento situacao, int tempoSituacaoPadrao = 0)
        {
            PrazoSituacaoCarga servicoPrazoSituacaoCarga = new PrazoSituacaoCarga(_unitOfWorkContainer.UnitOfWork);

            return servicoPrazoSituacaoCarga.ObterTempo(situacao, tempoSituacaoPadrao);
        }

        private Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga ObterTipoRetornoCargaReversa()
        {
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorio = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarPorTipoReversa() ?? throw new ServicoException("Tipo de retorno de carga carregado não encontrado");
        }

        private Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga ObterTipoRetornoCargaVazio()
        {
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorio = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarPorTipoVazio() ?? throw new ServicoException("Tipo de retorno de carga vazio não encontrado");
        }

        #endregion

        #region Métodos Privados de Notificação

        private void NotificarFilaAlterada(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            NotificarFilaAlterada(filaCarregamentoVeiculo, TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculo);
        }

        private void NotificarFilaAlterada(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, TipoFilaCarregamentoAlteracao tipo)
        {
            NotificarFilaAlterada(filaCarregamentoVeiculo, new List<TipoFilaCarregamentoAlteracao>() { tipo });
        }

        private void NotificarFilaAlterada(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, List<TipoFilaCarregamentoAlteracao> tipos)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao alteracao = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao();

            alteracao.ModelosVeicularesCarga.Add(filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo);

            foreach (TipoFilaCarregamentoAlteracao tipo in tipos)
                alteracao.Tipos.Add(tipo);

            if (filaCarregamentoVeiculo.CentroCarregamento != null)
                alteracao.CentrosCarregamento.Add(filaCarregamentoVeiculo.CentroCarregamento.Codigo);

            if (filaCarregamentoVeiculo.Filial != null)
                alteracao.Filiais.Add(filaCarregamentoVeiculo.Filial.Codigo);

            if (filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular != null)
                alteracao.GruposModelosVeicularesCarga.Add(filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular.Codigo);

            NotificarFilaAlterada(alteracao);
        }

        private void NotificarFilaAlterada(Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao alteracao)
        {
            if (_unitOfWorkContainer.TransacaoPorContainerAtiva)
                return;

            new Hubs.FilaCarregamento().NotificarTodosFilaAlterada(alteracao);
        }

        private void NotificarJanelaCarregamentoAtualizada(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (_unitOfWorkContainer.TransacaoPorContainerAtiva)
                return;

            if (cargaJanelaCarregamento != null)
                new Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
        }

        private void NotificarMobile(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, TipoNotificacaoMobile tipoNotificacao, string mensagem)
        {
            if (_unitOfWorkContainer.TransacaoPorContainerAtiva)
                return;

            Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoDados notificacao = new Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoDados()
            {
                Assunto = tipoNotificacao == TipoNotificacaoMobile.Mensagem ? "Notificação da fila de carregamento" : "Notificação de alteração da fila de carregamento",
                CentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento,
                Mensagem = mensagem,
                Tipo = tipoNotificacao,
                Usuario = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista
            };

            new HubsMobile.NotificacaoMobile().Notificar(notificacao, _unitOfWorkContainer.UnitOfWork);
        }

        private void NotificarMobilePorUltimoHistorico(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (_unitOfWorkContainer.TransacaoPorContainerAtiva)
                return;

            if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null)
            {
                FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico ultimoHistorico = servicoHistorico.ObterUltimo(filaCarregamentoVeiculo.Codigo);

                if (ultimoHistorico != null)
                {
                    switch (ultimoHistorico.Tipo)
                    {
                        case TipoFilaCarregamentoVeiculoHistorico.CargaCancelada:
                            NotificarMobile(filaCarregamentoVeiculo, TipoNotificacaoMobile.CargaCancelada, ultimoHistorico.Descricao);
                            break;

                        case TipoFilaCarregamentoVeiculoHistorico.CargaAdicionada:
                        case TipoFilaCarregamentoVeiculoHistorico.ConjuntoMotoristaAlterado:
                            NotificarMotoristaCargaAdicionada(filaCarregamentoVeiculo);
                            break;

                        case TipoFilaCarregamentoVeiculoHistorico.CargaRemovida:
                            NotificarMobile(filaCarregamentoVeiculo, TipoNotificacaoMobile.CargaRemovida, ultimoHistorico.Descricao);
                            break;
                    }
                }
            }
        }

        private void NotificarMotoristaCargaAdicionada(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.Carga != null && filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null)
            {
                Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao localAtual = filaCarregamentoVeiculo.ConjuntoVeiculo.ObterLocalAtual();
                string numeroCarga = ObterNumeroCarga(filaCarregamentoVeiculo.Carga);
                string mensagemLocalAtual = string.Empty;

                if (localAtual != null)
                {
                    if (filaCarregamentoVeiculo.ConjuntoVeiculo.ObterTotalPlacas() > 1)
                        mensagemLocalAtual = $". Local atual dos veículos da carga: {localAtual.DescricaoAcao}";
                    else
                        mensagemLocalAtual = $". Local atual do veículo da carga: {localAtual.DescricaoAcao}";
                }

                NotificarMobile(filaCarregamentoVeiculo, TipoNotificacaoMobile.CargaAdicionada, $"Carga {numeroCarga} adicionada{mensagemLocalAtual}");
            }
        }

        private void NotificarMotoristaCargaAlocada(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null)
            {
                string mensagemNotificacao;
                string numeroCarga = ObterNumeroCarga(filaCarregamentoVeiculo.Carga);

                if (filaCarregamentoVeiculo.CentroCarregamento?.TempoEncostaDoca > 0 && !string.IsNullOrWhiteSpace(filaCarregamentoVeiculo.Carga.NumeroDoca))
                    mensagemNotificacao = $"Você foi alocado na carga {numeroCarga} da {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}. Favor encostar na doca {filaCarregamentoVeiculo.Carga.NumeroDoca} em {filaCarregamentoVeiculo.CentroCarregamento.TempoEncostaDoca} minutos";
                else if (!string.IsNullOrWhiteSpace(filaCarregamentoVeiculo.Carga.NumeroDoca))
                    mensagemNotificacao = $"Você foi alocado na carga {numeroCarga} da {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}. Favor encostar na doca {filaCarregamentoVeiculo.Carga.NumeroDoca}";
                else
                    mensagemNotificacao = $"Você foi alocado na carga {numeroCarga} da {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}";

                NotificarMobile(filaCarregamentoVeiculo, TipoNotificacaoMobile.CargaAlocada, mensagemNotificacao);
            }
        }

        private void NotificarSituacaoFilaAlterada(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            new Hubs.FilaCarregamento().NotificarTodosSituacaoFilaAlterada(filaCarregamentoVeiculo);
        }

        #endregion

        #region Métodos Públicos

        public void AceitarCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!IsSituacaoPermiteAceitarOuRecusarCarga(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite aceitar a carga");

            try
            {
                _unitOfWorkContainer.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = ObterCargaJanelaCarregamento(filaCarregamentoVeiculo);
                AceitarCarga(filaCarregamentoVeiculo, descricao: $"Carga {ObterNumeroCarga(filaCarregamentoVeiculo.Carga)} aceita", cargaJanelaCarregamento, tipoServicoMultisoftware);

                _unitOfWorkContainer.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void AceitarCargaManualmente(int codigoFilaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();
            bool permitirAceitarCarga;
            bool permitirAceitarCargaComConjuntoVeiculoIncompleto;

            if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
            {
                permitirAceitarCarga = IsSituacaoPermiteAceitarOuRecusarCargaManualmente(filaCarregamentoVeiculo);
                permitirAceitarCargaComConjuntoVeiculoIncompleto = false;
            }
            else
            {
                permitirAceitarCarga = IsSituacaoPermiteAceitarOuRecusarCarga(filaCarregamentoVeiculo);
                permitirAceitarCargaComConjuntoVeiculoIncompleto = IsSituacaoPermiteAceitarOuRecusarCargaManualmente(filaCarregamentoVeiculo);
            }

            if (!permitirAceitarCargaComConjuntoVeiculoIncompleto && !permitirAceitarCarga)
                throw new ServicoException("Situação da fila de carregamento não permite aceitar a carga");

            try
            {
                _unitOfWorkContainer.Start();

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = ObterCargaJanelaCarregamento(filaCarregamentoVeiculo);

                if (permitirAceitarCargaComConjuntoVeiculoIncompleto)
                    AceitarCargaConjuntoVeiculoIncompleto(filaCarregamentoVeiculo);
                else
                    AceitarCarga(filaCarregamentoVeiculo, descricao: $"Carga {ObterNumeroCarga(filaCarregamentoVeiculo.Carga)} aceita manualmente", cargaJanelaCarregamento, tipoServicoMultisoftware);

                _unitOfWorkContainer.CommitChanges();

                if (permitirAceitarCargaComConjuntoVeiculoIncompleto)
                {
                    NotificarMotoristaCargaAlocada(filaCarregamentoVeiculo);
                    NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
                }
                else
                {
                    NotificarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                    if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem)
                        NotificarFilaAlterada(filaCarregamentoVeiculo);
                    else
                        NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
                }
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void AceitarPreCarga(int codigoFilaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (!IsSituacaoPermiteAceitarOuRecusarPreCarga(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite aceitar o pré planejamento");

            try
            {
                _unitOfWorkContainer.Start();

                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos;

                AtualizarSituacao(filaCarregamentoVeiculo, tipoServicoMultisoftware);
                AtualizarSituacaoPreCarga(filaCarregamentoVeiculo);
                repositorio.Atualizar(filaCarregamentoVeiculo);
                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Pré planejamento {filaCarregamentoVeiculo.PreCarga.NumeroPreCarga} aceito",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.PreCargaAceita
                });

                _unitOfWorkContainer.CommitChanges();

                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void AceitarSaida(int codigoFilaCarregamentoVeiculo, string justificativa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (!IsSituacaoPermiteAceitarOuRecusarSaidaFila(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite aceitar a saída.");

            if (filaCarregamentoVeiculo.Carga?.SituacaoCarga.IsSituacaoCargaEmitida() ?? false)
                throw new ServicoException("Situação atual da carga não permite aceitar a saída");

            if (string.IsNullOrWhiteSpace(justificativa))
                throw new ServicoException("Justificativa para aceitar a saída não informada.");

            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            try
            {
                _unitOfWorkContainer.Start();

                servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Saída da fila de carregamento aceita. Justificativa: {justificativa}",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.SaidaFila
                });
                RemoverFilaCarregamento(filaCarregamentoVeiculo, tipoServicoMultisoftware);

                _unitOfWorkContainer.CommitChanges();

                NotificarFilaAlterada(filaCarregamentoVeiculo);
                NotificarMobile(filaCarregamentoVeiculo, TipoNotificacaoMobile.SolicitacaoSaidaAceita, $"Solicitação de saída da fila aceita. Justificativa: {justificativa}");
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo Adicionar(Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar filaCarregamentoVeiculoAdicionar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var itemFila = new FilaCarregamentoVeiculoDadosAdicionar();

            itemFila.ConjuntoVeiculo = filaCarregamentoVeiculoAdicionar.ConjuntoVeiculo
                ?? ObterFilaCarregamentoConjuntoVeiculo(filaCarregamentoVeiculoAdicionar.CodigoVeiculo);

            itemFila.TipoRetornoCarga = (filaCarregamentoVeiculoAdicionar.CodigoTipoRetornoCarga > 0) ? ObterTipoRetornoCarga(filaCarregamentoVeiculoAdicionar.CodigoTipoRetornoCarga) : ObterTipoRetornoCargaVazio();
            itemFila.DataProgramada = filaCarregamentoVeiculoAdicionar.DataProgramada;
            itemFila.EmTransicao = filaCarregamentoVeiculoAdicionar.EmTransicao;
            itemFila.Motorista = filaCarregamentoVeiculoAdicionar.CodigoMotorista > 0 ? ObterMotorista(filaCarregamentoVeiculoAdicionar.CodigoMotorista) : null;
            itemFila.CodigosDestino = filaCarregamentoVeiculoAdicionar.CodigosDestino;
            itemFila.CodigosRegiaoDestino = filaCarregamentoVeiculoAdicionar.CodigosRegiaoDestino;
            itemFila.CodigosTipoCarga = filaCarregamentoVeiculoAdicionar.CodigosTipoCarga;
            itemFila.SiglasEstadoDestino = filaCarregamentoVeiculoAdicionar.SiglasEstadoDestino;
            itemFila.AreaVeiculo = filaCarregamentoVeiculoAdicionar.CodigoAreaVeiculo > 0 ? ObterAreaVeiculo(filaCarregamentoVeiculoAdicionar.CodigoAreaVeiculo) : null;
            itemFila.Equipamento = filaCarregamentoVeiculoAdicionar.CodigoEquipamento > 0 ? ObterEquipamento(filaCarregamentoVeiculoAdicionar.CodigoEquipamento) : null;

            if (!filaCarregamentoVeiculoAdicionar.EmTransicao)
            {
                itemFila.CentroCarregamento = (filaCarregamentoVeiculoAdicionar.CodigoCentroCarregamento > 0)
                    ? ObterCentroCarregamento(filaCarregamentoVeiculoAdicionar.CodigoCentroCarregamento)
                    : null;

                itemFila.Filial = (filaCarregamentoVeiculoAdicionar.CodigoFilial > 0)
                    ? ObterFilial(filaCarregamentoVeiculoAdicionar.CodigoFilial)
                    : null;

                // Verifica se possui alguma punição registrada
                var punicoes = ObterCentroCarregamentoPunicao(filaCarregamentoVeiculoAdicionar.CodigoCentroCarregamento);

                if (punicoes.Any())
                {
                    if (itemFila.ConjuntoVeiculo.Tracao != null)
                    {
                        punicoes = punicoes.Where(p => p.TipoFrota == itemFila.ConjuntoVeiculo.Tracao.TipoFrota).ToList();
                    }
                    else if (itemFila.ConjuntoVeiculo.Reboques != null && itemFila.ConjuntoVeiculo.Reboques.Any())
                    {
                        var tiposFrotaReboques = itemFila.ConjuntoVeiculo.Reboques.Select(r => r.TipoFrota);
                        punicoes = punicoes.Where(p => tiposFrotaReboques.Contains(p.TipoFrota)).ToList();
                    }

                    foreach (var p in punicoes)
                    {   // Adiciona o tempo à data da fila
                        if (itemFila.DataProgramada != null)
                            itemFila.DataProgramada = Convert.ToDateTime(itemFila.DataProgramada).AddHours(p.TempoPunicaoDouble);
                    }
                }

            }

            return Adicionar(itemFila, tipoServicoMultisoftware);
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo Adicionar(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Usuario motorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            FilaCarregamentoVeiculoDadosAdicionar dadosAdicionar = new FilaCarregamentoVeiculoDadosAdicionar()
            {
                CentroCarregamento = centroCarregamento,
                ConjuntoVeiculo = conjuntoVeiculo,
                Motorista = motorista,
                TipoRetornoCarga = ObterTipoRetornoCargaVazio()
            };

            return Adicionar(dadosAdicionar, tipoServicoMultisoftware);
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo Adicionar(Dominio.Entidades.Veiculo veiculo, int codigoTipoRetornoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = ObterCentroCarregamentoPorVeiculo(veiculo);

            try
            {
                _unitOfWorkContainer.Start();

                if (configuracaoEmbarcador?.FinalizarViagemAnteriorAoEntrarFilaCarregamento ?? false)
                    FinalizarPorVeiculo(veiculo);

                _unitOfWorkContainer.CommitChanges();

                return Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar()
                {
                    CodigoCentroCarregamento = centroCarregamento.Codigo,
                    CodigoTipoRetornoCarga = codigoTipoRetornoCarga,
                    CodigoVeiculo = veiculo.Codigo
                }, tipoServicoMultisoftware);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo AdicionarCargaPorPreCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorPreCargaAtiva(preCarga.Codigo);

            if (filaCarregamentoVeiculo == null)
                return null;

            AdicionarCarga(filaCarregamentoVeiculo, preCarga.Carga, tipoServicoMultisoftware);

            return filaCarregamentoVeiculo;
        }

        public void AlocarCargaManualmente(int codigoFilaCarregamentoVeiculo, int codigoCarga, string observacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Fila de carregamento não encontrada.");

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga) ?? throw new ServicoException("Carga não encontrada.");

            if (!IsSituacaoPermiteAlocarCargaOuPreCarga(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite alocar a carga");

            if (carga?.IsPossuiRestricaoFilaCarregamentoPorDestinatario() ?? false)
                throw new ServicoException("A carga possui restrição na fila de carregamento por destinatário");

            if (carga?.IsPossuiRestricaoFilaCarregamentoPorRemetente() ?? false)
                throw new ServicoException("A carga possui restrição na fila de carregamento por remetente");

            if (string.IsNullOrWhiteSpace(observacao))
                throw new ServicoException("A observação deve ser informada");

            VincularCarga(filaCarregamentoVeiculo, carga, observacao, tipoServicoMultisoftware);
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico AlocarCargaManualmente(int codigoFilaCarregamentoVeiculo, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem motivoSelecaoMotoristaForaOrdem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Fila de carregamento não encontrada.");

            if (!IsSituacaoPermiteAlocarCargaOuPreCarga(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite alocar a carga");

            string numeroCarga = ObterNumeroCarga(carga);

            filaCarregamentoVeiculo.Carga = carga;

            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico historicoCargaAlocada = servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = $"Carga {numeroCarga} alocada{(motivoSelecaoMotoristaForaOrdem == null ? "" : $" fora da ordem da fila. Motivo: {motivoSelecaoMotoristaForaOrdem.Descricao}")}",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                MotivoSelecaoMotoristaForaOrdem = motivoSelecaoMotoristaForaOrdem,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaAlocada
            });

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio repositorioConfiguracaoFluxoPatio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFluxoPatio(_unitOfWorkContainer.UnitOfWork);

            bool conjuntoMotoristaCompleto = filaCarregamentoVeiculo.ConjuntoMotorista.IsCompleto();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFluxoPatio configuracaoFluxoPatio = repositorioConfiguracaoFluxoPatio.BuscarConfiguracaoPadrao();

            if (conjuntoMotoristaCompleto)
                servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Carga {numeroCarga} confirmada para o motorista",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaAceita
                });
            else if (!configuracaoFluxoPatio.PermiteAlocarVeiculoSemConjuntoCarga)
                throw new ServicoException("O conjunto do motorista não está completo.");

            AtualizarCarga(filaCarregamentoVeiculo, tipoServicoMultisoftware);

            if (conjuntoMotoristaCompleto)
            {
                NotificarMotoristaCargaAlocada(filaCarregamentoVeiculo); AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.CargaAceita);
                AlterarParaEmViagem(filaCarregamentoVeiculo, tipoServicoMultisoftware);
            }
            else
                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos;

            repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);

            NotificarFilaAlterada(filaCarregamentoVeiculo);

            return historicoCargaAlocada;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> AlocarPreCargaManualmente(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, int codigoFilaCarregamentoVeiculo, int codigoFilaCarregamentoMotorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo repositorioFilaCarregamentoConjuntoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork);
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculoAlteradas = new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoAnterior = preCarga.Codigo > 0 ? repositorioFilaCarregamentoVeiculo.BuscarPorPreCarga(preCarga.Codigo) : null;
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoAtual = codigoFilaCarregamentoVeiculo > 0 ? repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) : null;
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = codigoFilaCarregamentoMotorista > 0 ? new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer.UnitOfWork).BuscarPorCodigo(codigoFilaCarregamentoMotorista) ?? throw new ServicoException("Fila do motorista não encontrada.") : null;
            bool isFilaCarregamentoVeiculoAlterada = (filaCarregamentoAnterior?.Codigo != filaCarregamentoAtual?.Codigo);

            if (isFilaCarregamentoVeiculoAlterada && (filaCarregamentoAnterior != null))
            {
                DesalocarCargaEPreCarga(filaCarregamentoAnterior, tipoServicoMultisoftware);

                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico> historicosFilaCarregamentoAnterior = servicoHistorico.ObterTodos(filaCarregamentoAnterior.Codigo);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico historicoPreCargaAlocada = historicosFilaCarregamentoAnterior.Where(o => o.Tipo == TipoFilaCarregamentoVeiculoHistorico.PreCargaAlocada).OrderByDescending(o => o.Codigo).FirstOrDefault();
                bool isMotoristaFilaCarregamentoAlterado = historicosFilaCarregamentoAnterior.Any(o => (o.Codigo > historicoPreCargaAlocada.Codigo) && (o.Tipo == TipoFilaCarregamentoVeiculoHistorico.ConjuntoMotoristaAlterado));
                bool isVeiculoFilaCarregamentoAlterado = historicosFilaCarregamentoAnterior.Any(o => (o.Codigo > historicoPreCargaAlocada.Codigo) && (o.Tipo == TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado));

                if (isMotoristaFilaCarregamentoAlterado && filaCarregamentoAnterior.ConjuntoMotorista.IsCompleto())
                    RemoverConjuntoMotorista(filaCarregamentoAnterior);

                if (isVeiculoFilaCarregamentoAlterado && filaCarregamentoAnterior.ConjuntoVeiculo.Tracao != null)
                {
                    servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = $"A tração {filaCarregamentoAnterior.ConjuntoVeiculo.Tracao.Placa_Formatada} foi removida",
                        FilaCarregamentoVeiculo = filaCarregamentoAtual,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado
                    });

                    filaCarregamentoAnterior.ConjuntoVeiculo.Tracao = null;

                    repositorioFilaCarregamentoConjuntoVeiculo.Atualizar(filaCarregamentoAnterior.ConjuntoVeiculo);
                }

                listaFilaCarregamentoVeiculoAlteradas.Add(filaCarregamentoAnterior);
            }

            if (filaCarregamentoAtual != null)
            {
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico> historicosFilaCarregamentoAtual = servicoHistorico.ObterTodos(filaCarregamentoAtual.Codigo);
                bool isVeiculoFilaCarregamentoAtualAlterado = historicosFilaCarregamentoAtual.Any(o => o.Tipo == TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado);

                if (isFilaCarregamentoVeiculoAlterada)
                {
                    VincularPreCarga(filaCarregamentoAtual, preCarga, tipoServicoMultisoftware);

                    if (preCarga.Carga != null)
                        VincularCarga(filaCarregamentoAtual, preCarga.Carga, tipoServicoMultisoftware);
                }
                else if (filaCarregamentoAtual.Carga?.Codigo != preCarga.Carga?.Codigo)
                {
                    if (filaCarregamentoAtual.Carga != null)
                        DesalocarCarga(filaCarregamentoAtual, tipoServicoMultisoftware);

                    if (preCarga.Carga != null)
                        AdicionarCarga(filaCarregamentoAtual, preCarga.Carga, tipoServicoMultisoftware);
                }

                if (filaCarregamentoMotorista != null)
                    VincularFilaCarregamentoMotorista(filaCarregamentoMotorista, filaCarregamentoAtual, tipoServicoMultisoftware);

                if (preCarga.Veiculo?.Codigo != filaCarregamentoAtual.ConjuntoVeiculo.Tracao?.Codigo)
                {
                    if ((filaCarregamentoAtual.ConjuntoVeiculo.Tracao == null) || isVeiculoFilaCarregamentoAtualAlterado)
                    {
                        if ((filaCarregamentoAtual.ConjuntoVeiculo.Tracao != null) && (filaCarregamentoAtual.ConjuntoMotorista.FilaCarregamentoMotorista?.ConjuntoVeiculo != null))
                            throw new ServicoException("A fila de carregamento não permite trocar a tração do motorista");

                        if (preCarga.Veiculo != null)
                        {
                            ValidarVeiculo(preCarga.Veiculo, filaCarregamentoAtual.CentroCarregamento?.NaoValidarIntegracaoGR ?? false);

                            if (filaCarregamentoAtual.ConjuntoVeiculo.Tracao != null)
                                servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                                {
                                    Descricao = $"A tração {filaCarregamentoAtual.ConjuntoVeiculo.Tracao.Placa_Formatada} foi removida",
                                    FilaCarregamentoVeiculo = filaCarregamentoAtual,
                                    OrigemAlteracao = _origemAlteracao,
                                    Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado
                                });

                            filaCarregamentoAtual.ConjuntoVeiculo.Tracao = preCarga.Veiculo;

                            ValidarTransportadorIgual(filaCarregamentoAtual);

                            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                            {
                                Descricao = $"A tração {filaCarregamentoAtual.ConjuntoVeiculo.Tracao.Placa_Formatada} foi informada",
                                FilaCarregamentoVeiculo = filaCarregamentoAtual,
                                OrigemAlteracao = _origemAlteracao,
                                Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado
                            });
                        }
                        else
                        {
                            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                            {
                                Descricao = $"A tração {filaCarregamentoAtual.ConjuntoVeiculo.Tracao.Placa_Formatada} foi removida",
                                FilaCarregamentoVeiculo = filaCarregamentoAtual,
                                OrigemAlteracao = _origemAlteracao,
                                Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado
                            });

                            filaCarregamentoAtual.ConjuntoVeiculo.Tracao = null;
                        }

                        repositorioFilaCarregamentoConjuntoVeiculo.Atualizar(filaCarregamentoAtual.ConjuntoVeiculo);
                    }
                    else
                        throw new ServicoException("A fila de carregamento não permite trocar a tração");
                }

                if (filaCarregamentoAtual.ConjuntoVeiculo.Reboques?.Count() > 0)
                {
                    if ((preCarga.VeiculosVinculados == null) || (preCarga.VeiculosVinculados.Count() == 0) || !filaCarregamentoAtual.ConjuntoVeiculo.Reboques.Contains(preCarga.VeiculosVinculados.FirstOrDefault()))
                        throw new ServicoException("A fila de carregamento não permite alteração nos reboques");
                }
                else if (preCarga.VeiculosVinculados?.Count() > 0)
                    throw new ServicoException("A fila de carregamento não permite reboques");

                AtualizarSituacao(filaCarregamentoAtual, tipoServicoMultisoftware);

                listaFilaCarregamentoVeiculoAlteradas.Add(filaCarregamentoAtual);
            }

            return listaFilaCarregamentoVeiculoAlteradas;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo AlocarParaPrimeiroDaFila(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga.Empresa != null)
                throw new ServicoException($"O transportador já foi informado para a {carga.DescricaoEntidade}");

            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.ObjetosDeValor.Localidade> localidadesDestino = servicoCargaDadosSumarizados.ObterDestinos(carga, _unitOfWorkContainer.UnitOfWork, tipoServicoMultisoftware);
            List<int> codigosDestinos = localidadesDestino.Select(localidadeDestino => localidadeDestino.Codigo).Distinct().ToList();

            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPrimeiroDisponivel filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPrimeiroDisponivel()
            {
                CodigoFilial = carga.Filial.Codigo,
                CodigoModeloVeicularCarga = carga.ModeloVeicularCarga.Codigo,
                CodigosDestinos = codigosDestinos,
                DataProgramada = carga.DataCarregamentoCarga.Value
            };

            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPrimeiroDisponivelNaFila(filtrosPesquisa);

            if (filaCarregamentoVeiculo == null)
                throw new ServicoException($"Nenhum veículo encontrado na fila de carregamento para as configurações da {carga.DescricaoEntidade}");

            VincularCarga(filaCarregamentoVeiculo, carga, tipoServicoMultisoftware);

            return filaCarregamentoVeiculo;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo AlocarParaPrimeiroDaFila(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (preCarga.SituacaoPreCarga != SituacaoPreCarga.Nova)
                throw new ServicoException($"A situação do pré planejamento ({preCarga.SituacaoPreCarga.ObterDescricao()}) não permite vincular veículo da fila de carregamento");

            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoAnterior = repositorioFilaCarregamentoVeiculo.BuscarPorPreCarga(preCarga.Codigo);

            if (filaCarregamentoAnterior != null)
                throw new ServicoException("O pré planejamento já possui veículo da fila de carregamento vinculado");

            if (preCarga.Empresa != null)
                throw new ServicoException("O transportador já foi informado para o pré planejamento");

            Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(_unitOfWorkContainer.UnitOfWork);

            List<int> codigosDestinos = repositorioPreCargaDestino.BuscarCodigosDestinosPorPreCarga(preCarga.Codigo);
            List<int> codigosRegioesDestino = repositorioPreCargaRegiaoDestino.BuscarCodigosRegioesDestinoPorPreCarga(preCarga.Codigo);
            List<string> siglasEstadosDestino = repositorioPreCargaEstadoDestino.BuscarSiglasEstadosDestinoPorPreCarga(preCarga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPrimeiroDisponivel filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPrimeiroDisponivel()
            {
                CodigoFilial = preCarga.Filial.Codigo,
                CodigoModeloVeicularCarga = preCarga.ModeloVeicularCarga?.Codigo ?? 0,
                CodigoPreCargaDesconsiderar = preCarga.Codigo,
                CodigosDestinos = codigosDestinos,
                CodigosRegioesDestino = codigosRegioesDestino,
                DataProgramada = preCarga.DataPrevisaoEntrega.Value,
                SiglasEstadosDestino = siglasEstadosDestino
            };

            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPrimeiroDisponivelNaFila(filtrosPesquisa);

            if (filaCarregamentoVeiculo == null)
                throw new ServicoException("Nenhum veículo encontrado na fila de carregamento para as configurações do pré planejamento");

            VincularPreCarga(filaCarregamentoVeiculo, preCarga, tipoServicoMultisoftware);
            AtualizarPreCarga(filaCarregamentoVeiculo);

            if (preCarga.Carga != null)
                VincularCarga(filaCarregamentoVeiculo, preCarga.Carga, tipoServicoMultisoftware);

            return filaCarregamentoVeiculo;
        }

        public void AlterarCentroCarregamento(int codigoFilaCarregamento, int codigoCentroCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamento) ?? throw new ServicoException("Fila de carregamento não encontrada.");
            int? codigoCentroCarregamentoAnterior = filaCarregamentoVeiculo.CentroCarregamento?.Codigo;

            AlterarCentroCarregamento(filaCarregamentoVeiculo, codigoCentroCarregamento, tipoServicoMultisoftware);

            Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao alteracao = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao(recarregarInformacoes: true);

            alteracao.CentrosCarregamento.Add(codigoCentroCarregamentoAnterior.Value);
            alteracao.CentrosCarregamento.Add(codigoCentroCarregamento);
            alteracao.ModelosVeicularesCarga.Add(filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo);
            alteracao.Tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculo);

            if (filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular != null)
                alteracao.GruposModelosVeicularesCarga.Add(filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular.Codigo);

            NotificarFilaAlterada(alteracao);
        }

        public void AlterarPrimeiraPosicao(int codigoFilaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (!IsSituacaoPermiteAlterarPosicao(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite alterar a posição.");

            AlterarPosicaoParaPrimeira(filaCarregamentoVeiculo, tipoServicoMultisoftware);
        }

        public void AlterarSituacao(int codigoFilaCarregamento, int codigoCentroCarregamento, SituacaoFilaCarregamentoVeiculo novaSituacao, int codigoMotivoRetiradaFilaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string observacao)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamento) ?? throw new ServicoException("Fila de carregamento não encontrada.");
            SituacaoFilaCarregamentoVeiculo situacaoAtual = filaCarregamentoVeiculo.Situacao;

            if (situacaoAtual != novaSituacao)
            {
                int? codigoCentroCarregamentoAnterior = filaCarregamentoVeiculo.CentroCarregamento?.Codigo;
                int? codigoFilialAnterior = filaCarregamentoVeiculo.Filial?.Codigo;

                AlterarSituacao(filaCarregamentoVeiculo, codigoCentroCarregamento, novaSituacao, codigoMotivoRetiradaFilaCarregamento, tipoServicoMultisoftware, observacao);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao alteracao = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao(recarregarInformacoes: true);
                List<TipoFilaCarregamentoAlteracao> tiposAtual = situacaoAtual.ObterTiposFilaCarregamentoAlteracao();
                List<TipoFilaCarregamentoAlteracao> tiposNovo = novaSituacao.ObterTiposFilaCarregamentoAlteracao();

                alteracao.ModelosVeicularesCarga.Add(filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo);

                if (codigoCentroCarregamentoAnterior.HasValue)
                    alteracao.CentrosCarregamento.Add(codigoCentroCarregamentoAnterior.Value);

                if (codigoCentroCarregamento > 0)
                    alteracao.CentrosCarregamento.Add(codigoCentroCarregamento);

                if (codigoFilialAnterior.HasValue)
                    alteracao.Filiais.Add(codigoFilialAnterior.Value);

                if (tiposAtual.Count > 0)
                    alteracao.Tipos.Add(tiposAtual.FirstOrDefault());

                if (tiposNovo.Count > 0)
                    alteracao.Tipos.Add(tiposNovo.FirstOrDefault());

                if (filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular != null)
                    alteracao.GruposModelosVeicularesCarga.Add(filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular.Codigo);

                NotificarFilaAlterada(alteracao);
            }
        }

        public void AlterarUltimaPosicao(int codigoFilaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (!IsSituacaoPermiteAlterarPosicao(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite alterar a posição.");

            AlterarPosicaoParaUltima(filaCarregamentoVeiculo, tipoServicoMultisoftware);
        }

        public void AtualizarPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (!configuracaoEmbarcador.UtilizarFilaCarregamento)
                    return;
            }
            else
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento repositorioConfiguracaoFilaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento(_unitOfWorkContainer.UnitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento configuracaoFilaCarregamento = repositorioConfiguracaoFilaCarregamento.BuscarPrimeiroRegistro();

                if (!configuracaoFilaCarregamento.AtualizarFilaCarregamentoAoAlterarDadosTransporteNaCarga)
                    return;
            }

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoesCarga = carga.GetCurrentChanges();
            bool motoristaAlterado = alteracoesCarga.Any(alteracao => alteracao.Propriedade == "Motoristas");
            bool tracaoAlterada = alteracoesCarga.Any(alteracao => alteracao.Propriedade == "Veiculo");
            bool reboquesAlterados = alteracoesCarga.Any(alteracao => alteracao.Propriedade == "VeiculosVinculados");
            bool dadosTransporteAlterados = (motoristaAlterado || tracaoAlterada || reboquesAlterados);

            if (!dadosTransporteAlterados)
                return;

            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista repositorioFilaCarregamentoConjuntoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Usuario motorista = repositorioCargaMotorista.BuscarPrimeiroMotoristaPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in filasCarregamentoVeiculo)
            {
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico> historicosFilaCarregamento = servicoHistorico.ObterTodos(filaCarregamentoVeiculo.Codigo);
                bool alterarMotorista = false;
                bool alterarTracao = false;
                bool desvincularCarga = false;

                try
                {
                    if (carga.Veiculo?.Codigo != filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Codigo)
                    {
                        bool tracaoFilaCarregamentoAlterada = historicosFilaCarregamento.Any(historico => historico.Tipo == TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado);

                        if ((filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao != null) && !tracaoFilaCarregamentoAlterada)
                            throw new ServicoException("A fila de carregamento não permite trocar a tração");

                        alterarTracao = true;
                    }

                    if (motorista?.Codigo != filaCarregamentoVeiculo.ConjuntoMotorista.Motorista?.Codigo)
                        alterarMotorista = true;

                    if (alterarTracao || alterarMotorista)
                    {
                        if (filaCarregamentoVeiculo.ConjuntoMotorista.Exclusivo)
                            throw new ServicoException("A fila de carregamento é exclusiva do motorista");

                        if ((alterarTracao != alterarMotorista) && (filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao != null) && (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista?.ConjuntoVeiculo != null))
                        {
                            if (alterarTracao)
                                throw new ServicoException("A fila de carregamento não permite trocar a tração do motorista");
                            else
                                throw new ServicoException("A fila de carregamento não permite trocar o motorista sem trocar a tração");
                        }
                    }

                    if (filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques?.Count() > 0)
                    {
                        if ((carga.VeiculosVinculados == null) || (carga.VeiculosVinculados.Count() == 0) || !filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques.Contains(carga.VeiculosVinculados.FirstOrDefault()))
                            throw new ServicoException("A fila de carregamento não permite alteração nos reboques");
                    }
                    else if (carga.VeiculosVinculados?.Count() > 0)
                        throw new ServicoException("A fila de carregamento não permite reboques");
                }
                catch (ServicoException)
                {
                    desvincularCarga = true;
                }

                if (desvincularCarga)
                    DesalocarCarga(filaCarregamentoVeiculo, tipoServicoMultisoftware);
                else if (alterarMotorista || alterarTracao)
                {
                    if (alterarMotorista)
                    {
                        if (filaCarregamentoVeiculo.ConjuntoMotorista.IsCompleto())
                        {
                            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                            {
                                Descricao = $"O motorista {filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Nome} ({filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.CPF_Formatado}) foi removido",
                                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                                OrigemAlteracao = _origemAlteracao,
                                Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoMotoristaAlterado
                            });

                            if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista != null)
                                AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.Disponivel);

                            filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista = null;
                            filaCarregamentoVeiculo.ConjuntoMotorista.Motorista = null;
                        }

                        if (motorista != null)
                        {
                            filaCarregamentoVeiculo.ConjuntoMotorista.Motorista = motorista;

                            repositorioFilaCarregamentoConjuntoMotorista.Atualizar(filaCarregamentoVeiculo.ConjuntoMotorista);

                            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                            {
                                Descricao = $"Motorista {filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Nome} ({filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.CPF_Formatado}) informado",
                                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                                OrigemAlteracao = _origemAlteracao,
                                Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoMotoristaAlterado
                            });
                        }
                    }

                    if (alterarTracao)
                    {
                        if (filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao != null)
                        {
                            filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao = null;

                            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                            {
                                Descricao = $"A tração {filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao.Placa_Formatada} foi removida",
                                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                                OrigemAlteracao = _origemAlteracao,
                                Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado
                            });
                        }

                        if (carga.Veiculo != null)
                        {
                            filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao = carga.Veiculo;

                            servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                            {
                                Descricao = $"A tração {filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao.Placa_Formatada} foi informada",
                                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                                OrigemAlteracao = _origemAlteracao,
                                Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado
                            });
                        }
                    }

                    repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);

                    AtualizarPreCarga(filaCarregamentoVeiculo);

                    if (filaCarregamentoVeiculo.IsConjuntosCompletos())
                        AlterarParaEmViagem(filaCarregamentoVeiculo, tipoServicoMultisoftware);
                }

                NotificarFilaAlterada(filaCarregamentoVeiculo);
            }
        }

        public void AtualizarSituacaoCargaJanelaCarregamentoParaProntaParaCarregamento(int codigoCarga)
        {
            if (IsUtilizaFilaCarregamentoMotorista())
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWorkContainer.UnitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);

                if ((cargaJanelaCarregamento != null) && (cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.AgEncosta))
                {
                    AtualizarSituacaoJanelaCarregamento(cargaJanelaCarregamento, SituacaoCargaJanelaCarregamento.ProntaParaCarregamento);
                    AdicionarCargaJanelaCarregamentoTransportador(cargaJanelaCarregamento);
                }
            }
        }

        public void AtrelarReboque(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista, Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork);
            FilaCarregamentoMotorista servicoFilaCarregamentoMotorista = new FilaCarregamentoMotorista(_unitOfWorkContainer, _origemAlteracao);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado repositorioFilaCarregamentoVeiculoAtrelado = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado(_unitOfWorkContainer.UnitOfWork);

            try
            {
                _unitOfWorkContainer.Start();

                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.ReboqueAtrelado;

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado filaCarregamentoVeiculoAtrelado = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado()
                {
                    FilaCarregamentoMotorista = filaCarregamentoMotorista,
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo
                };

                repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);
                servicoFilaCarregamentoMotorista.AlterarSituacao(filaCarregamentoMotorista, SituacaoFilaCarregamentoMotorista.ReboqueAtrelado);
                repositorioFilaCarregamentoVeiculoAtrelado.Inserir(filaCarregamentoVeiculoAtrelado);
                servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Tração {filaCarregamentoMotorista.ConjuntoVeiculo.Tracao.Placa_Formatada} atrelada",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.VeiculoAtrelado
                });

                _unitOfWorkContainer.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ConfirmarChegadaVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo = repositorio.BuscarAguardandoChegadaVeiculoPorVeiculo(veiculo.Codigo);

            if (filasCarregamentoVeiculo.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();

            try
            {
                _unitOfWorkContainer.Start();

                foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in filasCarregamentoVeiculo)
                    ConfirmarChegadaConjuntoVeiculo(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();

                return filasCarregamentoVeiculo;
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void ConfirmarChegadaVeiculo(int codigoFilaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (!IsSituacaoPermiteConfirmarChegadaVeiculo(filaCarregamentoVeiculo))
                throw new ServicoException("A situação da fila de carregamento não permite confirmar a chegada de veículo.");

            try
            {
                _unitOfWorkContainer.Start();

                ConfirmarChegadaConjuntoVeiculo(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();

                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void ConfirmarChegadaVeiculo(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (!IsSituacaoPermiteConfirmarChegadaVeiculo(filaCarregamentoVeiculo))
                return;

            try
            {
                _unitOfWorkContainer.Start();

                ConfirmarChegadaConjuntoVeiculo(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();

                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void DesatrelarVeiculo(int codigoFilaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado repositorioFilaCarregamentoVeiculoAtrelado = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado filaCarregamentoVeiculoAtrelado = repositorioFilaCarregamentoVeiculoAtrelado.BuscarPorFilaCarregamentoVeiculo(codigoFilaCarregamentoVeiculo);

            try
            {
                _unitOfWorkContainer.Start();

                DesatrelarVeiculo(filaCarregamentoVeiculoAtrelado);

                _unitOfWorkContainer.CommitChanges();

                NotificarFilaAlterada(filaCarregamentoVeiculoAtrelado.FilaCarregamentoVeiculo, new List<TipoFilaCarregamentoAlteracao>() {
                    TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculo, TipoFilaCarregamentoAlteracao.FilaCarregamentoMotorista
                });
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado DesatrelarVeiculo(Dominio.Entidades.Veiculo tracao, Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao local)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado repositorioFilaCarregamentoVeiculoAtrelado = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado filaCarregamentoVeiculoAtrelado = repositorioFilaCarregamentoVeiculoAtrelado.BuscarPorTracao(tracao.Codigo);

            try
            {
                _unitOfWorkContainer.Start();

                DesatrelarVeiculo(filaCarregamentoVeiculoAtrelado);
                AtualizarLocalAtual(filaCarregamentoVeiculoAtrelado.FilaCarregamentoVeiculo.ConjuntoVeiculo, local);
                AtualizarLocalAtual(filaCarregamentoVeiculoAtrelado.FilaCarregamentoMotorista.ConjuntoVeiculo, local);

                _unitOfWorkContainer.CommitChanges();

                return filaCarregamentoVeiculoAtrelado;
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void DesvincularCargasPrazoAceitacaoEsgotado()
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento> listaFilaCarregamentoAgrupamento = repositorioFilaCarregamentoVeiculo.BuscarAgrupamentoPorCargaAguardandoConfirmacao();

            if (listaFilaCarregamentoAgrupamento.Count > 0)
            {
                FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

                foreach (var agrupamento in listaFilaCarregamentoAgrupamento)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarCargaAguardandoConfirmacaoPorAgrupamento(agrupamento);

                    foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in listaFilaCarregamentoVeiculo)
                    {
                        if (IsMotoristaPerdeuSenhaCargaAlocada(filaCarregamentoVeiculo))
                        {
                            try
                            {
                                _unitOfWorkContainer.Start();

                                Dominio.Entidades.Embarcador.Cargas.Carga cargaDesvinculada = DesvincularCarga(filaCarregamentoVeiculo);

                                AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.SenhaPerdida);

                                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

                                repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);

                                string numeroCarga = ObterNumeroCarga(cargaDesvinculada);

                                servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                                {
                                    Descricao = $"Perdeu senha para a carga {numeroCarga}",
                                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                                    OrigemAlteracao = _origemAlteracao,
                                    Tipo = TipoFilaCarregamentoVeiculoHistorico.SenhaPerdida
                                });

                                _unitOfWorkContainer.CommitChanges();

                                NotificarMobile(filaCarregamentoVeiculo, TipoNotificacaoMobile.PerdeuSenha, $"Você perdeu senha para a carga {numeroCarga}");
                                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
                                NotificarJanelaCarregamentoAtualizada(ObterCargaJanelaCarregamentoPorCarga(cargaDesvinculada));
                            }
                            catch (Exception)
                            {
                                _unitOfWorkContainer.Rollback();
                                throw;
                            }
                        }
                    }
                }
            }
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> DisponibilizarPorCargaCancelada(int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            FilaCarregamentoVeiculoHistorico servicoFilaCarregamentoVeiculoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCargaAtiva(codigoCarga);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculoDisponibilizadas = new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();

            foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in filasCarregamentoVeiculo)
            {
                CancelarAlteracaoParaEmViagem(filaCarregamentoVeiculo, tipoServicoMultisoftware);

                if (filaCarregamentoVeiculo.Situacao != SituacaoFilaCarregamentoVeiculo.CargaCancelada)
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.CargaCancelada;

                repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);

                AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.CargaCancelada);

                servicoFilaCarregamentoVeiculoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Carga {ObterNumeroCarga(filaCarregamentoVeiculo.Carga)} cancelada",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaCancelada
                });

                filasCarregamentoVeiculoDisponibilizadas.Add(filaCarregamentoVeiculo);
            }

            return filasCarregamentoVeiculoDisponibilizadas;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo DisponibilizarPorPreCargaCancelada(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorPreCargaAtiva(preCarga.Codigo);

            if (filaCarregamentoVeiculo != null)
                DesalocarCargaEPreCarga(filaCarregamentoVeiculo, tipoServicoMultisoftware);

            return filaCarregamentoVeiculo;
        }

        public void EnviarNotificacao(int codigoFilaCarregamentoVeiculo, string mensagem)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (!filaCarregamentoVeiculo.ConjuntoMotorista.IsCompleto())
                throw new ServicoException("O cunjunto do motorista deve estar completo para permitir o envio de notificação.");

            if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista == null)
                throw new ServicoException("O motorista não está vinculado na fila de carregamento a partir do mobile.");

            NotificarMobile(filaCarregamentoVeiculo, TipoNotificacaoMobile.Mensagem, mensagem);
        }

        public void Finalizar(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, string descricaoHistorico)
        {
            FilaCarregamentoVeiculoHistorico servicoFilaCarregamentoVeiculoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Finalizada;

            repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);

            AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.Finalizada);
            servicoFilaCarregamentoVeiculoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = descricaoHistorico,
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.SaidaFila
            });
        }

        public void FinalizarPorCarga(int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCarga(codigoCarga);

            foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in filasCarregamentoVeiculo)
            {
                bool permitirFinalizar = (
                    (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem) ||
                    (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga) ||
                    (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos)
                );

                if (!permitirFinalizar)
                    continue;

                if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga)
                    AceitarCarga(filaCarregamentoVeiculo, descricao: $"Carga {ObterNumeroCarga(filaCarregamentoVeiculo.Carga)} aceita automaticamente ao ser encerrada", tipoServicoMultisoftware);

                if (filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos)
                    AlterarParaEmViagem(filaCarregamentoVeiculo, tipoServicoMultisoftware);

                Finalizar(filaCarregamentoVeiculo, descricaoHistorico: "Carga encerrada");
            }
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo FinalizarPorMotorista(int codigoMotorista)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarEmViagemPorMotorista(codigoMotorista);

            foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in filasCarregamentoVeiculo)
                Finalizar(filaCarregamentoVeiculo, descricaoHistorico: "Fila de carregamento encerrada");

            return filasCarregamentoVeiculo.LastOrDefault();
        }

        public void InformarDoca(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, string hash)
        {
            if (!IsSituacaoPermiteInformarDoca(filaCarregamentoVeiculo))
                throw new ServicoException("A situação da fila de carregamento não permite informar a doca.");

            GestaoPatio.FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao = new GestaoPatio.FluxoGestaoPatioConfiguracao(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();

            if (configuracaoGestaoPatio.InformarDocaCarregamentoUtilizarLocalCarregamento)
                throw new ServicoException("Não é possível informar a doca quando a configuração para utilizar o local de carregamento está ativa.");

            if (filaCarregamentoVeiculo.CentroCarregamento == null)
                throw new ServicoException("Não é possível informar a doca quando a fila não possui centro de carregamento.");

            if ((filaCarregamentoVeiculo.CentroCarregamento.Docas == null) || (filaCarregamentoVeiculo.CentroCarregamento.Docas.Count == 0))
                throw new ServicoException($"Nenhuma doca cadastrada para o centro de carregamento {filaCarregamentoVeiculo.CentroCarregamento.Descricao}.");

            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca doca = (from centroCarregamentoDoca in filaCarregamentoVeiculo.CentroCarregamento.Docas where centroCarregamentoDoca.CodigoIntegracao == hash select centroCarregamentoDoca).FirstOrDefault();

            if (doca == null)
                throw new ServicoException($"Nenhuma doca encontrada no centro de carregamento {filaCarregamentoVeiculo.CentroCarregamento.Descricao}.");

            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorCarga(filaCarregamentoVeiculo.Carga.Codigo) ?? throw new ServicoException("Não foi possível encontrar a doca de carregamento.");

            if (!docaCarregamento.EtapaDocaCarregamentoLiberada)
                throw new ServicoException($"A liberação da doca ainda não foi autorizada");

            if (docaCarregamento.Situacao == SituacaoDocaCarregamento.Informada)
                throw new ServicoException($"Doca já informada");

            try
            {
                _unitOfWorkContainer.Start();

                docaCarregamento.DataInformacaoDoca = DateTime.Now;
                docaCarregamento.NumeroDoca = doca.Numero.ToString();
                docaCarregamento.Situacao = SituacaoDocaCarregamento.Informada;
                docaCarregamento.Carga.NumeroDocaEncosta = docaCarregamento.NumeroDoca;

                if (string.IsNullOrWhiteSpace(docaCarregamento.Carga.NumeroDoca))
                    docaCarregamento.Carga.NumeroDoca = docaCarregamento.NumeroDoca;

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWorkContainer.UnitOfWork);
                repositorioCarga.Atualizar(docaCarregamento.Carga);

                if (IsAvancarEtapaGestaoPatio(docaCarregamento.Carga))
                {
                    Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoFluxoGestaoPatio configuracaoFluxoGestaoPatio = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.ConfiguracaoFluxoGestaoPatio()
                    {
                        LiberarComMensagemSemComfirmacao = true
                    };
                    GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(_unitOfWorkContainer.UnitOfWork, configuracaoFluxoGestaoPatio);

                    servicoFluxoGestaoPatio.LiberarProximaEtapa(docaCarregamento.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InformarDoca);
                }

                repositorioDocaCarregamento.Atualizar(docaCarregamento);

                AtualizarSituacaoCargaJanelaCarregamentoParaProntaParaCarregamento(docaCarregamento.Carga.Codigo);

                _unitOfWorkContainer.CommitChanges();

                Servicos.Embarcador.Integracao.Eship.IntegracaoEship serEShip = new Servicos.Embarcador.Integracao.Eship.IntegracaoEship(_unitOfWorkContainer.UnitOfWork);
                serEShip.VerificarIntegracaoEShip(docaCarregamento.Carga);

                string mensagemRetorno = string.Empty;
                Integracao.GPA.IntegracaoGPA.IntegrarEncostaVeiculo(docaCarregamento.Carga, ref mensagemRetorno, _unitOfWorkContainer.UnitOfWork);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void InformarMotorista(int codigoFilaCarregamentoVeiculo, int codigoMotorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool atualizarMotorista = false)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Fila de carregamento não encontrada.");

            if (!IsSituacaoPermiteInformarMotorista(filaCarregamentoVeiculo))
                throw new ServicoException("A situação da fila de carregamento não permite informar o motorista.");

            if (filaCarregamentoVeiculo.ConjuntoMotorista.IsCompleto() && !atualizarMotorista)
                throw new ServicoException("O conjunto do motorista já está completo");

            filaCarregamentoVeiculo.ConjuntoMotorista.Motorista = ObterMotorista(codigoMotorista) ?? throw new ServicoException("O motorista não foi encontrado");

            ValidarMotorista(filaCarregamentoVeiculo.ConjuntoMotorista.Motorista, filaCarregamentoVeiculo.CentroCarregamento?.NaoValidarIntegracaoGR ?? false);
            ValidarTransportadorIgual(filaCarregamentoVeiculo);

            try
            {
                _unitOfWorkContainer.Start();

                Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista repositorioFilaCarregamentoConjuntoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista(_unitOfWorkContainer.UnitOfWork);

                repositorioFilaCarregamentoConjuntoMotorista.Atualizar(filaCarregamentoVeiculo.ConjuntoMotorista);

                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Motorista {filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Nome} ({filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.CPF_Formatado}) informado",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoMotoristaAlterado
                });

                AtualizarMotoristaCarga(filaCarregamentoVeiculo);
                AtualizarMotoristaPreCarga(filaCarregamentoVeiculo);
                AtualizarSituacao(filaCarregamentoVeiculo, tipoServicoMultisoftware);

                _unitOfWorkContainer.CommitChanges();

                NotificarFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void InserirEquipamento(int codigoFilaCarregamentoVeiculo, int codigoEquipamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Fila de carregamento não encontrada.");

            try
            {
                _unitOfWorkContainer.Start();
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWorkContainer.UnitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(codigoEquipamento);

                filaCarregamentoVeiculo.Equipamento = equipamento;

                repositorio.Atualizar(filaCarregamentoVeiculo);
                _unitOfWorkContainer.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }
        public void InformarTracao(int codigoFilaCarregamentoVeiculo, int codigoTracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Fila de carregamento não encontrada.");

            if (!IsSituacaoPermiteInformarTracao(filaCarregamentoVeiculo))
                throw new ServicoException("A situação da fila de carregamento não permite informar a tração.");

            if (filaCarregamentoVeiculo.ConjuntoVeiculo.IsCompleto())
                throw new ServicoException("O conjunto do veículo já está completo");

            filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao = ObterVeiculoPorCodigo(codigoTracao);

            ValidarVeiculo(filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao, filaCarregamentoVeiculo.CentroCarregamento?.NaoValidarIntegracaoGR ?? false);
            ValidarTransportadorIgual(filaCarregamentoVeiculo);

            try
            {
                _unitOfWorkContainer.Start();

                Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo repositorioFilaCarregamentoConjuntoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork);

                repositorioFilaCarregamentoConjuntoVeiculo.Atualizar(filaCarregamentoVeiculo.ConjuntoVeiculo);

                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"A tração {filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao.Placa_Formatada} foi informada",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado
                });

                AtualizarTracaoCarga(filaCarregamentoVeiculo);
                AtualizarTracaoPreCarga(filaCarregamentoVeiculo);
                AtualizarSituacao(filaCarregamentoVeiculo, tipoServicoMultisoftware);

                _unitOfWorkContainer.CommitChanges();

                NotificarFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void Liberar(int codigoFilaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioVeiculo.BuscarPorCodigo(codigoFilaCarregamentoVeiculo);

            if (IsSituacaoPermiteLiberacao(filaCarregamentoVeiculo))
            {
                if (IsSituacaoPermiteLiberarVeiculo(filaCarregamentoVeiculo))
                    LiberarVeiculo(filaCarregamentoVeiculo);
                else
                    LiberarMotorista(filaCarregamentoVeiculo);
            }
            else
                throw new ServicoException("Situação da fila de carregamento não permite liberação.");
        }

        public void NotificarAlteracao(int codigoFilaCarregamentoVeiculo)
        {
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico ultimoHistorico = servicoHistorico.ObterUltimo(codigoFilaCarregamentoVeiculo);

            if (ultimoHistorico != null)
            {
                switch (ultimoHistorico.Tipo)
                {
                    case TipoFilaCarregamentoVeiculoHistorico.CargaAceita:
                    case TipoFilaCarregamentoVeiculoHistorico.CargaCancelada:
                    case TipoFilaCarregamentoVeiculoHistorico.ChecklistConcluido:
                    case TipoFilaCarregamentoVeiculoHistorico.ChegadaVeiculoConfirmada:
                    case TipoFilaCarregamentoVeiculoHistorico.ConjuntoMotoristaAlterado:
                    case TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado:
                    case TipoFilaCarregamentoVeiculoHistorico.EntradaFila:
                    case TipoFilaCarregamentoVeiculoHistorico.PosicaoAlterada:
                    case TipoFilaCarregamentoVeiculoHistorico.PreCargaAlocada:
                    case TipoFilaCarregamentoVeiculoHistorico.PreCargaDesalocada:
                    case TipoFilaCarregamentoVeiculoHistorico.SaidaFila:
                        Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao alteracao = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao();

                        alteracao.ModelosVeicularesCarga.Add(ultimoHistorico.FilaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo);

                        if ((ultimoHistorico.Tipo == TipoFilaCarregamentoVeiculoHistorico.EntradaFila) && (ultimoHistorico.FilaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.EmTransicao))
                            alteracao.Tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculoEmTransicao);
                        else
                            alteracao.Tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculo);

                        if (ultimoHistorico.Tipo == TipoFilaCarregamentoVeiculoHistorico.SaidaFila)
                            alteracao.Tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculoEmTransicao);

                        if (ultimoHistorico.Tipo == TipoFilaCarregamentoVeiculoHistorico.ConjuntoMotoristaAlterado)
                            alteracao.Tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoMotorista);

                        if (ultimoHistorico.FilaCarregamentoVeiculo.CentroCarregamento != null)
                            alteracao.CentrosCarregamento.Add(ultimoHistorico.FilaCarregamentoVeiculo.CentroCarregamento.Codigo);

                        if (ultimoHistorico.FilaCarregamentoVeiculo.Filial != null)
                            alteracao.Filiais.Add(ultimoHistorico.FilaCarregamentoVeiculo.Filial.Codigo);

                        if (ultimoHistorico.FilaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular != null)
                            alteracao.GruposModelosVeicularesCarga.Add(ultimoHistorico.FilaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular.Codigo);

                        NotificarFilaAlterada(alteracao);
                        break;

                    case TipoFilaCarregamentoVeiculoHistorico.CargaAceitaPortal:
                    case TipoFilaCarregamentoVeiculoHistorico.CargaAlocada:
                    case TipoFilaCarregamentoVeiculoHistorico.CargaRecusada:
                    case TipoFilaCarregamentoVeiculoHistorico.CargaRecusadaPortal:
                    case TipoFilaCarregamentoVeiculoHistorico.MotoristaLiberado:
                    case TipoFilaCarregamentoVeiculoHistorico.PreCargaRecusada:
                    case TipoFilaCarregamentoVeiculoHistorico.SaidaReversa:
                    case TipoFilaCarregamentoVeiculoHistorico.SenhaPerdida:
                    case TipoFilaCarregamentoVeiculoHistorico.SolicitacaoSaidaFila:
                    case TipoFilaCarregamentoVeiculoHistorico.SolicitacaoSaidaFilaRecusada:
                    case TipoFilaCarregamentoVeiculoHistorico.VeiculoAtrelado:
                    case TipoFilaCarregamentoVeiculoHistorico.VeiculoLiberado:

                        NotificarSituacaoFilaAlterada(ultimoHistorico.FilaCarregamentoVeiculo);
                        break;
                }
            }
        }

        public void NotificarAlteracao(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            NotificarAlteracoes(new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>() { filaCarregamentoVeiculo });
        }

        public void NotificarAlteracoes(List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo)
        {
            if (filasCarregamentoVeiculo.Count > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao alteracao = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAlteracao(recarregarInformacoes: true);

                foreach (var filaCarregamentoVeiculo in filasCarregamentoVeiculo)
                {
                    if (filaCarregamentoVeiculo != null)
                    {
                        NotificarMobilePorUltimoHistorico(filaCarregamentoVeiculo);

                        List<TipoFilaCarregamentoAlteracao> tiposFilaCarregamentoAlteracao = filaCarregamentoVeiculo.Situacao.ObterTiposFilaCarregamentoAlteracao();

                        if (tiposFilaCarregamentoAlteracao.Count > 0)
                        {
                            alteracao.ModelosVeicularesCarga.Add(filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo);

                            foreach (TipoFilaCarregamentoAlteracao tipoFilaCarregamentoAlteracao in tiposFilaCarregamentoAlteracao)
                                alteracao.Tipos.Add(tipoFilaCarregamentoAlteracao);

                            if (filaCarregamentoVeiculo.CentroCarregamento != null)
                                alteracao.CentrosCarregamento.Add(filaCarregamentoVeiculo.CentroCarregamento.Codigo);

                            if (filaCarregamentoVeiculo.Filial != null)
                                alteracao.Filiais.Add(filaCarregamentoVeiculo.Filial.Codigo);

                            if (filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular != null)
                                alteracao.GruposModelosVeicularesCarga.Add(filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular.Codigo);
                        }
                    }
                }

                NotificarFilaAlterada(alteracao);
            }
        }

        public void RecusarCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (!IsSituacaoPermiteAceitarOuRecusarCarga(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite recusar a carga");

            try
            {
                _unitOfWorkContainer.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga cargaDesvinculada = DesvincularCarga(filaCarregamentoVeiculo);

                AlterarSituacaoFilaCarregamentoMotorista(filaCarregamentoVeiculo, SituacaoFilaCarregamentoMotorista.CargaRecusada);

                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

                repositorioVeiculo.Atualizar(filaCarregamentoVeiculo);

                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Carga {ObterNumeroCarga(cargaDesvinculada)} recusada",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaRecusada
                });

                _unitOfWorkContainer.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();

                throw;
            }
        }

        public void RecusarCargaManualmente(int codigoFilaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (!IsSituacaoPermiteAceitarOuRecusarCargaManualmente(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite recusar a carga");

            if (filaCarregamentoVeiculo.Carga?.SituacaoCarga.IsSituacaoCargaEmitida() ?? false)
                throw new ServicoException("Situação atual da carga não permite recusar");

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();

            try
            {
                _unitOfWorkContainer.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga cargaDesvinculada = DesvincularCarga(filaCarregamentoVeiculo);

                if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                {
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;
                    AlterarPosicaoParaUltima(filaCarregamentoVeiculo, tipoServicoMultisoftware);
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = ObterCargaJanelaCarregamentoPorCarga(cargaDesvinculada);

                    if (cargaJanelaCarregamento?.CentroCarregamento?.TipoTransportador == TipoTransportadorCentroCarregamento.PorPrioridadeDeRota)
                    {
                        new CargaJanelaCarregamentoTransportador(_unitOfWorkContainer.UnitOfWork, ObterConfiguracaoEmbarcador()).DisponibilizarParaTransportadorPrioritarioPorRota(cargaJanelaCarregamento, tipoServicoMultisoftware);
                        new CargaJanelaCarregamento(_unitOfWorkContainer.UnitOfWork, ObterConfiguracaoEmbarcador()).AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
                    }
                }
                else
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado;

                repositorio.Atualizar(filaCarregamentoVeiculo);

                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Carga {ObterNumeroCarga(cargaDesvinculada)} recusada{(configuracaoGeralCarga.UtilizarProgramacaoCarga ? "" : " pelo portal")}",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.CargaRecusadaPortal
                });

                _unitOfWorkContainer.CommitChanges();

                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();

                throw;
            }
        }

        public void RecusarPreCarga(int codigoFilaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (!IsSituacaoPermiteAceitarOuRecusarPreCarga(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite recusar o pré planejamento");

            try
            {
                _unitOfWorkContainer.Start();

                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Pré planejamento {filaCarregamentoVeiculo.PreCarga.NumeroPreCarga} recusado",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.PreCargaRecusada
                });

                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

                DesvincularPreCarga(filaCarregamentoVeiculo);
                AlterarPosicaoParaUltima(filaCarregamentoVeiculo, tipoServicoMultisoftware);

                repositorio.Atualizar(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();

                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void RecusarSaida(int codigoFilaCarregamentoVeiculo, string justificativa)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (!IsSituacaoPermiteAceitarOuRecusarSaidaFila(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite recusar a saída.");

            if (string.IsNullOrWhiteSpace(justificativa))
                throw new ServicoException("Justificativa para recusar a saída não informada.");

            try
            {
                _unitOfWorkContainer.Start();

                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Saída da fila de carregamento recusada. Justificativa: {justificativa}",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.SolicitacaoSaidaFilaRecusada
                });

                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

                repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();

                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
                NotificarMobile(filaCarregamentoVeiculo, TipoNotificacaoMobile.SolicitacaoSaidaRecusada, $"Solicitação de saída da fila recusada. Justificativa: {justificativa}");
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> RemoverChecklist(Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio ocorrenciaPatio)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoVinculoExterno filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoVinculoExterno()
            {
                CentroCarregamento = ocorrenciaPatio.CentroCarregamento,
                Reboques = ocorrenciaPatio.Reboques,
                Situacao = SituacaoFilaCarregamentoVeiculo.EmChecklist,
                Tracao = ocorrenciaPatio.Tracao
            };

            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo = repositorio.ConsultarPorVinculoExterno(filtrosPesquisa);

            if (filasCarregamentoVeiculo.Count > 0)
            {
                FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

                foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in filasCarregamentoVeiculo)
                {
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.Disponivel;

                    repositorio.Atualizar(filaCarregamentoVeiculo);

                    servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = "Checklist concluído",
                        FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.ChecklistConcluido
                    });
                }
            }

            return filasCarregamentoVeiculo;
        }

        public void RemoverConjuntoMotorista(int codigoFilaCarregamentoVeiculo)
        {
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            try
            {
                _unitOfWorkContainer.Start();

                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista;

                RemoverConjuntoMotorista(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();

                List<TipoFilaCarregamentoAlteracao> tipos = new List<TipoFilaCarregamentoAlteracao>() { TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculo };

                if (filaCarregamentoMotorista != null)
                    tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoMotorista);

                NotificarFilaAlterada(filaCarregamentoVeiculo, tipos);

                // Notificar mobile
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void RemoverPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigoMotivoRetiradaFilaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento motivoRetiradaFilaCarregamento = ObterMotivoRetiradaFilaCarregamento(codigoMotivoRetiradaFilaCarregamento);

            foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in filasCarregamentoVeiculo)
            {
                if (!IsSituacaoPermiteRemoverFilaCarregamento(filaCarregamentoVeiculo))
                    throw new ServicoException("Situação não permite remover a fila de carregamento.");

                servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Removido da fila de carregamento. Motivo: {motivoRetiradaFilaCarregamento.Descricao}",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    MotivoRetiradaFilaCarregamento = motivoRetiradaFilaCarregamento,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.SaidaFila
                });

                RemoverFilaCarregamento(filaCarregamentoVeiculo, tipoServicoMultisoftware);
                NotificarFilaAlterada(filaCarregamentoVeiculo);
            }
        }

        public bool RemoverPorMotoristaJornadaEncerrada(Dominio.Entidades.Usuario motorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = ObterFilaCarregamentoVeiculoPorMotoristaNaFilaSemValidacaoNulo(motorista.Codigo);

            if (filaCarregamentoVeiculo == null)
                return false;

            if (!IsSituacaoPermiteRemoverFilaCarregamento(filaCarregamentoVeiculo))
                throw new ServicoException("Situação não permite remover a fila de carregamento.");

            new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
            {
                Descricao = "Removido da fila de carregamento. Motivo: Jornada do motorista encerrada",
                FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                OrigemAlteracao = _origemAlteracao,
                Tipo = TipoFilaCarregamentoVeiculoHistorico.SaidaFila
            });

            RemoverFilaCarregamento(filaCarregamentoVeiculo, tipoServicoMultisoftware);
            NotificarFilaAlterada(filaCarregamentoVeiculo);

            return true;
        }

        public void RemoverReversa(int codigoFilaCarregamentoVeiculo)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Fila de carregamento não encontrada.");

            RemoverReversa(filaCarregamentoVeiculo);
        }

        public void RemoverReversa(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (!IsSituacaoPermiteRemoverReversa(filaCarregamentoVeiculo))
                throw new ServicoException("A situação da fila de carregamento não permite remover a reversa.");

            try
            {
                _unitOfWorkContainer.Start();

                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

                filaCarregamentoVeiculo.Tipo = TipoFilaCarregamentoVeiculo.Vazio;

                if ((filaCarregamentoVeiculo.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) && IsConjuntoVeiculoEmChecklist(filaCarregamentoVeiculo.ConjuntoVeiculo))
                    filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.EmChecklist;

                repositorio.Atualizar(filaCarregamentoVeiculo);

                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = "Veículo removido da reversa",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.SaidaReversa
                });

                _unitOfWorkContainer.CommitChanges();

                NotificarSituacaoFilaAlterada(filaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void RemoverTracao(int codigoFilaCarregamentoVeiculo)
        {
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao == null)
                throw new ServicoException("A fila de carregamento não possui tração");

            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico> historicosFilaCarregamento = servicoHistorico.ObterTodos(filaCarregamentoVeiculo.Codigo);
            bool isVeiculoFilaCarregamentoAlterado = historicosFilaCarregamento.Any(o => o.Tipo == TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado);

            if (!isVeiculoFilaCarregamentoAlterado)
                throw new ServicoException("A fila de carregamento não permite remover somente a tração");

            if (filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista?.ConjuntoVeiculo != null)
                throw new ServicoException("A fila de carregamento não permite remover a tração do motorista");

            if (filaCarregamentoVeiculo.Carga != null)
                throw new ServicoException("A fila de carregamento não permite remover a tração com carga alocada");

            if (filaCarregamentoVeiculo.PreCarga != null)
                throw new ServicoException("A fila de carregamento não permite remover a tração com pré planejamento alocado");

            try
            {
                _unitOfWorkContainer.Start();

                servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"A tração {filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao.Placa_Formatada} foi removida",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado
                });

                filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao = null;

                new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.ConjuntoVeiculo);

                AtualizarTracaoCarga(filaCarregamentoVeiculo);
                AtualizarTracaoPreCarga(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();

                NotificarFilaAlterada(filaCarregamentoVeiculo, TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculo);
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void Reposicionar(int codigoFilaCarregamentoVeiculo, int novaPosicao, int codigoMotivo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string observacao = null)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Não foi possível encontrar a fila de carregamento.");

            if (!IsSituacaoPermiteAlterarPosicao(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite reposicionar.");

            Dominio.Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento motivo = ObterMotivoAlteracaoPosicaoFilaCarregamento(codigoMotivo);

            if (novaPosicao < 1)
                novaPosicao = 1;
            else
            {
                int ultimaPosicao = repositorio.BuscarUltimaPosicao(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao()
                {
                    CodigoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0,
                    CodigoFilial = filaCarregamentoVeiculo.Filial?.Codigo ?? 0,
                    CodigoModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo
                });

                if (novaPosicao > ultimaPosicao)
                    novaPosicao = ultimaPosicao;
            }

            AlterarPosicaoSituacaoNaFila(filaCarregamentoVeiculo, novaPosicao, motivo, tipoServicoMultisoftware, observacao);
        }

        public void SolicitarSaida(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, int codigoMotivoRetiradaFilaCarregamento)
        {
            if (!IsSituacaoPermiteRemoverFilaCarregamento(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite a saída");

            if (filaCarregamentoVeiculo.Carga?.SituacaoCarga.IsSituacaoCargaEmitida() ?? false)
                throw new ServicoException("Situação atual da carga não permite a saída");

            Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento motivoRetiradaFilaCarregamento = ObterMotivoRetiradaFilaCarregamento(codigoMotivoRetiradaFilaCarregamento);

            try
            {
                _unitOfWorkContainer.Start();

                new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario).Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"Solicitou saída da fila de carregamento. Motivo: {motivoRetiradaFilaCarregamento.Descricao}",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    MotivoRetiradaFilaCarregamento = motivoRetiradaFilaCarregamento,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.SolicitacaoSaidaFila
                });

                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

                filaCarregamentoVeiculo.Situacao = SituacaoFilaCarregamentoVeiculo.EmRemocao;

                repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);

                _unitOfWorkContainer.CommitChanges();
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = repositorio.BuscarPorCarga(cargaAtual.Codigo);

            foreach (var filaCarregamentoVeiculo in listaFilaCarregamentoVeiculo)
            {
                filaCarregamentoVeiculo.Carga = cargaNova;
                repositorio.Atualizar(filaCarregamentoVeiculo);
            }
        }

        public void VerificarDataProgramadaAlterarAutomaticamente(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS || tipoServicoMultisoftware == TipoServicoMultisoftware.TransportadorTerceiro)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = ObterConfiguracaoPreCarga();
            DateTime? dataProgramadaInicial = null;

            if (configuracaoPreCarga.DiasTransicaoAutomaticaFilaCarregamentoVeiculo > 0)
                dataProgramadaInicial = DateTime.Today.AddDays(-configuracaoPreCarga.DiasTransicaoAutomaticaFilaCarregamentoVeiculo);

            FilaCarregamentoVeiculoHistorico servicoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork);
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<int> codigosFilaCarregamentoVeiculoAlterarDataProgramadaAutomaticamente = repositorioFilaCarregamentoVeiculo.BuscarCodigosPorDataProgramadaAlterarAutomaticamente(dataProgramadaInicial);

            foreach (int codigoFilaCarregamentoVeiculo in codigosFilaCarregamentoVeiculoAlterarDataProgramadaAutomaticamente)
            {
                try
                {
                    _unitOfWorkContainer.UnitOfWork.Start();

                    Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigoFilaCarregamentoVeiculo, auditavel: false);
                    DateTime dataProgramadaAtual = filaCarregamentoVeiculo.DataProgramada.Value;
                    DateTime novaDataProgramada = filaCarregamentoVeiculo.DataProgramada.Value.AddDays(1);

                    if (repositorioFilaCarregamentoVeiculo.ExistePorDataProgramada(novaDataProgramada, filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0, filaCarregamentoVeiculo.Filial?.Codigo ?? 0, filaCarregamentoVeiculo.ConjuntoVeiculo.ObterCodigos(), filaCarregamentoVeiculo.Codigo))
                    {
                        _unitOfWorkContainer.UnitOfWork.Rollback();
                        continue;
                    }

                    AtualizarPosicoesFilaCarregamentoVeiculoRemovida(filaCarregamentoVeiculo, tipoServicoMultisoftware);

                    filaCarregamentoVeiculo.DataProgramada = novaDataProgramada;
                    filaCarregamentoVeiculo.DataProgramadaAlteradaAutomaticamente = true;
                    filaCarregamentoVeiculo.Posicao = repositorioFilaCarregamentoVeiculo.BuscarUltimaPosicao(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao()
                    {
                        CodigoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0,
                        CodigoFilial = filaCarregamentoVeiculo.Filial?.Codigo ?? 0,
                        CodigoModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo
                    });

                    int posicao = repositorioFilaCarregamentoVeiculo.BuscarProximaPosicao(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao()
                    {
                        CodigoCentroCarregamento = filaCarregamentoVeiculo.CentroCarregamento?.Codigo ?? 0,
                        CodigoFilaCarregamentoVeiculoDesconsiderar = filaCarregamentoVeiculo.Codigo,
                        CodigoFilial = filaCarregamentoVeiculo.Filial?.Codigo ?? 0,
                        CodigoModeloVeicularCarga = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : filaCarregamentoVeiculo.ConjuntoVeiculo.ModeloVeicularCarga.Codigo,
                        ConjuntoVeiculoDedicado = filaCarregamentoVeiculo.ConjuntoVeiculoDedicado,
                        DataEntrada = filaCarregamentoVeiculo.DataEntrada,
                        DataProgramada = filaCarregamentoVeiculo.DataProgramada,
                        DataProgramadaAlteradaAutomaticamente = filaCarregamentoVeiculo.DataProgramadaAlteradaAutomaticamente
                    });

                    AlterarPosicao(filaCarregamentoVeiculo, posicao, tipoServicoMultisoftware);
                    repositorioFilaCarregamentoVeiculo.Atualizar(filaCarregamentoVeiculo);
                    servicoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                    {
                        Descricao = $"Data de previsão de chegada alterada automaticamente de {dataProgramadaAtual.ToDateTimeString()} para {filaCarregamentoVeiculo.DataProgramada.Value.ToDateTimeString()}",
                        FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                        OrigemAlteracao = _origemAlteracao,
                        Tipo = TipoFilaCarregamentoVeiculoHistorico.DataProgramadaAlterada
                    });

                    _unitOfWorkContainer.UnitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    _unitOfWorkContainer.UnitOfWork.Rollback();
                    Log.TratarErro(excecao);
                }
            }
        }

        public void VincularCargaFilaCarregamentoCargaCancelada(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga != null)
            {
                Dominio.Entidades.Veiculo veiculo = null;
                if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                    veiculo = carga.VeiculosVinculados.FirstOrDefault();
                if (veiculo == null && carga.Veiculo != null)
                    veiculo = carga.Veiculo;

                if (veiculo != null)
                {
                    Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWorkContainer.UnitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

                    if (cargaJanelaCarregamento != null && cargaJanelaCarregamento.CentroCarregamento != null)
                    {
                        Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
                        Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPrimeiraComCargaCancelada(veiculo.Codigo, cargaJanelaCarregamento.CentroCarregamento.Codigo, cargaJanelaCarregamento.CentroCarregamento.Filial.Codigo);

                        if (filaCarregamentoVeiculo != null)
                        {
                            VincularCarga(filaCarregamentoVeiculo, carga, (filaCarregamentoVeiculoAtualizar) =>
                            {
                                if (carga.Veiculo != null && filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao != null && carga.Veiculo != filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao)
                                    filaCarregamentoVeiculoAtualizar.ConjuntoVeiculo.Tracao = carga.Veiculo;
                                else if (carga.Veiculo == null)
                                    filaCarregamentoVeiculoAtualizar.ConjuntoVeiculo.Tracao = null;

                                if (filaCarregamentoVeiculoAtualizar.ConjuntoMotorista.FilaCarregamentoMotorista == null)
                                {
                                    filaCarregamentoVeiculoAtualizar.ConjuntoMotorista.Motorista = carga.Motoristas?.FirstOrDefault() ?? null;

                                    new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculoAtualizar.ConjuntoMotorista);
                                }
                            }, tipoServicoMultisoftware);
                        }
                    }
                }
            }
        }

        public void VincularCargasSemTransportadorFilaCarregamentoDisponiveis(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento> listaFilaCarregamentoAgrupamento = repositorioFilaCarregamentoVeiculo.BuscarAgrupamentoPorDisponivel();

            if (listaFilaCarregamentoAgrupamento.Count > 0)
            {
                int prazoAlocarCargaAntesInicioCarregamento = ObterPrazoSituacaoCarga(SituacaoCargaJanelaCarregamento.SemTransportador, tempoSituacaoPadrao: 120);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWorkContainer.UnitOfWork);

                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento agrupamento in listaFilaCarregamentoAgrupamento)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCargaDisponibilizarFilaCarregamento(agrupamento.CodigoCentroCarregamento, agrupamento.CodigoModeloVeicularCarga, prazoAlocarCargaAntesInicioCarregamento, transportadorExclusivo: false);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamentoPermiteVincularAutomaticamente = (from cargaJanelaCarregamento in listaCargaJanelaCarregamento where cargaJanelaCarregamento.IsCargaDadosPermiteVincularFilaCarregamentoAutomaticamente() select cargaJanelaCarregamento).ToList();

                    if (listaCargaJanelaCarregamentoPermiteVincularAutomaticamente.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarDisponivelPorAgrupamento(agrupamento);

                        foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in listaFilaCarregamentoVeiculo)
                        {
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in listaCargaJanelaCarregamentoPermiteVincularAutomaticamente)
                            {
                                if (!IsPossuiRestricaoRodagem(filaCarregamentoVeiculo, cargaJanelaCarregamento.Carga, cargaJanelaCarregamento.InicioCarregamento))
                                {
                                    VincularCarga(filaCarregamentoVeiculo, cargaJanelaCarregamento.Carga, tipoServicoMultisoftware);

                                    listaCargaJanelaCarregamentoPermiteVincularAutomaticamente.Remove(cargaJanelaCarregamento);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void VincularCargasSemTransportadorFilaCarregamentoDisponiveisPorTransportadorExclusivo(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento> listaFilaCarregamentoAgrupamento = repositorioFilaCarregamentoVeiculo.BuscarAgrupamentoPorDisponivel();

            if (listaFilaCarregamentoAgrupamento.Count > 0)
            {
                int prazoAlocarCargaAntesInicioCarregamento = ObterPrazoSituacaoCarga(SituacaoCargaJanelaCarregamento.SemTransportador, tempoSituacaoPadrao: 120);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWorkContainer.UnitOfWork);

                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento agrupamento in listaFilaCarregamentoAgrupamento)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCargaDisponibilizarFilaCarregamento(agrupamento.CodigoCentroCarregamento, agrupamento.CodigoModeloVeicularCarga, prazoAlocarCargaAntesInicioCarregamento, transportadorExclusivo: true);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamentoPermiteVincularAutomaticamente = (from cargaJanelaCarregamento in listaCargaJanelaCarregamento where cargaJanelaCarregamento.IsCargaDadosPermiteVincularFilaCarregamentoAutomaticamente() select cargaJanelaCarregamento).ToList();

                    if (listaCargaJanelaCarregamentoPermiteVincularAutomaticamente.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarDisponivelPorAgrupamento(agrupamento);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in listaCargaJanelaCarregamentoPermiteVincularAutomaticamente)
                        {
                            foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in listaFilaCarregamentoVeiculo)
                            {
                                if (
                                    (filaCarregamentoVeiculo.ObterTransportador()?.Codigo == cargaJanelaCarregamento.TransportadorExclusivo.Codigo) &&
                                    !IsPossuiRestricaoRodagem(filaCarregamentoVeiculo, cargaJanelaCarregamento.Carga, cargaJanelaCarregamento.InicioCarregamento)
                                )
                                {
                                    VincularCarga(filaCarregamentoVeiculo, cargaJanelaCarregamento.Carga, observacaoAlocacao: "Transportador Exclusivo", tipoServicoMultisoftware);

                                    listaFilaCarregamentoVeiculo.Remove(filaCarregamentoVeiculo);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void VincularPreCargasSemTransportadorFilaCarregamentoDisponiveis(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento> listaFilaCarregamentoAgrupamento = repositorioFilaCarregamentoVeiculo.BuscarAgrupamentoPorDisponivel();

            if (listaFilaCarregamentoAgrupamento.Count > 0)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWorkContainer.UnitOfWork);

                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento agrupamento in listaFilaCarregamentoAgrupamento)
                {
                    int prazoAlocarPreCargaAntesInicioCarregamento = ObterPrazoAlocarPreCargaAntesInicioCarregamento(agrupamento.CodigoCentroCarregamento);
                    List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarDisponivelPorAgrupamento(agrupamento);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCargaDisponibilizarFilaCarregamento(agrupamento.CodigoCentroCarregamento, agrupamento.CodigoModeloVeicularCarga, prazoAlocarPreCargaAntesInicioCarregamento);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamentoPermiteVincularAutomaticamente = (from cargaJanelaCarregamento in listaCargaJanelaCarregamento where cargaJanelaCarregamento.IsCargaDadosPermiteVincularFilaCarregamentoAutomaticamente() select cargaJanelaCarregamento).ToList();

                    foreach (Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo in listaFilaCarregamentoVeiculo)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in listaCargaJanelaCarregamentoPermiteVincularAutomaticamente)
                        {
                            if (!IsPossuiRestricaoRodagem(filaCarregamentoVeiculo, cargaJanelaCarregamento.PreCarga, cargaJanelaCarregamento.InicioCarregamento))
                            {
                                VincularPreCarga(filaCarregamentoVeiculo, cargaJanelaCarregamento.PreCarga, tipoServicoMultisoftware);
                                AtualizarPreCarga(filaCarregamentoVeiculo);

                                listaCargaJanelaCarregamentoPermiteVincularAutomaticamente.Remove(cargaJanelaCarregamento);

                                break;
                            }
                        }
                    }
                }
            }
        }

        public void VincularFilaCarregamentoMotorista(int codigoFilaCarregamentoMotorista, int codigoFilaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork).BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Fila de carregamento não encontrada.");
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(_unitOfWorkContainer.UnitOfWork).BuscarPorCodigo(codigoFilaCarregamentoMotorista) ?? throw new ServicoException("Fila do motorista não encontrada.");

            try
            {
                _unitOfWorkContainer.Start();

                VincularFilaCarregamentoMotorista(filaCarregamentoMotorista, filaCarregamentoVeiculo, tipoServicoMultisoftware);

                _unitOfWorkContainer.CommitChanges();

                NotificarMotoristaCargaAdicionada(filaCarregamentoVeiculo);
                NotificarFilaAlterada(filaCarregamentoVeiculo, new List<TipoFilaCarregamentoAlteracao>() { TipoFilaCarregamentoAlteracao.FilaCarregamentoMotorista, TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculo });
            }
            catch (Exception)
            {
                _unitOfWorkContainer.Rollback();
                throw;
            }
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo VincularFilaCarregamentoMotorista(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCargaVazio = ObterTipoRetornoCargaVazio();

            return VincularFilaCarregamentoMotorista(filaCarregamentoMotorista, tipoRetornoCargaVazio, validarConjuntoVeiculoPermiteEntrarFilaCarregamento: false, tipoServicoMultisoftware);
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo VincularFilaCarregamentoMotorista(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista, Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga tipoRetornoCarga, bool validarConjuntoVeiculoPermiteEntrarFilaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = ObterFilaCarregamentoVeiculoDisponivelVincularFilaCarregamentoMotoristaPorMotorista(filaCarregamentoMotorista.Motorista.Codigo);

            if (filaCarregamentoVeiculo == null)
            {
                if (filaCarregamentoMotorista.ConjuntoVeiculo?.IsCompleto() ?? false)
                    filaCarregamentoVeiculo = Adicionar(filaCarregamentoMotorista, tipoRetornoCarga, tipoServicoMultisoftware);
                else if (validarConjuntoVeiculoPermiteEntrarFilaCarregamento)
                    throw new ServicoException("O conjunto de veículo do motorista não permite entrar na fila");

                return filaCarregamentoVeiculo;
            }

            return VincularFilaCarregamentoMotorista(filaCarregamentoMotorista, filaCarregamentoVeiculo, tipoServicoMultisoftware);
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo VincularFilaCarregamentoMotorista(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista filaCarregamentoMotorista, Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            FilaCarregamentoVeiculoHistorico servicoFilaCarregamentoVeiculoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);

            if (filaCarregamentoVeiculo.ConjuntoMotorista.IsCompleto() && (filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Codigo != filaCarregamentoMotorista.Motorista.Codigo))
                RemoverConjuntoMotorista(filaCarregamentoVeiculo);

            if (filaCarregamentoMotorista.ConjuntoVeiculo != null)
            {
                if (filaCarregamentoVeiculo.ConjuntoVeiculo.Equals(filaCarregamentoMotorista.ConjuntoVeiculo))
                    filaCarregamentoVeiculo.ConjuntoMotorista.Exclusivo = true;
                else if (filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Codigo != filaCarregamentoMotorista.ConjuntoVeiculo.Tracao?.Codigo)
                {
                    if ((filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao != null) && (filaCarregamentoMotorista.ConjuntoVeiculo.Tracao != null))
                        throw new ServicoException("Os conjuntos de veículo do motorista e da fila de carregamento não permitem o vínculo.");

                    if (filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao == null)
                    {
                        ValidarVeiculo(filaCarregamentoMotorista.ConjuntoVeiculo.Tracao, filaCarregamentoMotorista.CentroCarregamento?.NaoValidarIntegracaoGR ?? false);

                        filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao = filaCarregamentoMotorista.ConjuntoVeiculo.Tracao;

                        servicoFilaCarregamentoVeiculoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                        {
                            Descricao = $"A tração {filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao.Placa_Formatada} foi informada",
                            FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                            OrigemAlteracao = _origemAlteracao,
                            Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoVeiculoAlterado
                        });

                        AtualizarTracaoCarga(filaCarregamentoVeiculo);
                        AtualizarTracaoPreCarga(filaCarregamentoVeiculo);
                    }
                }
            }

            if (filaCarregamentoVeiculo.ConjuntoMotorista.Motorista?.Codigo != filaCarregamentoMotorista.Motorista.Codigo)
            {
                ValidarMotorista(filaCarregamentoMotorista.Motorista, filaCarregamentoVeiculo.CentroCarregamento?.NaoValidarIntegracaoGR ?? false);

                filaCarregamentoVeiculo.ConjuntoMotorista.Motorista = filaCarregamentoMotorista.Motorista;

                servicoFilaCarregamentoVeiculoHistorico.Adicionar(new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoHistoricoAdicionar()
                {
                    Descricao = $"O motorista {filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Nome} ({filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.CPF_Formatado}) foi adicionado",
                    FilaCarregamentoVeiculo = filaCarregamentoVeiculo,
                    OrigemAlteracao = _origemAlteracao,
                    Tipo = TipoFilaCarregamentoVeiculoHistorico.ConjuntoMotoristaAlterado
                });
            }

            filaCarregamentoVeiculo.ConjuntoMotorista.FilaCarregamentoMotorista = filaCarregamentoMotorista;

            ValidarTransportadorIgual(filaCarregamentoVeiculo);
            AtualizarMotoristaCarga(filaCarregamentoVeiculo);
            AtualizarMotoristaPreCarga(filaCarregamentoVeiculo);
            AtualizarSituacao(filaCarregamentoVeiculo, tipoServicoMultisoftware);

            new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoMotorista(_unitOfWorkContainer.UnitOfWork).Atualizar(filaCarregamentoVeiculo.ConjuntoMotorista);

            return filaCarregamentoVeiculo;
        }

        public void RealocarVeiculoNaFila(int codigoVeiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool remocaoAutomatica = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFilaCarregamentoVeiculo situacao = SituacaoFilaCarregamentoVeiculo.Disponivel)
        {

            _unitOfWorkContainer.UnitOfWork.Start();

            try
            {
                Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo repFilaCarregamentoConjuntoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo(_unitOfWorkContainer.UnitOfWork);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo();
                FilaCarregamentoVeiculoHistorico servicoFilaCarregamentoVeiculoHistorico = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork, _usuario);
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo();

                conjuntoVeiculo = repFilaCarregamentoConjuntoVeiculo.BuscarUltimoConjuntoPorVeiculo(codigoVeiculo);

                if (conjuntoVeiculo != null)
                {
                    filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarFilaVeiculoPorCodigoConjuntoVeiculo(conjuntoVeiculo.Codigo);
                    string placa = conjuntoVeiculo.Tracao != null ? conjuntoVeiculo.Tracao.Placa : string.Join(".", conjuntoVeiculo.Reboques.Select(o => o.Placa));
                    if (filaCarregamentoVeiculo == null)
                    {

                        Servicos.Log.TratarErro($"Fila de carregamento não encontrada para o veiculo {placa}");
                        return;
                    }

                    Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico historicoUltimaPosicaoVeiculo = servicoFilaCarregamentoVeiculoHistorico.ObterUltimoPorTipo(filaCarregamentoVeiculo.Codigo, situacao == SituacaoFilaCarregamentoVeiculo.Disponivel ? TipoFilaCarregamentoVeiculoHistorico.CargaAlocada : TipoFilaCarregamentoVeiculoHistorico.EntradaFila);

                    if (historicoUltimaPosicaoVeiculo != null)
                    {
                        AlterarPosicaoSituacaoFila(filaCarregamentoVeiculo, historicoUltimaPosicaoVeiculo.Posicao, situacao, TipoServicoMultisoftware.MultiTMS, placa);
                    }

                }

                _unitOfWorkContainer.UnitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao realocar Veiculo - ", ex.Message);
                _unitOfWorkContainer.UnitOfWork.Rollback();
            }

        }

        #endregion

        #region Métodos Públicos de Consulta

        public bool ExisteFilaCarregamentoVeiculoPorCarga(int codigoCarga)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorio.ExistePorCarga(codigoCarga);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento ObterCargaJanelaCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if ((filaCarregamentoVeiculo.Carga != null) && (filaCarregamentoVeiculo.PreCarga == null))
                return ObterCargaJanelaCarregamentoPorCarga(filaCarregamentoVeiculo.Carga);

            return ObterCargaJanelaCarregamentoPorPreCarga(filaCarregamentoVeiculo.PreCarga);
        }

        public string ObterDescricaoMotoristaIndisponivelAdicionarFilaCarregamento(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            return ObterDescricaoIndisponivelAdicionarFilaCarregamento(filaCarregamentoVeiculo, descricaotipo: "O motorista");
        }

        public string ObterDetalhesCarga(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo)
        {
            if (!IsSituacaoPermiteVisualizarDetalhesCarga(filaCarregamentoVeiculo))
                throw new ServicoException("Situação da fila de carregamento não permite visualizar os detalhes da carga");

            string numeroCarga = ObterNumeroCarga(filaCarregamentoVeiculo.Carga);

            if ((filaCarregamentoVeiculo.CentroCarregamento?.TempoEncostaDoca > 0) && !string.IsNullOrWhiteSpace(filaCarregamentoVeiculo.Carga.NumeroDoca))
                return $"Você foi alocado na carga {numeroCarga} da {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}. Favor encostar na doca {filaCarregamentoVeiculo.Carga.NumeroDoca} em {filaCarregamentoVeiculo.CentroCarregamento.TempoEncostaDoca} minutos";

            if (!string.IsNullOrWhiteSpace(filaCarregamentoVeiculo.Carga.NumeroDoca))
                return $"Você foi alocado na carga {numeroCarga} da {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}. Favor encostar na doca {filaCarregamentoVeiculo.Carga.NumeroDoca}";

            return $"Você foi alocado na carga {numeroCarga} da {filaCarregamentoVeiculo.Descricao.FirstLetterToLower()}";
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo ObterFilaCarregamentoVeiculo(int codigoVeiculo, int codigoMotoristaDesconsiderar)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarPrimeiraAtiva(codigoVeiculo, codigoMotoristaDesconsiderar);
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo ObterFilaCarregamentoVeiculoDisponivelVincularFilaCarregamentoMotoristaPorMotorista(int codigoMotorista)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarDisponivelPorMotoristaVincularFilaCarregamentoMotorista(codigoMotorista);
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo ObterFilaCarregamentoVeiculoPorMotoristaNaFila(int codigoMotorista)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarAtivaPorMotoristaNaFila(codigoMotorista) ?? throw new ServicoException("Fila de carregamento não encontrada para o motorista.");
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo ObterFilaCarregamentoVeiculoPorMotoristaNaFilaSemValidacaoNulo(int codigoMotorista)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarAtivaPorMotoristaNaFila(codigoMotorista);
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo ObterFilaCarregamentoVeiculoPorPreCarga(int codigoPreCarga)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarPorPreCarga(codigoPreCarga);
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoInformacao ObterInformacoesPorCarga(int codigoCarga)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);
            Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorio.BuscarPorCargaAtiva(codigoCarga).FirstOrDefault();

            if (filaCarregamentoVeiculo == null)
                return null;

            List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico> historicos = new FilaCarregamentoVeiculoHistorico(_unitOfWorkContainer.UnitOfWork).ObterTodos(filaCarregamentoVeiculo.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoInformacao informacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoInformacao()
            {
                DataEntrada = (from historico in historicos where historico.Tipo == TipoFilaCarregamentoVeiculoHistorico.EntradaFila && historico.Posicao > 0 select historico.Data).FirstOrDefault(),
                DataCargaAceita = (from historico in historicos where historico.Tipo == TipoFilaCarregamentoVeiculoHistorico.CargaAceita select (DateTime?)historico.Data).FirstOrDefault(),
                PosicaoEntrada = (from historico in historicos where historico.Tipo == TipoFilaCarregamentoVeiculoHistorico.EntradaFila && historico.Posicao > 0 select historico.Posicao).FirstOrDefault(),
                Reboques = filaCarregamentoVeiculo.ConjuntoVeiculo.Reboques.ToList(),
                Tracao = filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao
            };

            return informacao;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ObterListaFilaCarregamentoVeiculoAguardandoChegadaVeiculo(List<int> listaCodigoCentroCarregamento)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarAguardandoChegadaveiculo(listaCodigoCentroCarregamento);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ObterListaFilaCarregamentoVeiculoEmViagem(Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo filaCarregamentoConjuntoVeiculo, int codigoCentroCarregamento)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarEmViagemPorVeiculosECentroCarregamento(filaCarregamentoConjuntoVeiculo.ObterCodigos(), codigoCentroCarregamento);
        }

        public static OrigemAlteracaoFilaCarregamento ObterOrigemAlteracaoFilaCarregamento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            switch (tipoServicoMultisoftware)
            {
                case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe:
                case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS:
                    return OrigemAlteracaoFilaCarregamento.Transportador;

                case AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador:
                    return OrigemAlteracaoFilaCarregamento.Embarcador;

                default: return OrigemAlteracaoFilaCarregamento.Sistema;
            }
        }

        public Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga ObterTipoRetornoCarga(int codigoTipoRetornoCarga)
        {
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorio = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarPorCodigo(codigoTipoRetornoCarga) ?? throw new ServicoException("Tipo de retorno de carga não encontrado");
        }

        public Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga ObterTipoRetornoCargaSemValidacaoNulo(int codigoTipoRetornoCarga)
        {
            Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga repositorio = new Repositorio.Embarcador.Cargas.Retornos.TipoRetornoCarga(_unitOfWorkContainer.UnitOfWork);

            return repositorio.BuscarPorCodigo(codigoTipoRetornoCarga);
        }

        public Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga ObterTipoRetornoCarga(TipoFilaCarregamentoVeiculo tipo)
        {
            if (tipo == TipoFilaCarregamentoVeiculo.Vazio)
                return ObterTipoRetornoCargaVazio();

            return ObterTipoRetornoCargaReversa();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoPunicao> ObterCentroCarregamentoPunicao(int codigoCentroCarregamento)
        {
            var rep = new Repositorio.Embarcador.Logistica.CentroCarregamentoPunicao(_unitOfWorkContainer.UnitOfWork);
            var punicoes = rep.BuscarPorCentroDeCarregamento(codigoCentroCarregamento);
            return punicoes;
        }

        #endregion
    }
}
