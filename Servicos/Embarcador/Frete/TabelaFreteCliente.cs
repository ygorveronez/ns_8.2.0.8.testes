using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Frete
{
    public sealed class TabelaFreteCliente
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private SituacaoItemParametroBaseCalculoTabelaFrete? _situacaoItemValorAlterado;

        #endregion Atributos

        #region Construtores

        public TabelaFreteCliente(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void AdicionarParametroModeloReboqueOuTracao(IList<int> tabelasFreteCliente, int codigoObjeto)
        {
            Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repositorioParametro = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(_unitOfWork);

            StringBuilder sql = new StringBuilder("");

            for (int i = 0; i < tabelasFreteCliente.Count; i++)
            {
                sql.Append($"({codigoObjeto}, {tabelasFreteCliente[i]})");

                if (i < tabelasFreteCliente.Count)
                    sql.Append(", ");
            }

            List<string> listaValoresSql = sql.ToString().Split(new String[] { ")," }, StringSplitOptions.None).ToList();
            for (int i = 0; i < listaValoresSql.Count; i++)
                listaValoresSql[i] = listaValoresSql[i] + ')';

            listaValoresSql.RemoveAt(listaValoresSql.Count - 1);

            if (!string.IsNullOrWhiteSpace(sql.ToString()))
                repositorioParametro.ExecutarInsercaoDeParametros(listaValoresSql);
        }

        private void AdicionarItemModeloReboqueOuTracao(IList<int> tabelasFreteCliente, decimal valor, TipoCampoValorTabelaFrete tipoCampo, int codigoObjeto, int componente)
        {
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItem = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);

            StringBuilder sql = new StringBuilder("");

            for (int i = 0; i < tabelasFreteCliente.Count; i++)
            {
                sql.Append($"((SELECT TOP 1 TBC_CODIGO FROM T_TABELA_FRETE_PARAMETRO_BASE_CALCULO WHERE TFC_CODIGO = {tabelasFreteCliente[i]} AND TBC_CODIGO_OBJETO = {codigoObjeto} order by tbc_codigo desc) , {(int)TipoParametroBaseTabelaFrete.ComponenteFrete}, {(int)tipoCampo}, {valor.ToString(CultureInfo.InvariantCulture)}, {componente})"); // SQL-INJECTION-SAFE

                if (i < tabelasFreteCliente.Count)
                    sql.Append(", ");
            }


            List<string> listaValoresSql = sql.ToString().Split(new String[] { ")," }, StringSplitOptions.None).ToList();
            for (int i = 0; i < listaValoresSql.Count; i++)
                listaValoresSql[i] = listaValoresSql[i] + ')';

            listaValoresSql.RemoveAt(listaValoresSql.Count - 1);

            if (!string.IsNullOrWhiteSpace(sql.ToString()))
                repositorioItem.ExecutarInsercaoDeItens(listaValoresSql, true);
        }

        private void AdicionarItemSemParametroBase(IList<int> tabelasFreteCliente, decimal valor, TipoCampoValorTabelaFrete tipoCampo, int componente)
        {
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItem = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);

            StringBuilder sql = new StringBuilder("");

            for (int i = 0; i < tabelasFreteCliente.Count; i++)
            {
                sql.Append($"({tabelasFreteCliente[i]}, {(int)TipoParametroBaseTabelaFrete.ComponenteFrete}, {(int)tipoCampo}, {valor.ToString(CultureInfo.InvariantCulture)}, {componente})");

                if (i < tabelasFreteCliente.Count)
                    sql.Append(", ");
            }

            List<string> listaValoresSql = sql.ToString().Split(new String[] { ")," }, StringSplitOptions.None).ToList();
            for (int i = 0; i < listaValoresSql.Count; i++)
                listaValoresSql[i] = listaValoresSql[i] + ')';

            listaValoresSql.RemoveAt(listaValoresSql.Count - 1);

            if (!string.IsNullOrWhiteSpace(sql.ToString()))
                repositorioItem.ExecutarInsercaoDeItens(listaValoresSql);
        }

        private Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente Duplicar(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaOriginal, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, bool armazenarValoresOriginaisNaTabelaClonada)
        {
            return Duplicar(tabelaOriginal, tabelaFrete, codigosModeloTracaoDuplicar: null, codigosModeloReboqueDuplicar: null, codigosTipoCargaDuplicar: null, armazenarValoresOriginaisNaTabelaClonada: armazenarValoresOriginaisNaTabelaClonada);
        }

        private Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente Duplicar(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaOriginal, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, List<int> codigosModeloTracaoDuplicar, List<int> codigosModeloReboqueDuplicar, List<int> codigosTipoCargaDuplicar, bool armazenarValoresOriginaisNaTabelaClonada)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItemParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.IntegracaoFrete repositorioIntegracaoFrete = new Repositorio.Embarcador.Frete.IntegracaoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaClonada = tabelaOriginal.Clonar();

            Utilidades.Object.DefinirListasGenericasComoNulas(tabelaClonada);

            if (tabelaOriginal.Origens?.Count > 0)
            {
                tabelaClonada.Origens = new List<Dominio.Entidades.Localidade>();

                foreach (Dominio.Entidades.Localidade origem in tabelaOriginal.Origens)
                    tabelaClonada.Origens.Add(origem);
            }

            if (tabelaOriginal.Destinos?.Count > 0)
            {
                tabelaClonada.Destinos = new List<Dominio.Entidades.Localidade>();

                foreach (Dominio.Entidades.Localidade destino in tabelaOriginal.Destinos)
                    tabelaClonada.Destinos.Add(destino);
            }

            if (tabelaOriginal.ClientesOrigem?.Count > 0)
            {
                tabelaClonada.ClientesOrigem = new List<Dominio.Entidades.Cliente>();

                foreach (Dominio.Entidades.Cliente clienteOrigem in tabelaOriginal.ClientesOrigem)
                    tabelaClonada.ClientesOrigem.Add(clienteOrigem);
            }

            if (tabelaOriginal.ClientesDestino?.Count > 0)
            {
                tabelaClonada.ClientesDestino = new List<Dominio.Entidades.Cliente>();

                foreach (Dominio.Entidades.Cliente clienteDestino in tabelaOriginal.ClientesDestino)
                    tabelaClonada.ClientesDestino.Add(clienteDestino);
            }

            if (tabelaOriginal.RegioesOrigem?.Count > 0)
            {
                tabelaClonada.RegioesOrigem = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

                foreach (Dominio.Entidades.Embarcador.Localidades.Regiao regiaoOrigem in tabelaOriginal.RegioesOrigem)
                    tabelaClonada.RegioesOrigem.Add(regiaoOrigem);
            }

            if (tabelaOriginal.RegioesDestino?.Count > 0)
            {
                tabelaClonada.RegioesDestino = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

                foreach (Dominio.Entidades.Embarcador.Localidades.Regiao regiaoDestino in tabelaOriginal.RegioesDestino)
                    tabelaClonada.RegioesDestino.Add(regiaoDestino);
            }

            if (tabelaOriginal.RotasOrigem?.Count > 0)
            {
                tabelaClonada.RotasOrigem = new List<Dominio.Entidades.RotaFrete>();

                foreach (Dominio.Entidades.RotaFrete rotaOrigem in tabelaOriginal.RotasOrigem)
                    tabelaClonada.RotasOrigem.Add(rotaOrigem);
            }

            if (tabelaOriginal.RotasDestino?.Count > 0)
            {
                tabelaClonada.RotasDestino = new List<Dominio.Entidades.RotaFrete>();

                foreach (Dominio.Entidades.RotaFrete rotaDestino in tabelaOriginal.RotasDestino)
                    tabelaClonada.RotasDestino.Add(rotaDestino);
            }

            if (tabelaOriginal.EstadosOrigem?.Count > 0)
            {
                tabelaClonada.EstadosOrigem = new List<Dominio.Entidades.Estado>();

                foreach (Dominio.Entidades.Estado estadoOrigem in tabelaOriginal.EstadosOrigem)
                    tabelaClonada.EstadosOrigem.Add(estadoOrigem);
            }

            if (tabelaOriginal.EstadosDestino?.Count > 0)
            {
                tabelaClonada.EstadosDestino = new List<Dominio.Entidades.Estado>();

                foreach (Dominio.Entidades.Estado estadoDestino in tabelaOriginal.EstadosDestino)
                    tabelaClonada.EstadosDestino.Add(estadoDestino);
            }

            if (tabelaOriginal.PaisesOrigem?.Count > 0)
            {
                tabelaClonada.PaisesOrigem = new List<Dominio.Entidades.Pais>();

                foreach (Dominio.Entidades.Pais paisOrigem in tabelaOriginal.PaisesOrigem)
                    tabelaClonada.PaisesOrigem.Add(paisOrigem);
            }

            if (tabelaOriginal.PaisesDestino?.Count > 0)
            {
                tabelaClonada.PaisesDestino = new List<Dominio.Entidades.Pais>();

                foreach (Dominio.Entidades.Pais paisDestino in tabelaOriginal.PaisesDestino)
                    tabelaClonada.PaisesDestino.Add(paisDestino);
            }

            if (tabelaOriginal.TiposOperacao?.Count > 0)
            {
                tabelaClonada.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tabelaOriginal.TiposOperacao)
                    tabelaClonada.TiposOperacao.Add(tipoOperacao);
            }

            if (tabelaOriginal.TiposCarga?.Count > 0)
            {
                tabelaClonada.TiposCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga in tabelaOriginal.TiposCarga)
                    tabelaClonada.TiposCarga.Add(tipoCarga);
            }

            if (tabelaOriginal.Fronteiras?.Count > 0)
            {
                tabelaClonada.Fronteiras = new List<Dominio.Entidades.Cliente>();

                foreach (Dominio.Entidades.Cliente fronteira in tabelaOriginal.Fronteiras)
                    tabelaClonada.Fronteiras.Add(fronteira);
            }

            if (tabelaOriginal.TransportadoresTerceiros?.Count > 0)
            {
                tabelaClonada.TransportadoresTerceiros = new List<Dominio.Entidades.Cliente>();

                foreach (Dominio.Entidades.Cliente transportadorTerceiro in tabelaOriginal.TransportadoresTerceiros)
                    tabelaClonada.TransportadoresTerceiros.Add(transportadorTerceiro);
            }

            repositorioTabelaFreteCliente.Inserir(tabelaClonada);

            if (tabelaOriginal.CEPsOrigem.Count > 0)
            {
                Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem repositorioTabelaFreteClienteCEPOrigem = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem(_unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem cepOrigem in tabelaOriginal.CEPsOrigem)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem cepOrigemClonado = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem()
                    {
                        CEPFinal = cepOrigem.CEPFinal,
                        CEPInicial = cepOrigem.CEPInicial,
                        TabelaFreteCliente = tabelaClonada
                    };

                    repositorioTabelaFreteClienteCEPOrigem.Inserir(cepOrigemClonado);
                }
            }

            if (tabelaOriginal.CEPsDestino.Count > 0)
            {
                Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino repositorioTabelaFreteClienteCEPDestino = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino(_unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino cepDestino in tabelaOriginal.CEPsDestino)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino cepDestinoClonado = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino()
                    {
                        CEPFinal = cepDestino.CEPFinal,
                        CEPInicial = cepDestino.CEPInicial,
                        TabelaFreteCliente = tabelaClonada
                    };

                    repositorioTabelaFreteClienteCEPDestino.Inserir(cepDestinoClonado);
                }
            }

            if (tabelaOriginal.Subcontratacoes?.Count > 0)
            {
                Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacao repositorioTabelaFreteClienteSubContratacao = new Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacao(_unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto repositorioTabelaFreteClienteSubContratacaoAcrescimoDesconto = new Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto(_unitOfWork);
                List<int> codigosSubcontratacao = (from o in tabelaOriginal.Subcontratacoes select o.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto> subcontratacoesValores = repositorioTabelaFreteClienteSubContratacaoAcrescimoDesconto.BuscarPorSubcontratacoes(codigosSubcontratacao);

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao subcontratacao in tabelaOriginal.Subcontratacoes)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao subcontratacaoClonada = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao()
                    {
                        PercentualDesconto = subcontratacao.PercentualDesconto,
                        Pessoa = subcontratacao.Pessoa,
                        TabelaFreteCliente = tabelaClonada,
                        ValorFixoSubContratacaoParcial = subcontratacao.ValorFixoSubContratacaoParcial
                    };

                    repositorioTabelaFreteClienteSubContratacao.Inserir(subcontratacaoClonada);

                    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto> valoresPorSubcontratacao = (from o in subcontratacoesValores where o.TabelaFreteClienteSubContratacao.Codigo == subcontratacao.Codigo select o).ToList();

                    foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto valorPorSubcontratacao in valoresPorSubcontratacao)
                    {
                        Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto valorPorSubcontratacaoClonada = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto()
                        {
                            Justificativa = valorPorSubcontratacao.Justificativa,
                            TabelaFreteClienteSubContratacao = subcontratacaoClonada,
                            Valor = valorPorSubcontratacao.Valor
                        };

                        repositorioTabelaFreteClienteSubContratacaoAcrescimoDesconto.Inserir(valorPorSubcontratacaoClonada);
                    }
                }
            }

            if (!(tabelaFrete?.ParametroBase.HasValue ?? false))
            {
                List<int> codigosItensOriginais = tabelaOriginal.ItensBaseCalculo?.Select(item => item.Codigo).ToList() ?? new List<int>();
                List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoesFrete = repositorioIntegracaoFrete.BuscarPorCodigosIntegracaoETipo(codigosItensOriginais, TipoIntegracaoFrete.TabelaFreteCliente);

                foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemOriginal in tabelaOriginal.ItensBaseCalculo)
                {
                    if ((codigosModeloReboqueDuplicar?.Count > 0) && (itemOriginal.TipoObjeto == TipoParametroBaseTabelaFrete.ModeloReboque) && !codigosModeloReboqueDuplicar.Any(o => o == itemOriginal.CodigoObjeto))
                        continue;

                    if ((codigosModeloTracaoDuplicar?.Count > 0) && (itemOriginal.TipoObjeto == TipoParametroBaseTabelaFrete.ModeloTracao) && !codigosModeloTracaoDuplicar.Any(o => o == itemOriginal.CodigoObjeto))
                        continue;

                    if ((codigosTipoCargaDuplicar?.Count > 0) && (itemOriginal.TipoObjeto == TipoParametroBaseTabelaFrete.TipoCarga) && !codigosTipoCargaDuplicar.Any(o => o == itemOriginal.CodigoObjeto))
                        continue;

                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemAjuste = itemOriginal.Clonar();

                    itemAjuste.TabelaFrete = tabelaClonada;

                    if (armazenarValoresOriginaisNaTabelaClonada)
                        itemAjuste.ValorOriginal = itemOriginal.Valor;

                    repositorioItemParametroBaseCalculoTabelaFrete.Inserir(itemAjuste);

                    DuplicarIntegracaoFrete(integracoesFrete, itemOriginal, itemAjuste);
                }

                if (armazenarValoresOriginaisNaTabelaClonada)
                {
                    tabelaClonada.ValorMaximoOriginal = tabelaOriginal.ValorMaximo;
                    tabelaClonada.ValorBaseOriginal = tabelaOriginal.ValorBase;
                    tabelaClonada.ValorMinimoGarantidoOriginal = tabelaOriginal.ValorMinimoGarantido;
                    tabelaClonada.ValorAjudanteExcedenteOriginal = tabelaOriginal.ValorAjudanteExcedente;
                    tabelaClonada.ValorEntregaExcedenteOriginal = tabelaOriginal.ValorEntregaExcedente;
                    tabelaClonada.ValorPacoteExcedenteOriginal = tabelaOriginal.ValorPacoteExcedente;
                    tabelaClonada.ValorPalletExcedenteOriginal = tabelaOriginal.ValorPalletExcedente;
                    tabelaClonada.ValorPesoExcedenteOriginal = tabelaOriginal.ValorPesoExcedente;
                    tabelaClonada.ValorQuilometragemExcedenteOriginal = tabelaOriginal.ValorQuilometragemExcedente;
                }
            }
            else
            {
                Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repositorioParametroBaseCalculoTabelaFrete = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(_unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroOriginal in tabelaOriginal.ParametrosBaseCalculo)
                {
                    if ((codigosModeloReboqueDuplicar?.Count > 0) && (tabelaOriginal.TabelaFrete.ParametroBase == TipoParametroBaseTabelaFrete.ModeloReboque) && !codigosModeloReboqueDuplicar.Any(o => o == parametroOriginal.CodigoObjeto))
                        continue;

                    if ((codigosModeloTracaoDuplicar?.Count > 0) && (tabelaOriginal.TabelaFrete.ParametroBase == TipoParametroBaseTabelaFrete.ModeloTracao) && !codigosModeloTracaoDuplicar.Any(o => o == parametroOriginal.CodigoObjeto))
                        continue;

                    if ((codigosTipoCargaDuplicar?.Count > 0) && (tabelaOriginal.TabelaFrete.ParametroBase == TipoParametroBaseTabelaFrete.TipoCarga) && !codigosTipoCargaDuplicar.Any(o => o == parametroOriginal.CodigoObjeto))
                        continue;

                    Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametroAjuste = parametroOriginal.Clonar();

                    parametroAjuste.ItensBaseCalculo = null;
                    parametroAjuste.TabelaFrete = tabelaClonada;

                    if (armazenarValoresOriginaisNaTabelaClonada)
                    {
                        parametroAjuste.ValorBaseOriginal = parametroOriginal.ValorBase;
                        parametroAjuste.ValorMaximoOriginal = parametroOriginal.ValorMaximo;
                        parametroAjuste.ValorMinimoGarantidoOriginal = parametroOriginal.ValorMinimoGarantido;
                        parametroAjuste.ValorAjudanteExcedenteOriginal = parametroOriginal.ValorAjudanteExcedente;
                        parametroAjuste.ValorEntregaExcedenteOriginal = parametroOriginal.ValorEntregaExcedente;
                        parametroAjuste.ValorPacoteExcedenteOriginal = parametroOriginal.ValorPacoteExcedente;
                        parametroAjuste.ValorPalletExcedenteOriginal = parametroOriginal.ValorPalletExcedente;
                        parametroAjuste.ValorPesoExcedenteOriginal = parametroOriginal.ValorPesoExcedente;
                        parametroAjuste.ValorQuilometragemExcedenteOriginal = parametroOriginal.ValorQuilometragemExcedente;
                    }

                    repositorioParametroBaseCalculoTabelaFrete.Inserir(parametroAjuste);

                    List<int> codigosItensOriginais = parametroOriginal.ItensBaseCalculo?.Select(item => item.Codigo).ToList() ?? new List<int>();
                    List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoesFrete = repositorioIntegracaoFrete.BuscarPorCodigosIntegracaoETipo(codigosItensOriginais, TipoIntegracaoFrete.TabelaFreteCliente);

                    foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemOriginal in parametroOriginal.ItensBaseCalculo)
                    {
                        if ((codigosModeloReboqueDuplicar?.Count > 0) && (itemOriginal.TipoObjeto == TipoParametroBaseTabelaFrete.ModeloReboque) && !codigosModeloReboqueDuplicar.Any(o => o == itemOriginal.CodigoObjeto))
                            continue;

                        if ((codigosModeloTracaoDuplicar?.Count) > 0 && (itemOriginal.TipoObjeto == TipoParametroBaseTabelaFrete.ModeloTracao) && !codigosModeloTracaoDuplicar.Any(o => o == itemOriginal.CodigoObjeto))
                            continue;

                        if ((codigosTipoCargaDuplicar?.Count > 0) && (itemOriginal.TipoObjeto == TipoParametroBaseTabelaFrete.TipoCarga) && !codigosTipoCargaDuplicar.Any(o => o == itemOriginal.CodigoObjeto))
                            continue;

                        Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemAjuste = itemOriginal.Clonar();

                        itemAjuste.ParametroBaseCalculo = parametroAjuste;

                        if (armazenarValoresOriginaisNaTabelaClonada)
                            itemAjuste.ValorOriginal = itemOriginal.Valor;

                        repositorioItemParametroBaseCalculoTabelaFrete.Inserir(itemAjuste);

                        DuplicarIntegracaoFrete(integracoesFrete, itemOriginal, itemAjuste);
                    }
                }
            }

            return tabelaClonada;
        }

        private void DuplicarIntegracaoFrete(List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoesFrete, Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemOriginal, Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemAjuste)
        {
            Dominio.Entidades.Embarcador.Frete.IntegracaoFrete integracaoFreteItemOriginal = integracoesFrete.Where(integracao => integracao.CodigoIntegracao == itemOriginal.Codigo).FirstOrDefault();

            if (integracaoFreteItemOriginal == null)
                return;

            Repositorio.Embarcador.Frete.IntegracaoFrete repositorioIntegracaoFrete = new Repositorio.Embarcador.Frete.IntegracaoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.IntegracaoFrete integracaoFreteItemAjuste = new Dominio.Entidades.Embarcador.Frete.IntegracaoFrete()
            {
                CodigoIntegracao = itemAjuste.Codigo,
                CodigoRetornoIntegracao = integracaoFreteItemOriginal.CodigoRetornoIntegracao,
                IntegracaoFreteOrigem = integracaoFreteItemOriginal,
                Tipo = TipoIntegracaoFrete.TabelaFreteCliente
            };

            repositorioIntegracaoFrete.Inserir(integracaoFreteItemAjuste);
        }

        private SituacaoItemParametroBaseCalculoTabelaFrete ObterSituacaoItemValorAlterado()
        {
            if (!_situacaoItemValorAlterado.HasValue)
            {
                if (new TabelaFreteAprovacaoAlcada(_unitOfWork).IsUtilizarAlcadaTabelaFrete())
                    _situacaoItemValorAlterado = SituacaoItemParametroBaseCalculoTabelaFrete.Aprovacao;
                else if (new TabelaFreteClienteIntegracao(_unitOfWork).PossuiIntegracaoControlaSituacaoItens())
                    _situacaoItemValorAlterado = SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoIntegracao;
                else
                    _situacaoItemValorAlterado = SituacaoItemParametroBaseCalculoTabelaFrete.Ativo;
            }

            return _situacaoItemValorAlterado.Value;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void AgruparTabelasFreteCliente(List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelas)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaBase = tabelas.FirstOrDefault();
            int numeroDeTabelas = tabelas.Count;

            for (int i = 1; i < numeroDeTabelas; i++)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaIndex = tabelas[i];

                foreach (Dominio.Entidades.Localidade destino in tabelaIndex.Destinos)
                {
                    if (!tabelaBase.Destinos.Contains(destino))
                        tabelaBase.Destinos.Add(destino);
                }

                tabelaIndex.Ativo = false;
                repositorioTabelaFreteCliente.Atualizar(tabelaIndex);
            }

            tabelaBase.FreteValidoParaQualquerDestino = true;
            repositorioTabelaFreteCliente.Atualizar(tabelaBase);
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente DuplicarParaAjuste(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaOriginal, Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete, List<int> codigoModeloTracao, List<int> codigoModeloReboque, List<int> codigoTipoCarga, bool aplicarAlteracoesDoAjuste)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaClonada = Duplicar(tabelaOriginal, ajusteTabelaFrete.TabelaFrete, codigoModeloTracao, codigoModeloReboque, codigoTipoCarga, armazenarValoresOriginaisNaTabelaClonada: true);

            tabelaClonada.AplicarAlteracoesDoAjuste = aplicarAlteracoesDoAjuste;
            tabelaClonada.AjusteTabelaFrete = ajusteTabelaFrete;
            tabelaClonada.TabelaOriginaria = tabelaOriginal;
            tabelaClonada.Tipo = TipoTabelaFreteCliente.Ajuste;

            repositorioTabelaFreteCliente.Atualizar(tabelaClonada);

            return tabelaClonada;
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente DuplicarParaAlteracaoVigencia(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaOriginal)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaClonada = Duplicar(tabelaOriginal, tabelaOriginal.TabelaFrete, armazenarValoresOriginaisNaTabelaClonada: false);

            tabelaClonada.TabelaOriginaria = tabelaOriginal;

            repositorioTabelaFreteCliente.Atualizar(tabelaClonada);

            return tabelaClonada;
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente DuplicarParaHistoricoAlteracao(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaOriginal, Dominio.Entidades.Usuario usuarioAlteracao)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaClonada = Duplicar(tabelaOriginal, tabelaOriginal.TabelaFrete, armazenarValoresOriginaisNaTabelaClonada: false);

            tabelaOriginal.DataHistoricoAlteracao = DateTime.Now;
            tabelaOriginal.UsuarioHistoricoAlteracao = usuarioAlteracao;

            tabelaClonada.TabelaOriginaria = tabelaOriginal;
            tabelaClonada.Tipo = TipoTabelaFreteCliente.HistoricoAlteracao;

            repositorioTabelaFreteCliente.Atualizar(tabelaOriginal);
            repositorioTabelaFreteCliente.Atualizar(tabelaClonada);

            return tabelaClonada;
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente DuplicarParaLicitacaoParticipacao(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaOriginal, Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaClonada = Duplicar(tabelaOriginal, tabelaFrete: null, armazenarValoresOriginaisNaTabelaClonada: true);

            tabelaClonada.Empresa = licitacaoParticipacao.Transportador;
            tabelaClonada.LicitacaoParticipacao = licitacaoParticipacao;
            tabelaClonada.TabelaOriginaria = tabelaOriginal;
            tabelaClonada.Tipo = TipoTabelaFreteCliente.Licitacao;

            repositorioTabelaFreteCliente.Atualizar(tabelaClonada);

            return tabelaClonada;
        }

        public void AdicionarComponentesFrete(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repositorioComponente = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete> componentes = repositorioComponente.BuscarCodigosPorTabelaFreteValorNaoInformadoNaTabela(tabelaFrete.Codigo);

            IList<int> codigosTabelasFreteCliente = new List<int>();
            decimal valor = 0m;
            TipoCampoValorTabelaFrete tipoCampo = TipoCampoValorTabelaFrete.ValorFixo;

            switch (tabelaFrete.ParametroBase)
            {
                case TipoParametroBaseTabelaFrete.ModeloReboque:
                    foreach (Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componente in componentes)
                    {
                        valor = componente.TipoCalculo == TipoCalculoComponenteTabelaFrete.Percentual ? componente.Percentual.HasValue ? componente.Percentual.Value : 0m : componente.ValorFormula.HasValue ? componente.ValorFormula.Value : 0m;
                        tipoCampo = componente.TipoCalculo == TipoCalculoComponenteTabelaFrete.Percentual ? TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal : componente.TipoCalculo == TipoCalculoComponenteTabelaFrete.ValorFixo ? TipoCampoValorTabelaFrete.AumentoValor : TipoCampoValorTabelaFrete.Desabilitado;

                        foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga reboque in tabelaFrete.ModelosReboque)
                        {
                            codigosTabelasFreteCliente = repositorioTabelaFreteCliente.BuscarTabelasQueNaoPossuemParametroModeloReboqueOuTracao(reboque.Codigo, tabelaFrete.Codigo);

                            if (codigosTabelasFreteCliente.Count > 0)
                                AdicionarParametroModeloReboqueOuTracao(codigosTabelasFreteCliente, reboque.Codigo);

                            codigosTabelasFreteCliente = repositorioTabelaFreteCliente.BuscarCodigosTabelasQueNaoPossuemComponente(componente.Codigo, tabelaFrete.Codigo, reboque.Codigo);

                            if (codigosTabelasFreteCliente.Count > 0)
                                AdicionarItemModeloReboqueOuTracao(codigosTabelasFreteCliente, valor, tipoCampo, reboque.Codigo, componente.Codigo);
                        }
                    }
                    break;
                case TipoParametroBaseTabelaFrete.ModeloTracao:
                    foreach (Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componente in componentes)
                    {
                        valor = componente.TipoCalculo == TipoCalculoComponenteTabelaFrete.Percentual ? componente.Percentual.HasValue ? componente.Percentual.Value : 0m : componente.ValorFormula.HasValue ? componente.ValorFormula.Value : 0m;
                        tipoCampo = componente.TipoCalculo == TipoCalculoComponenteTabelaFrete.Percentual ? TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal : componente.TipoCalculo == TipoCalculoComponenteTabelaFrete.ValorFixo ? TipoCampoValorTabelaFrete.AumentoValor : TipoCampoValorTabelaFrete.Desabilitado;

                        foreach (Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloTracao in tabelaFrete.ModelosTracao)
                        {
                            codigosTabelasFreteCliente = repositorioTabelaFreteCliente.BuscarTabelasQueNaoPossuemParametroModeloReboqueOuTracao(modeloTracao.Codigo, tabelaFrete.Codigo);

                            if (codigosTabelasFreteCliente.Count > 0)
                                AdicionarParametroModeloReboqueOuTracao(codigosTabelasFreteCliente, modeloTracao.Codigo);

                            codigosTabelasFreteCliente = repositorioTabelaFreteCliente.BuscarCodigosTabelasQueNaoPossuemComponente(componente.Codigo, tabelaFrete.Codigo, modeloTracao.Codigo);

                            if (codigosTabelasFreteCliente.Count > 0)
                                AdicionarItemModeloReboqueOuTracao(codigosTabelasFreteCliente, valor, tipoCampo, modeloTracao.Codigo, componente.Codigo);
                        }
                    }
                    break;
                default:
                    if (!tabelaFrete.ParametroBase.HasValue)
                    {
                        foreach (Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componente in componentes)
                        {
                            codigosTabelasFreteCliente = repositorioTabelaFreteCliente.BuscarCodigosTabelasQueNaoPossuemComponente(componente.Codigo, tabelaFrete.Codigo, 0);
                            valor = componente.TipoCalculo == TipoCalculoComponenteTabelaFrete.Percentual ? componente.Percentual.HasValue ? componente.Percentual.Value : 0m : componente.ValorFormula.HasValue ? componente.ValorFormula.Value : 0m;
                            tipoCampo = componente.TipoCalculo == TipoCalculoComponenteTabelaFrete.Percentual ? TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal : componente.TipoCalculo == TipoCalculoComponenteTabelaFrete.ValorFixo ? TipoCampoValorTabelaFrete.AumentoValor : TipoCampoValorTabelaFrete.Desabilitado;
                            if (codigosTabelasFreteCliente.Count > 0)
                                AdicionarItemSemParametroBase(codigosTabelasFreteCliente, valor, tipoCampo, componente.Codigo);
                        }
                    }
                    break;
            }
        }

        //public void DefinirValorItem(Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item, decimal valor, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, bool viaImportacao = false)
        //{
        //    string descricao = ObterDescricaoItem(item);
        //    string porImportacao = viaImportacao ? " via Importação" : string.Empty;

        //    bool isNovoItem = (item.Codigo <= 0);

        //    if (!isNovoItem && item.Situacao == SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoRetornoIntegracao)
        //        throw new ServicoException($"Não é possível alterar o valor do {descricao} (item {item.Codigo}) na atual situação.");         

        //    if (!isNovoItem && item.Valor != valor)
        //    {
        //        item.ValorOriginal = item.Valor;
        //        item.Valor = valor;
        //        item.PendenteIntegracao = true;
        //    }
        //    else if (isNovoItem)
        //    {
        //        item.Valor = valor;
        //        item.PendenteIntegracao = (valor != 0m);
        //    }

        //    if (valor > 0 && (isNovoItem || item.Valor != item.ValorOriginal))
        //    {
        //        string mensagem = isNovoItem
        //            ? $"Informou o valor do {descricao} para {item.ValorFormatado}{porImportacao}."
        //            : $"Alterou o valor do {descricao} para {item.ValorFormatado}{porImportacao}.";

        //        Servicos.Auditoria.Auditoria.Auditar(auditado, item.ParametroBaseCalculo?.TabelaFrete ?? item.TabelaFrete, mensagem, _unitOfWork);
        //    }
        //}

        public void DefinirValorItem(Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item, decimal valor)
        {
            DefinirValorItem(item, valor, auditado: null, viaImportacao: false);
        }

        public void DefinirValorItem(Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item, decimal valor, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            DefinirValorItem(item, valor, auditado, viaImportacao: false);
        }

        public void DefinirValorItem(Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item, decimal valor, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool viaImportacao)
        {
            string descricao = ObterDescricaoItem(item);
            string porImportacao = viaImportacao ? " via Importação" : string.Empty;
            SituacaoItemParametroBaseCalculoTabelaFrete situacaoItemValorAlterado = ObterSituacaoItemValorAlterado();

            if (item.Codigo > 0)
            {
                if (item.Valor != valor)
                {
                    Log.TratarErro($"[DefinirValorItem] Alterou o valor do item '{item.Codigo} -> {descricao}' de '{item.Valor}' para '{valor}'", "DefinirValorItemTabelaFreteCliente");

                    if (
                        (item.Situacao == SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoRetornoIntegracao) ||
                        (item.Situacao == SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoIntegracao)
                    )
                        throw new ServicoException($"Não é possível alterar o valor do {descricao} (item {item.Codigo}) na atual situação.");

                    if (item.Situacao == SituacaoItemParametroBaseCalculoTabelaFrete.Ativo)
                        item.ValorOriginal = item.Valor;

                    item.Valor = valor;
                    item.PendenteIntegracao = true;
                    item.Situacao = situacaoItemValorAlterado;
                }

                Servicos.Auditoria.Auditoria.Auditar(auditado, item.ParametroBaseCalculo?.TabelaFrete ?? item.TabelaFrete, $"Alterou o valor do {descricao} para {item.ValorFormatado}{porImportacao}.", _unitOfWork);
            }
            else
            {
                item.Valor = valor;
                item.PendenteIntegracao = (item.Valor != 0m);
                item.Situacao = (item.Valor != 0m) ? situacaoItemValorAlterado : SituacaoItemParametroBaseCalculoTabelaFrete.Ativo;

                if (item.Valor > 0)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, item.ParametroBaseCalculo?.TabelaFrete ?? item.TabelaFrete, $"Informou o valor do {descricao} para {item.ValorFormatado}{porImportacao}.", _unitOfWork);
            }
        }

        public void SalvarValores(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosValores parametrosValores, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametroBaseCalculo = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemParametroBaseCalculo = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);

            dynamic valores = parametrosValores.Valores;
            dynamic observacoes = parametrosValores.Observacoes;
            dynamic valoresMinimosGarantidos = parametrosValores.ValoresMinimosGarantidos;
            dynamic valoresMaximos = parametrosValores.ValoresMaximos;
            dynamic valoresBases = parametrosValores.ValoresBases;
            dynamic valoresExcedentes = parametrosValores.ValoresExcedentes;
            dynamic percentuaisPagamentoAgregados = parametrosValores.PercentuaisPagamentoAgregados;

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete = parametrosValores.TabelaFreteCliente;
            List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> parametros = repParametroBaseCalculo.BuscarPorTabelaFrete(tabelaFrete.Codigo);

            List<int> codigosParametrosBase = new List<int>();
            List<KeyValuePair<int, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete>> codigosItens = new List<KeyValuePair<int, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete>>();

            List<int> listaCodigosItens = new List<int>();

            foreach (dynamic val in valores)
            {
                int codigoItemBase = (int)val.CodigoItemBase;
                int codigoItem = (int)val.Codigo;

                decimal.TryParse((string)val.Valor, out decimal valorItem);

                if ((int)val.TipoValor == (int)TipoCampoValorTabelaFrete.AumentoPercentual && valorItem > 1000000)
                    throw new ServicoException("Não é possível informar uma porcentagem superior a 100%");

                Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = null;

                if (codigoItemBase > 0)
                {
                    parametro = parametros.Find(o => o.CodigoObjeto == codigoItemBase);

                    if (parametro == null)
                    {
                        parametro = new Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete()
                        {
                            CodigoObjeto = codigoItemBase,
                            TabelaFrete = tabelaFrete
                        };

                        repParametroBaseCalculo.Inserir(parametro);
                        parametros.Add(parametro);
                    }
                    if (!codigosParametrosBase.Contains(parametro.Codigo))
                        codigosParametrosBase.Add(parametro.Codigo);
                }

                listaCodigosItens.Add(codigoItem);
            }

            List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itensParametroBaseCalculoTabelaFrete = new List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            if (listaCodigosItens.Count < 2000)
            {
                itensParametroBaseCalculoTabelaFrete.AddRange(repItemParametroBaseCalculo.BuscarPorCodigosItens(listaCodigosItens, tabelaFrete.Codigo));

                if (itensParametroBaseCalculoTabelaFrete.Count == 0)
                    itensParametroBaseCalculoTabelaFrete.AddRange(repItemParametroBaseCalculo.BuscarPorCodigosItensPorTabelaFrete(listaCodigosItens, tabelaFrete.Codigo));

                if (itensParametroBaseCalculoTabelaFrete.Count == 0)
                    itensParametroBaseCalculoTabelaFrete.AddRange(repItemParametroBaseCalculo.BuscarPorCodigosItensComParametroBaseCalculo(listaCodigosItens, tabelaFrete.Codigo));
            }
            else
            {
                decimal decimalBlocos = Math.Ceiling(((decimal)listaCodigosItens.Count) / 1000);
                int blocos = (int)Math.Truncate(decimalBlocos);

                for (int i = 0; i < blocos; i++)
                    itensParametroBaseCalculoTabelaFrete.AddRange(repItemParametroBaseCalculo.BuscarPorCodigosItens(listaCodigosItens.Skip(i * 1000).Take(1000).ToList(), tabelaFrete.Codigo));

                if (itensParametroBaseCalculoTabelaFrete.Count == 0)
                {
                    for (int i = 0; i < blocos; i++)
                        itensParametroBaseCalculoTabelaFrete.AddRange(repItemParametroBaseCalculo.BuscarPorCodigosItensPorTabelaFrete(listaCodigosItens.Skip(i * 1000).Take(1000).ToList(), tabelaFrete.Codigo));
                }

                if (itensParametroBaseCalculoTabelaFrete.Count == 0)
                {
                    for (int i = 0; i < blocos; i++)
                        itensParametroBaseCalculoTabelaFrete.AddRange(repItemParametroBaseCalculo.BuscarPorCodigosItensComParametroBaseCalculo(listaCodigosItens.Skip(i * 1000).Take(1000).ToList(), tabelaFrete.Codigo));
                }
            }

            foreach (dynamic valor in valores)
            {
                int codigoItemBase = (int)valor.CodigoItemBase;
                int codigoItem = (int)valor.Codigo;

                Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = parametros.Find(o => o.CodigoObjeto == codigoItemBase);

                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = null;

                if (codigoItemBase > 0)
                {
                    item = itensParametroBaseCalculoTabelaFrete.Where(o => o.CodigoObjeto == codigoItem &&
                    o.TipoObjeto == (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete)(int)valor.Tipo &&
                    o.ParametroBaseCalculo?.CodigoObjeto == codigoItemBase && o.ParametroBaseCalculo?.TabelaFrete?.Codigo == tabelaFrete.Codigo).FirstOrDefault();
                }
                else
                {
                    item = itensParametroBaseCalculoTabelaFrete.Where(o => o.CodigoObjeto == codigoItem &&
                    o.TipoObjeto == (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete)(int)valor.Tipo &&
                    o.TabelaFrete?.Codigo == tabelaFrete.Codigo).FirstOrDefault();
                }

                if (item == null)
                {
                    item = new Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete();
                    item.CodigoObjeto = codigoItem;
                    item.ParametroBaseCalculo = parametro;
                    item.TabelaFrete = item.ParametroBaseCalculo == null ? tabelaFrete : null;
                    item.TipoObjeto = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete)(int)valor.Tipo;
                }
                else
                    item.Initialize();

                item.TipoValor = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete)(int)valor.TipoValor;

                decimal valorItem = 0m;
                decimal.TryParse((string)valor.Valor, out valorItem);

                DefinirValorItem(item, valorItem, auditado);

                if (item.Codigo > 0)
                    repItemParametroBaseCalculo.Atualizar(item);
                else
                    repItemParametroBaseCalculo.Inserir(item);

                codigosItens.Add(new KeyValuePair<int, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete>(item.CodigoObjeto, item.TipoObjeto));
            }

            if (tabelaFrete.ParametrosBaseCalculo != null)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro in tabelaFrete.ParametrosBaseCalculo)
                {
                    if (parametro.ItensBaseCalculo != null)
                    {
                        foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametro in parametro.ItensBaseCalculo)
                        {
                            if (!(from obj in codigosItens where obj.Key == itemParametro.CodigoObjeto && obj.Value == itemParametro.TipoObjeto select obj).Any())
                            {
                                repItemParametroBaseCalculo.Deletar(itemParametro);
                            }
                        }
                    }
                }
            }

            if (tabelaFrete.ItensBaseCalculo != null)
            {
                foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete itemParametro in tabelaFrete.ItensBaseCalculo)
                {
                    if (!(from obj in codigosItens where obj.Key == itemParametro.CodigoObjeto && obj.Value == itemParametro.TipoObjeto select obj).Any())
                    {
                        repItemParametroBaseCalculo.Deletar(itemParametro);
                    }
                }
            }

            foreach (dynamic valorBase in valoresBases)
            {
                int codigoItem = (int)valorBase.CodigoItemBase;

                decimal.TryParse((string)valorBase.ValorBase, out decimal valorItem);

                if (codigoItem <= 0)
                {
                    Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);

                    tabelaFrete.ValorBaseOriginal = (tabelaFrete.ValorBase != valorItem) ? tabelaFrete.ValorBase : tabelaFrete.ValorBaseOriginal;
                    tabelaFrete.ValorBase = valorItem;

                    repTabelaFrete.Atualizar(tabelaFrete);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repParametroBaseCalculo.Buscar(tabelaFrete.Codigo, codigoItem);

                    if (parametro == null)
                    {
                        parametro = new Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete()
                        {
                            CodigoObjeto = codigoItem,
                            ValorBase = valorItem,
                            TabelaFrete = tabelaFrete
                        };

                        repParametroBaseCalculo.Inserir(parametro);

                    }
                    else
                    {
                        parametro.Initialize();

                        parametro.ValorBaseOriginal = (parametro.ValorBase != valorItem) ? parametro.ValorBase : parametro.ValorBaseOriginal;
                        parametro.ValorBase = valorItem;

                        repParametroBaseCalculo.Atualizar(parametro);

                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = parametro.GetChanges();
                        if (alteracoes.Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, tabelaFrete, alteracoes, "Alterou o valor base do parâmetro " + parametro.CodigoObjeto + ".", _unitOfWork);
                    }

                    if (!codigosParametrosBase.Contains(parametro.Codigo))
                        codigosParametrosBase.Add(parametro.Codigo);
                }
            }

            List<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete> parametrosDeletar = repParametroBaseCalculo.BuscarDiff(tabelaFrete.Codigo, codigosParametrosBase.ToArray());
            foreach (Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro in parametrosDeletar)
            {
                List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens = repItemParametroBaseCalculo.Buscar(parametro.Codigo);

                foreach (Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item in itens)
                    repItemParametroBaseCalculo.Deletar(item);

                repParametroBaseCalculo.Deletar(parametro);
            }

            foreach (dynamic observacao in observacoes)
            {
                int codigoItem = (int)observacao.CodigoItemBase;

                if (codigoItem <= 0)
                {
                    Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);

                    tabelaFrete.Observacao = (string)observacao.Observacao;
                    tabelaFrete.ObservacaoTerceiro = (string)observacao.ObservacaoTerceiro;
                    tabelaFrete.ImprimirObservacaoCTe = observacao.ImprimirObservacaoCTe != null ? (bool)observacao.ImprimirObservacaoCTe : false;

                    repTabelaFrete.Atualizar(tabelaFrete);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repParametroBaseCalculo.Buscar(tabelaFrete.Codigo, codigoItem);

                    if (parametro != null)
                    {
                        parametro.Initialize();
                        parametro.Observacao = (string)observacao.Observacao;
                        parametro.ObservacaoTerceiro = (string)observacao.ObservacaoTerceiro;
                        parametro.ImprimirObservacaoCTe = observacao.ImprimirObservacaoCTe != null ? (bool)observacao.ImprimirObservacaoCTe : false;
                        repParametroBaseCalculo.Atualizar(parametro);
                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = parametro.GetChanges();
                        if (alteracoes.Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, tabelaFrete, alteracoes, "Alterou a observação do parâmetro " + parametro.CodigoObjeto + " .", _unitOfWork);
                    }
                }
            }

            foreach (dynamic valorMinimoGarantido in valoresMinimosGarantidos)
            {
                int codigoItem = (int)valorMinimoGarantido.CodigoItemBase;

                if (codigoItem <= 0)
                {
                    Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);
                    decimal valorItem = 0m;
                    decimal.TryParse((string)valorMinimoGarantido.ValorMinimoGarantido, out valorItem);

                    tabelaFrete.ValorMinimoGarantidoOriginal = (tabelaFrete.ValorMinimoGarantido != valorItem) ? tabelaFrete.ValorMinimoGarantido : tabelaFrete.ValorMinimoGarantidoOriginal;
                    tabelaFrete.ValorMinimoGarantido = valorItem;

                    repTabelaFrete.Atualizar(tabelaFrete);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repParametroBaseCalculo.Buscar(tabelaFrete.Codigo, codigoItem);
                    if (parametro != null)
                    {
                        parametro.Initialize();
                        decimal valorItem = 0m;
                        decimal.TryParse((string)valorMinimoGarantido.ValorMinimoGarantido, out valorItem);

                        parametro.ValorMinimoGarantidoOriginal = (parametro.ValorMinimoGarantido != valorItem) ? parametro.ValorMinimoGarantido : parametro.ValorMinimoGarantidoOriginal;
                        parametro.ValorMinimoGarantido = valorItem;

                        repParametroBaseCalculo.Atualizar(parametro);

                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = parametro.GetChanges();
                        if (alteracoes.Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, tabelaFrete, alteracoes, "Alterou o valor Mínimo garantido do parâmetro " + parametro.CodigoObjeto + ".", _unitOfWork);
                    }
                }
            }

            foreach (dynamic valorMaximo in valoresMaximos)
            {
                int codigoItem = (int)valorMaximo.CodigoItemBase;

                decimal.TryParse((string)valorMaximo.ValorMaximo, out decimal valorItem);

                if (codigoItem <= 0)
                {
                    Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFreteCliente(_unitOfWork);

                    tabelaFrete.ValorMaximoOriginal = (tabelaFrete.ValorMaximo != valorItem) ? tabelaFrete.ValorMaximo : tabelaFrete.ValorMaximoOriginal;
                    tabelaFrete.ValorMaximo = valorItem;

                    repTabelaFrete.Atualizar(tabelaFrete);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repParametroBaseCalculo.Buscar(tabelaFrete.Codigo, codigoItem);
                    if (parametro != null)
                    {
                        parametro.Initialize();

                        parametro.ValorMaximoOriginal = (parametro.ValorMaximo != valorItem) ? parametro.ValorMaximo : parametro.ValorMaximoOriginal;
                        parametro.ValorMaximo = valorItem;

                        repParametroBaseCalculo.Atualizar(parametro);

                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = parametro.GetChanges();
                        if (alteracoes.Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, tabelaFrete, alteracoes, "Alterou o valor máximo do parâmetro " + parametro.CodigoObjeto + ".", _unitOfWork);
                    }
                }
            }

            foreach (dynamic valorExcedente in valoresExcedentes)
            {
                int codigoItem = (int)valorExcedente.CodigoItemBase;

                decimal.TryParse((string)valorExcedente.Valor, out decimal valorItem);

                if (codigoItem <= 0)
                {
                    switch ((string)valorExcedente.Tipo)
                    {
                        case "PacoteExcedente":
                            tabelaFrete.ValorPacoteExcedenteOriginal = (tabelaFrete.ValorPacoteExcedente != valorItem) ? tabelaFrete.ValorPacoteExcedente : tabelaFrete.ValorPacoteExcedenteOriginal;
                            tabelaFrete.ValorPacoteExcedente = valorItem;
                            break;
                        case "EntregaExcedente":
                            tabelaFrete.ValorEntregaExcedenteOriginal = (tabelaFrete.ValorEntregaExcedente != valorItem) ? tabelaFrete.ValorEntregaExcedente : tabelaFrete.ValorEntregaExcedenteOriginal;
                            tabelaFrete.ValorEntregaExcedente = valorItem;
                            break;
                        case "PalletExcedente":
                            tabelaFrete.ValorPalletExcedenteOriginal = (tabelaFrete.ValorPalletExcedente != valorItem) ? tabelaFrete.ValorPalletExcedente : tabelaFrete.ValorPalletExcedenteOriginal;
                            tabelaFrete.ValorPalletExcedente = valorItem;
                            break;
                        case "PesoExcedente":
                            tabelaFrete.ValorPesoExcedenteOriginal = (tabelaFrete.ValorPesoExcedente != valorItem) ? tabelaFrete.ValorPesoExcedente : tabelaFrete.ValorPesoExcedenteOriginal;
                            tabelaFrete.ValorPesoExcedente = valorItem;
                            break;
                        case "QuilometragemExcedente":
                            tabelaFrete.ValorQuilometragemExcedenteOriginal = (tabelaFrete.ValorQuilometragemExcedente != valorItem) ? tabelaFrete.ValorQuilometragemExcedente : tabelaFrete.ValorQuilometragemExcedenteOriginal;
                            tabelaFrete.ValorQuilometragemExcedente = valorItem;
                            break;
                        case "AjudanteExcedente":
                            tabelaFrete.ValorAjudanteExcedenteOriginal = (tabelaFrete.ValorAjudanteExcedente != valorItem) ? tabelaFrete.ValorAjudanteExcedente : tabelaFrete.ValorAjudanteExcedenteOriginal;
                            tabelaFrete.ValorAjudanteExcedente = valorItem;
                            break;
                        case "HoraExcedente":
                            tabelaFrete.ValorHoraExcedente = valorItem;
                            break;
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repParametroBaseCalculo.Buscar(tabelaFrete.Codigo, codigoItem);

                    if (parametro != null)
                    {
                        parametro.Initialize();
                        switch ((string)valorExcedente.Tipo)
                        {
                            case "PacoteExcedente":
                                parametro.ValorPacoteExcedenteOriginal = (parametro.ValorPacoteExcedente != valorItem) ? parametro.ValorPacoteExcedente : parametro.ValorPacoteExcedenteOriginal;
                                parametro.ValorPacoteExcedente = valorItem;
                                break;
                            case "EntregaExcedente":
                                parametro.ValorEntregaExcedenteOriginal = (parametro.ValorEntregaExcedente != valorItem) ? parametro.ValorEntregaExcedente : parametro.ValorEntregaExcedenteOriginal;
                                parametro.ValorEntregaExcedente = valorItem;
                                break;
                            case "PalletExcedente":
                                parametro.ValorPalletExcedenteOriginal = (parametro.ValorPalletExcedente != valorItem) ? parametro.ValorPalletExcedente : parametro.ValorPalletExcedenteOriginal;
                                parametro.ValorPalletExcedente = valorItem;
                                break;
                            case "PesoExcedente":
                                parametro.ValorPesoExcedenteOriginal = (parametro.ValorPesoExcedente != valorItem) ? parametro.ValorPesoExcedente : parametro.ValorPesoExcedenteOriginal;
                                parametro.ValorPesoExcedente = valorItem;
                                break;
                            case "QuilometragemExcedente":
                                parametro.ValorQuilometragemExcedenteOriginal = (parametro.ValorQuilometragemExcedente != valorItem) ? parametro.ValorQuilometragemExcedente : parametro.ValorQuilometragemExcedenteOriginal;
                                parametro.ValorQuilometragemExcedente = valorItem;
                                break;
                            case "AjudanteExcedente":
                                parametro.ValorAjudanteExcedenteOriginal = (parametro.ValorAjudanteExcedente != valorItem) ? parametro.ValorAjudanteExcedente : parametro.ValorAjudanteExcedenteOriginal;
                                parametro.ValorAjudanteExcedente = valorItem;
                                break;
                            case "HoraExcedente":
                                parametro.ValorHoraExcedente = valorItem;
                                break;
                        }

                        repParametroBaseCalculo.Atualizar(parametro);

                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = parametro.GetChanges();
                        if (alteracoes.Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, tabelaFrete, alteracoes, "Alterou o valor excedente do parâmetro " + parametro.CodigoObjeto + ".", _unitOfWork);
                    }
                }
            }

            foreach (dynamic percentualPagamentoAgregado in percentuaisPagamentoAgregados)
            {
                int codigoItem = (int)percentualPagamentoAgregado.CodigoItemBase;

                decimal.TryParse((string)percentualPagamentoAgregado.Valor, out decimal valorItem);

                if (codigoItem <= 0)
                {
                    tabelaFrete.PercentualPagamentoAgregado = valorItem;
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repParametroBaseCalculo.Buscar(tabelaFrete.Codigo, codigoItem);

                    if (parametro != null)
                    {
                        parametro.Initialize();

                        parametro.PercentualPagamentoAgregado = valorItem;

                        repParametroBaseCalculo.Atualizar(parametro);

                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = parametro.GetChanges();

                        if (alteracoes.Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, tabelaFrete, alteracoes, "Alterou o percentual de pagamento ao agregado do parâmetro " + parametro.CodigoObjeto + ".", _unitOfWork);
                    }
                }
            }
        }

        public async Task<(bool Sucesso, string Menssagem)> InformarValorManualAsync(Dominio.ObjetosDeValor.WebService.Rest.Frete.DadosInformarValorFreteOperador dadosValorFreteOperador, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica, CancellationToken cancellationToken, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (carga == null)
                throw new ServicoException("Carga informada não encontrada.");

            if (carga.ObrigatorioInformarValorFreteOperador && (carga.TabelaFrete == null || carga.ValorFreteTabelaFrete == 0))
                throw new ServicoException("Os valores do frete estão sendo calculados, não sendo possível alterá-lo.");

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            Repositorio.Embarcador.Cargas.MotivoSolicitacaoFrete repMotivoSolicitacaoFrete = new Repositorio.Embarcador.Cargas.MotivoSolicitacaoFrete(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete motivoSolicitacaoFrete = await repMotivoSolicitacaoFrete.BuscarPorCodigoAsync(dadosValorFreteOperador.CodigoMotivo, cancellationToken);

            if (carga != null && carga.TipoOperacao != null && carga.TipoOperacao.NaoPermitirAlterarValorFreteNaCarga)
                throw new ServicoException("O tipo de operação desta carga não permite a alteração do valor da mesma.");

            if (await serCarga.VerificarSeCargaEmEmissaoAsync(carga, tipoServicoMultisoftware))
            {
                throw new ServicoException("Não é possível recalcular o frete pois a carga " + carga.CodigoCargaEmbarcador + " está em processo de emissão neste momento. ");
            }

            bool podeCalcular = serCarga.PermiteInformarFreteManualmenteNaSituacaoAtual(carga);

            if (!podeCalcular)
                throw new ServicoException("Não é possível informar o frete na atual situação da carga (" + carga.DescricaoSituacaoCarga + ").");

            if (carga.CargaAgrupamento != null || carga.CargaVinculada != null)
                throw new ServicoException("A carga foi agrupada, sendo assim não é possível alterá-la.");

            if (carga.CalculandoFrete)
                throw new ServicoException("Os valores do frete estão sendo calculados, não sendo possível alterar o valor do frete.");

            if (configuracaoEmbarcador.ObrigarMotivoSolicitacaoFrete && motivoSolicitacaoFrete == null)
                throw new ServicoException("É obrigatório informar um Motivo Solicitação de Frete.");

            Repositorio.Embarcador.Cargas.CargaRotaFrete repRotaCargaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = await repRotaCargaFrete.BuscarPorCargaAsync(carga.Codigo);

            bool possuiRoteirizacaoPendente = (configuracaoEmbarcador.ExigirRotaRoteirizadaNaCarga || configuracaoEmbarcador.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && cargaRotaFrete == null;
            if (possuiRoteirizacaoPendente && (carga.TipoOperacao == null || !carga.TipoOperacao.NaoExigeRotaRoteirizada))
            {
                if (carga.Rota == null)
                    throw new ServicoException("Para informar o valor essa carga é necessário que ela tenha uma rota");
                else
                {
                    if (carga.Rota.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Aguardando)
                        throw new ServicoException("A rota da carga está em processo de roteirização, por favor, aguarde alguns instantes e tente novamente.");
                    else
                    {
                        string mensagemRota = "";
                        mensagemRota = "Não foi possível roteririzar a rota da carga.";
                        if (!string.IsNullOrWhiteSpace(carga.Rota.MotivoFalhaRoteirizacao))
                            mensagemRota += " Motivo (" + carga.Rota.MotivoFalhaRoteirizacao + ").";

                        mensagemRota += " Por favor, acesse o cadastro de rotas e conclua a roteirização da rota " + carga.Rota.Descricao + "  manualmente. ";

                        throw new ServicoException(mensagemRota);
                    }
                }
            }

            string mensagemRetorno = null;
            if (carga.TabelaFrete != null && carga.TabelaFrete.UtilizarValorDaTabelaMesmoInformandoUmValorDeFreteOperador)
            {
                carga.ValorFreteOperador = dadosValorFreteOperador.ValorFrete;
                mensagemRetorno = $"Informou manualmente o valor de Frete Operador: {carga.ValorFreteOperador}";
            }
            else
            {
                carga.MotivoSolicitacaoFrete = motivoSolicitacaoFrete;
                carga.ObservacaoSolicitacaoFrete = dadosValorFreteOperador.Observacao;
                if (usuario != null)
                    carga.DadosSumarizados.UsuarioAlteracaoFrete = usuario.Nome;
                dadosValorFreteOperador.FreteFilialEmissoraOperador = dadosValorFreteOperador.FreteFilialEmissoraOperador && (configuracaoGeralCarga.PermiteInformarFreteOperadorFilialEmissora ?? false);

                if (configuracaoEmbarcador.UtilizaMoedaEstrangeira)
                {
                    carga.Moeda = dadosValorFreteOperador.Moeda;
                    carga.ValorCotacaoMoeda = dadosValorFreteOperador.ValorCotacaoMoeda;
                    carga.ValorTotalMoeda = dadosValorFreteOperador.ValorTotalMoeda;
                }

                if (carga.FixarUtilizacaoContratoTransportador)
                {
                    carga.FixarUtilizacaoContratoTransportador = false;
                    carga.ContratoFreteTransportador = null;
                    carga.TabelaFrete = null;
                }

                await _unitOfWork.StartAsync();

                string mensagem = serCarga.SalvarValorFreteManual(usuario, operadorLogistica, tipoServicoMultisoftware, dadosValorFreteOperador.CodigoCarga, dadosValorFreteOperador.ValorFrete, _unitOfWork, dadosValorFreteOperador.FreteFilialEmissoraOperador);

                if (mensagem != null)
                    throw new ServicoException(mensagem);
                else
                {
                    mensagemRetorno = "Informou manualmente o valor de " + dadosValorFreteOperador.ValorFrete + " de frete";

                    if (dadosValorFreteOperador.FreteFilialEmissoraOperador)
                        mensagemRetorno += " da filial emissora.";

                }
            }

            if (dadosValorFreteOperador.AvancarCarga)
            {
                carga.DataEnvioUltimaNFe = DateTime.Now;
                carga.DataRecebimentoUltimaNFe = DateTime.Now;
            }

            await _unitOfWork.StartAsync();
            await Servicos.Auditoria.Auditoria.AuditarAsync(auditado, carga, null, mensagemRetorno, _unitOfWork);

            await repCarga.AtualizarAsync(carga);
            await _unitOfWork.CommitChangesAsync();

            return (true, mensagemRetorno);
        }

        public string ObterDescricaoItem(Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item)
        {
            string descricao = item.Descricao;

            if (item.TipoObjeto == TipoParametroBaseTabelaFrete.ComponenteFrete)
            {
                Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repositorioComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(_unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFrete componenteFreteTabelaFrete = repositorioComponenteFreteTabelaFrete.BuscarPorCodigo(item.CodigoObjeto);

                if (componenteFreteTabelaFrete != null)
                    descricao += $" ({componenteFreteTabelaFrete.ComponenteFrete.Descricao})";
            }

            return descricao;
        }

        #endregion Métodos Públicos
    }
}
