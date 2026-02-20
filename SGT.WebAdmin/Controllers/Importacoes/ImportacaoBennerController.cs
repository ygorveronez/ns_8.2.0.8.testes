using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.OleDb;

namespace SGT.WebAdmin.Controllers.Importacoes
{
    [AllowAnonymous]
    public class ImportacaoBennerController : BaseController
    {
        #region Construtores

        public ImportacaoBennerController(Conexao conexao) : base(conexao) { }

        #endregion

        // GET: ImportacaoBenner
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportarVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
                Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(unitOfWork);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

                DataSet ds = new DataSet();
                if (customFile.Length > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {

                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        customFile.SaveAs(fileLocation);
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                        try
                        {

                            excelConnection.Open();
                            DataTable dt = new DataTable();

                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            if (dt == null)
                            {
                                return null;
                            }

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            int indiceLinha = 1;
                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                indiceLinha++;
                                try
                                {
                                    //string cnpj = long.Parse(linha["CNPJTRANSPORTADOR"].ToString()).ToString("d14");
                                    Dominio.Entidades.Empresa empresa = null;//repEmpresa.BuscarPorCNPJ(cnpj);
                                    if (empresa == null)
                                    {
                                        string placa = linha["Placa"].ToString().Replace("-", "");
                                        if (!string.IsNullOrWhiteSpace(placa))
                                        {
                                            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(0, placa);
                                            bool inserir = false;
                                            if (veiculo == null)
                                            {
                                                inserir = true;
                                                veiculo = new Dominio.Entidades.Veiculo();
                                            }

                                            //Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.buscarPorCodigoEmbarcador(linha["TIPOVEICULO"].ToString());
                                            //veiculo.Empresa = empresa;
                                            //veiculo.ModeloVeicularCarga = modeloVeicular;
                                            //Dominio.Entidades.Localidade localidadeVeiculo = repLocalidade.BuscarPorCodigoIBGE(int.Parse(linha["IBGECAVALO"].ToString()));
                                            //veiculo.Estado = new Dominio.Entidades.Estado() { Sigla = localidadeVeiculo.Estado.Sigla };
                                            veiculo.Placa = placa;
                                            if (!string.IsNullOrWhiteSpace(linha["Renavan"].ToString()) && Utilidades.String.OnlyNumbers(linha["Renavan"].ToString()).Length > 11)
                                                veiculo.Renavam = Utilidades.String.OnlyNumbers(linha["Renavan"].ToString()).Substring(0, 11);
                                            else if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(linha["Renavan"].ToString())))
                                                veiculo.Renavam = Utilidades.String.OnlyNumbers(linha["Renavan"].ToString());

                                            bool situacaoAnterior = veiculo.Ativo;
                                            veiculo.Ativo = true;
                                            veiculo.TipoCombustivel = "D";
                                            if (!string.IsNullOrWhiteSpace(linha["Cidade emplacamento"].ToString()))
                                                veiculo.Estado = new Dominio.Entidades.Estado() { Sigla = linha["Cidade emplacamento"].ToString().Split('-')[1].Trim() };
                                            else
                                                veiculo.Estado = new Dominio.Entidades.Estado() { Sigla = "SC" };

                                            if (!string.IsNullOrWhiteSpace(linha["Tara(Kg)"].ToString()))
                                                veiculo.Tara = int.Parse(linha["Tara(Kg)"].ToString());
                                            else
                                                veiculo.Tara = 0;

                                            veiculo.ValorAquisicao = 0;
                                            veiculo.CapacidadeTanque = 0;
                                            if (!string.IsNullOrWhiteSpace(linha["Cap#Carga(M3)"].ToString()))
                                                veiculo.CapacidadeM3 = int.Parse(linha["Cap#Carga(M3)"].ToString());
                                            else
                                                veiculo.CapacidadeM3 = 0;
                                            if (!string.IsNullOrWhiteSpace(linha["Chassi"].ToString()) && linha["Chassi"].ToString().Length > 20)
                                                veiculo.Chassi = linha["Chassi"].ToString().Substring(0, 20);
                                            else
                                                veiculo.Chassi = linha["Chassi"].ToString();
                                            veiculo.KilometragemAtual = 0;
                                            if (!string.IsNullOrWhiteSpace(linha["Ano#Fab#"].ToString()))
                                                veiculo.AnoFabricacao = int.Parse(linha["Ano#Fab#"].ToString());
                                            else
                                                veiculo.AnoFabricacao = 0;
                                            veiculo.AnoModelo = 0;
                                            veiculo.NumeroMotor = linha["Nr#Motor"].ToString();
                                            veiculo.MediaPadrao = 0;
                                            veiculo.Observacao = linha["Gestor"].ToString() +
                                                (string.IsNullOrWhiteSpace(linha["Tipo objeto"].ToString()) ? string.Empty : " Tipo: " + linha["Tipo objeto"].ToString()) +
                                                (string.IsNullOrWhiteSpace(linha["Marca rastreador"].ToString()) ? string.Empty : " Rastreador: " + linha["Marca rastreador"].ToString() + " - " + linha["Nr#Rastreador"].ToString()) +
                                                " Proprietário " + linha["Proprietario"].ToString();

                                            if (linha["Proprio"].ToString() == "Terceiro")
                                                veiculo.Tipo = "T";
                                            else
                                                veiculo.Tipo = "P";

                                            if (linha["Tipo objeto"].ToString() == "Cavalo")
                                                veiculo.TipoVeiculo = "0";
                                            else
                                                veiculo.TipoVeiculo = "1";

                                            /// <summary>
                                            /// 00 - NAO APLICADO
                                            /// 01 - TRUCK
                                            /// 02 - TOCO
                                            /// 03 - CAVALO
                                            /// 04 - VAN
                                            /// 05 - UTILITARIO
                                            /// 06 - OUTROS

                                            /// 00 - NAO APLICADO
                                            /// 01 - ABERTA
                                            /// 02 - FECHADA / BAU
                                            /// 03 - GRANEL
                                            /// 04 - PORTA CONTAINER
                                            /// 05 - SIDER

                                            if (linha["Tipo objeto"].ToString() == "Cavalo")
                                            {
                                                veiculo.TipoRodado = "03";
                                                veiculo.TipoCarroceria = "00";
                                            }
                                            else
                                            {
                                                veiculo.TipoRodado = "00";
                                                veiculo.TipoCarroceria = "02";
                                            }

                                            if (!string.IsNullOrWhiteSpace(linha["Lotacao(Kg)"].ToString()))
                                                veiculo.CapacidadeKG = int.Parse(linha["Lotacao(Kg)"].ToString());
                                            else
                                                veiculo.CapacidadeKG = 0;

                                            if (!string.IsNullOrWhiteSpace(linha["Frota"].ToString()) && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(linha["Frota"].ToString())))
                                                veiculo.NumeroFrota = linha["Frota"].ToString();
                                            else
                                                veiculo.NumeroFrota = "";

                                            Dominio.Entidades.ModeloVeiculo modeloVeiculo = repModeloVeiculo.BuscarPorDescricao(linha["Modelo"].ToString(), 0);
                                            if (modeloVeiculo != null)
                                            {
                                                veiculo.Marca = modeloVeiculo.MarcaVeiculo;
                                                veiculo.Modelo = modeloVeiculo;
                                            }
                                            else
                                            {
                                                modeloVeiculo = new Dominio.Entidades.ModeloVeiculo();
                                                modeloVeiculo.Descricao = linha["Modelo"].ToString();
                                                modeloVeiculo.NumeroEixo = 0;
                                                modeloVeiculo.Status = "A";

                                                Dominio.Entidades.MarcaVeiculo marcaVeiculo = repMarcaVeiculo.BuscarPorDescricao(linha["Modelo"].ToString().Split(' ')[0], 0);
                                                if (marcaVeiculo == null)
                                                {
                                                    marcaVeiculo = new Dominio.Entidades.MarcaVeiculo();
                                                    marcaVeiculo.Descricao = linha["Modelo"].ToString().Split(' ')[0];
                                                    marcaVeiculo.Status = "A";
                                                    if (linha["Tipo objeto"].ToString() == "Cavalo")
                                                        marcaVeiculo.TipoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao;
                                                    else
                                                        marcaVeiculo.TipoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque;
                                                    repMarcaVeiculo.Inserir(marcaVeiculo);
                                                }

                                                modeloVeiculo.MarcaVeiculo = marcaVeiculo;
                                                repModeloVeiculo.Inserir(modeloVeiculo);

                                                veiculo.Modelo = modeloVeiculo;
                                                veiculo.Marca = marcaVeiculo;
                                            }

                                            //string cpfMotorista = long.Parse(linha["CPF"].ToString()).ToString("d11");
                                            //Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCPF(0, cpfMotorista, "M");
                                            //if (motorista == null)
                                            //{
                                            //    motorista = new Dominio.Entidades.Usuario();
                                            //    motorista.CPF = Utilidades.String.OnlyNumbers(cpfMotorista);
                                            //    motorista.Nome = linha["MOTORISTA"].ToString();
                                            //    motorista.Localidade = veiculo.Empresa.Localidade;
                                            //    motorista.Empresa = veiculo.Empresa;
                                            //    motorista.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };
                                            //    motorista.Tipo = "M";
                                            //    motorista.Status = "A";
                                            //    repUsuario.Inserir(motorista);
                                            //}
                                            //veiculo.Motorista = motorista;
                                            //if (veiculo.VeiculosVinculados != null)
                                            //{
                                            //    veiculo.VeiculosVinculados.Clear();
                                            //}
                                            //else
                                            //{
                                            //    veiculo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                                            //}

                                            //string placaCarreta = linha["PLACACARRETA"].ToString().Replace("-", "");

                                            //if(!string.IsNullOrWhiteSpace(placaCarreta) && placaCarreta != "NULL")
                                            //{
                                            //    Dominio.Entidades.Veiculo carreta = repVeiculo.BuscarPorPlaca(empresa.Codigo, placaCarreta);

                                            //    bool inserirCarreta = false;
                                            //    if (carreta == null)
                                            //    {
                                            //        inserirCarreta = true;
                                            //        carreta = new Dominio.Entidades.Veiculo();
                                            //    }

                                            //    carreta.Empresa = empresa;
                                            //    carreta.ModeloVeicularCarga = modeloVeicular;
                                            //    Dominio.Entidades.Localidade localidadeCarrreta = repLocalidade.BuscarPorCodigoIBGE(int.Parse(linha["IBGECARRETA"].ToString()));
                                            //    carreta.Estado = new Dominio.Entidades.Estado() { Sigla = localidadeCarrreta.Estado.Sigla };
                                            //    carreta.Placa = placaCarreta;
                                            //    carreta.Renavam = linha["RENAVAMCARRETA"].ToString();
                                            //    carreta.Status = "A";
                                            //    carreta.Tipo = "P";
                                            //    carreta.TipoCarroceria = "02";
                                            //    carreta.TipoCombustivel = "D";
                                            //    carreta.TipoRodado = "00";
                                            //    carreta.TipoVeiculo = "1";
                                            //    carreta.CapacidadeKG = int.Parse(linha["LOTACAOCARRETA"].ToString());
                                            //    carreta.Tara = int.Parse(linha["TARACARRETA"].ToString());

                                            //    if (inserirCarreta)
                                            //    {
                                            //        repVeiculo.Inserir(carreta);
                                            //    }
                                            //    else
                                            //    {
                                            //        repVeiculo.Atualizar(carreta);
                                            //    }

                                            //    veiculo.VeiculosVinculados.Add(carreta);
                                            //}

                                            if (inserir)
                                            {
                                                repVeiculo.Inserir(veiculo);
                                            }
                                            else
                                            {
                                                Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculo, situacaoAnterior, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MetodosAlteracaoVeiculo.ImportarVeiculos_ImportacaoBennerController, this.Usuario, unitOfWork);

                                                repVeiculo.Atualizar(veiculo, Auditado);
                                            }

                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }

                    sbErro.Append("Terminou importação");
                    string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Veiculo.txt");

                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                    ViewBag.Retorno = "Terminou";


                }
                return View();
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarMotoristas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                System.Text.StringBuilder sbErro = new System.Text.StringBuilder();
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

