using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Veiculo
{
    public class Veiculo
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Privados

        #region Construtores

        public Veiculo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public static string ObterDescricaoPlacas(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo == null)
                return string.Empty;

            string conjunto = veiculo.Placa;

            if (veiculo.TipoVeiculo == "0")
            {
                foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                    conjunto += ", " + reboque.Placa;
            }
            else if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
            {
                Dominio.Entidades.Veiculo tracao = veiculo.VeiculosTracao.FirstOrDefault();

                foreach (Dominio.Entidades.Veiculo reboque in tracao.VeiculosTracao)
                    conjunto += ", " + reboque.Placa;
            }

            return conjunto;
        }

        public static string ObterDescricaoComModeloVeicularEReboques(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo != null)
            {
                string conjunto = veiculo.Placa + " (" + veiculo.ModeloVeicularCarga.Descricao + ")";

                if (veiculo.TipoVeiculo == "0")
                {
                    foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                    {
                        conjunto += ", " + reboque.Placa + " (" + reboque.ModeloVeicularCarga.Descricao + ")";
                    }
                }
                else
                {
                    if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                    {
                        Dominio.Entidades.Veiculo tracao = veiculo.VeiculosTracao.FirstOrDefault();
                        foreach (Dominio.Entidades.Veiculo reboque in tracao.VeiculosTracao)
                        {
                            conjunto += ", " + reboque.Placa + " (" + reboque.ModeloVeicularCarga.Descricao + ")";
                        }
                    }
                }

                return conjunto;
            }
            else
            {
                return "";
            }

        }

        public static void ExtornarAutorizacoesPeso(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, string motivoExtorno)
        {
            Repositorio.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga repVeiculoToleranciaPesoAutorizacaoCarga = new Repositorio.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga> autorizacoesPesoExtornar = repVeiculoToleranciaPesoAutorizacaoCarga.BuscarNaoExtornadasPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga autorizacaoPesoExtornar in autorizacoesPesoExtornar)
            {
                if (autorizacaoPesoExtornar.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPesoCarga.Liberada)
                {
                    autorizacaoPesoExtornar.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPesoCarga.AutorizacaoExtornada;
                    autorizacaoPesoExtornar.MotivoExtornoAutorizacao = motivoExtorno;

                    repVeiculoToleranciaPesoAutorizacaoCarga.Atualizar(autorizacaoPesoExtornar);
                }
                else
                    repVeiculoToleranciaPesoAutorizacaoCarga.Deletar(autorizacaoPesoExtornar);
            }
        }

        public void VerificarSeNecessariaAutorizacaoPeso(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga repVeiculoToleranciaPesoAutorizacaoCarga = new Repositorio.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            if (!configuracao.ValidarToleranciaPesoModeloVeicular)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeiculares = carga.VeiculosVinculados.Where(o => o.ModeloVeicularCarga != null).Select(o => o.ModeloVeicularCarga).ToList();

            if (carga.Veiculo?.ModeloVeicularCarga != null)
                modelosVeiculares.Add(carga.Veiculo.ModeloVeicularCarga);

            if (modelosVeiculares.Count == 0)
                return;

            decimal pesoTotalCarga = repPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(carga.Codigo);
            decimal volumesTotalCarga = repPedidoXMLNotaFiscal.BuscarVolumesPorCarga(carga.Codigo);
            decimal metrosCubicosTotalCarga = repPedidoXMLNotaFiscal.BuscarMetrosCubicosPorCarga(carga.Codigo);
            decimal palletsTotalCarga = repPedidoXMLNotaFiscal.BuscarPalletsPorCarga(carga.Codigo);

            decimal capacidadeMaximaPeso = modelosVeiculares.Where(o => o.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Peso).Sum(o => o.CapacidadePesoTransporte + o.ToleranciaPesoExtra);
            decimal capacidadeMinimaPeso = modelosVeiculares.Where(o => o.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Peso).Sum(o => o.ToleranciaPesoMenor);

            decimal capacidadeMaximaUnidade = modelosVeiculares.Where(o => o.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Unidade).Sum(o => o.CapacidadePesoTransporte + o.ToleranciaPesoExtra);
            decimal capacidadeMinimaUnidade = modelosVeiculares.Where(o => o.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Unidade).Sum(o => o.ToleranciaPesoMenor);

            decimal capacidadeMaximaCubagem = modelosVeiculares.Where(o => o.ModeloControlaCubagem).Sum(o => o.Cubagem);
            decimal capacidadeMinimaCubagem = modelosVeiculares.Where(o => o.ModeloControlaCubagem).Sum(o => o.ToleranciaMinimaCubagem);

            decimal capacidadeMaximaPallets = modelosVeiculares.Where(o => o.VeiculoPaletizado).Sum(o => o.NumeroPaletes) ?? 0m;
            decimal capacidadeMinimaPallets = modelosVeiculares.Where(o => o.VeiculoPaletizado).Sum(o => o.ToleranciaMinimaPaletes);
            decimal ocupacaoCubicaPaletes = (carga.TipoDeCarga?.Paletizado ?? false) ? modelosVeiculares.Sum(o => o.ObterOcupacaoCubicaPaletes()) : 0m;

            metrosCubicosTotalCarga += ocupacaoCubicaPaletes;

            StringBuilder sbMensagemRetorno = new StringBuilder();

            bool bloquear = false;

            if (modelosVeiculares.Any(o => o.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Peso) && (capacidadeMaximaPeso < pesoTotalCarga || capacidadeMinimaPeso > pesoTotalCarga))
            {
                sbMensagemRetorno.Append("O modelo de veículo (").Append(string.Join("/", modelosVeiculares.Where(o => o.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Peso).Select(o => o.Descricao))).Append(") não pode transportar este peso (").Append(pesoTotalCarga.ToString("n2")).Append(") de acordo com a configuração, sendo necessária a autorização do responsável para prosseguir.");
                bloquear = true;
            }
            else if (modelosVeiculares.Any(o => o.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Unidade) && (capacidadeMaximaUnidade < volumesTotalCarga || capacidadeMinimaUnidade > volumesTotalCarga))
            {
                sbMensagemRetorno.Append("O modelo de veículo (").Append(string.Join("/", modelosVeiculares.Where(o => o.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Unidade).Select(o => o.Descricao))).Append(") não pode transportar esta quantidade de volumes (").Append(volumesTotalCarga.ToString("n2")).Append(") de acordo com a configuração, sendo necessária a autorização do responsável para prosseguir.");
                bloquear = true;
            }
            else if (modelosVeiculares.Any(o => o.ModeloControlaCubagem) && (capacidadeMaximaCubagem < metrosCubicosTotalCarga || capacidadeMinimaCubagem > metrosCubicosTotalCarga))
            {
                sbMensagemRetorno.Append("O modelo de veículo (").Append(string.Join("/", modelosVeiculares.Where(o => o.ModeloControlaCubagem).Select(o => o.Descricao))).Append(") não pode transportar esta quantidade de metros cúbicos (").Append(metrosCubicosTotalCarga.ToString("n2")).Append(") de acordo com a configuração, sendo necessária a autorização do responsável para prosseguir.");
                bloquear = true;
            }
            else if (modelosVeiculares.Any(o => o.VeiculoPaletizado) && (capacidadeMaximaPallets < palletsTotalCarga || capacidadeMinimaPallets > palletsTotalCarga))
            {
                sbMensagemRetorno.Append("O modelo de veículo (").Append(string.Join("/", modelosVeiculares.Where(o => o.VeiculoPaletizado).Select(o => o.Descricao))).Append(") não pode transportar esta quantidade de pallets (").Append(palletsTotalCarga.ToString("n2")).Append(") de acordo com a configuração, sendo necessária a autorização do responsável para prosseguir.");
                bloquear = true;
            }

            if (bloquear)
            {
                Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga veiculoToleranciaPesoAutorizacaoCarga = new Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga()
                {
                    Carga = carga,
                    ModeloVeicularCarga = modelosVeiculares.FirstOrDefault(),
                    Mensagem = Utilidades.String.Left(sbMensagemRetorno.ToString(), 500),
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPesoCarga.AgLiberacao
                };

                repVeiculoToleranciaPesoAutorizacaoCarga.Inserir(veiculoToleranciaPesoAutorizacaoCarga);
            }
        }

        public static bool VerificarSeEnecessarioAutorizacaoPeso(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, out string mensagem)
        {
            mensagem = string.Empty;

            Repositorio.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga repVeiculoToleranciaPesoAutorizacaoCarga = new Repositorio.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga> autorizacoes = repVeiculoToleranciaPesoAutorizacaoCarga.BuscarNaoExtornadasPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoToleranciaPesoAutorizacaoCarga autorizacao in autorizacoes)
            {
                if (autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoPesoCarga.AgLiberacao)
                {
                    mensagem = autorizacao.Mensagem;
                    return true;
                }
            }

            return false;
        }

        public static Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT IntegrarComCIOT(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterConfiguracaoCIOT(veiculo.Proprietario, veiculo.Empresa, unitOfWork);

            if (configuracaoCIOT != null && configuracaoCIOT.IntegrarVeiculoNoCadastro)
                return configuracaoCIOT;
            else
                return null;
        }

        public static void AtualizarIntegracoes(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Usuario motorista = null)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            IList<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> integracoes = repVeiculoIntegracao.BuscarPorVeiculo(veiculo.Codigo);

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracaoVeiculo in integracoes)
            {
                if (!integracaoVeiculo.TipoIntegracao.Ativo)
                    continue;

                TipoIntegracao tipoIntegracao = integracaoVeiculo.TipoIntegracao.Tipo;

                if (tipoIntegracao == TipoIntegracao.MultiEmbarcador || integracaoVeiculo.TipoIntegracao.IntegrarVeiculoTrocaMotorista)
                    continue;

                if (tipoIntegracao == TipoIntegracao.BrasilRiskVeiculoMotorista)
                    continue;

                if (tipoIntegracao != TipoIntegracao.CIOT && !configuracao.TiposIntegracaoValidarVeiculo.Any(o => o.Equals(integracaoVeiculo.TipoIntegracao)))
                    repVeiculoIntegracao.Deletar(integracaoVeiculo);
                else if (tipoIntegracao == TipoIntegracao.Buonny || tipoIntegracao == TipoIntegracao.BuonnyRNTRC)
                {
                    if (integracaoVeiculo.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                    {
                        integracaoVeiculo.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                        integracaoVeiculo.DataIntegracao = DateTime.Now;
                        integracaoVeiculo.NumeroTentativas = 0;
                        integracaoVeiculo.ProblemaIntegracao = "";
                        repVeiculoIntegracao.Atualizar(integracaoVeiculo);
                    }
                }
                else if (tipoIntegracao == TipoIntegracao.BrasilRiskGestao || tipoIntegracao == TipoIntegracao.Ultragaz || tipoIntegracao == TipoIntegracao.SemParar || tipoIntegracao == TipoIntegracao.Frota162 || tipoIntegracao == TipoIntegracao.KMM)
                {
                    integracaoVeiculo.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracaoVeiculo.DataIntegracao = DateTime.Now;
                    integracaoVeiculo.NumeroTentativas = 0;
                    integracaoVeiculo.ProblemaIntegracao = "";
                    repVeiculoIntegracao.Atualizar(integracaoVeiculo);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in configuracao.TiposIntegracaoValidarVeiculo)
            {
                if (tipoIntegracao.Ativo && !integracoes.Any(o => o.TipoIntegracao.Equals(tipoIntegracao)))
                {
                    if (tipoIntegracao.Tipo == TipoIntegracao.Frota162 && veiculo.Tipo == "T")
                        continue;

                    Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao = new Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao()
                    {
                        DataIntegracao = DateTime.Now,
                        Veiculo = veiculo,
                        ProblemaIntegracao = "",
                        SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                        TipoIntegracao = tipoIntegracao
                    };
                    repVeiculoIntegracao.Inserir(integracao);
                }
            }

            GerarOuAtualizarIntegracaoCIOT(veiculo, integracoes, unitOfWork);
            GerarIntegracaoVeiculoBRK(veiculo, integracoes, unitOfWork);
            AjustarIntegracoesMultiEmbarcador(veiculo, integracoes, unitOfWork);

            if (motorista == null)
            {
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
            }

            if (motorista != null)
                Transportadores.Motorista.AtualizarIntegracoes(motorista, unitOfWork);
        }

        public static void AtualizarIntegracoesTrocaMotorista(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracoes = repTipoIntegracao.BuscarComIntegracaoVeiculoTrocaMotorista();

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracoes)
            {
                Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao = repVeiculoIntegracao.BuscarPorVeiculoETipoIntegracao(veiculo.Codigo, tipoIntegracao.Codigo);

                if (veiculoIntegracao == null)
                {
                    veiculoIntegracao = new Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao()
                    {
                        TipoIntegracao = tipoIntegracao,
                        Veiculo = veiculo
                    };
                }

                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                veiculoIntegracao.DataIntegracao = DateTime.Now;
                veiculoIntegracao.ProblemaIntegracao = "";

                if (veiculoIntegracao.Codigo == 0)
                    repVeiculoIntegracao.Inserir(veiculoIntegracao);
                else
                    repVeiculoIntegracao.Atualizar(veiculoIntegracao);
            }
        }

        public static void AtualizarHistoricoVinculoVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Veiculo veiculo, int codigoUsuario)
        {
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repHistoricoVeiculoVinculo = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento repHistoricoVeiculoVinculoEquipamento = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque repHistoricoVeiculoVinculoReboque = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista repHistoricoVeiculoVinculoMotorista = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado repHistoricoVeiculoVinculoCentroResultado = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            if (veiculo == null)
                return;

            Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = veiculo.CentroResultado;
            Dominio.Entidades.Usuario usuario = codigoUsuario > 0 ? repUsuario.BuscarPorCodigo(codigoUsuario) : null;
            Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(veiculo.Codigo);

            if (historicoVeiculoVinculo == null)
            {
                if (veiculoMotorista != null || veiculo.VeiculosVinculados != null)
                {
                    var novoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                    {
                        Veiculo = veiculo,
                        DataHora = DateTime.Now,
                        Usuario = usuario != null ? usuario : null,
                        KmRodado = veiculo.KilometragemAtual,
                        KmAtualModificacao = 0,
                        DiasVinculado = 0
                    };
                    repHistoricoVeiculoVinculo.Inserir(novoVeiculoVinculo);

                    if (veiculo.Equipamentos != null)
                    {
                        foreach (var equipamento in veiculo.Equipamentos)
                        {
                            var novoVeiculoVinculoEquipamento = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento()
                            {
                                HistoricoVeiculoVinculo = novoVeiculoVinculo,
                                Equipamento = equipamento
                            };
                            repHistoricoVeiculoVinculoEquipamento.Inserir(novoVeiculoVinculoEquipamento);
                        }
                    }

                    if (veiculo.VeiculosVinculados != null)
                    {
                        foreach (var reboque in veiculo.VeiculosVinculados)
                        {
                            if (reboque.TipoVeiculo == "1")
                            {
                                var novoVeiculoVinculoReboque = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque()
                                {
                                    HistoricoVeiculoVinculo = novoVeiculoVinculo,
                                    Veiculo = reboque
                                };
                                repHistoricoVeiculoVinculoReboque.Inserir(novoVeiculoVinculoReboque);
                            }
                        }
                    }

                    var novoVeiculoVinculoMotorista = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista()
                    {
                        HistoricoVeiculoVinculo = novoVeiculoVinculo,
                        Motorista = veiculoMotorista ?? null
                    };
                    repHistoricoVeiculoVinculoMotorista.Inserir(novoVeiculoVinculoMotorista);

                    var novoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado()
                    {
                        HistoricoVeiculoVinculo = novoVeiculoVinculo,
                        CentroResultado = centroResultado ?? null,
                        DataHora = DateTime.Now,
                    };
                    repHistoricoVeiculoVinculoCentroResultado.Inserir(novoVeiculoVinculoCentroResultado);
                }
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento> listaEquipamentos = repHistoricoVeiculoVinculoEquipamento.BuscarPorVinculo(historicoVeiculoVinculo.Codigo);
                List<Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque> listaReboques = repHistoricoVeiculoVinculoReboque.BuscarPorVinculo(historicoVeiculoVinculo.Codigo);
                Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista historicoVeiculoVinculoMotorista = repHistoricoVeiculoVinculoMotorista.BuscarPorVinculo(historicoVeiculoVinculo.Codigo);
                Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = repHistoricoVeiculoVinculoCentroResultado.BuscarPorVinculo(historicoVeiculoVinculo.Codigo);

                bool atualizouMotorista = false;
                if (historicoVeiculoVinculoMotorista != null)
                {
                    if (veiculoMotorista == null || historicoVeiculoVinculoMotorista.Motorista == null || historicoVeiculoVinculoMotorista.Motorista.Codigo != veiculoMotorista.Codigo)
                        atualizouMotorista = true;
                }

                bool atualizouCentroResultado = false;
                if (historicoVeiculoVinculoCentroResultado != null)
                {
                    if (centroResultado == null || historicoVeiculoVinculoCentroResultado.CentroResultado == null || historicoVeiculoVinculoCentroResultado.CentroResultado.Codigo != centroResultado.Codigo)
                        atualizouCentroResultado = true;
                }

                bool atualizouEquipamento = false;
                if (veiculo.Equipamentos != null && listaEquipamentos != null)
                {
                    foreach (var equipamento in veiculo.Equipamentos)
                    {
                        foreach (var equipamentoCadastrato in listaEquipamentos)
                        {
                            if (equipamento.Codigo != equipamentoCadastrato.Equipamento.Codigo)
                            {
                                atualizouEquipamento = true;
                                break;
                            }
                        }
                    }
                    if (!atualizouEquipamento)
                    {
                        if (veiculo.Equipamentos.Count > 0 && listaEquipamentos.Count == 0)
                            atualizouEquipamento = true;
                    }
                }
                bool atualizouReboque = false;
                if (veiculo.VeiculosVinculados != null && listaReboques != null)
                {
                    if (veiculo.VeiculosVinculados.Count == 0 && listaReboques.Count > 0)
                        atualizouReboque = true;
                    if (veiculo.VeiculosVinculados.Count == 1 && listaReboques.Count == 0)
                        atualizouReboque = true;

                    foreach (var reboque in veiculo.VeiculosVinculados)
                    {
                        foreach (var reboqueCadastrado in listaReboques)
                        {
                            if (reboque.TipoVeiculo == "1")
                            {
                                if (reboque.Codigo != reboqueCadastrado.Veiculo.Codigo)
                                {
                                    atualizouReboque = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!atualizouReboque)
                    {
                        if (veiculo.VeiculosVinculados.Count > 0 && listaReboques.Count == 0)
                            atualizouReboque = true;
                    }
                }

                if (atualizouMotorista || atualizouEquipamento || atualizouReboque || atualizouCentroResultado)
                {
                    var dataAtual = DateTime.Now;
                    historicoVeiculoVinculo.KmAtualModificacao = (veiculo.KilometragemAtual - historicoVeiculoVinculo.KmRodado);
                    historicoVeiculoVinculo.DiasVinculado = ((dataAtual - historicoVeiculoVinculo.DataHora).Days);
                    repHistoricoVeiculoVinculo.Atualizar(historicoVeiculoVinculo);

                    var novoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                    {
                        Veiculo = veiculo,
                        DataHora = DateTime.Now,
                        Usuario = usuario != null ? usuario : null,
                        KmRodado = veiculo.KilometragemAtual,
                        KmAtualModificacao = 0,
                        DiasVinculado = 0
                    };
                    repHistoricoVeiculoVinculo.Inserir(novoVeiculoVinculo);

                    if (atualizouReboque || atualizouMotorista)
                    {
                        foreach (var reboque in veiculo.VeiculosVinculados)
                        {
                            var novoVeiculoVinculoReboque = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque()
                            {
                                HistoricoVeiculoVinculo = novoVeiculoVinculo,
                                Veiculo = reboque
                            };
                            repHistoricoVeiculoVinculoReboque.Inserir(novoVeiculoVinculoReboque);
                        }
                    }
                    if (atualizouEquipamento)
                    {
                        foreach (var equipamento in veiculo.Equipamentos)
                        {
                            var novoVeiculoVinculoEquipamento = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento()
                            {
                                HistoricoVeiculoVinculo = novoVeiculoVinculo,
                                Equipamento = equipamento
                            };
                            repHistoricoVeiculoVinculoEquipamento.Inserir(novoVeiculoVinculoEquipamento);
                        }
                    }

                    var novoVeiculoVinculoMotorista = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista()
                    {
                        HistoricoVeiculoVinculo = novoVeiculoVinculo,
                        Motorista = veiculoMotorista ?? null
                    };
                    repHistoricoVeiculoVinculoMotorista.Inserir(novoVeiculoVinculoMotorista);

                    var novoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado()
                    {
                        HistoricoVeiculoVinculo = novoVeiculoVinculo,
                        CentroResultado = centroResultado ?? null,
                        DataHora = DateTime.Now,
                    };
                    repHistoricoVeiculoVinculoCentroResultado.Inserir(novoVeiculoVinculoCentroResultado);
                }
            }
        }

        public static void AtualizarHistoricoVinculoMotorista(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario motorista, int codigoUsuario)
        {
            Repositorio.Embarcador.Veiculos.HistoricoMotoristaVinculo repHistoricoMotoristaVinculo = new Repositorio.Embarcador.Veiculos.HistoricoMotoristaVinculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado repHistoricoMotoristaVinculoCentroResultado = new Repositorio.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            if (motorista == null)
                return;

            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = motorista.CentroResultado;
            Dominio.Entidades.Usuario usuario = codigoUsuario > 0 ? repUsuario.BuscarPorCodigo(codigoUsuario) : null;
            Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculo historicoMotoristaVinculo = repHistoricoMotoristaVinculo.BuscarPorMotorista(motorista.Codigo);

            if (historicoMotoristaVinculo == null)
            {
                var novoMotoristaVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculo()
                {
                    Motorista = motorista,
                    DataHora = DateTime.Now,
                    Usuario = usuario != null ? usuario : null,
                    DiasVinculado = 0
                };
                repHistoricoMotoristaVinculo.Inserir(novoMotoristaVinculo);

                var novoMotoristaVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado()
                {
                    HistoricoMotoristaVinculo = novoMotoristaVinculo,
                    CentroResultado = centroResultado ?? null,
                    DataHora = DateTime.Now,
                };
                repHistoricoMotoristaVinculoCentroResultado.Inserir(novoMotoristaVinculoCentroResultado);
            }
            else
            {
                Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado historicoMotoristaVinculoCentroResultado = repHistoricoMotoristaVinculoCentroResultado.BuscarPorVinculo(historicoMotoristaVinculo.Codigo);

                bool atualizouCentroResultado = false;
                if (historicoMotoristaVinculoCentroResultado != null)
                {
                    if (centroResultado == null || historicoMotoristaVinculoCentroResultado.CentroResultado == null || historicoMotoristaVinculoCentroResultado.CentroResultado.Codigo != centroResultado.Codigo)
                        atualizouCentroResultado = true;
                }

                if (atualizouCentroResultado)
                {
                    var dataAtual = DateTime.Now;
                    historicoMotoristaVinculo.DiasVinculado = ((dataAtual - historicoMotoristaVinculo.DataHora).Days);
                    repHistoricoMotoristaVinculo.Atualizar(historicoMotoristaVinculo);

                    var novoMotoristaVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculo()
                    {
                        Motorista = motorista,
                        DataHora = DateTime.Now,
                        Usuario = usuario != null ? usuario : null,
                        DiasVinculado = 0
                    };
                    repHistoricoMotoristaVinculo.Inserir(novoMotoristaVinculo);

                    var novoMotoristaVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoMotoristaVinculoCentroResultado()
                    {
                        HistoricoMotoristaVinculo = novoMotoristaVinculo,
                        CentroResultado = centroResultado ?? null,
                        DataHora = DateTime.Now,
                    };
                    repHistoricoMotoristaVinculoCentroResultado.Inserir(novoMotoristaVinculoCentroResultado);
                }
            }
        }

        public static string ConsultarVeiculoBrasilRisk(Dominio.Entidades.Veiculo veiculo, ref bool falhaIntegracao, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            falhaIntegracao = false;
            string mensagemErro = string.Empty;
            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;

            var repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            var repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);
            var repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao);

            if (tipoIntegracao != null)
            {
                Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao = repVeiculoIntegracao.BuscarPorVeiculoETipo(veiculo.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao);

                if (integracao == null)
                {
                    integracao = new Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao()
                    {
                        DataIntegracao = DateTime.Now,
                        Veiculo = veiculo,
                        ProblemaIntegracao = "",
                        TipoIntegracao = tipoIntegracao
                    };

                    repVeiculoIntegracao.Inserir(integracao);
                }

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas = 1;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                integracao.ProblemaIntegracao = string.Empty;

                Servicos.ServicoBrasilRisk.GestaoAnaliseDePerfil.RetornoConsulta retorno = Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.ConsultaVeiculo(veiculo.Placa, ref mensagemErro, ref xmlRequest, ref xmlResponse, unitOfWork);
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                {
                    integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno == null)
                {
                    integracao.ProblemaIntegracao = "Integração não teve retorno.";
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno.Status)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
                else
                {
                    integracao.ProblemaIntegracao = retorno.Mensagem;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                mensagemErro = integracao.ProblemaIntegracao.Replace("\n", "");
                falhaIntegracao = integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                string stringRetorno = string.Empty;
                if (retorno != null)
                    stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
                integracaoArquivo.Data = DateTime.Now;
                integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                if (integracao.ArquivosTransacao == null)
                    integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

                integracao.ArquivosTransacao.Add(integracaoArquivo);

                repVeiculoIntegracao.Inserir(integracao);

                if (integracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                    GerarAtendimentoParaIntegracoesPendentesVeiculos(integracao, codigoCarga, unitOfWork);
            }

            return mensagemErro;
        }

        public static string ConsultarVeiculoAdagio(Dominio.Entidades.Veiculo veiculo, ref bool falhaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Adagio.IntegracaoAdagio integracaoAdagio = Servicos.Embarcador.Integracao.Adagio.IntegracaoAdagio.GetInstance(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarSituacaoDpaResponse retornoSituacao = integracaoAdagio.ConsultaChecklist(veiculo.Placa);
            if (retornoSituacao != null)
            {
                falhaIntegracao = !integracaoAdagio.VerificarStatusAceito(retornoSituacao.status);
                if (falhaIntegracao)
                {
                    return integracaoAdagio.AlterarMensagem(retornoSituacao.status);
                }

                Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaPlacaResponse retornoCartaPlaca = integracaoAdagio.ConsultaCartorialVeiculo(veiculo.Placa);
                if (retornoCartaPlaca != null)
                {
                    falhaIntegracao = !integracaoAdagio.VerificarStatusAceito(retornoCartaPlaca.status);
                    return integracaoAdagio.AlterarMensagem(retornoSituacao.status);
                }
            }

            falhaIntegracao = true;
            return "Falha";
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo ConverterObjetoVeiculoEmbarcador(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Veiculo> reboques, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = new Repositorio.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Servicos.WebService.Empresa.Motorista serMotorista = new Servicos.WebService.Empresa.Motorista(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();

            if (veiculo.ModeloVeicularCarga != null && grupoPessoas != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga = repGrupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.BuscarPorGrupoPessoasEModeloVeicular(grupoPessoas.Codigo, veiculo.ModeloVeicularCarga.Codigo);

                if (grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga != null)
                {
                    veiculoIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular()
                    {
                        CodigoIntegracao = grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ModeloVeicularEmbarcador.CodigoModeloVeicularEmbarcador,
                        Descricao = grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ModeloVeicularEmbarcador.Descricao,
                        TipoModeloVeicular = grupoPessoasModeloVeicularEmbarcadorModeloVeicularCarga.ModeloVeicular.Tipo
                    };
                }
            }

            veiculoIntegracao.Reboques = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();
            veiculoIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

            veiculoIntegracao.AnoFabricacao = veiculo.AnoFabricacao;
            veiculoIntegracao.AnoModelo = veiculo.AnoModelo;
            veiculoIntegracao.Ativo = veiculo.Ativo;
            veiculoIntegracao.CapacidadeKG = veiculo.CapacidadeKG;
            veiculoIntegracao.CapacidadeM3 = veiculo.CapacidadeM3;
            veiculoIntegracao.DataAquisicao = veiculo.DataCompra.HasValue ? veiculo.DataCompra.Value.ToString("dd/MM/yyyy") : "";

            Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
            if (veiculoMotorista != null)
                veiculoIntegracao.Motoristas.Add(serMotorista.ConverterObjetoMotorista(veiculoMotorista));

            if (!string.IsNullOrEmpty(veiculo.Chassi))
                veiculoIntegracao.NumeroChassi = veiculo.Chassi;

            //veiculoIntegracao.NumeroFrota = veiculo.NumeroFrota;
            //veiculoIntegracao.NumeroMotor = veiculo.NumeroMotor;
            veiculoIntegracao.Placa = veiculo.Placa;
            veiculoIntegracao.TipoPropriedadeVeiculo = veiculo.Tipo == "P" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Proprio : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Terceiros;

            if (veiculoIntegracao.TipoPropriedadeVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Terceiros)
            {
                veiculoIntegracao.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();
                veiculoIntegracao.Proprietario.TipoTACVeiculo = veiculo.TipoProprietario;

                if (veiculo.Proprietario != null)
                    veiculoIntegracao.Proprietario.TransportadorTerceiro = serEmpresa.ConverterObjetoEmpresa(veiculo.Proprietario);

                veiculoIntegracao.Proprietario.TransportadorTerceiro.RNTRC = veiculo.RNTRC.ToString();
                veiculoIntegracao.Proprietario.CIOT = veiculo.CIOT;
            }

            veiculoIntegracao.Renavam = veiculo.Renavam;
            veiculoIntegracao.RNTC = veiculo.RNTRC.ToString();
            veiculoIntegracao.Tara = veiculo.Tara;
            veiculoIntegracao.TipoCarroceria = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria)int.Parse(veiculo.TipoCarroceria);
            veiculoIntegracao.TipoRodado = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado)int.Parse(veiculo.TipoRodado);
            veiculoIntegracao.TipoVeiculo = veiculo.TipoVeiculo == "0" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque;
            veiculoIntegracao.UF = veiculo.Estado.Sigla;

            if (veiculo.PossuiRastreador != null)
                veiculoIntegracao.PossuiRastreador = veiculo.PossuiRastreador;

            if (veiculo.NumeroEquipamentoRastreador != null)
                veiculoIntegracao.NumeroEquipamentoRastreador = veiculo.NumeroEquipamentoRastreador;

            if (veiculo.TecnologiaRastreador != null)
                veiculoIntegracao.TecnologiaRastreador = ConverterObjetoTecnologiaRastreador(veiculo.TecnologiaRastreador);

            if (veiculo.TipoComunicacaoRastreador != null)
                veiculoIntegracao.TipoComunicacaoRastreador = ConverterObjetoTipoComunicacaoRastreador(veiculo.TipoComunicacaoRastreador);

            if (reboques != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in reboques)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboqueIntegracao = ConverterObjetoVeiculoEmbarcador(grupoPessoas, reboque, null, unitOfWork);

                    reboqueIntegracao.Reboques = null;

                    veiculoIntegracao.Reboques.Add(reboqueIntegracao);
                }
            }

            return veiculoIntegracao;
        }

        public void AtualizarModeloVeicularDoVeiculoCarga(Dominio.Entidades.Veiculo veiculo, Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular objetoModeloVeicular, ref StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicularCarga.buscarPorCodigoIntegracao(objetoModeloVeicular.CodigoIntegracao);

            if (modeloVeicularCarga == null)
            {
                stMensagem.AppendLine($"Não foi encontrado o modelo veicular {objetoModeloVeicular.CodigoIntegracao}");
                return;
            }

            if (veiculo.ModeloVeicularCarga?.Codigo == modeloVeicularCarga.Codigo)
                return;

            veiculo.ModeloVeicularCarga = modeloVeicularCarga;
            repVeiculo.Atualizar(veiculo);
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo ConverterObjetoVeiculo(Dominio.Entidades.Veiculo veiculo, List<Dominio.Entidades.Veiculo> reboques, string ciot, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

            Servicos.WebService.Empresa.Motorista serMotorista = new Servicos.WebService.Empresa.Motorista(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();

            veiculoIntegracao.Reboques = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();
            veiculoIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

            veiculoIntegracao.AnoFabricacao = veiculo.AnoFabricacao;
            veiculoIntegracao.AnoModelo = veiculo.AnoModelo;
            veiculoIntegracao.Ativo = veiculo.Ativo;
            veiculoIntegracao.CapacidadeKG = veiculo.CapacidadeKG;
            veiculoIntegracao.CapacidadeM3 = veiculo.CapacidadeM3;
            veiculoIntegracao.DataAquisicao = veiculo.DataCompra.HasValue ? veiculo.DataCompra.Value.ToString("dd/MM/yyyy") : "";

            if (veiculoMotorista != null)
                veiculoIntegracao.Motoristas.Add(serMotorista.ConverterObjetoMotorista(veiculoMotorista));

            //veiculoIntegracao.NumeroChassi = veiculo.Chassi;
            //veiculoIntegracao.NumeroFrota = veiculo.NumeroFrota;
            //veiculoIntegracao.NumeroMotor = veiculo.NumeroMotor;
            veiculoIntegracao.Placa = veiculo.Placa;
            veiculoIntegracao.TipoPropriedadeVeiculo = veiculo.Tipo == "P" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Proprio : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Terceiros;

            if (veiculoIntegracao.TipoPropriedadeVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedadeVeiculo.Terceiros)
            {
                veiculoIntegracao.Proprietario = new Dominio.ObjetosDeValor.Embarcador.Frota.Proprietario();
                veiculoIntegracao.Proprietario.TipoTACVeiculo = veiculo.TipoProprietario;

                if (veiculo.Proprietario != null)
                    veiculoIntegracao.Proprietario.TransportadorTerceiro = serEmpresa.ConverterObjetoEmpresa(veiculo.Proprietario);

                veiculoIntegracao.Proprietario.TransportadorTerceiro.RNTRC = veiculo.RNTRC.ToString();

                if (!string.IsNullOrWhiteSpace(ciot))
                    veiculoIntegracao.Proprietario.CIOT = ciot;
                else
                    veiculoIntegracao.Proprietario.CIOT = veiculo.CIOT;
            }

            veiculoIntegracao.Renavam = veiculo.Renavam;
            veiculoIntegracao.RNTC = veiculo.RNTRC.ToString();
            veiculoIntegracao.Tara = veiculo.Tara;
            veiculoIntegracao.TipoCarroceria = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria)int.Parse(veiculo.TipoCarroceria);
            veiculoIntegracao.TipoRodado = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado)int.Parse(veiculo.TipoRodado);
            veiculoIntegracao.TipoVeiculo = veiculo.TipoVeiculo == "0" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque;
            veiculoIntegracao.UF = veiculo.Estado.Sigla;

            if (reboques != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in reboques)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboqueIntegracao = ConverterObjetoVeiculo(reboque, null, ciot, unitOfWork);

                    reboqueIntegracao.Reboques = null;

                    veiculoIntegracao.Reboques.Add(reboqueIntegracao);
                }
            }

            return veiculoIntegracao;
        }

        public static byte[] ObterPdfQRCodeVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            return ObterPdfTodosQRCodeVeiculo(new List<Dominio.Entidades.Veiculo>() { veiculo });
        }

        public static byte[] ObterPdfTodosQRCodeVeiculo(List<Dominio.Entidades.Veiculo> veiculos)
        {
            return ReportRequest.WithType(ReportType.VeiculosQrCode)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("Veiculos", veiculos.ToJson())
                .CallReport()
                .GetContentFile();
        }

        public static byte[] ObterTodosPdfQRCodeCompactado(List<Dominio.Entidades.Veiculo> veiculos)
        {
            Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                string nomeArquivo = $"QR Code {veiculo.Placa}.pdf";

                if (!conteudoCompactar.ContainsKey(nomeArquivo))
                    conteudoCompactar.Add(nomeArquivo, ObterPdfQRCodeVeiculo(veiculo));
            }

            MemoryStream arquivoTodosPdfQrCode = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
            byte[] arquivoBinarioTodosPdfQrCode = arquivoTodosPdfQrCode.ToArray();

            arquivoTodosPdfQrCode.Dispose();

            return arquivoBinarioTodosPdfQrCode;
        }

        public static void ValidarBloqueioPorCargaNaoFinalizada(string placa, int codigoCargaDesconsiderar, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            if (!configuracaoEmbarcador.BloquearVeiculoExistenteEmCargaNaoFinalizada || string.IsNullOrWhiteSpace(placa))
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (repositorioCarga.ExisteCargaNaoFinalizadaComVeiculo(placa, codigoCargaDesconsiderar))
                throw new ServicoException("Já existe uma carga aberta com esse veículo.");
        }

        public static void ValidarDataLiberacaoSeguradora(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            if (veiculo == null)
                return;

            if (!configuracaoEmbarcador.ValidarDataLiberacaoSeguradoraVeiculo)
                return;

            ValidarDataLiberacaoSeguradora(veiculo);
        }

        public static void ValidarDataLiberacaoSeguradora(ICollection<Dominio.Entidades.Veiculo> veiculos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            if ((veiculos == null) || (veiculos.Count == 0))
                return;

            if (!configuracaoEmbarcador.ValidarDataLiberacaoSeguradoraVeiculo)
                return;

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
                ValidarDataLiberacaoSeguradora(veiculo);
        }

        public static void IntegrarEmailVeiculoNovo(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            if (email == null)
                return;

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupos = repGrupoPessoas.BuscarGrupoPessoasEnvioEmailCadastroVeiculo();
            if (grupos != null && grupos.Count > 0)
            {
                veiculoIntegracao.DataIntegracao = DateTime.Now;
                veiculoIntegracao.NumeroTentativas += 1;

                List<string> emails = new List<string>();
                foreach (var grupo in grupos)
                {
                    if (!string.IsNullOrWhiteSpace(grupo.EmailEnvioNovoVeiculo))
                        emails.AddRange(grupo.EmailEnvioNovoVeiculo.Split(';').ToList());
                }
                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    Dominio.Entidades.Veiculo veiculo = veiculoIntegracao.Veiculo;
                    Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                    string assunto = "Cadastro do veículo " + veiculo.Placa;
                    string mensagemEmail = string.Empty;

                    mensagemEmail = "<br/>Segue dados do novo veículo cadastrado:<br/>";

                    mensagemEmail += "<script src='https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js'></script>";
                    mensagemEmail += "<script src='http://www.developerdan.com/table-to-json/javascripts/jquery.tabletojson.min.js'></script>";
                    mensagemEmail += "<table style='width:100%; align='center'; border='1'; cellpadding='0'; cellspacing='0';>";
                    mensagemEmail += "<thead>";
                    mensagemEmail += "<th>Placa</th>";
                    mensagemEmail += "<th>RENAVAM</th>";
                    mensagemEmail += "<th>CPF/CNPJ Proprietário</th>";
                    mensagemEmail += "<th>Proprietário</th>";
                    mensagemEmail += "<th>UF</th>";
                    mensagemEmail += "<th>RNTRC</th>";
                    mensagemEmail += "<th>Chassi</th>";
                    mensagemEmail += "<th>CPF Motorista</th>";
                    mensagemEmail += "<th>Motorista</th>";
                    mensagemEmail += "<th>Reboque(s)</th>";
                    mensagemEmail += "<th>RENAVAM</th>";
                    mensagemEmail += "<th>Qtd. Eixos</th>";
                    mensagemEmail += "</thead>";
                    mensagemEmail += "<tbody>";

                    string reboques = "";
                    if (veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                        reboques = string.Join(", ", veiculo.VeiculosVinculados.Select(c => c.Placa).ToString());

                    mensagemEmail += "<tr>";
                    mensagemEmail += "<td>" + veiculo.Placa + "</td>";
                    mensagemEmail += "<td>" + veiculo.Renavam + "</td>";
                    mensagemEmail += "<td>" + (veiculo.Proprietario?.CPF_CNPJ_Formatado ?? "") + "</td>";
                    mensagemEmail += "<td>" + (veiculo.Proprietario?.Nome ?? "") + "</td>";
                    mensagemEmail += "<td>" + (veiculo.Estado?.Sigla ?? "") + "</td>";
                    mensagemEmail += "<td>" + (veiculo.RNTRC) + "</td>";
                    mensagemEmail += "<td>" + (veiculo.Chassi) + "</td>";
                    mensagemEmail += "<td>" + (veiculoMotorista?.CPF_Formatado ?? "") + "</td>";
                    mensagemEmail += "<td>" + (veiculoMotorista?.Nome ?? "") + "</td>";
                    mensagemEmail += "<td>" + (reboques) + "</td>";
                    mensagemEmail += "<td>" + (veiculo.Renavam) + "</td>";
                    mensagemEmail += "<td>" + (veiculo.ModeloVeicularCarga?.NumeroEixos?.ToString("n0") ?? "") + "</td>";
                    mensagemEmail += "</tr>";

                    mensagemEmail += "</tbody></table>";

                    mensagemEmail += "<br/><br/>E -mail enviado automaticamente. Por favor, não responda.";

                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out string mensagemErro, email.DisplayEmail,
                               null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);

                    if (!sucesso)
                    {
                        Servicos.Log.TratarErro("Problemas ao enviar email do novo cadastro de veículo: " + mensagemErro);
                        veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        veiculoIntegracao.ProblemaIntegracao = $"Falha ao enviar o e-mail {mensagemErro}.";
                    }
                    else
                    {
                        veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        veiculoIntegracao.ProblemaIntegracao = $"E-mail enviado com sucesso.";

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        integracaoArquivo.Mensagem = string.Join("; ", emails.ToList());
                        integracaoArquivo.Data = DateTime.Now;
                        integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                        repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                        if (veiculoIntegracao.ArquivosTransacao == null)
                            veiculoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
                        veiculoIntegracao.ArquivosTransacao.Add(integracaoArquivo);
                    }
                }
                else
                {
                    veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    veiculoIntegracao.ProblemaIntegracao = $"Nenhum e-mail configurado no grupo de pessoa configurado para o envio.";
                }
            }
            else
            {
                veiculoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = $"Nenhum grupo de pessoa configurado para o envio do e-mail.";
            }
            repVeiculoIntegracao.Atualizar(veiculoIntegracao);
        }

        public static void InativarVeiculosSemUtilizacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            int quantidadesDiasParaDesativarVeiculos = configuracaoEmbarcador.DesabilitarVeiculosInutilizadosDias;
            if (quantidadesDiasParaDesativarVeiculos <= 0)
                return;

            new Inutilizados(unitOfWork, tipoServicoMultisoftware, auditado).InativarVeiculosSemUtilizacao(quantidadesDiasParaDesativarVeiculos);
        }

        public static void IntegrarVeiculosPendentesIntegracaoTrocaMotorista(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> integracoes = repVeiculoIntegracao.BuscarPendentesIntegracaoTrocaMotorista(numeroTentativas, minutosACadaTentativa);

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao in integracoes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.A52:
                        Servicos.Embarcador.Integracao.A52.IntegracaoA52 servicoIntegracaoA52 = new Integracao.A52.IntegracaoA52(unitOfWork);
                        servicoIntegracaoA52.IntegrarTrocaMotorista(integracao);
                        break;

                    default:

                        integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        integracao.DataIntegracao = DateTime.Now;
                        integracao.NumeroTentativas++;
                        integracao.ProblemaIntegracao = "Integração não implementada.";

                        break;
                }
            }
        }

        public void ValidarDataANTT(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo)
        {
            if (!configuracaoVeiculo.ObrigarANTTVeiculoValidarSalvarDadosTransporte || veiculo == null)
                return;

            if (!veiculo.DataValidadeANTT.HasValue || veiculo.DataValidadeANTT.Value.Date <= DateTime.Now.Date)
                throw new ServicoException($"O veículo ({veiculo.Placa}) informado está com a data de ANTT vencida.");
        }

        public static void AlterarSituacaoVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo situacao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, string mensagemAuditoria)
        {
            try
            {
                if (veiculo == null) return;

                Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(unitOfWork);
                if (veiculo.SituacaoVeiculo != situacao)
                {
                    Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo ultimaSituacao = repSituacaoVeiculo.BuscarUltimoPorVeiculo(veiculo.Codigo);

                    DateTime dataSituacao = DateTime.Now;
                    Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacaoVeiculo = new Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo();
                    situacaoVeiculo.Situacao = situacao;
                    situacaoVeiculo.DataHoraEmissao = dataSituacao;
                    situacaoVeiculo.DataHoraSituacao = dataSituacao;
                    situacaoVeiculo.Veiculo = veiculo;
                    situacaoVeiculo.Localidade = veiculo.LocalidadeAtual;
                    repSituacaoVeiculo.Inserir(situacaoVeiculo);

                    if (ultimaSituacao != null && ultimaSituacao.Situacao == SituacaoVeiculo.EmManutencao && !ultimaSituacao.DataHoraSaidaManutencao.HasValue)
                    {
                        ultimaSituacao.DataHoraSaidaManutencao = dataSituacao;
                        repSituacaoVeiculo.Atualizar(ultimaSituacao);
                    }

                    if (Auditado != null)
                    {
                        if (string.IsNullOrEmpty(mensagemAuditoria))
                        {
                            if (situacao == SituacaoVeiculo.Indisponivel)
                                mensagemAuditoria = "Indisponibilizou o veículo";
                            else if (situacao == SituacaoVeiculo.Disponivel)
                                mensagemAuditoria = "Disponibilizou o veículo";
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, situacaoVeiculo, null, mensagemAuditoria, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, mensagemAuditoria, unitOfWork);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void FinalizarSituacaoAbertaVeiculos(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Veiculo repVeiculo = new(unitOfWork);
            Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new(unitOfWork);
            List<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo> situacaoVeiculos = repSituacaoVeiculo.BuscarVeiculosComSituacaoEmManutencaoAberta();
            if (situacaoVeiculos.Count == 0) return;

            foreach (Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacaoVeiculo in situacaoVeiculos)
            {
                try
                {
                    unitOfWork.Start();

                    situacaoVeiculo.DataHoraSaidaManutencao = situacaoVeiculo.DataHoraPrevisaoSaidaManutencao;
                    repSituacaoVeiculo.Atualizar(situacaoVeiculo);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, situacaoVeiculo, null, "Finalizado por atingimento da Data de Prevista da Situação.", unitOfWork);

                    SituacaoVeiculo situacao = situacaoVeiculo.Veiculo.Ativo ? SituacaoVeiculo.Disponivel : SituacaoVeiculo.Indisponivel;
                    AlterarSituacaoVeiculo(situacaoVeiculo.Veiculo, situacao, unitOfWork, auditado, "Situação gerada por atingimento da data prevista da situação.");
                    situacaoVeiculo.Veiculo.SituacaoVeiculo = situacao;
                    situacaoVeiculo.Veiculo.DataHoraPrevisaoDisponivel = null;
                    repVeiculo.Atualizar(situacaoVeiculo.Veiculo);

                    unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    unitOfWork.Rollback();
                }
            }
        }

        public static void AtualizarHistoricoSituacaoVeiculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Veiculo repVeiculo = new(unitOfWork);
            Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new(unitOfWork);
            IList<int> codigosVeiculosDivergentes = repSituacaoVeiculo.BuscarVeiculosComSituacaoDivergente();
            if (codigosVeiculosDivergentes.Count == 0) return;

            List<Dominio.Entidades.Veiculo> veiculosDivergentes = repVeiculo.BuscarPorCodigo(codigosVeiculosDivergentes);
            foreach (Dominio.Entidades.Veiculo veiculo in veiculosDivergentes)
            {
                try
                {
                    unitOfWork.Start();

                    if (!veiculo.Ativo)
                        veiculo.SituacaoVeiculo = SituacaoVeiculo.Indisponivel;

                    Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculo, !veiculo.Ativo, MetodosAlteracaoVeiculo.AtualizarHistoricoSituacaoVeiculo_Veiculo, auditado?.Usuario, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, veiculo, "Atualizado histórico de Situacação do Veículo (Rotina Diaria de Validação de Divergencia de Status)", unitOfWork);

                    repVeiculo.Atualizar(veiculo);

                    unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    unitOfWork.Rollback();
                }
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private static void AjustarIntegracoesMultiEmbarcador(Dominio.Entidades.Veiculo veiculo, IList<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> integracoes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MultiEmbarcador);

            if (tipoIntegracao == null)
                return;

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoas = repGrupoPessoas.BuscarGruposComIntegracaoVeiculosMultiEmbarcador();

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas in gruposPessoas)
            {
                Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao = integracoes.Where(o => o.GrupoPessoas != null && o.GrupoPessoas.Codigo == grupoPessoas.Codigo).FirstOrDefault();

                if (integracao != null)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.NumeroTentativas = 0;
                    integracao.ProblemaIntegracao = string.Empty;

                    repVeiculoIntegracao.Atualizar(integracao);
                }
                else
                {
                    integracao = new Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao()
                    {
                        DataIntegracao = DateTime.Now,
                        Veiculo = veiculo,
                        ProblemaIntegracao = "",
                        GrupoPessoas = grupoPessoas,
                        SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                        TipoIntegracao = tipoIntegracao
                    };

                    repVeiculoIntegracao.Inserir(integracao);
                }
            }
        }

        private static void ValidarDataLiberacaoSeguradora(Dominio.Entidades.Veiculo veiculo)
        {
            if (!veiculo.DataValidadeLiberacaoSeguradora.HasValue)
                throw new ServicoException($"O veículo {veiculo.Placa} não possui uma data de limite da seguradora configurada, por isso não é possível informar o veículo para essa carga, verifique e tente novamente.");

            if (veiculo.DataValidadeLiberacaoSeguradora.Value.Date < DateTime.Now.Date)
                throw new ServicoException($"A data de limite do veículo {veiculo.Placa} na seguradora está válido até {veiculo.DataValidadeLiberacaoSeguradora.Value.ToString("dd/MM/yyyy")}, por isso não é possível informar este veículo para essa carga, verifique e tente novamente.");
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.TecnologiaRastreador ConverterObjetoTecnologiaRastreador(Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador)
        {
            if (tecnologiaRastreador == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Frota.TecnologiaRastreador()
            {
                Descricao = tecnologiaRastreador.Descricao,
                CodigoIntegracao = tecnologiaRastreador.CodigoIntegracao
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.TipoComunicacaoRastreador ConverterObjetoTipoComunicacaoRastreador(Dominio.Entidades.Embarcador.Veiculos.TipoComunicacaoRastreador tipoComunicacaoRastreador)
        {
            if (tipoComunicacaoRastreador == null)
                return null;

            return new Dominio.ObjetosDeValor.Embarcador.Frota.TipoComunicacaoRastreador()
            {
                Descricao = tipoComunicacaoRastreador.Descricao,
                CodigoIntegracao = tipoComunicacaoRastreador.CodigoIntegracao
            };
        }

        private static void GerarAtendimentoParaIntegracoesPendentesVeiculos(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamado repositorioChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repositorioChamado.BuscarPorIntegracao(integracao.TipoIntegracao.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

            if (motivoChamado == null)
                return;

            if (carga == null) return;

            Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objetoChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado()
            {
                Observacao = $"Atendimento gerado a partir da rejeição da integração com {integracao.TipoIntegracao?.Descricao} do veículo {integracao.Veiculo.Placa ?? string.Empty}",
                MotivoChamado = motivoChamado,
                Carga = carga,
                Empresa = carga.Empresa,
                Cliente = carga.Pedidos?.FirstOrDefault()?.Pedido?.Remetente,
                TipoCliente = configuracaoTMS.ChamadoOcorrenciaUsaRemetente ? Dominio.Enumeradores.TipoTomador.Remetente : Dominio.Enumeradores.TipoTomador.Destinatario
            };

            Dominio.Entidades.Usuario usuario = carga.Operador;

            if (usuario == null)
                return;

            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = Servicos.Embarcador.Chamado.Chamado.AbrirChamado(objetoChamado, usuario, 0, null, unitOfWork);
        }

        private static void GerarOuAtualizarIntegracaoCIOT(Dominio.Entidades.Veiculo veiculo, IList<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> integracoes, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = IntegrarComCIOT(veiculo, unitOfWork);

            if (configuracaoCIOT == null)
                return;

            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao = integracoes.FirstOrDefault(o => o.ConfiguracaoCIOT?.Codigo == configuracaoCIOT.Codigo);

            if (integracao != null && integracao.TipoIntegracao.Ativo)
            {
                // Atualizando Veículo
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.NumeroTentativas = 0;
                integracao.ProblemaIntegracao = "";

                repVeiculoIntegracao.Atualizar(integracao);

                return;
            }

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.CIOT);

            if (tipoIntegracao == null)
                return;

            // Inserindo Veículo
            integracao = new Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao()
            {
                DataIntegracao = DateTime.Now,
                Veiculo = veiculo,
                ProblemaIntegracao = "",
                ConfiguracaoCIOT = configuracaoCIOT,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                TipoIntegracao = tipoIntegracao
            };

            repVeiculoIntegracao.Inserir(integracao);
        }

        private static void GerarIntegracaoVeiculoBRK(Dominio.Entidades.Veiculo veiculo, IList<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> integracoes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repositorioConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repositorioConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

            if (!configuracaoVeiculo.CadastrarVeiculoMotoristaBRK)
                return;

            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao = integracoes.FirstOrDefault(o => o.TipoIntegracao.Tipo == TipoIntegracao.BrasilRiskVeiculoMotorista);

            if (integracao != null && integracao.TipoIntegracao.Ativo)
            {
                // Atualizando Veículo
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.NumeroTentativas = 0;
                integracao.ProblemaIntegracao = "";

                repVeiculoIntegracao.Atualizar(integracao);

                return;
            }

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.BrasilRiskVeiculoMotorista);

            if (tipoIntegracao == null)
                return;

            // Inserindo Veículo
            integracao = new Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao()
            {
                DataIntegracao = DateTime.Now,
                Veiculo = veiculo,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                TipoIntegracao = tipoIntegracao
            };

            repVeiculoIntegracao.Inserir(integracao);
        }

        #endregion Métodos Privados
    }
}
