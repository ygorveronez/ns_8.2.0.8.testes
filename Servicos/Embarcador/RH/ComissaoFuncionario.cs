using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace Servicos.Embarcador.RH
{
    public class ComissaoFuncionario : ServicoBase
    {
        public ComissaoFuncionario(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken = default) : base(unitOfWork, cancellationToken)
        {
        }


        public bool GerarComissaoMotoristas(int codigoMotorista, int codigoCargo, int codigoComissao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao, decimal? percentualComissaoMedia = null, bool transacaoAtiva = false, Repositorio.UnitOfWork unitOfWork = null)
        {
            if(unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao);

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Notificacao.Notificacao(unitOfWork.StringConexao, cliente, tipoServicoMultisoftware, adminStringConexao);

            Repositorio.Embarcador.RH.TabelaMediaModeloPeso repTabelaMediaModeloPeso = new Repositorio.Embarcador.RH.TabelaMediaModeloPeso(unitOfWork);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unitOfWork);
            Repositorio.Embarcador.RH.TabelaPremioProdutividade repTabelaPremioProdutividade = new Repositorio.Embarcador.RH.TabelaPremioProdutividade(unitOfWork);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento repComissaoFuncionarioMotoristaAbastecimento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repConfiguracaoMotorista.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Usuario> motoristas = null;
            if (configuracaoTMS.UtilizarComissaoPorCargo)
                motoristas = repUsuario.BuscarMotoristasProprios(null, true);
            else
                motoristas = repUsuario.BuscarMotoristasProprios(null);

            if (codigoMotorista > 0)
                motoristas = motoristas.Where(o => o.Codigo == codigoMotorista).ToList();

            if (codigoCargo > 0)
                motoristas = motoristas.Where(o => o.CargoMotorista.Codigo == codigoCargo).ToList();

            Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(codigoComissao);

            if (configuracaoMotorista.HabilitarUsoCentroResultadoComissaoMotorista && comissaoFuncionario.CentroResultado != null)
                motoristas = motoristas.Where(o => o.CentroResultado?.Codigo == comissaoFuncionario.CentroResultado.Codigo).ToList();

            comissaoFuncionario.TotalFuncionarios = motoristas.Count();
            List<int> codigosMotoristas = new List<int>();

            if (motoristas != null && motoristas.Count > 0)
                codigosMotoristas = motoristas.Select(c => c.Codigo).Distinct().ToList();

            repComissaoFuncionario.Atualizar(comissaoFuncionario);

            int numeroMotoristasProcessados = 0;
            bool falha = false;
            foreach (int codigoMot in codigosMotoristas)
            {
                try
                {
                    if(!transacaoAtiva)
                        unitOfWork.FlushAndClear();

                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigoMot);

                    Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista = repComissaoFuncionarioMotorista.BuscarPorComissaoEMotorista(motorista.Codigo, comissaoFuncionario.Codigo);

                    if (comissaoFuncionarioMotorista == null)
                    {
                        if (!transacaoAtiva)
                            unitOfWork.Start();

                        List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = null;
                        if (configuracaoTMS.UtilizarComissaoPorCargo)
                            listaCarga = repCarga.BuscarCargasParaComissaoMotoristaPorCargo(motorista, comissaoFuncionario.DataInicio, comissaoFuncionario.DataFim);
                        else
                            listaCarga = repCarga.BuscarCargasParaComissaoMotorista(motorista, comissaoFuncionario.DataInicio, comissaoFuncionario.DataFim);

                        List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = null;
                        if (!configuracaoTMS.UtilizarComissaoPorCargo)
                            ocorrencias = repCargaOcorrencia.BuscarOcorrenciasParaComissaoMotorista(motorista, comissaoFuncionario.DataInicio, comissaoFuncionario.DataFim);
                        else
                            ocorrencias = new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

                        List<int> codigosCargas = null;
                        if (listaCarga != null && listaCarga.Count > 0)
                            codigosCargas = listaCarga.Select(c => c.Codigo).Distinct().ToList();

                        comissaoFuncionarioMotorista = new Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista();
                        comissaoFuncionarioMotorista.ComissaoFuncionario = comissaoFuncionario;
                        comissaoFuncionarioMotorista.Motorista = motorista;
                        comissaoFuncionarioMotorista.NumeroDiasEmViagem = comissaoFuncionario.NumeroDiasEmViagem;
                        comissaoFuncionarioMotorista.PercentualComissao = comissaoFuncionario.PercentualComissao;
                        comissaoFuncionarioMotorista.ValorNormativo = comissaoFuncionario.NumeroDiasEmViagem * comissaoFuncionario.ValorDiaria;
                        comissaoFuncionarioMotorista.AtingiuMedia = true;
                        comissaoFuncionarioMotorista.GerarComissao = true;
                        comissaoFuncionarioMotorista.PossuiDuasFrotas = false;

                        if (percentualComissaoMedia != null)
                            comissaoFuncionarioMotorista.PercentualAtingirMedia = percentualComissaoMedia.Value;
                        else
                            comissaoFuncionarioMotorista.PercentualAtingirMedia = null;

                        Dominio.Entidades.Veiculo primeiroVeiculoMotorista = null;

                        if (configuracaoTMS.UtilizarComissaoPorCargo)
                        {
                            comissaoFuncionarioMotorista.FaturamentoMinimo = motorista.CargoMotorista?.ValorFaturamento ?? 0m;
                            comissaoFuncionarioMotorista.CargoMotorista = motorista.CargoMotorista;
                            comissaoFuncionarioMotorista.ValorBonificacao = motorista.CargoMotorista?.ValorBonificacao ?? 0m;
                            comissaoFuncionarioMotorista.ValorBaseComissao = 0m;
                            comissaoFuncionarioMotorista.PercentualComissao = motorista.CargoMotorista != null && motorista.CargoMotorista.ComissaoPadrao > 0 ? motorista.CargoMotorista.ComissaoPadrao : configuracaoTMS.PercentualComissaoPadrao;
                            comissaoFuncionarioMotorista.PercentualMedia = motorista.CargoMotorista != null && motorista.CargoMotorista.MediaEquivalente > 0 ? motorista.CargoMotorista.MediaEquivalente : configuracaoTMS.PercentualMediaEquivalente;
                            comissaoFuncionarioMotorista.PercentualSinistro = motorista.CargoMotorista != null && motorista.CargoMotorista.SinistroEquivalente > 0 ? motorista.CargoMotorista.SinistroEquivalente : configuracaoTMS.PercentualEquivaleEquivalente;
                            comissaoFuncionarioMotorista.PercentualAdvertencia = motorista.CargoMotorista != null && motorista.CargoMotorista.AdvertenciaEquivalente > 0 ? motorista.CargoMotorista.AdvertenciaEquivalente : configuracaoTMS.PercentualAdvertenciaEquivalente;
                            comissaoFuncionarioMotorista.AtingiuMedia = true;
                            comissaoFuncionarioMotorista.NaoHouveSinitro = !repInfracao.ContemSinistroAdvertenciaMotorista(motorista.Codigo, comissaoFuncionario.DataInicio, comissaoFuncionario.DataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoInfracaoTransito.Sinistro);
                            comissaoFuncionarioMotorista.NaoHouveAdvertencia = !repInfracao.ContemSinistroAdvertenciaMotorista(motorista.Codigo, comissaoFuncionario.DataInicio, comissaoFuncionario.DataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoInfracaoTransito.Advertencia);
                            comissaoFuncionarioMotorista.ContemDeslocamentoVazio = codigosCargas != null && codigosCargas.Count > 0 ? repCarga.ContemDeslocamentoVazio(codigosCargas) : false;

                            //comissaoFuncionarioMotorista.ValorComissao = comissaoFuncionarioMotorista.ValorBonificacao;
                            if (codigosCargas != null && codigosCargas.Count > 0)
                            {
                                primeiroVeiculoMotorista = repCarga.BuscarPrimeiroVeiculo(codigosCargas);
                                if (primeiroVeiculoMotorista != null)
                                {
                                    comissaoFuncionarioMotorista.PossuiDuasFrotas = repCarga.PossuiModelosVeiculosDistintoso(codigosCargas);
                                    comissaoFuncionarioMotorista.PrimeiraFrota = primeiroVeiculoMotorista.NumeroFrota;

                                    comissaoFuncionarioMotorista.MediaPeso = repPedidoXMLNotaFiscal.BuscarMediaPesoPorCarga(codigosCargas);
                                    decimal totalMeiaIdeal = 0m;
                                    if (primeiroVeiculoMotorista.Modelo != null)
                                    {
                                        foreach (var codigoCarga in codigosCargas)
                                        {
                                            decimal pesoCarga = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(codigoCarga);
                                            if (repCarga.ContemDeslocamentoVazio(codigoCarga))
                                                totalMeiaIdeal += (primeiroVeiculoMotorista.Modelo?.MediaPadraoVazio ?? (primeiroVeiculoMotorista.Modelo?.MediaPadrao ?? 0m));
                                            else
                                                totalMeiaIdeal += repTabelaMediaModeloPeso.BuscarMediaPorModeloPeso(primeiroVeiculoMotorista.Modelo.Codigo, pesoCarga);
                                        }
                                        if (totalMeiaIdeal > 0)
                                            totalMeiaIdeal = totalMeiaIdeal / codigosCargas.Count;
                                    }

                                    if (primeiroVeiculoMotorista.Modelo != null && comissaoFuncionarioMotorista.MediaPeso > 0)
                                        comissaoFuncionarioMotorista.TabelaMediaModeloPeso = repTabelaMediaModeloPeso.BuscarPorModeloPeso(primeiroVeiculoMotorista.Modelo.Codigo, comissaoFuncionarioMotorista.MediaPeso);

                                    if (totalMeiaIdeal > 0)
                                        comissaoFuncionarioMotorista.MediaIdeal = Math.Round(totalMeiaIdeal, 2, MidpointRounding.AwayFromZero);
                                    else if (!comissaoFuncionarioMotorista.ContemDeslocamentoVazio && comissaoFuncionarioMotorista.TabelaMediaModeloPeso != null && comissaoFuncionarioMotorista.TabelaMediaModeloPeso.MediaIdeal > 0)
                                        comissaoFuncionarioMotorista.MediaIdeal = Math.Round(comissaoFuncionarioMotorista.TabelaMediaModeloPeso.MediaIdeal, 2, MidpointRounding.AwayFromZero);
                                    else
                                        comissaoFuncionarioMotorista.MediaIdeal = Math.Round(comissaoFuncionarioMotorista.ContemDeslocamentoVazio ? (primeiroVeiculoMotorista.Modelo?.MediaPadraoVazio ?? 0m) : (primeiroVeiculoMotorista.Modelo?.MediaPadrao ?? 0m), 2, MidpointRounding.AwayFromZero);

                                    comissaoFuncionarioMotorista.LitrosTotalAbastecimento = repAbastecimento.BuscarLitrosTotalAbastecimento(codigoMot, primeiroVeiculoMotorista.Codigo, comissaoFuncionario.DataInicio, comissaoFuncionario.DataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);
                                    comissaoFuncionarioMotorista.KMFinal = repAbastecimento.BuscarUltimoKMAbastecimentoPorData(primeiroVeiculoMotorista.Codigo, comissaoFuncionario.DataFim, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);
                                    Dominio.Entidades.Abastecimento primeiroAbastecimento = repAbastecimento.BuscarPrimeiroAbastecimento(codigoMot, primeiroVeiculoMotorista.Codigo, comissaoFuncionario.DataInicio, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);

                                    if (primeiroAbastecimento != null)
                                    {
                                        comissaoFuncionarioMotorista.KMInicial = repAbastecimento.BuscarPrimeiroKMAbastecimento(0, primeiroVeiculoMotorista.Codigo, primeiroAbastecimento.Data.Value, primeiroAbastecimento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);
                                        if (comissaoFuncionarioMotorista.KMInicial <= 0)
                                            comissaoFuncionarioMotorista.KMInicial = repAbastecimento.BuscarPrimeiroKMAbastecimento(0, primeiroVeiculoMotorista.Codigo, primeiroAbastecimento.Data.Value, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);

                                        comissaoFuncionarioMotorista.KMTotal = comissaoFuncionarioMotorista.KMFinal - comissaoFuncionarioMotorista.KMInicial;
                                        if (comissaoFuncionarioMotorista.KMTotal <= 0)
                                            comissaoFuncionarioMotorista.KMTotal = 0;
                                        if (comissaoFuncionarioMotorista.KMTotal > 0 && comissaoFuncionarioMotorista.LitrosTotalAbastecimento > 0)
                                        {
                                            comissaoFuncionarioMotorista.MediaFinal = Math.Round(comissaoFuncionarioMotorista.KMTotal / comissaoFuncionarioMotorista.LitrosTotalAbastecimento, 2, MidpointRounding.AwayFromZero);
                                            if (comissaoFuncionarioMotorista.MediaFinal > 0 && comissaoFuncionarioMotorista.MediaIdeal <= comissaoFuncionarioMotorista.MediaFinal)
                                                comissaoFuncionarioMotorista.AtingiuMedia = true;
                                        }
                                    }
                                }
                            }
                        }

                        repComissaoFuncionarioMotorista.Inserir(comissaoFuncionarioMotorista);

                        if (primeiroVeiculoMotorista != null && comissaoFuncionarioMotorista.KMInicial > 0 && comissaoFuncionarioMotorista.KMFinal > 0)
                        {
                            List<Dominio.Entidades.Abastecimento> abastecimentos = repAbastecimento.BuscarAbastecimentos(comissaoFuncionarioMotorista.KMInicial, comissaoFuncionarioMotorista.KMFinal, motorista.Codigo, primeiroVeiculoMotorista.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);
                            if (abastecimentos != null && abastecimentos.Count > 0)
                            {
                                foreach (var abastecimento in abastecimentos)
                                {
                                    if (abastecimento.Data.HasValue && abastecimento.Data.Value >= comissaoFuncionario.DataInicio)
                                    {
                                        Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento motoristaAbastecimento = new Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaAbastecimento()
                                        {
                                            Abastecimento = abastecimento,
                                            ComissaoFuncionarioMotorista = comissaoFuncionarioMotorista
                                        };
                                        repComissaoFuncionarioMotoristaAbastecimento.Inserir(motoristaAbastecimento);
                                    }
                                }
                            }
                        }

                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in listaCarga)
                        {
                            if (repComissaoFuncionarioMotoristaDocumento.ContemCargaDuplicada(carga.Codigo, comissaoFuncionarioMotorista.Codigo))
                                continue;
                            if (repComissaoFuncionarioMotoristaDocumento.ContemCargaEmOutraComissao(carga.Codigo, comissaoFuncionario.Codigo))
                                continue;

                            decimal percentualComissaoParcial = 100m;

                            Dominio.Entidades.Embarcador.RH.TabelaPremioProdutividade tabelaPremio = null;
                            if (carga.GrupoPessoaPrincipal != null && configuracaoTMS.UtilizarComissaoPorCargo)
                            {
                                tabelaPremio = repTabelaPremioProdutividade.BuscarPorGrupoEVigencia(carga.GrupoPessoaPrincipal.Codigo, DateTime.Now.Date);
                                if (tabelaPremio != null && tabelaPremio.Percentual > 0)
                                    percentualComissaoParcial = tabelaPremio.Percentual;
                                else
                                {
                                    tabelaPremio = repTabelaPremioProdutividade.BuscarSemGrupoEVigencia(DateTime.Now.Date);
                                    if (tabelaPremio != null)
                                        percentualComissaoParcial = tabelaPremio.Percentual;
                                }
                                if (percentualComissaoParcial == 100)
                                    percentualComissaoParcial = 0m;
                            }

                            if (carga.TipoOperacao?.GerarComissaoParcialMotorista ?? false)
                                percentualComissaoParcial = carga.TipoOperacao.PercentualComissaoParcialMotorista;

                            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponentesFrete.BuscarPorCargaSemComponenteCompoeFreteValor(carga.Codigo);

                            Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento comissaoFuncionarioMotoristaDocumento = new Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento();
                            comissaoFuncionarioMotoristaDocumento.Carga = carga;
                            comissaoFuncionarioMotoristaDocumento.Numero = carga.CodigoCargaEmbarcador;
                            comissaoFuncionarioMotoristaDocumento.DataEmissao = carga.DataFinalizacaoEmissao.HasValue ? carga.DataFinalizacaoEmissao.Value : carga.DataCriacaoCarga;
                            comissaoFuncionarioMotoristaDocumento.Veiculo = carga.Veiculo;
                            comissaoFuncionarioMotoristaDocumento.PercentualComissaoParcial = percentualComissaoParcial;

                            if (primeiroVeiculoMotorista != null)
                            {
                                comissaoFuncionarioMotoristaDocumento.PesoCarga = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(carga.Codigo);
                                if (repCarga.ContemDeslocamentoVazio(carga.Codigo))
                                    comissaoFuncionarioMotoristaDocumento.MediaIdeal = (primeiroVeiculoMotorista.Modelo?.MediaPadraoVazio ?? (primeiroVeiculoMotorista.Modelo?.MediaPadrao ?? 0m));
                                else
                                    comissaoFuncionarioMotoristaDocumento.MediaIdeal = repTabelaMediaModeloPeso.BuscarMediaPorModeloPeso(primeiroVeiculoMotorista.Modelo.Codigo, comissaoFuncionarioMotoristaDocumento.PesoCarga);
                            }

                            //if (comissaoFuncionarioMotoristaDocumento.PercentualComissaoParcial == 0m && configuracaoTMS.UtilizarComissaoPorCargo)
                            //    comissaoFuncionarioMotoristaDocumento.PercentualComissaoParcial = comissaoFuncionario.PercentualComissao;

                            if (configuracaoTMS.UtilizarComissaoPorCargo && (carga.TipoOperacao?.NaoPermitirGerarComissaoMotorista ?? false))
                            {
                                percentualComissaoParcial = 0m;
                                comissaoFuncionarioMotoristaDocumento.PercentualComissaoParcial = percentualComissaoParcial;
                            }

                            comissaoFuncionarioMotoristaDocumento.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                            if (carga.VeiculosVinculados != null)
                            {
                                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
                                {
                                    comissaoFuncionarioMotoristaDocumento.VeiculosVinculados.Add(reboque);
                                }
                            }

                            Dominio.Entidades.Embarcador.Cargas.CargaMotorista CargaMotorista = repCargaMotorista.BuscarPorCargaMotorista(carga.Codigo, motorista.Codigo);

                            comissaoFuncionarioMotoristaDocumento.PercentualExecucao = CargaMotorista?.PercentualExecucao ?? 100;

                            decimal valorFreteLiquido = (carga.ValorFreteLiquido * comissaoFuncionarioMotoristaDocumento.PercentualExecucao) / 100;

                            comissaoFuncionarioMotoristaDocumento.TipoDocumentoComissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoComissao.carga;
                            comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados = carga.DadosSumarizados;
                            comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista = comissaoFuncionarioMotorista;

                            decimal outrosValores = 0;
                            if (!configuracaoTMS.UtilizarComissaoPorCargo)
                                outrosValores = (from obj in cargaComponentesFrete where obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO && !obj.SomarComponenteFreteLiquido && !obj.DescontarComponenteFreteLiquido select obj.ValorComponente).Sum();
                            else
                                outrosValores = (from obj in cargaComponentesFrete where obj.DescricaoComponente == "GRIS" && !obj.SomarComponenteFreteLiquido && !obj.DescontarComponenteFreteLiquido select obj.ValorComponente).Sum();

                            comissaoFuncionarioMotoristaDocumento.OutrosValores = (outrosValores * comissaoFuncionarioMotoristaDocumento.PercentualExecucao) / 100;

                            comissaoFuncionarioMotorista.OutrosValores += comissaoFuncionarioMotoristaDocumento.OutrosValores;

                            if (configuracaoTMS.UtilizarComissaoPorCargo)
                                comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo = Math.Round(valorFreteLiquido, 2, MidpointRounding.AwayFromZero);
                            else
                                comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo = Math.Round(((valorFreteLiquido * (percentualComissaoParcial / 100)) * (comissaoFuncionario.PercentualBaseCalculoComissao / 100)), 2, MidpointRounding.AwayFromZero);
                            comissaoFuncionarioMotoristaDocumento.ValoBaseCalculoOriginal = comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;
                            comissaoFuncionarioMotorista.ValoBaseCalculo += comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;

                            comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido = valorFreteLiquido; //carga.ValorFreteLiquido;
                            comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal = comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;
                            comissaoFuncionarioMotorista.ValoFreteLiquido += comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;

                            if (configuracaoTMS.UtilizarComissaoPorCargo)
                            {
                                comissaoFuncionarioMotoristaDocumento.ValorComissao = Math.Round(comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo * (comissaoFuncionarioMotoristaDocumento.PercentualComissaoParcial / 100), 2, MidpointRounding.AwayFromZero);
                                comissaoFuncionarioMotoristaDocumento.ValorComissaoOriginal = comissaoFuncionarioMotoristaDocumento.ValorComissao;
                                comissaoFuncionarioMotorista.ValorComissao += comissaoFuncionarioMotoristaDocumento.ValorComissao;
                            }
                            else
                            {
                                comissaoFuncionarioMotoristaDocumento.ValorComissao = Math.Round(comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo * (comissaoFuncionario.PercentualComissao / 100), 2, MidpointRounding.AwayFromZero);
                                comissaoFuncionarioMotoristaDocumento.ValorComissaoOriginal = comissaoFuncionarioMotoristaDocumento.ValorComissao;
                                comissaoFuncionarioMotorista.ValorComissao += comissaoFuncionarioMotoristaDocumento.ValorComissao;
                            }

                            //comissaoFuncionarioMotoristaDocumento.ValorICMS = repCargaCTeComponentesFrete.BuscarValorComponentePorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);// carga.ValorICMS;
                            comissaoFuncionarioMotoristaDocumento.ValorICMS = repCargaCTe.BuscarValorICMSPorCargaSemAnulacao(carga.Codigo, "A");
                            comissaoFuncionarioMotorista.ValorICMS += comissaoFuncionarioMotoristaDocumento.ValorICMS;

                            //if (!configuracaoTMS.UtilizarComissaoPorCargo)
                            //    comissaoFuncionarioMotoristaDocumento.ValorPedagio = (from obj in cargaComponentesFrete where obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO && !obj.SomarComponenteFreteLiquido select obj.ValorComponente).Sum();
                            //else
                            comissaoFuncionarioMotoristaDocumento.ValorPedagio = (from obj in cargaComponentesFrete where obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO && !obj.SomarComponenteFreteLiquido && !obj.DescontarComponenteFreteLiquido select obj.ValorComponente).Sum();
                            comissaoFuncionarioMotorista.ValorPedagio += comissaoFuncionarioMotoristaDocumento.ValorPedagio;

                            comissaoFuncionarioMotoristaDocumento.ValorTotalFrete = repCargaCTe.BuscarValorAReceberPorCarga(carga.Codigo);
                            comissaoFuncionarioMotoristaDocumento.ValorTotalFreteOriginal = comissaoFuncionarioMotoristaDocumento.ValorTotalFrete;
                            comissaoFuncionarioMotorista.ValorTotalFrete += comissaoFuncionarioMotoristaDocumento.ValorTotalFrete;

                            repComissaoFuncionarioMotoristaDocumento.Inserir(comissaoFuncionarioMotoristaDocumento);
                        }

                        if (configuracaoTMS.UtilizarComissaoPorCargo)
                        {
                            if (comissaoFuncionarioMotorista.FaturamentoMinimo < comissaoFuncionarioMotorista.ValoBaseCalculo)
                            {
                                comissaoFuncionarioMotorista.ValoBaseCalculo = Math.Round(comissaoFuncionarioMotorista.ValoBaseCalculo - comissaoFuncionarioMotorista.FaturamentoMinimo, 2, MidpointRounding.AwayFromZero);
                                if (comissaoFuncionarioMotorista.ValoBaseCalculo <= 0)
                                    comissaoFuncionarioMotorista.ValoBaseCalculo = 0;

                                //decimal percentualPadrao = motorista.CargoMotorista != null && motorista.CargoMotorista.ComissaoPadrao > 0 ? motorista.CargoMotorista.ComissaoPadrao : configuracaoTMS.PercentualComissaoPadrao;
                                //comissaoFuncionarioMotorista.ValorComissao = Math.Round(comissaoFuncionarioMotorista.ValoBaseCalculo * (percentualPadrao / 100), 2, MidpointRounding.AwayFromZero);

                                decimal valorComissaoOriginal = comissaoFuncionarioMotorista.ValorComissao;
                                if (!comissaoFuncionarioMotorista.AtingiuMedia && comissaoFuncionarioMotorista.PercentualMedia > 0)
                                {
                                    decimal reducaoMedia = Math.Round(valorComissaoOriginal * (comissaoFuncionarioMotorista.PercentualMedia / 100), 2, MidpointRounding.AwayFromZero);
                                    comissaoFuncionarioMotorista.ValorComissao -= reducaoMedia;
                                }
                                if (!comissaoFuncionarioMotorista.NaoHouveAdvertencia && comissaoFuncionarioMotorista.PercentualAdvertencia > 0)
                                {
                                    decimal reducaoAdvertencia = Math.Round(valorComissaoOriginal * (comissaoFuncionarioMotorista.PercentualAdvertencia / 100), 2, MidpointRounding.AwayFromZero);
                                    comissaoFuncionarioMotorista.ValorComissao -= reducaoAdvertencia;
                                }
                                if (!comissaoFuncionarioMotorista.NaoHouveSinitro && comissaoFuncionarioMotorista.PercentualSinistro > 0)
                                {
                                    decimal reducaoSinistro = Math.Round(valorComissaoOriginal * (comissaoFuncionarioMotorista.PercentualSinistro / 100), 2, MidpointRounding.AwayFromZero);
                                    comissaoFuncionarioMotorista.ValorComissao -= reducaoSinistro;
                                }

                                if (comissaoFuncionarioMotorista.ValorComissao <= 0)
                                    comissaoFuncionarioMotorista.ValorComissao = 0;
                            }
                        }

                        foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
                        {
                            decimal percentualComissaoParcial = 100m;

                            if (ocorrencia?.Carga?.TipoOperacao?.GerarComissaoParcialMotorista ?? false)
                                percentualComissaoParcial = ocorrencia.Carga.TipoOperacao.PercentualComissaoParcialMotorista;

                            Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento comissaoFuncionarioMotoristaDocumento = new Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento();
                            comissaoFuncionarioMotoristaDocumento.CargaOcorrencia = ocorrencia;
                            comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista = comissaoFuncionarioMotorista;
                            comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados = ocorrencia.Carga?.DadosSumarizados;
                            comissaoFuncionarioMotoristaDocumento.Numero = ocorrencia.NumeroOcorrencia.ToString();
                            comissaoFuncionarioMotoristaDocumento.DataEmissao = ocorrencia.DataOcorrencia;
                            comissaoFuncionarioMotoristaDocumento.Veiculo = ocorrencia.Carga?.Veiculo;
                            comissaoFuncionarioMotoristaDocumento.VeiculosVinculados = ocorrencia.Carga?.VeiculosVinculados.ToList();
                            comissaoFuncionarioMotoristaDocumento.TipoDocumentoComissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoComissao.ocorrencia;
                            comissaoFuncionarioMotoristaDocumento.PercentualComissaoParcial = percentualComissaoParcial;


                            Dominio.Entidades.Embarcador.Cargas.CargaMotorista CargaMotorista = repCargaMotorista.BuscarPorCargaMotorista(ocorrencia.Carga?.Codigo ?? 0, motorista.Codigo);

                            comissaoFuncionarioMotoristaDocumento.PercentualExecucao = CargaMotorista?.PercentualExecucao ?? 100;

                            decimal valorOcorrenciaLiquida = repCargaCTeComplementoInfo.BuscarTotalFreteLiquidoPorOcorrencia(ocorrencia.Codigo);
                            decimal valorLiquido = ((valorOcorrenciaLiquida > 0 ? valorOcorrenciaLiquida : ocorrencia.ValorOcorrenciaLiquida) * comissaoFuncionarioMotoristaDocumento.PercentualExecucao) / 100;

                            comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo = Math.Round(((valorLiquido * (percentualComissaoParcial / 100)) * (comissaoFuncionario.PercentualBaseCalculoComissao / 100)), 2, MidpointRounding.AwayFromZero);
                            comissaoFuncionarioMotoristaDocumento.ValoBaseCalculoOriginal = comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;
                            comissaoFuncionarioMotorista.ValoBaseCalculo += comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;

                            comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido = valorLiquido;
                            comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal = comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;
                            comissaoFuncionarioMotorista.ValoFreteLiquido += comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;

                            comissaoFuncionarioMotoristaDocumento.ValorComissao = Math.Round(comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo * (comissaoFuncionario.PercentualComissao / 100), 2, MidpointRounding.AwayFromZero);
                            comissaoFuncionarioMotoristaDocumento.ValorComissaoOriginal = comissaoFuncionarioMotoristaDocumento.ValorComissao;


                            comissaoFuncionarioMotorista.ValorComissao += comissaoFuncionarioMotoristaDocumento.ValorComissao;

                            comissaoFuncionarioMotoristaDocumento.ValorICMS = repCargaCTeComplementoInfo.BuscarTotalICMSPorOcorrencia(ocorrencia.Codigo);
                            comissaoFuncionarioMotorista.ValorICMS += comissaoFuncionarioMotoristaDocumento.ValorICMS;

                            decimal valorOcorrencia = 0;
                            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarCTesPorOcorrencia(ocorrencia.Codigo);
                            foreach (var cargaCTe in cargaCTes)
                            {
                                if ((cargaCTe.CTe.ValorAReceber + cargaCTe.CTe.ValorICMS) > cargaCTe.CTe.ValorPrestacaoServico)
                                    valorOcorrencia = cargaCTe.CTe.ValorAReceber;
                                else
                                    valorOcorrencia = cargaCTe.CTe.ValorAReceber + cargaCTe.CTe.ValorICMS;
                            }

                            comissaoFuncionarioMotoristaDocumento.ValorTotalFrete = (valorOcorrencia > 0 ? valorOcorrencia : ocorrencia.ValorOcorrencia + comissaoFuncionarioMotoristaDocumento.ValorICMS);
                            comissaoFuncionarioMotoristaDocumento.ValorTotalFreteOriginal = comissaoFuncionarioMotoristaDocumento.ValorTotalFrete;
                            comissaoFuncionarioMotorista.ValorTotalFrete += comissaoFuncionarioMotoristaDocumento.ValorTotalFrete;

                            repComissaoFuncionarioMotoristaDocumento.Inserir(comissaoFuncionarioMotoristaDocumento);
                        }

                        if (!configuracaoTMS.UtilizarComissaoPorCargo)
                            comissaoFuncionarioMotorista.TabelaProdutividadeValores = RetornarTabelaProdutividadeValores(comissaoFuncionarioMotorista, unitOfWork);
                        repComissaoFuncionarioMotorista.Atualizar(comissaoFuncionarioMotorista);
                        //}

                        numeroMotoristasProcessados++;
                        int processados = (int)(100 * numeroMotoristasProcessados) / comissaoFuncionario.TotalFuncionarios;

                        if (comissaoFuncionario.PercentualGerado < processados)
                        {
                            comissaoFuncionario.PercentualGerado = processados;
                            repComissaoFuncionario.Atualizar(comissaoFuncionario);
                            serNotificacao.InfomarPercentualProcessamento(null, comissaoFuncionario.Codigo, "RH/ComissaoFuncionario", (decimal)comissaoFuncionario.PercentualGerado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.comissao, tipoServicoMultisoftware, unitOfWork);
                        }
                        if (!transacaoAtiva)
                            unitOfWork.CommitChanges();
                    }
                }
                catch (Exception ex)
                {
                    falha = true;
                    if (!transacaoAtiva)
                        unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    GerarNotificacaoComissao(comissaoFuncionario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.FalhaNaGeracao, "Ocorreu uma falha ao gerar a comissão do motorista.", Localization.Resources.Transportadores.Motorista.OcorreuFalhaGerarComissao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, cliente, tipoServicoMultisoftware, adminStringConexao);
                    break;
                }
            }
            if (!falha)
            {
                try
                {
                    GerarNotificacaoComissao(comissaoFuncionario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Gerada, "", Localization.Resources.Transportadores.Motorista.ComissaoGeradaSucessoMotoristas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, cliente, tipoServicoMultisoftware, adminStringConexao);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    GerarNotificacaoComissao(comissaoFuncionario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.FalhaNaGeracao, "Ocorreu uma falha ao finalizar a geração das comissões.", Localization.Resources.Transportadores.Motorista.OcorreuFalhaGerarComissao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, cliente, tipoServicoMultisoftware, adminStringConexao);
                }
            }
            if (!transacaoAtiva)
                unitOfWork.Dispose();

            return !falha;
        }


        private Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores RetornarTabelaProdutividadeValores(Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.RH.TabelaProdutividadeValores repValores = new Repositorio.Embarcador.RH.TabelaProdutividadeValores(unitOfWork);
            return repValores.BuscarPorValor(comissaoFuncionarioMotorista.ValoFreteLiquido);
        }

        private void GerarNotificacaoComissao(int codigoComissaoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario situacaoComissao, string mensagemFalha, string mensagemNotificacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string adminStringConexao)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Notificacao.Notificacao(StringConexao, cliente, tipoServicoMultisoftware, adminStringConexao);
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Embarcador.RH.ComissaoFuncionario repComissaoFuncionario = new Repositorio.Embarcador.RH.ComissaoFuncionario(unitOfWork);

            Dominio.Entidades.Embarcador.RH.ComissaoFuncionario comissaoFuncionario = repComissaoFuncionario.BuscarPorCodigo(codigoComissaoFuncionario);
            comissaoFuncionario.SituacaoComissaoFuncionario = situacaoComissao;
            comissaoFuncionario.MensagemFalhaGeracao = mensagemFalha;
            repComissaoFuncionario.Atualizar(comissaoFuncionario);
            serNotificacao.GerarNotificacao(comissaoFuncionario.UsuarioGerouComissao, comissaoFuncionario.Codigo, "RH/ComissaoFuncionario", mensagemNotificacao, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.comissao, tipoServicoMultisoftware, unitOfWork);
        }

        public void AjustarValoresComissaoMotorista(Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista, decimal diferencaFreteLiquido, decimal outrosValores, decimal diferencaoBC, decimal diferencaoComissao, Repositorio.UnitOfWork unitOfWork, decimal diferencaTotalFrete)
        {
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unitOfWork);

            if (diferencaTotalFrete != 0)
                comissaoFuncionarioMotorista.ValorTotalFrete += diferencaTotalFrete;
            comissaoFuncionarioMotorista.ValoFreteLiquido += diferencaFreteLiquido;
            comissaoFuncionarioMotorista.ValoBaseCalculo += diferencaoBC;
            comissaoFuncionarioMotorista.ValorComissao += diferencaoComissao;
            comissaoFuncionarioMotorista.OutrosValores += outrosValores;
            comissaoFuncionarioMotorista.TabelaProdutividadeValores = RetornarTabelaProdutividadeValores(comissaoFuncionarioMotorista, unitOfWork);

            repComissaoFuncionarioMotorista.Atualizar(comissaoFuncionarioMotorista);
        }


        public dynamic RetornarComissaoFuncionarioDadosGrid(Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista)
        {
            var retorno = new
            {
                comissaoFuncionarioMotorista.Codigo,
                CPF = comissaoFuncionarioMotorista.Motorista.CPF_Formatado,
                CodigoMotorista = comissaoFuncionarioMotorista.Motorista.Codigo,
                Motorista = comissaoFuncionarioMotorista.Motorista.Nome,
                comissaoFuncionarioMotorista.NumeroDiasEmViagem,
                comissaoFuncionarioMotorista.ValorProdutividade,
                OutrosValores = comissaoFuncionarioMotorista.OutrosValores.ToString("n2"),
                ValorTotalFrete = comissaoFuncionarioMotorista.ValorTotalFrete.ToString("n2"),
                PercentualComissao = comissaoFuncionarioMotorista.PercentualComissao.ToString("n2"),
                ValoBaseCalculo = comissaoFuncionarioMotorista.ValoBaseCalculo.ToString("n2"),
                ValoFreteLiquido = comissaoFuncionarioMotorista.ValoFreteLiquido.ToString("n2"),
                ValorComissao = comissaoFuncionarioMotorista.ValorComissao.ToString("n2"),
                ValorICMS = comissaoFuncionarioMotorista.ValorICMS.ToString("n2"),
                ValorDiaria = comissaoFuncionarioMotorista.ComissaoFuncionario.ValorDiaria,
                ValorNormativo = comissaoFuncionarioMotorista.ValorNormativo.ToString("n2"),
                ValorPedagio = comissaoFuncionarioMotorista.ValorPedagio.ToString("n2"),
                MediaFinal = comissaoFuncionarioMotorista.MediaFinal.ToString("n2"),
                GerarComissao = comissaoFuncionarioMotorista.GerarComissao,
                Total = (comissaoFuncionarioMotorista.ValorNormativo + comissaoFuncionarioMotorista.ValorComissao + comissaoFuncionarioMotorista.ValorBonificacao).ToString("n2"),
                comissaoFuncionarioMotorista.AtingiuMedia,
                DT_Enable = comissaoFuncionarioMotorista.GerarComissao,
                DT_RowColor = comissaoFuncionarioMotorista.PossuiDuasFrotas ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco,
                comissaoFuncionarioMotorista.PrimeiraFrota
            };
            return retorno;
        }




        public IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotorista> BuscarListaDataSetComissao(int codigo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unitOfWork);

            IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotorista> comissoesFuncionarioMotorista = repComissaoFuncionarioMotoristaDocumento.ConsultaRelatorio(codigo, 0);


            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            return comissoesFuncionarioMotorista;
        }


        public IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento> BuscarListaDataSetComissaoAbastecimento(int codigo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unitOfWork);

            IList<Dominio.Relatorios.Embarcador.DataSource.RH.ComissaoFuncionarioMotoristaAbastecimento> comissaoFuncionarioMotoristaAbastecimento = repComissaoFuncionarioMotoristaDocumento.ConsultaRelatorioAbastecimento(codigo, 0);


            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            return comissaoFuncionarioMotoristaAbastecimento;
        }


    }
}
