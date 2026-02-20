using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoTransportador
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoTransportador(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public CargaJanelaCarregamentoTransportador(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void DefinirDataDisponibilizacaoTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if (cargaJanelaCarregamento.DataDisponibilizacaoTransportadores.HasValue)
                return;

            cargaJanelaCarregamento.DataDisponibilizacaoTransportadores = DateTime.Now;

            new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork).Atualizar(cargaJanelaCarregamento);
        }

        private void DefinirDataLiberacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            if ((cargaJanelaCarregamento.ConfiguracaoRotaFrete == null) || !cargaJanelaCarregamento.ConfiguracaoRotaFrete.HoraEnvioTransportadorRota.HasValue || !cargaJanelaCarregamento.ConfiguracaoRotaFrete.EnviarTransportadorRotaConfigurado())
            {
                cargaJanelaCarregamento.DataLiberacaoShare = null;
                return;
            }

            DateTime dataLiberacao = cargaJanelaCarregamento.InicioCarregamento.Date.AddSeconds(cargaJanelaCarregamento.ConfiguracaoRotaFrete.HoraEnvioTransportadorRota.Value.TotalSeconds);
            int diasAntecedenciaDescontar = cargaJanelaCarregamento.ConfiguracaoRotaFrete.DiasAntecedenciaEnvioTransportadorRota;

            while (diasAntecedenciaDescontar > 0)
            {
                dataLiberacao = dataLiberacao.AddDays(-1);

                if (cargaJanelaCarregamento.ConfiguracaoRotaFrete.EnviarTransportadorRota(dataLiberacao))
                    diasAntecedenciaDescontar--;
            }

            cargaJanelaCarregamento.DataLiberacaoShare = dataLiberacao;
        }

        private void DefinirDataLiberacaoInicial(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete)
        {
            if (cargaJanelaCarregamento.ConfiguracaoRotaFrete?.Codigo == configuracaoRotaFrete?.Codigo)
                return;

            cargaJanelaCarregamento.ConfiguracaoRotaFrete = configuracaoRotaFrete;

            DefinirDataLiberacao(cargaJanelaCarregamento);
        }

        private int ObterCodigoModeloVeicularCargaParaDisponibilizarParaTransportadores(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();

            return configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga ? carga.ModeloVeicularCarga?.Codigo ?? 0 : 0;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        private Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete ObterConfiguracaoRotaFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Localidade localidadeOrigem = repositorioCarga.ObterLocalidadeOrigem(carga.Codigo);

            if (localidadeOrigem == null)
                return null;

            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);
            List<Dominio.ObjetosDeValor.Localidade> localidadesDestino = servicoCargaDadosSumarizados.ObterDestinos(carga, _unitOfWork, tipoServicoMultisoftware);
            List<int> listaCodigoLocalidadeDestino = (from o in localidadesDestino select o.Codigo).Distinct().ToList();
            List<string> listaUfDestino = (from o in localidadesDestino where !string.IsNullOrWhiteSpace(o.SiglaUF) select o.SiglaUF).Distinct().ToList();

            if (listaUfDestino.Count == 0)
            {
                if (!(carga.DadosSumarizados?.Destinos?.EndsWith(" - EX") ?? false))
                    return null;

                listaUfDestino.Add("EX");
            }

            Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete repositorioConfiguracaoRotaFrete = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete = repositorioConfiguracaoRotaFrete.BuscarPrimeiraDisponivel(localidadeOrigem.Codigo, carga.Filial?.Codigo ?? 0, listaUfDestino, listaCodigoLocalidadeDestino, carga.TipoDeCarga?.Codigo ?? 0, carga.ModeloVeicularCarga?.Codigo ?? 0);

            return configuracaoRotaFrete;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> ObterConfiguracaoRotaFreteEmpresas(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete = ObterConfiguracaoRotaFrete(carga, tipoServicoMultisoftware);

            if (configuracaoRotaFrete == null)
                return new List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa>();

            int codigoModeloVeicularCarga = ObterCodigoModeloVeicularCargaParaDisponibilizarParaTransportadores(carga);
            Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa repositorioConfiguracaoRotaFreteEmpresa = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> configuracaoRotaFreteEmpresas = repositorioConfiguracaoRotaFreteEmpresa.BuscarPorConfiguracaoRotaFrete(configuracaoRotaFrete.Codigo, codigoModeloVeicularCarga);

            return configuracaoRotaFreteEmpresas;
        }

        private DateTime ObterHorarioLiberacao(List<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao> listaTempoEsperaPorPontuacao, Dominio.Entidades.Empresa transportador)
        {
            int tempoEspera = (from o in listaTempoEsperaPorPontuacao where o.PontuacaoInicial <= transportador.Pontuacao && o.PontuacaoFinal >= transportador.Pontuacao select o.TempoEsperaEmMinutos).FirstOrDefault();

            return DateTime.Now.AddMinutes(tempoEspera);
        }

        private List<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao> ObterListaTempoEsperaPorPontuacao()
        {
            Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao repositorioTempoEsperaPorPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao(_unitOfWork);

            return repositorioTempoEsperaPorPontuacao.BuscarTodos();
        }

        private (Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa OfertaEscolhida, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> Ofertas) ObterRotaFreteEmpresaAdicionarCargaJanelaCarregamentoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.RotaFreteEmpresa> rotaFreteEmpresas, List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> configuracaoRotaFreteEmpresas, List<int> codigosTransportadoresRejeitaram)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamentoOfertaCarga repositorioCentroCarregamentoOfertaCarga = new Repositorio.Embarcador.Logistica.CentroCarregamentoOfertaCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga centroCarregamentoOfertaCarga = cargaJanelaCarregamento.CentroCarregamento?.Codigo > 0 ? repositorioCentroCarregamentoOfertaCarga.BuscarPorCentroCarregamento(cargaJanelaCarregamento.CentroCarregamento.Codigo).FirstOrDefault() : null;
            DateTime dataInicial;
            DateTime dataFinal;

            if (centroCarregamentoOfertaCarga?.PeriodoDiferenciadoShare ?? false)
            {
                dataInicial = centroCarregamentoOfertaCarga.DataInicialPeriodoDiferenciadoShare.Value;
                dataFinal = centroCarregamentoOfertaCarga.DataFinalPeriodoDiferenciadoShare.Value;
            }
            else
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
                DateTime dataBase = configuracaoEmbarcador.UtilizarDataCarregamentoDaJanelaCarregamentoAoSetarTransportadorPrioritarioPorRotaCarga ? cargaJanelaCarregamento.InicioCarregamento : cargaJanelaCarregamento.Carga.DataCriacaoCarga;
                dataInicial = dataBase.FirstDayOfMonth();
                dataFinal = dataBase.LastDayOfMonth();
            }

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> rotaFreteEmpresaOfertas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta>();
            Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa ofertaEscolhidaPorRota = ObterRotaFreteEmpresaAdicionarCargaJanelaCarregamentoTransportador(cargaJanelaCarregamento, rotaFreteEmpresas, rotaFreteEmpresaOfertas, codigosTransportadoresRejeitaram, dataInicial, dataFinal);

            if (ofertaEscolhidaPorRota != null)
                return ValueTuple.Create(ofertaEscolhidaPorRota, rotaFreteEmpresaOfertas);

            Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa ofertaEscolhidaPorConfiguracaoRota = ObterRotaFreteEmpresaAdicionarCargaJanelaCarregamentoTransportador(cargaJanelaCarregamento, configuracaoRotaFreteEmpresas, rotaFreteEmpresaOfertas, codigosTransportadoresRejeitaram, dataInicial, dataFinal);

            return ValueTuple.Create(ofertaEscolhidaPorConfiguracaoRota, rotaFreteEmpresaOfertas);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa ObterRotaFreteEmpresaAdicionarCargaJanelaCarregamentoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.RotaFreteEmpresa> rotaFreteEmpresas, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> rotaFreteEmpresaOfertas, List<int> codigosTransportadoresRejeitaram, DateTime dataInicio, DateTime dataFinal)
        {
            if (rotaFreteEmpresas.Count == 0)
                return null;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            int codigoModeloVeicularCarga = ObterCodigoModeloVeicularCargaParaDisponibilizarParaTransportadores(cargaJanelaCarregamento.Carga);
            int quantidadeCargasPorPeriodo = repositorioCarga.ContarQuantidadeCargaPorRota(cargaJanelaCarregamento.Carga.Rota.Codigo, cargaJanelaCarregamento.Carga.Codigo, dataInicio, dataFinal, codigoModeloVeicularCarga);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa> listaRotaFreteEmpresa = rotaFreteEmpresas.Select(o => new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa()
            {
                Descricao = o.Descricao,
                Empresa = o.Empresa,
                PercentualCargasDaRota = o.PercentualCargasDaRota,
                Prioridade = o.Prioridade,
                RotaFrete = o.RotaFrete
            }).ToList();

            return ObterRotaFreteEmpresaAdicionarCargaJanelaCarregamentoTransportador(cargaJanelaCarregamento, listaRotaFreteEmpresa, rotaFreteEmpresaOfertas, codigosTransportadoresRejeitaram, quantidadeCargasPorPeriodo, dataInicio, dataFinal);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa ObterRotaFreteEmpresaAdicionarCargaJanelaCarregamentoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> configuracaoRotaFreteEmpresas, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> rotaFreteEmpresaOfertas, List<int> codigosTransportadoresRejeitaram, DateTime dataInicio, DateTime dataFim)
        {
            if (configuracaoRotaFreteEmpresas.Count == 0)
                return null;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            int codigoConfiguracaoRotaFrete = configuracaoRotaFreteEmpresas.FirstOrDefault().ConfiguracaoRotaFrete.Codigo;
            int codigoModeloVeicularCarga = ObterCodigoModeloVeicularCargaParaDisponibilizarParaTransportadores(cargaJanelaCarregamento.Carga);
            int quantidadeCargasPorPeriodo = repositorioCarga.ContarQuantidadeCargaPorConfiguracaoRota(codigoConfiguracaoRotaFrete, cargaJanelaCarregamento.Carga.Codigo, dataInicio, dataFim, codigoModeloVeicularCarga);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa> listaRotaFreteEmpresa = configuracaoRotaFreteEmpresas.Select(o => new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa()
            {
                ConfiguracaoRotaFrete = o.ConfiguracaoRotaFrete,
                Descricao = o.Descricao,
                Empresa = o.Empresa,
                PercentualCargasDaRota = o.PercentualCargasDaRota,
                Prioridade = o.Prioridade
            }).ToList();

            return ObterRotaFreteEmpresaAdicionarCargaJanelaCarregamentoTransportador(cargaJanelaCarregamento, listaRotaFreteEmpresa, rotaFreteEmpresaOfertas, codigosTransportadoresRejeitaram, quantidadeCargasPorPeriodo, dataInicio, dataFim);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa ObterRotaFreteEmpresaAdicionarCargaJanelaCarregamentoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa> rotaFreteEmpresas, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> rotaFreteEmpresaOfertas, List<int> codigosTransportadoresRejeitaram, int quantidadeCargasPorPeriodo, DateTime dataInicial, DateTime dataFinal)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa> rotaFreteEmpresasPercentualAtingido = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa>();
            Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa rotaFreteEmpresaEscolhida = null;
            List<(Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa RotaFreteEmpresa, decimal PercentualCargas, int Prioridade, bool Rejeitada)> ofertas = new List<(Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa RotaFreteEmpresa, decimal PercentualCargas, int Prioridade, bool Rejeitada)>();
            int codigoModeloVeicularCarga = ObterCodigoModeloVeicularCargaParaDisponibilizarParaTransportadores(cargaJanelaCarregamento.Carga);

            if (configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorPrioridade)
                rotaFreteEmpresas = rotaFreteEmpresas.OrderBy(o => o.Prioridade).ToList();
            else
                rotaFreteEmpresas = rotaFreteEmpresas.OrderByDescending(o => o.PercentualCargasDaRota).ToList();

            for (int i = 0, s = rotaFreteEmpresas.Count; i < s; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa rotaFreteEmpresa = rotaFreteEmpresas[i];
                Dominio.Entidades.Empresa filialDaMatriz = ObterTransportadorPorFilialDaMatriz(cargaJanelaCarregamento, rotaFreteEmpresa.Empresa);

                if (filialDaMatriz != null)
                    rotaFreteEmpresa.Empresa = filialDaMatriz;

                bool transportadorRejeitouCarga = codigosTransportadoresRejeitaram.Any(obj => obj == rotaFreteEmpresa.Empresa.Codigo);
                decimal percentual = 0m;

                if (quantidadeCargasPorPeriodo > 0)
                {
                    int quantidadeCargasPorPeriodoEEmpresa = 0;

                    if (rotaFreteEmpresa.ConfiguracaoRotaFrete != null)
                        quantidadeCargasPorPeriodoEEmpresa = repositorioCarga.ContarQuantidadeCargaPorConfiguracaoRota(rotaFreteEmpresa.ConfiguracaoRotaFrete.Codigo, cargaJanelaCarregamento.Carga.Codigo, dataInicial, dataFinal, codigoModeloVeicularCarga, rotaFreteEmpresa.Empresa.Codigo);
                    else
                        quantidadeCargasPorPeriodoEEmpresa = repositorioCarga.ContarQuantidadeCargaPorRota(rotaFreteEmpresa.RotaFrete.Codigo, cargaJanelaCarregamento.Carga.Codigo, dataInicial, dataFinal, codigoModeloVeicularCarga, rotaFreteEmpresa.Empresa.Codigo);

                    decimal resultado = (decimal)(quantidadeCargasPorPeriodoEEmpresa * 100) / quantidadeCargasPorPeriodo;

                    percentual = Math.Round(resultado, 2, MidpointRounding.AwayFromZero);
                }

                ofertas.Add(ValueTuple.Create(rotaFreteEmpresa, percentual, rotaFreteEmpresa.Prioridade, transportadorRejeitouCarga));

                if (transportadorRejeitouCarga || (rotaFreteEmpresaEscolhida != null))
                    continue;

                if (configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorPrioridade || (percentual <= rotaFreteEmpresa.PercentualCargasDaRota))
                    rotaFreteEmpresaEscolhida = rotaFreteEmpresa;
                else
                    rotaFreteEmpresasPercentualAtingido.Add(rotaFreteEmpresa);
            }

            if (rotaFreteEmpresaEscolhida == null)
                rotaFreteEmpresaEscolhida = rotaFreteEmpresasPercentualAtingido.FirstOrDefault();

            int totalOfertas = rotaFreteEmpresaOfertas.Count;

            foreach ((Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa RotaFreteEmpresa, decimal PercentualCargas, int Prioridade, bool Rejeitada) oferta in ofertas)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta rotaFreteEmpresaOferta = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta()
                {
                    Descricao = oferta.RotaFreteEmpresa.Descricao,
                    Empresa = oferta.RotaFreteEmpresa.Empresa,
                    Ordem = ++totalOfertas,
                    PercentualCargas = oferta.PercentualCargas,
                    PercentualConfigurado = oferta.RotaFreteEmpresa.PercentualCargasDaRota,
                    Prioridade = oferta.Prioridade,
                    Tipo = TipoHistoricoOfertaTransportador.Registrada
                };

                if ((rotaFreteEmpresaEscolhida != null) && (rotaFreteEmpresaEscolhida.Empresa.Codigo == oferta.RotaFreteEmpresa.Empresa.Codigo))
                    rotaFreteEmpresaOferta.Tipo = TipoHistoricoOfertaTransportador.Escolhida;
                else if (oferta.Rejeitada)
                    rotaFreteEmpresaOferta.Tipo = TipoHistoricoOfertaTransportador.Rejeitada;

                rotaFreteEmpresaOfertas.Add(rotaFreteEmpresaOferta);
            }

            return rotaFreteEmpresaEscolhida;
        }

        private List<Dominio.Entidades.RotaFreteEmpresa> ObterRotaFreteEmpresas(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            int codigoModeloVeicularCarga = ObterCodigoModeloVeicularCargaParaDisponibilizarParaTransportadores(carga);
            Repositorio.RotaFreteEmpresa repositorioRotaFreteEmpresa = new Repositorio.RotaFreteEmpresa(_unitOfWork);
            List<Dominio.Entidades.RotaFreteEmpresa> rotaFreteEmpresas = repositorioRotaFreteEmpresa.BuscarPorRotaFrete(carga.Rota.Codigo, codigoModeloVeicularCarga);

            return rotaFreteEmpresas;
        }

        private Dominio.Entidades.Empresa ObterTransportadorPorFilialDaMatriz(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Empresa transportador)
        {
            if (!(cargaJanelaCarregamento.CentroCarregamento?.PermitirMatrizSelecionarFilial ?? false))
                return null;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Localidade origemCarga = repositorioCarga.ObterLocalidadeOrigem(cargaJanelaCarregamento.Carga.Codigo);

            if (origemCarga == null)
                return null;

            Dominio.Entidades.Empresa filialDoTransportador = transportador.Filiais.Where(o => o.Localidade == origemCarga).FirstOrDefault();

            return filialDoTransportador;
        }

        private void RejeitarJanelaCarregamentoTransportadorPorTempoConfirmacaoEncerrado(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);

            cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga = null;
            cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Rejeitada;

            carga.DataAtualizacaoCarga = DateTime.Now;
            carga.Empresa = null;
            carga.RejeitadaPeloTransportador = true;

            repositorioCarga.Atualizar(carga);
            repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
            SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga rejeitada para o transportador por tempo de confirmação encerrado");
        }

        private void SalvarHistoricoOfertaPorRota(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> ofertas)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico repositorioHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta repositorioHistoricoOferta = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico historico = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico()
            {
                CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador,
                Data = DateTime.Now,
                Descricao = "Carga disponibilizada para o transportador por rota",
                Tipo = TipoCargaJanelaCarregamentoTransportadorHistorico.OfertaCargaPorRota
            };

            repositorioHistorico.Inserir(historico);

            for (int i = 0; i < ofertas.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta oferta = ofertas[i];
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta historicoOferta = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta()
                {
                    CargaJanelaCarregamentoTransportadorHistorico = historico,
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

        private void ValidarCargasInteressadas(Dominio.Entidades.Empresa transportador, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos, int numeroPallet)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Servicos.Embarcador.Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Servicos.Embarcador.Hubs.JanelaCarregamento();
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaInteresse = repositorioCargaJanelaCarregamentoTransportador.BuscarCargaComInteressePorCarga(transportador.Codigo, numeroPallet, modelosVeicularesPermitidos);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaInteresse)
            {
                try
                {
                    ValidarPermissaoMarcarInteresseCarga(transportador, cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga);
                }
                catch (ServicoException)
                {
                    cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Disponivel;
                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                    SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Removido o interesse na carga");
                    servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento);
                }
            }
        }

        private List<int> BuscarCodigosTransportadoresPorGrupoRegional(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Localidade origemCarga = repCarga.ObterLocalidadeOrigem(cargaJanelaCarregamento.Carga.Codigo);
            string[] ufsEspecificas = new string[] { "MG", "PE", "GO" };
            string ufEspecifica = null;

            if (origemCarga?.Estado != null && ufsEspecificas.Contains(origemCarga?.Estado.Sigla))
                ufEspecifica = origemCarga.Estado.Sigla;

            return repEmpresa.BuscarCodigosAptosParaEmissaoPorEstado(ufEspecifica);
        }

        private void RejeitarCarga(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Embarcador.Cargas.Carga carga, string motivoRejeicaoCarga, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCarga(carga.Codigo);

            cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga = null;
            cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Rejeitada;
            cargaJanelaCarregamentoTransportador.NumeroRejeicoesManuais += 1;

            if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento?.ExigirTransportadorInformarMotivoAoRejeitarCarga ?? false)
            {
                cargaJanelaCarregamentoTransportador.MotivoRejeicaoCarga = motivoRejeicaoCarga;
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CargaBase, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRejeitouCargaPeloMotivo, cargaJanelaCarregamentoTransportador.Transportador.NomeFantasia, cargaJanelaCarregamentoTransportador.MotivoRejeicaoCarga)), _unitOfWork);
            }

            carga.DataAtualizacaoCarga = DateTime.Now;
            carga.Empresa = null;
            carga.RejeitadaPeloTransportador = true;

            if (agendamento != null && agendamento.Transportador != null)
            {
                string nomeTransportador = $"({agendamento.Transportador.CNPJ_Formatado}) {agendamento.Transportador.Descricao}";
                agendamento.Transportador = null;
                agendamento.EtapaAgendamentoColeta = (configuracaoAgendamentoColeta.RemoverEtapaAgendamentoAgendamentoColeta || (carga.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.RemoverEtapaAgendamentoDoAgendamentoColeta ?? false)) ? EtapaAgendamentoColeta.NFe : EtapaAgendamentoColeta.DadosTransporte;
                repAgendamentoColeta.Atualizar(agendamento);

                if (agendamento.Remetente != null && configuracaoEmbarcador.NotificarCargaAgConfirmacaoTransportador)
                    NotificarCargaRejeitadaPorTransportador(agendamento, nomeTransportador);

                Servicos.Auditoria.Auditoria.Auditar(auditado, agendamento, null, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRejeitouaCarga, nomeTransportador)), _unitOfWork);
            }

            repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
            SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRejeitouCarga, usuario);
            repositorioCarga.Atualizar(carga);
        }

        private void NotificarCargaRejeitadaPorTransportador(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento, string nomeTransportador)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);

            Dominio.Entidades.Cliente rementente = agendamento.Remetente;

            List<string> emails = (rementente.Email ?? string.Empty).Split(';').Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
