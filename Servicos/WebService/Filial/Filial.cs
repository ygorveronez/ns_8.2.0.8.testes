using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;

namespace Servicos.WebService.Filial
{
    public class Filial : ServicoBase
    {
        #region Variaveis Privadas

        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Constructores        
        public Filial(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { } 
        public Filial(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }
        #endregion

        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Filiais.Filial SalvarFilial(Dominio.ObjetosDeValor.Embarcador.Filial.Filial filialIntegracao, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Localidade repLocalidades = new Repositorio.Localidade(unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(filialIntegracao.CodigoIntegracao);
            bool inserir = false;
            if (filial == null)
            {
                filial = new Dominio.Entidades.Embarcador.Filiais.Filial();
                inserir = true;
            }

            if (string.IsNullOrWhiteSpace(filialIntegracao.CodigoIntegracao))
                mensagem += "É obrigatório informar o codigo de integração da filial. ";
            if (filialIntegracao.Endereco == null)
                mensagem += "É obrigatório informar o endereço da filial. ";
            else
            {
                if (filialIntegracao.Endereco.Cidade == null)
                {
                    mensagem += "É obrigatório informar a cidade da filial. ";
                }
                else
                {
                    if (filialIntegracao.Endereco.Cidade.IBGE == 0)
                    {
                        mensagem += "É obrigatório informar o código IBGE da Cidade da filial. ";
                    }
                    else
                    {
                        filial.Localidade = repLocalidades.BuscarPorCodigoIBGE(filialIntegracao.Endereco.Cidade.IBGE);
                        if (filial.Localidade == null)
                            mensagem += "O codigo IBGE informado (" + filialIntegracao.Endereco.Cidade.IBGE.ToString() + ") para a filial não é válido, por favor, verifique. ";
                    }
                }
            }
            if (filialIntegracao.CodigoAtividade == 0)
            {
                mensagem += "É obrigatório a atividade da filial. ";
            }
            else
            {
                filial.Atividade = repAtividade.BuscarPorCodigo(filialIntegracao.CodigoAtividade);
                if (filial.Atividade == null)
                {
                    mensagem += "A atividade informada (" + filialIntegracao.CodigoAtividade.ToString() + ") para a filial não é válida, por favor, verifique a tabela de atividades disponíbilizadas pela Multisoftware. ";
                }
            }

            if (!Utilidades.Validate.ValidarCNPJ(filialIntegracao.CNPJ))
                mensagem += "O CNPJ (" + filialIntegracao.CNPJ + ") informado para a filial não é válido. ";

            if (string.IsNullOrWhiteSpace(mensagem))
            {
                filial.Ativo = filialIntegracao.Ativo;
                filial.CNPJ = filialIntegracao.CNPJ;
                filial.CodigoFilialEmbarcador = filialIntegracao.CodigoIntegracao;
                filial.Descricao = filialIntegracao.Descricao;
                filial.TipoFilial = filialIntegracao.TipoFilial;

                if (inserir)
                    repFilial.Inserir(filial);
                else
                    repFilial.Atualizar(filial);

                return filial;
            }
            else
            {
                return null;
            }

        }

        public List<Dominio.ObjetosDeValor.Embarcador.Filial.Filial> RetornarFiliais(List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Filial.Filial> filiaisIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>();
            foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in filiais)
            {
                filiaisIntegracao.Add(ConverterObjetoFilial(filial));
            }

            return filiaisIntegracao;
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarIntegracaoFilial(List<int> protocolos)
        {
            if (protocolos == null || protocolos.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Sem Protocolo de filail para Confirmar.");

            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            List<int> protocolosNaoProcessado = new List<int>();

            foreach (int protocolo in protocolos)
            {
                Dominio.Entidades.Embarcador.Filiais.Filial existeFilial = repositorioFilial.BuscarPorCodigo(protocolo);
                if (existeFilial == null)
                {
                    protocolosNaoProcessado.Add(protocolo);
                    continue;
                }

                existeFilial.IntegradoERP = true;
                repositorioFilial.Atualizar(existeFilial);
            }

            if (protocolosNaoProcessado.Count > 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, $"O(s) protocolo(s) {string.Join(",", protocolosNaoProcessado)} não foram confirmados porque não foi achados registros existentes.");

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Todos os protocolos confirmados com sucesso");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>> BuscarFiliaisPendentesIntegracao(int quantidade)
        {
            Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Filiais.Filial> repositorioFiliais = new Repositorio.Embarcador.IntegracaoERP.IntegracaoERP<Dominio.Entidades.Embarcador.Filiais.Filial>(_unitOfWork);

            int totalFiliaisPendenteIntegracao = repositorioFiliais.ContarRegistroPendenteIntegracao();

            Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial> retornoPaginacao = new Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>()
            {
                Itens = new List<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>(),
                NumeroTotalDeRegistro = totalFiliaisPendenteIntegracao
            };

            if (totalFiliaisPendenteIntegracao == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>>.CriarRetornoSucesso(retornoPaginacao);

            IList<Dominio.Entidades.Embarcador.Filiais.Filial> filiaisPendentes = repositorioFiliais.BuscarRegitrosPendenteIntegracao(quantidade);

            foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in filiaisPendentes)
                retornoPaginacao.Itens.Add(ConverterObjetoFilial(filial));

            return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>>.CriarRetornoSucesso(retornoPaginacao);

        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarVolumesTanques(Dominio.ObjetosDeValor.WebService.Filial.FilialTanque filialTanque)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Tanques.Tanque repTanque = new Repositorio.Embarcador.Tanques.Tanque(_unitOfWork);
            Repositorio.Embarcador.Tanques.FilialTanque repFilialTanque = new Repositorio.Embarcador.Tanques.FilialTanque(_unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(filialTanque.Filial);

            if (filial == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Filial não encontrada.");

            _unitOfWork.Start();

            Dominio.Entidades.Embarcador.Tanques.Tanque tanque = repTanque.BuscarPorID(filialTanque.Tanque.ID);
            if (tanque == null)
            {
                tanque = new Dominio.Entidades.Embarcador.Tanques.Tanque();
            }
            else
            {
                tanque.Initialize();
            }
            tanque.Descricao = filialTanque.Tanque.Descricao;
            tanque.ID = filialTanque.Tanque.ID;

            if (tanque.Codigo == 0)
                repTanque.Inserir(tanque);
            else
                repTanque.Atualizar(tanque);

            Dominio.Entidades.Embarcador.Filiais.FilialTanque filialTanqueEntidade = repFilialTanque.BuscarPorFilialETanque(filial.CodigoFilialEmbarcador, filialTanque.Tanque.ID);

            if (filialTanqueEntidade == null)
                filialTanqueEntidade = new Dominio.Entidades.Embarcador.Filiais.FilialTanque();
            else
                filialTanqueEntidade.Initialize();

            filialTanqueEntidade.Filial = filial;
            filialTanqueEntidade.Tanque = tanque;
            filialTanqueEntidade.Volume = filialTanque.Volume;
            filialTanqueEntidade.Capacidade = filialTanque.Capacidade;
            filialTanqueEntidade.Vazao = filialTanque.Vazao;
            filialTanqueEntidade.Status = filialTanque.Status;
            filialTanqueEntidade.DataAtualizacao = filialTanque.DataAtualizacao;
            filialTanqueEntidade.Ocupacao = filialTanque.Ocupacao;

            if (filialTanqueEntidade.Codigo == 0)
                repFilialTanque.Inserir(filialTanqueEntidade);
            else
                repFilialTanque.Atualizar(filialTanqueEntidade);

            _unitOfWork.CommitChanges();

            _unitOfWork.FlushAndClear();

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Tanque e vínculo com filial incluído/atualizado com sucesso!");
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarEstoqueProdutoArmazem(Dominio.ObjetosDeValor.WebService.Filial.ProdutoEmbarcadorEstoqueArmazem salvarEstoqueProdutoArmazem)
        {
            Servicos.Log.TratarErro($"salvarEstoqueProdutoArmazem: {Newtonsoft.Json.JsonConvert.SerializeObject(salvarEstoqueProdutoArmazem)}");

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);
                if (!repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao().HabilitarCadastroArmazem)
                    throw new ServicoException("Sem permissão para adicionar");

                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
                Repositorio.Embarcador.Filiais.FilialArmazem repositorioFilialArmazem = new Repositorio.Embarcador.Filiais.FilialArmazem(_unitOfWork);
                Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem repositorioProdutoEmbarcadorEstoqueArmazem = new Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem(_unitOfWork);

                Dominio.Entidades.Embarcador.Filiais.Filial filial = !string.IsNullOrWhiteSpace(salvarEstoqueProdutoArmazem.CodigoFilial) ? repositorioFilial.BuscarPorCodigoIntegracao(salvarEstoqueProdutoArmazem.CodigoFilial) : null;
                if (filial == null)
                    throw new ServicoException("Filial não encontrada.");

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = !string.IsNullOrWhiteSpace(salvarEstoqueProdutoArmazem.CodigoProduto) ? repositorioProdutoEmbarcador.buscarPorCodigoEmbarcador(salvarEstoqueProdutoArmazem.CodigoProduto) : null;
                if (produtoEmbarcador == null)
                    throw new ServicoException("Produto não encontrado.");

                Dominio.Entidades.Embarcador.Filiais.FilialArmazem filialArmazem = !string.IsNullOrWhiteSpace(salvarEstoqueProdutoArmazem.CodigoFilialArmazem) ? repositorioFilialArmazem.BuscarPorCodigoIntegracao(salvarEstoqueProdutoArmazem.CodigoFilialArmazem) : null;
                if (filialArmazem == null)
                    throw new ServicoException("Armazém não encontrado.");

                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem produtoEmbarcadorEstoqueArmazem = repositorioProdutoEmbarcadorEstoqueArmazem.BuscarPorFilialProdutoArmazem(filial.Codigo, produtoEmbarcador.Codigo, filialArmazem.Codigo);
                decimal estoqueDisponivelAntigo = 0;
                bool adicionado = false;

                if (produtoEmbarcadorEstoqueArmazem != null)
                {
                    estoqueDisponivelAntigo += produtoEmbarcadorEstoqueArmazem.EstoqueDisponivel;
                    produtoEmbarcadorEstoqueArmazem.EstoqueDisponivel = salvarEstoqueProdutoArmazem.QuantidadeProduto;
                    repositorioProdutoEmbarcadorEstoqueArmazem.Atualizar(produtoEmbarcadorEstoqueArmazem);
                }
                else
                {
                    adicionado = true;
                    produtoEmbarcadorEstoqueArmazem = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem
                    {
                        Filial = filial,
                        Produto = produtoEmbarcador,
                        Armazem = filialArmazem,
                        EstoqueDisponivel = salvarEstoqueProdutoArmazem.QuantidadeProduto
                    };
                    repositorioProdutoEmbarcadorEstoqueArmazem.Inserir(produtoEmbarcadorEstoqueArmazem);
                }

                AuditarProdutoEmbarcadorEstoqueArmazem(adicionado, produtoEmbarcadorEstoqueArmazem, estoqueDisponivelAntigo);

                _unitOfWork.CommitChanges();
            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true, "Dados recebidos com sucesso");
        }

        #endregion

        #region Metodos Publicos Auxiliares
        public Dominio.ObjetosDeValor.Embarcador.Filial.Filial ConverterObjetoFilial(Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            if (filial == null)
                return null;

            Servicos.Embarcador.Localidades.Localidade serLocalidade = new Servicos.Embarcador.Localidades.Localidade();
            Dominio.ObjetosDeValor.Embarcador.Filial.Filial dynFilial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial();

            dynFilial.Protocolo = filial.Codigo;
            dynFilial.Ativo = filial.Ativo;
            dynFilial.CNPJ = filial.CNPJ_Formatado;
            dynFilial.CodigoAtividade = filial.Atividade.Codigo;
            dynFilial.CodigoIntegracao = filial.CodigoFilialEmbarcador;
            dynFilial.Descricao = filial.Descricao;
            dynFilial.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
            dynFilial.Endereco.Cidade = serLocalidade.ConverterObjetoLocalidade(filial.Localidade);
            dynFilial.TipoFilial = filial.TipoFilial;

            return dynFilial;
        }

        #endregion

        #region Metodos Privados

        private void AuditarProdutoEmbarcadorEstoqueArmazem(bool adicionado, Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazem produtoEmbarcadorEstoqueArmazem, decimal estoqueDisponivelAntigo)
        {
            Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazemHistorico repProdutoEmbarcadorEstoqueArmazemHistorico = new Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazemHistorico(_unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazemHistorico produtoEmbarcadorEstoqueArmazemHistorico = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazemHistorico
            {
                ProdutoEmbarcadorEstoqueArmazem = produtoEmbarcadorEstoqueArmazem,
                DataAlteracao = DateTime.Now,
                Acao = adicionado ? "Adicionado" : "Atualizado",
                QuantidadeAnterior = estoqueDisponivelAntigo,
                QuantidadeAtualizada = produtoEmbarcadorEstoqueArmazem.EstoqueDisponivel,
                Auditado = $"{_auditado.Integradora.Descricao} - ({_auditado.IP})"
            };

            repProdutoEmbarcadorEstoqueArmazemHistorico.Inserir(produtoEmbarcadorEstoqueArmazemHistorico);
        }


        #endregion
    }
}
