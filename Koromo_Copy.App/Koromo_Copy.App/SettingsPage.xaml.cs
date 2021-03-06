﻿// This source code is a part of Koromo Copy Project.
// Copyright (C) 2019. dc-koromo. Licensed under the MIT Licence.

using Acr.UserDialogs;
using Koromo_Copy.Framework;
using Koromo_Copy.Framework.Setting;
using Koromo_Copy.Framework.Utils;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamEffects;

namespace Koromo_Copy.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            Commands.SetTap(SettingSuperPath, new Command(async () => {
                var promptConfig = new PromptConfig();
                promptConfig.InputType = InputType.Name;
                promptConfig.IsCancellable = true;
                promptConfig.Message = "경로를 입력해주세요.";
                promptConfig.Text = Path.Text;
                promptConfig.OkText = "확인";
                promptConfig.CancelText = "취소";
                var result = await UserDialogs.Instance.PromptAsync(promptConfig);
                if (result.Ok)
                {
                    Path.Text = result.Text;
                    Settings.Instance.Model.SuperPath = result.Text;
                    Settings.Instance.Save();
                }
            }));

            Commands.SetTap(SettingNotify, new Command(() => {
                NotifyToggle.IsToggled = !NotifyToggle.IsToggled;
            }));

            Commands.SetTap(SettingBufferSize, new Command(async () => {
                var promptConfig = new PromptConfig();
                promptConfig.InputType = InputType.Number;
                promptConfig.IsCancellable = true;
                promptConfig.Message = "버퍼 크기를 입력해주세요.";
                promptConfig.Text = Settings.Instance.Network.DownloadBufferSize.ToString();
                promptConfig.OkText = "확인";
                promptConfig.CancelText = "취소";
                var result = await UserDialogs.Instance.PromptAsync(promptConfig);
                if (result.Ok)
                {
                    var value = result.Text.ToInt();
                    Settings.Instance.Network.DownloadBufferSize = value;
                    Settings.Instance.Save();
                    BufferSize.Text = "설정된 크기: " + byte_format(value);
                }
            }));

            Commands.SetTap(SettingPixiv, new Command(async () => {
                await Navigation.PushAsync(new PixivLoginPage());
            }));

            Commands.SetTap(ExportDB, new Command(() => {
                if (!Directory.Exists(AppProvider.DefaultSuperPath))
                    Directory.CreateDirectory(AppProvider.DefaultSuperPath);
                File.Copy(System.IO.Path.Combine(AppProvider.ApplicationPath, "download.db"),
                    System.IO.Path.Combine(AppProvider.DefaultSuperPath, "download.db"), true);
                Plugin.XSnack.CrossXSnack.Current.ShowMessage("데이터베이스를 성공적으로 내보냈습니다.");
            }));

            Commands.SetTap(ExportLog, new Command(() => {
                if (!Directory.Exists(AppProvider.DefaultSuperPath))
                    Directory.CreateDirectory(AppProvider.DefaultSuperPath);
                File.Copy(System.IO.Path.Combine(AppProvider.ApplicationPath, "log.txt"),
                    System.IO.Path.Combine(AppProvider.DefaultSuperPath, "log.txt"), true);
                Plugin.XSnack.CrossXSnack.Current.ShowMessage("로그를 성공적으로 내보냈습니다.");
            }));

            Path.Text = Settings.Instance.Model.SuperPath;
            ThreadCount.Value = Settings.Instance.Model.ThreadCount;
            BufferSize.Text = "설정된 크기: " + byte_format(Settings.Instance.Network.DownloadBufferSize);
            PPThreadCount.Value = Settings.Instance.Model.PostprocessorThreadCount;
        }

        private static string byte_format(int bytes)
        {
            if (bytes > 1024 * 1024)
                return (bytes / 1024.0).ToString("0,0.#") + " MB";
            if (bytes > 1024)
                return (bytes / 1024.0).ToString("0,0.#") + " KB";
            return bytes.ToString("0,0.#") + " Bytes";
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = (int)Math.Round(e.NewValue / 1.0);

            ThreadCount.Value = newStep;
            ThreadCountText.Text = "스레드 수: " + newStep;
            Settings.Instance.Model.ThreadCount = newStep;
            Settings.Instance.Save();
        }

        private void PPSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var newStep = (int)Math.Round(e.NewValue / 1.0);

            PPThreadCount.Value = newStep;
            PPThreadCountText.Text = "스레드 수: " + newStep;
            Settings.Instance.Model.PostprocessorThreadCount = newStep;
            Settings.Instance.Save();
        }
    }
}