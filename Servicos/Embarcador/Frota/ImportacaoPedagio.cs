using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frota
{
    public sealed class ImportacaoPedagio
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Usuario _usuario;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;

        public ImportacaoPedagio(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ImportacaoPedagio(UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario) : this(unitOfWork)
        {
            this._usuario = usuario;
        }

        public ImportacaoPedagio(UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, TipoServicoMultisoftware tipoServico) : this(unitOfWork, usuario)
        {
            this._tipoServicoMultisoftware = tipoServico;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao GerarImportacaoPedagio(List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas, string nomeArquivo)
        {
            Repositorio.Embarcador.Frota.ImportacaoPedagio repositorioImportacaoPedagio = new Repositorio.Embarcador.Frota.ImportacaoPedagio(_unitOfWork);
            Repositorio.Embarcador.Frota.ImportacaoPedagioLinha repositorioImportacaoPedagioLinha = new Repositorio.Embarcador.Frota.ImportacaoPedagioLinha(_unitOfWork);
            Repositorio.Embarcador.Frota.ImportacaoPedagioLinhaColuna repositorioImportacaoPedagioLinhaColuna = new Repositorio.Embarcador.Frota.ImportacaoPedagioLinhaColuna(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();

            _unitOfWork.Start();

            Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio importacaoPedagio = new Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio()
            {
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio.Pendente,
                Planilha = nomeArquivo,
                QuantidadeLinhas = linhas.Count,
                Usuario = _usuario,
                DataImportacao = DateTime.Now
            };

            repositorioImportacaoPedagio.Inserir(importacaoPedagio);

            for (int i = 0; i < importacaoPedagio.QuantidadeLinhas; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinhaArquivo = linhas[i];

                Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha linha = new Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha()
                {
                    ImportacaoPedagio = importacaoPedagio,
                    Numero = i + 1,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedagio.Pendente
                };

                repositorioImportacaoPedagioLinha.Inserir(linha);

                for (int j = 0; j < dadosLinhaArquivo.Colunas.Count; j++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna dadosColunaArquivo = dadosLinhaArquivo.Colunas[j];

                    Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna coluna = new Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna()
                    {
                        Linha = linha,
                        NomeCampo = dadosColunaArquivo.NomeCampo,
                        Valor = (string)dadosColunaArquivo.Valor
                    };
                    if (string.IsNullOrWhiteSpace(coluna.Valor))
                        coluna.Valor = "";

                    repositorioImportacaoPedagioLinhaColuna.Inserir(coluna);
                }
            }

            _unitOfWork.CommitChanges();

            retorno.MensagemAviso = "Planilha adicionada com sucesso à fila de processamento.";
            retorno.Total = linhas.Count;
            retorno.Importados = linhas.Count;

            return retorno;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.RetornoImportacaoPedagio ImportarPedagio(Dominio.Entidades.Embarcador.Frota.ImportacaoPedagio importacaoPedagio)
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.RetornoImportacaoPedagio retorno = new Dominio.ObjetosDeValor.Embarcador.Frota.RetornoImportacaoPedagio();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                Usuario = _usuario,
                Empresa = _usuario?.Empresa,
                Texto = ""
            };

            Repositorio.Embarcador.Frota.ImportacaoPedagio repositorioImportacao = new Repositorio.Embarcador.Frota.ImportacaoPedagio(_unitOfWork);
            Repositorio.Embarcador.Frota.ImportacaoPedagioLinha repositorioImportacaoLinha = new Repositorio.Embarcador.Frota.ImportacaoPedagioLinha(_unitOfWork);
            Repositorio.Embarcador.Frota.ImportacaoPedagioLinhaColuna repositorioImportacaoColuna = new Repositorio.Embarcador.Frota.ImportacaoPedagioLinhaColuna(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            List<int> codigosLinhasGerar = repositorioImportacaoLinha.BuscarCodigosLinhasPendentesGeracaoPedagio(importacaoPedagio.Codigo);
            List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna> colunasGerar = repositorioImportacaoColuna.BuscarPorImportacaoPendentesGeracaoPedagio(importacaoPedagio.Codigo);
            int contador = 0;
            string retornoFinaliza = "";

            try
            {
                for (int i = 0; i < codigosLinhasGerar.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinha linha = repositorioImportacaoLinha.BuscarPorCodigo(codigosLinhasGerar[i], false);

                    _unitOfWork.Start();
                    linha.Situacao = SituacaoImportacaoPedagio.Processando;
                    repositorioImportacaoLinha.Atualizar(linha);
                    _unitOfWork.CommitChanges();

                    List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna> colunas = colunasGerar.Where(o => o.Linha.Codigo == linha.Codigo).ToList();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinha = ConverterParaImportacao(colunas);

                    _unitOfWork.Start();

                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = ImportarRegistro(dadosLinha, auditado);

                        if (retornoLinha.contar)
                        {
                            if (retornoLinha.codigo > 0)
                            {
                                linha.Pedagio = new Dominio.Entidades.Embarcador.Pedagio.Pedagio() { Codigo = retornoLinha.codigo };
                                linha.Situacao = SituacaoImportacaoPedagio.Sucesso;
                                linha.Mensagem = "Pedágio importado.";
                                repositorioImportacaoLinha.Atualizar(linha);
                                contador++;
                            }
                        }
                        else
                        {
                            _unitOfWork.Rollback();
                            _unitOfWork.Start();
                            linha.Situacao = SituacaoImportacaoPedagio.Erro;
                            linha.Mensagem = retornoLinha.mensagemFalha;
                            repositorioImportacaoLinha.Atualizar(linha);
                        }

                        _unitOfWork.CommitChanges();
                        _unitOfWork.FlushAndClear();
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Rollback();
                        _unitOfWork.Start();
                        linha.Situacao = SituacaoImportacaoPedagio.Erro;
                        linha.Mensagem = ex.Message;
                        repositorioImportacaoLinha.Atualizar(linha);
                        _unitOfWork.CommitChanges();
                    }
                }

                retorno.TotalRegistros = repositorioImportacaoLinha.ContarPedagiosPorImportacaoPrecoCombustivel(importacaoPedagio.Codigo);
                retorno.Sucesso = (retorno.TotalRegistros > 0);
                retorno.Mensagem = string.IsNullOrWhiteSpace(retornoFinaliza) ? "Importação de pedágios finalizada (acompanhe o status das linhas)." : retornoFinaliza;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                Log.TratarErro(ex);

                retorno.Sucesso = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha ConverterParaImportacao(List<Dominio.Entidades.Embarcador.Frota.ImportacaoPedagioLinhaColuna> colunas)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha()
            {
                Colunas = colunas.Select(o => new Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna()
                {
                    NomeCampo = o.NomeCampo,
                    Valor = o.Valor
                }).ToList()
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha ImportarRegistro(Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");

            Repositorio.Embarcador.Pedagio.Pedagio repositorioPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinhaPedido = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha();
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork);

            try
            {
                string placa = string.Empty;
                string praca = string.Empty;
                string rodovia = string.Empty;
                string observacao = string.Empty;
                decimal valor = 0;
                DateTime? data = null;
                TipoPedagio? tipo = null;
                Dominio.Entidades.Usuario motorista = null;

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaPlaca = (from obj in linha.Colunas where obj.NomeCampo == "Placa" select obj).FirstOrDefault();
                if (colunaPlaca != null)
                {
                    placa = (string)colunaPlaca.Valor;

                    if (string.IsNullOrEmpty(placa))
                        return RetornarFalhaLinha("A placa é obrigatória");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaValor = (from obj in linha.Colunas where obj.NomeCampo == "Valor" select obj).FirstOrDefault();
                if (colunaValor != null)
                {
                    valor = ((string)colunaValor.Valor).ToDecimal();

                    if (valor == 0)
                        return RetornarFalhaLinha("O valor precisa ser maior que 0");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaDataPassagem = (from obj in linha.Colunas where obj.NomeCampo == "DataPassagem" select obj).FirstOrDefault();
                if (colunaDataPassagem != null)
                {
                    data = DateTime.Parse(((string)colunaDataPassagem.Valor).ToString(), cultura);

                    if (!data.HasValue)
                        return RetornarFalhaLinha("A data é obrigatória");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaHoraPassagem = (from obj in linha.Colunas where obj.NomeCampo == "HoraPassagem" select obj).FirstOrDefault();
                if (colunaHoraPassagem != null)
                {
                    TimeSpan? hora = ((string)colunaHoraPassagem.Valor).ToNullableTime();

                    if (!hora.HasValue)
                        return RetornarFalhaLinha("A hora é obrigatória");

                    if (data.HasValue)
                        data = data.Value.Add(hora.Value);
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaTipo = (from obj in linha.Colunas where obj.NomeCampo == "Tipo" select obj).FirstOrDefault();
                if (colunaTipo != null)
                {
                    string tipoDescricao = ((string)colunaTipo.Valor).ToString();

                    if (string.IsNullOrEmpty(tipoDescricao))
                        return RetornarFalhaLinha("O tipo é obrigatório");

                    if (tipoDescricao.ToUpper().Replace(" ", "") == "CR")
                        tipo = TipoPedagio.Credito;
                    else if (tipoDescricao.ToUpper().Replace(" ", "") == "DB")
                        tipo = TipoPedagio.Debito;
                    else
                        return RetornarFalhaLinha("Tipo não reconhecido.");
                }

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaPraca = (from obj in linha.Colunas where obj.NomeCampo == "Praca" select obj).FirstOrDefault();
                if (colunaPraca != null)
                    praca = ((string)colunaPraca.Valor).ToString();

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaRodovia = (from obj in linha.Colunas where obj.NomeCampo == "Rodovia" select obj).FirstOrDefault();
                if (colunaRodovia != null)
                    rodovia = ((string)colunaRodovia.Valor).ToString();

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                if (colunaObservacao != null)
                    observacao = ((string)colunaObservacao.Valor).ToString();

                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(placa);

                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaCPFMotorista = (from obj in linha.Colunas where obj.NomeCampo == "CPFMotorista" select obj).FirstOrDefault();
                if (colunaCPFMotorista != null)
                {

                    string cpf = (string)colunaCPFMotorista.Valor;

                    bool cpfValido = Utilidades.Validate.ValidarCPF(cpf);
                    if (!cpfValido)
                        return RetornarFalhaLinha("CPF Invalido");

                    motorista = repositorioMotorista.BuscarPorCPF(cpf);
                }


                if (motorista == null)
                    motorista = veiculo?.Motoristas?.FirstOrDefault()?.Motorista ?? null;

                Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = new Dominio.Entidades.Embarcador.Pedagio.Pedagio()
                {
                    Veiculo = veiculo,
                    Motorista = motorista,
                    PlacaVeiculoNaoCadastrado = veiculo == null ? placa : "",
                    TipoPedagio = tipo.Value,
                    Praca = praca,
                    Rodovia = rodovia,
                    Observacao = observacao,
                    Data = data.Value,
                    Valor = valor,
                    DataImportacao = DateTime.Now.Date
                };

                pedagio.SituacaoPedagio = RetornarSituacaoPedagio(pedagio);

                repositorioPedagio.Inserir(pedagio);

                retornoLinhaPedido.codigo = pedagio.Codigo;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                return RetornarFalhaLinha("Ocorreu uma falha ao processar a linha (" + excecao.Message + ").");
            }

            return RetornarSucessoLinha(retornoLinhaPedido?.codigo ?? 0);
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, bool contar = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { mensagemFalha = mensagem, processou = false, contar = contar };
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarSucessoLinha(int codigo)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { codigo = codigo, mensagemFalha = "", processou = true, contar = true };
            return retorno;
        }

        private SituacaoPedagio RetornarSituacaoPedagio(Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio)
        {
            if (pedagio.Veiculo == null)
                return SituacaoPedagio.Inconsistente;
            else
                return SituacaoPedagio.Lancado;
        }
    }
}
