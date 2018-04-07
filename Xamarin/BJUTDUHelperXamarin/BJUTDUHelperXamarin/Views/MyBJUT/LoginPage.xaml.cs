using BJUTDUHelperXamarin.Controls;
using BJUTDUHelperXamarin.Models.MyBJUT;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Globalization;
using System.Windows.Input;
using System.Collections;

namespace BJUTDUHelperXamarin.Views.MyBJUT
{
    public partial class LoginPage : ContentPage
    {
        public static LoginPage Instance;
        public LoginPage()
        {
            Instance = this;

            InitializeComponent();

            this.btnToRetrieve.Clicked += BtnToRetrieve_Clicked;
            this.btnToRegist.Clicked += BtnToRegist_Clicked;
            this.btnBackLogin.Clicked += BtnBackLogin_Clicked;
            this.btnBackLogin_Retrieve.Clicked += BtnBackLogin_Retrieve_Clicked;

            this.tbRegPassword.TextChanged += TbRegPassword_TextChanged;
            this.tbRegPasswordConfirm.TextChanged += TbRegPassword_TextChanged;
        }


        private void TbRegPassword_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tbRegPasswordConfirm.Text) && !string.IsNullOrWhiteSpace(this.tbRegPassword.Text))
            {
                return;
            }
            if (this.tbRegPassword.Text != this.tbRegPasswordConfirm.Text)
            {
                this.borderRegPasswordConfirm.IsVisible = true;
            }
            else
            {
                this.borderRegPasswordConfirm.IsVisible = false;
            }
        }
        private void BtnToRegist_Clicked(object sender, EventArgs e)
        {
            SetPageType(PageType.注册);
        }
        private void BtnToRetrieve_Clicked(object sender, EventArgs e)
        {
            SetPageType(PageType.重置密码);
        }

        private void BtnBackLogin_Clicked(object sender, EventArgs e)
        {
            SetPageType(PageType.登录);
        }
        private void BtnBackLogin_Retrieve_Clicked(object sender, EventArgs e)
        {
            SetPageType(PageType.登录);
        }

        public enum PageType
        {
            登录,
            注册,
            重置密码
        }
        public void SetPageType(PageType type)
        {
            switch (type)
            {
                case PageType.登录:
                    this.gridLogin.IsVisible = true;
                    this.gridRegist.IsVisible = false;
                    this.gridRetrieve.IsVisible = false;
                    this.Title = "登录";
                    break;
                case PageType.注册:

                    this.gridLogin.IsVisible = false;
                    this.gridRegist.IsVisible = true;
                    this.gridRetrieve.IsVisible = false;
                    this.Title = "注册";
                    break;
                case PageType.重置密码:
                    this.gridLogin.IsVisible = false;
                    this.gridRegist.IsVisible = false;
                    this.gridRetrieve.IsVisible = true;
                    this.Title = "重置密码";
                    break;
                default:
                    break;
            }
        }
    }
   
}
