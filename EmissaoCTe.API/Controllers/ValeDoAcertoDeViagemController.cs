using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ValeDoAcertoDeViagemController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorAcertoDeViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoAcertoViagem = 0;
                int.TryParse(Request.Params["CodigoAcertoViagem"], out codigoAcertoViagem);

                Repositorio.ValeDoAcertoDeViagem repValeAcertoViagem = new Repositorio.ValeDoAcertoDeViagem(unitOfWork);
                List<Dominio.Entidades.ValeDoAcertoDeViagem> listaVales = repValeAcertoViagem.BuscarPorAcertoDeViagem(codigoAcertoViagem);

                var retorno = from obj in listaVales
                              select new Dominio.ObjetosDeValor.ValeAcertoDeViagem
                              {
                                  Codigo = obj.Codigo,
                                  Numero = obj.Numero,
                                  Data = obj.Data.ToString("dd/MM/yyyy"),
                                  Descricao = obj.Descricao,
                                  Tipo = obj.Tipo,
                                  DescricaoTipo = obj.DescricaoTipo,
                                  Excluir = false,
                                  Observacao = obj.Observacao,
                                  Valor = obj.Valor.ToString("n2")
                              };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os vales do acerto de viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownlaodRebibo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);
                Repositorio.ValeDoAcertoDeViagem repValeAcertoViagem = new Repositorio.ValeDoAcertoDeViagem(unitOfWork);

                int.TryParse(Request.Params["Vale"], out int codigoVale);
                int.TryParse(Request.Params["AcertoViagem"], out int codigoAcertoViagem);

                Dominio.Entidades.ValeDoAcertoDeViagem vale = repValeAcertoViagem.BuscarPorCodigoEAcertoDeViagem(codigoVale, codigoAcertoViagem);

                if (vale == null)
                    return Json<bool>(false, false, "Não foi possível encontrar o registro.");

                Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoValeDevolucaoAcertoCabecalho recibo = new Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoValeDevolucaoAcertoCabecalho
                {
                    EmpresaCEP = this.EmpresaUsuario.CEP,
                    EmpresaCidade = this.EmpresaUsuario.Localidade.Descricao,
                    EmpresaCNPJ = this.EmpresaUsuario.CNPJ,
                    EmpresaEndereco = this.EmpresaUsuario.Endereco,
                    EmpresaFone = this.EmpresaUsuario.Telefone,
                    EmpresaIE = this.EmpresaUsuario.InscricaoEstadual,
                    EmpresaNome = this.EmpresaUsuario.RazaoSocial,
                    EmpresaNumero = this.EmpresaUsuario.Numero,
                    EmpresaUF = this.EmpresaUsuario.Localidade.Estado.Sigla,
                    EmpresaBairro = this.EmpresaUsuario.Bairro,

                    MotoristaCPF = vale.AcertoDeViagem.Motorista.CPF_Formatado,
                    MotoristaNome = vale.AcertoDeViagem.Motorista.Nome,
                    MotoristaRG = vale.AcertoDeViagem.Motorista.RG,
                    MotoristaCidade = vale.AcertoDeViagem.Motorista.Localidade.DescricaoCidadeEstado,
                };

                Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoValeDevolucaoAcerto informacoes = new Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoValeDevolucaoAcerto() {
                    NumeroAcerto = vale.AcertoDeViagem.Numero,
                    NumeroDocumento = vale.Numero,
                    Data = vale.Data,
                    Valor = vale.Valor
                };

                bool tipoDevolucao = vale.Tipo == Dominio.Enumeradores.TipoValeAcertoViagem.Devolucao;
                string valorExtenso = Utilidades.Conversor.DecimalToWords(vale.Valor);
                string descricaoTipo = vale.Tipo == Dominio.Enumeradores.TipoValeAcertoViagem.Vale ? "Adiantamento" : vale.DescricaoTipo;
                string tipoValeDevolucao = descricaoTipo.ToUpper();
                string observacaoRecibo = "Recebi(emos) " + (tipoDevolucao ? "do Motorista Acima Idenficado" : "da Empresa Acima Identificada") + ", Referente Pagamento de " + descricaoTipo.ToLower() + " a Importância de " + valorExtenso;
                if (!string.IsNullOrWhiteSpace(vale.Observacao))
                    observacaoRecibo = string.Concat(vale.Observacao, " / ", observacaoRecibo);
                string assinaturaDescricao = "Assinatura " + (tipoDevolucao ? "da Empresa" : "do Motorista");

                List<ReportDataSource> dataSources = new List<ReportDataSource>
                {
                    new ReportDataSource("Recibo", new List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoValeDevolucaoAcertoCabecalho>() { recibo }),
                    new ReportDataSource("Informacoes", new List<Dominio.ObjetosDeValor.Relatorios.ReciboPagamentoValeDevolucaoAcerto>() { informacoes })
                };

                List<ReportParameter> parametros = new List<ReportParameter>()
                {
                    new ReportParameter("Titulo", tipoValeDevolucao),
                    new ReportParameter("DescricaoTipo", descricaoTipo),
                    new ReportParameter("AssinaturaDescricao", assinaturaDescricao),
                    new ReportParameter("ObservacaoRecibo", observacaoRecibo),
                };

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/ReciboPagamentoValeDevolucaoAcerto.rdlc", "PDF", parametros, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "Recibo-" + descricaoTipo + "." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o recibo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
