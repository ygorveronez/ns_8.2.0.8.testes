using AdminMultisoftware.Dominio.Entidades.Pessoas;
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Canhotos
{
    public class CanhotoIntegracao : ServicoBase
    {
        public CanhotoIntegracao(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, ClienteURLAcesso clienteURLAcesso, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, clienteURLAcesso, cancelationToken)
        {
        }
        public CanhotoIntegracao(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Privados

        private async Task GerarCanhotosIntegracoesPorTiposIntegracaoAsync(List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                if (tipoIntegracao.Tipo == TipoIntegracao.Mars)
                {
                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = canhotos.OrderByDescending(canhoto => canhoto.DataDigitalizacao).FirstOrDefault();

                    if (canhoto != null)
                        await CriarCanhotoIntegracaoAsync(canhoto, tipoIntegracao, tipoServicoMultisoftware);
                }
                else
                {
                    foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                        await CriarCanhotoIntegracaoAsync(canhoto, tipoIntegracao, tipoServicoMultisoftware);
                }
            }
        }

        private async Task GerarIntegracoesParaCanhotosDigitalizadosAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (canhoto.Carga == null)
                return;

            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork, _cancellationToken);

            if (await repositorioCanhoto.VerificarSeExisteCanhotoNotaFiscalPendenteDigitalizacaoPorCargaAsync(canhoto.Carga.Codigo, canhoto.Codigo))
                return;

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosParaGerarIntegracao = await repositorioCanhoto.BuscarPorCargaAsync(canhoto.Carga.Codigo);

            await GerarCanhotosIntegracoesPorTiposIntegracaoAsync(canhotosParaGerarIntegracao, tiposIntegracao, tipoServicoMultisoftware);
        }

        #endregion

        #region Métodos Públicos

        public async Task GerarIntegracaoAceiteCanhotoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado)
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork, _cancellationToken);

            List<TipoIntegracao> tiposIntegracaoAceiteCanhotoReprovado = new List<TipoIntegracao> { TipoIntegracao.Mars };

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = await repositorioTipoIntegracao.BuscarPorTiposAsync(tiposIntegracaoAceiteCanhotoReprovado);

            if (tiposIntegracao.Count == 0)
                return;

            if (canhoto.TipoCanhoto == TipoCanhoto.Avulso)
                await GerarCanhotosIntegracoesPorTiposIntegracaoAsync(new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> { canhoto }, tiposIntegracao, tipoServicoMultisoftware);
            else
                await GerarIntegracoesParaCanhotosDigitalizadosAsync(canhoto, tiposIntegracao, tipoServicoMultisoftware);

        }

        public static void GerarIntegracaoDigitalizacaoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork, bool possuiFlagDoisOuMaisPaginas = false, bool aceitarEnvio = false)
        {
            new CanhotoIntegracao(unitOfWork).GerarIntegracaoDigitalizacaoCanhotoAsync(canhoto, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, possuiFlagDoisOuMaisPaginas, aceitarEnvio).GetAwaiter().GetResult();
        }

        public async Task GerarIntegracaoDigitalizacaoCanhotoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, bool possuiFlagDoisOuMaisPaginas = false, bool aceitarEnvio = false)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto repositorioGrupoPessoasFaturaCanhoto = new(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new(_unitOfWork, _cancellationToken);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = await repositorioConfiguracaoCanhoto.BuscarConfiguracaoPadraoAsync();

            List<TipoIntegracao> tiposIntegracaoGeracaoCanhoto = new()
            {
                TipoIntegracao.Piracanjuba,
                TipoIntegracao.Minerva,
                TipoIntegracao.Obramax,
                TipoIntegracao.Vedacit,
                TipoIntegracao.Buntech,
            };

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = await repositorioTipoIntegracao.BuscarPorTiposAsync(tiposIntegracaoGeracaoCanhoto);

            if (aceitarEnvio)
                await GerarIntegracaoAceiteCanhotoAsync(canhoto, tipoServicoMultisoftware);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                if (canhoto.TipoCanhoto != TipoCanhoto.NFe && canhoto.TipoCanhoto != TipoCanhoto.Avulso)
                    continue;

                if (((canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado && tipoIntegracao.Tipo != TipoIntegracao.Minerva && tipoIntegracao.Tipo != TipoIntegracao.Obramax) &&
                    !(tipoIntegracao.Tipo == TipoIntegracao.Piracanjuba && canhoto.TipoCanhoto != TipoCanhoto.Avulso && canhoto.XMLNotaFiscal != null && canhoto.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.NaoEntregue))
                    || tipoIntegracao.Tipo == TipoIntegracao.Mars)
                    continue;


                List<SituacaoDigitalizacaoCanhoto> tiposSituacaoDigitalizacaoCanhoto = new() { SituacaoDigitalizacaoCanhoto.AgAprovocao, SituacaoDigitalizacaoCanhoto.Digitalizado };

                Canhoto servicoCanhoto = new(_unitOfWork, _cancellationToken);

                bool todosCanhotosPorSituacao = await servicoCanhoto.TodosCanhotosPorSituacaoAsync(canhoto, tiposSituacaoDigitalizacaoCanhoto, TipoRegistroIntegracaoCTeCanhoto.SemTipo);

                if (tipoIntegracao.Tipo == TipoIntegracao.Vedacit && todosCanhotosPorSituacao)
                    continue;

                await CriarCanhotoIntegracaoAsync(canhoto, tipoIntegracao, tipoServicoMultisoftware);
            }

            if ((configuracaoCanhoto?.IntegrarCanhotosComValidadorIAComprovei ?? false) && !possuiFlagDoisOuMaisPaginas && !(configuracaoCanhoto.NaoIntegrarIAComproveiCanhotosDeNotasDevolvidas && canhoto.XMLNotaFiscal?.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Devolvida))
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(TipoIntegracao.Comprovei);

                if (tipoIntegracao != null && !(canhoto.XMLNotaFiscal?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.PossuiCanhotoDeDuasOuMaisPaginas ?? false))
                    await CriarCanhotoIntegracaoAsync(canhoto, tipoIntegracao, tipoServicoMultisoftware);
            }

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasEmitente = canhoto.Emitente.GrupoPessoas;

            //InforDoc
            if (grupoPessoasEmitente?.TipoIntegracao != null && grupoPessoasEmitente.TipoIntegracao.Tipo == TipoIntegracao.InforDoc)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(TipoIntegracao.InforDoc);

                if (tipoIntegracao != null && canhoto.TipoCanhoto == TipoCanhoto.NFe)
                    await CriarCanhotoIntegracaoAsync(canhoto, tipoIntegracao, tipoServicoMultisoftware);
            }

            if (grupoPessoasEmitente?.HabilitarIntegracaoDigitalizacaoCanhotoMultiEmbarcador ?? false)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(TipoIntegracao.MultiEmbarcador);

                if (tipoIntegracao != null && canhoto.TipoCanhoto == TipoCanhoto.NFe)
                    await CriarCanhotoIntegracaoAsync(canhoto, tipoIntegracao, tipoServicoMultisoftware);
            }
            else if (canhoto.Pedido != null && (grupoPessoasEmitente?.GerarOcorrenciaControleEntrega ?? false))
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia repositorioGrupoPessoaTipoOcorrencia = new(_unitOfWork, _cancellationToken);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia grupoPessoaTipoOcorrencia = await repositorioGrupoPessoaTipoOcorrencia.BuscarOcorrenciaCanhotoAsync(grupoPessoasEmitente.Codigo);

                if (grupoPessoaTipoOcorrencia != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarPorPedidoETomadorAsync(canhoto.Pedido.Codigo, canhoto.Emitente.CPF_CNPJ);

                    if (cargaPedido != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = await repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCargaCTesPorCargaPedidoAsync(cargaPedido.Codigo);

                        if (cargaCTes.Count == 0)
                            return;

                        Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarOcorrencia(carga, grupoPessoaTipoOcorrencia, null, null, null, cargaCTes, canhoto.DataDigitalizacao ?? DateTime.Now, null, null, string.Empty, 0m, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, _unitOfWork);
                    }
                }
            }

            if (grupoPessoasEmitente == null || grupoPessoasEmitente.Codigo == 0)
                return;

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto grupoPessoasFaturaCanhoto = (await repositorioGrupoPessoasFaturaCanhoto.BuscarPorGrupoPessoasETipoIntegracaoAsync(grupoPessoasEmitente.Codigo, Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoCanhoto.FTP, default))?.FirstOrDefault();

            if (grupoPessoasFaturaCanhoto?.HabilitarEnvioCanhoto ?? false)
            {
                string extensoes = grupoPessoasFaturaCanhoto?.ExtensaoArquivo ?? "";
                var extensoesPermitidas = extensoes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(ext => ext.Trim().Replace(".", "")).ToList();
                string extensaoArquivo = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower().Replace(".", "");

                if (extensoesPermitidas.Any() && !extensoesPermitidas.Contains(extensaoArquivo))
                    throw new Exception($"A extensão do aquivo ({extensaoArquivo}) não é compativel com as extensões configuradas ({extensoes})");

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = await repositorioTipoIntegracao.BuscarPorTipoAsync(TipoIntegracao.FTP);

                if (tipoIntegracao != null && canhoto.TipoCanhoto == TipoCanhoto.NFe)
                    await CriarCanhotoIntegracaoAsync(canhoto, tipoIntegracao, tipoServicoMultisoftware);
            }
        }


        public async Task VerificarIntegracoesCanhotosAsync()
        {
            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repositorioCanhotoIntegracao = new(_unitOfWork, _cancellationToken);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();
            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao> canhotoIntegracaos = await repositorioCanhotoIntegracao.BuscarCanhotoIntegracaoPendenteAsync(3, 5, "Codigo", "asc", 20, TipoEnvioIntegracao.Individual, configuracaoTMS, _cancellationToken);

            foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao in canhotoIntegracaos)
            {
                try
                {
                    switch (canhotoIntegracao.TipoIntegracao.Tipo)
                    {
                        case TipoIntegracao.Piracanjuba:
                            new Integracao.Piracanjuba.IntegracaoPiracanjuba(_unitOfWork, _tipoServicoMultisoftware).IntegracarCanhoto(canhotoIntegracao);
                            break;
                        case TipoIntegracao.MultiEmbarcador:
                            Servicos.Embarcador.Integracao.MultiEmbarcador.Canhoto.IntegracarCanhoto(canhotoIntegracao, _unitOfWork);
                            break;
                        case TipoIntegracao.InforDoc:
                            new Integracao.InforDoc.IntegracaoInforDoc(_unitOfWork).IntegrarCanhoto(canhotoIntegracao);
                            break;
                        case TipoIntegracao.Minerva:
                            new Integracao.Minerva.IntegracaoMinerva(_unitOfWork).IntegrarCanhoto(canhotoIntegracao);
                            break;
                        case TipoIntegracao.Comprovei:
                            new Integracao.Comprovei.IntegracaoComprovei(_unitOfWork).IntegrarIACanhoto(canhotoIntegracao);
                            break;
                        case TipoIntegracao.Mars:
                            new Integracao.Mars.IntegracaoMars(_unitOfWork).IntegrarCanhoto(canhotoIntegracao);
                            break;
                        case TipoIntegracao.Obramax:
                            new Integracao.Obramax.IntegracaoObramax(_unitOfWork, _clienteURLAcesso.URLAcesso).IntegrarCanhoto(canhotoIntegracao);
                            break;
                        case TipoIntegracao.FTP:
                            await new Integracao.FTP.IntegracaoFTP(_unitOfWork, _tipoServicoMultisoftware, _cancellationToken).EnviarCanhotoAsync(canhotoIntegracao);
                            break;
                        case TipoIntegracao.Vedacit:
                            new Integracao.Vedacit.IntegracaoVedacit(_unitOfWork).IntegracarCanhoto(canhotoIntegracao);
                            break;
                        case TipoIntegracao.Buntech:
                            await new Integracao.Buntech.IntegracaoBuntech(_unitOfWork).IntegrarCanhotoAsync(canhotoIntegracao);
                            break;
                        default:
                            canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            canhotoIntegracao.ProblemaIntegracao = "Tipo de integração não implementada";
                            await repositorioCanhotoIntegracao.AtualizarAsync(canhotoIntegracao);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "VerificarIntegracoesCanhotos");
                }
            }
        }

        public async Task CriarCanhotoIntegracaoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repositorioCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork, _cancellationToken);

            Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao = await repositorioCanhotoIntegracao.BuscarPorCanhotoETipoIntegracaoAsync(canhoto.Codigo, tipoIntegracao.Codigo);

            if (canhotoIntegracao == null || tipoIntegracao.Tipo == TipoIntegracao.Minerva || tipoIntegracao.Tipo == TipoIntegracao.Comprovei)
            {
                canhotoIntegracao = new Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao();
                canhotoIntegracao.Canhoto = canhoto;
                canhotoIntegracao.DataIntegracao = DateTime.Now;
                canhotoIntegracao.ProblemaIntegracao = string.Empty;
                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                canhotoIntegracao.TipoIntegracao = tipoIntegracao;
                canhotoIntegracao.TipoServicoMultisoftware = tipoServicoMultisoftware;
                await repositorioCanhotoIntegracao.InserirAsync(canhotoIntegracao);

                if (tipoIntegracao.Tipo == TipoIntegracao.Comprovei && canhoto.TipoCanhoto != TipoCanhoto.Avulso)
                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgIntegracao;
            }

            if (canhotoIntegracao != null && canhotoIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgIntegracao)
            {
                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                canhotoIntegracao.IniciouConexaoExterna = false;
                await repositorioCanhotoIntegracao.AtualizarAsync(canhotoIntegracao);
            }
        }

        #endregion

    }
}
