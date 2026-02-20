using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Relatorios
{
    public partial class Relatorio : ServicoBase
    {
        #region Atributos Privados

        private static AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo _configuracaoArquivo;
        private string _stringConexao;
        private CancellationToken _cancellationToken { get; set; }

        #endregion

        #region Construtores
        
        public Relatorio(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        public Relatorio(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Relatorio() : base() { }
        public Relatorio(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base()
        {
            _clienteMultisoftware = clienteMultisoftware;
        }

        #endregion

        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao AdicionarRelatorioParaGeracao(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, Dominio.Entidades.Usuario usuario, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio, Repositorio.UnitOfWork unitOfWork, int codigoEntidade = 0)
        {
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao
            {
                Relatorio = relatorio,
                DataInicioGeracao = DateTime.Now,
                DataFinalGeracao = DateTime.Now, //está data é atualizada ao termino da geração
                GuidArquivo = Guid.NewGuid().ToString().Replace("-", ""),
                Titulo = relatorio?.Titulo ?? "",
                Usuario = usuario != null ? usuario : repUsuario.BuscarPrimeiro(),
                TipoArquivoRelatorio = tipoArquivoRelatorio,
                SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao,
                CodigoEntidade = codigoEntidade
            };
#if DEBUG
            relatorioControleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucaoLocal;
#endif
            repRelatorioControleGeracao.Inserir(relatorioControleGeracao);

            return relatorioControleGeracao;
        }

        public async Task<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> AdicionarRelatorioParaGeracaoAsync(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, Dominio.Entidades.Usuario usuario, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio, Repositorio.UnitOfWork unitOfWork, int codigoEntidade = 0)
        {
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork, _cancellationToken);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork, _cancellationToken);

            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao
            {
                Relatorio = relatorio,
                DataInicioGeracao = DateTime.Now,
                DataFinalGeracao = DateTime.Now, //está data é atualizada ao termino da geração
                GuidArquivo = Guid.NewGuid().ToString().Replace("-", ""),
                Titulo = relatorio?.Titulo ?? "",
                Usuario = usuario != null ? usuario : await repUsuario.BuscarPrimeiroAsync(),
                TipoArquivoRelatorio = tipoArquivoRelatorio,
                SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao,
                CodigoEntidade = codigoEntidade
            };
#if DEBUG
            relatorioControleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucaoLocal;
#endif
            await repRelatorioControleGeracao.InserirAsync(relatorioControleGeracao);
            unitOfWork.CommitChanges();
            return relatorioControleGeracao;
        }

        public Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao AdicionarRelatorioParaGeracao(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio,
                                                                                                              Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio relatorioTemporario,
                                                                                                              Dominio.Entidades.Usuario usuario,
                                                                                                              Dominio.Entidades.Empresa empresa,
                                                                                                              object filtrosPesquisa,
                                                                                                              List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
                                                                                                              Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta,
                                                                                                              Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio,
                                                                                                              int? codigoEntidade,
                                                                                                              Repositorio.UnitOfWork unitOfWork,
                                                                                                              AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);

            if (repRelatorioControleGeracao.ContarRelatoriosEmExecucao(usuario.Codigo) > 0)
                throw new Dominio.Excecoes.Embarcador.ServicoException(Localization.Resources.Relatorios.Relatorio.JaExisteRelatorioEmGeracao);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = ObterConfiguracaoRelatorio(unitOfWork);

            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao
            {
                Relatorio = relatorio,
                DataInicioGeracao = DateTime.Now,
                DataFinalGeracao = DateTime.Now,
                GuidArquivo = Guid.NewGuid().ToString().Replace("-", ""),
                Titulo = relatorio?.Titulo ?? "",
                Usuario = usuario,
                TipoArquivoRelatorio = tipoArquivoRelatorio,
                SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao,
                CodigoEntidade = codigoEntidade ?? 0,
                Empresa = empresa,
                GerarPorServico = true /*configuracaoRelatorio?.ServicoGeracaoRelatorioHabilitado ?? false*/, //Sempre será gerado por serviço 
                DadosConsulta = ObterDadosConsulta(configuracaoRelatorio, relatorioTemporario, filtrosPesquisa, propriedades, parametrosConsulta, unitOfWork),
                TipoServicoMultisoftware = tipoServicoMultisoftware
            };

#if DEBUG
            relatorioControleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucaoLocal;
#endif

            repRelatorioControleGeracao.Inserir(relatorioControleGeracao);

            unitOfWork.CommitChanges();

            return relatorioControleGeracao;
        }

        public async Task<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> AdicionarRelatorioParaGeracaoAsync(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio,
                                                                                                              Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio relatorioTemporario,
                                                                                                              Dominio.Entidades.Usuario usuario,
                                                                                                              Dominio.Entidades.Empresa empresa,
                                                                                                              object filtrosPesquisa,
                                                                                                              List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
                                                                                                              Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta,
                                                                                                              Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio,
                                                                                                              int? codigoEntidade,
                                                                                                              Repositorio.UnitOfWork unitOfWork,
                                                                                                              AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork, _cancellationToken);

            if (await repRelatorioControleGeracao.ContarRelatoriosEmExecucaoAsync(usuario.Codigo) > 0)
                throw new Dominio.Excecoes.Embarcador.ServicoException(Localization.Resources.Relatorios.Relatorio.JaExisteRelatorioEmGeracao);

            await unitOfWork.StartAsync(_cancellationToken);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = await ObterConfiguracaoRelatorioAsync(unitOfWork);

            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao
            {
                Relatorio = relatorio,
                DataInicioGeracao = DateTime.Now,
                DataFinalGeracao = DateTime.Now,
                GuidArquivo = Guid.NewGuid().ToString().Replace("-", ""),
                Titulo = relatorio?.Titulo ?? "",
                Usuario = usuario,
                TipoArquivoRelatorio = tipoArquivoRelatorio,
                SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao,
                CodigoEntidade = codigoEntidade ?? 0,
                Empresa = empresa,
                GerarPorServico = true /*configuracaoRelatorio?.ServicoGeracaoRelatorioHabilitado ?? false*/, //Sempre será gerado por serviço 
                DadosConsulta = ObterDadosConsulta(configuracaoRelatorio, relatorioTemporario, filtrosPesquisa, propriedades, parametrosConsulta, unitOfWork), //TODO: ajustar para async
                TipoServicoMultisoftware = tipoServicoMultisoftware
            };

#if DEBUG
            relatorioControleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucaoLocal;
#endif

            await repRelatorioControleGeracao.InserirAsync(relatorioControleGeracao);

            await unitOfWork.CommitChangesAsync(_cancellationToken);

            return relatorioControleGeracao;
        }


        public void AlterarDadosRelatorio(Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico)
        {
            relatorio.CortarLinhas = dynRelatorio.CortarLinhas;
            relatorio.Descricao = dynRelatorio.Descricao;
            relatorio.ExibirSumarios = dynRelatorio.ExibirSumarios;
            relatorio.FontePadrao = dynRelatorio.FontePadrao;
            relatorio.PropriedadeAgrupa = dynRelatorio.PropriedadeAgrupa;
            relatorio.OrdemAgrupamento = dynRelatorio.OrdemAgrupamento ?? string.Empty;
            relatorio.PropriedadeOrdena = dynRelatorio.PropriedadeOrdena;
            relatorio.OrdemOrdenacao = dynRelatorio.OrdemOrdenacao;
            relatorio.OrientacaoRelatorio = dynRelatorio.OrientacaoRelatorio;
            relatorio.TamanhoPadraoFonte = dynRelatorio.TamanhoPadraoFonte;
            relatorio.Titulo = dynRelatorio.Titulo;
            relatorio.FundoListrado = dynRelatorio.FundoListrado;
            relatorio.TipoServicoMultisoftware = tipoServico;
            relatorio.OcultarDetalhe = dynRelatorio.OcultarDetalhe;
            relatorio.NovaPaginaAposAgrupamento = dynRelatorio.NovaPaginaAposAgrupamento;
            relatorio.RelatorioParaTodosUsuarios = dynRelatorio.RelatorioParaTodosUsuarios;
            relatorio.Padrao = dynRelatorio.Padrao;
        }

        public Dominio.Entidades.Embarcador.Relatorios.Relatorio BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, string Titulo, string pasta, string arquivo, Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio orientacao, string propOrdenacao, string ordemOrdenacao, string propAgrupa, string ordemAgrupa, int codigo, Repositorio.UnitOfWork unitOfWork, bool exibirSumarios, bool cortarLinhas, int tamanhoFonte = 10, string fontePadrao = "Arial", bool fundoListrado = false, int timeOut = 30, bool ocultarDetalhes = false, bool liberadoParaTodosUsuarios = true, bool salvarFiltrosConsulta = false, bool novaPaginaAposAgrupamento = false)
        {

            Repositorio.Embarcador.Relatorios.RelatorioColuna repRelatorioColuna = new Repositorio.Embarcador.Relatorios.RelatorioColuna(unitOfWork);
            Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio;
            if (codigo > 0)
                relatorio = repRelatorio.BuscarPorCodigo(codigo);
            else
                relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(codigoControleRelatorio, tipoServico);

            if (relatorio == null)
            {
                relatorio = new Dominio.Entidades.Embarcador.Relatorios.Relatorio();
                relatorio.CodigoControleRelatorios = codigoControleRelatorio;
                relatorio.Titulo = Titulo;
                relatorio.Padrao = true;
                relatorio.Ativo = true;
                relatorio.Descricao = Localization.Resources.Relatorios.Relatorio.RelatorioPadrao;
                relatorio.ArquivoRelatorio = arquivo;
                relatorio.CaminhoRelatorio = @"Areas\Relatorios\Reports\Default\" + pasta;
                relatorio.OrientacaoRelatorio = orientacao;
                relatorio.PropriedadeAgrupa = propAgrupa;
                relatorio.PropriedadeOrdena = propOrdenacao;
                relatorio.OrdemAgrupamento = ordemAgrupa;
                relatorio.OrdemOrdenacao = ordemOrdenacao;
                relatorio.PadraoMultisoftware = true;
                relatorio.TimeOutMinutos = timeOut;
                relatorio.FontePadrao = fontePadrao;
                relatorio.TamanhoPadraoFonte = tamanhoFonte;
                relatorio.ExibirSumarios = exibirSumarios;
                relatorio.CortarLinhas = cortarLinhas;
                relatorio.FundoListrado = fundoListrado;
                relatorio.TipoServicoMultisoftware = tipoServico;
                relatorio.OcultarDetalhe = ocultarDetalhes;
                relatorio.NovaPaginaAposAgrupamento = novaPaginaAposAgrupamento;
                relatorio.RelatorioParaTodosUsuarios = liberadoParaTodosUsuarios;
                repRelatorio.Inserir(relatorio);
            }

            relatorio.Colunas = repRelatorioColuna.BuscarPorRelatorio(relatorio.Codigo);

            if (relatorio.CodigoControleRelatorios == codigoControleRelatorio)
            {
                return relatorio;
            }
            else
            {

                throw new Exception(String.Format(Localization.Resources.Relatorios.Relatorio.CodigoPadraoDiferenteDoPesquisado, (codigoControleRelatorio, relatorio.CodigoControleRelatorios)));
            }
        }

        public async Task<Dominio.Entidades.Embarcador.Relatorios.Relatorio> BuscarConfiguracaoPadraoAsync(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, string Titulo, string pasta, string arquivo, Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio orientacao, string propOrdenacao, string ordemOrdenacao, string propAgrupa, string ordemAgrupa, int codigo, Repositorio.UnitOfWork unitOfWork, bool exibirSumarios, bool cortarLinhas, int tamanhoFonte = 10, string fontePadrao = "Arial", bool fundoListrado = false, int timeOut = 30, bool ocultarDetalhes = false, bool liberadoParaTodosUsuarios = true, bool salvarFiltrosConsulta = false, bool novaPaginaAposAgrupamento = false)
        {

            Repositorio.Embarcador.Relatorios.RelatorioColuna repRelatorioColuna = new Repositorio.Embarcador.Relatorios.RelatorioColuna(unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, _cancellationToken);
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio;
            if (codigo > 0)
                relatorio = await repRelatorio.BuscarPorCodigoAsync(codigo, _cancellationToken);
            else
                relatorio = await repRelatorio.BuscarPadraoPorCodigoControleRelatorioAsync(codigoControleRelatorio, tipoServico, _cancellationToken);

            if (relatorio == null)
            {
                relatorio = new Dominio.Entidades.Embarcador.Relatorios.Relatorio();
                relatorio.CodigoControleRelatorios = codigoControleRelatorio;
                relatorio.Titulo = Titulo;
                relatorio.Padrao = true;
                relatorio.Ativo = true;
                relatorio.Descricao = Localization.Resources.Relatorios.Relatorio.RelatorioPadrao;
                relatorio.ArquivoRelatorio = arquivo;
                relatorio.CaminhoRelatorio = @"Areas\Relatorios\Reports\Default\" + pasta;
                relatorio.OrientacaoRelatorio = orientacao;
                relatorio.PropriedadeAgrupa = propAgrupa;
                relatorio.PropriedadeOrdena = propOrdenacao;
                relatorio.OrdemAgrupamento = ordemAgrupa;
                relatorio.OrdemOrdenacao = ordemOrdenacao;
                relatorio.PadraoMultisoftware = true;
                relatorio.TimeOutMinutos = timeOut;
                relatorio.FontePadrao = fontePadrao;
                relatorio.TamanhoPadraoFonte = tamanhoFonte;
                relatorio.ExibirSumarios = exibirSumarios;
                relatorio.CortarLinhas = cortarLinhas;
                relatorio.FundoListrado = fundoListrado;
                relatorio.TipoServicoMultisoftware = tipoServico;
                relatorio.OcultarDetalhe = ocultarDetalhes;
                relatorio.NovaPaginaAposAgrupamento = novaPaginaAposAgrupamento;
                relatorio.RelatorioParaTodosUsuarios = liberadoParaTodosUsuarios;
                await repRelatorio.InserirAsync(relatorio);
            }

            relatorio.Colunas = await repRelatorioColuna.BuscarPorRelatorioAsync(relatorio.Codigo, _cancellationToken);

            if (relatorio.CodigoControleRelatorios == codigoControleRelatorio)
            {
                return relatorio;
            }
            else
            {
                throw new Exception(String.Format(Localization.Resources.Relatorios.Relatorio.CodigoPadraoDiferenteDoPesquisado, (codigoControleRelatorio, relatorio.CodigoControleRelatorios)));
            }
        }



        public void ExcluirArquivoRelatorio(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorio, Repositorio.UnitOfWork unitOfWork)
        {
            if (relatorio == null)
            {
                Log.TratarErro("Relatorio vazio ao ser excluido!", "Relatorio");
                return;
            }

            string extencao = "";

            switch (relatorio.TipoArquivoRelatorio)
            {
                case Dominio.Enumeradores.TipoArquivoRelatorio.PDF:
                    extencao = ".pdf";
                    break;
                case Dominio.Enumeradores.TipoArquivoRelatorio.XLS:
                    extencao = ".xls";
                    break;
                case Dominio.Enumeradores.TipoArquivoRelatorio.CSV:
                    extencao = ".csv";
                    break;
            }

            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(ObterConfiguracaoArquivo(unitOfWork).CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), relatorio.GuidArquivo) + extencao;

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
            {
                try
                {
                    Log.TratarErro("Deletando arquivo " + caminhoArquivo, "Relatorio");
                    Utilidades.IO.FileStorageService.Storage.Delete(caminhoArquivo);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(Localization.Resources.Relatorios.Relatorio.FalhaAoExcluir + ex, "Relatorio");
                }
            }
        }

        public string ObterCaminhoArquivoRelatorio(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Repositorio.UnitOfWork unitOfWork)
        {
            string pastaRelatorios = ObterConfiguracaoArquivo(unitOfWork).CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();
            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, relatorioControleGeracao.GuidArquivo);

            switch (relatorioControleGeracao.TipoArquivoRelatorio)
            {
                case Dominio.Enumeradores.TipoArquivoRelatorio.XLS: return caminhoArquivo + ".xls";
                case Dominio.Enumeradores.TipoArquivoRelatorio.CSV: return caminhoArquivo + ".csv";
                default: return caminhoArquivo + ".pdf";
            }
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo ObterConfiguracaoArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            if (_configuracaoArquivo != null)
                return _configuracaoArquivo;

