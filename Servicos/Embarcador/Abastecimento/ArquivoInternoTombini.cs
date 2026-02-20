using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Servicos.Embarcador.Abastecimento
{
    public class ArquivoInternoTombini : ServicoBase
    {
        public ArquivoInternoTombini(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento ProcessarArquivoInternoTombini(string caminhoArquivoZIP, string nomeArquivo, double cnpjPostoInterno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento retornoAbastecimento = new Dominio.ObjetosDeValor.Embarcador.Frota.Abastecimento();
            List<Dominio.Entidades.Abastecimento> listaAbastecimento = new List<Dominio.Entidades.Abastecimento>();

            string caminhoTemporario = Path.GetTempPath();

            if (System.IO.Directory.Exists(caminhoTemporario + nomeArquivo))
            {
                System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(caminhoTemporario + nomeArquivo);
                LimparDiretorio(directory);
                System.IO.Directory.Delete(caminhoTemporario + nomeArquivo);
            }

            System.IO.Compression.ZipFile.ExtractToDirectory(caminhoArquivoZIP, caminhoTemporario);
            string MsgRetorno = "";

            foreach (string file in Directory.EnumerateFiles(caminhoTemporario + nomeArquivo, "*.xml"))
            {

                XmlDocument doc = new XmlDocument();
                string data = Utilidades.IO.FileStorageService.Storage.ReadAllText(file).Replace("&", "");
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                Stream s = new MemoryStream(bytes);
                doc.Load(s);
                               
                string contents = doc.InnerXml;
                Dominio.ObjetosDeValor.Embarcador.Abastecimento.Raiz raizXML;
                Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();

                raizXML = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.Embarcador.Abastecimento.Raiz>(contents);
                string placa = raizXML.Transacoes.Placa;

                int kilometragem = raizXML.Transacoes.Odometro;
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(placa);

                double.TryParse(raizXML.Transacoes.EstabelecimentoCNPJ, out double CNPJPostoInternoXML);
                if (CNPJPostoInternoXML > 0)
                    cnpjPostoInterno = CNPJPostoInternoXML;


                abastecimento.Data = DateTime.Parse(raizXML.Transacoes.DataTransacao);
                abastecimento.Kilometragem = kilometragem;
                abastecimento.Litros = decimal.Parse(raizXML.Transacoes.Litros);
                abastecimento.NomePosto = raizXML.Transacoes.NomeReduzido;
                abastecimento.Pago = false;
                abastecimento.Situacao = "F";
                abastecimento.DataAlteracao = DateTime.Now;
                abastecimento.Status = "A";
                abastecimento.ValorUnitario = decimal.Parse(raizXML.Transacoes.ValorTransacao) / decimal.Parse(raizXML.Transacoes.Litros);
                abastecimento.Veiculo = veiculo;
                abastecimento.Posto = repCliente.BuscarPorCPFCNPJ(cnpjPostoInterno);
                abastecimento.Produto = repProduto.BuscarPorPostoTabelaDeValor(cnpjPostoInterno, raizXML.Transacoes.TipoCombustivel);
                abastecimento.Documento = raizXML.Transacoes.CodigoTransacao;

                Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

                if (abastecimento.Veiculo == null)
                {
                    if (!MsgRetorno.Contains(placa))
                        MsgRetorno = MsgRetorno + "- Veículo: " + placa + " não cadastrado.<br/>";
                }
                else if (abastecimento.Posto == null)
                {
                    if (!MsgRetorno.Contains(Convert.ToString(cnpjPostoInterno)))
                        MsgRetorno = MsgRetorno + "- Posto: " + raizXML.Transacoes.RazaoSocial + " CNPJ: " + Convert.ToString(cnpjPostoInterno) + " não cadastrado.<br/>";
                }
                else if (abastecimento.Produto == null)
                {
                    if (!MsgRetorno.Contains(raizXML.Transacoes.TipoCombustivel))
                        MsgRetorno = MsgRetorno + "- Posto: " + abastecimento.Posto.Nome + " CNPJ: " + Convert.ToString(cnpjPostoInterno) + " Código de Integração: " + raizXML.Transacoes.TipoCombustivel + " não cadastrado.<br/>";
                }
                else if (abastecimento.Litros > abastecimento.Veiculo.CapacidadeTanque && abastecimento.Veiculo.CapacidadeTanque > 0)
                {
                    MsgRetorno = MsgRetorno + "- Litros abastecidos no veículo " + placa + " é maior que sua Capacidade de Tanque (" + abastecimento.Veiculo.CapacidadeTanque.ToString() + ").";
                }
                else
                {
                    if (abastecimento.Produto.CodigoNCM.StartsWith("310210"))
                        abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;
                    else
                        abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                    listaAbastecimento.Add(abastecimento);
                }
            }

            retornoAbastecimento.Abastecimentos = listaAbastecimento;
            retornoAbastecimento.MsgRetorno = MsgRetorno;
            return retornoAbastecimento;
        }

        #endregion

        #region Métodos Privados

        private void LimparDiretorio(System.IO.DirectoryInfo directory)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        #endregion
    }
}
