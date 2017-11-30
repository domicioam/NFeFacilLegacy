﻿using NFeFacil.ItensBD;
using NFeFacil.Controles;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewDadosBase
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class GerenciarDadosBase : Page, IHambuguer
    {
        public GerenciarDadosBase()
        {
            InitializeComponent();
            using (var db = new AplicativoContext())
            {
                Clientes = db.Clientes.Where(x => x.Ativo).OrderBy(x => x.Nome).GerarObs();
                Motoristas = db.Motoristas.Where(x => x.Ativo).OrderBy(x => x.Nome).GerarObs();
                Produtos = db.Produtos.Where(x => x.Ativo).OrderBy(x => x.Descricao).GerarObs();
                Compradores = db.Compradores.Where(x => x.Ativo).OrderBy(x => x.Nome).GerarObs();

                var vendedores = db.Vendedores.Where(x => x.Ativo).ToArray();
                var imagens = db.Imagens;
                var quantVendedores = vendedores.Length;
                var conjuntos = new ObservableCollection<ExibicaoVendedor>();
                for (int i = 0; i < quantVendedores; i++)
                {
                    var atual = vendedores[i];
                    var novoConjunto = new ExibicaoVendedor
                    {
                        Id = atual.Id,
                        Principal = atual.Nome,
                        Secundario = atual.CPF.ToString("000,000,000-00"),
                        Vendedor = atual
                    };
                    var img = imagens.Find(atual.Id);
                    if (img != null && img.Bytes != null)
                    {
                        var task = img.GetSourceAsync();
                        task.Wait();
                        novoConjunto.Imagem = task.Result;
                    }
                    conjuntos.Add(novoConjunto);
                }
                Vendedores = conjuntos.OrderBy(x => x.Principal).GerarObs();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPage.Current.SeAtualizar(Symbol.Manage, "Dados base");
        }

        public ObservableCollection<ItemHambuguer> ConteudoMenu => new ObservableCollection<ItemHambuguer>
        {
            new ItemHambuguer(Symbol.People, "Clientes"),
            new ItemHambuguer(new Uri(GetUriVolante()), "Motoristas"),
            new ItemHambuguer(Symbol.Shop, "Produtos"),
            new ItemHambuguer(Symbol.People, "Vendedores"),
            new ItemHambuguer(Symbol.People, "Compradores")
        };

        string GetUriVolante()
        {
            var usarDark = Application.Current.RequestedTheme == ApplicationTheme.Dark;
            return usarDark ? "ms-appx:///Assets/VolanteDark.png" : "ms-appx:///Assets/Volante.png";
        }

        ObservableCollection<ClienteDI> Clientes { get; }
        ObservableCollection<MotoristaDI> Motoristas { get; }
        ObservableCollection<ProdutoDI> Produtos { get; }
        ObservableCollection<ExibicaoVendedor> Vendedores { get; }
        ObservableCollection<Comprador> Compradores { get; }

        public void AtualizarMain(int index) => flipView.SelectedIndex = index;

        private void TelaMudou(object sender, SelectionChangedEventArgs e)
        {
            var index = ((FlipView)sender).SelectedIndex;
            MainPage.Current.AlterarSelectedIndexHamburguer(index);
        }

        async void AdicionarCliente(object sender, RoutedEventArgs e)
        {
            var caixa = new EscolherTipoCliente();
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                switch (caixa.TipoCliente)
                {
                    case 0:
                        MainPage.Current.Navegar<AdicionarClienteBrasileiro>();
                        break;
                    case 1:
                        MainPage.Current.Navegar<AdicionarClienteBrasileiroPFContribuinte>();
                        break;
                    case 2:
                        MainPage.Current.Navegar<AdicionarClienteBrasileiroPJ>();
                        break;
                    case 3:
                        MainPage.Current.Navegar<AdicionarClienteEstrangeiro>();
                        break;
                }
            }
        }

        private void EditarCliente(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var dest = (ClienteDI)contexto;

            if (!string.IsNullOrEmpty(dest.CPF))
            {
                if (dest.IndicadorIE == 1)
                {
                    MainPage.Current.Navegar<AdicionarClienteBrasileiroPFContribuinte>(dest);
                }
                else
                {
                    MainPage.Current.Navegar<AdicionarClienteBrasileiro>(dest);
                }
            }
            else if (!string.IsNullOrEmpty(dest.CNPJ))
            {
                MainPage.Current.Navegar<AdicionarClienteBrasileiroPJ>(dest);
            }
            else
            {
                MainPage.Current.Navegar<AdicionarClienteEstrangeiro>(dest);
            }
        }

        private void InativarCliente(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var dest = (ClienteDI)contexto;

            using (var db = new AplicativoContext())
            {
                dest.Ativo = false;
                db.Update(dest);
                db.SaveChanges();
                Clientes.Remove(dest);
            }
        }

        private void AdicionarMotorista(object sender, RoutedEventArgs e)
        {
            MainPage.Current.Navegar<AdicionarMotorista>();
        }

        private void EditarMotorista(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            MainPage.Current.Navegar<AdicionarMotorista>((MotoristaDI)contexto);
        }

        private void InativarMotorista(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var mot = (MotoristaDI)contexto;

            using (var db = new AplicativoContext())
            {
                mot.Ativo = false;
                db.Update(mot);
                db.SaveChanges();
                Motoristas.Remove(mot);
            }
        }

        private void AdicionarProduto(object sender, RoutedEventArgs e)
        {
            MainPage.Current.Navegar<AdicionarProduto>();
        }

        private void EditarProduto(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            MainPage.Current.Navegar<AdicionarProduto>((ProdutoDI)contexto);
        }

        async void ControlarEstoque(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var produto = (ProdutoDI)contexto;
            using (var db = new AplicativoContext())
            {
                if (db.Estoque.Count(x => x.Id == produto.Id) > 0)
                {
                    Log.Popup.Current.Escrever(Log.TitulosComuns.Atenção, "O produto já foi adicionado ao controle de estoque.");
                }
                else
                {
                    var caixa = new MessageDialog("Essa é uma operação sem volta, uma vez adicionado ao controle de estoque este produto será permanentemente parte dele. Certeza que você realmente quer isso?", "Atenção");
                    caixa.Commands.Add(new UICommand("Sim", x =>
                    {
                        db.Estoque.Add(new Estoque() { Id = produto.Id });
                        db.SaveChanges();
                    }));
                    caixa.Commands.Add(new UICommand("Não"));
                    await caixa.ShowAsync();
                }
            }
        }

        private void InativarProduto(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var prod = (ProdutoDI)contexto;

            using (var db = new AplicativoContext())
            {
                prod.Ativo = false;
                db.Update(prod);
                db.SaveChanges();
                Produtos.Remove(prod);
            }
        }

        private void AdicionarVendedor(object sender, RoutedEventArgs e)
        {
            MainPage.Current.Navegar<AdicionarVendedor>();
        }

        private void EditarVendedor(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            MainPage.Current.Navegar<AdicionarVendedor>(((ExibicaoVendedor)contexto).Vendedor);
        }

        private void InativarVendedor(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var exib = (ExibicaoVendedor)contexto;

            var vend = exib.Vendedor;
            using (var db = new AplicativoContext())
            {
                vend.Ativo = false;
                db.Update(vend);
                db.SaveChanges();
                Vendedores.Remove(exib);
            }
        }

        async void ImagemVendedor(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var vend = (ExibicaoVendedor)contexto;
            var caixa = new View.DefinirImagem(vend.Id, vend.Imagem);
            if (await caixa.ShowAsync() == ContentDialogResult.Primary)
            {
                var index = Vendedores.IndexOf(vend);
                vend.Imagem = caixa.Imagem;
                Vendedores[index] = vend;
            }
        }

        private void AdicionarComprador(object sender, RoutedEventArgs e)
        {
            MainPage.Current.Navegar<AdicionarComprador>();
        }

        private void EditarComprador(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            MainPage.Current.Navegar<AdicionarComprador>((Comprador)contexto);
        }

        private void InativarComprador(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var compr = (Comprador)contexto;

            using (var db = new AplicativoContext())
            {
                compr.Ativo = false;
                db.Update(compr);
                db.SaveChanges();
                Compradores.Remove(compr);
            }
        }

        sealed class ExibicaoVendedor : ConjuntoBasicoExibicao
        {
            internal Vendedor Vendedor { get; set; }
        }
    }
}