#if DEBUG
            var objValorconfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork).BuscarConfiguracaoDebugLocal();

            _configuracaoArquivo = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo()
            {
                CaminhoRelatorios = objValorconfiguracaoArquivo.CaminhoRelatorios,
                CaminhoTempArquivosImportacao = objValorconfiguracaoArquivo.CaminhoTempArquivosImportacao,
                CaminhoCanhotos = objValorconfiguracaoArquivo.CaminhoCanhotos,
                CaminhoCanhotosAvulsos = objValorconfiguracaoArquivo.CaminhoCanhotosAvulsos,
                CaminhoXMLNotaFiscalComprovanteEntrega = objValorconfiguracaoArquivo.CaminhoXMLNotaFiscalComprovanteEntrega,
                CaminhoArquivosIntegracao = objValorconfiguracaoArquivo.CaminhoArquivosIntegracao,
                CaminhoRelatoriosEmbarcador = objValorconfiguracaoArquivo.CaminhoRelatoriosEmbarcador,
                CaminhoLogoEmbarcador = objValorconfiguracaoArquivo.CaminhoLogoEmbarcador,
                CaminhoDocumentosFiscaisEmbarcador = objValorconfiguracaoArquivo.CaminhoDocumentosFiscaisEmbarcador,
                Anexos = objValorconfiguracaoArquivo.Anexos,
                CaminhoGeradorRelatorios = objValorconfiguracaoArquivo.CaminhoGeradorRelatorios,
                CaminhoArquivosEmpresas = objValorconfiguracaoArquivo.CaminhoArquivosEmpresas,
                CaminhoRelatoriosCrystal = objValorconfiguracaoArquivo.CaminhoRelatoriosCrystal,
                CaminhoRetornoXMLIntegrador = objValorconfiguracaoArquivo.CaminhoRetornoXMLIntegrador,
                CaminhoArquivos = objValorconfiguracaoArquivo.CaminhoArquivos,
                CaminhoArquivosIntegracaoEDI = objValorconfiguracaoArquivo.CaminhoArquivosIntegracaoEDI,
                CaminhoArquivosImportacaoBoleto = objValorconfiguracaoArquivo.CaminhoArquivosImportacaoBoleto,
                CaminhoOcorrencias = objValorconfiguracaoArquivo.CaminhoOcorrencias,
                CaminhoOcorrenciasMobiles = objValorconfiguracaoArquivo.CaminhoOcorrenciasMobiles,
            };

            if (_configuracaoArquivo != null)
                return _configuracaoArquivo;
