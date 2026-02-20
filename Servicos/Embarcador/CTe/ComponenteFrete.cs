using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Servicos.Embarcador.CTe
{
    public class ComponenteFrete
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public ComponenteFrete(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> ConverterComponenteCTeParaComponenteFrete(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroComponenteFrete> componentesPrestacaoCTe)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> componentes = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);
            //todo: ver como usar esse método
            return componentes;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> ConverterComponenteCTeParaComponenteFrete(List<Dominio.Entidades.ComponentePrestacaoCTE> componentesPrestacaoCTe)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> componentes = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);
            //todo: ver como usar esse método
            return componentes;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> ConverterDynamicParaComponenteFrete(dynamic dynComponentes, int codigoLocalidadeOrigem = 0, int codigoLocalidadeDestino = 0)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> componentes = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);

            foreach (var dynComponente in dynComponentes)
            {
                if (codigoLocalidadeOrigem != 0 && codigoLocalidadeDestino != 0)
                {
                    if (!(dynComponente.CodigoLocalidadeOrigem == codigoLocalidadeOrigem && dynComponente.CodigoLocalidadeDestino == codigoLocalidadeDestino))
                        continue;
                }

                if ((int)dynComponente.CodigoComponente > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo((int)dynComponente.CodigoComponente);
                    componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                    componente.Componente.Descricao = (string)dynComponente.DescricaoComponente;
                    componente.Componente.TipoComponenteFrete = componenteFrete.TipoComponenteFrete;
                    componente.Componente.Codigo = componenteFrete.Codigo;
                    decimal.TryParse(dynComponente.Valor.ToString(), out decimal valor);
                    componente.ValorComponente = valor;
                    componente.IncluirTotalReceber = (bool)dynComponente.IncluirTotalReceber;
                    componente.DescontarValorTotalAReceber = (bool)dynComponente.DescontarTotalReceber;
                    componente.IncluirBaseCalculoICMS = (bool)dynComponente.IncluirBaseCalculoICMS;

                    componentes.Add(componente);
                }
            }

            return componentes;
        }

        public void SalvarComponentesPrestacao(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> componentes)
        {
            Repositorio.ComponentePrestacaoCTE repCompPrestCTe = new Repositorio.ComponentePrestacaoCTE(_unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);

            if (cte.Codigo > 0)
                repCompPrestCTe.DeletarPorCTe(cte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente in componentes)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(componente.Componente.Codigo);

                if (componente.DescontarValorTotalAReceber)
                    continue;

                if (componenteFrete.AcrescentaValorTotalAReceber)
                    continue;

                Dominio.Entidades.ComponentePrestacaoCTE componenteDaPrestacao = new Dominio.Entidades.ComponentePrestacaoCTE();
                componenteDaPrestacao.CTE = cte;
                string descricao = componente.Componente.Descricao;
                if (descricao.Length > 15)
                    descricao = componente.Componente.Descricao.Substring(0, 15);
                componenteDaPrestacao.Nome = descricao;
                componenteDaPrestacao.Valor = Math.Round(componente.ValorComponente, 2, MidpointRounding.ToEven);
                componenteDaPrestacao.IncluiNaBaseDeCalculoDoICMS = componente.IncluirBaseCalculoICMS;
                componenteDaPrestacao.IncluiNoTotalAReceber = componente.IncluirTotalReceber;
                componenteDaPrestacao.ComponenteFrete = componenteFrete;

                repCompPrestCTe.Inserir(componenteDaPrestacao);
            }

            if ((cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim) || (cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim && cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.PercentualImpostoSimplesNacional > 0))
            {
                Dominio.Entidades.ComponentePrestacaoCTE componenteImpostos = new Dominio.Entidades.ComponentePrestacaoCTE();
                componenteImpostos.CTE = cte;
                componenteImpostos.Nome = "IMPOSTOS";
                componenteImpostos.Valor = Math.Round(cte.ValorICMS, 2, MidpointRounding.ToEven);
                componenteImpostos.IncluiNaBaseDeCalculoDoICMS = false;

                if (cte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim && cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.PercentualImpostoSimplesNacional > 0)
                    componenteImpostos.IncluiNoTotalAReceber = true;
                else
                    componenteImpostos.IncluiNoTotalAReceber = false;
                repCompPrestCTe.Inserir(componenteImpostos);
            }

            Dominio.Entidades.ComponentePrestacaoCTE componentePadrao = new Dominio.Entidades.ComponentePrestacaoCTE();
            componentePadrao.CTE = cte;
            componentePadrao.Nome = "FRETE VALOR";
            componentePadrao.Valor = Math.Round(cte.ValorFrete, 2, MidpointRounding.ToEven);
            componentePadrao.IncluiNaBaseDeCalculoDoICMS = false;
            componentePadrao.IncluiNoTotalAReceber = true;
            repCompPrestCTe.Inserir(componentePadrao);
        }

        public void SalvarComponentesPrestacaoPreCte(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> componentes)
        {
            Repositorio.ComponentePrestacaoPreCTE repCompPrestPreCTe = new Repositorio.ComponentePrestacaoPreCTE(_unitOfWork);

            if (preCte.Codigo > 0)
                repCompPrestPreCTe.DeletarPorPreCTE(preCte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente in componentes)
            {
                Dominio.Entidades.ComponentePrestacaoPreCTE componenteDaPrestacao = new Dominio.Entidades.ComponentePrestacaoPreCTE();
                componenteDaPrestacao.PreCTE = preCte;
                componenteDaPrestacao.Nome = componente.Componente.Descricao;
                componenteDaPrestacao.Valor = Math.Round(componente.ValorComponente, 2, MidpointRounding.ToEven);
                componenteDaPrestacao.IncluiNaBaseDeCalculoDoICMS = componente.IncluirBaseCalculoICMS;
                componenteDaPrestacao.IncluiNoTotalAReceber = componente.IncluirTotalReceber;

                repCompPrestPreCTe.Inserir(componenteDaPrestacao);
            }

            if ((preCte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Nao && preCte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim) || (preCte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim && preCte.Empresa.Configuracao != null && preCte.Empresa.Configuracao.PercentualImpostoSimplesNacional > 0))
            {
                Dominio.Entidades.ComponentePrestacaoPreCTE componenteImpostos = new Dominio.Entidades.ComponentePrestacaoPreCTE();
                componenteImpostos.PreCTE = preCte;
                componenteImpostos.Nome = "IMPOSTOS";
                componenteImpostos.Valor = Math.Round(preCte.ValorICMS, 2, MidpointRounding.ToEven);
                componenteImpostos.IncluiNaBaseDeCalculoDoICMS = false;

                if (preCte.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim && preCte.Empresa.Configuracao != null && preCte.Empresa.Configuracao.PercentualImpostoSimplesNacional > 0)
                    componenteImpostos.IncluiNoTotalAReceber = true;
                else
                    componenteImpostos.IncluiNoTotalAReceber = false;
                repCompPrestPreCTe.Inserir(componenteImpostos);
            }

            Dominio.Entidades.ComponentePrestacaoPreCTE componentePadrao = new Dominio.Entidades.ComponentePrestacaoPreCTE();
            componentePadrao.PreCTE = preCte;
            componentePadrao.Nome = "FRETE VALOR";
            componentePadrao.Valor = Math.Round(preCte.ValorFrete, 2, MidpointRounding.ToEven);
            componentePadrao.IncluiNaBaseDeCalculoDoICMS = false;
            componentePadrao.IncluiNoTotalAReceber = true;
            repCompPrestPreCTe.Inserir(componentePadrao);
        }

        public List<dynamic> ObterInformacoesComponentesFrete(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe)
        {
            Repositorio.ComponentePrestacaoCTE repositorioComponentePrestacaoCTE = new Repositorio.ComponentePrestacaoCTE(_unitOfWork);
            Repositorio.ComponentePrestacaoPreCTE repositorioComponentePrestacaoPreCTE = new Repositorio.ComponentePrestacaoPreCTE(_unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado repositorioTransportadorComponenteCTeImportado = new Repositorio.Embarcador.Transportadores.TransportadorComponenteCTeImportado(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repositorioConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repositorioConfiguracaoCargaEmissaoDocumento.BuscarPrimeiroRegistro();

            List<dynamic> retorno = new List<dynamic>();
            List<(string Descricao, int Codigo)> componentes = new List<(string Descricao, int Codigo)>();

            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado> componentesCTesImportados = repositorioTransportadorComponenteCTeImportado.BuscarPorTransportador(cte.Empresa.Codigo);
            if (componentesCTesImportados.Count > 0)
                componentes = componentesCTesImportados.Select(o => ValueTuple.Create(o.Descricao, o.ComponenteFrete.Codigo)).ToList();

            if (componentes.Count == 0)
            {
                List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentesFrete = repositorioTransportadorComponenteCTeImportado.BuscarComponentesDePara();
                componentes = componentesFrete.Select(o => ValueTuple.Create(o.Descricao, o.Codigo)).ToList();
            }

            bool verificarComponenteFreteValorComOutraDescricao = !configuracao.ValidarSomenteFreteLiquidoNaImportacaoCTe && configuracaoCargaEmissaoDocumento.ControlarValoresComponentesCTe;
            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesPrestacaoCTE = verificarComponenteFreteValorComOutraDescricao ? repositorioComponentePrestacaoCTE.BuscarPorCTe(cte.Codigo) : repositorioComponentePrestacaoCTE.BuscarInformadosPorCTe(cte.Codigo);
            List<Dominio.Entidades.ComponentePrestacaoPreCTE> componentesPrestacaoPreCTE = preCTe != null ? repositorioComponentePrestacaoPreCTE.BuscarPorPreCTe(preCTe.Codigo) : new List<Dominio.Entidades.ComponentePrestacaoPreCTE>();

            foreach ((string Descricao, int Codigo) in componentes)
            {
                Dominio.Entidades.ComponentePrestacaoPreCTE componentePreCTe = componentesPrestacaoPreCTE.Find(obj => obj.Nome == Descricao);
                Dominio.Entidades.ComponentePrestacaoCTE componenteCTe = componentesPrestacaoCTE.Find(obj => obj.Nome == Descricao);

                if (verificarComponenteFreteValorComOutraDescricao && Descricao.Equals("FRETE VALOR") && componentePreCTe?.Valor == 0)
                    componentePreCTe = componentesPrestacaoPreCTE.Find(obj => obj.ComponenteFrete?.Codigo == Codigo);

                if (componentePreCTe == null && componenteCTe == null)
                    continue;

                string valorEsperado = componentePreCTe?.Valor.ToString("n2") ?? "0,00";
                string valorRecebido = componenteCTe?.Valor.ToString("n2") ?? "0,00";

                retorno.Add(new
                {
                    Componente = Descricao,
                    ValorEsperado = valorEsperado,
                    ValorRecebido = valorRecebido,
                    icon = valorEsperado == valorRecebido ? "fa fa-check" : "fa fa-ban",
                    color = valorEsperado == valorRecebido ? "green" : "red",
                    bgColor = valorEsperado == valorRecebido ? "" : "rgba(201, 76, 76, 0.3)"
                });
            }

            return retorno;
        }

        #endregion Métodos Públicos
    }
}
