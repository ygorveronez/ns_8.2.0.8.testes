using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class DestinadosNFesController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                FiltrosConsulta(out DateTime dataInicial, out DateTime dataFinal, out string placa, out string cnpjEmitente, out string nomeEmitente, out string chave, out int inicioRegistros, out int numero, out int numeroFinal, out int serie, out bool? cancelado, out bool? notasSemCTe, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumentoDestinado, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado modelo, out string ufDestinatario, out int fimRegistros);

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentos = repDocumentoDestinadoEmpresa.ConsultarMultiCTe(this.EmpresaUsuario.Codigo, modelo, tipoDocumentoDestinado, DateTime.MinValue, DateTime.MinValue, dataInicial, dataFinal, numero, numeroFinal, serie, cnpjEmitente, nomeEmitente, placa, chave, cancelado, notasSemCTe, string.Empty, string.Empty, string.Empty, string.Empty, ufDestinatario, false, inicioRegistros, fimRegistros);
                int count = repDocumentoDestinadoEmpresa.ContarMultiCTe(this.EmpresaUsuario.Codigo, modelo, tipoDocumentoDestinado, DateTime.MinValue, DateTime.MinValue, dataInicial, dataFinal, numero, numeroFinal, serie, cnpjEmitente, nomeEmitente, placa, chave, cancelado, notasSemCTe, string.Empty, string.Empty, string.Empty, string.Empty, false, ufDestinatario);

                var lista = (from obj in documentos
                             select new
                             {
                                 obj.Codigo,
                                 Emissor = obj.CPFCNPJEmitente_Formatado + " " + obj.NomeEmitente,
                                 obj.Numero,
                                 obj.Serie,
                                 Data = obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 Placa = obj.Placa,
                                 Chave = obj.Chave,
                             }).ToList();

                return Json(lista, true, null, new string[] { "Código", "Emitente|35", "Número|7", "Serie|7", "Dt Emissão|10", "Placa|10", "Chave|20" }, count);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        [AcceptVerbs("POST")]
        public ActionResult SelecionarTodosDocumentosDestinados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                FiltrosConsulta(out DateTime dataInicial, out DateTime dataFinal, out string placa, out string cnpjEmitente, out string nomeEmitente, out string chave, out int inicioRegistros, out int numero, out int numeroFinal, out int serie, out bool? cancelado, out bool? notasSemCTe, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumentoDestinado, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado modelo, out string ufDestinatario, out int fimRegistros);

                if (string.IsNullOrWhiteSpace(placa) && string.IsNullOrWhiteSpace(cnpjEmitente) && numero == 0 && numeroFinal == 0)
                    return Json<bool>(false, false, "É obrigatório informar uma PLACA ou CNPJ do Emitente.");

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentos = repDocumentoDestinadoEmpresa.ConsultarMultiCTe(this.EmpresaUsuario.Codigo, modelo, tipoDocumentoDestinado, DateTime.MinValue, DateTime.MinValue, dataInicial, dataFinal, numero, numeroFinal, serie, cnpjEmitente, nomeEmitente, placa, chave, cancelado, notasSemCTe, string.Empty, string.Empty, string.Empty, string.Empty, ufDestinatario, false, 0, 0);

                var lista = (from obj in documentos
                             select new
                             {
                                 obj.Codigo,
                                 obj.Numero,
                                 obj.Serie,
                                 Placa = obj.Placa
                             }).ToList();

                return Json(lista, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult ConsultarNFesDestinadasAdmin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unitOfWork);

                int.TryParse(Request.Params["Empresa"], out int empresa);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa configuracaoDocumentoDestinadoEmpresa = repConfiguracaoDocumentoDestinadoEmpresa.BuscarPorEmpresa(empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe);

                if (empresa == 0)
                    return Json<bool>(false, false, "Empresa é obrigatória.");

                if (configuracaoDocumentoDestinadoEmpresa != null && configuracaoDocumentoDestinadoEmpresa.EmConsulta)
                    return Json<bool>(false, false, "Consulta em andamento.");

                if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ValidaUltimaConsultaNotas(empresa, out string msg, unitOfWork))
                    return Json<bool>(false, false, msg);

                System.Threading.Tasks.Task.Factory.StartNew(() => ObterDocumentosDestinadosEmpresa(empresa, Conexao.StringConexao));


                return Json(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "NotasDestinadas");
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void ObterDocumentosDestinadosEmpresa(int empresa, string stringconexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa configuracaoDocumentoDestinadoEmpresa = repConfiguracaoDocumentoDestinadoEmpresa.BuscarPorEmpresa(empresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe);

            try
            {
                if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(empresa, stringconexao, new List<string>(), out string mensagemRetorno, out string codigoStatusRetornoSefaz))
                    Servicos.Log.TratarErro(mensagemRetorno, "NotasDestinadas");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "NotasDestinadas");
                configuracaoDocumentoDestinadoEmpresa.EmConsulta = false;
                repConfiguracaoDocumentoDestinadoEmpresa.Atualizar(configuracaoDocumentoDestinadoEmpresa);
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarNFesDestinadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa repConfiguracaoDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinadoEmpresa configuracaoDocumentoDestinadoEmpresa = repConfiguracaoDocumentoDestinadoEmpresa.BuscarPorEmpresa(this.EmpresaUsuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe);

                bool.TryParse(Request.Params["ConfirmaConsulta"], out bool confirmaConsulta);

                if (configuracaoDocumentoDestinadoEmpresa == null && !confirmaConsulta)
                    return Json(new { PossuiConfiguracao = false }, true);

                if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ValidaUltimaConsultaNotas(this.EmpresaUsuario.Codigo, out string msg, unitOfWork))
                    return Json<bool>(false, false, msg);

                if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(this.EmpresaUsuario.Codigo, Conexao.StringConexao, new List<string>(), out string mensagemRetorno, out string codigoStatusRetornoSefaz, 0, null, null, null, true))
                    return Json<bool>(false, false, mensagemRetorno);
                else
                    return Json(new { PossuiConfiguracao = true }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "NotasDestinadas");
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult RetornarObjetoNFes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                List<long> codigoDocumentosSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<List<long>>(Request.Params["DocumentosSelecionados"]);
                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosSelecionados = repDocumentoDestinadoEmpresa.BuscarPorCodigos(this.EmpresaUsuario.Codigo, codigoDocumentosSelecionados);

                List<object> nfes = Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterObjetoValorParaEmissao(this.EmpresaUsuario.Codigo, documentosSelecionados, this.UsuarioAdministrativo, Conexao.StringConexao, unitOfWork);

                return Json(nfes, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void FiltrosConsulta(out DateTime dataInicial, out DateTime dataFinal, out string placa, out string cnpjEmitente, out string nomeEmitente, out string chave, out int inicioRegistros, out int numero, out int numeroFinal, out int serie, out bool? cancelado, out bool? notasSemCTe, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa tipoDocumentoDestinado, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado modelo, out string ufDestinatario, out int fimRegistros)
        {
            DateTime.TryParse(Request.Params["DataInicial"], out dataInicial);
            DateTime.TryParse(Request.Params["DataFinal"], out dataFinal);

            placa = Request.Params["Placa"];
            ufDestinatario = Request.Params["UFDestinatario"];
            cnpjEmitente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJEmiente"]);
            nomeEmitente = Request.Params["Emiente"];
            chave = Utilidades.String.OnlyNumbers(Request.Params["Chave"]);

            int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
            int.TryParse(Request.Params["fimRegistros"], out fimRegistros);
            int.TryParse(Request.Params["Numero"], out numero);
            int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
            int.TryParse(Request.Params["Serie"], out serie);

            notasSemCTe = null;
            if (bool.TryParse(Request.Params["NotasSemCTe"], out bool notasSemCTeAux))
                notasSemCTe = notasSemCTeAux;

            cancelado = null;
            if (bool.TryParse(Request.Params["Cancelado"], out bool canceladoAux))
                cancelado = canceladoAux;

            tipoDocumentoDestinado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeTransporte;
            modelo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.NFe;

            if (fimRegistros == 0)
                fimRegistros = 20;
        }

    }
}