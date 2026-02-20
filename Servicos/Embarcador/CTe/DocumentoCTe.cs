using Dominio.Excecoes.Embarcador;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CTe
{
    public class DocumentoCTe : ServicoBase
    {
        public DocumentoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal CriarDocumentoParaXMLNotaFiscal(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.DocumentosCTE documento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);



            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
            if (documento.ModeloDocumentoFiscal.Numero == "55")
                xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe;
            else if (documento.ModeloDocumentoFiscal.Numero == "01")
                xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NotaFiscal;
            else
                xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros;

            xmlNotaFiscal.BaseCalculoICMS = documento.BaseCalculoICMS;
            xmlNotaFiscal.Chave = documento.ChaveNFE;
            xmlNotaFiscal.TipoEmissao = Utilidades.Chave.ObterTipoEmissao(xmlNotaFiscal.Chave).ToString().ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal>();
            xmlNotaFiscal.CNPJTranposrtador = cargaCTe.Carga.Empresa != null ? cargaCTe.Carga.Empresa.CNPJ_SemFormato : "";
            xmlNotaFiscal.Empresa = cargaCTe.Carga.Empresa;
            xmlNotaFiscal.DataEmissao = documento.DataEmissao;

            if (!cargaCTe.CTe.Destinatario.Exterior)
                xmlNotaFiscal.Destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(cargaCTe.CTe.Destinatario.CPF_CNPJ_SemFormato));
            else
            {
                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorRazaoExterior(cargaCTe.CTe.Destinatario.Nome, cargaCTe.CTe.Destinatario.Endereco);

                if (destinatario != null)
                    xmlNotaFiscal.Destinatario = destinatario;
                else
                {
                    Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

                    destinatario = new Dominio.Entidades.Cliente();
                    destinatario.Nome = cargaCTe.CTe.Destinatario.Nome;
                    destinatario.Endereco = cargaCTe.CTe.Destinatario.Endereco;
                    destinatario.Bairro = cargaCTe.CTe.Destinatario.Bairro;
                    destinatario.Cidade = cargaCTe.CTe.Destinatario.Cidade;
                    destinatario.Complemento = cargaCTe.CTe.Destinatario.Complemento;
                    destinatario.Email = cargaCTe.CTe.Destinatario.Email;
                    destinatario.Numero = cargaCTe.CTe.Destinatario.Numero;
                    destinatario.Cidade = cargaCTe.CTe.Destinatario.Cidade;
                    destinatario.Pais = cargaCTe.CTe.Destinatario.Pais;
                    destinatario.Tipo = "E";
                    destinatario.CPF_CNPJ = repCliente.BuscarPorProximoExterior();
                    destinatario.Localidade = repLocalidade.BuscarPorCodigoIBGE(9999999);
                    destinatario.Atividade = repAtividade.BuscarPorCodigo(1);
                    destinatario.Ativo = true;
                    repCliente.Inserir(destinatario);

                    xmlNotaFiscal.Destinatario = destinatario;
                }
            }

            xmlNotaFiscal.Modelo = documento.ModeloDocumentoFiscal.Numero;
            xmlNotaFiscal.nfAtiva = true;
            xmlNotaFiscal.Numero = !string.IsNullOrWhiteSpace(documento.Numero) ? int.Parse(documento.Numero) : 0;
            xmlNotaFiscal.Peso = documento.Peso;
            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;

            if (!cargaCTe.CTe.Remetente.Exterior)
                xmlNotaFiscal.Emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(cargaCTe.CTe.Remetente.CPF_CNPJ_SemFormato));
            else
            {
                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorRazaoExterior(cargaCTe.CTe.Remetente.Nome, cargaCTe.CTe.Remetente.Endereco);

                if (remetente != null)
                    xmlNotaFiscal.Emitente = remetente;
                else
                {
                    Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

                    remetente = new Dominio.Entidades.Cliente();
                    remetente.Nome = cargaCTe.CTe.Remetente.Nome;
                    remetente.Endereco = cargaCTe.CTe.Remetente.Endereco;
                    remetente.Bairro = cargaCTe.CTe.Remetente.Bairro;
                    remetente.Cidade = cargaCTe.CTe.Remetente.Cidade;
                    remetente.Complemento = cargaCTe.CTe.Remetente.Complemento;
                    remetente.Email = cargaCTe.CTe.Remetente.Email;
                    remetente.Numero = cargaCTe.CTe.Remetente.Numero;
                    remetente.Cidade = cargaCTe.CTe.Remetente.Cidade;
                    remetente.Pais = cargaCTe.CTe.Remetente.Pais;
                    remetente.Tipo = "E";
                    remetente.CPF_CNPJ = repCliente.BuscarPorProximoExterior();
                    remetente.Localidade = repLocalidade.BuscarPorCodigoIBGE(9999999);
                    remetente.Atividade = repAtividade.BuscarPorCodigo(1);
                    remetente.Ativo = true;
                    repCliente.Inserir(remetente);

                    xmlNotaFiscal.Emitente = remetente;
                }
            }

            xmlNotaFiscal.Serie = documento.Serie;
            xmlNotaFiscal.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
            xmlNotaFiscal.Descricao = documento.Descricao;
            xmlNotaFiscal.Valor = documento.Valor;
            xmlNotaFiscal.ValorICMS = documento.ValorICMS;
            xmlNotaFiscal.ValorST = documento.ValorICMSST;
            xmlNotaFiscal.ValorTotalProdutos = documento.ValorProdutos;
            xmlNotaFiscal.Volumes = documento.Volume;
            xmlNotaFiscal.XML = "";
            xmlNotaFiscal.PlacaVeiculoNotaFiscal = "";
            xmlNotaFiscal.DataRecebimento = DateTime.Now;

            repXMLNotaFiscal.Inserir(xmlNotaFiscal);

            return xmlNotaFiscal;
        }

        public void SetarEntidadeCTeParaDocumentos(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cte, ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao, Repositorio.UnitOfWork unitOfWork)
        {

            cteIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal>();
            cteIntegracao.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
            cteIntegracao.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

            foreach (var documento in cte.CTesTerceiroNFes)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.NFe nfe = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe();
                nfe.Chave = documento.Chave;
                int numeroNf = 0;
                int.TryParse(documento.Numero, out numeroNf);
                nfe.Numero = numeroNf;
                nfe.Peso = documento.Peso;
                nfe.DataEmissao = documento.DataEmissao.HasValue ? documento.DataEmissao.Value : DateTime.Now;
                nfe.NumeroReferenciaEDI = documento.NumeroReferenciaEDI;
                nfe.NumeroControleCliente = documento.NumeroControleCliente;
                nfe.PINSuframa = documento.PINSuframa;
                nfe.NCMPredominante = documento.NCM;
                nfe.Valor = documento.ValorTotal;
                cteIntegracao.NFEs.Add(nfe);
            }

            if (cte.CTesTerceiroOutrosDocumentos != null)
            {
                foreach (var documento in cte.CTesTerceiroOutrosDocumentos)
                {
                    cteIntegracao.OutrosDocumentos.Add(new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento()
                    {
                        CFOP = "",
                        DataEmissao = documento.CTeTerceiro.DataEmissao,
                        NCMPredominante = documento.NCM,
                        Numero = documento.Numero,
                        NumeroControleCliente = "",
                        NumeroReferenciaEDI = "",
                        Peso = documento.CTeTerceiro.Peso,
                        PINSuframa = "",
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Outros,
                        Valor = documento.Valor,
                        Descricao = documento.Numero
                    });
                }
            }
        }


        public void SetarEntidadeCTeParaDocumentos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao, Repositorio.UnitOfWork unitOfWork)
        {

            cteIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal>();
            cteIntegracao.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
            cteIntegracao.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

            foreach (var documento in cte.Documentos)
            {
                if (documento?.ModeloDocumentoFiscal != null)
                {
                    if (documento?.ModeloDocumentoFiscal.Numero == "55")
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.NFe nfe = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe();
                        nfe.Chave = documento.ChaveNFE;
                        int numeroNf = 0;
                        int.TryParse(documento.Numero, out numeroNf);
                        nfe.Numero = numeroNf;
                        nfe.Peso = documento.Peso;
                        nfe.DataEmissao = documento.DataEmissao;
                        nfe.NumeroReferenciaEDI = documento.NumeroReferenciaEDI;
                        nfe.NumeroControleCliente = documento.NumeroControleCliente;
                        nfe.PINSuframa = documento.PINSuframa;
                        nfe.NCMPredominante = documento.NCMPredominante;
                        nfe.Valor = documento.Valor;
                        cteIntegracao.NFEs.Add(nfe);
                    }
                    else if (documento?.ModeloDocumentoFiscal.Numero == "00" || documento.ModeloDocumentoFiscal.Numero == "99" || documento.ModeloDocumentoFiscal.Numero == "10")
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDocumento = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento();

                        outroDocumento.DataEmissao = documento.DataEmissao;
                        outroDocumento.Descricao = documento.Descricao;
                        outroDocumento.Numero = documento.Numero;
                        outroDocumento.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento)int.Parse(documento.ModeloDocumentoFiscal.Numero);
                        outroDocumento.Valor = documento.Valor;
                        cteIntegracao.OutrosDocumentos.Add(outroDocumento);
                    }
                    else if (documento?.ModeloDocumentoFiscal.Numero == "01" || documento.ModeloDocumentoFiscal.Numero == "04")
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal();
                        notaFiscal.BaseCalculoICMS = documento.BaseCalculoICMS;
                        notaFiscal.BaseCalculoICMSST = documento.BaseCalculoICMSST;
                        notaFiscal.CFOP = documento.CFOP;
                        notaFiscal.DataEmissao = documento.DataEmissao;
                        notaFiscal.ModeloNotaFiscal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloNotaFiscal)int.Parse(documento.ModeloDocumentoFiscal.Numero);
                        notaFiscal.Numero = (string)documento.Numero;
                        notaFiscal.Serie = (string)documento.Serie;
                        notaFiscal.Valor = documento.Valor;
                        notaFiscal.ValorICMS = documento.ValorICMS;
                        notaFiscal.ValorICMSST = documento.ValorICMSST;
                        notaFiscal.Peso = documento.Peso;
                        notaFiscal.NumeroReferenciaEDI = documento.NumeroReferenciaEDI;
                        notaFiscal.NumeroControleCliente = documento.NumeroControleCliente;
                        notaFiscal.PINSuframa = documento.PINSuframa;
                        notaFiscal.ValorProdutos = documento.ValorProdutos;
                        notaFiscal.PIN = documento.PINSuframa;
                        notaFiscal.NCMPredominante = documento.NCMPredominante;
                        cteIntegracao.NotasFiscais.Add(notaFiscal);
                    }
                }
            }

        }

        public void SetarDynamicParaDocumentos(dynamic dynCTe, ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, Repositorio.UnitOfWork unitOfWork)
        {
            cte.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal>();
            cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
            cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

            foreach (var documento in dynCTe.Documentos)
            {
                decimal valorNotaFiscal = 0, peso = 0;
                DateTime dataEmissao = DateTime.Now;

                if ((Dominio.Enumeradores.TipoDocumentoCTe)documento.TipoDocumento == Dominio.Enumeradores.TipoDocumentoCTe.NFe)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.NFe nfe = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe();
                    nfe.Chave = ((string)documento.Chave).Trim().Replace(" ", "");
                    nfe.Numero = ((string)documento.Numero).ToInt();

                    DateTime.TryParseExact((string)documento.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                    if (dataEmissao == DateTime.MinValue)
                        DateTime.TryParseExact((string)documento.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                    if (dataEmissao == DateTime.MinValue)
                        dataEmissao = DateTime.Now;

                    nfe.DataEmissao = dataEmissao;
                    if (documento.ValorNotaFiscal != null && !string.IsNullOrWhiteSpace(documento.ValorNotaFiscal.ToString()))
                        decimal.TryParse(documento.ValorNotaFiscal.ToString(), out valorNotaFiscal);
                    if (documento.Peso != null && !string.IsNullOrWhiteSpace(documento.Peso.ToString()))
                        decimal.TryParse(documento.Peso.ToString(), out peso);
                    nfe.Peso = peso;
                    nfe.Valor = valorNotaFiscal;
                    nfe.NumeroReferenciaEDI = (string)documento.NumeroReferenciaEDI;
                    nfe.NumeroControleCliente = (string)documento.NumeroControleCliente;
                    nfe.PINSuframa = (string)documento.PINSuframa;
                    nfe.NCMPredominante = (string)documento.NCMPredominante;
                    nfe.CFOP = (string)documento.CFOP;

                    cte.NFEs.Add(nfe);
                }
                else if ((Dominio.Enumeradores.TipoDocumentoCTe)documento.TipoDocumento == Dominio.Enumeradores.TipoDocumentoCTe.Outros)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDocumento = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento();

                    DateTime.TryParseExact((string)documento.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                    if (dataEmissao == DateTime.MinValue)
                        DateTime.TryParseExact((string)documento.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                    if (dataEmissao == DateTime.MinValue)
                        dataEmissao = DateTime.Now;

                    outroDocumento.DataEmissao = dataEmissao;
                    outroDocumento.Descricao = (string)documento.Descricao;
                    outroDocumento.Numero = (string)documento.Numero;
                    outroDocumento.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento)int.Parse((string)documento.Modelo);
                    decimal.TryParse(documento.ValorNotaFiscal.ToString(), out valorNotaFiscal);
                    outroDocumento.Valor = valorNotaFiscal;
                    outroDocumento.NumeroReferenciaEDI = (string)documento.NumeroReferenciaEDI;
                    outroDocumento.NumeroControleCliente = (string)documento.NumeroControleCliente;
                    outroDocumento.PINSuframa = (string)documento.PINSuframa;
                    outroDocumento.NCMPredominante = (string)documento.NCMPredominante;
                    outroDocumento.CFOP = (string)documento.CFOP;
                    decimal.TryParse(documento.Peso.ToString(), out peso);
                    outroDocumento.Peso = peso;

                    cte.OutrosDocumentos.Add(outroDocumento);
                }
                else if ((Dominio.Enumeradores.TipoDocumentoCTe)documento.TipoDocumento == Dominio.Enumeradores.TipoDocumentoCTe.NF)
                {

                    decimal baseCalculoICMS, baseCalculoICMSST, valorProdutos, ValorICMS, ValorICMSST;

                    Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal();
                    decimal.TryParse(documento.BaseCalculoICMS.ToString(), out baseCalculoICMS);
                    notaFiscal.BaseCalculoICMS = baseCalculoICMS;
                    decimal.TryParse(documento.BaseCalculoICMSST.ToString(), out baseCalculoICMSST);
                    notaFiscal.BaseCalculoICMSST = baseCalculoICMSST;
                    notaFiscal.CFOP = (string)documento.CFOP;

                    DateTime.TryParseExact((string)documento.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                    if (dataEmissao == DateTime.MinValue)
                        DateTime.TryParseExact((string)documento.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                    if (dataEmissao == DateTime.MinValue)
                        dataEmissao = DateTime.Now;

                    notaFiscal.DataEmissao = dataEmissao;
                    notaFiscal.ModeloNotaFiscal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloNotaFiscal)int.Parse((string)documento.Modelo);
                    notaFiscal.Numero = (string)documento.Numero;
                    notaFiscal.Serie = (string)documento.Serie;

                    decimal.TryParse(documento.ValorNotaFiscal.ToString(), out valorNotaFiscal);
                    notaFiscal.Valor = valorNotaFiscal;
                    decimal.TryParse(documento.ValorICMS.ToString(), out ValorICMS);
                    notaFiscal.ValorICMS = ValorICMS;
                    decimal.TryParse(documento.ValorICMSST.ToString(), out ValorICMSST);
                    notaFiscal.ValorICMSST = ValorICMSST;
                    decimal.TryParse(documento.Peso.ToString(), out peso);
                    notaFiscal.Peso = peso;
                    decimal.TryParse(documento.ValorProdutos.ToString(), out valorProdutos);
                    notaFiscal.ValorProdutos = valorProdutos;
                    notaFiscal.PIN = (string)documento.PIN;
                    notaFiscal.NumeroReferenciaEDI = (string)documento.NumeroReferenciaEDI;
                    notaFiscal.NumeroControleCliente = (string)documento.NumeroControleCliente;
                    notaFiscal.PINSuframa = (string)documento.PINSuframa;
                    notaFiscal.NCMPredominante = (string)documento.NCMPredominante;

                    cte.NotasFiscais.Add(notaFiscal);
                }
            }
        }

        public void SetarDynamicParaEntregasSimplificado(dynamic dynCTe, ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.CTe.ComponenteFrete serComponenteFrete = new ComponenteFrete(unitOfWork);
            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Localidades.Localidade(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            cte.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal>();
            cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
            cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();
            cte.Entregas = new List<Dominio.ObjetosDeValor.Embarcador.CTe.EntregaSimplificado>();

            foreach (var entrega in dynCTe.EntregasSimplificado)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.EntregaSimplificado entregaSimplificado = new Dominio.ObjetosDeValor.Embarcador.CTe.EntregaSimplificado();

                entregaSimplificado.Origem = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo((int)entrega.CodigoLocalidadeOrigem));
                entregaSimplificado.Destino = serLocalidade.ConverterObjetoLocalidade(repLocalidade.BuscarPorCodigo((int)entrega.CodigoLocalidadeDestino));
                entregaSimplificado.ValorFrete = entrega.ValorFrete;

                decimal valorFrete, valorPrestacaoServico, valorReceber;

                decimal.TryParse(entrega.ValorFrete.ToString(), out valorFrete);
                entregaSimplificado.ValorFrete = valorFrete;
                decimal.TryParse(entrega.ValorPrestacaoServico.ToString(), out valorPrestacaoServico);
                decimal.TryParse(entrega.ValorAReceber.ToString(), out valorReceber);

                entregaSimplificado.ComponentesAdicionais = serComponenteFrete.ConverterDynamicParaComponenteFrete(dynCTe.EntregasSimplificadoComponentesPrestacaoServico, (int)entrega.CodigoLocalidadeOrigem, (int)entrega.CodigoLocalidadeDestino);

                if (cte.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho || cte.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                {
                    entregaSimplificado.DocumentosAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior>();

                    foreach (var documento in dynCTe.EntregasSimplificadoDocumentosTransporteAnterior)
                    {
                        if (documento.CodigoLocalidadeOrigem == entrega.CodigoLocalidadeOrigem && documento.CodigoLocalidadeDestino == entrega.CodigoLocalidadeDestino)
                        {
                            Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior documentoAnterior = new Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior();
                            documentoAnterior.Emitente = serPessoa.ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ((double)documento.CodigoEmitente));
                            documentoAnterior.ChaveAcesso = (string)documento.Chave;
                            entregaSimplificado.DocumentosAnteriores.Add(documentoAnterior);
                        }
                    }

                    if (entregaSimplificado.DocumentosAnteriores.Count() == 0)
                        throw new ServicoException($"Entrega de origem {entregaSimplificado.Origem.Descricao} e destino {entregaSimplificado.Destino.Descricao} não possui documentos informados, favor verificar.");
                }
                else
                {
                    entregaSimplificado.NFes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();

                    foreach (var documento in dynCTe.EntregasSimplificadoDocumentos)
                    {
                        if (documento.CodigoLocalidadeOrigem == entrega.CodigoLocalidadeOrigem && documento.CodigoLocalidadeDestino == entrega.CodigoLocalidadeDestino)
                        {
                            decimal valorNotaFiscal = 0, peso = 0;
                            DateTime dataEmissao = DateTime.Now;

                            Dominio.ObjetosDeValor.Embarcador.CTe.NFe nfe = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe();
                            nfe.Chave = ((string)documento.Chave).Trim().Replace(" ", "");
                            nfe.Numero = ((string)documento.Numero).ToInt();

                            DateTime.TryParseExact((string)documento.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                            if (dataEmissao == DateTime.MinValue)
                                DateTime.TryParseExact((string)documento.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                            if (dataEmissao == DateTime.MinValue)
                                dataEmissao = DateTime.Now;

                            nfe.DataEmissao = dataEmissao;
                            if (documento.ValorNotaFiscal != null && !string.IsNullOrWhiteSpace(documento.ValorNotaFiscal.ToString()))
                                decimal.TryParse(documento.ValorNotaFiscal.ToString(), out valorNotaFiscal);
                            if (documento.Peso != null && !string.IsNullOrWhiteSpace(documento.Peso.ToString()))
                                decimal.TryParse(documento.Peso.ToString(), out peso);
                            nfe.Peso = peso;
                            nfe.Valor = valorNotaFiscal;
                            nfe.NumeroReferenciaEDI = (string)documento.NumeroReferenciaEDI;
                            nfe.NumeroControleCliente = (string)documento.NumeroControleCliente;
                            nfe.PINSuframa = (string)documento.PINSuframa;
                            nfe.NCMPredominante = (string)documento.NCMPredominante;
                            nfe.CFOP = (string)documento.CFOP;

                            entregaSimplificado.NFes.Add(nfe);
                        }
                    }

                    if (entregaSimplificado.NFes.Count() == 0)
                        throw new ServicoException($"Entrega de origem {entregaSimplificado.Origem.Descricao} e destino {entregaSimplificado.Destino.Descricao} não possui documentos informados, favor verificar.");
                }

                cte.Entregas.Add(entregaSimplificado);
            }
        }

        public void SalvarInformacoesDocumentos(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.DocumentosCTE repDocumento = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            if (cte.Codigo > 0)
                repDocumento.DeletarPorCTe(cte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.NFe nfe in cteIntegracao.NFEs)
            {
                Dominio.Entidades.DocumentosCTE documento = new Dominio.Entidades.DocumentosCTE();
                documento.CTE = cte;
                documento.ChaveNFE = nfe.Chave;
                documento.Valor = nfe.Valor;
                documento.Numero = nfe.Numero.ToString();
                documento.Peso = nfe.Peso;
                documento.Serie = "";
                documento.DataEmissao = nfe.DataEmissao;
                documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo("55");
                documento.NumeroReferenciaEDI = (string)nfe.NumeroReferenciaEDI;
                documento.NumeroControleCliente = (string)nfe.NumeroControleCliente;
                documento.PINSuframa = (string)nfe.PINSuframa;
                documento.NCMPredominante = (string)nfe.NCMPredominante;

                if (!string.IsNullOrWhiteSpace(documento.ChaveNFE))
                    documento.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(documento.ChaveNFE);

                repDocumento.Inserir(documento);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDoc in cteIntegracao.OutrosDocumentos)
            {
                Dominio.Entidades.DocumentosCTE documento = new Dominio.Entidades.DocumentosCTE();
                documento.CTE = cte;
                documento.Descricao = outroDoc.Descricao;
                documento.Numero = outroDoc.Numero;

                string modelo = "";
                if (outroDoc.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Declaracao)
                    modelo = "00";
                else
                    modelo = "99";
                documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo(modelo);

                documento.ChaveNFE = "";
                documento.DataEmissao = outroDoc.DataEmissao;
                documento.Valor = outroDoc.Valor;
                documento.NumeroReferenciaEDI = outroDoc.NumeroReferenciaEDI;
                documento.NumeroControleCliente = outroDoc.NumeroControleCliente;
                documento.PINSuframa = outroDoc.PINSuframa;
                documento.NCMPredominante = outroDoc.NCMPredominante;
                if (!string.IsNullOrWhiteSpace(documento.ChaveNFE))
                    documento.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(documento.ChaveNFE);
                repDocumento.Inserir(documento);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal in cteIntegracao.NotasFiscais)
            {
                Dominio.Entidades.DocumentosCTE documento = new Dominio.Entidades.DocumentosCTE();

                documento.BaseCalculoICMS = notaFiscal.BaseCalculoICMS;
                documento.BaseCalculoICMSST = notaFiscal.BaseCalculoICMSST;
                documento.CFOP = notaFiscal.CFOP;
                documento.ChaveNFE = "";
                documento.CTE = cte;
                documento.DataEmissao = notaFiscal.DataEmissao;
                string modelo = "";
                if (notaFiscal.ModeloNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloNotaFiscal.NFModelo01Avulsa)
                    modelo = "01";
                else
                    modelo = "04";

                documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo(modelo);
                documento.Numero = notaFiscal.Numero;
                documento.Peso = notaFiscal.Peso;
                documento.PINSuframa = notaFiscal.PIN;
                documento.NCMPredominante = notaFiscal.NCMPredominante;
                documento.Serie = notaFiscal.Serie;
                documento.Valor = notaFiscal.Valor;
                documento.ValorICMS = notaFiscal.ValorICMS;
                documento.ValorICMSST = notaFiscal.ValorICMSST;
                documento.ValorProdutos = notaFiscal.ValorProdutos;
                //documento.Volume = notaFiscal.;
                documento.NumeroReferenciaEDI = notaFiscal.NumeroReferenciaEDI;
                documento.NumeroControleCliente = notaFiscal.NumeroControleCliente;
                documento.PINSuframa = notaFiscal.PINSuframa;
                if (!string.IsNullOrWhiteSpace(documento.ChaveNFE))
                    documento.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(documento.ChaveNFE);
                repDocumento.Inserir(documento);
            }
        }

        public void SalvarInformacoesEntregasSimplificado(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.DocumentosCTE repDocumento = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoDeTransporteAnteriorCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.EntregaCTe repEntregaCTe = new Repositorio.EntregaCTe(unitOfWork);
            Repositorio.EntregaCTeDocumento repEntregaCTeDocumento = new Repositorio.EntregaCTeDocumento(unitOfWork);
            Repositorio.EntregaCTeDocumentoTransporteAnterior repEntregaCTeDocumentoTransporteAnterior = new Repositorio.EntregaCTeDocumentoTransporteAnterior(unitOfWork);
            Repositorio.EntregaCTeComponentePrestacao repEntregaCTeComponentePrestacao = new Repositorio.EntregaCTeComponentePrestacao(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (cte.Codigo > 0)
            {
                List<Dominio.Entidades.EntregaCTe> entregas = repEntregaCTe.BuscarPorCTe(cte.Codigo);

                foreach (Dominio.Entidades.EntregaCTe entrega in entregas)
                {
                    repEntregaCTeComponentePrestacao.DeletarPorEntrega(entrega.Codigo);
                    repEntregaCTeDocumento.DeletarPorEntrega(entrega.Codigo);
                    repEntregaCTeDocumentoTransporteAnterior.DeletarPorEntrega(entrega.Codigo);
                }

                repEntregaCTe.DeletarPorCTe(cte.Codigo);
                repDocumento.DeletarPorCTe(cte.Codigo);
                repDocumentoDeTransporteAnteriorCTe.DeletarPorCTe(cte.Codigo);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.EntregaSimplificado entrega in cteIntegracao.Entregas)
            {
                Dominio.Entidades.EntregaCTe entregaCTe = new Dominio.Entidades.EntregaCTe();
                entregaCTe.CTE = cte;
                entregaCTe.Origem = repLocalidade.BuscarPorCodigo(entrega.Origem.Codigo);
                entregaCTe.Destino = repLocalidade.BuscarPorCodigo(entrega.Destino.Codigo);
                entregaCTe.ValorFrete = entrega.ValorFrete;
                entregaCTe.ValorPrestacaoServico = entrega.ValorPrestacaoServico;
                entregaCTe.ValorAReceber = entrega.ValorAReceber;
                repEntregaCTe.Inserir(entregaCTe);

                #region Componente Frete

                foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente in entrega.ComponentesAdicionais)
                {
                    if (componente.DescontarValorTotalAReceber)
                        continue;

                    Dominio.Entidades.EntregaCTeComponentePrestacao compPrest = new Dominio.Entidades.EntregaCTeComponentePrestacao();
                    compPrest.EntregaCTe = entregaCTe;
                    compPrest.ComponenteFrete = null;
                    compPrest.NomeCTe = componente.Descricao;
                    compPrest.Valor = componente.ValorComponente;
                    compPrest.IncluiNaBaseDeCalculoDoICMS = componente.IncluirBaseCalculoICMS;
                    compPrest.IncluiNoTotalAReceber = componente.IncluirTotalReceber;
                    repEntregaCTeComponentePrestacao.Inserir(compPrest);
                }

                #endregion Componente Frete

                #region Documentos

                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.NFe nfe in entrega.NFes)
                {
                    Dominio.Entidades.DocumentosCTE documento = new Dominio.Entidades.DocumentosCTE();
                    documento.CTE = cte;
                    documento.ChaveNFE = nfe.Chave;
                    documento.Valor = nfe.Valor;
                    documento.Numero = nfe.Numero.ToString();
                    documento.Peso = nfe.Peso;
                    documento.Serie = "";
                    documento.DataEmissao = nfe.DataEmissao;
                    documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo("55");
                    documento.NumeroReferenciaEDI = (string)nfe.NumeroReferenciaEDI;
                    documento.NumeroControleCliente = (string)nfe.NumeroControleCliente;
                    documento.PINSuframa = (string)nfe.PINSuframa;
                    documento.NCMPredominante = (string)nfe.NCMPredominante;
                    if (!string.IsNullOrWhiteSpace(documento.ChaveNFE))
                        documento.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(documento.ChaveNFE);
                    repDocumento.Inserir(documento);

                    Dominio.Entidades.EntregaCTeDocumento doc = new Dominio.Entidades.EntregaCTeDocumento();
                    doc.EntregaCTe = entregaCTe;
                    doc.DocumentosCTE = documento;
                    repEntregaCTeDocumento.Inserir(doc);
                }

                #endregion Documentos


                #region Documentos Transporte Anterior

                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior docTransAnt in entrega.DocumentosAnteriores)
                {
                    Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();
                    documento.Chave = Utilidades.String.OnlyNumbers(docTransAnt.ChaveAcesso);
                    documento.CTe = cte;
                    documento.Emissor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(docTransAnt.Emitente.CPFCNPJ)));
                    repDocumentoDeTransporteAnteriorCTe.Inserir(documento);

                    Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior doc = new Dominio.Entidades.EntregaCTeDocumentoTransporteAnterior();
                    doc.EntregaCTe = entregaCTe;
                    doc.DocumentoTransporteAnterior = documento;
                    repEntregaCTeDocumentoTransporteAnterior.Inserir(doc);
                }

                #endregion Documentos Transporte Anterior
            }
        }

        public void SalvarInformacoesDocumentosPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.DocumentosPreCTE repDocumentoPreCTe = new Repositorio.DocumentosPreCTE(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            if (preCTe.Codigo > 0)
                repDocumentoPreCTe.DeletarPorPreCTe(preCTe.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.NFe nfe in cteIntegracao.NFEs)
            {
                Dominio.Entidades.DocumentosPreCTE documento = new Dominio.Entidades.DocumentosPreCTE();
                documento.PreCTE = preCTe;
                documento.ChaveNFE = nfe.Chave;
                documento.Valor = nfe.Valor;
                documento.Numero = "";
                documento.Serie = "";
                documento.DataEmissao = nfe.DataEmissao;
                documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo("55");
                repDocumentoPreCTe.Inserir(documento);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDoc in cteIntegracao.OutrosDocumentos)
            {
                Dominio.Entidades.DocumentosPreCTE documento = new Dominio.Entidades.DocumentosPreCTE();
                documento.PreCTE = preCTe;
                documento.Descricao = outroDoc.Descricao;
                documento.Numero = outroDoc.Numero;

                string modelo = "";
                if (outroDoc.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento.Declaracao)
                    modelo = "00";
                else
                    modelo = "99";
                documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo(modelo);

                documento.ChaveNFE = "";
                documento.DataEmissao = outroDoc.DataEmissao;
                documento.Valor = outroDoc.Valor;
                repDocumentoPreCTe.Inserir(documento);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal in cteIntegracao.NotasFiscais)
            {
                Dominio.Entidades.DocumentosPreCTE documento = new Dominio.Entidades.DocumentosPreCTE();

                documento.BaseCalculoICMS = notaFiscal.BaseCalculoICMS;
                documento.BaseCalculoICMSST = notaFiscal.BaseCalculoICMSST;
                documento.CFOP = notaFiscal.CFOP;
                documento.ChaveNFE = "";
                documento.PreCTE = preCTe;
                documento.DataEmissao = notaFiscal.DataEmissao;
                string modelo = "";
                if (notaFiscal.ModeloNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloNotaFiscal.NFModelo01Avulsa)
                    modelo = "01";
                else
                    modelo = "04";

                documento.ModeloDocumentoFiscal = repModelo.BuscarPorModelo(modelo);
                documento.Numero = notaFiscal.Numero;
                documento.Peso = notaFiscal.Peso;
                documento.PINSuframa = notaFiscal.PIN;
                documento.NCMPredominante = notaFiscal.NCMPredominante;
                documento.Serie = notaFiscal.Serie;
                documento.Valor = notaFiscal.Valor;
                documento.ValorICMS = notaFiscal.ValorICMS;
                documento.ValorICMSST = notaFiscal.ValorICMSST;
                documento.ValorProdutos = notaFiscal.ValorProdutos;
                //documento.Volume = notaFiscal.;

                repDocumentoPreCTe.Inserir(documento);
            }
        }

        public bool DeletarCTesSubContratacaoViculadosAoRotaFacilityMercadoLivre(out string mensagemErro, Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Repositorio.UnitOfWork unitOfWork)
        {

            try
            {

                if (!ValidaCTesSubContratacao(out mensagemErro, pedidoCTeParaSubContratacao.CargaPedido))
                    return false;

                ExcluirCTesSubContratacao(pedidoCTeParaSubContratacao, pedidoCTeParaSubContratacao.CargaPedido, unitOfWork);

                mensagemErro = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    mensagemErro = "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.";
                else
                    mensagemErro = "Ocorreu uma falha ao excluir.";

                return false;
            }
        }

        public void ExcluirCTesSubContratacao(Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Canhotos.Canhoto svcCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            Repositorio.Embarcador.CTe.CTeTerceiro repCTeParaSubContratacao = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete repPedidoCTeSubContratacaoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente repCargaPedidoCTeParaSubcontratacaoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao repPedidoCTeParaSubContratacaoContaContabilContabilizacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosAverbacao repCargaDadosAverbacao = new Repositorio.Embarcador.Cargas.CargaDadosAverbacao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicoesFrete = repCargaComposicaoFrete.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete cargaComposicaoFrete in cargaComposicoesFrete)
            {
                cargaComposicaoFrete.PedidoCTesParaSubContratacao = null;
                cargaComposicaoFrete.PedidoXMLNotasFiscais = null;

                repCargaComposicaoFrete.Deletar(cargaComposicaoFrete);
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente = repCargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);

            if (cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente != null)
                repCargaPedidoCTeParaSubcontratacaoTabelaFreteCliente.Deletar(cargaPedidoCTeParaSubcontratacaoTabelaFreteCliente);

            repCargaEntregaNotaFiscal.ExcluirCargaEntregaNotaFiscalPorCargaPedido(cargaPedido.Codigo);
            repCargaEntregaPedido.ExcluirPorCargaPedido(cargaPedido.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> pedidoCTeParaSubContratacaoPedidoNotasFiscais = repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarPorPedidoCTeParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal pedidoCTeParaSubContratacaoPedidoNotaFiscal in pedidoCTeParaSubContratacaoPedidoNotasFiscais)
            {
                repPedidoCTeParaSubContratacaoPedidoNotaFiscal.Deletar(pedidoCTeParaSubContratacaoPedidoNotaFiscal);
                repPedidoXMLNotaFiscalComponenteFrete.DeletarPorPedidoXMLNotaFiscal(pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal.Codigo, false);
                repIntegracaoAVIPED.DeletarPorPedidoXMLNotaFiscal(pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal.Codigo);
                repPedidoXMLNotaFiscal.Deletar(pedidoCTeParaSubContratacaoPedidoNotaFiscal.PedidoXMLNotaFiscal);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> componentes = repPedidoCTeSubContratacaoComponenteFrete.BuscarPorPedidoCteParaSubcontratacao(pedidoCTeParaSubContratacao.Codigo, false);
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete componente in componentes)
                repPedidoCTeSubContratacaoComponenteFrete.Deletar(componente);

            repPedidoCTeParaSubContratacaoContaContabilContabilizacao.DeletarPorCarga(cargaPedido.Carga.Codigo);
            repDocumentoContabil.SetarReferenciaProvisaoNulaPorCarga(cargaPedido.CargaOrigem.Codigo);
            repDocumentoProvisao.ExcluirTodosAgProvisaoPorCarga(cargaPedido.CargaOrigem.Codigo);
            repCargaDadosAverbacao.ExcluirTodosPorCTeECarga(cargaPedido.CargaOrigem.Codigo, pedidoCTeParaSubContratacao.CTeTerceiro.ChaveAcesso);

            repPedidoCTeParaSubContratacao.Deletar(pedidoCTeParaSubContratacao);


            if (!repPedidoCTeParaSubContratacao.ExisteEmOutroPedido(pedidoCTeParaSubContratacao.CTeTerceiro.Codigo, cargaPedido.Codigo))
            {
                pedidoCTeParaSubContratacao.CTeTerceiro.Ativo = false;
                repCTeParaSubContratacao.Atualizar(pedidoCTeParaSubContratacao.CTeTerceiro);
                Servicos.Log.TratarErro("Inativou o CTeTerceiro de código " + pedidoCTeParaSubContratacao.CTeTerceiro.Codigo, "CTeTerceiro");

                svcCanhoto.ExcluirCanhotoDoCTe(pedidoCTeParaSubContratacao.CTeTerceiro, unitOfWork);
            }
        }

        private bool ValidaCTesSubContratacao(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if (cargaPedido.Carga == null)
            {
                mensagemErro = "Pedido não encontrado";
                return false;
            }

            if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
            {
                mensagemErro = "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.";
                return false;
            }

            if (cargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
            {
                mensagemErro = "A atual situação da carga (" + cargaPedido.Carga.DescricaoSituacaoCarga + ") não permite a exclusão dos CT-es para Subcontratação";
                return false;
            }

            mensagemErro = string.Empty;
            return true;
        }

        private bool ExcessaoPorPossuirDependeciasNoBanco(Exception ex)
        {
            Servicos.Log.TratarErro(ex);
            if (ex.Message == "O registro possui dependências e não pode ser excluido.")
            {
                return true;
            }
            else if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
            {
                System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                if (excecao.Number == 547)
                {
                    return true;
                }
            }

            return false;
        }

    }

}
