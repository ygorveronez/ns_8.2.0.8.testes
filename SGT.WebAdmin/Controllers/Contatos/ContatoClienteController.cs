using Dominio.Entidades.Embarcador.Contatos;
using Newtonsoft.Json;
using Repositorio;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contatos
{
    [CustomAuthorize("Contatos/ContatoCliente", "Faturas/Fatura", "Financeiros/Bordero", "Financeiros/Titulo")]
    public class ContatoClienteController : BaseController
    {
		#region Construtores

		public ContatoClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Contatos.ContatoCliente repContatoCliente = new Repositorio.Embarcador.Contatos.ContatoCliente(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº", "Numero", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Prev. Retorno", "DataPrevistaRetorno", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 22, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Contato", "Contato", 22, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipoContato", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.left, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                int.TryParse(Request.Params("CodigoObjeto"), out int codigoObjeto);
                Enum.TryParse(Request.Params("TipoObjeto"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente tipoDocumento);

                // Consulta
                List<Dominio.Entidades.Embarcador.Contatos.ContatoCliente> listaGrid = repContatoCliente.Consultar(codigoObjeto, tipoDocumento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repContatoCliente.ContarConsulta(codigoObjeto, tipoDocumento);

                var lista = from obj in listaGrid
                            select new
                            {
                                obj.Codigo,
                                Numero = obj.Numero.ToString(),
                                Data = obj.Data.ToString("dd/MM/yyyy HH:mm"),
                                DataPrevistaRetorno = obj.DataPrevistaRetorno?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                obj.Descricao,
                                Contato = obj.Contato?.Contato ?? obj.ContatoSemCadastro,
                                obj.DescricaoSituacao,
                                obj.DescricaoTipoContato
                            };

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());

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

        public async Task<IActionResult> ObterDetalhesDocumento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("CodigoObjeto"), out int codigoObjeto);
                Enum.TryParse(Request.Params("TipoObjeto"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente tipoDocumento);

                int numeroDocumento, codigoGrupoPessoas;
                double cpfCnpjPessoa;
                string descricaoGrupoPessoas, nomePessoa;

                switch (tipoDocumento)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente.Titulo:
                        Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);

                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoObjeto);

                        if (titulo == null)
                            return new JsonpResult(false, true, "Título não encontrada.");

                        numeroDocumento = titulo.Codigo;
                        codigoGrupoPessoas = titulo.GrupoPessoas?.Codigo ?? 0;
                        cpfCnpjPessoa = titulo.Pessoa?.CPF_CNPJ ?? 0d;
                        descricaoGrupoPessoas = titulo.GrupoPessoas?.Descricao ?? string.Empty;
                        nomePessoa = titulo.Pessoa?.Nome;

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente.Fatura:
                        Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);

                        Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoObjeto);

                        if (fatura == null)
                            return new JsonpResult(false, true, "Fatura não encontrada.");

                        numeroDocumento = fatura.Numero;
                        codigoGrupoPessoas = fatura.GrupoPessoas?.Codigo ?? 0;
                        cpfCnpjPessoa = fatura.Cliente?.CPF_CNPJ ?? 0d;
                        descricaoGrupoPessoas = fatura.GrupoPessoas?.Descricao ?? string.Empty;
                        nomePessoa = fatura.Cliente?.Nome;

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente.Bordero:
                        Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unidadeTrabalho);

                        Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(codigoObjeto);

                        if (bordero == null)
                            return new JsonpResult(false, true, "Borderô não encontrado.");

                        numeroDocumento = bordero.Numero;
                        codigoGrupoPessoas = bordero.GrupoPessoas?.Codigo ?? 0;
                        cpfCnpjPessoa = bordero.Cliente?.CPF_CNPJ ?? 0d;
                        descricaoGrupoPessoas = bordero.GrupoPessoas?.Descricao ?? string.Empty;
                        nomePessoa = bordero.Cliente?.Nome;

                        break;
                    default:
                        numeroDocumento = 0;
                        codigoGrupoPessoas = 0;
                        cpfCnpjPessoa = 0d;
                        descricaoGrupoPessoas = string.Empty;
                        nomePessoa = string.Empty;
                        break;
                }

                return new JsonpResult(new
                {
                    GrupoPessoas = new
                    {
                        Descricao = descricaoGrupoPessoas,
                        Codigo = codigoGrupoPessoas
                    },
                    Pessoa = new
                    {
                        Descricao = nomePessoa,
                        Codigo = cpfCnpjPessoa
                    },
                    Numero = numeroDocumento
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os detalhes do documento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Contatos.ContatoCliente repContatoCliente = new Repositorio.Embarcador.Contatos.ContatoCliente(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Contatos.ContatoCliente contatoCliente = repContatoCliente.BuscarPorCodigo(codigo);

                // Valida
                if (contatoCliente == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    contatoCliente.Codigo,
                    contatoCliente.Descricao,
                    Contato = new
                    {
                        Descricao = contatoCliente.Contato?.Contato,
                        Codigo = contatoCliente.Contato?.Codigo
                    },
                    contatoCliente.ContatoSemCadastro,
                    Data = contatoCliente.Data.ToString("dd/MM/yyyy HH:mm"),
                    DataPrevistaRetorno = contatoCliente.DataPrevistaRetorno?.ToString("dd/MM/yyyy HH:mm"),
                    SituacaoContato = contatoCliente.Situacao.Codigo,
                    TipoContato = contatoCliente.TiposContato.Select(o => o.Codigo).ToList()
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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
                // Inicia transacao
                unitOfWork.Start();

                // Busca informacoes
                Dominio.Entidades.Embarcador.Contatos.ContatoCliente contatoCliente = new Dominio.Entidades.Embarcador.Contatos.ContatoCliente();

                // Preenche entidade com dados
                PreencheEntidade(unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                unitOfWork.Start();

                PreencheEntidade(unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                // Instancia repositorios
                Repositorio.Embarcador.Contatos.ContatoCliente repContatoCliente = new Repositorio.Embarcador.Contatos.ContatoCliente(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Contatos.ContatoCliente contatoCliente = repContatoCliente.BuscarPorCodigo(codigo);

                // Valida
                if (contatoCliente == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repContatoCliente.Deletar(contatoCliente);

                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */

        private void PreencheEntidade(UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Contatos.ContatoCliente repContatoCliente = new Repositorio.Embarcador.Contatos.ContatoCliente(unitOfWork);
            Repositorio.Embarcador.Contatos.ContatoClienteDocumento repContatoClienteDocumento = new Repositorio.Embarcador.Contatos.ContatoClienteDocumento(unitOfWork);
            Repositorio.Embarcador.Contatos.TipoContato repTipoContato = new Repositorio.Embarcador.Contatos.TipoContato(unitOfWork);
            Repositorio.Embarcador.Contatos.SituacaoContato repSituacaoContato = new Repositorio.Embarcador.Contatos.SituacaoContato(unitOfWork);
            Repositorio.Embarcador.Contatos.PessoaContato repPessoaContato = new Repositorio.Embarcador.Contatos.PessoaContato(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            int.TryParse(Request.Params("Codigo"), out int codigo);
            int.TryParse(Request.Params("SituacaoContato"), out int codigoSituacao);
            int.TryParse(Request.Params("CodigoObjeto"), out int codigoObjeto);
            int.TryParse(Request.Params("Contato"), out int codigoContato);
            int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);

            double.TryParse(Request.Params("Pessoa"), out double cpfCnpjPessoa);

            Enum.TryParse(Request.Params("TipoObjeto"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente tipoDocumento);

            DateTime.TryParseExact(Request.Params("Data"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime data);

            DateTime? dataPrevistaRetorno = null;
            if (DateTime.TryParseExact(Request.Params("DataPrevistaRetorno"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataPrevistaRetornoAux))
                dataPrevistaRetorno = dataPrevistaRetornoAux;

            string descricao = Request.Params("Descricao");
            string contatoSemCadastro = Request.Params("ContatoSemCadastro");
            
            string tipoContatoJson = Request.Params("TipoContato");

            List<int> codigosTipoContato = tipoContatoJson.Split(',').Select(int.Parse).ToList();

            Dominio.Entidades.Embarcador.Contatos.ContatoCliente contatoCliente = null;

            if (codigo > 0)
                contatoCliente = repContatoCliente.BuscarPorCodigo(codigo, true);
            else
            {
                contatoCliente = new ContatoCliente();
                contatoCliente.Usuario = Usuario;
                contatoCliente.Numero = repContatoCliente.BuscarUltimoNumero() + 1;
            }

            contatoCliente.Contato = codigoContato > 0 ? repPessoaContato.BuscarPorCodigo(codigoContato, false) : null;
            contatoCliente.ContatoSemCadastro = contatoSemCadastro;
            contatoCliente.Data = data;
            contatoCliente.DataPrevistaRetorno = dataPrevistaRetorno;
            contatoCliente.Descricao = descricao;
            contatoCliente.GrupoPessoas = codigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas) : null;
            contatoCliente.Pessoa = cpfCnpjPessoa > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
            contatoCliente.Situacao = repSituacaoContato.BuscarPorCodigo(codigoSituacao);

            if (contatoCliente.TiposContato == null)
                contatoCliente.TiposContato = new List<Dominio.Entidades.Embarcador.Contatos.TipoContato>();
            else
                contatoCliente.TiposContato.Clear();

            foreach (int codigoTipoContato in codigosTipoContato)
                contatoCliente.TiposContato.Add(repTipoContato.BuscarPorCodigo(codigoTipoContato));

            bool contatoNovo = false;
            if (contatoCliente.Codigo > 0)
                repContatoCliente.Atualizar(contatoCliente);
            else
            {
                contatoNovo = true;
                repContatoCliente.Inserir(contatoCliente);
            }

            List<Dominio.Entidades.Embarcador.Contatos.ContatoClienteDocumento> documentos = repContatoClienteDocumento.BuscarPorContatoCliente(contatoCliente.Codigo);

            foreach (Dominio.Entidades.Embarcador.Contatos.ContatoClienteDocumento documento in documentos)
                repContatoClienteDocumento.Deletar(documento);

            Dominio.Entidades.Embarcador.Contatos.ContatoClienteDocumento contatoClienteDocumento = new ContatoClienteDocumento()
            {
                ContatoCliente = contatoCliente,
                Tipo = tipoDocumento
            };

            object objeto = null;

            switch (tipoDocumento)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente.Titulo:
                    Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                    contatoClienteDocumento.Titulo = repTitulo.BuscarPorCodigo(codigoObjeto);
                    objeto = contatoClienteDocumento.Titulo;
                    
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente.Fatura:
                    Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                    contatoClienteDocumento.Fatura = repFatura.BuscarPorCodigo(codigoObjeto);
                    objeto = contatoClienteDocumento.Fatura;

                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoContatoCliente.Bordero:
                    Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);

                    contatoClienteDocumento.Bordero = repBordero.BuscarPorCodigo(codigoObjeto);
                    objeto = contatoClienteDocumento.Bordero;

                    break;
                default:
                    break;
            }

            repContatoClienteDocumento.Inserir(contatoClienteDocumento);

            if (contatoNovo)
                Servicos.Auditoria.Auditoria.Auditar(Auditado, objeto, null, "Adicionou o contato " + contatoCliente.Numero.ToString() + ".", unitOfWork);
            else
                Servicos.Auditoria.Auditoria.Auditar(Auditado, objeto, contatoCliente.GetChanges(), "Alterou o contato " + contatoCliente.Numero.ToString() + ".", unitOfWork);
            
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "DescricaoSituacao")
                propOrdenar = "Situacao.Descricao";

        }
        #endregion
    }
}
