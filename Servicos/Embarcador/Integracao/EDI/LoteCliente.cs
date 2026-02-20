using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class LoteCliente
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public LoteCliente(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.EDI.LoteCliente.LoteCliente ConverterLoteClienteParaEDI(Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI loteClienteIntegracaoEDI)
        {
            Repositorio.Embarcador.Integracao.LoteCliente repLoteCliente = new Repositorio.Embarcador.Integracao.LoteCliente(_unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente loteCliente = loteClienteIntegracaoEDI.LoteCliente;
            List<Dominio.Entidades.Cliente> clientes = repLoteCliente.BuscarClientesPorLote(loteCliente.Codigo);

            int totalRegistros = 0;
            string extensao = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterExtencaoPadrao(loteClienteIntegracaoEDI.LayoutEDI);
            List<Dominio.ObjetosDeValor.EDI.LoteCliente.Cliente> loteClientes = new List<Dominio.ObjetosDeValor.EDI.LoteCliente.Cliente>();

            foreach (Dominio.Entidades.Cliente cliente in clientes)
            {
                Dominio.ObjetosDeValor.EDI.LoteCliente.Cliente loteClienteEDI = new Dominio.ObjetosDeValor.EDI.LoteCliente.Cliente
                {
                    Nome = cliente.Nome,
                    Pais = cliente.Localidade.Estado?.Pais?.Abreviacao,
                    Estado = cliente.Localidade.Estado.Sigla,
                    CodigoCidade = cliente.Localidade.CodigoIBGE,
                    Cidade = cliente.Localidade.Descricao,
                    Bairro = cliente.Bairro,
                    EnderecoCompleto = cliente.Endereco + ", " + cliente.Numero + " " + cliente.Complemento,
                    Telefone = cliente.Telefone1.ObterSomenteNumeros(),
                    CEP = cliente.CEP.ObterSomenteNumeros(),
                    CnpjCpf = cliente.CPF_CNPJ_SemFormato,
                    Tipo = cliente.Tipo,
                    RaizCnpjCpf = cliente.Tipo == "F" ? cliente.CPF_CNPJ_SemFormato.Substring(0, 10) : cliente.CPF_CNPJ_SemFormato.Substring(0, 8),
                    Filial = cliente.Tipo == "F" ? 0 : cliente.CPF_CNPJ_SemFormato.Substring(8, 4).ToInt(),
                    Digito = cliente.Tipo == "F" ? cliente.CPF_CNPJ_SemFormato.Substring(9, 2).ToInt() : cliente.CPF_CNPJ_SemFormato.Substring(12, 2).ToInt(),
                    InscricaoEstadual = cliente.IE_RG,
                    InscricaoMunicipal = cliente.InscricaoMunicipal,
                    InscricaoSuframa = cliente.InscricaoSuframa,
                    Status = cliente.Ativo ? 1 : 2,
                    CodigoCEI = cliente.NumeroCEI,
                    NumeroPISPASEP = cliente.PISPASEP,
                    EnderecoEletronico = cliente.Email,
                    Logradouro = cliente.Endereco,
                    Numero = cliente.Numero,
                    EnderecoComplementar = cliente.Complemento
                };

                loteClientes.Add(loteClienteEDI);
                totalRegistros++;
            }

            // Preenche cabeçalho
            Dominio.ObjetosDeValor.EDI.LoteCliente.Cabecalho cabecalho = new Dominio.ObjetosDeValor.EDI.LoteCliente.Cabecalho()
            {
                DataGeracao = loteCliente.DataGeracaoLote,
                Sequencia = loteCliente.Numero,
                NomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(loteClienteIntegracaoEDI, null, _unitOfWork).Replace(extensao, ""),
            };

            // Preenche rodapé
            Dominio.ObjetosDeValor.EDI.LoteCliente.Rodape rodape = new Dominio.ObjetosDeValor.EDI.LoteCliente.Rodape()
            {
                Contadores = totalRegistros
            };

            //Preenche EDI
            Dominio.ObjetosDeValor.EDI.LoteCliente.LoteCliente edi = new Dominio.ObjetosDeValor.EDI.LoteCliente.LoteCliente
            {
                Cabecalho = cabecalho,
                Cliente = loteClientes,
                Rodape = rodape
            };

            return edi;
        }

        #endregion
    }
}
