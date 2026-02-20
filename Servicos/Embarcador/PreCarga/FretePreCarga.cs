using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Frete;
using Dominio.ObjetosDeValor.Embarcador.Frete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.PreCarga
{
    public class FretePreCarga : ServicoBase
    {
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware;

        #region Métodos Públicos

        public FretePreCarga(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware) : base(unitOfWork, tipoServicoMultisoftware)
        {
            TipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete ProcessarFrete(ref Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { };
            StringBuilder mensagemRetorno = new StringBuilder();

            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.ExcluirComposicoesFrete(preCarga, unitOfWork);

            if (SetarValorFreteNegociado(retorno, preCarga, unitOfWork, tipoServicoMultisoftware, configuracao))
                return retorno;

            TabelaFrete tabelaFrete = ObterTabelasFrete(preCarga, unitOfWork, TipoServicoMultisoftware, ref mensagemRetorno);

            if (tabelaFrete != null)
            {
                if (string.IsNullOrWhiteSpace(mensagemRetorno.ToString()))
                {
                    switch (tabelaFrete.TipoTabelaFrete)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente:
                            if (tabelaFrete.TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga)
                            {
                                if (CalcularFretePorCliente(ref retorno, ref preCarga, tabelaFrete, unitOfWork, configuracao))
                                    return retorno;
                            }
                            else
                            {
                                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                                retorno.mensagem = "Modelo de Tabela não está implementado para calculo de pré cargas";
                            }
                            break;
                        default:
                            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                            retorno.mensagem = "Modelo de Tabela não está implementado para calculo de pré cargas";
                            break;
                    }

                }
                else
                {
                    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                    retorno.mensagem = mensagemRetorno.ToString();
                }
            }
            else
            {
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                retorno.mensagem = "Não foi localizado nenhum modelo de tabela compativel com a pré carga";
            }


            return retorno;
        }

        public bool SetarValorFreteNegociado(Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS)
                return false;

            if (!preCarga.Pedidos.All(o => o.ValorFreteNegociado > 0m))
                return false;

            preCarga.ValorFrete = preCarga.Pedidos.Sum(o => o.ValorFreteNegociado);

            Servicos.Embarcador.Carga.RateioFrete svcRateioFrete = new Carga.RateioFrete(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in preCarga.Pedidos)
                svcRateioFrete.CalcularImpostos(ref preCarga, pedido, pedido.ValorFreteNegociado, false, tipoServicoMultisoftware, unitOfWork, configuracao);

            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;

            return true;
        }

        public bool CalcularFretePorCliente(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unitOfWork);

            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(unitOfWork);

            StringBuilder mensagemRetorno = new StringBuilder();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(preCarga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorCarga(tabelaFrete, preCarga, unitOfWork, unitOfWork.StringConexao, TipoServicoMultisoftware);

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, TipoServicoMultisoftware);

            if (tabelasCliente.Count <= 0 || tabelasCliente.Count > 1)
            {
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
                retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                retorno.mensagem = tabelasCliente.Count <= 0 ? mensagemRetorno.Insert(0, "Não foi localizada uma configuração de frete compatível com a tabela de frete " + tabelaFrete.Descricao + " para a pré carga.\n").ToString() : "Foi encontrada mais de uma configuração de frete disponível para a tabela de frete  " + tabelaFrete.Descricao + " na pré carga.";
                preCarga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);
                preCarga.TabelaFrete = null;
                preCarga.PendenciaCalculoFrete = true;
                repPreCarga.Atualizar(preCarga);
                return true;
            }
            else
            {
                retorno = svcFreteCliente.SetarTabelaFreteCarga(ref preCarga, parametrosCalculo, tabelasCliente[0], TipoServicoMultisoftware, configuracao);

            }

            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;

            return false;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFretePorCarga(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = preCarga.Pedidos.ToList();

            List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();


            origens = (from obj in pedidos where obj.Origem != null select obj.Origem).Distinct().ToList();
            destinos = (from obj in pedidos where obj.Destino != null select obj.Destino).Distinct().ToList();


            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                if (pedido.Destinatario != null && !destinatarios.Contains(pedido.Destinatario))
                    destinatarios.Add(pedido.Destinatario);

                if (pedido.Remetente != null && !destinatarios.Contains(pedido.Remetente))
                    remetentes.Add(pedido.Remetente);
            }

            decimal peso = tabelaFrete.UtilizarPesoLiquido ? pedidos.Sum(o => o.PesoLiquidoTotal) : pedidos.Sum(o => o.PesoTotal);
            int quantidadeNotasFiscais = pedidos.Count();
            decimal valorTotalNotasFiscais = pedidos.Sum(o => o.ValorTotalCarga);
            int quantidadeEntregas = pedidos.Count();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades();
            cargaPedidoQuantidade.Quantidade = peso;
            cargaPedidoQuantidade.Unidade = Dominio.Enumeradores.UnidadeMedida.KG;
            cargaPedidoQuantidades.Add(cargaPedidoQuantidade);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = new ParametrosCalculoFrete()
            {
                DataColeta = pedidos.Select(o => o.DataInicialColeta).Min(),
                DataFinalViagem = pedidos.Select(o => o.DataFinalViagemFaturada).Max(),
                DataInicialViagem = pedidos.Select(o => o.DataInicialViagemFaturada).Min(),
                DataVigencia = DateTime.Now,
                Desistencia = false,
                DespachoTransitoAduaneiro = pedidos.Any(o => o.DespachoTransitoAduaneiro),
                Destinatarios = destinatarios,
                Destinos = destinos,
                Distancia = preCarga.Distancia,
                Empresa = preCarga.Empresa,
                EscoltaArmada = pedidos.Any(o => o.EscoltaArmada),
                QuantidadeEscolta = pedidos.Where(o => o.EscoltaArmada).Sum(o => o.QtdEscolta),
                Filial = preCarga.Filial,
                PesoTotalCarga = preCarga?.DadosSumarizados?.PesoTotal ?? 0m,
                GerenciamentoRisco = pedidos.Any(o => o.GerenciamentoRisco),
                GrupoPessoas = pedidos.FirstOrDefault().ObterTomador()?.GrupoPessoas,
                ModeloVeiculo = preCarga.ModeloVeicularCarga,
                NecessarioReentrega = pedidos.Any(o => o.NecessarioReentrega),
                NumeroAjudantes = pedidos.Sum(o => o.QtdAjudantes),
                NumeroDeslocamento = pedidos.Sum(o => o.ValorDeslocamento ?? 0m),
                NumeroDiarias = pedidos.Sum(o => o.ValorDiaria ?? 0m),
                NumeroEntregas = quantidadeEntregas,
                NumeroPallets = pedidos.Sum(o => o.NumeroPaletes + o.NumeroPaletesFracionado),
                Origens = origens,
                PercentualDesistencia = 0,
                Peso = peso,
                PesoLiquido = pedidos.Sum(o => o.PesoLiquidoTotal),
                PesoCubado = pedidos.Sum(o => o.PesoCubado),
                PesoPaletizado = pedidos.Sum(o => o.PesoTotalPaletes),
                PossuiRestricaoTrafego = remetentes.Any(o => o.PossuiRestricaoTrafego) || destinatarios.Any(o => o.PossuiRestricaoTrafego),
                QuantidadeNotasFiscais = quantidadeNotasFiscais,
                Quantidades = (from obj in cargaPedidoQuantidades
                               select new ParametrosCalculoFreteQuantidade()
                               {
                                   Quantidade = obj.Quantidade,
                                   UnidadeMedida = obj.Unidade
                               }).ToList(),
                Rastreado = pedidos.Any(o => o.Rastreado),
                Remetentes = remetentes,
                Rota = preCarga.Rota,
                RotasDinamicas = pedidos.Where(o => o.RotaFrete != null).Select(o => o.RotaFrete).ToList(),
                TipoCarga = preCarga.TipoDeCarga,
                TipoOperacao = pedidos.FirstOrDefault().TipoOperacao,
                Tomador = pedidos.FirstOrDefault().ObterTomador(),
                ValorNotasFiscais = valorTotalNotasFiscais,
                Veiculo = preCarga.Veiculo,
                Reboques = preCarga.VeiculosVinculados?.ToList(),
                Volumes = (from obj in cargaPedidoQuantidades where obj.Unidade == Dominio.Enumeradores.UnidadeMedida.UN select obj.Quantidade).Sum(),
                DataBaseCRT = pedidos.Where(o => o.DataBaseCRT.HasValue).Select(o => o.DataBaseCRT).FirstOrDefault(),
                Fronteiras = preCarga.Rota?.Fronteiras?.Select(o => o.Cliente).ToList() ?? null,
                CargaPerigosa = preCarga.Carga?.CargaPerigosaIntegracaoLeilao ?? false,
            };

            return parametrosCalculoFrete;
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFrete ObterTabelasFrete(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref StringBuilder mensagem, Dominio.Entidades.Cliente tomador = null)
        {
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = null;

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorPreCarga(preCarga.Codigo).FirstOrDefault(); //preCarga.Pedidos.FirstOrDefault();

            if (pedido != null)
            {
                if (tomador == null)
                    tomador = pedido?.ObterTomador();

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = pedido?.TipoOperacao;

                ParametrosRetornarTabelasValidas parametrosRetornarTabelasValidas = new ParametrosRetornarTabelasValidas()
                {
                    CodigoEmpresa = preCarga.Empresa?.Codigo ?? 0,
                    CodigoFilial = preCarga.Filial?.Codigo ?? 0,
                    CodigoGrupoPessoaTomador = tomador.GrupoPessoas?.Codigo ?? 0,
                    CodigoTipoCarga = preCarga.TipoDeCarga?.Codigo ?? 0,
                    CodigoTipoOperacao = tipoOperacao?.Codigo ?? 0,
                    DataVigencia = DateTime.Now.Date,
                    CodigoModeloVeicularDaCarga = preCarga.ModeloVeicularCarga?.Codigo ?? 0,
                    CodigoModeloVeicularDoVeiculo = preCarga.Veiculo?.ModeloVeicularCarga?.Codigo ?? 0,
                    RetornarPrimeiraValida = true
                };

                if (preCarga.Empresa != null)
                {
                    tabelasFrete = repTabelaFrete.BuscarPorEmpresa(preCarga.Empresa.Codigo, false, false, false);
                    if (tabelasFrete.Count > 0)
                        tabelaFrete = Servicos.Embarcador.Carga.Frete.RetornarTabelaValida(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
                }

                if (tabelaFrete == null && preCarga.Filial != null)
                {
                    tabelasFrete = repTabelaFrete.BuscarPorFilial(preCarga.Filial.Codigo, false, false, false);
                    if (tabelasFrete.Count > 0)
                        tabelaFrete = Servicos.Embarcador.Carga.Frete.RetornarTabelaValida(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);

                }
                if (tabelaFrete == null && tipoOperacao != null)
                {
                    tabelasFrete = repTabelaFrete.BuscarPorTipoOperacao(tipoOperacao.Codigo, false, false, false);
                    if (tabelasFrete.Count > 0)
                        tabelaFrete = Servicos.Embarcador.Carga.Frete.RetornarTabelaValida(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
                }

                if (tabelaFrete == null && tomador.GrupoPessoas != null)
                {
                    tabelasFrete = repTabelaFrete.BuscarPorGrupoPessoas(tomador.GrupoPessoas.Codigo, false, false, false);
                    if (tabelasFrete.Count > 0)
                        tabelaFrete = Servicos.Embarcador.Carga.Frete.RetornarTabelaValida(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
                }

                if (tabelaFrete == null)
                {
                    tabelaFrete = repTabelaFrete.BuscarPadrao(false);
                    if (tabelaFrete != null)
                    {
                        tabelasFrete.Clear();
                        tabelasFrete.Add(tabelaFrete);
                        tabelaFrete = Servicos.Embarcador.Carga.Frete.RetornarTabelaValida(tabelasFrete, parametrosRetornarTabelasValidas, tipoServicoMultisoftware, unidadeDeTrabalho);
                    }
                }

                return tabelaFrete;

            }
            else
            {
                return null;
            }

        }

        #endregion
    }
}
