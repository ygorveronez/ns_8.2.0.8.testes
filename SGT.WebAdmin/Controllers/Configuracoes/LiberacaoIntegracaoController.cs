using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/LiberacaoIntegracao")]
    public class LiberacaoIntegracaoController : BaseController
    {
        #region Construtores

        public LiberacaoIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDados()
        {
            using Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            var tipoIntegracaoCategorias = new List<Dominio.ObjetosDeValor.TipoIntegracaoCategoria>();
            var naoAparecerTela = new List<string> { "Não Informada", "Não Possui Integração" };

            try
            {
                var integracoesAtivas = repTipoIntegracao.BuscarAtivos();

                foreach (TipoIntegracao integracao in Enum.GetValues(typeof(TipoIntegracao)))
                {
                    if (naoAparecerTela.Contains(integracao.ObterDescricao()))
                        continue;

                    var categoria = integracao.ObterCategoria();
                    var tipoIntegracaoCategoria =  tipoIntegracaoCategorias.Where(o => o.Descricao == categoria.ObterDescricao()).FirstOrDefault();

                    var tipoIntegracao = new Dominio.ObjetosDeValor.TipoIntegracaoCategoriaIntegracao
                    {
                        Codigo = (int)integracao,
                        Descricao = string.IsNullOrEmpty(integracao.ObterDescricao()) ? Enum.GetName(integracao) : integracao.ObterDescricao(),
                        Ativo = integracoesAtivas.Any(o => o.Tipo == integracao),
                        Parametros = ObterParametrosIntegracao(integracao, integracoesAtivas)
                    };

                    if (tipoIntegracaoCategoria == null)
                    {
                        tipoIntegracaoCategorias.Add( new Dominio.ObjetosDeValor.TipoIntegracaoCategoria 
                        {
                            Codigo = (int)categoria,
                            Descricao = categoria.ObterDescricao(),
                            Icone = categoria.ObterIcone(),
                            Integracoes = new List<Dominio.ObjetosDeValor.TipoIntegracaoCategoriaIntegracao> { tipoIntegracao }
                        } );
                    }
                    else
                    {
                        tipoIntegracaoCategoria.Integracoes.Add(tipoIntegracao);
                    }

                }

                return new JsonpResult(tipoIntegracaoCategorias.OrderBy(obj => obj.Codigo).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Configuracoes.LiberacaoIntegracao.FalhaBuscarIntegracao);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Atualizar(List<Dominio.ObjetosDeValor.TipoIntegracaoCategoria> categorias)
        {
            using Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeDeTrabalho);
            unidadeDeTrabalho.Start();

            try
            {
                var integracoesAtivas = repTipoIntegracao.BuscarAtivos();

                foreach (var categoria in categorias)
                {
                    foreach (var integracao in categoria.Integracoes)
                    {
                        var integracaoExistente = integracoesAtivas.FirstOrDefault(i => i.Tipo == (TipoIntegracao)integracao.Codigo);

                        if (integracao.Ativo)
                        {
                            var formatoIntegracao = TipoIntegracaoFormatosInsercao.ObterFormato((TipoIntegracao)integracao.Codigo);

                            //preenchimento das variaveis 
                            var tipoEnvio = formatoIntegracao.Campos.TryGetValue("TipoEnvio", out var tipoEnvioFormato)
                                     ? Convert.ToInt32(tipoEnvioFormato)
                                     : 0;

                            var quantidadeMaximaEnvioLote = formatoIntegracao.Campos.TryGetValue("QuantidadeMaximaEnvioLote", out var quantidadeFormato)
                                    ? Convert.ToInt32(quantidadeFormato)
                                    : 0;

                            bool integrarCargaTransbordo =
                                ConverterParametroComoBool(integracao, "IntegrarCargaTransbordo")
                                ?? ConverterFormatoComoBool(formatoIntegracao, "IntegrarCargaTransbordo")
                                ?? false;

                            bool gerarIntegracaoNasOcorrencias =
                                ConverterParametroComoBool(integracao, "GerarIntegracaoNasOcorrencias")
                                ?? ConverterFormatoComoBool(formatoIntegracao, "GerarIntegracaoNasOcorrencias")
                                ?? false;

                            bool permitirReenvioExcecao =
                                ConverterParametroComoBool(integracao, "PermitirReenvioExcecao")
                                ?? ConverterFormatoComoBool(formatoIntegracao, "PermitirReenvioExcecao")
                                ?? false;

                            var grupo = formatoIntegracao.Campos.TryGetValue("Grupo", out var grupoFormato)
                                    ? Convert.ToInt32(grupoFormato)
                                    : 0;

                            bool controlePorLote = ConverterParametroComoBool(integracao, "ControlePorLote")
                                ?? ConverterFormatoComoBool(formatoIntegracao,"ControlePorLote")
                                    ?? false;

                            bool gerarIntegracaoFechamentoCarga = ConverterParametroComoBool(integracao,"GerarIntegracaoFechamentoCarga")
                                ?? ConverterFormatoComoBool(formatoIntegracao, "GerarIntegracaoFechamentoCarga")
                                    ?? false;

                            bool integrarVeiculoTrocaMotorista = ConverterParametroComoBool(integracao,"IntegrarVeiculoTrocaMotorista") 
                                 ?? ConverterFormatoComoBool(formatoIntegracao, "IntegrarVeiculoTrocaMotorista")
                                    ?? false;

                            bool gerarIntegracaoDadosTransporteCarga =  ConverterParametroComoBool(integracao, "GerarIntegracaoDadosTransporteCarga")
                                            ?? ConverterFormatoComoBool(formatoIntegracao, "GerarIntegracaoDadosTransporteCarga")
                                            ?? false;

                            bool integracaoTransportador = ConverterParametroComoBool(integracao, "IntegracaoTransportador")
                                ?? ConverterFormatoComoBool(formatoIntegracao, "IntegracaoTransportador")
                                ?? false;

                            bool integrarPlataforma = ConverterParametroComoBool(integracao, "IntegrarComPlataformaNstech")
                                ?? ConverterFormatoComoBool(formatoIntegracao, "IntegrarComPlataformaNstech")
                                ?? false;

                            bool integrarPedidos = ConverterParametroComoBool(integracao, "IntegrarPedidos")
                                ?? ConverterFormatoComoBool(formatoIntegracao, "IntegrarPedidos")
                                ?? false;

                            TipoIntegracao tipoIntegracao = (TipoIntegracao)integracao.Codigo;
                            repTipoIntegracao.Ativar(new Dominio.Entidades.Embarcador.Cargas.TipoIntegracao
                            {
                                Tipo = tipoIntegracao,
                                Descricao = integracao.Descricao,
                                TipoEnvio = (TipoEnvioIntegracao)tipoEnvio,
                                QuantidadeMaximaEnvioLote = (int)quantidadeMaximaEnvioLote,
                                IntegrarCargaTransbordo = (bool)integrarCargaTransbordo,
                                GerarIntegracaoNasOcorrencias = (bool)gerarIntegracaoNasOcorrencias,
                                PermitirReenvioExcecao = (bool)permitirReenvioExcecao,
                                Grupo = (GrupoTipoIntegracao)grupo,
                                ControlePorLote = (bool)controlePorLote,
                                GerarIntegracaoFechamentoCarga = (bool)gerarIntegracaoFechamentoCarga,
                                IntegrarVeiculoTrocaMotorista = (bool)integrarVeiculoTrocaMotorista,
                                GerarIntegracaoDadosTransporteCarga = (bool)gerarIntegracaoDadosTransporteCarga,
                                IntegracaoTransportador = (bool)integracaoTransportador,
                                IntegrarComPlataformaNstech = (bool)integrarPlataforma,
                                IntegrarPedidos = (bool) integrarPedidos,
                            });
                            
                            Servicos.Embarcador.Configuracoes.LiberacaoIntegracao.ProcessarScriptsEspecificosPorIntegracao(tipoIntegracao, unidadeDeTrabalho);

                            var liberacaoIntegracao = new LiberacaoIntegracao { Codigo = 0, Descricao = integracao.Descricao };
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, liberacaoIntegracao, $"Ativou a integração com {integracao.Descricao}.", unidadeDeTrabalho);
                        }
                        else if (!integracao.Ativo && integracaoExistente != null)
                        {
                            repTipoIntegracao.Desativar(integracaoExistente);
                            var liberacaoIntegracao = new LiberacaoIntegracao { Codigo = integracaoExistente.Codigo, Descricao = integracaoExistente.Descricao };
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, liberacaoIntegracao, $"Desativou a integração com {integracaoExistente.Descricao}.", unidadeDeTrabalho);
                        }
                    }
                }

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true, Localization.Resources.Configuracoes.LiberacaoIntegracao.SucessoIntegracao);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Configuracoes.LiberacaoIntegracao.FalhaIntegracao);
            }
        }

        #endregion Métodos Globais
        private Dictionary<string, TipoIntegracaoCategoriaParametro> ObterParametrosIntegracao(TipoIntegracao tipo, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> integracoesAtivas)
        {
            var parametros = new Dictionary<string, TipoIntegracaoCategoriaParametro>();
            var integracaoAtiva = integracoesAtivas.FirstOrDefault(o => o.Tipo == tipo);

            switch (tipo)
            {
                //GR
                case TipoIntegracao.AngelLira:
                    AdicionarParametro(parametros, "GerarIntegracaoDadosTransporteCarga", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoDadosTransporteCarga, integracaoAtiva?.GerarIntegracaoDadosTransporteCarga ?? false);
                    AdicionarParametro(parametros, "IntegrarPedidos", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegrarPedidos, integracaoAtiva?.IntegrarPedidos ?? false);
                    AdicionarParametro(parametros, "IntegrarCargaTransbordo", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegrarCargaTransbordo, integracaoAtiva?.IntegrarCargaTransbordo ?? false);
                    break;
                case TipoIntegracao.OpenTech:
                    AdicionarParametro(parametros, "GerarIntegracaoDadosTransporteCarga", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoDadosTransporteCarga, integracaoAtiva?.GerarIntegracaoDadosTransporteCarga ?? false);
                    AdicionarParametro(parametros, "IntegracaoTransportador", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegracaoTransportador, integracaoAtiva?.IntegracaoTransportador ?? false);
                    AdicionarParametro(parametros, "IntegrarCargaTransbordo", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegrarCargaTransbordo, integracaoAtiva?.IntegrarCargaTransbordo ?? false);
                    break;
                case TipoIntegracao.NOX:
                    AdicionarParametro(parametros, "GerarIntegracaoDadosTransporteCarga", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoDadosTransporteCarga, integracaoAtiva?.GerarIntegracaoDadosTransporteCarga ?? false);
                    break;
                case TipoIntegracao.BrasilRisk:
                    AdicionarParametro(parametros, "GerarIntegracaoDadosTransporteCarga", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoDadosTransporteCarga, integracaoAtiva?.GerarIntegracaoDadosTransporteCarga ?? false);
                    AdicionarParametro(parametros, "IntegrarComPlataformaNstech", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegrarComPlataformaNstech, integracaoAtiva?.IntegrarComPlataformaNstech ?? false);
                    break;
                case TipoIntegracao.Buonny:
                    AdicionarParametro(parametros, "IntegrarComPlataformaNstech", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegrarComPlataformaNstech, integracaoAtiva?.IntegrarComPlataformaNstech ?? false);
                    break;
                case TipoIntegracao.LogRisk:
                    AdicionarParametro(parametros, "IntegrarComPlataformaNstech", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegrarComPlataformaNstech, integracaoAtiva?.IntegrarComPlataformaNstech ?? false);
                    break;
                case TipoIntegracao.Raster:
                    AdicionarParametro(parametros, "GerarIntegracaoDadosTransporteCarga", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoDadosTransporteCarga, integracaoAtiva?.GerarIntegracaoDadosTransporteCarga ?? false);
                    break;
                case TipoIntegracao.VSTrack:
                    AdicionarParametro(parametros, "GerarIntegracaoDadosTransporteCarga", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoDadosTransporteCarga, integracaoAtiva?.GerarIntegracaoDadosTransporteCarga ?? false);
                    break;
                //Outras
                case TipoIntegracao.KMM:
                    AdicionarParametro(parametros, "IntegrarCargaTransbordo", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegrarCargaTransbordo, integracaoAtiva?.IntegrarCargaTransbordo ?? false);
                    AdicionarParametro(parametros, "GerarIntegracaoNasOcorrencias", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoNasOcorrencias, integracaoAtiva?.GerarIntegracaoNasOcorrencias ?? false);
                    break;
                case TipoIntegracao.Natura:
                    AdicionarParametro(parametros, "PermitirReenvioExcecao", Localization.Resources.Configuracoes.LiberacaoIntegracao.PermitirReenvioExcecao, integracaoAtiva?.PermitirReenvioExcecao ?? false);
                    AdicionarParametro(parametros, "GerarIntegracaoNasOcorrencias", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoNasOcorrencias, integracaoAtiva?.GerarIntegracaoNasOcorrencias ?? false);
                    break;
                case TipoIntegracao.MercadoLivre:
                    AdicionarParametro(parametros, "GerarIntegracaoNasOcorrencias", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoNasOcorrencias, integracaoAtiva?.GerarIntegracaoNasOcorrencias ?? false);
                    AdicionarParametro(parametros, "PermitirReenvioExcecao", Localization.Resources.Configuracoes.LiberacaoIntegracao.PermitirReenvioExcecao, integracaoAtiva?.PermitirReenvioExcecao ?? false);
                    break;
                case TipoIntegracao.Trizy:
                    AdicionarParametro(parametros, "GerarIntegracaoFechamentoCarga", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoFechamentoCarga, integracaoAtiva?.GerarIntegracaoFechamentoCarga ?? false);
                    AdicionarParametro(parametros, "IntegrarCargaTransbordo", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegrarCargaTransbordo, integracaoAtiva?.IntegrarCargaTransbordo ?? false);
                    AdicionarParametro(parametros, "GerarIntegracaoDadosTransporteCarga", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoDadosTransporteCarga, integracaoAtiva?.GerarIntegracaoDadosTransporteCarga ?? false);
                    break;
                case TipoIntegracao.A52:
                    AdicionarParametro(parametros, "GerarIntegracaoDadosTransporteCarga", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoDadosTransporteCarga, integracaoAtiva?.GerarIntegracaoDadosTransporteCarga ?? false);
                    AdicionarParametro(parametros, "IntegrarCargaTransbordo", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegrarCargaTransbordo, integracaoAtiva?.IntegrarCargaTransbordo ?? false);
                    AdicionarParametro(parametros, "IntegrarVeiculoTrocaMotorista", Localization.Resources.Configuracoes.LiberacaoIntegracao.IntegrarVeiculoTrocaMotorista, integracaoAtiva?.IntegrarVeiculoTrocaMotorista ?? false);
                    break;
                case TipoIntegracao.ATSSmartWeb:
                    AdicionarParametro(parametros, "GerarIntegracaoDadosTransporteCarga", Localization.Resources.Configuracoes.LiberacaoIntegracao.GerarIntegracaoDadosTransporteCarga, integracaoAtiva?.GerarIntegracaoDadosTransporteCarga ?? false);
                    break;
            }

            return parametros;
        }

        private void AdicionarParametro(Dictionary<string, TipoIntegracaoCategoriaParametro> parametros, string nomeColuna, string descricao, bool valor)
        {
            parametros.Add(nomeColuna, new TipoIntegracaoCategoriaParametro
            {
                NomeColuna = nomeColuna,
                Descricao = descricao,
                Valor = valor.ToString()
            });
        }

        public bool? ConverterParametroComoBool(TipoIntegracaoCategoriaIntegracao integracao, string chave)
        {
            if (integracao?.Parametros != null && integracao.Parametros.TryGetValue(chave, out var parametro))
            {
                bool.TryParse(parametro.Valor, out var valorBool);
                return valorBool;
            }
            return null;
        }

        public bool? ConverterFormatoComoBool(TipoIntegracaoFormatosInsercao formato, string chave)
        {
            if (formato?.Campos != null && formato.Campos.TryGetValue(chave, out var dadosFormato))
            {
                return Convert.ToBoolean(dadosFormato);
            }
            return null;
        }

        public class LiberacaoIntegracao
        {
            public long Codigo { get; set; }
            public string Descricao { get; set; }
        }

    }
}
