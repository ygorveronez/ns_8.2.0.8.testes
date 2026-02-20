using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Pessoa
{
    public class ClienteBuscaAutomatica
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly System.Threading.CancellationToken _cancellationToken;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion

        #region Construtores

        public ClienteBuscaAutomatica(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, System.Threading.CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _cancellationToken = cancellationToken;
        }

        #endregion

        #region Métodos Públicos

        public async Task<Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao> ImportarAsync(List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas)
        {
            Repositorio.Embarcador.Pessoas.ClienteBuscaAutomatica repositorioBuscaCliente = new(_unitOfWork, _cancellationToken);
            Repositorio.Localidade repositorioLocalidade = new(_unitOfWork, _cancellationToken);
            Repositorio.Cliente repCliente = new(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Filiais.Filial repFilial = new(_unitOfWork, _cancellationToken);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new();
            Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica buscaCliente = new();

            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
            int contador = 0;

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    _unitOfWork.FlushAndClear();
                    await _unitOfWork.StartAsync();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                    string retorno = "";

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaDescricao = (from obj in linha.Colunas where obj.NomeCampo == "Descricao" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCNPJRementente = (from obj in linha.Colunas where obj.NomeCampo == "CNPJRementente" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCNPJDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "CNPJDestinatario" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCidadeOrigem = (from obj in linha.Colunas where obj.NomeCampo == "CidadeOrigem" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaUFOrigem = (from obj in linha.Colunas where obj.NomeCampo == "UFOrigem" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaSituacao = (from obj in linha.Colunas where obj.NomeCampo == "Situacao" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCliente = (from obj in linha.Colunas where obj.NomeCampo == "Cliente" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaTipoParticipante = (from obj in linha.Colunas where obj.NomeCampo == "TipoParticipante" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaFilial = (from obj in linha.Colunas where obj.NomeCampo == "Filial" select obj).FirstOrDefault();

                    string descricao = string.Empty;

                    if (colunaDescricao?.Valor != null || !string.IsNullOrEmpty(colunaDescricao.Valor))
                        descricao = colunaDescricao.Valor;

                    if (colunaDescricao?.Valor == null)
                        retorno = "Campo Descrição sem valor.";

                    var tipo = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParticipante();
                    if (colunaTipoParticipante != null)
                    {
                        var valorTexto = (string)colunaTipoParticipante.Valor;
                        if (!string.IsNullOrWhiteSpace(valorTexto)
                            && Enum.TryParse<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParticipante>(valorTexto, true, out var tipoParticipante))
                        {
                            tipo = tipoParticipante;
                        }
                    }

                    var situacao = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa();
                    if (colunaSituacao != null)
                    {
                        var valor = (string)colunaSituacao.Valor;
                        if (!string.IsNullOrWhiteSpace(valor)
                            && Enum.TryParse<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>(valor, true, out var situacaoImp))
                        {
                            situacao = situacaoImp;
                        }
                    }

                    string origemString = "";
                    Dominio.Entidades.Localidade origem = null;
                    if (colunaCidadeOrigem != null)
                    {
                        origemString = colunaCidadeOrigem.Valor;

                        string estadoString = "";

                        if (colunaUFOrigem != null)
                            estadoString = colunaUFOrigem.Valor;

                        origem = await repositorioLocalidade.BuscarPrimeiraPorUFDescricaoAsync(estadoString, origemString);

                        if (origem == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A origem não existe na base da Multisoftware", i));
                            await _unitOfWork.RollbackAsync();
                            continue;
                        }
                    }


                    Dominio.Entidades.Cliente remetente = null;
                    if (colunaCNPJRementente != null)
                    {
                        string somenteNumeros = Utilidades.String.OnlyNumbers((string)colunaCNPJRementente.Valor);
                        if (!string.IsNullOrEmpty(somenteNumeros))
                        {
                            double cpfCNPJRemetente = double.Parse(somenteNumeros);
                            remetente = await repCliente.BuscarPorCPFCNPJAsync(cpfCNPJRemetente);
                            if (remetente == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O remetente não existe na base da Multisoftware", i));
                                await _unitOfWork.RollbackAsync();
                                continue;
                            }
                        }
                    }

                    Dominio.Entidades.Cliente destinatario = null;
                    if (colunaCNPJDestinatario != null)
                    {
                        double cpfCNPJDestinatario = 0;
                        string somenteNumeros = Utilidades.String.OnlyNumbers((string)colunaCNPJDestinatario.Valor);
                        if (!string.IsNullOrEmpty(somenteNumeros))
                        {
                            cpfCNPJDestinatario = double.Parse(somenteNumeros);
                            destinatario = await repCliente.BuscarPorCPFCNPJAsync(cpfCNPJDestinatario);

                            if (destinatario == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O destinatário não existe na base da Multisoftware", i));
                                await _unitOfWork.RollbackAsync();
                                continue;
                            }
                        }
                    }

                    Dominio.Entidades.Cliente cliente = null;
                    if (colunaCliente != null && colunaCliente.Valor != null)
                    {
                        double cpfCNPJCliente = Utilidades.String.OnlyNumbers((string)colunaCliente.Valor).ToDouble();
                        if (cpfCNPJCliente > 0d)
                        {
                            cliente = await repCliente.BuscarPorCPFCNPJAsync(cpfCNPJCliente);
                            if (cliente == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O cliente não existe na base da Multisoftware", i));
                                await _unitOfWork.RollbackAsync();
                                continue;
                            }
                        }
                    }

                    Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                    if (colunaFilial != null)
                    {
                        string codigoIntegracaoFilial = (string)colunaFilial.Valor;
                        filial = repFilial.buscarPorCodigoEmbarcador(codigoIntegracaoFilial);
                        if (filial == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A filial não existe na base da Multisoftware", i));
                            await _unitOfWork.RollbackAsync();
                            continue;
                        }

                        if (remetente == null && colunaCNPJRementente == null)
                        {
                            remetente = await repCliente.BuscarPorCPFCNPJAsync(double.Parse(Utilidades.String.OnlyNumbers(filial.CNPJ)));
                            if (remetente == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O remetente não existe na base da Multisoftware", i));
                                await _unitOfWork.RollbackAsync();
                                continue;
                            }
                        }

                        if (destinatario == null && colunaCNPJDestinatario == null)
                        {
                            destinatario = await repCliente.BuscarPorCPFCNPJAsync(double.Parse(Utilidades.String.OnlyNumbers(filial.CNPJ)));
                            if (destinatario == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O destinatário não existe na base da Multisoftware", i));
                                await _unitOfWork.RollbackAsync();
                                continue;
                            }
                        }
                    }
                    else if (remetente != null)
                        filial = repFilial.buscarPorCodigoEmbarcador(remetente.CPF_CNPJ_SemFormato);

                    if (filial == null)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("É obrigatório informar a filial!", i));
                        await _unitOfWork.RollbackAsync();
                        continue;
                    }

                    buscaCliente.Descricao = descricao;
                    buscaCliente.TipoParticipante = tipo;
                    buscaCliente.Situacao = situacao;
                    buscaCliente.Origem = origem;
                    buscaCliente.Remetente = remetente;
                    buscaCliente.Destinatario = destinatario;
                    buscaCliente.Cliente = cliente;
                    buscaCliente.Filial = filial;

                    await repositorioBuscaCliente.InserirAsync(buscaCliente);

                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        await _unitOfWork.RollbackAsync();
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                    }
                    else
                    {
                        contador++;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new() { indice = i, processou = true, mensagemFalha = "" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);

                        await _unitOfWork.CommitChangesAsync();
                    }

                }
                catch (Dominio.Excecoes.Embarcador.ServicoException excecao)
                {
                    await _unitOfWork.RollbackAsync();
                    Log.TratarErro(excecao);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                }
            }

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count;
            retornoImportacao.Importados = contador;

            return retornoImportacao;
        }


        #endregion

        #region Métodos Privados


        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, bool contar = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new() { mensagemFalha = mensagem, processou = false, contar = contar };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int codigo)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new() { codigo = codigo, mensagemFalha = "", processou = true, contar = true };
            return retorno;
        }
        #endregion
    }
}
