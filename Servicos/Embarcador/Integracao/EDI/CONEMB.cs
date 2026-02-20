using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class CONEMB
    {
        public Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB ConverterCargaParaCONEMB_MartinBrower(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;

            if (cargaEDIIntegracao.CTe != null)
                cargaCTes = repCargaCTe.BuscarTodosPorCTe(cargaEDIIntegracao.CTe.Codigo);
            else if (cargaEDIIntegracao.LayoutEDI.AgruparPorRemetente && cargaEDIIntegracao.Remetente != null)
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo, cargaEDIIntegracao.Remetente.CPF_CNPJ);
            else
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo, 0D);

            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB conemb = new Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB();

            conemb.Data = DateTime.Now;
            conemb.Destinatario = cargaEDIIntegracao.Carga.Empresa.RazaoSocial;
            conemb.Intercambio = "CON" + DateTime.Now.ToString("ddMMHHmm") + "1";
            conemb.Remetente = cargaCTes.First().CTe.Remetente.Nome;

            conemb.CabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.CONEMB.CabecalhoDocumento();
            conemb.CabecalhoDocumento.IdentificacaoDocumento = "CONHE" + DateTime.Now.ToString("ddMMHHmm") + "1";

            conemb.CabecalhoDocumento.Transportadores = new List<Dominio.ObjetosDeValor.EDI.CONEMB.Transportador>();

            Dominio.ObjetosDeValor.EDI.CONEMB.Transportador transportador = new Dominio.ObjetosDeValor.EDI.CONEMB.Transportador();
            transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(cargaEDIIntegracao.Carga.Empresa);

            transportador.ConhecimentosEmbarcados = new List<Dominio.ObjetosDeValor.EDI.CONEMB.CTeEmbarcado>();

            decimal valorFreteTotal = 0m;

            for (var i = 0; i < cargaCTes.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTes[i];

                decimal pesoTotal = cargaCTe.CTe.QuantidadesCarga.Sum(o => o.Quantidade);
                valorFreteTotal += cargaCTe.CTe.ValorAReceber;

                Dominio.ObjetosDeValor.EDI.CONEMB.CTeEmbarcado cteEmbarcado = new Dominio.ObjetosDeValor.EDI.CONEMB.CTeEmbarcado();

                decimal valorCross = (from obj in cargaCTe.Componentes where obj.ComponenteFrete != null && obj.ComponenteFrete.Descricao.Contains("CROSS") select obj.ValorComponente).Sum();

                cteEmbarcado.CodigoCentroCusto = cargaEDIIntegracao.Carga.Empresa.CodigoCentroCusto;
                cteEmbarcado.CodigoEstabelecimento = cargaEDIIntegracao.Carga.Empresa.CodigoEstabelecimento;
                cteEmbarcado.Filial = cargaEDIIntegracao.Carga.Empresa.RazaoSocial;
                cteEmbarcado.Serie = cargaCTe.CTe.Serie.Numero;
                cteEmbarcado.Numero = cargaCTe.CTe.Numero;
                cteEmbarcado.DataEmissao = cargaCTe.CTe.DataEmissao.Value;
                cteEmbarcado.CondicaoFrete = cargaCTe.CTe.CondicaoPagamento;
                cteEmbarcado.PesoTransportado = cargaCTe.CTe.Documentos.Sum(o => o.Peso);
                cteEmbarcado.ValorTotalFrete = cargaCTe.CTe.ValorAReceber;
                cteEmbarcado.BaseCalculoICMS = cargaCTe.CTe.BaseCalculoICMS;
                cteEmbarcado.AliquotaICMS = cargaCTe.CTe.AliquotaICMS;
                cteEmbarcado.ValorICMS = cargaCTe.CTe.ValorICMS;
                cteEmbarcado.ValorFretePorPeso = cargaCTe.CTe.ValorFrete + valorCross;
                cteEmbarcado.ValorTaxas = cteEmbarcado.ValorTotalFrete - cteEmbarcado.ValorFretePorPeso;
                cteEmbarcado.ValorADVALOREM = (from obj in cargaCTe.Componentes where obj.ComponenteFrete != null && obj.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM select obj.ValorComponente).Sum();
                cteEmbarcado.ValorPedagio = (from obj in cargaCTe.Componentes where obj.ComponenteFrete != null && obj.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO select obj.ValorComponente).Sum();
                cteEmbarcado.ValorSECCAT = (from obj in cargaCTe.Componentes where obj.ComponenteFrete != null && obj.ComponenteFrete.Descricao.Contains("DESCARGA") select obj.ValorComponente).Sum();
                cteEmbarcado.SubstituicaoTributaria = cargaCTe.CTe.CST == "60" ? "1" : "2";
                cteEmbarcado.Filler = "FRT";
                cteEmbarcado.CNPJEmissorConhecimento = cargaCTe.CTe.Empresa.CNPJ;
                cteEmbarcado.CNPJTomadorServico = cargaCTe.CTe.Tomador?.CPF_CNPJ_SemFormato ?? new string('0', 14);

                for (int iDoc = 0; iDoc < cargaCTe.CTe.Documentos.Count && iDoc < 30; iDoc++)
                {
                    string strProp = (iDoc + 1).ToString();

                    int numeroNotaFiscal;
                    int.TryParse(cargaCTe.CTe.Documentos[iDoc].Numero, out numeroNotaFiscal);

                    Utilidades.Object.SetNestedPropertyValue(cteEmbarcado, "SerieNotaFiscal" + strProp, cargaCTe.CTe.Documentos[iDoc].SerieOuSerieDaChave);
                    Utilidades.Object.SetNestedPropertyValue(cteEmbarcado, "NumeroNotaFiscal" + strProp, string.Format("{0:00000000}", numeroNotaFiscal));
                }

                for (int iDoc = cargaCTe.CTe.Documentos.Count; iDoc < 27; iDoc++)
                {
                    string strProp = (iDoc + 1).ToString();

                    Utilidades.Object.SetNestedPropertyValue(cteEmbarcado, "NumeroNotaFiscal" + strProp, "00000000");
                }

                string cnpjRemetente = cargaCTe.CTe.Remetente?.CPF_CNPJ ?? new string('0', 14);
                string cnpjDestinatario = cargaCTe.CTe.Destinatario?.CPF_CNPJ ?? new string('0', 14);
                string chaveCTe = cargaCTe.CTe.Chave;
                string chaveMDFe = string.Empty;

                if (cargaEDIIntegracao.Carga.CargaMDFes.Count > 0)
                    chaveMDFe = cargaEDIIntegracao.Carga.CargaMDFes.Where(o => o.MDFe.MunicipiosDescarregamento.Any(m => m.Documentos.Any(d => d.CTe.Codigo == cargaCTe.CTe.Codigo))).FirstOrDefault()?.MDFe.Chave;

                if (string.IsNullOrWhiteSpace(chaveMDFe))
                    chaveMDFe = new string('0', 44);

                int numeroCarga;
                int.TryParse(cargaEDIIntegracao.Carga.CodigoCargaEmbarcador, out numeroCarga);

                int volumes = cargaCTe.NotasFiscais.Sum(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes);

                DateTime dataColeta = cargaCTe.CTe.DataColeta ?? cargaCTe.CTe.DataEmissao.Value;

                cteEmbarcado.NumeroNotaFiscal27 = string.Format("{0:00000000}", volumes);
                cteEmbarcado.SerieNotaFiscal28 = "";
                cteEmbarcado.NumeroNotaFiscal28 = string.Format("{0:00000000}", (int)cargaCTe.CTe.ValorTotalMercadoria);
                cteEmbarcado.SerieNotaFiscal29 = "402";
                cteEmbarcado.NumeroNotaFiscal29 = "00" + dataColeta.ToString("yyMMdd");
                cteEmbarcado.SerieNotaFiscal30 = cargaEDIIntegracao.Carga.Veiculo.Placa.Substring(0, 3);
                cteEmbarcado.NumeroNotaFiscal30 = "0000" + cargaEDIIntegracao.Carga.Veiculo.Placa.Substring(3, 4);
                cteEmbarcado.SerieNotaFiscal31 = cnpjRemetente.Substring(0, 3);
                cteEmbarcado.NumeroNotaFiscal31 = "00" + cargaCTe.CTe.DataEmissao.Value.ToString("yyMMdd");
                cteEmbarcado.SerieNotaFiscal32 = cnpjRemetente.Substring(3, 3);
                cteEmbarcado.NumeroNotaFiscal32 = string.Format("{0:00000000}", numeroCarga);
                cteEmbarcado.SerieNotaFiscal33 = cnpjRemetente.Substring(6, 3);
                cteEmbarcado.NumeroNotaFiscal33 = "00000000";
                cteEmbarcado.SerieNotaFiscal34 = cnpjRemetente.Substring(9, 3);

                string codigoIntegracaoCliente = Utilidades.String.Left(cargaCTe.NotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.CodigoIntegracaoCliente).FirstOrDefault(), 3);
                string codigoRota = Utilidades.String.Left(cargaCTe.NotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Rota).FirstOrDefault(), 7);

                if (string.IsNullOrWhiteSpace(codigoIntegracaoCliente))
                    codigoIntegracaoCliente = "XXX";

                string tipoOperacaoMB = "002";

                if (cargaCTe.CTe.Remetente.Cliente != null && cargaCTe.CTe.Destinatario.Cliente != null)
                {
                    if (cargaCTe.CTe.Remetente.Cliente.GrupoPessoas == cargaEDIIntegracao.Carga.GrupoPessoaPrincipal && cargaCTe.CTe.Destinatario.Cliente.GrupoPessoas == cargaEDIIntegracao.Carga.GrupoPessoaPrincipal)
                        tipoOperacaoMB = "003"; //Transferência
                    else if (cargaCTe.CTe.Remetente.Cliente.GrupoPessoas != cargaEDIIntegracao.Carga.GrupoPessoaPrincipal)
                        tipoOperacaoMB = "001"; //Coleta
                }

                cteEmbarcado.NumeroNotaFiscal34 = tipoOperacaoMB + "07" + codigoIntegracaoCliente;
                cteEmbarcado.SerieNotaFiscal35 = cnpjRemetente.Substring(12, 2);
                cteEmbarcado.NumeroNotaFiscal35 = chaveCTe.Substring(20, 8);
                cteEmbarcado.SerieNotaFiscal36 = cnpjDestinatario.Substring(0, 3);
                cteEmbarcado.NumeroNotaFiscal36 = chaveCTe.Substring(28, 8);
                cteEmbarcado.SerieNotaFiscal37 = cnpjDestinatario.Substring(3, 3);
                cteEmbarcado.NumeroNotaFiscal37 = chaveCTe.Substring(36, 8);
                cteEmbarcado.SerieNotaFiscal38 = cnpjDestinatario.Substring(6, 3);
                cteEmbarcado.NumeroNotaFiscal38 = chaveMDFe.Substring(20, 8);
                cteEmbarcado.SerieNotaFiscal39 = cnpjDestinatario.Substring(9, 3);
                cteEmbarcado.NumeroNotaFiscal39 = chaveMDFe.Substring(28, 8);
                cteEmbarcado.SerieNotaFiscal40 = cnpjDestinatario.Substring(12, 2);
                cteEmbarcado.NumeroNotaFiscal40 = chaveMDFe.Substring(36, 8);

                cteEmbarcado.AcaoDocumento = "I";
                cteEmbarcado.TipoConhecimento = "N";
                cteEmbarcado.IndicacaoRepeticao = "U";
                cteEmbarcado.CFOP = " " + cargaCTe.CTe.CFOP.CodigoCFOP.ToString();

                cteEmbarcado.DadosComplementares = new Dominio.ObjetosDeValor.EDI.CONEMB.DadosComplementaresCTeEmbarcado();
                cteEmbarcado.DadosComplementares.TipoMeioTransporte = "BR11";
                cteEmbarcado.DadosComplementares.FilialEmissora = cargaCTe.CTe.Empresa.RazaoSocial;
                cteEmbarcado.DadosComplementares.Serie = cargaCTe.CTe.Serie.Numero;
                cteEmbarcado.DadosComplementares.Numero = cargaCTe.CTe.Numero;
                cteEmbarcado.DadosComplementares.CodigoLoja = cnpjDestinatario;
                cteEmbarcado.DadosComplementares.NumeroViagem = cargaEDIIntegracao.Carga.CodigoCargaEmbarcador;
                cteEmbarcado.DadosComplementares.CodigoRota = codigoRota;

                transportador.ConhecimentosEmbarcados.Add(cteEmbarcado);
            }

            conemb.CabecalhoDocumento.Transportadores.Add(transportador);

            conemb.CabecalhoDocumento.Total = new Dominio.ObjetosDeValor.EDI.CONEMB.Total()
            {
                QuantidadeTotalCTe = cargaCTes.Count,
                ValorTotalCTe = valorFreteTotal
            };

            return conemb;
        }

        public Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.EDICONEMB_VW ConverterCargaParaCONEMB_VOLKS(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;

            if (cargaEDIIntegracao.CTe != null)
                cargaCTes = repCargaCTe.BuscarTodosPorCTe(cargaEDIIntegracao.CTe.Codigo);
            else if (cargaEDIIntegracao.LayoutEDI.AgruparPorRemetente && cargaEDIIntegracao.Remetente != null)
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo, cargaEDIIntegracao.Remetente.CPF_CNPJ);
            else
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo, 0D);

            List<string> referenciasEDI = cargaCTes.SelectMany(obj => obj.NotasFiscais.Select(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.NumeroReferenciaEDI).Distinct()).ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = cargaCTes.SelectMany(obj => obj.NotasFiscais.Select(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal).Distinct()).ToList();

            Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.EDICONEMB_VW conemb = new Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.EDICONEMB_VW();
            conemb.Cabecalhos = new List<Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.Cabecalho>();

            foreach (string referenciaEDI in referenciasEDI)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasPorReferencia = notasFiscais.Where(obj => obj.NumeroReferenciaEDI == referenciaEDI).ToList();

                Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.Cabecalho cabecalho = new Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.Cabecalho
                {
                    Tipo = "01",
                    CodigoPlanta = "13",
                    NumeroCVA = referenciaEDI,
                    CodigoTransportador = "9D82",
                    CodigoFornecedor = "CXC3",
                };

                cabecalho.Notas = new List<Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.Nota>();

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasPorReferencia)
                {
                    Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.Nota nota = new Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.Nota()
                    {
                        Tipo = "02",
                        SerieNotaFiscal = notaFiscal.Serie.TrimStart('0').PadLeft(3, ' '),
                        NumeroNotaFiscal = notaFiscal.Numero.ToString(),
                    };

                    cabecalho.Notas.Add(nota);
                }

                conemb.Cabecalhos.Add(cabecalho);
            }

            conemb.Rodape = new Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW.Rodape
            {
                Tipo = "03",
                QtdeCabecalhos = conemb.Cabecalhos.Count.ToString("D10"),
                QtdeNotas =  notasFiscais.Count.ToString("D10"),
            };

            return conemb;
        }

        public Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB ConverterCargaCTeParaCONEMB_MartinBrower(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.WebService.Pessoas.Pessoa svcPessoa = new WebService.Pessoas.Pessoa(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB conemb = new Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB();

            Dominio.Entidades.Empresa empresa = carga?.Empresa ?? ctes.FirstOrDefault()?.Empresa;
            string nomeRemetente = carga?.Pedidos.FirstOrDefault()?.Pedido.Remetente.Nome ?? ctes.FirstOrDefault()?.Remetente?.Nome;

            conemb.Data = DateTime.Now;
            conemb.Destinatario = empresa.RazaoSocial;
            conemb.Intercambio = "CON" + DateTime.Now.ToString("ddMMHHmm") + "1";
            conemb.Remetente = nomeRemetente;

            conemb.CabecalhoDocumento = new Dominio.ObjetosDeValor.EDI.CONEMB.CabecalhoDocumento();
            conemb.CabecalhoDocumento.IdentificacaoDocumento = "CONHE" + DateTime.Now.ToString("ddMMHHmm") + "1";

            conemb.CabecalhoDocumento.Transportadores = new List<Dominio.ObjetosDeValor.EDI.CONEMB.Transportador>();

            Dominio.ObjetosDeValor.EDI.CONEMB.Transportador transportador = new Dominio.ObjetosDeValor.EDI.CONEMB.Transportador();
            transportador.Pessoa = svcPessoa.ConverterObjetoEmpresa(empresa);

            transportador.ConhecimentosEmbarcados = new List<Dominio.ObjetosDeValor.EDI.CONEMB.CTeEmbarcado>();

            decimal valorFreteTotal = 0m;

            for (var i = 0; i < ctes.Count; i++)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];

                decimal pesoTotal = cte.QuantidadesCarga.Sum(o => o.Quantidade);
                valorFreteTotal += cte.ValorAReceber;

                Dominio.ObjetosDeValor.EDI.CONEMB.CTeEmbarcado cteEmbarcado = new Dominio.ObjetosDeValor.EDI.CONEMB.CTeEmbarcado();

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete> componentes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComponentesFrete>();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cte.CargaCTes)
                    componentes.AddRange(cargaCTe.Componentes);

                cteEmbarcado.CodigoCentroCusto = cte.Empresa.CodigoCentroCusto;
                cteEmbarcado.CodigoEstabelecimento = cte.Empresa.CodigoEstabelecimento;
                cteEmbarcado.Filial = cte.Empresa.RazaoSocial;
                cteEmbarcado.Serie = cte.Serie.Numero;
                cteEmbarcado.Numero = cte.Numero;
                cteEmbarcado.DataEmissao = cte.DataEmissao.Value;
                cteEmbarcado.CondicaoFrete = cte.CondicaoPagamento;
                cteEmbarcado.PesoTransportado = cte.Documentos.Sum(o => o.Peso);
                cteEmbarcado.ValorTotalFrete = cte.ValorAReceber;
                cteEmbarcado.BaseCalculoICMS = cte.BaseCalculoICMS;
                cteEmbarcado.AliquotaICMS = cte.AliquotaICMS;
                cteEmbarcado.ValorICMS = cte.ValorICMS;
                cteEmbarcado.ValorFretePorPeso = cte.ValorFrete;
                cteEmbarcado.ValorTaxas = cteEmbarcado.ValorTotalFrete - cteEmbarcado.ValorFretePorPeso;
                cteEmbarcado.ValorADVALOREM = (from obj in componentes where obj.ComponenteFrete != null && obj.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM select obj.ValorComponente).Sum();
                cteEmbarcado.ValorPedagio = (from obj in componentes where obj.ComponenteFrete != null && obj.ComponenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO select obj.ValorComponente).Sum();
                cteEmbarcado.SubstituicaoTributaria = cte.CST == "60" ? "1" : "2";
                cteEmbarcado.Filler = "FRT";
                cteEmbarcado.CNPJEmissorConhecimento = cte.Empresa.CNPJ;
                cteEmbarcado.CNPJTomadorServico = cte.Tomador?.CPF_CNPJ_SemFormato ?? new string('0', 14);

                for (int iDoc = 0; iDoc < cte.Documentos.Count && iDoc < 30; iDoc++)
                {
                    string strProp = (iDoc + 1).ToString();

                    int numeroNotaFiscal;
                    int.TryParse(cte.Documentos[iDoc].Numero, out numeroNotaFiscal);

                    Utilidades.Object.SetNestedPropertyValue(cteEmbarcado, "SerieNotaFiscal" + strProp, cte.Documentos[iDoc].SerieOuSerieDaChave);
                    Utilidades.Object.SetNestedPropertyValue(cteEmbarcado, "NumeroNotaFiscal" + strProp, string.Format("{0:00000000}", numeroNotaFiscal));
                }

                for (int iDoc = cte.Documentos.Count; iDoc < 27; iDoc++)
                {
                    string strProp = (iDoc + 1).ToString();

                    Utilidades.Object.SetNestedPropertyValue(cteEmbarcado, "NumeroNotaFiscal" + strProp, "00000000");
                }

                string cnpjRemetente = cte.Remetente?.CPF_CNPJ ?? new string('0', 14);
                string cnpjDestinatario = cte.Destinatario?.CPF_CNPJ ?? new string('0', 14);
                string chaveCTe = cte.Chave;
                string chaveMDFe = string.Empty;

                //if (carga.CargaMDFes.Count > 0)
                //    chaveMDFe = carga.CargaMDFes.Where(o => o.MDFe.MunicipiosDescarregamento.Any(m => m.Documentos.Any(d => d.CTe.Codigo == cte.Codigo))).FirstOrDefault()?.MDFe.Chave;

                if (string.IsNullOrWhiteSpace(chaveMDFe))
                    chaveMDFe = new string('0', 44);

                int numeroCarga = 0;

                if (carga != null)
                    int.TryParse(carga.CodigoCargaEmbarcador, out numeroCarga);

                int volumes = cte.XMLNotaFiscais.Sum(o => o.Volumes);

                DateTime dataColeta = cte.DataColeta ?? cte.DataEmissao.Value;
                string placaVeiculo = carga?.Veiculo?.Placa ?? cte.Veiculos.FirstOrDefault()?.Placa;


                cteEmbarcado.NumeroNotaFiscal27 = string.Format("{0:00000000}", volumes);
                cteEmbarcado.SerieNotaFiscal28 = "";
                cteEmbarcado.NumeroNotaFiscal28 = string.Format("{0:00000000}", (int)cte.ValorTotalMercadoria);
                cteEmbarcado.SerieNotaFiscal29 = "402";
                cteEmbarcado.NumeroNotaFiscal29 = "00" + dataColeta.ToString("yyMMdd");
                cteEmbarcado.SerieNotaFiscal30 = carga?.Veiculo.Placa.Substring(0, 3);
                cteEmbarcado.NumeroNotaFiscal30 = "0000" + placaVeiculo?.Substring(3, 4);
                cteEmbarcado.SerieNotaFiscal31 = cnpjRemetente.Substring(0, 3);
                cteEmbarcado.NumeroNotaFiscal31 = "00" + cte.DataEmissao.Value.ToString("yyMMdd");
                cteEmbarcado.SerieNotaFiscal32 = cnpjRemetente.Substring(3, 3);
                cteEmbarcado.NumeroNotaFiscal32 = string.Format("{0:00000000}", numeroCarga);
                cteEmbarcado.SerieNotaFiscal33 = cnpjRemetente.Substring(6, 3);
                cteEmbarcado.NumeroNotaFiscal33 = "00000000";
                cteEmbarcado.SerieNotaFiscal34 = cnpjRemetente.Substring(9, 3);

                string codigoIntegracaoCliente = Utilidades.String.Left(cte.XMLNotaFiscais.Select(o => o.CodigoIntegracaoCliente).FirstOrDefault(), 3);

                if (string.IsNullOrWhiteSpace(codigoIntegracaoCliente))
                    codigoIntegracaoCliente = "XXX";

                string tipoOperacaoMB = "002";

                if (cte.Remetente.Cliente != null && cte.Destinatario.Cliente != null && carga != null)
                {
                    if (cte.Remetente.Cliente.GrupoPessoas == carga.GrupoPessoaPrincipal && cte.Destinatario.Cliente.GrupoPessoas == carga.GrupoPessoaPrincipal)
                        tipoOperacaoMB = "003"; //Transferência
                    else if (cte.Remetente.Cliente.GrupoPessoas != carga.GrupoPessoaPrincipal)
                        tipoOperacaoMB = "001"; //Coleta
                }

                cteEmbarcado.NumeroNotaFiscal34 = tipoOperacaoMB + "07" + codigoIntegracaoCliente;
                cteEmbarcado.SerieNotaFiscal35 = cnpjRemetente.Substring(12, 2);
                cteEmbarcado.NumeroNotaFiscal35 = chaveCTe.Substring(20, 8);
                cteEmbarcado.SerieNotaFiscal36 = cnpjDestinatario.Substring(0, 3);
                cteEmbarcado.NumeroNotaFiscal36 = chaveCTe.Substring(28, 8);
                cteEmbarcado.SerieNotaFiscal37 = cnpjDestinatario.Substring(3, 3);
                cteEmbarcado.NumeroNotaFiscal37 = chaveCTe.Substring(36, 8);
                cteEmbarcado.SerieNotaFiscal38 = cnpjDestinatario.Substring(6, 3);
                cteEmbarcado.NumeroNotaFiscal38 = chaveMDFe.Substring(20, 8);
                cteEmbarcado.SerieNotaFiscal39 = cnpjDestinatario.Substring(9, 3);
                cteEmbarcado.NumeroNotaFiscal39 = chaveMDFe.Substring(28, 8);
                cteEmbarcado.SerieNotaFiscal40 = cnpjDestinatario.Substring(12, 2);
                cteEmbarcado.NumeroNotaFiscal40 = chaveMDFe.Substring(36, 8);

                cteEmbarcado.AcaoDocumento = "I";
                cteEmbarcado.TipoConhecimento = "N";
                cteEmbarcado.IndicacaoRepeticao = "U";
                cteEmbarcado.CFOP = " " + cte.CFOP.CodigoCFOP.ToString();

                cteEmbarcado.DadosComplementares = new Dominio.ObjetosDeValor.EDI.CONEMB.DadosComplementaresCTeEmbarcado();
                cteEmbarcado.DadosComplementares.TipoMeioTransporte = "BR11";
                cteEmbarcado.DadosComplementares.FilialEmissora = cte.Empresa.RazaoSocial;
                cteEmbarcado.DadosComplementares.Serie = cte.Serie.Numero;
                cteEmbarcado.DadosComplementares.Numero = cte.Numero;
                cteEmbarcado.DadosComplementares.CodigoLoja = cnpjDestinatario;
                cteEmbarcado.DadosComplementares.NumeroViagem = carga?.CodigoCargaEmbarcador;

                transportador.ConhecimentosEmbarcados.Add(cteEmbarcado);
            }

            conemb.CabecalhoDocumento.Transportadores.Add(transportador);

            conemb.CabecalhoDocumento.Total = new Dominio.ObjetosDeValor.EDI.CONEMB.Total()
            {
                QuantidadeTotalCTe = ctes.Count,
                ValorTotalCTe = valorFreteTotal
            };

            return conemb;
        }

        public Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarImportacao ConverterCargaParaCONEMB_CaterpillarImportacao(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unidadeTrabalho);
            Repositorio.ComponentePrestacaoCTE repComponentes = new Repositorio.ComponentePrestacaoCTE(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;

            if (cargaEDIIntegracao.CTe != null)
                cargaCTes = repCargaCTe.BuscarTodosPorCTe(cargaEDIIntegracao.CTe.Codigo);
            else if (cargaEDIIntegracao.LayoutEDI.AgruparPorRemetente && cargaEDIIntegracao.Remetente != null)
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo, cargaEDIIntegracao.Remetente.CPF_CNPJ);
            else
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo, 0D);

            Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarImportacao conemb = new Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarImportacao();

            conemb.Identificador = "ALI";
            conemb.Data = DateTime.Now;
            conemb.QuantidadeRegistro = 0;
            conemb.Componentes = new List<Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao>();

            int qtdRegistro = 0;

            for (var i = 0; i < cargaCTes.Count; i++)
            {
                decimal baseICMSLinhas = 0;
                decimal valorICMSLinhas = 0;
                qtdRegistro++;
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTes[i];
                Dominio.Entidades.InformacaoCargaCTE informacaoCarga = repInformacaoCarga.BuscarPorCTeUnidade(cargaCTe.CTe.Codigo, "01");
                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repFaturaDocumento.BuscarFaturaPorCTe(cargaCTe.CTe.Codigo);
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = null;
                if (faturaDocumento != null && faturaDocumento.Fatura != null)
                    titulo = repTitulo.BuscarPorFatura(faturaDocumento.Fatura.Codigo)?.FirstOrDefault() ?? null;
                if (titulo == null && faturaDocumento != null)
                    titulo = repTitulo.BuscarPorFaturaDocumento(faturaDocumento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)?.FirstOrDefault() ?? null;
                if (titulo == null)
                    titulo = repTitulo.BuscarTituloDocumentoPorCTe(cargaCTe.CTe.Codigo);
                if (titulo == null)
                    titulo = repTitulo.BuscarPorCTe(cargaCTe.CTe.Codigo);
                if (titulo == null)
                    titulo = repTituloDocumento.BuscarTitulosEmAbertoPorCTe(cargaCTe.CTe.Codigo)?.FirstOrDefault() ?? null;

                decimal valorICMSCTe = Math.Round((cargaCTe.CTe.ValorICMS), 2, MidpointRounding.ToEven);
                decimal baseICMSCTe = Math.Round((cargaCTe.CTe.BaseCalculoICMS), 2, MidpointRounding.ToEven);

                decimal baseICMSFrete = Math.Round((cargaCTe.CTe.ValorFrete), 2, MidpointRounding.ToEven);
                decimal valorICMSFrete = Math.Round(((cargaCTe.CTe.ValorFrete) * cargaCTe.CTe.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                decimal baseAliquota = 0m;
                if (cargaCTe.CTe.CST != "60")
                {
                    baseAliquota = (100 - cargaCTe.CTe.AliquotaICMS) / 100;
                    baseICMSFrete = Math.Round((cargaCTe.CTe.ValorFrete / baseAliquota), 2, MidpointRounding.ToEven);
                    valorICMSFrete = Math.Round((baseICMSFrete * cargaCTe.CTe.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                }
                baseICMSLinhas += baseICMSFrete;
                valorICMSLinhas += valorICMSFrete;

                string embarque = cargaCTe.CTe.Embarque;
                string numeroDI = cargaCTe.CTe.NumeroDI;
                string masterBL = cargaCTe.CTe.MasterBL;
                if (string.IsNullOrWhiteSpace(embarque) && cargaCTe.CTe.XMLNotaFiscais != null && cargaCTe.CTe.XMLNotaFiscais.Count > 0)
                    embarque = cargaCTe.CTe.XMLNotaFiscais.Where(n => n.Embarque != null && n.Embarque != "").Select(c => c.Embarque)?.FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(numeroDI) && cargaCTe.CTe.XMLNotaFiscais != null && cargaCTe.CTe.XMLNotaFiscais.Count > 0)
                    numeroDI = cargaCTe.CTe.XMLNotaFiscais.Where(n => n.NumeroDI != null && n.NumeroDI != "").Select(c => c.NumeroDI)?.FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(masterBL) && cargaCTe.CTe.XMLNotaFiscais != null && cargaCTe.CTe.XMLNotaFiscais.Count > 0)
                    masterBL = cargaCTe.CTe.XMLNotaFiscais.Where(n => n.MasterBL != null && n.MasterBL != "").Select(c => c.MasterBL)?.FirstOrDefault() ?? "";

                Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao componente = new Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao()
                {
                    TipoLancamento = "01",
                    NumeroDocumento = faturaDocumento != null && faturaDocumento.Fatura != null ? Utilidades.String.OnlyNumbers(faturaDocumento.Fatura.Numero.ToString("n0")) : titulo != null ? Utilidades.String.OnlyNumbers(titulo.Codigo.ToString("n0")) : Utilidades.String.OnlyNumbers(cargaCTe.Carga.Protocolo.ToString("n0")),
                    DataEmissao = faturaDocumento != null && faturaDocumento.Fatura != null ? faturaDocumento.Fatura.DataFatura : titulo != null ? titulo.DataEmissao.Value : cargaCTe.Carga.DataCriacaoCarga,
                    DataVencimento = titulo != null ? titulo.DataVencimento.Value : cargaCTe.CTe.DataPreviaVencimento.HasValue ? cargaCTe.CTe.DataPreviaVencimento.Value : cargaCTe.Carga.DataCriacaoCarga,
                    Embarque = embarque,
                    NumeroDIEmbarque = numeroDI,
                    MasterBL = masterBL,
                    NumeroHouse = "",
                    QuantidadeRegistro = qtdRegistro,
                    ValorDespesaMoeda = cargaCTe.CTe.ValorFrete,
                    CodigoMoeda = "790",
                    ValorDespesaMoedaNacional = cargaCTe.CTe.ValorFrete,
                    PesoBruto = informacaoCarga?.Quantidade ?? 0,
                    PesoLiquido = informacaoCarga?.Quantidade ?? 0,
                    NumeroDias = 0,
                    NumeroContainer = 0,
                    TipoContainer = "",
                    NumeroCTe = Utilidades.String.OnlyNumbers(cargaCTe.CTe.Numero.ToString("n0")),
                    SerieCTe = Utilidades.String.OnlyNumbers(cargaCTe.CTe.Serie.Numero.ToString("n0")),
                    DataCTe = cargaCTe.CTe.DataEmissao.Value,
                    CNPJEmitente = cargaCTe.CTe.Empresa.CNPJ,
                    BaseICMS = cargaCTe.CTe.CST != "60" ? baseICMSFrete : 0,
                    AliquotaICMS = cargaCTe.CTe.CST != "60" ? cargaCTe.CTe.AliquotaICMS : 0,
                    ValorICMS = cargaCTe.CTe.CST != "60" ? valorICMSFrete : 0,
                    BaseICMSST = cargaCTe.CTe.CST == "60" ? baseICMSFrete : 0,
                    AliquotaICMSST = cargaCTe.CTe.CST == "60" ? cargaCTe.CTe.AliquotaICMS : 0,
                    ValorICMSST = cargaCTe.CTe.CST == "60" ? valorICMSFrete : 0
                };
                conemb.Componentes.Add(componente);

                if (cargaCTe.CTe.CST != "60")
                {
                    baseICMSCTe -= baseICMSFrete;
                    valorICMSCTe -= valorICMSFrete;
                }

                Dominio.Entidades.ComponentePrestacaoCTE componenteAdValorem = repComponentes.BuscarPorCTeTipo(cargaCTe.CTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)?.FirstOrDefault() ?? null;
                Dominio.Entidades.ComponentePrestacaoCTE componentePedagio = repComponentes.BuscarPorCTeTipo(cargaCTe.CTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO)?.FirstOrDefault() ?? null;
                if (componenteAdValorem != null && componenteAdValorem.Valor > 0)
                {
                    qtdRegistro++;
                    decimal baseICMS = Math.Round(componenteAdValorem.Valor, 2, MidpointRounding.ToEven);
                    decimal valorICMS = Math.Round((componenteAdValorem.Valor * cargaCTe.CTe.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                    if (cargaCTe.CTe.CST != "60")
                    {
                        baseICMS = Math.Round((componenteAdValorem.Valor / baseAliquota), 2, MidpointRounding.ToEven);
                        valorICMS = Math.Round((baseICMS * cargaCTe.CTe.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                    }

                    baseICMSLinhas += baseICMS;
                    valorICMSLinhas += valorICMS;

                    Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao componenteAd = new Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao()
                    {
                        TipoLancamento = "03",
                        NumeroDocumento = faturaDocumento != null && faturaDocumento.Fatura != null ? Utilidades.String.OnlyNumbers(faturaDocumento.Fatura.Numero.ToString("n0")) : titulo != null ? Utilidades.String.OnlyNumbers(titulo.Codigo.ToString("n0")) : Utilidades.String.OnlyNumbers(cargaCTe.Carga.Protocolo.ToString("n0")),
                        DataEmissao = faturaDocumento != null && faturaDocumento.Fatura != null ? faturaDocumento.Fatura.DataFatura : titulo != null ? titulo.DataEmissao.Value : cargaCTe.Carga.DataCriacaoCarga,
                        DataVencimento = titulo != null ? titulo.DataVencimento.Value : cargaCTe.CTe.DataPreviaVencimento.HasValue ? cargaCTe.CTe.DataPreviaVencimento.Value : cargaCTe.Carga.DataCriacaoCarga,
                        Embarque = embarque,
                        NumeroDIEmbarque = numeroDI,
                        MasterBL = masterBL,
                        NumeroHouse = "",
                        QuantidadeRegistro = qtdRegistro,
                        ValorDespesaMoeda = componenteAdValorem.Valor,
                        CodigoMoeda = "790",
                        ValorDespesaMoedaNacional = componenteAdValorem.Valor,
                        PesoBruto = informacaoCarga?.Quantidade ?? 0,
                        PesoLiquido = informacaoCarga?.Quantidade ?? 0,
                        NumeroDias = 0,
                        NumeroContainer = 0,
                        TipoContainer = "",
                        NumeroCTe = Utilidades.String.OnlyNumbers(cargaCTe.CTe.Numero.ToString("n0")),
                        SerieCTe = Utilidades.String.OnlyNumbers(cargaCTe.CTe.Serie.Numero.ToString("n0")),
                        DataCTe = cargaCTe.CTe.DataEmissao.Value,
                        CNPJEmitente = cargaCTe.CTe.Empresa.CNPJ,
                        BaseICMS = cargaCTe.CTe.CST != "60" ? baseICMS : 0,
                        AliquotaICMS = cargaCTe.CTe.CST != "60" ? cargaCTe.CTe.AliquotaICMS : 0,
                        ValorICMS = cargaCTe.CTe.CST != "60" ? valorICMS : 0,
                        BaseICMSST = cargaCTe.CTe.CST == "60" ? baseICMS : 0,
                        AliquotaICMSST = cargaCTe.CTe.CST == "60" ? cargaCTe.CTe.AliquotaICMS : 0,
                        ValorICMSST = cargaCTe.CTe.CST == "60" ? valorICMS : 0
                    };
                    conemb.Componentes.Add(componenteAd);

                    if (cargaCTe.CTe.CST != "60")
                    {
                        baseICMSCTe -= baseICMS;
                        valorICMSCTe -= valorICMS;
                    }

                }

                if (componentePedagio != null && componentePedagio.Valor > 0)
                {
                    qtdRegistro++;
                    decimal baseICMS = Math.Round(componentePedagio.Valor, 2, MidpointRounding.ToEven);
                    decimal valorICMS = Math.Round((componentePedagio.Valor * cargaCTe.CTe.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                    if (cargaCTe.CTe.CST != "60")
                    {
                        baseICMS = Math.Round((componentePedagio.Valor / baseAliquota), 2, MidpointRounding.ToEven);
                        valorICMS = Math.Round((baseICMS * cargaCTe.CTe.AliquotaICMS / 100), 2, MidpointRounding.ToEven);

                        if (baseICMSCTe != 0.00m)
                            baseICMS = Math.Round((baseICMSCTe), 2, MidpointRounding.ToEven);

                        if (valorICMSCTe != 0.00m)
                            valorICMS = Math.Round((valorICMSCTe), 2, MidpointRounding.ToEven);
                    }
                    baseICMSLinhas += baseICMS;
                    valorICMSLinhas += valorICMS;

                    Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao componenteAd = new Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao()
                    {
                        TipoLancamento = "04",
                        NumeroDocumento = faturaDocumento != null && faturaDocumento.Fatura != null ? Utilidades.String.OnlyNumbers(faturaDocumento.Fatura.Numero.ToString("n0")) : titulo != null ? Utilidades.String.OnlyNumbers(titulo.Codigo.ToString("n0")) : Utilidades.String.OnlyNumbers(cargaCTe.Carga.Protocolo.ToString("n0")),
                        DataEmissao = faturaDocumento != null && faturaDocumento.Fatura != null ? faturaDocumento.Fatura.DataFatura : titulo != null ? titulo.DataEmissao.Value : cargaCTe.Carga.DataCriacaoCarga,
                        DataVencimento = titulo != null ? titulo.DataVencimento.Value : cargaCTe.CTe.DataPreviaVencimento.HasValue ? cargaCTe.CTe.DataPreviaVencimento.Value : cargaCTe.Carga.DataCriacaoCarga,
                        Embarque = embarque,
                        NumeroDIEmbarque = numeroDI,
                        MasterBL = masterBL,
                        NumeroHouse = "",
                        QuantidadeRegistro = qtdRegistro,
                        ValorDespesaMoeda = componentePedagio.Valor,
                        CodigoMoeda = "790",
                        ValorDespesaMoedaNacional = componentePedagio.Valor,
                        PesoBruto = informacaoCarga?.Quantidade ?? 0,
                        PesoLiquido = informacaoCarga?.Quantidade ?? 0,
                        NumeroDias = 0,
                        NumeroContainer = 0,
                        TipoContainer = "",
                        NumeroCTe = Utilidades.String.OnlyNumbers(cargaCTe.CTe.Numero.ToString("n0")),
                        SerieCTe = Utilidades.String.OnlyNumbers(cargaCTe.CTe.Serie.Numero.ToString("n0")),
                        DataCTe = cargaCTe.CTe.DataEmissao.Value,
                        CNPJEmitente = cargaCTe.CTe.Empresa.CNPJ,
                        BaseICMS = cargaCTe.CTe.CST != "60" ? baseICMS : 0,
                        AliquotaICMS = cargaCTe.CTe.CST != "60" ? cargaCTe.CTe.AliquotaICMS : 0,
                        ValorICMS = cargaCTe.CTe.CST != "60" ? valorICMS : 0,
                        BaseICMSST = cargaCTe.CTe.CST == "60" ? baseICMS : 0,
                        AliquotaICMSST = cargaCTe.CTe.CST == "60" ? cargaCTe.CTe.AliquotaICMS : 0,
                        ValorICMSST = cargaCTe.CTe.CST == "60" ? valorICMS : 0
                    };
                    conemb.Componentes.Add(componenteAd);
                }
            }

            conemb.QuantidadeRegistro = qtdRegistro;

            return conemb;
        }

        public Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarImportacao ConverterCargaCTeParaCONEMB_CaterpillarImportacao(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unidadeTrabalho);
            Repositorio.ComponentePrestacaoCTE repComponentes = new Repositorio.ComponentePrestacaoCTE(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarImportacao conemb = new Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarImportacao();

            conemb.Identificador = "ALI";
            conemb.Data = DateTime.Now;
            conemb.QuantidadeRegistro = 0;
            conemb.Componentes = new List<Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao>();

            int qtdRegistro = 0;

            for (var i = 0; i < ctes.Count; i++)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];

                decimal baseICMSLinhas = 0;
                decimal valorICMSLinhas = 0;
                qtdRegistro++;
                Dominio.Entidades.InformacaoCargaCTE informacaoCarga = repInformacaoCarga.BuscarPorCTeUnidade(cte.Codigo, "01");
                Dominio.Entidades.Embarcador.Fatura.FaturaDocumento faturaDocumento = repFaturaDocumento.BuscarFaturaPorCTe(cte.Codigo);
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = null;
                if (faturaDocumento != null && faturaDocumento.Fatura != null)
                    titulo = repTitulo.BuscarPorFatura(faturaDocumento.Fatura.Codigo)?.FirstOrDefault() ?? null;
                if (titulo == null && faturaDocumento != null)
                    titulo = repTitulo.BuscarPorFaturaDocumento(faturaDocumento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)?.FirstOrDefault() ?? null;
                if (titulo == null)
                    titulo = repTitulo.BuscarTituloDocumentoPorCTe(cte.Codigo);
                if (titulo == null)
                    titulo = repTitulo.BuscarPorCTe(cte.Codigo);
                if (titulo == null)
                    titulo = repTituloDocumento.BuscarTitulosEmAbertoPorCTe(cte.Codigo)?.FirstOrDefault() ?? null;

                decimal valorICMSCTe = Math.Round((cte.ValorICMS), 2, MidpointRounding.ToEven);
                decimal baseICMSCTe = Math.Round((cte.BaseCalculoICMS), 2, MidpointRounding.ToEven);

                decimal baseICMSFrete = Math.Round((cte.ValorFrete), 2, MidpointRounding.ToEven);
                decimal valorICMSFrete = Math.Round(((cte.ValorFrete) * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                decimal baseAliquota = 0m;
                if (cte.CST != "60")
                {
                    baseAliquota = (100 - cte.AliquotaICMS) / 100;
                    baseICMSFrete = Math.Round((cte.ValorFrete / baseAliquota), 2, MidpointRounding.ToEven);
                    valorICMSFrete = Math.Round((baseICMSFrete * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                }

                baseICMSLinhas += baseICMSFrete;
                valorICMSLinhas += valorICMSFrete;

                string embarque = cte.Embarque;
                string numeroDI = cte.NumeroDI;
                string masterBL = cte.MasterBL;
                if (string.IsNullOrWhiteSpace(embarque) && cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0)
                    embarque = cte.XMLNotaFiscais.Where(n => n.Embarque != null && n.Embarque != "").Select(c => c.Embarque)?.FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(numeroDI) && cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0)
                    numeroDI = cte.XMLNotaFiscais.Where(n => n.NumeroDI != null && n.NumeroDI != "").Select(c => c.NumeroDI)?.FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(masterBL) && cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0)
                    masterBL = cte.XMLNotaFiscais.Where(n => n.MasterBL != null && n.MasterBL != "").Select(c => c.MasterBL)?.FirstOrDefault() ?? "";

                Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao componente = new Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao()
                {
                    TipoLancamento = "01",
                    NumeroDocumento = faturaDocumento != null && faturaDocumento.Fatura != null ? Utilidades.String.OnlyNumbers(faturaDocumento.Fatura.Numero.ToString("n0")) : titulo != null ? Utilidades.String.OnlyNumbers(titulo.Codigo.ToString("n0")) : Utilidades.String.OnlyNumbers(carga?.Protocolo.ToString("n0") ?? "0"),
                    DataEmissao = faturaDocumento != null && faturaDocumento.Fatura != null ? faturaDocumento.Fatura.DataFatura : titulo != null ? titulo.DataEmissao.Value : carga?.DataCriacaoCarga ?? DateTime.Now,
                    DataVencimento = titulo != null ? titulo.DataVencimento.Value : cte.DataPreviaVencimento.HasValue ? cte.DataPreviaVencimento.Value : carga?.DataCriacaoCarga ?? DateTime.Now,
                    Embarque = embarque,
                    NumeroDIEmbarque = numeroDI,
                    MasterBL = masterBL,
                    NumeroHouse = "",
                    QuantidadeRegistro = qtdRegistro,
                    ValorDespesaMoeda = cte.ValorFrete,
                    CodigoMoeda = "790",
                    ValorDespesaMoedaNacional = cte.ValorFrete,
                    PesoBruto = informacaoCarga?.Quantidade ?? 0,
                    PesoLiquido = informacaoCarga?.Quantidade ?? 0,
                    NumeroDias = 0,
                    NumeroContainer = 0,
                    TipoContainer = "",
                    NumeroCTe = Utilidades.String.OnlyNumbers(cte.Numero.ToString("n0")),
                    SerieCTe = Utilidades.String.OnlyNumbers(cte.Serie.Numero.ToString("n0")),
                    DataCTe = cte.DataEmissao.Value,
                    CNPJEmitente = cte.Empresa.CNPJ,
                    BaseICMS = cte.CST != "60" ? baseICMSFrete : 0,
                    AliquotaICMS = cte.CST != "60" ? cte.AliquotaICMS : 0,
                    ValorICMS = cte.CST != "60" ? valorICMSFrete : 0,
                    BaseICMSST = cte.CST == "60" ? baseICMSFrete : 0,
                    AliquotaICMSST = cte.CST == "60" ? cte.AliquotaICMS : 0,
                    ValorICMSST = cte.CST == "60" ? valorICMSFrete : 0
                };
                conemb.Componentes.Add(componente);

                if (cte.CST != "60")
                {
                    baseICMSCTe -= baseICMSFrete;
                    valorICMSCTe -= valorICMSFrete;
                }

                Dominio.Entidades.ComponentePrestacaoCTE componenteAdValorem = repComponentes.BuscarPorCTeTipo(cte.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)?.FirstOrDefault() ?? null;
                Dominio.Entidades.ComponentePrestacaoCTE componentePedagio = repComponentes.BuscarPorCTeTipo(cte.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO)?.FirstOrDefault() ?? null;
                if (componenteAdValorem != null && componenteAdValorem.Valor > 0)
                {
                    qtdRegistro++;
                    decimal baseICMS = Math.Round(componenteAdValorem.Valor, 2, MidpointRounding.ToEven);
                    decimal valorICMS = Math.Round((componenteAdValorem.Valor * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                    if (cte.CST != "60")
                    {
                        baseICMS = Math.Round((componenteAdValorem.Valor / baseAliquota), 2, MidpointRounding.ToEven);
                        valorICMS = Math.Round((baseICMS * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                    }

                    baseICMSLinhas += baseICMS;
                    valorICMSLinhas += valorICMS;

                    Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao componenteAd = new Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao()
                    {
                        TipoLancamento = "03",
                        NumeroDocumento = faturaDocumento != null && faturaDocumento.Fatura != null ? Utilidades.String.OnlyNumbers(faturaDocumento.Fatura.Numero.ToString("n0")) : titulo != null ? Utilidades.String.OnlyNumbers(titulo.Codigo.ToString("n0")) : Utilidades.String.OnlyNumbers(carga?.Protocolo.ToString("n0") ?? "0"),
                        DataEmissao = faturaDocumento != null && faturaDocumento.Fatura != null ? faturaDocumento.Fatura.DataFatura : titulo != null ? titulo.DataEmissao.Value : carga?.DataCriacaoCarga ?? DateTime.Now,
                        DataVencimento = titulo != null ? titulo.DataVencimento.Value : cte.DataPreviaVencimento.HasValue ? cte.DataPreviaVencimento.Value : carga?.DataCriacaoCarga ?? DateTime.Now,
                        Embarque = embarque,
                        NumeroDIEmbarque = numeroDI,
                        MasterBL = masterBL,
                        NumeroHouse = "",
                        QuantidadeRegistro = qtdRegistro,
                        ValorDespesaMoeda = componenteAdValorem.Valor,
                        CodigoMoeda = "790",
                        ValorDespesaMoedaNacional = componenteAdValorem.Valor,
                        PesoBruto = informacaoCarga?.Quantidade ?? 0,
                        PesoLiquido = informacaoCarga?.Quantidade ?? 0,
                        NumeroDias = 0,
                        NumeroContainer = 0,
                        TipoContainer = "",
                        NumeroCTe = Utilidades.String.OnlyNumbers(cte.Numero.ToString("n0")),
                        SerieCTe = Utilidades.String.OnlyNumbers(cte.Serie.Numero.ToString("n0")),
                        DataCTe = cte.DataEmissao.Value,
                        CNPJEmitente = cte.Empresa.CNPJ,
                        BaseICMS = cte.CST != "60" ? baseICMS : 0,
                        AliquotaICMS = cte.CST != "60" ? cte.AliquotaICMS : 0,
                        ValorICMS = cte.CST != "60" ? valorICMS : 0,
                        BaseICMSST = cte.CST == "60" ? baseICMS : 0,
                        AliquotaICMSST = cte.CST == "60" ? cte.AliquotaICMS : 0,
                        ValorICMSST = cte.CST == "60" ? valorICMS : 0
                    };
                    conemb.Componentes.Add(componenteAd);


                    if (cte.CST != "60")
                    {
                        baseICMSCTe -= baseICMS;
                        valorICMSCTe -= valorICMS;
                    }
                }

                if (componentePedagio != null && componentePedagio.Valor > 0)
                {
                    qtdRegistro++;
                    decimal baseICMS = Math.Round(componentePedagio.Valor, 2, MidpointRounding.ToEven);
                    decimal valorICMS = Math.Round((componentePedagio.Valor * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);
                    if (cte.CST != "60")
                    {
                        baseICMS = Math.Round((componentePedagio.Valor / baseAliquota), 2, MidpointRounding.ToEven);
                        valorICMS = Math.Round((baseICMS * cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);

                        if (baseICMSCTe != 0.00m)
                            baseICMS = Math.Round((baseICMSCTe), 2, MidpointRounding.ToEven);

                        if (valorICMSCTe != 0.00m)
                            valorICMS = Math.Round((valorICMSCTe), 2, MidpointRounding.ToEven);
                    }
                    baseICMSLinhas += baseICMS;
                    valorICMSLinhas += valorICMS;

                    Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao componenteAd = new Dominio.ObjetosDeValor.EDI.CONEMB.ComponenteCaterpillarImportacao()
                    {
                        TipoLancamento = "04",
                        NumeroDocumento = faturaDocumento != null && faturaDocumento.Fatura != null ? Utilidades.String.OnlyNumbers(faturaDocumento.Fatura.Numero.ToString("n0")) : titulo != null ? Utilidades.String.OnlyNumbers(titulo.Codigo.ToString("n0")) : Utilidades.String.OnlyNumbers(carga?.Protocolo.ToString("n0") ?? "0"),
                        DataEmissao = faturaDocumento != null && faturaDocumento.Fatura != null ? faturaDocumento.Fatura.DataFatura : titulo != null ? titulo.DataEmissao.Value : carga?.DataCriacaoCarga ?? DateTime.Now,
                        DataVencimento = titulo != null ? titulo.DataVencimento.Value : cte.DataPreviaVencimento.HasValue ? cte.DataPreviaVencimento.Value : carga?.DataCriacaoCarga ?? DateTime.Now,
                        Embarque = embarque,
                        NumeroDIEmbarque = numeroDI,
                        MasterBL = masterBL,
                        NumeroHouse = "",
                        QuantidadeRegistro = qtdRegistro,
                        ValorDespesaMoeda = componentePedagio.Valor,
                        CodigoMoeda = "790",
                        ValorDespesaMoedaNacional = componentePedagio.Valor,
                        PesoBruto = informacaoCarga?.Quantidade ?? 0,
                        PesoLiquido = informacaoCarga?.Quantidade ?? 0,
                        NumeroDias = 0,
                        NumeroContainer = 0,
                        TipoContainer = "",
                        NumeroCTe = Utilidades.String.OnlyNumbers(cte.Numero.ToString("n0")),
                        SerieCTe = Utilidades.String.OnlyNumbers(cte.Serie.Numero.ToString("n0")),
                        DataCTe = cte.DataEmissao.Value,
                        CNPJEmitente = cte.Empresa.CNPJ,
                        BaseICMS = cte.CST != "60" ? baseICMS : 0,
                        AliquotaICMS = cte.CST != "60" ? cte.AliquotaICMS : 0,
                        ValorICMS = cte.CST != "60" ? valorICMS : 0,
                        BaseICMSST = cte.CST == "60" ? baseICMS : 0,
                        AliquotaICMSST = cte.CST == "60" ? cte.AliquotaICMS : 0,
                        ValorICMSST = cte.CST == "60" ? valorICMS : 0
                    };
                    conemb.Componentes.Add(componenteAd);
                }
            }
            conemb.QuantidadeRegistro = qtdRegistro;
            return conemb;
        }

        public Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarExportacao ConverterCargaParaCONEMB_CaterpillarExportacao(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unidadeTrabalho);
            Repositorio.ComponentePrestacaoCTE repComponentes = new Repositorio.ComponentePrestacaoCTE(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = null;

            if (cargaEDIIntegracao.CTe != null)
                cargaCTes = repCargaCTe.BuscarTodosPorCTe(cargaEDIIntegracao.CTe.Codigo);
            else if (cargaEDIIntegracao.LayoutEDI.AgruparPorRemetente && cargaEDIIntegracao.Remetente != null)
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo, cargaEDIIntegracao.Remetente.CPF_CNPJ);
            else
                cargaCTes = repCargaCTe.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo, 0D);

            Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarExportacao conemb = new Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarExportacao();

            conemb.IdentificadoRegistro = "ITP";
            conemb.IdentificadorProcesso = 15;
            conemb.NumeroVersaoTransacao = 3;
            conemb.NumeroControleTransmissao = 22003;
            conemb.IdentificadorGeracaoMovimento = DateTime.Now;
            conemb.IdentificadorTransmissor = cargaEDIIntegracao.Carga.Empresa.CNPJ;
            conemb.IdentificadorReceptor = "61064911000177";
            conemb.CodigoInternoTransmissor = "Q5875F1";
            conemb.CodigoInternoReceptor = "GE";
            conemb.NomeTransmissor = cargaEDIIntegracao.Carga.Empresa.RazaoSocial;
            conemb.NomeReceptor = "CATERPILLAR BRASIL LTDA";
            conemb.Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.CONEMB.ConhecimentoCaterpillarExportacao>();
            conemb.FTP = new Dominio.ObjetosDeValor.EDI.CONEMB.RodapeCaterpillarExportacao();

            int qtdRegistro = 0;
            decimal totalValores = 0;

            for (var i = 0; i < cargaCTes.Count; i++)
            {
                qtdRegistro++;
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCTes[i];
                Dominio.Entidades.InformacaoCargaCTE informacaoCarga = repInformacaoCarga.BuscarPorCTeUnidade(cargaCTe.CTe.Codigo, "01");
                Dominio.Entidades.InformacaoCargaCTE informacaoCargaVolume = repInformacaoCarga.BuscarPorCTeUnidade(cargaCTe.CTe.Codigo, "03");

                string embarque = cargaCTe.CTe.Embarque;
                string numeroDI = cargaCTe.CTe.NumeroDI;
                string masterBL = cargaCTe.CTe.MasterBL;
                if (string.IsNullOrWhiteSpace(embarque) && cargaCTe.CTe.XMLNotaFiscais != null && cargaCTe.CTe.XMLNotaFiscais.Count > 0)
                    embarque = cargaCTe.CTe.XMLNotaFiscais.Where(n => n.Embarque != null && n.Embarque != "").Select(c => c.Embarque)?.FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(numeroDI) && cargaCTe.CTe.XMLNotaFiscais != null && cargaCTe.CTe.XMLNotaFiscais.Count > 0)
                    numeroDI = cargaCTe.CTe.XMLNotaFiscais.Where(n => n.NumeroDI != null && n.NumeroDI != "").Select(c => c.NumeroDI)?.FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(masterBL) && cargaCTe.CTe.XMLNotaFiscais != null && cargaCTe.CTe.XMLNotaFiscais.Count > 0)
                    masterBL = cargaCTe.CTe.XMLNotaFiscais.Where(n => n.MasterBL != null && n.MasterBL != "").Select(c => c.MasterBL)?.FirstOrDefault() ?? "";

                Dominio.ObjetosDeValor.EDI.CONEMB.ConhecimentoCaterpillarExportacao conhecimento = new Dominio.ObjetosDeValor.EDI.CONEMB.ConhecimentoCaterpillarExportacao()
                {
                    IdentificadoRegistro = "CT1",
                    Numero = cargaCTe.CTe.Numero,
                    Serie = cargaCTe.CTe.Serie.Numero,
                    DataEmissao = cargaCTe.CTe.DataEmissao.Value,
                    QuantidadeNotas = cargaCTe.CTe.Documentos.Count(),
                    ValorNotas = cargaCTe.CTe.ValorTotalMercadoria,
                    Valor = cargaCTe.CTe.ValorAReceber,
                    CodigoFiscal = cargaCTe.CTe.CFOP.CodigoCFOP,
                    ModalidadeFrete = cargaCTe.CTe.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar ? "C" : "F",
                    SituacaoTributaria = cargaCTe.CTe.CST == "60" ? "1" : "2",
                    Peso = informacaoCarga?.Quantidade ?? 0,
                    Volume = informacaoCargaVolume?.Quantidade ?? 0,
                    BaseICMS = cargaCTe.CTe.BaseCalculoICMS,
                    AliquotaICMS = cargaCTe.CTe.AliquotaICMS,
                    ValorICMS = cargaCTe.CTe.ValorICMS,
                    IdentificacaoLocalEntrega = cargaCTe.CTe.LocalidadeTerminoPrestacao.Descricao,
                    LocalColeta = cargaCTe.CTe.LocalidadeInicioPrestacao.Descricao,
                    UnidadePeso = "KG",
                    UnidadeVolume = "UN",
                    CT2 = new Dominio.ObjetosDeValor.EDI.CONEMB.ComplementoConhecimentoCaterpillarExportacao()
                    {
                        IdentificadoRegistro = "CT2",
                        ValorSEC = cargaCTe.CTe.ComponentesPrestacao.Select(c => c.ValorSECCAT)?.Sum() ?? 0,
                        ValorITR = cargaCTe.CTe.ComponentesPrestacao.Select(c => c.ValorITR)?.Sum() ?? 0,
                        ValorDespacho = cargaCTe.CTe.ComponentesPrestacao.Select(c => c.ValorDespacho)?.Sum() ?? 0,
                        ValorPedagio = cargaCTe.CTe.ComponentesPrestacao.Select(c => c.ValorPedagio)?.Sum() ?? 0,
                        ValorAdeme = 0,
                        ValorADValorem = cargaCTe.CTe.ComponentesPrestacao.Select(c => c.ValorAdValorem)?.Sum() ?? 0,
                        FretePeso = 0,
                        ValorSUFRAMA = 0,
                        OutrosValores = cargaCTe.CTe.ComponentesPrestacao.Select(c => c.ValorAdicionais)?.Sum() ?? 0,
                        ValorIRRF = cargaCTe.CTe.ComponentesPrestacao.Select(c => c.ValorIRRF)?.Sum() ?? 0
                    },
                    CT4 = new Dominio.ObjetosDeValor.EDI.CONEMB.DestinatarioConhecimentoCaterpillarExportacao()
                    {
                        IdentificadoRegistro = "CT4",
                        Nome = cargaCTe.CTe.Recebedor?.Nome ?? cargaCTe.CTe.Destinatario.Nome,
                        CNPJ = cargaCTe.CTe.Recebedor?.CPF_CNPJ ?? cargaCTe.CTe.Destinatario.CPF_CNPJ,
                        IE = cargaCTe.CTe.Recebedor?.IE_RG ?? cargaCTe.CTe.Destinatario.IE_RG,
                        Endereco = cargaCTe.CTe.Recebedor?.Endereco ?? cargaCTe.CTe.Destinatario.Endereco,
                        Municipio = cargaCTe.CTe.Recebedor?.Localidade.Descricao ?? cargaCTe.CTe.Destinatario.Localidade.Descricao,
                        UF = cargaCTe.CTe.Recebedor?.Localidade.Estado.Sigla ?? cargaCTe.CTe.Destinatario.Localidade.Estado.Sigla
                    },
                    CT5 = new Dominio.ObjetosDeValor.EDI.CONEMB.DestinatarioConhecimentoCaterpillarExportacao()
                    {
                        IdentificadoRegistro = "CT5",
                        Nome = cargaCTe.CTe.Remetente.Nome,
                        CNPJ = cargaCTe.CTe.Remetente.CPF_CNPJ,
                        IE = cargaCTe.CTe.Remetente.IE_RG,
                        Endereco = cargaCTe.CTe.Remetente.Endereco,
                        Municipio = cargaCTe.CTe.Remetente.Localidade.Descricao,
                        UF = cargaCTe.CTe.Remetente.Localidade.Estado.Sigla
                    },
                    Notas = new List<Dominio.ObjetosDeValor.EDI.CONEMB.NotasConhecimentoCaterpillarExportacao>()
                };
                for (var n = 0; i < cargaCTe.CTe.Documentos.Count; n++)
                {
                    Dominio.Entidades.DocumentosCTE documento = cargaCTe.CTe.Documentos[n];
                    int.TryParse(documento.NumeroOuNumeroDaChave, out int numeroNota);
                    int.TryParse(documento.SerieOuSerieDaChave, out int serieNota);
                    Dominio.ObjetosDeValor.EDI.CONEMB.NotasConhecimentoCaterpillarExportacao nota = new Dominio.ObjetosDeValor.EDI.CONEMB.NotasConhecimentoCaterpillarExportacao()
                    {
                        IdentificadoRegistro = "CT3",
                        Numero = numeroNota,
                        Serie = serieNota,
                        Data = documento.DataEmissao,
                        Valor = documento.Valor,
                        NumeroEmbarque = embarque,
                        NumeroExportacao = numeroDI,
                        CodigoFabricaDestino = "",
                        TipoMercadoria = "P"
                    };

                    conhecimento.Notas.Add(nota);
                }
                conemb.Conhecimentos.Add(conhecimento);
            }

            conemb.FTP.IdentificadoRegistro = "FTP";
            conemb.FTP.NumeroControleTransmissao = 22003;
            conemb.FTP.QuantidadeRegistros = qtdRegistro;
            conemb.FTP.TotalValores = totalValores;
            conemb.FTP.CategoriaOperacao = "";

            return conemb;
        }

        public Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarExportacao ConverterCargaCTeParaCONEMB_CaterpillarExportacao(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(unidadeTrabalho);
            Repositorio.ComponentePrestacaoCTE repComponentes = new Repositorio.ComponentePrestacaoCTE(unidadeTrabalho);

            Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarExportacao conemb = new Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarExportacao();

            conemb.IdentificadoRegistro = "ITP";
            conemb.IdentificadorProcesso = 15;
            conemb.NumeroVersaoTransacao = 3;
            conemb.NumeroControleTransmissao = 22003;
            conemb.IdentificadorGeracaoMovimento = DateTime.Now;
            conemb.IdentificadorTransmissor = ctes.FirstOrDefault().Empresa.CNPJ;
            conemb.IdentificadorReceptor = "61064911000177";
            conemb.CodigoInternoTransmissor = "Q5875F1";
            conemb.CodigoInternoReceptor = "GE";
            conemb.NomeTransmissor = ctes.FirstOrDefault().Empresa.RazaoSocial;
            conemb.NomeReceptor = "CATERPILLAR BRASIL LTDA";
            conemb.Conhecimentos = new List<Dominio.ObjetosDeValor.EDI.CONEMB.ConhecimentoCaterpillarExportacao>();
            conemb.FTP = new Dominio.ObjetosDeValor.EDI.CONEMB.RodapeCaterpillarExportacao();

            int qtdRegistro = 1;
            decimal totalValores = 0;

            for (var i = 0; i < ctes.Count; i++)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ctes[i];
                Dominio.Entidades.InformacaoCargaCTE informacaoCarga = repInformacaoCarga.BuscarPorCTeUnidade(cte.Codigo, "01");
                Dominio.Entidades.InformacaoCargaCTE informacaoCargaVolume = repInformacaoCarga.BuscarPorCTeUnidade(cte.Codigo, "03");

                string embarque = cte.Embarque;
                string numeroDI = cte.NumeroDI;
                string masterBL = cte.MasterBL;
                if (string.IsNullOrWhiteSpace(embarque) && cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0)
                    embarque = cte.XMLNotaFiscais.Where(n => n.Embarque != null && n.Embarque != "").Select(c => c.Embarque)?.FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(numeroDI) && cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0)
                    numeroDI = cte.XMLNotaFiscais.Where(n => n.NumeroDI != null && n.NumeroDI != "").Select(c => c.NumeroDI)?.FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(masterBL) && cte.XMLNotaFiscais != null && cte.XMLNotaFiscais.Count > 0)
                    masterBL = cte.XMLNotaFiscais.Where(n => n.MasterBL != null && n.MasterBL != "").Select(c => c.MasterBL)?.FirstOrDefault() ?? "";

                totalValores += cte.ValorAReceber;

                Dominio.ObjetosDeValor.EDI.CONEMB.ConhecimentoCaterpillarExportacao conhecimento = new Dominio.ObjetosDeValor.EDI.CONEMB.ConhecimentoCaterpillarExportacao()
                {
                    IdentificadoRegistro = "CT1",
                    Numero = cte.Numero,
                    Serie = cte.Serie.Numero,
                    DataEmissao = cte.DataEmissao.Value,
                    QuantidadeNotas = cte.Documentos.Count(),
                    ValorNotas = cte.ValorTotalMercadoria,
                    Valor = cte.ValorAReceber,
                    CodigoFiscal = cte.CFOP.CodigoCFOP,
                    ModalidadeFrete = cte.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar ? "C" : "F",
                    SituacaoTributaria = cte.CST == "60" ? "1" : "2",
                    Peso = informacaoCarga?.Quantidade ?? 0,
                    Volume = informacaoCargaVolume?.Quantidade ?? 0,
                    BaseICMS = cte.BaseCalculoICMS,
                    AliquotaICMS = cte.AliquotaICMS,
                    ValorICMS = cte.ValorICMS,
                    IdentificacaoLocalEntrega = cte.LocalidadeTerminoPrestacao.Descricao,
                    LocalColeta = cte.LocalidadeInicioPrestacao.Descricao,
                    UnidadePeso = "KG",
                    UnidadeVolume = "UN",
                    CT2 = new Dominio.ObjetosDeValor.EDI.CONEMB.ComplementoConhecimentoCaterpillarExportacao()
                    {
                        IdentificadoRegistro = "CT2",
                        ValorSEC = cte.ComponentesPrestacao.Select(c => c.ValorSECCAT)?.Sum() ?? 0,
                        ValorITR = cte.ComponentesPrestacao.Select(c => c.ValorITR)?.Sum() ?? 0,
                        ValorDespacho = cte.ComponentesPrestacao.Select(c => c.ValorDespacho)?.Sum() ?? 0,
                        ValorPedagio = cte.ComponentesPrestacao.Select(c => c.ValorPedagio)?.Sum() ?? 0,
                        ValorAdeme = 0,
                        ValorADValorem = cte.ComponentesPrestacao.Select(c => c.ValorAdValorem)?.Sum() ?? 0,
                        FretePeso = 0,
                        ValorSUFRAMA = 0,
                        OutrosValores = cte.ComponentesPrestacao.Select(c => c.ValorAdicionais)?.Sum() ?? 0,
                        ValorIRRF = cte.ComponentesPrestacao.Select(c => c.ValorIRRF)?.Sum() ?? 0
                    },
                    CT4 = new Dominio.ObjetosDeValor.EDI.CONEMB.DestinatarioConhecimentoCaterpillarExportacao()
                    {
                        IdentificadoRegistro = "CT4",
                        Nome = cte.Recebedor?.Nome ?? cte.Destinatario.Nome,
                        CNPJ = cte.Recebedor?.CPF_CNPJ ?? cte.Destinatario.CPF_CNPJ,
                        IE = cte.Recebedor?.IE_RG ?? cte.Destinatario.IE_RG,
                        Endereco = cte.Recebedor?.Endereco ?? cte.Destinatario.Endereco,
                        Municipio = cte.Recebedor?.Localidade.Descricao ?? cte.Destinatario.Localidade.Descricao,
                        UF = cte.Recebedor?.Localidade.Estado.Sigla ?? cte.Destinatario.Localidade.Estado.Sigla
                    },
                    CT5 = new Dominio.ObjetosDeValor.EDI.CONEMB.DestinatarioConhecimentoCaterpillarExportacao()
                    {
                        IdentificadoRegistro = "CT5",
                        Nome = cte.Remetente.Nome,
                        CNPJ = cte.Remetente.CPF_CNPJ,
                        IE = cte.Remetente.IE_RG,
                        Endereco = cte.Remetente.Endereco,
                        Municipio = cte.Remetente.Localidade.Descricao,
                        UF = cte.Remetente.Localidade.Estado.Sigla
                    },
                    Notas = new List<Dominio.ObjetosDeValor.EDI.CONEMB.NotasConhecimentoCaterpillarExportacao>()

                };

                qtdRegistro += 4;

                for (var n = 0; n < cte.Documentos.Count; n++)
                {
                    qtdRegistro++;
                    Dominio.Entidades.DocumentosCTE documento = cte.Documentos[n];
                    int.TryParse(documento.NumeroOuNumeroDaChave, out int numeroNota);
                    int.TryParse(documento.SerieOuSerieDaChave, out int serieNota);
                    Dominio.ObjetosDeValor.EDI.CONEMB.NotasConhecimentoCaterpillarExportacao nota = new Dominio.ObjetosDeValor.EDI.CONEMB.NotasConhecimentoCaterpillarExportacao()
                    {
                        IdentificadoRegistro = "CT3",
                        Numero = numeroNota,
                        Serie = serieNota,
                        Data = documento.DataEmissao,
                        Valor = documento.Valor,
                        NumeroEmbarque = embarque,
                        NumeroExportacao = numeroDI,
                        CodigoFabricaDestino = "",
                        TipoMercadoria = "P"
                    };

                    conhecimento.Notas.Add(nota);
                }
                conemb.Conhecimentos.Add(conhecimento);
            }

            qtdRegistro++;
            conemb.FTP.IdentificadoRegistro = "FTP";
            conemb.FTP.NumeroControleTransmissao = 22003;
            conemb.FTP.QuantidadeRegistros = qtdRegistro;
            conemb.FTP.TotalValores = totalValores;
            conemb.FTP.CategoriaOperacao = "";

            return conemb;
        }

    }
}
