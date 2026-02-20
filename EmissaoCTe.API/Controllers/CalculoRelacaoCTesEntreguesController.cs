using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class CalculoRelacaoCTesEntreguesController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("calculorelacaoctesentregues.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.CalculoRelacaoCTesEntregues repCalculoRelacaoCTesEntregues = new Repositorio.CalculoRelacaoCTesEntregues(unitOfWork);

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                double.TryParse(Request.Params["Cliente"], out double cpfcnpjCliente);

                List<Dominio.Entidades.CalculoRelacaoCTesEntregues> calculoRelacaoCTesEntregues = repCalculoRelacaoCTesEntregues.Consultar(this.EmpresaUsuario.Codigo, cpfcnpjCliente, inicioRegistros, 50);
                int countLista = repCalculoRelacaoCTesEntregues.ContarConsulta(this.EmpresaUsuario.Codigo, cpfcnpjCliente);

                var retorno = (from obj in calculoRelacaoCTesEntregues
                               select new
                               {
                                   obj.Codigo,
                                   //Transportador = obj.Emissor != null ? obj.Emissor.CPF_CNPJ_Formatado + " " + obj.Emissor.Nome : string.Empty,
                                   Cliente = obj.Cliente != null ? obj.Cliente.CPF_CNPJ_Formatado + " " + obj.Cliente.Nome : obj.Descricao
                               }).ToList();

                unitOfWork.Dispose();

                return Json(retorno, true, null, new string[] { "Codigo", "Cliente|80" }, countLista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as PROPOSTAS.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesPorCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                double.TryParse(Request.Params["codigo"], out double codigo);
                double emissor = 0;
                //double.TryParse(Request.Params["emissor"], out emissor);

                Repositorio.CalculoRelacaoCTesEntregues repCalculoRelacaoCTesEntregues = new Repositorio.CalculoRelacaoCTesEntregues(unitOfWork);

                Dominio.Entidades.CalculoRelacaoCTesEntregues calculoRelacaoCTesEntregue = repCalculoRelacaoCTesEntregues.BuscarPorEmpresaECliente(this.EmpresaUsuario.Codigo, codigo, emissor);

                if (calculoRelacaoCTesEntregue == null)
                    return Json<dynamic>(null, true);

                var retorno = calculoRelacaoCTesEntregue.ObjetoCalculo();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params["Codigo"], out int codigo);

                Repositorio.CalculoRelacaoCTesEntregues repCalculoRelacaoCTesEntregues = new Repositorio.CalculoRelacaoCTesEntregues(unitOfWork);

                Dominio.Entidades.CalculoRelacaoCTesEntregues calculoRelacaoCTesEntregue = repCalculoRelacaoCTesEntregues.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (calculoRelacaoCTesEntregue == null)
                    return Json<bool>(false, false, "Não foi possível obter detalhes.");

                var retorno = new
                {
                    Codigo = calculoRelacaoCTesEntregue.Codigo,
                    Emissor = calculoRelacaoCTesEntregue.Emissor != null ? new { calculoRelacaoCTesEntregue.Emissor.Codigo, calculoRelacaoCTesEntregue.Emissor.Descricao } : null,
                    Cliente = new { calculoRelacaoCTesEntregue.Cliente.Codigo, calculoRelacaoCTesEntregue.Cliente.Descricao },
                    ValorDiaria = calculoRelacaoCTesEntregue.ValorDiaria.ToString("n2"),
                    ValorMeiaDiaria = calculoRelacaoCTesEntregue.ValorMeiaDiaria.ToString("n2"),
                    PercentualPorCTe = calculoRelacaoCTesEntregue.PercentualPorCTe.ToString("n2"),
                    ValorMinimoPorCTe = calculoRelacaoCTesEntregue.ValorMinimoPorCTe.ToString("n2"),
                    ValorMinimoCTeMesmoDestino = calculoRelacaoCTesEntregue.ValorMinimoCTeMesmoDestino.ToString("n2"),
                    FracaoKG = calculoRelacaoCTesEntregue.FracaoKG.ToString("n4"),
                    ValorPorFracao = calculoRelacaoCTesEntregue.ValorPorFracao.ToString("n2"),
                    ValorPorFracaoEmEntregasIguais = calculoRelacaoCTesEntregue.ValorPorFracaoEmEntregasIguais.ToString("n2"),
                    ValorKMExcedente = calculoRelacaoCTesEntregue.ValorKMExcedente.ToString("n2"),
                    ColetaValorPorEvento = calculoRelacaoCTesEntregue.ColetaValorPorEvento.ToString("n2"),
                    ColetaFracao = calculoRelacaoCTesEntregue.ColetaFracao.ToString("n4"),
                    ColetaValorPorFracao = calculoRelacaoCTesEntregue.ColetaValorPorFracao.ToString("n2"),
                    calculoRelacaoCTesEntregue.FranquiaKM,

                    Cidades = (from obj in calculoRelacaoCTesEntregue.PercentualCidades
                               select new
                               {
                                   Id = obj.Codigo,
                                   Cidade = obj.Cidade.Codigo,
                                   CidadeDescricao = obj.Cidade.Descricao,
                                   Estado = obj.Cidade.Estado.Sigla,
                                   Percentual = obj.Percentual,
                                   Excluir = false
                               }).ToList()
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPermissaoUsuario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeDelecao != "A")
                {
                    var retorno = new
                    {
                        Permissao = false
                    };
                    return Json(retorno, true);
                }
                else
                {
                    var retorno = new
                    {
                        Permissao = true
                    };
                    return Json(retorno, true);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [ValidateInput(false)]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.CalculoRelacaoCTesEntregues repCalculoRelacaoCTesEntregues = new Repositorio.CalculoRelacaoCTesEntregues(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.CalculoRelacaoCTesEntregues calculoRelacaoCTesEntregue = repCalculoRelacaoCTesEntregues.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (calculoRelacaoCTesEntregue != null)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada!");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada!");

                    calculoRelacaoCTesEntregue = new Dominio.Entidades.CalculoRelacaoCTesEntregues()
                    {
                        Empresa = this.EmpresaUsuario
                    };
                }

                decimal.TryParse(Request.Params["ValorDiaria"], out decimal valorDiaria);
                decimal.TryParse(Request.Params["ValorMeiaDiaria"], out decimal valorMeiaDiaria);
                decimal.TryParse(Request.Params["PercentualPorCTe"], out decimal percentualPorCTe);
                decimal.TryParse(Request.Params["ValorMinimoPorCTe"], out decimal valorMinimoPorCTe);
                decimal.TryParse(Request.Params["ValorMinimoCTeMesmoDestino"], out decimal valorMinimoCTeMesmoDestino);
                decimal.TryParse(Request.Params["FracaoKG"], out decimal fracaoKG);
                decimal.TryParse(Request.Params["ValorPorFracao"], out decimal valorPorFracao);
                decimal.TryParse(Request.Params["ValorPorFracaoEmEntregasIguais"], out decimal valorPorFracaoEmEntregasIguais);
                decimal.TryParse(Request.Params["ValorKMExcedente"], out decimal valorKMExcedente);
                decimal.TryParse(Request.Params["ColetaValorPorEvento"], out decimal coletaValorPorEvento);
                decimal.TryParse(Request.Params["ColetaFracao"], out decimal coletaFracao);
                decimal.TryParse(Request.Params["ColetaValorPorFracao"], out decimal coletaValorPorFracao);

                int.TryParse(Request.Params["FranquiaKM"], out int franquiaKM);

                double.TryParse(Request.Params["Cliente"], out double cliente);
                double.TryParse(Request.Params["Emissor"], out double emissor);

                calculoRelacaoCTesEntregue.ValorDiaria = valorDiaria;
                calculoRelacaoCTesEntregue.ValorMeiaDiaria = valorMeiaDiaria;
                calculoRelacaoCTesEntregue.PercentualPorCTe = percentualPorCTe;
                calculoRelacaoCTesEntregue.ValorMinimoPorCTe = valorMinimoPorCTe;
                calculoRelacaoCTesEntregue.ValorMinimoCTeMesmoDestino = valorMinimoCTeMesmoDestino;
                calculoRelacaoCTesEntregue.FracaoKG = fracaoKG;
                calculoRelacaoCTesEntregue.ValorPorFracao = valorPorFracao;
                calculoRelacaoCTesEntregue.ValorPorFracaoEmEntregasIguais = valorPorFracaoEmEntregasIguais;
                calculoRelacaoCTesEntregue.FranquiaKM = franquiaKM;
                calculoRelacaoCTesEntregue.ValorKMExcedente = valorKMExcedente;
                calculoRelacaoCTesEntregue.ColetaValorPorEvento = coletaValorPorEvento;
                calculoRelacaoCTesEntregue.ColetaFracao = coletaFracao;
                calculoRelacaoCTesEntregue.ColetaValorPorFracao = coletaValorPorFracao;
                calculoRelacaoCTesEntregue.Cliente = repCliente.BuscarPorCPFCNPJ(cliente);
                if (emissor > 0)
                    calculoRelacaoCTesEntregue.Emissor = repCliente.BuscarPorCPFCNPJ(emissor);

                // Valida
                if (calculoRelacaoCTesEntregue.Cliente == null)
                    return Json<bool>(false, false, "Cliente é obrigatório.");

                Dominio.Entidades.CalculoRelacaoCTesEntregues duplicidade = repCalculoRelacaoCTesEntregues.BuscarPorEmpresaECliente(this.EmpresaUsuario.Codigo, cliente, emissor);
                if (duplicidade != null && duplicidade.Codigo != calculoRelacaoCTesEntregue.Codigo)
                    return Json<bool>(false, false, "Já existe uma configuração para o cliente " + calculoRelacaoCTesEntregue.Cliente.Descricao + ".");

                unidadeDeTrabalho.Start();
                if (calculoRelacaoCTesEntregue.Codigo > 0)
                    repCalculoRelacaoCTesEntregues.Atualizar(calculoRelacaoCTesEntregue);
                else
                    repCalculoRelacaoCTesEntregues.Inserir(calculoRelacaoCTesEntregue);

                this.SalvarCidades(calculoRelacaoCTesEntregue, unidadeDeTrabalho);
                unidadeDeTrabalho.CommitChanges();

                return Json(new
                {
                    Codigo = calculoRelacaoCTesEntregue.Codigo
                }, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os dados.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        //[AcceptVerbs("POST", "GET")]
        //public ActionResult Visualizar()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
        //    try
        //    {
        //        int codigo = 0;
        //        int.TryParse(Request.Params["Codigo"], out codigo);

        //        Repositorio.CalculoRelacaoCTesEntregues repCalculoRelacaoCTesEntregues = new Repositorio.CalculoRelacaoCTesEntregues(unitOfWork);
        //        Repositorio.CalculoRelacaoCTesEntreguesItem repCalculoRelacaoCTesEntreguesItem = new Repositorio.CalculoRelacaoCTesEntreguesItem(unitOfWork);

        //        Dominio.Entidades.CalculoRelacaoCTesEntregues propostas = repCalculoRelacaoCTesEntregues.BuscaPorCodigo(this.EmpresaUsuario.Codigo, codigo);
        //        List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoCalculoRelacaoCTesEntregues> listaCalculoRelacaoCTesEntreguess = repCalculoRelacaoCTesEntregues.RelatorioPorCodigo(this.EmpresaUsuario.Codigo, codigo);
        //        List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoTextosCalculoRelacaoCTesEntreguess> listaTextosCalculoRelacaoCTesEntreguess = repCalculoRelacaoCTesEntregues.TextoRelatorioPorCodigo(this.EmpresaUsuario.Codigo, codigo);
        //        List<Dominio.ObjetosDeValor.Relatorios.VisualizacaoCalculoRelacaoCTesEntreguesItem> listaItens = repCalculoRelacaoCTesEntreguesItem.RelatorioPorCodigo(codigo);

        //        List<ReportParameter> parametros = new List<ReportParameter>();
        //        parametros.Add(new ReportParameter("Numero", propostas.Numero.ToString()));
        //        parametros.Add(new ReportParameter("Logo", this.EmpresaUsuario.CaminhoLogoDacte));

        //        List<ReportDataSource> dataSources = new List<ReportDataSource>();
        //        dataSources.Add(new ReportDataSource("CalculoRelacaoCTesEntregues", listaCalculoRelacaoCTesEntreguess));
        //        dataSources.Add(new ReportDataSource("Textos", listaTextosCalculoRelacaoCTesEntreguess));
        //        dataSources.Add(new ReportDataSource("Itens", listaItens));

        //        Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

        //        Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = null;

        //        if ((from o in listaItens where o.Tipo != 0 select o).Count() > 0)
        //            arquivo = svcRelatorio.GerarWeb("Relatorios/VisualizacaoCalculoRelacaoCTesEntreguesSemTotal.rdlc", "PDF", parametros, dataSources);
        //        else
        //            arquivo = svcRelatorio.GerarWeb("Relatorios/VisualizacaoCalculoRelacaoCTesEntregues.rdlc", "PDF", parametros, dataSources);

        //        return Arquivo(arquivo.Arquivo, arquivo.MimeType, "Proposta - " + propostas.Data.ToString("dd-MM-yyyy") + "." + arquivo.FileNameExtension);
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);
        //        return Json<bool>(false, false, "Ocorreu uma falha ao gerar a proposta.");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}
        #endregion

        #region Métodos Privados
        private void SalvarCidades(Dominio.Entidades.CalculoRelacaoCTesEntregues calculoRelacaoCTesEntregue, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CalculoRelacaoCTesEntreguesCidade repCalculoRelacaoCTesEntreguesCidade = new Repositorio.CalculoRelacaoCTesEntreguesCidade(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

            if (!string.IsNullOrWhiteSpace(Request.Params["Cidades"]))
            {
                List<dynamic> cidades = JsonConvert.DeserializeObject<List<dynamic>>(Request.Params["Cidades"]);

                if (cidades != null)
                {
                    for (var i = 0; i < cidades.Count; i++)
                    {
                        Dominio.Entidades.CalculoRelacaoCTesEntreguesCidade cidade = repCalculoRelacaoCTesEntreguesCidade.BuscarPorCidade((int)cidades[i].Cidade, calculoRelacaoCTesEntregue.Codigo);

                        if (cidade == null)
                        {
                            cidade = new Dominio.Entidades.CalculoRelacaoCTesEntreguesCidade()
                            {
                                CalculoRelacaoCTesEntregues = calculoRelacaoCTesEntregue
                            };
                        }

                        Dominio.Entidades.Localidade cidadeCalculo = repLocalidade.BuscarPorCodigo((int)cidades[i].Cidade);

                        cidade.Cidade = cidadeCalculo;
                        cidade.Percentual = (decimal)cidades[i].Percentual;

                        if ((bool)cidades[i].Excluir)
                            repCalculoRelacaoCTesEntreguesCidade.Deletar(cidade);
                        else if (cidade.Codigo > 0)
                            repCalculoRelacaoCTesEntreguesCidade.Atualizar(cidade);
                        else
                            repCalculoRelacaoCTesEntreguesCidade.Inserir(cidade);
                    }
                }
            }
        }
        #endregion
    }
}