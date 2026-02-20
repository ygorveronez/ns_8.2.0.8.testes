using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize(new string[] { "ObterDetalhes" }, "Configuracoes/ConfiguracaoFinanceira")]
    public class ConfiguracaoFinanceiraController : BaseController
    {
		#region Construtores

		public ConfiguracaoFinanceiraController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoContratoFreteTerceiros = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio repConfiguracaoFinanceiraPedagio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFinanceiraFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber repConfiguracaoFinanceiraBaixaTituloReceber = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE repConfiguracaoFinanceiraGNRE = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento repConfiguracaoFinanceiraFaturaModeloTipoMovimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao repConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoContratoFreteTerceiros = repConfiguracaoContratoFreteTerceiros.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configuracaoAcertoViagem = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio configuracaoPedagio = repConfiguracaoFinanceiraPedagio.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFatura = repConfiguracaoFinanceiraFatura.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber configuracaoBaixaTituloReceber = repConfiguracaoFinanceiraBaixaTituloReceber.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE configuracaoGNRE = repConfiguracaoFinanceiraGNRE.BuscarPrimeiroRegistro();
                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento> configuracaoFinanceiraFaturaModeloTipoMovimento = repConfiguracaoFinanceiraFaturaModeloTipoMovimento.BuscarTodosRrgistros();
                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao> configuracaoFinanceiraFaturaModeloTipoMovimentoReversao = repConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao.BuscarTodosRrgistros();

                return new JsonpResult(new
                {
                    ConfiguracaoGNRE = new
                    {
                        GerarGNREParaCTesEmitidos = configuracaoGNRE?.GerarGNREParaCTesEmitidos ?? false,
                        AlertarDisponibilidadeGNREParaCarga = configuracaoGNRE?.AlertarDisponibilidadeGNREParaCarga ?? false,
                        GerarGNREAutomaticamente = configuracaoGNRE?.GerarGNREAutomaticamente ?? false,
                        ConfiguracoesRegistros = (from o in (configuracaoGNRE?.ConfiguracoesRegistros ?? new List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro>())
                                                  select new
                                                  {
                                                      o.Codigo,
                                                      Estado = new { Codigo = o.Estado?.Sigla ?? "", Descricao = o.Estado?.Nome ?? "" },
                                                      CFOP = new { Codigo = o.CFOP?.Codigo ?? 0, Descricao = o.CFOP?.CodigoCFOP.ToString() ?? "" },
                                                      Pessoa = new { Codigo = o.Pessoa?.Codigo ?? 0, Descricao = o.Pessoa?.Descricao ?? "" },
                                                      TipoMovimento = new { Codigo = o.TipoMovimento?.Codigo ?? 0, Descricao = o.TipoMovimento?.Descricao ?? "" },
                                                      PorcentagemDesconto = o?.PorcentagemDesconto ?? 0,
                                                  }).ToList()
                    },
                    ConfiguracaoBaixaTituloReceber = new
                    {
                        GerarMovimentoAutomaticoDiferencaCotacaoMoeda = configuracaoBaixaTituloReceber?.GerarMovimentoAutomaticoDiferencaCotacaoMoeda ?? false,
                        ConfiguracoesMoedas = (from o in (configuracaoBaixaTituloReceber?.ConfiguracoesMoedas ?? new List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda>())
                                               select new
                                               {
                                                   o.Codigo,
                                                   o.Moeda,
                                                   JustificativaAcrescimo = new
                                                   {
                                                       Codigo = o.JustificativaAcrescimo?.Codigo ?? 0,
                                                       Descricao = o.JustificativaAcrescimo?.Descricao ?? string.Empty
                                                   },
                                                   JustificativaDesconto = new
                                                   {
                                                       Codigo = o.JustificativaDesconto?.Codigo ?? 0,
                                                       Descricao = o.JustificativaDesconto?.Descricao ?? string.Empty
                                                   }
                                               }).ToList()
                    },
                    ConfiguracaoContratoFreteTerceiros = new
                    {
                        GerarMovimentoAutomaticoNaGeracaoContratoFrete = configuracaoContratoFreteTerceiros?.GerarMovimentoAutomaticoNaGeracaoContratoFrete ?? false,
                        GerarMovimentoAutomaticoPorTipoOperacao = configuracaoContratoFreteTerceiros?.GerarMovimentoAutomaticoPorTipoOperacao ?? false,
                        TipoMovimentoReversaoValorPagoTerceiro = new
                        {
                            Codigo = configuracaoContratoFreteTerceiros?.TipoMovimentoReversaoValorPagoTerceiro?.Codigo ?? 0,
                            Descricao = configuracaoContratoFreteTerceiros?.TipoMovimentoReversaoValorPagoTerceiro?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoValorPagoTerceiro = new
                        {
                            Codigo = configuracaoContratoFreteTerceiros?.TipoMovimentoValorPagoTerceiro?.Codigo ?? 0,
                            Descricao = configuracaoContratoFreteTerceiros?.TipoMovimentoValorPagoTerceiro?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoValorPagoTerceiroCIOT = new
                        {
                            Codigo = configuracaoContratoFreteTerceiros?.TipoMovimentoReversaoValorPagoTerceiroCIOT?.Codigo ?? 0,
                            Descricao = configuracaoContratoFreteTerceiros?.TipoMovimentoReversaoValorPagoTerceiroCIOT?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoValorPagoTerceiroCIOT = new
                        {
                            Codigo = configuracaoContratoFreteTerceiros?.TipoMovimentoValorPagoTerceiroCIOT?.Codigo ?? 0,
                            Descricao = configuracaoContratoFreteTerceiros?.TipoMovimentoValorPagoTerceiroCIOT?.Descricao ?? string.Empty,
                        },
                        ConfiguracoesTipoOperacao = (from obj in configuracaoContratoFreteTerceiros?.ConfiguracoesTipoOperacao ?? new List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao>()
                                                     select new
                                                     {
                                                         Codigo = obj.Codigo,
                                                         TipoOperacao = new
                                                         {
                                                             Codigo = obj.TipoOperacao?.Codigo ?? 0,
                                                             Descricao = obj.TipoOperacao?.Descricao ?? string.Empty,
                                                         },
                                                         obj.DiferenciarMovimentoValorAbastecimento,
                                                         obj.DiferenciarMovimentoValorAdiantamento,
                                                         obj.DiferenciarMovimentoValorINSS,
                                                         obj.DiferenciarMovimentoValorINSSPatronal,
                                                         obj.DiferenciarMovimentoValorIRRF,
                                                         obj.DiferenciarMovimentoValorLiquido,
                                                         obj.DiferenciarMovimentoValorSaldo,
                                                         obj.DiferenciarMovimentoValorSENAT,
                                                         obj.DiferenciarMovimentoValorSEST,
                                                         obj.DiferenciarMovimentoValorTarifaSaque,
                                                         obj.DiferenciarMovimentoValorTarifaTransferencia,
                                                         obj.DiferenciarMovimentoValorTotal,
                                                         TipoMovimentoGeracaoTitulo = new
                                                         {
                                                             Codigo = obj.TipoMovimentoGeracaoTitulo?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoGeracaoTitulo?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoGeracaoTitulo = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoGeracaoTitulo?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoGeracaoTitulo?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoPagamentoViaCIOT = new
                                                         {
                                                             Codigo = obj.TipoMovimentoPagamentoViaCIOT?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoPagamentoViaCIOT?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoPagamentoViaCIOT = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoPagamentoViaCIOT?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoPagamentoViaCIOT?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorAbastecimento = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorAbastecimento?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorAbastecimento?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorAdiantamento = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorAdiantamento?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorAdiantamento?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorINSS = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorINSS?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorINSS?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorINSSPatronal = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorINSSPatronal?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorINSSPatronal?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorIRRF = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorIRRF?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorIRRF?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorLiquido = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorLiquido?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorLiquido?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorSaldo = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorSaldo?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorSaldo?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorSENAT = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorSENAT?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorSENAT?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorSEST = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorSEST?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorSEST?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorTarifaSaque = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorTarifaSaque?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorTarifaSaque?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorTarifaTransferencia = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorTarifaTransferencia?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorTarifaTransferencia?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoReversaoValorTotal = new
                                                         {
                                                             Codigo = obj.TipoMovimentoReversaoValorTotal?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoReversaoValorTotal?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorAbastecimento = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorAbastecimento?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorAbastecimento?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorAdiantamento = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorAdiantamento?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorAdiantamento?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorINSS = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorINSS?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorINSS?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorINSSPatronal = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorINSSPatronal?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorINSSPatronal?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorIRRF = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorIRRF?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorIRRF?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorLiquido = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorLiquido?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorLiquido?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorSaldo = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorSaldo?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorSaldo?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorSENAT = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorSENAT?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorSENAT?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorSEST = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorSEST?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorSEST?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorTarifaSaque = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorTarifaSaque?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorTarifaSaque?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorTarifaTransferencia = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorTarifaTransferencia?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorTarifaTransferencia?.Descricao ?? string.Empty,
                                                         },
                                                         TipoMovimentoValorTotal = new
                                                         {
                                                             Codigo = obj.TipoMovimentoValorTotal?.Codigo ?? 0,
                                                             Descricao = obj.TipoMovimentoValorTotal?.Descricao ?? string.Empty,
                                                         }
                                                     }).ToList()
                    },
                    ConfiguracaoContratoAcertoViagem = new
                    {
                        GerarMovimentoAutomaticoNoAcertoViagem = configuracaoAcertoViagem?.GerarMovimentoAutomaticoNaGeracaoAcertoViagem ?? false,
                        TipoMovimentoAbastecimentoPagoPeloMotorista = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoAbastecimentoPagoPeloMotorista?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoAbastecimentoPagoPeloMotorista?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoAbastecimentoPagoPeloMotorista = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoReversaoAbastecimentoPagoPeloMotorista?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoReversaoAbastecimentoPagoPeloMotorista?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoAbastecimentoPagoPelaEmpresa = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoAbastecimentoPagoPelaEmpresa?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoAbastecimentoPagoPelaEmpresa?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoAbastecimentoPagoPelaEmpresa = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoReversaoAbastecimentoPagoPelaEmpresa?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoReversaoAbastecimentoPagoPelaEmpresa?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoPedagioRecebidoEmbarcador = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoPedagioRecebidoEmbarcador?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoPedagioRecebidoEmbarcador?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoPedagioRecebidoEmbarcador = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoReversaoPedagioRecebidoEmbarcador?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoReversaoPedagioRecebidoEmbarcador?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoPedagioPagoPelaEmpresa = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoPedagioPagoPelaEmpresa?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoPedagioPagoPelaEmpresa?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoRevesaoPedagioPagoPelaEmpresa = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoRevesaoPedagioPagoPelaEmpresa?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoRevesaoPedagioPagoPelaEmpresa?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoPedagioPagoPeloMotorista = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoPedagioPagoPeloMotorista?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoPedagioPagoPeloMotorista?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoPedagioPagoPeloMotorista = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoReversaoPedagioPagoPeloMotorista?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoReversaoPedagioPagoPeloMotorista?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoComissaoDoMotorista = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoComissaoDoMotorista?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoComissaoDoMotorista?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoComissaoDoMotorista = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoReversaoComissaoDoMotorista?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoReversaoComissaoDoMotorista?.Descricao ?? string.Empty,
                        },
                        ContaEntradaAdiantamentoMotorista = new
                        {
                            Codigo = configuracaoAcertoViagem?.ContaEntradaAdiantamentoMotorista?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.ContaEntradaAdiantamentoMotorista?.Descricao ?? string.Empty,
                        },
                        ContaEntradaComissaoMotorista = new
                        {
                            Codigo = configuracaoAcertoViagem?.ContaEntradaComissaoMotorista?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.ContaEntradaComissaoMotorista?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoPedagioRecebidoEmbarcadorCredito = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoPedagioRecebidoEmbarcadorCredito?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoPedagioRecebidoEmbarcadorCredito?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoOutrasDespesas = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoOutrasDespesas?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoOutrasDespesas?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoOutrasDespesas = new
                        {
                            Codigo = configuracaoAcertoViagem?.TipoMovimentoReversaoOutrasDespesas?.Codigo ?? 0,
                            Descricao = configuracaoAcertoViagem?.TipoMovimentoReversaoOutrasDespesas?.Descricao ?? string.Empty,
                        }
                    },
                    ConfiguracaoPedagio = new
                    {
                        GerarMovimentoAutomaticoNoLancamentoPedagio = configuracaoPedagio?.GerarMovimentoAutomaticoNoLancamentoPedagio ?? false,
                        TipoMovimentoLancamentoPedagio = new
                        {
                            Codigo = configuracaoPedagio?.TipoMovimentoLancamentoPedagio?.Codigo ?? 0,
                            Descricao = configuracaoPedagio?.TipoMovimentoLancamentoPedagio?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoLancamentoPedagio = new
                        {
                            Codigo = configuracaoPedagio?.TipoMovimentoReversaoLancamentoPedagio?.Codigo ?? 0,
                            Descricao = configuracaoPedagio?.TipoMovimentoReversaoLancamentoPedagio?.Descricao ?? string.Empty,
                        }
                    },
                    ConfiguracaoAbastecimento = new
                    {
                        GerarMovimentoAutomaticoNoLancamentoAbastecimento = configuracaoAbastecimento?.GerarMovimentoAutomaticoNoLancamentoAbastecimento ?? false,
                        TipoMovimentoLancamentoAbastecimentoPosto = new
                        {
                            Codigo = configuracaoAbastecimento?.TipoMovimentoLancamentoAbastecimentoPosto?.Codigo ?? 0,
                            Descricao = configuracaoAbastecimento?.TipoMovimentoLancamentoAbastecimentoPosto?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoLancamentoAbastecimentoPosto = new
                        {
                            Codigo = configuracaoAbastecimento?.TipoMovimentoReversaoLancamentoAbastecimentoPosto?.Codigo ?? 0,
                            Descricao = configuracaoAbastecimento?.TipoMovimentoReversaoLancamentoAbastecimentoPosto?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoLancamentoAbastecimentoBombaPropria = new
                        {
                            Codigo = configuracaoAbastecimento?.TipoMovimentoLancamentoAbastecimentoBombaPropria?.Codigo ?? 0,
                            Descricao = configuracaoAbastecimento?.TipoMovimentoLancamentoAbastecimentoBombaPropria?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria = new
                        {
                            Codigo = configuracaoAbastecimento?.TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria?.Codigo ?? 0,
                            Descricao = configuracaoAbastecimento?.TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria?.Descricao ?? string.Empty,
                        }
                    },
                    ConfiguracaoFatura = new
                    {
                        GerarMovimentoAutomatico = configuracaoFatura?.GerarMovimentoAutomatico ?? false,
                        HabilitarGeracaoMovimentoFinanceiroPorModeloDocumento = configuracaoFatura?.GeracaoMovimentoFinanceiroPorModeloDocumento ?? false,
                        HabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao = configuracaoFatura?.GeracaoMovimentoFinanceiroPorModeloDocumentoReversao ?? false,
                        TipoMovimentoUso = new
                        {
                            Codigo = configuracaoFatura?.TipoMovimentoUso?.Codigo ?? 0,
                            Descricao = configuracaoFatura?.TipoMovimentoUso?.Descricao ?? string.Empty,
                        },
                        TipoMovimentoReversao = new
                        {
                            Codigo = configuracaoFatura?.TipoMovimentoReversao?.Codigo ?? 0,
                            Descricao = configuracaoFatura?.TipoMovimentoReversao?.Descricao ?? string.Empty,
                        },
                        GeracaoMovimentosFinanceirosPorModeloDocumento = (from obj in configuracaoFinanceiraFaturaModeloTipoMovimento
                                                                         select new
                                                                         {
                                                                             obj.Codigo,
                                                                             TipoMovimentoModeloDocumento = new { obj.TipoMovimento.Codigo, obj.TipoMovimento.Descricao },
                                                                             TipoModeloDocumentoFiscal = new { obj.ModeloDocumentoFiscal.Codigo, obj.ModeloDocumentoFiscal.Descricao }
                                                                         }),
                        GeracaoMovimentosFinanceirosPorModeloDocumentoReversao = (from obj in configuracaoFinanceiraFaturaModeloTipoMovimentoReversao
                                                                                  select new
                                                                                  {
                                                                                      obj.Codigo,
                                                                                      TipoMovimentoModeloDocumentoReversao = new { obj.TipoMovimento.Codigo, obj.TipoMovimento.Descricao },
                                                                                      TipoModeloDocumentoFiscalReversao = new { obj.ModeloDocumentoFiscal.Codigo, obj.ModeloDocumentoFiscal.Descricao }
                                                                                  })
                    }
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os detalhes das configurações.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoContratoFreteTerceiros()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool gerarMovimentoAutomaticoNaGeracaoContratoFrete;
                bool.TryParse(Request.Params("GerarMovimentoAutomaticoNaGeracaoContratoFrete"), out gerarMovimentoAutomaticoNaGeracaoContratoFrete);
                bool gerarMovimentoAutomaticoPorTipoOperacao = Request.GetBoolParam("GerarMovimentoAutomaticoPorTipoOperacao");

                int codigoTipoMovimentoValorPagoTerceiro, codigoTipoMovimentoReversaoValorPagoTerceiro, codigoTipoMovimentoReversaoValorPagoTerceiroCIOT, codigoTipoMovimentoValorPagoTerceiroCIOT;
                int.TryParse(Request.Params("TipoMovimentoReversaoValorPagoTerceiro"), out codigoTipoMovimentoReversaoValorPagoTerceiro);
                int.TryParse(Request.Params("TipoMovimentoValorPagoTerceiro"), out codigoTipoMovimentoValorPagoTerceiro);
                int.TryParse(Request.Params("TipoMovimentoReversaoValorPagoTerceiroCIOT"), out codigoTipoMovimentoReversaoValorPagoTerceiroCIOT);
                int.TryParse(Request.Params("TipoMovimentoValorPagoTerceiroCIOT"), out codigoTipoMovimentoValorPagoTerceiroCIOT);

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoContratoFreteTerceiros = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoContratoFreteTerceiros = repConfiguracaoContratoFreteTerceiros.BuscarPrimeiroRegistro();

                unidadeTrabalho.Start();

                if (configuracaoContratoFreteTerceiros == null)
                    configuracaoContratoFreteTerceiros = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros();
                else
                    configuracaoContratoFreteTerceiros.Initialize();

                configuracaoContratoFreteTerceiros.GerarMovimentoAutomaticoNaGeracaoContratoFrete = gerarMovimentoAutomaticoNaGeracaoContratoFrete;
                configuracaoContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiro = codigoTipoMovimentoReversaoValorPagoTerceiro > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorPagoTerceiro) : null;
                configuracaoContratoFreteTerceiros.TipoMovimentoValorPagoTerceiro = codigoTipoMovimentoValorPagoTerceiro > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorPagoTerceiro) : null;
                configuracaoContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiroCIOT = codigoTipoMovimentoReversaoValorPagoTerceiroCIOT > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorPagoTerceiroCIOT) : null;
                configuracaoContratoFreteTerceiros.TipoMovimentoValorPagoTerceiroCIOT = codigoTipoMovimentoValorPagoTerceiroCIOT > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorPagoTerceiroCIOT) : null;
                configuracaoContratoFreteTerceiros.GerarMovimentoAutomaticoPorTipoOperacao = gerarMovimentoAutomaticoPorTipoOperacao;

                if (gerarMovimentoAutomaticoNaGeracaoContratoFrete && (configuracaoContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiro == null || configuracaoContratoFreteTerceiros.TipoMovimentoValorPagoTerceiro == null || configuracaoContratoFreteTerceiros.TipoMovimentoReversaoValorPagoTerceiroCIOT == null || configuracaoContratoFreteTerceiros.TipoMovimentoValorPagoTerceiroCIOT == null))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para salvar.");
                }

                if (configuracaoContratoFreteTerceiros.Codigo > 0)
                    repConfiguracaoContratoFreteTerceiros.Atualizar(configuracaoContratoFreteTerceiros, Auditado);
                else
                    repConfiguracaoContratoFreteTerceiros.Inserir(configuracaoContratoFreteTerceiros, Auditado);

                SalvarConfiguracoesContratoTerceiroPorTipoOperacao(configuracaoContratoFreteTerceiros, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar a configuração para o contrato de frete de terceiros.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoAcertoViagem()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool.TryParse(Request.Params("GerarMovimentoAutomaticoNoAcertoViagem"), out bool gerarMovimentoAutomaticoNaGeracaoAcertoViagem);

                int.TryParse(Request.Params("TipoMovimentoAbastecimentoPagoPeloMotorista"), out int codigoTipoMovimentoAbastecimentoPagoPeloMotorista);
                int.TryParse(Request.Params("TipoMovimentoReversaoAbastecimentoPagoPeloMotorista"), out int codigoTipoMovimentoReversaoAbastecimentoPagoPeloMotorista);
                int.TryParse(Request.Params("TipoMovimentoAbastecimentoPagoPelaEmpresa"), out int codigoTipoMovimentoAbastecimentoPagoPelaEmpresa);
                int.TryParse(Request.Params("TipoMovimentoReversaoAbastecimentoPagoPelaEmpresa"), out int codigoTipoMovimentoReversaoAbastecimentoPagoPelaEmpresa);
                int.TryParse(Request.Params("TipoMovimentoPedagioRecebidoEmbarcador"), out int codigoTipoMovimentoPedagioRecebidoEmbarcador);
                int.TryParse(Request.Params("TipoMovimentoReversaoPedagioRecebidoEmbarcador"), out int codigoTipoMovimentoReversaoPedagioRecebidoEmbarcador);
                int.TryParse(Request.Params("TipoMovimentoPedagioRecebidoEmbarcadorCredito"), out int codigoTipoMovimentoPedagioRecebidoEmbarcadorCredito);
                int.TryParse(Request.Params("TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito"), out int codigoTipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito);
                int.TryParse(Request.Params("TipoMovimentoPedagioPagoPelaEmpresa"), out int codigoTipoMovimentoPedagioPagoPelaEmpresa);
                int.TryParse(Request.Params("TipoMovimentoRevesaoPedagioPagoPelaEmpresa"), out int codigoTipoMovimentoRevesaoPedagioPagoPelaEmpresa);
                int.TryParse(Request.Params("TipoMovimentoPedagioPagoPeloMotorista"), out int codigoTipoMovimentoPedagioPagoPeloMotorista);
                int.TryParse(Request.Params("TipoMovimentoReversaoPedagioPagoPeloMotorista"), out int codigoTipoMovimentoReversaoPedagioPagoPeloMotorista);
                int.TryParse(Request.Params("TipoMovimentoComissaoDoMotorista"), out int codigoTipoMovimentoComissaoDoMotorista);
                int.TryParse(Request.Params("TipoMovimentoReversaoComissaoDoMotorista"), out int codigoTipoMovimentoReversaoComissaoDoMotorista);
                int.TryParse(Request.Params("ContaEntradaComissaoMotorista"), out int codigoContaEntradaComissaoMotorista);
                int.TryParse(Request.Params("ContaEntradaAdiantamentoMotorista"), out int codigoContaEntradaAdiantamentoMotorista);
                int.TryParse(Request.Params("TipoMovimentoOutrasDespesas"), out int codigoTipoMovimentoOutrasDespesas);
                int.TryParse(Request.Params("TipoMovimentoReversaoOutrasDespesas"), out int codigoTipoMovimentoReversaoOutrasDespesas);

                Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configuracaoAcertoViagem = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();

                unidadeTrabalho.Start();

                if (configuracaoAcertoViagem == null)
                    configuracaoAcertoViagem = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem();
                else
                    configuracaoAcertoViagem.Initialize();

                configuracaoAcertoViagem.GerarMovimentoAutomaticoNaGeracaoAcertoViagem = gerarMovimentoAutomaticoNaGeracaoAcertoViagem;

                configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPeloMotorista = codigoTipoMovimentoAbastecimentoPagoPeloMotorista > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAbastecimentoPagoPeloMotorista) : null;
                configuracaoAcertoViagem.TipoMovimentoReversaoAbastecimentoPagoPeloMotorista = codigoTipoMovimentoReversaoAbastecimentoPagoPeloMotorista > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoAbastecimentoPagoPeloMotorista) : null;
                configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPelaEmpresa = codigoTipoMovimentoAbastecimentoPagoPelaEmpresa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAbastecimentoPagoPelaEmpresa) : null;
                configuracaoAcertoViagem.TipoMovimentoReversaoAbastecimentoPagoPelaEmpresa = codigoTipoMovimentoReversaoAbastecimentoPagoPelaEmpresa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoAbastecimentoPagoPelaEmpresa) : null;
                configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcador = codigoTipoMovimentoPedagioRecebidoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPedagioRecebidoEmbarcador) : null;
                configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcador = codigoTipoMovimentoReversaoPedagioRecebidoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoPedagioRecebidoEmbarcador) : null;
                configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcadorCredito = codigoTipoMovimentoPedagioRecebidoEmbarcadorCredito > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPedagioRecebidoEmbarcadorCredito) : null;
                configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito = codigoTipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito) : null;
                configuracaoAcertoViagem.TipoMovimentoPedagioPagoPelaEmpresa = codigoTipoMovimentoPedagioPagoPelaEmpresa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPedagioPagoPelaEmpresa) : null;
                configuracaoAcertoViagem.TipoMovimentoRevesaoPedagioPagoPelaEmpresa = codigoTipoMovimentoRevesaoPedagioPagoPelaEmpresa > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoRevesaoPedagioPagoPelaEmpresa) : null;
                configuracaoAcertoViagem.TipoMovimentoPedagioPagoPeloMotorista = codigoTipoMovimentoPedagioPagoPeloMotorista > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPedagioPagoPeloMotorista) : null;
                configuracaoAcertoViagem.TipoMovimentoReversaoPedagioPagoPeloMotorista = codigoTipoMovimentoReversaoPedagioPagoPeloMotorista > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoPedagioPagoPeloMotorista) : null;
                configuracaoAcertoViagem.TipoMovimentoComissaoDoMotorista = codigoTipoMovimentoComissaoDoMotorista > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoComissaoDoMotorista) : null;
                configuracaoAcertoViagem.TipoMovimentoReversaoComissaoDoMotorista = codigoTipoMovimentoReversaoComissaoDoMotorista > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoComissaoDoMotorista) : null;
                configuracaoAcertoViagem.ContaEntradaAdiantamentoMotorista = codigoContaEntradaAdiantamentoMotorista > 0 ? repPlanoConta.BuscarPorCodigo(codigoContaEntradaAdiantamentoMotorista) : null;
                configuracaoAcertoViagem.ContaEntradaComissaoMotorista = codigoContaEntradaComissaoMotorista > 0 ? repPlanoConta.BuscarPorCodigo(codigoContaEntradaComissaoMotorista) : null;
                configuracaoAcertoViagem.TipoMovimentoOutrasDespesas = codigoTipoMovimentoOutrasDespesas > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoOutrasDespesas) : null;
                configuracaoAcertoViagem.TipoMovimentoReversaoOutrasDespesas = codigoTipoMovimentoReversaoOutrasDespesas > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoOutrasDespesas) : null;

                if (gerarMovimentoAutomaticoNaGeracaoAcertoViagem &&
                    (configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPeloMotorista == null || configuracaoAcertoViagem.TipoMovimentoReversaoAbastecimentoPagoPeloMotorista == null
                    || configuracaoAcertoViagem.TipoMovimentoAbastecimentoPagoPelaEmpresa == null || configuracaoAcertoViagem.TipoMovimentoReversaoAbastecimentoPagoPelaEmpresa == null || configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcadorCredito == null
                    || configuracaoAcertoViagem.TipoMovimentoPedagioRecebidoEmbarcador == null || configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcador == null || configuracaoAcertoViagem.TipoMovimentoReversaoPedagioRecebidoEmbarcadorCredito == null
                    || configuracaoAcertoViagem.TipoMovimentoPedagioPagoPelaEmpresa == null || configuracaoAcertoViagem.TipoMovimentoRevesaoPedagioPagoPelaEmpresa == null
                    || configuracaoAcertoViagem.TipoMovimentoPedagioPagoPeloMotorista == null || configuracaoAcertoViagem.TipoMovimentoReversaoPedagioPagoPeloMotorista == null
                    || configuracaoAcertoViagem.TipoMovimentoComissaoDoMotorista == null || configuracaoAcertoViagem.TipoMovimentoReversaoComissaoDoMotorista == null
                    || configuracaoAcertoViagem.ContaEntradaAdiantamentoMotorista == null || configuracaoAcertoViagem.ContaEntradaComissaoMotorista == null
                    || configuracaoAcertoViagem.TipoMovimentoOutrasDespesas == null || configuracaoAcertoViagem.TipoMovimentoReversaoOutrasDespesas == null))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para salvar.");
                }

                if (configuracaoAcertoViagem.Codigo > 0)
                    repConfiguracaoFinanceiraContratoAcertoViagem.Atualizar(configuracaoAcertoViagem, Auditado);
                else
                    repConfiguracaoFinanceiraContratoAcertoViagem.Inserir(configuracaoAcertoViagem, Auditado);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar a configuração para o acerto de viagem.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoPedagio()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool gerarMovimentoAutomaticoNoLancamentoPedagio;
                bool.TryParse(Request.Params("GerarMovimentoAutomaticoNoLancamentoPedagio"), out gerarMovimentoAutomaticoNoLancamentoPedagio);

                int codigoTipoMovimentoLancamentoPedagio, codigoTipoMovimentoReversaoLancamentoPedagio;
                int.TryParse(Request.Params("TipoMovimentoLancamentoPedagio"), out codigoTipoMovimentoLancamentoPedagio);
                int.TryParse(Request.Params("TipoMovimentoReversaoLancamentoPedagio"), out codigoTipoMovimentoReversaoLancamentoPedagio);

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio repConfiguracaoFinanceiraPedagio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio configuracaoPedagio = repConfiguracaoFinanceiraPedagio.BuscarPrimeiroRegistro();

                unidadeTrabalho.Start();

                if (configuracaoPedagio == null)
                    configuracaoPedagio = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraPedagio();
                else
                    configuracaoPedagio.Initialize();

                configuracaoPedagio.GerarMovimentoAutomaticoNoLancamentoPedagio = gerarMovimentoAutomaticoNoLancamentoPedagio;
                configuracaoPedagio.TipoMovimentoLancamentoPedagio = codigoTipoMovimentoLancamentoPedagio > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoLancamentoPedagio) : null;
                configuracaoPedagio.TipoMovimentoReversaoLancamentoPedagio = codigoTipoMovimentoReversaoLancamentoPedagio > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoLancamentoPedagio) : null;

                if (gerarMovimentoAutomaticoNoLancamentoPedagio && (configuracaoPedagio.TipoMovimentoLancamentoPedagio == null || configuracaoPedagio.TipoMovimentoReversaoLancamentoPedagio == null))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para salvar.");
                }

                if (configuracaoPedagio.Codigo > 0)
                    repConfiguracaoFinanceiraPedagio.Atualizar(configuracaoPedagio, Auditado);
                else
                    repConfiguracaoFinanceiraPedagio.Inserir(configuracaoPedagio, Auditado);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar a configuração para o pedágio.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoAbastecimento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool gerarMovimentoAutomaticoNoLancamentoAbastecimento;
                bool.TryParse(Request.Params("GerarMovimentoAutomaticoNoLancamentoAbastecimento"), out gerarMovimentoAutomaticoNoLancamentoAbastecimento);

                int codigoTipMovimentoLancamentoAbastecimentoPosto, codigoTipoMovimentoReversaoLancamentoAbastecimentoPosto;
                int.TryParse(Request.Params("TipoMovimentoLancamentoAbastecimentoPosto"), out codigoTipMovimentoLancamentoAbastecimentoPosto);
                int.TryParse(Request.Params("TipoMovimentoReversaoLancamentoAbastecimentoPosto"), out codigoTipoMovimentoReversaoLancamentoAbastecimentoPosto);

                int codigoTipoMovimentoLancamentoAbastecimentoBombaPropria, codigoTipoMovimentoReversaoLancamentoAbastecimentoBombaPropria;
                int.TryParse(Request.Params("TipoMovimentoLancamentoAbastecimentoBombaPropria"), out codigoTipoMovimentoLancamentoAbastecimentoBombaPropria);
                int.TryParse(Request.Params("TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria"), out codigoTipoMovimentoReversaoLancamentoAbastecimentoBombaPropria);

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();

                unidadeTrabalho.Start();

                if (configuracaoAbastecimento == null)
                    configuracaoAbastecimento = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento();
                else
                    configuracaoAbastecimento.Initialize();

                configuracaoAbastecimento.GerarMovimentoAutomaticoNoLancamentoAbastecimento = gerarMovimentoAutomaticoNoLancamentoAbastecimento;
                configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto = codigoTipMovimentoLancamentoAbastecimentoPosto > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipMovimentoLancamentoAbastecimentoPosto) : null;
                configuracaoAbastecimento.TipoMovimentoReversaoLancamentoAbastecimentoPosto = codigoTipoMovimentoReversaoLancamentoAbastecimentoPosto > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoLancamentoAbastecimentoPosto) : null;
                configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoBombaPropria = codigoTipoMovimentoLancamentoAbastecimentoBombaPropria > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoLancamentoAbastecimentoBombaPropria) : null;
                configuracaoAbastecimento.TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria = codigoTipoMovimentoReversaoLancamentoAbastecimentoBombaPropria > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoLancamentoAbastecimentoBombaPropria) : null;

                if (gerarMovimentoAutomaticoNoLancamentoAbastecimento &&
                    (                    
                    configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoBombaPropria == null || configuracaoAbastecimento.TipoMovimentoReversaoLancamentoAbastecimentoBombaPropria == null
                    ))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para salvar.");
                }

                if (configuracaoAbastecimento.Codigo > 0)
                    repConfiguracaoFinanceiraAbastecimento.Atualizar(configuracaoAbastecimento, Auditado);
                else
                    repConfiguracaoFinanceiraAbastecimento.Inserir(configuracaoAbastecimento, Auditado);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar a configuração para o abastecimento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoFatura()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool gerarMovimentoAutomatico;
                bool.TryParse(Request.Params("GerarMovimentoAutomatico"), out gerarMovimentoAutomatico);

                int codigoTipoMovimentoUso, codigoTipoMovimentoReversao;
                int.TryParse(Request.Params("TipoMovimentoUso"), out codigoTipoMovimentoUso);
                int.TryParse(Request.Params("TipoMovimentoReversao"), out codigoTipoMovimentoReversao);

                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFatura = repConfiguracaoFatura.BuscarPrimeiroRegistro();


                unidadeTrabalho.Start();

                if (configuracaoFatura == null)
                    configuracaoFatura = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura();
                else
                    configuracaoFatura.Initialize();

                configuracaoFatura.GerarMovimentoAutomatico = gerarMovimentoAutomatico;
                configuracaoFatura.GeracaoMovimentoFinanceiroPorModeloDocumento = Request.GetBoolParam("HabilitarGeracaoMovimentoFinanceiroPorModeloDocumento");
                configuracaoFatura.GeracaoMovimentoFinanceiroPorModeloDocumentoReversao = Request.GetBoolParam("HabilitarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao");
                configuracaoFatura.TipoMovimentoUso = codigoTipoMovimentoUso > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUso) : null;
                configuracaoFatura.TipoMovimentoReversao = codigoTipoMovimentoReversao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversao) : null;

                if (gerarMovimentoAutomatico && (configuracaoFatura.TipoMovimentoUso == null || configuracaoFatura.TipoMovimentoReversao == null))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para salvar.");
                }

                if (configuracaoFatura.Codigo > 0)
                    repConfiguracaoFatura.Atualizar(configuracaoFatura, Auditado);
                else
                    repConfiguracaoFatura.Inserir(configuracaoFatura, Auditado);

                SalvarGeracaoMovimentoFinanceiroPorModeloDocumento(configuracaoFatura, unidadeTrabalho);
                SalvarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao(configuracaoFatura, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar a configuração para a fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoBaixaTituloReceber()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber repConfiguracaoFinanceiraBaixaTituloReceber = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber configuracaoFinanceiraBaixaTituloReceber = repConfiguracaoFinanceiraBaixaTituloReceber.BuscarPrimeiroRegistro();

                unidadeTrabalho.Start();

                if (configuracaoFinanceiraBaixaTituloReceber == null)
                    configuracaoFinanceiraBaixaTituloReceber = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber();
                else
                    configuracaoFinanceiraBaixaTituloReceber.Initialize();

                configuracaoFinanceiraBaixaTituloReceber.GerarMovimentoAutomaticoDiferencaCotacaoMoeda = Request.GetBoolParam("GerarMovimentoAutomaticoDiferencaCotacaoMoeda");

                if (configuracaoFinanceiraBaixaTituloReceber.Codigo > 0)
                    repConfiguracaoFinanceiraBaixaTituloReceber.Atualizar(configuracaoFinanceiraBaixaTituloReceber, Auditado);
                else
                    repConfiguracaoFinanceiraBaixaTituloReceber.Inserir(configuracaoFinanceiraBaixaTituloReceber, Auditado);

                SalvarConfiguracoesBaixaTituloReceberMoeda(configuracaoFinanceiraBaixaTituloReceber, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar a configuração para o contrato de frete de terceiros.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoGNRE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE repConfiguracaoFinanceiraGNRE = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE configuracaoFinanceiraGNRE = repConfiguracaoFinanceiraGNRE.BuscarPrimeiroRegistro();

                unidadeTrabalho.Start();

                if (configuracaoFinanceiraGNRE == null)
                    configuracaoFinanceiraGNRE = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE();
                else
                    configuracaoFinanceiraGNRE.Initialize();

                configuracaoFinanceiraGNRE.GerarGNREParaCTesEmitidos = Request.GetBoolParam("GerarGNREParaCTesEmitidos");
                configuracaoFinanceiraGNRE.AlertarDisponibilidadeGNREParaCarga = Request.GetBoolParam("AlertarDisponibilidadeGNREParaCarga");
                configuracaoFinanceiraGNRE.GerarGNREAutomaticamente = Request.GetBoolParam("GerarGNREAutomaticamente");

                if (configuracaoFinanceiraGNRE.Codigo > 0)
                    repConfiguracaoFinanceiraGNRE.Atualizar(configuracaoFinanceiraGNRE, Auditado);
                else
                    repConfiguracaoFinanceiraGNRE.Inserir(configuracaoFinanceiraGNRE, Auditado);

                SalvarConfiguracoesGNRERegistros(configuracaoFinanceiraGNRE, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar a configuração para GNRE.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #region Métodos Privados

        private void SalvarConfiguracoesGNRERegistros(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRE configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro repConfiguracaoFinanceiraGNRERegistro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

            dynamic configuracoesRegistros = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaConfiguracoesRegistros"));

            if (configuracao.ConfiguracoesRegistros != null && configuracao.ConfiguracoesRegistros.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic configuracaoRegistro in configuracoesRegistros)
                {
                    int codigo = 0;

                    if (int.TryParse((string)configuracaoRegistro.Codigo, out codigo))
                        codigos.Add((int)configuracaoRegistro.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro> configuracoesDeletar = (from obj in configuracao.ConfiguracoesRegistros where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < configuracoesDeletar.Count; i++)
                    repConfiguracaoFinanceiraGNRERegistro.Deletar(configuracoesDeletar[i]);
            }

            foreach (var configuracaoRegistro in configuracoesRegistros)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro config = null;

                int codigo = 0;

                if (configuracaoRegistro.Codigo != null && int.TryParse((string)configuracaoRegistro.Codigo, out codigo))
                    config = repConfiguracaoFinanceiraGNRERegistro.BuscarPorCodigo(codigo, false);

                if (config == null)
                    config = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraGNRERegistro();

                config.ConfiguracaoFinanceiraGNRE = configuracao;
                config.Estado = repEstado.BuscarPorSigla((string)configuracaoRegistro.Estado.Codigo);
                config.CFOP = repCFOP.BuscarPorCodigo((int)configuracaoRegistro.CFOP.Codigo);
                config.Pessoa = repCliente.BuscarPorCPFCNPJ((double)configuracaoRegistro.Pessoa.Codigo);
                config.TipoMovimento = repTipoMovimento.BuscarPorCodigo((int)configuracaoRegistro.TipoMovimento.Codigo);
                config.PorcentagemDesconto = ((string)configuracaoRegistro.PorcentagemDesconto).ToDecimal();

                if (config.Codigo > 0)
                    repConfiguracaoFinanceiraGNRERegistro.Atualizar(config);
                else
                    repConfiguracaoFinanceiraGNRERegistro.Inserir(config);
            }
        }

        private void SalvarConfiguracoesBaixaTituloReceberMoeda(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceber configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda repConfiguracaoFinanceiraBaixaTituloReceberMoeda = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);

            dynamic configuracoesMoedas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaConfiguracoesMoedas"));

            if (configuracao.ConfiguracoesMoedas != null && configuracao.ConfiguracoesMoedas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var configuracaoMoeda in configuracoesMoedas)
                {
                    int codigo = 0;

                    if (int.TryParse((string)configuracaoMoeda.Codigo, out codigo))
                        codigos.Add((int)configuracaoMoeda.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda> configuracoesDeletar = (from obj in configuracao.ConfiguracoesMoedas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < configuracoesDeletar.Count; i++)
                    repConfiguracaoFinanceiraBaixaTituloReceberMoeda.Deletar(configuracoesDeletar[i]);
            }

            foreach (var configuracaoMoeda in configuracoesMoedas)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda config = null;

                int codigo = 0;

                if (configuracaoMoeda.Codigo != null && int.TryParse((string)configuracaoMoeda.Codigo, out codigo))
                    config = repConfiguracaoFinanceiraBaixaTituloReceberMoeda.BuscarPorCodigo(codigo, false);

                if (config == null)
                    config = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraBaixaTituloReceberMoeda();

                config.ConfiguracaoFinanceiraBaixaTituloReceber = configuracao;
                config.Moeda = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral)configuracaoMoeda.Moeda;
                config.JustificativaAcrescimo = repJustificativa.BuscarPorCodigo((int)configuracaoMoeda.JustificativaAcrescimo.Codigo);
                config.JustificativaDesconto = repJustificativa.BuscarPorCodigo((int)configuracaoMoeda.JustificativaDesconto.Codigo);

                if (config.Codigo > 0)
                    repConfiguracaoFinanceiraBaixaTituloReceberMoeda.Atualizar(config);
                else
                    repConfiguracaoFinanceiraBaixaTituloReceberMoeda.Inserir(config);
            }
        }

        private void SalvarConfiguracoesContratoTerceiroPorTipoOperacao(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao repConfiguracaoTipoOperacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            dynamic configuracoesTipoOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaConfiguracoesTipoOperacao"));

            if (configuracao.ConfiguracoesTipoOperacao != null && configuracao.ConfiguracoesTipoOperacao.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var configuracaoTipoOperacao in configuracoesTipoOperacao)
                {
                    int codigo = 0;

                    if (int.TryParse((string)configuracaoTipoOperacao.Codigo, out codigo))
                        codigos.Add((int)configuracaoTipoOperacao.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao> configuracoesDeletar = (from obj in configuracao.ConfiguracoesTipoOperacao where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < configuracoesDeletar.Count; i++)
                    repConfiguracaoTipoOperacao.Deletar(configuracoesDeletar[i]);
            }

            foreach (var configuracaoTipoOperacao in configuracoesTipoOperacao)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao config = null;

                int codigo = 0;

                if (configuracaoTipoOperacao.Codigo != null && int.TryParse((string)configuracaoTipoOperacao.Codigo, out codigo))
                    config = repConfiguracaoTipoOperacao.BuscarPorCodigo(codigo, false);

                if (config == null)
                    config = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao();

                int codigoTipoOperacao = (int)configuracaoTipoOperacao.TipoOperacao.Codigo;
                int codigoTipoMovimentoGeracaoTitulo = (int)configuracaoTipoOperacao.TipoMovimentoGeracaoTitulo.Codigo;
                int codigoTipoMovimentoReversaoGeracaoTitulo = (int)configuracaoTipoOperacao.TipoMovimentoReversaoGeracaoTitulo.Codigo;
                int codigoTipoMovimentoPagamentoViaCIOT = (int)configuracaoTipoOperacao.TipoMovimentoPagamentoViaCIOT.Codigo;
                int codigoTipoMovimentoReversaoPagamentoViaCIOT = (int)configuracaoTipoOperacao.TipoMovimentoReversaoPagamentoViaCIOT.Codigo;

                bool diferenciarMovimentoValorINSS = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorINSS;
                int codigoTipoMovimentoValorINSS = (int)configuracaoTipoOperacao.TipoMovimentoValorINSS.Codigo;
                int codigoTipoMovimentoReversaoValorINSS = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorINSS.Codigo;

                bool diferenciarMovimentoValorINSSPatronal = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorINSSPatronal;
                int codigoTipoMovimentoValorINSSPatronal = (int)configuracaoTipoOperacao.TipoMovimentoValorINSSPatronal.Codigo;
                int codigoTipoMovimentoReversaoValorINSSPatronal = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorINSSPatronal.Codigo;

                bool diferenciarMovimentoValorIRRF = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorIRRF;
                int codigoTipoMovimentoValorIRRF = (int)configuracaoTipoOperacao.TipoMovimentoValorIRRF.Codigo;
                int codigoTipoMovimentoReversaoValorIRRF = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorIRRF.Codigo;

                bool diferenciarMovimentoValorSEST = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorSEST;
                int codigoTipoMovimentoValorSEST = (int)configuracaoTipoOperacao.TipoMovimentoValorSEST.Codigo;
                int codigoTipoMovimentoReversaoValorSEST = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorSEST.Codigo;

                bool diferenciarMovimentoValorSENAT = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorSENAT;
                int codigoTipoMovimentoValorSENAT = (int)configuracaoTipoOperacao.TipoMovimentoValorSENAT.Codigo;
                int codigoTipoMovimentoReversaoValorSENAT = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorSENAT.Codigo;

                bool diferenciarMovimentoValorTarifaSaque = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorTarifaSaque;
                int codigoTipoMovimentoValorTarifaSaque = (int)configuracaoTipoOperacao.TipoMovimentoValorTarifaSaque.Codigo;
                int codigoTipoMovimentoReversaoValorTarifaSaque = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorTarifaSaque.Codigo;

                bool diferenciarMovimentoValorTarifaTransferencia = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorTarifaTransferencia;
                int codigoTipoMovimentoValorTarifaTransferencia = (int)configuracaoTipoOperacao.TipoMovimentoValorTarifaTransferencia.Codigo;
                int codigoTipoMovimentoReversaoValorTarifaTransferencia = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorTarifaTransferencia.Codigo;

                bool diferenciarMovimentoValorLiquido = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorLiquido;
                int codigoTipoMovimentoValorLiquido = (int)configuracaoTipoOperacao.TipoMovimentoValorLiquido.Codigo;
                int codigoTipoMovimentoReversaoValorLiquido = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorLiquido.Codigo;

                bool diferenciarMovimentoValorAbastecimento = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorAbastecimento;
                int codigoTipoMovimentoValorAbastecimento = (int)configuracaoTipoOperacao.TipoMovimentoValorAbastecimento.Codigo;
                int codigoTipoMovimentoReversaoValorAbastecimento = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorAbastecimento.Codigo;

                bool diferenciarMovimentoValorAdiantamento = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorAdiantamento;
                int codigoTipoMovimentoValorAdiantamento = (int)configuracaoTipoOperacao.TipoMovimentoValorAdiantamento.Codigo;
                int codigoTipoMovimentoReversaoValorAdiantamento = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorAdiantamento.Codigo;

                bool diferenciarMovimentoValorSaldo = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorSaldo;
                int codigoTipoMovimentoValorSaldo = (int)configuracaoTipoOperacao.TipoMovimentoValorSaldo.Codigo;
                int codigoTipoMovimentoReversaoValorSaldo = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorSaldo.Codigo;

                bool diferenciarMovimentoValorTotal = (bool)configuracaoTipoOperacao.DiferenciarMovimentoValorTotal;
                int codigoTipoMovimentoValorTotal = (int)configuracaoTipoOperacao.TipoMovimentoValorTotal.Codigo;
                int codigoTipoMovimentoReversaoValorTotal = (int)configuracaoTipoOperacao.TipoMovimentoReversaoValorTotal.Codigo;

                config.ConfiguracaoFinanceiraContratoFreteTerceiros = configuracao;
                config.TipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;

                config.DiferenciarMovimentoValorAbastecimento = diferenciarMovimentoValorAbastecimento;
                config.DiferenciarMovimentoValorAdiantamento = diferenciarMovimentoValorAdiantamento;
                config.DiferenciarMovimentoValorINSS = diferenciarMovimentoValorINSS;
                config.DiferenciarMovimentoValorINSSPatronal = diferenciarMovimentoValorINSSPatronal;
                config.DiferenciarMovimentoValorIRRF = diferenciarMovimentoValorIRRF;
                config.DiferenciarMovimentoValorLiquido = diferenciarMovimentoValorLiquido;
                config.DiferenciarMovimentoValorSaldo = diferenciarMovimentoValorSaldo;
                config.DiferenciarMovimentoValorSENAT = diferenciarMovimentoValorSENAT;
                config.DiferenciarMovimentoValorSEST = diferenciarMovimentoValorSEST;
                config.DiferenciarMovimentoValorTarifaSaque = diferenciarMovimentoValorTarifaSaque;
                config.DiferenciarMovimentoValorTarifaTransferencia = diferenciarMovimentoValorTarifaTransferencia;
                config.DiferenciarMovimentoValorTotal = diferenciarMovimentoValorTotal;

                config.TipoMovimentoGeracaoTitulo = codigoTipoMovimentoGeracaoTitulo > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoGeracaoTitulo) : null;
                config.TipoMovimentoReversaoGeracaoTitulo = codigoTipoMovimentoReversaoGeracaoTitulo > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoGeracaoTitulo) : null;
                config.TipoMovimentoPagamentoViaCIOT = codigoTipoMovimentoPagamentoViaCIOT > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPagamentoViaCIOT) : null;
                config.TipoMovimentoReversaoPagamentoViaCIOT = codigoTipoMovimentoReversaoPagamentoViaCIOT > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoPagamentoViaCIOT) : null;
                config.TipoMovimentoReversaoValorAbastecimento = codigoTipoMovimentoReversaoValorAbastecimento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorAbastecimento) : null;
                config.TipoMovimentoReversaoValorAdiantamento = codigoTipoMovimentoReversaoValorAdiantamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorAdiantamento) : null;
                config.TipoMovimentoReversaoValorINSS = codigoTipoMovimentoReversaoValorINSS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorINSS) : null;
                config.TipoMovimentoReversaoValorINSSPatronal = codigoTipoMovimentoReversaoValorINSSPatronal > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorINSSPatronal) : null;
                config.TipoMovimentoReversaoValorIRRF = codigoTipoMovimentoReversaoValorIRRF > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorIRRF) : null;
                config.TipoMovimentoReversaoValorLiquido = codigoTipoMovimentoReversaoValorLiquido > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorLiquido) : null;
                config.TipoMovimentoReversaoValorSaldo = codigoTipoMovimentoReversaoValorSaldo > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorSaldo) : null;
                config.TipoMovimentoReversaoValorSENAT = codigoTipoMovimentoReversaoValorSENAT > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorSENAT) : null;
                config.TipoMovimentoReversaoValorSEST = codigoTipoMovimentoReversaoValorSEST > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorSEST) : null;
                config.TipoMovimentoReversaoValorTarifaSaque = codigoTipoMovimentoReversaoValorTarifaSaque > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorTarifaSaque) : null;
                config.TipoMovimentoReversaoValorTarifaTransferencia = codigoTipoMovimentoReversaoValorTarifaTransferencia > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorTarifaTransferencia) : null;
                config.TipoMovimentoReversaoValorTotal = codigoTipoMovimentoReversaoValorTotal > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoValorTotal) : null;
                config.TipoMovimentoValorAbastecimento = codigoTipoMovimentoValorAbastecimento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorAbastecimento) : null;
                config.TipoMovimentoValorAdiantamento = codigoTipoMovimentoValorAdiantamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorAdiantamento) : null;
                config.TipoMovimentoValorINSS = codigoTipoMovimentoValorINSS > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorINSS) : null;
                config.TipoMovimentoValorINSSPatronal = codigoTipoMovimentoValorINSSPatronal > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorINSSPatronal) : null;
                config.TipoMovimentoValorIRRF = codigoTipoMovimentoValorIRRF > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorIRRF) : null;
                config.TipoMovimentoValorLiquido = codigoTipoMovimentoValorLiquido > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorLiquido) : null;
                config.TipoMovimentoValorSaldo = codigoTipoMovimentoValorSaldo > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorSaldo) : null;
                config.TipoMovimentoValorSENAT = codigoTipoMovimentoValorSENAT > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorSENAT) : null;
                config.TipoMovimentoValorSEST = codigoTipoMovimentoValorSEST > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorSEST) : null;
                config.TipoMovimentoValorTarifaSaque = codigoTipoMovimentoValorTarifaSaque > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorTarifaSaque) : null;
                config.TipoMovimentoValorTarifaTransferencia = codigoTipoMovimentoValorTarifaTransferencia > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorTarifaTransferencia) : null;
                config.TipoMovimentoValorTotal = codigoTipoMovimentoValorTotal > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorTotal) : null;

                if (config.Codigo > 0)
                    repConfiguracaoTipoOperacao.Atualizar(config);
                else
                    repConfiguracaoTipoOperacao.Inserir(config);
            }
        }

        private void SalvarGeracaoMovimentoFinanceiroPorModeloDocumento(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFatura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            if (!configuracaoFatura.GeracaoMovimentoFinanceiroPorModeloDocumento)
            {
                repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento.DeletarTodos();
                return;
            }

            dynamic dynConfiguracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GeracaoMovimentosFinanceirosPorModeloDocumento"));

            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento> configuracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumento = repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento.BuscarTodos();

            if (configuracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumento.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento in dynConfiguracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumento)
                {
                    int codigo = ((string)configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento> deletar = (from obj in configuracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumento where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < deletar.Count; i++)
                    repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento.Deletar(deletar[i]);
            }

            foreach (dynamic configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento in dynConfiguracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumento)
            {
                int codigo = ((string)configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento geracaoMovimentoFinanceiroModeloDocumento = codigo > 0 ? repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento.BuscarPorCodigo(codigo, false) : null;

                if (geracaoMovimentoFinanceiroModeloDocumento == null)
                {
                    geracaoMovimentoFinanceiroModeloDocumento = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimento();

                    int codigoTipoMovimentoModeloDocumento = ((string)configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento.TipoMovimentoModeloDocumento.Codigo).ToInt();
                    int codigoTipoModeloDocumentoFiscal = ((string)configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento.TipoModeloDocumentoFiscal.Codigo).ToInt();

                    geracaoMovimentoFinanceiroModeloDocumento.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoModeloDocumento);
                    geracaoMovimentoFinanceiroModeloDocumento.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(codigoTipoModeloDocumentoFiscal);

                    repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumento.Inserir(geracaoMovimentoFinanceiroModeloDocumento);
                }
            }
        }

        private void SalvarGeracaoMovimentoFinanceiroPorModeloDocumentoReversao(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFatura configuracaoFatura, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            if (!configuracaoFatura.GeracaoMovimentoFinanceiroPorModeloDocumentoReversao) 
            {
                repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao.DeletarTodos();
                return;
            }

            dynamic dynConfiguracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumentoReversao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GeracaoMovimentosFinanceirosPorModeloDocumentoReversao"));

            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao> configuracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumentoReversao = repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao.BuscarTodos();

            if (configuracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumentoReversao.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao in dynConfiguracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumentoReversao)
                {
                    int codigo = ((string)configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao> deletar = (from obj in configuracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumentoReversao where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < deletar.Count; i++)
                    repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao.Deletar(deletar[i]);
            }

            foreach (dynamic configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao in dynConfiguracaoFinanceirasGeracaoMovimentosFinanceirosModeloDocumentoReversao)
            {
                int codigo = ((string)configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao geracaoMovimentoFinanceiroModeloDocumentoReversao = codigo > 0 ? repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao.BuscarPorCodigo(codigo, false) : null;

                if (geracaoMovimentoFinanceiroModeloDocumentoReversao == null)
                {
                    geracaoMovimentoFinanceiroModeloDocumentoReversao = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraFaturaModeloTipoMovimentoReversao();

                    int codigoTipoMovimentoModeloDocumento = ((string)configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao.TipoMovimentoModeloDocumentoReversao.Codigo).ToInt();
                    int codigoTipoModeloDocumentoFiscal = ((string)configuracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao.TipoModeloDocumentoFiscalReversao.Codigo).ToInt();

                    geracaoMovimentoFinanceiroModeloDocumentoReversao.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoModeloDocumento);
                    geracaoMovimentoFinanceiroModeloDocumentoReversao.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(codigoTipoModeloDocumentoFiscal);

                    repConfiguracaoFinanceiraGeracaoMovimentoFinanceiroModeloDocumentoReversao.Inserir(geracaoMovimentoFinanceiroModeloDocumentoReversao);
                }
            }
        }

        #endregion
    }
}