                DataSet ds = new DataSet();
                if (customFile.Length > 0)
                {
                    string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {

                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                        {
                            Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                        }
                        customFile.SaveAs(fileLocation);
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                        try
                        {

                            excelConnection.Open();
                            DataTable dt = new DataTable();

                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            if (dt == null)
                            {
                                return null;
                            }

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                            dataAdapter.Fill(ds);

                            int indiceLinha = 1;
                            foreach (DataRow linha in ds.Tables[0].Rows)
                            {
                                indiceLinha++;
                                try
                                {
                                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarMotoristaPorCPF(Utilidades.String.OnlyNumbers(linha["CPF/CNPJ"].ToString()));
                                    bool inserir = false;
                                    if (usuario == null)
                                    {
                                        inserir = true;
                                        usuario = new Dominio.Entidades.Usuario();
                                    }

                                    usuario.CPF = Utilidades.String.OnlyNumbers(linha["CPF/CNPJ"].ToString());

                                    if (usuario.Setor == null)
                                        usuario.Setor = repSetor.BuscarPorCodigo(1);

                                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(linha["CEP"].ToString())))
                                    {
                                        Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCEP(Utilidades.String.OnlyNumbers(linha["CEP"].ToString()).Substring(0, 3));
                                        if (localidade == null && !string.IsNullOrWhiteSpace(linha["Estado"].ToString()))
                                        {
                                            localidade = repLocalidade.BuscarPorEstado(linha["Estado"].ToString());
                                        }
                                        if (localidade != null)
                                            usuario.Localidade = localidade;
                                        else
                                            usuario.Localidade = repLocalidade.BuscarPorCodigo(77950);
                                    }
                                    else
                                        usuario.Localidade = repLocalidade.BuscarPorCodigo(77950);
                                    usuario.Nome = linha["Razao social"].ToString();
                                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(linha["Telefone"].ToString())) && Utilidades.String.OnlyNumbers(linha["Telefone"].ToString()).Length > 20)
                                        usuario.Telefone = Utilidades.String.OnlyNumbers(linha["Telefone"].ToString()).Substring(0, 20);
                                    else
                                        usuario.Telefone = Utilidades.String.OnlyNumbers(linha["Telefone"].ToString());
                                    usuario.Salario = 0;
                                    usuario.Tipo = "M";
                                    usuario.Endereco = linha["Endereco"].ToString();
                                    usuario.Complemento = linha["Complemento"].ToString();
                                    if (linha["Status"].ToString() == "Sim")
                                        usuario.Status = "A";
                                    else
                                        usuario.Status = "I";
                                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                                    usuario.PercentualComissao = 0;
                                    usuario.Bairro = linha["Bairro"].ToString();
                                    usuario.CEP = Utilidades.String.OnlyNumbers(linha["CEP"].ToString());
                                    usuario.UsuarioAdministrador = false;
                                    usuario.NumeroEndereco = linha["Numero"].ToString();
                                    usuario.Latitude = linha["Latitude"].ToString();
                                    usuario.Longitude = linha["Longitude"].ToString();
                                    usuario.TipoLogradouro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro.Rua;
                                    usuario.EnderecoDigitado = true;
                                    usuario.TipoEmail = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Principal;
                                    usuario.TipoEndereco = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEndereco.Residencial;

                                    if (inserir)
                                    {
                                        repUsuario.Inserir(usuario);
                                    }
                                    else
                                    {
                                        repUsuario.Atualizar(usuario);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                        }
                        finally
                        {
                            excelConnection.Close();
                        }
                    }

