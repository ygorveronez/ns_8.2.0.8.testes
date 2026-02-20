using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class EBSDocumentoSaida
    {
        public static Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaida GerarEBS(Dominio.Entidades.Empresa empresa, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);

            Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao;

#if DEBUG
            tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Homologacao;
#endif

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorRemetenteEBS(empresa.Codigo, string.Empty, dataEmissaoInicial, dataEmissaoFinal, new string[] { "A", "Z", "C" }, tipoAmbiente);

            Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaida ebs = new Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaida();

            ebs.DataGeracao = DateTime.Now;
            ebs.CNPJEmpresa = empresa.CNPJ_Formatado;
            ebs.OpcaoBases = "1";
            ebs.Origem = "";
            ebs.OpcaoRetencao = "0";
            ebs.Sequencia = 1;

            ebs.Remetentes = new List<Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaidaRemetente>();

            int sequencia = 1;

            string[] descricaoComponenteNaoSomarOutrasICMS = new string[] { "frete valor", "valor frete", "icms", "impostos", "imposto" };

            for (int i = 0; i < ctes.Count; i++)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];

                Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaidaRemetente remetenteEBS = new Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaidaRemetente();

                sequencia++;
                remetenteEBS.Pessoa = svcPessoa.ConverterObjetoParticipamenteCTe(cte.Remetente);
                remetenteEBS.Sequencia = sequencia;

                sequencia++;
                remetenteEBS.Destinatario = new Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaidaDestinatario()
                {
                    Pessoa = svcPessoa.ConverterObjetoParticipamenteCTe(cte.Destinatario),
                    Sequencia = sequencia
                };

                sequencia++;
                remetenteEBS.Documento = new Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaidaDocumento()
                {
                    DataLancamento = cte.DataEmissao.Value,
                    NumeroInicial = cte.Numero,
                    NumeroFinal = cte.Numero,
                    DataDocumento = cte.DataEmissao.Value,
                    Modelo = cte.ModeloDocumentoFiscal.Numero,
                    Serie = cte.Serie.Numero.ToString(),
                    SubSerie = "",
                    Natureza = cte.CFOP.CodigoCFOP,
                    Variacao = 1,
                    Classificacao1 = 0,
                    Classificacao2 = 0,
                    CPFCNPJDestinatario = cte.Destinatario.CPF_CNPJ_Formatado,
                    //ValorContabil = cte.Status == "C" ? 0m : cte.ValorAReceber,
                    BasePIS = 0m,
                    BaseCOFINS = 0m,
                    BaseCSLL = 0m,
                    BaseIRPJ = 0m,
                    //BaseICMSA = cte.Status == "C" ? 0m : cte.BaseCalculoICMS,
                    //AliquotaICMSA = cte.Status == "C" ? 0m : cte.AliquotaICMS,
                    //ValorICMSA = cte.Status == "C" ? 0m : cte.ValorICMS,
                    BaseICMSB = 0m,
                    AliquotaICMSB = 0m,
                    ValorICMSB = 0m,
                    BaseICMSC = 0m,
                    AliquotaICMSC = 0m,
                    ValorICMSC = 0m,
                    BaseICMSD = 0m,
                    AliquotaICMSD = 0m,
                    ValorICMSD = 0m,
                    IsentasICMS = 0m,
                    OutrasICMS = 0m,
                    BaseIPI = 0m,
                    ValorIPI = 0m,
                    IsentasIPI = 0m,
                    OutrasIPI = 0m,
                    MercadoriasST = 0m,
                    BaseST = 0m,
                    ICMSST = 0m,
                    Diferidas = 0m,
                    BaseISS = 0m,
                    AliquotaISS = 0m,
                    ValorISS = 0m,
                    IsentasISS = 0m,
                    IRRFRetido = 0m,
                    Observacoes = cte.Status == "C" ? "CANCELADO" : "EMITIDO",
                    Especie = "CTE",
                    VendaAVista = "N",
                    NaturezaOperacaoST = 0,
                    BasePISCOFINSST = 0m,
                    PISRetido = 0m,
                    COFINSRetido = 0m,
                    CSLLRetido = 0m,
                    DataRecebimento = new DateTime(2001, 01, 01),
                    OperacaoContabil = 0,
                    Materiais = 0m,
                    SubEmpreitada = 0m,
                    CodigoServico = 0,
                    CLIFOR = 0,
                    IdentificacaoExterior = "",
                    Sequencia = sequencia,
                    NumeroNotaInicial2 = cte.Numero,
                    NumeroNotaFinal2 = cte.Numero,
                    Observacoes2 = "",
                    CentroCusto = 0,
                    BasePISCOFINSICMSST = 0m
                };

                if (cte.Status != "C")
                {
                    string outraDescricaoICMS = string.Empty;
                    string outraDescricaoValorFrete = string.Empty;

                    if (cte.CargaCTes.Any(o => o.Carga.TipoOperacao != null && o.Carga.TipoOperacao.UsarConfiguracaoEmissao))
                        outraDescricaoICMS = cte.CargaCTes.Where(o => o.Carga.TipoOperacao != null && o.Carga.TipoOperacao.UsarConfiguracaoEmissao).Select(o => o.Carga.TipoOperacao.TipoOperacaoConfiguracoesComponentes.Where(tc => tc.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS).Select(tc => tc.OutraDescricaoCTe).FirstOrDefault()).FirstOrDefault();
                    else
                        outraDescricaoICMS = cte.TomadorPagador?.Cliente?.GrupoPessoas?.GrupoPessoasConfiguracaoComponentesFretes?.Where(o => o.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS).Select(o => o.OutraDescricaoCTe).FirstOrDefault();

                    if (cte.CargaCTes.Any(o => o.Carga.TipoOperacao != null && o.Carga.TipoOperacao.UsarConfiguracaoEmissao))
                        outraDescricaoValorFrete = cte.CargaCTes.Where(o => o.Carga.TipoOperacao != null && o.Carga.TipoOperacao.UsarConfiguracaoEmissao).Select(o => o.Carga.TipoOperacao.DescricaoComponenteFreteEmbarcador).FirstOrDefault();
                    else
                        outraDescricaoValorFrete = cte.TomadorPagador?.Cliente?.GrupoPessoas?.DescricaoComponenteFreteEmbarcador; 

                    List<string> outrasDescricoesTomadorNaoSomarOutrasICMS = new List<string>();

                    if (!string.IsNullOrWhiteSpace(outraDescricaoICMS))
                        outrasDescricoesTomadorNaoSomarOutrasICMS.Add(outraDescricaoICMS.ToLower());
                    if (!string.IsNullOrWhiteSpace(outraDescricaoValorFrete))
                        outrasDescricoesTomadorNaoSomarOutrasICMS.Add(outraDescricaoValorFrete.ToLower());

                    decimal valorComponentesIsentosTributacao = cte.ComponentesPrestacao.Where(o => !o.IncluiNaBaseDeCalculoDoICMS && !descricaoComponenteNaoSomarOutrasICMS.Contains(o.Nome.ToLower()) && !outrasDescricoesTomadorNaoSomarOutrasICMS.Contains(o.Nome.ToLower())).Sum(o => o.Valor);

                    remetenteEBS.Documento.OutrasICMS = valorComponentesIsentosTributacao;

                    remetenteEBS.Documento.ValorContabil = cte.ValorAReceber;

                    if (cte.CST == "40" || cte.CST == "41" || cte.CST == "")
                    {
                        remetenteEBS.Documento.IsentasICMS = cte.ValorAReceber - valorComponentesIsentosTributacao;
                    }
                    else if (cte.CST == "51")
                    {
                        remetenteEBS.Documento.Diferidas = cte.ValorAReceber;
                    }
                    else if (cte.CST == "60")
                    {
                        remetenteEBS.Documento.OutrasICMS += cte.ValorAReceber;
                        remetenteEBS.Documento.BaseST = cte.BaseCalculoICMS;
                        remetenteEBS.Documento.ICMSST = cte.ValorICMS;
                        remetenteEBS.Documento.BasePISCOFINSICMSST = cte.ValorAReceber;
                    }
                    else
                    {
                        remetenteEBS.Documento.BaseICMSA = cte.BaseCalculoICMS;
                        remetenteEBS.Documento.AliquotaICMSA = cte.AliquotaICMS;
                        remetenteEBS.Documento.ValorICMSA = cte.ValorICMS;
                    }
                }

                switch (cte.TipoPagamento)
                {
                    case Dominio.Enumeradores.TipoPagamento.Pago:
                        remetenteEBS.Documento.ModalidadeFrete = 1;
                        break;
                    case Dominio.Enumeradores.TipoPagamento.A_Pagar:
                        remetenteEBS.Documento.ModalidadeFrete = 2;
                        break;
                    case Dominio.Enumeradores.TipoPagamento.Outros:
                        remetenteEBS.Documento.ModalidadeFrete = 4;
                        break;
                    default:
                        remetenteEBS.Documento.ModalidadeFrete = 0;
                        break;
                }

                sequencia++;
                remetenteEBS.Documento.DadosComplementares = new Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaidaDocumentoDadoComplementar()
                {
                    ValorMercadorias = cte.ValorTotalMercadoria,
                    Desconto = 0m,
                    Frete = cte.ValorFrete,
                    Despesas = 0m,
                    Seguro = cte.ComponentesPrestacao.Sum(o => o.ValorSeguro),
                    PesoBruto = cte.QuantidadesCarga.Sum(o => o.Quantidade),
                    PesoLiquido = cte.QuantidadesCarga.Sum(o => o.Quantidade),
                    CPFCNPJTransportador = empresa.CNPJ_Formatado,
                    MeioTransporte = 0,
                    Placa = "",
                    Volumes = 0,
                    Especie = "",
                    NumeroRE = 0,
                    NumeroDespacho = 0,
                    PaisDestino = 0,
                    Moeda = 0,
                    DataDespacho = "00000000",
                    ValorDespacho = 0m,
                    CPFCNPJRemetente = cte.Remetente.CPF_CNPJ_Formatado,
                    UFDestino = cte.LocalidadeTerminoPrestacao.Estado.Sigla,
                    IdentificacaoExteriorRemetente = "",
                    Redespacho = "N",
                    INSSRetido = 0m,
                    FUNRURALRetido = 0m,
                    ChaveNFe = cte.Chave,
                    ISSRetido = "",
                    ISSDevidoPrestacao = "",
                    UFPrestacao = "",
                    MunicipioPrestacao = "",
                    UFOrigem = cte.LocalidadeInicioPrestacao.Estado.Sigla,
                    CodigoIBGEOrigem = cte.LocalidadeInicioPrestacao.CodigoIBGE,
                    ICMSSTRetidoAntecipadamente = "",
                    IEDestinatario = "",
                    TipoAssinanteTelecom = "",
                    TipoUtilizacaoTelecom = "",
                    NumeroTerminalTelecom = "",
                    NumeroFaturaTelecom = "",
                    CPFCNPJConsignatario = cte.OutrosTomador?.CPF_CNPJ_Formatado ?? string.Empty,
                    ChaveCTeReferencia = cte.ChaveCTESubComp,
                    Sequencia = sequencia
                };

                remetenteEBS.Documento.NotasFiscais = new List<Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaidaDocumentoNotaFiscal>();
                foreach (Dominio.Entidades.DocumentosCTE notaFiscal in cte.Documentos)
                {
                    sequencia++;
                    remetenteEBS.Documento.NotasFiscais.Add(new Dominio.ObjetosDeValor.EDI.EBS.DocumentoSaidaDocumentoNotaFiscal()
                    {
                        NotaCarga = "",
                        ModeloCarga = notaFiscal.ModeloDocumentoFiscal?.Numero ?? "55",
                        ValorCarga = notaFiscal.Valor,
                        SerieCarga = notaFiscal.SerieOuSerieDaChave,
                        DataEmissaoCarga = notaFiscal.DataEmissao,
                        NotaCarga2 = int.Parse(notaFiscal.Numero),
                        Sequencia = sequencia
                    });
                }

                ebs.Remetentes.Add(remetenteEBS);
            }

            return ebs;
        }
    }
}
