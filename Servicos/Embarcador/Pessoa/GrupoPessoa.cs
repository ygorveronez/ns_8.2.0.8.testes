using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pessoa
{
    public class GrupoPessoa
    {
        #region Atributos

        Repositorio.UnitOfWork _unitOfWork;
        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion

        #region Construtores

        public GrupoPessoa() : this(unitOfWork: null, auditado: null) { }

        public GrupoPessoa(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public GrupoPessoa(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao Importar(List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pessoas.PessoaClassificacao repositorioPessoaClassificacao = new Repositorio.Embarcador.Pessoas.PessoaClassificacao(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
            int contador = 0;

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    _unitOfWork.FlushAndClear();
                    _unitOfWork.Start();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                    string retorno = "";
                    bool adicionarRaizesCnpj = false;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaDescricao = (from obj in linha.Colunas where obj.NomeCampo == "Descricao" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaRaizCnpj = (from obj in linha.Colunas where obj.NomeCampo == "RaizCnpj" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCodigoIntegracao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracao" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaParametrizacaoDeHorarios = (from obj in linha.Colunas where obj.NomeCampo == "ParametrizacaoDeHorarios" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaClassificacao = (from obj in linha.Colunas where obj.NomeCampo == "Classificacao" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaVendedor = (from obj in linha.Colunas where obj.NomeCampo == "ListaVendedores" select obj).FirstOrDefault();

                    if (i > 0)
                    {
                        if (!string.IsNullOrEmpty(colunaCodigoIntegracao.Valor))
                        {
                            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas existeGrupoPessoas = repositorioGrupoPessoas.BuscarPorCodigoIntegracao(colunaCodigoIntegracao.Valor);

                            if (existeGrupoPessoas != null)
                            {
                                if (colunaParametrizacaoDeHorarios?.Valor != null)
                                {
                                    bool permiteParametrizacaoDeHorarios = false;

                                    if (!string.IsNullOrWhiteSpace((((string)colunaParametrizacaoDeHorarios.Valor).ToString())))
                                        permiteParametrizacaoDeHorarios = (((string)colunaParametrizacaoDeHorarios.Valor).ToString().ToUpper().Trim() == "SIM");

                                    existeGrupoPessoas.UtilizarParametrizacaoDeHorariosNoAgendamento = (permiteParametrizacaoDeHorarios == existeGrupoPessoas.UtilizarParametrizacaoDeHorariosNoAgendamento) ? existeGrupoPessoas.UtilizarParametrizacaoDeHorariosNoAgendamento : permiteParametrizacaoDeHorarios;
                                }

                                if (colunaDescricao?.Valor != null && !string.IsNullOrEmpty(colunaDescricao.Valor))
                                    existeGrupoPessoas.Descricao = colunaDescricao.Valor;

                                if (colunaRaizCnpj?.Valor != null)
                                {
                                    List<string> listaRaizes = new List<string>();
                                    listaRaizes = ((string)(colunaRaizCnpj.Valor)).ToString().Split(',').ToList();
                                    listaRaizes = listaRaizes.Select(x => Utilidades.String.OnlyNumbers(x)).ToList();
                                    IList<string> raizesExistentesEmOutroGrupoPessoa = new List<string>();

                                    if (listaRaizes.All(x => x.IsSomenteNumeros()))
                                    {
                                        raizesExistentesEmOutroGrupoPessoa = repositorioGrupoPessoas.ObterRaizesExistentes(listaRaizes);
                                        if (raizesExistentesEmOutroGrupoPessoa.Count > 0)
                                            retorno = $"A(s) raíz(es) já estão sendo usadas: {string.Join(", ", raizesExistentesEmOutroGrupoPessoa.Select(x => x))}";

                                        if (raizesExistentesEmOutroGrupoPessoa.Count == 0)
                                            CriarGrupoRaizCnpj(listaRaizes, existeGrupoPessoas);
                                    }
                                    else
                                        retorno = "Uma ou mais raízes estão no formato errado.";
                                }

                                if (colunaVendedor?.Valor != null)
                                {
                                    List<Dominio.Entidades.Usuario> listaFuncionarios = new List<Dominio.Entidades.Usuario>();
                                    List<string> listaCPFVendedores = ((string)colunaVendedor.Valor).Split('/').ToList();

                                    foreach (string CPFvendedor in listaCPFVendedores)
                                    {
                                        Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorCPF(CPFvendedor.Trim().ObterSomenteNumeros());
                                        if (usuario == null)
                                            retorno = $"Não existe um funcionário com o CPF {CPFvendedor}.";
                                        else
                                            listaFuncionarios.Add(usuario);
                                    }

                                    if (listaFuncionarios.Count > 0)
                                        CriaGrupoFuncionario(listaFuncionarios, existeGrupoPessoas);
                                }

                                if (colunaClassificacao?.Valor != null)
                                {
                                    Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao classificacao = repositorioPessoaClassificacao.BuscarPorDescricao(colunaClassificacao.Valor);

                                    if (classificacao != null)
                                        existeGrupoPessoas.Classificacao = classificacao;

                                    if (classificacao == null)
                                        retorno = "Classificação não encontrada";
                                }

                                repositorioGrupoPessoas.Atualizar(existeGrupoPessoas);
                                Servicos.Auditoria.Auditoria.Auditar(_auditado, existeGrupoPessoas, "Grupo de Pessoa atualizado via importação de planilha.", _unitOfWork);

                            }

                            if (existeGrupoPessoas == null)
                            {
                                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas()
                                {
                                    Ativo = true
                                };

                                if (colunaParametrizacaoDeHorarios != null)
                                {
                                    bool permiteParametrizacaoDeHorarios = false;

                                    if (colunaParametrizacaoDeHorarios.Valor != null && !string.IsNullOrWhiteSpace((((string)colunaParametrizacaoDeHorarios.Valor).ToString())))
                                        permiteParametrizacaoDeHorarios = (((string)colunaParametrizacaoDeHorarios.Valor).ToString().ToUpper().Trim() == "SIM");

                                    grupoPessoas.UtilizarParametrizacaoDeHorariosNoAgendamento = permiteParametrizacaoDeHorarios;
                                }

                                grupoPessoas.Descricao = string.Empty;

                                if (colunaDescricao?.Valor != null || !string.IsNullOrEmpty(colunaDescricao.Valor))
                                {
                                    if (repositorioGrupoPessoas.ExisteGrupoPessoaPorDescricao(((string)colunaDescricao.Valor).ToString()))
                                        retorno = "Já existe um grupo de pessoa cadastrado com essa descrição.";
                                    else
                                        grupoPessoas.Descricao = colunaDescricao.Valor;
                                }

                                if (colunaDescricao?.Valor == null)
                                    retorno = "Campo Descrição sem valor.";

                                grupoPessoas.CodigoIntegracao = colunaCodigoIntegracao.Valor;

                                repositorioGrupoPessoas.Inserir(grupoPessoas);

                                List<string> listaRaizes = new List<string>();

                                if (colunaRaizCnpj != null)
                                {
                                    listaRaizes = ((string)(colunaRaizCnpj.Valor)).ToString().Split(',').ToList();
                                    listaRaizes = listaRaizes.Select(x => Utilidades.String.OnlyNumbers(x)).ToList();
                                    IList<string> raizesExistentesEmOutroGrupoPessoa = new List<string>();
                                    if (listaRaizes.All(x => x.IsSomenteNumeros()))
                                    {
                                        raizesExistentesEmOutroGrupoPessoa = repositorioGrupoPessoas.ObterRaizesExistentes(listaRaizes);
                                        if (raizesExistentesEmOutroGrupoPessoa.Count > 0)
                                            retorno = $"A(s) raíz(es) já estão sendo usadas: {string.Join(", ", raizesExistentesEmOutroGrupoPessoa.Select(x => x))}";

                                        if (raizesExistentesEmOutroGrupoPessoa.Count == 0)
                                            CriarGrupoRaizCnpj(listaRaizes, grupoPessoas);
                                    }
                                    else
                                        retorno = "Uma ou mais raízes estão no formato errado.";
                                }

                                    List<Dominio.Entidades.Usuario> listaFuncionarios = new List<Dominio.Entidades.Usuario>();
                                if (colunaVendedor != null && !string.IsNullOrEmpty(colunaVendedor.Valor))
                                {
                                    List<string> listaCPFVendedores = ((string)colunaVendedor.Valor).Split('/').ToList();
                                    foreach (string CPFvendedor in listaCPFVendedores)
                                    {
                                        Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorCPF(CPFvendedor.Trim().ObterSomenteNumeros());
                                        if (usuario == null)
                                            retorno = $"Não existe um funcionário com o CPF {CPFvendedor}.";
                                        else
                                            listaFuncionarios.Add(usuario);
                                    }

                                    if (listaFuncionarios.Count > 0)
                                        CriaGrupoFuncionario(listaFuncionarios, grupoPessoas);
                                }

                                if (colunaClassificacao != null)
                                {
                                    Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao classificacao = repositorioPessoaClassificacao.BuscarPorDescricao(colunaClassificacao.Valor);
                                    if (classificacao != null)
                                        grupoPessoas.Classificacao = classificacao;
                                    else
                                        retorno = "Classificação não encontrada";
                                }

                                Servicos.Auditoria.Auditoria.Auditar(_auditado, grupoPessoas, "Grupo de Pessoa adicionado via importação de planilha.", _unitOfWork);
                            }

                        }

                        if (string.IsNullOrEmpty(colunaCodigoIntegracao.Valor))
                            retorno = "Codigo Integração não informado";
                    }

                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        _unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                    }
                    else
                    {
                        contador++;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);

                        _unitOfWork.CommitChanges();
                    }


                }
                catch (ServicoException excecao)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                }
            }

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count;
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }

        public string ObterLogoBase64(int codigoGrupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            if (codigoGrupoPessoas == 0)
                return "";

            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Logo", "GrupoPessoas" });
            if (!System.IO.Directory.Exists(caminho))
                return "";

            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigoGrupoPessoas}.*").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private string AjustarRaiz(string raiz)
        {
            if (raiz.Length >= 8)
                return raiz;

            string raizAjustada = string.Empty;

            int quantidadeZeros = 8 - raiz.Length;
            raizAjustada = string.Concat(Enumerable.Repeat("0", quantidadeZeros)) + raiz;

            return raizAjustada;
        }

        private void CriaGrupoFuncionario(List<Dominio.Entidades.Usuario> listaFuncionarios, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasFuncionario repositorioGrupoPessoasFuncionario = new Repositorio.Embarcador.Pessoas.GrupoPessoasFuncionario(_unitOfWork);

            foreach (Dominio.Entidades.Usuario usuario in listaFuncionarios)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFuncionario grupoPessoaFuncionario = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFuncionario();

                grupoPessoaFuncionario.Funcionario = usuario;
                grupoPessoaFuncionario.DataInicioVigencia = DateTime.Now;
                grupoPessoaFuncionario.DataFimVigencia = DateTime.Now;
                grupoPessoaFuncionario.PercentualComissao = 0.01m;
                grupoPessoaFuncionario.GrupoPessoas = grupoPessoas;
                repositorioGrupoPessoasFuncionario.Inserir(grupoPessoaFuncionario);
            }
        }
        
        private void CriarGrupoRaizCnpj(List<string> listaRaizes, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ repositorioGrupoPessoasRaizCnpj = new Repositorio.Embarcador.Pessoas.GrupoPessoasRaizCNPJ(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);

            foreach (string raiz in listaRaizes)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ raizAdicionar = new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ();
                string raizAjustada = AjustarRaiz(raiz);
                raizAdicionar.RaizCNPJ = raizAjustada;
                raizAdicionar.AdicionarPessoasMesmaRaiz = true;
                raizAdicionar.GrupoPessoas = grupoPessoas;
                repositorioGrupoPessoasRaizCnpj.Inserir(raizAdicionar, _auditado);
                repositorioGrupoPessoas.AtualizarGrupoPessoaCliente(grupoPessoas.Codigo, raizAjustada);
            }

        }
        #endregion
    }
}
