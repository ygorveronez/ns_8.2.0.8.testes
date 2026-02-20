using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.CTe
{
    public class ConsultaReceita : ServicoBase
    {        
        public ConsultaReceita(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<string> VerificarMensagemErro(Stream html)
        {
            List<string> Erros = new List<string>();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(html);

            HtmlNodeCollection erros = doc.DocumentNode.SelectNodes("//*[@id='ContentPlaceHolder1_vdsErros']//li");
            if (erros != null)
            {
                for (int i = 0; i < erros.Count; i++)
                {
                    if (erros[i] != null)
                        Erros.Add(erros[i].InnerText.Trim().Replace("\r\n", ""));
                }
            }
            HtmlNodeCollection msgErros = doc.DocumentNode.SelectNodes("//*[@id='ContentPlaceHolder1_bltMensagensErro']//li");
            if (msgErros != null)
            {
                for (int i = 0; i < msgErros.Count; i++)
                {
                    if (msgErros[i] != null)
                        Erros.Add(msgErros[i].InnerText.Trim().Replace("\r\n", ""));
                }
            }
            html.Dispose();
            html.Close();
            return Erros;
        }


        public Dominio.ObjetosDeValor.Embarcador.CTe.CTe ProcessarHTMLRetorno(Stream html)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(html);

            bool encontrou = PreecherDadosCTe(ref cte, doc);
            if (encontrou)
            {
                PreecherEmitente(ref cte, doc);
                PreencherParticipantes(ref cte, doc);
                PreencherTotais(ref cte, doc, cultura);
                PreencherCarga(ref cte, doc, cultura);
                PreencherInformacoesAdicionais(ref cte, doc);
                PreecherModais(ref cte, doc);
                html.Dispose();
                html.Close();
                return cte;
            }
            else
            {
                html.Dispose();
                html.Close();
                return null;
            }
        }

        private void PreecherModais(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, HtmlAgilityPack.HtmlDocument doc)
        {
            if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario)
            {
                PreecherModalRodoviario(ref cte, doc);
            }
            else if (cte.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aereo)
            {
                PreecherModalAereo(ref cte, doc);
            }
        }

        private void PreecherModalAereo(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, HtmlAgilityPack.HtmlDocument doc)
        {
            HtmlNode divModalAereo = doc.DocumentNode.SelectSingleNode("//*[@id='aereo']");
            if (divModalAereo != null)
            {
                cte.ModalAereo = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereo();
                HtmlNodeCollection htmlAereo = divModalAereo.SelectNodes("div");
                if (htmlAereo != null)
                {
                    for (int i = 0; i < htmlAereo.Count; i++)
                    {
                        HtmlNode htmlTitulo = htmlAereo[i];
                        string titulo = Utilidades.String.RemoveDiacritics(htmlTitulo.InnerText.Replace("\r\n", "").Trim().ToLower());

                        if (titulo.Contains("aereo"))
                        {
                            HtmlNode divNumeroMinuta = divModalAereo.SelectSingleNode("table[" + (i + 1).ToString() + "]/tr[2]/td[1]");
                            if (divNumeroMinuta != null)
                            {
                                if (long.TryParse(divNumeroMinuta.InnerText.Trim().Replace("\r\n", ""), out long numeroMinuta))
                                    cte.ModalAereo.NumeroMinuta = numeroMinuta;
                            }

                            HtmlNode divNumeroOCA = divModalAereo.SelectSingleNode("table[" + (i + 1).ToString() + "]/tr[2]/td[2]");
                            if (divNumeroOCA != null)
                            {
                                if (long.TryParse(divNumeroOCA.InnerText.Trim().Replace("\r\n", ""), out long numeroOCA))
                                    cte.ModalAereo.NumeroOperacionalConhecimentoAereo = numeroOCA;
                            }

                            HtmlNode divDataPrevistaEntrega = divModalAereo.SelectSingleNode("table[" + (i + 1).ToString() + "]/tr[2]/td[3]");
                            if (divDataPrevistaEntrega != null)
                            {
                                if (DateTime.TryParseExact(divDataPrevistaEntrega.InnerText.Trim().Replace("\r\n", ""), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevistaEntrega))
                                    cte.ModalAereo.DataPrevistaEntrega = dataPrevistaEntrega;
                            }
                        }
                    }
                }
            }
        }

        private void PreecherModalRodoviario(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, HtmlAgilityPack.HtmlDocument doc)
        {
            HtmlNode DivModalRodoviario = doc.DocumentNode.SelectSingleNode("//*[@id='rodoviario']");
            if (DivModalRodoviario != null)
            {
                cte.ModalRodoviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario();
                HtmlNodeCollection htmlRodoviario = DivModalRodoviario.SelectNodes("div");
                if (htmlRodoviario != null)
                {
                    for (int i = 0; i < htmlRodoviario.Count; i++)
                    {
                        HtmlNode htmlTitulo = htmlRodoviario[i];
                        string titulo = Utilidades.String.RemoveDiacritics(htmlTitulo.InnerText.Replace("\r\n", "").Trim().ToLower());

                        if (titulo.Contains("rodoviario"))
                        {
                            HtmlNode divRNTRC = DivModalRodoviario.SelectSingleNode("table[" + (i + 1).ToString() + "]/tr[2]/td[1]");
                            if (divRNTRC != null)
                            {
                                cte.ModalRodoviario.RNTRC = divRNTRC.InnerText.Trim().Replace("\r\n", "");
                                if (cte.Emitente != null)
                                    cte.Emitente.RNTRC = cte.ModalRodoviario.RNTRC;
                            }

                            HtmlNode HtmlDataEntrega = DivModalRodoviario.SelectSingleNode("table[" + (i + 1).ToString() + "]/tr[2]/td[2]");
                            if (HtmlDataEntrega != null)
                                cte.ModalRodoviario.DataEntrega = HtmlDataEntrega.InnerText.Trim().Replace("\r\n", "");

                            HtmlNode HtmlLotacao = DivModalRodoviario.SelectSingleNode("table[" + (i + 1).ToString() + "]/tr[2]/td[3]");
                            if (HtmlLotacao != null)
                            {
                                int lotacao = int.Parse(HtmlLotacao.InnerText.Trim().Replace("\r\n", ""));
                                cte.ModalRodoviario.Lotacao = lotacao == 1 ? true : false;
                            }

                            HtmlNode HtmlCIOT = DivModalRodoviario.SelectSingleNode("table[" + (i + 1).ToString() + "]/tr[2]/td[4]");
                            if (HtmlCIOT != null)
                                cte.ModalRodoviario.CIOT = HtmlCIOT.InnerText.Trim().Replace("\r\n", "");

                        }
                        else if (titulo.Contains("veiculo"))
                        {
                            HtmlNodeCollection htmlVeiculos = DivModalRodoviario.SelectNodes("table[" + (i + 1).ToString() + "]/tr");
                            if (htmlVeiculos != null)
                            {
                                cte.ModalRodoviario.Veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();

                                for (int j = 1; j < htmlVeiculos.Count; j++)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculo = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();

                                    HtmlNode htmlRenavam = htmlVeiculos[j].SelectSingleNode("td[1]");
                                    if (htmlRenavam != null)
                                        veiculo.Renavam = htmlRenavam.InnerText.Trim().Replace("\r\n", "");

                                    HtmlNode htmlPlacal = htmlVeiculos[j].SelectSingleNode("td[2]");
                                    if (htmlPlacal != null)
                                        veiculo.Placa = htmlPlacal.InnerText.Trim().Replace("\r\n", "");

                                    HtmlNode htmlTara = htmlVeiculos[j].SelectSingleNode("td[3]");
                                    if (htmlTara != null)
                                    {
                                        string tara = htmlTara.InnerText.Trim().Replace("\r\n", "");
                                        veiculo.Tara = !string.IsNullOrWhiteSpace(tara) ? int.Parse(tara) : 0;
                                    }

                                    HtmlNode htmlUF = htmlVeiculos[j].SelectSingleNode("td[4]");
                                    if (htmlUF != null)
                                        veiculo.UF = htmlUF.InnerText.Trim().Replace("\r\n", "");

                                    cte.ModalRodoviario.Veiculos.Add(veiculo);
                                }
                            }
                        }
                        else if (titulo.Contains("motorista"))
                        {
                            HtmlNodeCollection htmlMotorista = DivModalRodoviario.SelectNodes("table[" + (i + 1).ToString() + "]/tr");
                            if (htmlMotorista != null)
                            {
                                cte.ModalRodoviario.Motoristas = new List<Dominio.ObjetosDeValor.Motorista>();

                                for (int j = 1; j < htmlMotorista.Count; j++)
                                {
                                    Dominio.ObjetosDeValor.Motorista motorista = new Dominio.ObjetosDeValor.Motorista();
                                    motorista.Ativo = true;

                                    HtmlNode htmlCFP = htmlMotorista[j].SelectSingleNode("td[1]");
                                    if (htmlCFP != null)
                                        motorista.CPF = htmlCFP.InnerText.Trim().Replace("\r\n", "");

                                    HtmlNode htmlNome = htmlMotorista[j].SelectSingleNode("td[2]");
                                    if (htmlNome != null)
                                        motorista.Nome = htmlNome.InnerText.Trim().Replace("\r\n", "");

                                    cte.ModalRodoviario.Motoristas.Add(motorista);
                                }
                            }
                        }
                    }
                }
            }
        }


        private void PreencherCTeComplmentado(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, HtmlAgilityPack.HtmlDocument doc)
        {
            HtmlNode divCTeComplementado = doc.DocumentNode.SelectSingleNode("//*[@id='complementar']/table/tr[2]/td");
            if (divCTeComplementado != null)
                cte.ChaveCTeComplementado = divCTeComplementado.InnerText.Trim().Replace("\r\n", "").Replace(".", "");

        }
        private void PreencherInformacoesAdicionais(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, HtmlAgilityPack.HtmlDocument doc)
        {
            HtmlNode DivInformacoesAdicionais = doc.DocumentNode.SelectSingleNode("//*[@id='informacoes_adicionais']");
            if (DivInformacoesAdicionais != null)
            {
                HtmlNodeCollection htmlInformacoesAdicionais = DivInformacoesAdicionais.SelectNodes("div");
                if (htmlInformacoesAdicionais != null)
                {
                    for (int i = 0; i < htmlInformacoesAdicionais.Count; i++)
                    {
                        HtmlNode htmlTitulo = htmlInformacoesAdicionais[i];
                        string titulo = Utilidades.String.RemoveDiacritics(htmlTitulo.InnerText.Replace("\r\n", "").Trim().ToLower());

                        if (titulo.Contains("documentos anteriores"))
                        {
                            HtmlNodeCollection htmlDocAnterior = DivInformacoesAdicionais.SelectNodes("table[" + i.ToString() + "]/tr");
                            if (htmlDocAnterior != null)
                            {
                                cte.DocumentosAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior>();
                                for (int j = 1; j < htmlDocAnterior.Count; j++)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior docAnterior = new Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior();
                                    docAnterior.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

                                    HtmlNode htmlCNPJ = htmlDocAnterior[j].SelectSingleNode("td[1]");
                                    if (htmlCNPJ != null)
                                        docAnterior.Emitente.CPFCNPJ = htmlCNPJ.InnerText.Trim().Replace("\r\n", "");

                                    HtmlNode htmlNome = htmlDocAnterior[j].SelectSingleNode("td[2]");
                                    if (htmlNome != null)
                                        docAnterior.Emitente.RazaoSocial = htmlNome.InnerText.Trim().Replace("\r\n", "");

                                    HtmlNode htmlChaveAcesso = htmlDocAnterior[j].SelectSingleNode("td[3]");
                                    if (htmlChaveAcesso != null)
                                        docAnterior.ChaveAcesso = htmlChaveAcesso.InnerText.Trim().Replace("\r\n", "");

                                    cte.DocumentosAnteriores.Add(docAnterior);
                                }
                            }
                        }
                        else if (titulo.Contains("fisco"))
                        {
                            HtmlNode divObservacaoFisco = DivInformacoesAdicionais.SelectSingleNode("table[" + i.ToString() + "]/tr[2]/td");
                            if (divObservacaoFisco != null)
                                cte.InformacaoAdicionalFisco = divObservacaoFisco.InnerText.Trim().Replace("\r\n", "");
                        }
                        else if (titulo.Contains("contribuinte"))
                        {
                            HtmlNode divObservacaoContribuinte = DivInformacoesAdicionais.SelectSingleNode("table[" + i.ToString() + "]/tr[2]/td");
                            if (divObservacaoContribuinte != null)
                                cte.InformacaoAdicionalContribuinte = divObservacaoContribuinte.InnerText.Trim().Replace("\r\n", "");
                        }
                    }
                }

            }
        }

        private void PreencherCarga(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, HtmlAgilityPack.HtmlDocument doc, System.Globalization.CultureInfo cultura)
        {
            HtmlNode DivCarga = doc.DocumentNode.SelectSingleNode("//*[@id='carga']");

            if (DivCarga != null)
            {
                cte.InformacaoCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.InformacaoCarga();
                HtmlNode htmlValorTotalCarga = DivCarga.SelectSingleNode("table[1]/tr[2]/td[1]");
                if (htmlValorTotalCarga != null)
                {
                    string valorTotalCarga = htmlValorTotalCarga.InnerText.Trim().Replace("\r\n", "");
                    cte.InformacaoCarga.ValorTotalCarga = !string.IsNullOrEmpty(valorTotalCarga) ? decimal.Parse(valorTotalCarga, cultura) : 0m;
                }

                HtmlNode htmlProdutoPredominante = DivCarga.SelectSingleNode("table[1]/tr[2]/td[2]");
                if (htmlProdutoPredominante != null)
                    cte.InformacaoCarga.ProdutoPredominante = htmlProdutoPredominante.InnerText.Trim().Replace("\r\n", "");

                HtmlNode htmlOutrasCaracteristicas = DivCarga.SelectSingleNode("table[1]/tr[2]/td[3]");
                if (htmlOutrasCaracteristicas != null)
                    cte.InformacaoCarga.OutrasCaracteristicas = htmlOutrasCaracteristicas.InnerText.Trim().Replace("\r\n", "");

                HtmlNodeCollection htmlQuantidades = DivCarga.SelectNodes("table[2]/tr");
                if (htmlQuantidades != null)
                {
                    cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();
                    for (int i = 1; i < htmlQuantidades.Count; i++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();

                        HtmlNode htmlUnidade = htmlQuantidades[i].SelectSingleNode("td[1]");
                        if (htmlUnidade != null)
                        {
                            string[] sUnidade = htmlUnidade.InnerText.Trim().Replace("\r\n", "").Split('-');
                            int unidade = int.Parse(sUnidade[0]);
                            quantidadeCarga.Unidade = (Dominio.Enumeradores.UnidadeMedida)unidade;
                        }
                        HtmlNode htmlMedida = htmlQuantidades[i].SelectSingleNode("td[2]");
                        if (htmlMedida != null)
                            quantidadeCarga.Medida = htmlMedida.InnerText.Trim().Replace("\r\n", "");

                        HtmlNode htmlQuantidade = htmlQuantidades[i].SelectSingleNode("td[3]");
                        if (htmlQuantidade != null)
                        {
                            string quantidade = htmlQuantidade.InnerText.Trim().Replace("\r\n", "");
                            quantidadeCarga.Quantidade = !string.IsNullOrEmpty(quantidade) ? decimal.Parse(quantidade, cultura) : 0m;
                        }
                        cte.QuantidadesCarga.Add(quantidadeCarga);
                    }
                }

                HtmlNodeCollection htmlNotasFiscais = doc.DocumentNode.SelectNodes("//*[@id='tbNF']/tr");
                if (htmlNotasFiscais != null)
                {
                    cte.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal>();
                    for (int i = 1; i < htmlNotasFiscais.Count; i++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.CTe.NotaFiscal();

                        notaFiscal.ModeloNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloNotaFiscal.NFModelo01Avulsa;

                        HtmlNode htmlNumero = htmlNotasFiscais[i].SelectSingleNode("td[1]/a");
                        if (htmlNumero != null)
                            notaFiscal.Numero = htmlNumero.InnerText.Trim().Replace("\r\n", "");


                        HtmlNode htmlSerie = htmlNotasFiscais[i].SelectSingleNode("td[2]");
                        if (htmlSerie != null)
                            notaFiscal.Serie = htmlSerie.InnerText.Trim().Replace("\r\n", "");

                        HtmlNode htmlDataEmissao = htmlNotasFiscais[i].SelectSingleNode("td[3]");
                        if (htmlDataEmissao != null)
                        {
                            DateTime dataEmissao;
                            DateTime.TryParseExact(htmlDataEmissao.InnerText.Trim().Replace("\r\n", ""), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                            if (dataEmissao != DateTime.MinValue)
                                notaFiscal.DataEmissao = dataEmissao;
                            else
                                notaFiscal.DataEmissao = DateTime.Now;
                        }

                        HtmlNode htmlValor = htmlNotasFiscais[i].SelectSingleNode("td[4]");
                        if (htmlValor != null)
                        {
                            string valor = htmlValor.InnerText.Trim().Replace("\r\n", "");
                            notaFiscal.Valor = !string.IsNullOrEmpty(valor) ? decimal.Parse(valor, cultura) : 0m;
                        }

                        cte.NotasFiscais.Add(notaFiscal);
                    }
                }

                HtmlNodeCollection htmlOutrosDocumentos = doc.DocumentNode.SelectNodes("//*[@id='tbNFo']/tr");
                if (htmlOutrosDocumentos != null)
                {
                    cte.OutrosDocumentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento>();
                    for (int i = 1; i < htmlOutrosDocumentos.Count; i++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento outroDocumento = new Dominio.ObjetosDeValor.Embarcador.CTe.OutroDocumento();

                        HtmlNode htmlTipo = htmlOutrosDocumentos[i].SelectSingleNode("td[1]/a");
                        if (htmlTipo != null)
                        {
                            int tipo = int.Parse(htmlTipo.InnerText.Trim().Replace("\r\n", ""));
                            outroDocumento.Tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOutroDocumento)tipo;
                        }


                        HtmlNode htmlDescricao = htmlOutrosDocumentos[i].SelectSingleNode("td[2]");
                        if (htmlDescricao != null)
                            outroDocumento.Descricao = htmlDescricao.InnerText.Trim().Replace("\r\n", "");

                        HtmlNode htmlNumero = htmlOutrosDocumentos[i].SelectSingleNode("td[3]");
                        if (htmlNumero != null)
                            outroDocumento.Numero = htmlNumero.InnerText.Trim().Replace("\r\n", "");

                        HtmlNode htmlValor = htmlOutrosDocumentos[i].SelectSingleNode("td[4]");
                        if (htmlValor != null)
                        {
                            string valor = htmlValor.InnerText.Trim().Replace("\r\n", "");
                            outroDocumento.Valor = !string.IsNullOrEmpty(valor) ? decimal.Parse(valor, cultura) : 0m;
                        }

                        cte.OutrosDocumentos.Add(outroDocumento);
                    }
                }

                HtmlNodeCollection htmlChaves = doc.DocumentNode.SelectNodes("//*[@id='tbNFe']/tr");
                if (htmlChaves != null)
                {
                    cte.NFEs = new List<Dominio.ObjetosDeValor.Embarcador.CTe.NFe>();
                    for (int i = 1; i < htmlChaves.Count; i++)
                    {
                        HtmlNode htmlChave = htmlChaves[i].SelectSingleNode("td/a");
                        if (htmlChave != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.CTe.NFe NFe = new Dominio.ObjetosDeValor.Embarcador.CTe.NFe();
                            NFe.Chave = htmlChave.InnerText.Trim().Replace("\r\n", "");

                            if (int.TryParse(NFe.Chave.Substring(25, 9), out int numeroNFe))
                                NFe.Numero = numeroNFe;

                            cte.NFEs.Add(NFe);
                        }
                    }
                }
                HtmlNodeCollection htmlSeguros = doc.DocumentNode.SelectNodes("//*[@id='trSeg']/tr");
                if (htmlSeguros != null)
                {
                    cte.Seguros = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();
                    for (int i = 1; i < htmlSeguros.Count; i++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguro = new Dominio.ObjetosDeValor.Embarcador.CTe.Seguro();

                        HtmlNode htmlResponsavel = htmlSeguros[i].SelectSingleNode("td[1]");
                        if (htmlResponsavel != null)
                        {
                            string[] sResponsavel = htmlResponsavel.InnerText.Trim().Replace("\r\n", "").Split('-');
                            int responsavel = int.Parse(sResponsavel[0]);
                            seguro.ResponsavelSeguro = (Dominio.Enumeradores.TipoSeguro)responsavel;
                        }

                        HtmlNode htmlSeguradora = htmlSeguros[i].SelectSingleNode("td[2]");
                        if (htmlSeguradora != null)
                            seguro.Seguradora = htmlSeguradora.InnerText.Trim().Replace("\r\n", "");

                        HtmlNode htmlApolice = htmlSeguros[i].SelectSingleNode("td[3]");
                        if (htmlApolice != null)
                            seguro.Apolice = htmlApolice.InnerText.Trim().Replace("\r\n", "");

                        HtmlNode htmlAverbacao = htmlSeguros[i].SelectSingleNode("td[4]");
                        if (htmlAverbacao != null)
                            seguro.Averbacao = htmlAverbacao.InnerText.Trim().Replace("\r\n", "");

                        HtmlNode htmlValor = htmlSeguros[i].SelectSingleNode("td[5]");
                        if (htmlValor != null)
                        {
                            string valor = htmlValor.InnerText.Trim().Replace("\r\n", "");
                            seguro.Valor = !string.IsNullOrEmpty(valor) ? decimal.Parse(valor, cultura) : 0m;
                        }
                        cte.Seguros.Add(seguro);
                    }
                }
            }
        }

        private void PreencherTotais(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, HtmlAgilityPack.HtmlDocument doc, System.Globalization.CultureInfo cultura)
        {
            HtmlNode DivTotais = doc.DocumentNode.SelectSingleNode("//*[@id='totais']");
            if (DivTotais != null)
            {
                cte.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();
                cte.ValorFrete.ICMS = new Dominio.ObjetosDeValor.Embarcador.ICMS.ICMS();
                cte.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();

                HtmlNodeCollection htmlTotais = DivTotais.SelectNodes("div");
                if (htmlTotais != null)
                {
                    for (int i = 0; i < htmlTotais.Count; i++)
                    {
                        HtmlNode htmlTitulo = htmlTotais[i];
                        string titulo = Utilidades.String.RemoveDiacritics(htmlTitulo.InnerText.Replace("\r\n", "").Trim().ToLower());

                        if (titulo.Contains("valores"))
                        {
                            HtmlNode divValorPrestacao = DivTotais.SelectSingleNode("table[" + i.ToString() + "]/tr[2]/td[1]");
                            if (divValorPrestacao != null)
                            {
                                string valorPrestacao = divValorPrestacao.InnerText.Trim().Replace("\r\n", "");
                                cte.ValorFrete.ValorPrestacaoServico = !string.IsNullOrEmpty(valorPrestacao) ? decimal.Parse(valorPrestacao, cultura) : 0m;
                            }

                            HtmlNode divValorAReceber = DivTotais.SelectSingleNode("table[" + i.ToString() + "]/tr[2]/td[2]");
                            if (divValorAReceber != null)
                            {
                                string valorAReceber = divValorAReceber.InnerText.Trim().Replace("\r\n", "");
                                cte.ValorFrete.ValorTotalAReceber = !string.IsNullOrEmpty(valorAReceber) ? decimal.Parse(valorAReceber, cultura) : 0m;
                            }
                        }
                        else if (titulo.Contains("componentes"))
                        {
                            HtmlNodeCollection htmlComponentes = DivTotais.SelectNodes("table[" + i.ToString() + "]/tr");

                            if (htmlComponentes != null)
                            {
                                cte.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
                                for (int j = 1; j < htmlComponentes.Count; j++)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                                    componente.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                                    HtmlNode htmlComponente = htmlComponentes[j].SelectSingleNode("td[1]");
                                    if (htmlComponente != null)
                                        componente.Componente.Descricao = htmlComponente.InnerText.Trim().Replace("\r\n", "");

                                    HtmlNode htmlValorComponente = htmlComponentes[j].SelectSingleNode("td[2]");
                                    if (htmlValorComponente != null)
                                    {
                                        string valorValorComponente = htmlValorComponente.InnerText.Trim().Replace("\r\n", "");
                                        componente.ValorComponente = !string.IsNullOrEmpty(valorValorComponente) ? decimal.Parse(valorValorComponente, cultura) : 0m;
                                    }
                                    cte.ValorFrete.ComponentesAdicionais.Add(componente);
                                }
                            }
                        }
                        else if (titulo.Contains("impostos"))
                        {
                            HtmlNode htmlImpostos = DivTotais.SelectSingleNode("table[" + i.ToString() + "]/tr[1]/th[1]");
                            if (htmlImpostos != null)
                            {
                                string tituloImposto = Utilidades.String.RemoveDiacritics(htmlImpostos.InnerText.Replace("\r\n", "").Trim().ToLower());
                                if (tituloImposto.Contains("simples"))
                                {
                                    if (cte.Emitente != null)
                                        cte.Emitente.SimplesNacional = true;
                                    HtmlNode htmlValorTotalTributosSimples = DivTotais.SelectSingleNode("table[" + i.ToString() + "]/tr[3]/td");

                                    if (htmlValorTotalTributosSimples != null)
                                    {
                                        string valorTotalTributo = htmlValorTotalTributosSimples.InnerText.Trim().Replace("\r\n", "");
                                        cte.ValorFrete.ICMS.ValorTotalTributos = !string.IsNullOrEmpty(valorTotalTributo) ? decimal.Parse(valorTotalTributo, cultura) : 0m;
                                    }
                                }
                                else
                                {
                                    HtmlNode tableImpostos = DivTotais.SelectSingleNode("table[" + i.ToString() + "]");
                                    if (tableImpostos != null)
                                    {
                                        HtmlNode htmlCST = tableImpostos.SelectSingleNode("tr[2]/td[1]");
                                        if (htmlCST != null)
                                            cte.ValorFrete.ICMS.CST = htmlCST.InnerText.Trim().Replace("\r\n", "");

                                        if (cte.ValorFrete.ICMS.CST != "40" && cte.ValorFrete.ICMS.CST != "41" && cte.ValorFrete.ICMS.CST != "51")
                                        {
                                            HtmlNode htmlPercentualRecucao = tableImpostos.SelectSingleNode("tr[2]/td[2]");
                                            if (htmlPercentualRecucao != null)
                                            {
                                                string percentualReducaoBC = htmlPercentualRecucao.InnerText.Trim().Replace("\r\n", "").Replace("%", "");
                                                cte.ValorFrete.ICMS.PercentualReducaoBC = !string.IsNullOrEmpty(percentualReducaoBC) ? decimal.Parse(percentualReducaoBC, cultura) : 0m;
                                            }

                                            HtmlNode htmlValorCredito = tableImpostos.SelectSingleNode("tr[2]/td[3]");
                                            if (htmlValorCredito != null)
                                            {
                                                string valorCredito = htmlValorCredito.InnerText.Trim().Replace("\r\n", "").Replace("%", "");
                                                cte.ValorFrete.ICMS.ValorCreditoPresumido = !string.IsNullOrEmpty(valorCredito) ? decimal.Parse(valorCredito, cultura) : 0m;
                                            }

                                            HtmlNode htmlBaseCaluloICMS = tableImpostos.SelectSingleNode("tr[4]/td[1]");
                                            if (htmlBaseCaluloICMS != null)
                                            {
                                                string baseCaluloICMS = htmlBaseCaluloICMS.InnerText.Trim().Replace("\r\n", "");
                                                cte.ValorFrete.ICMS.ValorBaseCalculoICMS = !string.IsNullOrEmpty(baseCaluloICMS) ? decimal.Parse(baseCaluloICMS, cultura) : 0m;
                                            }

                                            HtmlNode htmlAliquota = tableImpostos.SelectSingleNode("tr[4]/td[2]");
                                            if (htmlAliquota != null)
                                            {
                                                string aliquota = htmlAliquota.InnerText.Trim().Replace("\r\n", "").Replace("%", "");
                                                cte.ValorFrete.ICMS.Aliquota = !string.IsNullOrEmpty(aliquota) ? decimal.Parse(aliquota, cultura) : 0m;
                                            }

                                            HtmlNode htmlValorICMS = tableImpostos.SelectSingleNode("tr[4]/td[3]");
                                            if (htmlValorICMS != null)
                                            {
                                                string valorICMS = htmlValorICMS.InnerText.Trim().Replace("\r\n", "");
                                                cte.ValorFrete.ICMS.ValorICMS = !string.IsNullOrEmpty(valorICMS) ? decimal.Parse(valorICMS, cultura) : 0m;
                                            }
                                            HtmlNode htmlValorTotalTributos = tableImpostos.SelectSingleNode("tr[6]/td[1]");
                                            if (htmlValorTotalTributos != null)
                                            {
                                                string valorTotalTributos = htmlValorTotalTributos.InnerText.Trim().Replace("\r\n", "");
                                                cte.ValorFrete.ICMS.ValorTotalTributos = !string.IsNullOrEmpty(valorTotalTributos) ? decimal.Parse(valorTotalTributos, cultura) : 0m;
                                            }
                                        }
                                        else
                                        {
                                            HtmlNode htmlValorTotalTributos = tableImpostos.SelectSingleNode("tr[4]/td[1]");
                                            if (htmlValorTotalTributos != null)
                                            {
                                                string valorTotalTributos = htmlValorTotalTributos.InnerText.Trim().Replace("\r\n", "");
                                                cte.ValorFrete.ICMS.ValorTotalTributos = !string.IsNullOrEmpty(valorTotalTributos) ? decimal.Parse(valorTotalTributos, cultura) : 0m;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        private void PreecherEmitente(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, HtmlAgilityPack.HtmlDocument doc)
        {
            HtmlNode DivEmitente = doc.DocumentNode.SelectSingleNode("//*[@id='emitente']");
            if (DivEmitente != null)
            {
                cte.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                HtmlNode divRazaoSocial = DivEmitente.SelectSingleNode("table/tr[2]/td[1]");
                if (divRazaoSocial != null)
                    cte.Emitente.RazaoSocial = divRazaoSocial.InnerText.Trim().Replace("\r\n", "");

                HtmlNode divNomeFantasia = DivEmitente.SelectSingleNode("table/tr[2]/td[2]");
                if (divNomeFantasia != null)
                    cte.Emitente.NomeFantasia = divNomeFantasia.InnerText.Trim().Replace("\r\n", "");

                HtmlNode divCNPJ = DivEmitente.SelectSingleNode("table/tr[5]/td[1]");
                if (divCNPJ != null)
                    cte.Emitente.CNPJ = divCNPJ.InnerText.Trim().Replace("\r\n", "");

                HtmlNode divIE = DivEmitente.SelectSingleNode("table/tr[5]/td[2]");
                if (divIE != null)
                    cte.Emitente.IE = divIE.InnerText.Trim().Replace("\r\n", "");

                HtmlNode divEndereco = DivEmitente.SelectSingleNode("table/tr[8]/td[1]");
                if (divEndereco != null)
                {
                    cte.Emitente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                    PreecherEndereco(cte.Emitente.Endereco, divEndereco);

                    HtmlNode divBairro = DivEmitente.SelectSingleNode("table/tr[8]/td[2]");
                    if (divBairro != null)
                        cte.Emitente.Endereco.Bairro = divBairro.InnerText.Trim().Replace("\r\n", "");

                    HtmlNode divTelefone = DivEmitente.SelectSingleNode("table/tr[11]/td[1]");
                    if (divTelefone != null)
                        cte.Emitente.Endereco.Telefone = divTelefone.InnerText.Trim().Replace("\r\n", "").Replace(" ", "");

                    HtmlNode divCEP = DivEmitente.SelectSingleNode("table/tr[11]/td[2]");
                    if (divCEP != null)
                        cte.Emitente.Endereco.CEP = divCEP.InnerText.Trim().Replace("\r\n", "");

                    HtmlNode divLocalidade = DivEmitente.SelectSingleNode("table/tr[14]/td[1]");
                    if (divLocalidade != null)
                    {
                        cte.Emitente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();

                        string[] localidade = divLocalidade.InnerText.Trim().Replace("\r\n", "").Split('-');
                        if (localidade.Length > 1)
                        {
                            cte.Emitente.Endereco.Cidade.IBGE = int.Parse(localidade[0]);
                            cte.Emitente.Endereco.Cidade.Descricao = localidade[1].Trim();
                        }
                        else
                            cte.Emitente.Endereco.Cidade.Descricao = localidade[0].Trim();

                        HtmlNode divEstado = DivEmitente.SelectSingleNode("table/tr[14]/td[2]");
                        if (divEstado != null)
                            cte.Emitente.Endereco.Cidade.SiglaUF = divEstado.InnerText.Trim().Replace("\r\n", "");
                    }

                    HtmlNode divPais = DivEmitente.SelectSingleNode("table/tr[17]/td[1]");
                    if (divPais != null)
                    {
                        cte.Emitente.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais();
                        string[] pais = divPais.InnerText.Trim().Replace("\r\n", "").Split('-');
                        if (pais.Length > 1)
                        {
                            cte.Emitente.Endereco.Cidade.Pais.CodigoPais = !string.IsNullOrWhiteSpace(pais[0]) ? int.Parse(pais[0]) : 0;
                            cte.Emitente.Endereco.Cidade.Pais.NomePais = pais[1].Trim();
                        }
                        else
                            cte.Emitente.Endereco.Cidade.Pais.NomePais = pais[0].Trim();
                    }
                }
            }
        }

        private void PreencherParticipantes(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, HtmlAgilityPack.HtmlDocument doc)
        {
            HtmlNode DivTomador = doc.DocumentNode.SelectSingleNode("//*[@id='tomador']");
            if (DivTomador != null)
                cte.Tomador = PreecherParticipante(DivTomador);

            HtmlNode DivRementente = doc.DocumentNode.SelectSingleNode("//*[@id='remetente']");
            if (DivRementente != null)
                cte.Remetente = PreecherParticipante(DivRementente);

            HtmlNode DivDestinatario = doc.DocumentNode.SelectSingleNode("//*[@id='destinatario']");
            if (DivDestinatario != null)
                cte.Destinatario = PreecherParticipante(DivDestinatario);

            HtmlNode DivExpedidor = doc.DocumentNode.SelectSingleNode("//*[@id='expedidor']");
            if (DivExpedidor != null)
                cte.Expedidor = PreecherParticipante(DivExpedidor);

            HtmlNode DivRecebedor = doc.DocumentNode.SelectSingleNode("//*[@id='recebedor']");
            if (DivRecebedor != null)
                cte.Recebedor = PreecherParticipante(DivRecebedor);

            if (cte.Tomador != null)
            {
                if (cte.Remetente != null && cte.Remetente.CPFCNPJ == cte.Tomador.CPFCNPJ)
                {
                    cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                    cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                }
                else
                {
                    if (cte.Destinatario != null && cte.Destinatario.CPFCNPJ == cte.Tomador.CPFCNPJ)
                    {
                        cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                        cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                    }
                    else
                    {
                        if (cte.Expedidor != null && cte.Expedidor.CPFCNPJ == cte.Tomador.CPFCNPJ)
                        {
                            cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                            cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Expedidor;
                        }
                        else
                        {
                            if (cte.Recebedor != null && cte.Recebedor.CPFCNPJ == cte.Tomador.CPFCNPJ)
                            {
                                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Recebedor;
                            }
                            else
                            {
                                cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                                cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                            }
                        }
                    }
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa PreecherParticipante(HtmlNode DivParticipante)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa participante = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
            HtmlNode divNome = DivParticipante.SelectSingleNode("table/tr[2]/td[1]");
            if (divNome != null)
                participante.RazaoSocial = divNome.InnerText.Trim().Replace("\r\n", "");

            HtmlNode divNomeFantasia = DivParticipante.SelectSingleNode("table/tr[2]/td[2]");
            if (divNomeFantasia != null)
                participante.NomeFantasia = divNomeFantasia.InnerText.Trim().Replace("\r\n", "");

            HtmlNode divCPFCNPJ = DivParticipante.SelectSingleNode("table/tr[5]/td[1]");
            if (divCPFCNPJ != null)
                participante.CPFCNPJ = divCPFCNPJ.InnerText.Trim().Replace("\r\n", "");

            HtmlNode divIERG = DivParticipante.SelectSingleNode("table/tr[5]/td[2]");
            if (divIERG != null)
                participante.RGIE = divIERG.InnerText.Trim().Replace("\r\n", "");

            HtmlNode divEndereco = DivParticipante.SelectSingleNode("table/tr[8]/td[1]");
            if (divEndereco != null)
            {
                participante.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                PreecherEndereco(participante.Endereco, divEndereco);

                HtmlNode divBairro = DivParticipante.SelectSingleNode("table/tr[8]/td[2]");
                if (divBairro != null)
                    participante.Endereco.Bairro = divBairro.InnerText.Trim().Replace("\r\n", "");

                HtmlNode divTelefone = DivParticipante.SelectSingleNode("table/tr[11]/td[1]");
                if (divTelefone != null)
                    participante.Endereco.Telefone = divTelefone.InnerText.Trim().Replace("\r\n", "").Replace(" ", "");

                HtmlNode divCEP = DivParticipante.SelectSingleNode("table/tr[11]/td[2]");
                if (divCEP != null)
                    participante.Endereco.CEP = divCEP.InnerText.Trim().Replace("\r\n", "");

                HtmlNode divLocalidade = DivParticipante.SelectSingleNode("table/tr[14]/td[1]");
                if (divLocalidade != null)
                {
                    participante.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();

                    string[] localidade = divLocalidade.InnerText.Trim().Replace("\r\n", "").Split('-');
                    if (localidade.Length > 1)
                    {
                        participante.Endereco.Cidade.IBGE = int.Parse(localidade[0]);
                        participante.Endereco.Cidade.Descricao = localidade[1].Trim();
                    }
                    else
                        participante.Endereco.Cidade.Descricao = localidade[0].Trim();

                    HtmlNode divEstado = DivParticipante.SelectSingleNode("table/tr[14]/td[2]");
                    if (divEstado != null)
                        participante.Endereco.Cidade.SiglaUF = divEstado.InnerText.Trim().Replace("\r\n", "");
                }

                HtmlNode divPais = DivParticipante.SelectSingleNode("table/tr[17]/td[1]");
                if (divPais != null)
                {
                    participante.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais();
                    string[] pais = divPais.InnerText.Trim().Replace("\r\n", "").Split('-');
                    if (pais.Length > 1)
                    {
                        participante.Endereco.Cidade.Pais.CodigoPais = !string.IsNullOrWhiteSpace(pais[0]) ? int.Parse(pais[0]) : 0;
                        participante.Endereco.Cidade.Pais.NomePais = pais[1].Trim();
                    }
                    else
                        participante.Endereco.Cidade.Pais.NomePais = pais[0].Trim();
                }
                participante.AtualizarEnderecoPessoa = true;

            }
            return participante;
        }

        private bool PreecherDadosCTe(ref Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte, HtmlAgilityPack.HtmlDocument doc)
        {
            cte.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao;
            HtmlNode DivCTe = doc.DocumentNode.SelectSingleNode("//*[@id='cte']");

            if (DivCTe != null)
            {
                HtmlNodeCollection htmlGerais = DivCTe.SelectNodes("//*[@id='cte']/div");


                for (int i = 0; i < htmlGerais.Count; i++)
                {
                    HtmlNode htmlTitulo = htmlGerais[i];
                    string titulo = Utilidades.String.RemoveDiacritics(htmlTitulo.InnerText.Replace("\r\n", "").Trim().ToLower());

                    if (titulo.Contains("dados do ct-e"))
                    {
                        HtmlNode htmlDadosCTe = DivCTe.SelectSingleNode("table[" + (i + 1).ToString() + "]");
                        if (htmlDadosCTe != null)
                        {
                            HtmlNode htmlNumero = htmlDadosCTe.SelectSingleNode("tr[2]/td[1]");
                            if (htmlNumero != null)
                                cte.Numero = int.Parse(htmlNumero.InnerText.Trim().Replace("\r\n", ""));

                            HtmlNode htmlSerie = htmlDadosCTe.SelectSingleNode("tr[2]/td[2]");
                            if (htmlSerie != null)
                                cte.Serie = htmlSerie.InnerText.Trim().Replace("\r\n", "");

                            HtmlNode htmlVersao = htmlDadosCTe.SelectSingleNode("tr[2]/td[4]");
                            if (htmlVersao != null)
                                cte.Versao = htmlVersao.InnerText.Trim().Replace("\r\n", "");

                            HtmlNode htmlDataEmissao = htmlDadosCTe.SelectSingleNode("tr[2]/td[3]");
                            if (htmlDataEmissao != null)
                            {
                                string[] sdataEmissao = htmlDataEmissao.InnerText.Trim().Replace("\r\n", "").Split('-');
                                string strindataEmissao = sdataEmissao[0].Replace(" ", "") + " " + sdataEmissao[1].Replace(" ", "");
                                DateTime dataEmissao;
                                DateTime.TryParseExact(strindataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                                if (dataEmissao != DateTime.MinValue)
                                    cte.DataEmissao = dataEmissao;
                                else
                                    cte.DataEmissao = DateTime.Now;
                            }

                        }
                    }
                    if (titulo.Contains("caracteristicas"))
                    {
                        HtmlNode htmlCaracteristicas = DivCTe.SelectSingleNode("table[" + (i + 1).ToString() + "]");
                        if (htmlCaracteristicas != null)
                        {
                            HtmlNode htmlTipoModal = htmlCaracteristicas.SelectSingleNode("tr[2]/td[1]");
                            if (htmlTipoModal != null)
                            {
                                string tipoModal = Utilidades.String.RemoveDiacritics(htmlTipoModal.InnerText.Trim().Replace("\r\n", "").ToLower());
                                if (tipoModal.Contains("rodoviario"))
                                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario;
                                if (tipoModal.Contains("aereo"))
                                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aereo;
                                if (tipoModal.Contains("aquaviario"))
                                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario;
                                if (tipoModal.Contains("ferroviario"))
                                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Ferroviario;
                                if (tipoModal.Contains("dutoviario"))
                                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Dutoviario;
                                if (tipoModal.Contains("multimodal"))
                                    cte.TipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal;
                            }

                            HtmlNode htmlTipoServico = htmlCaracteristicas.SelectSingleNode("tr[2]/td[2]");
                            if (htmlTipoServico != null)
                            {
                                string tipoServico = Utilidades.String.RemoveDiacritics(htmlTipoServico.InnerText.Trim().Replace("\r\n", "").ToLower());
                                if (tipoServico.Contains("normal"))
                                    cte.TipoServico = Dominio.Enumeradores.TipoServico.Normal;
                                if (tipoServico.Contains("redespacho"))
                                    cte.TipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                                if (tipoServico.Contains("intermediario"))
                                    cte.TipoServico = Dominio.Enumeradores.TipoServico.RedIntermediario;
                                if (tipoServico.Contains("subcontratacao"))
                                    cte.TipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;
                            }


                            HtmlNode htmlTipoCTe = htmlCaracteristicas.SelectSingleNode("tr[2]/td[3]");
                            if (htmlTipoCTe != null)
                            {
                                string tipoCte = Utilidades.String.RemoveDiacritics(htmlTipoCTe.InnerText.Trim().Replace("\r\n", "").ToLower());
                                if (tipoCte.Contains("normal"))
                                    cte.TipoCTE = Dominio.Enumeradores.TipoCTE.Normal;
                                if (tipoCte.Contains("anulacao"))
                                    cte.TipoCTE = Dominio.Enumeradores.TipoCTE.Anulacao;
                                if (tipoCte.Contains("complemento"))
                                {
                                    cte.TipoCTE = Dominio.Enumeradores.TipoCTE.Complemento;
                                    PreencherCTeComplmentado(ref cte, doc);
                                }
                                if (tipoCte.Contains("substituto"))
                                    cte.TipoCTE = Dominio.Enumeradores.TipoCTE.Substituto;
                            }

                            HtmlNode htmlCFOP = htmlCaracteristicas.SelectSingleNode("tr[4]/td[1]");
                            if (htmlCFOP != null)
                                cte.CFOP = int.Parse(htmlCFOP.InnerText.Trim().Replace("\r\n", ""));

                            HtmlNode htmlNaturezaOP = htmlCaracteristicas.SelectSingleNode("tr[4]/td[2]");
                            if (htmlNaturezaOP != null)
                                cte.NaturezaOP = htmlNaturezaOP.InnerText.Trim().Replace("\r\n", "");

                            HtmlNode htmlOrigem = htmlCaracteristicas.SelectSingleNode("tr[6]/td[1]");
                            if (htmlOrigem != null)
                            {
                                string localidadeText = htmlOrigem.InnerText.Trim().Replace("\r\n", "");
                                string[] localidade = localidadeText.Split('-');

                                cte.LocalidadeInicioPrestacao = new Dominio.ObjetosDeValor.Localidade();
                                if (localidade.Length == 2)
                                {
                                    cte.LocalidadeInicioPrestacao.Descricao = localidade[1].Trim();
                                    cte.LocalidadeInicioPrestacao.SiglaUF = localidade[0].Trim();
                                }
                                else
                                {
                                    cte.LocalidadeInicioPrestacao.Descricao = localidadeText;
                                }
                            }

                            HtmlNode htmlDestino = htmlCaracteristicas.SelectSingleNode("tr[6]/td[2]");
                            if (htmlDestino != null)
                            {
                                string localidadeText = htmlDestino.InnerText.Trim().Replace("\r\n", "");
                                string[] localidade = localidadeText.Split('-');

                                cte.LocalidadeFimPrestacao = new Dominio.ObjetosDeValor.Localidade();
                                if (localidade.Length == 2)
                                {
                                    cte.LocalidadeFimPrestacao.Descricao = localidade[1].Trim();
                                    cte.LocalidadeFimPrestacao.SiglaUF = localidade[0].Trim();
                                }
                                else
                                {
                                    cte.LocalidadeFimPrestacao.Descricao = localidadeText;
                                }
                            }
                        }
                    }


                    if (titulo.Contains("situacao atual"))
                    {
                        if (titulo.Contains("autorizada"))
                            cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada;
                        else if (titulo.Contains("denegada"))
                            cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada;
                        else if (titulo.Contains("denegada"))
                            cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Denegada;
                        else
                            cte.SituacaoCTeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada;
                    }

                }

                return true;
            }
            else
            {
                return false;
            }

        }


        private void PreecherEndereco(Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco, HtmlNode Logradouro)
        {
            string[] splitEndereco = Logradouro.InnerText.Split(',');
            endereco.Logradouro = splitEndereco[0].Trim().Replace("\r\n", "");
            if (splitEndereco.Length > 1)
            {
                string[] splitNumero = splitEndereco[1].Split('-');
                string numeroSefaz = splitNumero[0].Trim().Replace("\r\n", "");

                string numero = "";
                string complemento = "";
                bool aindaNumerico = true;
                for (int i = 0; i < numeroSefaz.Count(); i++)
                {
                    if (aindaNumerico)
                    {
                        if (IsNumeric(numeroSefaz[i].ToString()))
                            numero += numeroSefaz[i].ToString();
                        else
                        {
                            aindaNumerico = false;
                            complemento += numeroSefaz[i].ToString();
                        }
                    }
                    else
                    {
                        complemento += numeroSefaz[i].ToString();
                    }
                }
                endereco.Numero = numero;
                if (endereco.Numero == "0" || endereco.Numero == "")
                {
                    endereco.Numero = "S/N";
                    if (complemento.ToLower().Contains("s/n"))
                    {
                        complemento = complemento.Replace("s/n", "").Replace("S/N", "");
                    }
                }
                endereco.Complemento = complemento.Trim();
                if (splitNumero.Length > 1)
                {
                    endereco.Complemento += splitNumero[1];
                }
                if (splitEndereco.Length > 2)
                    endereco.Complemento += splitEndereco[2];
            }
            else
            {
                endereco.Numero = "S/N";
            }
        }

        private static Boolean IsNumeric(string stringNum)
        {
            int result;
            if (int.TryParse(stringNum, out result))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
