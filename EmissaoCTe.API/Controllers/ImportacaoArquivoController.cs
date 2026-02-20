using Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    #region Objeto De Valor
    public class DadosColuna
    {
        public string NomeCampo { get; set; }
        public string Valor { get; set; }
        public bool Aspas { get; set; }
    }
    public class DadosLinha
    {
        public List<DadosColuna> Colunas { get; set; }
    }
    public class DadosImportacao
    {
        public string TipoImportacao { get; set; }
        public List<DadosLinha> Importacao { get; set; }
    }
    #endregion

    public class ImportacaoArquivoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao(string pagina)
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals(pagina) select obj).FirstOrDefault();
        }

        private List<string> _Erros = new List<string>();
        private List<DadosLinha> _Reimportacao = new List<DadosLinha>();

        #endregion

        #region Métodos Globais
        [AcceptVerbs("POST")]
        public ActionResult ImportarInformcaoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {

                unitOfWork.Start();

                // Recebe os dados
                string dados = Request.Params["Dados"];
                string titulo = "";
                int total = 0, importados = 0;

                // Converte
                DadosImportacao jsonDados = JsonConvert.DeserializeObject<DadosImportacao>(dados);

                // Define o fluxo pra cada tipo de importacao
                if (jsonDados.TipoImportacao.Equals("Veiculos"))
                {
                    // No caso de veiculos quando informar proprietario verificar se possui cadastro como cliente
                    importados = this.ImportaVeiculos(jsonDados.Importacao, unitOfWork);
                    total = jsonDados.Importacao.Count();
                    titulo = "veículo(s)";
                }
                else if (jsonDados.TipoImportacao.Equals("Motoristas"))
                {
                    importados = this.ImportaMotoristas(jsonDados.Importacao, unitOfWork);
                    total = jsonDados.Importacao.Count();
                    titulo = "motorista(s)";
                }
                else
                {
                    return Json<bool>(false, false, "Tipo de importação inválida.");
                }

                unitOfWork.CommitChanges();

                return Json(new
                {
                    Erros = _Erros,
                    Label = titulo,
                    Importados = importados,
                    Total = total,
                    Reimportar = (_Reimportacao.Count() > 0) ? this.ConverteReimportacao(_Reimportacao) : null
                }, true);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);

                unitOfWork.Rollback();

                return Json<bool>(false, false, "Houve um erro ao converter arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConverterArquivo()
        {
            string[] extensoesValidas = { ".csv", ".xls", ".xlsx" };

            // Valida permissao
            if (this.Permissao("importacaoarquivos.aspx") == null || this.Permissao("importacaoarquivos.aspx").PermissaoDeAcesso != "A")
                return Json<bool>(false, false, "Permissão para importar arquivo negada!");

            // Valida quantidade aruivos
            if (Request.Files.Count != 1)
                return Json<bool>(false, false, "Nenhum arquivo recebido.");

            // Converte arquivo upado
            HttpPostedFileBase file = Request.Files[0];

            // Valida extensao
            string extensao = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!extensoesValidas.Contains(extensao))
                return Json<bool>(false, false, "Extensão " + extensao.Substring(1) + " inválida.");

            // Converte arquivo em json
            try
            {
                object obj = this.ConverteArquivo(file.InputStream, extensao);

                return Json(obj, true);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return Json<bool>(false, false, "Houve um erro ao converter arquivo.");
            }
        }


        [AcceptVerbs("GET", "POST")]
        public ActionResult ExportarDados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                // Valida permissao
                if (this.Permissao("exportacaodados.aspx") == null || this.Permissao("exportacaodados.aspx").PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão para exportar dados negada!");

                // Valida tipo
                string tipo = Request.Params["Tipo"] ?? string.Empty;
                if (string.IsNullOrWhiteSpace(tipo))
                    return Json<bool>(false, false, "Nenhum tipo selecionado.");

                List<List<string>> estruturaCSV = new List<List<string>>();

                // Gera a estrutura de acordo com cada dado
                switch (tipo)
                {
                    case "veiculos":
                        estruturaCSV = GeraCSVVeiculos(unitOfWork);
                        break;
                    case "motoristas":
                        estruturaCSV = GeraCSVMotoristas(unitOfWork);
                        break;
                    default:
                        break;
                }

                // Cria a string do arquivo
                string conteudoCSV = String.Join("\n", from o in estruturaCSV select String.Join(";", o).ToString());

                // Gera os bytes do arquivo
                byte[] arquivo = System.Text.Encoding.UTF8.GetBytes(conteudoCSV);

                return Arquivo(arquivo, "text/csv", "Exportacao - " + tipo + ".csv");
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);

                return Json<bool>(false, false, "Ocorreu um erro ao exportar os daods.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        private List<List<string>> GeraCSVVeiculos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarPorEmpresa(this.EmpresaUsuario.Codigo, "A");

            List<List<string>> estruturaCSV = new List<List<string>>();

            for (var i = 0; i < veiculos.Count; i++)
            {
                Dominio.Entidades.Veiculo veiculo = veiculos[i];

                estruturaCSV.Add(new List<string>() {
                    veiculo.Estado?.Sigla ?? string.Empty, // UF
                    veiculo.KilometragemAtual.ToString(), // KMAtual
                    veiculo.Placa, // Placa
                    veiculo.Chassi, // Chassi
                    veiculo.AnoFabricacao.ToString(), // Ano
                    veiculo.AnoModelo.ToString(), // AnoModelo
                    veiculo.Renavam, // Renavam
                    veiculo.Tara.ToString(), // Tara
                    veiculo.Tipo, // Tipo
                    veiculo.TipoVeiculo, // TipoVeiculo
                    veiculo.TipoRodado, // TipoRodado
                    veiculo.TipoCarroceria, // TipoCarroceria
                    veiculo.CapacidadeKG.ToString(), // CapKG
                    veiculo.CapacidadeM3.ToString(), // CapM3
                    veiculo.Observacao, // Observacao
                    veiculo.Motoristas?.FirstOrDefault()?.CPF, // CPFMotorista
                    veiculo.Motoristas?.FirstOrDefault()?.Nome, // NomeMotorista
                    veiculo.Proprietario?.CPF_CNPJ_SemFormato ?? string.Empty, // CNPJCPFProprietario
                    veiculo.RNTRC.ToString(), // RNTRCProprietaro
                    veiculo.TipoProprietario.ToString("D") // TipoProprietario
                });
            }

            return estruturaCSV;
        }

        private List<List<string>> GeraCSVMotoristas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            List<Dominio.Entidades.Usuario> motoristas = repUsuario.BuscarMotoristasPorEmpresa(this.EmpresaUsuario.Codigo);

            List<List<string>> estruturaCSV = new List<List<string>>();

            for (var i = 0; i < motoristas.Count; i++)
            {
                Dominio.Entidades.Usuario motorista = motoristas[i];

                estruturaCSV.Add(new List<string>() {
                    Utilidades.String.RemoveAllSpecialCharacters(motorista.Nome), // Nome 
                    motorista.CPF, // CPF 
                    motorista.Localidade != null ? motorista.Localidade.CodigoIBGE.ToString() : "", // IBGE
                    motorista.NumeroHabilitacao //CNH 
                });
            }

            return estruturaCSV;
        }

        private object ConverteReimportacao(List<DadosLinha> linhas)
        {
            List<List<string>> novoJson = new List<List<string>>();

            for (var i = 0; i < linhas.Count(); i++)
            {
                List<string> novaLinha = new List<string>();

                for (var j = 0; j < linhas[i].Colunas.Count(); j++)
                    novaLinha.Add(linhas[i].Colunas[j].Valor);

                novoJson.Add(novaLinha);
            }

            return novoJson;
        }

        #region ArquivosOffice

        private object ConverteArquivo(Stream stream, string extensao)
        {
            // Remove o ponto
            extensao = extensao.Substring(1);

            // Chama o metodo referente a extensao
            if (extensao.Equals("csv"))
                return this.ConverteCSV(stream);
            else if (extensao.Equals("xls"))
                return this.ConverteXLS(stream);
            else if (extensao.Equals("xlsx"))
                return this.ConverteXLSX(stream);

            return "";
        }
        private object ConverteCSV(Stream stream)
        {
            StreamReader csvreader = new StreamReader(stream);

            var linhas = new List<List<String>>();
            int j = 0;

            while (!csvreader.EndOfStream)
            {
                var line = csvreader.ReadLine();
                var values = line.Split(';');
                List<String> celulas = new List<string>();

                for (j = 0; j < values.Length; j++)
                {
                    celulas.Add(values[j]);
                }

                linhas.Add(celulas);
            }

            csvreader.Close();

            return linhas;
        }

        private object ConverteXLS(Stream stream)
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            return this.ArquivosOffice(excelReader);
        }

        private object ConverteXLSX(Stream stream)
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            return this.ArquivosOffice(excelReader);
        }

        private object ArquivosOffice(IExcelDataReader excelReader)
        {
            var linhas = new List<List<String>>();

            while (excelReader.Read())
            {
                int numeroDeCelulas = excelReader.FieldCount;
                List<String> celulas = new List<string>();

                for (var j = 0; j < numeroDeCelulas; j++)
                {
                    string cell = excelReader.GetString(j);
                    celulas.Add(!string.IsNullOrWhiteSpace(cell) ? cell : "");
                }

                linhas.Add(celulas);
            }

            excelReader.Close();

            return linhas;
        }
        #endregion

        #region Métodos de Importacao
        private int ImportaVeiculos(List<DadosLinha> Dados, Repositorio.UnitOfWork unitOfWork)
        {
            // Percorre todas as linhas e salva as informacoes pra cadastrar cliente
            // Se o tipo e realmente T, entao insereCliente recebe true, para insererir, senao, descatar informacoes
            int inserido = 0;

            // Repositorios
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            for (var i = 0; i < Dados.Count(); i++)
            {
                DadosLinha linha = Dados[i];
                double CNPJCPFProprietario = 0;
                string placa = "";
                bool tipoProprietario = false;
                bool inserir = true;
                string cpfMotorista = string.Empty;
                string nomeMotorista = string.Empty;

                // Busca a placa
                for (var j = 0; j < linha.Colunas.Count() && string.IsNullOrEmpty(placa); j++)
                    if (linha.Colunas[j].NomeCampo.Equals("Placa"))
                        placa = linha.Colunas[j].Valor;

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(this.EmpresaUsuario.Codigo, placa);

                if (veiculo == null)
                    veiculo = new Dominio.Entidades.Veiculo();

                try
                {
                    // Preenche as informacoes importadas
                    for (var j = 0; j < linha.Colunas.Count(); j++)
                    {
                        DadosColuna coluna = linha.Colunas[j];

                        if (coluna.NomeCampo.Equals("UF"))
                            veiculo.Estado = repEstado.BuscarPorSigla(coluna.Valor);
                        else if (coluna.NomeCampo.Equals("KMAtual"))
                        {
                            int KilometragemAtual;
                            int.TryParse(coluna.Valor, out KilometragemAtual);
                            veiculo.KilometragemAtual = KilometragemAtual;
                        }
                        else if (coluna.NomeCampo.Equals("Placa"))
                            veiculo.Placa = coluna.Valor.Trim().Replace("-", "");
                        else if (coluna.NomeCampo.Equals("Chassi"))
                            veiculo.Chassi = coluna.Valor;
                        else if (coluna.NomeCampo.Equals("Ano"))
                        {
                            int AnoFabricacao;
                            int.TryParse(coluna.Valor, out AnoFabricacao);
                            veiculo.AnoFabricacao = AnoFabricacao;
                        }
                        else if (coluna.NomeCampo.Equals("AnoModelo"))
                        {
                            int AnoModelo;
                            int.TryParse(coluna.Valor, out AnoModelo);
                            veiculo.AnoModelo = AnoModelo;
                        }
                        else if (coluna.NomeCampo.Equals("Renavam"))
                            veiculo.Renavam = coluna.Valor;
                        else if (coluna.NomeCampo.Equals("Tara"))
                        {
                            int Tara;
                            int.TryParse(coluna.Valor, out Tara);
                            veiculo.Tara = Tara;
                        }
                        else if (coluna.NomeCampo.Equals("Tipo"))
                            veiculo.Tipo = coluna.Valor;
                        else if (coluna.NomeCampo.Equals("TipoVeiculo"))
                            veiculo.TipoVeiculo = string.Format("{0:0}", int.Parse(coluna.Valor));
                        else if (coluna.NomeCampo.Equals("TipoRodado"))
                            veiculo.TipoRodado = string.Format("{0:00}", int.Parse(coluna.Valor));
                        else if (coluna.NomeCampo.Equals("TipoCarroceria"))
                            veiculo.TipoCarroceria = string.Format("{0:00}", int.Parse(coluna.Valor));
                        else if (coluna.NomeCampo.Equals("CapKG"))
                        {
                            int CapacidadeKG;
                            int.TryParse(coluna.Valor, out CapacidadeKG);
                            veiculo.CapacidadeKG = CapacidadeKG;
                        }
                        else if (coluna.NomeCampo.Equals("CapM3"))
                        {
                            int CapacidadeM3;
                            int.TryParse(coluna.Valor, out CapacidadeM3);
                            veiculo.CapacidadeM3 = CapacidadeM3;
                        }
                        else if (coluna.NomeCampo.Equals("Observacao"))
                            veiculo.Observacao = coluna.Valor;
                        else if (coluna.NomeCampo.Equals("CPFMotorista"))
                        {
                            cpfMotorista = Utilidades.String.OnlyNumbers(coluna.Valor).Trim();
                            if (!Utilidades.Validate.ValidarCPF(cpfMotorista))
                                cpfMotorista = string.Empty;
                        }
                        else if (coluna.NomeCampo.Equals("NomeMotorista"))
                        {
                            nomeMotorista = coluna.Valor;
                        }
                        else if (coluna.NomeCampo.Equals("CNPJCPFProprietario"))
                        {
                            double.TryParse(coluna.Valor, out CNPJCPFProprietario);
                        }
                        else if (coluna.NomeCampo.Equals("RNTRCProprietaro"))
                        {
                            int RNTRC;
                            int.TryParse(coluna.Valor, out RNTRC);
                            veiculo.RNTRC = RNTRC;
                        }
                        else if (coluna.NomeCampo.Equals("TipoProprietario"))
                        {
                            Dominio.Enumeradores.TipoProprietarioVeiculo tipo;
                            Enum.TryParse<Dominio.Enumeradores.TipoProprietarioVeiculo>(coluna.Valor, out tipo);
                            veiculo.TipoProprietario = tipo;
                            tipoProprietario = true;
                        }
                    }

                    // Informacoes fixas
                    bool situacaoAnterior = veiculo.Ativo;
                    veiculo.Ativo = true;
                    veiculo.Empresa = this.EmpresaUsuario;


                    // Informaçoes padroes
                    if (veiculo.Codigo == 0)
                    {
                        if (veiculo.Tipo == null)
                            veiculo.Tipo = "P";

                        if (!tipoProprietario)
                            veiculo.TipoProprietario = Dominio.Enumeradores.TipoProprietarioVeiculo.TACAgregado;

                        if (veiculo.Estado == null)
                            veiculo.Estado = this.EmpresaUsuario.Localidade.Estado;
                    }

                    // Se proprietario for como terceiro, busca o proprietario
                    if (veiculo.Tipo.Equals("T"))
                    {
                        Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(CNPJCPFProprietario);

                        if (cliente == null)
                        {
                            _Erros.Add("O cliente do CNPJ " + CNPJCPFProprietario.ToString() + " não está cadastrado.");
                            _Reimportacao.Add(linha);
                            inserir = false;
                        }
                        else
                            veiculo.Proprietario = cliente;
                    }

                    if (inserir)
                    {
                        if (veiculo.Codigo > 0)
                        {
                            Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculo, situacaoAnterior, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MetodosAlteracaoVeiculo.ImportaVeiculos_ImportacaoArquivoController, this.Usuario, unitOfWork);
                            repVeiculo.Atualizar(veiculo);
                        }
                        else
                            repVeiculo.Inserir(veiculo);

                        if (!string.IsNullOrWhiteSpace(cpfMotorista))
                        {
                            Dominio.Entidades.Usuario motora = repUsuario.BuscarMotoristaPorCPF(this.EmpresaUsuario.Codigo, cpfMotorista);
                            if (motora == null)
                            {
                                motora = new Dominio.Entidades.Usuario()
                                {
                                    Nome = cpfMotorista,
                                    CPF = nomeMotorista,
                                    Empresa = this.EmpresaUsuario,
                                    Setor = repSetor.BuscarPorCodigo(1),
                                    Tipo = "M",
                                    Status = "A",
                                    TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao,
                                    Localidade = this.EmpresaUsuario.Localidade,
                                };

                                repUsuario.Inserir(motora);
                            }

                            if (motora != null)
                            {
                                Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista motoristaPrincipal = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);
                                bool inserirMotoristaPrincipal = false;
                                if (motoristaPrincipal == null)
                                {
                                    motoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista();
                                    inserirMotoristaPrincipal = true;
                                }

                                motoristaPrincipal.Principal = true;
                                motoristaPrincipal.Motorista = motora;
                                motoristaPrincipal.Nome = motora.Nome;
                                motoristaPrincipal.CPF = motora.CPF;
                                motoristaPrincipal.Veiculo = veiculo;

                                if (inserirMotoristaPrincipal)
                                    repVeiculoMotorista.Inserir(motoristaPrincipal);
                                else
                                    repVeiculoMotorista.Atualizar(motoristaPrincipal);
                            }
                        }

                        inserido++;

                    }

                }
                catch (Exception e)
                {
                    _Erros.Add("Houve um erro ao inserir o veículo " + veiculo.Placa.ToString());
                    throw;
                }
            }

            return inserido;
        }
        private int ImportaMotoristas(List<DadosLinha> Dados, Repositorio.UnitOfWork unitOfWork)
        {
            // Percorre todas as linhas e salva as informacoes pra cadastrar cliente
            // Se o tipo e realmente T, entao insereCliente recebe true, para insererir, senao, descatar informacoes
            int inserido = 0;

            // Repositorios
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            for (var i = 0; i < Dados.Count(); i++)
            {
                DadosLinha linha = Dados[i];

                Dominio.Entidades.Usuario motorista = new Dominio.Entidades.Usuario();

                try
                {
                    // Preenche as informacoes importadas
                    for (var j = 0; j < linha.Colunas.Count(); j++)
                    {
                        DadosColuna coluna = linha.Colunas[j];

                        if (coluna.NomeCampo.Equals("Nome"))
                            motorista.Nome = coluna.Valor;
                        else if (coluna.NomeCampo.Equals("CPF"))
                            motorista.CPF = Utilidades.String.OnlyNumbers(coluna.Valor).Trim();
                        else if (coluna.NomeCampo.Equals("IBGE"))
                        {
                            int codigoIBGE = 0;
                            int.TryParse(coluna.Valor, out codigoIBGE);
                            motorista.Localidade = repLocalidade.BuscarPorCodigoIBGE(codigoIBGE);
                        }
                        else if (coluna.NomeCampo.Equals("CNH"))
                            motorista.NumeroHabilitacao = coluna.Valor;
                    }

                    // Informacoes fixass
                    motorista.Empresa = this.EmpresaUsuario;
                    motorista.Setor = repSetor.BuscarPorCodigo(1);
                    motorista.Tipo = "M";
                    motorista.Status = "A";
                    motorista.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;

                    // Informaçoes padroes
                    if (motorista.Codigo == 0)
                    {
                        if (motorista.Localidade == null)
                            motorista.Localidade = this.EmpresaUsuario.Localidade;
                    }

                    repUsuario.Inserir(motorista);
                    inserido++;
                }
                catch (Exception e)
                {
                    _Erros.Add("Houve um erro ao inserir o motorista" + motorista.Nome);
                }
            }

            return inserido;
        }

        #endregion

        #endregion
    }
}