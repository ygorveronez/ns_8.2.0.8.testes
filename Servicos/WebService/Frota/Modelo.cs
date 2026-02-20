using Dominio.Interfaces.Database;
using System.Threading;

namespace Servicos.WebService.Frota
{
    public class Modelo : ServicoBase
    {        
        public Modelo(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public Dominio.Entidades.ModeloVeiculo SalvarModeloVeiculo(Dominio.ObjetosDeValor.Embarcador.Frota.Modelo modeloIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(unitOfWork);
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(unitOfWork);
            Dominio.Entidades.MarcaVeiculo marca = repMarcaVeiculo.BuscarPorCodigoIntegracao(modeloIntegracao.Marca.CodigoIntegracao);
            bool inserirMarca = false;
            if (marca == null)
            {
                marca = new Dominio.Entidades.MarcaVeiculo();
                inserirMarca = true;
            }
            else
            {
                marca.Initialize();
            }
            marca.CodigoIntegracao = modeloIntegracao.Marca.CodigoIntegracao;
            marca.Descricao = modeloIntegracao.Marca.Descricao;
            marca.TipoVeiculo = modeloIntegracao.Marca.TipoVeiculo;
            marca.Status = modeloIntegracao.Marca.Ativo ? "A" : "I";
            if (inserirMarca)
                repMarcaVeiculo.Inserir(marca, auditado);
            else
                repMarcaVeiculo.Atualizar(marca, auditado);

            Dominio.Entidades.ModeloVeiculo modelo = repModeloVeiculo.BuscarPorCodigoIntegracao(modeloIntegracao.CodigoIntegracao);

            bool inserirModelo = false;

            if (modelo == null)
            {
                modelo = new Dominio.Entidades.ModeloVeiculo();
                inserirModelo = true;
            }
            else
            {
                modelo.Initialize();
            }

            modelo.CodigoFIPE = modeloIntegracao.CodigoFIPE;
            modelo.CodigoIntegracao = modeloIntegracao.CodigoIntegracao;
            modelo.Descricao = modeloIntegracao.Descricao;
            modelo.MarcaVeiculo = marca;
            modelo.NumeroEixo = modeloIntegracao.NumeroEixos;
            modelo.Status = modeloIntegracao.Ativo ? "A" : "I";
            if (inserirModelo)
                repModeloVeiculo.Inserir(modelo, auditado);
            else
                repModeloVeiculo.Atualizar(modelo, auditado);

            return modelo;

        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Modelo ConverterObjetoModelo(Dominio.Entidades.ModeloVeiculo modelo)
        {
            if (modelo != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.Modelo modeloIntegracao = new Dominio.ObjetosDeValor.Embarcador.Frota.Modelo();
                modeloIntegracao.Ativo = modelo.Status == "A" ? true : false;
                modeloIntegracao.CodigoFIPE = modelo.CodigoFIPE;
                modeloIntegracao.CodigoIntegracao = modelo.CodigoIntegracao;
                modeloIntegracao.Descricao = modelo.Descricao;
                modeloIntegracao.Marca = ConverterObjetoMarca(modelo.MarcaVeiculo);
                modeloIntegracao.NumeroEixos = modelo.NumeroEixo;
                modeloIntegracao.PossuiArla32 = modelo.PossuiArla32 == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim ? true : false;

                return modeloIntegracao;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Marca ConverterObjetoMarca(Dominio.Entidades.MarcaVeiculo marca)
        {
            if (marca != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.Marca marcaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Frota.Marca();
                marcaIntegracao.Ativo = marca.Status == "A" ? true : false;
                marcaIntegracao.CodigoIntegracao = marca.CodigoIntegracao;
                marcaIntegracao.Descricao = marca.Descricao;
                marcaIntegracao.TipoVeiculo = marca.TipoVeiculo.HasValue ? marca.TipoVeiculo.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao;

                return marcaIntegracao;
            }
            else
            {
                return null;
            }
        }
    }
}
