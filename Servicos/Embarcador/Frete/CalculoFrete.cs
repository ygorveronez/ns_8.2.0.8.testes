using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frete
{
    public class CalculoFrete
    {
        #region Propriedades Privadas

        private Repositorio.UnitOfWork _unitOfWork;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoTMS;
        private Repositorio.Embarcador.Pedidos.TipoOperacao _repositorioTipoOperacao;
        private Repositorio.Embarcador.Cargas.TipoDeCarga _repositorioTipoDeCarga;

        #endregion

        #region Construtores

        public CalculoFrete(Repositorio.UnitOfWork unitOfWork,
                            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
                            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _configuracaoTMS = configuracaoTMS ?? new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();
            _repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            _repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete> CalcularFrete(Dominio.ObjetosDeValor.WebService.Frete.DadosCalculoFrete dadosCalculoFrete)
        {
            if (!ValidarDadosCalculoFrete(dadosCalculoFrete, out Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete> retorno))
                return retorno;

            return ObterRetornoCalculoFrete(dadosCalculoFrete);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>> CalcularFreteVariacao(Dominio.ObjetosDeValor.WebService.Frete.DadosCalculoFrete dadosCalculoFrete)
        {
            if (dadosCalculoFrete.IBGEOrigem?.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos("É necessário informar um IBGE de origem.");
            else if (dadosCalculoFrete.IBGEOrigem?.Count > 1)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos("Informe apenas IBGE de origem.");

            if (dadosCalculoFrete.IBGEDestino?.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos("É necessário informar um IBGE de destino.");
            else if (dadosCalculoFrete.IBGEDestino?.Count > 1)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos("nforme apenas IBGE de destino.");

            if (string.IsNullOrWhiteSpace(dadosCalculoFrete.CodigoIntegracaoTipoCarga))
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos("É necessário informar o Código de Integração do Tipo da Carga.");

            if (string.IsNullOrWhiteSpace(dadosCalculoFrete.CodigoIntegracaoTipoOperacao))
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos("É necessário informar o Código de Integração do Tipo de Operação.");

            return ObterRetornoCalculoFreteVariacao(dadosCalculoFrete);
        }

        #endregion

        #region Métodos Privados

        private bool ValidarDadosCalculoFrete(Dominio.ObjetosDeValor.WebService.Frete.DadosCalculoFrete dadosCalculoFrete, out Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete> retorno)
        {
            if (dadosCalculoFrete.IBGEOrigem?.Count == 0 && dadosCalculoFrete.CEPOrigem?.Count == 0)
            {
                retorno = Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete>.CriarRetornoDadosInvalidos("É necessário informar um CEP ou IBGE de origem.");
                return false;
            }

            if (dadosCalculoFrete.IBGEDestino?.Count == 0 && dadosCalculoFrete.CEPDestino?.Count == 0)
            {
                retorno = Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete>.CriarRetornoDadosInvalidos("É necessário informar um CEP ou IBGE de destino.");
                return false;

            }

            retorno = null;
            return true;
        }

        private Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete> ObterRetornoCalculoFrete(Dominio.ObjetosDeValor.WebService.Frete.DadosCalculoFrete dadosCalculoFrete)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFrete(dadosCalculoFrete);

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo = Servicos.Embarcador.Carga.Frete.ObterDadosCalculoFrete(parametrosCalculo, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao, _configuracaoTMS);

            if (!dadosCalculo.FreteCalculado ||
                dadosCalculo.FreteCalculadoComProblemas)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete>.CriarRetornoDadosInvalidos(dadosCalculo.MensagemRetorno);

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete>.CriarRetornoSucesso(new Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete()
            {
                Valores = new List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValores>()
                {
                    new Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValores()
                    {
                         Componentes = dadosCalculo.Componentes?.Select(o => new Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteValoresComponente(){ Descricao = o.ComponenteFrete?.Descricao, Valor = o.ValorComponente, IncluirBaseCalculoICMS = o.IncluirBaseCalculoICMS}).ToList(),
                         ValorFreteLiquido = dadosCalculo.ValorFrete,
                         ValorTotalFrete = dadosCalculo.ValorTotal
                    }
                }
            });
        }

        private Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>> ObterRetornoCalculoFreteVariacao(Dominio.ObjetosDeValor.WebService.Frete.DadosCalculoFrete dadosCalculoFrete)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFrete(dadosCalculoFrete, calcularVariacoes: true);

            if (parametrosCalculo.Origens.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos("Nenhuma Origem encontrada com o Código IBGE informado.");

            if (parametrosCalculo.Destinos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos("Nenhum Destino encontrado com o Código IBGE informado.");

            if (parametrosCalculo.TipoOperacao == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos("Código de Integração do Tipo de Operação inválido.");

            if (parametrosCalculo.TipoCarga == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos("Código de Integração do Tipo da Carga inválido.");

            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculo = Servicos.Embarcador.Carga.Frete.ObterDadosCalculoFrete(parametrosCalculo, _tipoServicoMultisoftware, _unitOfWork, _unitOfWork.StringConexao, _configuracaoTMS);

            if (!dadosCalculo.FreteCalculado || dadosCalculo.FreteCalculadoComProblemas)
                return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoDadosInvalidos(dadosCalculo.MensagemRetorno);

            List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao> retorno = new List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete variacao in dadosCalculo.Variacoes)
            {
                ICollection<Dominio.Entidades.Empresa> transportadores = variacao.TabelaFrete.Transportadores;

                if (variacao.TabelaFreteCliente != null && variacao.TabelaFreteCliente.Empresa != null)
                    transportadores.Add(variacao.TabelaFreteCliente.Empresa);

                if (variacao.ComposicoesVariacao == null)
                    continue;

                foreach (var parametro in variacao.ComposicoesVariacao)
                {
                    if (variacao.TabelaFrete.ParametroBase != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque
                        && variacao.TabelaFrete.ParametroBase != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao)
                        continue;

                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repositorioModeloVeicularCarga.BuscarPorCodigo(parametro.Key.CodigoObjeto);

                    foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao in parametro.Value.Where(composicao => composicao.Valor > 0))
                        foreach (var transportador in transportadores)
                        {
                            Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao retornoCalculoFreteVariacao = new Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao()
                            {
                                ValorFrete = composicao.Valor,
                                CNPJTransportador = transportador.CNPJ_Formatado,
                                RazaoSocialTransportador = transportador.RazaoSocial ?? "",
                                CodigoIntegracaoTransportador = transportador.CodigoIntegracao ?? "",
                                CodigoIntegracaoTipoCarga = dadosCalculoFrete.CodigoIntegracaoTipoCarga ?? "",
                                CodigoIntegracaoTipoOperacao = dadosCalculoFrete.CodigoIntegracaoTipoOperacao ?? "",
                                CodigoIntegracaoModeloVeicular = modeloVeicular.CodigoIntegracao ?? "",
                            };

                            retorno.Add(retornoCalculoFreteVariacao);
                        }
                }
            }

            return Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFreteVariacao>>.CriarRetornoSucesso(retorno);
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFrete(Dominio.ObjetosDeValor.WebService.Frete.DadosCalculoFrete dadosCalculoFrete, bool calcularVariacoes = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete()
            {
                CEPsRemetentes = dadosCalculoFrete.CEPOrigem,
                CEPsDestinatarios = dadosCalculoFrete.CEPDestino,
                DataVigencia = DateTime.Now,
                Peso = dadosCalculoFrete.Peso,
                QuantidadeNotasFiscais = 1,
                Quantidades = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade>()
                {
                    new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFreteQuantidade()
                    {
                        Quantidade = dadosCalculoFrete.Peso,
                        UnidadeMedida = Dominio.Enumeradores.UnidadeMedida.KG
                    }
                },
                ValorNotasFiscais = dadosCalculoFrete.ValorMercadoria,
                Volumes = dadosCalculoFrete.Volumes,
                ModelosUtilizadosEmissao = new List<Dominio.Entidades.ModeloDocumentoFiscal>() { null },
                TipoOperacao = !string.IsNullOrWhiteSpace(dadosCalculoFrete.CodigoIntegracaoTipoOperacao) ? _repositorioTipoOperacao.BuscarPorCodigoIntegracao(dadosCalculoFrete.CodigoIntegracaoTipoOperacao) : null,
                TipoCarga = !string.IsNullOrWhiteSpace(dadosCalculoFrete.CodigoIntegracaoTipoCarga) ? _repositorioTipoDeCarga.BuscarPorCodigoEmbarcador(dadosCalculoFrete.CodigoIntegracaoTipoCarga) : null,
                CalcularVariacoes = calcularVariacoes,
                RementesEDestinatariosOpcionaisQuandoExistirLocalidade = calcularVariacoes,
                NaoValidarTransportador = calcularVariacoes
            };

            parametrosCalculoFrete.Origens = ObterLocalidades(dadosCalculoFrete.IBGEOrigem);
            parametrosCalculoFrete.Destinos = ObterLocalidades(dadosCalculoFrete.IBGEDestino);

            if (calcularVariacoes && parametrosCalculoFrete.Origens != null && parametrosCalculoFrete.Destinos != null)
            {
                Dominio.Entidades.Localidade origem = parametrosCalculoFrete.Origens.FirstOrDefault();
                Dominio.Entidades.Localidade destino = parametrosCalculoFrete.Destinos.FirstOrDefault();

                double.TryParse(origem?.Latitude?.ToString() ?? "", out double origemLatitude);
                double.TryParse(origem?.Longitude?.ToString() ?? "", out double origemLongitude);
                double.TryParse(destino?.Latitude?.ToString() ?? "", out double destinoLatitude);
                double.TryParse(destino?.Longitude?.ToString() ?? "", out double destinoLongitude);

                parametrosCalculoFrete.Distancia = Convert.ToDecimal(Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaKM(origemLatitude, origemLongitude, destinoLatitude, destinoLongitude));

                decimal distanciaInicialPadrao = 0.01m;
                if (parametrosCalculoFrete.Distancia == 0 && origem?.Codigo == destino?.Codigo)
                    parametrosCalculoFrete.Distancia = distanciaInicialPadrao;
            }

            return parametrosCalculoFrete;
        }

        private List<Dominio.Entidades.Localidade> ObterLocalidades(List<int> codigosIBGE)
        {
            if ((codigosIBGE?.Count() ?? 0) == 0)
                return null;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);

            return repLocalidade.BuscarPorCodigoIBGE(codigosIBGE);
        }

        #endregion
    }
}
