using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.IO;

namespace Servicos.Embarcador.NFe
{
    public class ConsultaReceita : ServicoBase
    {
        public ConsultaReceita(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal ProcessarHTMLRetorno(Stream html, string tBody)
        {
            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(html);

            bool encontrou = PreecherDadosNFe(ref notaFiscal, doc, tBody);
            if (encontrou)
            {
                PreecherDadosEmitente(ref notaFiscal, doc, tBody);
                PreecherDadosDestinatario(ref notaFiscal, doc, tBody);
                PreecherDadosTotais(ref notaFiscal, doc, tBody);
                PreecherDadosTransporte(ref notaFiscal, doc, tBody);
                PreencherDadosAdicionais(ref notaFiscal, doc, tBody);

                //html.Dispose();
                //html.Close();
                return notaFiscal;
            }
            else
            {
                html.Dispose();
                html.Close();
                return null;
            }

        }

        public List<string> VerificarMensagemErro(Stream html)
        {
            List<string> Erros = new List<string>();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(html);

            HtmlNodeCollection erros = doc.DocumentNode.SelectNodes("//*[@id='ctl00_ContentPlaceHolder1_vdsErros']//li");
            if (erros != null)
            {
                for (int i = 0; i < erros.Count; i++)
                {
                    if (erros[i] != null)
                        Erros.Add(erros[i].InnerText.Trim().Replace("\r\n", ""));
                }
            }

            HtmlNodeCollection msgErros = doc.DocumentNode.SelectNodes("//*[@id='ctl00_ContentPlaceHolder1_bltMensagensErro']//li");
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

        private void PreecherDadosTransporte(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlAgilityPack.HtmlDocument doc, string body)
        {
            notaFiscal.Volumes = new List<Dominio.ObjetosDeValor.Embarcador.NFe.Volume>();
            HtmlNodeCollection transporte = doc.DocumentNode.SelectNodes("//*[@id='Transporte']/fieldset");
            if (transporte != null)
            {
                for (int i = 0; i < transporte.Count; i++)
                {
                    HtmlNode Legend = transporte[i].SelectSingleNode("legend");

                    if (Legend != null)
                    {
                        string titulo = Legend.InnerText.Replace("\r\n", "").Trim().ToLower();

                        if (titulo.Contains("dados do transporte"))
                            PreencherModalidadeFrete(ref notaFiscal, transporte[i], body);
                        if (titulo.Contains("transportador"))
                            PreecherDadosTransportador(ref notaFiscal, transporte[i], body);
                        if (titulo.Contains("veículo") || titulo.Contains("veiculo") || titulo.Contains("veã­culo"))
                            PreecherDadosVeiculo(ref notaFiscal, transporte[i], body);
                        if (titulo.Contains("reboque"))
                            PreecherDadosReboque(ref notaFiscal, transporte[i], body);
                        if (titulo.Contains("volumes"))
                        {
                            HtmlNodeCollection Volumes = transporte[i].SelectNodes("table");
                            if (Volumes != null)
                            {
                                for (int j = 0; j < Volumes.Count; j++)
                                {
                                    PreecherDadosVolumes(ref notaFiscal, Volumes[j], body);
                                }
                            }
                        }
                    }
                }
            }
            notaFiscal.PesoLiquido = notaFiscal.Volumes.Sum(obj => obj.PesoLiquido);
            notaFiscal.PesoBruto = notaFiscal.Volumes.Sum(obj => obj.PesoBruto);
            notaFiscal.VolumesTotal = notaFiscal.Volumes.Sum(obj => obj.Quantidade);
        }

        private void PreecherDadosTransportador(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlNode HTMLtransportador, string body)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa transportador = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
            HtmlNode CNPJ = HTMLtransportador.SelectSingleNode("table/" + body + "tr[1]/td[1]/span");
            if (CNPJ != null)
                transportador.CNPJ = CNPJ.InnerText.Trim().Replace("\r\n", "");

            HtmlNode RazaoSocial = HTMLtransportador.SelectSingleNode("table/" + body + "tr[1]/td[2]/span");
            if (RazaoSocial != null)
                transportador.RazaoSocial = RazaoSocial.InnerText.Trim().Replace("\r\n", "");

            HtmlNode IE = HTMLtransportador.SelectSingleNode("table/" + body + "tr[2]/td[1]/span");
            if (RazaoSocial != null)
                transportador.IE = IE.InnerText.Trim().Replace("\r\n", "");

            transportador.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();

            HtmlNode Logradouro = HTMLtransportador.SelectSingleNode("table/" + body + "tr[2]/td[2]/span");
            if (Logradouro != null)
                transportador.Endereco.Logradouro = Logradouro.InnerText.Trim().Replace("\r\n", "");

            transportador.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();

            HtmlNode Cidade = HTMLtransportador.SelectSingleNode("table/" + body + "tr[2]/td[3]/span");
            if (Cidade != null)
                transportador.Endereco.Cidade.Descricao = Cidade.InnerText.Trim().Replace("\r\n", "");

            HtmlNode UF = HTMLtransportador.SelectSingleNode("table/" + body + "tr[3]/td[1]/span");
            if (UF != null)
                transportador.Endereco.Cidade.SiglaUF = UF.InnerText.Trim().Replace("\r\n", "");

            notaFiscal.Transportador = transportador;
        }

        private void PreecherDadosReboque(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlNode HTMLVeiculo, string body)
        {
            if (notaFiscal.Veiculo == null)
                PreecherDadosVeiculo(ref notaFiscal, HTMLVeiculo, body);
            else
            {
                if (notaFiscal.Veiculo.Reboques == null)
                    notaFiscal.Veiculo.Reboques = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();


                Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboque = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();

                HtmlNode Placa = HTMLVeiculo.SelectSingleNode("table/" + body + "tr/td[1]/span");
                if (Placa != null)
                    reboque.Placa = Placa.InnerText.Trim().Replace("\r\n", "");

                HtmlNode UF = HTMLVeiculo.SelectSingleNode("table/" + body + "tr/td[2]/span");
                if (Placa != null)
                    reboque.UF = UF.InnerText.Trim().Replace("\r\n", "");

                HtmlNode RNTC = HTMLVeiculo.SelectSingleNode("table/" + body + "tr/td[3]/span");
                if (Placa != null)
                    reboque.RNTC = RNTC.InnerText.Trim().Replace("\r\n", "");

                reboque.TipoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque;

                notaFiscal.Veiculo.Reboques.Add(reboque);
            }
        }

        private void PreecherDadosVeiculo(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlNode HTMLVeiculo, string body)
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculo = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();

            HtmlNode Placa = HTMLVeiculo.SelectSingleNode("table/" + body + "tr/td[1]/span");
            if (Placa != null)
                veiculo.Placa = Placa.InnerText.Trim().Replace("\r\n", "");

            HtmlNode UF = HTMLVeiculo.SelectSingleNode("table/" + body + "tr/td[2]/span");
            if (Placa != null)
                veiculo.UF = UF.InnerText.Trim().Replace("\r\n", "");

            HtmlNode RNTC = HTMLVeiculo.SelectSingleNode("table/" + body + "tr/td[3]/span");
            if (Placa != null)
                veiculo.RNTC = RNTC.InnerText.Trim().Replace("\r\n", "");

            veiculo.TipoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao;

            notaFiscal.Veiculo = veiculo;
        }

