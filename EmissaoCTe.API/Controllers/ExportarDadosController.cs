using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text;

namespace EmissaoCTe.API.Controllers
{
    public class ExportarDadosController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("exportardados.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Públicos

        [AcceptVerbs("POST", "GET")]
        public ActionResult ExportarEmpresas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar este recurso!");

                DateTime dataInicial = DateTime.MinValue;
                DateTime dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int codigoEmpresa = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarPorDataCadastro(dataInicial, dataFinal, codigoEmpresa, this.Usuario.Empresa.Codigo);
                if (listaEmpresas.Count == 0)
                    return Json<bool>(false, false, "Nenhuma empresa com data de atualização no período informado.");

                System.IO.MemoryStream arquivo = this.GerarArquivoTXTEmpresas(this.Usuario.Empresa.Codigo, codigoEmpresa, dataInicial, dataFinal, unidadeDeTrabalho);

                return Arquivo(arquivo, "text/plain", string.Concat("Empresas_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy_hhmmss"), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult ExportarEmissoes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar este recurso!");

                DateTime dataInicial = DateTime.MinValue;
                DateTime dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int codigoEmpresa = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = this.EmpresaUsuario.TipoAmbiente;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                int countCTes = repCTe.ContarCTesParaCobrancaMensal(this.EmpresaUsuario.Codigo, codigoEmpresa, dataInicial, dataFinal, tipoAmbiente, null, null);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                int countMDFes = repMDFe.ContarMDFesParaCobrancaMensal(this.EmpresaUsuario.Codigo, codigoEmpresa, dataInicial, dataFinal, tipoAmbiente, null, null);

                if (countCTes == 0 && countMDFes == 0)
                    return Json<bool>(false, false, "Nenhum documento com data de emissão no período informado.");

                System.IO.MemoryStream arquivo = this.GerarArquivoTXTEmissoes(this.Usuario.Empresa.Codigo, codigoEmpresa, dataInicial, dataFinal, tipoAmbiente, unidadeDeTrabalho);

                return Arquivo(arquivo, "text/plain", string.Concat("Empresas_", dataInicial.ToString("ddMMyy"), "_", dataFinal.ToString("ddMMyy_hhmmss"), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        #endregion

        private System.IO.MemoryStream GerarArquivoTXTEmpresas(int codigoEmpresaPai, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            MemoryStream memoStream = new MemoryStream();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarPorDataAtualizacao(dataInicial, dataFinal, codigoEmpresa, codigoEmpresaPai);

            if (listaEmpresas.Count > 0)
            {
                StringBuilder arquivo = null;
                arquivo = new StringBuilder();

                arquivo.Append(string.Concat("CNPJ", ";", "IE", ";", "RAZAO", ";", "FANTASIA", ";", "IBGE", ";", "CIDADE", ";", "UF", ";", "RUA", ";", "NUMERO", ";", "BAIRRO", ";", "CEP", ";", "TELEFONE", ";", "EMAIL"));
                arquivo.AppendLine();
                foreach (Dominio.Entidades.Empresa empresa in listaEmpresas)
                {
                    var email = !string.IsNullOrWhiteSpace(empresa.Email) ? empresa.Email : !string.IsNullOrWhiteSpace(empresa.EmailAdministrativo) ? empresa.EmailAdministrativo : !string.IsNullOrWhiteSpace(empresa.EmailContador) ? empresa.EmailContador : string.Empty;
                    email = email.Replace(";", ",");

                    arquivo.Append(string.Concat(empresa.CNPJ, ";",
                                                empresa.InscricaoEstadual, ";",
                                                empresa.RazaoSocial, ";",
                                                empresa.NomeFantasia, ";",
                                                empresa.Localidade.CodigoIBGE, ";",
                                                empresa.Localidade.Descricao, ";",
                                                empresa.Localidade.Estado.Sigla, ";",
                                                empresa.Endereco, ";",
                                                empresa.Numero, ";",
                                                empresa.Bairro, ";",
                                                empresa.CEP, ";",
                                                Utilidades.String.OnlyNumbers(empresa.Telefone), ";",
                                                email));
                    arquivo.AppendLine();
                }

                memoStream.Write(System.Text.Encoding.Default.GetBytes(arquivo.ToString()), 0, arquivo.ToString().Length);

                memoStream.Position = 0;
            }

            return memoStream;
        }

        private System.IO.MemoryStream GerarArquivoTXTEmissoes(int codigoEmpresaPai, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            MemoryStream memoStream = new MemoryStream();

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
            Repositorio.FaixaEmissaoCTe repFaixaEmissao = new Repositorio.FaixaEmissaoCTe(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarEmpresasParaCobranca(tipoAmbiente);

            if (listaEmpresas.Count > 0)
            {
                StringBuilder arquivo = null;
                arquivo = new StringBuilder();

                arquivo.Append(string.Concat("CNPJ", ";", "VALOR", ";", "DESCRICAO"));
                arquivo.AppendLine();
                foreach (Dominio.Entidades.Empresa empresa in listaEmpresas)
                {
                    if (empresa.PlanoEmissaoCTe != null && empresa.EmpresaCobradora == null)
                    {                        
                        int countCTes = repCTe.ContarCTesParaCobrancaMensal(this.EmpresaUsuario.Codigo, empresa.Codigo, dataInicial, dataFinal, tipoAmbiente, null, null);
                        int countMDFes = repMDFe.ContarMDFesParaCobrancaMensal(this.EmpresaUsuario.Codigo, empresa.Codigo, dataInicial, dataFinal, tipoAmbiente, null, null);

                        string empresasAssociadas = "";
                        List<Dominio.Entidades.Empresa> listaEmpresasAssociadas = repEmpresa.BuscarPorEmpresasDaEmpresaCobradora(empresa.CNPJ);
                        if (listaEmpresasAssociadas != null && listaEmpresasAssociadas.Count() > 0)
                        {
                            foreach (Dominio.Entidades.Empresa empresaAssociada in listaEmpresasAssociadas)
                            {
                                countCTes += repCTe.ContarCTesParaCobrancaMensal(this.EmpresaUsuario.Codigo, empresaAssociada.Codigo, dataInicial, dataFinal, tipoAmbiente, null, null);
                                countMDFes += repMDFe.ContarMDFesParaCobrancaMensal(this.EmpresaUsuario.Codigo, empresaAssociada.Codigo, dataInicial, dataFinal, tipoAmbiente, null, null);
                                empresasAssociadas = string.IsNullOrWhiteSpace(empresasAssociadas) ? empresaAssociada.CNPJ : empresasAssociadas + " / " + empresaAssociada.CNPJ;
                            }
                        }

                        int countTotal = countCTes + countMDFes;

                        decimal valor = 0;

                        if (countTotal > 0)
                        {
                            List<Dominio.Entidades.FaixaEmissaoCTe> listaFaixaEmissao = repFaixaEmissao.BuscarPorPlano(empresa.PlanoEmissaoCTe.Codigo);
                            foreach (Dominio.Entidades.FaixaEmissaoCTe faixaEmissao in listaFaixaEmissao)
                            {
                                double tolerancia = double.Parse(faixaEmissao.Quantidade.ToString()) * 1.1; //Acréscimo de 10%
                                if (countTotal <= tolerancia)
                                {
                                    valor = faixaEmissao.Valor;
                                    break;
                                }
                            }

                            string descricao = empresa.PlanoEmissaoCTe.Descricao + ": " + countCTes.ToString() + " CTe(s) e " + countMDFes.ToString() + " MDFe(s) ref. ao período " + dataInicial.ToString("dd/MM/yyyy") + " até " + dataFinal.ToString("dd/MM/yyyy") + ".";

                            if (valor == 0)
                                descricao = empresa.PlanoEmissaoCTe.Descricao + ": sem faixa cadastrada para a quantidade de documentos emitidos. " + countCTes.ToString() + " CTe(s) e " + countMDFes.ToString() + " MDFe(s) ref. ao período " + dataInicial.ToString("dd/MM/yyyy") + " até " + dataFinal.ToString("dd/MM/yyyy") + ".";

                            if (!string.IsNullOrWhiteSpace(empresasAssociadas))
                            {
                                empresasAssociadas = empresa.CNPJ + " / " + empresasAssociadas;
                                descricao = descricao + " Referente aos CNPJs " + empresasAssociadas + ".";
                            }

                            arquivo.Append(string.Concat(empresa.CNPJ, ";",
                                                         valor.ToString("n2"), ";",
                                                        descricao));
                            arquivo.AppendLine();
                        }
                    }
                }

                memoStream.Write(System.Text.Encoding.Default.GetBytes(arquivo.ToString()), 0, arquivo.ToString().Length);

                memoStream.Position = 0;
            }

            return memoStream;
        }

    }
}