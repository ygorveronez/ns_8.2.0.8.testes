using System;
using System.Text;
using System.IO;
using System.Globalization;

namespace Servicos.Embarcador.NotaFiscal
{
    public class ImpostoIBPTNFe : ServicoBase
    {
        public ImpostoIBPTNFe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public string ProcessarArquivoImpostoIBPTNFe(Stream stream, Repositorio.UnitOfWork unitOfWork, int codigoEmpresa)
        {
            Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe repImpostoIBPTNFe = new Repositorio.Embarcador.NotaFiscal.ImpostoIBPTNFe(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe imposto;
            var cellValue = "";

            string ncm, extensao, tipo, descricao, chave, versao, fonte;
            decimal nacionalFederal, importadosFederal, estadual, municipal;
            DateTime vigenciaInicio, vigenciaFim;

            StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding(CultureInfo.GetCultureInfo("pt-BR").TextInfo.ANSICodePage));
            int linha = 0;
            while ((cellValue = streamReader.ReadLine()) != null)
            {
                string[] linhaSeparada = cellValue.Split(';');
                if (linha == 0)
                {
                    cellValue = linhaSeparada[0];
                    if (cellValue.ToLower() != "codigo")
                        return "Arquivo não está no formato correto.";
                    cellValue = linhaSeparada[1];
                    if (cellValue.ToLower() != "ex")
                        return "Arquivo não está no formato correto.";
                    cellValue = linhaSeparada[2];
                    if (cellValue.ToLower() != "tipo")
                        return "Arquivo não está no formato correto.";
                    cellValue = linhaSeparada[3];
                    if (cellValue.ToLower() != "descricao")
                        return "Arquivo não está no formato correto.";
                }
                else
                {
                    ncm = linhaSeparada[0];
                    extensao = linhaSeparada[1];
                    tipo = linhaSeparada[2];
                    descricao = linhaSeparada[3].TrimStart('"').TrimEnd('"').Trim();
                    decimal.TryParse(linhaSeparada[4].Replace(".", ","), out nacionalFederal);
                    decimal.TryParse(linhaSeparada[5].Replace(".", ","), out importadosFederal);
                    decimal.TryParse(linhaSeparada[6].Replace(".", ","), out estadual);
                    decimal.TryParse(linhaSeparada[7].Replace(".", ","), out municipal);
                    DateTime.TryParse(linhaSeparada[8], out vigenciaInicio);
                    DateTime.TryParse(linhaSeparada[9], out vigenciaFim);
                    chave = linhaSeparada[10];
                    versao = linhaSeparada[11];
                    fonte = linhaSeparada[12];

                    imposto = repImpostoIBPTNFe.BuscarPorEmpresaNCM(0, codigoEmpresa, ncm, vigenciaInicio, vigenciaFim);
                    if (imposto == null)
                    {
                        imposto = new Dominio.Entidades.Embarcador.NotaFiscal.ImpostoIBPTNFe();
                        imposto.Descricao = descricao;
                        imposto.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                        imposto.Estadual = estadual;
                        imposto.Extensao = extensao;
                        imposto.Fonte = fonte;
                        imposto.ImportadosFederal = importadosFederal;
                        imposto.Municipal = municipal;
                        imposto.NacionalFederal = nacionalFederal;
                        imposto.NCM = ncm;
                        imposto.Tipo = tipo;
                        imposto.Versao = versao;
                        imposto.VigenciaFim = vigenciaFim;
                        imposto.VigenciaInicio = vigenciaInicio;

                        repImpostoIBPTNFe.Inserir(imposto);
                    }
                }
                linha = linha + 1;
            }

            return "";
        }
    }
}
