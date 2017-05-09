﻿using BibliotecaCentral.ItensBD;
using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes;
using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesProduto;
using BibliotecaCentral.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesTransporte;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BibliotecaCentral.Repositorio
{
    internal sealed class MudancaOtimizadaBancoDados : ConexaoBanco
    {
        internal MudancaOtimizadaBancoDados() : base() { }
        internal MudancaOtimizadaBancoDados(AplicativoContext contexto) : base(contexto) { }

        internal void AdicionarEmitentes(IEnumerable<Emitente> emitentes)
        {
            var analise = from emit in emitentes
                          group emit by Contexto.Emitentes.Count(x => x.CNPJ == emit.CNPJ) == 0;
            var repo = new Emitentes(Contexto);
            foreach (var item in analise)
            {
                if (item.Key)
                {
                    foreach (var subitem in item)
                    {
                        repo.Adicionar(subitem);
                    }
                }
                else
                {
                    foreach (var subitem in item)
                    {
                        repo.Atualizar(subitem);
                    }
                }
            }
        }

        internal void AdicionarClientes(IEnumerable<Destinatario> clientes)
        {
            var analise = from cli in clientes
                          group cli by Contexto.Clientes.Count(x => x.Documento == cli.Documento) == 0;
            var repo = new Clientes(Contexto);
            foreach (var item in analise)
            {
                if (item.Key)
                {
                    foreach (var subitem in item)
                    {
                        repo.Adicionar(subitem);
                    }
                }
                else
                {
                    foreach (var subitem in item)
                    {
                        repo.Atualizar(subitem);
                    }
                }
            }
        }

        internal void AdicionarMotoristas(IEnumerable<Motorista> motoristas)
        {
            var analise = from mot in motoristas
                          group mot by Contexto.Motoristas.Count(x => x.Documento == mot.Documento) == 0;
            var repo = new Motoristas(Contexto);
            foreach (var item in analise)
            {
                if (item.Key)
                {
                    foreach (var subitem in item)
                    {
                        repo.Adicionar(subitem);
                    }
                }
                else
                {
                    foreach (var subitem in item)
                    {
                        repo.Atualizar(subitem);
                    }
                }
            }
        }

        internal void AdicionarProdutos(IEnumerable<BaseProdutoOuServico> produtos)
        {
            var analise = from prod in produtos
                          group prod by Contexto.Produtos.Count(x => x.Descricao == prod.Descricao) == 0;
            var repo = new Produtos(Contexto);
            foreach (var item in analise)
            {
                if (item.Key)
                {
                    foreach (var subitem in item)
                    {
                        repo.Adicionar(subitem);
                    }
                }
                else
                {
                    foreach (var subitem in item)
                    {
                        repo.Atualizar(subitem);
                    }
                }
            }
        }

        internal async Task AdicionarNotasFiscais(IDictionary<NFeDI, XElement> notas)
        {
            var analise = from nota in notas
                          group nota by Contexto.NotasFiscais.Count(x => x.Id == nota.Key.Id) == 0;
            var repo = new NotasFiscais(Contexto);
            foreach (var item in analise)
            {
                if (item.Key)
                {
                    foreach (var subitem in item)
                    {
                        await repo.Adicionar(subitem.Key, subitem.Value);
                    }
                }
                else
                {
                    foreach (var subitem in item)
                    {
                        await repo.Atualizar(subitem.Key, subitem.Value);
                    }
                }
            }
        }
    }
}