        private void PreencherModalidadeFrete(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlNode HTMLTransporte, string body)
        {
            notaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;

            HtmlNode ModalidadeFrete = HTMLTransporte.SelectSingleNode("table/" + body + "tr/td/span");
            if (ModalidadeFrete != null)
            {
                int modalidadeFrete = int.Parse(ModalidadeFrete.InnerText.Replace("\r\n", "").Trim().Split('-')[0]);
                if (modalidadeFrete == 1)
                    notaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                else if (modalidadeFrete == 0)
                    notaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                else if (modalidadeFrete == 2)
                    notaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
                else
                    notaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido;
            }
        }

        private void PreecherDadosVolumes(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlNode HTMLVolume, string body)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            Dominio.ObjetosDeValor.Embarcador.NFe.Volume volume = new Dominio.ObjetosDeValor.Embarcador.NFe.Volume();

            HtmlNode Quantidade = HTMLVolume.SelectSingleNode(body + "tr[2]/td[1]/span");
            if (Quantidade != null)
            {
                string quantidade = Quantidade.InnerText.Trim().Replace("\r\n", "");
                volume.Quantidade = !string.IsNullOrWhiteSpace(quantidade) ? int.Parse(quantidade) : 0;
            }

            HtmlNode Especie = HTMLVolume.SelectSingleNode(body + "tr[2]/td[2]/span");
            if (Especie != null)
                volume.Especie = Especie.InnerText.Trim().Replace("\r\n", "");

