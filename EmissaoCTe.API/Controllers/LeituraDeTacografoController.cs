using System;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class LeituraDeTacografoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("ocorrenciasdefuncionarios.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                string nomeMotorista = Request.Params["NomeMotorista"];
                string placaVeiculo = Request.Params["PlacaVeiculo"];
                string status = Request.Params["Status"];

                Repositorio.LeituraDeTacografo repLeituraDeTacografo = new Repositorio.LeituraDeTacografo(unitOfWork);
                var listaLeituraTacografo = repLeituraDeTacografo.Consultar(this.EmpresaUsuario.Codigo, nomeMotorista, placaVeiculo, status, inicioRegistros, 50);
                int countLeituraTacografo = repLeituraDeTacografo.ContarConsulta(this.EmpresaUsuario.Codigo, nomeMotorista, placaVeiculo, status);

                var retorno = from obj in listaLeituraTacografo select new { obj.Codigo, DataInicial = obj.DataInicial.ToString("dd/MM/yyyy"), DataFinal = obj.DataFinal.ToString("dd/MM/yyyy"), Motorista = string.Concat(obj.Motorista.CPF, " - ", obj.Motorista.Nome), Veiculo = obj.Veiculo.Placa, obj.DescricaoStatus };

                return Json(retorno, true, null, new string[] { "Código", "Data Inicial|15", "Data Final|15", "Motorista|30", "Veículo|15", "Status|15" }, countLeituraTacografo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as leituras de tacógrafos.");
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
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.LeituraDeTacografo repLeituraDeTacografo = new Repositorio.LeituraDeTacografo(unitOfWork);
                Dominio.Entidades.LeituraDeTacografo leitura = repLeituraDeTacografo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (leitura != null)
                {
                    var retorno = new
                    {
                        leitura.Codigo,
                        DataInicial = leitura.DataInicial.ToString("dd/MM/yyyy"),
                        DataFinal = leitura.DataFinal.ToString("dd/MM/yyyy"),
                        DataDeCadastro = leitura.DataDeCadastro.ToString("dd/MM/yyyy"),
                        leitura.Observacao,
                        leitura.DescricaoStatus,
                        CodigoMotorista = leitura.Motorista.Codigo,
                        CPFMotorista = leitura.Motorista.CPF,
                        NomeMotorista = leitura.Motorista.Nome,
                        leitura.Status,
                        leitura.Excesso,
                        PlacaVeiculo = leitura.Veiculo != null ? leitura.Veiculo.Placa : string.Empty,
                        CodigoVeiculo = leitura.Veiculo != null ? leitura.Veiculo.Codigo : 0,
                    };
                    return Json(retorno, true);
                }
                else
                {
                    return Json<bool>(false, false, "Leitura de tacógrafo não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar a leitura de tacógrafo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoVeiculo, codigoMotorista = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["CodigoVeiculo"], out codigoVeiculo);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                if (dataInicial == DateTime.MinValue)
                    return Json<bool>(false, false, "Data inicial inválida.");
                if (dataFinal == DateTime.MinValue)
                    return Json<bool>(false, false, "Data final inválida.");

                string status = Request.Params["Status"];
                string observacao = Request.Params["Observacao"];

                if (string.IsNullOrWhiteSpace(status))
                    return Json<bool>(false, false, "Status inválido.");

                bool excesso = false;
                bool.TryParse(Request.Params["Excesso"], out excesso);

                Repositorio.LeituraDeTacografo repLeituraTacografo = new Repositorio.LeituraDeTacografo(unitOfWork);
                Dominio.Entidades.LeituraDeTacografo leitura;
                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    leitura = new Dominio.Entidades.LeituraDeTacografo();
                    leitura.Status = "A";
                    leitura.DataDeCadastro = DateTime.Now;
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    leitura = repLeituraTacografo.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                }

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                leitura.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
                if (leitura.Motorista == null)
                    return Json<bool>(false, false, "Motorista não encontrado.");

                leitura.Veiculo = repVeiculo.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoVeiculo);
                if (leitura.Veiculo == null)
                    return Json<bool>(false, false, "Veículo não encontrado.");

                leitura.DataInicial = dataInicial;
                leitura.DataFinal = dataFinal;
                leitura.Excesso = excesso;
                leitura.Observacao = observacao;

                if (this.Permissao() != null && this.Permissao().PermissaoDeDelecao == "A")
                    leitura.Status = status;

                if (codigo > 0)
                    repLeituraTacografo.Atualizar(leitura);
                else
                    repLeituraTacografo.Inserir(leitura);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a leitura de tacógrafo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
