using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Servicos.Embarcador.Abastecimento
{
    public class ArquivoPlanilha : ServicoBase
    {
        public ArquivoPlanilha(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento ProcessarArquivoPlanilha(Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento configuracao, ExcelPackage package, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha repColunas = new Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
            Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);
            Repositorio.Embarcador.Moedas.Cotacao repCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repositorioLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

            ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

            Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento retornoAbastecimento = new Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento();
            List<Dominio.Entidades.Abastecimento> listaAbastecimento = new List<Dominio.Entidades.Abastecimento>();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            DateTime? campoDataHora = DateTime.MinValue;
            DateTime? campoData = DateTime.MinValue;
            DateTime? campoHora = DateTime.MinValue;
            DateTime? campoDataCRT = DateTime.MinValue;

            int kilometragem = 0;
            int kilometragemAnterior = 0;
            int horimetro = 0;

            decimal quantidade = 0;
            decimal valorUnitario = 0;
            decimal valorTotal = 0;
            decimal valorMoedaEstrangeira = 0;

            string numeroNota = "";
            string numeroCupom = "";
            string cnpjPosto = "";
            string nomePosto = "";
            string placa = "";
            string cpfMotorista = "";
            string nomeMotorista = "";
            string codigoIntegracao = "";
            string nomeProduto = "";
            string MsgRetorno = "";
            string placaVeiculoLetras = "";
            string placaVeiculoNumero = "";
            string enderecoPosto = "";
            string localArmazenamento = "";

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;

            bool primeiraLinha = true;
            int posicao = 0;
            for (var i = 1; i <= worksheet.Dimension.End.Row; i++)
            {
                if (i > worksheet.Dimension.End.Row)
                {
                    break;
                }
                if (!primeiraLinha)
                {
                    if (worksheet.Cells[i, 1].Value == null && worksheet.Cells[i, 2].Value == null && worksheet.Cells[i, 3].Value == null && !configuracao.GerarContasAPagarParaAbastecimentoExternos)
                        continue;

                    Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();
                    campoDataHora = DateTime.MinValue;
                    campoData = DateTime.MinValue;
                    campoHora = DateTime.MinValue;
                    campoDataCRT = DateTime.MinValue;

                    kilometragem = 0;
                    horimetro = 0;
                    kilometragemAnterior = 0;

                    quantidade = 0;
                    valorUnitario = 0;
                    valorTotal = 0;
                    valorMoedaEstrangeira = 0;

                    numeroNota = "";
                    numeroCupom = "";
                    cnpjPosto = "";
                    nomePosto = "";
                    placa = "";
                    cpfMotorista = "";
                    nomeMotorista = "";
                    codigoIntegracao = "";
                    nomeProduto = "";
                    placaVeiculoLetras = "";
                    placaVeiculoNumero = "";
                    enderecoPosto = "";
                    localArmazenamento = "";

                    moeda = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real;

                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.DataEHora, repColunas);
                    if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                        campoDataHora = RetornaDateTime(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.Data, repColunas);
                    if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                        campoData = RetornaDate(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.Hora, repColunas);
                    if (posicao > 0 && worksheet.Cells[i, posicao].Value != null)
                        campoHora = RetornaTime(posicao, worksheet.Cells[i, posicao].Value.ToString());

                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.DataBaseCTR, repColunas);
                    if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                        campoDataCRT = RetornaDateTime(posicao, worksheet.Cells[i, posicao].Text);
                    if (!campoDataCRT.HasValue || campoDataCRT.Value <= DateTime.MinValue)
                    {
                        posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.DataBaseCTR, repColunas);
                        if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                            campoDataCRT = RetornaDate(posicao, worksheet.Cells[i, posicao].Text);
                    }

                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.KMAbastecimento, repColunas);
                    if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                        kilometragem = RetornaNumerico(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.KMAnterior, repColunas);
                    if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                        kilometragemAnterior = RetornaNumerico(posicao, worksheet.Cells[i, posicao].Text);

                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.Horimetro, repColunas);
                    if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                        horimetro = RetornaNumerico(posicao, worksheet.Cells[i, posicao].Text);

                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.NumeroNota, repColunas);
                    if (posicao > 0)
                        numeroNota = RetornaString(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.NumeroCupom, repColunas);
                    if (posicao > 0)
                        numeroCupom = RetornaString(posicao, worksheet.Cells[i, posicao].Text);

                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.Quantidade, repColunas);
                    if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                        quantidade = RetornaDecimal(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.ValorUnitario, repColunas);
                    if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                        valorUnitario = RetornaDecimal(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.ValorTotal, repColunas);
                    if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                        valorTotal = RetornaDecimal(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.ValorMoedaEstrangeira, repColunas);
                    if (posicao > 0 && !string.IsNullOrWhiteSpace(worksheet.Cells[i, posicao].Text))
                        valorMoedaEstrangeira = RetornaDecimal(posicao, worksheet.Cells[i, posicao].Text);

                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.CNPJPosto, repColunas);
                    if (posicao > 0)
                        cnpjPosto = RetornaString(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.NomePosto, repColunas);
                    if (posicao > 0)
                        nomePosto = RetornaString(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.Placa, repColunas);
                    if (posicao > 0)
                        placa = RetornaString(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.CPFMotorista, repColunas);
                    if (posicao > 0)
                        cpfMotorista = RetornaString(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.NomeMotorista, repColunas);
                    if (posicao > 0)
                        nomeMotorista = RetornaString(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.CodigoProduto, repColunas);
                    if (posicao > 0)
                        codigoIntegracao = RetornaString(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.DescricaoProduto, repColunas);
                    if (posicao > 0)
                        nomeProduto = RetornaString(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.EnderecoPosto, repColunas);
                    if (posicao > 0)
                        enderecoPosto = RetornaString(posicao, worksheet.Cells[i, posicao].Text);

                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.PlacaVeiculoLetras, repColunas);
                    if (posicao > 0)
                        placaVeiculoLetras = RetornaString(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.PlacaVeiculoNumero, repColunas);
                    if (posicao > 0)
                        placaVeiculoNumero = RetornaString(posicao, worksheet.Cells[i, posicao].Text);
                    posicao = RetornaPosicaoColuna(configuracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.LocalArmazenamento, repColunas);
                    if (posicao > 0)
                        localArmazenamento = RetornaString(posicao, worksheet.Cells[i, posicao].Text);

                    Dominio.Entidades.Veiculo veiculo = !string.IsNullOrWhiteSpace(placa) ? repVeiculo.BuscarPorPlaca(placa) : null;
                    Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = !string.IsNullOrWhiteSpace(placa) ? repEquipamento.BuscarPorDescricaoParaAbastecimento(placa) : null;

                    if (veiculo == null && equipamento == null)
                    {
                        string novaPlaca = placaVeiculoLetras.Trim() + placaVeiculoNumero.Trim();
                        if (!string.IsNullOrWhiteSpace(novaPlaca))
                        {
                            veiculo = !string.IsNullOrWhiteSpace(novaPlaca) ? repVeiculo.BuscarPorPlaca(novaPlaca) : null;
                            equipamento = !string.IsNullOrWhiteSpace(novaPlaca) ? repEquipamento.BuscarPorDescricaoParaAbastecimento(novaPlaca) : null;
                        }
                    }

                    if (campoDataHora.HasValue && campoDataHora > DateTime.MinValue)
                        abastecimento.Data = campoDataHora;
                    else if (campoData.HasValue && campoHora.HasValue)
                        abastecimento.Data = DateTime.Parse(campoData.ToString().Substring(0, 10) + " " + campoHora.ToString().Substring(11, 8));

                    if (equipamento != null && horimetro <= 0)
                        abastecimento.Horimetro = kilometragem;
                    else
                    {
                        abastecimento.Kilometragem = kilometragem;
                        abastecimento.Horimetro = horimetro;
                    }

                    abastecimento.MoedaCotacaoBancoCentral = moeda;
                    abastecimento.DataBaseCRT = campoDataCRT.HasValue && campoDataCRT.Value > DateTime.MinValue ? campoDataCRT : null;
                    abastecimento.ValorOriginalMoedaEstrangeira = valorMoedaEstrangeira * quantidade;
                    abastecimento.ValorMoedaCotacao = 0m;

                    abastecimento.Litros = quantidade;
                    abastecimento.NomePosto = nomePosto;
                    if (!string.IsNullOrWhiteSpace(numeroNota))
                        abastecimento.Documento = numeroNota;
                    else
                        abastecimento.Documento = numeroCupom;
                    abastecimento.Pago = false;
                    abastecimento.Situacao = "A";
                    abastecimento.DataAlteracao = DateTime.Now;
                    abastecimento.Status = "A";
                    if (valorUnitario > 0)
                        abastecimento.ValorUnitario = valorUnitario;
                    else if (valorTotal > 0)
                        abastecimento.ValorUnitario = quantidade / valorTotal;
                    else
                        abastecimento.ValorUnitario = 0;
                    abastecimento.Veiculo = veiculo;
                    abastecimento.Equipamento = equipamento;
                    if (!string.IsNullOrWhiteSpace(cnpjPosto))
                        abastecimento.Posto = repCliente.BuscarPorCPFCNPJ(double.Parse(cnpjPosto));
                    else if (!string.IsNullOrWhiteSpace(enderecoPosto) && !string.IsNullOrWhiteSpace(nomePosto))
                        abastecimento.Posto = repCliente.BuscarPorNomeEndereco(nomePosto, enderecoPosto);
                    if (!string.IsNullOrWhiteSpace(cpfMotorista))
                        abastecimento.Motorista = repUsuario.BuscarMotoristaPorCPF(cpfMotorista);
                    else if (veiculo != null)
                        abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                    if (!string.IsNullOrWhiteSpace(localArmazenamento))
                        abastecimento.LocalArmazenamento = repositorioLocalArmazenamentoProduto.BuscarPorCodigoIntegracao(localArmazenamento);

                    Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

                    if (abastecimento.Motorista == null && abastecimento.Equipamento != null)
                    {
                        Dominio.Entidades.Veiculo veiculoEquipamento = repVeiculo.BuscarPorEquipamento(abastecimento.Equipamento.Codigo);
                        Dominio.Entidades.Usuario MotoristaEquipamento = veiculoEquipamento != null ? repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoEquipamento.Codigo) : null;

                        if (veiculoEquipamento != null && MotoristaEquipamento != null)
                            abastecimento.Motorista = MotoristaEquipamento;
                        else if (veiculoEquipamento != null)
                        {
                            Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(veiculoEquipamento.Codigo);
                            if (veiculoTracao != null)
                                abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                        }
                    }
                    else if (abastecimento.Motorista == null && abastecimento.Veiculo != null)
                    {
                        Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(abastecimento.Veiculo.Codigo);
                        if (veiculoTracao != null)
                            abastecimento.Motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                    }

                    if (string.IsNullOrWhiteSpace(cnpjPosto) && abastecimento.Posto != null)
                        cnpjPosto = abastecimento.Posto.CPF_CNPJ.ToString();

                    if (!string.IsNullOrWhiteSpace(codigoIntegracao) && !string.IsNullOrWhiteSpace(cnpjPosto))
                        abastecimento.Produto = repProduto.BuscarPorPostoTabelaDeValor(double.Parse(cnpjPosto), codigoIntegracao);

                    if (abastecimento.Produto != null && !string.IsNullOrWhiteSpace(abastecimento.Produto.CodigoNCM) && abastecimento.Produto.CodigoNCM.StartsWith("310210"))
                        abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;
                    else
                        abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;

                    if (abastecimento.DataBaseCRT.HasValue && abastecimento.DataBaseCRT.Value > DateTime.MinValue && abastecimento.Posto != null && abastecimento.Produto != null)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores tabelaValores = repPostoCombustivelTabelaValores.BuscarModalidadeFornecedor(abastecimento.Produto.Codigo, abastecimento.Posto.CPF_CNPJ, abastecimento.DataBaseCRT.HasValue ? abastecimento.DataBaseCRT.Value : abastecimento.Data.HasValue ? abastecimento.Data.Value : DateTime.Now);
                        if (tabelaValores != null)
                        {
                            moeda = tabelaValores.MoedaCotacaoBancoCentral;
                            abastecimento.MoedaCotacaoBancoCentral = moeda;
                            abastecimento.ValorMoedaCotacao = tabelaValores.ValorMoedaCotacao;
                        }
                        if (abastecimento.ValorMoedaCotacao == 0)
                        {
                            Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = repCotacao.BuscarCotacao(abastecimento.MoedaCotacaoBancoCentral.Value, abastecimento.DataBaseCRT.HasValue ? abastecimento.DataBaseCRT.Value : abastecimento.Data.HasValue ? abastecimento.Data.Value : DateTime.Now);
                            if (cotacao != null && cotacao.ValorMoeda > 0)
                                abastecimento.ValorMoedaCotacao = cotacao.ValorMoeda;
                        }
                    }
                    if (abastecimento.ValorMoedaCotacao > 0 && abastecimento.Litros > 0 && valorMoedaEstrangeira > 0 && moeda.HasValue && moeda.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real)
                    {
                        valorTotal = abastecimento.ValorOriginalMoedaEstrangeira * abastecimento.ValorMoedaCotacao;
                        abastecimento.ValorUnitario = valorTotal / abastecimento.Litros;
                    }

                    Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, veiculo, configuracao, configuracaoTMS);

                    abastecimento.ConfiguracaoAbastecimento = configuracao;

                    listaAbastecimento.Add(abastecimento);

                }
                primeiraLinha = false;

            }

            retornoAbastecimento.Abastecimentos = listaAbastecimento;
            retornoAbastecimento.MsgRetorno = MsgRetorno;

            return retornoAbastecimento;
        }

        public bool ValidarArquivoImportacaoPlanilha(Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento abastecimentos, out string mensagemErroImportacao, Repositorio.UnitOfWork unitOfWork)
        {
            bool valido = true;
            string mensagem = string.Empty;

            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

            //validações da importação de abastecimentos via planilha 
            foreach (var abastecimento in abastecimentos.Abastecimentos)
            {
                if (abastecimento.Posto != null && abastecimento.LocalArmazenamento == null && abastecimento.Posto.Modalidades != null && abastecimento.Posto.Modalidades.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas postoModalidadeFornecedor = repModalidadeFornecedor.BuscarPorCliente(abastecimento.Posto.CPF_CNPJ);
                    if (postoModalidadeFornecedor != null && postoModalidadeFornecedor.ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento)
                    {
                        mensagem += "Arquivo selecionado não está de acordo com as exigências. A informação do local de armazenamento é necessária para os postos que a exigem. <br/>Necessário corrigir a planilha e realizar uma nova importação.";
                        valido = false;
                    }
                }
            }

            mensagemErroImportacao = mensagem;

            return valido;
        }

        #endregion

        #region Métodos Privados

        private int RetornaPosicaoColuna(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha coluna, Repositorio.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha repColunas)
        {
            return repColunas.BuscarPosicaoColuna(codigo, coluna);
        }

        private DateTime? RetornaDateTime(int posicao, string value)
        {
            value = value.Trim();
            DateTime.TryParseExact(value, "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out DateTime dataConvertida);

            if (posicao > 0 && dataConvertida > DateTime.MinValue)
                return dataConvertida;
            else
            {
                DateTime.TryParse(value, out dataConvertida);
                if (posicao > 0 && dataConvertida > DateTime.MinValue)
                    return dataConvertida;
                else
                    return null;
            }
        }

        private DateTime? RetornaDate(int posicao, string value)
        {
            value = value.Trim();
            DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataConvertida);

            if (posicao > 0 && dataConvertida > DateTime.MinValue)
                return dataConvertida;
            else
            {
                DateTime.TryParse(value, out dataConvertida);
                if (posicao > 0 && dataConvertida > DateTime.MinValue)
                    return dataConvertida;
                else
                    return null;
            }
        }

        private DateTime? RetornaTime(int posicao, string value)
        {
            value = value.Trim();
            DateTime.TryParseExact(value, "HH:mm", null, DateTimeStyles.None, out DateTime dataConvertida);

            if (posicao > 0 && dataConvertida > DateTime.MinValue)
                return dataConvertida;
            else
            {
                DateTime.TryParse(value, out dataConvertida);
                if (posicao > 0 && dataConvertida > DateTime.MinValue)
                    return dataConvertida;
                else
                    return null;
            }
        }

        private int RetornaNumerico(int posicao, string value)
        {
            if (posicao > 0)
                return int.Parse(value);
            else
                return 0;
        }

        private decimal RetornaDecimal(int posicao, string value)
        {
            if (posicao > 0)
            {
                if (value.Contains(",") && value.Contains("."))
                    return decimal.Parse(value.Replace(".", ""));
                else
                    return decimal.Parse(value.Replace(".", ","));
            }
            else
                return 0;
        }

        private string RetornaString(int posicao, string value)
        {
            if (posicao > 0)
                return value;
            else
                return "";
        }

        #endregion
    }
}
