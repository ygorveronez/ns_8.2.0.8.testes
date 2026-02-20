using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;
using AdminMultisoftware.Dominio.Enumeradores;

namespace Servicos.Embarcador.Frotas
{
    public class PlanejamentoFrotaMes
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public PlanejamentoFrotaMes(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void EnviarEmailParaTransportadoresSobreADisponibilidadeDaFrota(DateTime dataBaseGeracaoSugestaoFrota, List<string> listaEmails, List<string> listaPlacas)
        {
            if (listaEmails.Count == 0)
            {
                Log.TratarErro("Sem email disponives para envio da lista dos veiculos");
                return;
            }

            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);
            string assunto = $"Lista mensal ref. ao mês {dataBaseGeracaoSugestaoFrota.Month} está disponível para verificações, alterações e confirmação.";
            StringBuilder mensagem = new StringBuilder();

            mensagem.Append("<p>Prezado cliente. ").Append("<br />");
            mensagem.Append("Segue dados da lista dos veiculos:").Append("<br />");

            foreach (string placaVeiculo in listaPlacas)
                mensagem.Append(placaVeiculo).Append("<br />");

            mensagem.Append("</p><br />");
            mensagem.Append("Favor não responder este e-mail.<br />");

            foreach (string email in listaEmails)
                servicoEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, email, "", "", assunto, mensagem.ToString(), string.Empty, null, "", true, string.Empty, 0, _unitOfWork, 0, false);
        }

        private void GerarSugestaoFrota(List<Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada> listaConfiguracaoFrota, DateTime dataHistoricoInicial, DateTime dataHistoricoFinal)
        {
            if (listaConfiguracaoFrota.Count == 0)
                return;

            Repositorio.Embarcador.Frotas.PlanejamentoFrotaMes repositorioPlanejamentoFrota = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMes(_unitOfWork);
            Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo repositorioPlanejamentoFrotaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo(_unitOfWork);
            DateTime dataGeracaoSugestao = dataHistoricoFinal.AddMonths(1);
            List<Dominio.ObjetosDeValor.Embarcador.Frotas.CargaParaPlanejamentoDeFrota> cargasMesAnterior = repositorioPlanejamentoFrota.ObterCarregamentosDoMes(dataHistoricoInicial, dataHistoricoFinal);
            HashSet<string> placasCadastradas = new HashSet<string>();
            List<string> listaPlacas = new List<string>();
            List<string> listaEmails = new List<string>();
            bool enviarEmail = true;

#if DEBUG
            enviarEmail = false;
#endif

            List<(int CodigoFilial, string Placa, int QuantidadeCargas)> cargasMesAnteriorPorPlacaEFilial = cargasMesAnterior
                .GroupBy(carga => new
                {
                    Placa = carga.VeiculoPlaca,
                    Filial = carga.FilialCodigo
                })
                .Select(agrupamentoCarga => ValueTuple.Create(agrupamentoCarga.Key.Filial, agrupamentoCarga.Key.Placa, agrupamentoCarga.Count()))
                .ToList();

            //precisa remover as filiais q nao estao nas configuracoes porque se uma delas tiver mais cargas que outras entao as outras nao vão ter prioridade na disponibilização do veiculo por terem uma quantidade menor de cargas
            List<int> codigosFiliais = listaConfiguracaoFrota.SelectMany(configuracao => configuracao.Filiais).Select(filial => filial.Codigo).Distinct().ToList();
            HashSet<(long CodigoPorModelo, string DescricaoPorModelo)> planejamentosAdicionadosPorModelo = new HashSet<(long CodigoPorModelo, string DescricaoPorModelo)>();

            cargasMesAnteriorPorPlacaEFilial.RemoveAll(carga => !codigosFiliais.Contains(carga.CodigoFilial));

            foreach (Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada configuracaoFrota in listaConfiguracaoFrota)
            {
                List<int> codigosModelosVeiculares = configuracaoFrota.ModelosVeicularesCarga.Select(modelo => modelo.Codigo).ToList();
                List<int> codigosTiposOperacao = configuracaoFrota.TipoOperacoes.Select(tipoOperacao => tipoOperacao.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in configuracaoFrota.Filiais)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Frotas.CargaParaPlanejamentoDeFrota> cargasMesAnteriorPorFilial = cargasMesAnterior
                        .Where(carga => carga.FilialCodigo == filial.Codigo)
                        .Where(carga => codigosModelosVeiculares.Contains(carga.ModeloVeicularCodigo))
                        .Where(carga => codigosTiposOperacao.Contains(carga.TipoOperacaoCodigo))
                        .DistinctBy(carga => carga.VeiculoCodigo)
                        .ToList();

                    if (cargasMesAnteriorPorFilial.Count == 0)
                        continue;

                    if (repositorioPlanejamentoFrota.VerificarSugestaoFrotaGerada(filial.Codigo, dataGeracaoSugestao))
                        continue;

                    Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes planejamentoDoMes = new Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes()
                    {
                        Data = dataGeracaoSugestao,
                        Filial = filial
                    };

                    repositorioPlanejamentoFrota.Inserir(planejamentoDoMes, _auditado);

                    foreach (Dominio.ObjetosDeValor.Embarcador.Frotas.CargaParaPlanejamentoDeFrota cargaMesAnterior in cargasMesAnteriorPorFilial)
                    {
                        int totalCargasDaPlacaNaFilial = cargasMesAnteriorPorPlacaEFilial.Where(o => o.CodigoFilial == filial.Codigo && o.Placa == cargaMesAnterior.VeiculoPlaca).Select(o => o.QuantidadeCargas).FirstOrDefault();
                        int maiorQuantidadeDeCargasDaPlaca = cargasMesAnteriorPorPlacaEFilial.Where(o => o.Placa == cargaMesAnterior.VeiculoPlaca).Select(o => o.QuantidadeCargas).Max();

                        //se existe outra filial com mais cargas pra esse veiculo, então deixa cadastrar o veiculo pra filial com mais cargas
                        if (maiorQuantidadeDeCargasDaPlaca > totalCargasDaPlacaNaFilial)
                            continue;

                        if (!placasCadastradas.Add(cargaMesAnterior.VeiculoPlaca))
                            continue;
                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = configuracaoFrota.ModelosVeicularesCarga.Where(modelo => modelo.Codigo == cargaMesAnterior.ModeloVeicularCodigo).FirstOrDefault();
                        Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo veiculo = new Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo
                        {
                            GeradoPeloSistema = true,
                            PlacaVeiculo = cargaMesAnterior.VeiculoPlaca,
                            TipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao { Codigo = cargaMesAnterior.TipoOperacaoCodigo },
                            ModeloVeicular = modeloVeicular,
                            Veiculo = new Dominio.Entidades.Veiculo { Codigo = cargaMesAnterior.VeiculoCodigo },
                            PlanejamentoFrotaMes = planejamentoDoMes,
                            Situacao = SituacaoPlanejamentoFrota.EmAnaliseTransportador,
                            RespostaDoTransportador = RespostaTransportadorPlanejamentoFrota.Pendente
                        };

                        repositorioPlanejamentoFrotaVeiculo.Inserir(veiculo, _auditado);
                        listaPlacas.Add(veiculo.PlacaVeiculo);
                        planejamentosAdicionadosPorModelo.Add(ValueTuple.Create(veiculo.CodigoPorModelo, veiculo.DescricaoPorModelo));

                        if (!listaEmails.Any(e => e == cargaMesAnterior.EmailTransportador))
                            listaEmails.Add(cargaMesAnterior.EmailTransportador);
                    }
                }
            }

            if (_auditado != null)
            {
                foreach ((long CodigoPorModelo, string DescricaoPorModelo) planejamento in planejamentosAdicionadosPorModelo)
                    Auditoria.Auditoria.AuditarSemEntidade(_auditado, codigoEntidade: planejamento.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: planejamento.DescricaoPorModelo, descricaoAcao: "Adicionada sugestão mensal", _unitOfWork);
            }

            if (enviarEmail)
                EnviarEmailParaTransportadoresSobreADisponibilidadeDaFrota(dataHistoricoFinal, listaEmails, listaPlacas);
        }

        private DateTime ObterDataHistoricoFinal(int ano, int mes)
        {
            int diasNoMes = DateTime.DaysInMonth(ano, mes);
            DateTime dataHistoricoFinal = new DateTime(ano, mes, diasNoMes - 2);

            if (dataHistoricoFinal.DayOfWeek == DayOfWeek.Sunday)
                dataHistoricoFinal = dataHistoricoFinal.AddDays(-2);

            if (dataHistoricoFinal.DayOfWeek == DayOfWeek.Saturday)
                dataHistoricoFinal = dataHistoricoFinal.AddDays(-1);

            return dataHistoricoFinal;
        }

        private void ValidarDatasHistorico(DateTime dataHistoricoInicial, DateTime dataHistoricoFinal)
        {
            DateTime dataHistoricoMaxima = DateTime.Now.FirstDayOfMonth();
            DateTime dataHistoricoMinima = dataHistoricoMaxima.AddYears(-1);

            if (dataHistoricoInicial < dataHistoricoMinima)
                throw new ServicoException($"Não é possível gerar a lista mensal com data inferior a {dataHistoricoMinima.AddMonths(1):MM/yyyy}");

            if (dataHistoricoInicial > dataHistoricoMaxima)
                throw new ServicoException($"Não é possível gerar a lista mensal com data superior a {dataHistoricoMaxima.AddMonths(1):MM/yyyy}");
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<(string Placa, string MensagemRetorno, bool Sucesso)> AdicionarSugestaoFrota(Dominio.ObjetosDeValor.Embarcador.Frotas.PlanejamentoFrotaMesAdicionar planejamentoFrotaMesAdicionar, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (planejamentoFrotaMesAdicionar.CodigoFilial <= 0)
                throw new ServicoException("A filial deve ser informada");

            if (planejamentoFrotaMesAdicionar.CodigoModeloVeicularCarga <= 0)
                throw new ServicoException("O modelo veicular deve ser informado");

            if (planejamentoFrotaMesAdicionar.CodigosVeiculos.Count == 0)
                throw new ServicoException("Um ou mais veículos devem ser informados");

            DateTime dataHistoricoInicial = new DateTime(planejamentoFrotaMesAdicionar.Ano, planejamentoFrotaMesAdicionar.Mes, day: 1).AddMonths(-1);
            DateTime dataHistoricoFinal = ObterDataHistoricoFinal(dataHistoricoInicial.Year, dataHistoricoInicial.Month);

            ValidarDatasHistorico(dataHistoricoInicial, dataHistoricoFinal);

            DateTime dataGeracaoSugestao = dataHistoricoFinal.AddMonths(1);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Frotas.PlanejamentoFrotaMes repositorioPlanejamentoFrota = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMes(_unitOfWork);
            Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo repositorioPlanejamentoFrotaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes planejamentoDoMes = repositorioPlanejamentoFrota.BuscarSugestaoFrotaGerada(planejamentoFrotaMesAdicionar.CodigoFilial, dataGeracaoSugestao);
            bool adicionarPlanejamentoDoMes = (planejamentoDoMes == null);
            List<Dominio.Entidades.Veiculo> veiculos = repositorioVeiculo.BuscarPorCodigo(planejamentoFrotaMesAdicionar.CodigosVeiculos);
            List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculosExistentes;
            List<string> placasExistentesEmOutrasFiliais = repositorioPlanejamentoFrotaVeiculo.VerificarPlacasJaPlanejadasEmOutraFilial(veiculos.Select(x => x.Placa).ToList(), planejamentoFrotaMesAdicionar.CodigoFilial, dataGeracaoSugestao.FirstDayOfMonth());
            List<(string Placa, string MensagemRetorno, bool Sucesso)> retornos = new List<(string Placa, string MensagemRetorno, bool Sucesso)>();
            List<(long CodigoPorModelo, string DescricaoPorModelo, string Placa)> veiculosAdicionadosParaAuditar = new List<(long CodigoPorModelo, string DescricaoPorModelo, string Placa)>();

            if (adicionarPlanejamentoDoMes)
            {
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(planejamentoFrotaMesAdicionar.CodigoFilial);

                planejamentoDoMes = new Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes()
                {
                    Data = dataGeracaoSugestao,
                    Filial = filial
                };

                repositorioPlanejamentoFrota.Inserir(planejamentoDoMes, _auditado);

                veiculosExistentes = new List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo>();
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo filtroPesquisaVeiculosExistentes = new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo()
                {
                    CodigoFilial = planejamentoFrotaMesAdicionar.CodigoFilial,
                    CodigoModeloVeicularCarga = planejamentoFrotaMesAdicionar.CodigoModeloVeicularCarga,
                    CodigoPlanejamentoFrotaMes = planejamentoDoMes.Codigo
                };

                veiculosExistentes = repositorioPlanejamentoFrotaVeiculo.ObterPlanejamentosDoMes(filtroPesquisaVeiculosExistentes);
            }

            if (veiculosExistentes.Any(planejamentoVeiculo => planejamentoVeiculo.Situacao == SituacaoPlanejamentoFrota.ListaDiariaGerada))
                throw new ServicoException("Não é possivel fazer alterações. A lista diária já foi criada.");

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                if (veiculosExistentes.Any(planejamentoVeiculo => planejamentoVeiculo.PlacaVeiculo == veiculo.Placa))
                {
                    retornos.Add(ValueTuple.Create(veiculo.Placa, "O veículo já está cadastrado na filial", false));
                    continue;
                }

                if (placasExistentesEmOutrasFiliais.Contains(veiculo.Placa))
                {
                    retornos.Add(ValueTuple.Create(veiculo.Placa, "O veículo já está cadastrado em outra filial", false));
                    continue;
                }

                if (veiculo.ModeloVeicularCarga?.Codigo != planejamentoFrotaMesAdicionar.CodigoModeloVeicularCarga)
                {
                    retornos.Add(ValueTuple.Create(veiculo.Placa, "O veículo possui um modelo veicular diferente do informado", false));
                    continue;
                }

                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo planejamentoDoMesVeiculo = new Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo()
                {
                    IncluidoPeloEmbarcador = tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador,
                    IncluidoPeloTransportador = tipoServicoMultisoftware != TipoServicoMultisoftware.MultiEmbarcador,
                    Veiculo = veiculo,
                    ModeloVeicular = veiculo.ModeloVeicularCarga,
                    PlanejamentoFrotaMes = planejamentoDoMes,
                    PlacaVeiculo = veiculo.Placa,
                    Situacao = SituacaoPlanejamentoFrota.EmAnaliseTransportador,
                    RespostaDoTransportador = RespostaTransportadorPlanejamentoFrota.Pendente
                };

                repositorioPlanejamentoFrotaVeiculo.Inserir(planejamentoDoMesVeiculo, _auditado);
                retornos.Add(ValueTuple.Create(veiculo.Placa, "O veículo foi cadastrado com sucesso", true));
                veiculosAdicionadosParaAuditar.Add(ValueTuple.Create(planejamentoDoMesVeiculo.CodigoPorModelo, planejamentoDoMesVeiculo.DescricaoPorModelo, planejamentoDoMesVeiculo.PlacaVeiculo));
            }

            if (_auditado != null)
            {
                if (adicionarPlanejamentoDoMes)
                {
                    List<long> codigosPorModeloAdicionados = veiculosAdicionadosParaAuditar.Select(veiculo => veiculo.CodigoPorModelo).Distinct().ToList();

                    foreach (long codigoPorModelo in codigosPorModeloAdicionados)
                    {
                        (long CodigoPorModelo, string DescricaoPorModelo, string Placa) veiculo = veiculosAdicionadosParaAuditar.Where(veiculoAdicionado => veiculoAdicionado.CodigoPorModelo == codigoPorModelo).FirstOrDefault();

                        Auditoria.Auditoria.AuditarSemEntidade(_auditado, codigoEntidade: veiculo.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: veiculo.DescricaoPorModelo, descricaoAcao: "Adicionada sugestão mensal", _unitOfWork);
                    }
                }
                else
                {
                    foreach ((long CodigoPorModelo, string DescricaoPorModelo, string Placa) veiculo in veiculosAdicionadosParaAuditar)
                        Auditoria.Auditoria.AuditarSemEntidade(_auditado, codigoEntidade: veiculo.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: veiculo.DescricaoPorModelo, descricaoAcao: $"Veículo {veiculo.Placa} adicionado", _unitOfWork);
                }
            }

            return retornos;
        }

        public void GerarSugestaoFrota(int codigoGeracaoFrotaAutomatizada, int ano, int mes)
        {
            DateTime dataHistoricoInicial = new DateTime(ano, mes, day: 1).AddMonths(-1);
            DateTime dataHistoricoFinal = ObterDataHistoricoFinal(dataHistoricoInicial.Year, dataHistoricoInicial.Month);

            ValidarDatasHistorico(dataHistoricoInicial, dataHistoricoFinal);

            Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada repositorioConfiguracaoFrota = new Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada(_unitOfWork);
            Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada configuracaoFrota = repositorioConfiguracaoFrota.BuscarPorCodigo(codigoGeracaoFrotaAutomatizada);

            if (configuracaoFrota == null)
                throw new ServicoException("Não foi possível encontrar a configuração para geração automatizada");

            GerarSugestaoFrota(new List<Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada>() { configuracaoFrota }, dataHistoricoInicial, dataHistoricoFinal);
        }

        public void GerarSugestaoFrotaAutomaticamente()
        {
            DateTime dataAtual = DateTime.Now;
            DateTime dataHistoricoInicial = dataAtual.FirstDayOfMonth();
            DateTime dataHistoricoFinal = ObterDataHistoricoFinal(dataHistoricoInicial.Year, dataHistoricoInicial.Month);

            if (dataAtual.Day != dataHistoricoFinal.Day)
                return;

            Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada repositorioConfiguracaoFrota = new Repositorio.Embarcador.Frotas.GeracaoFrotaAutomatizada(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frotas.GeracaoFrotaAutomatizada> listaConfiguracaoFrota = repositorioConfiguracaoFrota.BuscarTodos();

            GerarSugestaoFrota(listaConfiguracaoFrota, dataHistoricoInicial, dataHistoricoFinal);
        }

        #endregion Métodos Públicos
    }
}