#if DEBUG
            emails = new List<string>() { "" };
#endif
            if (emails.Count == 0)
                return;

            string assunto = Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaRejeitada;
            StringBuilder stBuilder = new StringBuilder();

            stBuilder.Append("Olá " + rementente.Descricao + ",")
                 .AppendLine()
                 .AppendLine()
                 .Append("A carga ")
                 .Append(agendamento.Carga.CodigoCargaEmbarcador)
                 .Append(", com coleta prevista para o dia ")
                 .Append(agendamento.DataColeta.Value.ToDateTimeString())
                 .Append(", foi rejeitada pelo transportador")
                 .Append(nomeTransportador)
                 .Append(".")
                 .AppendLine()
                 .AppendLine();

            Task.Factory.StartNew(() => EnviarEmailAsync(emails, rementente.Nome, assunto, stBuilder.ToString(), _unitOfWork.StringConexao));
        }

        private void EnviarEmailAsync(List<string> emails, string nome, string assunto, string corpo, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            string erro;

            try
            {
                Servicos.Email.EnviarEmailAutenticado(string.Join(";", emails), assunto, corpo, unitOfWork, out erro, nome);
            }
            catch (Exception exception)
            {
                erro = exception.ToString();
            }
            finally
            {
                unitOfWork.Dispose();
            }

            if (!string.IsNullOrWhiteSpace(erro))
                Servicos.Log.TratarErro(erro, Localization.Resources.Logistica.JanelaCarregamentoTransportador.JanelaDeCarregamentoDoTransportador);
        }

        private void NotificarUsuarios(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, List<Dominio.Entidades.Usuario> usuariosNotificacao, string mensagemNotificacao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, UnitOfWork unitOfWork, string adminStringConexao)
        {
            if (usuariosNotificacao.Count <= 0)
                return;

            if ((cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento?.NotificarSomenteAlteracaoCotacao ?? false) && !cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CargaLiberadaCotacao)
                return;

            Servicos.Embarcador.Notificacao.Notificacao servicoNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, cliente, tipoServicoMultisoftware, adminStringConexao);
            int codigoJanelaCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo;
            List<Dominio.Entidades.Usuario> usuarios = usuariosNotificacao.Distinct().ToList();

            foreach (Dominio.Entidades.Usuario usuario in usuarios)
                servicoNotificacao.GerarNotificacao(usuario, codigoJanelaCarregamento, Localization.Resources.Logistica.JanelaCarregamentoTransportador.LogisticaJanelaCarregamento, mensagemNotificacao, IconesNotificacao.janelaMarcouInteresse, TipoNotificacao.janelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork);
        }

        private void NotificarEmails(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, List<string> emailsNotificacao, string mensagemNotificacao, UnitOfWork unitOfWork)
        {
            if (emailsNotificacao.Count <= 0)
                return;

            string transportador = cargaJanelaCarregamentoTransportador.Transportador.Descricao;
            List<string> emails = emailsNotificacao.Distinct().ToList();

            foreach (string email in emails)
            {
                if (!Servicos.Email.EnviarEmailAutenticado(email, string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AtualizacaoJanelaDeCarregamentoTransportador, transportador), mensagemNotificacao, unitOfWork, out string msg, ""))
                    Servicos.Log.TratarErro(string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.FalhaAoEnviarEmailDeAtualizacaoJanelaDeCarregamentoTransportadorPara, email, msg));
            }
        }

        private void InformarCargaAtualizadaEmbarcador(int codigoCarga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, string adminStringConexao)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(adminStringConexao);
            try
            {
                AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repConfig = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
                AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);

                if (cliente == null)
                    return;

                AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repConfig.BuscarPorClienteETipo(cliente.Codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                if (clienteURLAcesso == null)
                    return;

                string urlJanelaCarregamento = "http://" + clienteURLAcesso.URLAcesso;

                if (clienteURLAcesso.URLAcesso.Contains("192.168.0.125"))
                    urlJanelaCarregamento += "/Embarcador";

                urlJanelaCarregamento += "/JanelaCarregamento/InformarCargaAtualizada?Carga=" + codigoCarga;

                WebRequest wRequest = WebRequest.Create(urlJanelaCarregamento);
                wRequest.Timeout = 8000;
                wRequest.Method = "GET";

                WebResponse response = wRequest.GetResponse();

                response.Close();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void AceitarTermoDeAceite(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, Dominio.Entidades.Usuario usuario, string termoAceite, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite repositorioTransportadorTermoAceite = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite(_unitOfWork);

            if (repositorioTransportadorTermoAceite.ExistePorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo))
                throw new ServicoException("Você já aceitou esse termo.");

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite transportadorTermoAceite = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite()
            {
                CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador,
                Data = DateTime.Now,
                UsuarioResponsavel = usuario,
                TermoAceite = termoAceite
            };

            repositorioTransportadorTermoAceite.Inserir(transportadorTermoAceite);

            Auditoria.Auditoria.Auditar(auditoria, transportadorTermoAceite, "Transportador aceitou o termo de aceite", _unitOfWork);
        }

        public void AceitarTermoDeAceitePorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario usuario, string termoAceite, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite repositorioTransportadorTermoAceite = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite transportadorTermoAceite = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorTermoAceite()
            {
                Carga = carga,
                Data = DateTime.Now,
                UsuarioResponsavel = usuario,
                TermoAceite = termoAceite
            };

            repositorioTransportadorTermoAceite.Inserir(transportadorTermoAceite);

            Auditoria.Auditoria.Auditar(auditoria, transportadorTermoAceite, $"Termo de aceite aceito na tela de cargas pelo transportador {carga.Empresa.NomeFantasia}", _unitOfWork);
        }

        public void AtualizarDataLiberacao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((cargaJanelaCarregamento?.CentroCarregamento?.TipoTransportador != TipoTransportadorCentroCarregamento.PorPrioridadeDeRota) || cargaJanelaCarregamento.ShareLiberado)
                return;

            DateTime? dataLiberacaoAnterior = cargaJanelaCarregamento.DataLiberacaoShare;

            DefinirDataLiberacao(cargaJanelaCarregamento);

            if (cargaJanelaCarregamento.DataLiberacaoShare != dataLiberacaoAnterior)
            {
                DisponibilizarParaTransportadorPrioritarioPorRota(cargaJanelaCarregamento, tipoServicoMultisoftware);

                if (cargaJanelaCarregamento.ShareLiberado)
                {
                    cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.SemValorFrete;
                    new CargaJanelaCarregamento(_unitOfWork, ObterConfiguracaoEmbarcador()).AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
                }
            }
        }

        public void CalcularFreteParaTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Carga.Frete servicoFrete = new Carga.Frete(_unitOfWork, tipoServicoMultisoftware);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaCompativel = servicoFrete.ObterTabelaFreteJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador, _unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosFrete = null;

            if (tabelaCompativel != null)
                dadosFrete = servicoFrete.CalcularFretePorJanelaCarregamentoTransportador(tabelaCompativel, cargaJanelaCarregamentoTransportador, _unitOfWork, _unitOfWork.StringConexao, tipoServicoMultisoftware, configuracaoEmbarcador);

            cargaJanelaCarregamentoTransportador.ValorFreteTabela = dadosFrete?.ValorFrete ?? 0;
            cargaJanelaCarregamentoTransportador.PossuiFreteCalculado = dadosFrete?.FreteCalculado ?? false;
            cargaJanelaCarregamentoTransportador.FreteCalculadoComProblemas = dadosFrete?.FreteCalculadoComProblemas ?? false;
            cargaJanelaCarregamentoTransportador.PendenteCalcularFrete = false;
            cargaJanelaCarregamentoTransportador.TabelaFreteCliente = dadosFrete?.TabelaFreteCliente;

            if (dadosFrete != null)
            {
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponente repositorioCargaJanelaCarregamentoTransportadorComponente = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponente(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponente> componentesDeletar = repositorioCargaJanelaCarregamentoTransportadorComponente.BuscarPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponente componenteDeletar in componentesDeletar)
                    repositorioCargaJanelaCarregamentoTransportadorComponente.Deletar(componenteDeletar);

                foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFreteComponente freteComponente in dadosFrete.Componentes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponente componente = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponente()
                    {
                        Descricao = freteComponente.ComponenteFrete?.Descricao ?? "",
                        Valor = freteComponente.ValorComponente,
                        CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador
                    };

                    repositorioCargaJanelaCarregamentoTransportadorComponente.Inserir(componente);
                }
            }
            else
                cargaJanelaCarregamentoTransportador.ValorFreteTabela = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.ValorFreteAPagar;

            repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
        }

        public void CalcularFreteParaTransportadores(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

                bool regraAtiva = configuracaoEmbarcador.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela;

                if (!regraAtiva)
                    return;

                Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarCargasAguardandoCalculoFrete(limite: 5);

                if (listaCargaJanelaCarregamentoTransportador.Count == 0)
                    return;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in listaCargaJanelaCarregamentoTransportador)
                    CalcularFreteParaTransportador(cargaJanelaCarregamentoTransportador, tipoServicoMultisoftware);

                CargaJanelaCarregamento servicoCargaJanelaCarregamento = new CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = (from o in listaCargaJanelaCarregamentoTransportador select o.CargaJanelaCarregamento).Distinct().ToList();

                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
                List<Dominio.Entidades.RotaFrete> listaRotaFrete = repositorioRotaFrete.BuscarRotasFreteComDistruibuicaoDeCargasOfertadosComTransportadores();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento in listaCargaJanelaCarregamento)
                {
                    if (configuracaoEmbarcador?.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela ?? false)
                        DisponibilizarParaTransportadorPorMenorValorFreteTabelaCalculado(cargaJanelaCarregamento);

                    if (cargaJanelaCarregamento.CentroCarregamento?.AtivarRegraParaOfertarCarga ?? false)
                        DisponibilizarParaTransportadorPorRegraOferta(cargaJanelaCarregamento);

                    servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "CalcularFreteParaTransportador");
            }
        }

        public void DefinirTransportadorComValorFreteInformado(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            DefinirTransportadorComValorFreteInformado(cargaJanelaCarregamento, cargaJanelaCarregamentoTransportador, tipoServicoMultisoftware, usuario: null);
        }

        public void DefinirTransportadorComValorFreteInformado(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repositorioCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete repositorioCargaJanelaCarregamentoTransportadorComponenteFrete = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            Servicos.Embarcador.Carga.CargaAprovacaoFrete servicoCargaAprovacaoFrete = new Servicos.Embarcador.Carga.CargaAprovacaoFrete(_unitOfWork, configuracaoEmbarcador);
            Servicos.Embarcador.Carga.CargaOperador servicoCargaOperador = new Servicos.Embarcador.Carga.CargaOperador(_unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete servicoComponetesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(_unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);

            cargaJanelaCarregamento.Carga.DataAtualizacaoCarga = DateTime.Now;
            cargaJanelaCarregamento.Carga.Empresa = cargaJanelaCarregamentoTransportador.Transportador;
            cargaJanelaCarregamento.Carga.ValorFrete = cargaJanelaCarregamentoTransportador.ValorFreteTransportador;
            cargaJanelaCarregamento.Carga.ValorFreteOperador = cargaJanelaCarregamentoTransportador.ValorFreteTransportador;
            cargaJanelaCarregamento.Carga.TipoFreteEscolhido = configuracaoJanelaCarregamento.NaoPermitirRecalcularValorFreteInformadoPeloTransportador ? TipoFreteEscolhido.Embarcador : TipoFreteEscolhido.Operador;
            cargaJanelaCarregamento.Carga.OrigemFretePelaJanelaTransportador = true;

            if (usuario != null)
            {
                cargaJanelaCarregamento.Carga.OperadorContratouCarga = usuario;
                servicoCargaOperador.Atualizar(cargaJanelaCarregamento.Carga, usuario, tipoServicoMultisoftware);
            }

            if (cargaJanelaCarregamento.CentroCarregamento?.ManterComponentesTabelaFrete ?? false)
            {
                decimal teto = cargaJanelaCarregamento.CentroCarregamento?.PercentualMaximoDiferencaValorCotacao ?? 0m;
                decimal valorLimiteNaoSolicitarAprovacao = cargaJanelaCarregamento.Carga.ValorFreteTabelaFrete * (teto / 100.0m);

                if (cargaJanelaCarregamento.Carga.ValorFrete > valorLimiteNaoSolicitarAprovacao)
                    servicoCargaAprovacaoFrete.CriarAprovacao(cargaJanelaCarregamento.Carga, TipoRegraAutorizacaoCarga.InformadoManualmente, tipoServicoMultisoftware);
                else
                    servicoCargaAprovacaoFrete.RemoverAprovacao(cargaJanelaCarregamento.Carga);
            }
            else
            {
                cargaJanelaCarregamento.Carga.ValorFreteTabelaFrete = 0;
                cargaJanelaCarregamento.Carga.TabelaFrete = null;

                repositorioCargaComponentesFrete.DeletarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

                if (cargaJanelaCarregamento.Carga.CargaAgrupada)
                    repositorioCargaComponentesFrete.DeletarPorCargaAgrupamento(cargaJanelaCarregamento.Carga.Codigo, false);

                servicoCargaAprovacaoFrete.RemoverAprovacao(cargaJanelaCarregamento.Carga);
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete> componentesFrete = repositorioCargaJanelaCarregamentoTransportadorComponenteFrete.BuscarPorCargaJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportador.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete componenteFrete in componentesFrete)
                servicoComponetesFrete.AdicionarComponenteFreteCarga(cargaJanelaCarregamento.Carga, componenteFrete.ComponenteFrete, componenteFrete.ValorComponente, componenteFrete.Percentual, false, componenteFrete.TipoValor, componenteFrete.ComponenteFrete.TipoComponenteFrete, null, true, false, componenteFrete.ModeloDocumentoFiscal, tipoServicoMultisoftware, usuario, _unitOfWork, false, TipoCargaComponenteFrete.Manual, false, false, null, false, componenteFrete.Moeda, componenteFrete.ValorCotacaoMoeda, componenteFrete.ValorTotalMoeda);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(cargaJanelaCarregamento.Carga.Codigo);

            repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
            servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento.Carga, tipoServicoMultisoftware);
            servicoRateioFrete.RatearValorDoFrenteEntrePedidos(cargaJanelaCarregamento.Carga, cargaPedidos, configuracaoEmbarcador, false, _unitOfWork, tipoServicoMultisoftware, new Dominio.ObjetosDeValor.Embarcador.Carga.ConfiguracaoRateioValorFreteEntrePedidos() { ValorFreteInformadoPeloTransportador = true });
        }

        public void DefinirTransportadorSemValorFreteInformado(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Servicos.Embarcador.Carga.CargaOperador servicoCargaOperador = new Servicos.Embarcador.Carga.CargaOperador(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);

            cargaJanelaCarregamento.Carga.DataAtualizacaoCarga = DateTime.Now;
            cargaJanelaCarregamento.Carga.Empresa = cargaJanelaCarregamentoTransportador.Transportador;
            cargaJanelaCarregamento.Carga.OrigemFretePelaJanelaTransportador = true;

            if (usuario != null)
            {
                cargaJanelaCarregamento.Carga.OperadorContratouCarga = usuario;
                servicoCargaOperador.Atualizar(cargaJanelaCarregamento.Carga, usuario, tipoServicoMultisoftware);
            }

            if (!cargaJanelaCarregamento.Carga.ExigeNotaFiscalParaCalcularFrete)
            {
                cargaJanelaCarregamento.Carga.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                cargaJanelaCarregamento.Carga.PossuiPendencia = false;
                cargaJanelaCarregamento.Carga.CalcularFreteSemEstornarComplemento = true;
                cargaJanelaCarregamento.Carga.MotivoPendencia = "";
                cargaJanelaCarregamento.Carga.DataInicioCalculoFrete = DateTime.Now;
                cargaJanelaCarregamento.Carga.CalculandoFrete = true;
            }

            repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
            servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
        }

        public void DisponibilizarAutomaticamenteParaTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!(cargaJanelaCarregamento.CentroCarregamento?.LiberarCargaAutomaticamenteParaTransportadoras ?? false))
                return;

            if ((cargaJanelaCarregamento.Carga.ModeloVeicularCarga == null) || (cargaJanelaCarregamento.CargaJanelaCarregamentoAgrupador != null))
                return;

            bool bloquearLiberacaoAutomaticaPorTipoCarga = cargaJanelaCarregamento.CentroCarregamento.TiposCargaBloquearLiberacaoAutomaticaParaTransportadoras?.Any(o => o.Codigo == (cargaJanelaCarregamento.Carga.TipoDeCarga?.Codigo ?? 0)) ?? false;

            if (bloquearLiberacaoAutomaticaPorTipoCarga)
                return;

            DisponibilizarParaTransportadores(cargaJanelaCarregamento, cargaJanelaCarregamento.CentroCarregamento.TipoTransportador, transportador: null, tipoServicoMultisoftware);

            if (ObterConfiguracaoJanelaCarregamento().LiberarCargaParaCotacaoAoLiberarParaTransportadores)
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
                CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new CargaJanelaCarregamentoCotacao(_unitOfWork, configuracaoEmbarcador);
                CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, configuracaoJanelaCarregamento);

                servicoCargaJanelaCarregamentoCotacao.LiberarParaCotacaoAutomaticamente(cargaJanelaCarregamento, tipoServicoMultisoftware);
                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaLiberadaParaCotacaoParaTranportadores(cargaJanelaCarregamento);
            }
        }

        public void DisponibilizarParaTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, TipoTransportadorCentroCarregamento tipoTransportador, Dominio.Entidades.Empresa transportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            bool existePedidoInLand = repositorioCargaPedido.ExisteInLand(cargaJanelaCarregamento.Carga.Codigo);
            bool bloquearLiberacaoPorTipoCondicaoPagamento = cargaJanelaCarregamento.Carga.TipoCondicaoPagamento.HasValue && (cargaJanelaCarregamento.Carga.TipoCondicaoPagamento.Value == TipoCondicaoPagamento.FOB) && !(cargaJanelaCarregamento.Carga.TipoOperacao?.ConfiguracaoJanelaCarregamento?.PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores ?? false);
            bool bloquearLiberacaoPorTipoCarga = (cargaJanelaCarregamento.Carga.TipoDeCarga?.BloquearLiberacaoParaTransportadores ?? false) && !existePedidoInLand;

            if (bloquearLiberacaoPorTipoCondicaoPagamento || bloquearLiberacaoPorTipoCarga)
                return;

            cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.SemValorFrete;

            if (tipoTransportador == TipoTransportadorCentroCarregamento.PorPrioridadeDeRota && (cargaJanelaCarregamento.CentroCarregamento?.AtivarRegraParaOfertarCarga ?? false))
                DisponibilizarParaTransportadorPrioritarioPorRotaOfertaPrioritaria(cargaJanelaCarregamento, tipoServicoMultisoftware);
            else if (tipoTransportador == TipoTransportadorCentroCarregamento.PorPrioridadeDeRota)
                DisponibilizarParaTransportadorPrioritarioPorRota(cargaJanelaCarregamento, tipoServicoMultisoftware);
            else
                DisponibilizarParaTransportadoresPorTipo(cargaJanelaCarregamento, tipoTransportador, transportador, tipoServicoMultisoftware);
        }

        public void DisponibilizarParaTransportadoresPorCargaComPedidoInLand(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

            if (cargaJanelaCarregamento == null)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);

            if (repositorioCargaJanelaCarregamentoTransportador.VerificarExiste(cargaJanelaCarregamento.Codigo))
                return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            CargaJanelaCarregamento servicoCargaJanelaCarregamento = new CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);

            DisponibilizarParaTransportadores(cargaJanelaCarregamento, cargaJanelaCarregamento.CentroCarregamento.TipoTransportador, transportador: null, tipoServicoMultisoftware);
            servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);
        }

        public void DisponibilizarParaTransportadoresPorDataLiberacao(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            CargaJanelaCarregamento servicoCargaJanelaCarregamento = new CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            List<int> codigosJanelasCarregamentoLiberar = repositorioCargaJanelaCarregamento.BuscarCodigosJanelasCarregamentoLiberarShare();

            foreach (int codigoCargaJanelaCarregamento in codigosJanelasCarregamentoLiberar)
            {
                try
                {
                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCodigo(codigoCargaJanelaCarregamento);

                    cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.SemValorFrete;

                    DisponibilizarParaTransportadorPrioritarioPorRota(cargaJanelaCarregamento, tipoServicoMultisoftware);
                    servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamento, tipoServicoMultisoftware);

                    _unitOfWork.CommitChanges();
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    Log.TratarErro(excecao);
                }
            }
        }

        public void DisponibilizarParaTransportadoresPorGrupoTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador grupoTransportador)
        {
            cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.SemValorFrete;

            Repositorio.Embarcador.Transportadores.GrupoTransportadorEmpresa repositorioGrupoTransportadorEmpresa = new Repositorio.Embarcador.Transportadores.GrupoTransportadorEmpresa(_unitOfWork);
            List<Dominio.Entidades.Empresa> transportadores = repositorioGrupoTransportadorEmpresa.BuscarTransportadoresPorGrupoTransportador(grupoTransportador.Codigo);

            if (transportadores.Count == 0)
                return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargajanelaCarregamentoTrasnportadorInserir = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();
            int totalTransportadores = transportadores.Count;

            for (int i = 0; i < totalTransportadores; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
                {
                    CargaJanelaCarregamento = cargaJanelaCarregamento,
                    HorarioLiberacao = DateTime.Now,
                    Transportador = transportadores[i],
                    Situacao = SituacaoCargaJanelaCarregamentoTransportador.Disponivel,
                    PendenteCalcularFrete = configuracaoEmbarcador.CalcularFreteCargaJanelaCarregamentoTransportador,
                    Tipo = TipoCargaJanelaCarregamentoTransportador.PorGrupoTransportador
                };

                listaCargajanelaCarregamentoTrasnportadorInserir.Add(cargaJanelaCarregamentoTransportador);
            }

            repositorioCargaJanelaCarregamentoTransportador.InsertSQLListaCargajanelaCarregamentoTrasnportador(listaCargajanelaCarregamentoTrasnportadorInserir);
            repositorioCargaJanelaCarregamentoTransportador.InsertSQLListaCargaJanelaCarregamentoTransportadoHistorico(cargaJanelaCarregamento.Codigo);
        }

        public void DisponibilizarParaTransportadoresPorRegra(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            List<Dominio.Entidades.Empresa> transportadores = repositorioEmpresa.BuscarTransportadoresSemJanelaCarregamentoTransportadorPorCodigoJanelaCarregamento(cargaJanelaCarregamento.Codigo);

#if DEBUG
            transportadores = new int[] { 52017, 51009 }.Select(o => repositorioEmpresa.BuscarPorCodigo(o)).ToList();
#endif

            if (transportadores.Count == 0)
                return;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargajanelaCarregamentoTrasnportadorInserir = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();
            int totalTransportadores = transportadores.Count;

            for (int i = 0; i < totalTransportadores; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
                {
                    CargaJanelaCarregamento = cargaJanelaCarregamento,
                    HorarioLiberacao = DateTime.Now,
                    Transportador = transportadores[i],
                    Situacao = SituacaoCargaJanelaCarregamentoTransportador.Disponivel,
                    PendenteCalcularFrete = configuracaoEmbarcador.CalcularFreteCargaJanelaCarregamentoTransportador,
                    Tipo = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCarga
                };

                listaCargajanelaCarregamentoTrasnportadorInserir.Add(cargaJanelaCarregamentoTransportador);
            }

            repositorioCargaJanelaCarregamentoTransportador.InsertSQLListaCargajanelaCarregamentoTrasnportador(listaCargajanelaCarregamentoTrasnportadorInserir);
            repositorioCargaJanelaCarregamentoTransportador.InsertSQLListaCargaJanelaCarregamentoTransportadoHistorico(cargaJanelaCarregamento.Codigo);
        }

        public void DisponibilizarParaTransportadoresPorTipo(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, TipoTransportadorCentroCarregamento tipoTransportador, Dominio.Entidades.Empresa transportador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaJanelaCarregamento.Carga == null || cargaJanelaCarregamento.Carga.ModeloVeicularCarga == null || cargaJanelaCarregamento.Carga.TipoDeCarga == null)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repositorioTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();
            decimal numeroPaletesPedidos = repositorioCargaPedido.BuscarNumeroPaletesPorCarga(cargaJanelaCarregamento.Carga?.Codigo ?? 0);
            int numeroPallet = cargaJanelaCarregamento.Carga.ModeloVeicularCarga?.NumeroPaletes ?? (int)numeroPaletesPedidos;
            List<int> codigosTransportadores = new List<int>();
            List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModeloVeicular = repositorioTipoCargaModeloVeicular.ConsultarPorTipoCarga(cargaJanelaCarregamento.Carga.TipoDeCarga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos = (from o in tipoCargaModeloVeicular select o.ModeloVeicularCarga).ToList();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = ObterConfiguracaoEmbarcador();

            CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracao, configuracaoJanelaCarregamento);

            switch (tipoTransportador)
            {
                case TipoTransportadorCentroCarregamento.Todos:
                    codigosTransportadores = repositorioEmpresa.BuscarCodigosEmpresasEmissoras();
                    break;

                case TipoTransportadorCentroCarregamento.TodosCentroCarregamento:
                    if (cargaJanelaCarregamento.CentroCarregamento == null)
                        return;

                    codigosTransportadores = cargaJanelaCarregamento.CentroCarregamento.Transportadores.Select(o => o.Transportador.Codigo).ToList();
                    break;

                case TipoTransportadorCentroCarregamento.TodosCentroCarregamentoComTipoVeiculoCarga:
                    if (cargaJanelaCarregamento.CentroCarregamento == null || cargaJanelaCarregamento.Carga.ModeloVeicularCarga == null)
                        return;

                    codigosTransportadores = repositorioVeiculo.BuscarCodigosTransportadoresPorTipoVeiculo(cargaJanelaCarregamento.CentroCarregamento.Codigo, numeroPallet, modelosVeicularesPermitidos);
                    break;

                case TipoTransportadorCentroCarregamento.TodosComTipoVeiculoCarga:
                    codigosTransportadores = repositorioVeiculo.BuscarCodigosTransportadoresPorTipoVeiculo(numeroPallet, modelosVeicularesPermitidos);
                    break;

                case TipoTransportadorCentroCarregamento.PorGrupoRegional:
                    codigosTransportadores = BuscarCodigosTransportadoresPorGrupoRegional(cargaJanelaCarregamento);
                    break;

                case TipoTransportadorCentroCarregamento.TransportadorExclusivo:
                    if (transportador != null)
                        codigosTransportadores.Add(transportador.Codigo);

                    break;
            }

            List<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao> listaTempoEsperaPorPontuacao = ObterListaTempoEsperaPorPontuacao();
            List<Dominio.Entidades.Empresa> transportadores = new List<Dominio.Entidades.Empresa>();

            if (codigosTransportadores.Count < 2000)
                transportadores = repositorioEmpresa.BuscarPorCodigos(codigosTransportadores);
            else
            {
                try
                {
                    decimal decimalBlocos = Math.Ceiling(((decimal)codigosTransportadores.Count) / 1000);
                    int blocos = (int)Math.Truncate(decimalBlocos);

                    for (int i = 0; i < blocos; i++)
                    {
                        Log.TratarErro($"blocos {codigosTransportadores.Count} indice {i}");
                        transportadores.AddRange(repositorioEmpresa.BuscarPorCodigos(codigosTransportadores.Skip(i * 1000).Take(1000).ToList()));
                    }
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);
                }
            }

            List<int> janelaTransportadores = repositorioCargaJanelaCarregamentoTransportador.BuscarTransportadoresPorCargaJanelaCarregamento(cargaJanelaCarregamento.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargajanelaCarregamentoTrasnportadorInserir = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            SituacaoCargaJanelaCarregamentoTransportador situacaoCargaJanelaCarregamentoTransportador = (transportador != null) && !(configuracaoJanelaCarregamento?.LiberarCargaParaCotacaoAoLiberarParaTransportadores ?? false) ? SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao : SituacaoCargaJanelaCarregamentoTransportador.Disponivel;

            bool validacaoPendenciaCalcularFrete = (configuracao.CalcularFreteCargaJanelaCarregamentoTransportador)
                || (configuracao.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela)
                || (cargaJanelaCarregamento.CentroCarregamento?.AtivarRegraParaOfertarCarga ?? false);

            int tempoAguardarInteresse = 0;
            TipoCargaJanelaCarregamentoTransportador tipoTransportadorJanelaCarregamento = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCarga;

            if (cargaJanelaCarregamento.CentroCarregamento != null && cargaJanelaCarregamento.CentroCarregamento.TipoTransportadorSecundario.HasValue
                && (cargaJanelaCarregamento.CentroCarregamento.TipoTransportadorSecundario.Value == tipoTransportador)
                && cargaJanelaCarregamento.CentroCarregamento.TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente > 0)
            {
                tempoAguardarInteresse = cargaJanelaCarregamento.CentroCarregamento.TempoAguardarInteresseTransportadorParaCargaLiberadaAutomaticamente;
                tipoTransportadorJanelaCarregamento = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCargaSecundario;
            }

            foreach (Dominio.Entidades.Empresa transportadorAdicionarJanela in transportadores)
            {
                if (janelaTransportadores.Contains(transportadorAdicionarJanela.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador
                {
                    CargaJanelaCarregamento = cargaJanelaCarregamento,
                    HorarioLiberacao = ObterHorarioLiberacao(listaTempoEsperaPorPontuacao, transportadorAdicionarJanela),
                    Transportador = transportadorAdicionarJanela,
                    Situacao = situacaoCargaJanelaCarregamentoTransportador,
                    PendenteCalcularFrete = validacaoPendenciaCalcularFrete,
                    Tipo = tipoTransportadorJanelaCarregamento,
                    Bloqueada = configuracao.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela || (cargaJanelaCarregamento.CentroCarregamento?.AtivarRegraParaOfertarCarga ?? false),
                    HorarioLimiteConfirmarCarga = tempoAguardarInteresse > 0 ? DateTime.Now.AddMinutes(tempoAguardarInteresse) : null
                };

                listaCargajanelaCarregamentoTrasnportadorInserir.Add(cargaJanelaCarregamentoTransportador);
            }

            if (listaCargajanelaCarregamentoTrasnportadorInserir.Count > 0)
            {
                repositorioCargaJanelaCarregamentoTransportador.InsertSQLListaCargajanelaCarregamentoTrasnportador(listaCargajanelaCarregamentoTrasnportadorInserir);

                if (!configuracao.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela)
                    repositorioCargaJanelaCarregamentoTransportador.InsertSQLListaCargaJanelaCarregamentoTransportadoHistorico(cargaJanelaCarregamento.Codigo);
            }

            if ((transportadores.Count > 0) && !configuracao.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela)
                DefinirDataDisponibilizacaoTransportadores(cargaJanelaCarregamento);
        }

        public void DisponibilizarParaTransportadorPorMenorValorFreteTabelaCalculado(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);

            if (repositorioCargaJanelaCarregamentoTransportador.PossuiCargasAguardandoCalculoFrete(cargaJanelaCarregamento.Codigo))
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado = repositorioCargaJanelaCarregamentoTransportador.BuscarPorValorFreteTabelaCalculado(cargaJanelaCarregamento.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculadoFiltradaPorTipoFrete = (
                from o in listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado
                where o.TabelaFreteCliente?.TabelaFrete?.TipoFreteTabelaFrete == TipoFreteTabelaFrete.Terceiro
                select o
            ).ToList();

            if (listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculadoFiltradaPorTipoFrete.Count == 0)
            {
                listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculadoFiltradaPorTipoFrete = (
                    from o in listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado
                    where o.TabelaFreteCliente?.TabelaFrete?.TipoFreteTabelaFrete == TipoFreteTabelaFrete.Spot
                    select o
                ).ToList();
            }

            if (listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculadoFiltradaPorTipoFrete.Count == 0)
            {
                listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculadoFiltradaPorTipoFrete = (
                    from o in listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado
                    where o.TabelaFreteCliente == null || o.TabelaFreteCliente.TabelaFrete.TipoFreteTabelaFrete == TipoFreteTabelaFrete.NaoInformado
                    select o
                ).ToList();
            }

            CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new CargaJanelaCarregamentoNotificacao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargaJanelaCarregamentoTransportadorMenorValorFreteTabelaCalculado = null;
            decimal menorValorFreteTabelaCalculado = listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculadoFiltradaPorTipoFrete.Min(o => (decimal?)o.ObterValorFreteTabelaComComponentes()) ?? 0m;

            if (menorValorFreteTabelaCalculado > 0m)
                listaCargaJanelaCarregamentoTransportadorMenorValorFreteTabelaCalculado = listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculadoFiltradaPorTipoFrete.Where(o => o.ObterValorFreteTabelaComComponentes() == menorValorFreteTabelaCalculado).ToList();
            else
                listaCargaJanelaCarregamentoTransportadorMenorValorFreteTabelaCalculado = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>();

            if (listaCargaJanelaCarregamentoTransportadorMenorValorFreteTabelaCalculado.Count == 0)
            {
                int totalCargaJanelaCarregamentoTransportadorDesbloqueadas = repositorioCargaJanelaCarregamentoTransportador.DisponibilizarTodasBloqueadasPorCalculoFrete(cargaJanelaCarregamento.Codigo);

                if (cargaJanelaCarregamento.CargaBase.IsCarga() && (totalCargaJanelaCarregamentoTransportadorDesbloqueadas > 0))
                {
                    cargaJanelaCarregamento.Carga.RejeitadaPeloTransportador = false;

                    repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
                }
            }
            else if (listaCargaJanelaCarregamentoTransportadorMenorValorFreteTabelaCalculado.Count == 1)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = listaCargaJanelaCarregamentoTransportadorMenorValorFreteTabelaCalculado.FirstOrDefault();

                cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CargaBase.Empresa = cargaJanelaCarregamentoTransportador.Transportador;
                cargaJanelaCarregamentoTransportador.Bloqueada = false;
                cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao;
                cargaJanelaCarregamentoTransportador.Tipo = TipoCargaJanelaCarregamentoTransportador.PorMenorValorFreteTabelaCalculado;

                repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga disponibilizada para o transportador informar os dados de transporte por menor valor de frete");
                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador);

                if (cargaJanelaCarregamento.CargaBase.IsCarga())
                {
                    cargaJanelaCarregamento.Carga.Empresa = cargaJanelaCarregamentoTransportador.Transportador;
                    cargaJanelaCarregamento.Carga.DataAtualizacaoCarga = DateTime.Now;
                    cargaJanelaCarregamento.Carga.RejeitadaPeloTransportador = false;

                    repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
                }
                else
                {
                    cargaJanelaCarregamento.PreCarga.Empresa = cargaJanelaCarregamentoTransportador.Transportador;

                    repositorioPreCarga.Atualizar(cargaJanelaCarregamento.PreCarga);
                }
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in listaCargaJanelaCarregamentoTransportadorMenorValorFreteTabelaCalculado)
                {
                    cargaJanelaCarregamentoTransportador.Bloqueada = false;
                    cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.ComInteresse;
                    cargaJanelaCarregamentoTransportador.Tipo = TipoCargaJanelaCarregamentoTransportador.PorMenorValorFreteTabelaCalculado;

                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                    SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga disponibilizada para o transportador por menor valor de frete");
                }

                if (cargaJanelaCarregamento.CargaBase.IsCarga())
                {
                    cargaJanelaCarregamento.Carga.RejeitadaPeloTransportador = false;

                    repositorioCarga.Atualizar(cargaJanelaCarregamento.Carga);
                }
            }
        }

        public void DisponibilizarParaTransportadorPrioritarioPorRota(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaJanelaCarregamento.Carga?.Rota == null)
                return;

            if ((cargaJanelaCarregamento.Carga?.Empresa != null) && !(cargaJanelaCarregamento.CentroCarregamento?.AguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente ?? false))
                return;

            if (cargaJanelaCarregamento.DataLiberacaoShare.HasValue && cargaJanelaCarregamento.DataLiberacaoShare.Value > DateTime.Now)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            List<Dominio.Entidades.RotaFreteEmpresa> rotaFreteEmpresas = ObterRotaFreteEmpresas(cargaJanelaCarregamento.Carga);
            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFreteEmpresa> configuracaoRotaFreteEmpresas = ObterConfiguracaoRotaFreteEmpresas(cargaJanelaCarregamento.Carga, tipoServicoMultisoftware);

            if ((rotaFreteEmpresas.Count == 0) && (configuracaoRotaFreteEmpresas.Count == 0))
                return;

            if (cargaJanelaCarregamento.CentroCarregamento?.LiberarCargaAutomaticamenteParaTransportadorasForaRota ?? false)
            {
                if (repositorioCargaJanelaCarregamentoTransportador.VerificarExisteDesbloqueadasSemRejeicaoPorPrioridadeRotaGrupo(cargaJanelaCarregamento.Codigo))
                    return;

                if (repositorioCargaJanelaCarregamentoTransportador.VerificarExisteBloqueadasPorPrioridadeRotaGrupo(cargaJanelaCarregamento.Codigo))
                {
                    cargaJanelaCarregamento.Carga.RejeitadaPeloTransportador = false;
                    repositorioCargaJanelaCarregamentoTransportador.DesbloquearTodasPorPrioridadeRotaGrupo(cargaJanelaCarregamento.Codigo);
                    return;
                }
            }

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            bool primeiroTransportadorOfertado = repositorioCargaJanelaCarregamentoTransportador.ContarPorCargaJanelaCarregamento(cargaJanelaCarregamento.Codigo) == 0;
            int tempoAguardarConfirmacaoTransportador = cargaJanelaCarregamento.CentroCarregamento?.TempoAguardarConfirmacaoTransportadorParaCargaLiberadaAutomaticamente ?? 0;

            if (cargaJanelaCarregamento.Carga?.Empresa != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(cargaJanelaCarregamento.Carga.Codigo, cargaJanelaCarregamento.Carga.Empresa.Codigo);

                if (cargaJanelaCarregamentoTransportador != null)
                    return;

                cargaJanelaCarregamento.DataLiberacaoShare = DateTime.Now;
                cargaJanelaCarregamento.ShareLiberado = true;

                cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
                {
                    PrimeiroTransportadorOfertado = primeiroTransportadorOfertado,
                    CargaJanelaCarregamento = cargaJanelaCarregamento,
                    HorarioLiberacao = ObterHorarioLiberacao(cargaJanelaCarregamento.Carga.Empresa),
                    PendenteCalcularFrete = configuracaoEmbarcador.CalcularFreteCargaJanelaCarregamentoTransportador,
                    Transportador = cargaJanelaCarregamento.Carga.Empresa,
                    HorarioLimiteConfirmarCarga = (tempoAguardarConfirmacaoTransportador > 0) ? (DateTime?)DateTime.Now.AddMinutes(tempoAguardarConfirmacaoTransportador) : null,
                    Tipo = TipoCargaJanelaCarregamentoTransportador.PorTipoTransportadorCarga
                };

                repositorioCargaJanelaCarregamentoTransportador.Inserir(cargaJanelaCarregamentoTransportador);
                return;
            }

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = ObterConfiguracaoJanelaCarregamento();
            List<int> codigosTransportadoresRejeitaram = repositorioCargaJanelaCarregamentoTransportador.BuscarTransportadoresDisponibilizadosRejeitaram(cargaJanelaCarregamento.Codigo);
            (Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresa OfertaEscolhida, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteEmpresaOferta> Ofertas) retornoOferta = ObterRotaFreteEmpresaAdicionarCargaJanelaCarregamentoTransportador(cargaJanelaCarregamento, rotaFreteEmpresas, configuracaoRotaFreteEmpresas, codigosTransportadoresRejeitaram);

            if (retornoOferta.OfertaEscolhida != null)
            {
                DefinirDataLiberacaoInicial(cargaJanelaCarregamento, retornoOferta.OfertaEscolhida.ConfiguracaoRotaFrete);

                if (cargaJanelaCarregamento.DataLiberacaoShare.HasValue && (cargaJanelaCarregamento.DataLiberacaoShare.Value > DateTime.Now))
                {
                    cargaJanelaCarregamento.Situacao = SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores;
                    return;
                }

                cargaJanelaCarregamento.ShareLiberado = true;
                cargaJanelaCarregamento.Carga.DataAtualizacaoCarga = DateTime.Now;
                cargaJanelaCarregamento.Carga.Empresa = retornoOferta.OfertaEscolhida.Empresa;
                cargaJanelaCarregamento.Carga.RejeitadaPeloTransportador = false;

                Carga.CargaIndicador servicoCargaIndicador = new Carga.CargaIndicador(_unitOfWork);
                servicoCargaIndicador.DefinirIndicadorTransportador(cargaJanelaCarregamento.Carga, CargaIndicadorTransportador.InformadoViaShareRota);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(cargaJanelaCarregamento.Carga.Codigo, cargaJanelaCarregamento.Carga.Empresa.Codigo);

                if (cargaJanelaCarregamentoTransportador != null)
                {
                    cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgAceite;
                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                    return;
                }

                cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
                {
                    PrimeiroTransportadorOfertado = primeiroTransportadorOfertado,
                    CargaJanelaCarregamento = cargaJanelaCarregamento,
                    HorarioLiberacao = ObterHorarioLiberacao(retornoOferta.OfertaEscolhida.Empresa),
                    PendenteCalcularFrete = configuracaoEmbarcador.CalcularFreteCargaJanelaCarregamentoTransportador,
                    Transportador = retornoOferta.OfertaEscolhida.Empresa,
                    HorarioLimiteConfirmarCarga = (tempoAguardarConfirmacaoTransportador > 0) ? (DateTime?)DateTime.Now.AddMinutes(tempoAguardarConfirmacaoTransportador) : null,
                    Tipo = TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRota
                };

                repositorioCargaJanelaCarregamentoTransportador.Inserir(cargaJanelaCarregamentoTransportador);
                SalvarHistoricoOfertaPorRota(cargaJanelaCarregamentoTransportador, retornoOferta.Ofertas);

                return;
            }
            else if ((cargaJanelaCarregamento.CentroCarregamento?.LiberarParaCotacaoAposLimiteConfirmacaoTransportadorParaCargaLiberadaAutomaticamente ?? false) && !cargaJanelaCarregamento.CargaLiberadaCotacao && !cargaJanelaCarregamento.ProcessoCotacaoFinalizada)
            {
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema
                };

                CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new CargaJanelaCarregamentoCotacao(_unitOfWork, configuracaoEmbarcador, auditoria);
                CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new CargaJanelaCarregamentoNotificacao(_unitOfWork, configuracaoEmbarcador, configuracaoJanelaCarregamento);

                servicoCargaJanelaCarregamentoCotacao.LiberarParaCotacaoAutomaticamente(cargaJanelaCarregamento, tipoServicoMultisoftware);
                DisponibilizarParaTransportadoresPorRegra(cargaJanelaCarregamento);
                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaLiberadaParaCotacaoParaTranportadores(cargaJanelaCarregamento);
            }

            if (cargaJanelaCarregamento.CentroCarregamento?.LiberarCargaAutomaticamenteParaTransportadorasForaRota ?? false)
            {
                List<Dominio.Entidades.Empresa> transportadoresPorPrioridadeRotaGrupo = repositorioCargaJanelaCarregamentoTransportador.BuscarTransportadoresPorPrioridadeRotaGrupo(cargaJanelaCarregamento.Carga.Filial?.Codigo ?? 0, cargaJanelaCarregamento.Carga.TipoDeCarga?.Codigo ?? 0, diasHistorico: 0);
                List<int> codigosTransportadoresRejeitaramPorRotaGrupo = repositorioCargaJanelaCarregamentoTransportador.BuscarTransportadoresDisponibilizadosRejeitaram(cargaJanelaCarregamento.Codigo, TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRotaGrupo);

                foreach (Dominio.Entidades.Empresa transportador in transportadoresPorPrioridadeRotaGrupo)
                {
                    if (codigosTransportadoresRejeitaramPorRotaGrupo.Any(o => o == transportador.Codigo))
                        continue;

                    cargaJanelaCarregamento.Carga.RejeitadaPeloTransportador = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
                    {
                        CargaJanelaCarregamento = cargaJanelaCarregamento,
                        HorarioLiberacao = ObterHorarioLiberacao(transportador),
                        PendenteCalcularFrete = configuracaoEmbarcador.CalcularFreteCargaJanelaCarregamentoTransportador,
                        Transportador = transportador,
                        Tipo = TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRotaGrupo
                    };

                    repositorioCargaJanelaCarregamentoTransportador.Inserir(cargaJanelaCarregamentoTransportador);
                    SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga disponibilizada para o transportador fora da rota");
                }
            }

            if (cargaJanelaCarregamento.CentroCarregamento?.TipoTransportadorSecundario.HasValue ?? false)
            {
                TipoTransportadorCentroCarregamento tipoTransportadorSecundario = cargaJanelaCarregamento.CentroCarregamento.TipoTransportadorSecundario.Value;

                List<SituacaoCargaJanelaCarregamentoTransportador> situacoesTransportadoresSecundarios = repositorioCargaJanelaCarregamentoTransportador.BuscarSituacoesTransportadoresSecundarios(cargaJanelaCarregamento.Codigo);

                if (situacoesTransportadoresSecundarios.Count > 0 && situacoesTransportadoresSecundarios.TrueForAll(situacao => situacao == SituacaoCargaJanelaCarregamentoTransportador.Rejeitada))
                    tipoTransportadorSecundario = TipoTransportadorCentroCarregamento.Todos;

                DisponibilizarParaTransportadoresPorTipo(cargaJanelaCarregamento, tipoTransportadorSecundario, transportador: null, tipoServicoMultisoftware);
            }
        }

        public void DisponibilizarParaTransportadorPorRegraOferta(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamentoOfertaCarga repositorioCentroCarregamentoOfertaCarga = new Repositorio.Embarcador.Logistica.CentroCarregamentoOfertaCarga(_unitOfWork);

            if (repositorioCargaJanelaCarregamentoTransportador.PossuiCargasAguardandoCalculoFrete(cargaJanelaCarregamento.Codigo))
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado = repositorioCargaJanelaCarregamentoTransportador.BuscarPorValorFreteTabelaCalculado(cargaJanelaCarregamento.Codigo);

            List<Dominio.Entidades.Empresa> listaEmpresaRotaFreteFiltrada = new List<Dominio.Entidades.Empresa>();
            List<Dominio.Entidades.RotaFrete> listaRotaFreteComTransportadores = listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado.Select(o => o.CargaJanelaCarregamento.Carga.Rota).ToList();

            if (listaRotaFreteComTransportadores.Count == 0)
                return;

            foreach (Dominio.Entidades.RotaFrete rotaFrete in listaRotaFreteComTransportadores)
                listaEmpresaRotaFreteFiltrada.AddRange(rotaFrete.Empresas.Select(o => o.Empresa).ToList());

            if (listaEmpresaRotaFreteFiltrada.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga> CentroCarregamentoOfertasCarga = repositorioCentroCarregamentoOfertaCarga.BuscarPorCentroCarregamento(cargaJanelaCarregamento.CentroCarregamento.Codigo);

            List<RegraOfertaCarga> regraOfertaCarga = new List<RegraOfertaCarga>();

            foreach (Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoOfertaCarga CentroCarregamentoOfertaCarga in CentroCarregamentoOfertasCarga)
                regraOfertaCarga.Add(CentroCarregamentoOfertaCarga.Regra);

            // TODO: ToList não tem .Find
            listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado = repositorioCargaJanelaCarregamentoTransportador.BuscarCargaJanelaCarregamentoTransportadorPorOrdenacaoDeRegraParaOfertarCarga(listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado.Select(o => o.Codigo).ToList(), regraOfertaCarga).ToList();

            int ordem = 1;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado)
            {
                cargaJanelaCarregamentoTransportador.Ordem = ordem;

                ordem++;

                repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, "Carga disponibilizada para o transportador por menor valor de frete");
            }

            if (listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador primeiraCargaJanelaCarregamentoTransportador = listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado.Find(o => o.Ordem == 1);

                primeiraCargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgAceite;
                primeiraCargaJanelaCarregamentoTransportador.Bloqueada = false;

                repositorioCargaJanelaCarregamentoTransportador.Atualizar(primeiraCargaJanelaCarregamentoTransportador);
            }
        }

        public void DisponibilizarParaTransportadorPrioritarioPorRotaOfertaPrioritaria(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((cargaJanelaCarregamento.Carga?.Rota == null) || (cargaJanelaCarregamento.Carga?.Empresa != null))
                return;

            if (cargaJanelaCarregamento.ShareLiberado)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            List<Dominio.Entidades.RotaFreteEmpresa> rotaFreteEmpresas = ObterRotaFreteEmpresas(cargaJanelaCarregamento.Carga);

            if (rotaFreteEmpresas.Count == 0)
                return;

            Carga.CargaIndicador servicoCargaIndicador = new Carga.CargaIndicador(_unitOfWork);

            cargaJanelaCarregamento.ShareLiberado = true;
            cargaJanelaCarregamento.Carga.DataAtualizacaoCarga = DateTime.Now;
            cargaJanelaCarregamento.Carga.RejeitadaPeloTransportador = false;

            servicoCargaIndicador.DefinirIndicadorTransportador(cargaJanelaCarregamento.Carga, CargaIndicadorTransportador.InformadoViaShareRota);

            foreach (Dominio.Entidades.RotaFreteEmpresa retornoOferta in rotaFreteEmpresas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCargaETransportador(cargaJanelaCarregamento.Carga.Codigo, retornoOferta.Empresa.Codigo);

                if (cargaJanelaCarregamentoTransportador != null)
                    continue;

                cargaJanelaCarregamentoTransportador = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador()
                {
                    CargaJanelaCarregamento = cargaJanelaCarregamento,
                    HorarioLiberacao = ObterHorarioLiberacao(retornoOferta.Empresa),
                    PendenteCalcularFrete = true,
                    Transportador = retornoOferta.Empresa,
                    Tipo = TipoCargaJanelaCarregamentoTransportador.PorPrioridadeRota,
                    Bloqueada = true,
                    Situacao = SituacaoCargaJanelaCarregamentoTransportador.Disponivel,
                };

                repositorioCargaJanelaCarregamentoTransportador.Inserir(cargaJanelaCarregamentoTransportador);
            }
        }

        public void LimparTransportadoresRejeitadas(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador janelaPrimeiroTransportador = repositorioCargaJanelaCarregamentoTransportador.BuscarPrimeiraJanelaOfertada(cargaJanelaCarregamento.Codigo);

            repositorioCargaJanelaCarregamentoTransportador.SetarCargaJanelaCarregamentoTransportadoRejeitada(cargaJanelaCarregamento.Codigo, janelaPrimeiroTransportador?.Codigo ?? 0);

            if (janelaPrimeiroTransportador == null)
                return;

            janelaPrimeiroTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgAceite;
            repositorioCargaJanelaCarregamentoTransportador.Atualizar(janelaPrimeiroTransportador);
            SalvarHistoricoAlteracao(janelaPrimeiroTransportador, "Carga disponibilizada para o transportador realizar a confirmaçao");
        }

        public DateTime ObterHorarioLiberacao(Dominio.Entidades.Empresa transportador)
        {
            List<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao> listaTempoEsperaPorPontuacao = ObterListaTempoEsperaPorPontuacao();

            return ObterHorarioLiberacao(listaTempoEsperaPorPontuacao, transportador);
        }

        public void RejeitarJanelasCarregamentoTransportadorPorTempoEncerrado(SituacaoCargaJanelaCarregamentoTransportador situacaoAlvo, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);

            List<int> listaCodigoCargaJanelaCarregamentoTransportador = situacaoAlvo switch
            {
                SituacaoCargaJanelaCarregamentoTransportador.AgAceite => repositorioCargaJanelaCarregamentoTransportador.BuscarCodigosPorTempoAceiteEncerrado(limiteRegistros: 5),
                SituacaoCargaJanelaCarregamentoTransportador.Disponivel => repositorioCargaJanelaCarregamentoTransportador.BuscarCodigosPorTempoInteresseEncerrado(limiteRegistros: 5),
                _ => new List<int>()
            };

            if (listaCodigoCargaJanelaCarregamentoTransportador.Count == 0)
                return;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            CargaJanelaCarregamento servicoCargaJanelaCarregamento = new CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);
            CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new CargaJanelaCarregamentoTransportadorConsulta(_unitOfWork);
            Hubs.JanelaCarregamento servicoNotificacaoJanelaCarregamento = new Hubs.JanelaCarregamento();

            foreach (int codigoCargaJanelaCarregamentoTransportador in listaCodigoCargaJanelaCarregamentoTransportador)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCodigo(codigoCargaJanelaCarregamentoTransportador);

                    if (cargaJanelaCarregamentoTransportadorReferencia.Situacao != situacaoAlvo)
                        continue;

                    _unitOfWork.Start();

                    List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador = servicoCargaJanelaCarregamentoTransportadorConsulta.ObterCargasJanelaCarregamentoTransportador(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga.Codigo, cargaJanelaCarregamentoTransportadorReferencia.Transportador, retornarCargasOriginais: true);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
                    {
                        RejeitarJanelaCarregamentoTransportadorPorTempoConfirmacaoEncerrado(cargaJanelaCarregamentoTransportador, cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga);
                        servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, tipoServicoMultisoftware);
                    }

                    Auditoria.Auditoria.Auditar(auditoria, cargaJanelaCarregamentoTransportadorReferencia, null, "Carga rejeitada por tempo de confirmação encerrado.", _unitOfWork);

                    DisponibilizarParaTransportadorPrioritarioPorRota(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, tipoServicoMultisoftware);
                    servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, tipoServicoMultisoftware);

                    _unitOfWork.CommitChanges();

                    servicoNotificacaoJanelaCarregamento.InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento);
                }
                catch (Exception excecao)
                {
                    _unitOfWork.Rollback();
                    Log.TratarErro(excecao);
                }
            }
        }

        public void SalvarHistoricoAlteracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, string mensagem)
        {
            SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, mensagem, usuario: null);
        }

        public void SalvarHistoricoAlteracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, string mensagem, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico repositorioHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico historico = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico()
            {
                CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador,
                Data = DateTime.Now,
                Descricao = mensagem,
                Tipo = TipoCargaJanelaCarregamentoTransportadorHistorico.RegistroAlteracao,
                Usuario = usuario
            };

            repositorioHistorico.Inserir(historico);
        }
        public void SalvarHistoricoAlteracao(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, string mensagem, TipoCargaJanelaCarregamentoTransportadorHistorico tipo, Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento motivo, string justificativa, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico repositorioHistorico = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico historico = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico()
            {
                CargaJanelaCarregamentoTransportador = cargaJanelaCarregamentoTransportador,
                Data = DateTime.Now,
                Descricao = mensagem,
                Tipo = tipo,
                Usuario = usuario,
                MotivoRetiradaFilaCarregamento = motivo,
                Justificativa = justificativa
            };

            repositorioHistorico.Inserir(historico);
        }

        public void ValidarCargasInteressadas(Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repositorioTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModeloVeicular = repositorioTipoCargaModeloVeicular.ConsultarPorTipoCarga(carga.TipoDeCarga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos = (from o in tipoCargaModeloVeicular select o.ModeloVeicularCarga).ToList();
            int numeroPallet = carga.ModeloVeicularCarga?.NumeroPaletes != null ? carga.ModeloVeicularCarga.NumeroPaletes.Value : carga.Pedidos.Sum(obj => obj.Pedido.NumeroPaletes);

            ValidarCargasInteressadas(transportador, modelosVeicularesPermitidos, numeroPallet);
        }

        public void ValidarCargasInteressadas(Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            int numeroPallet = modeloVeicular?.NumeroPaletes != null ? modeloVeicular.NumeroPaletes.Value : 0;

            ValidarCargasInteressadas(transportador, modelosVeicularesPermitidos, numeroPallet);
        }

        public void ValidarPermissaoMarcarInteresseCarga(Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (configuracaoEmbarcador.NaoExigeInformarDisponibilidadeDeVeiculo)
                return;

            Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repositorioTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(_unitOfWork);
            Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento repositorioVeiculoDisponivelCarregamento = new Repositorio.Embarcador.Logistica.VeiculoDisponivelCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModeloVeicular = repositorioTipoCargaModeloVeicular.ConsultarPorTipoCarga(carga.TipoDeCarga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesPermitidos = (from o in tipoCargaModeloVeicular select o.ModeloVeicularCarga).ToList();
            int numeroVeiculosDisponiveisSemModelo = repositorioVeiculoDisponivelCarregamento.ContarNumeroVeiculosDisponiveisSemModelo(transportador.Codigo);
            int numeroPallet = carga.ModeloVeicularCarga.NumeroPaletes.HasValue ? carga.ModeloVeicularCarga.NumeroPaletes.Value : carga.Pedidos.Sum(o => o.Pedido.NumeroPaletes);

            if (carga.TipoOperacao?.ExigeQueVeiculoIgualModeloVeicularDaCarga ?? false)
            {
                numeroVeiculosDisponiveisSemModelo = 0;

                if (carga.ModeloVeicularCarga != null)
                    modelosVeicularesPermitidos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>() { carga.ModeloVeicularCarga };
            }

            int numeroVeiculosDisponiveisParaEssaCarga = repositorioVeiculoDisponivelCarregamento.ContarNumeroVeiculosDisponiveisPodemFazerCarga(numeroPallet, modelosVeicularesPermitidos, transportador.Codigo);
            int totalVeiculos = numeroVeiculosDisponiveisParaEssaCarga + numeroVeiculosDisponiveisSemModelo;

            if (totalVeiculos <= 0)
                throw new ServicoException($"Você não possui veículos disponíveis que possam ser utilizados para fazer esta carga ({carga.CodigoCargaEmbarcador}).");

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            int numeroCargasAguardandoConfirmacao = repositorioCargaJanelaCarregamentoTransportador.ContarNumeroCargasAguardandAceiteOuConfirmacaoPorTipoCargaEPallets(numeroPallet, transportador.Codigo, modelosVeicularesPermitidos);

            if (numeroVeiculosDisponiveisParaEssaCarga > numeroCargasAguardandoConfirmacao)
                return;

            if (numeroVeiculosDisponiveisSemModelo > 0)
            {
                int totalCargaAguardandoAceiteOuConfirmacao = repositorioCargaJanelaCarregamentoTransportador.ContarPorSituacoes(new List<SituacaoCargaJanelaCarregamentoTransportador> { SituacaoCargaJanelaCarregamentoTransportador.AgAceite, SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao }, transportador.Codigo);

                if (totalVeiculos > totalCargaAguardandoAceiteOuConfirmacao)
                    return;
            }

            throw new ServicoException($"Não é possível marcar interesse nesta carga ({carga.CodigoCargaEmbarcador}), pois, existem cargas aguardando a confirmação que irão utilizar todos os possiveis veículos disponíveis para essa carga.");
        }

        public void RejeitarCarga(int codigoCarga, string motivoRejeicaoCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string adminStringConexao)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = (from o in cargasJanelaCarregamentoTransportador where o.CargaJanelaCarregamento.Carga.Codigo == codigoCarga select o).FirstOrDefault();

            if (cargaJanelaCarregamentoTransportadorReferencia == null)
                throw new ServicoException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaNaoVinculadaAoTransportador);

            if ((cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.AgAceite) && (cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao))
                throw new ServicoException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ASituacaoDaCargaNaoPermiteRejeicao);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga;

            if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware))
                throw new ServicoException((string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AAtualSituacaoDaCargaNaoPermiteRejeicao, carga.DescricaoSituacaoCarga)));

            _unitOfWork.Start();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);

            List<Dominio.Entidades.Usuario> usuariosNotificacao = new List<Dominio.Entidades.Usuario>();
            List<string> emailsNotificacao = new List<string>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
            {
                RejeitarCarga(cargaJanelaCarregamentoTransportador, cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga, motivoRejeicaoCarga, usuario, auditado);

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento;
                bool shouldNotificarPorEmail = centroCarregamento?.EnviarNotificacoesPorEmail ?? false;
                bool notificarSomenteCargasRejeitadas = centroCarregamento?.EnviarNotificacoesCargasRejeitadasPorEmail ?? false;

                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail> emailsCentroCarregamento = (shouldNotificarPorEmail || notificarSomenteCargasRejeitadas) ? centroCarregamento?.Emails.ToList() : null;
                List<Dominio.Entidades.Usuario> usuariosNotificacaoCarregamento = centroCarregamento?.UsuariosNotificacao.ToList() ?? new List<Dominio.Entidades.Usuario>();

                if (usuariosNotificacaoCarregamento.Count > 0)
                    usuariosNotificacao.AddRange(usuariosNotificacaoCarregamento);
                else if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Operador != null)
                    usuariosNotificacao.Add(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Operador);

                if (emailsCentroCarregamento != null && emailsCentroCarregamento.Count > 0)
                    emailsNotificacao.AddRange(emailsCentroCarregamento.Select(o => o.Email));

                if (!(centroCarregamento?.NaoEnviarNotificacaoCargaRejeitadaParaTransportador ?? false))
                    emailsNotificacao.Add(cargaJanelaCarregamentoTransportador.Transportador.Email ?? string.Empty);
            }

            if (cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento?.AtivarRegraParaOfertarCarga ?? false)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado = repositorioCargaJanelaCarregamentoTransportador.BuscarPorValorFreteTabelaCalculado(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador proximaCargaJanelaCarregamentoTransportador = listaCargaJanelaCarregamentoTransportadorValorFreteTabelaCalculado.Find(o => o.Ordem == (cargaJanelaCarregamentoTransportadorReferencia.Ordem + 1));

                if (proximaCargaJanelaCarregamentoTransportador != null)
                {
                    proximaCargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgAceite;
                    proximaCargaJanelaCarregamentoTransportador.Bloqueada = false;

                    repositorioCargaJanelaCarregamentoTransportador.Atualizar(proximaCargaJanelaCarregamentoTransportador);
                    servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, tipoServicoMultisoftware);
                }
            }
            else if (
                (cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento?.LiberarCargaAutomaticamenteParaTransportadoras ?? false) &&
                cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento?.TipoTransportador == TipoTransportadorCentroCarregamento.PorPrioridadeDeRota
            )
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                bool existePedidoInLand = repositorioCargaPedido.ExisteInLand(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga?.Codigo ?? 0);
                int codigoTipoCarga = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CargaBase.TipoDeCarga?.Codigo ?? 0;
                bool bloquearLiberacaoAutomaticaParaTransportadoresPorTipoCarga = (
                    (cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento.TiposCargaBloquearLiberacaoAutomaticaParaTransportadoras?.Any(o => o.Codigo == codigoTipoCarga) ?? false) ||
                    ((cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CargaBase.TipoDeCarga?.BloquearLiberacaoParaTransportadores ?? false) && !existePedidoInLand)
                );
                bool bloquearLiberacaoParaTransportadoresPorTipoCondicaoPagamento = (
                    cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CargaBase.IsCarga() &&
                    cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga.TipoCondicaoPagamento.HasValue &&
                    cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga.TipoCondicaoPagamento.Value == TipoCondicaoPagamento.FOB &&
                    !(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga.TipoOperacao?.ConfiguracaoJanelaCarregamento?.PermitirLiberarCargaComTipoCondicaoPagamentoFOBParaTransportadores ?? false)
                );

                if (!bloquearLiberacaoParaTransportadoresPorTipoCondicaoPagamento && (!bloquearLiberacaoAutomaticaParaTransportadoresPorTipoCarga || cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.DataLiberacaoShare.HasValue))
                {
                    DisponibilizarParaTransportadorPrioritarioPorRota(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, tipoServicoMultisoftware);
                    servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, tipoServicoMultisoftware);
                }
            }
            else if (configuracaoEmbarcador.DisponibilizarCargaAutomaticamenteParaTransportadorComMenorValorFreteTabela)
            {
                DisponibilizarParaTransportadorPorMenorValorFreteTabelaCalculado(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento);
                servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, tipoServicoMultisoftware);
            }
            else if (cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento?.RetornarJanelaCarregamentoParaAgLiberacaoParaTransportadoresAposRejeicaoDoTransportador ?? false)
            {
                servicoCargaJanelaCarregamento.AlterarSituacao(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores);
            }

            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaJanelaCarregamentoTransportadorReferencia, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.RejeitouCarga, _unitOfWork);

            System.Text.StringBuilder mensagemNotificacao = new System.Text.StringBuilder();

            string numeroCarga = servicoCarga.ObterNumeroCarga(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga, configuracaoEmbarcador);
            string dataCarregamento = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.DataCarregamentoProgramada != null ? cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.DataCarregamentoProgramada.ToDateTimeString() : string.Empty;
            string dataRejeicao = carga.DataAtualizacaoCarga != null ? carga.DataAtualizacaoCarga.Value.ToDateTimeString() : string.Empty;
            string rota = carga.Rota?.Descricao != null ? carga.Rota.Descricao : string.Empty;
            string pedido = string.Join(", ", cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga?.Pedidos.Select(o => o.Pedido.NumeroPedidoEmbarcador).ToList());

            mensagemNotificacao.Append($"{usuario.Empresa.RazaoSocial} rejeitou a carga {numeroCarga} de pedido número {pedido}.").AppendLine();
            mensagemNotificacao.Append($"Rota: {rota}.").AppendLine();
            mensagemNotificacao.Append($"Data Carregamento: {dataCarregamento}.").AppendLine();
            mensagemNotificacao.Append($"Data Rejeição: {dataRejeicao}.").AppendLine().AppendLine();
            mensagemNotificacao.Append($"Rota ofertada para outra transportadora!").AppendLine();
            mensagemNotificacao.Append($"Importante: Conforme contrato vigente, se o valor do frete da nova transportadora for maior poderá haver cobrança da diferença.");

            NotificarUsuarios(cargaJanelaCarregamentoTransportadorReferencia, usuariosNotificacao, mensagemNotificacao.ToString(), cliente, tipoServicoMultisoftware, _unitOfWork, adminStringConexao);
            NotificarEmails(cargaJanelaCarregamentoTransportadorReferencia, emailsNotificacao, mensagemNotificacao.ToString(), _unitOfWork);

            _unitOfWork.CommitChanges();

            cargaJanelaCarregamentoTransportadorReferencia = repositorioCargaJanelaCarregamentoTransportador.BuscarPorCodigo(cargaJanelaCarregamentoTransportadorReferencia.Codigo);

            InformarCargaAtualizadaEmbarcador(carga.Codigo, cliente, adminStringConexao);
        }

        public void RejeitarCargaPorRotaOfertaPrioritaria(int codigoCarga, string motivoRejeicaoCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string adminStringConexao)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia = (from o in cargasJanelaCarregamentoTransportador where o.CargaJanelaCarregamento.Carga.Codigo == codigoCarga select o).FirstOrDefault();

            if (cargaJanelaCarregamentoTransportadorReferencia == null)
                throw new ServicoException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaNaoVinculadaAoTransportador);

            if ((cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.AgAceite) && (cargaJanelaCarregamentoTransportadorReferencia.Situacao != SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao))
                throw new ServicoException(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ASituacaoDaCargaNaoPermiteRejeicao);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga;

            if (!servicoCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware))
                throw new ServicoException((string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AAtualSituacaoDaCargaNaoPermiteRejeicao, carga.DescricaoSituacaoCarga)));

            if (cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.CentroCarregamento.TipoTransportador == TipoTransportadorCentroCarregamento.PorPrioridadeDeRota)
                return;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            _unitOfWork.Start();

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorRejeitada = cargasJanelaCarregamentoTransportador.FirstOrDefault(o => o.Situacao == SituacaoCargaJanelaCarregamentoTransportador.Rejeitada);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCarga(carga.Codigo);

            cargaJanelaCarregamentoTransportadorRejeitada.HorarioLimiteConfirmarCarga = null;
            cargaJanelaCarregamentoTransportadorRejeitada.Situacao = SituacaoCargaJanelaCarregamentoTransportador.Rejeitada;
            cargaJanelaCarregamentoTransportadorRejeitada.NumeroRejeicoesManuais += 1;

            if (cargaJanelaCarregamentoTransportadorRejeitada.CargaJanelaCarregamento.CentroCarregamento?.ExigirTransportadorInformarMotivoAoRejeitarCarga ?? false)
            {
                cargaJanelaCarregamentoTransportadorRejeitada.MotivoRejeicaoCarga = motivoRejeicaoCarga;
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaJanelaCarregamentoTransportadorRejeitada.CargaJanelaCarregamento.CargaBase, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRejeitouCargaPeloMotivo, cargaJanelaCarregamentoTransportadorRejeitada.Transportador.NomeFantasia, cargaJanelaCarregamentoTransportadorRejeitada.MotivoRejeicaoCarga)), _unitOfWork);
            }

            carga.DataAtualizacaoCarga = DateTime.Now;
            carga.Empresa = null;
            carga.RejeitadaPeloTransportador = true;

            if (agendamento != null && agendamento.Transportador != null)
            {
                string nomeTransportador = $"({agendamento.Transportador.CNPJ_Formatado}) {agendamento.Transportador.Descricao}";
                agendamento.Transportador = null;
                agendamento.EtapaAgendamentoColeta = (configuracaoAgendamentoColeta.RemoverEtapaAgendamentoAgendamentoColeta || (carga.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.RemoverEtapaAgendamentoDoAgendamentoColeta ?? false)) ? EtapaAgendamentoColeta.NFe : EtapaAgendamentoColeta.DadosTransporte;
                repAgendamentoColeta.Atualizar(agendamento);

                if (agendamento.Remetente != null && configuracaoEmbarcador.NotificarCargaAgConfirmacaoTransportador)
                    NotificarCargaRejeitadaPorTransportador(agendamento, nomeTransportador);

                Servicos.Auditoria.Auditoria.Auditar(auditado, agendamento, null, (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRejeitouaCarga, nomeTransportador)), _unitOfWork);
            }

            repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportadorRejeitada);
            SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportadorRejeitada, Localization.Resources.Logistica.JanelaCarregamentoTransportador.TransportadorRejeitouCarga, usuario);
            repositorioCarga.Atualizar(carga);
            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaJanelaCarregamentoTransportadorRejeitada, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.RejeitouCarga, _unitOfWork);

            _unitOfWork.CommitChanges();

            InformarCargaAtualizadaEmbarcador(carga.Codigo, cliente, adminStringConexao);
        }

        public void NotificaUsuarios(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia, List<Dominio.Entidades.Usuario> usuariosNotificacao, string mensagemNotificacao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao)
        {
            NotificarUsuarios(cargaJanelaCarregamentoTransportadorReferencia, usuariosNotificacao, mensagemNotificacao.ToString(), cliente, tipoServicoMultisoftware, _unitOfWork, adminStringConexao);
        }

        public void NotificaEmails(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia, List<string> emailsNotificacao, string mensagemNotificacao, UnitOfWork unitOfWork)
        {
            NotificarEmails(cargaJanelaCarregamentoTransportadorReferencia, emailsNotificacao, mensagemNotificacao.ToString(), unitOfWork);
        }

        public void InformaCargaAtualizadaEmbarcador(int codigoCarga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, string adminStringConexao)
        {
            InformarCargaAtualizadaEmbarcador(codigoCarga, cliente, adminStringConexao);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> ObterTransportadores(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repCargaJanelaTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> transportadores = repCargaJanelaTransportador.BuscarPorCargaJanelaCarregamentoDisponivel(cargaJanelaCarregamento.Codigo);
            return transportadores;
        }


        public void InformarAceiteCargasTransportador(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportadorReferencia, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador> cargasJanelaCarregamentoTransportador, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unitOfWork, configuracaoEmbarcador);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unitOfWork, configuracaoEmbarcador, null);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador repositorioCargaJanelaCarregamentoTransportador = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork, configuracaoEmbarcador);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta servicoCargaJanelaCarregamentoTransportadorConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorConsulta(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta servicoCargaJanelaCarregamentoTransportadorTerceiroConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiroConsulta(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);


            List<Dominio.Entidades.Usuario> usuariosNotificacao = new List<Dominio.Entidades.Usuario>();
            List<string> emailsNotificacao = new List<string>();
            foreach (var cargaJanelaCarregamentoTransportador in cargasJanelaCarregamentoTransportador)
            {
                bool situacaoCargaJanelaCarregamentoTransportadadorPermiteEnviarEmail = cargaJanelaCarregamentoTransportador.Situacao.PermitirEnviarEmailCargaDisponibilizada();

                cargaJanelaCarregamentoTransportador.HorarioLimiteConfirmarCarga = null;
                cargaJanelaCarregamentoTransportador.Situacao = SituacaoCargaJanelaCarregamentoTransportador.AgConfirmacao;

                if (situacaoCargaJanelaCarregamentoTransportadadorPermiteEnviarEmail)
                {
                    SalvarHistoricoAlteracao(cargaJanelaCarregamentoTransportador, Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaConfirmadaPeloTransportador, usuario);
                    servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaDisponibilizadaParaTransportador(cargaJanelaCarregamentoTransportador);
                }

                repositorioCargaJanelaCarregamentoTransportador.Atualizar(cargaJanelaCarregamentoTransportador);
                servicoCargaJanelaCarregamento.AtualizarSituacao(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, tipoServicoMultisoftware);

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.CentroCarregamento;
                bool shouldNotificarPorEmail = centroCarregamento?.EnviarNotificacoesPorEmail ?? false;

                List<Dominio.Entidades.Usuario> usuariosNotificacaoCarregamento = centroCarregamento?.UsuariosNotificacao.ToList() ?? new List<Dominio.Entidades.Usuario>();
                List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoEmail> emailsCentroCarregamento = shouldNotificarPorEmail ? centroCarregamento?.Emails.ToList() : null;

                if (usuariosNotificacaoCarregamento.Count > 0)
                    usuariosNotificacao.AddRange(usuariosNotificacaoCarregamento);
                else if (cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Operador != null)
                    usuariosNotificacao.Add(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Operador);

                if (emailsCentroCarregamento != null && emailsCentroCarregamento.Count > 0)
                    emailsNotificacao.AddRange(emailsCentroCarregamento.Select(o => o.Email));
            }

            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaJanelaCarregamentoTransportadorReferencia, null, Localization.Resources.Logistica.JanelaCarregamentoTransportador.ConfirmouCarga, unitOfWork);

            string numeroCarga = servicoCarga.ObterNumeroCarga(cargaJanelaCarregamentoTransportadorReferencia.CargaJanelaCarregamento.Carga, configuracaoEmbarcador);
            string mensagemNotificacao = (string.Format(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ConfirmouCarga, empresa.RazaoSocial, numeroCarga));

            servicoCargaJanelaCarregamentoTransportador.NotificaUsuarios(cargaJanelaCarregamentoTransportadorReferencia, usuariosNotificacao, mensagemNotificacao, cliente, tipoServicoMultisoftware, adminStringConexao);
            servicoCargaJanelaCarregamentoTransportador.NotificaEmails(cargaJanelaCarregamentoTransportadorReferencia, emailsNotificacao, mensagemNotificacao, unitOfWork);

        }


        #endregion Métodos Públicos
    }
}