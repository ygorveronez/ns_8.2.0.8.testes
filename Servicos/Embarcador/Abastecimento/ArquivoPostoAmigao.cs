using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Abastecimento
{
    public class ArquivoPostoAmigao : ServicoBase
    {
        public ArquivoPostoAmigao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento ProcessarArquivoPostoAmigao(ExcelPackage package, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

            ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

            Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento retornoAbastecimento = new Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento();
            List<Dominio.Entidades.Abastecimento> listaAbastecimento = new List<Dominio.Entidades.Abastecimento>();
            Dominio.Entidades.Abastecimento abastecimento = null;

            DateTime dataHora = DateTime.MinValue;
            int km = 0;
            decimal litros = 0;
            decimal valorUnitario = 0;
            string nomePosto = "";
            double cnpjPosto = 0;
            string placa = "";
            string numeroDocumento = "";
            string codigoIntegracao = "";
            string nomeProduto = "";
            var cellValue = "";
            string MsgRetorno = "";

            int posFilial = 10;
            int posNomeProduto = 16;
            int posCodigoProduto = 15;
            int posNumeroDocumento = 21;
            int posDataHora = 9;
            int posKM = 29;
            int posLitro = 35;
            int posValorUnitario = 38;

            Dominio.Entidades.Veiculo veiculo = null;
            for (var i = 1; i <= worksheet.Dimension.End.Row; i++)
            {
                for (var a = 1; i <= worksheet.Dimension.End.Column; a++)
                {
                    try
                    {
                        cellValue = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, a].Text);
                    }
                    catch (Exception)
                    {
                        cellValue = "";
                    }
                    if (a >= worksheet.Dimension.End.Column)
                    {
                        break;
                    }

                    if (cellValue != null && !string.IsNullOrWhiteSpace(cellValue))
                    {
                        if (i == 6 && !string.IsNullOrWhiteSpace(cellValue))
                        {
                            if (cellValue.Trim() == "Produto")
                            {
                                posNomeProduto = a + 2;
                                posCodigoProduto = a + 1;
                            }
                            if (cellValue.Trim() == "Nro NF")
                            {
                                posNumeroDocumento = a + 1;
                            }
                            if (cellValue.Trim() == "Data Hora Venda")
                            {
                                posDataHora = a;
                            }
                            if (cellValue.Trim() == "Odômetro" || cellValue.Trim() == "Odometro")
                            {
                                posKM = a;
                            }
                            if (cellValue.Trim() == "Qtde")
                            {
                                posLitro = a + 1;
                            }
                            if (cellValue.Trim() == "Vlr.Unit.")
                            {
                                posValorUnitario = a + 1;
                            }
                        }
                        if (cellValue == "Filial:" && i == 7)
                        {
                            nomePosto = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posFilial].Text);
                            if (nomePosto.Contains("AMIGAO"))
                                cnpjPosto = 7774853000120;
                            else if (nomePosto.Contains("IGUACU"))
                                cnpjPosto = 79063764000186;
                            else if (string.IsNullOrWhiteSpace(nomePosto))
                            {
                                nomePosto = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posFilial + 1].Text);
                                if (nomePosto.Contains("AMIGAO"))
                                    cnpjPosto = 7774853000120;
                                else if (nomePosto.Contains("IGUACU"))
                                    cnpjPosto = 79063764000186;
                            }
                            a = worksheet.Dimension.End.Column;
                        }
                        else if (cellValue == "Placa:")
                        {
                            placa = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, a + 1].Text);
                            veiculo = repVeiculo.BuscarPorPlaca(placa);
                            a = worksheet.Dimension.End.Column;
                        }
                        else if (veiculo != null && Utilidades.String.RemoveDiacritics(worksheet.Cells[i, 1].Text) != null && !string.IsNullOrWhiteSpace(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, 1].Text)))
                        {
                            if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posNomeProduto].Text))))
                            {
                                codigoIntegracao = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posCodigoProduto].Text);
                                nomeProduto = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posNomeProduto].Text);
                                numeroDocumento = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posNumeroDocumento].Text);
                                dataHora = DateTime.Parse(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posDataHora].Text));
                                km = int.Parse(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posKM].Text));
                                litros = decimal.Parse(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posLitro].Text));
                                valorUnitario = decimal.Parse(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posValorUnitario].Text));

                                if ((veiculo != null) && (dataHora > DateTime.MinValue) && (km > 0) && (litros > 0) && (valorUnitario > 0))
                                {
                                    abastecimento = new Dominio.Entidades.Abastecimento();
                                    abastecimento.Data = dataHora;
                                    abastecimento.Kilometragem = km;
                                    abastecimento.Litros = litros;
                                    abastecimento.NomePosto = nomePosto;
                                    abastecimento.Documento = numeroDocumento;
                                    abastecimento.Pago = false;
                                    abastecimento.Situacao = "F";
                                    abastecimento.DataAlteracao = DateTime.Now;
                                    abastecimento.Status = "A";
                                    abastecimento.ValorUnitario = valorUnitario;
                                    abastecimento.Veiculo = veiculo;
                                    if (cnpjPosto > 0)
                                        abastecimento.Posto = repCliente.BuscarPorCPFCNPJ(cnpjPosto);
                                    abastecimento.Produto = repProduto.BuscarPorPostoTabelaDeValor(cnpjPosto, codigoIntegracao);

                                    Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

                                    if (abastecimento.Produto == null)
                                    {
                                        if (!MsgRetorno.Contains(codigoIntegracao))
                                            MsgRetorno = MsgRetorno + "- Posto: " + nomePosto + " CNPJ: " + cnpjPosto + " Código de Integração: " + codigoIntegracao + " - " + nomeProduto + " não cadastrado.<br/>";
                                    }
                                    else if (abastecimento.Litros > abastecimento.Veiculo.CapacidadeTanque && abastecimento.Veiculo.CapacidadeTanque > 0)
                                    {
                                        MsgRetorno = MsgRetorno + "- Litros abastecidos no veículo " + abastecimento.Veiculo.Placa + " é maior que sua Capacidade de Tanque (" + abastecimento.Veiculo.CapacidadeTanque.ToString() + ").";
                                    }
                                    else
                                    {
                                        if (abastecimento.Produto.CodigoNCM.StartsWith("310210"))
                                            abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;
                                        else
                                            abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                                        listaAbastecimento.Add(abastecimento);
                                    }

                                    dataHora = DateTime.MinValue;
                                    km = 0;
                                    litros = 0;
                                    valorUnitario = 0;
                                    placa = "";
                                    numeroDocumento = "";
                                    codigoIntegracao = "";
                                    nomeProduto = "";
                                }
                                a = worksheet.Dimension.End.Column;
                            }
                        }
                    }
                }
            }

            retornoAbastecimento.Abastecimentos = listaAbastecimento;
            retornoAbastecimento.MsgRetorno = MsgRetorno;
            return retornoAbastecimento;
        }
    }
}
