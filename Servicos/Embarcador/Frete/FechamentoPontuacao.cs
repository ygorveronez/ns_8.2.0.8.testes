using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public sealed class FechamentoPontuacao
    {
        #region Atributos

        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador> _listaAdvertencia;
        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria> _listaPontuacaoPorModeloCarroceria;
        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao> _listaPontuacaoPorPessoaClassificacao;
        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga> _listaPontuacaoPorQuantidadeCarga;
        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao> _listaPontuacaoPorQuantidadeCargaGanhaCotacao;
        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade> _listaPontuacaoPorTempoAtividade;
        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao> _listaPontuacaoPorTipoOperacao;
        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga> _listaPontuacaoPorTipoCarga;
        private List<Dominio.Entidades.Empresa> _listaPontuacaoFixa;

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public FechamentoPontuacao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private void AdicionarPontuacaoTransportadores(Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao fechamentoPontuacao)
        {
            Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador repositorioFechamentoPontuacaoTransportador = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador(_unitOfWork);
            Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra repositorioFechamentoPontuacaoTransportadorRegra = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra(_unitOfWork);
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(_unitOfWork);
            List<Dominio.Entidades.Empresa> transportadores = repositorioTransportador.BuscarAtivas();
            bool fechamentoMesAnterior = fechamentoPontuacao.FechamentoMesAnterior;

            foreach (Dominio.Entidades.Empresa transportador in transportadores)
            {
                if (!transportador.DataInicioAtividade.HasValue)
                    continue;

                Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador fechamentoPontuacaoTransportador = repositorioFechamentoPontuacaoTransportador.BuscarPorFechamentoPontuacaoETransportador(fechamentoPontuacao.Codigo, transportador.Codigo);

                if (fechamentoPontuacaoTransportador == null)
                {
                    List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> listaRegraPontuacao = ObterRegrasPontuacao(fechamentoPontuacao, transportador);

                    try
                    {
                        _unitOfWork.Start();

                        fechamentoPontuacaoTransportador = new Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador()
                        {
                            FechamentoPontuacao = fechamentoPontuacao,
                            Pontuacao = listaRegraPontuacao.Sum(o => o.PontuacaoConvertida) + transportador.PontuacaoFixa,
                            Transportador = transportador
                        };

                        repositorioFechamentoPontuacaoTransportador.Inserir(fechamentoPontuacaoTransportador);

                        foreach (Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase regraPontuacao in listaRegraPontuacao)
                        {
                            Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra regra = new Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra()
                            {
                                Descricao = regraPontuacao.Descricao,
                                FechamentoPontuacaoTransportador = fechamentoPontuacaoTransportador,
                                Pontuacao = regraPontuacao.PontuacaoConvertida
                            };

                            repositorioFechamentoPontuacaoTransportadorRegra.Inserir(regra);
                        }

                        if (transportador.PontuacaoFixa > 0)
                        {
                            Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra regra = new Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra()
                            {
                                Descricao = "Pontuação Fixa",
                                FechamentoPontuacaoTransportador = fechamentoPontuacaoTransportador,
                                Pontuacao = transportador.PontuacaoFixa
                            };

                            repositorioFechamentoPontuacaoTransportadorRegra.Inserir(regra);
                        }

                        if (fechamentoMesAnterior)
                        {
                            transportador.Pontuacao = fechamentoPontuacaoTransportador.Pontuacao;

                            repositorioTransportador.Atualizar(transportador);
                        }

                        _unitOfWork.CommitChanges();
                    }
                    catch (Exception)
                    {
                        _unitOfWork.Rollback();
                        throw;
                    }
                }
            }
        }

        private void CarregarListasRegrasPontuacao()
        {
            Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador repositorioAdvertenciaTransportador = new Repositorio.Embarcador.Frete.Pontuacao.AdvertenciaTransportador(_unitOfWork);
            Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria repositorioPontuacaoPorModeloCarroceria = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria(_unitOfWork);
            Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao repositorioPontuacaoPorPessoaClassificacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao(_unitOfWork);
            Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga repositorioPontuacaoPorQuantidadeCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga(_unitOfWork);
            Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao repositorioPontuacaoPorQuantidadeCargaGanhaCotacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao(_unitOfWork);
            Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade repositorioPontuacaoPorTempoAtividade = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade(_unitOfWork);
            Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao repositorioPontuacaoPorTipoOperacao = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga repositorioPontuacaoPorTipoCarga = new Repositorio.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga(_unitOfWork);
            Repositorio.Empresa repositorioTransportador = new Repositorio.Empresa(_unitOfWork);

            _listaAdvertencia = repositorioAdvertenciaTransportador.BuscarTodos();
            _listaPontuacaoPorModeloCarroceria = repositorioPontuacaoPorModeloCarroceria.BuscarTodos();
            _listaPontuacaoPorPessoaClassificacao = repositorioPontuacaoPorPessoaClassificacao.BuscarTodos();
            _listaPontuacaoPorQuantidadeCarga = repositorioPontuacaoPorQuantidadeCarga.BuscarTodos();
            _listaPontuacaoPorQuantidadeCargaGanhaCotacao = repositorioPontuacaoPorQuantidadeCargaGanhaCotacao.BuscarTodos();
            _listaPontuacaoPorTempoAtividade = repositorioPontuacaoPorTempoAtividade.BuscarTodos();
            _listaPontuacaoPorTipoOperacao = repositorioPontuacaoPorTipoOperacao.BuscarTodos();
            _listaPontuacaoPorTipoCarga = repositorioPontuacaoPorTipoCarga.BuscarTodos();
            _listaPontuacaoFixa = repositorioTransportador.BuscarPontuacaoMaiorQueZero();
        }

        private void DefinirRegrasPontuacaoPorAdvertencia(List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> listaRegraPontuacao, Dominio.Entidades.Empresa transportador, DateTime dataInicialFechamento, DateTime dataFinalFechamento)
        {
            List<Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador> advertencias = (
                from o in _listaAdvertencia
                where o.Transportador.Codigo == transportador.Codigo && o.Data >= dataInicialFechamento && o.Data.Date <= dataFinalFechamento
                select o
            ).ToList();

            if (advertencias.Count > 0)
                listaRegraPontuacao.AddRange(advertencias);
        }

        private void DefinirRegrasPontuacaoPorModeloCarroceria(List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> listaRegraPontuacao, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas)
        {
            HashSet<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria> modelosCarroceria = new HashSet<Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria>();

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                if (carga.Veiculo?.ModeloCarroceria != null)
                    modelosCarroceria.Add(carga.Veiculo.ModeloCarroceria);

                if (carga.VeiculosVinculados?.Count() > 0)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
                    {
                        if (reboque?.ModeloCarroceria != null)
                            modelosCarroceria.Add(reboque.ModeloCarroceria);
                    }
                }
            }

            foreach (Dominio.Entidades.Embarcador.Veiculos.ModeloCarroceria modeloCarroceria in modelosCarroceria)
            {
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorModeloCarroceria pontuacaoPorModeloCarroceria = (
                    from o in _listaPontuacaoPorModeloCarroceria
                    where o.ModeloCarroceria.Codigo == modeloCarroceria.Codigo
                    select o
                ).FirstOrDefault();

                if (pontuacaoPorModeloCarroceria != null)
                    listaRegraPontuacao.Add(pontuacaoPorModeloCarroceria);
            }
        }

        private void DefinirRegrasPontuacaoPorPessoaClassificacao(List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> listaRegraPontuacao, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas)
        {
            foreach (Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao pontuacaoPorPessoaClassificacao in _listaPontuacaoPorPessoaClassificacao)
            {
                int totalCargas = (from o in cargas where o.DadosSumarizados.ClientesDestinatarios.Any(c => c.GrupoPessoas?.Classificacao?.Codigo == pontuacaoPorPessoaClassificacao.PessoaClassificacao.Codigo) select o)?.Count() ?? 0;

                if (totalCargas > 0)
                {
                    listaRegraPontuacao.Add(new Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao()
                    {
                        PessoaClassificacao = pontuacaoPorPessoaClassificacao.PessoaClassificacao,
                        Pontuacao = pontuacaoPorPessoaClassificacao.Pontuacao * totalCargas
                    });
                }
            }
        }

        private void DefinirRegrasPontuacaoPorQuantidadeCargaGanhasCotacao(List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> listaRegraPontuacao, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas)
        {
            List<int> codigosCargas = cargas.Select(o => o.Codigo).Distinct().ToList();
            Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Logistica.CargaJanelaCarregamentoConsulta(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargas);
            int totalCargas = listaCargaJanelaCarregamento.Where(o => o.CargaCotacaoGanhaAutomaticamente).Count();

            Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCargaGanhaCotacao pontuacaoPorQuantidadeCargaCargaGanhaCotacao = (
                from o in _listaPontuacaoPorQuantidadeCargaGanhaCotacao
                where o.QuantidadeInicio <= totalCargas && o.QuantidadeFim >= totalCargas
                select o
            ).FirstOrDefault();

            if (pontuacaoPorQuantidadeCargaCargaGanhaCotacao != null)
                listaRegraPontuacao.Add(pontuacaoPorQuantidadeCargaCargaGanhaCotacao);
        }

        private void DefinirRegrasPontuacaoPorQuantidadeCarga(List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> listaRegraPontuacao, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas)
        {
            int totalCargas = cargas.Count();

            Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga pontuacaoPorQuantidadeCarga = (
                from o in _listaPontuacaoPorQuantidadeCarga
                where o.QuantidadeInicio <= totalCargas && o.QuantidadeFim >= totalCargas
                select o
            ).FirstOrDefault();

            if (pontuacaoPorQuantidadeCarga != null)
                listaRegraPontuacao.Add(pontuacaoPorQuantidadeCarga);
        }

        private void DefinirRegrasPontuacaoPorTempoAtividade(List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> listaRegraPontuacao, Dominio.Entidades.Empresa transportador)
        {
            int? tempoAtividade = transportador.TempoAtividadeEmAnos;

            if (tempoAtividade.HasValue)
            {
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade pontuacaoPorTempoAtividade = (
                    from o in _listaPontuacaoPorTempoAtividade
                    where o.AnoInicio <= tempoAtividade.Value && o.AnoFim >= tempoAtividade.Value
                    select o
                ).FirstOrDefault();

                if (pontuacaoPorTempoAtividade != null)
                    listaRegraPontuacao.Add(pontuacaoPorTempoAtividade);
            }
        }

        private void DefinirRegrasPontuacaoPorTipoOperacao(List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> listaRegraPontuacao, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas)
        {
            IEnumerable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = (from o in cargas where o.TipoOperacao != null select o.TipoOperacao).ToList().Distinct();

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tiposOperacao)
            {
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao pontuacaoPorTipoOperacao = (
                    from o in _listaPontuacaoPorTipoOperacao
                    where o.TipoOperacao.Codigo == tipoOperacao.Codigo
                    select o
                ).FirstOrDefault();

                if (pontuacaoPorTipoOperacao != null)
                    listaRegraPontuacao.Add(pontuacaoPorTipoOperacao);
            }
        }

        private void DefinirRegrasPontuacaoPorTipoCarga(List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> listaRegraPontuacao, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas)
        {
            IEnumerable<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tipoDeCargas = (from o in cargas where o.TipoDeCarga != null select o.TipoDeCarga).ToList().Distinct();

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga in tipoDeCargas)
            {
                Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga pontuacaoPorTipoCarga = (
                    from o in _listaPontuacaoPorTipoCarga
                    where o.TipoCarga.Codigo == tipoDeCarga.Codigo
                    select o
                ).FirstOrDefault();

                if (pontuacaoPorTipoCarga != null)
                    listaRegraPontuacao.Add(pontuacaoPorTipoCarga);
            }
        }

        private bool IsExisteRegrasPontuacao()
        {
            return (
                (_listaAdvertencia.Count() > 0) ||
                (_listaPontuacaoPorModeloCarroceria.Count() > 0) ||
                (_listaPontuacaoPorPessoaClassificacao.Count() > 0) ||
                (_listaPontuacaoPorQuantidadeCarga.Count() > 0) ||
                (_listaPontuacaoPorTempoAtividade.Count() > 0) ||
                (_listaPontuacaoPorTipoOperacao.Count() > 0) ||
                (_listaPontuacaoFixa.Count() > 0)
            );
        }

        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> ObterRegrasPontuacao(Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao fechamentoPontuacao, Dominio.Entidades.Empresa transportador)
        {
            DateTime dataInicialFechamento = new DateTime(fechamentoPontuacao.Ano, (int)fechamentoPontuacao.Mes, 1);
            DateTime dataFinalFechamento = dataInicialFechamento.AddMonths(1).AddDays(-1);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargasFaturadasPorPeriodoETransportador(dataInicialFechamento, dataFinalFechamento, transportador.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase> listaRegraPontuacao = new List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoBase>();

            DefinirRegrasPontuacaoPorTempoAtividade(listaRegraPontuacao, transportador);
            DefinirRegrasPontuacaoPorQuantidadeCarga(listaRegraPontuacao, cargas);
            DefinirRegrasPontuacaoPorQuantidadeCargaGanhasCotacao(listaRegraPontuacao, cargas);
            DefinirRegrasPontuacaoPorTipoOperacao(listaRegraPontuacao, cargas);
            DefinirRegrasPontuacaoPorTipoCarga(listaRegraPontuacao, cargas);
            DefinirRegrasPontuacaoPorModeloCarroceria(listaRegraPontuacao, cargas);
            DefinirRegrasPontuacaoPorPessoaClassificacao(listaRegraPontuacao, cargas);
            DefinirRegrasPontuacaoPorAdvertencia(listaRegraPontuacao, transportador, dataInicialFechamento, dataFinalFechamento);

            return listaRegraPontuacao;
        }

        #endregion

        #region Métodos Públicos

        public void Finalizar()
        {
            Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacao repositorioFechamentoPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao> listaFechamentoPontuacao = repositorioFechamentoPontuacao.BuscarPorAguardandoFinalizacao(limiteRegistros: 1);

            if (listaFechamentoPontuacao.Count == 0)
                return;

            CarregarListasRegrasPontuacao();

            if (!IsExisteRegrasPontuacao())
                return;

            foreach (Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao fechamentoPontuacao in listaFechamentoPontuacao)
            {
                AdicionarPontuacaoTransportadores(fechamentoPontuacao);

                fechamentoPontuacao.Situacao = SituacaoFechamentoPontuacao.Finalizado;

                repositorioFechamentoPontuacao.Atualizar(fechamentoPontuacao);
            }
        }

        #endregion
    }
}
