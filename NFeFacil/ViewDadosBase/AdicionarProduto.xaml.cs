﻿using NFeFacil.Log;
using NFeFacil.Validacao;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using NFeFacil.ItensBD;
using Windows.UI.Popups;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewDadosBase
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class AdicionarProduto : Page
    {
        private ProdutoDI Produto;
        private ILog Log = Popup.Current;

        public AdicionarProduto()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
            {
                Produto = new ProdutoDI();
                MainPage.Current.SeAtualizar(Symbol.Add, "Produto");
                chkControleEstoque.IsEnabled = false;
            }
            else
            {
                Produto = (ProdutoDI)e.Parameter;
                MainPage.Current.SeAtualizar(Symbol.Edit, "Produto");
                using (var db = new AplicativoContext())
                    chkControleEstoque.IsChecked = db.Estoque.Find(Produto.Id) != null;
            }
            DataContext = Produto;
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new ValidadorProduto(Produto).Validar(Log))
                {
                    using (var db = new AplicativoContext())
                    {
                        Produto.UltimaData = DateTimeNow;
                        if (Produto.Id == Guid.Empty)
                        {
                            db.Add(Produto);
                            Log.Escrever(TitulosComuns.Sucesso, "Produto salvo com sucesso.");
                        }
                        else
                        {
                            db.Update(Produto);
                            Log.Escrever(TitulosComuns.Sucesso, "Produto alterado com sucesso.");
                        }
                        db.SaveChanges();
                    }
                    MainPage.Current.Retornar();
                }
            }
            catch (Exception erro)
            {
                erro.ManipularErro();
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Current.Retornar();
        }

        private async void ControleEstoque_Checked(object sender, RoutedEventArgs e)
        {
            var caixa = new MessageDialog("Essa é uma operação sem volta, um vez adicionado ao controle de estoque este produto será permanentemente parte dele. Certeza que quer isso?", "Atenção");
            caixa.Commands.Add(new UICommand("Sim", x =>
            {
                using (var db = new AplicativoContext())
                {
                    db.Estoque.Add(new Estoque() { Id = Produto.Id });
                    db.SaveChanges();
                }
            }));
            caixa.Commands.Add(new UICommand("Não"));
            await caixa.ShowAsync();
        }

        private void ControleEstoque_Unchecked(object sender, RoutedEventArgs e)
        {
            Log.Escrever(TitulosComuns.Log, "Não é possível remover um produto do controle de estoque.");
            chkControleEstoque.IsChecked = true;
        }
    }
}
