using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public class RegraPlanejamentoFrota : ServicoBase
   {        
        public RegraPlanejamentoFrota(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos públicos
        public Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoRegraPlanejamentoFrota BuscarRegraPlanejamentoFrota(Repositorio.UnitOfWork unitOfWork, DateTime dataAtual, int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota repRegraPlanejamentoFrota = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras = repRegraPlanejamentoFrota.BuscarRegrasAtivas(dataAtual);
            if (regras == null || regras.Count == 0)
                return null;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            bool localizadoRegraFiltrada = false;
            Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regra = null;
            List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regrasFiltradas = new List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota>();

            if (carga.DadosSumarizados.ValorTotalProdutos > 0)
            {
                regrasFiltradas = FiltrarPorValorMercadoria(carga.DadosSumarizados.ValorTotalProdutos, regras);
                if (regrasFiltradas != null && regrasFiltradas.Count > 0)
                {
                    regras = regrasFiltradas;
                    localizadoRegraFiltrada = true;
                }
            }

            regrasFiltradas = FiltrarPorCidadeOrigem(repCargaPedido.BuscarCodigosOrigem(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorCidadeDestino(repCargaPedido.BuscarCodigosDestino(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorClienteOrigem(repCargaPedido.BuscarCNPJsClienteOrigem(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorClienteDestino(repCargaPedido.BuscarCNPJsClienteDestino(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorEstadoOrigem(repCargaPedido.BuscarUFsOrigem(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorEstadoDestino(repCargaPedido.BuscarUFsDestino(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorRegiaoOrigem(repCargaPedido.BuscarCodigosRegiaoOrigem(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorRegiaoDestino(repCargaPedido.BuscarCodigosRegiaoDestino(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorRotaOrigem(repCargaPedido.BuscarCodigosRotaFrete(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorRotaDestino(repCargaPedido.BuscarCodigosRotaFrete(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorCEPOrigem(repCargaPedido.BuscarCEPsClienteOrigem(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorCEPDestino(repCargaPedido.BuscarCEPsClienteDestino(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorPaisOrigem(repCargaPedido.BuscarCodigosPaisOrigem(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorPaisDestino(repCargaPedido.BuscarCodigosPaisDestino(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            if (carga.GrupoPessoaPrincipal != null)
            {
                regrasFiltradas = FiltrarPorGrupoPessoa(carga.GrupoPessoaPrincipal.Codigo, regras);
                if (regrasFiltradas != null && regrasFiltradas.Count > 0)
                {
                    regras = regrasFiltradas;
                    localizadoRegraFiltrada = true;
                }
            }
            else
            {
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo).FirstOrDefault();
                if (pedidoXMLNotaFiscal != null)
                {
                    if(pedidoXMLNotaFiscal.XMLNotaFiscal?.Destinatario?.GrupoPessoas != null)
                    {
                        regrasFiltradas = FiltrarPorGrupoPessoa(pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.GrupoPessoas.Codigo, regras);
                        if (regrasFiltradas != null && regrasFiltradas.Count > 0)
                        {
                            regras = regrasFiltradas;
                            localizadoRegraFiltrada = true;
                        }
                    }
                }
            }

            if (carga.TipoOperacao != null)
            {
                regrasFiltradas = FiltrarPorTipoOperacao(carga.TipoOperacao.Codigo, regras);
                if (regrasFiltradas != null && regrasFiltradas.Count > 0)
                {
                    regras = regrasFiltradas;
                    localizadoRegraFiltrada = true;
                }
            }

            if (carga.TipoDeCarga != null)
            {
                regrasFiltradas = FiltrarPorTipoCarga(carga.TipoDeCarga.Codigo, regras);
                if (regrasFiltradas != null && regrasFiltradas.Count > 0)
                {
                    regras = regrasFiltradas;
                    localizadoRegraFiltrada = true;
                }
            }

            regrasFiltradas = FiltrarPorCentroResultado(repCargaPedido.BuscarCodigosCentroResultado(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            regrasFiltradas = FiltrarPorModeloVeicularCarga(repCargaPedido.BuscarCodigosModeloVeicularCarga(codigoCarga), regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoa = carga.Veiculo?.Proprietario?.Modalidades != null ? carga.Veiculo?.Proprietario.Modalidades.Where(o => o.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro).FirstOrDefault() : null;
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = modalidadePessoa != null ? repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidadePessoa.Codigo) : null;

            List<int> codigosTerceiro = null;
            if (modalidadeTerceiro?.TipoTerceiro != null)
            {
                codigosTerceiro = new List<int>();
                codigosTerceiro.Add(modalidadeTerceiro.TipoTerceiro.Codigo);
            }
            
            regrasFiltradas = FiltrarPorNivelCooperado(codigosTerceiro, regras);
            if (regrasFiltradas != null && regrasFiltradas.Count > 0)
            {
                regras = regrasFiltradas;
                localizadoRegraFiltrada = true;
            }

            //vai pegar a primeira caso tenha regras duplicadas
            if (localizadoRegraFiltrada && regras != null && regras.Count > 0)
                regra = regras.FirstOrDefault();

            if (regra == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoRegraPlanejamentoFrota retorno = new Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoRegraPlanejamentoFrota()
            {
                ApenasComInformacoesDeEscoltaInformadaNoPedido = regra.ApenasComInformacoesDeEscoltaInformadaNoPedido,
                ApenasComInformacoesDeIscaInformadaNoPedido = regra.ApenasComInformacoesDeIscaInformadaNoPedido,
                ApenasReboqueComIdadeMaxima = regra.ApenasReboqueComIdadeMaxima,
                ApenasTracaoComIdadeMaxima = regra.ApenasTracaoComIdadeMaxima,
                ApenasVeiculoQuePossuiImobilizador = regra.ApenasVeiculoQuePossuiImobilizador,
                ApenasVeiculoQuePossuiTravaQuintaRoda = regra.ApenasVeiculoQuePossuiTravaQuintaRoda,
                ApenasVeiculosComRastreadorAtivo = regra.ApenasVeiculosComRastreadorAtivo,
                CategoriasHabilitacao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao>(),
                CondicaoLicencas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca>(),
                CondicaoLiberacoesGR = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLiberacaoGR>(),
                Descricao = regra.Descricao,
                IdadeMaximaReboque = regra.IdadeMaximaReboque,
                IdadeMaximaTracao = regra.IdadeMaximaTracao,
                Licencas = new List<Dominio.ObjetosDeValor.WebService.Configuracoes.Licenca>(),
                LiberacoesGR = new List<Dominio.ObjetosDeValor.WebService.Configuracoes.LiberacaoGR>(),
                LiberacoesGRVeiculo = new List<Dominio.ObjetosDeValor.WebService.Configuracoes.LiberacaoGR>(),
                LimitarPelaAlturaCarreta = regra.LimitarPelaAlturaCarreta,
                LimitarPelaAlturaCavalo = regra.LimitarPelaAlturaCavalo,
                MetrosAlturaCarreta = regra.MetrosAlturaCarreta,
                MetrosAlturaCavalo = regra.MetrosAlturaCavalo,
                ModelosVeicularCargaReboque = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>(),
                ModelosVeicularCargaTracao = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular>(),
                Numero = regra.NumeroSequencial,
                QuantidadeEscolta = regra.QuantidadeEscolta,
                QuantidadeIsca = regra.QuantidadeIsca,
                TecnologiaRastreadores = new List<Dominio.ObjetosDeValor.Embarcador.Frota.TecnologiaRastreador>(),
                TiposCarroceria = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria>(),
                TiposPropriedade = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedade>(),
                TiposProprietarioVeiculo = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo>(),
                TiposRodado = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado>(),
                QuantidadeCarga = regra.QuantidadeCarga,
                PeriodoQuantidadeCarga = regra.PeriodoQuantidadeCarga,
                TipoPeriodoQuantidadeCarga = regra.TipoPeriodoQuantidadeCarga,
                ValidarQuantidadeVeiculoEReboque = regra.ValidarQuantidadeVeiculoEReboque,
                ValidarPorQuantidadeMotorista = regra.ValidarPorQuantidadeMotorista
            };

            if (regra.CategoriasHabilitacao != null && regra.CategoriasHabilitacao.Count > 0)
            {
                foreach (var categoria in regra.CategoriasHabilitacao)                
                    retorno.CategoriasHabilitacao.Add(categoria);                
            }
            if (regra.CondicaoLicencas != null && regra.CondicaoLicencas.Count > 0)
            {
                foreach (var licenca in regra.CondicaoLicencas)
                    retorno.CondicaoLicencas.Add(licenca);
            }
            if (regra.CondicaoLiberacoesGR != null && regra.CondicaoLiberacoesGR.Count > 0)
            {
                foreach (var LiberacoesGR in regra.CondicaoLiberacoesGR)
                    retorno.CondicaoLiberacoesGR.Add(LiberacoesGR);
            }
            if (regra.TiposCarroceria != null && regra.TiposCarroceria.Count > 0)
            {
                foreach (var tiposCarroceria in regra.TiposCarroceria)
                    retorno.TiposCarroceria.Add(tiposCarroceria);
            }
            if (regra.TiposPropriedade != null && regra.TiposPropriedade.Count > 0)
            {
                foreach (var tiposPropriedade in regra.TiposPropriedade)
                    retorno.TiposPropriedade.Add(tiposPropriedade);
            }
            if (regra.TiposProprietarioVeiculo != null && regra.TiposProprietarioVeiculo.Count > 0)
            {
                foreach (var tiposProprietarioVeiculo in regra.TiposProprietarioVeiculo)
                    retorno.TiposProprietarioVeiculo.Add(tiposProprietarioVeiculo);
            }
            if (regra.TiposRodado != null && regra.TiposRodado.Count > 0)
            {
                foreach (var tiposRodado in regra.TiposRodado)
                    retorno.TiposRodado.Add(tiposRodado);
            }
            if (regra.Licencas != null)
            {
                foreach (var licenca in regra.Licencas)
                {
                    retorno.Licencas.Add(new Dominio.ObjetosDeValor.WebService.Configuracoes.Licenca()
                    {
                        Codigo = licenca.Codigo.ToString(),
                        Descricao = licenca.Descricao,
                        CodigoInterno = licenca.Codigo
                    });
                }
            }

            if (regra.LiberacoesGR != null)
            {
                foreach (var LiberacaoGR in regra.LiberacoesGR)
                {
                    retorno.LiberacoesGR.Add(new Dominio.ObjetosDeValor.WebService.Configuracoes.LiberacaoGR()
                    {
                        Codigo = LiberacaoGR.Codigo.ToString(),
                        Descricao = LiberacaoGR.Descricao,
                        CodigoInterno = LiberacaoGR.Codigo
                    });
                }
            }

            if (regra.LiberacoesGRVeiculo != null)
            {
                foreach (var LiberacaoGR in regra.LiberacoesGRVeiculo)
                {
                    retorno.LiberacoesGRVeiculo.Add(new Dominio.ObjetosDeValor.WebService.Configuracoes.LiberacaoGR()
                    {
                        Codigo = LiberacaoGR.Codigo.ToString(),
                        Descricao = LiberacaoGR.Descricao,
                        CodigoInterno = LiberacaoGR.Codigo
                    });
                }
            }

            if (regra.ModelosVeicularCargaReboque != null)
            {
                foreach (var modelo in regra.ModelosVeicularCargaReboque)
                {
                    retorno.ModelosVeicularCargaReboque.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular()
                    {
                        CodigoIntegracao = modelo.Codigo.ToString(),
                        Descricao = modelo.Descricao,
                        CodigoInterno = modelo.Codigo
                    });
                }
            }

            if (regra.ModelosVeicularCargaTracao != null)
            {
                foreach (var modelo in regra.ModelosVeicularCargaTracao)
                {
                    retorno.ModelosVeicularCargaTracao.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular()
                    {
                        CodigoIntegracao = modelo.Codigo.ToString(),
                        Descricao = modelo.Descricao,
                        CodigoInterno = modelo.Codigo
                    });
                }
            }

            if (regra.TecnologiaRastreadores != null)
            {
                foreach (var tecnologia in regra.TecnologiaRastreadores)
                {
                    retorno.TecnologiaRastreadores.Add(new Dominio.ObjetosDeValor.Embarcador.Frota.TecnologiaRastreador()
                    {
                        CodigoIntegracao = tecnologia.Codigo.ToString(),
                        Descricao = tecnologia.Descricao,
                        CodigoInterno = tecnologia.Codigo
                    });
                }
            }

            return retorno;
        }

        public bool ValidarRegraPlanjamentoFrota(Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoRegraPlanejamentoFrota retornoRegraPlanejamento, Repositorio.UnitOfWork unitOfWork, DateTime dataAtual, int codigoCarga, out string msgRetorno)
        {
            msgRetorno = "";
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = null;

            string validacoesRegraPlanejamento = "";
            //validações bool
            if (retornoRegraPlanejamento.ApenasVeiculosComRastreadorAtivo && (carga.Veiculo == null || carga.Veiculo?.PossuiRastreador != true))
                validacoesRegraPlanejamento += "- Aceito apenas com tração com rastreador ativo; <br />";
            if (retornoRegraPlanejamento.ApenasVeiculoQuePossuiTravaQuintaRoda && (carga.Veiculo == null || carga.Veiculo?.PossuiTravaQuintaDeRoda != true))
                validacoesRegraPlanejamento += "- Aceito apenas com tração com trava na quinta roda; <br />";
            if (retornoRegraPlanejamento.ApenasVeiculoQuePossuiImobilizador && (carga.Veiculo == null || carga.Veiculo?.PossuiImobilizador != true))
                validacoesRegraPlanejamento += "- Aceito apenas com tração com imobilizador; <br />";

            //validações bool com int
            if (retornoRegraPlanejamento.ApenasTracaoComIdadeMaxima && retornoRegraPlanejamento.IdadeMaximaTracao > 0 && (carga.Veiculo == null || carga.Veiculo.AnoFabricacao == 0 || ((DateTime.Now.Year - carga.Veiculo.AnoFabricacao) > retornoRegraPlanejamento.IdadeMaximaTracao)))
                validacoesRegraPlanejamento += "- Aceito apenas com tração com idade máxima de " + retornoRegraPlanejamento.IdadeMaximaTracao + "; <br />";
            if (retornoRegraPlanejamento.ApenasReboqueComIdadeMaxima && retornoRegraPlanejamento.IdadeMaximaReboque > 0)
            {
                if (carga.VeiculosVinculados == null || carga.VeiculosVinculados.Count == 0)
                    validacoesRegraPlanejamento += "- Aceito apenas com reboque com idade máxima de " + retornoRegraPlanejamento.IdadeMaximaReboque + "; <br />";
                else
                {
                    foreach (var veiculo in carga.VeiculosVinculados)
                    {
                        if (veiculo.AnoFabricacao == 0 || ((DateTime.Now.Year - veiculo.AnoFabricacao) > retornoRegraPlanejamento.IdadeMaximaReboque))
                        {
                            validacoesRegraPlanejamento += "- Aceito apenas com reboque com idade máxima de " + retornoRegraPlanejamento.IdadeMaximaReboque + "; <br />";
                            break;
                        }
                    }
                }
            }
            if (retornoRegraPlanejamento.LimitarPelaAlturaCarreta && retornoRegraPlanejamento.MetrosAlturaCarreta > 0)
            {
                if (carga.VeiculosVinculados == null || carga.VeiculosVinculados.Count == 0)
                    validacoesRegraPlanejamento += "- Aceito apenas com carreta com altura máxima de " + retornoRegraPlanejamento.MetrosAlturaCarreta.ToString("n2") + "; <br />";
                else
                {
                    foreach (var veiculo in carga.VeiculosVinculados)
                    {
                        if (veiculo.Modelo == null || veiculo.Modelo.AlturaEmMetros == 0 || veiculo.Modelo.AlturaEmMetros > retornoRegraPlanejamento.MetrosAlturaCarreta)
                        {
                            validacoesRegraPlanejamento += "- Aceito apenas com carreta com altura máxima de " + retornoRegraPlanejamento.MetrosAlturaCarreta.ToString("n2") + "; <br />";
                            break;
                        }
                    }
                }
            }
            if (retornoRegraPlanejamento.ApenasComInformacoesDeIscaInformadaNoPedido && retornoRegraPlanejamento.QuantidadeIsca > 0 && carga.Pedidos.Sum(p => p.Pedido.QtdIsca) != retornoRegraPlanejamento.QuantidadeIsca)
                validacoesRegraPlanejamento += "- Aceito apenas com a quantidade de isca correta informada no pedido: " + retornoRegraPlanejamento.QuantidadeIsca + "; <br />";
            if (retornoRegraPlanejamento.ApenasComInformacoesDeEscoltaInformadaNoPedido && retornoRegraPlanejamento.QuantidadeEscolta > 0 && carga.Pedidos.Sum(p => p.Pedido.QtdEscolta) != retornoRegraPlanejamento.QuantidadeEscolta)
                validacoesRegraPlanejamento += "- Aceito apenas com a quantidade de escolta correta informada no pedido: " + retornoRegraPlanejamento.QuantidadeEscolta + "; <br />";
            if (retornoRegraPlanejamento.LimitarPelaAlturaCavalo && retornoRegraPlanejamento.MetrosAlturaCavalo > 0 && (carga.Veiculo == null || carga.Veiculo.Modelo == null || carga.Veiculo.Modelo.AlturaEmMetros == 0 || carga.Veiculo.Modelo.AlturaEmMetros > retornoRegraPlanejamento.MetrosAlturaCavalo))
                validacoesRegraPlanejamento += "- Aceito apenas com tração com altura máxima de " + retornoRegraPlanejamento.MetrosAlturaCavalo.ToString("n2") + "; <br />";

            //validações com enum
            if (retornoRegraPlanejamento.TiposPropriedade != null && retornoRegraPlanejamento.TiposPropriedade.Count > 0)
            {
                if (carga.Veiculo == null)
                    validacoesRegraPlanejamento += "- Obrigatório informar uma tração; <br />";
                else
                {
                    if (carga.Veiculo.Tipo == "P" && !retornoRegraPlanejamento.TiposPropriedade.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedade.FrotaPropria))
                        validacoesRegraPlanejamento += "- Não é aceito tração de frota própria; <br />";
                    if (carga.Veiculo.Tipo == "T" && !retornoRegraPlanejamento.TiposPropriedade.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedade.Agregado || c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedade.Terceiros))
                        validacoesRegraPlanejamento += "- Não é aceito tração de frota terceiro/agregado; <br />";
                }
            }
            if (retornoRegraPlanejamento.TiposCarroceria != null && retornoRegraPlanejamento.TiposCarroceria.Count > 0)
            {
                if (carga.VeiculosVinculados == null || carga.VeiculosVinculados.Count == 0)
                    validacoesRegraPlanejamento += "- Obrigatório informar uma carroceria; <br />";
                else
                {
                    foreach (var veiculo in carga.VeiculosVinculados)
                    {
                        if (veiculo.TipoCarroceria == "00" && !retornoRegraPlanejamento.TiposCarroceria.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria.NaoAplicavel))
                            validacoesRegraPlanejamento += "- Não é aceito carroceria do tipo Não Aplicável; <br />";
                        else if (veiculo.TipoCarroceria == "01" && !retornoRegraPlanejamento.TiposCarroceria.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria.Aberta))
                            validacoesRegraPlanejamento += "- Não é aceito carroceria do tipo Aberta; <br />";
                        else if (veiculo.TipoCarroceria == "02" && !retornoRegraPlanejamento.TiposCarroceria.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria.FechadaBau))
                            validacoesRegraPlanejamento += "- Não é aceito carroceria do tipo Fechada/Bau; <br />";
                        else if (veiculo.TipoCarroceria == "03" && !retornoRegraPlanejamento.TiposCarroceria.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria.Graneleira))
                            validacoesRegraPlanejamento += "- Não é aceito carroceria do tipo Granel; <br />";
                        else if (veiculo.TipoCarroceria == "04" && !retornoRegraPlanejamento.TiposCarroceria.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria.PortaContainer))
                            validacoesRegraPlanejamento += "- Não é aceito carroceria do tipo Porta Container; <br />";
                        else if (veiculo.TipoCarroceria == "05" && !retornoRegraPlanejamento.TiposCarroceria.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria.Sider))
                            validacoesRegraPlanejamento += "- Não é aceito carroceria do tipo Sider; <br />";
                    }
                }
            }

            if (retornoRegraPlanejamento.TiposProprietarioVeiculo != null && retornoRegraPlanejamento.TiposProprietarioVeiculo.Count > 0)
            {
                if (carga.Veiculo == null)
                {
                    validacoesRegraPlanejamento += "- Obrigatório informar uma tração; <br />";
                }
                else if (!retornoRegraPlanejamento.TiposProprietarioVeiculo.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.NaoAplicado))
                {
                    if (carga.Veiculo.TipoProprietario == Dominio.Enumeradores.TipoProprietarioVeiculo.Outros && !retornoRegraPlanejamento.TiposProprietarioVeiculo.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.Outros))
                        validacoesRegraPlanejamento += "- Não é aceito a propriedade do veículo como Outros; <br />";
                    else if (carga.Veiculo.TipoProprietario == Dominio.Enumeradores.TipoProprietarioVeiculo.TACAgregado && !retornoRegraPlanejamento.TiposProprietarioVeiculo.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado))
                        validacoesRegraPlanejamento += "- Não é aceito a propriedade do veículo como TAC Agregado; <br />";
                    else if (carga.Veiculo.TipoProprietario == Dominio.Enumeradores.TipoProprietarioVeiculo.TACIndependente && !retornoRegraPlanejamento.TiposProprietarioVeiculo.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACIndependente))
                        validacoesRegraPlanejamento += "- Não é aceito a propriedade do veículo como TAC Independente; <br />";
                }
            }
            if (retornoRegraPlanejamento.CategoriasHabilitacao != null && retornoRegraPlanejamento.CategoriasHabilitacao.Count > 0)
            {
                if (carga.Motoristas == null || carga.Motoristas.Count == 0)
                    validacoesRegraPlanejamento += "- Obrigatório informar um motorista; <br />";
                else
                {
                    foreach (var motorista in carga.Motoristas)
                    {
                        if (motorista.Categoria == "A" && !retornoRegraPlanejamento.CategoriasHabilitacao.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao.CategoriaA))
                            validacoesRegraPlanejamento += "- Não é aceito a motorista com a categoria A; <br />";
                        if (motorista.Categoria == "B" && !retornoRegraPlanejamento.CategoriasHabilitacao.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao.CategoriaB))
                            validacoesRegraPlanejamento += "- Não é aceito a motorista com a categoria B; <br />";
                        if (motorista.Categoria == "C" && !retornoRegraPlanejamento.CategoriasHabilitacao.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao.CategoriaC))
                            validacoesRegraPlanejamento += "- Não é aceito a motorista com a categoria C; <br />";
                        if (motorista.Categoria == "D" && !retornoRegraPlanejamento.CategoriasHabilitacao.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao.CategoriaD))
                            validacoesRegraPlanejamento += "- Não é aceito a motorista com a categoria D; <br />";
                        if (motorista.Categoria == "E" && !retornoRegraPlanejamento.CategoriasHabilitacao.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao.CategoriaE))
                            validacoesRegraPlanejamento += "- Não é aceito a motorista com a categoria E; <br />";
                        if (motorista.Categoria == "AB" && !retornoRegraPlanejamento.CategoriasHabilitacao.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao.CategoriaAB))
                            validacoesRegraPlanejamento += "- Não é aceito a motorista com a categoria AB; <br />";
                        if (motorista.Categoria == "AC" && !retornoRegraPlanejamento.CategoriasHabilitacao.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao.CategoriaAC))
                            validacoesRegraPlanejamento += "- Não é aceito a motorista com a categoria AC; <br />";
                        if (motorista.Categoria == "AD" && !retornoRegraPlanejamento.CategoriasHabilitacao.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao.CategoriaAD))
                            validacoesRegraPlanejamento += "- Não é aceito a motorista com a categoria AD; <br />";
                        if (motorista.Categoria == "AE" && !retornoRegraPlanejamento.CategoriasHabilitacao.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao.CategoriaAE))
                            validacoesRegraPlanejamento += "- Não é aceito a motorista com a categoria AE; <br />";
                        if (string.IsNullOrWhiteSpace(motorista.Categoria))
                            validacoesRegraPlanejamento += "- Obrigatório informar a CNH do motorista; <br />";
                    }
                }
            }
            if (retornoRegraPlanejamento.TiposRodado != null && retornoRegraPlanejamento.TiposRodado.Count > 0)
            {
                if (carga.Veiculo == null)
                    validacoesRegraPlanejamento += "- Obrigatório informar uma tração; <br />";
                else
                {
                    if (carga.Veiculo.TipoRodado == "00" && !retornoRegraPlanejamento.TiposRodado.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.NaoAplicavel))
                        validacoesRegraPlanejamento += "- Não é aceito tração com o tipo Não Aplicável; <br />";
                    if (carga.Veiculo.TipoRodado == "01" && !retornoRegraPlanejamento.TiposRodado.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.Truck))
                        validacoesRegraPlanejamento += "- Não é aceito tração com o tipo Truck; <br />";
                    if (carga.Veiculo.TipoRodado == "02" && !retornoRegraPlanejamento.TiposRodado.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.Toco))
                        validacoesRegraPlanejamento += "- Não é aceito tração com o tipo Toco; <br />";
                    if (carga.Veiculo.TipoRodado == "03" && !retornoRegraPlanejamento.TiposRodado.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.CavaloMecanico))
                        validacoesRegraPlanejamento += "- Não é aceito tração com o tipo Cavalo; <br />";
                    if (carga.Veiculo.TipoRodado == "04" && !retornoRegraPlanejamento.TiposRodado.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.VAN))
                        validacoesRegraPlanejamento += "- Não é aceito tração com o tipo Van; <br />";
                    if (carga.Veiculo.TipoRodado == "05" && !retornoRegraPlanejamento.TiposRodado.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.Utilitario))
                        validacoesRegraPlanejamento += "- Não é aceito tração com o tipo Utilitário; <br />";
                    if (carga.Veiculo.TipoRodado == "06" && !retornoRegraPlanejamento.TiposRodado.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado.Outros))
                        validacoesRegraPlanejamento += "- Não é aceito tração com o tipo Outros; <br />";
                }
            }

            //validações com entidades
            if (retornoRegraPlanejamento.ModelosVeicularCargaTracao != null && retornoRegraPlanejamento.ModelosVeicularCargaTracao.Count > 0)
            {
                if (carga.Veiculo == null || carga.Veiculo.ModeloVeicularCarga == null)
                    validacoesRegraPlanejamento += "- Obrigatório informar um modelo veícular na tração; <br />";
                else if (!retornoRegraPlanejamento.ModelosVeicularCargaTracao.Any(c => c.CodigoInterno == carga.Veiculo.ModeloVeicularCarga.Codigo))
                    validacoesRegraPlanejamento += "- Não é aceito tração com o modelo veícular " + carga.Veiculo.ModeloVeicularCarga.Descricao + "; <br />";
            }
            if (retornoRegraPlanejamento.ModelosVeicularCargaReboque != null && retornoRegraPlanejamento.ModelosVeicularCargaReboque.Count > 0)
            {
                if (carga.VeiculosVinculados == null || carga.VeiculosVinculados.Count == 0)
                    validacoesRegraPlanejamento += "- Obrigatório informar um reboque; <br />";
                else
                {
                    foreach (var veiculo in carga.VeiculosVinculados)
                    {
                        if (veiculo.ModeloVeicularCarga == null)
                            validacoesRegraPlanejamento += "- Obrigatório informar um modelo veícular no reboque; <br />";
                        else if (!retornoRegraPlanejamento.ModelosVeicularCargaReboque.Any(c => c.CodigoInterno == veiculo.ModeloVeicularCarga.Codigo))
                            validacoesRegraPlanejamento += "- Não é aceito um reboque com o modelo veícular " + veiculo.ModeloVeicularCarga.Descricao + "; <br />";
                    }
                }
            }
            if (retornoRegraPlanejamento.TecnologiaRastreadores != null && retornoRegraPlanejamento.TecnologiaRastreadores.Count > 0)
            {
                if (carga.Veiculo == null || carga.Veiculo.TecnologiaRastreador == null)
                    validacoesRegraPlanejamento += "- Obrigatório informar uma tecnologia no rastreado na tração; <br />";
                else if (!retornoRegraPlanejamento.TecnologiaRastreadores.Any(c => c.CodigoInterno == carga.Veiculo.TecnologiaRastreador.Codigo))
                    validacoesRegraPlanejamento += "- Não é aceito tração com a tecnologia de rastreado " + carga.Veiculo.TecnologiaRastreador.Descricao + "; <br />";
            }

            bool contemLicencaVencidaMotorista = false;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca condicaoLicenca = retornoRegraPlanejamento.CondicaoLicencas != null && retornoRegraPlanejamento.CondicaoLicencas.Count > 0 ? retornoRegraPlanejamento.CondicaoLicencas.FirstOrDefault() : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca.E;
            if (retornoRegraPlanejamento.Licencas != null && retornoRegraPlanejamento.Licencas.Count > 0)
            {
                string validacoesRegraLicenca = "";
                bool contemLicencaVencidaTracao = false;
                bool contemLicencaVencidaReboque = false;                

                foreach (var licenca in retornoRegraPlanejamento.Licencas)
                {
                    if (carga.Veiculo != null && carga.Veiculo.LicencasVeiculo != null && carga.Veiculo.LicencasVeiculo.Count > 0 && carga.Veiculo.LicencasVeiculo.Any(c => c.Licenca?.Codigo == licenca.CodigoInterno && c.DataVencimento.HasValue))
                    {
                        if (carga.Veiculo.LicencasVeiculo.Any(c => c.Licenca.Codigo == licenca.CodigoInterno && c.DataVencimento.Value <= dataAtual) && !carga.Veiculo.LicencasVeiculo.Any(c => c.Licenca?.Codigo == licenca.CodigoInterno && c.DataVencimento.Value > dataAtual))
                        {
                            contemLicencaVencidaTracao = true;
                            validacoesRegraLicenca += "- Licença da tração " + licenca.Descricao + " se encontra vencida; <br />";
                        }
                    }
                    else if (carga.Veiculo != null)
                    {
                        contemLicencaVencidaTracao = true;
                        validacoesRegraLicenca += "- Licença da tração " + licenca.Descricao + " não encontrada; <br />";
                    }

                    if (carga.Motoristas != null && carga.Motoristas.Count > 0)
                    {
                        foreach (var motorista in carga.Motoristas)
                        {
                            if (motorista != null && motorista.Licencas != null && motorista.Licencas.Count > 0 && motorista.Licencas.Any(c => c.Licenca?.Codigo == licenca.CodigoInterno && c.DataVencimento.HasValue))
                            {
                                if (motorista.Licencas.Any(c => c.Licenca.Codigo == licenca.CodigoInterno && c.DataVencimento.Value <= dataAtual) && !motorista.Licencas.Any(c => c.Licenca.Codigo == licenca.CodigoInterno && c.DataVencimento.Value > dataAtual))
                                {
                                    contemLicencaVencidaMotorista = true;
                                    validacoesRegraLicenca += "- Licença do motorista " + licenca.Descricao + " se encontra vencida; <br />";
                                }
                            }
                            else if (motorista.Licencas == null || motorista.Licencas.Count == 0)
                            {
                                contemLicencaVencidaMotorista = true;
                                validacoesRegraLicenca += "- Licença do motorista " + licenca.Descricao + " não encontrada; <br />";
                            }
                            else if (motorista.Licencas.Any(c => c.Licenca == null))
                            {
                                contemLicencaVencidaMotorista = true;
                                validacoesRegraLicenca += "- Licença do motorista " + motorista.Licencas.Where(c => c.Licenca == null).FirstOrDefault().Descricao + " não encontrada; <br />";
                            }
                        }
                    }


                    if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                    {
                        foreach (var reboque in carga.VeiculosVinculados)
                        {
                            if (reboque != null && reboque.LicencasVeiculo != null && reboque.LicencasVeiculo.Count > 0 && reboque.LicencasVeiculo.Any(c => c.Licenca?.Codigo == licenca.CodigoInterno && c.DataVencimento.HasValue))
                            {
                                if (reboque.LicencasVeiculo.Any(c => c.Licenca?.Codigo == licenca.CodigoInterno && c.DataVencimento.Value <= dataAtual) && !reboque.LicencasVeiculo.Any(c => c.Licenca?.Codigo == licenca.CodigoInterno && c.DataVencimento.Value > dataAtual))
                                {
                                    contemLicencaVencidaReboque = true;
                                    validacoesRegraLicenca += "- Licença do reboque " + licenca.Descricao + " se encontra vencida; <br />";
                                }
                            }
                            else if (reboque.LicencasVeiculo == null || reboque.LicencasVeiculo.Count == 0)
                            {
                                contemLicencaVencidaReboque = true;
                                validacoesRegraLicenca += "- Licença do reboque " + licenca.Descricao + " não encontrada; <br />";
                            }
                            else if (reboque.LicencasVeiculo.Any(c => c.Licenca == null))
                            {
                                contemLicencaVencidaReboque = true;
                                validacoesRegraLicenca += "- Licença do reboque " + reboque.LicencasVeiculo.Where(c => c.Licenca == null).FirstOrDefault().Descricao + " não encontrada; <br />";
                            }
                        }
                    }

                }


                if (!string.IsNullOrWhiteSpace(validacoesRegraLicenca) && condicaoLicenca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca.E && contemLicencaVencidaMotorista && contemLicencaVencidaReboque && contemLicencaVencidaTracao)
                    validacoesRegraPlanejamento += validacoesRegraLicenca;
                else if (!string.IsNullOrWhiteSpace(validacoesRegraLicenca) && condicaoLicenca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca.Ou && (contemLicencaVencidaMotorista || contemLicencaVencidaReboque || contemLicencaVencidaTracao))
                    validacoesRegraPlanejamento += validacoesRegraLicenca;
            }


            var condicaoLiberacaoGR = retornoRegraPlanejamento.CondicaoLiberacoesGR != null && retornoRegraPlanejamento.CondicaoLiberacoesGR.Count > 0
                ? retornoRegraPlanejamento.CondicaoLiberacoesGR.FirstOrDefault()
                : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLiberacaoGR.E;


            string validacoesRegraLicencaLiberacaoGR = "";
            int liberacoesGRValidadas = 0;
            if (retornoRegraPlanejamento.LiberacoesGR != null && retornoRegraPlanejamento.LiberacoesGR.Count > 0)
            {
                
                foreach (var liberacaoGR in retornoRegraPlanejamento.LiberacoesGR)
                {
                    if (carga.Motoristas != null && carga.Motoristas.Count > 0)
                    {
                        bool valido = true;
                        foreach (var motorista in carga.Motoristas)
                        {
                            if (motorista != null && motorista.LiberacoesGR != null && motorista.LiberacoesGR.Count > 0 && motorista.LiberacoesGR.Any(c => c.Licenca?.Codigo == liberacaoGR.CodigoInterno && c.DataVencimento.HasValue))
                            {
                                if (motorista.LiberacoesGR.Any(c => c.Licenca.Codigo == liberacaoGR.CodigoInterno && c.DataVencimento.Value <= dataAtual) && !motorista.LiberacoesGR.Any(c => c.Licenca.Codigo == liberacaoGR.CodigoInterno && c.DataVencimento.Value > dataAtual))
                                {
                                    valido = false;
                                    contemLicencaVencidaMotorista = true;
                                    validacoesRegraLicencaLiberacaoGR += "- Liberação GR do motorista " + liberacaoGR.Descricao + " se encontra vencida; <br />";
                                }
                            }
                            else
                            { 
                                valido = false;
                                contemLicencaVencidaMotorista = true;
                                validacoesRegraLicencaLiberacaoGR += "- Liberação GR do motorista " + liberacaoGR.Descricao + " não encontrada; <br />";
                            }
                        }
                        if (valido)
                            liberacoesGRValidadas++;
                    }
                    
                }
                if ((!string.IsNullOrWhiteSpace(validacoesRegraLicencaLiberacaoGR) && condicaoLiberacaoGR == CondicaoLiberacaoGR.E && contemLicencaVencidaMotorista && retornoRegraPlanejamento.LiberacoesGR.Count != liberacoesGRValidadas))
                {
                    validacoesRegraPlanejamento += validacoesRegraLicencaLiberacaoGR;
                }
                else if ((!string.IsNullOrWhiteSpace(validacoesRegraLicencaLiberacaoGR) && condicaoLiberacaoGR == CondicaoLiberacaoGR.Ou && (liberacoesGRValidadas == 0)))
                {
                    validacoesRegraPlanejamento += validacoesRegraLicencaLiberacaoGR;
                }
            }

            string validacoesRegraLicencaLiberacaoGRVeiculo = "";
            string validacoesRegraLicencaLiberacaoGRVeiculoReboque = "";
            string validacoesRegraLicencaLiberacaoGRVeiculoTracao = "";
            bool contemLicencaVencidaReboqueLiberacaoGRVeiculo = false;
            bool contemLicencaVencidaVeiculo = false;
            int liberacoesGRVeiculoValidadas = 0;
            int liberacoesGRVeiculoReboqueValidadas = 0;
            int liberacoesGRVeiculoTracaoValidadas = 0;

            if (retornoRegraPlanejamento.LiberacoesGRVeiculo != null && retornoRegraPlanejamento.LiberacoesGRVeiculo.Count > 0)
            {
                foreach (var liberacaoGR in retornoRegraPlanejamento.LiberacoesGRVeiculo)
                {
                    bool valido = true;
                    if (carga.Veiculo != null)
                    {
                        var veiculo = carga.Veiculo;

                        if (veiculo != null && veiculo.VeiculoLiberacaoGR != null && veiculo.VeiculoLiberacaoGR.Count > 0 && veiculo.VeiculoLiberacaoGR.Any(c => c.Licenca?.Codigo == liberacaoGR.CodigoInterno && c.DataVencimento.HasValue))
                        {
                            if (veiculo.VeiculoLiberacaoGR.Any(c => c.Licenca.Codigo == liberacaoGR.CodigoInterno && c.DataVencimento.Value <= dataAtual) && !veiculo.VeiculoLiberacaoGR.Any(c => c.Licenca.Codigo == liberacaoGR.CodigoInterno && c.DataVencimento.Value > dataAtual))
                            {
                                valido = false;
                                contemLicencaVencidaVeiculo = true;
                                validacoesRegraLicencaLiberacaoGRVeiculoTracao += "- Liberação GR do veículo " + liberacaoGR.Descricao + " se encontra vencida; <br />";
                            }
                            else
                            {
                                valido = true;
                            }
                        }
                        else
                        {
                            valido = false;
                            contemLicencaVencidaVeiculo = true;
                            validacoesRegraLicencaLiberacaoGRVeiculoTracao += "- Liberação GR do veículo " + liberacaoGR.Descricao + " não encontrada; <br />";
                        }
                    }
                    if (valido)
                        liberacoesGRVeiculoTracaoValidadas++;
                }

                foreach (var liberacaoGR in retornoRegraPlanejamento.LiberacoesGRVeiculo)
                {
                    bool valido = true;
                    if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                    {
                        foreach (var reboque in carga.VeiculosVinculados)
                        {
                            if (reboque != null && reboque.VeiculoLiberacaoGR != null && reboque.VeiculoLiberacaoGR.Count > 0 && reboque.VeiculoLiberacaoGR.Any(c => c.Licenca?.Codigo == liberacaoGR.CodigoInterno && c.DataVencimento.HasValue))
                            {
                                if (reboque.VeiculoLiberacaoGR.Any(c => c.Licenca.Codigo == liberacaoGR.CodigoInterno && c.DataVencimento.Value <= dataAtual) && !reboque.VeiculoLiberacaoGR.Any(c => c.Licenca.Codigo == liberacaoGR.CodigoInterno && c.DataVencimento.Value > dataAtual))
                                {
                                    valido = false;
                                    contemLicencaVencidaReboqueLiberacaoGRVeiculo = true;
                                    validacoesRegraLicencaLiberacaoGRVeiculoReboque += "- Licença da Liberação GR veículo do reboque " + liberacaoGR.Descricao + " se encontra vencida; <br />";
                                }
                                else
                                {
                                    valido = true;
                                }
                            }
                            else
                            {
                                valido = false;
                                contemLicencaVencidaReboqueLiberacaoGRVeiculo = true;
                                validacoesRegraLicencaLiberacaoGRVeiculoReboque += "- Licença da Liberação GR veículo do reboque " + liberacaoGR.Descricao + " não encontrada; <br />";
                            }
                            if (valido)
                                liberacoesGRVeiculoReboqueValidadas++;
                        }                      
                    }    
                }

                if(liberacoesGRVeiculoReboqueValidadas > 0 && liberacoesGRVeiculoTracaoValidadas > 0)
                    liberacoesGRVeiculoValidadas++;

                if (liberacoesGRVeiculoReboqueValidadas == 0)
                    validacoesRegraLicencaLiberacaoGRVeiculo += validacoesRegraLicencaLiberacaoGRVeiculoReboque;

                if (liberacoesGRVeiculoTracaoValidadas == 0)
                    validacoesRegraLicencaLiberacaoGRVeiculo += validacoesRegraLicencaLiberacaoGRVeiculoTracao;


                if ((!string.IsNullOrWhiteSpace(validacoesRegraLicencaLiberacaoGRVeiculo) && condicaoLiberacaoGR == CondicaoLiberacaoGR.E && contemLicencaVencidaVeiculo && contemLicencaVencidaReboqueLiberacaoGRVeiculo && retornoRegraPlanejamento.LiberacoesGRVeiculo.Count != liberacoesGRVeiculoValidadas))
                {
                    validacoesRegraPlanejamento += validacoesRegraLicencaLiberacaoGRVeiculo;
                }
                else if ((!string.IsNullOrWhiteSpace(validacoesRegraLicencaLiberacaoGRVeiculo) && condicaoLiberacaoGR == CondicaoLiberacaoGR.Ou && (liberacoesGRVeiculoValidadas == 0)))
                {
                    validacoesRegraPlanejamento += validacoesRegraLicencaLiberacaoGRVeiculo;
                }
            }




            if (retornoRegraPlanejamento.QuantidadeCarga > 0 && retornoRegraPlanejamento.PeriodoQuantidadeCarga > 0)
            {
                if (retornoRegraPlanejamento.ValidarQuantidadeVeiculoEReboque)
                {
                    if (carga.Veiculo != null)
                    {
                        bool contemQuantidadeCargaPeriodoViolado = false;
                        string validacoesQuantidadeCargaPeriodo = "";
                        int quantidadeCarga = 0;
                        repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                        switch (retornoRegraPlanejamento.TipoPeriodoQuantidadeCarga)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Dia:

                                quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGR(dataAtual.AddDays(retornoRegraPlanejamento.PeriodoQuantidadeCarga * -1), dataAtual, carga.Veiculo);
                                if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                {
                                    contemQuantidadeCargaPeriodoViolado = true;
                                    validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Veiculo), Quantidade de cargas (", quantidadeCarga, ") !  Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por dia <br />");
                                }
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Semana:
                                quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGR(dataAtual.AddDays((retornoRegraPlanejamento.PeriodoQuantidadeCarga * 7) * -1), dataAtual, carga.Veiculo);
                                if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                {
                                    contemQuantidadeCargaPeriodoViolado = true;
                                    validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Veiculo), Quantidade de cargas (", quantidadeCarga, ") ! Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por semana <br />");
                                }
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Mes:
                                quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGR(dataAtual.AddMonths(retornoRegraPlanejamento.PeriodoQuantidadeCarga * -1), dataAtual, carga.Veiculo);
                                if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                {
                                    contemQuantidadeCargaPeriodoViolado = true;
                                    validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Veiculo), Quantidade de cargas (", quantidadeCarga, ") ! Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por mês <br />");
                                }
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Ano:
                                quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGR(dataAtual.AddYears(retornoRegraPlanejamento.PeriodoQuantidadeCarga * -1), dataAtual, carga.Veiculo);
                                if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                {
                                    contemQuantidadeCargaPeriodoViolado = true;
                                    validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Veiculo), Quantidade de cargas (", quantidadeCarga, ") !  Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por ano <br />");
                                }
                                break;
                        }
                        if (contemQuantidadeCargaPeriodoViolado)
                            validacoesRegraPlanejamento += validacoesQuantidadeCargaPeriodo;
                    }
                    if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                    {
                        foreach (var reboque in carga.VeiculosVinculados)
                        {
                            bool contemQuantidadeCargaPeriodoViolado = false;
                            string validacoesQuantidadeCargaPeriodo = "";
                            int quantidadeCarga = 0;
                            repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                            switch (retornoRegraPlanejamento.TipoPeriodoQuantidadeCarga)
                            {
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Dia:

                                    quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGR(dataAtual.AddDays(retornoRegraPlanejamento.PeriodoQuantidadeCarga * -1), dataAtual, reboque);
                                    if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                    {
                                        contemQuantidadeCargaPeriodoViolado = true;
                                        validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Reboque), Quantidade de cargas (", quantidadeCarga, ") !  Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por dia <br />");
                                    }
                                    break;
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Semana:
                                    quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGR(dataAtual.AddDays((retornoRegraPlanejamento.PeriodoQuantidadeCarga * 7) * -1), dataAtual, reboque);
                                    if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                    {
                                        contemQuantidadeCargaPeriodoViolado = true;
                                        validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Reboque), Quantidade de cargas (", quantidadeCarga, ") ! Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por semana <br />");
                                    }
                                    break;
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Mes:
                                    quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGR(dataAtual.AddMonths(retornoRegraPlanejamento.PeriodoQuantidadeCarga * -1), dataAtual, reboque);
                                    if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                    {
                                        contemQuantidadeCargaPeriodoViolado = true;
                                        validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Reboque), Quantidade de cargas (", quantidadeCarga, ") ! Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por mês <br />");
                                    }
                                    break;
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Ano:
                                    quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGR(dataAtual.AddYears(retornoRegraPlanejamento.PeriodoQuantidadeCarga * -1), dataAtual, reboque);
                                    if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                    {
                                        contemQuantidadeCargaPeriodoViolado = true;
                                        validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Reboque), Quantidade de cargas (", quantidadeCarga, ") !  Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por ano <br />");
                                    }
                                    break;
                            }
                            if (contemQuantidadeCargaPeriodoViolado)
                                validacoesRegraPlanejamento += validacoesQuantidadeCargaPeriodo;
                        }
                    }

                }

                if (retornoRegraPlanejamento.ValidarPorQuantidadeMotorista)
                {
                    if (carga.Motoristas != null && carga.Motoristas.Count > 0)
                    {
                        foreach (var motorista in carga.Motoristas)
                        {
                            bool contemQuantidadeCargaPeriodoViolado = false;
                            string validacoesQuantidadeCargaPeriodo = "";
                            int quantidadeCarga = 0;
                            repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                            switch (retornoRegraPlanejamento.TipoPeriodoQuantidadeCarga)
                            {
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Dia:

                                    quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGRMotorista(dataAtual.AddDays(retornoRegraPlanejamento.PeriodoQuantidadeCarga * -1), dataAtual, motorista);
                                    if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                    {
                                        contemQuantidadeCargaPeriodoViolado = true;
                                        validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Motorista), Quantidade de cargas (", quantidadeCarga, ") !  Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por dia <br />");
                                    }
                                    break;
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Semana:
                                    quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGRMotorista(dataAtual.AddDays((retornoRegraPlanejamento.PeriodoQuantidadeCarga * 7) * -1), dataAtual, motorista);
                                    if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                    {
                                        contemQuantidadeCargaPeriodoViolado = true;
                                        validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Motorista), Quantidade de cargas (", quantidadeCarga, ") ! Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por semana <br />");
                                    }
                                    break;
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Mes:
                                    quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGRMotorista(dataAtual.AddMonths(retornoRegraPlanejamento.PeriodoQuantidadeCarga * -1), dataAtual, motorista);
                                    if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                    {
                                        contemQuantidadeCargaPeriodoViolado = true;
                                        validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Motorista), Quantidade de cargas (", quantidadeCarga, ") ! Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por mês <br />");
                                    }
                                    break;
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaMesAno.Ano:
                                    quantidadeCarga = repCargaPedido.BuscarCargasPeriodoLiberacaoGRMotorista(dataAtual.AddYears(retornoRegraPlanejamento.PeriodoQuantidadeCarga * -1), dataAtual, motorista);
                                    if (quantidadeCarga < retornoRegraPlanejamento.QuantidadeCarga)
                                    {
                                        contemQuantidadeCargaPeriodoViolado = true;
                                        validacoesQuantidadeCargaPeriodo += string.Concat("- Liberação de GR violado (Motorista), Quantidade de cargas (", quantidadeCarga, ") !  Permitido no mínimo ", retornoRegraPlanejamento.QuantidadeCarga, " no período de ", retornoRegraPlanejamento.PeriodoQuantidadeCarga, " por ano <br />");
                                    }
                                    break;
                            }
                            if (contemQuantidadeCargaPeriodoViolado)
                                validacoesRegraPlanejamento += validacoesQuantidadeCargaPeriodo;
                        }
                    }
                }
                
            }

            if (!string.IsNullOrWhiteSpace(validacoesRegraPlanejamento))
            {
                msgRetorno = "Para a regra Nº " + retornoRegraPlanejamento.Numero + " " + retornoRegraPlanejamento.Descricao + ", é obrigatório: <br />" + validacoesRegraPlanejamento;
                return false;
            }
            else
                return true;
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorValorMercadoria(decimal valorMercadoria, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            return regras.Where(c => (c.ValorDeDaMercadoria <= valorMercadoria && c.ValorAteDaMercadoria >= valorMercadoria) || (c.ValorAteDaMercadoria == 0 && c.ValorDeDaMercadoria == 0)).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorCidadeOrigem(List<int> codigosCidadeOrigem, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosCidadeOrigem == null || codigosCidadeOrigem.Count == 0)
                return null;

            return regras.Where(c => c.Origens == null || c.Origens.Any(d => codigosCidadeOrigem.Contains(d.Codigo))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorCidadeDestino(List<int> codigosCidadeDestino, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosCidadeDestino == null || codigosCidadeDestino.Count == 0)
                return null;

            return regras.Where(c => c.Destinos == null || c.Destinos.Any(d => codigosCidadeDestino.Contains(d.Codigo))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorClienteOrigem(List<double> cnpjsClienteOrigem, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (cnpjsClienteOrigem == null || cnpjsClienteOrigem.Count == 0)
                return null;

            return regras.Where(c => c.ClientesOrigem == null || c.ClientesOrigem.Any(d => cnpjsClienteOrigem.Contains(d.CPF_CNPJ))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorClienteDestino(List<double> cnpjsClienteDestino, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (cnpjsClienteDestino == null || cnpjsClienteDestino.Count == 0)
                return null;

            return regras.Where(c => c.ClientesDestino == null || c.ClientesDestino.Any(d => cnpjsClienteDestino.Contains(d.CPF_CNPJ))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorEstadoOrigem(List<string> ufOrigem, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (ufOrigem == null || ufOrigem.Count == 0)
                return null;

            return regras.Where(c => c.EstadosOrigem == null || c.EstadosOrigem.Count == 0 || c.EstadosOrigem.Any(d => ufOrigem.Contains(d.Abreviacao))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorEstadoDestino(List<string> ufDestino, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (ufDestino == null || ufDestino.Count == 0)
                return null;

            return regras.Where(c => c.EstadosDestino == null || c.EstadosDestino.Count == 0 || c.EstadosDestino.Any(d => ufDestino.Contains(d.Abreviacao))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorRegiaoOrigem(List<int> codigosRegiaoOrigem, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosRegiaoOrigem == null || codigosRegiaoOrigem.Count == 0)
                return null;

            return regras.Where(c => c.RegioesOrigem == null || c.RegioesOrigem.Any(d => codigosRegiaoOrigem.Contains(d.Codigo))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorRegiaoDestino(List<int> codigosRegiaoDestino, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosRegiaoDestino == null || codigosRegiaoDestino.Count == 0)
                return null;

            return regras.Where(c => c.RegioesDestino == null || c.RegioesDestino.Any(d => codigosRegiaoDestino.Contains(d.Codigo))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorRotaOrigem(List<int> codigosRotaOrigem, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosRotaOrigem == null || codigosRotaOrigem.Count == 0)
                return null;

            return regras.Where(c => c.RotasOrigem == null || c.RotasOrigem.Any(d => codigosRotaOrigem.Contains(d.Codigo))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorRotaDestino(List<int> codigosRotaDestino, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosRotaDestino == null || codigosRotaDestino.Count == 0)
                return null;

            return regras.Where(c => c.RotasDestino == null || c.RotasDestino.Any(d => codigosRotaDestino.Contains(d.Codigo))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorCEPOrigem(List<string> cepOrigem, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (cepOrigem == null || cepOrigem.Count == 0)
                return null;

            List<int> cepConvertido = new List<int>();
            foreach (var cep in cepOrigem)
                cepConvertido.Add(cep.ToInt());

            return regras.Where(c => c.CEPsOrigem == null || c.CEPsOrigem.Any(d => cepConvertido.Contains(d.CEPInicial) || cepConvertido.Contains(d.CEPFinal))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorCEPDestino(List<string> cepDestino, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (cepDestino == null || cepDestino.Count == 0)
                return null;

            List<int> cepConvertido = new List<int>();
            foreach (var cep in cepDestino)
                cepConvertido.Add(cep.ToInt());

            return regras.Where(c => c.CEPsDestino == null || c.CEPsDestino.Any(d => cepConvertido.Contains(d.CEPInicial) || cepConvertido.Contains(d.CEPFinal))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorPaisOrigem(List<int> codigosPaisOrigem, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosPaisOrigem == null || codigosPaisOrigem.Count == 0)
                return null;

            return regras.Where(c => c.PaisesOrigem == null || c.PaisesOrigem.Count == 0 || c.PaisesOrigem.Any(d => codigosPaisOrigem.Contains(d.Codigo))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorPaisDestino(List<int> codigosPaisDestino, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosPaisDestino == null || codigosPaisDestino.Count == 0)
                return null;

            return regras.Where(c => c.PaisesDestino == null || c.PaisesDestino.Count == 0 || c.PaisesDestino.Any(d => codigosPaisDestino.Contains(d.Codigo))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorGrupoPessoa(int grupoPessoa, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            return regras.Where(c => c.GrupoPessoas == null || c.GrupoPessoas.Any(d => d.Codigo == grupoPessoa)).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorTipoOperacao(int tipoOperacao, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            return regras.Where(c => c.TiposOperacao == null || c.TiposOperacao.Any(d => d.Codigo == tipoOperacao)).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorTipoCarga(int tipoCarga, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            return regras.Where(c => c.TiposDeCarga == null || c.TiposDeCarga.Any(d => d.Codigo == tipoCarga)).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorCentroResultado(List<int> codigosCentroResultado, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosCentroResultado == null || codigosCentroResultado.Count == 0)
                return null;

            return regras.Where(c => c.CentrosResultado == null || c.CentrosResultado.Any(d => codigosCentroResultado.Contains(d.Codigo))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorModeloVeicularCarga(List<int> codigosModeloVeicularCarga, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosModeloVeicularCarga == null || codigosModeloVeicularCarga.Count == 0)
                return null;

            return regras.Where(c => c.ModelosVeicularesCarga == null || c.ModelosVeicularesCarga.Any(d => codigosModeloVeicularCarga.Contains(d.Codigo))).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> FiltrarPorNivelCooperado(List<int> codigosNivelCooperado, List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> regras)
        {
            if (codigosNivelCooperado == null || codigosNivelCooperado.Count == 0)
                return null;

            return regras.Where(c => c.NiveisCooperados == null || c.NiveisCooperados.Any(d => codigosNivelCooperado.Contains(d.Codigo))).ToList();
        }
        #endregion
    }
}
