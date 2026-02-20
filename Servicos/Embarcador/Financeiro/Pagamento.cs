using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Financeiro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Financeiro
{
    public class Pagamento
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Pagamento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void VerificarSeNecessariaAutorizacaoValorMaximoPendentePagamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga repValorMaximoPendenteAutorizacaoCarga = new Repositorio.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            if (configuracao.ValidarValorMaximoPendentePagamento != true)
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);
            Dominio.Entidades.Cliente tomador = cargaPedido?.ObterTomador();

            if (tomador == null)
                return;

            decimal valorMaximoPendentePagamento = 0m;

            if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
                valorMaximoPendentePagamento = carga.TipoOperacao.ValorMaximoEmissaoPendentePagamento ?? 0m;
            else
            {
                if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                    valorMaximoPendentePagamento = tomador.ValorMaximoEmissaoPendentePagamento ?? 0m;
                else if (tomador.GrupoPessoas != null)
                    valorMaximoPendentePagamento = tomador.GrupoPessoas.ValorMaximoEmissaoPendentePagamento ?? 0m;
            }

            if (valorMaximoPendentePagamento <= 0m)
            {
                valorMaximoPendentePagamento = tomador.ValorMaximoEmissaoPendentePagamento ?? 0m;
                if (valorMaximoPendentePagamento <= 0m)
                {
                    valorMaximoPendentePagamento = tomador.GrupoPessoas?.ValorMaximoEmissaoPendentePagamento ?? 0m;
                    if (valorMaximoPendentePagamento <= 0m)
                        return;
                }
            }

            decimal valorPendente = repDocumentoFaturamento.ObterValorTotalNaoPagoPorPessoaOuGrupoPessoas(tomador.GrupoPessoas?.Codigo ?? 0, tomador.CPF_CNPJ);

            if (valorPendente <= valorMaximoPendentePagamento)
                return;

            StringBuilder mensagem = new StringBuilder();

            mensagem.Append(tomador.Descricao);

            if (tomador.GrupoPessoas != null)
                mensagem.Append($" ({tomador.GrupoPessoas.Descricao})");

            mensagem.Append($" possui um valor a pagar ({valorPendente.ToString("n2")}) maior que o permitido ({valorMaximoPendentePagamento.ToString("n2")}), não sendo possível prosseguir com a emissão.");

            Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga valorMaximoPendenteAutorizacaoCarga = new Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga()
            {
                Carga = carga,
                Mensagem = Utilidades.String.Left(mensagem.ToString(), 500),
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValorMaximoPendenteAutorizacaoCarga.AgLiberacao
            };

            repValorMaximoPendenteAutorizacaoCarga.Inserir(valorMaximoPendenteAutorizacaoCarga);
        }

        public static void ExtornarAutorizacoesValorMaximoPendentePagamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, string motivoExtorno)
        {
            Repositorio.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga repValorMaximoPendenteAutorizacaoCarga = new Repositorio.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga> autorizacoesExtornar = repValorMaximoPendenteAutorizacaoCarga.BuscarNaoExtornadasPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga autorizacaoExtornar in autorizacoesExtornar)
            {
                if (autorizacaoExtornar.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValorMaximoPendenteAutorizacaoCarga.Liberada)
                {
                    autorizacaoExtornar.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValorMaximoPendenteAutorizacaoCarga.AutorizacaoExtornada;
                    autorizacaoExtornar.MotivoExtornoAutorizacao = motivoExtorno;

                    repValorMaximoPendenteAutorizacaoCarga.Atualizar(autorizacaoExtornar);
                }
                else
                    repValorMaximoPendenteAutorizacaoCarga.Deletar(autorizacaoExtornar);
            }
        }

        public static bool VerificarSeEnecessarioAutorizacaoValorMaximoPendentePagamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, out string mensagem)
        {
            mensagem = string.Empty;

            Repositorio.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga repValorMaximoPendenteAutorizacaoCarga = new Repositorio.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga> autorizacoes = repValorMaximoPendenteAutorizacaoCarga.BuscarNaoExtornadasPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.ValorMaximoPendenteAutorizacaoCarga autorizacao in autorizacoes)
            {
                if (autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValorMaximoPendenteAutorizacaoCarga.AgLiberacao)
                {
                    mensagem = autorizacao.Mensagem;
                    return true;
                }
            }

            return false;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> ObterDocumentosPagarGeracaoAutomaticaDoPagamento(List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosValidar)
        {

            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumentos = new Repositorio.Embarcador.Documentos.ControleDocumento(_unitOfWork);
            Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGuiaRecolhimentoTributoEstadual = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoCte = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioCargaPedidoXml = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            List<int> codigoDocumentos = documentosValidar.Select(x => x.CTe.Codigo).Distinct().ToList();
            List<int> codigoTipoOperacao = documentosValidar.Where(x => x.CargaPagamento.TipoOperacao != null).Select(x => x.CargaPagamento.TipoOperacao.Codigo).Distinct().ToList();
            List<int> codigoTransportador = documentosValidar.Where(x => x.CargaPagamento.Empresa != null).Select(x => x.CargaPagamento.Empresa.Codigo).Distinct().ToList();
            List<int> codigosCargas = documentosValidar.Where(x => x.CargaPagamento != null).Select(x => x.CargaPagamento.Codigo).Distinct().ToList();

            List<(int codigo, int codigoCte, SituacaoControleDocumento situacao)> statusDocumentosControle = repositorioControleDocumentos.PesquisarDocumentoPelosCte(codigoDocumentos);
            List<(int codigo, int codigoCte)> statusGuiaRecolhimentoNaoValidas = repositorioGuiaRecolhimentoTributoEstadual.PesquisarGuiasNaoValidasPelosCte(codigoDocumentos);
            List<(int codigo, TipoLiberacaoPagamento tipo)> tipoOperacoes = repositorioTipoOperacao.BuscarTipoOperacaoPorCodigo(codigoTipoOperacao);
            List<(int codigoTransportador, bool emiteDocumentoNaMulti)> transportadores = repositorioEmpresa.BuscarTransportadores(codigoTransportador);
            List<(int codigo, int codigoCarga, SituacaoDigitalizacaoCanhoto situacaoDigitalizacao)> situacaoDigitalizacao = repositorioCargaPedidoCte.BuscarDadosCanhotos(codigosCargas);
            List<(int codigo, int codigoCargaPedido)> dadosMiro = repositorioCargaPedidoCte.BuscarDadosMiro(codigoDocumentos);
            List<(int codigo, int codigoCarga)> codigoOcorrencia = repositorioCargaOcorrencia.BuscarPorCodigoCargas(codigosCargas);
            List<(int codigoCarga, int quantideNota)> quantidadeDocumentos = repositorioCargaPedidoXml.BuscarQuantidadeNotasCarga(codigosCargas);
            IList<RetornoDocumentoPagamentoStage> retornoDocumentsoStage = repositorioDocumentoFaturamento.BuscarDadosStagePorCte(codigoDocumentos);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> novaListaRetornar = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaTemp = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento in documentosValidar)
            {
                RetornoDocumentoPagamentoStage dadosStage = retornoDocumentsoStage.Where(x => x.CodigoDocumento == documento.CTe.Codigo).FirstOrDefault();

                if (dadosStage == null || dadosStage.CodigoProvisao == 0 || string.IsNullOrEmpty(dadosStage.NumeroFolha) || dadosStage.SituacaoProvisao != SituacaoProvisao.Finalizado || dadosStage.StageCancelada)
                    continue;

                var transportador = transportadores.Where(x => x.codigoTransportador == documento.CargaPagamento?.Empresa?.Codigo).FirstOrDefault();
                int horasValidar = transportador.emiteDocumentoNaMulti ? configuracaoFinanceiro.HorasLiberacaoDocumentoPagamentoTransportadorSemCertificado : configuracaoFinanceiro.HorasLiberacaoDocumentoPagamentoTransportadorComCertificado;

                DateTime dataAtual = DateTime.Now;
                var restante = (dataAtual - documento.DataEmissao).TotalHours;

                if (restante >= horasValidar)
                    novaListaRetornar.Add(documento);
            }

            foreach (var itemListaRetornar in novaListaRetornar)
            {
                var existeGuiNaoValida = statusGuiaRecolhimentoNaoValidas.Where(x => x.codigoCte == itemListaRetornar.CTe.Codigo).FirstOrDefault();

                if (existeGuiNaoValida.codigo > 0)
                    continue;

                var existeCargaOcorrencia = codigoOcorrencia.Where(x => x.codigoCarga == itemListaRetornar.CargaPagamento.Codigo).FirstOrDefault();

                if (existeCargaOcorrencia.codigo > 0)
                {
                    var existeDocumentoNoControle = statusDocumentosControle.Where(x => x.codigoCte == itemListaRetornar.CTe.Codigo).FirstOrDefault();

                    if (existeDocumentoNoControle.codigo > 0 && existeDocumentoNoControle.situacao != SituacaoControleDocumento.Liberado)
                        continue;

                    if (itemListaRetornar.CargaOcorrenciaPagamento != null && itemListaRetornar.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                    {
                        bool recebeuMiro = repPagamentoIntegracao.PagamentoOutrarDocumentoRecebeuMiro(itemListaRetornar.CargaPagamento.Codigo);

                        if (recebeuMiro)
                            listaTemp.Add(itemListaRetornar);
                        continue;
                    }
                }

                var tipoOperacao = tipoOperacoes.Where(x => itemListaRetornar.CargaPagamento.TipoOperacao.Codigo == x.codigo).FirstOrDefault();

                if (tipoOperacao.tipo == TipoLiberacaoPagamento.DigitalizacaoImagemCanhoto)
                {
                    var canhotosDigitalizados = situacaoDigitalizacao.Where(x => x.codigoCarga == itemListaRetornar.CargaPagamento.Codigo).ToList();

                    if (!canhotosDigitalizados.Any(x => x.situacaoDigitalizacao != SituacaoDigitalizacaoCanhoto.AgAprovocao && x.situacaoDigitalizacao != SituacaoDigitalizacaoCanhoto.Digitalizado))
                        listaTemp.Add(itemListaRetornar);
                    continue;
                }

                if (tipoOperacao.codigo > 0 && tipoOperacao.tipo == TipoLiberacaoPagamento.AprovacaoImagemCanhoto)
                {
                    var canhotosDigitalizados = situacaoDigitalizacao.Where(x => x.codigoCarga == itemListaRetornar.CargaPagamento.Codigo).ToList();

                    if (!canhotosDigitalizados.Any(x => x.situacaoDigitalizacao != SituacaoDigitalizacaoCanhoto.Digitalizado))
                        listaTemp.Add(itemListaRetornar);
                    continue;
                }

                if (tipoOperacao.codigo > 0 && tipoOperacao.tipo == TipoLiberacaoPagamento.ReceberEscrituracaoNotaProduto)
                {
                    var dadoMiroDocumento = dadosMiro.Where(x => itemListaRetornar.CargaPagamento.Pedidos.Any(p => p.Codigo == x.codigoCargaPedido)).FirstOrDefault();

                    if (dadoMiroDocumento.codigo > 0)
                        listaTemp.Add(itemListaRetornar);
                    continue;
                }

                listaTemp.Add(itemListaRetornar);
            }

            var novaLista = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            foreach (var carga in codigosCargas)
            {
                var item = quantidadeDocumentos.Where(x => x.codigoCarga == carga).FirstOrDefault();
                int quantideDocumentosPorCarga = listaTemp.Where(x => x.CargaPagamento.Codigo == carga).Count();
                if (item.codigoCarga > 0 && item.quantideNota == quantideDocumentosPorCarga)
                    novaLista.AddRange(listaTemp.Where(x => x.CargaPagamento.Codigo == carga).ToList());
            }

            return novaLista;
        }
        #endregion
    }
}
