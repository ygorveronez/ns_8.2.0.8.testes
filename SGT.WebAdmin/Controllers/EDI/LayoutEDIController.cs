using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.EDI
{
    /// <summary>
    /// Quando precisar configurar um layout edi da natura, trocar para essa string de conexão que vai abrir nos EDIs da Natura.
    /// string coneApti = "Data Source=ank27fvlel.database.windows.net,1433;Initial Catalog=ControleCTeApti;User Id=MultiCTe;Password=Multi@2015;";
    /// </summary>
    [CustomAuthorize("EDI/LayoutEDI")]
    public class LayoutEDIController : BaseController
    {
        #region Construtores

        public LayoutEDIController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("descricao");
                Dominio.Enumeradores.TipoLayoutEDI tipoLayoutEDI = !string.IsNullOrEmpty(Request.Params("Tipo")) ? (Dominio.Enumeradores.TipoLayoutEDI)int.Parse(Request.Params("Tipo")) : Dominio.Enumeradores.TipoLayoutEDI.Todos;

                string sTiposLayoutsEDI = Request.Params("TipoLayoutEDI");
                int codigoGrupoPessoa;
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);

                List<Dominio.Enumeradores.TipoLayoutEDI> tiposLayoutsEDI = null;

                if (!string.IsNullOrWhiteSpace(sTiposLayoutsEDI))
                    tiposLayoutsEDI = JsonConvert.DeserializeObject<List<Dominio.Enumeradores.TipoLayoutEDI>>(sTiposLayoutsEDI);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 65, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.TipoDeLayout, "DescricaoTipoFormatado", 25, Models.Grid.Align.center, false);

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                List<Dominio.Entidades.LayoutEDI> listaLayoutEDI = repLayoutEDI.Consultar(descricao, tipoLayoutEDI, tiposLayoutsEDI, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, codigoGrupoPessoa);
                int countLayoutEDI = repLayoutEDI.ContarConsulta(descricao, tipoLayoutEDI, tiposLayoutsEDI, codigoGrupoPessoa);

                var lista = (from p in listaLayoutEDI
                             select new
                             {
                                 p.Codigo,
                                 p.Tipo,
                                 p.Descricao,
                                 p.DescricaoTipoFormatado
                             }).ToList();

                grid.setarQuantidadeTotal(countLayoutEDI);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                bool.TryParse(Request.Params("ValidarRota"), out bool validarRota);
                bool.TryParse(Request.Params("RemoverDiacriticos"), out bool removerDiacriticos);
                bool.TryParse(Request.Params("CamposPorIndices"), out bool camposPorIndices);
                bool.TryParse(Request.Params("GerarEDIEmOcorrencias"), out bool gerarEDIEmOcorrencias);

                unitOfWork.Start();

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Dominio.Entidades.LayoutEDI layoutEDI = new Dominio.Entidades.LayoutEDI();
                layoutEDI.Status = Request.GetStringParam("Status");
                layoutEDI.Descricao = Request.Params("Descricao");
                layoutEDI.Tipo = (Dominio.Enumeradores.TipoLayoutEDI)int.Parse(Request.Params("Tipo"));
                layoutEDI.Empresa = this.Empresa;
                layoutEDI.Separador = Request.Params("Separador");
                layoutEDI.QuantidadeNotasSequencia = Request.GetIntParam("QuantidadeNotasSequencia");
                layoutEDI.SeparadorDecimal = Request.Params("SeparadorDecimal");
                layoutEDI.SeparadorInicialFinal = bool.Parse(Request.Params("SeparadorInicialFinal"));
                layoutEDI.Nomenclatura = Request.Params("Nomenclatura");
                layoutEDI.ExtensaoArquivo = Request.Params("ExtensaoArquivo");
                layoutEDI.GerarEDIEmOcorrencias = gerarEDIEmOcorrencias;
                layoutEDI.IncluirCNPJEmitenteArquivoEDI = Request.GetBoolParam("IncluirCNPJEmitenteArquivoEDI");
                layoutEDI.ConsiderarDadosExpedidorECTe = Request.GetBoolParam("ConsiderarDadosExpedidorECTe");
                layoutEDI.ValidarRota = validarRota;
                layoutEDI.ValidarNumeroReferenciaEDI = Request.GetBoolParam("ValidarNumeroReferenciaEDI");
                layoutEDI.RemoverDiacriticos = removerDiacriticos;
                layoutEDI.BuscarNotaSemChaveDosDocumentosDestinados = Request.GetBoolParam("BuscarNotaSemChaveDosDocumentosDestinados");
                layoutEDI.AgruparNotasFiscaisDosCTesParaSubcontratacao = Request.GetBoolParam("AgruparNotasFiscaisDosCTesParaSubcontratacao");
                layoutEDI.Encoding = Request.GetStringParam("Encoding");
                layoutEDI.EmailLeitura = Request.GetStringParam("EmailLeitura");

                //todo: está fixo o numero de tentativas para cada novo layout cadastrado, não sei se é bom deixar para o usuário mudar isso, acredito que fique melhor nós mexermos diretamente no banco.
                layoutEDI.NumeroTentativasAutomaticasIntegracao = 3;

                List<Dominio.Entidades.CampoEDI> campos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.Entidades.CampoEDI>>((string)Request.Params("Campos"));
                layoutEDI.Campos = campos;
                repLayoutEDI.Inserir(layoutEDI, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool.TryParse(Request.Params("ValidarRota"), out bool validarRota);
                bool.TryParse(Request.Params("RemoverDiacriticos"), out bool removerDiacriticos);
                bool.TryParse(Request.Params("CamposPorIndices"), out bool camposPorIndices);
                bool.TryParse(Request.Params("GerarEDIEmOcorrencias"), out bool gerarEDIEmOcorrencias);

                unitOfWork.Start();
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                layoutEDI.Status = Request.GetStringParam("Status");
                layoutEDI.Descricao = Request.Params("Descricao");
                layoutEDI.Tipo = (Dominio.Enumeradores.TipoLayoutEDI)int.Parse(Request.Params("Tipo"));
                layoutEDI.Separador = Request.Params("Separador");
                layoutEDI.QuantidadeNotasSequencia = Request.GetIntParam("QuantidadeNotasSequencia");
                layoutEDI.SeparadorDecimal = Request.Params("SeparadorDecimal");
                layoutEDI.SeparadorInicialFinal = bool.Parse(Request.Params("SeparadorInicialFinal"));
                layoutEDI.Nomenclatura = Request.Params("Nomenclatura");
                layoutEDI.ExtensaoArquivo = Request.Params("ExtensaoArquivo");
                layoutEDI.ValidarRota = validarRota;
                layoutEDI.ValidarNumeroReferenciaEDI = Request.GetBoolParam("ValidarNumeroReferenciaEDI");
                layoutEDI.GerarEDIEmOcorrencias = gerarEDIEmOcorrencias;
                layoutEDI.IncluirCNPJEmitenteArquivoEDI = Request.GetBoolParam("IncluirCNPJEmitenteArquivoEDI");
                layoutEDI.ConsiderarDadosExpedidorECTe = Request.GetBoolParam("ConsiderarDadosExpedidorECTe");
                layoutEDI.CamposPorIndices = camposPorIndices;
                layoutEDI.RemoverDiacriticos = removerDiacriticos;
                layoutEDI.BuscarNotaSemChaveDosDocumentosDestinados = Request.GetBoolParam("BuscarNotaSemChaveDosDocumentosDestinados");
                layoutEDI.AgruparNotasFiscaisDosCTesParaSubcontratacao = Request.GetBoolParam("AgruparNotasFiscaisDosCTesParaSubcontratacao");
                layoutEDI.Encoding = Request.GetStringParam("Encoding");
                layoutEDI.EmailLeitura = Request.GetStringParam("EmailLeitura");

                List<Dominio.Entidades.CampoEDI> CamposAtualizar = new List<Dominio.Entidades.CampoEDI>();

                List<Dominio.Entidades.CampoEDI> campos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.Entidades.CampoEDI>>((string)Request.Params("Campos"));

                foreach (Dominio.Entidades.CampoEDI campoEDI in campos)
                {
                    Dominio.Entidades.CampoEDI campo = (from obj in layoutEDI.Campos where obj.Codigo == campoEDI.Codigo select obj).FirstOrDefault();
                    if (campo != null)
                    {
                        campo.Alinhamento = campoEDI.Alinhamento;
                        campo.Condicao = campoEDI.Condicao;
                        campo.Descricao = campoEDI.Descricao;
                        campo.IdentificadorRegistro = campoEDI.IdentificadorRegistro;
                        campo.IdentificadorRegistroPai = campoEDI.IdentificadorRegistroPai;
                        campo.Mascara = campoEDI.Mascara;
                        campo.Objeto = campoEDI.Objeto;
                        campo.Ordem = campoEDI.Ordem;
                        campo.PropriedadeObjeto = campoEDI.PropriedadeObjeto;
                        campo.Indice = campoEDI.Indice;
                        campo.QuantidadeCaracteres = campoEDI.QuantidadeCaracteres;
                        campo.QuantidadeDecimais = campoEDI.QuantidadeDecimais;
                        campo.QuantidadeInteiros = campoEDI.QuantidadeInteiros;
                        campo.Expressao = campoEDI.Expressao;
                        campo.NaoEscreverRegistro = campoEDI.NaoEscreverRegistro;
                        campo.PropriedadeObjetoPai = campoEDI.PropriedadeObjetoPai;
                        campo.Repetir = campoEDI.Repetir;
                        campo.Tipo = campoEDI.Tipo;
                        campo.ValorFixo = campoEDI.ValorFixo;
                        campo.RemoverCaracteresEspeciais = campoEDI.RemoverCaracteresEspeciais;

                        CamposAtualizar.Add(campo);
                    }
                    else
                    {
                        campoEDI.Codigo = 0;
                        CamposAtualizar.Add(campoEDI);
                    }
                }

                layoutEDI.Campos = CamposAtualizar;

                repLayoutEDI.Atualizar(layoutEDI, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorCodigo(codigo);
                var dynLayoutEDI = new
                {
                    layoutEDI.Codigo,
                    layoutEDI.Separador,
                    layoutEDI.QuantidadeNotasSequencia,
                    layoutEDI.Descricao,
                    layoutEDI.SeparadorDecimal,
                    layoutEDI.SeparadorInicialFinal,
                    layoutEDI.Status,
                    layoutEDI.CamposPorIndices,
                    layoutEDI.GerarEDIEmOcorrencias,
                    layoutEDI.IncluirCNPJEmitenteArquivoEDI,
                    layoutEDI.ConsiderarDadosExpedidorECTe,
                    layoutEDI.Tipo,
                    layoutEDI.Nomenclatura,
                    layoutEDI.ExtensaoArquivo,
                    layoutEDI.RemoverDiacriticos,
                    layoutEDI.BuscarNotaSemChaveDosDocumentosDestinados,
                    layoutEDI.AgruparNotasFiscaisDosCTesParaSubcontratacao,
                    layoutEDI.Encoding,
                    layoutEDI.EmailLeitura,
                    PermitirEdicao = (Usuario.UsuarioMultisoftware || Usuario.UsuarioAtendimento || Usuario.UsuarioAdministrador),
                    Campos = (from obj in layoutEDI.Campos
                             select new
                             {
                                 obj.Codigo,
                                 obj.Alinhamento,
                                 obj.Condicao,
                                 obj.Descricao,
                                 obj.IdentificadorRegistro,
                                 obj.IdentificadorRegistroPai,
                                 obj.Mascara,
                                 obj.Expressao,
                                 obj.Objeto,
                                 obj.Ordem,
                                 obj.PropriedadeObjeto,
                                 obj.PropriedadeObjetoPai,
                                 obj.Indice,
                                 obj.QuantidadeCaracteres,
                                 obj.QuantidadeDecimais,
                                 obj.QuantidadeInteiros,
                                 obj.Repetir,
                                 obj.Status,
                                 obj.Tipo,
                                 obj.ValorFixo,
                                 obj.NaoEscreverRegistro,
                                 obj.RemoverCaracteresEspeciais
                             }).ToList(),
                    layoutEDI.ValidarRota,
                    layoutEDI.ValidarNumeroReferenciaEDI
                };
                return new JsonpResult(dynLayoutEDI);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorCodigo(codigo);
                repLayoutEDI.Deletar(layoutEDI, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Globais Somente DEBUG

#if DEBUG

        public async Task<IActionResult> METODO_QUE_O_GUIGO_DEVERIA_TER_FEITO()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //IniciarEdiAmericanas(unitOfWork);
                //IniciarEdiDanone(unitOfWork);
                //IniciarEdiNatura(unitOfWork);
                IniciarEdiWalmart(unitOfWork);
                //IniciarLeitorOcr(unitOfWork);
                //IniciarEdiMartinBrower(unitOfWork);
                //IniciarEdiMagazineLuiza(unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        //public async Task<IActionResult> TESTEEDI()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
        //    try
        //    {
        //        Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
        //        Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
        //        Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

        //        Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorCodigo(15065);
        //        string arquivo = @"C:\Arquivos\FRDNEDANONE4381671900339520171229_181719.txt";
        //        System.IO.MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo));

        //        layoutEDI.Filial = repFilial.BuscarPorCodigo(3024);
        //        layoutEDI.TipoOperacao = repTipoOperacao.BuscarPorCodigo(2021);
        //        Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
        //        string retorno = serPedido.ImportarPedidoNOTFIS(layoutEDI, ms, null, TipoServicoMultisoftware, Auditado, unitOfWork);

        //        //Servicos.LeituraEDI leituraEDI = new Servicos.LeituraEDI(null, layoutEDI, ms, _conexao.StringConexao, 0, 0, 0, 0, 0, 0, 0, 0, true, true, System.Text.Encoding.GetEncoding("iso-8859-1"));
        //        //Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis retorno = leituraEDI.GerarNotasFis();

        //        return new JsonpResult(retorno);
        //    }
        //    catch (Exception ex)
        //    {
        //        unitOfWork.Rollback();
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
        //    }
        //}

#endif

        #endregion

        #region Métodos Privados

        private void IniciarEdiAmericanas(Repositorio.UnitOfWork unitOfWork)
        {            
            string adminStringConexao = _conexao.AdminStringConexao;
            Servicos.EDI.StartupEDIAmericanas startupEDIAmericanas = new Servicos.EDI.StartupEDIAmericanas(unitOfWork);
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
            string tipoArmazenamento = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoArmazenamento;
            string enderecoFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnderecoFTP;
            string usuarioFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UsuarioFTP;
            string senhaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().SenhaFTP;
            string caminhoRaizFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaizFTP;
            bool ftpPassivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FTPPassivo.Value;
            string portaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PortaFTP;
            bool utilizaSFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaSFTP.Value;

            startupEDIAmericanas.Iniciar(caminho, tipoArmazenamento, enderecoFTP, usuarioFTP, senhaFTP, caminhoRaizFTP, ftpPassivo, portaFTP, utilizaSFTP, adminStringConexao, "", 1000000);
        }

        private void IniciarEdiDanone(Repositorio.UnitOfWork unitOfWork)
        {            
            string adminStringConexao = _conexao.AdminStringConexao;
            Servicos.EDI.StartupEDIDanone startupEDIDanone = new Servicos.EDI.StartupEDIDanone(unitOfWork);
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
            string tipoArmazenamento = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoArmazenamento;
            string enderecoFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnderecoFTP;
            string usuarioFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UsuarioFTP;
            string senhaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().SenhaFTP;
            string caminhoRaizFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaizFTP;
            bool ftpPassivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FTPPassivo.Value;
            string portaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PortaFTP;
            bool utilizaSFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaSFTP.Value;
            string prefixosFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PrefixosFTP;

            startupEDIDanone.Iniciar(caminho, tipoArmazenamento, enderecoFTP, usuarioFTP, senhaFTP, caminhoRaizFTP, ftpPassivo, portaFTP, utilizaSFTP, adminStringConexao, "", "", false, 1000000, prefixosFTP);
        }

        private void IniciarEdiNatura(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.EDI.StartupEDINatura startupEDINatura = new Servicos.EDI.StartupEDINatura(unitOfWork);
            int empresa = int.Parse(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().CodigoEmpresaMultisoftware);
            int minutos = int.Parse(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().MinutosParaConsultaNatura);
            List<string> filiais = new List<string>();
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
            string[] filiaisSplit = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FiliaisNatura.Split(',');

            for (int i = 0; i < filiaisSplit.Length; i++)
            {
                filiais.Add(filiaisSplit[i]);
            }

            string tipoArmazenamento = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoArmazenamento;
            string enderecoFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnderecoFTP;
            string usuarioFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UsuarioFTP;
            string senhaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().SenhaFTP;
            string caminhoRaizFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaizFTP;
            string emails = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EmailsFTP;
            bool ftpPassivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FTPPassivo.Value;
            string portaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PortaFTP;
            bool utilizaSFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaSFTP.Value;
            bool gerarNotFisPorNota = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().GerarNotFisPorNota.Value;

            startupEDINatura.Iniciar(empresa, minutos, caminho, filiais, tipoArmazenamento, enderecoFTP, usuarioFTP, senhaFTP, caminhoRaizFTP, emails, ftpPassivo, portaFTP, utilizaSFTP, gerarNotFisPorNota);
        }

        private void IniciarEdiWalmart(Repositorio.UnitOfWork unitOfWork)
        {            
            string adminStringConexao = _conexao.AdminStringConexao;
            Servicos.EDI.StartupEDIWalmart startupEDIWalmart = new Servicos.EDI.StartupEDIWalmart(unitOfWork);
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
            string tipoArmazenamento = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoArmazenamento;
            string enderecoFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnderecoFTP;
            string usuarioFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UsuarioFTP;
            string senhaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().SenhaFTP;
            string caminhoRaizFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaizFTP;
            bool ftpPassivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FTPPassivo.Value;
            string portaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PortaFTP;
            bool utilizaSFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaSFTP.Value;

            startupEDIWalmart.Iniciar(caminho, tipoArmazenamento, enderecoFTP, usuarioFTP, senhaFTP, caminhoRaizFTP, caminhoRaizFTP, ftpPassivo, portaFTP, utilizaSFTP, adminStringConexao, "", 1000000);
        }

        private void IniciarEdiMartinBrower(Repositorio.UnitOfWork unitOfWork)
        {            
            string adminStringConexao = _conexao.AdminStringConexao;
            Servicos.EDI.StartupEDIMartinBrower startupEDIMartinBrower = new Servicos.EDI.StartupEDIMartinBrower(unitOfWork);
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
            string tipoArmazenamento = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoArmazenamento;
            string enderecoFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnderecoFTP;
            string usuarioFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UsuarioFTP;
            string senhaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().SenhaFTP;
            string caminhoRaizFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaizFTP;
            bool ftpPassivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FTPPassivo.Value;
            string portaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PortaFTP;
            bool utilizaSFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaSFTP.Value;

            startupEDIMartinBrower.Iniciar(caminho, tipoArmazenamento, enderecoFTP, usuarioFTP, senhaFTP, caminhoRaizFTP, ftpPassivo, portaFTP, utilizaSFTP, adminStringConexao, "", 1000000);
        }

        private void IniciarEdiMagazineLuiza(Repositorio.UnitOfWork unitOfWork)
        {            
            string adminStringConexao = _conexao.AdminStringConexao;
            Servicos.EDI.StartupEDIMagazineLuiza startupEDIMagazineLuiza = new Servicos.EDI.StartupEDIMagazineLuiza(unitOfWork);
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
            string tipoArmazenamento = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoArmazenamento;
            string enderecoFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnderecoFTP;
            string usuarioFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UsuarioFTP;
            string senhaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().SenhaFTP;
            string caminhoRaizFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaizFTP;
            bool ftpPassivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FTPPassivo.Value;
            string portaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PortaFTP;
            bool utilizaSFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaSFTP.Value;

            startupEDIMagazineLuiza.Iniciar(caminho, tipoArmazenamento, enderecoFTP, usuarioFTP, senhaFTP, caminhoRaizFTP, ftpPassivo, portaFTP, utilizaSFTP, adminStringConexao, "", 1000000);
        }

        private void IniciarLeitorOcr(Repositorio.UnitOfWork unitOfWork)
        {
            string adminStringConexao = _conexao.AdminStringConexao;
            Servicos.Embarcador.Canhotos.LeitorOCR serLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR(unitOfWork);
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
            string tipoArmazenamento = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().TipoArmazenamentoLeitorOCR;
            string enderecoFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnderecoFTP;
            string usuarioFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UsuarioFTP;
            string senhaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().SenhaFTP;
            string caminhoRaizFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaizFTP;
            bool ftpPassivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().FTPPassivo.Value;
            string portaFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PortaFTP;
            bool utilizaSFTP = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().UtilizaSFTP.Value;
            string caminhoCanhotos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoCanhotos;

            //api Gratuita
            string apiKey = "74dd86612188957";
            string apilink = "https://api.ocr.space/Parse/Image";

            ////api paga
            //string apiKey = "PKMXB6332888A";
            //string apilink = "https://apipro1.ocr.space/parse/image";

            serLeitorOCR.Iniciar(caminho, caminhoCanhotos, tipoArmazenamento, enderecoFTP, usuarioFTP, senhaFTP, caminhoRaizFTP, ftpPassivo, portaFTP, utilizaSFTP, adminStringConexao, "", apilink, apiKey);
        }

        #endregion
    }
}