                    sbErro.Append("Terminou importação");
                    string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Motorista.txt");
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                    ViewBag.Retorno = "Terminou";


                }
                return View();
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarLocalidades()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

            Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

            DataSet ds = new DataSet();
            if (customFile.Length > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {

                    string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                    if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                    {
                        Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                    }
                    customFile.SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                    try
                    {

                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                        dataAdapter.Fill(ds);

                        int indiceLinha = 1;


                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                        Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                        Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                        foreach (DataRow linha in ds.Tables[0].Rows)
                        {
                            unitOfWork.Start();

                            indiceLinha++;
                            try
                            {
                                string codigoPoloEmbarcador = linha["codigoPolo"].ToString();
                                string ibgePolo = linha["ibgPolo"].ToString();

                                Dominio.Entidades.Localidade localidadePolo = null;

                                if (!string.IsNullOrWhiteSpace(codigoPoloEmbarcador))
                                {
                                    var inserirPolo = false;

                                    if (ibgePolo == "0" || ibgePolo == "1111111" || ibgePolo == "1234567")
                                        ibgePolo = "9999999";

                                    if (!string.IsNullOrWhiteSpace(ibgePolo) && ibgePolo != "9999999")
                                        localidadePolo = repLocalidade.BuscarPorCodigoIBGE(int.Parse(ibgePolo));


                                    if (localidadePolo == null)
                                        localidadePolo = repLocalidade.buscarPorCodigoEmbarcador(codigoPoloEmbarcador);


                                    if (localidadePolo == null)
                                    {
                                        localidadePolo = new Dominio.Entidades.Localidade();
                                        localidadePolo.Codigo = repLocalidade.BuscarPorMaiorCodigo();
                                        localidadePolo.Codigo++;
                                        inserirPolo = true;
                                    }
                                    localidadePolo.Descricao = linha["CidadePolo"].ToString();

                                    if (inserirPolo)
                                    {
                                        localidadePolo.CodigoLocalidadeEmbarcador = codigoPoloEmbarcador;
                                        if (ibgePolo == "9999999")
                                        {
                                            localidadePolo.CodigoIBGE = 9999999;
                                            localidadePolo.CEP = "";
                                            localidadePolo.Estado = repEstado.BuscarPorSigla("EX");
                                            if (!string.IsNullOrWhiteSpace(linha["PaisPolo"].ToString()))
                                            {
                                                int codPais = int.Parse(linha["PaisPolo"].ToString());
                                                localidadePolo.Pais = repPais.BuscarPorCodigo(codPais);
                                            }
                                            repLocalidade.Inserir(localidadePolo);
                                        }
                                        else
                                        {
                                            localidadePolo = null;
                                        }
                                    }
                                }

                                string codigoEmbarcador = linha["Codigo"].ToString();
                                string ibge = linha["ibge"].ToString();

                                if (!string.IsNullOrWhiteSpace(codigoEmbarcador))
                                {
                                    var inserir = false;
                                    Dominio.Entidades.Localidade localidade = null;

                                    if (ibge == "0" || ibge == "1111111" || ibge == "1234567")
                                        ibge = "9999999";

                                    if (!string.IsNullOrWhiteSpace(ibge) && ibge != "9999999")
                                        localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(ibge));


                                    if (localidade == null)
                                        localidade = repLocalidade.buscarPorCodigoEmbarcador(codigoEmbarcador);


                                    if (localidade == null)
                                    {
                                        localidade = new Dominio.Entidades.Localidade();
                                        localidade.Codigo = repLocalidade.BuscarPorMaiorCodigo();
                                        localidade.Codigo++;
                                        inserir = true;
                                    }
                                    localidade.Descricao = linha["Cidade"].ToString();

                                    if (!string.IsNullOrWhiteSpace(localidade.Descricao))
                                    {
                                        localidade.CodigoLocalidadeEmbarcador = codigoEmbarcador;
                                        localidade.LocalidadePolo = localidadePolo;
                                        if (ibge == "9999999")
                                        {
                                            localidade.CodigoIBGE = 9999999;
                                            localidade.CEP = "";
                                            localidade.Estado = repEstado.BuscarPorSigla("EX");
                                            if (!string.IsNullOrWhiteSpace(linha["Pais"].ToString()))
                                            {
                                                int codPais = int.Parse(linha["Pais"].ToString());
                                                localidade.Pais = repPais.BuscarPorCodigo(codPais);
                                            }
                                        }
                                        else
                                        {
                                            if (localidade.Estado != null)
                                                localidade.Estado = repEstado.BuscarPorSigla(localidade.Estado.Sigla);
                                        }
                                        if (inserir)
                                        {
                                            if (ibge == "9999999")
                                            {
                                                repLocalidade.Inserir(localidade);
                                                unitOfWork.CommitChanges();
                                            }
                                            else
                                            {
                                                unitOfWork.Rollback();
                                            }
                                        }
                                        else
                                        {
                                            repLocalidade.Atualizar(localidade);
                                            unitOfWork.CommitChanges();
                                        }
                                    }


                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append("Informe o codigo da localidade no embarcador").AppendLine("");
                                }
                            }
                            catch (Exception ex)
                            {
                                unitOfWork.Rollback();
                                sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                    }
                    finally
                    {
                        excelConnection.Close();
                    }
                }

                sbErro.Append("Terminou importação");
                string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Localidades.txt");
                Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                ViewBag.Retorno = "Terminou";


            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportarPessoas()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

            Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

            DataSet ds = new DataSet();
            if (customFile.Length > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {

                    string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                    if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                    {
                        Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                    }
                    customFile.SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                    try
                    {

                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                        dataAdapter.Fill(ds);

                        int indiceLinha = 1;




                        Servicos.Cliente servicoCliente = new Servicos.Cliente(_conexao.StringConexao);

                        foreach (DataRow linha in ds.Tables[0].Rows)
                        {
                            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                            unitOfWork.Start();

                            indiceLinha++;
                            try
                            {
                                Dominio.ObjetosDeValor.CTe.Cliente clienteIntegracao = new Dominio.ObjetosDeValor.CTe.Cliente();
                                clienteIntegracao.Bairro = (linha["Bairro"].ToString() != "NULL" && !string.IsNullOrWhiteSpace(linha["Bairro"].ToString())) ? linha["Bairro"].ToString() : "INDETERMINADO";

                                if (clienteIntegracao.Bairro.Length == 1)
                                    clienteIntegracao.Bairro = "INDETERMINADO";

                                if (clienteIntegracao.Bairro.Length == 2)
                                    clienteIntegracao.Bairro += "_";

                                clienteIntegracao.CEP = linha["CEP"].ToString() != "NULL" ? linha["CEP"].ToString() : "";
                                clienteIntegracao.Cidade = linha["Cidade"].ToString() != "NULL" ? linha["Cidade"].ToString().ToUpper() : "";
                                clienteIntegracao.CodigoAtividade = ConverterCodigoAtividade(linha["TipoEmpresa"].ToString());
                                clienteIntegracao.CodigoIBGECidade = (linha["IBGE"].ToString() != "NULL" && !string.IsNullOrWhiteSpace(linha["IBGE"].ToString())) ? int.Parse(linha["IBGE"].ToString()) : 0;
                                clienteIntegracao.CodigoPais = linha["CodigoPais"].ToString() != "NULL" ? linha["CodigoPais"].ToString() : "";
                                clienteIntegracao.Complemento = linha["Complemento"].ToString() != "NULL" ? linha["Complemento"].ToString().ToUpper() : "";
                                clienteIntegracao.CodigoLocalidadeEmbarcador = linha["handleCidade"].ToString() != "NULL" ? linha["handleCidade"].ToString().ToUpper() : "";
                                clienteIntegracao.CPFCNPJ = Utilidades.String.OnlyNumbers(linha["CGC"].ToString());
                                int tipoPessoa = int.Parse(linha["Tipo"].ToString());
                                if (tipoPessoa == 2)
                                {
                                    clienteIntegracao.CPFCNPJ = long.Parse(clienteIntegracao.CPFCNPJ).ToString("d14");
                                }
                                else
                                {
                                    if (tipoPessoa == 1)
                                    {
                                        clienteIntegracao.CPFCNPJ = long.Parse(clienteIntegracao.CPFCNPJ).ToString("d11");
                                    }
                                    else
                                    {
                                        clienteIntegracao.CPFCNPJ = long.Parse(clienteIntegracao.CPFCNPJ).ToString();
                                        clienteIntegracao.Exportacao = true;
                                    }
                                }

                                clienteIntegracao.Emails = linha["Email"].ToString() != "NULL" ? linha["Email"].ToString() : "";
                                clienteIntegracao.Endereco = (linha["Endereco"].ToString() != "NULL" && !string.IsNullOrWhiteSpace(linha["Endereco"].ToString())) ? linha["Endereco"].ToString() : "INDETERMINADO";
                                if (clienteIntegracao.Endereco.Length == 1)
                                    clienteIntegracao.Endereco = "INDETERMINADO";

                                if (clienteIntegracao.Endereco.Length == 2)
                                    clienteIntegracao.Endereco += "_";

                                clienteIntegracao.IM = linha["InscricaoMunicipal"].ToString() != "NULL" ? linha["InscricaoMunicipal"].ToString() : "";
                                clienteIntegracao.NomeFantasia = linha["Fantasia"].ToString() != "NULL" ? linha["Fantasia"].ToString() : "";
                                clienteIntegracao.Numero = (linha["Numero"].ToString() != "NULL" && !string.IsNullOrWhiteSpace(linha["Numero"].ToString())) ? linha["Numero"].ToString() : "S/N";
                                clienteIntegracao.RazaoSocial = linha["Razao"].ToString() != "NULL" ? linha["Razao"].ToString() : "";
                                clienteIntegracao.RGIE = linha["InscricaoEstadual"].ToString() != "NULL" ? linha["InscricaoEstadual"].ToString() : "ISENTO";
                                clienteIntegracao.Telefone1 = linha["Telefone"].ToString() != "NULL" ? linha["Telefone"].ToString() : "";
                                clienteIntegracao.Telefone2 = linha["Telefone2"].ToString() != "NULL" ? linha["Telefone2"].ToString() : "";
                                Dominio.ObjetosDeValor.Cliente cliente = new Dominio.ObjetosDeValor.Cliente();

                                if (clienteIntegracao.CodigoIBGECidade > 0)
                                {
                                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = servicoCliente.converterClienteEmbarcador(clienteIntegracao, "Cliente", unitOfWork);
                                    if (retornoConversao.Status == false)
                                    {
                                        sbErro.Append("Linha: ").Append(indiceLinha).Append(" Mensagem erro na Conversão: ").Append(clienteIntegracao.RazaoSocial).Append(" (").Append(clienteIntegracao.CPFCNPJ).Append(" )- ").Append(retornoConversao.Mensagem).AppendLine("");
                                        unitOfWork.Rollback();
                                    }
                                    else
                                    {
                                        unitOfWork.CommitChanges();
                                    }
                                }
                                else
                                {
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Mensagem erro na Conversão: ").Append(clienteIntegracao.RazaoSocial).Append(" (").Append(clienteIntegracao.CPFCNPJ).Append(" )- ").Append("Codigo IBGE do cliente não é valido").AppendLine("");
                                    unitOfWork.Rollback();
                                }

                            }
                            catch (Exception ex)
                            {
                                unitOfWork.Rollback();
                                unitOfWork.Dispose();
                                sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                    }
                    finally
                    {
                        excelConnection.Close();
                    }
                }

                sbErro.Append("Terminou importação");
                string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_Pessoas.txt");
                Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                ViewBag.Retorno = "Terminou";


            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportarOutroEnderecoCliente()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            System.Text.StringBuilder sbErro = new System.Text.StringBuilder();

            Servicos.DTO.CustomFile customFile = HttpContext.GetFile("file");

            DataSet ds = new DataSet();
            if (customFile.Length > 0)
            {
                string fileExtension = System.IO.Path.GetExtension(customFile.FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {

                    string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid().ToString() + fileExtension);
                    if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                    {
                        Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                    }
                    customFile.SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                    try
                    {

                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                        dataAdapter.Fill(ds);

                        int indiceLinha = 1;

                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                        Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
                        Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                        Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

                        Servicos.Cliente servicoCliente = new Servicos.Cliente(_conexao.StringConexao);

                        foreach (DataRow linha in ds.Tables[0].Rows)
                        {
                            unitOfWork.Start();

                            indiceLinha++;
                            try
                            {
                                double cpfCnpj = 0;
                                if (double.TryParse(Utilidades.String.OnlyNumbers(linha["CGC"].ToString()), out cpfCnpj))
                                {

                                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);
                                    if (cliente != null)
                                    {

                                        Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigoEmbarcador(linha["HANDLE"].ToString(), 0);
                                        bool inserir = false;
                                        if (clienteOutroEndereco == null)
                                        {
                                            clienteOutroEndereco = new Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco();
                                            inserir = true;
                                        }
                                        clienteOutroEndereco.Cliente = cliente;
                                        clienteOutroEndereco.Bairro = (linha["BAIRRO"].ToString() != "NULL" && !string.IsNullOrWhiteSpace(linha["BAIRRO"].ToString())) ? linha["BAIRRO"].ToString() : "INDETERMINADO";

                                        if (clienteOutroEndereco.Bairro.Length == 1)
                                            clienteOutroEndereco.Bairro = "INDETERMINADO";

                                        if (clienteOutroEndereco.Bairro.Length == 2)
                                            clienteOutroEndereco.Bairro += "_";

                                        clienteOutroEndereco.CEP = linha["CEP"].ToString() != "NULL" ? linha["CEP"].ToString() : "";
                                        clienteOutroEndereco.CodigoEmbarcador = linha["HANDLE"].ToString();
                                        clienteOutroEndereco.Complemento = linha["COMPLEMENTO"].ToString() != "NULL" ? linha["COMPLEMENTO"].ToString() : "";

                                        clienteOutroEndereco.Endereco = (linha["ENDERECO"].ToString() != "NULL" && !string.IsNullOrWhiteSpace(linha["ENDERECO"].ToString())) ? linha["ENDERECO"].ToString() : "INDETERMINADO";
                                        if (clienteOutroEndereco.Endereco.Length == 1)
                                            clienteOutroEndereco.Endereco = "INDETERMINADO";

                                        if (clienteOutroEndereco.Endereco.Length == 2)
                                            clienteOutroEndereco.Endereco += "_";

                                        clienteOutroEndereco.Numero = (linha["NUMERO"].ToString() != "NULL" && !string.IsNullOrWhiteSpace(linha["NUMERO"].ToString())) ? linha["NUMERO"].ToString() : "S/N";
                                        clienteOutroEndereco.Telefone = linha["TELEFONE"].ToString() != "NULL" ? linha["TELEFONE"].ToString() : "";

                                        int IBGE = (linha["IBGE"].ToString() != "NULL" && !string.IsNullOrWhiteSpace(linha["IBGE"].ToString())) ? int.Parse(linha["IBGE"].ToString()) : 0;
                                        if (IBGE > 0)
                                        {
                                            if (IBGE != 9999999)
                                            {
                                                clienteOutroEndereco.Localidade = repLocalidade.BuscarPorCodigoIBGE(IBGE);
                                            }
                                            else
                                            {
                                                string handleEnderecoEmbarcador = linha["HANDLECIDADE"].ToString();
                                                if (!string.IsNullOrWhiteSpace(handleEnderecoEmbarcador))
                                                {
                                                    Dominio.Entidades.Localidade localidadeExt = repLocalidade.buscarPorCodigoEmbarcador(handleEnderecoEmbarcador);
                                                    if (localidadeExt == null)
                                                    {
                                                        localidadeExt = new Dominio.Entidades.Localidade();
                                                        localidadeExt.CEP = "";
                                                        localidadeExt.Codigo = repLocalidade.BuscarPorMaiorCodigo();
                                                        localidadeExt.Codigo++;
                                                        localidadeExt.Descricao = linha["CIDADE"].ToString();
                                                        localidadeExt.Estado = new Dominio.Entidades.Estado() { Sigla = "EX" };
                                                        localidadeExt.CodigoIBGE = 9999999;
                                                        localidadeExt.CodigoLocalidadeEmbarcador = handleEnderecoEmbarcador;
                                                        if (!string.IsNullOrWhiteSpace(linha["CODIGOPAIS"].ToString()))
                                                        {
                                                            localidadeExt.Pais = repPais.BuscarPorCodigo(int.Parse(linha["CODIGOPAIS"].ToString()));
                                                            repLocalidade.Inserir(localidadeExt);
                                                        }
                                                        else
                                                        {
                                                            localidadeExt = null;
                                                            sbErro.Append("Linha: ").Append(indiceLinha).Append(" Mensagem erro na Conversão: ").Append("Código do país não foi localizado no Multi Embarcador; ").AppendLine("");
                                                        }
                                                    }
                                                    clienteOutroEndereco.Localidade = localidadeExt;
                                                }
                                                else
                                                {
                                                    clienteOutroEndereco.Localidade = repLocalidade.BuscarPorCodigoIBGE(9999999);
                                                }
                                            }

                                            if (clienteOutroEndereco.Localidade != null)
                                            {
                                                if (inserir)
                                                    repClienteOutroEndereco.Inserir(clienteOutroEndereco);
                                                else
                                                    repClienteOutroEndereco.Atualizar(clienteOutroEndereco);

                                                unitOfWork.CommitChanges();
                                            }
                                            else
                                            {
                                                unitOfWork.Rollback();
                                                sbErro.Append("Linha: ").Append(indiceLinha).Append(" Mensagem erro na Conversão: ").Append("Localidade não encontrada no Multi Embarcador; ").AppendLine("");
                                            }
                                        }
                                        else
                                        {
                                            sbErro.Append("Linha: ").Append(indiceLinha).Append(" Mensagem erro na Conversão: ").Append("Codigo IBGE não é valido").AppendLine("");
                                            unitOfWork.Rollback();
                                        }
                                    }
                                    else
                                    {
                                        unitOfWork.Rollback();
                                        sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append("Cliente não encontrado").AppendLine("");
                                    }
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append("CGC Inválido").AppendLine("");
                                }
                            }
                            catch (Exception ex)
                            {
                                unitOfWork.Rollback();
                                sbErro.Append("Linha: ").Append(indiceLinha).Append(" Erro: ").Append(ex.Message.ToString()).AppendLine("");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        sbErro.Append("Erro ao acessar os dados: " + ex.Message).AppendLine("");
                    }
                    finally
                    {
                        excelConnection.Close();
                    }
                }

                sbErro.Append("Terminou importação");
                string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_OutroEndereco.txt");
                Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, sbErro.ToString());

                ViewBag.Retorno = "Terminou";


            }
            return View();
        }

        private int ConverterCodigoAtividade(string atividadeBenner)
        {
            int atividade = 2; //padrão industria

            if (atividadeBenner == "C" || atividadeBenner == "M" || atividadeBenner == "A")
                atividade = 3;
            else
                if (atividadeBenner == "I")
                atividade = 2;
            else
                    if (atividadeBenner == "S")
                atividade = 4;


            return atividade;
        }
    }
}
