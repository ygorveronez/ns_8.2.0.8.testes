using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class CONEMB_NF
    {
        public static Dominio.ObjetosDeValor.EDI.CONEMB_NF.Arquivo ConverterParaCONEMB_NF(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;

            if (cargaEDIIntegracao.CTe != null)
                cargaCTes = repCargaCTe.BuscarTodosPorCTe(cargaEDIIntegracao.CTe.Codigo);
            else if (cargaEDIIntegracao.LayoutEDI.AgruparPorRemetente && cargaEDIIntegracao.Remetente != null)
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo, cargaEDIIntegracao.Remetente.CPF_CNPJ);
            else
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo, 0D);

            int numeroSequencial = 0;

            Dominio.ObjetosDeValor.EDI.CONEMB_NF.Arquivo arquivo = new Dominio.ObjetosDeValor.EDI.CONEMB_NF.Arquivo() { Registros = new List<Dominio.ObjetosDeValor.EDI.CONEMB_NF.Registro>() };

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                int.TryParse(cargaCTe.CTe.NumeroCTeSubComp, out int numeroCTeSubComp);
                int.TryParse(cargaCTe.CTe.SerieCTeSubComp, out int serieCTeSubComp);

                foreach (Dominio.Entidades.DocumentosCTE documentoCTe in cargaCTe.CTe.Documentos)
                {
                    numeroSequencial++;

                    int.TryParse(documentoCTe.Numero, out int numeroNotaFiscal);
                    int.TryParse(documentoCTe.Serie, out int serieNotaFiscal);

                    Dominio.ObjetosDeValor.EDI.CONEMB_NF.Registro registro = new Dominio.ObjetosDeValor.EDI.CONEMB_NF.Registro()
                    {
                        BaseCalculoICMS = cargaCTe.CTe.BaseCalculoICMS,
                        CFOP = cargaCTe.CTe.CFOP.CodigoCFOP,
                        Chave = cargaCTe.CTe.Chave,
                        ChaveDocumentoOriginal = cargaCTe.CTe.ChaveCTESubComp,
                        CNPJDestinatario = cargaCTe.CTe.Destinatario?.CPF_CNPJ,
                        CNPJRemetente = cargaCTe.CTe.Remetente?.CPF_CNPJ,
                        CNPJTransportador = cargaCTe.CTe.Empresa?.CNPJ,
                        DataEmissao = cargaCTe.CTe.DataEmissao.Value,
                        DataEmissaoNotaFiscal = documentoCTe.DataEmissao,
                        IndicadorRedespacho = "0",
                        ModeloDocumento = cargaCTe.CTe.ModeloDocumentoCONEMB,
                        Numero = cargaCTe.CTe.Numero,
                        NumeroDocumentoOriginal = numeroCTeSubComp,
                        SerieDocumentoOriginal = serieCTeSubComp,
                        NumeroNotaFiscal = numeroNotaFiscal,
                        NumeroSequencia = numeroSequencial,
                        PesoBruto = documentoCTe.Peso,
                        Serie = cargaCTe.CTe.Serie.Numero,
                        SerieNotaFiscal = serieNotaFiscal,
                        ValorImposto = cargaCTe.CTe.ValorImposto,
                        ValorNotaFiscal = documentoCTe.Valor,
                        ValorPedagio = cargaCTe.Componentes.Where(o => o.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO).Sum(o => o.ValorComponente),
                        ValorServicoPrestado = cargaCTe.CTe.ValorAReceber,
                        ValorTotalMercadoria = cargaCTe.CTe.ValorTotalMercadoria
                    };

                    arquivo.Registros.Add(registro);
                }
            }

            return arquivo;
        }

        public static Dominio.ObjetosDeValor.EDI.CONEMB_NF.Arquivo ConverterParaCONEMB_NF(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes)
        {
            if (ctes == null || ctes.Count <= 0)
                return null;

            int numeroSequencial = 0;

            Dominio.ObjetosDeValor.EDI.CONEMB_NF.Arquivo arquivo = new Dominio.ObjetosDeValor.EDI.CONEMB_NF.Arquivo() { Registros = new List<Dominio.ObjetosDeValor.EDI.CONEMB_NF.Registro>() };

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            {
                int.TryParse(cte.NumeroCTeSubComp, out int numeroCTeSubComp);
                int.TryParse(cte.SerieCTeSubComp, out int serieCTeSubComp);

                foreach (Dominio.Entidades.DocumentosCTE documentoCTe in cte.Documentos)
                {
                    numeroSequencial++;

                    int.TryParse(documentoCTe.Numero, out int numeroNotaFiscal);
                    int.TryParse(documentoCTe.Serie, out int serieNotaFiscal);

                    Dominio.ObjetosDeValor.EDI.CONEMB_NF.Registro registro = new Dominio.ObjetosDeValor.EDI.CONEMB_NF.Registro()
                    {
                        BaseCalculoICMS = cte.BaseCalculoICMS,
                        CFOP = cte.CFOP.CodigoCFOP,
                        Chave = cte.Chave,
                        ChaveDocumentoOriginal = cte.ChaveCTESubComp,
                        CNPJDestinatario = cte.Destinatario?.CPF_CNPJ,
                        CNPJRemetente = cte.Remetente?.CPF_CNPJ,
                        CNPJTransportador = cte.Empresa?.CNPJ,
                        DataEmissao = cte.DataEmissao.Value,
                        DataEmissaoNotaFiscal = documentoCTe.DataEmissao,
                        IndicadorRedespacho = "0",
                        ModeloDocumento = cte.ModeloDocumentoCONEMB,
                        Numero = cte.Numero,
                        NumeroDocumentoOriginal = numeroCTeSubComp,
                        SerieDocumentoOriginal = serieCTeSubComp,
                        NumeroNotaFiscal = numeroNotaFiscal,
                        NumeroSequencia = numeroSequencial,
                        PesoBruto = documentoCTe.Peso,
                        Serie = cte.Serie.Numero,
                        SerieNotaFiscal = serieNotaFiscal,
                        ValorImposto = cte.ValorImposto,
                        ValorNotaFiscal = documentoCTe.Valor,
                        ValorPedagio = cte.ComponentesPrestacao.Sum(o => o.ValorPedagio),
                        ValorServicoPrestado = cte.ValorAReceber,
                        ValorTotalMercadoria = cte.ValorTotalMercadoria
                    };

                    arquivo.Registros.Add(registro);
                }
            }

            return arquivo;
        }
    }
}
