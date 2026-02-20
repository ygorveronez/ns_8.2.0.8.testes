using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class MinutaDevolucaoContainerController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("minutadevolucaocontainer.aspx") select obj).FirstOrDefault();
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
                int.TryParse(Request.Params["CTe"], out int codigoCTe);
                int.TryParse(Request.Params["NumeroMinuta"], out int numeroMinuta);

                string container = Request.Params["Container"];
                string importador = Request.Params["Importador"];
                string nomeTerminal = Request.Params["NomeTerminal"];
                string armador = Request.Params["Armador"];
                string navio = Request.Params["Navio"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string placaTracao = Request.Params["PlacaTracao"];
                string placaReboque = Request.Params["PlacaReboque"];

                Repositorio.MinutaDevolucaoContainer repMinutaDevolucaoContainer = new Repositorio.MinutaDevolucaoContainer(unidadeDeTrabalho);

                var minutas = repMinutaDevolucaoContainer.Consultar(this.EmpresaUsuario.Codigo, numeroMinuta, container, importador, nomeTerminal, armador, navio, nomeMotorista, placaTracao, placaReboque, codigoCTe, inicioRegistros, 50);
                int countMinutas = repMinutaDevolucaoContainer.ContarConsulta(this.EmpresaUsuario.Codigo, numeroMinuta, container, importador, nomeTerminal, armador, navio, nomeMotorista, placaTracao, placaReboque, codigoCTe);

                var retorno = (from obj in minutas
                               select new
                               {
                                   obj.Codigo,
                                   Numero = obj.Numero.ToString(),
                                   Container = obj.Container,
                                   Importador = obj.Importador,
                                   Terminal = obj.Terminal?.Descricao,
                                   CTe = obj.CTE.Descricao
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Número|10", "Container|20", "Importador|15", "Terminal|15", "CTe|10" }, countMinutas);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as coletas.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["CodigoMinuta"], out codigo);

                Repositorio.MinutaDevolucaoContainer repMinutaDevolucaoContainer = new Repositorio.MinutaDevolucaoContainer(unitOfWork);
                Dominio.Entidades.MinutaDevolucaoContainer minutaDevolucaoContainer = repMinutaDevolucaoContainer.BuscarPorCodigo(codigo);

                if (minutaDevolucaoContainer == null)
                    return Json<bool>(false, false, "Minuta não encontrada.");

                var retorno = new
                {
                    minutaDevolucaoContainer.Codigo,
                    minutaDevolucaoContainer.Numero,
                    minutaDevolucaoContainer.Container,
                    minutaDevolucaoContainer.Importador,
                    Status = minutaDevolucaoContainer.Ativo ? "1" : "0",
                    minutaDevolucaoContainer.Armador,
                    minutaDevolucaoContainer.TipoEquipamento,
                    Quantidade = minutaDevolucaoContainer.Quantidade.ToString("n0"),
                    Peso = minutaDevolucaoContainer.Peso.ToString("n2"),
                    minutaDevolucaoContainer.Navio,
                    minutaDevolucaoContainer.Observacao,
                    CodigoTerminal = minutaDevolucaoContainer.Terminal != null ? minutaDevolucaoContainer.Terminal.CPF_CNPJ : 0,
                    NomeTerminal = minutaDevolucaoContainer.Terminal != null ? minutaDevolucaoContainer.Terminal.Nome : string.Empty,
                    CodigoTracao = minutaDevolucaoContainer.Veiculo != null ? minutaDevolucaoContainer.Veiculo.Codigo : 0,
                    PlacaTracao = minutaDevolucaoContainer.Veiculo != null ? minutaDevolucaoContainer.Veiculo.Placa : string.Empty,
                    CodigoReboque = minutaDevolucaoContainer.Reboque != null ? minutaDevolucaoContainer.Reboque.Codigo : 0,
                    PlacaReboque = minutaDevolucaoContainer.Reboque != null ? minutaDevolucaoContainer.Reboque.Placa : string.Empty,
                    CodigoMotorista = minutaDevolucaoContainer.Motorista != null ? minutaDevolucaoContainer.Motorista.Codigo : 0,
                    NomeMotorista = minutaDevolucaoContainer.Motorista != null ? minutaDevolucaoContainer.Motorista.Nome : string.Empty,
                    CodigoCTe = minutaDevolucaoContainer.CTE != null ? minutaDevolucaoContainer.CTE.Codigo : 0,
                    DescricaoCTe = minutaDevolucaoContainer.CTE != null ? minutaDevolucaoContainer.CTE.Descricao : string.Empty
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes.");
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadEspelho()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["CodigoMinuta"], out int codigo);
                int.TryParse(Request.Params["Via"], out int via);

                Repositorio.MinutaDevolucaoContainer repMinutaDevolucaoContainer = new Repositorio.MinutaDevolucaoContainer(unitOfWork);
                Dominio.Entidades.MinutaDevolucaoContainer minutaDevolucaoContainer = repMinutaDevolucaoContainer.BuscarPorCodigo(codigo);

                if (minutaDevolucaoContainer == null)
                    return Json<bool>(false, false, "Minuta não encontrada.");

                Dominio.ObjetosDeValor.Relatorios.MinutaDevolucaoContainer minuta = new Dominio.ObjetosDeValor.Relatorios.MinutaDevolucaoContainer()
                {
                    Numero = minutaDevolucaoContainer.Numero,
                    Container = minutaDevolucaoContainer.Container,
                    Importador = minutaDevolucaoContainer.Importador,
                    NomeTerminal = minutaDevolucaoContainer.Terminal?.Nome ?? string.Empty,
                    CNPJTerminal = minutaDevolucaoContainer.Terminal?.CPF_CNPJ_Formatado ?? string.Empty,
                    EnderecoTerminal = minutaDevolucaoContainer.Terminal?.EnderecoCompletoCidadeeEstado ?? string.Empty,
                    ContatoTransportador = minutaDevolucaoContainer.Empresa.Contato + " " + minutaDevolucaoContainer.Empresa.TelefoneContato,
                    Armador = minutaDevolucaoContainer.Armador,
                    TipoEquipamento = minutaDevolucaoContainer.TipoEquipamento,
                    Quantidade = minutaDevolucaoContainer.Quantidade,
                    Peso = minutaDevolucaoContainer.Peso,
                    Navio = minutaDevolucaoContainer.Navio,
                    CPFMotorista = minutaDevolucaoContainer.Motorista?.CPF_CNPJ_Formatado ?? string.Empty,
                    NomeMotorista = minutaDevolucaoContainer.Motorista?.Nome ?? string.Empty,
                    CNHMotorista = minutaDevolucaoContainer.Motorista?.NumeroHabilitacao ?? string.Empty,
                    PlacaTracao = minutaDevolucaoContainer.Veiculo?.Placa ?? string.Empty,
                    PlacaReboque = minutaDevolucaoContainer.Reboque?.Placa ?? string.Empty,
                    CTe = minutaDevolucaoContainer.CTE?.Descricao ?? string.Empty,
                    Observacap = minutaDevolucaoContainer.Observacao
                };

                // Define o titulo do relatorio
                string tituloRelatorio = "Minuta de devolução de container";

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("Minuta", new List<Dominio.ObjetosDeValor.Relatorios.MinutaDevolucaoContainer>() { minuta }));

                List<ReportParameter> parametros = new List<ReportParameter>();

                parametros.Add(new ReportParameter("TituloRelatorio", tituloRelatorio.ToUpper()));
                parametros.Add(new ReportParameter("NomeEmpresa", this.EmpresaUsuario.RazaoSocial));
                parametros.Add(new ReportParameter("CnpjEmpresa", this.EmpresaUsuario.CNPJ));
                parametros.Add(new ReportParameter("IeEmpresa", this.EmpresaUsuario.InscricaoEstadual));
                parametros.Add(new ReportParameter("CidadeEmpresa", this.EmpresaUsuario.Localidade.Descricao + "/" + this.EmpresaUsuario.Localidade.Estado.Sigla));
                parametros.Add(new ReportParameter("TelefoneEmpresa", this.EmpresaUsuario.Telefone));
                parametros.Add(new ReportParameter("EnderecoEmpresa", this.EmpresaUsuario.Endereco + " - " + this.EmpresaUsuario.Numero));
                parametros.Add(new ReportParameter("BairroEmpresa", this.EmpresaUsuario.Bairro));
                parametros.Add(new ReportParameter("CepEmpresa", this.EmpresaUsuario.CEP));
                parametros.Add(new ReportParameter("Logo", this.EmpresaUsuario.CaminhoLogoDacte));
                parametros.Add(new ReportParameter("Via", via == 1 ? "Via 1: Transportadora" : "Via 2: Terminal" ));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/MinutaDevolucaoContainer.rdlc", "PDF", parametros, dataSources);

                string nomeArquivo = string.Empty;
                if (via == 1)
                    nomeArquivo = "Minuta_" + minuta.Numero.ToString() + "_Via1";
                else
                    nomeArquivo = "Minuta_" + minuta.Numero.ToString() + "_Via2";

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, nomeArquivo + "." + arquivo.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o espelho da coleta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

       [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params["Codigo"], out int codigo);
                int.TryParse(Request.Params["Quantidade"], out int quantidade);
                int.TryParse(Request.Params["CodigoMotorista"], out int codigoMotorista);
                int.TryParse(Request.Params["CodigoTracao"], out int codigoTracao);
                int.TryParse(Request.Params["CodigoReboque"], out int codigoReboque);
                int.TryParse(Request.Params["CodigoCTe"], out int codigoCTe);
                int.TryParse(Request.Params["Status"], out int status);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoTerminal"]), out double codigoTerminal);
                decimal.TryParse(Utilidades.String.OnlyNumbers(Request.Params["Peso"]), out decimal peso);

                string container = Request.Params["Container"];
                string importador = Request.Params["Importador"];
                string armador = Request.Params["Armador"];
                string tipoEquipamento = Request.Params["TipoEquipamento"];
                string navio = Request.Params["Navio"];
                string observacao = Request.Params["Observacao"];

                Repositorio.MinutaDevolucaoContainer repMinutaDevolucaoContainer = new Repositorio.MinutaDevolucaoContainer(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Dominio.Entidades.MinutaDevolucaoContainer minutaDevolucaoContainer;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");

                    minutaDevolucaoContainer = repMinutaDevolucaoContainer.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");

                    minutaDevolucaoContainer = new Dominio.Entidades.MinutaDevolucaoContainer();
                    minutaDevolucaoContainer.Empresa = this.EmpresaUsuario;
                    minutaDevolucaoContainer.Numero = repMinutaDevolucaoContainer.ObterUltimoNumero(this.EmpresaUsuario.Codigo) + 1;
                }

                minutaDevolucaoContainer.Container = container;
                minutaDevolucaoContainer.Importador = importador;
                minutaDevolucaoContainer.Ativo = status == 1;
                minutaDevolucaoContainer.Armador = armador;
                minutaDevolucaoContainer.TipoEquipamento = tipoEquipamento;
                minutaDevolucaoContainer.Quantidade = quantidade;
                minutaDevolucaoContainer.Peso = peso;
                minutaDevolucaoContainer.Navio = navio;
                minutaDevolucaoContainer.Observacao = observacao;
                minutaDevolucaoContainer.Terminal = codigoTerminal > 0 ? repCliente.BuscarPorCPFCNPJ(codigoTerminal) : null;
                minutaDevolucaoContainer.Motorista = codigoMotorista > 0 ? repMotorista.BuscarPorCodigo(codigoMotorista) : null;
                minutaDevolucaoContainer.Veiculo = codigoTracao > 0 ? repVeiculo.BuscarPorCodigo(codigoTracao) : null;
                minutaDevolucaoContainer.Reboque = codigoReboque > 0 ? repVeiculo.BuscarPorCodigo(codigoReboque) : null;
                minutaDevolucaoContainer.CTE = codigoCTe > 0 ? repCTe.BuscarPorCodigo(codigoCTe) : null;

                if (codigo > 0)
                    repMinutaDevolucaoContainer.Atualizar(minutaDevolucaoContainer);
                else
                    repMinutaDevolucaoContainer.Inserir(minutaDevolucaoContainer);


                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.CommitChanges();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a coleta.");
            }
        }

        public ActionResult ObterProximoNumero()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.MinutaDevolucaoContainer repMinutaDevolucaoContainer = new Repositorio.MinutaDevolucaoContainer(unitOfWork);
                int numero = repMinutaDevolucaoContainer.ObterUltimoNumero(this.EmpresaUsuario.Codigo) + 1;

                var retorno = new
                {
                    numero
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
    }
}
