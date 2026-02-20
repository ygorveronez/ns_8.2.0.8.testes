using Dominio.Interfaces.Database;
using Repositorio;
using System.Threading;

namespace Servicos.WebService.Frota
{
    public class Abastecimento : ServicoBase
    {        
        public Abastecimento(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        
        public Dominio.ObjetosDeValor.Embarcador.Frota.ConsultaAbastecimento ConverterObjetoAbastecimento(Dominio.Entidades.Abastecimento abastecimento, Repositorio.UnitOfWork unitOfWork)
        {
            if (abastecimento == null)
                return null;

            Servicos.WebService.Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Frota.Modelo serModelo = new Modelo(unitOfWork);
            Servicos.WebService.Carga.ModeloVeicularCarga serConverterObjetoModeloVeicular = new Carga.ModeloVeicularCarga(unitOfWork);
            Servicos.WebService.Empresa.Motorista serMotorista = new Empresa.Motorista(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new Empresa.Empresa(unitOfWork);
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Servicos.WebService.Frota.Veiculo(unitOfWork);            

            Dominio.ObjetosDeValor.Embarcador.Frota.ConsultaAbastecimento consultaAbastecimento = new Dominio.ObjetosDeValor.Embarcador.Frota.ConsultaAbastecimento()
            {
                Codigo = abastecimento.Codigo,
                Data = abastecimento.Data.HasValue ? abastecimento.Data.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                Documento = abastecimento.Documento,
                Equipamento = abastecimento.Equipamento != null ? serWSVeiculo.ConverterObjetoEquipamento(abastecimento.Equipamento, unitOfWork) : null,
                Horimetro = abastecimento.Horimetro,
                Litros = abastecimento.Litros,
                Motorista = abastecimento.Motorista != null ? serMotorista.ConverterObjetoMotorista(abastecimento.Motorista) : null,
                Observacao = abastecimento.Observacao,
                Posto = abastecimento.Posto != null ? serPessoa.ConverterObjetoPessoa(abastecimento.Posto) : null,
                Produto = abastecimento.Produto != null ? new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.Produto() { Codigo = abastecimento.Produto.Codigo, CodigoIntegracao = abastecimento.Produto.CodigoProduto, Descricao = abastecimento.Produto.Descricao, NCM = abastecimento.Produto.CodigoNCM } : null,
                Kilometragem = abastecimento.Kilometragem,
                Situacao = abastecimento.DescricaoSituacao,
                TipoAbastecimento = abastecimento.TipoAbastecimento,
                ValorTotal = abastecimento.ValorTotal,
                ValorUnitario = abastecimento.ValorUnitario,
                Veiculo = abastecimento.Veiculo != null ? serWSVeiculo.ConverterObjetoVeiculo(abastecimento.Veiculo, unitOfWork) : null,
                NumeroAcerto = abastecimento.NumeroAcertos
            };

            return consultaAbastecimento;
        }

    }
}
