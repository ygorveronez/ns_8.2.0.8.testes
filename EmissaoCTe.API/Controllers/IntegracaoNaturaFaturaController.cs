using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoNaturaFaturaController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult ConsultarDocumentosVinculados()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoFatura, tipoDocumento, numero;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoFatura"]), out codigoFatura);
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Tipo"]), out tipoDocumento);
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Numero"]), out numero);

                Repositorio.ItemFaturaNatura repItemFaturaNatura = new Repositorio.ItemFaturaNatura(unidadeDeTrabalho);

                List<Dominio.Entidades.ItemFaturaNatura> listaItemFaturaNatura = repItemFaturaNatura.BuscarDocumentosFatura(codigoFatura, tipoDocumento, numero);

                int countDocumentos = listaItemFaturaNatura.Count();

                var retorno = (from itemFatura in listaItemFaturaNatura
                               select new
                               {
                                   itemFatura.Codigo,
                                   Tipo = itemFatura.NotaFiscal.CTe != null ? "CT-e" : itemFatura.NotaFiscal.NFSe != null ? "NFS-e" : string.Empty,
                                   Numero = itemFatura.NotaFiscal.CTe != null ? itemFatura.NotaFiscal.CTe.Numero : itemFatura.NotaFiscal.NFSe != null ? itemFatura.NotaFiscal.NFSe.Numero : 0,
                                   ValorReceber = itemFatura.NotaFiscal.CTe != null ? itemFatura.NotaFiscal.CTe.ValorAReceber : itemFatura.NotaFiscal.NFSe != null ? itemFatura.NotaFiscal.NFSe.ValorServicos : 0
                               }).OrderBy(o => o.Tipo).ThenBy(o => o.Numero).ToList();


                return Json(retorno, true, null, new string[] { "Codigo", "Tipo|10", "Número|40", "Valor Receber|20" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar documentos da Fatura Natura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult RemoverDocumentoFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Codigo"]), out codigo);

                Repositorio.ItemFaturaNatura repItemFaturaNatura = new Repositorio.ItemFaturaNatura(unidadeDeTrabalho);

                Dominio.Entidades.ItemFaturaNatura itemFaturaNatura = repItemFaturaNatura.BuscarPorCodigo(codigo);

                if (itemFaturaNatura != null)
                {
                    repItemFaturaNatura.Deletar(itemFaturaNatura);
                    return Json<bool>(true, true);
                }
                else
                    return Json<bool>(false, false, "Documento não encontrado para exclusão.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao excluir documento da Fatura Natura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarIntegracoesNatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoFatura;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoFatura"]), out codigoFatura);

                Repositorio.FaturaNatura repFaturaNatura = new Repositorio.FaturaNatura(unidadeDeTrabalho);
                Dominio.Entidades.FaturaNatura fatura = repFaturaNatura.BuscarPorCodigo(codigoFatura);

                if (fatura != null && fatura.NaturaXMLs != null && fatura.NaturaXMLs.Count > 0)
                {
                    List<Dominio.Entidades.NaturaXML> listaNaturaXML = new List<Dominio.Entidades.NaturaXML>();
                    foreach (Dominio.Entidades.NaturaXML xml in fatura.NaturaXMLs)
                    {
                        if (xml != null)
                        {
                            if (xml.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaPreFatura || xml.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.EnvioFatura)
                                listaNaturaXML.Add(xml);
                        }
                    }

                    int countDocumentos = listaNaturaXML.Count();

                    var retorno = (from naturaXML in listaNaturaXML
                                   select new
                                   {
                                       Codigo = naturaXML.Codigo,
                                       Tipo = naturaXML.Tipo,
                                       Data = naturaXML.Data.ToString("dd/MM/yyyy HH:mm"),
                                       naturaXML.Usuario.Nome,
                                       naturaXML.DescricaoTipo,
                                       Mensagem = naturaXML.Mensagem ?? string.Empty
                                   }).OrderByDescending(o => o.Codigo).ToList();

                    return Json(retorno, true, null, new string[] { "Codigo", "Tipo", "Data|15", "Usuario|20", "Tipo|20", "Mensagem|25" }, countDocumentos);
                }
                else
                {
                    return Json(string.Empty.ToList(), true, null, new string[] { "Codigo", "Tipo", "Data|15", "Usuario|20", "Tipo|20", "Mensagem|25" }, 0);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar integrações para a Fatura Natura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                long numeroFatura, numeroPreFatura;
                long.TryParse(Utilidades.String.OnlyNumbers(Request.Params["NumeroFatura"]), out numeroFatura);
                long.TryParse(Utilidades.String.OnlyNumbers(Request.Params["NumeroPreFatura"]), out numeroPreFatura);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int inicioRegistros;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.FaturaNatura repFatura = new Repositorio.FaturaNatura(unitOfWork);

                List<Dominio.Entidades.FaturaNatura> faturas = repFatura.Consultar(this.EmpresaUsuario.Codigo, numeroFatura, numeroPreFatura, dataInicial, dataFinal, inicioRegistros, 50);

                int countDocumentos = repFatura.ContarConsulta(this.EmpresaUsuario.Codigo, numeroFatura, numeroPreFatura, dataInicial, dataFinal);

                var retorno = (from obj in faturas
                               select new
                               {
                                   obj.Codigo,
                                   Sacado = obj.Sacado != null ? obj.Sacado.CPF_CNPJ.ToString() : string.Empty,
                                   obj.Numero,
                                   obj.NumeroPreFatura,
                                   DataPreFatura = obj.DataPreFatura.ToString("dd/MM/yyyy"),
                                   DataEmissao = obj.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                   DataVencimento = obj.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                                   Valor = obj.ValorFrete.ToString("n2"),
                                   Desconto = obj.Itens.Sum(o => o.ValorDoDesconto).ToString("n2"),
                                   ValorCTes = (obj.Itens.Where(o => o.NotaFiscal.CTe != null && o.NotaFiscal.CTe.Status.Equals("A")).Sum(o => o.NotaFiscal.CTe.ValorAReceber) + obj.Itens.Where(o => o.NotaFiscal.NFSe != null && o.NotaFiscal.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado).Sum(o => o.NotaFiscal.NFSe.ValorServicos)).ToString("n2"),
                                   Documentos = obj.Itens.Where(o => o.NotaFiscal.CTe != null && o.NotaFiscal.CTe.Status.Equals("A")).Count() + obj.Itens.Where(o => o.NotaFiscal.NFSe != null && o.NotaFiscal.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado).Count(),
                                   Status = obj.DescricaoStatus
                               }).ToList();

                return Json(retorno, true, "", new string[] { "Código", "Sacado", "Nº Fat.|8", "Nº Pré Fat.|12", "Data Pré Fat.|10", "Data Emissão|10", "Data Vcto|10", "Valor Pré Fat.|10", "Valor Desc.|8", "Valor a Receber|11", "Docs.|5", "Status|8" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os documentos de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarFaturas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                long numeroPreFatura;
                long.TryParse(Utilidades.String.OnlyNumbers(Request.Params["NumeroPreFatura"]), out numeroPreFatura);

                bool atualizarPreFatura = false;
                bool.TryParse(Request.Params["AtualizarPreFatura"], out atualizarPreFatura);

                Servicos.Natura svcNatura = new Servicos.Natura(unidadeDeTrabalho);

                svcNatura.ConsultarPreFaturas(this.EmpresaUsuario.Codigo, numeroPreFatura, dataInicial, dataFinal, atualizarPreFatura, this.UsuarioAdministrativo != null ? this.UsuarioAdministrativo.Codigo : this.Usuario.Codigo, unidadeDeTrabalho);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os documentos de transporte.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EmitirFatura()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoFatura;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                DateTime dataVencimento, dataEmissao;
                DateTime.TryParseExact(Request.Params["DataVencimento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento);
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                double cnpjSacado = 0;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Sacado"]), out cnpjSacado);                              

                Repositorio.FaturaNatura repFatura = new Repositorio.FaturaNatura(unidadeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

                Dominio.Entidades.FaturaNatura fatura = repFatura.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoFatura);
                Dominio.Entidades.Cliente sacado = cnpjSacado <= 0 ? null : repCliente.BuscarPorCPFCNPJ(cnpjSacado);

                if (fatura == null)
                    return Json<bool>(false, false, "Fatura não encontrada.");

                if (fatura.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Emitida && fatura.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Pendente)
                    return Json<bool>(false, false, "O status da fatura não permite a emissão da mesma.");

                if (!VerificaCTesAutorizadosFatura(fatura, unidadeTrabalho))
                    return Json<bool>(false, false, "Não existem CTe(s) Autorizados associados a esta fatura.");

                fatura.DataEmissao = dataEmissao;
                fatura.DataVencimento = dataVencimento;
                fatura.Sacado = sacado;

                repFatura.Atualizar(fatura);

                Servicos.Natura svcNatura = new Servicos.Natura(unidadeTrabalho);

                svcNatura.EmitirFatura(fatura.Codigo, unidadeTrabalho, this.UsuarioAdministrativo != null ? this.UsuarioAdministrativo.Codigo : this.Usuario.Codigo);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir a fatura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult QuitarFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoFatura;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                Repositorio.FaturaNatura repFatura = new Repositorio.FaturaNatura(unidadeDeTrabalho);
                Repositorio.MovimentoDoFinanceiro repMovimentoFinanceiro = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                Repositorio.ParcelaCobrancaCTe repParcelaCTe = new Repositorio.ParcelaCobrancaCTe(unidadeDeTrabalho);

                Dominio.Entidades.FaturaNatura fatura = repFatura.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoFatura);

                if (fatura == null)
                    return Json<bool>(false, false, "Fatura não encontrada.");

                if (fatura.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Emitida)
                    return Json<bool>(false, false, "O status da fatura não permite que a mesma seja quitada.");

                Repositorio.ItemFaturaNatura repItemFatura = new Repositorio.ItemFaturaNatura(unidadeDeTrabalho);

                List<Dominio.Entidades.ItemFaturaNatura> itensFatura = repItemFatura.BuscarPorFatura(fatura.Codigo);

                unidadeDeTrabalho.Start();

                fatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Paga;

                repFatura.Atualizar(fatura);

                foreach (Dominio.Entidades.ItemFaturaNatura item in itensFatura)
                {
                    List<Dominio.Entidades.ParcelaCobrancaCTe> parcelas = repParcelaCTe.BuscarPorCTe(item.NotaFiscal.CTe.Empresa.Codigo, item.NotaFiscal.CTe.Codigo);

                    foreach (Dominio.Entidades.ParcelaCobrancaCTe parcela in parcelas)
                    {
                        parcela.DataPagamento = DateTime.Now;

                        repParcelaCTe.Atualizar(parcela);

                        Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimentoFinanceiro.BuscarPorParcelaCobrancaCTe(item.NotaFiscal.CTe.Empresa.Codigo, parcela.Codigo);

                        if (movimento != null)
                        {
                            movimento.DataBaixa = DateTime.Now;
                            movimento.DataPagamento = DateTime.Now;

                            repMovimentoFinanceiro.Atualizar(movimento);
                        }
                    }
                }

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir a fatura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult CancelarFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoFatura;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                Repositorio.FaturaNatura repFatura = new Repositorio.FaturaNatura(unidadeDeTrabalho);

                Dominio.Entidades.FaturaNatura fatura = repFatura.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoFatura);

                if (fatura == null)
                    return Json<bool>(false, false, "Fatura não encontrada.");

                if (fatura.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Emitida && fatura.Status != Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Pendente)
                    return Json<bool>(false, false, "O status da fatura não permite que a mesma seja cancelada.");

                fatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Cancelada;

                repFatura.Atualizar(fatura);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao cancelar a fatura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadDetalhesFatura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoFatura;
                int.TryParse(Request.Params["CodigoFatura"], out codigoFatura);

                string tipo = Request.Params["Tipo"];

                Repositorio.FaturaNatura repFatura = new Repositorio.FaturaNatura(unidadeDeTrabalho);

                Dominio.Entidades.FaturaNatura fatura = repFatura.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoFatura);

                if (fatura == null)
                    return Json<bool>(false, false, "Fatura não encontrada.");

                Repositorio.ItemFaturaNatura repItemFatura = new Repositorio.ItemFaturaNatura(unidadeDeTrabalho);

                List<Dominio.Entidades.ItemFaturaNatura> itensFatura = repItemFatura.BuscarPorFatura(fatura.Codigo);

                List<Dominio.ObjetosDeValor.Relatorios.DetalheFaturaNatura> detalhes = new List<Dominio.ObjetosDeValor.Relatorios.DetalheFaturaNatura>()
                {
                    new Dominio.ObjetosDeValor.Relatorios.DetalheFaturaNatura()
                    {
                         BairroEmpresa = fatura.Empresa.Bairro,
                         CEPEmpresa = fatura.Empresa.CEP,
                         CidadeEmpresa = fatura.Empresa.Localidade.DescricaoCidadeEstado,
                         CNPJEmpresa = fatura.Empresa.CNPJ_Formatado,
                         DataVencimento = fatura.DataVencimento.HasValue ? fatura.DataVencimento.Value : DateTime.Now,
                         EnderecoEmpresa = fatura.Empresa.Endereco,
                         IEEmpresa = fatura.Empresa.InscricaoEstadual,
                         Logo = Servicos.Imagem.GetFromPath(fatura.Empresa.CaminhoLogoDacte, System.Drawing.Imaging.ImageFormat.Bmp),
                         NomeEmpresa = fatura.Empresa.RazaoSocial,
                         Numero = fatura.Numero,
                         NumeroEmpresa = fatura.Empresa.Numero,
                         NumeroPreFatura = fatura.NumeroPreFatura,
                         Telefone = fatura.Empresa.Telefone,
                         ValorTotal = fatura.ValorFrete,
                         ValorDesconto = (from obj in itensFatura select (decimal?)obj.ValorDoDesconto).Sum() ?? 0m,
                         // ValorTotalPorExtenso = Utilidades.Conversor.DecimalToWords(fatura.ValorFrete - ((from obj in itensFatura select (decimal?)obj.ValorDoDesconto).Sum() ?? 0m) )
                         ValorTotalPorExtenso = Utilidades.Conversor.DecimalToWords( (from obj in itensFatura select (decimal?)obj.NotaFiscal.CTe?.ValorAReceber).Sum() ?? 0m + (from obj in fatura.Itens where obj.Fatura.Codigo == fatura.Codigo select (decimal?)obj.NotaFiscal.NFSe?.ValorServicos).Sum() ?? 0m - (from obj in itensFatura select (decimal?)obj.ValorDoDesconto).Sum() ?? 0m),
                         Sacado = fatura.Sacado != null ? fatura.Sacado.CPF_CNPJ_Formatado + " - " + fatura.Sacado.Nome : string.Empty
                    }
                };

                List<Dominio.ObjetosDeValor.Relatorios.DetalheFaturaNaturaCTe> ctes = (from obj in itensFatura
                                                                                       where (obj.NotaFiscal.CTe != null && obj.NotaFiscal.CTe.Status.Equals("A")) ||
                                                                                       (obj.NotaFiscal.NFSe != null && obj.NotaFiscal.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                                                                                       select new Dominio.ObjetosDeValor.Relatorios.DetalheFaturaNaturaCTe()
                                                                                       {
                                                                                           DataEmissao = obj.NotaFiscal.CTe?.DataEmissao.Value ?? obj.NotaFiscal.NFSe.DataEmissao,
                                                                                           Destinatario = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Destinatario.CPF_CNPJ_Formatado + " - " + obj.NotaFiscal.CTe.Destinatario.Nome : string.Empty,
                                                                                           NumeroCTe = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Numero : obj.NotaFiscal.NFSe.Numero,
                                                                                           Remetente = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Remetente.CPF_CNPJ_Formatado + " - " + obj.NotaFiscal.CTe.Remetente.Nome : obj.NotaFiscal.NFSe.Tomador.CPF_CNPJ_Formatado + " - " + obj.NotaFiscal.NFSe.Tomador.Nome,
                                                                                           SerieCTe = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Serie.Numero : obj.NotaFiscal.NFSe.Serie.Numero,
                                                                                           Desconto = obj.ValorDoDesconto,
                                                                                           ValorFrete = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.ValorFrete : obj.NotaFiscal.NFSe.ValorServicos,
                                                                                           ValorReceber = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.ValorAReceber : obj.NotaFiscal.NFSe.ValorServicos
                                                                                       }).OrderBy(o => o.NumeroCTe).ToList();

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeDeTrabalho);

                List<Microsoft.Reporting.WebForms.ReportDataSource> dataSources = new List<Microsoft.Reporting.WebForms.ReportDataSource>();

                dataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Fatura", detalhes));
                dataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("CTes", ctes));

                Dominio.ObjetosDeValor.Relatorios.Relatorio relatorio = svcRelatorio.GerarWeb("Relatorios/DetalheFaturaNatura.rdlc", tipo, null, dataSources, null);

                return Arquivo(relatorio.Arquivo, relatorio.MimeType, "Fatura_" + fatura.Numero.ToString() + "." + relatorio.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar os detalhes da fatura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                string tipo = Request.Params["Tipo"];

                if (codigo > 0)
                {
                    Repositorio.DocumentoTransporteNatura repDT = new Repositorio.DocumentoTransporteNatura(unidadeDeTrabalho);
                    Repositorio.NaturaXML repNaturaXML = new Repositorio.NaturaXML(unidadeDeTrabalho);

                    Dominio.Entidades.NaturaXML naturaXML = repNaturaXML.BuscaPorCodigo(codigo);

                    if (naturaXML != null)
                    {
                        if (tipo == "R")
                        {
                            if (naturaXML.XMLRetorno != null && naturaXML.XMLRetorno != "")
                            {
                                byte[] data = System.Text.Encoding.UTF8.GetBytes(naturaXML.XMLRetorno);

                                if (data != null)
                                {
                                    if (naturaXML.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaPreFatura)
                                        return Arquivo(data, "text/xml", string.Concat("RetornoPreFatura.xml"));
                                    else
                                        return Arquivo(data, "text/xml", string.Concat("RetornoEnvioFatura.xml"));
                                }
                                else
                                    return Json<bool>(false, false, "Integração não possui XML salvo.");
                            }
                            else
                                return Json<bool>(false, false, "Integração não possui XML salvo.");
                        }
                        else
                        {
                            byte[] data = System.Text.Encoding.UTF8.GetBytes(naturaXML.XMLEnvio);

                            if (data != null)
                            {
                                if (naturaXML.Tipo == Dominio.ObjetosDeValor.Enumerador.TipoXMLNatura.ConsultaPreFatura)
                                    return Arquivo(data, "text/xml", string.Concat("ConsultaPreFatura.xml"));
                                else
                                    return Arquivo(data, "text/xml", string.Concat("EnvioFatura.xml"));
                            }
                            else
                                return Json<bool>(false, false, "Integração não possui XML salvo.");
                        }
                    }
                    else
                        return Json<bool>(false, false, "Integração não encontrada, atualize a página e tente novamente.");
                }
                else
                    return Json<bool>(false, false, "Integração não encontrada, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        private bool VerificaCTesAutorizadosFatura(Dominio.Entidades.FaturaNatura fatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.ItemFaturaNatura repItemFatura = new Repositorio.ItemFaturaNatura(unidadeTrabalho);

            List<Dominio.Entidades.ItemFaturaNatura> itensFatura = repItemFatura.BuscarPorFatura(fatura.Codigo);

            List<Dominio.ObjetosDeValor.Relatorios.DetalheFaturaNaturaCTe> ctes = (from obj in itensFatura
                                                                                   where (obj.NotaFiscal.CTe != null && obj.NotaFiscal.CTe.Status.Equals("A")) ||
                                                                                         (obj.NotaFiscal.NFSe != null && obj.NotaFiscal.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                                                                                   select new Dominio.ObjetosDeValor.Relatorios.DetalheFaturaNaturaCTe()
                                                                                   {
                                                                                       DataEmissao = obj.NotaFiscal.CTe?.DataEmissao.Value ?? obj.NotaFiscal.NFSe.DataEmissao,
                                                                                       Destinatario = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Destinatario.CPF_CNPJ_Formatado + " - " + obj.NotaFiscal.CTe.Destinatario.Nome : string.Empty,
                                                                                       NumeroCTe = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Numero : obj.NotaFiscal.NFSe.Numero,
                                                                                       Remetente = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Remetente.CPF_CNPJ_Formatado + " - " + obj.NotaFiscal.CTe.Remetente.Nome : obj.NotaFiscal.NFSe.Tomador.CPF_CNPJ_Formatado + " - " + obj.NotaFiscal.NFSe.Tomador.Nome,
                                                                                       SerieCTe = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.Serie.Numero : obj.NotaFiscal.NFSe.Serie.Numero,
                                                                                       Desconto = obj.ValorDoDesconto,
                                                                                       ValorFrete = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.ValorFrete : obj.NotaFiscal.NFSe.ValorServicos,
                                                                                       ValorReceber = obj.NotaFiscal.CTe != null ? obj.NotaFiscal.CTe.ValorAReceber : obj.NotaFiscal.NFSe.ValorServicos
                                                                                   }).OrderBy(o => o.NumeroCTe).ToList();

            if (ctes == null || ctes.Count() == 0)
                return false;
            else
                return true;
        }
    }
}