using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.PreCarga
{
    public class PreCargaOfertaTransportador
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga _configuracaoPreCarga;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public PreCargaOfertaTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void DefinirDataLiberacao(Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta preCargaOferta, Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete)
        {
            if (configuracaoRotaFrete == null)
                return;

            if (!configuracaoRotaFrete.HoraEnvioTransportadorRota.HasValue || !configuracaoRotaFrete.EnviarTransportadorRotaConfigurado())
                return;

            DateTime dataLiberacao = preCargaOferta.PreCarga.DataPrevisaoEntrega.Value.Date.AddSeconds(configuracaoRotaFrete.HoraEnvioTransportadorRota.Value.TotalSeconds);
            int diasAntecedenciaDescontar = configuracaoRotaFrete.DiasAntecedenciaEnvioTransportadorRota;

            while (diasAntecedenciaDescontar > 0)
            {
                dataLiberacao = dataLiberacao.AddDays(-1);

                if (configuracaoRotaFrete.EnviarTransportadorRota(dataLiberacao))
                    diasAntecedenciaDescontar--;
            }

            preCargaOferta.DataLiberacao = dataLiberacao;
        }

        private void DisponibilizarParaTransportadorPorRota(Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta preCargaOferta)
        {
            if ((preCargaOferta.PreCarga.SituacaoPreCarga == SituacaoPreCarga.CargaGerada) || (preCargaOferta.PreCarga.Empresa != null))
                return;

            // Não controlaremos data  #59219
            //if (preCargaOferta.DataLiberacao.HasValue && (preCargaOferta.DataLiberacao.Value > DateTime.Now))
            //    return;

            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> configuracaoRotaFreteEmpresas = ObterConfiguracaoRotaFreteEmpresas(preCargaOferta.PreCarga);

            if (configuracaoRotaFreteEmpresas.Count == 0)
                return;

            Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador repositorioPreCargaOfertaTransportador = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador(_unitOfWork);

            if (repositorioPreCargaOfertaTransportador.VerificarExisteDesbloqueadasSemRejeicao(preCargaOferta.Codigo, TipoPreCargaOfertaTransportador.PorRota))
                return;

            if (repositorioPreCargaOfertaTransportador.VerificarExisteBloqueadas(preCargaOferta.Codigo, TipoPreCargaOfertaTransportador.PorRota))
            {
                repositorioPreCargaOfertaTransportador.DesbloquearTodas(preCargaOferta.Codigo, TipoPreCargaOfertaTransportador.PorRota);
                return;
            }

            if (repositorioPreCargaOfertaTransportador.VerificarExisteDesbloqueadasSemRejeicao(preCargaOferta.Codigo, TipoPreCargaOfertaTransportador.PorRotaGrupo))
                return;

            if (repositorioPreCargaOfertaTransportador.VerificarExisteBloqueadas(preCargaOferta.Codigo, TipoPreCargaOfertaTransportador.PorRotaGrupo))
            {
                repositorioPreCargaOfertaTransportador.DesbloquearTodas(preCargaOferta.Codigo, TipoPreCargaOfertaTransportador.PorRotaGrupo);
                return;
            }

            // oferta pela rota shere primario 
            if (!repositorioPreCargaOfertaTransportador.VerificarExiste(preCargaOferta.Codigo, TipoPreCargaOfertaTransportador.PorRota))
            {
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCargaOferta repositorioPreCargaOferta = new Repositorio.Embarcador.PreCargas.PreCargaOferta(_unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> ofertas = ObterRotaFreteEmpresaOfertas(preCargaOferta.PreCarga, configuracaoRotaFreteEmpresas);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> ofertasEscolhidas = ofertas.Where(oferta => oferta.Tipo == TipoHistoricoOfertaTransportador.Escolhida).ToList();

                if (ofertasEscolhidas.Count > 0)
                {
                    // Não controlaremos data chamado  #59219
                    //if (!preCargaOferta.DataLiberacao.HasValue)
                    //{
                    //    DefinirDataLiberacao(preCargaOferta, configuracaoRotaFreteEmpresas.FirstOrDefault().ConfiguracaoRotaFrete);
                    //    
                    //    if (preCargaOferta.DataLiberacao.HasValue && (preCargaOferta.DataLiberacao.Value > DateTime.Now))
                    //    {
                    //        repositorioPreCargaOferta.Atualizar(preCargaOferta);
                    //        return;
                    //    }
                    //}

                    preCargaOferta.Situacao = SituacaoPreCargaOferta.Liberada;

                    repositorioPreCargaOferta.Atualizar(preCargaOferta);

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = ObterConfiguracaoPreCarga();
                    int tempoAguardarConfirmacaoTransportador = configuracaoPreCarga.TempoAguardarConfirmacaoTransportador;
                    DateTime? dataLimiteConfirmacao = (tempoAguardarConfirmacaoTransportador > 0) ? (DateTime?)DateTime.Now.AddMinutes(tempoAguardarConfirmacaoTransportador) : null;

                    for (int i = 0, totalOfertas = ofertasEscolhidas.Count; i < totalOfertas; i++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta oferta = ofertasEscolhidas[i];
                        Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador = repositorioPreCargaOfertaTransportador.BuscarPorOfertaETransportador(preCargaOferta.Codigo, oferta.Empresa.Codigo);

                        if (ofertaTransportador != null)
                            continue;

                        ofertaTransportador = new Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador()
                        {
                            PreCargaOferta = preCargaOferta,
                            Transportador = oferta.Empresa,
                            HorarioLimiteConfirmacao = dataLimiteConfirmacao,
                            Tipo = TipoPreCargaOfertaTransportador.PorRota,
                            Situacao = SituacaoPreCargaOfertaTransportador.AguardandoAceite
                        };

                        repositorioPreCargaOfertaTransportador.Inserir(ofertaTransportador);
                        SalvarHistoricoOfertaPorRota(ofertaTransportador, ofertas);
                    }

                    return;
                }
            }

            
            //oferta fora rora shere secundario 
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            List<Dominio.Entidades.Empresa> transportadoresPorRotaGrupo = repositorioCargaJanelaCarregamentoTransportador.BuscarTransportadoresPorPrioridadeRotaGrupo(preCargaOferta.PreCarga.Filial?.Codigo ?? 0, preCargaOferta.PreCarga.TipoDeCarga?.Codigo ?? 0, diasHistorico: 180);
            List<int> codigosTransportadoresRejeitaramOfertaPorRotaGrupo = repositorioPreCargaOfertaTransportador.BuscarCodigosTransportadoresRejeitaramOferta(preCargaOferta.Codigo, TipoPreCargaOfertaTransportador.PorRotaGrupo);
            foreach (Dominio.Entidades.Empresa transportador in transportadoresPorRotaGrupo)
            {
                if (codigosTransportadoresRejeitaramOfertaPorRotaGrupo.Any(transportadorRejeitouOferta => transportadorRejeitouOferta == transportador.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador = new Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador()
                {
                    PreCargaOferta = preCargaOferta,
                    Transportador = transportador,
                    Tipo = TipoPreCargaOfertaTransportador.PorRotaGrupo,
                    Situacao = SituacaoPreCargaOfertaTransportador.Disponivel
                };

                repositorioPreCargaOfertaTransportador.Inserir(ofertaTransportador);
                SalvarHistoricoAlteracao(ofertaTransportador, "Pré planejamento disponibilizado para o transportador fora da rota");
            }
        }

        private int ObterCodigoModeloVeicularCargaParaDisponibilizarParaTransportadores(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

            return configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga ? preCarga.ModeloVeicularCarga?.Codigo ?? 0 : 0;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga ObterConfiguracaoPreCarga()
        {
            if (_configuracaoPreCarga == null)
                _configuracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoPreCarga;
        }

        private Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete ObterConfiguracaoRotaFrete(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Dominio.Entidades.Localidade localidadeOrigem = preCarga.Filial?.Localidade;

            if (localidadeOrigem == null)
                return null;

            Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete repositorioConfiguracaoRotaFrete = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(_unitOfWork);

            List<(int Localidade, string Estado)> localidadesPorDestino = repositorioPreCargaDestino.BuscarLocalidadeEEstadoPorPreCarga(preCarga.Codigo);
            List<(int Localidade, string Estado)> localidadesPorRegiaoDestino = repositorioPreCargaRegiaoDestino.BuscarLocalidadeEEstadoPorPreCarga(preCarga.Codigo);
            List<string> siglasEstadosDestino = repositorioPreCargaEstadoDestino.BuscarSiglasEstadosDestinoPorPreCarga(preCarga.Codigo);
            List<int> codigosLocalidadesDestino = new List<int>();

            codigosLocalidadesDestino.AddRange(localidadesPorDestino.Select(localidade => localidade.Localidade).ToList());
            codigosLocalidadesDestino.AddRange(localidadesPorRegiaoDestino.Select(localidade => localidade.Localidade).ToList());
            siglasEstadosDestino.AddRange(localidadesPorDestino.Select(localidade => localidade.Estado).ToList());
            siglasEstadosDestino.AddRange(localidadesPorRegiaoDestino.Select(localidade => localidade.Estado).ToList());

            Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete = repositorioConfiguracaoRotaFrete.BuscarPrimeiraDisponivel(localidadeOrigem.Codigo, preCarga.Filial.Codigo, siglasEstadosDestino, codigosLocalidadesDestino, preCarga.TipoDeCarga?.Codigo ?? 0, preCarga.ModeloVeicularCarga?.Codigo ?? 0);

            return configuracaoRotaFrete;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> ObterConfiguracaoRotaFreteEmpresas(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete = ObterConfiguracaoRotaFrete(preCarga);

            if (configuracaoRotaFrete == null)
                return new List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa>();

            int codigoModeloVeicularCarga = ObterCodigoModeloVeicularCargaParaDisponibilizarParaTransportadores(preCarga);
            Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa repositorioConfiguracaoRotaFreteEmpresa = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> configuracaoRotaFreteEmpresas = repositorioConfiguracaoRotaFreteEmpresa.BuscarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo, codigoModeloVeicularCarga);

            return configuracaoRotaFreteEmpresas;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> ObterRotaFreteEmpresaOfertas(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> configuracaoRotaFreteEmpresas)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            DateTime dataInicial = preCarga.DataPrevisaoEntrega.Value.FirstDayOfMonth();
            DateTime dataFinal = preCarga.DataPrevisaoEntrega.Value.LastDayOfMonth();
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            int codigoConfiguracaoRotaFrete = configuracaoRotaFreteEmpresas.FirstOrDefault().ConfiguracaoRotaFrete.Codigo;
            int codigoModeloVeicularCarga = ObterCodigoModeloVeicularCargaParaDisponibilizarParaTransportadores(preCarga);
            int quantidadeCargasPorPeriodo = repositorioCarga.ContarQuantidadeCargaPorConfiguracaoRota(codigoConfiguracaoRotaFrete, codigoCargaDesconsiderar: 0, dataInicial, dataFinal, codigoModeloVeicularCarga);
            int quantidadePreCargasPorPeriodo = repositorioPreCarga.ContarQuantidadePreCargaPorConfiguracaoRota(codigoConfiguracaoRotaFrete, preCarga.Codigo, dataInicial, dataFinal, codigoModeloVeicularCarga);
            int quantidadeCargasEPreCargasPorPeriodo = (quantidadeCargasPorPeriodo + quantidadePreCargasPorPeriodo);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> rotaFreteEmpresaOfertas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta>();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa> rotaFreteEmpresas = configuracaoRotaFreteEmpresas
                .Select(configuracaoPorEmpresa => new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa()
                {
                    ConfiguracaoRotaFrete = configuracaoPorEmpresa.ConfiguracaoRotaFrete,
                    Descricao = configuracaoPorEmpresa.Descricao,
                    Empresa = configuracaoPorEmpresa.Empresa,
                    PercentualCargasDaRota = configuracaoPorEmpresa.PercentualCargasDaRota,
                    Prioridade = configuracaoPorEmpresa.Prioridade
                })
                .ToList();

            if (configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorPrioridade)
                rotaFreteEmpresas = rotaFreteEmpresas.OrderBy(o => o.Prioridade).ToList();
            else
                rotaFreteEmpresas = rotaFreteEmpresas.OrderByDescending(o => o.PercentualCargasDaRota).ToList();

            List<(Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa RotaFreteEmpresa, decimal PercentualCargas, int Prioridade)> ofertas = new List<(Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa RotaFreteEmpresa, decimal PercentualCargas, int Prioridade)>();

            for (int i = 0, totalRotaFreteEmpresas = rotaFreteEmpresas.Count; i < totalRotaFreteEmpresas; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa rotaFreteEmpresa = rotaFreteEmpresas[i];
                decimal percentual = 0m;

                if (quantidadeCargasEPreCargasPorPeriodo > 0)
                {
                    int quantidadeCargasPorPeriodoEEmpresa = repositorioCarga.ContarQuantidadeCargaPorConfiguracaoRota(rotaFreteEmpresa.ConfiguracaoRotaFrete.Codigo, codigoCargaDesconsiderar: 0, dataInicial, dataFinal, codigoModeloVeicularCarga, rotaFreteEmpresa.Empresa.Codigo);
                    int quantidadePreCargasPorPeriodoEEmpresa = repositorioPreCarga.ContarQuantidadePreCargaPorConfiguracaoRota(rotaFreteEmpresa.ConfiguracaoRotaFrete.Codigo, preCarga.Codigo, dataInicial, dataFinal, codigoModeloVeicularCarga, rotaFreteEmpresa.Empresa.Codigo);
                    int quantidadeCargasEPreCargasPorPeriodoEEmpresa = (quantidadeCargasPorPeriodoEEmpresa + quantidadePreCargasPorPeriodoEEmpresa);
                    decimal resultado = (decimal)(quantidadeCargasEPreCargasPorPeriodoEEmpresa * 100) / quantidadeCargasEPreCargasPorPeriodo;

                    percentual = Math.Round(resultado, 2, MidpointRounding.AwayFromZero);
                }

                ofertas.Add(ValueTuple.Create(rotaFreteEmpresa, percentual, rotaFreteEmpresa.Prioridade));
            }

            for (int i = 0, totalOfertas = ofertas.Count; i < totalOfertas; i++)
            {
                (Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa RotaFreteEmpresa, decimal PercentualCargas, int Prioridade) oferta = ofertas[i];
                Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta rotaFreteEmpresaOferta = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta()
                {
                    Descricao = oferta.RotaFreteEmpresa.Descricao,
                    Empresa = oferta.RotaFreteEmpresa.Empresa,
                    Ordem = i + 1,
                    PercentualCargas = oferta.PercentualCargas,
                    PercentualConfigurado = oferta.RotaFreteEmpresa.PercentualCargasDaRota,
                    Prioridade = oferta.Prioridade,
                    Tipo = TipoHistoricoOfertaTransportador.Registrada
                };

                if (configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorPrioridade || (rotaFreteEmpresaOferta.PercentualCargas <= oferta.RotaFreteEmpresa.PercentualCargasDaRota))
                    rotaFreteEmpresaOferta.Tipo = TipoHistoricoOfertaTransportador.Escolhida;

                rotaFreteEmpresaOfertas.Add(rotaFreteEmpresaOferta);
            }

            return rotaFreteEmpresaOfertas;
        }

        private void SalvarHistoricoOfertaPorRota(Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> ofertas)
        {
            Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico repositorioHistorico = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta repositorioHistoricoOferta = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta(_unitOfWork);
            Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico historico = new Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico()
            {
                PreCargaOfertaTransportador = ofertaTransportador,
                Data = DateTime.Now,
                Descricao = "Pré planejamento disponibilizado para o transportador por rota",
                Tipo = TipoPreCargaOfertaTransportadorHistorico.OfertaPorRota
            };

            repositorioHistorico.Inserir(historico);

            for (int i = 0, totalOfertas = ofertas.Count; i < totalOfertas; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta oferta = ofertas[i];
                Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta historicoOferta = new Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta()
                {
                    PreCargaOfertaTransportadorHistorico = historico,
                    Descricao = oferta.Descricao,
                    Empresa = oferta.Empresa,
                    Ordem = oferta.Ordem,
                    PercentualCargas = oferta.PercentualCargas,
                    PercentualConfigurado = oferta.PercentualConfigurado,
                    Prioridade = oferta.Prioridade,
                    Tipo = oferta.Tipo
                };

                repositorioHistoricoOferta.Inserir(historicoOferta);
            }
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void DisponibilizarParaTransportadoresPorDataLiberacao()
        {
            Repositorio.Embarcador.PreCargas.PreCargaOferta repositorioPreCargaOferta = new Repositorio.Embarcador.PreCargas.PreCargaOferta(_unitOfWork);
            List<int> codigosPreCargaOfertaLiberar = repositorioPreCargaOferta.BuscarCodigosPreCargaOfertaLiberar();

            foreach (int codigosPreCargaOferta in codigosPreCargaOfertaLiberar)
            {
                try
                {
                    _unitOfWork.FlushAndClear();
                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta preCargaOferta = repositorioPreCargaOferta.BuscarPorCodigo(codigosPreCargaOferta, auditavel: false);
                    DisponibilizarParaTransportadorPorRota(preCargaOferta);

                    _unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    Log.TratarErro(excecao);
                }
            }
        }

        public void DisponibilizarParaTransportadorPorRota(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            if (!preCarga.ProgramacaoCarga)
                return;

            Repositorio.Embarcador.PreCargas.PreCargaOferta repositorioPreCargaOferta = new Repositorio.Embarcador.PreCargas.PreCargaOferta(_unitOfWork);
            Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta preCargaOferta = repositorioPreCargaOferta.BuscarPorPreCarga(preCarga.Codigo);

            if (preCargaOferta == null)
            {
                preCargaOferta = new Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta()
                {
                    PreCarga = preCarga,
                    Situacao = SituacaoPreCargaOferta.AguardandoLiberacao
                };

                repositorioPreCargaOferta.Inserir(preCargaOferta);
            }
            else if (preCargaOferta.Situacao == SituacaoPreCargaOferta.Finalizada)
                return;

            DisponibilizarParaTransportadorPorRota(preCargaOferta);
        }

        public void RejeitarOfertaTransportadorPorTempoAceiteEncerrado()
        {
            Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador repositorioPreCargaOfertaTransportador = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador(_unitOfWork);
            List<int> listaCodigoOfertaTransportador = repositorioPreCargaOfertaTransportador.BuscarCodigosPorTempoAceiteEncerrado(limiteRegistros: 5);

            if (listaCodigoOfertaTransportador.Count == 0)
                return;

            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);

            foreach (int codigoOfertaTransportador in listaCodigoOfertaTransportador)
            {
                try
                {
                    _unitOfWork.FlushAndClear();
                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador = repositorioPreCargaOfertaTransportador.BuscarPorCodigo(codigoOfertaTransportador, auditavel: false);

                    ofertaTransportador.HorarioLimiteConfirmacao = null;
                    ofertaTransportador.Situacao = SituacaoPreCargaOfertaTransportador.Rejeitada;
                    ofertaTransportador.PreCargaOferta.PreCarga.Empresa = null;

                    repositorioPreCarga.Atualizar(ofertaTransportador.PreCargaOferta.PreCarga);
                    repositorioPreCargaOfertaTransportador.Atualizar(ofertaTransportador);
                    SalvarHistoricoAlteracao(ofertaTransportador, "Pré planejamento rejeitado por tempo de confirmação encerrado");
                    DisponibilizarParaTransportadorPorRota(ofertaTransportador.PreCargaOferta);

                    _unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    Log.TratarErro(excecao);
                }
            }
        }

        public void SalvarHistoricoAlteracao(Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador, string mensagem)
        {
            SalvarHistoricoAlteracao(ofertaTransportador, mensagem, usuario: null);
        }

        public void SalvarHistoricoAlteracao(Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador ofertaTransportador, string mensagem, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico repositorioHistorico = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico(_unitOfWork);
            Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico historico = new Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico()
            {
                PreCargaOfertaTransportador = ofertaTransportador,
                Data = DateTime.Now,
                Descricao = mensagem,
                Tipo = TipoPreCargaOfertaTransportadorHistorico.RegistroAlteracao,
                Usuario = usuario
            };

            repositorioHistorico.Inserir(historico);
        }

        #endregion Métodos Públicos
    }
}
