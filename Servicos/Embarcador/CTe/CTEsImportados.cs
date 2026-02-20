using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.CTe
{
    public class CTEsImportados : ServicoBase
    {
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        public CTEsImportados(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        public CTEsImportados(UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
            _auditado = auditado;
        }
        public CTEsImportados(UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, cancelationToken)
        {
        }

        #region Métodos Públicos

        public bool VincularNotasFiscaisDaCargaTransbordada(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaPedidoDocumentosCTe = repCargaPedidoDocumentoCTe.BuscarPorCargaPedido(cargaPedido.Codigo);

            bool possuiSubcontratacao = false, possuiNotas = false;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe in cargaPedidoDocumentosCTe)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in cargaPedidoDocumentoCTe.CTe.XMLNotaFiscais)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repCargaPedidoXMLNotaFiscalCTe.BuscarXMLNotasFiscaisPorCTeENotaFiscal(cargaPedidoDocumentoCTe.CTe.Codigo, xmlNotaFiscal.Codigo);

                    if (pedidoXMLNotaFiscal == null)
                    {
                        mensagemErro = $"Não foi encontrado a nota fiscal vinculada à carga original para o CT-e '{cargaPedidoDocumentoCTe.CTe.Descricao}' e nota fiscal '{xmlNotaFiscal.Numero}'.";
                        return false;
                    }

                    if (pedidoXMLNotaFiscal.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao)
                        possuiNotas = true;

                    AdicionarNFeCargaPedido(xmlNotaFiscal, cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, tipoServicoMultisoftware, unitOfWork);
                }

                foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documentoTransporteAnterior in cargaPedidoDocumentoCTe.CTe.DocumentosTransporteAnterior)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = repCTeTerceiro.BuscarPorChave(documentoTransporteAnterior.Chave);

                    if (cteTerceiro != null)
                    {
                        possuiSubcontratacao = true;
                        AdicionarCTeTerceiroCargaPedido(cteTerceiro, cargaPedido, tipoServicoMultisoftware, unitOfWork);

                        if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                            serCarga.AtualizarTipoServicoCargaMultimodal(cteTerceiro, cargaPedido, unitOfWork, out string msgRetornoTipoServico);
                    }
                }
            }

            if (!(integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false))
            {
                if (possuiNotas)
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
                else if (possuiSubcontratacao)
                    cargaPedido.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;
            }

            repCargaPedido.Atualizar(cargaPedido);

            mensagemErro = null;
            return true;
        }

        public string CriarNotasFiscaisDaCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool obterDocumentosDoCTeComplementado = false, List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal> cargaIntegracaoEmbarcadorPedidoNotasFiscais = null, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null, bool adicionarNotaFiscalPedido = true)
        {
            string retorno = string.Empty;

            cargaPedido.ValorFreteAPagar = 0;
            cargaPedido.ValorFrete = 0;
            cargaPedido.ValorICMS = 0;
            cargaPedido.PercentualAliquota = 0;
            cargaPedido.CFOP = null;

            var repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            var repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            var repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            var repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            var repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            var repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            var repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unitOfWork);
            var repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);
            var repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            var repComponenteCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
            var repCargaPedidoDocumentoCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete(unitOfWork);
            var repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
            var repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            var configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            Servicos.Embarcador.Carga.Carga svcCarga = new Carga.Carga(unitOfWork);
            Servicos.Embarcador.NFe.NFe serNFe = new NFe.NFe(unitOfWork);
            Servicos.Embarcador.Carga.CTeSubContratacao servicoCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

            repCargaPedidoDocumentoCTeComponenteFrete.DeletarPorCargaPedido(cargaPedido.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaPedidoDocumentosCTe = repCargaPedidoDocumentoCTe.BuscarPorCargaPedidoParaProcessamento(cargaPedido);

            IEnumerable<int> codigosCTes = cargaPedidoDocumentosCTe.Select(o => o.CTe.Codigo);

            List<Dominio.Entidades.InformacaoCargaCTE> quantidadesCargaCTeGeral = repInformacaoCargaCTe.BuscarPorCTe(codigosCTes);
            List<Dominio.Entidades.DocumentosCTE> documentosCTeGeral = repDocumentoCTe.BuscarPorCTe(codigosCTes);
            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesCTeGeral = repComponenteCTe.BuscarPorCTe(codigosCTes);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponenteFrete.BuscarPorCargaComImpostos(cargaPedido.Carga.Codigo, false);
            string descricaoItemPeso = servicoCTeSubContratacao.ObterDescricaoItemPeso(cargaPedido, unitOfWork, out bool utilizarPrimeiraUnidadeMedidaPeso);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete in cargaComponentesFrete)
                repCargaComponenteFrete.Deletar(cargaComponenteFrete);

            cargaComponentesFrete = new List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();

            Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcador configuracao = ObterConfiguracoesComponentes(cargaPedido);

            //se tem mais de um tipo de tomador nos ct-es não deixa alterar o tomador do pedido, senão o tomador da carga pode ficar diferente do real (ex: CT-es a pagar e Pagos na mesma carga, BRF)
            bool permiteAlterarTomador = cargaPedidoDocumentosCTe.Select(o => o.CTe.TipoTomador).Distinct().Count() <= 1;

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTeBase = cargaPedidoDocumentosCTe.Find(o => o.CTe?.Remetente?.Cliente != null && o.CTe?.Destinatario?.Cliente != null);

            if (cargaPedidoDocumentoCTeBase != null)
            {
                if (cargaPedido.Pedido.Remetente == null)
                {
                    cargaPedido.Pedido.Remetente = cargaPedidoDocumentoCTeBase.CTe.Remetente?.Cliente ?? cargaPedidoDocumentoCTeBase.CTe.Expedidor?.Cliente;
                    cargaPedido.Origem = cargaPedidoDocumentoCTeBase.CTe.Remetente?.Localidade ?? cargaPedidoDocumentoCTeBase.CTe.Expedidor?.Localidade;
                }

                if (cargaPedido.Pedido.Destinatario == null)
                {
                    cargaPedido.Pedido.Destinatario = cargaPedidoDocumentoCTeBase.CTe.Destinatario?.Cliente ?? cargaPedidoDocumentoCTeBase.CTe.Recebedor.Cliente;
                    cargaPedido.Destino = cargaPedidoDocumentoCTeBase.CTe.Destinatario?.Localidade ?? cargaPedidoDocumentoCTeBase.CTe.Recebedor.Localidade;
                }
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTeBaseEmpresa = cargaPedidoDocumentosCTe[0];

            cargaPedido.IncluirICMSBaseCalculo = false;
            cargaPedido.Carga.Empresa = cargaPedidoDocumentoCTeBaseEmpresa.CTe.Empresa;
            cargaPedido.PercentualAliquota = cargaPedidoDocumentoCTeBaseEmpresa.CTe.AliquotaICMS;
            cargaPedido.CFOP = cargaPedidoDocumentoCTeBaseEmpresa.CTe.CFOP;
            cargaPedido.Pedido.TipoPagamento = cargaPedidoDocumentoCTeBaseEmpresa.CTe.TipoPagamento;
            cargaPedido.Pedido.UsarTipoPagamentoNF = false;

            if (permiteAlterarTomador)
                cargaPedido.TipoTomador = cargaPedidoDocumentoCTeBaseEmpresa.CTe.TipoTomador;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe in cargaPedidoDocumentosCTe)
            {
                if (cargaPedidoDocumentoCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    cargaPedido.ValorISS += cargaPedidoDocumentoCTe.CTe.ValorISS;
                else
                    cargaPedido.ValorICMS += cargaPedidoDocumentoCTe.CTe.ValorICMS;

                AdicionarComponentesDoCTe(ref cargaComponentesFrete, cargaPedido, cargaPedidoDocumentoCTe, configuracao, unitOfWork, componentesCTeGeral);

                cargaPedido.ValorFreteAPagar += cargaPedidoDocumentoCTe.CTe.ValorAReceber;
                cargaPedido.ValorFrete += cargaPedidoDocumentoCTe.CTe.ValorFrete;

                if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                {
                    if (cargaPedidoDocumentoCTe.CTe.OutrosTomador?.Cliente != null)
                        cargaPedido.Tomador = cargaPedidoDocumentoCTe.CTe.OutrosTomador.Cliente;
                    else if (cargaPedidoDocumentoCTe.CTe.TomadorPagador?.Cliente != null)
                        cargaPedido.Tomador = cargaPedidoDocumentoCTe.CTe.TomadorPagador.Cliente;
                }

                decimal pesoKG = 0m;

                List<Dominio.Entidades.InformacaoCargaCTE> quantidadesCargaCTe = quantidadesCargaCTeGeral.Where(o => o.CTE.Codigo == cargaPedidoDocumentoCTe.CTe.Codigo).ToList();
                List<Dominio.Entidades.DocumentosCTE> documentosCTe = documentosCTeGeral.Where(o => o.CTE.Codigo == cargaPedidoDocumentoCTe.CTe.Codigo).ToList();

                if (documentosCTe.Count == 0 && obterDocumentosDoCTeComplementado && cargaPedidoDocumentoCTe.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && !string.IsNullOrWhiteSpace(cargaPedidoDocumentoCTe.CTe.ChaveCTESubComp) && cargaPedidoDocumentoCTe.CTe.ChaveCTESubComp.Length == 44)
                    documentosCTe = repDocumentoCTe.BuscarPorChaveCTe(cargaPedidoDocumentoCTe.CTe.ChaveCTESubComp);

                foreach (Dominio.Entidades.InformacaoCargaCTE info in quantidadesCargaCTe)
                    pesoKG += obterPesoEmKG(info, cargaPedidoDocumentosCTe.Count, descricaoItemPeso);

                if (cargaPedidoDocumentoCTe.CTe.XMLNotaFiscais == null)
                    cargaPedidoDocumentoCTe.CTe.XMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                if (adicionarNotaFiscalPedido)
                {
                    foreach (Dominio.Entidades.DocumentosCTE documento in documentosCTe)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                        if (documento.ModeloDocumentoFiscal?.Numero == "55")
                        {
                            decimal pesoProvenienteEmbarcador = 0m;
                            decimal valorProvenienteEmbarcador = 0m;

                            if (cargaIntegracaoEmbarcadorPedidoNotasFiscais != null && !string.IsNullOrWhiteSpace(documento.ChaveNFE) && documento.ChaveNFE.Length == 44)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcadorPedidoNotaFiscal notaFiscalEmbarcador = cargaIntegracaoEmbarcadorPedidoNotasFiscais.Find(o => o.Chave == documento.ChaveNFE);

                                if (notaFiscalEmbarcador != null)
                                {
                                    pesoProvenienteEmbarcador = notaFiscalEmbarcador.Peso;
                                    valorProvenienteEmbarcador = notaFiscalEmbarcador.Valor;
                                }
                            }

                            if (configuracaoTMS?.UtilizarNotaFiscalExistenteNaImportacaoCTeEmbarcador ?? false)
                                xmlNotaFiscal = repXMLNotaFiscal.BuscarPorChave(documento.ChaveNFE);

                            if (xmlNotaFiscal == null)
                            {
                                xmlNotaFiscal = serNFe.CriarXMLNotaFiscal(cargaPedidoDocumentoCTe.CTe.Remetente?.Cliente ?? cargaPedidoDocumentoCTe.CTe.Expedidor.Cliente, cargaPedidoDocumentoCTe.CTe.Destinatario?.Cliente ?? cargaPedidoDocumentoCTe.CTe.Recebedor.Cliente, pesoProvenienteEmbarcador > 0m ? pesoProvenienteEmbarcador : pesoKG / documentosCTe.Count, cargaPedido, unitOfWork);

                                xmlNotaFiscal.DataEmissao = documento.DataEmissao;
                                xmlNotaFiscal.Numero = int.Parse(documento.Numero);
                                xmlNotaFiscal.Serie = Utilidades.String.Left(documento.Serie, 3);
                                xmlNotaFiscal.Chave = documento.ChaveNFE;
                                xmlNotaFiscal.Modelo = "55";
                                xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;

                                if (valorProvenienteEmbarcador > 0m)
                                    xmlNotaFiscal.Valor = valorProvenienteEmbarcador;
                                else if (configuracaoGeralCarga.AtribuirValorMercadoriaCTeNotasFiscaisDocumentosEmitidosEmbarcador)
                                {
                                    var mediaValorProdutos = 0M;
                                    var countNfs = cargaPedidoDocumentoCTe.CTe.Documentos.Count();
                                    var valorTotalNfs = cargaPedidoDocumentoCTe.CTe.ValorTotalMercadoria;
                                    mediaValorProdutos = valorTotalNfs / countNfs;
                                    xmlNotaFiscal.Valor = mediaValorProdutos;
                                }
                            }

                            retorno = AdicionarNFeCargaPedido(xmlNotaFiscal, cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, configuracaoCanhoto, pedidoXMLNotasFiscais);
                        }
                        else if (documento.ModeloDocumentoFiscal?.Numero == "01")
                        {
                            xmlNotaFiscal = serNFe.CriarXMLNotaFiscal(cargaPedidoDocumentoCTe.CTe.Remetente?.Cliente ?? cargaPedidoDocumentoCTe.CTe.Expedidor.Cliente, cargaPedidoDocumentoCTe.CTe.Destinatario?.Cliente ?? cargaPedidoDocumentoCTe.CTe.Recebedor.Cliente, pesoKG / documentosCTe.Count, cargaPedido, unitOfWork);
                            xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NotaFiscal;
                            xmlNotaFiscal.DataEmissao = documento.DataEmissao;
                            xmlNotaFiscal.Numero = int.Parse(Utilidades.String.OnlyNumbers(documento.Numero));
                            xmlNotaFiscal.Serie = documento.Serie;
                            xmlNotaFiscal.Valor = documento.Valor;
                            xmlNotaFiscal.Modelo = "01";
                            xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NotaFiscal;
                            retorno = AdicionarNFeCargaPedido(xmlNotaFiscal, cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, configuracaoCanhoto, pedidoXMLNotasFiscais);
                        }
                        else
                        {
                            xmlNotaFiscal = serNFe.CriarXMLNotaFiscal(cargaPedidoDocumentoCTe.CTe.Remetente?.Cliente ?? cargaPedidoDocumentoCTe.CTe.Expedidor.Cliente, cargaPedidoDocumentoCTe.CTe.Destinatario?.Cliente ?? cargaPedidoDocumentoCTe.CTe.Recebedor.Cliente, pesoKG / documentosCTe.Count, cargaPedido, unitOfWork);
                            xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros;
                            xmlNotaFiscal.Modelo = documento.ModeloDocumentoFiscal?.Numero ?? "99";
                            xmlNotaFiscal.DataEmissao = DateTime.Now;
                            xmlNotaFiscal.Descricao = documento.ModeloDocumentoFiscal?.Descricao ?? "OUTROS";
                            xmlNotaFiscal.Numero = int.Parse(Utilidades.String.OnlyNumbers(documento.Numero));
                            xmlNotaFiscal.Valor = documento.Valor;
                            retorno = AdicionarNFeCargaPedido(xmlNotaFiscal, cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda, tipoServicoMultisoftware, unitOfWork, configuracaoTMS, configuracaoCanhoto, pedidoXMLNotasFiscais);
                        }

                        if (!string.IsNullOrWhiteSpace(retorno))
                            return retorno;

                        cargaPedidoDocumentoCTe.CTe.XMLNotaFiscais.Add(xmlNotaFiscal);
                    }

                    cargaPedidoDocumentoCTe.CTe.Peso = pesoKG;

                    repCTe.Atualizar(cargaPedidoDocumentoCTe.CTe);
                }
            }

            repCargaPedido.Atualizar(cargaPedido);
            repPedido.Atualizar(cargaPedido.Pedido);

            if (cargaPedido.CTeEmitidoNoEmbarcador)
                svcCarga.SetarTipoContratacaoCarga(cargaPedido.Carga, unitOfWork);

            repCarga.Atualizar(cargaPedido.Carga);

            return retorno;
        }
        public static bool RemoverCTeCargaPedido(out string erro, int codigoCTe, int codigoCargaPedido, UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = null, bool reprocessarOrdem = true)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            if (cargaPedidoDocumentoCTe == null)
                cargaPedidoDocumentoCTe = repCargaPedidoDocumentoCTe.BuscarPorCTeECargaPedido(codigoCTe, codigoCargaPedido);

            if (cargaPedidoDocumentoCTe != null)
            {
                if (cargaPedidoDocumentoCTe.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                {
                    erro = "Não é possivel remover o CT-e na atual situação da Carga (" + cargaPedidoDocumentoCTe.CargaPedido.Carga.DescricaoSituacaoCarga + ").";
                    return false;
                }

                if (cargaPedidoDocumentoCTe.CargaPedido.CTeEmitidoNoEmbarcador)
                {
                    cargaPedidoDocumentoCTe.CTe.CTeSemCarga = true;
                    repCTe.Atualizar(cargaPedidoDocumentoCTe.CTe);
                }

                repCargaPedidoDocumentoCTe.Deletar(cargaPedidoDocumentoCTe, Auditado);
            }

            if (reprocessarOrdem)
                ReprocessarOrdemCargaPedidoDocumentoCTe(codigoCargaPedido, unitOfWork);

            erro = string.Empty;
            return true;
        }

        public static void ReprocessarOrdemCargaPedidoDocumentoCTe(int codigoCargaPedido, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> documentos = repCargaPedidoDocumentoCTe.BuscarCargaPedido(codigoCargaPedido);
            for (int i = 0; i < documentos.Count; i++)
            {
                documentos[i].Ordem = i + 1;
                repCargaPedidoDocumentoCTe.Atualizar(documentos[i]);

            }
        }

        public void VincularCargaCTeEmitidoAnteriormente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, bool controlarTransacao = true)
        {
            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);
            Repositorio.MotoristaCTE repMotoristaCTe = new Repositorio.MotoristaCTE(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.LacreMDFe repLacreMDFe = new Repositorio.LacreMDFe(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete repCargaPedidoDocumentoCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Servicos.Embarcador.Carga.CTe servAverbacaoCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = cargaPedido.Pedido?.CentroResultado;
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            if (centroResultado == null && carga.TipoOperacao != null)
                centroResultado = repCentroResultado.BuscarPorTipoOperacao(carga.TipoOperacao);

            repVeiculoCTe.DeletarPorCargaPedido(cargaPedido.Codigo);
            repMotoristaCTe.DeletarPorCargaPedido(cargaPedido.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> cargaPedidoDocumentosMDFe = repCargaPedidoDocumentoMDFe.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaPedidoDocumentosCTe = repCargaPedidoDocumentoCTe.BuscarPorCargaPedidoParaVinculoCarga(cargaPedido);
            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponenteFrete.BuscarPorCargaComImpostos(carga.Codigo, false);
            List<Dominio.Entidades.DocumentosCTE> documentosCTesGeral = repDocumentoCTe.BuscarPorCTe(cargaPedidoDocumentosCTe.Select(o => o.CTe.Codigo));
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete> cargaPedidoDocumentoCTeComponentesFreteGerais = repCargaPedidoDocumentoCTeComponenteFrete.BuscarPorCargaPedido(cargaPedido);

            Dictionary<int, decimal> componentesAdicionarPedido = new Dictionary<int, decimal>();
            List<int> codigosCTesExistentesNaCarga = repCargaCTe.BuscarCodigosCTesPorCarga(carga);

            if (controlarTransacao)
                unitOfWork.Start();

            for (int i = 0; i < cargaPedidoDocumentosCTe.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = cargaPedidoDocumentosCTe[i];

                if (codigosCTesExistentesNaCarga.Exists(codigoCTe => codigoCTe == cargaPedidoDocumentoCTe.CTe.Codigo))
                    continue;

                List<Dominio.Entidades.DocumentosCTE> documentosCTes = documentosCTesGeral.FindAll(o => o.CTE.Codigo == cargaPedidoDocumentoCTe.CTe.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete> cargaPedidoDocumentoCTeComponentesFrete = cargaPedidoDocumentoCTeComponentesFreteGerais.FindAll(o => o.CargaPedidoDocumentoCTe.Codigo == cargaPedidoDocumentoCTe.Codigo);

                codigosCTesExistentesNaCarga.Add(cargaPedidoDocumentoCTe.CTe.Codigo);

                if (cargaPedidoDocumentoCTe.CTe.CentroResultadoFaturamento?.Codigo != centroResultado?.Codigo)
                {
                    cargaPedidoDocumentoCTe.CTe.CentroResultadoFaturamento = centroResultado;

                    repCTe.Atualizar(cargaPedidoDocumentoCTe.CTe);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
                {
                    Carga = carga,
                    CargaOrigem = cargaPedido.CargaOrigem ?? carga,
                    CTe = cargaPedidoDocumentoCTe.CTe,
                    DataVinculoCarga = DateTime.Now
                };

                if (cargaPedido.CTeEmitidoNoEmbarcador)
                    cargaCTe.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.OutrosEmissores;
                else
                    cargaCTe.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe;

                repCargaCTe.Inserir(cargaCTe);

                if (carga.Veiculo != null)
                {
                    Dominio.Entidades.VeiculoCTE veiculoCTe = new Dominio.Entidades.VeiculoCTE
                    {
                        ImportadoCarga = true,
                        CTE = cargaCTe.CTe,
                        Veiculo = carga.Veiculo
                    };

                    veiculoCTe.SetarDadosVeiculo(carga.Veiculo);

                    repVeiculoCTe.Inserir(veiculoCTe);

                    foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
                    {
                        Dominio.Entidades.VeiculoCTE reboqueCTe = new Dominio.Entidades.VeiculoCTE
                        {
                            ImportadoCarga = true,
                            CTE = cargaCTe.CTe,
                            Veiculo = reboque
                        };

                        reboqueCTe.SetarDadosVeiculo(reboque);

                        repVeiculoCTe.Inserir(reboqueCTe);
                    }
                }

                if (carga.Motoristas?.Count > 0)
                {
                    foreach (Dominio.Entidades.Usuario motorista in carga.Motoristas)
                    {
                        Dominio.Entidades.MotoristaCTE motoristaCTe = new Dominio.Entidades.MotoristaCTE()
                        {
                            CPFMotorista = motorista.CPF,
                            NomeMotorista = motorista.Nome,
                            CTE = cargaCTe.CTe
                        };

                        repMotoristaCTe.Inserir(motoristaCTe);
                    }
                }

                foreach (Dominio.Entidades.DocumentosCTE docCTe in documentosCTes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTE = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe
                    {
                        CargaCTe = cargaCTe,
                        PedidoXMLNotaFiscal = ObterPedidoXMLNotaFiscalPorDocumentoCTe(docCTe, pedidoXMLNotasFiscais)
                    };

                    if (cargaPedidoXMLNotaFiscalCTE.PedidoXMLNotaFiscal != null)
                        repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTE);
                }

                if (cargaPedido.CTeEmitidoNoEmbarcador)
                    AdicionarComponentesCargaCTe(cargaCTe, cargaPedidoDocumentoCTe, cargaComponentesFrete, ref componentesAdicionarPedido, unitOfWork, cargaPedidoDocumentoCTeComponentesFrete);

                //Averbação do cte do importado do embarcador
                bool averbarCTeImportadoDoEmbarcador = false;
                if (carga.TipoOperacao != null && carga.TipoOperacao.ConfiguracaoEmissao != null)
                    averbarCTeImportadoDoEmbarcador = carga.TipoOperacao.ConfiguracaoEmissao.AverbarCTeImportadoDoEmbarcador;

                if (averbarCTeImportadoDoEmbarcador && cargaPedido.CTeEmitidoNoEmbarcador)
                {
                    bool emitindoCTeFilialEmissora = false;
                    if (cargaPedido.Carga.EmpresaFilialEmissora != null && !cargaPedido.Carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                        emitindoCTeFilialEmissora = true;

                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apoliceSeguroAverbacaos = repApoliceSeguroAverbacao.BuscarPorCargaPedido(cargaPedido.Codigo, emitindoCTeFilialEmissora);

                    //Caso não tenha apolice vinculada ao pedido procura a cadastrada no tipo de operação 
                    if (!apoliceSeguroAverbacaos.Any() && carga.TipoOperacao.ApolicesSeguro.Any())
                    {
                        foreach (var apoliceSeguro in carga.TipoOperacao.ApolicesSeguro)
                        {
                            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao apoliceCargaPedido = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao()
                            {
                                CargaPedido = cargaPedido,
                                ApoliceSeguro = apoliceSeguro.ApoliceSeguro,
                                SeguroFilialEmissora = emitindoCTeFilialEmissora
                            };

                            repApoliceSeguroAverbacao.Inserir(apoliceCargaPedido);

                            apoliceSeguroAverbacaos.Add(apoliceCargaPedido);
                        }
                    }

                    servAverbacaoCTe.AverbaCargaCTe(cargaCTe, apoliceSeguroAverbacaos, unitOfWork, configuracaoEmbarcador, cargaPedido.Pedido.FormaAverbacaoCTE);
                }
            }

            if (cargaPedido.CTeEmitidoNoEmbarcador)
                AdicionarComponentesCargaPedido(cargaPedido, ref componentesAdicionarPedido, unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe cargaPedidoDocumentoMDFe in cargaPedidoDocumentosMDFe)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorMDFeECarga(cargaPedidoDocumentoMDFe.MDFe.Codigo, carga.Codigo);

                if (cargaMDFe != null)
                    continue;

                cargaMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFe()
                {
                    MDFe = cargaPedidoDocumentoMDFe.MDFe,
                    Carga = carga
                };

                if (cargaPedido.CTeEmitidoNoEmbarcador)
                {
                    cargaMDFe.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.OutrosEmissores;

                    foreach (Dominio.Entidades.LacreMDFe lacre in cargaPedidoDocumentoMDFe.MDFe.Lacres)
                        repLacreMDFe.Deletar(lacre);

                    if (carga.Lacres != null)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaLacre cargaLacre in carga.Lacres)
                        {
                            Dominio.Entidades.LacreMDFe lacreMDFe = new Dominio.Entidades.LacreMDFe()
                            {
                                MDFe = cargaPedidoDocumentoMDFe.MDFe,
                                Numero = cargaLacre.Numero
                            };

                            repLacreMDFe.Inserir(lacreMDFe);
                        }
                    }
                }
                else
                    cargaMDFe.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe;

                repCargaMDFe.Inserir(cargaMDFe);
            }

            carga.CTesEmDigitacao = false;

            repCarga.Atualizar(carga);

            if (controlarTransacao)
                unitOfWork.CommitChanges();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal ObterPedidoXMLNotaFiscalPorDocumentoCTe(Dominio.Entidades.DocumentosCTE docCTe, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais)
        {
            if (docCTe.ModeloDocumentoFiscal == null)
                return pedidoXMLNotasFiscais.Find(obj => obj.XMLNotaFiscal.Numero == int.Parse(docCTe.Numero));
            else if (docCTe.ModeloDocumentoFiscal.Numero == "55")
                return pedidoXMLNotasFiscais.Find(obj => obj.XMLNotaFiscal.Chave == docCTe.ChaveNFE);
            else if (docCTe.ModeloDocumentoFiscal.Numero == "01")
                return pedidoXMLNotasFiscais.Find(obj => obj.XMLNotaFiscal.Numero == int.Parse(docCTe.Numero) && obj.XMLNotaFiscal.Serie == docCTe.Serie);
            else
                return pedidoXMLNotasFiscais.Find(obj => obj.XMLNotaFiscal.Numero == int.Parse(docCTe.Numero));
        }

        public static bool VincularCTeACargaPedido(out string erro, int codigoCTe, int codigoCargaPedido, bool permiteCTeComplementar, UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

            if (cte == null)
            {
                erro = "CT-e não encontrado.";
                return false;
            }

            if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
            {
                erro = "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.";
                return false;
            }

            if (repCargaPedidoDocumentoCTe.ExistePorCTeECargaPedido(cte.Codigo, codigoCargaPedido)) //O CT-e já está vinculado nesta CargaPedido, não sendo necessário vinculá-lo novamente
            {
                erro = null;
                return true;
            }

            if (repCargaPedidoDocumentoCTe.ExistePorCTeECargaPedidoDiff(cte.Codigo, codigoCargaPedido) || repCargaCTe.ExisteAutorizadoPorCTe(cte.Codigo))
            {
                erro = $"O CT-e {cte.Numero}-{cte.Serie.Numero} já está vinculado à uma carga.";
                return false;
            }

            if (!permiteCTeComplementar && cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
            {
                erro = $"O CT-e {cte.Numero}-{cte.Serie.Numero} é um complemento, não sendo possível vincular à carga.";
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
            {
                CargaPedido = cargaPedido,
                CTe = cte
            };

            repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe, auditado);

            svcCTe.SalvarInformacoesMultiModal(cte, cargaPedido, cte.ValorAReceber, unitOfWork);
            if (!configuracaoTMS.UtilizaEmissaoMultimodal)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);
                carga.CargaIntegradaEmbarcador = true;
                repCarga.Atualizar(carga);
            }

            if (auditado != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, $"Vinculou o CT-e {cte.Descricao} à carga.", unitOfWork);

            cte.CTeSemCarga = false;

            repCTe.Atualizar(cte);

            erro = string.Empty;
            return true;
        }

        public void VerificarSeCargaPossuiAlgumCTe(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            bool permiteVincularCTeComplementar = Servicos.Embarcador.Carga.Carga.PermiteVincularCTeComplementoCarga(cargaPedido);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if ((tomador?.GrupoPessoas?.VincularCTePeloNumeroPedido ?? false) && !string.IsNullOrWhiteSpace(cargaPedido.Pedido?.NumeroPedidoEmbarcador))
                ctes = repCTe.BuscarPorNumeroPedido(cargaPedido.Pedido.NumeroPedidoEmbarcador, permiteVincularCTeComplementar);
            else if (cargaPedido.Carga.Veiculo != null)
                ctes = repCTe.ConsultarCTesSemCargaCompativeis(cargaPedido.Carga.Veiculo.Codigo, cargaPedido.Origem.Codigo, 0, permiteVincularCTeComplementar);

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                if (repCargaPedidoDocumentoCTe.ExistePorCargaPedidoECTe(cargaPedido.Codigo, cte.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                {
                    CargaPedido = cargaPedido,
                    CTe = cte
                };

                repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe, auditado);

                if (auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, $"Vinculou o CT-e {cte.Descricao} à carga.", unitOfWork);

                cte.CTeSemCarga = false;

                repCTe.Atualizar(cte);
            }
        }

        public void VerificarSeCargaPossuiAlgumCTeTerceiro(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            if (!(tomador?.GrupoPessoas?.VincularCTeSubcontratacaoPeloNumeroPedido ?? false))
                return;

            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);

            if (repPedidoXMLNotaFiscal.VerificarSeExistePorCargaPedido(cargaPedido.Codigo))
                return;

            if (repPedidoCTeParaSubcontratacao.VerificarSeExistePorCargaPedido(cargaPedido.Codigo))
                return;

            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctes = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();

            if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido?.NumeroPedidoEmbarcador))
                ctes = repCTeTerceiro.BuscarPorNumeroPedido(cargaPedido.Pedido.NumeroPedidoEmbarcador);

            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cte in ctes)
            {
                if (string.IsNullOrWhiteSpace(cte.ChaveAcesso))
                    continue;

                if (repPedidoCTeParaSubcontratacao.ExistePorCargaPedidoEChave(cargaPedido.Codigo, cte.ChaveAcesso))
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                string retorno = serCargaCteParaSubContratacao.InformarDadosCTeNaCarga(unitOfWork, null, cargaPedido, tipoServicoMultisoftware, ref pedidoCTeParaSubContratacao, false, cte);

                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    Servicos.Log.TratarErro($"Não vinculou o CT-e ({cte.ChaveAcesso}) automaticamente na carga {cargaPedido.Carga.CodigoCargaEmbarcador}: {retorno}");

                    if (auditado != null)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, $"Vinculou o CT-e para subcontratação {cte.Descricao} à carga.", unitOfWork);
                }
            }
        }

        public void DestinarCTeImportadoParaSeuDestino(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string nomeArquivo, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (!string.IsNullOrWhiteSpace(nomeArquivo))
                nomeArquivo = nomeArquivo.Replace(".xml", "");

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            bool abrirUnit = false,
                 adicionou = false;

            if (!unitOfWork.IsActiveTransaction())
            {
                unitOfWork.Start();
                abrirUnit = true;
            }

            if (configuracaoTMS.GerarCargaDeCTesRecebidosPorEmail)
                GerarCargaCTe(cte, unitOfWork, stringConexao, tipoServicoMultisoftware, auditado);
            else
                adicionou = DestinarCTeACarga(cte, unitOfWork, nomeArquivo, configuracaoTMS, auditado);

            if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && !adicionou) //caso seja um CT-e complementar e não foi vinculado a uma carga o sistema tenta verificar se pode gerar a ocorrência automaticamente
                DestinarCTeAOcorrencia(cte, unitOfWork, tipoServicoMultisoftware);
            else if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao)
                DestinarCTeAnulacao(cte, unitOfWork, tipoServicoMultisoftware);
            else if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
                DestinarCTeSubstituicao(cte, unitOfWork, tipoServicoMultisoftware, configuracaoTMS);

            if (abrirUnit)
                unitOfWork.CommitChanges();
        }

        public void VerificarCTeImportadoPertenceAlgumaCargaParaCancelamento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            bool abrirUnit = false;
            if (!unitOfWork.IsActiveTransaction())
            {
                unitOfWork.Start();
                abrirUnit = true;
            }

            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarTodosPorCTe(cte.Codigo);
            if (cargaCTes.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                {
                    if (cargaCTe.CargaCTeComplementoInfo == null || cargaCTe.CargaCTeComplementoInfo.CargaOcorrencia == null)
                        VerificarCancelamentoCarga(cargaCTe.Carga, unitOfWork, tipoServicoMultisoftware, configuracao);
                    else
                        VerificarCancelamentoOcorrencia(cargaCTe.CargaCTeComplementoInfo.CargaOcorrencia, unitOfWork, tipoServicoMultisoftware);
                }
            }
            else
            {
                if (!cte.CTeSemCarga)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentosCTe = repCargaPedidoDocumentoCTe.BuscarPorCTe(cte.Codigo);
                    if (cargaPedidoDocumentosCTe != null)
                        RemoverCTeCanceladoDaCarga(cargaPedidoDocumentosCTe, unitOfWork);

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = repCargaOcorrenciaDocumento.BuscarPorCTeImportado(cte.Codigo);
                    if (cargaOcorrenciaDocumento != null)
                        RemoverCTeOcorrencia(cargaOcorrenciaDocumento, unitOfWork, tipoServicoMultisoftware);
                }
            }

            if (abrirUnit)
                unitOfWork.CommitChanges();
        }

        public void RemoverDocumentosParaRetornarEtapa(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(unitOfWork);

            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in cargaPedido.NotasFiscais)
            {
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedidoXMLNotaFiscal.XMLNotaFiscal;

                serCanhoto.ExcluirCanhotoDaNotaFiscal(xmlNotaFiscal, unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> pedidoXMLNotaFiscalComponentesFrete = repPedidoXMLNotaFiscalComponenteFrete.BuscarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete pedidoXMLNotaFiscalComponenteFrete in pedidoXMLNotaFiscalComponentesFrete)
                    repPedidoXMLNotaFiscalComponenteFrete.Deletar(pedidoXMLNotaFiscalComponenteFrete);

                repIntegracaoAVIPED.DeletarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);
                repPedidoXMLNotaFiscal.Deletar(pedidoXMLNotaFiscal);

                if (!repPedidoXMLNotaFiscal.VerificarSeExisteEmOutroPedidoAberto(xmlNotaFiscal.Codigo, cargaPedido.Codigo))
                    repXMLNotaFiscal.Deletar(xmlNotaFiscal);
            }

            cargaPedido.Carga.ValorFrete = 0;
            cargaPedido.Carga.ValorFreteAPagar = 0;
            cargaPedido.Carga.ValorFreteEmbarcador = 0;
            cargaPedido.Carga.ValorICMS = 0;
        }

        public void GerarCargaCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            StringBuilder erro = new StringBuilder();

            try
            {
                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                    return;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
                Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga(unitOfWork);
                Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido(unitOfWork);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorChaveCTe(cte.Chave);

                Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = ConverterCTeEmCargaIntegracao(cte, unitOfWork);

                int codigoCargaExistente = 0;
                int protocoloPedidoExistente = 0;
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao ?? "");
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (cargaIntegracao.TipoOperacao != null) ? repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracao.TipoOperacao.CodigoIntegracao) : null;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref erro, tipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, true, auditado);

                if (erro.Length == 0 || protocoloPedidoExistente > 0)
                {
                    serProdutoPedidoWS.AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref erro, unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = serCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref erro, ref codigoCargaExistente, unitOfWork, tipoServicoMultisoftware, false, true, auditado, configuracaoTMS, null, "", filial, tipoOperacao);

                    if (cargaPedido != null)
                    {
                        serCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref erro, unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

                        if (cargaIntegracao.FecharCargaAutomaticamente)
                        {
                            Servicos.Log.TratarErro("10 - Fechou Carga (" + cargaPedido.Carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");
                            cargaPedido.Carga.CargaFechada = true;
                        }


                        int sequecia = 0;
                        int.TryParse(cargaIntegracao.NumeroCarga, out sequecia);
                        cargaPedido.Carga.NumeroSequenciaCarga = sequecia;
                        cargaPedido.Carga.Empresa = cte.Empresa;
                        cargaPedido.Carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                        cargaPedido.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;

                        cargaPedido.CTeEmitidoNoEmbarcador = true;
                        //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                        Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {cargaIntegracao.PesoBruto}. CTEsImportados.GerarCargaCTe", "PesoCargaPedido");
                        cargaPedido.Peso = cargaIntegracao.PesoBruto;
                        cargaPedido.PesoLiquido = cargaIntegracao.PesoLiquido;

                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                        {
                            CargaPedido = cargaPedido,
                            CTe = cte
                        };

                        repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);

                        cte.CTeSemCarga = false;

                        repCargaPedido.Atualizar(cargaPedido);
                        repCarga.Atualizar(cargaPedido.Carga);
                        repCTe.Atualizar(cte);

                        if (mdfe != null)
                            FinalizarGeracaoCargaPorMDFeAsync(mdfe, cargaPedido.Carga).GetAwaiter().GetResult();
                    }
                }

                if (erro.Length > 0)
                {
                    Servicos.Log.TratarErro("Carga: " + cargaIntegracao.NumeroCarga + " Retornou essa mensagem: " + erro.ToString());

                    unitOfWork.Rollback();

                    if (codigoCargaExistente > 0 || protocoloPedidoExistente > 0)
                        Servicos.Log.TratarErro("Carga: " + cargaIntegracao.NumeroCarga + " já existente. Código carga: " + codigoCargaExistente + " Protocolo Pedido: " + protocoloPedidoExistente);
                }
                else
                {
                    unitOfWork.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Carga integração não gerada");
                Servicos.Log.TratarErro(ex);

                if (unitOfWork != null)
                    unitOfWork.Rollback();
            }
        }

        public async Task FinalizarGeracaoCargaCTeAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            if (mdfe != null)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repositorioCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(_unitOfWork);
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repositorioDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(_unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork, _cancellationToken);

                List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentos = await repositorioDocumentoMunicipioDescarregamentoMDFe.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken);

                foreach (Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento in documentos)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = await repositorioCTe.BuscarPorChaveAsync(documento.Chave);

                    if (cte != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = await repositorioCargaPedidoDocumentoCTe.BuscarPorCTeAsync(cte.Codigo, _cancellationToken);

                        if (cargaPedidoDocumentoCTe != null && cargaPedidoDocumentoCTe.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                            await FinalizarGeracaoCargaPorMDFeAsync(mdfe, cargaPedidoDocumentoCTe.CargaPedido.Carga);
                    }
                }
            }
        }

        public async Task FinalizarGeracaoCargaPorMDFeAsync(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                return;

            Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro servicoFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork, _cancellationToken);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocalPrestacao = new Carga.CargaLocaisPrestacao(_unitOfWork, _cancellationToken);

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repositorioCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(_unitOfWork, _cancellationToken);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork, _cancellationToken);
            Repositorio.VeiculoMDFe repositorioVeiculoMDFe = new Repositorio.VeiculoMDFe(_unitOfWork, _cancellationToken);
            Repositorio.ReboqueMDFe repositorioReboqueMDFe = new Repositorio.ReboqueMDFe(_unitOfWork, _cancellationToken);
            Repositorio.MotoristaMDFe repositorioMotoristaMDFe = new Repositorio.MotoristaMDFe(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repositorioCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork, _cancellationToken);
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repositorioCargaPedido.BuscarPorCargaAsync(carga.Codigo);

            if (repositorioCargaPedidoDocumentoMDFe.BuscarPorMDFeECargaPedidoAsync(mdfe.Codigo, cargaPedidos[0].Codigo, _cancellationToken) == null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe cargaPedidoDocumentoMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe()
                {
                    CargaPedido = cargaPedidos.FirstOrDefault(),
                    MDFe = mdfe
                };

                await repositorioCargaPedidoDocumentoMDFe.InserirAsync(cargaPedidoDocumentoMDFe);
            }

            Dominio.Entidades.VeiculoMDFe veiculoMDFe = await repositorioVeiculoMDFe.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken);
            List<Dominio.Entidades.ReboqueMDFe> reboquesMDFe = await repositorioReboqueMDFe.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken);
            List<Dominio.Entidades.MotoristaMDFe> motoristasMDFe = await repositorioMotoristaMDFe.BuscarPorMDFeAsync(mdfe.Codigo, _cancellationToken);

            if (veiculoMDFe != null)
            {
                Dominio.Entidades.Veiculo tracao = await repositorioVeiculo.BuscarPorPlacaAsync(veiculoMDFe.Placa, _cancellationToken);

                if (tracao != null)
                {
                    carga.Veiculo = tracao;
                    carga.ModeloVeicularCarga = tracao.ModeloVeicularCarga;
                }
            }

            carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

            foreach (Dominio.Entidades.ReboqueMDFe reboqueMDFe in reboquesMDFe)
            {
                Dominio.Entidades.Veiculo reboque = await repositorioVeiculo.BuscarPorPlacaAsync(reboqueMDFe.Placa, _cancellationToken);

                if (reboque != null)
                {
                    carga.VeiculosVinculados.Add(reboque);

                    if (carga.ModeloVeicularCarga == null)
                        carga.ModeloVeicularCarga = reboque.ModeloVeicularCarga;
                }
            }

            carga.Motoristas = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.MotoristaMDFe motoristaMDFe in motoristasMDFe)
            {
                Dominio.Entidades.Usuario motorista = await repositorioMotorista.BuscarMotoristaPorCPFAsync(motoristaMDFe.CPF, cancellationToken: _cancellationToken);

                if (motorista != null)
                    carga.Motoristas.Add(motorista);
            }

            carga.TipoOperacao = await repositorioTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracaoAsync();
            carga.TipoDeCarga = carga.TipoOperacao?.TipoDeCargaPadraoOperacao;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork).BuscarConfiguracaoPadraoAsync();
            CriarNotasFiscaisDaCarga(cargaPedidos.FirstOrDefault(), _tipoServicoMultisoftware, _unitOfWork, false, null, configuracaoTMS);

            carga.ValorICMS = cargaPedidos.Sum(obj => obj.ValorICMS);
            carga.ValorFrete = cargaPedidos.Sum(obj => obj.ValorFrete);
            carga.ValorFreteAPagar = cargaPedidos.Sum(obj => obj.ValorFreteAPagar);
            carga.ValorFreteLiquido = carga.ValorFrete;
            carga.ValorFreteEmbarcador = cargaPedidos.Sum(obj => obj.ValorFreteAPagar);
            carga.ValorIBSEstadual = cargaPedidos.Sum(obj => obj.ValorIBSEstadual);
            carga.ValorIBSMunicipal = cargaPedidos.Sum(obj => obj.ValorIBSMunicipal);
            carga.ValorCBS = cargaPedidos.Sum(obj => obj.ValorCBS);
            carga.PossuiPendencia = false;

            Servicos.Embarcador.Carga.Ocorrencia.RefazerComplementacaoValorFreteCarga(carga, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware, false);

            if (carga.EmpresaFilialEmissora != null)
                Servicos.Embarcador.Carga.Ocorrencia.RefazerComplementacaoValorFreteCarga(carga, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware, true);

            servicoFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, carga.TipoFreteEscolhido, _unitOfWork, false, _tipoServicoMultisoftware, _unitOfWork.StringConexao);

            await servicoCargaLocalPrestacao.VerificarEAjustarLocaisPrestacaoAsync(carga, cargaPedidos, _unitOfWork, _tipoServicoMultisoftware, configuracaoPedido);
            await servicoCargaDadosSumarizados.AlterarDadosSumarizadosCargaAsync(carga, cargaPedidos, configuracaoTMS, _unitOfWork, _tipoServicoMultisoftware);

            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                Servicos.Log.TratarErro("Atualizou a situação para calculo frete 35 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");

            if (await repositorioCargaComponenteFrete.ContarComponentesInvalidosPorCargaAsync(carga.Codigo, _cancellationToken) <= 0)
            {
                carga.DataInicioEmissaoDocumentos = DateTime.Now;
                carga.DataEnvioUltimaNFe = DateTime.Now;
                carga.DataRecebimentoUltimaNFe = DateTime.Now;
                carga.problemaAverbacaoCTe = false;
                carga.CTesEmDigitacao = false;
                carga.DataInicioGeracaoCTes = DateTime.Now;
            }

            await repositorioCarga.AtualizarAsync(carga);
        }

        public static void GerarCargaDosMDFesDisponiveis(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracao();

            if (tipoOperacao == null)
            {
                Servicos.Log.TratarErro("GerarCargaDosMDFesDisponiveis: Tipo de operação não configurado.");
                return;
            }

            if (tipoOperacao.TipoDeCargaPadraoOperacao == null)
            {
                Servicos.Log.TratarErro("GerarCargaDosMDFesDisponiveis: Tipo de carga padrão não configurado para o tipo de operação não configurado.");
                return;
            }

            IList<Dominio.ObjetosDeValor.Embarcador.MDFe.ConsultaMDFeGeracaoCargaEmbarcador> mdfesDisponiveis = repMDFe.ConsultarMDFesDisponiveisParaGerarCarga(new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMDFesDisponiveisGeracaoCargaEmbarcador() { ProblemaGeracaoCargaAutomaticamente = false, NaoGerarCargaAutomaticamente = false }, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { DirecaoOrdenar = "asc", PropriedadeOrdenar = "DataEmissao", InicioRegistros = 0, LimiteRegistros = 200 });

            foreach (Dominio.ObjetosDeValor.Embarcador.MDFe.ConsultaMDFeGeracaoCargaEmbarcador mdfeDisponivel in mdfesDisponiveis)
            {
                if (!GerarCarga(out string mensagem, new List<int>() { mdfeDisponivel.Codigo }, tipoOperacao.Codigo, tipoOperacao.TipoDeCargaPadraoOperacao.Codigo, 0, null, null, 0, unitOfWork, tipoServicoMultisoftware, null))
                {
                    Servicos.Log.TratarErro("GerarCargaDosMDFesDisponiveis: " + mensagem, "GerarCargaDosMDFesDisponiveis");

                    unitOfWork.FlushAndClear();

                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(mdfeDisponivel.Codigo);

                    unitOfWork.Start();

                    mdfe.ProblemaGeracaoCargaAutomaticamente = true;

                    repMDFe.Atualizar(mdfe);

                    unitOfWork.CommitChanges();
                }
                else
                {
                    unitOfWork.FlushAndClear();
                }
            }
        }

        public static bool GerarCarga(out string mensagem, List<int> codigosMDFes, int codigoTipoOperacao, int codigoTipoCarga, int codigoCentroResultado, DateTime? dataPrevisaoSaida, DateTime? dataPrevisaoEntrega, int codigoVeiculo, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaGeracaoEmbarcador repCargaGeracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaGeracaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes repConfiguracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga();
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido();
            Servicos.Embarcador.CTe.CTEsImportados svcCTesImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
            Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro svcFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados svcCargaDadosSumarizados = new Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao svcCargaLocalPrestacao = new Carga.CargaLocaisPrestacao(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.BuscarPorCodigo(codigosMDFes);
            List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentosMDFe = repDocumentoMunicipioDescarregamentoMDFe.BuscarPorMDFe(codigosMDFes);
            List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();
            Dominio.Entidades.Veiculo tracao = null;
            List<Dominio.Entidades.Veiculo> reboques = new List<Dominio.Entidades.Veiculo>();
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = null;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = codigoTipoCarga > 0 ? repTipoCarga.BuscarPorCodigo(codigoTipoCarga) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = codigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(codigoCentroResultado) : null;
            Dominio.Entidades.Veiculo veiculo = codigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = repConfiguracaoPaletes.BuscarConfiguracaoPadrao();



            if (veiculo != null)
            {
                if (veiculo.TipoVeiculo == "0")
                    tracao = veiculo;
                else
                    tracao = repVeiculo.BuscarTracaoPorReboque(veiculo.Codigo).FirstOrDefault();

                if (tracao != null)
                {
                    reboques = tracao.VeiculosVinculados.ToList();
                    modeloVeicularCarga = tracao.ModeloVeicularCarga;

                    if (modeloVeicularCarga == null)
                        modeloVeicularCarga = reboques.Where(o => o.ModeloVeicularCarga != null).Select(o => o.ModeloVeicularCarga).FirstOrDefault();
                }
            }

            if (centroResultado == null && tipoOperacao != null)
                centroResultado = repCentroResultado.BuscarPorTipoOperacao(tipoOperacao);

            if (tipoOperacao == null)
            {
                mensagem = "É necessário informar um tipo de operação.";
                return false;
            }

            if (tipoCarga == null)
            {
                tipoCarga = tipoOperacao.TipoDeCargaPadraoOperacao;

                if (tipoCarga == null)
                {
                    mensagem = "É necessário informar um tipo de carga ou configurar no tipo de operação um tipo de carga padrão.";
                    return false;
                }
            }

            foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes)
            {
                foreach (Dominio.Entidades.MotoristaMDFe motoristaMDFe in mdfe.Motoristas)
                {
                    string cpf = Utilidades.String.OnlyNumbers(motoristaMDFe.CPF);

                    if (motoristas.Any(o => o.CPF == cpf))
                        continue;

                    Dominio.Entidades.Usuario usuario = repMotorista.BuscarMotoristaPorCPF(cpf);

                    if (usuario == null && !configuracaoTMS.CadastrarMotoristaEVeiculoAutomaticamenteCargaImportada)
                    {
                        mensagem = $"O motorista {motoristaMDFe.CPF} - {motoristaMDFe.Nome} do MDF-e {mdfe.Numero} não está cadastrado.";
                        return false;
                    }
                    else if (usuario == null)
                    {
                        usuario = new Dominio.Entidades.Usuario()
                        {
                            Nome = motoristaMDFe.Nome,
                            CPF = motoristaMDFe.CPF,
                            Tipo = "M",
                            Status = "A",
                            ExibirUsuarioAprovacao = false
                        };

                        repMotorista.Inserir(usuario);
                    }

                    motoristas.Add(usuario);
                }

                if (veiculo == null || tracao == null)
                {
                    foreach (Dominio.Entidades.VeiculoMDFe veiculoMDFe in mdfe.Veiculos)
                    {
                        string placa = veiculoMDFe.Placa;

                        if (tracao != null && tracao.Placa != veiculoMDFe.Placa)
                        {
                            mensagem = "A tração dos MDF-es selecionados é diferente, não sendo possível gerar a carga.";
                            return false;
                        }

                        tracao = repVeiculo.BuscarPlaca(placa);

                        if (tracao == null && !configuracaoTMS.CadastrarMotoristaEVeiculoAutomaticamenteCargaImportada)
                        {
                            mensagem = $"A tração {placa} do MDF-e {mdfe.Numero} não está cadastrada.";
                            return false;
                        }
                        else if (tracao == null)
                        {
                            tracao = new Dominio.Entidades.Veiculo()
                            {
                                Placa = veiculoMDFe.Placa,
                                TipoVeiculo = "0",
                                Estado = veiculoMDFe.UF
                            };
                            repVeiculo.Inserir(tracao);
                        }

                        if (modeloVeicularCarga == null && tracao.ModeloVeicularCarga != null)
                            modeloVeicularCarga = tracao.ModeloVeicularCarga;
                    }

                    foreach (Dominio.Entidades.ReboqueMDFe reboqueMDFe in mdfe.Reboques)
                    {
                        string placa = reboqueMDFe.Placa;

                        if (reboques.Any(o => o.Placa == placa))
                            continue;

                        Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorPlaca(placa);

                        if (reboque == null && !configuracaoTMS.CadastrarMotoristaEVeiculoAutomaticamenteCargaImportada)
                        {
                            mensagem = $"O reboque {placa} do MDF-e {mdfe.Numero} não está cadastrado.";
                            return false;
                        }
                        else if (reboque == null)
                        {
                            reboque = new Dominio.Entidades.Veiculo()
                            {
                                Placa = reboqueMDFe.Placa,
                                TipoVeiculo = "1",
                                Estado = reboqueMDFe.UF
                            };
                            repVeiculo.Inserir(reboque);
                        }

                        if (modeloVeicularCarga == null)
                            modeloVeicularCarga = reboque.ModeloVeicularCarga;

                        reboques.Add(reboque);
                    }
                }
            }

            foreach (Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documentoMDFe in documentosMDFe)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = documentoMDFe.CTe;

                if (cte == null)
                    cte = repCTe.BuscarPorChave(documentoMDFe.Chave);

                if (cte == null)
                {
                    mensagem = $"O CT-e {documentoMDFe.Chave} do MDF-e {documentoMDFe.MunicipioDescarregamento.MDFe.Numero} não está disponível na base de dados.";
                    return false;
                }

                if (cte.Status != "A")
                {
                    mensagem = $"O CT-e {cte.Chave} não está autorizado, impossibilitando a geração da carga.";
                    return false;
                }

                ctes.Add(cte);
            }

            if (ctes.Count == 0)
            {
                mensagem = "Nenhum CT-e encontrado para os MDF-es selecionados.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> cargaMDFes = repCargaMDFe.BuscarPorChavesCTes(ctes.Select(o => o.Chave));

            if (cargaMDFes.Count() > 0)
            {
                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cargaMDFes.Select(o => o.Carga.Codigo).Distinct().Count() > 1) //se existe mais de uma carga para os MDF-es encontrados não gera/vincula a carga
                {
                    mensagem = $"Os CT-es deste MDF-e estão vinculados à outros MDF-es nas cargas {string.Join(", ", cargaMDFes.Select(o => o.Carga.CodigoCargaEmbarcador).Distinct())}, não sendo possível gerar uma nova carga.";
                    return false;
                }

                Dominio.Entidades.Embarcador.Cargas.Carga cargaExistente = cargaMDFes.Select(o => o.Carga).FirstOrDefault();

                if (ctes.All(o => cargaExistente.CargaCTes.Any(cc => cc.CTe.Chave == o.Chave))) //se todos os CT-es do MDF-e estão na carga
                {
                    unitOfWork.Start();

                    foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFe()
                        {
                            Carga = cargaExistente,
                            MDFe = mdfe,
                            SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.OutrosEmissores
                        };

                        repCargaMDFe.Inserir(cargaMDFe, auditado);
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador cargaGeracaoEmbarcadorExistente = new Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador()
                    {
                        MDFes = mdfes,
                        CTes = ctes,
                        Motoristas = motoristas,
                        Tracao = tracao,
                        Reboques = reboques,
                        TipoCarga = tipoCarga,
                        TipoOperacao = tipoOperacao,
                        Carga = cargaExistente,
                        DataCriacao = DateTime.Now
                    };

                    repCargaGeracaoEmbarcador.Inserir(cargaGeracaoEmbarcadorExistente, auditado);

                    unitOfWork.CommitChanges();

                    mensagem = string.Empty;
                    return true;
                }
                else if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    mensagem = $"Apenas alguns CT-es destes MDF-es estão vinculados à carga {cargaExistente.CodigoCargaEmbarcador}. É necessário que todos os CT-es estejam em uma única carga, não sendo possível gerar duas cargas com o mesmo CT-e.";
                    return false;
                }
            }

            if (repCargaCTe.ExisteAutorizadoPorCTe(ctes.Select(o => o.Codigo)) ||
                repCargaPedidoDocumentoCTe.ExistePorCTe(ctes.Select(o => o.Codigo)))
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesEmCarga = repCargaCTe.RetornarCTesExisteAutorizadoPorCTe(ctes.Select(o => o.Codigo));
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesEmDocumentoCTe = repCargaPedidoDocumentoCTe.RetornarCTesExistePorCTe(ctes.Select(o => o.Codigo));

                    if (ctesEmCarga != null && ctesEmCarga.Count > 0 && ctes != null && ctes.Count > 0)
                        ctes = ctes.Except(ctesEmCarga).ToList();
                    if (ctesEmDocumentoCTe != null && ctesEmDocumentoCTe.Count > 0 && ctes != null && ctes.Count > 0)
                        ctes = ctes.Except(ctesEmDocumentoCTe).ToList();

                    if (ctes == null || ctes.Count == 0)
                    {
                        mensagem = "O(s) CT-e(s) deste(s) MDF-e(s) já está(ão) vinculado(s) à uma carga. Todos já geraram cargas.";
                        return false;
                    }
                }
                else
                {
                    mensagem = "O(s) CT-e(s) deste(s) MDF-e(s) já está(ão) vinculado(s) à uma carga.";
                    return false;
                }
            }

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = ConverterCTesEmCargaIntegracao(tipoOperacao, ctes, unitOfWork, tipoServicoMultisoftware);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao ?? "");

            unitOfWork.Start();

            System.Text.StringBuilder mensagemGeracaoCarga = new StringBuilder();

            int codigoCargaExistente = 0;
            int protocoloPedidoExistente = 0;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref mensagemGeracaoCarga, tipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, true, auditado, configuracaoTMS);

            if (mensagemGeracaoCarga.Length == 0 || protocoloPedidoExistente > 0)
            {
                pedido.CentroResultado = centroResultado;
                pedido.DataPrevisaoSaida = dataPrevisaoSaida;
                pedido.PrevisaoEntrega = dataPrevisaoEntrega;

                repPedido.Atualizar(pedido);

                serProdutoPedidoWS.AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref mensagemGeracaoCarga, unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = serCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemGeracaoCarga, ref codigoCargaExistente, unitOfWork, tipoServicoMultisoftware, false, true, auditado, configuracaoTMS, null, "", filial, tipoOperacao);

                if (cargaPedido != null)
                {
                    carga = cargaPedido.Carga;

                    serCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemGeracaoCarga, unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

                    if (cargaIntegracao.FecharCargaAutomaticamente)
                        carga.CargaFechada = true;

                    carga.NumeroSequenciaCarga = int.Parse(cargaIntegracao.NumeroCarga);
                    carga.Empresa = mdfes[0].Empresa;
                    carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                    carga.TipoOperacao = tipoOperacao;
                    carga.TipoDeCarga = tipoCarga;
                    carga.Veiculo = tracao;
                    carga.ModeloVeicularCarga = modeloVeicularCarga;
                    carga.VeiculosVinculados = reboques;
                    carga.Motoristas = motoristas;

                    cargaPedido.CTeEmitidoNoEmbarcador = true;
                    //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                    Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {cargaIntegracao.PesoBruto}. CTEsImportados.GerarCarga", "PesoCargaPedido");
                    cargaPedido.Peso = cargaIntegracao.PesoBruto;
                    cargaPedido.PesoLiquido = cargaIntegracao.PesoLiquido;

                    if (carga.Veiculo?.Tipo == "T")
                        carga.FreteDeTerceiro = true;
                    else if (carga.ProvedorOS != null)
                        carga.FreteDeTerceiro = true;

                    repCargaPedido.Atualizar(cargaPedido);
                    repCarga.Atualizar(carga);

                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                        {
                            CargaPedido = cargaPedido,
                            CTe = cte
                        };

                        repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);

                        cte.CTeSemCarga = false;

                        repCTe.Atualizar(cte);
                    }

                    foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe cargaPedidoDocumentoMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe()
                        {
                            CargaPedido = cargaPedido,
                            MDFe = mdfe
                        };

                        repCargaPedidoDocumentoMDFe.Inserir(cargaPedidoDocumentoMDFe);

                        mdfe.MDFeSemCarga = false;

                        repMDFe.Atualizar(mdfe);
                    }

                    mensagem = svcCTesImportados.CriarNotasFiscaisDaCarga(cargaPedido, tipoServicoMultisoftware, unitOfWork, true, null, configuracaoTMS);

                    if (!string.IsNullOrWhiteSpace(mensagem))
                    {
                        unitOfWork.Rollback();
                        return false;
                    }

                    carga.ValorICMS = cargaPedido.ValorICMS;
                    carga.ValorFrete = cargaPedido.ValorFrete;
                    carga.ValorFreteAPagar = cargaPedido.ValorFreteAPagar;
                    carga.ValorFreteLiquido = cargaPedido.ValorFrete;
                    carga.ValorFreteEmbarcador = cargaPedido.ValorFreteAPagar;
                    carga.PossuiPendencia = false;

                    Servicos.Embarcador.Carga.Ocorrencia.RefazerComplementacaoValorFreteCarga(carga, unitOfWork, "", tipoServicoMultisoftware, false);

                    if (carga.EmpresaFilialEmissora != null)
                        Servicos.Embarcador.Carga.Ocorrencia.RefazerComplementacaoValorFreteCarga(carga, unitOfWork, "", tipoServicoMultisoftware, true);

                    svcCargaLocalPrestacao.VerificarEAjustarLocaisPrestacao(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
                    Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                    svcCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);

                    svcFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, carga.TipoFreteEscolhido, unitOfWork, false, tipoServicoMultisoftware, "");

                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 34 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");

                    if (!Servicos.Embarcador.Carga.Carga.PreencherDadosPalletsDoPedido(out mensagem, carga, cargaPedido, tipoOperacao, unitOfWork, configuracaoPaletes))
                    {
                        unitOfWork.Rollback();
                        return false;
                    }

                    if (repCargaComponenteFrete.ContarComponentesInvalidosPorCarga(carga.Codigo) > 0)
                    {
                        unitOfWork.Rollback();
                        mensagem = "Existem componentes de frete nos CT-es que não estão configurados corretamente para a geração da carga.";
                        return false;
                    }

                    Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);
                    if (configuracaoTMS.BloquearEmissaoComContratoFreteZerado && carga.FreteDeTerceiro && (contratoFrete?.ValorFreteSubcontratacao ?? 0m) <= 0m)
                    {
                        unitOfWork.Rollback();
                        mensagem = "De acordo com as configurações, o valor do contrato do terceiro deve ser maior que zero, não sendo possível gerar a carga.";
                        return false;
                    }

                    mensagem = Servicos.Embarcador.CIOT.CIOT.ObterCIOTCarga(carga, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);

                    if (!string.IsNullOrWhiteSpace(mensagem))
                    {
                        unitOfWork.Rollback();
                        return false;
                    }

                    carga.DataInicioEmissaoDocumentos = DateTime.Now;
                    carga.DataEnvioUltimaNFe = DateTime.Now;
                    carga.DataRecebimentoUltimaNFe = DateTime.Now;
                    carga.problemaAverbacaoCTe = false;
                    carga.CTesEmDigitacao = false;
                    carga.DataInicioGeracaoCTes = DateTime.Now;

                    repCarga.Atualizar(carga);
                }
            }
            else
            {
                unitOfWork.Rollback();
                mensagem = mensagemGeracaoCarga.ToString();
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador cargaGeracaoEmbarcador = new Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador()
            {
                MDFes = mdfes,
                CTes = ctes,
                Motoristas = motoristas,
                Tracao = tracao,
                Reboques = reboques,
                TipoCarga = tipoCarga,
                TipoOperacao = tipoOperacao,
                Carga = carga,
                DataCriacao = DateTime.Now
            };

            repCargaGeracaoEmbarcador.Inserir(cargaGeracaoEmbarcador, auditado);

            unitOfWork.CommitChanges();

            mensagem = "";
            return true;
        }

        public static bool GerarCargaCTes(out string mensagem, List<int> codigosCTes, int codigoTipoOperacao, int codigoTipoCarga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipioDescarregamentoMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaGeracaoEmbarcador repCargaGeracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaGeracaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);


            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga();
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido();
            Servicos.Embarcador.CTe.CTEsImportados svcCTesImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
            Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro svcFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados svcCargaDadosSumarizados = new Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao svcCargaLocalPrestacao = new Carga.CargaLocaisPrestacao(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(codigosCTes);
            List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();
            Dominio.Entidades.Veiculo tracao = null;
            List<Dominio.Entidades.Veiculo> reboques = new List<Dominio.Entidades.Veiculo>();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = codigoTipoCarga > 0 ? repTipoCarga.BuscarPorCodigo(codigoTipoCarga) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = null;

            if (tipoOperacao == null)
            {
                mensagem = "É necessário informar um tipo de operação.";
                return false;
            }

            if (tipoCarga == null)
            {
                tipoCarga = tipoOperacao.TipoDeCargaPadraoOperacao;

                if (tipoCarga == null)
                {
                    mensagem = "É necessário informar um tipo de carga ou configurar no tipo de operação um tipo de carga padrão.";
                    return false;
                }
            }

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                foreach (Dominio.Entidades.MotoristaCTE motoristaCTE in cte.Motoristas)
                {
                    string cpf = Utilidades.String.OnlyNumbers(motoristaCTE.CPFMotorista);

                    if (motoristas.Any(o => o.CPF == cpf))
                        continue;

                    Dominio.Entidades.Usuario usuario = repMotorista.BuscarMotoristaPorCPF(cpf);

                    if (usuario == null && !configuracaoTMS.CadastrarMotoristaEVeiculoAutomaticamenteCargaImportada)
                    {
                        mensagem = $"O motorista {motoristaCTE.CPFMotorista} - {motoristaCTE.NomeMotoristaCTe} do CTe-e {cte.Numero} não está cadastrado.";
                        return false;
                    }
                    else if (usuario == null)
                    {
                        usuario = new Dominio.Entidades.Usuario()
                        {
                            Nome = motoristaCTE.NomeMotorista,
                            CPF = motoristaCTE.CPFMotorista,
                            Tipo = "M",
                            Status = "A",
                            ExibirUsuarioAprovacao = false
                        };
                        repMotorista.Inserir(usuario);
                    }

                    motoristas.Add(usuario);
                }

                foreach (Dominio.Entidades.VeiculoCTE veiculoCTe in cte.Veiculos)
                {
                    if (tracao == null && veiculoCTe.Veiculo != null && veiculoCTe.Veiculo.TipoVeiculo == "0")
                        tracao = veiculoCTe.Veiculo;

                    if (veiculoCTe.Veiculo != null && veiculoCTe.Veiculo.TipoVeiculo == "1")
                    {
                        string placa = veiculoCTe.Veiculo.Placa;

                        if (reboques.Any(o => o.Placa == placa))
                            continue;

                        if (modeloVeicularCarga == null)
                            modeloVeicularCarga = veiculoCTe.Veiculo.ModeloVeicularCarga;

                        reboques.Add(veiculoCTe.Veiculo);
                    }
                }
            }

            if (modeloVeicularCarga == null && tracao != null)
                modeloVeicularCarga = tracao.ModeloVeicularCarga;

            if (ctes.Count() == 0)
            {
                mensagem = "Nenhum CT-e encontrado para os MDF-es selecionados.";
                return false;
            }

            if (repCargaCTe.ExisteAutorizadoPorCTe(ctes.Select(o => o.Codigo)) ||
                repCargaPedidoDocumentoCTe.ExistePorCTe(ctes.Select(o => o.Codigo)))
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesEmCarga = repCargaCTe.RetornarCTesExisteAutorizadoPorCTe(ctes.Select(o => o.Codigo));
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesEmDocumentoCTe = repCargaPedidoDocumentoCTe.RetornarCTesExistePorCTe(ctes.Select(o => o.Codigo));

                    if (ctesEmCarga != null && ctesEmCarga.Count > 0 && ctes != null && ctes.Count > 0)
                        ctes = ctes.Except(ctesEmCarga).ToList();
                    if (ctesEmDocumentoCTe != null && ctesEmDocumentoCTe.Count > 0 && ctes != null && ctes.Count > 0)
                        ctes = ctes.Except(ctesEmDocumentoCTe).ToList();

                    if (ctes == null || ctes.Count == 0)
                    {
                        mensagem = "O(s) CT-e(s) deste(s) MDF-e(s) já está(ão) vinculado(s) à uma carga. Todos já geraram cargas.";
                        return false;
                    }
                }
                else
                {
                    mensagem = "O(s) CT-e(s) deste(s) MDF-e(s) já está(ão) vinculado(s) à uma carga.";
                    return false;
                }
            }

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = ConverterCTesEmCargaIntegracao(tipoOperacao, ctes, unitOfWork, tipoServicoMultisoftware);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao ?? "");

            unitOfWork.Start();

            System.Text.StringBuilder mensagemGeracaoCarga = new StringBuilder();

            int codigoCargaExistente = 0;
            int protocoloPedidoExistente = 0;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref mensagemGeracaoCarga, tipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, true, auditado, configuracaoTMS);

            if (mensagemGeracaoCarga.Length == 0 || protocoloPedidoExistente > 0)
            {
                serProdutoPedidoWS.AdicionarProdutosPedido(pedido, configuracaoTMS, cargaIntegracao, ref mensagemGeracaoCarga, unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = serCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemGeracaoCarga, ref codigoCargaExistente, unitOfWork, tipoServicoMultisoftware, false, true, auditado, configuracaoTMS, null, "", filial, tipoOperacao);

                if (cargaPedido != null)
                {
                    carga = cargaPedido.Carga;

                    serCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemGeracaoCarga, unitOfWork, configuracaoTMS.UsarPesoProdutoSumarizacaoCarga);

                    if (cargaIntegracao.FecharCargaAutomaticamente)
                        carga.CargaFechada = true;

                    carga.NumeroSequenciaCarga = int.Parse(cargaIntegracao.NumeroCarga);
                    carga.Empresa = ctes[0].Empresa;
                    carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                    carga.TipoOperacao = tipoOperacao;
                    carga.TipoDeCarga = tipoCarga;
                    carga.Veiculo = tracao;
                    carga.ModeloVeicularCarga = modeloVeicularCarga;
                    carga.VeiculosVinculados = reboques;
                    carga.Motoristas = motoristas;

                    cargaPedido.CTeEmitidoNoEmbarcador = true;
                    //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                    Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {cargaIntegracao.PesoBruto}. CTEsImportados.GerarCargaCTes", "PesoCargaPedido");
                    cargaPedido.Peso = cargaIntegracao.PesoBruto;
                    cargaPedido.PesoLiquido = cargaIntegracao.PesoLiquido;

                    repCargaPedido.Atualizar(cargaPedido);
                    repCarga.Atualizar(carga);

                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                        {
                            CargaPedido = cargaPedido,
                            CTe = cte
                        };

                        repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);

                        cte.CTeSemCarga = false;

                        repCTe.Atualizar(cte);
                    }

                    mensagem = svcCTesImportados.CriarNotasFiscaisDaCarga(cargaPedido, tipoServicoMultisoftware, unitOfWork, true);

                    if (!string.IsNullOrWhiteSpace(mensagem))
                    {
                        unitOfWork.Rollback();
                        return false;
                    }

                    carga.ValorICMS = cargaPedido.ValorICMS;
                    carga.ValorFrete = cargaPedido.ValorFrete;
                    carga.ValorFreteAPagar = cargaPedido.ValorFreteAPagar;
                    carga.ValorFreteLiquido = cargaPedido.ValorFrete;
                    carga.ValorFreteEmbarcador = cargaPedido.ValorFreteAPagar;
                    carga.PossuiPendencia = false;

                    Servicos.Embarcador.Carga.Ocorrencia.RefazerComplementacaoValorFreteCarga(carga, unitOfWork, "", tipoServicoMultisoftware, false);

                    if (carga.EmpresaFilialEmissora != null)
                        Servicos.Embarcador.Carga.Ocorrencia.RefazerComplementacaoValorFreteCarga(carga, unitOfWork, "", tipoServicoMultisoftware, true);

                    svcCargaLocalPrestacao.VerificarEAjustarLocaisPrestacao(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
                    Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                    svcCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);

                    svcFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, carga.TipoFreteEscolhido, unitOfWork, false, tipoServicoMultisoftware, "");

                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 33 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");

                    Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);

                    if (repCargaComponenteFrete.ContarComponentesInvalidosPorCarga(carga.Codigo) > 0)
                    {
                        unitOfWork.Rollback();
                        mensagem = "Existem componentes de frete nos CT-es que não estão configurados corretamente para a geração da carga.";
                        return false;
                    }

                    if (configuracaoTMS.BloquearEmissaoComContratoFreteZerado && carga.FreteDeTerceiro && (contratoFrete?.ValorFreteSubcontratacao ?? 0m) <= 0m)
                    {
                        unitOfWork.Rollback();
                        mensagem = "De acordo com as configurações, o valor do contrato do terceiro deve ser maior que zero, não sendo possível gerar a carga.";
                        return false;
                    }

                    mensagem = Servicos.Embarcador.CIOT.CIOT.ObterCIOTCarga(carga, configuracaoTMS, tipoServicoMultisoftware, unitOfWork);

                    if (!string.IsNullOrWhiteSpace(mensagem))
                    {
                        unitOfWork.Rollback();
                        return false;
                    }

                    carga.DataInicioEmissaoDocumentos = DateTime.Now;
                    carga.DataEnvioUltimaNFe = DateTime.Now;
                    carga.DataRecebimentoUltimaNFe = DateTime.Now;
                    carga.problemaAverbacaoCTe = false;
                    carga.CTesEmDigitacao = false;
                    carga.DataInicioGeracaoCTes = DateTime.Now;

                    repCarga.Atualizar(carga);
                }
            }
            else
            {
                unitOfWork.Rollback();
                mensagem = mensagemGeracaoCarga.ToString();
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador cargaGeracaoEmbarcador = new Dominio.Entidades.Embarcador.Cargas.CargaGeracaoEmbarcador()
            {
                CTes = ctes,
                Motoristas = motoristas,
                Tracao = tracao,
                Reboques = reboques,
                TipoCarga = tipoCarga,
                TipoOperacao = tipoOperacao,
                Carga = carga,
                DataCriacao = DateTime.Now
            };

            repCargaGeracaoEmbarcador.Inserir(cargaGeracaoEmbarcador, auditado);

            unitOfWork.CommitChanges();

            mensagem = "";
            return true;
        }

        public async Task<bool> ProcessarXMLMDFeAsync(System.IO.Stream xml, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            try
            {
                xml.Position = 0;
                var mdfeLido = MultiSoftware.MDFe.Servicos.Leitura.Ler(xml);

                if (mdfeLido != null)
                {
                    Servicos.MDFe servicoMDFe = new Servicos.MDFe(_unitOfWork, _cancellationToken);

                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repositorioMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(_unitOfWork, _cancellationToken);
                    Servicos.Embarcador.MDFe.MDFeImportado servicoMDFeImportado = new MDFe.MDFeImportado(_unitOfWork, _cancellationToken);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();

                    if (mdfeLido.GetType() == typeof(MultiSoftware.MDFe.v100a.mdfeProc))
                    {
                        MultiSoftware.MDFe.v100a.mdfeProc mdfeProc = (MultiSoftware.MDFe.v100a.mdfeProc)mdfeLido;
                        Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe mdfe = Servicos.Embarcador.MDFe.MDFe.ConverterProcMDFeParaMDFe(mdfeProc);

                        Repositorio.Empresa repositoioEmpresa = new Repositorio.Empresa(_unitOfWork, _cancellationToken);

                        Dominio.Entidades.Empresa empresa = await repositoioEmpresa.BuscarPorCNPJAsync(mdfe.Emitente.CNPJ);

                        if (empresa != null)
                        {
                            object mdfeRetorno = await servicoMDFe.GerarMDFeAnteriorAsync(empresa, mdfeProc, xml, false);

                            if (mdfeRetorno.GetType() == typeof(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais))
                            {
                                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeConvertido = (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais)mdfeRetorno;

                                await _unitOfWork.StartAsync();


                                if (configuracaoTMS.GerarCargaDeCTesRecebidosPorEmail || configuracaoTMS.GerarCargaMDFeDestinado)
                                {
                                    await FinalizarGeracaoCargaCTeAsync(mdfeConvertido);

                                    await _unitOfWork.CommitChangesAsync();
                                }
                                else
                                {
                                    if (await servicoMDFeImportado.DestinarMDFeImportadoParaSeuDestinoAsync(mdfeConvertido, auditado, configuracaoTMS))
                                        await _unitOfWork.CommitChangesAsync();
                                    else
                                        await _unitOfWork.RollbackAsync();
                                }
                            }
                        }
                    }
                    else if (mdfeLido.GetType() == typeof(MultiSoftware.MDFe.v300.mdfeProc))
                    {
                        MultiSoftware.MDFe.v300.mdfeProc mdfeProc = (MultiSoftware.MDFe.v300.mdfeProc)mdfeLido;
                        Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe mdfe = Servicos.Embarcador.MDFe.MDFe.ConverterProcMDFeParaMDFe(mdfeProc);

                        Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork, _cancellationToken);

                        Dominio.Entidades.Empresa empresa = await repositorioEmpresa.BuscarPorCNPJAsync(mdfe.Emitente.CNPJ);

                        if (empresa != null)
                        {
                            object mdfeRetorno = await servicoMDFe.GerarMDFeAnteriorAsync(empresa, mdfeProc, xml, false);

                            if (mdfeRetorno.GetType() == typeof(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais) || mdfeRetorno.GetType().BaseType == typeof(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais))
                            {
                                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeConvertido = (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais)mdfeRetorno;

                                await _unitOfWork.StartAsync();

                                if (configuracaoTMS.GerarCargaDeCTesRecebidosPorEmail)
                                {
                                    await FinalizarGeracaoCargaCTeAsync(mdfeConvertido);

                                    await _unitOfWork.CommitChangesAsync();
                                }
                                else
                                {
                                    if (await servicoMDFeImportado.DestinarMDFeImportadoParaSeuDestinoAsync(mdfeConvertido, auditado, configuracaoTMS))
                                        await _unitOfWork.CommitChangesAsync();
                                    else
                                        await _unitOfWork.RollbackAsync();
                                }
                            }
                        }
                    }
                    else if (mdfeLido.GetType() == typeof(MultiSoftware.MDFe.v300.TProcEvento))
                    {
                        MultiSoftware.MDFe.v300.TProcEvento procEvento = (MultiSoftware.MDFe.v300.TProcEvento)mdfeLido;

                        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeIntegrado = await repositorioMDFe.BuscarPorChaveAsync(procEvento.retEventoMDFe.infEvento.chMDFe);

                        if (mdfeIntegrado != null && procEvento.retEventoMDFe.infEvento.cStat == "135")
                        {
                            if (procEvento.retEventoMDFe.infEvento.tpEvento == "110111")
                                await servicoMDFe.CancelarMDFeImportadoAsync(mdfeIntegrado, procEvento, xml, null, auditado);
                            else if (procEvento.retEventoMDFe.infEvento.tpEvento == "110112")
                                await servicoMDFe.EncerrarMDFeImportadoAsync(mdfeIntegrado, procEvento, xml, null, auditado);
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public static void GerarCargaDosCTesDisponiveis(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Log.TratarErro("Inicio.", "GerarCargaDosCTesDisponiveis");
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracao();

            if (tipoOperacao == null)
            {
                Servicos.Log.TratarErro("GerarCargaDosCTesDisponiveis: Tipo de operação não configurado.");
                return;
            }

            if (tipoOperacao.TipoDeCargaPadraoOperacao == null)
            {
                Servicos.Log.TratarErro("GerarCargaDosCTesDisponiveis: Tipo de carga padrão não configurado para o tipo de operação não configurado.");
                return;
            }
            Servicos.Log.TratarErro("Consultanto CTes.", "GerarCargaDosCTesDisponiveis");
            IList<Dominio.ObjetosDeValor.Embarcador.CTe.ConsultaCTeGeracaoCargaEmbarcador> ctesDisponiveis = repCTe.ConsultarCTesDisponiveisParaGerarCarga(new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { DirecaoOrdenar = "asc", PropriedadeOrdenar = "DataEmissao", InicioRegistros = 0, LimiteRegistros = 5000 });
            List<DateTime> datasCTes = new List<DateTime>();
            List<int> codigosVeiculso = new List<int>();
            if (ctesDisponiveis != null && ctesDisponiveis.Count > 0)
            {
                codigosVeiculso = ctesDisponiveis.Select(c => c.CodigoVeiculo).Distinct().ToList();
                datasCTes = ctesDisponiveis.Select(c => c.DataEmissao.Date).Distinct().ToList();
            }
            foreach (var dataEmissao in datasCTes)
            {
                foreach (var codigoVeiculo in codigosVeiculso)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.CTe.ConsultaCTeGeracaoCargaEmbarcador> ctesParaGeracao = ctesDisponiveis.Where(c => c.CodigoVeiculo == codigoVeiculo && c.DataEmissao.Date == dataEmissao.Date).ToList();
                    if (ctesParaGeracao != null && ctesParaGeracao.Count > 0)
                    {
                        if (!GerarCargaCTes(out string mensagem, ctesParaGeracao.Select(c => c.Codigo).ToList(), tipoOperacao.Codigo, tipoOperacao.TipoDeCargaPadraoOperacao.Codigo, unitOfWork, tipoServicoMultisoftware, null))
                        {
                            Servicos.Log.TratarErro("GerarCargaDosCTesDisponiveis: " + mensagem, "GerarCargaDosCTesDisponiveis");

                            unitOfWork.FlushAndClear();

                            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(ctesParaGeracao.Select(c => c.Codigo).ToList());

                            unitOfWork.Start();
                            foreach (var cte in ctes)
                            {
                                cte.ProblemaGeracaoCargaAutomaticamente = true;

                                repCTe.Atualizar(cte);
                            }

                            unitOfWork.CommitChanges();
                        }
                        else
                        {
                            unitOfWork.FlushAndClear();
                        }
                    }
                }
            }
            Servicos.Log.TratarErro("Fim.", "GerarCargaDosCTesDisponiveis");
        }

        #endregion

        #region Métodos Privados

        private void DestinarCTeAnulacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao || string.IsNullOrWhiteSpace(cte.ChaveCTESubComp) || cte.ChaveCTESubComp.Length != 44)
                return;

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            if (repCargaCTe.ExistePorCTe(cte.Codigo))
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorChaveCTeComCargaAtiva(cte.ChaveCTESubComp);

            if (cargaCTe == null || cargaCTe.CTe.Status != "A")
                return;

            //anula o ct-e original
            cargaCTe.CTe.Status = "Z";
            cargaCTe.CTe.DataAnulacao = cargaCTe.CTe.DataAutorizacao;

            repCTe.Atualizar(cargaCTe.CTe);

            cte.CentroResultadoFaturamento = cargaCTe.CTe.CentroResultadoFaturamento;
            cte.PossuiPedidoSubstituicao = cargaCTe.CTe.PossuiPedidoSubstituicao;

            repCTe.Atualizar(cte);

            //vincula o ct-e de anulação à carga do CT-e original
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeAnulacao = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
            {
                Carga = cargaCTe.Carga,
                CargaOrigem = cargaCTe.CargaOrigem,
                CTe = cte,
                DataVinculoCarga = DateTime.Now,
                GerouCanhoto = true,
                GerouControleFaturamento = true,
                GerouMovimentacaoAutorizacao = true,
                GerouTituloAutorizacao = true,
                GerouMovimentacaoCancelamento = true,
                SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.OutrosEmissores
            };

            repCargaCTe.Inserir(cargaCTeAnulacao);

            //reverte os títulos e gera as movimentações de anulação do CT-e original
            if (!Servicos.Embarcador.Carga.Cancelamento.GerarMovimentoCancelamentoCTe(out string erro, cargaCTe, tipoServicoMultisoftware, unitOfWork, "", false, true) ||
                !Servicos.Embarcador.Carga.Cancelamento.ReverterItensEmAbertoAposCancelamentoCTe(out erro, cargaCTe, tipoServicoMultisoftware, unitOfWork))
            {
                unitOfWork.Rollback();
                throw new Exception("Falha no método DestinarCTeAnulacao: " + erro);
            }

            Servicos.Embarcador.Escrituracao.DocumentoEscrituracao.AdicionarDocumentoParaEscrituracao(cargaCTeAnulacao, unitOfWork);
        }

        private void DestinarCTeSubstituicao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Substituto || string.IsNullOrWhiteSpace(cte.ChaveCTESubComp) || cte.ChaveCTESubComp.Length != 44)
                return;

            Servicos.Embarcador.Carga.Documentos svcDocumentos = new Carga.Documentos(unitOfWork);
            Servicos.Embarcador.Carga.RateioCTe serRateioCTe = new Servicos.Embarcador.Carga.RateioCTe(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            if (repCargaCTe.ExistePorCTe(cte.Codigo))
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorChaveCTeComCargaAtiva(cte.ChaveCTESubComp);

            if (cargaCTe == null)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotaFiscalCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarTodosCargaPedidoXMLNotaFiscalCTePorCargaCTe(cargaCTe.Codigo);

            cte.CentroResultadoFaturamento = cargaCTe.CTe.CentroResultadoFaturamento;
            cte.PossuiPedidoSubstituicao = cargaCTe.CTe.PossuiPedidoSubstituicao;

            //vincula o ct-e de anulação à carga do CT-e original
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeSubstituicao = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
            {
                Carga = cargaCTe.Carga,
                CargaOrigem = cargaCTe.CargaOrigem,
                CTe = cte,
                DataVinculoCarga = DateTime.Now,
                GerouCanhoto = true,
                GerouControleFaturamento = true,
                GerouMovimentacaoAutorizacao = true,
                GerouTituloAutorizacao = true,
                SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.OutrosEmissores
            };

            repCargaCTe.Inserir(cargaCTeSubstituicao);

            cte.XMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTeOriginal in cargaPedidoXMLNotaFiscalCTes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTeSubstituicao = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe()
                {
                    CargaCTe = cargaCTeSubstituicao,
                    PedidoXMLNotaFiscal = cargaPedidoXMLNotaFiscalCTeOriginal.PedidoXMLNotaFiscal
                };

                repCargaPedidoXMLNotaFiscalCTe.Inserir(cargaPedidoXMLNotaFiscalCTeSubstituicao);

                cte.XMLNotaFiscais.Add(cargaPedidoXMLNotaFiscalCTeOriginal.PedidoXMLNotaFiscal.XMLNotaFiscal);
            }

            repCTe.Atualizar(cte);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCargaAutorizadosSemAnulacao(cargaCTeSubstituicao.Carga.Codigo);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCTeSubstituicao.Carga.Codigo);

            svcDocumentos.GerarMovimentoEmissaoCTe(cargaCTeSubstituicao, tipoServicoMultisoftware, unitOfWork, false);

            Servicos.Embarcador.Escrituracao.DocumentoEscrituracao.AdicionarDocumentoParaEscrituracao(cargaCTeSubstituicao, unitOfWork);

            Servicos.Log.GravarInfo($"DestinarCTeSubstituicao inserindo documento faturamento - Carga {cargaCTeSubstituicao?.CargaOrigem?.Codigo ?? 0} - CTe {cargaCTeSubstituicao?.CTe?.Codigo ?? 0}", "ControleDocumentoFaturamento");
            Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarControleFaturamentoPorDocumento(cargaCTeSubstituicao.CargaOrigem, cargaCTeSubstituicao.CTe, null, null, null, null, false, false, false, configuracao, unitOfWork, tipoServicoMultisoftware);

            serRateioCTe.AjustarFretePorCTes(cargaPedido, cargaCTes, tipoServicoMultisoftware, unitOfWork);
        }

        private static Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao ConverterCTesEmCargaIntegracao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa();
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[0];

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();

            cargaIntegracao.NumeroCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();
            cargaIntegracao.CFOP = cte.CFOP.CodigoCFOP;
            cargaIntegracao.TransportadoraEmitente = serEmpresa.ConverterObjetoEmpresa(cte.Empresa);
            cargaIntegracao.UsarOutroEnderecoOrigem = false;
            cargaIntegracao.UsarOutroEnderecoDestino = false;
            cargaIntegracao.Remetente = serPessoa.ConverterObjetoParticipamenteCTe(cte.Remetente);
            cargaIntegracao.Destinatario = serPessoa.ConverterObjetoParticipamenteCTe(cte.Destinatario);
            cargaIntegracao.Tomador = serPessoa.ConverterObjetoParticipamenteCTe(cte.Tomador);
            cargaIntegracao.Recebedor = serPessoa.ConverterObjetoParticipamenteCTe(cte.Recebedor);
            cargaIntegracao.Expedidor = serPessoa.ConverterObjetoParticipamenteCTe(cte.Expedidor);
            cargaIntegracao.DataCriacaoCarga = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            cargaIntegracao.DataInicioCarregamento = cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss");
            cargaIntegracao.DataFinalCarregamento = cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss");
            cargaIntegracao.NumeroPaletes = 0;
            cargaIntegracao.PesoTotalPaletes = 0;
            cargaIntegracao.ValorTotalPaletes = 0;
            cargaIntegracao.PesoBruto = repInformacaoCargaCTe.ObterPesoKg(ctes.Select(o => o.Codigo).ToList());
            cargaIntegracao.CubagemTotal = 0;
            cargaIntegracao.TipoTomador = cte.TipoTomador;
            cargaIntegracao.TipoPagamento = cte.TipoPagamento;

            if (tipoOperacao?.ProdutosPadroes?.Count > 0)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoProdutosPadrao tipoOperacaoProdutoPadrao = tipoOperacao.ProdutosPadroes.FirstOrDefault();

                cargaIntegracao.ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto()
                {
                    CodigoProduto = tipoOperacaoProdutoPadrao.Produto.CodigoProdutoEmbarcador,
                    DescricaoProduto = tipoOperacaoProdutoPadrao.Produto.Descricao
                };
            }
            else
            {
                cargaIntegracao.ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto
                {
                    CodigoProduto = "1",
                    DescricaoProduto = !string.IsNullOrWhiteSpace(cte.ProdutoPredominanteCTe) ? cte.ProdutoPredominanteCTe : "Diversos"
                };
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                cargaIntegracao.NumeroPedidoEmbarcador = cargaIntegracao.NumeroCarga;
                cargaIntegracao.Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial
                {
                    CodigoIntegracao = cte.Remetente.CPF_CNPJ
                };

            }
            cargaIntegracao.FecharCargaAutomaticamente = true;

            return cargaIntegracao;
        }

        private Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao ConverterCTeEmCargaIntegracao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();

            cargaIntegracao.NumeroCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork).ToString();
            cargaIntegracao.CFOP = cte.CFOP.CodigoCFOP;
            cargaIntegracao.TransportadoraEmitente = serEmpresa.ConverterObjetoEmpresa(cte.Empresa);
            cargaIntegracao.UsarOutroEnderecoOrigem = false;
            cargaIntegracao.UsarOutroEnderecoDestino = false;
            cargaIntegracao.Remetente = serPessoa.ConverterObjetoParticipamenteCTe(cte.Remetente);
            cargaIntegracao.Destinatario = serPessoa.ConverterObjetoParticipamenteCTe(cte.Destinatario);
            cargaIntegracao.Tomador = serPessoa.ConverterObjetoParticipamenteCTe(cte.Tomador);
            cargaIntegracao.Recebedor = serPessoa.ConverterObjetoParticipamenteCTe(cte.Recebedor);
            cargaIntegracao.Expedidor = serPessoa.ConverterObjetoParticipamenteCTe(cte.Expedidor);
            cargaIntegracao.DataCriacaoCarga = cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss");
            cargaIntegracao.DataInicioCarregamento = cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss");
            cargaIntegracao.DataFinalCarregamento = cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss");
            cargaIntegracao.NumeroPaletes = 0;
            cargaIntegracao.PesoTotalPaletes = 0;
            cargaIntegracao.ValorTotalPaletes = 0;
            cargaIntegracao.PesoBruto = repInformacaoCargaCTe.ObterPesoKg(cte.Codigo);
            cargaIntegracao.CubagemTotal = 0;
            cargaIntegracao.ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto
            {
                CodigoProduto = "1",
                DescricaoProduto = !string.IsNullOrWhiteSpace(cte.ProdutoPredominanteCTe) ? cte.ProdutoPredominanteCTe : "Diversos"
            };
            cargaIntegracao.TipoTomador = cte.TipoTomador;
            cargaIntegracao.TipoPagamento = cte.TipoPagamento;

            if (configuracaoTMS.TipoOperacaoPadraoCargaDistribuidor != null && !string.IsNullOrWhiteSpace(configuracaoTMS.TipoOperacaoPadraoCargaDistribuidor.CodigoIntegracao))
            {
                cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao()
                {
                    CodigoIntegracao = configuracaoTMS.TipoOperacaoPadraoCargaDistribuidor.CodigoIntegracao
                };
            }

            System.Text.RegularExpressions.Regex patternCPF = new System.Text.RegularExpressions.Regex(@"([0-9]{2}[\.]?[0-9]{3}[\.]?[0-9]{3}[\/]?[0-9]{4}[-]?[0-9]{2})|([0-9]{3}[\.]?[0-9]{3}[\.]?[0-9]{3}[-]?[0-9]{2})");
            System.Text.RegularExpressions.Match matchCPF = patternCPF.Match(cte.ObservacoesGerais);
            if (matchCPF != null && !string.IsNullOrWhiteSpace(matchCPF.Value))
            {
                Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorCPF(matchCPF.Value.ToUpper().ObterSomenteNumeros());
                if (motorista != null)
                {
                    cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();
                    cargaIntegracao.Motoristas.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista()
                    {
                        Codigo = motorista.Codigo,
                        CPF = motorista.CPF,
                        Nome = motorista.Nome
                    });
                }
            }

            System.Text.RegularExpressions.Regex patternVeiculo = new System.Text.RegularExpressions.Regex(@"[A-Z]{3}[0-9]{1}[A-Z]{1}[0-9]{2}|[A-Z]{3}[0-9]{4}");
            System.Text.RegularExpressions.Match matchVeiculo = patternVeiculo.Match(cte.ObservacoesGerais);
            if (matchVeiculo != null && !string.IsNullOrWhiteSpace(matchVeiculo.Value))
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(matchVeiculo.Value.ToUpper().Replace("-", ""));
                if (veiculo != null)
                {
                    cargaIntegracao.Veiculo = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo()
                    {
                        Codigo = veiculo.Codigo,
                        Placa = veiculo.Placa
                    };
                    if (veiculo.ModeloVeicularCarga != null && !string.IsNullOrWhiteSpace(veiculo.ModeloVeicularCarga.CodigoIntegracao))
                    {
                        cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular()
                        {
                            CodigoIntegracao = veiculo.ModeloVeicularCarga.CodigoIntegracao,
                            Descricao = veiculo.ModeloVeicularCarga.Descricao
                        };
                    }
                }
            }

            cargaIntegracao.FecharCargaAutomaticamente = true;

            return cargaIntegracao;
        }

        private void AdicionarComponentesCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, ref Dictionary<int, decimal> componentesAdicionar, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unidadeTrabalho);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeTrabalho);

            foreach (KeyValuePair<int, decimal> componente in componentesAdicionar)
            {
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(componente.Key);

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponente = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete()
                {
                    NaoSomarValorTotalAReceber = false,
                    NaoSomarValorTotalPrestacao = false,
                    AcrescentaValorTotalAReceber = true,
                    CargaPedido = cargaPedido,
                    ComponenteFrete = componenteFrete,
                    TipoComponenteFrete = componenteFrete.TipoComponenteFrete,
                    TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo,
                    ValorComponente = componente.Value
                };

                repCargaPedidoComponenteFrete.Inserir(cargaPedidoComponente);
            }
        }

        private void AdicionarComponentesCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete, ref Dictionary<int, decimal> componentesAdicionarPedido, Repositorio.UnitOfWork unidadeTrabalho, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete> componentes)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete repCargaPedidoDocumentoCTeComponente = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unidadeTrabalho);
            Repositorio.ComponentePrestacaoCTE repComponentePrestacaoCTe = new Repositorio.ComponentePrestacaoCTE(unidadeTrabalho);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete componente in componentes)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = cargaComponentesFrete.Find(o => o.OutraDescricaoCTe == componente.Descricao);

                if (cargaComponenteFrete != null)
                {
                    componente.ComponenteFrete = cargaComponenteFrete.ComponenteFrete;

                    repCargaPedidoDocumentoCTeComponente.Atualizar(componente);

                    if (componente.ComponenteFrete != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete cargaCTeComponentesFrete = new Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete()
                        {
                            NaoSomarValorTotalPrestacao = false,
                            NaoSomarValorTotalAReceber = false,
                            AcrescentaValorTotalAReceber = true,
                            CargaCTe = cargaCTe,
                            ComponenteFrete = componente.ComponenteFrete,
                            TipoComponenteFrete = componente.ComponenteFrete.TipoComponenteFrete,
                            TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo,
                            ValorComponente = componente.Valor
                        };

                        repCargaCTeComponenteFrete.Inserir(cargaCTeComponentesFrete);
                    }

                    if (componente.ComponentePrestacaoCTe != null)
                    {
                        componente.ComponentePrestacaoCTe.ComponenteFrete = componente.ComponenteFrete;

                        repComponentePrestacaoCTe.Atualizar(componente.ComponentePrestacaoCTe);
                    }

                    if (componente.ComponenteFrete != null)
                    {
                        if (componentesAdicionarPedido.ContainsKey(componente.ComponenteFrete.Codigo))
                            componentesAdicionarPedido[componente.ComponenteFrete.Codigo] += componente.Valor;
                        else
                            componentesAdicionarPedido.Add(componente.ComponenteFrete.Codigo, componente.Valor);
                    }
                }
            }
        }

        private void AdicionarComponentesDoCTe(ref List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe, Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcador configuracao, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.ComponentePrestacaoCTE> componentesCTeGeral)
        {
            Repositorio.ComponentePrestacaoCTE repComponenteCTe = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete repCargaPedidoDocumentoCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaPedidoDocumentoCTe.CTe;
            List<Dominio.Entidades.ComponentePrestacaoCTE> componentesCTe = componentesCTeGeral.Where(o => o.CTE.Codigo == cte.Codigo).ToList();

            decimal valorComponentes = 0m;

            bool possuiComponenteICMS = false, possuiComponenteFreteLiquido = false;

            decimal valorFrete = 0m;

            foreach (Dominio.Entidades.ComponentePrestacaoCTE componenteCTe in componentesCTe)
            {

                string descricao = componenteCTe.Nome.ToLower().Trim();
                Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcadorComponente configuracaoComponenteFrete = configuracao.Componentes.Find(o => o.OutraDescricaoCTe.ToLower().Trim() == descricao);
                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = null;
                if (configuracaoComponenteFrete != null)
                {
                    componenteFrete = repComponenteFrete.BuscarPorCodigo(configuracaoComponenteFrete.Codigo);
                }

                if ((!string.IsNullOrWhiteSpace(configuracao.DescricaoComponenteFreteLiquido) && descricao == configuracao.DescricaoComponenteFreteLiquido.ToLower().Trim()) || (componenteFrete != null && componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.FRETE))
                {
                    possuiComponenteFreteLiquido = true;

                    valorFrete = valorFrete + componenteCTe.Valor;

                    continue;
                }

                valorComponentes += componenteCTe.Valor;

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete componenteNovo = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete
                {
                    CargaPedidoDocumentoCTe = cargaPedidoDocumentoCTe,
                    ComponentePrestacaoCTe = componenteCTe,
                    Descricao = componenteCTe.Nome,
                    Valor = componenteCTe.Valor
                };

                repCargaPedidoDocumentoCTeComponenteFrete.Inserir(componenteNovo);

                Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = cargaComponentesFrete.Find(o => o.OutraDescricaoCTe == componenteNovo.Descricao);

                if (cargaComponenteFrete == null)
                {
                    cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete
                    {
                        Carga = cargaPedido.Carga,
                        AcrescentaValorTotalAReceber = true,
                        OutraDescricaoCTe = componenteNovo.Descricao,
                        SomarComponenteFreteLiquido = true,
                        //DescontarComponenteFreteLiquido = true,
                        NaoSomarValorTotalAReceber = false,
                        NaoSomarValorTotalPrestacao = false,
                        TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.INCONSISTENTE,
                        TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo,
                        ValorComponente = componenteNovo.Valor
                    };

                    if (configuracaoComponenteFrete != null)
                    {
                        cargaComponenteFrete.ComponenteFrete = componenteFrete;
                        cargaComponenteFrete.TipoComponenteFrete = cargaComponenteFrete.ComponenteFrete.TipoComponenteFrete;
                        cargaComponenteFrete.IncluirBaseCalculoICMS = configuracaoComponenteFrete.IncluirICMS;

                        componenteNovo.ComponenteFrete = cargaComponenteFrete.ComponenteFrete;

                        repCargaPedidoDocumentoCTeComponenteFrete.Atualizar(componenteNovo);

                        if (cargaComponenteFrete.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
                        {
                            possuiComponenteICMS = true;
                        }
                        else
                        {
                            if (configuracaoComponenteFrete.IncluirICMS != componenteCTe.IncluiNaBaseDeCalculoDoICMS)
                            {
                                componenteCTe.IncluiNaBaseDeCalculoDoICMS = configuracaoComponenteFrete.IncluirICMS;

                                repComponenteCTe.Atualizar(componenteCTe);
                            }
                        }
                    }

                    repCargaComponenteFrete.Inserir(cargaComponenteFrete);

                    cargaComponentesFrete.Add(cargaComponenteFrete);
                }
                else
                {
                    int indexComplemento = cargaComponentesFrete.IndexOf(cargaComponenteFrete);

                    cargaComponentesFrete[indexComplemento].ValorComponente += componenteNovo.Valor;

                    if (cargaComponentesFrete[indexComplemento].ComponenteFrete != null)
                    {
                        componenteNovo.ComponenteFrete = cargaComponentesFrete[indexComplemento].ComponenteFrete;

                        repCargaPedidoDocumentoCTeComponenteFrete.Atualizar(componenteNovo);

                        if (cargaComponenteFrete.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS)
                            possuiComponenteICMS = true;
                    }

                    repCargaComponenteFrete.Atualizar(cargaComponentesFrete[indexComplemento]);
                }
            }

            if (possuiComponenteFreteLiquido)
            {
                if (configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS && cte.CST == "00" && cte.LocalidadeEmissao?.Estado?.Sigla == cte.LocalidadeInicioPrestacao?.Estado?.Sigla)
                    cte.ValorFrete = cte.ValorAReceber - cte.ValorICMS;
                else if (configuracao.ValorFreteLiquidoDeveSerValorAReceber)
                    cte.ValorFrete = cte.ValorAReceber;
                else if ((cte.CST == "60" || cte.CST == "20") && cte.ValorAReceber < cte.ValorPrestacaoServico && cte.ValorPresumido > 0m) //regra para JBS (Tombini)
                    cte.ValorFrete = cte.ValorAReceber - cte.ValorPresumido;
                else if (cte.CST == "00" && cte.BaseCalculoICMS > cte.ValorAReceber && cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Nao) //regra para JBS (Tombini)
                    cte.ValorFrete = cte.ValorAReceber - cte.ValorICMS;
                else
                    cte.ValorFrete = valorFrete;

                repCTe.Atualizar(cte);
            }

            if (possuiComponenteICMS || ((cte.CST == "60" || cte.CST == "20") && cte.ValorPrestacaoServico > cte.ValorAReceber))
            {
                cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                cte.PercentualICMSIncluirNoFrete = 100;

                repCTe.Atualizar(cte);
            }

            if (!possuiComponenteFreteLiquido && cte.ModeloDocumentoFiscal != null && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                if (configuracao.ValorFreteLiquidoDeveSerValorAReceber)
                    cte.ValorFrete = cte.ValorAReceber;
                else
                    cte.ValorFrete = 0m;

                repCTe.Atualizar(cte);
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcador ObterConfiguracoesComponentes(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcador configuracao = new Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcador();
            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();
            configuracao.Componentes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcadorComponente>();

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
            {
                configuracao.Componentes = cargaPedido.Carga.TipoOperacao.TipoOperacaoConfiguracoesComponentes.Where(o => o.ComponenteFrete != null && !string.IsNullOrWhiteSpace(o.OutraDescricaoCTe)).Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcadorComponente() { Codigo = o.ComponenteFrete.Codigo, OutraDescricaoCTe = o.OutraDescricaoCTe, IncluirICMS = o.IncluirICMS }).ToList();

                configuracao.DescricaoComponenteFreteLiquido = cargaPedido.Carga.TipoOperacao.DescricaoComponenteFreteEmbarcador;
                configuracao.ValorFreteLiquidoDeveSerValorAReceber = cargaPedido.Carga.TipoOperacao.ValorFreteLiquidoDeveSerValorAReceber;
                configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS = cargaPedido.Carga.TipoOperacao.ValorFreteLiquidoDeveSerValorAReceberSemICMS;
            }
            else if (tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    configuracao.Componentes = tomador.ClienteConfiguracoesComponentes.Where(o => o.ComponenteFrete != null && !string.IsNullOrWhiteSpace(o.OutraDescricaoCTe)).Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcadorComponente() { Codigo = o.ComponenteFrete.Codigo, OutraDescricaoCTe = o.OutraDescricaoCTe, IncluirICMS = o.IncluirICMS }).ToList();

                    configuracao.DescricaoComponenteFreteLiquido = tomador.DescricaoComponenteFreteEmbarcador;
                    configuracao.ValorFreteLiquidoDeveSerValorAReceber = tomador.ValorFreteLiquidoDeveSerValorAReceber;
                    configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS = tomador.ValorFreteLiquidoDeveSerValorAReceberSemICMS;
                }
                else if (tomador.GrupoPessoas != null)
                {
                    configuracao.Componentes = tomador.GrupoPessoas.GrupoPessoasConfiguracaoComponentesFretes.Where(o => o.ComponenteFrete != null && !string.IsNullOrWhiteSpace(o.OutraDescricaoCTe)).Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcadorComponente() { Codigo = o.ComponenteFrete.Codigo, OutraDescricaoCTe = o.OutraDescricaoCTe, IncluirICMS = o.IncluirICMS }).ToList();

                    configuracao.DescricaoComponenteFreteLiquido = tomador.GrupoPessoas.DescricaoComponenteFreteEmbarcador;
                    configuracao.ValorFreteLiquidoDeveSerValorAReceber = tomador.GrupoPessoas.ValorFreteLiquidoDeveSerValorAReceber;
                    configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS = tomador.GrupoPessoas.ValorFreteLiquidoDeveSerValorAReceberSemICMS;
                }
            }
            else
            {
                if (cargaPedido.Pedido.GrupoPessoas != null)
                {
                    configuracao.Componentes = cargaPedido.Pedido.GrupoPessoas.GrupoPessoasConfiguracaoComponentesFretes.Where(o => o.ComponenteFrete != null && !string.IsNullOrWhiteSpace(o.OutraDescricaoCTe)).Select(o => new Dominio.ObjetosDeValor.Embarcador.CTe.ConfiguracaoCTeEmitidoEmbarcadorComponente() { Codigo = o.ComponenteFrete.Codigo, OutraDescricaoCTe = o.OutraDescricaoCTe, IncluirICMS = o.IncluirICMS }).ToList();

                    configuracao.DescricaoComponenteFreteLiquido = cargaPedido.Pedido.GrupoPessoas.DescricaoComponenteFreteEmbarcador;
                    configuracao.ValorFreteLiquidoDeveSerValorAReceber = cargaPedido.Pedido.GrupoPessoas.ValorFreteLiquidoDeveSerValorAReceber;
                    configuracao.ValorFreteLiquidoDeveSerValorAReceberSemICMS = cargaPedido.Pedido.GrupoPessoas.ValorFreteLiquidoDeveSerValorAReceberSemICMS;
                }
            }

            return configuracao;
        }

        private string AdicionarNFeCargaPedido(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal tipoNotaFiscal, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = null, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = null)
        {
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Pedido.NotaFiscal(unitOfWork);

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            if (configuracaoTMS == null)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && cargaPedido.Pedido.Destinatario == null)
            {
                if (xmlNotaFiscal.Destinatario != null)
                {
                    cargaPedido.Pedido.Destinatario = xmlNotaFiscal.Destinatario;
                    cargaPedido.Pedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;

                    repPedido.Atualizar(cargaPedido.Pedido);
                }
                else if (xmlNotaFiscal.ObterEmitente != null)
                {
                    cargaPedido.Pedido.Destinatario = xmlNotaFiscal.ObterEmitente;
                    cargaPedido.Pedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;

                    repPedido.Atualizar(cargaPedido.Pedido);
                }
            }

            bool msgAlertaObservacao = false;
            bool notaFiscalEmOutraCarga = false;
            string retorno = (!cargaPedido.Pedido.PedidoTransbordo && !cargaPedido.Carga.CargaSVM && !cargaPedido.CTeEmitidoNoEmbarcador) ? serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga, configuracaoTMS) : "";
            if (msgAlertaObservacao && !string.IsNullOrWhiteSpace(retorno))
                retorno = "";

            if (string.IsNullOrEmpty(retorno))
            {
                if (!cargaPedido.Pedido.PedidoTransbordo)
                {
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                    xmlNotaFiscal.DataRecebimento = DateTime.Now;
                    repXMLNotaFiscal.Inserir(xmlNotaFiscal);
                }
                else
                {
                    if (cargaPedido.Pedido.Remetente == null)
                    {
                        cargaPedido.Pedido.Remetente = xmlNotaFiscal.ObterEmitente;
                        cargaPedido.Pedido.Origem = cargaPedido.Pedido.Remetente.Localidade;
                        repPedido.Atualizar(cargaPedido.Pedido);
                    }
                }

                if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    repPedido.Atualizar(cargaPedido.Pedido);
                    repCarga.Atualizar(cargaPedido.Carga);
                }

                serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, tipoNotaFiscal, configuracaoTMS, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga, null, null, configuracaoCanhoto, pedidoXMLNotasFiscais);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                Servicos.Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, "Adicionado via CT-es Importados", unitOfWork);
            }

            return retorno;
        }

        private string AdicionarCTeTerceiroCargaPedido(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Canhotos.Canhoto(unitOfWork);

            string retorno = !cargaPedido.Pedido.PedidoTransbordo ? serCTeSubContratacao.ValidarRegrasCTeParaSubContratacao(cteTerceiro, cargaPedido, unitOfWork, tipoServicoMultisoftware) : "";

            if (string.IsNullOrEmpty(retorno))
            {
                if (cargaPedido.Pedido.PedidoTransbordo)
                {
                    if (cargaPedido.Pedido.Remetente == null)
                    {
                        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                        cargaPedido.Pedido.Remetente = cteTerceiro.Remetente.Cliente;
                        cargaPedido.Pedido.Origem = cargaPedido.Pedido.Remetente.Localidade;
                        repPedido.Atualizar(cargaPedido.Pedido);
                    }
                }

                serCTeSubContratacao.InserirCTeSubContratacaoCargaPedido(cteTerceiro, cargaPedido, tipoServicoMultisoftware, unitOfWork);

                if (!cargaPedido.Pedido.PedidoTransbordo)
                    serCanhoto.SalvarCanhotoCTe(cteTerceiro, cargaPedido, cargaPedido.Carga.FreteDeTerceiro && cargaPedido.Carga.Veiculo != null ? cargaPedido.Carga.Veiculo.Proprietario : cargaPedido.Carga.ProvedorOS, cargaPedido.Carga.Motoristas.ToList(), tipoServicoMultisoftware, unitOfWork);
            }

            return retorno;
        }

        private void DestinarCTeAOcorrencia(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
            Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);
            Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCte.BuscarPorChaveCTe(cte.ChaveCTESubComp);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            if (configuracaoGeral?.NaoGerarOcorrenciaCTeImportadosEmailEmbarcador ?? false)
                return;

            if (cargaCTe != null)
            {
                if (repCargaOcorrenciaDocumento.ExisteAtivoPorCTe(cte.Codigo))
                    return;

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = null;

                if (cargaCTe.Carga.TipoOperacao != null && cargaCTe.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                {
                    if (cargaCTe.Carga.TipoOperacao.CTeEmitidoNoEmbarcador)
                        tipoOcorrencia = cargaCTe.Carga.TipoOperacao.TipoOcorrenciaCTeEmitidoEmbarcador;
                }
                else if (cargaCTe.CTe.TomadorPagador != null)
                {
                    if (cargaCTe.CTe.TomadorPagador.Cliente != null && cargaCTe.CTe.TomadorPagador.Cliente.NaoUsarConfiguracaoEmissaoGrupo)
                    {
                        if (cargaCTe.CTe.TomadorPagador.Cliente.CTeEmitidoNoEmbarcador)
                            tipoOcorrencia = cargaCTe.CTe.TomadorPagador.Cliente.TipoOcorrenciaCTeEmitidoEmbarcador;
                    }
                    else if (cargaCTe.CTe.TomadorPagador.GrupoPessoas != null)
                    {
                        if (cargaCTe.CTe.TomadorPagador.GrupoPessoas.CTeEmitidoNoEmbarcador)
                            tipoOcorrencia = cargaCTe.CTe.TomadorPagador.GrupoPessoas.TipoOcorrenciaCTeEmitidoEmbarcador;
                    }
                }

                if (!srvOcorrencia.AjustarCTeImportado(out string erro, cte, cargaCTe.Carga, tipoOcorrencia?.ComponenteFrete, unitOfWork))
                    throw new Exception(erro);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia
                {
                    Carga = cargaCTe.Carga,
                    DataAlteracao = DateTime.Now,
                    DataOcorrencia = cte.DataEmissao.HasValue ? cte.DataEmissao.Value : DateTime.Now,
                    DataFinalizacaoEmissaoOcorrencia = cte.DataEmissao.HasValue ? cte.DataEmissao.Value : DateTime.Now,
                    NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork),
                    Observacao = "Ocorrência gerada automaticamente pelo Embarcador.",
                    SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgInformacoes,
                    ValorOcorrencia = cte.ValorAReceber,
                    ValorOcorrenciaOriginal = cte.ValorAReceber,
                    ObservacaoCTe = !string.IsNullOrWhiteSpace(cte.ObservacoesGerais) ? cte.ObservacoesGerais : "",
                    CTeEmitidoNoEmbarcador = true
                };

                if (cargaOcorrencia.Codigo <= 0)
                    repOcorrencia.Inserir(cargaOcorrencia);
                else
                    repOcorrencia.Atualizar(cargaOcorrencia);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento
                {
                    CargaCTe = cargaCTe,
                    CargaOcorrencia = cargaOcorrencia,
                    CTeImportado = cte
                };

                repCargaOcorrenciaDocumento.Inserir(cargaOcorrenciaDocumento);

                cte.CTeSemCarga = false;

                repCTe.Atualizar(cte);

                if (tipoOcorrencia != null && tipoOcorrencia.ComponenteFrete != null)
                {
                    if (tipoOcorrencia.DataOcorrenciaIgualDataCTeComplementado)
                        cargaOcorrencia.DataOcorrencia = cargaCTe.CTe.DataEmissao ?? cargaOcorrencia.DataOcorrencia;

                    cargaOcorrencia.TipoOcorrencia = tipoOcorrencia;
                    cargaOcorrencia.OrigemOcorrencia = tipoOcorrencia.OrigemOcorrencia;

                    cargaOcorrencia.ComponenteFrete = tipoOcorrencia.ComponenteFrete;
                    cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar;

                    bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(cargaOcorrencia.Carga?.TabelaFrete, tipoOcorrencia.ComponenteFrete);
                    bool descontarComponenteFreteLiquido = (destacarComponenteTabelaFrete ? cargaOcorrencia.Carga?.TabelaFrete?.DescontarComponenteFreteLiquido : tipoOcorrencia.ComponenteFrete.DescontarComponenteFreteLiquido) ?? false;

                    if (tipoOcorrencia.ComponenteFrete.SomarComponenteFreteLiquido || descontarComponenteFreteLiquido)
                        cargaOcorrencia.ValorOcorrenciaLiquida = cargaOcorrencia.ValorOcorrencia;
                    else
                        cargaOcorrencia.ValorOcorrenciaLiquida = 0;

                    Dominio.Entidades.OcorrenciaDeCTe ocorrenciaDeCTe = srvOcorrencia.GerarOcorrenciaCTe(cargaOcorrencia, cargaCTe, unitOfWork);
                    cargaOcorrenciaDocumento.OcorrenciaDeCTe = ocorrenciaDeCTe;
                    repCargaOcorrenciaDocumento.Atualizar(cargaOcorrenciaDocumento);

                    repOcorrencia.Atualizar(cargaOcorrencia);

                    serCargaCTeComplementar.ImportarCTesComplementaresParaOcorrencia(cargaOcorrencia, unitOfWork, tipoServicoMultisoftware);

                    Servicos.Embarcador.Integracao.IntegracaoEDI.AdicionarEDIParaIntegracao(cargaOcorrencia, false, tipoServicoMultisoftware, unitOfWork);

                    serOcorrencia.ValidarEnviarEmissaoComplementosOcorrencia(cargaOcorrencia, unitOfWork);
                }
            }
        }

        public bool DestinarCTeACarga(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, string nomeArquivo, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null)
        {
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Normal && cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                return false;

            bool adicionou = false;

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            if (configuracaoTMS.UtilizaEmissaoMultimodal && !string.IsNullOrWhiteSpace(nomeArquivo))
            {
                Servicos.Log.TratarErro(nomeArquivo, "XMLEmail");
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorNumeroCargaSituacao(nomeArquivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)?.FirstOrDefault();
                if (cargaPedido == null)
                    cargaPedido = repCargaPedido.BuscarPorNumeroOSSituacao(nomeArquivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)?.FirstOrDefault();
                if (cargaPedido == null)
                    cargaPedido = repCargaPedido.BuscarPorNumeroBookingSituacao(nomeArquivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)?.FirstOrDefault();

                if (cargaPedido != null && cargaPedido.Carga != null)
                {
                    if (VerificarNaoPermiterVincularCTeComplementarEmCarga(cte, cargaPedido, unitOfWork))
                        return false;

                    if (cargaPedido.Carga.GrupoPessoaPrincipal?.Codigo != cte.Tomador?.GrupoPessoas?.Codigo)
                        return false;

                    svcCTe.SalvarInformacoesMultiModal(cte, cargaPedido, cte.ValorAReceber, unitOfWork);
                    if (!configuracaoTMS.UtilizaEmissaoMultimodal)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaPedido.Carga.Codigo);
                        carga.CargaIntegradaEmbarcador = true;
                        repCarga.Atualizar(carga);
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = repCargaPedidoDocumentoCTe.BuscarPorCTeECargaPedido(cte.Codigo, cargaPedido.Codigo);
                    if (cargaPedidoDocumentoCTe == null)
                        cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe();
                    cargaPedidoDocumentoCTe.CargaPedido = cargaPedido;
                    cargaPedidoDocumentoCTe.CTe = cte;
                    if (cargaPedidoDocumentoCTe.Codigo > 0)
                        repCargaPedidoDocumentoCTe.Atualizar(cargaPedidoDocumentoCTe);
                    else
                        repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe);
                    cte.CTeSemCarga = false;
                    repCTe.Atualizar(cte);
                    adicionou = true;
                }
            }
            else if (cte.Veiculos != null && cte.Veiculos.Count > 0)
            {
                if (repCargaPedidoDocumentoCTe.ExistePorCTe(cte.Codigo) ||
                    repCargaCTe.ExisteAutorizadoPorCTe(cte.Codigo))
                    return true;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorVeiculoOrigemSituacao(cte.Veiculos.FirstOrDefault().Veiculo.Codigo, cte.LocalidadeInicioPrestacao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe);

                if (cargaPedidos.Count == 1)
                {
                    if (VerificarNaoPermiterVincularCTeComplementarEmCarga(cte, cargaPedidos.FirstOrDefault(), unitOfWork))
                        return false;

                    if (cargaPedidos.FirstOrDefault()?.Carga?.GrupoPessoaPrincipal?.Codigo != cte.Tomador?.GrupoPessoas?.Codigo)
                        return false;

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                    {
                        CargaPedido = cargaPedidos.FirstOrDefault(),
                        CTe = cte
                    };

                    repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe, auditado);

                    if (auditado != null)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedidoDocumentoCTe.CargaPedido.Carga, $"Vinculou o CT-e {cte.Descricao} à carga.", unitOfWork);

                    cte.CTeSemCarga = false;

                    repCTe.Atualizar(cte);

                    adicionou = true;
                }
            }
            else if (cte.TomadorPagador?.GrupoPessoas != null && cte.TomadorPagador.GrupoPessoas.VincularCTePeloNumeroPedido && !string.IsNullOrWhiteSpace(cte.NumeroPedido))
            {
                if (repCargaPedidoDocumentoCTe.ExistePorCTe(cte.Codigo) ||
                    repCargaCTe.ExisteAutorizadoPorCTe(cte.Codigo))
                    return true;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorNumeroPedidoEmbarcador(cte.NumeroPedido, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe);

                if (cargaPedidos.Count() > 1 || cargaPedidos.Count() <= 0)
                    return false;

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();

                if (cargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo != cte.Tomador?.GrupoPessoas?.Codigo)
                    return false;

                if (VerificarNaoPermiterVincularCTeComplementarEmCarga(cte, cargaPedido, unitOfWork))
                    return false;

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                {
                    CargaPedido = cargaPedido,
                    CTe = cte
                };

                repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe, auditado);

                if (auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedidoDocumentoCTe.CargaPedido.Carga, $"Vinculou o CT-e {cte.Descricao} à carga.", unitOfWork);

                cte.CTeSemCarga = false;

                repCTe.Atualizar(cte);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasCTe = cte.TomadorPagador.GrupoPessoas;

                if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroPedidoEmbarcador) && !string.IsNullOrWhiteSpace(cte.NumeroPedido) && grupoPessoasCTe.LerNumeroPedidoObservacaoCTe && grupoPessoasCTe.SetarNumeroPedidoEmbarcadorPeloNumeroPedidoCTe)
                {
                    cargaPedido.Pedido.NumeroPedidoEmbarcador = cte.NumeroPedido;

                    repPedido.Atualizar(cargaPedido.Pedido);
                }

                adicionou = true;
            }

            return adicionou;
        }

        public static void ObterCTeVinculadoAoCTeTerceiro(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Repositorio.UnitOfWork unitOfWork, out Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, out Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            pedidoCTeParaSubContratacao = null;
            cargaCTe = null;

            if (cteTerceiro == null)
                return;

            if (cteTerceiro.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                return;

            if (repOcorrencia.ExisteOcorrenciaValidaPorCTeTerceiro(cteTerceiro.Codigo))
                return;

            pedidoCTeParaSubContratacao = repPedidoCTeParaSubcontratacao.BuscarPorChaveComCargaEmitida(cteTerceiro.ChaveCTEReferenciado);

            if (pedidoCTeParaSubContratacao == null || pedidoCTeParaSubContratacao.CTeTerceiro == null)
                return;

            cargaCTe = repCargaCTe.BuscarPorChaveDocumentoAnterior(pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso, pedidoCTeParaSubContratacao.CargaPedido.Carga.Codigo);
        }

        public static bool DestinarCTeTerceiroAPedido(out string mensagem, Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            mensagem = null;

            //if (configuracaoTMS.GerarOcorrenciaComplementoSubcontratacao)
            //    return true;

            if (string.IsNullOrWhiteSpace(cteTerceiro.NumeroPedido))
                return true;

            Servicos.Embarcador.Carga.CTeSubContratacao serCargaCteParaSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorNumeroPedidoEmbarcador(cteTerceiro.NumeroPedido).FirstOrDefault();

                if (cargaPedido == null)
                {
                    mensagem = $"Não foi possível encontrar carga pedido para o número {cteTerceiro.NumeroPedido}";
                    return true;
                }

                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = null;
                string retorno = serCargaCteParaSubContratacao.InformarDadosCTeNaCarga(unitOfWork, cte, cargaPedido, tipoServicoMultisoftware, ref pedidoCTeParaSubContratacao, false, cteTerceiro);
            }
            catch (ServicoException exception)
            {
                mensagem = exception.Message;
                return false;
            }

            return true;
        }

        public static bool DestinarCTeTerceiroAOcorrencia(out string mensagem, Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            mensagem = null;

            //if (!configuracaoTMS.GerarOcorrenciaComplementoSubcontratacao)
            //    return true;

            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

            ObterCTeVinculadoAoCTeTerceiro(cteTerceiro, unitOfWork, out Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, out Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe);

            if (cargaCTe == null)
                return true;

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = null;

            if (cargaCTe.Carga.TipoOperacao != null && cargaCTe.Carga.TipoOperacao.UsarConfiguracaoEmissao)
            {
                if (cargaCTe.Carga.TipoOperacao.GerarOcorrenciaComplementoSubcontratacao)
                    tipoOcorrencia = cargaCTe.Carga.TipoOperacao.TipoOcorrenciaComplementoSubcontratacao;
            }
            else if (cargaCTe.CTe.TomadorPagador != null)
            {
                if (cargaCTe.CTe.TomadorPagador.Cliente != null && cargaCTe.CTe.TomadorPagador.Cliente.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    if (cargaCTe.CTe.TomadorPagador.Cliente.GerarOcorrenciaComplementoSubcontratacao)
                        tipoOcorrencia = cargaCTe.CTe.TomadorPagador.Cliente.TipoOcorrenciaComplementoSubcontratacao;
                }
                else if (cargaCTe.CTe.TomadorPagador.GrupoPessoas != null)
                {
                    if (cargaCTe.CTe.TomadorPagador.GrupoPessoas.GerarOcorrenciaComplementoSubcontratacao)
                        tipoOcorrencia = cargaCTe.CTe.TomadorPagador.GrupoPessoas.TipoOcorrenciaComplementoSubcontratacao;
                }
            }

            if (tipoOcorrencia == null)
                return true;

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia
            {
                Carga = cargaCTe.Carga,
                CTeTerceiro = cteTerceiro,
                DataAlteracao = DateTime.Now,
                DataOcorrencia = cteTerceiro.DataEmissao,
                NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork),
                Observacao = $"Ocorrência gerada automaticamente pelo CT-e de complemento {cteTerceiro.Descricao} emitido para o CT-e para subcontratação {pedidoCTeParaSubContratacao.CTeTerceiro.Descricao}, referenciado no CT-e {cargaCTe.CTe.Descricao}.",
                SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada,
                ValorOcorrencia = cteTerceiro.ValorAReceber,
                ValorOcorrenciaOriginal = cteTerceiro.ValorAReceber,
                ObservacaoCTe = cteTerceiro.ObservacaoGeral,
                TipoOcorrencia = tipoOcorrencia,
                ComponenteFrete = tipoOcorrencia.ComponenteFrete,
                OrigemOcorrencia = tipoOcorrencia.OrigemOcorrencia
            };

            repOcorrencia.Inserir(cargaOcorrencia);

            if (!srvOcorrencia.FluxoGeralOcorrencia(ref cargaOcorrencia, new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> { cargaCTe }, null, ref mensagem, unitOfWork, tipoServicoMultisoftware, null, configuracaoTMS, null, null, false))
                return false;

            repOcorrencia.Atualizar(cargaOcorrencia);

            return true;
        }

        private void VerificarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            int naoCancelados = repCargaCTe.ContarNaoCanceladosPorCarga(carga.Codigo);

            if (naoCancelados == 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                {
                    Carga = carga,
                    DuplicarCarga = !configuracao.NaoDuplicarCargaAoCancelarPorImportacaoXMLCTeCancelado,
                    MotivoCancelamento = "CT-es da carga cancelados no Embarcador",
                    TipoServicoMultisoftware = tipoServicoMultisoftware,
                    UsuarioERPSolicitouCancelamento = "Usuário do Embarcador"
                };

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracao, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCancelamento, null, $"Adicionou {cargaCancelamento.DescricaoTipo} da Carga na importação do CTe", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaCancelamento.Carga, null, $"Adicionou {cargaCancelamento.DescricaoTipo} da Carga na importação do CTe", unitOfWork);

                if (cargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento ||
                    cargaCancelamento.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgConfirmacao)// quando o cancelamento foi rejeitado anteriormente deve apenas solicitar o envio novamente.
                {
                    cargaCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.EmCancelamento;
                    repCargaCancelamento.Atualizar(cargaCancelamento);
                }
            }
        }

        private void VerificarCancelamentoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.Ocorrencia serCargaOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia(cargaOcorrencia.Codigo);
            int naoCancelados = (from obj in cargaCTeComplementoInfo where obj.CTe != null && (obj.CTe.Status != "C" && obj.CTe.Status != "I") select obj).Count();
            if (naoCancelados == 0)
            {
                if (!serCargaOcorrencia.VerificarSeOcorrenciaPermiteCancelamento(out string mensagem, cargaOcorrencia, unitOfWork, tipoServicoMultisoftware))
                {
                    Servicos.Log.TratarErro("Falha ao cancelar a ocorrência: " + mensagem);
                    return;
                }

                cargaOcorrencia.SituacaoOcorrenciaNoCancelamento = cargaOcorrencia.SituacaoOcorrencia;

                repCargaOcorrencia.Atualizar(cargaOcorrencia);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento cancelamentoEmbarcador = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento()
                {
                    DataCancelamento = DateTime.Now,
                    MotivoCancelamento = "Cancelamento da Ocorrência Solicitado Pelo Embarcador",
                    Ocorrencia = cargaOcorrencia,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.EmCancelamento,
                    SituacaoOcorrenciaNoCancelamento = cargaOcorrencia.SituacaoOcorrencia,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Cancelamento,
                    Usuario = null
                };

                repOcorrenciaCancelamento.Inserir(cancelamentoEmbarcador);
            }
        }

        private void RemoverCTeCanceladoDaCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentosCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            int codigoCargaPedido = cargaPedidoDocumentosCTe.CargaPedido.Codigo;
            if (cargaPedidoDocumentosCTe.CargaPedido.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
            {
                RemoverDocumentosParaRetornarEtapa(cargaPedidoDocumentosCTe.CargaPedido, unitOfWork);
                cargaPedidoDocumentosCTe.CargaPedido.Carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe;
                repCarga.Atualizar(cargaPedidoDocumentosCTe.CargaPedido.Carga);

                cargaPedidoDocumentosCTe.CargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.AgEnvioNF;
                repCargaPedido.Atualizar(cargaPedidoDocumentosCTe.CargaPedido);

            }
            repCargaPedidoDocumentoCTe.Deletar(cargaPedidoDocumentosCTe);
            ReprocessarOrdemCargaPedidoDocumentoCTe(codigoCargaPedido, unitOfWork);
        }

        private void RemoverCTeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.Ocorrencia servicoCargaOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);
            Servicos.Embarcador.Integracao.IntegracaoOcorrenciaCancelamento servicoIntegracaoOcorrenciaCancelamento = new Servicos.Embarcador.Integracao.IntegracaoOcorrenciaCancelamento(unitOfWork);

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> cargaOcorrenciasDocumentos = repCargaOcorrenciaDocumento.BuscarPorOcorrencia(cargaOcorrenciaDocumento.CargaOcorrencia.Codigo);
            int naoCancelados = (from obj in cargaOcorrenciasDocumentos where obj.CargaCTe != null && (obj.CTeImportado.Status != "C" && obj.CTeImportado.Status != "I") select obj).Count();

            if (naoCancelados == 0)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento cancelamentoEmbarcador = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento()
                {
                    DataCancelamento = DateTime.Now,
                    MotivoCancelamento = "Cancelamento da Ocorrência Solicitado Pelo Embarcador",
                    Ocorrencia = cargaOcorrenciaDocumento.CargaOcorrencia,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.EmCancelamento,
                    SituacaoOcorrenciaNoCancelamento = cargaOcorrenciaDocumento.CargaOcorrencia.SituacaoOcorrencia,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCancelamentoOcorrencia.Cancelamento,
                    Usuario = null
                };

                repOcorrenciaCancelamento.Inserir(cancelamentoEmbarcador);

                bool possuiIntegracao = servicoIntegracaoOcorrenciaCancelamento.AdicionarIntegracoesCancelamento(cancelamentoEmbarcador);

                servicoCargaOcorrencia.CancelarOcorrencia(cancelamentoEmbarcador, unitOfWork, tipoServicoMultisoftware, StringConexao);

                if (possuiIntegracao && (cancelamentoEmbarcador.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.Cancelada))
                {
                    cancelamentoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoOcorrencia.AguardandoIntegracao;
                    repOcorrenciaCancelamento.Atualizar(cancelamentoEmbarcador);
                }
            }
            else
            {
                cargaOcorrenciaDocumento.CargaOcorrencia.ValorOcorrencia -= cargaOcorrenciaDocumento.CTeImportado.ValorAReceber;
                repCargaOcorrencia.Atualizar(cargaOcorrenciaDocumento.CargaOcorrencia);
            }
        }

        private decimal obterPesoEmKG(Dominio.Entidades.InformacaoCargaCTE infoCTe, int quantidadesCTe, string descricaoItemPeso)
        {
            decimal pesoKg = 0;
            if (!string.IsNullOrWhiteSpace(descricaoItemPeso))
            {
                if (infoCTe.Tipo.ToUpper() == descricaoItemPeso.ToUpper())
                    pesoKg = infoCTe.Quantidade;
            }
            else
            {
                if (infoCTe.UnidadeMedida == "01")
                    pesoKg = infoCTe.Quantidade;
                else if (infoCTe.UnidadeMedida == "02")
                    pesoKg = infoCTe.Quantidade * 1000;
                else if (infoCTe.UnidadeMedida == "03")
                {
                    if (quantidadesCTe == 1)
                        pesoKg = infoCTe.Quantidade;
                }
                else
                {
                    pesoKg += infoCTe.Quantidade;
                }
            }
            return pesoKg;
        }

        private bool VerificarNaoPermiterVincularCTeComplementarEmCarga(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                return false;

            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            if (cargaPedido.Carga?.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
            {
                return cargaPedido.Carga.TipoOperacao.CTeEmitidoNoEmbarcador ? cargaPedido.Carga.TipoOperacao.NaoPermitirVincularCTeComplementarEmCarga : false;
            }
            else
            {
                if (tomador != null)
                {
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                        return tomador.CTeEmitidoNoEmbarcador ? tomador.NaoPermitirVincularCTeComplementarEmCarga : false;
                    else if (tomador.GrupoPessoas != null)
                        return tomador.GrupoPessoas.CTeEmitidoNoEmbarcador ? tomador.GrupoPessoas.NaoPermitirVincularCTeComplementarEmCarga : false;
                }
            }

            return false;
        }

        #endregion
    }
}