#endif

            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
            _configuracaoArquivo = repConfiguracaoArquivo.BuscarPrimeiroRegistro();
            return _configuracaoArquivo;
        }

        public Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio ObterConfiguracaoRelatorio(Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioPadrao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna> colunas)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio
            {
                ArquivoRelatorio = relatorioPadrao.ArquivoRelatorio,
                Ativo = relatorioPadrao.Ativo,
                CaminhoRelatorio = relatorioPadrao.CaminhoRelatorio,
                CodigoControleRelatorios = relatorioPadrao.CodigoControleRelatorios,
                TimeOutMinutos = relatorioPadrao.TimeOutMinutos,
                CortarLinhas = dynRelatorio.CortarLinhas,
                Descricao = dynRelatorio.Descricao,
                ExibirSumarios = dynRelatorio.ExibirSumarios,
                FontePadrao = dynRelatorio.FontePadrao,
                PropriedadeAgrupa = dynRelatorio.PropriedadeAgrupa,
                OrdemAgrupamento = !string.IsNullOrWhiteSpace(dynRelatorio.OrdemAgrupamento) ? dynRelatorio.OrdemAgrupamento : "",
                PropriedadeOrdena = dynRelatorio.PropriedadeOrdena,
                OrdemOrdenacao = dynRelatorio.OrdemOrdenacao,
                FundoListrado = dynRelatorio.FundoListrado,
                OrientacaoRelatorio = dynRelatorio.OrientacaoRelatorio,
                TamanhoPadraoFonte = dynRelatorio.TamanhoPadraoFonte,
                Titulo = dynRelatorio.Titulo,
                Padrao = dynRelatorio.Padrao,
                TipoServicoMultisoftware = tipoServicoMultisoftware,
                OcultarDetalhe = dynRelatorio.OcultarDetalhe,
                RelatorioParaTodosUsuarios = dynRelatorio.RelatorioParaTodosUsuarios,
                Colunas = colunas,
                NovaPaginaAposAgrupamento = dynRelatorio.NovaPaginaAposAgrupamento
            };
        }

        public Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio ObterConfiguracaoRelatorio(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio()
            {
                ArquivoRelatorio = relatorio.ArquivoRelatorio,
                Ativo = relatorio.Ativo,
                CaminhoRelatorio = relatorio.CaminhoRelatorio,
                CodigoControleRelatorios = relatorio.CodigoControleRelatorios,
                Colunas = relatorio.Colunas?.Select(o => new Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna()
                {
                    Alinhamento = o.Alinhamento,
                    CodigoDinamico = o.CodigoDinamico,
                    DataTypeExportacao = o.DataTypeExportacao,
                    PermiteAgrupamento = o.PermiteAgrupamento,
                    Posicao = o.Posicao,
                    PrecisaoDecimal = o.PrecisaoDecimal,
                    Propriedade = o.Propriedade,
                    Tamanho = o.Tamanho,
                    TipoSumarizacao = o.TipoSumarizacao,
                    Titulo = o.Titulo,
                    UtilizarFormatoTexto = o.UtilizarFormatoTexto,
                    Visivel = o.Visivel
                }).ToList(),
                CortarLinhas = relatorio.CortarLinhas,
                Descricao = relatorio.Descricao,
                ExibirSumarios = relatorio.ExibirSumarios,
                FontePadrao = relatorio.FontePadrao,
                FundoListrado = relatorio.FundoListrado,
                OcultarDetalhe = relatorio.OcultarDetalhe,
                OrdemAgrupamento = relatorio.OrdemAgrupamento,
                OrdemOrdenacao = relatorio.OrdemOrdenacao,
                OrientacaoRelatorio = relatorio.OrientacaoRelatorio,
                Padrao = relatorio.Padrao,
                PadraoMultisoftware = relatorio.PadraoMultisoftware,
                PropriedadeAgrupa = relatorio.PropriedadeAgrupa,
                PropriedadeOrdena = relatorio.PropriedadeOrdena,
                RelatorioParaTodosUsuarios = relatorio.RelatorioParaTodosUsuarios,
                TamanhoPadraoFonte = relatorio.TamanhoPadraoFonte,
                TimeOutMinutos = relatorio.TimeOutMinutos,
                TipoServicoMultisoftware = relatorio.TipoServicoMultisoftware,
                Titulo = relatorio.Titulo,
                NovaPaginaAposAgrupamento = relatorio.NovaPaginaAposAgrupamento
            };
        }

        public Dominio.Entidades.Embarcador.Relatorios.Relatorio ObterRelatorioTemporario(Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioPadrao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico)
        {
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = new Dominio.Entidades.Embarcador.Relatorios.Relatorio();

            relatorio.ArquivoRelatorio = relatorioPadrao.ArquivoRelatorio;
            relatorio.Ativo = relatorioPadrao.Ativo;
            relatorio.CaminhoRelatorio = relatorioPadrao.CaminhoRelatorio;
            relatorio.CodigoControleRelatorios = relatorioPadrao.CodigoControleRelatorios;
            relatorio.TimeOutMinutos = relatorioPadrao.TimeOutMinutos;
            relatorio.CortarLinhas = dynRelatorio.CortarLinhas;
            relatorio.Descricao = dynRelatorio.Descricao;
            relatorio.ExibirSumarios = dynRelatorio.ExibirSumarios;
            relatorio.FontePadrao = dynRelatorio.FontePadrao;
            relatorio.PropriedadeAgrupa = dynRelatorio.PropriedadeAgrupa;
            relatorio.OrdemAgrupamento = !string.IsNullOrWhiteSpace(dynRelatorio.OrdemAgrupamento) ? dynRelatorio.OrdemAgrupamento : "";
            relatorio.PropriedadeOrdena = dynRelatorio.PropriedadeOrdena;
            relatorio.OrdemOrdenacao = dynRelatorio.OrdemOrdenacao;
            relatorio.FundoListrado = dynRelatorio.FundoListrado;
            relatorio.OrientacaoRelatorio = dynRelatorio.OrientacaoRelatorio;
            relatorio.TamanhoPadraoFonte = dynRelatorio.TamanhoPadraoFonte;
            relatorio.Titulo = dynRelatorio.Titulo;
            relatorio.Padrao = dynRelatorio.Padrao;
            relatorio.TipoServicoMultisoftware = tipoServico;
            relatorio.OcultarDetalhe = dynRelatorio.OcultarDetalhe;
            relatorio.NovaPaginaAposAgrupamento = dynRelatorio.NovaPaginaAposAgrupamento;
            relatorio.RelatorioParaTodosUsuarios = dynRelatorio.RelatorioParaTodosUsuarios;

            return relatorio;
        }

        public void RegistrarFalhaGeracaoRelatorio(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Repositorio.UnitOfWork unitOfWork, string ex)
        {
            RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, null, ex);
        }

        public void RegistrarFalhaGeracaoRelatorio(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Repositorio.UnitOfWork unitOfWork, Exception ex)
        {
            RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex, "");
        }

        public async Task RegistrarFalhaGeracaoRelatorioAsync(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Repositorio.UnitOfWork unitOfWork, Exception ex, CancellationToken cancellationToken)
        {
            await RegistrarFalhaGeracaoRelatorioAsync(relatorioControleGeracao, unitOfWork, ex, "", cancellationToken);
        }

        #endregion

        #region Metodos Privado

        private Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracaoDadosConsulta ObterDadosConsulta(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio,
                                                                                                                 Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio relatorioTemporario,
                                                                                                                 object filtrosPesquisa,
                                                                                                                 List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
                                                                                                                 Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta,
                                                                                                                 Repositorio.UnitOfWork unitOfWork)
        {
            //if (!(configuracaoRelatorio?.ServicoGeracaoRelatorioHabilitado ?? false))
            //    return null;
            // Sempre será gerado por serviço

            Repositorio.Embarcador.Relatorios.RelatorioControleGeracaoDadosConsulta repRelatorioControleGeracaoDadosConsulta = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracaoDadosConsulta(unitOfWork);

            Newtonsoft.Json.JsonSerializerSettings jsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore };

            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracaoDadosConsulta dadosConsulta = new Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracaoDadosConsulta()
            {
                FiltrosPesquisa = Newtonsoft.Json.JsonConvert.SerializeObject(filtrosPesquisa, jsonSerializerSettings),
                ParametrosConsulta = Newtonsoft.Json.JsonConvert.SerializeObject(parametrosConsulta, jsonSerializerSettings),
                Propriedades = Newtonsoft.Json.JsonConvert.SerializeObject(propriedades, jsonSerializerSettings),
                RelatorioTemporario = Newtonsoft.Json.JsonConvert.SerializeObject(relatorioTemporario, jsonSerializerSettings)
            };

            repRelatorioControleGeracaoDadosConsulta.Inserir(dadosConsulta);

            return dadosConsulta;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio ObterConfiguracaoRelatorio(Repositorio.UnitOfWork unitOfWork)
        {
            return new Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio(unitOfWork).BuscarConfiguracaoPadrao();
        }

        private Task<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio> ObterConfiguracaoRelatorioAsync(Repositorio.UnitOfWork unitOfWork)
        {
            return new Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio(unitOfWork, _cancellationToken).BuscarConfiguracaoPadraoAsync();
        }

        private void RegistrarFalhaGeracaoRelatorio(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Repositorio.UnitOfWork unitOfWork, Exception ex, string msg)
        {
            try
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, relatorioControleGeracao.TipoServicoMultisoftware ?? 0, "");

                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);

                unitOfWork.Start();

                relatorioControleGeracao = repRelatorioControleGeracao.BuscarPorCodigo(relatorioControleGeracao.Codigo);
                relatorioControleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.FalhaAoGerar;
                relatorioControleGeracao.DataFinalGeracao = DateTime.Now;

                repRelatorioControleGeracao.Atualizar(relatorioControleGeracao);

                if (ex != null)
                    Servicos.Log.TratarErro("Gerar Relatorio GUID: " + relatorioControleGeracao.GuidArquivo + " Falha: " + ex, "Relatorio");
                else
                    Servicos.Log.TratarErro("Gerar Relatorio GUID: " + relatorioControleGeracao.GuidArquivo + " Falha: " + msg, "Relatorio");

                serNotificacao.GerarNotificacao(relatorioControleGeracao.Usuario, relatorioControleGeracao.Codigo, relatorioControleGeracao.GuidArquivo, string.IsNullOrWhiteSpace(msg) ? string.Format(Localization.Resources.Relatorios.Relatorio.OcorreuFalhaGerarRelatorio, relatorioControleGeracao.Titulo) : (relatorioControleGeracao.Titulo + " - " + msg), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.relatorio, relatorioControleGeracao.TipoServicoMultisoftware ?? 0, unitOfWork, _clienteMultisoftware?.Codigo ?? 0);

                unitOfWork.CommitChanges();
            }
            catch (Exception ex2)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("Não conseguiu registrar erro de geração Guid: Falha 1: " + ex + " Falha 2: " + ex2, "Relatorio");
            }
        }

        private async Task RegistrarFalhaGeracaoRelatorioAsync(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Repositorio.UnitOfWork unitOfWork, Exception ex, string msg, CancellationToken cancellationToken)
        {
            try
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, relatorioControleGeracao.TipoServicoMultisoftware ?? 0, "");

                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork, cancellationToken);

                unitOfWork.Start();

                relatorioControleGeracao = await repRelatorioControleGeracao.BuscarPorCodigoAsync(relatorioControleGeracao.Codigo);
                relatorioControleGeracao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.FalhaAoGerar;
                relatorioControleGeracao.DataFinalGeracao = DateTime.Now;

                await repRelatorioControleGeracao.AtualizarAsync(relatorioControleGeracao);

                if (ex != null)
                    Servicos.Log.TratarErro("Gerar Relatorio GUID: " + relatorioControleGeracao.GuidArquivo + " Falha: " + ex, "Relatorio");
                else
                    Servicos.Log.TratarErro("Gerar Relatorio GUID: " + relatorioControleGeracao.GuidArquivo + " Falha: " + msg, "Relatorio");

                serNotificacao.GerarNotificacao(relatorioControleGeracao.Usuario, relatorioControleGeracao.Codigo, relatorioControleGeracao.GuidArquivo, string.IsNullOrWhiteSpace(msg) ? string.Format(Localization.Resources.Relatorios.Relatorio.OcorreuFalhaGerarRelatorio, relatorioControleGeracao.Titulo) : (relatorioControleGeracao.Titulo + " - " + msg), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.relatorio, relatorioControleGeracao.TipoServicoMultisoftware ?? 0, unitOfWork, _clienteMultisoftware?.Codigo ?? 0);

                unitOfWork.CommitChanges();
            }
            catch (Exception ex2)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("Não conseguiu registrar erro de geração Guid: Falha 1: " + ex + " Falha 2: " + ex2, "Relatorio");
            }
        }

        #endregion

        /// <summary>
        /// Preferencialmente não utilizar esse método
        /// </summary>
        public void GerarRelatorioDinamico(string caminhoPagniaRelatorio, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao controleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp,
            object dataSource, Repositorio.UnitOfWork unitOfWork, Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT IdentificacaoCamposRPT = null,
            List<KeyValuePair<string, dynamic>> subReportDataSources = null, bool ajustarLinhasAutomaticamente = true,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServico = null, string caminhoLogo = null)
        {
            var request = ReportRequest.WithType(ReportType.Dinamic)
                .WithExecutionType(ExecutionType.Async)
                .AddExtraData("CaminhoPagniaRelatorio", caminhoPagniaRelatorio)
                .AddExtraData("Parametros", parametros.ToJson())
                .AddExtraData("ControleGeracao", controleGeracao.Codigo)
                .AddExtraData("RelatorioTemp", ObterConfiguracaoRelatorio(relatorioTemp).ToJson())
                .AddExtraData("DataSource", dataSource.ToJson())
                .AddExtraData("IdentificacaoCamposRPT", IdentificacaoCamposRPT.ToJson())
                .AddExtraData("SubReportDataSources", subReportDataSources.ToJson())
                .AddExtraData("AjustarLinhasAutomaticamente", ajustarLinhasAutomaticamente)
                .AddExtraData("CaminhoLogo", caminhoLogo)
                .AddExtraData("Type", dataSource.GetType().FullName);

            var result = request.CallReport(false);

            //Gambiara "Temporaria" (Se Deus Quiser) para reenviar os relatórios que falharam por terem muitos dados
            result ??= EnviarRequestPorArquivo(request.ExtraData.ToJson(), controleGeracao.GuidArquivo, relatorioTemp.Titulo, unitOfWork);

            if (!string.IsNullOrWhiteSpace(result?.ErrorMessage))
                throw new Dominio.Excecoes.Embarcador.ServicoException(result.ErrorMessage);
        }

        private ReportResult EnviarRequestPorArquivo(string jsonRequest, string guidArquivo, string titulo, Repositorio.UnitOfWork unitOfWork)
        {
            string basePastaRelatorios = ObterConfiguracaoArquivo(unitOfWork).CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();
            string pastaRelatorios = Utilidades.IO.FileStorageService.Storage.Combine(basePastaRelatorios, $"{titulo.Replace(" ", "")}-{DateTime.Today:dd-MM-yyyy}");

            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(pastaRelatorios, $"{guidArquivo}.txt");

            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivo, jsonRequest);

            return ReportRequest.WithType(ReportType.Dinamic)
                .WithExecutionType(ExecutionType.Async)
                .AddExtraData("CaminhoArquivo", caminhoArquivo)
                .AddExtraData("ReenvioPorArquivo", true)
                .CallReport();
        }
    }
}