            HtmlNode MarcaVolume = HTMLVolume.SelectSingleNode(body + "tr[2]/td[3]/span");
            if (MarcaVolume != null)
                volume.MarcaVolume = MarcaVolume.InnerText.Trim().Replace("\r\n", "");

            HtmlNode Numeracao = HTMLVolume.SelectSingleNode(body + "tr[3]/td[1]/span");
            if (Numeracao != null)
                volume.Numeracao = Numeracao.InnerText.Trim().Replace("\r\n", "");

            HtmlNode PesoLiquido = HTMLVolume.SelectSingleNode(body + "tr[3]/td[2]/span");
            if (PesoLiquido != null)
            {
                string peso = PesoLiquido.InnerText.Trim().Replace(body + "\r\n", "");
                volume.PesoLiquido = !string.IsNullOrEmpty(peso) ? decimal.Parse(peso, cultura) : 0;
            }

            HtmlNode PesoBruto = HTMLVolume.SelectSingleNode(body + "tr[3]/td[3]/span");
            if (PesoBruto != null)
            {
                string peso = PesoBruto.InnerText.Trim().Replace(body + "\r\n", "");
                volume.PesoBruto = !string.IsNullOrEmpty(peso) ? decimal.Parse(peso, cultura) : 0;
            }

            HtmlNode NumeroLacre = HTMLVolume.SelectSingleNode(body + "tr[4]/td[1]/span");
            if (NumeroLacre != null)
                volume.NumeroLacre = NumeroLacre.InnerText.Trim().Replace("\r\n", "");

            notaFiscal.Volumes.Add(volume);
        }

        private void PreecherDadosTotais(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlAgilityPack.HtmlDocument doc, string body)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            HtmlNode DivTotais = doc.DocumentNode.SelectSingleNode("//*[@id='Totais']");

