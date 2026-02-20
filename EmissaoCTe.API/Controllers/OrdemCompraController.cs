using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class OrdemCompraController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("ordemdecompra.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                int numero = 0;
                int.TryParse(Request.Params["Numero"], out numero);

                DateTime data;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                string descricao = Request.Params["Descricao"];
                string solicitante = Request.Params["Solicitante"];

                Repositorio.OrdemDeCompra repOrdemDeCompra = new Repositorio.OrdemDeCompra(unidadeDeTrabalho);
                
                List<Dominio.Entidades.OrdemDeCompra> lista = repOrdemDeCompra.ConsultarOrdemDeCompra(this.EmpresaUsuario.Codigo, numero, descricao, data, solicitante, Dominio.Enumeradores.TipoOrdemDeCompra.Servicos, inicioRegistros, 50);
                int countLista = repOrdemDeCompra.ContarConsultaOrdemDeCompra(this.EmpresaUsuario.Codigo, numero, descricao, data, solicitante, Dominio.Enumeradores.TipoOrdemDeCompra.Servicos);

                dynamic dynLista = from obj in lista
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Numero,
                                       Data = obj.Data.ToString("dd/MM/yyyy"),
                                       Solicitante = obj.Solicitante != null ? obj.Solicitante.Nome : obj.NomeSolicitante,
                                       Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : string.Empty
                                   };

                return Json(dynLista, true, null, new string[] { "Codigo", "Número|15", "Data|20", "Solicitante|30", "Veiculo|15" }, countLista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarPorMateriais()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                int numero = 0;
                int.TryParse(Request.Params["Numero"], out numero);

                DateTime data;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                string descricao = Request.Params["Descricao"];
                string solicitante = Request.Params["Solicitante"];

                Repositorio.OrdemDeCompra repOrdemDeCompra = new Repositorio.OrdemDeCompra(unidadeDeTrabalho);

                List<Dominio.Entidades.OrdemDeCompra> lista = repOrdemDeCompra.ConsultarOrdemDeCompra(this.EmpresaUsuario.Codigo, numero, descricao, data, solicitante, Dominio.Enumeradores.TipoOrdemDeCompra.Materiais, inicioRegistros, 50);
                int countLista = repOrdemDeCompra.ContarConsultaOrdemDeCompra(this.EmpresaUsuario.Codigo, numero, descricao, data, solicitante, Dominio.Enumeradores.TipoOrdemDeCompra.Materiais);

                dynamic dynLista = from obj in lista
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Numero,
                                       Data = obj.Data.ToString("dd/MM/yyyy"),
                                       Solicitante = obj.Solicitante != null ? obj.Solicitante.Nome : obj.NomeSolicitante,
                                       Fornecedor = obj.Fornecedor != null ? obj.Fornecedor.Nome : string.Empty
                                   };

                return Json(dynLista, true, null, new string[] { "Codigo", "Número|15", "Data|20", "Solicitante|30", "Veiculo|15" }, countLista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.OrdemDeCompra repOrdemDeCompra = new Repositorio.OrdemDeCompra(unidadeDeTrabalho);
                Dominio.Entidades.OrdemDeCompra ordemDeCompra = repOrdemDeCompra.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (ordemDeCompra == null)
                    return Json<bool>(false, false, "Ordem não encontrada.");

                var retorno = new
                {
                    ordemDeCompra.Codigo,
                    ordemDeCompra.Numero,
                    Data = ordemDeCompra.Data.ToString("dd/MM/yyyy"),
                    Solicitante = ordemDeCompra.Solicitante != null ? new { Codigo = ordemDeCompra.Solicitante.Codigo, Descricao = ordemDeCompra.Solicitante.Nome } : null,
                    ordemDeCompra.Servico,
                    ordemDeCompra.NomeSolicitante,
                    ordemDeCompra.Setor,
                    Veiculo = ordemDeCompra.Veiculo != null ? new { Codigo = ordemDeCompra.Veiculo.Codigo, Descricao = ordemDeCompra.Veiculo.Placa } : null,
                    ModeloVeiculo = ordemDeCompra.ModeloVeiculo != null ? new { Codigo = ordemDeCompra.ModeloVeiculo.Codigo, Descricao = ordemDeCompra.ModeloVeiculo.Descricao } : null,
                    ordemDeCompra.Descricao,
                    Fornecedor = ordemDeCompra.Fornecedor != null ? new { Codigo = ordemDeCompra.Fornecedor.CPF_CNPJ_Formatado, Descricao = ordemDeCompra.Fornecedor.CPF_CNPJ_Formatado + " - " + ordemDeCompra.Fornecedor.Nome } : null
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadOrdem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Dominio.Enumeradores.TipoOrdemDeCompra tipoOrdemDeCompraAux;
                Dominio.Enumeradores.TipoOrdemDeCompra? tipoOrdemDeCompra = Dominio.Enumeradores.TipoOrdemDeCompra.Servicos;
                if (Enum.TryParse<Dominio.Enumeradores.TipoOrdemDeCompra>(Request.Params["Tipo"], out tipoOrdemDeCompraAux))
                    tipoOrdemDeCompra = tipoOrdemDeCompraAux;

                Repositorio.OrdemDeCompra repOrdemDeCompra = new Repositorio.OrdemDeCompra(unitOfWork);

                List<Dominio.ObjetosDeValor.Relatorios.OrdemDeCompra> ordemDeCompra = repOrdemDeCompra.EspelhoBuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (ordemDeCompra.Count() != 1)
                    return Json<bool>(false, false, "Ordem não encontrada.");

                string numeroDaOrdem = ordemDeCompra[0].Numero.ToString();

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("OrdemDeCompra", ordemDeCompra));

                List<ReportParameter> parametros = new List<ReportParameter>();
                parametros.Add(new ReportParameter("NumeroOrdem", numeroDaOrdem));
                parametros.Add(new ReportParameter("Logo", this.EmpresaUsuario.CaminhoLogoDacte));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                string nomeRelatorio = string.Empty;
                if (tipoOrdemDeCompra == Dominio.Enumeradores.TipoOrdemDeCompra.Servicos)
                    nomeRelatorio = "Relatorios/OrdemDeCompra.rdlc";
                else
                    nomeRelatorio = "Relatorios/OrdemDeCompraMateriais.rdlc";

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb(nomeRelatorio, "PDF", parametros, dataSources);

                unitOfWork.Dispose();

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "Ordem de compra - " + numeroDaOrdem + "." + arquivo.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o espelho da coleta.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoSolicitante, codigoVeiculo, codigoModelo, numero;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Solicitante"], out codigoSolicitante);
                int.TryParse(Request.Params["Veiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["Modelo"], out codigoModelo);
                int.TryParse(Request.Params["Numero"], out numero);

                DateTime data;
                DateTime.TryParseExact(Request.Params["Data"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                string descricao = Request.Params["Descricao"];
                string setor = Request.Params["Setor"];
                string servico = Request.Params["Servico"];
                string fornecedor = Request.Params["Fornecedor"];
                string nomeSolicitante = Request.Params["NomeSolicitante"];

                Dominio.Enumeradores.TipoOrdemDeCompra tipoOrdemDeCompraAux;
                Dominio.Enumeradores.TipoOrdemDeCompra? tipoOrdemDeCompra = Dominio.Enumeradores.TipoOrdemDeCompra.Servicos;
                if (Enum.TryParse<Dominio.Enumeradores.TipoOrdemDeCompra>(Request.Params["Tipo"], out tipoOrdemDeCompraAux))
                    tipoOrdemDeCompra = tipoOrdemDeCompraAux;

                if (numero == 0)
                    return Json<bool>(false, false, "Número é obrigatório.");

                if (data == DateTime.MinValue)
                    return Json<bool>(false, false, "Data é obrigatória.");

                if (codigoSolicitante == 0 && string.IsNullOrWhiteSpace(nomeSolicitante))
                    return Json<bool>(false, false, "Solicitante é obrigatório.");

                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição é obrigatória.");

                Repositorio.OrdemDeCompra repOrdemDeCompra = new Repositorio.OrdemDeCompra(unidadeDeTrabalho);
                Dominio.Entidades.OrdemDeCompra ordemDeCompra;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    ordemDeCompra = repOrdemDeCompra.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    ordemDeCompra = new Dominio.Entidades.OrdemDeCompra();
                    ordemDeCompra.Empresa = this.EmpresaUsuario;
                }

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.Usuario repSolicitante = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                if (numero == 0)
                    numero = repOrdemDeCompra.ObterUltimoNumero(this.EmpresaUsuario.Codigo, tipoOrdemDeCompra) + 1;

                ordemDeCompra.Numero = numero;
                ordemDeCompra.Data = data;
                ordemDeCompra.Servico = servico;
                ordemDeCompra.Setor = setor;
                ordemDeCompra.Veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
                ordemDeCompra.ModeloVeiculo = repModeloVeiculo.BuscarPorCodigo(codigoModelo, this.EmpresaUsuario.Codigo);
                ordemDeCompra.Descricao = descricao;
                ordemDeCompra.Fornecedor = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(fornecedor)) && Utilidades.String.OnlyNumbers(fornecedor) != "0" ? repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(fornecedor))) : null;

                ordemDeCompra.TipoOrdemDeCompra = tipoOrdemDeCompra;

                if(codigoSolicitante == 0)
                {
                    ordemDeCompra.NomeSolicitante = nomeSolicitante;
                    ordemDeCompra.Solicitante = null;
                }
                else
                {
                    ordemDeCompra.Solicitante = repSolicitante.BuscarPorCodigo(codigoSolicitante);
                    ordemDeCompra.NomeSolicitante = null;
                }

                if(repOrdemDeCompra.DuplicidadeDeNumero(this.EmpresaUsuario.Codigo, numero, codigo, tipoOrdemDeCompra))
                    return Json<bool>(false, false, "Já existe outra ordem com mesmo número.");

                unidadeDeTrabalho.Start();

                if (codigo > 0)
                    repOrdemDeCompra.Atualizar(ordemDeCompra);
                else
                    repOrdemDeCompra.Inserir(ordemDeCompra);

                unidadeDeTrabalho.CommitChanges();

                var retorno = new
                {
                    Codigo = ordemDeCompra.Codigo
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.CommitChanges();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a ordem.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterProximoNumero()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.OrdemDeCompra repOrdemDeCompra = new Repositorio.OrdemDeCompra(unidadeDeTrabalho);
                int numero = repOrdemDeCompra.ObterUltimoNumero(this.EmpresaUsuario.Codigo, Dominio.Enumeradores.TipoOrdemDeCompra.Servicos) + 1;

                var retorno = new
                {
                    Numero = numero
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os próximo número.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterProximoNumeroMateriais()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.OrdemDeCompra repOrdemDeCompra = new Repositorio.OrdemDeCompra(unidadeDeTrabalho);
                int numero = repOrdemDeCompra.ObterUltimoNumero(this.EmpresaUsuario.Codigo, Dominio.Enumeradores.TipoOrdemDeCompra.Materiais) + 1;

                var retorno = new
                {
                    Numero = numero
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os próximo número.");
            }
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
