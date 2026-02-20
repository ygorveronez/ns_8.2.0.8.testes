using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Abastecimento
{
    public class ArquivoPostoReforco : ServicoBase
    {
        public ArquivoPostoReforco(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento ProcessarArquivoPostoReforco(ExcelPackage package, Repositorio.UnitOfWork unitOfWork)
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
            double cnpjPosto = 7906908000108;
            string nomePosto = "POSTO REFORCO 4 LTDA";
            string placa = "";
            string numeroDocumento = "";
            string codigoIntegracao = "";
            string nomeProduto = "";
            var cellValue = "";
            string MsgRetorno = "";

            int posNomeProduto = 6;
            int posCodigoProduto = 6;
            int posNumeroDocumento = 4;
            int posDataHora = 1;
            int posKM = 3;
            int posLitro = 7;
            int posValorUnitario = 9;
            int posPlaca = 2;

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
                        if (i == 8 && !string.IsNullOrWhiteSpace(cellValue))
                        {
                            if (cellValue.Trim() == "Descrição" || cellValue.Trim() == "Descricao")
                            {
                                posNomeProduto = a;
                                posCodigoProduto = a;
                            }
                            if (cellValue.Trim() == "CP / NF")
                            {
                                posNumeroDocumento = a;
                            }
                            if (cellValue.Trim() == "Data/Hora")
                            {
                                posDataHora = a;
                            }
                            if (cellValue.Trim() == "KM")
                            {
                                posKM = a;
                            }
                            if (cellValue.Trim() == "Quantidade")
                            {
                                posLitro = a;
                            }
                            if (cellValue.Trim() == "Preço unit." || cellValue.Trim() == "Preco unit.")
                            {
                                posValorUnitario = a;
                            }
                            if (cellValue.Trim() == "Placa")
                            {
                                posPlaca = a;
                            }
                        }
                        else if (i >= 9 && !string.IsNullOrWhiteSpace(cellValue) && cellValue != "Total do cliente")
                        {
                            placa = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posPlaca].Text);
                            veiculo = repVeiculo.BuscarPorPlaca(placa);
                            if (veiculo != null)
                            {
                                codigoIntegracao = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posCodigoProduto].Text);
                                nomeProduto = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posNomeProduto].Text);
                                numeroDocumento = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posNumeroDocumento].Text);
                                dataHora = DateTime.Parse(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posDataHora].Text));

                                km = 0;
                                int.TryParse(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posKM].Text), out km);

                                litros = 0;
                                decimal.TryParse(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posLitro].Text), out litros);

                                valorUnitario = 0;
                                decimal.TryParse(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, posValorUnitario].Text), out valorUnitario);

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
                            }
                            a = worksheet.Dimension.End.Column;
                        }
                    }
                    else
                        a = worksheet.Dimension.End.Column;
                }
            }

            retornoAbastecimento.Abastecimentos = listaAbastecimento;
            retornoAbastecimento.MsgRetorno = MsgRetorno;
            return retornoAbastecimento;
        }
    }
}