            HtmlNode baseICMS = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[1]/td[1]/span");
            if (baseICMS != null && !string.IsNullOrWhiteSpace(baseICMS.InnerText))
                notaFiscal.BaseCalculoICMS = decimal.Parse(baseICMS.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode valorICMS = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[1]/td[2]/span");
            if (valorICMS != null && !string.IsNullOrWhiteSpace(valorICMS.InnerText))
                notaFiscal.ValorICMS = decimal.Parse(valorICMS.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode baseICMSST = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[1]/td[4]/span");
            if (baseICMSST != null && !string.IsNullOrWhiteSpace(baseICMSST.InnerText))
                notaFiscal.BaseCalculoST = decimal.Parse(baseICMSST.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode valorST = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[2]/td[1]/span");
            if (valorST != null && !string.IsNullOrWhiteSpace(valorST.InnerText))
                notaFiscal.ValorST = decimal.Parse(valorST.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode ValorTotalProdutos = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[2]/td[2]/span");
            if (ValorTotalProdutos != null && !string.IsNullOrWhiteSpace(ValorTotalProdutos.InnerText))
                notaFiscal.ValorTotalProdutos = decimal.Parse(ValorTotalProdutos.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode ValorFrete = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[2]/td[3]/span");
            if (ValorFrete != null && !string.IsNullOrWhiteSpace(ValorFrete.InnerText))
                notaFiscal.ValorFrete = decimal.Parse(ValorFrete.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode ValorSeguro = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[2]/td[4]/span");
            if (ValorSeguro != null && !string.IsNullOrWhiteSpace(ValorSeguro.InnerText))
                notaFiscal.ValorSeguro = decimal.Parse(ValorSeguro.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode ValorOutros = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[3]/td[1]/span");
            if (ValorOutros != null && !string.IsNullOrWhiteSpace(ValorOutros.InnerText))
                notaFiscal.ValorOutros = decimal.Parse(ValorOutros.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode ValorIPI = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[3]/td[2]/span");
            if (ValorIPI != null && !string.IsNullOrWhiteSpace(ValorIPI.InnerText))
                notaFiscal.ValorIPI = decimal.Parse(ValorIPI.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode ValorDesconto = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[3]/td[4]/span");
            if (ValorDesconto != null && !string.IsNullOrWhiteSpace(ValorDesconto.InnerText))
                notaFiscal.ValorDesconto = decimal.Parse(ValorDesconto.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode ValorImpostoImportacao = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[4]/td[1]/span");
            if (ValorImpostoImportacao != null && !string.IsNullOrWhiteSpace(ValorImpostoImportacao.InnerText))
                notaFiscal.ValorImpostoImportacao = decimal.Parse(ValorImpostoImportacao.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode ValorPIS = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[4]/td[2]/span");
            if (ValorPIS != null && !string.IsNullOrWhiteSpace(ValorPIS.InnerText))
                notaFiscal.ValorPIS = decimal.Parse(ValorPIS.InnerText.Trim().Replace("\r\n", ""), cultura);

            HtmlNode ValorCOFINS = DivTotais.SelectSingleNode("fieldset/fieldset/table/" + body + "tr[4]/td[3]/span");
            if (ValorCOFINS != null && !string.IsNullOrWhiteSpace(ValorCOFINS.InnerText))
                notaFiscal.ValorCOFINS = decimal.Parse(ValorCOFINS.InnerText.Trim().Replace("\r\n", ""), cultura);

        }

        private void PreencherDadosAdicionais(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlAgilityPack.HtmlDocument doc, string body)
        {
            var informacoesComplementares = "";
            HtmlNodeCollection informacoesAdicionais = doc.DocumentNode.SelectNodes("//*[@id='Inf']/fieldset");
            if (informacoesAdicionais != null)
            {
                for (int i = 0; i < informacoesAdicionais.Count; i++)
                {
                    HtmlNode Legend = informacoesAdicionais[i].SelectSingleNode("legend");

                    if (Legend != null)
                    {
                        string titulo = Legend.InnerText.Replace("\r\n", "").Trim().ToLower();

                        if (titulo.Contains("adicionais"))
                        {
                            HtmlNode divInformacoes = doc.DocumentNode.SelectSingleNode("//*[@id='Inf']");

                            HtmlNode informacoes = divInformacoes.SelectSingleNode("fieldset/fieldset/table/" + body + "tr/td/span/div");
                            if (informacoes != null)
                                informacoesComplementares = string.Concat(informacoesComplementares, " ", informacoes.InnerText.Trim().Replace("\r\n", ""));
                        }
                    }
                }
            }
            notaFiscal.InformacoesComplementares = informacoesComplementares;
        }

        private void PreecherDadosDestinatario(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlAgilityPack.HtmlDocument doc, string body)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
            destinatario.AtualizarEnderecoPessoa = true;

            HtmlNode DivDestinatario = doc.DocumentNode.SelectSingleNode("//*[@id='DestRem']");

            HtmlNode nome = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[1]/td/span");
            if (nome != null)
                destinatario.RazaoSocial = nome.InnerText.Trim().Replace("\r\n", "");

            HtmlNode tipoEmpresa = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[2]/td[1]/label");
            if (tipoEmpresa != null)
            {
                string tipo = tipoEmpresa.InnerHtml.Trim().Replace("\r\n", "").ToLower();
                if (tipo.Contains("estrangeiro"))
                    destinatario.ClienteExterior = true;
            }


            HtmlNode CNPJCPF = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[2]/td[1]/span");
            if (CNPJCPF != null)
                destinatario.CPFCNPJ = CNPJCPF.InnerText.Trim().Replace("\r\n", "");

            if (destinatario.CPFCNPJ.ToLower() == "exterior")
                destinatario.ClienteExterior = true;

            destinatario.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();

            HtmlNode Logradouro = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[2]/td[2]/span");
            if (Logradouro != null)
            {
                PreecherEndereco(destinatario.Endereco, Logradouro);
            }


            HtmlNode Bairro = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[3]/td[1]/span");
            if (Bairro != null)
                destinatario.Endereco.Bairro = Bairro.InnerText.Trim().Replace("\r\n", "");

            HtmlNode CEP = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[3]/td[2]/span");
            if (CEP != null)
                destinatario.Endereco.CEP = CEP.InnerText.Trim().Replace("\r\n", "");

            destinatario.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();

            HtmlNode Localidade = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[4]/td[1]/span");
            if (Localidade != null)
            {
                destinatario.Endereco.Cidade.IBGE = int.Parse(Localidade.InnerText.Split('-')[0]);
                destinatario.Endereco.Cidade.Descricao = Localidade.InnerText.Split('-')[1].Trim().Replace("\r\n", "").Replace("\r\n", "");
            }


            HtmlNode Telefone = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[4]/td[2]/span");
            if (Telefone != null)
                destinatario.Endereco.Telefone = Telefone.InnerText.Trim().Replace("\r\n", "");

            HtmlNode UF = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[5]/td[1]/span");
            if (UF != null)
                destinatario.Endereco.Cidade.SiglaUF = UF.InnerText.Trim().Replace("\r\n", "");

            destinatario.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais();
            HtmlNode Pais = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[5]/td[2]/span");
            if (Pais != null)
            {
                string[] splitPais = Pais.InnerText.Split('-');
                if (splitPais.Length > 1)
                {
                    destinatario.Endereco.Cidade.Pais.CodigoPais = int.Parse(splitPais[0]);
                    destinatario.Endereco.Cidade.Pais.NomePais = splitPais[1].Trim().Replace("\r\n", "");
                }
                else
                {
                    destinatario.Endereco.Cidade.Pais.NomePais = splitPais[0].Trim().Replace("\r\n", ""); ;
                }
            }

            HtmlNode Inscricao = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[6]/td[2]/span");
            if (Inscricao != null)
                destinatario.RGIE = Inscricao.InnerText.Trim().Replace("\r\n", "");

            HtmlNode InscricaoMunicipal = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[7]/td[1]/span");
            if (InscricaoMunicipal != null)
                destinatario.IM = InscricaoMunicipal.InnerText.Trim().Replace("\r\n", "");

            HtmlNode Email = DivDestinatario.SelectSingleNode("fieldset/table/" + body + "tr[7]/td[2]/span");
            if (Email != null)
                destinatario.Email = Utilidades.Email.ObterEmailsValidos(Email.InnerText.Trim().Replace("\r\n", ""), ';', ";");

            notaFiscal.Destinatario = destinatario;
        }

        private bool PreecherDadosNFe(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlAgilityPack.HtmlDocument doc, string body)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            HtmlNode DivNota = doc.DocumentNode.SelectSingleNode("//*[@id='NFe']");

            if (DivNota != null)
            {
                HtmlNode DivConteudoDinamico = doc.DocumentNode.SelectSingleNode("//*[@id='conteudoDinamico']");
                if (DivConteudoDinamico != null)
                {
                    notaFiscal.Chave = DivConteudoDinamico.SelectSingleNode("div[3]/div[1]/fieldset/table/" + body + "tr/td[1]/span").InnerText.Replace(" ", "");//DivConteudoDinamico.SelectSingleNode("").InnerText

                    notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;

                    HtmlNode Situacao = DivNota.SelectSingleNode("fieldset[5]/legend");
                    if (Situacao != null)
                    {
                        string situacao = Situacao.InnerText.Replace("\r\n", "").Trim().ToLower();
                        if (situacao.Contains("cancelada"))
                            notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Cancelada;
                        if (situacao.Contains("denegada"))
                            notaFiscal.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Denegada;
                    }

                    HtmlNode modelo = DivNota.SelectSingleNode("fieldset[1]/table/" + body + "tr/td[1]/span");
                    if (modelo != null)
                        notaFiscal.Modelo = modelo.InnerText.Trim().Replace("\r\n", "").Trim().Replace("\r\n", "");

                    HtmlNode numero = DivNota.SelectSingleNode("fieldset[1]/table/" + body + "tr/td[3]/span");
                    if (numero != null)
                        notaFiscal.Numero = int.Parse(numero.InnerText.Trim().Replace("\r\n", ""));

                    HtmlNode serie = DivNota.SelectSingleNode("fieldset[1]/table/" + body + "tr/td[2]/span");
                    if (serie != null)
                        notaFiscal.Serie = serie.InnerText.Trim().Replace("\r\n", "");

                    HtmlNode dataEmissao = DivNota.SelectSingleNode("fieldset[1]/table/" + body + "tr/td[4]/span");
                    if (dataEmissao != null)
                        notaFiscal.DataEmissao = dataEmissao.InnerText.Trim().Replace("\r\n", "").Split('-')[0];

                    HtmlNode valor = DivNota.SelectSingleNode("fieldset[1]/table/" + body + "tr/td[6]/span");
                    if (valor != null)
                        notaFiscal.Valor = decimal.Parse(valor.InnerText.Trim().Replace("\r\n", ""), cultura);

                    HtmlNode tipoOperacaoNotaFiscal = DivNota.SelectSingleNode("fieldset[4]/table/" + body + "tr[2]/td[2]/span");
                    if (tipoOperacaoNotaFiscal != null)
                        notaFiscal.TipoOperacaoNotaFiscal = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal)int.Parse(tipoOperacaoNotaFiscal.InnerText.Trim().Replace("\r\n", "").Split('-')[0]);

                    HtmlNode NaturezaOP = DivNota.SelectSingleNode("fieldset[4]/table/" + body + "tr[2]/td[1]/span");
                    if (NaturezaOP != null)
                        notaFiscal.NaturezaOP = NaturezaOP.InnerText.Trim().Replace("\r\n", "");

                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }

        }

        private void PreecherDadosEmitente(ref Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, HtmlAgilityPack.HtmlDocument doc, string body)
        {

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
            emitente.AtualizarEnderecoPessoa = true;


            HtmlNode DivEmitente = doc.DocumentNode.SelectSingleNode("//*[@id='Emitente']");

            HtmlNode EmitenteRazao = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[1]/td[1]/span");
            if (EmitenteRazao != null)
                emitente.RazaoSocial = EmitenteRazao.InnerText.Trim().Replace("\r\n", "");

            HtmlNode EmitenteFantasia = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[1]/td[2]/span");
            if (EmitenteFantasia != null)
                emitente.NomeFantasia = EmitenteFantasia.InnerText.Trim().Replace("\r\n", "");


            HtmlNode EmitenteCNPJ = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[2]/td[1]/span");
            if (EmitenteCNPJ != null)
                emitente.CPFCNPJ = EmitenteCNPJ.InnerText.Trim().Replace("\r\n", "");

            emitente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();

            HtmlNode EmitenteLogradouro = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[2]/td[2]/span");
            if (EmitenteLogradouro != null)
            {
                PreecherEndereco(emitente.Endereco, EmitenteLogradouro);
            }

            HtmlNode EmitenteBairro = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[3]/td[1]/span");
            if (EmitenteBairro != null)
                emitente.Endereco.Bairro = EmitenteBairro.InnerText.Trim().Replace("\r\n", "");

            HtmlNode EmitenteCEP = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[3]/td[2]/span");
            if (EmitenteCEP != null)
                emitente.Endereco.CEP = EmitenteCEP.InnerText.Trim().Replace("\r\n", "");

            emitente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();

            HtmlNode EmitenteLocalidade = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[4]/td[1]/span");
            if (EmitenteLocalidade != null)
            {
                emitente.Endereco.Cidade.IBGE = int.Parse(EmitenteLocalidade.InnerText.Split('-')[0]);
                emitente.Endereco.Cidade.Descricao = EmitenteLocalidade.InnerText.Split('-')[1].Trim().Replace("\r\n", "").Replace("\r\n", "");
            }

            HtmlNode EmitenteTelefone = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[4]/td[2]/span");
            if (EmitenteTelefone != null)
                emitente.Endereco.Telefone = EmitenteTelefone.InnerText.Trim().Replace("\r\n", "");

            HtmlNode EmitenteUF = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[5]/td[1]/span");
            if (EmitenteUF != null)
                emitente.Endereco.Cidade.SiglaUF = EmitenteUF.InnerText.Trim().Replace("\r\n", "");

            emitente.Endereco.Cidade.Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais();
            HtmlNode EmitentePais = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[5]/td[2]/span");
            if (EmitentePais != null)
            {
                string[] splitPais = EmitentePais.InnerText.Split('-');
                if (splitPais.Length > 1)
                {
                    emitente.Endereco.Cidade.Pais.CodigoPais = int.Parse(splitPais[0]);
                    emitente.Endereco.Cidade.Pais.NomePais = splitPais[1].Trim().Replace("\r\n", "");
                }
                else
                {
                    emitente.Endereco.Cidade.Pais.NomePais = splitPais[0].Trim().Replace("\r\n", ""); ;
                }

            }

            HtmlNode EmitenteInscricao = DivEmitente.SelectSingleNode("fieldset/table/" + body + "tr[6]/td[1]/span");
            if (EmitenteInscricao != null)
                emitente.RGIE = EmitenteInscricao.InnerText.Trim().Replace("\r\n", "");

            notaFiscal.Emitente = emitente;

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
                endereco.Complemento = complemento.Trim().Replace("   ", "");
                if (splitNumero.Length > 1)
                {
                    endereco.Complemento += splitNumero[1];
                }
                if (endereco.Complemento.Length > 60)
                {
                    endereco.Complemento = endereco.Complemento.Substring(0, 60);
                }
            }
            else
            {
                endereco.Numero = "S/N";
            }
        }

        private static bool IsNumeric(string stringNum)
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
