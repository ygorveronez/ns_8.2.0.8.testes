using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class EBSNotaEntrada
    {
        public int sequencia;
        public Dominio.ObjetosDeValor.EDI.EBS.NotaEntrada ConverterNotaEntradaEBS(Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS> notas, Repositorio.UnitOfWork unidadeTrabalho)
        {
            sequencia = 0;
            Dominio.ObjetosDeValor.EDI.EBS.NotaEntrada notasEntrada = new Dominio.ObjetosDeValor.EDI.EBS.NotaEntrada();
            List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaEmitenteDestinatario> listaEmitentes = new List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaEmitenteDestinatario>();

            notasEntrada.TipoRegistro = "0";
            notasEntrada.DataGeracao = DateTime.Now;
            notasEntrada.CNPJEmpresa = empresa.CNPJ_Formatado;
            notasEntrada.OpcaoBase = "0";
            notasEntrada.Origem = "";
            notasEntrada.OpcaoRetencao = "";
            notasEntrada.Brancos = "";
            notasEntrada.UsoEBS = "";
            sequencia += 1;
            notasEntrada.Sequencia = sequencia.ToString("000000");

            for (int i = 0; i < notas.Count; i++)
            {
                Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaEmitenteDestinatario emitente = new Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaEmitenteDestinatario();

                emitente = RetornaDadosEmitente(notas[i], unidadeTrabalho);
                emitente.NotaFiscal = RetornaDadosNotaFiscal(notas[i], unidadeTrabalho);
                emitente.NotaFiscal.DadosComplementares = RetornaDadoComplenentar(notas[i], unidadeTrabalho);
                emitente.NotaFiscal.Itens = RetornaDadosItens(empresa, notas[i], unidadeTrabalho);
                emitente.NotaFiscal.Parcelas = new List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaParcela>();
                //if (emitente.NotaFiscal.VendaAVista == "N")SEM REGISTRO DAS PARCELAS SEGUNDO SOLICITAÇÃO DO DIEGO
                //    emitente.NotaFiscal.Parcelas = RetornaParcelas(notas[i], unidadeTrabalho);
                //else
                //    emitente.NotaFiscal.Parcelas = new List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaParcela>();

                listaEmitentes.Add(emitente);
            }

            notasEntrada.Emitentes = listaEmitentes;
            notasEntrada.Trailler = RetornaDadosTrailler(listaEmitentes, unidadeTrabalho);
            return notasEntrada;
        }

        public Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaEmitenteDestinatario RetornaDadosEmitente(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS nota, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaEmitenteDestinatario emitente = new Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaEmitenteDestinatario();

            emitente.TipoRegistro = "4";
            emitente.CNPJCPF = nota.Fornecedor.CPF_CNPJ_Formatado;
            emitente.Razao = nota.Fornecedor.Nome;
            emitente.Fantasia = nota.Fornecedor.NomeFantasia;
            emitente.Estado = nota.Fornecedor.Localidade.Estado.Sigla;
            emitente.Inscricao = nota.Fornecedor.IE_RG;
            emitente.Endereco = nota.Fornecedor.Endereco;
            emitente.Bairro = nota.Fornecedor.Bairro;
            emitente.Cidade = nota.Fornecedor.Localidade.Descricao;
            emitente.CEP = Utilidades.String.OnlyNumbers(nota.Fornecedor.CEP);
            emitente.Municipio = !string.IsNullOrWhiteSpace(nota.Fornecedor.Localidade.CodigoLocalidadeEmbarcador) ? nota.Fornecedor.Localidade.CodigoLocalidadeEmbarcador : nota.Fornecedor.Localidade.Codigo.ToString();
            if (!string.IsNullOrWhiteSpace(nota.Fornecedor.Telefone1) && nota.Fornecedor.Telefone1.Length > 3)
                emitente.DDD = "0" + Utilidades.String.OnlyNumbers(nota.Fornecedor.Telefone1).Substring(0, 3);
            else
                emitente.DDD = "";
            emitente.Fone = nota.Fornecedor.Telefone1;
            emitente.ContaCliente = "000000";
            emitente.HistoricoCliente = "000";
            if (!string.IsNullOrWhiteSpace(nota.Fornecedor.ContaFornecedorEBS))
                emitente.ContaFornecedor = nota.Fornecedor.ContaFornecedorEBS;
            else
                emitente.ContaFornecedor = "000000";
            emitente.HistoricoFornecedor = "000";
            emitente.Produtor = "N";
            emitente.IdentificacaoExterior = "";
            emitente.Numero = Utilidades.String.OnlyNumbers(nota.Fornecedor.Numero);
            emitente.Complemento = nota.Fornecedor.Complemento;
            emitente.SUFRAMA = nota.Fornecedor.InscricaoSuframa;
            emitente.CodigoPais = nota.Fornecedor.Localidade.Pais.Codigo.ToString();
            emitente.NaturezaJuridica = "";
            emitente.MunicipioIBGE = nota.Fornecedor.Localidade.CodigoIBGE.ToString();
            emitente.Brancos = "";
            emitente.UsoEBS = "";
            sequencia += 1;
            emitente.Sequencial = sequencia.ToString("000000");

            return emitente;

        }

        public Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaNotaFiscal RetornaDadosNotaFiscal(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS nota, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDocumentoEntradaDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unidadeTrabalho);
            Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaNotaFiscal notaFiscal = new Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaNotaFiscal();

            List<decimal> aliquotasICMS = repDocumentoEntradaItem.BuscarAliquotasICMS(nota.Codigo);

            notaFiscal.TipoRegistro = "1";
            notaFiscal.DataLancamento = nota.DataEntrada;
            if (nota.Numero.ToString().Length > 6)
                notaFiscal.NumeroNota = "000000";
            else
                notaFiscal.NumeroNota = nota.Numero.ToString("000000");
            notaFiscal.DataDocumento = nota.DataEmissao;
            notaFiscal.Modelo = nota.Modelo.Numero;
            notaFiscal.Serie = nota.Serie;
            notaFiscal.SubSerie = "";
            notaFiscal.Natureza = nota.CFOP != null ? nota.CFOP.CodigoCFOP.ToString() : nota.NaturezaOperacao != null && nota.NaturezaOperacao.CFOP != null ? nota.NaturezaOperacao.CFOP.CodigoCFOP.ToString() : "";
            notaFiscal.Variacao = "01";
            if (nota.CFOP != null && !string.IsNullOrWhiteSpace(nota.CFOP.Extensao) && nota.CFOP.Extensao.Length >= 4)
                notaFiscal.Classificacao1 = nota.CFOP.Extensao.Substring(2, 2);
            else
                notaFiscal.Classificacao1 = "00";
            notaFiscal.Classificacao2 = "00";
            notaFiscal.CNPJCPFEmitente = nota.Fornecedor.CPF_CNPJ_Formatado;
            notaFiscal.ValorContabil = nota.ValorTotal;
            notaFiscal.BasePIS = nota.BasePIS;
            notaFiscal.BaseCOFINS = nota.BaseCOFINS;
            notaFiscal.BaseCSLL = nota.BaseCSLL;
            notaFiscal.BaseIRPJ = 0;
            if (aliquotasICMS != null && aliquotasICMS.Count >= 1 && aliquotasICMS[0] > 0)
            {
                notaFiscal.BaseICMSA = repDocumentoEntradaItem.BaseICMSAliquota(nota.Codigo, aliquotasICMS[0]);
                notaFiscal.AliquotaICMSA = aliquotasICMS[0];
                notaFiscal.ValorICMSA = repDocumentoEntradaItem.ValorICMSAliquota(nota.Codigo, aliquotasICMS[0]);
            }
            else
            {
                notaFiscal.BaseICMSA = 0;
                notaFiscal.AliquotaICMSA = 0;
                notaFiscal.ValorICMSA = 0;
            }
            if (aliquotasICMS != null && aliquotasICMS.Count >= 2 && aliquotasICMS[1] > 0)
            {
                notaFiscal.BaseICMSB = repDocumentoEntradaItem.BaseICMSAliquota(nota.Codigo, aliquotasICMS[1]);
                notaFiscal.AliquotaICMSB = aliquotasICMS[1];
                notaFiscal.ValorICMSB = repDocumentoEntradaItem.ValorICMSAliquota(nota.Codigo, aliquotasICMS[1]);
            }
            else
            {
                notaFiscal.BaseICMSB = 0;
                notaFiscal.AliquotaICMSB = 0;
                notaFiscal.ValorICMSB = 0;
            }
            if (aliquotasICMS != null && aliquotasICMS.Count >= 3 && aliquotasICMS[2] > 0)
            {
                notaFiscal.BaseICMSC = repDocumentoEntradaItem.BaseICMSAliquota(nota.Codigo, aliquotasICMS[2]);
                notaFiscal.AliquotaICMSC = aliquotasICMS[2];
                notaFiscal.ValorICMSC = repDocumentoEntradaItem.ValorICMSAliquota(nota.Codigo, aliquotasICMS[2]);
            }
            else
            {
                notaFiscal.BaseICMSC = 0;
                notaFiscal.AliquotaICMSC = 0;
                notaFiscal.ValorICMSC = 0;
            }
            if (aliquotasICMS != null && aliquotasICMS.Count >= 4 && aliquotasICMS[3] > 0)
            {
                notaFiscal.BaseICMSD = repDocumentoEntradaItem.BaseICMSAliquota(nota.Codigo, aliquotasICMS[3]);
                notaFiscal.AliquotaICMSD = aliquotasICMS[3];
                notaFiscal.ValorICMSD = repDocumentoEntradaItem.ValorICMSAliquota(nota.Codigo, aliquotasICMS[3]);
            }
            else
            {
                notaFiscal.BaseICMSD = 0;
                notaFiscal.AliquotaICMSD = 0;
                notaFiscal.ValorICMSD = 0;
            }
            notaFiscal.IsentaICMS = 0;// repDocumentoEntradaItem.ValorOutrasICMS(nota.Codigo);
            notaFiscal.OutraICMS = nota.ValorTotal - nota.BaseCalculoICMS;
            if (notaFiscal.OutraICMS < 0)
                notaFiscal.OutraICMS = 0;
            notaFiscal.BaseIPI = nota.BaseIPI;
            notaFiscal.ValorIPI = nota.ValorTotalIPI;
            notaFiscal.IsentaIPI = 0;
            notaFiscal.OutraIPI = 0;
            notaFiscal.MercadoriaST = 0;// repDocumentoEntradaItem.ValorMercadoriasComST(nota.Codigo); SEGUNDO DIEGO VIR ZERADO NESTE CAMPO
            notaFiscal.BaseST = nota.BaseCalculoICMSST;
            notaFiscal.ICMSST = nota.ValorTotalICMSST;
            notaFiscal.Diferidas = 0;
            notaFiscal.Observacao = "";
            notaFiscal.Especie = nota.Modelo.Abreviacao;
            notaFiscal.VendaAVista = repDocumentoEntradaDuplicata.PagamentoAVista(nota.Codigo) ? "S" : "N";
            notaFiscal.NaturezaOperacaoST = "0000";
            notaFiscal.BasePISCOFINSST = 0;
            notaFiscal.BaseISS = 0;
            notaFiscal.AliquotaISS = 0;
            notaFiscal.ValorISS = 0;
            notaFiscal.IsentaISS = 0;
            notaFiscal.IRRFRetido = 0;
            notaFiscal.PISRetido = nota.ValorTotalRetencaoPIS;
            notaFiscal.COFINSRetido = nota.ValorTotalRetencaoCOFINS;
            notaFiscal.CSLLRetido = nota.ValorTotalRetencaoCSLL;
            if (nota.Modelo.Abreviacao == "NFS-e")
                notaFiscal.DataPagamento = nota.DataEntrada.ToString("ddMMyyyy");
            else
                notaFiscal.DataPagamento = "00000000";
            notaFiscal.OperacaoContabil = "0000";
            notaFiscal.CliFor = "";
            if (nota.Fornecedor.Localidade.Estado.Sigla == "EX")
                notaFiscal.IdentificacaoExterior = nota.Fornecedor.RG_Passaporte;
            else
                notaFiscal.IdentificacaoExterior = "";
            notaFiscal.INSSRetido = nota.ValorTotalRetencaoINSS;
            notaFiscal.FunRuralRetido = 0;
            notaFiscal.CodigoServico = "0000";
            notaFiscal.ISSRetido = "";
            notaFiscal.ISSDevidoPrestacao = "";
            notaFiscal.UFPrestacao = "";
            notaFiscal.MunicipioPrestacao = "";
            notaFiscal.TipoEmissao = "T";
            notaFiscal.ModalidadeFrete = "";
            notaFiscal.Brancos = "";
            notaFiscal.UseEBS = "";
            sequencia += 1;
            notaFiscal.Sequencia = sequencia.ToString("000000");
            if (nota.Numero.ToString().Length > 6)
                notaFiscal.NumeroNota2 = nota.Numero.ToString("000000000");
            else
                notaFiscal.NumeroNota2 = "000000000";
            notaFiscal.Observacao2 = "";
            notaFiscal.CentroCusto = "";
            notaFiscal.BasePISCOFINSICMSST = 0;
            notaFiscal.DataEmissaoRPS = "00000000";
            notaFiscal.ICMSRelativoFCP = 0;
            notaFiscal.ICMSUFDestino = 0;
            notaFiscal.ICMSUFOrigem = 0;
            if (aliquotasICMS != null && aliquotasICMS.Count >= 5 && aliquotasICMS[4] > 0)
            {
                notaFiscal.BaseICMSE = repDocumentoEntradaItem.BaseICMSAliquota(nota.Codigo, aliquotasICMS[4]);
                notaFiscal.AliquotaICMSE = aliquotasICMS[4];
                notaFiscal.ValorICMSE = repDocumentoEntradaItem.ValorICMSAliquota(nota.Codigo, aliquotasICMS[4]);
            }
            else
            {
                notaFiscal.BaseICMSE = 0;
                notaFiscal.AliquotaICMSE = 0;
                notaFiscal.ValorICMSE = 0;
            }
            if (aliquotasICMS != null && aliquotasICMS.Count >= 6 && aliquotasICMS[5] > 0)
            {
                notaFiscal.BaseICMSF = repDocumentoEntradaItem.BaseICMSAliquota(nota.Codigo, aliquotasICMS[5]);
                notaFiscal.AliquotaICMSF = aliquotasICMS[5];
                notaFiscal.ValorICMSF = repDocumentoEntradaItem.ValorICMSAliquota(nota.Codigo, aliquotasICMS[5]);
            }
            else
            {
                notaFiscal.BaseICMSF = 0;
                notaFiscal.AliquotaICMSF = 0;
                notaFiscal.ValorICMSF = 0;
            }

            return notaFiscal;

        }

        public List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaItem> RetornaDadosItens(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS nota, Repositorio.UnitOfWork unidadeTrabalho)
        {
            List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaItem> listaItem = new List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaItem>();
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensNota = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(nota.Codigo);
            for (int i = 0; i < itensNota.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem documentoEntradaItem = itensNota[i];
                if (documentoEntradaItem.Produto != null)
                {
                    Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaItem item = new Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaItem();

                    item.TipoRegistro = "2";
                    item.Codigo = documentoEntradaItem.Produto.Codigo.ToString("0000000000");
                    item.Quantidade1 = documentoEntradaItem.Quantidade;
                    item.Valor = documentoEntradaItem.ValorTotal;// - documentoEntradaItem.Desconto;
                    item.Quantidade2 = 0;
                    item.Desconto = documentoEntradaItem.Desconto;
                    item.BaseICMS = documentoEntradaItem.BaseCalculoICMS;
                    item.AliquotaICMS = documentoEntradaItem.AliquotaICMS;
                    item.ValorIPI = documentoEntradaItem.ValorIPI;
                    item.BaseICMSST = documentoEntradaItem.BaseCalculoICMSST;
                    item.AliquotaIPI = documentoEntradaItem.AliquotaIPI;
                    item.PercentualReducaoICMS = 0;
                    item.CSTICMS = documentoEntradaItem.CSTICMS;
                    item.Identificacao = "000000000000000";
                    item.CSTIPI = documentoEntradaItem.CSTIPI;
                    item.BaseIPI = documentoEntradaItem.BaseCalculoIPI;
                    item.CSTPIS = documentoEntradaItem.CSTPIS;
                    item.BasePIS = documentoEntradaItem.BaseCalculoPIS;
                    item.AliquotaPIS = documentoEntradaItem.AliquotaPIS;
                    item.QuantidadeBasePIS = 0;
                    item.AliquotaPISReais = 0;
                    item.ValorPIS = documentoEntradaItem.ValorPIS;
                    item.CSTCOFINS = documentoEntradaItem.CSTCOFINS;
                    item.BaseCOFINS = documentoEntradaItem.BaseCalculoCOFINS;
                    item.AliquotaCOFINS = documentoEntradaItem.AliquotaCOFINS;
                    item.ValorCOFINS = documentoEntradaItem.ValorCOFINS;
                    item.QuantidadeBaseCOFINS = 0;
                    item.AliquotaCOIFINSReais = 0;
                    item.ValorICMSST = documentoEntradaItem.ValorICMSST;
                    item.AliquotaICMSST = documentoEntradaItem.AliquotaICMSST;
                    item.ValorICMS = documentoEntradaItem.ValorICMS;
                    item.NaturezaItem = documentoEntradaItem.CFOP != null ? documentoEntradaItem.CFOP.CodigoCFOP.ToString() : "";
                    item.Unidade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedidaHelper.ObterSigla(documentoEntradaItem.UnidadeMedida);
                    item.BaseICMSDiferencial = 0;
                    item.ICMSOrigemDiferencial = 0;
                    item.ICMSInternoDiferencial = 0;
                    item.ICMSRecolherDiferencial = 0;
                    item.BaseICMSAntecipacao = 0;
                    item.ICMSOrigemAntecipacao = 0;
                    item.ICMSIternoAntecipacao = 0;
                    item.ICMSRecolherAntecipacao = 0;
                    item.BaseSTAntecipacao = 0;
                    item.MVASTAntecipacao = 0;
                    item.BaseAjustadaSTAntecipacao = 0;
                    item.ICMSSTOrigemAntecipacao = 0;
                    item.ICMSSTInternoAntecipacao = 0;
                    item.ICMSSTRecolherAntecipacao = 0;
                    item.CSOSN = empresa.OptanteSimplesNacional ? "S" : "N";
                    item.ICMSAntecipacaoParcial = 0;
                    item.PercentualDescontoCondicional = 0;
                    item.ProdutoIndustrializado = "";
                    item.Brancos = "";
                    item.UsoEBS = "";
                    sequencia += 1;
                    item.Sequencia = sequencia.ToString("000000");
                    item.CreditoPIS = 0;
                    item.CreditoCOFINS = 0;
                    item.Frete = documentoEntradaItem.ValorFrete;
                    item.Seguro = documentoEntradaItem.ValorSeguro;
                    item.Despesas = documentoEntradaItem.OutrasDespesas;
                    item.SaidaNatureza = "";
                    item.SaidaOrigem = "";
                    item.SaidaCSTICMS = "";
                    item.SaidaBaseICMS = 0;
                    item.SaidaAliquotaICMS = 0;
                    item.SaidaValorICMS = 0;
                    item.SaidaBaseICMSST = 0;
                    item.SaidaAliquotaICMSST = 0;
                    item.SaidaValorICMSST = 0;
                    item.SaidaCSTIPI = "";
                    item.SaidaBaseIPI = 0;
                    item.SaidaAliquotaIPI = 0;
                    item.SaidaValorIPI = 0;
                    item.SaidaCSTPIS = "";
                    item.SaidaBasePIS = 0;
                    item.SaidaAliquotaPIS = 0;
                    item.SaidaValorPIS = 0;
                    item.SaidaQuantidadeBasePIS = 0;
                    item.SaidaAliquotaValorPIS = 0;
                    item.SaidaCSTCOFINS = "";
                    item.SaidaBaseCOFINS = 0;
                    item.SaidaAliquotaCOFINS = 0;
                    item.SaidaValorCOFINS = 0;
                    item.SaidaQuantidadeBaseCOFINS = 0;
                    item.SaidaAliquotaValorCOFINS = 0;
                    item.Identificacao2 = "";
                    item.GTIN = "";
                    item.MVAOriginalAntecipacao = 0;

                    listaItem.Add(item);
                }
            }

            return listaItem;

        }

        public Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaDadoComplementar RetornaDadoComplenentar(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS nota, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaDadoComplementar dado = new Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaDadoComplementar();

            dado.TipoRegistro = "5";
            dado.ValorMercadoria = nota.ValorProdutos;
            dado.Desconto = nota.ValorTotalDesconto;
            dado.Frete = nota.ValorTotalFrete;
            dado.Despesa = nota.ValorTotalOutrasDespesas;
            dado.Seguro = nota.ValorTotalSeguro;
            dado.PesoBruto = 0;
            dado.PesoLiquido = 0;
            dado.CNPJCPFTransportadora = "";
            dado.MeioTransporte = "";
            dado.Placa = "";
            dado.Volumes = 0;
            dado.Especie = "";
            dado.ChaveNFe = nota.Chave;
            dado.ICMSSTRetido = "N";
            dado.InscricaoEstadualRemetente = "";
            dado.ValorICMSAntecipacaoParcial = 0;
            dado.ValorDiferencialAliquota = 0;
            dado.ValorDiferencialAliquotaCredito = 0;
            dado.ValorDiferencialAliquotaDebito = 0;
            dado.BaseICMSAntecipacaoParcial = 0;
            dado.BaseICMSDiferencial = 0;
            dado.ValorAntecipacaoParcialDebito = 0;
            dado.ValorAntecipacaoParcialCredito = 0;
            dado.ValorBaseICMSSTAntecipacao = 0;
            dado.ValorICMSSTAntecipacao = 0;
            dado.ValorICMSSTInternoAntecipacao = 0;
            dado.ValorSTAntecipacaoICMSST = 0;
            dado.ChaveCTeReferencia = "";
            dado.TipoImportacao = "";
            dado.TipoDocumentoImportacao = "";
            dado.NumeroDISiscomex = "";
            dado.NumeroATODrawback = "";
            dado.BasePISImportacao = 0;
            dado.ValorPISImportacao = 0;
            dado.DataPagamentoPISPASEP = "";
            dado.BaseCOFINSImportacao = 0;
            dado.ValorCOFINSImportacao = 0;
            dado.DataPagamentoCOFINS = "";
            dado.ValePedagio = 0;
            dado.Brancos = "";
            dado.UsoEBS = "";
            sequencia += 1;
            dado.Sequencia = sequencia.ToString("000000");
            dado.ChaveCTeOrigem = "";

            return dado;

        }

        public List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaParcela> RetornaParcelas(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS nota, Repositorio.UnitOfWork unidadeTrabalho)
        {
            List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaParcela> listaParcela = new List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaParcela>();
            Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata repDocumentoEntradaDuplicata = new Repositorio.Embarcador.Financeiro.DocumentoEntradaDuplicata(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaDuplicata> duplicatasNota = repDocumentoEntradaDuplicata.BuscarPorDocumentoEntrada(nota.Codigo);
            for (int i = 0; i < duplicatasNota.Count; i++)
            {
                Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaParcela parcela = new Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaParcela();

                parcela.TipoRegistro = "9";
                parcela.TipoParcela = "N";
                parcela.NumeroFatura = duplicatasNota[i].Numero;
                parcela.TipoTitulo = "00";
                parcela.DataVencimento = duplicatasNota[i].DataVencimento;
                parcela.ValorParcela = duplicatasNota[i].Valor;
                parcela.ValorTarifa = 0;
                parcela.Brancos = "";
                parcela.UsoEBS = "";
                sequencia += 1;
                parcela.Sequencia = sequencia.ToString("000000");

                listaParcela.Add(parcela);
            }

            return listaParcela;

        }

        public Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaTrailler RetornaDadosTrailler(List<Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaEmitenteDestinatario> lsitaEmitentes, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaTrailler trailler = new Dominio.ObjetosDeValor.EDI.EBS.NotaEntradaTrailler();

            trailler.TipoRegistro = "3";
            trailler.ValorContabil = lsitaEmitentes.Select(obj => obj.NotaFiscal.ValorContabil).Sum();
            trailler.BasePIS = lsitaEmitentes.Select(obj => obj.NotaFiscal.BasePIS).Sum();
            trailler.BaseCOFINS = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseCOFINS).Sum();
            trailler.BaseCSLL = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseCSLL).Sum();
            trailler.BaseIRPJ = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseIRPJ).Sum();
            trailler.BaseICMSA = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseICMSA).Sum();
            trailler.ValorICMSA = lsitaEmitentes.Select(obj => obj.NotaFiscal.ValorICMSA).Sum();
            trailler.BaseICMSB = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseICMSB).Sum();
            trailler.ValorICMSB = lsitaEmitentes.Select(obj => obj.NotaFiscal.ValorICMSB).Sum();
            trailler.BaseICMSC = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseICMSC).Sum();
            trailler.ValorICMSC = lsitaEmitentes.Select(obj => obj.NotaFiscal.ValorICMSC).Sum();
            trailler.BaseICMSD = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseICMSD).Sum();
            trailler.ValorICMSD = lsitaEmitentes.Select(obj => obj.NotaFiscal.ValorICMSD).Sum();
            trailler.IsentaICMS = lsitaEmitentes.Select(obj => obj.NotaFiscal.IsentaICMS).Sum();
            trailler.OutraICMS = lsitaEmitentes.Select(obj => obj.NotaFiscal.OutraICMS).Sum();
            trailler.BaseIPI = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseIPI).Sum();
            trailler.ValorIPI = lsitaEmitentes.Select(obj => obj.NotaFiscal.ValorIPI).Sum();
            trailler.IsentaIPI = lsitaEmitentes.Select(obj => obj.NotaFiscal.IsentaIPI).Sum();
            trailler.OutraIPI = lsitaEmitentes.Select(obj => obj.NotaFiscal.OutraIPI).Sum();
            trailler.MercadoriaST = lsitaEmitentes.Select(obj => obj.NotaFiscal.MercadoriaST).Sum();
            trailler.BaseST = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseST).Sum();
            trailler.ICMSST = lsitaEmitentes.Select(obj => obj.NotaFiscal.ICMSST).Sum();
            trailler.Difereida = lsitaEmitentes.Select(obj => obj.NotaFiscal.Diferidas).Sum();
            trailler.BaseICMSE = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseICMSE).Sum();
            trailler.ValorICMSE = lsitaEmitentes.Select(obj => obj.NotaFiscal.ValorICMSE).Sum();
            trailler.BaseICMSF = lsitaEmitentes.Select(obj => obj.NotaFiscal.BaseICMSF).Sum();
            trailler.ValorICMSF = lsitaEmitentes.Select(obj => obj.NotaFiscal.ValorICMSF).Sum();
            trailler.Brancos = "";
            sequencia += 1;
            trailler.Sequencia = sequencia.ToString("000000");

            return trailler;

        }
    }
}
