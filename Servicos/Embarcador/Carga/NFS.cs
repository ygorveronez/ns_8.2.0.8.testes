using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class NFS : ServicoBase
    {        
        public NFS(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CriarNFPendenteEmissaoManualPorCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade localidadePrestacao, Repositorio.UnitOfWork unitOfWork, decimal valorResidual = 0)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaNFeParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaNFeParaEmissaoNFSManual = new Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual();

            decimal valorComponentes = repCargaPedidoComponentesFrete.BuscarValorComponentesFreteLiquido(cargaPedido.Codigo, false);

            cargaNFeParaEmissaoNFSManual.Carga = new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = carga.Codigo };
            cargaNFeParaEmissaoNFSManual.CargaOrigem = new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = cargaPedido.CargaOrigem.Codigo };
            cargaNFeParaEmissaoNFSManual.LocalidadePrestacao = localidadePrestacao;
            cargaNFeParaEmissaoNFSManual.CargaPedido = cargaPedido;
            cargaNFeParaEmissaoNFSManual.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("99");

            cargaNFeParaEmissaoNFSManual.Tomador = tomador;
            cargaNFeParaEmissaoNFSManual.ValorFrete = valorResidual == 0 ? cargaPedido.ValorFrete + valorComponentes : valorResidual;
            cargaNFeParaEmissaoNFSManual.Peso = cargaPedido.Peso;
            cargaNFeParaEmissaoNFSManual.Descricao = cargaPedido.Pedido.NumeroPedidoEmbarcador;
            cargaNFeParaEmissaoNFSManual.Numero = 0;
            cargaNFeParaEmissaoNFSManual.DataEmissao = cargaPedido.Carga.DataInicioGeracaoCTes.HasValue ? cargaPedido.Carga.DataInicioGeracaoCTes.Value : DateTime.Now;
            cargaNFeParaEmissaoNFSManual.NumeroPedidoCliente = cargaPedido.Pedido.CodigoPedidoCliente;

            if (valorResidual > 0)
                cargaNFeParaEmissaoNFSManual.DocResidual = true;

            cargaNFeParaEmissaoNFSManual.Remetente = new Dominio.Entidades.Cliente() { CPF_CNPJ = cargaPedido.Pedido.Remetente.CPF_CNPJ };
            cargaNFeParaEmissaoNFSManual.Destinatario = new Dominio.Entidades.Cliente() { CPF_CNPJ = cargaPedido.Pedido.Destinatario.CPF_CNPJ };

            Servicos.Embarcador.NFSe.NFSManual.ValidarExistenciaEInserirDocumentoParaEmissaoNFSManual(cargaNFeParaEmissaoNFSManual, repCargaNFeParaEmissaoNFSManual, unitOfWork);

            return cargaNFeParaEmissaoNFSManual;
        }


        public Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual CriarNFPendenteEmissaoManualDeNFS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade localidadePrestacao, Repositorio.UnitOfWork unitOfWork, decimal valorResidual = 0)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaNFeParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaNFeParaEmissaoNFSManual = new Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual();
            cargaNFeParaEmissaoNFSManual.Carga = new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = carga.Codigo };
            cargaNFeParaEmissaoNFSManual.CargaOrigem = new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = pedidoXMLNotaFiscal.CargaPedido.CargaOrigem.Codigo };
            cargaNFeParaEmissaoNFSManual.LocalidadePrestacao = localidadePrestacao;
            cargaNFeParaEmissaoNFSManual.PedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
            cargaNFeParaEmissaoNFSManual.Tomador = tomador;
            cargaNFeParaEmissaoNFSManual.ValorFrete = valorResidual == 0 ? pedidoXMLNotaFiscal.ValorFrete + pedidoXMLNotaFiscal.ValorTotalComponentes : valorResidual;
            cargaNFeParaEmissaoNFSManual.Peso = pedidoXMLNotaFiscal.XMLNotaFiscal.Peso;
            cargaNFeParaEmissaoNFSManual.Chave = pedidoXMLNotaFiscal.XMLNotaFiscal.Chave;
            cargaNFeParaEmissaoNFSManual.Descricao = pedidoXMLNotaFiscal.XMLNotaFiscal.Descricao;
            cargaNFeParaEmissaoNFSManual.Serie = pedidoXMLNotaFiscal.XMLNotaFiscal.Serie;
            cargaNFeParaEmissaoNFSManual.Numero = pedidoXMLNotaFiscal.XMLNotaFiscal.Numero;
            cargaNFeParaEmissaoNFSManual.DataEmissao = pedidoXMLNotaFiscal.XMLNotaFiscal.DataEmissao;
            cargaNFeParaEmissaoNFSManual.Moeda = pedidoXMLNotaFiscal.Moeda;
            cargaNFeParaEmissaoNFSManual.ValorCotacaoMoeda = pedidoXMLNotaFiscal.ValorCotacaoMoeda;
            cargaNFeParaEmissaoNFSManual.ValorTotalMoeda = (pedidoXMLNotaFiscal.ValorTotalMoeda ?? 0m) + pedidoXMLNotaFiscal.ValorTotalMoedaComponentes;

            cargaNFeParaEmissaoNFSManual.NumeroPedidoCliente = pedidoXMLNotaFiscal.CargaPedido?.Pedido?.CodigoPedidoCliente;

            if (valorResidual > 0)
                cargaNFeParaEmissaoNFSManual.DocResidual = true;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal tipoOperacaoNota = pedidoXMLNotaFiscal.XMLNotaFiscal.TipoOperacaoNotaFiscal;
            if (tipoOperacaoNota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada) // esse caso é utilizado para notas de importação
            {
                cargaNFeParaEmissaoNFSManual.Remetente = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario;
                cargaNFeParaEmissaoNFSManual.Destinatario = pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente;
            }
            else
            {
                cargaNFeParaEmissaoNFSManual.Remetente = new Dominio.Entidades.Cliente() { CPF_CNPJ = pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ };
                cargaNFeParaEmissaoNFSManual.Destinatario = new Dominio.Entidades.Cliente() { CPF_CNPJ = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ };
            }

            cargaNFeParaEmissaoNFSManual.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo(pedidoXMLNotaFiscal.XMLNotaFiscal.Modelo);
            if (cargaNFeParaEmissaoNFSManual.ModeloDocumentoFiscal == null)
                cargaNFeParaEmissaoNFSManual.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("55");

            Servicos.Embarcador.NFSe.NFSManual.ValidarExistenciaEInserirDocumentoParaEmissaoNFSManual(cargaNFeParaEmissaoNFSManual, repCargaNFeParaEmissaoNFSManual, unitOfWork);

            return cargaNFeParaEmissaoNFSManual;
        }

        public bool AverbaCargaNFe(Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual, List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> apolicesSeguro, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Enumeradores.FormaAverbacaoCTE forma)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);
            if (configuracaoTMS.UtilizaEmissaoMultimodal && !repCargaPedido.GerarAverbacaoCarga(cargaDocumentoParaEmissaoNFSManual.Carga.Codigo))
                return true;

            if (cargaDocumentoParaEmissaoNFSManual.PedidoXMLNotaFiscal != null) //&& cargaCTE.CTe.ModeloDocumentoFiscal.AverbarDocumento)
            {
                foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao seguro in apolicesSeguro)
                {
                    if (seguro.ApoliceSeguro.SeguradoraAverbacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido)
                    {
                        if (seguro.ApoliceSeguro.SeguradoraAverbacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM)
                        {
                            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(seguro.ApoliceSeguro.Codigo);
                            if (averbacaoATM?.AverbarNFeQuandoCargaPossuirNFSManual ?? false)
                            {
                                Dominio.Entidades.AverbacaoCTe averbaCTe = new Dominio.Entidades.AverbacaoCTe
                                {
                                    Carga = cargaDocumentoParaEmissaoNFSManual.Carga,
                                    XMLNotaFiscal = cargaDocumentoParaEmissaoNFSManual.PedidoXMLNotaFiscal.XMLNotaFiscal,
                                    CargaDocumentoParaEmissaoNFSManual = cargaDocumentoParaEmissaoNFSManual,
                                    ApoliceSeguroAverbacao = seguro,
                                    Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao,
                                    SeguradoraAverbacao = Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido,
                                    Desconto = seguro.Desconto.HasValue && seguro.Desconto.Value > 0 ? cargaDocumentoParaEmissaoNFSManual.ValorFrete * (seguro.Desconto.Value / 100) : 0,
                                    Percentual = seguro.Desconto,
                                    Forma = forma,
                                    Status = Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao
                                };
                                repAverbacaoCTe.Inserir(averbaCTe);
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void CriarCTePendenteEmissaoManualDeNFS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade localidadePrestacao, decimal valorFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaNFeParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaNFeParaEmissaoNFSManual = new Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual();
            cargaNFeParaEmissaoNFSManual.Carga = carga;
            cargaNFeParaEmissaoNFSManual.CargaOrigem = pedidoCTeParaSubContratacao.CargaPedido.CargaOrigem;
            cargaNFeParaEmissaoNFSManual.LocalidadePrestacao = localidadePrestacao;
            cargaNFeParaEmissaoNFSManual.PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao;
            cargaNFeParaEmissaoNFSManual.Tomador = tomador;
            cargaNFeParaEmissaoNFSManual.ValorFrete = valorFrete;
            cargaNFeParaEmissaoNFSManual.Peso = pedidoCTeParaSubContratacao.CTeTerceiro.CTeTerceiroQuantidades.Sum(p => p.Quantidade);
            cargaNFeParaEmissaoNFSManual.Chave = pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso;
            cargaNFeParaEmissaoNFSManual.Descricao = "";
            cargaNFeParaEmissaoNFSManual.Serie = pedidoCTeParaSubContratacao.CTeTerceiro.Serie;
            cargaNFeParaEmissaoNFSManual.Numero = pedidoCTeParaSubContratacao.CTeTerceiro.Numero;
            cargaNFeParaEmissaoNFSManual.DataEmissao = pedidoCTeParaSubContratacao.CTeTerceiro.DataEmissao;
            cargaNFeParaEmissaoNFSManual.Remetente = pedidoCTeParaSubContratacao.CTeTerceiro.Remetente.Cliente;
            cargaNFeParaEmissaoNFSManual.Destinatario = pedidoCTeParaSubContratacao.CTeTerceiro.Destinatario.Cliente;
            cargaNFeParaEmissaoNFSManual.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.CTe);
            cargaNFeParaEmissaoNFSManual.Moeda = pedidoCTeParaSubContratacao.Moeda;
            cargaNFeParaEmissaoNFSManual.ValorTotalMoeda = (pedidoCTeParaSubContratacao.ValorTotalMoeda ?? 0m) + pedidoCTeParaSubContratacao.ValorTotalMoedaComponentes;
            cargaNFeParaEmissaoNFSManual.ValorCotacaoMoeda = pedidoCTeParaSubContratacao.ValorCotacaoMoeda;

            cargaNFeParaEmissaoNFSManual.NumeroPedidoCliente = pedidoCTeParaSubContratacao.CargaPedido?.Pedido?.CodigoPedidoCliente;

            Servicos.Embarcador.NFSe.NFSManual.ValidarExistenciaEInserirDocumentoParaEmissaoNFSManual(cargaNFeParaEmissaoNFSManual, repCargaNFeParaEmissaoNFSManual, unitOfWork);
        }

        public void CriarCargaCTePendenteEmissaoManualDeNFS(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Localidade localidadePrestacao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = null)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaNFeParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaNFeParaEmissaoNFSManual = new Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual
            {
                Carga = carga,
                CargaOrigem = cargaCTe.CargaOrigem,
                CargaOcorrencia = cargaOcorrencia,
                LocalidadePrestacao = localidadePrestacao,
                CargaCTe = cargaCTe,
                Tomador = cargaCTe.CTe.TomadorPagador.Cliente,
                ValorFrete = cargaOcorrencia?.ValorOcorrencia ?? cargaCTe.CTe.ValorAReceber,
                Peso = repInformacaoCargaCTE.ObterPesoKg(cargaCTe.CTe.Codigo),
                Descricao = "",
                Serie = cargaCTe.CTe.Serie.Numero.ToString(),
                Numero = cargaCTe.CTe.Numero,
                DataEmissao = cargaCTe.CTe.DataEmissao.Value,
                Remetente = cargaCTe.CTe.Remetente?.Cliente ?? cargaCTe.CTe.Expedidor?.Cliente,
                Destinatario = cargaCTe.CTe.Destinatario?.Cliente ?? cargaCTe.CTe.Recebedor?.Cliente,
                ModeloDocumentoFiscal = cargaCTe.CTe.ModeloDocumentoFiscal,
                Moeda = cargaCTe.CTe.Moeda,
                ValorCotacaoMoeda = cargaOcorrencia?.ValorCotacaoMoeda ?? cargaCTe.CTe.ValorCotacaoMoeda,
                ValorTotalMoeda = cargaOcorrencia?.ValorTotalMoeda ?? cargaCTe.CTe.ValorTotalMoeda,
            };

            Servicos.Embarcador.NFSe.NFSManual.ValidarExistenciaEInserirDocumentoParaEmissaoNFSManual(cargaNFeParaEmissaoNFSManual, repCargaNFeParaEmissaoNFSManual, unitOfWork);
        }

        public void CriarCargaCTePendenteEmissaoManualDeNFSManual(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Localidade localidadePrestacao, Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaNFeParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = repCargaNFeParaEmissaoNFSManual.BuscarPorOcorrenciaECargaCTe(cargaOcorrencia.Codigo, cargaCTe.Codigo);

            if (cargaDocumentoParaEmissaoNFSManual == null)
            {
                Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaNFeParaEmissaoNFSManual = new Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual()
                {
                    Carga = carga,
                    CargaOrigem = cargaCTe.CargaOrigem,
                    CargaOcorrencia = cargaOcorrencia,
                    LocalidadePrestacao = localidadePrestacao,
                    CargaCTe = cargaCTe,
                    Tomador = cargaCTe.CTe.TomadorPagador.Cliente,
                    ValorFrete = cargaCTeComplementoInfo.ValorComplemento,
                    Peso = repInformacaoCargaCTE.ObterPesoKg(cargaCTe.CTe.Codigo),
                    Descricao = "",
                    Serie = cargaCTe.CTe.Serie.Numero.ToString(),
                    Numero = cargaCTe.CTe.Numero,
                    DataEmissao = cargaCTe.CTe.DataEmissao.Value,
                    Remetente = cargaCTe.CTe.Remetente.Cliente,
                    Destinatario = cargaCTe.CTe.Destinatario.Cliente,
                    ModeloDocumentoFiscal = cargaCTe.CTe.ModeloDocumentoFiscal,
                    Moeda = cargaCTe.CTe.Moeda,
                    ValorCotacaoMoeda = cargaCTe.CTe.ValorCotacaoMoeda,
                    ValorTotalMoeda = cargaCTe.CTe.ValorTotalMoeda
                };

                Servicos.Embarcador.NFSe.NFSManual.ValidarExistenciaEInserirDocumentoParaEmissaoNFSManual(cargaNFeParaEmissaoNFSManual, repCargaNFeParaEmissaoNFSManual, unitOfWork);
            }
        }

        public void CriarCargaCTePendenteEmissaoManualDeNFSManual(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Dominio.Entidades.Localidade localidadePrestacao, Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaNFeParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            for (int i = 0; i < cargas.Count(); i++)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCarga(cargas[i].Codigo);

                for (int j = 0; j < cargasCTe.Count(); j++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = repCargaNFeParaEmissaoNFSManual.BuscarPorOcorrenciaECargaCTe(cargaOcorrencia.Codigo, cargasCTe[i].Codigo);

                    if (cargaDocumentoParaEmissaoNFSManual == null)
                    {
                        Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaNFeParaEmissaoNFSManual = new Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual()
                        {
                            Carga = cargasCTe[i].Carga,
                            CargaOrigem = cargasCTe[i].CargaOrigem,
                            CargaOcorrencia = cargaOcorrencia,
                            LocalidadePrestacao = localidadePrestacao,
                            CargaCTe = cargasCTe[i],
                            Tomador = cargasCTe[i].CTe.TomadorPagador.Cliente,
                            ValorFrete = cargaCTeComplementoInfo.ValorComplemento,
                            Peso = repInformacaoCargaCTE.ObterPesoKg(cargasCTe[i].CTe.Codigo),
                            Descricao = "",
                            Serie = cargasCTe[i].CTe.Serie.Numero.ToString(),
                            Numero = cargasCTe[i].CTe.Numero,
                            DataEmissao = cargasCTe[i].CTe.DataEmissao.Value,
                            Remetente = cargasCTe[i].CTe.Remetente.Cliente,
                            Destinatario = cargasCTe[i].CTe.Destinatario.Cliente,
                            ModeloDocumentoFiscal = cargasCTe[i].CTe.ModeloDocumentoFiscal,
                            Moeda = cargasCTe[i].CTe.Moeda,
                            ValorCotacaoMoeda = cargasCTe[i].CTe.ValorCotacaoMoeda,
                            ValorTotalMoeda = cargasCTe[i].CTe.ValorTotalMoeda
                        };

                        Servicos.Embarcador.NFSe.NFSManual.ValidarExistenciaEInserirDocumentoParaEmissaoNFSManual(cargaNFeParaEmissaoNFSManual, repCargaNFeParaEmissaoNFSManual, unitOfWork);
                    }
                }
            }
        }
    }
}
